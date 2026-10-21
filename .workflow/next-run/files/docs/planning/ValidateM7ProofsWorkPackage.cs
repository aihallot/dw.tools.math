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
const string Report=".aura/workflow/reports/RS121/attempt-001.json";
const string C01T1="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string C01T2="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string C02T1="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string C02T2="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string W01="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-work-package-closure.md";
const string Matrix="docs/distribution/m7-w02-work-package-research-matrix.md";
const string Evidence="docs/planning/evidence/M7-W02-work-package-qualified.json";
const string Requests="docs/coordination/requests/README.md";
const string Aura="docs/coordination/requests/MATH-XR-001-aura.md";
const string DecisionRequest="docs/coordination/requests/MATH-XR-002-decision.md";
const string Mcdm="docs/coordination/requests/MATH-XR-003-mcdm.md";
const string Status="m7-w02-formal-research-and-document-portfolio-qualified-external-products-deferred";
const string Marker="dw.tools.math/m7-w02-work-package-closure/1.2 qualified; phases=2; tasks=4; qod=12; requests=unsent; provider=false; m7=open";
const string Counts="M7-W02-PORTFOLIO:PHASES=2;TASKS=4;QOD=12;REQUIREMENTS=106;SOURCES=20;DRAFTS=3;NODES=541";
const string Scope="M7-W02-SCOPE:WORK=RESEARCH-DOCUMENT-DONE;M7=IN-PROGRESS;PUBLIC-PRODUCT=false;REQUESTS-SENT=false";

static void Need(bool pass,string message)
{
    if(!pass) throw new InvalidOperationException("M7-W02 work-package qualification: "+message);
}
static string Local(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||
         path.StartsWith(".aura/workflow/",StringComparison.Ordinal),
         "Unexpected source path "+path);
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\'),"Unsafe source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static string Text(JsonElement value,string key)=>
    value.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing string "+key);
static bool Flag(JsonElement value,string key)=>value.GetProperty(key).GetBoolean();
static JsonElement Find(JsonElement array,string id)=>
    array.EnumerateArray().Single(x=>Text(x,"id")==id);
static void State(JsonElement item,string expected,string key="status")=>
    Need(Text(item,key)==expected,
        "Unexpected "+key+" for "+Text(item,"id")+" expected "+expected);
static void Has(string body,string needle,string owner)=>
    Need(body.Contains(needle,StringComparison.OrdinalIgnoreCase),
        owner+" is missing "+needle);
static int Count(JsonElement node)
{
    var count=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            count+=children.EnumerateArray().Sum(Count);
    return count;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check mode supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(Report);
using var c01t1Doc=Open(C01T1);
using var c01t2Doc=Open(C01T2);
using var c02t1Doc=Open(C02T1);
using var c02t2Doc=Open(C02T2);
using var w01Doc=Open(W01);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var c01t1=c01t1Doc.RootElement;
var c01t2=c01t2Doc.RootElement;
var c02t1=c02t1Doc.RootElement;
var c02t2=c02t2Doc.RootElement;
var w01=w01Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")==(target?"0.1.121":"0.1.120"),
    "Unexpected RS121 baseline or RS122 target plan version.");
Need(Text(report,"runId")=="RS121"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: close M7 C02 coverage and program review"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
     "RS121 must be a durable successful attempt 001.");
var failures=report.GetProperty("priorInvocationFailures");
Need(failures.GetProperty("count").GetInt32()==0&&
     failures.GetProperty("preparationFailureCount").GetInt32()==0&&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
     Flag(failures,"firstPassSucceeded"),
     "RS121 is a real first-pass; cannot misattribute prior RS120 failures.");
Need(Text(c01t1,"status")==
         "m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified"&&
     Flag(c01t1,"t1_done")&&!Flag(c01t1,"t2_done")&&
     !Flag(c01t1,"external_prover_adopted")&&
     !Flag(c01t1,"public_formal_api_delivered"),
     "C01 T1 finite exact certificate cannot become general formal product.");
Need(Text(c01t2,"status")==
         "m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified"&&
     Flag(c01t2,"t1_done")&&Flag(c01t2,"t2_done")&&
     Flag(c01t2,"c01_phase_done")&&
     !Flag(c01t2,"external_prover_adopted")&&
     !Flag(c01t2,"public_formal_api_delivered")&&
     !Flag(c01t2,"m7_release_done"),
     "Formal provider-deferral C01 evidence is not release evidence.");
