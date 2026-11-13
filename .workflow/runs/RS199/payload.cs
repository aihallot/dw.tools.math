using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Generator = "docs/discovery/qualification/GenerateM8SkillReference.cs";
const string Index = ".agents/skills/dw-tools-math/references/index.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS199", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS198", 1,
    "test: qualify source-free packaged Math Discovery invocations",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStaged("staged/" + Generator, Generator);
p.Files.ReplaceFromStaged("staged/" + Index, Index);
return p.Complete();
