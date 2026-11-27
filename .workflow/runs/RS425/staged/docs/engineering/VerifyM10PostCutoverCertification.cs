#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS425 independent certification: " + message);
}
static JsonNode Read(string path) => JsonNode.Parse(File.ReadAllText(path))!;
static string Hash(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static string Git(params string[] arguments)
{
    var info = new ProcessStartInfo("git") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("Git unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(60000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Git timed out."); }
    var result = stdout.GetAwaiter().GetResult();
    Need(process.ExitCode == 0, stderr.GetAwaiter().GetResult());
    return result;
}
const string GreenPath = "docs/engineering/m10-post-cutover-certification-green-rs424.json";
const string RedPath = "docs/engineering/m10-post-cutover-certification-red-rs417.json";
const string CandidatePath = "docs/engineering/m10-post-cutover-candidate-identities-rs420.json";
var green = Read(GreenPath);
var red = Read(RedPath);
var candidate = Read(CandidatePath);
Need(JsonNode.DeepEquals(green, JsonNode.Parse(Git("show", "91cff4a35b5eec69e71c235dcee900fe2be739f2:" + GreenPath))) &&
    JsonNode.DeepEquals(red, JsonNode.Parse(Git("show", "144d974f2b485020084e2a19ff2f33f3b1b1472a:" + RedPath))) &&
    JsonNode.DeepEquals(candidate, JsonNode.Parse(Git("show", "771fbdb008ab399d308870c4553357a9ef857ec8:" + CandidatePath))),
    "Historical ledger identity differs.");
var gates = green["requiredGates"]!.AsArray();
Need(gates.Count == 6 && gates.Select(x => (string?)x?["id"]).SequenceEqual(red["requiredGates"]!.AsArray().Select(x => (string?)x?["id"])) &&
    gates.All(x => (bool?)x?["qualified"] == true) && (bool?)green["qualified"] == false &&
    (bool?)green["independentVerificationQualified"] == false && (bool?)green["publication"] == false &&
    (bool?)green["reservation"] == false, "Certification boundary differs.");
