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
const string Task="M7-W01-C03-T2", Question="M7-W01-C03-T2-A";
const string Observation="M7-W01-C03-T2-B", DecisionNode="M7-W01-C03-T2-C";
const string Backlog="docs/planning/backlog.json", State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string Dag="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Plot="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string PriorOracle="docs/planning/ValidateM7DagPlotAggregate.cs";
const string Decision="docs/planning/decisions/m7-w01-c03-t2.md";
const string Distribution="docs/distribution/m7-dag-plot-closure.md";
const string Checker="docs/planning/ValidateM7DagPlotClosure.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-T2-qualified.json";
const string Status="m7-c03-t2-original-aggregate-owner-gated-research-closure-qualified";
const string Marker="dw.tools.math/m7-w01-c03-t2-original-research-closure/1.2 qualified; s01=done; s02=done; c03=done; runtime=false; renderer=false";
const string PriorMarker="dw.tools.math/m7-w01-c03-t1-independent-aggregate/1.2 qualified; dag-root=13; reciprocal-points=4; segments=2; children=done; product=false";
const string Trigger1="A real Math/client consumer and finite pure graph or known-singularity point/segment domain show a measurable gap in admitted Math APIs.";
const string Trigger2="Immutable graph identity/purity, cycle/alias/cancellation and no-partial policy plus known-pole plot topology, point/segment/time/total allocation byte budgets.";
const string Trigger3="Pinned source/version/SHA/license/notices and target TFM/RID with independent adverse oracles, offline rollback, source-free consumer and Math plus client owner approval.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS116",Milestone,Work,target?null:Parent,null,null);
p.Workflow.RequirePriorSuccess("RS115",1,
    "docs: qualify M7 DAG and PlotModel aggregate research",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.114"),JsonValue.Create("0.1.115"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.115":"0.1.114"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c03-t1-aggregate-pure-dag-and-reciprocal-plot-research-qualified"));
p.Require.JsonValueEquals(Prior,"/prior_receipts_verified",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/independent_research_families",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(Prior,"/diamond_root_value",JsonValue.Create(13));
p.Require.JsonValueEquals(Prior,"/diamond_shared_evaluations",JsonValue.Create(1));
p.Require.JsonValueEquals(Prior,"/reciprocal_finite_points",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/reciprocal_segments",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/known_pole_omitted_break_preserved",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/children_both_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/parent_c03_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_dag_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Dag,"/status",
    JsonValue.Create("m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified"));
p.Require.JsonValueEquals(Dag,"/disposition",
    JsonValue.Create("defer-unqualified-pure-dag-runtime-and-providers"));
p.Require.JsonValueEquals(Dag,"/s01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Dag,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Dag,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Plot,"/status",
    JsonValue.Create("m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified"));
p.Require.JsonValueEquals(Plot,"/disposition",
    JsonValue.Create("defer-unqualified-plotmodel-and-rendering-provider"));
p.Require.JsonValueEquals(Plot,"/s02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Plot,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Plot,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Split,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Split,"/new_phase_count",JsonValue.Create(2));
p.Require.JsonValueEquals(Split,"/parent_chunk_still_planned",JsonValue.Create(true));
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
    ("/work_packages/16/id","M7-W02"),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/status","done"),
    ("/chunks/52/status","done"),
    ("/chunks/53/id",Parent),
    ("/chunks/53/kind","spike"),
    ("/chunks/53/size","L"),
    ("/chunks/53/status",target?"done":"in_progress"),
    ("/chunks/53/tasks/0/id","M7-W01-C03-T1"),
    ("/chunks/53/tasks/0/status","done"),
    ("/chunks/53/tasks/1/id",Task),
    ("/chunks/53/tasks/1/status",target?"done":"planned"),
    ("/chunks/54/id",S01),
    ("/chunks/54/status","done"),
    ("/chunks/55/id",S02),
    ("/chunks/55/status","done"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,ptr,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/53/tasks/0/subtasks/"+i+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/53/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(S02,"done");
p.ProjectPlan.RequireNodeState(Parent,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C03-T1","done");
foreach(var id in new[]{"M7-W01-C03-T1-A","M7-W01-C03-T1-B","M7-W01-C03-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M7-W02","not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Checker,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/disposition",
        JsonValue.Create("research-complete-products-deferred"));
    p.Require.JsonValueEquals(Qualified,"/prior_aggregate_receipts",JsonValue.Create(5));
    p.Require.JsonValueEquals(Qualified,"/native_nodes_retained",JsonValue.Create(541));
    p.Require.JsonValueEquals(Qualified,"/research_family_count",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/s01_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/s02_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_dag_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_plot_model_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_provider_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var checkerWrite=p.Files.ReplaceFromStaged("staged/"+Checker,Checker);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged original C03 T2 Q/O/D decision SHA-256 absent.");
var distributionSha=distributionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged C03 final owner gate SHA-256 absent.");
var checkerSha=checkerWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged C03 final independent read-only gate SHA-256 absent.");
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "M7-W01-C03-S01","M7-W01-C03-S02",
    "All three review triggers are necessary",
    "no public DAG API","no public PlotModel API",
    "128 coordinate bytes","M7-W01","AURA","remain deferred"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Source-pinned accepted evidence",
    "## Ownership and cost screen","## Three re-entry triggers",
    "If any trigger is missing, remain deferred",
    "AURA/client-owned","source-free","128 coordinate bytes","541"})
    p.Require.TextContains(Distribution,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Checker,Marker);
if(!target)
{
    var originalStudy=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!originalStudy.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("The RS115 independently qualified C03 aggregate did not replay.");
}
var baselineCheck=Required(root,"dotnet","run","--file",Checker,"--","--check");
if(!baselineCheck.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("The RS116 independent owner/closure checker refused the baseline.");
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-T2-qualified",
        ["status"]=Status,
        ["question"]="Can the original size-L C03 research aggregate close while public DAG/PlotModel/renderer capabilities remain expressly deferred?",
        ["observation"]="RS115 original aggregate and its four child receipts preserve exact diamond-13/shared-once and four reciprocal points/two segments with no bridge over zero, but no qualified public provider/consumer or total memory/error/owner gates.",
        ["decision"]="Complete original C03 as a research spike only; retain S01 DAG runtime and S02 PlotModel/renderer deferrals pending named consumer, complete bounded semantics and exact source/recipient/owner review.",
        ["prior_aggregate_evidence"]=Prior,
        ["prior_dag_deferral"]=Dag,
        ["prior_plot_deferral"]=Plot,
        ["prior_split_evidence"]=Split,
        ["prior_aggregate_receipts"]=5,
        ["native_nodes_retained"]=541,
        ["research_family_count"]=2,
        ["reference_dag_root"]=13,
        ["reference_dag_shared_once"]=true,
        ["reference_plot_finite_points"]=4,
        ["reference_plot_segments"]=2,
        ["reference_known_pole_no_bridge"]=true,
        ["research_dag_max_nodes"]=8,
        ["research_dag_max_edges"]=10,
        ["research_dag_max_depth"]=4,
        ["research_plot_max_samples"]=8,
        ["research_plot_max_points"]=8,
        ["research_plot_max_segments"]=2,
        ["illustrative_plot_coordinate_payload_bytes"]=128,
        ["total_allocation_budget_qualified"]=false,
        ["complete_cancellation_policy_qualified"]=false,
        ["production_numeric_error_bound_qualified"]=false,
        ["disposition"]="research-complete-products-deferred",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["dag_runtime_provider_deferred"]=true,
        ["plot_model_and_renderer_deferred"]=true,
        ["s01_done"]=true,
        ["s02_done"]=true,
        ["t1_done"]=true,
        ["t2_done"]=true,
        ["parent_c03_done"]=true,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["public_dag_api_delivered"]=false,
        ["public_plot_model_api_delivered"]=false,
        ["renderer_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["external_provider_adopted"]=false,
        ["advanced_math_product_built"]=false,
        ["new_math_product_tests_executed"]=false,
        ["publicly_published"]=false,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=distributionSha,
        ["closure_validator"]=Checker,
        ["closure_validator_sha256"]=checkerSha,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01 work-package closure: independently reconcile precision, tensor, DAG and PlotModel research decisions, downstream ownership, resource limits and distinct M7-W02 pending work before W01 can close.",
        ["nonclaims"]=p.Json.StringArray(
            "Research C03 closure is not delivery of a public DAG, PlotModel, graphics engine or AURA task scheduler.",
            "No arbitrary-function continuity theorem, total memory/latency SLA, numerical error certification, native provider or isolated product consumer was qualified.",
            "No advanced package, public NuGet, dw.discovery consumer or Decision/MCDM/AURA cross-repository adoption was authorized.",
            "M7-W01 and M7 remain in progress, M7-W02 work package remains planned.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.114")
            throw new InvalidOperationException("Original M7 C03 T2 baseline version drifted.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var wp2=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var s01=doc["chunks"]!.AsArray()[54]!.AsObject();
        var s02=doc["chunks"]!.AsArray()[55]!.AsObject();
        var t2=parent["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone ||
           (string?)release["status"]!="in_progress" ||
           (string?)work["id"]!=Work ||
           (string?)work["status"]!="in_progress" ||
           (string?)wp2["id"]!="M7-W02" ||
           (string?)wp2["status"]!="planned" ||
           (string?)parent["id"]!=Parent ||
           (string?)parent["kind"]!="spike" ||
           (string?)parent["size"]!="L" ||
           (string?)parent["status"]!="in_progress" ||
           (string?)parent["tasks"]!.AsArray()[0]!["status"]!="done" ||
           (string?)s01["id"]!=S01 ||
           (string?)s01["status"]!="done" ||
           (string?)s02["id"]!=S02 ||
           (string?)s02["status"]!="done" ||
           (string?)t2["id"]!=Task ||
           (string?)t2["status"]!="planned")
            throw new InvalidOperationException("M7 original C03 T2 parent/child lifecycle drifted.");
        doc["plan_version"]="0.1.115";
        parent["status"]="done";
        parent["refinement"]="RS110 split the original size-L C03 into independent medium S01 pure DAG and S02 reciprocal PlotModel studies. RS111-RS114 completed both with source-pinned finite oracles and public runtime/renderer deferral. RS115 T1 recomputed diamond root 13/shared-once and 1/x two segments with skipped-zero break. RS116 T2 closes only the accepted original C03 research with bounded ownership and three future provider/consumer review triggers. No product DAG or PlotModel delivered.";
        parent["readiness"]="Original size-L C03 research qualified and complete with independent cross-family BCL evidence, owner-gated product deferrals, no-partial-result refusal and finite study budgets. M7-W01 remains in-progress pending its distinct work-package closure; M7-W02 and M7 release remain open.";
        var paths=parent["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        parent["files"]=p.Json.StringArray(paths.Concat(new[]{
            Decision,Distribution,Checker,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=parent["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        parent["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        parent["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7DagPlotClosure.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t2["status"]="done";
        t2["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t2["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("Original C03 T2 Q/O/D subtask was not planned.");
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
    p.ProjectPlan.ConvergeNodeToDone(Parent);
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(S02,"done");
p.ProjectPlan.RequireNodeState(Parent,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS116" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Parent) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS116 original C03 research closure cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalGate=Required(root,"dotnet","run","--file",Checker,"--","--check");
if(!finalGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Qualified original C03 research closure failed independent target gate.");
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
        throw new InvalidOperationException("RS116 bounded C# reference/plan command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS116 bounded independent validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS116 local C03 research validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2500)]);
    return output;
}
