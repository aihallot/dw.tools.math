#:property NuGetAudit=false
#:property PublishAot=false

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// Math's independent draft protocol gate: no Decision assembly, no broker,
// no external package, no unqualified Math feature claims.
// Execute at the repository root:
//   dotnet run --file docs/discovery/qualification/ValidatePortableProtocol.cs
//
// This program validates only a pinned, synthetic portable envelope corpus.
// An independent reflection-bound generated Math skill is a separate gate.

const string SchemaPath = "docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json";
const string CasesPath = "docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string DecisionCommit = "5d3af8580ea984425b314d589def18e031172c2c";
const string ExpectedSchemaSha256 = "678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
const string ExpectedCasesSha256 = "5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";
const string ProtocolId = "dw.discovery";
const string ProtocolVersion = "1.0.0-draft.1";
const int SchemaVersion = 2;

try
{
    var schemaBytes = File.ReadAllBytes(SchemaPath);
    var casesBytes = File.ReadAllBytes(CasesPath);
    var schemaHash = Hex(SHA256.HashData(schemaBytes));
    var casesHash = Hex(SHA256.HashData(casesBytes));
    Require(schemaHash == ExpectedSchemaSha256, "Pinned Decision schema SHA-256 does not match.");
    Require(casesHash == ExpectedCasesSha256, "Pinned Decision conformance SHA-256 does not match.");
    var schema = JsonNode.Parse(Encoding.UTF8.GetString(schemaBytes))!.AsObject();
    Require(Text(schema, "$id") == "urn:dw:discovery:manifest:2:draft-1",
        "Pinned JSON Schema identity changed.");
    var protocol = JsonNode.Parse(Encoding.UTF8.GetString(casesBytes))!.AsObject();
    Require(protocol["fixtureVersion"]?.GetValue<int>() == 1, "Fixture version changed.");
    var baseline = Object(protocol, "baseline");
    var cases = Array(protocol, "cases");
    Require(cases.Count == 13, "The accepted portable corpus must contain thirteen cases.");
    int accepted = 0, rejected = 0;
    var names = new HashSet<string>(StringComparer.Ordinal);

    foreach (var entry in cases)
    {
        var scenario = entry?.AsObject() ??
            throw new InvalidOperationException("Fixture case must be an object.");
        var name = Text(scenario, "name");
        Require(names.Add(name), "Duplicate fixture case: " + name);
        var expected = scenario["accept"]?.GetValue<bool>() ??
            throw new InvalidOperationException("Missing fixture verdict: " + name);
        var manifest = baseline.DeepClone().AsObject();
        foreach (var patchNode in Array(scenario, "patches"))
            ApplyPatch(manifest, patchNode!.AsObject());

        if (scenario["duplicateCapability"]?.GetValue<bool>() == true)
        {
            var capabilities = Array(manifest, "capabilities");
            Require(capabilities.Count > 0, "Cannot duplicate empty capability list.");
            capabilities.Add(capabilities[0]!.DeepClone());
        }

        bool observed;
        try
        {
            ValidateMathDraftEnvelope(manifest);
            observed = true;
        }
        catch (InvalidOperationException)
        {
            observed = false;
        }

        Require(expected == observed,
            $"CASE {name}: expected {expected}, observed {observed}.");
        if (observed) accepted++; else rejected++;
        Console.WriteLine($"CASE {name}: expected={(expected ? "accept" : "reject")} observed={(observed ? "accept" : "reject")} PASS");
    }

    Require(accepted == 2 && rejected == 11, "Unexpected accepted/rejected totals.");
    Console.WriteLine("DECISION_COMMIT " + DecisionCommit);
    Console.WriteLine("PROTOCOL " + ProtocolId + "/" + ProtocolVersion + " manifest-schema=" + SchemaVersion);
    Console.WriteLine("SCHEMA_SHA256 " + schemaHash);
    Console.WriteLine("FIXTURE_SHA256 " + casesHash);
    Console.WriteLine("RESULT PASS 13/13 (2 accepted, 11 rejected) independent Math C# envelope verifier");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine("RESULT FAIL independent Math protocol verifier: " + error.Message);
    return 1;
}

static void ApplyPatch(JsonObject root, JsonObject patch)
{
    var pointer = Text(patch, "path");
    Require(pointer.StartsWith("/", StringComparison.Ordinal), "Patch must use an absolute JSON pointer.");
    var segments = pointer[1..].Split('/').Select(s => s.Replace("~1", "/", StringComparison.Ordinal)
        .Replace("~0", "~", StringComparison.Ordinal)).ToArray();
    JsonNode? parent = root;
    foreach (var segment in segments[..^1])
    {
        if (parent is JsonObject objectNode)
            parent = objectNode[segment];
        else if (parent is JsonArray arrayNode)
            parent = arrayNode[int.Parse(segment, CultureInfo.InvariantCulture)];
        else
            throw new InvalidOperationException("Invalid fixture patch parent.");
        Require(parent is not null, "Missing fixture patch target.");
    }
    var value = patch["value"]?.DeepClone();
    if (parent is JsonObject obj) obj[segments[^1]] = value;
    else if (parent is JsonArray arr)
        arr[int.Parse(segments[^1], CultureInfo.InvariantCulture)] = value;
    else throw new InvalidOperationException("Invalid fixture patch target.");
}

