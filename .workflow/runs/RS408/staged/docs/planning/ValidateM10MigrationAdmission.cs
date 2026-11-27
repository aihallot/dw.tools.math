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
Need((string?)backlog["plan_version"] == "0.1.175" && (string?)backlog["status"] == "in_progress", "Aggregate target differs.");
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
Need(canonical.Count == 636 && current.Length == 17 && current.All(x => (string?)x["status"] == ((string?)x["id"] switch { "M10" or "M10-W01" or "M10-W01-C01" or "M10-W01-C01-T1" => "in_progress", "M10-W01-C01-T1-R" => "done", "M10-W01-C01-T1-G" => "in_progress", _ => "planned" }) && ((string?)x["id"] == "M10-W01-C01-T1-R" ? x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS401/attempt-001.json" : x["evidence"]!.AsArray().Count == 0)) &&
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
Need(native.Count == 630 && newNative.Length == 17 && newNative.All(x => (string?)x["state"] == ((string?)x["id"] switch { "M10" or "M10-W01" or "M10-W01-C01" or "M10-W01-C01-T1" => "in-progress", "M10-W01-C01-T1-R" or "M10-W01-C01-T1-G" => "done", "M10-W01-C01-T1-V" => "ready", _ => "not-ready" })) &&
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

var redClosureReport = Read(".aura/workflow/reports/RS401/attempt-001.json");
var redClosureProof = Read(".workflow/runs/RS401/preparation-proof.json");
string[] redClosureIds = ["math-rs401-m11-canonical-reconciliation","math-rs401-native-roadmap-parity","math-rs401-m10-red-closure","math-rs401-m9-admission","math-rs401-published-handoff","math-rs401-request-reconciliation","math-rs401-m8-evidence-frontier","math-rs401-planning-render-check","diff-check"];
Need((string?)redClosureReport["runId"] == "RS401" && (string?)redClosureReport["outcome"] == "succeeded" &&
    (int?)redClosureReport["attempt"] == 1 && (bool?)redClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)redClosureReport["priorInvocationFailures"]?["count"] == 0 &&
    (string?)redClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    redClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    redClosureReport["progress"]!["transitions"]!.AsArray().Count == 2 &&
    redClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)redClosureReport["progress"]!["completions"]![0]!["id"] == "M10-W01-C01-T1-R" &&
    (string?)redClosureProof["sourceCommit"] == "a8b7121f0f419f298bbbdcd9a3006f286ff76001" &&
    (bool?)redClosureProof["payloadCompiled"] == true && (int?)redClosureProof["rehearsedValidationCount"] == 9 &&
    redClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(redClosureIds) &&
    redClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(redClosureIds),
    "RS401 durable RED closure differs.");
Need(File.Exists("docs/planning/decisions/m10-green-project-dispositions-rs402.md") && File.Exists("docs/engineering/m10-project-dispositions-rs402.json"), "GREEN disposition material absent.");
Console.WriteLine("MATH-RS402-M10-GREEN-ACTIVATION|nativeGreen=in-progress|targetRestore=pending|rollback=pending|publication=false|PASS");

var dispositionsReport = Read(".aura/workflow/reports/RS402/attempt-001.json");
var dispositionsProof = Read(".workflow/runs/RS402/preparation-proof.json");
string[] dispositionIds = ["math-rs402-project-dispositions","math-rs402-m11-canonical-reconciliation","math-rs402-native-roadmap-parity","math-rs402-m10-green-activation","math-rs402-m9-admission","math-rs402-published-handoff","math-rs402-request-reconciliation","math-rs402-m8-evidence-frontier","math-rs402-planning-render-check","diff-check"];
var dispositionFailures = dispositionsReport["priorInvocationFailures"]!;
Need((string?)dispositionsReport["runId"] == "RS402" && (string?)dispositionsReport["outcome"] == "succeeded" &&
    (int?)dispositionsReport["attempt"] == 1 && (string?)dispositionsReport["executor"]?["toolVersion"] == "0.1.81" &&
    (int?)dispositionFailures["count"] == 1 && (int?)dispositionFailures["preparationFailureCount"] == 1 &&
    (int?)dispositionFailures["allocatedAttemptFailureCount"] == 0 && (bool?)dispositionFailures["firstPassSucceeded"] == false &&
    (string?)dispositionFailures["invocationIds"]![0] == "run-next-20261127T170741Z-b336ddbaff581a31" &&
    dispositionsReport["boundaryViolations"]!.AsArray().Count == 0 &&
    dispositionsReport["progress"]!["transitions"]!.AsArray().Count == 1 &&
    (string?)dispositionsReport["progress"]!["transitions"]![0]!["id"] == "M10-W01-C01-T1-G" &&
    (string?)dispositionsProof["sourceCommit"] == "30f83f468f100d465c9f13952fc5a4e6ad34db3c" &&
    (bool?)dispositionsProof["payloadCompiled"] == true && (int?)dispositionsProof["rehearsedValidationCount"] == 10 &&
    dispositionsProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(dispositionIds) &&
    dispositionsReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(dispositionIds),
    "RS402 durable corrected disposition evidence differs.");
Need(File.Exists("docs/planning/decisions/m10-isolated-solution-graph-rs403.md") && File.Exists("docs/engineering/ProbeM10IsolatedSolutionGraph.cs"), "Isolated graph material absent.");

var isolatedReport = Read(".aura/workflow/reports/RS403/attempt-001.json");
var isolatedProof = Read(".workflow/runs/RS403/preparation-proof.json");
string[] isolatedPreparationIds = ["math-rs403-project-dispositions", "math-rs403-m10-disposition-evidence", "diff-check"];
Need((string?)isolatedReport["runId"] == "RS403" && (string?)isolatedReport["outcome"] == "succeeded" &&
    (int?)isolatedReport["attempt"] == 1 && (string?)isolatedReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)isolatedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)isolatedReport["priorInvocationFailures"]?["count"] == 0 &&
    isolatedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    isolatedReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)isolatedProof["sourceCommit"] == "63bf41477ae5b634a10af34a7202e4f3abded0c4" &&
    (bool?)isolatedProof["payloadCompiled"] == true && (int?)isolatedProof["rehearsedValidationCount"] == 3 &&
    isolatedProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(isolatedPreparationIds) &&
    isolatedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(isolatedPreparationIds),
    "RS403 isolated graph durable evidence differs.");
