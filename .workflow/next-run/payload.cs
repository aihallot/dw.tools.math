using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS360", 1, "test: independently verify M9 immutable package and Linux replay", "0.1.78", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-G", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.136"), JsonValue.Create("0.1.137"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.31"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-package-task-closure-rs361.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.136" or "0.1.137")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.137";
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var source = Find(chunk, "tasks", "M9-W01-C01-T1");
    var package = Find(chunk, "tasks", "M9-W01-C01-T2");
    var red = Find(package, "subtasks", "M9-W01-C01-T2-R");
    var green = Find(package, "subtasks", "M9-W01-C01-T2-G");
    var verification = Find(package, "subtasks", "M9-W01-C01-T2-V");
    if ((string?)source["status"] != "done" || (string?)package["status"] != "in_progress" ||
        (string?)red["status"] != "done" || (string?)green["status"] != "done" ||
        (string?)verification["status"] != (target ? "in_progress" : "ready") || verification["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected canonical package Verification activation baseline.");
    verification["status"] = "in_progress";
    chunk["readiness"] = "RS360 durable independent package Verification activation and Linux replay; T2 closure pending durable result.";
    doc["plan_version"] = "0.1.137";
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
p.ProjectPlan.ConvergeNodeToDone("M9-W01-C01-T2-V");
p.ProjectPlan.ConvergeNodeToDone("M9-W01-C01-T2");
return p.Complete();
