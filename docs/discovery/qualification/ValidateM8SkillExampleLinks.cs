#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string IndexPath = ".agents/skills/dw-tools-math/references/index.md";
const string ExamplesPath = ".agents/skills/dw-tools-math/references/examples.md";
const string SeedPath = "tests/consumers/m8-discovery-clr-seed/Program.cs";
using var document = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var root = document.RootElement;
var selected = root.GetProperty("capabilities").EnumerateArray().ToArray();
var index = File.ReadAllText(IndexPath).Replace("\r\n", "\n", StringComparison.Ordinal);
var examples = File.ReadAllText(ExamplesPath).Replace("\r\n", "\n", StringComparison.Ordinal);
var seed = File.ReadAllText(SeedPath);
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    root.GetProperty("owner").GetString() == "math" &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    selected.Length == 4, "Canonical selected manifest boundary.");
Validate(index, examples);
var refused = 0;
Reject("missing-navigation-link", index.Replace("(examples.md)", "(missing.md)", StringComparison.Ordinal), examples);
Reject("missing-example-id", index, examples.Replace("`math.replay.try-evaluate`", "`math.replay.other`", StringComparison.Ordinal));
Reject("promoted-authorization", index, examples.Replace("not invocation authorization or broker routing", "broker authorization granted", StringComparison.Ordinal));
Reject("lost-source-link", index, examples.Replace("(../../../../tests/consumers/m8-discovery-clr-seed/Program.cs)", "(missing.cs)", StringComparison.Ordinal));
Reject("lost-source-scope", index, examples.Replace("not** evidence that a fresh external consumer installed", "is** evidence that a fresh external consumer installed", StringComparison.Ordinal));
Require(refused == 5, "Exactly five independent example-link refusals.");
Console.WriteLine("MATH-RS207-EXAMPLE-LINKS|selected=4|positive=1|negative=5|brokerReady=0|PASS");

void Reject(string label, string candidateIndex, string candidateExamples)
{
    Require(candidateIndex != index || candidateExamples != examples, "Mutation absent: " + label);
    try { Validate(candidateIndex, candidateExamples); }
    catch (InvalidDataException)
    {
        refused++;
        Console.WriteLine("REFUSAL " + label + " PASS");
        return;
    }
    throw new InvalidDataException("RS207 accepted altered " + label);
}

void Validate(string candidateIndex, string candidateExamples)
{
    const string sourceLink = "../../../../tests/consumers/m8-discovery-clr-seed/Program.cs";
    Require(candidateIndex.Contains("[Runnable source consumer examples and limitations](examples.md)",
        StringComparison.Ordinal), "Example navigation link.");
    Require(File.Exists(ExamplesPath) &&
        candidateExamples.Contains("](" + sourceLink + ")", StringComparison.Ordinal) &&
        File.Exists(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ExamplesPath)!, sourceLink))),
        "Resolvable source consumer link.");
    Require(candidateExamples.Contains("not invocation authorization or broker routing",
        StringComparison.Ordinal) &&
        candidateExamples.Contains("not** evidence that a fresh external consumer installed",
        StringComparison.Ordinal) &&
        candidateExamples.Contains("No item is `BrokerReady`", StringComparison.Ordinal) &&
        candidateExamples.Contains("`runtime-only`", StringComparison.Ordinal),
        "Examples must not grant permissions or overstate installation evidence.");
    foreach (var cap in selected)
    {
        var id = cap.GetProperty("capabilityId").GetString()
            ?? throw new InvalidDataException("Missing capability ID.");
        Require(cap.GetProperty("exposure").GetString() == "AgentCallable" &&
            cap.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only" &&
            candidateExamples.Contains("| `" + id + "` |", StringComparison.Ordinal),
            "Selected example identity or authorization: " + id);
    }
    Require(seed.Contains("ExactQuantityPipeline.Evaluate(boundedSteps)", StringComparison.Ordinal) &&
        seed.Contains("ExactReplayRunner.TryRun(boundedSteps", StringComparison.Ordinal) &&
        seed.Contains("IrCanonicalJsonCodec.Encode(exact)", StringComparison.Ordinal) &&
        seed.Contains("IrCanonicalHashes.SemanticStructuralSha256(exact)", StringComparison.Ordinal),
        "Four selected source-project invocations.");
}

static void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS207: " + reason);
}
