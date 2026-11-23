using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
// The DWF run selection is verified by the executor. Avoid manually traversing
// canonical workflow state, and use the typed lineage contract for RS296.
p.Workflow.RequirePriorSuccess("RS296", 1,
    "test: qualify independent selected CLR GREEN admission",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "done");
// The staged target-reentrant verification transition may already be ready during rehearsal.
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
