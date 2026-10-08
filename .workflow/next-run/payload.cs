using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C03";
const string WorkPackage = "M1-W02";
const string First = "M1-W02-C03-T1";
const string Second = "M1-W02-C03-T2";
const string FirstRed = "M1-W02-C03-T1-R";
const string SecondRed = "M1-W02-C03-T2-R";
const string Parser = "src/projects/dw.quantities.expression/ExpressionParser.cs";
const string Selection = "src/projects/dw.quantities.expression/ExactSelection.cs";
const string Diagnostics = "src/projects/dw.quantities.expression/ExpressionDiagnostics.cs";
const string RedTests = "tests/projects/dw.quantities.tests/M1W02C03RedTests.cs";
const string Tests = "tests/projects/dw.quantities.tests/M1W02C03Tests.cs";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string RedEvidence = "docs/planning/evidence/M1-W02-C03-red.json";
const string GreenEvidence = "docs/planning/evidence/M1-W02-C03-qualified.json";
const string PriorParserEvidence = "docs/planning/evidence/M1-W02-C02-boundary-qualified.json";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var c=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
var wp=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==WorkPackage);
var tasks=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
var t1=tasks.Single(x=>(string?)x["id"]==First);
var t2=tasks.Single(x=>(string?)x["id"]==Second);
string Sub(JsonObject t,string name)=> (string?)t["subtasks"]!.AsArray()
    .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==name)["status"]
    ?? throw new InvalidOperationException("Unknown subtask "+name);
var baseline=(string?)backlog["plan_version"]=="0.1.22" &&
    (string?)wp["status"]=="in_progress" && (string?)c["status"]=="planned" &&
    tasks.All(t=>(string?)t["status"]=="planned") &&
    new[]{"R","G","V"}.All(s=>Sub(t1,First+"-"+s)=="planned" &&
                                  Sub(t2,Second+"-"+s)=="planned");
var target=(string?)backlog["plan_version"]=="0.1.23" &&
    (string?)wp["status"]=="done" && (string?)c["status"]=="done" &&
    tasks.All(t=>(string?)t["status"]=="done") &&
    new[]{"R","G","V"}.All(s=>Sub(t1,First+"-"+s)=="done" &&
                                  Sub(t2,Second+"-"+s)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS024 status is neither the accepted baseline nor complete target.");

var prior=JsonNode.Parse(File.ReadAllText(Path.Combine(root,PriorParserEvidence)))!.AsObject();
var expectedSha=(string?)prior["modified_parser_sha256"]
    ?? throw new InvalidOperationException("Missing RS023 exact parser SHA.");
if(baseline && Sha(File.ReadAllBytes(Path.Combine(root,Parser)))!=expectedSha)
    throw new InvalidOperationException("Pre-RED exact parser differs from the RS023 qualified baseline.");

p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    var reds=new[]{
        ("StableSelectionOriginContractIsAbsentBeforeImplementation",
         "M1-W02-C03-T1 RED: stable-selection operand index is not exposed."),
        ("NeutralDiagnosticContractIsAbsentBeforeImplementation",
         "M1-W02-C03-T2 RED: stable bounded neutral diagnostic API is not exposed.")
    };
    foreach(var (method,marker) in reds)
    {
        var result=Run(root,"dotnet","test",TestProject,
            "-c","Release",
            "--filter","FullyQualifiedName~M1W02C03RedTests."+method,
            "--logger","console;verbosity=normal");
        if(result.Code==0 || !result.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Expected missing-public-API RED absent: "+
                method+" | "+Relevant(result.Output));
    }
}
var redEvidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C03-red",
    ["status"]="two-independent-missing-contract-reds-observed",
    ["prior_parser_evidence"]=PriorParserEvidence,
    ["prior_parser_sha256"]=expectedSha,
    ["red_methods"]=p.Json.StringArray(
        "M1W02C03RedTests.StableSelectionOriginContractIsAbsentBeforeImplementation",
        "M1W02C03RedTests.NeutralDiagnosticContractIsAbsentBeforeImplementation"),
    ["limits"]=p.Json.StringArray(
        "Missing selection-origin and neutral-diagnostic contracts were observed before product changes.",
        "Existing abs/min/max source semantics and stability claims must still be exercised through direct public APIs.")
};
p.Files.WriteComplete(RedEvidence,
    redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var path in new[]{Parser,Selection,Diagnostics,Tests})
    p.Files.ReplaceFromStaged("staged/"+path,path);

RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach(var name in new[]{
    "MinEquivalentMetreAndCentimetreIsExactAndStable",
    "SelectionStableTiesPreserveFirstIndexAndChooseLaterStrictImprovements",
    "NestedMinAndMaxRemainRationalAndRespectOriginalDimensions",
    "SelectionInputRequiresMultipleCompatibleOperands",
    "AbsoluteTemperatureSelectionPreservesAffineSemanticsButAbsRejectsPoints",
    "SelectionArityAndUnknownFunctionsProduceStableSyntaxFailures",
    "DivisionByZeroAndIncompatibleUnitsAreNotReclassified",
    "EveryExpressionFailureHasDistinctStableNeutralCode",
    "DiagnosticsContainBoundedMessagesAndOriginalPositionsWithoutUserInput",
    "DiagnosticsStayBoundedForMaximumAdmittedInputAndNeverEchoIt",
    "ErrorCategoriesRemainTypedAtParserBoundary"})
    RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M1W02C03Tests."+name,
        "--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M1W02C03",
    "--logger","console;verbosity=minimal");
RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var evidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C03-qualified",
    ["status"]="exact-selection-stable-ties-and-bounded-neutral-diagnostics-qualified",
    ["red"]=RedEvidence,
    ["production_files"]=p.Json.StringArray(Parser,Selection,Diagnostics),
    ["changed_parser_sha256"]=Sha(File.ReadAllBytes(Path.Combine(root,Parser))),
    ["focused_filter"]="FullyQualifiedName~M1W02C03",
    ["red_tests_passed_after_green"]=true,
    ["independent_exact_tests_passed"]=11,
    ["dw_quantities_suite_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["tested_contracts"]=p.Json.StringArray(
        "min(1 m,100 cm) returns the exact metre result; a public selection result exposes source index zero on exact ties.",
        "Stable min/max preserve first equal input, choose later strictly better values, and reject fewer than two or incompatible operands.",
        "Nested min/max and abs(-2 m) retain exact rational values and dimensional semantics.",
        "abs(absolute temperature), unknown functions, wrong arity, incompatible quantities, ambiguous units and division by zero return typed errors.",
        "Every accepted ExpressionFailureKind has a unique stable math.expression.* code; unknown enum values are refused.",
        "Diagnostic messages are fixed, neutral and bounded to 160 characters, preserve returned positions and never echo untrusted expression text.",
        "Previously qualified exact grammar, overflow hardening, resource limits, culture isolation, injected resolver and unit-profile behavior continue passing full regressions."),
    ["limitations"]=p.Json.StringArray(
        "The expression result still does not retain full parse-tree provenance; the separate ExactSelection API exposes selected operand index for callers that require it.",
        "Messages are fixed neutral English strings for programmatic diagnostics, not localized user-facing presentation.",
        "A general CPU/memory guarantee or AURA external consumer adoption is not claimed.",
        "Selection and diagnostics use fixed functions and enum classification, not a dynamic or executable function registry.")
};
p.Files.WriteComplete(GreenEvidence,
    evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",product=>
{
    var chunk=product["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    var work=product["work_packages"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==WorkPackage);
    product["plan_version"]="0.1.23";
    chunk["status"]="done";
    chunk["refinement"]="RS024 observes two public API absence REDs, adds exact indexed stable tie selection and bounded neutral diagnostic codes, verifies eleven selection/error oracles and full locked Math/foundation regressions, and closes C03 and M1-W02. Previously qualified abs/min/max, grammar bounds, AURA-neutral catalog and typed failure semantics remain in force.";
    chunk["files"]=p.Json.StringArray(
        Parser,Selection,Diagnostics,RedTests,Tests,RedEvidence,GreenEvidence);
    chunk["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M1W02C03",
        "dotnet test "+TestProject+" -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    chunk["evidence"]=p.Json.StringArray(RedEvidence,GreenEvidence);
    foreach(var task in chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedEvidence,GreenEvidence);
        foreach(var sub in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            sub["evidence"]=p.Json.StringArray(
                ((string?)sub["id"])!.EndsWith("-R",StringComparison.Ordinal)
                    ? RedEvidence : GreenEvidence);
        }
    }
    work["status"]="done";
    work["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M1-W02-C01-source-parity-qualified.json",
        "docs/planning/evidence/M1-W02-C02-boundary-qualified.json",
        GreenEvidence);
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    foreach(var taskName in new[]{First,Second})
    {
        p.ProjectPlan.TransitionNode(taskName,"not-ready","ready");
        var red=taskName+"-R";
        var green=taskName+"-G";
        var verify=taskName+"-V";
        p.ProjectPlan.TransitionNode(red,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(red);
        p.ProjectPlan.ConvergeNodeToDone(red);
        p.ProjectPlan.TransitionNode(green,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(green);
        p.ProjectPlan.ConvergeNodeToDone(green);
        p.ProjectPlan.TransitionNode(verify,"not-ready","ready");
        p.ProjectPlan.ActivateReadyContinuation(verify);
        p.ProjectPlan.ConvergeNodeToDone(verify);
        p.ProjectPlan.ConvergeNodeToDone(taskName);
    }
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(WorkPackage);
}
else
{
    foreach(var id in new[]{First+"-R",First+"-G",First+"-V",First,
        Second+"-R",Second+"-G",Second+"-V",Second,Phase,WorkPackage})
        p.ProjectPlan.RequireNodeState(id,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Sha(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var tail=i<0?output:output[i..];
    return tail.Length>3000?tail[..3000]:tail;
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
    if(!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(exe+" timed out");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static void RunRequired(string root,string exe,params string[] args)
{
    var r=Run(root,exe,args);
    if(r.Code!=0)throw new InvalidOperationException(
        exe+" "+string.Join(" ",args)+" exited "+r.Code+": "+Relevant(r.Output));
}
