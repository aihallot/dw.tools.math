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

const string Milestone="M6",Work="M6-W02",Phase="M6-W02-C03-S03",Task="M6-W02-C03-S03-T2";
const string Red="M6-W02-C03-S03-T2-R",Green="M6-W02-C03-S03-T2-G",Verify="M6-W02-C03-S03-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json";
const string Split="docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json";
const string RedProof="docs/planning/evidence/M6-W02-C03-S03-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json";
const string RedStatus="compiled-uniform-grid-numeric-audit-public-api-red-observed";
const string QualifiedStatus="bounded-uniform-grid-interval-and-numeric-risk-local-nuget-qualified";
const string Version="1.1.0-preview.10",Solution="dw.tools.math.slnx";
const string Product="src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Source="src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs";
const string Test="tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2Tests.cs";
const string UniformConsumer="tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj";
const string UniformConsumerSource="tests/consumers/m6-w02-c03-s03-t1/Program.cs";
const string UniformConsumerConfig="tests/consumers/m6-w02-c03-s03-t1/NuGet.Config";
const string SignalConsumer="tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj";
const string SignalConsumerSource="tests/consumers/m6-w02-c03-s02-t2/Program.cs";
const string SignalConsumerConfig="tests/consumers/m6-w02-c03-s02-t2/NuGet.Config";
const string GeometryConsumer="tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj";
const string GeometryConsumerSource="tests/consumers/m6-w02-c03-s01-t2/Program.cs";
const string GeometryConsumerConfig="tests/consumers/m6-w02-c03-s01-t2/NuGet.Config";
const string SpecialConsumer="tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj";
const string SpecialConsumerSource="tests/consumers/m6-w02-c02-t2/Program.cs";
const string SpecialConsumerConfig="tests/consumers/m6-w02-c02-t2/NuGet.Config";
const string Consumer="tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w02-c03-s03-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w02-c03-s03-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w02-c03-s03-t2-uniform-grid-audit.md";
const string Recipe="docs/distribution/m6-uniform-grid-audit.md";
const string PackageHelper=".workflow/runs/RS098/staged/checks/m6-w02-c03-s03-t2-package-sha.cs";
const string RedMarker="M6-W02-C03-S03-T2 RED: explicit uniform time-grid and numerical conditioning audit public API is absent.";
const string ConsumerMarker="dw.tools.math/m6-uniform-series-boundary-consumer/1.1 qualified; probes=8";
const string UniformMarker="dw.tools.math/m6-uniform-series-consumer/1.1 qualified; probes=8";
const string SignalMarker="dw.tools.math/m6-direct-signal-boundary-consumer/1.1 qualified; probes=8";
const string GeometryMarker="dw.tools.math/m6-finite-geometry-boundary-consumer/1.1 qualified; probes=8";
const string SpecialMarker="dw.tools.math/m6-special-function-boundary-consumer/1.1 qualified; probes=8";
const int IndependentGreen=106,AllTargeted=107,S03T1=61,S02T2=69,S02T1=60,S01T2=53,S01T1=25,C02T2=88,C02T1=80,C01T2=88,C01T1=70;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS098",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS097",1,
    "feat: qualify bounded uniform-series differences and moving averages",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-uniform-series-differences-sliding-means-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.9"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(60));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(S03T1));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_s03_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_s03_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w02_c03_parent_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Split,"/native_node_count",JsonValue.Create(523));
