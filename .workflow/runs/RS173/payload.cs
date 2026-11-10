using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Source = "src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs";
const string Tests = "tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs";
const string Documentation = "docs/distribution/electrochemical-unit-profile.md";
const string Evidence = "docs/distribution/electrochemical-unit-profile-source-qualified.json";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string Status = "rs173-math-electrochemical-six-unit-source-qualified-not-packaged";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS173", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS172", 1,
    "test: reconcile original Numerics F38 binary validation evidence",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M1", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
p.Require.JsonValueEquals(
    "docs/distribution/nuget-federation-f38-numerics-reconciled.json",
    "/status", JsonValue.Create(
        "rs172-f38-original-numerics-dwf-physical-evidence-reconciled-unpublished"));
p.Require.JsonValueEquals(
    "docs/distribution/nuget-federation-f10-standard-published-qualified.json",
    "/observation/packageVersion", JsonValue.Create("20260.20261103.1001"));
p.Require.TextContains(
    "src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs",
    "public const string Version = \"0.2.0\";");
p.Require.TextContains(
    "src/projects/dw.quantities.standard/StandardUnitCatalog.cs",
    "public const string CatalogVersion = \"2026-10-exact-v1\";");
p.Require.TextContains(
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
    "<Version>0.2.0-preview.1</Version>");

if (!complete)
{
    foreach (var path in new[] { Source, Tests, Documentation, Evidence })
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/tests/selected", JsonValue.Create(12));
    p.Require.JsonValueEquals(Evidence, "/tests/passed", JsonValue.Create(12));
    p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/package_candidate_created", JsonValue.Create(false));
}

var hashes = new JsonObject();
foreach (var file in new[] { Source, Tests, Documentation })
{
    var installed = p.Files.ReplaceFromStaged("staged/" + file, file);
    hashes[file] = installed.AfterSha256 ??
        throw new InvalidDataException("RS173 missing exact installed SHA: " + file);
}
p.Require.TextContains(Source, "public static class ElectrochemicalUnitProfile");
p.Require.TextContains(Source,
    "public const string ProfileVersion = \"2026-11-electrochemical-exact-v1\";");
p.Require.TextContains(Tests,
    "public sealed class ElectrochemicalUnitProfileTests");
p.Require.TextContains(Documentation, "## Preservation and release plan");

if (!complete)
{
    var restore = Execute(root, "locked-restore",
        ["restore", TestProject, "--locked-mode", "-p:NuGetAudit=false"], 300000);
    var resultDirectory = Path.Combine(root, "build", "artifacts",
        "rs173-electrochemical-targeted", Guid.NewGuid().ToString("N"));
    Need(!Directory.Exists(resultDirectory),
        "fresh targeted MSTest results directory collided");
    // The test runner owns creation of its disposable results directory.
    // A DWF payload must not mutate repository directories directly.
    var run = Execute(root, "targeted-mstest",
        ["test", TestProject, "-c", "Release", "--no-restore",
         "--filter", "FullyQualifiedName~ElectrochemicalUnitProfileTests",
         "--results-directory", resultDirectory,
         "--logger", "trx;LogFileName=electrochem.trx",
         "-p:NuGetAudit=false"], 540000);
    var reports = Directory.EnumerateFiles(resultDirectory, "electrochem.trx",
        SearchOption.AllDirectories).ToArray();
    Need(reports.Length == 1, "exactly one targeted TRX receipt required");
    var trx = XDocument.Load(reports[0], LoadOptions.None);
    var counters = trx.Descendants().Single(x => x.Name.LocalName == "Counters");
    static int Counter(XElement source, string key) =>
        int.Parse((string?)source.Attribute(key) ??
            throw new InvalidDataException("TRX counter missing: " + key),
            System.Globalization.CultureInfo.InvariantCulture);
    var total = Counter(counters, "total");
    var passed = Counter(counters, "passed");
    var failed = Counter(counters, "failed");
    Need(total == 12 && passed == 12 && failed == 0 &&
         Counter(counters, "executed") == 12,
         "the twelve new Math-owned source cases did not all pass");
    var sdk = Execute(root, "sdk", ["--version"], 120000);
    var evidence = new JsonObject
    {
        ["schemaVersion"] = 1,
        ["kind"] = "dw.tools.math.electrochemical-original-source-qualification",
        ["status"] = Status,
        ["request_gate"] = "MATH-ELECTROCHEM-UNIT-PROFILE",
        ["consumer_need"] = "A2-W02-C01-U-N",
        ["source_package_id"] = "dw.quantities.standard",
        ["source_project_version_retained"] = "0.2.0-preview.1",
        ["profile_clr_type"] = "dw.quantities.standard.ElectrochemicalUnitProfile",
        ["profile_id"] = "math.si-electrochemical",
        ["profile_version"] = "2026-11-electrochemical-exact-v1",
        ["unit_count"] = 6,
        ["historical_catalogue_version"] = "0.2.0",
        ["historical_unit_count"] = 58,
        ["standard_catalogue_version"] = "2026-10-exact-v1",
        ["standard_unit_count"] = 14,
        ["package_candidate_created"] = false,
        ["package_published"] = false,
        ["independent_package_consumer_qualified"] = false,
        ["aura_code_reuse_rights_established"] = false,
        ["public_license_selected"] = false,
        ["tests"] = new JsonObject
        {
            ["selected"] = total,
            ["passed"] = passed,
            ["failed"] = failed,
            ["definition_cases"] = 6,
            ["other_cases"] = 6,
            ["scope"] = "Math-owned source-compiled MSTest",
            ["filter"] = "FullyQualifiedName~ElectrochemicalUnitProfileTests"
        },
        ["source_files"] = hashes,
        ["sdk_version"] = sdk.Output.Trim(),
        ["commands"] = p.Json.Array([restore.Receipt, run.Receipt, sdk.Receipt]),
        ["next_action"] =
            "Qualify a new calendar-version dw.quantities.standard candidate using a fresh exact PackageReference-only consumer; private release and independent readback remain separate."
    };
    p.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
p.ProjectPlan.RequireNodeState("M1", "done");
p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
p.Require.JsonValueEquals(Evidence, "/tests/passed", JsonValue.Create(12));
p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
return p.Complete();

static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS173: " + reason);
}

static (string Output, JsonNode Receipt) Execute(
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
    Need(process.Start(), "unable to start " + id);
    var clock = Stopwatch.StartNew();
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeoutMs))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException(id + " timed out");
    }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
    {
        var message = output + "\n" + error;
        throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
            message[..System.Math.Min(message.Length, 4000)]);
    }
    return (output, new JsonObject
    {
        ["id"] = id,
        ["exit_code"] = process.ExitCode,
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["command"] = "dotnet " + string.Join(" ", args),
        ["stdout_excerpt"] = output[..System.Math.Min(output.Length, 1400)]
    });
}
