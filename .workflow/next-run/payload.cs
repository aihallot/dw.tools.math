using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS416", 1, "chore: close M10 production cutover and ready certification RED", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.183"), JsonValue.Create("0.1.184"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-post-cutover-certification-red-rs417.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM10PostCutoverCertificationRed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/m10-post-cutover-certification-red-rs417.json"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C02");
    var task = Find(chunk, "tasks", "M10-W01-C02-T1");
    var red = Find(task, "subtasks", "M10-W01-C02-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C02-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C02-T1-V");
    var next = Find(doc, "chunks", "M10-W01-C03");
    var nextTask = Find(next, "tasks", "M10-W01-C03-T1");
    var nextRed = Find(nextTask, "subtasks", "M10-W01-C03-T1-R");
    var target = (string?)doc["plan_version"] == "0.1.184";
    const string evidencePath = ".aura/workflow/reports/RS416/attempt-001.json";
    foreach (var node in new[] { chunk, task, verification })
    {
        var evidence = node["evidence"]!.AsArray();
        if ((string?)node["status"] != (target ? "done" : "in_progress") ||
            (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
            throw new InvalidOperationException("Unexpected C02 closure baseline.");
        node["status"] = "done";
        node["evidence"] = p.Json.StringArray(evidencePath);
    }
    foreach (var node in new[] { next, nextTask, nextRed })
    {
        if ((string?)node["status"] != (target ? "ready" : "planned") || node["evidence"]!.AsArray().Count != 0)
            throw new InvalidOperationException("Unexpected C03 readiness baseline.");
        node["status"] = "ready";
    }
    chunk["readiness"] = "RS416 durable C02 closure.";
    next["files"] = p.Json.StringArray("global.json", "Directory.Build.props", "dw.tools.math.slnx", "docs/engineering/m10-post-cutover-certification-red-rs417.json", "docs/engineering/ProbeM10PostCutoverCertificationRed.cs", "docs/planning/decisions/m10-post-cutover-certification-red-rs417.md");
    next["commands"] = p.Json.StringArray("dotnet run --file docs/engineering/ProbeM10PostCutoverCertificationRed.cs -- --red");
    next["refinement"] = "Qualify full regression after targeted tests, fresh unpublished net11 candidates, exact source-free locked consumers, Windows and Linux/WSL runtime, archive preservation and rollback in bounded evidence slices; retain the RED contract.";
    next["readiness"] = "RS416 durable C02 closure; C03 RED activation pending durable RS417 result.";
    doc["plan_version"] = "0.1.184";
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
p.ProjectPlan.TransitionNode("M10-W01-C03", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C03-T1", "ready", "in-progress");
p.ProjectPlan.TransitionNode("M10-W01-C03-T1-R", "ready", "in-progress");
return p.Complete();
