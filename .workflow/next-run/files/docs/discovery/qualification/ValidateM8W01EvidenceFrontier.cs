#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;
using System.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Decision = "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md";
const string Readme = "docs/discovery/README.md";
const string Baseline = "0.1.124";
var decision = File.ReadAllText(Decision);
var readme = File.ReadAllText(Readme);
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS280-M8-W01: " + reason);
}
static JsonDocument Load(string path) => JsonDocument.Parse(File.ReadAllText(path));
using var backlog = Load("docs/planning/backlog.json");
using var native = Load(".aura/workflow/plan/project.json");
using var protocol = Load("docs/discovery/qualification/RS131-portable-protocol-qualified.json");
using var manifest = Load("docs/discovery/qualification/m8-discovery-manifest.json");
using var historical = Load("docs/discovery/qualification/m8-selected-clr-bindings.json");
using var rs131 = Load(".aura/workflow/reports/RS131/attempt-001.json");
using var rs209 = Load(".aura/workflow/reports/RS209/attempt-001.json");
using var rs279 = Load(".aura/workflow/reports/RS279/attempt-001.json");
using var rs288 = Load(".aura/workflow/reports/RS288/attempt-001.json");
using var rs289 = Load(".aura/workflow/reports/RS289/attempt-001.json");
using var rs290 = Load(".aura/workflow/reports/RS290/attempt-001.json");
using var rs291 = Load(".aura/workflow/reports/RS291/attempt-001.json");
using var rs293 = Load(".aura/workflow/reports/RS293/attempt-001.json");
using var rs294 = Load(".aura/workflow/reports/RS294/attempt-001.json");
using var rs296 = Load(".aura/workflow/reports/RS296/attempt-001.json");
using var rs297 = Load(".aura/workflow/reports/RS297/attempt-001.json");
using var rs298 = Load(".aura/workflow/reports/RS298/attempt-001.json");
using var rs299 = Load(".aura/workflow/reports/RS299/attempt-001.json");
var product = backlog.RootElement;
Need(product.GetProperty("plan_version").GetString() == Baseline &&
    decision.Contains("version `" + Baseline + "`", StringComparison.Ordinal), "Canonical backlog version drift.");
foreach (var phaseId in new[] { "M8-W01-C01", "M8-W01-C02" })
    Need(product.GetProperty("chunks").EnumerateArray().Single(x => x.GetProperty("id").GetString() == phaseId)
        .GetProperty("status").GetString() == "planned", "Accepted chunk state drift: " + phaseId);
var m8 = native.RootElement.GetProperty("milestones").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8");
var w1 = m8.GetProperty("workPackages").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W01");
Need(m8.GetProperty("state").GetString() == "in-progress" &&
    w1.GetProperty("state").GetString() == "in-progress",
    "Native M8 and Discovery work-package readiness changed.");
foreach (var phase in w1.GetProperty("phases").EnumerateArray())
{
    Need(phase.GetProperty("state").GetString() ==
        (phase.GetProperty("id").GetString() == "M8-W01-C01" ? "done" : "ready"),
        "Native Discovery phase readiness differs.");
    foreach (var task in phase.GetProperty("tasks").EnumerateArray())
    {
        var targetTaskState = task.GetProperty("id").GetString() switch
        {
            "M8-W01-C01-T1" => "done",
            "M8-W01-C01-T2" => "done",
            _ => "not-ready"
        };
        Need(task.GetProperty("state").GetString() == targetTaskState,
            "Native completed T1 or active selected CLR T2 state differs.");
        foreach (var sub in task.GetProperty("subtasks").EnumerateArray())
        {
            var expected = sub.GetProperty("id").GetString() switch
            {
                "M8-W01-C01-T1-R" => "done",
                "M8-W01-C01-T1-G" => "done",
                "M8-W01-C01-T1-V" => "done",
                "M8-W01-C01-T2-R" => "done",
                "M8-W01-C01-T2-G" => "done",
                "M8-W01-C01-T2-V" => "done",
                _ => "not-ready"
            };
            Need(sub.GetProperty("state").GetString() == expected,
                "Native T1 completion, T2 RED closure or GREEN readiness differs.");
        }
    }
}
var receipt = protocol.RootElement;
Need(receipt.GetProperty("total_cases").GetInt32() == 13 &&
    receipt.GetProperty("accepted_cases").GetInt32() == 2 &&
    receipt.GetProperty("rejected_cases").GetInt32() == 11 &&
    receipt.GetProperty("case_results").GetArrayLength() == 13 &&
    receipt.GetProperty("case_results").EnumerateArray().All(x => x.GetProperty("passed").GetBoolean()),
    "Pinned RS131 full portable corpus proof is not complete.");
