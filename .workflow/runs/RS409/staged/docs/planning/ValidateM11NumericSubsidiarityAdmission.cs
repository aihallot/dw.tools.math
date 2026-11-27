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
Need((string?)b["plan_version"] == "0.1.176" && (string?)b["status"] == "in_progress", "Canonical version differs.");
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
    c11.All(x => (string?)x["status"] == (Id(x) switch { "M11" or "M11-W01" or "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-R" or "M11-W01-C01-T1-G" or "M11-W01-C01-T1-V" => "done", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-R" or "M11-W01-C02-T1-G" or "M11-W01-C02-T1-V" => "done", "M11-W01-C03" or "M11-W01-C03-T1" or "M11-W01-C03-T1-R" or "M11-W01-C03-T1-G" or "M11-W01-C03-T1-V" => "done", "M11-W01-C04" or "M11-W01-C04-T1" or "M11-W01-C04-T1-R" or "M11-W01-C04-T1-G" or "M11-W01-C04-T1-V" => "done", _ => "planned" }) &&
        (Id(x) switch { "M11" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS397/attempt-001.json", "M11-W01" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS396/attempt-001.json", "M11-W01-C01-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS373/attempt-001.json", "M11-W01-C01-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS375/attempt-001.json", "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS377/attempt-001.json", "M11-W01-C02-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS379/attempt-001.json", "M11-W01-C02-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS381/attempt-001.json", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS383/attempt-001.json", "M11-W01-C03-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS385/attempt-001.json", "M11-W01-C03-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS387/attempt-001.json", "M11-W01-C03" or "M11-W01-C03-T1" or "M11-W01-C03-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS389/attempt-001.json", "M11-W01-C04-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS391/attempt-001.json", "M11-W01-C04-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS393/attempt-001.json", "M11-W01-C04" or "M11-W01-C04-T1" or "M11-W01-C04-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS395/attempt-001.json", _ => x["evidence"]!.AsArray().Count == 0 })) &&
    n11.All(x => (string?)x["state"] == (Id(x) switch { "M11" or "M11-W01" => "done", "M11-W01-C01" or "M11-W01-C01-T1" or "M11-W01-C01-T1-R" or "M11-W01-C01-T1-G" or "M11-W01-C01-T1-V" => "done", "M11-W01-C02" or "M11-W01-C02-T1" or "M11-W01-C02-T1-R" or "M11-W01-C02-T1-G" or "M11-W01-C02-T1-V" => "done", "M11-W01-C03" or "M11-W01-C03-T1" or "M11-W01-C03-T1-R" or "M11-W01-C03-T1-G" or "M11-W01-C03-T1-V" => "done", "M11-W01-C04" or "M11-W01-C04-T1" or "M11-W01-C04-T1-R" or "M11-W01-C04-T1-G" or "M11-W01-C04-T1-V" => "done", _ => "not-ready" })) &&
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
var quantilesClosureReport = Read(".aura/workflow/reports/RS389/attempt-001.json");
var quantilesClosureProof = Read(".workflow/runs/RS389/preparation-proof.json");
string[] quantilesDone = ["M11-W01-C03", "M11-W01-C03-T1", "M11-W01-C03-T1-V"];
string[] candidatesReady = ["M11-W01-C04", "M11-W01-C04-T1", "M11-W01-C04-T1-R"];
Need((string?)quantilesClosureReport["runId"] == "RS389" && (string?)quantilesClosureReport["outcome"] == "succeeded" &&
    (int?)quantilesClosureReport["attempt"] == 1 && (bool?)quantilesClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)quantilesClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)quantilesClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    quantilesClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    quantilesClosureReport["progress"]!["transitions"]!.AsArray().Count == 6 &&
    quantilesClosureReport["progress"]!["completions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(quantilesDone) &&
    quantilesClosureReport["progress"]!["transitions"]!.AsArray().Where(x => (string?)x?["afterState"] == "ready").Select(x => (string?)x?["id"]).SequenceEqual(candidatesReady) &&
    (string?)quantilesClosureProof["sourceCommit"] == "6e2c2e757a68110bf3e6424eedeb74f663b7e2c5" &&
    (bool?)quantilesClosureProof["payloadCompiled"] == true && (int?)quantilesClosureProof["rehearsedValidationCount"] == 9,
    "RS389 durable quantile closure differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-red-rs390.md") && File.Exists("docs/engineering/ProbeM11NumericPackageCandidateRed.cs"), "Candidate RED material missing.");
var candidatesRedReport = Read(".aura/workflow/reports/RS390/attempt-001.json");
var candidatesRedProof = Read(".workflow/runs/RS390/preparation-proof.json");
string[] candidatesRedIds = ["math-rs390-numeric-package-candidate-red", "math-rs390-m11-candidate-red-activation", "math-rs390-native-roadmap-parity", "math-rs390-m10-readiness-preserved", "math-rs390-m9-admission", "math-rs390-published-handoff", "math-rs390-request-reconciliation", "math-rs390-m8-evidence-frontier", "math-rs390-planning-render-check", "diff-check"];
Need((string?)candidatesRedReport["runId"] == "RS390" && (string?)candidatesRedReport["outcome"] == "succeeded" &&
    (int?)candidatesRedReport["attempt"] == 1 && (bool?)candidatesRedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)candidatesRedReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidatesRedReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidatesRedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidatesRedReport["progress"]!["transitions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidatesReady) &&
    candidatesRedReport["progress"]!["transitions"]!.AsArray().All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    candidatesRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)candidatesRedProof["sourceCommit"] == "96d350633dce3f43d63ed9e87609032e7bd2ec0e" &&
    (bool?)candidatesRedProof["payloadCompiled"] == true && (int?)candidatesRedProof["rehearsedValidationCount"] == 10 &&
    Strings(candidatesRedProof, "preparationValidationIds").SequenceEqual(candidatesRedIds) &&
    candidatesRedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidatesRedIds),
    "RS390 durable physical package RED differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-red-closure-rs391.md"), "Candidate RED closure decision missing.");
