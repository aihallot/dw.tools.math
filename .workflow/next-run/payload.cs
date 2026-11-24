using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS328", 1,
    "chore: close Math migration RED and ready bounded GREEN",
    "0.1.72", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "done");
p.ProjectPlan.RequireNodeState("M8-W02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C02-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Require.JsonValueEquals("docs/workflow/manifest.json",
    "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/ProbeM8W02FoundationCrossSdkParityGreen.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w02-c02-t1-foundation-cross-sdk-green-rs329.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W02-C02-T1-G");
return p.Complete();
