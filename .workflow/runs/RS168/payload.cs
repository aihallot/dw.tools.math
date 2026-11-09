using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-numerics-candidate/";
const string Project = Tool + "dw.tools.math.nuget-numerics-candidate.csproj";
const string Validator = Distribution + "ValidateNugetNumericsCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f35-numerics-candidate-qualified.json";
const string Status = "rs168-numerics-dependent-calendar-candidate-qualified-unpublished";
const string Ready = "MATH-NUMERICS-F35:SOURCE=READY;DEPENDENCIES=PINNED;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F35:CANDIDATE=QUALIFIED;BINARY=15;DEPENDENCY=1;NATIVE=541";
const string Limits = "MATH-NUMERICS-F35:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
const string IrDll = "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af";
const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string QuantitiesDll = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));

payload.Workflow.RequireCurrentRun("RS168", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS167", 1,
    "test: independently verify published Composition Quantities and IR NuGet binaries",
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
    Distribution + "nuget-federation-f34-composition-published-qualified.json",
    "/status",
    JsonValue.Create("rs167-composition-published-local-three-binaries-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f34-composition-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f34-composition-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f34-composition-published-qualified.json",
    "/observation/packageId", JsonValue.Create("dw.tools.math.composition"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f34-composition-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(15));
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
    "/observed/packages/9/packageId", JsonValue.Create("dw.tools.math.numerics"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/9/historicalSourceVersion", JsonValue.Create("0.4.0-preview.4"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/9/projectReferences/0", JsonValue.Create("dw.tools.math.ir"));
payload.Require.JsonValueEquals(
    "src/projects/dw.tools.math.numerics/packages.lock.json",
    "/dependencies/net10.0/dw.tools.math.ir/type", JsonValue.Create("Project"));
payload.Require.JsonValueEquals(
    "src/projects/dw.tools.math.numerics/packages.lock.json",
    "/dependencies/net10.0/dw.quantities/type", JsonValue.Create("Project"));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",
    "<Version>0.4.0-preview.4</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",
    "<ProjectReference Include=\"../dw.tools.math.ir/dw.tools.math.ir.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj");

var files = new[]
{
    Distribution + "nuget-federation-f35-numerics-candidate.md",
    Validator,
    Project,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/NumericsBinaryConsumer.cs"
};
// Exact bytes installed by the failed attempt remain admissible for same-run retry.
// Any non-original or non-current staged bytes still refuse before replacement.
var originalStagedHashes = new Dictionary<string, string>(StringComparer.Ordinal)
{
    [Distribution + "nuget-federation-f35-numerics-candidate.md"] = "42386683fbe057bb93d42d4d7db0c5d0275cce89f8a189c734e0f5e0223500e7",
    [Validator] = "fd9b056ab32975a8eb269e8ede056782dc6e42ea2ee046f0f54abf62c9fa8985",
    [Project] = "5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f",
    [Tool + "Directory.Packages.props"] = "42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554",
    [Tool + "Program.cs"] = "d204b7767b71588ab68f7ee9a20327c7ee58c1530cf92f2e709037256e9572a2",
    [Tool + "fixtures/NumericsBinaryConsumer.cs"] = "98383b85fde7a4c773f68bddccda34e351db7f7ece038a77c92a759ea7f6766b",
};
if (!complete)
{
    foreach (var path in files)
    {
        var destination = Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(destination))
        {
            payload.Require.FileMissing(path);
            continue;
        }
        Need((File.GetAttributes(destination) & FileAttributes.ReparsePoint) == 0,
            "retained RS168 target must be a regular file: " + path);
        var retainedSha = FileSha(destination);
        var stagedPath = Path.Combine(root, ".workflow", "runs", "RS168", "staged",
            path.Replace('/', Path.DirectorySeparatorChar));
        Need(retainedSha == originalStagedHashes[path] || retainedSha == FileSha(stagedPath),
            "retained RS168 target differs from original attempt and corrected staged bytes: " + path);
    }
    payload.Require.FileMissing(Evidence);
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
        throw new InvalidDataException("RS168 installed source SHA missing " + path);
}
payload.Require.FileExists(Tool + "fixtures/NumericsBinaryConsumer.cs");
payload.Require.TextContains(Distribution + "nuget-federation-f35-numerics-candidate.md",
    "## Fifteen independent compiled binary probes");
