#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json";
const string Status = "rs150-independent-foundation-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-FOUNDATION-F17:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-FOUNDATION-F17:CANDIDATE=QUALIFIED;BINARY=5;DEPENDENCIES=0;NATIVE=541";
const string Limits = "MATH-FOUNDATION-F17:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f17-foundation-candidate.md",
    "docs/distribution/ValidateNugetFoundationCandidate.cs",
    "tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj",
    "tools/dw.tools.math.nuget-foundation-candidate/Program.cs",
    "tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs"
};

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS150 Foundation candidate: " + reason);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement source, string field) => source.GetProperty(field).GetString() ??
    throw new InvalidDataException("Missing exact source field " + field);
static bool Flag(JsonElement source, string field) => source.GetProperty(field).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var field in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(field, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is supported");
foreach (var file in files)
    Need(File.Exists(file), "a staged source file was not installed: " + file);
using var report = Read(".aura/workflow/reports/RS149/attempt-001.json");
using var previous = Read("docs/distribution/nuget-federation-f16-analysis-published-qualified.json");
using var inventory = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
using var rights = Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");

var prior = report.RootElement;
Need(Text(prior, "runId") == "RS149" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(prior.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "test: verify published Analysis NuGet archive and isolated binary consumer",
     "RS149 exact first-pass physical-feed qualification is required");
Need(Text(previous.RootElement, "status") ==
         "rs149-analysis-published-local-binary-independently-verified" &&
     Flag(previous.RootElement, "physically_published_package_verified") &&
     !Flag(previous.RootElement, "package_published_by_this_run") &&
     previous.RootElement.GetProperty("observation")
         .GetProperty("compiledConsumerChecks").GetInt32() == 13,
     "the fifth private Math package has not been independently qualified");

var packages = inventory.RootElement.GetProperty("observed")
    .GetProperty("packages").EnumerateArray().ToArray();
Need(inventory.RootElement.GetProperty("package_count").GetInt32() == 13 &&
     packages.Length == 13, "canonical F1 source-package count drifted");
var selected = packages.Single(item => Text(item, "packageId") == "dw.tools.math.foundation");
Need(Text(selected, "sourceProject") ==
         "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj" &&
     Text(selected, "historicalSourceVersion") == "0.1.0-preview.1" &&
     Text(selected, "effectiveTargetFramework") == "net10.0" &&
     Text(selected, "effectivePackable") == "true" &&
     selected.GetProperty("projectReferences").GetArrayLength() == 0 &&
     selected.GetProperty("externalPackageReferences").GetArrayLength() == 0,
     "original Foundation source and dependency-free inventory differ");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(milestone => Text(milestone, "state") == "done") &&
     Flag(rights.RootElement.GetProperty("authorization"), "package_from_dw_tools_math"),
     "Math native plan, guidance or private source-owner authority drifted");
Need(File.ReadAllText("src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj")
         .Contains("<Version>0.1.0-preview.1</Version>", StringComparison.Ordinal) &&
     File.ReadAllText("src/projects/dw.tools.math.foundation/FoundationContract.cs")
         .Contains("public static string Identity => PackageId + \"/\" + ContractVersion;",
             StringComparison.Ordinal),
     "original historical Foundation source is not preserved");
const string Tool = "tools/dw.tools.math.nuget-foundation-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-foundation-candidate.csproj")
         .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     !File.ReadAllText(Tool + "dw.tools.math.nuget-foundation-candidate.csproj")
         .Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "fixtures/FoundationBinaryConsumer.cs")
         .Contains("MATH-FOUNDATION-BINARY|PASS|5", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("source-pack", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("sourceFeedUnchanged = true", StringComparison.Ordinal),
     "bounded source-free Foundation producer and fixture are not retained");

var finished = File.Exists(Evidence);
if (finished)
{
    using var document = Read(Evidence);
    var result = document.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == "dw.tools.math.foundation" &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "source_versions_modified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "F17 candidate receipt may not assert a publication or migration");
    foreach (var file in files)
        Need(Text(result.GetProperty("source_files"), file) == Hash(file),
             "RS150 staged source SHA-256 drifted: " + file);
    var candidate = result.GetProperty("candidate");
    Need(Text(candidate, "status") ==
             "qualified-independent-foundation-calendar-candidate-not-published" &&
         Text(candidate, "packageId") == "dw.tools.math.foundation" &&
         Text(candidate, "packageVersion") == "20260.20261105.1001" &&
         Text(candidate, "historicalSourceVersion") == "0.1.0-preview.1" &&
         Text(candidate, "sourceProjectPath") ==
             "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj" &&
         Text(candidate, "candidateArchivePath") ==
             "build/artifacts/rs150-foundation-candidate/candidate/dw.tools.math.foundation.20260.20261105.1001.nupkg" &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 2000 &&
         Text(candidate, "candidateArchiveSha256").Length == 64 &&
         Text(candidate, "candidateDllSha256").Length == 64 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
         candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 5 &&
         Flag(candidate, "independentPackageReference") &&
         Flag(candidate, "sourceProjectUnmodified") &&
         Flag(candidate, "sourceFeedUnchanged") &&
         !Flag(candidate, "originalProjectReferencesUsed") &&
         !Flag(candidate, "packagePublished") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "publicLicenseSelected") &&
         !Flag(candidate, "installedPackageRollbackQualified") &&
         !string.IsNullOrWhiteSpace(Text(candidate, "sdkVersion")) &&
         !string.IsNullOrWhiteSpace(Text(candidate, "runtimeVersion")) &&
         !string.IsNullOrWhiteSpace(Text(candidate, "osDescription")) &&
         !string.IsNullOrWhiteSpace(Text(candidate, "processArchitecture")),
         "Foundation original candidate/consumer/platform evidence incomplete");
    var hashes = candidate.GetProperty("originalSourcesSha256");
    Need(hashes.EnumerateObject().Count() == 2,
         "exact FoundationContract.cs and historical project fingerprints required");
    foreach (var entry in hashes.EnumerateObject())
        Need(File.Exists(entry.Name) && Hash(entry.Name) == entry.Value.GetString(),
             "original Foundation source bytes changed: " + entry.Name);
    var inner = candidate.GetProperty("commandReceipts").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 7 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "source-restore", "source-build", "source-pack",
             "consumer-restore", "consumer-build", "consumer-binary", "sdk"
         }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "tool-restore", "tool-build", "foundation-candidate", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "seven original package and four outer actual command receipts required");
}

Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
