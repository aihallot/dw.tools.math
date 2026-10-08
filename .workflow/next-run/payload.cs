using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Red = "M1-W01-C01-T2-R";
const string Marker = "M1-W01-C01-T2 RED: default ExactRational exposes zero denominator; default policy is unresolved.";
const string Evidence = "docs/planning/evidence/M1-W01-C01-boundary-red.json";
var p = PayloadContext.Create();
p.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/M1W01C01BoundaryTests.cs",
    "tests/projects/dw.quantities.tests/M1W01C01BoundaryTests.cs");

var observed = Run(p.RepositoryRoot, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release",
    "--filter", "FullyQualifiedName~M1W01C01BoundaryTests",
    "--logger", "console;verbosity=normal");
if (observed.Code == 0 || !observed.Text.Contains(Marker, StringComparison.Ordinal))
    throw new InvalidOperationException("T2 RED not proven for the declared default-value failure: " + observed.Text);

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C01-boundary-red",
    ["status"] = "expected-red-observed",
    ["compiled_type"] = "dw.quantities.ExactRational",
    ["test_filter"] = "FullyQualifiedName~M1W01C01BoundaryTests",
    ["observed_failure_marker"] = Marker,
    ["observed_default_denominator"] = 0,
    ["test_exit_code_nonzero"] = true,
    ["source_unchanged"] = true,
    ["meaning"] = "The record struct default exposes an invalid denominator; GREEN must decide and implement explicit default handling.",
    ["scope_limit"] = "The bounded RED does not impose arbitrary integer size limits or approximate conversion."
};
p.Files.WriteComplete(Evidence, evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
p.Json.EditObject("docs/planning/backlog.json", root =>
{
    root["plan_version"] = "0.1.8";
    var chunk = root["chunks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01");
    var task = chunk["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01-T2");
    var red = task["subtasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Red);
    var green = task["subtasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01-T2-G");
    if ((string?)task["status"] is not ("ready" or "in_progress") ||
        (string?)red["status"] is not ("ready" or "done") ||
        (string?)green["status"] is not ("planned" or "ready"))
        throw new InvalidOperationException("T2 product baseline/target mismatch.");
    task["status"] = "in_progress";
    red["status"] = "done";
    red["evidence"] = p.Json.StringArray(Evidence);
    green["status"] = "ready";
    var chunkEvidence = chunk["evidence"]!.AsArray();
    if (!chunkEvidence.Any(x => (string?)x == Evidence))
        chunkEvidence.Add((JsonNode?)JsonValue.Create(Evidence));
});

p.ProjectPlan.TransitionNode("M1-W01-C01-T2", "ready", "in-progress");
p.ProjectPlan.TransitionNode(Red, "ready", "in-progress");
p.ProjectPlan.ConvergeNodeToDone(Red);
p.ProjectPlan.TransitionNode("M1-W01-C01-T2-G", "not-ready", "ready");

var rendered = Run(p.RepositoryRoot, "dotnet", "run", "--file",
    "docs/planning/ValidatePlan.cs", "--", "--write");
if (rendered.Code != 0) throw new InvalidOperationException("Planning renderer failed: " + rendered.Text);
return p.Complete();

static (int Code, string Text) Run(string root, string executable, params string[] args)
{
    using var proc = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    foreach (var arg in args) proc.StartInfo.ArgumentList.Add(arg);
    if (!proc.Start()) throw new InvalidOperationException("Cannot start " + executable);
    var stdout = proc.StandardOutput.ReadToEndAsync();
    var stderr = proc.StandardError.ReadToEndAsync();
    if (!proc.WaitForExit(360000))
    {
        proc.Kill(entireProcessTree: true);
        throw new TimeoutException("Command timed out: " + executable);
    }
    var text = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(text);
    return (proc.ExitCode, text);
}
