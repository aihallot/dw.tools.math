using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Triage = "docs/engineering/m8-net11-numeric-api-triage.md";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS235", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS234", 1,
    "test: inventory Math numerical API signals for net11",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W02-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.123"));
p.Require.TextContains("docs/engineering/InspectM8Net11NumericApiSignals.cs",
    "MATH-RS234-API-AUDIT");
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile(Triage));
p.Inspect.Search("Math.Round(", ["src/projects"], "numeric-math-round.md",
    maxMatches: 2000, purpose: PayloadArtifactPurpose.Evidence);
p.Inspect.Search("BigInteger", ["src/projects"], "numeric-biginteger.md",
    maxMatches: 2000, purpose: PayloadArtifactPurpose.Evidence);
p.Inspect.Search("Complex", ["src/projects"], "numeric-complex.md",
    maxMatches: 2000, purpose: PayloadArtifactPurpose.Evidence);
p.Inspect.Search("Convert.To", ["src/projects"], "numeric-convert.md",
    maxMatches: 2000, purpose: PayloadArtifactPurpose.Evidence);
p.Inspect.Search("checked(", ["src/projects"], "numeric-checked.md",
    maxMatches: 2000, purpose: PayloadArtifactPurpose.Evidence);
return p.Complete();
