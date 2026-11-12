#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string Manifest = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
using var source = JsonDocument.Parse(File.ReadAllText(Manifest));
var root = source.RootElement;
var text = File.ReadAllText(Skill);
var entries = root.GetProperty("capabilities").EnumerateArray().ToArray();
Require(entries.Length == 4, "Exactly four selected capabilities.");
Require(root.GetProperty("brokerCandidateIds").GetArrayLength() == 0, "No broker candidates.");
Validate(text);
var refusals = 0;
Reject("broker-promotion", text.Replace("Exposure: `AgentCallable` only.", "Exposure: `BrokerReady` only.", StringComparison.Ordinal));
Reject("authorization-grant", text.Replace("runtime-only", "granted", StringComparison.Ordinal));
Reject("missing-capability", text.Replace("### `math.ir.hash.semantic`", "### `unknown`", StringComparison.Ordinal));
Reject("signature-drift", text.Replace("SemanticStructuralSha256", "SemanticOtherHash", StringComparison.Ordinal));
Reject("limit-drift", text.Replace("MaximumJsonCharacters=262144", "MaximumJsonCharacters=1", StringComparison.Ordinal));
Reject("missing-fingerprint", text.Replace("ec4026982c019292a25cf8a683210890ce30d6cd3da8bc0c58c069a324345676", new string('0',64), StringComparison.Ordinal));
Reject("missing-boundary", text.Replace("Do not expose any capability as BrokerReady.", "BrokerReady is enabled.", StringComparison.Ordinal));
Require(refusals == 7, "Complete negative matrix.");
Console.WriteLine("MATH-RS189-SKILL|positive=1|negative=7|capabilities=4|brokerReady=0|PASS");

void Reject(string label, string candidate)
{
    Require(candidate != text, "Mutation did not alter source: " + label);
    try { Validate(candidate); }
    catch (InvalidDataException)
    {
        refusals++;
        Console.WriteLine("REFUSAL " + label + " PASS");
        return;
    }
    throw new InvalidDataException("Mutation unexpectedly accepted: " + label);
}

void Validate(string candidate)
{
    Require(candidate.StartsWith("---\nname: dw-tools-math\n", StringComparison.Ordinal),
        "Skill identity.");
    Require(candidate.Contains("Protocol: `dw.discovery/1.0.0-draft.1`", StringComparison.Ordinal),
        "Protocol identity.");
    Require(candidate.Contains("Exposure: `AgentCallable` only.", StringComparison.Ordinal),
        "Exposure boundary.");
    Require(candidate.Contains("This file grants no runtime permission", StringComparison.Ordinal),
        "No runtime grant.");
    Require(candidate.Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal),
        "Explicit broker refusal.");
    Require(!candidate.Contains("Exposure: `BrokerReady`", StringComparison.Ordinal),
        "No broker promotion.");
    foreach (var entry in entries)
    {
        string Read(JsonElement e, string field) => e.GetProperty(field).GetString()
            ?? throw new InvalidDataException("Missing " + field);
        var id = Read(entry, "capabilityId");
        var binding = entry.GetProperty("binding");
        var semantics = entry.GetProperty("semantics");
        var auth = Read(entry.GetProperty("execution"), "authorization");
        Require(auth == "runtime-only" && Read(entry, "exposure") == "AgentCallable",
            "Manifest permission boundary.");
        foreach (var expected in new[]
        {
            "### `" + id + "`",
            Read(binding, "type") + "." + Read(binding, "member"),
            Read(binding, "canonicalSignature"),
            Read(binding, "fingerprint"),
            Read(semantics, "limits"),
            "- Authorization: `runtime-only`"
        })
            Require(candidate.Contains(expected, StringComparison.Ordinal),
                "Missing or changed qualified skill content: " + id);
    }
    Require(candidate.Split("### `math.", StringSplitOptions.None).Length - 1 == 4,
        "Unexpected capability headings.");
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS189: " + message);
}
