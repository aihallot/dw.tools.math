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

const string Milestone="M6",Work="M6-W01";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string PriorC01="docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json";
const string PriorC02="docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json";
const string PriorC03="docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json";
const string Gate="docs/planning/evidence/M6-W01-local-gate-qualified.json";
const string GateStatus="m6-w01-all-three-numerical-families-local-nuget-qualified";
const string Solution="dw.tools.math.slnx",Version="1.1.0-preview.6";
const string Product="src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
const string Test="tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj";
const string C01="tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj";
const string C01Source="tests/consumers/m6-w01-c01-t2/Program.cs";
const string C01Config="tests/consumers/m6-w01-c01-t2/NuGet.Config";
const string C02="tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj";
const string C02Source="tests/consumers/m6-w01-c02-t2/Program.cs";
const string C02Config="tests/consumers/m6-w01-c02-t2/NuGet.Config";
const string C03="tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj";
const string C03Source="tests/consumers/m6-w01-c03-t2/Program.cs";
const string C03Config="tests/consumers/m6-w01-c03-t2/NuGet.Config";
const string Decision="docs/planning/decisions/m6-w01-local-numerical-gate.md";
const string Recipe="docs/distribution/m6-w01-local-numerical-gate.md";
const string Helper=".workflow/runs/RS087/staged/checks/m6-w01-gate-package-sha.cs";
const string C01Marker="dw.tools.math/m6-curve-boundary-consumer/1.1 qualified; probes=8";
const string C02Marker="dw.tools.math/m6-univariate-boundary-consumer/1.1 qualified; probes=8";
const string C03Marker="dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes=8";
const int C01T1=48,C01T2=60,C02T1=44,C02T2=51,C03T1=58,C03T2=67,AllFocused=328;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Gate));
p.Workflow.RequireCurrentRun("RS087",Milestone,target?null:Work,null,null,null);
p.Workflow.RequirePriorSuccess("RS086",1,
    "feat: qualify derivative and ODE regime boundaries and close M6 W01 C03",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(PriorC01,"/status",
    JsonValue.Create("bounded-curve-explicit-unit-conditioning-extrapolation-refusals-qualified"));
p.Require.JsonValueEquals(PriorC01,"/m6_w01_c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PriorC01,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorC02,"/status",
    JsonValue.Create("bounded-univariate-residual-and-regularity-audit-qualified"));
p.Require.JsonValueEquals(PriorC02,"/m6_w01_c02_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PriorC02,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorC03,"/status",
    JsonValue.Create("bounded-differential-declared-regularity-and-ode-regime-audit-qualified"));
p.Require.JsonValueEquals(PriorC03,"/independent_green_tests_passed",JsonValue.Create(66));
p.Require.JsonValueEquals(PriorC03,"/red_recovered_and_green_tests_passed",JsonValue.Create(C03T2));
p.Require.JsonValueEquals(PriorC03,"/local_product_version",JsonValue.Create(Version));
p.Require.JsonValueEquals(PriorC03,"/m6_w01_c03_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(PriorC03,"/m6_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorC03,"/publicly_published",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.85"),JsonValue.Create("0.1.86"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.86":"0.1.85"));
foreach(var (pointer,value) in new (string,string)[]
{
    ("/releases/5/status","done"),("/releases/6/status","in_progress"),
    ("/releases/7/status","planned"),
    ("/work_packages/13/status",target?"done":"in_progress"),
    ("/work_packages/14/status","planned"),
    ("/chunks/40/status","done"),("/chunks/41/status","done"),
    ("/chunks/42/status","done"),("/chunks/43/status","planned")
})p.Require.JsonValueEquals(Backlog,pointer,JsonValue.Create(value));
p.Require.TextContains(Product,"<Version>1.1.0-preview.6</Version>");
p.Require.TextContains(C01,target?"Version=\"[1.1.0-preview.6]\"":
    "Version=\"[1.1.0-preview.2]\"");
p.Require.TextContains(C01Source,target?"Version=\"1.1.0-preview.6\"":
    "Version=\"1.1.0-preview.2\"");
p.Require.TextContains(C02,target?"Version=\"[1.1.0-preview.6]\"":
    "Version=\"[1.1.0-preview.5]\"");
p.Require.TextContains(C02Source,target?"Version=\"1.1.0-preview.6\"":
    "Version=\"1.1.0-preview.5\"");
p.Require.TextContains(C03,"Version=\"[1.1.0-preview.6]\"");
p.Require.TextContains(C03Source,"Version=\"1.1.0-preview.6\"");

p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
foreach(var phase in new[]{"M6-W01-C01","M6-W01-C02","M6-W01-C03"})
    p.ProjectPlan.RequireNodeState(phase,"done");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
