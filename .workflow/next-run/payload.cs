using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11CodecsSource.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS225", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS224", 1,
    "test: qualify Math composition sources under isolated net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",
    "../dw.tools.math.ir/dw.tools.math.ir.csproj");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
