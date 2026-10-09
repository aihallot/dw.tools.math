using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M2-W01-C03";
const string T1 = "M2-W01-C03-T1";
const string T2 = "M2-W01-C03-T2";
const string Red = "M2-W01-C03-T1-R";
const string Green = "M2-W01-C03-T1-G";
const string Verify = "M2-W01-C03-T1-V";
const string NextRed = "M2-W01-C03-T2-R";
const string Codec = "src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs";
const string RedTests = "tests/projects/dw.tools.math.ir.tests/M2W01C03RedTests.cs";
const string Tests = "tests/projects/dw.tools.math.ir.tests/M2W01C03Tests.cs";
const string Doc = "docs/distribution/ir-canonical-json.md";
const string TestProject = "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj";
const string RedProof = "docs/planning/evidence/M2-W01-C03-functional-red.json";
const string GreenProof = "docs/planning/evidence/M2-W01-C03-functional-qualified.json";
const string Previous = "docs/planning/evidence/M2-W01-C02-boundary-qualified.json";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var plan=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var tasks=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
var first=tasks.Single(x=>(string?)x["id"]==T1);
var second=tasks.Single(x=>(string?)x["id"]==T2);
string Sub(JsonObject task,string id)=>(string?)task["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"] ?? throw new InvalidOperationException("Missing subtask "+id);
var baseline=(string?)plan["plan_version"]=="0.1.28" &&
    (string?)chunk["status"]=="planned" && (string?)first["status"]=="planned" &&
    (string?)second["status"]=="planned" &&
    new[]{Red,Green,Verify}.All(x=>Sub(first,x)=="planned") && Sub(second,NextRed)=="planned";
var target=(string?)plan["plan_version"]=="0.1.29" &&
    (string?)chunk["status"]=="in_progress" && (string?)first["status"]=="done" &&
    (string?)second["status"]=="ready" &&
    new[]{Red,Green,Verify}.All(x=>Sub(first,x)=="done") && Sub(second,NextRed)=="ready";
if(!baseline && !target)throw new InvalidOperationException("RS030 plan is not the admitted baseline or exact completed target.");
var qualified=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Previous)))!.AsObject();
if((string?)qualified["status"]!="immutable-bounded-typed-IR-chunk-qualified")
    throw new InvalidOperationException("Prior in-memory IR gate missing.");
var aura=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)aura["status"]!="draft" || (string?)aura["transmission"]!="none")
    throw new InvalidOperationException("The M2 run cannot claim external adoption.");

p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    foreach(var (name,marker) in new[]{
        ("CanonicalExactIrJsonPublicCodecMustExist","M2-W01-C03-T1 RED: lossless canonical IR JSON codec is absent."),
        ("PublicEncodeAndDecodeContractsMustBeInstalled","M2-W01-C03-T1 RED: versioned IR Encode and Decode are missing.")
    })
    {
        var red=Run(root,"dotnet","test",TestProject,"-c","Release",
            "--filter","FullyQualifiedName~M2W01C03RedTests."+name,"--logger","console;verbosity=normal");
        if(red.ExitCode==0 || !red.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Missing independent codec RED: "+name+" | "+Tail(red.Output));
    }
}
var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C03-functional-red",
    ["status"]="two-independent-absent-canonical-codec-reds-observed",
    ["prior_bounded_ir"]=Previous,
    ["observed_absences"]=p.Json.StringArray(
        "Public math-ir/1 canonical codec not installed before GREEN.",
        "Encode/Decode public pair missing before GREEN."),
    ["limits"]="RED is absence evidence only, not a claim of codec or external integration success."
};
p.Files.WriteComplete(RedProof,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var path in new[]{Codec,Tests,Doc})
    p.Files.ReplaceFromStaged("staged/"+path,path);
Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach(var method in new[]{
    "CanonicalExactIrJsonPublicCodecMustExist","PublicEncodeAndDecodeContractsMustBeInstalled"})
    Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C03RedTests."+method,"--logger","console;verbosity=normal");