var candidatesRedClosureReport = Read(".aura/workflow/reports/RS391/attempt-001.json");
var candidatesRedClosureProof = Read(".workflow/runs/RS391/preparation-proof.json");
Need((string?)candidatesRedClosureReport["runId"] == "RS391" && (string?)candidatesRedClosureReport["outcome"] == "succeeded" &&
    (int?)candidatesRedClosureReport["attempt"] == 1 && (bool?)candidatesRedClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)candidatesRedClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidatesRedClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidatesRedClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidatesRedClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    candidatesRedClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)candidatesRedClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C04-T1-R" &&
    (string?)candidatesRedClosureProof["sourceCommit"] == "4ce20f399ee983e8bb0a6df2695866e403ce8bd8" &&
    (bool?)candidatesRedClosureProof["payloadCompiled"] == true && (int?)candidatesRedClosureProof["rehearsedValidationCount"] == 9,
    "RS391 durable numeric package RED closure differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-green-rs392.md") && File.Exists("docs/engineering/ProbeM11NumericPackageCandidateGreen.cs") && File.Exists("docs/engineering/fixtures/M11NumericPackageConsumer.cs"), "Candidate GREEN material missing.");
var candidateGreenReport = Read(".aura/workflow/reports/RS392/attempt-002.json");
var candidateGreenProof = Read(".workflow/runs/RS392/preparation-proof.json");
string[] candidateGreenPrepIds = ["math-rs392-m11-candidate-green-activation", "math-rs392-native-roadmap-parity", "math-rs392-m10-readiness-preserved", "math-rs392-m9-admission", "math-rs392-published-handoff", "math-rs392-request-reconciliation", "math-rs392-m8-evidence-frontier", "math-rs392-planning-render-check", "diff-check"];
Need((string?)candidateGreenReport["runId"] == "RS392" && (string?)candidateGreenReport["outcome"] == "succeeded" &&
    (int?)candidateGreenReport["attempt"] == 2 && (bool?)candidateGreenReport["correctionMode"] == true &&
    (int?)candidateGreenReport["correctionSourceAttempt"] == 1 &&
    (bool?)candidateGreenReport["priorInvocationFailures"]?["firstPassSucceeded"] == false &&
    (int?)candidateGreenReport["priorInvocationFailures"]?["count"] == 1 &&
    (int?)candidateGreenReport["priorInvocationFailures"]?["preparationFailureCount"] == 0 &&
    (int?)candidateGreenReport["priorInvocationFailures"]?["allocatedAttemptFailureCount"] == 1 &&
    (string?)candidateGreenReport["priorInvocationFailures"]?["invocationIds"]?[0] == "run-next-20261127T133917Z-0e02714778b27099" &&
    (string?)candidateGreenReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidateGreenReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidateGreenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidateGreenReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)candidateGreenReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C04-T1-G" &&
    (string?)candidateGreenReport["progress"]!["transitions"]![0]!["beforeState"] == "ready" &&
    (string?)candidateGreenReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    candidateGreenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)candidateGreenProof["sourceCommit"] == "714932bbf2f9f60bd0603c4a14f3174628f81a43" &&
    (bool?)candidateGreenProof["payloadCompiled"] == true && (int?)candidateGreenProof["rehearsedValidationCount"] == 9 &&
    Strings(candidateGreenProof, "preparationValidationIds").SequenceEqual(candidateGreenPrepIds) &&
    candidateGreenReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidateGreenPrepIds),
    "RS392 durable corrected candidate GREEN lineage differs.");
