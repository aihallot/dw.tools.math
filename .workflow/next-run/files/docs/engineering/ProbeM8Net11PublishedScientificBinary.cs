#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS264 qualifies the Windows net11 independent binary consumer.");

const string Fixture = "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs";
var packages = new[]
{
    (Id: "dw.tools.math.scientific", Version: "20260.20261105.1001",
     Archive: "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f",
     Binary: "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c",
     Bytes: 37265L)
};
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must point to an existing absolute private feed.");
Require(File.Exists(Fixture), "Original RS156 independent Scientific consumer fixture is missing.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var root = Path.GetFullPath("build/artifacts/m8-rs264-ir-binary-net11");
var copiedFeed = Path.Combine(root, "feed");
Directory.CreateDirectory(copiedFeed);
foreach (var item in packages)
{
    var original = Path.Combine(feed!, item.Id + "." + item.Version + ".nupkg");
    Require(File.Exists(original) && new FileInfo(original).Length == item.Bytes &&
        (File.GetAttributes(original) & FileAttributes.ReparsePoint) == 0 &&
        Hash(original) == item.Archive, "Published archive identity mismatch: " + item.Id);
    using (var zip = ZipFile.OpenRead(original))
    {
        var dll = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + item.Id + ".dll");
        using var stream = dll.Open();
        Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == item.Binary,
            "Published DLL SHA-256 mismatch: " + item.Id);
    }
    var copy = Path.Combine(copiedFeed, Path.GetFileName(original));
    File.Copy(original, copy, overwrite: true);
    Require(Hash(copy) == item.Archive, "Isolated feed copy differs: " + item.Id);
}
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(root, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.tools.math.scientific\"" +
    " Version=\"[20260.20261105.1001]\" /></ItemGroup></Project>\n");
File.Copy(Fixture, Path.Combine(root, "Program.cs"), overwrite: true);
string Run(params string[] args)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in args) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS264 cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS264 dotnet " + string.Join(" ", args) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Incorrect isolated SDK11 selection.");
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", copiedFeed, "--packages", Path.Combine(root, "private-package-cache"),
    "-p:NuGetAudit=false");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--",
    packages[0].Binary);
Require(output.Contains("MATH-SCIENTIFIC-BINARY|PASS|15", StringComparison.Ordinal) &&
    output.Contains("MATH-SCIENTIFIC-CHECK|loaded-package-sha256|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-SCIENTIFIC-CHECK|integer-gcd|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-SCIENTIFIC-CHECK|special-log-domain|PASS", StringComparison.Ordinal),
    "Original RS156 Scientific fifteen-check binary fixture failed on net11.");
foreach (var item in packages)
{
    var name = item.Id + "." + item.Version + ".nupkg";
    Require(Hash(Path.Combine(feed!, name)) == item.Archive &&
        Hash(Path.Combine(copiedFeed, name)) == item.Archive,
        "Original or isolated immutable archive changed: " + item.Id);
}
Console.WriteLine("MATH-RS264-SCIENTIFIC-NET11|packages=1|sdk=11.0.100|tfm=net11.0|" +
    "binaryHashes=matched|checks=15|archivesUnchanged=true|publication=false|PASS");
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS264: " + reason);
}
