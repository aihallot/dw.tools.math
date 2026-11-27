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
Need((string?)backlog["plan_version"] == "0.1.173" && (string?)backlog["status"] == "in_progress", "Aggregate target differs.");
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
Need(canonical.Count == 636 && current.Length == 17 && current.All(x => (string?)x["status"] == ((string?)x["id"] is "M10" or "M10-W01" or "M10-W01-C01" or "M10-W01-C01-T1" or "M10-W01-C01-T1-R" ? "in_progress" : "planned") && x["evidence"]!.AsArray().Count == 0) &&
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
Need(native.Count == 630 && newNative.Length == 17 && newNative.All(x => (string?)x["state"] == ((string?)x["id"] switch { "M10" or "M10-W01" or "M10-W01-C01" or "M10-W01-C01-T1" => "in-progress", "M10-W01-C01-T1-R" => "done", "M10-W01-C01-T1-G" => "ready", _ => "not-ready" })) &&
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
Console.WriteLine("MATH-RS368-M10-READINESS|RS367=corrected|priorRefusals=2|historicalNativeReady=5|remainingNotReady=12|sourceProjects=13|testProjects=11|migrationExecuted=false|publication=false|PASS");

var postM11Report = Read(".aura/workflow/reports/RS398/attempt-001.json");
var postM11Proof = Read(".workflow/runs/RS398/preparation-proof.json");
string[] postM11Ids = ["math-rs398-m11-canonical-reconciliation","math-rs398-native-roadmap-parity","math-rs398-m10-readiness-preserved","math-rs398-m9-admission","math-rs398-published-handoff","math-rs398-request-reconciliation","math-rs398-m8-evidence-frontier","math-rs398-planning-render-check","diff-check"];
Need((string?)postM11Report["runId"] == "RS398" && (string?)postM11Report["outcome"] == "succeeded" &&
    (int?)postM11Report["attempt"] == 1 && (bool?)postM11Report["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)postM11Report["priorInvocationFailures"]?["count"] == 0 &&
    (string?)postM11Report["executor"]?["toolVersion"] == "0.1.81" &&
    (string?)postM11Report["executor"]?["assemblySha256"] == "f6e91746d8ec3ba2ea3d0586658749c440814080499832cfd6256805e054320a" &&
    postM11Report["boundaryViolations"]!.AsArray().Count == 0 &&
    postM11Report["progress"]!["transitions"]!.AsArray().Count == 0 &&
    postM11Report["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)postM11Proof["sourceCommit"] == "b94ca692048192af9a2faf7afb26a407e738f49c" &&
    (bool?)postM11Proof["payloadCompiled"] == true && (int?)postM11Proof["rehearsedValidationCount"] == 9 &&
    postM11Proof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(postM11Ids) &&
    postM11Report["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(postM11Ids),
    "RS398 durable canonical reconciliation differs.");
Need(canonical.Count(x => ((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)) == 22 &&
    canonical.Where(x => ((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)).All(x => (string?)x["status"] == "done") &&
    native.Count(x => ((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)) == 22 &&
    native.Where(x => ((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)).All(x => (string?)x["state"] == "done"),
    "M11 completion differs.");
var inventory = Read("docs/engineering/m10-post-m11-project-inventory-rs399.json");
var expectedProjects = inventory["projects"]!.AsArray().Select(x => (string)x!).ToArray();
var observedProjects = new[] { "src", "tests", "tools", "samples", "docs" }
    .SelectMany(root => Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories))
    .Select(path => path.Replace('\\', '/'))
    .Where(path => !path.Split('/').Any(part => part is "bin" or "obj" or "build" or ".git"))
    .OrderBy(path => path, StringComparer.Ordinal).ToArray();
Need((string?)inventory["sourceCommit"] == "1b68b26493f248815a7964f8b7d5481bfd2fb7a6" &&
    (string?)inventory["status"] == "inventory-only-classification-pending" &&
    expectedProjects.Length == 123 && observedProjects.SequenceEqual(expectedProjects, StringComparer.Ordinal),
    "Post-M11 project inventory differs.");
foreach (var path in observedProjects)
{
    var xml = XDocument.Load(path);
    string Values(string name) => string.Join(",", xml.Descendants().Where(x => x.Name.LocalName == name).Select(x => x.Value.Trim()));
    Console.WriteLine("MATH-RS399-PROJECT|path=" + path + "|tfm=" + Values("TargetFramework") + "|tfms=" + Values("TargetFrameworks") + "|lang=" + Values("LangVersion") + "|lock=" + File.Exists(Path.Combine(Path.GetDirectoryName(path)!, "packages.lock.json")));
}
Need(File.Exists("docs/planning/decisions/m10-post-m11-baseline-refresh-rs399.md"), "Post-M11 baseline decision missing.");
Console.WriteLine("MATH-RS399-M10-BASELINE-REFRESH|projects=123|m11Done=22|classification=red-initial|migration=false|publication=false|PASS");

var redReport = Read(".aura/workflow/reports/RS400/attempt-001.json");
var redProof = Read(".workflow/runs/RS400/preparation-proof.json");
string[] redIds = ["math-rs400-m10-cutover-readiness-red","math-rs400-m11-canonical-reconciliation","math-rs400-native-roadmap-parity","math-rs400-m10-red-activation","math-rs400-m9-admission","math-rs400-published-handoff","math-rs400-request-reconciliation","math-rs400-m8-evidence-frontier","math-rs400-planning-render-check","diff-check"];
string[] redActivation = ["M10", "M10-W01", "M10-W01-C01", "M10-W01-C01-T1", "M10-W01-C01-T1-R"];
var redFailures = redReport["priorInvocationFailures"]!;
Need((string?)redReport["runId"] == "RS400" && (string?)redReport["outcome"] == "succeeded" &&
    (int?)redReport["attempt"] == 1 && (string?)redReport["executor"]?["toolVersion"] == "0.1.81" &&
    (string?)redReport["executor"]?["assemblySha256"] == "f6e91746d8ec3ba2ea3d0586658749c440814080499832cfd6256805e054320a" &&
    (int?)redFailures["count"] == 1 && (int?)redFailures["preparationFailureCount"] == 1 &&
    (int?)redFailures["allocatedAttemptFailureCount"] == 0 && (bool?)redFailures["firstPassSucceeded"] == false &&
    (string?)redFailures["invocationIds"]![0] == "run-next-20261127T165304Z-254039219c9272aa" &&
    redReport["boundaryViolations"]!.AsArray().Count == 0 &&
    redReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    redReport["progress"]!["transitions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(redActivation) &&
    redReport["progress"]!["transitions"]!.AsArray().All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    (string?)redProof["sourceCommit"] == "2d165e6b3c48169505d48f70e0a214bc41cb9515" &&
    (bool?)redProof["payloadCompiled"] == true && (int?)redProof["rehearsedValidationCount"] == 10 &&
    redProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(redIds) &&
    redReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(redIds),
    "RS400 durable corrected RED evidence differs.");
var redLedger = Read("docs/engineering/m10-cutover-readiness-red-rs400.json");
Need((string?)redLedger["status"] == "red-unqualified" && redLedger["projects"]!.AsArray().Count == 123 &&
    redLedger["rollbackInputs"]!.AsArray().Count == 28 &&
    new[] { "targetRestoreVerified", "targetTestsVerified", "rollbackVerified", "classificationResolved" }
        .All(key => (bool?)redLedger["readiness"]?[key] == false) &&
    File.Exists("docs/planning/decisions/m10-cutover-readiness-red-closure-rs401.md"), "RED scope or unresolved qualification boundary differs.");
Console.WriteLine("MATH-RS401-M10-RED-CLOSURE|priorRefusals=1|allocatedFailures=0|projects=123|rollbackInputs=28|nativeRed=done|green=ready|migration=false|PASS");
