#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f27-statistics-published-qualified.json";
const string Status = "rs160-statistics-published-local-two-binaries-independently-verified";
const string Ready = "MATH-STATISTICS-F27:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F27:PHYSICAL=VERIFIED;BINARY=18;DEPENDENCY=1;NATIVE=541";
const string Limits = "MATH-STATISTICS-F27:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";
var files = new[]
{
    "docs/distribution/nuget-federation-f27-statistics-published.md",
    "docs/distribution/ValidateNugetStatisticsPublishedFeed.cs",
    "tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj",
    "tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs"
};
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS160 Statistics readback: " + reason);
}
static JsonDocument Read(string file) => JsonDocument.Parse(File.ReadAllText(file));
static string Text(JsonElement element, string name) =>
    element.GetProperty(name).GetString() ??
    throw new InvalidDataException("missing expected " + name);
static bool Flag(JsonElement element, string name) =>
    element.GetProperty(name).GetBoolean();
static string Sha(string file) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is admitted");
foreach (var file in files)
    Need(File.Exists(file), "missing staged source " + file);

using var predecessor = Read(".aura/workflow/reports/RS159/attempt-001.json");
using var candidate = Read("docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json");
using var rehearsal = Read("docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json");
using var correction = Read("docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json");
using var baseReceipt = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
using var guidance = Read("docs/workflow/manifest.json");

