using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
using var state = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(p.RepositoryRoot, ".aura", "workflow", "state.json")));
var activeLeaf = state.RootElement.GetProperty("activeSubtask").GetString();
if (activeLeaf is not (null or "M8-W01-C01-T1-V"))
    throw new InvalidDataException("RS291: Unexpected active subtask.");
p.Workflow.RequireCurrentRun("RS291",
    "M8", "M8-W01", "M8-W01-C01", "M8-W01-C01-T1", activeLeaf);
p.Workflow.RequirePriorSuccess("RS290", 1,
    "chore: complete Discovery protocol GREEN and ready Verification",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "in-progress");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ProbeM8VerificationPortableConsumer.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-w01-c01-t1-verification-independent-consumer-rs291.md"));
p.ProjectPlan.ActivateReadyContinuation("M8-W01-C01-T1-V");
return p.Complete();
