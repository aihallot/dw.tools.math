using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Audit = "docs/engineering/m8-net11-csharp15-migration-evidence.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS231", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS230", 1,
    "test: qualify Math scientific sources under isolated net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("global.json", "10.0.401");
p.Require.TextContains("Directory.Build.props", "net10.0");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Audit));
p.Require.TextContains(Audit, "Gates explicitly not satisfied");
return p.Complete();
