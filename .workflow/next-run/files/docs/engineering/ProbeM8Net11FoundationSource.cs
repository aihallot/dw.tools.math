#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Source = "src/projects/dw.tools.math.foundation/FoundationContract.cs";
const string Work = "build/artifacts/m8-rs218-net11-foundation-source";
var source = File.ReadAllText(Source);
Require(source.Contains("public static class FoundationContract", StringComparison.Ordinal) &&
    source.Contains("public const string ContractVersion = \"0.1\"", StringComparison.Ordinal),
    "The accepted Foundation public source identity changed.");
var root = Path.GetFullPath(Work);
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
File.WriteAllText(Path.Combine(root, "foundation-source-consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "FoundationContract.cs"), source);
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\nusing Dw.Tools.Math.Foundation;\n" +
    "if (Environment.Version.Major != 11 ||\n" +
    "    FoundationContract.Identity != \"dw.tools.math.foundation/0.1\")\n" +
    "    throw new InvalidOperationException(\"Math Foundation source contract changed.\");\n" +
    "Console.WriteLine(\"MATH-RS218-FOUNDATION|runtime=11|identity=dw.tools.math.foundation/0.1|PASS\");\n");
string Run(params string[] commandArguments)
{
    var processInfo = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var arg in commandArguments) processInfo.ArgumentList.Add(arg);
    using var process = Process.Start(processInfo) ??
        throw new InvalidOperationException("RS218: dotnet could not start.");
    var output = process.StandardOutput.ReadToEnd();
    var errors = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS218: dotnet " + string.Join(" ", commandArguments) +
            " failed:\n" + output + "\n" + errors);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "SDK 11.0.100 must be selected in isolation.");
Run("restore", "foundation-source-consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "foundation-source-consumer.csproj", "-c", "Release", "--no-restore");
var result = Run("run", "--project", "foundation-source-consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(result.Contains("MATH-RS218-FOUNDATION|runtime=11|identity=dw.tools.math.foundation/0.1|PASS",
    StringComparison.Ordinal), "Math Foundation source consumer failed.");
Console.WriteLine("MATH-RS218-NET11-FOUNDATION-SOURCE|sdk=11.0.100|tfm=net11.0|lang=15.0|realMathSources=1|packagesPublished=0|mainProjectsChanged=false|PASS");
static void Require(bool ok, string detail)
{
    if (!ok) throw new InvalidDataException("RS218: " + detail);
}
