using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C02";
const string Task = "M1-W02-C02-T2";
const string Red = "M1-W02-C02-T2-R";
const string Green = "M1-W02-C02-T2-G";
const string Verify = "M1-W02-C02-T2-V";
const string Parser = "src/projects/dw.quantities.expression/ExpressionParser.cs";
const string Adapter = "src/projects/dw.quantities.standard/ExplicitProfileExpressionUnitResolver.cs";
const string StandardProject = "src/projects/dw.quantities.standard/dw.quantities.standard.csproj";
const string StandardLock = "src/projects/dw.quantities.standard/packages.lock.json";
const string TestLock = "tests/projects/dw.quantities.tests/packages.lock.json";
const string RedTests = "tests/projects/dw.quantities.tests/M1W02C02BoundaryRedTests.cs";
const string Tests = "tests/projects/dw.quantities.tests/M1W02C02BoundaryTests.cs";
const string RedEvidence = "docs/planning/evidence/M1-W02-C02-boundary-red.json";
const string Qualified = "docs/planning/evidence/M1-W02-C02-boundary-qualified.json";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string OriginalParserSha = "6f553e58c5e4e788932b3d02cfadae2f8abecf564e8595296c8a91d7c4bfd765";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var plan = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk = plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var task = chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
var subtasks = task["subtasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
string State(string id) => (string?)subtasks.Single(x=>(string?)x["id"]==id)["status"]
    ?? throw new InvalidOperationException("Subtask state missing: "+id);
var baseline = (string?)plan["plan_version"]=="0.1.21" &&
    (string?)chunk["status"]=="in_progress" && (string?)task["status"]=="ready" &&
    State(Red)=="ready" && State(Green)=="planned" && State(Verify)=="planned";
var target = (string?)plan["plan_version"]=="0.1.22" &&
    (string?)chunk["status"]=="done" && (string?)task["status"]=="done" &&
    State(Red)=="done" && State(Green)=="done" && State(Verify)=="done";
if(!baseline && !target)
    throw new InvalidOperationException("RS023 lifecycle does not match the approved baseline or exact target.");

var priorEvidence = JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/planning/evidence/M1-W02-C02-green.json")))!.AsObject();
if((string?)priorEvidence["installed_parser_sha256"]!=OriginalParserSha)
    throw new InvalidOperationException("The previously qualified parser SHA was not preserved in evidence.");
var currentParser = Path.Combine(root,Parser);
if(baseline && Sha(File.ReadAllBytes(currentParser))!=OriginalParserSha)
    throw new InvalidOperationException("The pre-RED parser is not the exact RS022 qualified AURA source.");

p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    var redNames=new[]{
        "PublicExponentEvaluationMustRejectDimensionIntOverflow",
        "PublicMultiplyEvaluationMustConvertOverflowToTypedFailure",
        "ExplicitProfileExpressionResolverMustBePubliclyInstalled"
    };
    var markers=new[]{
        "M1-W02-C02-T2 RED: public evaluation wraps dimension exponent multiplication.",
        "M1-W02-C02-T2 RED: public evaluation leaks checked dimension overflow.",
        "M1-W02-C02-T2 RED: explicit-profile expression resolver is not installed."
    };
    for(var i=0;i<redNames.Length;i++)
    {
        var run=Run(root,"dotnet","test",TestProject,
            "-c","Release","--filter","FullyQualifiedName~M1W02C02BoundaryRedTests."+redNames[i],
            "--logger","console;verbosity=normal");
        if(run.Code==0 || !run.Output.Contains(markers[i],StringComparison.Ordinal))
            throw new InvalidOperationException("Independent parser boundary RED was not observed: "+redNames[i]+
                " | "+Relevant(run.Output));
    }
}

var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C02-boundary-red",
    ["status"]="three-independent-direct-public-boundary-reds-observed",
    ["original_parser_sha256"]=OriginalParserSha,
    ["test_filter"]="FullyQualifiedName~M1W02C02BoundaryRedTests",
    ["observed_prechange_failures"]=p.Json.StringArray(
        "Unchecked Scale dimension exponent multiplication can return a wrapped dimension.",
        "Checked DimensionVector additions may leak OverflowException outside the typed Evaluate outcome.",
        "The public explicit-profile expression catalogue bridge is not installed before GREEN."),
    ["limitations"]="RED identifies the prechange boundary failures; it does not itself validate fixed code."
};
p.Files.WriteComplete(RedEvidence,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var file in new[]{Parser,Adapter,StandardProject,StandardLock,TestLock,Tests})
    p.Files.ReplaceFromStaged("staged/"+file,file);

RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach(var name in new[]{
    "PublicExponentEvaluationMustRejectDimensionIntOverflow",
    "PublicMultiplyEvaluationMustConvertOverflowToTypedFailure",
    "ExplicitProfileExpressionResolverMustBePubliclyInstalled"})
    RunRequired(root,"dotnet","test",TestProject,
        "-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M1W02C02BoundaryRedTests."+name,
        "--logger","console;verbosity=normal");
