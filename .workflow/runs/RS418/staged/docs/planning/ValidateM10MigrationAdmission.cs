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
Need((string?)backlog["plan_version"] == "0.1.185" && (string?)backlog["status"] == "in_progress", "Aggregate target differs.");
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
Need(canonical.Count == 636 && current.Length == 17 && current.All(x => (string?)x["status"] == ((string?)x["id"] switch { "M10" or "M10-W01" => "in_progress", "M10-W01-C01" or "M10-W01-C01-T1" or "M10-W01-C01-T1-R" or "M10-W01-C01-T1-G" or "M10-W01-C01-T1-V" => "done", "M10-W01-C02" or "M10-W01-C02-T1" or "M10-W01-C02-T1-R" or "M10-W01-C02-T1-G" or "M10-W01-C02-T1-V" => "done", "M10-W01-C03" or "M10-W01-C03-T1" or "M10-W01-C03-T1-R" => "in_progress", _ => "planned" }) && ((string?)x["id"] switch { "M10-W01-C01-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS401/attempt-001.json", "M10-W01-C01-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS408/attempt-001.json", "M10-W01-C01" or "M10-W01-C01-T1" or "M10-W01-C01-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS410/attempt-001.json", "M10-W01-C02-T1-R" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS412/attempt-001.json", "M10-W01-C02-T1-G" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS414/attempt-001.json", "M10-W01-C02" or "M10-W01-C02-T1" or "M10-W01-C02-T1-V" => x["evidence"]!.AsArray().Count == 1 && (string?)x["evidence"]![0] == ".aura/workflow/reports/RS416/attempt-001.json", _ => x["evidence"]!.AsArray().Count == 0 })) &&
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
Need(native.Count == 630 && newNative.Length == 17 && newNative.All(x => (string?)x["state"] == ((string?)x["id"] switch { "M10" or "M10-W01" => "in-progress", "M10-W01-C01" or "M10-W01-C01-T1" or "M10-W01-C01-T1-R" or "M10-W01-C01-T1-G" or "M10-W01-C01-T1-V" => "done", "M10-W01-C02" or "M10-W01-C02-T1" or "M10-W01-C02-T1-R" or "M10-W01-C02-T1-G" or "M10-W01-C02-T1-V" => "done", "M10-W01-C03" or "M10-W01-C03-T1" => "in-progress", "M10-W01-C03-T1-R" => "done", "M10-W01-C03-T1-G" => "ready", _ => "not-ready" })) &&
    native.Except(newNative).Where(x => !((string)x["id"]!).StartsWith("M11", StringComparison.Ordinal)).All(x => (string?)x["state"] == "done"), "Native admission or prior completion differs.");
Need(current.Select(x => (string?)x["id"]).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(
    newNative.Select(x => (string?)x["id"]).OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal), "M10 roster mismatch.");
Need((string?)Read("global.json")["sdk"]?["version"] == "11.0.100" && (string?)Read("global.json")["sdk"]?["rollForward"] == "disable", "Actual cutover SDK differs.");
var props = XDocument.Load("Directory.Build.props");
Need(props.Descendants("TargetFramework").Single().Value == "net11.0" && props.Descendants("LangVersion").Single().Value == "15.0" && props.Descendants("TreatWarningsAsErrors").Single().Value == "true", "Actual cutover target differs.");
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
    // RS408 independently checked the actual 30 baseline hashes before migration.
    // The 28 authorized changes are now checked by VerifyM10ProductionCutover --target.
    // Retained rollback inputs must still match the original observed bytes.
    if (match.Groups[1].Value is "Directory.Packages.props" or "dw.tools.math.slnx")
    {
        var bytes = File.ReadAllBytes(match.Groups[1].Value);
        Need(Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes)) == match.Groups[2].Value, "Retained rollback input differs: " + match.Groups[1].Value);
    }
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

