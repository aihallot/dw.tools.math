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

const string Work="M4-W01",Phase="M4-W01-C03",Task="M4-W01-C03-T1";
const string Red="M4-W01-C03-T1-R",Green="M4-W01-C03-T1-G",
    Verify="M4-W01-C03-T1-V";
const string Backlog="docs/planning/backlog.json",State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M4-W01-C02-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M4-W01-C03-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M4-W01-C03-T1-contract-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicExactTransformations.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W01C03T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W01C03T1Tests.cs";
const string Decision="docs/planning/decisions/m4-w01-c03-t1-exact-transformations.md";
const string Document="docs/distribution/symbolic-exact-transformations.md";
const string ConsumerProject="tests/consumers/m4-w01-c03-t1/m4-exact-transform-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w01-c03-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w01-c03-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS057/staged/checks/m4-c03-product-sha.cs";
const string Solution="dw.tools.math.slnx",Version="0.5.0-preview.3";
const string RedMarker="M4-W01-C03-T1 RED: bounded exact symbolic transformations API is absent.";
const string ConsumerMarker="dw.tools.math/m4-exact-transform-consumer/0.5 qualified; probes=8";
const int NewGreenTests=28,TotalTargeted=29;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

p.Workflow.RequireCurrentRun("RS057","M4",Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS056",1,
    "feat: qualify typed symbolic provider output admission and close C02",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("strict-typed-symbolic-provider-output-and-condition-admission-qualified"));
