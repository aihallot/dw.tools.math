using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C01", Task="M2-W02-C01-T2";
const string Red="M2-W02-C01-T2-R", Green="M2-W02-C01-T2-G", Verify="M2-W02-C01-T2-V";
const string Catalog="src/projects/dw.tools.math.composition/MathCapabilityCatalog.cs";
const string Contracts="src/projects/dw.tools.math.composition/OperationContracts.cs";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C01BoundaryTests.cs";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string Doc="docs/distribution/composition-results.md";
const string Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M2-W02-C01-contract-qualified.json";
const string RedProof="docs/planning/evidence/M2-W02-C01-boundary-red.json";
const string Qualified="docs/planning/evidence/M2-W02-C01-boundary-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var phase=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C01-T1");
var t2=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
string Sub(string id)=>(string?)t2["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.31" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.32" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="done" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="done" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS033 expects exactly M2-W02-C01 T2 baseline 0.1.31 or target 0.1.32.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="provider-neutral-inspectable-result-and-capability-contract-qualified" ||
    prior["independent_tests_passed"]?.GetValue<int>()!=7)
    throw new InvalidOperationException("RS032 contract proof absent.");
var adoption=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)adoption["status"]!="draft" || (string?)adoption["transmission"]!="none")
    throw new InvalidOperationException("RS033 must not assert AURA permission or adoption.");

p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,baseline?"in-progress":"done");
p.ProjectPlan.RequireNodeState("M2-W02-C01-T1","done");
p.ProjectPlan.RequireNodeState("M2-W02-C02","not-ready");

p.Files.ReplaceFromStaged("staged/"+Test,Test);
p.Files.ReplaceFromStaged("staged/"+Doc,Doc);
if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C01BoundaryTests.DiscoveryContractExistsWithoutProviderBinding",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C01-T2 RED: public provider-independent capability discovery contract is absent.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("Missing controlled discovery RED: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1, ["id"]="M2-W02-C01-boundary-red",
        ["status"]="controlled-provider-independent-discovery-red-observed",
        ["prior_contract_evidence"]=Prior,
        ["oracle"]="Discovery returns immutable descriptions without external providers, authorization or execution.",
        ["red_setup"]="New assembly consumer test fails because MathCapabilityCatalog is absent before integration.",
        ["qualification_limit"]="A controlled RED proves the missing public contract, not any external provider behavior."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-provider-independent-discovery-red-observed")
        throw new InvalidOperationException("RS033 target re-entry requires recorded RED.");
}

p.Files.ReplaceFromStaged("staged/"+Catalog,Catalog);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C01BoundaryTests","--logger","console;verbosity=normal");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","composition-package-feed");
Required(root,"dotnet","pack",Proj,"-c","Release","--no-build","--no-restore","-o",feed);
var nupkg=Path.Combine(feed,"dw.tools.math.composition."+Version+".nupkg");
if(!File.Exists(nupkg))throw new InvalidOperationException("Composition NuGet package absent.");
using(var zip=ZipFile.OpenRead(nupkg))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.composition.dll") is null)
        throw new InvalidOperationException("Public composition DLL missing from package.");
    var manifests=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(manifests.Length!=1)throw new InvalidOperationException("Unexpected number of nuspec manifests.");
    using var stream=manifests[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var ver=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.composition" || ver!=Version ||
       !deps.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected NuGet id, version or dependency graph.");
}
var proof=new JsonObject
{
    ["schema_version"]=1, ["id"]="M2-W02-C01-boundary-qualified",
    ["status"]="provider-independent-discovery-no-permission-no-final-value-qualified",
    ["prior_contract_evidence"]=Prior, ["controlled_red_evidence"]=RedProof,
    ["contract_version"]="math-composition/1",
    ["catalog_source"]=Catalog, ["catalog_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Catalog))),
    ["contracts_source"]=Contracts, ["contracts_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Contracts))),
    ["boundary_tests"]=Test, ["tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["independent_boundary_tests_passed"]=8, ["prior_contract_tests_passed"]=7,
    ["controlled_red_then_green"]=true, ["locked_solution_build_passed"]=true,
    ["full_ir_and_foundation_regressions_passed"]=true, ["transfer_architecture_passed"]=true,
    ["local_package_structure_qualified"]=true,
    ["package_id"]="dw.tools.math.composition", ["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["admission_contracts"]=p.Json.StringArray(
        "Only known provider-neutral capability descriptions are publicly discoverable as an immutable array.",
        "No public provider, permission, authorization, AURA or executor surface is exposed.",
        "Unknown and null operation ids have no discovered capability; a capability remains non-executable.",
        "Unsupported and budget outcomes retain null final values and nonempty bounded reasons.",
        "Successful final values require explicitly supplied matching-precision typed IR nodes.",
        "Provenance is bounded identity/version metadata, never an execution or authorization trace."),
    ["nonclaims"]=p.Json.StringArray(
        "No AURA host integration, permission grant, provider execution, external adoption or consumer deployment.",
        "No typed pipeline, symbolic solver, numerical computation, resource scheduling or cancellation.")
};
p.Files.WriteComplete(Qualified,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.32";
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    c["status"]="done";
    c["refinement"]="RS032 T1 provides inspectable composition results and matrix capability contract; RS033 T2 adds explicit immutable provider-neutral discovery, denies public provider/permission/execution surface, qualifies missing-final-value and provenance boundaries with controlled semantic RED and eight independent GREEN tests. No AURA host authorization or provider execution.";
    c["readiness"]="closed: T1 and T2 independently qualified; M2-W02-C02 is next, still planned";
    c["files"]=p.Json.StringArray(Contracts,Catalog,Proj,
        "src/projects/dw.tools.math.composition/packages.lock.json",
        "tests/projects/dw.tools.math.composition.tests/M2W02C01Tests.cs",Test,
        TestProj,"tests/projects/dw.tools.math.composition.tests/packages.lock.json",
        Solution,Doc,"docs/planning/evidence/M2-W02-C01-contract-red.json",Prior,RedProof,Qualified);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C01BoundaryTests",
        "dotnet test "+TestProj+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+Proj+" -c Release");
    c["evidence"]=p.Json.StringArray("docs/planning/evidence/M2-W02-C01-contract-red.json",Prior,RedProof,Qualified);
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedProof,Qualified);
    foreach(var s in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        s["status"]="done";
        s["evidence"]=p.Json.StringArray((string?)s["id"]==Red?RedProof:Qualified);
    }
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"ready","in-progress");
    foreach(var node in new[]{Red,Green,Verify})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var node in new[]{Red,Green,Verify,Task,Phase})
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
