using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M3-W02", Phase="M3-W02-C04", Task="M3-W02-C04-T1";
const string Question="M3-W02-C04-T1-A", Observation="M3-W02-C04-T1-B", DecisionNode="M3-W02-C04-T1-C";
const string ConsumerRoot="tests/consumers/m3-w02-c04/";
const string Consumer=ConsumerRoot+"c04-consumer.csproj";
const string Source=ConsumerRoot+"Program.cs";
const string Config=ConsumerRoot+"NuGet.Config";
const string Decision="docs/planning/decisions/m3-w02-c04-t1-consumer-fit.md";
const string Distribution="docs/distribution/m3-w02-c04-isolated-consumer.md";
const string Qualified="docs/planning/evidence/M3-W02-C04-consumer-qualified.json";
const string Prior="docs/planning/evidence/M3-W02-C03-boundaries-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string QuantityProject="src/projects/dw.quantities/dw.quantities.csproj";
const string StatisticsProject="src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj";
const string QuantityVersion="0.2.0-preview.1";
const string StatisticsVersion="0.4.0-preview.6";
const string ExpectedOutput="dw.tools.math/c04-isolated-consumer/0.4 qualified; probes=31";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS050","M3",Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS049",1,
    "feat: qualify distribution boundaries and close reproducible randomness",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("finite-continuous-distributions-and-portable-prng-checkpoints-qualified"));
p.Require.JsonValueEquals(Prior,"/new_green_tests_passed",JsonValue.Create(30));
p.Require.JsonValueEquals(Prior,"/total_statistics_tests_passed",JsonValue.Create(166));
p.Require.JsonValueEquals(Prior,"/local_package_version",JsonValue.Create(StatisticsVersion));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.48"),JsonValue.Create("0.1.49"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.49":"0.1.48"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/8/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/25/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/26/tasks/1/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/26/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals("docs/planning/evidence/M3-W02-C02-boundaries-qualified.json",
    "/status",JsonValue.Create("typed-undefined-correlation-and-pair-missing-boundaries-qualified"));

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C01","done");
p.ProjectPlan.RequireNodeState("M3-W02-C02","done");
p.ProjectPlan.RequireNodeState("M3-W02-C03","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02-C04-T2","not-ready");
foreach(var child in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    p.Require.FileMissing(Qualified);
    foreach(var file in new[]{Source,Consumer,Config,Decision,Distribution})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("isolated-math-packages-and-primitive-proposals-qualified"));
    p.Require.JsonValueEquals(Qualified,"/consumer_oracles_passed",JsonValue.Create(31));
    p.Require.JsonValueEquals(Qualified,"/external_adoption_observed",JsonValue.Create(false));
}

var stagedDigests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Consumer,Config,Source,Decision,Distribution})
{
    var result=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedDigests[file]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing SDK staged SHA-256 for "+file);
}

