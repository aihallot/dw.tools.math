using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;

if (args.Length != 3)
    throw new ArgumentException("Expected the RS174 receipt, locked restore assets and fresh package cache.");
const string Standard = "dw.quantities.standard";
const string Expression = "dw.quantities.expression";
const string Quantities = "dw.quantities";
const string DependencyVersion = "20260.20261102.1001";
const string Feed = @"I:\nuget.local";
using var candidateDocument = JsonDocument.Parse(File.ReadAllBytes(args[0]));
var candidate = candidateDocument.RootElement.GetProperty("candidate");
using var assetDocument = JsonDocument.Parse(File.ReadAllBytes(args[1]));
var assets = assetDocument.RootElement;
var cache = Path.GetFullPath(args[2]);
var specs = new[]
{
    new Package(Standard, Text(candidate, "packageVersion"), 14269, Text(candidate, "candidateArchiveSha256"),
        Text(candidate, "candidateDllSha256"), typeof(ElectrochemicalUnitProfile).Assembly, [Expression, Quantities]),
    new Package(Expression, DependencyVersion, 13951, Text(candidate, "dependencyExpressionArchiveSha256"),
        Text(candidate, "dependencyExpressionDllSha256"), typeof(ExpressionParser).Assembly, [Quantities]),
    new Package(Quantities, DependencyVersion, 14747, Text(candidate, "dependencyQuantitiesArchiveSha256"),
        Text(candidate, "dependencyQuantitiesDllSha256"), typeof(ExactRational).Assembly, [])
};
var expectedKeys = specs.Select(s => s.Id + "/" + s.Version).ToHashSet(StringComparer.OrdinalIgnoreCase);
var libraries = assets.GetProperty("libraries").EnumerateObject().ToArray();
Need(expectedKeys.SetEquals(libraries.Select(x => x.Name)) &&
    libraries.All(x => Text(x.Value, "type") == "package"), "expected only the three Math NuGet packages");
var project = assets.GetProperty("project");
var direct = project.GetProperty("frameworks").GetProperty("net10.0").GetProperty("dependencies")
    .EnumerateObject().Single();
Need(direct.Name == Standard && Text(direct.Value, "target") == "Package", "one direct Standard PackageReference required");
var folders = assets.GetProperty("packageFolders").EnumerateObject().Select(x => x.Name).ToArray();
Need(folders.Length == 1 && SamePath(folders[0], cache), "locked restore used an unexpected package folder");
var sources = project.GetProperty("restore").GetProperty("sources").EnumerateObject().Select(x => x.Name).ToArray();
var sdkPacks = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "library-packs");
Need(sources.Any(x => SamePath(x, Feed)) && sources.All(x => SamePath(x, Feed) || SamePath(x, sdkPacks)),
    "restore source lies outside the published feed and SDK library-packs");

