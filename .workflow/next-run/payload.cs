using Dw.Tools.Workflow.Payloads;

const string Intake = "docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS266", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS265", 1,
    "test: qualify published Math Scientific binary on Linux net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M3-W02", "done");
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Intake));
return p.Complete();
