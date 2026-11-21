using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS277", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS276", 1,
    "docs: assess M9 bounded Statistics lifecycle evidence without retroactive progress",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM9WslNet11PublishedBoundedStatistics.cs"));
return p.Complete();
