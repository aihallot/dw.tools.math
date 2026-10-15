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

const string Work="M4-W02",Phase="M4-W02-C01",Task="M4-W02-C01-T2";
const string Red="M4-W02-C01-T2-R",Green="M4-W02-C01-T2-G",
    Verify="M4-W02-C01-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M4-W02-C01-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M4-W02-C01-T2-boundary-red.json";
const string Qualified="docs/planning/evidence/M4-W02-C01-T2-boundaries-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicDerivativePointAnalysis.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W02C01T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W02C01T2Tests.cs";
const string Decision="docs/planning/decisions/m4-w02-c01-t2-pointwise-boundary.md";
const string Document="docs/distribution/symbolic-pointwise-derivatives.md";
const string ConsumerProject="tests/consumers/m4-w02-c01-t2/m4-pointwise-derivative-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w02-c01-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w02-c01-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS060/staged/checks/m4-w02-c01-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="0.5.0-preview.6";
const string RedMarker="M4-W02-C01-T2 RED: pointwise real derivative boundary API is absent.";
const string ConsumerMarker="dw.tools.math/m4-pointwise-derivative-consumer/0.5 qualified; probes=8";
const int IndependentGreen=37,TotalTargeted=38;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS060","M4",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS059",1,
    "feat: qualify exact formal polynomial differentiation and real function domains",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("real-exact-formal-polynomial-differentiation-and-explicit-unit-refusal-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(38));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(39));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/derived_units_automatically_supported",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("0.5.0-preview.5"));
p.Require.JsonValueEquals(Prior,"/work_package_done",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.58"),JsonValue.Create("0.1.59"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.59":"0.1.58"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/30/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/31/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/32/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",JsonValue.Create("satisfied"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W02-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C03","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.5</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-pointwise-real-derivative-boundary-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("real-exact-pointwise-derivative-and-domain-refusal-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m4_w02_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/numerical_differentiation_qualified",
        JsonValue.Create(false));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.6</Version>");
}

var stagedHashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
stagedHashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing typed staged RED source identity.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W02C01T2RedTests.ExplicitPointwiseDerivativeBoundaryContractMustExist",
        "--logger","console;verbosity=normal");
    if(red.Code==0||!red.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A true compiled missing-pointwise-contract RED was not observed: "+Relevant(red.Output));
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C01-T2-boundary-red",
        ["status"]="compiled-pointwise-real-derivative-boundary-contract-absent-red-observed",
        ["prior_exact_derivative"]=Prior,
        ["compiled_source_product"]="dw.tools.math.symbolics/0.5.0-preview.5",
        ["red_test"]=RedTest,["red_test_sha256"]=stagedHashes[RedTest],
        ["missing_api"]="SymbolicDerivativePointAnalysis.Analyze(IrNode,IrSymbol,ExactRational)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["failed_compiler_or_restore_did_not_substitute_for_red"]=true,
        ["nonclaim"]="A missing public API is not a proof of real pointwise differentiability."
    };
    p.Files.WriteComplete(RedProof,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Project,Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedHashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing typed target/source hash for "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W02C01T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 38",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 38",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected 1 recovered RED plus 37 independent GREEN pointwise derivative tests.");
Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regression in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var output=Required(root,"dotnet","test",regression,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!output.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Required exact IR or quantities regression was not observed: "+regression);
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
var feed=Path.Combine(root,"build","artifacts","m4-w02-c01-t2-local-feed");
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
    throw new InvalidOperationException("The NuGet-only pointwise consumer must clear external sources.");
var isolated=Path.Combine(root,"build","artifacts","m4-w02-c01-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight NuGet-only pointwise derivative oracles not observed.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W02-C01-T2-boundaries-qualified",
        ["status"]="real-exact-pointwise-derivative-and-domain-refusal-qualified",
        ["prior_exact_formal_derivative"]=Prior,
        ["controlled_red_evidence"]=RedProof,
        ["question"]="Can a real polynomial derivative be evaluated at exact rational points while distinguishing nondifferentiability, undefined original domains, unsupported analytic functions and unit policy without any numerical finite differences?",
        ["observation"]="One compiled missing-boundary RED recovered; 37 exact/domain/units GREEN tests and eight independent NuGet-only client oracles observed polynomial point values and conservatively classified real analytic boundaries.",
        ["decision"]="Admit exact polynomial point evaluation, Abs and Sqrt(Square(real x)) away from zero, and explicit typed refusal at kinks, outside original real domain, unproved radical or quotient forms, units, and retained assumptions.",
        ["new_source"]=Product,["new_source_sha256"]=stagedHashes[Product],
        ["product_project"]=Project,["product_project_sha256"]=stagedHashes[Project],
        ["new_green_tests"]=GreenTests,["new_green_tests_sha256"]=stagedHashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=stagedHashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=stagedHashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=stagedHashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=TotalTargeted,
        ["compiled_red_observed"]=true,
        ["independent_consumer_probes_passed"]=8,
        ["exact_polynomial_point_oracle"]="D(x^3,x=2)=12",
        ["original_quotient_pole_at_x_equal_1_returns_value"]=false,
        ["absolute_value_at_zero_nondifferentiable"]=true,
        ["real_sqrt_of_square_at_zero_nondifferentiable"]=true,
        ["real_sqrt_negative_point_outside_domain"]=true,
        ["positive_sqrt_derivative_qualified"]=false,
        ["numerical_differentiation_qualified"]=false,
        ["no_implicit_binary64"]=true,
        ["units_return_unsupported_units"]=true,
        ["restricted_source_returns_unsupported_assumptions"]=true,
        ["maximum_exact_rational_bits"]=128,
        ["canonical_source_semantic_hash_recorded"]=true,
        ["differentiation_variable_identity_recorded"]=true,
        ["no_external_provider_product_dependency"]=true,
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
        ["m4_w02_c01_done"]=true,
        ["m4_w02_work_package_done"]=false,
        ["m4_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W02-C02-T1 bounded symbolic solving of exact polynomial equations with domain-aware results",
        ["nonclaims"]=p.Json.StringArray(
            "No general analytic or numerical derivative; no complete function domain/codomain or one-sided differential calculus.",
            "No derived unit computations, implicit restriction removal, binary64 finite differences, general quotient, complex branches or universal differentiability.",
            "No external Angouri/MathNET provider decoder, mandatory Python runtime, public NuGet feed, AURA/Decision/MCDM integration or dw.discovery agent skill.",
            "No M4-W02/C02/C03 completion, G-M4 release promotion or hard deadline guarantees.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.59";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M4-W02-C01 T2 baseline diverged.");
        chunk["status"]="done";
        chunk["refinement"]="RS059 T1 introduced dw.tools.math.symbolics preview.5 exact real formal polynomial derivative up to degree eight, with d(x^3)/dx=3x^2, absent-symbol zero, and explicit unit/assumption/nonpolynomial refusals. RS060 T2 adds preview.6 exact rational pointwise derivative values and conservatively classified scientific boundaries: |x| and sqrt(x^2) are nondifferentiable at zero; sqrt(x) is outside its real domain for x<0 and nondifferentiable at zero; the original quotient hole x=1 stays excluded. Generic radical/quotient differentiation, unit arithmetic and numerical finite differences remain unsupported. Controlled RED + 37 GREEN, eight local NuGet-only oracles, and scoped symbolic/IR/quantity regressions close C01 without claiming W02/M4 completion.";
        chunk["readiness"]="closed: both T1 exact real formal derivative and T2 pointwise analytic-domain/units refusal boundary qualified; C02/C03 remain planned";
        var priorFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Project,Product,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W02C01T2",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var oldEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{RedProof,Qualified})
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

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M4-W02-C01-T1","done");
p.ProjectPlan.RequireNodeState("M4-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C03","not-ready");
foreach(var child in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS060" ||
       (string?)state["activeMilestone"]!="M4" ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS060 exact before/after C01 active cursor diverged.");
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
    var prefix="M4-W02-C01-T2-PKG-SHA256:"+id+":"+version+":";
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
