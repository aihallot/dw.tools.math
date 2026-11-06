using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-scientific-feed-verify/";
const string Project = Tool + "dw.tools.math.nuget-scientific-feed-verify.csproj";
const string Validator = Distribution + "ValidateNugetScientificPublishedFeed.cs";
const string Evidence = Distribution + "nuget-federation-f23-scientific-published-qualified.json";
const string Status = "rs156-scientific-published-local-binary-independently-verified";
const string Ready = "MATH-SCIENTIFIC-F23:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F23:PHYSICAL=VERIFIED;BINARY=15;DEPENDENCIES=0;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS156", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS155", 1,
    "fix: correct Scientific private NuGet publication and verification diagnostics",
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
    "/candidate/candidateArchiveBytes", JsonValue.Create(37265));
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
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs154-scientific-immutable-private-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/rehearsal/disposableFeedPublished", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/rehearsal/consumerChecks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json",
    "/status", JsonValue.Create(
        "rs155-scientific-private-release-operator-messages-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json",
    "/release_controller_compiled", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json",
    "/release_modes_invoked", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json",
    "/operator_message_archive_bytes", JsonValue.Create(37265));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json",
    "/operator_message_binary_checks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<Version>1.1.0-preview.14</Version>");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-scientific-release/Program.cs");

var files = new[]
{
    Distribution + "nuget-federation-f23-scientific-published.md",
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
        throw new InvalidDataException("RS156 staged source SHA-256 missing " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f23-scientific-published.md",
    "## Physical archive and isolated binary verification");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-SCIENTIFIC-BINARY|PASS|15");
payload.Require.TextContains(Tool + "Program.cs",
    "physical-feed-snapshot");
payload.Require.TextContains(Tool + "Program.cs",
    "publicationInvoked = false");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var baseline = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready, StringComparison.Ordinal),
    "RS156 source and lineage baseline check refused");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select an existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs156", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
    ]).Receipt);
    var verified = Run(root, "scientific-feed-verify", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--verify-published-scientific", "."
    ]);
    commands.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four outer readback command receipts required");

    var observed = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("actual independent readback JSON required");
    Need(observed["status"]?.GetValue<string>() ==
             "scientific-published-local-binary-independently-verified" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.scientific" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261105.1001" &&
         observed["archiveBytes"]?.GetValue<int>() == 37265 &&
         observed["archiveSha256"]?.GetValue<string>() ==
             "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f" &&
         observed["packagedDllSha256"]?.GetValue<string>() ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         observed["loadedDllSha256"]?.GetValue<string>() ==
             "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c" &&
         observed["declaredDependencyCount"]?.GetValue<int>() == 0 &&
         observed["publishedPackageSnapshotCount"]?.GetValue<int>() == 1 &&
         observed["compiledConsumerChecks"]?.GetValue<int>() == 15 &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["physicalArchiveInspected"]?.GetValue<bool>() == true &&
         observed["publishedArchiveUnchanged"]?.GetValue<bool>() == true &&
         observed["originalMathProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observed["publicationInvoked"]?.GetValue<bool>() == false &&
         observed["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "published Scientific physical bytes or fifteen-case consumer not qualified");
    var inner = observed["commands"]?.AsArray() ??
        throw new InvalidDataException("four real isolated consumer commands required");
    Need(inner.Count == 4, "exact four source-free consumer commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.scientific-private-feed-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.scientific",
        ["previous_owner_run"] = "RS155",
        ["sourceBaselineCommit"] = "158b3629a71777be17660cd42038c87732f44467",
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
            "After exact Scientific private-feed readback, select only the next dependency-ready Math package candidate; keep immutable shared-feed publication a separate owner gate."
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
         "MATH-SCIENTIFIC-F23:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS156 full independent readback receipt was rejected");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS156: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs156", "packages");
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not start RS156 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS156 command timed out in " + id);
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
    Console.WriteLine("RS156 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
