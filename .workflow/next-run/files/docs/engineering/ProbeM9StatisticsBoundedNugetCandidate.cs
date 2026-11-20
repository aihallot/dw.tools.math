#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Reflection;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS269 candidate qualification uses the Windows private feed.");

const string Version = "20260.20261120.1001";
const string DependencyVersion = "20260.20261102.1001";
const string DependencySha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string DependencyDllSha = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string Id = "dw.tools.math.statistics";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "Original private feed must be an absolute directory.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var originalDependency = Path.Combine(feed!, "dw.quantities." + DependencyVersion + ".nupkg");
Need(File.Exists(originalDependency) && Hash(originalDependency) == DependencySha,
    "The published Quantities dependency has changed or is missing.");
using (var zip = ZipFile.OpenRead(originalDependency))
{
    var entry = zip.Entries.Single(x => x.FullName == "lib/net10.0/dw.quantities.dll");
    using var stream = entry.Open();
    Need(Convert.ToHexStringLower(SHA256.HashData(stream)) == DependencyDllSha,
        "Original Quantities DLL identity differs.");
}
Need(File.Exists("src/projects/dw.tools.math.statistics/SplitMix64Random.cs"),
    "Missing canonical Statistics source.");
var sourcePath = "src/projects/dw.tools.math.statistics";
var sources = Directory.GetFiles(sourcePath, "*.cs", SearchOption.TopDirectoryOnly);
Need(sources.Length >= 8 && sources.Any(x => Path.GetFileName(x) == "SplitMix64Random.cs"),
    "Expected complete existing Statistics source family.");
var sourceCode = File.ReadAllText(Path.Combine(sourcePath, "SplitMix64Random.cs"));
Need(sourceCode.Contains("NextBoundedUInt64(", StringComparison.Ordinal) &&
    sourceCode.Contains("NextInt64Inclusive(", StringComparison.Ordinal) &&
    sourceCode.Contains("splitmix64-bounded-rejection/v1", StringComparison.Ordinal),
    "RS268 bounded API is not in source.");

var root = Path.GetFullPath("build/artifacts/m9-rs269-bounded-statistics-candidate");
var snapshotFeed = Path.Combine(root, "feed");
var sourceRoot = Path.Combine(root, "source");
var packageDirectory = Path.Combine(root, "candidate");
Directory.CreateDirectory(snapshotFeed);
Directory.CreateDirectory(sourceRoot);
Directory.CreateDirectory(packageDirectory);
var snapshot = Path.Combine(snapshotFeed, Path.GetFileName(originalDependency));
File.Copy(originalDependency, snapshot, overwrite: true);
Need(Hash(snapshot) == DependencySha, "Quantities feed snapshot changed.");
foreach (var file in sources)
    File.Copy(file, Path.Combine(sourceRoot, Path.GetFileName(file)), overwrite: true);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework>" +
    "<LangVersion>14.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<Deterministic>true</Deterministic>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(root, "candidate.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>" +
    "<AssemblyName>dw.tools.math.statistics</AssemblyName>" +
    "<RootNamespace>Dw.Tools.Math.Statistics</RootNamespace>" +
    "<PackageId>" + Id + "</PackageId><Version>" + Version + "</Version>" +
    "<AssemblyVersion>1.0.0.0</AssemblyVersion><FileVersion>1.0.0.0</FileVersion>" +
    "<Authors>dw.tools.math</Authors><IsPackable>true</IsPackable>" +
    "<NuGetAudit>false</NuGetAudit><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.quantities\"" +
    " Version=\"[" + DependencyVersion + "]\" /></ItemGroup></Project>\n");

string Run(params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("dotnet start failed.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS269 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Need(Run("--version").StartsWith("10.0.", StringComparison.Ordinal),
    "Candidate must be produced with pinned production .NET 10 SDK family.");
Run("restore", "candidate.csproj", "--configfile", "NuGet.Config", "--source",
    snapshotFeed, "--packages", Path.Combine(root, "private-package-cache"), "-p:NuGetAudit=false");
Run("pack", "candidate.csproj", "-c", "Release", "--no-restore", "--output", packageDirectory);
var archive = Path.Combine(packageDirectory, Id + "." + Version + ".nupkg");
Need(File.Exists(archive), "Versioned candidate archive missing.");
Need(!File.Exists(Path.Combine(feed!, Path.GetFileName(archive))),
    "Candidate version already exists in immutable publication feed.");
Need(new FileInfo(archive).Length > 0, "Candidate archive is empty.");
using (var zip = ZipFile.OpenRead(archive))
{
    var dllEntry = zip.Entries.Single(x => x.FullName == "lib/net10.0/" + Id + ".dll");
    var nuspec = zip.Entries.Single(x => x.FullName == Id + ".nuspec");
    using var descriptor = nuspec.Open();
    var xml = XDocument.Load(descriptor);
    XNamespace ns = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";
    var metadata = xml.Root!.Element(ns + "metadata")!;
    Need(metadata.Element(ns + "id")?.Value == Id &&
        metadata.Element(ns + "version")?.Value == Version,
        "NuGet candidate identity differs.");
    var dependencies = metadata.Element(ns + "dependencies")!
        .Descendants(ns + "dependency").ToArray();
    Need(dependencies.Length == 1 &&
        dependencies[0].Attribute("id")?.Value == "dw.quantities" &&
        dependencies[0].Attribute("version")?.Value == "[" + DependencyVersion + "]",
        "Candidate must depend on exactly the published Quantities version.");
    using var binary = dllEntry.Open();
    using var buffer = new MemoryStream();
    binary.CopyTo(buffer);
    var assemblyBytes = buffer.ToArray();
    Need(assemblyBytes.Length > 0, "Candidate binary missing.");
    var assemblySha = Convert.ToHexStringLower(SHA256.HashData(assemblyBytes));
    Console.WriteLine("MATH-RS269-CANDIDATE-DLL|sha256=" + assemblySha);
}
Need(Hash(originalDependency) == DependencySha && Hash(snapshot) == DependencySha,
    "The immutable dependency was modified.");
Console.WriteLine("MATH-RS269-CANDIDATE|id=" + Id + "|version=" + Version +
    "|archiveSha256=" + Hash(archive) + "|packages=2|tfm=net10.0|" +
    "originalDependencyUnchanged=true|published=false|consumerNet11Qualified=false|PASS");

static void Need(bool value, string reason)
{
    if (!value) throw new InvalidDataException("RS269: " + reason);
}
