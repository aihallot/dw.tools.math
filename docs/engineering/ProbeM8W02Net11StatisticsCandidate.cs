#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check") throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("RS340 bounds Windows net11.");
const string BaseId = "dw.quantities";
const string Id = "dw.tools.math.statistics";
const string Version = "20260.20261125.1001-rs340";
const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Assessment = "docs/engineering/m8-w02-c02-phase-acceptance-rs339.json";
var repository = Directory.GetCurrentDirectory();
var baseRelative = "src/projects/" + BaseId;
var statRelative = "src/projects/" + Id;
var sources = Pass(repository, "git", "ls-files", "--", baseRelative, statRelative)
    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
    .Where(x => x.EndsWith(".cs", StringComparison.Ordinal) || x.EndsWith(".csproj", StringComparison.Ordinal)).ToArray();
Need(sources.Contains(baseRelative + "/" + BaseId + ".csproj", StringComparer.Ordinal) &&
    sources.Contains(statRelative + "/" + Id + ".csproj", StringComparer.Ordinal) &&
    sources.Any(x => x.StartsWith(baseRelative + "/", StringComparison.Ordinal) && x.EndsWith(".cs", StringComparison.Ordinal)) &&
    sources.Any(x => x.StartsWith(statRelative + "/", StringComparison.Ordinal) && x.EndsWith(".cs", StringComparison.Ordinal)),
    "Tracked source roster absent.");
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "Physical historical feed absent.");
var oldArchives = new Dictionary<string, string>(StringComparer.Ordinal)
{
    [Path.Combine(feed!, BaseId + ".20260.20261102.1001.nupkg")] = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
    [Path.Combine(feed!, Id + ".20260.20261120.1001.nupkg")] = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1",
    [Path.Combine(feed!, Id + ".20260.20261106.1001.nupkg")] = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8"
};
foreach (var p in oldArchives) Need(Hash(p.Key) == p.Value, "Historical archive differs: " + p.Key);
var tracked = sources.Concat(new[] { Fixture, Risk, Assessment, "global.json", "Directory.Build.props", "Directory.Packages.props" })
    .ToDictionary(x => x, Hash, StringComparer.Ordinal);
