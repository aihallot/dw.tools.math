#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string Skill = ".agents/skills/dw-tools-math";
const string Seed = "tests/consumers/m8-discovery-clr-seed/Program.cs";
const string FixtureMarker = "scope=source-project-consumer";
const string PortableMarker = "scope=package-reference-consumer";
const string Version = "20260.20261107.1001";
const string Protocol = "1.0.0-draft.1";
string Tick = ((char)96).ToString();
string Fence = new string((char)96, 3);
var inputs = new[]
{
    "SKILL.md", "references/index.md", "references/examples.md",
    "references/selected-capabilities.md", "references/manifest.json",
    "references/skill-version.json"
};
var packages = new[]
{
    (Id: "dw.tools.math.composition", Version: Version,
     Archive: "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f",
     Binary: "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252"),
    (Id: "dw.tools.math.ir", Version: "20260.20261104.1001",
     Archive: "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
     Binary: "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
Require(args.Length == 1 && args[0] == "--check" ||
        args.Length == 2 && args[0] == "--export",
    "Expected --check or --export <new-absolute-directory>.");
Require(File.Exists(Seed), "The original selected four-binding seed is missing.");
foreach (var name in inputs)
    Require(File.Exists(Path.Combine(Skill, name)), "Missing canonical skill file: " + name);
using (var source = JsonDocument.Parse(File.ReadAllText(
    "docs/discovery/qualification/m8-discovery-manifest.json")))
using (var installed = JsonDocument.Parse(File.ReadAllText(
    Path.Combine(Skill, "references/manifest.json"))))
{
    var first = source.RootElement;
    var second = installed.RootElement;
    Require(first.GetProperty("protocolVersion").GetString() == Protocol &&
        first.GetProperty("owner").GetString() == "math" &&
        first.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        second.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        second.GetProperty("capabilities").GetArrayLength() == 4 &&
        first.GetProperty("capabilities").GetArrayLength() == 4 &&
        first.GetProperty("capabilities").EnumerateArray().All(x =>
            x.GetProperty("packageVersion").GetString() == "0.3.0-preview.1" &&
            x.GetProperty("exposure").GetString() == "AgentCallable" &&
            x.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only") &&
        File.ReadAllBytes("docs/discovery/qualification/m8-discovery-manifest.json")
            .AsSpan().SequenceEqual(File.ReadAllBytes(
                Path.Combine(Skill, "references/manifest.json"))),
        "Canonical source-generated Discovery identity differs.");
}
if (args[0] == "--export")
{
    var absolute = args[1];
    Require(Path.IsPathFullyQualified(absolute), "Explicit export directory must be absolute.");
    absolute = Path.GetFullPath(absolute);
    Require(!Directory.Exists(absolute) && !File.Exists(absolute),
        "Export must not replace an existing destination.");
    var exported = Export(absolute);
    VerifyExport(absolute, exported);
    Console.WriteLine("MATH-RS281-PORTABLE-EXPORTED|files=" + exported.Length +
        "|privatePackages=not-included|authorization=runtime-only|path=" + absolute);
    return;
}
Require(OperatingSystem.IsWindows(), "Windows .NET 11 private-feed qualification required.");
var root = Path.GetFullPath(Path.Combine("build", "artifacts", "m8-rs281-portable-discovery",
    Guid.NewGuid().ToString("N")));
var a = Path.Combine(root, "export-a");
var b = Path.Combine(root, "export-b");
var paths = Export(a);
var secondPaths = Export(b);
Require(paths.SequenceEqual(secondPaths, StringComparer.Ordinal), "Export layout drift.");
foreach (var file in paths)
    Require(Hash(Path.Combine(a, file)) == Hash(Path.Combine(b, file)),
        "Second clean export differs: " + file);
VerifyExport(a, paths);
VerifyExport(b, secondPaths);
var privateFeed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(privateFeed) &&
    Path.IsPathFullyQualified(privateFeed) && Directory.Exists(privateFeed),
    "NUGET_LOCAL must name the existing absolute private feed.");
var isolatedFeed = Path.Combine(a, "build", "isolated-feed");
Directory.CreateDirectory(isolatedFeed);
foreach (var item in packages)
{
    var source = Path.Combine(privateFeed!, item.Id + "." + item.Version + ".nupkg");
    Require(File.Exists(source) && new FileInfo(source).Length > 0 &&
        (File.GetAttributes(source) & FileAttributes.ReparsePoint) == 0 &&
        Hash(source) == item.Archive, "Published immutable archive mismatch: " + item.Id);
    using (var zip = ZipFile.OpenRead(source))
    {
        var dll = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + item.Id + ".dll");
        using var bytes = dll.Open();
        Require(Convert.ToHexStringLower(SHA256.HashData(bytes)) == item.Binary,
            "Physical embedded package DLL mismatch: " + item.Id);
    }
    File.Copy(source, Path.Combine(isolatedFeed, Path.GetFileName(source)));
    Require(Hash(Path.Combine(isolatedFeed, Path.GetFileName(source))) == item.Archive,
        "Isolated feed archive mismatch: " + item.Id);
}
string RunDotnet(params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = a, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("Cannot start isolated dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Isolated net11 consumer timed out.");
    }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    Require(process.ExitCode == 0,
        "Isolated net11 command failed: " + string.Join(" ", arguments) +
        "\n" + output + "\n" + error);
    return output.Trim();
}
Require(RunDotnet("--version") == "11.0.100", "Unexpected isolated SDK.");
const string Project = "examples/net11/consumer.csproj";
var packageCache = Path.Combine(a, "build", "nuget-package-cache");
RunDotnet("restore", Project, "--configfile", "NuGet.Config",
    "--source", isolatedFeed, "--packages", packageCache, "-p:NuGetAudit=false");
