using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS318", 1,
    "test: verify copied Math SDK candidate inventory refusals",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "done");
p.ProjectPlan.RequireNodeState("M8-W02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02", "not-ready");
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
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w02-c01-t1-verification-closure-t2-red-readiness-rs319.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W02-C01-T1-V");
p.ProjectPlan.ConvergeNodeToDone("M8-W02-C01-T1");
p.ProjectPlan.TransitionNode("M8-W02-C01-T2", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M8-W02-C01-T2-R", "not-ready", "ready");
return p.Complete();