var greenClosureReport = Read(".aura/workflow/reports/RS408/attempt-001.json");
var greenClosureProof = Read(".workflow/runs/RS408/preparation-proof.json");
string[] greenClosureIds = ["math-rs408-m11-canonical-reconciliation","math-rs408-native-roadmap-parity","math-rs408-m10-green-closure","math-rs408-m9-admission","math-rs408-published-handoff","math-rs408-request-reconciliation","math-rs408-m8-evidence-frontier","math-rs408-planning-render-check","diff-check"];
var greenClosureMoves = greenClosureReport["progress"]!["transitions"]!.AsArray();
Need((string?)greenClosureReport["runId"] == "RS408" && (string?)greenClosureReport["outcome"] == "succeeded" &&
    (int?)greenClosureReport["attempt"] == 1 && (string?)greenClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)greenClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)greenClosureReport["priorInvocationFailures"]?["count"] == 0 &&
    greenClosureReport["boundaryViolations"]!.AsArray().Count == 0 &&
    greenClosureMoves.Count == 2 &&
    (string?)greenClosureMoves[0]!["id"] == "M10-W01-C01-T1-G" &&
    (string?)greenClosureMoves[0]!["beforeState"] == "in-progress" && (string?)greenClosureMoves[0]!["afterState"] == "done" &&
    (string?)greenClosureMoves[1]!["id"] == "M10-W01-C01-T1-V" &&
    (string?)greenClosureMoves[1]!["beforeState"] == "not-ready" && (string?)greenClosureMoves[1]!["afterState"] == "ready" &&
    greenClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)greenClosureProof["sourceCommit"] == "2e8b529bcd66c1c3fc0001ac90d920054cdf5b1e" &&
    (bool?)greenClosureProof["payloadCompiled"] == true && (int?)greenClosureProof["rehearsedValidationCount"] == 9 &&
    greenClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(greenClosureIds) &&
    greenClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(greenClosureIds) &&
    File.Exists("docs/planning/decisions/m10-cutover-readiness-verification-rs409.md"), "RS408 durable GREEN closure differs.");
Console.WriteLine("MATH-RS409-VERIFICATION-ACTIVATION|RS408=PASS|nativeGreen=done|verification=in-progress|production=false|PASS");

var verificationReport = Read(".aura/workflow/reports/RS409/attempt-001.json");
var verificationProof = Read(".workflow/runs/RS409/preparation-proof.json");
string[] verificationIds = ["math-rs409-m11-canonical-reconciliation", "math-rs409-native-roadmap-parity", "math-rs409-m10-verification-activation", "math-rs409-independent-readiness", "math-rs409-m9-admission", "math-rs409-published-handoff", "math-rs409-request-reconciliation", "math-rs409-m8-evidence-frontier", "math-rs409-planning-render-check", "diff-check"];
var verificationMoves = verificationReport["progress"]!["transitions"]!.AsArray();
Need((string?)verificationReport["runId"] == "RS409" && (string?)verificationReport["outcome"] == "succeeded" &&
    (int?)verificationReport["attempt"] == 1 && (string?)verificationReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)verificationReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)verificationReport["priorInvocationFailures"]?["count"] == 0 &&
    verificationReport["boundaryViolations"]!.AsArray().Count == 0 &&
    verificationMoves.Count == 1 && (string?)verificationMoves[0]!["id"] == "M10-W01-C01-T1-V" &&
    (string?)verificationMoves[0]!["beforeState"] == "ready" && (string?)verificationMoves[0]!["afterState"] == "in-progress" &&
    verificationReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)verificationProof["sourceCommit"] == "2e2b3c8e466b9daa046744ac5541a0645a3280b7" &&
    (bool?)verificationProof["payloadCompiled"] == true && (int?)verificationProof["rehearsedValidationCount"] == 10 &&
    verificationProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(verificationIds) &&
    verificationReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(verificationIds) &&
    File.Exists("docs/planning/decisions/m10-cutover-readiness-closure-rs410.md"), "RS409 independent verification evidence differs.");
Console.WriteLine("MATH-RS410-READINESS-CLOSURE|RS409=PASS|verification=done|C01=done|C02-RED=ready|production=false|publication=false|PASS");

