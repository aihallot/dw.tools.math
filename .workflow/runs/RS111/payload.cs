using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Parent="M7-W01-C03", Phase="M7-W01-C03-S01";
const string Plot="M7-W01-C03-S02", Task="M7-W01-C03-S01-T1";
const string Question="M7-W01-C03-S01-T1-A";
const string Observation="M7-W01-C03-S01-T1-B";
const string DecisionNode="M7-W01-C03-S01-T1-C";
const string Backlog="docs/planning/backlog.json", State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string PriorTensor="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s01-t1.md";
const string OwnerScreen="docs/planning/decisions/m7-w01-c03-s01-owner-screen.md";
const string Oracle="docs/planning/ValidateM7DagAdmission.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string Status="m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s01-t1-pure-dag-screen/1.2 qualified; diamond=13; shared=once; cycle=refused; limits=8/10/4; runtime=false";
const string Diamond="M7-DAG-ORACLE:DIAMOND:NODES=4;EDGES=4;VALUE=13;SHARED-EVALS=1;DEPTH=3";
const string Refusal="M7-DAG-ORACLE:REFUSALS:CYCLE=true;UNKNOWN=true;DUPLICATE=true;NODES=8;EDGES=10;DEPTH=4;OVERFLOW=true;PARTIAL=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS111",Milestone,Work,Phase,null,null);
p.Workflow.RequirePriorSuccess("RS110",1,
    "chore: split M7 DAG and PlotModel into bounded research phases",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Prior,"/split_parent",JsonValue.Create(Parent));
p.Require.JsonValueEquals(Prior,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Prior,"/new_phase_count",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/new_task_count",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/new_subtask_count",JsonValue.Create(12));
p.Require.JsonValueEquals(Prior,"/dag_split_active",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/plot_split_active",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/dag_oracles_executed",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorTensor,"/status",
    JsonValue.Create("m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified"));
p.Require.JsonValueEquals(PriorTensor,"/m7_w01_c02_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PriorTensor,"/advanced_tensor_api_delivered",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.109"),JsonValue.Create("0.1.110"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.110":"0.1.109"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/status","in_progress"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/status","done"),
    ("/chunks/52/id","M7-W01-C02"),
    ("/chunks/52/status","done"),
    ("/chunks/53/id",Parent),
    ("/chunks/53/kind","spike"),
    ("/chunks/53/size","L"),
    ("/chunks/53/status","planned"),
    ("/chunks/54/id",Phase),
    ("/chunks/54/kind","spike"),
    ("/chunks/54/size","M"),
    ("/chunks/54/parent_chunk",Parent),
    ("/chunks/54/status","in_progress"),
    ("/chunks/54/tasks/0/id",Task),
    ("/chunks/54/tasks/0/status",target?"done":"planned"),
    ("/chunks/54/tasks/1/status","planned"),
    ("/chunks/55/id",Plot),
    ("/chunks/55/status","planned"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/54/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/54/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create("planned"));
}
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Plot,"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{Phase+"-T2",Phase+"-T2-A",Phase+"-T2-B",Phase+"-T2-C",
    Plot+"-T1",Plot+"-T2"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var file in new[]{Decision,OwnerScreen,Oracle,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/independent_oracle_groups",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/reference_root_value",JsonValue.Create(13));
    p.Require.JsonValueEquals(Qualified,"/reference_node_count",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/reference_edge_count",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/reference_shared_evaluations",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/research_max_nodes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/research_max_edges",JsonValue.Create(10));
    p.Require.JsonValueEquals(Qualified,"/research_max_depth",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/cycle_refused",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/no_partial_result_on_refusal",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/s01_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/dag_runtime_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/plot_renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+OwnerScreen,OwnerScreen);
var oracleWrite=p.Files.ReplaceFromStaged("staged/"+Oracle,Oracle);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged pure DAG decision source SHA-256.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged pure DAG owner screen SHA-256.");
var oracleSha=oracleWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged independent BCL DAG oracle SHA-256.");
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "shared exactly once","root=left+right=13","8 nodes","10 dependency edges",
    "No public DAG","AURA","T2"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Independent diamond and refusal matrix",
    "## Ownership and provider decision","## Subsequent T2 admission criteria",
    "8 nodes","10 edges","4 depth","source-free consumer","AURA ownership"})
    p.Require.TextContains(OwnerScreen,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Oracle,Diamond);
