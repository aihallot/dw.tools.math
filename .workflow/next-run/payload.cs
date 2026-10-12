using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase="M3-W01-C03", Task="M3-W01-C03-T1";
const string Red="M3-W01-C03-T1-R", Green="M3-W01-C03-T1-G", Verify="M3-W01-C03-T1-V";
const string Source="src/projects/dw.tools.math.numerics/BoundedLinearAlgebra64.cs";
const string RedTest="tests/projects/dw.tools.math.numerics.tests/M3W01C03RedTests.cs";
const string Tests="tests/projects/dw.tools.math.numerics.tests/M3W01C03Tests.cs";
const string TestProject="tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj";
const string ProductProject="src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
const string SmokeProject="tests/providers/m3-w01-c03/ManagedLinearAlgebraSmoke.csproj";
const string SmokeSource="tests/providers/m3-w01-c03/Program.cs";
const string Distribution="docs/distribution/bounded-linear-algebra.md";
const string Prior="docs/planning/evidence/M3-W01-C02-boundaries-qualified.json";
const string Gate="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string RedEvidence="docs/planning/evidence/M3-W01-C03-contract-red.json";
const string Qualified="docs/planning/evidence/M3-W01-C03-contract-qualified.json";
const string Version="0.4.0-preview.3";
const int IndependentGreenTests=25;
const string ResultCommitMessage="feat: add bounded provider-neutral LU QR and spectral contracts";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
p.Workflow.RequireCurrentRun("RS042","M3","M3-W01",null,null,null);
p.Workflow.RequirePriorSuccess("RS041",1,
    "feat: qualify explicit rational-to-binary64 numerical boundaries",
    "0.1.59",2,2,0,false);

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="explicit-correctly-rounded-rational-binary64-and-copy-only-boundaries-qualified"
    || prior["independent_green_tests_passed"]?.GetValue<int>()!=19)
    throw new InvalidOperationException("RS041 C02 boundary qualification is missing or divergent.");
var provider=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Gate)))!.AsObject();
if((string?)provider["selected_package_id"]!="MathNet.Numerics"
    || (string?)provider["selected_package_version"]!="5.0.0")
    throw new InvalidOperationException("The managed Math.NET provider is not pinned and qualified.");

var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var release=backlog["releases"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]=="M3");
var wp=backlog["work_packages"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]=="M3-W01");
var priorChunk=backlog["chunks"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]=="M3-W01-C02");
var chunk=backlog["chunks"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]==Phase);
var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]==Task);
var nextTask=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]=="M3-W01-C03-T2");
string SubStatus(string id)=>(string?)task["subtasks"]!.AsArray()
    .Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==id)["status"]
    ??throw new InvalidOperationException("Missing C03 subtask "+id);

