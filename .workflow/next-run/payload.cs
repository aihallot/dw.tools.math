using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-composition-release/";
const string Project = Tool + "dw.tools.math.nuget-composition-release.csproj";
const string Controller = Tool + "Program.cs";
const string Documentation = Distribution + "nuget-federation-f33-composition-release-receipt.md";
const string Validator = Distribution + "ValidateNugetCompositionReleaseReceipt.cs";
const string Evidence = Distribution + "nuget-federation-f33-composition-release-receipt-qualified.json";
const string Status = "rs166-composition-private-receipt-two-dependencies-corrected-unpublished";
const string Ready = "MATH-COMPOSITION-F33:RECEIPT=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified = "MATH-COMPOSITION-F33:RECEIPT=CORRECTED;BUILD=PASS;NATIVE=541";
const string Limits = "MATH-COMPOSITION-F33:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";
const string ControllerSha = "08a7dccd4fd8e7df6f0e94f17a37dafaadad68c94a6d3e8b9bb28bfeebf64f30";
const string ArchiveSha = "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f";
const string DllSha = "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS166", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS165", 1,
    "test: qualify Composition immutable NuGet publisher rehearsal with two dependencies",
    "0.1.66", 2, 2, 0, false);
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/status", JsonValue.Create(
        "rs164-composition-dependent-calendar-candidate-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/packageId", JsonValue.Create("dw.tools.math.composition"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/packageVersion", JsonValue.Create("20260.20261107.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/candidateArchiveSha256", JsonValue.Create(ArchiveSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/candidateDllSha256", JsonValue.Create(DllSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f31-composition-candidate-qualified.json",
    "/candidate/packageDependencies", JsonValue.Create(2));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f32-composition-publisher-qualified.json",
    "/status", JsonValue.Create(
        "rs165-composition-immutable-local-publisher-rehearsal-qualified-unpublished"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f32-composition-publisher-qualified.json",
    "/package_published", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f32-composition-publisher-qualified.json",
    "/rehearsal/dependencyCount", JsonValue.Create(2));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f32-composition-publisher-qualified.json",
    "/rehearsal/publishedToNugetLocal", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f32-composition-publisher-qualified.json",
    "/source_files/tools~1dw.tools.math.nuget-composition-release~1Program.cs",
    JsonValue.Create(ControllerSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/redistribute_from_dw_tools_math", JsonValue.Create(true));
payload.Require.FileExists(Tool + "Directory.Packages.props");
payload.Require.FileExists(Project);
payload.Require.FileExists(Controller);
payload.Require.FileExists(".workflow/runs/RS165/staged/" + Controller);

var sources = new[] { Documentation, Validator, Controller };
if (!complete)
{
    foreach (var file in sources.Append(Evidence))
        payload.Require.FileMissing(file);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/release_modes_invoked",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/package_published_by_this_run",
        JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/direct_dependency_count",
        JsonValue.Create(2));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained",
        JsonValue.Create(541));
}

var sourceHashes = new JsonObject();
foreach (var path in sources)
{
    var changed = payload.Files.ReplaceFromStaged("staged/" + path, path);
    sourceHashes[path] = changed.AfterSha256 ??
        throw new InvalidDataException("RS166 staged SHA missing: " + path);
}
payload.Require.TextContains(Documentation, "## Exact RS166 correction and provenance");
payload.Require.TextContains(Controller, "directDependencyCount = Dependencies.Length");
payload.Require.TextContains(Controller, "MATH-COMPOSITION-BINARY|PASS|15");
payload.Require.TextContains(Controller, "RequireQualifiedRunAndCleanCheckout(root)");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var initial = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(initial.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "read-only prepublication receipt source validator refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
    Need(!string.IsNullOrWhiteSpace(feed) &&
         Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
         "NUGET_LOCAL must be an accessible absolute private feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs166", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
    ]).Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 3, "all three actual restore/build/SDK receipts required");

    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.composition-private-release-receipt-completeness",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.composition",
        ["preceding_run"] = "RS165",
        ["sourceBaselineCommit"] = "732bf99a8a0a7e760943654acc1f34766be66732",
        ["historical_controller_sha256"] = ControllerSha,
        ["corrected_controller_sha256"] = sourceHashes[Controller]?.GetValue<string>() ??
            throw new InvalidDataException("RS166 corrected controller fingerprint missing"),
        ["original_archive_sha256"] = ArchiveSha,
        ["original_dll_sha256"] = DllSha,
        ["direct_dependency_count"] = 2,
        ["direct_dependencies"] = new JsonArray(
            new JsonObject
            {
                ["id"] = "dw.quantities",
                ["version"] = "20260.20261102.1001",
                ["archiveSha256"] =
                    "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
                ["packagedDllSha256"] =
                    "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4"
            },
            new JsonObject
            {
                ["id"] = "dw.tools.math.ir",
                ["version"] = "20260.20261104.1001",
                ["archiveSha256"] =
                    "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
                ["packagedDllSha256"] =
                    "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"
            }),
        ["native_nodes_retained"] = 541,
        ["release_controller_compiled"] = true,
        ["release_modes_invoked"] = false,
        ["package_published_by_this_run"] = false,
        ["public_registry_published"] = false,
        ["new_public_license_selected"] = false,
        ["installed_package_rollback_qualified"] = false,
        ["original_composition_sources_modified"] = false,
        ["external_requests_transmitted"] = false,
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Only after RS166 succeeds may an owner separately authorize exact original F31 Composition private publication and verification; then independently qualify physical feed readback in a distinct DWF run."
    };
    payload.Files.WriteComplete(Evidence,
        record.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
var final = Run(root, "target-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
     "RS166 two-dependency publication receipt qualification target check failed");
return payload.Complete();

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS166: " + message);
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
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, ".workflow", "local", "nuget-rs166", "packages");
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    var watch = Stopwatch.StartNew();
    Need(process.Start(), "cannot start " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS166 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var details = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            details[..System.Math.Min(details.Length, 4200)]);
    }
    Console.WriteLine("RS166 " + id + " exit=0 elapsed_ms=" + watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", arguments),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1100)]
    }, output);
}