var readinessClosureReport = Read(".aura/workflow/reports/RS410/attempt-001.json");
var readinessClosureProof = Read(".workflow/runs/RS410/preparation-proof.json");
string[] readinessClosureIds = ["math-rs410-m11-canonical-reconciliation", "math-rs410-native-roadmap-parity", "math-rs410-m10-readiness-closure", "math-rs410-m9-admission", "math-rs410-published-handoff", "math-rs410-request-reconciliation", "math-rs410-m8-evidence-frontier", "math-rs410-planning-render-check", "diff-check"];
var readinessMoves = readinessClosureReport["progress"]!["transitions"]!.AsArray();
string[] readinessDone = ["M10-W01-C01", "M10-W01-C01-T1", "M10-W01-C01-T1-V"];
string[] cutoverReady = ["M10-W01-C02", "M10-W01-C02-T1", "M10-W01-C02-T1-R"];
Need((string?)readinessClosureReport["runId"] == "RS410" && (string?)readinessClosureReport["outcome"] == "succeeded" &&
    (int?)readinessClosureReport["attempt"] == 1 && (string?)readinessClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)readinessClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)readinessClosureReport["priorInvocationFailures"]?["count"] == 0 &&
    readinessClosureReport["boundaryViolations"]!.AsArray().Count == 0 && readinessMoves.Count == 6 &&
    readinessMoves.Take(3).Select(x => (string?)x?["id"]).SequenceEqual(readinessDone) &&
    readinessMoves.Take(3).All(x => (string?)x?["beforeState"] == "in-progress" && (string?)x?["afterState"] == "done") &&
    readinessMoves.Skip(3).Select(x => (string?)x?["id"]).SequenceEqual(cutoverReady) &&
    readinessMoves.Skip(3).All(x => (string?)x?["beforeState"] == "not-ready" && (string?)x?["afterState"] == "ready") &&
    readinessClosureReport["progress"]!["completions"]!.AsArray().Count == 3 &&
    (string?)readinessClosureProof["sourceCommit"] == "b4e7fca079bc4871b9739cf64227070fba935826" &&
    (bool?)readinessClosureProof["payloadCompiled"] == true && (int?)readinessClosureProof["rehearsedValidationCount"] == 9 &&
    readinessClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(readinessClosureIds) &&
    readinessClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(readinessClosureIds) &&
    File.Exists("docs/planning/decisions/m10-production-cutover-red-rs411.md"), "RS410 durable readiness closure differs.");
Console.WriteLine("MATH-RS411-CUTOVER-RED-ACTIVATION|RS410=PASS|C01=done|C02-RED=in-progress|production=false|PASS");

