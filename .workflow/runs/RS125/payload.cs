using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string PriorReport=".aura/workflow/reports/RS124/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Decision="docs/planning/decisions/post-m7-source-free-consumer-qualification.md";
const string Matrix="docs/distribution/post-m7-source-free-consumer-matrix.md";
const string Validator="docs/planning/ValidatePostM7ConsumerIsolation.cs";
const string Evidence="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
const string Status="rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished";
const string Counts="MATH-RS125:PACKAGES=2;SOURCE-FREE-CONSUMERS=2;FRESH-PROBES=16;NATIVE=541";
const string Scope="MATH-RS125:SCOPE=NO-PROGRESS;OFFLINE-LOCAL-FEEDS=true;PUBLICATION=false;AGENT-DISCOVERY=false";
const string Marker="dw.tools.math/rs125-source-free-nuget/1.1 qualified; consumers=2; probes=16; current-local-packages=true; external=false";
const string PackageCache="build/artifacts/rs125-isolated-packages";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS125",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS124",1,
    "docs: qualify post-M7 local delivery inventory without publication",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReceipt,"/status",
    JsonValue.Create("rs124-current-source-build-test-and-three-local-packages-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/source_projects",JsonValue.Create(13));
p.Require.JsonValueEquals(PriorReceipt,"/test_projects",JsonValue.Create(11));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_local_package_archives",JsonValue.Create(3));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_source_free_consumer_rerun",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(1));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/preparationFailureCount",JsonValue.Create(1));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(false));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
foreach(var milestone in new[]{"M5","M6","M7"})
    p.ProjectPlan.RequireNodeState(milestone,"done");
