#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;
using System.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Decision = "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md";
const string Readme = "docs/discovery/README.md";
const string Baseline = "0.1.171";
const string HistoricalDecisionBaseline = "0.1.143";
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
using var rs301 = Load(".aura/workflow/reports/RS301/attempt-001.json");
using var rs303 = Load(".aura/workflow/reports/RS303/attempt-001.json");
using var rs305 = Load(".aura/workflow/reports/RS305/attempt-001.json");
using var rs307 = Load(".aura/workflow/reports/RS307/attempt-001.json");
using var rs309 = Load(".aura/workflow/reports/RS309/attempt-001.json");
using var rs311 = Load(".aura/workflow/reports/RS311/attempt-001.json");
using var rs312 = Load(".aura/workflow/reports/RS312/attempt-001.json");
using var rs314 = Load(".aura/workflow/reports/RS314/attempt-001.json");
using var rs315 = Load(".aura/workflow/reports/RS315/attempt-001.json");
using var rs316 = Load(".aura/workflow/reports/RS316/attempt-001.json");
using var rs317 = Load(".aura/workflow/reports/RS317/attempt-001.json");
using var rs318 = Load(".aura/workflow/reports/RS318/attempt-001.json");
using var rs319 = Load(".aura/workflow/reports/RS319/attempt-001.json");
using var rs320 = Load(".aura/workflow/reports/RS320/attempt-001.json");
using var rs321 = Load(".aura/workflow/reports/RS321/attempt-001.json");
using var rs322 = Load(".aura/workflow/reports/RS322/attempt-001.json");
using var rs323 = Load(".aura/workflow/reports/RS323/attempt-001.json");
using var rs324 = Load(".aura/workflow/reports/RS324/attempt-001.json");
using var rs325 = Load(".aura/workflow/reports/RS325/attempt-001.json");
using var rs326 = Load(".aura/workflow/reports/RS326/attempt-001.json");
using var rs327 = Load(".aura/workflow/reports/RS327/attempt-001.json");
using var rs328 = Load(".aura/workflow/reports/RS328/attempt-001.json");
using var rs329 = Load(".aura/workflow/reports/RS329/attempt-001.json");
using var rs331 = Load(".aura/workflow/reports/RS331/attempt-001.json");
using var rs333 = Load(".aura/workflow/reports/RS333/attempt-001.json");
using var rs335 = Load(".aura/workflow/reports/RS335/attempt-001.json");
using var rs337 = Load(".aura/workflow/reports/RS337/attempt-001.json");
using var rs337Proof = Load(".workflow/runs/RS337/preparation-proof.json");
using var rs345 = Load(".aura/workflow/reports/RS345/attempt-001.json");
using var rs345Proof = Load(".workflow/runs/RS345/preparation-proof.json");
using var rs313 = Load(".aura/workflow/reports/RS313/attempt-001.json");
using var rs346 = Load(".aura/workflow/reports/RS346/attempt-001.json");
using var rs346Proof = Load(".workflow/runs/RS346/preparation-proof.json");
using var c02Acceptance = Load("docs/engineering/m8-w02-c02-phase-acceptance-rs345.json");
using var sdkInventoryMatrix = Load("docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json");
using var publishedSubset = Load("docs/engineering/m8-w02-c01-published-standard-closure-rs322.json");
var product = backlog.RootElement;
Need(product.GetProperty("plan_version").GetString() == Baseline &&
    decision.Contains("version `" + HistoricalDecisionBaseline + "`", StringComparison.Ordinal), "Canonical backlog version drift.");
foreach (var phaseId in new[] { "M8-W01-C01", "M8-W01-C02", "M8-W02-C01", "M8-W02-C02" })
    Need(product.GetProperty("chunks").EnumerateArray().Single(x => x.GetProperty("id").GetString() == phaseId)
        .GetProperty("status").GetString() == "done", "Accepted chunk state drift: " + phaseId);
var m8 = native.RootElement.GetProperty("milestones").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8");
var w1 = m8.GetProperty("workPackages").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W01");
var w2 = m8.GetProperty("workPackages").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02");
Need(m8.GetProperty("state").GetString() == "done" &&
    w1.GetProperty("state").GetString() == "done" &&
    w2.GetProperty("state").GetString() == "done" &&
    w2.GetProperty("phases").EnumerateArray().Count() == 2 &&
    w2.GetProperty("phases").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01")
        .GetProperty("state").GetString() == "done" &&
    w2.GetProperty("phases").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02")
        .GetProperty("state").GetString() == "done",
    "Native M8 Discovery closure or SDK audit readiness changed.");
