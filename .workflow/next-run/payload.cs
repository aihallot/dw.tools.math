using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Probe = "docs/engineering/ProbeM8Net11DistributionLockedRestore.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS253", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS252", 1,
    "docs: reconcile Math net11 distribution and binary evidence",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("docs/engineering/m8-net11-csharp15-migration-evidence.md",
    "RS250-RS251");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Probe));
return p.Complete();
