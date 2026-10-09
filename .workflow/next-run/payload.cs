using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M2-W01-C02";
const string Task = "M2-W01-C02-T2";
const string Red = "M2-W01-C02-T2-R";
const string Green = "M2-W01-C02-T2-G";
const string Verify = "M2-W01-C02-T2-V";
const string Model = "src/projects/dw.tools.math.ir/IrModel.cs";
const string Graph = "src/projects/dw.tools.math.ir/IrGraphLimits.cs";
const string Matrix = "src/projects/dw.tools.math.ir/IrMatrix.cs";
const string RedTests = "tests/projects/dw.tools.math.ir.tests/M2W01C02BoundaryRedTests.cs";
const string Tests = "tests/projects/dw.tools.math.ir.tests/M2W01C02BoundaryTests.cs";
const string TestProject = "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj";
const string RedEvidence = "docs/planning/evidence/M2-W01-C02-boundary-red.json";
const string GreenEvidence = "docs/planning/evidence/M2-W01-C02-boundary-qualified.json";
const string PriorEvidence = "docs/planning/evidence/M2-W01-C02-functional-green.json";
const string PackageVersion = "0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var product=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk=product["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
var parts=task["subtasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
string Sub(string id)=>(string?)parts.Single(x=>(string?)x["id"]==id)["status"]
    ?? throw new InvalidOperationException("Missing boundary subtask status.");
var baseline=(string?)product["plan_version"]=="0.1.27" &&
    (string?)chunk["status"]=="in_progress" && (string?)task["status"]=="ready" &&
    Sub(Red)=="ready" && Sub(Green)=="planned" && Sub(Verify)=="planned";
var target=(string?)product["plan_version"]=="0.1.28" &&
    (string?)chunk["status"]=="done" && (string?)task["status"]=="done" &&
    Sub(Red)=="done" && Sub(Green)=="done" && Sub(Verify)=="done";
if(!baseline && !target)
    throw new InvalidOperationException("RS029 lifecycle is neither qualified M2 boundary baseline nor exact completed target.");

var inherited=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorEvidence)))!.AsObject();
if((string?)inherited["status"]!="immutable-typed-IR-functional-contract-qualified-adversarial-boundaries-pending" ||
    inherited["complete_ir_tests_passed"]?.GetValue<bool>()!=true)
    throw new InvalidOperationException("RS028 functional typed IR qualification is missing.");

p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    var reds=new[]{
        ("PublicFactoriesMustBoundTotalDepth",
            "M2-W01-C02-T2 RED: nested graph depth over 32 is accepted."),
        ("PublicFactoriesMustBoundExpandedNodeBudget",
            "M2-W01-C02-T2 RED: shared DAG exceeds the 1024-node expanded budget."),
        ("MatrixShapeMustBeAnAdmittedPublicIRType",
            "M2-W01-C02-T2 RED: typed bounded immutable matrix is absent."),
        ("SymbolDomainMustNotLeakAcrossSameTextIdentity",
            "M2-W01-C02-T2 RED: positivity assumption leaks across symbol domains.")
    };
    foreach(var (name,marker) in reds)
    {
        var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
            "--filter","FullyQualifiedName~M2W01C02BoundaryRedTests."+name,
            "--logger","console;verbosity=normal");
        if(observed.ExitCode==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Expected independent IR boundary RED not observed: "+
                name+" | "+Relevant(observed.Output));
    }
}
var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C02-boundary-red",
    ["status"]="four-independent-public-boundary-reds-observed",
    ["prior_functional_evidence"]=PriorEvidence,
    ["focused_filter"]="FullyQualifiedName~M2W01C02BoundaryRedTests",
    ["red_failures"]=p.Json.StringArray(
        "Factory admits total graph depth over the proposed 32-node-level limit.",
        "Shared graph DAG reuses can exceed 1024 expanded node occurrences without refusal.",
        "No public homogeneous, rectangular immutable matrix exists.",
        "Nonnegative assumptions can leak between same-id symbols with distinct declared scalar domains."),
    ["limitations"]="Prechange RED evidence alone is not a GREEN implementation or an unbounded resource-security guarantee."
};
p.Files.WriteComplete(RedEvidence,
    redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var source in new[]{Model,Graph,Matrix,Tests})
    p.Files.ReplaceFromStaged("staged/"+source,source);
RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach(var name in new[]{
    "PublicFactoriesMustBoundTotalDepth",
    "PublicFactoriesMustBoundExpandedNodeBudget",
    "MatrixShapeMustBeAnAdmittedPublicIRType",
    "SymbolDomainMustNotLeakAcrossSameTextIdentity"})
    RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C02BoundaryRedTests."+name,
        "--logger","console;verbosity=normal");
foreach(var name in new[]{
    "Depth32AcceptedAndDepth33Rejected",
    "ExpandedDagCountsSharedNodesEachUse",
    "RectangularMatrixPreservesValuesWithoutAliasing",
    "MatrixRejectsShapesAndMixedNumericKinds",
    "MatrixLocalLimitAndGlobalLimitBothApply",
    "AssumptionsCannotCrossSymbolDomainsOrBindingScopes",
    "DuplicatedAssumptionsAndOversizedArraysAreRejected",
    "RestrictedExpressionAlsoEnforcesDepth",
    "OriginalExactAndFinitePoliciesRemainDistinct"})
    RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C02BoundaryTests."+name,
        "--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","ir-package-feed");
