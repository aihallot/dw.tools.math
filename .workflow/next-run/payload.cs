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

const string Work="M3-W02", Phase="M3-W02-C03", Task="M3-W02-C03-T1";
const string Red="M3-W02-C03-T1-R", Green="M3-W02-C03-T1-G", Verify="M3-W02-C03-T1-V";
const string Prng="src/projects/dw.tools.math.statistics/SplitMix64Random.cs";
const string Bernoulli="src/projects/dw.tools.math.statistics/BernoulliDistribution64.cs";
const string Project="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C03T1RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C03T1Tests.cs";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string Document="docs/distribution/reproducible-distributions.md";
const string Decision="docs/planning/decisions/m3-w02-c03-distributions-policy.md";
const string Prior="docs/planning/evidence/M3-W02-C02-boundaries-qualified.json";
const string PriorRed="docs/planning/evidence/M3-W02-C02-boundary-red.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C03-contract-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C03-contract-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Version="0.4.0-preview.5";
const int NewGreenTests=31, CompleteStatisticsTests=135;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS048","M3",Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS047",1,
    "feat: qualify typed correlation boundaries and close quantiles phase",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("typed-undefined-correlation-and-pair-missing-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/new_green_tests_passed",JsonValue.Create(31));
p.Require.JsonValueEquals(Prior,"/total_statistics_tests_passed",JsonValue.Create(103));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create("0.4.0-preview.4"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.46"),JsonValue.Create("0.1.47"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.47":"0.1.46"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/tasks/1/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/25/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/status",JsonValue.Create("planned"));

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01","done");
p.ProjectPlan.RequireNodeState("M3-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C03-T2","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    p.Require.FileMissing(RedEvidence);
    p.Require.FileMissing(Qualified);
    foreach(var file in new[]{Prng,Bernoulli,RedTest,GreenTest,Document,Decision})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("versioned-prng-and-bernoulli-public-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("versioned-splitmix64-and-bernoulli-distribution-qualified"));
    p.Require.JsonValueEquals(Qualified,"/new_green_tests_passed",
        JsonValue.Create(NewGreenTests));
    p.Require.JsonValueEquals(Qualified,"/total_statistics_tests_passed",
        JsonValue.Create(CompleteStatisticsTests));
}

// The compiled reflection RED must run on the already-qualified preview.4
// before installing the proposed versioned generator or Bernoulli API.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C03T1RedTests.VersionedDeterministicRandomAndBernoulliContractsMustExist",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C03-T1 RED: versioned PRNG and Bernoulli distribution contracts are absent.";
    if(observed.Code==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "RS048 PRNG/Bernoulli RED not independently observed: "+Relevant(observed.Output));
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C03-contract-red",
        ["status"]="versioned-prng-and-bernoulli-public-contract-absent-red-observed",
        ["prior_C02_T2_evidence"]=Prior,["prior_C02_T2_RED"]=PriorRed,
        ["compiled_red_test"]=RedTest,
        ["expected_marker"]=marker,
        ["qualified_baseline_package"]="dw.tools.math.statistics/0.4.0-preview.4",
        ["controlled_red_before_product_source_installation"]=true
    };
    p.Files.WriteComplete(RedEvidence,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
    p.Require.JsonValueEquals(RedEvidence,"/controlled_red_before_product_source_installation",
        JsonValue.Create(true));

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Prng,Bernoulli,Project,GreenTest,Document,Decision})
{
    var replaced=p.Files.ReplaceFromStaged("staged/"+path,path);
    digests[path]=replaced.AfterSha256 ??
        throw new InvalidOperationException("Missing SDK staged-file SHA-256 for "+path);
}
if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");

    var focused=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C03T1Tests","--logger","console;verbosity=normal");
    if(!focused.Contains("Total tests: 31",StringComparison.OrdinalIgnoreCase) ||
       !focused.Contains("Passed: 31",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS048 requires all 31 independent reproducibility and Bernoulli GREEN cases.");

    // The C03 change is isolated to statistics. The complete statistics
    // project includes all 103 qualified prior tests; no broad solution suite.
    var regression=Required(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=normal");
    if(!regression.Contains("Total tests: 135",StringComparison.OrdinalIgnoreCase) ||
       !regression.Contains("Passed: 135",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS048 requires 103 old and 32 new statistics tests.");
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var feed=Path.Combine(root,"build","artifacts","m3-w02-c03-t1-local-feed");
    Required(root,"dotnet","pack",Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("RS048 local package missing.");
    using(var zip=ZipFile.OpenRead(archive))
    {
        if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
            throw new InvalidOperationException("RS048 missing package statistics assembly.");
        var nuspec=zip.Entries.Single(e=>e.FullName.EndsWith(".nuspec",
            StringComparison.OrdinalIgnoreCase));
        using var stream=nuspec.Open();
        var xml=XDocument.Load(stream);
        var metadata=xml.Descendants().Single(e=>e.Name.LocalName=="metadata");
        var id=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
        var version=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
        var deps=metadata.Descendants().Where(e=>e.Name.LocalName=="dependency")
            .Select(e=>(string?)e.Attribute("id")??"").ToArray();
        if(id!="dw.tools.math.statistics" || version!=Version ||
           !deps.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("RS048 unexpected package identity/provider dependency.");
    }

    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C03-contract-qualified",
        ["status"]="versioned-splitmix64-and-bernoulli-distribution-qualified",
        ["prior_C02_T2_evidence"]=Prior,["prior_C02_T2_RED"]=PriorRed,
        ["controlled_T1_RED"]=RedEvidence,
        ["versioned_prng_source"]=Prng,["prng_source_sha256"]=digests[Prng],
        ["bernoulli_source"]=Bernoulli,["bernoulli_source_sha256"]=digests[Bernoulli],
        ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
        ["green_test"]=GreenTest,["green_test_sha256"]=digests[GreenTest],
        ["distribution_document"]=Document,["distribution_sha256"]=digests[Document],
        ["policy_decision"]=Decision,["policy_sha256"]=digests[Decision],
        ["package_project"]=Project,["project_sha256"]=digests[Project],
        ["new_green_tests_passed"]=NewGreenTests,
        ["total_statistics_tests_passed"]=CompleteStatisticsTests,
        ["prior_statistics_tests_passed"]=103,
        ["controlled_red_then_green"]=true,
        ["prng_algorithm_id"]="splitmix64/v1",
        ["prng_seed_bits"]=64,["counter_bits"]=64,
        ["counter_semantics"]="Unsigned number of raw 64-bit draws consumed; exhausted counter refuses.",
        ["uniform_conversion"]="Top 53 bits of a SplitMix64 word multiplied by 2^-53",
        ["uniform_support"]="[0,1)",
        ["golden_seeds"]=p.Json.StringArray("0","1","42"),
        ["golden_seed_zero_first"]="e220a8397b1dcdaf",
        ["golden_seed_one_first"]="910a2dec89025cc1",
        ["golden_seed_fortytwo_first"]="bdd732262feb6e95",
        ["bernoulli_parameter"]="finite double in [0,1]",
        ["bernoulli_pmf"]="P(0)=1-p; P(1)=p; zero integer mass elsewhere",
        ["bernoulli_cdf"]="0 below 0; 1-p at 0; 1 at or above 1",
        ["bernoulli_quantile"]="Smallest positive-mass support reaching q; p=1 selects 1 even at q=0",
        ["sample_draws_per_call"]=1,
        ["degenerate_bernoulli_samples_are_exact"]=true,
        ["noncryptographic"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["local_package_id"]="dw.tools.math.statistics",
        ["local_package_version"]=Version,
        ["local_package_bytes"]=new FileInfo(archive).Length,
        ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
        ["nonclaims"]=p.Json.StringArray(
            "No cryptographic generator, non-pseudorandom entropy source or security claim.",
            "No continuous normal-distribution quantile/cdf, unbounded sampling, or universal statistical-quality claim.",
            "No unobserved cross-RID build or downstream Decision/MCDM integration verification.",
            "No neutral dw.discovery manifest, AURA broker skill or public NuGet feed publishing.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.47";
        var c=plan["chunks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Phase);
        var t=c["tasks"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Task);
        c["status"]="in_progress";
        c["refinement"]="RS048 T1 qualifies versioned seeded SplitMix64/v1 with unsigned counter, restorable draw position and fixed multi-seed golden vectors; finite binary64 uniform [0,1) high-53-bit conversion; and Bernoulli PMF/CDF/inverse-support-quantile/sampling with exact p=0/1 outcomes. A compiled independent RED, 31 GREEN and 135 total statistics tests plus local preview.5 verify the bounded, explicitly noncryptographic baseline. C03-T2 retains remaining distribution boundaries and integration policy.";
        c["readiness"]="in progress: C03 T1 qualified on durable RS048 success; T2 remains planned";
        c["files"]=p.Json.StringArray(Prng,Bernoulli,Project,RedTest,GreenTest,
            Document,Decision,RedEvidence,Qualified);
        c["commands"]=p.Json.StringArray(
            "dotnet restore dw.tools.math.slnx --locked-mode",
            "dotnet build dw.tools.math.slnx -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C03T1Tests",
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
    p.Json.EditObject(".aura/workflow/state.json",state=>
    {
        if((string?)state["pendingRun"]!="RS048" ||
           (string?)state["activeMilestone"]!="M3" ||
           (string?)state["activeWorkPackage"]!=Work ||
           state["activePhase"] is not null ||
           state["activeTask"] is not null ||
           state["activeSubtask"] is not null)
            throw new InvalidOperationException("RS048 expected exact baseline execution hierarchy.");
        state["activePhase"]=Phase;
    });
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
}
else
{
    foreach(var node in new[]{Task,Red,Green,Verify})
        p.ProjectPlan.RequireNodeState(node,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C03-T2","not-ready");
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
