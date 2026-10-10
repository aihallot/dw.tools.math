using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M3-W01", Phase="M3-W01-C02", Task="M3-W01-C02-T1";
const string Red="M3-W01-C02-T1-R", Green="M3-W01-C02-T1-G", Verify="M3-W01-C02-T1-V";
const string Src="src/projects/dw.tools.math.numerics/NumericalMatrices.cs";
const string Proj="src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
const string ProdLock="src/projects/dw.tools.math.numerics/packages.lock.json";
const string Test="tests/projects/dw.tools.math.numerics.tests/M3W01C02Tests.cs";
const string RedTest="tests/projects/dw.tools.math.numerics.tests/M3W01C02RedTests.cs";
const string TestProj="tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.numerics.tests/packages.lock.json";
const string SmokeProj="tests/providers/m3-w01-c02/MathNetInteropSmoke.csproj";
const string SmokeSource="tests/providers/m3-w01-c02/Program.cs";
const string Doc="docs/distribution/numerical-matrices.md", Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string PriorBoundary="docs/planning/evidence/M3-W01-C01-boundary-qualified.json";
const string RedProof="docs/planning/evidence/M3-W01-C02-contract-red.json";
const string Qualified="docs/planning/evidence/M3-W01-C02-contract-qualified.json";
const string Version="0.4.0-preview.1";
const int GreenTestCount=17;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Work);
var chunk=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Phase);
var t1=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Task);
var t2=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]=="M3-W01-C02-T2");
string Sub(string id)=>(string?)t1["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
bool baseline=(string?)backlog["plan_version"]=="0.1.38" &&
    (string?)work["status"]=="in_progress" && (string?)chunk["status"]=="planned" &&
    (string?)t1["status"]=="planned" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
bool target=(string?)backlog["plan_version"]=="0.1.39" &&
    (string?)work["status"]=="in_progress" && (string?)chunk["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS040 expects M3-W01-C02 0.1.38 baseline or 0.1.39 T1 target.");

var previous=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
var previousBoundary=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorBoundary)))!.AsObject();
if((string?)previous["status"]!="managed-mathnet-5.0.0-solve-stats-quadrature-smoke-qualified" ||
    (string?)previousBoundary["status"]!="managed-mathnet-runtime-rid-license-and-native-deferral-qualified" ||
    (string?)previous["selected_package_version"]!="5.0.0")
    throw new InvalidOperationException("RS039 real provider qualification missing.");
var external=backlog["external_dependencies"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]=="EXT-NUMERIC");
if((string?)external["status"]!="satisfied")
    throw new InvalidOperationException("EXT-NUMERIC is not qualified.");
var aura=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)aura["status"]!="draft" || (string?)aura["transmission"]!="none")
    throw new InvalidOperationException("Math cannot claim adoption or host permission.");

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M3-W01-C03","not-ready");

// Compile an independent RED against an empty but real numerical assembly.
// The staged production source is deliberately absent until RED is recorded.
foreach(var file in new[]{Proj,ProdLock,TestProj,TestLock,Solution,RedTest})
    p.Files.ReplaceFromStaged("staged/"+file,file);
if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M3W01C02RedTests.BoundedFiniteMatrixMustHavePublicProviderNeutralContract",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M3-W01-C02-T1 RED: provider-neutral bounded finite matrix contract is absent.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("RS040 independent absent-contract RED not observed: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W01-C02-contract-red",
        ["status"]="controlled-absent-provider-neutral-numerical-matrix-red-observed",
        ["prior_provider_qualification"]=Prior,
        ["oracle"]="Numerical matrix contract must be public, finite and provider-neutral.",
        ["red_setup"]="Compiled independent assembly reflection test against numerical project without implementation.",
        ["qualification_limit"]="The missing-contract RED does not prove arithmetic, dimensional policy, boundedness or provider conversion."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-absent-provider-neutral-numerical-matrix-red-observed")
        throw new InvalidOperationException("RS040 target re-entry requires recorded RED.");
}

