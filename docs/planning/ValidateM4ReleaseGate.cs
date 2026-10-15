using System;
using System.IO;
using System.Linq;
using System.Text.Json;

// Read-only independent M4 science/product/native gate check. DWF owns lifecycle.
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string EvidenceRoot="docs/planning/evidence/";
const string Marker="dw.tools.math/m4-scientific-gate/0.5 qualified; phases=6; tasks=12; subtasks=36";

var phaseProofs=new (string Id,string File,string Status)[]
{
    ("M4-W01-C01","M4-W01-C01-T2-boundaries-qualified.json",
        "m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified"),
    ("M4-W01-C02","M4-W01-C02-T2-boundaries-qualified.json",
        "strict-typed-symbolic-provider-output-and-condition-admission-qualified"),
    ("M4-W01-C03","M4-W01-C03-T2-boundaries-qualified.json",
        "bounded-symbolic-provenance-cancellation-and-soft-deadline-qualified"),
    ("M4-W02-C01","M4-W02-C01-T2-boundaries-qualified.json",
        "real-exact-pointwise-derivative-and-domain-refusal-qualified"),
    ("M4-W02-C02","M4-W02-C02-T2-boundaries-qualified.json",
        "original-real-equation-root-and-multiplicity-boundaries-qualified"),
    ("M4-W02-C03","M4-W02-C03-T2-boundaries-qualified.json",
        "real-calculus-original-domain-pole-hole-and-unknown-convergence-qualified")
};
static void Require(bool okay,string why)
{
    if(!okay)throw new InvalidOperationException("M4 scientific release gate: "+why);
}
static JsonDocument Read(string relative)=>
    JsonDocument.Parse(File.ReadAllText(
        Path.Combine(Directory.GetCurrentDirectory(),
            relative.Replace('/',Path.DirectorySeparatorChar))));
static string Text(JsonElement x,string property)=>
    x.GetProperty(property).GetString() ??
    throw new InvalidOperationException("Null JSON authority: "+property);
static bool Done(JsonElement element,string key="status")=>
    Text(element,key)=="done";
static JsonElement ById(JsonElement array,string id)=>
    array.EnumerateArray().Single(x=>Text(x,"id")==id);
static string[] Strings(JsonElement array)=>
    array.EnumerateArray().Select(x=>x.GetString()??"").ToArray();

using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
using var priorM2Doc=Read(EvidenceRoot+"M2-gate.json");
using var priorM3Doc=Read(EvidenceRoot+"M3-gate.json");
using var providerDoc=Read(EvidenceRoot+"M4-W01-C01-T1-qualified.json");
using var boundaryDoc=Read(EvidenceRoot+"M4-W01-C01-T2-boundaries-qualified.json");
using var aggregateDoc=Read(EvidenceRoot+"M4-W02-work-package-qualified.json");
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var M2=ById(backlog.GetProperty("releases"),"M2");
var M3=ById(backlog.GetProperty("releases"),"M3");
var M4=ById(backlog.GetProperty("releases"),"M4");
var M5=ById(backlog.GetProperty("releases"),"M5");
var m4=ById(native.GetProperty("milestones"),"M4");
var m5=ById(native.GetProperty("milestones"),"M5");
var baseline=Text(backlog,"plan_version")=="0.1.63" &&
    Text(M4,"status")=="in_progress" && Text(m4,"state")=="in-progress";
var target=Text(backlog,"plan_version")=="0.1.64" &&
    Done(M4) && Text(m4,"state")=="done";
Require(baseline||target,
    "Milestone must match exact M4 0.1.63 baseline or 0.1.64 target.");
Require(Done(M2)&&Done(M3)&&
    Text(priorM2Doc.RootElement,"status")==
        "local-m2-typed-ir-composition-and-isolated-consumer-qualified"&&
    Text(priorM3Doc.RootElement,"status")==
        "m3-local-numerics-and-statistics-preview-gate-qualified",
    "Earlier typed IR and numerical release gates are not retained.");
Require(Text(M4,"gate")=="G-M4"&&Text(M4,"version")=="0.5.0-preview"&&
    Text(M5,"status")=="planned"&&Text(m5,"state")=="not-ready",
    "Release gate cannot promote or change the future M5 milestone.");
var required=Strings(M4.GetProperty("required_chunks"));
Require(required.SequenceEqual(phaseProofs.Select(x=>x.Id),StringComparer.Ordinal),
    "Exactly six accepted M4 phases must be covered in roadmap order.");
