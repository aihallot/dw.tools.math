using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Folder = "tests/consumers/electrochemical-published-readback/";
const string Project = Folder + "electrochemical-published-readback.csproj";
const string Check = "docs/distribution/ValidateElectrochemicalPublishedReadback.cs";
const string Evidence = "docs/distribution/electrochemical-published-readback.json";
const string Candidate = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
const string Status = "rs177-electrochemical-published-readback-verified";
const string ResultCommit = "8fbea2196c0f60e92da6608fd84d9b6356f75093";
var p = PayloadContext.Create();
var root = p.RepositoryRoot;
p.Workflow.RequireCurrentRun("RS177", null, null, null, null, null);
var publication = p.Workflow.RequirePriorSuccess("RS176", 1,
    "release: publish original electrochemical Standard archive to NUGET_LOCAL", "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.26"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Candidate, "/candidate/consumerChecks", JsonValue.Create(12));

string[] files = [Project, Folder + "NuGet.Config", Folder + "Program.cs", Check];
var complete = File.Exists(Path.Combine(root, Evidence));
foreach (var path in files)
{
    if (File.Exists(Path.Combine(root, path)))
    {
        Need(File.ReadAllBytes(Path.Combine(root, path)).SequenceEqual(
            File.ReadAllBytes(Path.Combine(p.RunRoot, "staged", path))), "unexpected consumer source: " + path);
    }
    else
    {
        Need(!complete, "source missing from completed target: " + path);
        p.Require.FileMissing(path);
    }
}
foreach (var path in files) p.Files.ReplaceFromStaged("staged/" + path, path);
if (!complete)
{
    p.Require.FileMissing(Evidence);
    var work = Path.Combine(root, "build", "artifacts", "rs177", Guid.NewGuid().ToString("N"));
    Need(!Directory.Exists(work), "a fresh readback workspace is required");
    var cache = Path.Combine(work, "packages");
    var seed = Path.Combine(work, "lock-packages");
    var obj = Path.Combine(work, "obj");
    var bin = Path.Combine(work, "bin");
    var lockPath = Path.Combine(work, "obj", "published.lock.json");
    // Global MSBuild properties apply before the repository imports its output defaults.
    string[] settings = ["-p:BaseIntermediateOutputPath=" + obj + "/", "-p:BaseOutputPath=" + bin + "/",
        "-p:NuGetLockFilePath=" + lockPath, "-p:DisableImplicitNuGetFallbackFolder=true", "-p:NuGetAudit=false"];
    string[] restore = ["restore", Project, "--configfile", Folder + "NuGet.Config", "--no-http-cache"];
    var commands = new List<JsonNode>();
    var before = Run(root, "feed-before", ["run", "--file", Check, "--", "--check-feed"], 240000);
    Need(before.Output.Contains("RS177-FEED|closure=3|older-standard=preserved", StringComparison.Ordinal), "physical baseline absent");
    commands.Add(before.Receipt);
    commands.Add(Run(root, "create-lock", [.. restore, "--use-lock-file", "--packages", seed, .. settings], 240000).Receipt);
    Need(File.Exists(lockPath) && !Directory.Exists(cache), "missing lock or nonempty verification cache");
    commands.Add(Run(root, "locked-restore", [.. restore, "--locked-mode", "--force", "--packages", cache, .. settings], 240000).Receipt);
    commands.Add(Run(root, "consumer-build", ["build", Project, "-c", "Release", "--no-restore", .. settings], 240000).Receipt);
    var application = Path.Combine(bin, "Release", "net10.0", "electrochemical-published-readback.dll");
    var consumer = Run(root, "consumer-readback", [application, Candidate, Path.Combine(obj, "project.assets.json"), cache], 120000);
    commands.Add(consumer.Receipt);
    var readback = JsonNode.Parse(consumer.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("published consumer did not emit its observation");
    var after = Run(root, "feed-after", ["run", "--file", Check, "--", "--check-feed"], 240000);
    Need(after.Output.Contains("RS177-FEED|closure=3|older-standard=preserved", StringComparison.Ordinal), "physical recheck absent");
    commands.Add(after.Receipt);
    var sdk = Run(root, "sdk", ["--version"], 120000);
    commands.Add(sdk.Receipt);
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1, ["status"] = Status,
        ["gate"] = "MATH-ELECTROCHEM-UNIT-PROFILE", ["consumerNeed"] = "A2-W02-C01-U-N",
        ["publicationRun"] = "RS176", ["publicationCommit"] = ResultCommit, ["publicationObservation"] = "created",
        ["publicationReport"] = publication.Path,
        ["candidateEvidence"] = Candidate, ["sourceEvidence"] = "docs/distribution/electrochemical-unit-profile-source-qualified.json",
        ["sdkVersion"] = sdk.Output.Trim(), ["readback"] = readback, ["commands"] = p.Json.Array(commands.ToArray()),
        ["olderStandardPreserved"] = true, ["auraIntegrationVerified"] = false,
        ["auraHistoricalReuseRightsEstablished"] = false, ["remotePublication"] = false
    };
    p.Files.WriteComplete(Evidence, evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
p.Require.JsonValueEquals(Evidence, "/publicationCommit", JsonValue.Create(ResultCommit));
var checkedReceipt = Run(root, "receipt", ["run", "--file", Check, "--", "--check-receipt"], 240000);
Need(checkedReceipt.Output.Contains("RS177-READBACK|archives=3|loaded-dlls=3|opt-in=6|inherited=58|standard=14",
    StringComparison.Ordinal), "readback receipt incomplete");
return p.Complete();

static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS177: " + message);
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
        throw new TimeoutException("RS177 timeout " + id);
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
