using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Parent="M7-W01-C03";
const string Phase="M7-W01-C03-S01", Plot="M7-W01-C03-S02";
const string Task="M7-W01-C03-S01-T2";
const string Question="M7-W01-C03-S01-T2-A";
const string Observation="M7-W01-C03-S01-T2-B";
const string DecisionNode="M7-W01-C03-S01-T2-C";
const string Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string PriorOracle="docs/planning/ValidateM7DagAdmission.cs";
const string Decision="docs/planning/decisions/m7-w01-c03-s01-t2.md";
const string Distribution="docs/distribution/m7-pure-dag-deferral.md";
const string Validator="docs/planning/ValidateM7DagBoundary.cs";
const string Qualified="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Status="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified";
const string Marker="dw.tools.math/m7-w01-c03-s01-t2-pure-dag-deferral/1.2 qualified; triggers=3; runtime=false; aura-scheduler=false; s01=done";
const string PriorMarker="dw.tools.math/m7-w01-c03-s01-t1-pure-dag-screen/1.2 qualified; diamond=13; shared=once; cycle=refused; limits=8/10/4; runtime=false";
const string Trigger1="An admitted named Math consumer and immutable finite operation set demonstrate a shared pure-DAG need beyond existing Math functionality.";
const string Trigger2="Explicit graph identity/alias, pure memoization, deterministic cycle/unknown-node and overflow refusals, cancellation, deadline and finite resource policy.";
const string Trigger3="If a new provider is needed, qualified source SHA/version/license/RID/notices, offline rollback, adverse exact oracles, isolated source-free consumer and owner authorization.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS112",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS111",1,
    "docs: qualify pure M7 diamond DAG research oracles",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.110"),JsonValue.Create("0.1.111"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.111":"0.1.110"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified"));
p.Require.JsonValueEquals(Prior,"/reference_root_value",JsonValue.Create(13));
p.Require.JsonValueEquals(Prior,"/reference_node_count",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/reference_edge_count",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/reference_shared_evaluations",JsonValue.Create(1));
p.Require.JsonValueEquals(Prior,"/research_max_nodes",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/research_max_edges",JsonValue.Create(10));
p.Require.JsonValueEquals(Prior,"/research_max_depth",JsonValue.Create(4));
p.Require.JsonValueEquals(Prior,"/cycle_refused",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/no_partial_result_on_refusal",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/s01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/aura_job_engine_copied",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/status",
    JsonValue.Create("m7-c03-bounded-dag-plotmodel-split-qualified"));
p.Require.JsonValueEquals(Split,"/native_nodes_after",JsonValue.Create(541));
p.Require.JsonValueEquals(Split,"/dag_split_active",JsonValue.Create(true));
p.Require.JsonValueEquals(Split,"/plot_split_active",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/dag_runtime_api_delivered",JsonValue.Create(false));
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
    ("/chunks/54/id",Phase),
    ("/chunks/54/kind","spike"),
    ("/chunks/54/parent_chunk",Parent),
    ("/chunks/54/status",target?"done":"in_progress"),
    ("/chunks/54/tasks/0/id","M7-W01-C03-S01-T1"),
    ("/chunks/54/tasks/0/status","done"),
    ("/chunks/54/tasks/1/id",Task),
    ("/chunks/54/tasks/1/status",target?"done":"planned"),
    ("/chunks/55/id",Plot),
    ("/chunks/55/status","planned"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,ptr,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/54/tasks/0/subtasks/"+i+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/54/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C03-S01-T1","done");
foreach(var id in new[]{"M7-W01-C03-S01-T1-A",
    "M7-W01-C03-S01-T1-B","M7-W01-C03-S01-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{Plot,Plot+"-T1",Plot+"-T2",
    Parent+"-T1",Parent+"-T2","M7-W02"})
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
        JsonValue.Create("defer-unqualified-pure-dag-runtime-and-providers"));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/deferred_provider_categories",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/prior_reference_root_value",JsonValue.Create(13));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/s01_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/parent_c03_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/plot_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/dag_runtime_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/plot_renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_dag_provider_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/automatic_cutover",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
}
var decisionResult=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionResult=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var validatorResult=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionResult.AfterSha256 ??
    throw new InvalidOperationException("Staged DAG T2 Q/O/D decision SHA-256 missing.");
var distributionSha=distributionResult.AfterSha256 ??
    throw new InvalidOperationException("Staged DAG T2 owner screen SHA-256 missing.");
var validatorSha=validatorResult.AfterSha256 ??
    throw new InvalidOperationException("Staged BCL-only DAG boundary SHA-256 missing.");

foreach(var snippet in new[]{"## Question","## Observation","## Decision",
    "All three triggers are necessary","No public DAG API",
    "M7-W01-C03-S02","AURA owns","source-free"})
    p.Require.TextContains(Decision,snippet,StringComparison.OrdinalIgnoreCase);
foreach(var snippet in new[]{"## Retained finite independent reference",
    "## Ownership/provider matrix","## Three review triggers",
    "If any review trigger is missing, remain deferred",
    "AURA-owned","source-free consumer"})
    p.Require.TextContains(Distribution,snippet,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);

