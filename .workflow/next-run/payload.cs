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

const string Milestone="M5",Work="M5-W02",Phase="M5-W02-C02",Task="M5-W02-C02-T1";
const string Red="M5-W02-C02-T1-R",Green="M5-W02-C02-T1-G",Verify="M5-W02-C02-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M5-W02-C02-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json";
const string QualifiedStatus="local-versioned-net10-package-graph-clean-consumer-and-notices-qualified";
const string Product="src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj";
const string Source="src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs";
const string TestProject="tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.distribution.tests/M5W02C02T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs";
const string ConsumerProject="tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w02-c02-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w02-c02-t1/NuGet.Config";
const string SampleProject="samples/m5-exact-vertical/m5-exact-vertical.csproj";
const string SampleConfig="samples/m5-exact-vertical/NuGet.Config";
const string Readme="docs/distribution/m5-package-readme.md";
const string Notices="docs/distribution/m5-package-notices.md";
const string Decision="docs/planning/decisions/m5-w02-c02-t1-clean-packages.md";
const string Matrix="docs/distribution/m5-local-support-evidence.md";
const string PackageHelper=".workflow/runs/RS076/staged/checks/m5-w02-c02-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.3";
const string RedMarker="M5-W02-C02-T1 RED: public bounded local NuGet qualification contract is absent.";
const string ConsumerMarker="dw.tools.math/m5-clean-package-consumer/1.0 qualified; probes=8";
const string SampleMarker="dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes=8";
const string ConsumerReceiptMarker="M5-W02-C02-T1-CONSUMER-SHA256:";
const int IndependentGreen=40,AllTargeted=41;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS076",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS075",1,
    "feat: qualify exact numeric-symbolic boundaries and close M5 W02 C01",
    "0.1.59",1,1,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-numeric-symbolic-original-domain-provenance-loss-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(41));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(42));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/retained_sample_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_distribution_version",JsonValue.Create("1.0.0-preview.2"));
p.Require.JsonValueEquals(Prior,"/m5_w02_c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.74"),JsonValue.Create("0.1.75"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.75":"0.1.74"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("in_progress"));
foreach(var index in new[]{33,34,35,36,37})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/38/tasks/0/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/39/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.3</Version>":
    "<Version>1.0.0-preview.2</Version>");
p.Require.TextContains(SampleProject,target?
    "Version=\"[1.0.0-preview.3]\"":
    "Version=\"[1.0.0-preview.2]\"");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,ConsumerProject,ConsumerSource,
        ConsumerConfig,Readme,Notices,Decision,Matrix,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-local-math-package-audit-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/retained_sample_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/local_distribution_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c02_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c02_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("The compiled package-audit RED source lacks SHA-256.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W02C02T1RedTests.IndependentBoundedLocalPackageAuditPublicContractMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A genuinely compiled missing local-package public API RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W02-C02-T1-contract-red",
        ["status"]="compiled-local-math-package-audit-absent-red-observed",
        ["prior_exact_public_vertical"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.distribution/1.0.0-preview.2",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="MathLocalReleaseQualification.Audit(string, byte[])",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="A missing archive inspection API does not certify actual NuGet bytes or operating systems."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,Source,GreenTests,ConsumerProject,ConsumerSource,
    ConsumerConfig,SampleProject,Readme,Notices,Decision,Matrix})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing Math local packaging asset SHA-256: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W02C02T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 41",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 41",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one recovered compiled RED and 40 independent nupkg GREEN oracles.");
Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj",
    "tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Qualified Math package consumer regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1","src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1","src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8","src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10","src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4","src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1",
        "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution",Version,Product,
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
        "dw.tools.math.symbolics","dw.tools.math.numerics","dw.tools.math.composition"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w02-c02-t1-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Local versioned net10 archive absent after build: "+package.Id);
    archives.Add(archive);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var inspected=Required(root,"dotnet",helperArgs.ToArray());
