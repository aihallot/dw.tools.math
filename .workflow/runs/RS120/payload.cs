using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W02",Phase="M7-W02-C02";
const string Task="M7-W02-C02-T1";
const string Question="M7-W02-C02-T1-A";
const string Observation="M7-W02-C02-T1-B";
const string DecisionNode="M7-W02-C02-T1-C";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Previous="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-c02-t1.md";
const string Screen="docs/distribution/m7-coverage-ownership-inventory.md";
const string Validator="docs/planning/ValidateM7CoverageInventory.cs";
const string Qualified="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string Status="m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified";
const string Marker="dw.tools.math/m7-w02-c02-t1-independent-coverage/1.2 qualified; requirements=106; sources=20; dispositions=6; drafts=3; external-ack=none";
const string Counts="M7-COVERAGE:REQUIREMENTS=106;OWNERS=53;SOURCES=20;DISPOSITIONS=6;EXTERNAL-DEPS=4;DRAFTS=3";
const string Scopes="M7-COVERAGE:SCOPE:CHUNKS=58;DONE=57;W02-C02=IN-PROGRESS;M7=OPEN;PUBLIC-PRODUCT=false";
const string RequestReadme="docs/coordination/requests/README.md";
const string RequestAura="docs/coordination/requests/MATH-XR-001-aura.md";
const string RequestDecision="docs/coordination/requests/MATH-XR-002-decision.md";
const string RequestMcdm="docs/coordination/requests/MATH-XR-003-mcdm.md";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS120",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS119",1,
    "docs: qualify formal provider deferral and close M7 C01",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.118"),JsonValue.Create("0.1.119"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.119":"0.1.118"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Previous,"/status",
    JsonValue.Create("m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"));
p.Require.JsonValueEquals(Previous,"/m7_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Previous,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Previous,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Previous,"/source_pinned_receipts",JsonValue.Create(10));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified"));
p.Require.JsonValueEquals(Prior,"/disposition",
    JsonValue.Create("defer-unqualified-formal-proof-providers-and-public-api"));
p.Require.JsonValueEquals(Prior,"/c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m7_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_formal_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_prover_adopted",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");
p.Require.TextContains(RequestReadme,"have not been sent");
p.Require.TextContains(RequestAura,"MATH-XR-001");
p.Require.TextContains(RequestDecision,"MATH-XR-002");
p.Require.TextContains(RequestMcdm,"MATH-XR-003");

foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id","M7-W01"),
    ("/work_packages/15/status","done"),
    ("/work_packages/16/id",Work),
    ("/work_packages/16/status","in_progress"),
    ("/chunks/56/id","M7-W02-C01"),
    ("/chunks/56/status","done"),
    ("/chunks/57/id",Phase),
    ("/chunks/57/kind","document"),
    ("/chunks/57/size","M"),
    ("/chunks/57/status",target?"in_progress":"planned"),
    ("/chunks/57/tasks/0/id",Task),
    ("/chunks/57/tasks/0/status",target?"done":"planned"),
    ("/chunks/57/tasks/1/id","M7-W02-C02-T2"),
    ("/chunks/57/tasks/1/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/57/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/57/tasks/1/subtasks/"+i+"/status",JsonValue.Create("planned"));
}

p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W02-C02-T2","M7-W02-C02-T2-A",
    "M7-W02-C02-T2-B","M7-W02-C02-T2-C"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var file in new[]{Decision,Screen,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/requirements_checked",JsonValue.Create(106));
    p.Require.JsonValueEquals(Qualified,"/distinct_requirement_owners",JsonValue.Create(53));
    p.Require.JsonValueEquals(Qualified,"/source_coverage_rows",JsonValue.Create(20));
    p.Require.JsonValueEquals(Qualified,"/external_owned_dispositions",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/excluded_dispositions",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/local_dependency_gates_satisfied",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/unsent_draft_requests",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/native_nodes_retained",JsonValue.Create(541));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/c02_parent_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_responses_confirmed",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_product_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_requests_transmitted",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+Screen,Screen);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged coverage Q/O/D decision SHA is absent.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged coverage/owner inventory SHA is absent.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged BCL read-only coverage checker SHA is absent.");
foreach(var term in new[]{"## Question","## Verified canonical observations",
    "106 accepted requirements","53 distinct","20 framing",
    "6 explicit","4 satisfied","3 coordination requests",
    "## T1 decision","## Candidate future program","M7-W02-C02-T2"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Structural and provenance counts",
    "## Explicit ownership/disposition matrix","## Pending external requests",
    "## Deferred research families","No release decision","541 nodes"})
    p.Require.TextContains(Screen,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);
p.Require.TextContains(Validator,Counts);
p.Require.TextContains(Validator,Scopes);

