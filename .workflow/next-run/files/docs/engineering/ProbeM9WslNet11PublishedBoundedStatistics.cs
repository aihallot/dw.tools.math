#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS277 qualifies the NEW private bounded Statistics package on Linux WSL net11.");

const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
var packages = new[]
{
    (Id: "dw.tools.math.statistics", Version: "20260.20261120.1001",
     Archive: "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1",
     Binary: "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea"),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must point to an existing absolute private feed.");
Require(File.Exists(Fixture), "RS270 bounded Statistics independent fixture is missing.");
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var root = Path.GetFullPath("build/artifacts/m9-rs277-bounded-statistics-linux-net11");
var copiedFeed = Path.Combine(root, "feed");
Directory.CreateDirectory(copiedFeed);
foreach (var item in packages)
{
    var original = Path.Combine(feed!, item.Id + "." + item.Version + ".nupkg");
    Require(File.Exists(original) && new FileInfo(original).Length > 0 &&
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
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.tools.math.statistics\"" +
    " Version=\"[20260.20261120.1001]\" /></ItemGroup></Project>\n");
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
        throw new InvalidOperationException("RS277 cannot start WSL.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS277 WSL failed: " +
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
var lockFile = Path.Combine(root, "packages.lock.json");
Require(File.Exists(lockFile), "Linux net11 locked NuGet graph is missing.");
using (var lockDoc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(lockFile)))
{
    var targets = lockDoc.RootElement.GetProperty("dependencies");
    Require(targets.EnumerateObject().Count() == 1 &&
        targets.TryGetProperty("net11.0", out _),
        "Linux lockfile is not restricted to net11.0.");
    var resolved = targets.GetProperty("net11.0").EnumerateObject()
        .ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Require(resolved.Count == 2 &&
        resolved["dw.tools.math.statistics"].GetProperty("resolved").GetString() ==
            "20260.20261120.1001" &&
        resolved["dw.quantities"].GetProperty("resolved").GetString() ==
            "20260.20261102.1001" &&
        resolved["dw.tools.math.statistics"].GetProperty("type").GetString() == "Direct" &&
        resolved["dw.quantities"].GetProperty("type").GetString() == "Transitive",
        "Linux net11 exact two-package closure differs.");
}
var lockHash = Hash(lockFile);
RunLinux("restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", linuxRoot + "/feed", "--packages",
    linuxRoot + "/private-package-cache", "--locked-mode", "-p:NuGetAudit=false");
Require(Hash(lockFile) == lockHash, "Linux locked restore changed dependency graph.");
RunLinux("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = RunLinux("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--",
    packages[0].Binary);
Require(output.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal) &&
    output.Contains("MATH-RS270-CHECK|candidate-loaded-dll|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-RS270-CHECK|forced-rejection-budget|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-RS270-CHECK|checkpoint-replay|PASS", StringComparison.Ordinal),
    "New published bounded Statistics twelve-check consumer failed under Linux net11.");
foreach (var item in packages)
{
    var name = item.Id + "." + item.Version + ".nupkg";
    Require(Hash(Path.Combine(feed!, name)) == item.Archive &&
        Hash(Path.Combine(copiedFeed, name)) == item.Archive,
        "Original or isolated immutable archive changed: " + item.Id);
}
Console.WriteLine("MATH-RS277-BOUNDED-STATISTICS-LINUX-NET11|packages=2|os=linux-x64|sdk=11.0.100|tfm=net11.0|" +
    "binaryHashes=matched|checks=12|lockedRestore=true|archivesUnchanged=true|publication=false|PASS");
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS277: " + reason);
}
