#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string DagT1="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string DagT2="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string PlotT1="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string PlotT2="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-t1.md";
const string Distribution="docs/distribution/m7-dag-plot-aggregate-screen.md";
const string Evidence="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string Parent="M7-W01-C03";
const string S01="M7-W01-C03-S01";
const string S02="M7-W01-C03-S02";
const string Task="M7-W01-C03-T1";
const string Status="m7-c03-t1-aggregate-pure-dag-and-reciprocal-plot-research-qualified";
const string Marker="dw.tools.math/m7-w01-c03-t1-independent-aggregate/1.2 qualified; dag-root=13; reciprocal-points=4; segments=2; children=done; product=false";

static void Need(bool success,string reason)
{
    if(!success) throw new InvalidOperationException("Independent M7 C03 aggregate: "+reason);
}
static string Local(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||path==Native,
        "Only Math docs/ and the native DWF plan may be read.");
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\'),"Unsafe local path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static JsonDocument Open(string path)=>JsonDocument.Parse(File.ReadAllText(Local(path)));
static string Read(string path)=>File.ReadAllText(Local(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static JsonElement ById(JsonElement array,string id)=>
    array.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static string Text(JsonElement value,string key)=>
    value.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing text "+key);
static bool Flag(JsonElement value,string key)=>value.GetProperty(key).GetBoolean();
static void State(JsonElement value,string expected,string key="status")=>
    Need(Text(value,key)==expected,
        "Wrong "+key+" for "+Text(value,"id")+"; expected "+expected);
static void Pins(JsonElement receipt,params (string Field,string Path)[] identities)
{
    foreach(var (field,path) in identities)
        Need(Text(receipt,field)==Sha(path),
            "Historical source SHA-256 identity drifted: "+path);
}
static void Contains(string source,string token,string label)=>
    Need(source.Contains(token,StringComparison.OrdinalIgnoreCase),
        label+" missing: "+token);

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var splitDoc=Open(Split);
using var dagT1Doc=Open(DagT1);
using var dagT2Doc=Open(DagT2);
using var plotT1Doc=Open(PlotT1);
using var plotT2Doc=Open(PlotT2);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var split=splitDoc.RootElement;
var dagT1=dagT1Doc.RootElement;
var dagT2=dagT2Doc.RootElement;
var plotT1=plotT1Doc.RootElement;
var plotT2=plotT2Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")==(target?"0.1.114":"0.1.113"),
    "Wrong RS114/RS115 exact baseline or target plan version.");
Need(Text(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     split.GetProperty("native_nodes_after").GetInt32()==541 &&
     split.GetProperty("new_phase_count").GetInt32()==2 &&
     Flag(split,"parent_chunk_still_planned") &&
     !Flag(split,"dag_runtime_api_delivered") &&
     !Flag(split,"plot_renderer_delivered"),
    "The size-L split must retain its original research-only identity.");
Need(Text(dagT1,"status")=="m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified" &&
     dagT1.GetProperty("reference_root_value").GetInt32()==13 &&
     dagT1.GetProperty("reference_node_count").GetInt32()==4 &&
     dagT1.GetProperty("reference_edge_count").GetInt32()==4 &&
     dagT1.GetProperty("reference_shared_evaluations").GetInt32()==1 &&
     dagT1.GetProperty("research_max_nodes").GetInt32()==8 &&
     dagT1.GetProperty("research_max_edges").GetInt32()==10 &&
     dagT1.GetProperty("research_max_depth").GetInt32()==4 &&
     Flag(dagT1,"cycle_refused")&&Flag(dagT1,"no_partial_result_on_refusal") &&
     !Flag(dagT1,"dag_runtime_api_delivered"),
    "The pure diamond-DAG T1 reference or refusal limits drifted.");
Need(Text(dagT2,"status")=="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified" &&
     Text(dagT2,"disposition")=="defer-unqualified-pure-dag-runtime-and-providers" &&
     Flag(dagT2,"s01_phase_done") &&
     !Flag(dagT2,"parent_c03_done") &&
     !Flag(dagT2,"dag_runtime_api_delivered") &&
     !Flag(dagT2,"aura_job_engine_copied"),
    "S01 T2 must remain a completed pure DAG provider deferral.");
Need(Text(plotT1,"status")=="m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified" &&
     plotT1.GetProperty("reference_function").GetString()=="1/x" &&
     plotT1.GetProperty("reference_samples").GetInt32()==5 &&
     plotT1.GetProperty("reference_finite_points").GetInt32()==4 &&
     plotT1.GetProperty("reference_segments").GetInt32()==2 &&
     plotT1.GetProperty("reference_pole_gaps").GetInt32()==1 &&
     plotT1.GetProperty("reference_breaks").GetInt32()==1 &&
     plotT1.GetProperty("pole_omitted_breaks").GetInt32()==1 &&
     plotT1.GetProperty("research_max_samples").GetInt32()==8 &&
     plotT1.GetProperty("research_max_points").GetInt32()==8 &&
     plotT1.GetProperty("research_max_segments").GetInt32()==2 &&
     Flag(plotT1,"pole_not_bridged") &&
     Flag(plotT1,"no_partial_result_on_refusal") &&
     !Flag(plotT1,"public_plot_model_api_delivered"),
    "The reciprocal S02 T1 finite topology and refusal corpus drifted.");
Need(Text(plotT2,"status")=="m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified" &&
     Text(plotT2,"disposition")=="defer-unqualified-plotmodel-and-rendering-provider" &&
     Flag(plotT2,"s02_phase_done") &&
     !Flag(plotT2,"parent_c03_done") &&
     !Flag(plotT2,"public_plot_model_api_delivered") &&
     !Flag(plotT2,"plot_renderer_delivered") &&
     !Flag(plotT2,"aura_job_engine_copied"),
    "S02 T2 must remain an owner-gated public PlotModel/renderer deferral.");

Pins(dagT1,
    ("oracle_validator_sha256","docs/planning/ValidateM7DagAdmission.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s01-t1.md"),
    ("owner_screen_sha256","docs/planning/decisions/m7-w01-c03-s01-owner-screen.md"));
Pins(dagT2,
    ("boundary_validator_sha256","docs/planning/ValidateM7DagBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s01-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-pure-dag-deferral.md"));
Pins(plotT1,
    ("oracle_validator_sha256","docs/planning/ValidateM7PlotSampling.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s02-t1.md"),
    ("owner_screen_sha256","docs/planning/decisions/m7-w01-c03-s02-ownership.md"));
Pins(plotT2,
    ("boundary_validator_sha256","docs/planning/ValidateM7PlotBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s02-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-plotmodel-deferral.md"));

State(ById(backlog.GetProperty("releases"),"M6"),"done");
State(ById(native.GetProperty("milestones"),"M6"),"done","state");
State(ById(backlog.GetProperty("releases"),"M7"),"in_progress");
State(ById(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var parent=ById(backlog.GetProperty("chunks"),Parent);
var s01=ById(backlog.GetProperty("chunks"),S01);
var s02=ById(backlog.GetProperty("chunks"),S02);
State(s01,"done");
State(s02,"done");
State(parent,target?"in_progress":"planned");
Need(Text(parent,"kind")=="spike"&&Text(parent,"size")=="L",
    "Original C03 must remain the size-L research aggregate.");
var t1=ById(parent.GetProperty("tasks"),Task);
var t2=ById(parent.GetProperty("tasks"),"M7-W01-C03-T2");
State(t1,target?"done":"planned");
State(t2,"planned");

var m7=ById(native.GetProperty("milestones"),"M7");
var w01=ById(m7.GetProperty("workPackages"),"M7-W01");
var phase=ById(w01.GetProperty("phases"),Parent);
State(m7,"in-progress","state");
State(w01,"in-progress","state");
State(ById(w01.GetProperty("phases"),S01),"done","state");
State(ById(w01.GetProperty("phases"),S02),"done","state");
State(phase,target?"in-progress":"not-ready","state");
var nativeT1=ById(phase.GetProperty("tasks"),Task);
var nativeT2=ById(phase.GetProperty("tasks"),"M7-W01-C03-T2");
State(nativeT1,target?"done":"not-ready","state");
State(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(ById(t1.GetProperty("subtasks"),Task+"-"+suffix),target?"done":"planned");
    State(ById(nativeT1.GetProperty("subtasks"),Task+"-"+suffix),target?"done":"not-ready","state");
    State(ById(t2.GetProperty("subtasks"),"M7-W01-C03-T2-"+suffix),"planned");
    State(ById(nativeT2.GetProperty("subtasks"),"M7-W01-C03-T2-"+suffix),"not-ready","state");
}
var decision=Read(Decision);
var screen=Read(Distribution);
foreach(var token in new[]{"## Question","## Observation","## Accepted aggregate T1 decision",
    "M7-W01-C03-S01","M7-W01-C03-S02","root = (2 + 3) + (2 * 4) = 13",
    "two disconnected segments","No public DAG API","no public PlotModel API",
    "## T2 integration and admission gate","541 native nodes"})
    Contains(decision,token,"C03 aggregate research Q/O/D");
foreach(var token in new[]{"## Immutable completed research chain",
    "## Falsifiable aggregate reference and finite budgets",
    "## Ownership and negative gate","## Remaining original-parent task",
    "root=13","128","source-free","two segments","541 nodes"})
    Contains(screen,token,"C03 aggregate ownership cross-family matrix");

// Independent hand-checkable mathematical witnesses, separate from the earlier
// two BCL implementations. Their SHA-pinned receipts attest broader refusal cases.
var sharedEvaluations=0;
long Shared() {sharedEvaluations++;return 2;}
var shared=Shared();
var left=checked(shared+3);
var right=checked(shared*4);
var root=checked(left+right);
Need(root==13&&left==5&&right==8&&sharedEvaluations==1,
    "Independent pure diamond cross-family witness drifted.");
Console.WriteLine("M7-AGGREGATE-ORACLE:DIAMOND:ROOT=13:SHARED=1:NODES=4:EDGES=4");

var xs=new[]{-2d,-1d,0d,1d,2d};
var finite=xs.Where(x=>x!=0d).Select(x=>(X:x,Y:1d/x)).ToArray();
var negative=finite.Where(x=>x.X<0).ToArray();
var positive=finite.Where(x=>x.X>0).ToArray();
Need(finite.Length==4&&negative.Length==2&&positive.Length==2&&
    finite.All(x=>double.IsFinite(x.Y))&&
    negative[0]==(-2d,-0.5d)&&negative[1]==(-1d,-1d)&&
    positive[0]==(1d,1d)&&positive[1]==(2d,0.5d)&&
    xs.Count(x=>x==0d)==1,
    "Independent reciprocal known-pole segment topology drifted.");
var skipped=new[]{-2d,-1d,1d,2d};
var signedTransition=skipped.Zip(skipped.Skip(1),(a,b)=>a<0&&b>0).Count(x=>x);
Need(signedTransition==1&&negative[^1].X<0&&positive[0].X>0,
    "Omitted zero must not bridge the known reciprocal discontinuity.");
Console.WriteLine("M7-AGGREGATE-ORACLE:RECIPROCAL:POINTS=4:SEGMENTS=2:POLE-GAP=1:OMITTED-BREAK=1");

if(target)
{
    using var eDoc=Open(Evidence);
    var e=eDoc.RootElement;
    Need(Text(e,"status")==Status &&
        e.GetProperty("independent_research_families").GetInt32()==2 &&
        e.GetProperty("prior_receipts_verified").GetInt32()==4 &&
        e.GetProperty("native_nodes_retained").GetInt32()==541 &&
        e.GetProperty("diamond_root_value").GetInt32()==13 &&
        e.GetProperty("diamond_shared_evaluations").GetInt32()==1 &&
        e.GetProperty("reciprocal_finite_points").GetInt32()==4 &&
        e.GetProperty("reciprocal_segments").GetInt32()==2 &&
        Flag(e,"known_pole_omitted_break_preserved") &&
        Flag(e,"children_both_done") &&
        Flag(e,"t1_done") && !Flag(e,"t2_done") &&
        !Flag(e,"parent_c03_done") &&
        !Flag(e,"m7_w01_work_package_done") &&
        !Flag(e,"m7_release_done") &&
        !Flag(e,"public_dag_api_delivered") &&
        !Flag(e,"public_plot_model_api_delivered") &&
        !Flag(e,"renderer_delivered") &&
        !Flag(e,"aura_job_engine_copied") &&
        Text(e,"aggregate_validator_sha256")==Sha(
            "docs/planning/ValidateM7DagPlotAggregate.cs") &&
        Text(e,"decision_sha256")==Sha(Decision) &&
        Text(e,"distribution_sha256")==Sha(Distribution),
        "C03 aggregate target receipt violates prior evidence/source or no-delivery constraints.");
}
Console.WriteLine(Marker);
return 0;
