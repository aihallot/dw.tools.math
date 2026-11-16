using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11DistributionGraph.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS232", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS231", 1,
    "docs: assess Math net11 migration evidence and remaining gates",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("docs/engineering/m8-net11-csharp15-migration-evidence.md",
    "Distribution");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
