#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string MatrixPath = "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json";
const string ProposalPath = "docs/planning/proposals/net11-numerics-impact-matrix.json";
using var matrix = JsonDocument.Parse(File.ReadAllText(MatrixPath));
using var proposal = JsonDocument.Parse(File.ReadAllText(ProposalPath));
using var global = JsonDocument.Parse(File.ReadAllText("global.json"));
var root = matrix.RootElement;
var accepted = proposal.RootElement;

Need(root.GetProperty("schemaVersion").GetInt32() == 1 &&
    root.GetProperty("id").GetString() == "M8-W02-C01-T1-G-RS316" &&
    root.GetProperty("status").GetString() ==
        "candidate-only-no-migration-or-compatibility-certification" &&
    root.GetProperty("proposal").GetString() == ProposalPath &&
    accepted.GetProperty("status").GetString() == "research-proposal-not-migrated",
    "Candidate-only matrix or historical proposal authority changed.");

var baseline = root.GetProperty("baseline");
var sdk = global.RootElement.GetProperty("sdk");
var props = XDocument.Load("Directory.Build.props");
Need(baseline.GetProperty("sdk").GetString() == "10.0.401" &&
    sdk.GetProperty("version").GetString() == "10.0.401" &&
    sdk.GetProperty("rollForward").GetString() == "latestPatch" &&
    baseline.GetProperty("rollForward").GetString() == "latestPatch" &&
    baseline.GetProperty("tfm").GetString() == "net10.0" &&
    Element(props, "TargetFramework") == "net10.0" &&
    baseline.GetProperty("language").GetString() == "14.0" &&
    Element(props, "LangVersion") == "14.0" &&
    baseline.GetProperty("installedSdkToInspect").GetString() == "11.0.100" &&
    baseline.GetProperty("packables").GetInt32() == 13 &&
    baseline.GetProperty("testProjects").GetInt32() == 11 &&
    baseline.GetProperty("sourceEdges").GetInt32() == 16 &&
    !baseline.GetProperty("mutated").GetBoolean(),
    "Production .NET/C# baseline or matrix boundary was silently migrated.");

var official = root.GetProperty("officialSources").EnumerateArray().ToArray();
string[] expectedUrls =
[
    "https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries",
    "https://learn.microsoft.com/en-us/dotnet/core/compatibility/11",
    "https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15",
    "https://learn.microsoft.com/en-us/dotnet/core/tools/global-json"
];
Need(official.Length == 4 &&
    official.Select(x => x.GetProperty("url").GetString())
        .OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(expectedUrls.OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal) &&
    official.All(x => !string.IsNullOrWhiteSpace(x.GetProperty("relevance").GetString())) &&
    official.Select(x => x.GetProperty("id").GetString())
        .Distinct(StringComparer.Ordinal).Count() == 4,
    "Official source identity or release/language/SDK coverage differs.");

