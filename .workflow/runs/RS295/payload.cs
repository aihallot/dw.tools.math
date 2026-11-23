using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeLeaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (activeLeaf is not ("M8-W01-C01-T2-R" or null))
    throw new InvalidDataException("RS295: Unexpected active RED leaf.");
p.Workflow.RequireCurrentRun("RS295",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T2", activeLeaf);
p.Workflow.RequirePriorSuccess("RS294", 1,
    "feat: admit selected Discovery bindings against real published CLR",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T2-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t2-red-closure-green-readiness-rs295.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T2-R");
p.ProjectPlan.TransitionNode("M8-W01-C01-T2-G", "not-ready", "ready");
return p.Complete();
