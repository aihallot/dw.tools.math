#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f12-ir-publisher-qualified.json";
const string Status = "rs145-ir-immutable-local-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-IR-F12:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-IR-F12:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-IR-F12:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f12-ir-publisher.md",
    "docs/distribution/ValidateNugetIrPublisher.cs",
    "tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj",
    "tools/dw.tools.math.nuget-ir-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-ir-release/Program.cs"
};
static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS145 IR publisher: " + reason);
}
static JsonDocument Load(string file) => JsonDocument.Parse(File.ReadAllText(file));
static string Text(JsonElement node, string key) =>
    node.GetProperty(key).GetString() ??
    throw new InvalidDataException("Required source identity field missing: " + key);
static bool Flag(JsonElement node, string key) => node.GetProperty(key).GetBoolean();
static string Sha(string file) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
static bool IsSha(string sha) =>
    Regex.IsMatch(sha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is supported");
foreach (var file in files)
    Need(File.Exists(file), "staged full-source file missing " + file);

using var previous = Load(".aura/workflow/reports/RS144/attempt-001.json");
using var source = Load("docs/distribution/nuget-federation-f11-ir-candidate-qualified.json");
using var baseReceipt = Load("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var backlog = Load("docs/planning/backlog.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var guidance = Load("docs/workflow/manifest.json");

Need(Text(previous.RootElement, "runId") == "RS144" &&
     previous.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous.RootElement, "outcome") == "succeeded" &&
     Text(previous.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     previous.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("count").GetInt32() == 0 &&
     Flag(previous.RootElement.GetProperty("priorInvocationFailures"),
         "firstPassSucceeded") &&
     Text(previous.RootElement.GetProperty("git"), "commitMessage") ==
         "test: qualify IR calendar NuGet candidate with exact Quantities dependency",
     "RS144 must be the exact successful first-pass DWF predecessor");

Need(Text(source.RootElement, "status") ==
         "rs144-ir-calendared-dependent-nuget-candidate-qualified-unpublished" &&
     !Flag(source.RootElement, "package_published") &&
     Text(baseReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseReceipt.RootElement, "physically_published_package_verified"),
     "RS144 candidate and previously published base evidence must remain retained");

var candidate = source.RootElement.GetProperty("candidate");
Need(Text(candidate, "status") ==
         "qualified-ir-dependent-calendar-candidate-not-published" &&
     Text(candidate, "packageId") == "dw.tools.math.ir" &&
     Text(candidate, "packageVersion") == "20260.20261104.1001" &&
     Text(candidate, "historicalSourceVersion") == "0.3.0-preview.1" &&
     Text(candidate, "candidateArchivePath") ==
         "build/artifacts/rs144-ir-candidate/candidate/dw.tools.math.ir.20260.20261104.1001.nupkg" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 21926 &&
     Text(candidate, "candidateArchiveSha256") ==
         "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
     Text(candidate, "candidateDllSha256") ==
         "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
     Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyQuantitiesArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(candidate, "dependencyQuantitiesDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Flag(candidate, "independentPackageReference") &&
     !Flag(candidate, "published"),
     "RS144 exact IR candidate archive or single published dependency drifted");
foreach (var original in source.RootElement.GetProperty("source_files").EnumerateObject())
    Need(File.Exists(original.Name) && Sha(original.Name) == original.Value.GetString(),
         "RS144 source-pinned file changed: " + original.Name);

Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "accepted Math product scope or native 541-node plan changed");

const string Tool = "tools/dw.tools.math.nuget-ir-release/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-ir-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "shared NuGet publisher must remain pinned through exact PackageReference");

var complete = File.Exists(Evidence);
if (complete)
{
    using var result = Load(Evidence);
    var receipt = result.RootElement;
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.tools.math.ir" &&
         Text(receipt, "feed_selector") == "NUGET_LOCAL" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(receipt, "package_published") &&
         !Flag(receipt, "release_authorized_by_this_run") &&
         !Flag(receipt, "source_versions_modified") &&
         !Flag(receipt, "new_public_license_selected") &&
         !Flag(receipt, "installed_package_rollback_qualified") &&
         !Flag(receipt, "external_requests_transmitted") &&
         !Flag(receipt, "discovery_migration_qualified"),
         "DWF rehearsal cannot assert publication, rights or future integration");

    var sourceHashes = receipt.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(sourceHashes, file) == Sha(file),
             "RS145 staged file SHA differs: " + file);

    var trial = receipt.GetProperty("rehearsal");
    Need(Text(trial, "status") ==
             "ir-dependent-package-publisher-rehearsal-qualified" &&
         Text(trial, "packageId") == "dw.tools.math.ir" &&
         Text(trial, "version") == "20260.20261104.1001" &&
         Text(trial, "historicalSourceVersion") == "0.3.0-preview.1" &&
         Text(trial, "rs144CandidateArchiveSha256") ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(trial, "rs144CandidateDllSha256") ==
             "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         IsSha(Text(trial, "rehearsalArchiveSha256")) &&
         IsSha(Text(trial, "rehearsalDllSha256")) &&
         Text(trial, "exactDependencyId") == "dw.quantities" &&
         Text(trial, "exactDependencyVersion") == "20260.20261102.1001" &&
         Text(trial, "exactDependencyArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(trial, "exactDependencyDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         trial.GetProperty("dependencyCount").GetInt32() == 1 &&
         Text(trial, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(trial, "publisherDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial.GetProperty("controls").GetInt32() == 7 &&
         trial.GetProperty("consumerChecks").GetInt32() == 15 &&
         Flag(trial, "disposableFeedPublished") &&
         !Flag(trial, "publishedToNugetLocal") &&
         Flag(trial, "collisionRefused") &&
         Flag(trial, "alteredDigestRefused") &&
         Flag(trial, "cancelledRefused") &&
         Flag(trial, "sourceFilesUnchanged") &&
         Flag(trial, "sourceProjectVersionUnchanged") &&
         Flag(trial, "sourceProjectReferencePreserved") &&
         Flag(trial, "publishedDependencyUnchanged") &&
         !Flag(trial, "sourceReleaseAuthorization") &&
         !Flag(trial, "publicLicenseSelected") &&
         !Flag(trial, "installedPackageRollbackQualified"),
         "immutable shared NuGet publisher rehearsal has missing refusal or binary evidence");

    var inner = trial.GetProperty("commandReceipts").EnumerateArray().ToArray();
    var innerIds = new[] {
        "source-restore", "source-build", "source-pack",
        "consumer-restore", "consumer-build", "consumer-binary", "sdk"
    };
    Need(inner.Length == innerIds.Length &&
         inner.Select(x => Text(x, "id")).SequenceEqual(innerIds) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "seven compiled source, package and source-free consumer command receipts required");
    var outer = receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    var outerIds = new[] { "tool-restore", "tool-build", "publisher-rehearsal", "sdk" };
    Need(outer.Length == outerIds.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerIds) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "the four DWF child commands must be successful");
}
Console.WriteLine(complete ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
