#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 2 || args[0] != "--bundle")
    throw new ArgumentException("Expected --bundle <skill-directory>.");
var directory = Path.GetFullPath(args[1]);
string BundleFile(string name) => Path.Combine(directory, name.Replace('/', Path.DirectorySeparatorChar));
string Read(string name) => File.ReadAllText(BundleFile(name)).Replace("\r\n", "\n", StringComparison.Ordinal);
const string Manifest = "references/manifest.json";
const string Version = "references/skill-version.json";
const string Reference = "references/selected-capabilities.md";
const string Navigation = "references/index.md";
const string Examples = "references/examples.md";
var manifest = Read(Manifest);
var version = Read(Version);
var reference = Read(Reference);
var navigation = Read(Navigation);
var examples = Read(Examples);
var skill = Read("SKILL.md");
Validate(manifest, version, reference, navigation, examples, skill);
var rejected = 0;
Reject("broker-promotion", manifest.Replace("\"exposure\": \"AgentCallable\"",
    "\"exposure\": \"BrokerReady\"", StringComparison.Ordinal), version, reference, navigation, examples, skill);
Reject("runtime-authority", manifest.Replace("\"authorization\": \"runtime-only\"",
    "\"authorization\": \"granted\"", StringComparison.Ordinal), version, reference, navigation, examples, skill);
Reject("removed-example-link", manifest, version, reference,
    navigation.Replace("(examples.md)", "(missing.md)", StringComparison.Ordinal), examples, skill);
Reject("selected-identity-drift", manifest.Replace("math.ir.hash.semantic",
    "math.ir.hash.algebraic", StringComparison.Ordinal), version, reference, navigation, examples, skill);
Require(rejected == 4, "Expected exactly four refusals.");
Console.WriteLine("MATH-RS211-ISOLATED-SKILL-METADATA|selected=4|packages=2|refusals=4|brokerReady=0|sourceManifestRead=false|PASS");

void Reject(string label, string m, string v, string referenceText, string navigationText, string examplesText, string skillText)
{
    Require(m != manifest || v != version || referenceText != reference ||
        navigationText != navigation || examplesText != examples || skillText != skill,
        "Missing mutation " + label);
    try { Validate(m, v, referenceText, navigationText, examplesText, skillText); }
    catch (InvalidDataException)
    {
        rejected++;
        Console.WriteLine("REFUSAL " + label + " PASS");
        return;
    }
    throw new InvalidDataException("RS211 accepted mutation " + label);
}

void Validate(string m, string v, string referenceText, string navigationText, string examplesText, string skillText)
{
    using var md = JsonDocument.Parse(m);
    using var vd = JsonDocument.Parse(v);
    var root = md.RootElement;
    var ver = vd.RootElement;
    Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
        root.GetProperty("owner").GetString() == "math" &&
        root.GetProperty("protocolId").GetString() == "dw.discovery" &&
        root.GetProperty("protocolVersion").GetString() == "1.0.0-draft.1" &&
        root.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
        "Manifest identity and broker exclusion.");
    Require(ver.GetProperty("protocolSchemaVersion").GetInt32() == 2 &&
        ver.GetProperty("owner").GetString() == root.GetProperty("owner").GetString() &&
        ver.GetProperty("protocolId").GetString() == root.GetProperty("protocolId").GetString() &&
        ver.GetProperty("protocolVersion").GetString() == root.GetProperty("protocolVersion").GetString() &&
        ver.GetProperty("libraryRelease").GetString() == root.GetProperty("libraryRelease").GetString() &&
        ver.GetProperty("skillBundleVersion").GetString() == root.GetProperty("skillBundleVersion").GetString() &&
        ver.GetProperty("brokerReadyCount").GetInt32() == 0 &&
        ver.GetProperty("authorization").GetString() == "runtime-only",
        "Bundle version or authorization.");
    var entries = root.GetProperty("capabilities").EnumerateArray().ToArray();
    string[] ids = [
        "math.quantity.exact-pipeline.evaluate",
        "math.replay.try-evaluate",
        "math.ir.canonical.encode",
        "math.ir.hash.semantic"
    ];
    Require(entries.Length == ids.Length &&
        ver.GetProperty("capabilityCount").GetInt32() == entries.Length,
        "Selected capability count.");
    var packages = new SortedDictionary<string, string>(StringComparer.Ordinal);
    for (var i = 0; i < entries.Length; i++)
    {
        var item = entries[i];
        var id = item.GetProperty("capabilityId").GetString() ?? "";
        var execution = item.GetProperty("execution");
        var binding = item.GetProperty("binding");
        var signature = binding.GetProperty("canonicalSignature").GetString() ?? "";
        var fingerprint = binding.GetProperty("fingerprint").GetString() ?? "";
        var computed = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(signature))).ToLowerInvariant();
        var package = item.GetProperty("package").GetString() ?? "";
        var packageVersion = item.GetProperty("packageVersion").GetString() ?? "";
        Require(id == ids[i] &&
            item.GetProperty("exposure").GetString() == "AgentCallable" &&
            execution.GetProperty("authorization").GetString() == "runtime-only" &&
            !item.TryGetProperty("brokerContract", out _) &&
            computed == fingerprint &&
            skillText.Contains("### `" + id + "`", StringComparison.Ordinal) &&
            skillText.Contains("- Fingerprint (SHA-256): `" + fingerprint + "`", StringComparison.Ordinal) &&
            referenceText.Contains("## `" + id + "`", StringComparison.Ordinal) &&
            referenceText.Contains("- Signature SHA-256: `" + fingerprint + "`", StringComparison.Ordinal) &&
            navigationText.Contains("`" + id + "`", StringComparison.Ordinal) &&
            examplesText.Contains("| `" + id + "` |", StringComparison.Ordinal),
            "Selected isolated capability drift: " + id);
        if (packages.TryGetValue(package, out var prior))
            Require(prior == packageVersion, "Package version collision.");
        else packages.Add(package, packageVersion);
    }
    var declared = ver.GetProperty("packages").EnumerateArray().ToArray();
    Require(packages.Count == 2 && declared.Length == 2, "Two selected Math packages.");
    for (var i = 0; i < declared.Length; i++)
    {
        var expected = packages.ElementAt(i);
        Require(declared[i].GetProperty("id").GetString() == expected.Key &&
            declared[i].GetProperty("version").GetString() == expected.Value,
            "Selected package identity.");
    }
    Require(navigationText.Contains("(examples.md)", StringComparison.Ordinal) &&
        navigationText.Contains("(selected-capabilities.md)", StringComparison.Ordinal) &&
        navigationText.Contains("(skill-version.json)", StringComparison.Ordinal) &&
        examplesText.Contains("not invocation authorization or broker routing", StringComparison.Ordinal) &&
        examplesText.Contains("not** evidence that a fresh external consumer installed", StringComparison.Ordinal) &&
        examplesText.Contains("No item is `BrokerReady`", StringComparison.Ordinal) &&
        referenceText.Contains("No item is BrokerReady", StringComparison.Ordinal) &&
        skillText.Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal),
        "Bundle navigation and non-authorization boundaries.");
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS211: " + message);
}