var isolatedValidation = isolatedReport["validations"]!.AsArray().Single()!;
Need((string?)isolatedValidation["Id"] == "math-rs403-isolated-net11-solution" && (int?)isolatedValidation["ExitCode"] == 0 &&
    (bool?)isolatedValidation["TimedOut"] == false && (bool?)isolatedValidation["Cancelled"] == false,
    "RS403 integrated validation differs.");
var isolatedOutput = (string)isolatedValidation["StandardOutput"]!;
Need(isolatedOutput.Contains("MATH-RS403-ISOLATED|sdk=11.0.100|tfm=net11.0|lang=15.0|projects=24|lockedRestore=PASS|build=PASS|tests=false|tools=false|rollback=false|publication=false|PASS", StringComparison.Ordinal) &&
    isolatedOutput.Contains("0 Warning(s)", StringComparison.Ordinal) && isolatedOutput.Contains("0 Error(s)", StringComparison.Ordinal),
    "RS403 graph result marker differs.");
var compiledRows = isolatedOutput.Split('\n').Select(x => x.TrimEnd('\r'))
    .Where(x => x.StartsWith("MATH-RS403-PROJECT|", StringComparison.Ordinal)).ToArray();
var compiledPaths = new HashSet<string>(StringComparer.Ordinal);
foreach (var line in compiledRows)
{
    var match = System.Text.RegularExpressions.Regex.Match(line,
        @"^MATH-RS403-PROJECT\|path=(.+)\|target=net11\.0\|lockSha256=[0-9a-f]{64}\|dllSha256=[0-9a-f]{64}$");
    Need(match.Success && compiledPaths.Add(match.Groups[1].Value), "Invalid or duplicate isolated project evidence.");
}
Need(compiledRows.Length == 24 && compiledPaths.SetEquals(projects) && File.Exists("docs/planning/decisions/m10-isolated-fragile-tests-rs404.md"),
    "RS403 isolated project roster or next qualification material differs.");
Console.WriteLine("MATH-RS404-BASELINE|RS403=PASS|projects=24|warning=0|error=0|productionMigration=false|PASS");