var candidateBinary = candidateGreenReport["validations"]!.AsArray().Single();
Need((string?)candidateBinary?["Id"] == "math-rs392-isolated-numeric-candidate-binary-consumers" &&
    (int?)candidateBinary?["ExitCode"] == 0 && (bool?)candidateBinary?["TimedOut"] == false &&
    (bool?)candidateBinary?["Cancelled"] == false, "RS392 post-attempt binary validation differs.");
var candidateOutput = (string?)candidateBinary?["StandardOutput"] ?? "";
string[] candidateMarkers = [
    "MATH-RS392-ARCHIVE|id=dw.quantities|version=20260.20261127.1001|archiveSha256=1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d|dllSha256=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|tfm=net10.0|PASS",
    "MATH-RS392-ARCHIVE|id=dw.tools.math.statistics|version=20260.20261127.1002|archiveSha256=cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8|dllSha256=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|tfm=net10.0|PASS",
    "MATH-RS392-CONSUMER|tfm=net10.0|sdk=10.0.404|lockSha256=cb140f47caa53088d16189fe7692adfde477eab56a37b92349fec8ceedb67498|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS",
    "MATH-RS392-CONSUMER|tfm=net11.0|sdk=11.0.100|lockSha256=944ed2abe6502ae1b33d9b935eb625b97b82d8bdbea1d690bde76c160bf1e66b|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS",
    "MATH-RS392-CANDIDATES|root=D:\\data.dev\\github\\ai@hallot.net\\dw.tools.math\\build\\artifacts\\m11-candidates-1aa0ad9d4f7e|oldArchivesUnchanged=true|publication=false|windowsNet10=PASS|windowsNet11=PASS|PASS"
];
Need(candidateMarkers.All(x => candidateOutput.Contains(x, StringComparison.Ordinal)) &&
    candidateOutput.Split("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringSplitOptions.None).Length == 3 &&
    candidateOutput.Split("MATH-M11-BINARY|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS", StringSplitOptions.None).Length == 3,
    "RS392 measured archive/consumer evidence differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-green-closure-rs393.md"), "Candidate GREEN closure decision missing.");
var candidateClosureReport = Read(".aura/workflow/reports/RS393/attempt-001.json");
var candidateClosureProof = Read(".workflow/runs/RS393/preparation-proof.json");
Need((string?)candidateClosureReport["runId"] == "RS393" && (string?)candidateClosureReport["outcome"] == "succeeded" &&
    (int?)candidateClosureReport["attempt"] == 1 && (bool?)candidateClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (string?)candidateClosureReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidateClosureReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidateClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidateClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    candidateClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)candidateClosureReport["progress"]!["completions"]![0]!["id"] == "M11-W01-C04-T1-G" &&
    (string?)candidateClosureProof["sourceCommit"] == "74bfd02291b391598359cf20dd1374e560409ef9" &&
    (bool?)candidateClosureProof["payloadCompiled"] == true && (int?)candidateClosureProof["rehearsedValidationCount"] == 9,
    "RS393 durable candidate GREEN closure differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-verification-rs394.md") && File.Exists("docs/engineering/VerifyM11NumericPackageCandidates.cs"), "Candidate verification material missing.");
