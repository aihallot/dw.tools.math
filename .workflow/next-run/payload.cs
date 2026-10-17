using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M5",Work="M5-W02",Phase="M5-W02-C03";
const string Task="M5-W02-C03-T2",Question="M5-W02-C03-T2-A";
const string Observation="M5-W02-C03-T2-B",DecisionNode="M5-W02-C03-T2-C";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W02-C03-T1-local-candidate-qualified.json";
const string PreviousPackage="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string PreviousWork="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string SourceRights="docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json";
const string ReleaseMap="docs/planning/releases/m5-v1-publication-boundary.json";
const string LegacyMap="docs/planning/releases/m5-v1-local-candidate-map.json";
const string LegacyValidator="docs/planning/ValidateM5ReleaseCandidate.cs";
const string Validator="docs/planning/ValidateM5PublicationBoundary.cs";
const string DecisionDocument="docs/planning/decisions/m5-w02-c03-t2-publication-boundary.md";
const string Migration="docs/distribution/m5-v1-migration-and-rollback.md";
const string LocalRecipe="docs/distribution/m5-v1-local-candidate.md";
const string OwnerDraft="docs/coordination/requests/MATH-XR-005-v1-owner-publication-decision-unsent.md";
const string PriorHandoff="docs/coordination/requests/MATH-XR-004-local-v1-consumer-adoption-draft.md";
const string Evidence="docs/planning/evidence/M5-W02-C03-T2-boundaries-qualified.json";
const string WorkProof="docs/planning/evidence/M5-W02-work-package-qualified.json";
const string EvidenceStatus="local-v1-publication-owner-and-recipient-boundaries-qualified-not-released";
const string WorkStatus="m5-w02-local-v1-qualification-complete-no-stable-publication";
const string BoundaryMarker="dw.tools.math/m5-local-publication-boundary/1.0 qualified; oracles=12; recipients=3; public=false";
const string V1Marker="dw.tools.math/m5-v1-local-rc-evidence/1.0 qualified; criteria=10; predecessor-gates=5; publication=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS079",Milestone,
    target?null:Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS078",1,
    "docs: qualify ten Math V1 criteria and prepare local release candidate",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("ten-v1-criteria-and-prior-gates-local-release-evidence-candidate-qualified"));
p.Require.JsonValueEquals(Prior,"/v1_criteria_qualified",JsonValue.Create(10));
p.Require.JsonValueEquals(Prior,"/bounded_scientific_witnesses_checked",JsonValue.Create(17));
p.Require.JsonValueEquals(Prior,"/prior_milestone_gates_verified",JsonValue.Create(5));
p.Require.JsonValueEquals(Prior,"/m5_w02_c03_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w02_c03_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/recipient_prompts_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/public_nuget_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PreviousPackage,"/status",
    JsonValue.Create("managed-api-native-rid-and-cross-version-release-boundaries-qualified"));
p.Require.JsonValueEquals(PreviousPackage,"/local_distribution_version",JsonValue.Create("1.0.0-preview.4"));
p.Require.JsonValueEquals(PreviousPackage,"/other_platform_rids_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(PreviousPackage,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(PreviousWork,"/m5_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(SourceRights,"/authorization/redistribute_from_dw_tools_math",JsonValue.Create(true));
p.Require.JsonValueEquals(LegacyMap,"/m5_final_gate_completed",JsonValue.Create(false));
p.Require.JsonValueEquals(LegacyMap,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.77"),JsonValue.Create("0.1.78"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.78":"0.1.77"));
foreach(var i in new[]{0,1,2,3,4})
    p.Require.JsonValueEquals(Backlog,$"/releases/{i}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",
    JsonValue.Create(target?"done":"in_progress"));
