#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f31-composition-candidate-qualified.json";
const string Status = "rs164-composition-dependent-calendar-candidate-qualified-unpublished";
const string Ready = "MATH-COMPOSITION-F31:SOURCE=READY;DEPENDENCIES=PINNED;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-COMPOSITION-F31:CANDIDATE=QUALIFIED;BINARY=15;DEPENDENCY=2;NATIVE=541";
const string Limits = "MATH-COMPOSITION-F31:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string Id = "dw.tools.math.composition";
const string Project = "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string SourceLock = "src/projects/dw.tools.math.composition/packages.lock.json";
const string Fixture = "tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs";
const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
const string IrDll = "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af";
const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string QuantitiesDll = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
var files = new[]
{
    "docs/distribution/nuget-federation-f31-composition-candidate.md",
    "docs/distribution/ValidateNugetCompositionCandidate.cs",
    "tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj",
    "tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-composition-candidate/Program.cs",
    Fixture
};
static void Need(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS164 Composition candidate: " + reason);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string key) => node.GetProperty(key).GetString() ??
    throw new InvalidDataException("Missing exact field: " + key);
static bool Flag(JsonElement node, string key) => node.GetProperty(key).GetBoolean();
static string Sha(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is admitted");
foreach (var path in files) Need(File.Exists(path), "missing installed source: " + path);

using var report = Read(".aura/workflow/reports/RS163/attempt-001.json");
using var f30 = Read("docs/distribution/nuget-federation-f30-codecs-published-qualified.json");
using var f13 = Read("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var inventory = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
using var rights = Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");

var previous = report.RootElement;
Need(Text(previous, "runId") == "RS163" &&
     previous.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous, "outcome") == "succeeded" &&
     Text(previous.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     previous.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1 &&
     !Flag(previous.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(previous.GetProperty("git"), "commitMessage") ==
         "test: independently verify published Codecs IR and Quantities NuGet binaries",
     "exact RS163 DWF predecessor with retained pre-attempt refusal required");
Need(Text(f30.RootElement, "status") ==
         "rs163-codecs-published-local-three-binaries-independently-verified" &&
     Flag(f30.RootElement, "physically_published_package_verified") &&
     Flag(f30.RootElement, "dependency_package_verified") &&
     !Flag(f30.RootElement, "package_published_by_this_run") &&
     f30.RootElement.GetProperty("observation")
         .GetProperty("compiledConsumerChecks").GetInt32() == 15,
     "F30 physically qualified ninth Math package must remain retained");
var ir = f13.RootElement.GetProperty("observation");
Need(Text(f13.RootElement, "status") ==
         "rs146-ir-published-local-two-binaries-independently-verified" &&
     Flag(f13.RootElement, "physically_published_package_verified") &&
     Text(ir, "packageId") == "dw.tools.math.ir" &&
     Text(ir, "packageVersion") == "20260.20261104.1001" &&
     ir.GetProperty("archiveBytes").GetInt32() == 21926 &&
     Text(ir, "archiveSha256") == IrSha &&
     Text(ir, "packagedDllSha256") == IrDll &&
     ir.GetProperty("declaredDependencyCount").GetInt32() == 1,
     "exact physical IR package F13 readback drifted");
var deps = ir.GetProperty("exactDependencies").EnumerateArray().ToArray();
Need(deps.Length == 1 &&
     Text(deps[0], "id") == "dw.quantities" &&
     Text(deps[0], "version") == "20260.20261102.1001" &&
     deps[0].GetProperty("archiveBytes").GetInt32() == 14747 &&
     Text(deps[0], "archiveSha256") == QuantitiesSha &&
     Text(deps[0], "packagedDllSha256") == QuantitiesDll,
     "transitive published Quantities package qualified in F13 drifted");
Need(inventory.RootElement.GetProperty("package_count").GetInt32() == 13,
    "F1 canonical inventory must preserve all thirteen source projects");
var packages = inventory.RootElement.GetProperty("observed")
    .GetProperty("packages").EnumerateArray().ToArray();
Need(packages.Length == 13, "F1 original inventory count drifted");
var composition = packages.Single(p => Text(p, "packageId") == Id);
Need(Text(composition, "sourceProject") == Project &&
     Text(composition, "historicalSourceVersion") == "0.3.0-preview.1" &&
     Text(composition, "effectiveTargetFramework") == "net10.0" &&
     Text(composition, "effectivePackable") == "true" &&
     composition.GetProperty("projectReferences").EnumerateArray()
         .Select(x => x.GetString()).SequenceEqual(["dw.quantities", "dw.tools.math.ir"]) &&
     composition.GetProperty("externalPackageReferences").GetArrayLength() == 0 &&
     !Flag(composition, "candidateIsReserved"),
     "Composition historical source package and one direct dependency graph changed");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done") &&
     Flag(rights.RootElement.GetProperty("authorization"), "package_from_dw_tools_math"),
     "accepted Math roadmap or packaging rights changed");
Need(File.ReadAllText(Project).Contains("<Version>0.3.0-preview.1</Version>",
        StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains(
         "<ProjectReference Include=\"../dw.tools.math.ir/dw.tools.math.ir.csproj\" />",
         StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains(
         "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />",
         StringComparison.Ordinal),
     "original Composition project version/source reference changed");
using (var locked = Read(SourceLock))
{
    var dependencies = locked.RootElement.GetProperty("dependencies")
        .GetProperty("net10.0").EnumerateObject().ToArray();
    Need(locked.RootElement.GetProperty("version").GetInt32() == 2 &&
         dependencies.Length == 2 &&
         dependencies.Any(x => x.Name == "dw.quantities" && Text(x.Value, "type") == "Project") &&
         dependencies.Any(x => x.Name == "dw.tools.math.ir" && Text(x.Value, "type") == "Project"),
         "original Composition lockfile must retain two direct dependencies");
}
const string Tool = "tools/dw.tools.math.nuget-composition-candidate/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-composition-candidate.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("originalSourceLockSha256 = ShaFile(lockPath)", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Program.cs")
         .Contains("MATH-COMPOSITION-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Fixture).Contains("MATH-COMPOSITION-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(files[0]).Contains(
         "## Bounded F31 unpublished candidate", StringComparison.Ordinal),
     "Composition candidate build and independent binary contract missing");

var finished = File.Exists(Evidence);
if (finished)
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
         "F31 cannot assert publication, rollback or cross-repository migration");
    foreach (var file in files)
        Need(Text(result.GetProperty("source_files"), file) == Sha(file),
             "installed Composition candidate source SHA-256 changed " + file);
    var candidate = result.GetProperty("candidate");
    var version = Text(candidate, "packageVersion");
    Need(Regex.IsMatch(version, @"^20260\.2026\d{4}\.1\d{3}$",
             RegexOptions.CultureInvariant) &&
         Text(candidate, "status") == "qualified-composition-dependent-calendar-candidate-not-published" &&
         Text(candidate, "packageId") == Id &&
         Text(candidate, "historicalSourceVersion") == "0.3.0-preview.1" &&
         Text(candidate, "candidateArchivePath") ==
             "build/artifacts/rs164-composition-candidate/candidate/" + Id + "." + version + ".nupkg" &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 2000 &&
         Text(candidate, "candidateArchiveSha256").Length == 64 &&
         Text(candidate, "candidateDllSha256").Length == 64 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
         Text(candidate, "dependencyIrId") == "dw.tools.math.ir" &&
         Text(candidate, "dependencyIrVersion") == "20260.20261104.1001" &&
         Text(candidate, "dependencyIrArchiveSha256") == IrSha &&
         Text(candidate, "dependencyIrDllSha256") == IrDll &&
         Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
         Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
         Text(candidate, "dependencyQuantitiesArchiveSha256") == QuantitiesSha &&
         Text(candidate, "dependencyQuantitiesDllSha256") == QuantitiesDll &&
         candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 2 &&
         Flag(candidate, "originalFourSourceFilesLinked") &&
         Flag(candidate, "sourceProjectUnmodified") &&
         Flag(candidate, "temporaryPackagingProjection") &&
         Flag(candidate, "sourceFeedInventoryUnchanged") &&
         Flag(candidate, "independentPackageReference") &&
         candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
         !Flag(candidate, "published") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "publicLicenseSelected") &&
         !Flag(candidate, "installedRollbackQualified") &&
         Text(candidate, "feedSelector") == "NUGET_LOCAL" &&
         Text(candidate, "targetFramework") == "net10.0" &&
         !string.IsNullOrWhiteSpace(Text(candidate, "sdkVersion")),
         "qualified Composition candidate, source and transitive dependency incomplete");
    Need(Text(candidate, "originalProjectSha256") == Sha(Project) &&
         Text(candidate, "originalSourceLockSha256") == Sha(SourceLock),
         "original Composition project/lockfile SHA-256 changed");
    var hashes = candidate.GetProperty("originalSourcesSha256");
    Need(hashes.EnumerateObject().Count() == 5,
         "four original Composition sources and lockfile hashes required");
    foreach (var item in hashes.EnumerateObject())
        Need(File.Exists(item.Name) && Sha(item.Name) == item.Value.GetString(),
             "original Composition source fingerprint changed " + item.Name);
    var inner = candidate.GetProperty("commands").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(inner.Length == 11 && outer.Length == 4 &&
         inner.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "f1-tool-restore", "f1-tool-build", "f1-inventory-before",
             "composition-linked-restore", "composition-linked-build", "composition-candidate-pack",
             "consumer-restore", "consumer-build", "consumer-binary-run",
             "f1-inventory-after", "sdk"
         }) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(new[]
         {
             "tool-restore", "tool-build", "composition-candidate", "sdk"
         }) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "eleven internal and four external actual command receipts required");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
