using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M7";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Advanced="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Formal="docs/planning/evidence/M7-W02-work-package-qualified.json";
const string FormalT2="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string CoverageT2="docs/planning/evidence/M7-W02-C02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-internal-gate-closure.md";
const string Matrix="docs/distribution/m7-internal-gate-matrix.md";
const string Validator="docs/planning/ValidateM7MilestoneGate.cs";
const string Qualified="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Status="m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld";
const string Marker="dw.tools.math/m7-internal-scope-gate/1.2 qualified; milestones=8; chunks=58; m7=research-done; public-release=false; external=unsent";
const string Counts="M7-GATE:MILESTONES=8;PACKAGES=17;CHUNKS=58;REQUIREMENTS=106;SOURCES=20;DISPOSITIONS=6;NODES=541";
const string Scope="M7-GATE:INTERNAL=DONE;PUBLICATION=NOT-AUTHORIZED;CLIENT-ADOPTION=false;PROVIDERS=DEFERRED;PROGRAM=NOT-APPROVED";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS123",target?null:Milestone,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS122",1,
    "docs: qualify M7 W02 research and document closure",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.121"),JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.122":"0.1.121"));
p.Require.JsonValueEquals(Backlog,"/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Advanced,"/status",
    JsonValue.Create("m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished"));
