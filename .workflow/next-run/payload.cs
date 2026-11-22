using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS287",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T1", "M8-W01-C01-T1-R");
p.Workflow.RequirePriorSuccess("RS286", 1,
    "test: begin Math Discovery protocol RED binding consistency",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-R", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-G", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-strict-binding-metadata-rs287.md"));
return p.Complete();
