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

const string Work="M4-W01", Phase="M4-W01-C02", Task="M4-W01-C02-T1";
const string Red="M4-W01-C02-T1-R", Green="M4-W01-C02-T1-G",
    Verify="M4-W01-C02-T1-V";
const string Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string PriorT1="docs/planning/evidence/M4-W01-C01-T1-qualified.json";
const string PriorT2="docs/planning/evidence/M4-W01-C01-T2-boundaries-qualified.json";
const string RedEvidence="docs/planning/evidence/M4-W01-C02-T1-contract-red.json";
const string Evidence="docs/planning/evidence/M4-W01-C02-T1-contract-qualified.json";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string ProjectLock="src/projects/dw.tools.math.symbolics/packages.lock.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicRationalCancellationBridge.cs";
const string Tests="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string TestsLock="tests/projects/dw.tools.math.symbolics.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W01C02T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W01C02T1Tests.cs";
const string Solution="dw.tools.math.slnx";
const string Decision="docs/planning/decisions/m4-w01-c02-t1-ir-bridge.md";
const string Document="docs/distribution/symbolic-ir-rational-bridge.md";
const string ConsumerProject="tests/consumers/m4-w01-c02-t1/m4-bridge-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w01-c02-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w01-c02-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS055/staged/checks/m4-ir-product-package-sha.cs";
const string Version="0.5.0-preview.1";
const string RedMarker="M4-W01-C02-T1 RED: public typed domain-preserving cancellation contract is absent.";
const string ConsumerMarker="dw.tools.math/m4-ir-bridge-consumer/0.5 qualified; probes=8";
const int NewTests=25;
const int TotalTargeted=26;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS055","M4",Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS054",1,
    "feat: qualify bounded symbolic provider host and close M4 C01",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(PriorT1,"/status",
    JsonValue.Create("m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified"));
p.Require.JsonValueEquals(PriorT1,"/selected_candidate",
    JsonValue.Create("AngouriMath/2.5.0-provisional"));
p.Require.JsonValueEquals(PriorT2,"/status",
    JsonValue.Create("m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified"));
