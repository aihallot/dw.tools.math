#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string CorpusPath = "docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string RedReportPath = ".aura/workflow/reports/RS286/attempt-001.json";

if (args.Length == 2 && args[0] == "--validate")
{
    var arbitrary = Load(args[1]);
    var accepted = CheckMetadata(arbitrary);
    Console.WriteLine("MATH-M8-STRICT-BINDING|mode=validate|capabilities=" +
        accepted + "|metadataOnly=true|brokerReady=0|PASS");
    return;
}
if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Use --check or --validate <manifest-json-path>.");

using (var reportDocument = JsonDocument.Parse(File.ReadAllText(RedReportPath)))
{
    var report = reportDocument.RootElement;
    var prior = report.GetProperty("priorInvocationFailures");
    Need(report.GetProperty("runId").GetString() == "RS286" &&
        report.GetProperty("outcome").GetString() == "succeeded" &&
        report.GetProperty("attempt").GetInt32() == 1 &&
        prior.GetProperty("count").GetInt32() == 0 &&
        prior.GetProperty("firstPassSucceeded").GetBoolean() &&
        report.GetProperty("contribution").GetProperty("observed")
            .GetProperty("validations").EnumerateArray().Any(item =>
                item.GetProperty("id").GetString() ==
                "m8-protocol-red-original-verifier-metadata-gap"),
        "The original RS286 RED gap observation is missing or altered.");
}

var manifest = Load(ManifestPath);
var admitted = CheckMetadata(manifest);
string[] selected =
[
    "math.quantity.exact-pipeline.evaluate",
    "math.replay.try-evaluate",
    "math.ir.canonical.encode",
    "math.ir.hash.semantic"
];
var actual = Capabilities(manifest).Select(item => Read(item.AsObject(), "capabilityId")).ToArray();
Need(admitted == selected.Length && actual.SequenceEqual(selected, StringComparer.Ordinal),
    "Only the four source-qualified Math capability identities are admitted.");

var corpus = Load(CorpusPath);
Need(corpus["fixtureVersion"]?.GetValue<int>() == 1,
    "Pinned Decision draft-1 fixture version is missing.");
var reference = corpus["baseline"]?.AsObject()
    ?? throw new InvalidDataException("RS287: Missing pinned portable fixture baseline.");
Need(CheckMetadata(reference) == 1, "Historical synthetic portable envelope is not a positive input.");

var refused = 0;
void Reject(string name, Action<JsonObject> alter, string expectedReason)
{
    var candidate = reference.DeepClone().AsObject();
    alter(Capabilities(candidate)[0]!.AsObject());
    try
    {
        CheckMetadata(candidate);
    }
    catch (InvalidDataException error)
        when (error.Message.Contains(expectedReason, StringComparison.Ordinal))
    {
        refused++;
        Console.WriteLine("MATH-RS287-REFUSAL|" + name + "|PASS");
        return;
    }
    throw new InvalidDataException("RS287 admitted or misclassified invalid metadata: " + name);
}
Reject("parameter-type-drift",
    item => Parameters(item)[0]!.AsObject()["type"] = "System.String",
    "Canonical CLR signature does not match declared fields");
Reject("binding-member-drift",
    item => Binding(item)["member"] = "NotMultiply",
    "Canonical CLR signature does not match declared fields");
Reject("return-type-drift",
    item => item["result"]!.AsObject()["type"] = "System.String",
    "Canonical CLR signature does not match declared fields");
Reject("assembly-version-drift",
    item => Binding(item)["assemblyVersion"] = "9.9.9.9",
    "Canonical CLR signature does not match declared fields");
Reject("fingerprint-tamper",
    item => Binding(item)["fingerprint"] = new string('0', 64),
    "Canonical CLR SHA-256 fingerprint differs");
Reject("unauthorized-runtime-grant",
    item => item["execution"]!.AsObject()["authorization"] = "granted",
    "Runtime-only authorization");
Reject("broker-ready-promotion",
    item => item["exposure"] = "BrokerReady",
    "AgentCallable only");
Need(refused == 7, "Expected seven strict independent refusals.");
Console.WriteLine("MATH-RS287-STRICT-BINDING|selected=4|syntheticPositive=1|" +
    "refusals=7|redGapsClosed=2|brokerReady=0|metadataOnly=true|PASS");

static JsonObject Load(string path) =>
    JsonNode.Parse(File.ReadAllText(path))?.AsObject()
    ?? throw new InvalidDataException("RS287: Missing or invalid JSON object: " + path);

static JsonArray Capabilities(JsonObject root) =>
    root["capabilities"] as JsonArray
    ?? throw new InvalidDataException("RS287: Missing capabilities.");

static JsonArray Parameters(JsonObject capability) =>
    capability["parameters"] as JsonArray
    ?? throw new InvalidDataException("RS287: Missing CLR parameters.");

static JsonObject Binding(JsonObject capability) =>
    capability["binding"] as JsonObject
    ?? throw new InvalidDataException("RS287: Missing CLR binding.");

static string Read(JsonObject node, string field) =>
    node[field]?.GetValue<string>() is { Length: > 0 } value
        ? value
        : throw new InvalidDataException("RS287: Missing field " + field);

static int CheckMetadata(JsonObject root)
{
    Need(root["schemaVersion"]?.GetValue<int>() == 2 &&
        Read(root, "protocolId") == "dw.discovery" &&
        Read(root, "protocolVersion") == "1.0.0-draft.1" &&
        Read(root, "owner") == "math",
        "Only the Math Discovery schema-2 draft-1 envelope is in scope.");
    Need(root["brokerCandidateIds"] is JsonArray brokers && brokers.Count == 0,
        "Broker candidate escalation is not admitted.");
    var capabilities = Capabilities(root);
    Need(capabilities.Count is >= 1 and <= 32,
        "Bounded Math capability count required.");
    var seen = new HashSet<string>(StringComparer.Ordinal);
    foreach (var node in capabilities)
    {
        var cap = node?.AsObject()
            ?? throw new InvalidDataException("RS287: Null selected capability.");
        var id = Read(cap, "capabilityId");
        Need(id.StartsWith("math.", StringComparison.Ordinal) && seen.Add(id),
            "Unique Math-owned capability identity required.");
        Need(Read(cap, "exposure") == "AgentCallable" &&
            !cap.ContainsKey("brokerContract"),
            "AgentCallable only; no BrokerReady contract.");
        Need(cap["execution"] is JsonObject execution &&
            Read(execution, "authorization") == "runtime-only",
            "Runtime-only authorization is required.");
        var binding = Binding(cap);
        var result = cap["result"] as JsonObject
            ?? throw new InvalidDataException("RS287: Missing result metadata.");
        var parameters = Parameters(cap);
        var typeNames = parameters.Select(parameter =>
        {
            var field = parameter?.AsObject()
                ?? throw new InvalidDataException("RS287: Invalid parameter metadata.");
            return Read(field, "type");
        });
        var reconstructed = string.Join("|",
            Read(binding, "assembly"),
            Read(binding, "assemblyVersion"),
            Read(binding, "type"),
            Read(binding, "member"),
            Read(result, "type") + "(" + string.Join(",", typeNames) + ")");
        Need(Read(binding, "canonicalSignature") == reconstructed,
            "Canonical CLR signature does not match declared fields: " + id);
        var fingerprint = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(reconstructed)));
        Need(Read(binding, "fingerprint") == fingerprint,
            "Canonical CLR SHA-256 fingerprint differs: " + id);
    }
    return capabilities.Count;
}
static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS287: " + reason);
}
