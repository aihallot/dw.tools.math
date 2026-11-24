#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Attester = "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs";
const string Verifier = "verify-export.cs";
const string Manifest = ".agents/skills/dw-tools-math/references/manifest.json";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
const string Receipt = "portable-attestation.json";
var source = new[] { Attester, Skill, "docs/discovery/qualification/m8-discovery-manifest.json" };
var originalSha = source.ToDictionary(x => x, Sha, StringComparer.Ordinal);
var repo = Path.GetFullPath(".");
var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs309-delivery-green", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
var a = Path.Combine(root, "export-a");
var b = Path.Combine(root, "export-b");
var copy = Path.Combine(root, "copied-consumer");
var host = Path.Combine(root, "verifier-host");
ProcessResult Export(string dest) => Run(repo, "run", "--file", Attester, "--", "--export", dest);
foreach (var destination in new[] { a, b })
{
    var result = Export(destination);
    Need(result.ExitCode == 0 &&
        result.Output.Contains("MATH-RS283-PORTABLE-ATTESTED|files=13|receipt=unsigned|",
            StringComparison.Ordinal) &&
        result.Output.Contains("brokerReady=false|PASS", StringComparison.Ordinal),
        "Fresh export refused: " + result.Output);
}
string[] List(string path) => Directory.GetFiles(path, "*", SearchOption.AllDirectories)
    .Select(x => Path.GetRelativePath(path, x).Replace('\\', '/'))
    .OrderBy(x => x, StringComparer.Ordinal).ToArray();
var names = List(a);
Need(names.Length == 14 && names.SequenceEqual(List(b), StringComparer.Ordinal),
    "Independent portable exports differ in file membership.");
foreach (var relative in names)
{
    var left = Path.Combine(a, relative.Replace('/', Path.DirectorySeparatorChar));
    var right = Path.Combine(b, relative.Replace('/', Path.DirectorySeparatorChar));
    Need(Sha(left) == Sha(right), "Independent exported byte drift: " + relative);
}
foreach (var destination in new[] { a, b })
{
    using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(destination, Receipt)));
    var record = receipt.RootElement;
    Need(record.GetProperty("signatureStatus").GetString() == "unsigned" &&
        record.GetProperty("authorization").GetString() == "runtime-only" &&
        record.GetProperty("files").GetArrayLength() == 13,
        "Portable delivery receipt falsely asserts origin or authority.");
    var caps = Path.Combine(destination, Manifest.Replace('/', Path.DirectorySeparatorChar));
    using var doc = JsonDocument.Parse(File.ReadAllText(caps));
    Need(doc.RootElement.GetProperty("capabilities").GetArrayLength() == 4 &&
        doc.RootElement.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        doc.RootElement.GetProperty("capabilities").EnumerateArray().All(x =>
            x.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only"),
        "Portable capability selection granted unqualified broker authority.");
}
Directory.CreateDirectory(host);
File.Copy(Path.Combine(a, Verifier), Path.Combine(host, Verifier));
File.Copy(Path.Combine(a, "global.json"), Path.Combine(host, "global.json"));
ProcessResult Verify(string directory) =>
    Run(host, "run", "--file", Verifier, "--", "--verify", directory);
void Clean(string dest)
{
    var result = Verify(dest);
    Need(result.ExitCode == 0 &&
        result.Output.Contains("MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS",
            StringComparison.Ordinal), "Copied standalone verifier rejected a clean export: " + result.Output);
}
Clean(a);
Clean(b);
Directory.CreateDirectory(copy);
foreach (var name in names)
{
    var target = Path.Combine(copy, name.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
    File.Copy(Path.Combine(a, name.Replace('/', Path.DirectorySeparatorChar)), target);
}
Clean(copy);
var skill = Path.Combine(copy, Skill.Replace('/', Path.DirectorySeparatorChar));
var before = File.ReadAllBytes(skill);
try
{
    File.AppendAllText(skill, " ", new UTF8Encoding(false));
    var denied = Verify(copy);
    Need(denied.ExitCode != 0 &&
        denied.Output.Contains("Portable file hash drift: " + Skill, StringComparison.Ordinal),
        "Copied modified skill escaped integrity verifier: " + denied.Output);
}
finally { File.WriteAllBytes(skill, before); }
Clean(copy);
foreach (var path in source)
    Need(Sha(path) == originalSha[path], "Math source modified during delivery: " + path);
Console.WriteLine("MATH-RS309-DELIVERY-GREEN|exports=2|files=14|identical=true|" +
    "receiptHashes=13|isolatedVerifier=2|copiedConsumer=1|tamperRefused=1|" +
    "unsigned=true|sourceUnchanged=true|brokerReady=0|PASS");

static string Sha(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS309: " + reason);
}
static ProcessResult Run(string cwd, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = cwd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS309: Cannot start portable CLI.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS309: Child process timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
