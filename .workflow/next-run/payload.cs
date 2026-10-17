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

const string Milestone="M6",Work="M6-W01",Phase="M6-W01-C02",Task="M6-W01-C02-T1";
const string Red="M6-W01-C02-T1-R",Green="M6-W01-C02-T1-G",Verify="M6-W01-C02-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M6-W01-C02-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M6-W01-C02-T1-qualified.json";
const string QualifiedStatus="bounded-root-bisection-and-simpson-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.3";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Source="src/projects/dw.tools.math.analysis/BoundedUnivariateAnalysis64.cs";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.analysis.tests/M6W01C02T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.analysis.tests/M6W01C02T1Tests.cs";
const string Consumer="tests/consumers/m6-w01-c02-t1/m6-univariate-consumer.csproj";
const string ConsumerSource="tests/consumers/m6-w01-c02-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m6-w01-c02-t1/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-c02-t1-bounded-univariate.md";
const string Recipe="docs/distribution/m6-bounded-univariate.md";
const string PackageHelper=".workflow/runs/RS083/staged/checks/m6-w01-c02-t1-package-sha.cs";
const string RedMarker="M6-W01-C02-T1 RED: bounded public root and quadrature API is absent.";
const string ConsumerMarker="dw.tools.math/m6-univariate-consumer/1.1 qualified; probes=8";
const int IndependentGreen=43,AllTargeted=44,RetainedC01T1=48,RetainedC01T2=60;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS083",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS082",1,
    "feat: qualify Math curve unit conditioning boundaries and close M6 W01 C01",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-curve-explicit-unit-conditioning-extrapolation-refusals-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(59));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(60));