if(!target)
{
    foreach(var file in new[]{Decision,Recipe,Gate})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Gate,"/status",JsonValue.Create(GateStatus));
    p.Require.JsonValueEquals(Gate,"/prior_family_phases_completed",JsonValue.Create(3));
    p.Require.JsonValueEquals(Gate,"/retained_targeted_tests_passed",JsonValue.Create(AllFocused));
    p.Require.JsonValueEquals(Gate,"/independent_consumer_probes_passed",JsonValue.Create(24));
    p.Require.JsonValueEquals(Gate,"/local_product_version",JsonValue.Create(Version));
    p.Require.JsonValueEquals(Gate,"/m6_w01_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Gate,"/m6_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{C01Source,C01,C02Source,C02,C03Source,Decision,Recipe})
{
    var staged=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=staged.AfterSha256 ??
        throw new InvalidOperationException("Missing W01 gate staged artifact SHA: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
foreach(var (filter,count) in new[]{
    ("M6W01C01T1",C01T1),("M6W01C01T2",C01T2),
    ("M6W01C02T1",C02T1),("M6W01C02T2",C02T2),
    ("M6W01C03T1",C03T1),("M6W01C03T2",C03T2)})
{
    var results=Required(root,"dotnet","test",Test,"-c","Release","--no-build",
        "--no-restore","--filter","FullyQualifiedName~"+filter,
        "--logger","console;verbosity=normal");
    MustPass(results,count,filter);
}
Required(root,"dotnet","test",Test,"-c","Release","--no-build",
    "--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"})
{
    var regression=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regression.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Cross-family Math regression failed: "+regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m6-w01-gate-local-feed");
Required(root,"dotnet","pack",Product,"-c","Release","--no-build","--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.analysis."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("Missing actual preview.6 numerical-analysis local NuGet archive.");
var inspected=ReadReceipt(
    Required(root,"dotnet","run","--file",Helper,"--",archive),
    "M6-W01-GATE-PKG-SHA256:");

static void CheckConsumer(string root,string project,string config,string version)
{
    var xml=XDocument.Load(Path.Combine(root,project));
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1 ||
       (string?)references[0].Attribute("Include")!="dw.tools.math.analysis" ||
       (string?)references[0].Attribute("Version")!="["+version+"]" ||
       xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("Math W01 consumer must pin one source-free package: "+project);
    var sources=XDocument.Load(Path.Combine(root,config))
        .Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear") ||
       sources.Elements().Any(x=>x.Name.LocalName=="add"))
        throw new InvalidOperationException("External NuGet sources not permitted: "+config);
}
var clients=new (string Project,string Config,string Marker,string Receipt,string Cache)[]
{
    (C01,C01Config,C01Marker,"M6-W01-C01-T2-CONSUMER-SHA256:","m6-w01-gate-c01-cache"),
    (C02,C02Config,C02Marker,"M6-W01-C02-T2-CONSUMER-SHA256:","m6-w01-gate-c02-cache"),
    (C03,C03Config,C03Marker,"M6-W01-C03-T2-CONSUMER-SHA256:","m6-w01-gate-c03-cache")
};
var consumerReceipts=new List<JsonNode>();
foreach(var client in clients)
{
    CheckConsumer(root,client.Project,client.Config,Version);
    var cache=Path.Combine(root,"build","artifacts",client.Cache);
    Required(root,"dotnet","restore",client.Project,"--source",feed,
        "--configfile",client.Config,"--packages",cache);
    Required(root,"dotnet","build",client.Project,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",client.Project,
        "-c","Release","--no-build","--no-restore","--",archive);
    if(!HasLine(output,client.Marker))
        throw new InvalidOperationException("Eight independent NuGet scientific probes absent: "+client.Project);
    var receipt=ReadReceipt(output,client.Receipt);
    if(receipt.Bytes!=inspected.Bytes ||
       !string.Equals(receipt.Sha,inspected.Sha,StringComparison.Ordinal))
        throw new InvalidOperationException("Actual NuGet bytes or SHA differ across Math consumers: "+client.Project);
    consumerReceipts.Add(new JsonObject
    {
        ["project"]=client.Project,
        ["marker"]=client.Marker,
        ["package_sha256"]=receipt.Sha,
        ["package_bytes"]=receipt.Bytes,
        ["independent_probes"]=8,
        ["project_references"]=0
    });
}
if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W01-local-gate-qualified",
        ["status"]=GateStatus,
        ["question"]="Can all three qualified bounded numerical Math families be independently consumed from exactly one actual local BCL-only preview.6 archive?",
        ["observation"]="All six targeted T1/T2 filters (328 tests), the complete analysis suite and four relevant regressions pass; three distinct eight-probe NuGet-only consumers inspect the same SHA-256 and archive bytes.",
        ["decision"]="Close M6-W01 only, preserving declared finite-domain numerical limitations, noncertified errors and excluded physical-unit, inference, stiffness/event/PDE and public release claims.",
        ["prior_family_phase_evidence"]=p.Json.StringArray(PriorC01,PriorC02,PriorC03),
        ["prior_family_phases_completed"]=3,
        ["retained_targeted_tests_passed"]=AllFocused,
        ["targeted_filter_count"]=6,
        ["full_analysis_test_project_passed"]=true,
        ["relevant_regression_projects_passed"]=4,
        ["independent_consumer_probes_passed"]=24,
        ["source_free_consumer_count"]=3,
        ["actual_inspected_package_bytes"]=inspected.Bytes,
        ["actual_inspected_package_sha256"]=inspected.Sha,
        ["consumer_receipts"]=p.Json.Array(consumerReceipts.ToArray()),
        ["consumer_c01_project"]=C01,
        ["consumer_c01_source_sha256"]=hashes[C01Source],
        ["consumer_c01_project_sha256"]=hashes[C01],
        ["consumer_c02_project"]=C02,
        ["consumer_c02_source_sha256"]=hashes[C02Source],
        ["consumer_c02_project_sha256"]=hashes[C02],
        ["consumer_c03_project"]=C03,
        ["consumer_c03_source_sha256"]=hashes[C03Source],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Recipe,
        ["distribution_document_sha256"]=hashes[Recipe],
        ["local_product_version"]=Version,
        ["source_project"]=Product,
        ["local_nuget_only"]=true,
        ["publicly_published"]=false,
        ["m6_w01_work_package_done"]=true,
        ["m6_release_done"]=false,
        ["m6_w02_work_package_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="M6-W02-C01-T1: bounded exact integer, combinatorial and sequence primitives with finite budgets",
        ["nonclaims"]=p.Json.StringArray(
            "Caller unit metadata does not imply automatic physical conversion or forecast/causal inference.",
            "Root and integration estimates lack universal continuity, uniqueness and rigorous global-error certification.",
            "Numerical derivatives/RK4 do not establish smoothness, stability, no-events or guaranteed local/global error.",
            "Stiff IVPs, event localization and PDE stay excluded.",
            "M6 remains in progress; no public NuGet, foreign provider, untested operating system or downstream integration is qualified.")
    };
    p.Files.WriteComplete(Gate,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,doc=>
    {
        doc["plan_version"]="0.1.86";
        var work=doc["work_packages"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Work);
        if((string?)work["status"]!="in_progress")
            throw new InvalidOperationException("W01 local gate baseline drifted.");
        var children=doc["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Where(x=>(string?)x["work_package"]==Work).ToArray();
        if(children.Length!=3 ||
           children.Any(c=>(string?)c["status"]!="done"))
            throw new InvalidOperationException("All accepted W01 phase nodes must be complete before work-package closure.");
        work["status"]="done";
        work["evidence"]=p.Json.StringArray(PriorC01,PriorC02,PriorC03,Gate);
    });
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"done");
foreach(var child in new[]{"M6-W01-C01","M6-W01-C02","M6-W01-C03"})
    p.ProjectPlan.RequireNodeState(child,"done");
p.ProjectPlan.RequireNodeState("M6-W02","not-ready");
p.Json.EditObject(State,doc=>
{
    if((string?)doc["pendingRun"]!="RS087"||
       (string?)doc["activeMilestone"]!=Milestone||
       (string?)doc["activeWorkPackage"]!=(target?null:Work)||
       doc["activePhase"] is not null||
       doc["activeTask"] is not null||
       doc["activeSubtask"] is not null)
        throw new InvalidOperationException("W01 gate native workflow cursor diverged.");
    doc["activeWorkPackage"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void MustPass(string text,int expected,string group)
{
    if(!text.Contains("Total tests: "+expected,StringComparison.OrdinalIgnoreCase)||
       !text.Contains("Passed: "+expected,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected "+expected+" passing "+group+" tests: "+Relevant(text));
}
static bool HasLine(string output,string line)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(x=>x.Trim()==line);
static (long Bytes,string Sha) ReadReceipt(string output,string marker)
{
    const string id="dw.tools.math.analysis",version="1.1.0-preview.6";
    var prefix=marker+id+":"+version+":";
    var receipts=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(receipts.Length!=1)
        throw new InvalidOperationException("Exactly one local numerical NuGet receipt required: "+marker);
    var parts=receipts[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
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
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" exceeded the finite numerical work-package gate timeout.");
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
