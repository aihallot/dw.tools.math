using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net10Net11ExactParserParity.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS238", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS237", 1,
    "test: qualify Math quantity dimension parity on net10 and net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.quantities/ExactRational.cs",
    "public static ExactRational ParseInvariant");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