foreach (var item in new[]
{
    (Document: rs131, Id: "RS131"),
    (Document: rs209, Id: "RS209"),
    (Document: rs279, Id: "RS279")
})
{
    var root = item.Document.RootElement;
    Need(root.GetProperty("runId").GetString() == item.Id &&
        root.GetProperty("outcome").GetString() == "succeeded" &&
        root.GetProperty("attempt").GetInt32() == 1 &&
        root.GetProperty("boundaryViolations").GetArrayLength() == 0,
        "Durable execution report changed: " + item.Id);
}
var latest = rs279.RootElement;
Need(latest.GetProperty("git").GetProperty("commitMessage").GetString() ==
        "test: qualify published Discovery CLR bindings on net11" &&
    latest.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    latest.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    latest.GetProperty("changedPaths").EnumerateArray().Single().GetString() ==
        "docs/engineering/ProbeM8Net11PublishedDiscoveryBindings.cs" &&
    latest.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-discovery-four-selected-published-binary-net11-locked-consumer"),
    "RS279 independent package-only qualification changed.");
var previousRed = rs288.RootElement;
var previousGreen = rs289.RootElement;
Need(previousRed.GetProperty("runId").GetString() == "RS288" &&
    previousRed.GetProperty("outcome").GetString() == "succeeded" &&
    previousRed.GetProperty("attempt").GetInt32() == 1 &&
    previousRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    previousRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    previousRed.GetProperty("progress").GetProperty("completions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W01-C01-T1-R" &&
    previousGreen.GetProperty("runId").GetString() == "RS289" &&
    previousGreen.GetProperty("outcome").GetString() == "succeeded" &&
    previousGreen.GetProperty("attempt").GetInt32() == 1 &&
    previousGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    previousGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    previousGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    previousGreen.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W01-C01-T1-G" &&
    previousGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() == "m8-green-actual-attestation-pre-receipt-refusals"),
    "RS288 RED or RS289 GREEN durable execution evidence differs.");
var greenDone = rs290.RootElement;
var verification = rs291.RootElement;
Need(greenDone.GetProperty("runId").GetString() == "RS290" &&
    greenDone.GetProperty("outcome").GetString() == "succeeded" &&
    greenDone.GetProperty("attempt").GetInt32() == 1 &&
    greenDone.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    greenDone.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    greenDone.GetProperty("progress").GetProperty("completions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W01-C01-T1-G" &&
    verification.GetProperty("runId").GetString() == "RS291" &&
    verification.GetProperty("outcome").GetString() == "succeeded" &&
    verification.GetProperty("attempt").GetInt32() == 1 &&
    verification.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    verification.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    verification.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    verification.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W01-C01-T1-V" &&
    verification.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("afterState").GetString() == "in-progress" &&
    verification.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(item =>
            item.GetProperty("id").GetString() ==
            "m8-verification-independent-public-export-consumer"),
    "RS290 GREEN or RS291 independent Verification durable evidence differs.");
var clrRed = rs293.RootElement;
var clrAdmission = rs294.RootElement;
Need(clrRed.GetProperty("runId").GetString() == "RS293" &&
    clrRed.GetProperty("outcome").GetString() == "succeeded" &&
    clrRed.GetProperty("attempt").GetInt32() == 1 &&
    clrRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    clrRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    clrRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    clrRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Count(x => x.GetProperty("afterState").GetString() == "in-progress") == 2 &&
    clrRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-selected-clr-red-published-reflection-falsifications") &&
    clrAdmission.GetProperty("runId").GetString() == "RS294" &&
    clrAdmission.GetProperty("outcome").GetString() == "succeeded" &&
    clrAdmission.GetProperty("attempt").GetInt32() == 1 &&
    clrAdmission.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    clrAdmission.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    clrAdmission.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    clrAdmission.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    clrAdmission.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-real-published-clr-admission-four-selected") &&
    clrAdmission.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-real-published-clr-reject-coherent-forgeries"),
    "RS293 actual published CLR RED or RS294 real method admission evidence changed.");
var greenConsumer = rs296.RootElement;
var priorGreenFailures = greenConsumer.GetProperty("priorInvocationFailures");
Need(greenConsumer.GetProperty("runId").GetString() == "RS296" &&
    greenConsumer.GetProperty("outcome").GetString() == "succeeded" &&
    greenConsumer.GetProperty("attempt").GetInt32() == 1 &&
    greenConsumer.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    priorGreenFailures.GetProperty("count").GetInt32() == 1 &&
    priorGreenFailures.GetProperty("preparationFailureCount").GetInt32() == 1 &&
    priorGreenFailures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !priorGreenFailures.GetProperty("firstPassSucceeded").GetBoolean() &&
    greenConsumer.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W01-C01-T2-G" &&
    greenConsumer.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() == "m8-green-independent-real-clr-consumer"),
    "RS296 independent GREEN qualification or pre-attempt failure history changed.");
