#:property PublishAot=false
#:property NuGetAudit=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

const string SourceRelativePath = "lib/dw.quantities/ExactRational.cs";
const string TargetRelativePath = "src/projects/dw.quantities/ExactRational.cs";
const string ExpectedSourceSha256 = "3c8d3beaa3b04884ca8424b5bbc6ee5f2d87b4adf1f6e329a7047bd974b1405a";
const string EvidenceRelativePath = "docs/planning/evidence/M1-W01-C01-green.json";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";

try
{
    var mode = args.Length == 0 ? "--check" : args.Single();
    if (mode is not ("--capture" or "--check"))
        throw new InvalidOperationException("Usage: dotnet run --file docs/planning/ValidateM1W01C01Green.cs -- [--capture|--check]");

    var root = Directory.GetCurrentDirectory();
    var baseline = Load(Path.Combine(root, "docs", "planning", "source-baseline.json"));
    var backlog = Load(Path.Combine(root, "docs", "planning", "backlog.json"));

    var aura = A(baseline, "repositories").Select(x => x!.AsObject()).Single(x => S(x, "repository") == "aura");
    Require(S(aura, "head") == "82a6b435a387a7e116b47a6b2c433ae9e067bf21", "Pinned AURA revision changed.");
    Require(S(aura, "observed_root").Length > 0, "AURA observed root is missing.");
    var source = A(aura, "files").Select(x => x!.AsObject()).Single(x => S(x, "path") == SourceRelativePath);
    Require(S(source, "sha256") == ExpectedSourceSha256, "Pinned ExactRational SHA-256 changed.");

    var targetPath = Path.Combine(root, TargetRelativePath.Replace('/', Path.DirectorySeparatorChar));
    Require(File.Exists(targetPath), "ExactRational.cs was not materialized.");
    Require(Hash(File.ReadAllBytes(targetPath)) == ExpectedSourceSha256, "Materialized ExactRational.cs does not match the authorized source bytes.");

    var solution = File.ReadAllText(Path.Combine(root, "dw.tools.math.slnx"));
    Require(solution.Contains("src/projects/dw.quantities/dw.quantities.csproj", StringComparison.Ordinal), "dw.quantities project is missing from the main solution.");
    Require(solution.Contains("tests/projects/dw.quantities.tests/dw.quantities.tests.csproj", StringComparison.Ordinal), "dw.quantities tests are missing from the main solution.");

    var restore = Run(root, "dotnet", ["restore", "dw.tools.math.slnx", "--locked-mode"], 180_000);
    Require(restore.ExitCode == 0, "Solution restore failed:\n" + restore.Combined);

    var build = Run(root, "dotnet", ["build", "dw.tools.math.slnx", "-c", "Release", "--no-restore"], 180_000);
    Require(build.ExitCode == 0, "Solution build failed:\n" + build.Combined);

    var tests = Run(root, "dotnet",
        ["test", TestProject, "-c", "Release", "--no-restore", "--no-build", "--filter", "FullyQualifiedName~M1W01C01Tests", "--logger", "console;verbosity=minimal"],
        180_000);
    Require(tests.ExitCode == 0, "ExactRational GREEN tests failed:\n" + tests.Combined);

    var foundation = Run(root, "pwsh", ["-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1"], 360_000);
    Require(foundation.ExitCode == 0, "Foundation regression chain failed:\n" + foundation.Combined);

    var chunk = FindById(backlog, "chunks", "M1-W01-C01");
    var task = A(chunk, "tasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T1");
    var subtasks = A(task, "subtasks").Select(x => x!.AsObject()).ToDictionary(x => S(x, "id"), StringComparer.Ordinal);
    Require(S(chunk, "status") == "in_progress", "M1-W01-C01 must remain in_progress.");
    Require(S(task, "status") == "done", "M1-W01-C01-T1 must be done after GREEN verification.");
    Require(S(subtasks["M1-W01-C01-T1-R"], "status") == "done", "RED must remain done.");
    Require(S(subtasks["M1-W01-C01-T1-G"], "status") == "done", "GREEN must be done.");
    Require(S(subtasks["M1-W01-C01-T1-V"], "status") == "done", "Verify/document must be done.");

    var t2 = A(chunk, "tasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T2");
    var t2Red = A(t2, "subtasks").Select(x => x!.AsObject()).Single(x => S(x, "id") == "M1-W01-C01-T2-R");
    Require(S(t2, "status") == "ready", "M1-W01-C01-T2 must be ready.");
    Require(S(t2Red, "status") == "ready", "M1-W01-C01-T2-R must be ready.");

    var evidence = new JsonObject
    {
        ["schema_version"] = 1,
        ["id"] = "M1-W01-C01-green",
        ["status"] = "green-and-verification-qualified",
        ["source_repository"] = "AURA",
        ["source_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
        ["source_path"] = SourceRelativePath,
        ["target_path"] = TargetRelativePath,
        ["source_sha256"] = ExpectedSourceSha256,
        ["target_sha256"] = ExpectedSourceSha256,
        ["source_bytes_materialized_exactly"] = true,
        ["main_solution_integrated"] = true,
        ["targeted_tests_passed"] = true,
        ["foundation_regression_passed"] = true,
        ["behavioral_oracle"] = new JsonArray(
            "1/3 + 1/6 = 1/2",
            "-1 1/2 = -3/2 through a supported mixed-number parse/construction path",
            "zero denominator is rejected",
            "2/-4 canonicalizes to -1/2"),
        ["limits"] = new JsonArray(
            "Only ExactRational.cs is transferred in this GREEN; ExactBinaryNumber, ExactDecimalFormatter, quantities, units, and tolerances remain outside this slice.",
            "The independent test uses public reflection so it does not assume an unpublished compile-time signature.",
            "AURA is read only; no sibling repository mutation occurs.")
    };

    var evidenceText = evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n";
    var evidencePath = Path.Combine(root, EvidenceRelativePath.Replace('/', Path.DirectorySeparatorChar));
    if (mode == "--capture")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(evidencePath)!);
        File.WriteAllText(evidencePath, evidenceText, new System.Text.UTF8Encoding(false));
    }
    else
    {
        Require(File.Exists(evidencePath), "Missing durable GREEN evidence.");
        Require(File.ReadAllText(evidencePath) == evidenceText, "GREEN evidence is stale.");
    }

    Console.WriteLine("M1-W01-C01 GREEN valid: authorized ExactRational bytes installed; accepted behaviors qualified; M0 regression passed");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("M1-W01-C01 GREEN invalid: " + ex.Message);
    return 1;
}

static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

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
