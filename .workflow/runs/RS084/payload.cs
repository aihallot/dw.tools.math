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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C02",Task="M6-W01-C02-T2";
const string Red="M6-W01-C02-T2-R",Green="M6-W01-C02-T2-G",Verify="M6-W01-C02-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-C02-T1-qualified.json";
const string RedProof="docs/planning/evidence/M6-W01-C02-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-univariate-residual-and-regularity-audit-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.4";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Source="src/projects/dw.tools.math.analysis/UnivariateBoundaryAudit64.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C02T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C02T2Tests.cs";
const string OriginalConsumer="tests/consumers/m6-w01-c02-t1/m6-univariate-consumer.csproj";
const string OriginalSource="tests/consumers/m6-w01-c02-t1/Program.cs";
const string OriginalConfig="tests/consumers/m6-w01-c02-t1/NuGet.Config";
const string Consumer="tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w01-c02-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c02-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c02-t2-audit-boundaries.md";
const string Recipe="docs/distribution/m6-univariate-audit-boundaries.md";
const string PackageHelper=".workflow/runs/RS084/staged/checks/m6-w01-c02-t2-package-sha.cs";
const string RedMarker="M6-W01-C02-T2 RED: the public residual and regularity audit API is absent.";
const string ConsumerMarker="dw.tools.math/m6-univariate-boundary-consumer/1.1 qualified; probes=8";
const string OriginalMarker="dw.tools.math/m6-univariate-consumer/1.1 qualified; probes=8";
const int IndependentGreen=50,AllTargeted=51,PreviousRootTests=44,C01T1Tests=48,C01T2Tests=60;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS084",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS083",1,
    "feat: add bounded univariate root and Simpson integration contracts",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-root-bisection-and-simpson-local-nuget-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(43));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(PreviousRootTests));
p.Require.JsonValueEquals(Prior,"/retained_t1_tests_passed",JsonValue.Create(C01T1Tests));
p.Require.JsonValueEquals(Prior,"/retained_t2_tests_passed",JsonValue.Create(C01T2Tests));
p.Require.JsonValueEquals(Prior,"/independent_source_free_nuget_probes",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.1.0-preview.3"));
p.Require.JsonValueEquals(Prior,"/m6_w01_c02_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w01_c02_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.82"),JsonValue.Create("0.1.83"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.83":"0.1.82"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/13/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/14/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var childIndex in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/41/tasks/1/subtasks/{childIndex}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/43/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.4</Version>":
    "<Version>1.1.0-preview.3</Version>");
p.Require.TextContains(OriginalConsumer,target?"Version=\"[1.1.0-preview.4]\"":
    "Version=\"[1.1.0-preview.3]\"");
p.Require.TextContains(OriginalSource,target?"Version=\"1.1.0-preview.4\"":
    "Version=\"1.1.0-preview.3\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C02-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C03","not-ready");
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
        JsonValue.Create("compiled-univariate-audit-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/retained_c02_t1_tests_passed",
        JsonValue.Create(PreviousRootTests));
    p.Require.JsonValueEquals(Qualified,"/retained_c01_t1_tests_passed",
        JsonValue.Create(C01T1Tests));
    p.Require.JsonValueEquals(Qualified,"/retained_c01_t2_tests_passed",
        JsonValue.Create(C01T2Tests));
    p.Require.JsonValueEquals(Qualified,"/independent_audit_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/retained_t1_consumer_probes",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/local_product_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c02_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c02_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_release_done",JsonValue.Create(false));
}
var identities=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
identities[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing compiled audit RED staged source identity.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redObservation=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C02T2RedTests.ExplicitResidualAndRegularityAuditPublicApiMustExist",
        "--logger","console;verbosity=normal");
    if(redObservation.Code==0 ||
       !redObservation.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One actually compiled missing-public-audit-API RED is required: "+
            Relevant(redObservation.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C02-T2-contract-red",
        ["status"]="compiled-univariate-audit-public-api-red-observed",
        ["prior_qualified_t1"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=identities[RedTest],
        ["expected_red_assertion"]=RedMarker,
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
    identities[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged RS084 source SHA-256: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C02T2",
    "--logger","console;verbosity=normal");
MustPass(targeted,AllTargeted,"new residual/regularity audit RED-recovered and GREEN");
var retainedRoot=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C02T1",
    "--logger","console;verbosity=normal");
MustPass(retainedRoot,PreviousRootTests,"retained C02 T1 numerical root and quadrature");
var retainedC01T1=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C01T1",
    "--logger","console;verbosity=normal");
MustPass(retainedC01T1,C01T1Tests,"retained C01 T1 curve");
var retainedC01T2=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M6W01C01T2",
    "--logger","console;verbosity=normal");
MustPass(retainedC01T2,C01T2Tests,"retained C01 T2 curve boundary");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Existing relevant Math regression refused: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w01-c02-t2-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release",
    "--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Exact preview.4 local Math package missing.");
var packageOutput=Required(root,"dotnet","run","--file",PackageHelper,"--",archive);
var inspected=ReadReceipt(packageOutput,"M6-W01-C02-T2-PKG-SHA256:");

