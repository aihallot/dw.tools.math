using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string M="M5",W="M5-W02",C="M5-W02-C03",T="M5-W02-C03-T1";
const string A="M5-W02-C03-T1-A",B="M5-W02-C03-T1-B",D="M5-W02-C03-T1-C";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Last="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string W01="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string GateM4="docs/planning/evidence/M4-gate.json";
const string Map="docs/planning/releases/m5-v1-local-candidate-map.json";
const string Validator="docs/planning/ValidateM5ReleaseCandidate.cs";
const string Decision="docs/planning/decisions/m5-w02-c03-t1-release-candidate.md";
const string Recipe="docs/distribution/m5-v1-local-candidate.md";
const string Handoffs="docs/coordination/requests/MATH-XR-004-local-v1-consumer-adoption-draft.md";
const string Proof="docs/planning/evidence/M5-W02-C03-T1-local-candidate-qualified.json";
const string QualifiedStatus="ten-v1-criteria-and-prior-gates-local-release-evidence-candidate-qualified";
const string Marker="dw.tools.math/m5-v1-local-rc-evidence/1.0 qualified; criteria=10; predecessor-gates=5; publication=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Proof));
p.Workflow.RequireCurrentRun("RS078",M,W,target?C:null,null,null);
p.Workflow.RequirePriorSuccess("RS077",1,
    "feat: qualify managed Math package API and RID boundaries and close C02",
    "0.1.59",2,2,0,false);
p.Require.JsonValueEquals(Last,"/status",
    JsonValue.Create("managed-api-native-rid-and-cross-version-release-boundaries-qualified"));
p.Require.JsonValueEquals(Last,"/independent_green_tests_passed",JsonValue.Create(36));
p.Require.JsonValueEquals(Last,"/red_recovered_and_green_tests_passed",JsonValue.Create(37));
p.Require.JsonValueEquals(Last,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Last,"/retained_clean_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Last,"/retained_sample_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Last,"/inspected_local_archives",JsonValue.Create(7));
p.Require.JsonValueEquals(Last,"/local_distribution_version",
    JsonValue.Create("1.0.0-preview.4"));
