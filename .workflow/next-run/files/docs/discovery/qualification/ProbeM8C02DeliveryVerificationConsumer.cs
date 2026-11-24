#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Attester = "docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs";
const string Receipt = "portable-attestation.json";
const string Verifier = "verify-export.cs";
const string Example = "examples/net11/consumer.csproj";
const string ProgramFile = "examples/net11/Program.cs";
const string Manifest = ".agents/skills/dw-tools-math/references/manifest.json";
const string ExpectedProgram = "M8-DISCOVERY-CLR-SEED|bindings=4|examples=5|positiveComposition=2|brokerReady=0|scope=package-reference-consumer";
var packages = new[]
{
    (Id: "dw.tools.math.composition", Version: "20260.20261107.1001",
        Archive: "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f",
        Binary: "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252"),
    (Id: "dw.tools.math.ir", Version: "20260.20261104.1001",
        Archive: "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
        Binary: "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
        Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
        Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
var canonical = new[] { Attester,
    "docs/discovery/qualification/m8-discovery-manifest.json",
    ".agents/skills/dw-tools-math/SKILL.md" };
var originalHashes = canonical.ToDictionary(path => path, Sha, StringComparer.Ordinal);
var repo = Path.GetFullPath(".");
var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs311-package-delivery", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
var exported = Path.Combine(root, "exported");
var received = Path.Combine(root, "received");
var host = Path.Combine(root, "verifier-host");
var runtime = Path.Combine(root, "package-consumer");
var localFeed = Path.Combine(root, "isolated-feed");
var packageCache = Path.Combine(root, "isolated-nuget-cache");

var export = Run(repo, "run", "--file", Attester, "--", "--export", exported);
Need(export.ExitCode == 0 &&
    export.Output.Contains("MATH-RS283-PORTABLE-ATTESTED|files=13|receipt=unsigned|", StringComparison.Ordinal) &&
    export.Output.Contains("brokerReady=false|PASS", StringComparison.Ordinal),
    "Attested public export failed: " + export.Output);
var names = Directory.GetFiles(exported, "*", SearchOption.AllDirectories)
    .Select(path => Path.GetRelativePath(exported, path).Replace('\\', '/'))
    .OrderBy(name => name, StringComparer.Ordinal).ToArray();
Need(names.Length == 14 && names.Contains(Receipt, StringComparer.Ordinal) &&
    names.Contains(Example, StringComparer.Ordinal) &&
    names.Contains(ProgramFile, StringComparer.Ordinal) &&
    names.Contains(Verifier, StringComparer.Ordinal),
    "Attested delivery does not include the complete package-only example.");
void CopyBundle(string source, string destination)
{
    Directory.CreateDirectory(destination);
    foreach (var name in names)
    {
        var relative = name.Replace('/', Path.DirectorySeparatorChar);
        var target = Path.Combine(destination, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(Path.Combine(source, relative), target);
        Need(Sha(target) == Sha(Path.Combine(source, relative)),
            "Copied delivery file differs: " + name);
    }
}
CopyBundle(exported, received);
Directory.CreateDirectory(host);
File.Copy(Path.Combine(received, Verifier), Path.Combine(host, Verifier));
File.Copy(Path.Combine(received, "global.json"), Path.Combine(host, "global.json"));
ProcessResult Verify() => Run(host, "run", "--file", Verifier, "--", "--verify", received);
void CheckReceived()
{
    var result = Verify();
    Need(result.ExitCode == 0 &&
        result.Output.Contains("MATH-RS283-PORTABLE-VERIFY|files=13|hashes=matched|brokerReady=false|PASS",
            StringComparison.Ordinal),
        "Independent copied verifier refused pristine delivery: " + result.Output);
}
CheckReceived();
using (var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(received, Receipt))))
using (var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(received,
    Manifest.Replace('/', Path.DirectorySeparatorChar)))))
{
    var r = receipt.RootElement;
    var m = manifest.RootElement;
    Need(r.GetProperty("signatureStatus").GetString() == "unsigned" &&
        r.GetProperty("authorization").GetString() == "runtime-only" &&
        r.GetProperty("files").GetArrayLength() == 13 &&
        m.GetProperty("capabilities").GetArrayLength() == 4 &&
        m.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        m.GetProperty("capabilities").EnumerateArray().All(cap =>
            cap.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only"),
        "Received portable skill improperly promotes authenticity or execution authority.");
}
var refusals = 0;
void RefuseChanged(string name)
{
    var target = Path.Combine(received, name.Replace('/', Path.DirectorySeparatorChar));
    var saved = File.ReadAllBytes(target);
    try
    {
        File.AppendAllText(target, " ", new UTF8Encoding(false));
        var denied = Verify();
        Need(denied.ExitCode != 0 &&
            denied.Output.Contains("Portable file hash drift: " + name, StringComparison.Ordinal),
            "Changed delivered package-only example escaped refusal: " + name + " " + denied.Output);
        refusals++;
    }
    finally { File.WriteAllBytes(target, saved); }
    CheckReceived();
}
RefuseChanged(ProgramFile);
RefuseChanged(Example);
Need(refusals == 2, "Two independent delivered source/project refusals required.");
var projectText = File.ReadAllText(Path.Combine(received,
    Example.Replace('/', Path.DirectorySeparatorChar)));
Need(projectText.Contains("<PackageReference Include=\"dw.tools.math.composition\"",
        StringComparison.Ordinal) &&
    !projectText.Contains("<ProjectReference", StringComparison.OrdinalIgnoreCase),
    "Delivered example is not a pinned, source-free PackageReference consumer.");

// Keep the checksum-verified received bundle immutable; builds create new files.
CopyBundle(received, runtime);
var sdk = Run(runtime, "--version");
Need(sdk.ExitCode == 0 && sdk.Output.Trim() == "11.0.100",
    "Package-only consumer requires the pinned SDK 11.0.100.");
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "A physical NUGET_LOCAL feed is required.");
Directory.CreateDirectory(localFeed);
foreach (var item in packages)
{
    var filename = item.Id + "." + item.Version + ".nupkg";
    var source = Path.Combine(feed!, filename);
    Need(File.Exists(source) && new FileInfo(source).Length > 0 &&
        (File.GetAttributes(source) & FileAttributes.ReparsePoint) == 0 &&
        Sha(source) == item.Archive,
        "Immutable published package differs: " + item.Id);
    using (var archive = ZipFile.OpenRead(source))
    {
        var dll = archive.Entries.Single(entry =>
            entry.FullName == "lib/net10.0/" + item.Id + ".dll");
        using var stream = dll.Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(stream)) == item.Binary,
            "Embedded physical DLL differs: " + item.Id);
    }
    File.Copy(source, Path.Combine(localFeed, filename));
    Need(Sha(Path.Combine(localFeed, filename)) == item.Archive,
        "Isolated feed copied a different package: " + item.Id);
}
RequireSuccess(Run(runtime, "restore", Example, "--configfile", "NuGet.Config",
    "--source", localFeed, "--packages", packageCache, "-p:NuGetAudit=false"),
    "Fresh isolated package-only restore");
