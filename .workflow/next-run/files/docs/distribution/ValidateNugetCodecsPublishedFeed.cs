#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f30-codecs-published-qualified.json";
const string Status = "rs163-codecs-published-local-three-binaries-independently-verified";
const string Ready = "MATH-CODECS-F30:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-CODECS-F30:PHYSICAL=VERIFIED;BINARY=15;DIRECT=1;TRANSITIVE=1;NATIVE=541";
const string Limits = "MATH-CODECS-F30:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string Doc = "docs/distribution/nuget-federation-f30-codecs-published.md";
const string Controller = "tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs";
const string Project = "tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj";
var staged = new[] { Doc, "docs/distribution/ValidateNugetCodecsPublishedFeed.cs", Project, Controller };

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS163 Codecs feed readback: " + message);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement obj, string property) => obj.GetProperty(property).GetString()
    ?? throw new InvalidDataException("Missing required field " + property);
static bool Flag(JsonElement obj, string property) => obj.GetProperty(property).GetBoolean();
static string Sha(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var n = 1;
    foreach (var field in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(field, out var nested))
            n += nested.EnumerateArray().Sum(Count);
    return n;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only read-only --check supported");
foreach (var file in staged) Need(File.Exists(file), "missing staged file " + file);

using var previous = Read(".aura/workflow/reports/RS162/attempt-001.json");
using var candidateReceipt = Read("docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json");
using var rehearsalReceipt = Read("docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json");
using var irReceipt = Read("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var quantitiesReceipt = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

var predecessor = previous.RootElement;
Need(Text(predecessor, "runId") == "RS162" &&
     predecessor.GetProperty("attempt").GetInt32() == 1 &&
     Text(predecessor, "outcome") == "succeeded" &&
     Text(predecessor.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     predecessor.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(predecessor.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(predecessor.GetProperty("git"), "commitMessage") ==
        "test: qualify Codecs immutable NuGet publisher rehearsal with IR closure",
     "exact first-pass RS162 predecessor is not retained");

var qualified = candidateReceipt.RootElement.GetProperty("candidate");
var rehearsal = rehearsalReceipt.RootElement.GetProperty("rehearsal");
Need(Text(candidateReceipt.RootElement, "status") ==
         "rs161-codecs-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(candidateReceipt.RootElement, "package_published") &&
     Text(qualified, "packageId") == "dw.tools.math.codecs" &&
     Text(qualified, "packageVersion") == "20260.20261107.1001" &&
     qualified.GetProperty("candidateArchiveBytes").GetInt32() == 35096 &&
     Text(qualified, "candidateArchiveSha256") ==
         "00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35" &&
     Text(qualified, "candidateDllSha256") ==
         "3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74" &&
     qualified.GetProperty("packageDependencies").GetInt32() == 1 &&
     qualified.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     Flag(qualified, "independentPackageReference") &&
     Text(qualified, "dependencyIrId") == "dw.tools.math.ir" &&
     Text(qualified, "dependencyIrVersion") == "20260.20261104.1001" &&
     Text(qualified, "dependencyQuantitiesId") == "dw.quantities" &&
     Text(qualified, "dependencyQuantitiesVersion") == "20260.20261102.1001",
     "F28 exact original unpublished Codecs candidate drifted");
Need(Text(rehearsalReceipt.RootElement, "status") ==
         "rs162-codecs-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsalReceipt.RootElement, "package_published") &&
     Text(rehearsal, "rs161CandidateArchiveSha256") ==
         "00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35" &&
     rehearsal.GetProperty("consumerChecks").GetInt32() == 15 &&
     Flag(rehearsal, "disposableFeedPublished") &&
     !Flag(rehearsal, "publishedToNugetLocal"),
     "F29 disposable publisher must not be mistaken for private publication");
Need(Text(irReceipt.RootElement, "status") ==
         "rs146-ir-published-local-two-binaries-independently-verified" &&
     Flag(irReceipt.RootElement, "physically_published_package_verified") &&
     Text(quantitiesReceipt.RootElement, "status") ==
         "rs137-published-local-math-nuget-binary-independently-verified" &&
     Flag(quantitiesReceipt.RootElement, "physically_published_package_verified"),
     "F13/F4 independent dependency proofs absent");
Need(Text(backlog.RootElement, "plan_version") == "0.1.122" &&
     Text(backlog.RootElement, "status") == "done" &&
     Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Text(x, "state") == "done"),
     "canonical plan, guidance or completed native scope drifted");
Need(File.ReadAllText(Doc).Contains("## Independent physical readback", StringComparison.Ordinal) &&
     File.ReadAllText(Controller).Contains("physical-feed-snapshot", StringComparison.Ordinal) &&
     File.ReadAllText(Controller).Contains("MATH-CODECS-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Controller).Contains("publicationInvoked = false", StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs"),
     "independent snapshot consumer or source fixture missing");

var finished = File.Exists(Evidence);
if (finished)
{
    using var result = Read(Evidence);
    var record = result.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.tools.math.codecs" &&
         Text(record, "previous_owner_run") == "RS162" &&
         Text(record, "sourceBaselineCommit") ==
             "8577c235ff3fe1fa37e39767dac281f114aef56d" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(record, "physically_published_package_verified") &&
         Flag(record, "dependency_package_verified") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "remote_registry_published") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "F30 exact outcome or independent publication scope incorrect");
    foreach (var file in staged)
        Need(Text(record.GetProperty("source_files"), file) == Sha(file),
             "staged F30 source hash mismatch " + file);

    var observed = record.GetProperty("observation");
    Need(Text(observed, "status") ==
         "codecs-published-local-three-binaries-independently-verified" &&
         Text(observed, "packageId") == "dw.tools.math.codecs" &&
         Text(observed, "packageVersion") == "20260.20261107.1001" &&
         observed.GetProperty("archiveBytes").GetInt32() == 35096 &&
         Text(observed, "archiveSha256") ==
            "00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35" &&
         Text(observed, "packagedDllSha256") ==
            "3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74" &&
         Text(observed, "loadedDllSha256") ==
            "3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 1 &&
         observed.GetProperty("transitiveDependencyCount").GetInt32() == 1 &&
         observed.GetProperty("publishedPackageSnapshotCount").GetInt32() == 3 &&
         observed.GetProperty("compiledConsumerChecks").GetInt32() == 15 &&
         Flag(observed, "physicalArchivesInspected") &&
         Flag(observed, "publishedArchivesUnchanged") &&
         Flag(observed, "independentPackageReference") &&
         !Flag(observed, "originalMathProjectReferencesUsed") &&
         !Flag(observed, "feedWideInventoryPerformed") &&
         !Flag(observed, "publicationInvoked") &&
         !Flag(observed, "remoteRegistryPublished") &&
         !Flag(observed, "installedPackageRollbackQualified") &&
         !Flag(observed, "otherOsQualified") &&
         !Flag(observed, "discoveryMigrationQualified") &&
         Text(observed, "sourceFeed") == "NUGET_LOCAL" &&
         !string.IsNullOrWhiteSpace(Text(observed, "sdkVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "runtimeVersion")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "osDescription")) &&
         !string.IsNullOrWhiteSpace(Text(observed, "processArchitecture")),
         "physical published Codecs evidence incomplete");
    var direct = observed.GetProperty("exactDependencies").EnumerateArray().ToArray();
    var transitive = observed.GetProperty("transitiveDependencies").EnumerateArray().ToArray();
    Need(direct.Length == 1 && transitive.Length == 1 &&
         Text(direct[0], "id") == "dw.tools.math.ir" &&
         Text(direct[0], "version") == "20260.20261104.1001" &&
         direct[0].GetProperty("archiveBytes").GetInt32() == 21926 &&
         Text(direct[0], "archiveSha256") ==
            "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(direct[0], "packagedDllSha256") ==
            "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         Text(direct[0], "loadedDllSha256") ==
            "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         Text(transitive[0], "id") == "dw.quantities" &&
         Text(transitive[0], "version") == "20260.20261102.1001" &&
         transitive[0].GetProperty("archiveBytes").GetInt32() == 14747 &&
         Text(transitive[0], "archiveSha256") ==
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(transitive[0], "packagedDllSha256") ==
            "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(transitive[0], "loadedDllSha256") ==
            "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
         "physical direct IR and transitive Quantities closure unqualified");
    var commands = observed.GetProperty("commands").EnumerateArray().ToArray();
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    var names = new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" };
    var outerNames = new[] { "tool-restore", "tool-build", "codecs-feed-verify", "sdk" };
    Need(commands.Length == 4 && outer.Length == 4 &&
         commands.Select(x => Text(x, "id")).SequenceEqual(names) &&
         outer.Select(x => Text(x, "id")).SequenceEqual(outerNames) &&
         commands.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "actual source-free consumer and outer command receipts missing");
}
Console.WriteLine(finished ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
