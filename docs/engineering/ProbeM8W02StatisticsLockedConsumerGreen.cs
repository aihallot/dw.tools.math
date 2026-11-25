#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS335 qualifies Windows SDK 11.0.100.");
const string Id = "dw.tools.math.statistics";
const string Version = "20260.20261120.1001";
const string BaseId = "dw.quantities";
const string BaseVersion = "20260.20261102.1001";
const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
const string expectedArchive = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string expectedDll = "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea";
const string expectedBase = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string baseDll = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string oldSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
void Require(bool valid, string why) { if (!valid) throw new InvalidDataException("RS335: " + why); }
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed) && File.Exists(Fixture), "Physical feed or original binary fixture missing.");
var archive = Path.Combine(feed!, Id + "." + Version + ".nupkg");
var dependency = Path.Combine(feed!, BaseId + "." + BaseVersion + ".nupkg");
var old = Path.Combine(feed!, Id + ".20260.20261106.1001.nupkg");
Require(File.Exists(old) && Hash(old) == oldSha, "Immutable earlier Statistics archive differs.");
foreach (var path in new[] { archive, dependency, old })
    Require(File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0,
        "Missing or redirected archive: " + path);
var tracked = new[] { "global.json", "Directory.Build.props", Fixture,
    "docs/engineering/m8-w02-c02-source-free-consumer-red-rs333.json" };
var hashes = tracked.ToDictionary(x => x, Hash, StringComparer.Ordinal);
using (var z = ZipFile.OpenRead(dependency))
using (var stream = z.Entries.Single(x => x.FullName == "lib/net10.0/dw.quantities.dll").Open())
    Require(Convert.ToHexStringLower(SHA256.HashData(stream)) == baseDll, "Base DLL differs.");
Require(File.Exists(archive) && File.Exists(dependency) &&
    Hash(archive) == expectedArchive && Hash(dependency) == expectedBase,
    "Physical archive bytes differ from pinned identities.");
using (var zip = ZipFile.OpenRead(archive))
{
    var entry = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + Id + ".dll");
    using var dll = entry.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(dll)) == expectedDll, "Packaged DLL differs.");
    var nuspec = zip.Entries.Single(e => e.FullName == Id + ".nuspec");
    using var stream = nuspec.Open();
    var xml = XDocument.Load(stream);
    var ns = xml.Root!.Name.Namespace;
    var metadata = xml.Root.Element(ns + "metadata")!;
    Require(metadata.Element(ns + "id")?.Value == Id &&
        metadata.Element(ns + "version")?.Value == Version, "NuGet identity differs.");
    var deps = metadata.Element(ns + "dependencies")!.Descendants(ns + "dependency").ToArray();
    Require(deps.Length == 1 && deps[0].Attribute("id")?.Value == BaseId &&
        deps[0].Attribute("version")?.Value == "[" + BaseVersion + "]",
        "Unexpected exact dependency.");
}
var work = Path.Combine(Path.GetFullPath("build/artifacts"),
    "f335-" + Guid.NewGuid().ToString("N")[..10]);
Directory.CreateDirectory(work);
var localFeed = Path.Combine(work, "feed");
Directory.CreateDirectory(localFeed);
foreach (var original in new[] { archive, dependency })
    File.Copy(original, Path.Combine(localFeed, Path.GetFileName(original)), true);
File.WriteAllText(Path.Combine(work, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(work, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(work,"NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(work, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile></PropertyGroup>" +
    "<ItemGroup><PackageReference Include=\"" + Id + "\" Version=\"[" + Version + "]\" />" +
    "</ItemGroup></Project>\n");
File.Copy(Fixture, Path.Combine(work,"Program.cs"),true);
var projectPath = Path.Combine(work, "consumer.csproj");
var cleanProject = File.ReadAllText(projectPath);
var projectHash = Hash(projectPath);
var parsedProject = XDocument.Load(projectPath);
Require(!parsedProject.Descendants("ProjectReference").Any() &&
    parsedProject.Descendants("PackageReference").Count() == 1 &&
    Directory.GetFiles(work, "*.cs", SearchOption.TopDirectoryOnly).Length == 1 &&
    Hash(Path.Combine(work, "Program.cs")) == hashes[Fixture],
    "Consumer must contain only its existing test fixture and one exact package reference.");
foreach (var path in new[] { archive, dependency })
    Require(Hash(Path.Combine(localFeed, Path.GetFileName(path))) == Hash(path), "Copied archive differs.");

(int ExitCode, string Output) Execute(params string[] argv)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory=work,
        UseShellExecute=false, RedirectStandardOutput=true, RedirectStandardError=true };
    foreach (var a in argv) info.ArgumentList.Add(a);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS335 isolated dotnet timed out.");
    }
    return (process.ExitCode, (stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult()).Trim());
}
string Run(params string[] argv)
{
    var result = Execute(argv);
    Require(result.ExitCode == 0, "Independent .NET11 consumer failed: " + result.Output);
    return result.Output;
}
Require(Run("--version") == "11.0.100", "Wrong independent SDK.");
Run("restore","consumer.csproj","--configfile","NuGet.Config","--source",localFeed,
    "--packages",Path.Combine(work,"private-cache"),"-p:NuGetAudit=false");
