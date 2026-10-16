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

const string Milestone="M5",Work="M5-W02",Phase="M5-W02-C01",Task="M5-W02-C01-T1";
const string Red="M5-W02-C01-T1-R",Green="M5-W02-C01-T1-G",Verify="M5-W02-C01-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string Last="docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M5-W02-C01-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json";
const string QualifiedStatus="exact-numeric-symbolic-public-console-composition-qualified";
const string Project="src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj";
const string ProductLock="src/projects/dw.tools.math.distribution/packages.lock.json";
const string Contract="src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs";
const string Source="src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs";
const string TestProject="tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.distribution.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.distribution.tests/M5W02C01T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.distribution.tests/M5W02C01T1Tests.cs";
const string SampleProject="samples/m5-exact-vertical/m5-exact-vertical.csproj";
const string SampleSource="samples/m5-exact-vertical/Program.cs";
const string SampleConfig="samples/m5-exact-vertical/NuGet.Config";
const string Decision="docs/planning/decisions/m5-w02-c01-t1-public-vertical.md";
const string Document="docs/distribution/m5-exact-console-vertical.md";
const string PackageHelper=".workflow/runs/RS074/staged/checks/m5-w02-c01-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.1";
const string RedMarker="M5-W02-C01-T1 RED: public exact numerical and symbolic vertical runner is absent.";
const string ConsumerMarker="dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes=8";
const int IndependentGreen=34,AllTargeted=35;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS074",Milestone,target?Work:null,
    target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS073",1,
    "feat: qualify exact math display boundaries and close M5 W01",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Last,"/status",
    JsonValue.Create("exact-office-omml-markdown-unicode-loss-and-format-boundaries-qualified"));
p.Require.JsonValueEquals(Last,"/independent_green_tests_passed",JsonValue.Create(38));
p.Require.JsonValueEquals(Last,"/red_recovered_and_green_tests_passed",JsonValue.Create(39));
p.Require.JsonValueEquals(Last,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Last,"/local_product_version",JsonValue.Create("1.0.0-preview.8"));
p.Require.JsonValueEquals(Last,"/m5_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m5-w01-exact-math-codecs-interchange-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/last_codecs_version",JsonValue.Create("1.0.0-preview.8"));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.72"),JsonValue.Create("0.1.73"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.73":"0.1.72"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",
    JsonValue.Create(target?"in_progress":"planned"));
foreach(var index in new[]{33,34,35,36})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/37/tasks/0/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/38/status",JsonValue.Create("planned"));

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");

