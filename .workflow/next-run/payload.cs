using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS297", 1,
    "chore: complete selected CLR GREEN and ready Verification",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-G", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ProbeM8SelectedClrVerificationConsumer.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w01-c01-t2-verification-isolated-real-clr-rs298.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C01-T2-V");
return p.Complete();