var lockFile=Path.Combine(work,"packages.lock.json");
Require(File.Exists(lockFile), "NuGet lock missing.");
using (var json=JsonDocument.Parse(File.ReadAllText(lockFile)))
{
    var all=json.RootElement.GetProperty("dependencies");
    Require(all.EnumerateObject().Count()==1 && all.TryGetProperty("net11.0",out _),
        "Lock lacks exclusive net11 TFM.");
    var entries=all.GetProperty("net11.0").EnumerateObject().ToDictionary(x=>x.Name,x=>x.Value,
        StringComparer.OrdinalIgnoreCase);
    Require(entries.Count==2 &&
        entries[Id].GetProperty("type").GetString()=="Direct" &&
        entries[Id].GetProperty("resolved").GetString()==Version &&
        entries[BaseId].GetProperty("type").GetString()=="Transitive" &&
        entries[BaseId].GetProperty("resolved").GetString()==BaseVersion,
        "Locked two-package closure differs.");
}
var lockHash=Hash(lockFile);
Run("restore","consumer.csproj","--configfile","NuGet.Config","--source",localFeed,
    "--packages",Path.Combine(work,"private-cache"),"--locked-mode","-p:NuGetAudit=false");
Require(Hash(lockFile)==lockHash, "Locked restore mutated dependency lock.");
Run("build","consumer.csproj","-c","Release","--no-restore");
var result=Run("run","--project","consumer.csproj","-c","Release","--no-build",
    "--no-restore","--",expectedDll);
Require(result.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12",StringComparison.Ordinal) &&
    result.Contains("MATH-RS270-CHECK|checkpoint-replay|PASS",StringComparison.Ordinal) &&
    result.Contains("MATH-RS270-CHECK|forced-rejection-budget|PASS",StringComparison.Ordinal),
    "Independent binary fixture refused.");
var cache = Path.Combine(work, "private-cache");
Require(Hash(Path.Combine(cache, BaseId, BaseVersion, "lib", "net10.0", BaseId + ".dll")) == baseDll,
    "Restored base binary differs.");
Require(Hash(Path.Combine(cache, Id, Version, "lib", "net10.0", Id + ".dll")) == expectedDll,
    "Restored Statistics binary differs.");
var negative = 0;
void Refuse(string label, string changed)
{
    Require(changed != cleanProject, "Missing negative mutation.");
    File.WriteAllText(projectPath, changed);
    try
    {
        var refusal = Execute("restore", "consumer.csproj", "--configfile", "NuGet.Config",
            "--source", localFeed, "--packages", cache, "--locked-mode", "-p:NuGetAudit=false");
        Require(refusal.ExitCode != 0 &&
            (refusal.Output.Contains("NU1004", StringComparison.OrdinalIgnoreCase) ||
             refusal.Output.Contains("NU1101", StringComparison.OrdinalIgnoreCase) ||
             refusal.Output.Contains("NU1102", StringComparison.OrdinalIgnoreCase)),
            "Expected lock refusal: " + label + " " + refusal.Output);
        negative++;
        Console.WriteLine("MATH-RS335-NEGATIVE|" + label + "|PASS");
    }
    finally { File.WriteAllText(projectPath, cleanProject); }
    Require(Hash(projectPath) == projectHash && Hash(lockFile) == lockHash, "Clean inputs changed.");
    Run("restore", "consumer.csproj", "--configfile", "NuGet.Config", "--source", localFeed,
        "--packages", cache, "--locked-mode", "-p:NuGetAudit=false");
    Require(Hash(lockFile) == lockHash, "Restored lock changed.");
}
Refuse("wrong-exact-version", cleanProject.Replace("[" + Version + "]", "[20260.20261120.9999]", StringComparison.Ordinal));
Refuse("transitive-to-direct", cleanProject.Replace("</ItemGroup>",
    "<PackageReference Include=\"" + BaseId + "\" Version=\"[" + BaseVersion + "]\" /></ItemGroup>", StringComparison.Ordinal));
Require(negative == 2, "Two real locked restore refusals required.");
Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
var restored = Run("run", "--project", "consumer.csproj", "-c", "Release", "--no-build", "--no-restore", "--", expectedDll);
Require(restored.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal), "Restored runtime fixture failed.");
Require(Hash(archive)==expectedArchive && Hash(dependency)==expectedBase && Hash(old)==oldSha &&
    Hash(Path.Combine(localFeed, Path.GetFileName(archive))) == expectedArchive &&
    Hash(Path.Combine(localFeed, Path.GetFileName(dependency))) == expectedBase,
    "Original or copied immutable archives changed.");
foreach (var item in hashes) Require(Hash(item.Key) == item.Value, "Repository authority changed: " + item.Key);
Console.WriteLine("MATH-RS335-STATISTICS-SOURCE-FREE|version="+Version+"|archiveSha256="+expectedArchive+
    "|dllSha256="+expectedDll+"|dependencySha256="+expectedBase+
    "|checks=12|net11=11.0.100|lockedRestore=true|negativeControls=2|publication=false|PASS");
