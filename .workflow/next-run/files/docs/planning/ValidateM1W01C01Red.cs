#:property PublishAot=false
#:property NuGetAudit=false

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

const string SourcePath = "lib/dw.quantities/ExactRational.cs";
const string SourceSha256 = "3c8d3beaa3b04884ca8424b5bbc6ee5f2d87b4adf1f6e329a7047bd974b1405a";
const string RedMarker = "M1-W01-C01 RED: Dw.Quantities.ExactRational is absent";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string EvidencePath = "docs/planning/evidence/M1-W01-C01-red.json";

try
{
    var mode = args.Length == 0 ? "--check" : args.Single();
    if (mode is not ("--capture" or "--check"))
        throw new InvalidOperationException("Usage: dotnet run --file docs/planning/ValidateM1W01C01Red.cs -- [--capture|--check]");

    var root = Directory.GetCurrentDirectory();
    var baseline = Load(Path.Combine(root, "docs", "planning", "source-baseline.json"));
    var backlog = Load(Path.Combine(root, "docs", "planning", "backlog.json"));

    var aura = A(baseline, "repositories").Select(x => x!.AsObject()).Single(x => S(x, "repository") == "aura");
    Require(S(aura, "head") == "82a6b435a387a7e116b47a6b2c433ae9e067bf21", "Pinned AURA revision changed.");
    var source = A(aura, "files").Select(x => x!.AsObject()).Single(x => S(x, "path") == SourcePath);
    Require(S(source, "sha256") == SourceSha256, "Pinned ExactRational SHA-256 changed.");

    Require(!File.Exists(Path.Combine(root, "src", "projects", "dw.quantities", "ExactRational.cs")),
        "RED invalid: ExactRational.cs is already installed.");
    Require(File.Exists(Path.Combine(root, TestProject.Replace('/', Path.DirectorySeparatorChar))),
        "Missing RED test project.");
    Require(File.Exists(Path.Combine(root, "tests", "projects", "dw.quantities.tests", "M1W01C01Tests.cs")),
        "Missing RED contract test.");

    var solution = File.ReadAllText(Path.Combine(root, "dw.tools.math.slnx"));
    Require(!solution.Contains("src/projects/dw.quantities/", StringComparison.Ordinal),
        "RED project must remain outside the main solution until GREEN.");

    var restore = Run(root, "dotnet", ["restore", TestProject, "--locked-mode"], 180_000);
    Require(restore.ExitCode == 0, "RED harness restore failed:\n" + restore.Combined);

    var test = Run(root, "dotnet",
        ["test", TestProject, "-c", "Release", "--no-restore", "--filter", "FullyQualifiedName~M1W01C01Tests", "--logger", "console;verbosity=minimal"],
        180_000);
    Require(test.ExitCode != 0, "RED invalid: targeted ExactRational test unexpectedly passed.");
    Require(test.Combined.Contains(RedMarker, StringComparison.Ordinal),
        "RED failed for an unexpected reason; missing marker: " + RedMarker);

    var chunk = FindById(backlog, "chunks", "M1-W01-C01");
    var task = A(chunk, "tasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T1");
    var red = A(task, "subtasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T1-R");
    var green = A(task, "subtasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T1-G");
    Require(S(chunk, "status") == "in_progress", "M1-W01-C01 must be in_progress after RED.");
    Require(S(task, "status") == "in_progress", "M1-W01-C01-T1 must be in_progress after RED.");
    Require(S(red, "status") == "done", "M1-W01-C01-T1-R must be done.");
    Require(S(green, "status") == "ready", "M1-W01-C01-T1-G must be ready.");

    var evidence = new JsonObject
    {
        ["schema_version"] = 1,
        ["id"] = "M1-W01-C01-red",
        ["status"] = "expected-red-observed",
        ["source_repository"] = "AURA",
        ["source_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
        ["source_path"] = SourcePath,
        ["expected_source_sha256"] = SourceSha256,
        ["math_source_materialized"] = false,
        ["test_project"] = TestProject,
        ["test_filter"] = "FullyQualifiedName~M1W01C01Tests",
        ["observed_failure_marker"] = RedMarker,
        ["test_exit_code_nonzero"] = true,
        ["main_solution_unchanged"] = true,
        ["green_acceptance_cases"] = new JsonArray(
            "1/3 + 1/6 = 1/2",
            "-1 1/2 = -3/2",
            "zero denominator is rejected",
            "2/-4 canonicalizes to -1/2"),
        ["limits"] = new JsonArray(
            "This RED proves the canonical ExactRational type is not implemented in Math yet.",
            "It does not claim access to or parity with the authorized source bytes.",
            "GREEN must materialize only source bytes whose SHA-256 matches the pinned manifest or record an explicit reconciliation.")
    };

    var evidenceText = evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n";
    var evidenceFullPath = Path.Combine(root, EvidencePath.Replace('/', Path.DirectorySeparatorChar));
    if (mode == "--capture")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(evidenceFullPath)!);
        File.WriteAllText(evidenceFullPath, evidenceText, new System.Text.UTF8Encoding(false));
    }
    else
    {
        Require(File.Exists(evidenceFullPath), "Missing durable RED evidence.");
        Require(File.ReadAllText(evidenceFullPath) == evidenceText, "RED evidence is stale or does not match the observed contract.");
    }

    Console.WriteLine("M1-W01-C01 RED valid: ExactRational absent for the expected reason; GREEN source hash pinned");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("M1-W01-C01 RED invalid: " + ex.Message);
    return 1;
}

static RunResult Run(string root, string fileName, string[] arguments, int timeoutMs)
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
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    if (!process.Start()) throw new InvalidOperationException("Failed to start " + fileName + ".");
    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeoutMs))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException(fileName + " exceeded timeout.");
    }
    return new RunResult(process.ExitCode, stdoutTask.GetAwaiter().GetResult(), stderrTask.GetAwaiter().GetResult());
}

static JsonObject Load(string path) =>
    JsonNode.Parse(File.ReadAllText(path))?.AsObject()
    ?? throw new InvalidOperationException("Invalid or empty JSON: " + path);

static JsonArray A(JsonObject o, string key) =>
    o[key] as JsonArray ?? throw new InvalidOperationException("Missing array " + key);

static string S(JsonObject o, string key) =>
    o[key]?.GetValue<string>() is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException("Missing string " + key);

static JsonObject FindById(JsonObject root, string arrayName, string id) =>
    A(root, arrayName).Select(x => x!.AsObject()).Single(x => S(x, "id") == id);

static void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}

sealed record RunResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string Combined => StandardOutput + Environment.NewLine + StandardError;
}
