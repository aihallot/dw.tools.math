using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS315", 1,
    "chore: complete Math SDK audit RED and ready GREEN",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "done");
p.ProjectPlan.RequireNodeState("M8-W02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/ValidateM8W02OfficialDeltaMatrix.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w02-c01-t1-green-official-delta-matrix-rs316.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W02-C01-T1-G");
return p.Complete();
