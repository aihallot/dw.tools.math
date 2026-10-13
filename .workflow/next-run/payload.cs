using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M3-W02", Phase="M3-W02-C01", Task="M3-W02-C01-T1";
const string Red="M3-W02-C01-T1-R", Green="M3-W02-C01-T1-G", Verify="M3-W02-C01-T1-V";
const string Slnx="dw.tools.math.slnx";
const string ProductProject="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string ProductLock="src/projects/dw.tools.math.statistics/packages.lock.json";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.statistics.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C01RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C01Tests.cs";
const string Source="src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs";
const string SmokeProject="tests/providers/m3-w02-c01/ManagedStatisticsSmoke.csproj";
const string SmokeSource="tests/providers/m3-w02-c01/Program.cs";
const string Document="docs/distribution/descriptive-statistics.md";
const string Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M3-W01-C03-boundaries-qualified.json";
const string Provider="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C01-contract-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C01-contract-qualified.json";
const string Version="0.4.0-preview.1";
const int AcceptedTests=19;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
var baseline=!target;
p.Workflow.RequireCurrentRun("RS044","M3",target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS043",1,
    "feat: qualify 2x2 conditioning boundaries and close numerical matrix scope",
    "0.1.59",2,2,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-2x2-condition-availability-rank-pivot-policy-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(16));
p.Require.JsonValueEquals(Prior,"/numerical_regressions_passed",JsonValue.Create(81));
p.Require.JsonValueEquals(Provider,"/selected_package_id",JsonValue.Create("MathNet.Numerics"));
p.Require.JsonValueEquals(Provider,"/selected_package_version",JsonValue.Create("5.0.0"));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.42"),JsonValue.Create("0.1.43"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.43":"0.1.42"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/7/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/22/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/tasks/1/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/23/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",JsonValue.Create("planned"));

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C02","not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");

if(target)
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("bounded-exact-and-compensated-descriptive-statistics-qualified"));
else
{
    p.Require.FileMissing(Qualified);
    p.Require.FileMissing(RedEvidence);
    p.Require.FileMissing(Source);
    p.Require.FileMissing(ProductProject);
    p.Require.FileMissing(TestProject);
}

// Install the reproducible .NET 10 project/test, lock files and solution
// before the RED. Product implementation and GREEN remain absent.
var projectFiles=new[]{Slnx,ProductProject,ProductLock,TestProject,TestLock};
var stagedSha=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in projectFiles)
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedSha[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged dependency identity: "+file);
}
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);

