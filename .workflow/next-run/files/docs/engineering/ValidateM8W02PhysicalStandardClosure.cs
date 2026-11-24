#:property NuGetAudit=false
#:property PublishAot=false

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Profile = "docs/engineering/m8-w02-c01-published-standard-closure-rs322.json";
const string OriginalAuthority = "docs/engineering/ProbeM8Net11PublishedStandardLockedRestore.cs";
using var document = JsonDocument.Parse(File.ReadAllText(Profile));
var root = document.RootElement;
Need(root.GetProperty("schemaVersion").GetInt32() == 1 &&
    root.GetProperty("id").GetString() == "M8-W02-C01-T2-G-RS322" &&
    root.GetProperty("status").GetString() ==
        "bounded-three-immutable-archives-source-graph-and-prior-net11-lock-evidence" &&
    root.GetProperty("archiveTargetFramework").GetString() == "net10.0" &&
    root.GetProperty("observedSdk").GetString() == "11.0.100" &&
    root.GetProperty("privateFeedEnvironmentVariable").GetString() == "NUGET_LOCAL",
    "The bounded published archive profile identity changed.");

using var global = JsonDocument.Parse(File.ReadAllText("global.json"));
var sdk = global.RootElement.GetProperty("sdk");
var build = XDocument.Load("Directory.Build.props");
Need(sdk.GetProperty("version").GetString() == "10.0.401" &&
    sdk.GetProperty("rollForward").GetString() == "latestPatch" &&
    Element(build, "TargetFramework") == "net10.0" &&
    Element(build, "LangVersion") == "14.0",
    "Production Math SDK, TFM or language baseline was migrated without approval.");

var knownDirectDependencies = new Dictionary<string, string[]>(StringComparer.Ordinal)
{
    ["dw.quantities"] = [],
    ["dw.quantities.expression"] = ["dw.quantities"],
    ["dw.quantities.standard"] = ["dw.quantities", "dw.quantities.expression"]
};
var standardId = "dw.quantities.standard";
var rows = root.GetProperty("packages").EnumerateArray().ToArray();
Need(rows.Length == 3 &&
    rows.Select(x => x.GetProperty("packageId").GetString()!)
        .ToHashSet(StringComparer.Ordinal).SetEquals(knownDirectDependencies.Keys) &&
    rows.Count(x => x.GetProperty("topology").GetString() == "Direct") == 1 &&
    rows.Single(x => x.GetProperty("topology").GetString() == "Direct")
        .GetProperty("packageId").GetString() == standardId,
    "The three-package physically released Standard closure drifted.");

var existingVerifier = File.ReadAllText(OriginalAuthority);
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must resolve to an existing absolute private feed.");

foreach (var row in rows)
{
    var id = row.GetProperty("packageId").GetString()!;
    var version = row.GetProperty("version").GetString()!;
    var bytes = row.GetProperty("archiveBytes").GetInt64();
    var archiveHash = row.GetProperty("archiveSha256").GetString()!;
    var binaryHash = row.GetProperty("binarySha256").GetString()!;
    Need(version.StartsWith("20260.", StringComparison.Ordinal) &&
        bytes > 0 && archiveHash.Length == 64 && binaryHash.Length == 64,
        "A published version or physical hash pin is incomplete: " + id);
    Need(existingVerifier.Contains("\"" + id + "\"", StringComparison.Ordinal) &&
        existingVerifier.Contains("\"" + version + "\"", StringComparison.Ordinal) &&
        existingVerifier.Contains("\"" + archiveHash + "\"", StringComparison.Ordinal) &&
        existingVerifier.Contains("\"" + binaryHash + "\"", StringComparison.Ordinal) &&
        existingVerifier.Contains("Bytes: " + bytes + "L", StringComparison.Ordinal),
        "The archive/DLL fingerprint does not match the qualified RS254 authority: " + id);

    var source = Path.Combine("src", "projects", id, id + ".csproj");
    Need(File.Exists(source), "The corresponding Math source project is missing: " + id);
    var project = XDocument.Load(source);
    Need(Element(project, "PackageId") == id &&
        Element(project, "IsPackable") == "true" &&
        Element(project, "RestorePackagesWithLockFile") == "true",
        "Qualified source package identity or locked restore setting changed: " + id);
    var observedRefs = project.Descendants()
        .Where(x => x.Name.LocalName == "ProjectReference")
        .Select(x => x.Attribute("Include")?.Value ??
            throw new InvalidDataException("RS322: Missing ProjectReference Include: " + id))
        .Select(x => Path.GetFileNameWithoutExtension(x.Replace('\\', '/')))
        .ToArray();
    var profileRefs = row.GetProperty("directSourceDeps").EnumerateArray()
        .Select(x => x.GetString()!).ToArray();
    Need(SameSet(observedRefs, knownDirectDependencies[id]) &&
        SameSet(profileRefs, knownDirectDependencies[id]) &&
        (id == standardId ? "Direct" : "Transitive") ==
            row.GetProperty("topology").GetString(),
        "The Standard/source dependency topology or archive profile differs: " + id);

    var archive = Path.Combine(feed!, id + "." + version + ".nupkg");
    Need(File.Exists(archive) &&
        (File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0 &&
        new FileInfo(archive).Length == bytes &&
        Hash(archive) == archiveHash,
        "The immutable NUGET_LOCAL archive is missing, redirected or altered: " + id);
    using (var zip = ZipFile.OpenRead(archive))
    {
        var dll = zip.Entries.Single(x =>
            x.FullName == "lib/net10.0/" + id + ".dll");
        using var stream = dll.Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(stream)) == binaryHash,
            "The physically packaged net10 DLL fingerprint changed: " + id);
    }
    Need(Hash(archive) == archiveHash,
        "A published private archive changed during the read-only audit: " + id);
    Console.WriteLine("MATH-RS322-PUBLISHED-STANDARD|id=" + id +
        "|version=" + version + "|physicalArchive=matched|" +
        "net10Dll=matched|sourceReferences=" + observedRefs.Length +
        "|lockTopology=" + row.GetProperty("topology").GetString() + "|PASS");
}

