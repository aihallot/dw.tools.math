#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS244 qualifies the Windows SDK11 binary consumer.");

const string Package = "dw.quantities";
const string Version = "20260.20261102.1001";
const string ArchiveHash = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string BinaryHash = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must identify an existing absolute private feed.");
var archive = Path.Combine(feed!, Package + "." + Version + ".nupkg");
Require(File.Exists(archive) && new FileInfo(archive).Length == 14747,
    "Published archive is missing or differs in length.");
Require((File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0,
    "Published archive may not be a reparse point.");
string Hash(string file) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
Require(Hash(archive) == ArchiveHash, "Published archive SHA-256 differs from RS137.");
using (var zip = ZipFile.OpenRead(archive))
{
    var dll = zip.Entries.Single(x =>
        x.FullName == "lib/net10.0/dw.quantities.dll");
    using var stream = dll.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == BinaryHash,
        "Archived net10 DLL SHA-256 differs from RS137.");
}
var root = Path.GetFullPath("build/artifacts/m8-rs244-published-quantities-net11");
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
File.WriteAllText(Path.Combine(root, "Program.cs"), """
    using System;
    using System.Security.Cryptography;
    using dw.quantities;

    if (!OperatingSystem.IsWindows() || Environment.Version.Major != 11)
        throw new InvalidOperationException("Incorrect binary-consumer runtime.");
    var assembly = typeof(ExactRational).Assembly;
    if (assembly.GetName().Name != "dw.quantities")
        throw new InvalidOperationException("Unexpected package assembly identity.");
    var hash = Convert.ToHexStringLower(SHA256.HashData(
        System.IO.File.ReadAllBytes(assembly.Location)));
    if (hash != "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
        throw new InvalidOperationException("Loaded DLL differs from immutable package.");
    var sum = new ExactRational(1, 3) + new ExactRational(1, 6);
    if (sum.ToFractionString() != "1/2")
        throw new InvalidOperationException("Package exact arithmetic drift.");
    var refused = false;
    try { _ = new ExactRational(1, 0); }
    catch (DivideByZeroException) { refused = true; }
    if (!refused)
        throw new InvalidOperationException("Zero denominator was accepted.");
    Console.WriteLine("MATH-RS244-PUBLISHED-BINARY|os=windows|runtime=11|" +
        "assembly=dw.quantities|loadedHash=matched|sum=1/2|zeroDenominator=refused|PASS");
    """);
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS244 cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS244 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK11 selection mismatch.");
Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source",
    feed!, "--packages", Path.Combine(root, "private-package-cache"), "-p:NuGetAudit=false");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(output == "MATH-RS244-PUBLISHED-BINARY|os=windows|runtime=11|" +
    "assembly=dw.quantities|loadedHash=matched|sum=1/2|zeroDenominator=refused|PASS",
    "Published immutable binary consumer verdict mismatch.");
Require(Hash(archive) == ArchiveHash, "Published archive changed during validation.");
Console.WriteLine("MATH-RS244-NET11-PACKAGE|id=dw.quantities|version=" + Version +
    "|archiveHash=matched|binaryHash=matched|tfm=net11.0|sdk=11.0.100|" +
    "publishedArchiveUnchanged=true|publicRelease=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS244: " + message);
}
