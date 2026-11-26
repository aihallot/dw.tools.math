#:property PublishAot=false
#:property NuGetAudit=false
using System.Text.Json.Nodes;

static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("Completed roadmap status: " + reason);
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
var backlog = Read("docs/planning/backlog.json");
var plan = Read(".aura/workflow/plan/project.json");
var state = Read(".aura/workflow/state.json");
var report = Read(".aura/workflow/reports/RS365/attempt-001.json");
var proof = Read(".workflow/runs/RS365/preparation-proof.json");
var ids = report["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
Need((string?)report["runId"] == "RS365" && (string?)report["outcome"] == "succeeded" &&
    (int?)report["attempt"] == 1 && (string?)report["executor"]?["toolVersion"] == "0.1.78" &&
    (int?)report["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)report["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    report["boundaryViolations"]!.AsArray().Count == 0 &&
    report["progress"]!["transitions"]!.AsArray().Count == 0 &&
    report["progress"]!["completions"]!.AsArray().Count == 0 &&
    (string?)proof["runId"] == "RS365" && (string?)proof["sourceCommit"] == "a43e14fe8a62b0d865d88e4fb8ba05ab4bea313b" &&
    (bool?)proof["payloadCompiled"] == true && (int?)proof["rehearsedValidationCount"] == 7 && ids.Length == 7 &&
    proof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(ids, StringComparer.Ordinal) &&
    ids.Contains("m9-rs365-canonical-completion-evidence", StringComparer.Ordinal), "RS365 durable proof differs.");
Need((string?)backlog["plan_version"] == "0.1.142" && (string?)backlog["status"] == "done", "Aggregate canonical status differs.");
var canonical = new List<JsonNode>();
foreach (var key in new[] { "releases", "work_packages", "chunks" })
    canonical.AddRange(backlog[key]!.AsArray().Select(x => x!));
foreach (var chunk in backlog["chunks"]!.AsArray())
foreach (var task in chunk!["tasks"]!.AsArray())
{
    canonical.Add(task!);
    canonical.AddRange(task!["subtasks"]!.AsArray().Select(x => x!));
}
Need(backlog["releases"]!.AsArray().Count == 10 && canonical.Count == 597 &&
    canonical.All(x => (string?)x["status"] == "done") &&
    canonical.Select(x => (string?)x["id"]).Distinct(StringComparer.Ordinal).Count() == 597, "Canonical completion roster differs.");
int nativeCount = 0;
void Walk(JsonNode node)
{
    Need((string?)node["state"] == "done", "Incomplete native node: " + (string?)node["id"]);
    nativeCount++;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node[key] is JsonArray children)
            foreach (var child in children) Walk(child!);
}
foreach (var milestone in plan["milestones"]!.AsArray()) Walk(milestone!);
Need(plan["milestones"]!.AsArray().Count == 10 && nativeCount == 591, "Native completion roster differs.");
foreach (var key in new[] { "activeMilestone", "activeWorkPackage", "activePhase", "activeTask", "activeSubtask" })
    Need(state.ContainsKey(key) && state[key] is null, "Unexpected active cursor: " + key);
var documents = new (string Path, string[] Markers)[]
{
    ("README.md", ["M0–M9 are complete", "597 completed nodes", "591 completed nodes", "20260.20261120.1001", "No BrokerReady grant", "Historical local qualification"]),
    ("docs/discovery/README.md", ["M8 is durably closed", "No BrokerReady authorization", "Historical qualification checkpoint — RS279"]),
    ("docs/distribution/current-local-consumption.md", ["Current checkpoint — RS365", "Historical qualified snapshot", "20260.20261120.1001", "No public NuGet release"]),
    ("docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md", ["M9 is done in the native plan and canonical backlog", "adoption not yet qualified", "Those historical states were superseded"]),
    ("docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md", ["Assessa adoption pending", "Subsequent actual native RED/GREEN/Verification", "No new publication or downstream adoption"]),
    ("docs/planning/implementation-handoff.md", ["The accepted M0–M9 roadmap is completed", "Historical adoption situation"])
};
foreach (var document in documents)
{
    var content = File.ReadAllText(document.Path);
    Need(document.Markers.All(x => content.Contains(x, StringComparison.Ordinal)), "Current or historical status marker missing: " + document.Path);
}
Console.WriteLine("MATH-RS366-COMPLETED-ROADMAP|canonical=597|native=591|milestones=10|status=done|docs=6|publication=false|adoption=false|PASS");
