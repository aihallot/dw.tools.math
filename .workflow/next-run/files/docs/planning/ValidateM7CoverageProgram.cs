#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Report=".aura/workflow/reports/RS120/attempt-001.json";
const string Prior="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string WorkResearch="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string FormalResearch="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-c02.md";
const string Distribution="docs/distribution/m7-coverage-program-disposition.md";
const string Evidence="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string Requests="docs/coordination/requests/README.md";
const string Aura="docs/coordination/requests/MATH-XR-001-aura.md";
const string DecisionRequest="docs/coordination/requests/MATH-XR-002-decision.md";
const string Mcdm="docs/coordination/requests/MATH-XR-003-mcdm.md";
const string Phase="M7-W02-C02";
const string Task="M7-W02-C02-T2";
const string Status="m7-w02-c02-t2-documentary-coverage-program-boundary-qualified";
const string Marker="dw.tools.math/m7-w02-c02-t2-coverage-program/1.2 qualified; c02=document-done; w02=open; m7=open; external=unsent; public-product=false";
const string Counts="M7-PROGRAM:REQUIREMENTS=106;OWNERS=53;SOURCES=20;DISPOSITIONS=6;DRAFTS=3;NODES=541";
const string Scope="M7-PROGRAM:SCOPE:C02=DOCUMENT-DONE;W02=OPEN;M7=OPEN;CLIENT-ADOPTION=false;RELEASE=false";

static void Need(bool pass,string reason)
{
    if(!pass) throw new InvalidOperationException("M7 program document validation: "+reason);
}
static string Local(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||
         path.StartsWith(".aura/workflow/",StringComparison.Ordinal),
         "Unexpected source path: "+path);
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\'),"Unsafe source path: "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static string Text(JsonElement item,string key)=>
    item.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing text field "+key);
static bool Flag(JsonElement item,string key)=>item.GetProperty(key).GetBoolean();
static JsonElement Find(JsonElement elements,string id)=>
    elements.EnumerateArray().Single(x=>Text(x,"id")==id);
static void State(JsonElement item,string state,string key="status")=>
    Need(Text(item,key)==state,"Wrong "+key+" for "+Text(item,"id")+" expected "+state);
static void Has(string body,string needle,string source)=>
    Need(body.Contains(needle,StringComparison.OrdinalIgnoreCase),
         source+" lacks "+needle);
static int Count(JsonElement node)
{
    var value=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            value+=children.EnumerateArray().Sum(Count);
    return value;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "This checker is observational; use --check only.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(Report);
using var t1Doc=Open(Prior);
using var workDoc=Open(WorkResearch);
using var formalDoc=Open(FormalResearch);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var t1=t1Doc.RootElement;
var work=workDoc.RootElement;
var formal=formalDoc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")==(target?"0.1.120":"0.1.119"),
    "Unexpected source-of-truth plan version.");
Need(Text(report,"runId")=="RS120"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify M7 coverage and owner inventory"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
     "RS120 must be a durable successful attempt 001.");
var refusal=report.GetProperty("priorInvocationFailures");
Need(refusal.GetProperty("count").GetInt32()==2&&
     refusal.GetProperty("preparationFailureCount").GetInt32()==2&&
     refusal.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
     !Flag(refusal,"firstPassSucceeded"),
     "RS120 pre-attempt refusals must not be recast as first-pass.");

Need(Text(t1,"status")==
         "m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified"&&
     Flag(t1,"t1_done")&&!Flag(t1,"t2_done")&&
     !Flag(t1,"c02_parent_done")&&
     !Flag(t1,"m7_w02_work_package_done")&&
     !Flag(t1,"m7_release_done")&&
     !Flag(t1,"external_responses_confirmed")&&
     !Flag(t1,"external_requests_transmitted")&&
     !Flag(t1,"public_product_delivered")&&
     t1.GetProperty("native_nodes_retained").GetInt32()==541,
     "Source-pinned RS120 T1 evidence was modified or promoted.");
foreach(var (field,path) in new[]{
    ("decision_sha256","docs/planning/decisions/m7-w02-c02-t1.md"),
    ("distribution_sha256","docs/distribution/m7-coverage-ownership-inventory.md"),
    ("validator_sha256","docs/planning/ValidateM7CoverageInventory.cs")
})
    Need(Text(t1,field)==Sha(path),"T1 source SHA changed: "+path);
