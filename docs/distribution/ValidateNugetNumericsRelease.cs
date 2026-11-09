#:property NuGetAudit=false
using System.Security.Cryptography;
using System.Text.Json;
using System.Linq;

const string Evidence = "docs/distribution/nuget-federation-f37-numerics-release-qualified.json";
const string Status = "rs170-numerics-immutable-release-controller-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F37:SOURCE=READY;CONTROLLER=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F37:CONTROLLER=QUALIFIED;ORIGINAL=PINNED;NATIVE=541";
const string Limits = "MATH-NUMERICS-F37:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string Tool = "tools/dw.tools.math.nuget-numerics-release/";
string[] files = [
    "docs/distribution/nuget-federation-f37-numerics-release.md",
    "docs/distribution/ValidateNugetNumericsRelease.cs",
    Tool + "dw.tools.math.nuget-numerics-release.csproj",
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
];
static string Text(JsonElement element, string key) =>
    element.GetProperty(key).GetString()
        ?? throw new InvalidDataException("missing exact field " + key);
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS170 Numerics release: " + reason);
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only observational --check is supported");
foreach (var p in files) Need(File.Exists(p), "missing release file " + p);
using var predecessor = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS169/attempt-002.json"));
using var f35 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json"));
using var f36 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json"));
var r = predecessor.RootElement;
var original = f35.RootElement.GetProperty("candidate");
var rehearsal = f36.RootElement.GetProperty("rehearsal");
Need(Text(r, "runId") == "RS169" &&
     r.GetProperty("attempt").GetInt32() == 2 &&
     Text(r, "outcome") == "succeeded" &&
     r.GetProperty("correctionMode").GetBoolean() &&
     r.GetProperty("correctionSourceAttempt").GetInt32() == 1 &&
     Text(r.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(f35.RootElement, "status") ==
         "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
     Text(f36.RootElement, "status") ==
         "rs169-numerics-immutable-disposable-rehearsal-qualified-unpublished" &&
     Text(original, "candidateArchiveSha256") ==
         "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d" &&
     Text(original, "candidateDllSha256") ==
         "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4" &&
     original.GetProperty("candidateArchiveBytes").GetInt32() == 18313 &&
     original.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Text(rehearsal, "originalF35ArchiveSha256") ==
         Text(original, "candidateArchiveSha256") &&
     Text(rehearsal, "rehearsalArchiveSha256") !=
         Text(original, "candidateArchiveSha256") &&
     !rehearsal.GetProperty("reconstructionIsOriginalArchiveProof").GetBoolean(),
     "F35 original and F36 non-identical rehearsal evidence must remain exact");
Need(File.ReadAllText(files[2]).Contains(
        "<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains(
        "Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("MATH-NUMERICS-F37-REFUSED",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("only exact F35 original Numerics archive",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("privatePublishGateEnabled = true",
        StringComparison.Ordinal),
     "publisher source and immutable F35 guard changed");
if (File.Exists(Evidence))
{
    using var receipt = JsonDocument.Parse(File.ReadAllText(Evidence));
    var obj = receipt.RootElement;
    var controller = obj.GetProperty("controller");
    Need(Text(obj, "status") == Status &&
         !obj.GetProperty("package_published").GetBoolean() &&
         !obj.GetProperty("release_authorized").GetBoolean() &&
         obj.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Text(controller, "status") ==
             "qualified-numerics-immutable-release-controller-unpublished" &&
         Text(controller, "packageId") == "dw.tools.math.numerics" &&
         Text(controller, "originalF35ArchiveSha256") ==
             Text(original, "candidateArchiveSha256") &&
         controller.GetProperty("originalF35ArchiveBytes").GetInt32() == 18313 &&
         controller.GetProperty("malformedCandidateRefused").GetBoolean() &&
         controller.GetProperty("privatePublishGateEnabled").GetBoolean() &&
         controller.GetProperty("privateVerifyGateEnabled").GetBoolean() &&
         !controller.GetProperty("sharedFeedPublishedByThisRun").GetBoolean() &&
         !controller.GetProperty("sharedPublicationAuthorized").GetBoolean() &&
         Text(controller, "directDependencyArchiveSha256") ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(controller, "transitiveDependencyArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         obj.GetProperty("command_receipts").GetArrayLength() == 4 &&
         obj.GetProperty("source_files").EnumerateObject().Count() == 5 &&
         obj.GetProperty("source_files").EnumerateObject()
             .All(x => File.Exists(x.Name) && Sha(x.Name) == x.Value.GetString()),
         "controller receipt, exact pinned dependencies, source hashes or non-publication are invalid");
}
Console.WriteLine(File.Exists(Evidence) ? Qualified : Ready);
Console.WriteLine(Limits);
