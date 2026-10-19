#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c01-t2.md";
const string Gate="docs/distribution/m7-precision-interval-deferral.md";
const string PriorDecision="docs/planning/decisions/m7-w01-c01.md";
const string PriorScreen="docs/planning/decisions/m7-w01-c01-provider-screen.md";
const string PriorOracle="docs/planning/ValidateM7PrecisionAdmission.cs";
const string Status="m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified";
const string Marker="dw.tools.math/m7-w01-c01-t2-deferred-provider-gate/1.2 qualified; triggers=3; provider=none; public-api=false";
const string Phase="M7-W01-C01";
const string Task="M7-W01-C01-T2";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("Independent M7 precision boundary: "+reason);
}
static string PathFor(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) ||
         relative==Native,"The read-only gate may inspect only local planning/evidence files.");
    Need(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal) &&
         !relative.Contains('\\'),"Unsafe local evidence path: "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(PathFor(relative));
static JsonDocument Json(string relative)=>JsonDocument.Parse(Read(relative));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(PathFor(relative))))
       .ToLowerInvariant();
static JsonElement Find(JsonElement array,string id)=>
    array.EnumerateArray().Single(item=>item.GetProperty("id").GetString()==id);
static void State(JsonElement item,string expected,string property="status")
{
    var found=item.GetProperty(property).GetString();
    Need(found==expected,"Wrong "+property+" for "+
         item.GetProperty("id").GetString()+": "+found);
}
static void Includes(string text,string term,string label)=>
    Need(text.Contains(term,StringComparison.OrdinalIgnoreCase),
        label+" is missing: "+term);

Need(args.Length==0 || (args.Length==1 && args[0]=="--check"),
    "Only read-only --check is supported.");
using var backlogDoc=Json(Backlog);
using var nativeDoc=Json(Native);
using var priorDoc=Json(Prior);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var target=File.Exists(PathFor(Evidence));
Need(backlog.GetProperty("plan_version").GetString()==
        (target?"0.1.106":"0.1.105"),
    "The exact baseline/target planning version must match the T2 receipt.");
State(Find(backlog.GetProperty("releases"),"M6"),"done");
State(Find(native.GetProperty("milestones"),"M6"),"done","state");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var chunk=Find(backlog.GetProperty("chunks"),Phase);
State(chunk,target?"done":"in_progress");
Need(chunk.GetProperty("kind").GetString()=="spike",
    "Research spike cannot be interpreted as a delivered product.");
var t1=Find(chunk.GetProperty("tasks"),"M7-W01-C01-T1");
var t2=Find(chunk.GetProperty("tasks"),Task);
State(t1,"done");
State(t2,target?"done":"planned");
var nextChunk=Find(backlog.GetProperty("chunks"),"M7-W01-C02");
State(nextChunk,"planned");
var m7=Find(native.GetProperty("milestones"),"M7");
var wp=Find(m7.GetProperty("workPackages"),"M7-W01");
var phase=Find(wp.GetProperty("phases"),Phase);
State(m7,"in-progress","state");
State(wp,"in-progress","state");
State(phase,target?"done":"in-progress","state");
var nativeT1=Find(phase.GetProperty("tasks"),"M7-W01-C01-T1");
var nativeT2=Find(phase.GetProperty("tasks"),Task);
State(nativeT1,"done","state");
State(nativeT2,target?"done":"not-ready","state");
State(Find(wp.GetProperty("phases"),"M7-W01-C02"),"not-ready","state");
for(var i=0;i<3;i++)
{
    State(Find(t1.GetProperty("subtasks"),"M7-W01-C01-T1-"+(char)('A'+i)),"done");
    State(Find(nativeT1.GetProperty("subtasks"),"M7-W01-C01-T1-"+(char)('A'+i)),
        "done","state");
    State(Find(t2.GetProperty("subtasks"),"M7-W01-C01-T2-"+(char)('A'+i)),
        target?"done":"planned");
    State(Find(nativeT2.GetProperty("subtasks"),"M7-W01-C01-T2-"+(char)('A'+i)),
        target?"done":"not-ready","state");
}
Need(prior.GetProperty("status").GetString()==
        "m7-c01-t1-bcl-exact-precision-interval-oracles-provider-screen-qualified" &&
     prior.GetProperty("precision_reference_oracles_run").GetBoolean() &&
     prior.GetProperty("independent_oracle_cases").GetInt32()==2 &&
     prior.GetProperty("reference_tenth_enclosed").GetBoolean() &&
     prior.GetProperty("exact_integer_result").GetString()=="1" &&
     prior.GetProperty("observed_binary64_cancellation").GetString()=="0" &&
     !prior.GetProperty("external_provider_adopted").GetBoolean() &&
     !prior.GetProperty("public_precision_api_delivered").GetBoolean() &&
     !prior.GetProperty("one_constant_enclosure_is_general_interval_certification").GetBoolean() &&
     prior.GetProperty("t1_done").GetBoolean() &&
     !prior.GetProperty("t2_done").GetBoolean(),
     "Historical independent BCL precision observation no longer supports deferral.");
