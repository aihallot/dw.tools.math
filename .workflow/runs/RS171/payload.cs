using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Tool = "tools/dw.tools.math.nuget-numerics-original-consumer/";
const string Project = Tool + "dw.tools.math.nuget-numerics-original-consumer.csproj";
const string Documentation = "docs/distribution/nuget-federation-f38-numerics-original.md";
const string Validator = "docs/distribution/ValidateNugetNumericsOriginal.cs";
const string Ready = "MATH-NUMERICS-F38:SOURCE=READY;PHYSICAL=POST-ATTEMPT;NATIVE=541";
const string Limits = "MATH-NUMERICS-F38:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string ArchiveSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
const string DllSha = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
p.Workflow.RequireCurrentRun("RS171", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS170", 1,
    "test: qualify Numerics immutable private release controller",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M3", "done");
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateArchiveSha256", JsonValue.Create(ArchiveSha));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/candidate/candidateDllSha256", JsonValue.Create(DllSha));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f37-numerics-release-qualified.json",
    "/status", JsonValue.Create("rs170-numerics-immutable-release-controller-qualified-unpublished"));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f37-numerics-release-qualified.json",
    "/controller/originalCandidatePhysicallyVerified", JsonValue.Create(true));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f37-numerics-release-qualified.json",
    "/package_published", JsonValue.Create(false));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
p.Require.FileExists("tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs");

var files = new[] { Documentation, Validator, Project, Tool + "Program.cs" };
// The DWF preparation clone intentionally lacks ignored F35 archive bytes.
// Install the same complete source files in both clone and operator checkout.
// The physical original-archive qualification is a post-attempt focused test.
foreach (var file in files)
    p.Files.ReplaceFromStaged("staged/" + file, file);
p.Require.TextContains(Documentation, "## Fresh original package-only consumer");
p.Require.TextContains(Tool + "Program.cs", "archiveRebuilt = false");
p.Require.TextContains(Tool + "Program.cs", "privateFeedPublished = false");
p.Require.TextContains(Tool + "Program.cs", "MATH-NUMERICS-BINARY|PASS|15");
p.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var source = Run(root, "source-check",
    ["run", "--file", Validator, "--", "--check"]);
Need(source.Output.Contains(Ready, StringComparison.Ordinal) &&
     source.Output.Contains(Limits, StringComparison.Ordinal),
    "RS171 source preflight refused");

var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
    throw new InvalidDataException("NUGET_LOCAL published dependency feed missing");
Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "read-only published IR/Quantities dependency feed is unavailable");
var cache = Path.Combine(root, ".workflow", "local", "nuget-rs171", "packages");
var restore = Run(root, "tool-restore",
    ["restore", Project, "--source", feed,
     "--packages", cache, "-p:NuGetAudit=false"]);
var build = Run(root, "tool-build",
    ["build", Project, "-c", "Release", "--no-restore",
     "-p:NuGetAudit=false"]);
var sdk = Run(root, "sdk", ["--version"]);
Need(restore.Receipt["exit_code"]?.GetValue<int>() == 0 &&
     build.Receipt["exit_code"]?.GetValue<int>() == 0 &&
     sdk.Receipt["exit_code"]?.GetValue<int>() == 0,
    "RS171 controller compilation and SDK must succeed");

p.ProjectPlan.RequireNodeState("M3", "done");
var final = Run(root, "target-check",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Ready, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
    "RS171 source-only target re-entry refused");
return p.Complete();

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS171: " + reason);
}
static (JsonNode Receipt, string Output) Run(string root, string id, string[] args)
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
        Path.Combine(root, ".workflow", "local", "nuget-rs171", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start RS171 command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS171 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var error = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            error[..System.Math.Min(error.Length, 3500)]);
    }
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = "dotnet " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
