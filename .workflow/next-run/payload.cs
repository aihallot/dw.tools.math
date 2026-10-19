using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Phase="M7-W01-C02", Task="M7-W01-C02-T1";
const string Question="M7-W01-C02-T1-A", Observation="M7-W01-C02-T1-B", DecisionNode="M7-W01-C02-T1-C";
const string Backlog="docs/planning/backlog.json", State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string M6Gate="docs/planning/evidence/M6-gate.json";
const string Decision="docs/planning/decisions/m7-w01-c02.md";
const string ProviderScreen="docs/planning/decisions/m7-w01-c02-provider-screen.md";
const string Oracle="docs/planning/ValidateM7TensorAdmission.cs";
const string Qualified="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string Status="m7-c02-t1-dense-shape-oracles-and-provider-screen-qualified";
const string Marker="dw.tools.math/m7-w01-c02-t1-independent-tensor-screen/1.2 qualified; dense=2x2x2; sum=36; mismatch-refused=true; cap=64; provider=none";
const string DenseMarker="M7-TENSOR-ORACLE:DENSE:SHAPE=2x2x2:SUM=36:INDEX[1,0,1]=6";
const string RefusalMarker="M7-TENSOR-ORACLE:REFUSALS:SHAPE-MISMATCH=true;BUDGET-OVER=64;ZERO-EXTENT=true";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS108",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS107",1,
    "docs: qualify precision provider deferral and close M7 C01",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(M6Gate,"/status",
    JsonValue.Create("m6-independent-local-analysis-scientific-release-gate-qualified-unpublished"));
p.Require.JsonValueEquals(M6Gate,"/m6_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(M6Gate,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/disposition",
    JsonValue.Create("defer-new-precision-interval-provider"));
p.Require.JsonValueEquals(Prior,"/m7_w01_c01_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_provider_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_precision_api_delivered",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.106"),JsonValue.Create("0.1.107"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.107":"0.1.106"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/status","in_progress"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/id","M7-W01-C01"),
    ("/chunks/51/status","done"),
    ("/chunks/52/id",Phase),
    ("/chunks/52/kind","spike"),
    ("/chunks/52/status",target?"in_progress":"planned"),
    ("/chunks/52/tasks/0/id",Task),
    ("/chunks/52/tasks/0/status",target?"done":"planned"),
    ("/chunks/52/tasks/1/id","M7-W01-C02-T2"),
    ("/chunks/52/tasks/1/status","planned"),
    ("/chunks/53/status","planned"),
    ("/chunks/54/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var index=0;index<3;index++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/52/tasks/0/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/52/tasks/1/subtasks/"+index+"/status",JsonValue.Create("planned"));
}
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C02-T2","M7-W01-C02-T2-A",
    "M7-W01-C02-T2-B","M7-W01-C02-T2-C",
    "M7-W01-C03","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,ProviderScreen,Oracle,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/independent_oracle_groups",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/reference_elements",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/reference_sum",JsonValue.Create(36));
    p.Require.JsonValueEquals(Qualified,"/reference_index_value",JsonValue.Create(6));
    p.Require.JsonValueEquals(Qualified,"/research_element_budget",JsonValue.Create(64));
    p.Require.JsonValueEquals(Qualified,"/research_byte_budget",JsonValue.Create(512));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_c02_parent_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/advanced_tensor_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_tensor_provider_adopted",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var providerWrite=p.Files.ReplaceFromStaged("staged/"+ProviderScreen,ProviderScreen);
var oracleWrite=p.Files.ReplaceFromStaged("staged/"+Oracle,Oracle);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged tensor Q/O/D decision SHA-256.");
var providerSha=providerWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged tensor provider-screen SHA-256.");
var oracleSha=oracleWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged BCL tensor oracle SHA-256.");
foreach(var snippet in new[]{"## Question","## Observation","## Decision",
    "[2,2,2]","sum = 36","[1,0,1] = 6",
    "64-element","512-byte","No product Tensor API","T2"})
    p.Require.TextContains(Decision,snippet,StringComparison.OrdinalIgnoreCase);
foreach(var snippet in new[]{"## Independent shape and resource oracle",
    "## Candidate approach and ownership matrix","[4,4,5]",
    "Unqualified","AURA","source-free"})
    p.Require.TextContains(ProviderScreen,snippet,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Oracle,DenseMarker);
