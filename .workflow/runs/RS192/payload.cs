using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Explorer = "docs/discovery/qualification/DescribeM8Discovery.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS192", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS191", 1,
    "test: qualify Math Discovery reference and eight drift refusals",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStaged("staged/" + Explorer, Explorer);
return p.Complete();
