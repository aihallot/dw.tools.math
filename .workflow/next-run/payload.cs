using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Parent="M7-W01-C03";
const string S01="M7-W01-C03-S01", Phase="M7-W01-C03-S02";
const string Task="M7-W01-C03-S02-T2", Question="M7-W01-C03-S02-T2-A";
const string Observation="M7-W01-C03-S02-T2-B", DecisionNode="M7-W01-C03-S02-T2-C";
const string Backlog="docs/planning/backlog.json", State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Dag="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string PriorOracle="docs/planning/ValidateM7PlotSampling.cs";
const string Decision="docs/planning/decisions/m7-w01-c03-s02-t2.md";
const string Distribution="docs/distribution/m7-plotmodel-deferral.md";
const string Validator="docs/planning/ValidateM7PlotBoundary.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string Status="m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s02-t2-plotmodel-deferral/1.2 qualified; triggers=3; renderer=false; s02=done";
const string PriorMarker="dw.tools.math/m7-w01-c03-s02-t1-reciprocal-segments/1.2 qualified; segments=2; finite=4; pole-break=true; max-points=8; renderer=false";
const string Trigger1="A real Math consumer specifies finite function/domain, known singularity, sample grid and immutable point/segment output unavailable from existing APIs.";
const string Trigger2="Operation-level point/segment/total byte/time budgets, sorted finite sample policy, typed invalid/nonfinite/partial/cancellation statuses and no pole bridging are accepted.";
const string Trigger3="If providers are proposed, exact source/version/SHA/license/RID/notices, offline rollback, adversarial oracles, source-free client and Math plus rendering owner approval are qualified.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS114",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS113",1,
    "docs: qualify M7 reciprocal PlotModel research oracles",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.112"),JsonValue.Create("0.1.113"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.113":"0.1.112"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_oracle_groups",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/reference_samples",JsonValue.Create(5));
p.Require.JsonValueEquals(Prior,"/reference_finite_points",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/reference_segments",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/reference_pole_gaps",JsonValue.Create(1));
p.Require.JsonValueEquals(Prior,"/reference_breaks",JsonValue.Create(1));
p.Require.JsonValueEquals(Prior,"/pole_omitted_breaks",JsonValue.Create(1));
p.Require.JsonValueEquals(Prior,"/research_max_samples",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/research_max_points",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/research_max_segments",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/pole_not_bridged",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/no_partial_result_on_refusal",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/s02_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/parent_c03_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Split,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Split,"/parent_chunk_still_planned",JsonValue.Create(true));
p.Require.JsonValueEquals(Split,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Dag,"/status",
    JsonValue.Create("m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified"));
p.Require.JsonValueEquals(Dag,"/s01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Dag,"/plot_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Dag,"/dag_runtime_api_delivered",JsonValue.Create(false));
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
    ("/chunks/52/status","done"),
    ("/chunks/53/id",Parent),
    ("/chunks/53/status","planned"),
    ("/chunks/53/size","L"),
    ("/chunks/54/id",S01),
    ("/chunks/54/status","done"),
    ("/chunks/55/id",Phase),
    ("/chunks/55/kind","spike"),
    ("/chunks/55/size","M"),
    ("/chunks/55/parent_chunk",Parent),
    ("/chunks/55/status",target?"done":"in_progress"),
    ("/chunks/55/tasks/0/id","M7-W01-C03-S02-T1"),
    ("/chunks/55/tasks/0/status","done"),
    ("/chunks/55/tasks/1/id",Task),
    ("/chunks/55/tasks/1/status",target?"done":"planned"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/55/tasks/0/subtasks/"+i+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/55/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C03-S02-T1","done");
foreach(var id in new[]{"M7-W01-C03-S02-T1-A",
    "M7-W01-C03-S02-T1-B","M7-W01-C03-S02-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{Parent+"-T1",Parent+"-T2","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/disposition",
        JsonValue.Create("defer-unqualified-plotmodel-and-rendering-provider"));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/deferred_provider_categories",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/prior_reference_finite_points",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/prior_reference_segments",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/s02_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_plot_model_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/plot_renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/adaptive_sampling_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_plot_provider_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/automatic_cutover",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged S02 Q/O/D decision SHA-256 missing.");
var distributionSha=distributionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged PlotModel owner distribution SHA-256 missing.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged independent PlotModel boundary SHA-256 missing.");
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "All three review triggers are necessary","No public PlotModel",
    "128 bytes","M7-W01-C03","source-free","AURA"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Historical independent reciprocal witnesses",
    "## Ownership and candidate screen","## Three mandatory review triggers",
    "If any review trigger is missing, remain deferred",
    "128 coordinate bytes","AURA/client-owned","source-free"})
    p.Require.TextContains(Distribution,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);

