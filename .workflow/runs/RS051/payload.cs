using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Work="M3-W02", Phase="M3-W02-C04", Task="M3-W02-C04-T2";
const string Question="M3-W02-C04-T2-A", Observation="M3-W02-C04-T2-B",
    DecisionNode="M3-W02-C04-T2-C";
const string Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M3-W02-C04-consumer-qualified.json";
const string Qualified="docs/planning/evidence/M3-W02-C04-boundaries-qualified.json";
const string DecisionRequest="docs/coordination/requests/MATH-XR-002-decision.md";
const string McdmRequest="docs/coordination/requests/MATH-XR-003-mcdm.md";
const string Matrix="docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json";
const string Decision="docs/planning/decisions/m3-w02-c04-t2-boundary-decisions.md";
const string Handoff="docs/distribution/m3-w02-consumer-handoff.md";
const string Validator="docs/planning/ValidateC04ConsumerBoundaries.cs";
const string ExpectedMarker="dw.tools.math/c04-boundaries/0.4 qualified; rows=8; candidate=4; hold=1; external=3; transmitted=0";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

// Completing the final phase must retire the work-package cursor, while M3
// remains open for the independently accepted release gate.
p.Workflow.RequireCurrentRun("RS051","M3",target?null:Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS050",1,
    "feat: qualify independent Math package consumer and primitive contracts",
    "0.1.59",2,2,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("isolated-math-packages-and-primitive-proposals-qualified"));
p.Require.JsonValueEquals(Prior,"/consumer_oracles_passed",JsonValue.Create(31));
p.Require.JsonValueEquals(Prior,"/package_only_consumer",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/external_adoption_observed",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/package_quantities_version",
    JsonValue.Create("0.2.0-preview.1"));
p.Require.JsonValueEquals(Prior,"/package_statistics_version",
    JsonValue.Create("0.4.0-preview.6"));
p.Require.JsonValueEquals(Prior,"/package_quantities_sha256",
    JsonValue.Create("f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5"));
