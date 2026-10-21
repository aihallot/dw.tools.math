using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Backlog="docs/planning/backlog.json";
const string PriorReport=".aura/workflow/reports/RS128/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json";
const string Decision="docs/planning/decisions/post-m7-delivery-portfolio.md";
const string Matrix="docs/distribution/post-m7-delivery-portfolio-matrix.md";
const string Validator="docs/planning/ValidatePostM7DeliveryPortfolio.cs";
const string Evidence="docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json";
const string Status="rs129-source-pinned-post-m7-local-delivery-portfolio-qualified-unpublished";
const string Counts="MATH-RS129:RUNS=5;PACKABLE=13;CONSUMER-EXECUTIONS=7;NUMBERED-PROBES=95;EXACT-SMOKE=PASS;NATIVE=541";
const string Scope="MATH-RS129:PORTFOLIO=EVIDENCE-ONLY;FIRST-PASS=3-YES-2-NO;PUBLICATION=false;ABI=false;EXTERNAL=UNSENT";
const string Marker="dw.tools.math/rs129-delivery-portfolio/1.0 qualified; reports=5; packages=13; clients=7; probes=95; publication=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS129",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS128",1,
    "docs: qualify fresh offline Math symbolics and codec consumers",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(true));
p.Require.JsonValueEquals(PriorReceipt,"/status",JsonValue.Create(
    "rs128-four-local-symbolic-codec-packages-two-source-free-consumers-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_consumer_probes",JsonValue.Create(40));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/cross_version_abi_certified",JsonValue.Create(false));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("README.md","Status on 2026-10-07: initial planning");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
p.ProjectPlan.RequireNodeState("M7","done");
if(!target)
{
    foreach(var destination in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(destination);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (field,count) in new (string,int)[]
    {
        ("/verified_run_reports",5),
        ("/source_projects",13),("/test_projects",11),
        ("/historical_consumer_directories",47),
        ("/freshly_packed_package_roster",13),
        ("/fresh_consumer_executions",7),
        ("/numbered_consumer_probes",95),
        ("/run_first_pass_yes",3),("/run_first_pass_no",2),
        ("/preparation_refusals_total",3),
        ("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,field,JsonValue.Create(count));
    foreach(var (field,value) in new (string,bool)[]
    {
        ("/exact_quantity_smoke_passed",true),
        ("/readme_current",false),
        ("/m0_m7_plan_mutated",false),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),
        ("/discovery_skill_generated",false),
        ("/future_program_approved",false),
        ("/new_public_product_delivered",false)
    })
        p.Require.JsonValueEquals(Evidence,field,JsonValue.Create(value));
}
var decisionChange=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixChange=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorChange=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionChange.AfterSha256 ??
    throw new InvalidOperationException("Missing source identity for portfolio decision.");
var matrixSha=matrixChange.AfterSha256 ??
    throw new InvalidOperationException("Missing source identity for portfolio matrix.");
var validatorSha=validatorChange.AfterSha256 ??
    throw new InvalidOperationException("Missing source identity for independent portfolio checker.");
foreach(var term in new[]{
    "## Scope and decision","## Qualified observed evidence",
    "## Read-only acceptance and provenance","## Future gates",
    "## Engineering constraints"
})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{
    "## Five qualified local runs","## Real delivery categories",
    "## No-progress DWF portfolio gate"
})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{Counts,Scope,Marker})
    p.Require.TextContains(Validator,term);
var initial=Check(root,"portfolio-baseline");
foreach(var marker in new[]{Counts,Scope,Marker})
    if(!initial.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Portfolio baseline marker missing: "+marker);
if(!target)
{
    var lineage=p.Json.StringArray(
        "docs/planning/evidence/RS124-post-m7-delivery-qualified.json",
        "docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json",
        "docs/planning/evidence/RS126-post-m7-distribution-qualified.json",
        "docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json",
        "docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS129-post-m7-delivery-portfolio-qualified",
        ["status"]=Status,
        ["question"]="What exactly is locally qualified for dw.tools.math under owner-selected track A after M7, and which future release, ABI, consumer, Discovery and external actions still require authorization?",
        ["observation"]="Five successful DWF runs RS124-RS128 establish thirteen packable local NuGet archives, seven observed source-free consumer executions across four runs, ninety-five numbered boundary probes and a separate unnumbered exact-quantity smoke; all observations are win-x64, SDK 10.0.402 and unpublished.",
        ["decision"]="Qualify the bounded source-pinned post-M7 local evidence portfolio; preserve historical first-pass truth, reject API/ABI and cross-RID overclaims, retain unsent recipient requests, deferred M7 providers, candidate Discovery and stale initial-plan README. Stop for the owner's next gate decision.",
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["verified_run_reports"]=5,
        ["source_projects"]=13,["test_projects"]=11,
        ["historical_consumer_directories"]=47,
        ["freshly_packed_package_roster"]=13,
        ["fresh_consumer_executions"]=7,
        ["numbered_consumer_probes"]=95,
        ["exact_quantity_smoke_passed"]=true,
        ["run_first_pass_yes"]=3,["run_first_pass_no"]=2,
        ["preparation_refusals_total"]=3,
        ["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["readme_current"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["publicly_published"]=false,
        ["public_release_authorized"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["discovery_skill_generated"]=false,
        ["future_program_approved"]=false,
        ["new_public_product_delivered"]=false,
        ["lineage_receipts"]=lineage,
        ["next_scope"]="Ask owner to authorize one next gate: reproducible public release/readiness, genuine pinned cross-version ABI comparison, recipient coordination, Discovery integration, documentation remediation, or new supported consumer-led roadmap. No successor selected automatically.",
        ["nonclaims"]=p.Json.StringArray(
            "The five successful receipts are individual local observations, not a newly rerun full end-to-end test in RS129 or a uniform version compatibility claim.",
            "Seven consumer executions and ninety-five probes are not complete Math API coverage; the exact quantity smoke is not counted as numbered probes.",
            "Thirteen packable local archives do not imply public NuGet publication, verified rights, source/binary ABI compatibility or multi-OS support.",
            "M7 advanced/formal providers remain deferred; AURA/Decision/MCDM requests are unsent, and Math Discovery skill is not generated.",
            "The original 541-node accepted M0-M7 roadmap remains done; the stale README is documented but not modified in this run.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var verified=Check(root,"portfolio-target");
if(!verified.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent target portfolio verifier failed.");
return p.Complete();

static string Check(string root,string label)
{
    using var process=new Process
    {
        StartInfo=new ProcessStartInfo
        {
            FileName="dotnet",WorkingDirectory=root,
            UseShellExecute=false,RedirectStandardOutput=true,
            RedirectStandardError=true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in new[]{"run","--file",
        "docs/planning/ValidatePostM7DeliveryPortfolio.cs",
        "--","--check"})
        process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("Could not run RS129 read-only verifier: "+label);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS129 independent checker timed out: "+label);
    }
    var result=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS129 "+label+
            " verifier rejected source/receipt; exit="+process.ExitCode+"\n"+
            result[..Math.Min(2500,result.Length)]);
    Console.WriteLine("RS129 "+label+" exit=0");
    return result;
}
