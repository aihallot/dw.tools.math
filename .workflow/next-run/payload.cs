using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-analysis-feed-verify/";
const string Project = Tool + "dw.tools.math.nuget-analysis-feed-verify.csproj";
const string Validator = Distribution + "ValidateNugetAnalysisPublishedFeed.cs";
const string Evidence = Distribution + "nuget-federation-f16-analysis-published-qualified.json";
const string Status = "rs149-analysis-published-local-binary-independently-verified";
const string Ready = "MATH-ANALYSIS-F16:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F16:PHYSICAL=VERIFIED;BINARY=13;DEPENDENCIES=0;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS149", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS148", 1,
    "test: qualify Analysis immutable NuGet publisher rehearsal",
    "0.1.63", 0, 0, 0, true);
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
    "/candidate/candidateArchiveBytes", JsonValue.Create(27831));
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
    Distribution + "nuget-federation-f15-analysis-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs148-analysis-immutable-private-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f15-analysis-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f15-analysis-publisher-qualified.json",
    "/rehearsal/disposableFeedPublished", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f15-analysis-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj",
    "<Version>1.1.0-preview.6</Version>");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-analysis-release/Program.cs");

var files = new[]
{
    Distribution + "nuget-federation-f16-analysis-published.md",
    Validator,
    Project,
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
    payload.Require.JsonValueEquals(Evidence,
        "/physically_published_package_verified", JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence,
        "/package_published_by_this_run", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence,
        "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS149 staged source SHA-256 missing " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f16-analysis-published.md",
    "## Independent read-only verification");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-ANALYSIS-BINARY|PASS|13");
payload.Require.TextContains(Tool + "Program.cs",
    "physical-feed-snapshot");
payload.Require.TextContains(Tool + "Program.cs",
    "publicationInvoked = false");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var baseline = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready, StringComparison.Ordinal),
    "RS149 source and lineage baseline check refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select an existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs149", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
    ]).Receipt);
    var verified = Run(root, "analysis-feed-verify", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--verify-published-analysis", "."
    ]);
    commands.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four outer readback command receipts required");

    var observed = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("actual independent readback JSON required");
    Need(observed["status"]?.GetValue<string>() ==
             "analysis-published-local-binary-independently-verified" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.analysis" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261104.1001" &&
         observed["archiveBytes"]?.GetValue<int>() == 27831 &&
         observed["archiveSha256"]?.GetValue<string>() ==
             "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf" &&
         observed["packagedDllSha256"]?.GetValue<string>() ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         observed["loadedDllSha256"]?.GetValue<string>() ==
             "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7" &&
         observed["declaredDependencyCount"]?.GetValue<int>() == 0 &&
         observed["publishedPackageSnapshotCount"]?.GetValue<int>() == 1 &&
         observed["compiledConsumerChecks"]?.GetValue<int>() == 13 &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["physicalArchiveInspected"]?.GetValue<bool>() == true &&
         observed["publishedArchiveUnchanged"]?.GetValue<bool>() == true &&
         observed["originalMathProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observed["publicationInvoked"]?.GetValue<bool>() == false &&
         observed["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "published Analysis physical bytes or thirteen-case consumer not qualified");
    var inner = observed["commands"]?.AsArray() ??
        throw new InvalidDataException("four real isolated consumer commands required");
    Need(inner.Count == 4, "exact four source-free consumer commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.analysis-private-feed-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.analysis",
        ["previous_owner_run"] = "RS148",
        ["sourceBaselineCommit"] = "7da88c0903253b5695931b3a0234dd1a98578f1e",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["physically_published_package_verified"] = true,
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
            "After exact Analysis private-feed readback, select only the next dependency-ready Math package candidate; keep immutable shared-feed publication a separate owner gate."
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
         "MATH-ANALYSIS-F16:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS149 full independent readback receipt was rejected");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS149: " + reason);
}
static (JsonNode Receipt, string Output) Run(
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
        Path.Combine(root, ".workflow", "local", "nuget-rs149", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not start RS149 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS149 command timed out in " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 4100)]);
    }
    Console.WriteLine("RS149 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
