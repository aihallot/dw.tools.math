using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-standard-feed-verify/";
const string ToolProject = Tool + "dw.tools.math.nuget-standard-feed-verify.csproj";
const string Validator = Distribution + "ValidateNugetStandardPublishedFeed.cs";
const string ReceiptPath = Distribution + "nuget-federation-f10-standard-published-qualified.json";
const string Status = "rs143-standard-published-local-three-binaries-independently-verified";
const string Ready = "MATH-STANDARD-F10:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541";
const string Success = "MATH-STANDARD-F10:PHYSICAL=VERIFIED;BINARY=10;DEPENDENCIES=2;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(
    root, ReceiptPath.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS143", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS142", 1,
    "test: qualify Standard immutable local NuGet publisher rehearsal",
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
    Distribution + "nuget-federation-f9-standard-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs142-standard-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f9-standard-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f9-standard-publisher-qualified.json",
    "/rehearsal/disposableFeedPublished", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-standard-release/Program.cs");

var files = new[]
{
    Distribution + "nuget-federation-f10-standard-published.md",
    Validator,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!complete)
{
    foreach (var file in files.Append(ReceiptPath))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(ReceiptPath, "/status",
        JsonValue.Create(Status));
    payload.Require.JsonValueEquals(ReceiptPath,
        "/physically_published_package_verified", JsonValue.Create(true));
    payload.Require.JsonValueEquals(ReceiptPath,
        "/transitive_packages_verified", JsonValue.Create(true));
    payload.Require.JsonValueEquals(ReceiptPath,
        "/package_published_by_this_run", JsonValue.Create(false));
    payload.Require.JsonValueEquals(ReceiptPath,
        "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var operation = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = operation.AfterSha256 ??
        throw new InvalidDataException("RS143 staged source SHA-256 missing " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f10-standard-published.md",
    "## Independent read-only verification");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-STANDARD-BINARY|PASS|10");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var before = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(before.Output.Contains(complete ? Success : Ready, StringComparison.Ordinal),
     "RS143 source/lineage baseline validator refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException(
            "NUGET_LOCAL must select the observed published private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute local directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs143", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var verified = Run(root, "standard-feed-verify", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release", "--no-build",
        "--no-restore", "--", "--verify-published-standard", "."
    ]);
    commands.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four parent process results must be captured");

    var observed = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException(
            "independent Standard published verifier must produce one JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "standard-published-local-three-binaries-independently-verified" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities.standard" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261103.1001" &&
         observed["archiveSha256"]?.GetValue<string>() ==
             "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75" &&
         observed["archiveBytes"]?.GetValue<int>() == 13604 &&
         observed["packagedDllSha256"]?.GetValue<string>() ==
             "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         observed["loadedDllSha256"]?.GetValue<string>() ==
             "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d" &&
         observed["declaredDependencyCount"]?.GetValue<int>() == 2 &&
         observed["publishedPackageSnapshotCount"]?.GetValue<int>() == 3 &&
         observed["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["compiledConsumerChecks"]?.GetValue<int>() == 10 &&
         observed["physicalArchivesInspected"]?.GetValue<bool>() == true &&
         observed["publishedArchivesUnchanged"]?.GetValue<bool>() == true &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["originalMathProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observed["publicationInvoked"]?.GetValue<bool>() == false &&
         observed["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "independent published Standard and transitive binary qualification refused");
    var dependencies = observed["exactDependencies"]?.AsArray() ??
        throw new InvalidDataException("missing exact two-package dependency receipt");
    Need(dependencies.Count == 2 &&
         dependencies[0]?["id"]?.GetValue<string>() == "dw.quantities" &&
         dependencies[0]?["version"]?.GetValue<string>() == "20260.20261102.1001" &&
         dependencies[0]?["archiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         dependencies[0]?["loadedDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         dependencies[1]?["id"]?.GetValue<string>() == "dw.quantities.expression" &&
         dependencies[1]?["version"]?.GetValue<string>() == "20260.20261102.1001" &&
         dependencies[1]?["archiveSha256"]?.GetValue<string>() ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         dependencies[1]?["loadedDllSha256"]?.GetValue<string>() ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1",
         "the two already published dependency archives or loaded DLLs differ");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-standard-private-feed-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.standard",
        ["previous_owner_run"] = "RS142",
        ["publisher_result_commit"] = "1faa208abceac82a4cb0f3ec62f98ad62ca7641a",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["physically_published_package_verified"] = true,
        ["transitive_packages_verified"] = true,
        ["package_published_by_this_run"] = false,
        ["remote_registry_published"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["new_public_license_selected"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["observation"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After independent acceptance of the third immutable Math package, inspect the remaining ten Math package source dependency closures; qualify only the next safely ordered source-independent package candidate and keep its private publication a separate owner gate."
    };
    payload.Files.WriteComplete(ReceiptPath,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(after.Output.Contains(Success, StringComparison.Ordinal) &&
     after.Output.Contains(
         "MATH-STANDARD-F10:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS143 full source-pinned readback receipt was not accepted");
return payload.Complete();

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS143: " + reason);
}

static (JsonObject Receipt, string Output) Run(
    string root, string id, string program, string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = program,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs143", "packages");
    foreach (var arg in arguments) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not run RS143 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS143 published-feed verification command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + error;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4100)]);
    }
    Console.WriteLine("RS143 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = program + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
