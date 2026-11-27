#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;
using System.Xml.Linq;
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 admission: " + message); }
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
var backlog = Read("docs/planning/backlog.json");
var plan = Read(".aura/workflow/plan/project.json");
var report = Read(".aura/workflow/reports/RS366/attempt-001.json");
var proof = Read(".workflow/runs/RS366/preparation-proof.json");
Need((string?)report["outcome"] == "succeeded" && (string?)report["runId"] == "RS366" &&
    (int?)report["attempt"] == 1 && (bool?)report["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    report["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)proof["payloadCompiled"] == true && (int?)proof["rehearsedValidationCount"] == 8 &&
    (string?)proof["sourceCommit"] == "1da26eb38c68afaabce9b2e38b18bdbb1756e624", "Durable RS366 evidence differs.");
Need((string?)backlog["plan_version"] == "0.1.166" && (string?)backlog["status"] == "in_progress", "Aggregate target differs.");
var canonical = new List<JsonNode>();
foreach (var key in new[] { "releases", "work_packages", "chunks" })
    canonical.AddRange(backlog[key]!.AsArray().Select(x => x!));
foreach (var chunk in backlog["chunks"]!.AsArray())
foreach (var task in chunk!["tasks"]!.AsArray())
{
    canonical.Add(task!);
    canonical.AddRange(task!["subtasks"]!.AsArray().Select(x => x!));
}
var current = canonical.Where(x => ((string)x["id"]!).StartsWith("M10", StringComparison.Ordinal)).ToArray();
Need(canonical.Count == 636 && current.Length == 17 && current.All(x => (string?)x["status"] == "planned" && x["evidence"]!.AsArray().Count == 0) &&
    canonical.Except(current).Where(x => !((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)).All(x => (string?)x["status"] == "done"), "Canonical admission or prior completion differs.");
var native = new List<JsonNode>();
void Walk(JsonNode node)
{
    native.Add(node);
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node[key] is JsonArray children) foreach (var child in children) Walk(child!);
}
foreach (var milestone in plan["milestones"]!.AsArray()) Walk(milestone!);
var newNative = native.Where(x => ((string)x["id"]!).StartsWith("M10", StringComparison.Ordinal)).ToArray();
Need(native.Count == 630 && newNative.Length == 17 && newNative.All(x => (string?)x["state"] == ((string?)x["id"] is "M10" or "M10-W01" or "M10-W01-C01" or "M10-W01-C01-T1" or "M10-W01-C01-T1-R" ? "ready" : "not-ready")) &&
    native.Except(newNative).Where(x => !((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)).All(x => (string?)x["state"] == "done"), "Native admission or prior completion differs.");
Need(current.Select(x => (string?)x["id"]).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(
    newNative.Select(x => (string?)x["id"]).OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal), "M10 roster mismatch.");
Need((string?)Read("global.json")["sdk"]?["version"] == "10.0.401", "Admission changed production SDK.");
var props = XDocument.Load("Directory.Build.props");
Need(props.Descendants("TargetFramework").Single().Value == "net10.0" && props.Descendants("LangVersion").Single().Value == "14.0", "Admission changed production target.");
Need((bool?)Read("docs/engineering/m8-w02-c02-phase-acceptance-rs345.json")["productionMigrationAuthorized"] == false,
    "Historical M8 decision was overwritten.");
var projects = XDocument.Load("dw.tools.math.slnx").Descendants("Project").Select(x => (string)x.Attribute("Path")!).ToArray();
Need(projects.Length == 24 && projects.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13 &&
    projects.Count(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)) == 11 &&
    projects.All(x => File.Exists(x) && File.Exists(Path.Combine(Path.GetDirectoryName(x)!, "packages.lock.json"))), "Current solution inventory differs.");
var admissionReport = Read(".aura/workflow/reports/RS367/attempt-001.json");
var admissionProof = Read(".workflow/runs/RS367/preparation-proof.json");
var admissionIds = admissionReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedAdmissionIds = [
    "math-rs367-m10-admission-and-production-baseline", "math-rs367-canonical-completion-evidence",
    "math-rs367-native-roadmap-parity", "math-rs367-published-handoff", "math-rs367-request-reconciliation",
    "math-rs367-m8-evidence-frontier", "math-rs367-planning-render-check", "diff-check"
];
var failures = admissionReport["priorInvocationFailures"]!;
Need((string?)admissionReport["runId"] == "RS367" && (string?)admissionReport["outcome"] == "succeeded" &&
    (int?)admissionReport["attempt"] == 1 && (string?)admissionReport["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)admissionReport["git"]?["commitMessage"] == "docs: admit M10 production net11 migration" &&
    (int?)failures["count"] == 2 && (int?)failures["preparationFailureCount"] == 2 &&
    (int?)failures["allocatedAttemptFailureCount"] == 0 && (bool?)failures["firstPassSucceeded"] == false &&
    failures["invocationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] {
        "run-next-20261126T211313Z-6b84b19eaa528d69", "run-next-20261126T211657Z-f35c7156243b042f"
    }, StringComparer.Ordinal) &&
    admissionReport["boundaryViolations"]!.AsArray().Count == 0 &&
    admissionReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    admissionReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)admissionProof["runId"] == "RS367" &&
    (string?)admissionProof["sourceCommit"] == "e6b7a13a87c93701b9eb6a45dea4a12372ff30a4" &&
    (bool?)admissionProof["payloadCompiled"] == true && (int?)admissionProof["rehearsedValidationCount"] == 8 &&
    admissionIds.SequenceEqual(expectedAdmissionIds, StringComparer.Ordinal) &&
    admissionProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(admissionIds, StringComparer.Ordinal),
    "Durable RS367 admission evidence or refusal lineage differs.");
Console.WriteLine("MATH-RS368-M10-READINESS|RS367=corrected|priorRefusals=2|nativeReady=5|remainingNotReady=12|sourceProjects=13|testProjects=11|migrationExecuted=false|publication=false|PASS");
