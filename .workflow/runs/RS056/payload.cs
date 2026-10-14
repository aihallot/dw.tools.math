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

const string Work="M4-W01", Phase="M4-W01-C02", Task="M4-W01-C02-T2";
const string Red="M4-W01-C02-T2-R", Green="M4-W01-C02-T2-G", Verify="M4-W01-C02-T2-V";
const string Backlog="docs/planning/backlog.json", State=".aura/workflow/state.json";
const string Prior="docs/planning/evidence/M4-W01-C02-T1-contract-qualified.json";
const string RedEvidence="docs/planning/evidence/M4-W01-C02-T2-boundary-red.json";
const string Qualified="docs/planning/evidence/M4-W01-C02-T2-boundaries-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicProviderResultAdmission.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W01C02T2RedTests.cs";
const string Tests="tests/projects/dw.tools.math.symbolics.tests/M4W01C02T2Tests.cs";
const string Decision="docs/planning/decisions/m4-w01-c02-t2-provider-admission.md";
const string Distribution="docs/distribution/symbolic-provider-result-admission.md";
const string ConsumerProject="tests/consumers/m4-w01-c02-t2/m4-provider-admission-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w01-c02-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w01-c02-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS056/staged/checks/m4-t2-product-sha.cs";
const string Solution="dw.tools.math.slnx";
const string Version="0.5.0-preview.2";
const string ExpectedRed="M4-W01-C02-T2 RED: untrusted symbolic provider output gate is absent.";
const string ConsumerMarker="dw.tools.math/m4-provider-admission-consumer/0.5 qualified; probes=8";
const int NewGreenTests=29, TargetedTests=30;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS056","M4",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS055",1,
    "feat: add exact restricted symbolic IR cancellation with pole-preserving consumer",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("typed-exact-symbolic-rational-cancellation-and-pole-exclusion-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(25));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(26));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/no_cas_provider_runtime_dependency",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_product_package_graph/2/id",
    JsonValue.Create("dw.tools.math.symbolics"));
