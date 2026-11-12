using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Generator = "docs/discovery/qualification/GenerateM8DiscoveryManifest.cs";
const string Manifest = "docs/discovery/qualification/m8-discovery-manifest.json";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS186", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS185", 1,
    "feat: generate deterministic selected Math Discovery CLR binding receipt",
    "0.1.66", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C02", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-selected-clr-bindings.json",
    "/status", JsonValue.Create("selected-clr-fingerprints-only"));
p.Require.JsonValueEquals("docs/discovery/qualification/m8-selected-clr-bindings.json",
    "/brokerReadyCount", JsonValue.Create(0));
p.Files.ReplaceFromStaged("staged/" + Generator, Generator);
RunGenerator();
p.Require.JsonValueEquals(Manifest, "/schemaVersion", JsonValue.Create(2));
p.Require.JsonValueEquals(Manifest, "/owner", JsonValue.Create("math"));
return p.Complete();

static void RunGenerator()
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = Environment.CurrentDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in new[] { "run", "--file",
        "docs/discovery/qualification/GenerateM8DiscoveryManifest.cs",
        "--", "--write" })
        start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("Cannot run manifest generator.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(180000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Manifest generator timed out.");
    }
    if (process.ExitCode != 0)
        throw new InvalidOperationException("Manifest generator failed: " + stdout + stderr);
    Console.WriteLine(stdout);
}
