using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Root = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-candidate/";
const string ToolProject = Tool + "dw.tools.math.nuget-candidate.csproj";
const string F1Project =
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
const string Verifier = Root + "ValidateNugetQuantitiesCandidate.cs";
const string Evidence = Root + "nuget-federation-f2-quantities-qualified.json";
const string Status = "rs135-quantities-calendar-candidate-qualified-unpublished";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var target = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS135", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS134", 1,
    "docs: qualify Math federated NuGet owner inventory without publication",
    "0.1.63", 2, 2, 0, false);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(Root + "nuget-federation-f1-qualified.json",
    "/status", JsonValue.Create("rs134-math-f1-calendar-feed-inventory-no-publication"));
payload.Require.JsonValueEquals(Root + "nuget-federation-f1-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals("docs/discovery/qualification/RS133-neutral-candidate-qualified.json",
    "/neutral_migration_qualified", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains("src/projects/dw.quantities/dw.quantities.csproj",
    "<Version>0.2.0-preview.1</Version>");
payload.Require.FileExists(F1Project);
payload.Require.FileExists("tools/dw.tools.math.nuget-preflight/Program.cs");

var files = new[]
{
    Root + "nuget-federation-f2-quantities.md",
    Verifier,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/QuantitiesBinaryConsumer.cs"
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
    var operation = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = operation.AfterSha256 ??
        throw new InvalidOperationException("RS135 staged source SHA-256 missing: " + file);
}
payload.Require.TextContains(Root + "nuget-federation-f2-quantities.md",
    "## Binary-only compiled consumer");
payload.Require.TextContains(Tool + "Program.cs",
    "--qualify-quantities");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");
payload.Require.TextContains(Tool + "fixtures/QuantitiesBinaryConsumer.cs",
    "MATH-QUANTITIES-BINARY|PASS|4");

var baseline = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(baseline.Output.Contains("MATH-NUGET-F2:PACKAGE=dw.quantities;BINARY=4;",
     StringComparison.Ordinal), "independent RS135 baseline audit did not pass");
if (!target)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
        ?? throw new InvalidOperationException("NUGET_LOCAL must be an existing approved local feed");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be fully qualified and exist");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-f2-rs135", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "f1-restore", "dotnet", [
        "restore", F1Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "f1-build", "dotnet", [
        "build", F1Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidate = Run(root, "candidate-consumer", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-quantities", "."
    ]);
    commands.Add(candidate.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 6, "RS135 must retain six parent as-run command receipts");

    var observed = JsonNode.Parse(candidate.Output.Trim())?.AsObject()
        ?? throw new InvalidOperationException("candidate command must return one JSON object");
    Need(observed["status"]?.GetValue<string>() ==
            "qualified-local-candidate-not-published" &&
         observed["sourcePackageId"]?.GetValue<string>() == "dw.quantities" &&
         observed["sourceVersion"]?.GetValue<string>() == "0.2.0-preview.1" &&
         observed["sharedCoreDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["compiledSourceFreeConsumerPassed"]?.GetValue<bool>() == true &&
         observed["consumerChecks"]?.GetValue<int>() == 4 &&
         observed["sourceFeedUnchanged"]?.GetValue<bool>() == true &&
         observed["published"]?.GetValue<bool>() == false,
         "Math package candidate did not meet the bounded binary qualification");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-federation-quantities-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities",
        ["previous_owner_run"] = "RS134",
        ["sourceBaselineCommit"] = "e01cf477bb82f01cb3643921094f3b58b3c217b6",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["source_versions_modified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["candidate"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] = "Review notices/rights and candidate source/archive/DLL proof before a separate immutable NUGET_LOCAL publication gate; do not overwrite legacy archives or assume physical package rollback. Qualify other Math libraries in dependency order."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var targetCheck = Run(root, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(targetCheck.Output.Contains("MATH-NUGET-F2:PUBLISHED=false;ROLLBACK=false;",
     StringComparison.Ordinal), "RS135 independently recorded receipt is invalid");
return payload.Complete();

static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidOperationException("RS135 candidate: " + reason);
}
static (JsonObject Receipt, string Output) Run(
    string root, string id, string executable, string[] args)
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
        Path.Combine(root, ".workflow", "local", "nuget-f2-rs135", "packages");
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "cannot start " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS135 timed out during " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidOperationException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3800)]);
    }
    Console.WriteLine("RS135 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 900)]
    }, output);
}
