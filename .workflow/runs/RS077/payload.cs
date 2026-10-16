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

const string M="M5",W="M5-W02",C="M5-W02-C02",T="M5-W02-C02-T2";
const string R="M5-W02-C02-T2-R",G="M5-W02-C02-T2-G",V="M5-W02-C02-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W02-C02-T2-contract-red.json";
const string Proof="docs/planning/evidence/M5-W02-C02-T2-boundaries-qualified.json";
const string ProofStatus="managed-api-native-rid-and-cross-version-release-boundaries-qualified";
const string Project="src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj";
const string Manifest="src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs";
const string Source="src/projects/dw.tools.math.distribution/MathPackageRuntimeSurfaceAudit.cs";
const string Test="tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.distribution.tests/M5W02C02T2RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.distribution.tests/M5W02C02T2Tests.cs";
const string RetainedTests="tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs";
const string Client="tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj";
const string ClientCode="tests/consumers/m5-w02-c02-t2/Program.cs";
const string ClientConfig="tests/consumers/m5-w02-c02-t2/NuGet.Config";
const string Clean="tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj";
const string CleanCode="tests/consumers/m5-w02-c02-t1/Program.cs";
const string CleanConfig="tests/consumers/m5-w02-c02-t1/NuGet.Config";
const string Sample="samples/m5-exact-vertical/m5-exact-vertical.csproj";
const string SampleConfig="samples/m5-exact-vertical/NuGet.Config";
const string Readme="docs/distribution/m5-package-readme.md";
const string Notices="docs/distribution/m5-package-notices.md";
const string Decision="docs/planning/decisions/m5-w02-c02-t2-abi-rid.md";
const string Matrix="docs/distribution/m5-api-abi-native-support-matrix.md";
const string Inspector=".workflow/runs/RS077/staged/checks/m5-w02-c02-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.4";
const string RedMarker="M5-W02-C02-T2 RED: bounded managed API and native/RID surface audit API is absent.";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Proof));
p.Workflow.RequireCurrentRun("RS077",M,W,target?null:C,null,null);
p.Workflow.RequirePriorSuccess("RS076",1,
    "feat: qualify clean local Math NuGet archive and distribution contracts",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("local-versioned-net10-package-graph-clean-consumer-and-notices-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(40));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(41));
p.Require.JsonValueEquals(Prior,"/local_distribution_version",JsonValue.Create("1.0.0-preview.3"));
p.Require.JsonValueEquals(Prior,"/m5_w02_c02_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w02_c02_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.75"),JsonValue.Create("0.1.76"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.76":"0.1.75"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("in_progress"));
foreach(var path in new[]{Project,Manifest,Sample,Clean})
    p.Require.TextContains(path,target?"1.0.0-preview.4":"1.0.0-preview.3");
p.ProjectPlan.RequireNodeState(M,"in-progress");
p.ProjectPlan.RequireNodeState(W,"in-progress");
p.ProjectPlan.RequireNodeState(C,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C02-T1","done");
p.ProjectPlan.RequireNodeState(T,target?"done":"not-ready");
foreach(var id in new[]{R,G,V})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");
if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTest,Client,ClientCode,
        ClientConfig,Decision,Matrix,RedProof,Proof})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-managed-public-audit-api-absent-red-observed"));
    p.Require.JsonValueEquals(Proof,"/status",JsonValue.Create(ProofStatus));
    p.Require.JsonValueEquals(Proof,"/independent_green_tests_passed",JsonValue.Create(36));
    p.Require.JsonValueEquals(Proof,"/red_recovered_and_green_tests_passed",JsonValue.Create(37));
    p.Require.JsonValueEquals(Proof,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Proof,"/retained_clean_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Proof,"/retained_sample_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Proof,"/m5_w02_c02_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Proof,"/m5_w02_c02_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Proof,"/m5_w02_work_package_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redFile=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redFile.AfterSha256 ??
    throw new InvalidOperationException("Compiled absent-API RED source SHA missing.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var failing=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W02C02T2RedTests.PublicManagedSurfaceAndRuntimeBoundaryAuditMustExist",
        "--logger","console;verbosity=normal");
    if(failing.Code==0 || !failing.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Genuinely compiled public managed audit RED required: "+Relevant(failing.Output));
    p.Files.WriteComplete(RedProof,new JsonObject{
        ["schema_version"]=1,["id"]="M5-W02-C02-T2-contract-red",
        ["status"]="compiled-managed-public-audit-api-absent-red-observed",
        ["prior_evidence"]=Prior,["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["expected_red_assertion"]=RedMarker,
        ["compiler_failure_is_not_a_red_substitute"]=true
    }.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Project,Manifest,Source,GreenTest,RetainedTests,Client,ClientCode,
    ClientConfig,Clean,CleanCode,Sample,Readme,Notices,Decision,Matrix})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Source hash unavailable: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var focused=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M5W02C02T2",
    "--logger","console;verbosity=normal");
if(!focused.Contains("Total tests: 37",StringComparison.OrdinalIgnoreCase) ||
   !focused.Contains("Passed: 37",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("The compiled RED and 36 independent GREEN tests must pass.");
var retainedOutput=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter",
    "FullyQualifiedName~M5W02C02T1Tests",
    "--logger","console;verbosity=normal");
if(!retainedOutput.Contains("Total tests: 40",StringComparison.OrdinalIgnoreCase) ||
   !retainedOutput.Contains("Passed: 40",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "All 40 retained T1 package archive oracles must pass the current declared version before the broader distribution suite.");
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore");
foreach(var regression in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj",
    "tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var result=Required(root,"dotnet","test",regression,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!result.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Prior Math regression failed: "+regression);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1","src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1","src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8","src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10","src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4","src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1","src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution",Version,Project,["dw.quantities","dw.tools.math.ir",
        "dw.tools.math.codecs","dw.tools.math.symbolics","dw.tools.math.numerics",
        "dw.tools.math.composition"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w02-c02-t2-local-feed");
var archives=new List<string>();
foreach(var item in packages)
{
    Required(root,"dotnet","pack",item.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var file=Path.Combine(feed,item.Id+"."+item.Version+".nupkg");
    if(!File.Exists(file))throw new InvalidOperationException("Package not packed: "+item.Id);
    archives.Add(file);
}
var inspectArgs=new List<string>{"run","--file",Inspector,"--"};
inspectArgs.AddRange(archives);
var inspected=Required(root,"dotnet",inspectArgs.ToArray());
var products=new List<JsonNode>();
var archiveReceipts=new Dictionary<string,(long Bytes,string Sha)>(StringComparer.Ordinal);
foreach(var item in packages)
{
    var sha=ReadProductReceipt(inspected,item.Id,item.Version);
    archiveReceipts.Add(item.Id,sha);
    products.Add(new JsonObject{
        ["id"]=item.Id,["version"]=item.Version,
        ["sha256"]=sha.Sha,["bytes"]=sha.Bytes,
        ["dependencies"]=p.Json.StringArray(item.DependsOn),
        ["assembly"]="lib/net10.0/"+item.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}
static void EnsurePinned(string root,string csproj,string config,string version)
{
    var d=XDocument.Load(Path.Combine(root,csproj));
    var refs=d.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(refs.Length!=1 ||
       (string?)refs[0].Attribute("Include")!="dw.tools.math.distribution" ||
       (string?)refs[0].Attribute("Version")!="["+version+"]" ||
       d.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Single pinned NuGet distribution required: "+csproj);
    var feeds=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!feeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
       feeds.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("No external NuGet feeds allowed: "+config);
}
EnsurePinned(root,Client,ClientConfig,Version);
Required(root,"dotnet","restore",Client,"--source",feed,"--configfile",ClientConfig,
    "--packages",Path.Combine(root,"build","artifacts","m5-w02-c02-t2-isolated"));
Required(root,"dotnet","build",Client,"-c","Release","--no-restore");
var clientArgs=new List<string>{"run","--project",Client,"-c","Release","--no-build","--no-restore","--"};
clientArgs.AddRange(archives);
var clientOutput=Required(root,"dotnet",clientArgs.ToArray());
if(!HasExactLine(clientOutput,"dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=8"))
    throw new InvalidOperationException("Eight independent public API NuGet-only probes not observed.");
var surfaceHashes=new List<JsonNode>();
foreach(var item in packages)
{
    var digest=ReadSurface(clientOutput,item.Id,item.Version);
    surfaceHashes.Add(new JsonObject{
        ["id"]=item.Id,["version"]=item.Version,
        ["public_api_sha256"]=digest.Public,
        ["managed_assembly_sha256"]=digest.Assembly
    });
}
EnsurePinned(root,Clean,CleanConfig,Version);
// The retained consumer accepts the exact current T2 local feed.
// Reuse the already inspected archive paths without copying files or
// creating untracked directories; retain byte-for-byte identity.
Required(root,"dotnet","restore",Clean,"--source",feed,"--configfile",CleanConfig,
    "--packages",Path.Combine(root,"build","artifacts","m5-w02-c02-t2-old-clean"));
Required(root,"dotnet","build",Clean,"-c","Release","--no-restore");
var oldArgs=new List<string>{"run","--project",Clean,"-c","Release","--no-build","--no-restore","--"};
oldArgs.AddRange(archives);
var oldOutput=Required(root,"dotnet",oldArgs.ToArray());
if(!HasExactLine(oldOutput,"dw.tools.math/m5-clean-package-consumer/1.0 qualified; probes=8"))
    throw new InvalidOperationException("Retained eight-probe clean NuGet consumer regressed.");
foreach(var item in packages)
{
    var oldSha=ReadProductReceipt(oldOutput,item.Id,item.Version,
        "M5-W02-C02-T1-CONSUMER-SHA256:");
    var current=archiveReceipts[item.Id];
    if(oldSha.Bytes!=current.Bytes || !string.Equals(oldSha.Sha,current.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("Clean NuGet consumer archive hash diverged: "+item.Id);
}
EnsurePinned(root,Sample,SampleConfig,Version);
Required(root,"dotnet","restore",Sample,"--source",feed,"--configfile",SampleConfig,
    "--packages",Path.Combine(root,"build","artifacts","m5-w02-c02-t2-old-sample"));
Required(root,"dotnet","build",Sample,"-c","Release","--no-restore");
var sampleOutput=Required(root,"dotnet","run","--project",Sample,"-c","Release",
    "--no-build","--no-restore");
if(!HasExactLine(sampleOutput,"dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes=8") ||
   !sampleOutput.Contains("\"derivative\":\"mul(2,x)\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"exact_value\":\"3\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"admitted_roots\":[\"-1\"]",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"excluded_candidates\":[\"1\"]",StringComparison.Ordinal))
    throw new InvalidOperationException("Original numeric/symbolic console regressed.");
if(!target)
{
    var science=new JsonObject{
        ["schema_version"]=1,["id"]="M5-W02-C02-T2-boundaries-qualified",
        ["status"]=ProofStatus,
        ["prior_clean_archive_evidence"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can bounded PE metadata verify seven real managed net10 API surfaces and native/RID refusals without inventing cross-version compatibility?",
        ["observation"]="One compiled missing-API RED, 36 GREEN oracles, seven local archives, eight independent NuGet surface probes and two retained eight-probe consumers passed on observed host.",
        ["decision"]="Admit current public API/managed DLL SHA only, reject foreign native assets and withhold untested operating-system, cross-version source/binary and portable SLA claims.",
        ["product_project"]=Project,["product_project_sha256"]=hashes[Project],
        ["manifest_source"]=Manifest,["manifest_source_sha256"]=hashes[Manifest],
        ["source_file"]=Source,["source_sha256"]=hashes[Source],
        ["green_tests"]=GreenTest,["green_tests_sha256"]=hashes[GreenTest],
        ["retained_t1_tests"]=RetainedTests,
        ["retained_t1_tests_sha256"]=hashes[RetainedTests],
        ["retained_t1_tests_passed"]=40,
        ["retained_clean_consumer_source"]=CleanCode,
        ["retained_clean_consumer_source_sha256"]=hashes[CleanCode],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["support_matrix_document"]=Matrix,["support_matrix_sha256"]=hashes[Matrix],
        ["independent_green_tests_passed"]=36,
        ["red_recovered_and_green_tests_passed"]=37,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["retained_clean_consumer_probes_passed"]=8,
        ["retained_sample_probes_passed"]=8,
        ["inspected_local_archives"]=7,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["current_managed_public_api_sha256"]=p.Json.Array(surfaceHashes.ToArray()),
        ["local_distribution_version"]=Version,
        ["managed_il_only_qualified"]=true,
        ["foreign_native_assets_refused"]=true,
        ["cross_version_source_compatibility_certified"]=false,
        ["cross_version_binary_compatibility_certified"]=false,
        ["other_platform_rids_certified"]=false,
        ["portable_performance_sla_certified"]=false,
        ["qualified_rid_matrix"]=new JsonArray(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["public_nuget_published"]=false,
        ["stable_release_certified"]=false,
        ["m5_w02_c02_t2_done"]=true,
        ["m5_w02_c02_phase_done"]=true,
        ["m5_w02_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["next_accepted_scope"]="M5-W02-C03-T1: evidence-linked release candidate gate; no unauthorized public release"
    };
    p.Files.WriteComplete(Proof,
        science.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.76";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==C);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==T);
        if((string?)phase["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("Accepted C02-T2 baseline changed.");
        phase["status"]="done";
        phase["refinement"]="RS076 T1 qualified seven actual local net10 packages, SHA-256, embedded documentation and clean NuGet consumers. RS077 T2 independently verifies current managed public PE API signatures, DLL digests, native/RID asset refusals and truthful unqualified source/binary compatibility and OS support claims. One compiled RED, 36 GREEN tests, seven packages and three isolated eight-probe NuGet clients complete C02 only. C03, M5-W02 and M5 remain open.";
        phase["readiness"]="C02 managed local package qualification complete; current public API/assembly SHA observed, foreign native assets refused, unknown RIDs and cross-version ABI not certified";
        var paths=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(paths.Concat(new[]{
            Project,Manifest,Source,GreenTest,RetainedTests,RedTest,Client,ClientCode,
            ClientConfig,Clean,CleanCode,Sample,Readme,Notices,Decision,Matrix,RedProof,Proof
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W02C02T2",
            "dotnet test "+Test+" -c Release");
        var evid=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evid.Concat(new[]{RedProof,Proof})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Proof);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(
                (string?)sub["id"]==R?RedProof:Proof);
        }
    });
    p.ProjectPlan.TransitionNode(T,"not-ready","ready");
    p.ProjectPlan.TransitionNode(R,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(R);
    p.ProjectPlan.ConvergeNodeToDone(R);
    foreach(var node in new[]{G,V})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(T);
    p.ProjectPlan.ConvergeNodeToDone(C);
}
p.ProjectPlan.RequireNodeState(M,"in-progress");
p.ProjectPlan.RequireNodeState(W,"in-progress");
p.ProjectPlan.RequireNodeState(C,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C02-T1","done");
foreach(var id in new[]{T,R,G,V})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS077" ||
       (string?)state["activeMilestone"]!=M ||
       (string?)state["activeWorkPackage"]!=W ||
       (string?)state["activePhase"]!=(target?null:C) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS077 original active-phase cursor drifted.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

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