var auditPhase = w2.GetProperty("phases").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01");
var auditT1 = auditPhase.GetProperty("tasks").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1");
var auditT2 = auditPhase.GetProperty("tasks").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2");
Need(auditT1.GetProperty("state").GetString() == "done" &&
    auditT2.GetProperty("state").GetString() == "done" &&
    auditT2.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-R")
        .GetProperty("state").GetString() == "done" &&
    auditT2.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-G")
        .GetProperty("state").GetString() == "done" &&
    auditT2.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-V")
        .GetProperty("state").GetString() == "done" &&
    auditT1.GetProperty("subtasks").EnumerateArray().Count() == 3 &&
    auditT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-R")
        .GetProperty("state").GetString() == "done" &&
    auditT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-G")
        .GetProperty("state").GetString() == "done" &&
    auditT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-V")
        .GetProperty("state").GetString() == "done",
    "SDK audit T1 RED activation widened to another task or state.");
var nextPhase = w2.GetProperty("phases").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02");
var nextT1 = nextPhase.GetProperty("tasks").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1");
var nextT2 = nextPhase.GetProperty("tasks").EnumerateArray()
    .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02-T2");
Need(nextT1.GetProperty("state").GetString() == "done" &&
    nextT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-R")
        .GetProperty("state").GetString() == "done" &&
    nextT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-G")
        .GetProperty("state").GetString() == "done" &&
    nextT1.GetProperty("subtasks").EnumerateArray()
        .Single(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-V")
        .GetProperty("state").GetString() == "done" &&
    nextT2.GetProperty("state").GetString() == "done" &&
    nextT2.GetProperty("subtasks").EnumerateArray()
        .All(x => x.GetProperty("state").GetString() ==
            (x.GetProperty("id").GetString() switch
            {
                "M8-W02-C02-T2-R" => "done",
                "M8-W02-C02-T2-G" => "done",
                "M8-W02-C02-T2-V" => "done",
                _ => "not-ready"
            })),
    "SDK migration RED readiness exceeded the intended next bounded scope.");
foreach (var phase in w1.GetProperty("phases").EnumerateArray())
{
    Need(phase.GetProperty("state").GetString() == "done",
        "Native Discovery phase completion differs.");
    foreach (var task in phase.GetProperty("tasks").EnumerateArray())
    {
        var targetTaskState = task.GetProperty("id").GetString() switch
        {
            "M8-W01-C01-T1" => "done",
            "M8-W01-C01-T2" => "done",
            "M8-W01-C02-T1" => "done",
            "M8-W01-C02-T2" => "done",
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
                "M8-W01-C02-T1-R" => "done",
                "M8-W01-C02-T1-G" => "done",
                "M8-W01-C02-T1-V" => "done",
                "M8-W01-C02-T2-R" => "done",
                "M8-W01-C02-T2-G" => "done",
                "M8-W01-C02-T2-V" => "done",
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
var skillRed = rs301.RootElement;
Need(skillRed.GetProperty("runId").GetString() == "RS301" &&
    skillRed.GetProperty("outcome").GetString() == "succeeded" &&
    skillRed.GetProperty("attempt").GetInt32() == 1 &&
    skillRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1 &&
    skillRed.GetProperty("priorInvocationFailures").GetProperty("preparationFailureCount").GetInt32() == 1 &&
    skillRed.GetProperty("priorInvocationFailures").GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !skillRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    skillRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    skillRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W01-C02-T1-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    skillRed.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(x => x.GetProperty("id").GetString() == "m8-skill-generator-red-isolated-drift"),
    "RS301 isolated skill-generator RED evidence or preselection history changed.");
var skillGreen = rs303.RootElement;
Need(skillGreen.GetProperty("runId").GetString() == "RS303" &&
    skillGreen.GetProperty("outcome").GetString() == "succeeded" &&
    skillGreen.GetProperty("attempt").GetInt32() == 1 &&
    skillGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    skillGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    skillGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    skillGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W01-C02-T1-G" &&
    skillGreen.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(v => v.GetProperty("id").GetString() ==
            "m8-skill-green-independent-strict-generation") &&
    skillGreen.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(v => v.GetProperty("id").GetString() ==
            "m8-skill-red-existing-regression"),
    "RS303 strict generator GREEN or prior RED regression evidence changed.");
var copiedSkillProof = rs305.RootElement;
Need(copiedSkillProof.GetProperty("runId").GetString() == "RS305" &&
    copiedSkillProof.GetProperty("outcome").GetString() == "succeeded" &&
    copiedSkillProof.GetProperty("attempt").GetInt32() == 1 &&
    copiedSkillProof.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    copiedSkillProof.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    copiedSkillProof.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    copiedSkillProof.GetProperty("progress").GetProperty("transitions").EnumerateArray().Single()
        .GetProperty("id").GetString() == "M8-W01-C02-T1-V" &&
    copiedSkillProof.GetProperty("contribution").GetProperty("observed").GetProperty("validations")
        .EnumerateArray().Any(v => v.GetProperty("id").GetString() ==
            "m8-skill-copied-independent-verification"),
    "RS305 independent copied skill Verification evidence changed.");
var deliveryRed = rs307.RootElement;
Need(deliveryRed.GetProperty("runId").GetString() == "RS307" &&
    deliveryRed.GetProperty("outcome").GetString() == "succeeded" &&
    deliveryRed.GetProperty("attempt").GetInt32() == 1 &&
    deliveryRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    deliveryRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    deliveryRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    deliveryRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W01-C02-T2-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    deliveryRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-delivery-red-independent-portable-refusals"),
    "RS307 independent portable delivery RED proof differs.");
var deliveryGreen = rs309.RootElement;
Need(deliveryGreen.GetProperty("runId").GetString() == "RS309" &&
    deliveryGreen.GetProperty("outcome").GetString() == "succeeded" &&
    deliveryGreen.GetProperty("attempt").GetInt32() == 1 &&
    deliveryGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    deliveryGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    deliveryGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    deliveryGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W01-C02-T2-G" &&
    deliveryGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    deliveryGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-delivery-green-reproducible-independent-exports"),
    "RS309 independent opt-in portable delivery GREEN evidence changed.");
var deliveredConsumer = rs311.RootElement;
var rs311Prior = deliveredConsumer.GetProperty("priorInvocationFailures");
Need(deliveredConsumer.GetProperty("runId").GetString() == "RS311" &&
    deliveredConsumer.GetProperty("outcome").GetString() == "succeeded" &&
    deliveredConsumer.GetProperty("attempt").GetInt32() == 1 &&
    rs311Prior.GetProperty("count").GetInt32() == 1 &&
    rs311Prior.GetProperty("preparationFailureCount").GetInt32() == 1 &&
    rs311Prior.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !rs311Prior.GetProperty("firstPassSucceeded").GetBoolean() &&
    deliveredConsumer.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    deliveredConsumer.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("id").GetString() ==
            "M8-W01-C02-T2-V" &&
    deliveredConsumer.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("afterState").GetString() ==
            "in-progress" &&
    deliveredConsumer.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-delivery-verification-pinned-net11-package-consumer"),
    "RS311 portable package-only Verification or preparation refusal lineage changed.");
var discoveryClosure = rs312.RootElement;
Need(discoveryClosure.GetProperty("runId").GetString() == "RS312" &&
    discoveryClosure.GetProperty("outcome").GetString() == "succeeded" &&
    discoveryClosure.GetProperty("attempt").GetInt32() == 1 &&
    discoveryClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    discoveryClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    discoveryClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    discoveryClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Select(x => x.GetProperty("id").GetString())
        .OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "M8-W01-C02-T2", "M8-W01-C02-T2-V" }, StringComparer.Ordinal),
    "RS312 delivery Verification/task closure or first-pass evidence changed.");
var sdkRed = rs314.RootElement;
Need(sdkRed.GetProperty("runId").GetString() == "RS314" &&
    sdkRed.GetProperty("outcome").GetString() == "succeeded" &&
    sdkRed.GetProperty("attempt").GetInt32() == 1 &&
    sdkRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    sdkRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-sdk11-stable-source-baseline") &&
    sdkRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-packable-source-reference-graph") &&
    sdkRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-test-project-coverage-red") &&
    sdkRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-prior-net11-historical-evidence"),
    "RS314 audited SDK, library graph, eleven tests or historical evidence changed.");
var sdkRedClosure = rs315.RootElement;
Need(sdkRedClosure.GetProperty("runId").GetString() == "RS315" &&
    sdkRedClosure.GetProperty("outcome").GetString() == "succeeded" &&
    sdkRedClosure.GetProperty("attempt").GetInt32() == 1 &&
    sdkRedClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkRedClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkRedClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkRedClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T1-R" &&
    sdkRedClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-G" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS315 SDK inventory RED completion and GREEN readiness changed.");
var officialDeltaGreen = rs316.RootElement;
Need(officialDeltaGreen.GetProperty("runId").GetString() == "RS316" &&
    officialDeltaGreen.GetProperty("outcome").GetString() == "succeeded" &&
    officialDeltaGreen.GetProperty("attempt").GetInt32() == 1 &&
    officialDeltaGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    officialDeltaGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    officialDeltaGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    officialDeltaGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T1-G" &&
    officialDeltaGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    officialDeltaGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-official-delta-candidate-ownership-and-refusal") &&
    officialDeltaGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-existing-lexical-numerical-api-signals") &&
    officialDeltaGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-original-sdk-source-baseline") &&
    officialDeltaGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-existing-thirteen-source-eleven-test-coverage"),
    "RS316 SDK candidate-only official delta audit or first-pass evidence changed.");
var sdkGreenClosure = rs317.RootElement;
Need(sdkGreenClosure.GetProperty("runId").GetString() == "RS317" &&
    sdkGreenClosure.GetProperty("outcome").GetString() == "succeeded" &&
    sdkGreenClosure.GetProperty("attempt").GetInt32() == 1 &&
    sdkGreenClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkGreenClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkGreenClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkGreenClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T1-G" &&
    sdkGreenClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T1-V" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS317 SDK delta GREEN completion or independent Verification readiness changed.");
var sdkInventoryVerification = rs318.RootElement;
var sdkVerificationPrior = sdkInventoryVerification.GetProperty("priorInvocationFailures");
Need(sdkInventoryVerification.GetProperty("runId").GetString() == "RS318" &&
    sdkInventoryVerification.GetProperty("outcome").GetString() == "succeeded" &&
    sdkInventoryVerification.GetProperty("attempt").GetInt32() == 1 &&
    sdkVerificationPrior.GetProperty("count").GetInt32() == 1 &&
    sdkVerificationPrior.GetProperty("preparationFailureCount").GetInt32() == 1 &&
    sdkVerificationPrior.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !sdkVerificationPrior.GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkInventoryVerification.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkInventoryVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T1-V" &&
    sdkInventoryVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    sdkInventoryVerification.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x => x.GetProperty("id").GetString() ==
            "m8-w02-independent-copied-sdk-delta-verification"),
    "RS318 independent copied inventory Verification or preselection history changed.");
var sdkInventoryClosure = rs319.RootElement;
Need(sdkInventoryClosure.GetProperty("runId").GetString() == "RS319" &&
    sdkInventoryClosure.GetProperty("outcome").GetString() == "succeeded" &&
    sdkInventoryClosure.GetProperty("attempt").GetInt32() == 1 &&
    sdkInventoryClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkInventoryClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkInventoryClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkInventoryClosure.GetProperty("progress").GetProperty("completions")
        .EnumerateArray().Select(x => x.GetProperty("id").GetString())
        .OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "M8-W02-C01-T1", "M8-W02-C01-T1-V" }, StringComparer.Ordinal) &&
    sdkInventoryClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-R" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS319 SDK official inventory completion or package-graph RED readiness changed.");
