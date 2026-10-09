using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W03-C02";
const string WorkPackage = "M1-W03";
const string Release = "M1";
const string T1 = "M1-W03-C02-T1";
const string T2 = "M1-W03-C02-T2";
const string Decision = "docs/planning/decisions/m1-w03-c02.md";
const string RecipientRequest = "docs/coordination/requests/MATH-XR-002-aura-adoption.json";
const string RecipientMatrix = "docs/coordination/matrices/m1-aura-adoption-consumers.json";
const string ReleaseRecipe = "docs/distribution/m1-release-gate.md";
const string GateValidator = "docs/planning/ValidateM1Gate.cs";
const string GateEvidence = "docs/planning/evidence/M1-W03-C02-gate.json";
const string Consumer = "tests/consumers/exact-math-smoke/exact-math-smoke.csproj";
const string ConsumerConfig = "tests/consumers/exact-math-smoke/NuGet.Config";
const string PriorReceipt = "docs/planning/evidence/M1-W03-C01-qualified.json";
const string RightsEvidence = "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json";
const string Version = "0.2.0-preview.1";
const string ConsumerMarker = "dw.tools.math/exact-consumer/0.2 qualified";

var projects = new (string Id, string Project, string[] DependsOn)[]
{
    ("dw.quantities", "src/projects/dw.quantities/dw.quantities.csproj", []),
    ("dw.quantities.expression", "src/projects/dw.quantities.expression/dw.quantities.expression.csproj", ["dw.quantities"]),
    ("dw.quantities.standard", "src/projects/dw.quantities.standard/dw.quantities.standard.csproj", ["dw.quantities","dw.quantities.expression"])
};

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var phase = backlog["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Phase);
var work = backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==WorkPackage);
var release = backlog["releases"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Release);
var tasks = phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
string Sub(JsonObject task,string suffix) =>
    (string?)task["subtasks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==(string?)task["id"]+"-"+suffix)["status"]
    ?? throw new InvalidOperationException("Missing document subtask status.");
var baseline=(string?)backlog["plan_version"]=="0.1.24" &&
    (string?)release["status"]=="in_progress" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="planned" &&
    tasks.All(x=>(string?)x["status"]=="planned") &&
    new[]{"A","B","C"}.All(s=>tasks.All(t=>Sub(t,s)=="planned"));
var target=(string?)backlog["plan_version"]=="0.1.25" &&
    (string?)release["status"]=="done" &&
    (string?)work["status"]=="done" && (string?)phase["status"]=="done" &&
    tasks.All(t=>(string?)t["status"]=="done") &&
    new[]{"A","B","C"}.All(s=>tasks.All(t=>Sub(t,s)=="done"));
if(!baseline && !target)
    throw new InvalidOperationException("RS026 lifecycle differs from the accepted M1 gate baseline/target.");

// Local authority: M1 closes only if its previously qualified product nodes
// and the owner-attested package boundaries remain fully present.
var allM1=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Where(x=>(string?)x["release"]==Release).ToArray();
if(allM1.Length!=9 || allM1.Where(x=>(string?)x["id"]!=Phase)
    .Any(x=>(string?)x["status"]!="done"))
    throw new InvalidOperationException("The eight preceding M1 product chunks are not all done.");
var receipt=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorReceipt)))!.AsObject();
if((string?)receipt["status"]!="isolated-local-package-consumer-and-provenance-qualified" ||
   receipt["local_only_consumer_restore_passed"]?.GetValue<bool>()!=true ||
   receipt["isolated_consumer_execution_passed"]?.GetValue<bool>()!=true)
    throw new InvalidOperationException("RS025 isolated consumer or package evidence is absent.");
var rights=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RightsEvidence)))!.AsObject();
if((string?)rights["source_revision"]!="82a6b435a387a7e116b47a6b2c433ae9e067bf21" ||
   (string?)rights["license_and_notices"]?["model"]!="owner-authorized proprietary code" ||
   rights["authorization"]?["package_from_dw_tools_math"]?.GetValue<bool>()!=true)
    throw new InvalidOperationException("Owner rights and version authority are not qualified.");

foreach(var doc in new[]{Decision,RecipientRequest,RecipientMatrix,ReleaseRecipe,GateValidator})
    p.Files.ReplaceFromStaged("staged/"+doc,doc);