var greenClosure = rs297.RootElement;
var verificationProof = rs298.RootElement;
Need(greenClosure.GetProperty("runId").GetString() == "RS297" &&
    greenClosure.GetProperty("outcome").GetString() == "succeeded" &&
    greenClosure.GetProperty("attempt").GetInt32() == 1 &&
    greenClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 2 &&
    greenClosure.GetProperty("priorInvocationFailures").GetProperty("preparationFailureCount").GetInt32() == 2 &&
    greenClosure.GetProperty("priorInvocationFailures").GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !greenClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    greenClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W01-C01-T2-G" &&
    verificationProof.GetProperty("runId").GetString() == "RS298" &&
    verificationProof.GetProperty("outcome").GetString() == "succeeded" &&
    verificationProof.GetProperty("attempt").GetInt32() == 1 &&
    verificationProof.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    verificationProof.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    verificationProof.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    verificationProof.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W01-C01-T2-V" &&
    verificationProof.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("afterState").GetString() == "in-progress" &&
    verificationProof.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(v => v.GetProperty("id").GetString() ==
            "m8-selected-clr-isolated-portable-verification"),
    "RS297 GREEN completion or RS298 independent published-CLR Verification evidence changed.");
var taskClosure = rs299.RootElement;
Need(taskClosure.GetProperty("runId").GetString() == "RS299" &&
    taskClosure.GetProperty("outcome").GetString() == "succeeded" &&
    taskClosure.GetProperty("attempt").GetInt32() == 1 &&
    taskClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1 &&
    taskClosure.GetProperty("priorInvocationFailures").GetProperty("preparationFailureCount").GetInt32() == 1 &&
    taskClosure.GetProperty("priorInvocationFailures").GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !taskClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    taskClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    taskClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Select(x => x.GetProperty("id").GetString()).OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "M8-W01-C01-T2", "M8-W01-C01-T2-V" }, StringComparer.Ordinal),
    "RS299 selected CLR Verification and parent completion evidence changed.");
var selection = historical.RootElement;
var sourceManifest = manifest.RootElement;
Need(selection.GetProperty("bindings").GetArrayLength() == 4 &&
    !selection.GetProperty("packageConsumerQualified").GetBoolean() &&
    !selection.GetProperty("auraIntegrationVerified").GetBoolean() &&
    selection.GetProperty("brokerReadyCount").GetInt32() == 0 &&
    sourceManifest.GetProperty("capabilities").GetArrayLength() == 4 &&
    sourceManifest.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    sourceManifest.GetProperty("capabilities").EnumerateArray().All(x =>
        x.GetProperty("packageVersion").GetString() == "0.3.0-preview.1" &&
        x.GetProperty("exposure").GetString() == "AgentCallable" &&
        x.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only"),
    "Source manifest, selected binding, or runtime authorization was silently promoted.");
var packages = new[]
{
    ("dw.tools.math.composition", "20260.20261107.1001", "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f"),
    ("dw.tools.math.ir", "20260.20261104.1001", "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480"),
    ("dw.quantities", "20260.20261102.1001", "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d")
};
foreach (var (id, version, archiveHash) in packages)
    Need(decision.Contains(id + " " + version, StringComparison.Ordinal) &&
        decision.Contains(archiveHash, StringComparison.Ordinal),
        "RS279 package identity missing from evidence frontier: " + id);
Need(decision.Contains("RS131", StringComparison.Ordinal) &&
    decision.Contains("RS209", StringComparison.Ordinal) &&
    decision.Contains("RS279", StringComparison.Ordinal) &&
    decision.Contains("forward-only", StringComparison.Ordinal) &&
    decision.Contains("not-ready", StringComparison.Ordinal) &&
    decision.Contains("broker", StringComparison.OrdinalIgnoreCase) &&
    readme.Contains("RS279", StringComparison.Ordinal) &&
    readme.Contains("source-free", StringComparison.Ordinal),
    "Documentation lost qualified evidence or external boundary.");
Console.WriteLine("MATH-M8-DISCOVERY-FRONTIER|portable=13|accept=2|reject=11|" +
    "selected=4|binaryConsumer=RS279|firstPass=true|packages=3|M8=in-progress|W01=in-progress|C01=done|C02=ready|T1=done|RED=done|GREEN=done|VERIFY=done|T2=done|T2RED=done|T2GREEN=done|T2VERIFY=done|brokerReady=0|PASS");
