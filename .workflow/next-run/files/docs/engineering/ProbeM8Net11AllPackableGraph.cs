#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

var expected = new Dictionary<string, string[]>(StringComparer.Ordinal)
{
    ["dw.tools.math.foundation"] = [],
    ["dw.quantities"] = [],
    ["dw.quantities.expression"] = ["dw.quantities"],
    ["dw.quantities.standard"] = ["dw.quantities", "dw.quantities.expression"],
    ["dw.tools.math.statistics"] = ["dw.quantities"],
    ["dw.tools.math.analysis"] = [],
    ["dw.tools.math.scientific"] = [],
    ["dw.tools.math.ir"] = ["dw.quantities"],
    ["dw.tools.math.codecs"] = ["dw.tools.math.ir"],
    ["dw.tools.math.symbolics"] = ["dw.tools.math.ir"],
    ["dw.tools.math.numerics"] = ["dw.tools.math.ir"],
    ["dw.tools.math.composition"] = ["dw.quantities", "dw.tools.math.ir"],
    ["dw.tools.math.distribution"] =
    [
        "dw.quantities", "dw.tools.math.ir", "dw.tools.math.codecs",
        "dw.tools.math.symbolics", "dw.tools.math.numerics",
        "dw.tools.math.composition"
    ]
};
const string Workspace = "build/artifacts/m8-rs233-net11-thirteen-project-graph";
var root = Path.GetFullPath(Workspace);
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\n  \"sdk\": { \"version\": \"11.0.100\", \"rollForward\": \"disable\", \"allowPrerelease\": false }\n}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
int sourceCount = 0;
int edgeCount = 0;
foreach (var (id, dependencies) in expected)
{
    var sourceRoot = Path.Combine("src", "projects", id);
    var sourceProject = XDocument.Load(Path.Combine(sourceRoot, id + ".csproj"));
    var refs = sourceProject.Descendants().Where(x => x.Name.LocalName == "ProjectReference")
        .Select(x => x.Attribute("Include")?.Value ??
            throw new InvalidDataException("Missing reference include: " + id))
        .Select(x => Path.GetFileNameWithoutExtension(x.Replace('\\', '/'))).ToArray();
    Require(refs.Length == dependencies.Length &&
        refs.ToHashSet(StringComparer.Ordinal).SetEquals(dependencies) &&
        refs.All(expected.ContainsKey), "Unexpected direct references: " + id);
    edgeCount += refs.Length;
    var target = Path.Combine(root, id);
    Directory.CreateDirectory(target);
    var files = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
        .Where(x => !x.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                    !x.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
        .ToArray();
    Require(files.Length > 0, "Missing source files: " + id);
    foreach (var file in files)
    {
        var output = Path.Combine(target, Path.GetRelativePath(sourceRoot, file));
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        File.Copy(file, output, overwrite: true);
    }
    sourceCount += files.Length;
    var generated = new XDocument(new XElement("Project",
        new XAttribute("Sdk", "Microsoft.NET.Sdk"),
        new XElement("PropertyGroup",
            new XElement("AssemblyName", id),
            new XElement("RootNamespace", sourceProject.Descendants()
                .Single(x => x.Name.LocalName == "RootNamespace").Value)),
        new XElement("ItemGroup", refs.Select(dependency =>
            new XElement("ProjectReference",
                new XAttribute("Include",
                    "../" + dependency + "/" + dependency + ".csproj"))))));
    generated.Save(Path.Combine(target, id + ".csproj"));
}
Require(expected.Count == 13 && edgeCount == 16 && expected["dw.tools.math.distribution"].Length == 6,
    "Thirteen-project Math graph must retain sixteen direct edges.");
var consumer = Path.Combine(root, "consumer");
Directory.CreateDirectory(consumer);
File.WriteAllText(Path.Combine(consumer, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "</PropertyGroup><ItemGroup>" +
    string.Concat(expected.Keys.Select(id => "<ProjectReference Include=\"../" + id +
        "/" + id + ".csproj\" />")) +
    "</ItemGroup></Project>\n");
File.WriteAllText(Path.Combine(consumer, "Program.cs"),
    "using System;\nusing System.Reflection;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Wrong runtime.\");\n" +
    "var assembly = Assembly.Load(\"dw.tools.math.distribution\");\n" +
    "if (!Array.Exists(assembly.GetTypes(), t => t.Namespace?.StartsWith(" +
    "\"Dw.Tools.Math.Distribution\", StringComparison.Ordinal) == true))\n" +
    "    throw new InvalidOperationException(\"Distribution types absent.\");\n" +
    "Console.WriteLine(\"MATH-RS233-ALL-PACKABLE|runtime=11|projects=13|edges=16|PASS\");\n");
string Run(params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS233 could not start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS233 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK mismatch.");
Run("restore", "consumer/consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "consumer/consumer.csproj", "-c", "Release", "--no-restore");
var result = Run("run", "--project", "consumer/consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(result.Contains("MATH-RS233-ALL-PACKABLE|runtime=11|projects=13|edges=16|PASS",
    StringComparison.Ordinal), "Independent distribution consumer failure.");
Console.WriteLine("MATH-RS233-NET11-ALL-PACKABLE|sdk=11.0.100|tfm=net11.0|lang=15.0|" +
    "projects=13|edges=16|sourceFiles=" + sourceCount +
    "|published=0|productionProjectsChanged=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS233: " + message);
}