var focusedReport = Read(".aura/workflow/reports/RS404/attempt-001.json");
var focusedProof = Read(".workflow/runs/RS404/preparation-proof.json");
string[] focusedPreparationIds = ["math-rs404-isolated-graph-evidence", "diff-check"];
Need((string?)focusedReport["runId"] == "RS404" && (string?)focusedReport["outcome"] == "succeeded" &&
    (int?)focusedReport["attempt"] == 1 && (string?)focusedReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)focusedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)focusedReport["priorInvocationFailures"]?["count"] == 0 &&
    focusedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    focusedReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)focusedProof["sourceCommit"] == "a30c17037d92e298cdeaf5aee2fa67f44cb28eb2" &&
    (bool?)focusedProof["payloadCompiled"] == true && (int?)focusedProof["rehearsedValidationCount"] == 2 &&
    focusedProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(focusedPreparationIds) &&
    focusedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(focusedPreparationIds),
    "RS404 durable evidence differs.");
var focusedValidation = focusedReport["validations"]!.AsArray().Single()!;
Need((string?)focusedValidation["Id"] == "math-rs404-isolated-fragile-tests" && (int?)focusedValidation["ExitCode"] == 0 &&
    (bool?)focusedValidation["TimedOut"] == false && (bool?)focusedValidation["Cancelled"] == false, "RS404 validation differs.");
var focusedOutput = (string)focusedValidation["StandardOutput"]!;
Need(focusedOutput.Contains("MATH-RS404-FOCUSED-TOTAL|sdk=11.0.100|tfm=net11.0|classes=8|passed=35|failed=0|PASS", StringComparison.Ordinal) &&
    focusedOutput.Contains("classes=3|passed=9|failed=0|trxSha256=42e7d981c83f22de1d745933c1a9506e54aca5f395aa558e7eb2176ddb4e6d12|PASS", StringComparison.Ordinal) &&
    focusedOutput.Contains("classes=5|passed=26|failed=0|trxSha256=9786ae5384a4d21dbdccc4fec677087bd1f57b03fb0cebbb57b96fbaacc5e3b6|PASS", StringComparison.Ordinal) &&
    File.Exists("docs/planning/decisions/m10-isolated-maintained-tools-rs405.md"), "RS404 coverage or next material differs.");
Console.WriteLine("MATH-RS405-BASELINE|RS404=PASS|classes=8|passed=35|productionMigration=false|PASS");

var toolsReport = Read(".aura/workflow/reports/RS405/attempt-002.json");
var toolsProof = Read(".workflow/runs/RS405/preparation-proof.json");
string[] toolsPreparationIds = ["math-rs405-focused-evidence", "diff-check"];
var toolsFailures = toolsReport["priorInvocationFailures"]!;
Need((string?)toolsReport["runId"] == "RS405" && (string?)toolsReport["outcome"] == "succeeded" &&
    (int?)toolsReport["attempt"] == 2 && (string?)toolsReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)toolsReport["correctionMode"] == true && (int?)toolsReport["correctionSourceAttempt"] == 1 &&
    (int?)toolsFailures["count"] == 1 && (int?)toolsFailures["allocatedAttemptFailureCount"] == 1 &&
    (int?)toolsFailures["preparationFailureCount"] == 0 && (bool?)toolsFailures["firstPassSucceeded"] == false &&
    (string?)toolsFailures["invocationIds"]![0] == "run-next-20261127T180439Z-bd74d7744d6bc85f" &&
    toolsReport["boundaryViolations"]!.AsArray().Count == 0 && toolsReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)toolsProof["sourceCommit"] == "d216de9bb94f3fb2751af4d5834c4205dfa20bf4" &&
    (bool?)toolsProof["payloadCompiled"] == true && (int?)toolsProof["rehearsedValidationCount"] == 2 &&
    toolsProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(toolsPreparationIds) &&
    toolsReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(toolsPreparationIds),
    "RS405 corrected maintained-tool evidence differs.");
var toolsValidation = toolsReport["validations"]!.AsArray().Single()!;
Need((string?)toolsValidation["Id"] == "math-rs405-isolated-maintained-tools" && (int?)toolsValidation["ExitCode"] == 0 &&
    (bool?)toolsValidation["TimedOut"] == false && (bool?)toolsValidation["Cancelled"] == false, "RS405 validation differs.");
