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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C01",Task="M6-W01-C01-T2";
const string Red="M6-W01-C01-T2-R",Green="M6-W01-C01-T2-G",Verify="M6-W01-C01-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M6-W01-C01-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-curve-explicit-unit-conditioning-extrapolation-refusals-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.2";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Source="src/projects/dw.tools.math.analysis/CurveBoundaryAudit64.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C01T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C01T2Tests.cs";
const string OriginalConsumer="tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj";
const string OriginalSource="tests/consumers/m6-w01-c01-t1/Program.cs";
const string OriginalConfig="tests/consumers/m6-w01-c01-t1/NuGet.Config";
const string Consumer="tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w01-c01-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c01-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c01-t2-curve-boundaries.md";
const string Recipe="docs/distribution/m6-curve-boundaries.md";
const string PackageHelper=".workflow/runs/RS082/staged/checks/m6-w01-c01-t2-package-sha.cs";
const string RedMarker="M6-W01-C01-T2 RED: the public unit, extrapolation and relative-conditioning audit API is absent.";
const string ConsumerMarker="dw.tools.math/m6-curve-boundary-consumer/1.1 qualified; probes=8";
const string OriginalMarker="dw.tools.math/m6-curve-analysis-consumer/1.1 qualified; probes=8";
const int IndependentGreen=59,AllTargeted=60;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS082",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS081",1,
    "feat: add bounded numerical interpolation and regression contracts and open M6",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-binary64-linear-natural-spline-and-ols-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(47));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(48));
p.Require.JsonValueEquals(Prior,"/independent_nuget_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.1"));
p.Require.JsonValueEquals(Prior,"/m6_w01_c01_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w01_c01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/packages_publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.80"),JsonValue.Create("0.1.81"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.81":"0.1.80"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/13/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/14/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/40/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/43/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.2</Version>":
    "<Version>1.1.0-preview.1</Version>");