var before=Required(root,"dotnet","run","--file",Validator,"--","--check");
foreach(var marker in new[]{Counts,Scopes,Marker})
    if(!before.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent source-coverage research marker missing: "+marker);
if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W02-C02-T1-qualified",
        ["status"]=Status,
        ["question"]="Does every accepted Math requirement, source and external disposition retain a truthful owner and review trigger without fabricating recipient responses or product delivery?",
        ["observation"]="The canonical backlog has 106 unique requirements with 53 chunk owners, 20 source-coverage entries, four externally-owned and two excluded dispositions, four satisfied Math-side evidence gates and three request drafts not transmitted. Seven milestones and 57 prior chunks are done; M7 and C02 remain open.",
        ["decision"]="Admit bounded source-of-truth coverage/ownership inventory only. Retain unsent external requests, provider deferrals and product nonclaims; defer program/roadmap and M7 gate decisions to distinct T2, W02 and M7 review.",
        ["previous_advanced_research_evidence"]=Previous,
        ["previous_formal_research_evidence"]=Prior,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Screen,
        ["distribution_sha256"]=screenSha,
        ["validator"]=Validator,
        ["validator_sha256"]=validatorSha,
        ["requirements_checked"]=106,
        ["distinct_requirement_owners"]=53,
        ["source_coverage_rows"]=20,
        ["external_owned_dispositions"]=4,
        ["excluded_dispositions"]=2,
        ["local_dependency_gates_satisfied"]=4,
        ["unsent_draft_requests"]=3,
        ["native_nodes_retained"]=541,
        ["milestones_previously_done"]=7,
        ["work_packages_previously_done"]=16,
        ["chunks_previously_done"]=57,
        ["source_target_missing_count"]=0,
        ["requirement_owner_missing_count"]=0,
        ["duplicate_requirement_id_count"]=0,
        ["external_responses_confirmed"]=false,
        ["external_requests_transmitted"]=false,
        ["new_milestones_approved"]=false,
        ["public_product_delivered"]=false,
        ["source_free_consumer_qualified_this_run"]=false,
        ["sibling_repository_changed"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["c02_parent_done"]=false,
        ["m7_w02_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W02-C02-T2: finalize owner coverage, external draft status, migration debt and next-program deferral/acceptance with explicit triggers; no new public package, outgoing request or release without independent owner gate.",
        ["nonclaims"]=p.Json.StringArray(
            "A backlog requirement linked to a completed spike is not a shipped mathematical implementation.",
            "Math-local dependency gates and draft AURA/Decision/MCDM requests are not recipient approvals or external replies.",
            "Potential future milestones and provider options in T1 are proposals, not an approved roadmap or authorized outbound action.",
            "M7-W02-C02 T2, work package W02 and M7 1.2.0-candidate milestone/release remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.118")
            throw new InvalidOperationException("Unexpected pre-RS120 coverage baseline.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var c01=doc["chunks"]!.AsArray()[56]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[57]!.AsObject();
        var t1=phase["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)release["id"]!=Milestone ||
           (string?)release["status"]!="in_progress" ||
           (string?)work["id"]!=Work ||
           (string?)work["status"]!="in_progress" ||
           (string?)c01["id"]!="M7-W02-C01" ||
           (string?)c01["status"]!="done" ||
           (string?)phase["id"]!=Phase ||
           (string?)phase["kind"]!="document" ||
           (string?)phase["status"]!="planned" ||
           (string?)t1["id"]!=Task ||
           (string?)t1["status"]!="planned" ||
           (string?)phase["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("Accepted coverage-owner T1 baseline drifted.");
        doc["plan_version"]="0.1.119";
        phase["status"]="in_progress";
        phase["refinement"]="RS120 T1 independently verifies 106 accepted Math requirement IDs with 53 owning chunks, 20 source-coverage targets, four external-owner and two excluded dispositions, four Math-side satisfied dependency gates and three explicit unsent AURA/Decision/MCDM drafts. Formal and advanced research remain provider-deferred; no new package, external response or owner-authorized milestone is inferred.";
        phase["readiness"]="Math owner/source coverage inventory qualified from a read-only BCL validator, canonical planning, SHA-pinned prior scientific receipts and unsent request evidence. C02 T2 must separately decide program/release boundaries and actual recipient statuses before a document-phase/work-package closure.";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Screen,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7CoverageInventory.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t1["status"]="done";
        t1["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("Coverage T1 Q/O/D child was not planned.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var id in new[]{Phase,Task,Question})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
    }
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var id in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W02-C02-T2","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS120" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 coverage inventory DWF cursor.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS120 completed target rejected by source-of-truth owner inventory.");
return p.Complete();

static string Required(string root,string command,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=command,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS120 read-only inventory validator could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS120 independent inventory command timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS120 Math source-of-truth coverage validation failed: "+
            command+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2500)]);
    return output;
}
