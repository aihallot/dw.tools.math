#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Root = "build/artifacts/m8-rs216-sdk11-compiler-probe";
var root = Path.GetFullPath(Root);
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
File.WriteAllText(Path.Combine(root, "probe.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>" +
    "<OutputType>Exe</OutputType><IsPackable>false</IsPackable>" +
    "<NuGetAudit>false</NuGetAudit><RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Unexpected runtime.\");\n" +
    "Console.WriteLine(\"MATH-RS216-RUNTIME|framework=net11.0|runtime=11|PASS\");\n");
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS216: dotnet process did not start.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS216: dotnet " +
            string.Join(" ", arguments) + " failed:\n" + output + "\n" + error);
    return output.Trim();
}
var sdk = Run("--version");
if (sdk != "11.0.100")
    throw new InvalidDataException("RS216: isolated SDK was " + sdk + ", expected 11.0.100.");
Run("restore", "probe.csproj", "--configfile", "NuGet.Config");
var build = Run("build", "probe.csproj", "-c", "Release", "--no-restore");
if (!build.Contains("Build succeeded.", StringComparison.Ordinal))
    throw new InvalidDataException("RS216: net11/C#15 compilation was not confirmed.");
var result = Run("run", "--project", "probe.csproj", "-c", "Release", "--no-build", "--no-restore");
if (!result.Contains("MATH-RS216-RUNTIME|framework=net11.0|runtime=11|PASS", StringComparison.Ordinal))
    throw new InvalidDataException("RS216: isolated net11 runtime proof missing.");
Console.WriteLine("MATH-RS216-SDK11-COMPILER|sdk=11.0.100|tfm=net11.0|lang=15.0|compile=true|run=true|mathPackagesTested=0|repoToolchainChanged=false|PASS");