p.Require.JsonValueEquals(Prior,"/package_statistics_sha256",
    JsonValue.Create("b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.49"),JsonValue.Create("0.1.50"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.50":"0.1.49"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/7/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",
    JsonValue.Create(target?"done":"in_progress"));
foreach(var index in new[]{23,24,25})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/26/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
foreach(var phase in new[]{"M3-W02-C01","M3-W02-C02","M3-W02-C03"})
    p.ProjectPlan.RequireNodeState(phase,"done");
p.ProjectPlan.RequireNodeState("M3-W01-C03","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C04-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var child in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

// Both original requests remain genuine, unsent drafts in baseline and target.
p.Require.TextContains(DecisionRequest,
    "Status: draft. Recipient: Decision agent. Not sent.");
p.Require.TextContains(McdmRequest,
    "Status: draft. Recipient: MCDM agent. Not sent.");
if(!target)
{
    foreach(var file in new[]{Matrix,Decision,Handoff,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("recipient-boundary-ownership-and-unsent-requests-qualified"));
    p.Require.JsonValueEquals(Qualified,"/external_adoption_observed",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/requests_transmitted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/work_package_closed",JsonValue.Create(true));
}

// Stage full Math-owned decision inputs; neither external owner is mutated,
// nor is an outgoing request transmitted.
var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{DecisionRequest,McdmRequest,Matrix,Decision,Handoff,Validator})
{
    var result=p.Files.ReplaceFromStaged("staged/"+file,file);
    digests[file]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing Payload SDK SHA-256 for staged file "+file);
}

// Independent read-only C# validation of the observed T1 receipt, two local
// package identities and both unsent external-owner proposals.
var observed=Required(root,"dotnet","run","--file",Validator);
if(!observed.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
    .Any(line=>line.Trim()==ExpectedMarker))
    throw new InvalidOperationException("RS051 did not observe the exact bounded owner-boundary C# validator marker: "+
        Relevant(observed));

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M3-W02-C04-boundaries-qualified",
        ["status"]="recipient-boundary-ownership-and-unsent-requests-qualified",
        ["prior_isolated_consumer_evidence"]=Prior,
        ["question"]="Which generic Math primitives can be proposed for owner evaluation without transferring Decision/MCDM business policies or falsely claiming adoption?",
        ["observation"]="Math-side C# validator accepted 8 independent owner-boundary rows (4 candidate, 1 hold, 3 protected external), 2 unsent drafts and pinned local RS050 package evidence.",
        ["decision"]="Retain owner-exclusive business and broker semantics; transmit nothing automatically; require independent owner approval, package transfer, semantic tests and rollback before adoption.",
        ["source_math_tip"]="9bbbdde4f0c8a2df30f585f5b1a20e5f6d514cc3",
        ["matrix"]=Matrix,["matrix_sha256"]=digests[Matrix],
        ["decision_request"]=DecisionRequest,
        ["decision_request_sha256"]=digests[DecisionRequest],
        ["mcdm_request"]=McdmRequest,["mcdm_request_sha256"]=digests[McdmRequest],
        ["decision_document"]=Decision,["decision_document_sha256"]=digests[Decision],
        ["consumer_handoff"]=Handoff,["consumer_handoff_sha256"]=digests[Handoff],
        ["validator"]=Validator,["validator_sha256"]=digests[Validator],
        ["validator_marker"]=ExpectedMarker,
        ["owner_boundary_rows"]=8,
        ["candidate_primitive_reviews"]=4,
        ["held_primitive_reviews"]=1,
        ["protected_external_scopes"]=3,
        ["recipient_revisions_verified"]=0,
        ["recipient_tests_executed"]=0,
        ["external_adoption_observed"]=false,
        ["requests_transmitted"]=false,
        ["packages_publicly_published"]=false,
        ["prior_math_consumer_oracles_passed"]=31,
        ["math_quantities_version"]="0.2.0-preview.1",
        ["math_quantities_local_artifact_sha256"]="f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5",
        ["math_statistics_version"]="0.4.0-preview.6",
        ["math_statistics_local_artifact_sha256"]="b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6",
        ["prng_is_noncryptographic"]=true,
        ["no_business_policy_changes"]=true,
        ["work_package_closed"]=true,
        ["milestone_release_gate_pending"]="G-M3",
        ["selected_dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["selected_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["nonclaims"]=p.Json.StringArray(
            "Recipient Decision/MCDM code, current revisions, APIs and business tests are not verified or modified.",
            "Draft requests are not transmitted and no external acceptance or migrated consumer exists.",
            "Observed RS050 local nupkg SHA-256s do not make the artifacts downloadable or prove rebuild identity.",
            "M3 numerical release gate G-M3 is still pending; no release, neutral dw.discovery schema, generated agent skill or AURA BrokerReady admission is claimed.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.50";
        var wp=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Work);
        var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var t=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        // All four required statistics phases now have durable evidence.
        wp["status"]="done";
        wp["evidence"]=p.Json.StringArray(
            "docs/planning/evidence/M3-W02-C01-contract-qualified.json",
            "docs/planning/evidence/M3-W02-C01-boundaries-qualified.json",
            "docs/planning/evidence/M3-W02-C02-contract-qualified.json",
            "docs/planning/evidence/M3-W02-C02-boundaries-qualified.json",
            "docs/planning/evidence/M3-W02-C03-contract-qualified.json",
            "docs/planning/evidence/M3-W02-C03-boundaries-qualified.json",
            Prior,Qualified);
        chunk["status"]="done";
        chunk["refinement"]="RS050 independently qualified a two-package Math-only consumer with 31 oracles and real local artifact SHA-256s. RS051 T2 now independently validates a Math-owned 8-row owner-boundary matrix (4 candidates, 1 hold, 3 retained external), two explicit Decision/MCDM request drafts still not transmitted, the exact T1 artifact identities, lossless scaling/quantile/tie/risk policy restrictions, and owner-side test/rollback requirements. Both tasks and all four M3-W02 chunks close while the M3 G-M3 release gate remains separately pending. No sibling adoption, public package transfer, dw.discovery or AURA broker implementation is claimed.";
        chunk["readiness"]="closed: C04 T1 independent consumer and T2 owner-boundary decision qualified; M3-W02 complete; G-M3 release gate pending";
        var oldFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(oldFiles.Concat(new[]{
            Matrix,DecisionRequest,McdmRequest,Decision,Handoff,Validator,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file "+Validator,
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs");
        var priorProofs=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(priorProofs.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        t["status"]="done";
        t["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });

    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Question,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Question);
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var child in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var item in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(item,"done");

// Retire the completed work-package cursor inside the same run transaction.
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS051" ||
       (string?)state["activeMilestone"]!="M3" ||
       (string?)state["activeWorkPackage"]!=(target?null:Work) ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS051 baseline or target cursor identity diverged.");
    state["activeWorkPackage"]=null;
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2200?part[..2200]:part;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
