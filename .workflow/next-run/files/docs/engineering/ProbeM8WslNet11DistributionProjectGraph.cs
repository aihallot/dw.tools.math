#:property NuGetAudit=false
using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS251 invokes Linux WSL from the Windows operator.");
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
var root = Path.GetFullPath("build/artifacts/m8-rs251-net11-distribution-graph");
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
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
File.WriteAllText(Path.Combine(consumer, "Program.cs"), """
    using System;
    using System.Reflection;
    using dw.quantities;
    if (!OperatingSystem.IsLinux() || Environment.Version.Major != 11)
        throw new InvalidOperationException("Unexpected runtime.");
    if (new ExactRational(2, 4).ToFractionString() != "1/2")
        throw new InvalidOperationException("Exact arithmetic drift.");
    foreach (var name in new[] { "dw.quantities", "dw.tools.math.ir",
        "dw.tools.math.codecs", "dw.tools.math.symbolics",
        "dw.tools.math.numerics", "dw.tools.math.composition",
        "dw.tools.math.distribution" })
        if (Assembly.Load(name).GetName().Name != name)
            throw new InvalidOperationException("Assembly graph mismatch: " + name);
    Console.WriteLine("MATH-RS251-GRAPH|runtime=11|assemblies=7|rational=1/2|PASS");
    """);
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
        throw new InvalidOperationException("Cannot start WSL.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS251 WSL " +
            string.Join(" ", arguments) + " failed:\n" + output + "\n" + error);
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
Require(RunLinux("--version") == "11.0.100", "Wrong isolated Linux SDK.");
RunLinux("restore", "consumer/consumer.csproj", "--configfile", "NuGet.Config");
RunLinux("build", "consumer/consumer.csproj", "-c", "Release", "--no-restore");
Require(RunLinux("run", "--project", "consumer/consumer.csproj", "-c", "Release",
    "--no-build", "--no-restore") ==
    "MATH-RS251-GRAPH|runtime=11|assemblies=7|rational=1/2|PASS",
    "Linux distribution graph execution failed.");
Console.WriteLine("MATH-RS251-DISTRIBUTION|os=linux-x64|projects=7|edges=12|sdk=11.0.100|" +
    "target=net11.0|lang=15.0|isolated=true|publication=false|PASS");
static void Require(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS251: " + reason);
}
