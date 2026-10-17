using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M5",Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string M5Gate="docs/planning/evidence/M5-gate.json";
const string QualifiedStatus="m5-local-v1-independent-nuget-evidence-gate-qualified-unpublished";
const string CandidateValidator="docs/planning/ValidateM5ReleaseCandidate.cs";
const string PublicationValidator="docs/planning/ValidateM5PublicationBoundary.cs";
const string GateValidator="docs/planning/ValidateM5ReleaseGate.cs";
const string Decision="docs/planning/decisions/m5-gate.md";
const string Document="docs/distribution/m5-local-release-gate.md";
const string W01="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string W02="docs/planning/evidence/M5-W02-work-package-qualified.json";
const string Boundary="docs/planning/evidence/M5-W02-C03-T2-boundaries-qualified.json";
const string Candidate="docs/planning/evidence/M5-W02-C03-T1-local-candidate-qualified.json";
const string NuGetProof="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string M4Gate="docs/planning/evidence/M4-gate.json";
const string OwnerRights="docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json";
const string Product="src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj";
const string Manifest="src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs";
const string TestCodecs="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string TestDistribution="tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj";
const string TestSymbolics="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string TestIr="tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj";
const string TestQuantities="tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string Solution="dw.tools.math.slnx";
const string Version="1.0.0-preview.4";
const string PackageHelper=".workflow/runs/RS077/staged/checks/m5-w02-c02-t2-package-sha.cs";
const string SurfaceConsumer="tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj";
const string SurfaceConfig="tests/consumers/m5-w02-c02-t2/NuGet.Config";
const string CleanConsumer="tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj";
const string CleanConfig="tests/consumers/m5-w02-c02-t1/NuGet.Config";
const string Sample="samples/m5-exact-vertical/m5-exact-vertical.csproj";
const string SampleConfig="samples/m5-exact-vertical/NuGet.Config";
const string GateMarker="dw.tools.math/m5-local-scientific-gate/1.0 qualified; phases=7; tasks=14; subtasks=42; v1=10; public=false";
const string CandidateMarker="dw.tools.math/m5-v1-local-rc-evidence/1.0 qualified; criteria=10; predecessor-gates=5; publication=false";
const string BoundaryMarker="dw.tools.math/m5-local-publication-boundary/1.0 qualified; oracles=12; recipients=3; public=false";
const string SurfaceMarker="dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=8";
const string CleanMarker="dw.tools.math/m5-clean-package-consumer/1.0 qualified; probes=8";
const string SampleMarker="dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes=8";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,M5Gate));
p.Workflow.RequireCurrentRun("RS080",target?null:Milestone,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS079",1,
    "docs: qualify Math local V1 release boundaries and close M5 W02",
    "0.1.59",1,1,0,false);

p.Require.JsonValueEquals(M4Gate,"/status",
    JsonValue.Create("m4-local-exact-symbolic-preview-gate-qualified"));
p.Require.JsonValueEquals(M4Gate,"/m4_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(OwnerRights,
    "/authorization/redistribute_from_dw_tools_math",JsonValue.Create(true));
p.Require.JsonValueEquals(W01,"/status",
    JsonValue.Create("m5-w01-exact-math-codecs-interchange-boundaries-qualified"));
p.Require.JsonValueEquals(W01,"/m5_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(W01,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(W02,"/status",
    JsonValue.Create("m5-w02-local-v1-qualification-complete-no-stable-publication"));
