using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS400", 1, "test: establish M10 cutover readiness RED", "0.1.81", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.172"), JsonValue.Create("0.1.173"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-cutover-readiness-red-closure-rs401.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var release = Find(doc, "releases", "M10");
    var wp = Find(doc, "work_packages", "M10-W01");
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var red = Find(task, "subtasks", "M10-W01-C01-T1-R");
    var target = (string?)doc["plan_version"] == "0.1.173";
    foreach (var node in new[] { release, wp, chunk, task, red })
    {
        if ((string?)node["status"] != (target ? "in_progress" : "ready") || node["evidence"]!.AsArray().Count != 0)
            throw new InvalidOperationException("Unexpected M10 RED activation baseline.");
        node["status"] = "in_progress";
    }
    chunk["readiness"] = "RS400 durable corrected RED activation; native RED closure and GREEN readiness pending durable result.";
    doc["plan_version"] = "0.1.173";
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
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C01-T1-R");
p.ProjectPlan.TransitionNode("M10-W01-C01-T1-G", "not-ready", "ready");
return p.Complete();
