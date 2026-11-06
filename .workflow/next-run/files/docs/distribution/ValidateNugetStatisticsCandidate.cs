#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json";
const string Status = "rs157-statistics-dependent-calendar-candidate-qualified-unpublished";
const string Ready = "MATH-STATISTICS-F24:SOURCE=READY;DEPENDENCY=PINNED;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F24:CANDIDATE=QUALIFIED;BINARY=18;DEPENDENCIES=1;NATIVE=541";
const string Limits = "MATH-STATISTICS-F24:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string Id = "dw.tools.math.statistics";
const string DependencyId = "dw.quantities";
const string DependencyVersion = "20260.20261102.1001";
const string DependencySha =
    "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string DependencyDll =
    "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string Project = "src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string Lock = "src/projects/dw.tools.math.statistics/packages.lock.json";
const string Fixture =
    "tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs";
var files = new[]
{
    "docs/distribution/nuget-federation-f24-statistics-candidate.md",
    "docs/distribution/ValidateNugetStatisticsCandidate.cs",
    "tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj",
    "tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-statistics-candidate/Program.cs",
    Fixture
};
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS157 Statistics: " + reason);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement source, string property) => source.GetProperty(property).GetString() ??
    throw new InvalidDataException("Missing exact source field: " + property);
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
    "only read-only --check mode is admitted");
foreach (var path in files)
    Need(File.Exists(path), "missing installed source: " + path);

using var report = Read(".aura/workflow/reports/RS156/attempt-001.json");
using var predecessor = Read("docs/distribution/nuget-federation-f23-scientific-published-qualified.json");
using var baseReceipt = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var inventory = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
using var rights = Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");

