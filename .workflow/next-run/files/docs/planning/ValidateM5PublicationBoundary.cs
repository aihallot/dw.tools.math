#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Contract="docs/planning/releases/m5-v1-publication-boundary.json";
const string T1Evidence="docs/planning/evidence/M5-W02-C03-T1-local-candidate-qualified.json";
const string T1Map="docs/planning/releases/m5-v1-local-candidate-map.json";
const string OwnerRights="docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json";
const string PackageProof="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string Recipients="docs/coordination/requests/MATH-XR-004-local-v1-consumer-adoption-draft.md";
const string OwnerDraft="docs/coordination/requests/MATH-XR-005-v1-owner-publication-decision-unsent.md";
const string Migration="docs/distribution/m5-v1-migration-and-rollback.md";
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Marker="dw.tools.math/m5-local-publication-boundary/1.0 qualified; oracles=12; recipients=3; public=false";

static void Require(bool ok,string reason)
{
    if(!ok)throw new InvalidOperationException(
        "M5 local publication boundary: "+reason);
}
static string Text(JsonElement item,string key) =>
    item.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing textual authority: "+key);
static bool Flag(JsonElement item,string key) =>
    item.GetProperty(key).GetBoolean();
static JsonElement Find(JsonElement items,string id) =>
    items.EnumerateArray().Single(x=>Text(x,"id")==id);
static string ReadText(string relative)
{
    Require(relative.StartsWith("docs/",StringComparison.Ordinal) &&
        !Path.IsPathRooted(relative) &&
        !relative.Contains("..",StringComparison.Ordinal) &&
        !relative.Contains('\\'),
        "Only local bounded docs may be read: "+relative);
    return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar)));
}
static JsonDocument Read(string relative) =>
    JsonDocument.Parse(ReadText(relative));
static string Digest(string relative) =>
    Convert.ToHexString(SHA256.HashData(
        File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(),
            relative.Replace('/',Path.DirectorySeparatorChar)))))
        .ToLowerInvariant();
static void State(JsonElement item,string expected,string field="status") =>
    Require(Text(item,field)==expected,
        "Unexpected lifecycle "+field+" for "+Text(item,"id"));
