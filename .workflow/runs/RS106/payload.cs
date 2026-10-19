using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7", Work="M7-W01", Phase="M7-W01-C01", Task="M7-W01-C01-T1";
const string Question="M7-W01-C01-T1-A", Observation="M7-W01-C01-T1-B", DecisionNode="M7-W01-C01-T1-C";
const string State=".aura/workflow/state.json", Backlog="docs/planning/backlog.json";
const string M6Gate="docs/planning/evidence/M6-gate.json";
const string Decision="docs/planning/decisions/m7-w01-c01.md";
const string Screen="docs/planning/decisions/m7-w01-c01-provider-screen.md";
const string Oracle="docs/planning/ValidateM7PrecisionAdmission.cs";
const string Qualified="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string Status="m7-c01-t1-bcl-exact-precision-interval-oracles-provider-screen-qualified";
const string Marker="dw.tools.math/m7-w01-c01-t1-independent-precision-screen/1.2 qualified; cancellation=1/0; tenth=enclosed; external-provider=false";
const string CancellationMarker="M7-PRECISION-ORACLE:CANCELLATION:EXACT=1:BINARY64=0:ULP=2";
const string TenthMarker="M7-PRECISION-ORACLE:DECIMAL-TENTH:ADJACENT-BINARY64-ENCLOSURE=TRUE";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS106",
    target?Milestone:null,target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS105",1,
    "docs: qualify local M6 scientific release gate",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(M6Gate,"/status",
    JsonValue.Create("m6-independent-local-analysis-scientific-release-gate-qualified-unpublished"));
