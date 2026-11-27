#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 actual cutover: " + message); }
static string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var root = Path.GetFullPath(".");
string Run(string cwd, int timeout, params string[] arguments)
{
    var start = new ProcessStartInfo("dotnet") { WorkingDirectory = cwd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var arg in arguments) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeout)) { process.Kill(entireProcessTree: true); throw new TimeoutException(string.Join(" ", arguments)); }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    if (process.ExitCode != 0) throw new InvalidOperationException(string.Join(" ", arguments) + " failed:\n" + output + "\n" + error);
    return output;
}
var sdk = Run(root, 30000, "--version").Trim();
Need(sdk == "11.0.100", "Actual SDK differs: " + sdk);
Console.WriteLine("MATH-RS413-SDK|observed=" + sdk + "|PASS");
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solution.Length == 24 && solution.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13, "Solution roster differs.");
var ledger = JsonNode.Parse(File.ReadAllText("docs/engineering/m10-cutover-readiness-verification-rs409.json"))!;
var discovery = "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj";
var preflight = "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
var protectedInputs = ledger["mutationInputs"]!.AsArray().Select(x => (string)x!)
    .Concat(ledger["retainedInputs"]!.AsArray().Select(x => (string)x!))
    .Concat(ledger["historicalProjects"]!.AsArray().Select(x => (string)x!["path"]!))
    .Concat(new[] { preflight, "tools/dw.tools.math.nuget-preflight/Directory.Packages.props",
        "docs/discovery/agent-surface-candidates.json", "docs/discovery/qualification/m8-selected-clr-bindings.json" })
    .Distinct(StringComparer.Ordinal).ToDictionary(x => x, Hash, StringComparer.Ordinal);