Need(Text(work,"status")==
         "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"&&
     Flag(work,"m7_w01_work_package_done")&&
     !Flag(work,"new_advanced_product_delivered")&&
     !Flag(work,"m7_release_done"),
     "Advanced work remained qualified research with deferred products.");
Need(Text(formal,"status")==
         "m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified"&&
     Flag(formal,"c01_phase_done")&&
     !Flag(formal,"public_formal_api_delivered")&&
     !Flag(formal,"external_prover_adopted")&&
     !Flag(formal,"m7_release_done"),
     "Formal-provider deferral must remain explicit.");

var releases=backlog.GetProperty("releases").EnumerateArray().ToArray();
var workPackages=backlog.GetProperty("work_packages").EnumerateArray().ToArray();
var chunks=backlog.GetProperty("chunks").EnumerateArray().ToArray();
var chunkIds=chunks.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
var releaseIds=releases.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
Need(releases.Length==8&&workPackages.Length==17&&chunks.Length==58,
     "Accepted Math roadmap dimensions changed.");
Need(releases.Count(x=>Text(x,"status")=="done")==7&&
     releases.Count(x=>Text(x,"status")=="in_progress")==1&&
     workPackages.Count(x=>Text(x,"status")=="done")==16&&
     workPackages.Count(x=>Text(x,"status")=="in_progress")==1,
     "M7 and M7-W02 must stay open.");
Need(chunks.Count(x=>Text(x,"status")=="done")==(target?58:57),
     "C02 alone may complete in this documentary phase.");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W02"),"in_progress");
State(Find(backlog.GetProperty("chunks"),"M7-W02-C01"),"done");
var phase=Find(backlog.GetProperty("chunks"),Phase);
State(phase,target?"done":"in_progress");
Need(Text(phase,"kind")=="document"&&Text(phase,"size")=="M",
     "This is an accepted document, not a product phase.");
var t1Task=Find(phase.GetProperty("tasks"),"M7-W02-C02-T1");
var t2Task=Find(phase.GetProperty("tasks"),Task);
State(t1Task,"done");
State(t2Task,target?"done":"planned");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(Find(t1Task.GetProperty("subtasks"),"M7-W02-C02-T1-"+suffix),"done");
    State(Find(t2Task.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"planned");
}
var requirements=backlog.GetProperty("requirements").EnumerateArray().ToArray();
var requirementIds=new HashSet<string>(StringComparer.Ordinal);
var owners=new HashSet<string>(StringComparer.Ordinal);
Need(requirements.Length==106,"Canonical requirement count changed.");
foreach(var item in requirements)
{
    var id=Text(item,"id");
    var owner=Text(item,"owner");
    var release=Text(item,"release");
    Need(requirementIds.Add(id)&&chunkIds.Contains(owner)&&
         releaseIds.Contains(release)&&
         chunks.Any(c=>Text(c,"id")==owner&&Text(c,"release")==release)&&
         !string.IsNullOrWhiteSpace(Text(item,"acceptance")),
         "Broken or duplicated accepted requirement: "+id);
    owners.Add(owner);
}
Need(owners.Count==53,"Requirement-owner set changed.");
var sources=backlog.GetProperty("source_coverage").EnumerateArray().ToArray();
var sourceIds=new HashSet<string>(StringComparer.Ordinal);
Need(sources.Length==20,"Canonical framing-source count changed.");
foreach(var source in sources)
{
    var id=Text(source,"source_id");
    var targets=source.GetProperty("targets").EnumerateArray()
        .Select(x=>x.GetString()??"").ToArray();
    Need(sourceIds.Add(id)&&!string.IsNullOrWhiteSpace(Text(source,"disposition"))&&
         targets.Length>0&&targets.All(chunkIds.Contains)&&
         targets.Distinct(StringComparer.Ordinal).Count()==targets.Length,
         "Bad source target or duplicate source: "+id);
}
var dispositions=backlog.GetProperty("dispositions").EnumerateArray().ToArray();
Need(dispositions.Length==6&&
     dispositions.Count(x=>Text(x,"status")=="externally_owned")==4&&
     dispositions.Count(x=>Text(x,"status")=="excluded")==2,
     "External and excluded dispositions must remain classified.");