RunRequired(root,"dotnet","pack",
    "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "-c","Release","--no-build","--no-restore","-o",feed);
var packagePath=Path.Combine(feed,"dw.tools.math.ir."+PackageVersion+".nupkg");
if(!File.Exists(packagePath))
    throw new InvalidOperationException("Bounded IR artifact missing.");
using(var archive=ZipFile.OpenRead(packagePath))
{
    if(archive.GetEntry("lib/net10.0/dw.tools.math.ir.dll") is null)
        throw new InvalidOperationException("Bounded IR package lacks its compiled assembly.");
    var nuspecs=archive.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspecs.Length!=1)
        throw new InvalidOperationException("Bounded IR package must have exactly one nuspec.");
    using var contents=nuspecs[0].Open();
    var xml=XDocument.Load(contents);
    var metadata=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var dependencies=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.ir" || version!=PackageVersion ||
       !dependencies.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("IR package identity or dependency contract drifted.");
}
var qualified=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C02-boundary-qualified",
    ["status"]="immutable-bounded-typed-IR-chunk-qualified",
    ["prior_exact_contract"]=PriorEvidence,
    ["independent_red"]=RedEvidence,
    ["modified_model_source"]=Model,
    ["model_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Model))),
    ["graph_validator_source"]=Graph,
    ["graph_validator_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Graph))),
    ["matrix_source"]=Matrix,
    ["matrix_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Matrix))),
    ["package_id"]="dw.tools.math.ir",
    ["package_version"]=PackageVersion,
    ["package_sha256"]=Hash(File.ReadAllBytes(packagePath)),
    ["package_dependencies"]=p.Json.StringArray("dw.quantities"),
    ["focused_reds_now_green"]=4,
    ["independent_boundary_tests_passed"]=9,
    ["full_ir_test_suite_passed"]=true,
    ["full_quantities_suite_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["accepted_contracts"]=p.Json.StringArray(
        "All public IrApply and IrRestrictedExpression constructions validate whole graph depth <=32.",
        "Expanded IR node occurrences <=1024 are enforced even when a subgraph is referenced more than once.",
        "Immutable rectangular homogeneous exact/approximate scalar matrices carry shape and defensive copied cells.",
        "Matrix source shapes have at most 4096 cells; expanded whole-graph quota can be stricter (at most 1023 scalar cells plus one matrix node).",
        "Exact and approximate cells cannot be combined by silent numeric widening.",
        "IR nonnegative assumptions require both stable symbol identity and matching scalar domain; free and bound scopes remain distinct.",
        "Duplicate identical assumptions and oversized assumption collections are rejected.",
        "Finite IEEE754 and exact rational identities and admitted real sqrt-square transformations survive full regressions."),
    ["qualified_limits"]=new JsonObject
    {
        ["maximum_expanded_nodes"]=1024,
        ["maximum_tree_depth"]=32,
        ["maximum_matrix_shape_cells"]=4096,
        ["maximum_assumptions"]=32,
        ["maximum_identifier_characters"]=128
    },
    ["remaining_ownership"]=p.Json.StringArray(
        "An executable binder, alpha-equivalent substitution and unbound-symbol validation are not implemented; stable bound symbol identities are carried as data only.",
        "IR codec, semantic hashing and versioned interchange are owned by M2-W01-C03 and not claimed here.",
        "The limits apply to admitted immutable in-memory IR factories; no arbitrary input parser, CAS, complex numbers or external AURA adoption is qualified."),
    ["external_adoption_transmitted"]=false
};
p.Files.WriteComplete(GreenEvidence,
    qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    plan["plan_version"]="0.1.28";
    c["status"]="done";
    c["refinement"]="RS028 qualified the provider-free exact/finite IR model and bound/free symbol semantics. RS029 observes four independent REDs, then enforces expanded DAG/node and depth budgets in public factories, adds homogeneous immutable bounded matrices and domain-aware assumption identity, and passes nine boundary tests plus all IR/quantity/foundation regressions and package inspection. The executable binder and canonical codec are explicitly deferred.";
    foreach(var path in new[]{Model,Graph,Matrix,RedTests,Tests,RedEvidence,GreenEvidence})
        if(!c["files"]!.AsArray().Any(x=>(string?)x==path))
            c["files"]!.AsArray().Add((JsonNode?)JsonValue.Create(path));
    foreach(var command in new[]{
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M2W01C02BoundaryRedTests",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M2W01C02BoundaryTests"})
        if(!c["commands"]!.AsArray().Any(x=>(string?)x==command))
            c["commands"]!.AsArray().Add((JsonNode?)JsonValue.Create(command));
    c["evidence"]!.AsArray().Add((JsonNode?)JsonValue.Create(RedEvidence));
    c["evidence"]!.AsArray().Add((JsonNode?)JsonValue.Create(GreenEvidence));
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,GreenEvidence);
    foreach(var sub in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedEvidence:GreenEvidence);
    }
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Task,"ready","in-progress");
    p.ProjectPlan.TransitionNode(Red,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    p.ProjectPlan.TransitionNode(Green,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Green,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Green);
    p.ProjectPlan.TransitionNode(Verify,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Verify,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var id in new[]{Red,Green,Verify,Task,Phase})
        p.ProjectPlan.RequireNodeState(id,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Relevant(string output)
{
    var position=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(position<0)position=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var useful=position<0?output:output[position..];
    return useful.Length>3000?useful[..3000]:useful;
}
static (int ExitCode,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string RunRequired(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.ExitCode!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.ExitCode+": "+Relevant(result.Output));
    return result.Output;
}