var candidateVerifyReport = Read(".aura/workflow/reports/RS394/attempt-001.json");
var candidateVerifyProof = Read(".workflow/runs/RS394/preparation-proof.json");
string[] candidateVerifyPrepIds = ["math-rs394-m11-candidate-verification-activation", "math-rs394-native-roadmap-parity", "math-rs394-m10-readiness-preserved", "math-rs394-m9-admission", "math-rs394-published-handoff", "math-rs394-request-reconciliation", "math-rs394-m8-evidence-frontier", "math-rs394-planning-render-check", "diff-check"];
Need((string?)candidateVerifyReport["runId"] == "RS394" && (string?)candidateVerifyReport["outcome"] == "succeeded" &&
    (int?)candidateVerifyReport["attempt"] == 1 && (bool?)candidateVerifyReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)candidateVerifyReport["priorInvocationFailures"]?["count"] == 0 &&
    (string?)candidateVerifyReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidateVerifyReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidateVerifyReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidateVerifyReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)candidateVerifyReport["progress"]!["transitions"]![0]!["id"] == "M11-W01-C04-T1-V" &&
    (string?)candidateVerifyReport["progress"]!["transitions"]![0]!["beforeState"] == "ready" &&
    (string?)candidateVerifyReport["progress"]!["transitions"]![0]!["afterState"] == "in-progress" &&
    candidateVerifyReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)candidateVerifyProof["sourceCommit"] == "6ea3cd7c692f6a147d1fe2a2846e29977f7ae171" &&
    (bool?)candidateVerifyProof["payloadCompiled"] == true && (int?)candidateVerifyProof["rehearsedValidationCount"] == 9 &&
    Strings(candidateVerifyProof, "preparationValidationIds").SequenceEqual(candidateVerifyPrepIds) &&
    candidateVerifyReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidateVerifyPrepIds),
    "RS394 durable independent candidate verification differs.");
var candidateVerifyBinary = candidateVerifyReport["validations"]!.AsArray().Single();
Need((string?)candidateVerifyBinary?["Id"] == "math-rs394-exact-candidate-readback-and-rollback" &&
    (int?)candidateVerifyBinary?["ExitCode"] == 0 && (bool?)candidateVerifyBinary?["TimedOut"] == false &&
    (bool?)candidateVerifyBinary?["Cancelled"] == false, "RS394 binary verification differs.");
var candidateVerifyOutput = (string?)candidateVerifyBinary?["StandardOutput"] ?? "";
string[] candidateVerifyMarkers = [
    "MATH-RS394-READBACK|id=dw.quantities|version=20260.20261127.1001|archiveSha256=1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d|dllSha256=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS",
    "MATH-RS394-READBACK|id=dw.tools.math.statistics|version=20260.20261127.1002|archiveSha256=cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8|dllSha256=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|PASS",
    "MATH-RS394-READBACK|id=dw.quantities|version=20260.20261102.1001|archiveSha256=e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d|dllSha256=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS",
    "MATH-RS394-READBACK|id=dw.tools.math.statistics|version=20260.20261120.1001|archiveSha256=feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1|dllSha256=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|PASS",
    "MATH-RS394-CONSUMER|mode=candidate|tfm=net10.0|sdk=10.0.404|lockSha256=cb140f47caa53088d16189fe7692adfde477eab56a37b92349fec8ceedb67498|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS",
    "MATH-RS394-CONSUMER|mode=candidate|tfm=net11.0|sdk=11.0.100|lockSha256=944ed2abe6502ae1b33d9b935eb625b97b82d8bdbea1d690bde76c160bf1e66b|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS",
    "MATH-RS394-CONSUMER|mode=rollback|tfm=net10.0|sdk=10.0.404|lockSha256=d6a5a88982d1996a2f7861591427079cfef156beea30b3419a89f977d3099379|statisticsDll=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|quantitiesDll=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS",
    "MATH-RS394-CONSUMER|mode=rollback|tfm=net11.0|sdk=11.0.100|lockSha256=4748bfc203a394cb0b1af60439576646504f9ac5eead4c18b30cb92abb2ed921|statisticsDll=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|quantitiesDll=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS",
    "MATH-RS394-VERIFY|archives=4|consumers=4|candidate=PASS|rollbackRecipe=PASS|rebuild=false|publication=false|PASS"
];
Need(candidateVerifyMarkers.All(x => candidateVerifyOutput.Contains(x, StringComparison.Ordinal)) &&
    candidateVerifyOutput.Split("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringSplitOptions.None).Length == 5 &&
    candidateVerifyOutput.Split("MATH-RS394-ROLLBACK-SURFACE|PASS", StringSplitOptions.None).Length == 3 &&
    candidateVerifyOutput.Split("MATH-M11-BINARY|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS", StringSplitOptions.None).Length == 3,
    "RS394 measured independent readback and rollback evidence differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-package-candidate-closure-rs395.md"), "Candidate closure decision missing.");
