#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--check" or "--prepublication-check"))
    throw new ArgumentException("Expected --check or --prepublication-check.");
var prepublication = args[0] == "--prepublication-check";
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("Windows verification only.");

const string Q = "dw.quantities";
const string S = "dw.tools.math.statistics";
const string Qv = "20260.20261127.1001";
const string Sv = "20260.20261127.1002";
const string OldQv = "20260.20261102.1001";
const string OldSv = "20260.20261120.1001";
const string Historical = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
const string NewChecks = "docs/engineering/fixtures/M11NumericPackageConsumer.cs";
var shared = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(shared) && Path.IsPathFullyQualified(shared) && Directory.Exists(shared), "Shared feed missing.");
var candidateFeed = Path.GetFullPath("build/artifacts/m11-candidates-1aa0ad9d4f7e/feed");
Need(Directory.Exists(candidateFeed), "Exact RS392 candidate feed missing; rebuilding is forbidden.");
string Archive(string dir, string id, string version) => Path.Combine(dir, id + "." + version + ".nupkg");
var packages = new[]
{
    (Id: Q, Version: Qv, Path: Archive(candidateFeed,Q,Qv), Sha: "1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d", Dll: "e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed", Dependency: ""),
    (Id: S, Version: Sv, Path: Archive(candidateFeed,S,Sv), Sha: "cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8", Dll: "134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3", Dependency: Qv),
    (Id: Q, Version: OldQv, Path: Archive(shared!,Q,OldQv), Sha: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d", Dll: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4", Dependency: ""),
    (Id: S, Version: OldSv, Path: Archive(shared!,S,OldSv), Sha: "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1", Dll: "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea", Dependency: OldQv)
};
void CheckUnpublished() => Need(!File.Exists(Archive(shared!,Q,Qv)) && !File.Exists(Archive(shared!,S,Sv)), "Unexpected publication.");
CheckUnpublished();
foreach (var item in packages)
{
    Need(File.Exists(item.Path) && Hash(item.Path) == item.Sha, "Physical archive differs: " + item.Id + " " + item.Version);
    using var zip = ZipFile.OpenRead(item.Path);
    using var manifest = zip.Entries.Single(e => e.FullName == item.Id + ".nuspec").Open();
    var xml = XDocument.Load(manifest).Root!;
    var ns = xml.Name.Namespace;
    var metadata = xml.Element(ns + "metadata")!;
    Need(metadata.Element(ns + "id")?.Value == item.Id && metadata.Element(ns + "version")?.Value == item.Version, "Package identity differs.");
    var deps = metadata.Descendants(ns + "dependency").ToArray();
    Need(item.Dependency.Length == 0 ? deps.Length == 0 : deps.Length == 1 &&
        deps[0].Attribute("id")?.Value == Q && deps[0].Attribute("version")?.Value == "[" + item.Dependency + "]", "Exact dependency differs.");
    using var dll = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + item.Id + ".dll").Open();
    Need(Convert.ToHexStringLower(SHA256.HashData(dll)) == item.Dll, "Embedded DLL differs.");
    Console.WriteLine("MATH-RS394-READBACK|id=" + item.Id + "|version=" + item.Version + "|archiveSha256=" + item.Sha + "|dllSha256=" + item.Dll + "|PASS");
}
if (prepublication)
{
    // Reuse the exact RS394 archive, embedded DLL and dependency inspection above.
    // This is a point-in-time, read-only intake check, not a release-policy engine.
    using var attestation = JsonDocument.Parse(File.ReadAllText(
        "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
    var authorization = attestation.RootElement.GetProperty("authorization");
    Need(authorization.GetProperty("package_from_dw_tools_math").GetBoolean() &&
        authorization.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
        "Owner packaging/redistribution attestation differs.");
    foreach (var item in packages.Take(2))
    {
        var flat = Archive(shared!, item.Id, item.Version);
        var versionDirectory = Path.Combine(shared!, item.Id, item.Version);
        Need(!File.Exists(flat) && !Directory.Exists(flat) &&
            !File.Exists(versionDirectory) && !Directory.Exists(versionDirectory),
            "Existing canonical flat or hierarchical target; stop without mutation: " + item.Id);
        Console.WriteLine("MATH-RS432-TARGET|id=" + item.Id + "|version=" + item.Version +
            "|canonicalFlatAbsent=true|canonicalHierarchicalAbsent=true|PASS");
    }
    Need(packages.All(item => Hash(item.Path) == item.Sha), "Original or rollback archive changed.");
    CheckUnpublished();
    Console.WriteLine("MATH-RS432-PREPUBLICATION|exactArchives=4|rightsAttested=true|canonicalTargetsAbsent=true|publication=false|rebuild=false|numericRequalification=false|fullFeedPolicyCheck=pending|ownerPublicationApproval=pending|PASS");
    return;
}
var root = Path.GetFullPath(Path.Combine("build","artifacts","m11-verify-" + Guid.NewGuid().ToString("N")[..12]));
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root,"global.json"), """{"sdk":{"version":"10.0.401","rollForward":"latestPatch","allowPrerelease":false}}""");
File.WriteAllText(Path.Combine(root,"Directory.Build.props"), """
<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>
</PropertyGroup></Project>
""");
File.WriteAllText(Path.Combine(root,"Directory.Build.targets"), "<Project />");
File.WriteAllText(Path.Combine(root,"Directory.Packages.props"), "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>");
var config = Path.Combine(root,"NuGet.Config");
File.WriteAllText(config,"<configuration><packageSources><clear /></packageSources></configuration>");
var historical = File.ReadAllText(Historical);
const string oldAdmission = "args.Length != 1 || args[0].Length != 64";
Need(historical.Split(oldAdmission,StringSplitOptions.None).Length == 2, "Historical fixture changed.");
foreach (bool rollback in new[] { false, true })
{
    var quantity = packages[rollback ? 2 : 0];
    var statistics = packages[rollback ? 3 : 1];
    string mode = rollback ? "rollback" : "candidate";
    var feed = Path.Combine(root,mode,"feed");
    Directory.CreateDirectory(feed);
    foreach (var item in new[] { quantity, statistics })
    {
        var copy = Archive(feed,item.Id,item.Version);
        File.Copy(item.Path,copy);
        Need(Hash(copy) == item.Sha, "Snapshot archive differs.");
    }
    foreach (string tfm in new[] { "net10.0","net11.0" })
    {
        var dir = Path.Combine(root,mode,tfm);
        Directory.CreateDirectory(dir);
        bool net11 = tfm == "net11.0";
        if (net11)
            File.WriteAllText(Path.Combine(dir,"global.json"), """{"sdk":{"version":"11.0.100","rollForward":"disable","allowPrerelease":false}}""");
        var project = new XDocument(new XElement("Project",new XAttribute("Sdk","Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",new XElement("OutputType","Exe"),new XElement("TargetFramework",tfm),
                new XElement("LangVersion",net11 ? "15.0" : "14.0"),new XElement("RestorePackagesWithLockFile","true"),new XElement("IsPackable","false")),
            new XElement("ItemGroup",new XElement("PackageReference",new XAttribute("Include",S),new XAttribute("Version","[" + statistics.Version + "]")))));
        project.Save(Path.Combine(dir,"consumer.csproj"));
        var program = historical.Replace(oldAdmission,"args.Length != 2 || args[0].Length != 64 || args[1].Length != 64",StringComparison.Ordinal)
            + "\n" + """
var quantitiesAssembly = typeof(dw.quantities.ExactRational).Assembly;
Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(quantitiesAssembly.Location))) == args[1], "Loaded Quantities hash differs.");
var r = new dw.quantities.ExactRational(3, 2);
Need(r + r == new dw.quantities.ExactRational(3, 1), "Historical rational golden differs.");
Console.WriteLine("MATH-RS394-QUANTITIES|loaded=" + args[1] + "|legacy=PASS");
""";
        if (rollback)
            program += "\n" + """
Need(quantitiesAssembly.GetType("dw.quantities.ExactRationalBudget")?.GetMethod("TryScaleToInt64") is null &&
    asm.GetType("Dw.Tools.Math.Statistics.StreamingMoments64") is null &&
    !typeof(QuantileStatistics64).GetMethods().Any(m => m.Name == "Evaluate" && m.GetParameters().Length == 4),
    "Rollback unexpectedly exposes new contracts.");
Console.WriteLine("MATH-RS394-ROLLBACK-SURFACE|PASS");
""";
        else
        {
            program += "\nM11NumericChecks.Run(args[1]);\n";
            File.Copy(NewChecks,Path.Combine(dir,"M11NumericPackageConsumer.cs"));
        }
        File.WriteAllText(Path.Combine(dir,"Program.cs"),program + "\n");
        var sdk = Run(dir,"--version").Trim();
        Need(net11 ? sdk == "11.0.100" : sdk.StartsWith("10.0.4",StringComparison.Ordinal),"SDK differs.");
        string[] restore = ["restore","consumer.csproj","--configfile",config,"--source",feed,"--packages",Path.Combine(dir,"cache"),"--use-lock-file"];
        Run(dir,restore);
        var lockPath = Path.Combine(dir,"packages.lock.json");
        var lockHash = Hash(lockPath);
        using (var doc = JsonDocument.Parse(File.ReadAllText(lockPath)))
        {
            var targets = doc.RootElement.GetProperty("dependencies");
            Need(targets.EnumerateObject().Count() == 1,"Unexpected lock targets.");
            var entries = targets.GetProperty(tfm).EnumerateObject().ToDictionary(x => x.Name,x => x.Value,StringComparer.OrdinalIgnoreCase);
            Need(entries.Count == 2 && entries[S].GetProperty("resolved").GetString() == statistics.Version &&
                entries[Q].GetProperty("resolved").GetString() == quantity.Version &&
                entries[S].GetProperty("type").GetString() == "Direct" && entries[Q].GetProperty("type").GetString() == "Transitive","Lock graph differs.");
        }
        Run(dir,[..restore,"--locked-mode"]);
        Run(dir,"build","consumer.csproj","-c","Release","--no-restore");
        var output = Run(dir,"run","--project","consumer.csproj","-c","Release","--no-build","--no-restore","--",statistics.Dll,quantity.Dll);
        Need(output.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12",StringComparison.Ordinal) &&
            output.Contains("MATH-RS394-QUANTITIES|loaded=" + quantity.Dll + "|legacy=PASS",StringComparison.Ordinal) &&
            output.Contains(rollback ? "MATH-RS394-ROLLBACK-SURFACE|PASS" : "MATH-M11-BINARY|quantitiesDll=" + quantity.Dll + "|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS",StringComparison.Ordinal),
            "Consumer checks incomplete.");
        Need(Hash(lockPath) == lockHash,"Locked graph changed.");
        Console.WriteLine("MATH-RS394-CONSUMER|mode=" + mode + "|tfm=" + tfm + "|sdk=" + sdk + "|lockSha256=" + lockHash +
            "|statisticsDll=" + statistics.Dll + "|quantitiesDll=" + quantity.Dll + "|PASS");
    }
    foreach (var item in new[] { quantity, statistics })
        Need(Hash(Archive(feed,item.Id,item.Version)) == item.Sha,"Snapshot archive changed.");
}
Need(packages.All(item => Hash(item.Path) == item.Sha),"Original archive changed.");
CheckUnpublished();
Console.WriteLine("MATH-RS394-VERIFY|archives=4|consumers=4|candidate=PASS|rollbackRecipe=PASS|rebuild=false|publication=false|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS394: " + message);
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
