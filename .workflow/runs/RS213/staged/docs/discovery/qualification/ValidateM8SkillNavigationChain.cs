#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string Bundle = ".agents/skills/dw-tools-math";
string Read(string relative) => File.ReadAllText(Path.Combine(Bundle, relative))
    .Replace("\r\n", "\n", StringComparison.Ordinal);
var entry = Read("SKILL.md");
var index = Read("references/index.md");
var examples = Read("references/examples.md");
var selected = Read("references/selected-capabilities.md");
var versions = Read("references/skill-version.json");
var manifest = Read("references/manifest.json");
Validate(entry, index, examples);
var refusals = 0;
Reject("entrypoint-link", entry.Replace("(references/index.md)", "(references/missing.md)", StringComparison.Ordinal), index, examples);
Reject("index-example-link", entry, index.Replace("(examples.md)", "(missing.md)", StringComparison.Ordinal), examples);
Reject("example-source-link", entry, index,
    examples.Replace("(../../../../tests/consumers/m8-discovery-clr-seed/Program.cs)", "(missing.cs)", StringComparison.Ordinal));
Reject("broker-promotion", entry.Replace("Do not expose any capability as BrokerReady.", "Expose all capabilities as BrokerReady.", StringComparison.Ordinal), index, examples);
Require(refusals == 4, "Four deliberate navigation refusals required.");
Console.WriteLine("MATH-RS213-SKILL-NAVIGATION|entry=1|references=4|selected=4|refusals=4|brokerReady=0|PASS");

void Reject(string id, string candidateEntry, string candidateIndex, string candidateExamples)
{
    Require(candidateEntry != entry || candidateIndex != index || candidateExamples != examples,
        "Mutation did not change input: " + id);
    try { Validate(candidateEntry, candidateIndex, candidateExamples); }
    catch (InvalidDataException)
    {
        refusals++;
        Console.WriteLine("REFUSAL " + id + " PASS");
        return;
    }
    throw new InvalidDataException("Admitted navigation drift: " + id);
}
void Validate(string candidateEntry, string candidateIndex, string candidateExamples)
{
    const string seed = "../../../../tests/consumers/m8-discovery-clr-seed/Program.cs";
    Require(candidateEntry.Contains("[the selected capability index](references/index.md)", StringComparison.Ordinal),
        "Skill entrypoint must link selected index.");
    foreach (var reference in new[] { "references/index.md", "references/examples.md",
        "references/selected-capabilities.md", "references/skill-version.json", "references/manifest.json" })
        Require(File.Exists(Path.Combine(Bundle, reference)), "Missing bundled reference: " + reference);
    foreach (var link in new[] { "selected-capabilities.md", "skill-version.json", "examples.md" })
        Require(candidateIndex.Contains("](" + link + ")", StringComparison.Ordinal),
            "Index missing reference: " + link);
    Require(candidateExamples.Contains("](" + seed + ")", StringComparison.Ordinal) &&
        File.Exists(Path.GetFullPath(Path.Combine(Bundle, "references", seed))),
        "Example source link unresolved.");
    Require(candidateEntry.Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal) &&
        candidateIndex.Contains("none of these capabilities is `BrokerReady`.", StringComparison.Ordinal) &&
        candidateExamples.Contains("No item is `BrokerReady`", StringComparison.Ordinal),
        "No broker promotion or authorization granted.");
    using var doc = JsonDocument.Parse(manifest);
    using var versionsDoc = JsonDocument.Parse(versions);
    var root = doc.RootElement;
    var caps = root.GetProperty("capabilities").EnumerateArray().ToArray();
    Require(caps.Length == 4 && root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        versionsDoc.RootElement.GetProperty("brokerReadyCount").GetInt32() == 0,
        "Selected four and zero BrokerReady.");
    foreach (var cap in caps)
    {
        var id = cap.GetProperty("capabilityId").GetString() ?? "";
        Require(cap.GetProperty("exposure").GetString() == "AgentCallable" &&
            cap.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only" &&
            candidateEntry.Contains("### `" + id + "`", StringComparison.Ordinal) &&
            candidateIndex.Contains("`" + id + "`", StringComparison.Ordinal) &&
            candidateExamples.Contains("| `" + id + "` |", StringComparison.Ordinal) &&
            selected.Contains("## `" + id + "`", StringComparison.Ordinal),
            "Selected navigation ID or authorization: " + id);
    }
}
static void Require(bool good, string message)
{
    if (!good) throw new InvalidDataException("RS213: " + message);
}
