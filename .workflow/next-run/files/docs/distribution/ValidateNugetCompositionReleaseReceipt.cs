#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json";
const string Status = "rs166-composition-private-receipt-two-dependencies-corrected-unpublished";
const string Ready = "MATH-COMPOSITION-F33:RECEIPT=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified = "MATH-COMPOSITION-F33:RECEIPT=CORRECTED;BUILD=PASS;NATIVE=541";
const string Limits = "MATH-COMPOSITION-F33:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string Controller = "tools/dw.tools.math.nuget-composition-release/Program.cs";
const string Historical = ".workflow/runs/RS165/staged/" + Controller;
const string Project = "tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj";
const string PackageProps = "tools/dw.tools.math.nuget-composition-release/Directory.Packages.props";
const string Candidate = "docs/distribution/nuget-federation-f31-composition-candidate-qualified.json";
const string Rehearsal = "docs/distribution/nuget-federation-f32-composition-publisher-qualified.json";
const string Dependency = "docs/distribution/nuget-federation-f13-ir-published-qualified.json";
const string Quantities = "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
const string SourceSha = "08a7dccd4fd8e7df6f0e94f17a37dafaadad68c94a6d3e8b9bb28bfeebf64f30";
const string ArchiveSha = "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f";
const string DllSha = "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252";
var files = new[]
{
    "docs/distribution/nuget-federation-f33-composition-release-receipt.md",
    "docs/distribution/ValidateNugetCompositionReleaseReceipt.cs",
    Controller
};

static void Need(bool truth, string reason)
{
    if (!truth) throw new InvalidDataException("RS166 Composition receipt: " + reason);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string key) => node.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Required provenance property missing: " + key);
