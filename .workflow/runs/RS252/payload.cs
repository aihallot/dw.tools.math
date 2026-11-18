using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Audit = "docs/engineering/m8-net11-csharp15-migration-evidence.md";
const string Validator = "docs/engineering/ValidateM8Net11EvidenceReconciliation.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS252", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS251", 1,
    "test: qualify Math distribution project graph on Linux net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Audit));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Validator));
return p.Complete();
