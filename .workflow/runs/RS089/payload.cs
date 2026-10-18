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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C01",Task="M6-W02-C01-T2";
const string Red="M6-W02-C01-T2-R",Green="M6-W02-C01-T2-G",Verify="M6-W02-C01-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C01-T1-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C01-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-exact-signed-and-intermediate-bit-audit-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.2";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Source="src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C01T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C01T2Tests.cs";
const string OriginalConsumer="tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj";
const string OriginalSource="tests/consumers/m6-w02-c01-t1/Program.cs";
const string OriginalConfig="tests/consumers/m6-w02-c01-t1/NuGet.Config";
const string Consumer="tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c01-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c01-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c01-t2-exact-boundaries.md";
const string Recipe="docs/distribution/m6-exact-boundary-science.md";
const string PackageHelper=".workflow/runs/RS089/staged/checks/m6-w02-c01-t2-package-sha.cs";
const string RedMarker="M6-W02-C01-T2 RED: the public signed-normalization and intermediate-budget audit API is absent.";
const string ConsumerMarker="dw.tools.math/m6-exact-boundary-consumer/1.1 qualified; probes=8";
const string OriginalMarker="dw.tools.math/m6-exact-science-consumer/1.1 qualified; probes=8";
const int IndependentGreen=87,AllTargeted=88,RetainedT1=70;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS089",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS088",1,
    "feat: qualify exact integers combinatorics and finite sequences in Math scientific",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-exact-bigint-and-finite-sequences-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(69));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",
    JsonValue.Create(RetainedT1));
p.Require.JsonValueEquals(Prior,"/independent_nuget_consumer_probes_passed",
    JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/local_product_version",
    JsonValue.Create("1.1.0-preview.1"));
p.Require.JsonValueEquals(Prior,"/m6_w02_c01_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_c01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.87"),JsonValue.Create("0.1.88"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.88":"0.1.87"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/releases/7/status","planned"),("/work_packages/13/status","done"),
    ("/work_packages/14/status","in_progress"),
    ("/chunks/40/status","done"),("/chunks/41/status","done"),
    ("/chunks/42/status","done"),
    ("/chunks/43/status",target?"done":"in_progress"),
    ("/chunks/43/tasks/0/status","done"),
    ("/chunks/43/tasks/1/status",target?"done":"planned"),
    ("/chunks/44/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/43/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.2</Version>":
    "<Version>1.1.0-preview.1</Version>");
p.Require.TextContains(OriginalConsumer,
    target?"Version=\"[1.1.0-preview.2]\"":"Version=\"[1.1.0-preview.1]\"");
