using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-codecs-candidate/";
const string Project = Tool + "dw.tools.math.nuget-codecs-candidate.csproj";
const string Validator = Distribution + "ValidateNugetCodecsCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f28-codecs-candidate-qualified.json";
const string Status = "rs161-codecs-dependent-calendar-candidate-qualified-unpublished";
const string Ready = "MATH-CODECS-F28:SOURCE=READY;DEPENDENCIES=PINNED;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-CODECS-F28:CANDIDATE=QUALIFIED;BINARY=15;DEPENDENCY=1;NATIVE=541";
const string Limits = "MATH-CODECS-F28:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
const string IrDll = "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af";
const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string QuantitiesDll = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS161", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS160", 1,
    "test: independently verify published Statistics and Quantities NuGet binaries",
    "0.1.66", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f27-statistics-published-qualified.json",
    "/status",
    JsonValue.Create("rs160-statistics-published-local-two-binaries-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f27-statistics-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f27-statistics-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/status", JsonValue.Create("rs146-ir-published-local-two-binaries-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261104.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/observation/archiveSha256", JsonValue.Create(IrSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/observation/packagedDllSha256", JsonValue.Create(IrDll));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/observation/exactDependencies/0/archiveSha256", JsonValue.Create(QuantitiesSha));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f13-ir-published-qualified.json",
    "/observation/exactDependencies/0/packagedDllSha256", JsonValue.Create(QuantitiesDll));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json", "/package_count",
    JsonValue.Create(13));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/4/packageId", JsonValue.Create("dw.tools.math.codecs"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/4/historicalSourceVersion", JsonValue.Create("1.0.0-preview.8"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/4/projectReferences", JsonValue.Create(new[] { "dw.tools.math.ir" }));
payload.Require.JsonValueEquals(
    "src/projects/dw.tools.math.codecs/packages.lock.json",
    "/dependencies/net10.0/dw.tools.math.ir/type", JsonValue.Create("Project"));
payload.Require.JsonValueEquals(
    "src/projects/dw.tools.math.codecs/packages.lock.json",
    "/dependencies/net10.0/dw.quantities/type", JsonValue.Create("Project"));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",
    "<Version>1.0.0-preview.8</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",
    "<ProjectReference Include=\"../dw.tools.math.ir/dw.tools.math.ir.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj");

var files = new[]
{
    Distribution + "nuget-federation-f28-codecs-candidate.md",
    Validator,
    Project,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/CodecsBinaryConsumer.cs"
};
if (!complete)
{
    foreach (var path in files.Append(Evidence)) payload.Require.FileMissing(path);
}
else
{
    payload.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    payload.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/source_versions_modified", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}
var hashes = new JsonObject();
foreach (var path in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = installed.AfterSha256 ??
        throw new InvalidDataException("RS161 installed source SHA missing " + path);
}
payload.Require.FileExists(Tool + "fixtures/CodecsBinaryConsumer.cs");
payload.Require.TextContains(Distribution + "nuget-federation-f28-codecs-candidate.md",
    "## Fifteen independent compiled binary probes");
payload.Require.TextContains(Tool + "Program.cs",
    "originalSourceLockSha256 = ShaFile(lockPath)");
payload.Require.TextContains(Tool + "Program.cs", "MATH-CODECS-BINARY|PASS|15");
payload.Require.TextContains(Tool + "fixtures/CodecsBinaryConsumer.cs",
    "MATH-CODECS-BINARY|PASS|15");
payload.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var sourceCheck = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(sourceCheck.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS161 original Codecs/dependency source verification failed");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL private feed not defined");
    Need(!string.IsNullOrWhiteSpace(feed) &&
         Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
         "NUGET_LOCAL must be an existing absolute private feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs161", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var process = Run(root, "codecs-candidate", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-codecs", "."
    ]);
    commands.Add(process.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four actual outer build, candidate and SDK receipts required");
    var candidate = JsonNode.Parse(process.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("Codecs candidate producer must emit structured JSON");
    Need(candidate["status"]?.GetValue<string>() ==
             "qualified-codecs-dependent-calendar-candidate-not-published" &&
         candidate["packageId"]?.GetValue<string>() == "dw.tools.math.codecs" &&
         candidate["historicalSourceVersion"]?.GetValue<string>() == "1.0.0-preview.8" &&
         candidate["originalProjectSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourceLockSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourcesSha256"]?.AsObject().Count == 10 &&
         candidate["packageDependencies"]?.GetValue<int>() == 1 &&
         candidate["dependencyIrId"]?.GetValue<string>() == "dw.tools.math.ir" &&
         candidate["dependencyIrVersion"]?.GetValue<string>() == "20260.20261104.1001" &&
         candidate["dependencyIrArchiveSha256"]?.GetValue<string>() == IrSha &&
         candidate["dependencyIrDllSha256"]?.GetValue<string>() == IrDll &&
         candidate["dependencyQuantitiesId"]?.GetValue<string>() == "dw.quantities" &&
         candidate["dependencyQuantitiesVersion"]?.GetValue<string>() == "20260.20261102.1001" &&
         candidate["dependencyQuantitiesArchiveSha256"]?.GetValue<string>() == QuantitiesSha &&
         candidate["dependencyQuantitiesDllSha256"]?.GetValue<string>() == QuantitiesDll &&
         candidate["candidateArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateDllSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateArchiveBytes"]?.GetValue<long>() > 2000 &&
         candidate["originalSourceProjectReferences"]?.GetValue<int>() == 1 &&
         candidate["originalNineSourceFilesLinked"]?.GetValue<bool>() == true &&
         candidate["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         candidate["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         candidate["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         candidate["independentPackageReference"]?.GetValue<bool>() == true &&
         candidate["compiledBinaryConsumerChecks"]?.GetValue<int>() == 15 &&
         candidate["published"]?.GetValue<bool>() == false &&
         candidate["versionReserved"]?.GetValue<bool>() == false,
         "qualified exact Codecs source and IR/Quantities transitive closure missing");
    var version = candidate["packageVersion"]?.GetValue<string>() ??
        throw new InvalidDataException("Codecs calendar version missing");
    Need(version.StartsWith("20260.2026", StringComparison.Ordinal) &&
         version.Length == 19 &&
         version.EndsWith(".1001", StringComparison.Ordinal),
         "Codecs package-version allocator did not return first free daily candidate");
    Need(candidate["candidateArchivePath"]?.GetValue<string>() ==
             "build/artifacts/rs161-codecs-candidate/candidate/dw.tools.math.codecs." +
             version + ".nupkg", "unexpected candidate output package path");
    var inner = candidate["commands"]?.AsArray() ??
        throw new InvalidDataException("missing Codecs inner process receipts");
    Need(inner.Count == 11, "eleven actual Codecs candidate and consumer receipts required");
    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.codecs-dependent-calendar-nuget-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.codecs",
        ["previous_owner_run"] = "RS160",
        ["sourceBaselineCommit"] = "fbe8bd62c59adc4a083379b9a400a06cbf2c98b4",
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
        ["source_files"] = hashes,
        ["command_receipts"] = payload.Json.Array(commands.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After durable Codecs candidate qualification, rehearse the exact original candidate and its IR/Quantities dependency closure on a disposable feed before any owner-only publication."
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
     "RS161 Codecs source-and-binary qualification receipt rejected");
return payload.Complete();

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS161: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs161", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start Codecs command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Codecs candidate command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var info = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            info[..System.Math.Min(info.Length, 4100)]);
    }
    Console.WriteLine("RS161 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1200)]
    }, output);
}
