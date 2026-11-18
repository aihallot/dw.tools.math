using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8WslNet11DistributionProjectGraph.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS251", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS250", 1,
    "test: qualify Math distribution project graph on net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj",
    "dw.tools.math.composition.csproj");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
