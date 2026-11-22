#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string SourceExporter = "docs/discovery/qualification/ExportM8PortableDiscoverySkill.cs";
const string SelfPath = "verify-export.cs";
const string ReceiptPath = "portable-attestation.json";
const string ReceiptKind = "math-discovery-portable-integrity-sha256/1";
const string Skill = ".agents/skills/dw-tools-math";
string[] expected = [
    "Directory.Build.props",
    "NuGet.Config",
    "PORTABLE-README.md",
    Skill + "/SKILL.md",
    Skill + "/references/examples.md",
    Skill + "/references/index.md",
    Skill + "/references/manifest.json",
    Skill + "/references/selected-capabilities.md",
    Skill + "/references/skill-version.json",
    "examples/net11/Program.cs",
    "examples/net11/consumer.csproj",
    "global.json",
    SelfPath
];
Array.Sort(expected, StringComparer.Ordinal);

if (args.Length == 2 && args[0] == "--verify")
{
    Verify(Path.GetFullPath(args[1]));
    Console.WriteLine("MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS");
    return;
}
if (args.Length == 2 && args[0] == "--export")
{
    var destination = Path.GetFullPath(args[1]);
    Need(Path.IsPathFullyQualified(args[1]) &&
        !Directory.Exists(destination) && !File.Exists(destination),
        "Export requires an absolute, not-yet-existing destination.");
    RequirePriorEvidence();
    ExportWithReceipt(destination);
    Verify(destination);
    Console.WriteLine("MATH-RS283-PORTABLE-ATTESTED|files=13|receipt=unsigned|" +
        "packages=not-included|brokerReady=false|PASS");
    return;
}
if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check, --export <absolute-new-dir> or --verify <dir>.");

RequirePriorEvidence();
var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs283-discovery-integrity", Guid.NewGuid().ToString("N")));
var first = Path.Combine(root, "first");
var second = Path.Combine(root, "second");
ExportWithReceipt(first);
ExportWithReceipt(second);
Verify(first);
Verify(second);
var a = TrackedPaths(first);
var b = TrackedPaths(second);
Need(a.Length == 14 && a.SequenceEqual(b, StringComparer.Ordinal),
    "Two independent attested exports differ in file membership.");
foreach (var item in a)
    Need(Hash(Path.Combine(first, item.Replace('/', Path.DirectorySeparatorChar))) ==
        Hash(Path.Combine(second, item.Replace('/', Path.DirectorySeparatorChar))),
        "Two independent attested exports differ: " + item);
var standalone = RunDotnet(first, "run", "--file", SelfPath, "--",
    "--verify", first);
Need(standalone.Contains("MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS",
    StringComparison.Ordinal), "Independent exported verifier did not accept its own clean bundle.");

var selected = Path.Combine(second, Skill, "references", "manifest.json");
var saved = File.ReadAllBytes(selected);
File.AppendAllText(selected, " ", new UTF8Encoding(false));
Reject("altered-selected-manifest", second);
File.WriteAllBytes(selected, saved);
Verify(second);

var example = Path.Combine(second, "examples", "net11", "Program.cs");
saved = File.ReadAllBytes(example);
File.Delete(example);
Reject("missing-package-only-example", second);
File.WriteAllBytes(example, saved);
Verify(second);

var receiptFile = Path.Combine(second, ReceiptPath);
var originalReceipt = File.ReadAllText(receiptFile);
File.WriteAllText(receiptFile,
    originalReceipt.Replace("math-discovery-portable-integrity-sha256/1",
        "math-discovery-portable-integrity-sha256/2", StringComparison.Ordinal),
    new UTF8Encoding(false));
Reject("altered-receipt-identity", second);
File.WriteAllText(receiptFile, originalReceipt, new UTF8Encoding(false));
Verify(second);

Console.WriteLine("MATH-RS283-PORTABLE-INTEGRITY|exportPairs=2|files=14|" +
    "trackedHashes=13|negative=3|independentVerifier=true|" +
    "sourcePackagesUntouched=true|receipt=unsigned|brokerReady=false|PASS");

void RequirePriorEvidence()
{
    foreach (var (id, priorFailures, firstPass) in new[]
    {
        ("RS281", 3, false),
        ("RS282", 0, true)
    })
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(
            ".aura/workflow/reports/" + id + "/attempt-001.json"));
        var report = doc.RootElement;
        var prior = report.GetProperty("priorInvocationFailures");
        Need(report.GetProperty("runId").GetString() == id &&
            report.GetProperty("attempt").GetInt32() == 1 &&
            report.GetProperty("outcome").GetString() == "succeeded" &&
            report.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
            prior.GetProperty("count").GetInt32() == priorFailures &&
            prior.GetProperty("firstPassSucceeded").GetBoolean() == firstPass,
            "Qualified portable export lineage changed: " + id);
    }
}

