#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json";
const string Status = "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-SCIENTIFIC-F20:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F20:CANDIDATE=QUALIFIED;BINARY=15;DEPENDENCIES=0;NATIVE=541";
const string Limits = "MATH-SCIENTIFIC-F20:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f20-scientific-candidate.md",
    "docs/distribution/ValidateNugetScientificCandidate.cs",
    "tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj",
    "tools/dw.tools.math.nuget-scientific-candidate/Program.cs",
    "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs"
};

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS153 Scientific candidate: " + reason);
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
using var report = Read(".aura/workflow/reports/RS152/attempt-001.json");
using var previous = Read("docs/distribution/nuget-federation-f19-foundation-published-qualified.json");
using var inventory = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
using var rights = Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");

var prior = report.RootElement;
Need(Text(prior, "runId") == "RS152" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(prior.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "test: verify published Foundation NuGet archive and isolated binary consumer",
     "RS152 exact first-pass physical-feed qualification is required");
Need(Text(previous.RootElement, "status") ==
         "rs152-foundation-published-local-binary-independently-verified" &&
     Flag(previous.RootElement, "physically_published_package_verified") &&
     !Flag(previous.RootElement, "package_published_by_this_run") &&
     previous.RootElement.GetProperty("observation")
         .GetProperty("compiledConsumerChecks").GetInt32() == 5,
     "the sixth private Math package has not been independently qualified");

var packages = inventory.RootElement.GetProperty("observed")
    .GetProperty("packages").EnumerateArray().ToArray();
Need(inventory.RootElement.GetProperty("package_count").GetInt32() == 13 &&
     packages.Length == 13, "canonical F1 source-package count drifted");
var selected = packages.Single(item => Text(item, "packageId") == "dw.tools.math.scientific");
Need(Text(selected, "sourceProject") ==
         "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj" &&
     Text(selected, "historicalSourceVersion") == "1.1.0-preview.14" &&
     Text(selected, "effectiveTargetFramework") == "net10.0" &&
     Text(selected, "effectivePackable") == "true" &&
     selected.GetProperty("projectReferences").GetArrayLength() == 0 &&
     selected.GetProperty("externalPackageReferences").GetArrayLength() == 0,
     "original Scientific source and dependency-free inventory differ");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(milestone => Text(milestone, "state") == "done") &&
     Flag(rights.RootElement.GetProperty("authorization"), "package_from_dw_tools_math"),
     "Math native plan, guidance or private source-owner authority drifted");
Need(File.ReadAllText("src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj")
         .Contains("<Version>1.1.0-preview.14</Version>", StringComparison.Ordinal) &&
     File.ReadAllText("src/projects/dw.tools.math.scientific/BoundedExactScientific.cs")
         .Contains("public const string RuleId=\"dw-math-bounded-exact-scientific-1\";",
             StringComparison.Ordinal),
     "original historical Scientific source is not preserved");
const string Tool = "tools/dw.tools.math.nuget-scientific-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-candidate.csproj")
         .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     !File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-candidate.csproj")
         .Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "fixtures/ScientificBinaryConsumer.cs")
         .Contains("MATH-SCIENTIFIC-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("source-pack", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("sourceFeedUnchanged = true", StringComparison.Ordinal),
     "bounded source-free Scientific producer and fixture are not retained");

var finished = File.Exists(Evidence);
if (finished)
{
    using var document = Read(Evidence);
    var result = document.RootElement;
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
         "F20 candidate receipt may not assert a publication or migration");
    foreach (var file in files)
        Need(Text(result.GetProperty("source_files"), file) == Hash(file),
             "RS153 staged source SHA-256 drifted: " + file);
    var candidate = result.GetProperty("candidate");
    Need(Text(candidate, "status") ==
             "qualified-independent-scientific-calendar-candidate-not-published" &&
         Text(candidate, "packageId") == "dw.tools.math.scientific" &&
         Text(candidate, "packageVersion") == "20260.20261105.1001" &&
         Text(candidate, "historicalSourceVersion") == "1.1.0-preview.14" &&
         Text(candidate, "sourceProjectPath") ==
             "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj" &&
         Text(candidate, "candidateArchivePath") ==
             "build/artifacts/rs153-scientific-candidate/candidate/dw.tools.math.scientific.20260.20261105.1001.nupkg" &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 2000 &&
         Text(candidate, "candidateArchiveSha256").Length == 64 &&
         Text(candidate, "candidateDllSha256").Length == 64 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
         candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
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
         "Scientific original candidate/consumer/platform evidence incomplete");
    var hashes = candidate.GetProperty("originalSourcesSha256");
    Need(hashes.EnumerateObject().Count() == 18,
         "exact sixteen C# source, historical project and lockfile fingerprints required");
    foreach (var entry in hashes.EnumerateObject())
        Need(File.Exists(entry.Name) && Hash(entry.Name) == entry.Value.GetString(),
             "original Scientific source bytes changed: " + entry.Name);
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
             "tool-restore", "tool-build", "scientific-candidate", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "seven original package and four outer actual command receipts required");
}

Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