p.Require.TextContains(Oracle,RefusalMarker);
var beforeOracle=Required(root,"dotnet","run","--file",Oracle,"--","--check");
foreach(var marker in new[]{DenseMarker,RefusalMarker,Marker})
    if(!beforeOracle.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent dense tensor oracle marker not observed: "+marker);
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C02-T1-qualified",
        ["status"]=Status,
        ["question"]="What precisely bounded tensor shape, storage and refusal witnesses demonstrate a real future advanced-structure need?",
        ["observation"]="An independent BCL-only rank-three 2x2x2 reference yields sum 36 and value 6 at [1,0,1]; same-count mismatched shape, zero extent and above-budget dimensions refuse before allocation.",
        ["decision"]="Admit tensor shape/layout and finite budget research fixtures only. Keep existing Math engines; do not install a provider, create an advanced tensor API or infer downstream demand.",
        ["prior_precision_disposition"]=Prior,
        ["prior_m6_local_gate"]=M6Gate,
        ["oracle_validator"]=Oracle,
        ["oracle_validator_sha256"]=oracleSha,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["provider_screen"]=ProviderScreen,
        ["provider_screen_sha256"]=providerSha,
        ["independent_oracle_groups"]=2,
        ["reference_rank"]=3,
        ["reference_shape"]="2x2x2",
        ["reference_elements"]=8,
        ["reference_sum"]=36,
        ["reference_index"]="1,0,1",
        ["reference_index_value"]=6,
        ["same_element_count_different_shape_refused"]=true,
        ["zero_extent_refused"]=true,
        ["oversized_extent_refused_before_allocation"]=true,
        ["research_element_budget"]=64,
        ["research_byte_budget"]=512,
        ["at_limit_shape"]="4x4x4",
        ["over_limit_shape"]="4x4x5",
        ["research_oracles_executed"]=true,
        ["third_party_tensor_candidates_installed"]=0,
        ["external_tensor_provider_adopted"]=false,
        ["advanced_tensor_api_delivered"]=false,
        ["sparse_broadcast_gpu_certified"]=false,
        ["aura_job_engine_copied"]=false,
        ["implicit_tensor_reshape_or_copy"]=false,
        ["new_math_product_tests_executed"]=false,
        ["new_math_package_built"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["m7_w01_c02_parent_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C02-T2: independently admit a bounded tensor operation with named consumer and source-free tests, or defer with owner trigger and resource/shape refusal policy.",
        ["nonclaims"]=p.Json.StringArray(
            "A BCL-only research shape oracle is not a public Tensor API, general operation engine, sparse/broadcast support or GPU qualification.",
            "No verified new managed/native tensor provider, source/right/version/RID or independent package consumer was admitted.",
            "The fixed 64-element/512-byte witness is a research budget, not a shipped public API limit.",
            "T2, C02 phase, W01 and M7 remain incomplete; no sibling modification or public NuGet release.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.106")
            throw new InvalidOperationException("Unexpected RS108 baseline backlog version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[52]!.AsObject();
        var task=chunk["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)work["id"]!=Work ||
            (string?)work["status"]!="in_progress" ||
            (string?)chunk["id"]!=Phase ||
            (string?)chunk["kind"]!="spike" ||
            (string?)chunk["status"]!="planned" ||
            (string?)task["id"]!=Task ||
            (string?)task["status"]!="planned" ||
            (string?)chunk["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("C02 tensor research baseline drifted.");
        doc["plan_version"]="0.1.107";
        chunk["status"]="in_progress";
        chunk["refinement"]="RS108 T1 studies a finite BCL rank-three 2x2x2 row-major reference: sum 36, cell [1,0,1]=6, equal-size incompatible shapes, zero dimension and 64-element/512-byte cap. The owner/provider screen admits only research fixtures, no third-party tensor API, GPU, sparse/broadcast or arbitrary precision inference. C02-T2 remains planned.";
        chunk["readiness"]="C02 T1 Q/O/D independently checks bounded shape, indexing and memory refusals. Provider/consumer evaluation remains for T2; a completed T1 spike is not a delivered tensor library.";
        var files=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,ProviderScreen,Oracle,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7TensorAdmission.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)sub["status"]!="planned")
                throw new InvalidOperationException("Tensor T1 Q/O/D subtask was not planned.");
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Qualified);
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
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M7-W01-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS108" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?Phase:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS108 M7 tensor research cursor.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var afterOracle=Required(root,"dotnet","run","--file",Oracle,"--","--check");
if(!afterOracle.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent tensor target oracle marker not observed.");
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
        throw new InvalidOperationException("RS108 BCL oracle/planning command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS108 independent validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS108 local oracle/planning failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