var lockPath = Path.Combine(a, "examples", "net11", "packages.lock.json");
Require(File.Exists(lockPath), "Isolated package lockfile is absent.");
using (var lockDocument = JsonDocument.Parse(File.ReadAllText(lockPath)))
{
    var targets = lockDocument.RootElement.GetProperty("dependencies");
    if (!targets.TryGetProperty("net11.0", out var net11))
        throw new InvalidDataException("RS281: Missing net11.0 locked dependency group.");
    Require(targets.EnumerateObject().Count() == 1,
        "Portable example must restore only net11.0.");
    var closure = net11.EnumerateObject()
        .ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
    Require(closure.Count == packages.Length, "Unexpected private dependency count.");
    foreach (var item in packages)
        Require(closure.TryGetValue(item.Id, out var resolved) &&
            resolved.GetProperty("resolved").GetString() == item.Version &&
            resolved.GetProperty("type").GetString() ==
                (item.Id == "dw.tools.math.composition" ? "Direct" : "Transitive"),
            "Exact private package closure mismatch: " + item.Id);
}
var lockedHash = Hash(lockPath);
RunDotnet("restore", Project, "--configfile", "NuGet.Config",
    "--source", isolatedFeed, "--packages", packageCache, "--locked-mode",
    "-p:NuGetAudit=false");
Require(Hash(lockPath) == lockedHash, "Locked package graph changed.");
RunDotnet("build", Project, "-c", "Release", "--no-restore");
var outputDir = Path.Combine(a, "build", "bin", "consumer", "Release", "net11.0");
foreach (var item in packages)
    Require(Hash(Path.Combine(outputDir, item.Id + ".dll")) == item.Binary,
        "Published DLL bytes changed during isolated consumer build: " + item.Id);
var stdoutFinal = RunDotnet("run", "--project", Project,
    "-c", "Release", "--no-build", "--no-restore");
Require(stdoutFinal.Contains(
        "M8-DISCOVERY-CLR-SEED|bindings=4|examples=5|positiveComposition=2|brokerReady=0|scope=" +
        PortableMarker, StringComparison.Ordinal),
    "The installed package-only seed did not pass all original selected examples.");
foreach (var item in packages)
    Require(Hash(Path.Combine(privateFeed!, item.Id + "." + item.Version + ".nupkg")) ==
        item.Archive, "Original immutable private archive was modified: " + item.Id);
Console.WriteLine("MATH-RS281-PORTABLE-SKILL|exports=2|byteIdentical=true|" +
    "selected=4|examples=5|net11=11.0.100|lockedRestore=true|packages=3|" +
    "skillInstalledInIsolatedDirectory=true|brokerReady=0|publication=false|PASS");

