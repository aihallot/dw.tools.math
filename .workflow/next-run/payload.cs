using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Backlog="docs/planning/backlog.json";
const string PriorReport=".aura/workflow/reports/RS129/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json";
const string Readme="README.md";
const string Guide="docs/distribution/current-local-consumption.md";
const string Decision="docs/planning/decisions/post-m7-readme-reconciliation.md";
const string Validator="docs/planning/ValidatePostM7ReadmeStatus.cs";
const string Evidence="docs/planning/evidence/RS130-post-m7-readme-qualified.json";
const string Status="rs130-current-math-readme-and-immutable-historical-proof-reconciled-unpublished";
const string Counts="MATH-RS130:PACKAGES=13;REPORTS=5;CONSUMERS=7;PROBES=95;EXACT-SMOKE=PASS;NODES=541";
const string Scope="MATH-RS130:DOCS=CURRENT;LEGACY-ANCHOR=ARCHIVED;SOURCE-PINS=UNCHANGED;PUBLICATION=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs130-readme-reconciliation/1.0 qualified; current=2026-10-21; archive=2026-10-07; public=false";
const string Historical="Status on 2026-10-07: initial planning; no mathematical engine has been implemented in this repository yet.";
const string Current="As of 2026-10-21: implemented libraries and local delivery qualification completed; no public NuGet release.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS130",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS129",1,
    "docs: qualify post-M7 Math delivery evidence portfolio without publication",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(true));
p.Require.JsonValueEquals(PriorReceipt,"/status",
    JsonValue.Create("rs129-source-pinned-post-m7-local-delivery-portfolio-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/readme_current",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/verified_run_reports",JsonValue.Create(5));
p.Require.JsonValueEquals(PriorReceipt,"/freshly_packed_package_roster",JsonValue.Create(13));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_consumer_executions",JsonValue.Create(7));
p.Require.JsonValueEquals(PriorReceipt,"/numbered_consumer_probes",JsonValue.Create(95));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/discovery_skill_generated",JsonValue.Create(false));
p.Require.TextContains(Readme,Historical);
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
foreach(var node in new[]{"M5","M6","M7"})
    p.ProjectPlan.RequireNodeState(node,"done");

if(!target)
{
    foreach(var f in new[]{Guide,Decision,Validator,Evidence})
        p.Require.FileMissing(f);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence,"/source_projects",JsonValue.Create(13));
    p.Require.JsonValueEquals(Evidence,"/independent_consumer_executions",JsonValue.Create(7));
    p.Require.JsonValueEquals(Evidence,"/numbered_probes_historical",JsonValue.Create(95));
    p.Require.JsonValueEquals(Evidence,"/native_nodes_retained",JsonValue.Create(541));
    p.Require.JsonValueEquals(Evidence,"/approved_sequence",JsonValue.Create("A-B-D"));
    foreach(var (key,flag) in new (string,bool)[]
    {
        ("/readme_current",true),("/archived_baseline_preserved",true),
        ("/historical_source_pins_preserved",true),
        ("/exact_quantity_smoke_passed",true),
        ("/m0_m7_plan_mutated",false),
        ("/publicly_published",false),("/external_requests_transmitted",false),
        ("/external_approvals_received",false),("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),("/discovery_skill_generated",false),
        ("/future_program_approved",false)
    })
        p.Require.JsonValueEquals(Evidence,key,JsonValue.Create(flag));
}
var readmeChange=p.Files.ReplaceFromStaged("staged/"+Readme,Readme);
var guideChange=p.Files.ReplaceFromStaged("staged/"+Guide,Guide);
var decisionChange=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var validatorChange=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var readmeSha=readmeChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS130 current README identity.");
var guideSha=guideChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS130 local package guide identity.");
var decisionSha=decisionChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS130 documentation decision identity.");
var validatorSha=validatorChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS130 independent validator identity.");
p.Require.TextContains(Readme,Current);
p.Require.TextContains(Readme,Historical);
p.Require.TextContains(Readme,"## Historical baseline (2026-10-07)");
p.Require.TextContains(Guide,"## Verified package roster (13 packable projects)");
p.Require.TextContains(Guide,"## Verified evidence by run");
p.Require.TextContains(Decision,"## The exact historical-anchor problem and resolution");
foreach(var marker in new[]{Counts,Scope,Marker})
    p.Require.TextContains(Validator,marker);
var initial=Check(root,"readme-baseline");
foreach(var marker in new[]{Counts,Scope,Marker})
    if(!initial.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "RS130 independent README baseline missing marker: "+marker);
if(!target)
{
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS130-post-m7-readme-qualified",
        ["status"]=Status,
        ["question"]="Can Math present its current locally verified delivery without rewriting historical evidence or falsely implying public release, stable ABI or external adoption?",
        ["observation"]="The README now leads with the 2026-10-21 current local delivery status and links to a 13-package consumer guide. The 2026-10-07 initial-plan sentence appears exactly once, solely in a marked historical archive section. Source-pinned old checkers and evidence remain unchanged.",
        ["decision"]="Accept truthful updated README and package guidance, with historical source markers preserved and no product release or DWF progress movement. Next authorized sequence: B Discovery/AURA qualification, then D interproject coordination; actual outbound transmission remains separately owner gated.",
        ["readme"]=Readme,["readme_sha256"]=readmeSha,
        ["guide"]=Guide,["guide_sha256"]=guideSha,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["previous_portfolio_evidence"]=PriorReceipt,
        ["readme_current"]=true,
        ["archived_baseline_preserved"]=true,
        ["historical_source_pins_preserved"]=true,
        ["source_projects"]=13,
        ["independent_consumer_executions"]=7,
        ["numbered_probes_historical"]=95,
        ["exact_quantity_smoke_passed"]=true,
        ["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["approved_sequence"]="A-B-D",
        ["m0_m7_plan_mutated"]=false,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["discovery_skill_generated"]=false,
        ["future_program_approved"]=false,
        ["next_scope"]="After verified RS130 first qualify broker-neutral Math Discovery protocols, actual agent callable functions and scoped CLR binding without fake native progress. Only then prepare owner-approved outbound coordination to AURA/Decision/MCDM. No publication.",
        ["nonclaims"]=p.Json.StringArray(
            "This run reconciles only documentation; it does not freshly build, package or rerun Math product consumers.",
            "The 2026-10-07 status remains an explicitly archived historical baseline, never the 2026-10-21 active product claim.",
            "The RS124-RS129 first-pass and prior source SHA records were not rewritten to accommodate this README update.",
            "The 13 local preview packages, 7 consumer executions and 95 probes do not imply public release or universal API/ABI support.",
            "No dw.discovery skill, AURA/Decision/MCDM adoption, new M8 milestone, provider or cross-repository change was delivered.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var final=Check(root,"readme-target");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS130 independent README target refused.");
return p.Complete();

static string Check(string root,string name)
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
    foreach(var argument in new[]{"run","--file",
        "docs/planning/ValidatePostM7ReadmeStatus.cs",
        "--","--check"})
        process.StartInfo.ArgumentList.Add(argument);
    if(!process.Start())
        throw new InvalidOperationException(
            "Cannot run independent README verifier: "+name);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS130 independent README verifier timed out: "+name);
    }
    var result=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS130 "+name+
            " failed exit="+process.ExitCode+"\n"+
            result[..Math.Min(2600,result.Length)]);
    Console.WriteLine("RS130 "+name+" exit=0");
    return result;
}
