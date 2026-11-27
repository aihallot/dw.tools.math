using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS413", 1, "feat: migrate Math production to net11 and CSharp 15", "0.1.81", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.180"), JsonValue.Create("0.1.181"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-production-cutover-green-closure-rs414.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C02");
    var task = Find(chunk, "tasks", "M10-W01-C02-T1");
    var red = Find(task, "subtasks", "M10-W01-C02-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C02-T1-G");
    var target = (string?)doc["plan_version"] == "0.1.181";
    if ((string?)red["status"] != "done" ||
        (string?)green["status"] != (target ? "in_progress" : "ready") ||
        green["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected C02 GREEN activation baseline.");
    green["status"] = "in_progress";
    chunk["files"] = p.Json.StringArray(chunk["files"]!.AsArray().Select(x => (string)x!).Concat(new[] { "docs/planning/decisions/m10-production-cutover-green-closure-rs414.md" }).Distinct(StringComparer.Ordinal).ToArray());
    chunk["readiness"] = "RS413 durable actual production net11/C#15 cutover; native GREEN closure and Verification readiness pending durable RS414 result.";
    doc["plan_version"] = "0.1.181";
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
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C02-T1-G");
p.ProjectPlan.TransitionNode("M10-W01-C02-T1-V", "not-ready", "ready");
return p.Complete();
