#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string ScientificDirectory = "src/projects/dw.tools.math.scientific";
const string Workspace = "build/artifacts/m8-rs230-net11-scientific-source";
var project = XDocument.Load(Path.Combine(ScientificDirectory, "dw.tools.math.scientific.csproj"));
Require(!project.Descendants().Any(x => x.Name.LocalName == "ProjectReference"),
    "The standalone scientific source dependency boundary changed.");
var sources = Directory.GetFiles(ScientificDirectory, "*.cs", SearchOption.TopDirectoryOnly);
Require(sources.Length > 0, "The standalone scientific source boundary is empty.");
var root = Path.GetFullPath(Workspace);
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\n  \"sdk\": { \"version\": \"11.0.100\", \"rollForward\": \"disable\", \"allowPrerelease\": false }\n}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(root, "scientific-source-consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
var destination = Path.Combine(root, Path.GetFileName(ScientificDirectory));
Directory.CreateDirectory(destination);
foreach (var source in sources)
    File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), overwrite: true);
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\n" +
    "using System.Reflection;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Wrong runtime.\");\n" +
    "var types = Assembly.GetExecutingAssembly().GetTypes();\n" +
    "if (!Array.Exists(types, t => t.Namespace?.StartsWith(\"Dw.Tools.Math.Scientific\", StringComparison.Ordinal) == true))\n" +
    "    throw new InvalidOperationException(\"Math scientific source types absent.\");\n" +
    "Console.WriteLine(\"MATH-RS230-SCIENTIFIC|runtime=11|scientificTypes=present|PASS\");\n");
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var item in arguments) info.ArgumentList.Add(item);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS230: cannot start dotnet.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS230 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + output + "\n" + error);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK must be 11.0.100.");
Run("restore", "scientific-source-consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "scientific-source-consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "scientific-source-consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(output.Contains("MATH-RS230-SCIENTIFIC|runtime=11|scientificTypes=present|PASS",
    StringComparison.Ordinal), "Analysis direct-source consumer verdict.");
Console.WriteLine("MATH-RS230-NET11-SCIENTIFIC|sdk=11.0.100|tfm=net11.0|lang=15.0|scientificSources=" +
    sources.Length + "|published=0|mainProjectsChanged=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS230: " + message);
}
