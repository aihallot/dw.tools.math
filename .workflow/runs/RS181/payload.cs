using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS181", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS180", 1,
    "test: qualify installed DWF milestone authoring availability for Math M8",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/evidence/RS180-dwf-native-topology-capability.json",
    "/supportedForNewMathMilestones", JsonValue.Create(true));
p.Require.JsonValueEquals("docs/planning/proposals/math-next-roadmap-m8-m12.json",
    "/new_milestones_not_yet_inserted", JsonValue.Create(true));
const string Json = "docs/planning/proposals/m8-admission-contract.json";
const string Markdown = "docs/planning/proposals/m8-admission-contract.md";
const string Validator = "docs/planning/proposals/ValidateM8AdmissionContract.cs";
p.Files.ReplaceFromStaged("staged/" + Json, Json);
p.Files.ReplaceFromStaged("staged/" + Markdown, Markdown);
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
p.Require.JsonValueEquals(Json, "/status",
    JsonValue.Create("qualified-design-pending-canonical-admission"));
p.Require.JsonValueEquals(Json, "/admission/expectedAddedNodes", JsonValue.Create(39));
return p.Complete();
