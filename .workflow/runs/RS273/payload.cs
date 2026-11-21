using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS273", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS272", 1,
    "test: qualify guarded private bounded Statistics publisher",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM9PublishedStatisticsReadback.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/distribution/m9-statistics-physical-private-publication-readback.md"));
return p.Complete();
