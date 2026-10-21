using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W02";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string C01T1="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string C01T2="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string C02T1="docs/planning/evidence/M7-W02-C02-T1-qualified.json";
const string C02T2="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string W01="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Decision="docs/planning/decisions/m7-w02-work-package-closure.md";
const string Matrix="docs/distribution/m7-w02-work-package-research-matrix.md";
const string Validator="docs/planning/ValidateM7ProofsWorkPackage.cs";
const string Qualified="docs/planning/evidence/M7-W02-work-package-qualified.json";
const string Status="m7-w02-formal-research-and-document-portfolio-qualified-external-products-deferred";
const string Marker="dw.tools.math/m7-w02-work-package-closure/1.2 qualified; phases=2; tasks=4; qod=12; requests=unsent; provider=false; m7=open";
const string Counts="M7-W02-PORTFOLIO:PHASES=2;TASKS=4;QOD=12;REQUIREMENTS=106;SOURCES=20;DRAFTS=3;NODES=541";
const string Scope="M7-W02-SCOPE:WORK=RESEARCH-DOCUMENT-DONE;M7=IN-PROGRESS;PUBLIC-PRODUCT=false;REQUESTS-SENT=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS122",Milestone,target?null:Work,null,null,null);
p.Workflow.RequirePriorSuccess("RS121",1,
    "docs: close M7 C02 coverage and program review",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.120"),JsonValue.Create("0.1.121"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.121":"0.1.120"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
foreach(var (path,value) in new (string,string)[]
{
    (C01T1,"m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified"),
    (C01T2,"m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified"),
    (C02T1,"m7-w02-c02-t1-owner-source-coverage-and-external-draft-inventory-qualified"),
    (C02T2,"m7-w02-c02-t2-documentary-coverage-program-boundary-qualified"),
    (W01,"m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished")
})
    p.Require.JsonValueEquals(path,"/status",JsonValue.Create(value));
p.Require.JsonValueEquals(C01T2,"/c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(C02T2,"/c02_document_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(C02T2,"/m7_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(C02T2,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(C02T2,"/unsent_requests",JsonValue.Create(3));
p.Require.JsonValueEquals(C02T2,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(C02T2,"/new_public_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(C02T2,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(C02T2,"/external_approvals_received",JsonValue.Create(false));
p.Require.JsonValueEquals(C01T2,"/external_prover_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(C01T2,"/public_formal_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(W01,"/m7_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(W01,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(".aura/workflow/reports/RS121/attempt-001.json",
    "/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(".aura/workflow/reports/RS121/attempt-001.json",
    "/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(true));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");

foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/7/id",Milestone),("/releases/7/status","in_progress"),
    ("/work_packages/15/id","M7-W01"),("/work_packages/15/status","done"),
    ("/work_packages/16/id",Work),
    ("/work_packages/16/status",target?"done":"in_progress"),
    ("/chunks/56/id","M7-W02-C01"),("/chunks/56/status","done"),
    ("/chunks/57/id","M7-W02-C02"),("/chunks/57/status","done"),
    ("/chunks/57/kind","document")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
foreach(var child in new[]{"M7-W02-C01","M7-W02-C02"})
    p.ProjectPlan.RequireNodeState(child,"done");
if(!target)
{
    foreach(var file in new[]{Decision,Matrix,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    foreach(var (pointer,value) in new (string,int)[]
    {
        ("/native_nodes_retained",541),("/qualified_phases",2),
        ("/qualified_tasks",4),("/qualified_subtasks",12),
        ("/source_pinned_receipts",4),("/unsent_requests",3)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(value));
    foreach(var (pointer,value) in new (string,bool)[]
    {
        ("/m7_w02_work_package_done",true),("/m7_release_done",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/new_public_product_delivered",false),
        ("/publicly_published",false)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(value));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixWrite=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("W02 work-package decision source SHA absent.");
var matrixSha=matrixWrite.AfterSha256 ??
    throw new InvalidOperationException("W02 research portfolio matrix source SHA absent.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("W02 independent CSharp checker source SHA absent.");
foreach(var part in new[]{
    "## Question","## Accepted scope and observations",
    "## Independent qualification and refusal gates",
    "## Work-package decision","## Next milestone gate",
    "MATH-XR-001","MATH-XR-002","MATH-XR-003"
})
    p.Require.TextContains(Decision,part,StringComparison.OrdinalIgnoreCase);
foreach(var part in new[]{
    "## Native scope and reproducibility",
    "## Ownership and remaining external debt",
    "## Work-package versus milestone gate","541","unsent"
})
    p.Require.TextContains(Matrix,part,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);
p.Require.TextContains(Validator,Counts);
p.Require.TextContains(Validator,Scope);
var before=Required(root,"dotnet","run","--file",Validator,"--","--check");
foreach(var marker in new[]{Marker,Counts,Scope})
    if(!before.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Baseline M7 W02 work-package witness absent: "+marker);
if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W02-work-package-qualified",
        ["status"]=Status,
        ["question"]="Can two accepted formal research and documentary review phases close their W02 work-package parent without inventing public product or M7 release?",
        ["observation"]="Four C01/C02 T1/T2 receipts and prior advanced W01 research are source-pinned; 2 phases, 4 tasks and 12 Q/O/D children are complete. Three client requests are drafts and no formal provider or public product is newly admitted.",
        ["decision"]="Close accepted M7-W02 research/document portfolio only. M7 release/scientific/rights/owner gate remains independently open; external requests and provider qualification stay deferred.",
        ["prior_evidence"]=p.Json.StringArray(C01T1,C01T2,C02T1,C02T2,W01),
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["native_nodes_retained"]=541,
        ["qualified_phases"]=2,["qualified_tasks"]=4,
        ["qualified_subtasks"]=12,["source_pinned_receipts"]=4,
        ["accepted_requirement_ids"]=106,["framing_sources"]=20,
        ["ownership_dispositions"]=6,["math_local_gates"]=4,
        ["unsent_requests"]=3,
        ["prior_rs121_attempt"]=1,["prior_rs121_first_pass"]=true,
        ["m7_w01_work_package_done"]=true,
        ["m7_w02_work_package_done"]=true,["m7_release_done"]=false,
        ["advanced_provider_adopted"]=false,
        ["general_proof_kernel_certified"]=false,
        ["public_formal_api_delivered"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["new_public_product_delivered"]=false,
        ["publicly_published"]=false,
        ["future_program_approved"]=false,
        ["sibling_repository_changed"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="Independent M7 G-M7 milestone and release-scientific-rights owner review; distinguish completed research/document portfolio from authorized product/publication and untransmitted external requests.",
        ["nonclaims"]=p.Json.StringArray(
            "Completed W02 scientific research and coverage documents do not imply general mathematical proof APIs or external trusted kernels.",
            "Math-side satisfied gates and unsent AURA/Decision/MCDM drafts are not recipient adoption or authorized migration.",
            "No advanced precision, tensor, DAG or PlotModel new public provider is qualified by this W02 closure.",
            "M7 candidate release and publication, future program admission and owner authorization remain pending.")
    };
    p.Files.WriteComplete(Qualified,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.120")
            throw new InvalidOperationException("Unexpected W02 work package baseline plan version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var advanced=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var chunks=doc["chunks"]!.AsArray();
        if((string?)release["id"]!=Milestone||
           (string?)release["status"]!="in_progress"||
           (string?)advanced["id"]!="M7-W01"||
           (string?)advanced["status"]!="done"||
           (string?)work["id"]!=Work||
           (string?)work["status"]!="in_progress")
            throw new InvalidOperationException("M7 W02 accepted parent baseline drifted.");
        foreach(var item in chunks)
        {
            var chunk=item!.AsObject();
            if((string?)chunk["status"]!="done")
                throw new InvalidOperationException("W02 may close only after every accepted M7 chunk is done.");
        }
        doc["plan_version"]="0.1.121";
        work["status"]="done";
        work["evidence"]=p.Json.StringArray(C01T1,C01T2,C02T1,C02T2,Qualified);
    });
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"done");
foreach(var child in new[]{"M7-W02-C01","M7-W02-C02"})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS122"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=(target?null:Work)||
       state["activePhase"] is not null||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected W02 research portfolio cursor.");
    state["activeWorkPackage"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS122 target M7 W02 work-package qualification failed.");
return p.Complete();

static string Required(string root,string command,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=command,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS122 local CSharp verification command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS122 bounded local verifier timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS122 scientific/documentary work-package gate failed: "+
            command+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
