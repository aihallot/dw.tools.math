#:property PublishAot=false
#:property NuGetAudit=false
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

const string Bindings = "docs/discovery/qualification/m8-selected-clr-bindings.json";
const string Candidates = "docs/discovery/agent-surface-candidates.json";
const string Output = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Evidence = ".aura/workflow/reports/RS185/attempt-001.json";

if (args.Length != 1 || args[0] is not ("--write" or "--check"))
    throw new ArgumentException("Use --write or --check.");

using var bindingsDoc = JsonDocument.Parse(File.ReadAllBytes(Bindings));
using var candidateDoc = JsonDocument.Parse(File.ReadAllBytes(Candidates));
var root = bindingsDoc.RootElement;
var candidates = candidateDoc.RootElement.GetProperty("proposed_agent_callable_seed")
    .EnumerateArray().ToArray();
var selected = root.GetProperty("bindings").EnumerateArray().ToArray();
Require(root.GetProperty("status").GetString() == "selected-clr-fingerprints-only",
    "RS185 binding receipt has unexpected status.");
Require(root.GetProperty("brokerReadyCount").GetInt32() == 0 &&
    !root.GetProperty("generatedSkill").GetBoolean(), "Premature skill or broker claim.");
Require(selected.Length == 4 && candidates.Length == 4, "Unexpected selected count.");
var unique = new HashSet<string>(StringComparer.Ordinal);
var capabilities = new JsonArray();
foreach (var binding in selected)
{
    var id = Text(binding, "capabilityId");
    Require(unique.Add(id) && id.StartsWith("math.", StringComparison.Ordinal),
        "Selected capability identifier or owner changed.");
    var meta = candidates.Single(m => Text(m, "capabilityId") == id);
    Require(Text(meta, "exposure") == "AgentCallable" &&
        !meta.GetProperty("brokerReady").GetBoolean(), "Unqualified exposure.");
    foreach (var field in new[] { "assembly", "type", "member" })
        Require(Text(meta, field) == Text(binding, field), "Source/CLR metadata drift: " + field);
    var signature = Text(binding, "canonicalSignature");
    Require(Text(binding, "fingerprint") == Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(signature))).ToLowerInvariant(),
        "CLR binding fingerprint mismatch.");
    var parameterTypes = meta.GetProperty("parameterTypes").EnumerateArray()
        .Select(v => v.GetString() ?? throw new InvalidDataException("Parameter type missing."))
        .ToArray();
    var parameterNames = id switch
    {
        "math.quantity.exact-pipeline.evaluate" => new[] { "steps" },
        "math.replay.try-evaluate" => new[] { "steps", "context", "cache", "cancellationToken" },
        "math.ir.canonical.encode" => new[] { "node" },
        "math.ir.hash.semantic" => new[] { "node" },
        _ => throw new InvalidDataException("Unexpected selected capability " + id)
    };
    Require(parameterTypes.Length == parameterNames.Length, "Parameter cardinality drift.");
    var parameters = new JsonArray();
    for (var i = 0; i < parameterTypes.Length; i++)
    {
        var optional = id == "math.replay.try-evaluate" && i >= 2;
        parameters.Add(new JsonObject
        {
            ["name"] = parameterNames[i], ["type"] = parameterTypes[i],
            ["optional"] = optional, ["defaultValue"] = optional && i == 3 ? "default" : null
        });
    }
    var family = Text(meta, "family");
    var sourceEvidence = Text(binding, "candidateEvidence");
    var limits = Text(meta, "limits");
    var description = id switch
    {
        "math.quantity.exact-pipeline.evaluate" => "Exact bounded quantity-pipeline outcome.",
        "math.replay.try-evaluate" => "Typed replay attempt with explicit incomplete outcomes.",
        "math.ir.canonical.encode" => "Canonical math-ir/1 JSON for admitted bounded IR nodes.",
        "math.ir.hash.semantic" => "Structural identity, not algebraic equivalence.",
        _ => throw new InvalidDataException("Unrecognized capability")
    };
    var numericModel = Text(meta, "precision");
    capabilities.Add(new JsonObject
    {
        ["capabilityId"] = id,
        ["legacyIds"] = new JsonArray(),
        ["contractVersion"] = "0.1.0",
        ["package"] = Text(meta, "package"),
        ["packageVersion"] = Text(meta, "packageVersion"),
        ["family"] = family,
        ["exposure"] = "AgentCallable",
        ["binding"] = new JsonObject
        {
            ["assembly"] = Text(binding, "assembly"),
            ["assemblyVersion"] = Text(binding, "assemblyVersion"),
            ["type"] = Text(binding, "type"),
            ["member"] = Text(binding, "member"),
            ["canonicalSignature"] = signature,
            ["fingerprint"] = Text(binding, "fingerprint")
        },
        ["parameters"] = parameters,
        ["result"] = new JsonObject
        {
            ["type"] = Text(meta, "resultType"),
            ["interpretation"] = description
        },
        ["semantics"] = new JsonObject
        {
            ["numericModel"] = numericModel,
            ["assumptions"] = new JsonArray("Caller provides valid explicitly typed Math inputs."),
            ["guarantee"] = Text(meta, "oracle"),
            ["limits"] = limits
        },
        ["execution"] = new JsonObject
        {
            ["kind"] = "direct-dotnet",
            ["budget"] = limits,
            ["cancellation"] = "Bounded cooperative policy; method-specific cancellation rules apply.",
            ["provider"] = "Pure managed Math library; no external provider implied.",
            ["platformRequirements"] = "net10.0, current Math source-project evidence only.",
            ["effects"] = "In-process mathematical computation; no external effects granted.",
            ["failurePolicy"] = "Respect the method's explicit result or exception contract.",
            ["authorization"] = "runtime-only"
        },
        ["provenance"] = new JsonObject
        {
            ["sourceCommit"] = "ae5125f809bbeea6b773ad99285fe2ce33fdbf46",
            ["evidence"] = new JsonArray(sourceEvidence,
                "docs/discovery/qualification/RS131-portable-protocol-qualified.json",
                Evidence)
        },
        ["example"] = new JsonObject
        {
            ["file"] = "tests/consumers/m8-discovery-clr-seed/Program.cs",
            ["method"] = "top-level-entrypoint"
        }
    });
}
var manifest = new JsonObject
{
    ["schemaVersion"] = 2,
    ["protocolId"] = "dw.discovery",
    ["protocolVersion"] = "1.0.0-draft.1",
    ["owner"] = "math",
    ["libraryRelease"] = "0.3.0-preview.1",
    ["generatorVersion"] = "0.1.0",
    ["skillBundleVersion"] = "0.1.0",
    ["capabilities"] = capabilities,
    ["brokerCandidateIds"] = new JsonArray()
};
Validate(manifest);
var output = manifest.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n";
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(Output)!);
    File.WriteAllText(Output, output, new UTF8Encoding(false));
}
else
    Require(File.Exists(Output) && File.ReadAllText(Output) == output,
        "Manifest is absent, non-deterministic or stale.");
