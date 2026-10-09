using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M2-W01-C02";
const string Task = "M2-W01-C02-T1";
const string Red = "M2-W01-C02-T1-R";
const string Green = "M2-W01-C02-T1-G";
const string Verify = "M2-W01-C02-T1-V";
const string Next = "M2-W01-C02-T2";
const string NextRed = "M2-W01-C02-T2-R";
const string Project = "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj";
const string Model = "src/projects/dw.tools.math.ir/IrModel.cs";
const string ProjectLock = "src/projects/dw.tools.math.ir/packages.lock.json";
const string RedTests = "tests/projects/dw.quantities.tests/M2W01C02RedTests.cs";
const string QuantitiesTestsProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string QuantitiesTestsLock = "tests/projects/dw.quantities.tests/packages.lock.json";
const string IrTestsProject = "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj";
const string IrTestsLock = "tests/projects/dw.tools.math.ir.tests/packages.lock.json";
const string Tests = "tests/projects/dw.tools.math.ir.tests/M2W01C02Tests.cs";
const string Solution = "dw.tools.math.slnx";
const string RedEvidence = "docs/planning/evidence/M2-W01-C02-red.json";
const string GreenEvidence = "docs/planning/evidence/M2-W01-C02-functional-green.json";
const string PriorAdr = "docs/planning/evidence/M2-W01-C01-qualified.json";
const string ExpectedVersion = "0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var product=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var phase=product["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Phase);
var tasks=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
var task=tasks.Single(x=>(string?)x["id"]==Task);
var next=tasks.Single(x=>(string?)x["id"]==Next);
string Sub(JsonObject task,string id)=>(string?)task["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"]
    ?? throw new InvalidOperationException("Missing product subtask status.");
var baseline=(string?)product["plan_version"]=="0.1.26" &&
    (string?)phase["status"]=="planned" && (string?)task["status"]=="planned" &&
    (string?)next["status"]=="planned" &&
    Sub(task,Red)=="planned" && Sub(task,Green)=="planned" && Sub(task,Verify)=="planned" &&
    Sub(next,NextRed)=="planned";
var target=(string?)product["plan_version"]=="0.1.27" &&
    (string?)phase["status"]=="in_progress" && (string?)task["status"]=="done" &&
    (string?)next["status"]=="ready" &&
    Sub(task,Red)=="done" && Sub(task,Green)=="done" && Sub(task,Verify)=="done" &&
    Sub(next,NextRed)=="ready";
if(!baseline && !target)
    throw new InvalidOperationException("RS028 lifecycle is neither the qualified M2 ADR baseline nor the accepted functional IR target.");

var adr=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorAdr)))!.AsObject();
if((string?)adr["status"]!="typed-ir-adr-qualified-no-product-implementation" ||
   adr["product_ir_compiled"]?.GetValue<bool>()!=false)
    throw new InvalidOperationException("The M2 typed-IR architectural contract was not previously qualified.");
var external=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)external["status"]!="draft" || (string?)external["transmission"]!="none")
    throw new InvalidOperationException("M2 cannot assert external AURA adoption.");

// RED uses the current quantities test assembly without a reference to the new
// IR project. No source, project or solution mutation is applied first.
p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    foreach(var (method,marker) in new[]{
        ("StandaloneMathematicalIrAssemblyMustExist",
         "M2-W01-C02-T1 RED: standalone immutable mathematical IR assembly is missing."),
        ("TypedScalarAndBoundSymbolContractsMustExist",
         "M2-W01-C02-T1 RED: typed scalar and bound symbol contracts are missing.")
    })
    {
        var observed=Run(root,"dotnet","test",QuantitiesTestsProject,
            "-c","Release","--filter","FullyQualifiedName~M2W01C02RedTests."+method,
            "--logger","console;verbosity=normal");
        if(observed.ExitCode==0 || !observed.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Independent missing-IR RED was not observed: "+
                method+" | "+Relevant(observed.Output));
    }
}