p.Require.JsonValueEquals(W02,"/m5_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(W02,"/final_m5_gate_pending",JsonValue.Create(true));
p.Require.JsonValueEquals(W02,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(W02,"/local_distribution_version",JsonValue.Create(Version));
p.Require.JsonValueEquals(W02,"/public_nuget_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Candidate,"/status",
    JsonValue.Create("ten-v1-criteria-and-prior-gates-local-release-evidence-candidate-qualified"));
p.Require.JsonValueEquals(Candidate,"/v1_criteria_qualified",JsonValue.Create(10));
p.Require.JsonValueEquals(Candidate,"/bounded_scientific_witnesses_checked",JsonValue.Create(17));
p.Require.JsonValueEquals(Candidate,"/prior_milestone_gates_verified",JsonValue.Create(5));
p.Require.JsonValueEquals(Candidate,"/public_nuget_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Boundary,"/status",
    JsonValue.Create("local-v1-publication-owner-and-recipient-boundaries-qualified-not-released"));
p.Require.JsonValueEquals(Boundary,"/read_only_release_boundary_oracles",JsonValue.Create(12));
p.Require.JsonValueEquals(Boundary,"/recipient_drafts_unsent",JsonValue.Create(3));
p.Require.JsonValueEquals(Boundary,"/m5_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Boundary,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Boundary,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(NuGetProof,"/local_distribution_version",JsonValue.Create(Version));
p.Require.JsonValueEquals(NuGetProof,"/inspected_local_archives",JsonValue.Create(7));
p.Require.JsonValueEquals(NuGetProof,"/other_platform_rids_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(NuGetProof,"/stable_release_certified",JsonValue.Create(false));
p.Require.TextContains(Product,"<Version>1.0.0-preview.4</Version>");
p.Require.TextContains(Manifest,"\"dw.tools.math.distribution\",\"1.0.0-preview.4\"");
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.78"),JsonValue.Create("0.1.79"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.79":"0.1.78"));
for(var index=0;index<5;index++)
    p.Require.JsonValueEquals(Backlog,$"/releases/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/releases/5/gate",JsonValue.Create("G-M5"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("planned"));
foreach(var index in new[]{11,12})
    p.Require.JsonValueEquals(Backlog,$"/work_packages/{index}/status",JsonValue.Create("done"));
foreach(var index in new[]{33,34,35,36,37,38,39})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState("M5-W02","done");
p.ProjectPlan.RequireNodeState("M6","not-ready");
foreach(var phase in new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03",
    "M5-W01-C04","M5-W02-C01","M5-W02-C02","M5-W02-C03"})
    p.ProjectPlan.RequireNodeState(phase,"done");

if(!target)
{
    foreach(var file in new[]{GateValidator,Decision,Document,M5Gate})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(M5Gate,"/status",
        JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(M5Gate,"/gate",JsonValue.Create("G-M5"));
    p.Require.JsonValueEquals(M5Gate,"/qualified_m5_phases",JsonValue.Create(7));
    p.Require.JsonValueEquals(M5Gate,"/qualified_tasks",JsonValue.Create(14));
    p.Require.JsonValueEquals(M5Gate,"/qualified_subtasks",JsonValue.Create(42));
    p.Require.JsonValueEquals(M5Gate,"/isolated_nuget_consumer_probes",JsonValue.Create(24));
    p.Require.JsonValueEquals(M5Gate,"/m5_milestone_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(M5Gate,"/packages_publicly_published",JsonValue.Create(false));
    p.Require.JsonValueEquals(M5Gate,"/stable_release_certified",JsonValue.Create(false));
    p.Require.JsonValueEquals(M5Gate,"/owner_stable_publication_approved",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{CandidateValidator,PublicationValidator,GateValidator,
    Decision,Document})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("G-M5 staged scientific/documentary asset SHA-256 is absent: "+file);
}
foreach(var entry in new[]{
    (File:CandidateValidator,Marker:CandidateMarker),
    (File:PublicationValidator,Marker:BoundaryMarker),
    (File:GateValidator,Marker:GateMarker)})
{
    var output=Required(root,"dotnet","run","--file",entry.File,"--","--check");
    if(!HasExactLine(output,entry.Marker))
        throw new InvalidOperationException("Required qualified local M5 science boundary did not pass: "+entry.File);
}
var gateOutput=Required(root,"dotnet","run","--file",GateValidator,"--","--check");
var phaseLines=gateOutput.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
    .Where(line=>line.StartsWith("M5-GATE-PHASE:",StringComparison.Ordinal)).ToArray();
if(phaseLines.Length!=7)
    throw new InvalidOperationException("Independent M5 gate must provide exactly seven phase SHA-256 observations.");
var phaseIds=new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03","M5-W01-C04",
    "M5-W02-C01","M5-W02-C02","M5-W02-C03"};
for(var i=0;i<phaseIds.Length;i++)
{
    var prefix="M5-GATE-PHASE:"+phaseIds[i]+":";
    var line=phaseLines[i];
    var digest=line.StartsWith(prefix,StringComparison.Ordinal)?
        line[prefix.Length..]:string.Empty;
    if(digest.Length!=64 ||
        digest.Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Missing ordered, content-derived M5 phase SHA-256: "+phaseIds[i]);
}

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
foreach(var test in new[]{TestDistribution,TestCodecs,TestIr,TestSymbolics,TestQuantities})
{
    var focused=Required(root,"dotnet","test",test,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!focused.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("A focused M5 scientific regression failed: "+test);
}
var fullTests=Required(root,"dotnet","test",Solution,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
if(!fullTests.Contains("Passed",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Full admitted Math solution suite did not show passing tests.");

var graph=new (string Id,string Version,string Project,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1","src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1","src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8","src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10","src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4","src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1","src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution",Version,Product,["dw.quantities","dw.tools.math.ir",
        "dw.tools.math.codecs","dw.tools.math.symbolics","dw.tools.math.numerics",
        "dw.tools.math.composition"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w02-c02-t2-local-feed");
var archives=new List<string>();
foreach(var pkg in graph)
{
    Required(root,"dotnet","pack",pkg.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,pkg.Id+"."+pkg.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("M5 inspected local package missing: "+pkg.Id);
    archives.Add(archive);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var packageOutput=Required(root,"dotnet",helperArgs.ToArray());
var archiveReceipts=new Dictionary<string,(long Bytes,string Sha)>(StringComparer.Ordinal);
var products=new List<JsonNode>();
foreach(var item in graph)
{
    var inspected=ReadProductReceipt(packageOutput,item.Id,item.Version);
    archiveReceipts.Add(item.Id,inspected);
    products.Add(new JsonObject
    {
        ["id"]=item.Id,["version"]=item.Version,["bytes"]=inspected.Bytes,
        ["sha256"]=inspected.Sha,
        ["dependencies"]=p.Json.StringArray(item.Dependencies),
        ["assembly"]="lib/net10.0/"+item.Id+".dll",
        ["publication"]="local-only-unpublished"
    });
}

foreach(var pair in new[]{
    (Project:SurfaceConsumer,Config:SurfaceConfig),
    (Project:CleanConsumer,Config:CleanConfig),
    (Project:Sample,Config:SampleConfig)})
    RequirePinnedSourceOnly(root,pair.Project,pair.Config,Version);

var consumers=new (string Project,string Config,string Marker,int Count)[]
{
    (SurfaceConsumer,SurfaceConfig,SurfaceMarker,8),
    (CleanConsumer,CleanConfig,CleanMarker,8),
    (Sample,SampleConfig,SampleMarker,8)
};
var consumerOutput=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var client in consumers)
{
    var cache=Path.Combine(root,"build","artifacts",
        "m5-gate-nuget-cache-"+(client.Project==SurfaceConsumer?"surface":
            client.Project==CleanConsumer?"clean":"sample"));
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var call=new List<string>{"run","--project",client.Project,
        "-c","Release","--no-build","--no-restore"};
    if(client.Project!=Sample)
    {
        call.Add("--");
        call.AddRange(archives);
    }
    var result=Required(root,"dotnet",call.ToArray());
    if(!HasExactLine(result,client.Marker))
        throw new InvalidOperationException("Expected eight independent local NuGet Math probes missing: "+client.Project);
    consumerOutput.Add(client.Project,result);
}
var clean=consumerOutput[CleanConsumer];
var publicApi=consumerOutput[SurfaceConsumer];
var surfaces=new List<JsonNode>();
foreach(var pkg in graph)
{
    var cleanReceipt=ReadProductReceipt(clean,pkg.Id,pkg.Version,
        "M5-W02-C02-T1-CONSUMER-SHA256:");
    var nativeReceipt=archiveReceipts[pkg.Id];
    if(cleanReceipt.Bytes!=nativeReceipt.Bytes ||
       !string.Equals(cleanReceipt.Sha,nativeReceipt.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("M5 gate package archive SHA differs from independent clean consumer: "+pkg.Id);
    var surface=ReadSurface(publicApi,pkg.Id,pkg.Version);
    surfaces.Add(new JsonObject
    {
        ["id"]=pkg.Id,["version"]=pkg.Version,
        ["public_api_sha256"]=surface.Public,
        ["managed_assembly_sha256"]=surface.Assembly
    });
}
var sampleReceipt=consumerOutput[Sample];
if(!sampleReceipt.Contains("\"derivative\":\"mul(2,x)\"",StringComparison.Ordinal) ||
   !sampleReceipt.Contains("\"exact_value\":\"3\"",StringComparison.Ordinal) ||
   !sampleReceipt.Contains("\"admitted_roots\":[\"-1\"]",StringComparison.Ordinal) ||
   !sampleReceipt.Contains("\"excluded_candidates\":[\"1\"]",StringComparison.Ordinal) ||
   !sampleReceipt.Contains("\"original_root_audit_complete\":false",StringComparison.Ordinal))
    throw new InvalidOperationException("M5 exact original-domain example changed under milestone gate.");

Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-gate",
        ["status"]=QualifiedStatus,
        ["milestone"]=Milestone,["gate"]="G-M5",
        ["release_version"]="1.0.0-local-evidence-only",
        ["local_distribution_version"]=Version,
        ["plan_version"]="0.1.79",
        ["previous_m4_gate"]=M4Gate,
        ["m5_w01_work_package"]=W01,
        ["m5_w02_work_package"]=W02,
        ["owner_rights_source"]=OwnerRights,
        ["candidate_t1_proof"]=Candidate,
        ["publication_boundary_t2_proof"]=Boundary,
        ["qualified_phase_ids"]=p.Json.StringArray(phaseIds),
        ["qualified_phase_evidence_sha256"]=p.Json.StringArray(phaseLines),
        ["qualified_phase_evidence"]=p.Json.StringArray(
            "docs/planning/evidence/M5-W01-C01-T2-boundaries-qualified.json",
            "docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json",
            "docs/planning/evidence/M5-W01-C03-T2-boundaries-qualified.json",
            "docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json",
            "docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json",
            NuGetProof,Boundary),
        ["qualified_m5_phases"]=7,["qualified_tasks"]=14,["qualified_subtasks"]=42,
        ["original_v1_criteria_checked"]=10,
        ["scientific_v1_witnesses_checked"]=17,
        ["owner_publication_boundary_oracles_checked"]=12,
        ["three_untransmitted_recipients_retained"]=true,
        ["gate_validator"]=GateValidator,
        ["gate_validator_sha256"]=hashes[GateValidator],
        ["candidate_validator"]=CandidateValidator,
        ["candidate_validator_sha256"]=hashes[CandidateValidator],
        ["publication_validator"]=PublicationValidator,
        ["publication_validator_sha256"]=hashes[PublicationValidator],
        ["gate_decision"]=Decision,["gate_decision_sha256"]=hashes[Decision],
        ["release_recipe"]=Document,["release_recipe_sha256"]=hashes[Document],
        ["locked_solution_restore_and_build_passed"]=true,
        ["focused_scientific_regressions_passed"]=5,
        ["full_solution_test_suite_passed"]=true,
        ["seven_local_nuget_packages_inspected"]=7,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["public_api_binary_surface_sha256"]=p.Json.Array(surfaces.ToArray()),
        ["package_inspector"]=PackageHelper,
        ["isolated_nuget_consumer_probes"]=24,
        ["independent_nuget_consumers"]=3,
        ["surface_consumer_marker"]=SurfaceMarker,
        ["clean_consumer_marker"]=CleanMarker,
        ["exact_sample_marker"]=SampleMarker,
        ["runtime_target_framework"]="net10.0",
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["local_math_milestone_scope_qualified"]=true,
        ["m5_milestone_done"]=true,
        ["m6_started"]=false,
        ["packages_publicly_published"]=false,
        ["stable_release_certified"]=false,
        ["owner_stable_publication_approved"]=false,
        ["other_os_rids_certified"]=false,
        ["cross_version_binary_compatibility_certified"]=false,
        ["external_aura_decision_mcdm_adoption_observed"]=false,
        ["next_accepted_scope"]="M6-W01-C01-T1: independently qualify bounded linear/spline interpolation and least squares; publication remains owner-controlled.",
        ["nonclaims"]=p.Json.StringArray(
            "M5 local milestone qualification is distinct from stable/public NuGet 1.0 publication.",
            "A single observed host does not certify other operating systems or source/binary compatibility across versions.",
            "The ten V1 criteria are bounded; no general CAS, Office document importer or universal mathematics.",
            "No AURA, Decision, MCDM external adoption, migration, owner publication approval or new license is implied.")
    };
    p.Files.WriteComplete(M5Gate,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,roadmap=>
    {
        roadmap["plan_version"]="0.1.79";
        var release=roadmap["releases"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Milestone);
        if((string?)release["status"]!="in_progress" ||
           (string?)release["gate"]!="G-M5")
            throw new InvalidOperationException("M5 original milestone version/state has drifted.");
        release["status"]="done";
        release["evidence"]=p.Json.StringArray(W01,W02,Candidate,Boundary,M5Gate);
    });
    p.ProjectPlan.ConvergeNodeToDone(Milestone);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"done");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState("M5-W02","done");
p.ProjectPlan.RequireNodeState("M6","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS080"||
       (string?)state["activeMilestone"]!=(target?null:Milestone)||
       state["activeWorkPackage"] is not null||
       state["activePhase"] is not null||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("G-M5 native state cursor differs from baseline/target.");
    state["activeMilestone"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
foreach(var check in new[]{
    (Path:GateValidator,Marker:GateMarker),
    (Path:CandidateValidator,Marker:CandidateMarker),
    (Path:PublicationValidator,Marker:BoundaryMarker)})
{
    var result=Required(root,"dotnet","run","--file",check.Path,"--","--check");
    if(!HasExactLine(result,check.Marker))
        throw new InvalidOperationException("Completed local M5 gate re-entry fails: "+check.Path);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void RequirePinnedSourceOnly(
    string root,string project,string config,string distributionVersion)
{
    var document=XDocument.Load(Path.Combine(root,project));
    var refs=document.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(refs.Length!=1 ||
       (string?)refs[0].Attribute("Include")!="dw.tools.math.distribution" ||
       (string?)refs[0].Attribute("Version")!="["+distributionVersion+"]" ||
       document.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Exactly one source-free pinned Math NuGet project is required: "+project);
    var feeds=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!feeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
       feeds.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External NuGet source is not admitted: "+config);
}

static bool HasExactLine(string output,string expected) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static (long Bytes,string Sha) ReadProductReceipt(
    string output,string id,string version,
    string marker="M5-W02-C02-T2-PKG-SHA256:")
{
    var prefix=marker+id+":"+version+":";
    var found=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(found.Length!=1)
        throw new InvalidOperationException("One local independent Math package receipt required for "+id);
    var parts=found[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid local NuGet bytes/SHA-256 receipt for "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
    return part.Length>2500?part[..2500]:part;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out during a bounded Math subprocess command.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}

static (string Public,string Assembly) ReadSurface(string output,string id,string version)
{
    var prefix="M5-W02-C02-T2-SURFACE-SHA256:"+id+":"+version+":";
    var hits=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(hits.Length!=1)throw new InvalidOperationException("One managed ABI receipt expected: "+id);
    var values=hits[0][prefix.Length..].Split(':');
    if(values.Length!=2 || values.Any(x=>x.Length!=64 ||
        x.Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f')))))
        throw new InvalidOperationException("Invalid public API or DLL SHA for "+id);
    return(values[0],values[1]);
}
