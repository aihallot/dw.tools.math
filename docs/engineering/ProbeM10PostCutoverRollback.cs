#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Expected Windows rollback qualification.");
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 post-cutover rollback: " + message); }
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

const string baseline = "e4bbaa004ae643f2090c6c0010a6587b41e455eb";
const string delivered = "4fe239713d1edbdc15c445e989e40dd338ba19be";
string Git(params string[] arguments) => Run(repository, "git", 60000, arguments);
static string[] Lines(string value) => value.Split(['\r','\n'], StringSplitOptions.RemoveEmptyEntries);
static JsonNode Read(string path) => JsonNode.Parse(File.ReadAllText(path))!;
static string Sha(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));
Need(Run(repository, "dotnet", 30000, "--version").Trim() == "11.0.100", "Production SDK differs.");
var ledger = Read("docs/engineering/m10-cutover-readiness-verification-rs409.json");
var changes = ledger["mutationInputs"]!.AsArray().Select(x => (string)x!).ToHashSet(StringComparer.Ordinal);
var retained = ledger["retainedInputs"]!.AsArray().Select(x => (string)x!).ToArray();
var originalInputs = changes.Concat(retained.Where(x => x != "NuGet.config")).ToHashSet(StringComparer.Ordinal);
Need(changes.Count == 28 && originalInputs.Count == 30, "Rollback roster differs.");
var prior = Read(".aura/workflow/reports/RS407/attempt-001.json");
Need((string?)prior["outcome"] == "succeeded" && (int?)prior["validations"]![0]!["ExitCode"] == 0, "RS407 evidence absent.");
var receipts = Lines((string)prior["validations"]![0]!["StandardOutput"]!)
    .Where(x => x.StartsWith("MATH-RS407-RESTORED|", StringComparison.Ordinal))
    .Select(x => x.Split('|')).ToDictionary(x => x[1]["path=".Length..], x => x[2]["sha256=".Length..], StringComparer.Ordinal);
Need(receipts.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(originalInputs), "Rollback evidence roster differs.");
var inputs = Lines(Git("ls-files", "--", "src/projects", "tests/projects", "docs/distribution/m5-package-readme.md", "docs/distribution/m5-package-notices.md"))
    .Concat(originalInputs).Concat(retained).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(inputs.All(x => !Path.IsPathRooted(x) && !x.Split('/').Any(y => y is "." or "..")), "Invalid source path.");
var currentHashes = inputs.ToDictionary(x => x, Hash, StringComparer.Ordinal);
Need(!Lines(Git("diff", "--name-only", delivered, "--")).Any(x => currentHashes.ContainsKey(x)), "Production input drift.");

var identities = Read("docs/engineering/m10-post-cutover-candidate-identities-rs420.json");
var version = (string)identities["version"]!;
var candidateFeed = Path.Combine(repository, (string)identities["relativeRoot"]!, "feed");
var packageRows = identities["packages"]!.AsArray();
var candidateHashes = packageRows.ToDictionary(x => (string)x!["id"]!, x => (string)x!["archiveSha256"]!, StringComparer.Ordinal);
Need(candidateHashes.Count == 13 && Directory.GetFiles(candidateFeed, "*.nupkg").Length == 13, "Candidate roster differs.");
var shared = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(shared) && Path.IsPathFullyQualified(shared) && Directory.Exists(shared), "Shared feed missing.");
Dictionary<string,string> SharedSnapshot()
{
    var archives = Directory.EnumerateFiles(shared!, "*.nupkg", SearchOption.AllDirectories)
        .Where(x => candidateHashes.Keys.Any(id => Path.GetFileName(x).StartsWith(id + ".", StringComparison.OrdinalIgnoreCase))).ToArray();
    Need(!archives.Any(x => candidateHashes.Keys.Any(id => Path.GetFileName(x).Equals(id + "." + version + ".nupkg", StringComparison.OrdinalIgnoreCase))),
        "Unexpected candidate publication.");
    return archives.ToDictionary(x => x, Hash, StringComparer.OrdinalIgnoreCase);
}
var before = SharedSnapshot();
Need(before.Count == 12, "Shared Math archive roster differs from RS420.");
var historical = new Dictionary<string,string>(StringComparer.Ordinal)
{
    ["dw.quantities.20260.20261102.1001.nupkg"] = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
    ["dw.tools.math.statistics.20260.20261120.1001.nupkg"] = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1",
    ["dw.tools.math.statistics.20260.20261106.1001.nupkg"] = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8"
};
foreach (var item in historical)
    Need(before.Any(x => Path.GetFileName(x.Key).Equals(item.Key, StringComparison.OrdinalIgnoreCase) && x.Value == item.Value), "Historical published identity differs: " + item.Key);
