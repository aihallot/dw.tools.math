using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7",Work="M7-W02",Phase="M7-W02-C01";
const string Task="M7-W02-C01-T1",Question="M7-W02-C01-T1-A";
const string Observation="M7-W02-C01-T1-B",DecisionNode="M7-W02-C01-T1-C";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Fixture="docs/planning/fixtures/m7-polynomial-identity-certificate.json";
const string Decision="docs/planning/decisions/m7-w02-c01-t1.md";
const string Screen="docs/distribution/m7-formal-provider-screen.md";
const string Checker="docs/planning/ValidateM7FormalCertificate.cs";
const string Qualified="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string Status="m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified";
const string Marker="dw.tools.math/m7-w02-c01-t1-independent-certificate/1.2 qualified; z-polynomial=-1,0,1; altered=invalid; hole=unproved; timeout=unproved; provider=none";
const string ExactMarker="M7-FORMAL-CERTIFICATE:Z-POLYNOMIAL:PRODUCT=-1,0,1;VERDICT=verified-finite-identity";
const string RefusalMarker="M7-FORMAL-CERTIFICATE:NEGATIVE:ALTERED=invalid-certificate;TRUNCATED=invalid-certificate;HOLE=unproved;TIMEOUT=unproved;PROVIDER-FAILURE=unproved";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS118",Milestone,target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS117",1,
    "docs: qualify M7 W01 advanced research portfolio",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.116"),JsonValue.Create("0.1.117"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.117":"0.1.116"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"));
p.Require.JsonValueEquals(Prior,"/m7_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m7_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(Prior,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");

foreach(var (ptr,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),
    ("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status","in_progress"),
    ("/work_packages/15/id","M7-W01"),
    ("/work_packages/15/status","done"),
    ("/work_packages/16/id",Work),
    ("/work_packages/16/status",target?"in_progress":"planned"),
    ("/chunks/53/status","done"),
    ("/chunks/54/status","done"),
    ("/chunks/55/status","done"),
    ("/chunks/56/id",Phase),
    ("/chunks/56/kind","spike"),
    ("/chunks/56/size","M"),
    ("/chunks/56/status",target?"in_progress":"planned"),
    ("/chunks/56/tasks/0/id",Task),
    ("/chunks/56/tasks/0/status",target?"done":"planned"),
    ("/chunks/56/tasks/1/id","M7-W02-C01-T2"),
    ("/chunks/56/tasks/1/status","planned"),
    ("/chunks/57/id","M7-W02-C02"),
    ("/chunks/57/kind","document"),
    ("/chunks/57/status","planned")
})
    p.Require.JsonValueEquals(Backlog,ptr,JsonValue.Create(value));
for(var i=0;i<3;i++)
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/56/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/56/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create("planned"));
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState("M7-W02-C02","not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M7-W02-C01-T2","M7-W02-C01-T2-A",
    "M7-W02-C01-T2-B","M7-W02-C01-T2-C","M7-W02-C02-T1",
    "M7-W02-C02-T2"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var file in new[]{Fixture,Decision,Screen,Checker,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/certificate_verdict",
        JsonValue.Create("verified-finite-identity"));
    p.Require.JsonValueEquals(Qualified,"/certificate_kind",
        JsonValue.Create("finite-integer-polynomial-coefficients"));
    p.Require.JsonValueEquals(Qualified,"/reference_degree",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/reference_coefficient_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Qualified,"/independent_oracle_groups",JsonValue.Create(2));
    p.Require.JsonValueEquals(Qualified,"/altered_certificate_rejected",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/hole_unproved",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/timeout_unproved",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/provider_failure_unproved",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/t2_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/c01_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m7_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_prover_adopted",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/public_formal_api_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/general_proof_kernel_certified",JsonValue.Create(false));
}
var fixtureWrite=p.Files.ReplaceFromStaged("staged/"+Fixture,Fixture);
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var screenWrite=p.Files.ReplaceFromStaged("staged/"+Screen,Screen);
var checkerWrite=p.Files.ReplaceFromStaged("staged/"+Checker,Checker);
var fixtureSha=fixtureWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged certificate fixture exact SHA-256 missing.");
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged formal Q/O/D decision exact SHA-256 missing.");
var screenSha=screenWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged formal provider screen exact SHA-256 missing.");
var checkerSha=checkerWrite.AfterSha256 ??
    throw new InvalidOperationException("Staged independent certificate verifier SHA-256 missing.");
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "Z[x]","BigInteger","verified","hole","timeout",
    "Lean","Rocq","SMT","No new","source-free"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Machine-readable reference and falsifiable outcomes",
    "## Candidate formal technologies and provenance gate",
    "## Three admission triggers","Lean","Rocq","SMT",
    "unproved","source-free","trusted base"})
    p.Require.TextContains(Screen,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Fixture,"untrusted-until-local-replay");
p.Require.TextContains(Checker,ExactMarker);
p.Require.TextContains(Checker,RefusalMarker);

