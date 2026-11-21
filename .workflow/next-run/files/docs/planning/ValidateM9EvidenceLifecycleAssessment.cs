#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
void Require(bool ok, string why) { if (!ok) throw new InvalidDataException("RS276: "+why); }
var assessment=File.ReadAllText("docs/planning/decisions/m9-w01-c01-evidence-lifecycle-assessment.md");
foreach(var marker in new[]{"RS268","RS269","RS270","RS271","RS272","RS273","RS274","RS275",
    "not-ready","planned","Assessa","forward-only","20260.20261120.1001"})
    Require(assessment.Contains(marker,StringComparison.Ordinal),
        "Assessment missing evidence or boundary: "+marker);
foreach(var item in new[]
{
    (Run:"RS268",Message:"feat: implement bounded unbiased SplitMix64 integer draws",Prior:1),
    (Run:"RS269",Message:"test: qualify unpublished bounded Statistics NuGet candidate",Prior:1),
    (Run:"RS270",Message:"test: qualify bounded Statistics candidate net11 binary consumer",Prior:1),
    (Run:"RS271",Message:"test: qualify bounded Statistics private release readiness",Prior:0),
    (Run:"RS272",Message:"test: qualify guarded private bounded Statistics publisher",Prior:1),
    (Run:"RS273",Message:"test: qualify published bounded Statistics private feed readback",Prior:0),
    (Run:"RS274",Message:"docs: deliver published bounded Statistics handoff to Assessa",Prior:1),
    (Run:"RS275",Message:"docs: reconcile Assessa bounded Statistics request with qualified delivery",Prior:0)
})
{
    using var report=JsonDocument.Parse(File.ReadAllText(
        ".aura/workflow/reports/"+item.Run+"/attempt-001.json"));
    var root=report.RootElement;
    var prior=root.GetProperty("priorInvocationFailures");
    Require(root.GetProperty("runId").GetString()==item.Run &&
        root.GetProperty("outcome").GetString()=="succeeded" &&
        root.GetProperty("attempt").GetInt32()==1 &&
        root.GetProperty("git").GetProperty("commitMessage").GetString()==item.Message &&
        prior.GetProperty("count").GetInt32()==item.Prior,
        "Qualified evidence lineage differs: "+item.Run);
}
using(var backlog=JsonDocument.Parse(File.ReadAllText("docs/planning/backlog.json")))
{
    var obj=backlog.RootElement;
    Require(obj.GetProperty("plan_version").GetString()=="0.1.124", "Backlog version differs.");
    Require(obj.GetProperty("releases").EnumerateArray()
        .Single(x=>x.GetProperty("id").GetString()=="M9")
        .GetProperty("status").GetString()=="planned", "M9 release not planned.");
    var chunk=obj.GetProperty("chunks").EnumerateArray()
        .Single(x=>x.GetProperty("id").GetString()=="M9-W01-C01");
    Require(chunk.GetProperty("status").GetString()=="planned" &&
        chunk.GetProperty("tasks").GetArrayLength()==2, "M9 backlog phase not planned.");
}
using(var native=JsonDocument.Parse(File.ReadAllText(".aura/workflow/plan/project.json")))
{
    var m9=native.RootElement.GetProperty("milestones").EnumerateArray()
        .Single(x=>x.GetProperty("id").GetString()=="M9");
    Require(m9.GetProperty("state").GetString()=="not-ready", "M9 native state differs.");
    var wp=m9.GetProperty("workPackages").EnumerateArray().Single();
    var phase=wp.GetProperty("phases").EnumerateArray().Single();
    Require(wp.GetProperty("state").GetString()=="not-ready" &&
        phase.GetProperty("state").GetString()=="not-ready", "M9 hierarchy has drifted.");
    foreach(var task in phase.GetProperty("tasks").EnumerateArray())
    {
        Require(task.GetProperty("state").GetString()=="not-ready", "Unexpected task state.");
        foreach(var sub in task.GetProperty("subtasks").EnumerateArray())
            Require(sub.GetProperty("state").GetString()=="not-ready",
                "Unexpected historical subtask state.");
    }
}
Console.WriteLine("MATH-RS276-M9-LIFECYCLE-ASSESSMENT|qualifiedRuns=8|tasks=2|subtasks=6|native=not-ready|backlog=planned|transition=false|PASS");