var sdkGraphRed = rs320.RootElement;
Need(sdkGraphRed.GetProperty("runId").GetString() == "RS320" &&
    sdkGraphRed.GetProperty("outcome").GetString() == "succeeded" &&
    sdkGraphRed.GetProperty("attempt").GetInt32() == 1 &&
    sdkGraphRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkGraphRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkGraphRed.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    sdkGraphRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkGraphRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    sdkGraphRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-w02-independent-copied-package-graph-red-refusals") &&
    sdkGraphRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() == "m8-existing-13-project-dependency-graph"),
    "RS320 independent dependency graph RED or 0.1.72 first-pass evidence changed.");
var graphRedClosure = rs321.RootElement;
Need(graphRedClosure.GetProperty("runId").GetString() == "RS321" &&
    graphRedClosure.GetProperty("outcome").GetString() == "succeeded" &&
    graphRedClosure.GetProperty("attempt").GetInt32() == 1 &&
    graphRedClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    graphRedClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    graphRedClosure.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    graphRedClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    graphRedClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T2-R" &&
    graphRedClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-G" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS321 graph RED closure, 0.1.72 executor or GREEN readiness differs.");
var physicalStandardGreen = rs322.RootElement;
Need(physicalStandardGreen.GetProperty("runId").GetString() == "RS322" &&
    physicalStandardGreen.GetProperty("outcome").GetString() == "succeeded" &&
    physicalStandardGreen.GetProperty("attempt").GetInt32() == 1 &&
    physicalStandardGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    physicalStandardGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    physicalStandardGreen.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    physicalStandardGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    physicalStandardGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T2-G" &&
    physicalStandardGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    physicalStandardGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-w02-published-standard-live-physical-three-archive-closure"),
    "RS322 physical three-archive GREEN entry or first-pass evidence differs.");
