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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C01",Task="M6-W01-C01-T1";
const string Red="M6-W01-C01-T1-R",Green="M6-W01-C01-T1-G",Verify="M6-W01-C01-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-gate.json";
const string RedProof="docs/planning/evidence/M6-W01-C01-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json";
const string QualifiedStatus="bounded-binary64-linear-natural-spline-and-ols-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.1";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string ProductLock="src/projects/dw.tools.math.analysis/packages.lock.json";
const string Point="src/projects/dw.tools.math.analysis/CurvePoint64.cs";
const string Source="src/projects/dw.tools.math.analysis/M6W01C01Contract.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.analysis.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C01T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C01T1Tests.cs";
const string Consumer="tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj";
const string ConsumerCode="tests/consumers/m6-w01-c01-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c01-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c01-t1-curve-analysis.md";
const string Recipe="docs/distribution/m6-curve-analysis.md";
const string PackageHelper=".workflow/runs/RS081/staged/checks/m6-w01-c01-t1-package-sha.cs";
const string RedMarker="M6-W01-C01-T1 RED: bounded public linear/spline interpolation and least-squares analysis API is absent.";
const string ConsumerMarker="dw.tools.math/m6-curve-analysis-consumer/1.1 qualified; probes=8";
const int IndependentGreen=47,AllTargeted=48;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS081",
    target?Milestone:null,target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS080",1,
    "feat: qualify independent local G-M5 science gate and complete M5",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m5-local-v1-independent-nuget-evidence-gate-qualified-unpublished"));
