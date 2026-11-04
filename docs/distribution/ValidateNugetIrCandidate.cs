#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Receipt = "docs/distribution/nuget-federation-f11-ir-candidate-qualified.json";
const string Status = "rs144-ir-calendared-dependent-nuget-candidate-qualified-unpublished";
const string Baseline = "MATH-IR-F11:SOURCE=READY;DEPENDENCY=PINNED;NATIVE=541";
const string Qualified = "MATH-IR-F11:CANDIDATE=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-IR-F11:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f11-ir-candidate.md",
    "docs/distribution/ValidateNugetIrCandidate.cs",
    "tools/dw.tools.math.nuget-ir-candidate/dw.tools.math.nuget-ir-candidate.csproj",
    "tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-ir-candidate/Program.cs",
    "tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs"
};
static void Need(bool value, string reason)
{
    if (!value) throw new InvalidDataException("RS144 IR candidate: " + reason);
}
static JsonDocument Load(string file) => JsonDocument.Parse(File.ReadAllText(file));
static string Text(JsonElement node, string name) => node.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing source authority: " + name);
static bool Flag(JsonElement node, string name) => node.GetProperty(name).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static bool IsSha(string hash) =>
    Regex.IsMatch(hash, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
static int Count(JsonElement node)
{
    var result = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var collection))
            result += collection.EnumerateArray().Sum(Count);
    return result;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is supported");
foreach (var path in files)
    Need(File.Exists(path), "staged full-source file absent " + path);
using var prior = Load(".aura/workflow/reports/RS143/attempt-001.json");
using var published = Load("docs/distribution/nuget-federation-f10-standard-published-qualified.json");
using var baseReceipt = Load("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var f1 = Load("docs/distribution/nuget-federation-f1-qualified.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var backlog = Load("docs/planning/backlog.json");
using var guidance = Load("docs/workflow/manifest.json");
Need(Text(prior.RootElement, "runId") == "RS143" &&
     prior.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior.RootElement, "outcome") == "succeeded" &&
     Text(prior.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.RootElement.GetProperty("priorInvocationFailures").GetProperty("count")
         .GetInt32() == 0 &&
     Flag(prior.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.RootElement.GetProperty("git"), "commitMessage") ==
         "test: verify published Standard NuGet and exact dependency binaries",
     "RS143 first-pass successful predecessor changed");
Need(Text(published.RootElement, "status") ==
         "rs143-standard-published-local-three-binaries-independently-verified" &&
     Flag(published.RootElement, "physically_published_package_verified") &&
     Flag(published.RootElement, "transitive_packages_verified") &&
     !Flag(published.RootElement, "package_published_by_this_run") &&
     Text(baseReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseReceipt.RootElement, "physically_published_package_verified"),
     "the three physically published Math dependencies must already be qualified");
var standard = published.RootElement.GetProperty("observation");
Need(Text(standard, "packageId") == "dw.quantities.standard" &&
     standard.GetProperty("compiledConsumerChecks").GetInt32() == 10 &&
     standard.GetProperty("publishedPackageSnapshotCount").GetInt32() == 3 &&
     Text(standard, "archiveSha256") ==
         "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75",
     "independent third-package published receipt changed");
Need(Text(f1.RootElement, "status") ==
         "rs134-math-f1-calendar-feed-inventory-no-publication" &&
     f1.RootElement.GetProperty("package_count").GetInt32() == 13 &&
     !Flag(f1.RootElement, "package_published"),
     "original F1 version proposal authority changed");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "Math 541-node completed native plan and accepted product roadmap changed");

const string OriginalProject = "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj";
var originalXml = File.ReadAllText(OriginalProject);
Need(originalXml.Contains("<Version>0.3.0-preview.1</Version>",
         StringComparison.Ordinal) &&
     originalXml.Contains(
         "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />",
         StringComparison.Ordinal),
     "historical IR source version or base ProjectReference changed");
const string Tool = "tools/dw.tools.math.nuget-ir-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-ir-candidate.csproj")
        .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
            StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
        .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
            StringComparison.Ordinal),
     "exact published shared NuGet library must be centrally pinned");

