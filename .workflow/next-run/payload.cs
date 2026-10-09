using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C03", Task="M2-W02-C03-T1";
const string Red="M2-W02-C03-T1-R", Green="M2-W02-C03-T1-G", Verify="M2-W02-C03-T1-V";
const string Src="src/projects/dw.tools.math.composition/ExactReplayContracts.cs";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C03Tests.cs";
const string RedTest="tests/projects/dw.tools.math.composition.tests/M2W02C03RedTests.cs";
const string Doc="docs/distribution/exact-replay-cache.md", Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M2-W02-C02-boundary-qualified.json";
const string RedProof="docs/planning/evidence/M2-W02-C03-contract-red.json";
const string GreenProof="docs/planning/evidence/M2-W02-C03-contract-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var phase=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
var t2=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C03-T2");
string Sub(string id)=>(string?)t1["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.34" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="planned" &&
    (string?)t1["status"]=="planned" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.35" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS036 requires exactly 0.1.34 C03 baseline or 0.1.35 T1 target.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="bounded-cancellable-step-provenance-exact-pipeline-qualified" ||
    prior["independent_boundary_green_tests_passed"]?.GetValue<int>()!=10)
    throw new InvalidOperationException("RS035 qualified exact pipeline proof absent.");
var adoption=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)adoption["status"]!="draft" || (string?)adoption["transmission"]!="none")
    throw new InvalidOperationException("RS036 cannot infer AURA adoption.");

p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M2-W02-C01","done");
p.ProjectPlan.RequireNodeState("M2-W02-C02","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState("M2-W02-C03-T2","not-ready");

p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C03RedTests.ReplayContractMustExposeDeterministicIdentityWithoutAProvider",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C03-T1 RED: exact canonical replay identity contract is missing.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("Controlled replay contract RED absent: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1, ["id"]="M2-W02-C03-contract-red",
        ["status"]="controlled-absent-versioned-replay-contract-red-observed",
        ["prior_exact_pipeline_evidence"]=Prior,
        ["oracle"]="A public deterministic exact replay entrypoint is needed before any replay identity or cache can be qualified.",
        ["red_setup"]="An independently compiled reflection-based test fails because ExactReplayRunner is not installed before the GREEN source.",
        ["qualification_limit"]="This RED establishes contract absence only; it does not prove key separation or cache correctness."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-absent-versioned-replay-contract-red-observed")
        throw new InvalidOperationException("Target re-entry lacks replay RED proof.");
}

foreach(var file in new[]{Src,Test,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C03","--logger","console;verbosity=normal");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","composition-package-feed");
Required(root,"dotnet","pack",Proj,"-c","Release","--no-build","--no-restore","-o",feed);
var nupkg=Path.Combine(feed,"dw.tools.math.composition."+Version+".nupkg");
if(!File.Exists(nupkg))throw new InvalidOperationException("Composition package missing.");
using(var zip=ZipFile.OpenRead(nupkg))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.composition.dll") is null)
        throw new InvalidOperationException("Composition DLL absent.");
    var manifests=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(manifests.Length!=1)throw new InvalidOperationException("Unexpected NuGet manifest count.");
    using var stream=manifests[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var ver=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"")
        .OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
    if(id!="dw.tools.math.composition" || ver!=Version ||
        !deps.SequenceEqual(new[]{"dw.quantities","dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Composition package dependency graph changed.");
}
var proof=new JsonObject
{
    ["schema_version"]=1, ["id"]="M2-W02-C03-contract-qualified",
    ["status"]="versioned-exact-replay-identity-and-bounded-local-cache-qualified",
    ["prior_exact_pipeline_evidence"]=Prior, ["controlled_red_evidence"]=RedProof,
    ["source"]=Src, ["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["red_test"]=RedTest, ["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["tests"]=Test, ["tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["replay_scheme"]="exact-quantity-replay/1-sha256",
    ["independent_green_tests_passed"]=14,["controlled_red_then_green"]=true,
    ["locked_solution_build_passed"]=true,["composition_and_ir_regressions_passed"]=true,
    ["foundation_chain_passed"]=true,["transfer_architecture_passed"]=true,
    ["local_package_qualified"]=true,
    ["cache_maximum_entries"]=32,["pipeline_maximum_steps"]=16,
    ["package_id"]="dw.tools.math.composition",["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.quantities","dw.tools.math.ir"),
    ["identity_dimensions"]=p.Json.StringArray(
        "ordered exact step sequence, rational magnitudes and unit identity/scales/dimensions",
        "assumption version","calculation policy version","unit catalog version",
        "tolerance policy version","provider identity version"),
    ["nonclaims"]=p.Json.StringArray(
        "Provider identity is an opaque caller value, not provider availability, authorization or execution.",
        "Cache is bounded, volatile, opt-in and in-process; no persistent or remote cache.",
        "No guarantee of bit-identical binary64 portability, external execution, AURA adoption or trust.",
        "M2-W02-C03-T2 remains planned for adversarial partial/eviction/quota and replay integration checks.")
};
p.Files.WriteComplete(GreenProof,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.35";
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    c["status"]="in_progress";
    c["refinement"]="RS036 T1 qualifies domain-separated deterministic SHA-256 exact replay identities for ordered typed quantity pipeline steps and explicit assumption, calculation, catalog, tolerance, provider version labels, plus an optional volatile FIFO cache of at most 32 successes. One controlled public-contract RED and 14 independent GREEN tests cover key separation, replay and cache behavior, with all local product regressions. T2 remains planned for deeper quota, partial replay and portability boundaries.";
    c["readiness"]="active T1 qualified; independently falsify T2 partial, quota and compatibility boundaries before phase closure";
    c["files"]=p.Json.StringArray(Src,Proj,RedTest,Test,TestProj,Doc,RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C03",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+Proj+" -c Release");
    c["evidence"]=p.Json.StringArray(RedProof,GreenProof);
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedProof,GreenProof);
    foreach(var s in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        s["status"]="done";
        s["evidence"]=p.Json.StringArray((string?)s["id"]==Red?RedProof:GreenProof);
    }
});
if(baseline)
{
    foreach(var node in new[]{Phase,Task})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
    }
    foreach(var node in new[]{Red,Green,Verify})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
else
{
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
    foreach(var node in new[]{Red,Green,Verify,Task})
        p.ProjectPlan.RequireNodeState(node,"done");
}
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var s=i<0?output:output[i..];
    return s.Length>2500?s[..2500]:s;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
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
static void Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)throw new InvalidOperationException(executable+" "+
        string.Join(" ",args)+" exited "+result.Code+": "+Relevant(result.Output));
}
