using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11PublishedQuantitiesBinary.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS244", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS243", 1,
    "test: qualify Math exact parser Linux WSL net11 consumer",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f4-quantities-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261102.1001"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
