#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

// Independent, BCL-only, read-only scientific admission for LOCAL Math M5.
// A completed local milestone is not a signed stable/public NuGet release.
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string W01="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string W02="docs/planning/evidence/M5-W02-work-package-qualified.json";
const string Candidate="docs/planning/evidence/M5-W02-C03-T1-local-candidate-qualified.json";
const string Boundary="docs/planning/evidence/M5-W02-C03-T2-boundaries-qualified.json";
const string Packages="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string Map="docs/planning/releases/m5-v1-local-candidate-map.json";
const string Matrix="docs/planning/releases/m5-v1-publication-boundary.json";
const string FinalGate="docs/planning/evidence/M5-gate.json";
const string Marker="dw.tools.math/m5-local-scientific-gate/1.0 qualified; phases=7; tasks=14; subtasks=42; v1=10; public=false";
const string GateStatus="m5-local-v1-independent-nuget-evidence-gate-qualified-unpublished";

var phases=new (string Id,string Evidence,string Status)[]
{
    ("M5-W01-C01","M5-W01-C01-T2-boundaries-qualified.json",
        "exact-safe-canonical-display-original-restrictions-and-ambiguity-qualified"),
    ("M5-W01-C02","M5-W01-C02-T2-boundaries-qualified.json",
        "bounded-latex-import-semantic-loss-and-adversarial-boundaries-qualified"),
    ("M5-W01-C03","M5-W01-C03-T2-boundaries-qualified.json",
        "exact-annotated-mathml-xml-security-and-loss-boundaries-qualified"),
    ("M5-W01-C04","M5-W01-C04-T2-boundaries-qualified.json",
        "exact-office-omml-markdown-unicode-loss-and-format-boundaries-qualified"),
    ("M5-W02-C01","M5-W02-C01-T2-boundaries-qualified.json",
        "exact-numeric-symbolic-original-domain-provenance-loss-boundaries-qualified"),
    ("M5-W02-C02","M5-W02-C02-T2-boundaries-qualified.json",
        "managed-api-native-rid-and-cross-version-release-boundaries-qualified"),
    ("M5-W02-C03","M5-W02-C03-T2-boundaries-qualified.json",
        "local-v1-publication-owner-and-recipient-boundaries-qualified-not-released")
};

static void Need(bool okay,string reason)
{
    if(!okay)throw new InvalidOperationException("Independent M5 local science gate: "+reason);
}
static string ReadText(string relative)
{
    Need((relative.StartsWith("docs/",StringComparison.Ordinal) ||
        string.Equals(relative,Native,StringComparison.Ordinal)) &&
        !Path.IsPathRooted(relative) &&
        !relative.Contains("..",StringComparison.Ordinal) &&
        !relative.Contains('\\'),
        "Only explicitly scoped local evidence and native plan may be read: "+relative);
    return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar)));
}
static JsonDocument Read(string relative)=>JsonDocument.Parse(ReadText(relative));
static string String(JsonElement node,string key)=>
    node.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing nonnull string "+key);
static JsonElement Find(JsonElement collection,string id)=>
    collection.EnumerateArray().Single(item=>String(item,"id")==id);
static void RequireState(JsonElement obj,string expected,string key="status")=>
    Need(String(obj,key)==expected,"Wrong "+key+" for "+String(obj,"id"));
static bool Flag(JsonElement obj,string key)=>obj.GetProperty(key).GetBoolean();
static string Hash(string relative)
{
    var bytes=File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar)));
    return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is allowed.");
using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
using var w01Doc=Read(W01);
using var w02Doc=Read(W02);
using var candidateDoc=Read(Candidate);
using var boundaryDoc=Read(Boundary);
using var packageDoc=Read(Packages);
using var mapDoc=Read(Map);
using var matrixDoc=Read(Matrix);
using var rightsDoc=Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var w01=w01Doc.RootElement;
var w02=w02Doc.RootElement;
var candidate=candidateDoc.RootElement;
var boundary=boundaryDoc.RootElement;
var package=packageDoc.RootElement;
var map=mapDoc.RootElement;
var matrix=matrixDoc.RootElement;
var rights=rightsDoc.RootElement;
var version=String(backlog,"plan_version");
var accepted=version=="0.1.78";
var completed=version=="0.1.79";
Need(accepted||completed,"Only accepted pre-gate and completed M5 versions are admitted.");

