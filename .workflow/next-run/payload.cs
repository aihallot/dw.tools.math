using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Solution="dw.tools.math.slnx";
const string TestProject="tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj";
const string GateTests="tests/projects/dw.tools.math.composition.tests/M2GateIntegrationTests.cs";
const string Recipe="docs/distribution/m2-release-gate.md";
const string Decision="docs/planning/decisions/m2-gate.md";
const string Evidence="docs/planning/evidence/M2-gate.json";
const string Adoption="docs/coordination/requests/MATH-XR-002-aura-adoption.json";
var phases=new[]{
    ("M2-W01-C01","docs/planning/evidence/M2-W01-C01-qualified.json",
        "typed-ir-adr-qualified-no-product-implementation"),
    ("M2-W01-C02","docs/planning/evidence/M2-W01-C02-boundary-qualified.json",
        "immutable-bounded-typed-IR-chunk-qualified"),
    ("M2-W01-C03","docs/planning/evidence/M2-W01-C03-boundary-qualified.json",
        "bounded-closed-ir-canonical-json-and-structural-hashes-qualified"),
    ("M2-W02-C01","docs/planning/evidence/M2-W02-C01-boundary-qualified.json",
        "provider-independent-discovery-no-permission-no-final-value-qualified"),
    ("M2-W02-C02","docs/planning/evidence/M2-W02-C02-boundary-qualified.json",
        "bounded-cancellable-step-provenance-exact-pipeline-qualified"),
    ("M2-W02-C03","docs/planning/evidence/M2-W02-C03-boundary-qualified.json",
        "explicit-nonfinal-exact-replay-cache-quota-and-portability-boundaries-qualified")
};
var packages=new[]{
    ("dw.quantities","0.2.0-preview.1","src/projects/dw.quantities/dw.quantities.csproj",
        Array.Empty<string>()),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",new[]{"dw.quantities"}),
    ("dw.tools.math.composition","0.3.0-preview.1",
        "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",
        new[]{"dw.quantities","dw.tools.math.ir"})
};
var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var plan=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var m2=plan["releases"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M2");
var m1=plan["releases"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1");
var baseState=(string?)plan["plan_version"]=="0.1.36" && (string?)m2["status"]=="in_progress";
var targetState=(string?)plan["plan_version"]=="0.1.37" && (string?)m2["status"]=="done";
if(!baseState && !targetState)
    throw new InvalidOperationException("RS038 requires the exact M2 0.1.36 gate baseline or 0.1.37 target.");
if((string?)m1["status"]!="done")
    throw new InvalidOperationException("M2 prerequisite M1 not complete.");
var accepted=m2["required_chunks"]!.AsArray().Select(x=>x!.GetValue<string>()).ToArray();
if(!accepted.SequenceEqual(phases.Select(x=>x.Item1),StringComparer.Ordinal))
    throw new InvalidOperationException("M2 required phase list differs from accepted gate.");
foreach(var workId in new[]{"M2-W01","M2-W02"})
{
    var w=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==workId);
    if((string?)w["status"]!="done")throw new InvalidOperationException("Incomplete work package "+workId);
    p.ProjectPlan.RequireNodeState(workId,"done");
}
foreach(var (id,evidence,status) in phases)
{
    var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==id);
    if((string?)chunk["status"]!="done")throw new InvalidOperationException("Incomplete phase "+id);
    var tasks=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
    if(tasks.Length!=2 || tasks.Any(t=>(string?)t["status"]!="done" ||
        t["subtasks"]!.AsArray().Any(s=>(string?)s!["status"]!="done")))
        throw new InvalidOperationException("Incomplete accepted task/subtask of "+id);
    p.ProjectPlan.RequireNodeState(id,"done");
    var qualified=JsonNode.Parse(File.ReadAllText(Path.Combine(root,evidence)))!.AsObject();
    if((string?)qualified["status"]!=status)
        throw new InvalidOperationException("Unqualified evidence "+id+": "+qualified["status"]);
}
var external=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Adoption)))!.AsObject();
if((string?)external["status"]!="draft" || (string?)external["transmission"]!="none")
    throw new InvalidOperationException("M2 local gate cannot assert external AURA adoption.");
p.ProjectPlan.RequireNodeState("M2",baseState?"in-progress":"done");
p.ProjectPlan.RequireNodeState("M3","not-ready");

foreach(var file in new[]{GateTests,Recipe,Decision})
    p.Files.ReplaceFromStaged("staged/"+file,file);
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var gateTestOutput=Required(root,"dotnet","test",TestProject,
    "-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M2GateIntegrationTests",
    "--logger","console;verbosity=normal");
