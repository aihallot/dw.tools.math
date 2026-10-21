using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Prior=".aura/workflow/reports/RS123/attempt-001.json";
const string Gate="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string Decision="docs/planning/decisions/post-m7-delivery-qualification.md";
const string Matrix="docs/distribution/post-m7-delivery-inventory.md";
const string Validator="docs/planning/ValidatePostM7DeliveryInventory.cs";
const string Evidence="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string Status="rs124-current-source-build-test-and-three-local-packages-qualified-unpublished";
const string Marker="dw.tools.math/rs124-post-m7-delivery/1.0 inventory-qualified; source=13; tests=11; historical-consumers=47; public=false";
const string Counts="MATH-DELIVERY:SOURCE=13;TESTS=11;SOLUTION=24;HISTORICAL-CONSUMER-DIRS=47";
const string Scope="MATH-DELIVERY:SCOPE:M0-M7=UNCHANGED;NODES=541;PACKS-TARGET=3;RELEASE=false;DISCOVERY-CANDIDATE=true";
const string PackDirectory="build/artifacts/rs124-local-packages";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS124",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS123",1,
    "docs: qualify M7 internal scope gate without publication",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/7/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Gate,"/status",
    JsonValue.Create("m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"));
p.Require.JsonValueEquals(Gate,"/public_release_authorized",JsonValue.Create(false));
p.Require.JsonValueEquals(Gate,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(Gate,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals(Gate,"/future_program_approved",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(Prior,"/priorInvocationFailures/firstPassSucceeded",JsonValue.Create(true));
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState("M6","done");
p.ProjectPlan.RequireNodeState("M7","done");

if(!target)
{
    foreach(var destination in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(destination);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    foreach(var (pointer,number) in new (string,int)[]
    {
        ("/source_projects",13),("/test_projects",11),
        ("/historical_consumer_directories",47),
        ("/fresh_local_package_archives",3),("/native_nodes_retained",541)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(number));
    foreach(var (pointer,value) in new (string,bool)[]
    {
        ("/locked_restore_passed",true),("/release_build_passed",true),
        ("/full_solution_tests_passed",true),
        ("/representative_local_packs_passed",true),
        ("/all_thirteen_packages_repacked",false),
        ("/public_release_authorized",false),("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false),
        ("/generated_discovery_skill_delivered",false)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(value));
}

var decisionOutput=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixOutput=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorOutput=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing RS124 decision SHA.");
var matrixSha=matrixOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing RS124 inventory SHA.");
var validatorSha=validatorOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing RS124 validator SHA.");
foreach(var text in new[]{"## Question and authorization",
    "## Source-controlled observations at selection baseline",
    "## Execution and independent acceptance",
    "## Accepted result and next decisions"})
    p.Require.TextContains(Decision,text,StringComparison.OrdinalIgnoreCase);
foreach(var text in new[]{"## Thirteen packable source projects in the solution",
    "## Qualified historical observations versus fresh RS124 checks",
    "## Static and dynamic verdicts"})
    p.Require.TextContains(Matrix,text,StringComparison.OrdinalIgnoreCase);
foreach(var text in new[]{Marker,Counts,Scope})
    p.Require.TextContains(Validator,text);
var initial=Execute(root,"inventory-before","dotnet",
    "run","--file",Validator,"--","--check");
foreach(var text in new[]{Marker,Counts,Scope})
    if(!initial.Output.Contains(text,StringComparison.Ordinal))
        throw new InvalidOperationException("RS124 independent baseline inventory marker absent: "+text);

if(!target)
{
    var checks=new List<JsonNode>();
    checks.Add(Execute(root,"locked-restore","dotnet",
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    checks.Add(Execute(root,"solution-release-build","dotnet",
        "build","dw.tools.math.slnx","-c","Release","--no-restore").Receipt);
    foreach(var family in new[]{"distribution","analysis","scientific"})
        checks.Add(Execute(root,"focused-"+family,"dotnet",
            "test","tests/projects/dw.tools.math."+family+".tests/dw.tools.math."+
                family+".tests.csproj",
            "-c","Release","--no-build","--no-restore").Receipt);
    checks.Add(Execute(root,"solution-release-tests","dotnet",
        "test","dw.tools.math.slnx","-c","Release","--no-build","--no-restore").Receipt);
    var packed=new List<JsonNode>();
    foreach(var (id,version) in new[]{
        ("dw.tools.math.distribution","1.0.0-preview.4"),
        ("dw.tools.math.analysis","1.1.0-preview.6"),
        ("dw.tools.math.scientific","1.1.0-preview.14")
    })
        checks.Add(Execute(root,"local-pack-"+id,"dotnet",
            "pack","src/projects/"+id+"/"+id+".csproj",
            "-c","Release","--no-build","--no-restore",
            "--output",PackDirectory).Receipt);
    var packInspection=Execute(root,"independent-local-package-inspection",
        "dotnet","run","--file",Validator,"--","--packages");
    checks.Add(packInspection.Receipt);
    var packageLines=packInspection.Output.Split('\n',
        StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)
        .Where(line=>line.StartsWith("RS124-PACK|",StringComparison.Ordinal))
        .ToArray();
    var declaredPackages=new[]{
        ("dw.tools.math.distribution","1.0.0-preview.4"),
        ("dw.tools.math.analysis","1.1.0-preview.6"),
        ("dw.tools.math.scientific","1.1.0-preview.14")
    };
    if(packageLines.Length!=declaredPackages.Length)
        throw new InvalidOperationException(
            "Independent CSharp package verifier did not return exactly three receipts.");
    for(var index=0;index<declaredPackages.Length;index++)
    {
        var (id,version)=declaredPackages[index];
        var data=packageLines[index].Split('|');
        if(data.Length!=5||data[0]!="RS124-PACK"||
           data[1]!=id||data[2]!=version||
           data[3].Length!=64||
           data[3].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f')))||
           !long.TryParse(data[4],out var packageBytes)||packageBytes<=0)
            throw new InvalidOperationException(
                "Unexpected independent NuGet package proof for "+id);
        packed.Add(new JsonObject
        {
            ["package_id"]=id,["version"]=version,
            ["relative_path"]=PackDirectory+"/"+id+"."+version+".nupkg",
            ["byte_length"]=packageBytes,["sha256"]=data[3],
            ["scope"]="ephemeral-local-only-not-published"
        });
    }
    var sdk=Execute(root,"dotnet-sdk","dotnet","--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS124-post-m7-delivery-qualified",
        ["status"]=Status,
        ["question"]="Which Math libraries, tests and local archives can be freshly qualified without reopening the completed M0-M7 plan or publishing?",
        ["observation"]="Thirteen source csprojs and eleven test projects remain in the source solution; forty-seven consumer directories are historical. Fresh local locked restore, Release build/test and three representative package creation/inspection completed under the controlled run.",
        ["decision"]="Accept local current-source build/test and only three fresh package SHA-qualified archives. Historical downstream consumers, the stale root README, candidate Discovery and all public/external/ABI/rights gates remain outstanding.",
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["distribution_document"]=Matrix,["distribution_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["source_projects"]=13,["test_projects"]=11,
        ["historical_consumer_directories"]=47,
        ["fresh_local_package_archives"]=3,["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["historical_m5_local_archives"]=7,
        ["historical_m6_source_free_consumers"]=10,
        ["historical_m6_source_free_probes"]=80,
        ["locked_restore_passed"]=true,["release_build_passed"]=true,
        ["focused_regressions_passed"]=true,
        ["full_solution_tests_passed"]=true,
        ["representative_local_packs_passed"]=true,
        ["all_thirteen_packages_repacked"]=false,
        ["fresh_source_free_consumer_rerun"]=false,
        ["public_release_authorized"]=false,["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["generated_discovery_skill_delivered"]=false,
        ["cross_version_abi_certified"]=false,
        ["other_os_rids_certified"]=false,
        ["new_advanced_public_product_delivered"]=false,
        ["m0_to_m7_native_plan_mutated"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(checks.ToArray()),
        ["local_archive_manifest"]=p.Json.Array(packed.ToArray()),
        ["next_scope"]="Owner chooses deeper source-free/API-ABI/packaging qualification or a supported DWF new-roadmap admission. No M8, publication or recipient transmission is selected.",
        ["nonclaims"]=p.Json.StringArray(
            "Thirteen declared packable projects are not thirteen newly qualified NuGet archives; only three representative archives were freshly packed.",
            "The forty-seven local consumer folders and prior M5/M6 source-free tests were not rerun.",
            "No public registry publication, multi-RID support, Discovery agent skill, client migration or new release permission was proven.",
            "The accepted 541-node M0-M7 plan stays unchanged; no new milestone or completion achievement is invented.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var checkedTarget=Execute(root,"inventory-after","dotnet",
    "run","--file",Validator,"--","--check");
if(!checkedTarget.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS124 independent target inventory rejected.");
return p.Complete();

static (JsonObject Receipt,string Output) Execute(
    string root,string id,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var item in args)process.StartInfo.ArgumentList.Add(item);
    var clock=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS124 command failed to start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(600000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS124 command timed out after ten minutes: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    clock.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS124 command failed: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(output.Length,2800)]);
    Console.WriteLine(id+" exit=0 elapsed_ms="+clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,["command"]=executable+" "+string.Join(" ",args),
        ["exit_code"]=process.ExitCode,
        ["duration_milliseconds"]=clock.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1200)],
        ["successful_local_execution"]=true
    },output);
}
