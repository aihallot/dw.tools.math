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
Need((string?)b["plan_version"] == "0.1.162" && (string?)b["status"] == "in_progress", "Canonical version differs.");
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
    c11.All(x => (string?)x["status"] == (Id(x) switch { "M11" or "M11-W01" => "in_progress", "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-R" or "M11-W01-C01-T1-G" or "M11-W01-C01-T1-V" => "done", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-R" or "M11-W01-C02-T1-G" or "M11-W01-C02-T1-V" => "done", "M11-W01-C03" or "M11-W01-C03-T1" => "in_progress", "M11-W01-C03-T1-R" => "done", "M11-W01-C03-T1-G" => "done", "M11-W01-C03-T1-V" => "in_progress", _ => "planned" }) &&
        (Id(x) switch { "M11-W01-C01-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS373/attempt-001.json", "M11-W01-C01-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS375/attempt-001.json", "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS377/attempt-001.json", "M11-W01-C02-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS379/attempt-001.json", "M11-W01-C02-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS381/attempt-001.json", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS383/attempt-001.json", "M11-W01-C03-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS385/attempt-001.json", "M11-W01-C03-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS387/attempt-001.json", _ => x["evidence"]!.AsArray().Count == 0 })) &&
    n11.All(x => (string?)x["state"] == (Id(x) switch { "M11" or "M11-W01" => "in-progress", "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-R" or "M11-W01-C01-T1-G" or "M11-W01-C01-T1-V" => "done", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-R" or "M11-W01-C02-T1-G" or "M11-W01-C02-T1-V" => "done", "M11-W01-C03" or "M11-W01-C03-T1" or "M11-W01-C03-T1-R" or "M11-W01-C03-T1-G" or "M11-W01-C03-T1-V" => "done", "M11-W01-C04" or "M11-W01-C04-T1" or "M11-W01-C04-T1-R" => "ready", _ => "not-ready" })) &&
    c11.Select(Id).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(n11.Select(Id).OrderBy(x => x, StringComparer.Ordinal)),
    "New scope roster or readiness frontier differs.");
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
var admissionReport = Read(".aura/workflow/reports/RS370/attempt-001.json");
var admissionProof = Read(".workflow/runs/RS370/preparation-proof.json");
Need((string?)admissionReport["runId"] == "RS370" && (string?)admissionReport["outcome"] == "succeeded" &&
    (int?)admissionReport["attempt"] == 1 && (bool?)admissionReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    admissionReport["boundaryViolations"]!.AsArray().Count == 0 && admissionReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)admissionProof["runId"] == "RS370" && (bool?)admissionProof["payloadCompiled"] == true &&
    (int?)admissionProof["rehearsedValidationCount"] == 9, "RS370 durable admission evidence differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-readiness-rs371.md"), "Exact scaling readiness decision missing.");
var readinessReport = Read(".aura/workflow/reports/RS371/attempt-001.json");
var readinessProof = Read(".workflow/runs/RS371/preparation-proof.json");
Need((string?)readinessReport["outcome"] == "succeeded" && (int?)readinessReport["attempt"] == 1 &&
    (bool?)readinessReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    readinessReport["boundaryViolations"]!.AsArray().Count == 0 &&
    readinessReport["progress"]!["transitions"]!.AsArray().Count == 5 &&
    (bool?)readinessProof["payloadCompiled"] == true && (int?)readinessProof["rehearsedValidationCount"] == 5,
    "RS371 durable readiness differs.");
var redReport = Read(".aura/workflow/reports/RS372/attempt-001.json");
var redProof = Read(".workflow/runs/RS372/preparation-proof.json");
Need((string?)redReport["runId"] == "RS372" && (string?)redReport["outcome"] == "succeeded" &&
    (int?)redReport["attempt"] == 1 && (bool?)redReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    redReport["boundaryViolations"]!.AsArray().Count == 0 &&
    redReport["progress"]!["transitions"]!.AsArray().Count == 5 &&
    redReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)redProof["runId"] == "RS372" && (bool?)redProof["payloadCompiled"] == true &&
    (int?)redProof["rehearsedValidationCount"] == 10 &&
    redProof["preparationValidationIds"]!.AsArray().Any(x => (string?)x == "math-rs372-exact-int64-red"),
    "RS372 durable compiled RED evidence differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-red-closure-rs373.md"), "RED closure decision missing.");
var closureReport = Read(".aura/workflow/reports/RS373/attempt-001.json");
var closureProof = Read(".workflow/runs/RS373/preparation-proof.json");
Need((string?)closureReport["runId"] == "RS373" && (string?)closureReport["outcome"] == "succeeded" &&
    (int?)closureReport["attempt"] == 1 && (bool?)closureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    closureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    closureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    closureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)closureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C01-T1-R" &&
    (bool?)closureProof["payloadCompiled"] == true && (int?)closureProof["rehearsedValidationCount"] == 9,
    "RS373 durable RED closure differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-green-rs374.md"), "GREEN decision missing.");
var greenReport = Read(".aura/workflow/reports/RS374/attempt-001.json");
var greenProof = Read(".workflow/runs/RS374/preparation-proof.json");
string[] greenIds = [
    "math-rs374-exact-int64-independent-green", "math-rs374-exact-int64-focused-maintained-tests",
    "math-rs374-quantities-locked-regression", "math-rs374-m11-green-activation", "math-rs374-native-roadmap-parity",
    "math-rs374-m10-readiness-preserved", "math-rs374-m9-admission", "math-rs374-published-handoff",
    "math-rs374-request-reconciliation", "math-rs374-m8-evidence-frontier", "math-rs374-planning-render-check", "diff-check"
];
Need((string?)greenReport["runId"] == "RS374" && (string?)greenReport["outcome"] == "succeeded" &&
    (int?)greenReport["attempt"] == 1 && (bool?)greenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)greenReport["executor"]?["toolVersion"] == "0.1.78" &&
    greenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    greenReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)greenReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C01-T1-G" &&
    (string?)greenReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    greenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)greenProof["sourceCommit"] == "23c6d1d39215ae32cb0d73d970cff44875fbb5e0" &&
    (bool?)greenProof["payloadCompiled"] == true && (int?)greenProof["rehearsedValidationCount"] == 12 &&
    Strings(greenProof, "preparationValidationIds").SequenceEqual(greenIds) &&
    greenReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(greenIds),
    "RS374 durable GREEN qualification differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-green-closure-rs375.md"), "GREEN closure decision missing.");
