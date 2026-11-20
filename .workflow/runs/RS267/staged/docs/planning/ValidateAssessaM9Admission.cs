#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;

var backlog = JsonNode.Parse(File.ReadAllText("docs/planning/backlog.json"))!.AsObject();
var plan = JsonNode.Parse(File.ReadAllText(".aura/workflow/plan/project.json"))!.AsObject();
void Need(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
JsonObject Record(JsonArray source, string id) =>
    source.Select(x => x!.AsObject()).Single(x => x["id"]!.GetValue<string>() == id);
Need(backlog["plan_version"]!.GetValue<string>() == "0.1.124", "Unexpected backlog version.");
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
Need(milestone["state"]!.GetValue<string>() == "not-ready", "M9 cannot be activated by admission.");
var nativeWp = Record(milestone["workPackages"]!.AsArray(), "M9-W01");
var phase = Record(nativeWp["phases"]!.AsArray(), "M9-W01-C01");
Need(phase["state"]!.GetValue<string>() == "not-ready", "New phase cannot be marked ready.");
Need(phase["tasks"]!.AsArray().Count == 2, "Native plan must contain both tasks.");
foreach (var task in phase["tasks"]!.AsArray().Select(x => x!.AsObject()))
{
    Need(task["state"]!.GetValue<string>() == "not-ready", "Unexpected task progress.");
    Need(task["subtasks"]!.AsArray().Count == 3, "Expected RED/GREEN/verify subtasks.");
    Need(task["subtasks"]!.AsArray().All(x => x!["state"]!.GetValue<string>() == "not-ready"),
        "Subtasks must not claim progress.");
}
Console.WriteLine("MATH-ASSSESSA-M9-ADMISSION|version=0.1.124|tasks=2|subtasks=6|historicalM3=done|status=not-ready|PASS");
