using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C02";
const string T1 = "M1-W02-C02-T1";
const string T2 = "M1-W02-C02-T2";
const string Green = "M1-W02-C02-T1-G";
const string Verify = "M1-W02-C02-T1-V";
const string RedPath = "tests/projects/dw.quantities.tests/M1W02C02RedTests.cs";
const string TestPath = "tests/projects/dw.quantities.tests/M1W02C02Tests.cs";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string Parser = "src/projects/dw.quantities.expression/ExpressionParser.cs";
const string Source = "docs/planning/evidence/M1-W02-C02-AURA-ExpressionParser.txt";
const string SourceProject = "docs/planning/evidence/M1-W02-C02-AURA-expression-project.txt";
const string SourceSha = "6f553e58c5e4e788932b3d02cfadae2f8abecf564e8595296c8a91d7c4bfd765";
const string ProjectSha = "7c7c1d8bc7df4b69fc642af0b79c88721a38e9f8e706ff530973a8664f25f833";
const string ExpressionProject = "src/projects/dw.quantities.expression/dw.quantities.expression.csproj";
const string ExpressionLock = "src/projects/dw.quantities.expression/packages.lock.json";
const string TestLock = "tests/projects/dw.quantities.tests/packages.lock.json";
const string Solution = "dw.tools.math.slnx";
const string Evidence = "docs/planning/evidence/M1-W02-C02-green.json";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var t1 = chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T1);
var t2 = chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T2);
var subtasks = t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
string Sub(string id) => (string?)subtasks.Single(x=>(string?)x["id"]==id)["status"]
    ?? throw new InvalidOperationException("Missing T1 subtask status: "+id);
var baseline = (string?)backlog["plan_version"]=="0.1.20" &&
    (string?)chunk["status"]=="in_progress" &&
    (string?)t1["status"]=="in_progress" &&
    Sub(T1+"-R")=="done" && Sub(Green)=="ready" && Sub(Verify)=="planned" &&
    (string?)t2["status"]=="planned";
var target = (string?)backlog["plan_version"]=="0.1.21" &&
    (string?)chunk["status"]=="in_progress" &&
    (string?)t1["status"]=="done" &&
    Sub(T1+"-R")=="done" && Sub(Green)=="done" && Sub(Verify)=="done" &&
    (string?)t2["status"]=="ready";
if(!baseline && !target)
    throw new InvalidOperationException("RS022 product lifecycle neither accepted baseline nor exact target.");

var originals = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = originals["repositories"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["repository"]=="aura");
if((string?)aura["head"]!="82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("Pinned AURA source revision is not authoritative.");
var manifest = aura["files"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
if((string?)manifest.Single(x=>(string?)x["path"]=="lib/dw.quantities.expression/ExpressionParser.cs")["sha256"]!=SourceSha ||
   (string?)manifest.Single(x=>(string?)x["path"]=="lib/dw.quantities.expression/dw.quantities.expression.csproj")["sha256"]!=ProjectSha)
    throw new InvalidOperationException("Authorized expression parser source SHA manifest drifted.");

var parserBytes = File.ReadAllBytes(Path.Combine(root,Source));
var projectBytes = File.ReadAllBytes(Path.Combine(root,SourceProject));
if(Hash(parserBytes)!=SourceSha || Hash(projectBytes)!=ProjectSha)
    throw new InvalidOperationException("RS021 durable parser/project review sources differ from pinned SHA.");
var parserText = new UTF8Encoding(false,true).GetString(parserBytes);
if(!parserText.Contains("public static class ExpressionParser",StringComparison.Ordinal) ||
   !parserText.Contains("public interface IExpressionUnitResolver",StringComparison.Ordinal) ||
   !parserText.Contains("MaximumRootDegree = 16",StringComparison.Ordinal))
    throw new InvalidOperationException("Expected public parser API and fixed resource-budget declarations absent.");

// Copy exact authorized parser source; installation must match the SHA of the
// owner-attested AURA source reviewed under RS021.
var destination = Path.Combine(root,Parser);
if(File.Exists(destination) && Hash(File.ReadAllBytes(destination))!=SourceSha)
    throw new InvalidOperationException("Existing parser source differs from approved AURA source.");
p.Files.WriteComplete(Parser,parserText);
if(Hash(File.ReadAllBytes(destination))!=SourceSha)
    throw new InvalidOperationException("Installed exact parser bytes do not match pinned AURA SHA.");

foreach(var path in new[]{ExpressionProject,ExpressionLock,TestProject,TestLock,Solution,TestPath})
    p.Files.ReplaceFromStaged("staged/"+path,path);

RunRequired(root,"dotnet","restore",Solution,"--locked-mode");
RunRequired(root,"dotnet","build",Solution,"-c","Release","--no-restore");
foreach(var name in new[]{
    "ExactFractionsAddAndOperatorsRespectPrecedence",
    "RationalRootsAndPowersRemainExactWithoutFloatingPoint",
    "SelectionFunctionsRemainExact",
    "ResolverIsInjectedAndInformationDimensionsRetainPowers",
    "UnsupportedCultureAndAmbiguityAreTypedFailures",
    "AbsoluteTemperatureAlgebraRejectsInvalidExpressions",
    "CharacterCountLimitAccepts4096AndRejects4097",
    "TokenCountLimitAccepts255AndRejects257",
    "NestingDepthLimitAccepts32PrimariesAndRejects33",
    "DecimalMagnitudeLimitAccepts256DigitsAndRejects257",
    "InvalidSyntaxAndFreeVariablesNeverExecuteCSharp"
})
    RunRequired(root,"dotnet","test",TestProject,
        "-c","Release","--no-restore","--no-build",
        "--filter","FullyQualifiedName~M1W02C02Tests."+name,
        "--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",TestProject,
    "-c","Release","--no-restore","--no-build",
    "--filter","FullyQualifiedName~M1W02C02",
    "--logger","console;verbosity=minimal");
RunRequired(root,"dotnet","test",TestProject,
    "-c","Release","--no-restore","--no-build",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var evidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C02-green",
    ["status"]="exact-parser-functional-green-verified-boundary-integration-pending",
    ["source_revision"]="82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["source_snapshot"]=Source,
    ["source_sha256"]=SourceSha,
    ["installed_parser"]=Parser,
    ["installed_parser_sha256"]=Hash(File.ReadAllBytes(destination)),
    ["source_project_sha256"]=ProjectSha,
    ["packaging"]="Standalone dw.quantities.expression project references only dw.quantities; test consumer references both with NuGet locked-mode and compiles without AURA.",
    ["focused_filter"]="FullyQualifiedName~M1W02C02Tests",
    ["focused_test_count"]=11,
    ["focused_passed"]=true,
    ["complete_dw_quantities_suite_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["verified_oracles"]=p.Json.StringArray(
        "1/3 + 1/6 = 1/2 exactly; arithmetic precedence, unary signs and min/max/abs exact.",
        "root(81/16,2)=9/4 and root(65536,16)=2; irrational or unsupported-degree roots reject exact mode.",
        "Powers preserve exact fractions and enforce an explicit +/-16 exponent domain.",
        "Injected IExpressionUnitResolver controls unit meaning; dimensions and information exponents remain correct.",
        "Absolute-temperature arithmetic is rejected or yields interval semantics as declared.",
        "4096-character, 256-token, depth-32, 256-digit and power/root-16 limits are tested at their actual public input boundary.",
        "C# syntax and free variables reject as typed syntax errors; unknown units and incompatible dimensions return typed failure."),
    ["boundary_pending"]=p.Json.StringArray(
        "T2 must independently characterize integer dimension exponent overflow and adversarial overflow/resource paths, not just the ordinary exact grammar cases covered by T1.",
        "T2 must integrate the previously qualified explicit-profile standard resolver without allowing process culture to choose ambiguous aliases.",
        "No unconstrained CPU/memory resource bound or host facade adoption is claimed; exact source transfer alone does not prove such guarantees.")
};
p.Files.WriteComplete(Evidence,evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true })+"\n");

