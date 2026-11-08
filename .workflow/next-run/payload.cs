using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Dist = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-composition-feed-verify/";
const string Project = Tool + "dw.tools.math.nuget-composition-feed-verify.csproj";
const string Validator = Dist + "ValidateNugetCompositionPublishedFeed.cs";
const string Evidence = Dist + "nuget-federation-f34-composition-published-qualified.json";
const string Status = "rs167-composition-published-local-three-binaries-independently-verified";
const string Ready = "MATH-COMPOSITION-F34:SOURCE=READY;READBACK=PENDING;NATIVE=541";
const string Qualified = "MATH-COMPOSITION-F34:PHYSICAL=VERIFIED;BINARY=15;DIRECT=2;TRANSITIVE=1;NATIVE=541";
const string Limits = "MATH-COMPOSITION-F34:REPUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS167", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS166", 1,
    "fix: qualify Composition private release receipt with exact two-dependency closure",
    "0.1.66", 1, 1, 0, false);
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f31-composition-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs164-composition-dependent-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/candidateArchiveSha256",
    JsonValue.Create("b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(2));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/candidateDllSha256",
    JsonValue.Create("6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f32-composition-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs165-composition-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f32-composition-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f32-composition-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f33-composition-release-receipt-qualified.json",
    "/status", JsonValue.Create("rs166-composition-private-receipt-two-dependencies-corrected-unpublished"));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f33-composition-release-receipt-qualified.json",
    "/direct_dependency_count", JsonValue.Create(2));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f33-composition-release-receipt-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Dist + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists("tools/dw.tools.math.nuget-composition-release/Program.cs");
payload.Require.FileExists("tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs");