p.Require.JsonValueEquals(Prior,"/retained_t1_tests_passed",JsonValue.Create(RetainedC01T1));
p.Require.JsonValueEquals(Prior,"/m6_w01_c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",JsonValue.Create("0.1.81"),JsonValue.Create("0.1.82"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.82":"0.1.81"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/13/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/40/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/status",JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/tasks/0/status",JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/41/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/41/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/42/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?"<Version>1.1.0-preview.3</Version>":
    "<Version>1.1.0-preview.2</Version>");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C03","not-ready");
if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Consumer,ConsumerSource,
        ConsumerConfig,Decision,Recipe,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-bounded-univariate-public-contract-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/retained_t1_tests_passed",JsonValue.Create(RetainedC01T1));
    p.Require.JsonValueEquals(Qualified,"/retained_t2_tests_passed",JsonValue.Create(RetainedC01T2));
    p.Require.JsonValueEquals(Qualified,"/local_product_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c02_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_c02_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m6_w01_work_package_done",JsonValue.Create(false));
}
var identities=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
identities[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing compiled univariate RED identity.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M6W01C02T1RedTests.BoundedRootAndQuadraturePublicContractMustExist",
        "--logger","console;verbosity=normal");
    if(observed.Code==0 || !observed.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("One genuinely compiled missing public univariate API RED required: "+
            Relevant(observed.Output));
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C02-T1-contract-red",
        ["status"]="compiled-bounded-univariate-public-contract-red-observed",
        ["prior_qualified_c01"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=identities[RedTest],
        ["red_assertion"]=RedMarker,["compiled_red_tests"]=1,
        ["compiler_error_is_not_a_red_substitute"]=true
    };
    p.Files.WriteComplete(RedProof,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var path in new[]{Product,Source,GreenTests,Consumer,ConsumerSource,
    ConsumerConfig,Decision,Recipe})
{
    var changed=p.Files.ReplaceFromStaged("staged/"+path,path);
    identities[path]=changed.AfterSha256 ??
        throw new InvalidOperationException("Missing bounded numerical staged SHA: "+path);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W01C02T1","--logger","console;verbosity=normal");
MustPass(targeted,AllTargeted,"new bounded root/quadrature RED-recovered and GREEN tests");
var previousT1=Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W01C01T1","--logger","console;verbosity=normal");
MustPass(previousT1,RetainedC01T1,"retained curve-analysis T1");
var previousT2=Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M6W01C01T2","--logger","console;verbosity=normal");
MustPass(previousT2,RetainedC01T2,"retained curve-boundary T2");
Required(root,"dotnet","test",Test,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var observed=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!observed.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Math numerical regression refused: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
var feed=Path.Combine(root,"build","artifacts","m6-w01-c02-t1-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Expected real isolated preview.3 archive not generated.");
var inspection=ReadReceipt(Required(root,"dotnet","run","--file",PackageHelper,"--",archive),
    "M6-W01-C02-T1-PKG-SHA256:");
var xml=XDocument.Load(Path.Combine(root,Consumer));
var refs=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(refs.Length!=1||
   (string?)refs[0].Attribute("Include")!="dw.tools.math.analysis"||
   (string?)refs[0].Attribute("Version")!="["+Version+"]"||
   xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("NuGet-only consumer is not source-free and exactly pinned.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
   sources.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("NuGet client admits an unqualified external feed.");
var cache=Path.Combine(root,"build","artifacts","m6-w01-c02-t1-consumer-cache");
Required(root,"dotnet","restore",Consumer,"--source",feed,"--configfile",ConsumerConfig,"--packages",cache);
Required(root,"dotnet","build",Consumer,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",Consumer,
    "-c","Release","--no-build","--no-restore","--",archive);
if(!HasLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight independent NuGet-only univariate science probes absent.");
var consumerReceipt=ReadReceipt(consumerOutput,"M6-W01-C02-T1-CONSUMER-SHA256:");
if(consumerReceipt.Bytes!=inspection.Bytes||consumerReceipt.Sha!=inspection.Sha)
    throw new InvalidOperationException("Source-free NuGet consumer inspected different archive bytes.");
if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-C02-T1-qualified",
        ["status"]=QualifiedStatus,["prior_c01_qualification"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["observation"]="Compiled RED recovered, 43 independent GREEN tests, 48 T1 and 60 T2 retained, two Math regression suites, one real preview.3 package, independent eight-probe NuGet-only consumer and equal package bytes.",
        ["decision"]="Admit finite bracketed bisection and composite Simpson with observable residual, bracket width and noncertified refinement estimate; no theorem of continuity or rigorous error bound.",
        ["source"]=Source,["source_sha256"]=identities[Source],
        ["product_project"]=Product,["product_project_sha256"]=identities[Product],
        ["red_test"]=RedTest,["red_test_sha256"]=identities[RedTest],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=identities[GreenTests],
        ["consumer_project"]=Consumer,["consumer_project_sha256"]=identities[Consumer],
        ["consumer_source"]=ConsumerSource,["consumer_source_sha256"]=identities[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,["consumer_config_sha256"]=identities[ConsumerConfig],
        ["decision_document"]=Decision,["decision_document_sha256"]=identities[Decision],
        ["distribution_document"]=Recipe,["distribution_document_sha256"]=identities[Recipe],
        ["compiled_red_observed"]=true,
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["retained_t1_tests_passed"]=RetainedC01T1,
        ["retained_t2_tests_passed"]=RetainedC01T2,
        ["regression_projects_passed"]=2,
        ["independent_source_free_nuget_probes"]=8,
        ["inspected_package_bytes"]=inspection.Bytes,
        ["inspected_package_sha256"]=inspection.Sha,
        ["consumer_package_bytes"]=consumerReceipt.Bytes,
        ["consumer_package_sha256"]=consumerReceipt.Sha,
        ["local_product_version"]=Version,
        ["accepted_sqrt_two_root"]=global::System.Math.Sqrt(2d),
        ["accepted_square_integral"]=1d/3d,
        ["root_error_is_proven_bound"]=false,
        ["simpson_error_estimate_is_proven_bound"]=false,
        ["cancellation_supported"]=true,
        ["publicly_published"]=false,
        ["m6_w01_c02_t1_done"]=true,
        ["m6_w01_c02_phase_done"]=false,
        ["m6_w01_work_package_done"]=false,
        ["m6_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_task"]="M6-W01-C02-T2: falsify continuity, false tolerance guarantees and budget boundaries; do not close C02 from T1 alone.",
        ["nonclaims"]=p.Json.StringArray(
            "A sign-changing finite endpoint pair does not certify continuity, uniqueness or a root error bound.",
            "Composite Simpson Richardson-like difference is a heuristic for smooth functions, not a rigorous global bound.",
            "No public stable package, foreign provider, untested operating system or downstream agent adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.82";
        var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="planned"||(string?)task["status"]!="planned")
            throw new InvalidOperationException("C02 T1 baseline drifted.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS083 T1: one compiled API RED, 43 independently asserted GREENs, retained 48 curve T1 and 60 T2 tests, two Math regression suites and source-free preview.3 eight-probe NuGet client. Numeric endpoints, finite bounded bisection, observed root residual/bracket diagnostics and Simpson heuristic error estimates qualified; false certified-error and discontinuity claims refused. C02 T2 remains planned.";
        chunk["readiness"]="T1 delivered finite bounded bisection and Simpson diagnostics; T2 must separately falsify discontinuity, tolerance and budget limits before C02 may close.";
        var originalFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(originalFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,Consumer,ConsumerSource,ConsumerConfig,
            Decision,Recipe,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M6W01C02T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+Consumer+" -c Release");
        var oldEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{RedProof,Qualified})
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
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M6-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M6-W01-C03","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS083"||
       (string?)state["activeMilestone"]!=Milestone||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?Phase:null)||
       state["activeTask"] is not null||state["activeSubtask"] is not null)
        throw new InvalidOperationException("C02 phase cursor diverged from baseline/target.");
    state["activePhase"]=Phase;
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
        throw new InvalidOperationException("Expected "+expected+" passing "+label+" tests: "+
            Relevant(log));
}
static bool HasLine(string output,string marker)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.analysis",version="1.1.0-preview.3";
    var prefix=marker+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)
        throw new InvalidOperationException("Exactly one versioned local NuGet SHA receipt required.");
    var parts=lines[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0||parts[1].Length!=64||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Malformed actual local NuGet SHA-256 receipt.");
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
        throw new TimeoutException(executable+" timed out in bounded Math scientific validation.");
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
