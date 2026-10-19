using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M6", Work="M6-W02", Phase="M6-W02-C05", Task="M6-W02-C05-T1";
const string Question="M6-W02-C05-T1-A", Observation="M6-W02-C05-T1-B", DecisionNode="M6-W02-C05-T1-C";
const string State=".aura/workflow/state.json", Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json";
const string Decision="docs/planning/decisions/m6-w02-c05.md";
const string Catalog="docs/distribution/m6-scientific-family-adoption.md";
const string Request="docs/planning/requests/decision-bounded-bracketed-root-assessment.md";
const string Qualified="docs/planning/evidence/M6-W02-C05-T1-qualified.json";
const string Status="seven-scientific-family-contract-pins-and-owner-gated-reuse-request-documented";
const string LatestVersion="1.1.0-preview.14";
const string LatestArchiveSha="2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf";
const long LatestArchiveBytes=37270;
const int QualifiedFamilies=7, PriorSourceFreeClients=7;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS103",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS102",1,
    "feat: qualify dated financial boundaries and close M6 W02 C04",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-actual-day-finance-root-topology-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create(LatestVersion));
p.Require.JsonValueEquals(Prior,"/actual_inspected_package_sha256",
    JsonValue.Create(LatestArchiveSha));
p.Require.JsonValueEquals(Prior,"/actual_inspected_package_bytes",
    JsonValue.Create(LatestArchiveBytes));
