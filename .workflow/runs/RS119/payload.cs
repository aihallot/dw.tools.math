using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W02",Phase="M7-W02-C01";
const string Task="M7-W02-C01-T2",Question="M7-W02-C01-T2-A";
const string Observation="M7-W02-C01-T2-B",DecisionNode="M7-W02-C01-T2-C";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string PrevWork="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Fixture="docs/planning/fixtures/m7-polynomial-identity-certificate.json";
const string PriorOracle="docs/planning/ValidateM7FormalCertificate.cs";
const string Decision="docs/planning/decisions/m7-w02-c01-t2.md";
const string Distribution="docs/distribution/m7-formal-toolchain-deferral.md";
const string Validator="docs/planning/ValidateM7FormalBoundary.cs";
const string Qualified="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Status="m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified";
const string Marker="dw.tools.math/m7-w02-c01-t2-formal-provider-deferral/1.2 qualified; fixture=replayed; statuses=verified,invalid,unproved; triggers=3; prover=none; c01=done";
const string Witness="M7-FORMAL-BOUNDARY:POLYNOMIAL=-1,0,1;CERTIFICATE=verified-finite-identity;TRUSTED-BASE=local-bcl-only";
const string Refusal="M7-FORMAL-BOUNDARY:UNPROVED:HOLE=true;TIMEOUT=true;PROVIDER-FAILURE=true;MISSING=true;ALTERED=invalid-certificate";
const string PriorMarker="dw.tools.math/m7-w02-c01-t1-independent-certificate/1.2 qualified; z-polynomial=-1,0,1; altered=invalid; hole=unproved; timeout=unproved; provider=none";
const string Trigger1="A named consuming Math or client use case requires a bounded proof language, explicit axioms and certificate term set beyond the accepted exact algebra operations.";
const string Trigger2="A pinned trusted kernel or sound checker, finite proof and resource budgets, invalid/timeout/hole/unproved statuses and independent falsifying adversarial certificates are qualified.";
const string Trigger3="Exact source/version/license/notices/TFM/RID, offline replay, rollback, independently isolated source-free recipient and explicit Math/client owner approval are verified.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS119",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS118",1,
    "docs: qualify M7 polynomial certificate research",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.117"),JsonValue.Create("0.1.118"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.118":"0.1.117"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified"));
p.Require.JsonValueEquals(Prior,"/certificate_verdict",
    JsonValue.Create("verified-finite-identity"));
p.Require.JsonValueEquals(Prior,"/certificate_kind",
    JsonValue.Create("finite-integer-polynomial-coefficients"));
