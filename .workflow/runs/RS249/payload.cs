using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8WslNet11PublishedStandardBinary.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS249", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS248", 1,
    "test: qualify published Math Standard binaries on net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f10-standard-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261103.1001"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
