using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM9Net11BoundedCandidateConsumer.cs";
const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS270", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS269", 1,
    "test: qualify unpublished bounded Statistics NuGet candidate",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Fixture));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
