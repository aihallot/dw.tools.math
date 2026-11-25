#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check") throw new ArgumentException("Expected --check.");
const string Report = ".aura/workflow/reports/RS340/attempt-001.json";
const string Proof = ".workflow/runs/RS340/preparation-proof.json";
const string Previous = "docs/engineering/m8-w02-c02-phase-acceptance-rs339.json";
const string Profile = "docs/engineering/m8-w02-c02-phase-acceptance-rs341.json";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Probe = "docs/engineering/ProbeM8W02Net11StatisticsCandidate.cs";
const string BaseProject = "src/projects/dw.quantities/dw.quantities.csproj";
const string StatProject = "src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
var paths = new[] { Report, Proof, Previous, Profile, Risk, Probe, BaseProject, StatProject,
    "global.json", "Directory.Build.props", "docs/planning/backlog.json", ".aura/workflow/plan/project.json" };
var hashes = paths.ToDictionary(p => p, Hash, StringComparer.Ordinal);
var report = Read(Report);
var proof = Read(Proof);
Check(report, proof);
var previous = Read(Previous);
Need(previous["newNet11PackageGraphQualified"]?.GetValue<bool>() == false &&
    previous["phaseClosureAuthorized"]?.GetValue<bool>() == false, "RS339 historical assessment changed.");
var profile = Read(Profile);
var candidate = profile["candidate"]!;
Need(profile["phase"]?.GetValue<string>() == "M8-W02-C02" &&
    profile["status"]?.GetValue<string>() == "acceptance-incomplete" &&
    profile["supersedes"]?.GetValue<string>() == Previous &&
    profile["boundedNewNet11PackageGraphQualified"]?.GetValue<bool>() == true &&
    profile["productionMigrationAuthorized"]?.GetValue<bool>() == false &&
    profile["phaseClosureAuthorized"]?.GetValue<bool>() == false &&
    candidate["version"]?.GetValue<string>() == "20260.20261125.1001-rs340" &&
    candidate["sdk"]?.GetValue<string>() == "11.0.100" &&
    candidate["targetFramework"]?.GetValue<string>() == "net11.0" &&
    Strings(candidate, "packages").SequenceEqual(new[] { "dw.tools.math.statistics", "dw.quantities" }, StringComparer.Ordinal) &&
    candidate["dependency"]?.GetValue<string>() == "dw.tools.math.statistics -> dw.quantities" &&
    candidate["evidence"]?.GetValue<string>() == "RS340" &&
    candidate["firstPassSucceeded"]?.GetValue<bool>() == false &&
    candidate["originalArchiveFingerprintsRetained"]?.GetValue<bool>() == false &&
    candidate["publicationAuthorized"]?.GetValue<bool>() == false, "Bounded disposition overclaims candidate evidence.");
var r1 = Find(profile, "requirements", "M8-W02-C02-R1");
var r2 = Find(profile, "requirements", "M8-W02-C02-R2");
Need(profile["requirements"]!.AsArray().Count == 2 &&
    r1["status"]?.GetValue<string>() == "partial" &&
    Strings(r1, "missing").SequenceEqual(new[] { "targeted-source-migration-qualification" }, StringComparer.Ordinal) &&
    Strings(r1, "evidence").SequenceEqual(new[] { "RS329", "RS331", "RS335", "RS337", "RS340" }, StringComparer.Ordinal) &&
    r2["status"]?.GetValue<string>() == "preserved-in-observed-scope" &&
    Strings(r2, "evidence").SequenceEqual(new[] { "RS332", "RS338", "RS340" }, StringComparer.Ordinal) &&
    Strings(r2, "missing").Length == 0, "Requirement disposition differs.");
var risk = Read(Risk);
Need(risk["gates"]?["migrationAuthorization"]?.GetValue<string>() == "not-granted" &&
    risk["gates"]?["allPublishedPackageGraph"]?.GetValue<string>() == "unqualified", "Risk gate changed.");
var m8 = Find(Read(".aura/workflow/plan/project.json"), "milestones", "M8");
var w2 = Find(m8, "workPackages", "M8-W02");
var phase = Find(w2, "phases", "M8-W02-C02");
Need(m8["state"]?.GetValue<string>() == "in-progress" && w2["state"]?.GetValue<string>() == "in-progress" &&
    phase["state"]?.GetValue<string>() == "in-progress" && phase["tasks"]!.AsArray().Count == 2 &&
    phase["tasks"]!.AsArray().All(t => t?["state"]?.GetValue<string>() == "done"), "Phase closed prematurely.");
var baseDoc = XDocument.Load(BaseProject);
var statDoc = XDocument.Load(StatProject);
Need(!baseDoc.Descendants("ProjectReference").Any() &&
    statDoc.Descendants("ProjectReference").Single().Attribute("Include")?.Value ==
        "../dw.quantities/dw.quantities.csproj" &&
    !baseDoc.Descendants("PackageVersion").Any() && !statDoc.Descendants("PackageVersion").Any(),
    "Original project graph or candidate version boundary changed.");
