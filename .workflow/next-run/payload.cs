using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS410", 1, "chore: close M10 readiness and ready production cutover RED", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.177"), JsonValue.Create("0.1.178"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-production-cutover-red-rs411.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/VerifyM10ProductionCutover.cs"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var green = Find(task, "subtasks", "M10-W01-C01-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C01-T1-V");
    var next = Find(doc, "chunks", "M10-W01-C02");
    var nextTask = Find(next, "tasks", "M10-W01-C02-T1");
    var red = Find(nextTask, "subtasks", "M10-W01-C02-T1-R");
    var target = (string?)doc["plan_version"] == "0.1.178";
    const string evidencePath = ".aura/workflow/reports/RS410/attempt-001.json";
    foreach (var node in new[] { chunk, task, verification })
    {
        var evidence = node["evidence"]!.AsArray();
        if ((string?)node["status"] != (target ? "done" : "in_progress") ||
            (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
            throw new InvalidOperationException("Unexpected C01 closure baseline.");
        node["status"] = "done";
        node["evidence"] = p.Json.StringArray(evidencePath);
    }
    foreach (var node in new[] { next, nextTask, red })
    {
        if ((string?)node["status"] != (target ? "ready" : "planned") || node["evidence"]!.AsArray().Count != 0)
            throw new InvalidOperationException("Unexpected C02 readiness baseline.");
        node["status"] = "ready";
    }
    chunk["readiness"] = "RS410 durable C01 closure.";
    next["files"] = p.Json.StringArray("global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx", "NuGet.config", "docs/engineering/m10-cutover-readiness-verification-rs409.json", "docs/engineering/VerifyM10ProductionCutover.cs", "docs/planning/decisions/m10-production-cutover-red-rs411.md");
    next["commands"] = p.Json.StringArray("dotnet run --file docs/engineering/VerifyM10ProductionCutover.cs -- --red", "dotnet run --file docs/engineering/VerifyM10ProductionCutover.cs -- --target");
    next["refinement"] = "Reuse the RS409 qualified cutover scope and RS407 rollback baseline; establish RED against the exact production graph, then apply only the 28 qualified SDK, framework, lock and tool input changes with locked restore and targeted tests before independent verification.";
    next["readiness"] = "RS410 durable C01 closure; actual production cutover RED activation pending durable result.";
    doc["plan_version"] = "0.1.178";
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
p.ProjectPlan.TransitionNode("M10-W01-C02", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C02-T1", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C02-T1-R", "ready", "in-progress");
return p.Complete();
