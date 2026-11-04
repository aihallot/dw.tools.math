using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-ir-candidate/";
const string ToolProject = Tool + "dw.tools.math.nuget-ir-candidate.csproj";
const string F1Project =
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
const string Validator = Distribution + "ValidateNugetIrCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f11-ir-candidate-qualified.json";
const string Status = "rs144-ir-calendared-dependent-nuget-candidate-qualified-unpublished";
const string Qualified = "MATH-IR-F11:CANDIDATE=QUALIFIED;BINARY=15;NATIVE=541";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var completed = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS144", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS143", 1,
    "test: verify published Standard NuGet and exact dependency binaries",
    "0.1.63", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f10-standard-published-qualified.json",
    "/status", JsonValue.Create(
        "rs143-standard-published-local-three-binaries-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f10-standard-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f10-standard-published-qualified.json",
    "/transitive_packages_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f10-standard-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(10));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f10-standard-published-qualified.json",
    "/observation/archiveSha256",
    JsonValue.Create("c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "<Version>0.3.0-preview.1</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.FileExists(F1Project);
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-standard-feed-verify/Program.cs");

var staged = new[]
{
    Distribution + "nuget-federation-f11-ir-candidate.md",
    Validator,
    ToolProject,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/IrBinaryConsumer.cs"
};
if (!completed)
{
    foreach (var path in staged.Append(Evidence))
        payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var fingerprints = new JsonObject();
foreach (var path in staged)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + path, path);
    fingerprints[path] = installed.AfterSha256 ??
        throw new InvalidDataException("RS144 staged source SHA-256 missing: " + path);
}
payload.Require.TextContains(
    Distribution + "nuget-federation-f11-ir-candidate.md",
    "## Fifteen independent binary-only IR checks");
payload.Require.TextContains(Tool + "Program.cs",
    "DotNetLocalPackageRepository");
payload.Require.TextContains(Tool + "Program.cs",
    "f1-inventory-after");
payload.Require.TextContains(Tool + "fixtures/IrBinaryConsumer.cs",
    "MATH-IR-BINARY|PASS|15");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var original = Run(root, "baseline-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(original.Output.Contains(
     completed ? Qualified :
         "MATH-IR-F11:SOURCE=READY;DEPENDENCY=PINNED;NATIVE=541",
     StringComparison.Ordinal),
     "source/lineage IR candidate baseline check refused");

if (!completed)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException(
            "NUGET_LOCAL is required for the exact published dependency");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
         Directory.Exists(feed), "NUGET_LOCAL must select an existing absolute feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs144", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "f1-restore", "dotnet", [
        "restore", F1Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "f1-build", "dotnet", [
        "build", F1Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", ToolProject, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", ToolProject, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var candidate = Run(root, "ir-candidate", "dotnet", [
        "run", "--project", ToolProject, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-ir", "."
    ]);
    receipts.Add(candidate.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 6,
         "six outer IR source and consumer commands are required");

    var observed = JsonNode.Parse(candidate.Output.Trim())?.AsObject() ??
        throw new InvalidDataException(
            "the IR candidate tool must return an observed JSON object");
    Need(observed["status"]?.GetValue<string>() ==
             "qualified-ir-dependent-calendar-candidate-not-published" &&
         observed["packageId"]?.GetValue<string>() == "dw.tools.math.ir" &&
         observed["historicalSourceVersion"]?.GetValue<string>() ==
             "0.3.0-preview.1" &&
         observed["dependencyQuantitiesId"]?.GetValue<string>() == "dw.quantities" &&
         observed["dependencyQuantitiesVersion"]?.GetValue<string>() ==
             "20260.20261102.1001" &&
         observed["dependencyQuantitiesArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         observed["dependencyQuantitiesDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         observed["sharedNugetDllSha256"]?.GetValue<string>() ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observed["originalSourceProjectReferences"]?.GetValue<int>() == 1 &&
         observed["packageDependencies"]?.GetValue<int>() == 1 &&
         observed["compiledBinaryConsumerChecks"]?.GetValue<int>() == 15 &&
         observed["originalSixSourceFilesLinked"]?.GetValue<bool>() == true &&
         observed["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         observed["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         observed["independentPackageReference"]?.GetValue<bool>() == true &&
         observed["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         observed["published"]?.GetValue<bool>() == false &&
         observed["versionReserved"]?.GetValue<bool>() == false,
         "IR dependent calendar NuGet candidate or fifteen-case consumer was not qualified");

    var receipt = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.nuget-ir-calendar-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.ir",
        ["previous_owner_run"] = "RS143",
        ["publishedDependencyResultCommit"] = "c4290095f37dd71babe93b8097f00c703ed40f80",
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
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After IR package candidate is independently qualified, verify its exact archive/DLL SHA-256, source and private rights. Rehearse the existing immutable no-overwrite publisher on a disposable feed, then explicitly authorize a separate private local publication before DWF readback."
    };
    payload.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet", [
    "run", "--file", Validator, "--", "--check"
]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(
         "MATH-IR-F11:PUBLISHED=false;PUBLIC=false;ROLLBACK=false",
         StringComparison.Ordinal),
     "RS144 IR candidate qualification target was rejected");
return payload.Complete();

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS144: " + reason);
}

static (JsonObject Receipt, string Output) Run(
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
        Path.Combine(root, ".workflow", "local", "nuget-rs144", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var stopwatch = Stopwatch.StartNew();
    Need(process.Start(), "could not launch bounded RS144 process " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS144 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    stopwatch.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + details[..System.Math.Min(details.Length, 4200)]);
    }
    Console.WriteLine("RS144 " + id + " exit=0 elapsed_ms=" + stopwatch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = program + " " + string.Join(" ", args),
        ["elapsed_ms"] = stopwatch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1250)]
    }, output);
}
