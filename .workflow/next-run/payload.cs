using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M6", Work="M6-W02", Phase="M6-W02-C05", Task="M6-W02-C05-T2";
const string Question="M6-W02-C05-T2-A", Observation="M6-W02-C05-T2-B", DecisionNode="M6-W02-C05-T2-C";
const string State=".aura/workflow/state.json", Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C05-T1-qualified.json";
const string PriorScientific="docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json";
const string Publication="docs/planning/releases/m5-v1-publication-boundary.json";
const string Decision="docs/planning/decisions/m6-w02-c05-t2.md";
const string Notices="docs/distribution/m6-scientific-adoption-notices.md";
const string Qualified="docs/planning/evidence/M6-W02-C05-T2-qualified.json";
const string Status="owner-gated-scientific-adoption-no-cutover-and-notices-qualified";
const string LatestVersion="1.1.0-preview.14";
const string LatestArchiveSha="2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf";
const long LatestArchiveBytes=37270;
const int FamilyCount=7, RetainedConsumerCount=7;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS104",Milestone,target?null:Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS103",1,
    "docs: qualify M6 scientific family pins and Decision reuse assessment",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("seven-scientific-family-contract-pins-and-owner-gated-reuse-request-documented"));
p.Require.JsonValueEquals(Prior,"/qualified_family_count",JsonValue.Create(FamilyCount));
p.Require.JsonValueEquals(Prior,"/previous_source_free_consumer_count",
    JsonValue.Create(RetainedConsumerCount));
p.Require.JsonValueEquals(Prior,"/prior_package_version",JsonValue.Create(LatestVersion));
p.Require.JsonValueEquals(Prior,"/prior_package_sha256",JsonValue.Create(LatestArchiveSha));
p.Require.JsonValueEquals(Prior,"/prior_package_bytes",JsonValue.Create(LatestArchiveBytes));
p.Require.JsonValueEquals(Prior,"/historical_receipts_preserved",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/decision_duplicate_verified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/decision_owner_approval_obtained",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/decision_sibling_modified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/new_scientific_tests_executed",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w02_c05_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_c05_t2_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w02_c05_parent_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorScientific,"/local_product_version",JsonValue.Create(LatestVersion));
p.Require.JsonValueEquals(PriorScientific,"/actual_inspected_package_sha256",
    JsonValue.Create(LatestArchiveSha));
p.Require.JsonValueEquals(PriorScientific,"/actual_inspected_package_bytes",
    JsonValue.Create(LatestArchiveBytes));
p.Require.JsonValueEquals(Publication,"/owner/stable_v1_publication_authorized",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Publication,"/publication/public_nuget_published",
    JsonValue.Create(false));
p.Require.TextContains("src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<Version>1.1.0-preview.14</Version>");
p.Require.TextContains("docs/distribution/m6-scientific-family-adoption.md",
    "1.1.0-preview.14");
p.Require.TextContains("docs/planning/requests/decision-bounded-bracketed-root-assessment.md",
    "No duplicate implementation");
