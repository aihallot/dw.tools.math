#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string W01="docs/planning/evidence/M6-W01-local-gate-qualified.json";
const string Scientific="docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json";
const string T1="docs/planning/evidence/M6-W02-C05-T1-qualified.json";
const string T2="docs/planning/evidence/M6-W02-C05-T2-qualified.json";
const string Publication="docs/planning/releases/m5-v1-publication-boundary.json";
const string Gate="docs/planning/evidence/M6-gate.json";
const string GateStatus="m6-independent-local-analysis-scientific-release-gate-qualified-unpublished";
const string AnalysisSha="facb88dcf029de1445122eef38a147b609b89d1c5a36567506b1c10c2ce423ac";
const string ScientificSha="2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf";
const string Marker="dw.tools.math/m6-local-science-gate/1.1 qualified; phases=11; tasks=22; subtasks=66; consumers=10; public=false";
var phases=new (string Id,string Status)[]
{
    ("M6-W01-C01","bounded-curve-explicit-unit-conditioning-extrapolation-refusals-qualified"),
    ("M6-W01-C02","bounded-univariate-residual-and-regularity-audit-qualified"),
    ("M6-W01-C03","bounded-differential-declared-regularity-and-ode-regime-audit-qualified"),
    ("M6-W02-C01","bounded-exact-signed-and-intermediate-bit-audit-local-nuget-qualified"),
    ("M6-W02-C02","bounded-managed-special-angle-gamma-branch-risk-audit-qualified"),
    ("M6-W02-C03","bounded-cross-family-scientific-boundary-integration-local-nuget-qualified"),
    ("M6-W02-C03-S01","bounded-finite-euclidean-2d-3d-resolution-and-projection-risk-local-nuget-qualified"),
    ("M6-W02-C03-S02","bounded-direct-dft-explicit-sampling-linear-topology-cancellation-local-nuget-qualified"),
    ("M6-W02-C03-S03","bounded-uniform-grid-interval-and-numeric-risk-local-nuget-qualified"),
    ("M6-W02-C04","bounded-actual-day-finance-root-topology-local-nuget-qualified"),
    ("M6-W02-C05","owner-gated-scientific-adoption-no-cutover-and-notices-qualified")
};
static void Need(bool truth,string reason)
{
    if(!truth)throw new InvalidOperationException("Independent G-M6 local gate: "+reason);
}
static string ReadText(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal) ||
         path==Native,"Only accepted local evidence and native plan paths are readable.");
    Need(!Path.IsPathRooted(path) && !path.Contains("..",StringComparison.Ordinal) &&
        !path.Contains('\\'),"Invalid evidence path: "+path);
    return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar)));
}
static JsonDocument Read(string path)=>JsonDocument.Parse(ReadText(path));
static string Field(JsonElement o,string key)=>
    o.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing string field: "+key);
static bool Flag(JsonElement o,string key)=>o.GetProperty(key).GetBoolean();
static JsonElement ById(JsonElement list,string id)=>
    list.EnumerateArray().Single(o=>Field(o,"id")==id);
static void CheckState(JsonElement o,string state,string property="status")=>
    Need(Field(o,property)==state,"State differs: "+Field(o,"id")+" expected "+state);
static string Hash(string path)=>
    Convert.ToHexString(SHA256.HashData(
        File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(),
            path.Replace('/',Path.DirectorySeparatorChar))))).ToLowerInvariant();
static string EvidencePath(string id)=>"docs/planning/evidence/"+
    id+"-T2-"+(id=="M6-W02-C05"?"qualified":"boundaries-qualified")+".json";
static void CheckConsumers(JsonElement array,int count,string sha,long bytes)
{
    Need(array.GetArrayLength()==count,"Wrong historical isolated consumer count.");
    var distinct=array.EnumerateArray().Select(o=>Field(o,"project")).ToArray();
    Need(distinct.Distinct(StringComparer.Ordinal).Count()==count,
        "Historical source-free consumers are not independent.");
    foreach(var c in array.EnumerateArray())
    {
        Need(Field(c,"package_sha256")==sha &&
             c.GetProperty("package_bytes").GetInt64()==bytes &&
             c.GetProperty("independent_probes").GetInt32()==8 &&
             c.GetProperty("project_references").GetInt32()==0,
             "Historical consumer bytes, probes or project independence differ.");
    }
}
Need(args.Length==0 || (args.Length==1 && args[0]=="--check"),
    "Only read-only --check is supported.");
