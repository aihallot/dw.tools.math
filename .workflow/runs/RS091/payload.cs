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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C02",Task="M6-W02-C02-T2";
const string Red="M6-W02-C02-T2-R",Green="M6-W02-C02-T2-G",Verify="M6-W02-C02-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C02-T1-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C02-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-managed-special-angle-gamma-branch-risk-audit-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.4";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Source="src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C02T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C02T2Tests.cs";
const string PriorConsumer="tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj";
const string PriorConsumerSource="tests/consumers/m6-w02-c02-t1/Program.cs";
const string PriorConsumerConfig="tests/consumers/m6-w02-c02-t1/NuGet.Config";
const string Consumer="tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c02-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c02-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c02-t2-special-boundaries.md";
const string Recipe="docs/distribution/m6-special-function-boundaries.md";
const string PackageHelper=".workflow/runs/RS091/staged/checks/m6-w02-c02-t2-package-sha.cs";
const string RedMarker="M6-W02-C02-T2 RED: explicit special-function angular, pole and complex branch audit API is absent.";
const string ConsumerMarker="dw.tools.math/m6-special-function-boundary-consumer/1.1 qualified; probes=8";
const string PriorConsumerMarker="dw.tools.math/m6-special-functions-consumer/1.1 qualified; probes=8";
const int IndependentGreen=87,AllTargeted=88,C02T1=80,C01T2=88,C01T1=70;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS091",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS090",1,
    "feat: qualify bounded managed special functions and positive Gamma in Math scientific",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-managed-bcl-real-complex-special-functions-local-nuget-qualified"));
foreach(var (pointer,value) in new (string,JsonNode?)[]
{
    ("/independent_green_tests_passed",JsonValue.Create(79)),
    ("/red_recovered_and_green_tests_passed",JsonValue.Create(C02T1)),
    ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
    ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
    ("/independent_special_consumer_probes_passed",JsonValue.Create(8)),
    ("/retained_exact_consumer_probes_passed",JsonValue.Create(8)),
    ("/local_product_version",JsonValue.Create("1.1.0-preview.3")),
    ("/m6_w02_c02_t1_done",JsonValue.Create(true)),
    ("/m6_w02_c02_phase_done",JsonValue.Create(false)),
    ("/m6_w02_work_package_done",JsonValue.Create(false)),
    ("/publicly_published",JsonValue.Create(false))
})p.Require.JsonValueEquals(Prior,pointer,value);

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.89"),JsonValue.Create("0.1.90"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.90":"0.1.89"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","done"),("/work_packages/14/status","in_progress"),
    ("/external_dependencies/2/status","satisfied"),
    ("/chunks/43/status","done"),
    ("/chunks/44/status",target?"done":"in_progress"),
    ("/chunks/44/tasks/0/status","done"),
    ("/chunks/44/tasks/1/status",target?"done":"planned"),
    ("/chunks/45/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/44/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.4</Version>":
    "<Version>1.1.0-preview.3</Version>");
p.Require.TextContains(PriorConsumer,target?"Version=\"[1.1.0-preview.4]\"":
    "Version=\"[1.1.0-preview.3]\"");