if(!target)
{
    foreach(var file in new[]{Project,ProductLock,Contract,Source,
        TestProject,TestLock,RedTest,GreenTests,SampleProject,
        SampleSource,SampleConfig,Decision,Document,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Solution,
        "<Project Path=\"src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj\" />");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-public-math-vertical-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/compiled_red_observed",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c01_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_c01_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Project,ProductLock,Contract,TestProject,TestLock,
    RedTest,Solution})
{
    var prepared=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=prepared.AfterSha256 ??
        throw new InvalidOperationException("Missing source hash for Math vertical RED prerequisite: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W02C01T1RedTests.PublicSourceBoundNumericalAndSymbolicVerticalMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A compiled missing-public-Math-vertical-runner RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W02-C01-T1-contract-red",
        ["status"]="compiled-public-math-vertical-api-absent-red-observed",
        ["prior_codecs_work_package"]=Prior,
        ["compiled_red_project"]=Project,
        ["compiled_red_contract"]=Contract,
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="ExactMathVerticalRunner.Run canonical mathematical source through original-domain roots, formal derivative, exact evaluation, numeric conversion, composition and replay",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="The missing vertical facade is not proof of the numerical or symbolic outcome."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Source,GreenTests,SampleProject,SampleSource,
    SampleConfig,Decision,Document})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing integrated Math vertical file SHA-256: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W02C01T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 35",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 35",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one compiled recovered RED and all 34 exact numerical/symbolic GREEN oracles.");

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
            "Qualified public vertical component regression did not pass: "+regressionProject);
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
    ("dw.tools.math.distribution",Version,Project,
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
        "dw.tools.math.symbolics","dw.tools.math.numerics","dw.tools.math.composition"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w02-c01-t1-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Required locally rebuilt Math vertical NuGet archive absent: "+package.Id);
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
var consumerXml=XDocument.Load(Path.Combine(root,SampleProject));
var references=consumerXml.Descendants()
    .Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!="dw.tools.math.distribution" ||
   (string?)references[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Independent console sample requires only one pinned distribution NuGet package.");
var consumerFeed=XDocument.Load(Path.Combine(root,SampleConfig))
    .Descendants().Single(e=>e.Name.LocalName=="packageSources");
if(!consumerFeed.Elements().Any(e=>e.Name.LocalName=="clear") ||
   consumerFeed.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("Math vertical sample cannot restore from external feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w02-c01-t1-isolated-packages");
Required(root,"dotnet","restore",SampleProject,"--source",feed,
    "--configfile",SampleConfig,"--packages",isolated);
Required(root,"dotnet","build",SampleProject,"-c","Release","--no-restore");
var sampleOutput=Required(root,"dotnet","run","--project",SampleProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(sampleOutput,ConsumerMarker) ||
   !sampleOutput.Contains("\"derivative\":\"mul(2,x)\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"exact_value\":\"3\"",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"admitted_roots\":[\"-1\"]",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"excluded_candidates\":[\"1\"]",StringComparison.Ordinal) ||
   !sampleOutput.Contains("\"original_root_audit_complete\":false",StringComparison.Ordinal))
    throw new InvalidOperationException(
        "Public pinned NuGet console did not independently prove exact derivative, value, original-domain exclusions and non-completeness.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W02-C01-T1-contract-qualified",
        ["status"]=QualifiedStatus,
        ["prior_qualified_codecs_work_package"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can an independent .NET 10 console compose qualified canonical parsing, original restricted Math IR, formal polynomial differentiation, exact point evaluation, root verification, explicit binary64 and typed composition without an agent or CAS?",
        ["observation"]="One compiled missing-public-runner RED recovered, 34 independent GREEN tests and eight standalone pinned NuGet console probes verify derivative 2x, evaluation x=2 -> 3, original-domain verified -1, excluded x=1 and rejected derivative-to-solver pipeline.",
        ["decision"]="Admit one explicit source-bound x²-c with x!=k vertical using established symbolic exact solving and original root audit, polynomial point continuity, explicit rational-to-binary64 conversion, typed composition and canonical IR replay; refuse unqualified semantics.",
        ["distribution_project"]=Project,
        ["distribution_project_sha256"]=hashes[Project],
        ["distribution_lock"]=ProductLock,
        ["distribution_lock_sha256"]=hashes[ProductLock],
        ["distribution_contract"]=Contract,
        ["distribution_contract_sha256"]=hashes[Contract],
        ["distribution_runner"]=Source,
        ["distribution_runner_sha256"]=hashes[Source],
        ["test_project"]=TestProject,
        ["test_project_sha256"]=hashes[TestProject],
        ["test_lock"]=TestLock,
        ["test_lock_sha256"]=hashes[TestLock],
        ["solution_file"]=Solution,
        ["solution_sha256"]=hashes[Solution],
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,
        ["green_tests_sha256"]=hashes[GreenTests],
        ["sample_project"]=SampleProject,
        ["sample_project_sha256"]=hashes[SampleProject],
        ["sample_source"]=SampleSource,
        ["sample_source_sha256"]=hashes[SampleSource],
        ["sample_config"]=SampleConfig,
        ["sample_config_sha256"]=hashes[SampleConfig],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["isolated_consumer_probes_passed"]=8,
        ["accepted_canonical_source"]="where(sub(square(x),1),ne(x,1))",
        ["exact_formal_derivative"]="mul(2,x)",
        ["exact_value_at_two"]="3",
        ["original_root_admitted"]="-1",
        ["original_root_excluded"]="1",
        ["original_root_audit_is_globally_complete"]=false,
        ["unrestricted_candidate_solver_is_complete"]=true,
        ["original_ir_replay_and_semantic_hash_retained"]=true,
        ["binary64_conversion_requires_explicit_rounding_policy"]=true,
        ["incompatible_derivative_to_solver_pipeline_refused"]=true,
        ["numeric_and_symbolic_provenance_exposed"]=true,
        ["no_aura_or_external_cas_required"]=true,
        ["new_local_distribution_package_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["product_package_manifest_sha_helper"]=PackageHelper,
        ["consumer_marker"]=ConsumerMarker,
        ["packages_publicly_published"]=false,
        ["m5_w02_c01_t1_done"]=true,
        ["m5_w02_c01_phase_done"]=false,
        ["m5_w02_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W02-C01-T2: independent numerical/symbolic public vertical provider, assumptions, losses, incompatible pipeline and no-AURA boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "Only a bounded original-domain rational quadratic vertical is admitted; no arbitrary symbolic computation, general CAS, infix grammar or external provider.",
            "Formal differentiation of the body is NOT solving the restricted original equation; an unrestricted candidate set does not prove complete original-domain root coverage.",
            "Binary64 projection is explicitly typed even when exact representable; rounding requires explicit consent and exact absolute error.",
            "No public/stable package, cross-OS matrix, sibling consumer adoption, M5-W02 closure or 1.0 release gate.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.73";
        var work=plan["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)work["status"]!="planned" ||
           (string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5-W02 C01/T1 exact accepted baseline diverged.");
        work["status"]="in_progress";
        chunk["status"]="in_progress";
        chunk["refinement"]="RS074 T1 delivers a standalone .NET 10 public console sample from one pinned NuGet distribution package, combining canonical source parsing, typed restricted Math IR, exact formal body derivative 2x, polynomial point value 3 at admissible x=2, complete unrestricted quadratic candidates and original-domain audited root -1 with excluded 1. It retains exact/explicit binary64 composition, original JSON replay, source SHA-256, provider and loss policy. Incompatible derivative-as-original-equation pipelines and unauthorized rounding are refused. One compiled absent-API RED, 34 independent GREEN tests, eight isolated NuGet console probes and seven independently inspected local packages qualify only C01 T1. T2 and all wider M5-W02 release gates remain open.";
        chunk["readiness"]="T1 exact public numerical/symbolic console and local package consumption qualified; T2 provider/assumption/loss boundaries still planned, C01/W02/M5 remain in-progress";
        chunk["files"]=p.Json.StringArray(Project,ProductLock,Contract,Source,
            TestProject,TestLock,RedTest,GreenTests,SampleProject,SampleSource,
            SampleConfig,Solution,Decision,Document,RedProof,Qualified);
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M5W02C01T1",
            "dotnet test "+TestProject+" -c Release",
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
    p.ProjectPlan.TransitionNode(Work,"not-ready","ready");
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
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS074" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=(target?Work:null) ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS074 starting new M5-W02 cursor diverged from declared baseline/target.");
    state["activeWorkPackage"]=Work;
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
    string output,string id,string version)
{
    var prefix="M5-W02-C01-T1-PKG-SHA256:"+id+":"+version+":";
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
