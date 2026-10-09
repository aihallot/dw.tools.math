using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C02", Task="M2-W02-C02-T1";
const string Red="M2-W02-C02-T1-R", Green="M2-W02-C02-T1-G", Verify="M2-W02-C02-T1-V";
const string Src="src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.composition.tests/packages.lock.json";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C02Tests.cs";
const string RedTest="tests/projects/dw.tools.math.composition.tests/M2W02C02RedTests.cs";
const string Doc="docs/distribution/exact-quantity-pipeline.md";
const string Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M2-W02-C01-boundary-qualified.json";
const string RedProof="docs/planning/evidence/M2-W02-C02-contract-red.json";
const string GreenProof="docs/planning/evidence/M2-W02-C02-contract-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var phase=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
var t2=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C02-T2");
string Sub(string id)=>(string?)t1["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.32" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="planned" &&
    (string?)t1["status"]=="planned" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.33" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS034 expects 0.1.32 untouched C02 or 0.1.33 qualified T1.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="provider-independent-discovery-no-permission-no-final-value-qualified" ||
    prior["independent_boundary_tests_passed"]?.GetValue<int>()!=8)
    throw new InvalidOperationException("RS033 qualified composition boundary proof absent.");
var adoption=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)adoption["status"]!="draft" || (string?)adoption["transmission"]!="none")
    throw new InvalidOperationException("RS034 cannot infer or transmit AURA adoption.");

p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M2-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState("M2-W02-C03","not-ready");
p.ProjectPlan.RequireNodeState("M2-W02-C02-T2","not-ready");

foreach(var file in new[]{Proj,TestLock,RedTest})
    p.Files.ReplaceFromStaged("staged/"+file,file);

if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C02RedTests.ExactQuantityPipelineMustExposeOneSharedDirectAndFluentEvaluator",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C02-T1 RED: public exact quantity pipeline direct/fluent contract is absent.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("Controlled direct/fluent semantic RED absent: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1, ["id"]="M2-W02-C02-contract-red",
        ["status"]="controlled-shared-direct-fluent-pipeline-red-observed",
        ["prior_composition_evidence"]=Prior,
        ["oracle"]="A single public exact quantity pipeline supports both direct and fluent evaluation.",
        ["red_setup"]="The independent compiled RED test checks for the absent shared public pipeline API before source installation.",
        ["qualification_limit"]="Missing-contract RED does not prove arithmetic or dimensional safety."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-shared-direct-fluent-pipeline-red-observed")
        throw new InvalidOperationException("Target re-entry needs controlled RED proof.");
}

foreach(var file in new[]{Src,Test,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C02","--logger","console;verbosity=normal");
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
        throw new InvalidOperationException("Composition DLL absent from NuGet package.");
    var manifests=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(manifests.Length!=1)throw new InvalidOperationException("Unexpected NuGet manifest count.");
    using var stream=manifests[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"")
        .OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
    if(id!="dw.tools.math.composition" || version!=Version ||
        !deps.SequenceEqual(new[]{"dw.quantities","dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected NuGet dependency graph.");
}
var proof=new JsonObject
{
    ["schema_version"]=1, ["id"]="M2-W02-C02-contract-qualified",
    ["status"]="bounded-exact-direct-fluent-quantity-pipeline-qualified",
    ["prior_composition_evidence"]=Prior, ["controlled_red_evidence"]=RedProof,
    ["source"]=Src, ["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["red_test"]=RedTest, ["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["tests"]=Test, ["tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["contract_version"]="exact-quantity-pipeline/1",
    ["independent_green_tests_passed"]=10, ["controlled_red_then_green"]=true,
    ["direct_fluent_equivalence_qualified"]=true,
    ["example"]="5 km / 2 min = 150 km/h exactly; base unit result 125/3 m/s",
    ["locked_solution_build_passed"]=true,["prior_composition_and_ir_regressions_passed"]=true,
    ["foundation_chain_passed"]=true, ["transfer_architecture_passed"]=true,
    ["local_package_structure_qualified"]=true,
    ["package_id"]="dw.tools.math.composition", ["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.quantities","dw.tools.math.ir"),
    ["admission_limits"]=new JsonObject { ["maximum_steps"]=16, ["maximum_exact_numeral_digits"]=256 },
    ["nonclaims"]=p.Json.StringArray(
        "M2-W02-C02-T2 remains planned for deeper cancellation, resource and step-provenance boundaries.",
        "No symbolic solver, expression parser, distributed scheduler, provider execution, AURA permission or adoption.",
        "Only explicit linear, zero-offset unit definitions are admitted; absolute affine temperatures are refused.")
};
p.Files.WriteComplete(GreenProof,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.33";
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    c["status"]="in_progress";
    c["refinement"]="RS034 T1 introduces the bounded exact linear quantity pipeline with single direct/fluent evaluator, full dimension pre-validation, immutable bounded step history, explicit UnitDefinition injection and 5 km / 2 min = exactly 150 km/h acceptance. Controlled RED, ten independent GREEN tests, IR/composition/foundation regressions and local NuGet packaging. T2 remains planned for deeper operational boundaries.";
    c["readiness"]="active T1 qualified; complete independent T2 boundaries before C02 closure";
    c["files"]=p.Json.StringArray(Src,Proj,TestLock,RedTest,Test,Doc,RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C02",
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
