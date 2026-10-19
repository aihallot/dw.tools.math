using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M6", State=".aura/workflow/state.json";
const string Backlog="docs/planning/backlog.json";
const string Gate="docs/planning/evidence/M6-gate.json";
const string Validator="docs/planning/ValidateM6ReleaseGate.cs";
const string Decision="docs/planning/decisions/m6-gate.md";
const string Recipe="docs/distribution/m6-local-release-gate.md";
const string Numerical="docs/planning/evidence/M6-W01-local-gate-qualified.json";
const string Scientific="docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json";
const string Last="docs/planning/evidence/M6-W02-C05-T2-qualified.json";
const string Status="m6-independent-local-analysis-scientific-release-gate-qualified-unpublished";
const string Marker="dw.tools.math/m6-local-science-gate/1.1 qualified; phases=11; tasks=22; subtasks=66; consumers=10; public=false";
const string AnalysisSha="facb88dcf029de1445122eef38a147b609b89d1c5a36567506b1c10c2ce423ac";
const string ScientificSha="2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf";
const long AnalysisBytes=27820, ScientificBytes=37270;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Gate));
p.Workflow.RequireCurrentRun("RS105",target?null:Milestone,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS104",1,
    "docs: qualify M6 scientific adoption boundaries and close W02",
    "0.1.59",0,0,0,true);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.103"),JsonValue.Create("0.1.104"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.104":"0.1.103"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("in_progress"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/id",Milestone),
    ("/releases/6/version","1.1.0"),("/releases/6/gate","G-M6"),
    ("/releases/6/status",target?"done":"in_progress"),
    ("/releases/7/status","planned"),
    ("/work_packages/13/id","M6-W01"),("/work_packages/13/status","done"),
    ("/work_packages/14/id","M6-W02"),("/work_packages/14/status","done")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState("M6-W02","done");
p.ProjectPlan.RequireNodeState("M7","not-ready");

var phases=new (string Id,string Status)[]
{
    ("M6-W01-C01","bounded-curve-explicit-unit-conditioning-extrapolation-refusals-qualified"),
    ("M6-W01-C02","bounded-univariate-residual-and-regularity-audit-qualified"),
    ("M6-W01-C03","bounded-differential-declared-regularity-and-ode-regime-audit-qualified"),
    ("M6-W02-C01","bounded-exact-signed-and-intermediate-bit-audit-local-nuget-qualified"),
    ("M6-W02-C02","bounded-managed-special-angle-gamma-branch-risk-audit-qualified"),
    ("M6-W02-C03","bounded-cross-family-scientific-boundary-integration-local-nuget-qualified"),
    ("M6-W02-C03-S01","bounded-finite-euclidean-2d-3d-resolution-and-projection-risk-local-nuget-qualified"),
    ("M6-W02-C03-S02","bounded-direct-dft-explicit-sampling-linear-topology-cancellation-local-nuget-qualified"),
    ("M6-W02-C03-S03","bounded-uniform-grid-interval-and-numeric-risk-local-nuget-qualified"),
    ("M6-W02-C04","bounded-actual-day-finance-root-topology-local-nuget-qualified"),
    ("M6-W02-C05","owner-gated-scientific-adoption-no-cutover-and-notices-qualified")
};
if(phases.Length!=11 || phases.Select(x=>x.Id).Distinct(StringComparer.Ordinal).Count()!=11)
    throw new InvalidOperationException("The accepted M6 release must contain exactly eleven unique phases.");
var proofPaths=new List<string>();
var proofSha=new List<string>();
foreach(var phase in phases)
{
    var proof="docs/planning/evidence/"+phase.Id+"-T2-"+
        (phase.Id=="M6-W02-C05"?"qualified":"boundaries-qualified")+".json";
    p.ProjectPlan.RequireNodeState(phase.Id,"done");
    var record=p.Require.JsonValueEquals(proof,"/status",JsonValue.Create(phase.Status));
    p.Require.JsonValueEquals(proof,"/m6_release_done",JsonValue.Create(false));
    var hash=record.AfterSha256 ??
        throw new InvalidOperationException("Missing DWF bounded evidence hash: "+proof);
    proofPaths.Add(proof);
    proofSha.Add("M6-GATE-PHASE:"+phase.Id+":"+hash);
}
p.Require.JsonValueEquals(Numerical,"/status",
    JsonValue.Create("m6-w01-all-three-numerical-families-local-nuget-qualified"));
p.Require.JsonValueEquals(Numerical,"/local_product_version",JsonValue.Create("1.1.0-preview.6"));
p.Require.JsonValueEquals(Numerical,"/actual_inspected_package_sha256",
    JsonValue.Create(AnalysisSha));
p.Require.JsonValueEquals(Numerical,"/actual_inspected_package_bytes",
    JsonValue.Create(AnalysisBytes));
p.Require.JsonValueEquals(Numerical,"/source_free_consumer_count",JsonValue.Create(3));
p.Require.JsonValueEquals(Numerical,"/independent_consumer_probes_passed",JsonValue.Create(24));
p.Require.JsonValueEquals(Numerical,"/m6_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Numerical,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Scientific,"/local_product_version",JsonValue.Create("1.1.0-preview.14"));
p.Require.JsonValueEquals(Scientific,"/actual_inspected_package_sha256",
    JsonValue.Create(ScientificSha));
p.Require.JsonValueEquals(Scientific,"/actual_inspected_package_bytes",
    JsonValue.Create(ScientificBytes));
p.Require.JsonValueEquals(Scientific,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/status",
    JsonValue.Create("owner-gated-scientific-adoption-no-cutover-and-notices-qualified"));
p.Require.JsonValueEquals(Last,"/m6_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Last,"/owner_approval_obtained",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/external_cutover_performed",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Last,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(
    "docs/planning/releases/m5-v1-publication-boundary.json",
    "/owner/stable_v1_publication_authorized",JsonValue.Create(false));
p.Require.JsonValueEquals(
    "docs/planning/releases/m5-v1-publication-boundary.json",
    "/publication/public_nuget_published",JsonValue.Create(false));
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.Require.TextContains("src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj",
    "<Version>1.1.0-preview.6</Version>");
p.Require.TextContains("src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj",
    "<Version>1.1.0-preview.14</Version>");
if(!target)
{
    foreach(var file in new[]{Validator,Decision,Recipe,Gate})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Gate,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Gate,"/gate",JsonValue.Create("G-M6"));
    p.Require.JsonValueEquals(Gate,"/m6_milestone_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Gate,"/qualified_phases",JsonValue.Create(11));
    p.Require.JsonValueEquals(Gate,"/qualified_tasks",JsonValue.Create(22));
    p.Require.JsonValueEquals(Gate,"/qualified_subtasks",JsonValue.Create(66));
    p.Require.JsonValueEquals(Gate,"/packages_publicly_published",JsonValue.Create(false));
    p.Require.JsonValueEquals(Gate,"/stable_release_certified",JsonValue.Create(false));
}
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var recipeWrite=p.Files.ReplaceFromStaged("staged/"+Recipe,Recipe);
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing gate validator SHA-256.");
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing gate decision SHA-256.");
var recipeSha=recipeWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing gate recipe SHA-256.");
p.Require.TextContains(Validator,"M6-GATE-PHASE:");
p.Require.TextContains(Decision,"no public NuGet",StringComparison.OrdinalIgnoreCase);
p.Require.TextContains(Recipe,"G-M6");
if(!target)
{
    Required(root,540000,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,720000,"dotnet","test","dw.tools.math.slnx",
        "--configuration","Release","--no-restore");
    var baselineCheck=Required(root,180000,"dotnet","run","--file",Validator,"--","--check");
    if(!baselineCheck.Contains(Marker,StringComparison.Ordinal))
        throw new InvalidOperationException("The independent G-M6 pre-transition gate marker was not observed.");
    var manifest=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-gate",["status"]=Status,
        ["milestone"]=Milestone,["gate"]="G-M6",
        ["release_version"]="1.1.0-local-evidence-only",
        ["plan_version"]="0.1.104",
        ["question"]="Do eleven admitted finite-domain M6 phases and independent package receipts support a local scientific milestone without public release?",
        ["observation"]="All eleven phases and their 22 tasks and 66 subtasks are done. Separate historical analysis and scientific NuGet receipts retain ten source-free eight-probe consumers. A fresh full solution test and the independent local-science gate succeed.",
        ["decision"]="Qualify the locally bounded M6 scientific milestone only; no stable/public release, foreign provider, Decision adoption or license change is authorized.",
        ["qualified_phases"]=11,["qualified_tasks"]=22,["qualified_subtasks"]=66,
        ["qualified_phase_evidence"]=p.Json.StringArray(proofPaths.ToArray()),
        ["qualified_phase_evidence_sha256"]=p.Json.StringArray(proofSha.ToArray()),
        ["numerical_work_package_evidence"]=Numerical,
        ["scientific_work_package_evidence"]=Last,
        ["scientific_latest_package_evidence"]=Scientific,
        ["analysis_historical_package_version"]="1.1.0-preview.6",
        ["analysis_historical_archive_sha256"]=AnalysisSha,
        ["analysis_historical_archive_bytes"]=AnalysisBytes,
        ["scientific_historical_package_version"]="1.1.0-preview.14",
        ["scientific_historical_archive_sha256"]=ScientificSha,
        ["scientific_historical_archive_bytes"]=ScientificBytes,
        ["historical_source_free_consumers"]=10,
        ["historical_source_free_probes"]=80,
        ["new_nuget_archives_built"]=false,
        ["historical_receipts_not_rerun"]=true,
        ["locked_solution_restore_passed"]=true,
        ["full_solution_tests_passed"]=true,
        ["independent_read_only_gate_passed"]=true,
        ["gate_validator"]=Validator,["gate_validator_sha256"]=validatorSha,
        ["decision_document"]=Decision,["decision_document_sha256"]=decisionSha,
        ["release_recipe"]=Recipe,["release_recipe_sha256"]=recipeSha,
        ["m6_milestone_done"]=true,
        ["packages_publicly_published"]=false,
        ["stable_release_certified"]=false,
        ["owner_stable_publication_approved"]=false,
        ["foreign_provider_adopted"]=false,
        ["external_cutover_performed"]=false,
        ["decision_owner_approval_obtained"]=false,
        ["other_os_rids_certified"]=false,
        ["cross_version_abi_certified"]=false,
        ["dotnet_sdk"]=Required(root,30000,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M7 advanced extension admission and independent oracles require owner planning decision; no automatic external cutover or stable release.",
        ["nonclaims"]=p.Json.StringArray(
            "Milestone M6 1.1.0 is local acceptance, not a compiled, signed or published stable NuGet 1.1.0 product.",
            "Historical source-free NuGet archives and ten consumers were not rebuilt or rerun during this release gate.",
            "No new owner publication/license permission, Decision/AURA/MCDM acceptance or sibling repository mutation.",
            "No additional OS/ABI certification, general FFT/interval/tensor/PDE capability or certified error theorem.")
    };
    p.Files.WriteComplete(Gate,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        if((string?)doc["plan_version"]!="0.1.103")
            throw new InvalidOperationException("Unexpected G-M6 pre-gate backlog version.");
        var release=doc["releases"]!.AsArray()[6]!.AsObject();
        if((string?)release["id"]!=Milestone ||
            (string?)release["status"]!="in_progress" ||
            (string?)release["gate"]!="G-M6" ||
            (string?)doc["work_packages"]!.AsArray()[13]!["status"]!="done" ||
            (string?)doc["work_packages"]!.AsArray()[14]!["status"]!="done")
            throw new InvalidOperationException("Unqualified M6 release baseline.");
        doc["plan_version"]="0.1.104";
        release["status"]="done";
        var acceptedEvidence=release["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>())
            .Concat(new[]{Gate}).Distinct(StringComparer.Ordinal).ToArray();
        release["evidence"]=p.Json.StringArray(acceptedEvidence);
    });
    p.ProjectPlan.ConvergeNodeToDone(Milestone);
}
p.ProjectPlan.RequireNodeState(Milestone,"done");
p.ProjectPlan.RequireNodeState("M7","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS105" ||
        (string?)state["activeMilestone"]!=(target?null:Milestone) ||
        state["activeWorkPackage"] is not null ||
        state["activePhase"] is not null ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected G-M6 release-milestone cursor.");
    state["activeMilestone"]=null;
});
if(!target)
    Required(root,240000,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,180000,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,180000,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
var finalGate=Required(root,180000,"dotnet","run","--file",Validator,"--","--check");
if(!finalGate.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent G-M6 completed-target gate marker absent.");
return p.Complete();

static string Required(string root,int timeoutMilliseconds,string executable,params string[] args)
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
        throw new InvalidOperationException("G-M6 gate process did not start: "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(timeoutMilliseconds))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("Independent G-M6 command timed out: "+executable);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output.Length>6000?output[^6000..]:output);
    if(process.ExitCode!=0)
        throw new InvalidOperationException("G-M6 command failed: "+executable+
            " "+string.Join(" ",args)+"\n"+output[..Math.Min(output.Length,2500)]);
    return output;
}