if(baseline)
{
    Required(root,"dotnet","restore",Slnx,"--locked-mode");
    Required(root,"dotnet","build",Slnx,"-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release","--no-build",
        "--no-restore","--filter",
        "FullyQualifiedName~M3W02C01RedTests.ProviderNeutralStatisticsMustExposeAnExplicitFiniteAggregateContract",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C01-T1 RED: exact and finite descriptive statistics contract is missing.";
    if(observed.Code==0||!observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent missing-contract statistics RED absent: "+
            Relevant(observed.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C01-contract-red",
        ["status"]="controlled-provider-neutral-statistics-contract-absent-red-observed",
        ["prior_M3_W01_evidence"]=Prior,
        ["test"]=RedTest,["expected_marker"]=marker,
        ["scope"]="Independent reflection test before the first descriptive implementation file is installed."
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("controlled-provider-neutral-statistics-contract-absent-red-observed"));

// The new package is not a provider facade; Math.NET is test-only.
foreach(var file in new[]{Source,GreenTest,SmokeProject,SmokeSource,Document})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedSha[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing installed source identity: "+file);
}

Required(root,"dotnet","restore",Slnx,"--locked-mode");
Required(root,"dotnet","build",Slnx,"-c","Release","--no-restore");
var focused=Required(root,"dotnet","test",TestProject,"-c","Release","--no-build",
    "--no-restore","--filter","FullyQualifiedName~M3W02C01Tests",
    "--logger","console;verbosity=normal");
if(!focused.Contains("Total tests: 19",StringComparison.OrdinalIgnoreCase)
    ||!focused.Contains("Passed: 19",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS044 19 exact/approximate GREEN oracles were not observed.");
var stats=Required(root,"dotnet","test",TestProject,"-c","Release","--no-build",
    "--no-restore","--logger","console;verbosity=normal");
if(!stats.Contains("Total tests: 20",StringComparison.OrdinalIgnoreCase)
    ||!stats.Contains("Passed: 20",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("The controlled RED did not become GREEN or sample suite is incomplete.");
var independent=Required(root,"dotnet","run","--project",SmokeProject,"-c","Release");
if(!independent.Contains("M3-W02-C01:managed-mathnet-5.0.0-descriptive-oracles:qualified",
    StringComparison.Ordinal))
    throw new InvalidOperationException("Managed provider mean/variance corroboration missing.");

// Rational semantics and prior numerical contracts are actual consumers.
Required(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","test",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m3-w02-c01-t1-local-feed");
Required(root,"dotnet","pack",ProductProject,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
if(!File.Exists(archive))throw new InvalidOperationException("Stats package missing.");
using(var zip=ZipFile.OpenRead(archive))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
        throw new InvalidOperationException("Stats package lacks its compiled assembly.");
    var nuspec=zip.Entries.Single(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase));
    using var stream=nuspec.Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.statistics"||version!=Version||
        !deps.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Stats package identity or direct dependency changed.");
}

var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W02-C01-contract-qualified",
    ["status"]="bounded-exact-and-compensated-descriptive-statistics-qualified",
    ["prior_numerical_work_package_evidence"]=Prior,
    ["provider_baseline_evidence"]=Provider,
    ["controlled_red_evidence"]=RedEvidence,
    ["statistics_implementation"]=Source,["statistics_source_sha256"]=stagedSha[Source],
    ["test_source"]=GreenTest,["test_source_sha256"]=stagedSha[GreenTest],
    ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
    ["solution_file"]=Slnx,["solution_file_sha256"]=stagedSha[Slnx],
    ["locked_restore_passed"]=true,
    ["statistics_library_lock_sha256"]=stagedSha[ProductLock],
    ["statistics_test_lock_sha256"]=stagedSha[TestLock],
    ["independent_green_tests_passed"]=AcceptedTests,
    ["complete_stats_tests_passed"]=20,
    ["controlled_red_then_green"]=true,
    ["hand_oracles"]="[1,2,3,4]: mean 5/2, population variance 5/4, sample variance 5/3",
    ["exact_rational_oracle"]="1/3+1/6=1/2, mean=1/4",
    ["finite_sum_algorithm"]="Neumaier compensated binary64, finite output required",
    ["variance_algorithm"]="two-pass sample-centered binary64",
    ["maximum_observations"]=4096,
    ["exact_operand_and_intermediate_bit_budget"]=4096,
    ["singleton_sample_variance_unavailable"]=true,
    ["nonfinite_and_unrepresentable_values_refused"]=true,
    ["independent_mathnet_5_0_0_descriptive_smoke_passed"]=true,
    ["rational_and_numerical_prior_regressions_passed"]=true,
    ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
    ["runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["local_package_id"]="dw.tools.math.statistics",
    ["local_package_version"]=Version,
    ["local_package_bytes"]=new FileInfo(archive).Length,
    ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
    ["no_provider_assembly_reference"]=true,
    ["nonclaims"]=p.Json.StringArray(
        "No public NuGet feed publication, provider runtime dependency, native BLAS, or unobserved RID.",
        "No exactness or universal stability guarantee for finite floating-point calculations.",
        "Missing/weighted observations and larger offset/overflow policies remain M3-W02-C01-T2 work.",
        "No quantiles, correlations, probability distributions, or Decision VaR/CVaR semantics.",
        "No dw.discovery contract/skill generator, BrokerReady or Aura action.")
};
p.Files.WriteComplete(Qualified,proof.ToJsonString(
    new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject(Backlog,plan=>
{
    plan["plan_version"]="0.1.43";
    var wp=plan["work_packages"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Work);
    var chunk=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Phase);
    var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Task);
    wp["status"]="in_progress";
    chunk["status"]="in_progress";
    chunk["refinement"]="RS044 opens the standalone provider-neutral dw.tools.math.statistics .NET 10 product with locked restoration and independent package tests. T1 admits bounded Neumaier compensated finite sums, mean/min/max, centered population/sample variance, and exact rational sum/mean/extrema. A controlled absent-contract RED, 19 focused GREEN tests, exact hand oracles, managed Math.NET corroboration and a local package establish the initial contract. Empty/singleton, missing, weighted, scale/overflow policy completion remains T2.";
    chunk["readiness"]="T1 qualified; C01-T2 remains planned for missing-data, weighting and extended numerical boundary policies before C01 closure";
    var files=chunk["files"]!.AsArray().Select(v=>v!.GetValue<string>());
    chunk["files"]=p.Json.StringArray(files.Concat(new[]{Slnx,ProductProject,ProductLock,
        TestProject,TestLock,RedTest,GreenTest,Source,SmokeProject,SmokeSource,
        Document,RedEvidence,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
    chunk["commands"]=p.Json.StringArray(
        "dotnet restore "+Slnx+" --locked-mode",
        "dotnet build "+Slnx+" -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C01Tests",
        "dotnet run --project "+SmokeProject+" -c Release",
        "dotnet pack "+ProductProject+" -c Release");
    chunk["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    task["status"]="done";
    task["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    foreach(var child in task["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
    {
        child["status"]="done";
        child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedEvidence:Qualified);
    }
});
if(baseline)
{
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
else
{
    foreach(var id in new[]{Task,Red,Green,Verify})
        p.ProjectPlan.RequireNodeState(id,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C02","not-ready");
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS044"
       ||(string?)state["activeMilestone"]!="M3"
       ||(string?)state["activeWorkPackage"]!=(target?Work:null)
       ||(string?)state["activePhase"]!=(target?Phase:null)
       ||state["activeTask"] is not null
       ||state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS044 execution cursor is not the declared baseline or target.");
    state["activeWorkPackage"]=Work;
    state["activePhase"]=Phase;
});

Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var s=i<0?output:output[i..];
    return s.Length>2400?s[..2400]:s;
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
