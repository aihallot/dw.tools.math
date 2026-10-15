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

const string Work="M4-W02",Phase="M4-W02-C03",Task="M4-W02-C03-T2";
const string Red="M4-W02-C03-T2-R",Green="M4-W02-C03-T2-G",Verify="M4-W02-C03-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M4-W02-C03-T1-contract-qualified.json";
const string C01Formal="docs/planning/evidence/M4-W02-C01-T1-contract-qualified.json";
const string C01Point="docs/planning/evidence/M4-W02-C01-T2-boundaries-qualified.json";
const string C02Solve="docs/planning/evidence/M4-W02-C02-T1-contract-qualified.json";
const string C02Audit="docs/planning/evidence/M4-W02-C02-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M4-W02-C03-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M4-W02-C03-T2-boundaries-qualified.json";
const string Aggregate="docs/planning/evidence/M4-W02-work-package-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicRealCalculusBoundaries.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W02C03T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W02C03T2Tests.cs";
const string Decision="docs/planning/decisions/m4-w02-c03-t2-calculus-boundaries.md";
const string Document="docs/distribution/symbolic-calculus-boundaries.md";
const string ConsumerProject="tests/consumers/m4-w02-c03-t2/m4-calculus-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w02-c03-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w02-c03-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS064/staged/checks/m4-w02-c03-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="0.5.0-preview.10";
const string RedMarker="M4-W02-C03-T2 RED: original-domain integral and limit boundary APIs are absent.";
const string ConsumerMarker="dw.tools.math/m4-calculus-boundary-consumer/0.5 qualified; probes=8";
const int IndependentGreen=51,TotalTargeted=52;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS064","M4",target?null:Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS063",1,
    "feat: qualify exact real polynomial primitives and finite limits",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(C01Formal,"/status",
    JsonValue.Create("real-exact-formal-polynomial-differentiation-and-explicit-unit-refusal-qualified"));
p.Require.JsonValueEquals(C01Point,"/status",
    JsonValue.Create("real-exact-pointwise-derivative-and-domain-refusal-qualified"));
p.Require.JsonValueEquals(C02Solve,"/status",
    JsonValue.Create("real-exact-elementary-equation-and-ray-solving-qualified"));
p.Require.JsonValueEquals(C02Audit,"/status",
    JsonValue.Create("original-real-equation-root-and-multiplicity-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-real-polynomial-antiderivative-integral-and-finite-limit-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(50));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(51));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("0.5.0-preview.9"));
p.Require.JsonValueEquals(Prior,"/m4_w02_c03_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m4_w02_c03_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m4_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/arbitrary_additive_constant_explicit",JsonValue.Create(true));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.62"),JsonValue.Create("0.1.63"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.63":"0.1.62"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/31/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/32/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/32/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/32/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/32/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",JsonValue.Create("satisfied"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W02-C01","done");
p.ProjectPlan.RequireNodeState("M4-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W02-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified,Aggregate})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.9</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-real-calculus-boundary-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("real-calculus-original-domain-pole-hole-and-unknown-convergence-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m4_w02_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m4_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Aggregate,"/status",
        JsonValue.Create("exact-symbolic-m4-w02-work-package-qualified"));
    p.Require.JsonValueEquals(Aggregate,"/m4_release_done",JsonValue.Create(false));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.10</Version>");
}
var stagedHashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
stagedHashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing typed original-domain boundary RED source hash.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W02C03T2RedTests.IndependentRealCalculusBoundaryContractMustExist",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "The compiled absent public calculus boundary API RED was not observed: "+Relevant(red.Output));
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C03-T2-contract-red",
        ["status"]="compiled-real-calculus-boundary-contract-absent-red-observed",
        ["prior_exact_polynomial_calculus"]=Prior,
        ["compiled_source_product"]="dw.tools.math.symbolics/0.5.0-preview.9",
        ["red_test"]=RedTest,["red_test_sha256"]=stagedHashes[RedTest],
        ["missing_api"]="SymbolicRealCalculusBoundaries.AnalyzeIntegral(IrNode,IrSymbol,ExactRational,ExactRational);AnalyzeLimit(IrNode,IrSymbol,ExactRational)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_did_not_substitute_for_red"]=true,
        ["nonclaim"]="An absent public API does not itself prove convergence or integrability."
    };
    p.Files.WriteComplete(RedProof,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Project,Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedHashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing typed staged source/target hash for "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W02C03T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 52",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 52",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected one recovered RED and 51 independent GREEN real calculus boundary tests.");
Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regression in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var output=Required(root,"dotnet","test",regression,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!output.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Exact IR/quantities regression not observed: "+regression);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,Project,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-w02-c03-t2-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Expected local .NET product archive absent: "+package.Id);
    archives.Add(archive);
}
var helperArguments=new List<string>{"run","--file",PackageHelper,"--"};
helperArguments.AddRange(archives);
var archiveOutput=Required(root,"dotnet",helperArguments.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(archiveOutput,package.Id,package.Version);
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
var refs=consumerXml.Descendants().Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(refs.Length!=1||
   (string?)refs[0].Attribute("Include")!="dw.tools.math.symbolics"||
   (string?)refs[0].Attribute("Version")!="["+Version+"]"||
   consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Independent consumer must use one pinned symbolics NuGet dependency.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig)).Descendants()
    .Single(e=>e.Name.LocalName=="packageSources");
