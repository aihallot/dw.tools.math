using System;
using System.IO;
using System.Linq;
using System.Text.Json;

// Read-only M3 science/planning gate. DWF alone owns lifecycle mutation;
// this validator will neither recalculate workflow hashes nor mutate product.
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Provider="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string C04="docs/planning/evidence/M3-W02-C04-boundaries-qualified.json";
var checks=new (string Id,string File,string Status)[]
{
    ("M3-W01-C01","M3-W01-C01-boundary-qualified.json",
        "managed-mathnet-runtime-rid-license-and-native-deferral-qualified"),
    ("M3-W01-C02","M3-W01-C02-boundaries-qualified.json",
        "explicit-correctly-rounded-rational-binary64-and-copy-only-boundaries-qualified"),
    ("M3-W01-C03","M3-W01-C03-boundaries-qualified.json",
        "bounded-2x2-condition-availability-rank-pivot-policy-qualified"),
    ("M3-W02-C01","M3-W02-C01-boundaries-qualified.json",
        "frequency-weight-and-explicit-null-missing-statistics-qualified"),
    ("M3-W02-C02","M3-W02-C02-boundaries-qualified.json",
        "typed-undefined-correlation-and-pair-missing-boundaries-qualified"),
    ("M3-W02-C03","M3-W02-C03-boundaries-qualified.json",
        "finite-continuous-distributions-and-portable-prng-checkpoints-qualified"),
    ("M3-W02-C04","M3-W02-C04-boundaries-qualified.json",
        "recipient-boundary-ownership-and-unsent-requests-qualified")
};
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException("M3 release preflight: "+message);
}
static JsonDocument Read(string relative) =>
    JsonDocument.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar))));
static string Text(JsonElement node,string key) => node.GetProperty(key).GetString() ??
    throw new InvalidOperationException("Null authoritative JSON field: "+key);
static JsonElement ById(JsonElement array,string id) =>
    array.EnumerateArray().Single(x=>Text(x,"id")==id);
static bool Done(JsonElement node)=>Text(node,"status")=="done";
using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
using var previousDoc=Read("docs/planning/evidence/M2-gate.json");
using var providerDoc=Read(Provider);
using var consumerDoc=Read(C04);

var root=backlogDoc.RootElement;
var plan=nativeDoc.RootElement;
var M2=ById(root.GetProperty("releases"),"M2");
var M3=ById(root.GetProperty("releases"),"M3");
var M4=ById(root.GetProperty("releases"),"M4");
var milestone=ById(plan.GetProperty("milestones"),"M3");
var future=ById(plan.GetProperty("milestones"),"M4");
var version=Text(root,"plan_version");
var baseline=version=="0.1.50" && Text(M3,"status")=="in_progress" &&
    Text(milestone,"state")=="in-progress";
var target=version=="0.1.51" && Done(M3) && Text(milestone,"state")=="done";
Require(baseline || target,"M3 must be exact baseline 0.1.50 or completed target 0.1.51.");
Require(Done(M2) && Text(previousDoc.RootElement,"status")==
    "local-m2-typed-ir-composition-and-isolated-consumer-qualified",
    "Previous release M2 must retain independent accepted gate evidence.");
Require(Text(M3,"gate")=="G-M3" && Text(M3,"version")=="0.4.0-preview" &&
    Text(M4,"status")=="planned" && Text(future,"state")=="not-ready",
    "Numerical preview only; future symbolic milestone must stay unopened.");

var accepted=M3.GetProperty("required_chunks").EnumerateArray()
    .Select(x=>x.GetString()??"").ToArray();
Require(accepted.SequenceEqual(checks.Select(x=>x.Id),StringComparer.Ordinal),
    "Canonical release acceptance must list exactly seven approved M3 phases.");

var packages=new[]{"M3-W01","M3-W02"};
var countedTasks=0;
var countedChildren=0;
foreach(var work in packages)
{
    var roadmap=ById(root.GetProperty("work_packages"),work);
    var executable=ById(milestone.GetProperty("workPackages"),work);
    Require(Done(roadmap) && Text(executable,"state")=="done",
        "Work package is not durably closed: "+work);
    Require(roadmap.TryGetProperty("evidence",out var e) &&
        e.ValueKind==JsonValueKind.Array && e.GetArrayLength()>0,
        "A closed M3 work package requires concrete evidence: "+work);
}
foreach(var (id,file,status) in checks)
{
    var chunk=ById(root.GetProperty("chunks"),id);
    var targetWork=id.StartsWith("M3-W01-",StringComparison.Ordinal)?"M3-W01":"M3-W02";
    var phase=ById(ById(milestone.GetProperty("workPackages"),targetWork)
        .GetProperty("phases"),id);
    Require(Done(chunk) && Text(phase,"state")=="done",
        "Accepted Math phase incomplete in product or native DWF: "+id);
    var evidence="docs/planning/evidence/"+file;
    using var proof=Read(evidence);
    Require(Text(proof.RootElement,"status")==status,
        "Scientific phase evidence status not independently admitted: "+id);
    Require(chunk.GetProperty("evidence").EnumerateArray().Any(x=>x.GetString()==evidence),
        "A required scientific phase proof is absent from the canonical chunk: "+id);
    var roadmapTasks=chunk.GetProperty("tasks").EnumerateArray().ToArray();
    var nativeTasks=phase.GetProperty("tasks").EnumerateArray().ToArray();
    Require(roadmapTasks.Length==2 && nativeTasks.Length==2,
        "Every accepted M3 phase must contain its two original tasks: "+id);
    foreach(var task in roadmapTasks)
    {
        var taskId=Text(task,"id");
        var nativeTask=ById(phase.GetProperty("tasks"),taskId);
        Require(Done(task) && Text(nativeTask,"state")=="done",
            "Accepted product/native task incomplete: "+taskId);
        var subtasks=task.GetProperty("subtasks").EnumerateArray().ToArray();
        var nativeSubtasks=nativeTask.GetProperty("subtasks").EnumerateArray().ToArray();
        Require(subtasks.Length==3 && nativeSubtasks.Length==3,
            "Every original M3 task must have three accepted children: "+taskId);
        foreach(var child in subtasks)
        {
            var childId=Text(child,"id");
            var nativeChild=ById(nativeTask.GetProperty("subtasks"),childId);
            Require(Done(child) && Text(nativeChild,"state")=="done",
                "Accepted product/native subtask incomplete: "+childId);
            countedChildren++;
        }
        countedTasks++;
    }
}
Require(countedTasks==14 && countedChildren==42,
    "The M3 gate must cover all fourteen tasks and forty-two subtasks.");

var provider=providerDoc.RootElement;
Require(Text(provider,"selected_package_id")=="MathNet.Numerics" &&
    Text(provider,"selected_package_version")=="5.0.0" &&
    provider.GetProperty("smoke_managed_provider_selection_passed").GetBoolean(),
    "The historical managed Math.NET provider qualification must remain pinned.");
var consumer=consumerDoc.RootElement;
Require(Text(consumer,"status")=="recipient-boundary-ownership-and-unsent-requests-qualified" &&
    consumer.GetProperty("owner_boundary_rows").GetInt32()==8 &&
    !consumer.GetProperty("external_adoption_observed").GetBoolean() &&
    !consumer.GetProperty("requests_transmitted").GetBoolean(),
    "A Math-local consumer must not become invented Decision/MCDM adoption.");
Console.WriteLine(
    "dw.tools.math/m3-scientific-gate/0.4 qualified; phases=7; tasks=14; subtasks=42");
