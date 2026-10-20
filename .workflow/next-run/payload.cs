using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Parent="M7-W01-C03";
const string PriorPhase="M7-W01-C03-S01", Phase="M7-W01-C03-S02";
const string Task="M7-W01-C03-S02-T1";
const string Question="M7-W01-C03-S02-T1-A";
const string Observation="M7-W01-C03-S02-T1-B";
const string DecisionNode="M7-W01-C03-S02-T1-C";
const string Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s02-t1.md";
const string OwnerScreen="docs/planning/decisions/m7-w01-c03-s02-ownership.md";
const string Oracle="docs/planning/ValidateM7PlotSampling.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string Status="m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s02-t1-reciprocal-segments/1.2 qualified; segments=2; finite=4; pole-break=true; max-points=8; renderer=false";
const string SegmentsMarker="M7-PLOT-ORACLE:RECIPROCAL:FINITE=4;SEGMENTS=2;GAPS=1;BREAKS=1;ZERO-OMITTED-BREAK=true";
const string RefusalMarker="M7-PLOT-ORACLE:REFUSALS:MAX-SAMPLES=8;MAX-POINTS=8;MAX-SEGMENTS=2;NONFINITE=true;ORDER=true;PARTIAL=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS113",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS112",1,
    "docs: qualify pure DAG deferral and close M7 S01",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.111"),JsonValue.Create("0.1.112"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.112":"0.1.111"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified"));
p.Require.JsonValueEquals(Prior,"/disposition",
    JsonValue.Create("defer-unqualified-pure-dag-runtime-and-providers"));