p.Require.JsonValueEquals(Prior,"/local_product_package_graph/2/version",
    JsonValue.Create("0.5.0-preview.1"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.54"),JsonValue.Create("0.1.55"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.55":"0.1.54"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/28/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",JsonValue.Create("satisfied"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/qualified_by",
    JsonValue.Create("M4-W01-C01"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C02-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,Tests,Decision,Distribution,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedEvidence,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.1</Version>");
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("strict-typed-symbolic-provider-output-and-condition-admission-qualified"));
    p.Require.JsonValueEquals(Qualified,"/new_green_tests_passed",
        JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/external_provider_integration_qualified",
        JsonValue.Create(false));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.2</Version>");
}

var sourceHashes=new Dictionary<string,string>(StringComparer.Ordinal);
// First execute an actual compiled missing-public-contract RED against the
// already-qualified preview.1 product, before installing the new implementation.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
sourceHashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing typed RED source digest.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W01C02T2RedTests.PublicProviderOutputAdmissionMustRejectUnverifiedConditions",
        "--logger","console;verbosity=normal");
    if(red.Code==0||!red.Output.Contains(ExpectedRed,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Compiled missing-gate RED was not independently observed: "+Relevant(red.Output));
    var redReceipt=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C02-T2-boundary-red",
        ["status"]="compiled-untrusted-symbolic-output-gate-absent-red-observed",
        ["prior_exact_bridge_evidence"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=sourceHashes[RedTest],
        ["product_version_before_green"]="0.5.0-preview.1",
        ["missing_public_gate"]="Dw.Tools.Math.Symbolics.SymbolicProviderResultAdmission.TryAdmit(IrNode,object)",
        ["red_failure_assertion"]=ExpectedRed,
        ["independent_red_assertions"]=1,
        ["red_exited_nonzero"]=true,
        ["product_was_compilable_before_green"]=true,
        ["boundary"]="Absence of a typed public admission gate is not a scientific equivalence test."
    };
    p.Files.WriteComplete(RedEvidence,
        redReceipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var path in new[]{Project,Product,Tests,Decision,Distribution,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var result=p.Files.ReplaceFromStaged("staged/"+path,path);
    sourceHashes[path]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing Payload SDK staged-file SHA-256: "+path);
}

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W01C02T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 30",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 30",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected 1 recovered RED and 29 independent GREEN tests.");
Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var related in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var output=Required(root,"dotnet","test",related,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!output.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Missing scientific regression result: "+related);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var products=new (string Id,string Version,string Csproj,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,Project,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-w01-c02-t2-local-feed");
var archives=new List<string>();
foreach(var product in products)
{
    Required(root,"dotnet","pack",product.Csproj,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,product.Id+"."+product.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Generated local NuGet package absent: "+product.Id);
    archives.Add(archive);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var packageEvidence=Required(root,"dotnet",helperArgs.ToArray());
var hashes=new List<JsonNode>();
foreach(var product in products)
{
    var archive=RequirePackageReceipt(packageEvidence,product.Id,product.Version);
    hashes.Add(new JsonObject
    {
        ["id"]=product.Id,["version"]=product.Version,
        ["bytes"]=archive.Bytes,["sha256"]=archive.Sha,
        ["dependencies"]=p.Json.StringArray(product.Dependencies),
        ["assembly"]="lib/net10.0/"+product.Id+".dll",
        ["publication"]="local-ignored-artifact-not-transferred"
    });
}
var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var packageReferences=consumerXml.Descendants()
    .Where(node=>node.Name.LocalName=="PackageReference").ToArray();
if(packageReferences.Length!=1 ||
   (string?)packageReferences[0].Attribute("Include")!="dw.tools.math.symbolics"||
   (string?)packageReferences[0].Attribute("Version")!="["+Version+"]"||
   consumerXml.Descendants().Any(node=>node.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Independent T2 client must have one exact symbolics NuGet reference.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig)).Descendants()
    .Single(node=>node.Name.LocalName=="packageSources");
if(!sources.Elements().Any(node=>node.Name.LocalName=="clear")||
   sources.Elements().Any(node=>node.Name.LocalName=="add"))
    throw new InvalidOperationException("T2 client must refuse external package feeds.");
var cache=Path.Combine(root,"build","artifacts","m4-w01-c02-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,
    "--source",feed,"--configfile",ConsumerConfig,"--packages",cache);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight NuGet-only provider admission oracles did not pass.");

if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C02-T2-boundaries-qualified",
        ["status"]="strict-typed-symbolic-provider-output-and-condition-admission-qualified",
        ["prior_exact_symbolics_evidence"]=Prior,
        ["controlled_red_evidence"]=RedEvidence,
        ["question"]="How can a proposed external symbolic result be refused unless the complete native typed condition matches an independently proven exact source transformation?",
        ["observation"]="One independently compiled missing-gate RED is recovered, 29 further exact/domain/scope/opaque-provider GREEN tests pass and eight package-only oracles verify that no untrusted text or lost domain is admitted.",
        ["decision"]="Admit only a native IrRestrictedExpression exactly identical to a freshly recomputed Math proof; refuse provider text, foreign AST, missing/wrong/extra conditions and contradictory or unsupported sources with explicit statuses.",
        ["new_product_source"]=Product,["new_product_source_sha256"]=sourceHashes[Product],
        ["product_project"]=Project,["product_project_sha256"]=sourceHashes[Project],
        ["new_green_tests"]=Tests,["new_green_tests_sha256"]=sourceHashes[Tests],
        ["controlled_red_test"]=RedTest,["controlled_red_test_sha256"]=sourceHashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=sourceHashes[Decision],
        ["distribution_document"]=Distribution,
        ["distribution_document_sha256"]=sourceHashes[Distribution],
        ["new_green_tests_passed"]=NewGreenTests,
        ["red_recovered_and_green_tests_passed"]=TargetedTests,
        ["complete_symbolics_suite_passed"]=true,
        ["existing_ir_quantities_regressions_passed"]=true,
        ["compiled_red_observed"]=true,
        ["untrusted_text_rejected"]=true,
        ["hostile_object_tostring_never_called"]=true,
        ["foreign_ast_rejected"]=true,
        ["unconditional_candidate_rejected"]=true,
        ["missing_or_changed_conditions_rejected"]=true,
        ["unrelated_symbol_identity_rejected"]=true,
        ["source_assumptions_preserved"]=true,
        ["contradictions_and_budgets_typed"]=true,
        ["canonical_typed_ir_roundtrip_passed"]=true,
        ["accepted_output_is_locally_proven_fresh_object"]=true,
        ["external_provider_integration_qualified"]=false,
        ["no_provider_product_runtime_dependency"]=true,
        ["local_package_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(hashes.ToArray()),
        ["package_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=sourceHashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=sourceHashes[ConsumerSource],
        ["consumer_nuget_config"]=ConsumerConfig,
        ["consumer_nuget_config_sha256"]=sourceHashes[ConsumerConfig],
        ["independent_consumer_probes_passed"]=8,
        ["consumer_marker"]=ConsumerMarker,
        ["source_only_nuget_consumer"]=true,
        ["packages_publicly_published"]=false,
        ["locked_solution_build_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W01-C03-T1 bounded symbolic simplification, expansion and substitution with scope preservation",
        ["nonclaims"]=p.Json.StringArray(
            "No external Angouri or MathNET Symbolics object can be automatically decoded as Math IR from its string or opaque type.",
            "No general CAS mathematical equivalence, foreign provider algorithm, executable callback, broad domain solver, or principal complex branch.",
            "No AURA/Decision/MCDM ownership change, neutral dw.discovery skill, public NuGet publication, broker authorization or native CAS interruption.",
            "No completed M4-W01, M4 symbolic release or claim of cross-platform provider behavior.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.55";
        var chunk=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Task);
        chunk["status"]="done";
        chunk["refinement"]="RS055 T1 qualified exact provider-free typed cancellation (x²-c²)/(x-c) to restricted x+c with x!=c, 25 scientific tests and eight NuGet-only oracles. RS056 T2 adds the strictly checked external-result admission API: only a native canonical IrRestrictedExpression exactly equal to the fresh local T1 proof is admitted, while opaque foreign objects, provider 'provided' strings, missing/extra/wrong conditions, foreign identities and unsupported domains are typed refusals without invoking untrusted ToString. A compiled RED, 29 further GREEN tests, eight independent preview.2 consumer probes, locked IR/quantities regressions, three actual NuGet manifest/hash receipts and unchanged CAS isolation close C02. Angouri provider integration and general symbolic proof remain explicitly unqualified.";
        chunk["readiness"]="closed: T1 structural exact IR cancellation and T2 strict independent provider-output admission boundary qualified; C03 symbolic transformations still planned";
        var files=chunk["files"]!.AsArray().Select(v=>v!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(files.Concat(new[]{
            Project,Product,RedTest,Tests,Decision,Distribution,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedEvidence,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W01C02T2",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var oldProofs=chunk["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(oldProofs.Concat(new[]{RedEvidence,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedEvidence:Qualified);
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
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState("M4-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var child in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS056"||
       (string?)state["activeMilestone"]!="M4"||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS056 expected exact C02 baseline or done cursor.");
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

static (long Bytes,string Sha) RequirePackageReceipt(
    string output,string id,string version)
{
    var prefix="M4-T2-PKG-SHA256:"+id+":"+version+":";
    var matching=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matching.Length!=1)
        throw new InvalidOperationException("One observed local product package receipt required for "+id);
    var items=matching[0][prefix.Length..].Split(':');
    if(items.Length!=2 ||
       !long.TryParse(items[0],NumberStyles.None,CultureInfo.InvariantCulture,out var size)||
       size<=0 || items[1].Length!=64 ||
       items[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid local NuGet SHA-256 output for "+id);
    return(size,items[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
    return part.Length>2400?part[..2400]:part;
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
