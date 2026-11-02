#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
const string Status = "rs137-published-local-math-nuget-binary-independently-verified";
const string Success = "MATH-NUGET-F4:SHARED-FEED=VERIFIED;BINARY=4;NATIVE=541";
const string Limits = "MATH-NUGET-F4:REPUBLICATION=false;PUBLIC-REGISTRY=false;ROLLBACK=false";

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS137 local-feed qualification: " + message);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement element, string key) => element.GetProperty(key).GetString() ??
    throw new InvalidDataException("Missing string value " + key);
static bool Flag(JsonElement element, string key) => element.GetProperty(key).GetBoolean();
static string Sha(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is supported");
var files = new[]
{
    "docs/distribution/nuget-federation-f4-quantities-published.md",
    "docs/distribution/ValidateNugetQuantitiesPublishedFeed.cs",
    "tools/dw.tools.math.nuget-feed-verify/dw.tools.math.nuget-feed-verify.csproj",
    "tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props",
    "tools/dw.tools.math.nuget-feed-verify/Program.cs"
};
foreach (var file in files)
    Need(File.Exists(file), "missing declared source file " + file);

using var rs135 = Read(".aura/workflow/reports/RS135/attempt-001.json");
using var rs136 = Read(".aura/workflow/reports/RS136/attempt-001.json");
using var candidate = Read("docs/distribution/nuget-federation-f2-quantities-qualified.json");
using var rehearsal = Read("docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
Need(Text(rs135.RootElement, "runId") == "RS135" &&
     Text(rs135.RootElement, "outcome") == "succeeded" &&
     rs135.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Flag(rs135.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded"),
     "original RS135 package creation run is not a successful first pass");
Need(Text(rs136.RootElement, "runId") == "RS136" &&
     Text(rs136.RootElement, "outcome") == "succeeded" &&
     rs136.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Flag(rs136.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(rs136.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     Text(rs136.RootElement.GetProperty("git"), "commitMessage") ==
         "test: qualify Math immutable local NuGet publisher rehearsal",
     "original RS136 publisher qualification changed");
Need(Text(candidate.RootElement, "status") ==
     "rs135-quantities-calendar-candidate-qualified-unpublished" &&
     Text(rehearsal.RootElement, "status") ==
     "rs136-math-immutable-local-nuget-publisher-rehearsal-qualified-unpublished" &&
     !Flag(candidate.RootElement, "package_published") &&
     !Flag(rehearsal.RootElement, "package_published"),
     "the original candidate and rehearsal receipts must remain historical");
var identity = candidate.RootElement.GetProperty("candidate");
Need(Text(identity, "sourcePackageId") == "dw.quantities" &&
     Text(identity, "packageVersion") == "20260.20261102.1001" &&
     Text(identity, "candidateArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(identity, "candidateDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     identity.GetProperty("candidateArchiveBytes").GetInt64() == 14747,
     "original RS135 package bytes changed");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "the accepted Math native plan/backlog or guidance changed");

const string Tool = "tools/dw.tools.math.nuget-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-feed-verify.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "NuGet shared read-only reader is not referenced by exact package identity");
var hasReceipt = File.Exists(Evidence);
if (hasReceipt)
{
    using var receipt = Read(Evidence);
    var record = receipt.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.quantities" &&
         Text(record, "feed_selector") == "NUGET_LOCAL" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(record, "physically_published_package_verified") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "remote_registry_published") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "receipt claims unauthorized publication, license, rollback or migration");
    var sources = record.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(sources, file) == Sha(file),
             "RS137 staged source SHA-256 mismatch: " + file);
    var observed = record.GetProperty("observation");
    Need(Text(observed, "status") == "published-local-binary-verified-read-only" &&
         Text(observed, "packageId") == "dw.quantities" &&
         Text(observed, "packageVersion") == "20260.20261102.1001" &&
         Text(observed, "archiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed.GetProperty("archiveBytes").GetInt32() == 14747 &&
         Text(observed, "packagedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(observed, "loadedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(observed, "sharedNugetPackageVersion") == "20260.20261026.1010" &&
         Text(observed, "sharedNugetDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Text(observed, "sourceFeed") == "NUGET_LOCAL" &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 4 &&
         Flag(observed, "independentPackageReference") &&
         Flag(observed, "physicalArchiveInspected") &&
         Flag(observed, "publishedArchiveUnchanged") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "physicalInstalledPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified"),
         "actual independent feed and binary observation is incomplete");
    var commands = observed.GetProperty("commands").EnumerateArray().ToArray();
    var expected = new[] { "consumer-restore", "consumer-build", "consumer-execute", "sdk" };
    Need(commands.Length == 4 &&
         commands.Select(c => Text(c, "id")).SequenceEqual(expected) &&
         commands.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "internal independently compiled consumer was not executed");
    var parent = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    var parentIds = new[] { "tool-restore", "tool-build", "published-feed-verify", "sdk" };
    Need(parent.Length == 4 &&
         parent.Select(c => Text(c, "id")).SequenceEqual(parentIds) &&
         parent.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "the new published-feed executable was not actually built and run");
}
Console.WriteLine(hasReceipt ? Success :
    "MATH-NUGET-F4:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541");
Console.WriteLine(Limits);
return 0;
