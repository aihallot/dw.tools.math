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

const string Work="M3-W02", Phase="M3-W02-C03", Task="M3-W02-C03-T2";
const string Red="M3-W02-C03-T2-R", Green="M3-W02-C03-T2-G", Verify="M3-W02-C03-T2-V";
const string Uniform="src/projects/dw.tools.math.statistics/ContinuousUniformDistribution64.cs";
const string Codec="src/projects/dw.tools.math.statistics/SplitMix64PositionCodec.cs";
const string Prng="src/projects/dw.tools.math.statistics/SplitMix64Random.cs";
const string Bernoulli="src/projects/dw.tools.math.statistics/BernoulliDistribution64.cs";
const string Project="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C03T2RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C03T2Tests.cs";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string Document="docs/distribution/reproducible-distributions.md";
const string Decision="docs/planning/decisions/m3-w02-c03-distributions-policy.md";
const string Prior="docs/planning/evidence/M3-W02-C03-contract-qualified.json";
const string PriorRed="docs/planning/evidence/M3-W02-C03-contract-red.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C03-boundary-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C03-boundaries-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Version="0.4.0-preview.6";
const int NewGreenTests=30, CompleteStatisticsTests=166;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

p.Workflow.RequireCurrentRun("RS049","M3",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS048",1,
    "feat: qualify reproducible random sampling and Bernoulli distributions",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("versioned-splitmix64-and-bernoulli-distribution-qualified"));