p.Require.JsonValueEquals(Prior,"/new_green_tests_passed",JsonValue.Create(29));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(30));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/external_provider_integration_qualified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/no_provider_product_runtime_dependency",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_package_version",
    JsonValue.Create("0.5.0-preview.2"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.55"),JsonValue.Create("0.1.56"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.56":"0.1.55"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/tasks/1/status",JsonValue.Create("planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/29/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",
    JsonValue.Create("satisfied"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState("M4-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C03-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.2</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-exact-symbolic-transformations-public-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("exact-binomial-transformation-free-symbol-substitution-and-real-sqrt-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",JsonValue.Create(8));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.3</Version>");
}

var stageHashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
stageHashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing typed RED test digest.");

if(!target)
{
    // A controlled RED must fail a test assertion against the actually
    // compiled, previously qualified symbolics preview.2 product.
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redRun=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W01C03T1RedTests.PublicExactTransformationsContractMustExist",
        "--logger","console;verbosity=normal");
    if(redRun.Code==0 ||
       !redRun.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "The expected compiled missing-API RED was not observed: "+Relevant(redRun.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C03-T1-contract-red",
        ["status"]="compiled-exact-symbolic-transformations-public-contract-absent-red-observed",
        ["prior_symbolics_bridge"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=stageHashes[RedTest],
        ["real_compiled_product_version"]="0.5.0-preview.2",
        ["missing_public_api"]="Dw.Tools.Math.Symbolics.SymbolicExactTransformations.ExpandSquare(IrNode)",
        ["expected_red_assertion"]=RedMarker,
        ["failed_red_test_count"]=1,
        ["red_exited_nonzero"]=true,
        ["implementation_not_staged_before_red"]=true,
        ["nonclaim"]="An absent API is not proof of symbolic equivalence or general CAS support."
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Project,Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    stageHashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing typed staged-source SHA-256: "+file);
}

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");

var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W01C03T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 29",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 29",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected one recovered RED and 28 distinct GREEN tests.");

Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var project in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var receipt=Required(root,"dotnet","test",project,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!receipt.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Relevant typed-IR or exact regression was not observed: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var products=new (string Id,string PackageVersion,string Csproj,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,Project,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-w01-c03-t1-local-feed");
var archives=new List<string>();
foreach(var product in products)
{
    Required(root,"dotnet","pack",product.Csproj,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,product.Id+"."+product.PackageVersion+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Missing actual local Math package: "+product.Id);
    archives.Add(archive);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var hashesOutput=Required(root,"dotnet",helperArgs.ToArray());
var observedPackages=new List<JsonNode>();
foreach(var product in products)
{
    var info=ReadProductReceipt(hashesOutput,product.Id,product.PackageVersion);
    observedPackages.Add(new JsonObject
    {
        ["id"]=product.Id,["version"]=product.PackageVersion,
        ["bytes"]=info.Bytes,["sha256"]=info.Sha,
        ["assembly"]="lib/net10.0/"+product.Id+".dll",
        ["dependencies"]=p.Json.StringArray(product.Dependencies),
        ["availability"]="local-ignored-artifact-not-published"
    });
}

var clientXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var clientReferences=clientXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(clientReferences.Length!=1 ||
   (string?)clientReferences[0].Attribute("Include")!="dw.tools.math.symbolics" ||
   (string?)clientReferences[0].Attribute("Version")!="["+Version+"]" ||
   clientXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("The M4 transform consumer must use only exact NuGet packages.");
var packageSources=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!packageSources.Elements().Any(x=>x.Name.LocalName=="clear") ||
   packageSources.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("NuGet consumer must clear external feeds.");
var isolatedCache=Path.Combine(root,"build","artifacts","m4-w01-c03-t1-consumer-packages");
Required(root,"dotnet","restore",ConsumerProject,
    "--source",feed,"--configfile",ConsumerConfig,"--packages",isolatedCache);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerReceipt=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasLine(consumerReceipt,ConsumerMarker))
    throw new InvalidOperationException("Eight independent local NuGet exact-transform probes not observed.");

if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C03-T1-contract-qualified",
        ["status"]="exact-binomial-transformation-free-symbol-substitution-and-real-sqrt-qualified",
        ["prior_symbolic_output_admission"]=Prior,
        ["controlled_red_evidence"]=RedProof,
        ["question"]="Which exact expansion, factorization, substitution and real sqrt transformations can be qualified in closed typed Math IR without silently losing conditions or depending on a general CAS?",
        ["observation"]="One compiled absent-API RED became GREEN, 28 independent mathematical tests passed and an eight-probe NuGet-only consumer verified the preview.3 local package with exact typed IR/assumption semantics.",
        ["decision"]="Admit only the closed exact binomial-square expansion/factor rules, refusal-safe free-symbol substitution and real sqrt(square(x)) conditional normalization; leave broad capture-avoiding binders, resource budgets and provider equivalence to separately admitted work.",
        ["source"]=Product,["source_sha256"]=stageHashes[Product],
        ["project"]=Project,["project_sha256"]=stageHashes[Project],
        ["green_test"]=GreenTests,["green_test_sha256"]=stageHashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=stageHashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=stageHashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=stageHashes[Document],
        ["independent_green_tests_passed"]=NewGreenTests,
        ["red_recovered_and_green_tests_passed"]=TotalTargeted,
        ["controlled_red_then_green"]=true,
        ["exact_rules"]=p.Json.StringArray(
            "binomial-square/exact/v1","binomial-factor/exact/v1",
            "free-symbol-substitute/closed/v1","sqrt-real-square/domain/v1"),
        ["coefficient_maximum_bits"]=128,
        ["expanded_three_term_binomial_oracle"]="(x+1)^2 -> x^2+2*x+1",
        ["exact_fraction_oracle"]="(x+1/2)^2 -> x^2+x+1/4",
        ["factor_round_trip_tested"]=true,
        ["substitution_uses_symbol_identity"]=true,
        ["bound_scope_and_unsafe_restricted_substitution_refused"]=true,
        ["prior_assumptions_preserved"]=true,
        ["real_sqrt_square_normalized_to_abs"]=true,
        ["nonnegative_sqrt_assumption_kept_typed"]=true,
        ["unproved_rules_return_unsupported"]=true,
        ["general_binder_alpha_renaming_qualified"]=false,
        ["external_CAS_runtime_required"]=false,
        ["full_symbolics_test_suite_passed"]=true,
        ["typed_ir_and_exact_quantities_regressions_passed"]=true,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(observedPackages.ToArray()),
        ["post_packaging_manifest_sha_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=stageHashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=stageHashes[ConsumerSource],
        ["consumer_nuget_config"]=ConsumerConfig,
        ["consumer_nuget_config_sha256"]=stageHashes[ConsumerConfig],
        ["independent_consumer_probes_passed"]=8,
        ["consumer_exact_output"]=ConsumerMarker,
        ["source_only_nuget_client"]=true,
        ["packages_publicly_published"]=false,
        ["locked_solution_build_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W01-C03-T2 bounded rules, proof provenance, assumption/cancellation and capture-deferral tests",
        ["nonclaims"]=p.Json.StringArray(
            "No unrestricted factoring, general polynomial solver, arbitrary simplifier, general capture-avoiding substitution or alpha-renaming.",
            "No external Angouri or MathNET Symbolics AST/string translation, native cooperative cancellation, complex principal-branch contract or unrestricted compute.",
            "No public NuGet feed, M4-W01/C03 completion, AURA/Decision/MCDM integration, neutral dw.discovery metadata or BrokerReady agent skill.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.56";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        chunk["status"]="in_progress";
        chunk["refinement"]="RS055 and RS056 qualified the provider-neutral exact restricted rational bridge and strict proposed-result admission in preview.2, without a CAS runtime. RS057 T1 now adds four exact closed preview.3 rules: binomial-square expansion, canonical factoring, source-identity free symbol substitution with capture/restriction refusal, and real sqrt(square(x)) -> abs(x) unless an explicit nonnegative typed assumption permits x. A real compiled missing-contract RED recovers with 28 GREEN tests, eight isolated NuGet client probes, relevant IR/quantity suites, three local package SHA-256s, and explicit limits. T2 remains for deeper budget/proof/cancellation/condition interoperability boundaries before closing C03.";
        chunk["readiness"]="in progress: bounded exact transformations T1 qualified; T2 algorithmic proof, assumptions and budget limitations pending";
        var existing=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(existing.Concat(new[]{
            Project,Product,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W01C03T1",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        chunk["evidence"]=p.Json.StringArray(RedProof,Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedProof:Qualified);
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

p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C03-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");

p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS057"||
       (string?)state["activeMilestone"]!="M4"||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?Phase:null)||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS057 active project cursor must match exact baseline or target.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasLine(string output,string marker) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);

static (long Bytes,string Sha) ReadProductReceipt(string output,string id,string version)
{
    var prefix="M4-C03-PKG-SHA256:"+id+":"+version+":";
    var matching=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matching.Length!=1)
        throw new InvalidOperationException("One independent product package receipt required for "+id);
    var parts=matching[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid SHA-256/byte receipt from actual package output "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
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
        throw new TimeoutException(executable+" timed out.");
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
