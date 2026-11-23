using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS304", 1,
    "chore: complete Discovery skill GREEN and ready Verification",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T1-G", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ProbeM8C02SkillVerificationConsumer.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w01-c02-t1-verification-copied-skill-rs305.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C02-T1-V");
return p.Complete();
