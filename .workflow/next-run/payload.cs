using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeTask = state.RootElement.GetProperty("activeTask").GetString();
var activeLeaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (activeTask is not ("M8-W01-C01-T1" or null) ||
    activeLeaf is not ("M8-W01-C01-T1-V" or null))
    throw new InvalidDataException("RS292: Unexpected active T1 or Verification subtask.");
p.Workflow.RequireCurrentRun("RS292",
    "M8", "M8-W01", "M8-W01-C01", activeTask, activeLeaf);
p.Workflow.RequirePriorSuccess("RS291", 1,
    "test: verify portable Discovery public export from isolated consumer",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-portable-discovery-bundle.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-verification-closure-t2-red-readiness-rs292.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T1-V");
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T1");
p.ProjectPlan.TransitionNode("M8-W01-C01-T2", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M8-W01-C01-T2-R", "not-ready", "ready");
return p.Complete();