var candidatePhaseReport = Read(".aura/workflow/reports/RS395/attempt-001.json");
var candidatePhaseProof = Read(".workflow/runs/RS395/preparation-proof.json");
string[] candidatePhaseDone = ["M11-W01-C04", "M11-W01-C04-T1", "M11-W01-C04-T1-V"];
string[] candidatePhaseIds = ["math-rs395-m11-candidate-phase-closure", "math-rs395-native-roadmap-parity", "math-rs395-m10-readiness-preserved", "math-rs395-m9-admission", "math-rs395-published-handoff", "math-rs395-request-reconciliation", "math-rs395-m8-evidence-frontier", "math-rs395-planning-render-check", "diff-check"];
Need((string?)candidatePhaseReport["runId"] == "RS395" && (string?)candidatePhaseReport["outcome"] == "succeeded" &&
    (int?)candidatePhaseReport["attempt"] == 1 && (bool?)candidatePhaseReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)candidatePhaseReport["priorInvocationFailures"]?["count"] == 0 &&
    (string?)candidatePhaseReport["executor"]?["toolVersion"] == "0.1.80" &&
    (string?)candidatePhaseReport["executor"]?["assemblySha256"] == "b7eec1950dbd71e727b173d8695569d2c3dc5b933c41edf89c4787e56317b2fb" &&
    candidatePhaseReport["boundaryViolations"]!.AsArray().Count == 0 &&
    candidatePhaseReport["progress"]!["transitions"]!.AsArray().Count == 3 &&
    candidatePhaseReport["progress"]!["completions"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidatePhaseDone) &&
    (string?)candidatePhaseProof["sourceCommit"] == "a2f46bb5fe47019eb52433befe32aacc317b50c0" &&
    (bool?)candidatePhaseProof["payloadCompiled"] == true && (int?)candidatePhaseProof["rehearsedValidationCount"] == 9 &&
    Strings(candidatePhaseProof, "preparationValidationIds").SequenceEqual(candidatePhaseIds) &&
    candidatePhaseReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(candidatePhaseIds),
    "RS395 durable candidate phase closure differs.");
