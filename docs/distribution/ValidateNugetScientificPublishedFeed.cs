#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f23-scientific-published-qualified.json";
const string Status = "rs156-scientific-published-local-binary-independently-verified";
const string Ready = "MATH-SCIENTIFIC-F23:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F23:PHYSICAL=VERIFIED;BINARY=15;DEPENDENCIES=0;NATIVE=541";
const string Limits = "MATH-SCIENTIFIC-F23:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f23-scientific-published.md",
    "docs/distribution/ValidateNugetScientificPublishedFeed.cs",
    "tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj",
    "tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs"
};

static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS156 Scientific readback: " + message);
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

using var report = Read(".aura/workflow/reports/RS155/attempt-001.json");
using var candidateReceipt = Read("docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json");
using var rehearsalReceipt = Read("docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json");
using var correctionReceipt = Read("docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

var prior = report.RootElement;
Need(Text(prior, "runId") == "RS155" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(prior.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "fix: correct Scientific private NuGet publication and verification diagnostics",
     "exact successful first-pass RS155 predecessor required");
var source = candidateReceipt.RootElement;
var candidate = source.GetProperty("candidate");
var rehearsal = rehearsalReceipt.RootElement;
var trial = rehearsal.GetProperty("rehearsal");
Need(Text(source, "status") ==
         "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished" &&
     !Flag(source, "package_published") &&
     Text(candidate, "packageId") == "dw.tools.math.scientific" &&
     Text(candidate, "packageVersion") == "20260.20261105.1001" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 37265 &&
     Text(candidate, "candidateArchiveSha256") ==
         "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
     Text(candidate, "candidateDllSha256") ==
         "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Flag(candidate, "independentPackageReference"),
     "original F20 candidate identity or fifteen oracles drifted");
Need(Text(rehearsal, "status") ==
         "rs154-scientific-immutable-private-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsal, "package_published") &&
     Text(trial, "originalCandidateArchiveSha256") ==
         "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
     Text(trial, "originalCandidateDllSha256") ==
         "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
     trial.GetProperty("consumerChecks").GetInt32() == 15 &&
     Flag(trial, "disposableFeedPublished") &&
     !Flag(trial, "publishedToNugetLocal"),
     "RS154 rehearsal is not evidence of shared-feed publication");
var corrected = correctionReceipt.RootElement;
Need(Text(corrected, "status") ==
         "rs155-scientific-private-release-operator-messages-qualified-unpublished" &&
     Flag(corrected, "release_controller_compiled") &&
     !Flag(corrected, "release_modes_invoked") &&
     !Flag(corrected, "package_published_by_this_run") &&
     corrected.GetProperty("operator_message_archive_bytes").GetInt32() == 37265 &&
     corrected.GetProperty("operator_message_binary_checks").GetInt32() == 15 &&
     Text(corrected, "original_archive_sha256") ==
         "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
     Text(corrected, "original_dll_sha256") ==
         "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c",
     "RS155 source-correct Scientific release controller must remain qualified");

Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "native Math roadmap or guidance drifted");

const string CorrectedReleaseSource =
    "tools/dw.tools.math.nuget-scientific-release/Program.cs";
const string HistoricalReleaseSource =
    ".workflow/runs/RS154/staged/tools/dw.tools.math.nuget-scientific-release/Program.cs";
foreach (var previous in new[]
{
    source.GetProperty("source_files"),
    corrected.GetProperty("source_files")
})
    foreach (var item in previous.EnumerateObject())
        Need(File.Exists(item.Name) && Hash(item.Name) == item.Value.GetString(),
             "current original or RS155 source fingerprint changed: " + item.Name);
foreach (var item in rehearsal.GetProperty("source_files").EnumerateObject())
{
    var inspected = item.Name == CorrectedReleaseSource
        ? HistoricalReleaseSource
        : item.Name;
    Need(File.Exists(inspected) && Hash(inspected) == item.Value.GetString(),
         "RS154 historical source fingerprint changed: " + item.Name);
}
Need(Text(corrected, "historical_controller_source_sha256") ==
         Text(rehearsal.GetProperty("source_files"), CorrectedReleaseSource) &&
     Text(corrected, "corrected_controller_source_sha256") ==
         Hash(CorrectedReleaseSource),
     "RS154 historical and RS155 corrected release controller lineage has drifted");

const string Tool = "tools/dw.tools.math.nuget-scientific-feed-verify/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-feed-verify.csproj")
         .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     !File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-feed-verify.csproj")
         .Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("MATH-SCIENTIFIC-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("physical-feed-snapshot", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("publicationInvoked = false", StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs"),
     "independent BCL readback and unchanged binary oracle required");

var finished = File.Exists(Evidence);
if (finished)
{
    using var document = Read(Evidence);
    var result = document.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.scientific" &&
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
             "RS156 installed verifier SHA-256 drifted: " + file);
    var observed = result.GetProperty("observation");
    Need(Text(observed, "status") ==
             "scientific-published-local-binary-independently-verified" &&
         Text(observed, "packageId") == "dw.tools.math.scientific" &&
         Text(observed, "packageVersion") == "20260.20261105.1001" &&
         observed.GetProperty("archiveBytes").GetInt32() == 37265 &&
         Text(observed, "archiveSha256") ==
             "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
         Text(observed, "packagedDllSha256") ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         Text(observed, "loadedDllSha256") ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 0 &&
         observed.GetProperty("publishedPackageSnapshotCount").GetInt32() == 1 &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 15 &&
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
             "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs" &&
         !string.IsNullOrWhiteSpace(Text(observed, "sdkVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "runtimeVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "osDescription")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "processArchitecture")),
         "physical archive or independent fifteen-case consumer proof incomplete");
    var inner = observed.GetProperty("commands").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 4 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
             { "consumer-restore", "consumer-build", "consumer-binary", "sdk" }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
             { "tool-restore", "tool-build", "scientific-feed-verify", "sdk" }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "actual read-only verifier and binary process receipts absent");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
