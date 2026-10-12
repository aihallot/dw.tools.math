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

const string Phase="M3-W01-C03", Task="M3-W01-C03-T2";
const string Red="M3-W01-C03-T2-R", Green="M3-W01-C03-T2-G", Verify="M3-W01-C03-T2-V";
const string Work="M3-W01";
const string Source="src/projects/dw.tools.math.numerics/BoundedLinearAlgebra64.cs";
const string RedTest="tests/projects/dw.tools.math.numerics.tests/M3W01C03T2RedTests.cs";
const string Tests="tests/projects/dw.tools.math.numerics.tests/M3W01C03T2Tests.cs";
const string TestProject="tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj";
const string ProductProject="src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
const string Distribution="docs/distribution/bounded-linear-algebra.md";
const string Decision="docs/planning/decisions/m3-w01-c03-svd-scope.md";
const string Prior="docs/planning/evidence/M3-W01-C03-contract-qualified.json";
const string PriorNumerics="docs/planning/evidence/M3-W01-C02-boundaries-qualified.json";
const string Gate="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string Smoke="tests/providers/m3-w01-c03/ManagedLinearAlgebraSmoke.csproj";
const string RedEvidence="docs/planning/evidence/M3-W01-C03-boundary-red.json";
const string Qualified="docs/planning/evidence/M3-W01-C03-boundaries-qualified.json";
const string Version="0.4.0-preview.4";
const string Backlog="docs/planning/backlog.json";
const int IndependentGreenTests=16;
const int ExpectedNumericsTests=81;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
// Presence of a qualified T2 receipt selects completed target re-entry.
// All authority values are then verified through bounded Payload SDK contracts.
var target=File.Exists(Path.Combine(root,Qualified));
var baseline=!target;
p.Workflow.RequireCurrentRun("RS043","M3",target?null:Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS042",1,
    "feat: add bounded provider-neutral LU QR and spectral contracts",
    "0.1.59",2,2,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-dense-lu-qr-and-analytical-2x2-singular-spectrum-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(25));
p.Require.JsonValueEquals(Prior,"/numerical_regression_tests_passed",JsonValue.Create(64));
p.Require.JsonValueEquals(PriorNumerics,"/status",
    JsonValue.Create("explicit-correctly-rounded-rational-binary64-and-copy-only-boundaries-qualified"));
p.Require.JsonValueEquals(Gate,"/selected_package_id",JsonValue.Create("MathNet.Numerics"));
p.Require.JsonValueEquals(Gate,"/selected_package_version",JsonValue.Create("5.0.0"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.41"),JsonValue.Create("0.1.42"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.42":"0.1.41"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/7/status",JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/20/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/21/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/22/status",JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/22/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/22/tasks/1/status",JsonValue.Create(target?"done":"planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/22/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/23/status",JsonValue.Create("planned"));
if(target)
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("bounded-2x2-condition-availability-rank-pivot-policy-qualified"));
}
else
{
    p.Require.FileMissing(Qualified);
    p.Require.FileMissing(RedEvidence);
}

p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C01","done");
p.ProjectPlan.RequireNodeState("M3-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");

// Controlled RED: compile against qualified preview.3 without the new
// spectral-condition property; only the reflection-based RED is installed.
var installedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var observed=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore","--filter",
        "FullyQualifiedName~M3W01C03T2RedTests.AUniqueTwoByTwoSolveMustExposeConditionAvailability",
        "--logger","console;verbosity=normal");
    const string marker="M3-W01-C03-T2 RED: bounded optional 2x2 condition estimate missing from solver result.";
    if(observed.Code==0||!observed.Output.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("RS043 expected independent condition-availability RED absent: "+
            Relevant(observed.Output));
    var redReceipt=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W01-C03-boundary-red",
        ["status"]="controlled-optional-solve-condition-result-contract-absent-red-observed",
        ["prior_C03_T1_evidence"]=Prior,["red_test"]=RedTest,
        ["expected_marker"]=marker,
        ["scope"]="Independent reflection RED against preview.3 before installing any modified solver implementation."
    };
    p.Files.WriteComplete(RedEvidence,redReceipt.ToJsonString(
        new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    p.Require.JsonValueEquals(RedEvidence,"/status",
        JsonValue.Create("controlled-optional-solve-condition-result-contract-absent-red-observed"));
}

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Source,Tests,ProductProject,Distribution,Decision})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    digests[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("SDK staged-file SHA missing for "+file);
}

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");

var green=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M3W01C03T2Tests",
    "--logger","console;verbosity=normal");
