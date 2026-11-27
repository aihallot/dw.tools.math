#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args is not ["--check"] and not ["--linux"]) throw new ArgumentException("Expected --check or --linux.");
var linux = args[0] == "--linux";
Need(linux ? OperatingSystem.IsLinux() : OperatingSystem.IsWindows(), "Unexpected platform.");
var expectedRid = linux ? "linux-x64" : "win-x64";
var repo = Path.GetFullPath(".");
Need(Run(repo, "--version").Trim() == "11.0.100", "Actual SDK differs.");
using var identity = JsonDocument.Parse(File.ReadAllText("docs/engineering/m10-post-cutover-candidate-identities-rs420.json"));
var data = identity.RootElement;
var version = data.GetProperty("version").GetString()!;
Need(version == "20260.20261127.1001-rs420" && !data.GetProperty("publication").GetBoolean(), "Candidate identity differs.");
var candidates = Path.Combine(repo, data.GetProperty("relativeRoot").GetString()!, "feed");
var packages = data.GetProperty("packages").EnumerateArray().ToDictionary(x => x.GetProperty("id").GetString()!, StringComparer.Ordinal);
string Sha(string id, string key) => packages[id].GetProperty(key).GetString()!;
string[] Deps(string id) => packages[id].GetProperty("dependencies").EnumerateArray().Select(x => x.GetString()!).ToArray();
string Archive(string dir, string id) => Path.Combine(dir, id + "." + version + ".nupkg");
Need(packages.Count == 13 && packages.Keys.Sum(x => Deps(x).Length) == 16 &&
    Directory.GetFiles(candidates, "*.nupkg").Length == 13, "Candidate roster differs; no rebuild allowed.");
