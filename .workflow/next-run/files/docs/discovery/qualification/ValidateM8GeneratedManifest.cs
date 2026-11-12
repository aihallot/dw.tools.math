#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

const string Manifest = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Bindings = "docs/discovery/qualification/m8-selected-clr-bindings.json";
const string Candidates = "docs/discovery/agent-surface-candidates.json";
const string SourceEvidence = ".aura/workflow/reports/RS186/attempt-001.json";

var manifest = Load(Manifest);
var bindingReceipt = Load(Bindings);
var candidates = Load(Candidates);
var source = Load(SourceEvidence);
Require((string?)source["outcome"] == "succeeded" &&
        (string?)source["runId"] == "RS186" &&
        source["attempt"]?.GetValue<int>() == 1, "RS186 source receipt.");
Validate(manifest, bindingReceipt, candidates);

var refusals = 0;
Reject("schema-version", copy => copy["schemaVersion"] = 1);
Reject("protocol-id", copy => copy["protocolId"] = "unexpected");
Reject("owner", copy => copy["owner"] = "decision");
Reject("duplicate-id", copy =>
{
    var caps = RequiredArray(copy, "capabilities");
    caps.Add(caps[0]!.DeepClone());
});
Reject("binding-fingerprint", copy =>
    Capability(copy, 0)["binding"]!["fingerprint"] = new string('0', 64));
Reject("broker-ready", copy => Capability(copy, 0)["exposure"] = "BrokerReady");
Reject("runtime-authorization", copy =>
    Capability(copy, 0)["execution"]!["authorization"] = "granted");
Reject("missing-limits", copy =>
    Capability(copy, 0)["semantics"]!["limits"] = "");
Reject("candidate-drift", copy =>
    Capability(copy, 0)["binding"]!["member"] = "UnknownMember");
Reject("catalogue-leak", copy => copy["catalogOnly"] = new JsonArray());
Reject("premature-candidate", copy =>
    RequiredArray(copy, "brokerCandidateIds").Add("math.ir.canonical.encode"));

Require(refusals == 11, "Incomplete negative-case matrix.");
Console.WriteLine("MATH-RS187-MANIFEST|positive=1|negative=11|capabilities=4|brokerReady=0|PASS");

void Reject(string name, Action<JsonObject> mutate)
{
    var changed = manifest.DeepClone().AsObject();
    mutate(changed);
    try
    {
        Validate(changed, bindingReceipt, candidates);
    }
    catch (InvalidDataException)
    {
        refusals++;
        Console.WriteLine("REFUSAL " + name + " PASS");
        return;
    }
    throw new InvalidDataException("Mutation unexpectedly accepted: " + name);
}

static JsonObject Load(string path) =>
    JsonNode.Parse(File.ReadAllText(path))?.AsObject()
    ?? throw new InvalidDataException("Missing or invalid JSON " + path);

static JsonArray RequiredArray(JsonObject root, string key) =>
    root[key] as JsonArray ?? throw new InvalidDataException("Missing array " + key);

static JsonObject Capability(JsonObject root, int index) =>
    RequiredArray(root, "capabilities")[index]?.AsObject()
    ?? throw new InvalidDataException("Missing capability " + index);

static string Text(JsonObject obj, string name) =>
    obj[name]?.GetValue<string>() is { Length: > 0 } value
        ? value : throw new InvalidDataException("Missing value " + name);

static void Validate(JsonObject root, JsonObject bindings, JsonObject candidates)
{
    Require(root["schemaVersion"]?.GetValue<int>() == 2, "Schema version.");
    Require(Text(root, "protocolId") == "dw.discovery" &&
            Text(root, "protocolVersion") == "1.0.0-draft.1" &&
            Text(root, "owner") == "math", "Portable protocol identity.");
    Require(!root.ContainsKey("catalogOnly"), "Catalogued helpers leak.");
    Require(RequiredArray(root, "brokerCandidateIds").Count == 0, "Broker candidates.");
    Require(Text(root, "generatorVersion") == "0.1.0" &&
            Text(root, "skillBundleVersion") == "0.1.0", "Version drift.");

    var caps = RequiredArray(root, "capabilities");
    var pins = RequiredArray(bindings, "bindings");
    var metadata = RequiredArray(candidates, "proposed_agent_callable_seed");
    Require(caps.Count == 4 && pins.Count == 4 && metadata.Count == 4,
        "Selected cardinality.");

    var identifiers = new HashSet<string>(StringComparer.Ordinal);
    for (var i = 0; i < caps.Count; i++)
    {
        var cap = Capability(root, i);
        var pin = pins[i]?.AsObject() ?? throw new InvalidDataException("Binding pin.");
        var seed = metadata[i]?.AsObject() ?? throw new InvalidDataException("Candidate seed.");
        var id = Text(cap, "capabilityId");
        Require(id == Text(pin, "capabilityId") &&
                id == Text(seed, "capabilityId") &&
                Regex.IsMatch(id, "^math\\.[a-z0-9-]+(?:\\.[a-z0-9-]+)+$",
                    RegexOptions.CultureInvariant) &&
                identifiers.Add(id), "Selected capability identity.");
        Require(Text(cap, "exposure") == "AgentCallable" &&
                !cap.ContainsKey("brokerContract"), "Premature broker exposure.");
        Require(RequiredArray(cap, "legacyIds").Count == 0, "Unqualified alias.");
        var binding = cap["binding"]?.AsObject()
            ?? throw new InvalidDataException("Missing CLR binding.");
        foreach (var field in new[] { "assembly", "assemblyVersion", "type", "member",
                                     "canonicalSignature", "fingerprint" })
            Require(Text(binding, field) == Text(pin, field), "CLR binding drift " + field);
        var canonical = Text(binding, "canonicalSignature");
        var fingerprint = Convert.ToHexString(SHA256.HashData(
            Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        Require(Text(binding, "fingerprint") == fingerprint, "Binding hash mismatch.");
        Require(Text(cap, "package") == Text(seed, "package") &&
                Text(cap, "packageVersion") == Text(seed, "packageVersion") &&
                Text(cap, "family") == Text(seed, "family"), "Package metadata drift.");
        var semantic = cap["semantics"]?.AsObject()
            ?? throw new InvalidDataException("Missing semantics.");
        Require(Text(semantic, "limits") == Text(seed, "limits") &&
                Text(semantic, "guarantee") == Text(seed, "oracle") &&
                RequiredArray(semantic, "assumptions").Count > 0, "Numerical contract drift.");
        var execution = cap["execution"]?.AsObject()
            ?? throw new InvalidDataException("Missing execution metadata.");
        Require(Text(execution, "authorization") == "runtime-only", "Authorization leak.");
        var parameters = RequiredArray(cap, "parameters");
        Require(parameters.Count == RequiredArray(seed, "parameterTypes").Count,
            "Parameter count drift.");
        foreach (var parameter in parameters)
        {
            var field = parameter?.AsObject()
                ?? throw new InvalidDataException("Missing parameter.");
            Text(field, "name");
            Text(field, "type");
            Require(field["optional"] is JsonValue &&
                    field.ContainsKey("defaultValue"), "Parameter shape.");
        }
        var result = cap["result"]?.AsObject()
            ?? throw new InvalidDataException("Missing result.");
        Require(Text(result, "type") == Text(seed, "resultType"), "Return type drift.");
        var provenance = cap["provenance"]?.AsObject()
            ?? throw new InvalidDataException("Missing provenance.");
        Require(RequiredArray(provenance, "evidence").Count >= 2,
            "Unqualified provenance.");
    }
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS187: " + message);
}
