using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var task = state.RootElement.GetProperty("activeTask").GetString();
var leaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (task is not (null or "M8-W01-C01-T2") ||
    leaf is not (null or "M8-W01-C01-T2-R"))
    throw new InvalidDataException("RS293: Unexpected active task/leaf.");
p.Workflow.RequireCurrentRun("RS293", "M8", "M8-W01", "M8-W01-C01", task, leaf);
p.Workflow.RequirePriorSuccess("RS292", 1,
    "chore: complete Discovery protocol T1 and ready selected CLR RED",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-G", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t2-red-reflection-falsification-rs293.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C01-T2-R");
return p.Complete();
