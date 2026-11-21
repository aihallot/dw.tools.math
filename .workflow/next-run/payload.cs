using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS281", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS280", 1,
    "docs: reconcile M8 Discovery positive binary evidence and acceptance frontier",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ExportM8PortableDiscoverySkill.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-portable-discovery-bundle.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/README.md"));
return p.Complete();