p.Require.JsonValueEquals(Advanced,"/m7_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Advanced,"/research_only",JsonValue.Create(true));
p.Require.JsonValueEquals(Advanced,"/new_advanced_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Advanced,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/status",
    JsonValue.Create("m7-w02-formal-research-and-document-portfolio-qualified-external-products-deferred"));
p.Require.JsonValueEquals(Formal,"/m7_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Formal,"/m7_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/external_approvals_received",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/new_public_product_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/future_program_approved",JsonValue.Create(false));
p.Require.JsonValueEquals(Formal,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(FormalT2,"/external_prover_adopted",JsonValue.Create(false));
p.Require.JsonValueEquals(FormalT2,"/public_formal_api_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(CoverageT2,"/c02_document_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(CoverageT2,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(CoverageT2,"/future_program_approved",JsonValue.Create(false));
p.Require.JsonValueEquals(".aura/workflow/reports/RS122/attempt-001.json",
    "/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(".aura/workflow/reports/RS122/attempt-001.json",
    "/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(true));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 541,");

foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/6/id","M6"),("/releases/6/status","done"),
    ("/releases/7/id",Milestone),
    ("/releases/7/status",target?"done":"in_progress"),
    ("/releases/7/version","1.2.0-candidate"),
    ("/releases/7/gate","G-M7"),
    ("/work_packages/15/id","M7-W01"),
    ("/work_packages/15/status","done"),
    ("/work_packages/16/id","M7-W02"),
    ("/work_packages/16/status","done"),
    ("/chunks/56/status","done"),("/chunks/57/status","done")
})
    p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState("M7-W02","done");
foreach(var chunk in new[]{
    "M7-W01-C01","M7-W01-C02","M7-W01-C03",
    "M7-W01-C03-S01","M7-W01-C03-S02",
    "M7-W02-C01","M7-W02-C02"})
    p.ProjectPlan.RequireNodeState(chunk,"done");
if(!target)
{
    foreach(var file in new[]{Decision,Matrix,Validator,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    foreach(var (pointer,value) in new (string,int)[]
    {
        ("/milestones_done",8),("/work_packages_done",17),
        ("/chunks_done",58),("/requirements_checked",106),
        ("/source_rows_checked",20),("/native_nodes_retained",541),
        ("/unsent_requests",3)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(value));
    foreach(var (pointer,value) in new (string,bool)[]
    {
        ("/m7_internal_scope_done",true),
        ("/public_release_authorized",false),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/future_program_approved",false),
        ("/new_advanced_public_product_delivered",false),
        ("/general_formal_kernel_certified",false)
    })
        p.Require.JsonValueEquals(Qualified,pointer,JsonValue.Create(value));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixWrite=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("M7 internal gate decision source SHA is absent.");
var matrixSha=matrixWrite.AfterSha256 ??
    throw new InvalidOperationException("M7 publication boundary source SHA is absent.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("M7 independent checker SHA is absent.");
foreach(var term in new[]{
    "## Question","## Source-pinned observations",
    "## Functional G-M7 decision",
    "## Publication and owner authorization",
    "## Future scope options","## Independent gate and cost",
    "1.2.0-candidate","NOT authorized"
})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{
    "## Completed accepted scope",
    "## Publication and recipient gate matrix",
    "## Negative assertions and reproducibility",
    "541","106","unsent","NOT authorized"
})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Validator,Marker);
p.Require.TextContains(Validator,Counts);
p.Require.TextContains(Validator,Scope);
var before=Required(root,"dotnet","run","--file",Validator,"--","--check");
foreach(var marker in new[]{Marker,Counts,Scope})
    if(!before.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS123 baseline scientific gate marker missing: "+marker);
if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M7-internal-gate-qualified",
        ["status"]=Status,
        ["question"]="Are all accepted M7 research/decision obligations internally complete with explicit advanced/formal deferrals, without authorizing a public 1.2.0 candidate, recipient migrations or an extended roadmap?",
        ["observation"]="The accepted eight-milestone, seventeen-package, fifty-eight-chunk Math roadmap is complete as bounded products/research/docs. M7 has seven done chunks; W01/W02 receipts are source-pinned, 106 requirement IDs and 20 framing sources retain owners, and all three external requests remain unsent.",
        ["decision"]="Qualify only the internal M7 scientific and scope-decision gate. Mark the accepted research/document roadmap done; withhold any public version/package publication, provider or client adoption, source/license transfer and unapproved new roadmap.",
        ["advanced_work_package_evidence"]=Advanced,
        ["formal_and_coverage_work_package_evidence"]=Formal,
        ["decision_document"]=Decision,
        ["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,
        ["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,
        ["validator_sha256"]=validatorSha,
        ["milestones_done"]=8,
        ["work_packages_done"]=17,
        ["chunks_done"]=58,
        ["m7_accepted_chunks"]=7,
        ["m7_accepted_work_packages"]=2,
        ["requirements_checked"]=106,
        ["distinct_requirement_owners"]=53,
        ["source_rows_checked"]=20,
        ["external_owned_dispositions"]=4,
        ["excluded_dispositions"]=2,
        ["math_local_gates_satisfied"]=4,
        ["unsent_requests"]=3,
        ["native_nodes_retained"]=541,
        ["prior_rs122_attempt"]=1,
        ["prior_rs122_first_pass"]=true,
        ["m7_internal_scope_done"]=true,
        ["public_release_authorized"]=false,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["future_program_approved"]=false,
        ["new_advanced_public_product_delivered"]=false,
        ["general_formal_kernel_certified"]=false,
        ["new_provider_installed"]=false,
        ["sibling_repository_changed"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="Owner decision: whether to qualify/publicly publish a separately authorized package, transmit current-revision AURA/Decision/MCDM requests, or accept a new consumer-driven Math roadmap. No successor M8 is selected.",
        ["nonclaims"]=p.Json.StringArray(
            "The internally done M7 research/decision milestone is not a published 1.2.0 package or universal mathematics implementation.",
            "No advanced tensor, precision/interval, pure DAG, PlotModel or formal trusted-kernel public provider has been introduced by M7.",
            "No external recipient has acknowledged the three unsent Math requests or approved migration.",
            "No new milestone, package publication, public release or irreversible license decision has been authorized.")
    };
    p.Files.WriteComplete(Qualified,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.121"||
           (string?)doc["status"]!="in_progress")
            throw new InvalidOperationException("M7 G-M7 baseline backlog drifted.");
        var release=doc["releases"]!.AsArray()[7]!.AsObject();
        var wp=doc["work_packages"]!.AsArray();
        var chunks=doc["chunks"]!.AsArray();
        if((string?)release["id"]!=Milestone||
           (string?)release["status"]!="in_progress"||
           (string?)release["version"]!="1.2.0-candidate"||
           (string?)release["gate"]!="G-M7"||
           wp.Any(x=>(string?)x!["status"]!="done")||
           chunks.Any(x=>(string?)x!["status"]!="done")||
           doc["releases"]!.AsArray().Take(7)
               .Any(x=>(string?)x!["status"]!="done"))
            throw new InvalidOperationException("M7 accepted research scope not complete.");
        var required=release["required_chunks"]!.AsArray()
            .Select(x=>x!.GetValue<string>()).ToArray();
        var actual=chunks.Where(x=>(string?)x!["release"]==Milestone)
            .Select(x=>(string?)x!["id"]??"").ToHashSet(StringComparer.Ordinal);
        if(required.Length!=7||!actual.SetEquals(required)||
           release["evidence"]!.AsArray().Count!=0)
            throw new InvalidOperationException("M7 functional gate or evidence baseline drifted.");
        doc["plan_version"]="0.1.122";
        doc["status"]="done";
        release["status"]="done";
        release["evidence"]=p.Json.StringArray(Advanced,Formal,Qualified);
    });
    p.ProjectPlan.ConvergeNodeToDone(Milestone);
}
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState(Milestone,"done");
p.ProjectPlan.RequireNodeState("M7-W01","done");
p.ProjectPlan.RequireNodeState("M7-W02","done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS123"||
       (string?)state["activeMilestone"]!=(target?null:Milestone)||
       state["activeWorkPackage"] is not null||
       state["activePhase"] is not null||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected M7 final internal gate DWF cursor.");
    state["activeMilestone"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var after=Required(root,"dotnet","run","--file",Validator,"--","--check");
if(!after.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("M7 target research/decision closure gate failed.");
return p.Complete();

static string Required(string root,string command,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=command,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS123 CSharp scientific gate command could not start.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(300000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS123 local scientific gate timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS123 internal M7 scope gate failed: "+
            command+" "+string.Join(" ",args)+"\n"+
            output[..Math.Min(output.Length,2400)]);
    return output;
}