var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C02-red",
    ["status"]="independent-missing-IR-package-and-contract-red-observed",
    ["source_adr"]=PriorAdr,
    ["focused_filter"]="FullyQualifiedName~M2W01C02RedTests",
    ["observed_absences"]=p.Json.StringArray(
        "The standalone dw.tools.math.ir assembly does not yet exist as a Math product.",
        "No typed IrExactScalar and IrSymbol public contracts are materialized before GREEN."),
    ["limits"]="RED alone proves no functionality and does not authorize an IR project without executable oracles."
};
p.Files.WriteComplete(RedEvidence,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var file in new[]{
    Project,ProjectLock,Model,QuantitiesTestsProject,QuantitiesTestsLock,
    IrTestsProject,IrTestsLock,Tests,Solution})
    p.Files.ReplaceFromStaged("staged/"+file,file);

var irCsproj=File.ReadAllText(Path.Combine(root,Project));
if(!irCsproj.Contains("../dw.quantities/dw.quantities.csproj",StringComparison.Ordinal) ||
   irCsproj.Contains("aura.",StringComparison.OrdinalIgnoreCase) ||
   irCsproj.Contains("dw.localization",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("The IR package must reference only the exact quantities mathematical layer.");
RunRequired(root,"dotnet","restore",Solution,"--locked-mode");
RunRequired(root,"dotnet","build",Solution,"-c","Release","--no-restore");

foreach(var name in new[]{
    "StandaloneMathematicalIrAssemblyMustExist",
    "TypedScalarAndBoundSymbolContractsMustExist"})
    RunRequired(root,"dotnet","test",QuantitiesTestsProject,
        "-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C02RedTests."+name,
        "--logger","console;verbosity=normal");
foreach(var name in new[]{
    "ExactScalarAlwaysRetainsCanonicalRationalInsteadOfBinary64",
    "FiniteBinary64HasDistinctKindAndPreservesReceivedBits",
    "NonfiniteBinary64CannotEnterAdmittedApproximateScalar",
    "BoundAndFreeSymbolsWithIdenticalTextAreDifferentIdentities",
    "RealSquareRootOfSquareNormalizesOnlyToAbsoluteWithoutProof",
    "ProvenNonnegativeSymbolMayEliminateAbsoluteWithoutCapture",
    "NonzeroDenominatorExclusionMustSurviveRestrictedExpression",
    "QuantityLiteralsPreserveSourceUnitIdentityAndEightDimensionalType",
    "InputMutationsCannotChangeStoredOperationOrAssumptions",
    "InvalidArityAndIdentityAreRejectedBeforeMaterializingIr"})
    RunRequired(root,"dotnet","test",IrTestsProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C02Tests."+name,
        "--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",IrTestsProject,"-c","Release","--no-build","--no-restore");
RunRequired(root,"dotnet","test",QuantitiesTestsProject,"-c","Release","--no-build","--no-restore");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","ir-package-feed");
RunRequired(root,"dotnet","pack",Project,"-c","Release","--no-build","--no-restore","-o",feed);
var packagePath=Path.Combine(feed,"dw.tools.math.ir."+ExpectedVersion+".nupkg");
if(!File.Exists(packagePath))
    throw new InvalidOperationException("New qualified IR NuGet artifact was not produced.");
using(var package=ZipFile.OpenRead(packagePath))
{
    if(package.GetEntry("lib/net10.0/dw.tools.math.ir.dll") is null)
        throw new InvalidOperationException("The IR package does not contain its compiled public assembly.");
    var specs=package.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)
        throw new InvalidOperationException("Expected one NuGet manifest for IR.");
    using var stream=specs[0].Open();
    var manifest=XDocument.Load(stream);
    var metadata=manifest.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var name=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var dependencies=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(name!="dw.tools.math.ir" || version!=ExpectedVersion ||
       !dependencies.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("IR package identity, version or dependency graph escaped its contract.");
}
var complete=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C02-functional-green",
    ["status"]="immutable-typed-IR-functional-contract-qualified-adversarial-boundaries-pending",
    ["source_adr"]=PriorAdr,
    ["independent_red"]=RedEvidence,
    ["ir_assembly"]="dw.tools.math.ir",
    ["package_id"]="dw.tools.math.ir",
    ["package_version"]=ExpectedVersion,
    ["package_sha256"]=Hash(File.ReadAllBytes(packagePath)),
    ["package_bytes"]=new FileInfo(packagePath).Length,
    ["package_dependencies"]=p.Json.StringArray("dw.quantities"),
    ["local_only_package"]=true,
    ["model_source"]=Model,
    ["model_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Model))),
    ["focused_tests"]=10,
    ["independent_reds_now_green"]=2,
    ["complete_ir_tests_passed"]=true,
    ["complete_quantities_tests_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["adoption_transmitted"]=false,
    ["oracles"]=p.Json.StringArray(
        "Exact 1/3 and finite IEEE-754 0.1 have distinct types; no implicit widening or shared rounding-based equality.",
        "Binary64 negative zero preserves its exact sign bit; non-finite NaN and infinity are rejected.",
        "Free and bound symbols with identical labels have disjoint identities; bound scopes and slots remain stable.",
        "sqrt(square(x)) over the real domain normalizes to abs(x) unless an explicit nonnegative assumption proves x >= 0.",
        "A bound x with the same label cannot borrow a free x positivity assumption.",
        "(x^2-1)/(x-1) preserves x != 1 and its original divide node; no automatic unsound simplification.",
        "Quantities preserve canonical unit ID, explicit UnitSystem, dimension and temperature tag.",
        "Operation and assumption inputs are defensively copied; unsupported arity and invalid symbol identity are refused."),
    ["pending_boundary_work"]=p.Json.StringArray(
        "T2 must independently RED/GREEN resource quotas for maximum tree nodes, depth, matrix shape and identifier limits across whole graphs, not just local factory bounds.",
        "T2 must verify immutability and reject unsafe cyclic/unbound or duplicate scopes, and qualify exact/approximate policies under adversarial inputs.",
        "M2-W01-C03 owns the canonical versioned JSON codec; no serialization or universal symbolic engine is claimed here."),
    ["limits"]="The admitted IR is a provider-neutral AST and one guarded sqrt-square rule; no general symbolic solver, automatic simplifier or OpenMath/MathML interchange codec is delivered."
};
p.Files.WriteComplete(GreenEvidence,complete.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    var t1=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    var t2=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Next);
    plan["plan_version"]="0.1.27";
    chunk["status"]="in_progress";
    chunk["refinement"]="RS028 establishes a two-case independent missing-IR RED, delivers a standalone provider-free immutable typed dw.tools.math.ir assembly referencing only ExactRational's quantity layer, and independently tests 10 exact/approximate, bound symbol, sqrt-square domain restriction, immutable collection and contract behaviors. T2 boundary quotas and package consumer hardening remain to be qualified before C02 closes.";
    chunk["files"]=p.Json.StringArray(Project,ProjectLock,Model,
        QuantitiesTestsProject,QuantitiesTestsLock,IrTestsProject,IrTestsLock,Tests,
        RedTests,Solution,RedEvidence,GreenEvidence);
    chunk["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+QuantitiesTestsProject+" -c Release --filter FullyQualifiedName~M2W01C02RedTests",
        "dotnet test "+IrTestsProject+" -c Release --filter FullyQualifiedName~M2W01C02Tests",
        "dotnet test "+QuantitiesTestsProject+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    chunk["evidence"]=p.Json.StringArray(RedEvidence,GreenEvidence);
    t1["status"]="done";
    t1["evidence"]=p.Json.StringArray(RedEvidence,GreenEvidence);
    foreach(var sub in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedEvidence:GreenEvidence);
    }
    t2["status"]="ready";
    t2["subtasks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==NextRed)["status"]="ready";
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Phase,"ready","in-progress");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"ready","in-progress");
    foreach(var id in new[]{Red,Green,Verify})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.TransitionNode(Next,"not-ready","ready");
    p.ProjectPlan.TransitionNode(NextRed,"not-ready","ready");
}
else
{
    foreach(var id in new[]{Task,Red,Green,Verify})
        p.ProjectPlan.RequireNodeState(id,"done");
    foreach(var id in new[]{Next,NextRed})
        p.ProjectPlan.RequireNodeState(id,"ready");
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes) =>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Relevant(string output)
{
    var idx=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(idx<0)idx=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var useful=idx<0?output:output[idx..];
    return useful.Length>3200?useful[..3200]:useful;
}
static (int ExitCode,string Output) Run(string root,string executable,params string[] arguments)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var argument in arguments)process.StartInfo.ArgumentList.Add(argument);
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
    return (process.ExitCode,output);
}
static string RunRequired(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.ExitCode!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.ExitCode+": "+Relevant(result.Output));
    return result.Output;
}
