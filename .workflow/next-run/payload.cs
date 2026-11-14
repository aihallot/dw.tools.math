using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Readme = "docs/discovery/README.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS210", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS209", 1,
    "test: verify pinned Discovery protocol fixture receipt parity",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Readme));
return p.Complete();
