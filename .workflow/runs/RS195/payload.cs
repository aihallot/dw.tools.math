using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Audit = "docs/discovery/qualification/ValidateM8SkillBundleConsistency.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS195", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS194", 1,
    "test: qualify Math Discovery version matrix and ten refusals",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals(".agents/skills/dw-tools-math/references/skill-version.json",
    "/brokerReadyCount", JsonValue.Create(0));
p.Files.ReplaceFromStaged("staged/" + Audit, Audit);
return p.Complete();
