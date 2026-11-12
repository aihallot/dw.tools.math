using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS183", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS182", 1,
    "feat: admit bounded M8 Foundation and Discovery scope into native Math roadmap",
    "0.1.66", 3, 3, 0, false);
p.ProjectPlan.RequireNodeState("M7", "done");
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("in_progress"));
const string Matrix = "docs/planning/proposals/net11-numerics-impact-matrix.json";
const string Narrative = "docs/planning/proposals/net11-numerics-impact.md";
const string Validator = "docs/planning/proposals/ValidateNet11NumericsImpact.cs";
p.Files.ReplaceFromStaged("staged/" + Matrix, Matrix);
p.Files.ReplaceFromStaged("staged/" + Narrative, Narrative);
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
p.Require.JsonValueEquals(Matrix, "/status", JsonValue.Create("research-proposal-not-migrated"));
return p.Complete();