var liveStandardLockedGreen = rs323.RootElement;
var rs323Prior = liveStandardLockedGreen.GetProperty("priorInvocationFailures");
Need(liveStandardLockedGreen.GetProperty("runId").GetString() == "RS323" &&
    liveStandardLockedGreen.GetProperty("outcome").GetString() == "succeeded" &&
    liveStandardLockedGreen.GetProperty("attempt").GetInt32() == 1 &&
    rs323Prior.GetProperty("count").GetInt32() == 1 &&
    rs323Prior.GetProperty("preparationFailureCount").GetInt32() == 1 &&
    rs323Prior.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
    !rs323Prior.GetProperty("firstPassSucceeded").GetBoolean() &&
    liveStandardLockedGreen.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    liveStandardLockedGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    liveStandardLockedGreen.GetProperty("git").GetProperty("commitMessage").GetString() ==
        "test: qualify live Math Standard locked restore and pin refusals" &&
    liveStandardLockedGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-rs323-live-locked-three-package-consumer-and-refusals") &&
    liveStandardLockedGreen.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0,
    "RS323 locked Standard consumer or actual pre-attempt refusal history differs.");
var packageGreenClosure = rs324.RootElement;
Need(packageGreenClosure.GetProperty("runId").GetString() == "RS324" &&
    packageGreenClosure.GetProperty("outcome").GetString() == "succeeded" &&
    packageGreenClosure.GetProperty("attempt").GetInt32() == 1 &&
    packageGreenClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    packageGreenClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    packageGreenClosure.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    packageGreenClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    packageGreenClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C01-T2-G" &&
    packageGreenClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C01-T2-V" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS324 first-pass package GREEN closure or Verification readiness changed.");
