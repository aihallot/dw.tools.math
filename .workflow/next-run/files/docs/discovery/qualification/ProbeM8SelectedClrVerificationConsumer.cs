#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Exporter = "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs";
const string RealGate = "docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs";
const string Manifest = ".agents/skills/dw-tools-math/references/manifest.json";
const string Verifier = "verify-export.cs";
const string Receipt = "portable-attestation.json";
const string SourceManifest = "docs/discovery/qualification/m8-discovery-manifest.json";
const string RealAccepted = "MATH-RS294-REAL-CLR-ADMISSION|mode=validate|bindings=4|archives=3|";
const string OfflineAccepted = "MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS";

var originalSha = Hash(SourceManifest);
var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs298-clr-verification", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
var exported = Path.Combine(root, "public-export");
var consumer = Path.Combine(root, "isolated-consumer");
var host = Path.Combine(root, "offline-verifier-host");
var repo = Path.GetFullPath(".");
var fresh = Run(repo, "run", "--file", Exporter, "--", "--export", exported);
Need(fresh.ExitCode == 0 &&
    fresh.Output.Contains("MATH-RS283-PORTABLE-ATTESTED|files=13|", StringComparison.Ordinal),
    "Public portable export failed: " + fresh.Output);
var files = Directory.GetFiles(exported, "*", SearchOption.AllDirectories);
Need(files.Length == 14, "Unexpected public export file count.");
Directory.CreateDirectory(consumer);
foreach (var file in files)
{
    var relative = Path.GetRelativePath(exported, file);
    var destination = Path.Combine(consumer, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    File.Copy(file, destination);
    Need(Hash(file) == Hash(destination), "Copied portable file differs: " + relative);
}
var manifest = Path.Combine(consumer, Manifest.Replace('/', Path.DirectorySeparatorChar));
Need(File.Exists(manifest) && File.Exists(Path.Combine(consumer, Receipt)) &&
    File.Exists(Path.Combine(consumer, Verifier)),
    "The copied consumer lacks required portable metadata and verifier.");
using (var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(consumer, Receipt))))
{
    var record = receipt.RootElement;
    Need(record.GetProperty("signatureStatus").GetString() == "unsigned" &&
        record.GetProperty("authorization").GetString() == "runtime-only" &&
        record.GetProperty("files").GetArrayLength() == 13,
        "The copied receipt overstates authenticity or execution rights.");
}
Directory.CreateDirectory(host);
File.Copy(Path.Combine(consumer, Verifier), Path.Combine(host, Verifier));
File.Copy(Path.Combine(consumer, "global.json"), Path.Combine(host, "global.json"));
ProcessResult Offline() => Run(host, "run", "--file", Verifier, "--", "--verify", consumer);
ProcessResult Real() => Run(repo, "run", "--file", RealGate, "--", "--validate", manifest);
void Clean()
{
    var offline = Offline();
    Need(offline.ExitCode == 0 &&
        offline.Output.Contains(OfflineAccepted, StringComparison.Ordinal),
        "Independent copied offline verifier rejected clean bundle: " + offline.Output);
    var real = Real();
    Need(real.ExitCode == 0 &&
        real.Output.Contains(RealAccepted, StringComparison.Ordinal) &&
        real.Output.Contains("methodInvocations=0|brokerReady=0|PASS", StringComparison.Ordinal),
        "Actual published CLR gate rejected copied manifest: " + real.Output);
}
Clean();
var original = File.ReadAllBytes(manifest);
var count = 0;
void Refuse(string name, Action<JsonObject> alter)
{
    var item = JsonNode.Parse(original)?.AsObject() ??
        throw new InvalidDataException("RS298: Invalid copied manifest.");
    var cap = item["capabilities"]?.AsArray()[0]?.AsObject() ??
        throw new InvalidDataException("RS298: Missing selected capability.");
    alter(cap);
    var binding = cap["binding"]?.AsObject() ??
        throw new InvalidDataException("RS298: Missing CLR binding.");
    var parameters = cap["parameters"]?.AsArray() ??
        throw new InvalidDataException("RS298: Missing CLR parameters.");
    var result = cap["result"]?.AsObject() ??
        throw new InvalidDataException("RS298: Missing CLR result.");
    static string Field(JsonObject obj, string field) =>
        obj[field]?.GetValue<string>() ??
            throw new InvalidDataException("RS298: Missing " + field);
    var signature = string.Join("|", Field(binding, "assembly"),
        Field(binding, "assemblyVersion"), Field(binding, "type"),
        Field(binding, "member"), Field(result, "type") + "(" +
        string.Join(",", parameters.Select(p => Field(p?.AsObject() ??
            throw new InvalidDataException("RS298: Invalid parameter."), "type"))) + ")");
    binding["canonicalSignature"] = signature;
    binding["fingerprint"] = Convert.ToHexStringLower(
        SHA256.HashData(Encoding.UTF8.GetBytes(signature)));
    File.WriteAllText(manifest, item.ToJsonString(
        new JsonSerializerOptions { WriteIndented = true }) + "\n", new UTF8Encoding(false));
    var denied = Real();
    Need(denied.ExitCode != 0 &&
        denied.Output.Contains("Real published CLR method mismatch:", StringComparison.Ordinal),
        "Coherently forged method escaped real admission: " + name + " " + denied.Output);
    var offline = Offline();
    Need(offline.ExitCode != 0 &&
        offline.Output.Contains("Portable file hash drift: " + Manifest, StringComparison.Ordinal),
        "Coherent forged method escaped copied receipt checksum: " + name + " " + offline.Output);
    File.WriteAllBytes(manifest, original);
    Clean();
    count++;
}
Refuse("missing-member", cap => cap["binding"]!.AsObject()["member"] =
    "NonexistentSelectedMethod");
Refuse("wrong-parameter", cap => cap["parameters"]!.AsArray()[0]!.AsObject()["type"] =
    "System.String");
Need(count == 2 && Hash(SourceManifest) == originalSha,
    "Source-qualified Math manifest changed or negative matrix incomplete.");
Console.WriteLine("MATH-RS298-CLR-VERIFICATION|publicExport=14|copiedReceiptFiles=13|" +
    "realPublishedMethods=4|coherentForgeriesRefused=2|offlineRefusals=2|" +
    "unsignedReceipt=true|runtimeOnly=true|brokerReady=0|PASS");

static string Hash(string path)
{
    using var file = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(file));
}
static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS298: " + reason);
}
static ProcessResult Run(string workingDirectory, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workingDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS298: Cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS298: Bounded subprocess timeout.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
