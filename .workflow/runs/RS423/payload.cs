using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS422", 1, "test: qualify exact source-free Linux WSL consumers", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.189"), JsonValue.Create("0.1.190"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-post-cutover-rollback-rs423.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM10PostCutoverRollback.cs"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C03");
    var task = Find(chunk, "tasks", "M10-W01-C03-T1");
    var red = Find(task, "subtasks", "M10-W01-C03-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C03-T1-G");
    if ((string?)chunk["status"] != "in_progress" || (string?)task["status"] != "in_progress" ||
        (string?)red["status"] != "done" || red["evidence"]!.AsArray().Count != 1 ||
        (string?)red["evidence"]![0] != ".aura/workflow/reports/RS418/attempt-001.json" ||
        (string?)green["status"] != "in_progress" || green["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected C03 GREEN activation baseline.");
    green["status"] = "in_progress";
    var files = chunk["files"]!.AsArray().Select(x => (string)x!).ToArray();
    foreach (var path in new[] { "docs/planning/decisions/m10-post-cutover-rollback-rs423.md", "docs/engineering/ProbeM10PostCutoverRollback.cs" })
        if (!files.Contains(path, StringComparer.Ordinal)) files = files.Append(path).ToArray();
    chunk["files"] = p.Json.StringArray(files);
    const string command = "dotnet run --file docs/engineering/ProbeM10PostCutoverRollback.cs -- --check";
    var commands = chunk["commands"]!.AsArray().Select(x => (string)x!).ToArray();
    if (!commands.Contains(command, StringComparer.Ordinal)) chunk["commands"] = p.Json.StringArray(commands.Append(command).ToArray());
    chunk["readiness"] = "RS422 durable Linux consumer qualification; GREEN active, post-cutover rollback pending RS423.";
    doc["plan_version"] = "0.1.190";
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
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-G", "in-progress");
return p.Complete();
