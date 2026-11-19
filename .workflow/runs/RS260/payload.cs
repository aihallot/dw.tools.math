using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11PublishedCompositionBinary.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS260", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS259", 1,
    "test: qualify published Math Codecs binaries on Linux net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f34-composition-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261107.1001"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
