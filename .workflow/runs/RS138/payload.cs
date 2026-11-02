using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-expression-candidate/";
const string ToolProject = Tool + "dw.tools.math.nuget-expression-candidate.csproj";
const string F1Project =
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
const string Verifier = Distribution + "ValidateNugetExpressionCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f5-expression-candidate-qualified.json";
const string Status = "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished";
const string TargetMessage = "MATH-EXPRESSION-F5:CANDIDATE=QUALIFIED;BINARY=6;NATIVE=541";

var payload = PayloadContext.Create();
var repository = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(repository,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS138", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS137", 1,
    "test: verify published dw.quantities binary from local NuGet feed",
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
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/status", JsonValue.Create(
        "rs137-published-local-math-nuget-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261102.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/observation/archiveSha256",
    JsonValue.Create("e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d"));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.TextContains(
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
    "<Version>0.2.0-preview.1</Version>");
payload.Require.FileExists(F1Project);
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-feed-verify/Program.cs");

var files = new[]
{
    Distribution + "nuget-federation-f5-expression-candidate.md",
    Verifier,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/ExpressionBinaryConsumer.cs"
};
if (!completed)
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
        throw new InvalidDataException("RS138 staged source hash absent: " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f5-expression-candidate.md",
    "## Packaging projection, without source edits");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "f1-feed-inspection-after");
payload.Require.TextContains(Tool + "fixtures/ExpressionBinaryConsumer.cs",
    "MATH-EXPRESSION-BINARY|PASS|6");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(repository, "baseline-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(baseline.Output.Contains(
     completed ? TargetMessage :
       "MATH-EXPRESSION-F5:SOURCE=READY;DEPENDENCY=PINNED;NATIVE=541",
     StringComparison.Ordinal),
     "independent source/lineage verifier refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException(
            "NUGET_LOCAL must point at the qualified shared local feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL requires an existing absolute directory");
    var cache = Path.Combine(repository, ".workflow", "local", "nuget-rs138", "packages");
    var receiptCommands = new List<JsonNode>();
    receiptCommands.Add(Run(repository, "f1-restore", "dotnet", [
        "restore", F1Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receiptCommands.Add(Run(repository, "f1-build", "dotnet", [
        "build", F1Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    receiptCommands.Add(Run(repository, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receiptCommands.Add(Run(repository, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var inspected = Run(repository, "expression-candidate", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-expression", "."
    ]);
    receiptCommands.Add(inspected.Receipt);
    var sdk = Run(repository, "sdk", "dotnet", ["--version"]);
    receiptCommands.Add(sdk.Receipt);
    Need(receiptCommands.Count == 6,
        "all six Math Expression source/preflight commands are required");

    var observed = JsonNode.Parse(inspected.Output.Trim())?.AsObject() ??
        throw new InvalidDataException(
            "expression candidate must produce a complete bounded JSON result");
    Need(observed["status"]?.GetValue<string>() ==
             "qualified-local-dependent-candidate-not-published" &&
         observed["packageId"]?.GetValue<string>() ==
             "dw.quantities.expression" &&
         observed["historicalSourceVersion"]?.GetValue<string>() ==
             "0.2.0-preview.1" &&
         observed["exactBasePackageVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observed["exactBasePackageSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed["exactBaseDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observed["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["packagedDependencies"]?.GetValue<int>() == 1 &&
         observed["consumerChecks"]?.GetValue<int>() == 6 &&
         observed["actualSourceFreeConsumerPassed"]?.GetValue<bool>() == true &&
         observed["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         observed["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         observed["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         observed["published"]?.GetValue<bool>() == false,
         "the exact NuGet dependency closure or compiled binary consumer refused");
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-federation-expression-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.expression",
        ["previous_owner_run"] = "RS137",
        ["basePublishedResultCommit"] = "deb82a6887838c9feefa2e61e5b06f758c7cf428",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_package_versions_modified"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["candidate"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receiptCommands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Audit private Expression candidate SHA-256, exact NuGet base dependency and source/binary closure. Reuse the shared no-overwrite publisher in a separate disposable rehearsal before an explicitly approved local NUGET_LOCAL release."
    };
    payload.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(repository, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(after.Output.Contains(TargetMessage, StringComparison.Ordinal) &&
     after.Output.Contains("MATH-EXPRESSION-F5:PUBLISHED=false;",
     StringComparison.Ordinal),
     "Expression NuGet candidate target receipt did not validate");
return payload.Complete();

static void Need(bool valid, string issue)
{
    if (!valid) throw new InvalidDataException("RS138: " + issue);
}

static (JsonObject Receipt, string Output) Run(
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
        Path.Combine(root, ".workflow", "local", "nuget-rs138", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start RS138 command: " + id);
    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS138 command timed out: " + id);
    }
    var output = outputTask.GetAwaiter().GetResult();
    var error = errorTask.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + error;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4600)]);
    }
    Console.WriteLine("RS138 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1250)]
    }, output);
}
