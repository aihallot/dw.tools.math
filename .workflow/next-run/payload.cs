using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeLeaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (activeLeaf is not ("M8-W01-C01-T2-G" or null))
    throw new InvalidDataException("RS297: Unexpected active GREEN leaf.");
p.Workflow.RequireCurrentRun("RS297",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T2", activeLeaf);
using (var prior = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura/workflow/reports/RS296/attempt-001.json"))))
{
    var report = prior.RootElement;
    var failures = report.GetProperty("priorInvocationFailures");
    if (report.GetProperty("runId").GetString() != "RS296" ||
        report.GetProperty("outcome").GetString() != "succeeded" ||
        report.GetProperty("attempt").GetInt32() != 1 ||
        failures.GetProperty("count").GetInt32() != 1 ||
        failures.GetProperty("preparationFailureCount").GetInt32() != 1 ||
        failures.GetProperty("allocatedAttemptFailureCount").GetInt32() != 0 ||
        failures.GetProperty("firstPassSucceeded").GetBoolean())
        throw new InvalidDataException("RS297: RS296 delivery or history changed.");
}
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w01-c01-t2-green-closure-verification-readiness-rs297.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T2-G");
p.ProjectPlan.TransitionNode("M8-W01-C01-T2-V", "not-ready", "ready");
return p.Complete();