p.Require.TextContains(OriginalConsumer,target?'Version="[1.1.0-preview.2]"':
    'Version="[1.1.0-preview.1]"');
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Consumer,ConsumerSource,
        ConsumerConfig,Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-curve-boundary-public-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/independent_boundary_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/retained_t1_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/local_product_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c01_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c01_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redStaged=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redStaged.AfterSha256 ??
    throw new InvalidOperationException("Missing compiled public boundary RED source SHA.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C01T2RedTests.PublicUnitAndConditioningBoundaryAuditMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 || !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One genuinely compiled missing-public-boundary-API RED is required: "+
            Relevant(redOutput.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C01-T2-contract-red",
        ["status"]="compiled-curve-boundary-public-api-absent-red-observed",
        ["prior_qualified_t1"]=Prior,
        ["compiled_predecessor_version"]="dw.tools.math.analysis/1.1.0-preview.1",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_red_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,OriginalConsumer,OriginalSource,
    Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing bounded Math curve T2 source identity: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W01C01T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 60",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 60",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("T2 requires one compiled RED recovered plus 59 new independent GREEN.");
var retained=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C01T1",
    "--logger","console;verbosity=normal");
if(!retained.Contains("Total tests: 48",StringComparison.OrdinalIgnoreCase) ||
   !retained.Contains("Passed: 48",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Original qualified T1 forty-seven GREEN and compiled RED tests regressed.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regression=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regression.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained core Math regression refused: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m6-w01-c01-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release",
    "--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Exact local preview.2 analysis archive not generated.");
var inspected=Required(root,"dotnet","run","--file",PackageHelper,"--",archive);
var inspection=ReadProductReceipt(inspected,"dw.tools.math.analysis",Version);
static void RequireLocalConsumer(
    string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var reference=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(reference.Length!=1 ||
       (string?)reference[0].Attribute("Include")!="dw.tools.math.analysis" ||
       (string?)reference[0].Attribute("Version")!="["+version+"]" ||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Only one pinned, source-free Math analysis NuGet package is admissible: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear") ||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External NuGet sources are forbidden: "+config);
}
var consumers=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W01-C01-T2-CONSUMER-SHA256:","m6-w01-c01-t2-boundary-cache"),
    (OriginalConsumer,OriginalConfig,OriginalMarker,
        "M6-W01-C01-T1-CONSUMER-SHA256:","m6-w01-c01-t2-retained-cache")
};
var consumerReceipts=new List<JsonNode>();
foreach(var client in consumers)
{
    RequireLocalConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var result=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(result,client.Marker))
        throw new InvalidOperationException("Eight source-free curve oracles absent: "+client.Project);
    var receipt=ReadProductReceipt(result,"dw.tools.math.analysis",Version,client.Receipt);
    if(receipt.Bytes!=inspection.Bytes ||
       !string.Equals(receipt.Sha,inspection.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("Package bytes and SHA disagree across independent clients: "+client.Project);
    consumerReceipts.Add(new JsonObject
    {
        ["project"]=client.Project,["marker"]=client.Marker,
        ["product_sha256"]=receipt.Sha,["product_bytes"]=receipt.Bytes,
        ["independent_probes"]=8,["source_project_references"]=0
    });
}
if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C01-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_t1_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can the qualified Math line/OLS package apply explicit finite conditioning, declared coordinate units and opt-in linear-only extrapolation without a false forecast or partial outputs?",
        ["observation"]="One genuine missing-API RED recovered; 59 boundary GREEN, 48 retained T1 tests, four core Math suites, two distinct eight-probe NuGet consumers and actual preview.2 SHA-qualified archive passed.",
        ["decision"]="Admit explicit linear-segment extrapolation only on caller request, case-sensitive unit declarations with no conversion, finite relative-span threshold and typed refusal for near-rank/invalid data; no causal or predictive guarantee.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_t1_consumer_project"]=OriginalConsumer,
        ["retained_t1_consumer_project_sha256"]=hashes[OriginalConsumer],
        ["retained_t1_consumer_source"]=OriginalSource,
        ["retained_t1_consumer_source_sha256"]=hashes[OriginalSource],
        ["boundary_consumer_project"]=Consumer,
        ["boundary_consumer_project_sha256"]=hashes[Consumer],
        ["boundary_consumer_source"]=ConsumerSource,
        ["boundary_consumer_source_sha256"]=hashes[ConsumerSource],
        ["boundary_consumer_config"]=ConsumerConfig,
        ["boundary_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["retained_t1_tests_passed"]=48,
        ["targeted_existing_regression_projects_passed"]=4,
        ["independent_boundary_consumer_probes"]=8,
        ["retained_t1_consumer_probes"]=8,
        ["actual_inspected_package_archive_bytes"]=inspection.Bytes,
        ["actual_inspected_package_archive_sha256"]=inspection.Sha,
        ["product_has_no_external_nuget_dependencies"]=true,
        ["isolated_nuget_consumer_receipts"]=p.Json.Array(consumerReceipts.ToArray()),
        ["local_product_version"]=Version,
        ["accepted_default_curve_value_at_half"]=2,
        ["accepted_explicit_linear_extrapolated_value_at_three"]=7,
        ["accepted_ols_slope"]=2,
        ["accepted_ols_intercept"]=1,
        ["accepted_ols_zero_sse"]=true,
        ["unit_query_mismatch_refused"]=true,
        ["unit_conversion_performed"]=false,
        ["near_rank_relative_threshold_refusal_verified"]=true,
        ["explicit_extrapolation_required"]=true,
        ["spline_extrapolation_admitted"]=false,
        ["weighted_fit_admitted"]=false,
        ["causal_or_forecast_guarantee_admitted"]=false,
        ["maximum_samples"]=32,
        ["local_package_only"]=true,
        ["packages_publicly_published"]=false,
        ["m6_w01_c01_t2_done"]=true,
        ["m6_w01_c01_phase_done"]=true,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M6-W01-C02-T1: bounded univariate numerical root finding and quadrature with residual/tolerance policies",
        ["nonclaims"]=p.Json.StringArray(
            "Caller unit tags are not independently verified physical units and do not trigger automatic conversion.",
            "A relative abscissa threshold does not mathematically prove reliable conditioning of all admitted splines or fits.",
            "Opt-in LINEAR extrapolation does not provide a forecast, accuracy interval or causal model.",
            "M6-W01 and M6 remain in-progress; no public NuGet, native provider or unobserved OS is certified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.81";
        var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M6-W01-C01 T2 accepted baseline changed.");
        chunk["status"]="done";
        chunk["refinement"]="RS081 T1 delivered real provider-free finite binary64 line, natural cubic spline and unweighted OLS with 47 scientific GREENs and an eight-oracle local NuGet consumer. RS082 T2 independently qualifies declared axis-unit labels and exact-match query units, explicit caller-selected LINEAR segment extrapolation, finite policy/conditioned relative X-span refusal and no causal or predictive inference. One compiled RED, 59 GREENs, retained 48 T1 tests, four Math regression suites, two eight-oracle source-free consumers and verified preview.2 nupkg SHA close only C01. M6-W01/M6 remain in-progress; C02 numerical roots/integration is the next scoped task.";
        chunk["readiness"]="C01 complete: T1 numerical analysis and T2 unit/extrapolation/conditioning policies with typed nonpartial refusals, real local NuGet SHA and both source-free client receipts; M6-W01-C02 planned.";
        var originalFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(originalFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,OriginalConsumer,OriginalSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C01T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+OriginalConsumer+" -c Release");
        var evidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(evidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var item in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            item["status"]="done";
            item["evidence"]=p.Json.StringArray(
                (string?)item["id"]==Red?RedProof:Qualified);
        }
    });
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
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS082" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null||state["activeSubtask"] is not null)
        throw new InvalidOperationException("M6 curve T2 original phase cursor diverged.");
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
    string output,string id,string version,
    string marker="M6-W01-C01-T2-PKG-SHA256:")
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
