using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W01-C03";
const string T1 = "M1-W01-C03-T1";
const string T2 = "M1-W01-C03-T2";
const string TestPath = "tests/projects/dw.quantities.tests/M1W01C03Tests.cs";
const string RedEvidence = "docs/planning/evidence/M1-W01-C03-red.json";
const string GreenEvidence = "docs/planning/evidence/M1-W01-C03-green.json";

var sources = new (string Original, string Target, string Hash)[]
{
    ("lib/dw.quantities/DimensionVector.cs", "src/projects/dw.quantities/DimensionVector.cs", "2c2ebabbb800ff151309dbc4f2cbb611091f687f30dcc5fb742e99339406e5b9"),
    ("lib/dw.quantities/Quantity.cs", "src/projects/dw.quantities/Quantity.cs", "f9121bedc85e810a958f6d2f833652a59f5f7b66a81a8445e9d0c6cec7104e6f"),
    ("lib/dw.quantities/UnitDefinition.cs", "src/projects/dw.quantities/UnitDefinition.cs", "a8eaf48386ec10d6586a8f7be80765fb6e21c6a18acc5309d21a6d544dba676f")
};

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = backlog["chunks"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["id"] == Phase);
var task = chunk["tasks"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["id"] == T1);
var next = chunk["tasks"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["id"] == T2);
string GetSubtask(JsonObject parent, string suffix) =>
    (string?)parent["subtasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == (string?)parent["id"] + "-" + suffix)["status"] ?? "<missing>";

var baseline = (string?)backlog["plan_version"] == "0.1.13" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)task["status"] == "in_progress" &&
    GetSubtask(task, "R") == "done" && GetSubtask(task, "G") == "ready" &&
    GetSubtask(task, "V") == "planned" &&
    (string?)next["status"] == "planned" && GetSubtask(next, "R") == "planned";
var target = (string?)backlog["plan_version"] == "0.1.14" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)task["status"] == "done" &&
    GetSubtask(task, "R") == "done" && GetSubtask(task, "G") == "done" &&
    GetSubtask(task, "V") == "done" &&
    (string?)next["status"] == "ready" && GetSubtask(next, "R") == "ready";
if (!baseline && !target)
    throw new InvalidOperationException("RS015 product lifecycle is neither baseline nor target.");

var manifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = manifest["repositories"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["repository"] == "aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("AURA snapshot revision is not approved.");
var observedRoot = (string?)aura["observed_root"]
    ?? throw new InvalidOperationException("AURA observed root is missing.");
var entries = aura["files"]!.AsArray().Select(x => x!.AsObject()).ToArray();

foreach (var source in sources)
{
    var entry = entries.Single(x => (string?)x["path"] == source.Original);
    if ((string?)entry["sha256"] != source.Hash)
        throw new InvalidOperationException("Pinned manifest changed: " + source.Original);
    var external = Path.Combine(observedRoot, source.Original.Replace('/', Path.DirectorySeparatorChar));
    var bytes = File.ReadAllBytes(external);
    if (Hash(bytes) != source.Hash)
        throw new InvalidOperationException("AURA source bytes drifted: " + source.Original);

    var destination = Path.Combine(root, source.Target.Replace('/', Path.DirectorySeparatorChar));
    if (File.Exists(destination) && Hash(File.ReadAllBytes(destination)) != source.Hash)
        throw new InvalidOperationException("Math source destination has unapproved third-state bytes: " + source.Target);
    p.Files.WriteComplete(source.Target, new UTF8Encoding(false, true).GetString(bytes));
    if (Hash(File.ReadAllBytes(destination)) != source.Hash)
        throw new InvalidOperationException("Math source transfer changed the pinned bytes: " + source.Target);
}

p.Files.ReplaceFromStaged("staged/" + TestPath, TestPath);

// Compile separately so a source/analyzer failure is reported by the build before test discovery.
RunRequired(root, "dotnet", "build",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--verbosity", "normal");
// Isolate each functional oracle after the first aborted test run. This prevents
// a failing/crashing test host from hiding which contract needs correction.
foreach (var testName in new[]
{
    "ThreeAuthorizedPublicTypesAreInstalled",
    "AreaOfThreeMetresByFourMetresIsTwelveSquareMetres",
    "TenKilometresPerThirtyMinutesIsExactlyFiftyNinthsMetresPerSecond",
    "InformationDimensionRemainsIndependent",
    "AdditionOfMassAndTimeIsRejectedAtThePublicQuantityBoundary",
    "FahrenheitAndCelsiusAbsoluteConversionsAreExact",
    "NineFahrenheitDegreesOfIntervalCorrespondToFiveKelvin"
})
{
    Console.WriteLine("RS015 focused oracle: " + testName);
    RunRequired(root, "dotnet", "test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c", "Release", "--no-build", "--no-restore",
        "--filter", "FullyQualifiedName~M1W01C03Tests." + testName,
        "--logger", "console;verbosity=normal");
}
RunRequired(root, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--no-build", "--no-restore",
    "--logger", "console;verbosity=minimal");
RunRequired(root, "pwsh", "-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1");

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C03-green",
    ["status"] = "functional-green-verified-boundary-pending",
    ["aura_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["sources"] = p.Json.Array(sources.Select(s => (JsonNode)new JsonObject
    {
        ["source_path"] = s.Original, ["target_path"] = s.Target, ["sha256"] = s.Hash
    }).ToArray()),
    ["actual_api"] = p.Json.StringArray(
        "DimensionVector LengthDimension, MassDimension, TimeDimension, TemperatureDimension, InformationDimension",
        "Quantity(ExactRational, DimensionVector), Multiply, Divide, TryAdd",
        "UnitDefinition(ToBase, FromBase, ScaleToBase, OffsetToBase) with reflected public constructor"),
    ["tested_oracles"] = p.Json.StringArray(
        "3 m multiplied by 4 m = 12 square metres with exact squared-length dimension",
        "10 km divided by 30 minutes = 50/9 m/s",
        "Information dimension remains independent of scalar and length",
        "Adding mass to time is rejected at Quantity.TryAdd",
        "32 degF and 0 degC convert to the same exact Kelvin absolute",
        "A 9 degF interval has scale-only magnitude 5 K; absolute ToBase applies offset"),
    ["focused_filter"] = "FullyQualifiedName~M1W01C03Tests",
    ["focused_tests_passed"] = true,
    ["dw_quantities_tests_passed"] = true,
    ["foundation_chain_passed"] = true,
    ["source_aura_modified"] = false,
    ["limits"] = p.Json.StringArray(
        "T2 exponent overflow and direct absolute-versus-interval misuse remain unqualified.",
        "Scale-only interval magnitude is not proof of a dedicated temperature interval API.",
        "No AURA or sibling repository modification or external adoption is claimed.")
};
p.Files.WriteComplete(GreenEvidence, evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

p.Json.EditObject("docs/planning/backlog.json", product =>
{
    var c = product["chunks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Phase);
    var tasks = c["tasks"]!.AsArray().Select(x => x!.AsObject()).ToArray();
    var first = tasks.Single(x => (string?)x["id"] == T1);
    var second = tasks.Single(x => (string?)x["id"] == T2);
    product["plan_version"] = "0.1.14";
    c["status"] = "in_progress";
    c["refinement"] = "RS014 captured pinned source declarations and missing-type RED; RS015 transfers the exact approved three-source surface, qualifies exact dimensions, quantity operations and affine temperature conversions, and leaves exponent-overflow and absolute/interval boundary policy for T2.";
    c["files"] = p.Json.StringArray(
        sources[0].Target, sources[1].Target, sources[2].Target, TestPath, RedEvidence, GreenEvidence);
    c["commands"] = p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C03Tests",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    var ce = c["evidence"]!.AsArray();
    if (!ce.Any(x => (string?)x == GreenEvidence))
        ce.Add((JsonNode?)JsonValue.Create(GreenEvidence));
    first["status"] = "done";
    first["evidence"] = p.Json.StringArray(RedEvidence, GreenEvidence);
    foreach (var sub in first["subtasks"]!.AsArray().Select(x => x!.AsObject()))
    {
        sub["status"] = "done";
        if ((string?)sub["id"] != T1 + "-R")
            sub["evidence"] = p.Json.StringArray(GreenEvidence);
    }
    second["status"] = "ready";
    var nextRed = second["subtasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == T2 + "-R");
    nextRed["status"] = "ready";
});
if (baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(T1 + "-G");
    p.ProjectPlan.ConvergeNodeToDone(T1 + "-G");
    p.ProjectPlan.TransitionNode(T1 + "-V", "not-ready", "ready");
    p.ProjectPlan.ActivateReadyContinuation(T1 + "-V");
    p.ProjectPlan.ConvergeNodeToDone(T1 + "-V");
    p.ProjectPlan.ConvergeNodeToDone(T1);
    p.ProjectPlan.TransitionNode(T2, "not-ready", "ready");
    p.ProjectPlan.TransitionNode(T2 + "-R", "not-ready", "ready");
}
else
{
    foreach (var id in new[] { T1 + "-G", T1 + "-V", T1 })
        p.ProjectPlan.RequireNodeState(id, "done");
    foreach (var id in new[] { T2, T2 + "-R" })
        p.ProjectPlan.RequireNodeState(id, "ready");
}
RunRequired(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
return p.Complete();

static string Hash(byte[] bytes) =>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static void RunRequired(string root, string executable, params string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable, WorkingDirectory = root, UseShellExecute = false,
            RedirectStandardOutput = true, RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start())
        throw new InvalidOperationException("Failed to start " + executable);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException(executable + " timed out.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (process.ExitCode != 0)
        throw new InvalidOperationException(
            executable + " " + string.Join(" ", args) +
            " exited " + process.ExitCode + ": " + output);
}
