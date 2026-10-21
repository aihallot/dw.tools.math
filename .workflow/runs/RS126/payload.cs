using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string PriorReport=".aura/workflow/reports/RS125/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Decision="docs/planning/decisions/post-m7-distribution-graph-qualification.md";
const string Matrix="docs/distribution/post-m7-seven-package-surface-matrix.md";
const string Validator="docs/planning/ValidatePostM7DistributionGraph.cs";
const string Evidence="docs/planning/evidence/RS126-post-m7-distribution-qualified.json";
const string Status="rs126-fresh-seven-package-offline-distribution-surface-qualified-unpublished";
const string Counts="MATH-RS126:PACKAGES=7;SOURCE-FREE-CONSUMERS=1;FRESH-PROBES=8;NATIVE=541";
const string Scope="MATH-RS126:SCOPE=NO-PROGRESS;LOCAL-NUGET=true;API-SHA=OBSERVED-ONLY;PUBLICATION=false";
const string Marker="dw.tools.math/rs126-multipackage-surface/1.0 qualified; archive-graph=7; probes=8; abi-certified=false";
const string Feed="build/artifacts/m5-w02-c02-t2-local-feed";
const string NuGetCache="build/artifacts/rs126-isolated-packages";
const string Client="tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS126",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS125",1,
    "docs: qualify fresh offline Math NuGet consumers without publication",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReceipt,"/status",
    JsonValue.Create("rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/source_free_consumers",JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_consumer_probes",JsonValue.Create(16));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_packages_built",JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/cross_version_abi_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(true));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
foreach(var node in new[]{"M5","M6","M7"})
    p.ProjectPlan.RequireNodeState(node,"done");
