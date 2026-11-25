#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;

var backlog = JsonNode.Parse(File.ReadAllText("docs/planning/backlog.json"))!.AsObject();
var plan = JsonNode.Parse(File.ReadAllText(".aura/workflow/plan/project.json"))!.AsObject();
void Need(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
JsonObject Record(JsonArray source, string id) =>
    source.Select(x => x!.AsObject()).Single(x => x["id"]!.GetValue<string>() == id);
Need(backlog["plan_version"]!.GetValue<string>() == "0.1.125", "Unexpected backlog version.");
var releases = backlog["releases"]!.AsArray();
var release = Record(releases, "M9");
Need(release["status"]!.GetValue<string>() == "planned", "New release must remain planned.");
Need(release["depends_on"]!.AsArray().Single()!.GetValue<string>() == "M3", "M9 must depend on completed M3.");
Need(Record(releases, "M3")["status"]!.GetValue<string>() == "done", "Do not rewrite M3 history.");
var wp = Record(backlog["work_packages"]!.AsArray(), "M9-W01");
Need(wp["status"]!.GetValue<string>() == "planned", "M9 work package must remain planned.");
var chunk = Record(backlog["chunks"]!.AsArray(), "M9-W01-C01");
Need(chunk["status"]!.GetValue<string>() == "planned", "New sampling chunk must remain planned.");
Need(chunk["depends_on"]!.AsArray().Single()!.GetValue<string>() == "M3-W02-C03",
    "Sampling must depend on qualified existing SplitMix64.");
Need(chunk["tasks"]!.AsArray().Count == 2, "Expected source and package qualification tasks.");
Need(chunk["requirements"]!.AsArray().Count == 2, "Expected bounded sampling and consumer requirements.");
Need(chunk["acceptance"]!.GetValue<string>().Contains("rejection", StringComparison.OrdinalIgnoreCase),
    "Unbiased rejection contract missing.");
Need(chunk["boundary_acceptance"]!.GetValue<string>().Contains("Assessa", StringComparison.Ordinal),
    "External ownership boundary missing.");
Need(Record(backlog["chunks"]!.AsArray(), "M3-W02-C03")["status"]!.GetValue<string>() == "done",
    "Historical PRNG chunk must remain done.");
var milestone = Record(plan["milestones"]!.AsArray(), "M9");
Need(milestone["state"]!.GetValue<string>() == "ready", "M9 must be ready without activation.");
var nativeWp = Record(milestone["workPackages"]!.AsArray(), "M9-W01");
var phase = Record(nativeWp["phases"]!.AsArray(), "M9-W01-C01");
Need(nativeWp["state"]!.GetValue<string>() == "ready" && phase["state"]!.GetValue<string>() == "ready", "M9 work package and phase must be ready.");
Need(phase["tasks"]!.AsArray().Count == 2, "Native plan must contain both tasks.");
foreach (var task in phase["tasks"]!.AsArray().Select(x => x!.AsObject()))
{
    Need(task["state"]!.GetValue<string>() == (task["id"]!.GetValue<string>() == "M9-W01-C01-T1" ? "ready" : "not-ready"), "Unexpected task progress.");
    Need(task["subtasks"]!.AsArray().Count == 3, "Expected RED/GREEN/verify subtasks.");
    Need(task["subtasks"]!.AsArray().All(x => x!["state"]!.GetValue<string>() == (x["id"]!.GetValue<string>() == "M9-W01-C01-T1-R" ? "ready" : "not-ready")),
        "Subtasks must not claim progress.");
}
Need(Record(releases, "M8")["status"]!.GetValue<string>() == "done", "M8 canonical reconciliation absent.");
var m8 = Record(plan["milestones"]!.AsArray(), "M8");
Need(m8["state"]!.GetValue<string>() == "done", "Native M8 closure absent.");
var evidence = new (string Id, int Prior, string Validation, string Scope)[]
{
    ("RS267", 2, "assessa-m9-admission-contract", "M8"),
    ("RS268", 1, "m9-bounded-integer-exact-regressions", "M9"),
    ("RS269", 1, "m9-unpublished-statistics-candidate-exact-package", "M9"),
    ("RS270", 1, "m9-net11-locked-bounded-candidate-package-consumer", "M9"),
    ("RS271", 0, "m9-statistics-private-readiness-no-publish", "M9"),
    ("RS272", 1, "m9-private-publisher-disposable-rehearsal-and-net11-readback", "M9"),
    ("RS273", 0, "m9-physical-private-feed-independent-net11-readback", "M9"),
    ("RS274", 1, "m9-assessa-private-package-handoff-durable-identities", "M9"),
    ("RS275", 0, "m9-reconcile-delivered-assessa-request", "M9"),
    ("RS277", 0, "m9-published-bounded-statistics-linux-net11-independent-binary", "M9"),
    ("RS278", 0, "m9-assessa-handoff-rs277-linux-locked-binary-evidence", "M9")
};
foreach (var item in evidence)
{
    var r = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/" + item.Id + "/attempt-001.json"))!.AsObject();
    Need((string?)r["runId"] == item.Id && (string?)r["outcome"] == "succeeded" &&
        (int?)r["attempt"] == 1 && (string?)r["executor"]?["toolVersion"] == "0.1.66" &&
        (int?)r["priorInvocationFailures"]?["count"] == item.Prior &&
        (bool?)r["priorInvocationFailures"]?["firstPassSucceeded"] == (item.Prior == 0) &&
        r["boundaryViolations"]!.AsArray().Count == 0 &&
        (bool?)r["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
        r["projectNodes"]!.AsArray().Count == 1 && (string?)r["projectNodes"]![0] == item.Scope &&
        r["progress"]!["completions"]!.AsArray().Count == 0 &&
        r["contribution"]!["observed"]!["validations"]!.AsArray().Any(v => (string?)v?["id"] == item.Validation),
        "Historical evidence or scope differs: " + item.Id);
}
Console.WriteLine("MATH-M9-RS349-READINESS|version=0.1.125|historicalReports=11|M8=done|M9=ready|T1RED=ready|nativeCompletions=0|assessaAdoption=pending|PASS");
