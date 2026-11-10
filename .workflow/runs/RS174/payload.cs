using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Tool = "tools/dw.tools.math.nuget-electrochemical-candidate/";
const string Project = Tool + "dw.tools.math.nuget-electrochemical-candidate.csproj";
const string Validator = "docs/distribution/ValidateElectrochemicalNugetCandidate.cs";
const string Document = "docs/distribution/electrochemical-nuget-candidate.md";
const string Evidence = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
const string Status = "rs174-electrochemical-standard-calendar-candidate-qualified-unpublished";
const string Baseline = "MATH-ELECTROCHEM-CANDIDATE:SOURCE=READY;BINARY=PENDING;NATIVE=541";
const string Qualified = "MATH-ELECTROCHEM-CANDIDATE:ARCHIVE=VERIFIED;BINARY=12;NATIVE=541";
const string Limits = "MATH-ELECTROCHEM-CANDIDATE:SHARED-PUBLISHED=false;PUBLIC=false;RIGHTS-UNESTABLISHED";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS174", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS173", 1,
    "feat: add explicit Math electrochemical SI unit profile with exact dimensions",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
p.Require.JsonValueEquals(
    "docs/distribution/electrochemical-unit-profile-source-qualified.json",
    "/status", JsonValue.Create(
        "rs173-math-electrochemical-six-unit-source-qualified-not-packaged"));
p.Require.JsonValueEquals(
    "docs/distribution/electrochemical-unit-profile-source-qualified.json",
    "/tests/passed", JsonValue.Create(12));
p.Require.JsonValueEquals(
    "docs/distribution/nuget-federation-f10-standard-published-qualified.json",
    "/observation/archiveSha256",
    JsonValue.Create("c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75"));
p.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<Version>0.2.0-preview.1</Version>");
p.Require.TextContains(
    "src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs",
    "public const string Version = \"0.2.0\";");
p.Require.TextContains(
    "src/projects/dw.quantities.standard/StandardUnitCatalog.cs",
    "public const string CatalogVersion = \"2026-10-exact-v1\";");

var files = new[]
{
    Document, Validator, Project,
    Tool + "Directory.Packages.props", Tool + "Program.cs",
    Tool + "fixtures/ElectrochemicalBinaryConsumer.cs"
};
if (!complete)
{
    foreach (var path in files.Append(Evidence)) p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
    p.Require.JsonValueEquals(Evidence, "/candidate/consumerChecks", JsonValue.Create(12));
}
var hashes = new JsonObject();
foreach (var path in files)
{
    var installed = p.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = installed.AfterSha256 ??
        throw new InvalidDataException("RS174 installed source SHA missing " + path);
}
p.Require.TextContains(Document,
    "## New calendar candidate and actual package-dependent binary consumer");
p.Require.TextContains(Tool + "Program.cs",
    "qualified-electrochemical-standard-calendar-candidate-unpublished");
p.Require.TextContains(Tool + "Program.cs", "publishedToNugetLocal = false");
p.Require.TextContains(Tool + "fixtures/ElectrochemicalBinaryConsumer.cs",
    "MATH-ELECTROCHEM-BINARY|PASS|12");
p.Require.TextContains(Project, "<PackageReference Include=\"Dw.Tools.NuGet\" />");
var ready = Run(root, "source-validator",
    ["run", "--file", Validator, "--", "--check"], 240000);
Need(ready.Output.Contains(complete ? Qualified : Baseline, StringComparison.Ordinal) &&
     ready.Output.Contains(Limits, StringComparison.Ordinal),
     "RS174 source/candidate receipt check refused");

