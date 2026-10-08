using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string TestPath = "tests/projects/dw.quantities.tests/M1W01C03Tests.cs";
const string EvidencePath = "docs/planning/evidence/M1-W01-C03-red.json";
const string Phase = "M1-W01-C03";
const string Task = "M1-W01-C03-T1";
const string Red = "M1-W01-C03-T1-R";
const string Green = "M1-W01-C03-T1-G";

var sources = new (string RelativePath, string Sha256)[]
{
    ("lib/dw.quantities/DimensionVector.cs", "2c2ebabbb800ff151309dbc4f2cbb611091f687f30dcc5fb742e99339406e5b9"),
    ("lib/dw.quantities/Quantity.cs", "f9121bedc85e810a958f6d2f833652a59f5f7b66a81a8445e9d0c6cec7104e6f"),
    ("lib/dw.quantities/UnitDefinition.cs", "a8eaf48386ec10d6586a8f7be80765fb6e21c6a18acc5309d21a6d544dba676f")
};

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var sourceManifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = sourceManifest["repositories"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["repository"] == "aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("The authorized AURA baseline revision changed.");
var observedRoot = (string?)aura["observed_root"]
    ?? throw new InvalidOperationException("Missing pinned AURA observed root.");
var entries = aura["files"]!.AsArray().Select(x => x!.AsObject()).ToArray();

JsonObject Discover(string source, string sha)
{
    var pinned = entries.Single(x => (string?)x["path"] == source);
    if ((string?)pinned["sha256"] != sha)
        throw new InvalidOperationException("AURA manifest SHA-256 differs for " + source);
    var fullPath = Path.Combine(observedRoot, source.Replace('/', Path.DirectorySeparatorChar));
    var bytes = File.ReadAllBytes(fullPath);
    var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if (actual != sha)
        throw new InvalidOperationException("AURA source snapshot drifted for " + source);

    var content = new UTF8Encoding(false, true).GetString(bytes);
    var lines = content.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
    var declarations = lines.Select(x => x.Trim())
        .Where(x => x.StartsWith("public ", StringComparison.Ordinal) ||
                    x.StartsWith("internal ", StringComparison.Ordinal) ||
                    x.StartsWith("namespace ", StringComparison.Ordinal) ||
                    x.StartsWith("using ", StringComparison.Ordinal))
        .Take(200).ToArray();
    var imports = lines.Select(x => x.Trim())
        .Where(x => x.StartsWith("using ", StringComparison.Ordinal))
        .Distinct(StringComparer.Ordinal).Take(60).ToArray();
    var names = new[] { "DimensionVector", "Quantity", "UnitDefinition", "ExactRational",
        "ExactBinaryNumber", "ExactDecimalFormatter" };
    var mentions = names.Where(name => content.Contains(name, StringComparison.Ordinal)).ToArray();
    return new JsonObject
    {
        ["source_path"] = source,
        ["target_path"] = "src/projects/dw.quantities/" + Path.GetFileName(source),
        ["sha256"] = sha,
        ["byte_count"] = bytes.Length,
        ["line_count"] = lines.Length,
        ["declaration_lines_captured"] = declarations.Length,
        ["declaration_capture_limit"] = 200,
        ["declarations"] = p.Json.StringArray(declarations),
        ["imports"] = p.Json.StringArray(imports),
        ["candidate_type_mentions"] = p.Json.StringArray(mentions),
        ["observation_limit"] = "Public/internal and namespace/using declaration lines are captured. This is not proof that multi-line API signatures or transitive dependencies are complete."
    };
}

var inspected = sources.Select(pair => Discover(pair.RelativePath, pair.Sha256)).ToArray();

p.Files.ReplaceFromStaged("staged/" + TestPath, TestPath);
var red = Run(root, "dotnet", "test", "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--filter", "FullyQualifiedName~M1W01C03Tests",
    "--logger", "console;verbosity=normal");
var expectedMarkers = new[]
{
    "M1-W01-C03 RED: DimensionVector is not materialized in Math.",
    "M1-W01-C03 RED: Quantity is not materialized in Math.",
    "M1-W01-C03 RED: UnitDefinition is not materialized in Math."
};
if (red.Code == 0 || expectedMarkers.Any(marker => !red.Output.Contains(marker, StringComparison.Ordinal)))
    throw new InvalidOperationException("The three-source RED did not fail for each expected type: " + red.Output);

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C03-red",
    ["status"] = "expected-absence-observed",
    ["aura_baseline"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["source_candidates"] = p.Json.Array(inspected),
    ["test_project"] = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    ["test_filter"] = "FullyQualifiedName~M1W01C03Tests",
    ["observed_missing_type_markers"] = p.Json.StringArray(expectedMarkers),
    ["run_result"] = "The targeted tests failed for all three missing public types before source transfer.",
    ["accepted_future_cases"] = p.Json.StringArray(
        "3 m * 4 m = 12 m squared",
        "10 km / 30 min = 50/9 m/s",
        "32 degF = 0 degC",
        "a 9 degF interval equals 5 K",
        "reject addition of mass and time",
        "bound dimension-exponent overflow and direct absolute/interval operations"),
    ["limitations"] = p.Json.StringArray(
        "No source is copied in this RED run.",
        "Declaration lines and candidate imports are observations, not compiled API verification.",
        "Physical units and affine temperature behavior remain unqualified until independent GREEN tests run.")
};
p.Files.WriteComplete(EvidencePath, evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

var sourceState = false;
p.Json.EditObject("docs/planning/backlog.json", product =>
{
    var chunk = product["chunks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == Phase);
    var task = chunk["tasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == Task);
    var subtasks = task["subtasks"]!.AsArray().Select(x => x!.AsObject()).ToArray();
    var redNode = subtasks.Single(x => (string?)x["id"] == Red);
    var greenNode = subtasks.Single(x => (string?)x["id"] == Green);
    var verifyNode = subtasks.Single(x => (string?)x["id"] == "M1-W01-C03-T1-V");
    var t2 = chunk["tasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == "M1-W01-C03-T2");

    sourceState = (string?)product["plan_version"] == "0.1.12" &&
        (string?)chunk["status"] == "planned" &&
        (string?)task["status"] == "planned" &&
        (string?)redNode["status"] == "planned" &&
        (string?)greenNode["status"] == "planned" &&
        (string?)verifyNode["status"] == "planned" &&
        (string?)t2["status"] == "planned";
    var targetState = (string?)product["plan_version"] == "0.1.13" &&
        (string?)chunk["status"] == "in_progress" &&
        (string?)task["status"] == "in_progress" &&
        (string?)redNode["status"] == "done" &&
        (string?)greenNode["status"] == "ready" &&
        (string?)verifyNode["status"] == "planned" &&
        (string?)t2["status"] == "planned";
    if (!sourceState && !targetState)
        throw new InvalidOperationException("C03 status is neither the accepted baseline nor exact RED target.");

    product["plan_version"] = "0.1.13";
    chunk["status"] = "in_progress";
    chunk["refinement"] = "RS014 pins and inspects all three AURA sources and establishes independent missing-type RED. Subsequent GREEN must qualify exact dimensions, quantities, direct affine temperature behavior, and exponent boundaries against their observed APIs.";
    chunk["files"] = p.Json.StringArray(
        "src/projects/dw.quantities/DimensionVector.cs",
        "src/projects/dw.quantities/Quantity.cs",
        "src/projects/dw.quantities/UnitDefinition.cs", TestPath, EvidencePath);
    chunk["commands"] = p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C03Tests",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    chunk["evidence"] = p.Json.StringArray(EvidencePath);
    task["status"] = "in_progress";
    redNode["status"] = "done";
    redNode["evidence"] = p.Json.StringArray(EvidencePath);
    greenNode["status"] = "ready";
});
if (sourceState)
{
    p.ProjectPlan.TransitionNode(Phase, "not-ready", "ready");
    p.ProjectPlan.TransitionNode(Task, "not-ready", "ready");
    p.ProjectPlan.TransitionNode(Red, "not-ready", "ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    p.ProjectPlan.TransitionNode(Green, "not-ready", "ready");
}
else
{
    p.ProjectPlan.RequireNodeState(Phase, "in-progress");
    p.ProjectPlan.RequireNodeState(Task, "in-progress");
    p.ProjectPlan.RequireNodeState(Red, "done");
    p.ProjectPlan.RequireNodeState(Green, "ready");
}

var rendered = Run(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
if (rendered.Code != 0)
    throw new InvalidOperationException("Product plan projections failed: " + rendered.Output);
return p.Complete();

static (int Code, string Output) Run(string root, string executable, params string[] args)
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
    foreach (var argument in args)
        process.StartInfo.ArgumentList.Add(argument);
    if (!process.Start())
        throw new InvalidOperationException("Cannot start " + executable);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException(executable + " timed out.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode, output);
}
