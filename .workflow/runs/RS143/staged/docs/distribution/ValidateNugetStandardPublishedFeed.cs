#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f10-standard-published-qualified.json";
const string Status = "rs143-standard-published-local-three-binaries-independently-verified";
const string Ready = "MATH-STANDARD-F10:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541";
const string Qualified = "MATH-STANDARD-F10:PHYSICAL=VERIFIED;BINARY=10;DEPENDENCIES=2;NATIVE=541";
const string Limits = "MATH-STANDARD-F10:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f10-standard-published.md",
    "docs/distribution/ValidateNugetStandardPublishedFeed.cs",
    "tools/dw.tools.math.nuget-standard-feed-verify/dw.tools.math.nuget-standard-feed-verify.csproj",
    "tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props",
    "tools/dw.tools.math.nuget-standard-feed-verify/Program.cs"
};
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS143 Standard readback: " + reason);
}
static JsonDocument ReadJson(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string field) => node.GetProperty(field).GetString() ??
    throw new InvalidDataException("Missing exact field " + field);
static bool Flag(JsonElement node, string field) => node.GetProperty(field).GetBoolean();
static string Sha(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var amount = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var children))
            amount += children.EnumerateArray().Sum(Count);
    return amount;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is admitted");
foreach (var path in files)
    Need(File.Exists(path), "missing source file " + path);

