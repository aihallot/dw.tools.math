#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("Windows net10/net11 qualification only.");

const string Q = "dw.quantities";
const string S = "dw.tools.math.statistics";
const string Qv = "20260.20261127.1001";
const string Sv = "20260.20261127.1002";
const string OldQv = "20260.20261102.1001";
const string OldSv = "20260.20261120.1001";
const string OldQHash = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string OldSHash = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string Historical = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
const string NewChecks = "docs/engineering/fixtures/M11NumericPackageConsumer.cs";
var shared = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(shared) && Path.IsPathFullyQualified(shared) && Directory.Exists(shared),
    "Original private feed missing.");
string Archive(string dir, string id, string version) => Path.Combine(dir, id + "." + version + ".nupkg");
var oldQ = Archive(shared!, Q, OldQv);
var oldS = Archive(shared!, S, OldSv);
void CheckShared()
{
    Need(Hash(oldQ) == OldQHash && Hash(oldS) == OldSHash, "Historical archive changed.");
    Need(!File.Exists(Archive(shared!, Q, Qv)) && !File.Exists(Archive(shared!, S, Sv)),
        "Candidate version already exists in shared feed; refuse.");
}
CheckShared();
var root = Path.GetFullPath(Path.Combine("build", "artifacts", "m11-candidates-" + Guid.NewGuid().ToString("N")[..12]));
var feed = Path.Combine(root, "feed");
Directory.CreateDirectory(feed);
File.WriteAllText(Path.Combine(root, "global.json"), """{"sdk":{"version":"10.0.401","rollForward":"latestPatch","allowPrerelease":false}}""");
File.WriteAllText(Path.Combine(root, "Directory.Build.props"), """
<Project><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors><Deterministic>true</Deterministic>
<BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>
</PropertyGroup></Project>
""");
File.WriteAllText(Path.Combine(root, "Directory.Build.targets"), "<Project />");
File.WriteAllText(Path.Combine(root, "Directory.Packages.props"),
    "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>");
var config = Path.Combine(root, "NuGet.Config");
File.WriteAllText(config, "<configuration><packageSources><clear /></packageSources></configuration>");
var sourceHashes = new Dictionary<string, string>(StringComparer.Ordinal);
foreach (string id in new[] { Q, S })
{
    var source = Path.Combine("src", "projects", id);
    var dest = Path.Combine(root, id);
    Directory.CreateDirectory(dest);
    foreach (var file in Directory.GetFiles(source, "*.cs").OrderBy(x => x, StringComparer.Ordinal))
    {
        sourceHashes.Add(file, Hash(file));
        File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
    }
    var sourceProject = Path.Combine(source, id + ".csproj");
    sourceHashes.Add(sourceProject, Hash(sourceProject));
    var project = XDocument.Load(sourceProject);
    project.Descendants("Version").Single().Value = id == Q ? Qv : Sv;
    // Calendar NuGet versions are not valid CLR assembly/file versions.
    // Preserve the established candidate recipe's separate binary identity.
    var properties = project.Descendants("Version").Single().Parent!;
    foreach (var name in new[] { "AssemblyVersion", "FileVersion" })
    {
        var existing = project.Descendants(name).SingleOrDefault();
        if (existing is null)
            properties.Add(new XElement(name, "1.0.0.0"));
        else
            existing.Value = "1.0.0.0";
    }
    if (id == S)
    {
        var reference = project.Descendants("ProjectReference").Single();
        Need(reference.Attribute("Include")?.Value.Replace('\\', '/') == "../dw.quantities/dw.quantities.csproj",
            "Canonical Statistics project dependency differs.");
        reference.ReplaceWith(new XElement("PackageReference", new XAttribute("Include", Q),
            new XAttribute("Version", "[" + Qv + "]")));
    }
    Need(!project.Descendants("ProjectReference").Any(), "Candidate must be package-only.");
    var projectFile = id + ".csproj";
    project.Save(Path.Combine(dest, projectFile));
    var sdk = Run(dest, "--version").Trim();
    Need(sdk.StartsWith("10.0.4", StringComparison.Ordinal), "Unexpected candidate SDK.");
    RestoreLocked(dest, projectFile);
    Run(dest, "pack", projectFile, "-c", "Release", "--no-restore", "--output", feed);
    Need(File.Exists(Archive(feed, id, id == Q ? Qv : Sv)), "Candidate archive missing.");
}
var quantity = Inspect(Q, Qv, false);
var statistics = Inspect(S, Sv, true);
foreach (string tfm in new[] { "net10.0", "net11.0" })
{
    var dir = Path.Combine(root, "consumer-" + tfm);
    Directory.CreateDirectory(dir);
    bool net11 = tfm == "net11.0";
    if (net11)
        File.WriteAllText(Path.Combine(dir, "global.json"), """{"sdk":{"version":"11.0.100","rollForward":"disable","allowPrerelease":false}}""");
    var project = new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
        new XElement("PropertyGroup", new XElement("OutputType", "Exe"),
            new XElement("TargetFramework", tfm), new XElement("LangVersion", net11 ? "15.0" : "14.0"),
            new XElement("RestorePackagesWithLockFile", "true"), new XElement("IsPackable", "false")),
        new XElement("ItemGroup", new XElement("PackageReference", new XAttribute("Include", S),
            new XAttribute("Version", "[" + Sv + "]")))));
    project.Save(Path.Combine(dir, "consumer.csproj"));
    // Preserve the complete historical M9 fixture; extend only its argument
    // admission and append a call into the independent M11 check class.
    var historic = File.ReadAllText(Historical);
    const string oldAdmission = "args.Length != 1 || args[0].Length != 64";
    Need(historic.Split(oldAdmission, StringSplitOptions.None).Length == 2, "Historical fixture admission changed.");
    File.WriteAllText(Path.Combine(dir, "Program.cs"), historic.Replace(oldAdmission,
        "args.Length != 2 || args[0].Length != 64 || args[1].Length != 64", StringComparison.Ordinal)
        + "\nM11NumericChecks.Run(args[1]);\n");
    File.Copy(NewChecks, Path.Combine(dir, "M11NumericPackageConsumer.cs"));
    var sdk = Run(dir, "--version").Trim();
    Need(net11 ? sdk == "11.0.100" : sdk.StartsWith("10.0.4", StringComparison.Ordinal), "Consumer SDK differs.");
    var lockHash = RestoreLocked(dir, "consumer.csproj");
    using (var lockDoc = JsonDocument.Parse(File.ReadAllText(Path.Combine(dir, "packages.lock.json"))))
    {
        var frame = lockDoc.RootElement.GetProperty("dependencies");
        Need(frame.EnumerateObject().Count() == 1, "Unexpected lock targets.");
        var entries = frame.GetProperty(tfm).EnumerateObject().ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
        Need(entries.Count == 2 && entries[S].GetProperty("resolved").GetString() == Sv &&
            entries[Q].GetProperty("resolved").GetString() == Qv &&
            entries[S].GetProperty("type").GetString() == "Direct" && entries[Q].GetProperty("type").GetString() == "Transitive",
            "Consumer lock graph differs.");
    }
    Run(dir, "build", "consumer.csproj", "-c", "Release", "--no-restore");
    var result = Run(dir, "run", "--project", "consumer.csproj", "-c", "Release", "--no-build", "--no-restore",
        "--", statistics.Dll, quantity.Dll);
    Need(result.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal) &&
        result.Contains("MATH-M11-BINARY|quantitiesDll=" + quantity.Dll + "|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS", StringComparison.Ordinal),
        "Binary consumer markers missing.");
    Need(Hash(Path.Combine(dir, "packages.lock.json")) == lockHash, "Consumer lock changed.");
    Console.WriteLine("MATH-RS392-CONSUMER|tfm=" + tfm + "|sdk=" + sdk + "|lockSha256=" + lockHash +
        "|statisticsDll=" + statistics.Dll + "|quantitiesDll=" + quantity.Dll + "|PASS");
}
Need(sourceHashes.All(x => Hash(x.Key) == x.Value), "Canonical source changed.");
Need(Hash(Archive(feed, Q, Qv)) == quantity.Archive && Hash(Archive(feed, S, Sv)) == statistics.Archive, "Candidate archive changed.");
CheckShared();
Console.WriteLine("MATH-RS392-CANDIDATES|root=" + root + "|oldArchivesUnchanged=true|publication=false|windowsNet10=PASS|windowsNet11=PASS|PASS");

