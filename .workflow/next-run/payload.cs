using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-expression-feed-verify/";
const string Project = Tool + "dw.tools.math.nuget-expression-feed-verify.csproj";
const string Validator = Distribution + "ValidateNugetExpressionPublishedFeed.cs";
const string Evidence = Distribution + "nuget-federation-f7-expression-published-qualified.json";
const string Status = "rs140-expression-published-local-binary-independently-verified";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var target = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS140", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS139", 1,
    "test: qualify Expression immutable local NuGet publisher rehearsal",
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
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f5-expression-candidate-qualified.json",
    "/candidate/exactBasePackageVersion", JsonValue.Create("20260.20261102.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f6-expression-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs139-expression-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f6-expression-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-expression-release/Program.cs");

var stagedFiles = new[]
{
    Distribution + "nuget-federation-f7-expression-published.md",
    Validator,
    Project,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!target)
{
    foreach (var path in stagedFiles.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/physically_published_package_verified",
        JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence, "/dependency_package_verified",
        JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence, "/package_published_by_this_run",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}

var hashes = new JsonObject();
foreach (var path in stagedFiles)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = installed.AfterSha256 ??
        throw new InvalidDataException("RS140 staged source SHA-256 missing: " + path);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f7-expression-published.md",
    "## Independent DWF observation");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-EXPRESSION-BINARY|PASS|6");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var before = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(before.Output.Contains(
     target ? "MATH-EXPRESSION-F7:PHYSICAL=VERIFIED;BINARY=6;NATIVE=541"
            : "MATH-EXPRESSION-F7:SOURCE=READY;PUBLISHED-CHECK=PENDING;NATIVE=541",
     StringComparison.Ordinal),
     "independent RS140 source/lineage preflight refused");

if (!target)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must identify the approved local feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL is not an existing absolute local directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs140", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var verified = Run(root, "expression-feed-verify", "dotnet", [
        "run", "--project", Project, "-c", "Release", "--no-build",
        "--no-restore", "--", "--verify-published-expression", "."
    ]);
    commands.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "RS140 requires four parent as-run commands");

    var observation = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("RS140 verifier must produce one bounded JSON object");
    Need(observation["status"]?.GetValue<string>() ==
             "expression-published-local-binary-verified-read-only" &&
         observation["packageId"]?.GetValue<string>() == "dw.quantities.expression" &&
         observation["packageVersion"]?.GetValue<string>() == "20260.20261102.1001" &&
         observation["archiveSha256"]?.GetValue<string>() ==
             "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae" &&
         observation["packagedDllSha256"]?.GetValue<string>() ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         observation["loadedDllSha256"]?.GetValue<string>() ==
             "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1" &&
         observation["exactDependencyPackageId"]?.GetValue<string>() == "dw.quantities" &&
         observation["exactDependencyArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observation["loadedDependencyDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observation["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observation["packageReaderSnapshotCount"]?.GetValue<int>() == 2 &&
         observation["compiledConsumerChecks"]?.GetValue<int>() == 6 &&
         observation["actualPhysicalArchiveVerified"]?.GetValue<bool>() == true &&
         observation["independentPackageReference"]?.GetValue<bool>() == true &&
         observation["publishedArchiveUnchanged"]?.GetValue<bool>() == true &&
         observation["publicationInvoked"]?.GetValue<bool>() == false &&
         observation["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observation["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "source-independent published Expression package or transitive binary verification failed");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-published-expression-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.quantities.expression",
        ["previous_owner_run"] = "RS139",
        ["producer_result_commit"] = "1dbfa8ce92f8c399b40868b7e6e398d195e38974",
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
        ["observation"] = observation,
        ["source_files"] = hashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After independent private-feed acceptance, qualify a separate dw.quantities.standard candidate with exact calendar PackageReferences to both already published quantities libraries, source-free gram/kilogram and unit tests, and its own immutable release gate."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(after.Output.Contains(
     "MATH-EXPRESSION-F7:PHYSICAL=VERIFIED;BINARY=6;NATIVE=541",
     StringComparison.Ordinal) &&
     after.Output.Contains(
     "MATH-EXPRESSION-F7:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false",
     StringComparison.Ordinal),
     "independent RS140 completed receipt was not accepted");
return payload.Complete();

static void Need(bool condition, string explanation)
{
    if (!condition) throw new InvalidDataException("RS140: " + explanation);
}

static (JsonObject Receipt, string Output) Run(
    string repository, string id, string command, string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = command,
            WorkingDirectory = repository,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(repository, ".workflow", "local", "nuget-rs140", "packages");
    foreach (var arg in arguments)
        process.StartInfo.ArgumentList.Add(arg);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not start exact published Expression verification " + id);
    var standardOutput = process.StandardOutput.ReadToEndAsync();
    var standardError = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS140 command timed out: " + id);
    }
    var output = standardOutput.GetAwaiter().GetResult();
    var errors = standardError.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 3900)]);
    }
    Console.WriteLine("RS140 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = command + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
