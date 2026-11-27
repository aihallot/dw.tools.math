using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS426", 1, "chore: close verified M10 post-cutover certification phase", "0.1.81", 1, 1, 0, false, expectedCorrectionMode: false, expectedCorrectionSourceAttempt: null);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.193"), JsonValue.Create("0.1.194"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-work-package-closure-rs427.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C03");
    var task = Find(chunk, "tasks", "M10-W01-C03-T1");
    var red = Find(task, "subtasks", "M10-W01-C03-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C03-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C03-T1-V");
    var target = (string?)doc["plan_version"] == "0.1.194";
    const string evidencePath = ".aura/workflow/reports/RS426/attempt-001.json";
    if ((string?)red["status"] != "done" || red["evidence"]!.AsArray().Count != 1 ||
        (string?)red["evidence"]![0] != ".aura/workflow/reports/RS418/attempt-001.json" ||
        (string?)green["status"] != "done" || green["evidence"]!.AsArray().Count != 1 ||
        (string?)green["evidence"]![0] != ".aura/workflow/reports/RS424/attempt-001.json")
        throw new InvalidOperationException("Unexpected C03 RED/GREEN closure baseline.");
    foreach (var node in new[] { chunk, task, verification })
    {
        var evidence = node["evidence"]!.AsArray();
        if ((string?)node["status"] != (target ? "done" : "in_progress") ||
            (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
            throw new InvalidOperationException("Unexpected canonical C03 closure baseline.");
        node["status"] = "done";
        node["evidence"] = p.Json.StringArray(evidencePath);
    }
    chunk["files"] = p.Json.StringArray(chunk["files"]!.AsArray().Select(x => (string)x!).Append("docs/planning/decisions/m10-work-package-closure-rs427.md").Distinct(StringComparer.Ordinal).ToArray());
    chunk["readiness"] = "RS426 durable C03 closure after one preselection correction; native work package closure pending durable RS427 result.";
    doc["plan_version"] = "0.1.194";
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
p.ProjectPlan.RequireNodeState("M10-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-V", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03", "done");
p.ProjectPlan.ConvergeNodeToDone("M10-W01");
return p.Complete();