var observations = new List<object>();
foreach (var spec in specs)
{
    var leaf = spec.Id + "." + spec.Version + ".nupkg";
    var published = Path.Combine(Feed, leaf);
    var restored = Path.Combine(cache, spec.Id, spec.Version);
    var cached = Path.Combine(restored, leaf);
    var physicalHash = HashFile(published);
    var cachedHash = HashFile(cached);
    Need(physicalHash == spec.ArchiveSha && cachedHash == physicalHash &&
        new FileInfo(published).Length == spec.Bytes, "physical or restored archive mismatch: " + leaf);
    using var metadata = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(restored, ".nupkg.metadata")));
    var source = Text(metadata.RootElement, "source");
    var sourcePath = Uri.TryCreate(source, UriKind.Absolute, out var uri) && uri.IsFile ? uri.LocalPath : source;
    Need(SamePath(sourcePath, Feed), "NuGet did not restore the archive from the published feed: " + leaf);
    using var zip = ZipFile.OpenRead(published);
    var inventory = zip.Entries.OrderBy(e => e.FullName, StringComparer.Ordinal).Select(e =>
    {
        using var stream = e.Open();
        return new Entry(e.FullName, e.Length, e.CompressedLength, Convert.ToHexStringLower(SHA256.HashData(stream)));
    }).ToArray();
    Need(inventory.Length is > 0 and <= 48 &&
        inventory.Select(e => e.Path).Distinct(StringComparer.Ordinal).Count() == inventory.Length,
        "unexpected ZIP inventory: " + leaf);
    if (spec.Id == Standard)
    {
        var original = candidate.GetProperty("packagedFileInventory").EnumerateArray().Select(e =>
            new Entry(Text(e, "path"), e.GetProperty("bytes").GetInt64(), e.GetProperty("compressedBytes").GetInt64(),
                Text(e, "sha256"))).OrderBy(e => e.Path, StringComparer.Ordinal);
        Need(original.SequenceEqual(inventory), "the physical Standard ZIP inventory differs from RS174");
    }
    var manifest = zip.GetEntry(spec.Id + ".nuspec") ?? throw new InvalidDataException("missing package nuspec");
    using var manifestStream = manifest.Open();
    var xml = XDocument.Load(manifestStream);
    var owner = xml.Descendants().Single(e => e.Name.LocalName == "metadata");
    Need(owner.Elements().Single(e => e.Name.LocalName == "id").Value == spec.Id &&
        owner.Elements().Single(e => e.Name.LocalName == "version").Value == spec.Version, "nuspec identity mismatch");
    var dependencies = owner.Descendants().Where(e => e.Name.LocalName == "dependency")
        .Select(e => new { id = (string?)e.Attribute("id"), version = (string?)e.Attribute("version") })
        .OrderBy(d => d.id, StringComparer.Ordinal).ToArray();
    Need(dependencies.Select(d => d.id).SequenceEqual(spec.Dependencies.Order(StringComparer.Ordinal)) &&
        dependencies.All(d => d.version == "[" + DependencyVersion + "]"), "exact nuspec dependencies differ: " + leaf);
    var dll = inventory.Single(e => e.Path == "lib/net10.0/" + spec.Id + ".dll");
    var loaded = spec.Assembly.Location;
    var loadedHash = HashFile(loaded);
    Need(SamePath(loaded, Path.Combine(AppContext.BaseDirectory, spec.Id + ".dll")) &&
        spec.Assembly.GetName().Name == spec.Id && spec.Assembly.GetName().Version?.ToString() == "0.2.0.0" &&
        loadedHash == dll.Sha256 && loadedHash == spec.DllSha, "loaded DLL differs from the published DLL: " + spec.Id);
    observations.Add(new
    {
        id = spec.Id, version = spec.Version, archivePath = published, bytes = new FileInfo(published).Length,
        archiveSha256 = physicalHash, cachedArchiveSha256 = cachedHash, nugetSource = source, dependencies,
        inventory, packagedDllSha256 = dll.Sha256, loadedDllSha256 = loadedHash,
        loadedPath = loaded, clrIdentity = spec.Assembly.FullName
    });
}
var ids = ElectrochemicalUnitProfile.All.Select(u => u.Id).Order(StringComparer.Ordinal).ToArray();
string[] required = ["si.ampere", "si.mole", "si.mole-per-cubic-metre", "si.ohm", "si.volt", "si.watt"];
Need(ids.SequenceEqual(required) && InheritedStandardUnitCatalog.All.Length == 58 && StandardUnitCatalog.All.Length == 14,
    "published electrochemical profile or historical catalogues changed");
// The public version fields are constants. Read the loaded assembly metadata, avoiding compiler-inlined observations.
var profileId = Constant(typeof(ElectrochemicalUnitProfile), "ProfileId");
var profileVersion = Constant(typeof(ElectrochemicalUnitProfile), "ProfileVersion");
var inheritedVersion = Constant(typeof(InheritedStandardUnitCatalog), "Version");
var standardVersion = Constant(typeof(StandardUnitCatalog), "CatalogVersion");
Need(profileId == "math.si-electrochemical" && profileVersion == "2026-11-electrochemical-exact-v1" &&
    inheritedVersion == "0.2.0" && standardVersion == "2026-10-exact-v1", "compiled catalogue identity changed");
Console.WriteLine(JsonSerializer.Serialize(new
{
    directPackage = direct.Name, feed = Feed, sources, packageFolders = folders, packages = observations,
    profileId, profileVersion, unitIds = ids, inheritedVersion, inheritedUnits = InheritedStandardUnitCatalog.All.Length,
    standardVersion, standardUnits = StandardUnitCatalog.All.Length
}, JsonSerializerOptions.Web));

static string Constant(Type type, string name) =>
    type.GetField(name, BindingFlags.Public | BindingFlags.Static)?.GetRawConstantValue() as string ??
        throw new InvalidDataException("missing compiled version field: " + type.Name + "." + name);
static string Text(JsonElement value, string key) => value.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing string: " + key);
static string HashFile(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static bool SamePath(string a, string b) =>
    Path.IsPathFullyQualified(a) && Path.IsPathFullyQualified(b) &&
    string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
        Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)), StringComparison.OrdinalIgnoreCase);
static void Need(bool valid, string why)
{
    if (!valid) throw new InvalidDataException("RS177 readback: " + why);
}
internal sealed record Package(string Id, string Version, long Bytes, string ArchiveSha, string DllSha,
    Assembly Assembly, string[] Dependencies);
internal sealed record Entry(string Path, long Bytes, long CompressedBytes, string Sha256);