if(!gateTestOutput.Contains("Total tests: 8",StringComparison.OrdinalIgnoreCase) ||
   !gateTestOutput.Contains("Passed: 8",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("The eight independent M2 gate tests did not all execute and pass.");
Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","m2-gate-local-feed");
Directory.CreateDirectory(feed);
var observed=new JsonArray();
foreach(var (id,version,project,deps) in packages)
{
    Required(root,"dotnet","pack",project,"-c","Release","--no-build","--no-restore","-o",feed);
    var path=Path.Combine(feed,id+"."+version+".nupkg");
    if(!File.Exists(path))throw new InvalidOperationException("Local package missing: "+id);
    using(var zip=ZipFile.OpenRead(path))
    {
        if(zip.GetEntry("lib/net10.0/"+id+".dll") is null)
            throw new InvalidOperationException("Missing net10 assembly "+id);
        var manifests=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
        if(manifests.Length!=1)throw new InvalidOperationException("Unexpected nuspec count "+id);
        using var stream=manifests[0].Open();
        var xml=XDocument.Load(stream);
        var meta=xml.Descendants().Single(e=>e.Name.LocalName=="metadata");
        var actualId=meta.Elements().Single(e=>e.Name.LocalName=="id").Value;
        var actualVersion=meta.Elements().Single(e=>e.Name.LocalName=="version").Value;
        var actualDeps=meta.Descendants().Where(e=>e.Name.LocalName=="dependency")
            .Select(e=>(string?)e.Attribute("id")??"").OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
        if(actualId!=id || actualVersion!=version ||
            !actualDeps.SequenceEqual(deps.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Local NuGet graph/identity incorrect for "+id);
    }
    observed.Add(new JsonObject {
        ["id"]=id,["version"]=version,
        ["bytes"]=new FileInfo(path).Length,
        ["sha256"]=Hash(File.ReadAllBytes(path)),
        ["dependencies"]=p.Json.StringArray(deps),
        ["assembly"]="lib/net10.0/"+id+".dll",
        ["scope"]="local-ignored-artifact-not-publicly-published"
    });
}

var consumer=Path.Combine(Path.GetTempPath(),"dw-tools-math-m2-gate-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(consumer);
try
{
    var project=Path.Combine(consumer,"M2GateConsumer.csproj");
    File.WriteAllText(project,"""
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="dw.tools.math.composition" Version="0.3.0-preview.1" />
      </ItemGroup>
    </Project>
    """);
    File.WriteAllText(Path.Combine(consumer,"Program.cs"),"""
    using dw.quantities;
    using Dw.Tools.Math.Ir;
    using Dw.Tools.Math.Composition;
    var fraction = new ExactRational(1,3);
    var coded = IrCanonicalJsonCodec.Encode(new IrExactScalar(fraction));
    if (((IrExactScalar)IrCanonicalJsonCodec.Decode(coded)).Value != fraction)
        throw new Exception("Independent IR/quantities NuGet roundtrip failed");
    var m = new UnitDefinition("m","m","metre",UnitSystem.Si,
        DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
    var steps = new[] { ExactPipelineStep.Start(new ExactRational(5,1),m) };
    var context = ExactReplayContext.Create("a/1","p/1","c/1","exact","none/1");
    var cache = new ExactReplayCache(2);
    var first = ExactReplayRunner.TryRun(steps,context,cache);
    var second = ExactReplayRunner.TryRun(steps,context,cache);
    if (!first.HasFinalValue || first.Receipt!.Result.DisplayValue != new ExactRational(5,1) ||
        !second.Receipt!.CacheHit)
        throw new Exception("Independent composition/replay NuGet consumer failed");
    Console.WriteLine("dw.tools.math/m2-gate-consumer/0.3 qualified");
    """);
    Required(root,"dotnet","restore",project,"--source",feed);
    var output=Required(root,"dotnet","run","--project",project,"-c","Release","--no-restore");
    if(!output.Contains("dw.tools.math/m2-gate-consumer/0.3 qualified",StringComparison.Ordinal))
        throw new InvalidOperationException("Isolated M2 NuGet consumer receipt missing.");
}
finally
{
    Directory.Delete(consumer,recursive:true);
}
var sdk=Required(root,"dotnet","--version").Trim();
var proof=new JsonObject
{
    ["schema_version"]=1,["id"]="M2-gate",
    ["status"]="local-m2-typed-ir-composition-and-isolated-consumer-qualified",
    ["milestone"]="M2",["release_version"]="0.3.0-preview",["plan_version"]="0.1.37",
    ["checked_chunk_ids"]=p.Json.StringArray(phases.Select(x=>x.Item1).ToArray()),
    ["qualified_phase_evidence"]=p.Json.StringArray(phases.Select(x=>x.Item2).ToArray()),
    ["gate_tests"]=GateTests,
    ["gate_tests_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,GateTests))),
    ["gate_tests_passed"]=8,["full_composition_ir_and_quantities_regressions_passed"]=true,
    ["foundation_verification_passed"]=true,["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,["isolated_local_nuget_consumer_passed"]=true,
    ["isolated_consumer_receipt"]="dw.tools.math/m2-gate-consumer/0.3 qualified",
    ["observed_local_packages"]=observed,
    ["runtime_os"]=RuntimeInformation.OSDescription,
    ["runtime_architecture"]=RuntimeInformation.ProcessArchitecture.ToString(),
    ["dotnet_sdk"]=sdk,
    ["release_recipe"]=Recipe,["decision"]=Decision,
    ["external_adoption_request"]=Adoption,["aura_transmission"]="none",
    ["nonclaims"]=p.Json.StringArray(
        "No external AURA adoption, consumer-side AURA testing, permission or package publication.",
        "Structural hashes do not demonstrate general mathematical equivalence.",
        "No symbolic solver, numerical provider execution, cross-platform double portability, or M3/M4 results.",
        "Package hashes represent observed local build bytes, not anticipated reproducible archives.")
};
p.Files.WriteComplete(Evidence,proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",planRoot=>
{
    planRoot["plan_version"]="0.1.37";
    var release=planRoot["releases"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]=="M2");
    release["status"]="done";
    release["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M2-W01-C03-boundary-qualified.json",
        "docs/planning/evidence/M2-W02-C03-boundary-qualified.json",Evidence);
});
if(baseState) p.ProjectPlan.ConvergeNodeToDone("M2");
else p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3","not-ready");
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
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)throw new InvalidOperationException(executable+" "+
        string.Join(" ",args)+" exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
