#:property NuGetAudit=false
#:property PublishAot=false
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Green = ".aura/workflow/reports/RS335/attempt-001.json";
const string Closure = ".aura/workflow/reports/RS336/attempt-001.json";
const string Proof = ".workflow/runs/RS335/preparation-proof.json";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Profile = "docs/engineering/m8-w02-c02-source-free-consumer-red-rs333.json";
const string Fixture = "docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs";
const string Probe = "docs/engineering/ProbeM8W02StatisticsLockedConsumerGreen.cs";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "Missing private physical feed.");
var statistics = Path.Combine(feed!, "dw.tools.math.statistics.20260.20261120.1001.nupkg");
var quantities = Path.Combine(feed!, "dw.quantities.20260.20261102.1001.nupkg");
var tracked = new[] { Green, Closure, Proof, Risk, Profile, Fixture, Probe, "global.json",
    "Directory.Build.props", statistics, quantities };
var hashes = tracked.ToDictionary(path => path, Hash, StringComparer.Ordinal);
var green = Read(Green);
var closure = Read(Closure);
CheckGreen(green);
CheckRun(closure, "RS336");
var moves = closure["progress"]!["transitions"]!.AsArray();
Need(moves.Count == 2 &&
    moves[0]?["id"]?.GetValue<string>() == "M8-W02-C02-T2-G" &&
    moves[0]?["afterState"]?.GetValue<string>() == "done" &&
    moves[1]?["id"]?.GetValue<string>() == "M8-W02-C02-T2-V" &&
    moves[1]?["afterState"]?.GetValue<string>() == "ready" &&
    closure["progress"]?["completions"]?.AsArray().Count == 1, "Closure differs.");
var proof = Read(Proof);
Need(proof["runId"]?.GetValue<string>() == "RS335" &&
    proof["payloadCompiled"]?.GetValue<bool>() == true &&
    proof["rehearsedValidationCount"]?.GetValue<int>() == 7 &&
    proof["preparationValidationIds"]!.AsArray().Any(v =>
        v?.GetValue<string>() == "m8-rs335-statistics-source-free-locked-consumer"), "Compiled GREEN proof differs.");
var expectedIds = proof["preparationValidationIds"]!.AsArray().Select(v => v!.GetValue<string>()).ToArray();
Need(expectedIds.SequenceEqual(green["preparationValidationEvidence"]!["validations"]!.AsArray()
        .Select(v => v!["id"]!.GetValue<string>()), StringComparer.Ordinal) &&
    expectedIds.SequenceEqual(green["contribution"]!["observed"]!["validations"]!.AsArray()
        .Select(v => v!["id"]!.GetValue<string>()), StringComparer.Ordinal), "Proof and reports disagree.");
CheckArchive(statistics, "dw.tools.math.statistics", "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1",
    "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea");
CheckArchive(quantities, "dw.quantities", "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
    "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4");
var risk = Read(Risk);
Need(risk["status"]?.GetValue<string>() == "blocked-not-authorized" &&
    risk["gates"]?["migrationAuthorization"]?.GetValue<string>() == "not-granted" &&
    risk["observed"]?["publishedArchiveClosuresPhysicallyProven"]?.GetValue<int>() == 3 &&
    risk["observed"]?["publishedArchiveClosuresUnqualified"]?.GetValue<int>() == 10 &&
    risk["observed"]?["unqualifiedSemanticVerdicts"]?.GetValue<int>() == 13,
    "Historical migration-risk frontier changed.");
var profile = Read(Profile);
Need(profile["publicationAuthorized"]?.GetValue<bool>() == false &&
    profile["migrationAuthorized"]?.GetValue<bool>() == false, "Authorization widened.");
var props = XDocument.Load("Directory.Build.props");
Need(Read("global.json")["sdk"]?["version"]?.GetValue<string>() == "10.0.401" &&
    props.Descendants().First(e => e.Name.LocalName == "TargetFramework").Value.Trim() == "net10.0" &&
    props.Descendants().First(e => e.Name.LocalName == "LangVersion").Value.Trim() == "14.0",
    "Production baseline changed.");