var passed=0;
void Probe(string label,Action check)
{
    try{check();passed++;Console.WriteLine("M5-BOUNDARY-ORACLE:"+label);}
    catch(Exception e)
    {
        throw new InvalidOperationException(
            "M5 independent release-boundary falsification: "+label,e);
    }
}
Require(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
using var contractDoc=Read(Contract);
using var t1Doc=Read(T1Evidence);
using var mapDoc=Read(T1Map);
using var ownerDoc=Read(OwnerRights);
using var packageDoc=Read(PackageProof);
using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
var contract=contractDoc.RootElement;
var t1=t1Doc.RootElement;
var map=mapDoc.RootElement;
var owner=ownerDoc.RootElement;
var package=packageDoc.RootElement;
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var version=Text(backlog,"plan_version");
var isBaseline=version=="0.1.77";
var isTarget=version=="0.1.78";
Require(isBaseline||isTarget,"Only C03-T2 exact accepted lifecycle allowed.");

Probe("candidate-identity",()=>
{
    Require(contract.GetProperty("schema_version").GetInt32()==1 &&
        Text(contract,"id")=="M5-W02-C03-T2-publication-and-external-adoption-boundaries" &&
        Text(contract,"status")=="local-publication-owner-and-recipient-boundaries-qualified-unreleased",
        "Unexpected publication boundary identity.");
    var candidate=contract.GetProperty("candidate");
    Require(Text(candidate,"label")=="1.0.0-local-rc-evidence-only" &&
        Text(candidate,"distribution_version")=="1.0.0-preview.4" &&
        candidate.GetProperty("v1_criteria").GetInt32()==10 &&
        candidate.GetProperty("independent_witnesses").GetInt32()==17 &&
        candidate.GetProperty("prior_gate_receipts").GetInt32()==5 &&
        candidate.GetProperty("inspected_local_math_packages").GetInt32()==7,
        "Candidate package or V1 scientific scope drifted.");
});
Probe("source-links",()=>
{
    var basis=contract.GetProperty("basis");
    Require(Text(basis,"candidate_evidence")==T1Evidence &&
        Text(basis,"candidate_map")==T1Map &&
        Text(basis,"package_evidence")==PackageProof &&
        Text(basis,"source_owner_rights")==OwnerRights &&
        Text(basis,"unsent_recipient_handoff")==Recipients,
        "All five durable source authorities must match.");
});
Probe("ten-canonical-v1",()=>
{
    Require(Text(t1,"status")==
        "ten-v1-criteria-and-prior-gates-local-release-evidence-candidate-qualified" &&
        t1.GetProperty("v1_criteria_qualified").GetInt32()==10 &&
        t1.GetProperty("bounded_scientific_witnesses_checked").GetInt32()==17 &&
        t1.GetProperty("prior_milestone_gates_verified").GetInt32()==5 &&
        Flag(t1,"m5_w02_c03_t1_done") &&
        !Flag(t1,"m5_release_done") &&
        !Flag(t1,"recipient_prompts_transmitted"),
        "Qualified T1 candidate proof cannot be substituted with a release claim.");
    Require(map.GetProperty("criteria").GetArrayLength()==10 &&
        !Flag(map,"m5_final_gate_completed") &&
        !Flag(map,"public_nuget_published") &&
        !Flag(map,"stable_release_certified"),
        "Previously witnessed V1 map is required without promotion.");
});
Probe("owner-source-rights-not-publication",()=>
{
    var authorization=owner.GetProperty("authorization");
    var rights=contract.GetProperty("owner");
    Require(Flag(authorization,"copy_into_dw_tools_math") &&
        Flag(authorization,"modify_in_dw_tools_math") &&
        Flag(authorization,"package_from_dw_tools_math") &&
        Flag(authorization,"redistribute_from_dw_tools_math") &&
        Text(owner.GetProperty("license_and_notices"),"model")==
            "owner-authorized proprietary code" &&
        Flag(rights,"source_copy_modify_package_redistribution_authorized") &&
        Flag(rights,"owner_proprietary_code_no_new_license_selected") &&
        !Flag(rights,"stable_v1_publication_authorized") &&
        !Flag(rights,"public_nuget_publication_authorized"),
        "Source redistribution consent is NOT stable 1.0 public publication approval.");
});
Probe("real-local-nuget-and-host",()=>
{
    var host=contract.GetProperty("qualified_host");
    Require(Text(package,"status")==
        "managed-api-native-rid-and-cross-version-release-boundaries-qualified" &&
        Text(package,"local_distribution_version")=="1.0.0-preview.4" &&
        package.GetProperty("inspected_local_archives").GetInt32()==7 &&
        Text(host,"target_framework")=="net10.0" &&
        Text(host,"observed_rid")==Text(package,"observed_runtime_identifier") &&
        Text(host,"observed_sdk")==Text(package,"dotnet_sdk") &&
        Flag(host,"native_foreign_assets_refused") &&
        !Flag(host,"additional_os_rids_certified"),
        "A documented TFM or host must not be an invented OS support matrix.");
});
Probe("not-a-cross-version-binary-contract",()=>
{
    var host=contract.GetProperty("qualified_host");
    Require(!Flag(host,"source_cross_version_compatibility_certified") &&
        !Flag(host,"binary_cross_version_compatibility_certified") &&
        !Flag(host,"portable_performance_sla_certified") &&
        !Flag(package,"cross_version_binary_compatibility_certified") &&
        !Flag(package,"other_platform_rids_certified") &&
        !Flag(package,"portable_performance_sla_certified"),
        "Actual public API SHA fingerprint is not a future ABI guarantee.");
});
Probe("release-refusal-no-action",()=>
{
    var release=contract.GetProperty("publication");
    Require(!Flag(release,"public_nuget_published") &&
        !Flag(release,"stable_v1_package_published") &&
        !Flag(release,"stable_v1_release_certified") &&
        !Flag(release,"m5_final_gate_qualified") &&
        !Flag(release,"irreversible_release_actions_performed") &&
        Flag(release,"source_only_local_candidate"),
        "Local boundary qualification cannot imply external publication.");
});
Probe("three-unsent-recipient-states",()=>
{
    var rec=contract.GetProperty("recipient_adoption").EnumerateArray().ToArray();
    Require(rec.Length==3,"Exactly three recipient owners must be represented.");
    var names=new[]{"AURA","Decision","MCDM"};
    for(var i=0;i<names.Length;i++)
    {
        var row=rec[i];
        Require(Text(row,"recipient")==names[i] &&
            Text(row,"status")=="draft-unsent" &&
            !Flag(row,"delivered")&&!Flag(row,"acknowledged")&&!Flag(row,"adopted"),
            "Recipient is not an implemented external integration: "+names[i]);
    }
});
Probe("unsent-draft-content-and-historic-hash",()=>
{
    var source=ReadText(Recipients);
    Require(source.Contains("Status: draft only.",StringComparison.Ordinal) &&
        source.Contains("## AURA recipient",StringComparison.Ordinal) &&
        source.Contains("## Decision recipient",StringComparison.Ordinal) &&
        source.Contains("## MCDM recipient",StringComparison.Ordinal) &&
        source.Contains("None of these drafts has been sent.",StringComparison.Ordinal) &&
        Digest(Recipients)==Text(t1,"unsent_recipient_drafts_sha256"),
        "Previous owner-recipient handoff must stay independently unsent.");
    var future=ReadText(OwnerDraft);
    Require(future.Contains("UNSENT",StringComparison.Ordinal) &&
        future.Contains("stable publication authorized = false",
            StringComparison.OrdinalIgnoreCase),
        "Owner release request is a question, not authorization.");
});
Probe("migration-and-rollback-only-on-consumer-side",()=>
{
    var migration=contract.GetProperty("migration");
    var text=ReadText(Migration);
    Require(Flag(migration,"local_documentation_only") &&
        !Flag(migration,"external_deployments_performed") &&
        !Flag(migration,"external_rollback_executed") &&
        !Flag(migration,"cutover_approved") &&
        Text(migration,"decision")==
            "blocked-without-owner-and-recipient-attestation" &&
        text.Contains("rollback",StringComparison.OrdinalIgnoreCase) &&
        text.Contains("recipient",StringComparison.OrdinalIgnoreCase) &&
        text.Contains("owner",StringComparison.OrdinalIgnoreCase) &&
        text.Contains("NOT executed",StringComparison.Ordinal),
        "A rollback recipe is not an authorized actual migration.");
});
Probe("future-owner-decisions-are-explicit",()=>
{
    var decisions=contract.GetProperty("required_future_decisions")
        .EnumerateArray().Select(x=>x.GetString()??"").ToArray();
    Require(decisions.Length==4 &&
        decisions.All(x=>x.Length>40)&&
        contract.GetProperty("nonclaims").GetArrayLength()==5,
        "Publication/OS/recipient permissions cannot be invented later.");
});
Probe("workflow-and-backlog-are-not-promoted",()=>
{
    for(var i=0;i<5;i++)
    {
        State(Find(backlog.GetProperty("releases"),"M"+i),"done");
        State(Find(native.GetProperty("milestones"),"M"+i),"done","state");
    }
    State(Find(backlog.GetProperty("releases"),"M5"),"in_progress");
    State(Find(native.GetProperty("milestones"),"M5"),"in-progress","state");
    var current=Find(backlog.GetProperty("work_packages"),"M5-W02");
    var milestone=Find(native.GetProperty("milestones"),"M5");
    var work=Find(milestone.GetProperty("workPackages"),"M5-W02");
    var phase=Find(backlog.GetProperty("chunks"),"M5-W02-C03");
    var nativePhase=Find(work.GetProperty("phases"),"M5-W02-C03");
    var task=Find(phase.GetProperty("tasks"),"M5-W02-C03-T2");
    var nativeTask=Find(nativePhase.GetProperty("tasks"),"M5-W02-C03-T2");
    State(current,isBaseline?"in_progress":"done");
    State(work,isBaseline?"in-progress":"done","state");
    State(phase,isBaseline?"in_progress":"done");
    State(nativePhase,isBaseline?"in-progress":"done","state");
    State(Find(phase.GetProperty("tasks"),"M5-W02-C03-T1"),"done");
    State(Find(nativePhase.GetProperty("tasks"),"M5-W02-C03-T1"),"done","state");
    State(task,isBaseline?"planned":"done");
    State(nativeTask,isBaseline?"not-ready":"done","state");
    foreach(var sub in new[]{"A","B","C"})
    {
        State(Find(task.GetProperty("subtasks"),"M5-W02-C03-T2-"+sub),
            isBaseline?"planned":"done");
        State(Find(nativeTask.GetProperty("subtasks"),"M5-W02-C03-T2-"+sub),
            isBaseline?"not-ready":"done","state");
    }
    Require(Text(milestone,"id")=="M5" &&
        Text(Find(backlog.GetProperty("releases"),"M5"),"gate")=="G-M5",
        "The final independent G-M5 gate remains open.");
});
Require(passed==12,"Exactly twelve independently reported local release-boundary oracles required.");
Console.WriteLine(Marker);
