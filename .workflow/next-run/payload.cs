using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-scientific-candidate/";
const string Project = Tool + "dw.tools.math.nuget-scientific-candidate.csproj";
const string Validator = Distribution + "ValidateNugetScientificCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f20-scientific-candidate-qualified.json";
const string Status = "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-SCIENTIFIC-F20:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F20:CANDIDATE=QUALIFIED;BINARY=15;DEPENDENCIES=0;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS153", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS152", 1,
    "test: verify published Foundation NuGet archive and isolated binary consumer",
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
    Distribution + "nuget-federation-f19-foundation-published-qualified.json",
    "/status", JsonValue.Create(
        "rs152-foundation-published-local-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f19-foundation-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f19-foundation-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f19-foundation-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(5));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<Version>1.1.0-preview.14</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<PackageId>dw.tools.math.scientific</PackageId>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.scientific/BoundedExactScientific.cs",
    "public const string RuleId=\"dw-math-bounded-exact-scientific-1\";");

var files = new[]
{
    Distribution + "nuget-federation-f20-scientific-candidate.md",
    Validator,
    Project,
    Tool + "Program.cs",
    Tool + "fixtures/ScientificBinaryConsumer.cs"
};
if (!complete)
{
    foreach (var file in files.Append(Evidence))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence,
        "/source_versions_modified", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence,
        "/native_nodes_retained", JsonValue.Create(541));
}
var sources = new JsonObject();
foreach (var file in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sources[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS153 staged source SHA-256 missing: " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f20-scientific-candidate.md",
    "## Independent binary-only Scientific consumer");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-SCIENTIFIC-BINARY|PASS|15");
payload.Require.TextContains(Tool + "Program.cs",
    "sourceFeedUnchanged = true");
payload.Require.TextContains(Tool + "fixtures/ScientificBinaryConsumer.cs",
    "MATH-SCIENTIFIC-BINARY|PASS|15");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var before = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(before.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS153 Scientific source/lineage baseline rejected");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs153", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidateResult = Run(root, "scientific-candidate", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-scientific", "."
    ]);
    receipts.Add(candidateResult.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "four real parent process receipts are required");
    var candidate = JsonNode.Parse(candidateResult.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("the original Scientific candidate must produce JSON");
    Need(candidate["status"]?.GetValue<string>() ==
             "qualified-independent-scientific-calendar-candidate-not-published" &&
         candidate["packageId"]?.GetValue<string>() == "dw.tools.math.scientific" &&
         candidate["packageVersion"]?.GetValue<string>() == "20260.20261105.1001" &&
         candidate["historicalSourceVersion"]?.GetValue<string>() == "1.1.0-preview.14" &&
         candidate["sourceProjectPath"]?.GetValue<string>() ==
             "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj" &&
         candidate["candidateArchivePath"]?.GetValue<string>() ==
             "build/artifacts/rs153-scientific-candidate/candidate/dw.tools.math.scientific.20260.20261105.1001.nupkg" &&
         candidate["candidateArchiveBytes"]?.GetValue<long>() > 2000 &&
         candidate["candidateArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateDllSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["packageDependencies"]?.GetValue<int>() == 0 &&
         candidate["compiledBinaryConsumerChecks"]?.GetValue<int>() == 15 &&
         candidate["independentPackageReference"]?.GetValue<bool>() == true &&
         candidate["originalProjectReferencesUsed"]?.GetValue<bool>() == false &&
         candidate["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         candidate["sourceFeedUnchanged"]?.GetValue<bool>() == true &&
         candidate["packagePublished"]?.GetValue<bool>() == false &&
         candidate["versionReserved"]?.GetValue<bool>() == false &&
         candidate["publicLicenseSelected"]?.GetValue<bool>() == false &&
         candidate["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "the real Scientific archive or five-case binary consumer failed");
    var original = candidate["originalSourcesSha256"]?.AsObject() ??
        throw new InvalidDataException("eighteen original Scientific SHA-256 entries required");
    Need(original.Count == 18, "sixteen source, project and lockfile SHA-256 required");
    var commands = candidate["commandReceipts"]?.AsArray() ??
        throw new InvalidDataException("seven real candidate and consumer receipts required");
    Need(commands.Count == 7, "exactly seven inner commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.scientific-local-nuget-candidate-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.scientific",
        ["previous_owner_run"] = "RS152",
        ["sourceBaselineCommit"] = "ba7a33dd453c51332cebac1a3246197b8d193c2d",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized_by_this_run"] = false,
        ["source_versions_modified"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["candidate"] = candidate,
        ["source_files"] = sources,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After independent Scientific candidate qualification, rehearse immutable publication on a disposable feed; original private publication requires a separate owner authorization and byte-exact release."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M6", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(after.Output.Contains(Qualified, StringComparison.Ordinal) &&
     after.Output.Contains(
         "MATH-SCIENTIFIC-F20:PUBLISHED=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS153 Scientific candidate target receipt rejected");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS153: " + reason);
}
static (JsonNode Receipt, string Output) Run(
    string root, string id, string executable, string[] args)
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
        Path.Combine(root, ".workflow", "local", "nuget-rs153", "packages");
    foreach (var argument in args)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not execute bounded command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS153 Scientific candidate command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostic = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostic[..System.Math.Min(diagnostic.Length, 4000)]);
    }
    Console.WriteLine("RS153 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
