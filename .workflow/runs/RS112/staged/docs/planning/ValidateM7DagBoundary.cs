#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s01-t2.md";
const string Distribution="docs/distribution/m7-pure-dag-deferral.md";
const string PriorOracle="docs/planning/ValidateM7DagAdmission.cs";
const string PriorDecision="docs/planning/decisions/m7-w01-c03-s01-t1.md";
const string PriorScreen="docs/planning/decisions/m7-w01-c03-s01-owner-screen.md";
const string Phase="M7-W01-C03-S01";
const string Task="M7-W01-C03-S01-T2";
const string Parent="M7-W01-C03";
const string Plot="M7-W01-C03-S02";
const string Status="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s01-t2-pure-dag-deferral/1.2 qualified; triggers=3; runtime=false; aura-scheduler=false; s01=done";

static void Need(bool condition,string reason)
{
    if(!condition)throw new InvalidOperationException("Independent M7 pure DAG boundary: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) || relative==Native,
        "Only bounded local Math planning and evidence can be inspected.");
    Need(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal) &&
         !relative.Contains('\\'),"Unsafe evidence path: "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Local(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(relative))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement nodes,string id)=>
    nodes.EnumerateArray().Single(item=>item.GetProperty("id").GetString()==id);
static string Get(JsonElement obj,string property)=>
    obj.GetProperty(property).GetString() ??
        throw new InvalidOperationException("Missing text: "+property);
static bool Flag(JsonElement obj,string property)=>
    obj.GetProperty(property).GetBoolean();
static void State(JsonElement obj,string expected,string property="status")=>
    Need(Get(obj,property)==expected,"Unexpected "+property+" for "+
        Get(obj,"id")+"; expected "+expected);
static void Contains(string text,string term,string label)=>
    Need(text.Contains(term,StringComparison.OrdinalIgnoreCase),
        label+" missing: "+term);

Need(args.Length==0 || (args.Length==1 && args[0]=="--check"),
    "Only read-only --check mode is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var splitDoc=Open(Split);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var split=splitDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Get(backlog,"plan_version")==(target?"0.1.111":"0.1.110"),
    "Wrong exact accepted RS111 baseline or RS112 target planning version.");

Need(Get(prior,"status")=="m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified" &&
    prior.GetProperty("independent_oracle_groups").GetInt32()==2 &&
    prior.GetProperty("reference_root_value").GetInt32()==13 &&
    prior.GetProperty("reference_node_count").GetInt32()==4 &&
    prior.GetProperty("reference_edge_count").GetInt32()==4 &&
    prior.GetProperty("reference_shared_evaluations").GetInt32()==1 &&
    prior.GetProperty("research_max_nodes").GetInt32()==8 &&
    prior.GetProperty("research_max_edges").GetInt32()==10 &&
    prior.GetProperty("research_max_depth").GetInt32()==4 &&
    Flag(prior,"cycle_refused") &&
    Flag(prior,"unknown_dependency_refused") &&
    Flag(prior,"checked_overflow_refused") &&
    Flag(prior,"no_partial_result_on_refusal") &&
    Flag(prior,"local_bcl_oracles_executed") &&
    Flag(prior,"t1_done") && !Flag(prior,"t2_done") &&
    !Flag(prior,"s01_phase_done") &&
    !Flag(prior,"dag_runtime_api_delivered") &&
    !Flag(prior,"aura_job_engine_copied"),
    "Prior T1 pure DAG source-free research and negative oracle receipt changed.");
foreach(var (name,path) in new[]
{
    ("oracle_validator_sha256",PriorOracle),
    ("decision_document_sha256",PriorDecision),
    ("owner_screen_sha256",PriorScreen)
})
    Need(Get(prior,name)==Hash(path),
         "Previously admitted RS111 byte-pinned source changed: "+path);
