using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS399", 1, "docs: refresh post-M11 production migration baseline", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M11", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.171"), JsonValue.Create("0.1.172"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/m10-cutover-readiness-red-rs400.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM10CutoverReadinessRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-cutover-readiness-red-rs400.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var release = Find(doc, "releases", "M10");
    var wp = Find(doc, "work_packages", "M10-W01");
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var red = Find(task, "subtasks", "M10-W01-C01-T1-R");
    var target = (string?)doc["plan_version"] == "0.1.172";
    foreach (var node in new[] { release, wp, chunk, task, red })
    {
        if ((string?)node["status"] != (target ? "ready" : "planned") || node["evidence"]!.AsArray().Count != 0)
            throw new InvalidOperationException("Unexpected M10 readiness baseline.");
        node["status"] = "ready";
    }
    chunk["readiness"] = "RS399 durable post-M11 inventory; native RED activation and bounded falsification pending durable result.";
    doc["plan_version"] = "0.1.172";
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
p.ProjectPlan.TransitionNode("M10", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C01", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C01-T1", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C01-T1-R", "ready", "in-progress");
return p.Complete();
