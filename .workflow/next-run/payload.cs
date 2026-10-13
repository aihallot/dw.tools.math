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

const string Work="M3-W02", Phase="M3-W02-C02", Task="M3-W02-C02-T2";
const string Red="M3-W02-C02-T2-R", Green="M3-W02-C02-T2-G", Verify="M3-W02-C02-T2-V";
const string Correlation="src/projects/dw.tools.math.statistics/CorrelationStatistics64.cs";
const string Quantile="src/projects/dw.tools.math.statistics/QuantileStatistics64.cs";
const string Project="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C02T2RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C02T2Tests.cs";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string Document="docs/distribution/quantiles-and-correlations.md";
const string Decision="docs/planning/decisions/m3-w02-c02-statistics-policy.md";
const string Prior="docs/planning/evidence/M3-W02-C02-contract-qualified.json";
const string PriorRed="docs/planning/evidence/M3-W02-C02-contract-red.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C02-boundary-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C02-boundaries-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Version="0.4.0-preview.4";
const int NewGreenTests=31, CompleteStatisticsTests=103;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS047","M3",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS046",1,
    "feat: qualify conventioned quantiles and paired correlation",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("conventioned-quantiles-and-paired-finite-correlation-qualified"));
p.Require.JsonValueEquals(Prior,"/new_green_tests_passed",JsonValue.Create(26));
p.Require.JsonValueEquals(Prior,"/total_statistics_tests_passed",JsonValue.Create(71));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create("0.4.0-preview.3"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.45"),JsonValue.Create("0.1.46"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.46":"0.1.45"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/24/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/status",JsonValue.Create("planned"));
if(!target)
{
    foreach(var file in new[]{RedTest,GreenTest,RedEvidence,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("typed-correlation-and-missing-pair-public-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("typed-undefined-correlation-and-pair-missing-boundaries-qualified"));
    p.Require.JsonValueEquals(Qualified,"/new_green_tests_passed",
        JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/total_statistics_tests_passed",
        JsonValue.Create(CompleteStatisticsTests));
}

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C02-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C03","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

// Compile and observe a real RED against the RS046 preview.3 baseline.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C02T2RedTests.TypedUndefinedCorrelationAndMissingPairContractsMustExist",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C02-T2 RED: typed undefined correlation and explicit pair-missing contracts are absent.";
    if(observed.Code==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "RS047 typed-undefined/missing RED was not independently observed: "+Relevant(observed.Output));

    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C02-boundary-red",
        ["status"]="typed-correlation-and-missing-pair-public-contract-absent-red-observed",
        ["prior_T1_evidence"]=Prior,["prior_T1_RED"]=PriorRed,
        ["compiled_red_test"]=RedTest,["expected_marker"]=marker,
        ["previous_local_package"]="dw.tools.math.statistics/0.4.0-preview.3",
        ["independent_red"]=true
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Correlation,Quantile,Project,GreenTest,Document,Decision})
{
    var result=p.Files.ReplaceFromStaged("staged/"+path,path);
    digests[path]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing staged-file SHA-256: "+path);
}

if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");

    var green=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter","FullyQualifiedName~M3W02C02T2Tests",
        "--logger","console;verbosity=normal");
    if(!green.Contains("Total tests: 31",StringComparison.OrdinalIgnoreCase) ||
       !green.Contains("Passed: 31",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS047 all 31 independent boundary GREEN oracles are required.");

    var full=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=normal");
    if(!full.Contains("Total tests: 103",StringComparison.OrdinalIgnoreCase) ||
       !full.Contains("Passed: 103",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS047 requires all 71 prior and 32 new statistics tests.");

    Required(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","test",
        "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var feed=Path.Combine(root,"build","artifacts","m3-w02-c02-t2-local-feed");
    Required(root,"dotnet","pack",Project,"-c","Release","--no-build",
        "--no-restore","-o",feed);
    var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("RS047 local package is missing.");
    using(var zip=ZipFile.OpenRead(archive))
    {
        if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
            throw new InvalidOperationException("RS047 package lacks compiled statistics.");
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
            throw new InvalidOperationException("RS047 unexpected local package identity or dependency.");
    }

    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C02-boundaries-qualified",
        ["status"]="typed-undefined-correlation-and-pair-missing-boundaries-qualified",
        ["prior_T1_evidence"]=Prior,["prior_T1_RED"]=PriorRed,
        ["controlled_T2_RED"]=RedEvidence,
        ["typed_outcome_source"]=Correlation,
        ["typed_outcome_source_sha256"]=digests[Correlation],
        ["type7_boundary_source"]=Quantile,
        ["type7_boundary_source_sha256"]=digests[Quantile],
        ["new_green_test"]=GreenTest,
        ["new_green_test_sha256"]=digests[GreenTest],
        ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
        ["policy_decision"]=Decision,["decision_sha256"]=digests[Decision],
        ["distribution"]=Document,["distribution_sha256"]=digests[Document],
        ["project_file"]=Project,["project_file_sha256"]=digests[Project],
        ["new_green_tests_passed"]=NewGreenTests,
        ["total_statistics_tests_passed"]=CompleteStatisticsTests,
        ["prior_statistics_tests_passed"]=71,
        ["controlled_red_then_green"]=true,
        ["defined_correlations_have_nonnull_pearson"]=true,
        ["undefined_zero_variance_pearson"]=null,
        ["undefined_insufficient_pairs_pearson"]=null,
        ["null_missing_default"]="Reject; opt-in SkipPair removes complete aligned pair",
        ["nonfinite_values_always_rejected"]=true,
        ["empty_and_all_skipped_datasets_refuse"]=true,
        ["typed_zero_variance_population_and_sample_covariance"]=0,
        ["typed_singleton_population_covariance"]=0,
        ["typed_singleton_sample_covariance"]=null,
        ["provided_source_pair_budget"]=4096,
        ["type7_exact_knots_avoid_overflow_subtraction"]=true,
        ["prior_quantile_and_correlation_T1_tests_preserved"]=true,
        ["quantities_and_numerics_regressions_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["local_package_id"]="dw.tools.math.statistics",
        ["local_package_version"]=Version,
        ["local_package_bytes"]=new FileInfo(archive).Length,
        ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
        ["nonclaims"]=p.Json.StringArray(
            "No Decision/MCDM consumer implementation or independent interoperability test has been executed.",
            "No general nonfrequency weighting, exact rational bivariate moments, imputation or statistical inference is claimed.",
            "No universally robust Pearson rescaling at extreme binary64 magnitudes or full rounding error guarantee is claimed.",
            "No public feed publication, dw.discovery package/skill, broker adapter or Aura admission is claimed.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.46";
        var c=plan["chunks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Phase);
        var task=c["tasks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Task);
        c["status"]="done";
        c["refinement"]="RS046 T1 qualified separate Type-7 and nearest-rank sample quantiles over exact rational and binary64 inputs, plus finite paired covariance/Pearson. RS047 T2 qualifies machine-readable undefined Pearson statuses for truly constant/insufficient pairs; default Reject or explicit aligned SkipPair null policy, nonfinite refusal, count/budget contracts, exact-knots Type-7 extreme-boundary correction, 31 independent GREEN oracles and 103 total statistics tests with local preview.4 package inspection. Downstream Decision/MCDM adapters and neutral dw.discovery remain separately unqualified.";
        c["readiness"]="closed: T1 and T2 accepted and qualified; distributions/randomness remain M3-W02-C03";
        var files=c["files"]!.AsArray().Select(n=>n!.GetValue<string>());
        c["files"]=p.Json.StringArray(files.Concat(new[]{
            Correlation,Quantile,Project,RedTest,GreenTest,Document,Decision,
            RedEvidence,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        c["commands"]=p.Json.StringArray(
            "dotnet restore dw.tools.math.slnx --locked-mode",
            "dotnet build dw.tools.math.slnx -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C02T2Tests",
            "dotnet test "+TestProject+" -c Release",
            "dotnet pack "+Project+" -c Release");
        var receipts=c["evidence"]!.AsArray().Select(n=>n!.GetValue<string>());
        c["evidence"]=p.Json.StringArray(receipts.Concat(new[]{RedEvidence,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(n=>n!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedEvidence:Qualified);
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

// Exact baseline and target hierarchy are both accepted; the active phase
// cursor must be retired in the same controlled payload that closes C02.
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS047" ||
       (string?)state["activeMilestone"]!="M3" ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS047 baseline or completed hierarchy mismatch.");
    state["activePhase"]=null;
});

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M3-W02-C03","not-ready");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
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
