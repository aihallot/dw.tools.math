#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS282 requires a Windows host with default WSL Linux.");

const string Exporter = "docs/discovery/qualification/ExportM8PortableDiscoverySkill.cs";
const string PublishedMarker =
    "M8-DISCOVERY-CLR-SEED|bindings=4|examples=5|positiveComposition=2|brokerReady=0|scope=package-reference-consumer";
var packages = new[]
{
    (Id: "dw.tools.math.composition", Version: "20260.20261107.1001",
     Archive: "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f",
     Binary: "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252"),
    (Id: "dw.tools.math.ir", Version: "20260.20261104.1001",
     Archive: "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
     Binary: "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
static void Need(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS282: " + reason);
}
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

Need(File.Exists(Exporter), "The RS281 source-owned portable exporter is absent.");
using (var receipt = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS281/attempt-001.json")))
{
    var run = receipt.RootElement;
    var prior = run.GetProperty("priorInvocationFailures");
    Need(run.GetProperty("runId").GetString() == "RS281" &&
        run.GetProperty("attempt").GetInt32() == 1 &&
        run.GetProperty("outcome").GetString() == "succeeded" &&
        run.GetProperty("git").GetProperty("commitMessage").GetString() ==
            "feat: export portable Math Discovery skill with locked net11 example" &&
        prior.GetProperty("count").GetInt32() == 3 &&
        prior.GetProperty("preparationFailureCount").GetInt32() == 3 &&
        prior.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
        !prior.GetProperty("firstPassSucceeded").GetBoolean() &&
        run.GetProperty("boundaryViolations").GetArrayLength() == 0,
        "The durable RS281 export qualification or retry history differs.");
}
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) &&
    Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "NUGET_LOCAL must designate an existing absolute private feed.");

string RunProcess(string command, string workingDirectory, params string[] arguments)
{
    var start = new ProcessStartInfo(command)
    {
        WorkingDirectory = workingDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS282 cannot start " + command);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS282 timed out: " + command);
    }
    var output = stdout.GetAwaiter().GetResult().Trim();
    var error = stderr.GetAwaiter().GetResult().Trim();
    Need(process.ExitCode == 0,
        command + " failed (" + process.ExitCode + "): " +
        string.Join(" ", arguments) + "\nstdout:\n" + output +
        "\nstderr:\n" + error);
    return output;
}

var repoRoot = Path.GetFullPath(".");
var root = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs282-portable-discovery-linux", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
var export = Path.Combine(root, "portable");
Need(!Directory.Exists(export), "An RS282 export destination already exists.");
var exported = RunProcess("dotnet", repoRoot, "run", "--file", Exporter,
    "--", "--export", export);
Need(exported.Contains(
    "MATH-RS281-PORTABLE-EXPORTED|files=12|privatePackages=not-included|authorization=runtime-only",
    StringComparison.Ordinal), "The existing RS281 exporter did not produce its verified bundle.");
Need(Directory.Exists(export) && File.Exists(Path.Combine(export, "global.json")) &&
    File.Exists(Path.Combine(export, ".agents/skills/dw-tools-math/SKILL.md")) &&
    File.Exists(Path.Combine(export, "examples/net11/Program.cs")) &&
    File.Exists(Path.Combine(export, "examples/net11/consumer.csproj")),
    "Missing selected portable skill or source-free example.");

var tracked = Directory.GetFiles(export, "*", SearchOption.AllDirectories)
    .Select(file => (Path: Path.GetRelativePath(export, file), Sha: Hash(file)))
    .OrderBy(x => x.Path, StringComparer.Ordinal).ToArray();
Need(tracked.Length == 12, "RS281 portable export has an unexpected file count.");
var project = File.ReadAllText(Path.Combine(export, "examples/net11/consumer.csproj"));
var example = File.ReadAllText(Path.Combine(export, "examples/net11/Program.cs"));
Need(project.Contains("<PackageReference Include=\"dw.tools.math.composition\"",
        StringComparison.Ordinal) &&
    project.Contains("[20260.20261107.1001]", StringComparison.Ordinal) &&
    !project.Contains("<ProjectReference", StringComparison.OrdinalIgnoreCase) &&
    example.Contains(PublishedMarker, StringComparison.Ordinal) &&
    !example.Contains("scope=source-project-consumer", StringComparison.Ordinal),
    "Exported sample is not pinned and package-only.");