var baseline=(string?)backlog["plan_version"]=="0.1.40"
    && (string?)release["status"]=="in_progress"
    && (string?)wp["status"]=="in_progress"
    && (string?)priorChunk["status"]=="done"
    && (string?)chunk["status"]=="planned"
    && (string?)task["status"]=="planned"
    && (string?)nextTask["status"]=="planned"
    && new[]{Red,Green,Verify}.All(id=>SubStatus(id)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.41"
    && (string?)release["status"]=="in_progress"
    && (string?)wp["status"]=="in_progress"
    && (string?)priorChunk["status"]=="done"
    && (string?)chunk["status"]=="in_progress"
    && (string?)task["status"]=="done"
    && (string?)nextTask["status"]=="planned"
    && new[]{Red,Green,Verify}.All(id=>SubStatus(id)=="done");
if(!baseline&&!target)
    throw new InvalidOperationException("RS042 expects exact C03 T1 baseline 0.1.40 or target 0.1.41.");

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState(Task,baseline?"not-ready":"done");
p.ProjectPlan.RequireNodeState("M3-W01-C03-T2","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");

// Independent RED is compiled against the already-qualified 0.4.0-preview.2
// assembly before any new linear algebra implementation is installed.
p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release","--no-build",
        "--no-restore","--filter",
        "FullyQualifiedName~M3W01C03RedTests.BoundedProviderNeutralLinearAlgebraMustBeExplicitlyImplemented",
        "--logger","console;verbosity=normal");
    const string marker="M3-W01-C03-T1 RED: bounded provider-neutral LU solve QR and singular spectrum contract is absent.";
    if(observed.Code==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS042 independent missing-contract RED not observed: "+
            Relevant(observed.Output));
    var record=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W01-C03-contract-red",
        ["status"]="controlled-provider-neutral-lu-qr-and-spectral-contract-absent-red-observed",
        ["prior_numerical_evidence"]=Prior,["red_test"]=RedTest,
        ["expected_marker"]=marker,
        ["scope"]="One separately compiled public-contract RED before installing any candidate numerical implementation."
    };
    p.Files.WriteComplete(RedEvidence,record.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var existing=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedEvidence)))!.AsObject();
    if((string?)existing["status"]!="controlled-provider-neutral-lu-qr-and-spectral-contract-absent-red-observed")
        throw new InvalidOperationException("Exact completed target lacks the controlled RED evidence.");
}

// Make the public API and the isolated, test-only provider smoke available.
foreach(var file in new[]{Source,Tests,ProductProject,SmokeProject,SmokeSource,Distribution})
    p.Files.ReplaceFromStaged("staged/"+file,file);

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
var green=Required(root,"dotnet","test",TestProject,"-c","Release","--no-build",
    "--no-restore","--filter","FullyQualifiedName~M3W01C03Tests",
    "--logger","console;verbosity=normal");