void ExportWithReceipt(string destination)
{
    Need(!Directory.Exists(destination) && !File.Exists(destination),
        "Attested export must never overwrite an existing destination.");
    Need(File.Exists(SourceExporter) && File.Exists(
        "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs"),
        "Qualified source exporter or attestation source is missing.");
    var stdout = RunDotnet(Path.GetFullPath("."), "run", "--file", SourceExporter,
        "--", "--export", destination);
    Need(stdout.Contains(
        "MATH-RS281-PORTABLE-EXPORTED|files=12|privatePackages=not-included|authorization=runtime-only",
        StringComparison.Ordinal), "The original twelve-file Math exporter refused.");
    var source = File.ReadAllText(
        "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs")
        .Replace("\r\n", "\n", StringComparison.Ordinal);
    File.WriteAllText(Path.Combine(destination, SelfPath), source, new UTF8Encoding(false));
    var actual = TrackedPaths(destination);
    Need(actual.Length == expected.Length &&
        actual.SequenceEqual(expected, StringComparer.Ordinal),
        "Attested export differs from the original twelve-file canonical membership.");
    var receipt = new
    {
        schemaVersion = 1,
        kind = ReceiptKind,
        owner = "math",
        protocol = "dw.discovery/1.0.0-draft.1",
        sourceQualification = "RS281",
        linuxQualification = "RS282",
        signatureStatus = "unsigned",
        authorization = "runtime-only",
        files = expected.Select(path => new
        {
            path,
            sha256 = Hash(Path.Combine(destination,
                path.Replace('/', Path.DirectorySeparatorChar)))
        }).ToArray()
    };
    File.WriteAllText(Path.Combine(destination, ReceiptPath),
        JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }) +
        "\n", new UTF8Encoding(false));
}

void Verify(string directory)
{
    Need(Directory.Exists(directory), "Portable bundle directory does not exist.");
    var receiptFile = Path.Combine(directory, ReceiptPath);
    Need(File.Exists(receiptFile), "Portable integrity receipt is missing.");
    using var document = JsonDocument.Parse(File.ReadAllText(receiptFile));
    var receipt = document.RootElement;
    Need(receipt.GetProperty("schemaVersion").GetInt32() == 1 &&
        receipt.GetProperty("kind").GetString() == ReceiptKind &&
        receipt.GetProperty("owner").GetString() == "math" &&
        receipt.GetProperty("protocol").GetString() == "dw.discovery/1.0.0-draft.1" &&
        receipt.GetProperty("sourceQualification").GetString() == "RS281" &&
        receipt.GetProperty("linuxQualification").GetString() == "RS282" &&
        receipt.GetProperty("signatureStatus").GetString() == "unsigned" &&
        receipt.GetProperty("authorization").GetString() == "runtime-only",
        "Portable receipt provenance, schema or authorization changed.");
    var files = receipt.GetProperty("files").EnumerateArray().ToArray();
    Need(files.Length == expected.Length, "Portable receipt file cardinality differs.");
    var actual = TrackedPaths(directory);
    Need(actual.Length == expected.Length + 1 &&
        actual.SequenceEqual(expected.Append(ReceiptPath).OrderBy(
            path => path, StringComparer.Ordinal), StringComparer.Ordinal),
        "Portable bundle has missing, extra or renamed files.");
    for (var i = 0; i < expected.Length; i++)
    {
        var path = expected[i];
        Need(files[i].GetProperty("path").GetString() == path,
            "Portable receipt file order or path differs.");
        var absolute = Path.Combine(directory,
            path.Replace('/', Path.DirectorySeparatorChar));
        Need(File.Exists(absolute) &&
            (File.GetAttributes(absolute) & FileAttributes.ReparsePoint) == 0,
            "Portable source is absent or a link: " + path);
        Need(files[i].GetProperty("sha256").GetString() == Hash(absolute),
            "Portable file hash drift: " + path);
    }
    var manifest = Path.Combine(directory, Skill, "references", "manifest.json");
    using var doc = JsonDocument.Parse(File.ReadAllText(manifest));
    var root = doc.RootElement;
    Need(root.GetProperty("capabilities").GetArrayLength() == 4 &&
        root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        root.GetProperty("capabilities").EnumerateArray().All(item =>
            item.GetProperty("exposure").GetString() == "AgentCallable" &&
            item.GetProperty("execution").GetProperty("authorization").GetString() ==
                "runtime-only"), "Portable manifest granted unqualified authority.");
    Need(File.ReadAllText(Path.Combine(directory, Skill, "SKILL.md"))
        .Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal),
        "Portable skill lost BrokerReady refusal.");
}

string[] TrackedPaths(string directory) =>
    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
        .Select(path => Path.GetRelativePath(directory, path).Replace('\\', '/'))
        .OrderBy(path => path, StringComparer.Ordinal).ToArray();

void Reject(string label, string directory)
{
    try
    {
        Verify(directory);
    }
    catch (InvalidDataException)
    {
        Console.WriteLine("MATH-RS283-REFUSAL|" + label + "|PASS");
        return;
    }
    throw new InvalidDataException("RS283 admitted mutation: " + label);
}

string RunDotnet(string workingDirectory, params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workingDirectory, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS283 cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS283 dotnet source export / verifier timed out.");
    }
    var output = stdout.GetAwaiter().GetResult().Trim();
    var error = stderr.GetAwaiter().GetResult().Trim();
    Need(process.ExitCode == 0, "dotnet failed: " +
        string.Join(" ", arguments) + "\n" + output + "\n" + error);
    return output;
}
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS283: " + reason);
}
