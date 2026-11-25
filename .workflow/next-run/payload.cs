using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS336", 1,
    "chore: close Statistics package GREEN and ready Verification",
    "0.1.75", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "done");
p.ProjectPlan.RequireNodeState("M8-W02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T2-R", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T2-G", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T1", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Require.JsonValueEquals("docs/workflow/manifest.json",
    "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/VerifyM8W02StatisticsPackageEvidence.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w02-c02-t2-statistics-verification-activation-rs337.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W02-C02-T2-V");
return p.Complete();
