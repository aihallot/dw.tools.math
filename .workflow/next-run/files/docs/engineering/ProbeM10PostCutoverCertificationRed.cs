#:property PublishAot=false
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--red"]) throw new ArgumentException("Expected --red.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 certification RED: " + message); }
static JsonNode Read(string path) => JsonNode.Parse(File.ReadAllText(path))!;
var contract = Read("docs/engineering/m10-post-cutover-certification-red-rs417.json");
var projects = XDocument.Load("dw.tools.math.slnx").Descendants("Project").Select(x => ((string)x.Attribute("Path")!).Replace('\\','/')).ToArray();
var source = projects.Where(x => x.StartsWith("src/projects/", StringComparison.Ordinal)).ToHashSet(StringComparer.Ordinal);
Need(projects.Length == 24 && source.Count == 13, "Solution roster differs.");
Need((string?)Read("global.json")["sdk"]?["version"] == "11.0.100", "Current SDK pin differs.");
var props = XDocument.Load("Directory.Build.props");
Need(props.Descendants("TargetFramework").Single().Value == "net11.0" && props.Descendants("LangVersion").Single().Value == "15.0", "Current framework/language differs.");
foreach (var path in projects)
{
    var lockPath = Path.Combine(Path.GetDirectoryName(path)!, "packages.lock.json");
    var graph = Read(lockPath)["dependencies"]!.AsObject();
    Need(graph.Count == 1 && graph["net11.0"] is JsonObject, "Lock target differs: " + path);
}
string[] gateIds = ["full-solution-regression", "fresh-net11-candidate-archives", "source-free-locked-consumers",
    "windows-runtime", "linux-wsl-runtime", "archive-preservation-and-rollback"];
string? Refusal(JsonNode c)
{
    if ((int?)c["schemaVersion"] != 1 || (string?)c["status"] != "red-unqualified" ||
        (string?)c["sourceCommit"] != "4fe239713d1edbdc15c445e989e40dd338ba19be" ||
        (string?)c["cutoverClosureCommit"] != "8e47d5864768889dcef688d68fed537f39d9ce14") return "baseline";
    if ((string?)c["target"]?["sdk"] != "11.0.100" || (string?)c["target"]?["tfm"] != "net11.0" ||
        (string?)c["target"]?["language"] != "15.0") return "target";
    var roster = c["sourceProjects"]!.AsArray();
    if (roster.Count != 13 || !source.SetEquals(roster.Select(x => (string)x!))) return "roster";
    var policy = c["candidatePolicy"]!;
    if ((bool?)policy["isolated"] != true || (bool?)policy["exactVersions"] != true ||
        (bool?)policy["sourceFree"] != true || (bool?)policy["immutableHistoricalArchives"] != true ||
        (bool?)policy["publication"] != false) return "boundary";
    var gates = c["requiredGates"]!.AsArray();
    if (gates.Count != gateIds.Length || !gateIds.SequenceEqual(gates.Select(x => (string?)x?["id"]))) return "coverage";
    if ((bool?)c["qualified"] != false || gates.Any(x => (bool?)x?["qualified"] != false ||
        x?["evidence"] is not JsonArray a || a.Count != 0)) return "premature-qualification";
    return null;
}
Need(Refusal(contract) is null, "Invalid RED contract.");
void Negative(string name, string expected, Action<JsonNode> mutate)
{
    var clone = contract.DeepClone();
    mutate(clone);
    Need(Refusal(clone) == expected, "Negative control failed: " + name);
    Console.WriteLine("MATH-RS417-NEGATIVE|case=" + name + "|PASS");
}
Negative("wrong-source", "baseline", x => x["sourceCommit"] = new string('0',40));
Negative("net10-fallback", "target", x => x["target"]!["tfm"] = "net10.0");
Negative("missing-library", "roster", x => x["sourceProjects"]!.AsArray().RemoveAt(0));
Negative("floating-version", "boundary", x => x["candidatePolicy"]!["exactVersions"] = false);
Negative("project-reference-substitute", "boundary", x => x["candidatePolicy"]!["sourceFree"] = false);
Negative("archive-overwrite", "boundary", x => x["candidatePolicy"]!["immutableHistoricalArchives"] = false);
Negative("publication", "boundary", x => x["candidatePolicy"]!["publication"] = true);
Negative("missing-platform-gate", "coverage", x => x["requiredGates"]!.AsArray().RemoveAt(4));
Negative("unproved-linux", "premature-qualification", x => x["requiredGates"]![4]!["qualified"] = true);
Negative("premature-certification", "premature-qualification", x => x["qualified"] = true);
var cutover = Read(".aura/workflow/reports/RS413/attempt-001.json");
var closure = Read(".aura/workflow/reports/RS416/attempt-001.json");
Need((string?)cutover["outcome"] == "succeeded" && (int?)cutover["validations"]![0]!["ExitCode"] == 0 &&
    ((string)cutover["validations"]![0]!["StandardOutput"]!).Contains("MATH-RS413-PRODUCTION|sdk=11.0.100|tfm=net11.0|language=15.0|projects=24|tests=35|tools=2|bindings=4|inspect=13|lockedRestore=PASS|build=PASS|publication=false|PASS", StringComparison.Ordinal) &&
    (string?)closure["outcome"] == "succeeded", "Prior cutover evidence absent.");
Console.WriteLine("MATH-RS417-CERTIFICATION-RED|libraries=13|solution=24|gatesPending=6|negative=10|qualified=false|publication=false|PASS");