var request=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RecipientRequest)))!.AsObject();
var matrix=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RecipientMatrix)))!.AsObject();
if((string?)request["status"]!="draft" || (string?)request["transmission"]!="none" ||
   (string?)matrix["external_adoption_state"]!="not_adopted" ||
   (string?)matrix["transmission"]!="none")
    throw new InvalidOperationException("An external adoption request must remain untransmitted and unadopted.");
var rows=matrix["rows"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
if(rows.Length!=7 || rows.Any(x=>(string?)x["adoption_state"]!="not_adopted" ||
    (string?)x["test_state"]!="not_run_in_aura"))
    throw new InvalidOperationException("The AURA recipient test matrix falsely claims external qualification.");

RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
RunRequired(root,"dotnet","test","tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-restore","--no-build","--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");

var feed = Path.Combine(root,"build","artifacts","math-package-feed");
var packageCache=Path.Combine(root,"build","artifacts","math-isolated-packages");
var metadata=new List<JsonNode>();
foreach(var (id,source,deps) in projects)
{
    RunRequired(root,"dotnet","pack",source,"-c","Release","--no-restore","--no-build","-o",feed);
    var archivePath=Path.Combine(feed,id+"."+Version+".nupkg");
    if(!File.Exists(archivePath))
        throw new InvalidOperationException("M1 local NuGet package missing: "+id);
    using var archive=ZipFile.OpenRead(archivePath);
    if(archive.GetEntry("lib/net10.0/"+id+".dll") is null)
        throw new InvalidOperationException("M1 package missing net10.0 assembly: "+id);
    var specs=archive.Entries.Where(e=>e.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)
        throw new InvalidOperationException("Expected one .nuspec for: "+id);
    using var specStream=specs[0].Open();
    var xml=XDocument.Load(specStream);
    var meta=xml.Descendants().Single(e=>e.Name.LocalName=="metadata");
    var actualId=meta.Elements().Single(e=>e.Name.LocalName=="id").Value;
    var actualVersion=meta.Elements().Single(e=>e.Name.LocalName=="version").Value;
    var actualDeps=meta.Descendants().Where(e=>e.Name.LocalName=="dependency")
        .Select(e=>(string?)e.Attribute("id")??"").ToArray();
    if(actualId!=id || actualVersion!=Version ||
       !actualDeps.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(deps) ||
       actualDeps.Length!=deps.Length)
        throw new InvalidOperationException("M1 package identity, version or dependency mismatch: "+id);

    metadata.Add(new JsonObject
    {
        ["package_id"]=id,
        ["version"]=actualVersion,
        ["sha256"]=Hash(File.ReadAllBytes(archivePath)),
        ["bytes"]=new FileInfo(archivePath).Length,
        ["dependencies"]=p.Json.StringArray(actualDeps),
        ["assembly"]="lib/net10.0/"+id+".dll",
        ["scope"]="local ignored output; neither publicly published nor installed in AURA"
    });
}
var isolatedProject=Path.Combine(root,Consumer);
var config=Path.Combine(root,ConsumerConfig);
var consumerSource=File.ReadAllText(isolatedProject);
var configText=File.ReadAllText(config);
if(consumerSource.Contains("ProjectReference",StringComparison.Ordinal) ||
   !configText.Contains("<clear />",StringComparison.Ordinal) ||
   projects.Any(x=>!consumerSource.Contains("PackageReference Include=\""+x.Id+"\"",StringComparison.Ordinal)))
    throw new InvalidOperationException("M1 gate consumer lacks package-only isolated NuGet contract.");
RunRequired(root,"dotnet","restore",isolatedProject,
    "--configfile",config,"--source",feed,"--packages",packageCache);
RunRequired(root,"dotnet","build",isolatedProject,"-c","Release","--no-restore");
var sample=RunRequired(root,"dotnet","run","--project",isolatedProject,
    "-c","Release","--no-restore","--no-build");
if(!sample.Split('\n').Any(line=>line.Trim()==ConsumerMarker))
    throw new InvalidOperationException("The independent M1 package consumer did not qualify: "+Relevant(sample));

var finalChunks=allM1.Select(x=>(string)x["id"]!).Order(StringComparer.Ordinal).ToArray();
var gate=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W03-C02-gate",
    ["status"]="local-exact-m1-gate-qualified-external-adoption-not-started",
    ["milestone"]="M1",
    ["release_version"]="0.2.0-preview",
    ["plan_version"]="0.1.25",
    ["checked_chunk_ids"]=p.Json.StringArray(finalChunks),
    ["prior_package_receipt"]=PriorReceipt,
    ["source_baseline_revision"]="82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["owner_rights_evidence"]=RightsEvidence,
    ["newly_built_packages"]=p.Json.Array(metadata.ToArray()),
    ["locked_solution_and_full_regressions_passed"]=true,
    ["packages_verified"]=true,
    ["local_consumer_execution_passed"]=true,
    ["local_consumer_output"]=ConsumerMarker,
    ["aura_adoption_request"]=RecipientRequest,
    ["aura_consumer_matrix"]=RecipientMatrix,
    ["aura_transmission"]="none",
    ["external_adoption"]="not_adopted",
    ["recipient_tests_run"]=false,
    ["gate_decision"]=Decision,
    ["release_recipe"]=ReleaseRecipe,
    ["validation_commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --no-restore --no-build",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs",
        "dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs",
        "dotnet run --file docs/planning/ValidateM1Gate.cs -- --check"),
    ["limitations"]=p.Json.StringArray(
        "Local M1 availability is a release-gate result and is not an acknowledgement, adoption or integration of Math in AURA.",
        "The AURA request is draft with transmission none; seven consumer domains remain not adopted and not tested externally.",
        "SHA-256s identify only newly observed local ignored .nupkg output; a rebuild may generate different binary hashes.",
        "No AURA/Decision/MCDM or other sibling repository source is modified.",
        "AURA retains host policies, localization, command envelopes, worker and CLI contracts until recipient-side tested cutover.")
};
p.Files.WriteComplete(GateEvidence,gate.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",document=>
{
    var result=document["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    var releaseNode=document["releases"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Release);
    var workNode=document["work_packages"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==WorkPackage);
    document["plan_version"]="0.1.25";
    result["status"]="done";
    result["refinement"]="RS026 creates a draft AURA adoption request and seven-domain consumer test matrix without transmission; independently rechecks locked Math solution and full tests, foundation chain and transfer architecture; builds/inspects three local NuGet packages and executes the isolated consumer; and qualifies the complete nine-chunk local M1 gate while explicitly excluding external adoption.";
    result["files"]=p.Json.StringArray(Decision,RecipientRequest,RecipientMatrix,
        ReleaseRecipe,GateValidator,GateEvidence);
    result["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs",
        "dotnet run --file docs/planning/ValidateM1Gate.cs -- --check",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    result["evidence"]=p.Json.StringArray(Decision,GateEvidence);
    foreach(var task in result["tasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Decision,GateEvidence);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(
                ((string?)sub["id"])!.EndsWith("-A",StringComparison.Ordinal)
                    ? RecipientRequest : ((string?)sub["id"])!.EndsWith("-B",StringComparison.Ordinal)
                        ? RecipientMatrix : GateEvidence);
        }
    }
    workNode["status"]="done";
    workNode["evidence"]=p.Json.StringArray(PriorReceipt,GateEvidence);
    releaseNode["status"]="done";
    releaseNode["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M1-W01-C04-qualified.json",
        "docs/planning/evidence/M1-W02-C03-qualified.json",
        PriorReceipt,GateEvidence);
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    foreach(var taskName in new[]{T1,T2})
    {
        p.ProjectPlan.TransitionNode(taskName,"not-ready","ready");
        foreach(var suffix in new[]{"A","B","C"})
        {
            var sub=taskName+"-"+suffix;
            p.ProjectPlan.TransitionNode(sub,"not-ready","ready");
            p.ProjectPlan.ActivateReadyContinuation(sub);
            p.ProjectPlan.ConvergeNodeToDone(sub);
        }
        p.ProjectPlan.ConvergeNodeToDone(taskName);
    }
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(WorkPackage);
    p.ProjectPlan.ConvergeNodeToDone(Release);
}
else
{
    foreach(var node in new[]{
        T1+"-A",T1+"-B",T1+"-C",T1,
        T2+"-A",T2+"-B",T2+"-C",T2,Phase,WorkPackage,Release})
        p.ProjectPlan.RequireNodeState(node,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
RunRequired(root,"dotnet","run","--file",GateValidator,"--","--check");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var relevant=index<0?output:output[index..];
    return relevant.Length>3000?relevant[..3000]:relevant;
}

static (int ExitCode,string Output) Run(string root,string executable,params string[] arguments)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var argument in arguments)process.StartInfo.ArgumentList.Add(argument);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string RunRequired(string root,string executable,params string[] arguments)
{
    var result=Run(root,executable,arguments);
    if(result.ExitCode!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",arguments)+
            " exited "+result.ExitCode+": "+Relevant(result.Output));
    return result.Output;
}