if(!target)
{
    foreach(var destination in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(destination);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (ptr,number) in new (string,int)[]
    {
        ("/packages_packed",7),("/source_free_consumers",1),
        ("/fresh_consumer_probes",8),("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,ptr,JsonValue.Create(number));
    foreach(var (ptr,value) in new (string,bool)[]
    {
        ("/offline_local_feed_only",true),
        ("/isolated_nuget_package_cache",true),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/discovery_skill_generated",false),
        ("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),
        ("/all_historical_consumers_rerun",false),
        ("/m0_m7_plan_mutated",false)
    })
        p.Require.JsonValueEquals(Evidence,ptr,JsonValue.Create(value));
}
var decisionFile=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixFile=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorFile=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionFile.AfterSha256 ??
    throw new InvalidOperationException("RS126 decision source SHA is unavailable.");
var matrixSha=matrixFile.AfterSha256 ??
    throw new InvalidOperationException("RS126 matrix source SHA is unavailable.");
var validatorSha=validatorFile.AfterSha256 ??
    throw new InvalidOperationException("RS126 independent validator source SHA is unavailable.");
foreach(var term in new[]{
    "## Question and accepted scope","## Verified source and historical evidence",
    "## Controlled evidence chain","## Decision boundary and follow-up"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{
    "## Closed dependency and package roster",
    "## Independent current consumer",
    "## Historical versus fresh and release restrictions"})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{Marker,Counts,Scope})
    p.Require.TextContains(Validator,term);
var initial=Execute(root,"initial-graph-validator","dotnet",false,
    "run","--file",Validator,"--","--check");
foreach(var marker in new[]{Marker,Counts,Scope})
    if(!initial.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS126 baseline source graph marker missing: "+marker);

if(!target)
{
    var commands=new List<JsonNode>();
    commands.Add(Execute(root,"source-locked-restore","dotnet",false,
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    commands.Add(Execute(root,"distribution-release-build","dotnet",false,
        "build","src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj",
        "-c","Release","--no-restore").Receipt);
    var accepted=new (string Id,string Version)[]
    {
        ("dw.quantities","0.2.0-preview.1"),
        ("dw.tools.math.ir","0.3.0-preview.1"),
        ("dw.tools.math.codecs","1.0.0-preview.8"),
        ("dw.tools.math.symbolics","0.5.0-preview.10"),
        ("dw.tools.math.numerics","0.4.0-preview.4"),
        ("dw.tools.math.composition","0.3.0-preview.1"),
        ("dw.tools.math.distribution","1.0.0-preview.4")
    };
    foreach(var (id,version) in accepted)
        commands.Add(Execute(root,"pack-"+id,"dotnet",false,
            "pack","src/projects/"+id+"/"+id+".csproj",
            "-c","Release","--no-build","--no-restore","--output",Feed).Receipt);
    var audited=Execute(root,"independent-package-audit","dotnet",false,
        "run","--file",Validator,"--","--packages");
    commands.Add(audited.Receipt);
    var packageRows=audited.Output.Split('\n',
        StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
        .Where(s=>s.StartsWith("RS126-PACK|",StringComparison.Ordinal)).ToArray();
    if(packageRows.Length!=accepted.Length)
        throw new InvalidOperationException("Seven independent fresh package rows are mandatory.");
    var packageResults=new List<JsonNode>();
    for(var i=0;i<accepted.Length;i++)
    {
        var fields=packageRows[i].Split('|');
        if(fields.Length!=5||fields[0]!="RS126-PACK"||
           fields[1]!=accepted[i].Id||fields[2]!=accepted[i].Version||
           !long.TryParse(fields[3],out var length)||length<=0||length>4_194_304||
           !ValidSha(fields[4]))
            throw new InvalidOperationException(
                "Invalid independent ZIP identity or hash: "+accepted[i].Id);
        packageResults.Add(new JsonObject
        {
            ["package_id"]=accepted[i].Id,["version"]=accepted[i].Version,
            ["archive_relative_path"]=Feed+"/"+accepted[i].Id+"."+
                accepted[i].Version+".nupkg",
            ["archive_bytes"]=length,["archive_sha256"]=fields[4],
            ["public_api_sha256"]="pending-consumer",
            ["managed_assembly_sha256"]="pending-consumer",
            ["scope"]="fresh-local-only-not-published"
        });
    }
    var localFeed=Path.Combine(root,
        Feed.Replace('/',Path.DirectorySeparatorChar));
    var isolatedCache=Path.Combine(root,
        NuGetCache.Replace('/',Path.DirectorySeparatorChar));
    commands.Add(Execute(root,"distribution-offline-restore","dotnet",true,
        "restore",Client,"--source",localFeed,
        "--packages",isolatedCache,"-p:NuGetAudit=false").Receipt);
    var programArguments=new List<string>{
        "run","--project",Client,"-c","Release","--no-restore","--"
    };
    foreach(var item in accepted)
        programArguments.Add(Path.Combine(localFeed,
            item.Id+"."+item.Version+".nupkg"));
    var consumer=Execute(root,"distribution-source-free-consumer","dotnet",true,
        programArguments.ToArray());
    commands.Add(consumer.Receipt);
    const string ConsumerMarker=
        "dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=8";
    if(!consumer.Output.Contains(ConsumerMarker,StringComparison.Ordinal))
        throw new InvalidOperationException("Eight source-free M5 public surface probes failed.");
    var surfaceRows=consumer.Output.Split('\n',
        StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
        .Where(s=>s.StartsWith("M5-W02-C02-T2-SURFACE-SHA256:",StringComparison.Ordinal))
        .ToArray();
    if(surfaceRows.Length!=accepted.Length)
        throw new InvalidOperationException("Seven observed managed/API SHA pairs are mandatory.");
    for(var i=0;i<accepted.Length;i++)
    {
        var fields=surfaceRows[i].Split(':');
        if(fields.Length!=5||fields[0]!="M5-W02-C02-T2-SURFACE-SHA256"||
           fields[1]!=accepted[i].Id||fields[2]!=accepted[i].Version||
           !ValidSha(fields[3])||!ValidSha(fields[4]))
            throw new InvalidOperationException(
                "Invalid observed current-archive managed/public signature: "+
                accepted[i].Id);
        var row=(JsonObject)packageResults[i];
        row["public_api_sha256"]=fields[3];
        row["managed_assembly_sha256"]=fields[4];
    }
    var runtime="OBSERVED-RUNTIME:"+RuntimeInformation.RuntimeIdentifier;
    if(!consumer.Output.Contains(runtime,StringComparison.Ordinal))
        throw new InvalidOperationException("Consumer runtime RID does not match host.");
    if(commands.Count!=12||packageResults.Count!=7)
        throw new InvalidOperationException("Expected twelve successful commands and seven archives.");
    var sdk=Execute(root,"dotnet-sdk","dotnet",false,"--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS126-post-m7-distribution-qualified",
        ["status"]=Status,
        ["question"]="Can current-source seven-package Math net10 preview distribution qualify with an independent offline source-free consumer and public API surface fingerprints, without claiming cross-version compatibility or publication?",
        ["observation"]="Seven current-source local NuGet packages with exact dependency graph were packed and inspected, restored from one local feed in a fresh isolated cache, and independently consumed with eight mathematical, metadata and refusal probes.",
        ["decision"]="Admit only this observed current-source seven-package distribution graph and one local eight-probe source-free consumer. Per-archive public API and managed DLL SHA-256 are current fingerprints, not cross-version ABI guarantees.",
        ["previous_consumer_evidence"]=PriorReceipt,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["packages_packed"]=7,["source_free_consumers"]=1,
        ["fresh_consumer_probes"]=8,["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["previous_rs125_attempt"]=1,["previous_rs125_first_pass"]=true,
        ["offline_local_feed_only"]=true,
        ["isolated_nuget_package_cache"]=true,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["discovery_skill_generated"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["all_historical_consumers_rerun"]=false,
        ["all_thirteen_packages_repacked"]=false,
        ["new_advanced_public_product_delivered"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(commands.ToArray()),
        ["package_results"]=p.Json.Array(packageResults.ToArray()),
        ["next_scope"]="Independent version-pinned API/ABI comparison or remaining source-free preview consumers require separate scope; no automatic publication, external integration or M8.",
        ["nonclaims"]=p.Json.StringArray(
            "Current public surface hashes and one source-free consumer do not certify cross-version source or binary ABI, future builds or other RIDs.",
            "Seven locally packed net10 Math archives were not published to any public NuGet feed and their stable licensing remains owner gated.",
            "Historic consumer receipts from M5, M6, RS124 and RS125 remain separate; only eight new RS126 probes are counted.",
            "No agent Discovery implementation, AURA/Decision/MCDM cutover, new provider or change to 541 completed native nodes occurred.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var verified=Execute(root,"target-graph-validator","dotnet",false,
    "run","--file",Validator,"--","--check");
if(!verified.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent target source-free graph qualification failed.");
return p.Complete();

static bool ValidSha(string value)=>value.Length==64&&
    value.All(c=>(c>='0'&&c<='9')||(c>='a'&&c<='f'));
static (JsonObject Receipt,string Output) Execute(
    string root,string id,string executable,bool isolatedCache,
    params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,
        UseShellExecute=false,RedirectStandardOutput=true,
        RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    if(isolatedCache)
        process.StartInfo.Environment["NUGET_PACKAGES"]=Path.Combine(
            root,"build","artifacts","rs126-isolated-packages");
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS126 command did not start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS126 local source-free command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    clock.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS126 local command failed: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(output.Length,2600)]);
    Console.WriteLine("RS126 "+id+" exit=0 elapsed_ms="+clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,["exit_code"]=process.ExitCode,
        ["command"]=executable+" "+string.Join(" ",args),
        ["elapsed_ms"]=clock.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1200)],
        ["source_free_consumer_execution"]=id=="distribution-source-free-consumer"
    },output);
}
