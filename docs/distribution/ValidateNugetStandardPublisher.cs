#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f9-standard-publisher-qualified.json";
const string Status = "rs142-standard-immutable-local-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-STANDARD-F9:SOURCE=READY;PUBLISHER=PENDING;NATIVE=541";
const string Qualified = "MATH-STANDARD-F9:REHEARSAL=QUALIFIED;BINARY=10;NATIVE=541";
const string Limits = "MATH-STANDARD-F9:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f9-standard-publisher.md",
    "docs/distribution/ValidateNugetStandardPublisher.cs",
    "tools/dw.tools.math.nuget-standard-release/dw.tools.math.nuget-standard-release.csproj",
    "tools/dw.tools.math.nuget-standard-release/Directory.Packages.props",
    "tools/dw.tools.math.nuget-standard-release/Program.cs"
};
static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS142 Standard release: " + reason);
}
static JsonDocument Open(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string key) =>
    node.GetProperty(key).GetString() ??
    throw new InvalidDataException("Required property absent: " + key);
static bool Flag(JsonElement node, string key) => node.GetProperty(key).GetBoolean();
static string Sha(string file) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
static bool Hash64(string sha) =>
    Regex.IsMatch(sha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var collection))
            total += collection.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check is accepted");
foreach (var file in files)
    Need(File.Exists(file), "missing staged complete source " + file);

