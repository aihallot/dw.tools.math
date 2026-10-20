using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W02",Phase="M7-W02-C02";
const string Task="M7-W02-C02-T2",Question="M7-W02-C02-T2-A";
const string Observation="M7-W02-C02-T2-B",DecisionNode="M7-W02-C02-T2-C";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string WorkResearch="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string FormalResearch="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-c02.md";
const string Distribution="docs/distribution/m7-coverage-program-disposition.md";
const string Validator="docs/planning/ValidateM7CoverageProgram.cs";
const string Qualified="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string Status="m7-w02-c02-t2-documentary-coverage-program-boundary-qualified";
const string Marker="dw.tools.math/m7-w02-c02-t2-coverage-program/1.2 qualified; c02=document-done; w02=open; m7=open; external=unsent; public-product=false";
const string Counts="M7-PROGRAM:REQUIREMENTS=106;OWNERS=53;SOURCES=20;DISPOSITIONS=6;DRAFTS=3;NODES=541";
const string Scope="M7-PROGRAM:SCOPE:C02=DOCUMENT-DONE;W02=OPEN;M7=OPEN;CLIENT-ADOPTION=false;RELEASE=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS121",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS120",1,
    "docs: qualify M7 coverage and owner inventory",
    "0.1.59",2,2,0,false);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.119"),JsonValue.Create("0.1.120"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.120":"0.1.119"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified"));
foreach(var (pointer,value) in new (string,bool)[]
{
    ("/t1_done",true),("/t2_done",false),("/c02_parent_done",false),
    ("/m7_w02_work_package_done",false),("/m7_release_done",false),
    ("/external_responses_confirmed",false),("/external_requests_transmitted",false),
    ("/public_product_delivered",false)
})
    p.Require.JsonValueEquals(Prior,pointer,JsonValue.Create(value));
