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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C03",Task="M6-W01-C03-T2";
const string Red="M6-W01-C03-T2-R",Green="M6-W01-C03-T2-G",Verify="M6-W01-C03-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-C03-T1-qualified.json";
const string RedProof="docs/planning/evidence/M6-W01-C03-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-differential-declared-regularity-and-ode-regime-audit-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.6";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Source="src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C03T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C03T2Tests.cs";
const string PreviousConsumer="tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj";
const string PreviousConsumerSource="tests/consumers/m6-w01-c03-t1/Program.cs";
const string PreviousConsumerConfig="tests/consumers/m6-w01-c03-t1/NuGet.Config";
const string Consumer="tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w01-c03-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c03-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c03-t2-differential-boundaries.md";
const string Recipe="docs/distribution/m6-differential-boundaries.md";
const string ArchiveHelper=".workflow/runs/RS086/staged/checks/m6-w01-c03-t2-package-sha.cs";
const string RedMarker="M6-W01-C03-T2 RED: explicit differential and ODE regime audit API is absent.";
const string NewConsumerMarker="dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes=8";
const string PreviousConsumerMarker="dw.tools.math/m6-differential-consumer/1.1 qualified; probes=8";
const int IndependentGreen=66,AllTargeted=67,C03T1=58,C02T2=51,C02T1=44,C01T2=60,C01T1=48;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS086",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS085",1,
    "feat: qualify bounded central-difference and RK4 numerical analysis contracts",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",JsonValue.Create(
    "bounded-binary64-central-difference-and-rk4-local-nuget-qualified"));
foreach(var (pointer,value) in new (string,JsonNode?)[]
{
    ("/independent_green_tests_passed",JsonValue.Create(57)),
    ("/red_recovered_and_green_tests_passed",JsonValue.Create(C03T1)),
    ("/retained_c02_t2_tests_passed",JsonValue.Create(C02T2)),
    ("/retained_c02_t1_tests_passed",JsonValue.Create(C02T1)),
    ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
    ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
    ("/independent_differential_consumer_probes",JsonValue.Create(8)),
    ("/retained_univariate_consumer_probes",JsonValue.Create(8)),
    ("/local_product_version",JsonValue.Create("1.1.0-preview.5")),
    ("/m6_w01_c03_t1_done",JsonValue.Create(true)),
    ("/m6_w01_c03_phase_done",JsonValue.Create(false)),
    ("/m6_w01_work_package_done",JsonValue.Create(false)),
    ("/publicly_published",JsonValue.Create(false))
})p.Require.JsonValueEquals(Prior,pointer,value);
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.84"),JsonValue.Create("0.1.85"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.85":"0.1.84"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","in_progress"),("/work_packages/14/status","planned"),
    ("/chunks/40/status","done"),("/chunks/41/status","done"),
    ("/chunks/42/status",target?"done":"in_progress"),
    ("/chunks/42/tasks/0/status","done"),
    ("/chunks/42/tasks/1/status",target?"done":"planned"),
    ("/chunks/43/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/42/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.6</Version>":
    "<Version>1.1.0-preview.5</Version>");
p.Require.TextContains(PreviousConsumer,target?"Version=\"[1.1.0-preview.6]\"":
    "Version=\"[1.1.0-preview.5]\"");