var sourceFiles = Directory.EnumerateFiles("src/projects", "*.csproj",
        SearchOption.AllDirectories)
    .Where(path => !path.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                   !path.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Where(path => Element(XDocument.Load(path), "IsPackable") == "true")
    .ToArray();
var sources = new Dictionary<string, string>(StringComparer.Ordinal);
foreach (var path in sourceFiles)
{
    var project = XDocument.Load(path);
    var id = Element(project, "PackageId") ??
        throw new InvalidDataException("RS316: Missing package identity: " + path);
    Need(sources.TryAdd(id, Path.GetFullPath(path)) &&
        Path.GetFileNameWithoutExtension(path) == id,
        "Duplicate or unexpected packable project identity: " + id);
}
Need(sources.Count == 13, "Expected thirteen current Math packable sources.");

var candidates = accepted.GetProperty("candidates").EnumerateArray()
    .Select(x => (Id: x.GetProperty("id").GetString()!,
                  Owner: x.GetProperty("ownerPackage").GetString()!))
    .ToArray();
Need(candidates.Length == 9 &&
    candidates.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() == 9 &&
    candidates.All(x => sources.ContainsKey(x.Owner)),
    "Nine historical research candidates or owner assignments changed.");

var rows = root.GetProperty("packages").EnumerateArray().ToArray();
var rowIds = rows.Select(x => x.GetProperty("packageId").GetString()!)
    .ToHashSet(StringComparer.Ordinal);
Need(rows.Length == 13 && rowIds.Count == 13 &&
    rowIds.SetEquals(sources.Keys),
    "Official delta candidate matrix does not cover every packable Math source.");

var candidateCount = 0;
foreach (var row in rows)
{
    var id = row.GetProperty("packageId").GetString()!;
    var project = row.GetProperty("projectPath").GetString()!;
    var test = row.GetProperty("testProject").GetString()!;
    Need(Path.GetFullPath(project) == sources[id] &&
        File.Exists(test) &&
        Path.GetFileNameWithoutExtension(test).EndsWith(".tests", StringComparison.Ordinal) &&
        Element(XDocument.Load(test), "IsTestProject") == "true",
        "A matrix source or test owner is ungrounded: " + id);
    var testXml = XDocument.Load(test);
    var testReferences = testXml.Descendants()
        .Where(x => x.Name.LocalName == "ProjectReference")
        .Select(x => x.Attribute("Include")?.Value ??
            throw new InvalidDataException("RS316: Test ProjectReference has no path."))
        .Select(x => Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(test)!, x.Replace('\\', Path.DirectorySeparatorChar))))
        .ToArray();
    Need(testReferences.Contains(sources[id], OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal),
        "A declared direct test owner does not reference the candidate package: " + id);

    var actual = row.GetProperty("proposalCandidateIds")
        .EnumerateArray().Select(x => x.GetString()!).ToArray();
    var expected = candidates.Where(x => x.Owner == id).Select(x => x.Id).ToArray();
    Need(RisksMatch(actual, expected) &&
        actual.Distinct(StringComparer.Ordinal).Count() == actual.Length,
        "Candidate IDs do not match historical research ownership: " + id);
    candidateCount += actual.Length;

    var posture = actual.Length == 0
        ? "not-enumerated-in-proposal-requires-review"
        : "candidate-requires-qualification";
    Need(row.GetProperty("disposition").GetString() == posture &&
        row.GetProperty("compatibility").GetString() == "unqualified" &&
        (row.GetProperty("nextOracle").GetString()?.Length ?? 0) >= 65,
        "A candidate was silently declared applicable, compatible or lacking its next oracle: " + id);
}
Need(candidateCount == 9,
    "The nine original research candidates are not all accounted for.");

var omitted = rowIds.Where(x => x != "dw.tools.math.analysis")
    .ToHashSet(StringComparer.Ordinal);
Need(!omitted.SetEquals(sources.Keys) &&
    !RisksMatch(["N11-999-FORGED"], candidates
        .Where(x => x.Owner == "dw.quantities").Select(x => x.Id)),
    "Missing package or invented candidate ID escaped the same strict membership checks.");

var limitations = root.GetProperty("limits").EnumerateArray()
    .Select(x => x.GetString()).ToArray();
Need(limitations.Length == 5 &&
    limitations.All(x => !string.IsNullOrWhiteSpace(x)) &&
    limitations.Any(x => x!.Contains("Absence", StringComparison.Ordinal)) &&
    limitations.Any(x => x!.Contains("no", StringComparison.OrdinalIgnoreCase)),
    "The uncertainty, non-applicability and no-migration disclaimers changed.");

Console.WriteLine("MATH-RS316-OFFICIAL-DELTA|packages=13|directTests=13|" +
    "proposalCandidates=9|officialSources=4|missingRowRefused=true|" +
    "forgedCandidateRefused=true|compatibility=unqualified|migration=false|PASS");

static string? Element(XDocument doc, string name) =>
    doc.Descendants().FirstOrDefault(x => x.Name.LocalName == name)?.Value.Trim();

static bool RisksMatch(IEnumerable<string> observed, IEnumerable<string> expected) =>
    observed.OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(expected.OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal);

static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS316: " + reason);
}