var historical = root.GetProperty("historicalEvidence").EnumerateArray().ToArray();
Need(historical.Length == 2 &&
    historical.Select(x => x.GetProperty("runId").GetString())
        .OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "RS253", "RS254" }, StringComparer.Ordinal),
    "Bounded source/published locked restore history changed.");
foreach (var item in historical)
{
    var id = item.GetProperty("runId").GetString()!;
    var file = item.GetProperty("report").GetString()!;
    var validation = item.GetProperty("validation").GetString()!;
    Need(file == ".aura/workflow/reports/" + id + "/attempt-001.json",
        "Historical run report path differs from the qualified scope: " + id);
    using var report = JsonDocument.Parse(File.ReadAllText(file));
    var q = report.RootElement;
    var prior = q.GetProperty("priorInvocationFailures");
    Need(q.GetProperty("runId").GetString() == id &&
        q.GetProperty("outcome").GetString() == "succeeded" &&
        q.GetProperty("attempt").GetInt32() == 1 &&
        q.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
        prior.GetProperty("count").GetInt32() == (id == "RS254" ? 1 : 0) &&
        prior.GetProperty("preparationFailureCount").GetInt32() == (id == "RS254" ? 1 : 0) &&
        prior.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
        prior.GetProperty("firstPassSucceeded").GetBoolean() == (id == "RS253") &&
        q.GetProperty("contribution").GetProperty("observed")
            .GetProperty("validations").EnumerateArray().Any(x =>
                x.GetProperty("id").GetString() == validation) &&
        validation == (id == "RS253"
            ? "m8-net11-distribution-locked-restore"
            : "m8-published-standard-net11-locked-restore"),
        "Physical/history report or its true first-pass lineage differs: " + id);
}

var limitations = root.GetProperty("limitations").EnumerateArray()
    .Select(x => x.GetString()!).ToArray();
Need(limitations.Length == 5 &&
    limitations.Any(x => x.Contains("other ten", StringComparison.Ordinal)) &&
    limitations.Any(x => x.Contains("no current net11 locked restore", StringComparison.Ordinal)) &&
    limitations.Any(x => x.Contains("immutable", StringComparison.Ordinal)),
    "Bounded historical vs current physical acceptance limit was lost.");

Console.WriteLine("MATH-RS322-PHYSICAL-STANDARD-CLOSURE|archives=3|" +
    "dllHashes=3|publishedDirect=1|publishedTransitive=2|" +
    "sourceProjects=3|historicalReports=2|rs254FirstPass=false|" +
    "lockedRestore=newExecutionNo|otherTenPackages=unqualified|" +
    "productionSdk=10.0.401|guidanceUpgrade=false|PASS");

static string? Element(XDocument doc, string name) =>
    doc.Descendants().FirstOrDefault(x => x.Name.LocalName == name)?.Value.Trim();
static bool SameSet(string[] left, string[] right) =>
    left.Length == right.Length &&
    left.Distinct(StringComparer.Ordinal).Count() == left.Length &&
    left.ToHashSet(StringComparer.Ordinal).SetEquals(right);
static string Hash(string file)
{
    using var stream = File.OpenRead(file);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS322: " + reason);
}
