using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Validator = "docs/discovery/qualification/ValidateM8MathSkill.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS189", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS188", 1,
    "feat: generate bounded Math Discovery skill from qualified manifest",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
return p.Complete();
