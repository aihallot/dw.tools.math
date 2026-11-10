#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence =
    "docs/distribution/electrochemical-nuget-candidate-qualified.json";
const string Status =
    "rs174-electrochemical-standard-calendar-candidate-qualified-unpublished";
const string Baseline =
    "MATH-ELECTROCHEM-CANDIDATE:SOURCE=READY;BINARY=PENDING;NATIVE=541";
const string Qualified =
    "MATH-ELECTROCHEM-CANDIDATE:ARCHIVE=VERIFIED;BINARY=12;NATIVE=541";
const string Limits =
    "MATH-ELECTROCHEM-CANDIDATE:SHARED-PUBLISHED=false;PUBLIC=false;RIGHTS-UNESTABLISHED";
string[] inputs =
[
    "docs/distribution/electrochemical-nuget-candidate.md",
    "docs/distribution/ValidateElectrochemicalNugetCandidate.cs",
    "tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs",
    "tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj",
    "tools/dw.tools.math.nuget-electrochemical-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs"
];
static void Need(bool ok, string why)
{
    if (!ok) throw new InvalidDataException("RS174 electrochemical candidate: " + why);
}
static string Text(JsonElement data, string key) =>
    data.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing exact candidate field " + key);
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
     "only observational --check is supported");
foreach (var path in inputs) Need(File.Exists(path), "missing staged source " + path);
using var prior = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS173/attempt-001.json"));
using var qualifiedSource = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/electrochemical-unit-profile-source-qualified.json"));
using var old = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f10-standard-published-qualified.json"));
var result = prior.RootElement;
var source = qualifiedSource.RootElement;
Need(Text(result, "runId") == "RS173" &&
     result.GetProperty("attempt").GetInt32() == 1 &&
     Text(result, "outcome") == "succeeded" &&
     Text(result.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     !result.GetProperty("priorInvocationFailures")
        .GetProperty("firstPassSucceeded").GetBoolean() &&
     result.GetProperty("priorInvocationFailures")
        .GetProperty("preparationFailureCount").GetInt32() == 1 &&
     Text(result.GetProperty("git"), "commitMessage") ==
         "feat: add explicit Math electrochemical SI unit profile with exact dimensions" &&
     Text(source, "status") ==
         "rs173-math-electrochemical-six-unit-source-qualified-not-packaged" &&
     source.GetProperty("tests").GetProperty("passed").GetInt32() == 12 &&
     Text(old.RootElement, "status") ==
         "rs143-standard-published-local-three-binaries-independently-verified" &&
     old.RootElement.GetProperty("physically_published_package_verified").GetBoolean(),
     "RS173 source test result or old Standard immutable release changed");
Need(File.ReadAllText(inputs[3]).Contains(
        "<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(inputs[4]).Contains(
        "Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(inputs[5]).Contains("MATH-ELECTROCHEM-BINARY|PASS|12",
        StringComparison.Ordinal) &&
     File.ReadAllText(inputs[2]).Contains("originalSixSourceFilesLinked = true",
        StringComparison.Ordinal) &&
     File.ReadAllText(inputs[2]).Contains("publishedToNugetLocal = false",
        StringComparison.Ordinal),
     "original candidate tool, consumer or exact reader reference changed");
if (File.Exists(Evidence))
{
    using var package = JsonDocument.Parse(File.ReadAllText(Evidence));
    var receipt = package.RootElement;
    var c = receipt.GetProperty("candidate");
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.quantities.standard" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !receipt.GetProperty("package_published").GetBoolean() &&
         !receipt.GetProperty("release_authorized").GetBoolean() &&
         !receipt.GetProperty("aura_code_reuse_rights_established").GetBoolean() &&
         receipt.GetProperty("source_files").EnumerateObject().Count() == 6 &&
         receipt.GetProperty("source_files").EnumerateObject()
             .All(x => File.Exists(x.Name) && Hash(x.Name) == x.Value.GetString()),
         "F40 source-owned and unpublished boundaries not retained");
    Need(Text(c, "status") ==
             "qualified-electrochemical-standard-calendar-candidate-unpublished" &&
         Text(c, "packageId") == "dw.quantities.standard" &&
         Regex.IsMatch(Text(c, "packageVersion"),
             @"^20260\.2026\d{4}\.1\d{3}$", RegexOptions.CultureInvariant) &&
         Text(c, "historicalSourceVersion") == "0.2.0-preview.1" &&
         c.GetProperty("originalSixSourceFilesLinked").GetBoolean() &&
         c.GetProperty("packageDependencies").GetInt32() == 2 &&
         c.GetProperty("consumerChecks").GetInt32() == 12 &&
         c.GetProperty("loadedThreeDllHashesVerified").GetBoolean() &&
         c.GetProperty("legacyCataloguesPreserved").GetBoolean() &&
         c.GetProperty("sourceFilesUnchangedAfter").GetBoolean() &&
         !c.GetProperty("publishedToNugetLocal").GetBoolean() &&
         !c.GetProperty("published").GetBoolean() &&
         !c.GetProperty("versionReserved").GetBoolean() &&
         !c.GetProperty("auraCodeReuseRightsEstablished").GetBoolean() &&
         Text(c, "dependencyQuantitiesArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(c, "dependencyExpressionArchiveSha256") ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         Text(c, "originalStandardArchiveSha256") ==
             "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
         c.GetProperty("originalSourcesSha256").EnumerateObject().Count() == 6 &&
         Text(c, "loadedClrAssemblyIdentities").StartsWith(
             "MATH-ELECTROCHEM-CLR|standard=", StringComparison.Ordinal) &&
         c.GetProperty("packagedFileInventory").GetArrayLength() >= 3 &&
         c.GetProperty("packagedFileInventory").GetArrayLength() <= 48 &&
         c.GetProperty("producerEffectiveSources").GetArrayLength() >= 1 &&
         c.GetProperty("consumerEffectiveSources").GetArrayLength() >= 2 &&
         c.GetProperty("commands").GetArrayLength() == 9 &&
         receipt.GetProperty("command_receipts").GetArrayLength() == 4 &&
         c.GetProperty("candidateArchiveBytes").GetInt64() > 10000 &&
         Regex.IsMatch(Text(c, "candidateArchiveSha256"), "^[a-f0-9]{64}$") &&
         Regex.IsMatch(Text(c, "candidateDllSha256"), "^[a-f0-9]{64}$"),
         "candidate NuGet, direct package closure, actual binary test and archive manifests incomplete");
}
Console.WriteLine(File.Exists(Evidence) ? Qualified : Baseline);
Console.WriteLine(Limits);
