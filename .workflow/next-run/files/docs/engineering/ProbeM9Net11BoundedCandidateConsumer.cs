#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS270 uses an isolated Windows net11 consumer.");
const string Version = "20260.20261120.1001";
const string DependencyVersion = "20260.20261102.1001";
const string DependencySha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string DependencyDllSha = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string CandidateId = "dw.tools.math.statistics";
const string OriginalFixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS270: " + message);
}
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Need(File.Exists(OriginalFixture), "Missing immutable source-free bounded consumer fixture.");
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must be an existing private feed.");
var originalDependency = Path.Combine(feed!, "dw.quantities." + DependencyVersion + ".nupkg");
Need(File.Exists(originalDependency) && Hash(originalDependency) == DependencySha,
    "Original published Quantities archive changed.");
var previousPublished = Path.Combine(feed!, CandidateId + ".20260.20261106.1001.nupkg");
Need(File.Exists(previousPublished), "Original published Statistics archive missing.");
var previousHash = Hash(previousPublished);
Need(!File.Exists(Path.Combine(feed!, CandidateId + "." + Version + ".nupkg")),
    "Candidate already present in immutable feed: refuse overwrite or publication.");
string Run(string wd, params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = wd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var a in arguments) info.ArgumentList.Add(a);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("Cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(120000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS270 dotnet " + string.Join(" ", arguments) +
            " failed:\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}
// RS269 only stages the repeatable packaging recipe, not a durable .nupkg.
// Re-create exactly that candidate under a disposable directory in this run.
var repository = Directory.GetCurrentDirectory();
var recipeOutput = Run(repository, "run", "--file",
    "docs/engineering/ProbeM9StatisticsBoundedNugetCandidate.cs", "--", "--check");
Need(recipeOutput.Contains("MATH-RS269-CANDIDATE|", StringComparison.Ordinal) &&
    recipeOutput.Contains("published=false", StringComparison.Ordinal),
    "Previous qualified source-to-candidate recipe did not pass.");
var candidate = Path.GetFullPath("build/artifacts/m9-rs269-bounded-statistics-candidate/candidate/" +
    CandidateId + "." + Version + ".nupkg");
Need(File.Exists(candidate), "RS269 candidate archive not reproduced.");
var candidateHash = Hash(candidate);
var candidateDllSha = "";
using (var zip = ZipFile.OpenRead(candidate))
{
    var dll = zip.Entries.Single(x => x.FullName == "lib/net10.0/" + CandidateId + ".dll");
    using var bin = dll.Open();
    candidateDllSha = Convert.ToHexStringLower(SHA256.HashData(bin));
    var nuspec = zip.Entries.Single(x => x.FullName == CandidateId + ".nuspec");
    using var xmlStream = nuspec.Open();
    var desc = XDocument.Load(xmlStream);
    var n = desc.Root!.Name.Namespace;
    var meta = desc.Root.Element(n + "metadata")!;
    Need(meta.Element(n + "id")?.Value == CandidateId &&
         meta.Element(n + "version")?.Value == Version, "Candidate package identity mismatch.");
    var deps = meta.Element(n + "dependencies")!.Descendants(n + "dependency").ToArray();
    Need(deps.Length == 1 && deps[0].Attribute("id")?.Value == "dw.quantities" &&
        deps[0].Attribute("version")?.Value == "[" + DependencyVersion + "]",
        "Unexpected candidate dependency graph.");
}
var root = Path.GetFullPath("build/artifacts/m9-rs270-net11-bounded-consumer");
var localFeed = Path.Combine(root, "feed");
Directory.CreateDirectory(localFeed);
var dependencyCopy = Path.Combine(localFeed, Path.GetFileName(originalDependency));
var candidateCopy = Path.Combine(localFeed, Path.GetFileName(candidate));
File.Copy(originalDependency, dependencyCopy, overwrite:true);
File.Copy(candidate, candidateCopy, overwrite:true);
Need(Hash(dependencyCopy) == DependencySha && Hash(candidateCopy) == candidateHash,
    "Temporary feed archives do not match original bytes.");
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
File.WriteAllText(Path.Combine(root, "consumer.csproj"),
    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
    "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
    "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
    "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
    "</PropertyGroup><ItemGroup><PackageReference Include=\"" + CandidateId +
    "\" Version=\"[" + Version + "]\" /></ItemGroup></Project>\n");
File.Copy(OriginalFixture, Path.Combine(root, "Program.cs"), overwrite:true);
Need(Run(root, "--version") == "11.0.100", "Net11 SDK identity mismatch.");
Run(root, "restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", localFeed, "--packages", Path.Combine(root,"private-package-cache"),
    "-p:NuGetAudit=false");
var lockPath = Path.Combine(root, "packages.lock.json");
Need(File.Exists(lockPath), "Net11 lock file missing.");
using (var d = JsonDocument.Parse(File.ReadAllText(lockPath)))
{
    var dependencies = d.RootElement.GetProperty("dependencies");
    Need(dependencies.EnumerateObject().Count() == 1 &&
         dependencies.TryGetProperty("net11.0", out _), "Lock must have net11.0 only.");
    var frame = dependencies.GetProperty("net11.0");
    var entries = frame.EnumerateObject().ToDictionary(x => x.Name,
        x => x.Value, StringComparer.OrdinalIgnoreCase);
    Need(entries.Count == 2 && entries[CandidateId].GetProperty("resolved").GetString() == Version &&
        entries["dw.quantities"].GetProperty("resolved").GetString() == DependencyVersion,
        "Net11 locked package versions differ.");
    Need(entries[CandidateId].GetProperty("type").GetString() == "Direct" &&
        entries["dw.quantities"].GetProperty("type").GetString() == "Transitive",
        "Unexpected lock topology.");
}
var lockHash = Hash(lockPath);
Run(root, "restore", "consumer.csproj", "--configfile", "NuGet.Config",
    "--source", localFeed, "--packages", Path.Combine(root, "private-package-cache"),
    "--locked-mode", "-p:NuGetAudit=false");
Need(Hash(lockPath) == lockHash, "Locked restore modified lockfile.");
Run(root, "build", "consumer.csproj", "-c", "Release", "--no-restore");
var result = Run(root, "run", "--project", "consumer.csproj",
    "-c", "Release", "--no-build", "--no-restore", "--", candidateDllSha);
Need(result.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal) &&
    result.Contains("MATH-RS270-CHECK|forced-rejection-budget|PASS", StringComparison.Ordinal) &&
    result.Contains("MATH-RS270-CHECK|checkpoint-replay|PASS", StringComparison.Ordinal),
    "Net11 independent binary consumer did not pass original bounded checks.");
Need(Hash(originalDependency) == DependencySha && Hash(previousPublished) == previousHash &&
    Hash(dependencyCopy) == DependencySha && Hash(candidateCopy) == candidateHash,
    "Immutable packages or feed copies changed.");
Need(!File.Exists(Path.Combine(feed!, CandidateId + "." + Version + ".nupkg")),
    "Candidate was unexpectedly published in original feed.");
Console.WriteLine("MATH-RS270-NET11-CANDIDATE|id=" + CandidateId + "|version=" + Version +
    "|candidateSha256=" + candidateHash + "|dllSha256=" + candidateDllSha +
    "|quantitiesSha256=" + DependencySha +
    "|checks=12|lockedRestore=pass|published=false|PASS");
