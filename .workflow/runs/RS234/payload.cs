using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Audit = "docs/engineering/InspectM8Net11NumericApiSignals.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS234", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS233", 1,
    "test: qualify thirteen-project Math source graph under net11 C#15",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("docs/engineering/m8-net11-csharp15-migration-evidence.md",
    "Gates explicitly not satisfied");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Audit));
return p.Complete();