var greenClosureReport = Read(".aura/workflow/reports/RS375/attempt-001.json");
var greenClosureProof = Read(".workflow/runs/RS375/preparation-proof.json");
Need((string?)greenClosureReport["runId"] == "RS375" && (string?)greenClosureReport["outcome"] == "succeeded" &&
    (int?)greenClosureReport["attempt"] == 1 && (bool?)greenClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    greenClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    greenClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    greenClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)greenClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C01-T1-G" &&
    (bool?)greenClosureProof["payloadCompiled"] == true && (int?)greenClosureProof["rehearsedValidationCount"] == 9,
    "RS375 durable GREEN closure differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-verification-rs376.md"), "Verification decision missing.");
var verifyReport = Read(".aura/workflow/reports/RS376/attempt-001.json");
var verifyProof = Read(".workflow/runs/RS376/preparation-proof.json");
string[] verifyIds = [
    "math-rs376-exact-int64-independent-boundaries", "math-rs376-m11-verification-activation",
    "math-rs376-native-roadmap-parity", "math-rs376-m10-readiness-preserved", "math-rs376-m9-admission",
    "math-rs376-published-handoff", "math-rs376-request-reconciliation", "math-rs376-m8-evidence-frontier",
    "math-rs376-planning-render-check", "diff-check"
];
Need((string?)verifyReport["runId"] == "RS376" && (string?)verifyReport["outcome"] == "succeeded" &&
    (int?)verifyReport["attempt"] == 1 && (bool?)verifyReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)verifyReport["executor"]?["toolVersion"] == "0.1.78" &&
    verifyReport["boundaryViolations"]!.AsArray().Count == 0 &&
    verifyReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)verifyReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C01-T1-V" &&
    (string?)verifyReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    verifyReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)verifyProof["sourceCommit"] == "5c5586beaea8eff5be041b7c98ccc949850594b0" &&
    (bool?)verifyProof["payloadCompiled"] == true && (int?)verifyProof["rehearsedValidationCount"] == 10 &&
    Strings(verifyProof, "preparationValidationIds").SequenceEqual(verifyIds) &&
    verifyReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(verifyIds),
    "RS376 durable independent verification differs.");
