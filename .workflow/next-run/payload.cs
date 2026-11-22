using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS284", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS283", 1,
    "feat: attest portable Math Discovery exports with SHA-256 receipt",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M7", "done");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-root-readiness-rs284.md"));
p.ProjectPlan.TransitionNode("M8", "not-ready", "ready");
return p.Complete();
