using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Tool = "tools/dw.tools.math.nuget-electrochemical-release/";
const string Project = Tool + "dw.tools.math.nuget-electrochemical-release.csproj";
const string Validator = "docs/distribution/ValidateElectrochemicalPublisher.cs";
const string Document = "docs/distribution/electrochemical-publisher.md";
const string Evidence = "docs/distribution/electrochemical-publisher-qualified.json";
const string Candidate = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
const string Status = "rs175-electrochemical-original-archive-publisher-qualified-unpublished";
const string Ready = "MATH-ELECTROCHEM-PUBLISHER:SOURCE=READY;QUALIFICATION=PENDING;NATIVE=541";
const string Qualified = "MATH-ELECTROCHEM-PUBLISHER:CONTROLS=7;ORIGINAL=EXACT;NATIVE=541";
const string Limits = "MATH-ELECTROCHEM-PUBLISHER:SHARED-PUBLISHED=false;REBUILT=false;PUBLIC=false";
var p = PayloadContext.Create();
var root = p.RepositoryRoot;
p.Workflow.RequireCurrentRun("RS175", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS174", 1,
    "test: qualify electrochemical Standard NuGet candidate and twelve binary checks",
    "0.1.66", 3, 3, 0, false);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.26"));
p.Require.JsonValueEquals(Candidate, "/status",
    JsonValue.Create("rs174-electrochemical-standard-calendar-candidate-qualified-unpublished"));
p.Require.JsonValueEquals(Candidate, "/candidate/packageVersion", JsonValue.Create("20260.20261110.1001"));
p.Require.JsonValueEquals(Candidate, "/candidate/candidateArchiveSha256",
    JsonValue.Create("766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d"));
p.Require.JsonValueEquals(Candidate, "/candidate/consumerChecks", JsonValue.Create(12));
p.Require.JsonValueEquals(Candidate, "/package_published", JsonValue.Create(false));
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
var files = new[] { Document, Validator, Project, Tool + "Directory.Packages.props", Tool + "Program.cs" };
if (!complete)
{
    foreach (var path in files.Append(Evidence)) p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
    p.Require.JsonValueEquals(Evidence, "/publisher/controlsPassed", JsonValue.Create(7));
    p.Require.JsonValueEquals(Evidence, "/publisher/packageRebuilt", JsonValue.Create(false));
}
var hashes = new JsonObject();
foreach (var path in files)
{
    var installed = p.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = installed.AfterSha256 ?? throw new InvalidDataException("RS175 source hash missing " + path);
}
var initial = Run(root, "publisher-source-validator", ["run", "--file", Validator, "--", "--check"], 240000);
Need(initial.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal) &&
     initial.Output.Contains(Limits, StringComparison.Ordinal), "publisher source or target receipt refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL required");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed), "existing absolute private feed required");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs175", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "publisher-tool-restore",
        ["restore", Project, "--source", feed, "--packages", cache, "-p:NuGetAudit=false"], 240000).Receipt);
    commands.Add(Run(root, "publisher-tool-build",
        ["build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"], 240000).Receipt);
    var rehearsal = Run(root, "original-archive-publisher-rehearsal",
        ["run", "--project", Project, "-c", "Release", "--no-build", "--no-restore",
         "--", "--rehearse-publisher", "."], 240000);
    commands.Add(rehearsal.Receipt);
    var sdk = Run(root, "sdk", ["--version"], 120000);
    commands.Add(sdk.Receipt);
    var observed = JsonNode.Parse(rehearsal.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("publisher emitted no structured JSON");
    Need(observed["status"]?.GetValue<string>() == "qualified-original-electrochemical-publisher-unpublished" &&
         observed["controlsPassed"]?.GetValue<int>() == 7 &&
         observed["packageRebuilt"]?.GetValue<bool>() == false &&
         observed["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         observed["independentArchiveReadback"]?.GetValue<bool>() == true &&
         commands.Count == 4, "original publisher rehearsal incomplete");
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.electrochemical-original-archive-publisher",
        ["status"] = Status,
        ["gate"] = "MATH-ELECTROCHEM-UNIT-PROFILE",
        ["consumer_need"] = "A2-W02-C01-U-N",
        ["previous_result_run"] = "RS174",
        ["previous_result_commit"] = "bca063866734c105f5448bba049e620b9d490c42",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["public_release_authorized"] = false,
        ["aura_code_reuse_rights_established"] = false,
        ["publisher"] = observed,
        ["source_files"] = hashes,
        ["command_receipts"] = p.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] = "Publish the original qualified archive to NUGET_LOCAL without overwrite under the existing owner authorization, then independently read the physically published delivery in a separate gate."
    };
    p.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
var final = Run(root, "publisher-receipt-validator", ["run", "--file", Validator, "--", "--check"], 240000);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal), "exact publisher receipt rejected");
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
return p.Complete();

static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS175: " + message);
}
static (string Output, JsonNode Receipt) Run(string root, string id, string[] args, int timeoutMs)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet", WorkingDirectory = root, UseShellExecute = false,
            RedirectStandardOutput = true, RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    Need(process.Start(), "cannot start " + id);
    var watch = Stopwatch.StartNew();
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeoutMs))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS175 timeout " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostics = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            diagnostics[..System.Math.Min(diagnostics.Length, 4500)]);
    }
    return (output, new JsonObject
    {
        ["id"] = id, ["exit_code"] = process.ExitCode,
        ["command"] = "dotnet " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["stdout_excerpt"] = output[..System.Math.Min(output.Length, 1500)],
        ["stderr_excerpt"] = errors[..System.Math.Min(errors.Length, 500)]
    });
}
