using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase="M3-W01-C02", Task="M3-W01-C02-T2";
const string Red="M3-W01-C02-T2-R", Green="M3-W01-C02-T2-G", Verify="M3-W01-C02-T2-V";
const string Source="src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs";
const string RedTest="tests/projects/dw.tools.math.numerics.tests/M3W01C02T2RedTests.cs";
const string Tests="tests/projects/dw.tools.math.numerics.tests/M3W01C02T2Tests.cs";
const string TestProject="tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj";
const string ProductProject="src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
const string Distribution="docs/distribution/numerical-matrices.md";
const string Prior="docs/planning/evidence/M3-W01-C02-contract-qualified.json";
const string Gate="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string RedEvidence="docs/planning/evidence/M3-W01-C02-boundary-red.json";
const string Qualified="docs/planning/evidence/M3-W01-C02-boundaries-qualified.json";
const string Version="0.4.0-preview.2";
const int IndependentGreenTests=19;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
p.Workflow.RequireCurrentRun("RS041","M3","M3-W01",Phase,null,null);
p.Workflow.RequirePriorSuccess("RS040",1,
    "feat: introduce bounded provider-neutral numerical matrix contracts",
    "0.1.59",0,0,0,true);
var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)prior["status"]!="provider-neutral-bounded-binary64-dense-sparse-matrix-and-vector-contract-qualified" ||
   prior["independent_green_tests_passed"]?.GetValue<int>()!=17)
    throw new InvalidOperationException("RS040 T1 qualification is not the accepted baseline.");
var provider=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Gate)))!.AsObject();
if((string?)provider["selected_package_id"]!="MathNet.Numerics" ||
   (string?)provider["selected_package_version"]!="5.0.0")
    throw new InvalidOperationException("Numerical provider smoke baseline is missing.");

var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var release=backlog["releases"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]=="M3");
var wp=backlog["work_packages"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]=="M3-W01");
var chunk=backlog["chunks"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Phase);
var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Task);
string SubStatus(string id)=>(string?)task["subtasks"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]==id)["status"] ?? throw new InvalidOperationException(id);

