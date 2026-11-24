#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS323 qualifies the Windows SDK 11 locked package consumer.");

const string Profile = "docs/engineering/m8-w02-c01-published-standard-closure-rs322.json";
const string Fixture = "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs";
using var profile = JsonDocument.Parse(File.ReadAllText(Profile));
var rows = profile.RootElement.GetProperty("packages").EnumerateArray().ToArray();
Need(rows.Length == 3 &&
    rows[0].GetProperty("packageId").GetString() == "dw.quantities.standard" &&
    rows[1].GetProperty("packageId").GetString() == "dw.quantities.expression" &&
    rows[2].GetProperty("packageId").GetString() == "dw.quantities" &&
    profile.RootElement.GetProperty("archiveTargetFramework").GetString() == "net10.0" &&
    profile.RootElement.GetProperty("observedSdk").GetString() == "11.0.100",
    "The RS322 three-archive identity/order changed.");

using var global = JsonDocument.Parse(File.ReadAllText("global.json"));
var build = XDocument.Load("Directory.Build.props");
Need(global.RootElement.GetProperty("sdk").GetProperty("version").GetString() == "10.0.401" &&
    build.Descendants().Single(x => x.Name.LocalName == "TargetFramework").Value == "net10.0" &&
    build.Descendants().Single(x => x.Name.LocalName == "LangVersion").Value == "14.0",
    "Production SDK/TFM/language unexpectedly changed.");
Need(File.Exists(Fixture), "Original Math ten-check published-binary fixture missing.");

var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute private feed.");

var root = Path.GetFullPath(Path.Combine(
    "build", "artifacts", "l323-" + Guid.NewGuid().ToString("N")[..10]));