foreach(var item in dispositions)
    Need(!string.IsNullOrWhiteSpace(Text(item,"owner"))&&
         !string.IsNullOrWhiteSpace(Text(item,"reason"))&&
         !string.IsNullOrWhiteSpace(Text(item,"review_trigger"))&&
         item.GetProperty("targets").EnumerateArray().All(x=>
             chunkIds.Contains(x.GetString()??"")),
         "Disposition lost owner, reason or target: "+Text(item,"id"));
var dependencies=backlog.GetProperty("external_dependencies").EnumerateArray().ToArray();
Need(dependencies.Length==4&&
     dependencies.All(x=>Text(x,"status")=="satisfied"),
     "Only the four Math-side local dependency gates are satisfied.");
foreach(var item in dependencies)
    Need(item.GetProperty("evidence").EnumerateArray().All(x=>
        File.Exists(Local(x.GetString()??""))),
        "Missing local dependency evidence: "+Text(item,"id"));

var m7=Find(native.GetProperty("milestones"),"M7");
var w02=Find(m7.GetProperty("workPackages"),"M7-W02");
var c02=Find(w02.GetProperty("phases"),Phase);
var nativeT2=Find(c02.GetProperty("tasks"),Task);
State(m7,"in-progress","state");
State(Find(m7.GetProperty("workPackages"),"M7-W01"),"done","state");
State(w02,"in-progress","state");
State(Find(w02.GetProperty("phases"),"M7-W02-C01"),"done","state");
State(c02,target?"done":"in-progress","state");
State(Find(c02.GetProperty("tasks"),"M7-W02-C02-T1"),"done","state");
State(nativeT2,target?"done":"not-ready","state");
for(var i=0;i<3;i++)
    State(Find(nativeT2.GetProperty("subtasks"),Task+"-"+((char)('A'+i))),
          target?"done":"not-ready","state");
Need(native.GetProperty("milestones").EnumerateArray().Sum(Count)==541,
     "Native roadmap must retain 541 nodes.");

Has(Read(Requests),"have not been sent","Coordination README");
foreach(var (path,token) in new[]{
    (Aura,"MATH-XR-001"),(DecisionRequest,"MATH-XR-002"),(Mcdm,"MATH-XR-003")
})
{
    var draft=Read(path);
    Has(draft,token,"Recipient request draft");
    Has(draft,"draft","Recipient request draft");
    Has(draft,"Transmission","Recipient request draft");
    Has(draft,"none","Recipient request draft");
}
var decision=Read(Decision);
foreach(var term in new[]{"## Question","## Verified observations",
    "## Coverage, migration and owner disposition",
    "## T2 Q/O/D decision","Close","document parent",
    "## Candidate next program","M7-W02 work-package","release"})
    Has(decision,term,"Canonical C02 closure decision");
var matrix=Read(Distribution);
foreach(var term in new[]{"## Source-pinned inventory",
    "## Ownership and pending response matrix",
    "## Program and release boundary","## Reproducibility and review",
    "106","541","unsent","not a public"})
    Has(matrix,term,"Distribution program matrix");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var receipt=evidenceDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("requirements_checked").GetInt32()==106&&
         receipt.GetProperty("distinct_owners").GetInt32()==53&&
         receipt.GetProperty("source_coverage_rows").GetInt32()==20&&
         receipt.GetProperty("ownership_dispositions").GetInt32()==6&&
         receipt.GetProperty("local_dependency_gates").GetInt32()==4&&
         receipt.GetProperty("unsent_requests").GetInt32()==3&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         Flag(receipt,"t1_done")&&Flag(receipt,"t2_done")&&
         Flag(receipt,"c02_document_phase_done")&&
         !Flag(receipt,"m7_w02_work_package_done")&&
         !Flag(receipt,"m7_release_done")&&
         !Flag(receipt,"new_public_product_delivered")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"future_program_approved")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Distribution)&&
         Text(receipt,"validator_sha256")==Sha(
             "docs/planning/ValidateM7CoverageProgram.cs"),
         "Document closure must not become external or release approval.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
