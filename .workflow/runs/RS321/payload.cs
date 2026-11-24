using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS320", 1,
    "test: falsify Math SDK package graph risks in isolated copies",
    "0.1.72", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "done");
p.ProjectPlan.RequireNodeState("M8-W02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Require.JsonValueEquals("docs/workflow/manifest.json",
    "/guidanceVersion", JsonValue.Create("0.26"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w02-c01-t2-red-closure-green-readiness-rs321.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W02-C01-T2-R");
p.ProjectPlan.TransitionNode("M8-W02-C01-T2-G", "not-ready", "ready");
return p.Complete();