if(!green.Contains("Total tests: 16",StringComparison.OrdinalIgnoreCase)
    ||!green.Contains("Passed: 16",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS043 must pass exactly 16 independent condition/rank boundary oracles.");

var numerics=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=normal");
if(!numerics.Contains("Total tests: 81",StringComparison.OrdinalIgnoreCase)
    ||!numerics.Contains("Passed: 81",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS043 numerical boundary closure requires 81 passing tests.");

var smoke=Required(root,"dotnet","run","--project",Smoke,"-c","Release");
if(!smoke.Contains("M3-W01-C03:independent-managed-lu-qr-svd-smoke:qualified",
    StringComparison.Ordinal))
    throw new InvalidOperationException("Managed provider optional LU/QR/SVD corroboration missing.");

// Completing the accepted numerical work package is a scientific trust
// boundary, so replay the previously qualified composition, IR and exact
// quantity regressions after the focused RED/GREEN numerical gates.
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

var feed=Path.Combine(root,"build","artifacts","m3-c03-t2-local-feed");
Required(root,"dotnet","pack",ProductProject,"-c","Release",
    "--no-build","--no-restore","-o",feed);
var package=Path.Combine(feed,"dw.tools.math.numerics."+Version+".nupkg");
if(!File.Exists(package))
    throw new InvalidOperationException("RS043 local package is missing.");
using(var zip=ZipFile.OpenRead(package))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.numerics.dll") is null)
        throw new InvalidOperationException("Numerics package lacks its compiled assembly.");
    var nuspecs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspecs.Length!=1)throw new InvalidOperationException("Expected one NuGet manifest.");
    using var stream=nuspecs[0].Open();
    var xml=XDocument.Load(stream);
    var metadata=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var packageId=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var dependencies=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(packageId!="dw.tools.math.numerics"||version!=Version||
        !dependencies.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Package version or dependency boundary drifted.");
}

var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W01-C03-boundaries-qualified",
    ["status"]="bounded-2x2-condition-availability-rank-pivot-policy-qualified",
    ["prior_C03_T1_evidence"]=Prior,
    ["prior_C02_boundary_evidence"]=PriorNumerics,
    ["managed_provider_qualified_evidence"]=Gate,
    ["controlled_red_evidence"]=RedEvidence,
    ["source"]=Source,["source_sha256"]=digests[Source],
    ["green_test"]=Tests,["green_test_sha256"]=digests[Tests],
    ["red_test"]=RedTest,["red_test_sha256"]=installedRed.AfterSha256,
    ["decision"]=Decision,["decision_sha256"]=digests[Decision],
    ["distribution"]=Distribution,["distribution_sha256"]=digests[Distribution],
    ["independent_green_tests_passed"]=IndependentGreenTests,
    ["numerical_regressions_passed"]=ExpectedNumericsTests,
    ["controlled_red_then_green"]=true,
    ["condition_number_scope"]="Approximate optional 2x2 spectral condition estimate only.",
    ["one_by_one_and_higher_dimension_condition_status"]="unavailable-null",
    ["ill_conditioned_spectrum_can_remain_unavailable"]=true,
    ["unavailable_relative_residual_never_fabricated"]=true,
    ["rank_pivot_threshold_policy"]="relative maximum entry magnitude; 1e-12 default, caller factor in (0,1e-6]",
    ["qr_reconstruction_and_orthogonality_not_vector_sign"]=true,
    ["explicit_inverse_avoided_for_solve"]=true,
    ["managed_provider_independent_smoke_passed"]=true,
    ["copy_only_no_provider_aliases"]=true,
    ["qualified_dimension_cap"]=16,
    ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
    ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["package_id"]="dw.tools.math.numerics",
    ["package_version"]=Version,
    ["package_bytes"]=new FileInfo(package).Length,
    ["nuspec_dependency_boundary_passed"]=true,
    ["direct_product_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["full_public_svd_admission"]="deferred",
    ["nonclaims"]=p.Json.StringArray(
        "No full public SVD, pseudoinverse, arbitrary precision condition number or rank-revealing pivoted QR.",
        "No mathematical proof of exact rank or low forward error from a small residual.",
        "No native BLAS, unexecuted RID, public NuGet publication or provider auto-loading.",
        "No dw.discovery skill generation, BrokerReady, Decision, MCDM or Aura integration.")
};
p.Files.WriteComplete(Qualified,
    proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject(Backlog,plan=>
{
    plan["plan_version"]="0.1.42";
    var wp=plan["work_packages"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Work);
    var chunk=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Phase);
    var t=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
        .Single(v=>(string?)v["id"]==Task);
    wp["status"]="done";
    chunk["status"]="done";
    chunk["refinement"]="RS042 qualified bounded partial-pivot LU, dense residual solve, Householder QR and limited 2x2 singular values. RS043 closes T2 with a controlled missing-condition-contract RED, 16 independent condition/rank/scale/copy GREEN tests, 81 numerical regressions, managed Math.NET corroboration and provider-neutral NuGet preview.4 packaging. Optional spectral 2x2 solve condition is null when unavailable; full public SVD and rank-revealing QR remain explicitly deferred by a reviewed scope decision.";
    chunk["readiness"]="closed: both T1 and T2 accepted; all M3-W01 numerical phases completed, M3-W02 remains planned";
    var files=chunk["files"]!.AsArray().Select(v=>v!.GetValue<string>());
    chunk["files"]=p.Json.StringArray(files.Concat(new[]{Source,RedTest,Tests,
        ProductProject,Distribution,Decision,RedEvidence,Qualified})
        .Distinct(StringComparer.Ordinal).ToArray());
    chunk["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W01C03T2Tests",
        "dotnet test "+TestProject+" -c Release",
        "dotnet run --project "+Smoke+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet pack "+ProductProject+" -c Release");
    var previous=chunk["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
    chunk["evidence"]=p.Json.StringArray(previous.Concat(new[]{RedEvidence,Qualified})
        .Distinct(StringComparer.Ordinal).ToArray());
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    foreach(var child in t["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
    {
        child["status"]="done";
        child["evidence"]=p.Json.StringArray((string?)child["id"]==Red?RedEvidence:Qualified);
    }
});

if(baseline)
{
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var child in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
else
{
    foreach(var id in new[]{Task,Red,Green,Verify,Phase,Work})
        p.ProjectPlan.RequireNodeState(id,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");

Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var slice=i<0?output:output[i..];
    return slice.Length>2400?slice[..2400]:slice;
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
