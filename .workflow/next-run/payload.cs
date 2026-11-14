using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Inspector = "docs/engineering/InspectM8Net11PackageGraph.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS217", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS216", 1,
    "test: qualify isolated .NET 11 C# 15 compiler probe",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Inspector));
return p.Complete();