var productionRedReport = Read(".aura/workflow/reports/RS411/attempt-001.json");
var productionRedProof = Read(".workflow/runs/RS411/preparation-proof.json");
string[] productionRedIds = ["math-rs411-production-red", "math-rs411-m11-canonical-reconciliation", "math-rs411-native-roadmap-parity", "math-rs411-m10-cutover-red-activation", "math-rs411-m9-admission", "math-rs411-published-handoff", "math-rs411-request-reconciliation", "math-rs411-m8-evidence-frontier", "math-rs411-planning-render-check", "diff-check"];
var productionRedMoves = productionRedReport["progress"]!["transitions"]!.AsArray();
var productionRedPrior = productionRedReport["priorInvocationFailures"]!;
var productionRedFailures = productionRedPrior["failures"]!.AsArray();
Need((string?)productionRedReport["runId"] == "RS411" && (string?)productionRedReport["outcome"] == "succeeded" &&
    (int?)productionRedReport["attempt"] == 1 && (string?)productionRedReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)productionRedReport["correctionMode"] == false &&
    (bool?)productionRedPrior["firstPassSucceeded"] == false &&
    (int?)productionRedPrior["count"] == 1 && (int?)productionRedPrior["preparationFailureCount"] == 1 &&
    (int?)productionRedPrior["allocatedAttemptFailureCount"] == 0 &&
    productionRedFailures.Count == 1 && (string?)productionRedFailures[0]!["invocationId"] == "run-next-20261127T191010Z-924faba16b8e2ac6" &&
    (bool?)productionRedFailures[0]!["attemptAllocated"] == false &&
    (string?)productionRedFailures[0]!["publicationOutcome"] == "preparation-failed" &&
    productionRedReport["boundaryViolations"]!.AsArray().Count == 0 && productionRedMoves.Count == 3 &&
    productionRedMoves.Select(x => (string?)x?["id"]).SequenceEqual(new[] {"M10-W01-C02", "M10-W01-C02-T1", "M10-W01-C02-T1-R"}) &&
    productionRedMoves.All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    productionRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)productionRedProof["sourceCommit"] == "727722532260a8ed9759ae6325ae8bb676f53501" &&
    (bool?)productionRedProof["payloadCompiled"] == true && (int?)productionRedProof["rehearsedValidationCount"] == 10 &&
    productionRedProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(productionRedIds) &&
    productionRedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(productionRedIds) &&
    File.Exists("docs/planning/decisions/m10-production-cutover-red-closure-rs412.md"), "RS411 durable corrected production RED evidence differs.");
Console.WriteLine("MATH-RS412-RED-CLOSURE|RS411=PASS|red=done|green=ready|production=false|PASS");

var cutoverRedClosureReport = Read(".aura/workflow/reports/RS412/attempt-001.json");
var cutoverRedClosureProof = Read(".workflow/runs/RS412/preparation-proof.json");
string[] cutoverRedClosureIds = ["math-rs412-m11-canonical-reconciliation", "math-rs412-native-roadmap-parity", "math-rs412-m10-cutover-red-closure", "math-rs412-m9-admission", "math-rs412-published-handoff", "math-rs412-request-reconciliation", "math-rs412-m8-evidence-frontier", "math-rs412-planning-render-check", "diff-check"];
var cutoverRedClosureMoves = cutoverRedClosureReport["progress"]!["transitions"]!.AsArray();
var cutoverRedClosurePrior = cutoverRedClosureReport["priorInvocationFailures"]!;
Need((string?)cutoverRedClosureReport["runId"] == "RS412" && (string?)cutoverRedClosureReport["outcome"] == "succeeded" &&
    (int?)cutoverRedClosureReport["attempt"] == 1 && (string?)cutoverRedClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)cutoverRedClosureReport["correctionMode"] == false &&
    (bool?)cutoverRedClosurePrior["firstPassSucceeded"] == false && (int?)cutoverRedClosurePrior["count"] == 1 &&
    (int?)cutoverRedClosurePrior["preparationFailureCount"] == 1 && (int?)cutoverRedClosurePrior["allocatedAttemptFailureCount"] == 0 &&
    (string?)cutoverRedClosurePrior["invocationIds"]![0] == "run-next-20261127T195118Z-20b82322a7a88996" &&
    cutoverRedClosureReport["boundaryViolations"]!.AsArray().Count == 0 && cutoverRedClosureMoves.Count == 2 &&
    (string?)cutoverRedClosureMoves[0]!["id"] == "M10-W01-C02-T1-R" && (string?)cutoverRedClosureMoves[0]!["beforeState"] == "in-progress" && (string?)cutoverRedClosureMoves[0]!["afterState"] == "done" &&
    (string?)cutoverRedClosureMoves[1]!["id"] == "M10-W01-C02-T1-G" && (string?)cutoverRedClosureMoves[1]!["beforeState"] == "not-ready" && (string?)cutoverRedClosureMoves[1]!["afterState"] == "ready" &&
    cutoverRedClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)cutoverRedClosureProof["sourceCommit"] == "8fd75e4de4b31789a18f22fd252a2288dc18946e" &&
    (bool?)cutoverRedClosureProof["payloadCompiled"] == true && (int?)cutoverRedClosureProof["rehearsedValidationCount"] == 9 &&
    cutoverRedClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(cutoverRedClosureIds) &&
    cutoverRedClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(cutoverRedClosureIds) &&
    File.Exists("docs/planning/decisions/m10-production-cutover-green-rs413.md"), "RS412 corrected RED closure evidence differs.");
