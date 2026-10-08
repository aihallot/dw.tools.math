using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W01-C03";
const string Task = "M1-W01-C03-T2";
const string Red = "M1-W01-C03-T2-R";
const string Green = "M1-W01-C03-T2-G";
const string Verify = "M1-W01-C03-T2-V";
const string RedTests = "tests/projects/dw.quantities.tests/M1W01C03BoundaryRedTests.cs";
const string GreenTests = "tests/projects/dw.quantities.tests/M1W01C03BoundaryTests.cs";
const string DimensionPath = "src/projects/dw.quantities/DimensionVector.cs";
const string UnitPath = "src/projects/dw.quantities/UnitDefinition.cs";
const string RedEvidence = "docs/planning/evidence/M1-W01-C03-boundary-red.json";
const string QualifiedEvidence = "docs/planning/evidence/M1-W01-C03-boundary-qualified.json";
const string Marker = "M1-W01-C03-T2 RED: direct DimensionVector exponent overflow is unchecked";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var document = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = document["chunks"]!.AsArray().Select(n => n!.AsObject())
    .Single(n => (string?)n["id"] == Phase);
var task = chunk["tasks"]!.AsArray().Select(n => n!.AsObject())
    .Single(n => (string?)n["id"] == Task);
var stateById = task["subtasks"]!.AsArray().Select(n => n!.AsObject())
    .ToDictionary(n => (string)n["id"]!, n => (string)n["status"]!, StringComparer.Ordinal);
var baseline = (string?)document["plan_version"] == "0.1.14" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)task["status"] == "ready" &&
    stateById[Red] == "ready" && stateById[Green] == "planned" && stateById[Verify] == "planned";
var target = (string?)document["plan_version"] == "0.1.15" &&
    (string?)chunk["status"] == "done" &&
    (string?)task["status"] == "done" &&
    stateById[Red] == "done" && stateById[Green] == "done" && stateById[Verify] == "done";
if (!baseline && !target)
    throw new InvalidOperationException("RS016 product lifecycle is neither the accepted baseline nor the complete target.");

// RED is independently observed before changing any source; target re-entry skips an
// impossible repeated RED, but still reruns the complete GREEN and regression evidence.
p.Files.ReplaceFromStaged("staged/" + RedTests, RedTests);
if (baseline)
{
    var red = Run(root, "dotnet", "test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c", "Release",
        "--filter", "FullyQualifiedName~M1W01C03BoundaryRedTests.DimensionExponentOverflowIsRejectedAtDirectPublicOperator",
        "--logger", "console;verbosity=normal");
    if (red.Code == 0 || !red.Output.Contains(Marker, StringComparison.Ordinal))
        throw new InvalidOperationException(
            "The independently observed exponent-overflow RED did not match its expected failure: " +
            ImportantFailure(red.Output));
}