if(!target)
{
    foreach(var path in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (key,number) in new (string,int)[]
    {
        ("/source_free_consumers",2),("/fresh_consumer_probes",16),
        ("/fresh_packages_built",2),("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,key,JsonValue.Create(number));
    foreach(var (key,state) in new (string,bool)[]
    {
        ("/offline_local_feeds_only",true),
        ("/isolated_nuget_package_cache",true),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/discovery_skill_generated",false),
        ("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),
        ("/all_historical_consumers_rerun",false)
    })
        p.Require.JsonValueEquals(Evidence,key,JsonValue.Create(state));
}
var decisionChange=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixChange=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorChange=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS125 decision source SHA.");
var matrixSha=matrixChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS125 matrix source SHA.");
var validatorSha=validatorChange.AfterSha256 ??
    throw new InvalidOperationException("Missing RS125 consumer oracle source SHA.");
foreach(var term in new[]{"## Question and authority","## Pinned baseline",
    "## Exact controlled execution","## Acceptance and nonclaims"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Source-to-package-to-consumer table",
    "## Historical versus fresh evidence","## Receipt and release boundary"})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{Marker,Counts,Scope})
    p.Require.TextContains(Validator,term);
var original=Required(root,"baseline-isolated-consumer-check","dotnet",false,
    "run","--file",Validator,"--","--check");
foreach(var term in new[]{Marker,Counts,Scope})
    if(!original.Output.Contains(term,StringComparison.Ordinal))
        throw new InvalidOperationException("RS125 baseline oracle output missing: "+term);

if(!target)
{
    var steps=new List<JsonNode>();
    var results=new List<JsonNode>();
    steps.Add(Required(root,"source-locked-restore","dotnet",false,
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    foreach(var item in new[]{"analysis","scientific"})
        steps.Add(Required(root,item+"-source-build","dotnet",false,
            "build","src/projects/dw.tools.math."+item+
                "/dw.tools.math."+item+".csproj",
            "-c","Release","--no-restore").Receipt);
    foreach(var item in new[]{"analysis","scientific"})
    {
        var feed=item=="analysis"?
            "build/artifacts/m6-w01-c03-t2-local-feed":
            "build/artifacts/m6-w02-c04-t2-local-feed";
        steps.Add(Required(root,item+"-local-pack","dotnet",false,
            "pack","src/projects/dw.tools.math."+item+
                "/dw.tools.math."+item+".csproj",
            "-c","Release","--no-build","--no-restore",
            "--output",feed).Receipt);
    }
    foreach(var (family,id,version,feedName,clientProject,hashPrefix,successMarker)
        in new[]{
        ("analysis","dw.tools.math.analysis","1.1.0-preview.6",
            "m6-w01-c03-t2-local-feed",
            "tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj",
            "M6-W01-C03-T2-CONSUMER-SHA256:",
            "dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes=8"),
        ("scientific","dw.tools.math.scientific","1.1.0-preview.14",
            "m6-w02-c04-t2-local-feed",
            "tests/consumers/m6-w02-c04-t2/m6-dated-finance-consumer.csproj",
            "M6-W02-C04-T2-CONSUMER-SHA256:",
            "dw.tools.math/m6-dated-finance-consumer/1.1 qualified; probes=8")
    })
    {
        var feed=Path.Combine(root,"build","artifacts",feedName);
        var archive=Path.Combine(feed,id+"."+version+".nupkg");
        var cache=Path.Combine(root,
            PackageCache.Replace('/',Path.DirectorySeparatorChar));
        steps.Add(Required(root,family+"-offline-restore","dotnet",true,
            "restore",clientProject,"--source",feed,
            "--packages",cache,"-p:NuGetAudit=false").Receipt);
        var executed=Required(root,family+"-source-free-consumer","dotnet",true,
            "run","--project",clientProject,"-c","Release",
            "--no-restore","--",archive);
        steps.Add(executed.Receipt);
        if(!executed.Output.Contains(successMarker,StringComparison.Ordinal))
            throw new InvalidOperationException("Missing qualified eight-probe marker: "+family);
        var lines=executed.Output.Split('\n',
            StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
        var observed=lines.Where(line=>
            line.StartsWith(hashPrefix,StringComparison.Ordinal)).ToArray();
        if(observed.Length!=1)
            throw new InvalidOperationException(
                "Consumer must report exactly one archive identity: "+family);
        var fields=observed[0].Split(':');
        if(fields.Length!=5||fields[0]!=hashPrefix.TrimEnd(':')||
           fields[1]!=id||fields[2]!=version||
           !long.TryParse(fields[3],out var byteCount)||byteCount<=0||
           fields[4].Length!=64||
           fields[4].Any(c=>!((c>='0'&&c<='9')||(c>='a'&&c<='f'))))
            throw new InvalidOperationException(
                "NuGet-only independent archive SHA receipt invalid: "+family);
        results.Add(new JsonObject
        {
            ["package_id"]=id,["version"]=version,["probes"]=8,
            ["archive_sha256"]=fields[4],["archive_bytes"]=byteCount,
            ["archive_relative_path"]="build/artifacts/"+feedName+
                "/"+id+"."+version+".nupkg",
            ["source_free"]=true,["offline_feed"]=true,
            ["isolated_cache"]=true,["consumer_project"]=clientProject,
            ["qualified_marker"]=successMarker
        });
    }
    if(steps.Count!=9||results.Count!=2)
        throw new InvalidOperationException(
            "RS125 must have nine local commands and two independent consumers.");
    var sdk=Required(root,"dotnet-sdk","dotnet",false,"--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS125-post-m7-nuget-consumers-qualified",
        ["status"]=Status,
        ["question"]="Do fresh current-source local analysis and scientific packages pass independent isolated source-free NuGet consumers without external feeds or publication?",
        ["observation"]="The analysis and scientific packages were freshly built and packed locally, restored from named offline feeds into a dedicated ignored NuGet cache and consumed by two existing independently asserting eight-probe executable clients.",
        ["decision"]="Admit only these two specific observed source-free net10 package consumers and sixteen bounded probes; do not infer publication, stable ABI, cross-RID support, broker discovery, integration or the remaining historic clients.",
        ["prior_delivery_evidence"]=PriorReceipt,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["source_free_consumers"]=2,["fresh_consumer_probes"]=16,
        ["fresh_packages_built"]=2,["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["previous_rs124_attempt"]=1,
        ["previous_rs124_preparation_refusals"]=1,
        ["previous_rs124_first_pass"]=false,
        ["offline_local_feeds_only"]=true,
        ["isolated_nuget_package_cache"]=true,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["discovery_skill_generated"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["all_historical_consumers_rerun"]=false,
        ["all_thirteen_packages_repacked"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(steps.ToArray()),
        ["consumer_results"]=p.Json.Array(results.ToArray()),
        ["next_scope"]="Qualify current source-free multi-package distribution consumption and/or public API/ABI stability separately. No automatic publication, recipient request transmission or M8 admission.",
        ["nonclaims"]=p.Json.StringArray(
            "These two consumers prove only their recorded current-source local preview packages and sixteen exact boundary checks.",
            "Historical additional Math clients, cross-version ABI and other platforms were not newly qualified.",
            "No public NuGet publication, external AURA/Decision/MCDM adoption or generated Discovery skill occurred.",
            "The previously completed Math native 541-node roadmap and accepted backlog remain unchanged.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var checkedTarget=Required(root,"target-consumer-check","dotnet",false,
    "run","--file",Validator,"--","--check");
if(!checkedTarget.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException(
        "Independent source-free target receipt refused.");
return p.Complete();

static (JsonObject Receipt,string Output) Required(
    string root,string id,string executable,bool isolatedPackages,
    params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    if(isolatedPackages)
        process.StartInfo.Environment["NUGET_PACKAGES"]=Path.Combine(
            root,"build","artifacts","rs125-isolated-packages");
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var timer=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS125 command did not start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS125 bounded command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    timer.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS125 failed: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(output.Length,2600)]);
    Console.WriteLine("RS125 "+id+" exit=0 elapsed_ms="+timer.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,["exit_code"]=process.ExitCode,
        ["command"]=executable+" "+string.Join(" ",args),
        ["elapsed_ms"]=timer.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1200)],
        ["source_free_consumer_execution"]=id.EndsWith("-source-free-consumer",
            StringComparison.Ordinal)
    },output);
}
