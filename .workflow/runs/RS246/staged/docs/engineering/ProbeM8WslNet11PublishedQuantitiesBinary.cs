#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS246 invokes Linux WSL from Windows.");

const string Package = "dw.quantities";
const string Version = "20260.20261102.1001";
const string ArchiveHash = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string DllHash = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
        Directory.Exists(feed), "NUGET_LOCAL must resolve to an existing absolute feed.");
var original = Path.Combine(feed!, Package + "." + Version + ".nupkg");
Require(File.Exists(original) && new FileInfo(original).Length == 14747 &&
        (File.GetAttributes(original) & FileAttributes.ReparsePoint) == 0,
    "The exact immutable archive is absent or has an unexpected filesystem identity.");
string Hash(string file) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
Require(Hash(original) == ArchiveHash, "Published archive hash differs from RS137.");
using (var zip = ZipFile.OpenRead(original))
{
    var entry = zip.Entries.Single(x => x.FullName == "lib/net10.0/dw.quantities.dll");
    using var stream = entry.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == DllHash,
        "Packaged net10 DLL differs from RS137.");
}

var root = Path.GetFullPath("build/artifacts/m8-rs246-wsl-published-quantities");
Directory.CreateDirectory(root);
var localFeed = Path.Combine(root, "feed");
Directory.CreateDirectory(localFeed);
var snapshot = Path.Combine(localFeed, Path.GetFileName(original));
File.Copy(original, snapshot, overwrite: true);
Require(Hash(snapshot) == ArchiveHash, "Isolated Linux feed snapshot hash differs.");
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
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using dw.quantities;

    if (!OperatingSystem.IsLinux() || RuntimeInformation.OSArchitecture != Architecture.X64 ||
        Environment.Version.Major != 11)
        throw new InvalidOperationException("Unexpected Linux binary runtime.");
    var assembly = typeof(ExactRational).Assembly;
    if (assembly.GetName().Name != "dw.quantities")
        throw new InvalidOperationException("Wrong package assembly identity.");
    var binaryHash = Convert.ToHexStringLower(
        SHA256.HashData(System.IO.File.ReadAllBytes(assembly.Location)));
    if (binaryHash != "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
        throw new InvalidOperationException("Loaded Linux DLL differs from immutable package.");
    if ((new ExactRational(1, 3) + new ExactRational(1, 6)).ToFractionString() != "1/2")
        throw new InvalidOperationException("Exact arithmetic differs from the Windows binary oracle.");
    var refused = false;
    try { _ = new ExactRational(1, 0); }
    catch (DivideByZeroException) { refused = true; }
    var length = new Quantity(ExactRational.One, DimensionVector.LengthDimension);
    var mass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
    if (!refused || length.TryAdd(mass, out _))
        throw new InvalidOperationException("Binary invalid-input or dimension refusal drift.");
    Console.WriteLine("MATH-RS246-LINUX-BINARY|os=linux|arch=x64|runtime=11|" +
        "assembly=dw.quantities|loadedHash=matched|sum=1/2|" +
        "zeroDenominator=refused|dimensionMismatch=refused|PASS");
    """);
string Wsl(params string[] arguments)
{
    var info = new ProcessStartInfo("wsl.exe")
    {
        UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS246 could not start WSL.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS246 WSL failed: " +
            string.Join(" ", arguments) + "\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
var linuxRoot = Wsl("--exec", "wslpath", "-a", "-u", root);
Require(linuxRoot.StartsWith("/", StringComparison.Ordinal) &&
        !linuxRoot.Contains('\n'), "WSL workspace mapping failed.");
string RunLinux(params string[] arguments)
{
    var command = new List<string>
    {
        "--exec", "bash", "-lc", "cd \"$1\" && shift && dotnet \"$@\"", "bash", linuxRoot
    };
    command.AddRange(arguments);
    return Wsl(command.ToArray());
}
Require(RunLinux("--version") == "11.0.100",
    "The isolated Linux consumer did not select SDK 11.0.100.");
RunLinux("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", linuxRoot + "/feed",
    "--packages", linuxRoot + "/private-package-cache", "-p:NuGetAudit=false");
RunLinux("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = RunLinux("run", "--project", "consumer.csproj", "-c", "Release",
    "--no-build", "--no-restore");
Require(output == "MATH-RS246-LINUX-BINARY|os=linux|arch=x64|runtime=11|" +
    "assembly=dw.quantities|loadedHash=matched|sum=1/2|" +
    "zeroDenominator=refused|dimensionMismatch=refused|PASS",
    "Linux package-only consumer differs from the cross-runtime binary oracle.");
Require(Hash(snapshot) == ArchiveHash && Hash(original) == ArchiveHash,
    "Original archive or isolated snapshot changed during Linux consumption.");
Console.WriteLine("MATH-RS246-PUBLISHED-LINUX|package=dw.quantities|version=" + Version +
    "|sdk=11.0.100|tfm=net11.0|os=linux-x64|loadedHash=matched|" +
    "feedOriginalUnchanged=true|publication=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS246: " + message);
}