p.Require.JsonValueEquals(Prior,"/new_green_tests_passed",JsonValue.Create(31));
p.Require.JsonValueEquals(Prior,"/total_statistics_tests_passed",JsonValue.Create(135));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create("0.4.0-preview.5"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.47"),JsonValue.Create("0.1.48"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.48":"0.1.47"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/25/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/status",JsonValue.Create("planned"));
if(!target)
{
    foreach(var file in new[]{Uniform,Codec,RedTest,GreenTest,RedEvidence,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("continuous-uniform-and-canonical-checkpoint-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("finite-continuous-distributions-and-portable-prng-checkpoints-qualified"));
    p.Require.JsonValueEquals(Qualified,"/new_green_tests_passed",
        JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/total_statistics_tests_passed",
        JsonValue.Create(CompleteStatisticsTests));
}

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01","done");
p.ProjectPlan.RequireNodeState("M3-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C04","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

// Real independently compiled public-contract RED against the previously
// qualified preview.5 before either new product type is installed.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C03T2RedTests.ContinuousApproximationAndPortableCheckpointRequirePublicContracts",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C03-T2 RED: continuous uniform and portable PRNG checkpoint public contracts are absent.";
    if(observed.Code==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "RS049 public-contract RED not independently observed: "+Relevant(observed.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C03-boundary-red",
        ["status"]="continuous-uniform-and-canonical-checkpoint-contract-absent-red-observed",
        ["prior_T1_evidence"]=Prior,["prior_T1_RED"]=PriorRed,
        ["compiled_red_test"]=RedTest,["expected_marker"]=marker,
        ["qualified_baseline_package"]="dw.tools.math.statistics/0.4.0-preview.5",
        ["red_observed_before_new_product_sources"]=true
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/red_observed_before_new_product_sources",
        JsonValue.Create(true));
}

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Uniform,Codec,Project,GreenTest,Document,Decision})
{
    var result=p.Files.ReplaceFromStaged("staged/"+file,file);
    digests[file]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing SDK staged SHA-256: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");

    var focused=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C03T2Tests","--logger","console;verbosity=normal");
    if(!focused.Contains("Total tests: 30",StringComparison.OrdinalIgnoreCase) ||
       !focused.Contains("Passed: 30",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS049 requires all 30 independent T2 GREEN tests.");

    var statistics=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=normal");
    if(!statistics.Contains("Total tests: 166",StringComparison.OrdinalIgnoreCase) ||
       !statistics.Contains("Passed: 166",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS049 requires 135 previously qualified and 31 new statistics tests.");

    Required(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","test",
        "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var feed=Path.Combine(root,"build","artifacts","m3-w02-c03-t2-local-feed");
    Required(root,"dotnet","pack",Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("RS049 local package is missing.");
    using(var zip=ZipFile.OpenRead(archive))
    {
        if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
            throw new InvalidOperationException("RS049 package lacks compiled statistics DLL.");
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
            throw new InvalidOperationException("RS049 wrong package identity or runtime dependencies.");
    }

    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C03-boundaries-qualified",
        ["status"]="finite-continuous-distributions-and-portable-prng-checkpoints-qualified",
        ["prior_T1_evidence"]=Prior,["prior_T1_RED"]=PriorRed,
        ["controlled_T2_RED"]=RedEvidence,
        ["continuous_uniform_source"]=Uniform,
        ["continuous_uniform_sha256"]=digests[Uniform],
        ["portable_checkpoint_source"]=Codec,
        ["portable_checkpoint_sha256"]=digests[Codec],
        ["prng_algorithm_id"]="splitmix64/v1",
        ["prng_source"]=Prng,["bernoulli_source"]=Bernoulli,
        ["green_test"]=GreenTest,["green_test_sha256"]=digests[GreenTest],
        ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
        ["policy_document"]=Decision,["policy_sha256"]=digests[Decision],
        ["distribution_document"]=Document,["distribution_sha256"]=digests[Document],
        ["package_project"]=Project,["package_project_sha256"]=digests[Project],
        ["new_green_tests_passed"]=NewGreenTests,
        ["total_statistics_tests_passed"]=CompleteStatisticsTests,
        ["prior_statistics_tests_passed"]=135,
        ["controlled_red_then_green"]=true,
        ["uniform_bounds"]="finite a<b with finite width and reciprocal density",
        ["uniform_pdf"]="1/(b-a) on [a,b), zero elsewhere including upper endpoint",
        ["uniform_cdf"]="0 for x<=a; approximate (x-a)/(b-a) inside; 1 for x>=b",
        ["uniform_quantile"]="approximate a+q*(b-a), q=0 is a and q=1 is b",
        ["uniform_sample_support"]="finite IEEE binary64 [a,b); round-up clamps to BitDecrement(b)",
        ["prng_checkpoint_format"]="splitmix64/v1|lowercase16hex-seed|lowercase16hex-consumed-counter",
        ["prng_checkpoint_is_canonical"]=true,
        ["bernoulli_zero_and_one_quantile_conventions_preserved"]=true,
        ["sampling_consumes_one_raw_word"]=true,
        ["prng_is_noncryptographic"]=true,
        ["relevant_quantities_and_numerics_regressions_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["local_package_id"]="dw.tools.math.statistics",
        ["local_package_version"]=Version,
        ["local_package_bytes"]=new FileInfo(archive).Length,
        ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
        ["nonclaims"]=p.Json.StringArray(
            "No normal/Gaussian distribution, independent tails, cryptographic randomness or statistically exact continuous sampling.",
            "No Math.Net runtime provider dependency, unobserved target RID, public NuGet feed or full universal rounding error guarantee.",
            "No implemented or independently qualified Decision/MCDM adapter, neutral dw.discovery metadata, AURA broker skill or agent admission.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.48";
        var c=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Phase);
        var t=c["tasks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Task);
        c["status"]="done";
        c["refinement"]="RS048 qualifies fixed SplitMix64/v1 multi-seed vectors, consumed-counter checkpoints, bounded high-53-bit [0,1) draws, Bernoulli PMF/CDF/positive-mass quantile and deterministic sampling. RS049 T2 adds explicit finite continuous-uniform PDF/CDF/quantile/sampling with half-open binary64 samples, invalid-width/density/NaN refusals, and a strict invariant ASCII seed/counter codec. Compiled RED, 30 independent GREEN and 166 total statistics tests, quantities/numerics regressions, local preview.6 and scientific limitations close C03 without cryptographic or normal-distribution claims.";
        c["readiness"]="closed: both T1 and T2 qualified on durable RS048/RS049 evidence; C04 Decision/MCDM consumer contracts still planned";
        var oldFiles=c["files"]!.AsArray().Select(v=>v!.GetValue<string>());
        c["files"]=p.Json.StringArray(oldFiles.Concat(new[]{
            Uniform,Codec,Project,RedTest,GreenTest,Document,Decision,RedEvidence,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        c["commands"]=p.Json.StringArray(
            "dotnet restore dw.tools.math.slnx --locked-mode",
            "dotnet build dw.tools.math.slnx -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C03T2Tests",
            "dotnet test "+TestProject+" -c Release",
            "dotnet pack "+Project+" -c Release");
        var oldEvidence=c["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
        c["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{
            RedEvidence,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        t["status"]="done";
        t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
        foreach(var child in t["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
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

// Retire active C03 cursor in the same controlled transaction on success;
// re-entry accepts only the exact initial and exact completed hierarchy.
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS049" ||
       (string?)state["activeMilestone"]!="M3" ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS049 expected exact baseline or target DWF cursor.");
    state["activePhase"]=null;
});
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M3-W02-C04","not-ready");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

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
