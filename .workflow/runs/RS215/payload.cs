using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Inspector = "docs/engineering/InspectM8Net11SourceBaseline.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS215", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS214", 1,
    "test: inspect thirteen Math packable project upgrade baseline",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Inspector));
return p.Complete();
