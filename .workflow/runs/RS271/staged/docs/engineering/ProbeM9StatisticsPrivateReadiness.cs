#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("The private NUGET_LOCAL feed is inspected from Windows.");

const string PackageId = "dw.tools.math.statistics";
const string Version = "20260.20261120.1001";
const string BaseId = "dw.quantities";
const string BaseVersion = "20260.20261102.1001";
const string BaseArchiveSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string BaseDllSha = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string OldPublishedSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";

void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS271: " + reason);
}
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
JsonElement Report(string run)
{
    using var doc = JsonDocument.Parse(File.ReadAllText(
        ".aura/workflow/reports/" + run + "/attempt-001.json"));
    var root = doc.RootElement;
    Require(root.GetProperty("outcome").GetString() == "succeeded" &&
        root.GetProperty("attempt").GetInt32() == 1,
        "Missing durable qualifying run " + run);
    return root.Clone();
}
var source = Report("RS268");
var candidateQualification = Report("RS269");
var consumer = Report("RS270");
Require(source.GetProperty("git").GetProperty("commitMessage").GetString() ==
    "feat: implement bounded unbiased SplitMix64 integer draws",
    "Source qualification lineage changed.");
Require(candidateQualification.GetProperty("git").GetProperty("commitMessage").GetString() ==
    "test: qualify unpublished bounded Statistics NuGet candidate",
    "Candidate packaging lineage changed.");
Require(consumer.GetProperty("git").GetProperty("commitMessage").GetString() ==
    "test: qualify bounded Statistics candidate net11 binary consumer",
    "net11 binary consumer lineage changed.");
Require(consumer.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1,
    "RS270 preselection failure history must not be erased.");
var rights = File.ReadAllText("docs/planning/decisions/m0-w02-c01-source-rights.md");
Require(rights.Contains("owner-authorized proprietary code", StringComparison.Ordinal) &&
    rights.Contains("package, and redistribute", StringComparison.Ordinal),
    "Required source-rights decision is missing or contradictory.");
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "Private feed is unavailable or not absolute.");
var oldPackage = Path.Combine(feed!, PackageId + ".20260.20261106.1001.nupkg");
var basePackage = Path.Combine(feed!, BaseId + "." + BaseVersion + ".nupkg");
var target = Path.Combine(feed!, PackageId + "." + Version + ".nupkg");
Require(File.Exists(oldPackage) && Hash(oldPackage) == OldPublishedSha,
    "Original published Statistics version differs.");
Require(File.Exists(basePackage) && Hash(basePackage) == BaseArchiveSha,
    "Base Quantities package differs.");
Require(!File.Exists(target), "The new version already exists in NUGET_LOCAL; refuse collision.");
using (var zip = ZipFile.OpenRead(basePackage))
{
    var dll = zip.Entries.Single(x => x.FullName == "lib/net10.0/dw.quantities.dll");
    using var raw = dll.Open();
    Require(Convert.ToHexStringLower(SHA256.HashData(raw)) == BaseDllSha,
        "Base Quantities packaged DLL differs.");
}
// Disposable DWF validation workspaces do not retain RS269 output files.
// Reproduce the qualified candidate recipe without writing into NUGET_LOCAL.
var start = new ProcessStartInfo("dotnet")
{
    UseShellExecute = false,
    WorkingDirectory = Directory.GetCurrentDirectory(),
    RedirectStandardOutput = true,
    RedirectStandardError = true
};
foreach (var argument in new[]
{
    "run", "--file", "docs/engineering/ProbeM9StatisticsBoundedNugetCandidate.cs",
    "--", "--check"
})
    start.ArgumentList.Add(argument);
using (var process = Process.Start(start) ??
    throw new InvalidOperationException("Cannot start isolated Statistics packaging recipe."))
{
    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    Require(process.WaitForExit(120000) && process.ExitCode == 0 &&
        output.Contains("MATH-RS269-CANDIDATE|", StringComparison.Ordinal) &&
        output.Contains("published=false", StringComparison.Ordinal),
        "Previously qualified candidate recipe refused: " + output + "\n" + error);
}
var candidate = Path.GetFullPath("build/artifacts/m9-rs269-bounded-statistics-candidate/candidate/" +
    PackageId + "." + Version + ".nupkg");
Require(File.Exists(candidate),
    "No candidate archive in this disposable workspace. Run the qualified RS269 recipe first.");
var candidateSha = Hash(candidate);
string dllSha;
using (var zip = ZipFile.OpenRead(candidate))
{
    var dll = zip.Entries.Single(x => x.FullName == "lib/net10.0/" + PackageId + ".dll");
    using (var bytes = dll.Open())
        dllSha = Convert.ToHexStringLower(SHA256.HashData(bytes));
    var nuspec = zip.Entries.Single(x => x.FullName == PackageId + ".nuspec");
    using var nuspecStream = nuspec.Open();
    var document = XDocument.Load(nuspecStream);
    var ns = document.Root!.Name.Namespace;
    var meta = document.Root.Element(ns + "metadata")!;
    Require(meta.Element(ns + "id")?.Value == PackageId &&
        meta.Element(ns + "version")?.Value == Version,
        "Candidate NuGet package ID/version do not match.");
    var dependencies = meta.Element(ns + "dependencies")!.Descendants(ns + "dependency").ToArray();
    Require(dependencies.Length == 1 &&
        dependencies[0].Attribute("id")?.Value == BaseId &&
        dependencies[0].Attribute("version")?.Value == "[" + BaseVersion + "]",
        "Candidate exact dependency differs.");
}
Require(Hash(oldPackage) == OldPublishedSha && Hash(basePackage) == BaseArchiveSha &&
    !File.Exists(target), "Read-only inspection changed publication feed.");
Console.WriteLine("MATH-RS271-PRIVATE-READINESS|package=" + PackageId +
    "|version=" + Version + "|candidateSha256=" + candidateSha +
    "|candidateDllSha256=" + dllSha +
    "|dependency=" + BaseId + ":" + BaseVersion +
    "|net11Consumer=RS270|authorizedPublication=false|" +
    "published=false|previousArchivesUnchanged=true|PASS");
