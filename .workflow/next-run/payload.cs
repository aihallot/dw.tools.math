using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Release="M3", Work="M3-W01", Phase="M3-W01-C01";
const string Task1="M3-W01-C01-T1", Task2="M3-W01-C01-T2";
const string SmokeProject="tests/providers/m3-w01-c01/MathNetSmoke.csproj";
const string SmokeSource="tests/providers/m3-w01-c01/Program.cs";
const string Decision="docs/planning/decisions/m3-w01-c01.md";
const string Inventory="docs/planning/decisions/m3-w01-c01-candidates.json";
const string ProviderProof="docs/planning/evidence/M3-W01-C01-provider-qualified.json";
const string BoundaryProof="docs/planning/evidence/M3-W01-C01-boundary-qualified.json";
const string PriorGate="docs/planning/evidence/M2-gate.json";
const string ProviderPackage="MathNet.Numerics", ProviderVersion="5.0.0";
const string AlternativePackage="Meta.Numerics", AlternativeVersion="4.2.0";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var release=backlog["releases"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Release);
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
var chunk=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task1);
var t2=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task2);
var ext=backlog["external_dependencies"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]=="EXT-NUMERIC");
bool AllSubs(JsonObject task,string status) =>
    task["subtasks"]!.AsArray().All(s=>(string?)s!["status"]==status);
bool baseline=(string?)backlog["plan_version"]=="0.1.37" &&
    (string?)release["status"]=="planned" && (string?)work["status"]=="planned" &&
    (string?)chunk["status"]=="planned" &&
    (string?)t1["status"]=="planned" && (string?)t2["status"]=="planned" &&
    AllSubs(t1,"planned") && AllSubs(t2,"planned") &&
    (string?)ext["status"]=="unverified";
bool target=(string?)backlog["plan_version"]=="0.1.38" &&
    (string?)release["status"]=="in_progress" && (string?)work["status"]=="in_progress" &&
    (string?)chunk["status"]=="done" &&
    (string?)t1["status"]=="done" && (string?)t2["status"]=="done" &&
    AllSubs(t1,"done") && AllSubs(t2,"done") &&
    (string?)ext["status"]=="satisfied";
if(!baseline && !target)
    throw new InvalidOperationException("RS039 requires exact M3 0.1.37 baseline or 0.1.38 qualified C01 target.");

var previous=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorGate)))!.AsObject();
if((string?)previous["status"]!="local-m2-typed-ir-composition-and-isolated-consumer-qualified" ||
    previous["gate_tests_passed"]?.GetValue<int>()!=8)
    throw new InvalidOperationException("Qualified RS038 M2 release gate not available.");
var aura=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)aura["status"]!="draft" || (string?)aura["transmission"]!="none")
    throw new InvalidOperationException("Math numerical provider spike cannot assert external AURA adoption.");
p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState(Release,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState(Work,baseline?"not-ready":"in-progress");
p.ProjectPlan.RequireNodeState(Phase,baseline?"not-ready":"done");
p.ProjectPlan.RequireNodeState("M3-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M3-W02","not-ready");

foreach(var file in new[]{SmokeProject,SmokeSource,Decision,Inventory})
    p.Files.ReplaceFromStaged("staged/"+file,file);
var inventory=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Inventory)))!.AsObject();
if((string?)inventory["selected_provisional"]?["package_id"]!=ProviderPackage ||
    (string?)inventory["selected_provisional"]?["version"]!=ProviderVersion ||
    (string?)inventory["alternative"]?["package_id"]!=AlternativePackage ||
    (string?)inventory["alternative"]?["version"]!=AlternativeVersion)
    throw new InvalidOperationException("Pinned candidate inventory differs from M3 acceptance.");

var projectXml=XDocument.Load(Path.Combine(root,SmokeProject));
var references=projectXml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!=ProviderPackage ||
   (string?)references[0].Attribute("Version")!="[5.0.0]" ||
   projectXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Numerical smoke fixture must have one exact Math.NET dependency only.");
Required(root,"dotnet","restore",SmokeProject);
Required(root,"dotnet","build",SmokeProject,"-c","Release","--no-restore");
var smoke=Required(root,"dotnet","run","--project",SmokeProject,"-c","Release",
    "--no-build","--no-restore");
