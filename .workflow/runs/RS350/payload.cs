using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS349", 1, "chore: ready M9 bounded sampling from durable evidence", "0.1.76", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.125"), JsonValue.Create("0.1.126"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/AssessM8W02C02Acceptance.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM9BoundedSamplingRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/fixtures/M9BoundedSamplingRedOracle.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-red-rs350.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.125" or "0.1.126")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.126";
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var release = Find(doc, "releases", "M9");
    var wp = Find(doc, "work_packages", "M9-W01");
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var task = Find(chunk, "tasks", "M9-W01-C01-T1");
    var red = Find(task, "subtasks", "M9-W01-C01-T1-R");
    foreach (var node in new[] { release, wp, chunk, task, red })
    {
        if ((string?)node["status"] != (target ? "ready" : "planned")) throw new InvalidOperationException("Unexpected readiness baseline.");
        node["status"] = "ready";
    }
    chunk["files"] = p.Json.StringArray("src/projects/dw.tools.math.statistics/SplitMix64Random.cs", "tests/projects/dw.tools.math.statistics.tests/M9W01C01BoundedSamplingTests.cs", "docs/engineering/ProbeM9BoundedSamplingRed.cs", "docs/engineering/fixtures/M9BoundedSamplingRedOracle.cs");
    chunk["commands"] = p.Json.StringArray("dotnet run --file docs/engineering/ProbeM9BoundedSamplingRed.cs -- --check", "dotnet test tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj -c Release --filter FullyQualifiedName~M9W01C01BoundedSamplingTests");
    chunk["refinement"] = "RS349 durable readiness; RS268-RS278 historical source and immutable candidate qualification reused. Independently falsify current contract without repeating publication.";
    chunk["readiness"] = "Ready from RS349 durable evidence; native RED activation and current oracle qualification are separate.";
    doc["plan_version"] = "0.1.126";
});
using (var process = new Process { StartInfo = new ProcessStartInfo
{
    FileName = "dotnet", WorkingDirectory = p.RepositoryRoot, UseShellExecute = false,
    RedirectStandardOutput = true, RedirectStandardError = true
}})
{
    foreach (var arg in new[] { "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write" }) process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start()) throw new InvalidOperationException("Renderer unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(240000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Renderer timed out."); }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (process.ExitCode != 0) throw new InvalidOperationException("Renderer failed: " + output);
}
p.ProjectPlan.TransitionNode("M9", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M9-W01", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M9-W01-C01", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M9-W01-C01-T1", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M9-W01-C01-T1-R", "ready", "in-progress");
return p.Complete();
