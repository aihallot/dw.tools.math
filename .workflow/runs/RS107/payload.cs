using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Phase="M7-W01-C01", Task="M7-W01-C01-T2";
const string Question="M7-W01-C01-T2-A", Observation="M7-W01-C01-T2-B", DecisionNode="M7-W01-C01-T2-C";
const string State=".aura/workflow/state.json", Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string PriorOracle="docs/planning/ValidateM7PrecisionAdmission.cs";
const string Decision="docs/planning/decisions/m7-w01-c01-t2.md";
const string Distribution="docs/distribution/m7-precision-interval-deferral.md";
const string Validator="docs/planning/ValidateM7PrecisionBoundary.cs";
const string Qualified="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string Status="m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified";
const string Marker="dw.tools.math/m7-w01-c01-t2-deferred-provider-gate/1.2 qualified; triggers=3; provider=none; public-api=false";
const string PriorMarker="dw.tools.math/m7-w01-c01-t1-independent-precision-screen/1.2 qualified; cancellation=1/0; tenth=enclosed; external-provider=false";
const string Trigger1="A named consumer and bounded Math operation demonstrate a gap against existing exact/binary64 contracts.";
const string Trigger2="An exact version/source/license/RID/notices/provider-cost and offline rollback record is independently qualified.";
const string Trigger3="Adversarial exact inclusion, rounding/refusal/budget tests and a source-free package consumer pass for each proposed operation.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS107",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS106",1,
    "docs: qualify M7 precision and interval research oracles",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.105"),JsonValue.Create("0.1.106"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.106":"0.1.105"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),("/releases/6/status","done"),
    ("/releases/7/id",Milestone),("/releases/7/status","in_progress"),
    ("/work_packages/15/id",Work),("/work_packages/15/status","in_progress"),
    ("/work_packages/16/status","planned"),
    ("/chunks/50/status","done"),("/chunks/51/id",Phase),
    ("/chunks/51/kind","spike"),
    ("/chunks/51/status",target?"done":"in_progress"),
    ("/chunks/51/tasks/0/id","M7-W01-C01-T1"),
    ("/chunks/51/tasks/0/status","done"),
    ("/chunks/51/tasks/1/id",Task),
    ("/chunks/51/tasks/1/status",target?"done":"planned"),
    ("/chunks/52/status","planned"),
    ("/chunks/53/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var index=0;index<3;index++)
{
    p.Require.JsonValueEquals(Backlog,"/chunks/51/tasks/0/subtasks/"+index+"/status",
        JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,"/chunks/51/tasks/1/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01-C01-T1","done");
foreach(var id in new[]{"M7-W01-C01-T1-A","M7-W01-C01-T1-B","M7-W01-C01-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C02","M7-W01-C03","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-c01-t1-bcl-exact-precision-interval-oracles-provider-screen-qualified"));
p.Require.JsonValueEquals(Prior,"/exact_integer_result",JsonValue.Create("1"));
p.Require.JsonValueEquals(Prior,"/observed_binary64_cancellation",JsonValue.Create("0"));
p.Require.JsonValueEquals(Prior,"/reference_tenth_enclosed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/independent_oracle_cases",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/precision_reference_oracles_run",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/one_constant_enclosure_is_general_interval_certification",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_provider_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_precision_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w01_c01_parent_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.Require.TextContains(PriorOracle,
    "M7-PRECISION-ORACLE:DECIMAL-TENTH:ADJACENT-BINARY64-ENCLOSURE=TRUE");

if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/disposition",
        JsonValue.Create("defer-new-precision-interval-provider"));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/external_provider_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_precision_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/automatic_cutover",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_c01_parent_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
}
var decisionResult=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var distributionResult=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var validatorResult=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionResult.AfterSha256 ??
    throw new InvalidOperationException("Missing staged T2 decision source SHA-256.");
var distributionSha=distributionResult.AfterSha256 ??
    throw new InvalidOperationException("Missing staged T2 distribution source SHA-256.");
var validatorSha=validatorResult.AfterSha256 ??
    throw new InvalidOperationException("Missing T2 BCL validator SHA-256.");
foreach(var marker in new[]{"## Question","## Observation","## Decision",
    "All three triggers are necessary","DEFER","No product API",
    "M7-W01-C02"})
    p.Require.TextContains(Decision,marker,StringComparison.OrdinalIgnoreCase);
foreach(var marker in new[]{"## Not-admitted candidate matrix","## Three review triggers",
    "If any trigger is missing: **remain deferred**","M7-W01-C02"})
    p.Require.TextContains(Distribution,marker,StringComparison.Ordinal);
p.Require.TextContains(Validator,Marker);
if(!target)
{
    var replay=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!replay.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Historical independent T1 exact numeric oracles were not reverified.");
}
var baseline=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!baseline.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent T2 source/deferral gate did not pass.");
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,["id"]="M7-W01-C01-T2-qualified",
        ["status"]=Status,
        ["question"]="Do Math consumers and verifiable source/rounding/rights evidence justify an external arbitrary-precision or interval provider now?",
        ["observation"]="T1 independent exact-oracle evidence remains pinned; no named product gap, verified external provider/license/RID or admitted per-operation interval corpus and consumer is available.",
        ["decision"]="Explicitly defer all new high-precision and interval providers and public APIs until all three bounded review triggers are satisfied in a future owner-approved run.",
        ["prior_precision_evidence"]=Prior,
        ["prior_oracle_validator"]=PriorOracle,
        ["prior_oracles_reexecuted_before_boundary_transition"]=true,
        ["prior_reference_oracle_cases"]=2,
        ["disposition"]="defer-new-precision-interval-provider",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["provider_categories_screened"]=5,
        ["new_provider_categories_admitted"]=0,
        ["external_provider_adopted"]=false,
        ["external_library_installed"]=false,
        ["provider_license_verified"]=false,
        ["provider_rid_qualified"]=false,
        ["provider_source_free_consumer_qualified"]=false,
        ["certified_interval_arithmetic_delivered"]=false,
        ["physical_measurement_uncertainty_certified"]=false,
        ["public_precision_api_delivered"]=false,
        ["automatic_cutover"]=false,
        ["publicly_published"]=false,
        ["new_math_code_or_packages_built"]=false,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_document_sha256"]=distributionSha,
        ["boundary_validator"]=Validator,
        ["boundary_validator_sha256"]=validatorSha,
        ["t1_done"]=true,["t2_done"]=true,
        ["m7_w01_c01_parent_done"]=true,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C02-T1: research tensor shape, 2x2x2 independent oracle, incompatible dimensions and size budgets without inventing an advanced package.",
        ["nonclaims"]=p.Json.StringArray(
            "A completed research spike does not deliver a high-precision real or certified interval arithmetic API.",
            "No provider is selected, installed, version/right/RID-qualified or integrated; the three review triggers are not owner approval.",
            "Two rational reference oracles are not a general error theorem, statistical confidence or physical uncertainty model.",
            "M7-W01 and M7 remain active while later advanced research scopes are separately admitted.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.105")
            throw new InvalidOperationException("M7 C01 T2 baseline version changed.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[51]!.AsObject();
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
            throw new InvalidOperationException("M7 precision provider disposition baseline drifted.");
        doc["plan_version"]="0.1.106";
        chunk["status"]="done";
        chunk["refinement"]="T1 retained two BCL-only independent integer/rational reference oracles and a provider screening matrix. RS107 T2 explicitly defers new high-precision/interval provider and API until all three review triggers (consumer need, source/license/RID/cost evidence, per-operation rounding and independent consumer tests) are independently qualified. No provider cutover or new product capability.";
        chunk["readiness"]="M7-W01-C01 research spike complete by a documented provider-deferral decision with three review triggers, not by delivery of an arbitrary-precision or interval engine. Existing Math exact and binary64 contracts remain admitted; M7-W01-C02 remains planned.";
        var files=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(files.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7PrecisionBoundary.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        t2["status"]="done";
        t2["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t2["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("M7 C01 T2 Q/O/D child was not planned.");
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
p.ProjectPlan.RequireNodeState("M7-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M7-W01-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS107" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?null:Phase) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS107 deferred scientific spike workflow cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!finalGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Qualified T2 target failed independent source and boundary gate.");
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
        throw new InvalidOperationException("RS107 independent boundary validation process did not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS107 independent boundary validation timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS107 bounded documentary validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2200)]);
    return output;
}