Console.WriteLine("MATH-RS413-CUTOVER-ACTIVATION|RS412=PASS|green=in-progress|target=net11.0|publication=false|PASS");

var actualCutoverReport = Read(".aura/workflow/reports/RS413/attempt-001.json");
var actualCutoverProof = Read(".workflow/runs/RS413/preparation-proof.json");
string[] actualCutoverPreparationIds = ["math-rs413-actual-target", "math-rs413-m11-canonical-reconciliation", "math-rs413-native-roadmap-parity", "math-rs413-m10-cutover-green-activation", "math-rs413-m9-admission", "math-rs413-published-handoff", "math-rs413-request-reconciliation", "math-rs413-m8-evidence-frontier", "math-rs413-planning-render-check", "diff-check"];
var actualCutoverPrior = actualCutoverReport["priorInvocationFailures"]!;
var actualCutoverMoves = actualCutoverReport["progress"]!["transitions"]!.AsArray();
Need((string?)actualCutoverReport["runId"] == "RS413" && (string?)actualCutoverReport["outcome"] == "succeeded" &&
    (int?)actualCutoverReport["attempt"] == 1 && (string?)actualCutoverReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)actualCutoverReport["correctionMode"] == false && (bool?)actualCutoverPrior["firstPassSucceeded"] == false &&
    (int?)actualCutoverPrior["count"] == 2 && (int?)actualCutoverPrior["preparationFailureCount"] == 2 &&
    (int?)actualCutoverPrior["allocatedAttemptFailureCount"] == 0 &&
    actualCutoverPrior["invocationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] {
        "run-next-20261127T200216Z-56aac8ecafc4a05e", "run-next-20261127T200659Z-dfe3e4b433a636e2" }) &&
    actualCutoverPrior["failures"]!.AsArray().Count == 2 &&
    actualCutoverPrior["failures"]!.AsArray().All(x => (bool?)x?["attemptAllocated"] == false && (string?)x?["publicationOutcome"] == "preparation-failed") &&
    actualCutoverReport["boundaryViolations"]!.AsArray().Count == 0 && actualCutoverMoves.Count == 1 &&
    (string?)actualCutoverMoves[0]!["id"] == "M10-W01-C02-T1-G" && (string?)actualCutoverMoves[0]!["beforeState"] == "ready" &&
    (string?)actualCutoverMoves[0]!["afterState"] == "in-progress" && actualCutoverReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)actualCutoverProof["sourceCommit"] == "c7356be45cd987d94590e82468fa71b4d9fb4144" &&
    (bool?)actualCutoverProof["payloadCompiled"] == true && (int?)actualCutoverProof["rehearsedValidationCount"] == 10 &&
    actualCutoverProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(actualCutoverPreparationIds) &&
    actualCutoverReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(actualCutoverPreparationIds),
    "RS413 corrected production cutover lineage differs.");
var actualCutoverValidation = actualCutoverReport["validations"]!.AsArray().Single()!;
Need((string?)actualCutoverValidation["Id"] == "math-rs413-actual-production" && (int?)actualCutoverValidation["ExitCode"] == 0 &&
    (bool?)actualCutoverValidation["TimedOut"] == false && (bool?)actualCutoverValidation["Cancelled"] == false, "RS413 actual production validation differs.");
