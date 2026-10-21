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
const string PriorReport=".aura/workflow/reports/RS122/attempt-001.json";
const string Advanced="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Formal="docs/planning/evidence/M7-W02-work-package-qualified.json";
const string FormalT2="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string CoverageT2="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-internal-gate-closure.md";
const string Matrix="docs/distribution/m7-internal-gate-matrix.md";
const string Evidence="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Requests="docs/coordination/requests/README.md";
const string Aura="docs/coordination/requests/MATH-XR-001-aura.md";
const string DecisionRequest="docs/coordination/requests/MATH-XR-002-decision.md";
const string Mcdm="docs/coordination/requests/MATH-XR-003-mcdm.md";
const string Status="m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld";
const string Marker="dw.tools.math/m7-internal-scope-gate/1.2 qualified; milestones=8; chunks=58; m7=research-done; public-release=false; external=unsent";
const string Counts="M7-GATE:MILESTONES=8;PACKAGES=17;CHUNKS=58;REQUIREMENTS=106;SOURCES=20;DISPOSITIONS=6;NODES=541";
const string Scope="M7-GATE:INTERNAL=DONE;PUBLICATION=NOT-AUTHORIZED;CLIENT-ADOPTION=false;PROVIDERS=DEFERRED;PROGRAM=NOT-APPROVED";

