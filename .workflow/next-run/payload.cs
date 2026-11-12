using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Project = "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj";
const string Program = "tools/dw.tools.math.discovery-bindings/Program.cs";
const string Receipt = "docs/discovery/qualification/m8-selected-clr-bindings.json";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS185", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS184", 1,
    "test: qualify four Math Discovery CLR bindings and bounded IR examples",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/RS131-portable-protocol-qualified.json",
    "/total_cases", JsonValue.Create(13));
p.Files.ReplaceFromStaged("staged/" + Project, Project);
p.Files.ReplaceFromStaged("staged/" + Program, Program);
RunBindingGenerator();
p.Require.JsonValueEquals(Receipt, "/status", JsonValue.Create("selected-clr-fingerprints-only"));
p.Require.JsonValueEquals(Receipt, "/brokerReadyCount", JsonValue.Create(0));
return p.Complete();

static void RunBindingGenerator()
{
    var processStart = new ProcessStartInfo("dotnet")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Environment.CurrentDirectory
    };
    foreach (var value in new[] { "run", "--project",
        "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj",
        "-c", "Release", "--", "--write" })
        processStart.ArgumentList.Add(value);
    using var process = Process.Start(processStart)
        ?? throw new InvalidOperationException("Unable to run the binding generator.");
    var output = process.StandardOutput.ReadToEnd();
    var errors = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(240000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Binding generation timed out.");
    }
    if (process.ExitCode != 0)
        throw new InvalidOperationException("Binding generation failed: " + output + errors);
    Console.WriteLine(output);
}