var receivedPackageVerification = rs325.RootElement;
Need(receivedPackageVerification.GetProperty("runId").GetString() == "RS325" &&
    receivedPackageVerification.GetProperty("outcome").GetString() == "succeeded" &&
    receivedPackageVerification.GetProperty("attempt").GetInt32() == 1 &&
    receivedPackageVerification.GetProperty("priorInvocationFailures")
        .GetProperty("count").GetInt32() == 0 &&
    receivedPackageVerification.GetProperty("priorInvocationFailures")
        .GetProperty("firstPassSucceeded").GetBoolean() &&
    receivedPackageVerification.GetProperty("executor")
        .GetProperty("toolVersion").GetString() == "0.1.72" &&
    receivedPackageVerification.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    receivedPackageVerification.GetProperty("progress").GetProperty("completions")
        .GetArrayLength() == 0 &&
    receivedPackageVerification.GetProperty("progress").GetProperty("transitions")
        .EnumerateArray().Single().GetProperty("id").GetString() == "M8-W02-C01-T2-V" &&
    receivedPackageVerification.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(x =>
            x.GetProperty("id").GetString() ==
            "m8-rs325-received-copy-original-physical-verifier-four-tamper-refusals"),
    "RS325 independent received archive Verification or first-pass history differs.");
var inventoryIds = sdkInventoryMatrix.RootElement.GetProperty("packages")
    .EnumerateArray().Select(x => x.GetProperty("packageId").GetString()!)
    .ToHashSet(StringComparer.Ordinal);
var verifiedArchiveIds = publishedSubset.RootElement.GetProperty("packages")
    .EnumerateArray().Select(x => x.GetProperty("packageId").GetString()!)
    .ToHashSet(StringComparer.Ordinal);
Need(inventoryIds.Count == 13 &&
    verifiedArchiveIds.Count == 3 &&
    verifiedArchiveIds.IsSubsetOf(inventoryIds) &&
    inventoryIds.Except(verifiedArchiveIds, StringComparer.Ordinal).Count() == 10 &&
    sdkInventoryMatrix.RootElement.GetProperty("packages").EnumerateArray().All(x =>
        x.GetProperty("compatibility").GetString() == "unqualified") &&
    publishedSubset.RootElement.GetProperty("limitations").EnumerateArray().Any(x =>
        x.GetString()!.Contains("other ten", StringComparison.Ordinal)),
    "The SDK audit lost its thirteen-versus-three-versus-ten qualification boundary.");
