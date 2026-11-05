#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f16-analysis-published-qualified.json";
const string Status = "rs149-analysis-published-local-binary-independently-verified";
const string Ready = "MATH-ANALYSIS-F16:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F16:PHYSICAL=VERIFIED;BINARY=13;DEPENDENCIES=0;NATIVE=541";
const string Limits = "MATH-ANALYSIS-F16:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f16-analysis-published.md",
    "docs/distribution/ValidateNugetAnalysisPublishedFeed.cs",
    "tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj",
    "tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs"
};

static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS149 Analysis readback: " + message);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement element, string key) => element.GetProperty(key).GetString() ??
    throw new InvalidDataException("Missing expected source field: " + key);
static bool Flag(JsonElement element, string key) => element.GetProperty(key).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var count = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only observational --check is supported");
foreach (var file in files)
    Need(File.Exists(file), "missing installed verifier source: " + file);

using var report = Read(".aura/workflow/reports/RS148/attempt-001.json");
using var candidateReceipt = Read("docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json");
using var rehearsalReceipt = Read("docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

var prior = report.RootElement;
Need(Text(prior, "runId") == "RS148" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(prior.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "test: qualify Analysis immutable NuGet publisher rehearsal",
     "exact successful first-pass RS148 predecessor required");
var source = candidateReceipt.RootElement;
var candidate = source.GetProperty("candidate");
var rehearsal = rehearsalReceipt.RootElement;
var trial = rehearsal.GetProperty("rehearsal");
Need(Text(source, "status") ==
         "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished" &&
     !Flag(source, "package_published") &&
     Text(candidate, "packageId") == "dw.tools.math.analysis" &&
     Text(candidate, "packageVersion") == "20260.20261104.1001" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 27831 &&
     Text(candidate, "candidateArchiveSha256") ==
         "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
     Text(candidate, "candidateDllSha256") ==
         "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 13 &&
     Flag(candidate, "independentPackageReference"),
     "original F14 candidate identity or thirteen oracles drifted");
Need(Text(rehearsal, "status") ==
         "rs148-analysis-immutable-private-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsal, "package_published") &&
     Text(trial, "originalCandidateArchiveSha256") ==
         "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
     Text(trial, "originalCandidateDllSha256") ==
         "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
     Flag(trial, "disposableFeedPublished") &&
     !Flag(trial, "publishedToNugetLocal"),
     "RS148 rehearsal is not evidence of shared-feed publication");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "native Math roadmap or guidance drifted");

foreach (var previous in new[]
{
    source.GetProperty("source_files"),
    rehearsal.GetProperty("source_files")
})
    foreach (var item in previous.EnumerateObject())
        Need(File.Exists(item.Name) && Hash(item.Name) == item.Value.GetString(),
             "earlier owner source fingerprint changed: " + item.Name);

const string Tool = "tools/dw.tools.math.nuget-analysis-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-analysis-feed-verify.csproj")
         .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     !File.ReadAllText(Tool + "dw.tools.math.nuget-analysis-feed-verify.csproj")
         .Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("MATH-ANALYSIS-BINARY|PASS|13", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("physical-feed-snapshot", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("publicationInvoked = false", StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs"),
     "independent BCL readback and unchanged binary oracle required");

var finished = File.Exists(Evidence);
if (finished)
{
    using var document = Read(Evidence);
    var result = document.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.analysis" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(result, "physically_published_package_verified") &&
         !Flag(result, "package_published_by_this_run") &&
         !Flag(result, "remote_registry_published") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "DWF readback must not claim a new publication, license or migration");
    var hashes = result.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(hashes, file) == Hash(file),
             "RS149 installed verifier SHA-256 drifted: " + file);
    var observed = result.GetProperty("observation");
    Need(Text(observed, "status") ==
             "analysis-published-local-binary-independently-verified" &&
         Text(observed, "packageId") == "dw.tools.math.analysis" &&
         Text(observed, "packageVersion") == "20260.20261104.1001" &&
         observed.GetProperty("archiveBytes").GetInt32() == 27831 &&
         Text(observed, "archiveSha256") ==
             "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
         Text(observed, "packagedDllSha256") ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         Text(observed, "loadedDllSha256") ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 0 &&
         observed.GetProperty("publishedPackageSnapshotCount").GetInt32() == 1 &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 13 &&
         Flag(observed, "independentPackageReference") &&
         Flag(observed, "physicalArchiveInspected") &&
         Flag(observed, "publishedArchiveUnchanged") &&
         !Flag(observed, "originalMathProjectReferencesUsed") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "installedPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified") &&
         Text(observed, "sourceFeed") == "NUGET_LOCAL" &&
         Text(observed, "consumerSource") ==
             "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs" &&
         !string.IsNullOrWhiteSpace(Text(observed, "sdkVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "runtimeVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "osDescription")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "processArchitecture")),
         "physical archive or independent thirteen-case consumer proof incomplete");
    var inner = observed.GetProperty("commands").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 4 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
             { "consumer-restore", "consumer-build", "consumer-binary", "sdk" }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
             { "tool-restore", "tool-build", "analysis-feed-verify", "sdk" }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "actual read-only verifier and binary process receipts absent");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