var props = XDocument.Load("Directory.Build.props");
Need(Read("global.json")["sdk"]?["version"]?.GetValue<string>() == "10.0.401" &&
    props.Descendants("TargetFramework").Single().Value.Trim() == "net10.0" &&
    props.Descendants("LangVersion").Single().Value.Trim() == "14.0", "Production baseline changed.");
var negatives = 0;
Reject("false-first-pass", (r,p) => r["priorInvocationFailures"]!["firstPassSucceeded"] = true);
Reject("missing-preselection-failure", (r,p) => r["priorInvocationFailures"]!["failures"]!.AsArray().Clear());
Reject("missing-candidate-validation", (r,p) => p["preparationValidationIds"]!.AsArray().Clear());
Reject("wrong-correction-lineage", (r,p) => p["sourceCommit"] = new string('0', 40));
Need(negatives == 4, "Four independent evidence refusals required.");
foreach (var item in hashes) Need(Hash(item.Key) == item.Value, "Original changed: " + item.Key);
Console.WriteLine("MATH-RS341-CANDIDATE-VERIFY|report=PASS|proof=PASS|firstPass=false|negativeControls=4|boundedGraph=true|originalArchiveHashesRetained=false|phaseClosure=false|PASS");

void Reject(string label, Action<JsonObject, JsonObject> mutate)
{
    var r = JsonNode.Parse(report.ToJsonString())!.AsObject();
    var p = JsonNode.Parse(proof.ToJsonString())!.AsObject();
    mutate(r,p);
    bool refused = false;
    try { Check(r,p); } catch (InvalidDataException) { refused = true; }
    Need(refused, "Corrupted evidence accepted: " + label);
    negatives++;
    Console.WriteLine("MATH-RS341-NEGATIVE|" + label + "|PASS");
}
static void Check(JsonObject r, JsonObject p)
{
    var prior = r["priorInvocationFailures"]!;
    Need(r["runId"]?.GetValue<string>() == "RS340" &&
        r["outcome"]?.GetValue<string>() == "succeeded" &&
        r["attempt"]?.GetValue<int>() == 1 &&
        r["executor"]?["toolVersion"]?.GetValue<string>() == "0.1.76" &&
        r["correctionMode"]?.GetValue<bool>() == false &&
        r["payload"]?["process"]?["ExitCode"]?.GetValue<int>() == 0 &&
        r["boundaryViolations"]?.AsArray().Count == 0 &&
        r["contribution"]?["observed"]?["mutationBoundaryClean"]?.GetValue<bool>() == true &&
        Strings(r, "projectNodes").SequenceEqual(new[] { "M8-W02-C02" }, StringComparer.Ordinal) &&
        r["progress"]?["transitions"]?.AsArray().Count == 0 &&
        r["progress"]?["completions"]?.AsArray().Count == 0 &&
        prior["count"]?.GetValue<int>() == 1 &&
        prior["preparationFailureCount"]?.GetValue<int>() == 1 &&
        prior["allocatedAttemptFailureCount"]?.GetValue<int>() == 0 &&
        prior["firstPassSucceeded"]?.GetValue<bool>() == false &&
        prior["failures"]?.AsArray().Count == 1, "RS340 durable outcome or retry provenance differs.");
    var failure = prior["failures"]![0]!;
    Need(failure["invocationId"]?.GetValue<string>() == "run-next-20261125T161912Z-e4e3347515279d3b" &&
        failure["attemptAllocated"]?.GetValue<bool>() == false &&
        failure["failureStage"]?.GetValue<string>() == "pre-attempt" &&
        failure["evidencePublicationReference"]?.GetValue<string>() ==
            "refs/heads/workflow/evidence/preselection-rs340/run-next-20261125T161912Z-e4e3347515279d3b",
        "RS340 preselection evidence lineage differs.");
    var expected = new[] { "m8-rs340-new-net11-two-package-locked-consumer",
        "m8-rs340-source-migration-still-blocked", "m8-rs340-native-package-verification-task-done", "diff-check" };
    Need(p["runId"]?.GetValue<string>() == "RS340" &&
        p["sourceCommit"]?.GetValue<string>() == "482e2114461c39dbb8121b8dd8f352aff2037b4d" &&
        p["payloadCompiled"]?.GetValue<bool>() == true &&
        p["rehearsedValidationCount"]?.GetValue<int>() == 4 &&
        Strings(p, "preparationValidationIds").SequenceEqual(expected, StringComparer.Ordinal) &&
        r["preparationValidationEvidence"]!["validations"]!.AsArray()
            .Select(v => v!["id"]!.GetValue<string>()).SequenceEqual(expected, StringComparer.Ordinal) &&
        r["contribution"]!["observed"]!["validations"]!.AsArray()
            .Select(v => v!["id"]!.GetValue<string>()).SequenceEqual(expected, StringComparer.Ordinal),
        "RS340 preparation proof and observed validations disagree.");
}
static string[] Strings(JsonNode n, string key) => n[key]!.AsArray().Select(v => v!.GetValue<string>()).ToArray();
static JsonNode Find(JsonNode n, string key, string id) => n[key]!.AsArray().Single(v => v?["id"]?.GetValue<string>() == id)!;
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
static void Need(bool ok, string why) { if (!ok) throw new InvalidDataException("RS341: " + why); }