var actualCutoverOutput = (string)actualCutoverValidation["StandardOutput"]!;
Need(actualCutoverOutput.Contains("MATH-RS413-PRODUCTION|sdk=11.0.100|tfm=net11.0|language=15.0|projects=24|tests=35|tools=2|bindings=4|inspect=13|lockedRestore=PASS|build=PASS|publication=false|PASS", StringComparison.Ordinal) &&
    actualCutoverOutput.Contains("MATH-RS413-TESTS|project=tests/projects/dw.quantities.tests/dw.quantities.tests.csproj|classes=3|passed=9|trxSha256=c7edb81516dcd6c1514ac18bc82e565bee5532b2235141b8568a7fb8142a5693|PASS", StringComparison.Ordinal) &&
    actualCutoverOutput.Contains("MATH-RS413-TESTS|project=tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj|classes=5|passed=26|trxSha256=e8642d7433319b9d41baa682c2cee8e63ae4267cfe1d5c5b4d803a33b46ce369|PASS", StringComparison.Ordinal) &&
    actualCutoverOutput.Contains("MATH-RS413-DISCOVERY|bindings=4|dllSha256=25b3fdc36513dd3ad0265f99abc3b8e227fce6d21dbdf580657234ca3b5153fe|PASS", StringComparison.Ordinal) &&
    actualCutoverOutput.Contains("MATH-RS413-INSPECT|packages=13|queries=13|selfTests=5|dllSha256=d003a1a17e9ffea3640b06745a0699531bd5cffae3292ffd2aedfd1efd8f7ce2|publication=false|PASS", StringComparison.Ordinal),
    "RS413 actual build, tests or maintained tool evidence differs.");
var actualCutoverRows = actualCutoverOutput.Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.StartsWith("MATH-RS413-PROJECT|", StringComparison.Ordinal)).ToArray();
var actualCutoverProjects = new HashSet<string>(StringComparer.Ordinal);
foreach (var line in actualCutoverRows)
{
    var match = System.Text.RegularExpressions.Regex.Match(line, @"^MATH-RS413-PROJECT\|path=(.+)\|tfm=net11\.0\|dllSha256=[0-9a-f]{64}\|PASS$");
    Need(match.Success && actualCutoverProjects.Add(match.Groups[1].Value), "Invalid or duplicate actual production project row.");
}
Need(actualCutoverRows.Length == 24 && actualCutoverProjects.SetEquals(projects) && File.Exists("docs/planning/decisions/m10-production-cutover-green-closure-rs414.md"), "Actual cutover coverage differs.");
Console.WriteLine("MATH-RS414-GREEN-CLOSURE|RS413=PASS|projects=24|tests=35|tools=2|green=done|verification=ready|production=net11.0|publication=false|PASS");

var actualGreenClosureReport = Read(".aura/workflow/reports/RS414/attempt-001.json");
var actualGreenClosureProof = Read(".workflow/runs/RS414/preparation-proof.json");
string[] actualGreenClosureIds = ["math-rs414-actual-target", "math-rs414-m11-canonical-reconciliation", "math-rs414-native-roadmap-parity", "math-rs414-m10-cutover-green-closure", "math-rs414-m9-admission", "math-rs414-published-handoff", "math-rs414-request-reconciliation", "math-rs414-m8-evidence-frontier", "math-rs414-planning-render-check", "diff-check"];
var actualGreenClosureMoves = actualGreenClosureReport["progress"]!["transitions"]!.AsArray();
Need((string?)actualGreenClosureReport["runId"] == "RS414" && (string?)actualGreenClosureReport["outcome"] == "succeeded" &&
    (int?)actualGreenClosureReport["attempt"] == 1 && (string?)actualGreenClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    (int?)actualGreenClosureReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)actualGreenClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    actualGreenClosureReport["boundaryViolations"]!.AsArray().Count == 0 && actualGreenClosureMoves.Count == 2 &&
    (string?)actualGreenClosureMoves[0]!["id"] == "M10-W01-C02-T1-G" &&
    (string?)actualGreenClosureMoves[0]!["beforeState"] == "in-progress" && (string?)actualGreenClosureMoves[0]!["afterState"] == "done" &&
    (string?)actualGreenClosureMoves[1]!["id"] == "M10-W01-C02-T1-V" &&
    (string?)actualGreenClosureMoves[1]!["beforeState"] == "not-ready" && (string?)actualGreenClosureMoves[1]!["afterState"] == "ready" &&
    actualGreenClosureReport["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)actualGreenClosureProof["sourceCommit"] == "ce4b5c4727186ab4c26b0e6ff3e74fbddff900c3" &&
    (bool?)actualGreenClosureProof["payloadCompiled"] == true && (int?)actualGreenClosureProof["rehearsedValidationCount"] == 10 &&
    actualGreenClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(actualGreenClosureIds) &&
    actualGreenClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(actualGreenClosureIds) &&
    File.Exists("docs/planning/decisions/m10-production-cutover-verification-rs415.md"), "RS414 durable GREEN closure differs.");
