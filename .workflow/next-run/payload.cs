using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Parent="M7-W01-C03";
const string S01="M7-W01-C03-S01", S02="M7-W01-C03-S02";
const string Task="M7-W01-C03-T1";
const string Question="M7-W01-C03-T1-A";
const string Observation="M7-W01-C03-T1-B";
const string DecisionNode="M7-W01-C03-T1-C";
const string Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string DagT1="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string DagT2="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string PlotT1="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string PlotT2="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-t1.md";
const string Distribution="docs/distribution/m7-dag-plot-aggregate-screen.md";
const string Checker="docs/planning/ValidateM7DagPlotAggregate.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string Status="m7-c03-t1-aggregate-pure-dag-and-reciprocal-plot-research-qualified";
const string Marker="dw.tools.math/m7-w01-c03-t1-independent-aggregate/1.2 qualified; dag-root=13; reciprocal-points=4; segments=2; children=done; product=false";
const string DagMarker="M7-AGGREGATE-ORACLE:DIAMOND:ROOT=13:SHARED=1:NODES=4:EDGES=4";
const string PlotMarker="M7-AGGREGATE-ORACLE:RECIPROCAL:POINTS=4:SEGMENTS=2:POLE-GAP=1:OMITTED-BREAK=1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS115",Milestone,Work,target?Parent:null,null,null);
p.Workflow.RequirePriorSuccess("RS114",1,
    "docs: qualify PlotModel deferral and close M7 S02",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.113"),JsonValue.Create("0.1.114"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.114":"0.1.113"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(DagT1,"/status",
    JsonValue.Create("m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified"));
p.Require.JsonValueEquals(DagT1,"/reference_root_value",JsonValue.Create(13));
p.Require.JsonValueEquals(DagT1,"/reference_node_count",JsonValue.Create(4));
p.Require.JsonValueEquals(DagT1,"/reference_edge_count",JsonValue.Create(4));
p.Require.JsonValueEquals(DagT1,"/reference_shared_evaluations",JsonValue.Create(1));
p.Require.JsonValueEquals(DagT1,"/cycle_refused",JsonValue.Create(true));
p.Require.JsonValueEquals(DagT1,"/no_partial_result_on_refusal",JsonValue.Create(true));
p.Require.JsonValueEquals(DagT1,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(DagT2,"/status",
    JsonValue.Create("m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified"));
p.Require.JsonValueEquals(DagT2,"/disposition",
    JsonValue.Create("defer-unqualified-pure-dag-runtime-and-providers"));
p.Require.JsonValueEquals(DagT2,"/s01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(DagT2,"/parent_c03_done",JsonValue.Create(false));
p.Require.JsonValueEquals(DagT2,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(DagT2,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(PlotT1,"/status",
    JsonValue.Create("m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified"));
p.Require.JsonValueEquals(PlotT1,"/reference_function",JsonValue.Create("1/x"));
p.Require.JsonValueEquals(PlotT1,"/reference_finite_points",JsonValue.Create(4));
p.Require.JsonValueEquals(PlotT1,"/reference_segments",JsonValue.Create(2));
p.Require.JsonValueEquals(PlotT1,"/reference_pole_gaps",JsonValue.Create(1));
p.Require.JsonValueEquals(PlotT1,"/pole_omitted_breaks",JsonValue.Create(1));
p.Require.JsonValueEquals(PlotT1,"/pole_not_bridged",JsonValue.Create(true));
p.Require.JsonValueEquals(PlotT1,"/no_partial_result_on_refusal",JsonValue.Create(true));
p.Require.JsonValueEquals(PlotT1,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(PlotT2,"/status",
    JsonValue.Create("m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified"));
p.Require.JsonValueEquals(PlotT2,"/disposition",
    JsonValue.Create("defer-unqualified-plotmodel-and-rendering-provider"));
p.Require.JsonValueEquals(PlotT2,"/s02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PlotT2,"/parent_c03_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PlotT2,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(PlotT2,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(PlotT2,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Split,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Split,"/new_phase_count",JsonValue.Create(2));
p.Require.JsonValueEquals(Split,"/parent_chunk_still_planned",JsonValue.Create(true));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/status","done"),
    ("/chunks/52/status","done"),
    ("/chunks/53/id",Parent),
    ("/chunks/53/size","L"),
    ("/chunks/53/kind","spike"),
    ("/chunks/53/status",target?"in_progress":"planned"),
    ("/chunks/53/tasks/0/id",Task),
    ("/chunks/53/tasks/0/status",target?"done":"planned"),
    ("/chunks/53/tasks/1/id","M7-W01-C03-T2"),
    ("/chunks/53/tasks/1/status","planned"),
    ("/chunks/54/id",S01),
    ("/chunks/54/status","done"),
    ("/chunks/55/id",S02),
    ("/chunks/55/status","done"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/53/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/53/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create("planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(S02,"done");
p.ProjectPlan.RequireNodeState(Parent,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C03-T2","M7-W01-C03-T2-A",
    "M7-W01-C03-T2-B","M7-W01-C03-T2-C","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Checker,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/independent_research_families",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/prior_receipts_verified",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/native_nodes_retained",JsonValue.Create(541));
    p.Require.JsonValueEquals(Qualified,"/diamond_root_value",JsonValue.Create(13));
    p.Require.JsonValueEquals(Qualified,"/diamond_shared_evaluations",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/reciprocal_finite_points",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/reciprocal_segments",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/known_pole_omitted_break_preserved",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/children_both_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_dag_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_plot_model_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var checkerWrite=p.Files.ReplaceFromStaged("staged/"+Checker,Checker);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged C03 aggregate T1 decision SHA-256 absent.");
var distributionSha=distributionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged C03 research ownership matrix SHA-256 absent.");
var checkerSha=checkerWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged C03 independent aggregate oracle SHA-256 absent.");

foreach(var token in new[]{"## Question","## Observation","## Accepted aggregate T1 decision",
    "M7-W01-C03-S01","M7-W01-C03-S02","two disconnected segments",
    "root = (2 + 3) + (2 * 4) = 13","no public DAG API",
    "no public PlotModel API","541 native nodes"})
    p.Require.TextContains(Decision,token,StringComparison.OrdinalIgnoreCase);
foreach(var token in new[]{"## Immutable completed research chain",
    "## Falsifiable aggregate reference and finite budgets",
    "## Ownership and negative gate","## Remaining original-parent task",
    "root=13","128","source-free","two segments","541 nodes"})
    p.Require.TextContains(Distribution,token,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Checker,DagMarker);
p.Require.TextContains(Checker,PlotMarker);
var beforeOracle=Required(root,"dotnet","run","--file",Checker,"--","--check");
foreach(var marker in new[]{DagMarker,PlotMarker,Marker})
    if(!beforeOracle.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent cross-family C03 aggregate reference marker absent: "+marker);
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-T1-qualified",
        ["status"]=Status,
        ["question"]="Do S01 pure DAG and S02 reciprocal PlotModel qualified research satisfy both original C03 independent oracle families without claiming product delivery?",
        ["observation"]="SHA-pinned RS111-RS114 C03 child research receipts agree with BCL-independent diamond root 13/shared once and reciprocal four finite points split into two segments with an omitted-pole break.",
        ["decision"]="Admit original C03 T1 aggregate research evidence only, preserving S01 DAG and S02 renderer/provider deferrals. T2 independently gates resource/ownership and any parent completion.",
        ["prior_split_evidence"]=Split,
        ["dag_t1_evidence"]=DagT1,
        ["dag_t2_evidence"]=DagT2,
        ["plot_t1_evidence"]=PlotT1,
        ["plot_t2_evidence"]=PlotT2,
        ["independent_research_families"]=2,
        ["prior_receipts_verified"]=4,
        ["native_nodes_retained"]=541,
        ["diamond_node_count"]=4,
        ["diamond_edge_count"]=4,
        ["diamond_root_value"]=13,
        ["diamond_shared_evaluations"]=1,
        ["dag_research_max_nodes"]=8,
        ["dag_research_max_edges"]=10,
        ["dag_research_max_depth"]=4,
        ["reciprocal_finite_points"]=4,
        ["reciprocal_segments"]=2,
        ["reciprocal_pole_gaps"]=1,
        ["known_pole_omitted_break_preserved"]=true,
        ["plot_research_max_samples"]=8,
        ["plot_research_max_points"]=8,
        ["plot_research_max_segments"]=2,
        ["children_both_done"]=true,
        ["dag_provider_disposition"]="defer-unqualified-pure-dag-runtime-and-providers",
        ["plot_provider_disposition"]="defer-unqualified-plotmodel-and-rendering-provider",
        ["source_only_bcl_oracles_executed"]=true,
        ["public_dag_api_delivered"]=false,
        ["public_plot_model_api_delivered"]=false,
        ["renderer_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["advanced_library_installed"]=false,
        ["new_math_product_tests_executed"]=false,
        ["new_math_package_built"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["parent_c03_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=distributionSha,
        ["aggregate_validator"]=Checker,
        ["aggregate_validator_sha256"]=checkerSha,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03-T2: independently finalize parent integration/ownership, finite graph/plot resource semantics and no-delivery evidence; close C03 only after a bounded owner decision.",
        ["nonclaims"]=p.Json.StringArray(
            "The original size-L research aggregate T1 is not a product DAG, PlotModel, renderer or AURA task engine.",
            "Two small BCL scientific oracles do not qualify arbitrary graphs/functions, general discontinuities, physical uncertainties or memory SLAs.",
            "S01/S02 provider deferrals remain binding until named consumers and separate independent provider/rights/RID/source-free qualification.",
            "C03-T2, parent C03, M7-W01 and M7 remain unfinished; no public package or sibling repository migration.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.113")
            throw new InvalidOperationException("Unexpected RS115 aggregate baseline plan version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var wp=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var s01=doc["chunks"]!.AsArray()[54]!.AsObject();
        var s02=doc["chunks"]!.AsArray()[55]!.AsObject();
        var t1=parent["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)wp["id"]!=Work ||
            (string?)wp["status"]!="in_progress" ||
            (string?)parent["id"]!=Parent ||
            (string?)parent["kind"]!="spike" ||
            (string?)parent["size"]!="L" ||
            (string?)parent["status"]!="planned" ||
            (string?)s01["id"]!=S01 ||
            (string?)s01["status"]!="done" ||
            (string?)s02["id"]!=S02 ||
            (string?)s02["status"]!="done" ||
            (string?)t1["id"]!=Task ||
            (string?)t1["status"]!="planned" ||
            (string?)parent["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("Original M7 C03 T1 owner/evidence baseline drifted.");
        doc["plan_version"]="0.1.114";
        parent["status"]="in_progress";
        parent["refinement"]="RS110 split the accepted size-L C03 into two medium research phases. RS111/112 completed pure diamond DAG/cycle reference and explicit runtime deferral. RS113/114 completed reciprocal 1/x two-disconnected-segment reference and PlotModel renderer deferral. RS115 T1 independently rechecks four source SHA-pinned receipts and both hand-computable oracles, retaining C03 T2 for separate quota/ownership gate.";
        parent["readiness"]="Original size-L C03 in progress after independent S01/S02 completion and aggregate T1 acceptance: exact diamond root 13/shared-once, known-pole 1/x no bridge, limited research quotas and explicit no-DAG/no-renderer product claims. Parent T2 and W01/M7 release gates remain pending.";
        var files=parent["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        parent["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Checker,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var proofs=parent["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        parent["evidence"]=p.Json.StringArray(proofs.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        parent["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7DagPlotAggregate.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t1["status"]="done";
        t1["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("Original C03 T1 Q/O/D child was not planned.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var id in new[]{Parent,Task,Question})
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
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(S02,"done");
p.ProjectPlan.RequireNodeState(Parent,"in-progress");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C03-T2","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS115" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?Parent:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected original M7 C03 aggregate Q/O/D cursor.");
    state["activePhase"]=Parent;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalOracle=Required(root,"dotnet","run","--file",Checker,"--","--check");
if(!finalOracle.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS115 target failed independent aggregate source and science gate.");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS115 local BCL/plan aggregate gate did not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS115 independent aggregate gate timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS115 local DAG/PlotModel reconciliation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2600)]);
    return output;
}
