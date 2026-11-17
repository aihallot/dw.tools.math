using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8WslNet11ExactParserConsumer.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS243", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS242", 1,
    "test: qualify Math temperature Linux WSL net11 consumer",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.quantities/ExactRational.cs",
    "public static ExactRational ParseInvariant");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