Need(File.Exists("docs/planning/decisions/m11-exact-int64-scaling-source-closure-rs377.md"), "Source closure decision missing.");
var scalingClosureReport = Read(".aura/workflow/reports/RS377/attempt-001.json");
var scalingClosureProof = Read(".workflow/runs/RS377/preparation-proof.json");
string[] scalingDone = ["M11-W01-C01", "M11-W01-C01-T1", "M11-W01-C01-T1-V"];
string[] momentsReady = ["M11-W01-C02", "M11-W01-C02-T1", "M11-W01-C02-T1-R"];
Need((string?)scalingClosureReport["runId"] == "RS377" && (string?)scalingClosureReport["outcome"] == "succeeded" &&
    (int?)scalingClosureReport["attempt"] == 1 && (bool?)scalingClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    scalingClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    scalingClosureReport["progress"]!["transitions"]!.AsArray().Count == 6 &&
    scalingClosureReport["progress"]!["completions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(scalingDone) &&
    scalingClosureReport["progress"]!["transitions"]!.AsArray().Where(x => (string?)x?["afterState"] == "ready").Select(x => (string?)x?["id"]).SequenceEqual(momentsReady) &&
    (bool?)scalingClosureProof["payloadCompiled"] == true && (int?)scalingClosureProof["rehearsedValidationCount"] == 9,
    "RS377 durable exact scaling closure and moments readiness differ.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-red-rs378.md"), "Streaming moments RED decision missing.");
var momentsRedReport = Read(".aura/workflow/reports/RS378/attempt-001.json");
var momentsRedProof = Read(".workflow/runs/RS378/preparation-proof.json");
string[] momentsRedIds = [
    "math-rs378-streaming-moments-red", "math-rs378-m11-moments-red-activation",
    "math-rs378-native-roadmap-parity", "math-rs378-m10-readiness-preserved", "math-rs378-m9-admission",
    "math-rs378-published-handoff", "math-rs378-request-reconciliation", "math-rs378-m8-evidence-frontier",
    "math-rs378-planning-render-check", "diff-check"
];
Need((string?)momentsRedReport["runId"] == "RS378" && (string?)momentsRedReport["outcome"] == "succeeded" &&
    (int?)momentsRedReport["attempt"] == 1 && (bool?)momentsRedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)momentsRedReport["executor"]?["toolVersion"] == "0.1.78" &&
    momentsRedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsRedReport["progress"]!["transitions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(momentsReady) &&
    momentsRedReport["progress"]!["transitions"]!.AsArray().All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    momentsRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)momentsRedProof["sourceCommit"] == "89c85029d761297afac4971bb9996d13c1119a0b" &&
    (bool?)momentsRedProof["payloadCompiled"] == true && (int?)momentsRedProof["rehearsedValidationCount"] == 10 &&
    Strings(momentsRedProof, "preparationValidationIds").SequenceEqual(momentsRedIds) &&
    momentsRedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(momentsRedIds),
    "RS378 durable streaming moments RED differs.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-red-closure-rs379.md"), "Streaming moments RED closure decision missing.");
var momentsRedClosureReport = Read(".aura/workflow/reports/RS379/attempt-001.json");
var momentsRedClosureProof = Read(".workflow/runs/RS379/preparation-proof.json");
Need((string?)momentsRedClosureReport["runId"] == "RS379" && (string?)momentsRedClosureReport["outcome"] == "succeeded" &&
    (int?)momentsRedClosureReport["attempt"] == 1 && (bool?)momentsRedClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    momentsRedClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsRedClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    momentsRedClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)momentsRedClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C02-T1-R" &&
    (bool?)momentsRedClosureProof["payloadCompiled"] == true && (int?)momentsRedClosureProof["rehearsedValidationCount"] == 9,
    "RS379 durable moments RED closure differs.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-green-rs380.md"), "Moments GREEN decision missing.");
