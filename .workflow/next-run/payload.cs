using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Graph = "docs/engineering/InspectM8Net11PackageGraph.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS220", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS219", 1,
    "test: qualify Math quantities source under isolated net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains(Graph, "Expected sixteen observed direct dependency edges.");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Graph));
return p.Complete();
