#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string SourceDirectory = "src/projects/dw.quantities";
const string Workspace = "build/artifacts/m8-rs219-net11-quantities-source";
var sources = Directory.GetFiles(SourceDirectory, "*.cs", SearchOption.TopDirectoryOnly)
    .OrderBy(Path.GetFileName, StringComparer.Ordinal).ToArray();
Require(sources.Length == 9 && sources.Any(p => Path.GetFileName(p) == "ExactRational.cs") &&
    sources.Any(p => Path.GetFileName(p) == "Quantity.cs") &&
    sources.Any(p => Path.GetFileName(p) == "UnitDefinition.cs"),
    "The nine-file exact quantities source boundary changed.");
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
File.WriteAllText(Path.Combine(root, "quantities-source-consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
foreach (var source in sources)
    File.Copy(source, Path.Combine(root, Path.GetFileName(source)), overwrite: true);
File.WriteAllText(Path.Combine(root, "Program.cs"),
    "using System;\nusing dw.quantities;\n" +
    "if (Environment.Version.Major != 11) throw new InvalidOperationException(\"Wrong runtime.\");\n" +
    "var exact = new ExactRational(2, 4) + new ExactRational(1, 4);\n" +
    "if (exact != new ExactRational(3, 4) || exact.ToFractionString() != \"3/4\")\n" +
    "    throw new InvalidOperationException(\"Exact rational semantics changed.\");\n" +
    "var quantity = new Quantity(exact, DimensionVector.LengthDimension);\n" +
    "if (!quantity.TryAdd(new Quantity(new ExactRational(1, 4), DimensionVector.LengthDimension), out var sum)\n" +
    "    || sum.Value != ExactRational.One)\n" +
    "    throw new InvalidOperationException(\"Exact quantity semantics changed.\");\n" +
    "Console.WriteLine(\"MATH-RS219-QUANTITIES|runtime=11|rational=3/4|sum=1|PASS\");\n");
string Run(params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var item in arguments) info.ArgumentList.Add(item);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS219: cannot start dotnet.");
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS219 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + output + "\n" + error);
    return output.Trim();
}
Require(Run("--version") == "11.0.100", "Isolated SDK 11.0.100 pin.");
Run("restore", "quantities-source-consumer.csproj", "--configfile", "NuGet.Config");
Run("build", "quantities-source-consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "quantities-source-consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(output.Contains("MATH-RS219-QUANTITIES|runtime=11|rational=3/4|sum=1|PASS",
    StringComparison.Ordinal), "Exact Math quantity consumer verdict.");
Console.WriteLine("MATH-RS219-NET11-QUANTITIES|sdk=11.0.100|tfm=net11.0|lang=15.0|sourceFiles=9|published=0|projectMutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS219: " + message);
}
