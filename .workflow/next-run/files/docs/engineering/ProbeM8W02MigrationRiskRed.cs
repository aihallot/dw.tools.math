#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string CandidatePath = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string MatrixPath = "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json";
const string PublishedPath = "docs/engineering/m8-w02-c01-published-standard-closure-rs322.json";
var sources = Directory.EnumerateFiles("src/projects", "*.csproj", SearchOption.AllDirectories)
    .Where(path => !path.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                   !path.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Select(path => XDocument.Load(path))
    .Where(doc => DocValue(doc, "IsPackable") == "true")
    .Select(doc => DocValue(doc, "PackageId") ??
        throw new InvalidDataException("RS327: Source package has no PackageId"))
    .ToHashSet(StringComparer.Ordinal);
Need(sources.Count == 13,
    "Actual Math source project coverage differs from the thirteen-package audit.");

using var global = JsonDocument.Parse(File.ReadAllText("global.json"));
var build = XDocument.Load("Directory.Build.props");
string sdk = global.RootElement.GetProperty("sdk").GetProperty("version").GetString()!;
string tfm = DocValue(build, "TargetFramework")!;
string language = DocValue(build, "LangVersion")!;
Need(sdk == "10.0.401" &&
    global.RootElement.GetProperty("sdk").GetProperty("rollForward").GetString() ==
        "latestPatch" &&
    tfm == "net10.0" && language == "14.0",
    "Production SDK/TFM/C# baseline was migrated before a qualified RED gate.");

var protectedInputs = new[] { CandidatePath, MatrixPath, PublishedPath,
    "global.json", "Directory.Build.props" };
var unchanged = protectedInputs.ToDictionary(path => path, Sha256, StringComparer.Ordinal);
JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))?.AsObject() ??
    throw new InvalidDataException("RS327: Invalid authority JSON " + path);
var candidate = Read(CandidatePath);
var matrix = Read(MatrixPath);
var physical = Read(PublishedPath);

string? Evaluate(JsonObject proposal, JsonObject audit, JsonObject archives)
{
    if (proposal["schemaVersion"]?.GetValue<int>() != 1 ||
        proposal["id"]?.GetValue<string>() != "M8-W02-C02-T1-R-RS327" ||
        proposal["status"]?.GetValue<string>() != "blocked-not-authorized" ||
        proposal["sourceMatrix"]?.GetValue<string>() != MatrixPath ||
        proposal["physicalArchiveLedger"]?.GetValue<string>() != PublishedPath)
        return "Unqualified migration status or source authority was promoted.";

    var baseline = proposal["productionBaseline"]?.AsObject();
    var target = proposal["prospectiveTarget"]?.AsObject();
    if (baseline?["sdk"]?.GetValue<string>() != sdk ||
        baseline?["targetFramework"]?.GetValue<string>() != tfm ||
        baseline?["language"]?.GetValue<string>() != language ||
        target?["sdk"]?.GetValue<string>() != "11.0.100" ||
        target?["targetFramework"]?.GetValue<string>() != "net11.0" ||
        target?["language"]?.GetValue<string>() != "15.0")
        return "Production baseline or future SDK/TFM target was silently modified.";

    var rows = audit["packages"]?.AsArray();
    if (rows is null || rows.Count != 13 ||
        rows.Any(x => x?["packageId"] is null) ||
        !rows.Select(x => x!["packageId"]!.GetValue<string>())
            .ToHashSet(StringComparer.Ordinal).SetEquals(sources))
        return "The thirteen qualified source-project audit identities were lost.";
    if (audit["status"]?.GetValue<string>() !=
        "candidate-only-no-migration-or-compatibility-certification" ||
        rows.Any(x => x?["compatibility"]?.GetValue<string>() != "unqualified"))
        return "A semantic/source API compatibility verdict was falsely qualified.";
    int candidates = rows.Sum(x => x?["proposalCandidateIds"]?.AsArray().Count ?? 0);
    if (candidates != 9 || audit["officialSources"]?.AsArray().Count != 4)
        return "The nine historic candidate risks or four official sources changed.";

    var published = archives["packages"]?.AsArray();
    if (published is null || published.Count != 3 ||
        !published.Select(x => x!["packageId"]!.GetValue<string>())
            .ToHashSet(StringComparer.Ordinal).SetEquals(
                new[] { "dw.quantities", "dw.quantities.expression", "dw.quantities.standard" }) ||
        !published.All(x => x?["archiveSha256"]?.GetValue<string>().Length == 64 &&
            x?["binarySha256"]?.GetValue<string>().Length == 64))
        return "An unsupported published archive was added or a physical identity lost.";
    if (archives["limitations"]?.AsArray().Count != 5 ||
        !archives["limitations"]!.AsArray().Any(x =>
            x?.GetValue<string>().Contains("other ten", StringComparison.Ordinal) == true))
        return "The missing ten-package closure qualification was erased.";

    var observed = proposal["observed"]?.AsObject();
    if (observed?["packableSourceProjects"]?.GetValue<int>() != rows.Count ||
        observed?["publishedArchiveClosuresPhysicallyProven"]?.GetValue<int>() != published.Count ||
        observed?["publishedArchiveClosuresUnqualified"]?.GetValue<int>() != rows.Count - published.Count ||
        observed?["unqualifiedSemanticVerdicts"]?.GetValue<int>() != rows.Count ||
        observed?["historicalProposalCandidates"]?.GetValue<int>() != candidates)
        return "A migration coverage claim exceeds the observed thirteen/three/ten evidence.";

    var gates = proposal["gates"]?.AsObject();
    if (gates?["migrationAuthorization"]?.GetValue<string>() != "not-granted")
        return "An unauthorized production migration approval was asserted.";
    if (gates?["rollbackEvidence"]?.GetValue<string>() != "unqualified")
        return "A missing migration rollback proof was asserted to exist.";
    if (gates?["crossPlatformAndRid"]?.GetValue<string>() != "unqualified" ||
        gates?["numericBehavior"]?.GetValue<string>() != "unqualified" ||
        gates?["allPublishedPackageGraph"]?.GetValue<string>() != "unqualified")
        return "Platform, numerical behavior or complete published package graph was overclaimed.";

    var limitations = proposal["limitations"]?.AsArray();
    if (limitations?.Count != 6 ||
        !limitations.Any(x => x!.GetValue<string>().Contains("ten", StringComparison.Ordinal)) ||
        !limitations.Any(x => x!.GetValue<string>().Contains("Rollback", StringComparison.Ordinal)))
        return "Independent readiness limitations were removed.";
    return null;
}