var work = Path.Combine(root, "build", "artifacts", "m10-rs413-production-cutover", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(work);
try
{
    var packages = Path.Combine(work, "packages");
    Console.WriteLine(Run(root, 480000, "restore", "dw.tools.math.slnx", "--locked-mode", "--configfile", "NuGet.config", "--packages", packages));
    Console.WriteLine(Run(root, 480000, "build", "dw.tools.math.slnx", "-c", "Release", "--no-restore", "--no-incremental"));
    string Dll(string project)
    {
        var id = Path.GetFileNameWithoutExtension(project);
        var assets = Path.Combine(root, "build", "artifacts", "obj", id, "project.assets.json");
        var targets = JsonNode.Parse(File.ReadAllText(assets))!["targets"]!.AsObject();
        Need(targets.Count > 0 && targets.All(x => x.Key == "net11.0" || x.Key.StartsWith("net11.0/", StringComparison.Ordinal)), "Actual target differs: " + project);
        var dll = Path.Combine(root, "build", "artifacts", "bin", id, "Release", "net11.0", id + ".dll");
        Need(File.Exists(dll), "Missing target assembly: " + project);
        return dll;
    }
    foreach (var project in solution)
        Console.WriteLine("MATH-RS413-PROJECT|path=" + project + "|tfm=net11.0|dllSha256=" + Hash(Dll(project)) + "|PASS");
    var groups = new[]
    {
        (Project: "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
            Classes: new[] { "M11ExactInt64ScalingTests", "M11ExactInt64ScalingVerificationTests", "M1W01C01BudgetTests" }, Count: 9),
        (Project: "tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj",
            Classes: new[] { "M11StreamingMomentsTests", "M11StreamingMomentsVerificationTests",
                "M11LargerSampleQuantilesTests", "M11LargerSampleQuantilesVerificationTests", "M9W01C01BoundedSamplingTests" }, Count: 26)
    };
    foreach (var group in groups)
    {
        var resultDir = Path.Combine(work, "tests", Path.GetFileNameWithoutExtension(group.Project));
        Directory.CreateDirectory(resultDir);
        var filter = string.Join("|", group.Classes.Select(x => "FullyQualifiedName~" + x + "."));
        Console.WriteLine(Run(root, 300000, "test", group.Project, "-c", "Release", "--no-build", "--no-restore",
            "--filter", filter, "--results-directory", resultDir, "--logger", "trx;LogFileName=focused.trx"));
        var trxPath = Path.Combine(resultDir, "focused.trx");
        var trx = XDocument.Load(trxPath);
        var counters = trx.Descendants().Single(x => x.Name.LocalName == "Counters");
        var tests = trx.Descendants().Where(x => x.Name.LocalName == "UnitTest").ToDictionary(x => (string)x.Attribute("id")!,
            x => (string)x.Elements().Single(e => e.Name.LocalName == "TestMethod").Attribute("className")!, StringComparer.OrdinalIgnoreCase);
        var results = trx.Descendants().Where(x => x.Name.LocalName == "UnitTestResult").ToArray();
        var classes = results.Select(x => tests[(string)x.Attribute("testId")!].Split('.').Last()).ToHashSet(StringComparer.Ordinal);
        Need((int?)counters.Attribute("total") == group.Count && (int?)counters.Attribute("passed") == group.Count &&
            (int?)counters.Attribute("failed") == 0 && results.Length == group.Count && classes.SetEquals(group.Classes) &&
            results.All(x => (string?)x.Attribute("outcome") == "Passed"), "Fragile test result differs: " + group.Project);
        Console.WriteLine("MATH-RS413-TESTS|project=" + group.Project + "|classes=" + group.Classes.Length + "|passed=" + group.Count + "|trxSha256=" + Hash(trxPath) + "|PASS");
    }
    Console.WriteLine(Run(root, 240000, "restore", discovery, "--configfile", "NuGet.config", "--packages", packages));
    Console.WriteLine(Run(root, 240000, "build", discovery, "-c", "Release", "--no-restore", "--no-incremental"));
    var discoveryDll = Dll(discovery);
    var seed = "docs/discovery/agent-surface-candidates.json";
    var receipt = "docs/discovery/qualification/m8-selected-clr-bindings.json";
    foreach (var path in new[] { seed, receipt })
    {
        var destination = Path.Combine(work, path);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(path, destination);
    }
    var bindingOutput = Run(work, 120000, discoveryDll, "--write");
    Need(bindingOutput.Contains("M8-DISCOVERY-BINDINGS|selected=4|fingerprints=4|brokerReady=0|mode=--write", StringComparison.Ordinal) &&
        JsonNode.DeepEquals(JsonNode.Parse(File.ReadAllText(receipt)), JsonNode.Parse(File.ReadAllText(Path.Combine(work, receipt)))), "Discovery semantics differ.");
    Console.WriteLine("MATH-RS413-DISCOVERY|bindings=4|dllSha256=" + Hash(discoveryDll) + "|PASS");
    var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
    Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed), "Missing qualified local feed.");
    var archive = Path.Combine(feed!, "dw.tools.nuget.20260.20261026.1010.nupkg");
    const string archiveSha = "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf";
    const string coreSha = "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    Need(File.Exists(archive) && Hash(archive) == archiveSha, "Shared package differs.");
    var config = XDocument.Load("NuGet.config");
    var sources = config.Root!.Element("packageSources")!;
    Need(sources.Elements("add").Count() == 1 && (string?)sources.Elements("add").Single().Attribute("value") == "https://api.nuget.org/v3/index.json", "Source baseline differs.");
    sources.Add(new XElement("add", new XAttribute("key", "math-private-local"), new XAttribute("value", feed!)));
    var privateConfig = Path.Combine(work, "NuGet.maintained-tools.config");
    config.Save(privateConfig);
    Console.WriteLine(Run(root, 240000, "restore", preflight, "--configfile", privateConfig, "--packages", packages));
    Console.WriteLine(Run(root, 240000, "build", preflight, "-c", "Release", "--no-restore", "--no-incremental"));
    var preflightDll = Dll(preflight);
    Need(Hash(Path.Combine(Path.GetDirectoryName(preflightDll)!, "dw.tools.nuget.dll")) == coreSha, "Loaded shared library differs.");
    Need(Run(root, 120000, preflightDll, "--self-test").Contains("MATH-NUGET-F1-SELFTEST|PASS|5", StringComparison.Ordinal), "Preflight self-tests differ.");
    var inspected = JsonNode.Parse(Run(root, 420000, preflightDll, "--inspect", root))!;
    var rows = inspected["packages"]!.AsArray();
    var ids = solution.Where(x => x.StartsWith("src/projects/", StringComparison.Ordinal)).Select(x => Path.GetFileNameWithoutExtension(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
    Need((int?)inspected["packageCount"] == 13 && (int?)inspected["msbuildPropertyQueries"] == 13 &&
        (bool?)inspected["versionsReserved"] == false && (bool?)inspected["feedPublished"] == false &&
        (string?)inspected["repositoryPackageSha256"] == archiveSha && (string?)inspected["loadedLibrarySha256"] == coreSha &&
        rows.Count == 13 && ids.SetEquals(rows.Select(x => (string)x!["packageId"]!)) &&
        rows.All(x => (string?)x!["effectiveTargetFramework"] == "net11.0" && (bool?)x["candidateIsReserved"] == false), "Actual inspector differs.");
    Need(Hash(archive) == archiveSha, "Shared archive changed.");
    Console.WriteLine("MATH-RS413-INSPECT|packages=13|queries=13|selfTests=5|dllSha256=" + Hash(preflightDll) + "|publication=false|PASS");
    Need(protectedInputs.All(x => Hash(x.Key) == x.Value), "Production validation mutated protected input.");
    Console.WriteLine("MATH-RS413-PRODUCTION|sdk=11.0.100|tfm=net11.0|language=15.0|projects=24|tests=35|tools=2|bindings=4|inspect=13|lockedRestore=PASS|build=PASS|publication=false|PASS");
}
finally
{
    // Keep the restore cache with its build outputs; it is required by project.assets.json.
    // The work tree is ignored by the repository and scoped to this invocation.
}
