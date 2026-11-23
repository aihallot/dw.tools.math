using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var workflowState = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeLeaf = workflowState.RootElement.GetProperty("activeSubtask").GetString();
if (activeLeaf is not ("M8-W01-C01-T1-G" or null))
    throw new InvalidDataException("RS290: Unexpected active subtask.");
p.Workflow.RequireCurrentRun("RS290",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T1", activeLeaf);
p.Workflow.RequirePriorSuccess("RS289", 1,
    "test: qualify Discovery GREEN pre-attestation refusals",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-portable-discovery-bundle.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-green-closure-verification-readiness-rs290.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T1-G");
p.ProjectPlan.TransitionNode("M8-W01-C01-T1-V", "not-ready", "ready");
return p.Complete();
