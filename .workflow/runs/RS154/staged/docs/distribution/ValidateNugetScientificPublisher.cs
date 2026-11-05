#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json";
const string Status = "rs154-scientific-immutable-private-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-SCIENTIFIC-F21:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F21:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-SCIENTIFIC-F21:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";
var files = new[]
{
    "docs/distribution/nuget-federation-f21-scientific-publisher.md",
    "docs/distribution/ValidateNugetScientificPublisher.cs",
    "tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj",
    "tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-scientific-release/Program.cs"
};
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS154 Scientific publisher: " + reason);
}
static JsonDocument Load(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement element, string name) =>
    element.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing " + name);
static bool Flag(JsonElement element, string name) =>
    element.GetProperty(name).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check mode admitted");
foreach (var path in files)
    Need(File.Exists(path), "missing source " + path);
using var report = Load(".aura/workflow/reports/RS153/attempt-001.json");
using var candidateEvidence = Load("docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json");
using var priorPhysical = Load("docs/distribution/nuget-federation-f19-foundation-published-qualified.json");
using var backlog = Load("docs/planning/backlog.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var guidance = Load("docs/workflow/manifest.json");

var previous = report.RootElement;
var failures = previous.GetProperty("priorInvocationFailures");
Need(Text(previous, "runId") == "RS153" &&
     previous.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous, "outcome") == "succeeded" &&
     Text(previous.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     failures.GetProperty("count").GetInt32() == 0 &&
     failures.GetProperty("preparationFailureCount").GetInt32() == 0 &&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     Flag(failures, "firstPassSucceeded") &&
     Text(previous.GetProperty("git"), "commitMessage") ==
         "test: qualify Scientific calendar NuGet candidate without publication",
     "RS153 must be the observed successful first-pass predecessor");
var owner = candidateEvidence.RootElement;
var candidate = owner.GetProperty("candidate");
Need(Text(owner, "status") ==
         "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished" &&
     !Flag(owner, "package_published") &&
     Text(candidate, "status") ==
         "qualified-independent-scientific-calendar-candidate-not-published" &&
     Text(candidate, "packageId") == "dw.tools.math.scientific" &&
     Text(candidate, "packageVersion") == "20260.20261105.1001" &&
     Text(candidate, "historicalSourceVersion") == "1.1.0-preview.14" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 37265 &&
     Text(candidate, "candidateArchiveSha256") ==
         "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
     Text(candidate, "candidateDllSha256") ==
         "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Flag(candidate, "independentPackageReference"),
     "qualified original F20 Scientific source-free package identity has changed");
Need(Text(priorPhysical.RootElement, "status") ==
         "rs152-foundation-published-local-binary-independently-verified" &&
     Flag(priorPhysical.RootElement, "physically_published_package_verified") &&
     !Flag(priorPhysical.RootElement, "package_published_by_this_run"),
     "published prerequisite must be independently qualified");
foreach (var file in candidateEvidence.RootElement.GetProperty("source_files").EnumerateObject())
    Need(File.Exists(file.Name) && Sha(file.Name) == file.Value.GetString(),
         "RS153 source-pin drift: " + file.Name);
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(node => Text(node, "state") == "done"),
     "unchanged 541-node Math roadmap and guidance required");
const string Tool = "tools/dw.tools.math.nuget-scientific-release/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "DotNetLocalPackageRepository", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "MATH-SCIENTIFIC-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "RequireCleanOwnerAndRs154", StringComparison.Ordinal),
     "shared publisher and guarded owner modes must remain exact");

var finished = File.Exists(Evidence);
if (finished)
{
    using var receipt = Load(Evidence);
    var result = receipt.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.scientific" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "source_versions_modified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "DWF RS154 cannot claim actual shared-feed publication or license");
    foreach (var path in files)
        Need(Text(result.GetProperty("source_files"), path) == Sha(path),
             "RS154 staged source checksum mismatch " + path);
    var trial = result.GetProperty("rehearsal");
    Need(Text(trial, "status") ==
             "scientific-immutable-disposable-publisher-qualified" &&
         Text(trial, "packageId") == "dw.tools.math.scientific" &&
         Text(trial, "packageVersion") == "20260.20261105.1001" &&
         Text(trial, "historicalSourceVersion") == "1.1.0-preview.14" &&
         Text(trial, "originalCandidateArchiveSha256") ==
             "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
         Text(trial, "originalCandidateDllSha256") ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         Text(trial, "rehearsalArchiveSha256").Length == 64 &&
         Text(trial, "rehearsalDllSha256").Length == 64 &&
         Text(trial, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(trial, "publisherDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial.GetProperty("packageDependencies").GetInt32() == 0 &&
         trial.GetProperty("consumerChecks").GetInt32() == 15 &&
         Flag(trial, "disposableFeedPublished") &&
         !Flag(trial, "publishedToNugetLocal") &&
         Flag(trial, "collisionRefused") &&
         Flag(trial, "alteredDigestRefused") &&
         Flag(trial, "cancelledRefused") &&
         Flag(trial, "originalSourcesUnchanged") &&
         Flag(trial, "originalSourceProjectVersionUnchanged") &&
         Flag(trial, "sharedFeedUnchanged") &&
         !Flag(trial, "licenseSelected") &&
         !Flag(trial, "rollbackQualified"),
         "real immutable publisher rehearsal or fifteen-oracle binary checks incomplete");
    var inner = trial.GetProperty("commandReceipts").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 7 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "source-restore", "source-build", "source-pack",
             "consumer-restore", "consumer-build", "consumer-binary", "sdk"
         }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "tool-restore", "tool-build", "scientific-rehearsal", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "rehearsal must record seven inner and four outer real commands");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