p.Require.TextContains(PriorConsumerSource,target?"Version=\"1.1.0-preview.4\"":
    "Version=\"1.1.0-preview.3\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C02-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
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
        JsonValue.Create("compiled-special-function-boundary-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var (pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/retained_c02_t1_tests_passed",JsonValue.Create(C02T1)),
        ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
        ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
        ("/independent_boundary_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_t1_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c02_t2_done",JsonValue.Create(true)),
        ("/m6_w02_c02_phase_done",JsonValue.Create(true)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing compiled special-function audit RED source SHA.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W02C02T2RedTests.BoundedRealComplexAnglePoleAndBranchAuditMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Require one genuinely compiled public boundary API RED before staging source: "+
            Relevant(redOutput.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C02-T2-contract-red",
        ["status"]="compiled-special-function-boundary-public-api-red-observed",
        ["prior_qualified_t1"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_assertion"]=RedMarker,["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,PriorConsumer,PriorConsumerSource,
    Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged special-function science source SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var newTests=Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W02C02T2","--logger","console;verbosity=normal");
MustPass(newTests,AllTargeted,"boundary RED-recovered and GREEN");
foreach(var (filter,count,description) in new[]{
    ("M6W02C02T1",C02T1,"retained C02 T1 special provider"),
    ("M6W02C01T2",C01T2,"retained C01 T2 exact boundaries"),
    ("M6W02C01T1",C01T1,"retained C01 T1 exact integers")})
{
    var regressionOutput=Required(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore","--filter","FullyQualifiedName~"+filter,
        "--logger","console;verbosity=normal");
    MustPass(regressionOutput,count,description);
}
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regression=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!regression.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained Math regression not passed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m6-w02-c02-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("No real locally packed scientific preview.4 archive.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C02-T2-PKG-SHA256:");

static void CheckSourceFreeConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1||
       (string?)references[0].Attribute("Include")!="dw.tools.math.scientific"||
       (string?)references[0].Attribute("Version")!="["+version+"]"||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Source-free client must pin one exact local Math package: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("No external NuGet feeds are allowed: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W02-C02-T2-CONSUMER-SHA256:","m6-w02-c02-t2-boundary-cache"),
    (PriorConsumer,PriorConsumerConfig,PriorConsumerMarker,
        "M6-W02-C02-T1-CONSUMER-SHA256:","m6-w02-c02-t2-retained-cache")
};
var receipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckSourceFreeConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(output,client.Marker))
        throw new InvalidOperationException("Eight distinct NuGet-only boundary oracles absent: "+client.Project);
    var receipt=ReadReceipt(output,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||receipt.Sha!=inspected.Sha)
        throw new InvalidOperationException("Actual local NuGet bytes/SHA mismatch across independent clients.");
    receipts.Add(new JsonObject
    {
        ["project"]=client.Project,["marker"]=client.Marker,
        ["package_sha256"]=receipt.Sha,["package_bytes"]=receipt.Bytes,
        ["independent_probes"]=8,["project_references"]=0
    });
}
if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C02-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_t1_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can callers explicitly declare real angular units, refuse Gamma poles and require principal complex logarithm cut approval without unsupported precision or numeric range claims?",
        ["observation"]="One compiled missing-public-boundary-API RED recovered, 87 GREEN, 238 retained C01/C02 T1 tests, four core Math regressions, one real local preview.4 archive and two SHA-equal eight-probe NuGet-only clients.",
        ["decision"]="Admit explicit radians/degrees real trig, typed Gamma pole and risk refusals, opt-in principal complex negative-real log cut; reject complex Gamma and nonprincipal alternatives without partial results.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_consumer_project"]=PriorConsumer,
        ["retained_consumer_project_sha256"]=hashes[PriorConsumer],
        ["retained_consumer_source"]=PriorConsumerSource,
        ["retained_consumer_source_sha256"]=hashes[PriorConsumerSource],
        ["boundary_consumer_project"]=Consumer,
        ["boundary_consumer_project_sha256"]=hashes[Consumer],
        ["boundary_consumer_source"]=ConsumerSource,
        ["boundary_consumer_source_sha256"]=hashes[ConsumerSource],
        ["boundary_consumer_config"]=ConsumerConfig,
        ["boundary_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_c02_t1_tests_passed"]=C02T1,
        ["retained_c01_t2_tests_passed"]=C01T2,
        ["retained_c01_t1_tests_passed"]=C01T1,
        ["regression_projects_passed"]=4,
        ["independent_boundary_consumer_probes_passed"]=8,
        ["retained_t1_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["real_trigonometry_requires_declared_units"]=true,
        ["degree_to_radian_conversion_approximate"]=true,
        ["real_nonpositive_integer_gamma_poles_refused"]=true,
        ["complex_gamma_supported"]=false,
        ["complex_negative_real_cut_requires_opt_in"]=true,
        ["only_principal_complex_log_branch_admitted"]=true,
        ["exponential_risk_prechecks_are_not_observed_overflows"]=true,
        ["certified_error_bounds"]=false,
        ["publicly_published"]=false,
        ["m6_w02_c02_t2_done"]=true,
        ["m6_w02_c02_phase_done"]=true,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C03 geometry, signals and time series: split the accepted L-size chunk before ready and qualify independent finite kernels.",
        ["nonclaims"]=p.Json.StringArray(
            "Gamma is a positive-real bounded Lanczos approximation; negative-integer poles do not imply computed analytic continuation or a proven error bound.",
            "Angular degrees to radians is binary64 approximation, not exact unit conversion.",
            "Complex log principal cut is caller-selected; no alternate branch, complex Gamma or global branch continuity guarantee.",
            "Exponential risk statuses are preemptive numeric refusals, not observations of actual overflow or underflow.",
            "M6-W02/M6 stay open; no stable/public NuGet, untested OS, foreign provider, CAS, agent/Decision/MCDM adoption.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.90";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="in_progress"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("Accepted M6-W02-C02 T2 baseline changed.");
        phase["status"]="done";
        phase["refinement"]="RS090 T1 qualified a managed BCL-only positive-real Gamma and bounded real/complex principal elementary-function provider with one compiled RED, 79 GREEN and local NuGet SHA. RS091 T2 adds explicit real degrees/radians, negative-integer Gamma pole refusal, complex logarithm cut opt-in, and typed exponential overflow/underflow risk prechecks. Its compiled RED, 87 GREEN, 238 retained scientific tests, four Math regressions and two independent eight-probe source-free consumers sharing one real local preview.4 SHA close only C02; M6-W02/M6 remain in progress.";
        phase["readiness"]="C02 complete with explicit provider, domain, angle unit, Gamma pole, complex branch and finite risk policies. M6-W02-C03 planned L-size split must precede its qualification.";
        var before=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(before.Concat(new[]{
            Product,Source,RedTest,GreenTests,PriorConsumer,PriorConsumerSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C02T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+PriorConsumer+" -c Release");
        var oldEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(
                (string?)sub["id"]==Red?RedProof:Qualified);
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
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS091"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("C02-T2 native DWF state cursor differs from baseline or target.");
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
static bool HasExactLine(string output,string marker) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(x=>x.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.scientific",version="1.1.0-preview.4";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)
        throw new InvalidOperationException("Exactly one local Math scientific NuGet receipt required: "+marker);
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed actual NuGet byte/SHA receipt.");
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
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