static void CheckLocalConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var packages=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(packages.Length!=1 ||
       (string?)packages[0].Attribute("Include")!="dw.tools.math.analysis" ||
       (string?)packages[0].Attribute("Version")!="["+version+"]" ||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Expected one pinned, source-free Math NuGet package: "+project);
    var sourceList=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sourceList.Elements().Any(x=>x.Name.LocalName=="clear")||
       sourceList.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("Unqualified external NuGet feed forbidden: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (Consumer,ConsumerConfig,ConsumerMarker,
        "M6-W01-C02-T2-CONSUMER-SHA256:","m6-w01-c02-t2-consumer-cache"),
    (OriginalConsumer,OriginalConfig,OriginalMarker,
        "M6-W01-C02-T1-CONSUMER-SHA256:","m6-w01-c02-t2-retained-cache")
};
var clientReceipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckLocalConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasLine(output,client.Marker))
        throw new InvalidOperationException("Eight source-free numerical science oracles absent: "+client.Project);
    var receipt=ReadReceipt(output,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes||
       !string.Equals(receipt.Sha,inspected.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("NuGet bytes/SHA mismatch across isolated consumers: "+client.Project);
    clientReceipts.Add(new JsonObject
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
        ["schema_version"]=1,["id"]="M6-W01-C02-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,["prior_t1_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can bounded root and Simpson results refuse high residuals and undeclared function regularity while preserving previously qualified APIs?",
        ["observation"]="One compiled missing-public-API RED recovered, 50 independent GREENs, 44 prior C02 T1, 48 C01 T1 and 60 C01 T2 tests, two Math regression suites, real preview.4 archive, two eight-probe NuGet clients and identical SHA.",
        ["decision"]="Admit residual-qualified root observations only when the observed residual meets caller threshold; require explicit unverified smoothness declaration for Simpson observations. Never certify a root-error or global integral-error theorem.",
        ["source"]=Source,["source_sha256"]=identities[Source],
        ["product_project"]=Product,["product_project_sha256"]=identities[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=identities[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=identities[GreenTests],
        ["retained_consumer_project"]=OriginalConsumer,
        ["retained_consumer_project_sha256"]=identities[OriginalConsumer],
        ["retained_consumer_source"]=OriginalSource,
        ["retained_consumer_source_sha256"]=identities[OriginalSource],
        ["boundary_consumer_project"]=Consumer,
        ["boundary_consumer_project_sha256"]=identities[Consumer],
        ["boundary_consumer_source"]=ConsumerSource,
        ["boundary_consumer_source_sha256"]=identities[ConsumerSource],
        ["boundary_consumer_config"]=ConsumerConfig,
        ["boundary_consumer_config_sha256"]=identities[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=identities[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=identities[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_c02_t1_tests_passed"]=PreviousRootTests,
        ["retained_c01_t1_tests_passed"]=C01T1Tests,
        ["retained_c01_t2_tests_passed"]=C01T2Tests,
        ["regression_projects_passed"]=2,
        ["independent_audit_consumer_probes"]=8,
        ["retained_t1_consumer_probes"]=8,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["isolated_consumer_receipts"]=p.Json.Array(clientReceipts.ToArray()),
        ["local_product_version"]=Version,
        ["accepted_sqrt_two_root"]=global::System.Math.Sqrt(2d),
        ["accepted_square_integral"]=1d/3d,
        ["sign_jump_high_residual_refused"]=true,
        ["width_only_convergence_refused_by_audit"]=true,
        ["undeclared_quadrature_regularity_refused"]=true,
        ["smoothness_independently_proved"]=false,
        ["root_error_bound_certified"]=false,
        ["simpson_error_bound_certified"]=false,
        ["publicly_published"]=false,
        ["m6_w01_c02_t2_done"]=true,
        ["m6_w01_c02_phase_done"]=true,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W01-C03-T1: bounded numerical derivative and small ODE integration with explicit step and error policies",
        ["nonclaims"]=p.Json.StringArray(
            "No finite sampling proves arbitrary caller continuity, smoothness, uniqueness or derivative regularity.",
            "Residual acceptance is not a mathematical root enclosure or rigorous absolute root error bound.",
            "A declared smoothness assumption and Simpson refinement estimate do not imply a global error guarantee.",
            "No public package, native provider, untested OS or Decision/MCDM/AURA adoption is qualified.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.83";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)phase["status"]!="in_progress"||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("Accepted M6 C02 T2 backlog baseline changed.");
        phase["status"]="done";
        phase["refinement"]="RS083 T1 admitted provider-free bounded bisection and composite Simpson with noncertified error diagnostics, 43 GREEN tests and one compiled RED. RS084 T2 independently adds residual-only root admission and explicit unverified Simpson smoothness declaration, with 50 GREEN and recovered RED, 152 retained focused tests, two Math regressions and 16 source-free NuGet probes from a SHA-qualified preview.4 archive. The entire C02 phase closes; M6-W01 and M6 remain open.";
        phase["readiness"]="C02 complete: T1 bounded numerical estimates and T2 typed, fail-closed residual and regularity audits independently qualified on local NuGet preview.4; C03 numerical differentiation and ODE planned.";
        var originalFiles=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(originalFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,OriginalConsumer,OriginalSource,
            Consumer,ConsumerSource,ConsumerConfig,Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C02T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release",
            "dotnet run --project "+OriginalConsumer+" -c Release");
        var existing=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(existing.Concat(new[]{RedProof,Qualified})
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
p.ProjectPlan.RequireNodeState(Phase,"done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS084"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("M6 C02 T2 native DWF cursor diverged.");
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
        throw new InvalidOperationException("Expected "+expected+" passing "+label+" tests: "+Relevant(log));
}
static bool HasLine(string output,string marker) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.analysis",version="1.1.0-preview.4";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)
        throw new InvalidOperationException("One independently observed package receipt required: "+marker);
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid NuGet actual package bytes/SHA-256.");
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
    using var process=new Process{StartInfo=new ProcessStartInfo{
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
        throw new TimeoutException(executable+" timed out during bounded Math run.");
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