p.Require.JsonValueEquals(M6Gate,"/m6_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(M6Gate,"/qualified_phases",JsonValue.Create(11));
p.Require.JsonValueEquals(M6Gate,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(M6Gate,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(M6Gate,"/external_cutover_performed",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.104"),JsonValue.Create("0.1.105"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.105":"0.1.104"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/version","1.2.0-candidate"),
    ("/releases/7/gate","G-M7"),
    ("/releases/7/status",target?"in_progress":"planned"),
    ("/work_packages/15/id",Work),
    ("/work_packages/15/status",target?"in_progress":"planned"),
    ("/work_packages/16/status","planned"),
    ("/chunks/50/status","done"),
    ("/chunks/51/id",Phase),
    ("/chunks/51/kind","spike"),
    ("/chunks/51/status",target?"in_progress":"planned"),
    ("/chunks/51/tasks/0/id",Task),
    ("/chunks/51/tasks/0/status",target?"done":"planned"),
    ("/chunks/51/tasks/1/status","planned"),
    ("/chunks/52/status","planned"),
    ("/chunks/53/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var index in new[]{0,1,2})
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/51/tasks/0/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/51/tasks/1/subtasks/"+index+"/status",JsonValue.Create("planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
foreach(var id in new[]{Milestone,Work,Phase})
    p.ProjectPlan.RequireNodeState(id,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W01-C01-T2","M7-W01-C01-T2-A","M7-W01-C01-T2-B",
    "M7-W01-C01-T2-C","M7-W01-C02","M7-W01-C03","M7-W02"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.Require.FileExists("src/projects/dw.quantities/ExactRational.cs");
p.Require.FileExists("src/projects/dw.quantities/ExactBinaryNumber.cs");

if(!target)
{
    foreach(var file in new[]{Decision,Screen,Oracle,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/precision_reference_oracles_run",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/exact_integer_result",
        JsonValue.Create("1"));
    p.Require.JsonValueEquals(Qualified,"/observed_binary64_cancellation",
        JsonValue.Create("0"));
    p.Require.JsonValueEquals(Qualified,"/reference_tenth_enclosed",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_provider_adopted",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_precision_api_delivered",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",
        JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+Screen,Screen);
var oracleWrite=p.Files.ReplaceFromStaged("staged/"+Oracle,Oracle);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged precision decision SHA-256.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged provider evaluation SHA-256.");
var oracleSha=oracleWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing staged independent oracle SHA-256.");
foreach(var value in new[]{"## Question","## Observation","## Decision",
    "1/10","BigInteger","M7-W01-C01-T2","defer","no product capability"})
    p.Require.TextContains(Decision,value,StringComparison.OrdinalIgnoreCase);
foreach(var value in new[]{"## Candidate strategy screen","BigInteger",
    "decimal","native","not admitted","source-free consumer"})
    p.Require.TextContains(Screen,value,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Oracle,"M7-PRECISION-ORACLE:CANCELLATION:EXACT=1:BINARY64=0:ULP=2");
p.Require.TextContains(Oracle,"M7-PRECISION-ORACLE:DECIMAL-TENTH:ADJACENT-BINARY64-ENCLOSURE=TRUE");

var observed=Required(root,"dotnet","run","--file",Oracle,"--","--check");
foreach(var marker in new[]{CancellationMarker,TenthMarker,Marker})
    if(!observed.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent precision oracle marker missing: "+marker);

if(!target)
{
    var record=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W01-C01-T1-qualified",
        ["status"]=Status,
        ["question"]="Do finite high-precision and interval needs exceed existing exact rationals and binary64, and what evidence would justify a provider?",
        ["observation"]="A BCL-only independent integer reference gives 1 versus binary64 cancellation 0 at 1e16; exact binary decoding proves adjacent IEEE 754 doubles enclose 1/10. No external provider was inspected.",
        ["decision"]="Admit reproducible research oracles and candidate criteria only. Defer any new high-precision or interval product API/provider until separate T2 qualification and owner admission.",
        ["prior_m6_gate"]=M6Gate,
        ["oracle_validator"]=Oracle,
        ["oracle_validator_sha256"]=oracleSha,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["provider_screen"]=Screen,
        ["provider_screen_sha256"]=screenSha,
        ["exact_integer_result"]="1",
        ["observed_binary64_cancellation"]="0",
        ["binary64_ulp_at_1e16"]="2",
        ["reference_tenth_enclosed"]=true,
        ["reference_upper_exact_numerator"]="3602879701896397",
        ["reference_upper_exact_denominator"]="36028797018963968",
        ["precision_reference_oracles_run"]=true,
        ["independent_oracle_cases"]=2,
        ["existing_exact_rational_retained"]=true,
        ["calculation_precision_is_measurement_error"]=false,
        ["one_constant_enclosure_is_general_interval_certification"]=false,
        ["unverified_external_provider_count_adopted"]=0,
        ["external_provider_adopted"]=false,
        ["external_library_installed"]=false,
        ["public_precision_api_delivered"]=false,
        ["scientific_tests_rerun"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["m7_w01_c01_parent_done"]=false,
        ["m7_w01_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W01-C01-T2: qualify finite operation limits, provider/license/version/rights/RID and rounding semantics or issue a documented deferral with trigger; no product API without independent acceptance.",
        ["nonclaims"]=p.Json.StringArray(
            "No public high-precision, interval or physical uncertainty API was implemented or packed.",
            "A single correctly enclosing rational witness is not general directed-rounding interval certification.",
            "Third-party managed and native provider categories have no pinned package, license, platform or source-free client qualification.",
            "M7-W01-C01-T2, parent C01, M7-W01 and M7 remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        record.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.104")
            throw new InvalidOperationException("M7 precision baseline plan version changed.");
        var milestone=doc["releases"]!.AsArray()[7]!.AsObject();
        var wp=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[51]!.AsObject();
        var task=chunk["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)milestone["id"]!=Milestone ||
            (string?)milestone["status"]!="planned" ||
            (string?)wp["id"]!=Work || (string?)wp["status"]!="planned" ||
            (string?)chunk["id"]!=Phase ||
            (string?)chunk["kind"]!="spike" ||
            (string?)chunk["status"]!="planned" ||
            (string?)task["id"]!=Task ||
            (string?)task["status"]!="planned" ||
            (string?)chunk["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("M7 C01 T1 documentary baseline drifted.");
        doc["plan_version"]="0.1.105";
        milestone["status"]="in_progress";
        wp["status"]="in_progress";
        chunk["status"]="in_progress";
        chunk["refinement"]="RS106 T1 qualifies a BCL-only exact cancellation and adjacent-double rational enclosure oracle, documents existing exact-rational reuse and conditional provider costs. This spike delivers no precision/interval API, certifying interval engine or public package. T2 stays planned for admission/deferral, version, license and rounding gates.";
        chunk["readiness"]="T1 Q/O/D research qualified with two independent finite oracles, explicit noncertification, and a bounded candidate-provider screen. T2 is pending; no external numeric provider or precise rounding contract has been selected.";
        var existingFiles=chunk["files"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(existingFiles.Concat(new[]{
            Decision,Screen,Oracle,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var existingEvidence=chunk["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(existingEvidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7PrecisionAdmission.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("T1 Q/O/D subtask baseline changed.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var id in new[]{Milestone,Work,Phase,Task,Question})
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
foreach(var id in new[]{Milestone,Work,Phase})
    p.ProjectPlan.RequireNodeState(id,"in-progress");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M7-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M7-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6","done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS106" ||
        (string?)state["activeMilestone"]!=(target?Milestone:null) ||
        (string?)state["activeWorkPackage"]!=(target?Work:null) ||
        (string?)state["activePhase"]!=(target?Phase:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS106 M7 Q/O/D cursor.");
    state["activeMilestone"]=Milestone;
    state["activeWorkPackage"]=Work;
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalOracle=Required(root,"dotnet","run","--file",Oracle,"--","--check");
if(!finalOracle.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("M7 qualified precision oracle target marker absent.");
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
        throw new InvalidOperationException("RS106 independent research validator did not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS106 independent research validator timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS106 local research validation failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
