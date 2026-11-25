#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check") throw new ArgumentException("Expected --check.");
const string Profile = "docs/engineering/m8-w02-c02-phase-acceptance-rs339.json";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Backlog = "docs/planning/backlog.json";
const string Plan = ".aura/workflow/plan/project.json";
var ids = new[] { "RS329", "RS331", "RS332", "RS335", "RS337", "RS338" };
var tracked = ids.Select(ReportPath).Concat(new[] { Profile, Risk, Backlog, Plan, "global.json", "Directory.Build.props" })
    .ToDictionary(x => x, Hash, StringComparer.Ordinal);
var reports = ids.ToDictionary(x => x, x => Read(ReportPath(x)), StringComparer.Ordinal);
foreach (var item in reports)
{
    var r = item.Value;
    Need(r["runId"]?.GetValue<string>() == item.Key &&
        r["outcome"]?.GetValue<string>() == "succeeded" &&
        r["attempt"]?.GetValue<int>() == 1 &&
        r["priorInvocationFailures"]?["count"]?.GetValue<int>() == 0 &&
        r["priorInvocationFailures"]?["firstPassSucceeded"]?.GetValue<bool>() == true &&
        r["boundaryViolations"]?.AsArray().Count == 0 &&
        r["contribution"]?["observed"]?["mutationBoundaryClean"]?.GetValue<bool>() == true,
        "Durable first-pass evidence missing: " + item.Key);
}
Need(Completed(reports["RS332"], "M8-W02-C02-T1") &&
    Completed(reports["RS338"], "M8-W02-C02-T2"), "Expected task closures missing.");
Need(HasValidation(reports["RS331"], "m8-rs331-targeted-foundation-dual-sdk-source-replay") &&
    HasValidation(reports["RS337"], "m8-rs337-targeted-statistics-locked-consumer-replay") &&
    HasValidation(reports["RS337"], "m8-rs337-independent-statistics-package-evidence-and-three-refusals"),
    "Bounded source or binary evidence missing.");
var backlog = Read(Backlog);
var chunk = Find(backlog, "chunks", "M8-W02-C02");
Need(chunk["deliverables"]?.GetValue<string>() ==
    "Migrate only measured targeted Math constructs and qualify a source-free consumer.", "Canonical deliverable drift.");
Need(Find(backlog, "requirements", "M8-W02-C02-R1")["acceptance"]?.GetValue<string>() ==
    "Prove new exact package graph, tests and external consumer behavior without old package overwrite." &&
    Find(backlog, "requirements", "M8-W02-C02-R2")["acceptance"]?.GetValue<string>() ==
    "No retrospective mutation of .NET 10 archives; no public release.", "Canonical acceptance drift.");
var m8 = Find(Read(Plan), "milestones", "M8");
var w2 = Find(m8, "workPackages", "M8-W02");
var phase = Find(w2, "phases", "M8-W02-C02");
Need(m8["state"]?.GetValue<string>() == "in-progress" && w2["state"]?.GetValue<string>() == "in-progress" &&
    phase["state"]?.GetValue<string>() == "in-progress" && phase["tasks"]!.AsArray().Count == 2 &&
    phase["tasks"]!.AsArray().All(t => t?["state"]?.GetValue<string>() == "done" &&
        t["subtasks"]!.AsArray().Count == 3 && t["subtasks"]!.AsArray().All(s => s?["state"]?.GetValue<string>() == "done")),
    "Task closure must not be confused with phase acceptance.");
var risk = Read(Risk);
Need(risk["status"]?.GetValue<string>() == "blocked-not-authorized" &&
    risk["gates"]?["migrationAuthorization"]?.GetValue<string>() == "not-granted" &&
    risk["gates"]?["allPublishedPackageGraph"]?.GetValue<string>() == "unqualified",
    "Migration gate changed.");
