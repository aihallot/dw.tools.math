using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Source = "src/projects/dw.quantities/ExactRational.cs";
const string Tests = "tests/projects/dw.quantities.tests/M1W01C01BoundaryTests.cs";
const string Evidence = "docs/planning/evidence/M1-W01-C01-boundary-green.json";
var p = PayloadContext.Create();

p.Files.ReplaceFromStaged("staged/src/projects/dw.quantities/ExactRational.cs", Source);
p.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/M1W01C01BoundaryTests.cs", Tests);

var result = Run(p.RepositoryRoot, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--filter", "FullyQualifiedName~M1W01C01BoundaryTests",
    "--logger", "console;verbosity=minimal");
if (result.Code != 0)
    throw new InvalidOperationException("RS010 ExactRational boundary GREEN failed: " + result.Text);

var sourceSha = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(p.RepositoryRoot,Source)))).ToLowerInvariant();
var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C01-boundary-green",
    ["status"] = "green-tested-verification-pending",
    ["compiled_type"] = "dw.quantities.ExactRational",
    ["source_path"] = Source,
    ["source_sha256"] = sourceSha,
    ["default_policy"] = "default(ExactRational) is canonical zero: numerator 0, denominator 1; equals ExactRational.Zero.",
    ["storage"] = "The denominator is stored as denominator minus one; no double conversion.",
    ["test_project"] = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    ["test_filter"] = "FullyQualifiedName~M1W01C01BoundaryTests",
    ["targeted_tests_passed"] = true,
    ["source_aura_modified"] = false,
    ["remaining_verification"] = "T2-V must verify integration, resource-budget behavior and test coverage; no arbitrary numeric size limit is claimed."
};
p.Files.WriteComplete(Evidence, evidence.ToJsonString(new JsonSerializerOptions {WriteIndented=true})+"\n");
p.Json.EditObject("docs/planning/backlog.json", root =>
{
    root["plan_version"] = "0.1.9";
    var chunk = root["chunks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W01-C01");
    var task = chunk["tasks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W01-C01-T2");
    var green = task["subtasks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W01-C01-T2-G");
    var verify = task["subtasks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W01-C01-T2-V");
    var baseline = (string?)green["status"] == "ready" && (string?)verify["status"] == "planned";
    var target = (string?)green["status"] == "done" && (string?)verify["status"] == "ready";
    if (!baseline && !target || (string?)task["status"] != "in_progress")
        throw new InvalidOperationException("RS010 product status neither baseline nor target.");
    green["status"] = "done";
    green["evidence"] = p.Json.StringArray(Evidence);
    verify["status"] = "ready";
    var evidenceList = chunk["evidence"]!.AsArray();
    if (!evidenceList.Any(n => (string?)n == Evidence))
        evidenceList.Add((JsonNode?)JsonValue.Create(Evidence));
});

p.ProjectPlan.ActivateReadyContinuation("M1-W01-C01-T2-G");
p.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T2-G");
p.ProjectPlan.TransitionNode("M1-W01-C01-T2-V","not-ready","ready");

var render = Run(p.RepositoryRoot, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
if (render.Code != 0)
    throw new InvalidOperationException("Planning regeneration failed: " + render.Text);
return p.Complete();

static (int Code,string Text) Run(string root,string exe,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=exe, WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    foreach(var a in args)proc.StartInfo.ArgumentList.Add(a);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+exe);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000)){
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(exe+" timed out");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
