#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f34-composition-published-qualified.json";
const string Status = "rs167-composition-published-local-three-binaries-independently-verified";
const string Ready = "MATH-COMPOSITION-F34:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-COMPOSITION-F34:PHYSICAL=VERIFIED;BINARY=15;DIRECT=2;TRANSITIVE=1;NATIVE=541";
const string Limits = "MATH-COMPOSITION-F34:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string Doc = "docs/distribution/nuget-federation-f34-composition-published.md";
const string Controller = "tools/dw.tools.math.nuget-composition-feed-verify/Program.cs";
const string Project = "tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj";
var staged = new[] { Doc, "docs/distribution/ValidateNugetCompositionPublishedFeed.cs", Project, Controller };

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS167 Composition feed readback: " + message);
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

using var previous = Read(".aura/workflow/reports/RS166/attempt-001.json");
using var candidateReceipt = Read("docs/distribution/nuget-federation-f31-composition-candidate-qualified.json");
using var rehearsalReceipt = Read("docs/distribution/nuget-federation-f32-composition-publisher-qualified.json");
using var releaseReceipt = Read("docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json");
using var irReceipt = Read("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var quantitiesReceipt = Read("docs/distribution/nuget-federation-f4-quantities-published-qualified.json");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var guidance = Read("docs/workflow/manifest.json");

var predecessor = previous.RootElement;
Need(Text(predecessor, "runId") == "RS166" &&
     predecessor.GetProperty("attempt").GetInt32() == 1 &&
     Text(predecessor, "outcome") == "succeeded" &&
     Text(predecessor.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     predecessor.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1 &&
     !Flag(predecessor.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Text(predecessor.GetProperty("git"), "commitMessage") ==
        "fix: qualify Composition private release receipt with exact two-dependency closure",
     "exact corrected RS166 predecessor with one pre-attempt failure not retained");

var qualified = candidateReceipt.RootElement.GetProperty("candidate");
var rehearsal = rehearsalReceipt.RootElement.GetProperty("rehearsal");
Need(Text(candidateReceipt.RootElement, "status") ==
         "rs164-composition-dependent-calendar-candidate-qualified-unpublished" &&
     !Flag(candidateReceipt.RootElement, "package_published") &&
     Text(qualified, "packageId") == "dw.tools.math.composition" &&
     Text(qualified, "packageVersion") == "20260.20261107.1001" &&
     qualified.GetProperty("candidateArchiveBytes").GetInt32() == 17101 &&
     Text(qualified, "candidateArchiveSha256") ==
         "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f" &&
     Text(qualified, "candidateDllSha256") ==
         "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252" &&
     qualified.GetProperty("packageDependencies").GetInt32() == 2 &&
     qualified.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
      qualified.GetProperty("originalSourceProjectReferences").GetInt32() == 2 &&
      Flag(qualified, "originalFourSourceFilesLinked") &&
     Flag(qualified, "independentPackageReference") &&
     Text(qualified, "dependencyIrId") == "dw.tools.math.ir" &&
     Text(qualified, "dependencyIrVersion") == "20260.20261104.1001" &&
     Text(qualified, "dependencyQuantitiesId") == "dw.quantities" &&
     Text(qualified, "dependencyQuantitiesVersion") == "20260.20261102.1001",
     "F31 exact original unpublished Composition candidate drifted");
Need(Text(rehearsalReceipt.RootElement, "status") ==
         "rs165-composition-immutable-local-publisher-rehearsal-qualified-unpublished" &&
     !Flag(rehearsalReceipt.RootElement, "package_published") &&
     rehearsal.GetProperty("dependencyCount").GetInt32() == 2 &&
      Text(rehearsal, "rs164CandidateArchiveSha256") ==
         "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f" &&
     rehearsal.GetProperty("consumerChecks").GetInt32() == 15 &&
     Flag(rehearsal, "disposableFeedPublished") &&
     !Flag(rehearsal, "publishedToNugetLocal"),
     "F32 disposable publisher must not be mistaken for private publication");
Need(Text(releaseReceipt.RootElement, "status") ==
       "rs166-composition-private-receipt-two-dependencies-corrected-unpublished" &&
     !Flag(releaseReceipt.RootElement, "package_published_by_this_run") &&
     !Flag(releaseReceipt.RootElement, "release_modes_invoked") &&
     releaseReceipt.RootElement.GetProperty("direct_dependency_count").GetInt32() == 2,
     "F33 compiled controller with two direct dependencies is not proof of publication");
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
     File.ReadAllText(Controller).Contains("MATH-COMPOSITION-BINARY|PASS|15", StringComparison.Ordinal) &&
     File.ReadAllText(Controller).Contains("publicationInvoked = false", StringComparison.Ordinal) &&
     File.ReadAllText(Project).Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
     File.Exists("tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs"),
     "independent snapshot consumer or source fixture missing");

var finished = File.Exists(Evidence);
if (finished)
{
    using var result = Read(Evidence);
    var record = result.RootElement;
    Need(Text(record, "status") == Status &&
         Text(record, "source_package_id") == "dw.tools.math.composition" &&
         Text(record, "previous_owner_run") == "RS166" &&
         Text(record, "sourceBaselineCommit") ==
             "1b3f8b603fee77276681f0adf24c5a7f4a907cf0" &&
         record.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(record, "physically_published_package_verified") &&
         Flag(record, "dependency_package_verified") &&
         !Flag(record, "package_published_by_this_run") &&
         !Flag(record, "remote_registry_published") &&
         !Flag(record, "installed_package_rollback_qualified") &&
         !Flag(record, "new_public_license_selected") &&
         !Flag(record, "external_requests_transmitted") &&
         !Flag(record, "discovery_migration_qualified"),
         "F34 exact outcome or independent publication scope incorrect");
    foreach (var file in staged)
        Need(Text(record.GetProperty("source_files"), file) == Sha(file),
             "staged F34 source hash mismatch " + file);

    var observed = record.GetProperty("observation");
    Need(Text(observed, "status") ==
         "composition-published-local-three-binaries-independently-verified" &&
         Text(observed, "packageId") == "dw.tools.math.composition" &&
         Text(observed, "packageVersion") == "20260.20261107.1001" &&
         observed.GetProperty("archiveBytes").GetInt32() == 17101 &&
         Text(observed, "archiveSha256") ==
            "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f" &&
         Text(observed, "packagedDllSha256") ==
            "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252" &&
         Text(observed, "loadedDllSha256") ==
            "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252" &&
         observed.GetProperty("declaredDependencyCount").GetInt32() == 2 &&
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
         "physical published Composition evidence incomplete");
    var direct = observed.GetProperty("exactDependencies").EnumerateArray().ToArray();
    var transitive = observed.GetProperty("transitiveDependencies").EnumerateArray().ToArray();
    Need(direct.Length == 2 && transitive.Length == 1 &&
         Text(direct[0], "id") == "dw.quantities" &&
         Text(direct[0], "version") == "20260.20261102.1001" &&
         direct[0].GetProperty("archiveBytes").GetInt32() == 14747 &&
         Text(direct[0], "archiveSha256") ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         Text(direct[0], "packagedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(direct[0], "loadedDllSha256") ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         Text(direct[1], "id") == "dw.tools.math.ir" &&
         Text(direct[1], "version") == "20260.20261104.1001" &&
         direct[1].GetProperty("archiveBytes").GetInt32() == 21926 &&
         Text(direct[1], "archiveSha256") ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         Text(direct[1], "packagedDllSha256") ==
             "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         Text(direct[1], "loadedDllSha256") ==
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
         "physical direct Quantities/IR and IR transitive Quantities closure unqualified");
    var commands = observed.GetProperty("commands").EnumerateArray().ToArray();
    var outer = record.GetProperty("command_receipts").EnumerateArray().ToArray();
    var names = new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" };
    var outerNames = new[] { "tool-restore", "tool-build", "composition-feed-verify", "sdk" };
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
