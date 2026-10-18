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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C02",Task="M6-W02-C02-T1";
const string Red="M6-W02-C02-T1-R",Green="M6-W02-C02-T1-G",Verify="M6-W02-C02-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json";
const string External="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C02-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C02-T1-qualified.json";
const string QualifiedStatus="bounded-managed-bcl-real-complex-special-functions-local-nuget-qualified";
const string Version="1.1.0-preview.3",Solution="dw.tools.math.slnx";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Contracts="src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs";
const string Source="src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C02T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C02T1Tests.cs";
const string RetainedConsumer="tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj";
const string RetainedSource="tests/consumers/m6-w02-c01-t2/Program.cs";
const string RetainedConfig="tests/consumers/m6-w02-c01-t2/NuGet.Config";
const string Consumer="tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c02-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c02-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c02-t1-managed-special-functions.md";
const string Recipe="docs/distribution/m6-special-functions-real-complex.md";
const string PackageHelper=".workflow/runs/RS090/staged/checks/m6-w02-c02-t1-package-sha.cs";
const string RedMarker="M6-W02-C02-T1 RED: public bounded real and complex special-function facade is absent.";
const string ConsumerMarker="dw.tools.math/m6-special-functions-consumer/1.1 qualified; probes=8";
const string RetainedMarker="dw.tools.math/m6-exact-boundary-consumer/1.1 qualified; probes=8";
const int IndependentGreen=79,AllTargeted=80,C01T1=70,C01T2=88;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS090",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS089",1,
    "feat: qualify exact integer sign and intermediate budgets and close M6 W02 C01",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-exact-signed-and-intermediate-bit-audit-local-nuget-qualified"));
