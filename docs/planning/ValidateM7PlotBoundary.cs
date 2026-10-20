#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Dag="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s02-t2.md";
const string Distribution="docs/distribution/m7-plotmodel-deferral.md";
const string PriorOracle="docs/planning/ValidateM7PlotSampling.cs";
const string PriorDecision="docs/planning/decisions/m7-w01-c03-s02-t1.md";
const string PriorScreen="docs/planning/decisions/m7-w01-c03-s02-ownership.md";
const string Parent="M7-W01-C03";
const string S01="M7-W01-C03-S01";
const string Phase="M7-W01-C03-S02";
const string Task="M7-W01-C03-S02-T2";
const string Status="m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s02-t2-plotmodel-deferral/1.2 qualified; triggers=3; renderer=false; s02=done";

static void Need(bool value,string reason)
{
    if(!value)throw new InvalidOperationException("Independent M7 PlotModel deferral: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal)||relative==Native,
        "Only local planning and historical Math evidence may be read.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\'),"Unsafe planning/evidence path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Local(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(relative))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement items,string id)=>
    items.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static string Get(JsonElement value,string property)=>
    value.GetProperty(property).GetString()??
    throw new InvalidOperationException("Missing string property "+property);
static bool Flag(JsonElement value,string field)=>
    value.GetProperty(field).GetBoolean();
static void State(JsonElement value,string expected,string field="status")=>
    Need(Get(value,field)==expected,
        "Unexpected "+field+" for "+Get(value,"id")+"; expected "+expected);
static void Contains(string value,string token,string label)=>
    Need(value.Contains(token,StringComparison.OrdinalIgnoreCase),
        label+" missing term: "+token);

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "The research boundary checker supports only read-only --check.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var splitDoc=Open(Split);
using var dagDoc=Open(Dag);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var split=splitDoc.RootElement;
var dag=dagDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Get(backlog,"plan_version")==(target?"0.1.113":"0.1.112"),
    "Expected exact RS113 baseline or RS114 target backlog version.");
Need(Get(prior,"status")=="m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified" &&
    prior.GetProperty("independent_oracle_groups").GetInt32()==2 &&
    prior.GetProperty("reference_function").GetString()=="1/x" &&
    prior.GetProperty("reference_samples").GetInt32()==5 &&
    prior.GetProperty("reference_finite_points").GetInt32()==4 &&
    prior.GetProperty("reference_segments").GetInt32()==2 &&
    prior.GetProperty("reference_pole_gaps").GetInt32()==1 &&
    prior.GetProperty("reference_breaks").GetInt32()==1 &&
    prior.GetProperty("pole_omitted_breaks").GetInt32()==1 &&
    prior.GetProperty("research_max_samples").GetInt32()==8 &&
    prior.GetProperty("research_max_points").GetInt32()==8 &&
    prior.GetProperty("research_max_segments").GetInt32()==2 &&
    Flag(prior,"pole_not_bridged") &&
    Flag(prior,"nonfinite_sample_rejected") &&
    Flag(prior,"nonfinite_reciprocal_rejected") &&
    Flag(prior,"no_partial_result_on_refusal") &&
    Flag(prior,"t1_done") && !Flag(prior,"t2_done") &&
    !Flag(prior,"s02_phase_done") &&
    !Flag(prior,"parent_c03_done") &&
    !Flag(prior,"public_plot_model_api_delivered") &&
    !Flag(prior,"plot_renderer_delivered") &&
    !Flag(prior,"aura_job_engine_copied"),
    "Historical RS113 reciprocal point/segment research receipt has drifted.");
foreach(var (field,path) in new[]
{
    ("oracle_validator_sha256",PriorOracle),
    ("decision_document_sha256",PriorDecision),
    ("owner_screen_sha256",PriorScreen)
})
    Need(Get(prior,field)==Hash(path),
        "Immutable RS113 reference SHA drift: "+path);