if(!target)
{
    var priorObservations=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!priorObservations.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS113 independent reciprocal research oracles did not requalify.");
}
var baselineCheck=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!baselineCheck.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS114 independently read-only deferral baseline refused.");

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-S02-T2-qualified",
        ["status"]=Status,
        ["question"]="Do finite reciprocal sampling oracles and an owner boundary admit a public PlotModel, general sampler or graphics provider?",
        ["observation"]="Historical RS113 SHA-pinned 1/x samples retain four points, two segments, a recognized pole gap and no bridge even if zero is omitted. No Math consumer, general interpolation/domain/error model, cancellation/memory limits or renderer was qualified.",
        ["decision"]="Defer public PlotModel, adaptive sampling, UI renderer and external graphics providers until all three independent owner/consumer, resource and source-free provider gates are met.",
        ["prior_plot_sampling_evidence"]=Prior,
        ["prior_dag_deferral"]=Dag,
        ["prior_split_evidence"]=Split,
        ["prior_oracle_validator"]=PriorOracle,
        ["prior_oracles_reexecuted_before_boundary_transition"]=true,
        ["prior_reference_oracle_groups"]=2,
        ["prior_reference_finite_points"]=4,
        ["prior_reference_segments"]=2,
        ["prior_reference_pole_gaps"]=1,
        ["prior_pole_omitted_breaks"]=1,
        ["prior_research_max_samples"]=8,
        ["prior_research_max_points"]=8,
        ["prior_research_max_segments"]=2,
        ["reference_coordinate_payload_bytes"]=128,
        ["reference_bytes_exclude_heap_and_metadata"]=true,
        ["disposition"]="defer-unqualified-plotmodel-and-rendering-provider",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["provider_categories_screened"]=4,
        ["deferred_provider_categories"]=3,
        ["new_plot_provider_categories_admitted"]=0,
        ["external_plot_provider_adopted"]=false,
        ["external_plot_library_installed"]=false,
        ["provider_license_verified"]=false,
        ["provider_rid_qualified"]=false,
        ["provider_source_free_consumer_qualified"]=false,
        ["public_plot_model_api_delivered"]=false,
        ["plot_renderer_delivered"]=false,
        ["adaptive_sampling_delivered"]=false,
        ["general_discontinuity_certified"]=false,
        ["aura_job_engine_copied"]=false,
        ["automatic_cutover"]=false,
        ["publicly_published"]=false,
        ["new_math_code_or_packages_built"]=false,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_document_sha256"]=distributionSha,
        ["boundary_validator"]=Validator,
        ["boundary_validator_sha256"]=validatorSha,
        ["t1_done"]=true,
        ["t2_done"]=true,
        ["s02_phase_done"]=true,
        ["parent_c03_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03 original aggregate T1/T2: independently reconcile S01 pure diamond and S02 1/x no-bridge evidence, quotas and explicit Math/AURA owner deferrals before completing C03.",
        ["nonclaims"]=p.Json.StringArray(
            "S02 spike is a deferred PlotModel API/renderer decision, not an implemented plotting library.",
            "Only the known reciprocal 1/x pole and exact small sampling fixture were checked; no general discontinuity or certified error theorem.",
            "The 128 coordinate bytes omit all allocations and are not a public package memory, latency or precision service level.",
            "M7 original size-L C03, its two aggregate tasks, M7-W01 and M7 release remain unfinished.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.112")
            throw new InvalidOperationException("RS114 baseline planning version changed.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var wp=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var s01=doc["chunks"]!.AsArray()[54]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[55]!.AsObject();
        var t2=phase["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)wp["id"]!=Work ||
            (string?)wp["status"]!="in_progress" ||
            (string?)parent["id"]!=Parent ||
            (string?)parent["status"]!="planned" ||
            (string?)s01["id"]!=S01 ||
            (string?)s01["status"]!="done" ||
            (string?)phase["id"]!=Phase ||
            (string?)phase["kind"]!="spike" ||
            (string?)phase["parent_chunk"]!=Parent ||
            (string?)phase["status"]!="in_progress" ||
            (string?)phase["tasks"]!.AsArray()[0]!["status"]!="done" ||
            (string?)t2["id"]!=Task ||
            (string?)t2["status"]!="planned")
            throw new InvalidOperationException("Accepted M7 S02 T2 scope/parent baseline drifted.");
        doc["plan_version"]="0.1.113";
        phase["status"]="done";
        phase["refinement"]="RS113 T1 independently qualified 1/x finite four-point/two-segment no-bridge witnesses, skipped-zero pole and conservative 8-sample/8-point/2-segment nonfinite/invalid/budget refusals. RS114 T2 retains these oracles and explicitly defers public PlotModel, adaptive sampling and AURA/client rendering pending named caller, finite budget/error and source-free owner review. No renderer or model runtime delivered.";
        phase["readiness"]="S02 research done by SHA-pinned independent finite reciprocal oracles and deferred public plotting/graphics ownership decision. The original size-L C03 aggregate is still planned, requiring its own T1/T2 cross-S01/S02 acceptance and preserved Math/AURA non-delivery.";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var oldEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7PlotBoundary.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t2["status"]="done";
        t2["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t2["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("S02 T2 Q/O/D child was not planned.");
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
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(S01,"done");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Parent+"-T1","not-ready");
p.ProjectPlan.RequireNodeState(Parent+"-T2","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS114" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS114 PlotModel S02 research cursor changed.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!finalGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Completed RS114 negative PlotModel gate marker was not observed.");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS114 bounded .NET research command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS114 bounded validator timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS114 bounded local PlotModel validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