Console.WriteLine("MATH-RS186-MANIFEST|schema=2|capabilities=4|brokerReady=0|mode=" + args[0]);

static string Text(JsonElement element, string field) =>
    element.GetProperty(field).GetString() ?? throw new InvalidDataException("Missing text " + field);

static void Validate(JsonObject manifest)
{
    Require(manifest["schemaVersion"]?.GetValue<int>() == 2 &&
        (string?)manifest["protocolId"] == "dw.discovery" &&
        (string?)manifest["owner"] == "math", "Protocol identity.");
    Require(manifest["capabilities"] is JsonArray list && list.Count == 4, "Four capabilities.");
    var ids = new HashSet<string>(StringComparer.Ordinal);
    foreach (var item in (JsonArray)manifest["capabilities"]!)
    {
        var capability = item?.AsObject() ?? throw new InvalidDataException("Null capability.");
        var id = (string?)capability["capabilityId"] ??
            throw new InvalidDataException("Missing capability ID.");
        Require(Regex.IsMatch(id, "^math\\.[a-z0-9.-]+$", RegexOptions.CultureInvariant) &&
            ids.Add(id), "Invalid or duplicate capability.");
        Require((string?)capability["exposure"] == "AgentCallable" &&
            capability["brokerContract"] is null, "Broker readiness is not admitted.");
        var details = capability["binding"]?.AsObject() ??
            throw new InvalidDataException("Missing CLR binding.");
        var signature = (string?)details["canonicalSignature"] ??
            throw new InvalidDataException("Missing canonical signature.");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(signature)))
            .ToLowerInvariant();
        Require((string?)details["fingerprint"] == hash, "Signature fingerprint drift.");
        Require((string?)capability["execution"]?["authorization"] == "runtime-only",
            "Metadata must not grant authorization.");
        Require(capability["parameters"] is JsonArray, "Missing parameter list.");
    }
    Require(manifest["brokerCandidateIds"] is JsonArray empty && empty.Count == 0,
        "Premature broker candidates.");
}
static void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS186: " + reason);
}
