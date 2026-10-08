using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string BinarySource = "lib/dw.quantities/ExactBinaryNumber.cs";
const string DecimalSource = "lib/dw.quantities/ExactDecimalFormatter.cs";
const string BinaryTarget = "src/projects/dw.quantities/ExactBinaryNumber.cs";
const string DecimalTarget = "src/projects/dw.quantities/ExactDecimalFormatter.cs";
const string BinaryHash = "cda9835b8a83f01239d344bd83b4144c7a6e9dcad59d9c2cc385103b76a2225d";
const string DecimalHash = "20499543d9635a291a54ff91763ece266176fa08bf67421d173caae5f59d45ce";
const string TestPath = "tests/projects/dw.quantities.tests/M1W01C02Tests.cs";
const string RedMarker = "M1-W01-C02-T2 RED: binary64 and decimal boundary APIs are absent";
const string RedEvidence = "docs/planning/evidence/M1-W01-C02-red.json";
const string GreenEvidence = "docs/planning/evidence/M1-W01-C02-qualified.json";
const string Task1 = "M1-W01-C02-T1";
const string Task2 = "M1-W01-C02-T2";

var payload = PayloadContext.Create();
var root = payload.RepositoryRoot;
var product = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = product["chunks"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["id"] == "M1-W01-C02");
var t1 = chunk["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Task1);
var t2 = chunk["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Task2);
string Substate(JsonObject task, string suffix) =>
    (string?)task["subtasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == (string?)task["id"] + "-" + suffix)["status"] ?? "<missing>";

var baseline = (string?)product["plan_version"] == "0.1.11" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)t1["status"] == "in_progress" &&
    Substate(t1, "R") == "done" && Substate(t1, "G") == "ready" && Substate(t1, "V") == "planned" &&
    (string?)t2["status"] == "planned" &&
    Substate(t2, "R") == "planned" && Substate(t2, "G") == "planned" && Substate(t2, "V") == "planned";
var target = (string?)product["plan_version"] == "0.1.12" &&
    (string?)chunk["status"] == "done" &&
    (string?)t1["status"] == "done" &&
    Substate(t1, "R") == "done" && Substate(t1, "G") == "done" && Substate(t1, "V") == "done" &&
    (string?)t2["status"] == "done" &&
    Substate(t2, "R") == "done" && Substate(t2, "G") == "done" && Substate(t2, "V") == "done";
if (!baseline && !target)
    throw new InvalidOperationException("RS013 product lifecycle is neither declared baseline nor complete target.");

var sourceManifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = sourceManifest["repositories"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["repository"] == "aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("AURA source baseline revision changed.");
var observedRoot = (string?)aura["observed_root"] ??
    throw new InvalidOperationException("Missing owner-approved AURA observed root.");
var entries = aura["files"]!.AsArray().Select(x => x!.AsObject()).ToArray();

string ExactSourceText(string relative, string expectedHash)
{
    var entry = entries.Single(x => (string?)x["path"] == relative);
    if ((string?)entry["sha256"] != expectedHash)
        throw new InvalidOperationException("Pinned source manifest changed for " + relative);
    var path = Path.Combine(observedRoot, relative.Replace('/', Path.DirectorySeparatorChar));
    var bytes = File.ReadAllBytes(path);
    var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if (actual != expectedHash)
        throw new InvalidOperationException("Authorized AURA source bytes drifted: " + relative);
    return new UTF8Encoding(false, true).GetString(bytes);
}

var binary = ExactSourceText(BinarySource, BinaryHash);
var decimalText = ExactSourceText(DecimalSource, DecimalHash);

// Keep a meaningful boundary RED before transferring either source. On target re-entry,
// the same test file is already GREEN, so no repeat RED is required or permitted.
payload.Files.ReplaceFromStaged("staged/" + TestPath, TestPath);
if (baseline)
{
    var red = Run(root, "dotnet", "test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c", "Release",
        "--filter", "FullyQualifiedName~M1W01C02Tests.BoundaryContractsRequireBothInstalledTypes",
        "--logger", "console;verbosity=normal");
    if (red.Code == 0 || !red.Output.Contains(RedMarker, StringComparison.Ordinal))
        throw new InvalidOperationException("T2 boundary RED did not fail for the expected absence: " + red.Output);
}

payload.Files.WriteComplete(BinaryTarget, binary);
payload.Files.WriteComplete(DecimalTarget, decimalText);

// The pin is a byte-level contract even after the SDK installs the complete source text.
foreach (var pair in new[] { (BinaryTarget, BinaryHash), (DecimalTarget, DecimalHash) })
{
    var path = Path.Combine(root, pair.Item1.Replace('/', Path.DirectorySeparatorChar));
    var actual = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    if (actual != pair.Item2)
        throw new InvalidOperationException("Transferred exact source bytes are not preserved: " + pair.Item1);
}

RunRequired(root, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--filter", "FullyQualifiedName~M1W01C02Tests",
    "--logger", "console;verbosity=normal");
RunRequired(root, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--no-restore",
    "--logger", "console;verbosity=minimal");
RunRequired(root, "pwsh", "-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1");

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C02-qualified",
    ["status"] = "qualified-with-explicit-scope",
    ["aura_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["exact_source_transfer"] = true,
    ["sources"] = payload.Json.Array(
        new JsonObject { ["from"] = BinarySource, ["to"] = BinaryTarget, ["sha256"] = BinaryHash },
        new JsonObject { ["from"] = DecimalSource, ["to"] = DecimalTarget, ["sha256"] = DecimalHash }),
    ["red"] = new JsonObject {
        ["T1"] = RedEvidence,
        ["T2"] = "BoundaryContractsRequireBothInstalledTypes failed for the documented absence before transfer."
    },
    ["test_project"] = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    ["focused_filter"] = "FullyQualifiedName~M1W01C02Tests",
    ["focused_passed"] = true,
    ["project_tests_passed"] = true,
    ["foundation_chain"] = "scripts/verify.ps1",
    ["foundation_passed"] = true,
    ["binary64_oracles"] = payload.Json.StringArray(
        "0.5 -> 1/2", "0.1 -> 3602879701896397/36028797018963968",
        "double.Epsilon -> 1/(2^1074)", "NaN/infinities rejected",
        "signed negative zero maps to rational zero"),
    ["decimal_oracles"] = payload.Json.StringArray(
        "public DecimalRoundingMode cases report one-digit +/-1.25 tie results",
        "two-decimal display preserves +/-1.25",
        "100 decimal places supported; -1 and 101 refused",
        "display does not mutate ExactRational"),
    ["limits"] = payload.Json.StringArray(
        "DecimalRoundingMode semantics beyond the tested exact tie results are not inferred.",
        "Negative zero's IEEE sign bit is not representable in canonical ExactRational.",
        "No claim is made that all direct decimal input magnitudes or allocations are globally budgeted.",
        "No AURA, Decision, MCDM, or brainstorming repository is mutated.")
};
payload.Files.WriteComplete(GreenEvidence,
    evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

payload.Json.EditObject("docs/planning/backlog.json", json =>
{
    var c = json["chunks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == "M1-W01-C02");
    var tasks = c["tasks"]!.AsArray().Select(x => x!.AsObject()).ToArray();
    if (tasks.Length != 2)
        throw new InvalidOperationException("Unexpected C02 task topology.");
    var first = tasks.Single(x => (string?)x["id"] == Task1);
    var second = tasks.Single(x => (string?)x["id"] == Task2);
    json["plan_version"] = "0.1.12";
    c["status"] = "done";
    c["refinement"] = "RS012 pins source and establishes T1 RED; RS013 transfers exact bytes, observes T2 missing-boundary RED before transfer, then qualifies binary64 exactness, non-finite rejection, subnormal/signed zero behavior, decimal modes and precision bounds with independent tests.";
    c["files"] = payload.Json.StringArray(BinaryTarget, DecimalTarget, TestPath, RedEvidence, GreenEvidence);
    c["commands"] = payload.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C02Tests",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    var evidenceList = c["evidence"]!.AsArray();
    if (!evidenceList.Any(x => (string?)x == GreenEvidence))
        evidenceList.Add((JsonNode?)JsonValue.Create(GreenEvidence));
    foreach (var task in tasks)
    {
        task["status"] = "done";
        task["evidence"] = payload.Json.StringArray(RedEvidence, GreenEvidence);
        foreach (var sub in task["subtasks"]!.AsArray().Select(x => x!.AsObject()))
        {
            sub["status"] = "done";
            if ((string?)sub["id"] == Task1 + "-R")
                continue;
            sub["evidence"] = payload.Json.StringArray(GreenEvidence);
        }
    }
});

if (baseline)
{
    payload.ProjectPlan.ActivateReadyContinuation(Task1 + "-G");
    payload.ProjectPlan.ConvergeNodeToDone(Task1 + "-G");
    payload.ProjectPlan.TransitionNode(Task1 + "-V", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation(Task1 + "-V");
    payload.ProjectPlan.ConvergeNodeToDone(Task1 + "-V");
    payload.ProjectPlan.ConvergeNodeToDone(Task1);
    payload.ProjectPlan.TransitionNode(Task2, "not-ready", "ready");
    payload.ProjectPlan.TransitionNode(Task2 + "-R", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation(Task2 + "-R");
    payload.ProjectPlan.ConvergeNodeToDone(Task2 + "-R");
    payload.ProjectPlan.TransitionNode(Task2 + "-G", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation(Task2 + "-G");
    payload.ProjectPlan.ConvergeNodeToDone(Task2 + "-G");
    payload.ProjectPlan.TransitionNode(Task2 + "-V", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation(Task2 + "-V");
    payload.ProjectPlan.ConvergeNodeToDone(Task2 + "-V");
    payload.ProjectPlan.ConvergeNodeToDone(Task2);
    payload.ProjectPlan.ConvergeNodeToDone("M1-W01-C02");
}
else
{
    foreach (var id in new[] {
        Task1 + "-G", Task1 + "-V", Task1,
        Task2 + "-R", Task2 + "-G", Task2 + "-V", Task2, "M1-W01-C02" })
        payload.ProjectPlan.RequireNodeState(id, "done");
}

RunRequired(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
return payload.Complete();

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
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start())
        throw new InvalidOperationException("Unable to start " + executable);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Command timed out: " + executable);
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode, output);
}

static void RunRequired(string root, string executable, params string[] args)
{
    var result = Run(root, executable, args);
    if (result.Code != 0)
        throw new InvalidOperationException(executable + " failed with exit code " +
            result.Code + ": " + result.Output);
}