var copiedFeed = Path.Combine(root, "feed");
var packageCache = Path.Combine(root, "cache");
Directory.CreateDirectory(copiedFeed);
var originals = new Dictionary<string, (string Path, string Hash)>(StringComparer.Ordinal);
foreach (var item in rows)
{
    var id = item.GetProperty("packageId").GetString()!;
    var version = item.GetProperty("version").GetString()!;
    var hash = item.GetProperty("archiveSha256").GetString()!;
    var archive = Path.Combine(feed!, id + "." + version + ".nupkg");
    Need(File.Exists(archive) &&
        (File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0 &&
        Hash(archive) == hash,
        "Original immutable private archive does not match RS322: " + id);
    var copy = Path.Combine(copiedFeed, Path.GetFileName(archive));
    File.Copy(archive, copy);
    Need(Hash(copy) == hash, "Copied immutable archive mismatch: " + id);
    originals.Add(id, (archive, hash));
}

File.WriteAllText(Path.Combine(root, "global.json"),
    "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
    "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
    "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
    "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
    "<BaseOutputPath>out/bin/</BaseOutputPath>" +
    "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
    "</PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root, "NuGet.Config"),
    "<configuration><packageSources><clear /></packageSources></configuration>\n");

const string StandardVersion = "[20260.20261103.1001]";
const string BaseVersion = "[20260.20261102.1001]";
var projectPath = Path.Combine(root, "consumer.csproj");
var cleanProject =
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.quantities.standard\"" +
    " Version=\"" + StandardVersion + "\" /></ItemGroup></Project>\n";
File.WriteAllText(projectPath, cleanProject);
File.Copy(Fixture, Path.Combine(root, "Program.cs"));
var cleanProjectHash = Hash(projectPath);

ProcessResult Run(params string[] cmd) => Execute(root, cmd);
void Pass(params string[] cmd)
{
    var result = Run(cmd);
    Need(result.ExitCode == 0,
        "Isolated dotnet " + string.Join(" ", cmd) + " refused: " + result.Output);
}
string[] Restore(bool locked) => locked
    ? ["restore", "consumer.csproj", "--configfile", "NuGet.Config",
       "--source", copiedFeed, "--packages", packageCache,
       "--locked-mode", "-p:NuGetAudit=false"]
    : ["restore", "consumer.csproj", "--configfile", "NuGet.Config",
       "--source", copiedFeed, "--packages", packageCache,
       "-p:NuGetAudit=false"];

var sdk = Run("--version");
Need(sdk.ExitCode == 0 && sdk.Output.Trim() == "11.0.100",
    "The isolated consumer must select the installed stable SDK 11.0.100.");
Pass(Restore(false));
var lockPath = Path.Combine(root, "packages.lock.json");
Need(File.Exists(lockPath), "Initial isolated restore did not generate a package lock.");
using (var locked = JsonDocument.Parse(File.ReadAllText(lockPath)))
{
    var frameworks = locked.RootElement.GetProperty("dependencies");
    Need(frameworks.EnumerateObject().Count() == 1 &&
        frameworks.TryGetProperty("net11.0", out var graph),
        "The package lock is not exclusively net11.0.");
    var entries = graph.EnumerateObject().ToDictionary(x => x.Name, x => x.Value,
        StringComparer.OrdinalIgnoreCase);
    Need(entries.Count == 3, "The lock has an unqualified additional package.");
    foreach (var item in rows)
    {
        var id = item.GetProperty("packageId").GetString()!;
        Need(entries.TryGetValue(id, out var node) &&
            node.GetProperty("resolved").GetString() ==
                item.GetProperty("version").GetString() &&
            node.GetProperty("type").GetString() ==
                item.GetProperty("topology").GetString(),
            "The three-package locked dependency topology differs: " + id);
    }
}

var lockHash = Hash(lockPath);
Pass(Restore(true));
Need(Hash(lockPath) == lockHash, "Locked restore rewrote its exact package graph.");
Pass("build", "consumer.csproj", "-c", "Release", "--no-restore");
var output = Run("run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--",
    rows[0].GetProperty("binarySha256").GetString()!,
    rows[1].GetProperty("binarySha256").GetString()!,
    rows[2].GetProperty("binarySha256").GetString()!);
Need(output.ExitCode == 0 &&
    output.Output.Contains("MATH-STANDARD-BINARY|PASS|10", StringComparison.Ordinal) &&
    output.Output.Contains("MATH-STANDARD-CHECK|loaded-dll-hashes|PASS",
        StringComparison.Ordinal) &&
    output.Output.Contains("MATH-STANDARD-CHECK|inherited-catalog-58|PASS",
        StringComparison.Ordinal),
    "Unmodified ten-check Standard package-only consumer failed: " + output.Output);

int refusals = 0;
void Reject(string label, string mutatedProject)
{
    Need(mutatedProject != cleanProject,
        "Negative control failed to change the copied project: " + label);
    File.WriteAllText(projectPath, mutatedProject);
    try
    {
        var result = Run(Restore(true));
        Need(result.ExitCode != 0 &&
            (result.Output.Contains("NU1004", StringComparison.OrdinalIgnoreCase) ||
             result.Output.Contains("NU1102", StringComparison.OrdinalIgnoreCase) ||
             result.Output.Contains("NU1101", StringComparison.OrdinalIgnoreCase)),
            "Locked restore did not refuse " + label + ": " + result.Output);
        refusals++;
        Console.WriteLine("MATH-RS323-LOCKED-REFUSAL|" + label + "|PASS");
    }
    finally
    {
        File.WriteAllText(projectPath, cleanProject);
    }
    Need(Hash(projectPath) == cleanProjectHash &&
        Hash(lockPath) == lockHash,
        "Copied consumer or exact lock mutated after refused " + label);
    Pass(Restore(true));
    Need(Hash(lockPath) == lockHash,
        "Clean locked restore changed its graph after " + label);
}

Reject("unavailable-standard-pin",
    cleanProject.Replace(StandardVersion, "[20260.20261103.9999]",
        StringComparison.Ordinal));
Reject("promote-transitive-base-to-direct",
    cleanProject.Replace("</ItemGroup>",
        "<PackageReference Include=\"dw.quantities\" Version=\"" +
        BaseVersion + "\" /></ItemGroup>", StringComparison.Ordinal));

Need(refusals == 2, "Expected two independent locked restore refusals.");
foreach (var (id, original) in originals)
{
    Need(Hash(original.Path) == original.Hash &&
        Hash(Path.Combine(copiedFeed, Path.GetFileName(original.Path))) == original.Hash,
        "Source feed or copied physical package changed: " + id);
}
Need(Hash(projectPath) == cleanProjectHash && Hash(lockPath) == lockHash,
    "Final independent consumer or lock differs from the qualified clean state.");

Console.WriteLine("MATH-RS323-STANDARD-LIVE-LOCK|sdk=11.0.100|tfm=net11.0|" +
    "packages=3|direct=1|transitive=2|binaryChecks=10|" +
    "lockedRestore=PASS|wrongVersion=refused|topologyDrift=refused|" +
    "cleanRestored=2|archivesUnchanged=true|otherTen=unqualified|" +
    "productionMigration=false|guidanceUpgrade=false|PASS");

static string Hash(string file)
{
    using var stream = File.OpenRead(file);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}

static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS323: " + reason);
}

static ProcessResult Execute(string directory, string[] cmd)
{
    var psi = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = directory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var token in cmd) psi.ArgumentList.Add(token);
    using var process = Process.Start(psi) ??
        throw new InvalidOperationException("RS323: Cannot launch isolated dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(150000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS323: Independent package consumer timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}

sealed record ProcessResult(int ExitCode, string Output);
