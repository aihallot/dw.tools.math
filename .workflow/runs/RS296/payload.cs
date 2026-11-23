using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS296",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T2", null);
p.Workflow.RequirePriorSuccess("RS295", 1,
    "chore: close selected CLR RED and ready GREEN",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "done");
// Activation is target-reentrant: rehearsal can observe GREEN already in-progress.
// ProjectPlan.ActivateReadyContinuation verifies the exact ready/active target.

p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ProbeM8SelectedClrGreenConsumer.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w01-c01-t2-green-independent-admission-rs296.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C01-T2-G");
return p.Complete();
