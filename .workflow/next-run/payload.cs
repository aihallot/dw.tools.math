using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Consumer = "docs/discovery/qualification/VerifyM8PrivateBinaryBindings.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS197", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS196", 1,
    "test: audit Math Discovery source and published package boundary",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f34-composition-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
p.Files.ReplaceFromStaged("staged/" + Consumer, Consumer);
return p.Complete();
