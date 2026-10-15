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

const string Work="M4-W02",Phase="M4-W02-C01",Task="M4-W02-C01-T1";
const string Red="M4-W02-C01-T1-R",Green="M4-W02-C01-T1-G",
    Verify="M4-W02-C01-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M4-W01-C03-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M4-W02-C01-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M4-W02-C01-T1-contract-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicExactDifferentiation.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W02C01T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W02C01T1Tests.cs";
const string Decision="docs/planning/decisions/m4-w02-c01-t1-exact-differentiation.md";
const string Document="docs/distribution/symbolic-exact-differentiation.md";
const string ConsumerProject="tests/consumers/m4-w02-c01-t1/m4-exact-derivative-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w02-c01-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w02-c01-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS059/staged/checks/m4-w02-c01-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="0.5.0-preview.5";
const string RedMarker="M4-W02-C01-T1 RED: exact provider-neutral polynomial derivative API is absent.";
const string ConsumerMarker="dw.tools.math/m4-exact-derivative-consumer/0.5 qualified; probes=8";
const int GreenCount=38, TargetedCount=39;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS059","M4",target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS058",1,
    "feat: qualify bounded symbolic execution and close M4-W01",
    "0.1.59",1,1,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-symbolic-provenance-cancellation-and-soft-deadline-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(26));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(27));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/m4_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m4_milestone_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("0.5.0-preview.4"));
p.Require.JsonValueEquals(Prior,"/deadline_is_preemptive",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.57"),JsonValue.Create("0.1.58"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.58":"0.1.57"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/tasks/1/status",JsonValue.Create("planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/30/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/31/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",JsonValue.Create("satisfied"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C03","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.4</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-exact-formal-polynomial-derivative-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("real-exact-formal-polynomial-differentiation-and-explicit-unit-refusal-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(GreenCount));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/derived_units_automatically_supported",
        JsonValue.Create(false));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.5</Version>");
}