var before=Required(root,"dotnet","run","--file",Checker,"--","--check");
foreach(var marker in new[]{ExactMarker,RefusalMarker,Marker})
    if(!before.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Finite certificate replay marker absent: "+marker);

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-W02-C01-T1-qualified",
        ["status"]=Status,
        ["question"]="Can a finite Z[x] polynomial certificate be independently replayed while distinguishing kernel proof, certificate, derivation, hole and timeout?",
        ["observation"]="A source-owned coefficient fixture for (x-1)(x+1)=x^2-1 replays with exact BCL BigInteger multiplication to [-1,0,1]; altered/truncated witnesses are invalid and missing/hole/timeout/provider failure remain unproved.",
        ["decision"]="Admit one bounded local integer polynomial identity certificate and error-status research only. Defer proof assistant/toolchain/provider admission and any public formal API until independently qualified T2 ownership, rights and trusted-base gates.",
        ["prior_m7_w01_research_gate"]=Prior,
        ["certificate_fixture"]=Fixture,
        ["fixture_sha256"]=fixtureSha,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["provider_screen"]=Screen,
        ["screen_sha256"]=screenSha,
        ["validator"]=Checker,
        ["validator_sha256"]=checkerSha,
        ["certificate_kind"]="finite-integer-polynomial-coefficients",
        ["certificate_domain"]="Z[x]",
        ["certificate_claim"]="(x-1)(x+1)=x^2-1",
        ["certificate_coefficients"]="-1,0,1",
        ["certificate_verdict"]="verified-finite-identity",
        ["reference_degree"]=2,
        ["reference_coefficient_count"]=3,
        ["independent_oracle_groups"]=2,
        ["altered_certificate_rejected"]=true,
        ["truncated_certificate_rejected"]=true,
        ["missing_certificate_unproved"]=true,
        ["hole_unproved"]=true,
        ["timeout_unproved"]=true,
        ["provider_failure_unproved"]=true,
        ["trusted_base"]="local finite C# verifier and BCL BigInteger",
        ["external_prover_adopted"]=false,
        ["lean_kernel_qualified"]=false,
        ["rocq_kernel_qualified"]=false,
        ["smt_certificate_checker_qualified"]=false,
        ["general_proof_kernel_certified"]=false,
        ["source_free_consumer_qualified"]=false,
        ["public_formal_api_delivered"]=false,
        ["publicly_published"]=false,
        ["new_math_product_package_built"]=false,
        ["t1_done"]=true,
        ["t2_done"]=false,
        ["c01_phase_done"]=false,
        ["m7_w02_work_package_done"]=false,
        ["m7_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7-W02-C01-T2: independent trusted-base/license/toolchain review, exact negative/unproved statuses and source-free consumer gate; choose a qualified provider or explicit documented deferral.",
        ["nonclaims"]=p.Json.StringArray(
            "One BCL-checked polynomial identity is not a trusted Lean/Rocq kernel theorem, SMT certificate or general theorem prover.",
            "A hole, timeout, provider error, wrong or incomplete certificate is not mathematical proof; sample evaluations are not sufficient.",
            "No external proof assistant/toolchain/version/license/RID, general certified proof API, public NuGet or source-free consumer is qualified.",
            "M7-W02-C01 T2, formal research phase, W02 work package and M7 1.2.0-candidate release remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.116")
            throw new InvalidOperationException("M7 W02 formal research baseline version drifted.");
        var milestone=doc["releases"]!.AsArray()[7]!.AsObject();
        var previous=doc["work_packages"]!.AsArray()[15]!.AsObject();
        var work=doc["work_packages"]!.AsArray()[16]!.AsObject();
        var phase=doc["chunks"]!.AsArray()[56]!.AsObject();
        var other=doc["chunks"]!.AsArray()[57]!.AsObject();
        var task=phase["tasks"]!.AsArray()[0]!.AsObject();
        if((string?)milestone["id"]!=Milestone ||
            (string?)milestone["status"]!="in_progress" ||
            (string?)previous["id"]!="M7-W01" ||
            (string?)previous["status"]!="done" ||
            (string?)work["id"]!=Work ||
            (string?)work["status"]!="planned" ||
            (string?)phase["id"]!=Phase ||
            (string?)phase["size"]!="M" ||
            (string?)phase["kind"]!="spike" ||
            (string?)phase["status"]!="planned" ||
            (string?)task["id"]!=Task ||
            (string?)task["status"]!="planned" ||
            (string?)phase["tasks"]!.AsArray()[1]!["status"]!="planned" ||
            (string?)other["id"]!="M7-W02-C02" ||
            (string?)other["status"]!="planned")
            throw new InvalidOperationException("Formal interoperability T1 accepted baseline has drifted.");
        doc["plan_version"]="0.1.117";
        work["status"]="in_progress";
        phase["status"]="in_progress";
        phase["refinement"]="RS118 T1 researches formal interoperability via an independently replayed finite Z[x] coefficient certificate, exact multiplication result [-1,0,1], and unproved hole/timeout/provider-failure statuses. Lean/Rocq/SMT are candidate categories only; no external trusted kernel, source/rights/RID or source-free recipient is qualified.";
        phase["readiness"]="Finite exact polynomial certificate BCL replay and false/unknown result refusals qualified as research Q/O/D only. C01 T2 must still decide owner/toolchain/trusted base and external formal integration admission or deferral before C01 can close.";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Fixture,Decision,Screen,Checker,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        var evidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidateM7FormalCertificate.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)sub["status"]!="planned")
                throw new InvalidOperationException("Formal T1 Q/O/D child was not planned.");
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var node in new[]{Work,Phase,Task,Question})
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
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M7-W02-C02","not-ready");
foreach(var node in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M7-W02-C01-T2","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS118" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=(target?Work:null) ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 formal proof research workflow cursor.");
    state["activeWorkPackage"]=Work;
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Checker,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Qualified formal certificate target failed BCL replay.");
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
        throw new InvalidOperationException("RS118 bounded certificate replay or planning command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS118 independent proof/certificate check timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS118 local finite proof certificate/planning check failed: "+
            executable+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2300)]);
    return output;
}
