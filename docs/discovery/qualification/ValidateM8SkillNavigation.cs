#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string IndexPath = ".agents/skills/dw-tools-math/references/index.md";
var tick = ((char)96).ToString();
using var document = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var root = document.RootElement;
var selected = root.GetProperty("capabilities").EnumerateArray().ToArray();
var index = File.ReadAllText(IndexPath).Replace("\r\n", "\n", StringComparison.Ordinal);
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    root.GetProperty("owner").GetString() == "math" &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    selected.Length == 4, "Selected manifest identity.");
Validate(index);
var refused = 0;
Reject("missing-link", index.Replace("(skill-version.json)", "(missing.json)", StringComparison.Ordinal));
Reject("broker-promotion", index.Replace("none of these capabilities is " + tick + "BrokerReady" + tick,
    "all of these capabilities are BrokerReady", StringComparison.Ordinal));
Reject("renamed-capability", index.Replace(tick + "math.ir.hash.semantic" + tick,
    tick + "math.ir.hash.algebraic" + tick, StringComparison.Ordinal));
Reject("family-drift", index.Replace("## replay", "## other", StringComparison.Ordinal));
Reject("description-drift", index.Replace("Structural identity, not algebraic equivalence.",
    "Algebraic equivalence proof.", StringComparison.Ordinal));
Reject("extra-entry", index.Replace("## ir\n", "## ir\n- " + tick + "math.unselected.helper" +
    tick + " — Wrong.\n", StringComparison.Ordinal));
Require(refused == 6, "Exactly six refusals.");
Console.WriteLine("MATH-RS200-NAVIGATION|positive=1|negative=6|families=3|capabilities=4|brokerReady=0|PASS");

void Reject(string label, string candidate)
{
    Require(candidate != index, "Mutation did not change navigation: " + label);
    try { Validate(candidate); }
    catch (InvalidDataException)
    {
        refused++;
        Console.WriteLine("REFUSAL " + label + " PASS");
        return;
    }
    throw new InvalidDataException("RS200 mutation accepted: " + label);
}
void Validate(string candidate)
{
    Require(candidate.StartsWith("# Math Discovery — navigation\n", StringComparison.Ordinal),
        "Navigation identity.");
    Require(candidate.Contains("(selected-capabilities.md)", StringComparison.Ordinal) &&
        candidate.Contains("(skill-version.json)", StringComparison.Ordinal),
        "Required links.");
    Require(File.Exists(".agents/skills/dw-tools-math/references/selected-capabilities.md") &&
        File.Exists(".agents/skills/dw-tools-math/references/skill-version.json"),
        "Linked targets missing.");
    Require(candidate.Contains("not broker authorization.", StringComparison.Ordinal) &&
        candidate.Contains("none of these capabilities is " + tick + "BrokerReady" + tick,
            StringComparison.Ordinal), "No broker authorization.");
    var headings = candidate.Split('\n')
        .Where(line => line.StartsWith("## ", StringComparison.Ordinal)).ToArray();
    Require(headings.SequenceEqual(new[] { "## quantities", "## replay", "## ir" },
        StringComparer.Ordinal), "Exact selected families and order.");
    var capabilityLines = candidate.Split('\n')
        .Where(line => line.StartsWith("- " + tick + "math.", StringComparison.Ordinal)).ToArray();
    Require(capabilityLines.Length == selected.Length, "Unselected/missing capability.");
    for (var i = 0; i < selected.Length; i++)
    {
        var item = selected[i];
        Require(item.GetProperty("exposure").GetString() == "AgentCallable" &&
            item.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only",
            "Unqualified selection.");
        var id = item.GetProperty("capabilityId").GetString() ??
            throw new InvalidDataException("Missing capability ID.");
        var explanation = item.GetProperty("result").GetProperty("interpretation").GetString() ??
            throw new InvalidDataException("Missing interpretation.");
        Require(capabilityLines[i] == "- " + tick + id + tick + " — " + explanation,
            "Capability selection/order/interpretation drift: " + id);
    }
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS200: " + message);
}
