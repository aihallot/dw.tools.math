using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string PriorReport=".aura/workflow/reports/RS127/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Decision="docs/planning/decisions/post-m7-symbolic-codec-consumers.md";
const string Matrix="docs/distribution/post-m7-symbolic-codec-consumer-matrix.md";
const string Validator="docs/planning/ValidatePostM7SymbolicCodecConsumers.cs";
const string Evidence="docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json";
const string Status="rs128-four-local-symbolic-codec-packages-two-source-free-consumers-qualified-unpublished";
const string Counts="MATH-RS128:PACKAGES=4;CONSUMERS=2;M4-PROBES=32;M5-PROBES=8;NATIVE=541";
const string Scope="MATH-RS128:SCOPE=NO-PROGRESS;LOCAL-FEED=true;EXTERNAL=false;PUBLICATION=false;ABI=false";
const string Marker="dw.tools.math/rs128-symbolic-codec-consumers/1.0 qualified; packages=4; consumers=2; probes=40; public=false";
const string Feed="build/artifacts/rs128-symbolic-codec-feed";
const string Cache="build/artifacts/rs128-isolated-packages";
const string M4Project="tests/consumers/m4-gate/m4-gate-consumer.csproj";
const string M5Project="tests/consumers/m5-w01-c04-t2/m5-math-display-boundary-consumer.csproj";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS128",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS127",1,
    "docs: qualify full local Math package inventory and isolated clients",
    "0.1.59",2,2,0,false);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReceipt,"/status",JsonValue.Create(
    "rs127-thirteen-fresh-local-packages-two-independent-consumers-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/packages_packed",JsonValue.Create(13));
p.Require.JsonValueEquals(PriorReceipt,"/source_free_consumers",JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReceipt,"/m3_fresh_probes",JsonValue.Create(31));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/cross_version_abi_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/preparationFailureCount",
    JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(false));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
foreach(var node in new[]{"M5","M6","M7"})
    p.ProjectPlan.RequireNodeState(node,"done");