var momentsGreenReport = Read(".aura/workflow/reports/RS380/attempt-001.json");
var momentsGreenProof = Read(".workflow/runs/RS380/preparation-proof.json");
string[] momentsGreenIds = [
    "math-rs380-streaming-moments-independent-green", "math-rs380-streaming-moments-focused-maintained-tests",
    "math-rs380-statistics-locked-regression", "math-rs380-m11-moments-green-activation", "math-rs380-native-roadmap-parity",
    "math-rs380-m10-readiness-preserved", "math-rs380-m9-admission", "math-rs380-published-handoff",
    "math-rs380-request-reconciliation", "math-rs380-m8-evidence-frontier", "math-rs380-planning-render-check", "diff-check"
];
Need((string?)momentsGreenReport["runId"] == "RS380" && (string?)momentsGreenReport["outcome"] == "succeeded" &&
    (int?)momentsGreenReport["attempt"] == 1 && (bool?)momentsGreenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)momentsGreenReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)momentsGreenReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    momentsGreenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsGreenReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)momentsGreenReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C02-T1-G" &&
    (string?)momentsGreenReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    momentsGreenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)momentsGreenProof["sourceCommit"] == "79f54d76cb2684f32f8da4788080641dc637958f" &&
    (bool?)momentsGreenProof["payloadCompiled"] == true && (int?)momentsGreenProof["rehearsedValidationCount"] == 12 &&
    Strings(momentsGreenProof, "preparationValidationIds").SequenceEqual(momentsGreenIds) &&
    momentsGreenReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(momentsGreenIds),
    "RS380 durable GREEN qualification differs.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-green-closure-rs381.md"), "Moments GREEN closure decision missing.");
var momentsGreenClosureReport = Read(".aura/workflow/reports/RS381/attempt-001.json");
var momentsGreenClosureProof = Read(".workflow/runs/RS381/preparation-proof.json");
Need((string?)momentsGreenClosureReport["runId"] == "RS381" && (string?)momentsGreenClosureReport["outcome"] == "succeeded" &&
    (int?)momentsGreenClosureReport["attempt"] == 1 && (bool?)momentsGreenClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)momentsGreenClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)momentsGreenClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    momentsGreenClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsGreenClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    momentsGreenClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)momentsGreenClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C02-T1-G" &&
    (string?)momentsGreenClosureProof["sourceCommit"] == "910aed8cb36d175bb7f0dff3f08dccc027e6ae46" &&
    (bool?)momentsGreenClosureProof["payloadCompiled"] == true && (int?)momentsGreenClosureProof["rehearsedValidationCount"] == 9,
    "RS381 durable moments GREEN closure differs.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-verification-rs382.md"), "Moments verification decision missing.");
var momentsVerifyReport = Read(".aura/workflow/reports/RS382/attempt-001.json");
var momentsVerifyProof = Read(".workflow/runs/RS382/preparation-proof.json");
string[] momentsVerifyIds = ["math-rs382-m11-moments-verification-activation", "math-rs382-native-roadmap-parity", "math-rs382-m10-readiness-preserved", "math-rs382-m9-admission", "math-rs382-published-handoff", "math-rs382-request-reconciliation", "math-rs382-m8-evidence-frontier", "math-rs382-planning-render-check", "diff-check", "math-rs382-streaming-moments-independent-boundaries"];
Need((string?)momentsVerifyReport["runId"] == "RS382" && (string?)momentsVerifyReport["outcome"] == "succeeded" &&
    (int?)momentsVerifyReport["attempt"] == 1 &&
    (bool?)momentsVerifyReport["priorInvocationFailures"]?["firstPassSucceeded"] == false &&
    (int?)momentsVerifyReport["priorInvocationFailures"]?["count"] == 1 &&
    (int?)momentsVerifyReport["priorInvocationFailures"]?["preparationFailureCount"] == 1 &&
    (int?)momentsVerifyReport["priorInvocationFailures"]?["allocatedAttemptFailureCount"] == 0 &&
    (string?)momentsVerifyReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)momentsVerifyReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    momentsVerifyReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsVerifyReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)momentsVerifyReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C02-T1-V" &&
    (string?)momentsVerifyReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    momentsVerifyReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)momentsVerifyProof["sourceCommit"] == "786dfe2d920ebaabc1303da287d905477defb6b9" &&
    (bool?)momentsVerifyProof["payloadCompiled"] == true &&
    (int?)momentsVerifyProof["rehearsedValidationCount"] == 10 &&
    Strings(momentsVerifyProof, "preparationValidationIds").SequenceEqual(momentsVerifyIds) &&
    momentsVerifyReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(momentsVerifyIds),
    "RS382 durable independent moments verification differs.");
