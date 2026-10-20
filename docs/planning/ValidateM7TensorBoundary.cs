#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c02-t2.md";
const string Distribution="docs/distribution/m7-tensor-shape-deferral.md";
const string PriorOracle="docs/planning/ValidateM7TensorAdmission.cs";
const string PriorDecision="docs/planning/decisions/m7-w01-c02.md";
const string PriorScreen="docs/planning/decisions/m7-w01-c02-provider-screen.md";
const string Phase="M7-W01-C02";
const string Task="M7-W01-C02-T2";
const string Status="m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified";
const string Marker="dw.tools.math/m7-w01-c02-t2-tensor-deferral/1.2 qualified; triggers=3; provider=none; api=false; phase=done";

static void Require(bool condition,string reason)
{
    if(!condition) throw new InvalidOperationException("Independent M7 tensor boundary: "+reason);
}
static string Absolute(string relative)
{
    Require(relative.StartsWith("docs/",StringComparison.Ordinal) ||
        relative==Native,"Only bounded local evidence and planning may be read.");
    Require(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal) &&
        !relative.Contains('\\'),"Unsafe local path: "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Absolute(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Absolute(relative))))
        .ToLowerInvariant();
static string Name(JsonElement obj,string field="id")=>
    obj.GetProperty(field).GetString() ??
    throw new InvalidOperationException("Missing text: "+field);
static JsonElement Find(JsonElement values,string id)=>
    values.EnumerateArray().Single(value=>Name(value)==id);
static void StatusOf(JsonElement obj,string expected,string property="status")=>
    Require(Name(obj,property)==expected,
        "Unexpected "+property+" for "+Name(obj)+": "+Name(obj,property));
static void Contains(string text,string term,string label)=>
    Require(text.Contains(term,StringComparison.OrdinalIgnoreCase),label+" missing: "+term);
static bool Yes(JsonElement obj,string property)=>obj.GetProperty(property).GetBoolean();

