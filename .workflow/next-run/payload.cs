using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Distribution = "docs/distribution/";
const string Tool = "tools/dw.tools.math.nuget-statistics-candidate/";
const string Project = Tool + "dw.tools.math.nuget-statistics-candidate.csproj";
const string Validator = Distribution + "ValidateNugetStatisticsCandidate.cs";
const string Evidence = Distribution + "nuget-federation-f24-statistics-candidate-qualified.json";
const string Status = "rs157-statistics-dependent-calendar-candidate-qualified-unpublished";
const string Ready = "MATH-STATISTICS-F24:SOURCE=READY;DEPENDENCY=PINNED;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-STATISTICS-F24:CANDIDATE=QUALIFIED;BINARY=18;DEPENDENCIES=1;NATIVE=541";
const string Limits = "MATH-STATISTICS-F24:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var complete = File.Exists(Path.Combine(root,
    Evidence.Replace('/', Path.DirectorySeparatorChar)));
payload.Workflow.RequireCurrentRun("RS157", null, null, null, null, null);
payload.Workflow.RequirePriorSuccess("RS156", 1,
    "test: verify published Scientific NuGet archive and isolated binary consumer",
    "0.1.63", 0, 0, 0, true);
payload.ProjectPlan.RequireNodeState("M3", "done");
payload.ProjectPlan.RequireNodeState("M7", "done");
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
payload.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
payload.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f23-scientific-published-qualified.json",
    "/status", JsonValue.Create("rs156-scientific-published-local-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f23-scientific-published-qualified.json",
    "/physically_published_package_verified", JsonValue.Create(true));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f23-scientific-published-qualified.json",
    "/package_published_by_this_run", JsonValue.Create(false));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f23-scientific-published-qualified.json",
    "/observation/compiledConsumerChecks", JsonValue.Create(15));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/status", JsonValue.Create("rs137-published-local-math-nuget-binary-independently-verified"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261102.1001"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f4-quantities-published-qualified.json",
    "/observation/archiveSha256", JsonValue.Create(
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d"));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/package_count", JsonValue.Create(13));
payload.Require.JsonValueEquals(
    Distribution + "nuget-federation-f1-qualified.json",
    "/observed/packages/11/packageId", JsonValue.Create("dw.tools.math.statistics"));
payload.Require.JsonValueEquals(
    "src/projects/dw.tools.math.statistics/packages.lock.json",
    "/dependencies/net10.0/dw.quantities/type", JsonValue.Create("Project"));
payload.Require.JsonValueEquals(
    "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json",
    "/authorization/package_from_dw_tools_math", JsonValue.Create(true));
payload.Require.TextContains(
    "src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj",
    "<Version>0.4.0-preview.6</Version>");
payload.Require.TextContains(
    "src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj",
    "<ProjectReference Include=\"../dw.quantities/dw.quantities.csproj\" />");