foreach(var name in new[]{
    "InheritedExactUnitsResolveThroughInjectedProfileBridge",
    "AmbiguousInheritedCupsNeedExplicitProfileAndExposeCandidates",
    "ParserCultureArgumentCannotChangeUnitProfile",
    "AdmittedAndInheritedCataloguesRemainExplicitChoices",
    "UnsupportedTokensCurrenciesAndFreeVariablesAreRejected",
    "ParserNeverReturnsWrappedDimensionsOrLeaksOverflowExceptions",
    "AdversarialInputsRetainTypedBoundsAndExactResults",
    "OriginalTemperatureUnitsRemainExactAndAffine"})
    RunRequired(root,"dotnet","test",TestProject,
        "-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M1W02C02BoundaryTests."+name,
        "--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",TestProject,
    "-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M1W02C02",
    "--logger","console;verbosity=minimal");
RunRequired(root,"dotnet","test",TestProject,
    "-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var evidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C02-boundary-qualified",
    ["status"]="bounded-parser-and-explicit-profile-unit-integration-qualified",
    ["baseline_source_sha256"]=OriginalParserSha,
    ["modified_parser_sha256"]=Sha(File.ReadAllBytes(currentParser)),
    ["source_change_policy"]="Constrained overflow hardening of AURA-origin parser after observed RED; original pinned source and RS022 exact transfer evidence remain preserved.",
    ["adapter"]=Adapter,
    ["project_dependencies"]="dw.quantities.standard -> dw.quantities.expression -> dw.quantities; no AURA/dw.localization and no circular package dependency.",
    ["independent_red"]=RedEvidence,
    ["targeted_reds_passed_after_fix"]=true,
    ["boundary_oracles_passed"]=8,
    ["dw_quantities_test_suite_passed"]=true,
    ["foundation_regressions_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["contracts"]=p.Json.StringArray(
        "Scale checks all eight dimensional exponents and cannot silently wrap int values.",
        "Arithmetic dimension overflows are represented as typed MagnitudeLimit outcomes at public Evaluate, not uncaught exceptions.",
        "Exact expression evaluation resolves 1 inch + 1 cm to 177/5000 metres and 2 KiB to 2048 B.",
        "Ambiguous cup and pint symbols produce typed UnitAmbiguous with complete sorted candidate IDs unless an explicit UnitSystem profile is selected.",
        "Ambient culture and the parser-provided culture argument cannot silently change the selected unit profile.",
        "The strict 14-unit and inherited 58-unit catalogues are intentionally separate consumer options.",
        "Invalid currencies, logarithmic units, C# input and free variables remain typed refusals.",
        "Original published length, token, nesting, magnitude and degree/power limits remain enforced in full exact regression."),
    ["limits"]=p.Json.StringArray(
        "OverflowException is intentionally translated to MagnitudeLimit with position 0; a more precise diagnostic position would require deeper parser refactoring.",
        "Math does not promise a universal CPU/memory budget for all exact operations; accepted integer and grammar limits define only the qualified scope.",
        "Legacy AURA culture-priority aliases are not emulated; Math requires explicit profiles.",
        "No AURA facade adoption or sibling-repository modification is claimed.")
};
p.Files.WriteComplete(Qualified,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",document=>{
    var c=document["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    document["plan_version"]="0.1.22";
    c["status"]="done";
    c["refinement"]="RS021 characterized and pinned AURA parser/source RED. RS022 transferred exact pure parser, compiling independent rational/root and six public bound oracles. RS023 observed independent exponent overflow and missing-adapter RED; hardened checked dimension scaling and typed overflow rejection, integrated explicit-profile catalog resolution, tested 8 additional boundary cases and full Math/foundation regressions. No AURA-host integration is claimed.";
    var files=c["files"]!.AsArray();
    foreach(var file in new[]{Parser,Adapter,StandardProject,StandardLock,TestLock,RedTests,Tests,RedEvidence,Qualified})
        if(!files.Any(x=>(string?)x==file))files.Add((JsonNode?)JsonValue.Create(file));
    var commands=c["commands"]!.AsArray();
    foreach(var cmd in new[]{
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M1W02C02BoundaryRedTests",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M1W02C02BoundaryTests"})
        if(!commands.Any(x=>(string?)x==cmd))commands.Add((JsonNode?)JsonValue.Create(cmd));
    var cEvidence=c["evidence"]!.AsArray();
    foreach(var path in new[]{RedEvidence,Qualified})
        if(!cEvidence.Any(x=>(string?)x==path))cEvidence.Add((JsonNode?)JsonValue.Create(path));
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,Qualified);
    foreach(var sub in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedEvidence:Qualified);
    }
});
if(baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    p.ProjectPlan.TransitionNode(Green,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Green);
    p.ProjectPlan.ConvergeNodeToDone(Green);
    p.ProjectPlan.TransitionNode(Verify,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var id in new[]{Red,Green,Verify,Task,Phase})
        p.ProjectPlan.RequireNodeState(id,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Sha(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string Relevant(string output)
{
    var start=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(start<0)start=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var interesting=start<0?output:output[start..];
    return interesting.Length>2800?interesting[..2800]:interesting;
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
        throw new TimeoutException(executable+" timed out");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode,output);
}

static void RunRequired(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
}
