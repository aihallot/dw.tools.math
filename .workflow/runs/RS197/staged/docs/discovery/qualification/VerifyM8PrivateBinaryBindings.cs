#:property NuGetAudit=false
#:property PublishAot=false

using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string CompositionReceipt = "docs/distribution/nuget-federation-f34-composition-published-qualified.json";
const string IrReceipt = "docs/distribution/nuget-federation-f13-ir-published-qualified.json";
const string QuantitiesReceipt = "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";

if (args.Length != 1 || args[0] != "--verify-private-binaries")
    throw new ArgumentException("Use --verify-private-binaries.");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS197: " + message);
}
static string Text(JsonElement node, string key) => node.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Missing string " + key);
static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string ClrName(Type type)
{
    if (type.IsGenericType)
    {
        var fullName = type.GetGenericTypeDefinition().FullName
            ?? throw new InvalidDataException("Missing CLR generic definition.");
        var tick = fullName.IndexOf('`', StringComparison.Ordinal);
        Require(tick > 0, "Malformed CLR generic definition.");
        return fullName[..tick] + "<" + string.Join(",", type.GetGenericArguments().Select(ClrName)) + ">";
    }
    return type.FullName ?? throw new InvalidDataException("Missing CLR type name.");
}
static bool IsSha256(string value) =>
    value.Length == 64 && value.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f');

var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) &&
    Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "An existing absolute NUGET_LOCAL private feed is required.");

using var compositionDoc = JsonDocument.Parse(File.ReadAllText(CompositionReceipt));
using var irDoc = JsonDocument.Parse(File.ReadAllText(IrReceipt));
using var quantitiesDoc = JsonDocument.Parse(File.ReadAllText(QuantitiesReceipt));
using var manifestDoc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var selected = manifestDoc.RootElement.GetProperty("capabilities").EnumerateArray().ToArray();
Require(selected.Length == 4 &&
    manifestDoc.RootElement.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
    "Four selected, zero broker candidates.");
var receipts = new[]
{
    compositionDoc.RootElement, irDoc.RootElement, quantitiesDoc.RootElement
};
var expectedIds = new[]
{
    "dw.tools.math.composition", "dw.tools.math.ir", "dw.quantities"
};
var expectedVersions = new[]
{
    "20260.20261107.1001", "20260.20261104.1001", "20260.20261102.1001"
};
var observedDlls = new Dictionary<string, byte[]>(StringComparer.Ordinal);
for (var i = 0; i < receipts.Length; i++)
{
    var receipt = receipts[i];
    Require(receipt.GetProperty("physically_published_package_verified").GetBoolean() &&
        !receipt.GetProperty("package_published_by_this_run").GetBoolean() &&
        !receipt.GetProperty("remote_registry_published").GetBoolean() &&
        !receipt.GetProperty("discovery_migration_qualified").GetBoolean(),
        "Qualified immutable private receipt only.");
    var observation = receipt.GetProperty("observation");
    var package = Text(observation, "packageId");
    var version = Text(observation, "packageVersion");
    var archiveHash = Text(observation, "archiveSha256");
    var dllHash = Text(observation, "packagedDllSha256");
    Require(package == expectedIds[i] && version == expectedVersions[i] &&
        IsSha256(archiveHash) && IsSha256(dllHash),
        "Unrecognized package identity or hash.");
    var archivePath = Path.Combine(feed!, package + "." + version + ".nupkg");
    Require(File.Exists(archivePath), "Qualified archive not present: " + package);
    var archiveBytes = File.ReadAllBytes(archivePath);
    Require(Hash(archiveBytes) == archiveHash, "Immutable archive SHA-256 mismatch.");
    using var archive = new ZipArchive(new MemoryStream(archiveBytes), ZipArchiveMode.Read);
    var dllEntries = archive.Entries.Where(entry =>
        entry.FullName.Equals("lib/net10.0/" + package + ".dll",
            StringComparison.OrdinalIgnoreCase)).ToArray();
    Require(dllEntries.Length == 1, "Exactly one qualified net10 DLL: " + package);
    using var input = dllEntries[0].Open();
    using var buffer = new MemoryStream();
    input.CopyTo(buffer);
    var binary = buffer.ToArray();
    Require(Hash(binary) == dllHash, "Packaged DLL SHA-256 mismatch: " + package);
    observedDlls.Add(package, binary);
}

