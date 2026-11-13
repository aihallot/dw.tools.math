#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;
using System.Text.Json.Nodes;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string VersionPath = ".agents/skills/dw-tools-math/references/skill-version.json";

using var manifestDoc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var manifest = manifestDoc.RootElement;
var original = JsonNode.Parse(File.ReadAllText(VersionPath))?.AsObject()
    ?? throw new InvalidDataException("Missing generated skill version object.");
Validate(original);
var refusals = 0;
Reject("schema", x => x["schemaVersion"] = 2);
Reject("owner", x => x["owner"] = "decision");
Reject("protocol", x => x["protocolVersion"] = "1.0.0");
Reject("generator", x => x["manifestGeneratorVersion"] = "9.9.9");
Reject("bundle", x => x["skillBundleVersion"] = "9.9.9");
Reject("count", x => x["capabilityCount"] = 5);
Reject("broker", x => x["brokerReadyCount"] = 1);
Reject("authorization", x => x["authorization"] = "granted");
Reject("package-version", x => x["packages"]![0]!["version"] = "9.9.9");
Reject("duplicate-package", x => x["packages"]!.AsArray()[1] =
    x["packages"]![0]!.DeepClone());
Require(refusals == 10, "Incomplete negative-case matrix.");
Console.WriteLine("MATH-RS194-SKILL-VERSION|positive=1|negative=10|packages=2|brokerReady=0|PASS");

void Reject(string name, Action<JsonObject> change)
{
    var candidate = original.DeepClone().AsObject();
    change(candidate);
    try { Validate(candidate); }
    catch (InvalidDataException)
    {
        refusals++;
        Console.WriteLine("REFUSAL " + name + " PASS");
        return;
    }
    throw new InvalidDataException("RS194: mutation accepted: " + name);
}

void Validate(JsonObject candidate)
{
    static string String(JsonObject obj, string name) =>
        obj[name]?.GetValue<string>() is { Length: > 0 } value
        ? value : throw new InvalidDataException("Missing " + name);
    static int Number(JsonObject obj, string name) =>
        obj[name]?.GetValue<int>() ?? throw new InvalidDataException("Missing " + name);
    string FromManifest(string name) => manifest.GetProperty(name).GetString()
        ?? throw new InvalidDataException("Manifest missing " + name);

    Require(Number(candidate, "schemaVersion") == 1 &&
        String(candidate, "owner") == FromManifest("owner") &&
        String(candidate, "protocolId") == FromManifest("protocolId") &&
        String(candidate, "protocolVersion") == FromManifest("protocolVersion") &&
        Number(candidate, "protocolSchemaVersion") ==
            manifest.GetProperty("schemaVersion").GetInt32(),
        "Protocol identity drift.");
    Require(String(candidate, "libraryRelease") == FromManifest("libraryRelease") &&
        String(candidate, "manifestGeneratorVersion") == FromManifest("generatorVersion") &&
        String(candidate, "skillBundleVersion") == FromManifest("skillBundleVersion") &&
        String(candidate, "sourceManifest") == ManifestPath,
        "Version or provenance drift.");
    var caps = manifest.GetProperty("capabilities").EnumerateArray().ToArray();
    Require(caps.Length == 4 && Number(candidate, "capabilityCount") == caps.Length &&
        manifest.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        Number(candidate, "brokerReadyCount") == 0 &&
        String(candidate, "authorization") == "runtime-only",
        "Unqualified capabilities or runtime grant.");
    var expectedPackages = new SortedDictionary<string, string>(StringComparer.Ordinal);
    foreach (var cap in caps)
    {
        Require(cap.GetProperty("exposure").GetString() == "AgentCallable" &&
            cap.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only",
            "Unqualified selected exposure.");
        var id = cap.GetProperty("package").GetString()
            ?? throw new InvalidDataException("Missing package.");
        var version = cap.GetProperty("packageVersion").GetString()
            ?? throw new InvalidDataException("Missing version.");
        if (expectedPackages.TryGetValue(id, out var current))
            Require(current == version, "Source package conflict.");
        else
            expectedPackages.Add(id, version);
    }
    var actualPackages = candidate["packages"] as JsonArray
        ?? throw new InvalidDataException("Missing packages.");
    Require(actualPackages.Count == expectedPackages.Count && actualPackages.Count == 2,
        "Package cardinality.");
    var index = 0;
    foreach (var pair in expectedPackages)
    {
        var entry = actualPackages[index++] as JsonObject
            ?? throw new InvalidDataException("Malformed package.");
        Require(String(entry, "id") == pair.Key &&
            String(entry, "version") == pair.Value,
            "Package identity, order or version drift.");
    }
}
static void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS194: " + reason);
}