Require(M4.GetProperty("depends_on").EnumerateArray().Any(x=>x.GetString()=="M2"),
    "The accepted M2 prerequisite must not be erased.");
var countedTasks=0;
var countedChildren=0;
foreach(var workId in new[]{"M4-W01","M4-W02"})
{
    var product=ById(backlog.GetProperty("work_packages"),workId);
    var execution=ById(m4.GetProperty("workPackages"),workId);
    Require(Done(product)&&Text(execution,"state")=="done",
        "Work package has not been durably completed: "+workId);
    Require(product.GetProperty("evidence").GetArrayLength()>0,
        "Completed work package lacks durable evidence: "+workId);
}
foreach(var (id,file,status) in phaseProofs)
{
    var workId=id.StartsWith("M4-W01-",StringComparison.Ordinal)
        ?"M4-W01":"M4-W02";
    var chunk=ById(backlog.GetProperty("chunks"),id);
    var wp=ById(m4.GetProperty("workPackages"),workId);
    var phase=ById(wp.GetProperty("phases"),id);
    Require(Done(chunk)&&Text(phase,"state")=="done",
        "Qualified product/native phase state mismatch: "+id);
    var evidencePath=EvidenceRoot+file;
    using var proof=Read(evidencePath);
    Require(Text(proof.RootElement,"status")==status,
        "Final scientific phase receipt does not carry accepted status: "+id);
    Require(Strings(chunk.GetProperty("evidence")).Contains(evidencePath,StringComparer.Ordinal),
        "Canonical chunk lacks accepted scientific evidence: "+id);
    var tasks=chunk.GetProperty("tasks").EnumerateArray().ToArray();
    var nativeTasks=phase.GetProperty("tasks").EnumerateArray().ToArray();
    Require(tasks.Length==2&&nativeTasks.Length==2,
        "A qualified phase must contain its two accepted tasks: "+id);
    foreach(var task in tasks)
    {
        var taskId=Text(task,"id");
        var matching=ById(phase.GetProperty("tasks"),taskId);
        Require(Done(task)&&Text(matching,"state")=="done",
            "Task state is not completed in both authorities: "+taskId);
        var children=task.GetProperty("subtasks").EnumerateArray().ToArray();
        var nativeChildren=matching.GetProperty("subtasks").EnumerateArray().ToArray();
        Require(children.Length==3&&nativeChildren.Length==3,
            "Every M4 task must retain its three accepted child identities: "+taskId);
        foreach(var child in children)
        {
            var childId=Text(child,"id");
            var nativeChild=ById(matching.GetProperty("subtasks"),childId);
            Require(Done(child)&&Text(nativeChild,"state")=="done",
                "Subtask is not durably completed: "+childId);
            countedChildren++;
        }
        countedTasks++;
    }
}
Require(countedTasks==12&&countedChildren==36,
    "Exactly 12 accepted M4 tasks and 36 subtasks must be complete.");
var provider=providerDoc.RootElement;
Require(Text(provider,"status")==
    "m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified",
    "The experimental symbolic provider decision must remain evidence-backed.");
var boundaries=boundaryDoc.RootElement;
Require(Text(boundaries,"status")==
    "m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified"&&
    !boundaries.GetProperty("public_symbolic_adapter_delivered").GetBoolean(),
    "An experimental CAS host is not a released Math provider adapter.");
var aggregate=aggregateDoc.RootElement;
Require(Text(aggregate,"status")=="exact-symbolic-m4-w02-work-package-qualified"&&
    aggregate.GetProperty("m4_w02_work_package_done").GetBoolean()&&
    !aggregate.GetProperty("m4_release_done").GetBoolean()&&
    Text(aggregate,"last_symbolics_version")=="0.5.0-preview.10"&&
    aggregate.GetProperty("qualified_chains").GetArrayLength()==6,
    "M4-W02 aggregate scientific chain is not fully qualified.");
Require(File.ReadAllText(
    Path.Combine(Directory.GetCurrentDirectory(),
        "src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj"))
    .Contains("<Version>0.5.0-preview.10</Version>",StringComparison.Ordinal),
    "The local symbolic product version must remain pinned.");
if(target)
{
    var requiredGate=EvidenceRoot+"M4-gate.json";
    using var gateDoc=Read(requiredGate);
    Require(Text(gateDoc.RootElement,"status")==
        "m4-local-exact-symbolic-preview-gate-qualified"&&
        Strings(M4.GetProperty("evidence")).Contains(requiredGate,StringComparer.Ordinal),
        "A completed M4 requires the matching local release gate receipt.");
}
Console.WriteLine(Marker);
