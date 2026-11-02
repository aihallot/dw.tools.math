using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Root = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-feed-verify/";
const string ToolProject = Tool + "dw.tools.math.nuget-feed-verify.csproj";
const string Verifier = Root + "ValidateNugetQuantitiesPublishedFeed.cs";
const string Evidence = Root + "nuget-federation-f4-quantities-published-qualified.json";
const string Status = "rs137-published-local-math-nuget-binary-independently-verified";

var payload = PayloadContext.Create();
var repositoryRoot = payload.RepositoryRoot;
var target = File.Exists(Path.Combine(repositoryRoot,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS137", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS136", 1,
    "test: qualify Math immutable local NuGet publisher rehearsal",
    "0.1.63", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Root + "nuget-federation-f2-quantities-qualified.json",
    "/status", JsonValue.Create("rs135-quantities-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Root + "nuget-federation-f2-quantities-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261102.1001"));
payload.Require.JsonValueEquals(
    Root + "nuget-federation-f2-quantities-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d"));
payload.Require.JsonValueEquals(
    Root + "nuget-federation-f3-quantities-publisher-qualified.json",
    "/status", JsonValue.Create("rs136-math-immutable-local-nuget-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Root + "nuget-federation-f3-quantities-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));

var files = new[]
{
    Root + "nuget-federation-f4-quantities-published.md",
    Verifier,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!target)
{
    foreach (var file in files.Append(Evidence))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/physically_published_package_verified",
        JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence, "/package_published_by_this_run",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var operation = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = operation.AfterSha256 ??
        throw new InvalidOperationException("RS137 staged SHA-256 missing: " + file);
}
payload.Require.TextContains(Root + "nuget-federation-f4-quantities-published.md",
    "## Independent read-only DWF verification");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-QUANTITIES-BINARY|PASS|4");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(repositoryRoot, "baseline-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(baseline.Output.Contains(
     target ? "MATH-NUGET-F4:SHARED-FEED=VERIFIED;BINARY=4;NATIVE=541"
            : "MATH-NUGET-F4:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541",
     StringComparison.Ordinal), "independent source baseline verifier refused");
if (!target)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
        ?? throw new InvalidDataException(
            "NUGET_LOCAL must be set for independent read-only feed verification");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute local feed");
    var cache = Path.Combine(repositoryRoot, ".workflow", "local",
        "nuget-rs137", "packages");
    var commandReceipts = new List<JsonNode>();
    commandReceipts.Add(Run(repositoryRoot, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commandReceipts.Add(Run(repositoryRoot, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var inspected = Run(repositoryRoot, "published-feed-verify", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release", "--no-build",
        "--no-restore", "--", "--verify-published", "."
    ]);
    commandReceipts.Add(inspected.Receipt);
    var sdk = Run(repositoryRoot, "sdk", "dotnet", ["--version"]);
    commandReceipts.Add(sdk.Receipt);
    Need(commandReceipts.Count == 4,
        "RS137 requires four observed source/tool and independent verifier commands");

    var observation = JsonNode.Parse(inspected.Output.Trim())?.AsObject() ??
        throw new InvalidDataException(
            "RS137 published feed verifier must output one bounded JSON object");
    Need(observation["status"]?.GetValue<string>() ==
             "published-local-binary-verified-read-only" &&
         observation["packageId"]?.GetValue<string>() == "dw.quantities" &&
         observation["packageVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observation["archiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observation["packagedDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observation["loadedDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observation["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observation["compiledConsumerChecks"]?.GetValue<int>() == 4 &&
         observation["physicalArchiveInspected"]?.GetValue<bool>() == true &&
         observation["independentPackageReference"]?.GetValue<bool>() == true &&
         observation["publishedArchiveUnchanged"]?.GetValue<bool>() == true &&
         observation["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observation["publicationInvoked"]?.GetValue<bool>() == false &&
         observation["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observation["physicalInstalledPackageRollbackQualified"]?.GetValue<bool>() == false,
         "source-independent physical feed inspection or consumer identity differs");

    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.immutable-nuget-local-publication-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities",
        ["previous_owner_run"] = "RS136",
        ["producer_result_commit"] = "595d145e663ecec6c67b4098ba4a98e4b6fd7ec3",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["physically_published_package_verified"] = true,
        ["package_published_by_this_run"] = false,
        ["remote_registry_published"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["new_public_license_selected"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["observation"] = observation,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commandReceipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] = "Advance only after read-only DWF qualification. Qualify the dependent Math package graph with exact calendar PackageReferences, source-free consumers, rights/notices and separately authorized immutable NUGET_LOCAL publications."
    };
    payload.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var checkedTarget = Run(repositoryRoot, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(checkedTarget.Output.Contains(
     "MATH-NUGET-F4:REPUBLICATION=false;PUBLIC-REGISTRY=false;ROLLBACK=false",
     StringComparison.Ordinal), "RS137 independent durable target receipt refused");
return payload.Complete();

static void Need(bool result, string message)
{
    if (!result) throw new InvalidDataException("RS137: " + message);
}

static (JsonObject Receipt, string Output) Run(
    string repositoryRoot, string id, string executable, string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(repositoryRoot, ".workflow", "local", "nuget-rs137", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not start verification " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("published Math feed check timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3800)]);
    }
    Console.WriteLine("RS137 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