foreach(var (pointer,value) in new (string,JsonNode?)[]
{
    ("/independent_green_tests_passed",JsonValue.Create(87)),
    ("/red_recovered_and_green_tests_passed",JsonValue.Create(C01T2)),
    ("/retained_t1_tests_passed",JsonValue.Create(C01T1)),
    ("/independent_boundary_consumer_probes_passed",JsonValue.Create(8)),
    ("/retained_t1_consumer_probes_passed",JsonValue.Create(8)),
    ("/local_product_version",JsonValue.Create("1.1.0-preview.2")),
    ("/m6_w02_c01_phase_done",JsonValue.Create(true)),
    ("/m6_w02_work_package_done",JsonValue.Create(false)),
    ("/publicly_published",JsonValue.Create(false))
})p.Require.JsonValueEquals(Prior,pointer,value);
p.Require.JsonValueEquals(External,"/selected_package_id",JsonValue.Create("MathNet.Numerics"));
p.Require.JsonValueEquals(External,"/selected_package_version",JsonValue.Create("5.0.0"));
p.Require.JsonValueEquals(External,"/package_license_expression",
    JsonValue.Create("MIT"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.88"),JsonValue.Create("0.1.89"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.89":"0.1.88"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","done"),("/work_packages/14/status","in_progress"),
    ("/external_dependencies/2/status","satisfied"),
    ("/chunks/43/status","done"),
    ("/chunks/44/status",target?"in_progress":"planned"),
    ("/chunks/44/tasks/0/status",target?"done":"planned"),
    ("/chunks/44/tasks/1/status","planned"),
    ("/chunks/45/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
foreach(var childIndex in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/44/tasks/0/subtasks/{childIndex}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.3</Version>":
    "<Version>1.1.0-preview.2</Version>");
p.Require.TextContains(RetainedConsumer,
    target?"Version=\"[1.1.0-preview.3]\"":"Version=\"[1.1.0-preview.2]\"");
p.Require.TextContains(RetainedSource,
    target?"Version=\"1.1.0-preview.3\"":"Version=\"1.1.0-preview.2\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
if(!target)
{
    foreach(var file in new[]{Contracts,Source,RedTest,GreenTests,Consumer,ConsumerSource,
        ConsumerConfig,Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-managed-special-functions-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var (pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
        ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
        ("/independent_special_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_exact_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c02_t1_done",JsonValue.Create(true)),
        ("/m6_w02_c02_phase_done",JsonValue.Create(false)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Contracts,RedTest})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing compiled RED provider-contract staging SHA: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redLog=Run(root,"dotnet","test",Test,"-c","Release","--no-build",
        "--no-restore",
        "--filter","FullyQualifiedName~M6W02C02T1RedTests.BoundedManagedSpecialFunctionsProviderAndFacadeMustExist",
        "--logger","console;verbosity=normal");
    if(redLog.Code==0||!redLog.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One genuinely compiled missing special-functions public-provider RED required: "+
            Relevant(redLog.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C02-T1-contract-red",
        ["status"]="compiled-managed-special-functions-public-api-red-observed",
        ["prior_exact_c01"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["observed_red_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,RetainedConsumer,RetainedSource,
    Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged numerical special-function source SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var current=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W02C02T1",
    "--logger","console;verbosity=normal");
MustPass(current,AllTargeted,"new provider and Gamma/complex scientific RED-recovered/GREEN");
foreach(var (filter,count,label) in new[]{
    ("M6W02C01T1",C01T1,"retained C01 T1"),
    ("M6W02C01T2",C01T2,"retained C01 T2")})
{
    var retained=Required(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore","--filter","FullyQualifiedName~"+filter,
        "--logger","console;verbosity=normal");
    MustPass(retained,count,label);
}
Required(root,"dotnet","test",Test,"-c","Release","--no-build",
    "--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained Math special-function regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w02-c02-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Exact real local special-functions preview.3 package missing.");
var inspected=ReadReceipt(Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C02-T1-PKG-SHA256:");

static void CheckIsolatedConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1||
       (string?)references[0].Attribute("Include")!="dw.tools.math.scientific"||
       (string?)references[0].Attribute("Version")!="["+version+"]"||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Only one pinned source-free scientific NuGet is permitted: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External scientific NuGet feed is forbidden: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W02-C02-T1-CONSUMER-SHA256:","m6-w02-c02-t1-special-cache"),
    (RetainedConsumer,RetainedConfig,RetainedMarker,
        "M6-W02-C01-T2-CONSUMER-SHA256:","m6-w02-c02-t1-exact-cache")
};
var receipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckIsolatedConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var consumerOutput=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(consumerOutput,client.Marker))
        throw new InvalidOperationException("Eight independent special-function or exact NuGet probes absent: "+client.Project);
    var receipt=ReadReceipt(consumerOutput,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||receipt.Sha!=inspected.Sha)
        throw new InvalidOperationException("Scientific package SHA or byte count differs across independent clients.");
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
        ["schema_version"]=1,["id"]="M6-W02-C02-T1-qualified",
        ["status"]=QualifiedStatus,
        ["prior_scientific_c01"]=Prior,
        ["external_mathnet_smoke_qualification"]=External,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can one explicit managed provider admit bounded real trig/exp/log and positive-real Gamma plus finite complex principal trig/exp/log without false branch or exactness claims?",
        ["observation"]="Compiled missing-public-provider RED recovered; 79 independent GREEN tests, 158 retained exact C01 tests, four core Math regressions, real local preview.3 SHA and two eight-probe independent NuGet-only consumers.",
        ["decision"]="Select a BCL-managed provider with a disclosed positive-real Lanczos Gamma subset and principal complex BCL elementary subset, explicit radians and range policy, no error theorem or silent MathNet package fallback.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["contracts"]=Contracts,["contracts_sha256"]=hashes[Contracts],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_consumer_project"]=RetainedConsumer,
        ["retained_consumer_project_sha256"]=hashes[RetainedConsumer],
        ["retained_consumer_source"]=RetainedSource,
        ["retained_consumer_source_sha256"]=hashes[RetainedSource],
        ["special_consumer_project"]=Consumer,
        ["special_consumer_project_sha256"]=hashes[Consumer],
        ["special_consumer_source"]=ConsumerSource,
        ["special_consumer_source_sha256"]=hashes[ConsumerSource],
        ["special_consumer_config"]=ConsumerConfig,
        ["special_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_c01_t1_tests_passed"]=C01T1,
        ["retained_c01_t2_tests_passed"]=C01T2,
        ["regression_projects_passed"]=4,
        ["independent_special_consumer_probes_passed"]=8,
        ["retained_exact_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["selected_managed_provider_id"]="dw-math-managed-bcl-special-1",
        ["previous_mathnet_package_not_imported"]=true,
        ["complex_gamma_supported"]=false,
        ["trig_angle_unit"]="radians",
        ["gamma_positive_real_low"]=0.5d,
        ["gamma_positive_real_high"]=20d,
        ["max_complex_component_magnitude"]=12d,
        ["gamma_accuracy_is_certified"]=false,
        ["complex_log_branch"]="principal",
        ["publicly_published"]=false,
        ["m6_w02_c02_t1_done"]=true,
        ["m6_w02_c02_phase_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C02-T2: real Gamma poles/ranges, complex log branches, angle conversion, underflow/overflow and explicitly unsupported complex Gamma.",
        ["nonclaims"]=p.Json.StringArray(
            "Real Gamma is a finite, noncertified Lanczos approximation for x in [0.5,20], not analytic continuation or a proven exact value.",
            "Complex Gamma, degree/radian conversion and alternate log branches are not admitted in T1.",
            "Previously qualified MathNet 5.0.0 remains external smoke evidence; it is not a transitive runtime dependency or automatic fallback.",
            "No cross-platform bitwise precision theorem, public NuGet, nonobserved OS, CAS, AURA, Decision or MCDM adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.89";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="planned"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("C02 T1 baseline differs from accepted planned state.");
        phase["status"]="in_progress";
        phase["refinement"]="RS090 T1 qualifies provider-neutral explicit BCL managed real sin/cos/exp/log and finite positive-real Lanczos Gamma, plus bounded complex principal Sin/Cos/Exp/Log. One compiled RED, 79 GREEN tests, 158 retained C01 tests, four core Math regressions and two SHA-matched eight-probe consumers of actual scientific preview.3. Complex Gamma, poles, alternate branches and degrees conversion are deferred to T2; no numerical guarantee or MathNet auto-import is claimed.";
        phase["readiness"]="C02 T1 admitted managed provider identification, radians, bounded real/complex domains and positive Gamma subset with typed refusals and independent local NuGet oracles; T2 must separately qualify poles, branch, units, overflow/underflow.";
        var existing=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(existing.Concat(new[]{
            Product,Contracts,Source,RedTest,GreenTests,RetainedConsumer,RetainedSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C02T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+RetainedConsumer+" -c Release");
        var priorEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(priorEvidence.Concat(new[]{RedProof,Qualified})
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
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var child in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.ProjectPlan.RequireNodeState("M6-W02-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS090"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?Phase:null)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("C02 T1 native DWF cursor differs from baseline/target.");
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
static bool HasExactLine(string output,string text)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==text);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.scientific",version="1.1.0-preview.3";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)throw new InvalidOperationException("One real special NuGet SHA receipt required.");
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed actual NuGet bytes/SHA-256 receipt.");
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
        throw new TimeoutException(executable+" exceeded bounded special-function timeout.");
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
