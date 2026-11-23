using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS294",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T2", "M8-W01-C01-T2-R");
p.Workflow.RequirePriorSuccess("RS293", 1,
    "test: probe selected CLR RED reflection against published binaries",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-R", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-G", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t2-real-clr-admission-rs294.md"));
return p.Complete();
