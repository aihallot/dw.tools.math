#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

if (args.Length != 1 || (args[0] != "--check" && args[0] != "--check-integrated"))
    throw new ArgumentException("Expected --check or --check-integrated.");
var integrated = args[0] == "--check-integrated";
if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("RS342 bounds Windows SDKs.");
const string Source = "src/projects/dw.quantities/ExactRational.cs";
const string Fixture = "docs/engineering/fixtures/M8ExactRationalSpanModernizationConsumer.cs";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Profile = "docs/engineering/m8-w02-c02-phase-acceptance-rs341.json";
const string Old = "var digits = string.Concat(integral.IsEmpty ? \"0\" : integral.ToString(), fractional.ToString());";
const string New = "var digits = string.Concat(integral.IsEmpty ? \"0\".AsSpan() : integral, fractional);";
var tracked = new[] { Source, Fixture, Risk, Profile, "global.json", "Directory.Build.props",
    "src/projects/dw.quantities/dw.quantities.csproj" }.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var actual = File.ReadAllText(Source);
Need(integrated
    ? actual.Split(New, StringSplitOptions.None).Length == 2 && !actual.Contains(Old, StringComparison.Ordinal)
    : actual.Split(Old, StringSplitOptions.None).Length == 2 && !actual.Contains(New, StringComparison.Ordinal),
    "Expected exact source anchor for selected mode.");
var original = integrated ? actual.Replace(New, Old, StringComparison.Ordinal) : actual;
var candidate = original.Replace(Old, New, StringComparison.Ordinal);
Need(candidate.Replace(New, Old, StringComparison.Ordinal) == original && (!integrated || candidate == actual),
    "Unexpected source delta.");
using (var risk = JsonDocument.Parse(File.ReadAllText(Risk)))
using (var profile = JsonDocument.Parse(File.ReadAllText(Profile)))
    Need(risk.RootElement.GetProperty("gates").GetProperty("migrationAuthorization").GetString() == "not-granted" &&
        profile.RootElement.GetProperty("boundedNewNet11PackageGraphQualified").GetBoolean() &&
        !profile.RootElement.GetProperty("phaseClosureAuthorized").GetBoolean(), "Acceptance or authorization drift.");
var work = Path.GetFullPath(Path.Combine("build", "artifacts", "f342-" + Guid.NewGuid().ToString("N")[..8]));
Directory.CreateDirectory(work);
long RunCase(string name, int major, bool changed)
{
    var root = Path.Combine(work, name);
    Directory.CreateDirectory(root);
    File.WriteAllText(Path.Combine(root, "global.json"), major == 10
        ? "{\"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}}\n"
        : "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
    File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
        "<Project><PropertyGroup><TargetFramework>net" + major + ".0</TargetFramework>" +
        "<LangVersion>" + (major == 10 ? "14.0" : "15.0") + "</LangVersion>" +
        "<Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
        "<EnableNETAnalyzers>true</EnableNETAnalyzers><AnalysisLevel>latest</AnalysisLevel>" +
        "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
        "<BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
        "</PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(root, "Directory.Packages.props"),
        "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(root, "Directory.Build.targets"), "<Project />\n");
    File.WriteAllText(Path.Combine(root, "NuGet.Config"), "<configuration><packageSources><clear /></packageSources></configuration>\n");
    File.WriteAllText(Path.Combine(root, "consumer.csproj"),
        "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><IsPackable>false</IsPackable>" +
        "<NuGetAudit>false</NuGetAudit></PropertyGroup></Project>\n");
    var source = Path.Combine(root, "ExactRational.cs");
    if (changed == integrated) File.Copy(Source, source); else File.WriteAllText(source, changed ? candidate : original);
    File.Copy(Fixture, Path.Combine(root, "Program.cs"));
    Need(File.ReadAllText(source) == (changed ? candidate : original), "Source copy drift.");
    var sdk = Pass(root, "--version").Trim();
    Need(major == 10 ? sdk.StartsWith("10.0.", StringComparison.Ordinal) : sdk == "11.0.100", "SDK mismatch.");
    Pass(root, "restore", "consumer.csproj", "--configfile", "NuGet.Config", "--packages", Path.Combine(root, "cache"));
    Pass(root, "build", "consumer.csproj", "-c", "Release", "--no-restore");
    var output = Pass(root, "run", "--project", "consumer.csproj", "-c", "Release", "--no-build", "--no-restore",
        "--", major.ToString(CultureInfo.InvariantCulture));
    Need(output.Contains("MATH-RS342-ORACLE|PASS", StringComparison.Ordinal), "Oracle missing: " + name);
    var allocation = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        .Single(x => x.StartsWith("MATH-RS342-ALLOC|", StringComparison.Ordinal));
    var bytes = long.Parse(allocation.Split('|')[1], CultureInfo.InvariantCulture);
    Console.WriteLine("MATH-RS342-CASE|" + name + "|sdk=" + sdk + "|allocated=" + bytes + "|PASS");
    return bytes;
}
var net10 = RunCase("net10-original", 10, false);
var net11 = RunCase("net11-original", 11, false);
var modernized = RunCase("net11-candidate", 11, true);
if (integrated)
{
    var currentNet10 = RunCase("net10-integrated", 10, true);
    Need(currentNet10 < net10, "Integrated net10 source did not reduce same-runtime allocations.");
    Console.WriteLine("MATH-RS343-NET10-INTEGRATED|baseline=" + net10 + "|candidate=" + currentNet10 + "|PASS");
}
Need(modernized < net11, "Candidate did not reduce measured same-runtime allocations.");
foreach (var item in tracked) Need(Hash(item.Key) == item.Value, "Original changed: " + item.Key);
Console.WriteLine("MATH-RS342-SPAN-MODERNIZATION|net10=" + net10 + "|net11=" + net11 +
    "|candidate=" + modernized + "|saved=" + (net11 - modernized) +
    "|iterations=4096|scope=isolated-source|productionMutation=false|PASS");
if (integrated) Console.WriteLine("MATH-RS343-INTEGRATED|source=actual-repository|net10=PASS|net11=PASS|allocationReduction=both|PASS");

static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
static void Need(bool ok, string why) { if (!ok) throw new InvalidDataException("RS342: " + why); }
static string Pass(string cwd, params string[] args)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = cwd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    // Compare allocation under the same fixed runtime execution policy.
    info.Environment["DOTNET_TieredCompilation"] = "0";
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS342 dotnet timed out: " + string.Join(" ", args));
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Need(process.ExitCode == 0, "dotnet " + string.Join(" ", args) + " failed: " + output);
    return output.Trim();
}