var sdkAuditClosure = rs326.RootElement;
Need(sdkAuditClosure.GetProperty("runId").GetString() == "RS326" &&
    sdkAuditClosure.GetProperty("outcome").GetString() == "succeeded" &&
    sdkAuditClosure.GetProperty("attempt").GetInt32() == 1 &&
    sdkAuditClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    sdkAuditClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    sdkAuditClosure.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    sdkAuditClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    sdkAuditClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Select(x => x.GetProperty("id").GetString())
        .ToHashSet(StringComparer.Ordinal)
        .SetEquals(new[] { "M8-W02-C01-T2-V", "M8-W02-C01-T2", "M8-W02-C01" }) &&
    sdkAuditClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-R" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS326 first-pass SDK audit closure and C02 migration RED readiness differs.");
var migrationRedProof = rs327.RootElement;
Need(migrationRedProof.GetProperty("runId").GetString() == "RS327" &&
    migrationRedProof.GetProperty("outcome").GetString() == "succeeded" &&
    migrationRedProof.GetProperty("attempt").GetInt32() == 1 &&
    migrationRedProof.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    migrationRedProof.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    migrationRedProof.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    migrationRedProof.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    migrationRedProof.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    migrationRedProof.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    migrationRedProof.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-rs327-migration-readiness-falsification-six-controls"),
    "RS327 first-pass migration RED evidence or six refusal qualification differs.");
var migrationRedClosure = rs328.RootElement;
Need(migrationRedClosure.GetProperty("runId").GetString() == "RS328" &&
    migrationRedClosure.GetProperty("outcome").GetString() == "succeeded" &&
    migrationRedClosure.GetProperty("attempt").GetInt32() == 1 &&
    migrationRedClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    migrationRedClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    migrationRedClosure.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    migrationRedClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    migrationRedClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02-T1-R" &&
    migrationRedClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C02-T1-G" &&
            x.GetProperty("afterState").GetString() == "ready"),
    "RS328 first-pass migration RED closure and GREEN readiness changed.");
var foundationSourceGreen = rs329.RootElement;
Need(foundationSourceGreen.GetProperty("runId").GetString() == "RS329" &&
    foundationSourceGreen.GetProperty("outcome").GetString() == "succeeded" &&
    foundationSourceGreen.GetProperty("attempt").GetInt32() == 1 &&
    foundationSourceGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    foundationSourceGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    foundationSourceGreen.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.72" &&
    foundationSourceGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    foundationSourceGreen.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    foundationSourceGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02-T1-G" &&
    foundationSourceGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    foundationSourceGreen.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-rs329-isolated-foundation-net10-net11-parity-and-two-contract-refusals"),
    "RS329 independent Foundation cross-SDK GREEN or first-pass provenance changed.");
var foundationVerification = rs331.RootElement;
Need(foundationVerification.GetProperty("runId").GetString() == "RS331" &&
    foundationVerification.GetProperty("outcome").GetString() == "succeeded" &&
    foundationVerification.GetProperty("attempt").GetInt32() == 1 &&
    foundationVerification.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.73" &&
    foundationVerification.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    foundationVerification.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    foundationVerification.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    foundationVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02-T1-V" &&
    foundationVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    foundationVerification.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-rs331-independent-foundation-source-evidence-and-three-refusals") &&
    foundationVerification.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() ==
            "m8-rs331-targeted-foundation-dual-sdk-source-replay"),
    "RS331 independent Foundation Verification provenance changed.");
var packageRed = rs333.RootElement;
Need(packageRed.GetProperty("runId").GetString() == "RS333" &&
    packageRed.GetProperty("outcome").GetString() == "succeeded" &&
    packageRed.GetProperty("attempt").GetInt32() == 1 &&
    packageRed.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.75" &&
    packageRed.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    packageRed.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    packageRed.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    packageRed.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    packageRed.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Any(x => x.GetProperty("id").GetString() == "M8-W02-C02-T2-R" &&
            x.GetProperty("afterState").GetString() == "in-progress") &&
    packageRed.GetProperty("contribution").GetProperty("observed")
        .GetProperty("validations").EnumerateArray().Any(v =>
            v.GetProperty("id").GetString() == "m8-rs333-source-free-consumer-red-eight-refusals"),
    "RS333 source-free consumer RED provenance changed.");