var locked = Path.Combine(runtime, "examples", "net11", "packages.lock.json");
Need(File.Exists(locked), "Package-only restored lockfile is missing.");
using (var lockfile = JsonDocument.Parse(File.ReadAllText(locked)))
{
    var groups = lockfile.RootElement.GetProperty("dependencies");
    Need(groups.EnumerateObject().Count() == 1,
        "The delivered example restored an unexpected target framework.");
    if (!groups.TryGetProperty("net11.0", out var net11))
        throw new InvalidDataException("RS311: Missing net11.0 locked group.");
    var resolved = net11.EnumerateObject()
        .ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Need(resolved.Count == packages.Length, "Private package closure differs.");
    foreach (var item in packages)
    {
        Need(resolved.TryGetValue(item.Id, out var dep) &&
            dep.GetProperty("resolved").GetString() == item.Version &&
            dep.GetProperty("type").GetString() ==
                (item.Id == "dw.tools.math.composition" ? "Direct" : "Transitive"),
            "Unexpected locked package version or dependency: " + item.Id);
    }
}
var lockedHash = Sha(locked);
RequireSuccess(Run(runtime, "restore", Example, "--configfile", "NuGet.Config",
    "--source", localFeed, "--packages", packageCache, "--locked-mode",
    "-p:NuGetAudit=false"), "Locked isolated package-only restore");
Need(Sha(locked) == lockedHash, "Locked restore modified package graph.");
RequireSuccess(Run(runtime, "build", Example, "-c", "Release", "--no-restore"),
    "Package-only build");
var output = Path.Combine(runtime, "build", "bin", "consumer", "Release", "net11.0");
foreach (var item in packages)
    Need(Sha(Path.Combine(output, item.Id + ".dll")) == item.Binary,
        "Copied package-only consumer DLL differs: " + item.Id);
var result = Run(runtime, "run", "--project", Example,
    "-c", "Release", "--no-build", "--no-restore");
Need(result.ExitCode == 0 &&
    result.Output.Contains(ExpectedProgram, StringComparison.Ordinal),
    "The five bounded package-only Math examples failed: " + result.Output);
CheckReceived();
foreach (var item in packages)
{
    var filename = item.Id + "." + item.Version + ".nupkg";
    Need(Sha(Path.Combine(feed!, filename)) == item.Archive &&
        Sha(Path.Combine(localFeed, filename)) == item.Archive,
        "Original or isolated private feed archive was modified: " + item.Id);
}
foreach (var path in canonical)
    Need(Sha(path) == originalHashes[path],
        "Canonical Math input changed during package-only verification: " + path);
Console.WriteLine("MATH-RS311-DELIVERY-VERIFICATION|attested=14|receiptHashes=13|" +
    "copiedRefusals=2|net11=11.0.100|lockedRestore=true|packages=3|" +
    "publishedDlls=3|examples=5|sourceFree=true|unsigned=true|brokerReady=0|PASS");

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS311: " + reason);
}
static void RequireSuccess(ProcessResult result, string label) =>
    Need(result.ExitCode == 0, label + ": " + result.Output);
static string Sha(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static ProcessResult Run(string workingDirectory, params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workingDirectory, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in arguments) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS311: Could not launch dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS311: Bounded dotnet child process timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
