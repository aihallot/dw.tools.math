#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;

var backlog = JsonNode.Parse(File.ReadAllText("docs/planning/backlog.json"))!.AsObject();
var plan = JsonNode.Parse(File.ReadAllText(".aura/workflow/plan/project.json"))!.AsObject();
void Need(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
JsonObject Record(JsonArray source, string id) =>
    source.Select(x => x!.AsObject()).Single(x => x["id"]!.GetValue<string>() == id);
Need(backlog["plan_version"]!.GetValue<string>() == "0.1.129", "Unexpected backlog version.");
var releases = backlog["releases"]!.AsArray();
var release = Record(releases, "M9");
Need(release["status"]!.GetValue<string>() == "in_progress", "M9 canonical readiness missing.");
Need(release["depends_on"]!.AsArray().Single()!.GetValue<string>() == "M3", "M9 must depend on completed M3.");
Need(Record(releases, "M3")["status"]!.GetValue<string>() == "done", "Do not rewrite M3 history.");
var wp = Record(backlog["work_packages"]!.AsArray(), "M9-W01");
Need(wp["status"]!.GetValue<string>() == "in_progress", "M9 canonical work package readiness missing.");
var chunk = Record(backlog["chunks"]!.AsArray(), "M9-W01-C01");
Need(chunk["status"]!.GetValue<string>() == "in_progress", "M9 canonical phase readiness missing.");
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
Need(milestone["state"]!.GetValue<string>() == "in-progress", "M9 source RED must be active.");
var nativeWp = Record(milestone["workPackages"]!.AsArray(), "M9-W01");
var phase = Record(nativeWp["phases"]!.AsArray(), "M9-W01-C01");
Need(nativeWp["state"]!.GetValue<string>() == "in-progress" && phase["state"]!.GetValue<string>() == "in-progress", "M9 work package and phase must be active.");
Need(phase["tasks"]!.AsArray().Count == 2, "Native plan must contain both tasks.");
foreach (var task in phase["tasks"]!.AsArray().Select(x => x!.AsObject()))
{
    Need(task["state"]!.GetValue<string>() == (task["id"]!.GetValue<string>() == "M9-W01-C01-T1" ? "in-progress" : "not-ready"), "Unexpected task progress.");
    Need(task["subtasks"]!.AsArray().Count == 3, "Expected RED/GREEN/verify subtasks.");
    Need(task["subtasks"]!.AsArray().All(x => x!["state"]!.GetValue<string>() == (x["id"]!.GetValue<string>() switch { "M9-W01-C01-T1-R" or "M9-W01-C01-T1-G" => "done", "M9-W01-C01-T1-V" => "ready", _ => "not-ready" })),
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
var redReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS350/attempt-001.json"))!.AsObject();
var redProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS350/preparation-proof.json"))!.AsObject();
var validationIds = redReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
Need((string?)redReport["runId"] == "RS350" && (string?)redReport["outcome"] == "succeeded" &&
    (int?)redReport["attempt"] == 1 && (string?)redReport["executor"]?["toolVersion"] == "0.1.76" &&
    (int?)redReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)redReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    redReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)redReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    redReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    redReport["progress"]!["transitions"]!.AsArray().Count == 5 &&
    redReport["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T1-R" && (string?)x?["afterState"] == "in-progress") &&
    validationIds.Length == 8 && validationIds.Contains("m9-rs350-source-oracle-and-three-compiled-mutants", StringComparer.Ordinal) &&
    (string?)redProof["runId"] == "RS350" && (bool?)redProof["payloadCompiled"] == true &&
    (int?)redProof["rehearsedValidationCount"] == 8 &&
    redProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(validationIds, StringComparer.Ordinal),
    "RS350 durable RED qualification evidence differs.");
Need(!redReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "RED modified production source.");
var closure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS351/attempt-001.json"))!.AsObject();
Need((string?)closure["runId"] == "RS351" && (string?)closure["outcome"] == "succeeded" &&
    (int?)closure["attempt"] == 1 && (string?)closure["executor"]?["toolVersion"] == "0.1.76" &&
    (int?)closure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)closure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    closure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)closure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    closure["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)closure["progress"]!["completions"]![0]!["id"] == "M9-W01-C01-T1-R" &&
    (string?)closure["progress"]!["completions"]![0]!["afterState"] == "done" &&
    closure["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T1-G" && (string?)x?["afterState"] == "ready"),
    "RS351 durable RED closure or GREEN readiness differs.");
var productTask = Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T1");
var productRed = Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-R");
var productGreen = Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-G");
Need((string?)productRed["status"] == "done" &&
    productRed["evidence"]!.AsArray().Count == 1 &&
    (string?)productRed["evidence"]![0] == ".aura/workflow/reports/RS351/attempt-001.json" &&
    (string?)productGreen["status"] == "in_progress" && productGreen["evidence"]!.AsArray().Count == 0 &&
    (string?)Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-V")["status"] == "planned", "Canonical durable RED closure and GREEN activation projection differs.");
var greenReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS352/attempt-001.json"))!.AsObject();
var greenProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS352/preparation-proof.json"))!.AsObject();
var greenIds = greenReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedGreenIds = [
    "m9-rs352-authentic-source-independent-oracle", "m9-rs352-statistics-locked-regression",
    "m9-rs352-red-closure-evidence", "m9-rs352-native-green-active", "m9-rs352-published-handoff",
    "m9-rs352-request-reconciliation", "m9-rs352-m8-evidence-frontier", "m9-rs352-planning-render-check", "diff-check"
];
var greenTransitions = greenReport["progress"]!["transitions"]!.AsArray();
Need((string?)greenReport["runId"] == "RS352" && (string?)greenReport["outcome"] == "succeeded" &&
    (int?)greenReport["attempt"] == 1 && (string?)greenReport["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)greenReport["git"]?["commitMessage"] == "test: qualify existing M9 bounded sampling GREEN" &&
    (int?)greenReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)greenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    greenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)greenReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    greenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    greenTransitions.Count == 1 && (string?)greenTransitions[0]?["id"] == "M9-W01-C01-T1-G" &&
    (string?)greenTransitions[0]?["beforeState"] == "ready" && (string?)greenTransitions[0]?["afterState"] == "in-progress" &&
    greenIds.SequenceEqual(expectedGreenIds, StringComparer.Ordinal) &&
    (string?)greenProof["runId"] == "RS352" && (bool?)greenProof["payloadCompiled"] == true &&
    (int?)greenProof["rehearsedValidationCount"] == 9 &&
    greenProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(greenIds, StringComparer.Ordinal),
    "RS352 durable GREEN qualification or compiled preparation proof differs.");
Need(!greenReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "GREEN modified production source.");
Console.WriteLine("MATH-M9-RS353-GREEN-CLOSURE|RS352=first-pass|validations=9|T1GREEN=done|T1Verification=ready|assessaAdoption=pending|PASS");
