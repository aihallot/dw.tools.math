using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net10Net11ExactRationalParity.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS236", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS235", 1,
    "docs: capture Math net11 numeric API triage evidence",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("src/projects/dw.quantities/ExactRational.cs",
    "BigInteger.GreatestCommonDivisor");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