var products=new List<JsonNode>();
var expectedReceipts=new Dictionary<string,(long Bytes,string Sha)>(StringComparer.Ordinal);
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(inspected,package.Id,package.Version);
    expectedReceipts.Add(package.Id,receipt);
    products.Add(new JsonObject
    {
        ["id"]=package.Id,["version"]=package.Version,
        ["bytes"]=receipt.Bytes,["sha256"]=receipt.Sha,
        ["dependencies"]=p.Json.StringArray(package.DependsOn),
        ["assembly"]="lib/net10.0/"+package.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}
var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var references=consumerXml.Descendants().Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!="dw.tools.math.distribution" ||
   (string?)references[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Clean Math nupkg consumer must use exactly one pinned distribution PackageReference.");
var consumerFeeds=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(e=>e.Name.LocalName=="packageSources");
if(!consumerFeeds.Elements().Any(e=>e.Name.LocalName=="clear") ||
   consumerFeeds.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("External NuGet feeds are forbidden for clean consumer.");
var isolated=Path.Combine(root,"build","artifacts","m5-w02-c02-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var clientArgs=new List<string>{"run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore","--"};
clientArgs.AddRange(archives);
var clientOutput=Required(root,"dotnet",clientArgs.ToArray());
if(!HasExactLine(clientOutput,ConsumerMarker) ||
   !clientOutput.Contains("OBSERVED-RUNTIME:",StringComparison.Ordinal))
    throw new InvalidOperationException(
        "Eight independent Math clean NuGet consumer probes and host observation not present.");
foreach(var package in packages)
{
    var consumerReceipt=ReadProductReceipt(clientOutput,package.Id,
        package.Version,ConsumerReceiptMarker);
    var verified=expectedReceipts[package.Id];
    if(consumerReceipt.Bytes!=verified.Bytes ||
       !string.Equals(consumerReceipt.Sha,verified.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Archive bytes and SHA-256 disagree between independent inspector and package-only consumer: "+package.Id);
}
var sampleXml=XDocument.Load(Path.Combine(root,SampleProject));
var sampleRefs=sampleXml.Descendants().Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(sampleRefs.Length!=1 ||
   (string?)sampleRefs[0].Attribute("Include")!="dw.tools.math.distribution" ||
   (string?)sampleRefs[0].Attribute("Version")!="["+Version+"]" ||
   sampleXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Existing exact Math console must remain one pinned NuGet-only consumer.");
var sampleFeeds=XDocument.Load(Path.Combine(root,SampleConfig))
    .Descendants().Single(e=>e.Name.LocalName=="packageSources");
if(!sampleFeeds.Elements().Any(e=>e.Name.LocalName=="clear") ||
   sampleFeeds.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("Existing console must use local package feed only.");
var sampleIsolated=Path.Combine(root,"build","artifacts","m5-w02-c02-t1-sample-packages");
Required(root,"dotnet","restore",SampleProject,"--source",feed,
    "--configfile",SampleConfig,"--packages",sampleIsolated);
Required(root,"dotnet","build",SampleProject,"-c","Release","--no-restore");
var sampleOutput=Required(root,"dotnet","run","--project",SampleProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(sampleOutput,SampleMarker) ||
   !sampleOutput.Contains("\"derivative\":\"mul(2,x)\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"exact_value\":\"3\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"admitted_roots\":[\"-1\"]",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"excluded_candidates\":[\"1\"]",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"original_root_audit_complete\":false",StringComparison.Ordinal))
    throw new InvalidOperationException(
        "Existing exact Math standalone sample semantics regressed under preview.3.");

if(!target)
{
    var observedRid=RuntimeInformation.RuntimeIdentifier;
    var sdk=Required(root,"dotnet","--version").Trim();
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W02-C02-T1-contract-qualified",
        ["status"]=QualifiedStatus,
        ["prior_exact_vertical_boundary"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Do seven actual versioned net10 local Math archives, embedded distribution README/NOTICE, an independent ZIP/nuget client and the original exact console qualify without machine-specific source or unobserved platform claims?",
        ["observation"]="One compiled missing archive-audit public API RED recovered, 40 independent GREEN tests, seven SHA-verified local archive checks, eight pinned package-only clean consumer oracles and the retained eight-probe exact console passed on the recorded local .NET host.",
        ["decision"]="Admit only locally verified seven-package net10 archives with real hashes, metadata, dependency IDs/versions and distribution notices. Refuse invalid archives. Record the observed RID without claiming other platforms, public NuGet or stable 1.0.",
        ["package_audit_source"]=Source,
        ["package_audit_source_sha256"]=hashes[Source],
        ["product_project"]=Product,
        ["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=hashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["sample_project"]=SampleProject,
        ["sample_project_sha256"]=hashes[SampleProject],
        ["package_readme"]=Readme,["package_readme_sha256"]=hashes[Readme],
        ["package_notices"]=Notices,["package_notices_sha256"]=hashes[Notices],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["support_matrix_document"]=Matrix,["support_matrix_sha256"]=hashes[Matrix],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["isolated_consumer_probes_passed"]=8,
        ["retained_sample_probes_passed"]=8,
        ["inspected_local_archives"]=7,
        ["clean_consumer_source_project_reference_count"]=0,
        ["archive_qualification_rule"]="dw-math-local-package-archive-net10/1",
        ["distribution_package_embeds_readme_and_notice"]=true,
        ["distribution_readme_is_not_a_license_grant"]=true,
        ["actual_archive_bytes_and_sha256_cross_checked"]=true,
        ["qualified_target_framework"]="net10.0",
        ["qualified_rid_matrix"]=p.Json.StringArray(),
        ["multi_os_release_certified"]=false,
        ["public_nuget_published"]=false,
        ["stable_release_certified"]=false,
        ["native_or_machine_path_certification_claimed"]=false,
        ["local_distribution_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["archive_manifest_sha_inspector"]=PackageHelper,
        ["consumer_marker"]=ConsumerMarker,
        ["sample_marker"]=SampleMarker,
        ["observed_runtime_identifier"]=observedRid,
        ["dotnet_sdk"]=sdk,
        ["m5_w02_c02_t1_done"]=true,
        ["m5_w02_c02_phase_done"]=false,
        ["m5_w02_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["next_accepted_scope"]="M5-W02-C02-T2: independent source/binary compatibility, platform/RID, native dependency, performance and explicit support matrix boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "A declared manifest is not actual archive evidence; only local generated inspected nupkg bytes and restores are qualified.",
            "No claim of Linux/macOS certification, unobserved RIDs, public CI or stable/public NuGet release.",
            "No general package-signing, licensing or complete source/binary ABI promise; do not treat README/NOTICE as ownership proof.",
            "Only C02-T1 completes; C02-T2, C02, M5-W02, M5 and G-M5 remain outstanding.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.75";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5-W02-C02-T1 accepted baseline changed.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS076 T1 qualifies actual local versioned .NET 10 Math nupkg archives and a separate public BCL ZIP inspector, embeds distribution README and rights notices, and verifies all seven archive digests and dependency graphs against an independent clean pinned NuGet consumer. It preserves the prior exact console through its eight original probes, records local host/SDK, and explicitly refuses to certify unobserved OS RIDs, public releases or source/binary ABIs. One compiled absent-API RED, 40 GREEN MSTests, eight package-only probes and seven archive checks qualify only T1. T2 owns cross-platform, native, API/ABI, performance and final support matrix; C02, W02 and M5 remain in-progress.";
        chunk["readiness"]="C02-T1 qualified local archive graph, one pinned clean NuGet consumer, retained Math console, distribution README/NOTICE and observed host only; T2 source/binary/platform support boundaries remain pending";
        chunk["files"]=p.Json.StringArray(Product,Source,RedTest,GreenTests,
            ConsumerProject,ConsumerSource,ConsumerConfig,SampleProject,
            Readme,Notices,Decision,Matrix,RedProof,Qualified);
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M5W02C02T1",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release",
            "dotnet run --project "+SampleProject+" -c Release");
        chunk["evidence"]=p.Json.StringArray(RedProof,Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var child in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS076" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS076 local nupkg release qualification phase start cursor diverged.");
    state["activePhase"]=Phase;
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
    string marker="M5-W02-C02-T1-PKG-SHA256:")
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