var staged=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
staged[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing controlled RED source identity.");

if(!target)
{
    // A compiled, previously qualified preview.4 is essential. An absent
    // public API reflected at runtime constitutes RED; compile errors do not.
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W02C01T1RedTests.ExactRealPolynomialDerivativeNeedsAPublicTypedContract",
        "--logger","console;verbosity=normal");
    if(red.Code==0||!red.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Missing-API derivative RED not independently observed: "+
            Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C01-T1-contract-red",
        ["status"]="compiled-exact-formal-polynomial-derivative-api-absent-red-observed",
        ["prior_symbolic_transform_evidence"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=staged[RedTest],
        ["missing_public_api"]="SymbolicExactDifferentiation.TryDifferentiate(IrNode,IrSymbol)",
        ["compiled_product_version"]="0.5.0-preview.4",
        ["expected_red_assertion"]=RedMarker,
        ["independent_missing_api_red"]=true,
        ["nonclaim"]="No mathematical derivative is admitted until a separately built GREEN implementation passes."
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Project,Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    staged[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged-file typed SHA-256 for "+file);
}

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W02C01T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 39",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 39",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected recovered RED plus 38 independent polynomial derivative GREEN tests.");
Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var path in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var receipt=Required(root,"dotnet","test",path,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!receipt.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Relevant exact or typed IR regression did not pass: "+path);
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
var feed=Path.Combine(root,"build","artifacts","m4-w02-c01-t1-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var path=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(path))
        throw new InvalidOperationException("Expected generated NuGet archive missing: "+package.Id);
    archives.Add(path);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var archiveOutput=Required(root,"dotnet",helperArgs.ToArray());
var packageRecords=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(archiveOutput,package.Id,package.Version);
    packageRecords.Add(new JsonObject
    {
        ["id"]=package.Id,["version"]=package.Version,
        ["bytes"]=receipt.Bytes,["sha256"]=receipt.Sha,
        ["dependencies"]=p.Json.StringArray(package.DependsOn),
        ["assembly"]="lib/net10.0/"+package.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}
var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var refs=consumerXml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(refs.Length!=1 ||
   (string?)refs[0].Attribute("Include")!="dw.tools.math.symbolics" ||
   (string?)refs[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("NuGet-only derivative consumer must use one exact Math package.");
var feeds=XDocument.Load(Path.Combine(root,ConsumerConfig)).Descendants()
    .Single(x=>x.Name.LocalName=="packageSources");
if(!feeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
   feeds.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("Derivative consumer must refuse outside NuGet feeds.");
var isolatedCache=Path.Combine(root,"build","artifacts","m4-w02-c01-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolatedCache);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var client=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(client,ConsumerMarker))
    throw new InvalidOperationException("Eight independent NuGet-only exact derivative oracles were not observed.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C01-T1-contract-qualified",
        ["status"]="real-exact-formal-polynomial-differentiation-and-explicit-unit-refusal-qualified",
        ["prior_bounded_transform_evidence"]=Prior,
        ["controlled_red_evidence"]=RedProof,
        ["question"]="Can an exact, real-to-real formal polynomial function differentiate x^3 to 3x^2, return zero for an absent variable and explicitly refuse unsupported derived units and uncertain analytic branches?",
        ["observation"]="One compiled absent-public-contract RED became GREEN, 38 independent mathematical tests passed and eight package-only oracles independently exercised exact derivatives and typed domain/units refusals.",
        ["decision"]="Admit only polynomial formal derivatives up to degree eight, constants, sums and exact constant products over free real IR symbols; refuse units, retained assumptions, bound/integer domains, and unproven analytic functions.",
        ["product_source"]=Product,["product_source_sha256"]=staged[Product],
        ["product_project"]=Project,["product_project_sha256"]=staged[Project],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=staged[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=staged[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=staged[Decision],
        ["distribution_document"]=Document,["distribution_document_sha256"]=staged[Document],
        ["independent_green_tests_passed"]=GreenCount,
        ["red_recovered_and_green_tests_passed"]=TargetedCount,
        ["independent_consumer_probes_passed"]=8,
        ["controlled_red_then_green"]=true,
        ["formal_rule_id"]="formal-polynomial-derivative/exact/v1",
        ["cubic_derivative"]="D(x^3,x)=3*x^2",
        ["absent_symbol_derivative"]="D(x^3,y)=0",
        ["maximum_qualified_positive_integer_power"]=8,
        ["domain"]="IrScalarDomain.Real",
        ["codomain"]="IrScalarDomain.Real",
        ["function_parameter_is_free_real"]=true,
        ["derived_units_automatically_supported"]=false,
        ["unit_literals_return_unsupported_units"]=true,
        ["conditional_derivative_admitted"]=false,
        ["nondifferentiable_absolute_or_sqrt_admitted"]=false,
        ["nonconstant_product_and_quotient_rules_admitted"]=false,
        ["source_semantic_structural_sha256_recorded"]=true,
        ["differentiation_variable_identity_recorded"]=true,
        ["canonical_ir_roundtrip_passed"]=true,
        ["no_implicit_binary64"]=true,
        ["complete_symbolics_tests_passed"]=true,
        ["typed_ir_quantities_regressions_passed"]=true,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(packageRecords.ToArray()),
        ["product_package_sha_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=staged[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=staged[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=staged[ConsumerConfig],
        ["consumer_marker"]=ConsumerMarker,
        ["source_only_nuget_consumer"]=true,
        ["packages_publicly_published"]=false,
        ["locked_solution_build_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["work_package"]="M4-W02",
        ["work_package_done"]=false,
        ["m4_milestone_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W02-C01-T2 differentiation at nondifferentiable/domain boundaries, unit policy and unsupported analytic forms",
        ["nonclaims"]=p.Json.StringArray(
            "No general CAS, arbitrary polynomial degree, broad chain/product/quotient rules, complex branches, general higher derivatives or complete symbolic functions.",
            "No unit-derivative calculation, implicit quantity erasure, arbitrary conditional predicate transport or globally qualified pointwise differentiability.",
            "No mandatory Angouri/MathNET runtime, public NuGet package feed, AURA/Decision/MCDM consumption or dw.discovery broker skill.",
            "No M4-W02 work package completion, M4 milestone/G-M4 gate or mathematical guarantee beyond the observed exact closed subset.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.58";
        var work=plan["work_packages"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Work);
        var chunk=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Task);
        if((string?)work["status"]!="planned" ||
           (string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M4-W02 first active derivative scope has diverged.");
        work["status"]="in_progress";
        chunk["status"]="in_progress";
        chunk["refinement"]="RS053-RS058 qualified the first symbolic provider review, typed IR bridge and bounded exact transformations without an external CAS dependency. RS059 T1 now adds preview.5 closed exact formal polynomial differentiation with explicit real domain/codomain: d(x^3)/dx=3*x^2; derivative wrt an absent symbol is exact zero, exact constant products/sums are admitted, and powers beyond degree eight, non-polynomial analytic forms, scoped restrictions and derived units return typed refusals. One compiled RED recovered with 38 independent GREEN tests, eight NuGet-only consumer oracles, locked IR/quantities regression suites and three actual local product SHA-256s. C01-T2 retains pointwise differentiability, units and conditional-provider boundaries.";
        chunk["readiness"]="in progress: exact real formal-polynomial differentiation T1 qualified; T2 domain, unit and unsupported-function boundaries planned";
        var priorFiles=chunk["files"]!.AsArray().Select(v=>v!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Project,Product,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W02C01T1",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        chunk["evidence"]=p.Json.StringArray(RedProof,Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });

    // The M4 milestone is already active; the new W02 suffix is ready.
    // Activate only the declared Work/Phase/Task/Red continuation.
    foreach(var node in new[]{Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
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

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M4-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C03","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS059" ||
       (string?)state["activeMilestone"]!="M4" ||
       (string?)state["activeWorkPackage"]!=(target?Work:null) ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS059 exact baseline/target active hierarchy mismatch.");
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
    var prefix="M4-W02-C01-T1-PKG-SHA256:"+id+":"+version+":";
    var found=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(found.Length!=1)
        throw new InvalidOperationException("One independent local package receipt required for "+id);
    var parts=found[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes) ||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid archive hash or byte record for "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2600?part[..2600]:part;
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
        throw new TimeoutException(executable+" exceeded the bounded command execution window.");
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
