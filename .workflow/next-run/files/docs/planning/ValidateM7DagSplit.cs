#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-split.md";
const string Validator="docs/planning/ValidateNativeDwfAdoption.cs";
const string M6="docs/planning/evidence/M6-gate.json";
const string Parent="M7-W01-C03";
const string DAG="M7-W01-C03-S01";
const string Plot="M7-W01-C03-S02";
const string Marker="dw.tools.math/m7-w01-c03-independent-split/1.2 qualified; phases=2; tasks=4; subtasks=12; nodes=541; renderer=false";

static void Need(bool ok,string reason)
{
    if(!ok)throw new InvalidOperationException("Independent M7 size-L split: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) || relative==Native,
        "Only local read-only planning/evidence paths are supported.");
    Need(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal) &&
        !relative.Contains('\\'),"Invalid relative path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static JsonDocument Read(string relative)=>JsonDocument.Parse(File.ReadAllText(Local(relative)));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(relative))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement a,string id)=>
    a.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static bool Has(JsonElement a,string id)=>
    a.EnumerateArray().Any(x=>x.GetProperty("id").GetString()==id);
static string Get(JsonElement n,string key)=>n.GetProperty(key).GetString() ??
    throw new InvalidOperationException("Missing text field "+key);
static void RequireState(JsonElement n,string expected,string key="status")=>
    Need(Get(n,key)==expected,Get(n,"id")+" incorrect "+key+" expected "+expected);
static string[] Strings(JsonElement a)=>
    a.EnumerateArray().Select(x=>x.GetString()??"").ToArray();
static int Count(JsonElement node)
{
    var count=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
    {
        if(node.TryGetProperty(key,out var children))
            count+=children.EnumerateArray().Sum(Count);
    }
    return count;
}
Need(args.Length==0 || (args.Length==1 && args[0]=="--check"),
    "The independent split checker is read-only; only --check is admitted.");
using var bDoc=Read(Backlog);
using var pDoc=Read(Native);
using var priorDoc=Read(Prior);
using var m6Doc=Read(M6);
var b=bDoc.RootElement;
var p=pDoc.RootElement;
var prior=priorDoc.RootElement;
var m6=m6Doc.RootElement;
var target=File.Exists(Local(Split));
Need(Get(m6,"status")=="m6-independent-local-analysis-scientific-release-gate-qualified-unpublished" &&
     m6.GetProperty("m6_milestone_done").GetBoolean() &&
     !m6.GetProperty("packages_publicly_published").GetBoolean(),
     "M6 predecessor must remain a local unpublished qualified milestone.");
Need(Get(prior,"status")=="m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified" &&
     prior.GetProperty("m7_w01_c02_parent_done").GetBoolean() &&
     !prior.GetProperty("m7_w01_work_package_done").GetBoolean() &&
     !prior.GetProperty("advanced_tensor_api_delivered").GetBoolean(),
     "RS109 predecessor must remain a deferred tensor research spike.");
Need(Get(b,"plan_version")==(target?"0.1.109":"0.1.108"),
    "Unexpected accepted split baseline/target version.");
var r=Find(b.GetProperty("releases"),"M7");
var wp=Find(b.GetProperty("work_packages"),"M7-W01");
var original=Find(b.GetProperty("chunks"),Parent);
var m7=Find(p.GetProperty("milestones"),"M7");
var nativeWP=Find(m7.GetProperty("workPackages"),"M7-W01");
var nativeOriginal=Find(nativeWP.GetProperty("phases"),Parent);
RequireState(r,"in_progress");
RequireState(wp,"in_progress");
RequireState(original,"planned");
RequireState(m7,"in-progress","state");
RequireState(nativeWP,"in-progress","state");
RequireState(nativeOriginal,"not-ready","state");
Need(Get(original,"size")=="L"&&Get(original,"kind")=="spike",
    "The original accepted size-L phase must remain an aggregate research spike.");
var chunks=b.GetProperty("chunks");
var phases=nativeWP.GetProperty("phases");
var gate=Strings(r.GetProperty("required_chunks"));
Need(gate.Contains(Parent,StringComparer.Ordinal), "The original C03 gate is not optional.");
if(!target)
{
    Need(!Has(chunks,DAG)&&!Has(chunks,Plot) &&
         !Has(phases,DAG)&&!Has(phases,Plot) &&
         gate.Length==5&&p.GetProperty("milestones").EnumerateArray().Sum(Count)==523,
         "Unmodified RS109 baseline must not already contain a partial split.");
    Console.WriteLine("M7-DAG-SPLIT:BASELINE:UNSPLIT=TRUE");
    return 0;
}
Need(gate.Length==7&&gate.Contains(DAG,StringComparer.Ordinal)&&
     gate.Contains(Plot,StringComparer.Ordinal),
     "Both split phases and parent must remain required M7 release chunks.");
var dag=Find(chunks,DAG);
var plot=Find(chunks,Plot);
var nativeDag=Find(phases,DAG);
var nativePlot=Find(phases,Plot);
foreach(var (chunk,plan,id,productState,planState,dependency) in new[]
{
    (dag,nativeDag,DAG,"in_progress","in-progress","M7-W01-C02"),
    (plot,nativePlot,Plot,"planned","not-ready",DAG)
})
{
    Need(Get(chunk,"id")==id&&Get(chunk,"parent_chunk")==Parent&&
         Get(chunk,"kind")=="spike"&&Get(chunk,"size")=="M"&&
         Get(chunk,"work_package")=="M7-W01",
         "Split child identities, parents or finite sizes changed: "+id);
    RequireState(chunk,productState);
    RequireState(plan,planState,"state");
    Need(Strings(chunk.GetProperty("depends_on")).SequenceEqual(new[]{dependency}) &&
         Strings(plan.GetProperty("dependsOn")).SequenceEqual(new[]{dependency}),
         "Split DWF/product dependencies differ: "+id);
    Need(chunk.GetProperty("tasks").GetArrayLength()==2&&
         plan.GetProperty("tasks").GetArrayLength()==2,
         "Each split research phase must have exactly two explicit T1/T2 tasks.");
    foreach(var j in new[]{1,2})
    {
        var task=id+"-T"+j;
        var bTask=Find(chunk.GetProperty("tasks"),task);
        var pTask=Find(plan.GetProperty("tasks"),task);
        RequireState(bTask,"planned");
        RequireState(pTask,"not-ready","state");
        Need(bTask.GetProperty("subtasks").GetArrayLength()==3&&
             pTask.GetProperty("subtasks").GetArrayLength()==3,
             "Each research task must retain exact Q/O/D traceability: "+task);
        foreach(var s in new[]{"A","B","C"})
        {
            RequireState(Find(bTask.GetProperty("subtasks"),task+"-"+s),"planned");
            RequireState(Find(pTask.GetProperty("subtasks"),task+"-"+s),
                "not-ready","state");
        }
    }
}
Need(p.GetProperty("milestones").EnumerateArray().Sum(Count)==541,
    "Full native DWF topology must gain exactly 18 nodes to total 541.");
Need(File.ReadAllText(Local(Validator)).Contains("Require(expected.Count == 541,",
    StringComparison.Ordinal),"Native mapping validator was not upgraded to 541 nodes.");
var splitDoc=File.ReadAllText(Local(Decision));
foreach(var token in new[]{"M7-W01-C03-S01","M7-W01-C03-S02","diamond DAG","1/x",
    "AURA","before readiness","523","541","no new"})
    Need(splitDoc.Contains(token,StringComparison.OrdinalIgnoreCase),
        "Split decision lacks a required scientific/ownership boundary: "+token);
using var eDoc=Read(Split);
var e=eDoc.RootElement;
Need(Get(e,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     Get(e,"split_parent")==Parent &&
     e.GetProperty("new_phase_count").GetInt32()==2 &&
     e.GetProperty("new_task_count").GetInt32()==4 &&
     e.GetProperty("new_subtask_count").GetInt32()==12 &&
     e.GetProperty("native_nodes_after").GetInt32()==541 &&
     e.GetProperty("parent_chunk_still_planned").GetBoolean() &&
     e.GetProperty("dag_split_active").GetBoolean() &&
     !e.GetProperty("plot_split_active").GetBoolean() &&
     !e.GetProperty("dag_runtime_api_delivered").GetBoolean() &&
     !e.GetProperty("plot_renderer_delivered").GetBoolean() &&
     !e.GetProperty("m7_release_done").GetBoolean() &&
     Get(e,"decision_sha256")==Hash(Decision) &&
     Get(e,"native_validator_sha256")==Hash(Validator),
     "Split evidence cannot certify an unsplit or product-delivered target.");
Console.WriteLine(Marker);
