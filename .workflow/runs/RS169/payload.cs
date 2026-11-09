using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Tool = "tools/dw.tools.math.nuget-numerics-rehearsal/";
const string Project = Tool + "dw.tools.math.nuget-numerics-rehearsal.csproj";
const string Validator = "docs/distribution/ValidateNugetNumericsRehearsal.cs";
const string Documentation = "docs/distribution/nuget-federation-f36-numerics-rehearsal.md";
const string Evidence = "docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json";
const string Status = "rs169-numerics-immutable-disposable-rehearsal-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F36:SOURCE=READY;REHEARSAL=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F36:REHEARSAL=QUALIFIED;BINARY=15;NATIVE=541";
const string Limits = "MATH-NUMERICS-F36:SHARED-PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string CandidateSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
const string CandidateDll = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS169", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS168", 3,
    "test: qualify Numerics calendar NuGet candidate with published IR and Quantities",
    "0.1.66", 4, 2, 2, false, true, 2);
p.ProjectPlan.RequireNodeState("M3", "done");
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/status", JsonValue.Create("rs168-numerics-dependent-calendar-candidate-qualified-unpublished"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateArchiveSha256", JsonValue.Create(CandidateSha));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateDllSha256", JsonValue.Create(CandidateDll));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/compiledBinaryConsumerChecks", JsonValue.Create(15));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
p.Require.JsonValueEquals("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
p.Require.FileExists("tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs");

var files = new[] { Documentation, Validator, Project, Tool + "Directory.Packages.props",
    Tool + "Program.cs" };
if (!complete)
{
    foreach (var file in files.Append(Evidence)) p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/shared_package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var installed = p.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS169 staged SHA missing " + file);
}
p.Require.TextContains(Documentation,
    "## Rehearsal and independent consumer");
p.Require.TextContains(Tool + "Program.cs", "disposableFeedPublished = true");
p.Require.TextContains(Tool + "Program.cs", "sharedFeedPublished = false");
p.Require.TextContains(Tool + "Program.cs", "independentCompiledBinaryChecks = 15");
p.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
var check = Run(root, "source-check", ["run", "--file", Validator, "--", "--check"]);
Need(check.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS169 source validation failed");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL private feed missing");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "private dependency feed must be absolute and present");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs169", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", [
        "build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
    ]).Receipt);
    var rehearsal = Run(root, "numerics-disposable-rehearsal", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--rehearse", "."
    ]);
    commands.Add(rehearsal.Receipt);
    var sdk = Run(root, "sdk", ["--version"]);
    commands.Add(sdk.Receipt);
    var observation = JsonNode.Parse(rehearsal.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("RS169 tool did not emit structured JSON");
    Need(observation["status"]?.GetValue<string>() ==
             "qualified-numerics-immutable-disposable-rehearsal-unpublished" &&
         observation["packageId"]?.GetValue<string>() == "dw.tools.math.numerics" &&
         observation["originalF35ArchiveSha256"]?.GetValue<string>() == CandidateSha &&
         observation["originalF35DllSha256"]?.GetValue<string>() == CandidateDll &&
         observation["disposableFeedPublished"]?.GetValue<bool>() == true &&
         observation["sharedFeedPublished"]?.GetValue<bool>() == false &&
         observation["releaseAuthorized"]?.GetValue<bool>() == false &&
         observation["immutableCollisionRefused"]?.GetValue<bool>() == true &&
         observation["falsifiedDigestRefused"]?.GetValue<bool>() == true &&
         observation["cancellationRefused"]?.GetValue<bool>() == true &&
         observation["independentCompiledBinaryChecks"]?.GetValue<int>() == 15 &&
         observation["candidateInnerCommands"]?.GetValue<int>() == 11 &&
         observation["commands"]?.AsArray().Count == 7 &&
         observation["directDependencyCount"]?.GetValue<int>() == 1 &&
         commands.Count == 4,
         "RS169 exact disposable rehearsal and actual command receipts refused");
    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.numerics-immutable-disposable-publisher-rehearsal",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.numerics",
        ["previous_owner_run"] = "RS168",
        ["source_baseline_commit"] = "494a3ec2e2dc36dfa52f6299c68dfd814cd76008",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["shared_package_published"] = false,
        ["owner_release_authorized"] = false,
        ["public_license_selected"] = false,
        ["installed_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["rehearsal"] = observation,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = p.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Inspect the exact F35 candidate and qualify a distinct no-overwrite private Numerics release controller; publication requires owner authorization."
    };
    p.Files.WriteComplete(Evidence,
        record.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
p.ProjectPlan.RequireNodeState("M3", "done");
var final = Run(root, "target-check", ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
     "RS169 evidence did not pass qualified read-only validator");
return p.Complete();

static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS169: " + reason);
}
static (JsonNode Receipt, string Output) Run(string root, string id, string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs169", "packages");
    foreach (var arg in arguments) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "failed to start bounded command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS169 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            details[..System.Math.Min(details.Length, 3900)]);
    }
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = "dotnet " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1200)]
    }, output);
}