foreach(var file in new[]{Src,Test,SmokeProj,SmokeSource,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var green=Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M3W01C02","--logger","console;verbosity=normal");
if(!green.Contains("Total tests: 18",StringComparison.OrdinalIgnoreCase) ||
   !green.Contains("Passed: 18",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS040 expected 1 recovered RED plus 17 independent numerical GREEN tests.");

Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","restore",SmokeProj);
Required(root,"dotnet","build",SmokeProj,"-c","Release","--no-restore");
var smoke=Required(root,"dotnet","run","--project",SmokeProj,"-c","Release","--no-build","--no-restore");
if(!smoke.Contains("M3-W01-C02:managed-mathnet-5.0.0-provider-copy-smoke:qualified",
    StringComparison.Ordinal))
    throw new InvalidOperationException("Managed Math.NET copied-array numerical smoke failed.");
Required(root,"dotnet","test",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","test",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","numerics-package-feed");
Required(root,"dotnet","pack",Proj,"-c","Release","--no-build","--no-restore","-o",feed);
var nupkg=Path.Combine(feed,"dw.tools.math.numerics."+Version+".nupkg");
if(!File.Exists(nupkg))throw new InvalidOperationException("Numerics NuGet archive missing.");
using(var zip=ZipFile.OpenRead(nupkg))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.numerics.dll") is null)
        throw new InvalidOperationException("Numerical product assembly missing from local NuGet archive.");
    var manifests=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(manifests.Length!=1)throw new InvalidOperationException("Expected one NuGet manifest.");
    using var stream=manifests[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.numerics" || version!=Version ||
       !deps.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected numerical product package version or dependency graph.");
}

var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W01-C02-contract-qualified",
    ["status"]="provider-neutral-bounded-binary64-dense-sparse-matrix-and-vector-contract-qualified",
    ["prior_provider_evidence"]=Prior,["prior_provider_boundary_evidence"]=PriorBoundary,
    ["controlled_red_evidence"]=RedProof,
    ["source"]=Src,["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["test"]=Test,["test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["red_test"]=RedTest,["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["provider_smoke_project"]=SmokeProj,
    ["provider_smoke_source"]=SmokeSource,
    ["provider_smoke_project_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,SmokeProj))),
    ["provider_smoke_source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,SmokeSource))),
    ["independent_green_tests_passed"]=GreenTestCount,
    ["prior_red_test_now_green"]=true,["controlled_red_then_green"]=true,
    ["matrix_add_oracle"]="[[1,2],[3,4]]+[[4,3],[2,1]]=[[5,5],[5,5]]",
    ["matrix_multiply_oracle"]="[[1,2],[3,4]]*[[2,0],[1,2]]=[[4,4],[10,8]]",
    ["matrix_vector_oracle"]="[[1,2],[3,4]]*[5,6]=[17,39]",
    ["maximum_matrix_elements"]=4096,["maximum_scalar_products"]=65536,
    ["precision"]="explicit finite approximate binary64",
    ["sparse_strategy"]="unique canonical row-major nonzero coordinate triples; explicit dense conversion",
    ["managed_mathnet_5_0_0_test_only_copy_bridge_qualified"]=true,
    ["package_id"]="dw.tools.math.numerics",["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["observed_package_bytes"]=new FileInfo(nupkg).Length,
    ["observed_package_sha256"]=Hash(File.ReadAllBytes(nupkg)),
    ["runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["locked_solution_build_passed"]=true,
    ["full_prior_math_regressions_passed"]=true,
    ["transfer_architecture_passed"]=true,["local_package_structure_qualified"]=true,
    ["nonclaims"]=p.Json.StringArray(
        "No general rational-to-binary64 conversion, implicit widening, or exact numerical result.",
        "No provider types or Math.NET dependencies in public numerics package.",
        "No native BLAS qualification, provider routing, unexecuted RID or cross-platform floating bitwise identity.",
        "M3-W01-C02-T2 copy/alias, explicit conversion policies and optional provider-load boundaries still planned.",
        "No AURA, Decision or MCDM adoption or repository changes.")
};
p.Files.WriteComplete(Qualified,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.39";
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Task);
    c["status"]="in_progress";
    c["refinement"]="RS040 T1 adds a standalone .NET 10 provider-neutral dw.tools.math.numerics package: bounded finite binary64 dense matrices and vectors, explicit canonical sparse coordinate strategy, shape-safe 2x2 addition/multiplication, matrix-vector oracles, approximate IR-only transport and copied provider arrays. One independently observed missing-contract RED, seventeen GREEN tests, a real managed Math.NET copied-array smoke, prior Math regressions and locked NuGet package are qualified. T2 remains planned for explicit rational-to-binary64 and stronger alias/provider-loading boundaries.";
    c["readiness"]="active T1 qualified; refine T2 exact/approximate conversion and copy/provider availability boundaries before C02 closure";
    c["files"]=p.Json.StringArray(Src,Proj,ProdLock,Test,RedTest,TestProj,TestLock,
        Solution,SmokeProj,SmokeSource,Doc,RedProof,Qualified);
    c["commands"]=p.Json.StringArray(
        "dotnet restore "+Solution+" --locked-mode",
        "dotnet build "+Solution+" -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M3W01C02",
        "dotnet run --project "+SmokeProj+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+Proj+" -c Release");
    c["evidence"]=p.Json.StringArray(RedProof,Qualified);
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedProof,Qualified);
    foreach(var child in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        child["status"]="done";
        child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedProof:Qualified);
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
p.ProjectPlan.RequireNodeState("M3-W01-C02-T2","not-ready");
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
    return s.Length>2300?s[..2300]:s;
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
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)throw new InvalidOperationException(executable+" "+
        string.Join(" ",args)+" exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
