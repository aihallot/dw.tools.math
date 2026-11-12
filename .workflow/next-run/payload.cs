using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Generator = "docs/discovery/qualification/GenerateM8MathSkill.cs";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS188", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS187", 1,
    "test: qualify generated Math Discovery manifest and eleven refusals",
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
p.Files.ReplaceFromStaged("staged/" + Generator, Generator);
p.Files.ReplaceFromStaged("staged/" + Skill, Skill);
return p.Complete();
