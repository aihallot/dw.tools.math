#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Canonical = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Gate = "docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs";
var originalSha = HashFile(Canonical);
var original = JsonNode.Parse(File.ReadAllText(Canonical))?.AsObject() ??
    throw new InvalidDataException("RS296: Missing canonical Math Discovery manifest.");
var work = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs296-independent-green", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(work);

var valid = Path.Combine(work, "valid-four-methods.json");
Write(valid, original);
var passed = Execute(valid);
Check(passed.ExitCode == 0 &&
    passed.Output.Contains("MATH-RS294-REAL-CLR-ADMISSION|mode=validate|bindings=4|archives=3|",
        StringComparison.Ordinal) &&
    passed.Output.Contains("methodInvocations=0|brokerReady=0|PASS", StringComparison.Ordinal),
    "Independent process refused real four-method admission: " + passed.Output);

var count = 0;
void Forgery(string label, Action<JsonObject> change)
{
    var forged = original.DeepClone().AsObject();
    var caps = forged["capabilities"]?.AsArray() ??
        throw new InvalidDataException("RS296: Missing capabilities.");
    var first = caps[0]?.AsObject() ??
        throw new InvalidDataException("RS296: Missing selected first capability.");
    change(first);
    var binding = first["binding"]?.AsObject() ??
        throw new InvalidDataException("RS296: Missing binding.");
    var parameters = first["parameters"]?.AsArray() ??
        throw new InvalidDataException("RS296: Missing parameters.");
    var result = first["result"]?.AsObject() ??
        throw new InvalidDataException("RS296: Missing result.");
    static string Field(JsonObject o, string key) =>
        o[key]?.GetValue<string>() ??
            throw new InvalidDataException("RS296: Missing " + key);
    var canonical = string.Join("|", Field(binding, "assembly"),
        Field(binding, "assemblyVersion"), Field(binding, "type"),
        Field(binding, "member"), Field(result, "type") + "(" +
        string.Join(",", parameters.Select(x => Field(x?.AsObject() ??
            throw new InvalidDataException("RS296: Parameter is not an object."), "type"))) + ")");
    binding["canonicalSignature"] = canonical;
    binding["fingerprint"] = Convert.ToHexStringLower(
        SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    var file = Path.Combine(work, label + ".json");
    Write(file, forged);
    var denied = Execute(file);
    Check(denied.ExitCode != 0 &&
        denied.Output.Contains("Real published CLR method mismatch:", StringComparison.Ordinal),
        "Real CLR admission did not reject coherent forgery " + label + ": " + denied.Output);
    count++;
}
Forgery("missing-member", x => x["binding"]!.AsObject()["member"] =
    "NonexistentSelectedMethod");
Forgery("changed-parameter", x => x["parameters"]!.AsArray()[0]!.AsObject()["type"] =
    "System.String");
Forgery("changed-result", x => x["result"]!.AsObject()["type"] =
    "System.String");
Check(count == 3, "Expected three independently rejected coherent forgeries.");

var unauthorized = original.DeepClone().AsObject();
unauthorized["capabilities"]!.AsArray()[0]!.AsObject()
    ["execution"]!.AsObject()["authorization"] = "granted";
var unauthorizedFile = Path.Combine(work, "unauthorized.json");
Write(unauthorizedFile, unauthorized);
var refused = Execute(unauthorizedFile);
Check(refused.ExitCode != 0,
    "Real CLR gate accepted unauthorized execution metadata.");
Check(HashFile(Canonical) == originalSha,
    "Canonical Discovery manifest mutated by independent consumer.");
Console.WriteLine("MATH-RS296-CLR-GREEN|processCaller=independent|positives=4|" +
    "coherentForgeriesRefused=3|authorizationRefused=1|" +
    "methodInvocations=0|brokerReady=0|PASS");

static void Write(string path, JsonObject value) =>
    File.WriteAllText(path,
        value.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n",
        new UTF8Encoding(false));
static string HashFile(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static void Check(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS296: " + reason);
}
static ProcessResult Execute(string manifestPath)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = Path.GetFullPath("."),
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in new[] { "run", "--file", Gate, "--", "--validate", manifestPath })
        start.ArgumentList.Add(arg);
    using var child = Process.Start(start) ??
        throw new InvalidOperationException("RS296: Unable to start admission CLI.");
    var stdout = child.StandardOutput.ReadToEndAsync();
    var stderr = child.StandardError.ReadToEndAsync();
    if (!child.WaitForExit(120000))
    {
        child.Kill(entireProcessTree: true);
        throw new TimeoutException("RS296: Admission CLI timed out.");
    }
    return new ProcessResult(child.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
