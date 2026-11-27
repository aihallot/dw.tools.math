#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--check" or "--focused" or "--tools" or "--inspect" or "--rollback")) throw new ArgumentException("Expected --check, --focused, --tools, --inspect or --rollback.");
var focused = args[0] == "--focused";
var inspect = args[0] == "--inspect";
var tools = args[0] == "--tools" || inspect;
var rollback = args[0] == "--rollback";
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 isolated graph: " + message); }
static string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var repository = Path.GetFullPath(".");
string Run(string cwd, string executable, int timeout, params string[] arguments)
{
    var start = new ProcessStartInfo(executable) { WorkingDirectory = cwd, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var arg in arguments) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start " + executable);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeout)) { process.Kill(entireProcessTree: true); throw new TimeoutException(executable + " " + string.Join(" ", arguments)); }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    if (process.ExitCode != 0) throw new InvalidOperationException(executable + " " + string.Join(" ", arguments) + " failed:\n" + output + "\n" + error);
    return output;
}
// Observe the actual baseline SDK selected by the production latestPatch policy.
var baselineSdk = "";
if (inspect || rollback)
{
    var baselinePolicy = JsonNode.Parse(File.ReadAllText(Path.Combine(repository, "global.json")))!["sdk"]!;
    Need((string?)baselinePolicy["version"] == "10.0.401" &&
        (string?)baselinePolicy["rollForward"] == "latestPatch" &&
        (bool?)baselinePolicy["allowPrerelease"] == false, "Original baseline SDK policy differs.");
    baselineSdk = Run(repository, "dotnet", 30000, "--version").Trim();
    Console.WriteLine("MATH-RS406-SDK|context=production|policy=10.0.401/latestPatch|observed=" + baselineSdk);
    Need(Version.TryParse(baselineSdk, out var baselineVersion) &&
        baselineVersion.Major == 10 && baselineVersion.Minor == 0 &&
        baselineVersion.Build >= 401 && baselineVersion.Build < 500 &&
        baselineVersion.Revision == -1, "Production SDK is outside its stable 10.0.4xx feature band: " + baselineSdk);
}
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solution.Length == 24 && solution.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13 &&
    solution.Count(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)) == 11, "Solution roster differs.");
var tracked = Run(repository, "git", 30000, "ls-files", "--", "src/projects", "tests/projects")
    .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
Need(tracked.Length == 287 && tracked.All(x => !x.Split('/').Any(part => part is "." or ".." or "bin" or "obj")), "Tracked source graph differs.");
var toolInputs = new[] {
    "tools/dw.tools.math.discovery-bindings/Program.cs",
    "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj",
    "tools/dw.tools.math.nuget-preflight/Program.cs",
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj",
    "tools/dw.tools.math.nuget-preflight/Directory.Packages.props",
    "docs/discovery/agent-surface-candidates.json",
    "docs/discovery/qualification/m8-selected-clr-bindings.json"
};
var inputs = tracked.Concat(tools || rollback ? toolInputs : Array.Empty<string>()).Concat(new[] { "global.json", "Directory.Build.props", "Directory.Packages.props", "NuGet.config", "dw.tools.math.slnx" })
    .Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
