using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W03-C01";
const string WorkPackage = "M1-W03";
const string T1 = "M1-W03-C01-T1";
const string T2 = "M1-W03-C01-T2";
const string ConsumerDir = "tests/consumers/exact-math-smoke";
const string ConsumerProject = ConsumerDir + "/exact-math-smoke.csproj";
const string ConsumerProgram = ConsumerDir + "/Program.cs";
const string ConsumerConfig = ConsumerDir + "/NuGet.Config";
const string Notice = "docs/distribution/exact-math-packages.md";
const string RedEvidence = "docs/planning/evidence/M1-W03-C01-red.json";
const string QualifiedEvidence = "docs/planning/evidence/M1-W03-C01-qualified.json";
const string ExpectedConsumerOutput = "dw.tools.math/exact-consumer/0.2 qualified";
const string PackageVersion = "0.2.0-preview.1";
const string RightsPath = "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json";

var packages = new (string Id,string Project)[]
{
    ("dw.quantities","src/projects/dw.quantities/dw.quantities.csproj"),
    ("dw.quantities.expression","src/projects/dw.quantities.expression/dw.quantities.expression.csproj"),
    ("dw.quantities.standard","src/projects/dw.quantities.standard/dw.quantities.standard.csproj")
};

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var plan = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk = plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var work = plan["work_packages"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==WorkPackage);
var tasks = chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
var first = tasks.Single(x=>(string?)x["id"]==T1);
var second = tasks.Single(x=>(string?)x["id"]==T2);
string State(JsonObject task,string suffix) => (string?)task["subtasks"]!.AsArray()
    .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==(string?)task["id"]+"-"+suffix)["status"]
    ?? throw new InvalidOperationException("Missing subtask status "+suffix);

var baseline = (string?)plan["plan_version"]=="0.1.23" &&
    (string?)work["status"]=="planned" && (string?)chunk["status"]=="planned" &&
    tasks.All(t=>(string?)t["status"]=="planned") &&
    new[]{"R","G","V"}.All(s=>State(first,s)=="planned" && State(second,s)=="planned");
