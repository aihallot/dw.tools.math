#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string Dag="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Plot="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C03-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-t2.md";
const string Distribution="docs/distribution/m7-dag-plot-closure.md";
const string PriorOracle="docs/planning/ValidateM7DagPlotAggregate.cs";
const string PriorDecision="docs/planning/decisions/m7-w01-c03-t1.md";
const string PriorScreen="docs/distribution/m7-dag-plot-aggregate-screen.md";
const string Parent="M7-W01-C03";
const string Task="M7-W01-C03-T2";
const string Status="m7-c03-t2-original-aggregate-owner-gated-research-closure-qualified";
const string Marker="dw.tools.math/m7-w01-c03-t2-original-research-closure/1.2 qualified; s01=done; s02=done; c03=done; runtime=false; renderer=false";

static void Need(bool truth,string reason)
{
    if(!truth)throw new InvalidOperationException("Independent M7 original research closure: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) || relative==Native,
        "Read-only checker supports only local Math docs and canonical native plan.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\'),"Unsafe local path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Hash(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement values,string id)=>
    values.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static string Get(JsonElement value,string property)=>
    value.GetProperty(property).GetString()??
        throw new InvalidOperationException("Missing text "+property);
static bool Flag(JsonElement item,string property)=>item.GetProperty(property).GetBoolean();
static void State(JsonElement item,string expected,string property="status")=>
    Need(Get(item,property)==expected,
        "Expected "+expected+" for "+Get(item,"id")+" field "+property);
static void Contains(string body,string term,string label)=>
    Need(body.Contains(term,StringComparison.OrdinalIgnoreCase),
        label+" missing "+term);

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is accepted.");

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var dagDoc=Open(Dag);
using var plotDoc=Open(Plot);
using var splitDoc=Open(Split);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var dag=dagDoc.RootElement;
var plot=plotDoc.RootElement;
var split=splitDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Get(backlog,"plan_version")==(target?"0.1.115":"0.1.114"),
    "Expected exact original C03 RS115 baseline or RS116 target version.");
Need(Get(prior,"status")=="m7-c03-t1-aggregate-pure-dag-and-reciprocal-plot-research-qualified" &&
    prior.GetProperty("prior_receipts_verified").GetInt32()==4 &&
    prior.GetProperty("independent_research_families").GetInt32()==2 &&
    prior.GetProperty("native_nodes_retained").GetInt32()==541 &&
    prior.GetProperty("diamond_root_value").GetInt32()==13 &&
    prior.GetProperty("diamond_shared_evaluations").GetInt32()==1 &&
    prior.GetProperty("reciprocal_finite_points").GetInt32()==4 &&
    prior.GetProperty("reciprocal_segments").GetInt32()==2 &&
    Flag(prior,"known_pole_omitted_break_preserved") &&
    Flag(prior,"children_both_done") &&
    Flag(prior,"t1_done")&&!Flag(prior,"t2_done") &&
    !Flag(prior,"parent_c03_done") &&
    !Flag(prior,"public_dag_api_delivered") &&
    !Flag(prior,"public_plot_model_api_delivered") &&
    !Flag(prior,"renderer_delivered") &&
    !Flag(prior,"aura_job_engine_copied"),
    "Original T1 cross-family evidence must not drift or acquire product claims.");
foreach(var (field,path) in new[]
{
    ("aggregate_validator_sha256",PriorOracle),
    ("decision_sha256",PriorDecision),
    ("distribution_sha256",PriorScreen)
})
    Need(Get(prior,field)==Hash(path),
        "Immutable RS115 cross-family source identity changed: "+path);
Need(Get(dag,"status")=="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified" &&
     Get(dag,"disposition")=="defer-unqualified-pure-dag-runtime-and-providers" &&
     Flag(dag,"s01_phase_done") &&
     !Flag(dag,"dag_runtime_api_delivered") &&
     !Flag(dag,"aura_job_engine_copied"),
     "Prior Math DAG provider must remain explicitly deferred.");
Need(Get(plot,"status")=="m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified" &&
     Get(plot,"disposition")=="defer-unqualified-plotmodel-and-rendering-provider" &&
     Flag(plot,"s02_phase_done") &&
     !Flag(plot,"public_plot_model_api_delivered") &&
     !Flag(plot,"plot_renderer_delivered"),
     "Prior public PlotModel and renderer must remain explicitly deferred.");
Need(Get(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     split.GetProperty("native_nodes_after").GetInt32()==541 &&
     split.GetProperty("new_phase_count").GetInt32()==2 &&
     Flag(split,"parent_chunk_still_planned") &&
     !Flag(split,"plot_renderer_delivered"),
     "The historical size-L split proof cannot be rewritten.");

State(Find(backlog.GetProperty("releases"),"M6"),"done");
State(Find(native.GetProperty("milestones"),"M6"),"done","state");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W02"),"planned");
var product=Find(backlog.GetProperty("chunks"),Parent);
State(product,target?"done":"in_progress");
Need(Get(product,"kind")=="spike"&&Get(product,"size")=="L",
    "Parent C03 stays an accepted size-L research phase.");
State(Find(backlog.GetProperty("chunks"),"M7-W01-C03-S01"),"done");
State(Find(backlog.GetProperty("chunks"),"M7-W01-C03-S02"),"done");
var t1=Find(product.GetProperty("tasks"),"M7-W01-C03-T1");
var t2=Find(product.GetProperty("tasks"),Task);
State(t1,"done");
State(t2,target?"done":"planned");
var milestone=Find(native.GetProperty("milestones"),"M7");
var work=Find(milestone.GetProperty("workPackages"),"M7-W01");
var phase=Find(work.GetProperty("phases"),Parent);
State(milestone,"in-progress","state");
State(work,"in-progress","state");
State(Find(work.GetProperty("phases"),"M7-W01-C03-S01"),"done","state");
State(Find(work.GetProperty("phases"),"M7-W01-C03-S02"),"done","state");
State(phase,target?"done":"in-progress","state");
var nativeT1=Find(phase.GetProperty("tasks"),"M7-W01-C03-T1");
var nativeT2=Find(phase.GetProperty("tasks"),Task);
State(nativeT1,"done","state");
State(nativeT2,target?"done":"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(Find(t1.GetProperty("subtasks"),"M7-W01-C03-T1-"+suffix),"done");
    State(Find(nativeT1.GetProperty("subtasks"),"M7-W01-C03-T1-"+suffix),"done","state");
    State(Find(t2.GetProperty("subtasks"),Task+"-"+suffix),target?"done":"planned");
    State(Find(nativeT2.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"not-ready","state");
}
State(Find(work.GetProperty("phases"),"M7-W01-C01"),"done","state");
State(Find(work.GetProperty("phases"),"M7-W01-C02"),"done","state");

var decision=Read(Decision);
var matrix=Read(Distribution);
foreach(var term in new[]{
    "## Question","## Observation","## Decision",
    "M7-W01-C03-S01","M7-W01-C03-S02",
    "All three review triggers are necessary",
    "no public DAG API","no public PlotModel API",
    "128 coordinate bytes","M7-W01","AURA",
    "remain deferred"
})
    Contains(decision,term,"Final C03 owner-gated Q/O/D decision");
foreach(var term in new[]{
    "## Source-pinned accepted evidence",
    "## Ownership and cost screen",
    "## Three re-entry triggers",
    "If any trigger is missing, remain deferred",
    "AURA/client-owned","source-free",
    "128 coordinate bytes","541","M7-W01"
})
    Contains(matrix,term,"Final C03 distribution gate");
Need(matrix.Split('\n').Count(line=>
    line.Contains("| **Deferred",StringComparison.Ordinal))==2,
    "DAG and PlotModel product APIs must both remain explicitly deferred.");

// Recompute small exact scalar identities without treating the old programs
// as executable Math product libraries.
long shared=2;
var diamond=checked((shared+3)+(shared*4));
double[] samples=[-2,-1,0,1,2];
var left=samples.Where(x=>x<0).Select(x=>1d/x).ToArray();
var right=samples.Where(x=>x>0).Select(x=>1d/x).ToArray();
var omitted=new[]{-2d,-1d,1d,2d};
var omittedCrossings=omitted.Zip(omitted.Skip(1),
    (a,b)=>a<0&&b>0).Count(x=>x);
Need(diamond==13 && left.SequenceEqual(new[]{-0.5d,-1d}) &&
     right.SequenceEqual(new[]{1d,0.5d}) &&
     samples.Count(x=>x==0)==1 && omittedCrossings==1,
     "Independent exact diamond and omitted-pole segment witness drifted.");
Console.WriteLine("M7-C03-CLOSURE-ORACLE:DAG=13;POINTS=4;SEGMENTS=2;KNOWN-POLE-BREAK=TRUE");

if(target)
{
    using var eDoc=Open(Evidence);
    var e=eDoc.RootElement;
    Need(Get(e,"status")==Status &&
        Get(e,"disposition")=="research-complete-products-deferred" &&
        e.GetProperty("prior_aggregate_receipts").GetInt32()==5 &&
        e.GetProperty("native_nodes_retained").GetInt32()==541 &&
        e.GetProperty("research_family_count").GetInt32()==2 &&
        e.GetProperty("review_trigger_count").GetInt32()==3 &&
        Flag(e,"s01_done")&&Flag(e,"s02_done") &&
        Flag(e,"t1_done")&&Flag(e,"t2_done") &&
        Flag(e,"parent_c03_done") &&
        !Flag(e,"m7_w01_work_package_done") &&
        !Flag(e,"m7_release_done") &&
        !Flag(e,"public_dag_api_delivered") &&
        !Flag(e,"public_plot_model_api_delivered") &&
        !Flag(e,"renderer_delivered") &&
        !Flag(e,"aura_job_engine_copied") &&
        !Flag(e,"external_provider_adopted") &&
        !Flag(e,"publicly_published") &&
        Get(e,"decision_sha256")==Hash(Decision) &&
        Get(e,"distribution_sha256")==Hash(Distribution) &&
        Get(e,"closure_validator_sha256")==Hash(
            "docs/planning/ValidateM7DagPlotClosure.cs"),
        "The original C03 target would overclaim delivered product or change pinned source.");
}
Console.WriteLine(Marker);
return 0;
