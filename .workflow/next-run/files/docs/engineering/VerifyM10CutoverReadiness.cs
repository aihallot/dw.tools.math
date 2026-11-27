#:property PublishAot=false
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 independent readiness: " + message); }
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Git(params string[] args)
{
    using var p = new Process { StartInfo = new ProcessStartInfo("git") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true } };
    foreach (var arg in args) p.StartInfo.ArgumentList.Add(arg);
    Need(p.Start(), "Cannot start git.");
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    if (!p.WaitForExit(30000)) { p.Kill(entireProcessTree: true); throw new TimeoutException("Git inspection timed out."); }
    Need(p.ExitCode == 0, stderr.GetAwaiter().GetResult());
    return stdout.GetAwaiter().GetResult().Trim();
}
var ledger = Read("docs/engineering/m10-cutover-readiness-verification-rs409.json");
var red = Read("docs/engineering/m10-cutover-readiness-red-rs400.json");
var dispositions = Read("docs/engineering/m10-project-dispositions-rs402.json");
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project").Select(x => ((string)x.Attribute("Path")!).Replace('\\','/')).ToHashSet(StringComparer.Ordinal);
var inventory = dispositions["projects"]!.AsArray().ToDictionary(x => (string)x!["path"]!, x => (string)x!["disposition"]!, StringComparer.Ordinal);
var redBlobs = red["projects"]!.AsArray().ToDictionary(x => (string)x!["path"]!, x => (string)x!["blob"]!, StringComparer.Ordinal);
Need(inventory.Count == 123 && redBlobs.Count == 123 && redBlobs.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(inventory.Keys), "Inventory mismatch.");
Need(solution.Count == 24 && solution.SetEquals(inventory.Where(x => x.Value == "migrate-solution").Select(x => x.Key)), "Solution membership mismatch.");
var tools = new[] { "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj", "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj" };
Need(tools.ToHashSet(StringComparer.Ordinal).SetEquals(inventory.Where(x => x.Value == "migrate-maintained-tool").Select(x => x.Key)), "Maintained tool membership mismatch.");
var history = inventory.Where(x => x.Value.StartsWith("preserve-", StringComparison.Ordinal)).ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
Need(history.Count == 97, "Preservation membership mismatch.");
var changes = solution.Select(x => x[..x.LastIndexOf('/')] + "/packages.lock.json").Concat(new[] {
    "global.json", "Directory.Build.props", tools[0], "tools/dw.tools.math.nuget-preflight/Program.cs" }).ToHashSet(StringComparer.Ordinal);
var retained = new[] { "Directory.Packages.props", "dw.tools.math.slnx", "NuGet.config" }.ToHashSet(StringComparer.Ordinal);
var rollback = changes.Concat(retained.Where(x => x != "NuGet.config")).ToHashSet(StringComparer.Ordinal);
Need(changes.Count == 28 && rollback.Count == 30 && !changes.Overlaps(history.Keys), "Cutover boundary mismatch.");
var report = Read(".aura/workflow/reports/RS407/attempt-001.json");
var output = (string)report["validations"]!.AsArray().Single()!["StandardOutput"]!;
var restored = output.Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.StartsWith("MATH-RS407-RESTORED|", StringComparison.Ordinal))
    .Select(x => x.Split('|')[1]["path=".Length..]).ToHashSet(StringComparer.Ordinal);
Need(restored.SetEquals(rollback), "Rollback and cutover scope mismatch.");
const string baseline = "6ee011ca551c696918a020f5abf1f8ac5df0527a";
const string candidate = "f79b6917884841673d5523296a64f9298e0c0a8e0ee06ac7003730e8dd8cee6e";
var inspector = Read(".aura/workflow/reports/RS406/attempt-002.json");
Need(((string)inspector["validations"]!.AsArray().Single()!["StandardOutput"]!).Contains("MATH-RS406-CANDIDATE|sourceSha256=" + candidate + "|production=false", StringComparison.Ordinal), "Candidate evidence mismatch.");
var tree = Git("ls-tree", "-r", "HEAD").Split('\n').Where(x => x.Contains('\t')).ToDictionary(
    x => x[(x.IndexOf('\t') + 1)..].TrimEnd('\r'), x => x[..x.IndexOf('\t')].Split(' ')[2], StringComparer.Ordinal);
