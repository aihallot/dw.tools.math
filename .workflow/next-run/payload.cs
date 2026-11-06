using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-statistics-feed-verify/";
const string Project = Tool + "dw.tools.math.nuget-statistics-feed-verify.csproj";
const string Validator = Distribution + "ValidateNugetStatisticsPublishedFeed.cs";
const string Evidence = Distribution + "nuget-federation-f27-statistics-published-qualified.json";
const string Status = "rs160-statistics-published-local-two-binaries-independently-verified";
const string Ready = "MATH-STATISTICS-F27:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F27:PHYSICAL=VERIFIED;BINARY=18;DEPENDENCY=1;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS160", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS159", 1,
    "fix: correct Statistics immutable private publication receipt field",
    "0.1.66", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs157-statistics-dependent-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs158-statistics-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f26-statistics-release-receipt-qualified.json",
    "/status", JsonValue.Create("rs159-statistics-private-publication-receipt-corrected-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f26-statistics-release-receipt-qualified.json",
    "/release_controller_compiled", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f26-statistics-release-receipt-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f26-statistics-release-receipt-qualified.json",
    "/original_archive_sha256", JsonValue.Create(
        "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists("tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs");
payload.Require.FileExists("tools/dw.tools.math.nuget-statistics-release/Program.cs");

var sourceFiles = new[]
{
    Distribution + "nuget-federation-f27-statistics-published.md",
    Validator,
    Project,
    Tool + "Program.cs"
};
if (!completed)
{
    foreach (var file in sourceFiles.Append(Evidence))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence,
        "/physically_published_package_verified", JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence,
        "/dependency_package_verified", JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence,
        "/package_published_by_this_run", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence,
        "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in sourceFiles)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS160 staged source SHA missing: " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f27-statistics-published.md",
    "## Independent physical readback");
payload.Require.TextContains(Tool + "Program.cs", "MATH-STATISTICS-BINARY|PASS|18");
payload.Require.TextContains(Tool + "Program.cs", "physical-feed-snapshot");
payload.Require.TextContains(Tool + "Program.cs", "publicationInvoked = false");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var baseline = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready,
        StringComparison.Ordinal), "RS160 source/lineage baseline check refused");

if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs160", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var verified = Run(root, "statistics-feed-verify", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--verify-published-statistics", "."
    ]);
    receipts.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "four direct command receipts required");
    var observed = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("independent verifier must return an actual JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "statistics-published-local-two-binaries-independently-verified" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.statistics" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261106.1001" &&
         observed["archiveBytes"]?.GetValue<int>() == 17451 &&
         observed["archiveSha256"]?.GetValue<string>() ==
             "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8" &&
         observed["packagedDllSha256"]?.GetValue<string>() ==
             "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96" &&
         observed["loadedDllSha256"]?.GetValue<string>() ==
             "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96" &&
         observed["declaredDependencyCount"]?.GetValue<int>() == 1 &&
         observed["publishedPackageSnapshotCount"]?.GetValue<int>() == 2 &&
         observed["compiledConsumerChecks"]?.GetValue<int>() == 18 &&
         observed["physicalArchivesInspected"]?.GetValue<bool>() == true &&
         observed["publishedArchivesUnchanged"]?.GetValue<bool>() == true &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["originalMathProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observed["publicationInvoked"]?.GetValue<bool>() == false &&
         observed["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "new Statistics published package-only consumer or actual archive is not qualified");
    var dependency = observed["exactDependencies"]?.AsArray() ??
        throw new InvalidDataException("exact Quantities dependency absent");
    Need(dependency.Count == 1 &&
         dependency[0]?["id"]?.GetValue<string>() == "dw.quantities" &&
         dependency[0]?["version"]?.GetValue<string>() == "20260.20261102.1001" &&
         dependency[0]?["archiveBytes"]?.GetValue<int>() == 14747 &&
         dependency[0]?["archiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         dependency[0]?["loadedDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
         "physical Quantities dependency changed");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-statistics-private-feed-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.statistics",
        ["previous_owner_run"] = "RS159",
        ["sourceBaselineCommit"] = "c84e7647b5a75067535feec1085ec66f0a27688c",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["physically_published_package_verified"] = true,
        ["dependency_package_verified"] = true,
        ["package_published_by_this_run"] = false,
        ["remote_registry_published"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["new_public_license_selected"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["observation"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After independent Statistics readback, inspect the next Math package candidate and preserve the separate owner-controlled publication gate."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}

payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(
         "MATH-STATISTICS-F27:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal), "Statistics independent qualification target check failed");
return payload.Complete();

static void Need(bool okay, string description)
{
    if (!okay) throw new InvalidDataException("RS160: " + description);
}
static (JsonNode Receipt, string Output) Run(
    string root, string id, string executable, string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs160", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not start RS160 child process " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS160 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4100)]);
    }
    Console.WriteLine("RS160 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
