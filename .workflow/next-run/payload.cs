using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M2-W02", Phase="M2-W02-C03", Task="M2-W02-C03-T2";
const string Red="M2-W02-C03-T2-R", Green="M2-W02-C03-T2-G", Verify="M2-W02-C03-T2-V";
const string Src="src/projects/dw.tools.math.composition/ExactReplayContracts.cs";
const string Proj="src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string TestProj="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string Test="tests/projects/dw.tools.math.composition.tests/M2W02C03BoundaryTests.cs";
const string RedTest="tests/projects/dw.tools.math.composition.tests/M2W02C03BoundaryRedTests.cs";
const string Doc="docs/distribution/exact-replay-cache.md", Solution="dw.tools.math.slnx";
const string Prior="docs/planning/evidence/M2-W02-C03-contract-qualified.json";
const string RedProof="docs/planning/evidence/M2-W02-C03-boundary-red.json";
const string GreenProof="docs/planning/evidence/M2-W02-C03-boundary-qualified.json";
const string Version="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var phase=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2-W02-C03-T1");
var t2=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
string Sub(string id)=>(string?)t2["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.35" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="in_progress" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.36" &&
    (string?)work["status"]=="done" && (string?)phase["status"]=="done" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="done" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS037 requires 0.1.35 C03 T2 baseline or exact 0.1.36 W02 qualified target.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="versioned-exact-replay-identity-and-bounded-local-cache-qualified" ||
    prior["independent_green_tests_passed"]?.GetValue<int>()!=14)
    throw new InvalidOperationException("RS036 replay qualification absent.");
var adoption=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)adoption["status"]!="draft" || (string?)adoption["transmission"]!="none")
    throw new InvalidOperationException("RS037 cannot claim AURA adoption.");

p.ProjectPlan.RequireNodeState("M2","in-progress");
p.ProjectPlan.RequireNodeState("M2-W01","done");
p.ProjectPlan.RequireNodeState("M2-W02-C01","done");
p.ProjectPlan.RequireNodeState("M2-W02-C02","done");
p.ProjectPlan.RequireNodeState(Work,baseline?"in-progress":"done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"in-progress":"done");
p.ProjectPlan.RequireNodeState("M2-W02-C03-T1","done");

p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W02C03BoundaryRedTests.IncompleteReplayMustHaveASeparateNonFinalPublicContract",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M2-W02-C03-T2 RED: public explicit non-final replay status contract is missing.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("Controlled non-final result RED absent: "+Relevant(red.Output));
    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M2-W02-C03-boundary-red",
        ["status"]="controlled-nonfinal-replay-status-contract-red-observed",
        ["prior_contract_evidence"]=Prior,
        ["oracle"]="Replay needs a typed non-final outcome that cannot be cached or misrepresented as an exact final value.",
        ["red_setup"]="Independent public API reflection test fails before installing TryRun and ExactReplayAttempt.",
        ["qualification_limit"]="Contract-absence RED does not alone qualify cache integrity, cancellation, or policy separation."
    };
    p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedProof)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-nonfinal-replay-status-contract-red-observed")
        throw new InvalidOperationException("Target re-entry lacks targeted RED.");
}

foreach(var file in new[]{Src,Test,Doc})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
Required(root,"dotnet","test",TestProj,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2W02C03Boundary","--logger","console;verbosity=normal");
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
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
    if(id!="dw.tools.math.composition" || version!=Version ||
        !deps.SequenceEqual(new[]{"dw.quantities","dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Composition NuGet identity or dependencies changed.");
}
var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M2-W02-C03-boundary-qualified",
    ["status"]="explicit-nonfinal-exact-replay-cache-quota-and-portability-boundaries-qualified",
    ["prior_contract_evidence"]=Prior,["controlled_red_evidence"]=RedProof,
    ["source"]=Src,["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Src))),
    ["red_test"]=RedTest,["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["boundary_tests"]=Test,["boundary_tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Test))),
    ["replay_scheme"]="exact-quantity-replay/1-sha256",
    ["independent_boundary_green_tests_passed"]=13,
    ["prior_replay_green_tests_passed"]=14,["controlled_red_then_green"]=true,
    ["locked_solution_build_passed"]=true,["composition_and_ir_regressions_passed"]=true,
    ["foundation_chain_passed"]=true,["transfer_architecture_passed"]=true,
    ["local_package_qualified"]=true,
    ["cache_maximum_entries"]=32,["pipeline_maximum_steps"]=16,
    ["package_id"]="dw.tools.math.composition",["package_version"]=Version,
    ["package_dependencies"]=p.Json.StringArray("dw.quantities","dw.tools.math.ir"),
    ["admission_contracts"]=p.Json.StringArray(
        "Exact replay attempt status is explicit: Exact, Unsupported, BudgetExceeded or Cancelled.",
        "Only Exact contains one completed result receipt; rejected or cancelled work has no partial final numerical value.",
        "Optional cache stores completed exact results only, with caller-owned volatile FIFO quota of at most 32 entries.",
        "Cancellation, invalid dimensions, zero divisors, excessive steps, numerals and exponent overflow retain bounded failure reasons and no cached result.",
        "Provider/catalog version changes invalidate exact cache identity independently of their values.",
        "No binary64 cross-platform reproducibility or provider authorization is inferred."),
    ["nonclaims"]=p.Json.StringArray(
        "The caller owns semantic context version correctness; a hash is not an algebraic equivalence proof.",
        "No binary64 bit-for-bit portability claim, provider execution, AURA permission or external adoption.",
        "Cancellation remains cooperative, without forced preemption of blocking iterators or arithmetic.",
        "Milestone M2 remains in progress pending its independent gate qualification.")
};
p.Files.WriteComplete(GreenProof,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.36";
    var w=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    w["status"]="done";
    w["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M2-W02-C01-boundary-qualified.json",
        "docs/planning/evidence/M2-W02-C02-boundary-qualified.json",GreenProof);
    c["status"]="done";
    c["refinement"]="RS036 T1 qualifies versioned deterministic exact replay identity and optional bounded FIFO cache. RS037 T2 adds explicit Exact/Unsupported/BudgetExceeded/Cancelled result statuses without a fabricated partial final value, cache quota/eviction, cancellation and invalid-input nonadmission, and independent RED/GREEN boundary tests. W02 is done; M2 is not asserted complete before its independent gate.";
    c["readiness"]="closed: T1 and T2 qualified; M2 gate pending separate final review";
    c["files"]=p.Json.StringArray(Src,Proj,RedTest,Test,
        "tests/projects/dw.tools.math.composition.tests/M2W02C03RedTests.cs",
        "tests/projects/dw.tools.math.composition.tests/M2W02C03Tests.cs",
        TestProj,Doc,"docs/planning/evidence/M2-W02-C03-contract-red.json",Prior,
        RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProj+" -c Release --filter FullyQualifiedName~M2W02C03",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+Proj+" -c Release");
    c["evidence"]=p.Json.StringArray("docs/planning/evidence/M2-W02-C03-contract-red.json",
        Prior,RedProof,GreenProof);
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedProof,GreenProof);
    foreach(var sub in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedProof:GreenProof);
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
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
else
{
    foreach(var node in new[]{Red,Green,Verify,Task,Phase,Work})
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
