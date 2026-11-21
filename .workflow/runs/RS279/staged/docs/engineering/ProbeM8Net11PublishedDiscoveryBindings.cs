#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS279 qualifies a Windows net11 independent published-package consumer.");

const string Fixture = "tests/consumers/m8-discovery-clr-seed/Program.cs";
const string Bindings = "docs/discovery/qualification/m8-selected-clr-bindings.json";
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
var expectedBindings = new[]
{
    (Id: "math.quantity.exact-pipeline.evaluate", Assembly: "dw.tools.math.composition",
     Type: "Dw.Tools.Math.Composition.ExactQuantityPipeline", Method: "Evaluate"),
    (Id: "math.replay.try-evaluate", Assembly: "dw.tools.math.composition",
     Type: "Dw.Tools.Math.Composition.ExactReplayRunner", Method: "TryRun"),
    (Id: "math.ir.canonical.encode", Assembly: "dw.tools.math.ir",
     Type: "Dw.Tools.Math.Ir.IrCanonicalJsonCodec", Method: "Encode"),
    (Id: "math.ir.hash.semantic", Assembly: "dw.tools.math.ir",
     Type: "Dw.Tools.Math.Ir.IrCanonicalHashes", Method: "SemanticStructuralSha256")
};
Require(File.Exists(Fixture) && File.Exists(Bindings), "Selected source consumer or binding receipt missing.");
using (var selection = JsonDocument.Parse(File.ReadAllText(Bindings)))
{
    var root = selection.RootElement;
    Require(root.GetProperty("status").GetString() == "selected-clr-fingerprints-only" &&
        root.GetProperty("brokerReadyCount").GetInt32() == 0 &&
        !root.GetProperty("packageConsumerQualified").GetBoolean() &&
        !root.GetProperty("auraIntegrationVerified").GetBoolean(),
        "Historical source-only selection or runtime authorization boundary changed.");
    var chosen = root.GetProperty("bindings").EnumerateArray().ToArray();
    Require(chosen.Length == 4, "Exactly four approved CLR bindings required.");
    foreach (var expected in expectedBindings)
    {
        var match = chosen.SingleOrDefault(x =>
            x.GetProperty("capabilityId").GetString() == expected.Id);
        Require(match.ValueKind == JsonValueKind.Object &&
            match.GetProperty("assembly").GetString() == expected.Assembly &&
            match.GetProperty("assemblyVersion").GetString() == "0.3.0.0" &&
            match.GetProperty("type").GetString() == expected.Type &&
            match.GetProperty("member").GetString() == expected.Method &&
            !match.GetProperty("brokerReady").GetBoolean(),
            "Selected CLR identity changed: " + expected.Id);
        var signature = match.GetProperty("canonicalSignature").GetString() ?? "";
        var fingerprint = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(signature)));
        Require(signature.StartsWith(expected.Assembly + "|0.3.0.0|" +
                expected.Type + "|" + expected.Method + "|", StringComparison.Ordinal) &&
            match.GetProperty("fingerprint").GetString() == fingerprint,
            "Selected fingerprint changed: " + expected.Id);
    }
}

var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute private feed.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var rootPath = Path.GetFullPath("build/artifacts/m8-rs279-published-discovery-net11");
var copiedFeed = Path.Combine(rootPath, "feed");
Directory.CreateDirectory(copiedFeed);
foreach (var item in packages)
{
    var source = Path.Combine(feed!, item.Id + "." + item.Version + ".nupkg");
    Require(File.Exists(source) && new FileInfo(source).Length > 0 &&
        (File.GetAttributes(source) & FileAttributes.ReparsePoint) == 0 &&
        Hash(source) == item.Archive, "Immutable published archive mismatch: " + item.Id);
    using (var zip = ZipFile.OpenRead(source))
    {
        var entry = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + item.Id + ".dll");
        using var bytes = entry.Open();
        Require(Convert.ToHexStringLower(SHA256.HashData(bytes)) == item.Binary,
            "Published embedded DLL mismatch: " + item.Id);
    }
    var copy = Path.Combine(copiedFeed, Path.GetFileName(source));
    File.Copy(source, copy, overwrite: true);
    Require(Hash(copy) == item.Archive, "Isolated package differs: " + item.Id);
}

