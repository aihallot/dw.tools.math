using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS268", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS267", 1,
    "docs: admit Assessa bounded integer Math milestone",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "src/projects/dw.tools.math.statistics/SplitMix64Random.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "tests/projects/dw.tools.math.statistics.tests/M9W01C01BoundedSamplingTests.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(
    "docs/distribution/bounded-integer-sampling.md"));
return p.Complete();
