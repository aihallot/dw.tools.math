using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Phase="M7-W01-C02", Task="M7-W01-C02-T2";
const string Question="M7-W01-C02-T2-A", Observation="M7-W01-C02-T2-B", DecisionNode="M7-W01-C02-T2-C";
const string State=".aura/workflow/state.json", Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string PriorOracle="docs/planning/ValidateM7TensorAdmission.cs";
const string M6Gate="docs/planning/evidence/M6-gate.json";
const string Decision="docs/planning/decisions/m7-w01-c02-t2.md";
const string Distribution="docs/distribution/m7-tensor-shape-deferral.md";
const string Validator="docs/planning/ValidateM7TensorBoundary.cs";
const string Qualified="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string Status="m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified";
const string Marker="dw.tools.math/m7-w01-c02-t2-tensor-deferral/1.2 qualified; triggers=3; provider=none; api=false; phase=done";
const string PriorMarker="dw.tools.math/m7-w01-c02-t1-independent-tensor-screen/1.2 qualified; dense=2x2x2; sum=36; mismatch-refused=true; cap=64; provider=none";
const string Trigger1="A named Math consumer with a missing bounded tensor operation and explicit finite shape demonstrates a real gap.";
const string Trigger2="An owner-approved shape/stride/element/byte/cancellation policy rejects incompatible and oversized requests without hidden copy.";
const string Trigger3="Exact source/license/RID/notices and an independent shape/oracle, offline source-free consumer and rollback budget qualify any new provider.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS109",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS108",1,
    "docs: qualify M7 tensor shape and resource research oracles",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(M6Gate,"/status",
    JsonValue.Create("m6-independent-local-analysis-scientific-release-gate-qualified-unpublished"));
