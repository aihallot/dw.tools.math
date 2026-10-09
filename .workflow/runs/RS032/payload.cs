using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C01", Task="M2-W02-C01-T1";
const string Red="M2-W02-C01-T1-R", Green="M2-W02-C01-T1-G", Verify="M2-W02-C01-T1-V";
const string Src="src/projects/dw.tools.math.composition/OperationContracts.cs";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string ProdLock="src/projects/dw.tools.math.composition/packages.lock.json";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C01Tests.cs";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.composition.tests/packages.lock.json";
const string Solution="dw.tools.math.slnx", Doc="docs/distribution/composition-results.md";
const string RedProof="docs/planning/evidence/M2-W02-C01-contract-red.json";
const string GreenProof="docs/planning/evidence/M2-W02-C01-contract-qualified.json";
const string Prior="docs/planning/evidence/M2-W01-C03-boundary-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var plan=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var work=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
var t2=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C01-T2");
string Sub(string id)=>(string?)task["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing subtask "+id);
var baseline=(string?)plan["plan_version"]=="0.1.30" &&
    (string?)work["status"]=="planned" && (string?)chunk["status"]=="planned" &&
    (string?)task["status"]=="planned" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)plan["plan_version"]=="0.1.31" &&
    (string?)work["status"]=="in_progress" && (string?)chunk["status"]=="in_progress" &&
    (string?)task["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS032 is neither the exact 0.1.30 baseline nor target 0.1.31.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="bounded-closed-ir-canonical-json-and-structural-hashes-qualified" ||
    prior["independent_adversarial_tests_passed"]?.GetValue<int>()!=11)
    throw new InvalidOperationException("RS031 qualified typed IR proof absent.");
var aura=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)aura["status"]!="draft" || (string?)aura["transmission"]!="none")
    throw new InvalidOperationException("RS032 cannot claim external AURA adoption.");

foreach(var file in new[]{Proj,ProdLock,TestProj,TestLock,Test,Solution,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
if(baseline)
{
    var source=File.ReadAllText(Path.Combine(p.RunRoot,"staged",Src));
    const string accepted="ImmutableArray.Create(MathValueKind.Matrix)";
    if(source.Split(accepted,StringSplitOptions.None).Length!=2)
        throw new InvalidOperationException("Controlled matrix RED anchor mismatch.");
    p.Files.WriteComplete(Src,source.Replace(accepted,
        "ImmutableArray.Create(MathValueKind.Scalar)",StringComparison.Ordinal));
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C01Tests.MatrixInspectionAdvertisesAdmittedShapeAndPrecision",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C01-T1 RED: matrix inspection must admit an exact matrix shape.",StringComparison.Ordinal))
        throw new InvalidOperationException("Controlled semantic RED absent: "+Relevant(red.Output));
}
var redEvidence=new JsonObject
{
    ["schema_version"]=1, ["id"]="M2-W02-C01-contract-red",
    ["status"]="controlled-matrix-capability-red-observed",
    ["previous_ir_evidence"]=Prior,
    ["oracle"]="Matrix inspection must admit an admitted exact 2x2 scalar matrix.",
    ["red_setup"]="Tested a temporary composition capability misadvertising Scalar rather than Matrix as its admitted input kind.",
    ["qualification_limit"]="Controlled RED is not a complete proof of consumer behavior."
};
p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Files.ReplaceFromStaged("staged/"+Src,Src);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C01Tests","--logger","console;verbosity=normal");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","composition-package-feed");
Required(root,"dotnet","pack",Proj,"-c","Release","--no-build","--no-restore","-o",feed);
var nupkg=Path.Combine(feed,"dw.tools.math.composition."+Version+".nupkg");
if(!File.Exists(nupkg))throw new InvalidOperationException("Composition package absent.");
using(var zip=ZipFile.OpenRead(nupkg))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.composition.dll") is null)
        throw new InvalidOperationException("Composition assembly missing from package.");
    var manifests=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(manifests.Length!=1)throw new InvalidOperationException("Exactly one composition manifest required.");
    using var stream=manifests[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.composition" || version!=Version ||
        !deps.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected package identity or dependency graph.");
}

var proof=new JsonObject
{
    ["schema_version"]=1, ["id"]="M2-W02-C01-contract-qualified",
    ["status"]="provider-neutral-inspectable-result-and-capability-contract-qualified",
    ["prior_ir_evidence"]=Prior,["red_evidence"]=RedProof,
    ["contract_version"]="math-composition/1",
    ["source"]=Src,["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["tests"]=Test,["tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["package_id"]="dw.tools.math.composition",["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["independent_tests_passed"]=7,["controlled_red_then_green"]=true,
    ["full_ir_and_foundation_regression_passed"]=true,
    ["locked_solution_build_passed"]=true,["package_structure_qualified"]=true,
    ["statuses"]=p.Json.StringArray("Exact","Approximate","Unsupported","BudgetExceeded"),
    ["nonclaims"]=p.Json.StringArray(
        "No provider dispatch, AURA host permission, consumer adoption or external integration.",
        "No symbolic solver, numerical execution, global binder, scheduler, cancellation or execution trace.",
        "M2-W02-C01-T2 integration and missing-result boundaries are still planned.")
};
p.Files.WriteComplete(GreenProof,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",backlog=>
{
    backlog["plan_version"]="0.1.31";
    var w=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
    var c=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    w["status"]="in_progress";
    c["status"]="in_progress";
    c["refinement"]="RS032 qualifies a standalone composition/1 result and capability contract: bounded matrix shapes, typed exact/approximate/unsupported/budget statuses, missing final value and versioned provenance. Seven contract tests and a controlled semantic RED verify T1. T2 remains planned; no provider execution or permission follows from discovery.";
    c["readiness"]="active T1 qualified; refine and implement T2 independently before C01 closure";
    c["files"]=p.Json.StringArray(Src,Proj,ProdLock,Test,TestProj,TestLock,Solution,Doc,RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C01Tests",
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
    foreach(var node in new[]{Work,Phase,Task})
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
    p.ProjectPlan.RequireNodeState(Work,"in-progress");
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
