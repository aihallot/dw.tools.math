#:property NuGetAudit=false
using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS253 qualifies a Windows isolated SDK11 restore topology.");
const string Distribution = "dw.tools.math.distribution";
var originalRoot = Path.GetFullPath("src/projects");
var graph = new Dictionary<string, string[]>(StringComparer.Ordinal);
void Collect(string id)
{
    if (graph.ContainsKey(id)) return;
    var directory = Path.Combine(originalRoot, id);
    var project = Path.Combine(directory, id + ".csproj");
    Require(File.Exists(project), "Missing project: " + id);
    var xml = XDocument.Load(project);
    var references = xml.Descendants().Where(e => e.Name.LocalName == "ProjectReference")
        .Select(e => e.Attribute("Include")?.Value ??
            throw new InvalidDataException("Missing ProjectReference Include"))
        .Select(path => Path.GetFullPath(Path.Combine(directory, path)))
        .Select(path =>
        {
            var parent = Path.GetDirectoryName(path)!;
            Require(Path.GetDirectoryName(parent) == originalRoot &&
                Path.GetFileName(path) == Path.GetFileName(parent) + ".csproj",
                "Reference escapes Math package graph: " + path);
            return Path.GetFileNameWithoutExtension(path);
        }).OrderBy(x => x, StringComparer.Ordinal).ToArray();
    graph.Add(id, references);
    foreach (var reference in references) Collect(reference);
}
Collect(Distribution);
Require(graph.Count == 7 && graph[Distribution].Length == 6 &&
    graph.Values.Sum(x => x.Length) == 12, "RS232 distribution graph baseline changed.");
var root = Path.GetFullPath("build/artifacts/m8-rs253-net11-distribution-graph");
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors><RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "<BaseOutputPath>$(MSBuildThisFileDirectory)out/bin/$(MSBuildProjectName)/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)out/obj/$(MSBuildProjectName)/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
foreach (var (id, references) in graph)
{
    var destination = Path.Combine(root, "projects", id);
    Directory.CreateDirectory(destination);
    var sources = Directory.GetFiles(Path.Combine(originalRoot, id), "*.cs");
    Require(sources.Length > 0, "No sources in " + id);
    foreach (var source in sources)
        File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), true);
    var project = new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
        new XElement("PropertyGroup", new XElement("AssemblyName", id),
            new XElement("IsPackable", "false"), new XElement("NuGetAudit", "false")),
        new XElement("ItemGroup", references.Select(dep =>
            new XElement("ProjectReference", new XAttribute("Include",
                "../" + dep + "/" + dep + ".csproj")))));
    File.WriteAllText(Path.Combine(destination, id + ".csproj"), project + "\n");
}
var consumer = Path.Combine(root, "consumer");
Directory.CreateDirectory(consumer);
File.WriteAllText(Path.Combine(consumer, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "</PropertyGroup><ItemGroup><ProjectReference Include=\"../projects/" +
    Distribution + "/" + Distribution + ".csproj\" /></ItemGroup></Project>\n");
string Run(params string[] args)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in args) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS253 cannot start dotnet.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS253 dotnet " +
            string.Join(" ", args) + " failed:\n" + output + "\n" + error);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "Wrong isolated SDK.");
Run("restore", "consumer/consumer.csproj", "--configfile", "NuGet.Config",
    "-p:NuGetAudit=false");
var locks = Directory.GetFiles(root, "packages.lock.json", SearchOption.AllDirectories)
    .OrderBy(x => x, StringComparer.Ordinal).ToArray();
Require(locks.Length > 0 && locks.Length <= 8 &&
    locks.Any(p => p.Contains(Path.DirectorySeparatorChar + "consumer" +
        Path.DirectorySeparatorChar, StringComparison.Ordinal)),
    "The isolated consumer failed to produce a lock file.");
var hashes = locks.Select(p => (Path: p,
    Sha: Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(
        File.ReadAllBytes(p))))).ToArray();
Run("restore", "consumer/consumer.csproj", "--configfile", "NuGet.Config",
    "--locked-mode", "-p:NuGetAudit=false");
foreach (var item in hashes)
    Require(File.Exists(item.Path) &&
        Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(
            File.ReadAllBytes(item.Path))) == item.Sha,
        "Locked restore changed dependency lock: " + item.Path);
Require(graph.Count == 7 && graph.Values.Sum(x => x.Length) == 12 &&
    File.ReadAllText(Path.Combine(root, "NuGet.Config")).Contains("<clear />", StringComparison.Ordinal),
    "Distribution topology or isolated feed policy drift.");
Console.WriteLine("MATH-RS253-LOCKED-RESTORE|sdk=11.0.100|net=11.0|projects=7|" +
    "edges=12|consumerLock=present|lockedRestore=pass|locksStable=true|" +
    "privateFeedUnchanged=true|productionMutation=false|PASS");
static void Require(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS253: " + reason);
}
