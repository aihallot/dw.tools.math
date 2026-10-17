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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C03",Task="M6-W01-C03-T1";
const string Red="M6-W01-C03-T1-R",Green="M6-W01-C03-T1-G",Verify="M6-W01-C03-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M6-W01-C03-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C03-T1-qualified.json";
const string QualifiedStatus="bounded-binary64-central-difference-and-rk4-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.5";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Source="src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C03T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C03T1Tests.cs";
const string OriginalConsumer="tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj";
const string OriginalSource="tests/consumers/m6-w01-c02-t2/Program.cs";
const string OriginalConfig="tests/consumers/m6-w01-c02-t2/NuGet.Config";
const string Consumer="tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w01-c03-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c03-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c03-t1-differential-ode.md";
const string Recipe="docs/distribution/m6-differential-ode.md";
const string PackageHelper=".workflow/runs/RS085/staged/checks/m6-w01-c03-t1-package-sha.cs";
const string RedMarker="M6-W01-C03-T1 RED: bounded public derivative and ODE API is absent.";
const string NewConsumerMarker="dw.tools.math/m6-differential-consumer/1.1 qualified; probes=8";
const string OriginalConsumerMarker="dw.tools.math/m6-univariate-boundary-consumer/1.1 qualified; probes=8";
const int IndependentGreen=57,AllTargeted=58;
const int C02T2=51,C02T1=44,C01T2=60,C01T1=48;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS085",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS084",1,
    "feat: qualify bounded root residual and Simpson regularity audits and close M6 W01 C02",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-univariate-residual-and-regularity-audit-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(50));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(C02T2));
p.Require.JsonValueEquals(Prior,"/retained_c02_t1_tests_passed",JsonValue.Create(C02T1));
p.Require.JsonValueEquals(Prior,"/retained_c01_t1_tests_passed",JsonValue.Create(C01T1));
p.Require.JsonValueEquals(Prior,"/retained_c01_t2_tests_passed",JsonValue.Create(C01T2));
p.Require.JsonValueEquals(Prior,"/independent_audit_consumer_probes",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/retained_t1_consumer_probes",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.4"));
p.Require.JsonValueEquals(Prior,"/m6_w01_c02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.83"),JsonValue.Create("0.1.84"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.84":"0.1.83"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/13/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/14/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/42/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/43/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.5</Version>":
    "<Version>1.1.0-preview.4</Version>");
p.Require.TextContains(OriginalConsumer,target?"Version=\"[1.1.0-preview.5]\"":
    "Version=\"[1.1.0-preview.4]\"");
