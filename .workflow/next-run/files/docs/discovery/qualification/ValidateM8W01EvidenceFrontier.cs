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
        (phase.GetProperty("id").GetString() == "M8-W01-C01" ? "in-progress" : "not-ready"),
        "Native Discovery phase readiness differs.");
    foreach (var task in phase.GetProperty("tasks").EnumerateArray())
    {
        Need(task.GetProperty("state").GetString() ==
            (task.GetProperty("id").GetString() == "M8-W01-C01-T1" ? "in-progress" : "not-ready"),
            "Native protocol task readiness differs.");
        foreach (var sub in task.GetProperty("subtasks").EnumerateArray())
            Need(sub.GetProperty("state").GetString() ==
                (sub.GetProperty("id").GetString() == "M8-W01-C01-T1-R" ? "in-progress" : "not-ready"),
                "Native protocol RED readiness differs.");
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
    "selected=4|binaryConsumer=RS279|firstPass=true|packages=3|M8=in-progress|W01=in-progress|C01=in-progress|T1=in-progress|RED=in-progress|brokerReady=0|PASS");