if(!target)
{
    var priorResult=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!priorResult.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Historical bounded RS111 DAG numeric oracles did not requalify.");
}
var beforeGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!beforeGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent pure DAG T2 deferral baseline refused.");

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C03-S01-T2-qualified",
        ["status"]=Status,
        ["question"]="Can a public Math DAG engine or AURA scheduling integration be admitted from the finite RS111 diamond/reference oracles alone?",
        ["observation"]="The SHA-bound four-node root-13 pure DAG oracle remains valid; no named consumer, independently qualified new runtime/provider, cancellation/resource policy or isolated package-only consumer has been admitted.",
        ["decision"]="Defer unqualified pure Math DAG runtimes, external graph providers and AURA scheduling integration until three owner-gated review triggers and independent source-free qualification.",
        ["prior_dag_research_evidence"]=Prior,
        ["prior_split_evidence"]=Split,
        ["prior_oracle_validator"]=PriorOracle,
        ["prior_oracles_reexecuted_before_boundary_transition"]=true,
        ["prior_reference_oracle_groups"]=2,
        ["prior_reference_root_value"]=13,
        ["prior_reference_shared_evaluations"]=1,
        ["prior_research_max_nodes"]=8,
        ["prior_research_max_edges"]=10,
        ["prior_research_max_depth"]=4,
        ["disposition"]="defer-unqualified-pure-dag-runtime-and-providers",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["provider_categories_screened"]=4,
        ["deferred_provider_categories"]=3,
        ["new_dag_provider_categories_admitted"]=0,
        ["external_dag_provider_adopted"]=false,
        ["external_dag_library_installed"]=false,
        ["provider_license_verified"]=false,
        ["provider_rid_qualified"]=false,
        ["provider_source_free_consumer_qualified"]=false,
        ["dag_runtime_api_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["plot_renderer_delivered"]=false,
        ["plot_model_api_delivered"]=false,
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
        ["s01_phase_done"]=true,
        ["parent_c03_done"]=false,
        ["plot_phase_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C03-S02-T1: independently research finite 1/x sampling, discontinuity segment breaks and point budgets without UI renderer or AURA scheduling.",
        ["nonclaims"]=p.Json.StringArray(
            "A completed S01 pure DAG research phase does not deliver a public Math DAG or caching runtime.",
            "No provider source/version/license/RID, source-free consumer, AURA executor, PlotModel renderer or published NuGet was qualified.",
            "Research limits 8 nodes, 10 edges and depth 4 do not establish a production numerical/resource support contract.",
            "S02, the original C03 size-L aggregate, M7-W01 and M7 remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.110")
            throw new InvalidOperationException("M7 S01 T2 predecessor backlog version drifted.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var parent=doc["chunks"]!.AsArray()[53]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[54]!.AsObject();
        var plot=doc["chunks"]!.AsArray()[55]!.AsObject();
        var t2=phase["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)work["id"]!=Work ||
            (string?)work["status"]!="in_progress" ||
            (string?)parent["id"]!=Parent ||
            (string?)parent["status"]!="planned" ||
            (string?)phase["id"]!=Phase ||
            (string?)phase["parent_chunk"]!=Parent ||
            (string?)phase["kind"]!="spike" ||
            (string?)phase["status"]!="in_progress" ||
            (string?)phase["tasks"]!.AsArray()[0]!["status"]!="done" ||
            (string?)t2["id"]!=Task ||
            (string?)t2["status"]!="planned" ||
            (string?)plot["id"]!=Plot ||
            (string?)plot["status"]!="planned")
            throw new InvalidOperationException("M7 S01 pure DAG research Q/O/D baseline drifted.");
        doc["plan_version"]="0.1.111";
        phase["status"]="done";
        phase["refinement"]="RS111 qualified four-node/four-edge pure DAG root-13 and shared-once, exact cycle/identity/resource/overflow refusals with no partial results. RS112 T2 explicitly defers public DAG engines and AURA orchestration until consumer need, pure graph identity/cancellation/budget policy, source/rights/RID and independent package-only recipient oracles are qualified. S01 closes as research only.";
        phase["readiness"]="S01 research spike done through SHA-verified local pure graph oracles and an explicit defer-with-review-trigger decision; no public DAG runtime, scheduler or graphics renderer was delivered. S02 remains planned and depends on S01.";
        var files=phase["files"]!.AsArray().Select(item=>item!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=phase["evidence"]!.AsArray().Select(item=>item!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7DagBoundary.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t2["status"]="done";
        t2["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t2["subtasks"]!.AsArray().Select(item=>item!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("S01 T2 Q/O/D subtask was not planned.");
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
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState(Plot,"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C03-S02-T1","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS112" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?null:Phase) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS112 pure DAG research cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!finalGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Completed RS112 DAG owner deferral gate marker absent.");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,
        UseShellExecute=false,
        RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS112 bounded local validation could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS112 bounded local validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS112 DWF local decision gate failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2300)]);
    return output;
}
