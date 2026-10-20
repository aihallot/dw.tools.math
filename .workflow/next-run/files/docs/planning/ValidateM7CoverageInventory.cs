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
const string Prior="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Previous="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-c02-t1.md";
const string Screen="docs/distribution/m7-coverage-ownership-inventory.md";
const string Evidence="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string Requests="docs/coordination/requests/README.md";
const string Aura="docs/coordination/requests/MATH-XR-001-aura.md";
const string DecisionRequest="docs/coordination/requests/MATH-XR-002-decision.md";
const string Mcdm="docs/coordination/requests/MATH-XR-003-mcdm.md";
const string Phase="M7-W02-C02";
const string Task="M7-W02-C02-T1";
const string Status="m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified";
const string Marker="dw.tools.math/m7-w02-c02-t1-independent-coverage/1.2 qualified; requirements=106; sources=20; dispositions=6; drafts=3; external-ack=none";
const string CountsMarker="M7-COVERAGE:REQUIREMENTS=106;OWNERS=53;SOURCES=20;DISPOSITIONS=6;EXTERNAL-DEPS=4;DRAFTS=3";
const string ScopeMarker="M7-COVERAGE:SCOPE:CHUNKS=58;DONE=57;W02-C02=IN-PROGRESS;M7=OPEN;PUBLIC-PRODUCT=false";

static void Need(bool value,string reason)
{
    if(!value) throw new InvalidOperationException("Independent Math coverage inventory: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal)||relative==Native,
        "Read-only checker accepts only Math planning/evidence and coordination documents.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
        !relative.Contains('\\'),"Unsafe local path: "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Local(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Sha(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(relative))))
        .ToLowerInvariant();
static string Text(JsonElement node,string property)=>
    node.GetProperty(property).GetString() ??
        throw new InvalidOperationException("Missing text "+property);
static JsonElement Find(JsonElement items,string id)=>
    items.EnumerateArray().Single(item=>Text(item,"id")==id);
static void State(JsonElement node,string expected,string property="status")=>
    Need(Text(node,property)==expected,
        "Unexpected "+property+" for "+Text(node,"id")+"; expected "+expected);
static bool Flag(JsonElement obj,string key)=>obj.GetProperty(key).GetBoolean();
static void Has(string body,string term,string owner)=>
    Need(body.Contains(term,StringComparison.OrdinalIgnoreCase),owner+" lacks "+term);
static int Count(JsonElement node)
{
    var total=1;
    foreach(var name in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(name,out var items))
            total+=items.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check mode is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var previousDoc=Open(Previous);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var previous=previousDoc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")==(target?"0.1.119":"0.1.118"),
    "Unexpected exact RS119 baseline or RS120 target plan version.");
Need(Text(previous,"status")==
    "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished" &&
    Flag(previous,"m7_w01_work_package_done") &&
    !Flag(previous,"m7_release_done") &&
    !Flag(previous,"new_advanced_product_delivered") &&
    previous.GetProperty("source_pinned_receipts").GetInt32()==10,
    "M7-W01 previous portfolio qualified only as research.");
Need(Text(prior,"status")==
    "m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified" &&
    Text(prior,"disposition")==
    "defer-unqualified-formal-proof-providers-and-public-api" &&
    Flag(prior,"c01_phase_done") &&
    !Flag(prior,"m7_w02_work_package_done") &&
    !Flag(prior,"m7_release_done") &&
    !Flag(prior,"public_formal_api_delivered") &&
    !Flag(prior,"external_prover_adopted"),
    "Formal source research must remain provider-deferred and not released.");
foreach(var (field,path) in new[]
{
    ("validator_sha256","docs/planning/ValidateM7AdvancedWorkPackage.cs"),
    ("decision_sha256","docs/planning/decisions/m7-w01-research-closure.md"),
    ("distribution_sha256","docs/distribution/m7-w01-advanced-research-matrix.md")
})
    Need(Text(previous,field)==Sha(path),
        "Prior advanced research source SHA identity changed: "+path);
foreach(var (field,path) in new[]
{
    ("boundary_validator_sha256","docs/planning/ValidateM7FormalBoundary.cs"),
    ("decision_sha256","docs/planning/decisions/m7-w02-c01-t2.md"),
    ("distribution_sha256","docs/distribution/m7-formal-toolchain-deferral.md")
})
    Need(Text(prior,field)==Sha(path),
        "Formal C01 deferral source SHA identity changed: "+path);

var chunks=backlog.GetProperty("chunks").EnumerateArray().ToArray();
var releases=backlog.GetProperty("releases").EnumerateArray().ToArray();
var work=backlog.GetProperty("work_packages").EnumerateArray().ToArray();
var chunkIds=chunks.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
var releaseIds=releases.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
Need(chunkIds.Count==58&&chunks.Length==58&&releaseIds.Count==8&&
    work.Length==17,"Canonical accepted Math planning roster changed.");
Need(releases.Count(x=>Text(x,"status")=="done")==7 &&
    releases.Count(x=>Text(x,"status")=="in_progress")==1,
    "Seven prior Math milestones must remain done while M7 stays open.");
Need(work.Count(x=>Text(x,"status")=="done")==16&&
    work.Count(x=>Text(x,"status")=="in_progress")==1,
    "W02 remains the sole active work package; earlier sixteen are done.");
var completed=chunks.Count(x=>Text(x,"status")=="done");
Need(completed==57 && chunks.Length-completed==1,
    "Coverage audit must distinguish exactly one incomplete C02 document.");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"done");