p.Json.EditObject("docs/planning/backlog.json",product=>{
    var c=product["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var task=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T1);
    var next=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T2);
    product["plan_version"]="0.1.21";
    c["status"]="in_progress";
    c["refinement"]="RS021 pinned four complete parser/project/test/host sources and qualified independent missing-assembly RED. RS022 transfers exact authorized pure ExpressionParser bytes into new locked standalone dw.quantities.expression, verifies eleven independent rational, root, injected-resolver and public budget oracles plus full project/foundation regressions, and leaves T2 integration/adversarial boundary qualification open.";
    c["files"]=p.Json.StringArray(Parser,ExpressionProject,ExpressionLock,TestProject,TestLock,
        Solution,RedPath,TestPath,
        "docs/planning/evidence/M1-W02-C02-source-red.json",Source,SourceProject,Evidence);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W02C02Tests",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs");
    var ev=c["evidence"]!.AsArray();
    if(!ev.Any(x=>(string?)x==Evidence))ev.Add((JsonNode?)JsonValue.Create(Evidence));
    task["status"]="done";
    task["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M1-W02-C02-source-red.json",Evidence);
    foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        if((string?)sub["id"]!=T1+"-R")
            sub["evidence"]=p.Json.StringArray(Evidence);
    }
    next["status"]="ready";
    next["subtasks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==T2+"-R")["status"]="ready";
});
if(baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(Green);
    p.ProjectPlan.ConvergeNodeToDone(Green);
    p.ProjectPlan.TransitionNode(Verify,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Verify);
    p.ProjectPlan.ConvergeNodeToDone(T1);
    p.ProjectPlan.TransitionNode(T2,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T2+"-R","not-ready","ready");
}
else
{
    foreach(var id in new[]{Green,Verify,T1})p.ProjectPlan.RequireNodeState(id,"done");
    foreach(var id in new[]{T2,T2+"-R"})p.ProjectPlan.RequireNodeState(id,"ready");
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Hash(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string ImportantFailure(string output)
{
    var at=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(at<0)at=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var relevant=at<0?output:output[at..];
    return relevant.Length>3000?relevant[..3000]:relevant;
}
static (int Code,string Output) Run(string root,string exe,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=exe,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+exe);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000)){
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(exe+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static void RunRequired(string root,string exe,params string[] args)
{
    var result=Run(root,exe,args);
    if(result.Code!=0)
        throw new InvalidOperationException(exe+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+ImportantFailure(result.Output));
}