var plan = Read(".aura/workflow/plan/project.json");
var m8 = plan["milestones"]!.AsArray().Single(v => v?["id"]?.GetValue<string>() == "M8")!;
var w2 = m8["workPackages"]!.AsArray().Single(v => v?["id"]?.GetValue<string>() == "M8-W02")!;
var phase = w2["phases"]!.AsArray().Single(v => v?["id"]?.GetValue<string>() == "M8-W02-C02")!;
var t2 = phase["tasks"]!.AsArray().Single(v => v?["id"]?.GetValue<string>() == "M8-W02-C02-T2")!;
Need(m8["state"]?.GetValue<string>() == "in-progress" && w2["state"]?.GetValue<string>() == "in-progress" &&
    phase["state"]?.GetValue<string>() == "in-progress" && t2["state"]?.GetValue<string>() == "in-progress" &&
    t2["subtasks"]!.AsArray().Count == 3 &&
    t2["subtasks"]!.AsArray().All(v => v?["state"]?.GetValue<string>() ==
        (v?["id"]?.GetValue<string>() switch {
            "M8-W02-C02-T2-R" => "done", "M8-W02-C02-T2-G" => "done",
            "M8-W02-C02-T2-V" => "in-progress", _ => "invalid" })),
    "Verification activation widened its scope.");
var negative = 0;
Reject("false-success", x => x["outcome"] = "failed");
Reject("missing-locked-consumer-evidence", x => x["preparationValidationEvidence"]!["validations"]!.AsArray().Clear());
Reject("concealed-boundary-violation", x => x["boundaryViolations"]!.AsArray().Add(JsonValue.Create("synthetic")));
Need(negative == 3, "Three evidence refusals required.");
foreach (var item in hashes) Need(Hash(item.Key) == item.Value, "Original changed: " + item.Key);
Console.WriteLine("MATH-RS337-STATISTICS-VERIFY|greenReport=PASS|closureReport=PASS|archiveAndDllIdentities=2|negativeControls=3|scope=bounded|migrationAuthorization=not-granted|originalsPreserved=true|PASS");

void Reject(string label, Action<JsonObject> mutate)
{
    var copy = JsonNode.Parse(green.ToJsonString())!.AsObject();
    mutate(copy);
    bool refused = false;
    try { CheckGreen(copy); } catch (InvalidDataException) { refused = true; }
    Need(refused, "Tampered GREEN evidence escaped: " + label);
    negative++;
    Console.WriteLine("MATH-RS337-NEGATIVE|" + label + "|PASS");
}
static void CheckGreen(JsonObject report)
{
    CheckRun(report, "RS335");
    var moves = report["progress"]!["transitions"]!.AsArray();
    Need(report["projectNodes"]?.AsArray().Count == 1 &&
        report["projectNodes"]![0]?.GetValue<string>() == "M8-W02-C02-T2-G" &&
        moves.Count == 1 && moves[0]?["afterState"]?.GetValue<string>() == "in-progress" &&
        report["progress"]?["completions"]?.AsArray().Count == 0 &&
        report["preparationValidationEvidence"]?["validations"]?.AsArray().Any(v =>
            v?["id"]?.GetValue<string>() == "m8-rs335-statistics-source-free-locked-consumer") == true &&
        report["contribution"]?["observed"]?["validations"]?.AsArray().Any(v =>
            v?["id"]?.GetValue<string>() == "m8-rs335-statistics-source-free-locked-consumer") == true,
        "Scoped GREEN proof absent.");
}
static void CheckRun(JsonObject report, string id)
{
    Need(report["runId"]?.GetValue<string>() == id &&
        report["attempt"]?.GetValue<int>() == 1 && report["outcome"]?.GetValue<string>() == "succeeded" &&
        report["correctionMode"]?.GetValue<bool>() == false &&
        report["executor"]?["toolVersion"]?.GetValue<string>() == "0.1.75" &&
        report["priorInvocationFailures"]?["count"]?.GetValue<int>() == 0 &&
        report["priorInvocationFailures"]?["firstPassSucceeded"]?.GetValue<bool>() == true &&
        report["boundaryViolations"]?.AsArray().Count == 0 &&
        report["payload"]?["process"]?["ExitCode"]?.GetValue<int>() == 0 &&
        report["contribution"]?["observed"]?["mutationBoundaryClean"]?.GetValue<bool>() == true,
        "Durable first-pass provenance differs: " + id);
}
static void CheckArchive(string path, string id, string archiveHash, string dllHash)
{
    Need((File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0 && Hash(path) == archiveHash,
        "Immutable archive differs: " + id);
    using var zip = ZipFile.OpenRead(path);
    using var dll = zip.Entries.Single(e => e.FullName == "lib/net10.0/" + id + ".dll").Open();
    Need(Convert.ToHexStringLower(SHA256.HashData(dll)) == dllHash, "Packaged DLL differs: " + id);
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
static void Need(bool ok, string reason) { if (!ok) throw new InvalidDataException("RS337-VERIFY: " + reason); }