string[] Export(string destination)
{
    Require(!Directory.Exists(destination) && !File.Exists(destination),
        "Portable export never overwrites an existing directory.");
    var files = new SortedSet<string>(StringComparer.Ordinal);
    void Put(string relative, string content)
    {
        var absolute = Path.GetFullPath(Path.Combine(destination, relative));
        Require(absolute.StartsWith(Path.GetFullPath(destination) +
            Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase),
            "Invalid exported relative path.");
        Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
        File.WriteAllText(absolute, content.Replace("\r\n", "\n", StringComparison.Ordinal),
            new UTF8Encoding(false));
        Require(files.Add(relative), "Duplicate output file: " + relative);
    }
    string ReadSource(string file) => File.ReadAllText(Path.Combine(Skill, file))
        .Replace("\r\n", "\n", StringComparison.Ordinal);
    foreach (var file in inputs)
    {
        var text = ReadSource(file);
        if (file == "SKILL.md")
        {
            text = ReplaceCount(text, "- Source example: " + Tick + Seed + Tick + ".",
                "- Portable published-package example: " + Tick +
                "examples/net11/Program.cs" + Tick + ".", 4);
            text = ReplaceCount(text,
                "Generated from " + Tick + "docs/discovery/qualification/m8-discovery-manifest.json" +
                Tick + "; qualified by RS186 and RS187. Recreate with " + Tick +
                "dotnet run --file docs/discovery/qualification/GenerateM8MathSkill.cs -- --write" +
                Tick + "; verify drift with " + Tick + "--check" + Tick + ".",
                "Portable export of the upstream source-generated Math Discovery skill. " +
                "See PORTABLE-README.md for the published-package example and provenance; " +
                "it grants no broker authorization.");
        }
        if (file == "references/index.md")
            text = ReplaceCount(text, "Runnable source consumer examples and limitations",
                "Portable .NET 11 published-package examples and limitations");
        if (file == "references/selected-capabilities.md")
            text = ReplaceCount(text,
                "This is source-project CLR evidence, not a package-installed or broker-authorized consumer. No public distribution or AURA adoption is implied.",
                "Signatures come from source-qualified CLR metadata. The separately exported example consumes already published private binaries; no agent-host installation, broker authorization, public release or AURA adoption is implied.");
        if (file == "references/examples.md")
        {
            text = ReplaceCount(text,
                "These examples run inside the repository's independent source-project consumer. They are evidence for **selected CLR operations**, not invocation authorization or broker routing.",
                "These examples run from the portable export as an independent .NET 11 published-package consumer. They are evidence for **selected CLR operations**, not invocation authorization or broker routing.");
            text = ReplaceCount(text,
                "From the Math repository root, with the repository's pinned toolchain and required restore inputs:",
                "From this export root with SDK 11.0.100 and the explicitly selected NUGET_LOCAL private feed:");
            text = ReplaceCount(text,
                "dotnet run --project tests/consumers/m8-discovery-clr-seed/m8-discovery-clr-seed.csproj -c Release",
                "dotnet restore examples/net11/consumer.csproj --source <ABSOLUTE_PRIVATE_FEED> -p:NuGetAudit=false\n" +
                "dotnet run --project examples/net11/consumer.csproj -c Release --no-restore");
            text = ReplaceCount(text,
                "Source: [M8 Discovery CLR seed](../../../../tests/consumers/m8-discovery-clr-seed/Program.cs). This consumer uses project references; it is **not** evidence that a fresh external consumer installed the immutable private packages.",
                "Portable source: [published-package net11 Program.cs](../../../../examples/net11/Program.cs). This export uses only a pinned PackageReference; it does **not** prove installation into any external agent host or broker execution.");
            text = ReplaceCount(text,
                "All four selected methods have typed source-project calls. This does **not** mean four independently installed package examples; the existing packaged verifier covers immutable " +
                Tick + "NUGET_LOCAL" + Tick + " archives and different invocation probes.",
                "All four selected methods have typed package-only calls in this exported example. The immutable " +
                Tick + "NUGET_LOCAL" + Tick + " archive identities remain pinned. This is not a broker-authorized or externally adopted skill.");
        }
        Put(Skill + "/" + file, text);
    }
    Put("global.json", "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
    Put("NuGet.Config", "<configuration><packageSources><clear /></packageSources></configuration>\n");
    Put("Directory.Build.props",
        "<Project><PropertyGroup><TargetFramework>net11.0</TargetFramework>" +
        "<LangVersion>15.0</LangVersion><Nullable>enable</Nullable>" +
        "<ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
        "<BaseOutputPath>$(MSBuildThisFileDirectory)build/bin/$(MSBuildProjectName)/</BaseOutputPath>" +
        "<BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)build/obj/$(MSBuildProjectName)/</BaseIntermediateOutputPath>" +
        "</PropertyGroup></Project>\n");
    Put("examples/net11/consumer.csproj",
        "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
        "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
        "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>" +
        "<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>" +
        "</PropertyGroup><ItemGroup><PackageReference Include=\"dw.tools.math.composition\"" +
        " Version=\"[" + Version + "]\" /></ItemGroup></Project>\n");
    Put("examples/net11/Program.cs", ReplaceCount(
        File.ReadAllText(Seed).Replace("\r\n", "\n", StringComparison.Ordinal),
        FixtureMarker, PortableMarker));
    Put("PORTABLE-README.md",
        "# Math Discovery portable private-package example\n\n" +
        "This deterministic export contains the four Math-selected source-qualified Discovery metadata entries, " +
        "their navigable references and an independent net11 PackageReference sample. " +
        "It does not bundle any NuGet archive, install into an agent host, grant runtime authorization, " +
        "or prove AURA adoption. Published package versions are separate from the source manifest version.\n\n" +
        "Required SDK: 11.0.100; private feed: NUGET_LOCAL containing immutable Composition " +
        Version + ", IR " + packages[1].Version + ", and Quantities " + packages[2].Version + ".\n\n" +
        Fence + "text\n" +
        "dotnet restore examples/net11/consumer.csproj --source <ABSOLUTE_PRIVATE_FEED> -p:NuGetAudit=false\n" +
        "dotnet run --project examples/net11/consumer.csproj -c Release --no-restore\n" +
        Fence + "\n\n" +
        "Source lineage: tests/consumers/m8-discovery-clr-seed/Program.cs. " +
        "The only source adaptation is its output scope label; its four selected typed operations remain unchanged. " +
        "The source-generated canonical Math skill remains in the original repository.\n");
    return files.ToArray();
}
void VerifyExport(string destination, string[] expected)
{
    Require(expected.Length == 12 &&
        Directory.GetFiles(destination, "*", SearchOption.AllDirectories).Length ==
            expected.Length, "Incomplete or extra portable output.");
    var bundle = Path.Combine(destination, Skill);
    using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(bundle, "references", "manifest.json")));
    var root = manifest.RootElement;
    Require(root.GetProperty("capabilities").GetArrayLength() == 4 &&
        root.GetProperty("brokerCandidateIds").GetArrayLength() == 0 &&
        root.GetProperty("capabilities").EnumerateArray().All(x =>
            x.GetProperty("exposure").GetString() == "AgentCallable" &&
            x.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only"),
        "Portable metadata changed selection or runtime authorization.");
    var index = File.ReadAllText(Path.Combine(bundle, "references", "index.md"));
    var examples = File.ReadAllText(Path.Combine(bundle, "references", "examples.md"));
    var skill = File.ReadAllText(Path.Combine(bundle, "SKILL.md"));
    Require(skill.Contains("(references/index.md)", StringComparison.Ordinal) &&
        index.Contains("(examples.md)", StringComparison.Ordinal) &&
        examples.Contains("](../../../../examples/net11/Program.cs)", StringComparison.Ordinal) &&
        skill.Contains("Do not expose any capability as BrokerReady.", StringComparison.Ordinal) &&
        examples.Contains("not invocation authorization or broker routing", StringComparison.Ordinal),
        "Portable selected links or authorization boundary changed.");
    var linked = Path.GetFullPath(Path.Combine(bundle, "references", "..", "..", "..", "..",
        "examples", "net11", "Program.cs"));
    Require(File.Exists(linked) && linked == Path.GetFullPath(
        Path.Combine(destination, "examples/net11/Program.cs")),
        "Example source is not reachable from installed skill.");
    var project = File.ReadAllText(Path.Combine(destination, "examples/net11/consumer.csproj"));
    var program = File.ReadAllText(linked);
    Require(project.Contains("<PackageReference Include=\"dw.tools.math.composition\"",
            StringComparison.Ordinal) &&
        project.Contains("[" + Version + "]", StringComparison.Ordinal) &&
        !project.Contains("<ProjectReference", StringComparison.OrdinalIgnoreCase) &&
        program.Contains("ExactQuantityPipeline.Evaluate(boundedSteps)", StringComparison.Ordinal) &&
        program.Contains("ExactReplayRunner.TryRun(boundedSteps", StringComparison.Ordinal) &&
        program.Contains("IrCanonicalJsonCodec.Encode(exact)", StringComparison.Ordinal) &&
        program.Contains("IrCanonicalHashes.SemanticStructuralSha256(exact)", StringComparison.Ordinal) &&
        program.Contains(PortableMarker, StringComparison.Ordinal) &&
        !program.Contains(FixtureMarker, StringComparison.Ordinal),
        "Portable consumer is not the selected source-free Math fixture.");
}
static string ReplaceCount(string source, string original, string target, int expected = 1)
{
    var count = source.Split(original, StringSplitOptions.None).Length - 1;
    Require(count == expected, "Canonical skill projection anchor changed: " + original);
    return source.Replace(original, target, StringComparison.Ordinal);
}
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static void Require(bool good, string reason)
{
    if (!good) throw new InvalidDataException("RS281: " + reason);
}
