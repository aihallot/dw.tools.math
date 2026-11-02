#:property NuGetAudit=false
#:property PublishAot=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json";
const string Status = "rs136-math-immutable-local-nuget-publisher-rehearsal-qualified-unpublished";
const string Positive = "MATH-NUGET-F3:RS135=FIRST-PASS;PUBLISHER=DISPOSABLE;CONTROLS=6;NATIVE=541";
const string Refusal = "MATH-NUGET-F3:SHARED-PUBLICATION=false;PHYSICAL-ROLLBACK=false;EXTERNAL=false";

static void Need(bool ok, string message)
{
    if (!ok) throw new InvalidDataException("RS136 publication boundary: " + message);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement element, string name) =>
    element.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing owner identity " + name);
static bool Bool(JsonElement element, string name) => element.GetProperty(name).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static bool Hash64(string value) => Regex.IsMatch(value, "^[0-9a-f]{64}$",
    RegexOptions.CultureInvariant);
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is allowed");
var files = new[]
{
    "docs/distribution/nuget-federation-f3-quantities-publisher.md",
    "docs/distribution/ValidateNugetQuantitiesReleaseController.cs",
    "tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj",
    "tools/dw.tools.math.nuget-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-release/Program.cs"
};
foreach (var file in files) Need(File.Exists(file), "missing full staged source " + file);
using var rs135 = Read(".aura/workflow/reports/RS135/attempt-001.json");
using var candidateOwner = Read("docs/distribution/nuget-federation-f2-quantities-qualified.json");
using var rs134 = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var native = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
using var guidance = Read("docs/workflow/manifest.json");
Need(Text(rs135.RootElement, "runId") == "RS135" &&
     rs135.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(rs135.RootElement, "outcome") == "succeeded" &&
     Text(rs135.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     rs135.RootElement.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Bool(rs135.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(rs135.RootElement.GetProperty("git"), "commitMessage") ==
         "test: qualify dw.quantities calendar NuGet binary candidate",
     "real first-pass RS135 DWF report changed");
Need(Text(candidateOwner.RootElement, "status") ==
     "rs135-quantities-calendar-candidate-qualified-unpublished" &&
     Text(rs134.RootElement, "status") ==
     "rs134-math-f1-calendar-feed-inventory-no-publication" &&
     !Bool(candidateOwner.RootElement, "package_published") &&
     !Bool(rs134.RootElement, "package_published"),
     "earlier owner F1/F2 qualification was promoted prematurely");
var input = candidateOwner.RootElement.GetProperty("candidate");
Need(Text(input, "sourcePackageId") == "dw.quantities" &&
     Text(input, "packageVersion") == "20260.20261102.1001" &&
     Text(input, "candidateArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(input, "candidateDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     input.GetProperty("candidateArchiveBytes").GetInt64() == 14747 &&
     input.GetProperty("consumerChecks").GetInt32() == 4 &&
     Bool(input, "compiledSourceFreeConsumerPassed") &&
     !Bool(input, "published"),
     "RS135 exact physical candidate evidence changed");
foreach (var owned in candidateOwner.RootElement.GetProperty("source_files").EnumerateObject())
{
    Need(Sha(owned.Name) == owned.Value.GetString(),
         "RS135 source-pinned file was modified " + owned.Name);
}
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     native.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     native.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "Math product roadmap or workflow guidance changed");
var localProject = File.ReadAllText("tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj");
var central = File.ReadAllText("tools/dw.tools.math.nuget-release/Directory.Packages.props");
Need(localProject.Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
         StringComparison.Ordinal) &&
     central.Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
         StringComparison.Ordinal),
     "shared publisher PackageReference is not centrally and exactly pinned");

if (File.Exists(Evidence))
{
    using var document = Read(Evidence);
    var receipt = document.RootElement;
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.quantities" &&
         Text(receipt, "feed_selector") == "NUGET_LOCAL" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Bool(receipt, "package_published") &&
         !Bool(receipt, "release_authorized_by_this_run") &&
         !Bool(receipt, "installed_package_rollback_qualified") &&
         !Bool(receipt, "public_license_selected") &&
         !Bool(receipt, "source_versions_modified") &&
         !Bool(receipt, "external_requests_transmitted") &&
         !Bool(receipt, "discovery_migration_qualified"),
         "RS136 receipt may not claim shared-feed release, rights or rollback");
    var sources = receipt.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(sources, file) == Sha(file), "RS136 staged source hash drifted: " + file);
    var actual = receipt.GetProperty("rehearsal");
    Need(Text(actual, "status") == "real-math-package-disposable-publisher-qualified" &&
         Text(actual, "packageId") == "dw.quantities" &&
         Text(actual, "packageVersion") == "20260.20261102.1001" &&
         Text(actual, "sourceArchiveSha256") ==
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(actual, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(actual, "publisherDllSha256") ==
            "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Hash64(Text(actual, "rehearsalArchiveSha256")) &&
         Hash64(Text(actual, "rehearsalDllSha256")) &&
         actual.GetProperty("controls").GetInt32() == 6 &&
         Bool(actual, "publishedInDisposableFeed") &&
         Bool(actual, "collisionRefused") &&
         Bool(actual, "alteredHashRefused") &&
         Bool(actual, "cancellationRefused") &&
         !Bool(actual, "candidateIsPublishedToSharedFeed") &&
         !Bool(actual, "sharedFeedTouched") &&
         !Bool(actual, "publicationModeEnabledByDwf") &&
         !Bool(actual, "physicalRollbackQualified"),
         "real-package publisher rehearsal and refusal cases are incomplete");
    var ids = new[] { "source-restore", "source-build", "source-pack" };
    var commandReceipts = actual.GetProperty("commands").EnumerateArray().ToArray();
    Need(commandReceipts.Length == ids.Length &&
         commandReceipts.Select(x => Text(x, "id")).SequenceEqual(ids) &&
         commandReceipts.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "rehearsal did not build and pack a real Math DLL");
    var outerIds = new[] { "tool-restore", "tool-build", "publisher-rehearsal", "sdk" };
    var outerCommands = receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outerCommands.Length == outerIds.Length &&
         outerCommands.Select(x => Text(x, "id")).SequenceEqual(outerIds) &&
         outerCommands.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "release-controller compilation or rehearsal was not observed");
}
Console.WriteLine(Positive);
Console.WriteLine(Refusal);
return 0;