if (!complete)
{
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
        throw new InvalidDataException("NUGET_LOCAL required for published prerequisites");
    Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
        "exact local NuGet dependency feed must be an existing absolute directory");
    var cache = Path.Combine(root, ".workflow", "local", "nuget-rs174", "packages");
    var receipts = new List<JsonNode>();
    receipts.Add(Run(root, "tool-restore",
        ["restore", Project, "--source", feed, "--packages", cache,
         "-p:NuGetAudit=false"], 240000).Receipt);
    receipts.Add(Run(root, "tool-build",
        ["build", Project, "-c", "Release", "--no-restore",
         "-p:NuGetAudit=false"], 240000).Receipt);
    var candidateProcess = Run(root, "independent-package-candidate",
        ["run", "--project", Project, "-c", "Release", "--no-build",
         "--no-restore", "--", "--qualify-candidate", "."], 600000);
    receipts.Add(candidateProcess.Receipt);
    var sdk = Run(root, "sdk", ["--version"], 120000);
    receipts.Add(sdk.Receipt);
    Need(receipts.Count == 4, "four outer command receipts required");
    var observed = JsonNode.Parse(candidateProcess.Output.Trim())?.AsObject() ??
        throw new InvalidDataException("candidate process emitted no structured JSON");
    Need(observed["status"]?.GetValue<string>() ==
             "qualified-electrochemical-standard-calendar-candidate-unpublished" &&
         observed["packageId"]?.GetValue<string>() == "dw.quantities.standard" &&
         observed["historicalSourceVersion"]?.GetValue<string>() == "0.2.0-preview.1" &&
         observed["profileId"]?.GetValue<string>() == "math.si-electrochemical" &&
         observed["profileVersion"]?.GetValue<string>() ==
             "2026-11-electrochemical-exact-v1" &&
         observed["originalSixSourceFilesLinked"]?.GetValue<bool>() == true &&
         observed["consumerChecks"]?.GetValue<int>() == 12 &&
         observed["packageDependencies"]?.GetValue<int>() == 2 &&
         observed["loadedThreeDllHashesVerified"]?.GetValue<bool>() == true &&
         observed["legacyCataloguesPreserved"]?.GetValue<bool>() == true &&
         observed["sourceFilesUnchangedAfter"]?.GetValue<bool>() == true &&
         observed["publishedToNugetLocal"]?.GetValue<bool>() == false &&
         observed["published"]?.GetValue<bool>() == false &&
         observed["auraCodeReuseRightsEstablished"]?.GetValue<bool>() == false &&
         observed["commands"]?.AsArray().Count == 9 &&
         observed["originalSourcesSha256"]?.AsObject().Count == 6 &&
         observed["loadedClrAssemblyIdentities"]?.GetValue<string>()
             ?.StartsWith("MATH-ELECTROCHEM-CLR|standard=",
                 StringComparison.Ordinal) == true &&
         observed["packagedFileInventory"]?.AsArray().Count >= 3 &&
         observed["producerEffectiveSources"]?.AsArray().Count >= 1 &&
         observed["consumerEffectiveSources"]?.AsArray().Count >= 2,
         "exact NuGet candidate/independent consumer qualification incomplete");
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.electrochemical-calendar-nuget-candidate",
        ["status"] = Status,
        ["gate"] = "MATH-ELECTROCHEM-UNIT-PROFILE",
        ["consumer_need"] = "A2-W02-C01-U-N",
        ["source_package_id"] = "dw.quantities.standard",
        ["previous_result_run"] = "RS173",
        ["previous_result_commit"] = "2befc6eb9ea4f54bf17256e91b3cbcb420806e54",
        ["native_nodes_retained"] = 541,
        ["package_published"] = false,
        ["release_authorized"] = false,
        ["public_license_selected"] = false,
        ["aura_code_reuse_rights_established"] = false,
        ["installed_rollback_qualified"] = false,
        ["candidate"] = observed,
        ["source_files"] = hashes,
        ["command_receipts"] = p.Json.Array(receipts.ToArray()),
        ["sdk_version"] = sdk.Output.Trim(),
        ["next_action"] =
            "Qualify exact original-candidate private publisher; publish without overwrite and independently verify published archives in separate gates."
    };
    p.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
p.ProjectPlan.RequireNodeState("M1", "done");
var final = Run(root, "candidate-receipt-validator",
    ["run", "--file", Validator, "--", "--check"], 240000);
Need(final.Output.Contains(Qualified, StringComparison.Ordinal) &&
     final.Output.Contains(Limits, StringComparison.Ordinal),
     "RS174 exact candidate receipt rejected");
return p.Complete();

static void Need(bool okay, string why)
{
    if (!okay) throw new InvalidDataException("RS174: " + why);
}
static (string Output, JsonNode Receipt) Run(
    string root, string id, string[] args, int timeoutMs)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    Need(process.Start(), "unable to launch " + id);
    var watch = Stopwatch.StartNew();
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeoutMs))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS174 timeout " + id);
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    watch.Stop();
    if (process.ExitCode != 0)
    {
        var error = output + "\n" + errors;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            error[..System.Math.Min(error.Length, 4500)]);
    }
    return (output, new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["command"] = "dotnet " + string.Join(" ", args),
        ["elapsed_ms"] = watch.ElapsedMilliseconds,
        ["stdout_excerpt"] = output[..System.Math.Min(output.Length, 1300)],
        ["stderr_excerpt"] = errors[..System.Math.Min(errors.Length, 500)]
    });
}
