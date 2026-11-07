#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json";
const string Status = "rs162-codecs-immutable-local-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-CODECS-F29:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-CODECS-F29:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-CODECS-F29:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f29-codecs-publisher.md",
    "docs/distribution/ValidateNugetCodecsPublisher.cs",
    "tools/dw.tools.math.nuget-codecs-release/dw.tools.math.nuget-codecs-release.csproj",
    "tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-codecs-release/Program.cs"
};

static void Need(bool value, string reason)
{
    if (!value) throw new InvalidDataException("RS162 Codecs publisher: " + reason);
}
static JsonDocument Read(string file) => JsonDocument.Parse(File.ReadAllText(file));
static string Text(JsonElement node, string name) => node.GetProperty(name).GetString()
    ?? throw new InvalidDataException("required exact metadata missing: " + name);
static bool Flag(JsonElement node, string name) => node.GetProperty(name).GetBoolean();
static string Sha(string path) => Convert.ToHexStringLower(
    SHA256.HashData(File.ReadAllBytes(path)));
static bool IsSha(string hash) => Regex.IsMatch(
    hash, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
static int Count(JsonElement node)
{
    var count = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only non-mutating --check supported");
foreach (var file in files) Need(File.Exists(file), "staged file missing: " + file);
using var previous = Read(".aura/workflow/reports/RS161/attempt-001.json");
using var source = Read("docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json");
using var ir = Read("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

var prior = previous.RootElement;
var failures = prior.GetProperty("priorInvocationFailures");
Need(Text(prior, "runId") == "RS161" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "test: qualify Codecs NuGet candidate with published IR and Quantities dependency closure" &&
     failures.GetProperty("count").GetInt32() == 1 &&
     failures.GetProperty("preparationFailureCount").GetInt32() == 1 &&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     !Flag(failures, "firstPassSucceeded"),
     "RS161 exact succeeded predecessor with one pre-attempt refusal is required");
Need(Text(source.RootElement, "status") ==
         "rs161-codecs-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(source.RootElement, "package_published") &&
     Text(ir.RootElement, "status") ==
         "rs146-ir-published-local-two-binaries-independently-verified" &&
     Flag(ir.RootElement, "physically_published_package_verified"),
     "candidate or independent published IR evidence differs");
var candidate = source.RootElement.GetProperty("candidate");
Need(Text(candidate, "packageId") == "dw.tools.math.codecs" &&
     Text(candidate, "status") == "qualified-codecs-dependent-calendar-candidate-not-published" &&
     Text(candidate, "packageVersion") == "20260.20261107.1001" &&
     Text(candidate, "historicalSourceVersion") == "1.0.0-preview.8" &&
     Text(candidate, "candidateArchivePath") ==
         "build/artifacts/rs161-codecs-candidate/candidate/dw.tools.math.codecs.20260.20261107.1001.nupkg" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 35096 &&
     Text(candidate, "candidateArchiveSha256") ==
         "00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35" &&
     Text(candidate, "candidateDllSha256") ==
         "3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 1 &&
     Flag(candidate, "originalNineSourceFilesLinked") &&
     Flag(candidate, "independentPackageReference") &&
     !Flag(candidate, "published") &&
     Text(candidate, "dependencyIrId") == "dw.tools.math.ir" &&
     Text(candidate, "dependencyIrVersion") == "20260.20261104.1001" &&
     Text(candidate, "dependencyIrArchiveSha256") ==
         "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
     Text(candidate, "dependencyIrDllSha256") ==
         "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
     Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyQuantitiesArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(candidate, "dependencyQuantitiesDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
     "RS161 exact 35,096-byte Codecs candidate or dependency closure changed");

foreach (var file in source.RootElement.GetProperty("source_files").EnumerateObject())
    Need(File.Exists(file.Name) && Sha(file.Name) == file.Value.GetString(),
         "F28 qualified file changed " + file.Name);
var project = "src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
var lockfile = "src/projects/dw.tools.math.codecs/packages.lock.json";
Need(Sha(project) == Text(candidate, "originalProjectSha256") &&
     Sha(lockfile) == Text(candidate, "originalSourceLockSha256") &&
     candidate.GetProperty("originalSourcesSha256").EnumerateObject().Count() == 10,
     "original Codecs project and nine sources or lockfile changed");
foreach (var file in candidate.GetProperty("originalSourcesSha256").EnumerateObject())
    Need(File.Exists(file.Name) && Sha(file.Name) == file.Value.GetString(),
         "F28 original source differs: " + file.Name);
var irObservation = ir.RootElement.GetProperty("observation");
Need(Text(irObservation, "packageId") == "dw.tools.math.ir" &&
     Text(irObservation, "archiveSha256") ==
         "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
     irObservation.GetProperty("declaredDependencyCount").GetInt32() == 1 &&
     irObservation.GetProperty("exactDependencies").EnumerateArray().Any(x =>
         Text(x, "id") == "dw.quantities" &&
         Text(x, "version") == "20260.20261102.1001"),
     "F13 published IR transitive Quantities authority is missing");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().All(
         x => Text(x, "state") == "done"),
     "native/product plan or DWF guidance drifted");
Need(File.ReadAllText(files[4]).Contains("--rehearse", StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("DotNetLocalPackageRepository", StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("MATH-CODECS-PUBLISHED|id=", StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains(
         "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
         StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs"),
     "bounded publisher, exact provider or binary consumer fixture missing");

var finished = File.Exists(Evidence);
if (finished)
{
    using var output = Read(Evidence);
    var receipt = output.RootElement;
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.tools.math.codecs" &&
         Text(receipt, "previous_owner_run") == "RS161" &&
         Text(receipt, "sourceBaselineCommit") ==
             "0733e6018ecd5dd8afc41d4b7e5f000fef3b154d" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(receipt, "package_published") &&
         !Flag(receipt, "release_authorized_by_this_run") &&
         !Flag(receipt, "source_versions_modified") &&
         !Flag(receipt, "new_public_license_selected") &&
         !Flag(receipt, "installed_package_rollback_qualified") &&
         !Flag(receipt, "external_requests_transmitted") &&
         !Flag(receipt, "discovery_migration_qualified"),
         "F29 DWF target receipt claims publication or invalid predecessor");

    foreach (var file in files)
        Need(Text(receipt.GetProperty("source_files"), file) == Sha(file),
             "installed staged source does not match F29 receipt " + file);
    var run = receipt.GetProperty("rehearsal");
    Need(Text(run, "status") == "codecs-dependent-package-publisher-rehearsal-qualified" &&
         Text(run, "packageId") == "dw.tools.math.codecs" &&
         Text(run, "version") == "20260.20261107.1001" &&
         Text(run, "historicalSourceVersion") == "1.0.0-preview.8" &&
         Text(run, "rs161CandidateArchiveSha256") ==
             "00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35" &&
         Text(run, "rs161CandidateDllSha256") ==
             "3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74" &&
         IsSha(Text(run, "rehearsalArchiveSha256")) &&
         IsSha(Text(run, "rehearsalDllSha256")) &&
         Text(run, "exactDependencyId") == "dw.tools.math.ir" &&
         Text(run, "exactDependencyVersion") == "20260.20261104.1001" &&
         Text(run, "transitiveQuantityId") == "dw.quantities" &&
         Text(run, "transitiveQuantityVersion") == "20260.20261102.1001" &&
         Text(run, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         run.GetProperty("dependencyCount").GetInt32() == 1 &&
         run.GetProperty("controls").GetInt32() == 7 &&
         run.GetProperty("consumerChecks").GetInt32() == 15 &&
         Flag(run, "disposableFeedPublished") &&
         !Flag(run, "publishedToNugetLocal") &&
         Flag(run, "collisionRefused") &&
         Flag(run, "alteredDigestRefused") &&
         Flag(run, "cancelledRefused") &&
         Flag(run, "sourceFilesUnchanged") &&
         Flag(run, "sourceProjectVersionUnchanged") &&
         Flag(run, "sourceProjectReferencePreserved") &&
         Flag(run, "publishedDependencyUnchanged") &&
         Flag(run, "transitiveDependencyUnchanged") &&
         !Flag(run, "sourceReleaseAuthorization") &&
         !Flag(run, "publicLicenseSelected") &&
         !Flag(run, "installedPackageRollbackQualified"),
         "F29 immutable disposable publisher, dependency and refusal controls incomplete");
    var inner = run.GetProperty("commandReceipts").EnumerateArray().ToArray();
    var innerIds = new[] {
        "source-restore", "source-build", "source-pack",
        "consumer-restore", "consumer-build", "consumer-binary", "sdk"
    };
    Need(inner.Length == 7 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(innerIds) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "F29 seven real internal command receipts required");
    var outer = receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    var outerIds = new[] { "tool-restore", "tool-build", "publisher-rehearsal", "sdk" };
    Need(outer.Length == 4 &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerIds) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "F29 four real outer command receipts required");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
