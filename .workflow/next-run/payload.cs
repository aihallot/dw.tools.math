using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS418", 1, "chore: close M10 certification RED and ready GREEN", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.185"), JsonValue.Create("0.1.186"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-post-cutover-regression-rs419.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM10PostCutoverRegression.cs"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C03");
    var task = Find(chunk, "tasks", "M10-W01-C03-T1");
    var red = Find(task, "subtasks", "M10-W01-C03-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C03-T1-G");
    var target = (string?)doc["plan_version"] == "0.1.186";
    const string evidencePath = ".aura/workflow/reports/RS418/attempt-001.json";
    if ((string?)chunk["status"] != "in_progress" || (string?)task["status"] != "in_progress" ||
        (string?)red["status"] != (target ? "done" : "in_progress") ||
        (target ? red["evidence"]!.AsArray().Count != 1 || (string?)red["evidence"]![0] != evidencePath : red["evidence"]!.AsArray().Count != 0) ||
        (string?)green["status"] != (target ? "ready" : "planned") || green["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected C03 RED closure baseline.");
    red["status"] = "done";
    red["evidence"] = p.Json.StringArray(evidencePath);
    green["status"] = "ready";
    var files = chunk["files"]!.AsArray().Select(x => (string)x!).ToArray();
    foreach (var path in new[] { "docs/planning/decisions/m10-post-cutover-regression-rs419.md", "docs/engineering/ProbeM10PostCutoverRegression.cs" })
        if (!files.Contains(path, StringComparer.Ordinal)) files = files.Append(path).ToArray();
    chunk["files"] = p.Json.StringArray(files);
    const string command = "dotnet run --file docs/engineering/ProbeM10PostCutoverRegression.cs -- --check";
    var commands = chunk["commands"]!.AsArray().Select(x => (string)x!).ToArray();
    if (!commands.Contains(command, StringComparer.Ordinal)) chunk["commands"] = p.Json.StringArray(commands.Append(command).ToArray());
    chunk["readiness"] = "RS418 durable RED closure; native GREEN activation and bounded full regression pending durable RS419 result.";
    doc["plan_version"] = "0.1.186";
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
p.ProjectPlan.TransitionNode("M10-W01-C03-T1-G", "ready", "in-progress");
return p.Complete();
