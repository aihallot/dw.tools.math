#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string SkillPath = ".agents/skills/dw-tools-math/SKILL.md";
const string ReferencePath = ".agents/skills/dw-tools-math/references/selected-capabilities.md";
const string VersionPath = ".agents/skills/dw-tools-math/references/skill-version.json";

using var manifestDoc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
using var versionDoc = JsonDocument.Parse(File.ReadAllText(VersionPath));
var manifest = manifestDoc.RootElement;
var version = versionDoc.RootElement;
var skill = File.ReadAllText(SkillPath).Replace("\r\n", "\n", StringComparison.Ordinal);
var reference = File.ReadAllText(ReferencePath).Replace("\r\n", "\n", StringComparison.Ordinal);
string Read(JsonElement node, string field) => node.GetProperty(field).GetString()
    ?? throw new InvalidDataException("Missing field " + field);
void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS195: " + reason);
}
Require(manifest.GetProperty("schemaVersion").GetInt32() == 2 &&
    Read(manifest, "owner") == "math" &&
    Read(manifest, "protocolId") == "dw.discovery" &&
    Read(version, "protocolVersion") == Read(manifest, "protocolVersion") &&
    version.GetProperty("protocolSchemaVersion").GetInt32() == 2,
    "Portable protocol identity and version mismatch.");
Require(Read(version, "owner") == Read(manifest, "owner") &&
    Read(version, "protocolId") == Read(manifest, "protocolId") &&
    Read(version, "libraryRelease") == Read(manifest, "libraryRelease") &&
    Read(version, "manifestGeneratorVersion") == Read(manifest, "generatorVersion") &&
    Read(version, "skillBundleVersion") == Read(manifest, "skillBundleVersion") &&
    Read(version, "sourceManifest") == ManifestPath,
    "Bundle version metadata and manifest provenance diverged.");
Require(manifest.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    version.GetProperty("brokerReadyCount").GetInt32() == 0 &&
    Read(version, "authorization") == "runtime-only",
    "Unqualified broker or authorization claim.");
Require(skill.Contains("Exposure: `AgentCallable` only.", StringComparison.Ordinal) &&
    skill.Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal) &&
    reference.Contains("No item is BrokerReady; invocation must be authorized independently by the runtime.", StringComparison.Ordinal),
    "Missing explicit human-readable authorization boundary.");
Require(skill.Contains("Skill bundle: `" + Read(version, "skillBundleVersion") + "`.",
    StringComparison.Ordinal), "Skill heading version drift.");

var capabilities = manifest.GetProperty("capabilities").EnumerateArray().ToArray();
Require(capabilities.Length == 4 &&
    version.GetProperty("capabilityCount").GetInt32() == capabilities.Length,
    "Exact selected capability count.");
var skillHeadings = Regex.Matches(skill, @"(?m)^### `(math\.[a-z0-9.-]+)`$");
var referenceHeadings = Regex.Matches(reference, @"(?m)^## `(math\.[a-z0-9.-]+)`$");
Require(skillHeadings.Count == capabilities.Length &&
    referenceHeadings.Count == capabilities.Length,
    "Unselected or missing capability headings.");
var expectedPackages = new SortedDictionary<string, string>(StringComparer.Ordinal);
for (var i = 0; i < capabilities.Length; i++)
{
    var item = capabilities[i];
    var id = Read(item, "capabilityId");
    var binding = item.GetProperty("binding");
    var execution = item.GetProperty("execution");
    var semantics = item.GetProperty("semantics");
    Require(skillHeadings[i].Groups[1].Value == id &&
        referenceHeadings[i].Groups[1].Value == id,
        "Cross-artifact selection/order drift for " + id);
    Require(Read(item, "exposure") == "AgentCallable" &&
        Read(execution, "authorization") == "runtime-only" &&
        !item.TryGetProperty("brokerContract", out _),
        "Selected capability authorization changed.");
    var signature = Read(binding, "canonicalSignature");
    var fingerprint = Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(signature))).ToLowerInvariant();
    Require(fingerprint == Read(binding, "fingerprint"), "CLR fingerprint is not SHA-256.");
    Require(skill.Contains("- Signature: `" + signature + "`.", StringComparison.Ordinal) &&
        reference.Contains("- Signature SHA-256: `" + fingerprint + "`.",
            StringComparison.Ordinal) &&
        skill.Contains("- Limits: " + Read(semantics, "limits") + ".", StringComparison.Ordinal) &&
        reference.Contains("- Numerical limits: " + Read(semantics, "limits"),
            StringComparison.Ordinal),
        "Binding or numeric limits differ between the manifest, skill and reference.");
    var package = Read(item, "package");
    var packageVersion = Read(item, "packageVersion");
    if (expectedPackages.TryGetValue(package, out var previous))
        Require(previous == packageVersion, "Manifest package version collision.");
    else
        expectedPackages.Add(package, packageVersion);
}
var packageVersions = version.GetProperty("packages").EnumerateArray().ToArray();
Require(packageVersions.Length == expectedPackages.Count &&
    expectedPackages.Count == 2, "Wrong selected package set.");
for (var i = 0; i < packageVersions.Length; i++)
{
    var expected = expectedPackages.ElementAt(i);
    Require(Read(packageVersions[i], "id") == expected.Key &&
        Read(packageVersions[i], "version") == expected.Value,
        "Source manifest and version-file packages differ.");
}
Console.WriteLine("MATH-RS195-BUNDLE-CONSISTENCY|manifest=1|skill=1|reference=1|versions=1|capabilities=4|packages=2|brokerReady=0|PASS");
