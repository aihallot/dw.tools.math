using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M2-W01-C01";
const string WorkPackage = "M2-W01";
const string Release = "M2";
const string T1 = "M2-W01-C01-T1";
const string T2 = "M2-W01-C01-T2";
const string Decision = "docs/planning/decisions/m2-w01-c01.md";
const string Mapping = "docs/planning/evidence/M2-W01-C01-mapping.json";
const string Validator = "docs/planning/ValidateM2IrDecision.cs";
const string Evidence = "docs/planning/evidence/M2-W01-C01-qualified.json";
const string M1Gate = "docs/planning/evidence/M1-W03-C02-gate.json";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var product=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var work=product["work_packages"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==WorkPackage);
var milestone=product["releases"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Release);
var phase=product["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Phase);
var tasks=phase["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
string Sub(JsonObject task,string part)=>(string?)task["subtasks"]!.AsArray()
    .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==(string?)task["id"]+"-"+part)["status"]
    ?? throw new InvalidOperationException("Missing study subtask state: "+part);
var baseline=(string?)product["plan_version"]=="0.1.25" &&
    (string?)milestone["status"]=="planned" &&
    (string?)work["status"]=="planned" && (string?)phase["status"]=="planned" &&
    tasks.Length==2 && tasks.All(x=>(string?)x["status"]=="planned") &&
    new[]{"A","B","C"}.All(s=>tasks.All(t=>Sub(t,s)=="planned"));
var target=(string?)product["plan_version"]=="0.1.26" &&
    (string?)milestone["status"]=="in_progress" &&
    (string?)work["status"]=="in_progress" && (string?)phase["status"]=="done" &&
    tasks.Length==2 && tasks.All(x=>(string?)x["status"]=="done") &&
    new[]{"A","B","C"}.All(s=>tasks.All(t=>Sub(t,s)=="done"));
if(!baseline && !target)
    throw new InvalidOperationException("RS027 lifecycle is neither M1-closed M2 baseline nor exact architecture study target.");

var lastGate=JsonNode.Parse(File.ReadAllText(Path.Combine(root,M1Gate)))!.AsObject();
if((string?)lastGate["status"]!="local-exact-m1-gate-qualified-external-adoption-not-started")
    throw new InvalidOperationException("The M1 local extraction milestone is not qualified.");
var external=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)external["status"]!="draft" || (string?)external["transmission"]!="none")
    throw new InvalidOperationException("RS027 cannot claim external AURA adoption or transmission.");

foreach(var path in new[]{Decision,Mapping,Validator})
    p.Files.ReplaceFromStaged("staged/"+path,path);