p.Require.TextContains("docs/distribution/m5-package-notices.md",
    "does not select or grant an open-source license");
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.102"),JsonValue.Create("0.1.103"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.103":"0.1.102"));
foreach(var(pointer,value) in new (string,string)[]
{
    ("/status","in_progress"),("/releases/5/status","done"),
    ("/releases/6/status","in_progress"),("/work_packages/13/status","done"),
    ("/work_packages/14/status",target?"done":"in_progress"),
    ("/chunks/43/status","done"),("/chunks/44/status","done"),
    ("/chunks/45/status","done"),("/chunks/46/status","done"),
    ("/chunks/47/status","done"),("/chunks/48/status","done"),
    ("/chunks/49/status","done"),("/chunks/50/id",Phase),
    ("/chunks/50/kind","document"),
    ("/chunks/50/status",target?"done":"in_progress"),
    ("/chunks/50/tasks/0/id","M6-W02-C05-T1"),
    ("/chunks/50/tasks/0/status","done"),
    ("/chunks/50/tasks/1/id",Task),
    ("/chunks/50/tasks/1/status",target?"done":"planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var index in new[]{0,1,2})
{
    p.Require.JsonValueEquals(Backlog,
        "/chunks/50/tasks/0/subtasks/"+index+"/status",JsonValue.Create("done"));
    p.Require.JsonValueEquals(Backlog,
        "/chunks/50/tasks/1/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
}
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
foreach(var id in new[]{"M6-W02-C01","M6-W02-C02","M6-W02-C03",
    "M6-W02-C03-S01","M6-W02-C03-S02","M6-W02-C03-S03","M6-W02-C04"})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C05-T1","done");
foreach(var id in new[]{"M6-W02-C05-T1-A","M6-W02-C05-T1-B","M6-W02-C05-T1-C"})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,Notices,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_c05_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_c05_parent_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/owner_approval_obtained",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/publicly_published",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/external_cutover_performed",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/new_scientific_tests_executed",JsonValue.Create(false));
}

var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var noticesWrite=p.Files.ReplaceFromStaged("staged/"+Notices,Notices);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing T2 decision source SHA-256.");
var noticesSha=noticesWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing T2 notices source SHA-256.");
foreach(var snippet in new[]{"## Question","## Observation","## Decision",
    "No automatic external cutover","G-M6 release gate",
    "owner-authorized","1.1.0-preview.14"})
    p.Require.TextContains(Decision,snippet);
foreach(var snippet in new[]{"## Family dispositions","Retain in Math",
    "Semver and contract-change gates","Notices, rights and operational gates",
    "37,270 bytes","no family is removed"})
    p.Require.TextContains(Notices,snippet);
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M6-W02-C05-T2-qualified",
        ["status"]=Status,
        ["question"]="Which exact owner, semver, source pin, notices and independent-consumer gates apply before any scientific provider adoption or removal?",
        ["observation"]="RS103 admitted seven historical family contracts; RS102 inspected preview.14 with seven source-free clients. Owner release permission and Decision reuse approval are absent.",
        ["decision"]="Retain all seven Math families. Defer any adoption, removal, public publication or external cutover pending independently verified owner and consumer gates.",
        ["prior_documentary_qualification"]=Prior,
        ["prior_scientific_qualification"]=PriorScientific,
        ["prior_package_version"]=LatestVersion,
        ["prior_package_sha256"]=LatestArchiveSha,
        ["prior_package_bytes"]=LatestArchiveBytes,
        ["historical_family_count"]=FamilyCount,
        ["historical_source_free_consumer_count"]=RetainedConsumerCount,
        ["historical_receipts_preserved"]=true,
        ["family_disposition"]="retain-all-seven-pending-explicit-owner-action",
        ["removed_family_count"]=0,
        ["external_adoption_authorized"]=false,
        ["owner_approval_required"]=true,
        ["owner_approval_obtained"]=false,
        ["semver_and_notices_review_required"]=true,
        ["compatibility_and_rollback_review_required"]=true,
        ["immutable_source_and_package_pins_required"]=true,
        ["target_runtime_os_independent_qualification_required"]=true,
        ["decision_request_transmitted"]=false,
        ["decision_duplicate_verified"]=false,
        ["decision_sibling_modified"]=false,
        ["external_cutover_performed"]=false,
        ["publicly_published"]=false,
        ["new_scientific_tests_executed"]=false,
        ["new_scientific_package_built"]=false,
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=decisionSha,
        ["notices_document"]=Notices,
        ["notices_document_sha256"]=noticesSha,
        ["m6_w02_c05_t1_done"]=true,
        ["m6_w02_c05_t2_done"]=true,
        ["m6_w02_c05_parent_done"]=true,
        ["m6_w02_work_package_done"]=true,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="Independent G-M6 release gate: verify all required M6 chunks and scientific local package/consumer evidence; preserve explicit owner/publication blockers.",
        ["nonclaims"]=p.Json.StringArray(
            "No new numerical code, scientific tests, package pack, stable or public release was executed by this document task.",
            "No Decision/AURA/MCDM/dw.discovery integration, sibling mutation, transmitted request, owner approval or provider cutover.",
            "Historic source/package identities remain pinned; no additional OS/ABI, rights grant or certified numeric error guarantee.",
            "C05 and W02 local planning completion is distinct from the unfinished M6 gate and external publication authorization.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.102")
            throw new InvalidOperationException("Unexpected RS104 baseline backlog version.");
        var release=doc["releases"]!.AsArray()[6]!.AsObject();
        var workPackage=doc["work_packages"]!.AsArray()[14]!.AsObject();
        var chunk=doc["chunks"]!.AsArray()[50]!.AsObject();
        var task=chunk["tasks"]!.AsArray()[1]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)workPackage["id"]!=Work ||
            (string?)workPackage["status"]!="in_progress" ||
            (string?)chunk["id"]!=Phase ||
            (string?)chunk["kind"]!="document" ||
            (string?)chunk["status"]!="in_progress" ||
            (string?)task["id"]!=Task ||
            (string?)task["status"]!="planned")
            throw new InvalidOperationException("Scientific adoption T2 baseline changed.");
        doc["plan_version"]="0.1.103";
        chunk["status"]="done";
        chunk["refinement"]="RS103 independently qualified seven historical family pins and a targeted Decision assessment proposal. RS104 documents semver/notices/rights and an explicit retain-and-defer decision without external owner approval. C05 T1/T2 are now done, W02 all phases complete. The separate G-M6 release gate remains open.";
        chunk["readiness"]="C05 documentary acceptance complete: seven family oracle/limits/source/provider pins and historical source-free receipts; owner-gated semver, notices, compatibility and removal disposition. No automatic external cutover, public release or external adoption is claimed.";
        chunk["files"]=p.Json.StringArray(chunk["files"]!.AsArray()
            .Select(item=>item!.GetValue<string>())
            .Concat(new[]{Decision,Notices,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        chunk["evidence"]=p.Json.StringArray(chunk["evidence"]!.AsArray()
            .Select(item=>item!.GetValue<string>())
            .Concat(new[]{Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidateTransferArchitecture.cs");
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(node=>node!.AsObject()))
        {
            if((string?)sub["status"]!="planned")
                throw new InvalidOperationException("Documentary T2 Q/O/D subtask unexpectedly advanced.");
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Qualified);
        }
        workPackage["status"]="done";
        workPackage["evidence"]=p.Json.StringArray(Prior,Qualified);
    });
    foreach(var id in new[]{Task,Question})
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
    foreach(var id in new[]{Task,Question})
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var id in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
foreach(var id in new[]{Question,Observation,DecisionNode,Task,Phase,Work})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS104" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=(target?null:Work) ||
        (string?)state["activePhase"]!=(target?null:Phase) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS104 owner-gate workflow cursor.");
    state["activeWorkPackage"]=null;
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Required(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable, WorkingDirectory=root,
        UseShellExecute=false, RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("Cannot execute documentary validation.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS104 documentary gate validator timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("DWF RS104 validation failed: "+output[..Math.Min(output.Length,2000)]);
    return output;
}
