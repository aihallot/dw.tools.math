using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-analysis-release/";
const string Project = Tool + "dw.tools.math.nuget-analysis-release.csproj";
const string Validator = Distribution + "ValidateNugetAnalysisPublisher.cs";
const string Evidence = Distribution + "nuget-federation-f15-analysis-publisher-qualified.json";
const string Status = "rs148-analysis-immutable-private-publisher-rehearsal-qualified-unpublished";
const string Ready = "MATH-ANALYSIS-F15:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F15:REHEARSAL=QUALIFIED;BINARY=13;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS148", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS147", 1,
    "test: qualify independent Analysis calendar NuGet candidate without publication",
    "0.1.63", 1, 1, 0, false);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M6", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(0));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f14-analysis-candidate-qualified.json",
    "/candidate/compiledBinaryConsumerChecks", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj",
    "<Version>1.1.0-preview.6</Version>");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs");

var staged = new[]
{
    Distribution + "nuget-federation-f15-analysis-publisher.md",
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
        throw new InvalidDataException("RS148 staged source SHA missing " + path);
}
payload.Require.TextContains(Distribution + "nuget-federation-f15-analysis-publisher.md",
    "## Single RS148 rehearsal contract");
payload.Require.TextContains(Tool + "Program.cs", "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs", "MATH-ANALYSIS-BINARY|PASS|13");
payload.Require.TextContains(Tool + "Program.cs", "RequireCleanOwnerAndRs148");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready, StringComparison.Ordinal),
    "RS148 original candidate / source lineage baseline check refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs148", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var executed = Run(root, "analysis-rehearsal", "dotnet", [
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
             "analysis-immutable-disposable-publisher-qualified" &&
         trial["packageId"]?.GetValue<string>() == "dw.tools.math.analysis" &&
         trial["packageVersion"]?.GetValue<string>() == "20260.20261104.1001" &&
         trial["sourceVersion"]?.GetValue<string>() == "1.1.0-preview.6" &&
         trial["originalCandidateArchiveSha256"]?.GetValue<string>() ==
             "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
         trial["originalCandidateDllSha256"]?.GetValue<string>() ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         trial["rehearsalArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         trial["rehearsalDllSha256"]?.GetValue<string>()?.Length == 64 &&
         trial["publisher"]?.GetValue<string>() == "Dw.Tools.NuGet/[20260.20261026.1010]" &&
         trial["publisherDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         trial["packageDependencies"]?.GetValue<int>() == 0 &&
         trial["consumerChecks"]?.GetValue<int>() == 13 &&
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
         "RS148 immutable disposable publication or thirteen-case binary contract refused");
    var inner = trial["commandReceipts"]?.AsArray() ??
        throw new InvalidDataException("seven actual inner commands required");
    Need(inner.Count == 7, "seven inner commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.analysis-private-immutable-publisher-rehearsal",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.analysis",
        ["previous_owner_run"] = "RS147",
        ["sourceBaselineCommit"] = "8d4fe85ce6a545b398dac6e0230ca5f83cd348d6",
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
            "After RS148 succeeds, validate original 27831-byte F14 candidate and owner rights. Separately authorize guarded --publish and --verify from clean main, then independent DWF physical-feed readback."
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
         "MATH-ANALYSIS-F15:SHARED-PUBLICATION=false;ROLLBACK=false;PUBLIC=false",
         StringComparison.Ordinal),
     "RS148 exact immutable rehearsal receipt target check failed");
return payload.Complete();

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS148: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs148", "packages");
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not launch RS148 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS148 command timed out: " + id);
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
    Console.WriteLine("RS148 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = program + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
