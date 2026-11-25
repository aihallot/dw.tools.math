using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS354", 1, "test: independently verify M9 bounded sampling source and replay", "0.1.76", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-G", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-V", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.130"), JsonValue.Create("0.1.131"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-source-closure-rs355.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.130" or "0.1.131")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.131";
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var task = Find(chunk, "tasks", "M9-W01-C01-T1");
    var red = Find(task, "subtasks", "M9-W01-C01-T1-R");
    var green = Find(task, "subtasks", "M9-W01-C01-T1-G");
    var verification = Find(task, "subtasks", "M9-W01-C01-T1-V");
    if ((string?)task["status"] != "in_progress" || (string?)red["status"] != "done" ||
        (string?)green["status"] != "done" || (string?)verification["status"] != (target ? "in_progress" : "ready") ||
        verification["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected canonical Verification activation baseline.");
    verification["status"] = "in_progress";
    chunk["readiness"] = "RS354 durable Verification activation and independent source replay; T1 closure and T2 RED readiness pending durable result.";
    doc["plan_version"] = "0.1.131";
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
p.ProjectPlan.ConvergeNodeToDone("M9-W01-C01-T1-V");
p.ProjectPlan.ConvergeNodeToDone("M9-W01-C01-T1");
p.ProjectPlan.TransitionNode("M9-W01-C01-T2", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M9-W01-C01-T2-R", "not-ready", "ready");
return p.Complete();
