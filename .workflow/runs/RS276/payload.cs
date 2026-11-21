using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS276", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS275", 1,
    "docs: reconcile Assessa bounded Statistics request with qualified delivery",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-w01-c01-evidence-lifecycle-assessment.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM9EvidenceLifecycleAssessment.cs"));
return p.Complete();