payload.Require.FileExists(
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj");
var files = new[]
{
    Distribution + "nuget-federation-f24-statistics-candidate.md",
    Validator,
    Project,
    Tool + "Directory.Packages.props",
    Tool + "Program.cs",
    Tool + "fixtures/StatisticsBinaryConsumer.cs"
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
    payload.Require.JsonValueEquals(Evidence, "/source_versions_modified", JsonValue.Create(false));
    payload.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}

var sourceHashes = new JsonObject();
foreach (var file in files)
{
    var installed = payload.Files.ReplaceFromStaged("staged/" + file, file);
    sourceHashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS157 staged source fingerprint missing " + file);
}
payload.Require.FileExists(Tool + "fixtures/StatisticsBinaryConsumer.cs");
payload.Require.TextContains(Distribution + "nuget-federation-f24-statistics-candidate.md",
    "## Eighteen isolated package-only binary probes");
payload.Require.TextContains(Tool + "Program.cs", "originalSourceLockSha256 = ShaFile(lockPath)");
payload.Require.TextContains(Tool + "Program.cs", "MATH-STATISTICS-BINARY|PASS|18");
payload.Require.TextContains(Tool + "fixtures/StatisticsBinaryConsumer.cs",
    "MATH-STATISTICS-BINARY|PASS|18");
payload.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
payload.Require.TextContains(Tool + "Directory.Packages.props",
    "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />");

var initial = Run(root, "source-check", "dotnet",
    ["run", "--file", Validator, "--", "--check"]);
Need(initial.Output.Contains(complete ? Qualified : Ready, StringComparison.Ordinal),
    "RS157 original Statistics sources or pinned package dependency were refused");
if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL must identify existing private package feed");
    Need(!string.IsNullOrWhiteSpace(feed) &&
         Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
         "NUGET_LOCAL must be an existing absolute directory");

    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore", "dotnet",
        ["restore", Project, "--source", feed, "--packages",
         Path.Combine(root, ".workflow", "local", "nuget-rs157", "packages"),
         "-p:NuGetAudit=false"]).Receipt);
    receipts.Add(Run(root, "tool-build", "dotnet",
        ["build", Project, "-c", "Release", "--no-restore",
         "-p:NuGetAudit=false"]).Receipt);
    var candidateProcess = Run(root, "statistics-candidate", "dotnet",
        ["run", "--project", Project, "-c", "Release",
         "--no-build", "--no-restore", "--", "--qualify-statistics", "."]);
    receipts.Add(candidateProcess.Receipt);
    var sdk = Run(root, "sdk", "dotnet", ["--version"]);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "four actual outer process receipts required");

    var candidate = JsonNode.Parse(candidateProcess.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("Statistics package producer must emit exact JSON");
    Need(candidate["status"]?.GetValue<string>() ==
             "qualified-statistics-dependent-calendar-candidate-not-published" &&
         candidate["packageId"]?.GetValue<string>() == "dw.tools.math.statistics" &&
         candidate["historicalSourceVersion"]?.GetValue<string>() == "0.4.0-preview.6" &&
         candidate["originalProjectSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourceLockSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["originalSourcesSha256"]?.AsObject().Count == 9 &&
         candidate["packageDependencies"]?.GetValue<int>() == 1 &&
         candidate["dependencyQuantitiesId"]?.GetValue<string>() == "dw.quantities" &&
         candidate["dependencyQuantitiesVersion"]?.GetValue<string>() == "20260.20261102.1001" &&
         candidate["dependencyQuantitiesArchiveSha256"]?.GetValue<string>() ==
             "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
         candidate["dependencyQuantitiesDllSha256"]?.GetValue<string>() ==
             "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4" &&
         candidate["candidateArchiveSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateDllSha256"]?.GetValue<string>()?.Length == 64 &&
         candidate["candidateArchiveBytes"]?.GetValue<long>() > 2000 &&
         candidate["originalSourceProjectReferences"]?.GetValue<int>() == 1 &&
         candidate["originalEightSourceFilesLinked"]?.GetValue<bool>() == true &&
         candidate["sourceProjectUnmodified"]?.GetValue<bool>() == true &&
         candidate["sourceFeedInventoryUnchanged"]?.GetValue<bool>() == true &&
         candidate["temporaryPackagingProjection"]?.GetValue<bool>() == true &&
         candidate["independentPackageReference"]?.GetValue<bool>() == true &&
         candidate["compiledBinaryConsumerChecks"]?.GetValue<int>() == 18 &&
         candidate["published"]?.GetValue<bool>() == false &&
         candidate["versionReserved"]?.GetValue<bool>() == false,
         "qualified original Statistics candidate and exact Quantities package proof missing");
    var chosenVersion = candidate["packageVersion"]?.GetValue<string>() ??
        throw new InvalidDataException("Statistics candidate calendar version was not provided");
    Need(chosenVersion.StartsWith("20260.2026", StringComparison.Ordinal) &&
         chosenVersion.Length == 19 && chosenVersion.EndsWith(".1001", StringComparison.Ordinal),
         "Statistics allocator did not select an exact first daily calendar candidate");
    Need(candidate["candidateArchivePath"]?.GetValue<string>() ==
             "build/artifacts/rs157-statistics-candidate/candidate/dw.tools.math.statistics." +
             chosenVersion + ".nupkg", "unexpected Statistics candidate output archive path");
    var inner = candidate["commands"]?.AsArray() ??
        throw new InvalidDataException("Statistics internal process evidence absent");
    Need(inner.Count == 11, "exactly eleven F1 tool, package, consumer and SDK receipts required");

    var record = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.statistics-private-dependent-package-candidate",
        ["status"] = Status,
        ["source_package_id"] = "dw.tools.math.statistics",
        ["previous_owner_run"] = "RS156",
        ["sourceBaselineCommit"] = "c279ad23a5268d38f487015d14746d655828d26f",
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
        ["source_files"] = sourceHashes,
        ["command_receipts"] = payload.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "After exact Statistics dependency and candidate qualification, rehearse immutable publishing in a disposable feed; original bytes may be released only by a separate owner gate."
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
     "RS157 original Statistics/Quantities candidate receipt target check failed");
return payload.Complete();

static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS157 Statistics candidate: " + reason);
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
        Path.Combine(root, ".workflow", "local", "nuget-rs157", "packages");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    Need(process.Start(), "cannot start Statistics candidate command " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(540000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Statistics candidate command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var diagnostic = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode +
            "\n" + diagnostic[..System.Math.Min(diagnostic.Length, 4100)]);
    }
    Console.WriteLine("RS157 " + id + " exit=0 elapsed_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..System.Math.Min(output.Length, 1200)]
    }, output);
}