p.Require.TextContains(Oracle,Refusal);
var observed=Required(root,"dotnet","run","--file",Oracle,"--","--check");
foreach(var marker in new[]{Diamond,Refusal,Marker})
    if(!observed.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent bounded pure DAG marker absent: "+marker);

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-S01-T1-qualified",
        ["status"]=Status,
        ["question"]="Can a small Math-owned pure diamond DAG share one node without AURA scheduling and refuse invalid cycles/resources before numerical work?",
        ["observation"]="An independent BCL four-node/four-edge diamond yields root 13, shared 2 evaluated once, left 5 and right 8; malformed/over-budget graphs and checked arithmetic overflow produce no published partial result.",
        ["decision"]="Admit reproducible finite research oracles only; defer any public DAG engine, AURA scheduler integration or graph visualization until independent S01-T2 and owner disposition.",
        ["prior_dag_plotmodel_split"]=Prior,
        ["prior_tensor_deferral"]=PriorTensor,
        ["oracle_validator"]=Oracle,
        ["oracle_validator_sha256"]=oracleSha,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["owner_screen"]=OwnerScreen,
        ["owner_screen_sha256"]=screenSha,
        ["independent_oracle_groups"]=2,
        ["reference_graph"]="pure-diamond",
        ["reference_node_count"]=4,
        ["reference_edge_count"]=4,
        ["reference_root_depth"]=3,
        ["reference_shared_value"]=2,
        ["reference_shared_evaluations"]=1,
        ["reference_left_value"]=5,
        ["reference_right_value"]=8,
        ["reference_root_value"]=13,
        ["research_max_nodes"]=8,
        ["research_max_edges"]=10,
        ["research_max_depth"]=4,
        ["cycle_refused"]=true,
        ["self_cycle_refused"]=true,
        ["unknown_dependency_refused"]=true,
        ["duplicate_node_refused"]=true,
        ["node_budget_refused"]=true,
        ["edge_budget_refused"]=true,
        ["depth_budget_refused"]=true,
        ["unsupported_operation_shape_refused"]=true,
        ["checked_overflow_refused"]=true,
        ["no_partial_result_on_refusal"]=true,
        ["local_bcl_oracles_executed"]=true,
        ["new_math_product_tests_executed"]=false,
        ["new_math_package_built"]=false,
        ["dag_runtime_api_delivered"]=false,
        ["plot_renderer_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["external_provider_adopted"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["s01_phase_done"]=false,
        ["parent_c03_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03-S01-T2: independently qualify pure DAG identity, cancellation, resource/ownership and no-partial-result policies or defer any runtime API with review trigger.",
        ["nonclaims"]=p.Json.StringArray(
            "An independent reference evaluator is not an installed or delivered public DAG runtime.",
            "No AURA scheduling, agent/workflow engine, PlotModel renderer, graphics package or sibling repository writes were authorized.",
            "Research budgets 8 nodes, 10 edges, depth 4 are fixture limits, not general production DAG scalability guarantees.",
            "S01-T2, S01 parent, S02 PlotModel, original C03, W01 and M7 remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.109")
            throw new InvalidOperationException("Unexpected pre-RS111 M7 backlog version.");
        var m7=doc["releases"]!.AsArray()[7]!.AsObject();
        var wp=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[54]!.AsObject();
        var t1=chunk["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)m7["id"]!=Milestone ||
            (string?)m7["status"]!="in_progress" ||
            (string?)wp["id"]!=Work ||
            (string?)wp["status"]!="in_progress" ||
            (string?)parent["id"]!=Parent ||
            (string?)parent["status"]!="planned" ||
            (string?)chunk["id"]!=Phase ||
            (string?)chunk["status"]!="in_progress" ||
            (string?)chunk["parent_chunk"]!=Parent ||
            (string?)t1["id"]!=Task ||
            (string?)t1["status"]!="planned" ||
            (string?)chunk["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("M7 bounded DAG T1 baseline identity/status drifted.");
        doc["plan_version"]="0.1.110";
        chunk["refinement"]="RS110 staged two separate medium DAG and PlotModel research phases. RS111 T1 independently qualifies a pure four-node diamond: exact shared-once root 13 and deterministic cycle/unknown/duplicate/8-node/10-edge/4-depth/overflow refusals with no published partial result. No graph runtime, AURA scheduling or renderer is admitted.";
        chunk["readiness"]="DAG S01 T1 independent BCL oracles and Q/O/D research decision qualified; T2 must still decide identity/resources, cancellation and owner-gated provider admission or deferral before S01 phase closure.";
        var files=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,OwnerScreen,Oracle,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7DagAdmission.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t1["status"]="done";
        t1["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("S01-T1 Q/O/D child was not planned.");
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
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Plot,"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Phase+"-T2","not-ready");
p.ProjectPlan.RequireNodeState(Plot+"-T1","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS111" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=Phase ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 pure DAG T1 workflow cursor.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var final=Required(root,"dotnet","run","--file",Oracle,"--","--check");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS111 independent target DAG reference oracle did not pass.");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,
        UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS111 BCL study/plan validator could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS111 independent read-only validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS111 bounded Math planning/oracle check failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2600)]);
    return output;
}
