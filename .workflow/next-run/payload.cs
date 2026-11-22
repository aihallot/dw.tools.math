using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS285", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS284", 1,
    "chore: admit M8 milestone readiness for forward-only development",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M7", "done");
p.ProjectPlan.RequireNodeState("M8", "ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-red-readiness-rs285.md"));
p.ProjectPlan.TransitionNode("M8-W01", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M8-W01-C01", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M8-W01-C01-T1", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M8-W01-C01-T1-R", "not-ready", "ready");
return p.Complete();