var ledger=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Mapping)))!.AsObject();
var rows=ledger["mapping"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
var desired=new[]{"rational","quantity","matrix","function","exclusion"};
if((string?)ledger["status"]!="proposed-not-implemented" ||
   (string?)ledger["selected_exact_primitive"]!="dw.quantities.ExactRational" ||
   ledger["new_bigrational_implementation"]?.GetValue<bool>()!=false ||
   rows.Length!=desired.Length ||
   !rows.Select(x=>(string)x["id"]!).ToHashSet(StringComparer.Ordinal).SetEquals(desired) ||
   rows.Any(x=>x["preserved"]?.AsArray().Count is not >0 ||
                x["unsupported"]?.AsArray().Count is not >0))
    throw new InvalidOperationException("The accepted five-row exact IR study or faithful mapping policy is incomplete.");
var decision=File.ReadAllText(Path.Combine(root,Decision));
foreach(var criterion in new[]{"OpenMath 2.0 revision 2","Content MathML","BigRational",
    "x != 1","sqrt(x²)","NaN","capture-free","canonical JSON","MaximumNodes=1024"})
    if(!decision.Contains(criterion,StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("IR architecture decision omits: "+criterion);

RunRequired(root,"dotnet","run","--file","docs/planning/ValidateM1Gate.cs","--","--check");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var mappingBytes=File.ReadAllBytes(Path.Combine(root,Mapping));
var decisionBytes=File.ReadAllBytes(Path.Combine(root,Decision));
var validatorBytes=File.ReadAllBytes(Path.Combine(root,Validator));
var result=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C01-qualified",
    ["status"]="typed-ir-adr-qualified-no-product-implementation",
    ["milestone"]="M2",
    ["phase"]=Phase,
    ["source_m1_gate"]=M1Gate,
    ["decision"]=Decision,
    ["decision_sha256"]=Hash(decisionBytes),
    ["mapping"]=Mapping,
    ["mapping_sha256"]=Hash(mappingBytes),
    ["read_only_validator"]=Validator,
    ["read_only_validator_sha256"]=Hash(validatorBytes),
    ["source_aware_standards"]=p.Json.StringArray(
        "OpenMath 2.0 Revision 2 standard (https://openmath.org/standard/)",
        "W3C MathML 4 Working Draft 2026-10-08 content markup (https://www.w3.org/TR/2026/WD-mathml4-20261008/)"),
    ["alternatives"]=p.Json.StringArray(
        "OpenMath content dictionaries as external semantics: not chosen as the internal data model.",
        "Strict Content MathML as external semantic encoding: not chosen as native model or fixed release codec.",
        "Typed immutable provider-neutral AST: selected for the admitted exact/approximate/quantity/binding/domain semantics.",
        "Provider-owned AST: rejected as a public canonical contract due to coupling and semantic drift."),
    ["observed_cases"]=p.Json.StringArray(desired),
    ["decision_reuse_exact_rational"]=true,
    ["decision_keep_binders_and_restrictions"]=true,
    ["decision_separate_exact_and_approximate"]=true,
    ["decision_defer_complex_and_intervals"]=true,
    ["resource_limits_proposed_not_enforced"]=true,
    ["product_ir_compiled"]=false,
    ["codec_implemented"]=false,
    ["adoption_transmitted"]=false,
    ["tests"]=p.Json.StringArray(
        "Read-only M1 release gate verified before product progression.",
        "Transfer architecture validator passed.",
        "All five structured IR mapping rows independently checked for exactness/loss/guard fields.",
        "Read-only M2 IR ADR validator executed after native/product state convergence.",
        "Canonical product planning validator executed with check-only mode after projection."),
    ["next_action"]="M2-W01-C02: RED test typed exact-vs-approximate AST, binding identities, explicit assumptions and bounded immutable nodes before implementing dw.tools.math.ir.",
    ["limits"]=p.Json.StringArray(
        "This is an architecture spike: no IR project, executable AST, codec or OpenMath/MathML serializer is created.",
        "Exact IR limits are prospective policy, not currently enforced as product code.",
        "No arbitrary equivalence, automatic simplification, provider implementation, symbolic solver or external consumer adoption is claimed.",
        "No AURA/Decision/MCDM sibling repository is changed.")
};
p.Files.WriteComplete(Evidence,result.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",backlog=>
{
    var c=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Phase);
    var w=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==WorkPackage);
    var m=backlog["releases"]!.AsArray().Select(x=>x!.AsObject())
        .Single(x=>(string?)x["id"]==Release);
    backlog["plan_version"]="0.1.26";
    c["status"]="done";
    c["refinement"]="RS027 compares OpenMath 2.0 Revision 2, W3C Content MathML 4 Draft and native/provider ASTs. It chooses a bounded immutable typed provider-neutral model, reuses ExactRational, separates symbolic scopes/domains/assumptions, records five exact mapping/loss cases, prospective limits and deferred kinds. A read-only C# decision validator verifies the ADR; no implementation package or serializer is claimed.";
    c["files"]=p.Json.StringArray(Decision,Mapping,Validator,Evidence);
    c["commands"]=p.Json.StringArray(
        "dotnet run --file docs/planning/ValidateM2IrDecision.cs -- --check",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check",
        "dotnet run --file docs/planning/ValidateM1Gate.cs -- --check",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs");
    c["evidence"]=p.Json.StringArray(Mapping,Evidence);
    foreach(var task in c["tasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Mapping,Evidence);
        foreach(var part in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            part["status"]="done";
            var isQuestion=((string?)part["id"])!.EndsWith("-A",StringComparison.Ordinal);
            var isObservation=((string?)part["id"])!.EndsWith("-B",StringComparison.Ordinal);
            part["evidence"]=p.Json.StringArray(isQuestion ? Decision : isObservation ? Mapping : Evidence);
        }
    }
    w["status"]="in_progress";
    m["status"]="in_progress";
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Release,"not-ready","ready");
    p.ProjectPlan.TransitionNode(WorkPackage,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    foreach(var id in new[]{T1,T2})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        foreach(var part in new[]{"A","B","C"})
        {
            var sub=id+"-"+part;
            p.ProjectPlan.TransitionNode(sub,"not-ready","ready");
            p.ProjectPlan.ActivateReadyContinuation(sub);
            p.ProjectPlan.ConvergeNodeToDone(sub);
        }
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var id in new[]{T1+"-A",T1+"-B",T1+"-C",T1,
        T2+"-A",T2+"-B",T2+"-C",T2,Phase})
        p.ProjectPlan.RequireNodeState(id,"done");
    p.ProjectPlan.RequireNodeState(WorkPackage,"in-progress");
    p.ProjectPlan.RequireNodeState(Release,"in-progress");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
RunRequired(root,"dotnet","run","--file",Validator,"--","--check");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string Relevant(string output)
{
    var pos=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(pos<0)pos=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var slice=pos<0?output:output[pos..];
    return slice.Length>3000?slice[..3000]:slice;
}
static (int ExitCode,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Could not start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" exceeded the bounded validation window.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string RunRequired(string root,string executable,params string[] args)
{
    var actual=Run(root,executable,args);
    if(actual.ExitCode!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+actual.ExitCode+": "+Relevant(actual.Output));
    return actual.Output;
}
