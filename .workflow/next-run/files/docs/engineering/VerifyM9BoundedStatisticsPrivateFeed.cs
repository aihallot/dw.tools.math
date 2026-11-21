#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Text.Json;

if (args.Length != 5 || args[0] != "--check")
    throw new ArgumentException("Expected --check <feed> <archive-sha256> <dll-sha256> <base-sha256>.");
const string Id = "dw.tools.math.statistics";
const string Version = "20260.20261120.1001";
const string BaseId = "dw.quantities";
const string BaseVersion = "20260.20261102.1001";
const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
var feed = Path.GetFullPath(args[1]);
var expectedArchive = args[2];
var expectedDll = args[3];
var expectedBase = args[4];
void Require(bool valid, string why) { if (!valid) throw new InvalidDataException("M9-INDEPENDENT-VERIFY: " + why); }
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Require(Directory.Exists(feed) && File.Exists(Fixture), "Physical feed or original binary fixture missing.");
var archive = Path.Combine(feed, Id + "." + Version + ".nupkg");
var dependency = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
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
var work = Path.GetFullPath("build/artifacts/m9-rs272-private-independent-verification");
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
string Run(params string[] argv)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory=work,
        UseShellExecute=false, RedirectStandardOutput=true, RedirectStandardError=true };
    foreach (var a in argv) info.ArgumentList.Add(a);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable");
    var output=process.StandardOutput.ReadToEnd();
    var error=process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode!=0)
        throw new InvalidOperationException("Independent .NET11 consumer failed:\n"+output+"\n"+error);
    return output.Trim();
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
Require(Hash(archive)==expectedArchive && Hash(dependency)==expectedBase,
    "Physical source archives mutated during verification.");
Console.WriteLine("MATH-RS272-PRIVATE-READBACK|version="+Version+"|archiveSha256="+expectedArchive+
    "|dllSha256="+expectedDll+"|dependencySha256="+expectedBase+
    "|checks=12|net11=11.0.100|lockedRestore=true|PASS");