foreach(var method in new[]{
    "HugeCanonicalRationalRoundTripsWithoutDoubleOrDecimalLoss",
    "ApproximateIeeeBitsPreserveNegativeZeroExactly",
    "QuantitiesRoundTripCanonicalUnitEightDimensionsAndTemperature",
    "InformationDimensionUsesEighthExponentAfterRoundTrip",
    "BoundAndFreeSameLabelKeepDifferentStableIdentities",
    "RationalExclusionAndExpressionRemainStructurallyIdentical",
    "RectangularMatrixPreservesExactCellOrderAndValues",
    "RepeatedCanonicalEncodingIsByteIdenticalUnderSamePolicy",
    "UnsupportedVersionAndNonCanonicalRationalsAreNotAccepted",
    "UnknownBindingIdentityDoesNotSilentlyChangeScope"})
    Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C03Tests."+method,"--logger","console;verbosity=normal");
Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore");
Required(root,"dotnet","test","tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
Required(root,"dotnet","pack","src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "-c","Release","--no-build","--no-restore","-o","build/artifacts/ir-package-feed");
var package=Path.Combine(root,"build/artifacts/ir-package-feed/dw.tools.math.ir.0.3.0-preview.1.nupkg");
if(!File.Exists(package))throw new InvalidOperationException("IR package artifact missing.");

var receipt=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C03-functional-qualified",
    ["status"]="versioned-exact-ir-json-codec-functional-qualified-boundary-hardening-pending",
    ["prior_bounded_ir"]=Previous,
    ["independent_red"]=RedProof,
    ["source"]=Codec,
    ["source_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Codec))),
    ["schema_version_id"]="math-ir/1",
    ["package_id"]="dw.tools.math.ir",
    ["package_version"]="0.3.0-preview.1",
    ["package_sha256"]=Hash(File.ReadAllBytes(package)),
    ["red_tests_green"]=2,
    ["independent_round_trip_oracles"]=10,
    ["full_ir_and_quantities_tests_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["contracts"]=p.Json.StringArray(
        "BigInteger numerator and denominator remain exact decimal JSON strings without binary64 widening.",
        "Finite binary64 retains IEEE-754 bit patterns including negative zero.",
        "Canonical unit ID, explicit system, all eight dimensions and temperature kind round trip.",
        "Free and bound symbol identities and scope remain distinct.",
        "Matrices, operations and x != 1 restrictions preserve structure.",
        "Repeated encoding and encode-decode-encode are byte-identical for the admitted subset.",
        "Unknown version, noncanonical rational and mismatched symbol identity are rejected."),
    ["pending"]=p.Json.StringArray(
        "T2 must reject duplicate and unknown fields, malformed shapes and unsafe scopes with independent RED/GREEN.",
        "T2 must distinguish semantic structural hash from presentation hash; structural bytes do not establish algebraic equivalence.",
        "No universal CAS, external OpenMath/MathML codec or AURA adoption is claimed."),
    ["adoption_transmitted"]=false
};
p.Files.WriteComplete(GreenProof,receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",doc=>
{
    var c=doc["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t1=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T1);
    var t2=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T2);
    doc["plan_version"]="0.1.29";
    c["status"]="in_progress";
    c["refinement"]="RS030 independently observes two missing-public-codec REDs and qualifies math-ir/1 canonical structural JSON encoding and decoding for huge exact rationals, finite binary64 bits, quantities, typed symbols, matrices, operations and restrictions. Ten direct round-trip oracles and full IR/quantity/foundation regressions pass. T2 owns adversarial malformed-input and hash semantics.";
    c["files"]=p.Json.StringArray(Codec,RedTests,Tests,Doc,RedProof,GreenProof);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M2W01C03Tests",
        "dotnet test "+TestProject+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    c["evidence"]=p.Json.StringArray(RedProof,GreenProof);
    t1["status"]="done";
    t1["evidence"]=p.Json.StringArray(RedProof,GreenProof);
    foreach(var sub in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedProof:GreenProof);
    }
    t2["status"]="ready";
    t2["subtasks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==NextRed)["status"]="ready";
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Phase,"ready","in-progress");
    p.ProjectPlan.TransitionNode(T1,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T1,"ready","in-progress");
    foreach(var id in new[]{Red,Green,Verify})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(T1);
    p.ProjectPlan.TransitionNode(T2,"not-ready","ready");
    p.ProjectPlan.TransitionNode(NextRed,"not-ready","ready");
}
else
{
    foreach(var id in new[]{T1,Red,Green,Verify})p.ProjectPlan.RequireNodeState(id,"done");
    foreach(var id in new[]{T2,NextRed})p.ProjectPlan.RequireNodeState(id,"ready");
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
}
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Tail(string output)=>output.Length>3000?output[^3000..]:output;
static (int ExitCode,string Output) Run(string root,string exe,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=exe,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+exe);
    var a=proc.StandardOutput.ReadToEndAsync();
    var b=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000)){proc.Kill(entireProcessTree:true);throw new TimeoutException(exe);}
    var output=a.GetAwaiter().GetResult()+"\n"+b.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static void Required(string root,string exe,params string[] args)
{
    var result=Run(root,exe,args);
    if(result.ExitCode!=0)throw new InvalidOperationException(exe+" "+
        string.Join(" ",args)+" exited "+result.ExitCode+": "+Tail(result.Output));
}
