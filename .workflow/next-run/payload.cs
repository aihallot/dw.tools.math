using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS300", 1,
    "chore: complete Discovery C01 and ready skill generation",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T1-G", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ProbeM8C02SkillGeneratorRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/planning/decisions/m8-w01-c02-t1-red-isolated-skill-drift-rs301.md"));
// A continuation requires the descendants to be effectively ready.
// Preserve target re-entry: rehearsal may already have activated these nodes.
void AdmitReadyOrActive(string nodeId)
{
    try
    {
        p.ProjectPlan.TransitionNode(nodeId, "not-ready", "ready");
    }
    catch (InvalidOperationException)
    {
        // Refuse any unexpected state through the typed project-plan authority.
        p.ProjectPlan.RequireNodeState(nodeId, "in-progress");
    }
}
AdmitReadyOrActive("M8-W01-C02-T1");
AdmitReadyOrActive("M8-W01-C02-T1-R");
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C02-T1-R");
return p.Complete();
