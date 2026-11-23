#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Generator = "docs/discovery/qualification/GenerateM8MathSkill.cs";
const string Source = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
const string Bundle = ".agents/skills/dw-tools-math/references/manifest.json";
var files = new[] { Generator, Source, Skill, Bundle };
var before = files.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var work = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs301-generator-red", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(work);
foreach (var path in files)
{
    var dest = Path.Combine(work, path.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(path, dest);
    Need(Hash(path) == Hash(dest), "Isolated source copy differs: " + path);
}
ProcessResult Check() => Run(work, "run", "--file", Generator, "--", "--check");
var clean = Check();
Need(clean.ExitCode == 0 &&
    clean.Output.Contains("MATH-RS201-SKILL-MANIFEST|capabilities=4|brokerReady=0|mode=--check",
        StringComparison.Ordinal), "Clean source-qualified generated skill refused: " + clean.Output);

var refused = 0;
void Tamper(string path, string label, string expected)
{
    var target = Path.Combine(work, path.Replace('/', Path.DirectorySeparatorChar));
    var original = File.ReadAllBytes(target);
    try
    {
        File.AppendAllText(target, " ", new UTF8Encoding(false));
        var result = Check();
        Need(result.ExitCode != 0 && result.Output.Contains(expected, StringComparison.Ordinal),
            "Generator did not refuse isolated " + label + ": " + result.Output);
        refused++;
    }
    finally { File.WriteAllBytes(target, original); }
    Need(Check().ExitCode == 0, "Clean skill did not recover after " + label);
}
Tamper(Skill, "skill-text-drift", "Generated skill is absent or stale");
Tamper(Bundle, "bundled-manifest-drift", "Bundled manifest is missing or differs");

var sourcePath = Path.Combine(work, Source.Replace('/', Path.DirectorySeparatorChar));
var sourceOriginal = File.ReadAllBytes(sourcePath);
try
{
    var obj = JsonNode.Parse(sourceOriginal)?.AsObject() ??
        throw new InvalidDataException("RS301: Invalid copied source manifest.");
    var capability = obj["capabilities"]?.AsArray()[0]?.AsObject() ??
        throw new InvalidDataException("RS301: Missing selected capability.");
    capability["execution"]!.AsObject()["authorization"] = "granted";
    File.WriteAllText(sourcePath, obj.ToJsonString() + "\n", new UTF8Encoding(false));
    var result = Check();
    Need(result.ExitCode != 0 &&
        result.Output.Contains("Selection or authorization drift", StringComparison.Ordinal),
        "Generator admitted forged execution authorization: " + result.Output);
    refused++;
}
finally { File.WriteAllBytes(sourcePath, sourceOriginal); }
Need(Check().ExitCode == 0 && refused == 3,
    "Isolated RED matrix or clean restoration failed.");
foreach (var file in files)
    Need(Hash(file) == before[file], "Canonical tracked input was modified: " + file);
Console.WriteLine("MATH-RS301-SKILL-RED|clean=1|skillDriftRefused=1|" +
    "manifestDriftRefused=1|executionGrantRefused=1|canonicalUnchanged=true|" +
    "brokerReady=0|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS301: " + reason);
}
static ProcessResult Run(string workingDirectory, params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workingDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in arguments) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS301: Cannot start generator.");
    var output = process.StandardOutput.ReadToEndAsync();
    var error = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS301: Generator subprocess timed out.");
    }
    return new ProcessResult(process.ExitCode,
        output.GetAwaiter().GetResult() + "\n" + error.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
