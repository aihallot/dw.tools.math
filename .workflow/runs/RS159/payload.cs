using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-statistics-release/";
const string Project = Tool + "dw.tools.math.nuget-statistics-release.csproj";
const string Program = Tool + "Program.cs";
const string Validator = Distribution + "ValidateNugetStatisticsReleaseReceipt.cs";
const string Documentation = Distribution + "nuget-federation-f26-statistics-release-receipt.md";
const string Evidence = Distribution + "nuget-federation-f26-statistics-release-receipt-qualified.json";
const string Status = "rs159-statistics-private-publication-receipt-corrected-unpublished";
const string Ready = "MATH-STATISTICS-F26:FIELD=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F26:FIELD=CORRECTED;BUILD=PASS;NATIVE=541";
const string Limits = "MATH-STATISTICS-F26:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string ArchiveSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
const string DllSha = "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96";
const string HistoricalSourceSha = "d4ba2b3b00b29104cecd95a672fcce9777352ca12045276be583c6043b7bc920";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS159", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS158", 1,
    "test: qualify Statistics immutable local NuGet publisher rehearsal",
    "0.1.66", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/status", JsonValue.Create("rs157-statistics-dependent-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/packageId", JsonValue.Create("dw.tools.math.statistics"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261106.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/candidateArchiveSha256", JsonValue.Create(ArchiveSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/candidateDllSha256", JsonValue.Create(DllSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/candidateArchiveBytes", JsonValue.Create(17451));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f24-statistics-candidate-qualified.json",
    "/candidate/compiledBinaryConsumerChecks", JsonValue.Create(18));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/status", JsonValue.Create("rs158-statistics-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/rehearsal/consumerChecks", JsonValue.Create(18));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f25-statistics-publisher-qualified.json",
    "/source_files/tools~1dw.tools.math.nuget-statistics-release~1Program.cs",
    JsonValue.Create(HistoricalSourceSha));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists(Program);
payload.Require.FileExists(Project);
payload.Require.FileExists(Tool + "Directory.Packages.props");

var sources = new[] { Documentation, Validator, Program };
if (!complete)
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
        throw new InvalidDataException("RS159 installed source SHA missing " + file);
}
payload.Require.TextContains(Documentation, "## Exact RS159 correction and source lineage");
payload.Require.TextContains(Program, "statisticsDllSha256 = CandidateDllSha");
payload.Require.TextContains(Program, "MATH-STATISTICS-BINARY|PASS|18");
payload.Require.TextContains(Program, "RequireQualifiedRunAndCleanCheckout(root)");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var baseline = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(baseline.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS159 source correction failed independent read-only validator");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must name existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an accessible absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs159", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 3, "three real restore/build/SDK receipts required");
    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.statistics-private-publication-receipt-correction",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.statistics",
        ["preceding_run"] = "RS158",
        ["sourceBaselineCommit"] = "48407dd5cf4f6df16b2ea5ba15a079f257421219",
        ["historical_controller_source_sha256"] = HistoricalSourceSha,
        ["corrected_controller_source_sha256"] =
            sourceHashes[Program]?.GetValue<string>() ??
                throw new InvalidDataException("corrected Statistics controller hash missing"),
        ["original_archive_sha256"] = ArchiveSha,
        ["original_dll_sha256"] = DllSha,
        ["native_nodes_retained"] = 541,
        ["release_controller_compiled"] = true,
        ["release_modes_invoked"] = false,
        ["package_published_by_this_run"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["original_statistics_sources_modified"] = false,
        ["external_requests_transmitted"] = false,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Only after durable RS159 success may owner publish and verify the exact original F24 Statistics archive, then independently qualify physical NUGET_LOCAL readback in a separate DWF run."
    };
    payload.Files.WriteComplete(Evidence,
        record.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
     "RS159 corrected Statistics release receipt target verification failed");
return payload.Complete();

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS159: " + message);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs159", "packages");
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "cannot start " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS159 Statistics command timed out " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            diagnostics[..System.Math.Min(diagnostics.Length, 4100)]);
    }
    Console.WriteLine("RS159 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
