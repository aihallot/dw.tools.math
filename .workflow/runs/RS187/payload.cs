using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Validator = "docs/discovery/qualification/ValidateM8GeneratedManifest.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS187", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS186", 1,
    "feat: generate selected Math Discovery schema-2 manifest without broker claims",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-selected-clr-bindings.json",
    "/brokerReadyCount", JsonValue.Create(0));
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
return p.Complete();
