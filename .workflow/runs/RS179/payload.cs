using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Proposal = "docs/planning/proposals/math-next-roadmap-m8-m12.json";
const string Narrative = "docs/planning/proposals/math-next-roadmap-m8-m12.md";
const string Validator = "docs/planning/proposals/ValidateMathNextRoadmap.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS179", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS178", 1,
    "docs: prepare AURA electrochemical reception dossier from verified Math delivery",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.26"));
foreach (var path in new[] { Proposal, Narrative, Validator })
{
    p.Files.ReplaceFromStaged("staged/" + path, path);
}
p.Require.JsonValueEquals(Proposal, "/id", JsonValue.Create("MATH-NEXT-M8-M12"));
p.Require.JsonValueEquals(Proposal, "/new_milestones_not_yet_inserted", JsonValue.Create(true));
return p.Complete();