foreach (var path in history.Keys)
    Need(tree.TryGetValue(path, out var blob) && blob == redBlobs[path] && Git("hash-object", "--path=" + path, path) == blob, "Historical project drift: " + path);
foreach (var path in solution.Select(x => x[..x.LastIndexOf('/')] + "/packages.lock.json"))
{
    var deps = Read(path)["dependencies"]!.AsObject();
    Need(deps.Count == 1 && deps["net10.0"] is JsonObject, "Baseline lock target mismatch: " + path);
}
string? Refusal(JsonObject p)
{
    if ((int?)p["schemaVersion"] != 1 || (string?)p["status"] != "bounded-readiness-verification-candidate" ||
        (string?)p["baselineCommit"] != baseline) return "baseline";
    if ((string?)p["target"]?["sdk"] != "11.0.100" || (string?)p["target"]?["tfm"] != "net11.0" ||
        (string?)p["target"]?["language"] != "15.0") return "target";
    if ((int?)p["scope"]?["solutionProjects"] != 24 || (int?)p["scope"]?["maintainedTools"] != 2 ||
        (int?)p["scope"]?["historicalProjects"] != 97 || (int?)p["scope"]?["rollbackInputs"] != 30) return "scope";
    var delta = p["mutationInputs"]!.AsArray();
    if (delta.Count != 28 || !changes.SetEquals(delta.Select(x => (string)x!))) return "mutation";
    var keep = p["retainedInputs"]!.AsArray();
    if (keep.Count != 3 || !retained.SetEquals(keep.Select(x => (string)x!))) return "retained";
    var rows = p["historicalProjects"]!.AsArray();
    if (rows.Count != 97 || !history.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(rows.Select(x => (string)x!["path"]!))) return "history";
    foreach (var row in rows)
    {
        var path = (string)row!["path"]!;
        if ((string?)row["disposition"] != history[path] || (string?)row["baselineBlob"] != redBlobs[path]) return "history-identity";
    }
    if ((string?)p["inspectorCandidateSha256"] != candidate) return "candidate";
    if ((bool?)p["productionMigration"] != false || (bool?)p["publication"] != false || (bool?)p["linuxQualified"] != false) return "premature-claim";
    return null;
}
Need(Refusal(ledger) is null, "Cutover contract differs.");
void Negative(string expected, Action<JsonObject> edit)
{
    var copy = ledger.DeepClone().AsObject();
    edit(copy);
    Need(Refusal(copy) == expected, "Negative control did not refuse " + expected);
    Console.WriteLine("MATH-RS409-NEGATIVE|case=" + expected + "|PASS");
}
Negative("baseline", x => x["baselineCommit"] = new string('0',40));
Negative("target", x => x["target"]!["sdk"] = "10.0.404");
Negative("scope", x => x["scope"]!["historicalProjects"] = 96);
Negative("mutation", x => x["mutationInputs"]!.AsArray().RemoveAt(0));
Negative("retained", x => x["retainedInputs"]!.AsArray().RemoveAt(0));
Negative("history", x => x["historicalProjects"]!.AsArray().RemoveAt(0));
Negative("history-identity", x => x["historicalProjects"]![0]!["baselineBlob"] = new string('0',40));
Negative("candidate", x => x["inspectorCandidateSha256"] = new string('0',64));
Negative("premature-claim", x => x["publication"] = true);
Console.WriteLine("MATH-RS409-INDEPENDENT|solution=24|tools=2|historical=97|mutationInputs=28|rollbackInputs=30|negative=9|production=false|publication=false|PASS");
