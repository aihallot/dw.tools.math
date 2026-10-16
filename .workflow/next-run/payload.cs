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

const string Milestone="M5",Work="M5-W02",Phase="M5-W02-C01",Task="M5-W02-C01-T2";
const string Red="M5-W02-C01-T2-R",Green="M5-W02-C01-T2-G",Verify="M5-W02-C01-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W02-C01-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json";
const string QualifiedStatus="exact-numeric-symbolic-original-domain-provenance-loss-boundaries-qualified";
const string Product="src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj";
const string Source="src/projects/dw.tools.math.distribution/ExactVerticalBoundaryAudit.cs";
const string TestProject="tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.distribution.tests/M5W02C01T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.distribution.tests/M5W02C01T2Tests.cs";
const string ConsumerProject="tests/consumers/m5-w02-c01-t2/m5-exact-vertical-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w02-c01-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w02-c01-t2/NuGet.Config";
const string SampleProject="samples/m5-exact-vertical/m5-exact-vertical.csproj";
const string SampleSource="samples/m5-exact-vertical/Program.cs";
const string SampleConfig="samples/m5-exact-vertical/NuGet.Config";
const string Decision="docs/planning/decisions/m5-w02-c01-t2-public-vertical-boundaries.md";
const string Document="docs/distribution/m5-exact-console-boundaries.md";
const string PackageHelper=".workflow/runs/RS075/staged/checks/m5-w02-c01-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.2";
const string RedMarker="M5-W02-C01-T2 RED: independent numerical/symbolic provenance and loss audit API is absent.";
const string ConsumerMarker="dw.tools.math/m5-exact-vertical-boundary-consumer/1.0 qualified; probes=8";
const string SampleMarker="dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes=8";
const int IndependentGreen=41,AllTargeted=42;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS075",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS074",1,
    "feat: qualify exact numerical-symbolic console vertical and open M5 W02",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-numeric-symbolic-public-console-composition-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(34));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(35));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/new_local_distribution_package_version",
    JsonValue.Create("1.0.0-preview.1"));
p.Require.JsonValueEquals(Prior,"/original_root_audit_is_globally_complete",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w02_c01_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w02_c01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.73"),JsonValue.Create("0.1.74"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.74":"0.1.73"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",
    JsonValue.Create("in_progress"));
foreach(var index in new[]{33,34,35,36})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/37/tasks/1/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.2</Version>":
    "<Version>1.0.0-preview.1</Version>");