static void Need(bool condition,string reason)
{
    if(!condition) throw new InvalidOperationException("M7 internal scope gate: "+reason);
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
static string Text(JsonElement node,string field)=>
    node.GetProperty(field).GetString() ??
        throw new InvalidOperationException("Missing text property "+field);
static bool Flag(JsonElement node,string key)=>node.GetProperty(key).GetBoolean();
static JsonElement Find(JsonElement array,string id)=>
    array.EnumerateArray().Single(item=>Text(item,"id")==id);
static void State(JsonElement node,string expected,string property="status")=>
    Need(Text(node,property)==expected,
        "Incorrect "+property+" for "+Text(node,"id")+" expected "+expected);
static void Pin(JsonElement receipt,string field,string path)=>
    Need(Text(receipt,field)==Sha(path),"Source SHA drift: "+path);
static void Has(string source,string phrase,string owner)=>
    Need(source.Contains(phrase,StringComparison.OrdinalIgnoreCase),
        owner+" lacks "+phrase);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int NotDone(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(NotDone);
    return n;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Checker is read-only; use --check only.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(PriorReport);
using var advancedDoc=Open(Advanced);
using var formalDoc=Open(Formal);
using var formalT2Doc=Open(FormalT2);
using var coverageT2Doc=Open(CoverageT2);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var advanced=advancedDoc.RootElement;
var formal=formalDoc.RootElement;
var formalT2=formalT2Doc.RootElement;
var coverageT2=coverageT2Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")==(target?"0.1.122":"0.1.121")&&
     Text(backlog,"status")==(target?"done":"in_progress"),
     "Backlog is not exact RS122 baseline / RS123 target.");
Need(Text(report,"runId")=="RS122"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify M7 W02 research and document closure"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
     "Previous RS122 success receipt is not authoritative.");
var refusals=report.GetProperty("priorInvocationFailures");
Need(refusals.GetProperty("count").GetInt32()==0&&
     refusals.GetProperty("preparationFailureCount").GetInt32()==0&&
     refusals.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
     Flag(refusals,"firstPassSucceeded"),
     "RS122 first-pass truth changed.");

Need(Text(advanced,"status")==
         "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"&&
     Flag(advanced,"m7_w01_work_package_done")&&
     Flag(advanced,"research_only")&&
     advanced.GetProperty("source_pinned_receipts").GetInt32()==10&&
     advanced.GetProperty("review_family_count").GetInt32()==4&&
     !Flag(advanced,"new_advanced_product_delivered")&&
     !Flag(advanced,"publicly_published"),
     "M7 advanced model research is not public product evidence.");
foreach(var flag in new[]{
    "precision_provider_deferred","tensor_provider_deferred",
    "dag_runtime_deferred","plot_model_deferred"
}) Need(Flag(advanced,flag),"An advanced provider deferral was erased: "+flag);
Need(Text(formal,"status")==
         "m7-w02-formal-research-and-document-portfolio-qualified-external-products-deferred"&&
     Flag(formal,"m7_w02_work_package_done")&&
     !Flag(formal,"m7_release_done")&&
     !Flag(formal,"advanced_provider_adopted")&&
     !Flag(formal,"general_proof_kernel_certified")&&
     !Flag(formal,"public_formal_api_delivered")&&
     !Flag(formal,"external_requests_transmitted")&&
     !Flag(formal,"external_approvals_received")&&
     !Flag(formal,"new_public_product_delivered")&&
     !Flag(formal,"publicly_published")&&
     !Flag(formal,"future_program_approved")&&
     formal.GetProperty("qualified_phases").GetInt32()==2&&
     formal.GetProperty("qualified_tasks").GetInt32()==4&&
     formal.GetProperty("qualified_subtasks").GetInt32()==12&&
     formal.GetProperty("native_nodes_retained").GetInt32()==541,
     "M7 W02 work package must stay research/document with no published provider.");
Need(Text(formalT2,"status")==
         "m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified"&&
     Flag(formalT2,"c01_phase_done")&&
     !Flag(formalT2,"external_prover_adopted")&&
     !Flag(formalT2,"public_formal_api_delivered"),
     "Finite formal C01 research cannot become a proof-kernel claim.");
Need(Text(coverageT2,"status")==
         "m7-w02-c02-t2-documentary-coverage-program-boundary-qualified"&&
     Flag(coverageT2,"c02_document_phase_done")&&
     !Flag(coverageT2,"external_approvals_received")&&
     !Flag(coverageT2,"external_requests_transmitted")&&
     !Flag(coverageT2,"future_program_approved")&&
     !Flag(coverageT2,"new_public_product_delivered")&&
     coverageT2.GetProperty("unsent_requests").GetInt32()==3,
     "Coverage C02 is a local document, not recipient approval.");
foreach(var (receipt,field,path) in new[]{
    (advanced,"decision_sha256","docs/planning/decisions/m7-w01-research-closure.md"),
    (advanced,"distribution_sha256","docs/distribution/m7-w01-advanced-research-matrix.md"),
    (advanced,"validator_sha256","docs/planning/ValidateM7AdvancedWorkPackage.cs"),
    (formal,"decision_sha256","docs/planning/decisions/m7-w02-work-package-closure.md"),
    (formal,"distribution_sha256","docs/distribution/m7-w02-work-package-research-matrix.md"),
    (formal,"validator_sha256","docs/planning/ValidateM7ProofsWorkPackage.cs"),
    (formalT2,"decision_sha256","docs/planning/decisions/m7-w02-c01-t2.md"),
    (formalT2,"boundary_validator_sha256","docs/planning/ValidateM7FormalBoundary.cs"),
    (coverageT2,"decision_sha256","docs/planning/decisions/m7-w02-c02.md"),
    (coverageT2,"validator_sha256","docs/planning/ValidateM7CoverageProgram.cs")
}) Pin(receipt,field,path);

var releases=backlog.GetProperty("releases").EnumerateArray().ToArray();
var packages=backlog.GetProperty("work_packages").EnumerateArray().ToArray();
var chunks=backlog.GetProperty("chunks").EnumerateArray().ToArray();
Need(releases.Length==8&&packages.Length==17&&chunks.Length==58,
     "Accepted roadmap dimensions changed.");
Need(releases.Count(x=>Text(x,"status")=="done")==(target?8:7)&&
     releases.Count(x=>Text(x,"status")=="in_progress")==(target?0:1)&&
     packages.All(x=>Text(x,"status")=="done")&&
     chunks.All(x=>Text(x,"status")=="done"),
     "All previously accepted products/decisions must be reconciled.");
var m7=Find(backlog.GetProperty("releases"),"M7");
State(m7,target?"done":"in_progress");
Need(Text(m7,"version")=="1.2.0-candidate"&&Text(m7,"gate")=="G-M7"&&
     m7.GetProperty("depends_on").EnumerateArray()
         .Any(x=>x.GetString()=="M6"),
     "M7 internal functional gate differs from accepted product definition.");
var acceptedChunks=m7.GetProperty("required_chunks").EnumerateArray()
    .Select(x=>x.GetString()??"").ToArray();
var observedChunks=chunks.Where(x=>Text(x,"release")=="M7")
    .Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
Need(acceptedChunks.Length==7&&
     observedChunks.SetEquals(acceptedChunks)&&
     chunks.Count(x=>Text(x,"release")=="M7"&&Text(x,"kind")=="spike")==6&&
     chunks.Count(x=>Text(x,"release")=="M7"&&Text(x,"kind")=="document")==1,
     "Seven accepted M7 scope decisions were changed.");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"done");
State(Find(backlog.GetProperty("work_packages"),"M7-W02"),"done");
Need(releases.Where(x=>Text(x,"id")!="M7")
        .All(x=>Text(x,"status")=="done"),
     "Earlier accepted milestones are not all complete.");

