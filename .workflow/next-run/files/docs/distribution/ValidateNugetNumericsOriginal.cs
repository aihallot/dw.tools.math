#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f38-numerics-original-qualified.json";
const string Status = "rs171-original-numerics-consumer-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F38:ORIGINAL=READY;BINARY=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F38:ORIGINAL=VERIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-NUMERICS-F38:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
string[] files =
[
    "docs/distribution/nuget-federation-f38-numerics-original.md",
    "docs/distribution/ValidateNugetNumericsOriginal.cs",
    "tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj",
    "tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs"
];
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS171 original Numerics: " + message);
}
static string Text(JsonElement value, string key) =>
    value.GetProperty(key).GetString() ??
        throw new InvalidDataException("required exact JSON value missing: " + key);
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "validator only accepts observational --check");
foreach (var file in files) Need(File.Exists(file), "missing immutable source " + file);
using var prior = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS170/attempt-001.json"));
using var f35 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json"));
using var f37 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f37-numerics-release-qualified.json"));
var report = prior.RootElement;
var original = f35.RootElement.GetProperty("candidate");
Need(Text(report, "runId") == "RS170" &&
     report.GetProperty("attempt").GetInt32() == 1 &&
     Text(report, "outcome") == "succeeded" &&
     !report.GetProperty("priorInvocationFailures")
         .GetProperty("firstPassSucceeded").GetBoolean() &&
     Text(report.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(f35.RootElement, "status") ==
         "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
     Text(f37.RootElement, "status") ==
         "rs170-numerics-immutable-release-controller-qualified-unpublished" &&
     !f37.RootElement.GetProperty("package_published").GetBoolean() &&
     f37.RootElement.GetProperty("controller")
         .GetProperty("originalCandidatePhysicallyVerified").GetBoolean() &&
     Text(original, "candidateArchiveSha256") ==
         "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d" &&
     Text(original, "candidateDllSha256") ==
         "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4",
     "RS170 or original F35 binary predecessor is not exact");
Need(File.ReadAllText(files[2]).Contains("<Compile Include=\"Program.cs\" />",
        StringComparison.Ordinal) &&
     !File.ReadAllText(files[2]).Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("MATH-NUMERICS-F38-REFUSED",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("archiveRebuilt = false",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("privateFeedPublished = false",
        StringComparison.Ordinal),
     "original consumer and no-publication code contract changed");
if (File.Exists(Evidence))
{
    using var document = JsonDocument.Parse(File.ReadAllText(Evidence));
    var root = document.RootElement;
    var found = root.GetProperty("observation");
    Need(Text(root, "status") == Status &&
         root.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !root.GetProperty("package_published").GetBoolean() &&
         !root.GetProperty("release_authorized").GetBoolean() &&
         Text(found, "status") ==
             "qualified-original-f35-numerics-consumer-unpublished" &&
         Text(found, "packageId") == "dw.tools.math.numerics" &&
         Text(found, "packageVersion") == "20260.20261109.1001" &&
         Text(found, "originalArchiveSha256") ==
             Text(original, "candidateArchiveSha256") &&
         Text(found, "originalDllSha256") ==
             Text(original, "candidateDllSha256") &&
         Text(found, "disposableArchiveSha256") ==
             Text(original, "candidateArchiveSha256") &&
         found.GetProperty("originalArchivePhysicallyVerified").GetBoolean() &&
         found.GetProperty("freshNuGetPackagesCache").GetBoolean() &&
         !found.GetProperty("archiveRebuilt").GetBoolean() &&
         !found.GetProperty("rehearsalArchiveSubstituted").GetBoolean() &&
         !found.GetProperty("privateFeedPublished").GetBoolean() &&
         !found.GetProperty("releaseAuthorized").GetBoolean() &&
         found.GetProperty("compiledBinaryChecks").GetInt32() == 15 &&
         Text(found, "directDependencyArchiveSha256") ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(found, "transitiveDependencyArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         found.GetProperty("commandReceipts").GetArrayLength() == 4 &&
         root.GetProperty("command_receipts").GetArrayLength() == 4 &&
         root.GetProperty("source_files").EnumerateObject().Count() == 4 &&
         root.GetProperty("source_files").EnumerateObject().All(x =>
             File.Exists(x.Name) && Sha(x.Name) == x.Value.GetString()),
         "immutable F35 actual binary consumer receipts are incomplete");
}
Console.WriteLine(File.Exists(Evidence) ? Qualified : Ready);
Console.WriteLine(Limits);
