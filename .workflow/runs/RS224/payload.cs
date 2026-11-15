using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11CompositionSource.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS224", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS223", 1,
    "test: qualify standard units source under isolated net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",
    "../dw.tools.math.ir/dw.tools.math.ir.csproj");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
