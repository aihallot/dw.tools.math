using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C02", Task="M2-W02-C02-T2";
const string Red="M2-W02-C02-T2-R", Green="M2-W02-C02-T2-G", Verify="M2-W02-C02-T2-V";
const string Src="src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C02BoundaryTests.cs";
const string RedTest="tests/projects/dw.tools.math.composition.tests/M2W02C02BoundaryRedTests.cs";
const string LegacyRedTest="tests/projects/dw.tools.math.composition.tests/M2W02C02RedTests.cs";
const string Doc="docs/distribution/exact-quantity-pipeline.md", Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M2-W02-C02-contract-qualified.json";
const string RedProof="docs/planning/evidence/M2-W02-C02-boundary-red.json";
const string GreenProof="docs/planning/evidence/M2-W02-C02-boundary-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var phase=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C02-T1");
var t2=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
string Sub(string id)=>(string?)t2["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.33" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.34" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="done" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="done" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS035 requires 0.1.33 C02 T2 baseline or 0.1.34 C02 done target.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="bounded-exact-direct-fluent-quantity-pipeline-qualified" ||
    prior["independent_green_tests_passed"]?.GetValue<int>()!=10)
    throw new InvalidOperationException("RS034 exact pipeline qualification missing.");
var adoption=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)adoption["status"]!="draft" || (string?)adoption["transmission"]!="none")
    throw new InvalidOperationException("RS035 does not grant or transmit AURA adoption.");

p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M2-W02-C01","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"in-progress":"done");
p.ProjectPlan.RequireNodeState("M2-W02-C02-T1","done");
p.ProjectPlan.RequireNodeState("M2-W02-C03","not-ready");

p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
p.Files.ReplaceFromStaged("staged/"+LegacyRedTest,LegacyRedTest);
if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C02BoundaryRedTests.Positive257DigitNumeratorMustBeRefusedBeforeEvaluation",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C02-T2 RED: 257-digit positive numerator wrongly passed the 256-digit bound.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("Controlled numeral bound RED not observed: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1, ["id"]="M2-W02-C02-boundary-red",
        ["status"]="controlled-positive-257-digit-numeral-budget-red-observed",
        ["prior_contract_evidence"]=Prior,
        ["oracle"]="Exact numerator magnitude of 257 decimal digits must be rejected at the advertised 256-digit boundary.",
        ["red_setup"]="An independent compiled test exercised the existing pipeline before applying the corrected magnitude bound.",
        ["qualification_limit"]="The targeted RED alone does not qualify cancellation or provenance."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-positive-257-digit-numeral-budget-red-observed")
        throw new InvalidOperationException("Target re-entry lacks controlled RED evidence.");
}

foreach(var file in new[]{Src,Test,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C02Boundary","--logger","console;verbosity=normal");
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
    ["schema_version"]=1, ["id"]="M2-W02-C02-boundary-qualified",
    ["status"]="bounded-cancellable-step-provenance-exact-pipeline-qualified",
    ["prior_contract_evidence"]=Prior, ["controlled_red_evidence"]=RedProof,
    ["source"]=Src, ["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["boundary_red_test"]=RedTest,["boundary_red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["previous_t1_red_test_compatibility_fix"]=LegacyRedTest,
    ["previous_t1_red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,LegacyRedTest))),
    ["boundary_tests"]=Test,["boundary_tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["contract_version"]="exact-quantity-pipeline/1",
    ["controlled_semantic_red_then_green"]=true, ["independent_boundary_green_tests_passed"]=10,
    ["prior_pipeline_tests_passed"]=10, ["locked_solution_build_passed"]=true,
    ["composition_and_ir_regressions_passed"]=true, ["foundation_chain_passed"]=true,
    ["transfer_architecture_passed"]=true, ["local_package_qualified"]=true,
    ["admission_limits"]=new JsonObject { ["maximum_steps"]=16, ["maximum_numeral_magnitude_digits"]=256 },
    ["package_id"]="dw.tools.math.composition", ["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.quantities","dw.tools.math.ir"),
    ["contracts"]=p.Json.StringArray(
        "Previously qualified reflection tests select exact direct/fluent method signatures to remain valid with cancellation overloads.",
        "Direct and fluent cancellation-token overloads share the validated evaluator; cancellation throws without returning a partial result.",
        "Finite immutable per-step snapshots retain index, kind, exact base quantity, dimensions and optional unit id.",
        "Input, scale, intermediate and presentation rational magnitudes refuse 257-digit numerator or denominator.",
        "No provider admission, AURA host permission, external execution or distributed scheduler is involved."),
    ["nonclaims"]=p.Json.StringArray(
        "Cancellation is cooperative; arbitrary blocking iterators and uninterrupted arithmetic cannot be forcibly preempted.",
        "Provenance snapshots describe only local numerical state, not external provider telemetry or authorization.",
        "M2-W02-C03 remains planned; no symbolic solver, provider dispatch or AURA adoption.")
};
p.Files.WriteComplete(GreenProof,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.34";
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    c["status"]="done";
    c["refinement"]="RS034 T1 qualifies the immutable direct/fluent exact rational quantity pipeline with unit/dimension prevalidation and 150 km/h acceptance. RS035 T2 adds cooperative cancellation, exact per-stage bounded immutable numerical provenance, correct magnitude bounds for positive 257-digit numerals, and independent RED/GREEN integration tests. Neither AURA authority nor external execution is inferred.";
    c["readiness"]="closed: both T1 contract and T2 boundary/integration qualified; M2-W02-C03 remains planned";
    c["files"]=p.Json.StringArray(Src,Proj,RedTest,Test,
        LegacyRedTest,
        "tests/projects/dw.tools.math.composition.tests/M2W02C02Tests.cs",
        TestProj,Doc,
        "docs/planning/evidence/M2-W02-C02-contract-red.json",Prior,RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C02",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+Proj+" -c Release");
    c["evidence"]=p.Json.StringArray("docs/planning/evidence/M2-W02-C02-contract-red.json",
        Prior,RedProof,GreenProof);
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