foreach(var marker in new[]{
    "smoke-managed-provider:pass:","smoke-matrix:pass","smoke-statistics:pass",
    "smoke-integration:pass","smoke-independent-parallel:pass",
    "smoke-net10-managed:M3-W01-C01:qualified"})
    if(!smoke.Contains(marker,StringComparison.Ordinal))
        throw new InvalidOperationException("Missing independent Math.NET real-package smoke oracle: "+marker);

var assetsPath=Path.Combine(root,"build","artifacts","obj","MathNetSmoke","project.assets.json");
if(!File.Exists(assetsPath))throw new InvalidOperationException("NuGet package asset graph missing.");
var assets=JsonNode.Parse(File.ReadAllText(assetsPath))!.AsObject();
var libraries=assets["libraries"]!.AsObject();
var packages=libraries.Where(x=>(string?)x.Value?["type"]=="package")
    .Select(x=>x.Key).OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
if(!packages.SequenceEqual(new[]{"MathNet.Numerics/5.0.0"},StringComparer.OrdinalIgnoreCase))
    throw new InvalidOperationException("Unexpected transitive package graph: "+
        string.Join(", ",packages));
var packageRoot=assets["packageFolders"]!.AsObject().Select(kv=>kv.Key)
    .Select(folder=>Path.Combine(folder,"mathnet.numerics","5.0.0"))
    .FirstOrDefault(dir=>File.Exists(Path.Combine(dir,"mathnet.numerics.nuspec")));
if(packageRoot is null)throw new InvalidOperationException("Restored Math.NET nuspec not available.");
var nuspecPath=Path.Combine(packageRoot,"mathnet.numerics.nuspec");
var nuspec=XDocument.Load(nuspecPath);
var metadata=nuspec.Descendants().Single(e=>e.Name.LocalName=="metadata");
var packageId=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
var packageVersion=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
var license=metadata.Elements().FirstOrDefault(e=>e.Name.LocalName=="license")?.Value.Trim();
if(packageId!=ProviderPackage || packageVersion!=ProviderVersion || license!="MIT")
    throw new InvalidOperationException("Math.NET restored package identity or MIT license mismatch.");
var pkgPath=Path.Combine(packageRoot,"mathnet.numerics.5.0.0.nupkg");
if(!File.Exists(pkgPath))throw new InvalidOperationException("Restored Math.NET package bytes missing.");
var packageSha=Hash(File.ReadAllBytes(pkgPath));
var packageBytes=new FileInfo(pkgPath).Length;

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
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
var sdk=Required(root,"dotnet","--version").Trim();

