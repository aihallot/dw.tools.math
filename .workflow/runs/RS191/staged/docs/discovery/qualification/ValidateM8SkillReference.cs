#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string ReferencePath = ".agents/skills/dw-tools-math/references/selected-capabilities.md";
using var doc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var root = doc.RootElement;
var selected = root.GetProperty("capabilities").EnumerateArray().ToArray();
var reference = File.ReadAllText(ReferencePath).Replace("\r\n", "\n", StringComparison.Ordinal);
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    selected.Length == 4, "Qualified source selection");
Validate(reference);
var refused = 0;
Reject("broker-promotion",
    reference.Replace("No item is BrokerReady;", "Every item is BrokerReady;", StringComparison.Ordinal));
Reject("runtime-grant",
    reference.Replace("authorization: `runtime-only`", "authorization: `granted`", StringComparison.Ordinal));
Reject("missing-capability",
    reference.Replace("## `math.ir.hash.semantic`", "## `math.ir.hash.unknown`", StringComparison.Ordinal));
Reject("fingerprint-drift",
    reference.Replace("bee43baf778153e5d26959e8c803de9fc25df0026c65aa40f55f16dd189a621c",
        new string('0', 64), StringComparison.Ordinal));
Reject("numeric-limit-drift",
    reference.Replace("MaximumJsonCharacters=262144", "MaximumJsonCharacters=100", StringComparison.Ordinal));
Reject("optional-parameter-drift",
    reference.Replace("`cancellationToken`: `System.Threading.CancellationToken` (optional)",
        "`cancellationToken`: `System.Threading.CancellationToken` (required)", StringComparison.Ordinal));
Reject("package-drift",
    reference.Replace("dw.tools.math.composition", "dw.tools.math.unverified", StringComparison.Ordinal));
Reject("failure-policy-drift",
    reference.Replace("Respect the method's explicit result or exception contract.",
        "All failures are silently ignored.", StringComparison.Ordinal));
Require(refused == 8, "Complete negative matrix");
Console.WriteLine("MATH-RS191-REFERENCE|positive=1|negative=8|capabilities=4|brokerReady=0|PASS");

void Reject(string name, string mutated)
{
    Require(!string.Equals(mutated, reference, StringComparison.Ordinal),
        "No-op mutation " + name);
    try { Validate(mutated); }
    catch (InvalidDataException)
    {
        refused++;
        Console.WriteLine("REFUSAL " + name + " PASS");
        return;
    }
    throw new InvalidDataException("RS191 unexpectedly accepted " + name);
}

void Validate(string candidate)
{
    Require(candidate.StartsWith("# Math Discovery — selected capability reference\n",
        StringComparison.Ordinal), "Reference identity");
    Require(candidate.Contains("This is source-project CLR evidence, not a package-installed or broker-authorized consumer.",
        StringComparison.Ordinal), "Source-only provenance");
    Require(candidate.Contains("No item is BrokerReady; invocation must be authorized independently by the runtime.",
        StringComparison.Ordinal), "Broker authorization boundary");
    Require(!candidate.Contains("authorization: `granted`", StringComparison.Ordinal),
        "No permission grant");
    var blocks = candidate.Split("\n## `", StringSplitOptions.None);
    Require(blocks.Length == selected.Length + 1, "Exactly four reference entries");
    for (var i = 0; i < selected.Length; i++)
    {
        var capability = selected[i];
        var block = "`" + blocks[i + 1];
        string Read(JsonElement value, string field) =>
            value.GetProperty(field).GetString()
            ?? throw new InvalidDataException("Missing " + field);
        var id = Read(capability, "capabilityId");
        Require(block.StartsWith("`" + id + "`\n", StringComparison.Ordinal),
            "Capability order and identity");
        var binding = capability.GetProperty("binding");
        var result = capability.GetProperty("result");
        var semantics = capability.GetProperty("semantics");
        var execution = capability.GetProperty("execution");
        Require(Read(capability, "exposure") == "AgentCallable" &&
            Read(execution, "authorization") == "runtime-only", "Selected runtime-only exposure");
        var expected = new[]
        {
            "- Package: `" + Read(capability, "package") + "` (`" +
                Read(capability, "packageVersion") + "`).",
            "- CLR binding: `" + Read(binding, "type") + "." +
                Read(binding, "member") + "`.",
            "- Signature SHA-256: `" + Read(binding, "fingerprint") + "`.",
            "- Result: `" + Read(result, "type") + "`.",
            "- Interpretation: " + Read(result, "interpretation"),
            "- Numerical limits: " + Read(semantics, "limits"),
            "- Failure policy: " + Read(execution, "failurePolicy"),
            "- Evidence example: `" + Read(capability.GetProperty("example"), "file") + "`.",
            "- Exposure: `AgentCallable`; authorization: `runtime-only`.",
        };
        foreach (var line in expected)
            Require(block.Contains(line, StringComparison.Ordinal),
                "Reference content drift for " + id + ": " + line);
        var parameters = capability.GetProperty("parameters").EnumerateArray().ToArray();
        Require(block.Contains("- Parameters:\n", StringComparison.Ordinal),
            "Parameters missing for " + id);
        var actualParameters = block.Split("\n  - `", StringSplitOptions.None);
        Require(actualParameters.Length - 1 == parameters.Length, "Parameter count for " + id);
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var expectedParameter = Read(parameter, "name") + "`: `" +
                Read(parameter, "type") + "`" +
                (parameter.GetProperty("optional").GetBoolean() ? " (optional)" : " (required)") + ".";
            Require(actualParameters[index + 1].StartsWith(expectedParameter, StringComparison.Ordinal),
                "Parameter order/type/optionality drift for " + id);
        }
    }
}
static void Require(bool value, string message)
{
    if (!value) throw new InvalidDataException("RS191: " + message);
}