p.Require.JsonValueEquals(Split,"/product_qualified",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.96"),JsonValue.Create("0.1.97"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.97":"0.1.96"));
foreach(var(pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/work_packages/13/status","done"),("/work_packages/14/status","in_progress"),
    ("/chunks/43/status","done"),("/chunks/44/status","done"),
    ("/chunks/45/id","M6-W02-C03"),("/chunks/45/status","planned"),
    ("/chunks/46/id","M6-W02-C03-S01"),("/chunks/46/status","done"),
    ("/chunks/47/id","M6-W02-C03-S02"),("/chunks/47/status","done"),
    ("/chunks/48/id",Phase),("/chunks/48/status",target?"done":"in_progress"),
    ("/chunks/48/tasks/0/status","done"),
    ("/chunks/48/tasks/1/id",Task),
    ("/chunks/48/tasks/1/status",target?"done":"planned"),
    ("/chunks/49/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
for(var i=0;i<3;i++)
    p.Require.JsonValueEquals(Backlog,
        "/chunks/48/tasks/1/subtasks/"+i+"/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.10</Version>":
    "<Version>1.1.0-preview.9</Version>");
foreach(var client in new[]{UniformConsumer,SignalConsumer,GeometryConsumer,SpecialConsumer})
    p.Require.TextContains(client,target?"Version=\"[1.1.0-preview.10]\"":
        "Version=\"[1.1.0-preview.9]\"");
foreach(var source in new[]{UniformConsumerSource,SignalConsumerSource,GeometryConsumerSource})
    p.Require.TextContains(source,target?"Version = \"1.1.0-preview.10\"":
        "Version = \"1.1.0-preview.9\"");
p.Require.TextContains(SpecialConsumerSource,
    target?"Version=\"1.1.0-preview.10\"":"Version=\"1.1.0-preview.9\"");
p.Require.TextContains("docs/planning/ValidateNativeDwfAdoption.cs",
    "Require(expected.Count == 523,");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState("M6-W02-C02","done");
p.ProjectPlan.RequireNodeState("M6-W02-C03","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03-S01","done");
p.ProjectPlan.RequireNodeState("M6-W02-C03-S02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState(Phase+"-T1","done");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
foreach(var id in new[]{"M6-W02-C04","M6-W02-C05"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
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
        ("/retained_s03_t1_tests_passed",JsonValue.Create(S03T1)),
        ("/retained_s02_t2_tests_passed",JsonValue.Create(S02T2)),
        ("/retained_s02_t1_tests_passed",JsonValue.Create(S02T1)),
        ("/retained_s01_t2_tests_passed",JsonValue.Create(S01T2)),
        ("/retained_s01_t1_tests_passed",JsonValue.Create(S01T1)),
        ("/retained_c02_t2_tests_passed",JsonValue.Create(C02T2)),
        ("/retained_c02_t1_tests_passed",JsonValue.Create(C02T1)),
        ("/retained_c01_t2_tests_passed",JsonValue.Create(C01T2)),
        ("/retained_c01_t1_tests_passed",JsonValue.Create(C01T1)),
        ("/uniform_audit_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_uniform_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_signal_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_geometry_consumer_probes_passed",JsonValue.Create(8)),
        ("/retained_special_consumer_probes_passed",JsonValue.Create(8)),
        ("/local_product_version",JsonValue.Create(Version)),
        ("/m6_w02_c03_s03_t2_done",JsonValue.Create(true)),
        ("/m6_w02_c03_s03_phase_done",JsonValue.Create(true)),
        ("/m6_w02_c03_parent_done",JsonValue.Create(false)),
        ("/m6_w02_work_package_done",JsonValue.Create(false)),
        ("/m6_release_done",JsonValue.Create(false)),
        ("/publicly_published",JsonValue.Create(false))
    })p.Require.JsonValueEquals(Qualified,pointer,value);
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing staged compiled uniform-grid RED hash.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redObservation=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W02C03S03T2RedTests.UniformTimeGridAndNumericalAuditPublicContractMustExist",
        "--logger","console;verbosity=normal");
    if(redObservation.Code==0 ||
        !redObservation.Output.Contains(RedMarker,StringComparison.Ordinal) ||
        !redObservation.Output.Contains("Failed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Require one independently compiled missing uniform-grid public API RED: "+
            Relevant(redObservation.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C03-S03-T2-contract-red",
        ["status"]=RedStatus,["prior_uniform_series_qualification"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["expected_assertion"]=RedMarker,
        ["independent_compiled_red_tests"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{
    Product,Source,GreenTests,UniformConsumer,UniformConsumerSource,
    SignalConsumer,SignalConsumerSource,GeometryConsumer,GeometryConsumerSource,
    SpecialConsumer,SpecialConsumerSource,Consumer,ConsumerSource,ConsumerConfig,
    Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged uniform-grid source hash: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var fresh=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W02C03S03T2",
    "--logger","console;verbosity=normal");
MustPass(fresh,AllTargeted,"finite uniform-grid RED/GREEN");
foreach(var(filter,count,label) in new[]{
    ("M6W02C03S03T1",S03T1,"retained uniform-series T1"),
    ("M6W02C03S02T2",S02T2,"retained signal audit"),
    ("M6W02C03S02T1",S02T1,"retained direct DFT/convolution"),
    ("M6W02C03S01T2",S01T2,"retained Euclidean audit"),
    ("M6W02C03S01T1",S01T1,"retained Euclidean core"),
    ("M6W02C02T2",C02T2,"retained special-function boundary"),
    ("M6W02C02T1",C02T1,"retained special functions"),
    ("M6W02C01T2",C01T2,"retained exact boundary"),
    ("M6W02C01T1",C01T1,"retained exact integers")
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
        throw new InvalidOperationException("Retained Math regression project not passed: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w02-c03-s03-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.scientific."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Real local scientific preview.10 archive missing.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W02-C03-S03-T2-PKG-SHA256:");

static void CheckSourceFreeConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1 ||
        (string?)references[0].Attribute("Include")!="dw.tools.math.scientific" ||
        (string?)references[0].Attribute("Version")!="["+version+"]" ||
        xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Source-free client must pin one exact scientific NuGet archive: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear") ||
        sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External NuGet feed forbidden: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W02-C03-S03-T2-CONSUMER-SHA256:","m6-w02-c03-s03-t2-audit-cache"),
    (UniformConsumer,UniformConsumerConfig,UniformMarker,
        "M6-W02-C03-S03-T1-CONSUMER-SHA256:","m6-w02-c03-s03-t2-retained-uniform-cache"),
    (SignalConsumer,SignalConsumerConfig,SignalMarker,
        "M6-W02-C03-S02-T2-CONSUMER-SHA256:","m6-w02-c03-s03-t2-retained-signal-cache"),
    (GeometryConsumer,GeometryConsumerConfig,GeometryMarker,
        "M6-W02-C03-S01-T2-CONSUMER-SHA256:","m6-w02-c03-s03-t2-retained-geometry-cache"),
    (SpecialConsumer,SpecialConsumerConfig,SpecialMarker,
        "M6-W02-C02-T2-CONSUMER-SHA256:","m6-w02-c03-s03-t2-retained-special-cache")
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
        throw new InvalidOperationException("Eight-probe source-free NuGet receipt absent: "+client.Project);
    var receipt=ReadReceipt(observed,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes || receipt.Sha!=inspected.Sha)
        throw new InvalidOperationException("Scientific preview.10 SHA mismatch across independent caches.");
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
        ["schema_version"]=1,["id"]="M6-W02-C03-S03-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,["prior_uniform_series_qualification"]=Prior,
        ["structural_split_qualification"]=Split,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can caller-supplied finite time coordinates falsify irregular uniform sampling while preserving T1 differences/means under conservative binary64 time, difference and mean cancellation refusals?",
        ["observation"]="One compiled missing uniform-grid audit public API RED recovered, 106 GREEN, 594 retained scientific tests, four Math regressions and five SHA-identical eight-probe preview.10 NuGet-only consumers.",
        ["decision"]="Admit explicit monotonic finite time coordinates consistent with caller interval within 1e-10 relative tolerance, plus conservative 1e-12 timestamp/delta/mean signal risk floors without forecasting or error theorem.",
        ["source"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["retained_uniform_consumer_project"]=UniformConsumer,
        ["retained_uniform_consumer_project_sha256"]=hashes[UniformConsumer],
        ["retained_uniform_consumer_source"]=UniformConsumerSource,
        ["retained_uniform_consumer_source_sha256"]=hashes[UniformConsumerSource],
        ["retained_signal_consumer_project"]=SignalConsumer,
        ["retained_signal_consumer_project_sha256"]=hashes[SignalConsumer],
        ["retained_signal_consumer_source"]=SignalConsumerSource,
        ["retained_signal_consumer_source_sha256"]=hashes[SignalConsumerSource],
        ["retained_geometry_consumer_project"]=GeometryConsumer,
        ["retained_geometry_consumer_project_sha256"]=hashes[GeometryConsumer],
        ["retained_geometry_consumer_source"]=GeometryConsumerSource,
        ["retained_geometry_consumer_source_sha256"]=hashes[GeometryConsumerSource],
        ["retained_special_consumer_project"]=SpecialConsumer,
        ["retained_special_consumer_project_sha256"]=hashes[SpecialConsumer],
        ["retained_special_consumer_source"]=SpecialConsumerSource,
        ["retained_special_consumer_source_sha256"]=hashes[SpecialConsumerSource],
        ["uniform_audit_consumer_project"]=Consumer,
        ["uniform_audit_consumer_project_sha256"]=hashes[Consumer],
        ["uniform_audit_consumer_source"]=ConsumerSource,
        ["uniform_audit_consumer_source_sha256"]=hashes[ConsumerSource],
        ["uniform_audit_consumer_config"]=ConsumerConfig,
        ["uniform_audit_consumer_config_sha256"]=hashes[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=hashes[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
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
        ["uniform_audit_consumer_probes_passed"]=8,
        ["retained_uniform_consumer_probes_passed"]=8,
        ["retained_signal_consumer_probes_passed"]=8,
        ["retained_geometry_consumer_probes_passed"]=8,
        ["retained_special_consumer_probes_passed"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(receipts.ToArray()),
        ["local_product_version"]=Version,
        ["max_input_samples"]=32,
        ["max_abs_time_coordinate"]=1e15d,
        ["max_relative_interval_deviation"]=1e-10d,
        ["minimum_relative_time_step"]=1e-12d,
        ["minimum_relative_difference_signal"]=1e-12d,
        ["minimum_mean_signal_ratio"]=1e-12d,
        ["timing_coordinates_explicit"]=true,
        ["irregular_timing_refused"]=true,
        ["forecasting_or_resampling"]=false,
        ["direct_uniform_series_t1_unchanged"]=true,
        ["ratios_are_conservative_heuristics"]=true,
        ["certified_error_bounds"]=false,
        ["publicly_published"]=false,
        ["m6_w02_c03_s03_t2_done"]=true,
        ["m6_w02_c03_s03_phase_done"]=true,
        ["m6_w02_c03_parent_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C03-T1: independently qualify original C03 aggregate family acceptance across S01 geometry, S02 direct signals and S03 uniform-series T1/T2, retaining original aggregate T2.",
        ["nonclaims"]=p.Json.StringArray(
            "Time-coordinate units, spacing and clock origin remain caller-supplied and are not independently authenticated.",
            "Fixed timing and cancellation ratios are conservative diagnostics, not certified binary64 rounding-error bounds.",
            "No forecasting, implicit derivative, irregular resampling, interpolation, geodesic meaning, FFT or circular convolution is provided.",
            "S03 is completed but original C03 aggregate T1/T2 remain unfinished; M6-W02 and M6 are not closed.",
            "No public NuGet release, other operating system or AURA/Decision/MCDM adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        doc["plan_version"]="0.1.97";
        var phase=doc["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="in_progress" ||
            (string?)task["status"]!="planned" ||
            (string?)phase["tasks"]!.AsArray()[0]!["status"]!="done")
            throw new InvalidOperationException("Accepted S03-T2 baseline changed.");
        phase["status"]="done";
        phase["refinement"]="RS097 T1 qualified finite uniform-series adjacent differences and valid-window arithmetic means; RS098 T2 independently qualifies explicit monotonic time coordinates, irregular-time and numerical risk refusals with one compiled RED, 106 GREEN, 594 retained scientific tests, four Math regressions and five SHA-identical preview.10 package consumers. Original C03 aggregate T1/T2 and M6 remain open.";
        phase["readiness"]="S03 T1/T2 qualified for 1..32 bounded binary64 samples and explicit finite monotonic timing with conservative ratio heuristics; no forecast, resampling, inferred units or original C03 aggregate completion.";
        var priorFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,UniformConsumer,UniformConsumerSource,
            SignalConsumer,SignalConsumerSource,GeometryConsumer,GeometryConsumerSource,
            SpecialConsumer,SpecialConsumerSource,Consumer,ConsumerSource,ConsumerConfig,
            Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W02C03S03T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+UniformConsumer+" -c Release",
            "dotnet run --project "+SignalConsumer+" -c Release",
            "dotnet run --project "+GeometryConsumer+" -c Release",
            "dotnet run --project "+SpecialConsumer+" -c Release");
        var priorEvidence=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(priorEvidence.Concat(new[]{RedProof,Qualified})
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
    foreach(var id in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,"done");
foreach(var id in new[]{"M6-W02-C03","M6-W02-C04","M6-W02-C05"})
    p.ProjectPlan.RequireNodeState(id,"not-ready");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS098" ||
        (string?)state["activeMilestone"]!=Milestone ||
        (string?)state["activeWorkPackage"]!=Work ||
        (string?)state["activePhase"]!=(target?null:Phase) ||
        state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected RS098 pre/post native cursor.");
    state["activePhase"]=null;
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
        throw new InvalidOperationException("Expected "+expected+" passing "+label+" tests: "+Relevant(output));
}
static bool HasExactLine(string output,string marker)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(x=>x.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string version="1.1.0-preview.10";
    var prefix=marker+"dw.tools.math.scientific:"+version+":";
    var rows=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(rows.Length!=1)
        throw new InvalidOperationException("Require one actual local preview.10 NuGet SHA receipt.");
    var parts=rows[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
        !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes) ||
        bytes<=0 || parts[1].Length!=64 ||
        parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed actual preview.10 package length and SHA receipt.");
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
        throw new TimeoutException("Direct signal boundary qualification timed out.");
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
