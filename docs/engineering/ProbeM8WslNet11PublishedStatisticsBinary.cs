#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS263 qualifies the Linux WSL net11 independent binary consumer.");

const string Fixture = "tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs";
var packages = new[]
{
    (Id: "dw.tools.math.statistics", Version: "20260.20261106.1001",
     Archive: "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8",
     Binary: "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96",
     Bytes: 17451L),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4",
     Bytes: 14747L)
};
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must point to an existing absolute private feed.");
Require(File.Exists(Fixture), "Original RS160 independent Statistics consumer fixture is missing.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var root = Path.GetFullPath("build/artifacts/m8-rs263-statistics-linux-net11");
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
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.tools.math.statistics\"" +
    " Version=\"[20260.20261106.1001]\" /></ItemGroup></Project>\n");
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
        throw new InvalidOperationException("RS263 cannot start WSL.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS263 WSL failed: " +
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
Require(output.Contains("MATH-STATISTICS-BINARY|PASS|18", StringComparison.Ordinal) &&
    output.Contains("MATH-STATISTICS-CHECK|loaded-package-sha256|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-STATISTICS-CHECK|descriptive-variances|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-STATISTICS-CHECK|splitmix-checkpoint-replay|PASS", StringComparison.Ordinal),
    "Original RS160 eighteen-check Statistics fixture failed under net11.");
foreach (var item in packages)
{
    var name = item.Id + "." + item.Version + ".nupkg";
    Require(Hash(Path.Combine(feed!, name)) == item.Archive &&
        Hash(Path.Combine(copiedFeed, name)) == item.Archive,
        "Original or isolated immutable archive changed: " + item.Id);
}
Console.WriteLine("MATH-RS263-STATISTICS-LINUX-NET11|packages=2|os=linux-x64|sdk=11.0.100|tfm=net11.0|" +
    "binaryHashes=matched|checks=18|archivesUnchanged=true|publication=false|PASS");
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS263: " + reason);
}
