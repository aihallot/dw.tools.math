#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS245 compares Windows net10/net11 binary consumers.");

const string Package = "dw.quantities";
const string Version = "20260.20261102.1001";
const string ArchiveHash = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string BinaryHash = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must select an existing absolute private feed.");
var archive = Path.Combine(feed!, Package + "." + Version + ".nupkg");
Require(File.Exists(archive) && new FileInfo(archive).Length == 14747,
    "Immutable package missing or wrong archive size.");
Require((File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0,
    "Immutable package must not be a reparse point.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Require(Hash(archive) == ArchiveHash, "Immutable package archive differs from RS137.");
using (var zip = ZipFile.OpenRead(archive))
{
    var binary = zip.Entries.Single(x => x.FullName == "lib/net10.0/dw.quantities.dll");
    using var bytes = binary.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(bytes)) == BinaryHash,
        "Published net10 DLL differs from RS137.");
}
const string Oracle = "MATH-RS245-BINARY|loadedHash=matched|assembly=dw.quantities|" +
    "sum=1/2|zeroDenominator=refused|dimensionMismatch=refused|PASS";
const string Program = """
    using System;
    using System.Security.Cryptography;
    using dw.quantities;

    if (!OperatingSystem.IsWindows() || Environment.Version.Major is not (10 or 11))
        throw new InvalidOperationException("Unexpected binary runtime.");
    var assembly = typeof(ExactRational).Assembly;
    if (assembly.GetName().Name != "dw.quantities")
        throw new InvalidOperationException("Incorrect assembly identity.");
    var hash = Convert.ToHexStringLower(
        SHA256.HashData(System.IO.File.ReadAllBytes(assembly.Location)));
    if (hash != "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
        throw new InvalidOperationException("Immutable loaded DLL hash mismatch.");
    if ((new ExactRational(1, 3) + new ExactRational(1, 6)).ToFractionString() != "1/2")
        throw new InvalidOperationException("Exact arithmetic contract drift.");
    var denominatorRefused = false;
    try { _ = new ExactRational(1, 0); }
    catch (DivideByZeroException) { denominatorRefused = true; }
    var length = new Quantity(ExactRational.One, DimensionVector.LengthDimension);
    var mass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
    if (!denominatorRefused || length.TryAdd(mass, out _))
        throw new InvalidOperationException("Invalid-input refusal contract drift.");
    Console.WriteLine("MATH-RS245-BINARY|loadedHash=matched|assembly=dw.quantities|" +
        "sum=1/2|zeroDenominator=refused|dimensionMismatch=refused|PASS");
    """;
var observed = new Dictionary<int, string>();
foreach (var major in new[] { 10, 11 })
{
    var root = Path.GetFullPath(Path.Combine(
        "build/artifacts/m8-rs245-published-quantities-dual-runtime", "net" + major));
    Directory.CreateDirectory(root);
    File.WriteAllText(Path.Combine(root, "global.json"), major == 10
        ? "{\n \"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}\n}\n"
        : "{\n \"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}\n}\n");
    File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
        "<Project><PropertyGroup><TargetFramework>net" + major + ".0</TargetFramework>" +
        "<LangVersion>" + (major == 10 ? "14.0" : "15.0") + "</LangVersion>" +
        "<Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings>" +
        "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
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
    File.WriteAllText(Path.Combine(root, "Program.cs"), Program);
    string Run(params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = root, UseShellExecute = false,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        foreach (var item in arguments) start.ArgumentList.Add(item);
        using var process = Process.Start(start) ??
            throw new InvalidOperationException("RS245 cannot start dotnet.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(120000) || process.ExitCode != 0)
            throw new InvalidOperationException("RS245 dotnet " +
                string.Join(" ", arguments) + " failed:\n" + stdout + "\n" + stderr);
        return stdout.Trim();
    }
    var sdk = Run("--version");
    Require(sdk.StartsWith(major + ".0.", StringComparison.Ordinal) &&
        (major != 11 || sdk == "11.0.100"), "Incorrect isolated SDK " + sdk);
    Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source",
        feed!, "--packages", Path.Combine(root, "private-package-cache"), "-p:NuGetAudit=false");
    Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
    var output = Run("run", "--project", "consumer.csproj",
        "-c", "Release", "--no-build", "--no-restore");
    Require(output == Oracle, "Published package contract mismatch under net" + major);
    Require(Hash(archive) == ArchiveHash, "Immutable archive changed during net" + major);
    observed.Add(major, output);
    Console.WriteLine("MATH-RS245-RUNTIME|net=" + major + "|sdk=" + sdk +
        "|binary=verified|oracle=matched|PASS");
}
Require(observed[10] == observed[11], "Published binary behavior differs between runtimes.");
Console.WriteLine("MATH-RS245-PUBLISHED-PARITY|package=dw.quantities|version=" + Version +
    "|net10=pass|net11=pass|archiveHash=matched|dllHash=matched|" +
    "publishedArchiveUnchanged=true|publicRelease=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS245: " + message);
}