var toolsOutput = (string)toolsValidation["StandardOutput"]!;
Need(toolsOutput.Contains("MATH-RS405-TOOLS|sdk=11.0.100|tfm=net11.0|tools=2|bindings=4|selfTests=5|inspect=false|publication=false|PASS", StringComparison.Ordinal) &&
    toolsOutput.Contains("MATH-RS405-NUGET-DEPENDENCY|version=20260.20261026.1010|archiveSha256=60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf|dllSha256=b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61|PASS", StringComparison.Ordinal) &&
    File.Exists("docs/planning/decisions/m10-isolated-preflight-inspect-rs406.md"), "RS405 tool scope or next material differs.");
Console.WriteLine("MATH-RS406-BASELINE|RS405=PASS|corrected=true|tools=2|productionMigration=false|PASS");

var inspectorReport = Read(".aura/workflow/reports/RS406/attempt-002.json");
var inspectorProof = Read(".workflow/runs/RS406/preparation-proof.json");
string[] inspectorPreparationIds = ["math-rs406-maintained-tool-evidence", "diff-check"];
var inspectorFailures = inspectorReport["priorInvocationFailures"]!;
Need((string?)inspectorReport["runId"] == "RS406" && (string?)inspectorReport["outcome"] == "succeeded" &&
    (int?)inspectorReport["attempt"] == 2 && (string?)inspectorReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)inspectorReport["correctionMode"] == true && (int?)inspectorReport["correctionSourceAttempt"] == 1 &&
    (int?)inspectorFailures["count"] == 2 && (int?)inspectorFailures["allocatedAttemptFailureCount"] == 1 &&
    (int?)inspectorFailures["preparationFailureCount"] == 1 && (bool?)inspectorFailures["firstPassSucceeded"] == false &&
    inspectorFailures["invocationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] {
        "run-next-20261127T181002Z-f04a16ca27e826cc", "run-next-20261127T181135Z-cc0b237bf640489f" }) &&
    inspectorReport["boundaryViolations"]!.AsArray().Count == 0 && inspectorReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)inspectorProof["sourceCommit"] == "b08e8a4bc60dc6c193e7a59e066bda038047fd5b" &&
    (bool?)inspectorProof["payloadCompiled"] == true && (int?)inspectorProof["rehearsedValidationCount"] == 2 &&
    inspectorProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(inspectorPreparationIds) &&
    inspectorReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(inspectorPreparationIds),
    "RS406 corrected inspector evidence differs.");
var inspectorValidation = inspectorReport["validations"]!.AsArray().Single()!;
Need((string?)inspectorValidation["Id"] == "math-rs406-isolated-nuget-inspect" && (int?)inspectorValidation["ExitCode"] == 0 &&
    (bool?)inspectorValidation["TimedOut"] == false && (bool?)inspectorValidation["Cancelled"] == false, "RS406 validation differs.");
var inspectorOutput = (string)inspectorValidation["StandardOutput"]!;
Need(inspectorOutput.Contains("MATH-RS406-INSPECTOR|buildSdk=11.0.100|targets=2|packagesPerTarget=13|sourceParity=PASS|negative=PASS|production=false|publication=false|PASS", StringComparison.Ordinal) &&
    inspectorOutput.Contains("MATH-RS406-SDK|context=isolated-baseline|expected=10.0.404|observed=10.0.404", StringComparison.Ordinal) &&
    inspectorOutput.Contains("MATH-RS406-CANDIDATE|sourceSha256=f79b6917884841673d5523296a64f9298e0c0a8e0ee06ac7003730e8dd8cee6e|production=false", StringComparison.Ordinal) &&
    File.Exists("docs/planning/decisions/m10-isolated-rollback-rs407.md"), "RS406 inspector scope or next material differs.");
Console.WriteLine("MATH-RS407-BASELINE|RS406=PASS|corrected=true|targets=2|productionMigration=false|PASS");

