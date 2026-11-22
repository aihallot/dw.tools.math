using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeLeaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (activeLeaf is not ("M8-W01-C01-T1-R" or null))
    throw new InvalidDataException("RS288 unexpected active subtask.");
p.Workflow.RequireCurrentRun("RS288",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T1", activeLeaf);
p.Workflow.RequirePriorSuccess("RS287", 1,
    "feat: validate strict Math Discovery binding metadata alignment",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-portable-discovery-bundle.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-red-closure-green-readiness-rs288.md"));
p.ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T1-R");
p.ProjectPlan.TransitionNode("M8-W01-C01-T1-G", "not-ready", "ready");
return p.Complete();
