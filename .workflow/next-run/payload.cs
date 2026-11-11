using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Md = "docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md";
const string Json = "docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json";
const string Validator = "docs/coordination/requests/ValidateMathXr006AuraElectrochemical.cs";
const string Readme = "docs/coordination/requests/README.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS178", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS177", 1,
    "test: verify published electrochemical archives and three loaded DLLs",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/distribution/electrochemical-published-readback.json", "/status",
    JsonValue.Create("rs177-electrochemical-published-readback-verified"));
p.Require.JsonValueEquals("docs/distribution/electrochemical-published-readback.json", "/auraIntegrationVerified",
    JsonValue.Create(false));
p.Require.JsonValueEquals("docs/distribution/electrochemical-published-readback.json", "/auraHistoricalReuseRightsEstablished",
    JsonValue.Create(false));
foreach (var path in new[] { Md, Json, Validator }) p.Require.FileMissing(path);
p.Files.ReplaceFromStaged("staged/" + Md, Md);
p.Files.ReplaceFromStaged("staged/" + Json, Json);
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
p.Files.ConvergeFromStaged("staged/" + Readme, Readme, "898ebd7a2f9c6db844711d9e7c7b2770ae42feb8cf1b7e45f07160ce0a0af844");
p.Require.JsonValueEquals(Json, "/status", JsonValue.Create("draft"));
p.Require.JsonValueEquals(Json, "/transmission", JsonValue.Create("none"));
return p.Complete();
