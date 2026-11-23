#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Generator = "docs/discovery/qualification/GenerateM8MathSkill.cs";
const string StrictGate = "docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs";
const string Source = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
const string Bundle = ".agents/skills/dw-tools-math/references/manifest.json";
var originals = new[] { Generator, StrictGate, Source, Skill, Bundle };
var originalHashes = originals.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var parent = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs303-generator-green", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(parent);
string[] roots = [Path.Combine(parent, "consumer-a"), Path.Combine(parent, "consumer-b")];
foreach (var root in roots)
{
    foreach (var name in new[] { Generator, StrictGate, Source })
    {
        var to = Path.Combine(root, name.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(to)!);
        File.Copy(name, to);
    }
}
ProcessResult Execute(string root, string mode) =>
    Run(root, "run", "--file", Generator, "--", mode);
string At(string root, string path) =>
    Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
void AssertClean(string root)
{
    var check = Execute(root, "--check");
    Need(check.ExitCode == 0 &&
        check.Output.Contains("MATH-RS201-SKILL-MANIFEST|capabilities=4|brokerReady=0|mode=--check",
            StringComparison.Ordinal),
        "Independent generator did not accept clean output: " + check.Output);
}
foreach (var root in roots)
{
    var write = Execute(root, "--write");
    Need(write.ExitCode == 0 &&
        write.Output.Contains("MATH-RS201-SKILL-MANIFEST|capabilities=4|brokerReady=0|mode=--write",
            StringComparison.Ordinal),
        "Independent deterministic generation failed: " + write.Output);
    AssertClean(root);
}
Need(Hash(At(roots[0], Skill)) == Hash(At(roots[1], Skill)) &&
    Hash(At(roots[0], Bundle)) == Hash(At(roots[1], Bundle)),
    "Independent skill or manifest bytes differ.");
Need(Hash(At(roots[0], Skill)) == originalHashes[Skill] &&
    Hash(At(roots[0], Bundle)) == originalHashes[Bundle],
    "Fresh generator output differs from existing canonical skill or manifest.");

var targetRoot = roots[0];
var inputPath = At(targetRoot, Source);
var sourceBytes = File.ReadAllBytes(inputPath);
var skillHash = Hash(At(targetRoot, Skill));
var bundledHash = Hash(At(targetRoot, Bundle));
var rejected = 0;
void Reject(string label, Action<JsonObject> alter, string diagnostic)
{
    var source = JsonNode.Parse(sourceBytes)?.AsObject() ??
        throw new InvalidDataException("RS303: Invalid copied source manifest.");
    var first = source["capabilities"]?.AsArray()[0]?.AsObject() ??
        throw new InvalidDataException("RS303: Missing first selected capability.");
    alter(first);
    try
    {
        File.WriteAllText(inputPath, source.ToJsonString() + "\n",
            new UTF8Encoding(false));
        var result = Execute(targetRoot, "--write");
        Need(result.ExitCode != 0 &&
            result.Output.Contains("RS287: " + diagnostic, StringComparison.Ordinal),
            "Invalid source metadata was written or misclassified: " + label + " " + result.Output);
        Need(Hash(At(targetRoot, Skill)) == skillHash &&
            Hash(At(targetRoot, Bundle)) == bundledHash,
            "Refused generation mutated existing output: " + label);
        rejected++;
    }
    finally
    {
        File.WriteAllBytes(inputPath, sourceBytes);
    }
    AssertClean(targetRoot);
}
Reject("fingerprint-forgery", first =>
    first["binding"]!.AsObject()["fingerprint"] = new string('0', 64),
    "Canonical CLR SHA-256 fingerprint differs");
Reject("parameter-type-drift", first =>
    first["parameters"]!.AsArray()[0]!.AsObject()["type"] = "System.String",
    "Canonical CLR signature does not match declared fields");
Need(rejected == 2, "Two new source-side refusals are required.");
foreach (var original in originals)
    Need(Hash(original) == originalHashes[original],
        "Canonical tracked source changed: " + original);
Console.WriteLine("MATH-RS303-SKILL-GREEN|independentWrites=2|byteIdentical=true|" +
    "fingerprintRefused=1|signatureRefused=1|outputPreserved=2|" +
    "canonicalUnchanged=true|brokerReady=0|PASS");

static string Hash(string path)
{
    using var file = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(file));
}
static void Need(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS303: " + reason);
}
static ProcessResult Run(string cwd, params string[] args)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = cwd,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in args) start.ArgumentList.Add(arg);
    using var p = Process.Start(start) ??
        throw new InvalidOperationException("RS303: Cannot start generator.");
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    if (!p.WaitForExit(120000))
    {
        p.Kill(entireProcessTree: true);
        throw new TimeoutException("RS303: Generator timed out.");
    }
    return new ProcessResult(p.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