var outputs = new Dictionary<string,string>(StringComparer.Ordinal);
foreach (var gate in gates)
foreach (var evidence in gate!["evidence"]!.AsArray())
{
    var run = (string)evidence!["run"]!;
    var path = (string)evidence["report"]!;
    var commit = (string)evidence["resultCommit"]!;
    var report = Read(path);
    Need(JsonNode.DeepEquals(report, JsonNode.Parse(Git("show", commit + ":" + path))) &&
        (string?)report["runId"] == run && (string?)report["outcome"] == "succeeded" &&
        (int?)report["attempt"] == 1 && (bool?)report["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
        (int?)report["priorInvocationFailures"]?["count"] == 0 && report["boundaryViolations"]!.AsArray().Count == 0 &&
        (string?)report["executor"]?["toolVersion"] == "0.1.81", "Delivered report differs: " + run);
    var validation = report["validations"]!.AsArray().Single(x => (string?)x?["Id"] == (string?)evidence["validation"])!;
    Need((int?)validation["ExitCode"] == 0 && (bool?)validation["TimedOut"] == false && (bool?)validation["Cancelled"] == false, "Validation failed: " + run);
    outputs[run] = (string)validation["StandardOutput"]!;
}
Need(outputs.Count == 5, "Gate report coverage differs.");
string[] Rows(string output, string prefix) => output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(x => x.TrimEnd('\r')).Where(x => x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
Dictionary<string,string> Fields(string row) => row.Split('|').Skip(1).Where(x => x.Contains('='))
    .ToDictionary(x => x[..x.IndexOf('=')],x => x[(x.IndexOf('=')+1)..],StringComparer.Ordinal);
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project").Select(x => (string)x.Attribute("Path")!).ToArray();
var testProjects = solution.Where(x => x.StartsWith("tests/projects/",StringComparison.Ordinal)).ToHashSet(StringComparer.Ordinal);
var full = Rows(outputs["RS419"],"MATH-RS419-FULL-PROJECT|").Select(Fields).ToArray();
var targeted = Rows(outputs["RS419"],"MATH-RS419-TESTS|").Select(Fields).ToArray();
Need(testProjects.Count == 11 && full.Length == 11 && testProjects.SetEquals(full.Select(x => x["project"])) &&
    full.All(x => x["total"] == x["passed"] && x["failed"] == "0" && x["trxSha256"].Length == 64) &&
    full.Sum(x => int.Parse(x["passed"],System.Globalization.CultureInfo.InvariantCulture)) == 2722 &&
    targeted.Length == 2 && targeted.Sum(x => int.Parse(x["passed"],System.Globalization.CultureInfo.InvariantCulture)) == 35 &&
    Rows(outputs["RS419"],"MATH-RS419-PROJECT|").Length == 24, "Regression roster or counts differ.");
var root = Path.GetFullPath((string)candidate["relativeRoot"]!);
var feed = Path.Combine(root,"feed");
var producer = Path.Combine(root,"producer");
var version = (string)candidate["version"]!;
var packages = candidate["packages"]!.AsArray().ToDictionary(x => (string)x!["id"]!, x => x!,StringComparer.Ordinal);
Need(packages.Count == 13 && Directory.GetFiles(feed,"*.nupkg").Length == 13 &&
    solution.Where(x => x.StartsWith("src/projects/",StringComparison.Ordinal)).Select(Path.GetFileNameWithoutExtension).ToHashSet(StringComparer.Ordinal).SetEquals(packages.Keys),
    "Candidate roster differs.");
var archiveRows = Rows(outputs["RS420"],"MATH-RS420-ARCHIVE|").Select(Fields).ToDictionary(x => x["id"],StringComparer.Ordinal);
Need(archiveRows.Count == 13, "Candidate report coverage differs.");
int edges = 0;
foreach (var (id,item) in packages.OrderBy(x => x.Key,StringComparer.Ordinal))
{
    var archive = Path.Combine(feed,id+"."+version+".nupkg");
    var expectedArchive = (string)item["archiveSha256"]!;
    var expectedDll = (string)item["dllSha256"]!;
    var expectedLock = (string)item["lockSha256"]!;
    var expectedDeps = item["dependencies"]!.AsArray().Select(x => (string)x!).ToArray();
    var row = archiveRows[id];
    Need(row["archiveSha256"] == expectedArchive && row["dllSha256"] == expectedDll && row["lockSha256"] == expectedLock &&
        row["version"] == version && row["tfm"] == "net11.0" && row["deps"] == string.Join(",",expectedDeps) &&
        Hash(archive) == expectedArchive,"Archive/report identity differs: "+id);
    var source = XDocument.Load(Path.Combine("src","projects",id,id+".csproj"));
    var actualDeps = source.Descendants("ProjectReference").Select(x => Path.GetFileNameWithoutExtension(
        ((string)x.Attribute("Include")!).Replace('\\','/'))).OrderBy(x => x,StringComparer.Ordinal).ToArray();
    Need(expectedDeps.SequenceEqual(actualDeps),"Current source graph differs: "+id);
    using var zip = ZipFile.OpenRead(archive);
    using var manifest = zip.Entries.Single(x => x.FullName == id+".nuspec").Open();
    var xml = XDocument.Load(manifest);
    var ns = xml.Root!.Name.Namespace;
    var meta = xml.Root.Element(ns+"metadata")!;
    var deps = meta.Descendants(ns+"dependency").ToArray();
    Need(meta.Element(ns+"id")?.Value == id && meta.Element(ns+"version")?.Value == version &&
        expectedDeps.SequenceEqual(deps.Select(x => (string)x.Attribute("id")!).OrderBy(x => x,StringComparer.Ordinal)) &&
        deps.All(x => (string?)x.Attribute("version") == "["+version+"]"),"Nuspec identity differs: "+id);
    var binaries = zip.Entries.Where(x => x.FullName.StartsWith("lib/",StringComparison.Ordinal) && x.FullName.EndsWith(".dll",StringComparison.Ordinal)).ToArray();
    Need(binaries.Length == 1 && binaries[0].FullName == "lib/net11.0/"+id+".dll","Archive TFM differs.");
    using var binary = binaries[0].Open();
    Need(Convert.ToHexStringLower(SHA256.HashData(binary)) == expectedDll &&
        Hash(Path.Combine(producer,"build","artifacts","bin",id,"Release","net11.0",id+".dll")) == expectedDll &&
        Hash(Path.Combine(producer,"src","projects",id,"packages.lock.json")) == expectedLock,"Producer readback differs: "+id);
    foreach (var run in new[] {"RS421","RS422"})
        Need(outputs[run].Contains("MATH-M10-LOADED|id="+id+"|sha256="+expectedDll+"|PASS",StringComparison.Ordinal),"Platform binary differs: "+run+" "+id);
    edges += deps.Length;
    Console.WriteLine("MATH-RS425-READBACK|id="+id+"|archiveSha256="+expectedArchive+"|dllSha256="+expectedDll+"|lockSha256="+expectedLock+"|PASS");
}
Need(edges == 16,"Exact dependency count differs.");
foreach (var (run,rid) in new[] {("RS421","win-x64"),("RS422","linux-x64")})
{
    var rows = Rows(outputs[run],"MATH-M10-CONSUMER|").Select(Fields).ToArray();
    Need(rows.Length == 2 && rows.Select(x => x["mode"]).ToHashSet(StringComparer.Ordinal).SetEquals(new[] {"numeric","broad"}) &&
        rows.All(x => x["sdk"] == "11.0.100" && x["rid"] == rid && x["sourceFree"] == "true" && x["lockSha256"].Length == 64 &&
            x["packages"] == (x["mode"] == "numeric" ? "2" : "12")) &&
        outputs[run].Contains("MATH-M10-LOCKED-CONSUMERS|sdk=11.0.100|rid="+rid+"|tfm=net11.0|consumers=2|packages=13|rebuild=false|publication=false|PASS",StringComparison.Ordinal),
        "Platform evidence differs: "+run);
}
var shared = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(shared) && Path.IsPathFullyQualified(shared) && Directory.Exists(shared),"Shared feed unavailable.");
var historical = green["sharedArchives"]!.AsArray().ToDictionary(x => (string)x!["file"]!,x => (string)x!["sha256"]!,StringComparer.OrdinalIgnoreCase);
var physical = Directory.EnumerateFiles(shared!,"*.nupkg",SearchOption.AllDirectories)
    .Where(x => packages.Keys.Any(id => Path.GetFileName(x).StartsWith(id+".",StringComparison.OrdinalIgnoreCase))).ToArray();
Need(historical.Count == 12 && physical.Length == 12 && historical.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(physical.Select(Path.GetFileName)) &&
    physical.All(x => Hash(x) == historical[Path.GetFileName(x)]),"Shared archive preservation or publication boundary differs.");
var restored = Rows(outputs["RS423"],"MATH-RS423-RESTORED|").Select(Fields).ToDictionary(x => x["path"],x => x["sha256"],StringComparer.Ordinal);
var prior = Read(".aura/workflow/reports/RS407/attempt-001.json");
var originals = Rows((string)prior["validations"]![0]!["StandardOutput"]!,"MATH-RS407-RESTORED|").Select(Fields).ToDictionary(x => x["path"],x => x["sha256"],StringComparer.Ordinal);
Need(restored.Count == 30 && originals.Count == 30 && restored.All(x => originals.TryGetValue(x.Key,out var hash) && hash == x.Value) &&
    Rows(outputs["RS423"],"MATH-RS423-PROJECT|").Length == 24 &&
    outputs["RS423"].Contains("MATH-RS423-ROLLBACK|sdk=10.0.404|tfm=net10.0|restored=30|projects=24|smoke=9|candidateArchives=13|sharedArchives=12|lockedRestore=PASS|build=PASS|production=net11.0|publication=false|PASS",StringComparison.Ordinal),
    "Rollback evidence differs.");
Console.WriteLine("MATH-RS425-INDEPENDENT|gates=6|reports=5|regression=2722|focused=35|packages=13|edges=16|producerLocks=13|windows=PASS|linux=PASS|rollbackInputs=30|sharedArchives=12|rebuild=false|publication=false|PASS");
