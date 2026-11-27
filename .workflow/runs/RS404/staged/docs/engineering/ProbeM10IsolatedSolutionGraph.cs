#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--check" or "--focused")) throw new ArgumentException("Expected --check or --focused.");
var focused = args[0] == "--focused";
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
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solution.Length == 24 && solution.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13 &&
    solution.Count(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)) == 11, "Solution roster differs.");
var tracked = Run(repository, "git", 30000, "ls-files", "--", "src/projects", "tests/projects")
    .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
Need(tracked.Length == 287 && tracked.All(x => !x.Split('/').Any(part => part is "." or ".." or "bin" or "obj")), "Tracked source graph differs.");
var inputs = tracked.Concat(new[] { "global.json", "Directory.Build.props", "Directory.Packages.props", "NuGet.config", "dw.tools.math.slnx" })
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
    if (focused)
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
        foreach (var group in groups)
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
        Console.WriteLine("MATH-RS404-FOCUSED-TOTAL|sdk=11.0.100|tfm=net11.0|classes=8|passed=" + totalPassed + "|failed=0|PASS");
    }
    Need(locks.All(x => Hash(Path.Combine(work, x.Key)) == x.Value), "Build changed locked dependencies.");
    Need(originalHashes.All(x => Hash(x.Key) == x.Value), "Original repository input changed.");
    Console.WriteLine("MATH-RS403-ISOLATED|sdk=11.0.100|tfm=net11.0|lang=15.0|projects=24|lockedRestore=PASS|build=PASS|tests=" + (focused ? "focused" : "false") + "|tools=false|rollback=false|publication=false|PASS");
}
finally
{
    if (Directory.Exists(work)) Directory.Delete(work, true);
}
