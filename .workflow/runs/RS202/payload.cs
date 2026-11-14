using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Validator = "docs/discovery/qualification/ValidateM8BundledManifest.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS202", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS201", 1,
    "feat: bundle qualified Math Discovery manifest byte-for-byte",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-discovery-manifest.json",
    "/owner", JsonValue.Create("math"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Validator));
return p.Complete();
