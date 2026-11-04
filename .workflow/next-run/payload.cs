using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-analysis-candidate/";
const string Project = Tool + "dw.tools.math.nuget-analysis-candidate.csproj";
const string Validator = Distribution + "ValidateNugetAnalysisCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f14-analysis-candidate-qualified.json";
const string Status = "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-ANALYSIS-F14:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F14:CANDIDATE=QUALIFIED;BINARY=13;DEPENDENCIES=0;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS147", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS146", 1,
    "test: verify published IR NuGet and exact Quantities dependency binaries",
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
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/status",
    JsonValue.Create("rs146-ir-published-local-two-binaries-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj",
    "<Version>1.1.0-preview.6</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj",
    "<PackageId>dw.tools.math.analysis</PackageId>");

var sourceFiles = new[]
{
    Distribution + "nuget-federation-f14-analysis-candidate.md",
    Validator,
    Project,
    Tool + "Program.cs",
    Tool + "fixtures/AnalysisBinaryConsumer.cs"
};
if (!complete)
{
    foreach (var path in sourceFiles.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/source_versions_modified", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var fingerprints = new JsonObject();
foreach (var path in sourceFiles)
{
    var operation = payload.Files.ReplaceFromStaged("staged/" + path, path);
    fingerprints[path] = operation.AfterSha256 ??
        throw new InvalidDataException("RS147 source SHA missing " + path);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f14-analysis-candidate.md",
    "## Qualified candidate boundaries");
payload.Require.TextContains(Tool + "Program.cs", "source-pack");
payload.Require.TextContains(Tool + "Program.cs", "sourceFeedUnchanged");
payload.Require.TextContains(Tool + "fixtures/AnalysisBinaryConsumer.cs",
    "MATH-ANALYSIS-BINARY|PASS|13");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var initial = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(initial.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS147 source and roadmap baseline validator refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing local source");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs147", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidate = Run(root, "analysis-candidate", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-analysis", "."
    ]);
    commands.Add(candidate.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four outer actual command receipts required");

    var observed = JsonNode.Parse(candidate.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("analysis candidate tool must produce actual JSON");
    Need(observed["status"]?.GetValue<string>() ==
             "qualified-independent-analysis-calendar-candidate-not-published" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.analysis" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261104.1001" &&
         observed["sourceVersion"]?.GetValue<string>() == "1.1.0-preview.6" &&
         observed["candidateArchivePath"]?.GetValue<string>() ==
             "build/artifacts/rs147-analysis-candidate/candidate/dw.tools.math.analysis.20260.20261104.1001.nupkg" &&
         observed["candidateArchiveBytes"]?.GetValue<long>() > 2000 &&
         observed["candidateArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         observed["candidateDllSha256"]?.GetValue<string>()?.Length == 64 &&
         observed["packageDependencies"]?.GetValue<int>() == 0 &&
         observed["compiledBinaryConsumerChecks"]?.GetValue<int>() == 13 &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["originalProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         observed["sourceFeedUnchanged"]?.GetValue<bool>() == true &&
         observed["packagePublished"]?.GetValue<bool>() == false &&
         observed["versionReserved"]?.GetValue<bool>() == false &&
         observed["publicLicenseSelected"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "actual analysis archive or 13-case consumer not qualified");
    var sources = observed["originalSourcesSha256"]?.AsObject() ??
        throw new InvalidDataException("all original analysis source hashes are required");
    Need(sources.Count == 8, "seven source files and original csproj are required");
    var inner = observed["commandReceipts"]?.AsArray() ??
        throw new InvalidDataException("seven real candidate commands must be recorded");
    Need(inner.Count == 7, "expected seven inner candidate/consumer commands");
    var result = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.analysis-local-nuget-candidate-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.analysis",
        ["previous_owner_run"] = "RS146",
        ["sourceBaselineCommit"] = "aea1b730b0897b070239bc0e86255799c8b961f3",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_versions_modified"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["candidate"] = observed,
        ["source_files"] = fingerprints,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After the original Analysis candidate is independently qualified, rehearse a no-overwrite publisher only on a disposable feed; real private publication requires a separate owner decision and exact original archive."
    };
    payload.Files.WriteComplete(Evidence,
        result.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(
         "MATH-ANALYSIS-F14:PUBLISHED=false;LICENSE=false;ROLLBACK=false",
         StringComparison.Ordinal), "RS147 independent analysis candidate receipt was refused");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS147: " + reason);
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
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs147", "packages");
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not start RS147 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS147 qualification timed out in " + id);
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
    Console.WriteLine("RS147 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