for(var index=0;index<5;index++)
{
    var id="M"+index;
    RequireState(Find(backlog.GetProperty("releases"),id),"done");
    RequireState(Find(native.GetProperty("milestones"),id),"done","state");
}
var milestone=Find(backlog.GetProperty("releases"),"M5");
var execution=Find(native.GetProperty("milestones"),"M5");
RequireState(milestone,completed?"done":"in_progress");
RequireState(execution,completed?"done":"in-progress","state");
Need(String(milestone,"gate")=="G-M5" &&
    String(milestone,"version")=="1.0.0" &&
    milestone.GetProperty("required_chunks").GetArrayLength()==7,
    "The accepted seven-phase local G-M5 gate must not be redefined.");
var w01Product=Find(backlog.GetProperty("work_packages"),"M5-W01");
var w02Product=Find(backlog.GetProperty("work_packages"),"M5-W02");
var w01Plan=Find(execution.GetProperty("workPackages"),"M5-W01");
var w02Plan=Find(execution.GetProperty("workPackages"),"M5-W02");
foreach(var current in new[]{w01Product,w02Product})
    RequireState(current,"done");
foreach(var current in new[]{w01Plan,w02Plan})
    RequireState(current,"done","state");

Need(String(w01,"status")=="m5-w01-exact-math-codecs-interchange-boundaries-qualified" &&
    Flag(w01,"m5_w01_work_package_done") &&
    !Flag(w01,"m5_release_done"),"The original exact codec package lacks durable evidence.");
Need(String(w02,"status")=="m5-w02-local-v1-qualification-complete-no-stable-publication" &&
    Flag(w02,"m5_w02_work_package_done") &&
    !Flag(w02,"m5_release_done") &&
    w02.GetProperty("ten_original_v1_criteria_qualified").GetInt32()==10 &&
    w02.GetProperty("source_free_local_net10_archives_qualified").GetInt32()==7 &&
    !Flag(w02,"owner_stable_publication_authorized") &&
    !Flag(w02,"public_nuget_published"),"The local package must not be mistaken for a public release.");
Need(String(candidate,"status")==
    "ten-v1-criteria-and-prior-gates-local-release-evidence-candidate-qualified" &&
    candidate.GetProperty("v1_criteria_qualified").GetInt32()==10 &&
    candidate.GetProperty("bounded_scientific_witnesses_checked").GetInt32()==17 &&
    candidate.GetProperty("prior_milestone_gates_verified").GetInt32()==5 &&
    !Flag(candidate,"m5_release_done"),
    "Ten bounded V1 criteria and independent M0-M4 facts must be retained.");
Need(String(boundary,"status")==
    "local-v1-publication-owner-and-recipient-boundaries-qualified-not-released" &&
    boundary.GetProperty("read_only_release_boundary_oracles").GetInt32()==12 &&
    boundary.GetProperty("recipient_drafts_unsent").GetInt32()==3 &&
    Flag(boundary,"m5_w02_work_package_done") &&
    !Flag(boundary,"public_nuget_published") &&
    !Flag(boundary,"m5_release_done"),
    "T2 must be bounded, tested and explicitly unpublished.");
Need(String(package,"status")==
    "managed-api-native-rid-and-cross-version-release-boundaries-qualified" &&
    String(package,"local_distribution_version")=="1.0.0-preview.4" &&
    package.GetProperty("inspected_local_archives").GetInt32()==7 &&
    !Flag(package,"cross_version_binary_compatibility_certified") &&
    !Flag(package,"other_platform_rids_certified") &&
    !Flag(package,"public_nuget_published"),
    "Seven real nupkg archives and true platform boundaries must survive.");

Need(map.GetProperty("criteria").GetArrayLength()==10 &&
    !Flag(map,"m5_final_gate_completed") &&
    !Flag(map,"public_nuget_published") &&
    !Flag(map,"stable_release_certified") &&
    Flag(map,"publication_requires_owner_authorization"),
    "The historical candidate is evidence-only.");
var publication=matrix.GetProperty("publication");
Need(String(matrix,"status")==
    "local-publication-owner-and-recipient-boundaries-qualified-unreleased" &&
    !Flag(publication,"public_nuget_published") &&
    !Flag(publication,"stable_v1_release_certified") &&
    !Flag(publication,"m5_final_gate_qualified") &&
    !Flag(matrix.GetProperty("owner"),"stable_v1_publication_authorized") &&
    matrix.GetProperty("recipient_adoption").GetArrayLength()==3,
    "Public/stable publication and external adoption remain unobserved.");