using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
using var w01Doc=Read(W01);
using var sciDoc=Read(Scientific);
using var t1Doc=Read(T1);
using var t2Doc=Read(T2);
using var pubDoc=Read(Publication);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var w01=w01Doc.RootElement;
var sci=sciDoc.RootElement;
var t1=t1Doc.RootElement;
var t2=t2Doc.RootElement;
var publication=pubDoc.RootElement;

var version=Field(backlog,"plan_version");
Need(version=="0.1.103"||version=="0.1.104",
    "Expected accepted RS104 baseline or exact RS105 target backlog version.");
var completed=version=="0.1.104";
for(var i=0;i<6;i++)
{
    var id="M"+i;
    CheckState(ById(backlog.GetProperty("releases"),id),"done");
    CheckState(ById(native.GetProperty("milestones"),id),"done","state");
}
var release=ById(backlog.GetProperty("releases"),"M6");
var milestone=ById(native.GetProperty("milestones"),"M6");
CheckState(release,completed?"done":"in_progress");
CheckState(milestone,completed?"done":"in-progress","state");
Need(Field(release,"gate")=="G-M6" && Field(release,"version")=="1.1.0",
    "Accepted M6 scientific milestone identity/version changed.");
var required=release.GetProperty("required_chunks").EnumerateArray()
    .Select(v=>v.GetString()??"").ToArray();
Need(required.Length==11 && required.SequenceEqual(phases.Select(x=>x.Id)),
    "Exactly eleven accepted M6 phase ids must remain in canonical order.");

foreach(var id in new[]{"M6-W01","M6-W02"})
{
    CheckState(ById(backlog.GetProperty("work_packages"),id),"done");
    CheckState(ById(milestone.GetProperty("workPackages"),id),"done","state");
}
Need(Field(w01,"status")=="m6-w01-all-three-numerical-families-local-nuget-qualified" &&
     Field(w01,"local_product_version")=="1.1.0-preview.6" &&
     w01.GetProperty("source_free_consumer_count").GetInt32()==3 &&
     w01.GetProperty("retained_targeted_tests_passed").GetInt32()==328 &&
     w01.GetProperty("independent_consumer_probes_passed").GetInt32()==24 &&
     Flag(w01,"m6_w01_work_package_done") && !Flag(w01,"m6_release_done") &&
     !Flag(w01,"publicly_published") &&
     Field(w01,"actual_inspected_package_sha256")==AnalysisSha &&
     w01.GetProperty("actual_inspected_package_bytes").GetInt64()==27820,
     "W01 local analysis receipt is not the admitted historical package.");
CheckConsumers(w01.GetProperty("consumer_receipts"),3,AnalysisSha,27820);
Need(Field(sci,"status")=="bounded-actual-day-finance-root-topology-local-nuget-qualified" &&
     Field(sci,"local_product_version")=="1.1.0-preview.14" &&
     Field(sci,"actual_inspected_package_sha256")==ScientificSha &&
     sci.GetProperty("actual_inspected_package_bytes").GetInt64()==37270 &&
     !Flag(sci,"publicly_published") && !Flag(sci,"m6_release_done"),
     "W02 scientific historical local package receipt differs.");
CheckConsumers(sci.GetProperty("isolated_consumer_receipts"),7,ScientificSha,37270);
Need(Field(t1,"status")=="seven-scientific-family-contract-pins-and-owner-gated-reuse-request-documented" &&
     t1.GetProperty("qualified_family_count").GetInt32()==7 &&
     Field(t1,"prior_package_sha256")==ScientificSha &&
     !Flag(t1,"decision_owner_approval_obtained") &&
     !Flag(t1,"publicly_published"),
     "C05-T1 seven independently qualified family records changed.");
Need(Field(t2,"status")=="owner-gated-scientific-adoption-no-cutover-and-notices-qualified" &&
     Flag(t2,"m6_w02_c05_parent_done") && Flag(t2,"m6_w02_work_package_done") &&
     !Flag(t2,"m6_release_done") &&
     !Flag(t2,"owner_approval_obtained") &&
     !Flag(t2,"external_cutover_performed") &&
     !Flag(t2,"publicly_published"),
     "C05-T2 no automatic adoption or owner gate was lost.");
Need(!Flag(publication.GetProperty("owner"),"stable_v1_publication_authorized") &&
     !Flag(publication.GetProperty("publication"),"public_nuget_published"),
     "Existing publication authorization boundary has changed.");
Need(ReadText("docs/planning/decisions/m6-gate.md")
        .Contains("no public NuGet",StringComparison.OrdinalIgnoreCase) &&
     ReadText("docs/distribution/m6-local-release-gate.md")
        .Contains("G-M6",StringComparison.Ordinal),
     "Missing independently staged M6 local-science gate decision or recipe.");