p.Require.TextContains(OriginalSource,
    target?"Version=\"1.1.0-preview.2\"":"Version=\"1.1.0-preview.1\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01-T1","done");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Consumer,ConsumerSource,
        ConsumerConfig,Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-exact-scientific-audit-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var (pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/retained_t1_tests_passed",JsonValue.Create(RetainedT1)),
        ("/independent_boundary_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_t1_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c01_t2_done",JsonValue.Create(true)),
        ("/m6_w02_c01_phase_done",JsonValue.Create(true)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redSource=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redSource.AfterSha256 ??
    throw new InvalidOperationException("No compiled exact-boundary RED staged source digest.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var observedRed=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W02C01T2RedTests.SignedNormalizationAndFiniteIntermediateAuditMustExist",
        "--logger","console;verbosity=normal");
    if(observedRed.Code==0||
       !observedRed.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Require one genuinely compiled public exact-boundary-audit API RED: "+Relevant(observedRed.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C01-T2-contract-red",
        ["status"]="compiled-exact-scientific-audit-public-api-red-observed",
        ["prior_qualified_t1"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["observed_missing_api_assertion"]=RedMarker,
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
        throw new InvalidOperationException("Missing exact-scientific T2 source digest: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var current=Required(root,"dotnet","test",Test,"-c","Release","--no-build",
    "--no-restore","--filter","FullyQualifiedName~M6W02C01T2",
    "--logger","console;verbosity=normal");
MustPass(current,AllTargeted,"exact signed and intermediate boundary RED-recovered/ GREEN");
var retained=Required(root,"dotnet","test",Test,"-c","Release","--no-build",
    "--no-restore","--filter","FullyQualifiedName~M6W02C01T1",
    "--logger","console;verbosity=normal");
MustPass(retained,RetainedT1,"retained C01 T1 exact integer tests");
Required(root,"dotnet","test",Test,"-c","Release","--no-build",
    "--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained Math scientific regression did not pass: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w02-c01-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Real local scientific preview.2 NuGet archive missing.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C01-T2-PKG-SHA256:");

static void CheckSourceFreeConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1||
       (string?)references[0].Attribute("Include")!="dw.tools.math.scientific"||
       (string?)references[0].Attribute("Version")!="["+version+"]"||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("An independent consumer must use one exactly pinned package: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("Foreign NuGet feeds are forbidden: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W02-C01-T2-CONSUMER-SHA256:","m6-w02-c01-t2-boundary-cache"),
    (OriginalConsumer,OriginalConfig,OriginalMarker,
        "M6-W02-C01-T1-CONSUMER-SHA256:","m6-w02-c01-t2-retained-cache")
};
var receipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckSourceFreeConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var consumerOutput=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(consumerOutput,client.Marker))
        throw new InvalidOperationException("Eight independent source-free exact oracles absent: "+client.Project);
    var receipt=ReadReceipt(consumerOutput,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||receipt.Sha!=inspected.Sha)
        throw new InvalidOperationException("NuGet package actual bytes/SHA differ across scientific consumers: "+client.Project);
    receipts.Add(new JsonObject
    {
        ["project"]=client.Project,["marker"]=client.Marker,
        ["package_bytes"]=receipt.Bytes,["package_sha256"]=receipt.Sha,
        ["independent_probes"]=8,["project_references"]=0
    });
}
if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C01-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_t1_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can explicit signed magnitude admission and independent intermediate size bounds preserve exact BigInteger combinatorics without hidden resource or primality guarantees?",
        ["observation"]="One compiled missing-public-API RED recovered, 87 new GREENs, 70 retained T1 tests, four core Math regressions, one real local preview.2 package and two independent eight-probe source-free NuGet consumers with identical bytes/SHA.",
        ["decision"]="Admit caller-selected signed magnitudes for bounded Euclidean GCD/LCM only, and bound signed arithmetic series products separately from final output. Typed nonpartial refusals and no primality theorem.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_consumer_project"]=OriginalConsumer,
        ["retained_consumer_project_sha256"]=hashes[OriginalConsumer],
        ["retained_consumer_source"]=OriginalSource,
        ["retained_consumer_source_sha256"]=hashes[OriginalSource],
        ["boundary_consumer_project"]=Consumer,
        ["boundary_consumer_project_sha256"]=hashes[Consumer],
        ["boundary_consumer_source"]=ConsumerSource,
        ["boundary_consumer_source_sha256"]=hashes[ConsumerSource],
        ["boundary_consumer_config"]=ConsumerConfig,
        ["boundary_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,
        ["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_t1_tests_passed"]=RetainedT1,
        ["regression_projects_passed"]=4,
        ["independent_boundary_consumer_probes_passed"]=8,
        ["retained_t1_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["signed_normalization_default_reject"]=true,
        ["caller_magnitude_opt_in_only"]=true,
        ["finite_intermediate_size_bound_admitted"]=true,
        ["probabilistic_primality_guarantee"]=false,
        ["publicly_published"]=false,
        ["m6_w02_c01_t2_done"]=true,
        ["m6_w02_c01_phase_done"]=true,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C02-T1: bounded special functions/trigonometry provider qualification with real/complex domains, poles and branch decisions.",
        ["nonclaims"]=p.Json.StringArray(
            "Negative Euclidean inputs normalize only with explicit caller opt-in; T1 default semantics are unchanged.",
            "Finite BigInteger operand, result and sequence-intermediate budgets are not memory or runtime upper-bound theorems for all operations.",
            "No primality assertion, unbounded series, arbitrary-precision performance or probability model is qualified.",
            "No stable/public NuGet, foreign scientific provider, untested OS or AURA/Decision/MCDM adoption is qualified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.88";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="in_progress"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("Accepted M6-W02-C01-T2 baseline changed.");
        phase["status"]="done";
        phase["refinement"]="RS088 T1 qualified bounded exact BigInteger gcd/lcm, factorial, binomial and finite arithmetic sequence sums with one compiled RED, 69 GREEN and a real source-free local preview.1 package. RS089 T2 adds explicit opt-in signed Euclidean normalization, independent finite intermediate-bit budgets and primality nonclaims, with one compiled RED recovered, 87 GREEN, 70 retained T1 tests, four Math regressions and two independent eight-probe consumers from one SHA-inspected preview.2 archive. C01 complete; M6-W02 and M6 remain in progress.";
        phase["readiness"]="C01 qualified: exact bounded integer/combinatorial formulas and explicit signed/intermediate refusal policies, backed by two compiled RED/GREEN gates and source-free NuGet evidence. C02 special functions remains planned.";
        var existingFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(existingFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,OriginalConsumer,OriginalSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C01T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+OriginalConsumer+" -c Release");
        var existingEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(existingEvidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
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
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState("M6-W02-C02","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS089"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("M6-W02-C01 T2 native cursor diverged.");
    state["activePhase"]=null;
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
static bool HasExactLine(string output,string text)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==text);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.scientific",version="1.1.0-preview.2";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)
        throw new InvalidOperationException("Exactly one local scientific NuGet SHA receipt required: "+marker);
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed NuGet package SHA-256 and byte-count receipt.");
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
        throw new TimeoutException(executable+" exceeded bounded scientific payload timeout.");
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