State(Find(backlog.GetProperty("work_packages"),"M7-W02"),"in_progress");
State(Find(backlog.GetProperty("chunks"),"M7-W02-C01"),"done");
var next=Find(backlog.GetProperty("chunks"),Phase);
State(next,target?"in_progress":"planned");
Need(Text(next,"kind")=="document"&&Text(next,"size")=="M",
    "Last C02 remains the bounded accepted owner review document.");
var t1=Find(next.GetProperty("tasks"),Task);
var t2=Find(next.GetProperty("tasks"),"M7-W02-C02-T2");
State(t1,target?"done":"planned");
State(t2,"planned");

var m7=Find(native.GetProperty("milestones"),"M7");
var w02=Find(m7.GetProperty("workPackages"),"M7-W02");
var c01=Find(w02.GetProperty("phases"),"M7-W02-C01");
var phase=Find(w02.GetProperty("phases"),Phase);
State(m7,"in-progress","state");
State(Find(m7.GetProperty("workPackages"),"M7-W01"),"done","state");
State(w02,"in-progress","state");
State(c01,"done","state");
State(phase,target?"in-progress":"not-ready","state");
var nativeT1=Find(phase.GetProperty("tasks"),Task);
var nativeT2=Find(phase.GetProperty("tasks"),"M7-W02-C02-T2");
State(nativeT1,target?"done":"not-ready","state");
State(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(Find(t1.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"planned");
    State(Find(nativeT1.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"not-ready","state");
    State(Find(t2.GetProperty("subtasks"),"M7-W02-C02-T2-"+suffix),"planned");
    State(Find(nativeT2.GetProperty("subtasks"),"M7-W02-C02-T2-"+suffix),
        "not-ready","state");
}
Need(native.GetProperty("milestones").EnumerateArray().Sum(Count)==541,
    "The accepted 541-node DWF research roadmap cannot be shrunk.");

var requirements=backlog.GetProperty("requirements").EnumerateArray().ToArray();
Need(requirements.Length==106,"Expected canonical 106 accepted requirement IDs.");
var reqIds=new HashSet<string>(StringComparer.Ordinal);
var owners=new HashSet<string>(StringComparer.Ordinal);
foreach(var requirement in requirements)
{
    var id=Text(requirement,"id");
    var owner=Text(requirement,"owner");
    var release=Text(requirement,"release");
    Need(!string.IsNullOrWhiteSpace(id)&&reqIds.Add(id)&&chunkIds.Contains(owner) &&
         releaseIds.Contains(release) &&
         chunks.Any(x=>Text(x,"id")==owner&&Text(x,"release")==release) &&
         !string.IsNullOrWhiteSpace(Text(requirement,"title")) &&
         !string.IsNullOrWhiteSpace(Text(requirement,"acceptance")),
         "Requirement has ambiguous, duplicate or non-existent owner/release: "+id);
    owners.Add(owner);
}
Need(owners.Count==53,"Original 53 accepted owning product chunks must remain identifiable.");
Console.WriteLine("M7-COVERAGE:REQUIREMENTS=106;OWNERS=53;SOURCES=20;DISPOSITIONS=6;EXTERNAL-DEPS=4;DRAFTS=3");

var sources=backlog.GetProperty("source_coverage").EnumerateArray().ToArray();
var ids=new HashSet<string>(StringComparer.Ordinal);
Need(sources.Length==20,"Framing source-coverage entries changed.");
foreach(var source in sources)
{
    var id=Text(source,"source_id");
    Need(!string.IsNullOrWhiteSpace(id)&&ids.Add(id)&&
         !string.IsNullOrWhiteSpace(Text(source,"title")) &&
         !string.IsNullOrWhiteSpace(Text(source,"disposition")),
         "Unowned or duplicate source-coverage row "+id);
    var targets=source.GetProperty("targets").EnumerateArray()
        .Select(x=>x.GetString()??"").ToArray();
    Need(targets.Length>0&&targets.All(x=>chunkIds.Contains(x)) &&
         targets.Distinct(StringComparer.Ordinal).Count()==targets.Length,
         "Framing source target references unknown or repeated product chunk: "+id);
}
var dispositions=backlog.GetProperty("dispositions").EnumerateArray().ToArray();
var dispositionIds=new HashSet<string>(StringComparer.Ordinal);
Need(dispositions.Length==6 &&
    dispositions.Count(x=>Text(x,"status")=="externally_owned")==4 &&
    dispositions.Count(x=>Text(x,"status")=="excluded")==2,
    "Expected four external ownership and two explicit exclusions.");
foreach(var item in dispositions)
{
    var id=Text(item,"id");
    Need(dispositionIds.Add(id)&&
         !string.IsNullOrWhiteSpace(Text(item,"owner")) &&
         !string.IsNullOrWhiteSpace(Text(item,"reason")) &&
         !string.IsNullOrWhiteSpace(Text(item,"review_trigger")) &&
         item.GetProperty("targets").EnumerateArray().All(x=>
             chunkIds.Contains(x.GetString()??"")),
         "Missing owner, reason, review trigger or valid target in "+id);
}
var external=backlog.GetProperty("external_dependencies").EnumerateArray().ToArray();
Need(external.Length==4&&external.All(x=>Text(x,"status")=="satisfied"),
    "Four Math-side evidence gates must be locally qualified.");
foreach(var item in external)
{
    var id=Text(item,"id");
    var recorded=item.GetProperty("evidence").EnumerateArray().Select(x=>
        x.GetString()??"").ToArray();
    Need(!string.IsNullOrWhiteSpace(Text(item,"owner")) &&
         !string.IsNullOrWhiteSpace(Text(item,"acceptance")) &&
         recorded.Length>0 &&
         recorded.All(x=>x.StartsWith("docs/",StringComparison.Ordinal)&&
             File.Exists(Local(x))),
         "Local dependency has missing owner or evidence: "+id);
}
var requestIntro=Read(Requests);
Has(requestIntro,"have not been sent","Coordination README");
foreach(var (path,token) in new[]{
    (Aura,"MATH-XR-001"),(DecisionRequest,"MATH-XR-002"),(Mcdm,"MATH-XR-003")
})
{
    var request=Read(path);
    Has(request,token,"Owner request draft");
    Has(request,"draft","Owner request draft");
    Has(request,"Transmission","Owner request draft");
    Has(request,"none","Owner request draft");
}
var decision=Read(Decision);
var screen=Read(Screen);
foreach(var term in new[]{"## Question","## Verified canonical observations",
    "106 accepted requirements","53 distinct","20 framing",
    "6 explicit","4 satisfied","3 coordination requests",
    "## T1 decision","## Candidate future program",
    "M7-W02-C02-T2","AURA"})
    Has(decision,term,"Coverage Q/O/D decision");
foreach(var term in new[]{"## Structural and provenance counts",
    "## Explicit ownership/disposition matrix",
    "## Pending external requests",
    "## Deferred research families",
    "No release decision","source-free","541 nodes"})
    Has(screen,term,"Coverage/ownership distribution");

Console.WriteLine("M7-COVERAGE:SCOPE:CHUNKS=58;DONE=57;W02-C02=IN-PROGRESS;M7=OPEN;PUBLIC-PRODUCT=false");
if(target)
{
    using var evidenceDoc=Open(Evidence);
    var evidence=evidenceDoc.RootElement;
    Need(Get(evidence,"status")==Status &&
         evidence.GetProperty("requirements_checked").GetInt32()==106 &&
         evidence.GetProperty("distinct_requirement_owners").GetInt32()==53 &&
         evidence.GetProperty("source_coverage_rows").GetInt32()==20 &&
         evidence.GetProperty("external_owned_dispositions").GetInt32()==4 &&
         evidence.GetProperty("excluded_dispositions").GetInt32()==2 &&
         evidence.GetProperty("local_dependency_gates_satisfied").GetInt32()==4 &&
         evidence.GetProperty("unsent_draft_requests").GetInt32()==3 &&
         evidence.GetProperty("native_nodes_retained").GetInt32()==541 &&
         Flag(evidence,"t1_done") && !Flag(evidence,"t2_done") &&
         !Flag(evidence,"c02_parent_done") &&
         !Flag(evidence,"m7_w02_work_package_done") &&
         !Flag(evidence,"m7_release_done") &&
         !Flag(evidence,"external_responses_confirmed") &&
         !Flag(evidence,"public_product_delivered") &&
         !Flag(evidence,"external_requests_transmitted") &&
         Get(evidence,"decision_sha256")==Sha(Decision) &&
         Get(evidence,"distribution_sha256")==Sha(Screen) &&
         Get(evidence,"validator_sha256")==Sha(
             "docs/planning/ValidateM7CoverageInventory.cs"),
         "Completed coverage T1 evidence may not claim external responses or product delivery.");
}
Console.WriteLine(Marker);
return 0;
