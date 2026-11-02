using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-expression-release/";
const string ToolProject = Tool + "dw.tools.math.nuget-expression-release.csproj";
const string VerifySource = Distribution + "ValidateNugetExpressionPublisher.cs";
const string Evidence = Distribution + "nuget-federation-f6-expression-publisher-qualified.json";
const string Status = "rs139-expression-immutable-local-publisher-rehearsal-qualified-unpublished";

var payload = PayloadContext.Create();
var repo = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(repo,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS139", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS138", 1,
    "test: qualify exact NuGet Expression candidate with published base",
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
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/packageId", JsonValue.Create("dw.quantities.expression"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261102.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/packagedDependencies", JsonValue.Create(1));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
    "<Version>0.2.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs");

var files = new[]
{
    Distribution + "nuget-federation-f6-expression-publisher.md",
    VerifySource,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
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
var hashes = new JsonObject();
foreach (var path in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = installed.AfterSha256 ??
        throw new InvalidDataException("RS139 source SHA-256 missing: " + path);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f6-expression-publisher.md",
    "## One shared publisher, not another package engine");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-EXPRESSION-PUBLISHED|id=");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(repo, "baseline-check", "dotnet", [
    "run", "--file", VerifySource, "--", "--check"
]);
Need(baseline.Output.Contains(
     completed ? "MATH-EXPRESSION-F6:REHEARSAL=QUALIFIED;BINARY=6;NATIVE=541"
        : "MATH-EXPRESSION-F6:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541",
     StringComparison.Ordinal), "source-only RS139 baseline validator refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL is required for read-only package restore");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must identify an existing absolute feed");
    var cache = Path.Combine(repo, ".workflow", "local", "nuget-rs139", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(repo, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(repo, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var rehearsal = Run(repo, "publisher-rehearsal", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--rehearse"
    ]);
    commands.Add(rehearsal.Receipt);
    var sdk = Run(repo, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "RS139 needs four real parent process receipts");

    var observed = JsonNode.Parse(rehearsal.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("RS139 publisher must return an exact JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "expression-real-dependent-package-publisher-qualified" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities.expression" &&
         observed["version"]?.GetValue<string>() == "20260.20261102.1001" &&
         observed["rs138CandidateArchiveSha256"]?.GetValue<string>() ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         observed["exactDependencyVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observed["dependencyArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed["publisherDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["checks"]?.GetValue<int>() == 7 &&
         observed["consumerChecks"]?.GetValue<int>() == 6 &&
         observed["disposableFeedPublished"]?.GetValue<bool>() == true &&
         observed["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         observed["duplicateIdentityRefused"]?.GetValue<bool>() == true &&
         observed["invalidHashRefused"]?.GetValue<bool>() == true &&
         observed["cancelledRefused"]?.GetValue<bool>() == true &&
         observed["sourceFeedBaseArchiveUnchanged"]?.GetValue<bool>() == true &&
         observed["sourceReleaseAuthorization"]?.GetValue<bool>() == false,
         "real Expression dependency publisher rehearsal did not pass exact gates");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.expression-local-nuget-publisher-rehearsal",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.expression",
        ["preceding_owner_run"] = "RS138",
        ["sourceBaselineCommit"] = "154d0c2b770de0d0d81efd9ee318cc6e4bc54662",
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
            "Inspect exact RS138 candidate bytes and source rights before explicit guarded --publish; verify independently, then qualify a DWF feed readback without writing packages. Do not assume rehearsal archive hashes equal the original candidate."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var finalCheck = Run(repo, "target-check", "dotnet", [
    "run", "--file", VerifySource, "--", "--check"
]);
Need(finalCheck.Output.Contains(
     "MATH-EXPRESSION-F6:REHEARSAL=QUALIFIED;BINARY=6;NATIVE=541",
     StringComparison.Ordinal) &&
     finalCheck.Output.Contains(
     "MATH-EXPRESSION-F6:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false",
     StringComparison.Ordinal), "durable RS139 independently verified target rejected");
return payload.Complete();

static void Need(bool yes, string reason)
{
    if (!yes) throw new InvalidDataException("RS139: " + reason);
}
static (JsonObject Receipt, string Output) Run(
    string root, string id, string command, string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = command,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs139", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not start publisher command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS139 command timed out: " + id);
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
    Console.WriteLine("RS139 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = command + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
