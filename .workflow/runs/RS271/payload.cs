using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM9StatisticsPrivateReadiness.cs";
const string Decision = "docs/distribution/m9-statistics-private-publication-readiness.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS271", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS270", 1,
    "test: qualify bounded Statistics candidate net11 binary consumer",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Decision));
return p.Complete();