File.WriteAllText(Path.Combine(rootPath, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(rootPath, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(rootPath, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(rootPath, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.tools.math.composition\"" +
    " Version=\"[20260.20261107.1001]\" /></ItemGroup></Project>\n");
File.Copy(Fixture, Path.Combine(rootPath, "Program.cs"), overwrite: true);

string Run(params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = rootPath, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS279 cannot start dotnet.");
    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS279 dotnet process timed out.");
    }
    var stdout = stdoutTask.GetAwaiter().GetResult();
    var stderr = stderrTask.GetAwaiter().GetResult();
    if (process.ExitCode != 0)
        throw new InvalidOperationException("RS279 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK11 selection differs.");
var packageCache = Path.Combine(rootPath, "private-package-cache");
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", copiedFeed, "--packages", packageCache, "-p:NuGetAudit=false");
var lockPath = Path.Combine(rootPath, "packages.lock.json");
Require(File.Exists(lockPath), "Published packages must produce a net11 NuGet lockfile.");
using (var lockDoc = JsonDocument.Parse(File.ReadAllText(lockPath)))
{
    var targetGroups = lockDoc.RootElement.GetProperty("dependencies");
    Require(targetGroups.EnumerateObject().Count() == 1 &&
        targetGroups.TryGetProperty("net11.0", out _), "Only net11.0 is admitted.");
    var targets = targetGroups.GetProperty("net11.0").EnumerateObject()
        .ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Require(targets.Count == 3, "Discovery binary closure must have three packages.");
    foreach (var item in packages)
    {
        Require(targets.TryGetValue(item.Id, out var dependency) &&
            dependency.GetProperty("resolved").GetString() == item.Version &&
            dependency.GetProperty("type").GetString() ==
                (item.Id == "dw.tools.math.composition" ? "Direct" : "Transitive"),
            "Published dependency closure differs: " + item.Id);
    }
}
var beforeLockedRestore = Hash(lockPath);
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", copiedFeed, "--packages", packageCache,
    "--locked-mode", "-p:NuGetAudit=false");
Require(Hash(lockPath) == beforeLockedRestore, "Locked restore changed the exact graph.");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var binaryRoot = Path.Combine(rootPath, "out", "bin", "Release", "net11.0");
foreach (var item in packages)
{
    var assemblyPath = Path.Combine(binaryRoot, item.Id + ".dll");
    Require(File.Exists(assemblyPath) && Hash(assemblyPath) == item.Binary,
        "Loaded-consumer dependency bytes changed: " + item.Id);
    var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
    Require(assemblyName.Name == item.Id && assemblyName.Version?.ToString() ==
        (item.Id == "dw.quantities" ? "0.2.0.0" : "0.3.0.0"),
        "Published CLR assembly identity drift: " + item.Id);
}
var stdout = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(stdout.Contains("M8-DISCOVERY-CLR-SEED|bindings=4|examples=5|positiveComposition=2|",
            StringComparison.Ordinal) &&
    stdout.Contains("brokerReady=0|scope=source-project-consumer", StringComparison.Ordinal),
    "Original four-selected-binding source fixture failed when compiled against published binaries.");
foreach (var item in packages)
{
    var name = item.Id + "." + item.Version + ".nupkg";
    Require(Hash(Path.Combine(feed!, name)) == item.Archive &&
        Hash(Path.Combine(copiedFeed, name)) == item.Archive,
        "Private source or isolated archive changed: " + item.Id);
}
Console.WriteLine("MATH-RS279-DISCOVERY-PUBLISHED-NET11|bindings=4|examples=5|packages=3|" +
    "sdk=11.0.100|lockedRestore=true|binaryHashes=matched|" +
    "sourceProjects=false|brokerReady=false|publication=false|PASS");

static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS279: " + reason);
}
