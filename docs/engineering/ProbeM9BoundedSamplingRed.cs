#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check") throw new ArgumentException("Expected --check.");
const string Source = "src/projects/dw.tools.math.statistics/SplitMix64Random.cs";
const string Fixture = "docs/engineering/fixtures/M9BoundedSamplingRedOracle.cs";
var original = File.ReadAllText(Source);
var inputHashes = new[] { Source, Fixture }.ToDictionary(x => x, x => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(x))), StringComparer.Ordinal);
var root = Path.GetFullPath(Path.Combine("build", "artifacts", "f350-" + Guid.NewGuid().ToString("N")[..8]));
Directory.CreateDirectory(root);
var cases = new (string Name, string Old, string New, string Refusal)[]
{
    ("rejection", "if (raw >= threshold)", "if (raw >= threshold || threshold != 0UL)", "rejection"),
    ("budget", "attempt < maxRawDraws", "attempt <= maxRawDraws", "budget"),
    ("signed", "^ signBit));", "^ (signBit >> 1)));", "signed interval")
};
Run("original", original, "");
foreach (var item in cases)
{
    if (original.Split(item.Old, StringSplitOptions.None).Length != 2)
        throw new InvalidDataException("Mutation anchor differs: " + item.Name);
    Run(item.Name, original.Replace(item.Old, item.New, StringComparison.Ordinal), item.Refusal);
}
foreach (var item in inputHashes)
    if (Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(item.Key))) != item.Value)
        throw new InvalidDataException("Original input changed: " + item.Key);
Console.WriteLine("MATH-RS350-RED|original=PASS|compiledMutantsRefused=3|productionMutation=false|PASS");

void Run(string name, string source, string refusal)
{
    var dir = Path.Combine(root, name);
    Directory.CreateDirectory(dir);
    File.WriteAllText(Path.Combine(dir, "global.json"), "{\"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}}\n");
    File.WriteAllText(Path.Combine(dir, "Directory.Build.props"), "<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath></PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(dir, "Directory.Build.targets"), "<Project />\n");
    File.WriteAllText(Path.Combine(dir, "Directory.Packages.props"), "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(dir, "NuGet.Config"), "<configuration><packageSources><clear /></packageSources></configuration>\n");
    File.WriteAllText(Path.Combine(dir, "oracle.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><NuGetAudit>false</NuGetAudit></PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(dir, "SplitMix64Random.cs"), source);
    File.Copy(Fixture, Path.Combine(dir, "Program.cs"));
    RequirePass(Execute(dir, "restore", "oracle.csproj", "--configfile", "NuGet.Config"));
    RequirePass(Execute(dir, "build", "oracle.csproj", "-c", "Release", "--no-restore"));
    var result = Execute(dir, "run", "--project", "oracle.csproj", "-c", "Release", "--no-build", "--no-restore");
    if (refusal.Length == 0)
    {
        RequirePass(result);
        if (!result.Output.Contains("M9-ORACLE-PASS", StringComparison.Ordinal)) throw new InvalidDataException("Original oracle marker missing.");
    }
    else if (result.ExitCode == 0 || !result.Output.Contains("M9-ORACLE: " + refusal, StringComparison.Ordinal))
        throw new InvalidDataException("Wrong mutant result: " + name + "\n" + result.Output);
    Console.WriteLine("MATH-RS350-CASE|" + name + "|PASS");
}
static void RequirePass((int ExitCode, string Output) result)
{
    if (result.ExitCode != 0) throw new InvalidDataException(result.Output);
}
static (int ExitCode, string Output) Execute(string dir, params string[] args)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = dir, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Oracle command timed out."); }
    return (process.ExitCode, stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
