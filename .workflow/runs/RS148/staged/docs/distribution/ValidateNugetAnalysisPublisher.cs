#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json";
const string Status = "rs148-analysis-immutable-private-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-ANALYSIS-F15:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F15:REHEARSAL=QUALIFIED;BINARY=13;NATIVE=541";
const string Limits = "MATH-ANALYSIS-F15:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";
var files = new[]
{
    "docs/distribution/nuget-federation-f15-analysis-publisher.md",
    "docs/distribution/ValidateNugetAnalysisPublisher.cs",
    "tools/dw.tools.math.nuget-analysis-release/dw.tools.math.nuget-analysis-release.csproj",
    "tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-analysis-release/Program.cs"
};
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS148 Analysis publisher: " + reason);
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
using var report = Load(".aura/workflow/reports/RS147/attempt-001.json");
using var candidateEvidence = Load("docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json");
using var priorPhysical = Load("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var backlog = Load("docs/planning/backlog.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var guidance = Load("docs/workflow/manifest.json");

var previous = report.RootElement;
var failures = previous.GetProperty("priorInvocationFailures");
Need(Text(previous, "runId") == "RS147" &&
     previous.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous, "outcome") == "succeeded" &&
     Text(previous.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     failures.GetProperty("count").GetInt32() == 1 &&
     failures.GetProperty("preparationFailureCount").GetInt32() == 1 &&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     !Flag(failures, "firstPassSucceeded") &&
     Text(previous.GetProperty("git"), "commitMessage") ==
         "test: qualify independent Analysis calendar NuGet candidate without publication",
     "RS147 must be the observed successful corrected-preselection predecessor");
var owner = candidateEvidence.RootElement;
var candidate = owner.GetProperty("candidate");
Need(Text(owner, "status") ==
         "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished" &&
     !Flag(owner, "package_published") &&
     Text(candidate, "status") ==
         "qualified-independent-analysis-calendar-candidate-not-published" &&
     Text(candidate, "packageId") == "dw.tools.math.analysis" &&
     Text(candidate, "packageVersion") == "20260.20261104.1001" &&
     Text(candidate, "sourceVersion") == "1.1.0-preview.6" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 27831 &&
     Text(candidate, "candidateArchiveSha256") ==
         "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
     Text(candidate, "candidateDllSha256") ==
         "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 13 &&
     Flag(candidate, "independentPackageReference"),
     "qualified original F14 Analysis source-free package identity has changed");
Need(Text(priorPhysical.RootElement, "status") ==
         "rs146-ir-published-local-two-binaries-independently-verified" &&
     Flag(priorPhysical.RootElement, "physically_published_package_verified") &&
     !Flag(priorPhysical.RootElement, "package_published_by_this_run"),
     "published prerequisite must be independently qualified");
foreach (var file in candidateEvidence.RootElement.GetProperty("source_files").EnumerateObject())
    Need(File.Exists(file.Name) && Sha(file.Name) == file.Value.GetString(),
         "RS147 source-pin drift: " + file.Name);
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(node => Text(node, "state") == "done"),
     "unchanged 541-node Math roadmap and guidance required");
const string Tool = "tools/dw.tools.math.nuget-analysis-release/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-analysis-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "DotNetLocalPackageRepository", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "MATH-ANALYSIS-BINARY|PASS|13", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs").Contains(
         "RequireCleanOwnerAndRs148", StringComparison.Ordinal),
     "shared publisher and guarded owner modes must remain exact");

var finished = File.Exists(Evidence);
if (finished)
{
    using var receipt = Load(Evidence);
    var result = receipt.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.analysis" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "source_versions_modified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "DWF RS148 cannot claim actual shared-feed publication or license");
    foreach (var path in files)
        Need(Text(result.GetProperty("source_files"), path) == Sha(path),
             "RS148 staged source checksum mismatch " + path);
    var trial = result.GetProperty("rehearsal");
    Need(Text(trial, "status") ==
             "analysis-immutable-disposable-publisher-qualified" &&
         Text(trial, "packageId") == "dw.tools.math.analysis" &&
         Text(trial, "packageVersion") == "20260.20261104.1001" &&
         Text(trial, "sourceVersion") == "1.1.0-preview.6" &&
         Text(trial, "originalCandidateArchiveSha256") ==
             "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
         Text(trial, "originalCandidateDllSha256") ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         Text(trial, "rehearsalArchiveSha256").Length == 64 &&
         Text(trial, "rehearsalDllSha256").Length == 64 &&
         Text(trial, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(trial, "publisherDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial.GetProperty("packageDependencies").GetInt32() == 0 &&
         trial.GetProperty("consumerChecks").GetInt32() == 13 &&
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
         "real immutable publisher rehearsal or thirteen-oracle binary checks incomplete");
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
             "tool-restore", "tool-build", "analysis-rehearsal", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "rehearsal must record seven inner and four outer real commands");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