using (var risk = JsonDocument.Parse(File.ReadAllText(Risk)))
using (var assessment = JsonDocument.Parse(File.ReadAllText(Assessment)))
{
    Need(risk.RootElement.GetProperty("gates").GetProperty("migrationAuthorization").GetString() == "not-granted" &&
        !assessment.RootElement.GetProperty("phaseClosureAuthorized").GetBoolean() &&
        !assessment.RootElement.GetProperty("newNet11PackageGraphQualified").GetBoolean(),
        "Historical refusal or incomplete acceptance changed.");
}
var work = Path.GetFullPath(Path.Combine("build", "artifacts", "f340-" + Guid.NewGuid().ToString("N")[..8]));
Directory.CreateDirectory(work);
var candidateFeed = Path.Combine(work, "feed");
var producer = Path.Combine(work, "producer");
var consumer = Path.Combine(work, "consumer");
var producerCache = Path.Combine(work, "pcache");
var consumerCache = Path.Combine(work, "ccache");
foreach (var p in new[] { candidateFeed, producer, consumer }) Directory.CreateDirectory(p);
File.WriteAllText(Path.Combine(work, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(work, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework><LangVersion>15.0</LangVersion>" +
    "<Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<EnableNETAnalyzers>true</EnableNETAnalyzers><AnalysisLevel>latest</AnalysisLevel><Deterministic>true</Deterministic>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<BaseOutputPath>$(MSBuildThisFileDirectory)out/bin/$(MSBuildProjectName)/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)out/obj/$(MSBuildProjectName)/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(work, "Directory.Packages.props"),
    "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(work, "Directory.Build.targets"), "<Project />\n");
var config = Path.Combine(work, "NuGet.Config");
File.WriteAllText(config, "<configuration><packageSources><clear /></packageSources></configuration>\n");
Need(Pass(work, "dotnet", "--version").Trim() == "11.0.100", "Wrong candidate SDK.");
foreach (var source in sources)
{
    var rel = source["src/projects/".Length..];
    var target = Path.Combine(producer, rel);
    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
    File.Copy(source, target);
    Need(Hash(target) == tracked[source], "Copied source differs: " + source);
}
var baseProject = Path.Combine(producer, BaseId, BaseId + ".csproj");
var statProject = Path.Combine(producer, Id, Id + ".csproj");
var baseDoc = XDocument.Load(baseProject);
var statDoc = XDocument.Load(statProject);
Need(Field(baseDoc, "PackageId") == BaseId && Field(statDoc, "PackageId") == Id &&
    !baseDoc.Descendants("ProjectReference").Any() && !baseDoc.Descendants("PackageReference").Any() &&
    !statDoc.Descendants("PackageReference").Any(), "Selected source graph differs.");
var edge = statDoc.Descendants("ProjectReference").Single();
Need(edge.Attribute("Include")?.Value == "../dw.quantities/dw.quantities.csproj", "Source graph edge differs.");
edge.ReplaceWith(new XElement("PackageReference", new XAttribute("Include", BaseId),
    new XAttribute("Version", "[" + Version + "]")));
foreach (var (doc, path) in new[] { (baseDoc, baseProject), (statDoc, statProject) })
{
    // The calendar candidate version is a NuGet identity, not a CLR assembly version.
    // Preserve the original project Version and its valid SDK-derived assembly/file versions.
    Need(!doc.Descendants("PackageVersion").Any(), "Unexpected existing package version override.");
    doc.Descendants("Version").Single().AddAfterSelf(new XElement("PackageVersion", Version));
    doc.Save(path);
}
foreach (var project in new[] { baseProject, statProject })
{
    Pass(work, "dotnet", "restore", project, "--configfile", config, "--source", candidateFeed,
        "--packages", producerCache, "-p:NuGetAudit=false");
    Pass(work, "dotnet", "pack", project, "-c", "Release", "--no-restore", "-o", candidateFeed);
}
var baseArchive = Path.Combine(candidateFeed, BaseId + "." + Version + ".nupkg");
var statArchive = Path.Combine(candidateFeed, Id + "." + Version + ".nupkg");
Need(Directory.GetFiles(candidateFeed, "*.nupkg").Length == 2, "Candidate feed must contain exactly two archives.");
var baseDll = Inspect(baseArchive, BaseId, false);
var statDll = Inspect(statArchive, Id, true);
var archiveHashes = new[] { baseArchive, statArchive }.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var projectPath = Path.Combine(consumer, "consumer.csproj");
File.WriteAllText(projectPath, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "<NuGetAudit>false</NuGetAudit></PropertyGroup><ItemGroup><PackageReference Include=\"" + Id +
    "\" Version=\"[" + Version + "]\" /></ItemGroup></Project>\n");
var program = Path.Combine(consumer, "Program.cs");
File.Copy(Fixture, program);
Need(Hash(program) == tracked[Fixture] && Directory.GetFiles(consumer, "*.cs", SearchOption.AllDirectories).Length == 1 &&
    !XDocument.Load(projectPath).Descendants("ProjectReference").Any(), "Consumer is not source-free.");
var cleanProject = File.ReadAllText(projectPath);
var projectHash = Hash(projectPath);
string[] RestoreArgs(bool locked) => new[] { "dotnet", "restore", projectPath, "--configfile", config,
    "--source", candidateFeed, "--packages", consumerCache, "-p:NuGetAudit=false" }
    .Concat(locked ? new[] { "--locked-mode" } : Array.Empty<string>()).ToArray();
Pass(consumer, RestoreArgs(false));
var lockPath = Path.Combine(consumer, "packages.lock.json");
using (var json = JsonDocument.Parse(File.ReadAllText(lockPath)))
{
    var tfms = json.RootElement.GetProperty("dependencies");
    Need(tfms.EnumerateObject().Count() == 1, "Unexpected lock TFM count.");
    var entries = tfms.GetProperty("net11.0").EnumerateObject().ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Need(entries.Count == 2 &&
        entries[Id].GetProperty("type").GetString() == "Direct" &&
        entries[Id].GetProperty("resolved").GetString() == Version &&
        entries[BaseId].GetProperty("type").GetString() == "Transitive" &&
        entries[BaseId].GetProperty("resolved").GetString() == Version,
        "Exact new package closure differs.");
}
var lockHash = Hash(lockPath);
Pass(consumer, RestoreArgs(true));
Need(Hash(lockPath) == lockHash, "Locked restore changed the lock.");
foreach (var (id, dllHash) in new[] { (BaseId, baseDll), (Id, statDll) })
    Need(Hash(Path.Combine(consumerCache, id, Version, "lib", "net11.0", id + ".dll")) == dllHash,
        "Restored candidate binary differs: " + id);
void Replay()
{
    Pass(consumer, "dotnet", "build", projectPath, "-c", "Release", "--no-restore");
    var output = Pass(consumer, "dotnet", "run", "--project", projectPath, "-c", "Release",
        "--no-build", "--no-restore", "--", statDll);
    Need(output.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal) &&
        output.Contains("MATH-RS270-CHECK|checkpoint-replay|PASS", StringComparison.Ordinal),
        "New candidate runtime fixture failed.");
}
Replay();
try
{
    File.WriteAllText(projectPath, cleanProject.Replace("[" + Version + "]", "[20260.20261125.9999-rs340]", StringComparison.Ordinal));
    var refusal = Execute(consumer, RestoreArgs(true));
    Need(refusal.ExitCode != 0 && (refusal.Output.Contains("NU1004", StringComparison.OrdinalIgnoreCase) ||
        refusal.Output.Contains("NU1102", StringComparison.OrdinalIgnoreCase)), "Wrong locked candidate version was not refused: " + refusal.Output);
    Console.WriteLine("MATH-RS340-NEGATIVE|wrong-exact-version|PASS");
}
finally { File.WriteAllText(projectPath, cleanProject); }
Need(Hash(projectPath) == projectHash && Hash(lockPath) == lockHash, "Clean consumer or lock was not preserved.");
Pass(consumer, RestoreArgs(true));
Need(Hash(lockPath) == lockHash, "Restored lock differs.");
Replay();
foreach (var item in archiveHashes) Need(Hash(item.Key) == item.Value, "New archive changed after packing.");
foreach (var item in oldArchives) Need(Hash(item.Key) == item.Value, "Historical archive changed.");
foreach (var item in tracked) Need(Hash(item.Key) == item.Value, "Original repository input changed: " + item.Key);
foreach (var source in sources.Where(x => x.EndsWith(".cs", StringComparison.Ordinal)))
    Need(Hash(Path.Combine(producer, source["src/projects/".Length..])) == tracked[source], "Candidate source changed: " + source);
Console.WriteLine("MATH-RS340-NET11-CANDIDATE|version=" + Version + "|packages=2|edges=1|checks=12|negativeControls=1|lockSha256=" + lockHash +
    "|statisticsArchiveSha256=" + archiveHashes[statArchive] + "|statisticsDllSha256=" + statDll +
    "|quantitiesArchiveSha256=" + archiveHashes[baseArchive] + "|quantitiesDllSha256=" + baseDll +
    "|published=false|productionMutation=false|PASS");

string Inspect(string archive, string id, bool hasDependency)
{
    using var zip = ZipFile.OpenRead(archive);
    var libs = zip.Entries.Where(e => e.FullName.StartsWith("lib/", StringComparison.Ordinal) && e.FullName.EndsWith(".dll", StringComparison.Ordinal)).ToArray();
    Need(libs.Length == 1 && libs[0].FullName == "lib/net11.0/" + id + ".dll", "New net11 binary not exclusive: " + id);
    using var dll = libs[0].Open();
    var dllHash = Convert.ToHexStringLower(SHA256.HashData(dll));
    using var spec = zip.Entries.Single(e => e.FullName == id + ".nuspec").Open();
    var doc = XDocument.Load(spec);
    var ns = doc.Root!.Name.Namespace;
    var metadata = doc.Root.Element(ns + "metadata")!;
    Need(metadata.Element(ns + "id")?.Value == id && metadata.Element(ns + "version")?.Value == Version,
        "Candidate nuspec identity differs.");
    var deps = metadata.Descendants(ns + "dependency").ToArray();
    Need(hasDependency ? deps.Length == 1 && deps[0].Attribute("id")?.Value == BaseId &&
        deps[0].Attribute("version")?.Value == "[" + Version + "]" : deps.Length == 0,
        "Candidate nuspec exact graph differs: " + id);
    return dllHash;
}
static string? Field(XDocument doc, string name) => doc.Descendants(name).FirstOrDefault()?.Value.Trim();
static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
static void Need(bool ok, string reason) { if (!ok) throw new InvalidDataException("RS340: " + reason); }
static string Pass(string cwd, params string[] command)
{
    var result = Execute(cwd, command);
    Need(result.ExitCode == 0, string.Join(" ", command) + " failed: " + result.Output);
    return result.Output;
}
static (int ExitCode, string Output) Execute(string cwd, params string[] command)
{
    var info = new ProcessStartInfo(command[0]) { WorkingDirectory = cwd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var arg in command.Skip(1)) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("Cannot start " + command[0]);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(150000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS340 command timed out: " + string.Join(" ", command));
    }
    return (process.ExitCode, (stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult()).Trim());
}