var previous = report.RootElement;
Need(Text(previous, "runId") == "RS156" &&
     previous.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous, "outcome") == "succeeded" &&
     Text(previous.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     previous.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(previous.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(previous.GetProperty("git"), "commitMessage") ==
         "test: verify published Scientific NuGet archive and isolated binary consumer",
     "exact successful first-pass RS156 predecessor required");
Need(Text(predecessor.RootElement, "status") ==
         "rs156-scientific-published-local-binary-independently-verified" &&
     Flag(predecessor.RootElement, "physically_published_package_verified") &&
     !Flag(predecessor.RootElement, "package_published_by_this_run") &&
     predecessor.RootElement.GetProperty("observation")
         .GetProperty("compiledConsumerChecks").GetInt32() == 15,
     "F23 Scientific physical-feed qualification drifted");
var basePackage = baseReceipt.RootElement.GetProperty("observation");
Need(Text(baseReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(baseReceipt.RootElement, "physically_published_package_verified") &&
     Text(basePackage, "packageId") == DependencyId &&
     Text(basePackage, "packageVersion") == DependencyVersion &&
     Text(basePackage, "archiveSha256") == DependencySha &&
     Text(basePackage, "packagedDllSha256") == DependencyDll &&
     basePackage.GetProperty("archiveBytes").GetInt32() == 14747,
     "published exact Quantities package dependency drifted");

var entries = inventory.RootElement.GetProperty("observed").GetProperty("packages")
    .EnumerateArray().ToArray();
Need(inventory.RootElement.GetProperty("package_count").GetInt32() == 13 &&
     entries.Length == 13, "F1 thirteen source package inventory drifted");
var selected = entries.Single(x => Text(x, "packageId") == Id);
Need(Text(selected, "sourceProject") == Project &&
     Text(selected, "historicalSourceVersion") == "0.4.0-preview.6" &&
     Text(selected, "effectiveTargetFramework") == "net10.0" &&
     Text(selected, "effectivePackable") == "true" &&
     selected.GetProperty("projectReferences").EnumerateArray()
         .Select(x => x.GetString()).SequenceEqual([DependencyId]) &&
     selected.GetProperty("externalPackageReferences").GetArrayLength() == 0 &&
     !Flag(selected, "candidateIsReserved"),
     "Statistics original project and exact dependency graph drifted");
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done") &&
     Flag(rights.RootElement.GetProperty("authorization"), "package_from_dw_tools_math"),
     "accepted Math native DWF plan, guidance or original rights drifted");
Need(File.ReadAllText(Project).Contains("<Version>0.4.0-preview.6</Version>",
         StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains(
         "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />",
         StringComparison.Ordinal),
     "original Statistics source/project reference must remain unchanged");
using (var locked = Read(Lock))
{
    var graph = locked.RootElement.GetProperty("dependencies")
        .GetProperty("net10.0").EnumerateObject().ToArray();
    Need(locked.RootElement.GetProperty("version").GetInt32() == 2 &&
         graph.Length == 1 && graph[0].Name == DependencyId &&
         Text(graph[0].Value, "type") == "Project",
         "original lockfile must retain one Project dependency");
}
const string Tool = "tools/dw.tools.math.nuget-statistics-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-statistics-candidate.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("originalSourceLockSha256 = ShaFile(lockPath)", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("MATH-STATISTICS-BINARY|PASS|18", StringComparison.Ordinal) &&
     File.ReadAllText(Fixture).Contains("MATH-STATISTICS-BINARY|PASS|18",
         StringComparison.Ordinal) &&
     File.ReadAllText("docs/distribution/nuget-federation-f24-statistics-candidate.md")
         .Contains("## Eighteen isolated package-only binary probes", StringComparison.Ordinal),
     "statistics dependency projection and independent consumer fixture missing");

var completed = File.Exists(Evidence);
if (completed)
{
    using var receipt = Read(Evidence);
    var result = receipt.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_package_id") == Id &&
         Text(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "source_versions_modified") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "F24 cannot claim an actual release or migration");
    foreach (var file in files)
        Need(Text(result.GetProperty("source_files"), file) == Sha(file),
             "RS157 staged source SHA drifted: " + file);
    var candidate = result.GetProperty("candidate");
    var version = Text(candidate, "packageVersion");
    Need(Regex.IsMatch(version, @"^20260\.2026\d{4}\.1\d{3}$",
             RegexOptions.CultureInvariant) &&
         Text(candidate, "status") == "qualified-statistics-dependent-calendar-candidate-not-published" &&
         Text(candidate, "packageId") == Id &&
         Text(candidate, "historicalSourceVersion") == "0.4.0-preview.6" &&
         Text(candidate, "candidateArchivePath") ==
             "build/artifacts/rs157-statistics-candidate/candidate/" + Id + "." + version + ".nupkg" &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 2000 &&
         Text(candidate, "candidateArchiveSha256").Length == 64 &&
         Text(candidate, "candidateDllSha256").Length == 64 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
         Text(candidate, "dependencyQuantitiesId") == DependencyId &&
         Text(candidate, "dependencyQuantitiesVersion") == DependencyVersion &&
         Text(candidate, "dependencyQuantitiesArchiveSha256") == DependencySha &&
         Text(candidate, "dependencyQuantitiesDllSha256") == DependencyDll &&
         candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 1 &&
         Flag(candidate, "originalEightSourceFilesLinked") &&
         Flag(candidate, "sourceProjectUnmodified") &&
         Flag(candidate, "temporaryPackagingProjection") &&
         Flag(candidate, "sourceFeedInventoryUnchanged") &&
         Flag(candidate, "independentPackageReference") &&
         candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 18 &&
         !Flag(candidate, "published") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "publicLicenseSelected") &&
         !Flag(candidate, "installedRollbackQualified") &&
         Text(candidate, "feedSelector") == "NUGET_LOCAL" &&
         Text(candidate, "targetFramework") == "net10.0" &&
         !string.IsNullOrWhiteSpace(Text(candidate, "sdkVersion")),
         "dependent Statistics candidate/archive/consumer qualification incomplete");
    Need(Text(candidate, "originalProjectSha256") == Sha(Project) &&
         Text(candidate, "originalSourceLockSha256") == Sha(Lock),
         "original project or lockfile SHA-256 drifted");
    var sourceHashes = candidate.GetProperty("originalSourcesSha256");
    Need(sourceHashes.EnumerateObject().Count() == 9,
         "eight original Statistics C# files plus lockfile fingerprint required");
    foreach (var item in sourceHashes.EnumerateObject())
        Need(File.Exists(item.Name) && Sha(item.Name) == item.Value.GetString(),
             "original Statistics source/lockfile changed: " + item.Name);
    var inner = candidate.GetProperty("commands").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 9 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "f1-inventory-before", "statistics-linked-restore",
             "statistics-linked-build", "statistics-candidate-pack",
             "consumer-restore", "consumer-build", "consumer-binary-run",
             "f1-inventory-after", "sdk"
         }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "tool-restore", "tool-build", "statistics-candidate", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "nine inner and four outer actual process receipts required");
}
Console.WriteLine(completed ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