(string Archive, string Dll) Inspect(string id, string version, bool hasDependency)
{
    var path = Archive(feed, id, version);
    var archiveHash = Hash(path);
    using var zip = ZipFile.OpenRead(path);
    using var descriptor = zip.Entries.Single(e => e.FullName == id + ".nuspec").Open();
    var xml = XDocument.Load(descriptor).Root!;
    var ns = xml.Name.Namespace;
    var meta = xml.Element(ns + "metadata")!;
    Need(meta.Element(ns + "id")?.Value == id && meta.Element(ns + "version")?.Value == version, "Candidate identity differs.");
    var deps = meta.Descendants(ns + "dependency").ToArray();
    Need(hasDependency ? deps.Length == 1 && deps[0].Attribute("id")?.Value == Q &&
        deps[0].Attribute("version")?.Value == "[" + Qv + "]" : deps.Length == 0, "Candidate dependency differs.");
    using var dll = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + id + ".dll").Open();
    var dllHash = Convert.ToHexStringLower(SHA256.HashData(dll));
    Console.WriteLine("MATH-RS392-ARCHIVE|id=" + id + "|version=" + version +
        "|archiveSha256=" + archiveHash + "|dllSha256=" + dllHash + "|tfm=net10.0|PASS");
    return (archiveHash, dllHash);
}
string RestoreLocked(string dir, string project)
{
    var cache = Path.Combine(dir, "private-package-cache");
    string[] common = ["restore", project, "--configfile", config, "--source", feed, "--packages", cache, "--use-lock-file"];
    Run(dir, common);
    var lockPath = Path.Combine(dir, "packages.lock.json");
    var before = Hash(lockPath);
    Run(dir, [.. common, "--locked-mode"]);
    Need(Hash(lockPath) == before, "Locked restore changed lock.");
    return before;
}
static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS392: " + message);
}
static string Run(string dir, params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = dir, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet command timed out.");
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    if (process.ExitCode != 0) throw new InvalidDataException("dotnet failed: " + string.Join(" ", arguments) + "\n" + output + "\n" + errors);
    if (arguments[0] == "run") Console.WriteLine(output);
    if (!string.IsNullOrWhiteSpace(errors)) Console.WriteLine(errors);
    return output;
}
