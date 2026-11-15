using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11StandardSource.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS223", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS222", 1,
    "test: qualify quantities expression sources under isolated net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "../dw.quantities.expression/dw.quantities.expression.csproj");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
