#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 post-cutover regression: " + message); }
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
var rid = RuntimeInformation.RuntimeIdentifier;
Need(!string.IsNullOrWhiteSpace(rid), "Runtime identifier absent.");
Console.WriteLine("MATH-RS419-SDK|observed=" + sdk + "|rid=" + rid + "|PASS");
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solution.Length == 24 && solution.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13 && solution.Count(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)) == 11, "Solution roster differs.");
var ledger = JsonNode.Parse(File.ReadAllText("docs/engineering/m10-cutover-readiness-verification-rs409.json"))!;
var preflight = "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
var protectedInputs = ledger["mutationInputs"]!.AsArray().Select(x => (string)x!)
    .Concat(ledger["retainedInputs"]!.AsArray().Select(x => (string)x!))
    .Concat(ledger["historicalProjects"]!.AsArray().Select(x => (string)x!["path"]!))
    .Concat(new[] { preflight, "tools/dw.tools.math.nuget-preflight/Directory.Packages.props",
        "docs/discovery/agent-surface-candidates.json", "docs/discovery/qualification/m8-selected-clr-bindings.json" })
    .Distinct(StringComparer.Ordinal).ToDictionary(x => x, Hash, StringComparer.Ordinal);
var work = Path.Combine(root, "build", "artifacts", "m10-rs419-regression", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(work);
try
{
    var packages = Path.Combine(work, "packages");
    Console.WriteLine(Run(root, 480000, "restore", "dw.tools.math.slnx", "--locked-mode", "--configfile", "NuGet.config", "--packages", packages));
    Console.WriteLine(Run(root, 480000, "build", "dw.tools.math.slnx", "-c", "Release", "--no-restore", "--no-incremental", "-warnaserror"));
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
        Console.WriteLine("MATH-RS419-PROJECT|path=" + project + "|tfm=net11.0|dllSha256=" + Hash(Dll(project)) + "|PASS");
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
        Console.WriteLine("MATH-RS419-TESTS|project=" + group.Project + "|classes=" + group.Classes.Length + "|passed=" + group.Count + "|trxSha256=" + Hash(trxPath) + "|PASS");
    }

    var testProjects = solution.Where(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)).ToArray();
    int total = 0;
    foreach (var project in testProjects)
    {
        var id = Path.GetFileNameWithoutExtension(project);
        var resultDir = Path.Combine(work, "full-tests", id);
        Directory.CreateDirectory(resultDir);
        Console.WriteLine(Run(root, 480000, "test", project, "-c", "Release", "--no-build", "--no-restore",
            "--results-directory", resultDir, "--logger", "trx;LogFileName=full.trx"));
        var trxPath = Path.Combine(resultDir, "full.trx");
        var trx = XDocument.Load(trxPath);
        var counters = trx.Descendants().Single(x => x.Name.LocalName == "Counters");
        var results = trx.Descendants().Where(x => x.Name.LocalName == "UnitTestResult").ToArray();
        var count = (int?)counters.Attribute("total") ?? 0;
        Need(count > 0 && (int?)counters.Attribute("executed") == count &&
            (int?)counters.Attribute("passed") == count && (int?)counters.Attribute("failed") == 0 &&
            results.Length == count && results.All(x => (string?)x.Attribute("outcome") == "Passed"),
            "Full regression differs or has unexecuted tests: " + project);
        total = checked(total + count);
        Console.WriteLine("MATH-RS419-FULL-PROJECT|project=" + project + "|total=" + count + "|passed=" + count +
            "|failed=0|trxSha256=" + Hash(trxPath) + "|PASS");
    }
    Need(testProjects.Length == 11 && total >= 35, "Incomplete full test roster.");
    Need(protectedInputs.All(x => Hash(x.Key) == x.Value), "Regression mutated protected input.");
    Console.WriteLine("MATH-RS419-REGRESSION|sdk=" + sdk + "|rid=" + rid +
        "|tfm=net11.0|projects=24|focused=35|testProjects=11|fullPassed=" + total +
        "|failed=0|lockedRestore=PASS|build=PASS|publication=false|PASS");
}
finally
{
    // Retain ignored build artifacts and TRX files with the invocation's isolated restore cache.
}
