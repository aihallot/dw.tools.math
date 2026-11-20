#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS265 qualifies the Linux WSL net11 independent binary consumer.");

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
var root = Path.GetFullPath("build/artifacts/m8-rs265-scientific-linux-net11");
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
string Wsl(params string[] arguments)
{
    var start = new ProcessStartInfo("wsl.exe")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS265 cannot start WSL.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS265 WSL failed: " +
            string.Join(" ", arguments) + "\n" + output + "\n" + error);
    return output.Trim();
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
Require(RunLinux("--version") == "11.0.100", "Unexpected Linux SDK11 selection.");
RunLinux("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", linuxRoot + "/feed", "--packages",
    linuxRoot + "/private-package-cache", "-p:NuGetAudit=false");
RunLinux("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = RunLinux("run", "--project", "consumer.csproj",
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
Console.WriteLine("MATH-RS265-SCIENTIFIC-LINUX-NET11|packages=1|os=linux-x64|sdk=11.0.100|tfm=net11.0|" +
    "binaryHashes=matched|checks=15|archivesUnchanged=true|publication=false|PASS");
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS265: " + reason);
}