var target = (string?)plan["plan_version"]=="0.1.24" &&
    (string?)work["status"]=="in_progress" && (string?)chunk["status"]=="done" &&
    tasks.All(t=>(string?)t["status"]=="done") &&
    new[]{"R","G","V"}.All(s=>State(first,s)=="done" && State(second,s)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS025 product lifecycle is neither accepted baseline nor exact completed target.");

var rights = JsonNode.Parse(File.ReadAllText(Path.Combine(root,RightsPath)))!.AsObject();
if((string?)rights["source_revision"]!="82a6b435a387a7e116b47a6b2c433ae9e067bf21" ||
   rights["authorization"]?["package_from_dw_tools_math"]?.GetValue<bool>()!=true ||
   rights["authorization"]?["redistribute_from_dw_tools_math"]?.GetValue<bool>()!=true ||
   (string?)rights["license_and_notices"]?["model"]!="owner-authorized proprietary code")
    throw new InvalidOperationException("Owner authorization or packaging rights differ from approved M0 evidence.");

foreach(var file in new[]{ConsumerProject,ConsumerProgram,ConsumerConfig})
    p.Files.ReplaceFromStaged("staged/"+file,file);

var consumerCsproj = File.ReadAllText(Path.Combine(root,ConsumerProject));
var consumerNuGet = File.ReadAllText(Path.Combine(root,ConsumerConfig));
if(consumerCsproj.Contains("ProjectReference",StringComparison.Ordinal) ||
   consumerCsproj.Contains("aura.",StringComparison.OrdinalIgnoreCase) ||
   consumerCsproj.Contains("D:/",StringComparison.OrdinalIgnoreCase) ||
   !consumerNuGet.Contains("<clear />",StringComparison.Ordinal) ||
   !consumerNuGet.Contains("bounded-math-local",StringComparison.Ordinal) ||
   packages.Any(x=>!consumerCsproj.Contains(
       "PackageReference Include=\""+x.Id+"\" Version=\"["+PackageVersion+"]\"",StringComparison.Ordinal)))
    throw new InvalidOperationException("Isolated consumer must reference only the exact authorized package IDs and local feed.");

var localFeed = Path.Combine(root,"build","artifacts","math-package-feed");
var isolatedPackages = Path.Combine(root,"build","artifacts","math-isolated-packages");
var config = Path.Combine(root,ConsumerConfig);
var project = Path.Combine(root,ConsumerProject);
var packagePaths = packages.Select(x=>Path.Combine(localFeed,x.Id+"."+PackageVersion+".nupkg")).ToArray();

if(baseline)
{
    if(packagePaths.Any(File.Exists))
        throw new InvalidOperationException("RS025 RED expected the local Math NuGet feed to contain no built packages.");
    var red=Run(root,"dotnet","restore",project,
        "--configfile",config,"--source",localFeed,"--packages",isolatedPackages);
    if(red.Code==0 ||
        !(red.Output.Contains("NU1101",StringComparison.OrdinalIgnoreCase) ||
          red.Output.Contains("NU1301",StringComparison.OrdinalIgnoreCase)))
        throw new InvalidOperationException("Isolated consumer missing-package RED was not observed: "+
            Relevant(red.Output));
    if(File.Exists(Path.Combine(root,QualifiedEvidence)) ||
       File.Exists(Path.Combine(root,Notice)))
        throw new InvalidOperationException("RS025 provenance RED expected the qualified receipt and distribution notice to be absent.");
}

var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W03-C01-red",
    ["status"]="isolated-consumer-package-absence-and-provenance-absence-observed",
    ["consumer_project"]=ConsumerProject,
    ["nuget_config"]=ConsumerConfig,
    ["feed_mode"]="local-only NuGet.Config sources cleared; isolated packages directory and explicit local --source",
    ["T1_red"]="Consumer restore fails with missing local Math package feed before package production.",
    ["T2_red"]="Qualified distribution hash/rights receipt and consumer notice absent before validation.",
    ["limits"]="This records pre-production absence only, not package contents, source parity or downstream consumer success."
};
p.Files.WriteComplete(RedEvidence,
    redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
RunRequired(root,"dotnet","test","tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-restore","--no-build","--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");

foreach(var (id,sourceProject) in packages)
{
    RunRequired(root,"dotnet","pack",sourceProject,
        "-c","Release","--no-restore","--no-build","-o",localFeed);
}

var packagesMetadata = new List<JsonNode>();
var allowedPackageIds = packages.Select(x=>x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
foreach(var (id,index) in packages.Select((entry,i)=>(entry.Id,i)))
{
    var packagePath=packagePaths[index];
    if(!File.Exists(packagePath))
        throw new InvalidOperationException("Required locally built nupkg missing: "+id);
    using var zip=ZipFile.OpenRead(packagePath);
    if(zip.GetEntry("lib/net10.0/"+id+".dll") is null)
        throw new InvalidOperationException("NuGet package lacks expected independent net10.0 assembly: "+id);
    var nuspecs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspecs.Length!=1)
        throw new InvalidOperationException("Expected exactly one nuspec for "+id);
    using var stream=nuspecs[0].Open();
    var nuspec=XDocument.Load(stream);
    var metadata=nuspec.Descendants().FirstOrDefault(x=>x.Name.LocalName=="metadata")
        ?? throw new InvalidOperationException("Missing NuGet metadata for "+id);
    var packagedId=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var packagedVersion=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(packagedId!=id || packagedVersion!=PackageVersion ||
       deps.Any(x=>!allowedPackageIds.Contains(x)))
        throw new InvalidOperationException("Package identity/dependency escape in "+id);
    if(id=="dw.quantities" && deps.Length!=0)
        throw new InvalidOperationException("Primitive exact quantities package must have no dependencies.");
    if(id=="dw.quantities.expression" &&
       !deps.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expression package must depend only on dw.quantities.");
    if(id=="dw.quantities.standard" &&
       !(deps.Contains("dw.quantities",StringComparer.OrdinalIgnoreCase) &&
         deps.Contains("dw.quantities.expression",StringComparer.OrdinalIgnoreCase) &&
         deps.Length==2))
        throw new InvalidOperationException("Standard catalog package must retain both declared Math dependencies.");

    packagesMetadata.Add(new JsonObject
    {
        ["package_id"]=id,
        ["version"]=packagedVersion,
        ["sha256"]=Sha(File.ReadAllBytes(packagePath)),
        ["bytes"]=new FileInfo(packagePath).Length,
        ["assembly_path"]="lib/net10.0/"+id+".dll",
        ["dependencies"]=p.Json.StringArray(deps),
        ["artifact_scope"]="Local ignored disposable output; package is not committed or publicly published."
    });
}

RunRequired(root,"dotnet","restore",project,
    "--configfile",config,"--source",localFeed,"--packages",isolatedPackages);
RunRequired(root,"dotnet","build",project,"-c","Release","--no-restore");
var execution=RunRequired(root,"dotnet","run","--project",project,"-c","Release","--no-restore","--no-build");
if(!execution.Split('\n').Any(line=>line.Trim()==ExpectedConsumerOutput))
    throw new InvalidOperationException("Isolated Math consumer did not print its independently checked qualification receipt: "+
        Relevant(execution));

// For T2, the verified mathematical differences from AURA are already
// accepted in M0 and C01; do not silently rewrite inherited oracles.
var sourceDifferences=new[]{
    "AURA's historical culture-priority resolver intentionally differs from Math's explicit UnitSystem selection (M0-W02-C02).",
    "Math's strict Australian 570mL beer-pint is a named extra profile, not a unit asserted to exist in the 58-unit inherited AURA catalogue.",
    "Historical defects are not promoted to an oracle; independently checked conversion and exact arithmetic results remain authoritative."
};
p.Files.ReplaceFromStaged("staged/"+Notice,Notice);
var qualified=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W03-C01-qualified",
    ["status"]="isolated-local-package-consumer-and-provenance-qualified",
    ["local_feed"]="build/artifacts/math-package-feed",
    ["isolated_package_directory"]="build/artifacts/math-isolated-packages",
    ["consumer_project"]=ConsumerProject,
    ["consumer_source"]=ConsumerProgram,
    ["consumer_nuget_config"]=ConsumerConfig,
    ["package_artifacts"]=p.Json.Array(packagesMetadata.ToArray()),
    ["distribution_recipe"]=Notice,
    ["rights_reference"]=RightsPath,
    ["rights_model"]="owner-authorized proprietary code; no open-source license asserted",
    ["owner_notice_separate_required"]=false,
    ["independent_red"]=RedEvidence,
    ["sample_output"]=ExpectedConsumerOutput,
    ["source_parity_evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M1-W02-C01-source-parity-qualified.json",
        "docs/planning/evidence/M1-W02-C02-boundary-qualified.json",
        "docs/planning/evidence/M1-W02-C03-qualified.json"),
    ["documented_divergences"]=p.Json.StringArray(sourceDifferences),
    ["locked_math_restore_passed"]=true,
    ["all_quantities_tests_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["three_nupkg_contents_and_dependency_graph_qualified"]=true,
    ["local_only_consumer_restore_passed"]=true,
    ["isolated_consumer_execution_passed"]=true,
    ["limitations"]=p.Json.StringArray(
        "This run produces local NuGet artifacts in an ignored disposable build tree; no binary package is checked into main or published to a public feed.",
        "SHA-256 values identify these exact observed package bytes. Bitwise identity across future SDK builds is not claimed.",
        "The receipt is an independently executed consumption smoke test, not a release signature, third-party license audit or external AURA migration.",
        "AURA and other sibling repository sources are not modified; any future adoption remains separately authorized.")
};
p.Files.WriteComplete(QualifiedEvidence,
    qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",data=>{
    var w=data["work_packages"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==WorkPackage);
    var c=data["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    data["plan_version"]="0.1.24";
    w["status"]="in_progress";
    var paths=w["paths"]!.AsArray();
    foreach(var candidate in new[]{"tests/consumers/exact-math-smoke/","docs/distribution/"})
        if(!paths.Any(x=>(string?)x==candidate))
            paths.Add((JsonNode?)JsonValue.Create(candidate));
    c["status"]="done";
    c["refinement"]="RS025 observes an independent missing-local-package and missing-provenance RED, builds three exact Math NuGet packages with inspected assembly/dependency manifests and SHA-256s, restores and executes a package-only isolated consumer from a cleared local feed, verifies exact calculations/conversions/comparisons and reconciles inherited AURA divergences and owner-rights. M1-W03-C02 external adoption dossier remains future work.";
    c["files"]=p.Json.StringArray(ConsumerProject,ConsumerProgram,ConsumerConfig,Notice,
        RedEvidence,QualifiedEvidence);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet pack src/projects/dw.quantities/dw.quantities.csproj -c Release --no-restore --no-build -o build/artifacts/math-package-feed",
        "dotnet pack src/projects/dw.quantities.expression/dw.quantities.expression.csproj -c Release --no-restore --no-build -o build/artifacts/math-package-feed",
        "dotnet pack src/projects/dw.quantities.standard/dw.quantities.standard.csproj -c Release --no-restore --no-build -o build/artifacts/math-package-feed",
        "dotnet restore tests/consumers/exact-math-smoke/exact-math-smoke.csproj --configfile tests/consumers/exact-math-smoke/NuGet.Config --source build/artifacts/math-package-feed",
        "dotnet run --project tests/consumers/exact-math-smoke/exact-math-smoke.csproj -c Release --no-restore",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    c["evidence"]=p.Json.StringArray(RedEvidence,QualifiedEvidence);
    foreach(var task in c["tasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedEvidence,QualifiedEvidence);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(
                ((string?)sub["id"])!.EndsWith("-R",StringComparison.Ordinal)
                    ? RedEvidence : QualifiedEvidence);
        }
    }
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(WorkPackage,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    foreach(var taskName in new[]{T1,T2})
    {
        p.ProjectPlan.TransitionNode(taskName,"not-ready","ready");
        var red=taskName+"-R";
        var green=taskName+"-G";
        var verification=taskName+"-V";
        p.ProjectPlan.TransitionNode(red,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(red);
        p.ProjectPlan.ConvergeNodeToDone(red);
        p.ProjectPlan.TransitionNode(green,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(green);
        p.ProjectPlan.ConvergeNodeToDone(green);
        p.ProjectPlan.TransitionNode(verification,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(verification);
        p.ProjectPlan.ConvergeNodeToDone(verification);
        p.ProjectPlan.ConvergeNodeToDone(taskName);
    }
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var id in new[]{T1+"-R",T1+"-G",T1+"-V",T1,
        T2+"-R",T2+"-G",T2+"-V",T2,Phase})
        p.ProjectPlan.RequireNodeState(id,"done");
    p.ProjectPlan.RequireNodeState(WorkPackage,"in-progress");
    p.ProjectPlan.RequireNodeState("M1-W03-C02","not-ready");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Sha(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string Relevant(string output)
{
    var pos=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(pos<0)pos=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var interesting=pos<0?output:output[pos..];
    return interesting.Length>2500?interesting[..2500]:interesting;
}

static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static string RunRequired(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
