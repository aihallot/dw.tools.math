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
const string Backlog="docs/planning/backlog.json";
const string Plan=".aura/workflow/plan/project.json";
const string State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string M6Gate="docs/planning/evidence/M6-gate.json";
const string Decision="docs/planning/decisions/m7-w01-c03-split.md";
const string SplitValidator="docs/planning/ValidateM7DagSplit.cs";
const string NativeValidator="docs/planning/ValidateNativeDwfAdoption.cs";
const string Evidence="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Status="m7-c03-bounded-dag-plotmodel-split-qualified";
const string Marker="dw.tools.math/m7-w01-c03-independent-split/1.2 qualified; phases=2; tasks=4; subtasks=12; nodes=541; renderer=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS110",Milestone,Work,target?S01:null,null,null);
p.Workflow.RequirePriorSuccess("RS109",1,
    "docs: qualify tensor provider deferral and close M7 C02",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified"));
p.Require.JsonValueEquals(Prior,"/m7_w01_c02_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/advanced_tensor_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_tensor_provider_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(M6Gate,"/status",
    JsonValue.Create("m6-independent-local-analysis-scientific-release-gate-qualified-unpublished"));
p.Require.JsonValueEquals(M6Gate,"/m6_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(M6Gate,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.108"),JsonValue.Create("0.1.109"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.109":"0.1.108"));
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
    ("/chunks/53/tasks/0/status","planned"),
    ("/chunks/53/tasks/1/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState("M7-W02","not-ready");
p.Require.TextContains(NativeValidator,
    target?"Require(expected.Count == 541,":"Require(expected.Count == 523,");
if(!target)
    p.Require.FileMissing(Evidence);
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence,"/native_nodes_after",JsonValue.Create(541));
    p.Require.JsonValueEquals(Evidence,"/split_parent",JsonValue.Create(Parent));
    p.Require.JsonValueEquals(Evidence,"/new_phase_count",JsonValue.Create(2));
    p.Require.JsonValueEquals(Evidence,"/new_task_count",JsonValue.Create(4));
    p.Require.JsonValueEquals(Evidence,"/new_subtask_count",JsonValue.Create(12));
    p.Require.JsonValueEquals(Evidence,"/parent_chunk_still_planned",JsonValue.Create(true));
    p.Require.JsonValueEquals(Evidence,"/dag_split_active",JsonValue.Create(true));
    p.Require.JsonValueEquals(Evidence,"/plot_split_active",JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence,"/dag_runtime_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence,"/plot_renderer_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence,"/m7_release_done",JsonValue.Create(false));
}

var decisionResult=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var nativeResult=p.Files.ReplaceFromStaged("staged/"+NativeValidator,NativeValidator);
var checkResult=p.Files.ReplaceFromStaged("staged/"+SplitValidator,SplitValidator);
var decisionSha=decisionResult.AfterSha256 ??
    throw new InvalidOperationException("M7 DAG split decision staged SHA is absent.");
var nativeSha=nativeResult.AfterSha256 ??
    throw new InvalidOperationException("Native roadmap 541-node validator staged SHA is absent.");
var checkerSha=checkResult.AfterSha256 ??
    throw new InvalidOperationException("Independent split validator staged SHA is absent.");
foreach(var term in new[]{"M7-W01-C03-S01","M7-W01-C03-S02","before readiness",
    "diamond DAG","1/x","AURA","523","541","No new"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(NativeValidator,"Require(expected.Count == 541,");
p.Require.TextContains(SplitValidator,Marker);

if(!target)
{
    // The accepted aggregate identity is preserved; split children are added
    // before any future parent C03 readiness or product implementation.
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.108")
            throw new InvalidOperationException("Unrecognized pre-split backlog version.");
        var milestones=doc["releases"]!.AsArray();
        var release=milestones[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var chunks=doc["chunks"]!.AsArray();
        var parent=chunks[53]!.AsObject();
        if((string?)release["id"]!=Milestone ||
           (string?)release["status"]!="in_progress" ||
           (string?)work["id"]!=Work ||
           (string?)work["status"]!="in_progress" ||
           (string?)parent["id"]!=Parent ||
           (string?)parent["kind"]!="spike" ||
           (string?)parent["size"]!="L" ||
           (string?)parent["status"]!="planned" ||
           chunks.Any(c=>string.Equals((string?)c!["id"],S01,StringComparison.Ordinal)||string.Equals((string?)c!["id"],S02,StringComparison.Ordinal)))
            throw new InvalidOperationException("M7 C03 accepted baseline already diverged.");
        doc["plan_version"]="0.1.109";
        release["required_chunks"]=p.Json.StringArray(
            "M7-W01-C01","M7-W01-C02",Parent,S01,S02,
            "M7-W02-C01","M7-W02-C02");
        parent["refinement"]="RS110 decomposes the accepted size-L C03 research into S01 (pure DAG sharing/cycle/resource bounded study) and S02 (finite PlotModel discontinuity/point-quota study), preserving aggregate C03 Q/O/D and distinct AURA rendering ownership. The parent stays planned pending both split decisions.";
        parent["readiness"]="Size-L split completed structurally before any parent readiness: S01 active, S02 planned and sequentially dependent. Original C03 aggregate remains planned until both split research phases are independently qualified.";
        var originalIndex=chunks.ToList().FindIndex(c=>
            string.Equals((string?)c!["id"],Parent,StringComparison.Ordinal));
        if(originalIndex!=53)throw new InvalidOperationException("Unexpected C03 split insert index.");
        chunks.Insert(originalIndex+1,(JsonNode)BuildProductPhase(S01,
            "Pure bounded DAG sharing and cycle refusal","in_progress",
            "M7-W01-C02",
            "Study memoized pure diamond-graph evaluation, one shared node, deterministic cycle rejection and bounded node/edge/depth budgets.",
            "One diamond DAG evaluates the shared source only once and rejects a directed cycle; finite cap refuses before executing a large graph.",
            "No AURA scheduler, shared mutable resources, agent/job graph runtime or product DAG API is admitted."));
        chunks.Insert(originalIndex+2,(JsonNode)BuildProductPhase(S02,
            "Finite PlotModel and discontinuity sampling","planned",
            S01,
            "Study bounded point/segment representation for sampling finite mathematical functions without owning chart rendering.",
            "Finite 1/x sampling must not connect points across the discontinuity at zero; explicit point quotas and segment breaks.",
            "No UI/graphics engine, automatic interpolation over poles, dynamic stream or AURA rendering dependency."));
    });
    p.Json.EditObject(Plan,doc=>
    {
        var m7=doc["milestones"]!.AsArray()[7]!.AsObject();
        var w01=m7["workPackages"]!.AsArray()[0]!.AsObject();
        var phases=w01["phases"]!.AsArray();
        if((string?)m7["id"]!=Milestone ||
           (string?)w01["id"]!=Work ||
           (string?)phases[2]!["id"]!=Parent ||
           (string?)phases[2]!["state"]!="not-ready" ||
           phases.Any(c=>string.Equals((string?)c!["id"],S01,StringComparison.Ordinal)||string.Equals((string?)c!["id"],S02,StringComparison.Ordinal)))
            throw new InvalidOperationException("Unrecognized original native C03 parent.");
        phases.Insert(3,(JsonNode)BuildNativePhase(S01,
            "Pure bounded DAG sharing and cycle refusal","M7-W01-C02"));
        phases.Insert(4,(JsonNode)BuildNativePhase(S02,
            "Finite PlotModel and discontinuity sampling",S01));
    });
    p.ProjectPlan.RequireNodeState(S01,"not-ready");
    p.ProjectPlan.RequireNodeState(S02,"not-ready");
    p.ProjectPlan.RequireDirectChild(Work,S01);
    p.ProjectPlan.RequireDirectChild(Work,S02);
    p.ProjectPlan.TransitionNode(S01,"not-ready","ready");
    p.ProjectPlan.TransitionNode(S01,"ready","in-progress");
    p.ProjectPlan.RequireNodeState(S01,"in-progress");
    p.ProjectPlan.RequireNodeState(Parent,"not-ready");

    var manifest=new JsonObject
    {
        ["schema_version"]=1,["id"]="M7-W01-C03-split-qualified",
        ["status"]=Status,
        ["question"]="How must size-L DAG/PlotModel research be structurally split before readiness without inventing AURA scheduling or graph rendering?",
        ["observation"]="Two medium independent source/ownership research phases are projected into both accepted backlog and native DWF hierarchy; all 523 preexisting nodes retain their IDs and states.",
        ["decision"]="Split DAG sharing/cycles and discontinuous PlotModel sampling into two dependent size-M phases, activate only S01, leave original size-L C03 aggregate planned and both research tasks unstarted.",
        ["prior_tensor_deferral"]=Prior,
        ["split_parent"]=Parent,
        ["split_child_ids"]=p.Json.StringArray(S01,S02),
        ["new_phase_count"]=2,
        ["new_task_count"]=4,
        ["new_subtask_count"]=12,
        ["native_nodes_before"]=523,
        ["native_nodes_after"]=541,
        ["parent_chunk_still_planned"]=true,
        ["dag_split_active"]=true,
        ["plot_split_active"]=false,
        ["dag_oracles_executed"]=false,
        ["plot_oracles_executed"]=false,
        ["dag_runtime_api_delivered"]=false,
        ["plot_renderer_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["new_product_package_built"]=false,
        ["publicly_published"]=false,
        ["m7_release_done"]=false,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["native_validator"]=NativeValidator,
        ["native_validator_sha256"]=nativeSha,
        ["independent_split_validator"]=SplitValidator,
        ["independent_split_validator_sha256"]=checkerSha,
        ["next_scope"]="RS111 M7-W01-C03-S01-T1: independent bounded diamond DAG with memoized pure node, cycle refusal and node/edge/depth budgets; no AURA job runtime.",
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["nonclaims"]=p.Json.StringArray(
            "The size-L split is roadmap structure, not execution of diamond or 1/x computational oracles.",
            "No DAG engine, renderer, PlotModel runtime, AURA/Decision/MCDM adoption or graphics provider exists by this preparation.",
            "The original M7 C03 is not ready or complete; two split children need separate independently qualified research.",
            "All advanced packages remain local research-only and M7 release is in progress.")
    };
    p.Files.WriteComplete(Evidence,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01","done");
p.ProjectPlan.RequireNodeState("M7-W01-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState(S01,"in-progress");
p.ProjectPlan.RequireNodeState(S02,"not-ready");
foreach(var id in new[]{S01+"-T1",S01+"-T1-A",S01+"-T1-B",S01+"-T1-C",
    S01+"-T2",S02+"-T1",S02+"-T2"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS110" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?S01:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected DWF M7 split workflow cursor.");
    state["activePhase"]=S01;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var final=Required(root,"dotnet","run","--file",SplitValidator,"--","--check");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("M7 accepted split failed independent target gate.");
return p.Complete();

JsonObject BuildProductPhase(string id,string title,string status,string dependency,
    string deliverable,string acceptance,string boundary)
{
    return new JsonObject
    {
        ["id"]=id,
        ["tag"]="advanced",
        ["release"]=Milestone,
        ["work_package"]=Work,
        ["parent_chunk"]=Parent,
        ["title"]=title,
        ["kind"]="spike",
        ["size"]="M",
        ["status"]=status,
        ["depends_on"]=p.Json.StringArray(dependency),
        ["external_dependencies"]=p.Json.StringArray(),
        ["requirements"]=p.Json.StringArray(Parent+"-R1",Parent+"-R2"),
        ["deliverables"]=deliverable,
        ["acceptance"]=acceptance,
        ["boundary_acceptance"]=boundary,
        ["test_suites"]=p.Json.StringArray("PLANNING"),
        ["proposed_files"]=p.Json.StringArray(Decision),
        ["proposed_test_file"]=null,
        ["proposed_commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check"),
        ["refinement"]="RS110 accepted size-L split: independent finite research questions, no new Math/AURA runtime or provider qualification.",
        ["files"]=p.Json.StringArray(Decision,NativeValidator,SplitValidator,Evidence),
        ["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7DagSplit.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check"),
        ["evidence"]=p.Json.StringArray(),
        ["readiness"]=status=="in_progress"
            ? "S01 active after split; upcoming T1 must establish independent pure DAG sharing, cycle, resource and ownership oracles. No product engine admitted."
            : "S02 stays planned until S01 evidence; finite discontinuity/PlotModel oracle and point-quota review must be independently qualified.",
        ["tasks"]=p.Json.Array(
            (JsonNode)BuildProductTask(id+"-T1","Contract and outcome",deliverable,acceptance),
            (JsonNode)BuildProductTask(id+"-T2","Boundaries and integration",boundary,boundary))
    };
}
JsonObject BuildProductTask(string id,string title,string behavior,string testCase)
{
    return new JsonObject
    {
        ["id"]=id,["title"]=title,["behavior"]=behavior,
        ["test_case"]=testCase,["status"]="planned",
        ["evidence"]=p.Json.StringArray(),
        ["subtasks"]=p.Json.Array(
            (JsonNode)ProductSubtask(id+"-A","Question","Fix the independent finite research question, scope and non-delivery criterion."),
            (JsonNode)ProductSubtask(id+"-B","Observation","Record exact hand-checkable local BCL oracle, negative cases and numeric/resource limits."),
            (JsonNode)ProductSubtask(id+"-C","Decision","Document acceptance or deferral with owner, rights, resource and AURA boundary; never invent product delivery."))
    };
}
JsonObject ProductSubtask(string id,string title,string description)=>
    new()
    {
        ["id"]=id,["title"]=title,["description"]=description,
        ["status"]="planned",["evidence"]=p.Json.StringArray()
    };
JsonObject BuildNativePhase(string id,string title,string dependency)=>
    new()
    {
        ["id"]=id,["title"]=title,["state"]="not-ready",
        ["dependsOn"]=p.Json.StringArray(dependency),
        ["completionCondition"]="Complete independent two-task bounded size-M research and owner/non-delivery boundaries in docs/planning/backlog.json.",
        ["tasks"]=p.Json.Array(
            (JsonNode)NativeTask(id+"-T1","Contract and outcome"),
            (JsonNode)NativeTask(id+"-T2","Boundaries and integration"))
    };
JsonObject NativeTask(string id,string title)=>
    new()
    {
        ["id"]=id,["title"]=title,["state"]="not-ready",
        ["completionCondition"]="Satisfy accepted "+id+" research decision and oracle evidence from docs/planning/backlog.json.",
        ["subtasks"]=p.Json.Array(
            (JsonNode)NativeSubtask(id+"-A","Question"),
            (JsonNode)NativeSubtask(id+"-B","Observation"),
            (JsonNode)NativeSubtask(id+"-C","Decision"))
    };
JsonObject NativeSubtask(string id,string title)=>
    new()
    {
        ["id"]=id,["title"]=title,["state"]="not-ready",
        ["completionCondition"]="Satisfy accepted "+id+" scoped evidence or decision in docs/planning/backlog.json."
    };

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
        throw new InvalidOperationException("RS110 planning validator could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS110 controlled planning command timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS110 local DWF planning failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2500)]);
    return output;
}