p.Require.JsonValueEquals(Prior,"/certificate_domain",JsonValue.Create("Z[x]"));
p.Require.JsonValueEquals(Prior,"/reference_degree",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/reference_coefficient_count",JsonValue.Create(3));
p.Require.JsonValueEquals(Prior,"/independent_oracle_groups",JsonValue.Create(2));
p.Require.JsonValueEquals(Prior,"/altered_certificate_rejected",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/truncated_certificate_rejected",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/hole_unproved",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/timeout_unproved",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/provider_failure_unproved",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/c01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_prover_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/general_proof_kernel_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/source_free_consumer_qualified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_formal_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(PrevWork,"/status",
    JsonValue.Create("m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"));
p.Require.JsonValueEquals(PrevWork,"/m7_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PrevWork,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PrevWork,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id","M7-W01"),
    ("/work_packages/15/status","done"),
    ("/work_packages/16/id",Work),
    ("/work_packages/16/status","in_progress"),
    ("/chunks/53/status","done"),
    ("/chunks/54/status","done"),
    ("/chunks/55/status","done"),
    ("/chunks/56/id",Phase),
    ("/chunks/56/kind","spike"),
    ("/chunks/56/size","M"),
    ("/chunks/56/status",target?"done":"in_progress"),
    ("/chunks/56/tasks/0/id","M7-W02-C01-T1"),
    ("/chunks/56/tasks/0/status","done"),
    ("/chunks/56/tasks/1/id",Task),
    ("/chunks/56/tasks/1/status",target?"done":"planned"),
    ("/chunks/57/id","M7-W02-C02"),
    ("/chunks/57/kind","document"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/56/tasks/0/subtasks/"+i+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/56/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C01-T1","done");
foreach(var node in new[]{"M7-W02-C01-T1-A",
    "M7-W02-C01-T1-B","M7-W02-C01-T1-C"})
    p.ProjectPlan.RequireNodeState(node,"done");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
foreach(var node in new[]{"M7-W02-C02","M7-W02-C02-T1",
    "M7-W02-C02-T2"})
    p.ProjectPlan.RequireNodeState(node,"not-ready");

if(!target)
{
    foreach(var file in new[]{Decision,Distribution,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/disposition",
        JsonValue.Create("defer-unqualified-formal-proof-providers-and-public-api"));
    p.Require.JsonValueEquals(Qualified,"/review_trigger_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/deferred_external_provider_categories",
        JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/certificate_verdict",
        JsonValue.Create("verified-finite-identity"));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/c01_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m7_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/lean_kernel_qualified",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/rocq_kernel_qualified",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/smt_certificate_checker_qualified",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_prover_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/general_proof_kernel_certified",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_formal_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/source_free_consumer_qualified",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/automatic_provider_cutover",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+Distribution,Distribution);
var checkerWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged trusted-base T2 decision source SHA absent.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged formal provider deferral source SHA absent.");
var checkerSha=checkerWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged finite formal T2 source verifier SHA absent.");
foreach(var t in new[]{"## Question","## Observation","## Decision","DEFER",
    "verified-finite-identity","invalid-certificate","unproved",
    "Three mandatory","All three review triggers are necessary",
    "Lean","Rocq","SMT","trusted base","source-free","no public","M7-W02-C02"})
    p.Require.TextContains(Decision,t,StringComparison.OrdinalIgnoreCase);
foreach(var t in new[]{"## Proof/certificate/derivation separation",
    "## Candidate toolchain / legality / platform matrix",
    "## Three re-entry triggers","Lean","Rocq","SMT",
    "If any trigger is missing, remain deferred","source-free","AURA"})
    p.Require.TextContains(Distribution,t,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Witness);
p.Require.TextContains(Validator,Refusal);
p.Require.TextContains(Validator,Marker);

if(!target)
{
    var earlierProof=Required(root,"dotnet","run","--file",PriorOracle,"--","--check");
    if(!earlierProof.Contains(PriorMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Prior finite polynomial identity verifier did not requalify.");
}
var beforeGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
foreach(var token in new[]{Witness,Refusal,Marker})
    if(!beforeGate.Contains(token,StringComparison.Ordinal))
        throw new InvalidOperationException("Local formal T2 pre-admission gate missing marker: "+token);

if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W02-C01-T2-qualified",
        ["status"]=Status,
        ["question"]="Does one verified finite polynomial identity justify Lean/Rocq/SMT/kernel adoption or a public Math proof API?",
        ["observation"]="Historical source-pinned T1 proves only one Z[x] coefficient convolution with BCL BigInteger; modified/incomplete certificates are invalid and missing/hole/timeout/provider errors remain unproved, without external trusted kernel/version/license/rights or source-free consumer.",
        ["decision"]="Explicitly defer Lean, Rocq, SMT proof-provider and public formal API adoption until an independently qualified consuming domain, trusted-base resource/error contract and immutable provider/rights/recipient authorization are all met.",
        ["prior_certificate_evidence"]=Prior,
        ["prior_advanced_research_gate"]=PrevWork,
        ["prior_certificate_fixture"]=Fixture,
        ["prior_t1_oracle"]=PriorOracle,
        ["prior_t1_oracles_reexecuted_before_transition"]=true,
        ["certificate_kind"]="finite-integer-polynomial-coefficients",
        ["certificate_domain"]="Z[x]",
        ["certificate_coefficients"]="-1,0,1",
        ["certificate_verdict"]="verified-finite-identity",
        ["trusted_base"]="local C# finite convolution verifier and .NET BCL BigInteger only",
        ["trusted_base_restricted_to_finite_fixture"]=true,
        ["invalid_certificate_refused"]=true,
        ["missing_certificate_unproved"]=true,
        ["hole_unproved"]=true,
        ["timeout_unproved"]=true,
        ["provider_failure_unproved"]=true,
        ["sample_values_are_not_proof"]=true,
        ["derivation_is_not_kernel_certificate"]=true,
        ["disposition"]="defer-unqualified-formal-proof-providers-and-public-api",
        ["review_trigger_count"]=3,
        ["review_triggers"]=p.Json.StringArray(Trigger1,Trigger2,Trigger3),
        ["deferred_external_provider_categories"]=3,
        ["lean_kernel_qualified"]=false,
        ["rocq_kernel_qualified"]=false,
        ["smt_certificate_checker_qualified"]=false,
        ["external_prover_adopted"]=false,
        ["general_proof_kernel_certified"]=false,
        ["source_free_consumer_qualified"]=false,
        ["public_formal_api_delivered"]=false,
        ["new_formal_package_built"]=false,
        ["aura_scheduler_copied"]=false,
        ["automatic_provider_cutover"]=false,
        ["publicly_published"]=false,
        ["t1_done"]=true,
        ["t2_done"]=true,
        ["c01_phase_done"]=true,
        ["m7_w02_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=screenSha,
        ["boundary_validator"]=Validator,
        ["boundary_validator_sha256"]=checkerSha,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W02-C02-T1: independently audit the accepted Math-family coverage/ownership statuses and unfinished external requests; a spike is not a shipped feature and the release gate remains open.",
        ["nonclaims"]=p.Json.StringArray(
            "T2 closes a formal interoperability research spike by deferral, not by delivering a theorem prover or proof kernel.",
            "The local finite certificate is not a kernel-proofed Lean or Rocq term or an SMT general certificate.",
            "No source/license/version/TFM/RID-verified provider, isolated consumer, public formal package or NuGet publication was qualified.",
            "W02 coverage review, M7-W02 work-package conclusion and M7 1.2.0-candidate milestone remain pending.")
    };
    p.Files.WriteComplete(Qualified,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.117")
            throw new InvalidOperationException("RS119 baseline formal planning version changed.");
        var m7=doc["releases"]!.AsArray()[7]!.AsObject();
        var priorWork=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[56]!.AsObject();
        var next=doc["chunks"]!.AsArray()[57]!.AsObject();
        var task=phase["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)m7["id"]!=Milestone ||
            (string?)m7["status"]!="in_progress" ||
            (string?)priorWork["id"]!="M7-W01" ||
            (string?)priorWork["status"]!="done" ||
            (string?)work["id"]!=Work ||
            (string?)work["status"]!="in_progress" ||
            (string?)phase["id"]!=Phase ||
            (string?)phase["status"]!="in_progress" ||
            (string?)phase["kind"]!="spike" ||
            (string?)phase["tasks"]!.AsArray()[0]!["status"]!="done" ||
            (string?)task["id"]!=Task ||
            (string?)task["status"]!="planned" ||
            (string?)next["id"]!="M7-W02-C02" ||
            (string?)next["status"]!="planned")
            throw new InvalidOperationException("RS119 formal T2 research parent boundary drifted.");
        doc["plan_version"]="0.1.118";
        phase["status"]="done";
        phase["refinement"]="RS118 T1 independently replayed one fixed Z[x] polynomial certificate with exact BigInteger coefficients [-1,0,1], invalid modified/incomplete evidence and unproved hole/timeout statuses. RS119 T2 pins that evidence and explicitly defers Lean/Rocq/SMT kernels, external proof providers and public APIs until named caller, trusted-base/refusal/resource and source-license-RID/source-free owner gates are independently qualified.";
        phase["readiness"]="C01 finite formal interoperability research complete with source-pinned local verifier/certificate and an explicit provider/API deferral. No installed kernel/prover, general certificate engine, public package or source-free client. M7-W02-C02 coverage review remains separate and planned.";
        var oldFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(oldFiles.Concat(new[]{
            Decision,Distribution,Validator,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var oldEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7FormalBoundary.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)child["status"]!="planned")
                throw new InvalidOperationException("Formal C01-T2 Q/O/D child was not planned.");
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var node in new[]{Task,Question})
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
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M7-W02-C01-T1","done");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M7-W02-C02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS119" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 formal provider deferral cursor.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var afterGate=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!afterGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Completed formal interoperability T2 deferral failed read-only verification.");
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
        throw new InvalidOperationException("RS119 local formal verification command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS119 bounded .NET formal research check timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS119 formal provider/owner gate refused: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2300)]);
    return output;
}