static void ValidateMathDraftEnvelope(JsonObject envelope)
{
    Require(envelope["schemaVersion"]?.GetValue<int>() == 2, "Unsupported schemaVersion.");
    Require(Text(envelope, "protocolId") == "dw.discovery", "Unsupported protocol.");
    Require(Text(envelope, "protocolVersion") == "1.0.0-draft.1", "Unsupported protocol version.");
    var owner = Text(envelope, "owner");
    Require(Regex.IsMatch(owner, "^[a-z0-9-]+$", RegexOptions.CultureInvariant),
        "Invalid owner.");
    foreach (var version in new[] { "libraryRelease", "generatorVersion", "skillBundleVersion" })
        Require(Version(Text(envelope, version)), "Invalid release or generator version.");
    Require(!envelope.ContainsKey("catalogOnly"), "Internal-only catalogue leaked into public manifest.");

    var identifiers = new HashSet<string>(StringComparer.Ordinal);
    foreach (var node in Array(envelope, "capabilities"))
    {
        var cap = node?.AsObject() ?? throw new InvalidOperationException("Invalid capability.");
        var id = Text(cap, "capabilityId");
        Require(Regex.IsMatch(id, "^[a-z0-9-]+(?:\\.[a-z0-9-]+)+$",
            RegexOptions.CultureInvariant) &&
            id.StartsWith(owner + ".", StringComparison.Ordinal), "Wrong capability owner.");
        Require(identifiers.Add(id), "Duplicate capability identifier.");

        foreach (var alias in Array(cap, "legacyIds"))
            Require(identifiers.Add(ScalarText(alias, "legacy capability ID")), "Duplicate alias.");

        Require(Version(Text(cap, "contractVersion")), "Invalid capability contract version.");
        Text(cap, "package");
        Require(Version(Text(cap, "packageVersion")), "Invalid capability package version.");
        Text(cap, "family");
        var exposure = Text(cap, "exposure");
        Require(exposure is "AgentCallable" or "BrokerReady",
            "Only explicitly agent-callable operations can be published.");

        var binding = Object(cap, "binding");
        foreach (var item in new[] { "assembly", "assemblyVersion", "type", "member" })
            Text(binding, item);
        var signature = Text(binding, "canonicalSignature");
        Require(Text(binding, "fingerprint") == Hex(
                SHA256.HashData(Encoding.UTF8.GetBytes(signature))),
            "CLR binding signature fingerprint mismatch.");

        var paramNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in Array(cap, "parameters"))
        {
            var param = value?.AsObject() ??
                throw new InvalidOperationException("Invalid parameter.");
            Require(paramNames.Add(Text(param, "name")), "Duplicate parameter.");
            Text(param, "type");
            Require(param["optional"] is JsonValue v &&
                v.TryGetValue<bool>(out _), "Missing parameter optionality.");
            Require(param.ContainsKey("defaultValue") &&
                (param["defaultValue"] is null || param["defaultValue"] is JsonValue d &&
                    d.TryGetValue<string>(out _)), "Invalid defaultValue.");
        }

        var result = Object(cap, "result");
        Text(result, "type");
        Text(result, "interpretation");

        var semantics = Object(cap, "semantics");
        foreach (var name in new[] { "numericModel", "guarantee", "limits" })
            Text(semantics, name);
        var assumptions = Array(semantics, "assumptions");
        Require(assumptions.Count != 0, "Assumptions missing.");
        foreach (var assumption in assumptions)
            ScalarText(assumption, "assumption");

        var execution = Object(cap, "execution");
        foreach (var name in new[] {
            "kind", "budget", "cancellation", "provider", "platformRequirements",
            "effects", "failurePolicy" })
            Text(execution, name);
        Require(Text(execution, "authorization") == "runtime-only",
            "Capability metadata cannot grant authorization.");

        var provenance = Object(cap, "provenance");
        Require(Regex.IsMatch(Text(provenance, "sourceCommit"), "^[0-9a-fA-F]{40}$"),
            "Invalid provenance commit.");
        var evidence = Array(provenance, "evidence");
        Require(evidence.Count != 0, "Evidence cannot be empty.");
        foreach (var evidenceItem in evidence)
            ScalarText(evidenceItem, "evidence reference");

        var example = Object(cap, "example");
        Text(example, "file");
        Text(example, "method");

        if (exposure == "BrokerReady")
        {
            var broker = Object(cap, "brokerContract");
            foreach (var name in new[] {
                "inputSchema", "outputSchema", "adapter", "qualificationEvidence" })
                Text(broker, name);
        }
    }

    if (envelope.TryGetPropertyValue("brokerCandidateIds", out var candidateIds) &&
        candidateIds is not null)
    {
        foreach (var candidate in Array(envelope, "brokerCandidateIds"))
            ScalarText(candidate, "broker candidate");
    }
}

static bool Version(string value) => Regex.IsMatch(value,
    @"^[0-9]+\.[0-9]+\.[0-9]+(?:-[A-Za-z0-9.-]+)?(?:\+[A-Za-z0-9.-]+)?$",
    RegexOptions.CultureInvariant);

static string Hex(byte[] data) => Convert.ToHexString(data).ToLowerInvariant();

static string ScalarText(JsonNode? node, string label)
{
    if (node is not JsonValue value ||
        !value.TryGetValue<string>(out var text) || string.IsNullOrWhiteSpace(text))
        throw new InvalidOperationException("Missing or invalid " + label);
    return text;
}

static string Text(JsonObject obj, string name) => ScalarText(obj[name], name);

static JsonObject Object(JsonObject obj, string name) =>
    obj[name] as JsonObject ??
        throw new InvalidOperationException("Expected object: " + name);

static JsonArray Array(JsonObject obj, string name) =>
    obj[name] as JsonArray ??
        throw new InvalidOperationException("Expected array: " + name);

static void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}