var sharedTool = Path.Combine(shared!, "dw.tools.nuget.20260.20261026.1010.nupkg");
Need(Hash(sharedTool) == "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf", "Shared tool archive differs.");
void CheckCandidates()
{
    foreach (var item in packageRows)
    {
        var id = (string)item!["id"]!;
        var archive = Path.Combine(candidateFeed, id + "." + version + ".nupkg");
        Need(Hash(archive) == candidateHashes[id], "Candidate archive changed: " + id);
        using var zip = ZipFile.OpenRead(archive);
        using var dll = zip.Entries.Single(x => x.FullName == "lib/net11.0/" + id + ".dll").Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(dll)) == (string?)item["dllSha256"], "Candidate DLL changed: " + id);
    }
}
CheckCandidates();
var work = Path.Combine(repository, "build", "artifacts", "m10-rs423-rollback-" + Guid.NewGuid().ToString("N")[..12]);
Directory.CreateDirectory(work);
foreach (var relative in inputs)
{
    var destination = Path.Combine(work, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    File.Copy(relative, destination);
    Need(Hash(destination) == currentHashes[relative], "Source copy differs: " + relative);
}
foreach (var pair in receipts)
{
    var original = Git("show", baseline + ":" + pair.Key);
    var lf = original.Replace("\r\n", "\n", StringComparison.Ordinal);
    var crlf = lf.Replace("\n", "\r\n", StringComparison.Ordinal);
    var bytes = new[] { original, lf, crlf }.Select(Encoding.UTF8.GetBytes).FirstOrDefault(x => Sha(x) == pair.Value);
    Need(bytes is not null, "Exact original bytes unrecoverable: " + pair.Key);
    var destination = Path.Combine(work, pair.Key);
    File.WriteAllBytes(destination, bytes!);
    Need(Hash(destination) == pair.Value, "Restored input differs: " + pair.Key);
    Console.WriteLine("MATH-RS423-RESTORED|path=" + pair.Key + "|sha256=" + pair.Value + "|PASS");
}
Need(inputs.Where(x => !originalInputs.Contains(x)).All(x => Hash(Path.Combine(work,x)) == currentHashes[x]), "Unexpected rollback mutation.");
var props = XDocument.Load(Path.Combine(work,"Directory.Build.props"));
Need(props.Descendants("TargetFramework").Single().Value == "net10.0" &&
    props.Descendants("LangVersion").Single().Value == "14.0" &&
    props.Descendants("TreatWarningsAsErrors").Single().Value == "true", "Restored target differs.");
var sdk = Run(work, "dotnet", 30000, "--version").Trim();
Need(Version.TryParse(sdk,out var parsed) && parsed.Major == 10 && parsed.Minor == 0 && parsed.Build >= 401 && parsed.Build < 500,
    "Restored SDK policy differs: " + sdk);
var solution = XDocument.Load(Path.Combine(work,"dw.tools.math.slnx")).Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\','/')).ToArray();
Need(solution.Length == 24 && solution.Count(x => x.StartsWith("src/projects/",StringComparison.Ordinal)) == 13, "Restored graph differs.");
Console.WriteLine(Run(work,"dotnet",480000,"restore","dw.tools.math.slnx","--locked-mode","--configfile","NuGet.config","--packages",Path.Combine(work,"packages")));
Console.WriteLine(Run(work,"dotnet",480000,"build","dw.tools.math.slnx","-c","Release","--no-restore","-warnaserror"));
foreach (var project in solution)
{
    var id = Path.GetFileNameWithoutExtension(project);
    var assets = Read(Path.Combine(work,"build","artifacts","obj",id,"project.assets.json"))["targets"]!.AsObject();
    Need(assets.Count > 0 && assets.All(x => x.Key == "net10.0" || x.Key.StartsWith("net10.0/",StringComparison.Ordinal)), "Wrong restored TFM: " + project);
    var dll = Path.Combine(work,"build","artifacts","bin",id,"Release","net10.0",id + ".dll");
    Console.WriteLine("MATH-RS423-PROJECT|path=" + project + "|target=net10.0|dllSha256=" + Hash(dll) + "|PASS");
}
const string smoke = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
string[] classes = ["M11ExactInt64ScalingTests","M11ExactInt64ScalingVerificationTests","M1W01C01BudgetTests"];
var resultDir = Path.Combine(work,"test-results");
Directory.CreateDirectory(resultDir);
Console.WriteLine(Run(work,"dotnet",480000,"test",smoke,"-c","Release","--no-build","--no-restore",
    "--filter",string.Join("|",classes.Select(x => "FullyQualifiedName~" + x + ".")),
    "--results-directory",resultDir,"--logger","trx;LogFileName=rollback.trx"));
var trxPath = Path.Combine(resultDir,"rollback.trx");
var trx = XDocument.Load(trxPath);
var counters = trx.Descendants().Single(x => x.Name.LocalName == "Counters");
var tests = trx.Descendants().Where(x => x.Name.LocalName == "UnitTest").ToDictionary(x => (string)x.Attribute("id")!,
    x => (string)x.Elements().Single(e => e.Name.LocalName == "TestMethod").Attribute("className")!,StringComparer.OrdinalIgnoreCase);
var results = trx.Descendants().Where(x => x.Name.LocalName == "UnitTestResult").ToArray();
Need((int?)counters.Attribute("total") == 9 && (int?)counters.Attribute("passed") == 9 &&
    (int?)counters.Attribute("failed") == 0 && results.Length == 9 && results.All(x => (string?)x.Attribute("outcome") == "Passed") &&
    results.Select(x => tests[(string)x.Attribute("testId")!].Split('.').Last()).ToHashSet(StringComparer.Ordinal).SetEquals(classes), "Rollback smoke differs.");
Need(receipts.All(x => Hash(Path.Combine(work,x.Key)) == x.Value), "Restored inputs changed during build.");
Need(currentHashes.All(x => Hash(x.Key) == x.Value), "Production inputs changed.");
Need(Run(repository,"dotnet",30000,"--version").Trim() == "11.0.100", "Production SDK changed.");
CheckCandidates();
var after = SharedSnapshot();
Need(before.Count == after.Count && before.All(x => after.TryGetValue(x.Key,out var value) && value == x.Value), "Shared archives changed.");
Need(Hash(sharedTool) == "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf", "Shared tool archive changed.");
foreach (var item in before.OrderBy(x => x.Key,StringComparer.OrdinalIgnoreCase))
    Console.WriteLine("MATH-RS423-SHARED|path=" + item.Key + "|sha256=" + item.Value + "|PASS");
Console.WriteLine("MATH-RS423-SMOKE|sdk=" + sdk + "|tfm=net10.0|passed=9|trxSha256=" + Hash(trxPath) + "|PASS");
Console.WriteLine("MATH-RS423-ROLLBACK|sdk=" + sdk + "|tfm=net10.0|restored=30|projects=24|smoke=9|candidateArchives=13|sharedArchives=12|lockedRestore=PASS|build=PASS|production=net11.0|publication=false|PASS");