var redDocument = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C03-boundary-red",
    ["status"] = "expected-exponent-overflow-red-observed",
    ["test"] = "M1W01C03BoundaryRedTests.DimensionExponentOverflowIsRejectedAtDirectPublicOperator",
    ["failure_marker"] = Marker,
    ["observed_before_source_mutation"] = true,
    ["scope"] = "Unchecked public dimension exponent addition is observed before modifying the Math source; no claim about temperature behavior is inferred from this RED."
};
p.Files.WriteComplete(RedEvidence, redDocument.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

// The original AURA-derived bytes are the only admitted source state, apart from
// the complete staged target. SDK convergence rejects undeclared third states.
p.Files.ConvergeFromStaged(
    "staged/" + DimensionPath, DimensionPath,
    "2c2ebabbb800ff151309dbc4f2cbb611091f687f30dcc5fb742e99339406e5b9");
p.Files.ConvergeFromStaged(
    "staged/" + UnitPath, UnitPath,
    "a8eaf48386ec10d6586a8f7be80765fb6e21c6a18acc5309d21a6d544dba676f");
p.Files.ReplaceFromStaged("staged/" + GreenTests, GreenTests);

RunRequired(root, "dotnet", "build",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--verbosity", "minimal");
RunRequired(root, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--no-build", "--no-restore",
    "--filter", "FullyQualifiedName~M1W01C03",
    "--logger", "console;verbosity=normal");
RunRequired(root, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--no-build", "--no-restore",
    "--logger", "console;verbosity=minimal");
RunRequired(root, "pwsh", "-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1");

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C03-boundary-qualified",
    ["status"] = "qualified-with-explicit-typed-temperature-boundary",
    ["prior_transfer"] = "docs/planning/evidence/M1-W01-C03-green.json",
    ["independent_red"] = RedEvidence,
    ["changed_production_files"] = p.Json.StringArray(DimensionPath, UnitPath),
    ["observed_contracts"] = p.Json.StringArray(
        "All eight DimensionVector exponent additions and subtractions detect int overflow, including operations through Quantity.",
        "TemperatureMeasurement distinguishes exact absolute temperatures from temperature intervals.",
        "UnitDefinition.ToBase/FromBase overloads for TemperatureMeasurement apply offsets only to absolutes and scale only to intervals.",
        "Absolute + absolute and interval - absolute are rejected at TemperatureMeasurement.Add/Subtract.",
        "Absolute - absolute produces an interval and absolute +/- interval preserves an absolute.",
        "Invalid temperature kind and non-temperature dimension are rejected in typed temperature conversion.",
        "Exact rational values remain BigInteger-based and are not converted to double."),
    ["focused_filter"] = "FullyQualifiedName~M1W01C03",
    ["focused_tests_passed"] = true,
    ["dw_quantities_tests_passed"] = true,
    ["foundation_chain_passed"] = true,
    ["limits"] = p.Json.StringArray(
        "The prior scalar UnitDefinition.ToBase(ExactRational)/FromBase(ExactRational) remain for compatibility; callers must use the tagged overload for safe absolute/interval semantics.",
        "Bare Quantity carries dimensional exponents but cannot encode whether temperature is an affine point; TemperatureMeasurement is required at the affine boundary.",
        "Checked int exponent overflow is a representational bound, not a general resource-budget guarantee.",
        "This run changes only Math. No AURA, Decision, MCDM or brainstorming adoption is implied.")
};
p.Files.WriteComplete(QualifiedEvidence,
    evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

p.Json.EditObject("docs/planning/backlog.json", product =>
{
    var c = product["chunks"]!.AsArray().Select(n => n!.AsObject())
        .Single(n => (string?)n["id"] == Phase);
    var t = c["tasks"]!.AsArray().Select(n => n!.AsObject())
        .Single(n => (string?)n["id"] == Task);
    product["plan_version"] = "0.1.15";
    c["status"] = "done";
    c["refinement"] = "RS014 established source-pinned RED. RS015 qualified functional dimension/quantity/affine conversion GREEN. RS016 observed exponent-overflow RED, implemented checked exponent arithmetic and typed absolute/interval temperature semantics, and verified the complete Math test project and foundation regression.";
    c["files"] = p.Json.StringArray(
        "src/projects/dw.quantities/DimensionVector.cs",
        "src/projects/dw.quantities/Quantity.cs",
        "src/projects/dw.quantities/UnitDefinition.cs",
        "tests/projects/dw.quantities.tests/M1W01C03Tests.cs",
        RedTests, GreenTests,
        "docs/planning/evidence/M1-W01-C03-red.json",
        "docs/planning/evidence/M1-W01-C03-green.json",
        RedEvidence, QualifiedEvidence);
    c["commands"] = p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C03",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    var ce = c["evidence"]!.AsArray();
    foreach (var evidencePath in new[] { RedEvidence, QualifiedEvidence })
        if (!ce.Any(n => (string?)n == evidencePath))
            ce.Add((JsonNode?)JsonValue.Create(evidencePath));
    t["status"] = "done";
    t["evidence"] = p.Json.StringArray(RedEvidence, QualifiedEvidence);
    foreach (var sub in t["subtasks"]!.AsArray().Select(n => n!.AsObject()))
    {
        sub["status"] = "done";
        var id = (string?)sub["id"];
        sub["evidence"] = p.Json.StringArray(id == Red ? RedEvidence : QualifiedEvidence);
    }
});
if (baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    p.ProjectPlan.TransitionNode(Green, "not-ready", "ready");
    p.ProjectPlan.ActivateReadyContinuation(Green);
    p.ProjectPlan.ConvergeNodeToDone(Green);
    p.ProjectPlan.TransitionNode(Verify, "not-ready", "ready");
    p.ProjectPlan.ActivateReadyContinuation(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach (var id in new[] { Red, Green, Verify, Task, Phase })
        p.ProjectPlan.RequireNodeState(id, "done");
}
RunRequired(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
return p.Complete();

static string ImportantFailure(string output)
{
    var marker = output.IndexOf("Error Message:", StringComparison.Ordinal);
    if (marker < 0)
        marker = output.IndexOf("error ", StringComparison.OrdinalIgnoreCase);
    var useful = marker >= 0 ? output[marker..] : output;
    return useful.Length > 3000 ? useful[..3000] : useful;
}

static (int Code, string Output) Run(string root, string fileName, params string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    if (!process.Start())
        throw new InvalidOperationException("Failed to start " + fileName);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException(fileName + " timed out.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode, output);
}

static void RunRequired(string root, string fileName, params string[] arguments)
{
    var result = Run(root, fileName, arguments);
    if (result.Code != 0)
        throw new InvalidOperationException(
            fileName + " " + string.Join(" ", arguments) +
            " exited " + result.Code + ": " + ImportantFailure(result.Output));
}
