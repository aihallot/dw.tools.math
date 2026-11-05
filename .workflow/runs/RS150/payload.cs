using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-foundation-candidate/";
const string Project = Tool + "dw.tools.math.nuget-foundation-candidate.csproj";
const string Validator = Distribution + "ValidateNugetFoundationCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f17-foundation-candidate-qualified.json";
const string Status = "rs150-independent-foundation-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-FOUNDATION-F17:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-FOUNDATION-F17:CANDIDATE=QUALIFIED;BINARY=5;DEPENDENCIES=0;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS150", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS149", 1,
    "test: verify published Analysis NuGet archive and isolated binary consumer",
    "0.1.63", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M0", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f16-analysis-published-qualified.json",
    "/status", JsonValue.Create(
        "rs149-analysis-published-local-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f16-analysis-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f16-analysis-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f16-analysis-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj",
    "<Version>0.1.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj",
    "<PackageId>dw.tools.math.foundation</PackageId>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.foundation/FoundationContract.cs",
    "public static string Identity => PackageId + \"/\" + ContractVersion;");

var files = new[]
{
    Distribution + "nuget-federation-f17-foundation-candidate.md",
    Validator,
    Project,
    Tool + "Program.cs",
    Tool + "fixtures/FoundationBinaryConsumer.cs"
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
        throw new InvalidDataException("RS150 staged source SHA-256 missing: " + file);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f17-foundation-candidate.md",
    "## Independent binary-only Foundation consumer");
payload.Require.TextContains(Tool + "Program.cs",
    "MATH-FOUNDATION-BINARY|PASS|5");
payload.Require.TextContains(Tool + "Program.cs",
    "sourceFeedUnchanged = true");
payload.Require.TextContains(Tool + "fixtures/FoundationBinaryConsumer.cs",
    "MATH-FOUNDATION-BINARY|PASS|5");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var before = Run(root, "baseline-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(before.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS150 Foundation source/lineage baseline rejected");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs150", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidateResult = Run(root, "foundation-candidate", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-foundation", "."
    ]);
    receipts.Add(candidateResult.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "four real parent process receipts are required");
    var candidate = JsonNode.Parse(candidateResult.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("the original Foundation candidate must produce JSON");
    Need(candidate["status"]?.GetValue<string>() ==
             "qualified-independent-foundation-calendar-candidate-not-published" &&
         candidate["packageId"]?.GetValue<string>() == "dw.tools.math.foundation" &&
         candidate["packageVersion"]?.GetValue<string>() == "20260.20261105.1001" &&
         candidate["historicalSourceVersion"]?.GetValue<string>() == "0.1.0-preview.1" &&
         candidate["sourceProjectPath"]?.GetValue<string>() ==
             "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj" &&
         candidate["candidateArchivePath"]?.GetValue<string>() ==
             "build/artifacts/rs150-foundation-candidate/candidate/dw.tools.math.foundation.20260.20261105.1001.nupkg" &&
         candidate["candidateArchiveBytes"]?.GetValue<long>() > 2000 &&
         candidate["candidateArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateDllSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["packageDependencies"]?.GetValue<int>() == 0 &&
         candidate["compiledBinaryConsumerChecks"]?.GetValue<int>() == 5 &&
         candidate["independentPackageReference"]?.GetValue<bool>() == true &&
         candidate["originalProjectReferencesUsed"]?.GetValue<bool>() == false &&
         candidate["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         candidate["sourceFeedUnchanged"]?.GetValue<bool>() == true &&
         candidate["packagePublished"]?.GetValue<bool>() == false &&
         candidate["versionReserved"]?.GetValue<bool>() == false &&
         candidate["publicLicenseSelected"]?.GetValue<bool>() == false &&
         candidate["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "the real Foundation archive or five-case binary consumer failed");
    var original = candidate["originalSourcesSha256"]?.AsObject() ??
        throw new InvalidDataException("two original Foundation SHA-256 entries required");
    Need(original.Count == 2, "exact original Foundation source and project SHA-256 required");
    var commands = candidate["commandReceipts"]?.AsArray() ??
        throw new InvalidDataException("seven real candidate and consumer receipts required");
    Need(commands.Count == 7, "exactly seven inner commands required");
    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.foundation-local-nuget-candidate-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.foundation",
        ["previous_owner_run"] = "RS149",
        ["sourceBaselineCommit"] = "f37f9269bcaf17080e2ee9efea009454c850190f",
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
            "After independent Foundation candidate qualification, rehearse immutable publication on a disposable feed; original private publication requires a separate owner authorization and byte-exact release."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M0", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(after.Output.Contains(Qualified, StringComparison.Ordinal) &&
     after.Output.Contains(
         "MATH-FOUNDATION-F17:PUBLISHED=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS150 Foundation candidate target receipt rejected");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS150: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs150", "packages");
    foreach (var argument in args)
        process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "could not execute bounded command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS150 Foundation candidate command timed out: " + id);
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
    Console.WriteLine("RS150 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1000)]
    }, output);
}
