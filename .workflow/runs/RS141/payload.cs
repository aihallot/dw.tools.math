using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-standard-candidate/";
const string ToolProject = Tool + "dw.tools.math.nuget-standard-candidate.csproj";
const string F1Project =
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
const string Verifier = Distribution + "ValidateNugetStandardCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f8-standard-candidate-qualified.json";
const string Status = "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished";
const string Qualified = "MATH-STANDARD-F8:CANDIDATE=QUALIFIED;BINARY=10;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS141", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS140", 1,
    "test: verify published Expression NuGet binary and exact base closure",
    "0.1.63", 1, 1, 0, false);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/status", JsonValue.Create(
        "rs140-expression-published-local-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/dependency_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/observation/archiveSha256",
    JsonValue.Create("d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/observation/exactDependencyArchiveSha256",
    JsonValue.Create("e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f7-expression-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(6));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<Version>0.2.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<ProjectReference Include=\"../dw.quantities.expression/dw.quantities.expression.csproj\" />");
payload.Require.FileExists(F1Project);
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-expression-feed-verify/Program.cs");

var files = new[]
{
    Distribution + "nuget-federation-f8-standard-candidate.md",
    Verifier,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/StandardBinaryConsumer.cs"
};
if (!complete)
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
        throw new InvalidDataException("RS141 missing staged SHA-256 for " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f8-standard-candidate.md",
    "## Temporary packaging projection, with no source edit");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "f1-inventory-after");
payload.Require.TextContains(Tool + "fixtures/StandardBinaryConsumer.cs",
    "MATH-STANDARD-BINARY|PASS|10");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var initial = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(initial.Output.Contains(
     complete ? Qualified : "MATH-STANDARD-F8:SOURCE=READY;DEPENDENCIES=2;NATIVE=541",
     StringComparison.Ordinal), "Standard candidate baseline verifier rejected source");

if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException(
            "NUGET_LOCAL is required for read-only exact published dependency restore");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs141", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "f1-restore", "dotnet", [
        "restore", F1Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "f1-build", "dotnet", [
        "build", F1Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidate = Run(root, "standard-candidate", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-standard", "."
    ]);
    receipts.Add(candidate.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 6, "all six parent Standard qualification commands are required");

    var observed = JsonNode.Parse(candidate.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("Standard candidate must return a single JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "qualified-standard-dependent-candidate-not-published" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities.standard" &&
         observed["historicalSourceVersion"]?.GetValue<string>() ==
             "0.2.0-preview.1" &&
         observed["dependencyQuantitiesVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observed["dependencyQuantitiesArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed["dependencyExpressionVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observed["dependencyExpressionArchiveSha256"]?.GetValue<string>() ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         observed["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["packageDependencies"]?.GetValue<int>() == 2 &&
         observed["originalSourceProjectReferences"]?.GetValue<int>() == 2 &&
         observed["consumerChecks"]?.GetValue<int>() == 10 &&
         observed["independentlyCompiledBinaryConsumer"]?.GetValue<bool>() == true &&
         observed["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         observed["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         observed["originalFiveSourceFilesLinked"]?.GetValue<bool>() == true &&
         observed["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         observed["published"]?.GetValue<bool>() == false &&
         observed["versionReserved"]?.GetValue<bool>() == false,
         "Standard dependent calendar candidate or independent consumer refused");

    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.standard-calendared-nuget-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.standard",
        ["previous_owner_run"] = "RS140",
        ["publishedDependenciesResultCommit"] = "c3ddede907b0dc1b5cc4ebee18dc3545769875c1",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_versions_modified"] = false,
        ["public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["candidate"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After RS141, verify exact Standard candidate archive/DLL SHA-256, two published dependency pins and rights/notices. Qualify the shared no-overwrite publisher in a separate disposable rehearsal before explicit private publication and independent DWF readback."
    };
    payload.Files.WriteComplete(Evidence,
        record.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet", [
    "run", "--file", Verifier, "--", "--check"
]);
Need(after.Output.Contains(Qualified, StringComparison.Ordinal) &&
     after.Output.Contains("MATH-STANDARD-F8:PUBLISHED=false;LICENSE=false;ROLLBACK=false",
          StringComparison.Ordinal),
     "RS141 independent target receipt validation failed");
return payload.Complete();

static void Need(bool yes, string explanation)
{
    if (!yes) throw new InvalidDataException("RS141: " + explanation);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs141", "packages");
    foreach (var arg in arguments) process.StartInfo.ArgumentList.Add(arg);
    var stopwatch = Stopwatch.StartNew();
    Need(process.Start(), "cannot start bounded Standard command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Standard candidate tool timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    stopwatch.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + error;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 4400)]);
    }
    Console.WriteLine("RS141 " + id + " exit=0 elapsed_ms=" + stopwatch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = stopwatch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1200)]
    }, output);
}
