using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Generator = "docs/discovery/qualification/GenerateM8SkillVersion.cs";
const string Version = ".agents/skills/dw-tools-math/references/skill-version.json";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS193", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS192", 1,
    "feat: add deterministic Math Discovery list and explain inspection",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStaged("staged/" + Generator, Generator);
p.Files.ReplaceFromStaged("staged/" + Version, Version);
return p.Complete();