p.Require.JsonValueEquals(Prior,"/m6_w02_c04_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.101"),JsonValue.Create("0.1.102"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.102":"0.1.101"));

foreach(var(pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","done"),("/work_packages/14/status","in_progress"),
    ("/chunks/43/status","done"),("/chunks/44/status","done"),
    ("/chunks/45/status","done"),("/chunks/49/status","done"),
    ("/chunks/50/id",Phase),("/chunks/50/kind","document"),
    ("/chunks/50/status",target?"in_progress":"planned"),
    ("/chunks/50/tasks/0/id",Task),
    ("/chunks/50/tasks/0/status",target?"done":"planned"),
    ("/chunks/50/tasks/1/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var index=0;index<3;index++)
    p.Require.JsonValueEquals(Backlog,
        "/chunks/50/tasks/0/subtasks/"+index+"/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
foreach(var id in new[]{
    "M6-W02-C01","M6-W02-C02","M6-W02-C03","M6-W02-C04"})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C05-T2","not-ready");
foreach(var id in new[]{
    "M6-W02-C05-T2-A","M6-W02-C05-T2-B","M6-W02-C05-T2-C"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");

if(!target)
{
    foreach(var file in new[]{Decision,Catalog,Request,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Qualified,"/qualified_family_count",
        JsonValue.Create(QualifiedFamilies));
    p.Require.JsonValueEquals(Qualified,"/historical_receipts_preserved",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/new_scientific_tests_executed",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/decision_sibling_modified",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_c05_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_c05_parent_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
}

var pins=new FamilyPin[]
{
    new("C01 exact scientific integers",
        "docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json",
        "bounded-exact-signed-and-intermediate-bit-audit-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs",
        "d3dd0a01a76fe42c9d38a35dca4cdc18134c56c4728762fdbaa2adb41708e9db",
        "1.1.0-preview.2","Binomial(5,2)=10"),
    new("C02 managed special functions",
        "docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json",
        "bounded-managed-special-angle-gamma-branch-risk-audit-qualified",
        "src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs",
        "f9d8067dfcb1112ff997c15529fb3c866f2aba1b70b7c7eff064dc3433ecf467",
        "1.1.0-preview.4","Gamma(5)=24"),
    new("S01 finite Euclidean geometry",
        "docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json",
        "bounded-finite-euclidean-2d-3d-resolution-and-projection-risk-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/FiniteGeometryBoundaryAudit64.cs",
        "7355c99cb2bdcb72a3cdb2880136e8bd4643645ee578608582fbc76bf68e1fa6",
        "1.1.0-preview.6","Norm(3,4)=5"),
    new("S02 direct signals",
        "docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json",
        "bounded-direct-dft-explicit-sampling-linear-topology-cancellation-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs",
        "da135763f464d65fc5f71d1bb09689259ea480ac26bde4b976db5ac6ee450ce3",
        "1.1.0-preview.8","DFT([1,0,0,0])=[1,1,1,1]"),
    new("S03 finite uniform series",
        "docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json",
        "bounded-uniform-grid-interval-and-numeric-risk-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs",
        "908f18668b23ea788e5f1be531bf8a3bb6108c048a1e663f6c1927bdd9ab5d10",
        "1.1.0-preview.10","difference([1,3,6])=[2,3]"),
    new("C03 cross-family aggregation",
        "docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json",
        "bounded-cross-family-scientific-boundary-integration-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/M6W02C03BoundaryAggregate64.cs",
        "634f5e701d92c9820d910b9a97f898c95c14e6abeb16ac6a3be3347d9d8b00e4",
        "1.1.0-preview.12","Seven independently admitted"),
    new("C04 pure financial arithmetic",
        Prior,"bounded-actual-day-finance-root-topology-local-nuget-qualified",
        "src/projects/dw.tools.math.scientific/M6W02C04Boundary64.cs",
        "df2ce1460020743df127c19377e99b85b65a4e1fd57104db54bf833ea3357b60",
        LatestVersion,"1000/11")
};
if(pins.Length!=QualifiedFamilies ||
    pins.Select(x=>x.Evidence).Distinct(StringComparer.Ordinal).Count()!=QualifiedFamilies ||
    pins.Select(x=>x.Source).Distinct(StringComparer.Ordinal).Count()!=QualifiedFamilies)
    throw new InvalidOperationException("Exactly seven unique independent qualified source pins required.");
foreach(var family in pins)
{
    p.Require.JsonValueEquals(family.Evidence,"/status",JsonValue.Create(family.Status));
    p.Require.JsonValueEquals(family.Evidence,"/source",JsonValue.Create(family.Source));
    p.Require.JsonValueEquals(family.Evidence,"/source_sha256",
        JsonValue.Create(family.SourceSha256));
    p.Require.JsonValueEquals(family.Evidence,"/local_product_version",
        JsonValue.Create(family.Version));
    if(!File.Exists(Path.Combine(root,family.Source)))
        throw new InvalidOperationException("Missing accepted source path: "+family.Source);
}
var priorReceipt=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))?.AsObject()
    ?? throw new InvalidOperationException("Durable RS102 receipt is not JSON.");
var clients=priorReceipt["isolated_consumer_receipts"]?.AsArray()
    ?? throw new InvalidOperationException("Missing historical consumer receipts.");
if(clients.Count!=PriorSourceFreeClients)
    throw new InvalidOperationException("RS102 archive must have seven isolated consumer receipts.");
foreach(var entry in clients)
{
    var client=entry?.AsObject()
        ?? throw new InvalidOperationException("Null historical consumer receipt.");
    if((string?)client["package_sha256"]!=LatestArchiveSha ||
        (long?)client["package_bytes"]!=LatestArchiveBytes ||
        (int?)client["independent_probes"]!=8 ||
        (int?)client["project_references"]!=0)
        throw new InvalidOperationException("Historical source-free package SHA/probe receipt drifted.");
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Decision,Catalog,Request})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256
        ?? throw new InvalidOperationException("Missing staged documentary source SHA: "+file);
}
var matrix=File.ReadAllText(Path.Combine(root,Catalog));
foreach(var family in pins)
{
    if(!matrix.Contains(family.Evidence,StringComparison.Ordinal) ||
        !matrix.Contains(family.SourceSha256,StringComparison.Ordinal) ||
        !matrix.Contains(family.Version,StringComparison.Ordinal))
        throw new InvalidOperationException("Family lacks documentary evidence/source/provider pin: "+family.Name);
}
foreach(var snippet in new[]{
    "Binomial(5,2)=10","Gamma(5)=24","Norm(3,4)=5",
    "DFT([1,0,0,0])=[1,1,1,1]","difference([1,3,6])=[2,3]",
    "1000/11","Seven independently admitted"})
    if(!matrix.Contains(snippet,StringComparison.Ordinal))
        throw new InvalidOperationException("Admitted family oracle missing in catalog: "+snippet);
var decisionText=File.ReadAllText(Path.Combine(root,Decision));
var reuseText=File.ReadAllText(Path.Combine(root,Request));
if(!decisionText.Contains("Question",StringComparison.Ordinal) ||
    !decisionText.Contains("Observation",StringComparison.Ordinal) ||
    !decisionText.Contains("Decision",StringComparison.Ordinal) ||
    !reuseText.Contains("f(x)=x-0.1",StringComparison.Ordinal) ||
    !reuseText.Contains("f(x)=x*x-4",StringComparison.Ordinal) ||
    !reuseText.Contains("not a confirmed duplicate",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Missing exact document Q/O/D or owner-gated root assessment criteria.");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
if(!target)
{
    var admitted=new JsonArray();
    foreach(var family in pins)
        admitted.Add(new JsonObject
        {
            ["family"]=family.Name,["evidence"]=family.Evidence,
            ["source"]=family.Source,["source_sha256"]=family.SourceSha256,
            ["historical_provider_version"]=family.Version,["oracle"]=family.Oracle
        });
    var manifest=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C05-T1-qualified",
        ["status"]=Status,
        ["question"]="Which scientific families have independent oracle/limit/provider-pin evidence, and which exact primitive should Decision review before any owner-approved reuse?",
        ["observation"]="Seven durable family qualifications and a separate RS102 local preview.14 receipt with seven SHA-equal eight-probe clients; no new scientific test or external adoption.",
        ["decision"]="Admit seven version-pinned documentary family contracts and one bounded scalar bracketed-root owner review request, without confirming duplication or triggering a cutover.",
        ["prior_scientific_qualification"]=Prior,
        ["prior_package_version"]=LatestVersion,
        ["prior_package_sha256"]=LatestArchiveSha,
        ["prior_package_bytes"]=LatestArchiveBytes,
        ["historical_receipts_preserved"]=true,
        ["qualified_family_count"]=QualifiedFamilies,
        ["qualified_families"]=admitted,
        ["previous_source_free_consumer_count"]=PriorSourceFreeClients,
        ["documentation"]=Decision,
        ["documentation_sha256"]=hashes[Decision],
        ["adoption_catalog"]=Catalog,
        ["adoption_catalog_sha256"]=hashes[Catalog],
        ["decision_assessment_request"]=Request,
        ["decision_assessment_request_sha256"]=hashes[Request],
        ["decision_request_primitive"]="bounded finite scalar bracketed bisection",
        ["decision_duplicate_verified"]=false,
        ["decision_owner_approval_obtained"]=false,
        ["decision_sibling_modified"]=false,
        ["new_scientific_tests_executed"]=false,
        ["new_scientific_package_built"]=false,
        ["publicly_published"]=false,
        ["m6_w02_c05_t1_done"]=true,
        ["m6_w02_c05_t2_done"]=false,
        ["m6_w02_c05_parent_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C05-T2: separately qualify optional adoption boundaries, semver/notices, owner disposition and no automatic external cutover before any work-package or milestone closure.",
        ["nonclaims"]=p.Json.StringArray(
            "Historical family SHA pins are evidence records; downstream clients must independently check exact source/runtime/provider bytes.",
            "No new scientific source, test suite, numerical guarantee or NuGet package is delivered by this document task.",
            "No Decision, AURA, MCDM, dw.discovery or other sibling repo mutation or runtime integration is authorized.",
            "A proposed reusable bisection primitive is not proof that Decision has a duplicate or needs that interface.",
            "C05-T2, C05 parent, work package M6-W02 and milestone M6 remain incomplete.")
    };
    p.Files.WriteComplete(Qualified,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        doc["plan_version"]="0.1.102";
        var chunk=doc["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="planned" ||
            (string?)chunk["kind"]!="document" ||
            (string?)task["status"]!="planned" ||
            (string?)chunk["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("Documentary adoption T1 baseline moved.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS102 completed all scientific C01/C02/C03/C04 families. RS103 T1 admits seven independent historical source/evidence/preview pins, each with oracle and bounded domain, and publishes one owner-gated scalar-bisection Decision assessment. No package build, new scientific tests, sibling edit or cutover. C05-T2 and M6 release gates remain open.";
        chunk["readiness"]="C05 T1 documentation qualified on seven family receipts with pinned source identities, source-free consumer evidence, exact independent oracles, a Decision owner-review request and explicit non-adoption limits; C05 T2 semver/notices/owner disposition is planned.";
        var existingFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(existingFiles.Concat(new[]{
            Decision,Catalog,Request,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check",
            "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
            "dotnet run --file docs/planning/ValidateTransferArchitecture.cs");
        var existingEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(existingEvidence.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            if((string?)sub["status"]!="planned")
                throw new InvalidOperationException("Documentary Q/O/D subtask not initially planned.");
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(Qualified);
        }
    });
    foreach(var id in new[]{Phase,Task,Question})
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
    foreach(var id in new[]{Phase,Task,Question})
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var id in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M6-W02-C05-T2","not-ready");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS103" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?Phase:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS103 documentary Q/O/D cursor.");
    state["activePhase"]=Phase;
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
        throw new TimeoutException("RS103 planning documentary validator timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("DWF documentary validation failed: "+output[..Math.Min(output.Length,2000)]);
    return output;
}
sealed record FamilyPin(string Name,string Evidence,string Status,
    string Source,string SourceSha256,string Version,string Oracle);