var statisticsGreen = rs335.RootElement;
Need(statisticsGreen.GetProperty("runId").GetString() == "RS335" &&
    statisticsGreen.GetProperty("outcome").GetString() == "succeeded" &&
    statisticsGreen.GetProperty("attempt").GetInt32() == 1 &&
    statisticsGreen.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.75" &&
    statisticsGreen.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    statisticsGreen.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    statisticsGreen.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    statisticsGreen.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    statisticsGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02-T2-G" &&
    statisticsGreen.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    statisticsGreen.GetProperty("contribution").GetProperty("observed").GetProperty("mutationBoundaryClean").GetBoolean() &&
    statisticsGreen.GetProperty("contribution").GetProperty("observed").GetProperty("validations").GetArrayLength() == 7 &&
    statisticsGreen.GetProperty("contribution").GetProperty("observed").GetProperty("validations").EnumerateArray()
        .Any(v => v.GetProperty("id").GetString() == "m8-rs335-statistics-source-free-locked-consumer"),
    "RS335 bounded Statistics source-free GREEN provenance changed.");
var statisticsVerification = rs337.RootElement;
Need(statisticsVerification.GetProperty("runId").GetString() == "RS337" &&
    statisticsVerification.GetProperty("outcome").GetString() == "succeeded" &&
    statisticsVerification.GetProperty("attempt").GetInt32() == 1 &&
    statisticsVerification.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.75" &&
    statisticsVerification.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    statisticsVerification.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    statisticsVerification.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    statisticsVerification.GetProperty("progress").GetProperty("completions").GetArrayLength() == 0 &&
    statisticsVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02-T2-V" &&
    statisticsVerification.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "in-progress" &&
    statisticsVerification.GetProperty("contribution").GetProperty("observed").GetProperty("mutationBoundaryClean").GetBoolean(),
    "RS337 independent Statistics Verification provenance changed.");
var statisticsValidationIds = statisticsVerification.GetProperty("contribution").GetProperty("observed")
    .GetProperty("validations").EnumerateArray().Select(v => v.GetProperty("id").GetString()).ToArray();
Need(statisticsValidationIds.Length == 6 &&
    statisticsValidationIds.Contains("m8-rs337-independent-statistics-package-evidence-and-three-refusals", StringComparer.Ordinal) &&
    statisticsValidationIds.Contains("m8-rs337-targeted-statistics-locked-consumer-replay", StringComparer.Ordinal) &&
    rs337Proof.RootElement.GetProperty("runId").GetString() == "RS337" &&
    rs337Proof.RootElement.GetProperty("payloadCompiled").GetBoolean() &&
    rs337Proof.RootElement.GetProperty("rehearsedValidationCount").GetInt32() == 6 &&
    rs337Proof.RootElement.GetProperty("preparationValidationIds").EnumerateArray()
        .Select(v => v.GetString()).SequenceEqual(statisticsValidationIds, StringComparer.Ordinal),
    "RS337 independent compiled evidence and targeted replay proof differ.");
var modernizationClosure = rs345.RootElement;
Need(modernizationClosure.GetProperty("runId").GetString() == "RS345" &&
    modernizationClosure.GetProperty("outcome").GetString() == "succeeded" &&
    modernizationClosure.GetProperty("attempt").GetInt32() == 1 &&
    modernizationClosure.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.76" &&
    modernizationClosure.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    modernizationClosure.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    modernizationClosure.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
    modernizationClosure.GetProperty("contribution").GetProperty("observed").GetProperty("mutationBoundaryClean").GetBoolean() &&
    modernizationClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Single().GetProperty("id").GetString() == "M8-W02-C02" &&
    modernizationClosure.GetProperty("progress").GetProperty("transitions").EnumerateArray()
        .Single().GetProperty("afterState").GetString() == "done" &&
    sdkAuditClosure.GetProperty("progress").GetProperty("completions").EnumerateArray()
        .Any(v => v.GetProperty("id").GetString() == "M8-W02-C01" &&
            v.GetProperty("afterState").GetString() == "done"),
    "Both durable modernization phase completions are required.");
var modernizationValidationIds = modernizationClosure.GetProperty("contribution").GetProperty("observed")
    .GetProperty("validations").EnumerateArray().Select(v => v.GetProperty("id").GetString()).ToArray();
Need(modernizationValidationIds.Length == 5 &&
    modernizationValidationIds.Contains("m8-rs345-bounded-canonical-acceptance-and-four-refusals", StringComparer.Ordinal) &&
    rs345Proof.RootElement.GetProperty("runId").GetString() == "RS345" &&
    rs345Proof.RootElement.GetProperty("payloadCompiled").GetBoolean() &&
    rs345Proof.RootElement.GetProperty("rehearsedValidationCount").GetInt32() == 5 &&
    rs345Proof.RootElement.GetProperty("preparationValidationIds").EnumerateArray()
        .Select(v => v.GetString()).SequenceEqual(modernizationValidationIds, StringComparer.Ordinal),
    "RS345 compiled acceptance proof differs.");