var selection=new JsonObject {
    ["schema_version"]=1,["id"]="M3-W01-C01-provider-qualified",
    ["status"]="managed-mathnet-5.0.0-solve-stats-quadrature-smoke-qualified",
    ["prior_m2_gate"]=PriorGate,["plan_target_version"]="0.1.38",
    ["selected_package_id"]=ProviderPackage,["selected_package_version"]=ProviderVersion,
    ["observed_package_bytes"]=packageBytes,["observed_package_sha256"]=packageSha,
    ["package_license_expression"]=license,
    ["nuget_dependency_graph"]=p.Json.StringArray(packages),
    ["observed_transitive_package_count"]=packages.Length-1,
    ["smoke_project"]=SmokeProject,["smoke_project_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,SmokeProject))),
    ["smoke_source"]=SmokeSource,["smoke_source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,SmokeSource))),
    ["smoke_matrix_2x2_solve_and_residual_passed"]=true,
    ["smoke_mean_sample_population_variance_passed"]=true,
    ["smoke_gauss_legendre_quadrature_passed"]=true,
    ["smoke_managed_provider_selection_passed"]=true,
    ["smoke_32_independent_concurrent_solves_passed"]=true,
    ["runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["runtime_os"]=RuntimeInformation.OSDescription,
    ["runtime_architecture"]=RuntimeInformation.ProcessArchitecture.ToString(),
    ["dotnet_sdk"]=sdk,["candidate_inventory"]=Inventory,["decision"]=Decision,
    ["alternative_package_id"]=AlternativePackage,
    ["alternative_package_version"]=AlternativeVersion,
    ["alternative_runtime_smoke"]="not_executed",
    ["previous_math_regressions_passed"]=true,
    ["scope"]="managed Math.NET candidate on this observed host, not new Math public API"
};
p.Files.WriteComplete(ProviderProof,selection.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
var boundary=new JsonObject {
    ["schema_version"]=1,["id"]="M3-W01-C01-boundary-qualified",
    ["status"]="managed-mathnet-runtime-rid-license-and-native-deferral-qualified",
    ["provider_evidence"]=ProviderProof,
    ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
    ["observed_package_sha256"]=packageSha,
    ["license_expression"]=license,
    ["managed_backend_forced_and_observed"]=true,
    ["separate_native_blas_qualified"]=false,
    ["unexecuted_rids_qualified"]=false,
    ["alternative_runtime_qualified"]=false,
    ["thread_safety_scope"]="32 independent simultaneous solves, no shared mutable matrix or global provider switching",
    ["cancellation_scope"]="no general cancellation demonstrated for third-party numerical kernels",
    ["precision_scope"]="approximate binary64; no implicit rational-to-double widening",
    ["decision"]=Decision,["inventory"]=Inventory,
    ["nonclaims"]=p.Json.StringArray(
        "No native MKL/OpenBLAS/CUDA package installed or native BLAS qualified.",
        "No Linux/macOS or unexecuted Windows RID smoke or cross-platform bitwise reproducibility.",
        "No blanket concurrent/shared mutable resource safety or third-party kernel cancellation guarantee.",
        "No public numerical adapter shipped, provider binding, AURA permission or external adoption.")
};
p.Files.WriteComplete(BoundaryProof,boundary.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    plan["plan_version"]="0.1.38";
    var rel=plan["releases"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Release);
    var wp=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    rel["status"]="in_progress";
    wp["status"]="in_progress";
    c["status"]="done";
    c["refinement"]="RS039 independently compares managed Math.NET Numerics 5.0.0 (MIT) to metadata-only Meta.Numerics 4.2.0 (MS-PL), qualifies a genuine .NET 10 real-package managed smoke of 2x2 solve/residual, mean/variance and Gauss-Legendre integration, pins observed NuGet graph/hash/license/runtime, and restricts thread and cancellation claims. Both spike tasks are complete, without creating a provider adapter or qualifying native BLAS.";
    c["readiness"]="closed: managed Math.NET package selected for a future optional adapter, with explicit native and RID nonclaims";
    c["files"]=p.Json.StringArray(SmokeProject,SmokeSource,Decision,Inventory,ProviderProof,BoundaryProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore "+SmokeProject,
        "dotnet build "+SmokeProject+" -c Release --no-restore",
        "dotnet run --project "+SmokeProject+" -c Release --no-build --no-restore",
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    c["evidence"]=p.Json.StringArray(ProviderProof,BoundaryProof);
    foreach(var task in c["tasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        var isPrimary=(string?)task["id"]==Task1;
        var evidence=isPrimary?ProviderProof:BoundaryProof;
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(evidence);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(evidence);
        }
    }
    var dependency=plan["external_dependencies"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]=="EXT-NUMERIC");
    dependency["status"]="satisfied";
    dependency["evidence"]=p.Json.StringArray(ProviderProof,BoundaryProof);
});
if(baseline)
{
    foreach(var node in new[]{Release,Work,Phase})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
    }
    foreach(var task in new[]{Task1,Task2})
    {
        p.ProjectPlan.TransitionNode(task,"not-ready","ready");
        p.ProjectPlan.TransitionNode(task,"ready","in-progress");
        foreach(var child in new[]{"A","B","C"})
        {
            var id=task+"-"+child;
            p.ProjectPlan.TransitionNode(id,"not-ready","ready");
            p.ProjectPlan.TransitionNode(id,"ready","in-progress");
            p.ProjectPlan.ConvergeNodeToDone(id);
        }
        p.ProjectPlan.ConvergeNodeToDone(task);
    }
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    p.ProjectPlan.RequireNodeState(Release,"in-progress");
    p.ProjectPlan.RequireNodeState(Work,"in-progress");
    foreach(var task in new[]{Task1,Task2})
    {
        foreach(var child in new[]{"A","B","C"})
            p.ProjectPlan.RequireNodeState(task+"-"+child,"done");
        p.ProjectPlan.RequireNodeState(task,"done");
    }
    p.ProjectPlan.RequireNodeState(Phase,"done");
}
p.ProjectPlan.RequireNodeState("M3-W01-C02","not-ready");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] value)=>Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();
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
        RedirectStandardOutput=true,RedirectStandardError:true}};
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