p.Require.JsonValueEquals(M6Gate,"/m6_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(M6Gate,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c02-t1-dense-shape-oracles-and-provider-screen-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_oracle_groups",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/reference_rank",JsonValue.Create(3));
p.Require.JsonValueEquals(Prior,"/reference_elements",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/reference_sum",JsonValue.Create(36));
p.Require.JsonValueEquals(Prior,"/reference_index_value",JsonValue.Create(6));
p.Require.JsonValueEquals(Prior,"/same_element_count_different_shape_refused",
    JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/zero_extent_refused",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/oversized_extent_refused_before_allocation",
    JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/research_element_budget",JsonValue.Create(64));
p.Require.JsonValueEquals(Prior,"/research_byte_budget",JsonValue.Create(512));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w01_c02_parent_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/advanced_tensor_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_tensor_provider_adopted",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.107"),JsonValue.Create("0.1.108"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.108":"0.1.107"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/status","in_progress"),
    ("/releases/6/id","M6"),("/releases/6/status","done"),
    ("/releases/7/id",Milestone),("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/id","M7-W01-C01"),("/chunks/51/status","done"),
    ("/chunks/52/id",Phase),("/chunks/52/kind","spike"),
    ("/chunks/52/status",target?"done":"in_progress"),
    ("/chunks/52/tasks/0/id","M7-W01-C02-T1"),
    ("/chunks/52/tasks/0/status","done"),
    ("/chunks/52/tasks/1/id",Task),
    ("/chunks/52/tasks/1/status",target?"done":"planned"),
    ("/chunks/53/id","M7-W01-C03"),
    ("/chunks/53/status","planned"),
    ("/chunks/54/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var index=0;index<3;index++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/52/tasks/0/subtasks/"+index+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/52/tasks/1/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C02-T1","done");
foreach(var id in new[]{"M7-W01-C02-T1-A","M7-W01-C02-T1-B","M7-W01-C02-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C03","M7-W01-C03-T1","M7-W01-C03-T2","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
p.Require.TextContains(PriorOracle,
    "M7-TENSOR-ORACLE:REFUSALS:SHAPE-MISMATCH=true;BUDGET-OVER=64;ZERO-EXTENT=true");

if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/disposition",
        JsonValue.Create("defer-unqualified-tensor-api-and-providers"));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/provider_categories_deferred",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_c02_parent_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_tensor_provider_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/advanced_tensor_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/automatic_cutover",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged tensor T2 decision SHA-256.");
var distributionSha=distributionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged tensor T2 provider screen SHA-256.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing independent tensor T2 gate SHA-256.");
foreach(var snippet in new[]{"## Question","## Observation","## Decision",
    "no public Tensor API","three mandatory","All three gates",
    "M7-W01-C03"})
    p.Require.TextContains(Decision,snippet,StringComparison.OrdinalIgnoreCase);
foreach(var snippet in new[]{"## Retained independent shape evidence",
    "## Provider options and admission status","## Three review triggers",
    "If any review trigger is missing, remain deferred","M7-W01-C03"})
    p.Require.TextContains(Distribution,snippet,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);

if(!target)
{
    var reference=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!reference.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS108 exact tensor research oracle did not requalify.");
}
var baseline=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!baseline.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent tensor deferral baseline refused.");
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C02-T2-qualified",
        ["status"]=Status,
        ["question"]="Can a bounded tensor API and provider be responsibly admitted without a named consumer or source-free qualification?",
        ["observation"]="The RS108 dense rank-three shape and resource refusal oracles retain pinned source identities; no independent tensor consumer, provider source/license/RID or public Tensor contract was qualified.",
        ["decision"]="Explicitly defer new Tensor APIs, advanced packages, implicit broadcasting/sparse/GPU features and third-party providers until three independently evidenced owner gates are met.",
        ["prior_tensor_evidence"]=Prior,
        ["prior_oracle_validator"]=PriorOracle,
        ["prior_oracles_reexecuted_before_boundary_transition"]=true,
        ["prior_reference_oracle_groups"]=2,
        ["prior_reference_shape"]="2x2x2",
        ["prior_reference_elements"]=8,
        ["prior_research_element_budget"]=64,
        ["prior_research_byte_budget"]=512,
        ["disposition"]="defer-unqualified-tensor-api-and-providers",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["provider_categories_screened"]=5,
        ["provider_categories_deferred"]=4,
        ["new_tensor_provider_categories_admitted"]=0,
        ["external_tensor_provider_adopted"]=false,
        ["external_tensor_library_installed"]=false,
        ["provider_license_verified"]=false,
        ["provider_rid_qualified"]=false,
        ["provider_source_free_consumer_qualified"]=false,
        ["advanced_tensor_api_delivered"]=false,
        ["sparse_broadcast_gpu_certified"]=false,
        ["automatic_cutover"]=false,
        ["publicly_published"]=false,
        ["new_math_code_or_packages_built"]=false,
        ["aura_job_engine_copied"]=false,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_document_sha256"]=distributionSha,
        ["boundary_validator"]=Validator,
        ["boundary_validator_sha256"]=validatorSha,
        ["t1_done"]=true,
        ["t2_done"]=true,
        ["m7_w01_c02_parent_done"]=true,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03 needs explicit size-L split and independent DAG/PlotModel resource and AURA ownership research; no Tensor API inheritance.",
        ["nonclaims"]=p.Json.StringArray(
            "Research completion is not an implemented Tensor API, released advanced package, or third-party license/cutover.",
            "One rank-three dense oracle does not establish sparse, broadcasting, GPU, universal rank or numerical precision guarantees.",
            "The 64-element/512-byte witness is an internal study cap, not a public Math API contract.",
            "M7-W01 and M7 remain incomplete; DAG execution/rendering and AURA/Decision integration are not authorized.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.107")
            throw new InvalidOperationException("Unexpected RS109 baseline backlog version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[52]!.AsObject();
        var t2=chunk["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)work["id"]!=Work ||
            (string?)work["status"]!="in_progress" ||
            (string?)chunk["id"]!=Phase ||
            (string?)chunk["kind"]!="spike" ||
            (string?)chunk["status"]!="in_progress" ||
            (string?)chunk["tasks"]!.AsArray()[0]!["status"]!="done" ||
            (string?)t2["id"]!=Task || (string?)t2["status"]!="planned")
            throw new InvalidOperationException("M7 C02 T2 scientific deferral baseline drifted.");
        doc["plan_version"]="0.1.108";
        chunk["status"]="done";
        chunk["refinement"]="RS108 T1 independently qualified finite 2x2x2 row-major shape/sum, incompatibility and a 64-element/512-byte budget. RS109 T2 defers a new Tensor API/provider pending named consumer gap, bounded rank/shape/resources, and source/license/RID and independent source-free consumer evidence. Complete C02 as a research spike only; no package or AURA engine is delivered.";
        chunk["readiness"]="C02 research spike complete through explicit tensor-provider deferral and three owner-gated review triggers. Current Math matrix/array contracts remain; next size-L DAG/PlotModel C03 needs splitting before readiness.";
        var files=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7TensorBoundary.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t2["status"]="done";
        t2["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t2["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("M7 tensor T2 Q/O/D subtask not originally planned.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var id in new[]{Task,Question})
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
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS109" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?null:Phase) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS109 tensor research closure cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var final=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS109 completed target did not pass independent tensor deferral gate.");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS109 independent boundary checker could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS109 independent boundary checker timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS109 tensor documentary validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
