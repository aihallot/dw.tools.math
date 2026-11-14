using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Reconciliation = "docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS208", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS207", 1,
    "test: qualify selected Discovery example links and refusals",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Reconciliation));
return p.Complete();