Need(File.Exists("docs/planning/decisions/m11-streaming-moments-source-closure-rs383.md"), "Moments source closure decision missing.");
var momentsClosureReport = Read(".aura/workflow/reports/RS383/attempt-001.json");
var momentsClosureProof = Read(".workflow/runs/RS383/preparation-proof.json");
string[] momentsDone = ["M11-W01-C02", "M11-W01-C02-T1", "M11-W01-C02-T1-V"];
string[] quantilesReady = ["M11-W01-C03", "M11-W01-C03-T1", "M11-W01-C03-T1-R"];
Need((string?)momentsClosureReport["runId"] == "RS383" && (string?)momentsClosureReport["outcome"] == "succeeded" &&
    (int?)momentsClosureReport["attempt"] == 1 && (bool?)momentsClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)momentsClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)momentsClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    momentsClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    momentsClosureReport["progress"]!["transitions"]!.AsArray().Count == 6 &&
    momentsClosureReport["progress"]!["completions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(momentsDone) &&
    momentsClosureReport["progress"]!["transitions"]!.AsArray().Where(x => (string?)x?["afterState"] == "ready").Select(x => (string?)x?["id"]).SequenceEqual(quantilesReady) &&
    (string?)momentsClosureProof["sourceCommit"] == "4698f362ad1b1ee83cc2d37838fe530fedc556ef" &&
    (bool?)momentsClosureProof["payloadCompiled"] == true && (int?)momentsClosureProof["rehearsedValidationCount"] == 9,
    "RS383 durable moments closure differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-red-rs384.md") && File.Exists("docs/engineering/ProbeM11LargerSampleQuantilesRed.cs") && File.Exists("docs/engineering/fixtures/M11LargerSampleQuantilesOracle.cs"), "Quantiles RED material missing.");
var quantilesRedReport = Read(".aura/workflow/reports/RS384/attempt-001.json");
var quantilesRedProof = Read(".workflow/runs/RS384/preparation-proof.json");
string[] quantilesRedIds = ["math-rs384-larger-sample-quantiles-red", "math-rs384-m11-quantiles-red-activation", "math-rs384-native-roadmap-parity", "math-rs384-m10-readiness-preserved", "math-rs384-m9-admission", "math-rs384-published-handoff", "math-rs384-request-reconciliation", "math-rs384-m8-evidence-frontier", "math-rs384-planning-render-check", "diff-check"];
Need((string?)quantilesRedReport["runId"] == "RS384" && (string?)quantilesRedReport["outcome"] == "succeeded" &&
    (int?)quantilesRedReport["attempt"] == 1 && (bool?)quantilesRedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesRedReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesRedReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesRedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesRedReport["progress"]!["transitions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(quantilesReady) &&
    quantilesRedReport["progress"]!["transitions"]!.AsArray().All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    quantilesRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)quantilesRedProof["sourceCommit"] == "bb16e63732618b71745d6a27d732d0c915a484b9" &&
    (bool?)quantilesRedProof["payloadCompiled"] == true && (int?)quantilesRedProof["rehearsedValidationCount"] == 10 &&
    Strings(quantilesRedProof, "preparationValidationIds").SequenceEqual(quantilesRedIds) &&
    quantilesRedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(quantilesRedIds),
    "RS384 durable independent quantiles RED differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-red-closure-rs385.md"), "Quantiles RED closure decision missing.");
var quantilesRedClosureReport = Read(".aura/workflow/reports/RS385/attempt-001.json");
var quantilesRedClosureProof = Read(".workflow/runs/RS385/preparation-proof.json");
Need((string?)quantilesRedClosureReport["runId"] == "RS385" && (string?)quantilesRedClosureReport["outcome"] == "succeeded" &&
    (int?)quantilesRedClosureReport["attempt"] == 1 && (bool?)quantilesRedClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesRedClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesRedClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesRedClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesRedClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    quantilesRedClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)quantilesRedClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C03-T1-R" &&
    (string?)quantilesRedClosureProof["sourceCommit"] == "0c47a56bd63a0320dcb23348721f1c53bb49a828" &&
    (bool?)quantilesRedClosureProof["payloadCompiled"] == true && (int?)quantilesRedClosureProof["rehearsedValidationCount"] == 9,
    "RS385 durable quantiles RED closure differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-green-rs386.md") && File.Exists("tests/projects/dw.tools.math.statistics.tests/M11LargerSampleQuantilesTests.cs"), "Quantiles GREEN material missing.");
