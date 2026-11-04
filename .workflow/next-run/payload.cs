using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-ir-release/";
const string ToolProject = Tool + "dw.tools.math.nuget-ir-release.csproj";
const string Validator = Distribution + "ValidateNugetIrPublisher.cs";
const string Evidence = Distribution + "nuget-federation-f12-ir-publisher-qualified.json";
const string Status = "rs145-ir-immutable-local-publisher-rehearsal-qualified-unpublished";
const string Accepted = "MATH-IR-F12:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(
    Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS145", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS144", 1,
    "test: qualify IR calendar NuGet candidate with exact Quantities dependency",
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
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs144-ir-calendared-dependent-nuget-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/packageId", JsonValue.Create("dw.tools.math.ir"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261104.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/candidateArchiveBytes", JsonValue.Create(21926));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(1));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
    "/candidate/compiledBinaryConsumerChecks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f11-ir-candidate-qualified.json",
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
    "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "<Version>0.3.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs");

var files = new[]
{
    Distribution + "nuget-federation-f12-ir-publisher.md",
    Validator,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!complete)
{
    foreach (var file in files.Append(Evidence))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var install = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = install.AfterSha256 ??
        throw new InvalidDataException("RS145 staged source SHA-256 missing: " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f12-ir-publisher.md",
    "## Shared publisher, three separate operator modes");
payload.Require.TextContains(Tool + "Program.cs", "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs", "MATH-IR-PUBLISHED|id=");
payload.Require.TextContains(
    Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var initial = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(initial.Output.Contains(
     complete ? Accepted : "MATH-IR-F12:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541",
     StringComparison.Ordinal),
     "IR shared-publisher baseline/target validator refused");

if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException(
            "NUGET_LOCAL is required for exact read-only base dependency restore");
    Need(!string.IsNullOrWhiteSpace(feed) &&
         Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed),
         "NUGET_LOCAL must be an existing absolute private feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs145", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var observedRun = Run(root, "publisher-rehearsal", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--rehearse"
    ]);
    commands.Add(observedRun.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4,
         "all four DWF real child command receipts are required");

    var trial = JsonNode.Parse(observedRun.Output.Trim())?.AsObject() ??
        throw new InvalidDataException(
            "shared IR publisher must emit one structured as-run JSON result");
    Need(trial["status"]?.GetValue<string>() ==
             "ir-dependent-package-publisher-rehearsal-qualified" &&
         trial["packageId"]?.GetValue<string>() == "dw.tools.math.ir" &&
         trial["version"]?.GetValue<string>() == "20260.20261104.1001" &&
         trial["rs144CandidateArchiveSha256"]?.GetValue<string>() ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         trial["rs144CandidateDllSha256"]?.GetValue<string>() ==
             "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         trial["exactDependencyId"]?.GetValue<string>() == "dw.quantities" &&
         trial["exactDependencyVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         trial["exactDependencyArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         trial["publisherDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial["dependencyCount"]?.GetValue<int>() == 1 &&
         trial["controls"]?.GetValue<int>() == 7 &&
         trial["consumerChecks"]?.GetValue<int>() == 15 &&
         trial["disposableFeedPublished"]?.GetValue<bool>() == true &&
         trial["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         trial["collisionRefused"]?.GetValue<bool>() == true &&
         trial["alteredDigestRefused"]?.GetValue<bool>() == true &&
         trial["cancelledRefused"]?.GetValue<bool>() == true &&
         trial["sourceFilesUnchanged"]?.GetValue<bool>() == true &&
         trial["sourceProjectReferencePreserved"]?.GetValue<bool>() == true &&
         trial["publishedDependencyUnchanged"]?.GetValue<bool>() == true &&
         trial["sourceReleaseAuthorization"]?.GetValue<bool>() == false,
         "independent IR package publisher trial, refusal or consumer did not qualify");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-ir-local-publisher-rehearsal",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.ir",
        ["previous_owner_run"] = "RS144",
        ["sourceBaselineCommit"] = "762ca5bcd2b86e36a5f116e63b0f55148ac950dc",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_versions_modified"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["rehearsal"] = trial,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After RS145 succeeds, inspect original immutable 21926-byte RS144 IR candidate and rights. Only a separate clean-main operator action may invoke guarded --publish and --verify, followed by a new independent DWF local-feed readback."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}

payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var finalCheck = Run(root, "target-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(finalCheck.Output.Contains(Accepted, StringComparison.Ordinal) &&
     finalCheck.Output.Contains(
         "MATH-IR-F12:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false",
         StringComparison.Ordinal),
     "RS145 exact IR publisher receipt target check failed");
return payload.Complete();

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS145: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs145", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start RS145 child command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("IR publisher command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 4200)]);
    }
    Console.WriteLine("RS145 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