var baseline=(string?)backlog["plan_version"]=="0.1.39" &&
    (string?)release["status"]=="in_progress" && (string?)wp["status"]=="in_progress" &&
    (string?)chunk["status"]=="in_progress" && (string?)task["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(id=>SubStatus(id)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.40" &&
    (string?)release["status"]=="in_progress" && (string?)wp["status"]=="in_progress" &&
    (string?)chunk["status"]=="done" && (string?)task["status"]=="done" &&
    new[]{Red,Green,Verify}.All(id=>SubStatus(id)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS041 requires exact 0.1.39 T2 baseline or completed 0.1.40 target.");
var dependency=backlog["external_dependencies"]!.AsArray().Select(v=>v!.AsObject())
    .Single(v=>(string?)v["id"]=="EXT-NUMERIC");
if((string?)dependency["status"]!="satisfied")
    throw new InvalidOperationException("External numerical package is not previously qualified.");
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,baseline?"in-progress":"done");
p.ProjectPlan.RequireNodeState(Task,baseline?"not-ready":"done");
p.ProjectPlan.RequireNodeState("M3-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");

// T2 RED: the independent reflection test is compiled against the completed
// RS040 numerical assembly before the new conversion source is installed.
p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
if(baseline)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",TestProject,"-c","Release","--no-build",
        "--no-restore","--filter",
        "FullyQualifiedName~M3W01C02T2RedTests.AgentIndependentRationalToBinary64RequiresExplicitStatusContract",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(
        "M3-W01-C02-T2 RED: explicit exact-rational binary64 conversion result contract is absent.",
        StringComparison.Ordinal))
        throw new InvalidOperationException("RS041 missing independently observed absent-contract RED: "+
            Relevant(red.Output));
    var evidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-W01-C02-boundary-red",
        ["status"]="controlled-missing-explicit-rational-binary64-contract-red-observed",
        ["baseline_t1_evidence"]=Prior,
        ["red_test"]=RedTest,
        ["scope"]="compiled absent public conversion contract, not an arithmetic or rounding proof",
        ["expected_marker"]="M3-W01-C02-T2 RED: explicit exact-rational binary64 conversion result contract is absent."
    };
    p.Files.WriteComplete(RedEvidence,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
else
{
    var redEvidence=JsonNode.Parse(File.ReadAllText(Path.Combine(root,RedEvidence)))!.AsObject();
    if((string?)redEvidence["status"]!="controlled-missing-explicit-rational-binary64-contract-red-observed")
        throw new InvalidOperationException("RS041 completed target lacks original controlled RED evidence.");
}

foreach(var file in new[]{Source,Tests,ProductProject,Distribution})
    p.Files.ReplaceFromStaged("staged/"+file,file);

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
var green=Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M3W01C02T2Tests","--logger","console;verbosity=normal");
if(!green.Contains("Total tests: 19",StringComparison.OrdinalIgnoreCase) ||
   !green.Contains("Passed: 19",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("RS041 19 independent rounding/boundary GREEN oracles not confirmed.");
var allNumerics=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
if(!allNumerics.Contains("Passed:",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("All numerical tests did not emit an admission count.");
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

// Packaging is local, intentionally not a public NuGet publication.
// The Math product must carry only an IR project dependency; managed
// Math.NET remains a separate test-only consumer reference.
var feed=Path.Combine(root,"build","artifacts","m3-c02-t2-local-feed");
Required(root,"dotnet","pack",ProductProject,"-c","Release","--no-build",
    "--no-restore","-o",feed);
var archive=Path.Combine(feed,"dw.tools.math.numerics."+Version+".nupkg");
if(!File.Exists(archive))throw new InvalidOperationException("Versioned numerical package not produced.");
using(var zip=ZipFile.OpenRead(archive))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.numerics.dll") is null)
        throw new InvalidOperationException("Versioned package lacks its numerical assembly.");
    var nuspecs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspecs.Length!=1)throw new InvalidOperationException("Expected one versioned NuGet manifest.");
    using var stream=nuspecs[0].Open();
    var xml=XDocument.Load(stream);
    var meta=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=meta.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=meta.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=meta.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.numerics" || version!=Version ||
        !deps.SequenceEqual(new[]{"dw.tools.math.ir"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Numerical package has unexpected version/provider dependencies.");
}
var sdk=Required(root,"dotnet","--version").Trim();
var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M3-W01-C02-boundaries-qualified",
    ["status"]="explicit-correctly-rounded-rational-binary64-and-copy-only-boundaries-qualified",
    ["baseline_t1_evidence"]=Prior,["provider_baseline_evidence"]=Gate,
    ["controlled_red_evidence"]=RedEvidence,
    ["source"]=Source,["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Source))),
    ["green_test"]=Tests,["green_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Tests))),
    ["red_test"]=RedTest,["red_test_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,RedTest))),
    ["independent_green_tests_passed"]=IndependentGreenTests,
    ["prior_red_test_now_green"]=true,["red_then_green"]=true,
    ["rounding"]="IEEE 754 binary64 nearest, ties to even; exact rational difference returned",
    ["outcomes"]=p.Json.StringArray("Exact","Rounded","Underflow","Overflow","BudgetExceeded"),
    ["maximum_operand_bits"]=4096,
    ["underflow_and_overflow_refuse_partial_values"]=true,
    ["matrix_exact_only_and_allow_rounded_policies_qualified"]=true,
    ["matrix_partial_conversion_refused"]=true,
    ["copy_in_copy_out_no_provider_views"]=true,
    ["dotnet_sdk"]=sdk,
    ["runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["locked_solution_build_passed"]=true,["prior_math_regressions_passed"]=true,
    ["nuget_product_version"]=Version,
    ["nuget_package_sha256"]=Hash(File.ReadAllBytes(archive)),
    ["nuget_package_bytes"]=new FileInfo(archive).Length,
    ["direct_package_dependencies"]=p.Json.StringArray("dw.tools.math.ir"),
    ["mathnet_only_in_independent_test_consumer"]=true,
    ["no_AURA_or_Decision_dependency"]=true,
    ["nonclaims"]=p.Json.StringArray(
        "No public NuGet publication, native BLAS or unexecuted platform admission.",
        "No implicit rational-to-double widening, algebraic exactness after a rounded conversion, or permission to call third-party providers.",
        "No mutable provider-array views or shared mutable matrix aliases.",
        "No generated Math skill, shared dw.discovery package or AURA broker admission.",
        "Only the bounded M3-W01-C02 scientific contract is qualified by this run.")
};
p.Files.WriteComplete(Qualified,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.40";
    var c=plan["chunks"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(v=>v!.AsObject()).Single(v=>(string?)v["id"]==Task);
    c["status"]="done";
    c["refinement"]="RS040 qualified provider-neutral bounded dense/sparse finite binary64 matrix and vector contracts; RS041 closes T2 with an independently observed RED followed by nineteen hand/binary IEEE 754 GREEN cases. Explicit correctly-rounded rational-to-binary64 conversion reports exact absolute loss, exact-only or rounded matrix policies, refusal on underflow/overflow/budget, no partial matrix, copy-only provider arrays, and local provider-neutral NuGet preview.2 packaging.";
    c["readiness"]="closed: both T1 and T2 accepted; M3-W01-C03 remains not-ready for solving and decompositions";
    var existingPaths=c["files"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["files"]=p.Json.StringArray(existingPaths.Concat(new[]{Source,RedTest,Tests,ProductProject,Distribution,RedEvidence,Qualified})
        .Distinct(StringComparer.Ordinal).ToArray());
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M3W01C02T2",
        "dotnet test "+TestProject+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs",
        "dotnet pack "+ProductProject+" -c Release");
    var existingEvidence=c["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
    c["evidence"]=p.Json.StringArray(existingEvidence.Concat(new[]{RedEvidence,Qualified})
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
    foreach(var node in new[]{Task,Red,Green,Verify,Phase})
        p.ProjectPlan.RequireNodeState(node,"done");
}
p.ProjectPlan.RequireNodeState("M3","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","in-progress");
p.ProjectPlan.RequireNodeState("M3-W01-C03","not-ready");
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
    if(result.Code!=0)throw new InvalidOperationException(executable+" "+
        string.Join(" ",args)+" exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
