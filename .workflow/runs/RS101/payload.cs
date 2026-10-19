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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C04",Task="M6-W02-C04-T1";
const string Red="M6-W02-C04-T1-R",Green="M6-W02-C04-T1-G",Verify="M6-W02-C04-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C04-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C04-T1-qualified.json";
const string RedStatus="compiled-pure-finance-effective-rate-public-api-red-observed";
const string QualifiedStatus="bounded-effective-discrete-rate-discount-single-sign-change-irr-local-nuget-qualified";
const string Version="1.1.0-preview.13",Solution="dw.tools.math.slnx";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Source="src/projects/dw.tools.math.scientific/M6W02C04Contract.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C04T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C04T1Tests.cs";
const string ParentBoundary="tests/consumers/m6-w02-c03-t2/m6-scientific-boundary-consumer.csproj";
const string ParentBoundarySource="tests/consumers/m6-w02-c03-t2/Program.cs";
const string ParentBoundaryConfig="tests/consumers/m6-w02-c03-t2/NuGet.Config";
const string ParentDirect="tests/consumers/m6-w02-c03-t1/m6-scientific-families-consumer.csproj";
const string ParentDirectSource="tests/consumers/m6-w02-c03-t1/Program.cs";
const string ParentDirectConfig="tests/consumers/m6-w02-c03-t1/NuGet.Config";
const string UniformConsumer="tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj";
const string UniformConsumerSource="tests/consumers/m6-w02-c03-s03-t2/Program.cs";
const string UniformConsumerConfig="tests/consumers/m6-w02-c03-s03-t2/NuGet.Config";
const string SignalConsumer="tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj";
const string SignalConsumerSource="tests/consumers/m6-w02-c03-s02-t2/Program.cs";
const string SignalConsumerConfig="tests/consumers/m6-w02-c03-s02-t2/NuGet.Config";
const string SpecialConsumer="tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj";
const string SpecialConsumerSource="tests/consumers/m6-w02-c02-t2/Program.cs";
const string SpecialConsumerConfig="tests/consumers/m6-w02-c02-t2/NuGet.Config";
const string Consumer="tests/consumers/m6-w02-c04-t1/m6-effective-finance-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c04-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c04-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c04-t1-financial-rate-contracts.md";
const string Recipe="docs/distribution/m6-pure-finance-t1.md";
const string PackageHelper=".workflow/runs/RS101/staged/checks/m6-w02-c04-t1-package-sha.cs";
const string RedMarker="M6-W02-C04-T1 RED: explicit effective-rate discount and bracketed IRR public API is absent.";
const string ConsumerMarker="dw.tools.math/m6-effective-finance-consumer/1.1 qualified; probes=8";
const string ParentBoundaryMarker="dw.tools.math/m6-scientific-boundary-consumer/1.1 qualified; probes=8";
const string ParentDirectMarker="dw.tools.math/m6-scientific-families-consumer/1.1 qualified; probes=8";
const string UniformMarker="dw.tools.math/m6-uniform-series-boundary-consumer/1.1 qualified; probes=8";
const string SignalMarker="dw.tools.math/m6-direct-signal-boundary-consumer/1.1 qualified; probes=8";
const string SpecialMarker="dw.tools.math/m6-special-function-boundary-consumer/1.1 qualified; probes=8";
const int IndependentGreen=66,AllTargeted=67,C03T2=79,C03T1=57,S03T2=107,S03T1=61,S02T2=69,S02T1=60,S01T2=53,S01T1=25,C02T2=88,C02T1=80,C01T2=88,C01T1=70;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS101",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS100",1,
    "feat: qualify cross-family scientific boundaries and close M6 W02 C03",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-cross-family-scientific-boundary-integration-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.12"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(78));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(C03T2));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_t2_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_parent_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.99"),JsonValue.Create("0.1.100"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.100":"0.1.99"));
foreach(var(pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","done"),("/work_packages/14/status","in_progress"),
    ("/chunks/43/status","done"),("/chunks/44/status","done"),
    ("/chunks/45/id","M6-W02-C03"),("/chunks/45/status","done"),
    ("/chunks/46/status","done"),("/chunks/47/status","done"),("/chunks/48/status","done"),
    ("/chunks/49/id",Phase),("/chunks/49/status",target?"in_progress":"planned"),
    ("/chunks/49/tasks/0/id",Task),
    ("/chunks/49/tasks/0/status",target?"done":"planned"),
    ("/chunks/49/tasks/1/status","planned"),
    ("/chunks/50/id","M6-W02-C05"),("/chunks/50/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
    p.Require.JsonValueEquals(Backlog,
        "/chunks/49/tasks/0/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.13</Version>":
    "<Version>1.1.0-preview.12</Version>");
foreach(var client in new[]{ParentBoundary,ParentDirect,UniformConsumer,SignalConsumer,SpecialConsumer})
    p.Require.TextContains(client,target?"Version=\"[1.1.0-preview.13]\"":
        "Version=\"[1.1.0-preview.12]\"");
foreach(var source in new[]{ParentBoundarySource,ParentDirectSource,UniformConsumerSource,SignalConsumerSource})
    p.Require.TextContains(source,target?"Version = \"1.1.0-preview.13\"":
        "Version = \"1.1.0-preview.12\"");
p.Require.TextContains(SpecialConsumerSource,
    target?"Version=\"1.1.0-preview.13\"":"Version=\"1.1.0-preview.12\"");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState("M6-W02-C02","done");
p.ProjectPlan.RequireNodeState("M6-W02-C03","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState(Phase+"-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C05","not-ready");
if(!target)
{
    foreach(var file in new[]{
        Source,RedTest,GreenTests,Consumer,ConsumerSource,ConsumerConfig,
        Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",JsonValue.Create(RedStatus));
    p.Require.JsonValueEquals(RedProof,"/independent_compiled_red_tests",JsonValue.Create(1));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    foreach(var(pointer,value) in new (string,JsonNode?)[]
    {
        ("/independent_green_tests_passed",JsonValue.Create(IndependentGreen)),
        ("/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted)),
        ("/retained_c03_t2_tests_passed",JsonValue.Create(C03T2)),
        ("/retained_c03_t1_tests_passed",JsonValue.Create(C03T1)),
        ("/retained_s03_t2_tests_passed",JsonValue.Create(S03T2)),
        ("/retained_s03_t1_tests_passed",JsonValue.Create(S03T1)),
        ("/retained_s02_t2_tests_passed",JsonValue.Create(S02T2)),
        ("/retained_s02_t1_tests_passed",JsonValue.Create(S02T1)),
        ("/retained_s01_t2_tests_passed",JsonValue.Create(S01T2)),
        ("/retained_s01_t1_tests_passed",JsonValue.Create(S01T1)),
        ("/retained_c02_t2_tests_passed",JsonValue.Create(C02T2)),
        ("/retained_c02_t1_tests_passed",JsonValue.Create(C02T1)),
        ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
        ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
        ("/finance_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_parent_boundary_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_parent_direct_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_uniform_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_signal_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_special_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c04_t1_done",JsonValue.Create(true)),
        ("/m6_w02_c04_t2_done",JsonValue.Create(false)),
        ("/m6_w02_c04_parent_done",JsonValue.Create(false)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing staged independently compiled financial RED SHA.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redObservation=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W02C04T1RedTests.EffectiveRateDiscountAndIrrPublicContractMustExist",
        "--logger","console;verbosity=normal");
    if(redObservation.Code==0 ||
        !redObservation.Output.Contains(RedMarker,StringComparison.Ordinal) ||
        !redObservation.Output.Contains("Failed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Require an independently compiled missing finance API RED: "+
            Relevant(redObservation.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C04-T1-contract-red",
        ["status"]=RedStatus,["prior_original_c03_qualification"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{
    Product,Source,GreenTests,ParentBoundary,ParentBoundarySource,
    ParentDirect,ParentDirectSource,UniformConsumer,UniformConsumerSource,
    SignalConsumer,SignalConsumerSource,SpecialConsumer,SpecialConsumerSource,
    Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged pure financial source SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var newTests=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W02C04T1",
    "--logger","console;verbosity=normal");
MustPass(newTests,AllTargeted,"pure effective-rate discount and IRR RED/GREEN");
foreach(var(filter,count,label) in new[]{
    ("M6W02C03T2",C03T2,"retained cross-family boundary aggregate"),
    ("M6W02C03T1",C03T1,"retained cross-family direct aggregate"),
    ("M6W02C03S03T2",S03T2,"retained uniform time-grid boundary"),
    ("M6W02C03S03T1",S03T1,"retained uniform-series direct"),
    ("M6W02C03S02T2",S02T2,"retained signal numerical boundary"),
    ("M6W02C03S02T1",S02T1,"retained direct DFT/linear convolution"),
    ("M6W02C03S01T2",S01T2,"retained Euclidean boundary"),
    ("M6W02C03S01T1",S01T1,"retained Euclidean direct"),
    ("M6W02C02T2",C02T2,"retained special-function boundary"),
    ("M6W02C02T1",C02T1,"retained managed special functions"),
    ("M6W02C01T2",C01T2,"retained exact numeric boundary"),
    ("M6W02C01T1",C01T1,"retained exact integer")
})
{
    var regressionOutput=Required(root,"dotnet","test",Test,
        "-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~"+filter,"--logger","console;verbosity=normal");
    MustPass(regressionOutput,count,label);
}
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var project in new[]{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",project,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Retained math regression project not passed: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w02-c04-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Real local scientific preview.13 package missing.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C04-T1-PKG-SHA256:");

static void CheckSourceFreeConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var refs=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(refs.Length!=1 ||
        (string?)refs[0].Attribute("Include")!="dw.tools.math.scientific" ||
        (string?)refs[0].Attribute("Version")!="["+version+"]" ||
        xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Client must use one exact source-free scientific NuGet: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear") ||
        sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External NuGet source forbidden: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W02-C04-T1-CONSUMER-SHA256:","m6-w02-c04-t1-finance-cache"),
    (ParentBoundary,ParentBoundaryConfig,ParentBoundaryMarker,
        "M6-W02-C03-T2-CONSUMER-SHA256:","m6-w02-c04-t1-parent-boundary-cache"),
    (ParentDirect,ParentDirectConfig,ParentDirectMarker,
        "M6-W02-C03-T1-CONSUMER-SHA256:","m6-w02-c04-t1-parent-direct-cache"),
    (UniformConsumer,UniformConsumerConfig,UniformMarker,
        "M6-W02-C03-S03-T2-CONSUMER-SHA256:","m6-w02-c04-t1-uniform-cache"),
    (SignalConsumer,SignalConsumerConfig,SignalMarker,
        "M6-W02-C03-S02-T2-CONSUMER-SHA256:","m6-w02-c04-t1-signal-cache"),
    (SpecialConsumer,SpecialConsumerConfig,SpecialMarker,
        "M6-W02-C02-T2-CONSUMER-SHA256:","m6-w02-c04-t1-special-cache")
};
var receipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckSourceFreeConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var observed=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasExactLine(observed,client.Marker))
        throw new InvalidOperationException("Eight-probe source-free finance NuGet receipt missing: "+client.Project);
    var receipt=ReadReceipt(observed,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes || receipt.Sha!=inspected.Sha)
        throw new InvalidOperationException("Preview.13 package bytes/SHA diverged across clients.");
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
        ["schema_version"]=1,["id"]="M6-W02-C04-T1-qualified",
        ["status"]=QualifiedStatus,["prior_c03_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can BCL-only bounded effective per-period discount and a single-sign-change bracketed IRR solver return typed finite results with independent numeric oracles and no implicit market/calendar inputs?",
        ["observation"]="One compiled missing-finance-API RED recovered, 66 GREEN, 837 retained scientific tests, four Math regressions and six SHA-identical eight-probe local preview.13 NuGet-only clients.",
        ["decision"]="Admit effective discrete rate per caller-managed period, bounded amount and period discounts, and bisection IRR only for one nonzero cashflow sign transition with caller-supplied root bracket; refuse other patterns without market-data claims.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_parent_boundary_project"]=ParentBoundary,
        ["retained_parent_boundary_project_sha256"]=hashes[ParentBoundary],
        ["retained_parent_boundary_source"]=ParentBoundarySource,
        ["retained_parent_boundary_source_sha256"]=hashes[ParentBoundarySource],
        ["retained_parent_direct_project"]=ParentDirect,
        ["retained_parent_direct_project_sha256"]=hashes[ParentDirect],
        ["retained_parent_direct_source"]=ParentDirectSource,
        ["retained_parent_direct_source_sha256"]=hashes[ParentDirectSource],
        ["retained_uniform_project"]=UniformConsumer,
        ["retained_uniform_project_sha256"]=hashes[UniformConsumer],
        ["retained_uniform_source"]=UniformConsumerSource,
        ["retained_uniform_source_sha256"]=hashes[UniformConsumerSource],
        ["retained_signal_project"]=SignalConsumer,
        ["retained_signal_project_sha256"]=hashes[SignalConsumer],
        ["retained_signal_source"]=SignalConsumerSource,
        ["retained_signal_source_sha256"]=hashes[SignalConsumerSource],
        ["retained_special_project"]=SpecialConsumer,
        ["retained_special_project_sha256"]=hashes[SpecialConsumer],
        ["retained_special_source"]=SpecialConsumerSource,
        ["retained_special_source_sha256"]=hashes[SpecialConsumerSource],
        ["finance_consumer_project"]=Consumer,
        ["finance_consumer_project_sha256"]=hashes[Consumer],
        ["finance_consumer_source"]=ConsumerSource,
        ["finance_consumer_source_sha256"]=hashes[ConsumerSource],
        ["finance_consumer_config"]=ConsumerConfig,
        ["finance_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_c03_t2_tests_passed"]=C03T2,
        ["retained_c03_t1_tests_passed"]=C03T1,
        ["retained_s03_t2_tests_passed"]=S03T2,
        ["retained_s03_t1_tests_passed"]=S03T1,
        ["retained_s02_t2_tests_passed"]=S02T2,
        ["retained_s02_t1_tests_passed"]=S02T1,
        ["retained_s01_t2_tests_passed"]=S01T2,
        ["retained_s01_t1_tests_passed"]=S01T1,
        ["retained_c02_t2_tests_passed"]=C02T2,
        ["retained_c02_t1_tests_passed"]=C02T1,
        ["retained_c01_t2_tests_passed"]=C01T2,
        ["retained_c01_t1_tests_passed"]=C01T1,
        ["regression_projects_passed"]=4,
        ["finance_consumer_probes_passed"]=8,
        ["retained_parent_boundary_consumer_probes_passed"]=8,
        ["retained_parent_direct_consumer_probes_passed"]=8,
        ["retained_uniform_consumer_probes_passed"]=8,
        ["retained_signal_consumer_probes_passed"]=8,
        ["retained_special_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["discount_oracle"]="100 / (1 + 0.1) = 1000/11",
        ["irr_oracle"]="[-100,110] has IRR 0.1 inside [0,0.5]",
        ["effective_discrete_rate_per_period"]=true,
        ["caller_rate_bracket_required"]=true,
        ["exactly_one_nonzero_cashflow_sign_transition_required"]=true,
        ["max_discount_periods"]=32,
        ["max_cashflow_count"]=16,
        ["minimum_effective_rate"]=-0.9d,
        ["maximum_effective_rate"]=4d,
        ["calendar_day_count_implicit"]=false,
        ["market_data_or_recommendations"]=false,
        ["multiple_root_disposition_certified"]=false,
        ["exact_binary64_error_bounds"]=false,
        ["publicly_published"]=false,
        ["m6_w02_c04_t1_done"]=true,
        ["m6_w02_c04_t2_done"]=false,
        ["m6_w02_c04_parent_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C04-T2: independently qualify caller day-count and financial root-topology boundaries, including multiple/no-solution refusals, without prices or recommendations.",
        ["nonclaims"]=p.Json.StringArray(
            "Rates and integer periods use a caller-supplied effective discrete convention; no calendar, continuous/nominal rate or day-count conversion is inferred.",
            "A sign-pattern refusal is not a theorem that multiple roots exist; a non-bracketed root is not a global proof of no IRR.",
            "Approximate binary64 discounts and bisection results have no certified rounding-error or financial risk bound.",
            "C04-T2, parent C04, C05, work package M6-W02 and release M6 remain unfinished.",
            "No market data, trading advice, public package, untested operating system or AURA/Decision/MCDM downstream adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        doc["plan_version"]="0.1.100";
        var phase=doc["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="planned" ||
            (string?)task["status"]!="planned" ||
            (string?)phase["tasks"]!.AsArray()[1]!["status"]!="planned")
            throw new InvalidOperationException("Pure-finance C04 T1 backlog baseline changed.");
        phase["status"]="in_progress";
        phase["refinement"]="RS100 completed C03. RS101 T1 independently qualifies effective-discrete-per-period discount and single-sign-change bracketed IRR with one compiled RED, 66 GREEN, 837 retained scientific tests, four Math regressions and six isolated SHA-equal preview.13 NuGet consumers. C04-T2 calendar/day-count and root-topology boundaries, C05 and M6 gates remain open.";
        phase["readiness"]="C04 T1 qualified for finite caller-owned amount/rate inputs, integer periods 0..32, 2..16 finite cashflows, one nonzero sign transition and explicitly bracketed root. Dates, day-count, multiple-root/no-solution classification and market data are not admitted.";
        var previousFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(previousFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,ParentBoundary,ParentBoundarySource,
            ParentDirect,ParentDirectSource,UniformConsumer,UniformConsumerSource,
            SignalConsumer,SignalConsumerSource,SpecialConsumer,SpecialConsumerSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C04T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+ParentBoundary+" -c Release",
            "dotnet run --project "+ParentDirect+" -c Release",
            "dotnet run --project "+UniformConsumer+" -c Release",
            "dotnet run --project "+SignalConsumer+" -c Release",
            "dotnet run --project "+SpecialConsumer+" -c Release");
        var previousEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(previousEvidence.Concat(new[]{RedProof,Qualified})
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
    foreach(var id in new[]{Phase,Task,Red})
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
    foreach(var id in new[]{Phase,Task,Red})
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var id in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,"done");
p.ProjectPlan.RequireNodeState(Phase+"-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C05","not-ready");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS101" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?Phase:null) ||
        state["activeTask"] is not null ||
        state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected C04-T1 DWF native pre/post cursor.");
    state["activePhase"]=Phase;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void MustPass(string output,int expected,string label)
{
    if(!output.Contains("Total tests: "+expected,StringComparison.OrdinalIgnoreCase) ||
        !output.Contains("Passed: "+expected,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected "+expected+" passing "+label+
            " tests: "+Relevant(output));
}
static bool HasExactLine(string output,string marker)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(x=>x.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string version="1.1.0-preview.13";
    var prefix=marker+"dw.tools.math.scientific:"+version+":";
    var rows=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(rows.Length!=1)
        throw new InvalidOperationException("Require one actual scientific preview.13 NuGet SHA receipt.");
    var parts=rows[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
        !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes) ||
        bytes<=0 || parts[1].Length!=64 ||
        parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed inspected scientific preview.13 package bytes/SHA.");
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var text=index<0?output:output[index..];
    return text.Length>2300?text[..2300]:text;
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
    if(!process.Start())throw new InvalidOperationException("Cannot launch "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("Bounded direct-signal scientific qualification timed out.");
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