p.Require.JsonValueEquals(Last,"/cross_version_binary_compatibility_certified",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/other_platform_rids_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/public_nuget_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/m5_w02_c02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Last,"/m5_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(W01,"/m5_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(W01,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(GateM4,"/m4_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(GateM4,"/m5_milestone_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.76"),JsonValue.Create("0.1.77"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.77":"0.1.76"));
foreach(var i in new[]{0,1,2,3,4})
    p.Require.JsonValueEquals(Backlog,$"/releases/{i}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("in_progress"));
foreach(var i in new[]{33,34,35,36,37,38})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{i}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/tasks/1/status",JsonValue.Create("planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/39/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(M,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(W,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState("M5-W02-C02","done");
p.ProjectPlan.RequireNodeState(C,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(T,target?"done":"not-ready");
foreach(var node in new[]{A,B,D})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03-T2","not-ready");

if(!target)
{
    foreach(var path in new[]{Map,Validator,Decision,Recipe,Handoffs,Proof})
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Map,"/status",
        JsonValue.Create("bounded-v1-evidence-map-local-rc-not-promoted"));
    p.Require.JsonValueEquals(Proof,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Proof,"/v1_criteria_qualified",JsonValue.Create(10));
    p.Require.JsonValueEquals(Proof,"/bounded_scientific_witnesses_checked",
        JsonValue.Create(17));
    p.Require.JsonValueEquals(Proof,"/prior_milestone_gates_verified",
        JsonValue.Create(5));
    p.Require.JsonValueEquals(Proof,"/m5_w02_c03_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Proof,"/m5_w02_c03_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Proof,"/m5_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Proof,"/m5_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Map,Validator,Decision,Recipe,Handoffs})
{
    var stage=p.Files.ReplaceFromStaged("staged/"+path,path);
    hashes[path]=stage.AfterSha256 ??
        throw new InvalidOperationException("Missing local candidate asset receipt: "+path);
}
var scan=RunDotnet(root,"run","--file",Validator,"--","--check");
if(!HasLine(scan,Marker))
    throw new InvalidOperationException("Ten original V1 criteria and five predecessor gates were not independently accepted.");
var receipts=scan.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
    .Where(x=>x.StartsWith("M5-V1-CRITERION:",StringComparison.Ordinal)).ToArray();
if(receipts.Length!=10)
    throw new InvalidOperationException("Exactly ten independent source-bound V1 scientific receipts are required.");
for(var i=0;i<10;i++)
{
    var prefix="M5-V1-CRITERION:V1-"+(i+1).ToString("00",
        System.Globalization.CultureInfo.InvariantCulture)+":";
    var found=receipts[i];
    var digest=found.StartsWith(prefix,StringComparison.Ordinal)?
        found[prefix.Length..]:string.Empty;
    if(digest.Length!=64 ||
        digest.Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("V1 criterion receipt order/SHA is invalid: "+prefix);
}
if(!target)
{
    var scientific=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W02-C03-T1-local-candidate",
        ["status"]=QualifiedStatus,
        ["question"]="Can all ten canonical V1 criteria and M0-M4 gates be independently traced to bounded scientific evidence without claiming that M5 or publication has completed?",
        ["observation"]="Ten canonical coverage lines, 17 scoped Math witnesses and five accepted predecessor receipts passed the separate read-only C# gate; earlier standalone console and seven net10 product packages remain preview only.",
        ["decision"]="Prepare only local evidence-backed 1.0 candidate documentation and three unsent consumer prompts. M5-W02-C03-T2, work package, final M5 gate and stable/public release remain incomplete.",
        ["v1_source_coverage"]="docs/planning/coverage.md",
        ["candidate_map"]=Map,["candidate_map_sha256"]=hashes[Map],
        ["validator"]=Validator,["validator_sha256"]=hashes[Validator],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["reproducibility_recipe"]=Recipe,["recipe_sha256"]=hashes[Recipe],
        ["unsent_recipient_drafts"]=Handoffs,
        ["unsent_recipient_drafts_sha256"]=hashes[Handoffs],
        ["validator_marker"]=Marker,
        ["v1_criteria_qualified"]=10,
        ["bounded_scientific_witnesses_checked"]=17,
        ["prior_milestone_gates_verified"]=5,
        ["v1_criterion_sha_receipts"]=p.Json.StringArray(receipts),
        ["required_m5_phases"]=7,
        ["completed_m5_phases_prior_to_candidate"]=6,
        ["local_product_version"]="1.0.0-preview.4",
        ["candidate_label"]="1.0.0-local-rc-evidence-only",
        ["all_v1_mathematics_unbounded"]=false,
        ["m5_final_gate_certified"]=false,
        ["stable_publication_certified"]=false,
        ["public_nuget_published"]=false,
        ["external_aura_decision_mcdm_adoption_observed"]=false,
        ["cross_version_binary_compatibility_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["recipient_prompts_transmitted"]=false,
        ["m5_w02_c03_t1_done"]=true,
        ["m5_w02_c03_phase_done"]=false,
        ["m5_w02_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=RunDotnet(root,"--version").Trim(),
        ["observed_runtime_identifier"]=System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W02-C03-T2: owner-controlled publication, unsent recipient drafts, migration rollback, current-platform-only stable release nonclaims",
        ["nonclaims"]=p.Json.StringArray(
            "Ten bounded criteria do not imply arbitrary mathematics or a general solver/parser.",
            "No public NuGet, stable 1.0, owner licensing change or unobserved OS support.",
            "No actual AURA/Decision/MCDM adoption or recipient transmission.",
            "T1 only completes: M5-W02-C03-T2, C03, M5-W02 and G-M5 remain open.")
    };
    p.Files.WriteComplete(Proof,
        scientific.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,roadmap=>
    {
        roadmap["plan_version"]="0.1.77";
        var chunk=roadmap["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==C);
        var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==T);
        if((string?)chunk["status"]!="planned"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5 V1 release candidate accepted starting state changed.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS078 T1 qualifies ten canonical V1 framing criteria against 17 exact scoped evidence witnesses and five M0-M4 predecessor gates, with one read-only executable C# validator, a bounded local 1.0 candidate map and documentation/unsent AURA Decision MCDM recipient drafts. This is an evidence-only local candidate; distribution remains preview.4 and six prior M5 phases are done. Publication, cross-OS support, owner licensing choices, recipient adoption and M5 G-M5 remain unqualified; C03 T2 is planned.";
        chunk["readiness"]="C03-T1 local ten-V1-criterion evidence candidate documented and independently checked; C03-T2 owner/publication/external-adoption boundaries still pending";
        chunk["files"]=p.Json.StringArray(Map,Validator,Decision,Recipe,Handoffs,Proof);
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file "+Validator+" -- --check",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        chunk["evidence"]=p.Json.StringArray(Proof);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Proof);
        foreach(var node in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            node["status"]="done";
            node["evidence"]=p.Json.StringArray(Proof);
        }
    });
    p.ProjectPlan.TransitionNode(C,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T,"not-ready","ready");
    p.ProjectPlan.TransitionNode(A,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(A);
    p.ProjectPlan.ConvergeNodeToDone(A);
    foreach(var node in new[]{B,D})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(T);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(M,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(W,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState("M5-W02-C02","done");
p.ProjectPlan.RequireNodeState(C,"in-progress");
foreach(var node in new[]{T,A,B,D})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C03-T2","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS078" ||
       (string?)state["activeMilestone"]!=M ||
       (string?)state["activeWorkPackage"]!=W ||
       (string?)state["activePhase"]!=(target?C:null) ||
       state["activeTask"] is not null||state["activeSubtask"] is not null)
        throw new InvalidOperationException("Local V1 candidate active phase cursor drifted.");
    state["activePhase"]=C;
});
if(!target)
    RunDotnet(root,"run","--file","docs/planning/ValidatePlan.cs","--","--write");
RunDotnet(root,"run","--file",Validator,"--","--check");
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
