#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f6-expression-publisher-qualified.json";
const string Status = "rs139-expression-immutable-local-publisher-rehearsal-qualified-unpublished";
const string Before = "MATH-EXPRESSION-F6:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string After = "MATH-EXPRESSION-F6:REHEARSAL=QUALIFIED;BINARY=6;NATIVE=541";
const string Restrictions = "MATH-EXPRESSION-F6:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f6-expression-publisher.md",
    "docs/distribution/ValidateNugetExpressionPublisher.cs",
    "tools/dw.tools.math.nuget-expression-release/dw.tools.math.nuget-expression-release.csproj",
    "tools/dw.tools.math.nuget-expression-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-expression-release/Program.cs"
};
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS139 Expression release: " + message);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement element, string key) => element.GetProperty(key).GetString() ??
    throw new InvalidDataException("Missing string field " + key);
static bool Flag(JsonElement element, string key) => element.GetProperty(key).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static bool IsSha(string text) =>
    Regex.IsMatch(text, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
static int Count(JsonElement element)
{
    var count = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (element.TryGetProperty(name, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is admitted");
foreach (var file in files)
    Need(File.Exists(file), "missing source: " + file);

using var previous = Read(".aura/workflow/reports/RS138/attempt-001.json");
using var prior = Read("docs/distribution/nuget-federation-f5-expression-candidate-qualified.json");
using var publishedBase = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");
var history = previous.RootElement;
Need(Text(history, "runId") == "RS138" &&
     Text(history, "outcome") == "succeeded" &&
     history.GetProperty("attempt").GetInt32() == 1 &&
     history.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(history.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(history.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     Text(history.GetProperty("git"), "commitMessage") ==
         "test: qualify exact NuGet Expression candidate with published base",
     "RS138 DWF first-pass lineage changed");
Need(Text(prior.RootElement, "status") ==
         "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished" &&
     !Flag(prior.RootElement, "package_published") &&
     Text(publishedBase.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(publishedBase.RootElement, "physically_published_package_verified"),
     "RS137/RS138 owner receipts not authoritative");
var candidate = prior.RootElement.GetProperty("candidate");
Need(Text(candidate, "packageId") == "dw.quantities.expression" &&
     Text(candidate, "packageVersion") == "20260.20261102.1001" &&
     Text(candidate, "candidateArchiveSha256") ==
         "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13951 &&
     Text(candidate, "candidateDllSha256") ==
         "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
     Text(candidate, "exactBasePackageId") == "dw.quantities" &&
     Text(candidate, "exactBasePackageVersion") == "20260.20261102.1001" &&
     Text(candidate, "exactBasePackageSha256") ==
         "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(candidate, "exactBaseDllSha256") ==
         "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     candidate.GetProperty("packagedDependencies").GetInt32() == 1 &&
     candidate.GetProperty("consumerChecks").GetInt32() == 6 &&
     Flag(candidate, "actualSourceFreeConsumerPassed"),
     "original expression package binary and exact dependency have drifted");
var original = prior.RootElement.GetProperty("source_files");
foreach (var source in original.EnumerateObject())
    Need(Sha(source.Name) == source.Value.GetString(),
         "RS138 pinned source changed: " + source.Name);
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones")
         .EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "Math source roadmap, native DWF plan or guidance changed");

const string Tool = "tools/dw.tools.math.nuget-expression-release/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-expression-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
             StringComparison.Ordinal),
     "the shared publisher reference is not exact and central");

var complete = File.Exists(Evidence);
if (complete)
{
    using var receipt = Read(Evidence);
    var record = receipt.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.quantities.expression" &&
         Text(record, "feed_selector") == "NUGET_LOCAL" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(record, "package_published") &&
         !Flag(record, "release_authorized_by_this_run") &&
         !Flag(record, "source_versions_modified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "DWF receipt cannot claim private shared publication or unrelated rights");
    var sources = record.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(sources, file) == Sha(file),
             "RS139 staged file SHA differs from receipt: " + file);
    var trial = record.GetProperty("rehearsal");
    Need(Text(trial, "status") == "expression-real-dependent-package-publisher-qualified" &&
         Text(trial, "packageId") == "dw.quantities.expression" &&
         Text(trial, "version") == "20260.20261102.1001" &&
         Text(trial, "historicalVersion") == "0.2.0-preview.1" &&
         Text(trial, "rs138CandidateArchiveSha256") ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         Text(trial, "rs138CandidateDllSha256") ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         IsSha(Text(trial, "rehearsalArchiveSha256")) &&
         IsSha(Text(trial, "rehearsalDllSha256")) &&
         Text(trial, "exactDependencyId") == "dw.quantities" &&
         Text(trial, "exactDependencyVersion") == "20260.20261102.1001" &&
         Text(trial, "dependencyArchiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(trial, "dependencyDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(trial, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(trial, "publisherDllSha256") ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial.GetProperty("checks").GetInt32() == 7 &&
         trial.GetProperty("consumerChecks").GetInt32() == 6 &&
         Flag(trial, "disposableFeedPublished") &&
         !Flag(trial, "publishedToNugetLocal") &&
         Flag(trial, "invalidHashRefused") &&
         Flag(trial, "duplicateIdentityRefused") &&
         Flag(trial, "cancelledRefused") &&
         Flag(trial, "packagedSourceFilesUnchanged") &&
         Flag(trial, "historicalProjectVersionUnchanged") &&
         Flag(trial, "sourceFeedBaseArchiveUnchanged") &&
         !Flag(trial, "installedPackageRollbackQualified") &&
         !Flag(trial, "sourceReleaseAuthorization"),
         "real Expression package publisher rehearsal did not pass each refusal boundary");
    var ids = new[]
    {
        "source-restore", "source-build", "source-pack",
        "consumer-restore", "consumer-build", "consumer-binary", "sdk"
    };
    var cmds = trial.GetProperty("commandReceipts").EnumerateArray().ToArray();
    Need(cmds.Length == ids.Length &&
         cmds.Select(c => Text(c, "id")).SequenceEqual(ids) &&
         cmds.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "required seven real source/package and binary consumer commands are missing");
    var outerIds = new[] { "tool-restore", "tool-build", "publisher-rehearsal", "sdk" };
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outer.Length == outerIds.Length &&
         outer.Select(c => Text(c, "id")).SequenceEqual(outerIds) &&
         outer.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "DWF did not build and run the actual shared publisher test");
}
Console.WriteLine(complete ? After : Before);
Console.WriteLine(Restrictions);
return 0;
