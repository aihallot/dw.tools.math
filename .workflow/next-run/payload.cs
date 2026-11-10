using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Publisher = "docs/distribution/electrochemical-publisher-qualified.json";
const string Candidate = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS176", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS175", 1,
    "test: qualify immutable electrochemical publisher on original candidate bytes",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.26"));
p.Require.JsonValueEquals(Publisher, "/status",
    JsonValue.Create("rs175-electrochemical-original-archive-publisher-qualified-unpublished"));
p.Require.JsonValueEquals(Publisher, "/publisher/controlsPassed", JsonValue.Create(7));
p.Require.JsonValueEquals(Publisher, "/package_published", JsonValue.Create(false));
p.Require.JsonValueEquals(Candidate, "/candidate/candidateArchiveSha256",
    JsonValue.Create("766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d"));

string[] paths =
[
    "docs/distribution/electrochemical-local-publication.md",
    "tools/dw.tools.math.nuget-electrochemical-release/publication/dw.tools.math.nuget-electrochemical-publication.csproj",
    "tools/dw.tools.math.nuget-electrochemical-release/publication/Program.cs"
];
// Accept only missing or exact staged files, including interrupted installation.
foreach (var path in paths)
{
    var destination = Path.Combine(p.RepositoryRoot, path);
    if (File.Exists(destination))
    {
        var staged = Path.Combine(p.RunRoot, "staged", path);
        if (!File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(staged)))
            throw new InvalidDataException("RS176 destination differs from its staged target: " + path);
    }
    else
    {
        p.Require.FileMissing(path);
    }
}
foreach (var path in paths)
    p.Files.ReplaceFromStaged("staged/" + path, path);
return p.Complete();