Need(Flag(rights.GetProperty("authorization"),"redistribute_from_dw_tools_math") &&
    String(rights.GetProperty("license_and_notices"),"model")==
        "owner-authorized proprietary code",
    "Selected source rights are recorded without inventing a stable publication approval.");

var acceptedPhases=milestone.GetProperty("required_chunks")
    .EnumerateArray().Select(x=>x.GetString()??"").ToArray();
Need(phases.Length==7 &&
    acceptedPhases.SequenceEqual(phases.Select(x=>x.Id),StringComparer.Ordinal) &&
    phases.Select(x=>x.Id).Distinct(StringComparer.Ordinal).Count()==7,
    "Seven required M5 phases must retain canonical order/identity.");
var tasks=0;
var subtasks=0;
foreach(var phase in phases)
{
    var product=Find(backlog.GetProperty("chunks"),phase.Id);
    var workPlan=phase.Id.StartsWith("M5-W01-",StringComparison.Ordinal)?w01Plan:w02Plan;
    var nativePhase=Find(workPlan.GetProperty("phases"),phase.Id);
    RequireState(product,"done");
    RequireState(nativePhase,"done","state");
    Need(product.GetProperty("evidence").GetArrayLength()>0,
        "Missing scientific evidence for "+phase.Id);
    var proof="docs/planning/evidence/"+phase.Evidence;
    using var proofDoc=Read(proof);
    var observation=proofDoc.RootElement;
    Need(String(observation,"status")==phase.Status,
        "Qualified mathematical limits changed: "+phase.Id);
    var scientificFlag=phase.Id.ToLowerInvariant().Replace('-','_')+"_phase_done";
    Need(Flag(observation,scientificFlag) &&
        !Flag(observation,"m5_release_done"),
        "The phase scientific receipt cannot prequalify the M5 milestone: "+phase.Id);
    var listed=product.GetProperty("tasks").EnumerateArray().ToArray();
    var actual=nativePhase.GetProperty("tasks").EnumerateArray().ToArray();
    Need(listed.Length==2 && actual.Length==2,"Each M5 phase has two accepted tasks: "+phase.Id);
    foreach(var item in listed)
    {
        var id=String(item,"id");
        var task=Find(nativePhase.GetProperty("tasks"),id);
        RequireState(item,"done");
        RequireState(task,"done","state");
        tasks++;
        var children=item.GetProperty("subtasks").EnumerateArray().ToArray();
        var nativeChildren=task.GetProperty("subtasks").EnumerateArray().ToArray();
        Need(children.Length==3 && nativeChildren.Length==3,
            "Each qualified Math task has exactly three accepted substages: "+id);
        foreach(var child in children)
        {
            var childId=String(child,"id");
            RequireState(child,"done");
            RequireState(Find(task.GetProperty("subtasks"),childId),"done","state");
            subtasks++;
        }
    }
    Console.WriteLine("M5-GATE-PHASE:"+phase.Id+":"+Hash(proof));
}
Need(tasks==14 && subtasks==42,"The milestone requires 14 done tasks and 42 done subtasks.");
if(completed)
{
    Need(File.Exists(Path.Combine(Directory.GetCurrentDirectory(),FinalGate)),
        "A completed M5 milestone requires its own science-gate receipt.");
    using var gateDoc=Read(FinalGate);
    var gate=gateDoc.RootElement;
    Need(String(gate,"status")==GateStatus &&
        String(gate,"gate")=="G-M5" &&
        Flag(gate,"m5_milestone_done") &&
        !Flag(gate,"packages_publicly_published") &&
        !Flag(gate,"stable_release_certified") &&
        !Flag(gate,"owner_stable_publication_approved") &&
        gate.GetProperty("qualified_phase_evidence").GetArrayLength()==7,
        "A completed local milestone must remain independently qualified, not published.");
    Need(milestone.GetProperty("evidence").EnumerateArray()
        .Any(x=>x.GetString()==FinalGate),
        "The authoritative milestone must point to its durable M5 gate.");
}
Console.WriteLine(Marker);