Need(Get(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
    split.GetProperty("native_nodes_after").GetInt32()==541 &&
    Flag(split,"parent_chunk_still_planned") &&
    !Flag(split,"plot_renderer_delivered") &&
    !Flag(split,"dag_runtime_api_delivered"),
    "Size-L split and no-AURA owner boundaries changed.");
Need(Get(dag,"status")=="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified" &&
    Flag(dag,"s01_phase_done") &&
    !Flag(dag,"plot_phase_done") &&
    !Flag(dag,"dag_runtime_api_delivered") &&
    !Flag(dag,"plot_renderer_delivered"),
    "Prior DAG research deferral has been silently promoted.");

State(Find(backlog.GetProperty("releases"),"M6"),"done");
State(Find(native.GetProperty("milestones"),"M6"),"done","state");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var parent=Find(backlog.GetProperty("chunks"),Parent);
var oldPhase=Find(backlog.GetProperty("chunks"),S01);
var productPhase=Find(backlog.GetProperty("chunks"),Phase);
State(parent,"planned");
State(oldPhase,"done");
State(productPhase,target?"done":"in_progress");
Need(Get(parent,"size")=="L"&&Get(productPhase,"size")=="M"&&
     Get(productPhase,"kind")=="spike"&&
     Get(productPhase,"parent_chunk")==Parent,
    "Expected original size-L C03 and bounded S02 research split.");
var t1=Find(productPhase.GetProperty("tasks"),"M7-W01-C03-S02-T1");
var t2=Find(productPhase.GetProperty("tasks"),Task);
State(t1,"done");
State(t2,target?"done":"planned");

var m7=Find(native.GetProperty("milestones"),"M7");
var wp=Find(m7.GetProperty("workPackages"),"M7-W01");
var parentNode=Find(wp.GetProperty("phases"),Parent);
var s01Node=Find(wp.GetProperty("phases"),S01);
var phaseNode=Find(wp.GetProperty("phases"),Phase);
State(m7,"in-progress","state");
State(wp,"in-progress","state");
State(parentNode,"not-ready","state");
State(s01Node,"done","state");
State(phaseNode,target?"done":"in-progress","state");
var nativeT1=Find(phaseNode.GetProperty("tasks"),"M7-W01-C03-S02-T1");
var nativeT2=Find(phaseNode.GetProperty("tasks"),Task);
State(nativeT1,"done","state");
State(nativeT2,target?"done":"not-ready","state");
for(var i=0;i<3;i++)
{
    var ending=((char)('A'+i)).ToString();
    var previous="M7-W01-C03-S02-T1-"+ending;
    var current=Task+"-"+ending;
    State(Find(t1.GetProperty("subtasks"),previous),"done");
    State(Find(nativeT1.GetProperty("subtasks"),previous),"done","state");
    State(Find(t2.GetProperty("subtasks"),current),target?"done":"planned");
    State(Find(nativeT2.GetProperty("subtasks"),current),
        target?"done":"not-ready","state");
}
var decision=Read(Decision);
var distribution=Read(Distribution);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "DEFER","Three mandatory","All three review triggers are necessary",
    "No public PlotModel","AURA or the consuming client",
    "128 bytes","zero partial","M7-W01-C03","source-free"})
    Contains(decision,term,"S02 T2 Q/O/D disposition");
foreach(var term in new[]{"## Historical independent reciprocal witnesses",
    "## Ownership and candidate screen","## Three mandatory review triggers",
    "Consumer and domain","Resource and failure contract",
    "Provider/source-free recipient authority",
    "If any review trigger is missing, remain deferred",
    "128 coordinate bytes","AURA/client-owned","source-free"})
    Contains(distribution,term,"S02 T2 negative ownership gate");
Need(distribution.Split('\n').Count(line=>
    line.Contains("| **Defer** |",StringComparison.Ordinal))==3,
    "Exactly three new PlotModel/sampler/provider options must be explicitly deferred.");

if(target)
{
    using var proofDoc=Open(Evidence);
    var proof=proofDoc.RootElement;
    Need(Get(proof,"status")==Status &&
        Get(proof,"disposition")=="defer-unqualified-plotmodel-and-rendering-provider" &&
        proof.GetProperty("review_trigger_count").GetInt32()==3 &&
        proof.GetProperty("deferred_provider_categories").GetInt32()==3 &&
        proof.GetProperty("prior_reference_finite_points").GetInt32()==4 &&
        proof.GetProperty("prior_reference_segments").GetInt32()==2 &&
        Flag(proof,"t1_done") && Flag(proof,"t2_done") &&
        Flag(proof,"s02_phase_done") &&
        !Flag(proof,"parent_c03_done") &&
        !Flag(proof,"m7_w01_work_package_done") &&
        !Flag(proof,"m7_release_done") &&
        !Flag(proof,"public_plot_model_api_delivered") &&
        !Flag(proof,"plot_renderer_delivered") &&
        !Flag(proof,"adaptive_sampling_delivered") &&
        !Flag(proof,"aura_job_engine_copied") &&
        !Flag(proof,"external_plot_provider_adopted") &&
        !Flag(proof,"automatic_cutover") &&
        !Flag(proof,"publicly_published") &&
        Get(proof,"decision_document_sha256")==Hash(Decision) &&
        Get(proof,"distribution_document_sha256")==Hash(Distribution) &&
        Get(proof,"boundary_validator_sha256")==Hash(
            "docs/planning/ValidateM7PlotBoundary.cs"),
        "Completed S02 target must retain exactly the qualified source/deferral contract.");
}
Console.WriteLine(Marker);
return 0;
