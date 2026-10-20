using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W01",NextWork="M7-W02";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string C01T1="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string C01T2="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string C02T1="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string C02T2="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string S01T1="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string S01T2="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string S02T1="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string S02T2="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string C03T1="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string C03T2="docs/planning/evidence/M7-W01-C03-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-research-closure.md";
const string Distribution="docs/distribution/m7-w01-advanced-research-matrix.md";
const string Checker="docs/planning/ValidateM7AdvancedWorkPackage.cs";
const string Qualified="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Status="m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished";
const string Marker="dw.tools.math/m7-w01-advanced-research-closure/1.2 qualified; phases=5; tasks=10; qod=30; families=4; product=false; m7=open";
const string NumericMarker="M7-ADVANCED-PORTFOLIO-ORACLE:PRECISION=1/0;TENSOR=36/6;DAG=13;PLOT=4/2";
const string TriggerPrecision="Named precision or interval consumer, operation-specific numerical error/rounding and provider SHA/version/license/RID with isolated exact source-free validation.";
const string TriggerTensor="Named immutable tensor operation and rank/shape/alias/finite byte/refusal semantics, provider rights and independent source-free consumer.";
const string TriggerDag="Named immutable pure DAG consumer and cycle/alias/cancellation/no-partial/bounded execution policy without importing AURA scheduling.";
const string TriggerPlot="Named finite PlotModel point/segment domain with known-pole topology and total memory/time/partial/cancellation limits, source-free client and renderer owner.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS117",Milestone,target?null:Work,null,null,null);
p.Workflow.RequirePriorSuccess("RS116",1,
    "docs: qualify original M7 C03 research closure",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.115"),JsonValue.Create("0.1.116"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.116":"0.1.115"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(C01T2,"/status",
    JsonValue.Create("m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified"));
p.Require.JsonValueEquals(C01T2,"/disposition",
    JsonValue.Create("defer-new-precision-interval-provider"));
p.Require.JsonValueEquals(C01T2,"/m7_w01_c01_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(C01T2,"/external_provider_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(C01T2,"/public_precision_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(C02T2,"/status",
    JsonValue.Create("m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified"));
p.Require.JsonValueEquals(C02T2,"/disposition",
    JsonValue.Create("defer-unqualified-tensor-api-and-providers"));
p.Require.JsonValueEquals(C02T2,"/m7_w01_c02_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(C02T2,"/advanced_tensor_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(S01T2,"/status",
    JsonValue.Create("m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified"));
p.Require.JsonValueEquals(S01T2,"/disposition",
    JsonValue.Create("defer-unqualified-pure-dag-runtime-and-providers"));
p.Require.JsonValueEquals(S01T2,"/s01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(S01T2,"/dag_runtime_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(S02T2,"/status",
    JsonValue.Create("m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified"));
p.Require.JsonValueEquals(S02T2,"/disposition",
    JsonValue.Create("defer-unqualified-plotmodel-and-rendering-provider"));
p.Require.JsonValueEquals(S02T2,"/s02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(S02T2,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(S02T2,"/plot_renderer_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(C03T2,"/status",
    JsonValue.Create("m7-c03-t2-original-aggregate-owner-gated-research-closure-qualified"));
p.Require.JsonValueEquals(C03T2,"/disposition",
    JsonValue.Create("research-complete-products-deferred"));
p.Require.JsonValueEquals(C03T2,"/parent_c03_done",JsonValue.Create(true));
p.Require.JsonValueEquals(C03T2,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(C03T2,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(C03T2,"/public_dag_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(C03T2,"/public_plot_model_api_delivered",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");

foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status",target?"done":"in_progress"),
    ("/work_packages/16/id",NextWork),
    ("/work_packages/16/status","planned"),
    ("/chunks/51/id","M7-W01-C01"),
    ("/chunks/51/status","done"),
    ("/chunks/52/id","M7-W01-C02"),
    ("/chunks/52/status","done"),
    ("/chunks/53/id","M7-W01-C03"),
    ("/chunks/53/status","done"),
    ("/chunks/54/id","M7-W01-C03-S01"),
    ("/chunks/54/status","done"),
    ("/chunks/55/id","M7-W01-C03-S02"),
    ("/chunks/55/status","done"),
    ("/chunks/56/id","M7-W02-C01"),
    ("/chunks/56/status","planned"),
    ("/chunks/57/id","M7-W02-C02"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState(NextWork,"not-ready");
foreach(var id in new[]{"M7-W01-C01","M7-W01-C02","M7-W01-C03",
    "M7-W01-C03-S01","M7-W01-C03-S02"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{"M7-W02-C01","M7-W02-C02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var path in new[]{Decision,Distribution,Checker,Qualified})
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/research_phases_done",JsonValue.Create(5));
    p.Require.JsonValueEquals(Qualified,"/research_tasks_done",JsonValue.Create(10));
    p.Require.JsonValueEquals(Qualified,"/research_subtasks_done",JsonValue.Create(30));
    p.Require.JsonValueEquals(Qualified,"/source_pinned_receipts",JsonValue.Create(10));
    p.Require.JsonValueEquals(Qualified,"/native_nodes_retained",JsonValue.Create(541));
    p.Require.JsonValueEquals(Qualified,"/review_family_count",JsonValue.Create(4));
    p.Require.JsonValueEquals(Qualified,"/precision_provider_deferred",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/tensor_provider_deferred",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/dag_runtime_deferred",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/plot_model_deferred",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/new_advanced_product_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/aura_job_engine_copied",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var checkerWrite=p.Files.ReplaceFromStaged("staged/"+Checker,Checker);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged M7 advanced research decision SHA-256.");
var matrixSha=matrixWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged M7 portfolio negative matrix SHA-256.");
var checkerSha=checkerWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing independent read-only W01 C# source SHA-256.");
foreach(var term in new[]{"## Question","## Verified observations","## Decision",
    "Four mandatory future","10 tasks","30 Q/O/D",
    "No public advanced Math product","M7-W02","541 nodes"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Family-level evidence","## Future review triggers",
    "## Completed versus pending","No provider is automatically admitted",
    "source-free","541"})
    p.Require.TextContains(Distribution,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Checker,Marker);
p.Require.TextContains(Checker,NumericMarker);

var baseline=Required(root,"dotnet","run","--file",Checker,"--","--check");
foreach(var marker in new[]{NumericMarker,Marker})
    if(!baseline.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent M7 W01 advanced research marker absent: "+marker);

if(!target)
{
    var allReceipts=new[]{C01T1,C01T2,C02T1,C02T2,S01T1,S01T2,
        S02T1,S02T2,C03T1,C03T2};
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-work-package-qualified",
        ["status"]=Status,
        ["question"]="Are all accepted M7-W01 advanced-model research phases complete with source-pinned scientific evidence and explicit non-delivery of providers and APIs?",
        ["observation"]="All five accepted research phases, ten Q/O/D tasks and thirty subtasks are complete; precision, 2x2x2 tensor, diamond DAG and reciprocal point/segment BCL witnesses agree with ten prior receipts; each advanced public product request remains deferred.",
        ["decision"]="Close M7-W01 as a locally qualified research portfolio with four owner-gated deferred product families; leave M7-W02, milestone M7 and any public advanced Math API/release open.",
        ["source_pinned_receipts"]=10,
        ["receipt_paths"]=p.Json.StringArray(allReceipts),
        ["research_phases_done"]=5,
        ["research_tasks_done"]=10,
        ["research_subtasks_done"]=30,
        ["native_nodes_retained"]=541,
        ["research_only"]=true,
        ["review_family_count"]=4,
        ["family_review_triggers"]=p.Json.StringArray(
            TriggerPrecision,TriggerTensor,TriggerDag,TriggerPlot),
        ["precision_provider_deferred"]=true,
        ["tensor_provider_deferred"]=true,
        ["dag_runtime_deferred"]=true,
        ["plot_model_deferred"]=true,
        ["precision_exact_cancellation"]="1",
        ["binary64_cancellation"]="0",
        ["tensor_sum"]=36,
        ["tensor_index_value"]=6,
        ["dag_root_value"]=13,
        ["plot_finite_points"]=4,
        ["plot_segments"]=2,
        ["public_precision_api_delivered"]=false,
        ["advanced_tensor_api_delivered"]=false,
        ["public_dag_api_delivered"]=false,
        ["public_plot_model_api_delivered"]=false,
        ["aura_job_engine_copied"]=false,
        ["new_advanced_product_delivered"]=false,
        ["new_provider_installed"]=false,
        ["qualified_external_provider_count"]=0,
        ["new_source_free_consumer_qualified"]=false,
        ["new_package_built"]=false,
        ["publicly_published"]=false,
        ["m7_w01_work_package_done"]=true,
        ["m7_w02_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=matrixSha,
        ["validator"]=Checker,
        ["validator_sha256"]=checkerSha,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W02-C01-T1 formal interoperability and proof research: establish named source/rights/TFM toolchain and independently check a finite trusted-base proof oracle; do not claim a formal engine.",
        ["nonclaims"]=p.Json.StringArray(
            "A completed advanced research portfolio is not a new arbitrary-precision, tensor, DAG or PlotModel product package.",
            "No general interval arithmetic, tensor broadcasting, AURA scheduler, graph renderer, GPU/native provider, certified physical uncertainty or numeric error bound was introduced.",
            "Research-only bounds and 128 coordinate bytes do not qualify total resource/cancellation SLAs or source-free clients.",
            "M7-W02 proofs/review and M7 1.2.0-candidate milestone/release gate remain unfinished; no public NuGet or sibling changes.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.115")
            throw new InvalidOperationException("Unexpected M7 W01 portfolio baseline plan version.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var next=doc["work_packages"]!.AsArray()[16]!.AsObject();
        if((string?)release["id"]!=Milestone ||
           (string?)release["status"]!="in_progress" ||
           (string?)work["id"]!=Work ||
           (string?)work["status"]!="in_progress" ||
           work["evidence"] is not null ||
           (string?)next["id"]!=NextWork ||
           (string?)next["status"]!="planned")
            throw new InvalidOperationException("M7 W01 advanced research baseline or downstream gate changed.");
        var accepted=new[]{"M7-W01-C01","M7-W01-C02","M7-W01-C03",
            "M7-W01-C03-S01","M7-W01-C03-S02"};
        var chunks=doc["chunks"]!.AsArray();
        foreach(var id in accepted)
        {
            var chunk=chunks.Single(x=>string.Equals((string?)x!["id"],id,
                StringComparison.Ordinal))!.AsObject();
            if((string?)chunk["status"]!="done" ||
                chunk["tasks"]!.AsArray().Any(x=>(string?)x!["status"]!="done"))
                throw new InvalidOperationException("Research-only work package has unqualified child "+id);
        }
        doc["plan_version"]="0.1.116";
        work["status"]="done";
        work["evidence"]=p.Json.StringArray(C01T1,C01T2,C02T1,C02T2,
            S01T1,S01T2,S02T1,S02T2,C03T1,C03T2,Qualified);
    });
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState(NextWork,"not-ready");
foreach(var id in new[]{"M7-W01-C01","M7-W01-C02","M7-W01-C03",
    "M7-W01-C03-S01","M7-W01-C03-S02"})
    p.ProjectPlan.RequireNodeState(id,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS117" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=(target?null:Work) ||
        state["activePhase"] is not null ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 W01 owner-gated research closure cursor.");
    state["activeWorkPackage"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var final=Required(root,"dotnet","run","--file",Checker,"--","--check");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Completed M7 W01 research portfolio failed independent target gate.");
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
        throw new InvalidOperationException("M7 W01 independent C# research check could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("M7 W01 research check timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS117 M7 advanced research qualification failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2500)]);
    return output;
}