p.Require.JsonValueEquals(PriorT2,"/host_probes_passed",JsonValue.Create(14));
p.Require.JsonValueEquals(PriorT2,"/public_symbolic_adapter_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorT2,"/conditional_provider_text_is_typed_IR",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.53"),JsonValue.Create("0.1.54"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.54":"0.1.53"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/28/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/qualified_by",
    JsonValue.Create("M4-W01-C01"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",
    JsonValue.Create(target?"satisfied":"unverified"));
if(!target)
{
    foreach(var file in new[]{Product,Project,ProjectLock,Tests,TestsLock,
        RedTest,GreenTests,Decision,Document,ConsumerProject,ConsumerSource,
        ConsumerConfig,RedEvidence,Evidence})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("symbolic-restricted-exact-IR-public-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Evidence,"/status",
        JsonValue.Create("typed-exact-symbolic-rational-cancellation-and-pole-exclusion-qualified"));
    p.Require.JsonValueEquals(Evidence,"/independent_green_tests_passed",JsonValue.Create(NewTests));
    p.Require.JsonValueEquals(Evidence,"/isolated_consumer_probes_passed",JsonValue.Create(8));
}
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

// Controlled RED runs with a real, empty, compiled product assembly and
// reflection against its absent public API. The implementation and 25 GREEN
// tests are explicitly not installed until that RED is observed.
var stagedHashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Project,ProjectLock,Tests,TestsLock,Solution,RedTest})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+path,path);
    stagedHashes[path]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged-file identity for "+path);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redRun=Run(root,"dotnet","test",Tests,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W01C02T1RedTests.PublicTypedExactCancellationContractMustExist",
        "--logger","console;verbosity=normal");
    if(redRun.Code==0 || !redRun.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("A controlled missing-API RED test was not independently observed: "+
            Relevant(redRun.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C02-T1-contract-red",
        ["status"]="symbolic-restricted-exact-IR-public-contract-absent-red-observed",
        ["prior_symbolic_boundary_evidence"]=PriorT2,
        ["test"]=RedTest,["test_sha256"]=stagedHashes[RedTest],
        ["real_compiled_product_project"]=Project,
        ["missing_api"]="Dw.Tools.Math.Symbolics.SymbolicRationalCancellationBridge.TryCancel(IrNode)",
        ["independent_red_test_count"]=1,
        ["red_assertion_marker"]=RedMarker,
        ["red_exit_nonzero"]=true,
        ["implementation_deliberately_absent_until_red"]=true,
        ["nonclaims"]=p.Json.StringArray(
            "The absence RED proves only missing API; it is not a domain-preserving mathematical result.",
            "No public symbolic provider integration or runtime capability is inferred from a RED.")
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

// The typed source, GREEN tests and package consumer are all staged through
// the same bounded Payload SDK, never by a shell copy.
foreach(var path in new[]{Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+path,path);
    stagedHashes[path]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged-file identity for "+path);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targetedResult=Required(root,"dotnet","test",Tests,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W01C02T1",
    "--logger","console;verbosity=normal");
if(!targetedResult.Contains("Passed: 26",StringComparison.OrdinalIgnoreCase) ||
   !targetedResult.Contains("Total tests: 26",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Exact T1 RED recovery + 25 independent GREEN tests not observed.");
Required(root,"dotnet","test",Tests,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var project in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regressionResult=Required(root,"dotnet","test",project,
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!regressionResult.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Relevant exact or typed-IR regression lacked a passing receipt: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

// Three scoped local packages prove a provider-neutral transitive graph:
// quantities -> IR -> new symbolics product.
var products=new (string Id,string PackageVersion,string Csproj,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,Project,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-w01-c02-t1-local-feed");
var archives=new List<string>();
foreach(var product in products)
{
    Required(root,"dotnet","pack",product.Csproj,
        "-c","Release","--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,product.Id+"."+product.PackageVersion+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Expected local Math product archive absent: "+product.Id);
    archives.Add(archive);
}
var packageArgs=new List<string>{"run","--file",PackageHelper,"--"};
packageArgs.AddRange(archives);
var productArchivesOutput=Required(root,"dotnet",packageArgs.ToArray());
var observedPackages=new List<JsonNode>();
foreach(var product in products)
{
    var info=ReadPackageProof(productArchivesOutput,product.Id,product.PackageVersion);
    observedPackages.Add(new JsonObject
    {
        ["id"]=product.Id,["version"]=product.PackageVersion,
        ["bytes"]=info.Size,["sha256"]=info.Sha,
        ["dependencies"]=p.Json.StringArray(product.DependsOn),
        ["assembly"]="lib/net10.0/"+product.Id+".dll",
        ["scope"]="local-ignored-product-package-not-published"
    });
}
var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var dependencies=consumerXml.Descendants()
    .Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(dependencies.Length!=1||
   (string?)dependencies[0].Attribute("Include")!="dw.tools.math.symbolics"||
   (string?)dependencies[0].Attribute("Version")!="["+Version+"]"||
   consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Consumer is not a single direct NuGet-only symbolic client.");
var nugetXml=XDocument.Load(Path.Combine(root,ConsumerConfig));
var packageSources=nugetXml.Descendants().Single(e=>e.Name.LocalName=="packageSources");
if(!packageSources.Elements().Any(e=>e.Name.LocalName=="clear")||
   packageSources.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("Consumer package source is not closed to external feeds.");
var isolatedPackages=Path.Combine(root,"build","artifacts","m4-w01-c02-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolatedPackages);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight independent package-only IR bridge oracles did not all pass.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C02-T1-contract-qualified",
        ["status"]="typed-exact-symbolic-rational-cancellation-and-pole-exclusion-qualified",
        ["prior_provider_decision"]=PriorT1,["prior_symbolic_host_boundary"]=PriorT2,
        ["controlled_red_evidence"]=RedEvidence,
        ["question"]="Can exact IR structural cancellation preserve the original rational denominator exclusion while refusing unsupported input without a CAS runtime dependency?",
        ["observation"]="One true compiled absent-API RED becomes GREEN; 25 further scientific tests and eight independent package-only client oracles qualify the exact typed IR bridge.",
        ["decision"]="Admit only a free real-symbol exact (x²-c²)/(x-c) cancellation with x!=c preserved as IrRestrictedExpression and checked exact point values; defer all provider text adaptation and general symbolic transformations.",
        ["source"]=Product,["source_sha256"]=stagedHashes[Product],
        ["project"]=Project,["project_sha256"]=stagedHashes[Project],
        ["project_lock"]=ProjectLock,["project_lock_sha256"]=stagedHashes[ProjectLock],
        ["test_project"]=Tests,["test_project_sha256"]=stagedHashes[Tests],
        ["test_project_lock"]=TestsLock,["test_project_lock_sha256"]=stagedHashes[TestsLock],
        ["red_test"]=RedTest,["red_test_sha256"]=stagedHashes[RedTest],
        ["green_test"]=GreenTests,["green_test_sha256"]=stagedHashes[GreenTests],
        ["solution"]=Solution,["solution_sha256"]=stagedHashes[Solution],
        ["decision_document"]=Decision,["decision_sha256"]=stagedHashes[Decision],
        ["distribution_document"]=Document,["distribution_sha256"]=stagedHashes[Document],
        ["independent_green_tests_passed"]=NewTests,
        ["red_recovered_and_green_tests_passed"]=TotalTargeted,
        ["controlled_red_then_green"]=true,
        ["exact_rational_pole_exclusion"]="(x²-c²)/(x-c) => x+c only with x!=c",
        ["exclusion_at_x_equal_c_returns_numeric_value"]=false,
        ["canonical_restricted_ir_round_trip"]=true,
        ["bound_and_integer_symbols_refused"]=true,
        ["existing_assumptions_preserved"]=true,
        ["contradictions_refused"]=true,
        ["max_constant_operand_bits"]=128,
        ["no_implicit_binary64"]=true,
        ["local_product_package_graph"]=p.Json.Array(observedPackages.ToArray()),
        ["package_sha256_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,["consumer_project_sha256"]=stagedHashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,["consumer_source_sha256"]=stagedHashes[ConsumerSource],
        ["consumer_nuget_config"]=ConsumerConfig,["consumer_nuget_config_sha256"]=stagedHashes[ConsumerConfig],
        ["isolated_consumer_probes_passed"]=8,
        ["consumer_exact_output"]=ConsumerMarker,
        ["no_cas_provider_runtime_dependency"]=true,
        ["symbolic_external_dependency_satisfied_by_completed_C01"]=true,
        ["provider_scientific_scope"]="AngouriMath/2.5.0 only as provisionally qualified prior provider, not mandatory symbolics implementation",
        ["locked_solution_build_passed"]=true,
        ["complete_symbolics_tests_passed"]=true,
        ["relevant_ir_and_quantities_regressions_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W01-C02-T2 unsupported provider output and conditional-IR integration boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "No general CAS, provider AST or provided-text parser, arbitrary identity proof, full symbolic simplifier or solver.",
            "No lost-domain cancellation, unrepresentable predicate coercion, non-real principal branch, native provider cancellation or execution permissions.",
            "No MathNET/Angouri runtime dependency in new product, public NuGet feed, Decision/MCDM/AURA integration or dw.discovery skill.",
            "No C02, M4-W01 or M4 release completion; T2 symbolic integration restrictions remain planned.")
    };
    p.Files.WriteComplete(Evidence,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.54";
        var ext=plan["external_dependencies"]!.AsArray()
            .Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]=="EXT-SYMBOLIC");
        if((string?)ext["status"]!="unverified"||
           (string?)ext["qualified_by"]!="M4-W01-C01")
            throw new InvalidOperationException("Existing provider boundary qualification is not attributable to C01.");
        ext["status"]="satisfied";
        ext["evidence"]=p.Json.StringArray(PriorT1,PriorT2);
        var c=plan["chunks"]!.AsArray()
            .Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Phase);
        var task=c["tasks"]!.AsArray()
            .Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Task);
        c["status"]="in_progress";
        c["refinement"]="RS053 and RS054 independently qualified an isolated AngouriMath conditional-text example and a bounded child-process host, satisfying only the accepted EXT-SYMBOLIC provider-research prerequisite. RS055 T1 introduces a provider-free dw.tools.math.symbolics 0.5.0-preview.1 product referencing canonical IR and exact quantities only: the closed exact (x²-c²)/(x-c) rule yields typed restricted x+c with x!=c and an explicit status-bearing exact point evaluator. A compiled absent-contract RED recovers; 25 independent GREEN tests, 8 NuGet-only consumer oracles, all relevant IR/quantities regressions, new lockfiles, 3 local packages and actual archive SHA-256s qualify the narrow T1. T2 remains responsible for unsupported provider predicate integration, general conditional boundaries and copy/serialization rules.";
        c["readiness"]="in progress: T1 provider-free exact restricted IR bridge qualified; T2 unsupported external-provider condition and safety integration planned";
        var existingFiles=c["files"]!.AsArray().Select(v=>v!.GetValue<string>());
        c["files"]=p.Json.StringArray(existingFiles.Concat(new[]{Project,ProjectLock,Product,
            Tests,TestsLock,RedTest,GreenTests,Solution,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedEvidence,Evidence})
            .Distinct(StringComparer.Ordinal).ToArray());
        c["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Tests+" -c Release --filter FullyQualifiedName~M4W01C02T1",
            "dotnet test "+Tests+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        c["evidence"]=p.Json.StringArray(RedEvidence,Evidence);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedEvidence,Evidence);
        foreach(var child in task["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray((string?)child["id"]==Red
                ?RedEvidence:Evidence);
        }
    });

    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var node in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C03","not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.Json.EditObject(State,st=>
{
    if((string?)st["pendingRun"]!="RS055" ||
       (string?)st["activeMilestone"]!="M4" ||
       (string?)st["activeWorkPackage"]!=Work ||
       (string?)st["activePhase"]!=(target?Phase:null) ||
       st["activeTask"] is not null || st["activeSubtask"] is not null)
        throw new InvalidOperationException("RS055 expected exact pre/post C02 first-task execution cursor.");
    st["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasLine(string output,string marker) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);
static (long Size,string Sha) ReadPackageProof(string output,string id,string version)
{
    var prefix="M4-IR-PKG-SHA256:"+id+":"+version+":";
    var matched=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matched.Length!=1)
        throw new InvalidOperationException("Expected exactly one independent product NuGet hash for "+id);
    var parts=matched[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes) ||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid independent product archive identity "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var snippet=index<0?output:output[index..];
    return snippet.Length>2300?snippet[..2300]:snippet;
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
        throw new TimeoutException(executable+" timed out.");
    }
    var combined=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(combined);
    return(process.ExitCode,combined);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
