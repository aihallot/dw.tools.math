using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-scientific-release/";
const string Project = Tool + "dw.tools.math.nuget-scientific-release.csproj";
const string Program = Tool + "Program.cs";
const string Validator = Distribution + "ValidateNugetScientificReleaseMessages.cs";
const string Documentation =
    Distribution + "nuget-federation-f22-scientific-release-messages.md";
const string Evidence =
    Distribution + "nuget-federation-f22-scientific-release-messages-qualified.json";
const string Status =
    "rs155-scientific-private-release-operator-messages-qualified-unpublished";
const string Ready =
    "MATH-SCIENTIFIC-F22:SOURCE=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified =
    "MATH-SCIENTIFIC-F22:SOURCE=CORRECTED;BUILD=PASS;BINARY-LABEL=15;NATIVE=541";
const string Limits =
    "MATH-SCIENTIFIC-F22:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS155", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS154", 1,
    "test: qualify Scientific immutable NuGet publisher rehearsal",
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
    "/candidate/candidateArchiveBytes", JsonValue.Create(37265));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create(
        "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f20-scientific-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create(
        "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c"));
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
    "/rehearsal/consumerChecks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f21-scientific-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists(Program);
payload.Require.FileExists(Tool + "dw.tools.math.nuget-scientific-release.csproj");
payload.Require.FileExists(Tool + "Directory.Packages.props");

var sources = new[] { Documentation, Validator, Program };
if (!completed)
{
    payload.Require.FileMissing(Documentation);
    payload.Require.FileMissing(Validator);
    payload.Require.FileMissing(Evidence);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published_by_this_run",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/release_modes_invoked",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}

var sourceHashes = new JsonObject();
foreach (var file in sources)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS155 staged file SHA-256 missing: " + file);
}
payload.Require.TextContains(Documentation,
    "## Exact repair and qualification boundary");
payload.Require.TextContains(Program,
    "only the exact original 37,265-byte RS153 Scientific candidate may be published");
payload.Require.TextContains(Program,
    "|binary=15|archive=");
payload.Require.TextContains(Program, "MATH-SCIENTIFIC-BINARY|PASS|15");
payload.Require.TextContains(Program, "RequireCleanOwnerAndRs154(root)");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(completed ? Qualified : Ready, StringComparison.Ordinal),
    "RS155 exact private operator output source did not pass the observed validator");
if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must identify the existing private NuGet feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be a valid existing absolute feed");

    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs155", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "release-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "release-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 3, "exactly three actual restore/build/SDK receipts required");
    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.scientific-private-release-operator-confirmation",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.scientific",
        ["preceding_run"] = "RS154",
        ["sourceBaselineCommit"] = "c6621ea1c5032c8e52da391bd233e0e0df5f1ff1",
        ["original_archive_sha256"] =
            "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f",
        ["original_dll_sha256"] =
            "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c",
        ["historical_controller_source_sha256"] =
            "3dad3a39d3f6290c669d9f7bc7c2e561f215a9bf12fad026326305629907f394",
        ["corrected_controller_source_sha256"] =
            sourceHashes[Program]?.GetValue<string>() ??
                throw new InvalidDataException("RS155 corrected controller SHA missing"),
        ["operator_message_archive_bytes"] = 37265,
        ["operator_message_binary_checks"] = 15,
        ["native_nodes_retained"] = 541,
        ["release_controller_compiled"] = true,
        ["release_modes_invoked"] = false,
        ["package_published_by_this_run"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["original_scientific_sources_modified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Only after durable RS155 success may the owner separately choose guarded private --publish then --verify using original F20 bytes; independent physical-feed readback remains a separate DWF run."
    };
    payload.Files.WriteComplete(Evidence,
        record.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M6", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));

var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
     "RS155 compiled source and operator diagnostic receipt failed read-only final validation");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS155 Scientific release messages: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs155", "packages");
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not launch bounded Scientific command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS155 Scientific command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4100)]);
    }
    Console.WriteLine("RS155 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