if(!green.Contains("Total tests: 25",StringComparison.OrdinalIgnoreCase) ||
   !green.Contains("Passed: 25",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS042 25 independent LU/QR/singular-spectrum GREEN oracles not confirmed.");
var numerics=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
if(!numerics.Contains("Passed:    64",StringComparison.OrdinalIgnoreCase)
    && !numerics.Contains("Total:    64",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Numerical regression expected 64 tests, including recovered RED and 25 GREEN.");

var smoke=Required(root,"dotnet","run","--project",SmokeProject,"-c","Release");
if(!smoke.Contains("M3-W01-C03:independent-managed-lu-qr-svd-smoke:qualified",
    StringComparison.Ordinal))
    throw new InvalidOperationException("Managed Math.NET LU/QR/SVD corroboration failed.");

// Scope-risk requires exact prior mathematical regressions; no repository CI.
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

var feed=Path.Combine(root,"build","artifacts","m3-c03-t1-local-feed");
Required(root,"dotnet","pack",ProductProject,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.numerics."+Version+".nupkg");
if(!File.Exists(archive))
    throw new InvalidOperationException("RS042 versioned product package missing.");
using(var zip=ZipFile.OpenRead(archive))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.numerics.dll") is null)
        throw new InvalidOperationException("RS042 package lacks its numerical assembly.");
    var nuspecs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspecs.Length!=1)throw new InvalidOperationException("Expected one NuGet manifest.");
    using var stream=nuspecs[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var packageId=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var packageVersion=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(packageId!="dw.tools.math.numerics" || packageVersion!=Version
        || !deps.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("RS042 product introduced a provider dependency or wrong version.");
}
var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W01-C03-contract-qualified",
    ["status"]="bounded-dense-lu-qr-and-analytical-2x2-singular-spectrum-qualified",
    ["prior_M3_W01_C02_evidence"]=Prior,
    ["provider_baseline_evidence"]=Gate,
    ["controlled_red_evidence"]=RedEvidence,
    ["source"]=Source,
    ["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Source))),
    ["green_test"]=Tests,
    ["green_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Tests))),
    ["red_test"]=RedTest,
    ["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["provider_smoke_project"]=SmokeProject,
    ["provider_smoke_source"]=SmokeSource,
    ["provider_smoke_source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,SmokeSource))),
    ["independent_green_tests_passed"]=IndependentGreenTests,
    ["numerical_regression_tests_passed"]=64,
    ["controlled_red_then_green"]=true,
    ["lu_partial_pivot_PA_equals_LU"]=true,
    ["solve_hand_oracle"]="[[2,1],[1,3]]*[1,2]=[4,7]",
    ["singularity_does_not_claim_unique_solution"]=true,
    ["qr_full_and_rectangular_reconstruction"]=true,
    ["qr_orthogonality_invariant_checked"]=true,
    ["singular_spectrum_2x2_only"]=true,
    ["managed_mathnet_5_0_0_LU_QR_SVD_smoke_qualified_test_only"]=true,
    ["max_dimension"]=16,
    ["default_relative_pivot_rank_threshold"]=1e-12,
    ["absolute_and_relative_residual_reported"]=true,
    ["copy_in_copy_out_only"]=true,
    ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
    ["runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["locked_solution_build_passed"]=true,
    ["prior_math_regressions_passed"]=true,
    ["nuget_product_version"]=Version,
    ["nuget_package_sha256"]=Hash(File.ReadAllBytes(archive)),
    ["nuget_package_bytes"]=new FileInfo(archive).Length,
    ["direct_product_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["nonclaims"]=p.Json.StringArray(
        "No full public SVD factorization or arbitrary-size singular vectors; the independent provider full-SVD test does not admit a new Math API.",
        "No platform except the actually observed RID, no native BLAS, no unconditional condition-number accuracy or robust unpivoted QR rank theorem.",
        "No AURA, Decision, dw.discovery, generated skill or BrokerReady adoption.",
        "M3-W01-C03-T2 still requires independent rank/pivot/conditioning and provider-loading boundary qualification.")
};
p.Files.WriteComplete(Qualified,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.41";
    var c=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Task);
    c["status"]="in_progress";
    c["refinement"]="RS042 T1 supplies bounded provider-neutral partial-pivot LU, exact row-permutation reconstruction, dense solve with residuals, unpivoted full-Q Householder QR, and analytical 2x2 singular values; a real managed Math.NET 5.0.0 smoke independently checks LU/QR/full SVD without becoming a runtime dependency. Controlled missing-contract RED and 25 independent GREEN tests plus existing numerical/math regressions and NuGet preview.3 package are qualification evidence. C03-T2 remains planned for rank/pivot/ill-conditioning and full-SVD admission decisions.";
    c["readiness"]="T1 qualified; C03-T2 still planned: refine conditioning, rank/pivot policy, optional managed provider loading and full SVD limitations before C03 closure";
    var existingPaths=c["files"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["files"]=p.Json.StringArray(existingPaths.Concat(new[]{Source,RedTest,Tests,
        ProductProject,SmokeProject,SmokeSource,Distribution,RedEvidence,Qualified})
        .Distinct(StringComparer.Ordinal).ToArray());
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W01C03Tests",
        "dotnet run --project "+SmokeProject+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+ProductProject+" -c Release");
    var previous=c["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["evidence"]=p.Json.StringArray(previous.Concat(new[]{RedEvidence,Qualified})
        .Distinct(StringComparer.Ordinal).ToArray());
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    foreach(var sub in t["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedEvidence:Qualified);
    }
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var node in new[]{Green,Verify})
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
    foreach(var node in new[]{Task,Red,Green,Verify})
        p.ProjectPlan.RequireNodeState(node,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C03-T2","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS042"
        ||(string?)state["activeMilestone"]!="M3"
        ||(string?)state["activeWorkPackage"]!="M3-W01"
        ||(string?)state["activeTask"] is not null
        ||(string?)state["activeSubtask"] is not null
        ||(string?)state["activePhase"] is not (null or Phase))
        throw new InvalidOperationException("RS042 active workflow state diverged.");
    state["activePhase"]=Phase;
});

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
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
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
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
