using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Root = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-release/";
const string ToolProject = Tool + "dw.tools.math.nuget-release.csproj";
const string Evidence = Root + "nuget-federation-f3-quantities-publisher-qualified.json";
const string Verifier = Root + "ValidateNugetQuantitiesReleaseController.cs";
const string Status = "rs136-math-immutable-local-nuget-publisher-rehearsal-qualified-unpublished";

var payload = PayloadContext.Create();
var repository = payload.RepositoryRoot;
var target = File.Exists(Path.Combine(repository,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS136", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS135", 1,
    "test: qualify dw.quantities calendar NuGet binary candidate",
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
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));

var files = new[]
{
    Root + "nuget-federation-f3-quantities-publisher.md",
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
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidOperationException("RS136 staged file hash missing: " + file);
}
payload.Require.TextContains(Root + "nuget-federation-f3-quantities-publisher.md",
    "## Shared implementation, no second publisher");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "publicationModeEnabledByDwf = false");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(repository, "baseline-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(baseline.Output.Contains("MATH-NUGET-F3:RS135=FIRST-PASS;", StringComparison.Ordinal),
    "independent baseline qualification refused");
if (!target)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
        ?? throw new InvalidOperationException("NUGET_LOCAL is required for source-only restore");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(repository, ".workflow", "local", "nuget-rs136", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(repository, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(repository, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var rehearsal = Run(repository, "publisher-rehearsal", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release", "--no-build",
        "--no-restore", "--", "--rehearse"
    ]);
    commands.Add(rehearsal.Receipt);
    var sdk = Run(repository, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "RS136 must record four distinct actual tool commands");

    var observed = JsonNode.Parse(rehearsal.Output.Trim())?.AsObject()
        ?? throw new InvalidOperationException("rehearsal must return one JSON object");
    Need(observed["status"]?.GetValue<string>() ==
            "real-math-package-disposable-publisher-qualified" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261102.1001" &&
         observed["publisherDllSha256"]?.GetValue<string>() ==
            "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["controls"]?.GetValue<int>() == 6 &&
         observed["publishedInDisposableFeed"]?.GetValue<bool>() == true &&
         observed["collisionRefused"]?.GetValue<bool>() == true &&
         observed["alteredHashRefused"]?.GetValue<bool>() == true &&
         observed["cancellationRefused"]?.GetValue<bool>() == true &&
         observed["candidateIsPublishedToSharedFeed"]?.GetValue<bool>() == false &&
         observed["sharedFeedTouched"]?.GetValue<bool>() == false &&
         observed["publicationModeEnabledByDwf"]?.GetValue<bool>() == false,
         "bounded real-package publisher trial did not meet the source release contract");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-release-controller-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities",
        ["sourceBaselineCommit"] = "cc88f8c69a37037ab5c1d2437accf07ff104df26",
        ["ownerRequest"] = "Math local NuGet federation F1-F3",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["public_license_selected"] = false,
        ["source_versions_modified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["rehearsal"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After explicit owner review, invoke the separate guarded local --publish mode only on the candidate's actual creation date with the exact archived RS135 bytes, then independently run --verify and qualify a DWF feed-read receipt. If stale, requalify a fresh version first."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(repository, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(final.Output.Contains("MATH-NUGET-F3:SHARED-PUBLICATION=false;",
    StringComparison.Ordinal), "independent publisher receipt validation refused");
return payload.Complete();

static void Need(bool ok, string message)
{
    if (!ok) throw new InvalidOperationException("RS136: " + message);
}
static (JsonObject Receipt, string Output) Run(
    string repository, string id, string executable, string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = repository,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(repository, ".workflow", "local", "nuget-rs136", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "cannot start " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS136 timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidOperationException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3800)]);
    }
    Console.WriteLine("RS136 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