Need(Get(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     split.GetProperty("native_nodes_after").GetInt32()==541 &&
     Flag(split,"dag_split_active") &&
     !Flag(split,"plot_split_active") &&
     !Flag(split,"dag_runtime_api_delivered") &&
     !Flag(split,"plot_renderer_delivered"),
     "Accepted size-L split may not be weakened or treated as product delivery.");

State(Find(backlog.GetProperty("releases"),"M6"),"done");
State(Find(native.GetProperty("milestones"),"M6"),"done","state");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var parent=Find(backlog.GetProperty("chunks"),Parent);
var product=Find(backlog.GetProperty("chunks"),Phase);
var next=Find(backlog.GetProperty("chunks"),Plot);
State(parent,"planned");
State(product,target?"done":"in_progress");
State(next,"planned");
Need(Get(parent,"size")=="L"&&Get(product,"size")=="M"&&
     Get(product,"parent_chunk")==Parent&&Get(product,"kind")=="spike",
     "Accepted size-L parent and bounded research split identities have drifted.");
var t1=Find(product.GetProperty("tasks"),"M7-W01-C03-S01-T1");
var t2=Find(product.GetProperty("tasks"),Task);
State(t1,"done");
State(t2,target?"done":"planned");

var m7=Find(native.GetProperty("milestones"),"M7");
var wp=Find(m7.GetProperty("workPackages"),"M7-W01");
var parentPlan=Find(wp.GetProperty("phases"),Parent);
var phasePlan=Find(wp.GetProperty("phases"),Phase);
var plotPlan=Find(wp.GetProperty("phases"),Plot);
State(m7,"in-progress","state");
State(wp,"in-progress","state");
State(parentPlan,"not-ready","state");
State(phasePlan,target?"done":"in-progress","state");
State(plotPlan,"not-ready","state");
var nativeT1=Find(phasePlan.GetProperty("tasks"),"M7-W01-C03-S01-T1");
var nativeT2=Find(phasePlan.GetProperty("tasks"),Task);
State(nativeT1,"done","state");
State(nativeT2,target?"done":"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(Find(t1.GetProperty("subtasks"),"M7-W01-C03-S01-T1-"+suffix),"done");
    State(Find(nativeT1.GetProperty("subtasks"),"M7-W01-C03-S01-T1-"+suffix),
        "done","state");
    State(Find(t2.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"planned");
    State(Find(nativeT2.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"not-ready","state");
}
var document=Read(Decision);
var distribution=Read(Distribution);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "DEFER","Three mandatory","All three triggers are necessary",
    "No public DAG API","AURA owns","No partial result",
    "M7-W01-C03-S02","source-free"})
    Contains(document,term,"T2 Q/O/D decision");
foreach(var term in new[]{"## Retained finite independent reference",
    "## Ownership/provider matrix","## Three review triggers",
    "Consumer/operation need","Identity/failure/resource contract",
    "Source/provider and recipient gate","If any review trigger is missing, remain deferred",
    "AURA-owned","source-free consumer"})
    Contains(distribution,term,"T2 negative owner gate");
Need(distribution.Split('\n').Count(line=>line.Contains("| **Defer** |",
    StringComparison.Ordinal))==3,
    "Exactly three new pure DAG runtime/provider categories must remain deferred.");
Need(!document.Contains("public DAG runtime delivered",StringComparison.OrdinalIgnoreCase),
    "The decision must not claim a new Math graph runtime.");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var evidence=evidenceDoc.RootElement;
    Need(Get(evidence,"status")==Status &&
         Get(evidence,"disposition")=="defer-unqualified-pure-dag-runtime-and-providers" &&
         evidence.GetProperty("review_trigger_count").GetInt32()==3 &&
         evidence.GetProperty("deferred_provider_categories").GetInt32()==3 &&
         evidence.GetProperty("prior_reference_root_value").GetInt32()==13 &&
         Flag(evidence,"t1_done") && Flag(evidence,"t2_done") &&
         Flag(evidence,"s01_phase_done") &&
         !Flag(evidence,"parent_c03_done") &&
         !Flag(evidence,"plot_phase_done") &&
         !Flag(evidence,"m7_w01_work_package_done") &&
         !Flag(evidence,"m7_release_done") &&
         !Flag(evidence,"dag_runtime_api_delivered") &&
         !Flag(evidence,"aura_job_engine_copied") &&
         !Flag(evidence,"plot_renderer_delivered") &&
         !Flag(evidence,"external_dag_provider_adopted") &&
         !Flag(evidence,"automatic_cutover") &&
         !Flag(evidence,"publicly_published") &&
         Get(evidence,"decision_document_sha256")==Hash(Decision) &&
         Get(evidence,"distribution_document_sha256")==Hash(Distribution) &&
         Get(evidence,"boundary_validator_sha256")==Hash(
             "docs/planning/ValidateM7DagBoundary.cs"),
         "Completed pure DAG decision is not the exact owner-gated deferred target.");
}
Console.WriteLine(Marker);
return 0;