var sources = new[]
{
    Dist + "nuget-federation-f34-composition-published.md",
    Validator,
    Project,
    Tool + "Program.cs"
};
if (!completed)
{
    foreach (var path in sources.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/physically_published_package_verified",
        JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence, "/dependency_package_verified",
        JsonValue.Create(true));
    payload.Require.JsonValueEquals(Evidence, "/package_published_by_this_run",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}

var sourceHashes = new JsonObject();
foreach (var file in sources)
{
    var receipt = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = receipt.AfterSha256 ??
        throw new InvalidDataException("RS167 staged source SHA missing: " + file);
}
payload.Require.TextContains(Dist + "nuget-federation-f34-composition-published.md",
    "## Independent physical readback");
payload.Require.TextContains(Tool + "Program.cs", "physical-feed-snapshot");
payload.Require.TextContains(Tool + "Program.cs", "MATH-COMPOSITION-BINARY|PASS|15");
payload.Require.TextContains(Tool + "Program.cs", "publicationInvoked = false");
payload.Require.TextContains(Project, "<Compile Include=\"Program.cs\" />");

var before = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(before.Output.Contains(completed ? Qualified : Ready,
         StringComparison.Ordinal), "F34 independent source validator baseline refused");

if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs167", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet",
        ["restore", Project, "--source", feed, "--packages", cache,
         "-p:NuGetAudit=false"]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet",
        ["build", Project, "-c", "Release", "--no-restore",
         "-p:NuGetAudit=false"]).Receipt);
    var verified = Run(root, "composition-feed-verify", "dotnet",
        ["run", "--project", Project, "-c", "Release", "--no-build",
         "--no-restore", "--", "--verify-published-composition", "."]);
    receipts.Add(verified.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "exactly four real outer process receipts required");
    var observed = JsonNode.Parse(verified.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("Independent readback must emit one real JSON receipt");
    Need(observed["status"]?.GetValue<string>() ==
             "composition-published-local-three-binaries-independently-verified" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.composition" &&
         observed["packageVersion"]?.GetValue<string>() == "20260.20261107.1001" &&
         observed["archiveBytes"]?.GetValue<int>() == 17101 &&
         observed["archiveSha256"]?.GetValue<string>() ==
             "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f" &&
         observed["packagedDllSha256"]?.GetValue<string>() ==
             "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252" &&
         observed["loadedDllSha256"]?.GetValue<string>() ==
             "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252" &&
         observed["declaredDependencyCount"]?.GetValue<int>() == 2 &&
         observed["transitiveDependencyCount"]?.GetValue<int>() == 1 &&
         observed["publishedPackageSnapshotCount"]?.GetValue<int>() == 3 &&
         observed["compiledConsumerChecks"]?.GetValue<int>() == 15 &&
         observed["physicalArchivesInspected"]?.GetValue<bool>() == true &&
         observed["publishedArchivesUnchanged"]?.GetValue<bool>() == true &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["originalMathProjectReferencesUsed"]?.GetValue<bool>() == false &&
         observed["feedWideInventoryPerformed"]?.GetValue<bool>() == false &&
         observed["publicationInvoked"]?.GetValue<bool>() == false &&
         observed["remoteRegistryPublished"]?.GetValue<bool>() == false &&
         observed["installedPackageRollbackQualified"]?.GetValue<bool>() == false,
         "independent Composition physical-feed proof is incomplete");
    var direct = observed["exactDependencies"]?.AsArray() ??
        throw new InvalidDataException("Missing two direct Quantities/IR dependency receipts");
    var transitive = observed["transitiveDependencies"]?.AsArray() ??
        throw new InvalidDataException("Missing transitive Quantities receipt");
    Need(direct.Count == 2 && transitive.Count == 1 &&
          direct[0]?["id"]?.GetValue<string>() == "dw.quantities" &&
          direct[0]?["version"]?.GetValue<string>() == "20260.20261102.1001" &&
          direct[0]?["archiveBytes"]?.GetValue<int>() == 14747 &&
          direct[0]?["archiveSha256"]?.GetValue<string>() ==
              "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
          direct[0]?["packagedDllSha256"]?.GetValue<string>() ==
              "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         direct[1]?["id"]?.GetValue<string>() == "dw.tools.math.ir" &&
         direct[1]?["version"]?.GetValue<string>() == "20260.20261104.1001" &&
         direct[1]?["archiveBytes"]?.GetValue<int>() == 21926 &&
         direct[1]?["archiveSha256"]?.GetValue<string>() ==
             "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480" &&
         direct[1]?["packagedDllSha256"]?.GetValue<string>() ==
             "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af" &&
         transitive[0]?["id"]?.GetValue<string>() == "dw.quantities" &&
         transitive[0]?["version"]?.GetValue<string>() == "20260.20261102.1001" &&
         transitive[0]?["archiveBytes"]?.GetValue<int>() == 14747 &&
         transitive[0]?["archiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
         "original IR/Quantities dependency proof is not exact");
    var inner = observed["commands"]?.AsArray() ??
        throw new InvalidDataException("Missing compiled binary process receipts");
    Need(inner.Count == 4 &&
         inner.All(x => x?["exit_code"]?.GetValue<int>() == 0),
         "all four inner commands must succeed");
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-composition-private-feed-independent-qualification",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.composition",
        ["previous_owner_run"] = "RS166",
        ["sourceBaselineCommit"] = "1b3f8b603fee77276681f0adf24c5a7f4a907cf0",
        ["feed_selector"] = "NUGET_LOCAL",
        ["native_nodes_retained"] = 541,
        ["physically_published_package_verified"] = true,
        ["dependency_package_verified"] = true,
        ["package_published_by_this_run"] = false,
        ["remote_registry_published"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["new_public_license_selected"] = false,
        ["external_requests_transmitted"] = false,
        ["discovery_migration_qualified"] = false,
        ["observation"] = observed,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After independent Composition physical readback, inspect the next Math package candidate. Do not infer remote publication, rollback or external consumer adoption."
    };
    payload.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var after = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(after.Output.Contains(Qualified, StringComparison.Ordinal) &&
     after.Output.Contains(Limits, StringComparison.Ordinal),
     "RS167 independent physical readback target validation refused");
return payload.Complete();

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS167: " + reason);
}
static (JsonNode Receipt, string Output) Run(
    string root, string id, string program, string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = program,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs167", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "could not start bounded RS167 command " + id);
    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorsTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS167 command timed out: " + id);
    }
    var stdout = outputTask.GetAwaiter().GetResult();
    var stderr = errorsTask.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var details = stdout + "\n" + stderr;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 4500)]);
    }
    Console.WriteLine("RS167 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = program + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = stdout[..System.Math.Min(stdout.Length, 1200)]
    }, stdout);
}