static bool Flag(JsonElement node, string key) => node.GetProperty(key).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var field in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(field, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}
static string ReplaceExactlyOnce(string source, string before, string after)
{
    Need(source.Split(before, StringSplitOptions.None).Length == 2,
        "historical controller does not have exactly one expected original span");
    return source.Replace(before, after, StringComparison.Ordinal);
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only observational --check supported");
foreach (var path in files.Append(Historical).Append(Project).Append(PackageProps))
    Need(File.Exists(path), "missing immutable source: " + path);
using var prior = Read(".aura/workflow/reports/RS165/attempt-001.json");
using var f31 = Read(Candidate);
using var f32 = Read(Rehearsal);
using var f13 = Read(Dependency);
using var f4 = Read(Quantities);
using var plan = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
using var guidance = Read("docs/workflow/manifest.json");
var report = prior.RootElement;
var failures = report.GetProperty("priorInvocationFailures");
Need(Text(report, "runId") == "RS165" &&
     report.GetProperty("attempt").GetInt32() == 1 &&
     Text(report, "outcome") == "succeeded" &&
     Text(report.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(report.GetProperty("git"), "commitMessage") ==
         "test: qualify Composition immutable NuGet publisher rehearsal with two dependencies" &&
     failures.GetProperty("count").GetInt32() == 2 &&
     failures.GetProperty("preparationFailureCount").GetInt32() == 2 &&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     !Flag(failures, "firstPassSucceeded"),
     "RS165 result must preserve two pre-attempt refusals, without first-pass claim");
var candidate = f31.RootElement.GetProperty("candidate");
var trial = f32.RootElement.GetProperty("rehearsal");
Need(Text(f31.RootElement, "status") ==
         "rs164-composition-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(f31.RootElement, "package_published") &&
     Text(candidate, "packageId") == "dw.tools.math.composition" &&
     Text(candidate, "packageVersion") == "20260.20261107.1001" &&
     Text(candidate, "candidateArchiveSha256") == ArchiveSha &&
     Text(candidate, "candidateDllSha256") == DllSha &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 17101 &&
     candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
     candidate.GetProperty("originalSourceProjectReferences").GetInt32() == 2 &&
     Flag(candidate, "originalFourSourceFilesLinked") &&
     candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Text(f32.RootElement, "status") ==
         "rs165-composition-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(f32.RootElement, "package_published") &&
     Text(trial, "rs164CandidateArchiveSha256") == ArchiveSha &&
     trial.GetProperty("dependencyCount").GetInt32() == 2 &&
     trial.GetProperty("consumerChecks").GetInt32() == 15 &&
     Flag(trial, "disposableFeedPublished") &&
     !Flag(trial, "publishedToNugetLocal"),
     "F31 original candidate or F32 unpublished rehearsal authority changed");
Need(Text(f13.RootElement, "status") ==
         "rs146-ir-published-local-two-binaries-independently-verified" &&
     Flag(f13.RootElement, "physically_published_package_verified") &&
     Text(f4.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(f4.RootElement, "physically_published_package_verified") &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyQuantitiesArchiveSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(candidate, "dependencyQuantitiesDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     Text(candidate, "dependencyIrVersion") == "20260.20261104.1001" &&
     Text(candidate, "dependencyIrArchiveSha256") ==
         "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
     Text(candidate, "dependencyIrDllSha256") ==
         "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af",
     "independently qualified direct dependency graph changed");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "accepted Math plan or DWF guidance changed");
Need(Sha(Historical) == SourceSha &&
     Text(f32.RootElement.GetProperty("source_files"), Controller) == SourceSha &&
     Sha("src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj") ==
         Text(candidate, "originalProjectSha256") &&
     Sha("src/projects/dw.tools.math.composition/packages.lock.json") ==
         Text(candidate, "originalSourceLockSha256"),
     "historical controller or original Composition project provenance changed");

var historical = File.ReadAllText(Historical).Replace("\r\n", "\n", StringComparison.Ordinal);
var corrected = historical;
corrected = ReplaceExactlyOnce(corrected,
    "                dependencyId = BaseId,\n                dependencyVersion = BaseVersion,",
    "                directDependencyCount = Dependencies.Length,\n" +
    "                directDependencies = new[]\n" +
    "                {\n" +
    "                    new\n" +
    "                    {\n" +
    "                        id = QuantitiesId,\n" +
    "                        version = QuantitiesVersion,\n" +
    "                        archiveSha256 = QuantitiesArchiveSha,\n" +
    "                        packagedDllSha256 = QuantitiesDllSha\n" +
    "                    },\n" +
    "                    new\n" +
    "                    {\n" +
    "                        id = BaseId,\n" +
    "                        version = BaseVersion,\n" +
    "                        archiveSha256 = BaseArchiveSha,\n" +
    "                        packagedDllSha256 = BaseDllSha\n" +
    "                    }\n" +
    "                },");
corrected = ReplaceExactlyOnce(corrected,
    "            \"|dependency=\" + BaseVersion +\n" +
    "            \"|feed=NUGET_LOCAL|verification=pending\");",
    "            \"|dependencies=\" + QuantitiesId + \"@\" + QuantitiesVersion +\n" +
    "            \",\" + BaseId + \"@\" + BaseVersion +\n" +
    "            \"|feed=NUGET_LOCAL|verification=pending\");");
corrected = ReplaceExactlyOnce(corrected,
    "                \"|dependency=\" + BaseVersion + \"|feed=NUGET_LOCAL|PASS\");",
    "                \"|dependencies=\" + QuantitiesId + \"@\" + QuantitiesVersion +\n" +
    "                \",\" + BaseId + \"@\" + BaseVersion +\n" +
    "                \"|feed=NUGET_LOCAL|PASS\");");
var current = File.ReadAllText(Controller).Replace("\r\n", "\n", StringComparison.Ordinal);
Need(corrected == current, "corrected complete source must equal only the three authorized substitutions");
Need(current.Contains("RequireQualifiedRunAndCleanCheckout(root)", StringComparison.Ordinal) &&
     current.Contains("PackageDllSha(candidate, PackageId, Version, Dependencies)",
         StringComparison.Ordinal) &&
     current.Contains("MATH-COMPOSITION-BINARY|PASS|15", StringComparison.Ordinal) &&
     current.Contains("directDependencyCount = Dependencies.Length", StringComparison.Ordinal) &&
     current.Contains("id = QuantitiesId", StringComparison.Ordinal) &&
     current.Contains("id = BaseId", StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains(
         "<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(PackageProps).Contains(
         "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
         StringComparison.Ordinal) &&
     File.ReadAllText(files[0]).Contains(
         "## Exact RS166 correction and provenance", StringComparison.Ordinal),
     "bounded corrected release mode and source document changed");

var finished = File.Exists(Evidence);
if (finished)
{
    using var result = Read(Evidence);
    var record = result.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.tools.math.composition" &&
         Text(record, "preceding_run") == "RS165" &&
         Text(record, "sourceBaselineCommit") ==
             "732bf99a8a0a7e760943654acc1f34766be66732" &&
         Text(record, "historical_controller_sha256") == SourceSha &&
         Text(record, "corrected_controller_sha256") == Sha(Controller) &&
         Text(record, "original_archive_sha256") == ArchiveSha &&
         Text(record, "original_dll_sha256") == DllSha &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         record.GetProperty("direct_dependency_count").GetInt32() == 2 &&
         Flag(record, "release_controller_compiled") &&
         !Flag(record, "release_modes_invoked") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "public_registry_published") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "external_requests_transmitted"),
         "F33 receipt claims unsupported publication or changed provenance");
    var depend = record.GetProperty("direct_dependencies").EnumerateArray().ToArray();
    Need(depend.Length == 2 &&
         Text(depend[0], "id") == "dw.quantities" &&
         Text(depend[0], "version") == "20260.20261102.1001" &&
         Text(depend[1], "id") == "dw.tools.math.ir" &&
         Text(depend[1], "version") == "20260.20261104.1001",
         "corrected receipt must retain both exact original direct dependencies");
    foreach (var file in files)
        Need(Text(record.GetProperty("source_files"), file) == Sha(file),
             "staged source SHA differs: " + file);
    var commands = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.Length == 3 &&
         commands.Select(x => Text(x, "id"))
             .SequenceEqual(new[] { "tool-restore", "tool-build", "sdk" }) &&
         commands.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         !string.IsNullOrWhiteSpace(Text(record, "sdk_version")),
         "three real tool compilation/SDK process receipts missing");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
