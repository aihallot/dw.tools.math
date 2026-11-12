using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;

const string Seed = "docs/discovery/agent-surface-candidates.json";
const string Report = "docs/discovery/qualification/m8-selected-clr-bindings.json";
if (args is not ["--write"] and not ["--check"])
    throw new ArgumentException("Expected --write or --check.");

var selections = new BindingSpec[]
{
    new("math.quantity.exact-pipeline.evaluate", typeof(ExactQuantityPipeline),
        "Evaluate", [typeof(IEnumerable<ExactPipelineStep>)], typeof(ExactPipelineResult)),
    new("math.replay.try-evaluate", typeof(ExactReplayRunner),
        "TryRun", [typeof(IEnumerable<ExactPipelineStep>), typeof(ExactReplayContext),
            typeof(ExactReplayCache), typeof(CancellationToken)], typeof(ExactReplayAttempt)),
    new("math.ir.canonical.encode", typeof(IrCanonicalJsonCodec),
        "Encode", [typeof(IrNode)], typeof(string)),
    new("math.ir.hash.semantic", typeof(IrCanonicalHashes),
        "SemanticStructuralSha256", [typeof(IrNode)], typeof(string))
};

using var source = JsonDocument.Parse(File.ReadAllBytes(Seed));
var proposed = source.RootElement.GetProperty("proposed_agent_callable_seed")
    .EnumerateArray().ToArray();
Require(proposed.Length == selections.Length, "Candidate count changed.");
var ids = new HashSet<string>(StringComparer.Ordinal);
var bindings = new JsonArray();
foreach (var selection in selections)
{
    Require(ids.Add(selection.CapabilityId), "Duplicate selected capability.");
    var metadata = proposed.Single(item =>
        item.GetProperty("capabilityId").GetString() == selection.CapabilityId);
    Require(metadata.GetProperty("exposure").GetString() == "AgentCallable" &&
            !metadata.GetProperty("brokerReady").GetBoolean(), "Unadmitted exposure.");
    Require(metadata.GetProperty("type").GetString() == selection.Owner.FullName &&
            metadata.GetProperty("member").GetString() == selection.Member, "Candidate CLR drift.");
    var declaredParameters = metadata.GetProperty("parameterTypes").EnumerateArray()
        .Select(x => x.GetString()).ToArray();
    Require(declaredParameters.SequenceEqual(selection.ParameterTypes.Select(ClrName)),
        "Selected parameter signature drift: " + selection.CapabilityId);
    Require(metadata.GetProperty("resultType").GetString() == ClrName(selection.ResultType),
        "Selected return type drift: " + selection.CapabilityId);
    var method = selection.Owner.GetMethod(selection.Member,
        BindingFlags.Static | BindingFlags.Public, selection.ParameterTypes);
    if (method is null || method.ReturnType != selection.ResultType)
        throw new InvalidOperationException("Missing qualified CLR overload: " + selection.CapabilityId);
    var declaredAssembly = metadata.GetProperty("assembly").GetString();
    var actualAssembly = method.DeclaringType?.Assembly
        ?? throw new InvalidOperationException("Selected declaring assembly is missing: " + selection.CapabilityId);
    Require(actualAssembly.GetName().Name == declaredAssembly,
        "Selected assembly drift: " + selection.CapabilityId);
    var version = actualAssembly.GetName().Version?.ToString()
        ?? throw new InvalidOperationException("Assembly version unknown.");
    var parameters = string.Join(",", selection.ParameterTypes.Select(ClrName));
    var signature = string.Join("|", declaredAssembly, version, selection.Owner.FullName,
        selection.Member, ClrName(selection.ResultType) + "(" + parameters + ")");
    var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(signature)))
        .ToLowerInvariant();
    bindings.Add(new JsonObject
    {
        ["capabilityId"] = selection.CapabilityId,
        ["assembly"] = declaredAssembly,
        ["assemblyVersion"] = version,
        ["type"] = selection.Owner.FullName,
        ["member"] = selection.Member,
        ["canonicalSignature"] = signature,
        ["fingerprint"] = fingerprint,
        ["source"] = metadata.GetProperty("source").GetString(),
        ["candidateEvidence"] = metadata.GetProperty("evidence").GetString(),
        ["exposure"] = "AgentCallable",
        ["brokerReady"] = false
    });
}
var receipt = new JsonObject
{
    ["schemaVersion"] = 1,
    ["status"] = "selected-clr-fingerprints-only",
    ["protocol"] = "dw.discovery/1.0.0-draft.1",
    ["manifestSchema"] = 2,
    ["sourceCandidate"] = Seed,
    ["sourceConsumerEvidence"] = ".aura/workflow/reports/RS184/attempt-001.json",
    ["sourceProtocolEvidence"] = "docs/discovery/qualification/RS131-portable-protocol-qualified.json",
    ["bindings"] = bindings,
    ["brokerReadyCount"] = 0,
    ["generatedSkill"] = false,
    ["packageConsumerQualified"] = false,
    ["auraIntegrationVerified"] = false
};
var content = receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n";
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(Report)!);
    File.WriteAllText(Report, content, new UTF8Encoding(false));
}
else
{
    Require(File.Exists(Report) && File.ReadAllText(Report) == content,
        "Selected CLR binding receipt is stale or missing.");
}
Console.WriteLine("M8-DISCOVERY-BINDINGS|selected=4|fingerprints=4|brokerReady=0|mode=" + args[0]);

static string ClrName(Type type)
{
    if (type.IsGenericType)
    {
        var prefix = type.GetGenericTypeDefinition().FullName
            ?? throw new InvalidOperationException("Generic name unknown.");
        prefix = prefix[..prefix.IndexOf('`')];
        return prefix + "<" + string.Join(",", type.GetGenericArguments().Select(ClrName)) + ">";
    }
    return type.FullName ?? throw new InvalidOperationException("CLR name unknown.");
}
static void Require(bool condition, string error)
{
    if (!condition) throw new InvalidOperationException(error);
}
sealed record BindingSpec(string CapabilityId, Type Owner, string Member,
    Type[] ParameterTypes, Type ResultType);
