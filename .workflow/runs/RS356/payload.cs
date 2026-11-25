using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS355", 1, "chore: close M9 sampling source and ready package RED", "0.1.76", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-G", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-V", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.131"), JsonValue.Create("0.1.132"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.27"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-package-red-rs356.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.131" or "0.1.132")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.132";
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var task = Find(chunk, "tasks", "M9-W01-C01-T1");
    var verification = Find(task, "subtasks", "M9-W01-C01-T1-V");
    var package = Find(chunk, "tasks", "M9-W01-C01-T2");
    var packageRed = Find(package, "subtasks", "M9-W01-C01-T2-R");
    const string evidencePath = ".aura/workflow/reports/RS355/attempt-001.json";
    foreach (var node in new[] { task, verification })
    {
        var evidence = node["evidence"]!.AsArray();
        if ((string?)node["status"] != (target ? "done" : "in_progress") ||
            (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
            throw new InvalidOperationException("Unexpected canonical source closure baseline.");
        node["status"] = "done";
        node["evidence"] = p.Json.StringArray(evidencePath);
    }
    foreach (var node in new[] { package, packageRed })
    {
        if ((string?)node["status"] != (target ? "ready" : "planned"))
            throw new InvalidOperationException("Unexpected canonical package readiness baseline.");
        node["status"] = "ready";
    }
    chunk["readiness"] = "RS355 durable source closure and package RED readiness; falsify the immutable package consumer boundary.";
    doc["plan_version"] = "0.1.132";
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
p.ProjectPlan.ActivateReadyContinuation("M9-W01-C01-T2-R");
return p.Complete();
