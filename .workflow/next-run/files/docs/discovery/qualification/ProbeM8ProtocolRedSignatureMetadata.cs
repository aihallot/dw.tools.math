#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Use --check.");

const string Original = "docs/discovery/qualification/ValidatePortableProtocol.cs";
const string Schema = "docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json";
const string Corpus = "docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string SchemaHash = "678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
const string CorpusHash = "5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";
const string Anchor =
    "    Require(accepted == 2 && rejected == 11, \"Unexpected accepted/rejected totals.\");";
const string RedCases = """
    var newParameterMismatch = baseline.DeepClone().AsObject();
    var parameterCapability = Array(newParameterMismatch, "capabilities")[0]!.AsObject();
    Array(parameterCapability, "parameters")[0]!.AsObject()["type"] = "System.String";
    var paramAdmitted = false;
    try { ValidateMathDraftEnvelope(newParameterMismatch); paramAdmitted = true; }
    catch (InvalidOperationException) { }

    var newMemberMismatch = baseline.DeepClone().AsObject();
    var memberCapability = Array(newMemberMismatch, "capabilities")[0]!.AsObject();
    Object(memberCapability, "binding")["member"] = "NotMultiply";
    var memberAdmitted = false;
    try { ValidateMathDraftEnvelope(newMemberMismatch); memberAdmitted = true; }
    catch (InvalidOperationException) { }

    var knownAuthorityEscalation = baseline.DeepClone().AsObject();
    var authorityCapability = Array(knownAuthorityEscalation, "capabilities")[0]!.AsObject();
    Object(authorityCapability, "execution")["authorization"] = "granted";
    var unauthorizedAdmitted = false;
    try { ValidateMathDraftEnvelope(knownAuthorityEscalation); unauthorizedAdmitted = true; }
    catch (InvalidOperationException) { }

    Require(!unauthorizedAdmitted, "RED control: existing authorization refusal was lost.");
    Require(paramAdmitted && memberAdmitted,
        "The specified signature/metadata drift gap was not reproduced; re-evaluate RED baseline.");
    Console.WriteLine("MATH-RS286-RED-GAP|parameter-type=admitted|binding-member=admitted|" +
        "known-authorization=refused|source-verifier-unmodified=true|PASS");
    """;

Need(File.Exists(Original) && File.Exists(Schema) && File.Exists(Corpus),
    "Pinned original Math protocol verifier and fixtures must exist.");
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Need(Hash(Schema) == SchemaHash && Hash(Corpus) == CorpusHash,
    "Canonical pinned Decision schema or fixtures changed.");
using (var prior = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS209/attempt-001.json")))
{
    var report = prior.RootElement;
    Need(report.GetProperty("runId").GetString() == "RS209" &&
        report.GetProperty("outcome").GetString() == "succeeded" &&
        report.GetProperty("attempt").GetInt32() == 1,
        "Prior pinned fixture receipt is not qualified.");
}
var source = File.ReadAllText(Original);
Need(source.Split(Anchor, StringSplitOptions.None).Length == 2 &&
    source.Contains("static void ValidateMathDraftEnvelope(JsonObject envelope)", StringComparison.Ordinal) &&
    source.Contains("Capability metadata cannot grant authorization.", StringComparison.Ordinal),
    "The original independent C# protocol method or injection anchor changed.");

var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs286-protocol-red", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n",
    new UTF8Encoding(false));
foreach (var file in new[] { Schema, Corpus })
{
    var destination = Path.Combine(root, file);
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    File.Copy(file, destination);
    Need(Hash(destination) == Hash(file), "Disposable fixture copy changed: " + file);
}
var executable = Path.Combine(root, Original);
Directory.CreateDirectory(Path.GetDirectoryName(executable)!);
File.WriteAllText(executable, source.Replace(Anchor, RedCases + "\n" + Anchor,
    StringComparison.Ordinal), new UTF8Encoding(false));

var start = new ProcessStartInfo("dotnet")
{
    WorkingDirectory = root,
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};
foreach (var argument in new[] { "run", "--file", Original })
    start.ArgumentList.Add(argument);
using var process = Process.Start(start) ??
    throw new InvalidOperationException("RS286 could not start the isolated original verifier.");
var stdout = process.StandardOutput.ReadToEndAsync();
var stderr = process.StandardError.ReadToEndAsync();
if (!process.WaitForExit(120000))
{
    process.Kill(entireProcessTree: true);
    throw new TimeoutException("RS286 original protocol verifier test timed out.");
}
var output = stdout.GetAwaiter().GetResult();
var error = stderr.GetAwaiter().GetResult();
Need(process.ExitCode == 0, "Isolated verifier RED probe failed: " + output + "\n" + error);
Need(output.Contains("RESULT PASS 13/13 (2 accepted, 11 rejected)", StringComparison.Ordinal) &&
    output.Contains("MATH-RS286-RED-GAP|parameter-type=admitted|binding-member=admitted|" +
        "known-authorization=refused|source-verifier-unmodified=true|PASS", StringComparison.Ordinal),
    "Original pinned case results or new RED acceptance observations changed: " + output);
Need(Hash(Schema) == SchemaHash && Hash(Corpus) == CorpusHash,
    "The original pinned corpus was modified during the RED probe.");
Need(File.ReadAllText(Original) == source, "Original verifier was changed by the RED probe.");
Console.WriteLine("MATH-RS286-RED-BOUNDARY|pinned=13|newCases=3|knownRefusal=1|" +
    "newMetadataGaps=2|sdk=11.0.100|originalsUnchanged=true|PASS");

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS286: " + reason);
}
