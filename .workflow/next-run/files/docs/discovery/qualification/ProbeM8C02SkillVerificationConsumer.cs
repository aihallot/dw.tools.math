#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Generator = "docs/discovery/qualification/GenerateM8MathSkill.cs";
const string Gate = "docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs";
const string Source = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Skill = ".agents/skills/dw-tools-math/SKILL.md";
const string Bundle = ".agents/skills/dw-tools-math/references/manifest.json";
var copiedFiles = new[] { Generator, Gate, Source, Skill, Bundle };
var canonicalHashes = copiedFiles.ToDictionary(x => x, Digest, StringComparer.Ordinal);
var consumer = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs305-copied-skill-verification", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(consumer);
string Copied(string name) => Path.Combine(consumer, name.Replace('/', Path.DirectorySeparatorChar));
foreach (var name in copiedFiles)
{
    var dest = Copied(name);
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(name, dest);
    Need(Digest(dest) == canonicalHashes[name], "Copied skill consumer file differs: " + name);
}
using (var source = JsonDocument.Parse(File.ReadAllText(Copied(Source))))
using (var bundled = JsonDocument.Parse(File.ReadAllText(Copied(Bundle))))
{
    var a = source.RootElement;
    var b = bundled.RootElement;
    var selected = a.GetProperty("capabilities").EnumerateArray().ToArray();
    var copied = b.GetProperty("capabilities").EnumerateArray().ToArray();
    Need(selected.Length == 4 && copied.Length == selected.Length &&
        a.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        b.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
        "Copied consumer capability/broker scope is wrong.");
    for (var i = 0; i < selected.Length; i++)
    {
        Need(selected[i].GetProperty("capabilityId").GetString() ==
            copied[i].GetProperty("capabilityId").GetString() &&
            selected[i].GetProperty("binding").GetProperty("fingerprint").GetString() ==
            copied[i].GetProperty("binding").GetProperty("fingerprint").GetString() &&
            copied[i].GetProperty("execution").GetProperty("authorization").GetString() ==
            "runtime-only",
            "Copied source-to-skill CLR identity or authorization differs.");
    }
}
ProcessResult Strict() => Execute("run", "--file", Gate, "--", "--validate", Copied(Bundle));
ProcessResult Generated() => Execute("run", "--file", Generator, "--", "--check");
void Clean()
{
    var strict = Strict();
    Need(strict.ExitCode == 0 &&
        strict.Output.Contains("MATH-M8-STRICT-BINDING|mode=validate|capabilities=4|metadataOnly=true|brokerReady=0|PASS",
            StringComparison.Ordinal), "Strict copied metadata failed: " + strict.Output);
    var generated = Generated();
    Need(generated.ExitCode == 0 &&
        generated.Output.Contains("MATH-RS201-SKILL-MANIFEST|capabilities=4|brokerReady=0|mode=--check",
            StringComparison.Ordinal), "Copied generated skill failed: " + generated.Output);
}
Clean();
var altered = 0;
void Refuse(string path, string suffix, Func<ProcessResult> check, string message)
{
    var absolute = Copied(path);
    var saved = File.ReadAllBytes(absolute);
    try
    {
        File.AppendAllText(absolute, suffix, new UTF8Encoding(false));
        var result = check();
        Need(result.ExitCode != 0 && result.Output.Contains(message, StringComparison.Ordinal),
            "Modified consumer file escaped refusal: " + path + " " + result.Output);
        altered++;
    }
    finally { File.WriteAllBytes(absolute, saved); }
    Clean();
}
Refuse(Skill, " ", Generated, "Generated skill is absent or stale");
var originalBundle = File.ReadAllBytes(Copied(Bundle));
try
{
    var node = System.Text.Json.Nodes.JsonNode.Parse(originalBundle)?.AsObject() ??
        throw new InvalidDataException("RS305: Invalid copied manifest.");
    node["capabilities"]!.AsArray()[0]!.AsObject()["binding"]!.AsObject()
        ["fingerprint"] = new string('0', 64);
    File.WriteAllText(Copied(Bundle), node.ToJsonString() + "\n", new UTF8Encoding(false));
    var strict = Strict();
    Need(strict.ExitCode != 0 &&
        strict.Output.Contains("Canonical CLR SHA-256 fingerprint differs", StringComparison.Ordinal),
        "Copied forged CLR fingerprint escaped the strict gate: " + strict.Output);
    altered++;
}
finally { File.WriteAllBytes(Copied(Bundle), originalBundle); }
Clean();
Need(altered == 2, "Expected two independent copied-consumer refusals.");
foreach (var name in copiedFiles)
    Need(Digest(name) == canonicalHashes[name], "Canonical file changed: " + name);
Console.WriteLine("MATH-RS305-COPIED-SKILL-VERIFY|files=5|selected=4|cleanGates=2|" +
    "skillDriftRefused=1|fingerprintRefused=1|restored=true|brokerReady=0|PASS");

static string Digest(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static void Need(bool ok, string label)
{
    if (!ok) throw new InvalidDataException("RS305: " + label);
}
ProcessResult Execute(params string[] args)
{
    var p = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = consumer, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in args) p.ArgumentList.Add(arg);
    using var process = Process.Start(p) ??
        throw new InvalidOperationException("RS305: Cannot start copied consumer CLI.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS305: Copied consumer CLI timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