var root = Path.Combine(repo, "build", "artifacts", "m10-rs421-consumers-" + Guid.NewGuid().ToString("N")[..12]);
var feed = Path.Combine(root, "feed");
Directory.CreateDirectory(feed);
foreach (var id in packages.Keys.OrderBy(x => x, StringComparer.Ordinal))
{
    var original = Archive(candidates, id);
    Need(Hash(original) == Sha(id, "archiveSha256"), "Original archive differs: " + id);
    using (var zip = ZipFile.OpenRead(original))
    {
        using var descriptor = zip.Entries.Single(x => x.FullName == id + ".nuspec").Open();
        var xml = XDocument.Load(descriptor).Root!;
        var ns = xml.Name.Namespace;
        var meta = xml.Element(ns + "metadata")!;
        Need(meta.Element(ns + "id")?.Value == id && meta.Element(ns + "version")?.Value == version, "Nuspec differs.");
        var deps = meta.Descendants(ns + "dependency").ToArray();
        Need(Deps(id).SequenceEqual(deps.Select(x => (string)x.Attribute("id")!).OrderBy(x => x, StringComparer.Ordinal)) &&
            deps.All(x => (string?)x.Attribute("version") == "[" + version + "]"), "Exact dependency differs.");
        using var binary = zip.Entries.Single(x => x.FullName == "lib/net11.0/" + id + ".dll").Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(binary)) == Sha(id, "dllSha256"), "Archive DLL differs.");
    }
    var copy = Archive(feed, id);
    File.Copy(original, copy);
    Need(Hash(copy) == Sha(id, "archiveSha256"), "Snapshot differs.");
}
File.Copy("global.json", Path.Combine(root, "global.json"));
File.WriteAllText(Path.Combine(root, "Directory.Build.props"), """
<Project><PropertyGroup>
<TargetFramework>net11.0</TargetFramework><LangVersion>15.0</LangVersion>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors><EnableNETAnalyzers>true</EnableNETAnalyzers>
<AnalysisLevel>latest</AnalysisLevel><Deterministic>true</Deterministic>
<BaseOutputPath>$(MSBuildThisFileDirectory)out/bin/$(MSBuildProjectName)/</BaseOutputPath>
<BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)out/obj/$(MSBuildProjectName)/</BaseIntermediateOutputPath>
</PropertyGroup></Project>
""");
File.WriteAllText(Path.Combine(root, "Directory.Build.targets"), "<Project />");
File.WriteAllText(Path.Combine(root, "Directory.Packages.props"), "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>");
var config = Path.Combine(root, "NuGet.config");
File.WriteAllText(config, "<configuration><packageSources><clear /></packageSources></configuration>");
var coverage = new HashSet<string>(StringComparer.Ordinal);
foreach (var mode in new[] { "numeric", "broad" })
{
    var numeric = mode == "numeric";
    string[] direct = numeric ? ["dw.tools.math.statistics"] :
        ["dw.quantities.standard", "dw.tools.math.distribution", "dw.tools.math.foundation", "dw.tools.math.analysis", "dw.tools.math.scientific"];
    var closure = new HashSet<string>(StringComparer.Ordinal);
    void Add(string id) { if (closure.Add(id)) foreach (var dep in Deps(id)) Add(dep); }
    foreach (var id in direct) Add(id);
    Need(closure.Count == (numeric ? 2 : 12), "Unexpected consumer closure.");
    coverage.UnionWith(closure);
    var dir = Path.Combine(root, mode);
    Directory.CreateDirectory(dir);
    var name = "consumer-" + mode;
    var project = Path.Combine(dir, name + ".csproj");
    new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
        new XElement("PropertyGroup", new XElement("OutputType", "Exe"), new XElement("IsPackable", "false"),
            new XElement("RestorePackagesWithLockFile", "true")),
        new XElement("ItemGroup", direct.Select(id => new XElement("PackageReference", new XAttribute("Include", id),
            new XAttribute("Version", "[" + version + "]")))))).Save(project);
    Need(!XDocument.Load(project).Descendants("ProjectReference").Any(), "Source-free boundary differs.");
    string program;
    if (numeric)
    {
        program = File.ReadAllText("docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs");
        const string admission = "args.Length != 1 || args[0].Length != 64";
        Need(program.Split(admission, StringSplitOptions.None).Length == 2, "Historical fixture admission differs.");
        program = program.Replace(admission, "args.Length != 2 || args[0].Length != 64 || args[1].Length != 64", StringComparison.Ordinal) +
            "\nM11NumericChecks.Run(args[1]);\n";
        File.Copy("docs/engineering/fixtures/M11NumericPackageConsumer.cs", Path.Combine(dir, "M11NumericPackageConsumer.cs"));
    }
    else
    {
        program = File.ReadAllText("tests/consumers/exact-math-smoke/Program.cs") + """

var vertical = Dw.Tools.Math.Distribution.ExactMathVerticalRunner.Run(
    "where(sub(square(x),1),ne(x,1))", new ExactRational(2,1));
Require(vertical.IsAdmitted && vertical.CanonicalDerivative == "mul(2,x)" &&
    vertical.ExactValueAtPoint == new ExactRational(3,1) &&
    vertical.OriginalRootAudit!.VerifiedRoots.Single().Value == new ExactRational(-1,1) &&
    vertical.OriginalRootAudit.ExcludedFromDomain.Single() == new ExactRational(1,1), "Distribution golden differs.");
var curve = Dw.Tools.Math.Analysis.BoundedCurveAnalysis64.InterpolateLinear(
    [new(0,1),new(1,3),new(2,5)],0.5);
Require(curve.Succeeded && curve.Value == 2d, "Analysis golden differs.");
var exactScience = Dw.Tools.Math.Scientific.BoundedExactScientific.GreatestCommonDivisor(48,18);
Require(exactScience.Succeeded && exactScience.Value == 6, "Scientific golden differs.");
Require(Dw.Tools.Math.Foundation.FoundationContract.Identity == "dw.tools.math.foundation/0.1", "Foundation identity differs.");
Console.WriteLine("MATH-M10-BROAD-SEMANTICS|PASS");
""";
    }
    foreach (var id in closure.OrderBy(x => x, StringComparer.Ordinal))
    {
        var check = numeric ? "Need" : "Require";
        program += "\n{\nvar loaded = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(\"" + id + "\"));\n" +
            check + "(Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(loaded.Location))) == \"" +
            Sha(id, "dllSha256") + "\", \"Loaded assembly differs: " + id + "\");\n" +
            "Console.WriteLine(\"MATH-M10-LOADED|id=" + id + "|sha256=" + Sha(id, "dllSha256") + "|PASS\");\n}\n";
    }
    program += "\nConsole.WriteLine(\"MATH-M10-RID|\" + System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier);\n";
    File.WriteAllText(Path.Combine(dir, "Program.cs"), program);
    Need(Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories).Length == (numeric ? 2 : 1), "Unexpected consumer source files.");
    var cache = Path.Combine(root, "cache", mode);
    string[] restore = ["restore", project, "--configfile", config, "--source", feed, "--packages", cache, "--use-lock-file"];
    Run(dir, restore);
    var lockPath = Path.Combine(dir, "packages.lock.json");
    var lockHash = Hash(lockPath);
    using (var doc = JsonDocument.Parse(File.ReadAllText(lockPath)))
    {
        var targets = doc.RootElement.GetProperty("dependencies");
        Need(targets.EnumerateObject().Count() == 1, "Unexpected lock targets.");
        var entries = targets.GetProperty("net11.0").EnumerateObject().ToArray();
        Need(closure.SetEquals(entries.Select(x => x.Name)), "Locked closure differs.");
        foreach (var entry in entries)
        {
            Need(entry.Value.GetProperty("resolved").GetString() == version &&
                entry.Value.GetProperty("type").GetString() == (direct.Contains(entry.Name, StringComparer.Ordinal) ? "Direct" : "Transitive"),
                "Locked identity differs: " + entry.Name);
            Need(Hash(Path.Combine(cache, entry.Name, version, "lib", "net11.0", entry.Name + ".dll")) == Sha(entry.Name, "dllSha256"),
                "Restored assembly differs: " + entry.Name);
        }
    }
    Run(dir, [.. restore, "--locked-mode"]);
    Need(Hash(lockPath) == lockHash, "Locked restore changed lock.");
    Run(dir, "build", project, "-c", "Release", "--no-restore", "-warnaserror");
    string[] run = ["run", "--project", project, "-c", "Release", "--no-build", "--no-restore"];
    var output = Run(dir, numeric ? [.. run, "--", Sha("dw.tools.math.statistics", "dllSha256"), Sha("dw.quantities", "dllSha256")] : run);
    Need(output.Contains("MATH-M10-RID|" + expectedRid, StringComparison.Ordinal) &&
        closure.All(id => output.Contains("MATH-M10-LOADED|id=" + id + "|sha256=" + Sha(id, "dllSha256") + "|PASS", StringComparison.Ordinal)),
        "Actual runtime or assembly evidence differs.");
    Need(numeric ? output.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal) &&
        output.Contains("MATH-M11-BINARY|quantitiesDll=" + Sha("dw.quantities", "dllSha256") + "|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS", StringComparison.Ordinal)
        : output.Contains("dw.tools.math/exact-consumer/0.2 qualified", StringComparison.Ordinal) &&
        output.Contains("MATH-M10-BROAD-SEMANTICS|PASS", StringComparison.Ordinal), "Semantic consumer evidence differs.");
    Need(Hash(lockPath) == lockHash, "Consumer lock changed.");
    Console.WriteLine("MATH-M10-CONSUMER|mode=" + mode + "|sdk=11.0.100|rid=" + expectedRid + "|packages=" + closure.Count +
        "|lockSha256=" + lockHash + "|sourceFree=true|PASS");
}
Need(coverage.SetEquals(packages.Keys) && packages.Keys.All(id => Hash(Archive(candidates, id)) == Sha(id, "archiveSha256") &&
    Hash(Archive(feed, id)) == Sha(id, "archiveSha256")), "Coverage or archive preservation differs.");
Console.WriteLine("MATH-M10-LOCKED-CONSUMERS|sdk=11.0.100|rid=" + expectedRid + "|tfm=net11.0|consumers=2|packages=13|rebuild=false|publication=false|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("M10 locked consumers: " + message);
}
static string Run(string dir, params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = dir, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
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