p.Require.TextContains(SampleProject,target?
    "Version=\"[1.0.0-preview.2]\"":
    "Version=\"[1.0.0-preview.1]\"");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W02-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,ConsumerProject,
        ConsumerSource,ConsumerConfig,Decision,Document,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-public-numeric-symbolic-boundary-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/retained_sample_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c01_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c01_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("The exact vertical boundary RED source hash is absent.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W02C01T2RedTests.PublicIndependentOriginalDomainAndNumericLossAuditMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A compiled missing public independent audit RED must be observed: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W02-C01-T2-contract-red",
        ["status"]="compiled-public-numeric-symbolic-boundary-api-absent-red-observed",
        ["prior_exact_vertical_contract"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.distribution/1.0.0-preview.1",
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="ExactVerticalBoundaryAudit.Audit(string, ExactRational, IrNode?, RationalBinary64Policy)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="A missing public audit API is not proof of exact mathematical or scientific integration."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,Source,GreenTests,ConsumerProject,
    ConsumerSource,ConsumerConfig,SampleProject,Decision,Document})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing independent Math vertical boundary file SHA-256: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W02C01T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 42",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 42",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one recovered compiled RED and 41 independent exact vertical boundary GREEN oracles.");
Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Exact Math vertical component regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8",
        "src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10",
        "src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",
        "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1",
        "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",
        ["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution",Version,Product,
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
        "dw.tools.math.symbolics","dw.tools.math.numerics","dw.tools.math.composition"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w02-c01-t2-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Required generated exact vertical NuGet product archive missing: "+package.Id);
    archives.Add(archive);
}
var inspectorArgs=new List<string>{"run","--file",PackageHelper,"--"};
inspectorArgs.AddRange(archives);
var packageInspectionOutput=Required(root,"dotnet",inspectorArgs.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(packageInspectionOutput,package.Id,package.Version);
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
var references=consumerXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!="dw.tools.math.distribution" ||
   (string?)references[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Independent boundary client must pin one local distribution NuGet package.");
var consumerFeeds=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!consumerFeeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
   consumerFeeds.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("Boundary client cannot restore external feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w02-c01-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent pinned NuGet exact vertical boundary probes not observed.");

var sampleXml=XDocument.Load(Path.Combine(root,SampleProject));
var sampleReferences=sampleXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(sampleReferences.Length!=1 ||
   (string?)sampleReferences[0].Attribute("Include")!="dw.tools.math.distribution" ||
   (string?)sampleReferences[0].Attribute("Version")!="["+Version+"]" ||
   sampleXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Retained console sample must also use only the pinned preview.2 NuGet product.");
var sampleFeeds=XDocument.Load(Path.Combine(root,SampleConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!sampleFeeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
   sampleFeeds.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("Retained sample cannot restore external feeds.");
var sampleIsolated=Path.Combine(root,"build","artifacts","m5-w02-c01-t2-sample-packages");
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
        "Existing eight-probe NuGet console original-domain contract did not survive preview.2.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W02-C01-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_exact_vertical"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can a separate qualified .NET numerical/symbolic audit reproduce original-domain, derivative, exact-value, root, provider and numeric-loss evidence while refusing unsupported or incompatible pipelines without a CAS or AURA?",
        ["observation"]="One compiled absent-API RED recovered, 41 independent GREEN boundary tests, eight NuGet-only boundary probes, the original eight-probe console and seven SHA-inspected local Math packages qualify explicit original-IR mathematical evidence and refusals.",
        ["decision"]="Reuse exact Math engines to independently verify typed source IR JSON and structural hash, formal-body derivative, admissible-domain point, original restricted root proof, full unrestricted candidates and explicit binary64 loss; refuse ambiguous pipelines.",
        ["source_file"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["consumer_project"]=ConsumerProject,["consumer_project_sha256"]=hashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["sample_project"]=SampleProject,["sample_project_sha256"]=hashes[SampleProject],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["retained_sample_probes_passed"]=8,
        ["qualified_boundary_rule"]="m5-exact-numeric-symbolic-vertical-boundary/1",
        ["original_source_sha256_is_not_mathematical_proof"]=true,
        ["canonical_original_ir_json_and_semantic_hash_rechecked"]=true,
        ["formal_body_derivative_separated_from_original_root_equation"]=true,
        ["complete_unrestricted_candidates_not_original_completeness"]=true,
        ["original_domain_exclusions_checked_before_evaluation"]=true,
        ["explicit_numeric_rounding_policy_and_exact_absolute_error_checked"]=true,
        ["typed_unsupported_and_pipeline_refusals_return_no_partial_result"]=true,
        ["provider_and_assumption_provenance_requalified"]=true,
        ["no_aura_or_external_cas_required"]=true,
        ["local_distribution_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["product_package_manifest_sha_helper"]=PackageHelper,
        ["consumer_marker"]=ConsumerMarker,
        ["sample_marker"]=SampleMarker,
        ["packages_publicly_published"]=false,
        ["m5_w02_c01_t2_done"]=true,
        ["m5_w02_c01_phase_done"]=true,
        ["m5_w02_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W02-C02-T1: independent clean NuGet packaging and support matrix for the proven scope; no public 1.0 release without owner authorization",
        ["nonclaims"]=p.Json.StringArray(
            "No arbitrary symbolic computation, complete original-root proof from an unrestricted candidate set or derivative-as-solving equivalence.",
            "No silent binary64 rounding, source SHA as mathematical proof, unqualified provider or assumptions.",
            "No AURA/Decision/MCDM integration, foreign CAS, public stable NuGet or cross-OS platform certification.",
            "C01 only closes; M5-W02 and M5 remain open and C02/C03 await their own qualification.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.74";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5-W02-C01 T2 accepted baseline changed.");
        chunk["status"]="done";
        chunk["refinement"]="RS074 T1 qualified the exact source-bound numerical/symbolic public console: original x!=1 IR, body derivative 2x, point value 3 at x=2, unrestricted roots -1/1 with original excluded 1, explicit exact/binary64 composition, independent local NuGet sample and seven archives. RS075 T2 independently rechecks source IR structural identity, provider/assumptions, formal derivative versus solving, exact point proof, original root audit versus unrestricted candidate completeness, precise binary64 loss and typed refusal boundaries. One compiled RED, 41 GREEN oracles, eight new isolated NuGet boundary probes, the retained eight-probe console and seven SHA-inspected local packages complete C01 only. M5-W02 and M5 remain open.";
        chunk["readiness"]="C01 complete with T1 exact console composition and T2 independent original-domain/pipeline/provenance/numeric-loss boundaries; C02 clean packaging matrix and C03 release gate remain pending";
        var priorFiles=chunk["files"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,ConsumerProject,ConsumerSource,
            ConsumerConfig,SampleProject,Decision,Document,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M5W02C01T2",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release",
            "dotnet run --project "+SampleProject+" -c Release");
        var priorEvidence=chunk["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(priorEvidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });
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
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C01-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS075" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS075 exact public vertical phase close cursor diverged.");
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
    string output,string id,string version)
{
    var prefix="M5-W02-C01-T2-PKG-SHA256:"+id+":"+version+":";
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
