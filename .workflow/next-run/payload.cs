using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-scientific-release/";
const string Project = Tool + "dw.tools.math.nuget-scientific-release.csproj";
const string Validator = Distribution + "ValidateNugetScientificPublisher.cs";
const string Evidence = Distribution + "nuget-federation-f21-scientific-publisher-qualified.json";
const string Status = "rs154-scientific-immutable-private-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-SCIENTIFIC-F21:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F21:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS154", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS153", 1,
    "test: qualify Scientific calendar NuGet candidate without publication",
    "0.1.63", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M6", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(0));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/compiledBinaryConsumerChecks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f19-foundation-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<Version>1.1.0-preview.14</Version>");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs");

var staged = new[]
{
    Distribution + "nuget-federation-f21-scientific-publisher.md",
    Validator,
    Project,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
};
if (!completed)
{
    foreach (var path in staged.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var path in staged)
{
    var operation = payload.Files.ReplaceFromStaged("staged/" + path, path);
    sourceHashes[path] = operation.AfterSha256 ??
        throw new InvalidDataException("RS154 staged source SHA missing " + path);
}
payload.Require.TextContains(Distribution + "nuget-federation-f21-scientific-publisher.md",
    "## Single RS154 disposable-feed publisher contract");
payload.Require.TextContains(Tool + "Program.cs", "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs", "MATH-SCIENTIFIC-BINARY|PASS|15");
payload.Require.TextContains(Tool + "Program.cs", "RequireCleanOwnerAndRs154");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready, StringComparison.Ordinal),
    "RS154 original candidate / source lineage baseline check refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs154", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var executed = Run(root, "scientific-rehearsal", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--rehearse"
    ]);
    commands.Add(executed.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four parent source and rehearsal command receipts required");
    var trial = JsonNode.Parse(executed.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("rehearsal must produce real structured result");
    Need(trial["status"]?.GetValue<string>() ==
             "scientific-immutable-disposable-publisher-qualified" &&
         trial["packageId"]?.GetValue<string>() == "dw.tools.math.scientific" &&
         trial["packageVersion"]?.GetValue<string>() == "20260.20261105.1001" &&
         trial["historicalSourceVersion"]?.GetValue<string>() == "1.1.0-preview.14" &&
         trial["originalCandidateArchiveSha256"]?.GetValue<string>() ==
             "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
         trial["originalCandidateDllSha256"]?.GetValue<string>() ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         trial["rehearsalArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         trial["rehearsalDllSha256"]?.GetValue<string>()?.Length == 64 &&
         trial["publisher"]?.GetValue<string>() == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         trial["publisherDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial["packageDependencies"]?.GetValue<int>() == 0 &&
         trial["consumerChecks"]?.GetValue<int>() == 15 &&
         trial["disposableFeedPublished"]?.GetValue<bool>() == true &&
         trial["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         trial["collisionRefused"]?.GetValue<bool>() == true &&
         trial["alteredDigestRefused"]?.GetValue<bool>() == true &&
         trial["cancelledRefused"]?.GetValue<bool>() == true &&
         trial["originalSourcesUnchanged"]?.GetValue<bool>() == true &&
         trial["originalSourceProjectVersionUnchanged"]?.GetValue<bool>() == true &&
         trial["sharedFeedUnchanged"]?.GetValue<bool>() == true &&
         trial["licenseSelected"]?.GetValue<bool>() == false &&
         trial["rollbackQualified"]?.GetValue<bool>() == false,
         "RS154 immutable disposable publication or fifteen-case binary contract refused");
    var inner = trial["commandReceipts"]?.AsArray() ??
        throw new InvalidDataException("seven actual inner commands required");
    Need(inner.Count == 7, "seven inner commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.scientific-private-immutable-publisher-rehearsal",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.scientific",
        ["previous_owner_run"] = "RS153",
        ["sourceBaselineCommit"] = "16779f7aa5dfcc1cebd1c3be43aaaa2fa6ab566e",
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
            "After RS154 succeeds, validate original 37265-byte F20 candidate and owner rights. Separately authorize guarded --publish and --verify from clean main, then independent DWF physical-feed readback."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(
         "MATH-SCIENTIFIC-F21:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false",
         StringComparison.Ordinal),
     "RS154 exact immutable rehearsal receipt target check failed");
return payload.Complete();

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS154: " + reason);
}
static (JsonNode Receipt, string Output) Run(
    string root, string id, string program, string[] args)
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
        Path.Combine(root, ".workflow", "local", "nuget-rs154", "packages");
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not launch RS154 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS154 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4200)]);
    }
    Console.WriteLine("RS154 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = program + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