if(!target)
{
    // Verify this is a source-independent package consumer, not a project
    // reference disguised as a consumer test.
    var consumerXml=XDocument.Load(Path.Combine(root,Consumer.Replace('/',Path.DirectorySeparatorChar)));
    if(consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("RS050 isolated consumer cannot use a ProjectReference.");
    var refs=consumerXml.Descendants().Where(e=>e.Name.LocalName=="PackageReference")
        .Select(e=>((string?)e.Attribute("Include")??"",
            (string?)e.Attribute("Version")??"")).OrderBy(e=>e.Item1,StringComparer.Ordinal).ToArray();
    if(refs.Length!=2 ||
       refs[0].Item1!="dw.quantities" || refs[0].Item2!="["+QuantityVersion+"]" ||
       refs[1].Item1!="dw.tools.math.statistics" ||
       refs[1].Item2!="["+StatisticsVersion+"]")
        throw new InvalidOperationException("RS050 consumer package identities/versions are not pinned.");
    var nugetXml=XDocument.Load(Path.Combine(root,Config.Replace('/',Path.DirectorySeparatorChar)));
    var sources=nugetXml.Descendants().Single(e=>e.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(e=>e.Name.LocalName=="clear") ||
       sources.Elements().Count(e=>e.Name.LocalName=="add")!=1)
        throw new InvalidOperationException("RS050 consumer NuGet configuration has outside sources.");

    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var feed=Path.Combine(root,"build","artifacts","m3-w02-c04-local-feed");
    Required(root,"dotnet","pack",QuantityProject,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    Required(root,"dotnet","pack",StatisticsProject,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var quantityArchive=Path.Combine(feed,"dw.quantities."+QuantityVersion+".nupkg");
    var statisticsArchive=Path.Combine(feed,"dw.tools.math.statistics."+StatisticsVersion+".nupkg");
    InspectPackage(quantityArchive,"dw.quantities",QuantityVersion,[]);
    InspectPackage(statisticsArchive,"dw.tools.math.statistics",
        StatisticsVersion,["dw.quantities"]);

    // Product .nupkg SHA-256 is obtained by a dedicated, staged
    // post-package verifier. DWF workflow authority identities remain SDK-owned.
    var packageProof=Required(root,"dotnet","run","--file",
        ".workflow/runs/RS050/staged/checks/package-sha.cs","--",
        quantityArchive,statisticsArchive);
    var qtyDigest=RequirePackageChecksum(packageProof,Path.GetFileName(quantityArchive));
    var statsDigest=RequirePackageChecksum(packageProof,Path.GetFileName(statisticsArchive));

    var cache=Path.Combine(root,"build","artifacts","m3-w02-c04-isolated-packages");
    Required(root,"dotnet","restore",Consumer,"--source",feed,
        "--configfile",Config,"--packages",cache);
    Required(root,"dotnet","build",Consumer,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",Consumer,
        "-c","Release","--no-build","--no-restore");
    if(output.Trim()!=ExpectedOutput)
        throw new InvalidOperationException("RS050 isolated 31-oracle consumer marker mismatch: "+
            Relevant(output));
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W02-C04-consumer-qualified",
        ["status"]="isolated-math-packages-and-primitive-proposals-qualified",
        ["prior_C03_qualification"]=Prior,
        ["question"]="Are exact-scaling, conventioned quantiles and numerical primitives usable through independent Math-only NuGet packages without silently changing Decision/MCDM semantics?",
        ["observation"]="Isolated package-only .NET 10 consumer passes 31 named mathematical oracles and explicit loss/overflow refusal.",
        ["decision"]="Propose generic primitive evaluation to owning Decision/MCDM agents; preserve business ownership and leave recipient requests unsent.",
        ["consumer_source"]=Source,["consumer_source_sha256"]=stagedDigests[Source],
        ["consumer_project"]=Consumer,["consumer_project_sha256"]=stagedDigests[Consumer],
        ["consumer_config"]=Config,["consumer_config_sha256"]=stagedDigests[Config],
        ["policy_decision"]=Decision,["policy_sha256"]=stagedDigests[Decision],
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=stagedDigests[Distribution],
        ["package_only_consumer"]=true,
        ["no_project_references"]=true,
        ["external_nuget_sources_disabled"]=true,
        ["consumer_oracles_passed"]=31,
        ["consumer_exact_output"]=ExpectedOutput,
        ["lossy_scaling_refused"]=true,
        ["consumer_scaler_is_math_product_api"]=false,
        ["exact_scaling_oracles"]=p.Json.StringArray(
            "3/4 * 100 -> 75", "-3/4 * 100 -> -75",
            "1/3 * 100 -> rejected fractional scaled value",
            "(Int64.MaxValue+1)*1 -> refused"),
        ["exact_sum"]="1/3 + 1/6 = 1/2",
        ["type7_quantile"]="[0,10] at p=1/4 -> exact 5/2; finite 2.5",
        ["nearest_rank_quantile"]="[0,10] at p=1/4 -> 0",
        ["typed_undefined_pearson_is_null"]=true,
        ["splitmix64_golden_seed_zero"]="e220a8397b1dcdaf",
        ["package_quantities_id"]="dw.quantities",
        ["package_quantities_version"]=QuantityVersion,
        ["package_quantities_sha256"]=qtyDigest,
        ["package_statistics_id"]="dw.tools.math.statistics",
        ["package_statistics_version"]=StatisticsVersion,
        ["package_statistics_sha256"]=statsDigest,
        ["package_statistics_dependencies"]=p.Json.StringArray("dw.quantities"),
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["external_adoption_observed"]=false,
        ["requests_transmitted"]=false,
        ["no_business_policy_changes"]=true,
        ["next_math_scope"]="M3-W02-C04-T2 ownership and recipient-boundary decisions",
        ["nonclaims"]=p.Json.StringArray(
            "No Decision/MCDM implementation, tests or business-oracle equivalence were independently observed.",
            "No public feed publication, adopted recipient request, neutral dw.discovery adapter, generated skill or AURA broker admission.",
            "No newly exported Math exact scaler, inferred VaR/CVaR policy or cryptographic PRNG contract.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.49";
        var c=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Phase);
        var t=c["tasks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Task);
        c["status"]="in_progress";
        c["refinement"]="RS050 T1 qualifies a separate .NET 10 package-only consumer of dw.quantities preview.1 and statistics preview.6, with 31 independent named oracles for exact scaling loss/overflow refusal, explicit Type-7/nearest-rank quantile conventions, exact and compensated sums, typed Pearson, Bernoulli and SplitMix64 checkpoints. Both local package identities/dependencies and SHA-256 are recorded in evidence; consumer decision proposals remain unsent and no recipient adaptation is claimed. C04-T2 retains external ownership/adoption boundary review.";
        c["readiness"]="in progress: T1 package-only consumer and proposals qualified on durable RS050; T2 recipient boundaries still planned";
        c["files"]=p.Json.StringArray(Consumer,Config,Source,Decision,Distribution,Qualified);
        c["commands"]=p.Json.StringArray(
            "dotnet restore dw.tools.math.slnx --locked-mode",
            "dotnet pack "+QuantityProject+" -c Release",
            "dotnet pack "+StatisticsProject+" -c Release",
            "dotnet restore "+Consumer+" --source build/artifacts/m3-w02-c04-local-feed",
            "dotnet run --project "+Consumer+" -c Release");
        c["evidence"]=p.Json.StringArray(Qualified);
        t["status"]="done";
        t["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in t["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });

    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Question,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Question);
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var child in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.Json.EditObject(".aura/workflow/state.json",state=>
    {
        if((string?)state["pendingRun"]!="RS050" ||
           (string?)state["activeMilestone"]!="M3" ||
           (string?)state["activeWorkPackage"]!=Work ||
           state["activePhase"] is not null ||
           state["activeTask"] is not null ||
           state["activeSubtask"] is not null)
            throw new InvalidOperationException("RS050 baseline DWF hierarchy mismatch.");
        state["activePhase"]=Phase;
    });
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
}
else
{
    foreach(var node in new[]{Task,Question,Observation,DecisionNode})
        p.ProjectPlan.RequireNodeState(node,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W02-C04-T2","not-ready");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static void InspectPackage(string path,string expectedId,string expectedVersion,string[] expectedDependencies)
{
    if(!File.Exists(path))throw new InvalidOperationException("Missing local package: "+path);
    using(var zip=ZipFile.OpenRead(path))
    {
        if(zip.GetEntry("lib/net10.0/"+expectedId+".dll") is null)
            throw new InvalidOperationException("Local package has no target framework library: "+expectedId);
        var entry=zip.Entries.Single(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase));
        using var stream=entry.Open();
        var doc=XDocument.Load(stream);
        var meta=doc.Descendants().Single(x=>x.Name.LocalName=="metadata");
        var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
        var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
        var dependencies=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
            .Select(x=>(string?)x.Attribute("id")??"").ToArray();
        if(id!=expectedId || version!=expectedVersion ||
           !dependencies.SequenceEqual(expectedDependencies,StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Local NuGet identity or dependency mismatch: "+expectedId);
    }
}
static string RequirePackageChecksum(string output,string packageFileName)
{
    var marker="C04-PKG-SHA256:"+packageFileName+":";
    var values=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(marker,StringComparison.Ordinal)).ToArray();
    if(values.Length!=1)
        throw new InvalidOperationException("One independent package SHA-256 receipt is required for "+packageFileName);
    var digest=values[0][marker.Length..];
    if(digest.Length!=64 || digest.Any(ch=>!((ch>='0' && ch<='9') || (ch>='a' && ch<='f'))))
        throw new InvalidOperationException("Invalid independent package SHA-256 receipt for "+packageFileName);
    return digest;
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2400?part[..2400]:part;
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
    if(!process.Start())throw new InvalidOperationException("Unable to start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
