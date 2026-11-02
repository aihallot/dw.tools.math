using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Root = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-preflight/";
const string ToolProject = Tool + "dw.tools.math.nuget-preflight.csproj";
const string Receipt = Root + "nuget-federation-f1-qualified.json";
const string Verifier = Root + "ValidateNugetFederationF1.cs";
const string Status = "rs134-math-f1-calendar-feed-inventory-no-publication";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var target = File.Exists(Path.Combine(root, Receipt.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS134", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS133", 1,
    "test: qualify Math direct fallback against neutral Discovery package",
    "0.1.61", 1, 1, 0, false);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/discovery/qualification/RS133-neutral-candidate-qualified.json",
    "/neutral_migration_qualified", JsonValue.Create(false));

var files = new[]
{
    Root + "nuget-federation-f1.md",
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Verifier
};
if (!target)
{
    foreach (var file in files.Append(Receipt))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Receipt, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Receipt, "/native_nodes_retained", JsonValue.Create(541));
    payload.Require.JsonValueEquals(Receipt, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Receipt, "/source_project_versions_changed",
        JsonValue.Create(false));
}

var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var result = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = result.AfterSha256 ??
        throw new InvalidOperationException("Missing exact staged source SHA-256 " + file);
}
payload.Require.TextContains(Root + "nuget-federation-f1.md",
    "## Bounded F1 consumer");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetCalendarPackageVersion.Calculate");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var first = Run(root, "baseline-check", "dotnet", ["run", "--file", Verifier, "--", "--check"]);
Need(first.Output.Contains("MATH-NUGET-F1:SOURCE=13;", StringComparison.Ordinal),
    "baseline independent validator did not pass");
if (!target)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must select an existing absolute local feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-f1-rs134", "packages");
    Directory.CreateDirectory(cache);
    var commandReceipts = new List<JsonNode>();
    commandReceipts.Add(Run(root, "restore", "dotnet", [
        "restore", ToolProject, "--source", feed!, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commandReceipts.Add(Run(root, "build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var selfTest = Run(root, "self-test", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--self-test"
    ]);
    commandReceipts.Add(selfTest.Receipt);
    Need(selfTest.Output.Contains("MATH-NUGET-F1-SELFTEST|PASS|5",
        StringComparison.Ordinal), "the qualified calendar primitive did not pass controls");
    var inspected = Run(root, "inspect", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--inspect", "."
    ]);
    commandReceipts.Add(inspected.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commandReceipts.Add(sdk.Receipt);

    var observed = JsonNode.Parse(inspected.Output.Trim())?.AsObject() ??
        throw new InvalidOperationException("Math NuGet owner intake must emit one JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "f1-source-and-feed-inventory-qualified-not-published" &&
         observed["packageCount"]?.GetValue<int>() == 13 &&
         observed["msbuildPropertyQueries"]?.GetValue<int>() == 13 &&
         observed["repositoryPackageSha256"]?.GetValue<string>() ==
             "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf" &&
         observed["loadedLibrarySha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["feedPublished"]?.GetValue<bool>() == false &&
         observed["versionsReserved"]?.GetValue<bool>() == false,
         "Math source/feed identity or calendar inspection is incomplete");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.federated-nuget-owner-intake",
        ["status"] = Status,
        ["requestId"] = "DWF-NUGET-F1-07",
        ["provider"] = "aihallot/dw.tools.math",
        ["sourceBaselineCommit"] = "6ec2652ecab1da7dde82e6c53fa23bf9911a0d2e",
        ["scope"] = "read-only first party Math package inventory and calendar-version preview",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["source_project_versions_changed"] = false,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["package_count"] = 13,
        ["observed"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commandReceipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] = "Qualify one dw.quantities candidate with pack-only PackageVersion override, source-free exact rational consumer, archive and loaded DLL hashes and explicit local-release/rollback decision. Then qualify the dependent package graph and publish only under a separate authorized no-overwrite gate."
    };
    payload.Files.WriteComplete(Receipt,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var last = Run(root, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(last.Output.Contains("MATH-NUGET-F1:VERSION-PREVIEW=true;PUBLICATION=false;",
    StringComparison.Ordinal), "independent receipt validator refused the final target");
return payload.Complete();

static void Need(bool value, string issue)
{
    if (!value) throw new InvalidOperationException("RS134 Math NuGet F1: " + issue);
}
static (JsonObject Receipt, string Output) Run(
    string directory, string id, string executable, string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = directory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(directory, ".workflow", "local", "nuget-f1-rs134", "packages");
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "cannot start " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(600000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS134 process timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
        throw new InvalidOperationException(id + " exited " + process.ExitCode +
            "\n" + (output + errors)[..System.Math.Min((output + errors).Length, 4000)]);
    Console.WriteLine("RS134 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["stdout_excerpt"] = output[..System.Math.Min(output.Length, 1400)]
    }, output);
}
