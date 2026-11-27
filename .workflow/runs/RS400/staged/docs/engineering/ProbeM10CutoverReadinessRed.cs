#:property PublishAot=false
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Xml.Linq;
if (args.Length != 1 || args[0] != "--red") throw new ArgumentException("Expected --red.");
static void Need(bool condition, string message) { if (!condition) throw new InvalidDataException("M10 RED: " + message); }
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Role(string path) => path.StartsWith("src/projects/", StringComparison.Ordinal) ? "production" :
    path.StartsWith("tests/projects/", StringComparison.Ordinal) ? "solution-test" :
    path.StartsWith("tools/", StringComparison.Ordinal) ? "tool-review" :
    path.StartsWith("tests/consumers/", StringComparison.Ordinal) ? "consumer-preserve" :
    path.StartsWith("tests/providers/", StringComparison.Ordinal) ? "provider-preserve" :
    path.StartsWith("samples/", StringComparison.Ordinal) ? "sample-preserve" : "historical-preserve";
var ledger = Read("docs/engineering/m10-cutover-readiness-red-rs400.json");
var inventory = Read("docs/engineering/m10-post-m11-project-inventory-rs399.json");
var paths = inventory["projects"]!.AsArray().Select(x => (string)x!).OrderBy(x => x, StringComparer.Ordinal).ToArray();
var observed = new[] { "src", "tests", "tools", "samples", "docs" }
    .SelectMany(root => Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories))
    .Select(path => path.Replace('\\', '/'))
    .Where(path => !path.Split('/').Any(part => part is "bin" or "obj" or "build" or ".git"))
    .OrderBy(path => path, StringComparer.Ordinal).ToArray();
Need(paths.Length == 123 && paths.SequenceEqual(observed), "Project membership drift.");
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solution.SequenceEqual(paths.Where(p => Role(p) is "production" or "solution-test")), "Actual solution membership differs.");
var rollbackPaths = new[] { "global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx" }
    .Concat(solution.Select(path => path[..path.LastIndexOf('/')] + "/packages.lock.json"))
    .OrderBy(x => x, StringComparer.Ordinal).ToArray();
using var git = new Process { StartInfo = new ProcessStartInfo("git") {
    UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true } };
foreach (var arg in new[] { "ls-tree", "-r", "HEAD" }) git.StartInfo.ArgumentList.Add(arg);
Need(git.Start(), "Cannot inspect Git baseline.");
var stdout = git.StandardOutput.ReadToEndAsync();
var stderr = git.StandardError.ReadToEndAsync();
if (!git.WaitForExit(30000)) { git.Kill(entireProcessTree: true); throw new TimeoutException("Git tree inspection timed out."); }
Need(git.ExitCode == 0, stderr.GetAwaiter().GetResult());
var gitBlobs = new Dictionary<string, string>(StringComparer.Ordinal);
foreach (var line in stdout.GetAwaiter().GetResult().Split('\n', StringSplitOptions.RemoveEmptyEntries))
{
    var tab = line.IndexOf('\t');
    if (tab < 0) continue;
    var metadata = line[..tab].Split(' ');
    if (metadata.Length == 3 && metadata[1] == "blob")
        gitBlobs[line[(tab + 1)..].TrimEnd('\r')] = metadata[2];
}
string? Evaluate(JsonObject proposal)
{
    if ((int?)proposal["schemaVersion"] != 1 || (string?)proposal["sourceCommit"] != "22600cbca33e5e04c6d42f54ea93e75494823583" ||
        (string?)proposal["status"] != "red-unqualified") return "scope";
    if ((string?)proposal["target"]?["sdk"] != "11.0.100" || (string?)proposal["target"]?["tfm"] != "net11.0" ||
        (string?)proposal["target"]?["language"] != "15.0") return "target";
    var rows = proposal["projects"]!.AsArray();
    if (!rows.Select(x => (string)x!["path"]!).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(paths)) return "roster";
    foreach (var row in rows)
    {
        var path = (string)row!["path"]!;
        if ((string?)row["role"] != Role(path)) return "role";
        if (!gitBlobs.TryGetValue(path, out var blob) || (string?)row["blob"] != blob) return "project-identity";
    }
    var rollback = proposal["rollbackInputs"]!.AsArray();
    if (!rollback.Select(x => (string)x!["path"]!).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(rollbackPaths)) return "rollback-roster";
    foreach (var row in rollback)
    {
        var path = (string)row!["path"]!;
        if (!File.Exists(path) || !gitBlobs.TryGetValue(path, out var blob) || (string?)row["blob"] != blob)
            return "rollback-identity";
    }
    foreach (var key in new[] { "targetRestoreVerified", "targetTestsVerified", "rollbackVerified", "classificationResolved" })
        if ((bool?)proposal["readiness"]?[key] != false) return "premature-qualification";
    return null;
}
Need(Evaluate(ledger) is null, "Baseline RED ledger does not match actual authorities.");
void Negative(string name, string expected, Action<JsonObject> mutate)
{
    var copy = ledger.DeepClone().AsObject();
    mutate(copy);
    Need(Evaluate(copy) == expected, "Negative control failed: " + name);
    Console.WriteLine("MATH-RS400-NEGATIVE|case=" + name + "|refusal=" + expected + "|PASS");
}
Negative("missing-project", "roster", x => x["projects"]!.AsArray().RemoveAt(0));
Negative("duplicate-project", "roster", x => x["projects"]!.AsArray().Add(x["projects"]![0]!.DeepClone()));
Negative("incorrect-role", "role", x => x["projects"]![0]!["role"] = "production");
Negative("wrong-project-blob", "project-identity", x => x["projects"]![0]!["blob"] = new string('0', 40));
Negative("wrong-target", "target", x => x["target"]!["tfm"] = "net10.0");
Negative("missing-rollback-input", "rollback-roster", x => x["rollbackInputs"]!.AsArray().RemoveAt(0));
Negative("wrong-rollback-blob", "rollback-identity", x => x["rollbackInputs"]![0]!["blob"] = new string('0', 40));
Negative("premature-restore-claim", "premature-qualification", x => x["readiness"]!["targetRestoreVerified"] = true);
Negative("premature-rollback-claim", "premature-qualification", x => x["readiness"]!["rollbackVerified"] = true);
Need((string?)Read("global.json")["sdk"]?["version"] == "10.0.401", "Production SDK migrated prematurely.");
var props = XDocument.Load("Directory.Build.props");
Need(props.Descendants("TargetFramework").Single().Value == "net10.0" &&
    props.Descendants("LangVersion").Single().Value == "14.0", "Production framework migrated prematurely.");
Console.WriteLine("MATH-RS400-RED|projects=123|solution=24|toolReview=40|rollbackInputs=28|negative=9|ready=false|migration=false|PASS");
