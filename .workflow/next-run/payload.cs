using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS267", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS266", 1,
    "docs: record Assessa unbiased bounded integer Math request",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/backlog.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(".aura/workflow/plan/project.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
return p.Complete();
