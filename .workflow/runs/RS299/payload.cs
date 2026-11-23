using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS298", 1,
    "test: verify selected CLR admission from isolated portable export",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
// Parent T2 can already be done in the target-reentry rehearsal.
// ConvergeNodeToDone checks its own admissible baseline/completed states.
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-G", "done");
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
    "docs/planning/decisions/m8-w01-c01-t2-verification-task-closure-rs299.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T2-V");
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T2");
return p.Complete();
