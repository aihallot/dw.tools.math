using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS350", 1, "test: activate M9 bounded sampling RED and falsify source mutants", "0.1.76", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.126"), JsonValue.Create("0.1.127"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-red-closure-rs351.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.126" or "0.1.127")) throw new InvalidOperationException("Backlog version differs.");
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var release = Find(doc, "releases", "M9");
    var wp = Find(doc, "work_packages", "M9-W01");
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var task = Find(chunk, "tasks", "M9-W01-C01-T1");
    var red = Find(task, "subtasks", "M9-W01-C01-T1-R");
    foreach (var node in new[] { release, wp, chunk, task, red })
    {
        if ((string?)node["status"] != (version == "0.1.127" ? "in_progress" : "ready"))
            throw new InvalidOperationException("Unexpected canonical activation baseline.");
        node["status"] = "in_progress";
    }
    chunk["readiness"] = "Native source RED activation and independent oracle qualification delivered in RS350; RED closure and GREEN readiness pending durable result.";
    doc["plan_version"] = "0.1.127";
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
p.ProjectPlan.ConvergeNodeToDone("M9-W01-C01-T1-R");
p.ProjectPlan.TransitionNode("M9-W01-C01-T1-G", "not-ready", "ready");
return p.Complete();
