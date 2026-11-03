using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-standard-release/";
const string ToolProject = Tool + "dw.tools.math.nuget-standard-release.csproj";
const string Validator = Distribution + "ValidateNugetStandardPublisher.cs";
const string Evidence = Distribution + "nuget-federation-f9-standard-publisher-qualified.json";
const string Status = "rs142-standard-immutable-local-publisher-rehearsal-qualified-unpublished";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS142", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS141", 1,
    "test: qualify Standard NuGet candidate with two exact dependencies",
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
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/packageId", JsonValue.Create("dw.quantities.standard"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261103.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(2));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/candidate/consumerChecks", JsonValue.Create(10));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f8-standard-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/dependency_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<Version>0.2.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<ProjectReference Include=\"../dw.quantities.expression/dw.quantities.expression.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs");

var files = new[]
{
    Distribution + "nuget-federation-f9-standard-publisher.md",
    Validator,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!complete)
{
    foreach (var path in files.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var hashes = new JsonObject();
foreach (var path in files)
{
    var operation = payload.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = operation.AfterSha256 ??
        throw new InvalidDataException("RS142 staged source hash missing: " + path);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f9-standard-publisher.md",
    "## Shared publisher mechanics, not a new release engine");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-STANDARD-PUBLISHED|id=");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var before = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(before.Output.Contains(
     complete ? "MATH-STANDARD-F9:REHEARSAL=QUALIFIED;BINARY=10;NATIVE=541"
              : "MATH-STANDARD-F9:SOURCE=READY;PUBLISHER=PENDING;NATIVE=541",
     StringComparison.Ordinal),
     "RS142 source-only qualification baseline did not pass");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL is required for read-only dependency restore");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs142", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var trial = Run(root, "publisher-rehearsal", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--rehearse"
    ]);
    commands.Add(trial.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "RS142 must capture four actual parent process results");

    var observed = JsonNode.Parse(trial.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("RS142 rehearsal must return one bounded JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "standard-real-two-dependency-publisher-qualified" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities.standard" &&
         observed["version"]?.GetValue<string>() == "20260.20261103.1001" &&
         observed["rs141ArchiveSha256"]?.GetValue<string>() ==
             "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
         observed["rs141DllSha256"]?.GetValue<string>() ==
             "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         observed["dependencyCount"]?.GetValue<int>() == 2 &&
         observed["publisherDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["controls"]?.GetValue<int>() == 7 &&
         observed["consumerChecks"]?.GetValue<int>() == 10 &&
         observed["disposableFeedPublished"]?.GetValue<bool>() == true &&
         observed["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         observed["collisionRefused"]?.GetValue<bool>() == true &&
         observed["alteredDigestRefused"]?.GetValue<bool>() == true &&
         observed["cancelledRefused"]?.GetValue<bool>() == true &&
         observed["publishedDependencyArchivesUnchanged"]?.GetValue<bool>() == true &&
         observed["sourceReleaseAuthorization"]?.GetValue<bool>() == false,
         "Standard dependent publisher trial failed its own exact binary/refusal contract");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-standard-local-publisher-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.standard",
        ["previous_owner_run"] = "RS141",
        ["sourceBaselineCommit"] = "2ba80728052cd5611d99d0a7c5984bee4b7fdba9",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_versions_modified"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["rehearsal"] = observed,
        ["source_files"] = hashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After the disposable rehearsal passes, review exact original RS141 candidate bytes, rights and notices. Separately invoke guarded --publish and --verify from clean Math main, then prepare a read-only independent published-feed DWF gate."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var last = Run(root, "target-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(last.Output.Contains(
     "MATH-STANDARD-F9:REHEARSAL=QUALIFIED;BINARY=10;NATIVE=541",
     StringComparison.Ordinal) &&
     last.Output.Contains(
     "MATH-STANDARD-F9:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false",
     StringComparison.Ordinal),
     "RS142 source-pinned completed target receipt was refused");
return payload.Complete();

static void Need(bool yes, string reason)
{
    if (!yes) throw new InvalidDataException("RS142: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs142", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "unable to start bounded RS142 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS142 child timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var failure = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + failure[..System.Math.Min(failure.Length, 3800)]);
    }
    Console.WriteLine("RS142 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