foreach(var (field,file) in new[]{
    ("oracle_validator_sha256",PriorOracle),
    ("decision_document_sha256",PriorDecision),
    ("provider_screen_sha256",PriorScreen)
})
    Need(prior.GetProperty(field).GetString()==Hash(file),
        "The retained RS106 source/evidence SHA identity changed: "+file);

var decision=Read(Decision);
var distribution=Read(Gate);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "DEFER","trigger","consumer gap","license","rounding",
    "no product","M7-W01-C02"})
    Includes(decision,term,"T2 Q/O/D decision");
foreach(var term in new[]{"Not-admitted candidate matrix","Three review triggers",
    "Consumer/operation need","Provider provenance and cost",
    "Independent mathematical acceptance","no provider packages",
    "Do not promote","Defer","M7-W01-C02"})
    Includes(distribution,term,"T2 distribution/review gate");
Need(distribution.Split('\n').Count(line=>line.Contains("| **Defer** |",
    StringComparison.Ordinal))==3,
    "Exactly three new managed/native provider categories must remain deferred.");
Need(distribution.Contains("If any trigger is missing: **remain deferred**",
    StringComparison.Ordinal),
    "Missing negative owner-gate policy.");
Need(decision.Contains("All three triggers are necessary",
    StringComparison.Ordinal),
    "No automatic adoption: all three review gates must be necessary.");

if(target)
{
    using var evidenceDoc=Json(Evidence);
    var e=evidenceDoc.RootElement;
    Need(e.GetProperty("status").GetString()==Status &&
        e.GetProperty("disposition").GetString()=="defer-new-precision-interval-provider" &&
        e.GetProperty("review_trigger_count").GetInt32()==3 &&
        e.GetProperty("t1_done").GetBoolean() &&
        e.GetProperty("t2_done").GetBoolean() &&
        e.GetProperty("m7_w01_c01_parent_done").GetBoolean() &&
        !e.GetProperty("m7_w01_work_package_done").GetBoolean() &&
        !e.GetProperty("m7_release_done").GetBoolean() &&
        !e.GetProperty("external_provider_adopted").GetBoolean() &&
        !e.GetProperty("public_precision_api_delivered").GetBoolean() &&
        !e.GetProperty("automatic_cutover").GetBoolean() &&
        e.GetProperty("decision_document_sha256").GetString()==Hash(Decision) &&
        e.GetProperty("distribution_document_sha256").GetString()==Hash(Gate) &&
        e.GetProperty("boundary_validator_sha256").GetString()==Hash(
            "docs/planning/ValidateM7PrecisionBoundary.cs"),
        "Completed T2 deferral failed source/provenance or non-adoption gates.");
}
Console.WriteLine(Marker);