p.Require.JsonValueEquals(Prior,"/prior_reference_root_value",JsonValue.Create(13));
p.Require.JsonValueEquals(Prior,"/s01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/plot_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/parent_c03_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Split,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Split,"/new_phase_count",JsonValue.Create(2));
p.Require.JsonValueEquals(Split,"/parent_chunk_still_planned",JsonValue.Create(true));
p.Require.JsonValueEquals(Split,"/plot_split_active",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");
foreach(var (ptr,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/52/status","done"),
    ("/chunks/53/id",Parent),
    ("/chunks/53/status","planned"),
    ("/chunks/53/size","L"),
    ("/chunks/54/id",PriorPhase),
    ("/chunks/54/status","done"),
    ("/chunks/55/id",Phase),
    ("/chunks/55/kind","spike"),
    ("/chunks/55/size","M"),
    ("/chunks/55/parent_chunk",Parent),
    ("/chunks/55/status",target?"in_progress":"planned"),
    ("/chunks/55/tasks/0/id",Task),
    ("/chunks/55/tasks/0/status",target?"done":"planned"),
    ("/chunks/55/tasks/1/id","M7-W01-C03-S02-T2"),
    ("/chunks/55/tasks/1/status","planned"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,ptr,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/55/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/55/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create("planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(PriorPhase,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C03-S02-T2","M7-W01-C03-S02-T2-A",
    "M7-W01-C03-S02-T2-B","M7-W01-C03-S02-T2-C",
    Parent+"-T1",Parent+"-T2","M7-W02"})
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
    p.Require.JsonValueEquals(Qualified,"/reference_samples",JsonValue.Create(5));
    p.Require.JsonValueEquals(Qualified,"/reference_finite_points",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/reference_segments",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/reference_pole_gaps",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/reference_breaks",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/pole_omitted_breaks",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/research_max_samples",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/research_max_points",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/research_max_segments",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/no_partial_result_on_refusal",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/pole_not_bridged",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/s02_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_plot_model_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/plot_renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+OwnerScreen,OwnerScreen);
var oracleWrite=p.Files.ReplaceFromStaged("staged/"+Oracle,Oracle);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged S02 T1 PlotModel decision SHA-256 is absent.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged S02 T1 owner screen SHA-256 is absent.");
var oracleSha=oracleWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged independent finite PlotModel oracle SHA-256 is absent.");
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "[-2, -1, 0, 1, 2]","two disconnected segments","one pole gap",
    "no product PlotModel renderer","S02-T2","AURA"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Independent reciprocal segment oracle",
    "## Responsibility matrix","## T2 admission / deferral criteria",
    "eight samples","AURA/client owner","source-free"})
    p.Require.TextContains(OwnerScreen,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Oracle,SegmentsMarker);
p.Require.TextContains(Oracle,RefusalMarker);
var before=Required(root,"dotnet","run","--file",Oracle,"--","--check");
foreach(var marker in new[]{SegmentsMarker,RefusalMarker,Marker})
    if(!before.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent reciprocal PlotModel oracle was not observed: "+marker);

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-S02-T1-qualified",
        ["status"]=Status,
        ["question"]="Can a finite reciprocal-only PlotModel point/segment study preserve the discontinuity at zero without integrating UI rendering or AURA?",
        ["observation"]="The local BCL 1/x reference grid [-2,-1,0,1,2] yields four finite points, two segments, one gap and one break; omitting zero still produces a break, while invalid/over-budget inputs refuse without partial points.",
        ["decision"]="Admit finite PlotModel point/segment research witnesses only. Defer public PlotModel or chart renderer and any adaptive/general discontinuity semantics to a separately qualified owner/consumer gate.",
        ["prior_dag_deferral"]=Prior,
        ["prior_size_l_split"]=Split,
        ["oracle_validator"]=Oracle,
        ["oracle_validator_sha256"]=oracleSha,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["owner_screen"]=OwnerScreen,
        ["owner_screen_sha256"]=screenSha,
        ["independent_oracle_groups"]=2,
        ["reference_function"]="1/x",
        ["reference_samples"]=5,
        ["reference_sample_positions"]="-2,-1,0,1,2",
        ["reference_finite_points"]=4,
        ["reference_segments"]=2,
        ["reference_pole_gaps"]=1,
        ["reference_breaks"]=1,
        ["pole_omitted_breaks"]=1,
        ["research_max_samples"]=8,
        ["research_max_points"]=8,
        ["research_max_segments"]=2,
        ["pole_not_bridged"]=true,
        ["nonfinite_sample_rejected"]=true,
        ["nonmonotonic_or_duplicate_grid_rejected"]=true,
        ["nonfinite_reciprocal_rejected"]=true,
        ["over_budget_rejected"]=true,
        ["no_partial_result_on_refusal"]=true,
        ["local_bcl_oracles_executed"]=true,
        ["new_math_product_tests_executed"]=false,
        ["new_math_package_built"]=false,
        ["public_plot_model_api_delivered"]=false,
        ["plot_renderer_delivered"]=false,
        ["adaptive_sampling_delivered"]=false,
        ["general_discontinuity_certified"]=false,
        ["aura_job_engine_copied"]=false,
        ["external_plot_provider_adopted"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["s02_phase_done"]=false,
        ["parent_c03_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03-S02-T2: independently qualify sample/segment byte and point budgets, known-domain discontinuity policy, cancellation and Math/AURA renderer ownership or defer public PlotModel with a review trigger.",
        ["nonclaims"]=p.Json.StringArray(
            "One reciprocal-only finite oracle does not prove general discontinuity discovery, correct interpolation, plotting precision or a shipped public PlotModel API.",
            "No AURA execution job engine, chart renderer, UI library, remote provider or third-party NuGet was installed, published or integrated.",
            "Internal 8-sample/8-point/2-segment caps are research fixture limits, not public product availability guarantees.",
            "S02 T2, original size-L C03, M7-W01 and M7 remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.111")
            throw new InvalidOperationException("Unexpected pre-RS113 M7 planning version.");
        var milestone=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var s01=doc["chunks"]!.AsArray()[54]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[55]!.AsObject();
        var task=phase["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)milestone["id"]!=Milestone ||
           (string?)milestone["status"]!="in_progress" ||
           (string?)work["id"]!=Work ||
           (string?)work["status"]!="in_progress" ||
           (string?)parent["id"]!=Parent ||
           (string?)parent["status"]!="planned" ||
           (string?)s01["id"]!=PriorPhase ||
           (string?)s01["status"]!="done" ||
           (string?)phase["id"]!=Phase ||
           (string?)phase["kind"]!="spike" ||
           (string?)phase["parent_chunk"]!=Parent ||
           (string?)phase["status"]!="planned" ||
           (string?)task["id"]!=Task ||
           (string?)task["status"]!="planned" ||
           (string?)phase["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("M7 reciprocal S02-T1 pre-run lifecycle differs.");
        doc["plan_version"]="0.1.112";
        phase["status"]="in_progress";
        phase["refinement"]="RS112 closed S01 as independently qualified pure DAG research with no runtime. RS113 S02 T1 independently qualifies finite reciprocal 1/x point/segment data: four points and two unconnected segments across the known pole, including absent-zero gaps, plus 8-sample/8-point/2-segment and invalid/nonfinite refusal budgets. No renderer or public PlotModel API.";
        phase["readiness"]="S02 T1 Q/O/D finite reciprocal research qualified by BCL-only reference oracles, explicit known-pole topology and no-partial-result refusals. S02 T2 must still decide provider/owner, point budget and PlotModel product admission or deferral.";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,OwnerScreen,Oracle,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7PlotSampling.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)sub["status"]!="planned")
                throw new InvalidOperationException("Reciprocal T1 Q/O/D child was not planned.");
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var node in new[]{Phase,Task,Question})
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
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(PriorPhase,"done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C03-S02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M7-W01-C03-T1","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS113" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS113 reciprocal PlotModel research cursor.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Oracle,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Qualified RS113 plot study target did not satisfy independent BCL oracle.");
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
        throw new InvalidOperationException("RS113 bounded PlotModel oracle/planning command did not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS113 independent PlotModel validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS113 PlotModel research validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2300)]);
    return output;
}