p.Require.JsonValueEquals(Prior,"/gate",JsonValue.Create("G-M5"));
p.Require.JsonValueEquals(Prior,"/qualified_m5_phases",JsonValue.Create(7));
p.Require.JsonValueEquals(Prior,"/qualified_tasks",JsonValue.Create(14));
p.Require.JsonValueEquals(Prior,"/qualified_subtasks",JsonValue.Create(42));
p.Require.JsonValueEquals(Prior,"/original_v1_criteria_checked",JsonValue.Create(10));
p.Require.JsonValueEquals(Prior,"/isolated_nuget_consumer_probes",JsonValue.Create(24));
p.Require.JsonValueEquals(Prior,"/m5_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/stable_release_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/owner_stable_publication_approved",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.79"),JsonValue.Create("0.1.80"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.80":"0.1.79"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/releases/7/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/13/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/14/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/tasks/1/status",JsonValue.Create("planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/40/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/43/status",JsonValue.Create("planned"));
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.ProjectPlan.RequireNodeState("M7","not-ready");

if(!target)
{
    foreach(var file in new[]{Product,ProductLock,Point,Source,Test,TestLock,
        RedTest,GreenTests,Consumer,ConsumerCode,ConsumerConfig,Decision,
        Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Solution,
        "<Project Path=\"src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj\" />");
}
else
{
    p.Require.TextContains(Solution,
        "<Project Path=\"src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj\" />");
    p.Require.TextContains(Product,"<Version>1.1.0-preview.1</Version>");
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-public-bounded-curve-analysis-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/independent_nuget_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c01_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c01_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
}
var sha=new Dictionary<string,string>(StringComparer.Ordinal);
// Only the compilation-ready project skeleton and RED oracle are present
// at this stage. The actual public analysis operation API does not exist.
foreach(var file in new[]{Solution,Product,ProductLock,Point,Test,TestLock,RedTest})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    sha[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing immutable scoped project/test source SHA: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C01T1RedTests.BoundedPublicLinearSplineAndLeastSquaresContractMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0||!redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One actual compiled absent-analysis-API RED is required: "+
            Relevant(redOutput.Output));
    var redReceipt=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C01-T1-contract-red",
        ["status"]="compiled-public-bounded-curve-analysis-api-absent-red-observed",
        ["prior_local_m5_gate"]=Prior,
        ["compiled_empty_analysis_package"]=Version,
        ["compiled_red_test"]=RedTest,["compiled_red_test_sha256"]=sha[RedTest],
        ["missing_public_api"]="BoundedCurveAnalysis64.InterpolateLinear / InterpolateNaturalSpline / FitLeastSquares",
        ["expected_red_assertion"]=RedMarker,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["independent_compiled_red_tests"]=1
    };
    p.Files.WriteComplete(RedProof,
        redReceipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Source,GreenTests,Consumer,ConsumerCode,
    ConsumerConfig,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    sha[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("New bounded analysis product scientific SHA absent: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C01T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 48",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 48",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one compiled RED recovered and 47 independent numeric/spline/OLS GREEN oracles.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var output=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!output.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Prior Math analysis/typed IR numerical regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m6-w01-c01-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release",
    "--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("New local Math analysis nupkg missing.");
var inspected=Required(root,"dotnet","run","--file",PackageHelper,"--",archive);
var productReceipt=ReadProductReceipt(inspected,"dw.tools.math.analysis",Version);
var consumerXml=XDocument.Load(Path.Combine(root,Consumer));
var references=consumerXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
    (string?)references[0].Attribute("Include")!="dw.tools.math.analysis" ||
    (string?)references[0].Attribute("Version")!="["+Version+"]" ||
    consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Only one explicit pinned NuGet analysis dependency and zero source refs are admitted.");
var feeds=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!feeds.Elements().Any(x=>x.Name.LocalName=="clear") ||
    feeds.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException(
        "External NuGet sources are forbidden for independent analysis consumption.");
var isolated=Path.Combine(root,"build","artifacts","m6-w01-c01-t1-consumer-packages");
Required(root,"dotnet","restore",Consumer,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",Consumer,"-c","Release","--no-restore");
var observed=Required(root,"dotnet","run","--project",Consumer,
    "-c","Release","--no-build","--no-restore","--",archive);
if(!HasExactLine(observed,ConsumerMarker))
    throw new InvalidOperationException("Eight independent source-free Math analysis probes not present.");
var consumerReceipt=ReadProductReceipt(observed,"dw.tools.math.analysis",
    Version,"M6-W01-C01-T1-CONSUMER-SHA256:");
if(consumerReceipt.Bytes!=productReceipt.Bytes ||
   !string.Equals(consumerReceipt.Sha,productReceipt.Sha,StringComparison.Ordinal))
    throw new InvalidOperationException(
        "Exact nupkg bytes/SHA-256 differ between independent inspector and NuGet-only consumer.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C01-T1-contract-qualified",
        ["status"]=QualifiedStatus,["prior_m5_gate"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can a new provider-free net10 Math analysis package interpolate and fit 2-32 ordered finite binary64 points with bounded typed refusals and a source-free consumer?",
        ["observation"]="A compiled missing-API RED recovered, 47 independent linear/natural spline/OLS GREEN tests, relevant numerical/IR/quantities/composition regressions and eight standalone NuGet probes passed.",
        ["decision"]="Admit finite binary64 linear/natural cubic interpolation and descriptive unweighted least squares only within accepted domain and budgets, without extrapolation, units or inference guarantees.",
        ["solution"]=Solution,["solution_sha256"]=sha[Solution],
        ["product_project"]=Product,["product_project_sha256"]=sha[Product],
        ["product_lock"]=ProductLock,["product_lock_sha256"]=sha[ProductLock],
        ["product_sample"]=Point,["product_sample_sha256"]=sha[Point],
        ["product_source"]=Source,["product_source_sha256"]=sha[Source],
        ["tests_project"]=Test,["tests_project_sha256"]=sha[Test],
        ["tests_lock"]=TestLock,["tests_lock_sha256"]=sha[TestLock],
        ["red_test"]=RedTest,["red_test_sha256"]=sha[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=sha[GreenTests],
        ["consumer_project"]=Consumer,["consumer_project_sha256"]=sha[Consumer],
        ["consumer_source"]=ConsumerCode,["consumer_source_sha256"]=sha[ConsumerCode],
        ["consumer_config"]=ConsumerConfig,["consumer_config_sha256"]=sha[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=sha[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=sha[Recipe],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["targeted_existing_numerical_regression_projects_passed"]=4,
        ["independent_nuget_consumer_probes_passed"]=8,
        ["package_archive_inspected"]=true,
        ["product_package_id"]="dw.tools.math.analysis",
        ["local_product_version"]=Version,
        ["product_archive_bytes"]=productReceipt.Bytes,
        ["product_archive_sha256"]=productReceipt.Sha,
        ["product_assembly"]="lib/net10.0/dw.tools.math.analysis.dll",
        ["package_inspector"]=PackageHelper,
        ["product_has_no_external_nuget_dependencies"]=true,
        ["consumer_has_no_project_references"]=true,
        ["accepted_linear_interpolation_at_half"]=2,
        ["accepted_spline_interpolation_at_half"]=2,
        ["accepted_ols_slope"]=2,
        ["accepted_ols_intercept"]=1,
        ["accepted_ols_zero_sse"]=true,
        ["natural_curved_spline_value_at_half"]=0.6875,
        ["maximum_samples"]=32,
        ["bounded_binary64_only"]=true,
        ["implicit_extrapolation_admitted"]=false,
        ["weighted_regression_certified"]=false,
        ["unit_conversion_certified"]=false,
        ["causal_or_predictive_inference_certified"]=false,
        ["source_free_local_package_only"]=true,
        ["packages_publicly_published"]=false,
        ["m6_w01_c01_t1_done"]=true,
        ["m6_w01_c01_phase_done"]=false,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M6-W01-C01-T2: independent abscissa, conditioning, extrapolation and unit policy/admission boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "Only 2-32 strictly increasing finite points, binary64 numeric outputs; no exact rational or arbitrary spline proof.",
            "No implicit extrapolation, weighted fitting, automated unit conversion, prediction or causality.",
            "No external CAS, AURA, Decision/MCDM owner replacement or unqualified provider dependency.",
            "M6-W01-C01-T2, M6 work packages, M6 release, stable/public NuGet and other OS support remain unqualified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.80";
        var release=plan["releases"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Milestone);
        var work=plan["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        var phase=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)release["status"]!="planned" ||
           (string?)work["status"]!="planned" ||
           (string?)phase["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException(
                "M6 initial analysis product/T1 native and roadmap baseline drifted.");
        release["status"]="in_progress";
        work["status"]="in_progress";
        phase["status"]="in_progress";
        phase["refinement"]="RS081 T1 introduces a real .NET 10 dw.tools.math.analysis library and MSTest project with committed lockfiles and independent compiled absent-API RED, followed by 47 GREEN oracles and eight source-free NuGet probes. The bounded provider-free package admits finite strictly ascending 2-32 point binary64 piecewise linear and natural cubic interpolation and descriptive unweighted OLS. It qualifies y=2x+1, x=0.5 -> 2, zero SSE, curved natural spline 0.6875 and typed failure on duplicate/order/outside-domain/nonfinite/over-budget/rank-deficient data. T2 separately owns deeper extrapolation, unit, near-singular and precision conditions; M6 and W01 remain unfinished.";
        phase["readiness"]="T1 proved finite binary64 interpolation, natural spline and unweighted OLS with 47 GREEN, a genuine compiled RED, local versioned nupkg SHA and independent eight-probe NuGet consumer; T2 limits remain planned";
        phase["files"]=p.Json.StringArray(Solution,Product,ProductLock,Point,Source,
            Test,TestLock,RedTest,GreenTests,Consumer,ConsumerCode,ConsumerConfig,
            Decision,Recipe,RedProof,Qualified);
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C01T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release");
        phase["evidence"]=p.Json.StringArray(RedProof,Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });
    foreach(var node in new[]{Milestone,Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
    foreach(var node in new[]{Milestone,Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var node in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.ProjectPlan.RequireNodeState("M6-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.ProjectPlan.RequireNodeState("M7","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS081" ||
       (string?)state["activeMilestone"]!=(target?Milestone:null) ||
       (string?)state["activeWorkPackage"]!=(target?Work:null) ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null||state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS081 new M6 hierarchy cursor differs from baseline/target.");
    state["activeMilestone"]=Milestone;
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
    var prefix="M6-W01-C01-T1-PKG-SHA256:"+id+":"+version+":";
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