using var predecessor = ReadJson(".aura/workflow/reports/RS142/attempt-001.json");
using var candidateSource = ReadJson("docs/distribution/nuget-federation-f8-standard-candidate-qualified.json");
using var rehearsal = ReadJson("docs/distribution/nuget-federation-f9-standard-publisher-qualified.json");
using var expressionReceipt = ReadJson("docs/distribution/nuget-federation-f7-expression-published-qualified.json");
using var baseReceipt = ReadJson("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var backlog = ReadJson("docs/planning/backlog.json");
using var plan = ReadJson(".aura/workflow/plan/project.json");
using var guidance = ReadJson("docs/workflow/manifest.json");

var predecessorRoot = predecessor.RootElement;
Need(Text(predecessorRoot, "runId") == "RS142" &&
     predecessorRoot.GetProperty("attempt").GetInt32() == 1 &&
     Text(predecessorRoot, "outcome") == "succeeded" &&
     Text(predecessorRoot.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     predecessorRoot.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(predecessorRoot.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(predecessorRoot.GetProperty("git"), "commitMessage") ==
         "test: qualify Standard immutable local NuGet publisher rehearsal",
     "RS142 must be the exact successful first-pass DWF predecessor");
Need(Text(candidateSource.RootElement, "status") ==
         "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished" &&
     !Flag(candidateSource.RootElement, "package_published") &&
     Text(rehearsal.RootElement, "status") ==
         "rs142-standard-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsal.RootElement, "package_published") &&
     Text(expressionReceipt.RootElement, "status") ==
         "rs140-expression-published-local-binary-independently-verified" &&
     Flag(expressionReceipt.RootElement, "physically_published_package_verified") &&
     Flag(expressionReceipt.RootElement, "dependency_package_verified") &&
     Text(baseReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseReceipt.RootElement, "physically_published_package_verified"),
     "prior candidate, rehearsal and dependency published receipts are not retained");
var candidate = candidateSource.RootElement.GetProperty("candidate");
Need(Text(candidate, "packageId") == "dw.quantities.standard" &&
     Text(candidate, "packageVersion") == "20260.20261103.1001" &&
     Text(candidate, "candidateArchiveSha256") ==
         "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13604 &&
     Text(candidate, "candidateDllSha256") ==
         "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
     candidate.GetProperty("consumerChecks").GetInt32() == 10 &&
     Text(candidate, "dependencyExpressionVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyExpressionArchiveSha256") ==
         "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyQuantitiesArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     "original RS141 candidate bytes or two exact dependencies changed");
var rehearsalResult = rehearsal.RootElement.GetProperty("rehearsal");
Need(Text(rehearsalResult, "rs141ArchiveSha256") ==
         "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
     rehearsalResult.GetProperty("disposableFeedPublished").GetBoolean() &&
     !rehearsalResult.GetProperty("publishedToNugetLocal").GetBoolean(),
     "RS142 disposable-feed evidence may not claim real shared publication");
foreach (var historical in new[]
{
    candidateSource.RootElement.GetProperty("source_files"),
    rehearsal.RootElement.GetProperty("source_files")
})
    foreach (var source in historical.EnumerateObject())
        Need(File.Exists(source.Name) && Sha(source.Name) == source.Value.GetString(),
             "prior source-pinned file changed: " + source.Name);
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "Math accepted product roadmap or native state drifted");
const string Tool = "tools/dw.tools.math.nuget-standard-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-standard-feed-verify.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "NuGet shared reader must be a centrally pinned exact PackageReference");

var finished = File.Exists(Evidence);
if (finished)
{
    using var receipt = ReadJson(Evidence);
    var result = receipt.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.quantities.standard" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(result, "physically_published_package_verified") &&
         Flag(result, "transitive_packages_verified") &&
         !Flag(result, "package_published_by_this_run") &&
         !Flag(result, "remote_registry_published") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "DWF readback cannot assert new publication, licensing or migration");
    var sourceHashes = result.GetProperty("source_files");
    foreach (var path in files)
        Need(Text(sourceHashes, path) == Sha(path),
             "RS143 staged source changed after qualification: " + path);
    var observed = result.GetProperty("observation");
    Need(Text(observed, "status") ==
             "standard-published-local-three-binaries-independently-verified" &&
         Text(observed, "packageId") == "dw.quantities.standard" &&
         Text(observed, "packageVersion") == "20260.20261103.1001" &&
         Text(observed, "archiveSha256") ==
             "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
         observed.GetProperty("archiveBytes").GetInt32() == 13604 &&
         Text(observed, "packagedDllSha256") ==
             "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         Text(observed, "loadedDllSha256") ==
             "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 2 &&
         observed.GetProperty("publishedPackageSnapshotCount").GetInt32() == 3 &&
         Text(observed, "sharedNugetPackageVersion") == "20260.20261026.1010" &&
         Text(observed, "sharedNugetDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Text(observed, "sourceFeed") == "NUGET_LOCAL" &&
         Text(observed, "consumerSource") ==
             "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs" &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 10 &&
         Flag(observed, "independentPackageReference") &&
         Flag(observed, "physicalArchivesInspected") &&
         Flag(observed, "publishedArchivesUnchanged") &&
         !Flag(observed, "originalMathProjectReferencesUsed") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "installedPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified"),
         "physical private-feed Standard and three-DLL binary proof incomplete");

    var dependencies = observed.GetProperty("exactDependencies").EnumerateArray().ToArray();
    Need(dependencies.Length == 2 &&
         Text(dependencies[0], "id") == "dw.quantities" &&
         Text(dependencies[0], "version") == "20260.20261102.1001" &&
         Text(dependencies[0], "archiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         dependencies[0].GetProperty("archiveBytes").GetInt32() == 14747 &&
         Text(dependencies[0], "packagedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(dependencies[0], "loadedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(dependencies[1], "id") == "dw.quantities.expression" &&
         Text(dependencies[1], "version") == "20260.20261102.1001" &&
         Text(dependencies[1], "archiveSha256") ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         dependencies[1].GetProperty("archiveBytes").GetInt32() == 13951 &&
         Text(dependencies[1], "packagedDllSha256") ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         Text(dependencies[1], "loadedDllSha256") ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1",
         "the exact two-dependency published DLL closure changed");
    var inner = observed.GetProperty("commands").EnumerateArray().ToArray();
    var innerNames = new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" };
    Need(inner.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(innerNames) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "ten-case actual isolated consumer or SDK commands missing");
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    var outerNames = new[] { "tool-restore", "tool-build", "standard-feed-verify", "sdk" };
    Need(outer.Length == 4 &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerNames) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "DWF did not compile and execute the new independent tool");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
