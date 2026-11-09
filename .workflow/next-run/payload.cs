using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Tool = "tools/dw.tools.math.nuget-numerics-release/";
const string Project = Tool + "dw.tools.math.nuget-numerics-release.csproj";
const string Documentation = "docs/distribution/nuget-federation-f37-numerics-release.md";
const string Validator = "docs/distribution/ValidateNugetNumericsRelease.cs";
const string Evidence = "docs/distribution/nuget-federation-f37-numerics-release-qualified.json";
const string Status = "rs170-numerics-immutable-release-controller-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F37:SOURCE=READY;CONTROLLER=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F37:CONTROLLER=QUALIFIED;ORIGINAL=PINNED;NATIVE=541";
const string Limits = "MATH-NUMERICS-F37:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string ArchiveSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
const string DllSha = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS170", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS169", 2,
    "test: rehearse Numerics immutable NuGet publisher with IR and Quantities",
    "0.1.66", 1, 0, 1, false, true, 1);
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
    "/candidate/candidateArchiveSha256", JsonValue.Create(ArchiveSha));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateDllSha256", JsonValue.Create(DllSha));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateArchiveBytes", JsonValue.Create(18313));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json",
    "/status", JsonValue.Create("rs169-numerics-immutable-disposable-rehearsal-qualified-unpublished"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json",
    "/rehearsal/reconstructionIsOriginalArchiveProof", JsonValue.Create(false));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json",
    "/shared_package_published", JsonValue.Create(false));
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
    p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var hashes = new JsonObject();
foreach (var file in files)
{
    var installed = p.Files.ReplaceFromStaged("staged/" + file, file);
    hashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS170 source SHA not recorded: " + file);
}
p.Require.TextContains(Documentation, "## Qualified controller and bounds");
p.Require.TextContains(Tool + "Program.cs", "privatePublishGateEnabled = true");
p.Require.TextContains(Tool + "Program.cs", "sharedFeedPublishedByThisRun = false");
p.Require.TextContains(Tool + "Program.cs", "only exact F35 original Numerics archive");
p.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
var before = Run(root, "source-check", ["run", "--file", Validator, "--", "--check"]);
Need(before.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS170 source preflight refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL dependency feed required");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "NUGET_LOCAL must be a valid absolute private dependency feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs170", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", [
        "restore", Project, "--source", feed, "--packages", cache, "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", [
        "build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
    ]).Receipt);
    var result = Run(root, "controller-qualification", [
        "run", "--project", Project, "-c", "Release", "--no-build", "--no-restore",
        "--", "--qualify"
    ]);
    commands.Add(result.Receipt);
    var sdk = Run(root, "sdk", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four observed command receipts required");
    var evidence = JsonNode.Parse(result.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("controller must emit exact JSON");
    Need(evidence["status"]?.GetValue<string>() ==
             "qualified-numerics-immutable-release-controller-unpublished" &&
         evidence["packageId"]?.GetValue<string>() == "dw.tools.math.numerics" &&
         evidence["originalF35ArchiveSha256"]?.GetValue<string>() == ArchiveSha &&
         evidence["originalF35DllSha256"]?.GetValue<string>() == DllSha &&
         evidence["originalF35ArchiveBytes"]?.GetValue<int>() == 18313 &&
         evidence["malformedCandidateRefused"]?.GetValue<bool>() == true &&
         evidence["sourceProjectPreserved"]?.GetValue<bool>() == true &&
         evidence["privatePublishGateEnabled"]?.GetValue<bool>() == true &&
         evidence["privateVerifyGateEnabled"]?.GetValue<bool>() == true &&
         evidence["sharedFeedPublishedByThisRun"]?.GetValue<bool>() == false &&
         evidence["sharedPublicationAuthorized"]?.GetValue<bool>() == false,
         "RS170 controller did not prove exact original-only unpublished qualification");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.numerics-immutable-private-release-controller",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.numerics",
        ["previous_owner_run"] = "RS169",
        ["source_baseline_commit"] = "4887094c0265d676b41d09f3191efa6f0b64408e",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized"] = false,
        ["public_license_selected"] = false,
        ["installed_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["controller"] = evidence,
        ["source_files"] = hashes,
        ["command_receipts"] = p.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
           "Only with specific owner authorization, publish exact original F35 archive via the guarded controller, then independently verify physical private-feed bytes."
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
p.ProjectPlan.RequireNodeState("M3", "done");
var after = Run(root, "target-check", ["run", "--file", Validator, "--", "--check"]);
Need(after.Output.Contains(Qualified, StringComparison.Ordinal) &&
     after.Output.Contains(Limits, StringComparison.Ordinal),
     "RS170 final exact controller receipt refused");
return p.Complete();

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS170: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs170", "packages");
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start controller command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("controller command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var detail = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            detail[..System.Math.Min(detail.Length, 3000)]);
    }
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = "dotnet " + string.Join(" ", arguments),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
