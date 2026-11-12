using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Generator = "docs/discovery/qualification/GenerateM8SkillReference.cs";
const string Reference = ".agents/skills/dw-tools-math/references/selected-capabilities.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS190", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS189", 1,
    "test: qualify Math Discovery skill and seven drift refusals",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStaged("staged/" + Generator, Generator);
p.Files.ReplaceFromStaged("staged/" + Reference, Reference);
return p.Complete();