using var report = Open(".aura/workflow/reports/RS141/attempt-001.json");
using var candidateDoc = Open("docs/distribution/nuget-federation-f8-standard-candidate-qualified.json");
using var publishedBase = Open("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var publishedExpression = Open("docs/distribution/nuget-federation-f7-expression-published-qualified.json");
using var backlog = Open("docs/planning/backlog.json");
using var plan = Open(".aura/workflow/plan/project.json");
using var guidance = Open("docs/workflow/manifest.json");
Need(Text(report.RootElement, "runId") == "RS141" &&
     report.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(report.RootElement, "outcome") == "succeeded" &&
     Text(report.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     report.RootElement.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(report.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(report.RootElement.GetProperty("git"), "commitMessage") ==
        "test: qualify Standard NuGet candidate with two exact dependencies",
     "RS141 successful first-pass DWF authority has changed");

Need(Text(candidateDoc.RootElement, "status") ==
         "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished" &&
     !Flag(candidateDoc.RootElement, "package_published") &&
     Text(publishedBase.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(publishedBase.RootElement, "physically_published_package_verified") &&
     Text(publishedExpression.RootElement, "status") ==
         "rs140-expression-published-local-binary-independently-verified" &&
     Flag(publishedExpression.RootElement, "physically_published_package_verified") &&
     Flag(publishedExpression.RootElement, "dependency_package_verified"),
     "the source candidate or two independently published dependencies are missing");
var candidate = candidateDoc.RootElement.GetProperty("candidate");
Need(Text(candidate, "packageId") == "dw.quantities.standard" &&
     Text(candidate, "packageVersion") == "20260.20261103.1001" &&
     Text(candidate, "historicalSourceVersion") == "0.2.0-preview.1" &&
     Text(candidate, "candidateArchivePath") ==
        "build/artifacts/rs141-standard-candidate/candidate/dw.quantities.standard.20260.20261103.1001.nupkg" &&
     Text(candidate, "candidateArchiveSha256") ==
        "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
     candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13604 &&
     Text(candidate, "candidateDllSha256") ==
        "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
     Text(candidate, "dependencyQuantitiesId") == "dw.quantities" &&
     Text(candidate, "dependencyQuantitiesVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyQuantitiesArchiveSha256") ==
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
     Text(candidate, "dependencyQuantitiesDllSha256") ==
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
     Text(candidate, "dependencyExpressionId") == "dw.quantities.expression" &&
     Text(candidate, "dependencyExpressionVersion") == "20260.20261102.1001" &&
     Text(candidate, "dependencyExpressionArchiveSha256") ==
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
     Text(candidate, "dependencyExpressionDllSha256") ==
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
     candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
     candidate.GetProperty("consumerChecks").GetInt32() == 10 &&
     Flag(candidate, "independentlyCompiledBinaryConsumer") &&
     !Flag(candidate, "published"),
     "exact Standard original archive or two NuGet dependency identities changed");

foreach (var pinned in candidateDoc.RootElement.GetProperty("source_files").EnumerateObject())
    Need(File.Exists(pinned.Name) && Sha(pinned.Name) == pinned.Value.GetString(),
         "previous Standard qualification source changed: " + pinned.Name);

Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "Math native 541-node completion or project guidance changed");

const string Tool = "tools/dw.tools.math.nuget-standard-release/";
Need(File.ReadAllText(Tool + "dw.tools.math.nuget-standard-release.csproj")
        .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
            StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "Directory.Packages.props")
        .Contains("<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
            StringComparison.Ordinal),
     "only exact published shared NuGet package reader is approved");

var completed = File.Exists(Evidence);
if (completed)
{
    using var doc = Open(Evidence);
    var record = doc.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.quantities.standard" &&
         Text(record, "feed_selector") == "NUGET_LOCAL" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(record, "package_published") &&
         !Flag(record, "release_authorized_by_this_run") &&
         !Flag(record, "source_versions_modified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "run does not authorize shared publication, licensing, rollback or integration");
    var hashes = record.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(hashes, file) == Sha(file),
             "RS142 staged source hash differs: " + file);
    var trial = record.GetProperty("rehearsal");
    Need(Text(trial, "status") == "standard-real-two-dependency-publisher-qualified" &&
         Text(trial, "packageId") == "dw.quantities.standard" &&
         Text(trial, "version") == "20260.20261103.1001" &&
         Text(trial, "historicalVersion") == "0.2.0-preview.1" &&
         Text(trial, "rs141ArchiveSha256") ==
            "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
         Text(trial, "rs141DllSha256") ==
            "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         Hash64(Text(trial, "rehearsalArchiveSha256")) &&
         Hash64(Text(trial, "rehearsalDllSha256")) &&
         trial.GetProperty("dependencyCount").GetInt32() == 2 &&
         Text(trial, "publisher") == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         Text(trial, "publisherDllSha256") ==
            "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial.GetProperty("controls").GetInt32() == 7 &&
         trial.GetProperty("consumerChecks").GetInt32() == 10 &&
         Flag(trial, "collisionRefused") &&
         Flag(trial, "alteredDigestRefused") &&
         Flag(trial, "cancelledRefused") &&
         Flag(trial, "disposableFeedPublished") &&
         !Flag(trial, "publishedToNugetLocal") &&
         Flag(trial, "sourceFilesUnchanged") &&
         Flag(trial, "originalProjectVersionUnchanged") &&
         Flag(trial, "historicalProjectReferencesPreserved") &&
         Flag(trial, "publishedDependencyArchivesUnchanged") &&
         !Flag(trial, "sourceReleaseAuthorization") &&
         !Flag(trial, "installedPackageRollbackQualified"),
         "Standard real-package disposable shared-publisher rehearsal incomplete");

    var dependencies = trial.GetProperty("dependencies").EnumerateArray().ToArray();
    Need(dependencies.Length == 2 &&
         Text(dependencies[0], "id") == "dw.quantities" &&
         Text(dependencies[0], "version") == "20260.20261102.1001" &&
         Text(dependencies[0], "archiveSha256") ==
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(dependencies[0], "dllSha256") ==
            "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(dependencies[1], "id") == "dw.quantities.expression" &&
         Text(dependencies[1], "version") == "20260.20261102.1001" &&
         Text(dependencies[1], "archiveSha256") ==
            "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         Text(dependencies[1], "dllSha256") ==
            "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1",
         "both independently published NuGet dependency pins must remain exact");

    var inner = trial.GetProperty("commands").EnumerateArray().ToArray();
    var innerIds = new[] {
        "source-restore", "source-build", "source-pack",
        "consumer-restore", "consumer-build", "consumer-binary", "sdk"
    };
    Need(inner.Length == innerIds.Length &&
         inner.Select(x => Text(x, "id")).SequenceEqual(innerIds) &&
         inner.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "seven actual source/rehearsal/consumer command receipts are required");
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    var outerIds = new[] { "tool-restore", "tool-build", "publisher-rehearsal", "sdk" };
    Need(outer.Length == outerIds.Length &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerIds) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "parent DWF must compile and execute the real Standard publisher rehearsal");
}
Console.WriteLine(completed ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
