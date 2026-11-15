using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11FoundationSource.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS218", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS217", 1,
    "test: qualify thirteen-package Math dependency graph",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
