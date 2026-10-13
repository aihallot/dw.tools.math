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

const string Work="M3-W02", Phase="M3-W02-C02", Task="M3-W02-C02-T1";
const string Red="M3-W02-C02-T1-R", Green="M3-W02-C02-T1-G", Verify="M3-W02-C02-T1-V";
const string Quantile="src/projects/dw.tools.math.statistics/QuantileStatistics64.cs";
const string Correlation="src/projects/dw.tools.math.statistics/CorrelationStatistics64.cs";
const string Project="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C02T1RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C02T1Tests.cs";
const string Document="docs/distribution/quantiles-and-correlations.md";
const string Decision="docs/planning/decisions/m3-w02-c02-statistics-policy.md";
const string Prior="docs/planning/evidence/M3-W02-C01-boundaries-qualified.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C02-contract-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C02-contract-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Version="0.4.0-preview.3";
const int NewGreenTests=26, CompleteStatisticsTests=71;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS046","M3",Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS045",1,
    "feat: qualify frequency weighted statistics and close descriptive boundaries",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("frequency-weight-and-explicit-null-missing-statistics-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(23));
p.Require.JsonValueEquals(Prior,"/descriptive_suite_total_passed",JsonValue.Create(44));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create("0.4.0-preview.2"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.44"),JsonValue.Create("0.1.45"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.45":"0.1.44"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/tasks/1/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/24/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C02-T2","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    p.Require.FileMissing(RedEvidence);
    p.Require.FileMissing(Qualified);
    foreach(var path in new[]{Quantile,Correlation,RedTest,GreenTest,Document,Decision})
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("controlled-quantile-and-correlation-public-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("conventioned-quantiles-and-paired-finite-correlation-qualified"));
    p.Require.JsonValueEquals(Qualified,"/new_green_tests_passed",JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/total_statistics_tests_passed",
        JsonValue.Create(CompleteStatisticsTests));
}

// RED is compiled and executed against the qualified preview.2 assembly,
// before either proposed public contract is installed.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C02T1RedTests.ConventionedQuantilesAndPairedCorrelationRequirePublicContracts",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C02-T1 RED: conventioned quantile and paired correlation contracts are absent.";
    if(observed.Code==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS046 public-contract RED was not independently observed: "+
            Relevant(observed.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C02-contract-red",
        ["status"]="controlled-quantile-and-correlation-public-contract-absent-red-observed",
        ["baseline_package_version"]="0.4.0-preview.2",
        ["prior_statistics_evidence"]=Prior,
        ["compiled_red_test"]=RedTest,
        ["expected_marker"]=marker,
        ["interpretation"]="Both new product contracts were absent from the previously qualified assembly at compilation and RED execution."
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

// Complete, staged source files converge idempotently on either baseline or target.
var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Quantile,Correlation,Project,GreenTest,Document,Decision})
{
    var result=p.Files.ReplaceFromStaged("staged/"+path,path);
    digests[path]=result.AfterSha256 ??
        throw new InvalidOperationException("SDK did not return a staged-file SHA-256 for "+path);
}

if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var green=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter","FullyQualifiedName~M3W02C02T1Tests",
        "--logger","console;verbosity=normal");
    if(!green.Contains("Total tests: 26",StringComparison.OrdinalIgnoreCase) ||
       !green.Contains("Passed: 26",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS046 requires every one of its 26 independent GREEN oracles.");

    var all=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=normal");
    if(!all.Contains("Total tests: 71",StringComparison.OrdinalIgnoreCase) ||
       !all.Contains("Passed: 71",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS046 requires all 44 existing and 27 new statistics tests.");

    Required(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","test",
        "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var feed=Path.Combine(root,"build","artifacts","m3-w02-c02-t1-local-feed");
    Required(root,"dotnet","pack",Project,"-c","Release","--no-build",
        "--no-restore","-o",feed);
    var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("RS046 local package was not created.");
    using(var zip=ZipFile.OpenRead(archive))
    {
        if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
            throw new InvalidOperationException("RS046 package lacks the compiled product DLL.");
        var nuspec=zip.Entries.Single(x=>x.FullName.EndsWith(".nuspec",
            StringComparison.OrdinalIgnoreCase));
        using var stream=nuspec.Open();
        var xml=XDocument.Load(stream);
        var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
        var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
        var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
        var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
            .Select(x=>(string?)x.Attribute("id")??"").ToArray();
        if(id!="dw.tools.math.statistics" || version!=Version ||
            !deps.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Unexpected RS046 package identity or dependencies.");
    }

    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C02-contract-qualified",
        ["status"]="conventioned-quantiles-and-paired-finite-correlation-qualified",
        ["prior_descriptive_statistics"]=Prior,
        ["controlled_T1_RED"]=RedEvidence,
        ["new_quantile_source"]=Quantile,["quantile_source_sha256"]=digests[Quantile],
        ["new_paired_source"]=Correlation,["correlation_source_sha256"]=digests[Correlation],
        ["green_test"]=GreenTest,["green_test_sha256"]=digests[GreenTest],
        ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
        ["policy_decision"]=Decision,["policy_decision_sha256"]=digests[Decision],
        ["distribution"]=Document,["distribution_sha256"]=digests[Document],
        ["project_file"]=Project,["project_file_sha256"]=digests[Project],
        ["new_green_tests_passed"]=NewGreenTests,
        ["total_statistics_tests_passed"]=CompleteStatisticsTests,
        ["prior_statistics_tests_passed"]=44,
        ["controlled_red_then_green"]=true,
        ["quantile_conventions"]=p.Json.StringArray("Type-7 interpolated (n-1)*p",
            "Nearest-rank ceiling(n*p), first datum at p=0"),
        ["exact_rational_quantiles_without_widening"]=true,
        ["paired_covariance_population_denominator"]="N",
        ["paired_covariance_sample_denominator"]="N-1",
        ["paired_pearson_oracle"]="x=[1,2,3],y=[2,4,6]: population covariance=4/3, sample covariance=2, Pearson=1",
        ["finite_observation_budget"]=4096,
        ["exact_operand_bits_budget"]=4096,
        ["correlation_minimum_pair_count"]=2,
        ["regression_quantities_and_numerics_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["local_package_id"]="dw.tools.math.statistics",
        ["local_package_version"]=Version,
        ["local_package_bytes"]=new FileInfo(archive).Length,
        ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
        ["nonclaims"]=p.Json.StringArray(
            "T2 typed undefined correlation, complete singleton, missing/null and degenerate policies are not qualified.",
            "Exact rational bivariate moments, weighted covariance, unbounded inputs and universal binary64 error bounds are not qualified.",
            "No public NuGet push, Decision/MCDM implementation, neutral dw.discovery dependency or AURA broker capability is delivered.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.45";
        var c=plan["chunks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Phase);
        var t=c["tasks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Task);
        c["status"]="in_progress";
        c["refinement"]="RS046 qualifies conventioned Type-7 and nearest-rank quantiles for exact rational and finite binary64 samples, together with bounded centered binary64 covariance and Pearson correlation. Compiled independent RED, 26 GREEN tests, 71 total statistics tests, rational/numeric regressions and local preview.3 manifest qualify only T1. T2 retains typed undefined constant-vector outcomes, missing/degenerate and Decision consumer conventions.";
        c["readiness"]="in progress: C02 T1 admitted on RS046 durable success; T2 planned and not qualified";
        c["files"]=p.Json.StringArray(
            Quantile,Correlation,Project,RedTest,GreenTest,Document,Decision,RedEvidence,Qualified);
        c["commands"]=p.Json.StringArray(
            "dotnet restore dw.tools.math.slnx --locked-mode",
            "dotnet build dw.tools.math.slnx -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C02T1Tests",
            "dotnet test "+TestProject+" -c Release",
            "dotnet pack "+Project+" -c Release");
        c["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
        t["status"]="done";
        t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
        foreach(var child in t["subtasks"]!.AsArray().Select(n=>n!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedEvidence:Qualified);
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

    // Only the C02 phase becomes active; T2 remains not-ready.
    p.Json.EditObject(".aura/workflow/state.json",state=>
    {
        if((string?)state["pendingRun"]!="RS046" ||
           (string?)state["activeMilestone"]!="M3" ||
           (string?)state["activeWorkPackage"]!=Work ||
           state["activePhase"] is not null ||
           state["activeTask"] is not null ||
           state["activeSubtask"] is not null)
            throw new InvalidOperationException("RS046 expected exact initial execution hierarchy.");
        state["activePhase"]=Phase;
    });
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
}
else
{
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
    p.ProjectPlan.RequireNodeState(Task,"done");
    foreach(var child in new[]{Red,Green,Verify})
        p.ProjectPlan.RequireNodeState(child,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C02-T2","not-ready");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
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
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
