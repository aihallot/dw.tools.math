#:property NuGetAudit=false
using System.Security.Cryptography;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json";
const string Status = "rs169-numerics-immutable-disposable-rehearsal-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F36:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F36:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-NUMERICS-F36:SHARED-PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
string[] files =
[
    "docs/distribution/nuget-federation-f36-numerics-rehearsal.md",
    "docs/distribution/ValidateNugetNumericsRehearsal.cs",
    "tools/dw.tools.math.nuget-numerics-rehearsal/dw.tools.math.nuget-numerics-rehearsal.csproj",
    "tools/dw.tools.math.nuget-numerics-rehearsal/Directory.Packages.props",
    "tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs"
];
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS169 Numerics rehearsal: " + reason);
}
static string Text(JsonElement e, string key) =>
    e.GetProperty(key).GetString() ?? throw new InvalidDataException("Missing " + key);
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only observational --check is allowed");
foreach (var file in files) Need(File.Exists(file), "required rehearsal file missing: " + file);
using var resultReport = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS168/attempt-003.json"));
using var original = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json"));
using var state = JsonDocument.Parse(File.ReadAllText("docs/planning/backlog.json"));
using var project = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/plan/project.json"));
Need(Text(resultReport.RootElement, "runId") == "RS168" &&
     resultReport.RootElement.GetProperty("attempt").GetInt32() == 3 &&
     Text(resultReport.RootElement, "outcome") == "succeeded" &&
     Text(original.RootElement, "status") ==
         "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
     !original.RootElement.GetProperty("package_published").GetBoolean() &&
     Text(original.RootElement.GetProperty("candidate"), "candidateArchiveSha256") ==
         "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d" &&
     Text(state.RootElement, "status") == "done" &&
     Text(state.RootElement, "plan_version") == "0.1.122" &&
     project.RootElement.GetProperty("milestones")[3].GetProperty("state").GetString() == "done",
     "prior success, candidate or native product authority changed");
Need(File.ReadAllText(files[2]).Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("Version=\"[20260.20261026.1010]\"",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("MATH-NUMERICS-F36-REFUSED",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("disposableFeedPublished = true",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("sharedFeedPublished = false",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("independentCompiledBinaryChecks = 15",
        StringComparison.Ordinal),
     "rehearsal controller surface or refusal contract changed");
if (File.Exists(Evidence))
{
    using var record = JsonDocument.Parse(File.ReadAllText(Evidence));
    var root = record.RootElement;
    var rehearsal = root.GetProperty("rehearsal");
    var originalSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
    Need(Text(root, "status") == Status &&
         !root.GetProperty("shared_package_published").GetBoolean() &&
         !root.GetProperty("owner_release_authorized").GetBoolean() &&
         root.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Text(rehearsal, "status") ==
            "qualified-numerics-immutable-disposable-rehearsal-unpublished" &&
         Text(rehearsal, "packageId") == "dw.tools.math.numerics" &&
         Text(rehearsal, "originalF35ArchiveSha256") == originalSha &&
         rehearsal.GetProperty("disposableFeedPublished").GetBoolean() &&
         !rehearsal.GetProperty("sharedFeedPublished").GetBoolean() &&
         !rehearsal.GetProperty("releaseAuthorized").GetBoolean() &&
         rehearsal.GetProperty("immutableCollisionRefused").GetBoolean() &&
         rehearsal.GetProperty("falsifiedDigestRefused").GetBoolean() &&
         rehearsal.GetProperty("cancellationRefused").GetBoolean() &&
         rehearsal.GetProperty("independentCompiledBinaryChecks").GetInt32() == 15 &&
         rehearsal.GetProperty("directDependencyCount").GetInt32() == 1 &&
         Text(rehearsal, "directDependencyArchiveSha256") ==
            "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(rehearsal, "transitiveDependencyArchiveSha256") ==
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         rehearsal.GetProperty("candidateInnerCommands").GetInt32() == 11 &&
         rehearsal.GetProperty("commands").GetArrayLength() == 7 &&
         root.GetProperty("command_receipts").GetArrayLength() == 4 &&
         root.GetProperty("source_files").EnumerateObject().Count() == 5 &&
         root.GetProperty("source_files").EnumerateObject().All(x =>
             File.Exists(x.Name) && x.Value.GetString() == Sha(x.Name)) &&
         Regex.IsMatch(Text(rehearsal, "packageVersion"),
             @"\A20260\.2026\d{4}\.1\d{3}\z", RegexOptions.CultureInvariant),
         "rehearsal evidence, refusal or binary/dependency proof incomplete");
}
Console.WriteLine(File.Exists(Evidence) ? Qualified : Ready);
Console.WriteLine(Limits);
