#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 independent production verification: " + message); }
static string Git(params string[] args)
{
    using var p = new Process { StartInfo = new ProcessStartInfo("git") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true } };
    foreach (var arg in args) p.StartInfo.ArgumentList.Add(arg);
    Need(p.Start(), "Git unavailable.");
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    if (!p.WaitForExit(30000)) { p.Kill(entireProcessTree: true); throw new TimeoutException("Git timed out."); }
    Need(p.ExitCode == 0, stderr.GetAwaiter().GetResult());
    return stdout.GetAwaiter().GetResult();
}
static string[] Lines(string s) => s.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimEnd('\r')).ToArray();
static Dictionary<string,string> Tree(string revision) => Lines(Git("ls-tree", "-r", revision)).ToDictionary(
    x => x[(x.IndexOf('\t') + 1)..], x => x[..x.IndexOf('\t')].Split(' ')[2], StringComparer.Ordinal);
static string Sha(string text) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
static JsonNode Read(string path) => JsonNode.Parse(File.ReadAllText(path))!;
const string baseline = "e4bbaa004ae643f2090c6c0010a6587b41e455eb";
const string delivered = "4fe239713d1edbdc15c445e989e40dd338ba19be";
var ledger = Read("docs/engineering/m10-cutover-readiness-verification-rs409.json");
var changes = ledger["mutationInputs"]!.AsArray().Select(x => (string)x!).ToHashSet(StringComparer.Ordinal);
var retained = ledger["retainedInputs"]!.AsArray().Select(x => (string)x!).ToHashSet(StringComparer.Ordinal);
var history = ledger["historicalProjects"]!.AsArray().ToDictionary(x => (string)x!["path"]!, x => (string)x!["baselineBlob"]!, StringComparer.Ordinal);
Need(changes.Count == 28 && retained.Count == 3 && history.Count == 97 && !changes.Overlaps(retained) && !changes.Overlaps(history.Keys), "Qualified roster differs.");
var before = Tree(baseline);
var after = Tree(delivered);
var head = Tree("HEAD");
bool Product(string path) => changes.Contains(path) || retained.Contains(path) || history.ContainsKey(path) ||
    path.StartsWith("src/", StringComparison.Ordinal) || path.StartsWith("tests/", StringComparison.Ordinal) ||
    path.StartsWith("tools/", StringComparison.Ordinal) || path.StartsWith("samples/", StringComparison.Ordinal);
var delta = before.Keys.Union(after.Keys, StringComparer.Ordinal).Where(Product).Where(p =>
    before.GetValueOrDefault(p) != after.GetValueOrDefault(p)).ToHashSet(StringComparer.Ordinal);
Need(delta.SetEquals(changes), "Delivered product delta differs.");
Need(after.Keys.Union(head.Keys, StringComparer.Ordinal).Where(Product).All(p =>
    after.GetValueOrDefault(p) == head.GetValueOrDefault(p)), "Delivered product tree drift.");
Need(!Lines(Git("diff", "--name-only", "--no-renames", delivered, "--")).Any(Product), "Working product drift.");
foreach (var entry in history)
    Need(before[entry.Key] == entry.Value && after[entry.Key] == entry.Value, "Historical identity differs: " + entry.Key);
foreach (var path in retained)
    Need(before[path] == after[path], "Retained authority differs: " + path);
var locks = changes.Where(p => p.EndsWith("/packages.lock.json", StringComparison.Ordinal)).OrderBy(p => p, StringComparer.Ordinal).ToArray();
Need(locks.Length == 24, "Lock count differs.");
foreach (var path in locks)
{
    var original = JsonNode.Parse(Git("show", baseline + ":" + path))!;
    var migrated = Read(path);
    var deps = migrated["dependencies"]!.AsObject();
    Need(deps.Count == 1 && deps["net11.0"] is JsonObject, "Migrated dependency target differs: " + path);
    var graph = deps["net11.0"]!.DeepClone();
    deps.Remove("net11.0");
    deps["net10.0"] = graph;
    Need(JsonNode.DeepEquals(original, migrated), "Reverse dependency graph differs: " + path);
    Console.WriteLine("MATH-RS415-LOCK|path=" + path + "|reverseGraph=PASS");
}
var rollback = Read(".aura/workflow/reports/RS407/attempt-001.json");
Need((string?)rollback["outcome"] == "succeeded" && (int?)rollback["validations"]![0]!["ExitCode"] == 0, "Original rollback evidence absent.");
var rollbackRows = Lines((string)rollback["validations"]![0]!["StandardOutput"]!)
    .Where(x => x.StartsWith("MATH-RS407-RESTORED|", StringComparison.Ordinal)).ToArray();
var originalInputs = changes.Concat(retained.Where(p => p != "NuGet.config")).ToHashSet(StringComparer.Ordinal);
var seen = new HashSet<string>(StringComparer.Ordinal);
foreach (var row in rollbackRows)
{
    var match = System.Text.RegularExpressions.Regex.Match(row, @"^MATH-RS407-RESTORED\|path=(.+)\|sha256=([0-9a-f]{64})\|PASS$");
    Need(match.Success && seen.Add(match.Groups[1].Value), "Invalid rollback receipt.");
    var path = match.Groups[1].Value;
    Need(originalInputs.Contains(path), "Unexpected rollback input: " + path);
    var original = Git("show", baseline + ":" + path);
    var lf = original.Replace("\r\n", "\n", StringComparison.Ordinal);
    var crlf = lf.Replace("\n", "\r\n", StringComparison.Ordinal);
    var expected = match.Groups[2].Value;
    var mode = Sha(original) == expected ? "git-blob-text" : Sha(lf) == expected ? "lf" : Sha(crlf) == expected ? "crlf" : "";
    Need(mode.Length > 0, "Original checkout bytes not recoverable from Git text: " + path);
    Console.WriteLine("MATH-RS415-RECOVERY|path=" + path + "|sha256=" + expected + "|mode=" + mode + "|PASS");
}
Need(rollbackRows.Length == 30 && seen.SetEquals(originalInputs), "Original recovery roster differs.");
Console.WriteLine("MATH-RS415-INDEPENDENT|delta=28|reverseLocks=24|historical=97|retained=3|originalInputs=30|production=net11.0|rollbackBuild=RS407|publication=false|PASS");
