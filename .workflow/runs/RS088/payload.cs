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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C01",Task="M6-W02-C01-T1";
const string Red="M6-W02-C01-T1-R",Green="M6-W02-C01-T1-G",Verify="M6-W02-C01-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-local-gate-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C01-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C01-T1-qualified.json";
const string QualifiedStatus="bounded-exact-bigint-and-finite-sequences-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.1";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string ProductLock="src/projects/dw.tools.math.scientific/packages.lock.json";
const string Contracts="src/projects/dw.tools.math.scientific/ExactScientificContracts.cs";
const string Source="src/projects/dw.tools.math.scientific/BoundedExactScientific.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.scientific.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C01T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C01T1Tests.cs";
const string Consumer="tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c01-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c01-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c01-t1-exact-integer-contract.md";
const string Recipe="docs/distribution/m6-exact-integer-science.md";
const string PackageHelper=".workflow/runs/RS088/staged/checks/m6-w02-c01-t1-package-sha.cs";
const string RedMarker="M6-W02-C01-T1 RED: exact gcd, factorial, binomial and finite sequence public API is absent.";
const string ConsumerMarker="dw.tools.math/m6-exact-science-consumer/1.1 qualified; probes=8";
const int IndependentGreen=69,AllTargeted=70;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS088",
    Milestone,target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS087",1,
    "feat: qualify integrated Math numerical analysis local gate and close M6 W01",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m6-w01-all-three-numerical-families-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/prior_family_phases_completed",JsonValue.Create(3));
p.Require.JsonValueEquals(Prior,"/retained_targeted_tests_passed",JsonValue.Create(328));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",JsonValue.Create(24));
p.Require.JsonValueEquals(Prior,"/source_free_consumer_count",JsonValue.Create(3));
p.Require.JsonValueEquals(Prior,"/m6_w01_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.6"));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.86"),JsonValue.Create("0.1.87"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.87":"0.1.86"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/releases/7/status","planned"),
    ("/work_packages/13/status","done"),
    ("/work_packages/14/status",target?"in_progress":"planned"),
    ("/chunks/40/status","done"),("/chunks/41/status","done"),
    ("/chunks/42/status","done"),
    ("/chunks/43/status",target?"in_progress":"planned"),
    ("/chunks/43/tasks/0/status",target?"done":"planned"),
    ("/chunks/43/tasks/1/status","planned"),
    ("/chunks/44/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/43/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Solution,
    target?"<Project Path=\"src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj\" />":
           "<Project Path=\"src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj\" />");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M7","not-ready");
