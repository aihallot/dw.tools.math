#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Attester = "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs";
const string Receipt = "portable-attestation.json";
const string Verifier = "verify-export.cs";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
var originals = new[] { Attester, Skill,
    "docs/discovery/qualification/m8-discovery-manifest.json" };
var originalHashes = originals.ToDictionary(path => path, Hash, StringComparer.Ordinal);
var repo = Path.GetFullPath(".");
var work = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs307-delivery-red", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(work);
var exported = Path.Combine(work, "export");
var consumer = Path.Combine(work, "copied-consumer");
var verifierHost = Path.Combine(work, "verifier-host");
var occupied = Path.Combine(work, "occupied");
Directory.CreateDirectory(occupied);
var sentinel = Path.Combine(occupied, "do-not-replace.txt");
File.WriteAllText(sentinel, "SENTINEL\n", new UTF8Encoding(false));
var originalSentinel = Hash(sentinel);

ProcessResult Export(string path) => Run(repo, "run", "--file", Attester, "--", "--export", path);
var positive = Export(exported);
Need(positive.ExitCode == 0 &&
    positive.Output.Contains("MATH-RS283-PORTABLE-ATTESTED|files=13|receipt=unsigned|",
        StringComparison.Ordinal) &&
    positive.Output.Contains("brokerReady=false|PASS", StringComparison.Ordinal),
    "Fresh public opt-in export refused: " + positive.Output);
var files = Directory.GetFiles(exported, "*", SearchOption.AllDirectories);
Need(files.Length == 14, "Expected fourteen portable files.");
var occupiedAttempt = Export(occupied);
Need(occupiedAttempt.ExitCode != 0 &&
    occupiedAttempt.Output.Contains("Export requires an absolute, not-yet-existing destination.",
        StringComparison.Ordinal) &&
    Hash(sentinel) == originalSentinel &&
    Directory.GetFiles(occupied, "*", SearchOption.AllDirectories).Length == 1,
    "Occupied destination was overwritten or not explicitly refused: " + occupiedAttempt.Output);

Directory.CreateDirectory(consumer);
foreach (var file in files)
{
    var relative = Path.GetRelativePath(exported, file);
    var dest = Path.Combine(consumer, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(file, dest);
    Need(Hash(dest) == Hash(file), "Independent consumer copy differs: " + relative);
}
Directory.CreateDirectory(verifierHost);
File.Copy(Path.Combine(consumer, Verifier), Path.Combine(verifierHost, Verifier));
File.Copy(Path.Combine(consumer, "global.json"), Path.Combine(verifierHost, "global.json"));
ProcessResult Verify() => Run(verifierHost, "run", "--file", Verifier, "--", "--verify", consumer);
void Clean()
{
    var v = Verify();
    Need(v.ExitCode == 0 &&
        v.Output.Contains("MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS",
            StringComparison.Ordinal),
        "Independent copied offline verifier rejected clean bundle: " + v.Output);
}
Clean();
var refused = 0;
void RejectFile(string path, string extra, string failure)
{
    var target = Path.Combine(consumer, path.Replace('/', Path.DirectorySeparatorChar));
    var original = File.ReadAllBytes(target);
    try
    {
        File.AppendAllText(target, extra, new UTF8Encoding(false));
        var result = Verify();
        Need(result.ExitCode != 0 && result.Output.Contains(failure, StringComparison.Ordinal),
            "Portable delivery drift accepted: " + path + " " + result.Output);
        refused++;
    }
    finally { File.WriteAllBytes(target, original); }
    Clean();
}
RejectFile(Skill, " ", "Portable file hash drift: " + Skill);
var surprise = Path.Combine(consumer, "unexpected-injected.txt");
try
{
    File.WriteAllText(surprise, "not-attested\n", new UTF8Encoding(false));
    var denied = Verify();
    Need(denied.ExitCode != 0 &&
        denied.Output.Contains("Portable bundle has missing, extra or renamed files.",
            StringComparison.Ordinal),
        "Extra unqualified portable delivery file escaped refusal: " + denied.Output);
    refused++;
}
finally { if (File.Exists(surprise)) File.Delete(surprise); }
Clean();
var receiptPath = Path.Combine(consumer, Receipt);
var originalReceipt = File.ReadAllBytes(receiptPath);
try
{
    var doc = JsonNode.Parse(originalReceipt)?.AsObject() ??
        throw new InvalidDataException("RS307: Invalid portable receipt.");
    doc["authorization"] = "granted";
    File.WriteAllText(receiptPath, doc.ToJsonString() + "\n", new UTF8Encoding(false));
    var denied = Verify();
    Need(denied.ExitCode != 0 &&
        denied.Output.Contains("Portable receipt provenance, schema or authorization changed.",
            StringComparison.Ordinal),
        "Forged portable authorization accepted: " + denied.Output);
    refused++;
}
finally { File.WriteAllBytes(receiptPath, originalReceipt); }
Clean();
Need(refused == 3, "Expected three independent refusal cases.");
foreach (var path in originals)
    Need(Hash(path) == originalHashes[path], "Canonical Math source mutated: " + path);
Need(Hash(sentinel) == originalSentinel, "Occupied opt-in destination changed.");
Console.WriteLine("MATH-RS307-DELIVERY-RED|exported=14|recordedHashes=13|copiedVerifier=true|" +
    "occupiedRefused=1|tamperRefused=1|extraRefused=1|grantRefused=1|" +
    "unsignedReceipt=true|sourceUnchanged=true|brokerReady=0|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool valid, string error)
{
    if (!valid) throw new InvalidDataException("RS307: " + error);
}
static ProcessResult Run(string cwd, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = cwd,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS307: Cannot start portable CLI.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS307: Portable CLI timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