var originalHashes = inputs.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var work = Path.Combine(repository, "build", "artifacts", "m10-rs403-isolated-solution", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(work);
try
{
    foreach (var relative in inputs)
    {
        var destination = Path.Combine(work, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(relative, destination, false);
        Need(Hash(destination) == originalHashes[relative], "Source copy differs: " + relative);
    }
    var rollbackPaths = new List<string>();
    var snapshot = Path.Combine(work, "rollback-originals");
    if (rollback)
    {
        var ledger = JsonNode.Parse(File.ReadAllText("docs/engineering/m10-cutover-readiness-red-rs400.json"))!;
        var rows = ledger["rollbackInputs"]!.AsArray();
        var expected = new[] { "global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx" }
            .Concat(solution.Select(p => p[..p.LastIndexOf('/')] + "/packages.lock.json")).ToHashSet(StringComparer.Ordinal);
        Need(rows.Count == 28 && expected.SetEquals(rows.Select(x => (string)x!["path"]!)), "Rollback ledger roster differs.");
        foreach (var row in rows)
        {
            var path = (string)row!["path"]!;
            var blob = (string)row["blob"]!;
            Need(Run(repository, "git", 30000, "rev-parse", "HEAD:" + path).Trim() == blob &&
                Run(repository, "git", 30000, "hash-object", "--path=" + path, path).Trim() == blob,
                "Rollback baseline Git identity differs: " + path);
            rollbackPaths.Add(path);
        }
        rollbackPaths.Add(toolInputs[1]);
        rollbackPaths.Add(toolInputs[2]);
        Need(rollbackPaths.Distinct(StringComparer.Ordinal).Count() == 30, "Rollback path roster differs.");
        foreach (var path in rollbackPaths)
        {
            var saved = Path.Combine(snapshot, path);
            Directory.CreateDirectory(Path.GetDirectoryName(saved)!);
            File.Copy(Path.Combine(work, path), saved);
            Need(Hash(saved) == originalHashes[path], "Rollback snapshot differs: " + path);
        }
        Console.WriteLine("MATH-RS407-SNAPSHOT|ledger=28|toolInputs=2|files=30|PASS");
    }
    var sdkPath = Path.Combine(work, "global.json");
    var sdk = JsonNode.Parse(File.ReadAllText(sdkPath))!.AsObject();
    Need((string?)sdk["sdk"]?["version"] == "10.0.401", "Original SDK baseline differs.");
    sdk["sdk"]!["version"] = "11.0.100";
    sdk["sdk"]!["rollForward"] = "disable";
    File.WriteAllText(sdkPath, sdk.ToJsonString() + "\n");
    var propsPath = Path.Combine(work, "Directory.Build.props");
    var props = XDocument.Load(propsPath);
    Need(props.Descendants("TargetFramework").Single().Value == "net10.0" &&
        props.Descendants("LangVersion").Single().Value == "14.0" &&
        props.Descendants("TreatWarningsAsErrors").Single().Value == "true", "Original framework or warning policy differs.");
    props.Descendants("TargetFramework").Single().Value = "net11.0";
    props.Descendants("LangVersion").Single().Value = "15.0";
    props.Save(propsPath);
    var locks = new SortedDictionary<string, string>(StringComparer.Ordinal);
    foreach (var project in solution)
    {
        var projectPath = Path.Combine(work, project);
        var projectXml = XDocument.Load(projectPath);
        Need(!projectXml.Descendants("TargetFramework").Any() && !projectXml.Descendants("TargetFrameworks").Any() &&
            !projectXml.Descendants("LangVersion").Any(), "Unexpected solution project override: " + project);
        var relativeLock = project[..project.LastIndexOf('/')] + "/packages.lock.json";
        var lockPath = Path.Combine(work, relativeLock);
        var doc = JsonNode.Parse(File.ReadAllText(lockPath))!.AsObject();
        var dependencies = doc["dependencies"]!.AsObject();
        Need(dependencies.Count == 1 && dependencies["net10.0"] is JsonObject, "Original lock TFM differs: " + project);
        var graph = dependencies["net10.0"]!.DeepClone();
        dependencies.Remove("net10.0");
        dependencies["net11.0"] = graph;
        File.WriteAllText(lockPath, doc.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }) + "\n");
        locks.Add(relativeLock, Hash(lockPath));
    }
    Need(Run(work, "dotnet", 30000, "--version").Trim() == "11.0.100", "Isolated SDK mismatch.");
    if (tools || rollback)
    {
        var discoveryProject = toolInputs[1];
        var xml = XDocument.Load(Path.Combine(work, discoveryProject));
        var target = xml.Descendants("TargetFramework").Single();
        Need(target.Value == "net10.0", "Discovery baseline target differs.");
        target.Value = "net11.0";
        xml.Save(Path.Combine(work, discoveryProject));
    }
    if (inspect || rollback)
    {
        // Qualify the proposed production change without altering the original tool.
        var sourcePath = Path.Combine(work, toolInputs[2]);
        var source = File.ReadAllText(sourcePath);
        const string anchor = "var sources = LoadSources(root);";
        const string oldTarget = "properties[\"TargetFramework\"] == \"net10.0\"";
        const string targetPolicy = """
            var central = XDocument.Load(Path.Combine(root, "Directory.Build.props"));
            var framework = central.Descendants("TargetFramework").Single().Value;
            var language = central.Descendants("LangVersion").Single().Value;
            Need((framework, language) is ("net10.0", "14.0") or ("net11.0", "15.0"),
                "unqualified central framework/language pair");
            var sources = LoadSources(root);
        """;
        Need(source.Split(anchor, StringSplitOptions.None).Length == 2 &&
            source.Split(oldTarget, StringSplitOptions.None).Length == 2, "Inspector source anchors differ.");
        source = source.Replace(anchor, targetPolicy, StringComparison.Ordinal)
            .Replace(oldTarget, "properties[\"TargetFramework\"] == framework", StringComparison.Ordinal);
        File.WriteAllText(sourcePath, source);
        Console.WriteLine("MATH-RS406-CANDIDATE|sourceSha256=" + Hash(sourcePath) + "|production=false");
    }
    if (tools)
    {
        var discovery = toolInputs[1];
        var preflight = toolInputs[3];
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed), "Missing qualified NUGET_LOCAL feed.");
        var archive = Path.Combine(feed!, "dw.tools.nuget.20260.20261026.1010.nupkg");
        const string archiveSha = "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf";
        Need(File.Exists(archive) && Hash(archive) == archiveSha, "Shared package archive identity differs.");
        // RS405 attempt 001: explicit --source HTTPS was treated as a Windows local path (NU1301).
        // Preserve the existing NuGet.org source and add the private source in a disposable XML config.
        var privateConfig = Path.Combine(work, "NuGet.maintained-tools.config");
        var config = XDocument.Load(Path.Combine(work, "NuGet.config"));
        var sources = config.Root!.Element("packageSources")!;
        Need(sources.Elements("add").Count() == 1 &&
            (string?)sources.Elements("add").Single().Attribute("value") == "https://api.nuget.org/v3/index.json",
            "Baseline NuGet sources differ.");
        sources.Add(new XElement("add", new XAttribute("key", "math-private-local"), new XAttribute("value", feed!)));
        config.Save(privateConfig);
        foreach (var project in inspect ? new[] { preflight } : new[] { discovery, preflight })
        {
            var id = Path.GetFileNameWithoutExtension(project);
            var restored = project == discovery
                ? Run(work, "dotnet", 480000, "restore", project, "--configfile", "NuGet.config", "--packages", Path.Combine(work, "packages"))
                : Run(work, "dotnet", 480000, "restore", project, "--configfile", privateConfig, "--packages", Path.Combine(work, "packages"));
            Console.WriteLine(restored);
            Console.WriteLine(Run(work, "dotnet", 480000, "build", project, "-c", "Release", "--no-restore"));
            var assets = Path.Combine(work, "build", "artifacts", "obj", id, "project.assets.json");
            var targets = JsonNode.Parse(File.ReadAllText(assets))!["targets"]!.AsObject();
            Need(targets.Count == 1 && targets["net11.0"] is JsonObject, "Tool restored wrong target: " + id);
            var dll = Path.Combine(work, "build", "artifacts", "bin", id, "Release", "net11.0", id + ".dll");
            Need(File.Exists(dll), "Missing compiled tool: " + id);
            if (project == discovery)
            {
                var receipt = Path.Combine(work, toolInputs[6]);
                var expected = JsonNode.Parse(File.ReadAllText(receipt));
                var output = Run(work, "dotnet", 120000, dll, "--write");
                Need(output.Contains("M8-DISCOVERY-BINDINGS|selected=4|fingerprints=4|brokerReady=0|mode=--write", StringComparison.Ordinal) &&
                    JsonNode.DeepEquals(expected, JsonNode.Parse(File.ReadAllText(receipt))), "Discovery binding semantics drifted.");
                Console.WriteLine(output);
            }
            else
            {
                Need(targets["net11.0"]!.AsObject().Any(x => x.Key.Equals("Dw.Tools.NuGet/20260.20261026.1010", StringComparison.OrdinalIgnoreCase)), "Shared package resolution differs.");
                var core = Path.Combine(Path.GetDirectoryName(dll)!, "dw.tools.nuget.dll");
                Need(File.Exists(core) && Hash(core) == "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61", "Dependency assembly identity differs.");
                var output = Run(work, "dotnet", 120000, dll, "--self-test");
                Need(output.Contains("MATH-NUGET-F1-SELFTEST|PASS|5", StringComparison.Ordinal), "NuGet self-test marker differs.");
                Console.WriteLine(output);
                Console.WriteLine("MATH-RS405-NUGET-DEPENDENCY|version=20260.20261026.1010|archiveSha256=" + Hash(archive) + "|dllSha256=" + Hash(core) + "|PASS");
                if (inspect)
                {
                    var expectedIds = solution.Where(p => p.StartsWith("src/projects/", StringComparison.Ordinal))
                        .Select(p => Path.GetFileNameWithoutExtension(p)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    JsonNode Inspect(string framework)
                    {
                        var result = Run(work, "dotnet", 900000, dll, "--inspect", work);
                        var receipt = JsonNode.Parse(result)!.AsObject();
                        Need((string?)receipt["status"] == "f1-source-and-feed-inventory-qualified-not-published" &&
                            (string?)receipt["repositoryPackageSha256"] == archiveSha &&
                            (string?)receipt["loadedLibrarySha256"] == Hash(core) &&
                            (int?)receipt["packageCount"] == 13 && (int?)receipt["msbuildPropertyQueries"] == 13 &&
                            (bool?)receipt["versionsReserved"] == false && (bool?)receipt["feedPublished"] == false,
                            "Inspector summary differs.");
                        var rows = receipt["packages"]!.AsArray();
                        Need(rows.Count == 13 && expectedIds.SetEquals(rows.Select(x => (string)x!["packageId"]!)) &&
                            rows.All(x => (string?)x!["effectiveTargetFramework"] == framework &&
                                (string?)x["effectivePackable"] == "true" && (bool?)x["candidateIsReserved"] == false &&
                                System.Text.RegularExpressions.Regex.IsMatch((string)x["candidateVersionPreview"]!,
                                    @"^20260\.2026[0-9]{4}\.1[0-9]{3}$")),
                            "Inspector row coverage, target or candidate policy differs.");
                        Console.WriteLine(result);
                        Console.WriteLine("MATH-RS406-INSPECT|target=" + framework + "|packages=13|queries=13|reserved=false|published=false|PASS");
                        return new JsonArray(rows.Select(row =>
                        {
                            var item = row!.DeepClone().AsObject();
                            foreach (var key in new[] { "effectiveTargetFramework", "candidateVersionPreview", "historicalArchives", "calendarArchives" })
                                item.Remove(key);
                            return (JsonNode?)item;
                        }).ToArray());
                    }
                    var net11 = Inspect("net11.0");
                    var targetProps = File.ReadAllBytes(propsPath);
                    var targetSdk = File.ReadAllBytes(sdkPath);
                    try
                    {
                        File.Copy(Path.Combine(repository, "Directory.Build.props"), propsPath, true);
                        File.Copy(Path.Combine(repository, "global.json"), sdkPath, true);
                        var isolatedBaselineSdk = Run(work, "dotnet", 30000, "--version").Trim();
                        Console.WriteLine("MATH-RS406-SDK|context=isolated-baseline|expected=" + baselineSdk + "|observed=" + isolatedBaselineSdk);
                        Need(isolatedBaselineSdk == baselineSdk, "Baseline inspection SDK differs: expected " + baselineSdk + ", observed " + isolatedBaselineSdk);
                        var net10 = Inspect("net10.0");
                        Need(JsonNode.DeepEquals(net10, net11), "Cross-target package identity or dependency drift.");
                        var invalid = XDocument.Load(propsPath);
                        invalid.Descendants("LangVersion").Single().Value = "15.0";
                        invalid.Save(propsPath);
                        var refused = false;
                        try { _ = Run(work, "dotnet", 120000, dll, "--inspect", work); }
                        catch (InvalidOperationException error) when (error.Message.Contains(
                            "MATH-NUGET-F1-REFUSED: unqualified central framework/language pair", StringComparison.Ordinal))
                        { refused = true; }
                        Need(refused, "Unsupported framework/language pair was not refused.");
                        Console.WriteLine("MATH-RS406-NEGATIVE|mixedFrameworkLanguage=REFUSED|PASS");
                    }
                    finally
                    {
                        File.WriteAllBytes(propsPath, targetProps);
                        File.WriteAllBytes(sdkPath, targetSdk);
                    }
                }
            }
            Console.WriteLine("MATH-RS405-TOOL|path=" + project + "|target=net11.0|dllSha256=" + Hash(dll) + "|PASS");
        }
        Need(Hash(archive) == archiveSha, "Shared package changed.");
        Need(locks.All(x => Hash(Path.Combine(work, x.Key)) == x.Value), "Tool restore changed solution locks.");
        Need(originalHashes.All(x => Hash(x.Key) == x.Value), "Original repository input changed.");
        Console.WriteLine(inspect
            ? "MATH-RS406-INSPECTOR|buildSdk=11.0.100|targets=2|packagesPerTarget=13|sourceParity=PASS|negative=PASS|production=false|publication=false|PASS"
            : "MATH-RS405-TOOLS|sdk=11.0.100|tfm=net11.0|tools=2|bindings=4|selfTests=5|inspect=false|publication=false|PASS");
        return;
    }
    var restore = Run(work, "dotnet", 480000, "restore", "dw.tools.math.slnx", "--locked-mode", "--configfile", "NuGet.config",
        "--packages", Path.Combine(work, "packages"));
    Console.WriteLine(restore);
    foreach (var item in locks)
        Need(Hash(Path.Combine(work, item.Key)) == item.Value, "Locked restore changed dependency lock: " + item.Key);
    var build = Run(work, "dotnet", 480000, "build", "dw.tools.math.slnx", "-c", "Release", "--no-restore");
    Console.WriteLine(build);
    foreach (var project in solution)
    {
        var id = Path.GetFileNameWithoutExtension(project);
        var assets = Path.Combine(work, "build", "artifacts", "obj", id, "project.assets.json");
        var targets = JsonNode.Parse(File.ReadAllText(assets))!["targets"]!.AsObject();
        Need(targets.Count > 0 && targets.All(x => x.Key == "net11.0" || x.Key.StartsWith("net11.0/", StringComparison.Ordinal)), "Wrong restored target: " + project);
        var dll = Path.Combine(work, "build", "artifacts", "bin", id, "Release", "net11.0", id + ".dll");
        Need(File.Exists(dll), "Missing compiled solution assembly: " + project);
        Console.WriteLine("MATH-RS403-PROJECT|path=" + project + "|target=net11.0|lockSha256=" + locks[project[..project.LastIndexOf('/')] + "/packages.lock.json"] + "|dllSha256=" + Hash(dll));
    }
    if (rollback)
    {
        Need(locks.All(x => Hash(Path.Combine(work, x.Key)) == x.Value), "Target build changed locks.");
        var changed = rollbackPaths.Where(path => Hash(Path.Combine(work, path)) != originalHashes[path]).ToHashSet(StringComparer.Ordinal);
        var expectedChanged = locks.Keys.Concat(new[] { "global.json", "Directory.Build.props", toolInputs[1], toolInputs[2] }).ToHashSet(StringComparer.Ordinal);
        Need(changed.SetEquals(expectedChanged) && changed.Count == 28, "Target rollback delta differs.");
        Console.WriteLine("MATH-RS407-TARGET|sdk=11.0.100|projects=24|changedInputs=28|PASS");
        foreach (var path in rollbackPaths)
        {
            var saved = Path.Combine(snapshot, path);
            Need(Hash(saved) == originalHashes[path], "Saved rollback input changed: " + path);
            File.Copy(saved, Path.Combine(work, path), true);
            Need(Hash(Path.Combine(work, path)) == originalHashes[path], "Restored input differs: " + path);
            Console.WriteLine("MATH-RS407-RESTORED|path=" + path + "|sha256=" + Hash(Path.Combine(work, path)) + "|PASS");
        }
        Need(inputs.All(path => Hash(Path.Combine(work, path)) == originalHashes[path]), "Rollback did not restore all copied inputs.");
        var restoredSdk = Run(work, "dotnet", 30000, "--version").Trim();
        Need(restoredSdk == baselineSdk, "Rollback SDK differs: expected " + baselineSdk + ", observed " + restoredSdk);
        Directory.Delete(Path.Combine(work, "build", "artifacts"), true);
        Console.WriteLine(Run(work, "dotnet", 480000, "restore", "dw.tools.math.slnx", "--locked-mode", "--configfile", "NuGet.config",
            "--packages", Path.Combine(work, "packages-rollback")));
        Console.WriteLine(Run(work, "dotnet", 480000, "build", "dw.tools.math.slnx", "-c", "Release", "--no-restore"));
        foreach (var project in solution)
        {
            var id = Path.GetFileNameWithoutExtension(project);
            var assets = Path.Combine(work, "build", "artifacts", "obj", id, "project.assets.json");
            var targets = JsonNode.Parse(File.ReadAllText(assets))!["targets"]!.AsObject();
            Need(targets.Count > 0 && targets.All(x => x.Key == "net10.0" || x.Key.StartsWith("net10.0/", StringComparison.Ordinal)),
                "Rollback restored wrong target: " + project);
            var dll = Path.Combine(work, "build", "artifacts", "bin", id, "Release", "net10.0", id + ".dll");
            Need(File.Exists(dll), "Rollback compiled assembly missing: " + project);
            Console.WriteLine("MATH-RS407-PROJECT|path=" + project + "|target=net10.0|dllSha256=" + Hash(dll) + "|PASS");
        }
    }
    if (focused || rollback)
    {
        var groups = new[]
        {
            (Project: "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
                Classes: new[] { "M11ExactInt64ScalingTests", "M11ExactInt64ScalingVerificationTests", "M1W01C01BudgetTests" }, Minimum: 9),
            (Project: "tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj",
                Classes: new[] { "M11StreamingMomentsTests", "M11StreamingMomentsVerificationTests",
                    "M11LargerSampleQuantilesTests", "M11LargerSampleQuantilesVerificationTests",
                    "M9W01C01BoundedSamplingTests" }, Minimum: 26)
        };
        var totalPassed = 0;
        foreach (var group in rollback ? groups.Take(1) : groups)
        {
            var resultDirectory = Path.Combine(work, "test-results", Path.GetFileNameWithoutExtension(group.Project));
            Directory.CreateDirectory(resultDirectory);
            var filter = string.Join("|", group.Classes.Select(x => "FullyQualifiedName~" + x + "."));
            var output = Run(work, "dotnet", 480000, "test", group.Project, "-c", "Release", "--no-build", "--no-restore",
                "--filter", filter, "--results-directory", resultDirectory, "--logger", "trx;LogFileName=focused.trx");
            Console.WriteLine(output);
            var trxPath = Path.Combine(resultDirectory, "focused.trx");
            var trx = XDocument.Load(trxPath);
            var counters = trx.Descendants().Single(x => x.Name.LocalName == "Counters");
            int Count(string name) => (int?)counters.Attribute(name) ?? throw new InvalidDataException("Missing TRX counter: " + name);
            var tests = trx.Descendants().Where(x => x.Name.LocalName == "UnitTest").ToDictionary(x => (string)x.Attribute("id")!,
                x => (string)x.Elements().Single(e => e.Name.LocalName == "TestMethod").Attribute("className")!, StringComparer.OrdinalIgnoreCase);
            var results = trx.Descendants().Where(x => x.Name.LocalName == "UnitTestResult").ToArray();
            var actualClasses = results.Select(x => tests[(string)x.Attribute("testId")!].Split('.').Last()).ToHashSet(StringComparer.Ordinal);
            Need(actualClasses.SetEquals(group.Classes), "Focused test class coverage differs: " + group.Project);
            Need(Count("total") >= group.Minimum && Count("passed") == Count("total") &&
                Count("failed") == 0 && results.Length == Count("total") &&
                results.All(x => (string?)x.Attribute("outcome") == "Passed"), "Focused test result differs: " + group.Project);
            totalPassed += Count("passed");
            Console.WriteLine("MATH-RS404-FOCUSED|project=" + group.Project + "|classes=" + group.Classes.Length +
                "|passed=" + Count("passed") + "|failed=0|trxSha256=" + Hash(trxPath) + "|PASS");
        }
        Console.WriteLine(rollback
            ? "MATH-RS407-SMOKE|sdk=" + baselineSdk + "|tfm=net10.0|classes=3|passed=" + totalPassed + "|failed=0|PASS"
            : "MATH-RS404-FOCUSED-TOTAL|sdk=11.0.100|tfm=net11.0|classes=8|passed=" + totalPassed + "|failed=0|PASS");
    }
    if (rollback)
    {
        Need(inputs.All(path => Hash(Path.Combine(work, path)) == originalHashes[path]), "Restored input changed during baseline build or tests.");
        Need(originalHashes.All(x => Hash(x.Key) == x.Value), "Original repository input changed.");
        Console.WriteLine("MATH-RS407-ROLLBACK|sdk=" + baselineSdk + "|tfm=net10.0|restored=30|projects=24|lockedRestore=PASS|build=PASS|smoke=PASS|production=false|publication=false|PASS");
        return;
    }
    Need(locks.All(x => Hash(Path.Combine(work, x.Key)) == x.Value), "Build changed locked dependencies.");
    Need(originalHashes.All(x => Hash(x.Key) == x.Value), "Original repository input changed.");
    Console.WriteLine("MATH-RS403-ISOLATED|sdk=11.0.100|tfm=net11.0|lang=15.0|projects=24|lockedRestore=PASS|build=PASS|tests=" + (focused ? "focused" : "false") + "|tools=false|rollback=false|publication=false|PASS");
}
finally
{
    if (Directory.Exists(work)) Directory.Delete(work, true);
}
