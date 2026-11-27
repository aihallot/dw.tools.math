using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS425", 2, "test: independently verify M10 post-cutover certification", "0.1.81", 1, 0, 1, false);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.192"), JsonValue.Create("0.1.193"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-post-cutover-certification-closure-rs426.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/m10-post-cutover-certification-verification-rs426.json"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C03");
    var task = Find(chunk, "tasks", "M10-W01-C03-T1");
    var red = Find(task, "subtasks", "M10-W01-C03-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C03-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C03-T1-V");
    var target = (string?)doc["plan_version"] == "0.1.193";
    if ((string?)chunk["status"] != "in_progress" || (string?)task["status"] != "in_progress" ||
        (string?)red["status"] != "done" || red["evidence"]!.AsArray().Count != 1 ||
        (string?)red["evidence"]![0] != ".aura/workflow/reports/RS418/attempt-001.json" ||
        (string?)green["status"] != "done" || green["evidence"]!.AsArray().Count != 1 ||
        (string?)green["evidence"]![0] != ".aura/workflow/reports/RS424/attempt-001.json" ||
        (string?)verification["status"] != (target ? "in_progress" : "ready") || verification["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected C03 Verification activation baseline.");
    verification["status"] = "in_progress";
    var files = chunk["files"]!.AsArray().Select(x => (string)x!).ToArray();
    foreach (var path in new[] { "docs/planning/decisions/m10-post-cutover-certification-closure-rs426.md", "docs/engineering/m10-post-cutover-certification-verification-rs426.json" })
        if (!files.Contains(path, StringComparer.Ordinal)) files = files.Append(path).ToArray();
    chunk["files"] = p.Json.StringArray(files);
    chunk["readiness"] = "RS425 durable independent Verification after one allocated correction; native C03 closure pending durable RS426 result.";
    doc["plan_version"] = "0.1.193";
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
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-G", "done");
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C03-T1-V");
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C03-T1");
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C03");
return p.Complete();