var props = XDocument.Load("Directory.Build.props");
Need(Read("global.json")["sdk"]?["version"]?.GetValue<string>() == "10.0.401" &&
    props.Descendants().First(e => e.Name.LocalName == "TargetFramework").Value.Trim() == "net10.0" &&
    props.Descendants().First(e => e.Name.LocalName == "LangVersion").Value.Trim() == "14.0",
    "Production baseline changed.");
var assessment = Read(Profile);
Check(assessment);
var count = 0;
Reject("premature-phase-acceptance", x => x["phaseClosureAuthorized"] = true);
Reject("unsupported-new-package-graph", x => x["newNet11PackageGraphQualified"] = true);
Reject("unsupported-migration-authorization", x => x["productionMigrationAuthorized"] = true);
Reject("missing-r1-gap", x => x["requirements"]![0]!["missing"]!.AsArray().Clear());
Need(count == 4, "Four negative controls required.");
foreach (var item in tracked) Need(Hash(item.Key) == item.Value, "Original changed: " + item.Key);
Console.WriteLine("MATH-RS339-C02-ASSESSMENT|tasks=2/2|R1=partial|R2=preserved-in-observed-scope|phaseClosure=false|newGraph=false|negativeControls=4|PASS");

void Reject(string label, Action<JsonObject> mutate)
{
    var copy = JsonNode.Parse(assessment.ToJsonString())!.AsObject();
    mutate(copy);
    bool refused = false;
    try { Check(copy); } catch (InvalidDataException) { refused = true; }
    Need(refused, "Tampered assessment accepted: " + label);
    count++;
    Console.WriteLine("MATH-RS339-NEGATIVE|" + label + "|PASS");
}
static void Check(JsonObject p)
{
    Need(p["schemaVersion"]?.GetValue<int>() == 1 && p["phase"]?.GetValue<string>() == "M8-W02-C02" &&
        p["status"]?.GetValue<string>() == "acceptance-incomplete" &&
        Strings(p, "taskClosures").SequenceEqual(new[] { "RS332", "RS338" }, StringComparer.Ordinal) &&
        p["newNet11PackageGraphQualified"]?.GetValue<bool>() == false &&
        p["productionMigrationAuthorized"]?.GetValue<bool>() == false &&
        p["publicationAuthorized"]?.GetValue<bool>() == false &&
        p["phaseClosureAuthorized"]?.GetValue<bool>() == false,
        "Unsupported phase disposition.");
    var r1 = Find(p, "requirements", "M8-W02-C02-R1");
    var r2 = Find(p, "requirements", "M8-W02-C02-R2");
    Need(p["requirements"]!.AsArray().Count == 2 &&
        r1["status"]?.GetValue<string>() == "partial" &&
        Strings(r1, "evidence").SequenceEqual(new[] { "RS329", "RS331", "RS335", "RS337" }, StringComparer.Ordinal) &&
        Strings(r1, "missing").SequenceEqual(new[] { "new-net11-package-graph", "targeted-source-migration-qualification" }, StringComparer.Ordinal) &&
        r2["status"]?.GetValue<string>() == "preserved-in-observed-scope" &&
        Strings(r2, "evidence").SequenceEqual(new[] { "RS332", "RS338" }, StringComparer.Ordinal) &&
        Strings(r2, "missing").Length == 0, "Requirement disposition or evidence differs.");
}
static string[] Strings(JsonNode p, string key) => p[key]!.AsArray().Select(x => x!.GetValue<string>()).ToArray();
static bool Completed(JsonObject r, string id) => r["progress"]!["completions"]!.AsArray().Any(x =>
    x?["id"]?.GetValue<string>() == id && x["afterState"]?.GetValue<string>() == "done");
static bool HasValidation(JsonObject r, string id) => r["contribution"]!["observed"]!["validations"]!.AsArray()
    .Any(x => x?["id"]?.GetValue<string>() == id);
static JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray()
    .Single(x => x?["id"]?.GetValue<string>() == id)!;
static string ReportPath(string id) => ".aura/workflow/reports/" + id + "/attempt-001.json";
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
static void Need(bool ok, string reason) { if (!ok) throw new InvalidDataException("RS339: " + reason); }