var compositionDeps = receipts[0].GetProperty("observation")
    .GetProperty("exactDependencies").EnumerateArray().ToArray();
Require(compositionDeps.Length == 2 &&
    compositionDeps.Any(x => Text(x, "id") == expectedIds[1] &&
        Text(x, "version") == expectedVersions[1] &&
        Text(x, "packagedDllSha256") == Hash(observedDlls[expectedIds[1]])) &&
    compositionDeps.Any(x => Text(x, "id") == expectedIds[2] &&
        Text(x, "version") == expectedVersions[2] &&
        Text(x, "packagedDllSha256") == Hash(observedDlls[expectedIds[2]])),
    "Published Composition dependency closure.");
var context = new AssemblyLoadContext("rs197-discovery-private-binary", isCollectible: true);
var loaded = new Dictionary<string, Assembly>(StringComparer.Ordinal);
context.Resolving += (_, assemblyName) =>
    assemblyName.Name is { } name && loaded.TryGetValue(name, out var assembly)
        ? assembly : null;
foreach (var package in expectedIds.Reverse())
{
    using var dllStream = new MemoryStream(observedDlls[package]);
    var assembly = context.LoadFromStream(dllStream);
    Require(assembly.GetName().Name == package, "Loaded DLL identity changed.");
    loaded.Add(package, assembly);
}
var accepted = 0;
foreach (var capability in selected)
{
    var id = Text(capability, "capabilityId");
    var binding = capability.GetProperty("binding");
    var execution = capability.GetProperty("execution");
    Require(Text(capability, "exposure") == "AgentCallable" &&
        Text(execution, "authorization") == "runtime-only" &&
        !capability.TryGetProperty("brokerContract", out _),
        "No broker privileges permitted.");
    var assemblyName = Text(binding, "assembly");
    Require(loaded.TryGetValue(assemblyName, out var assembly),
        "Selected package absent from qualified private feed.");
    var owner = assembly!.GetType(Text(binding, "type"), throwOnError: false);
    Require(owner is not null, "Selected CLR type is missing from packaged DLL.");
    var candidates = owner!.GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(m => m.Name == Text(binding, "member") &&
            !m.IsGenericMethod &&
            m.GetParameters().Length == capability.GetProperty("parameters").GetArrayLength())
        .ToArray();
    var matches = candidates.Where(method =>
        method.GetParameters().Select(parameter => ClrName(parameter.ParameterType))
            .SequenceEqual(capability.GetProperty("parameters").EnumerateArray()
                .Select(parameter => Text(parameter, "type")), StringComparer.Ordinal) &&
        ClrName(method.ReturnType) == Text(capability.GetProperty("result"), "type"))
        .ToArray();
    Require(matches.Length == 1, "Selected CLR overload absent or ambiguous: " + id);
    var method = matches[0];
    var assemblyVersion = assembly.GetName().Version?.ToString();
    Require(assemblyVersion == Text(binding, "assemblyVersion"),
        "Qualified CLR assembly version drift.");
    var signature = string.Join("|", assemblyName, assemblyVersion, owner.FullName,
        method.Name, ClrName(method.ReturnType) + "(" +
        string.Join(",", method.GetParameters().Select(p => ClrName(p.ParameterType))) + ")");
    Require(signature == Text(binding, "canonicalSignature") &&
        Hash(Encoding.UTF8.GetBytes(signature)) == Text(binding, "fingerprint"),
        "Packaged CLR signature fingerprint differs from source manifest: " + id);
    accepted++;
}
Require(accepted == 4, "Four packaged CLR bindings required.");
Console.WriteLine("MATH-RS197-PRIVATE-CLR|archives=3|bindings=4|sourceFree=true|brokerReady=0|PASS");