foreach(var i in new[]{33,34,35,36,37,38})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{i}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/39/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState("M5-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
if(!target)
{
    foreach(var path in new[]{ReleaseMap,Validator,DecisionDocument,Migration,
        OwnerDraft,Evidence,WorkProof})
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(ReleaseMap,"/status",
        JsonValue.Create("local-publication-owner-and-recipient-boundaries-qualified-unreleased"));
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(EvidenceStatus));
    p.Require.JsonValueEquals(Evidence,"/read_only_release_boundary_oracles",JsonValue.Create(12));
    p.Require.JsonValueEquals(Evidence,"/recipient_drafts_unsent",JsonValue.Create(3));
    p.Require.JsonValueEquals(Evidence,"/m5_w02_c03_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Evidence,"/m5_w02_c03_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Evidence,"/m5_w02_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Evidence,"/m5_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(WorkProof,"/status",JsonValue.Create(WorkStatus));
    p.Require.JsonValueEquals(WorkProof,"/m5_w02_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(WorkProof,"/m5_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{ReleaseMap,LegacyValidator,Validator,DecisionDocument,
    Migration,LocalRecipe,OwnerDraft})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing bounded release source SHA: "+file);
}
var v1Output=RunDotnet(root,"run","--file",LegacyValidator,"--","--check");
if(!HasLine(v1Output,V1Marker))
    throw new InvalidOperationException("All ten V1 criteria must remain independently qualified.");
var boundaryOutput=RunDotnet(root,"run","--file",Validator,"--","--check");
if(!HasLine(boundaryOutput,BoundaryMarker))
    throw new InvalidOperationException("Independent owner/publication/recipient boundary gate did not pass.");
var oracleLines=boundaryOutput.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
    .Where(x=>x.StartsWith("M5-BOUNDARY-ORACLE:",StringComparison.Ordinal)).ToArray();
var oracleLabels=new[]{
    "candidate-identity","source-links","ten-canonical-v1",
    "owner-source-rights-not-publication","real-local-nuget-and-host",
    "not-a-cross-version-binary-contract","release-refusal-no-action",
    "three-unsent-recipient-states","unsent-draft-content-and-historic-hash",
    "migration-and-rollback-only-on-consumer-side",
    "future-owner-decisions-are-explicit","workflow-and-backlog-are-not-promoted"
};
if(oracleLines.Length!=oracleLabels.Length ||
   oracleLines.Where((line,i)=>line!="M5-BOUNDARY-ORACLE:"+oracleLabels[i]).Any())
    throw new InvalidOperationException("Require 12 distinct documented read-only release boundary oracles.");
if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W02-C03-T2-boundaries-qualified",
        ["status"]=EvidenceStatus,
        ["prior_ten_criteria_evidence"]=Prior,
        ["prior_scientific_package_evidence"]=PreviousPackage,
        ["owner_source_rights_evidence"]=SourceRights,
        ["unsent_existing_consumer_draft"]=PriorHandoff,
        ["question"]="Does a local Math V1 candidate meet owner/publication, external recipient, API/RID and migration/rollback boundaries without shipping any stable package?",
        ["observation"]="A read-only independent C# gate verified 12 actual source-linked refusal oracles and three untransmitted recipient states; the ten canonical V1 criteria remain qualified and net10 archives remain preview.4 on the observed host.",
        ["decision"]="Qualify the local release documentary boundary and completed work package, withhold public/stable 1.0 and final M5 gate until owner authorization and independent future acceptance.",
        ["publication_matrix"]=ReleaseMap,
        ["publication_matrix_sha256"]=hashes[ReleaseMap],
        ["t1_validator_extended"]=LegacyValidator,
        ["t1_validator_extended_sha256"]=hashes[LegacyValidator],
        ["t1_validator_historical_sha256"]=p.Json.String(Prior,"/validator_sha256"),
        ["boundary_validator"]=Validator,["boundary_validator_sha256"]=hashes[Validator],
        ["decision_document"]=DecisionDocument,
        ["decision_document_sha256"]=hashes[DecisionDocument],
        ["migration_recipe"]=Migration,["migration_recipe_sha256"]=hashes[Migration],
        ["candidate_recipe"]=LocalRecipe,
        ["candidate_recipe_sha256"]=hashes[LocalRecipe],
        ["unsent_owner_decision_request"]=OwnerDraft,
        ["unsent_owner_decision_request_sha256"]=hashes[OwnerDraft],
        ["local_candidate_version"]="1.0.0-preview.4",
        ["read_only_release_boundary_oracles"]=12,
        ["read_only_release_boundary_oracle_receipts"]=p.Json.StringArray(oracleLines),
        ["ten_original_v1_criteria_retained"]=true,
        ["bounded_scientific_witnesses_retained"]=17,
        ["prior_milestone_gates_retained"]=5,
        ["recipient_drafts_unsent"]=3,
        ["owner_source_redistribution_rights_attested"]=true,
        ["stable_publication_owner_approval_observed"]=false,
        ["public_nuget_published"]=false,
        ["stable_release_certified"]=false,
        ["other_platforms_certified"]=false,
        ["cross_version_abi_certified"]=false,
        ["recipient_acknowledgments_received"]=false,
        ["external_adoption_or_cutover_executed"]=false,
        ["rollback_executed"]=false,
        ["m5_w02_c03_t2_done"]=true,
        ["m5_w02_c03_phase_done"]=true,
        ["m5_w02_work_package_done"]=true,
        ["m5_release_done"]=false,
        ["candidate_label"]="1.0.0-local-rc-evidence-only",
        ["next_accepted_scope"]="Independent G-M5 gate/release decision; no public stable 1.0 or external adoption without owner authorization.",
        ["nonclaims"]=p.Json.StringArray(
            "Local work-package completion does not certify stable 1.0 or final M5 gate.",
            "Math has no authority to change AURA, Decision, MCDM or their deployment state.",
            "No new license, package publication, untested OS RID, cross-version ABI or rollback execution.")
    };
    p.Files.WriteComplete(Evidence,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    var work=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W02-work-package",
        ["status"]=WorkStatus,
        ["work_package"]="M5-W02",
        ["qualified_phase_ids"]=p.Json.StringArray("M5-W02-C01","M5-W02-C02","M5-W02-C03"),
        ["qualified_phase_evidence"]=p.Json.StringArray(
            "docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json",
            PreviousPackage,Evidence),
        ["ten_original_v1_criteria_qualified"]=10,
        ["prior_five_gate_facts_qualified"]=true,
        ["source_free_local_net10_archives_qualified"]=7,
        ["observed_host"]="win-x64",
        ["local_distribution_version"]="1.0.0-preview.4",
        ["local_scope_qualified"]=true,
        ["owner_stable_publication_authorized"]=false,
        ["public_nuget_published"]=false,
        ["cross_os_release_certified"]=false,
        ["cross_version_binary_compatibility_certified"]=false,
        ["external_agent_adoption_observed"]=false,
        ["m5_w02_work_package_done"]=true,
        ["m5_release_done"]=false,
        ["final_m5_gate_pending"]=true,
        ["boundary_validator"]=Validator,
        ["boundary_validator_sha256"]=hashes[Validator],
        ["next_gate"]="Independent G-M5 milestone acceptance and owner release decision."
    };
    p.Files.WriteComplete(WorkProof,
        work.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,roadmap=>
    {
        roadmap["plan_version"]="0.1.78";
        var chunk=roadmap["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        var workPackage=roadmap["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        if((string?)chunk["status"]!="in_progress"||
           (string?)task["status"]!="planned"||
           (string?)workPackage["status"]!="in_progress")
            throw new InvalidOperationException("M5 C03-T2 accepted phase/work-package baseline changed.");
        chunk["status"]="done";
        chunk["refinement"]="RS078 T1 independently proved 10 canonical V1 criteria, 17 source-bound mathematical witnesses and five M0-M4 receipts as an evidence-only local candidate. RS079 T2 qualifies owner/source rights vs stable publication refusal, three unsent recipient states, observed-host-only support and future opt-in migration/rollback rules with twelve read-only independent C# oracles. The current distribution stays local preview.4. C03 and M5-W02 close as bounded local work; M5, G-M5 and all public/stable/external adoption gates remain open.";
        chunk["readiness"]="Complete local candidate publication/recipient/migration boundary; future final G-M5 requires owner release decision and independent acceptance; no stable package publicly published";
        var previously=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(previously.Concat(new[]{
            ReleaseMap,LegacyValidator,Validator,DecisionDocument,Migration,
            LocalRecipe,OwnerDraft,Evidence,WorkProof
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file "+LegacyValidator+" -- --check",
            "dotnet run --file "+Validator+" -- --check",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        var priorEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(
            priorEvidence.Concat(new[]{Evidence,WorkProof})
                .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Evidence);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Evidence);
        }
        workPackage["status"]="done";
        workPackage["evidence"]=p.Json.StringArray(
            "docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json",
            PreviousPackage,Prior,Evidence,WorkProof);
    });
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Question,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Question);
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var node in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState("M5-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C03-T1","done");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS079" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=(target?null:Work) ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null||state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS079 current local-work-package cursor mismatched baseline/target.");
    state["activeWorkPackage"]=null;
    state["activePhase"]=null;
});
if(!target)
    RunDotnet(root,"run","--file","docs/planning/ValidatePlan.cs","--","--write");
var finalV1=RunDotnet(root,"run","--file",LegacyValidator,"--","--check");
if(!HasLine(finalV1,V1Marker))
    throw new InvalidOperationException("Ten previously admitted V1 criteria regressed at C03 work-package closure.");
var finalBoundary=RunDotnet(root,"run","--file",Validator,"--","--check");
if(!HasLine(finalBoundary,BoundaryMarker))
    throw new InvalidOperationException("Publication/recipient boundary regressed after local work package closure.");
RunDotnet(root,"run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
RunDotnet(root,"run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasLine(string output,string expected)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static string RunDotnet(string root,params string[] arguments)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName="dotnet",WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var argument in arguments)process.StartInfo.ArgumentList.Add(argument);
    if(!process.Start())throw new InvalidOperationException("Cannot start local dotnet.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("Bounded .NET evidence validator timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("Independent Math candidate dotnet "+
            string.Join(" ",arguments)+" failed with "+process.ExitCode+
            ":\n"+(output.Length>2600?output[^2600..]:output));
    return output;
}
