#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Attester = "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs";
const string StrictGate = "docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs";
const string CanonicalManifest = "docs/discovery/qualification/m8-discovery-manifest.json";
const string ReceiptFile = "portable-attestation.json";
const string PortableVerifier = "verify-export.cs";
const string RelativeManifest = ".agents/skills/dw-tools-math/references/manifest.json";
const string ExpectedOffline = "MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS";
const string ExpectedExport = "MATH-RS283-PORTABLE-ATTESTED|files=13|receipt=unsigned|packages=not-included|brokerReady=false|PASS";
const string ExpectedStrict = "MATH-M8-STRICT-BINDING|mode=validate|capabilities=4|metadataOnly=true|brokerReady=0|PASS";
const string Schema = "docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json";
const string Fixtures = "docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string ExpectedSchemaSha = "678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
const string ExpectedFixturesSha = "5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";

Need(File.Exists(Attester) && File.Exists(StrictGate) &&
    File.Exists(CanonicalManifest) && File.Exists(Schema) && File.Exists(Fixtures),
    "Math-owned source and immutable pinned protocol inputs must exist.");
Need(Sha(Fixtures) == ExpectedFixturesSha, "Pinned 13-case protocol corpus changed.");
Need(Sha(Schema) == ExpectedSchemaSha, "Pinned Decision schema bytes changed.");
using (var prior = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS290/attempt-001.json")))
{
    var run = prior.RootElement;
    var first = run.GetProperty("priorInvocationFailures");
    Need(run.GetProperty("runId").GetString() == "RS290" &&
        run.GetProperty("attempt").GetInt32() == 1 &&
        run.GetProperty("outcome").GetString() == "succeeded" &&
        run.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
        first.GetProperty("count").GetInt32() == 0 &&
        first.GetProperty("firstPassSucceeded").GetBoolean() &&
        run.GetProperty("progress").GetProperty("completions")
            .EnumerateArray().Single().GetProperty("id").GetString() ==
            "M8-W01-C01-T1-G",
        "RS290 GREEN completion evidence or first-pass lineage changed.");
}
using (var corpus = JsonDocument.Parse(File.ReadAllText(Fixtures)))
{
    var root = corpus.RootElement;
    var cases = root.GetProperty("cases").EnumerateArray().ToArray();
    Need(root.GetProperty("fixtureVersion").GetInt32() == 1 &&
        cases.Length == 13 &&
        cases.Count(item => item.GetProperty("accept").GetBoolean()) == 2,
        "Pinned conformance corpus is not exactly 2 accepted / 11 refused.");
}
using (var protocolReport = JsonDocument.Parse(File.ReadAllText(
    "docs/discovery/qualification/RS131-portable-protocol-qualified.json")))
{
    var proof = protocolReport.RootElement;
    Need(proof.GetProperty("total_cases").GetInt32() == 13 &&
        proof.GetProperty("accepted_cases").GetInt32() == 2 &&
        proof.GetProperty("rejected_cases").GetInt32() == 11,
        "Original protocol conformance receipt differs.");
}

var repoRoot = Path.GetFullPath(".");
var rootDir = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs291-independent-verification", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(rootDir);
var generated = Path.Combine(rootDir, "generated");
var consumer = Path.Combine(rootDir, "consumer");
var verifierHost = Path.Combine(rootDir, "verifier-host");
var sourceHash = Sha(CanonicalManifest);
var attesterHash = Sha(Attester);
var strictHash = Sha(StrictGate);
var exported = RunDotnet(repoRoot, "run", "--file", Attester, "--",
    "--export", generated);
Need(exported.ExitCode == 0 && exported.Output.Contains(ExpectedExport, StringComparison.Ordinal),
    "Public attested export failed: " + exported.Output);
Need(Directory.Exists(generated) && !Directory.Exists(consumer),
    "Fresh public export or unoccupied consumer destination is missing.");

var sourceFiles = Directory.GetFiles(generated, "*", SearchOption.AllDirectories);
Need(sourceFiles.Length == 14, "Qualified portable export must contain exactly fourteen files.");
Directory.CreateDirectory(consumer);
foreach (var file in sourceFiles)
{
    var relative = Path.GetRelativePath(generated, file);
    var dest = Path.Combine(consumer, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(file, dest);
    Need(Sha(dest) == Sha(file), "Independent consumer copy changed: " + relative);
}

var receiptPath = Path.Combine(consumer, ReceiptFile);
var manifestPath = Path.Combine(consumer,
    RelativeManifest.Replace('/', Path.DirectorySeparatorChar));
var verifierPath = Path.Combine(consumer, PortableVerifier);
Need(File.Exists(manifestPath) && File.Exists(verifierPath) && File.Exists(receiptPath),
    "Copied consumer is missing its metadata, verifier or unsigned receipt.");
using (var receipt = JsonDocument.Parse(File.ReadAllText(receiptPath)))
{
    var r = receipt.RootElement;
    Need(r.GetProperty("schemaVersion").GetInt32() == 1 &&
        r.GetProperty("kind").GetString() == "math-discovery-portable-integrity-sha256/1" &&
        r.GetProperty("signatureStatus").GetString() == "unsigned" &&
        r.GetProperty("authorization").GetString() == "runtime-only" &&
        r.GetProperty("sourceQualification").GetString() == "RS281" &&
        r.GetProperty("linuxQualification").GetString() == "RS282",
        "Copied portable receipt changed its scope or invented signed provenance.");
    var records = r.GetProperty("files").EnumerateArray().ToArray();
    Need(records.Length == 13, "Copied portable receipt must cover thirteen files.");
    foreach (var item in records)
    {
        var path = item.GetProperty("path").GetString()
            ?? throw new InvalidDataException("RS291: Missing attested file path.");
        Need(!Path.IsPathRooted(path) && !path.Contains("..", StringComparison.Ordinal),
            "Unsafe attested file path.");
        var expectedSha = item.GetProperty("sha256").GetString();
        var destination = Path.Combine(consumer,
            path.Replace('/', Path.DirectorySeparatorChar));
        Need(File.Exists(destination) && Sha(destination) == expectedSha,
            "Copied file does not match its portable checksum: " + path);
    }
}
using (var canonical = JsonDocument.Parse(File.ReadAllText(CanonicalManifest)))
using (var copied = JsonDocument.Parse(File.ReadAllText(manifestPath)))
{
    var canonicalIds = canonical.RootElement.GetProperty("capabilities")
        .EnumerateArray().Select(item => item.GetProperty("capabilityId").GetString()).ToArray();
    var copiedCaps = copied.RootElement.GetProperty("capabilities").EnumerateArray().ToArray();
    Need(canonicalIds.Length == 4 && copiedCaps.Length == 4 &&
        copied.RootElement.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
        "Only four nonbroker Math capabilities are selected.");
    for (var i = 0; i < copiedCaps.Length; i++)
    {
        var original = canonical.RootElement.GetProperty("capabilities")[i];
        var actual = copiedCaps[i];
        Need(original.GetProperty("capabilityId").GetString() == actual.GetProperty("capabilityId").GetString() &&
            original.GetProperty("binding").GetProperty("canonicalSignature").GetString() ==
            actual.GetProperty("binding").GetProperty("canonicalSignature").GetString() &&
            original.GetProperty("binding").GetProperty("fingerprint").GetString() ==
            actual.GetProperty("binding").GetProperty("fingerprint").GetString() &&
            actual.GetProperty("exposure").GetString() == "AgentCallable" &&
            actual.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only",
            "Copied Math capability binding or runtime-only scope changed.");
    }
}
var cleanStrict = RunDotnet(repoRoot, "run", "--file", StrictGate, "--",
    "--validate", manifestPath);
Need(cleanStrict.ExitCode == 0 &&
    cleanStrict.Output.Contains(ExpectedStrict, StringComparison.Ordinal),
    "Strict CLR metadata gate refused an unchanged public export: " + cleanStrict.Output);

Directory.CreateDirectory(verifierHost);
File.Copy(verifierPath, Path.Combine(verifierHost, PortableVerifier));
File.Copy(Path.Combine(consumer, "global.json"),
    Path.Combine(verifierHost, "global.json"));
Need(File.ReadAllText(Path.Combine(verifierHost, PortableVerifier))
        .Replace("\r\n", "\n", StringComparison.Ordinal) ==
    File.ReadAllText(Attester).Replace("\r\n", "\n", StringComparison.Ordinal),
    "Standalone copied verifier does not match Math's public export.");
void VerifyClean()
{
    var result = RunDotnet(verifierHost, "run", "--file", PortableVerifier,
        "--", "--verify", consumer);
    Need(result.ExitCode == 0 &&
        result.Output.Contains(ExpectedOffline, StringComparison.Ordinal),
        "Consumer-isolated offline verifier refused a valid export: " + result.Output);
}
VerifyClean();

var manifestOriginal = File.ReadAllBytes(manifestPath);
void Reject(string label, Action<JsonObject> mutation, string reason)
{
    var json = JsonNode.Parse(File.ReadAllText(manifestPath))?.AsObject()
        ?? throw new InvalidDataException("RS291: Invalid copied manifest.");
    var selected = json["capabilities"]?.AsArray() ??
        throw new InvalidDataException("RS291: No copied capabilities.");
    var first = selected[0]?.AsObject()
        ?? throw new InvalidDataException("RS291: No first copied capability.");
    mutation(first);
    File.WriteAllText(manifestPath,
        json.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) +
        "\n", new UTF8Encoding(false));

    var strictResult = RunDotnet(repoRoot, "run", "--file", StrictGate,
        "--", "--validate", manifestPath);
    Need(strictResult.ExitCode != 0 &&
        strictResult.Output.Contains("RS287: " + reason, StringComparison.Ordinal),
        "Consumer mutation escaped the strict Math metadata boundary: " +
        label + " " + strictResult.Output);
    var offline = RunDotnet(verifierHost, "run", "--file", PortableVerifier,
        "--", "--verify", consumer);
    Need(offline.ExitCode != 0 &&
        offline.Output.Contains("Portable file hash drift: " + RelativeManifest,
            StringComparison.Ordinal),
        "Consumer mutation escaped recorded checksum verification: " +
        label + " " + offline.Output);
    File.WriteAllBytes(manifestPath, manifestOriginal);
    VerifyClean();
    Console.WriteLine("MATH-RS291-INDEPENDENT-REFUSAL|" + label +
        "|strict=true|offlineChecksum=true|PASS");
}
Reject("member-drift", cap =>
    (cap["binding"]?.AsObject() ??
        throw new InvalidDataException("RS291: Binding absent."))["member"] = "OtherMember",
    "Canonical CLR signature does not match declared fields");
Reject("authorization-grant", cap =>
    (cap["execution"]?.AsObject() ??
        throw new InvalidDataException("RS291: Execution absent."))["authorization"] = "granted",
    "Runtime-only authorization is required");

Need(Sha(CanonicalManifest) == sourceHash &&
    Sha(Attester) == attesterHash &&
    Sha(StrictGate) == strictHash,
    "Verification mutated canonical source or qualified Math tooling.");
Need(Sha(manifestPath) == Sha(Path.Combine(generated,
    RelativeManifest.Replace('/', Path.DirectorySeparatorChar))),
    "Independent consumer was not returned to its clean metadata.");
Console.WriteLine("MATH-RS291-INDEPENDENT-VERIFICATION|publicExport=PASS|" +
    "files=14|selected=4|protocolCases=13|strict=PASS|offline=PASS|" +
    "negative=2|unsigned=true|packagesNotIncluded=true|brokerReady=0|PASS");

static ProcessResult RunDotnet(string cwd, params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = cwd,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS291: Could not start dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS291 timed out: " + string.Join(" ", arguments));
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult().Trim() + "\n" +
        stderr.GetAwaiter().GetResult().Trim());
}
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS291: " + reason);
}
sealed record ProcessResult(int ExitCode, string Output);