Console.WriteLine("MATH-RS415-VERIFICATION-ACTIVATION|RS414=PASS|green=done|verification=in-progress|production=net11.0|PASS");

var cutoverVerificationReport = Read(".aura/workflow/reports/RS415/attempt-001.json");
var cutoverVerificationProof = Read(".workflow/runs/RS415/preparation-proof.json");
string[] cutoverVerificationIds = ["math-rs415-actual-target", "math-rs415-independent-cutover", "math-rs415-m11-canonical-reconciliation", "math-rs415-native-roadmap-parity", "math-rs415-m10-cutover-verification-activation", "math-rs415-m9-admission", "math-rs415-published-handoff", "math-rs415-request-reconciliation", "math-rs415-m8-evidence-frontier", "math-rs415-planning-render-check", "diff-check"];
var cutoverVerificationMoves = cutoverVerificationReport["progress"]!["transitions"]!.AsArray();
Need((string?)cutoverVerificationReport["runId"] == "RS415" && (string?)cutoverVerificationReport["outcome"] == "succeeded" &&
    (int?)cutoverVerificationReport["attempt"] == 1 && (string?)cutoverVerificationReport["executor"]?["toolVersion"] == "0.1.81" &&
    (int?)cutoverVerificationReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)cutoverVerificationReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    cutoverVerificationReport["boundaryViolations"]!.AsArray().Count == 0 && cutoverVerificationMoves.Count == 1 &&
    (string?)cutoverVerificationMoves[0]!["id"] == "M10-W01-C02-T1-V" &&
    (string?)cutoverVerificationMoves[0]!["beforeState"] == "ready" && (string?)cutoverVerificationMoves[0]!["afterState"] == "in-progress" &&
    cutoverVerificationReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)cutoverVerificationProof["sourceCommit"] == "d801b26d1cf1d59feca4161deed13c04d4ca87f2" &&
    (bool?)cutoverVerificationProof["payloadCompiled"] == true && (int?)cutoverVerificationProof["rehearsedValidationCount"] == 11 &&
    cutoverVerificationProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(cutoverVerificationIds) &&
    cutoverVerificationReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(cutoverVerificationIds) &&
    File.Exists("docs/planning/decisions/m10-production-cutover-closure-rs416.md"), "RS415 independent cutover verification evidence differs.");
Console.WriteLine("MATH-RS416-CUTOVER-CLOSURE|RS415=PASS|verification=done|C02=done|C03-RED=ready|production=net11.0|publication=false|PASS");

