#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS254 qualifies the Windows net11 package graph.");

const string Fixture = "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs";
var packages = new[]
{
    (Id: "dw.quantities.standard", Version: "20260.20261103.1001",
     Archive: "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75",
     Binary: "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d",
     Bytes: 13604L),
    (Id: "dw.quantities.expression", Version: "20260.20261102.1001",
     Archive: "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae",
     Binary: "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1",
     Bytes: 13951L),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
     Bytes: 14747L)
};
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must identify an existing absolute private feed.");
Require(File.Exists(Fixture), "Existing RS143 ten-check fixture is missing.");
string Hash(string file) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
foreach (var package in packages)
{
    var archive = Path.Combine(feed!, package.Id + "." + package.Version + ".nupkg");
    Require(File.Exists(archive) && new FileInfo(archive).Length == package.Bytes &&
        (File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0 &&
        Hash(archive) == package.Archive, "Published archive mismatch: " + package.Id);
    using var zip = ZipFile.OpenRead(archive);
    var entry = zip.Entries.Single(x =>
        x.FullName == "lib/net10.0/" + package.Id + ".dll");
    using var stream = entry.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == package.Binary,
        "Published net10 DLL mismatch: " + package.Id);
}
var root = Path.GetFullPath("build/artifacts/m8-rs254-published-standard-net11");
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\n \"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}\n}\n");
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
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.quantities.standard\"" +
    " Version=\"[20260.20261103.1001]\" /></ItemGroup></Project>\n");
File.Copy(Fixture, Path.Combine(root, "Program.cs"), overwrite: true);
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS254 cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS254 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated net11 SDK identity mismatch.");
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source",
    feed!, "--packages", Path.Combine(root, "private-package-cache"), "-p:NuGetAudit=false");
var lockFile = Path.Combine(root, "packages.lock.json");
Require(File.Exists(lockFile), "Initial restore did not create a package lock.");
using (var document = JsonDocument.Parse(File.ReadAllText(lockFile)))
{
    var frameworks = document.RootElement.GetProperty("dependencies");
    Require(frameworks.EnumerateObject().Count() == 1 &&
        frameworks.TryGetProperty("net11.0", out var framework),
        "Lock must contain exactly net11.0 dependencies.");
    var locked = framework.EnumerateObject().ToDictionary(
        item => item.Name, item => item.Value, StringComparer.OrdinalIgnoreCase);
    Require(locked.Count == packages.Length, "Lock closure must contain exactly three packages.");
    foreach (var package in packages)
    {
        Require(locked.TryGetValue(package.Id, out var item) &&
            item.GetProperty("resolved").GetString() == package.Version,
            "Locked package version mismatch: " + package.Id);
    }
    Require(locked["dw.quantities.standard"].GetProperty("type").GetString() == "Direct" &&
        locked["dw.quantities.expression"].GetProperty("type").GetString() == "Transitive" &&
        locked["dw.quantities"].GetProperty("type").GetString() == "Transitive",
        "The package lock must preserve exact direct/transitive dependency topology.");
}
var lockHash = Hash(lockFile);
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source",
    feed!, "--packages", Path.Combine(root, "private-package-cache"),
    "--locked-mode", "-p:NuGetAudit=false");
Require(Hash(lockFile) == lockHash, "Locked restore modified packages.lock.json.");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--",
    packages[0].Binary, packages[1].Binary, packages[2].Binary);
Require(output.Contains("MATH-STANDARD-BINARY|PASS|10", StringComparison.Ordinal) &&
    output.Contains("MATH-STANDARD-CHECK|loaded-dll-hashes|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-STANDARD-CHECK|inherited-catalog-58|PASS", StringComparison.Ordinal),
    "Original RS143 ten-check consumer failed on net11.");
foreach (var package in packages)
    Require(Hash(Path.Combine(feed!, package.Id + "." + package.Version + ".nupkg")) ==
        package.Archive, "Immutable published archive changed: " + package.Id);
Console.WriteLine("MATH-RS254-STANDARD-NET11|packages=3|sdk=11.0.100|tfm=net11.0|" +
    "binaryHashes=matched|checks=10|lockedRestore=pass|closure=3|" +
    "publishedArchivesUnchanged=true|" +
    "publication=false|productionMutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS254: " + message);
}
