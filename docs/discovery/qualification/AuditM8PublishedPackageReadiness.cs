#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string VersionsPath = ".agents/skills/dw-tools-math/references/skill-version.json";
const string CompositionReceipt = "docs/distribution/nuget-federation-f34-composition-published-qualified.json";
const string IrReceipt = "docs/distribution/nuget-federation-f13-ir-published-qualified.json";
const string QuantitiesReceipt = "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";

using var manifestDoc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
using var versionsDoc = JsonDocument.Parse(File.ReadAllText(VersionsPath));
using var compositionDoc = JsonDocument.Parse(File.ReadAllText(CompositionReceipt));
using var irDoc = JsonDocument.Parse(File.ReadAllText(IrReceipt));
using var quantitiesDoc = JsonDocument.Parse(File.ReadAllText(QuantitiesReceipt));
var manifest = manifestDoc.RootElement;
var versions = versionsDoc.RootElement;
var composition = compositionDoc.RootElement;
var ir = irDoc.RootElement;
var quantities = quantitiesDoc.RootElement;

static string Text(JsonElement obj, string key) => obj.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Missing " + key);
static void Require(bool value, string reason)
{
    if (!value) throw new InvalidDataException("RS196: " + reason);
}
static void CheckPublished(JsonElement receipt, string packageId, string packageVersion,
    string archiveSha, string dllSha)
{
    Require(receipt.GetProperty("physically_published_package_verified").GetBoolean() &&
        !receipt.GetProperty("package_published_by_this_run").GetBoolean() &&
        !receipt.GetProperty("remote_registry_published").GetBoolean() &&
        !receipt.GetProperty("discovery_migration_qualified").GetBoolean(),
        "Read-only private publication and Discovery boundary.");
    var observation = receipt.GetProperty("observation");
    Require(Text(observation, "packageId") == packageId &&
        Text(observation, "packageVersion") == packageVersion &&
        Text(observation, "archiveSha256") == archiveSha &&
        Text(observation, "packagedDllSha256") == dllSha &&
        Text(observation, "loadedDllSha256") == dllSha,
        "Pinned private archive or loaded binary identity.");
}
CheckPublished(composition, "dw.tools.math.composition", "20260.20261107.1001",
    "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f",
    "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252");
CheckPublished(ir, "dw.tools.math.ir", "20260.20261104.1001",
    "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
    "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af");
CheckPublished(quantities, "dw.quantities", "20260.20261102.1001",
    "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
    "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4");

var compositionObs = composition.GetProperty("observation");
var irObs = ir.GetProperty("observation");
var dep = compositionObs.GetProperty("exactDependencies").EnumerateArray().ToArray();
Require(dep.Length == 2 &&
    dep.Any(d => Text(d, "id") == "dw.tools.math.ir" &&
        Text(d, "version") == "20260.20261104.1001" &&
        Text(d, "packagedDllSha256") ==
            Text(irObs, "packagedDllSha256")) &&
    dep.Any(d => Text(d, "id") == "dw.quantities" &&
        Text(d, "version") == "20260.20261102.1001" &&
        Text(d, "packagedDllSha256") ==
            Text(quantities.GetProperty("observation"), "packagedDllSha256")),
    "Composition published binary dependency closure.");
Require(manifest.GetProperty("schemaVersion").GetInt32() == 2 &&
    Text(manifest, "owner") == "math" &&
    manifest.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
    versions.GetProperty("brokerReadyCount").GetInt32() == 0,
    "No broker promotion.");
var selected = manifest.GetProperty("capabilities").EnumerateArray().ToArray();
Require(selected.Length == 4 && versions.GetProperty("capabilityCount").GetInt32() == 4,
    "Four explicitly selected capabilities.");
var packages = selected.ToDictionary(x => Text(x, "capabilityId"),
    x => (Id: Text(x, "package"), Version: Text(x, "packageVersion")),
    StringComparer.Ordinal);
Require(packages.Values.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() == 2 &&
    packages.Values.All(x => x.Version == "0.3.0-preview.1") &&
    packages.Values.Any(x => x.Id == "dw.tools.math.composition") &&
    packages.Values.Any(x => x.Id == "dw.tools.math.ir"),
    "Qualified source-version catalogue.");
Require(!packages.Values.Any(x =>
    x.Version == "20260.20261107.1001" ||
    x.Version == "20260.20261104.1001"),
    "Source manifest must not claim the installed package versions.");

Console.WriteLine("MATH-RS196-DISCOVERY-PACKAGE-AUDIT|sourceCapabilities=4|publishedDependencies=3|sourceVersion=0.3.0-preview.1|installedDiscoveryBindings=unverified|brokerReady=0|PASS");