var cutoverClosureReport = Read(".aura/workflow/reports/RS416/attempt-001.json");
var cutoverClosureProof = Read(".workflow/runs/RS416/preparation-proof.json");
string[] cutoverClosureIds = ["math-rs416-actual-target", "math-rs416-m11-canonical-reconciliation", "math-rs416-native-roadmap-parity", "math-rs416-m10-cutover-closure", "math-rs416-m9-admission", "math-rs416-published-handoff", "math-rs416-request-reconciliation", "math-rs416-m8-evidence-frontier", "math-rs416-planning-render-check", "diff-check"];
var cutoverClosureMoves = cutoverClosureReport["progress"]!["transitions"]!.AsArray();
Need((string?)cutoverClosureReport["runId"] == "RS416" && (string?)cutoverClosureReport["outcome"] == "succeeded" &&
    (int?)cutoverClosureReport["attempt"] == 1 && (string?)cutoverClosureReport["executor"]?["toolVersion"] == "0.1.81" &&
    (bool?)cutoverClosureReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    (int?)cutoverClosureReport["priorInvocationFailures"]?["count"] == 0 &&
    cutoverClosureReport["boundaryViolations"]!.AsArray().Count == 0 && cutoverClosureMoves.Count == 6 &&
    cutoverClosureMoves.Take(3).Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M10-W01-C02", "M10-W01-C02-T1", "M10-W01-C02-T1-V" }) &&
    cutoverClosureMoves.Take(3).All(x => (string?)x?["beforeState"] == "in-progress" && (string?)x?["afterState"] == "done") &&
    cutoverClosureMoves.Skip(3).Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M10-W01-C03", "M10-W01-C03-T1", "M10-W01-C03-T1-R" }) &&
    cutoverClosureMoves.Skip(3).All(x => (string?)x?["beforeState"] == "not-ready" && (string?)x?["afterState"] == "ready") &&
    cutoverClosureReport["progress"]!["completions"]!.AsArray().Count == 3 &&
    (string?)cutoverClosureProof["sourceCommit"] == "477e011fe657b964b1b03fc08867a998ea9efb17" &&
    (bool?)cutoverClosureProof["payloadCompiled"] == true && (int?)cutoverClosureProof["rehearsedValidationCount"] == 10 &&
    cutoverClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(cutoverClosureIds) &&
    cutoverClosureReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(cutoverClosureIds) &&
    File.Exists("docs/planning/decisions/m10-post-cutover-certification-red-rs417.md"), "RS416 durable cutover closure differs.");
Console.WriteLine("MATH-RS417-CERTIFICATION-RED-ACTIVATION|RS416=PASS|C02=done|C03-RED=in-progress|publication=false|PASS");

var certificationRedReport = Read(".aura/workflow/reports/RS417/attempt-001.json");
var certificationRedProof = Read(".workflow/runs/RS417/preparation-proof.json");
string[] certificationRedIds = ["math-rs417-certification-red", "math-rs417-actual-target", "math-rs417-m11-canonical-reconciliation", "math-rs417-native-roadmap-parity", "math-rs417-m10-certification-red-activation", "math-rs417-m9-admission", "math-rs417-published-handoff", "math-rs417-request-reconciliation", "math-rs417-m8-evidence-frontier", "math-rs417-planning-render-check", "diff-check"];
var certificationRedMoves = certificationRedReport["progress"]!["transitions"]!.AsArray();
Need((string?)certificationRedReport["runId"] == "RS417" && (string?)certificationRedReport["outcome"] == "succeeded" &&
    (int?)certificationRedReport["attempt"] == 1 && (string?)certificationRedReport["executor"]?["toolVersion"] == "0.1.81" &&
    (string?)certificationRedReport["executor"]?["assemblySha256"] == "f6e91746d8ec3ba2ea3d0586658749c440814080499832cfd6256805e054320a" &&
    (int?)certificationRedReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)certificationRedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    certificationRedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    certificationRedMoves.Count == 3 &&
    certificationRedMoves.Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M10-W01-C03", "M10-W01-C03-T1", "M10-W01-C03-T1-R" }) &&
    certificationRedMoves.All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    certificationRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)certificationRedProof["sourceCommit"] == "2ffc2c5ce90547ca1625a78659a3cb29dbfbc5df" &&
    (bool?)certificationRedProof["payloadCompiled"] == true && (int?)certificationRedProof["rehearsedValidationCount"] == 11 &&
    certificationRedProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(certificationRedIds) &&
    certificationRedReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(certificationRedIds) &&
    File.Exists("docs/planning/decisions/m10-post-cutover-certification-red-closure-rs418.md"), "RS417 durable certification RED activation differs.");
Console.WriteLine("MATH-RS418-CERTIFICATION-RED-CLOSURE|RS417=PASS|red=done|green=ready|qualified=false|publication=false|PASS");
