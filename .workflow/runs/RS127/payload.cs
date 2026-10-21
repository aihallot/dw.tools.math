using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string PriorReport=".aura/workflow/reports/RS126/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS126-post-m7-distribution-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Decision="docs/planning/decisions/post-m7-complete-local-packaging.md";
const string Matrix="docs/distribution/post-m7-complete-package-matrix.md";
const string Validator="docs/planning/ValidatePostM7CompletePackaging.cs";
const string Evidence="docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json";
const string Status="rs127-thirteen-fresh-local-packages-two-independent-consumers-qualified-unpublished";
const string Counts="MATH-RS127:PACKAGES=13;NEW-CONSUMERS=2;M3-PROBES=31;EXACT-SMOKE=PASS;NATIVE=541";
const string Scope="MATH-RS127:SCOPE=M0-M7-UNCHANGED;LOCAL-NUGET=true;PUBLICATION=false;ABI=false";
const string Marker="dw.tools.math/rs127-complete-current-packaging/1.0 qualified; packages=13; consumers=2; m3=31; exact=pass";
const string Feed="build/artifacts/rs127-complete-local-feed";
const string NuGetCache="build/artifacts/rs127-isolated-packages";
const string M3Project="tests/consumers/m3-gate/m3-gate-consumer.csproj";
const string ExactProject="tests/consumers/exact-math-smoke/exact-math-smoke.csproj";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS127",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS126",1,
    "docs: qualify offline Math distribution package graph and API surface",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReceipt,"/status",
    JsonValue.Create("rs126-fresh-seven-package-offline-distribution-surface-qualified-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/packages_packed",JsonValue.Create(7));
p.Require.JsonValueEquals(PriorReceipt,"/source_free_consumers",JsonValue.Create(1));
p.Require.JsonValueEquals(PriorReceipt,"/fresh_consumer_probes",JsonValue.Create(8));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/cross_version_abi_certified",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(true));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
foreach(var id in new[]{"M5","M6","M7"})
    p.ProjectPlan.RequireNodeState(id,"done");