p.Require.TextContains(OriginalSource,target?"Version=\"1.1.0-preview.5\"":
    "Version=\"1.1.0-preview.4\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState("M6-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C03-T2","not-ready");
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
        JsonValue.Create("compiled-bounded-differential-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/retained_c02_t2_tests_passed",JsonValue.Create(C02T2));
    p.Require.JsonValueEquals(Qualified,"/retained_c02_t1_tests_passed",JsonValue.Create(C02T1));
    p.Require.JsonValueEquals(Qualified,"/retained_c01_t2_tests_passed",JsonValue.Create(C01T2));
    p.Require.JsonValueEquals(Qualified,"/retained_c01_t1_tests_passed",JsonValue.Create(C01T1));
    p.Require.JsonValueEquals(Qualified,"/independent_differential_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/retained_univariate_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/local_product_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c03_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c03_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redSource=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redSource.AfterSha256 ??
    throw new InvalidOperationException("Compiled RED staged source identity missing.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C03T1RedTests.PublicBoundedDerivativeAndOdeApiMustExist",
        "--logger","console;verbosity=normal");
    if(observed.Code==0 || !observed.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Require one genuinely compiled missing public differential/ODE API RED: "+
            Relevant(observed.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C03-T1-contract-red",
        ["status"]="compiled-bounded-differential-public-api-red-observed",
        ["prior_qualified_c02"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["observed_red_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_error_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,OriginalConsumer,OriginalSource,
    Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var changed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=changed.AfterSha256 ??
        throw new InvalidOperationException("Missing independently staged differential source SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C03T1",
    "--logger","console;verbosity=normal");
MustPass(targeted,AllTargeted,"new differentiation and ODE");
foreach(var (filter,expected,name) in new[]{
    ("M6W01C02T2",C02T2,"retained C02 T2"),
    ("M6W01C02T1",C02T1,"retained C02 T1"),
    ("M6W01C01T2",C01T2,"retained C01 T2"),
    ("M6W01C01T1",C01T1,"retained C01 T1")})
{
    var retainedOutput=Required(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore","--filter","FullyQualifiedName~"+filter,
        "--logger","console;verbosity=normal");
    MustPass(retainedOutput,expected,name);
}
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained Math numerical regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m6-w01-c03-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Actual local preview.5 archive was not generated.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W01-C03-T1-PKG-SHA256:");

static void VerifyPinnedConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1||
       (string?)references[0].Attribute("Include")!="dw.tools.math.analysis"||
       (string?)references[0].Attribute("Version")!="["+version+"]"||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Consumer must pin only the actual local Math NuGet archive: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("Unexpected external NuGet source: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,NewConsumerMarker,
        "M6-W01-C03-T1-CONSUMER-SHA256:","m6-w01-c03-t1-derivative-cache"),
    (OriginalConsumer,OriginalConfig,OriginalConsumerMarker,
        "M6-W01-C02-T2-CONSUMER-SHA256:","m6-w01-c03-t1-retained-cache")
};
var clientReceipts=new List<JsonNode>();
foreach(var client in clients)
{
    VerifyPinnedConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var clientOutput=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasLine(clientOutput,client.Marker))
        throw new InvalidOperationException("Eight independent source-free Math probes were not observed: "+client.Project);
    var receipt=ReadReceipt(clientOutput,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||
       !string.Equals(receipt.Sha,inspected.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("Real package bytes/SHA disagree between separate clients: "+client.Project);
    clientReceipts.Add(new JsonObject
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
        ["schema_version"]=1,["id"]="M6-W01-C03-T1-qualified",
        ["status"]=QualifiedStatus,["prior_c02_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can finite central differences and fixed-step RK4 expose bounded approximate observations and noncertified error diagnostics through a source-free net10 Math package?",
        ["observation"]="Compiled public-API RED recovered, 57 new GREEN tests, 203 retained focused Math tests, two Math regressions, one real local preview.5 package and two eight-probe NuGet consumers with identical SHA.",
        ["decision"]="Admit four-callback h/h2 central difference and N/2N fixed-step scalar RK4 with explicitly finite policies and heuristic estimates, typed nonpartial refusals and no guaranteed local/global-error theorem.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["project"]=Product,["project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["consumer_project"]=Consumer,["consumer_project_sha256"]=hashes[Consumer],
        ["consumer_source"]=ConsumerSource,["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["retained_consumer_project"]=OriginalConsumer,
        ["retained_consumer_project_sha256"]=hashes[OriginalConsumer],
        ["retained_consumer_source"]=OriginalSource,
        ["retained_consumer_source_sha256"]=hashes[OriginalSource],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_c02_t2_tests_passed"]=C02T2,
        ["retained_c02_t1_tests_passed"]=C02T1,
        ["retained_c01_t2_tests_passed"]=C01T2,
        ["retained_c01_t1_tests_passed"]=C01T1,
        ["compiled_red_observed"]=true,
        ["regression_projects_passed"]=2,
        ["independent_differential_consumer_probes"]=8,
        ["retained_univariate_consumer_probes"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(clientReceipts.ToArray()),
        ["local_product_version"]=Version,
        ["accepted_square_derivative_at_two"]=4d,
        ["accepted_euler_growth_ode_t1"]=global::System.Math.E,
        ["derivative_error_bound_certified"]=false,
        ["ode_global_error_bound_certified"]=false,
        ["stiffness_event_or_pde_admitted"]=false,
        ["publicly_published"]=false,
        ["m6_w01_c03_t1_done"]=true,
        ["m6_w01_c03_phase_done"]=false,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W01-C03-T2: independent step, overflow, local/global-error, stiffness, event and PDE boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "Central differences are approximate and the h/h2 difference is a noncertified error estimate.",
            "Fixed-step RK4 step-doubling does not prove global error or stability; stiffness and events are not handled.",
            "No arbitrary ODE systems, PDE, broad OS support, stable/public NuGet or agent/Decision/MCDM integration is certified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.84";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="planned"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M6 C03 T1 accepted baseline changed.");
        phase["status"]="in_progress";
        phase["refinement"]="RS085 T1 delivers provider-free bounded binary64 numerical differentiation and scalar RK4 with one compiled API RED, 57 GREEN tests, 203 retained Math analysis tests, two relevant Math regressions and 16 NuGet-only source-free probes against one SHA-qualified preview.5 archive. Step doubling is a heuristic, not a proof. T2 retains stiffness, overflow, events, PDE and global/local error boundaries.";
        phase["readiness"]="T1 numerical differentiation and scalar IVP solution qualified locally with explicit step and evaluation budgets, typed failures and noncertified estimates; T2 requires independent error/stability/boundary characterization.";
        var previousFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(previousFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,OriginalConsumer,OriginalSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C03T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release");
        var previousEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(previousEvidence.Concat(new[]{RedProof,Qualified})
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
    foreach(var node in new[]{Phase,Task,Red})
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
    foreach(var node in new[]{Phase,Task,Red})
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
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState("M6-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W01-C03-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS085"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?Phase:null)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS085 native workflow cursor drifted from baseline/target.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void MustPass(string output,int count,string label)
{
    if(!output.Contains("Total tests: "+count,StringComparison.OrdinalIgnoreCase)||
       !output.Contains("Passed: "+count,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected "+count+" passed "+label+" tests: "+Relevant(output));
}
static bool HasLine(string output,string marker)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(x=>x.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.analysis",version="1.1.0-preview.5";
    var prefix=marker+id+":"+version+":";
    var matches=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matches.Length!=1)
        throw new InvalidOperationException("Expected one independent versioned local NuGet receipt: "+marker);
    var parts=matches[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(c=>!((c>='0'&&c<='9')||(c>='a'&&c<='f'))))
        throw new InvalidOperationException("Invalid actual package SHA-256 and byte-count receipt.");
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2300?part[..2300]:part;
}
static (int Code,string Output) Run(string root,string command,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=command,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Unable to start "+command);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(command+" exceeded finite Math subprocess timeout.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string command,params string[] args)
{
    var result=Run(root,command,args);
    if(result.Code!=0)
        throw new InvalidOperationException(command+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
