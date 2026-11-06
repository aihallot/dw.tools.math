#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json";
const string Status = "rs159-statistics-private-publication-receipt-corrected-unpublished";
const string Ready = "MATH-STATISTICS-F26:FIELD=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F26:FIELD=CORRECTED;BUILD=PASS;NATIVE=541";
const string Limits = "MATH-STATISTICS-F26:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string Tool = "tools/dw.tools.math.nuget-statistics-release/";
const string Source = Tool + "Program.cs";
const string Historical = ".workflow/runs/RS158/staged/" + Source;
const string F24 = "docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json";
const string F25 = "docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json";
const string Prior = ".aura/workflow/reports/RS158/attempt-001.json";
const string ArchiveSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
const string DllSha = "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96";
const string OldField = "irDllSha256 = CandidateDllSha";
const string NewField = "statisticsDllSha256 = CandidateDllSha";
var files = new[]
{
    "docs/distribution/nuget-federation-f26-statistics-release-receipt.md",
    "docs/distribution/ValidateNugetStatisticsReleaseReceipt.cs",
    Source
};

static void Need(bool value, string message)
{
    if (!value) throw new InvalidDataException("RS159 Statistics receipt: " + message);
}
static JsonDocument Read(string file) => JsonDocument.Parse(File.ReadAllText(file));
static string Text(JsonElement source, string property) => source.GetProperty(property).GetString()
    ?? throw new InvalidDataException("Missing exact field: " + property);
static bool Flag(JsonElement source, string property) => source.GetProperty(property).GetBoolean();
static string Sha(string path) =>
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
    "only observational --check is admitted");
foreach (var file in files) Need(File.Exists(file), "missing source: " + file);
using var report = Read(Prior);
using var f24 = Read(F24);
using var f25 = Read(F25);
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
var prior = report.RootElement;
Need(Text(prior, "runId") == "RS158" &&
     prior.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior, "outcome") == "succeeded" &&
     Text(prior.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(prior.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(prior.GetProperty("git"), "commitMessage") ==
         "test: qualify Statistics immutable local NuGet publisher rehearsal",
     "exact successful first-pass RS158 predecessor required");

var candidate = f24.RootElement.GetProperty("candidate");
var rehearsal = f25.RootElement.GetProperty("rehearsal");
Need(Text(f24.RootElement, "status") ==
         "rs157-statistics-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(f24.RootElement, "package_published") &&
     Text(candidate, "packageId") == "dw.tools.math.statistics" &&
     Text(candidate, "packageVersion") == "20260.20261106.1001" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 17451 &&
     Text(candidate, "candidateArchiveSha256") == ArchiveSha &&
     Text(candidate, "candidateDllSha256") == DllSha &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 18 &&
     candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001",
     "original F24 Statistics candidate authority changed");
Need(Text(f25.RootElement, "status") ==
         "rs158-statistics-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(f25.RootElement, "package_published") &&
     Text(rehearsal, "rs157CandidateArchiveSha256") == ArchiveSha &&
     Text(rehearsal, "rs157CandidateDllSha256") == DllSha &&
     rehearsal.GetProperty("consumerChecks").GetInt32() == 18 &&
     Flag(rehearsal, "disposableFeedPublished") &&
     !Flag(rehearsal, "publishedToNugetLocal"),
     "F25 must prove only the independent disposable publisher rehearsal");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "accepted Math native roadmap changed");

foreach (var item in candidate.GetProperty("originalSourcesSha256").EnumerateObject())
    Need(File.Exists(item.Name) && Sha(item.Name) == item.Value.GetString(),
         "original Statistics source changed: " + item.Name);
Need(Sha("src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj") ==
         Text(candidate, "originalProjectSha256"),
     "original Statistics project changed");

Need(File.Exists(Historical), "immutable RS158 controller source absent");
var historicalHash = Text(f25.RootElement.GetProperty("source_files"), Source);
Need(Sha(Historical) == historicalHash, "RS158 staged source SHA not equal F25 authority");
var historical = File.ReadAllText(Historical);
var corrected = File.ReadAllText(Source);
Need(historical.Split(OldField, StringSplitOptions.None).Length - 1 == 1 &&
     corrected.Split(NewField, StringSplitOptions.None).Length - 1 == 1 &&
     !corrected.Contains(OldField, StringComparison.Ordinal) &&
     corrected == historical.Replace(OldField, NewField, StringComparison.Ordinal),
     "controller must differ by exactly one Statistics receipt field substitution");
Need(corrected.Contains("RequireQualifiedRunAndCleanCheckout(root)", StringComparison.Ordinal) &&
     corrected.Contains("IsPhysical(candidate, 17451, CandidateSha)", StringComparison.Ordinal) &&
     corrected.Contains("MATH-STATISTICS-VERIFY|id=", StringComparison.Ordinal) &&
     corrected.Contains("|binary=18|archive=", StringComparison.Ordinal) &&
     corrected.Contains("MATH-STATISTICS-BINARY|PASS|18", StringComparison.Ordinal),
     "unchanged owner release guards or eighteen-case consumer must remain present");
Need(File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "dw.tools.math.nuget-statistics-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal),
     "shared publisher dependency must remain pinned");
Need(File.ReadAllText(files[0]).Contains(
         "## Exact RS159 correction and source lineage", StringComparison.Ordinal),
     "canonical exact corrective scope missing");

var exists = File.Exists(Evidence);
if (exists)
{
    using var receipt = Read(Evidence);
    var record = receipt.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.tools.math.statistics" &&
         Text(record, "historical_controller_source_sha256") == historicalHash &&
         Text(record, "corrected_controller_source_sha256") == Sha(Source) &&
         Text(record, "original_archive_sha256") == ArchiveSha &&
         Text(record, "original_dll_sha256") == DllSha &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(record, "release_controller_compiled") &&
         !Flag(record, "release_modes_invoked") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "original_statistics_sources_modified") &&
         !Flag(record, "external_requests_transmitted"),
         "F26 cannot claim actual publication or unverified source identity");
    var fingerprints = record.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(fingerprints, file) == Sha(file),
             "RS159 installed source fingerprint changed: " + file);
    var commands = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.Length == 3 &&
         commands.Select(c => Text(c, "id")).SequenceEqual(
             ["tool-restore", "tool-build", "sdk"]) &&
         commands.All(c => c.GetProperty("exit_code").GetInt32() == 0) &&
         !string.IsNullOrWhiteSpace(Text(record, "sdk_version")),
         "three actual compiler and SDK process receipts required");
}
Console.WriteLine(exists ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