var reqs=backlog.GetProperty("requirements").EnumerateArray().ToArray();
var ownerIds=chunks.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
var releaseIds=releases.Select(x=>Text(x,"id")).ToHashSet(StringComparer.Ordinal);
var reqIds=new HashSet<string>(StringComparer.Ordinal);
var owners=new HashSet<string>(StringComparer.Ordinal);
Need(reqs.Length==106,"Accepted requirement count changed.");
foreach(var r in reqs)
{
    var id=Text(r,"id");
    var owner=Text(r,"owner");
    var release=Text(r,"release");
    Need(reqIds.Add(id)&&ownerIds.Contains(owner)&&releaseIds.Contains(release)&&
         chunks.Any(x=>Text(x,"id")==owner&&Text(x,"release")==release)&&
         !string.IsNullOrWhiteSpace(Text(r,"acceptance")),
         "Missing accepted owner, acceptance or repeated requirement: "+id);
    owners.Add(owner);
}
Need(owners.Count==53,"Unique accepted requirement owners drifted.");
var sources=backlog.GetProperty("source_coverage").EnumerateArray().ToArray();
var sourceIds=new HashSet<string>(StringComparer.Ordinal);
Need(sources.Length==20,"Framing source roster differs.");
foreach(var row in sources)
{
    var id=Text(row,"source_id");
    var targets=row.GetProperty("targets").EnumerateArray()
        .Select(x=>x.GetString()??"").ToArray();
    Need(sourceIds.Add(id)&&targets.Length>0&&
         targets.All(ownerIds.Contains)&&
         targets.Distinct(StringComparer.Ordinal).Count()==targets.Length,
         "Source coverage is ambiguous or points to unknown chunks: "+id);
}
var dispositions=backlog.GetProperty("dispositions").EnumerateArray().ToArray();
Need(dispositions.Length==6&&
     dispositions.Count(x=>Text(x,"status")=="externally_owned")==4&&
     dispositions.Count(x=>Text(x,"status")=="excluded")==2,
     "External owners and exclusions changed.");
foreach(var d in dispositions)
    Need(!string.IsNullOrWhiteSpace(Text(d,"owner"))&&
         !string.IsNullOrWhiteSpace(Text(d,"reason"))&&
         !string.IsNullOrWhiteSpace(Text(d,"review_trigger")),
         "Disposition lacks owner, reason or trigger: "+Text(d,"id"));
var localGates=backlog.GetProperty("external_dependencies").EnumerateArray().ToArray();
Need(localGates.Length==4&&localGates.All(x=>Text(x,"status")=="satisfied"),
     "Math-local dependency gates were changed.");

var nativeM7=Find(native.GetProperty("milestones"),"M7");
State(nativeM7,target?"done":"in-progress","state");
var milestoneNodes=native.GetProperty("milestones").EnumerateArray().ToArray();
Need(milestoneNodes.Length==8&&
     milestoneNodes.Sum(Count)==541&&
     milestoneNodes.Sum(NotDone)==(target?0:1),
     "Native 541-node acceptance or only-parent M7 completion drifted.");
foreach(var wp in nativeM7.GetProperty("workPackages").EnumerateArray())
{
    State(wp,"done","state");
    foreach(var phase in wp.GetProperty("phases").EnumerateArray())
    {
        State(phase,"done","state");
        foreach(var task in phase.GetProperty("tasks").EnumerateArray())
        {
            State(task,"done","state");
            foreach(var child in task.GetProperty("subtasks").EnumerateArray())
                State(child,"done","state");
        }
    }
}
Need(nativeM7.GetProperty("workPackages").GetArrayLength()==2,
     "M7 accepted W01/W02 work packages changed.");

Has(Read(Requests),"have not been sent","Coordination README");
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
foreach(var term in new[]{
    "## Question","## Source-pinned observations",
    "## Functional G-M7 decision","## Publication and owner authorization",
    "## Future scope options","## Independent gate and cost",
    "1.2.0-candidate","NOT authorized","unsent"
}) Has(decision,term,"Internal milestone gate decision");
var matrix=Read(Matrix);
foreach(var term in new[]{
    "## Completed accepted scope",
    "## Publication and recipient gate matrix",
    "## Negative assertions and reproducibility",
    "541","106","unsent","NOT authorized"
}) Has(matrix,term,"Internal scope and publication matrix");

if(target)
{
    using var receiptDoc=Open(Evidence);
    var receipt=receiptDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("milestones_done").GetInt32()==8&&
         receipt.GetProperty("work_packages_done").GetInt32()==17&&
         receipt.GetProperty("chunks_done").GetInt32()==58&&
         receipt.GetProperty("requirements_checked").GetInt32()==106&&
         receipt.GetProperty("source_rows_checked").GetInt32()==20&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         receipt.GetProperty("unsent_requests").GetInt32()==3&&
         Flag(receipt,"m7_internal_scope_done")&&
         !Flag(receipt,"public_release_authorized")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"future_program_approved")&&
         !Flag(receipt,"new_advanced_public_product_delivered")&&
         !Flag(receipt,"general_formal_kernel_certified")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(
             "docs/planning/ValidateM7MilestoneGate.cs"),
         "Target receipt implies forbidden publication/provider/recipient approval.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
