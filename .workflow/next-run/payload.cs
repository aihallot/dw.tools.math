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

const string Work="M3-W02", Phase="M3-W02-C01", Task="M3-W02-C01-T2";
const string Red="M3-W02-C01-T2-R", Green="M3-W02-C01-T2-G", Verify="M3-W02-C01-T2-V";
const string Source="src/projects/dw.tools.math.statistics/FrequencyWeightedStatistics64.cs";
const string Shared="src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs";
const string Project="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string RedTest="tests/projects/dw.tools.math.statistics.tests/M3W02C01T2RedTests.cs";
const string GreenTest="tests/projects/dw.tools.math.statistics.tests/M3W02C01T2Tests.cs";
const string TestProject="tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj";
const string Document="docs/distribution/descriptive-statistics.md";
const string Decision="docs/planning/decisions/m3-w02-c01-statistics-policy.md";
const string Prior="docs/planning/evidence/M3-W02-C01-contract-qualified.json";
const string PriorRed="docs/planning/evidence/M3-W02-C01-contract-red.json";
const string RedEvidence="docs/planning/evidence/M3-W02-C01-boundary-red.json";
const string Qualified="docs/planning/evidence/M3-W02-C01-boundaries-qualified.json";
const string BaselineProvider="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string Smoke="tests/providers/m3-w02-c01/ManagedStatisticsSmoke.csproj";
const string Backlog="docs/planning/backlog.json";
const string Version="0.4.0-preview.2";
const int IndependentGreenTests=23;
const int TotalStatisticsTests=44;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
// A completed C01 receipt is the exact target discriminator. Native phase
// completion also retires the active-phase cursor through the typed SDK.
var target=File.Exists(Path.Combine(root,Qualified));
var baseline=!target;
p.Workflow.RequireCurrentRun("RS045","M3",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS044",1,
    "feat: qualify independent finite and exact descriptive statistics",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-exact-and-compensated-descriptive-statistics-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(19));
p.Require.JsonValueEquals(Prior,"/complete_stats_tests_passed",JsonValue.Create(20));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create("0.4.0-preview.1"));
p.Require.JsonValueEquals(BaselineProvider,"/selected_package_id",
    JsonValue.Create("MathNet.Numerics"));
p.Require.JsonValueEquals(BaselineProvider,"/selected_package_version",
    JsonValue.Create("5.0.0"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.43"),JsonValue.Create("0.1.44"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.44":"0.1.43"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/7/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/23/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/24/status",JsonValue.Create("planned"));
if(target)
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("frequency-weight-and-explicit-null-missing-statistics-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreenTests));
}
else
{
    p.Require.FileMissing(Qualified);
    p.Require.FileMissing(RedEvidence);
    p.Require.FileMissing(Source);
    p.Require.FileMissing(RedTest);
    p.Require.FileMissing(GreenTest);
    p.Require.FileMissing(Decision);
}

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

// First compile the independent RED using only the RS044 qualified product.
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W02C01T2RedTests.FrequencyWeightAndMissingPolicyRequireAnExplicitPublicContract",
        "--logger","console;verbosity=normal");
    const string marker="M3-W02-C01-T2 RED: explicit frequency weights and missing-value policy contract is absent.";
    if(observed.Code==0||!observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS045 missing-contract RED not independently observed: "+
            Relevant(observed.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C01-boundary-red",
        ["status"]="controlled-explicit-frequency-and-missing-policy-contract-absent-red-observed",
        ["prior_T1_evidence"]=Prior,
        ["test"]=RedTest,["expected_marker"]=marker,
        ["scope"]="Compiled against the RS044 package before installing new public policy contracts."
    };
    p.Files.WriteComplete(RedEvidence,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("controlled-explicit-frequency-and-missing-policy-contract-absent-red-observed"));
}

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Source,Shared,Project,GreenTest,Document,Decision})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    digests[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing SDK staged-file identity: "+file);
}

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
var green=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M3W02C01T2Tests",
    "--logger","console;verbosity=normal");