var tasks=0;
var subtasks=0;
foreach(var phase in phases)
{
    var product=ById(backlog.GetProperty("chunks"),phase.Id);
    var wpId=phase.Id.StartsWith("M6-W01-",StringComparison.Ordinal)?"M6-W01":"M6-W02";
    var wp=ById(milestone.GetProperty("workPackages"),wpId);
    var planPhase=ById(wp.GetProperty("phases"),phase.Id);
    CheckState(product,"done");
    CheckState(planPhase,"done","state");
    var proofPath=EvidencePath(phase.Id);
    Need(product.GetProperty("evidence").EnumerateArray()
        .Any(v=>v.GetString()==proofPath),
        "Accepted product phase does not cite its durable gate receipt: "+phase.Id);
    using(var proofDoc=Read(proofPath))
    {
        var proof=proofDoc.RootElement;
        Need(Field(proof,"status")==phase.Status && !Flag(proof,"m6_release_done"),
            "Historical scientific receipt status or release truth changed: "+phase.Id);
    }
    var productTasks=product.GetProperty("tasks").EnumerateArray().ToArray();
    var planTasks=planPhase.GetProperty("tasks").EnumerateArray().ToArray();
    Need(productTasks.Length==2 && planTasks.Length==2,
        "M6 requires exactly two accepted tasks per phase: "+phase.Id);
    foreach(var task in productTasks)
    {
        var id=Field(task,"id");
        var nativeTask=ById(planPhase.GetProperty("tasks"),id);
        CheckState(task,"done");
        CheckState(nativeTask,"done","state");
        tasks++;
        var productSubtasks=task.GetProperty("subtasks").EnumerateArray().ToArray();
        var nativeSubtasks=nativeTask.GetProperty("subtasks").EnumerateArray().ToArray();
        Need(productSubtasks.Length==3 && nativeSubtasks.Length==3,
            "M6 requires exactly three accepted subtasks per task: "+id);
        foreach(var subtask in productSubtasks)
        {
            var childId=Field(subtask,"id");
            CheckState(subtask,"done");
            CheckState(ById(nativeTask.GetProperty("subtasks"),childId),"done","state");
            subtasks++;
        }
    }
    Console.WriteLine("M6-GATE-PHASE:"+phase.Id+":"+Hash(proofPath));
}
Need(tasks==22 && subtasks==66,
    "M6 release gate must retain exactly 22 tasks and 66 subtask completions.");
if(completed)
{
    using var gateDoc=Read(Gate);
    var gate=gateDoc.RootElement;
    Need(Field(gate,"status")==GateStatus &&
         Field(gate,"gate")=="G-M6" &&
         Field(gate,"release_version")=="1.1.0-local-evidence-only" &&
         gate.GetProperty("qualified_phases").GetInt32()==11 &&
         gate.GetProperty("qualified_tasks").GetInt32()==22 &&
         gate.GetProperty("qualified_subtasks").GetInt32()==66 &&
         gate.GetProperty("historical_source_free_consumers").GetInt32()==10 &&
         Field(gate,"analysis_historical_archive_sha256")==AnalysisSha &&
         Field(gate,"scientific_historical_archive_sha256")==ScientificSha &&
         Flag(gate,"full_solution_tests_passed") &&
         Flag(gate,"m6_milestone_done") &&
         !Flag(gate,"stable_release_certified") &&
         !Flag(gate,"packages_publicly_published") &&
         !Flag(gate,"owner_stable_publication_approved") &&
         !Flag(gate,"foreign_provider_adopted") &&
         !Flag(gate,"external_cutover_performed"),
         "Completed M6 local gate does not preserve all accepted scientific limits.");
    Need(Field(gate,"gate_validator_sha256")==
             Hash("docs/planning/ValidateM6ReleaseGate.cs") &&
         Field(gate,"decision_document_sha256")==
             Hash("docs/planning/decisions/m6-gate.md") &&
         Field(gate,"release_recipe_sha256")==
             Hash("docs/distribution/m6-local-release-gate.md"),
         "Gate receipt source/document SHA-256 differs from repository bytes.");
    var gateEvidence=gate.GetProperty("qualified_phase_evidence")
        .EnumerateArray().Select(x=>x.GetString()??"").ToArray();
    Need(gateEvidence.SequenceEqual(phases.Select(x=>EvidencePath(x.Id))),
        "M6 gate evidence must include all 11 accepted phase receipts.");
    Need(release.GetProperty("evidence").EnumerateArray()
        .Any(x=>x.GetString()==Gate),
        "Completed release must cite its durable G-M6 science gate.");
}
Console.WriteLine(Marker);