Need(File.Exists("docs/planning/decisions/m11-numeric-work-package-closure-rs396.md"), "M11 work package closure decision missing.");
var workPackageReport = Read(".aura/workflow/reports/RS396/attempt-001.json");
var workPackageProof = Read(".workflow/runs/RS396/preparation-proof.json");
string[] workPackageIds = ["math-rs396-m11-work-package-closure", "math-rs396-native-roadmap-parity", "math-rs396-m10-readiness-preserved", "math-rs396-m9-admission", "math-rs396-published-handoff", "math-rs396-request-reconciliation", "math-rs396-m8-evidence-frontier", "math-rs396-planning-render-check", "diff-check"];
Need((string?)workPackageReport["runId"] == "RS396" && (string?)workPackageReport["outcome"] == "succeeded" &&
    (int?)workPackageReport["attempt"] == 1 && (bool?)workPackageReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)workPackageReport["priorInvocationFailures"]?["count"] == 0 &&
    (string?)workPackageReport["executor"]?["toolVersion"] == "0.1.81" &&
    (string?)workPackageReport["executor"]?["assemblySha256"] == "f6e91746d8ec3ba2ea3d0586658749c440814080499832cfd6256805e054320a" &&
    workPackageReport["boundaryViolations"]!.AsArray().Count == 0 &&
    workPackageReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)workPackageReport["progress"]!["completions"]![0]!["id"] == "M11-W01" &&
    workPackageReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)workPackageProof["sourceCommit"] == "537c24240ae4f201e4efd6c22b17dbf68529e2ca" &&
    (bool?)workPackageProof["payloadCompiled"] == true && (int?)workPackageProof["rehearsedValidationCount"] == 9 &&
    Strings(workPackageProof, "preparationValidationIds").SequenceEqual(workPackageIds) &&
    workPackageReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(workPackageIds),
    "RS396 durable work package closure differs.");
Need((string?)C("M11")["gate"] == "G-M11" &&
    Strings(C("M11"), "required_chunks").SequenceEqual(new[] { "M11-W01-C01", "M11-W01-C02", "M11-W01-C03", "M11-W01-C04" }) &&
    Strings(C("M11"), "depends_on").SequenceEqual(new[] { "M3", "M9" }) &&
    Strings(N("M11"), "dependsOn").SequenceEqual(new[] { "M3", "M9" }) &&
    new[] { "M3", "M9" }.All(id => (string?)C(id)["status"] == "done" && (string?)N(id)["state"] == "done") &&
    File.Exists("docs/planning/decisions/m11-numeric-milestone-closure-rs397.md"),
    "G-M11 scope or prerequisite differs.");
var milestoneReport = Read(".aura/workflow/reports/RS397/attempt-001.json");
var milestoneProof = Read(".workflow/runs/RS397/preparation-proof.json");
string[] milestoneIds = ["math-rs397-m11-milestone-closure", "math-rs397-native-roadmap-parity", "math-rs397-m10-readiness-preserved", "math-rs397-m9-admission", "math-rs397-published-handoff", "math-rs397-request-reconciliation", "math-rs397-m8-evidence-frontier", "math-rs397-planning-render-check", "diff-check"];
Need((string?)milestoneReport["runId"] == "RS397" && (string?)milestoneReport["outcome"] == "succeeded" &&
    (int?)milestoneReport["attempt"] == 1 && (bool?)milestoneReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)milestoneReport["priorInvocationFailures"]?["count"] == 0 &&
    (string?)milestoneReport["executor"]?["toolVersion"] == "0.1.81" &&
    (string?)milestoneReport["executor"]?["assemblySha256"] == "f6e91746d8ec3ba2ea3d0586658749c440814080499832cfd6256805e054320a" &&
    milestoneReport["boundaryViolations"]!.AsArray().Count == 0 &&
    milestoneReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    milestoneReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)milestoneReport["progress"]!["completions"]![0]!["id"] == "M11" &&
    (string?)milestoneProof["sourceCommit"] == "68f7d0c2ba93a7162191ed0022c2b4743beed3b7" &&
    (bool?)milestoneProof["payloadCompiled"] == true && (int?)milestoneProof["rehearsedValidationCount"] == 9 &&
    Strings(milestoneProof, "preparationValidationIds").SequenceEqual(milestoneIds) &&
    milestoneReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(milestoneIds),
    "RS397 durable milestone closure differs.");
Need(c11.All(x => (string?)x["status"] == "done") && n11.All(x => (string?)x["state"] == "done") &&
    File.Exists("docs/planning/decisions/m11-numeric-canonical-reconciliation-rs398.md"),
    "Canonical M11 reconciliation differs.");
Console.WriteLine("MATH-RS398-M11-CANONICAL-RECONCILIATION|gate=G-M11|canonicalDone=22|nativeDone=22|m10Refresh=pending|publication=false|PASS");