var run = predecessor.RootElement;
Need(Text(run, "runId") == "RS159" &&
     run.GetProperty("attempt").GetInt32() == 1 &&
     Text(run, "outcome") == "succeeded" &&
     Text(run.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     run.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(run.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(run.GetProperty("git"), "commitMessage") ==
         "fix: correct Statistics immutable private publication receipt field",
     "RS159 first-pass publisher rehearsal result not retained");
var original = candidate.RootElement.GetProperty("candidate");
Need(Text(candidate.RootElement, "status") ==
         "rs157-statistics-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(candidate.RootElement, "package_published") &&
     Text(original, "packageId") == "dw.tools.math.statistics" &&
     Text(original, "packageVersion") == "20260.20261106.1001" &&
     original.GetProperty("candidateArchiveBytes").GetInt32() == 17451 &&
     Text(original, "candidateArchiveSha256") ==
         "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8" &&
     Text(original, "candidateDllSha256") ==
         "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96" &&
     original.GetProperty("packageDependencies").GetInt32() == 1 &&
     original.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 18 &&
     Text(original, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(original, "dependencyQuantitiesArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(original, "dependencyQuantitiesDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
     "original RS157 Statistics candidate identity changed");
Need(Text(rehearsal.RootElement, "status") ==
         "rs158-statistics-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsal.RootElement, "package_published") &&
     Text(rehearsal.RootElement.GetProperty("rehearsal"),
         "rs157CandidateArchiveSha256") ==
         "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8" &&
     Text(baseReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseReceipt.RootElement, "physically_published_package_verified"),
     "the publisher rehearsal or published base prerequisite is missing");

var corrected = correction.RootElement;
Need(Text(corrected, "status") ==
         "rs159-statistics-private-publication-receipt-corrected-unpublished" &&
     Flag(corrected, "release_controller_compiled") &&
     !Flag(corrected, "release_modes_invoked") &&
     !Flag(corrected, "package_published_by_this_run") &&
     Text(corrected, "original_archive_sha256") ==
         "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8" &&
     Text(corrected, "original_dll_sha256") ==
         "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96",
     "F26 corrected original private publisher provenance missing");
const string CurrentPublisher = "tools/dw.tools.math.nuget-statistics-release/Program.cs";
const string HistoricalPublisher =
    ".workflow/runs/RS158/staged/tools/dw.tools.math.nuget-statistics-release/Program.cs";
foreach (var hashes in new[] {
    candidate.RootElement.GetProperty("source_files"),
    correction.RootElement.GetProperty("source_files")
})
    foreach (var item in hashes.EnumerateObject())
        Need(File.Exists(item.Name) && Sha(item.Name) == item.Value.GetString(),
             "current Math source has drifted: " + item.Name);
foreach (var item in rehearsal.RootElement.GetProperty("source_files").EnumerateObject())
{
    var inspected = item.Name == CurrentPublisher ? HistoricalPublisher : item.Name;
    Need(File.Exists(inspected) && Sha(inspected) == item.Value.GetString(),
         "F25 original historical publisher source changed: " + item.Name);
}
Need(Text(corrected, "historical_controller_source_sha256") ==
         Text(rehearsal.RootElement.GetProperty("source_files"), CurrentPublisher) &&
     Text(corrected, "corrected_controller_source_sha256") == Sha(CurrentPublisher),
     "F25 historical and F26 corrected publisher SHA-256 lineage must be exact");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "native 541-node roadmap or guidance differs");

const string Tool = "tools/dw.tools.math.nuget-statistics-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-statistics-feed-verify.csproj")
         .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     !File.ReadAllText(Tool + "dw.tools.math.nuget-statistics-feed-verify.csproj")
         .Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("MATH-STATISTICS-BINARY|PASS|18", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("physical-feed-snapshot", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("publicationInvoked = false", StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs"),
     "standalone BCL physical-feed inspector must preserve exact oracle");
var finished = File.Exists(Evidence);
if (finished)
{
    using var resultDocument = Read(Evidence);
    var result = resultDocument.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.statistics" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(result, "physically_published_package_verified") &&
         Flag(result, "dependency_package_verified") &&
         !Flag(result, "package_published_by_this_run") &&
         !Flag(result, "remote_registry_published") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "discovery_migration_qualified"),
         "RS160 target cannot assert publication or other unqualified capabilities");
    foreach (var file in files)
        Need(Text(result.GetProperty("source_files"), file) == Sha(file),
             "staged verifier source SHA differs: " + file);

    var observed = result.GetProperty("observation");
    Need(Text(observed, "status") ==
             "statistics-published-local-two-binaries-independently-verified" &&
         Text(observed, "packageId") == "dw.tools.math.statistics" &&
         Text(observed, "packageVersion") == "20260.20261106.1001" &&
         observed.GetProperty("archiveBytes").GetInt32() == 17451 &&
         Text(observed, "archiveSha256") ==
             "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8" &&
         Text(observed, "packagedDllSha256") ==
             "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96" &&
         Text(observed, "loadedDllSha256") ==
             "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 1 &&
         observed.GetProperty("publishedPackageSnapshotCount").GetInt32() == 2 &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 18 &&
         Flag(observed, "independentPackageReference") &&
         Flag(observed, "physicalArchivesInspected") &&
         Flag(observed, "publishedArchivesUnchanged") &&
         !Flag(observed, "originalMathProjectReferencesUsed") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "installedPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified") &&
         Text(observed, "sourceFeed") == "NUGET_LOCAL" &&
         !string.IsNullOrWhiteSpace(Text(observed, "sdkVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "runtimeVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "osDescription")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "processArchitecture")),
         "Statistics private-feed physical and binary proof incomplete");
    var dependencies = observed.GetProperty("exactDependencies").EnumerateArray().ToArray();
    Need(dependencies.Length == 1 &&
         Text(dependencies[0], "id") == "dw.quantities" &&
         Text(dependencies[0], "version") == "20260.20261102.1001" &&
         dependencies[0].GetProperty("archiveBytes").GetInt32() == 14747 &&
         Text(dependencies[0], "archiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(dependencies[0], "packagedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(dependencies[0], "loadedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
         "original published Quantities dependency identity drifted");
    var inner = observed.GetProperty("commands").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 4 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id"))
             .SequenceEqual(new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" }) &&
         outer.Select(x => Text(x, "id"))
             .SequenceEqual(new[] { "tool-restore", "tool-build", "statistics-feed-verify", "sdk" }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "actual independent verifier or binary consumer receipts absent");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
