using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string TestPath = "tests/projects/dw.quantities.tests/M1W01C02Tests.cs";
const string Evidence = "docs/planning/evidence/M1-W01-C02-red.json";
const string BinaryPath = "lib/dw.quantities/ExactBinaryNumber.cs";
const string DecimalPath = "lib/dw.quantities/ExactDecimalFormatter.cs";
const string BinaryHash = "cda9835b8a83f01239d344bd83b4144c7a6e9dcad59d9c2cc385103b76a2225d";
const string DecimalHash = "20499543d9635a291a54ff91763ece266176fa08bf67421d173caae5f59d45ce";
const string RedMarker = "M1-W01-C02-T1 RED: ExactBinaryNumber and ExactDecimalFormatter are absent";

var p = PayloadContext.Create();
p.Files.ReplaceFromStaged("staged/" + TestPath, TestPath);

var baseline = JsonNode.Parse(File.ReadAllText(Path.Combine(p.RepositoryRoot,"docs/planning/source-baseline.json")))!.AsObject();
var aura = baseline["repositories"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["repository"]=="aura");
var root = (string?)aura["observed_root"] ?? throw new InvalidOperationException("Missing pinned AURA root.");
var entries = aura["files"]!.AsArray().Select(n=>n!.AsObject()).ToArray();
JsonObject Inspect(string relative, string expectedHash)
{
    var pinned = entries.Single(n=>(string?)n["path"]==relative);
    if ((string?)pinned["sha256"]!=expectedHash)
        throw new InvalidOperationException("AURA source manifest drifted: "+relative);
    var full = Path.Combine(root,relative.Replace('/',Path.DirectorySeparatorChar));
    var bytes = File.ReadAllBytes(full);
    var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if(hash!=expectedHash) throw new InvalidOperationException("AURA source bytes drifted: "+relative);
    var lines = System.Text.Encoding.UTF8.GetString(bytes).Split('\n');
    var surface = lines.Select(x=>x.Trim()).Where(x=>x.StartsWith("namespace ",StringComparison.Ordinal)||
        x.StartsWith("public ",StringComparison.Ordinal)).Take(64).ToArray();
    return new JsonObject {
        ["path"]=relative,
        ["sha256"]=hash,
        ["declaration_lines"]=p.Json.StringArray(surface),
        ["declaration_count_captured"]=surface.Length
    };
}
var binary = Inspect(BinaryPath,BinaryHash);
var decimalFormatter = Inspect(DecimalPath,DecimalHash);

var result=Run(p.RepositoryRoot,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--filter","FullyQualifiedName~M1W01C02Tests.TransferContractRemainsRedUntilBothTypesAreMaterialized",
    "--logger","console;verbosity=normal");
if(result.Code==0||!result.Text.Contains(RedMarker,StringComparison.Ordinal))
    throw new InvalidOperationException("C02 RED did not fail with the expected missing-types marker: "+result.Text);

var evidence = new JsonObject {
    ["schema_version"]=1,
    ["id"]="M1-W01-C02-red",
    ["status"]="expected-type-absence-observed",
    ["test_project"]="tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    ["test_filter"]="FullyQualifiedName~M1W01C02Tests.TransferContractRemainsRedUntilBothTypesAreMaterialized",
    ["observed_failure_marker"]=RedMarker,
    ["source_candidates"]=p.Json.Array(binary,decimalFormatter),
    ["accepted_future_oracles"]=p.Json.StringArray(
        "FromDouble(0.5) = 1/2",
        "FromDouble(0.1) = 3602879701896397/36028797018963968",
        "round +/-1.25 to one digit under documented mode",
        "reject NaN and infinity; test subnormal and signed-zero semantics"),
    ["limits"]="RED proves absence only; source declarations are read-only discovery, not behavior or boundary qualification."
};
p.Files.WriteComplete(Evidence,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

var fromBaseline=false;
p.Json.EditObject("docs/planning/backlog.json",j=>{
    var chunk=j["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W01-C02");
    var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W01-C02-T1");
    var red=task["subtasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W01-C02-T1-R");
    var green=task["subtasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W01-C02-T1-G");
    fromBaseline=(string?)chunk["status"]=="planned"&&(string?)task["status"]=="planned"&&
        (string?)red["status"]=="planned"&&(string?)green["status"]=="planned";
    var target=(string?)chunk["status"]=="in_progress"&&(string?)task["status"]=="in_progress"&&
        (string?)red["status"]=="done"&&(string?)green["status"]=="ready";
    if(!fromBaseline&&!target)throw new InvalidOperationException("C02 product progress outside baseline/target.");
    j["plan_version"]="0.1.11";
    chunk["status"]="in_progress";
    chunk["refinement"]="RS012 independently pins and inspects both authorized source candidates, records T1 RED before transfer, and reserves behavior/boundary qualification for subsequent exact tests.";
    chunk["files"]=p.Json.StringArray(
        "src/projects/dw.quantities/ExactBinaryNumber.cs",
        "src/projects/dw.quantities/ExactDecimalFormatter.cs",
        TestPath,
        Evidence);
    chunk["commands"]=p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C02Tests",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    chunk["evidence"]=p.Json.StringArray(Evidence);
    task["status"]="in_progress";
    red["status"]="done";
    red["evidence"]=p.Json.StringArray(Evidence);
    green["status"]="ready";
});
if(fromBaseline){
    p.ProjectPlan.TransitionNode("M1-W01-C02","not-ready","ready");
    p.ProjectPlan.TransitionNode("M1-W01-C02-T1","not-ready","ready");
    p.ProjectPlan.TransitionNode("M1-W01-C02-T1-R","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation("M1-W01-C02-T1-R");
    p.ProjectPlan.ConvergeNodeToDone("M1-W01-C02-T1-R");
    p.ProjectPlan.TransitionNode("M1-W01-C02-T1-G","not-ready","ready");
}else{
    p.ProjectPlan.RequireNodeState("M1-W01-C02","in-progress");
    p.ProjectPlan.RequireNodeState("M1-W01-C02-T1","in-progress");
    p.ProjectPlan.RequireNodeState("M1-W01-C02-T1-R","done");
    p.ProjectPlan.RequireNodeState("M1-W01-C02-T1-G","ready");
}
var render=Run(p.RepositoryRoot,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
if(render.Code!=0)throw new InvalidOperationException(render.Text);
return p.Complete();

static (int Code,string Text) Run(string root,string executable,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Failed to start "+executable);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(360000)){proc.Kill(entireProcessTree:true);throw new TimeoutException(executable);}
    var text=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(text);
    return(proc.ExitCode,text);
}
