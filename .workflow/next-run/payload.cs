using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Validator = "docs/discovery/qualification/ValidateM8PortableFixtureReceipt.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS209", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS208", 1,
    "docs: reconcile M8 Discovery acceptance evidence and gaps",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Validator));
return p.Complete();