Need(Text(c02t1,"status")==
         "m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified"&&
     Flag(c02t1,"t1_done")&&!Flag(c02t1,"c02_parent_done")&&
     !Flag(c02t1,"external_responses_confirmed")&&
     !Flag(c02t1,"external_requests_transmitted"),
     "C02 T1 is an inventory without external confirmations.");
Need(Text(c02t2,"status")==
         "m7-w02-c02-t2-documentary-coverage-program-boundary-qualified"&&
     Flag(c02t2,"t1_done")&&Flag(c02t2,"t2_done")&&
     Flag(c02t2,"c02_document_phase_done")&&
     !Flag(c02t2,"m7_w02_work_package_done")&&
     !Flag(c02t2,"m7_release_done")&&
     !Flag(c02t2,"future_program_approved")&&
     !Flag(c02t2,"new_public_product_delivered")&&
     !Flag(c02t2,"external_requests_transmitted"),
     "C02 T2 is completed as document only, not as released product.");
Need(Text(w01,"status")==
         "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"&&
     Flag(w01,"m7_w01_work_package_done")&&
     !Flag(w01,"new_advanced_product_delivered")&&
     !Flag(w01,"m7_release_done")&&
     w01.GetProperty("source_pinned_receipts").GetInt32()==10,
     "Prior advanced work package stays research only.");
foreach(var (doc,items) in new[]{
    (c01t1,new[]{
        ("fixture_sha256","docs/planning/fixtures/m7-polynomial-identity-certificate.json"),
        ("decision_sha256","docs/planning/decisions/m7-w02-c01-t1.md"),
        ("screen_sha256","docs/distribution/m7-formal-provider-screen.md"),
        ("validator_sha256","docs/planning/ValidateM7FormalCertificate.cs")}),
    (c01t2,new[]{
        ("decision_sha256","docs/planning/decisions/m7-w02-c01-t2.md"),
        ("distribution_sha256","docs/distribution/m7-formal-toolchain-deferral.md"),
        ("boundary_validator_sha256","docs/planning/ValidateM7FormalBoundary.cs")}),
    (c02t1,new[]{
        ("decision_sha256","docs/planning/decisions/m7-w02-c02-t1.md"),
        ("distribution_sha256","docs/distribution/m7-coverage-ownership-inventory.md"),
        ("validator_sha256","docs/planning/ValidateM7CoverageInventory.cs")}),
    (c02t2,new[]{
        ("decision_sha256","docs/planning/decisions/m7-w02-c02.md"),
        ("distribution_sha256","docs/distribution/m7-coverage-program-disposition.md"),
        ("validator_sha256","docs/planning/ValidateM7CoverageProgram.cs")}),
    (w01,new[]{
        ("decision_sha256","docs/planning/decisions/m7-w01-research-closure.md"),
        ("distribution_sha256","docs/distribution/m7-w01-advanced-research-matrix.md"),
        ("validator_sha256","docs/planning/ValidateM7AdvancedWorkPackage.cs")})
})
    foreach(var (field,path) in items)
        Need(Text(doc,field)==Sha(path),"Prior SHA identity changed: "+path);

var releases=backlog.GetProperty("releases").EnumerateArray().ToArray();
var packages=backlog.GetProperty("work_packages").EnumerateArray().ToArray();
var chunks=backlog.GetProperty("chunks").EnumerateArray().ToArray();
Need(releases.Length==8&&packages.Length==17&&chunks.Length==58&&
     chunks.All(x=>Text(x,"status")=="done"),
     "Accepted M7 chunks must be complete before parent W02 closure.");
Need(releases.Count(x=>Text(x,"status")=="done")==7&&
     releases.Count(x=>Text(x,"status")=="in_progress")==1&&
     packages.Count(x=>Text(x,"status")=="done")==(target?17:16)&&
     packages.Count(x=>Text(x,"status")=="in_progress")==(target?0:1),
     "Only W02 work-package status may complete; M7 stays open.");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"done");
State(Find(backlog.GetProperty("work_packages"),"M7-W02"),
      target?"done":"in_progress");
