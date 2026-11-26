#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;
using System.Xml.Linq;
static void Need(bool condition, string message) { if (!condition) throw new InvalidDataException("RS370: " + message); }
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
var b = Read("docs/planning/backlog.json");
var p = Read(".aura/workflow/plan/project.json");
var report = Read(".aura/workflow/reports/RS369/attempt-001.json");
var proof = Read(".workflow/runs/RS369/preparation-proof.json");
Need((string?)report["outcome"] == "succeeded" && (int?)report["attempt"] == 1 &&
    (bool?)report["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    report["boundaryViolations"]!.AsArray().Count == 0 &&
    report["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (bool?)proof["payloadCompiled"] == true && (int?)proof["rehearsedValidationCount"] == 5,
    "RS369 durable source inventory evidence differs.");
Need((string?)b["plan_version"] == "0.1.144" && (string?)b["status"] == "in_progress", "Canonical version differs.");
var canonical = new List<JsonNode>();
foreach (var key in new[] { "releases", "work_packages", "chunks" })
    canonical.AddRange(b[key]!.AsArray().Select(x => x!));
foreach (var chunk in b["chunks"]!.AsArray())
foreach (var task in chunk!["tasks"]!.AsArray()) {
    canonical.Add(task!);
    canonical.AddRange(task!["subtasks"]!.AsArray().Select(x => x!));
}
var native = new List<JsonNode>();
void Walk(JsonNode node) {
    native.Add(node);
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node[key] is JsonArray children) foreach (var child in children) Walk(child!);
}
foreach (var milestone in p["milestones"]!.AsArray()) Walk(milestone!);
string Id(JsonNode x) => (string)x["id"]!;
bool IsM11(JsonNode x) => Id(x) == "M11" || Id(x).StartsWith("M11-", StringComparison.Ordinal);
bool IsM10(JsonNode x) => Id(x) == "M10" || Id(x).StartsWith("M10-", StringComparison.Ordinal);
var c11 = canonical.Where(IsM11).ToArray();
var n11 = native.Where(IsM11).ToArray();
Need(canonical.Count == 636 && native.Count == 630 && c11.Length == 22 && n11.Length == 22 &&
    c11.All(x => (string?)x["status"] == "planned" && x["evidence"]!.AsArray().Count == 0) &&
    n11.All(x => (string?)x["state"] == "not-ready") &&
    c11.Select(Id).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(n11.Select(Id).OrderBy(x => x, StringComparer.Ordinal)),
    "New scope roster or unstarted state differs.");
Need(canonical.Count(x => !IsM10(x) && !IsM11(x) && (string?)x["status"] == "done") == 597 &&
    native.Count(x => !IsM10(x) && !IsM11(x) && (string?)x["state"] == "done") == 591,
    "Historical completion frontier differs.");
JsonNode C(string id) => canonical.Single(x => Id(x) == id);
JsonNode N(string id) => native.Single(x => Id(x) == id);
string[] Strings(JsonNode x, string key) => x[key]!.AsArray().Select(v => (string)v!).ToArray();
string[] deps = ["M11-W01-C01", "M11-W01-C02", "M11-W01-C03"];
Need(Strings(C("M11-W01-C04"), "depends_on").SequenceEqual(deps) &&
    Strings(N("M11-W01-C04"), "dependsOn").SequenceEqual(deps) &&
    Strings(C("M10-W01-C02"), "depends_on").SequenceEqual(new[] { "M10-W01-C01", "M11-W01-C04" }) &&
    Strings(N("M10-W01-C02"), "dependsOn").SequenceEqual(new[] { "M10-W01-C01", "M11-W01-C04" }),
    "Candidate qualification or cutover gate differs.");
Need((string?)Read("global.json")["sdk"]?["version"] == "10.0.401" &&
    XDocument.Load("Directory.Build.props").Descendants("TargetFramework").Single().Value == "net10.0" &&
    XDocument.Load("Directory.Build.props").Descendants("LangVersion").Single().Value == "14.0", "Production baseline changed.");
Need(File.Exists("docs/planning/evidence/decision-numeric-source-inventory-rs369.md") &&
    File.Exists("docs/planning/decisions/m11-decision-numeric-subsidiarity-admission-rs370.md"), "Consultation evidence absent.");
Console.WriteLine("MATH-RS370-M11-ADMISSION|newNodes=22|canonical=636|native=630|cutoverGate=M11-W01-C04|newCompletion=false|publication=false|PASS");
