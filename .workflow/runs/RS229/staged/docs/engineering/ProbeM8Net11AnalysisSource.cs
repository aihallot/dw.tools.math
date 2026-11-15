#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string AnalysisDirectory = "src/projects/dw.tools.math.analysis";
const string Workspace = "build/artifacts/m8-rs229-net11-analysis-source";
var project = XDocument.Load(Path.Combine(AnalysisDirectory, "dw.tools.math.analysis.csproj"));
Require(!project.Descendants().Any(x => x.Name.LocalName == "ProjectReference"),
    "The standalone analysis source dependency boundary changed.");
var sources = Directory.GetFiles(AnalysisDirectory, "*.cs", SearchOption.TopDirectoryOnly);
Require(sources.Length > 0, "The standalone analysis source boundary is empty.");
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
File.WriteAllText(Path.Combine(root, "analysis-source-consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
var destination = Path.Combine(root, Path.GetFileName(AnalysisDirectory));
Directory.CreateDirectory(destination);
foreach (var source in sources)
    File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), overwrite: true);
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\n" +
    "using System.Reflection;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Wrong runtime.\");\n" +
    "var types = Assembly.GetExecutingAssembly().GetTypes();\n" +
    "if (!Array.Exists(types, t => t.Namespace?.StartsWith(\"Dw.Tools.Math.Analysis\", StringComparison.Ordinal) == true))\n" +
    "    throw new InvalidOperationException(\"Math analysis source types absent.\");\n" +
    "Console.WriteLine(\"MATH-RS229-ANALYSIS|runtime=11|analysisTypes=present|PASS\");\n");
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var item in arguments) info.ArgumentList.Add(item);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS229: cannot start dotnet.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS229 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + output + "\n" + error);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK must be 11.0.100.");
Run("restore", "analysis-source-consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "analysis-source-consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "analysis-source-consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(output.Contains("MATH-RS229-ANALYSIS|runtime=11|analysisTypes=present|PASS",
    StringComparison.Ordinal), "Analysis direct-source consumer verdict.");
Console.WriteLine("MATH-RS229-NET11-ANALYSIS|sdk=11.0.100|tfm=net11.0|lang=15.0|analysisSources=" +
    sources.Length + "|published=0|mainProjectsChanged=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS229: " + message);
}