p.Require.TextContains(PreviousConsumerSource,target?"Version=\"1.1.0-preview.6\"":
    "Version=\"1.1.0-preview.5\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState("M6-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C03-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Consumer,ConsumerSource,
        ConsumerConfig,Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-differential-boundary-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var (pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/retained_c03_t1_tests_passed",JsonValue.Create(C03T1)),
        ("/retained_c02_t2_tests_passed",JsonValue.Create(C02T2)),
        ("/retained_c02_t1_tests_passed",JsonValue.Create(C02T1)),
        ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
        ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
        ("/independent_boundary_consumer_probes",JsonValue.Create(8)),
        ("/retained_differential_consumer_probes",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w01_c03_t2_done",JsonValue.Create(true)),
        ("/m6_w01_c03_phase_done",JsonValue.Create(true)),
        ("/m6_w01_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing compiled differential RED staged SHA.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redLog=Run(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C03T2RedTests.PublicDifferentialAndOdeBoundaryAuditMustExist",
        "--logger","console;verbosity=normal");
    if(redLog.Code==0||!redLog.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("An independently compiled missing-public-API RED was not observed: "+Relevant(redLog.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C03-T2-contract-red",
        ["status"]="compiled-differential-boundary-public-api-red-observed",
        ["prior_qualified_t1"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(
        new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,PreviousConsumer,
    PreviousConsumerSource,Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var changed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=changed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged numeric audit source SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W01C03T2","--logger","console;verbosity=normal");
MustPass(targeted,AllTargeted,"new differential/ODE audit");
foreach(var (filter,count,label) in new[]{
    ("M6W01C03T1",C03T1,"C03 T1"),
    ("M6W01C02T2",C02T2,"C02 T2"),
    ("M6W01C02T1",C02T1,"C02 T1"),
    ("M6W01C01T2",C01T2,"C01 T2"),
    ("M6W01C01T1",C01T1,"C01 T1")})
{
    var previous=Required(root,"dotnet","test",Test,"-c","Release","--no-build",
        "--no-restore","--filter","FullyQualifiedName~"+filter,
        "--logger","console;verbosity=normal");
    MustPass(previous,count,label);
}
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Relevant Math numerical regression did not pass: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w01-c03-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Exact local preview.6 Math package missing.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",ArchiveHelper,"--",archive),
    "M6-W01-C03-T2-PKG-SHA256:");

static void VerifySourceFreeConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1||
       (string?)references[0].Attribute("Include")!="dw.tools.math.analysis"||
       (string?)references[0].Attribute("Version")!="["+version+"]"||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Expected one exact pinned source-free Math NuGet package: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("Foreign NuGet feeds are not admissible: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,NewConsumerMarker,
        "M6-W01-C03-T2-CONSUMER-SHA256:","m6-w01-c03-t2-boundary-cache"),
    (PreviousConsumer,PreviousConsumerConfig,PreviousConsumerMarker,
        "M6-W01-C03-T1-CONSUMER-SHA256:","m6-w01-c03-t2-retained-cache")
};
var receipts=new List<JsonNode>();
foreach(var client in clients)
{
    VerifySourceFreeConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var observedClient=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(observedClient,client.Marker))
        throw new InvalidOperationException("Eight source-free independent probes absent: "+client.Project);
    var receipt=ReadReceipt(observedClient,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||
       !string.Equals(receipt.Sha,inspected.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("NuGet archive bytes/SHA disagreed between clients.");
    receipts.Add(new JsonObject
    {
        ["project"]=client.Project,["marker"]=client.Marker,
        ["package_sha256"]=receipt.Sha,["package_bytes"]=receipt.Bytes,
        ["independent_probes"]=8,["project_references"]=0
    });
}
if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C03-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_t1_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can finite derivative/RK4 observations require explicit caller regularity and nonstiff/event-free assumptions without false numerical theorems?",
        ["observation"]="Compiled public-API RED recovered, 66 GREEN, 261 retained targeted tests, two Math regressions, one local preview.6 archive and two independent eight-probe source-free NuGet consumers with matching SHA.",
        ["decision"]="Admit only caller-assumed smooth derivatives and nonstiff, event-free scalar IVPs. Known stiffness, events, PDE and unknown regimes refuse without partial values; no smoothness, stability or error theorem is certified.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_consumer_project"]=PreviousConsumer,
        ["retained_consumer_project_sha256"]=hashes[PreviousConsumer],
        ["retained_consumer_source"]=PreviousConsumerSource,
        ["retained_consumer_source_sha256"]=hashes[PreviousConsumerSource],
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
        ["retained_c03_t1_tests_passed"]=C03T1,
        ["retained_c02_t2_tests_passed"]=C02T2,
        ["retained_c02_t1_tests_passed"]=C02T1,
        ["retained_c01_t2_tests_passed"]=C01T2,
        ["retained_c01_t1_tests_passed"]=C01T1,
        ["targeted_existing_regression_projects_passed"]=2,
        ["independent_boundary_consumer_probes"]=8,
        ["retained_differential_consumer_probes"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["accepted_square_derivative_at_two"]=4d,
        ["accepted_euler_growth_ode_t1"]=global::System.Math.E,
        ["derivative_smoothness_verified"]=false,
        ["ode_nonstiffness_verified"]=false,
        ["event_absence_verified"]=false,
        ["ode_global_error_bound_certified"]=false,
        ["unqualified_regimes_rejected"]=true,
        ["publicly_published"]=false,
        ["m6_w01_c03_t2_done"]=true,
        ["m6_w01_c03_phase_done"]=true,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W01 numerical-analysis closure gate and M6-W02 scientific expansion require separate qualification.",
        ["nonclaims"]=p.Json.StringArray(
            "Caller-declared smoothness is not verified; a finite difference does not prove a derivative exists.",
            "Nonstiffness and absence of events are caller assumptions, not independently detected.",
            "RK4 step doubling does not prove global error or stability, and stiff IVPs, events and PDE are out of scope.",
            "No stable/public NuGet, nonobserved OS or downstream AURA/Decision/MCDM adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.85";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="in_progress"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("C03 T2 accepted baseline drifted.");
        phase["status"]="done";
        phase["refinement"]="RS085 T1 qualified finite central difference and scalar RK4 with 57 GREEN tests and one compiled RED. RS086 T2 adds explicit unverified caller smoothness, nonstiff/event-free assumption policies and typed unsupported regime refusals. Its 66 GREEN plus recovered RED, 261 retained tests, two Math regressions and two eight-probe NuGet-only consumers from one SHA-inspected local preview.6 package complete C03. M6-W01/M6 remain in progress.";
        phase["readiness"]="C03 complete: bounded numerical derivative/ODE contracts plus typed and explicitly noncertified regularity, stability and event policies. M6-W01 remains open for separate closure.";
        var originalFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(originalFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,PreviousConsumer,PreviousConsumerSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C03T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+PreviousConsumer+" -c Release");
        var evidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(evidence.Concat(new[]{RedProof,Qualified})
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
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState("M6-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS086"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("C03 T2 native cursor diverged from exact baseline/target.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void MustPass(string log,int expected,string label)
{
    if(!log.Contains("Total tests: "+expected,StringComparison.OrdinalIgnoreCase)||
       !log.Contains("Passed: "+expected,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected "+expected+" passed "+label+" tests: "+Relevant(log));
}
static bool HasExactLine(string output,string marker)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.analysis",version="1.1.0-preview.6";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)throw new InvalidOperationException("Expected one local NuGet SHA receipt: "+marker);
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid real NuGet bytes/SHA-256 receipt.");
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
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" exceeded finite Math subprocess timeout.");
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