if(!green.Contains("Total tests: 23",StringComparison.OrdinalIgnoreCase) ||
   !green.Contains("Passed: 23",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS045 all 23 independent frequency/missing GREEN oracles required.");

var complete=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=normal");
if(!complete.Contains("Total tests: 44",StringComparison.OrdinalIgnoreCase) ||
   !complete.Contains("Passed: 44",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS045 all 44 prior and new descriptive tests required.");

// Repeated managed Math.NET smoke is a separate test-only oracle.
var smoke=Required(root,"dotnet","run","--project",Smoke,"-c","Release");
if(!smoke.Contains("M3-W02-C01:managed-mathnet-5.0.0-descriptive-oracles:qualified",
    StringComparison.Ordinal))
    throw new InvalidOperationException("Existing managed descriptive corroboration failed.");

Required(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","test",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m3-w02-c01-t2-local-feed");
Required(root,"dotnet","pack",Project,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.statistics."+Version+".nupkg");
if(!File.Exists(archive))throw new InvalidOperationException("RS045 package missing.");
using(var zip=ZipFile.OpenRead(archive))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.statistics.dll") is null)
        throw new InvalidOperationException("RS045 package lacks compiled statistics assembly.");
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
        throw new InvalidOperationException("RS045 wrong package version or unexpected dependency.");
}

var evidence=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W02-C01-boundaries-qualified",
    ["status"]="frequency-weight-and-explicit-null-missing-statistics-qualified",
    ["prior_T1_evidence"]=Prior,["prior_T1_RED"]=PriorRed,
    ["controlled_T2_RED"]=RedEvidence,
    ["new_weighted_source"]=Source,["new_weighted_source_sha256"]=digests[Source],
    ["shared_compensation_source"]=Shared,["shared_source_sha256"]=digests[Shared],
    ["new_green_test"]=GreenTest,["green_test_sha256"]=digests[GreenTest],
    ["red_test"]=RedTest,["red_test_sha256"]=redInstalled.AfterSha256,
    ["policy_decision"]=Decision,["decision_sha256"]=digests[Decision],
    ["distribution"]=Document,["distribution_sha256"]=digests[Document],
    ["independent_green_tests_passed"]=IndependentGreenTests,
    ["descriptive_suite_total_passed"]=TotalStatisticsTests,
    ["controlled_red_then_green"]=true,
    ["missing_policy"]="Null-only; default Reject; caller-opt-in Skip; NaN/infinity always refused.",
    ["weight_semantics"]="positive integer replication frequencies, not continuous reliability weights",
    ["weighted_population_denominator"]="effective N",
    ["weighted_sample_denominator"]="effective N-1 when N>1, otherwise null",
    ["source_row_budget"]=4096,["effective_observation_budget"]=4096,
    ["independent_expansion_oracle"]="1 weighted twice and 3 once: sum 5, mean 5/3, population variance 8/9, sample variance 4/3",
    ["nonfinite_and_oversized_inputs_refused"]=true,
    ["no_partial_or_mutable_result_aliases"]=true,
    ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
    ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["existing_managed_mathnet_smoke_passed"]=true,
    ["existing_quantities_and_numerics_regressions_passed"]=true,
    ["local_package_id"]="dw.tools.math.statistics",
    ["local_package_version"]=Version,
    ["local_package_bytes"]=new FileInfo(archive).Length,
    ["local_package_dependencies"]=p.Json.StringArray("dw.quantities"),
    ["nonclaims"]=p.Json.StringArray(
        "No general continuous or reliability-weighted sample-variance convention or nonrandom missing-data estimator.",
        "No exact rational weighted moments, infinite streams, unbounded observations or complete floating error bound.",
        "No Decision/MCDM risk-specific statistics, nutrition-domain dataflow, provider runtime dependencies or other RID admission.",
        "No public NuGet feed, dw.discovery agent skill, BrokerReady or Aura invocation.")
};
p.Files.WriteComplete(Qualified,
    evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject(Backlog,plan=>
{
    plan["plan_version"]="0.1.44";
    var c=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Task);
    c["status"]="done";
    c["refinement"]="RS044 qualified bounded exact rational and finite compensated descriptive statistics with 19 independent T1 GREEN oracles. RS045 qualifies explicit null Reject/Skip missing policy and positive integer frequency-replicated weighted moments with 23 new GREEN oracles, exact-expanded-sample variance denominators, nonfinite/overflow and 4096-record/frequency limits. The complete 44-test statistics suite, separate managed Math.NET oracle, prior rational/numeric regressions, deterministic policy decision and provider-free local preview.2 NuGet manifest close C01 without claiming continuous weights.";
    c["readiness"]="closed: both T1 and T2 qualified; M3-W02-C02 quantiles/correlation remains not-ready";
    var previousFiles=c["files"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["files"]=p.Json.StringArray(previousFiles.Concat(new[]{
        Source,Shared,Project,RedTest,GreenTest,Document,Decision,RedEvidence,Qualified
    }).Distinct(StringComparer.Ordinal).ToArray());
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W02C01T2Tests",
        "dotnet test "+TestProject+" -c Release",
        "dotnet run --project "+Smoke+" -c Release",
        "dotnet pack "+Project+" -c Release");
    var previousEvidence=c["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["evidence"]=p.Json.StringArray(previousEvidence.Concat(new[]{
        RedEvidence,Qualified
    }).Distinct(StringComparer.Ordinal).ToArray());
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    foreach(var child in t["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
    {
        child["status"]="done";
        child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedEvidence:Qualified);
    }
});

if(baseline)
{
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
else
{
    foreach(var node in new[]{Task,Red,Green,Verify,Phase})
        p.ProjectPlan.RequireNodeState(node,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M3-W02-C02","not-ready");

// The workflow cursor follows C01 completion without retiring M3-W02.
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS045"
       ||(string?)state["activeMilestone"]!="M3"
       ||(string?)state["activeWorkPackage"]!=Work
       ||(string?)state["activePhase"]!=(target?null:Phase)
       ||state["activeTask"] is not null
       ||state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS045 expected exact baseline or completed execution hierarchy.");
    state["activePhase"]=null;
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
    return s.Length>2600?s[..2600]:s;
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