var nativeM7=Find(native.GetProperty("milestones"),"M7");
var nativeW02=Find(nativeM7.GetProperty("workPackages"),"M7-W02");
State(nativeM7,"in-progress","state");
State(Find(nativeM7.GetProperty("workPackages"),"M7-W01"),"done","state");
State(nativeW02,target?"done":"in-progress","state");
var phases=nativeW02.GetProperty("phases").EnumerateArray().ToArray();
Need(phases.Length==2,"Accepted W02 phase count differs.");
foreach(var phase in phases)
{
    State(phase,"done","state");
    var tasks=phase.GetProperty("tasks").EnumerateArray().ToArray();
    Need(tasks.Length==2,"All four W02 Q/O/D tasks must be present.");
    foreach(var task in tasks)
    {
        State(task,"done","state");
        var sub=task.GetProperty("subtasks").EnumerateArray().ToArray();
        Need(sub.Length==3,"Each W02 task requires all Q/O/D child receipts.");
        foreach(var child in sub) State(child,"done","state");
    }
}
Need(native.GetProperty("milestones").EnumerateArray().Sum(Count)==541,
    "Accepted 541-node DWF roadmap must not change.");
foreach(var phase in chunks.Where(x=>Text(x,"work_package")=="M7-W02"))
{
    State(phase,"done");
    foreach(var task in phase.GetProperty("tasks").EnumerateArray())
    {
        State(task,"done");
        foreach(var child in task.GetProperty("subtasks").EnumerateArray())
            State(child,"done");
    }
}

var requirements=backlog.GetProperty("requirements").EnumerateArray().ToArray();
var ownerIds=chunks.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
var unique=new HashSet<string>(StringComparer.Ordinal);
Need(requirements.Length==106&&
     requirements.All(x=>unique.Add(Text(x,"id"))&&
         ownerIds.Contains(Text(x,"owner"))),
     "Requirement ownership drifted.");
Need(backlog.GetProperty("source_coverage").GetArrayLength()==20&&
     backlog.GetProperty("dispositions").GetArrayLength()==6&&
     backlog.GetProperty("external_dependencies").GetArrayLength()==4,
     "Canonical source or disposition totals drifted.");

Has(Read(Requests),"have not been sent","Coordination README");
foreach(var (path,token) in new[]{
    (Aura,"MATH-XR-001"),(DecisionRequest,"MATH-XR-002"),(Mcdm,"MATH-XR-003")
})
{
    var body=Read(path);
    Has(body,token,"Unsent draft");
    Has(body,"draft","Unsent draft");
    Has(body,"Transmission","Unsent draft");
    Has(body,"none","Unsent draft");
}
var decision=Read(Decision);
foreach(var part in new[]{
    "## Question","## Accepted scope and observations",
    "## Independent qualification and refusal gates",
    "## Work-package decision","## Next milestone gate",
    "C01","C02","three","unsent"
}) Has(decision,part,"Work-package decision");
var matrix=Read(Matrix);
foreach(var part in new[]{
    "## Native scope and reproducibility",
    "## Ownership and remaining external debt",
    "## Work-package versus milestone gate",
    "541","unsent","M7"
}) Has(matrix,part,"Work-package evidence matrix");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var e=evidenceDoc.RootElement;
    Need(Text(e,"status")==Status&&
         e.GetProperty("native_nodes_retained").GetInt32()==541&&
         e.GetProperty("qualified_phases").GetInt32()==2&&
         e.GetProperty("qualified_tasks").GetInt32()==4&&
         e.GetProperty("qualified_subtasks").GetInt32()==12&&
         e.GetProperty("source_pinned_receipts").GetInt32()==4&&
         e.GetProperty("unsent_requests").GetInt32()==3&&
         Flag(e,"m7_w02_work_package_done")&&
         !Flag(e,"m7_release_done")&&
         !Flag(e,"external_requests_transmitted")&&
         !Flag(e,"external_approvals_received")&&
         !Flag(e,"new_public_product_delivered")&&
         !Flag(e,"publicly_published")&&
         Text(e,"decision_sha256")==Sha(Decision)&&
         Text(e,"distribution_sha256")==Sha(Matrix)&&
         Text(e,"validator_sha256")==Sha(
             "docs/planning/ValidateM7ProofsWorkPackage.cs"),
         "Target work-package receipt must not imply M7 release or client adoption.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