if(!sources.Elements().Any(e=>e.Name.LocalName=="clear")||
   sources.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("The NuGet-only calculus-boundary consumer must clear external sources.");
var isolated=Path.Combine(root,"build","artifacts","m4-w02-c03-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight NuGet-only calculus-boundary oracles not observed.");


if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C03-T2-boundaries-qualified",
        ["status"]="real-calculus-original-domain-pole-hole-and-unknown-convergence-qualified",
        ["prior_exact_polynomial_calculus"]=Prior,
        ["controlled_red_evidence"]=RedProof,
        ["question"]="Can exact real bounded symbolic calculus classify simple rational poles, distinguish removable original-domain holes, qualify finite removable limits, and keep unproved improper convergence unknown while preserving additive integration constants?",
        ["observation"]="One compiled missing-public-API RED recovered, 51 independent exact/boundary GREEN tests and eight NuGet-only consumer oracles demonstrate finite polynomial integrals, original-domain hole exclusion and limited exact rational limits without binary64.",
        ["decision"]="Admit the previously qualified polynomial rules plus two structural quotient families: k/(x-c) and the existing (x^2-c^2)/(x-c) bridge, with explicit pole/hole/unknown outcomes and exact only on proven domain-safe intervals.",
        ["new_source"]=Product,["new_source_sha256"]=stagedHashes[Product],
        ["product_project"]=Project,["product_project_sha256"]=stagedHashes[Project],
        ["new_green_tests"]=GreenTests,["new_green_tests_sha256"]=stagedHashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=stagedHashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=stagedHashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=stagedHashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=TotalTargeted,
        ["compiled_red_observed"]=true,["independent_consumer_probes_passed"]=8,
        ["simple_pole_integral_crossing_point_claims_finite_value"]=false,
        ["original_quotient_hole_distinguished_from_simple_pole"]=true,
        ["exact_removable_limit_with_undefined_original_function_value"]=true,
        ["hole_free_rational_bridge_integral_two_three"]="7/2",
        ["unqualified_quotient_or_improper_convergence_returns_unknown"]=true,
        ["arbitrary_additive_constant_preserved_for_admitted_integrals"]=true,
        ["no_general_numerical_quadrature"]=true,
        ["maximum_exact_rational_bits"]=128,
        ["nested_dimensional_values_explicitly_refused"]=true,
        ["assumptions_not_discarded"]=true,
        ["no_binary64_promoted_to_exact"]=true,
        ["original_source_semantic_hash_and_variable_identity_recorded"]=true,
        ["no_external_cas_or_agent_dependency"]=true,
        ["complete_symbolics_test_suite_passed"]=true,
        ["relevant_ir_quantities_regressions_passed"]=true,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["product_package_manifest_sha_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=stagedHashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=stagedHashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=stagedHashes[ConsumerConfig],
        ["consumer_marker"]=ConsumerMarker,
        ["packages_publicly_published"]=false,
        ["locked_solution_build_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["m4_w02_c03_t2_done"]=true,
        ["m4_w02_c03_phase_done"]=true,
        ["m4_w02_work_package_done"]=true,
        ["m4_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4 scientific release gate G-M4: inspect all accepted M4 chunks, cross-consumer evidence, nonclaims and release promotion separately",
        ["nonclaims"]=p.Json.StringArray(
            "No general CAS, logarithmic antiderivatives, numerical quadrature, complex branch or arbitrary improper convergence claims.",
            "No silent domain-hole elimination; the exact removable limit never becomes the value of the original undefined function.",
            "No general analytic real/complex calculus, infinite/asymptotic limit or hard deadline guarantees.",
            "No M4 release gate promotion, public NuGet release, AURA or sibling repository integration.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    var aggregate=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M4-W02",
        ["status"]="exact-symbolic-m4-w02-work-package-qualified",
        ["qualified_chains"]=p.Json.StringArray(C01Formal,C01Point,C02Solve,C02Audit,Prior,Qualified),
        ["original_domain_and_assumption_policy_preserved"]=true,
        ["exact_vs_unknown_convergence_kept_distinct"]=true,
        ["compiled_red_recovered_for_c03_t2"]=true,
        ["independent_green_tests_for_c03_t2"]=IndependentGreen,
        ["nuget_only_consumer_probes_for_c03_t2"]=8,
        ["last_symbolics_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["m4_w02_c01_done"]=true,
        ["m4_w02_c02_done"]=true,
        ["m4_w02_c03_done"]=true,
        ["m4_w02_work_package_done"]=true,
        ["m4_release_done"]=false,
        ["scope"]="Exact real derivative, elementary solving and qualified bounded integral/limit rules; no general CAS.",
        ["next_gate"]="G-M4 remains separate and unpromoted"
    };
    p.Files.WriteComplete(Aggregate,
        aggregate.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.63";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        var wp=plan["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned" ||
           (string?)wp["status"]!="in_progress")
            throw new InvalidOperationException("M4-W02 C03/T2/work-package baseline diverged.");
        chunk["status"]="done";
        chunk["refinement"]="RS063 T1 qualified degree-seven exact real polynomial primitives and finite limits with +C. RS064 T2 adds conservative exact simple-pole and removable-original-hole classification over k/(x-c) and the existing (x^2-c^2)/(x-c) bridge. Exact results only on certified domain-safe intervals or finite point limits; unqualified logarithmic/improper convergence stays unknown and no numerical result is promoted. Compiled missing-API RED, 51 GREEN, eight isolated NuGet-only oracles, local product SHA-256 inspection and relevant regressions close C03 and M4-W02. M4 release gate remains independent.";
        chunk["readiness"]="closed: T1 polynomial exact calculus and T2 original-domain singularity/unknown convergence boundaries qualified; M4-W02 complete, M4 gate outstanding";
        var priorFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Project,Product,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified,Aggregate
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W02C03T2",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var priorEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
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
        wp["status"]="done";
        wp["evidence"]=p.Json.StringArray(
            C01Formal,C01Point,C02Solve,C02Audit,Prior,Qualified,Aggregate);
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
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState("M4-W02-C01","done");
p.ProjectPlan.RequireNodeState("M4-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M4-W02-C03-T1","done");
foreach(var child in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS064" ||
       (string?)state["activeMilestone"]!="M4" ||
       (string?)state["activeWorkPackage"]!=(target?null:Work) ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS064 exact before/after M4-W02 active cursor diverged.");
    state["activeWorkPackage"]=null;
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
    var prefix="M4-W02-C03-T2-PKG-SHA256:"+id+":"+version+":";
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