if(!target)
{
    foreach(var file in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (pointer,number) in new (string,int)[]
    {
        ("/packages_packed",4),("/source_free_consumers",2),
        ("/fresh_consumer_probes",40),("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(number));
    foreach(var (pointer,flag) in new (string,bool)[]
    {
        ("/offline_feed_only",true),("/isolated_package_cache",true),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/discovery_skill_generated",false),
        ("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),
        ("/m0_m7_plan_mutated",false)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(flag));
}
var decisionWrite=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixWrite=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorWrite=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing RS128 decision SHA.");
var matrixSha=matrixWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing RS128 distribution matrix SHA.");
var validatorSha=validatorWrite.AfterSha256 ??
    throw new InvalidOperationException("Missing RS128 independent validator SHA.");
foreach(var term in new[]{
    "## Authorization and purpose","## Verified input and exact science scope",
    "## One bounded DWF qualification chain","## Decision and remaining debts"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{
    "## Four-package closed dependency graph",
    "## Two independent existing consumers",
    "## Scientific nonclaims and historical continuity"})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{Status,Counts,Scope,Marker})
    p.Require.TextContains(Validator,term);
var baseline=Execute(root,"baseline-symbolic-codec-gate","dotnet",false,
    "run","--file",Validator,"--","--check");
foreach(var term in new[]{Counts,Scope,Marker})
    if(!baseline.Output.Contains(term,StringComparison.Ordinal))
        throw new InvalidOperationException("RS128 independent source baseline marker missing: "+term);

if(!target)
{
    var commands=new List<JsonNode>();
    var archiveResults=new List<JsonNode>();
    var clientResults=new List<JsonNode>();
    commands.Add(Execute(root,"source-locked-restore","dotnet",false,
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    foreach(var family in new[]{"symbolics","codecs"})
        commands.Add(Execute(root,family+"-release-build","dotnet",false,
            "build","src/projects/dw.tools.math."+family+
                "/dw.tools.math."+family+".csproj",
            "-c","Release","--no-restore").Receipt);
    var packages=new (string Id,string Version)[]
    {
        ("dw.quantities","0.2.0-preview.1"),
        ("dw.tools.math.ir","0.3.0-preview.1"),
        ("dw.tools.math.symbolics","0.5.0-preview.10"),
        ("dw.tools.math.codecs","1.0.0-preview.8")
    };
    foreach(var (id,version) in packages)
        commands.Add(Execute(root,"pack-"+id,"dotnet",false,
            "pack","src/projects/"+id+"/"+id+".csproj",
            "-c","Release","--no-build","--no-restore","--output",Feed).Receipt);
    var inspected=Execute(root,"independent-local-package-audit",
        "dotnet",false,"run","--file",Validator,"--","--packages");
    commands.Add(inspected.Receipt);
    var packageRows=inspected.Output.Split('\n',
        StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
        .Where(row=>row.StartsWith("RS128-PACK|",StringComparison.Ordinal))
        .ToArray();
    if(packageRows.Length!=packages.Length)
        throw new InvalidOperationException(
            "Independent RS128 archive audit must return four exact ZIP receipts.");
    for(var i=0;i<packages.Length;i++)
    {
        var values=packageRows[i].Split('|');
        if(values.Length!=5||values[0]!="RS128-PACK"||
           values[1]!=packages[i].Id||values[2]!=packages[i].Version||
           !long.TryParse(values[3],out var length)||length<=0||length>4_194_304||
           !Hex64(values[4]))
            throw new InvalidOperationException(
                "Incorrect independent nupkg size/SHA for "+packages[i].Id);
        archiveResults.Add(new JsonObject
        {
            ["package_id"]=packages[i].Id,["version"]=packages[i].Version,
            ["archive_relative_path"]=Feed+"/"+packages[i].Id+"."+
                packages[i].Version+".nupkg",
            ["archive_bytes"]=length,["archive_sha256"]=values[4],
            ["scope"]="fresh-current-local-net10-only"
        });
    }
    var localFeed=Path.Combine(root,Feed.Replace('/',Path.DirectorySeparatorChar));
    var localCache=Path.Combine(root,Cache.Replace('/',Path.DirectorySeparatorChar));
    foreach(var (family,client,probes,success) in new[]{
        ("m4",M4Project,32,
            "dw.tools.math/m4-nuget-gate/0.5 qualified; probes=32"),
        ("m5",M5Project,8,
            "dw.tools.math/m5-markdown-omml-boundary-consumer/1.0 qualified; probes=8")
    })
    {
        commands.Add(Execute(root,family+"-offline-restore","dotnet",true,
            "restore",client,"--source",localFeed,
            "--packages",localCache,"-p:NuGetAudit=false").Receipt);
        var observed=Execute(root,family+"-source-free-consumer",
            "dotnet",true,"run","--project",client,
            "-c","Release","--no-restore");
        commands.Add(observed.Receipt);
        if(!observed.Output.Contains(success,StringComparison.Ordinal))
            throw new InvalidOperationException("Independent consumer probes failed: "+family);
        clientResults.Add(new JsonObject
        {
            ["id"]=family,["project"]=client,["probes"]=probes,
            ["passed"]=true,["source_free"]=true,
            ["offline_feed_only"]=true,["isolated_cache"]=true,
            ["qualified_marker"]=success
        });
    }
    if(commands.Count!=12||archiveResults.Count!=4||
       clientResults.Count!=2)
        throw new InvalidOperationException(
            "RS128 expected twelve local commands, four packages and two consumers.");
    var sdk=Execute(root,"observed-sdk","dotnet",false,"--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS128-post-m7-symbolic-codec-qualified",
        ["status"]=Status,
        ["question"]="Do the current-source bounded symbolics and exact Markdown/OMML NuGet clients pass their original forty independent probes from a fresh isolated four-package local feed?",
        ["observation"]="Four local preview Math archives were built and ZIP/dependency/SHA qualified. Two pre-existing source-free clients restored only from the local feed in a private cache and reran thirty-two symbolic and eight codec/OMML probes.",
        ["decision"]="Admit these two local source-free client executions on the observed host. No general symbolic proof engine, Office document fidelity, ABI promise, publication, downstream migration or new roadmap is established.",
        ["prior_distribution_evidence"]=PriorReceipt,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["packages_packed"]=4,["source_free_consumers"]=2,
        ["fresh_consumer_probes"]=40,["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["previous_rs127_attempt"]=1,["previous_rs127_first_pass"]=false,
        ["previous_rs127_preparation_refusals"]=2,
        ["offline_feed_only"]=true,["isolated_package_cache"]=true,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["discovery_skill_generated"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(commands.ToArray()),
        ["package_results"]=p.Json.Array(archiveResults.ToArray()),
        ["consumer_results"]=p.Json.Array(clientResults.ToArray()),
        ["next_scope"]="Owner may select further isolated source-free clients or a version-pinned ABI audit where historical package bytes are genuinely available. No automatic M8, publication or recipient request transmission.",
        ["nonclaims"]=p.Json.StringArray(
            "Eight M5 display probes do not certify complete Office documents, typography or lossless arbitrary Markdown conversions.",
            "Thirty-two M4 symbolic probes do not create a general CAS, unconstrained symbolic engine or proof kernel.",
            "A fresh four-package local feed and two consumers do not certify public release, cross-version ABI or other RIDs.",
            "AURA, Decision and MCDM requests remain unsent; the 541-node DWF plan remains unchanged.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var checkedTarget=Execute(root,"target-symbolic-codec-gate","dotnet",false,
    "run","--file",Validator,"--","--check");
if(!checkedTarget.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent RS128 target science gate refused.");
return p.Complete();

static bool Hex64(string value)=>value.Length==64&&
    value.All(c=>(c>='0'&&c<='9')||(c>='a'&&c<='f'));
static (JsonObject Receipt,string Output) Execute(
    string root,string id,string command,bool isolated,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=command,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    if(isolated)
        process.StartInfo.Environment["NUGET_PACKAGES"]=Path.Combine(
            root,"build","artifacts","rs128-isolated-packages");
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    var watch=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS128 local command could not start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS128 bounded command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    watch.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS128 local command failed: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(output.Length,2600)]);
    Console.WriteLine("RS128 "+id+" exit=0 elapsed_ms="+watch.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,["exit_code"]=process.ExitCode,
        ["command"]=command+" "+string.Join(" ",args),
        ["elapsed_ms"]=watch.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1100)],
        ["source_free_consumer"]=id.EndsWith("-source-free-consumer",
            StringComparison.Ordinal)
    },output);
}