if(!target)
{
    foreach(var sourcePath in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(sourcePath);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (pointer,number) in new (string,int)[]
    {
        ("/source_projects",13),("/test_projects",11),
        ("/packages_packed",13),("/source_free_consumers",2),
        ("/m3_fresh_probes",31),("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(number));
    foreach(var (pointer,expected) in new (string,bool)[]
    {
        ("/exact_quantity_smoke_passed",true),
        ("/offline_feed_only",true),("/isolated_package_cache",true),
        ("/foundation_source_free_consumer_rerun",false),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/cross_version_abi_certified",false),
        ("/other_os_rids_certified",false),
        ("/discovery_skill_generated",false),
        ("/all_historical_consumers_rerun",false),
        ("/m0_m7_plan_mutated",false)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(expected));
}
var decisionOutput=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixOutput=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorOutput=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionOutput.AfterSha256 ??
    throw new InvalidOperationException("RS127 canonical decision SHA absent.");
var matrixSha=matrixOutput.AfterSha256 ??
    throw new InvalidOperationException("RS127 canonical package matrix SHA absent.");
var validatorSha=validatorOutput.AfterSha256 ??
    throw new InvalidOperationException("RS127 independent BCL verifier SHA absent.");
foreach(var term in new[]{"## Owner-selected scope",
    "## Source-controlled acceptance inventory",
    "## DWF-controlled reproducibility",
    "## Technical meaning and next choice"})
    p.Require.TextContains(Decision,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{"## Complete package graph",
    "## Fresh source-free consumers",
    "## Qualification lineage and pending gaps"})
    p.Require.TextContains(Matrix,term,StringComparison.OrdinalIgnoreCase);
foreach(var term in new[]{Status,Marker,Counts,Scope})
    p.Require.TextContains(Validator,term);
var initial=Run(root,"baseline-complete-package-check","dotnet",false,
    "run","--file",Validator,"--","--check");
foreach(var marker in new[]{Counts,Scope,Marker})
    if(!initial.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS127 baseline package marker missing: "+marker);

if(!target)
{
    var steps=new List<JsonNode>();
    var results=new List<JsonNode>();
    var consumers=new List<JsonNode>();
    steps.Add(Run(root,"locked-solution-restore","dotnet",false,
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    steps.Add(Run(root,"release-solution-build","dotnet",false,
        "build","dw.tools.math.slnx","-c","Release","--no-restore").Receipt);
    var packages=new (string Id,string Version)[]
    {
        ("dw.tools.math.foundation","0.1.0-preview.1"),
        ("dw.quantities","0.2.0-preview.1"),
        ("dw.quantities.standard","0.2.0-preview.1"),
        ("dw.quantities.expression","0.2.0-preview.1"),
        ("dw.tools.math.ir","0.3.0-preview.1"),
        ("dw.tools.math.composition","0.3.0-preview.1"),
        ("dw.tools.math.numerics","0.4.0-preview.4"),
        ("dw.tools.math.statistics","0.4.0-preview.6"),
        ("dw.tools.math.symbolics","0.5.0-preview.10"),
        ("dw.tools.math.codecs","1.0.0-preview.8"),
        ("dw.tools.math.distribution","1.0.0-preview.4"),
        ("dw.tools.math.analysis","1.1.0-preview.6"),
        ("dw.tools.math.scientific","1.1.0-preview.14")
    };
    foreach(var (id,version) in packages)
        steps.Add(Run(root,"pack-"+id,"dotnet",false,
            "pack","src/projects/"+id+"/"+id+".csproj",
            "-c","Release","--no-build","--no-restore",
            "--output",Feed).Receipt);
    var archiveAudit=Run(root,"independent-thirteen-package-audit","dotnet",false,
        "run","--file",Validator,"--","--packages");
    steps.Add(archiveAudit.Receipt);
    var outputs=archiveAudit.Output.Split('\n',
        StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith("RS127-PACK|",StringComparison.Ordinal))
        .ToArray();
    if(outputs.Length!=packages.Length)
        throw new InvalidOperationException("Thirteen independent ZIP identities required.");
    for(var i=0;i<packages.Length;i++)
    {
        var fields=outputs[i].Split('|');
        if(fields.Length!=5||fields[0]!="RS127-PACK"||
           fields[1]!=packages[i].Id||fields[2]!=packages[i].Version||
           !long.TryParse(fields[3],out var byteLength)||byteLength<=0||
           !Hex64(fields[4]))
            throw new InvalidOperationException(
                "Wrong independently observed local package SHA/identity: "+packages[i].Id);
        results.Add(new JsonObject
        {
            ["package_id"]=packages[i].Id,
            ["version"]=packages[i].Version,
            ["archive_bytes"]=byteLength,
            ["archive_sha256"]=fields[4],
            ["archive_relative_path"]=Feed+"/"+packages[i].Id+"."+
                packages[i].Version+".nupkg",
            ["scope"]="fresh-local-net10-not-published"
        });
    }
    var localFeed=Path.Combine(root,Feed.Replace('/',Path.DirectorySeparatorChar));
    var localCache=Path.Combine(root,NuGetCache.Replace('/',Path.DirectorySeparatorChar));
    foreach(var (id,csproj,expectedMarker,probes) in new[]{
        ("m3",M3Project,"dw.tools.math/m3-nuget-gate/0.4 qualified; probes=31",31),
        ("exact",ExactProject,"dw.tools.math/exact-consumer/0.2 qualified",0)
    })
    {
        steps.Add(Run(root,id+"-offline-restore","dotnet",true,
            "restore",csproj,"--source",localFeed,
            "--packages",localCache,"-p:NuGetAudit=false").Receipt);
        var consumer=Run(root,id+"-source-free-consumer","dotnet",true,
            "run","--project",csproj,"-c","Release","--no-restore");
        steps.Add(consumer.Receipt);
        if(!consumer.Output.Contains(expectedMarker,StringComparison.Ordinal))
            throw new InvalidOperationException("Independent source-free client failed: "+id);
        consumers.Add(new JsonObject
        {
            ["id"]=id,["project"]=csproj,["passed"]=true,
            ["source_free"]=true,["local_feed_only"]=true,
            ["isolated_package_cache"]=true,["probes"]=probes,
            ["qualified_marker"]=expectedMarker
        });
    }
    if(steps.Count!=20||results.Count!=13||consumers.Count!=2)
        throw new InvalidOperationException(
            "Complete-package run requires twenty commands, thirteen packages and two clients.");
    var sdk=Run(root,"observed-sdk","dotnet",false,"--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS127-post-m7-complete-packaging-qualified",
        ["status"]=Status,
        ["question"]="Can all thirteen currently accepted Math net10 libraries be packed from one local source tree and independently consumed for quantities and statistics, without publication or DWF progress changes?",
        ["observation"]="One locked solution restore and Release build, thirteen exact local NuGet archives, independent ZIP/nuspec/dependency SHA inspection and two isolated source-free consumers succeeded. M3 provided 31 asserted probes; the exact quantity smoke passed without an invented numeric count.",
        ["decision"]="Admit a complete local thirteen-package snapshot and the two specific observed NuGet consumers only. Foundation was archive-checked, not independently smoke-rerun; public, ABI, other-RID, Discovery, external adoption and new-roadmap gates remain open.",
        ["previous_distribution_evidence"]=PriorReceipt,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["source_projects"]=13,["test_projects"]=11,
        ["packages_packed"]=13,["source_free_consumers"]=2,
        ["m3_fresh_probes"]=31,["exact_quantity_smoke_passed"]=true,
        ["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["previous_rs126_attempt"]=1,
        ["previous_rs126_first_pass"]=true,
        ["offline_feed_only"]=true,
        ["isolated_package_cache"]=true,
        ["foundation_source_free_consumer_rerun"]=false,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["discovery_skill_generated"]=false,
        ["all_historical_consumers_rerun"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(steps.ToArray()),
        ["package_results"]=p.Json.Array(results.ToArray()),
        ["consumer_results"]=p.Json.Array(consumers.ToArray()),
        ["next_scope"]="Owner choice: pinned genuine cross-version API/ABI comparison, remaining source-free consumers, stale public docs audit, or authorized supported new roadmap. No M8 or release is selected.",
        ["nonclaims"]=p.Json.StringArray(
            "A complete thirteen-package local feed is not a public stable package release or proof that every mathematical operation has been implemented.",
            "Thirty-one checked M3 cases and one passing unnumbered exact smoke cannot certify all statistical, algebraic, parser or dimensional uses.",
            "No fresh source-free foundation test, cross-version ABI guarantee, external recipient migration, generated Discovery skill or other-RID certification was obtained.",
            "M0–M7 remains accepted done with all 541 native nodes unchanged; no new completion achievement or product milestone was created.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var verified=Run(root,"target-complete-package-check","dotnet",false,
    "run","--file",Validator,"--","--check");
if(!verified.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS127 source-pinned target package gate failed.");
return p.Complete();

static bool Hex64(string value)=>value.Length==64&&
    value.All(x=>(x>='0'&&x<='9')||(x>='a'&&x<='f'));
static (JsonObject Receipt,string Output) Run(
    string root,string id,string executable,bool isolated,
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
    if(isolated)
        process.StartInfo.Environment["NUGET_PACKAGES"]=Path.Combine(
            root,"build","artifacts","rs127-isolated-packages");
    foreach(var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS127 local process did not start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS127 local command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    clock.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS127 local command failure: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(output.Length,2600)]);
    Console.WriteLine("RS127 "+id+" exit=0 elapsed_ms="+clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,["exit_code"]=process.ExitCode,
        ["command"]=executable+" "+string.Join(" ",args),
        ["elapsed_ms"]=clock.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1200)],
        ["independent_consumer"]=id.EndsWith("-source-free-consumer",
            StringComparison.Ordinal)
    },output);
}