var exists = File.Exists(Receipt);
if (exists)
{
    using var finished = Load(Receipt);
    var record = finished.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.tools.math.ir" &&
         Text(record, "feed_selector") == "NUGET_LOCAL" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(record, "package_published") &&
         !Flag(record, "release_authorized_by_this_run") &&
         !Flag(record, "source_versions_modified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "candidate cannot assert publication, license, rollback or integration");

    var sourceHashes = record.GetProperty("source_files");
    foreach (var path in files)
        Need(Text(sourceHashes, path) == Sha(path),
             "RS144 staged source changed: " + path);
    var candidate = record.GetProperty("candidate");
    Need(Text(candidate, "status") ==
             "qualified-ir-dependent-calendar-candidate-not-published" &&
         Text(candidate, "packageId") == "dw.tools.math.ir" &&
         Text(candidate, "historicalSourceVersion") == "0.3.0-preview.1" &&
         Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
         Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
         Text(candidate, "dependencyQuantitiesArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(candidate, "dependencyQuantitiesDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(candidate, "sharedNugetPackageVersion") == "20260.20261026.1010" &&
         Text(candidate, "sharedNugetDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Text(candidate, "feedSelector") == "NUGET_LOCAL" &&
         Text(candidate, "targetFramework") == "net10.0" &&
         candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
         candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 1 &&
         candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
         Flag(candidate, "originalSixSourceFilesLinked") &&
         Flag(candidate, "sourceProjectUnmodified") &&
         Flag(candidate, "temporaryPackagingProjection") &&
         Flag(candidate, "sourceFeedInventoryUnchanged") &&
         Flag(candidate, "independentPackageReference") &&
         !Flag(candidate, "published") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "publicLicenseSelected") &&
         !Flag(candidate, "installedRollbackQualified") &&
         IsSha(Text(candidate, "candidateArchiveSha256")) &&
         IsSha(Text(candidate, "candidateDllSha256")) &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 0,
         "new IR candidate source, binary or single dependency does not match the observed contract");
    var version = Text(candidate, "packageVersion");
    Need(Regex.IsMatch(version, @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
             RegexOptions.CultureInvariant) &&
         int.TryParse(version.Split('.')[2], out var ordinal) &&
         ordinal is >= 1001 and <= 1999 &&
         Text(candidate, "candidateArchivePath") ==
             "build/artifacts/rs144-ir-candidate/candidate/dw.tools.math.ir." +
             version + ".nupkg",
         "IR candidate calendar version or ignored archive path mismatch");
    Need(Text(candidate, "originalProjectSha256") == Sha(OriginalProject),
         "original IR project SHA-256 changed");
    var source = candidate.GetProperty("originalSourcesSha256");
    var sourceNames = new[] {
        "IrCanonicalHashes.cs", "IrCanonicalJsonCodec.cs", "IrCanonicalJsonGuard.cs",
        "IrGraphLimits.cs", "IrMatrix.cs", "IrModel.cs"
    };
    Need(source.EnumerateObject().Count() == sourceNames.Length,
         "exactly six original IR source file hashes must be recorded");
    foreach (var name in sourceNames)
    {
        var path = "src/projects/dw.tools.math.ir/" + name;
        Need(Text(source, path) == Sha(path), "IR source SHA-256 differs for " + name);
    }
    var expectedInner = new[] {
        "f1-inventory-before", "ir-linked-restore", "ir-linked-build",
        "ir-candidate-pack", "consumer-restore", "consumer-build",
        "consumer-binary-run", "f1-inventory-after", "sdk"
    };
    var inner = candidate.GetProperty("commands").EnumerateArray().ToArray();
    Need(inner.Length == expectedInner.Length &&
         inner.Select(x => Text(x, "id")).SequenceEqual(expectedInner) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "all nine exact source and consumer process results must have succeeded");
    var expectedOuter = new[] {
        "f1-restore", "f1-build", "tool-restore",
        "tool-build", "ir-candidate", "sdk"
    };
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outer.Length == expectedOuter.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(expectedOuter) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "six parent DWF IR package qualification commands must be recorded");
}
Console.WriteLine(exists ? Qualified : Baseline);
Console.WriteLine(Limits);
return 0;