Require(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check mode is accepted.");

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var target=File.Exists(Absolute(Evidence));
Require(Name(backlog,"plan_version")==(target?"0.1.108":"0.1.107"),
    "Unexpected baseline/target M7 tensor plan version.");

StatusOf(Find(backlog.GetProperty("releases"),"M6"),"done");
StatusOf(Find(native.GetProperty("milestones"),"M6"),"done","state");
StatusOf(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
StatusOf(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var productPhase=Find(backlog.GetProperty("chunks"),Phase);
var priorPhase=Find(backlog.GetProperty("chunks"),"M7-W01-C01");
var nextPhase=Find(backlog.GetProperty("chunks"),"M7-W01-C03");
StatusOf(priorPhase,"done");
StatusOf(productPhase,target?"done":"in_progress");
StatusOf(nextPhase,"planned");
Require(Name(productPhase,"kind")=="spike",
    "C02 must remain a research spike, not a Tensor product delivery.");
var productT1=Find(productPhase.GetProperty("tasks"),"M7-W01-C02-T1");
var productT2=Find(productPhase.GetProperty("tasks"),Task);
StatusOf(productT1,"done");
StatusOf(productT2,target?"done":"planned");

var milestone=Find(native.GetProperty("milestones"),"M7");
var workPackage=Find(milestone.GetProperty("workPackages"),"M7-W01");
var phase=Find(workPackage.GetProperty("phases"),Phase);
StatusOf(milestone,"in-progress","state");
StatusOf(workPackage,"in-progress","state");
StatusOf(Find(workPackage.GetProperty("phases"),"M7-W01-C01"),"done","state");
StatusOf(phase,target?"done":"in-progress","state");
StatusOf(Find(workPackage.GetProperty("phases"),"M7-W01-C03"),"not-ready","state");
var nativeT1=Find(phase.GetProperty("tasks"),"M7-W01-C02-T1");
var nativeT2=Find(phase.GetProperty("tasks"),Task);
StatusOf(nativeT1,"done","state");
StatusOf(nativeT2,target?"done":"not-ready","state");
for(var index=0;index<3;index++)
{
    var suffix=((char)('A'+index)).ToString();
    var doneT1="M7-W01-C02-T1-"+suffix;
    var plannedT2="M7-W01-C02-T2-"+suffix;
    StatusOf(Find(productT1.GetProperty("subtasks"),doneT1),"done");
    StatusOf(Find(nativeT1.GetProperty("subtasks"),doneT1),"done","state");
    StatusOf(Find(productT2.GetProperty("subtasks"),plannedT2),target?"done":"planned");
    StatusOf(Find(nativeT2.GetProperty("subtasks"),plannedT2),target?"done":"not-ready","state");
}

Require(Name(prior,"status")=="m7-c02-t1-dense-shape-oracles-and-provider-screen-qualified" &&
        prior.GetProperty("independent_oracle_groups").GetInt32()==2 &&
        prior.GetProperty("reference_elements").GetInt32()==8 &&
        prior.GetProperty("reference_sum").GetInt32()==36 &&
        prior.GetProperty("reference_index_value").GetInt32()==6 &&
        prior.GetProperty("research_element_budget").GetInt32()==64 &&
        prior.GetProperty("research_byte_budget").GetInt32()==512 &&
        Yes(prior,"same_element_count_different_shape_refused") &&
        Yes(prior,"zero_extent_refused") &&
        Yes(prior,"oversized_extent_refused_before_allocation") &&
        Yes(prior,"t1_done") && !Yes(prior,"t2_done") &&
        !Yes(prior,"advanced_tensor_api_delivered") &&
        !Yes(prior,"external_tensor_provider_adopted") &&
        !Yes(prior,"m7_w01_c02_parent_done"),
        "The historical independent tensor research evidence has drifted.");
foreach(var item in new (string Field,string Path)[]
{
    ("oracle_validator_sha256",PriorOracle),
    ("decision_document_sha256",PriorDecision),
    ("provider_screen_sha256",PriorScreen)
})
    Require(Name(prior,item.Field)==Hash(item.Path),
        "RS108 exact source/document SHA pin drifted: "+item.Path);

var decision=Read(Decision);
var distribution=Read(Distribution);
foreach(var token in new[]
{
    "## Question","## Observation","## Decision","DEFER",
    "three mandatory","[2,2,2]","[2,4]","512 bytes",
    "All three gates","source-free","no public Tensor API",
    "M7-W01-C03","not a tensor implementation"
})
    Contains(decision,token,"Tensor T2 Q/O/D");
foreach(var token in new[]
{
    "Retained independent shape evidence",
    "Provider options and admission status",
    "Three review triggers","Consumer/operation evidence",
    "Bounded shape and resources","Provider and adoption evidence",
    "If any review trigger is missing, remain deferred",
    "source-free consumer","M7-W01-C03"
})
    Contains(distribution,token,"Tensor T2 negative gate");
Require(distribution.Split('\n')
        .Count(line=>line.Contains("| **Defer** |",StringComparison.Ordinal))==4,
    "Exactly four new tensor provider/backend options must remain deferred.");
Require(!decision.Contains("public tensor package delivered",StringComparison.OrdinalIgnoreCase),
    "Contradictory claim of delivered product tensor package.");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var proof=evidenceDoc.RootElement;
    Require(Name(proof,"status")==Status &&
        Name(proof,"disposition")=="defer-unqualified-tensor-api-and-providers" &&
        proof.GetProperty("review_trigger_count").GetInt32()==3 &&
        proof.GetProperty("provider_categories_deferred").GetInt32()==4 &&
        Yes(proof,"t1_done") && Yes(proof,"t2_done") &&
        Yes(proof,"m7_w01_c02_parent_done") &&
        !Yes(proof,"m7_w01_work_package_done") &&
        !Yes(proof,"m7_release_done") &&
        !Yes(proof,"external_tensor_provider_adopted") &&
        !Yes(proof,"advanced_tensor_api_delivered") &&
        !Yes(proof,"automatic_cutover") &&
        !Yes(proof,"publicly_published") &&
        Name(proof,"decision_document_sha256")==Hash(Decision) &&
        Name(proof,"distribution_document_sha256")==Hash(Distribution) &&
        Name(proof,"boundary_validator_sha256")==Hash("docs/planning/ValidateM7TensorBoundary.cs"),
        "Completed T2 source and deferral record is not equal to exact qualified target.");
}
Console.WriteLine(Marker);
