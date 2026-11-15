#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string QuantityDirectory = "src/projects/dw.quantities";
const string IrDirectory = "src/projects/dw.tools.math.ir";
const string CodecsDirectory = "src/projects/dw.tools.math.codecs";
const string Workspace = "build/artifacts/m8-rs225-net11-codecs-source";
var reference = XDocument.Load(Path.Combine(IrDirectory, "dw.tools.math.ir.csproj"))
    .Descendants().Single(x => x.Name.LocalName == "ProjectReference")
    .Attribute("Include")?.Value;
Require(reference?.Replace('\\', '/') == "../dw.quantities/dw.quantities.csproj",
    "The IR direct-source dependency is no longer the accepted quantities project.");
var codecsReferences = XDocument.Load(Path.Combine(CodecsDirectory, "dw.tools.math.codecs.csproj"))
    .Descendants().Where(x => x.Name.LocalName == "ProjectReference")
    .Select(x => x.Attribute("Include")?.Value?.Replace('\\', '/'))
    .ToHashSet(StringComparer.Ordinal);
Require(codecsReferences.SetEquals(new[]
{
    "../dw.tools.math.ir/dw.tools.math.ir.csproj"
}), "The codecs project dependency boundary changed.");
var quantities = Directory.GetFiles(QuantityDirectory, "*.cs", SearchOption.TopDirectoryOnly);
var ir = Directory.GetFiles(IrDirectory, "*.cs", SearchOption.TopDirectoryOnly);
var codecs = Directory.GetFiles(CodecsDirectory, "*.cs", SearchOption.TopDirectoryOnly);
Require(quantities.Length == 9 && ir.Length > 0 && codecs.Length > 0 &&
    quantities.Any(p => Path.GetFileName(p) == "ExactRational.cs"),
    "The qualified source boundaries changed.");
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
File.WriteAllText(Path.Combine(root, "codecs-source-consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
foreach (var directory in new[] { QuantityDirectory, IrDirectory, CodecsDirectory })
{
    var destination = Path.Combine(root, Path.GetFileName(directory));
    Directory.CreateDirectory(destination);
    foreach (var source in Directory.GetFiles(directory, "*.cs", SearchOption.TopDirectoryOnly))
        File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), overwrite: true);
}
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\nusing dw.quantities;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Wrong runtime.\");\n" +
    "var rational = new ExactRational(2, 4);\n" +
    "if (rational.ToFractionString() != \"1/2\") throw new InvalidOperationException(\"Exact rational regression.\");\n" +
    "var codecsTypes = typeof(ExactRational).Assembly.GetTypes();\n" +
    "if (!Array.Exists(codecsTypes, t => t.Namespace?.StartsWith(\"Dw.Tools.Math.Codecs\", StringComparison.Ordinal) == true))\n" +
    "    throw new InvalidOperationException(\"Codecs source types absent.\");\n" +
    "Console.WriteLine(\"MATH-RS225-CODECS|runtime=11|rational=1/2|codecsTypes=present|PASS\");\n");
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var item in arguments) info.ArgumentList.Add(item);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS225: cannot start dotnet.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS225 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + output + "\n" + error);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK must be 11.0.100.");
Run("restore", "codecs-source-consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "codecs-source-consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "codecs-source-consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(output.Contains("MATH-RS225-CODECS|runtime=11|rational=1/2|codecsTypes=present|PASS",
    StringComparison.Ordinal), "Codecs direct-source consumer verdict.");
Console.WriteLine("MATH-RS225-NET11-CODECS|sdk=11.0.100|tfm=net11.0|lang=15.0|quantitiesSources=" +
    quantities.Length + "|irSources=" + ir.Length + "|codecsSources=" + codecs.Length + "|published=0|mainProjectsChanged=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS225: " + message);
}