var quantilesGreenReport = Read(".aura/workflow/reports/RS386/attempt-001.json");
var quantilesGreenProof = Read(".workflow/runs/RS386/preparation-proof.json");
string[] quantilesGreenIds = ["math-rs386-larger-sample-quantiles-independent-green", "math-rs386-m11-quantiles-green-activation", "math-rs386-native-roadmap-parity", "math-rs386-m10-readiness-preserved", "math-rs386-m9-admission", "math-rs386-published-handoff", "math-rs386-request-reconciliation", "math-rs386-m8-evidence-frontier", "math-rs386-planning-render-check", "diff-check", "math-rs386-quantile-maintained-regression"];
Need((string?)quantilesGreenReport["runId"] == "RS386" && (string?)quantilesGreenReport["outcome"] == "succeeded" &&
    (int?)quantilesGreenReport["attempt"] == 1 && (bool?)quantilesGreenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesGreenReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesGreenReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesGreenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesGreenReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)quantilesGreenReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C03-T1-G" &&
    (string?)quantilesGreenReport["progress"]!["transitions"]![0]!["beforeState"] == "ready" &&
    (string?)quantilesGreenReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    quantilesGreenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)quantilesGreenProof["sourceCommit"] == "698503f0508516f5a1df683dd510bfebb0326db5" &&
    (bool?)quantilesGreenProof["payloadCompiled"] == true && (int?)quantilesGreenProof["rehearsedValidationCount"] == 11 &&
    Strings(quantilesGreenProof, "preparationValidationIds").SequenceEqual(quantilesGreenIds) &&
    quantilesGreenReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(quantilesGreenIds),
    "RS386 durable explicit-budget quantiles GREEN differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-green-closure-rs387.md"), "Quantiles GREEN closure decision missing.");
var quantilesGreenClosureReport = Read(".aura/workflow/reports/RS387/attempt-001.json");
var quantilesGreenClosureProof = Read(".workflow/runs/RS387/preparation-proof.json");
Need((string?)quantilesGreenClosureReport["runId"] == "RS387" && (string?)quantilesGreenClosureReport["outcome"] == "succeeded" &&
    (int?)quantilesGreenClosureReport["attempt"] == 1 && (bool?)quantilesGreenClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesGreenClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesGreenClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesGreenClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesGreenClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    quantilesGreenClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)quantilesGreenClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C03-T1-G" &&
    (string?)quantilesGreenClosureProof["sourceCommit"] == "d1501be177ee2300e385b0bb13f9c8b8c9a989a5" &&
    (bool?)quantilesGreenClosureProof["payloadCompiled"] == true && (int?)quantilesGreenClosureProof["rehearsedValidationCount"] == 9,
    "RS387 durable quantiles GREEN closure differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-verification-rs388.md") && File.Exists("tests/projects/dw.tools.math.statistics.tests/M11LargerSampleQuantilesVerificationTests.cs"), "Quantiles verification material missing.");
var quantilesVerifyReport = Read(".aura/workflow/reports/RS388/attempt-001.json");
var quantilesVerifyProof = Read(".workflow/runs/RS388/preparation-proof.json");
string[] quantilesVerifyIds = ["math-rs388-m11-quantiles-verification-activation", "math-rs388-native-roadmap-parity", "math-rs388-m10-readiness-preserved", "math-rs388-m9-admission", "math-rs388-published-handoff", "math-rs388-request-reconciliation", "math-rs388-m8-evidence-frontier", "math-rs388-planning-render-check", "diff-check", "math-rs388-quantiles-independent-boundaries"];
Need((string?)quantilesVerifyReport["runId"] == "RS388" && (string?)quantilesVerifyReport["outcome"] == "succeeded" &&
    (int?)quantilesVerifyReport["attempt"] == 1 && (bool?)quantilesVerifyReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesVerifyReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesVerifyReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesVerifyReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesVerifyReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)quantilesVerifyReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C03-T1-V" &&
    (string?)quantilesVerifyReport["progress"]!["transitions"]![0]!["beforeState"] == "ready" &&
    (string?)quantilesVerifyReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    quantilesVerifyReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)quantilesVerifyProof["sourceCommit"] == "611fb5abed9677b1c2c323533559391e3a37eca7" &&
    (bool?)quantilesVerifyProof["payloadCompiled"] == true && (int?)quantilesVerifyProof["rehearsedValidationCount"] == 10 &&
    Strings(quantilesVerifyProof, "preparationValidationIds").SequenceEqual(quantilesVerifyIds) &&
    quantilesVerifyReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(quantilesVerifyIds),
    "RS388 durable independent quantile verification differs.");
Need(File.Exists("docs/planning/decisions/m11-larger-sample-quantiles-source-closure-rs389.md"), "Quantiles source closure decision missing.");
Console.WriteLine("MATH-RS389-M11-QUANTILES-CLOSURE|verify=done|task=done|phase=done|candidateRed=ready|cutoverGate=M11-W01-C04|publication=false|PASS");
