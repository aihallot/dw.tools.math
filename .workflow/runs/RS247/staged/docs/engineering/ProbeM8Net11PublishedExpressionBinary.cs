#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS247 qualifies the Windows SDK11 binary consumer.");

const string Package = "dw.quantities.expression";
const string BasePackage = "dw.quantities";
const string Version = "20260.20261102.1001";
const string ArchiveHash = "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
const string BaseArchiveHash = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string BinaryHash = "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
const string BaseBinaryHash = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must identify an existing absolute private feed.");
string Hash(string file) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
var packages = new[]
{
    (Id: Package, Archive: ArchiveHash, Binary: BinaryHash, Size: 13951L),
    (Id: BasePackage, Archive: BaseArchiveHash, Binary: BaseBinaryHash, Size: 14747L)
};
foreach (var item in packages)
{
    var path = Path.Combine(feed!, item.Id + "." + Version + ".nupkg");
    Require(File.Exists(path) && new FileInfo(path).Length == item.Size &&
        (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0 &&
        Hash(path) == item.Archive, "Immutable archive mismatch: " + item.Id);
    using var zip = ZipFile.OpenRead(path);
    var entry = zip.Entries.Single(x => x.FullName == "lib/net10.0/" + item.Id + ".dll");
    using var stream = entry.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == item.Binary,
        "Immutable DLL mismatch: " + item.Id);
}
var root = Path.GetFullPath("build/artifacts/m8-rs247-published-expression-net11");
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
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"" + Package +
    "\" Version=\"[" + Version + "]\" /></ItemGroup></Project>\n");
const string Fixture = "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs";
Require(File.Exists(Fixture), "Original six-check expression fixture is missing.");
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
        throw new InvalidOperationException("RS247 cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS247 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK11 selection mismatch.");
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source",
    feed!, "--packages", Path.Combine(root, "private-package-cache"), "-p:NuGetAudit=false");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--", BinaryHash, BaseBinaryHash);
Require(output.Contains("MATH-EXPRESSION-BINARY|PASS|6", StringComparison.Ordinal) &&
    output.Contains("MATH-EXPRESSION-CHECK|loaded-binaries|PASS", StringComparison.Ordinal),
    "Published source-free expression binary consumer failed on net11.");
foreach (var item in packages)
    Require(Hash(Path.Combine(feed!, item.Id + "." + Version + ".nupkg")) == item.Archive,
        "Original immutable package changed: " + item.Id);
Console.WriteLine("MATH-RS247-NET11-EXPRESSION|packages=2|version=" + Version +
    "|binaryHashes=matched|checks=6|archiveUnchanged=true|publication=false|PASS");
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS247: " + message);
}
