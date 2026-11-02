#:property NuGetAudit=false
#:property PublishAot=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Receipt = "docs/distribution/nuget-federation-f5-expression-candidate-qualified.json";
const string Status = "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished";
const string Baseline = "MATH-EXPRESSION-F5:SOURCE=READY;DEPENDENCY=PINNED;NATIVE=541";
const string Target = "MATH-EXPRESSION-F5:CANDIDATE=QUALIFIED;BINARY=6;NATIVE=541";
const string Limits = "MATH-EXPRESSION-F5:PUBLISHED=false;PUBLIC-LICENSE=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f5-expression-candidate.md",
    "docs/distribution/ValidateNugetExpressionCandidate.cs",
    "tools/dw.tools.math.nuget-expression-candidate/dw.tools.math.nuget-expression-candidate.csproj",
    "tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-expression-candidate/Program.cs",
    "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs"
};
static void Need(bool ok, string detail)
{
    if (!ok) throw new InvalidOperationException("RS138 Expression NuGet: " + detail);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string field) => node.GetProperty(field).GetString() ??
    throw new InvalidDataException("Missing source identity field: " + field);
static bool Flag(JsonElement node, string name) => node.GetProperty(name).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var kind in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(kind, out var childNodes))
            total += childNodes.EnumerateArray().Sum(Count);
    return total;
}
static bool Hash64(string hash) => Regex.IsMatch(hash,
    "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is supported");
foreach (var file in files)
    Need(File.Exists(file), "missing staged Expression source: " + file);
using var prev = Read(".aura/workflow/reports/RS137/attempt-001.json");
using var first = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var published = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
using var guidance = Read("docs/workflow/manifest.json");
Need(Text(prev.RootElement, "runId") == "RS137" &&
     Text(prev.RootElement, "outcome") == "succeeded" &&
     prev.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(prev.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prev.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("count").GetInt32() == 0 &&
     Flag(prev.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prev.RootElement.GetProperty("git"), "commitMessage") ==
         "test: verify published dw.quantities binary from local NuGet feed",
     "RS137 first-pass successful DWF lineage changed");
Need(Text(first.RootElement, "status") ==
     "rs134-math-f1-calendar-feed-inventory-no-publication" &&
     !Flag(first.RootElement, "package_published") &&
     Text(published.RootElement, "status") ==
     "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(published.RootElement, "physically_published_package_verified") &&
     !Flag(published.RootElement, "package_published_by_this_run"),
     "earlier source/product NuGet archive qualification absent");
var input = published.RootElement.GetProperty("observation");
Need(Text(input, "packageId") == "dw.quantities" &&
     Text(input, "packageVersion") == "20260.20261102.1001" &&
     Text(input, "archiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(input, "loadedDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     input.GetProperty("compiledConsumerChecks").GetInt32() == 4,
     "immutable published base Quantities binary drift");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones")
         .EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones")
         .EnumerateArray().All(n => Text(n, "state") == "done"),
     "Math historical 541-node native product scope changed");

const string SourceProject =
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj";
Need(File.ReadAllText(SourceProject).Contains(
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />",
    StringComparison.Ordinal) &&
     File.ReadAllText(SourceProject).Contains(
    "<Version>0.2.0-preview.1</Version>", StringComparison.Ordinal),
    "the original source-to-source Expression project has changed");
const string Tool = "tools/dw.tools.math.nuget-expression-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-expression-candidate.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "shared NuGet repository reference must remain exact, scoped and central");

var complete = File.Exists(Receipt);
if (complete)
{
    using var doc = Read(Receipt);
    var result = doc.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.quantities.expression" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "source_package_versions_modified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "RS138 receiver cannot claim a release, licensing or broker migration");
    var provenance = result.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(provenance, file) == Hash(file),
             "RS138 staged source SHA changed: " + file);
    var q = result.GetProperty("candidate");
    Need(Text(q, "status") ==
             "qualified-local-dependent-candidate-not-published" &&
         Text(q, "packageId") == "dw.quantities.expression" &&
         Text(q, "historicalSourceVersion") == "0.2.0-preview.1" &&
         Text(q, "exactBasePackageId") == "dw.quantities" &&
         Text(q, "exactBasePackageVersion") == "20260.20261102.1001" &&
         Text(q, "exactBasePackageSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(q, "exactBaseDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(q, "sourceFeed") == "NUGET_LOCAL" &&
         Text(q, "sharedNugetPackageVersion") == "20260.20261026.1010" &&
         Text(q, "sharedNugetDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Hash64(Text(q, "candidateArchiveSha256")) &&
         Hash64(Text(q, "candidateDllSha256")) &&
         q.GetProperty("candidateArchiveBytes").GetInt64() > 0 &&
         q.GetProperty("packagedDependencies").GetInt32() == 1 &&
         q.GetProperty("sourceProjectReferencesRetained").GetInt32() == 1 &&
         q.GetProperty("consumerChecks").GetInt32() == 6 &&
         Flag(q, "temporaryPackagingProjection") &&
         Flag(q, "sourceProjectUnmodified") &&
         Flag(q, "sourceDllCompilationFromUnchangedSources") &&
         Flag(q, "sourceFeedInventoryUnchanged") &&
         Flag(q, "actualSourceFreeConsumerPassed") &&
         Flag(q, "privateCandidateFeedOnly") &&
         !Flag(q, "published") && !Flag(q, "versionReserved") &&
         !Flag(q, "publicLicenseSelected") &&
         !Flag(q, "installedRollbackQualified"),
         "qualified Expression package and binary dependency must retain exact identities");
    var version = Text(q, "packageVersion");
    Need(Regex.IsMatch(version, @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
             RegexOptions.CultureInvariant) &&
         int.TryParse(version.Split('.')[2], out var counter) &&
         counter is >= 1001 and <= 1999 &&
         Text(q, "candidateArchivePath") ==
            "build/artifacts/rs138-expression-candidate/candidate/dw.quantities.expression." +
            version + ".nupkg",
         "candidate version or immutable ignored archive path is incorrect");
    Need(Text(q, "originalProjectSha256") == Hash(SourceProject),
         "original source project bytes have changed after candidate creation");
    var sourceHashes = q.GetProperty("originalSourceSha256");
    var sourceNames = new[]
    {
        "ExactSelection.cs", "ExpressionDiagnostics.cs", "ExpressionParser.cs"
    };
    Need(sourceHashes.EnumerateObject().Count() == sourceNames.Length,
         "unexpected original compiled source count");
    foreach (var name in sourceNames)
    {
        var sourcePath = "src/projects/dw.quantities.expression/" + name;
        Need(Text(sourceHashes, sourcePath) == Hash(sourcePath),
             "original linked source SHA mismatch " + name);
    }
    var commandIds = new[]
    {
        "f1-feed-inspection-before", "linked-source-restore",
        "linked-source-build", "expression-candidate-pack",
        "consumer-local-only-restore", "consumer-release-build",
        "consumer-binary-run", "f1-feed-inspection-after", "sdk"
    };
    var commands = q.GetProperty("commands").EnumerateArray().ToArray();
    Need(commands.Length == commandIds.Length &&
         commands.Select(c => Text(c, "id"))
             .SequenceEqual(commandIds, StringComparer.Ordinal) &&
         commands.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "as-run linked-source/consumer commands incomplete");
    var outerIds = new[] { "f1-restore", "f1-build", "tool-restore",
                           "tool-build", "expression-candidate", "sdk" };
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outer.Length == outerIds.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerIds) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "DWF did not compile and execute the intended separate candidate controller");
}
Console.WriteLine(complete ? Target : Baseline);
Console.WriteLine(Limits);
return 0;
