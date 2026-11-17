#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Runtime.InteropServices;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS243 invokes WSL from the Windows operator.");

const string SourceDirectory = "src/projects/dw.quantities";
const string Workspace = "build/artifacts/m8-rs243-wsl-parser-source";
var sources = Directory.GetFiles(SourceDirectory, "*.cs", SearchOption.TopDirectoryOnly)
    .OrderBy(Path.GetFileName, StringComparer.Ordinal).ToArray();
Require(sources.Length == 9 &&
    sources.Any(p => Path.GetFileName(p) == "ExactRational.cs") &&
    sources.Any(p => Path.GetFileName(p) == "Quantity.cs"),
    "The nine-file quantities baseline changed.");
var root = Path.GetFullPath(Workspace);
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root, "global.json"),
    "{\n \"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}\n}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(root, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
    "</PropertyGroup></Project>\n");
foreach (var source in sources)
    File.Copy(source, Path.Combine(root, Path.GetFileName(source)), overwrite: true);
File.WriteAllText(Path.Combine(root, "Program.cs"), """
    using System;
    using System.Runtime.InteropServices;
    using dw.quantities;

    if (!OperatingSystem.IsLinux() || RuntimeInformation.OSArchitecture != Architecture.X64 ||
        Environment.Version.Major != 11)
        throw new InvalidOperationException("Incorrect Linux runtime.");
    var positive = ExactRational.ParseInvariantDecimal("+1.250");
    var negative = ExactRational.ParseInvariant("-2 1/3");
    var tiny = ExactRational.ParseInvariant(".005");
    var zero = ExactRational.ParseInvariantDecimal("-0.000");
    var fraction = ExactRational.ParseInvariant("6/8");
    var denominatorRefused = false;
    try { _ = ExactRational.ParseInvariant("1/0"); }
    catch (FormatException) { denominatorRefused = true; }
    var decimalRefused = false;
    try { _ = ExactRational.ParseInvariantDecimal("1,5"); }
    catch (FormatException) { decimalRefused = true; }
    if (positive.ToFractionString() != "5/4" ||
        negative.ToFractionString() != "-7/3" ||
        negative.ToMixedNumberString() != "-2 1/3" ||
        tiny.ToFractionString() != "1/200" ||
        zero.ToFractionString() != "0" ||
        fraction.ToFractionString() != "3/4" ||
        !denominatorRefused || !decimalRefused)
        throw new InvalidOperationException("Linux exact parser canonical oracle failed.");
    Console.WriteLine("MATH-RS243-LINUX-PARSER|os=linux|arch=x64|runtime=11|" +
        "positive=5/4|negative=-7/3|tiny=1/200|zero=0|fraction=3/4|" +
        "invalidDenominator=refused|invalidDecimal=refused|PASS");
    """);
string Wsl(params string[] arguments)
{
    var info = new ProcessStartInfo("wsl.exe")
    {
        UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS243 cannot start WSL.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS243 WSL failed: " + string.Join(" ", arguments) +
            "\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
var linuxRoot = Wsl("--exec", "wslpath", "-a", "-u", root);
Require(linuxRoot.StartsWith("/", StringComparison.Ordinal) &&
        !linuxRoot.Contains('\n'), "WSL workspace path mapping failed.");
string RunLinux(params string[] arguments)
{
    var command = new List<string>
    {
        "--exec", "bash", "-lc", "cd \"$1\" && shift && dotnet \"$@\"", "bash", linuxRoot
    };
    command.AddRange(arguments);
    return Wsl(command.ToArray());
}
Require(RunLinux("--version") == "11.0.100", "Isolated WSL SDK 11.0.100 not selected.");
RunLinux("restore", "consumer.csproj", "--configfile", "NuGet.Config");
RunLinux("build", "consumer.csproj", "-c", "Release", "--no-restore");
var result = RunLinux("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore");
Require(result == "MATH-RS243-LINUX-PARSER|os=linux|arch=x64|runtime=11|" +
    "positive=5/4|negative=-7/3|tiny=1/200|zero=0|fraction=3/4|" +
    "invalidDenominator=refused|invalidDecimal=refused|PASS",
    "Linux exact parser differs from the RS238 canonical oracle.");
Console.WriteLine("MATH-RS243-WSL-PARSER|distro=ubuntu-24.04|sdk=11.0.100|" +
    "tfm=net11.0|lang=15.0|sources=9|consumer=pass|" +
    "published=0|productionMutation=false|PASS");
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS243: " + message);
}