Need(Evaluate(candidate, matrix, physical) is null,
    "The authentic Math migration hazard profile was not refused safely.");

int negatives = 0;
void Reject(string name, Action<JsonObject, JsonObject, JsonObject> falsify,
    string expectedRefusal)
{
    var p = candidate.DeepClone().AsObject();
    var m = matrix.DeepClone().AsObject();
    var x = physical.DeepClone().AsObject();
    falsify(p, m, x);
    var observed = Evaluate(p, m, x);
    Need(observed == expectedRefusal,
        "Migration RED did not refuse " + name + " with its exact reason: " + observed);
    Need(Evaluate(candidate, matrix, physical) is null,
        "The original accepted migration blocker was altered by " + name);
    negatives++;
    Console.WriteLine("MATH-RS327-MIGRATION-RED|refusal=" + name + "|PASS");
}
Reject("invent-ten-physical-package-proofs",
    (p, m, x) => p["observed"]!["publishedArchiveClosuresPhysicallyProven"] = 13,
    "A migration coverage claim exceeds the observed thirteen/three/ten evidence.");
Reject("promote-an-unqualified-api",
    (p, m, x) => m["packages"]![0]!["compatibility"] = "qualified",
    "A semantic/source API compatibility verdict was falsely qualified.");
Reject("forge-production-migration-approval",
    (p, m, x) => p["gates"]!["migrationAuthorization"] = "granted",
    "An unauthorized production migration approval was asserted.");
Reject("forge-rollback-evidence",
    (p, m, x) => p["gates"]!["rollbackEvidence"] = "qualified",
    "A missing migration rollback proof was asserted to exist.");
Reject("remove-one-source-project",
    (p, m, x) => m["packages"]!.AsArray().RemoveAt(0),
    "The thirteen qualified source-project audit identities were lost.");
Reject("rebaseline-to-future-sdk",
    (p, m, x) => p["productionBaseline"]!["sdk"] = "11.0.100",
    "Production baseline or future SDK/TFM target was silently modified.");

Need(negatives == 6, "Expected six independent migration blocker refusals.");
foreach (var path in protectedInputs)
    Need(Sha256(path) == unchanged[path], "Math evidence or production SDK changed: " + path);
Console.WriteLine("MATH-RS327-MIGRATION-RED|sourceProjects=13|physicallyPinned=3|" +
    "unqualifiedPhysical=10|unqualifiedSemantic=13|historicalCandidates=9|" +
    "independentRefusals=6|migrationAuthorized=false|rollbackQualified=false|" +
    "sdk=10.0.401|guidance=0.27|productionMutation=false|PASS");

static string? DocValue(XDocument doc, string name) =>
    doc.Descendants().FirstOrDefault(x => x.Name.LocalName == name)?.Value.Trim();
static string Sha256(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS327: " + reason);
}
