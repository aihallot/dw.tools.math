#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f7-expression-published-qualified.json";
const string Status = "rs140-expression-published-local-binary-independently-verified";
const string Ready = "MATH-EXPRESSION-F7:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541";
const string Qualified = "MATH-EXPRESSION-F7:PHYSICAL=VERIFIED;BINARY=6;NATIVE=541";
const string Boundary = "MATH-EXPRESSION-F7:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";

static void Need(bool ok, string explanation)
{
    if (!ok) throw new InvalidDataException("RS140 published Expression: " + explanation);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement item, string name) => item.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing string property " + name);
static bool Flag(JsonElement item, string name) => item.GetProperty(name).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var count = 1;
    foreach (var field in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(field, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is allowed");
var files = new[]
{
    "docs/distribution/nuget-federation-f7-expression-published.md",
    "docs/distribution/ValidateNugetExpressionPublishedFeed.cs",
    "tools/dw.tools.math.nuget-expression-feed-verify/dw.tools.math.nuget-expression-feed-verify.csproj",
    "tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props",
    "tools/dw.tools.math.nuget-expression-feed-verify/Program.cs"
};
foreach (var file in files)
    Need(File.Exists(file), "a staged RS140 source file is missing: " + file);

using var history = Read(".aura/workflow/reports/RS139/attempt-001.json");
using var original = Read("docs/distribution/nuget-federation-f5-expression-candidate-qualified.json");
using var publisher = Read("docs/distribution/nuget-federation-f6-expression-publisher-qualified.json");
using var publishedBase = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var planning = Read("docs/planning/backlog.json");
using var native = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

Need(Text(history.RootElement, "runId") == "RS139" &&
     history.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(history.RootElement, "outcome") == "succeeded" &&
     Text(history.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     history.RootElement.GetProperty("priorInvocationFailures").GetProperty("count")
         .GetInt32() == 0 &&
     Flag(history.RootElement.GetProperty("priorInvocationFailures"),
         "firstPassSucceeded") &&
     Text(history.RootElement.GetProperty("git"), "commitMessage") ==
         "test: qualify Expression immutable local NuGet publisher rehearsal",
     "RS139 must be the exact successful first-pass predecessor");

Need(Text(original.RootElement, "status") ==
         "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished" &&
     !Flag(original.RootElement, "package_published") &&
     Text(publisher.RootElement, "status") ==
         "rs139-expression-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(publisher.RootElement, "package_published") &&
     Text(publishedBase.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(publishedBase.RootElement, "physically_published_package_verified"),
     "historical candidate/rehearsal and published prerequisite states changed");

var originalCandidate = original.RootElement.GetProperty("candidate");
Need(Text(originalCandidate, "packageId") == "dw.quantities.expression" &&
     Text(originalCandidate, "packageVersion") == "20260.20261102.1001" &&
     Text(originalCandidate, "candidateArchiveSha256") ==
         "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
     originalCandidate.GetProperty("candidateArchiveBytes").GetInt32() == 13951 &&
     Text(originalCandidate, "candidateDllSha256") ==
         "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
     Text(originalCandidate, "exactBasePackageVersion") == "20260.20261102.1001" &&
     Text(originalCandidate, "exactBasePackageSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(originalCandidate, "exactBaseDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     originalCandidate.GetProperty("packagedDependencies").GetInt32() == 1 &&
     originalCandidate.GetProperty("consumerChecks").GetInt32() == 6,
     "RS138 exact binary identities and package dependency closure changed");

foreach (var historical in new[]
{
    original.RootElement.GetProperty("source_files"),
    publisher.RootElement.GetProperty("source_files")
})
    foreach (var item in historical.EnumerateObject())
        Need(File.Exists(item.Name) && Hash(item.Name) == item.Value.GetString(),
             "previously qualified source file changed: " + item.Name);

Need(Text(planning.RootElement, "status") == "done" &&
     Text(planning.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     native.RootElement.GetProperty("milestones").EnumerateArray()
         .Sum(Count) == 541 &&
     native.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "accepted Math product roadmap or executable DWF state changed");

const string Tool = "tools/dw.tools.math.nuget-expression-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-expression-feed-verify.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "NuGet reader must be an exact published, central PackageReference");

var finished = File.Exists(Evidence);
if (finished)
{
    using var receipt = Read(Evidence);
    var record = receipt.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.quantities.expression" &&
         Text(record, "feed_selector") == "NUGET_LOCAL" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(record, "physically_published_package_verified") &&
         Flag(record, "dependency_package_verified") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "remote_registry_published") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "the independent qualification receipt must not claim a release or unapproved permissions");
    var sourceHashes = record.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(sourceHashes, file) == Hash(file),
             "RS140 staged source SHA-256 differs from the receipt: " + file);

    var observed = record.GetProperty("observation");
    Need(Text(observed, "status") ==
            "expression-published-local-binary-verified-read-only" &&
         Text(observed, "packageId") == "dw.quantities.expression" &&
         Text(observed, "packageVersion") == "20260.20261102.1001" &&
         Text(observed, "archiveSha256") ==
            "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         observed.GetProperty("archiveBytes").GetInt32() == 13951 &&
         Text(observed, "packagedDllSha256") ==
            "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         Text(observed, "loadedDllSha256") ==
            "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         Text(observed, "exactDependencyPackageId") == "dw.quantities" &&
         Text(observed, "exactDependencyPackageVersion") == "20260.20261102.1001" &&
         Text(observed, "exactDependencyArchiveSha256") ==
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed.GetProperty("exactDependencyArchiveBytes").GetInt32() == 14747 &&
         Text(observed, "packagedDependencyDllSha256") ==
            "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(observed, "loadedDependencyDllSha256") ==
            "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observed.GetProperty("packageDependencyCount").GetInt32() == 1 &&
         Text(observed, "sharedNugetVersion") == "20260.20261026.1010" &&
         Text(observed, "sharedNugetDllSha256") ==
            "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Text(observed, "feedSelector") == "NUGET_LOCAL" &&
         observed.GetProperty("packageReaderSnapshotCount").GetInt32() == 2 &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 6 &&
         Flag(observed, "actualPhysicalArchiveVerified") &&
         Flag(observed, "independentPackageReference") &&
         Flag(observed, "publishedArchiveUnchanged") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "sourceProjectReferencesUsed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "installedPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified"),
         "real published Expression and transitive base DLL observations are incomplete");

    var inner = observed.GetProperty("commands").EnumerateArray().ToArray();
    var expectedInner = new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" };
    Need(inner.Length == expectedInner.Length &&
         inner.Select(x => Text(x, "id")).SequenceEqual(expectedInner) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "a required isolated consumer process was not executed successfully");
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    var expectedOuter = new[] { "tool-restore", "tool-build", "expression-feed-verify", "sdk" };
    Need(outer.Length == expectedOuter.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(expectedOuter) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "RS140 must build and execute the independent verification tool");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Boundary);
return 0;
