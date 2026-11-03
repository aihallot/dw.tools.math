#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f8-standard-candidate-qualified.json";
const string Status = "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished";
const string Baseline = "MATH-STANDARD-F8:SOURCE=READY;DEPENDENCIES=2;NATIVE=541";
const string Target = "MATH-STANDARD-F8:CANDIDATE=QUALIFIED;BINARY=10;NATIVE=541";
const string Refusal = "MATH-STANDARD-F8:PUBLISHED=false;LICENSE=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f8-standard-candidate.md",
    "docs/distribution/ValidateNugetStandardCandidate.cs",
    "tools/dw.tools.math.nuget-standard-candidate/dw.tools.math.nuget-standard-candidate.csproj",
    "tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-standard-candidate/Program.cs",
    "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs"
};
static void Need(bool valid, string why)
{
    if (!valid) throw new InvalidDataException("RS141 Standard candidate: " + why);
}
static JsonDocument Load(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string name) =>
    node.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing required package field " + name);
static bool Flag(JsonElement node, string name) => node.GetProperty(name).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var amount = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            amount += children.EnumerateArray().Sum(Count);
    return amount;
}
static bool Hash64(string value) =>
    Regex.IsMatch(value, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is supported");
foreach (var file in files)
    Need(File.Exists(file), "missing full Standard qualification source: " + file);
using var prior = Load(".aura/workflow/reports/RS140/attempt-001.json");
using var baseProof = Load("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var expressionProof = Load("docs/distribution/nuget-federation-f7-expression-published-qualified.json");
using var sourceF1 = Load("docs/distribution/nuget-federation-f1-qualified.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var backlog = Load("docs/planning/backlog.json");
using var guidance = Load("docs/workflow/manifest.json");
Need(Text(prior.RootElement, "runId") == "RS140" &&
     prior.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior.RootElement, "outcome") == "succeeded" &&
     Text(prior.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("count").GetInt32() == 1 &&
     prior.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("preparationFailureCount").GetInt32() == 1 &&
     prior.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     !Flag(prior.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.RootElement.GetProperty("git"), "commitMessage") ==
         "test: verify published Expression NuGet binary and exact base closure",
     "RS140 real successful attempt with one prior pre-attempt refusal changed");
Need(Text(sourceF1.RootElement, "status") ==
         "rs134-math-f1-calendar-feed-inventory-no-publication" &&
     sourceF1.RootElement.GetProperty("package_count").GetInt32() == 13 &&
     !Flag(sourceF1.RootElement, "package_published") &&
     Text(expressionProof.RootElement, "status") ==
         "rs140-expression-published-local-binary-independently-verified" &&
     Flag(expressionProof.RootElement, "physically_published_package_verified") &&
     Flag(expressionProof.RootElement, "dependency_package_verified") &&
     !Flag(expressionProof.RootElement, "package_published_by_this_run") &&
     Text(baseProof.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseProof.RootElement, "physically_published_package_verified"),
     "the two exact published prerequisite package receipts are absent or altered");
var published = expressionProof.RootElement.GetProperty("observation");
Need(Text(published, "packageId") == "dw.quantities.expression" &&
     Text(published, "packageVersion") == "20260.20261102.1001" &&
     Text(published, "archiveSha256") ==
         "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
     Text(published, "loadedDllSha256") ==
         "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
     Text(published, "exactDependencyPackageId") == "dw.quantities" &&
     Text(published, "exactDependencyPackageVersion") == "20260.20261102.1001" &&
     Text(published, "exactDependencyArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(published, "loadedDependencyDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     published.GetProperty("compiledConsumerChecks").GetInt32() == 6,
     "published Expression and base DLLs differ from the exact dependency baseline");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "the accepted 541-node Math product scope must remain complete and unchanged");
const string SourceProject = "src/projects/dw.quantities.standard/dw.quantities.standard.csproj";
var originalProject = File.ReadAllText(SourceProject);
Need(originalProject.Contains("<Version>0.2.0-preview.1</Version>",
             StringComparison.Ordinal) &&
     originalProject.Contains(
             "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />",
             StringComparison.Ordinal) &&
     originalProject.Contains(
             "<ProjectReference Include=\"../dw.quantities.expression/dw.quantities.expression.csproj\" />",
             StringComparison.Ordinal),
     "historical source version or exactly two source dependency edges changed");
const string Tool = "tools/dw.tools.math.nuget-standard-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-standard-candidate.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "the NuGet shared library must remain an exact centrally pinned dependency");

var qualified = File.Exists(Evidence);
if (qualified)
{
    using var doc = Load(Evidence);
    var receipt = doc.RootElement;
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.quantities.standard" &&
         Text(receipt, "feed_selector") == "NUGET_LOCAL" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(receipt, "package_published") &&
         !Flag(receipt, "release_authorized_by_this_run") &&
         !Flag(receipt, "source_versions_modified") &&
         !Flag(receipt, "public_license_selected") &&
         !Flag(receipt, "installed_package_rollback_qualified") &&
         !Flag(receipt, "external_requests_transmitted") &&
         !Flag(receipt, "discovery_migration_qualified"),
         "a candidate qualification cannot claim an implicit public or private release");
    var hashes = receipt.GetProperty("source_files");
    foreach (var path in files)
        Need(Text(hashes, path) == Hash(path), "RS141 staged source changed: " + path);
    var candidate = receipt.GetProperty("candidate");
    Need(Text(candidate, "status") ==
             "qualified-standard-dependent-candidate-not-published" &&
         Text(candidate, "packageId") == "dw.quantities.standard" &&
         Text(candidate, "historicalSourceVersion") == "0.2.0-preview.1" &&
         Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
         Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
         Text(candidate, "dependencyQuantitiesArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(candidate, "dependencyQuantitiesDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(candidate, "dependencyExpressionId") == "dw.quantities.expression" &&
         Text(candidate, "dependencyExpressionVersion") == "20260.20261102.1001" &&
         Text(candidate, "dependencyExpressionArchiveSha256") ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         Text(candidate, "dependencyExpressionDllSha256") ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         Text(candidate, "sharedNugetPackageVersion") == "20260.20261026.1010" &&
         Text(candidate, "sharedNugetDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Text(candidate, "feedSelector") == "NUGET_LOCAL" &&
         candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 2 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
         candidate.GetProperty("consumerChecks").GetInt32() == 10 &&
         Flag(candidate, "sourceProjectUnmodified") &&
         Flag(candidate, "temporaryPackagingProjection") &&
         Flag(candidate, "originalFiveSourceFilesLinked") &&
         Flag(candidate, "sourceFeedInventoryUnchanged") &&
         Flag(candidate, "independentlyCompiledBinaryConsumer") &&
         !Flag(candidate, "published") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "publicLicenseSelected") &&
         !Flag(candidate, "installedRollbackQualified") &&
         Hash64(Text(candidate, "candidateArchiveSha256")) &&
         Hash64(Text(candidate, "candidateDllSha256")) &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 0,
         "Standard exact dependency closure, source provenance or binary consumer invalid");
    var version = Text(candidate, "packageVersion");
    Need(Regex.IsMatch(version, @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
             RegexOptions.CultureInvariant) &&
         int.TryParse(version.Split('.')[2], out var ordinal) &&
         ordinal is >= 1001 and <= 1999 &&
         Text(candidate, "candidateArchivePath") ==
            "build/artifacts/rs141-standard-candidate/candidate/dw.quantities.standard." +
            version + ".nupkg",
         "candidate calendar version or private package path invalid");
    var projectFingerprint = Text(candidate, "originalProjectSha256");
    Need(projectFingerprint == Hash(SourceProject),
         "Standard source project changed after candidate qualification");
    var sourceHashes = candidate.GetProperty("originalSourcesSha256");
    var names = new[]
    {
        "ExplicitProfileExpressionUnitResolver.cs", "InheritedStandardUnitCatalog.cs",
        "PureUnitConverter.cs", "StandardExpressionUnitResolver.cs", "StandardUnitCatalog.cs"
    };
    Need(sourceHashes.EnumerateObject().Count() == names.Length,
         "only the five original Standard source files are admissible");
    foreach (var name in names)
    {
        var source = "src/projects/dw.quantities.standard/" + name;
        Need(Text(sourceHashes, source) == Hash(source),
             "Standard linked source changed: " + source);
    }
    var expected = new[]
    {
        "f1-inventory-before", "standard-linked-restore",
        "standard-linked-build", "standard-candidate-pack",
        "consumer-local-only-restore", "consumer-release-build",
        "consumer-binary-run", "f1-inventory-after", "sdk"
    };
    var inner = candidate.GetProperty("commands").EnumerateArray().ToArray();
    Need(inner.Length == expected.Length &&
         inner.Select(x => Text(x, "id")).SequenceEqual(expected) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "actual Standard source/consumer process receipts are incomplete");
    var outerExpected = new[]
    {
        "f1-restore", "f1-build", "tool-restore",
        "tool-build", "standard-candidate", "sdk"
    };
    var outer = receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outer.Length == outerExpected.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerExpected) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "Standard candidate tool was not compiled and run independently");
}
Console.WriteLine(qualified ? Target : Baseline);
Console.WriteLine(Refusal);
return 0;