if(!target)
{
    foreach(var file in new[]{
        Product,ProductLock,Contracts,Source,Test,TestLock,
        RedTest,GreenTests,Consumer,ConsumerSource,ConsumerConfig,
        Decision,Recipe,RedProof,Qualified
    })p.Require.FileMissing(file);
}
else
{
    p.Require.TextContains(Product,"<Version>1.1.0-preview.1</Version>");
    p.Require.TextContains(Solution,
        "<Project Path=\"tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj\" />");
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-exact-scientific-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var (pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/independent_nuget_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c01_t1_done",JsonValue.Create(true)),
        ("/m6_w02_c01_phase_done",JsonValue.Create(false)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Solution,Product,ProductLock,Contracts,Test,TestLock,RedTest})
{
    var changed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=changed.AfterSha256 ??
        throw new InvalidOperationException("Missing RED-stage scientific artifact digest: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var observedRed=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W02C01T1RedTests.BoundedExactScientificOperationsMustExist",
        "--logger","console;verbosity=normal");
    if(observedRed.Code==0||
       !observedRed.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("A genuinely compiled absent public exact-scientific API RED is required: "+
            Relevant(observedRed.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C01-T1-contract-red",
        ["status"]="compiled-exact-scientific-public-api-red-observed",
        ["prior_w01_gate"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{
    Source,GreenTests,Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe
})
{
    var changed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=changed.AfterSha256 ??
        throw new InvalidOperationException("Missing exact scientific source digest: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W02C01T1",
    "--logger","console;verbosity=normal");
MustPass(targeted,AllTargeted,"new bounded exact scientific contract and outcomes");
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained Math exact-integer regression refused: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w02-c01-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Actual preview.1 scientific NuGet archive was not generated.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C01-T1-PKG-SHA256:");
var xml=XDocument.Load(Path.Combine(root,Consumer));
var packages=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(packages.Length!=1 ||
   (string?)packages[0].Attribute("Include")!="dw.tools.math.scientific"||
   (string?)packages[0].Attribute("Version")!="["+Version+"]"||
   xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Scientific consumer must pin one source-free package.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
   sources.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("Unqualified external NuGet feed not allowed.");
var isolatedCache=Path.Combine(root,"build","artifacts","m6-w02-c01-t1-consumer-cache");
Required(root,"dotnet","restore",Consumer,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolatedCache);
Required(root,"dotnet","build",Consumer,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",Consumer,
    "-c","Release","--no-build","--no-restore","--",archive);
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight independent exact BigInteger NuGet oracles absent.");
var actual=ReadReceipt(consumerOutput,"M6-W02-C01-T1-CONSUMER-SHA256:");
if(actual.Bytes!=inspected.Bytes||actual.Sha!=inspected.Sha)
    throw new InvalidOperationException("Consumer inspected different actual NuGet bytes or SHA.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C01-T1-qualified",
        ["status"]=QualifiedStatus,["prior_m6_w01_gate"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can bounded BCL-only exact BigInteger gcd/lcm, factorial, binomial and finite arithmetic-series sums be independently consumed without lossy approximation?",
        ["observation"]="One compiled missing-public-API RED recovered, 69 independent GREENs, 3 relevant Math regressions, actual preview.1 archive SHA and eight source-free BigInteger NuGet oracles.",
        ["decision"]="Admit nonnegative gcd/lcm, finite factorial/binomial and signed finite arithmetic sums with exact BigInteger results and explicit bit/iteration budgets, typed no-partial refusals, no primality claim.",
        ["solution"]=Solution,["solution_sha256"]=hashes[Solution],
        ["product"]=Product,["product_sha256"]=hashes[Product],
        ["product_lock"]=ProductLock,["product_lock_sha256"]=hashes[ProductLock],
        ["contracts"]=Contracts,["contracts_sha256"]=hashes[Contracts],
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["test_project"]=Test,["test_project_sha256"]=hashes[Test],
        ["test_lock"]=TestLock,["test_lock_sha256"]=hashes[TestLock],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["consumer_project"]=Consumer,["consumer_project_sha256"]=hashes[Consumer],
        ["consumer_source"]=ConsumerSource,["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["relevant_regression_projects_passed"]=3,
        ["independent_nuget_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["consumer_inspected_package_bytes"]=actual.Bytes,
        ["consumer_inspected_package_sha256"]=actual.Sha,
        ["local_product_version"]=Version,
        ["accepted_gcd_48_18"]=6,
        ["accepted_lcm_6_8"]=24,
        ["accepted_factorial_zero"]=1,
        ["accepted_binomial_5_2"]=10,
        ["accepted_progression_sum_1_2_5"]=25,
        ["operand_max_bits_default"]=128,
        ["output_max_bits_default"]=4096,
        ["maximum_steps_default"]=4096,
        ["publicly_published"]=false,
        ["m6_w02_c01_t1_done"]=true,
        ["m6_w02_c01_phase_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_task"]="M6-W02-C01-T2: independently qualify signed-domain, output size and integer budgets without primality overclaim.",
        ["nonclaims"]=p.Json.StringArray(
            "BigInteger exactness is not an unbounded performance or resource guarantee.",
            "Negative gcd/lcm inputs currently refuse; negative factorial/binomial domains do not silently normalize.",
            "No probabilistic-prime inference, distribution of factorial probabilities, arbitrary infinite series or mathematical provider integration.",
            "No stable/public NuGet, unobserved OS or downstream AURA/Decision/MCDM adoption is qualified.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.87";
        var work=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Work);
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)work["status"]!="planned"||
           (string?)phase["status"]!="planned"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("Accepted M6-W02 C01 T1 baseline drifted.");
        work["status"]="in_progress";
        phase["status"]="in_progress";
        phase["refinement"]="RS088 T1 creates the BCL-only dw.tools.math.scientific net10 package and qualifies bounded exact gcd/lcm, finite factorial/binomial and signed arithmetic sequence sums. One genuinely compiled absent-public-API RED and 69 independent GREENs, three retained core Math regressions, real local preview.1 NuGet SHA and eight independent source-free consumer oracles prove only the stated T1 contract. T2 input signs, output and resource boundaries remain planned.";
        phase["readiness"]="C01 T1 delivered finite exact integer/combinatorial contracts with operand/output bit and step budgets, typed refusals and local NuGet-only evidence. C01 T2 requires independent boundary qualification.";
        phase["files"]=p.Json.StringArray(
            Solution,Product,ProductLock,Contracts,Source,Test,TestLock,
            RedTest,GreenTests,Consumer,ConsumerSource,ConsumerConfig,
            Decision,Recipe,RedProof,Qualified);
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C01T1",
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
    foreach(var node in new[]{Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
    foreach(var node in new[]{Work,Phase,Task,Red})
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
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W02-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS088"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=(target?Work:null)||
       (string?)state["activePhase"]!=(target?Phase:null)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("M6-W02 T1 native DWF cursor differs from declared baseline/target.");
    state["activeWorkPackage"]=Work;
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void MustPass(string output,int expected,string label)
{
    if(!output.Contains("Total tests: "+expected,StringComparison.OrdinalIgnoreCase)||
       !output.Contains("Passed: "+expected,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected "+expected+" passing "+label+" tests: "+Relevant(output));
}
static bool HasExactLine(string output,string expected)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.scientific",version="1.1.0-preview.1";
    var prefix=marker+id+":"+version+":";
    var matches=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matches.Length!=1)
        throw new InvalidOperationException("Exactly one versioned local scientific NuGet receipt required.");
    var parts=matches[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid NuGet archive byte count or SHA-256.");
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2300?part[..2300]:part;
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
    if(!process.Start())throw new InvalidOperationException("Unable to start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" exceeded bounded scientific validation timeout.");
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