p.Require.JsonValueEquals(Prior,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(WorkResearch,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(FormalResearch,"/public_formal_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(FormalResearch,"/external_prover_adopted",JsonValue.Create(false));
const string RunReport=".aura/workflow/reports/RS120/attempt-001.json";
p.Require.JsonValueEquals(RunReport,"/priorInvocationFailures/count",JsonValue.Create(2));
p.Require.JsonValueEquals(RunReport,"/priorInvocationFailures/preparationFailureCount",JsonValue.Create(2));
p.Require.JsonValueEquals(RunReport,"/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(false));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/coordination/requests/MATH-XR-001-aura.md","Transmission: **none**");
p.Require.TextContains("docs/coordination/requests/MATH-XR-002-decision.md","Transmission: none");
p.Require.TextContains("docs/coordination/requests/MATH-XR-003-mcdm.md","Transmission: none");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");

foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),("/releases/6/status","done"),
    ("/releases/7/id",Milestone),("/releases/7/status","in_progress"),
    ("/work_packages/15/id","M7-W01"),("/work_packages/15/status","done"),
    ("/work_packages/16/id",Work),("/work_packages/16/status","in_progress"),
    ("/chunks/56/id","M7-W02-C01"),("/chunks/56/status","done"),
    ("/chunks/57/id",Phase),("/chunks/57/kind","document"),
    ("/chunks/57/size","M"),("/chunks/57/status",target?"done":"in_progress"),
    ("/chunks/57/tasks/0/id","M7-W02-C02-T1"),
    ("/chunks/57/tasks/0/status","done"),
    ("/chunks/57/tasks/1/id",Task),
    ("/chunks/57/tasks/1/status",target?"done":"planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/57/tasks/0/subtasks/"+i+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/57/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
foreach(var node in new[]{"M7-W02-C02-T1","M7-W02-C02-T1-A",
    "M7-W02-C02-T1-B","M7-W02-C02-T1-C"})
    p.ProjectPlan.RequireNodeState(node,"done");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    foreach(var (pointer,number) in new (string,int)[]
    {
        ("/requirements_checked",106),("/distinct_owners",53),
        ("/source_coverage_rows",20),("/ownership_dispositions",6),
        ("/local_dependency_gates",4),("/unsent_requests",3),
        ("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(number));
    foreach(var (pointer,expected) in new (string,bool)[]
    {
        ("/t1_done",true),("/t2_done",true),
        ("/c02_document_phase_done",true),
        ("/m7_w02_work_package_done",false),("/m7_release_done",false),
        ("/external_approvals_received",false),
        ("/external_requests_transmitted",false),("/future_program_approved",false),
        ("/new_public_product_delivered",false)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(expected));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("C02 documentary decision SHA missing.");
var matrixSha=matrixWrite.AfterSha256 ??
    throw new InvalidOperationException("C02 program distribution SHA missing.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("C02 independent validator SHA missing.");
foreach(var term in new[]{"## Question","## Verified observations",
    "## Coverage, migration and owner disposition","## T2 Q/O/D decision",
    "## Candidate next program","## Next independent gate",
    "MATH-XR-001","MATH-XR-002","MATH-XR-003"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Source-pinned inventory",
    "## Ownership and pending response matrix",
    "## Program and release boundary","## Reproducibility and review",
    "541","not a public"})
    p.Require.TextContains(Distribution,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);
p.Require.TextContains(Validator,Counts);
p.Require.TextContains(Validator,Scope);
var before=Required(root,"dotnet","run","--file",Validator,"--","--check");
foreach(var expected in new[]{Marker,Counts,Scope})
    if(!before.Contains(expected,StringComparison.Ordinal))
        throw new InvalidOperationException("RS121 baseline program marker missing: "+expected);
if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W02-C02-T2-qualified",
        ["status"]=Status,
        ["question"]="Can the accepted coverage/program document close without inventing Math product delivery, authorized recipient migration or M7 release?",
        ["observation"]="RS120 covered 106 requirement IDs, 53 chunk owners, 20 sources, six dispositions, four local gates and three unsent drafts; advanced and formal provider research remains deferred, with no recipient acceptance.",
        ["decision"]="Close C02 as a bounded documentary review only; preserve unsent requests and deferred providers and separately gate M7-W02 work-package and M7 publication.",
        ["previous_inventory_evidence"]=Prior,
        ["previous_work_research_evidence"]=WorkResearch,
        ["previous_formal_research_evidence"]=FormalResearch,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["requirements_checked"]=106,["distinct_owners"]=53,
        ["source_coverage_rows"]=20,["ownership_dispositions"]=6,
        ["external_owned_dispositions"]=4,["excluded_dispositions"]=2,
        ["local_dependency_gates"]=4,["unsent_requests"]=3,
        ["native_nodes_retained"]=541,["previous_rs120_attempt"]=1,
        ["previous_rs120_pre_attempt_refusals"]=2,
        ["previous_rs120_first_pass"]=false,
        ["source_target_missing_count"]=0,["requirement_owner_missing_count"]=0,
        ["external_approvals_received"]=false,
        ["external_requests_transmitted"]=false,
        ["future_program_approved"]=false,
        ["new_public_product_delivered"]=false,
        ["publicly_published"]=false,
        ["sibling_repository_changed"]=false,
        ["t1_done"]=true,["t2_done"]=true,
        ["c02_document_phase_done"]=true,
        ["m7_w02_work_package_done"]=false,["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="Review M7-W02 research/document work-package closure from C01/C02 receipts, with external responses outstanding; M7 release remains independently owner-gated.",
        ["nonclaims"]=p.Json.StringArray(
            "A completed planning requirement or research spike is not proof that its public product was shipped.",
            "Unsent AURA/Decision/MCDM requests and Math-local dependency gates are not recipient approvals.",
            "Advanced and formal provider adoption still requires named consumers, rights, resource and trusted-base oracles.",
            "M7-W02, M7 1.2.0 release and any future program approval remain open.")
    };
    p.Files.WriteComplete(Qualified,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.119")
            throw new InvalidOperationException("Unexpected C02 T2 baseline plan version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var c01=doc["chunks"]!.AsArray()[56]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[57]!.AsObject();
        var first=phase["tasks"]!.AsArray()[0]!.AsObject();
        var second=phase["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone||
           (string?)release["status"]!="in_progress"||
           (string?)work["id"]!=Work||
           (string?)work["status"]!="in_progress"||
           (string?)c01["id"]!="M7-W02-C01"||
           (string?)c01["status"]!="done"||
           (string?)phase["id"]!=Phase||
           (string?)phase["kind"]!="document"||
           (string?)phase["status"]!="in_progress"||
           (string?)first["id"]!="M7-W02-C02-T1"||
           (string?)first["status"]!="done"||
           (string?)second["id"]!=Task||
           (string?)second["status"]!="planned")
            throw new InvalidOperationException("M7 C02 documentary source baseline drifted.");
        foreach(var child in first["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
            if((string?)child["status"]!="done")
                throw new InvalidOperationException("T1 Q/O/D must stay completed.");
        doc["plan_version"]="0.1.120";
        phase["status"]="done";
        phase["refinement"]="RS120 T1 and RS121 T2 verified 106 accepted requirements, 53 owners, 20 source entries, six dispositions, four Math-local gates and three unsent client drafts. C02 is a document decision, not Math product, external acceptance, migration or release.";
        phase["readiness"]="C02 documentary review completed with separate Q/O/D receipts. M7-W02 and M7 publication, external approvals and provider admission remain independent future gates.";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var receipts=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(receipts.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7CoverageProgram.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        second["status"]="done";
        second["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in second["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("T2 Q/O/D baseline was not planned.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var node in new[]{Task,Question})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
    }
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var node in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var node in new[]{"M7-W02-C02-T1","M7-W02-C02-T1-A",
    "M7-W02-C02-T1-B","M7-W02-C02-T1-C",
    Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS121"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 C02 document cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS121 completed document gate failed source-pinned re-entry.");
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
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS121 CSharp verification command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS121 program check exceeded local deadline.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS121 source-pinned program check failed: "+
            command+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2500)]);
    return output;
}