payload.Require.TextContains(Tool + "Program.cs",
    "originalSourceLockSha256 = ShaFile(lockPath)");
payload.Require.TextContains(Tool + "Program.cs", "MATH-NUMERICS-BINARY|PASS|15");
payload.Require.TextContains(Tool + "fixtures/NumericsBinaryConsumer.cs",
    "MATH-NUMERICS-BINARY|PASS|15");
payload.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var sourceCheck = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(sourceCheck.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS168 original Numerics/dependency source verification failed");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL private feed not defined");
    Need(!string.IsNullOrWhiteSpace(feed) &&
         Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
         "NUGET_LOCAL must be an existing absolute private feed");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs168", "packages");
    var commands = new List<JsonNode>();
    commands.Add(Run(root, "tool-restore", "dotnet", [
        "restore", Project, "--source", feed, "--packages", cache,
        "-p:NuGetAudit=false"
    ]).Receipt);
    commands.Add(Run(root, "tool-build", "dotnet", [
        "build", Project, "-c", "Release", "--no-restore",
        "-p:NuGetAudit=false"
    ]).Receipt);
    var process = Run(root, "numerics-candidate", "dotnet", [
        "run", "--project", Project, "-c", "Release",
        "--no-build", "--no-restore", "--", "--qualify-numerics", "."
    ]);
    commands.Add(process.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    commands.Add(sdk.Receipt);
    Need(commands.Count == 4, "four actual outer build, candidate and SDK receipts required");
    var candidate = JsonNode.Parse(process.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("Numerics candidate producer must emit structured JSON");
    Need(candidate["status"]?.GetValue<string>() ==
             "qualified-numerics-dependent-calendar-candidate-not-published" &&
         candidate["packageId"]?.GetValue<string>() == "dw.tools.math.numerics" &&
         candidate["historicalSourceVersion"]?.GetValue<string>() == "0.4.0-preview.4" &&
         candidate["originalProjectSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourceLockSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourcesSha256"]?.AsObject().Count == 4 &&
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
         candidate["originalThreeSourceFilesLinked"]?.GetValue<bool>() == true &&
         candidate["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         candidate["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         candidate["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         candidate["independentPackageReference"]?.GetValue<bool>() == true &&
         candidate["compiledBinaryConsumerChecks"]?.GetValue<int>() == 15 &&
         candidate["published"]?.GetValue<bool>() == false &&
         candidate["versionReserved"]?.GetValue<bool>() == false,
         "qualified exact Numerics source and IR/Quantities transitive closure missing");
    var version = candidate["packageVersion"]?.GetValue<string>() ??
        throw new InvalidDataException("Numerics calendar version missing");
    Need(version.StartsWith("20260.2026", StringComparison.Ordinal) &&
         version.Length == 19 &&
         version.EndsWith(".1001", StringComparison.Ordinal),
         "Numerics package-version allocator did not return first free daily candidate");
    Need(Regex.IsMatch(candidate["candidateArchivePath"]?.GetValue<string>() ?? "",
             @"\Abuild/artifacts/rs168-numerics-candidate/scratch/[0-9a-f]{32}/candidate/" +
             Regex.Escape("dw.tools.math.numerics") + @"\." +
             Regex.Escape(version) + @"\.nupkg\z",
             RegexOptions.CultureInvariant), "unexpected candidate output package path");
    var inner = candidate["commands"]?.AsArray() ??
        throw new InvalidDataException("missing Numerics inner process receipts");
    Need(inner.Count == 11, "eleven actual Numerics candidate and consumer receipts required");
    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.numerics-dependent-calendar-nuget-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.numerics",
        ["previous_owner_run"] = "RS167",
        ["sourceBaselineCommit"] = "a7dd0a6b873affaac57d677ee7a6a6024b44c355",
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
            "After durable Numerics candidate qualification, rehearse the exact original candidate and its IR/Quantities dependency closure on a disposable feed before any owner-only publication."
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
     "RS168 Numerics source-and-binary qualification receipt rejected");
return payload.Complete();

static string FileSha(string file) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));

static void Need(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS168: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs168", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "unable to start Numerics command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Numerics candidate command timed out: " + id);
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
    Console.WriteLine("RS168 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1200)]
    }, output);
}