var rollbackReport = Read(".aura/workflow/reports/RS407/attempt-001.json");
var rollbackProof = Read(".workflow/runs/RS407/preparation-proof.json");
string[] rollbackPreparationIds = ["math-rs407-inspector-evidence", "diff-check"];
Need((string?)rollbackReport["runId"] == "RS407" && (string?)rollbackReport["outcome"] == "succeeded" &&
    (int?)rollbackReport["attempt"] == 1 && (string?)rollbackReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)rollbackReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)rollbackReport["priorInvocationFailures"]?["count"] == 0 &&
    rollbackReport["boundaryViolations"]!.AsArray().Count == 0 && rollbackReport["progress"]!["transitions"]!.AsArray().Count == 0 &&
    (string?)rollbackProof["sourceCommit"] == "0eb5baca1007d2043531fb79543bedb92d3a0fc2" &&
    (bool?)rollbackProof["payloadCompiled"] == true && (int?)rollbackProof["rehearsedValidationCount"] == 2 &&
    rollbackProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(rollbackPreparationIds) &&
    rollbackReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(rollbackPreparationIds),
    "RS407 durable rollback evidence differs.");
var rollbackValidation = rollbackReport["validations"]!.AsArray().Single()!;
Need((string?)rollbackValidation["Id"] == "math-rs407-isolated-rollback" && (int?)rollbackValidation["ExitCode"] == 0 &&
    (bool?)rollbackValidation["TimedOut"] == false && (bool?)rollbackValidation["Cancelled"] == false, "RS407 validation differs.");
var rollbackOutput = (string)rollbackValidation["StandardOutput"]!;
Need(rollbackOutput.Contains("MATH-RS407-ROLLBACK|sdk=10.0.404|tfm=net10.0|restored=30|projects=24|lockedRestore=PASS|build=PASS|smoke=PASS|production=false|publication=false|PASS", StringComparison.Ordinal) &&
    rollbackOutput.Contains("MATH-RS407-TARGET|sdk=11.0.100|projects=24|changedInputs=28|PASS", StringComparison.Ordinal) &&
    rollbackOutput.Contains("MATH-RS407-SMOKE|sdk=10.0.404|tfm=net10.0|classes=3|passed=9|failed=0|PASS", StringComparison.Ordinal) &&
    rollbackOutput.Contains("trxSha256=caf98e894b15c8fbba861b782af8af6a64ee44ae04772bd0994ca64b8b5ce98d|PASS", StringComparison.Ordinal),
    "RS407 rollback scope differs.");
var restoredPaths = redLedger["rollbackInputs"]!.AsArray().Select(x => (string)x!["path"]!).Concat(new[] {
    "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj",
    "tools/dw.tools.math.nuget-preflight/Program.cs" }).ToHashSet(StringComparer.Ordinal);
var restoredRows = rollbackOutput.Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.StartsWith("MATH-RS407-RESTORED|", StringComparison.Ordinal)).ToArray();
var restoredObserved = new HashSet<string>(StringComparer.Ordinal);
foreach (var line in restoredRows)
{
    var match = System.Text.RegularExpressions.Regex.Match(line, @"^MATH-RS407-RESTORED\|path=(.+)\|sha256=([0-9a-f]{64})\|PASS$");
    Need(match.Success && restoredObserved.Add(match.Groups[1].Value), "Invalid or duplicate rollback row.");
    var bytes = File.ReadAllBytes(match.Groups[1].Value);
    Need(Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes)) == match.Groups[2].Value, "Current rollback input differs: " + match.Groups[1].Value);
}
var restoredProjects = rollbackOutput.Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.StartsWith("MATH-RS407-PROJECT|", StringComparison.Ordinal)).ToArray();
var restoredProjectPaths = new HashSet<string>(StringComparer.Ordinal);
foreach (var line in restoredProjects)
{
    var match = System.Text.RegularExpressions.Regex.Match(line, @"^MATH-RS407-PROJECT\|path=(.+)\|target=net10\.0\|dllSha256=[0-9a-f]{64}\|PASS$");
    Need(match.Success && restoredProjectPaths.Add(match.Groups[1].Value), "Invalid or duplicate rollback project row.");
}
Need(restoredRows.Length == 30 && restoredObserved.SetEquals(restoredPaths) && restoredProjects.Length == 24 &&
    restoredProjectPaths.SetEquals(projects) && File.Exists("docs/planning/decisions/m10-cutover-readiness-green-closure-rs408.md"), "Rollback coverage or closure decision differs.");
Console.WriteLine("MATH-RS408-GREEN-CLOSURE|RS402-RS407=PASS|restored=30|projects=24|nativeGreen=done|verification=ready|production=false|publication=false|PASS");