var boundedAcceptance = c02Acceptance.RootElement;
Need(boundedAcceptance.GetProperty("status").GetString() == "accepted-bounded" &&
    boundedAcceptance.GetProperty("phaseClosureAuthorized").GetBoolean() &&
    !boundedAcceptance.GetProperty("productionMigrationAuthorized").GetBoolean() &&
    !boundedAcceptance.GetProperty("publicationAuthorized").GetBoolean() &&
    boundedAcceptance.GetProperty("candidate").GetProperty("binaryQualification").GetString() == "RS344",
    "Bounded modernization disposition was widened.");
var discoveryWorkPackage = rs313.RootElement;
var modernizationWorkPackage = rs346.RootElement;
foreach (var item in new[] {
    (Report: discoveryWorkPackage, Id: "RS313", Node: "M8-W01", Executor: "0.1.66"),
    (Report: modernizationWorkPackage, Id: "RS346", Node: "M8-W02", Executor: "0.1.76") })
{
    var r = item.Report;
    Need(r.GetProperty("runId").GetString() == item.Id &&
        r.GetProperty("outcome").GetString() == "succeeded" &&
        r.GetProperty("attempt").GetInt32() == 1 &&
        r.GetProperty("executor").GetProperty("toolVersion").GetString() == item.Executor &&
        r.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
        r.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
        r.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
        r.GetProperty("contribution").GetProperty("observed").GetProperty("mutationBoundaryClean").GetBoolean() &&
        r.GetProperty("progress").GetProperty("completions").EnumerateArray()
            .Any(v => v.GetProperty("id").GetString() == item.Node &&
                v.GetProperty("afterState").GetString() == "done"),
        "Missing bounded M8 work package completion: " + item.Id);
}
var w02ValidationIds = modernizationWorkPackage.GetProperty("contribution").GetProperty("observed")
    .GetProperty("validations").EnumerateArray().Select(v => v.GetProperty("id").GetString()).ToArray();
Need(w02ValidationIds.Length == 5 &&
    w02ValidationIds.Contains("m8-rs346-bounded-canonical-acceptance-and-four-refusals", StringComparer.Ordinal) &&
    rs346Proof.RootElement.GetProperty("runId").GetString() == "RS346" &&
    rs346Proof.RootElement.GetProperty("payloadCompiled").GetBoolean() &&
    rs346Proof.RootElement.GetProperty("rehearsedValidationCount").GetInt32() == 5 &&
    rs346Proof.RootElement.GetProperty("preparationValidationIds").EnumerateArray()
        .Select(v => v.GetString()).SequenceEqual(w02ValidationIds, StringComparer.Ordinal),
    "RS346 compiled work package acceptance proof differs.");
var milestoneProduct = product.GetProperty("releases").EnumerateArray()
    .Single(v => v.GetProperty("id").GetString() == "M8");
Need(milestoneProduct.GetProperty("gate").GetString() == "G-M8" &&
    milestoneProduct.GetProperty("required_chunks").EnumerateArray().Select(v => v.GetString())
        .SequenceEqual(new[] { "M8-W01-C01", "M8-W01-C02", "M8-W02-C01", "M8-W02-C02" }, StringComparer.Ordinal) &&
    milestoneProduct.GetProperty("outcome").GetString() ==
        "Qualify opt-in Math Discovery and independently assess safe .NET 11 / C# 15 modernization without consumer ownership transfer." &&
    milestoneProduct.GetProperty("status").GetString() == "done" &&
    native.RootElement.GetProperty("milestones").EnumerateArray()
        .Single(v => v.GetProperty("id").GetString() == "M9").GetProperty("state").GetString() == "done",
    "Canonical M8 outcome or deferred reconciliation boundary differs.");
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
    "selected=4|binaryConsumer=RS279|firstPass=true|packages=3|M8=done|W01=done|W02=done|W02C01=done|W02C02=done|W02C02T1=done|W02C02RED=done|W02C02GREEN=done|W02C02VERIFY=done|W02C02T2=done|W02C02T2RED=done|W02C02T2GREEN=done|W02C02T2VERIFY=done|W02T1=done|W02T2=done|W02T2RED=done|W02T2GREEN=done|W02T2VERIFY=done|W02RED=done|W02GREEN=done|W02VERIFY=done|C01=done|C02=done|C02T1=done|C02T2=done|C02T2RED=done|C02T2GREEN=done|C02T2VERIFY=done|C02RED=done|C02GREEN=done|C02VERIFY=done|T1=done|RED=done|GREEN=done|VERIFY=done|T2=done|T2RED=done|T2GREEN=done|T2VERIFY=done|brokerReady=0|PASS");