var privateFeed = Path.Combine(export, "build", "isolated-feed");
Directory.CreateDirectory(privateFeed);
foreach (var package in packages)
{
    var fileName = package.Id + "." + package.Version + ".nupkg";
    var published = Path.Combine(feed!, fileName);
    Need(File.Exists(published) &&
        (File.GetAttributes(published) & FileAttributes.ReparsePoint) == 0 &&
        Hash(published) == package.Archive,
        "Physical private feed archive differs: " + package.Id);
    using (var archive = ZipFile.OpenRead(published))
    {
        var entry = archive.Entries.Single(e =>
            e.FullName == "lib/net10.0/" + package.Id + ".dll");
        using var stream = entry.Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(stream)) == package.Binary,
            "Physical published DLL differs: " + package.Id);
    }
    var copy = Path.Combine(privateFeed, fileName);
    File.Copy(published, copy);
    Need(Hash(copy) == package.Archive, "Isolated archive differs: " + package.Id);
}

string Wsl(params string[] arguments) =>
    RunProcess("wsl.exe", repoRoot, arguments);
var linuxRoot = Wsl("--exec", "wslpath", "-a", "-u", export);
Need(linuxRoot.StartsWith("/", StringComparison.Ordinal) &&
    !linuxRoot.Contains('\n'), "WSL export path mapping differs.");
string RunLinux(params string[] arguments)
{
    var command = new List<string>
    {
        "--exec", "bash", "-lc",
        "cd \"$1\" && shift && dotnet \"$@\"",
        "bash", linuxRoot
    };
    command.AddRange(arguments);
    return Wsl(command.ToArray());
}
Need(RunLinux("--version") == "11.0.100",
    "Default WSL distribution does not select exact SDK 11.0.100.");
const string ClientProject = "examples/net11/consumer.csproj";
RunLinux("restore", ClientProject, "--configfile", "NuGet.Config",
    "--source", linuxRoot + "/build/isolated-feed",
    "--packages", linuxRoot + "/build/private-linux-packages",
    "-p:NuGetAudit=false");
var lockFile = Path.Combine(export, "examples", "net11", "packages.lock.json");
Need(File.Exists(lockFile), "Linux net11 package lockfile is missing.");
using (var lockDocument = JsonDocument.Parse(File.ReadAllText(lockFile)))
{
    var groups = lockDocument.RootElement.GetProperty("dependencies");
    Need(groups.EnumerateObject().Count() == 1 &&
        groups.TryGetProperty("net11.0", out _),
        "Linux restore must target exactly net11.0.");
    var targets = groups.GetProperty("net11.0").EnumerateObject()
        .ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Need(targets.Count == packages.Length, "Linux private closure has extra dependencies.");
    foreach (var package in packages)
        Need(targets.TryGetValue(package.Id, out var value) &&
            value.GetProperty("resolved").GetString() == package.Version &&
            value.GetProperty("type").GetString() ==
                (package.Id == "dw.tools.math.composition" ? "Direct" : "Transitive"),
            "Linux pinned package identity or dependency type differs: " + package.Id);
}
var lockHash = Hash(lockFile);
RunLinux("restore", ClientProject, "--configfile", "NuGet.Config",
    "--source", linuxRoot + "/build/isolated-feed",
    "--packages", linuxRoot + "/build/private-linux-packages",
    "--locked-mode", "-p:NuGetAudit=false");
Need(Hash(lockFile) == lockHash, "Linux locked restore changed dependency bytes.");
RunLinux("build", ClientProject, "-c", "Release", "--no-restore");
var bin = Path.Combine(export, "build", "bin", "consumer", "Release", "net11.0");
foreach (var package in packages)
    Need(File.Exists(Path.Combine(bin, package.Id + ".dll")) &&
        Hash(Path.Combine(bin, package.Id + ".dll")) == package.Binary,
        "Linux consumer DLL differs from immutable private archive: " + package.Id);
var output = RunLinux("run", "--project", ClientProject,
    "-c", "Release", "--no-build", "--no-restore");
Need(output.Contains(PublishedMarker, StringComparison.Ordinal),
    "Linux package-only selected calls failed. Expected: " +
    PublishedMarker + "; actual output: " + output);
foreach (var trackedFile in tracked)
    Need(Hash(Path.Combine(export, trackedFile.Path)) == trackedFile.Sha,
        "Linux validation modified a portable export input: " + trackedFile.Path);
foreach (var package in packages)
{
    var fileName = package.Id + "." + package.Version + ".nupkg";
    Need(Hash(Path.Combine(feed!, fileName)) == package.Archive &&
        Hash(Path.Combine(privateFeed, fileName)) == package.Archive,
        "Linux validation modified a published or copied private archive.");
}
Console.WriteLine("MATH-RS282-PORTABLE-DISCOVERY-LINUX-NET11|export=12|selected=4|" +
    "examples=5|packages=3|sdk=11.0.100|lockedRestore=true|" +
    "binaryHashes=matched|archivesUnchanged=true|brokerReady=0|publication=false|PASS");
