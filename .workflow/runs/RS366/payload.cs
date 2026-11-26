using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS365", 1, "chore: reconcile completed M9 milestone", "0.1.78", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M9-W01", "done");
p.ProjectPlan.RequireNodeState("M9", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.141"), JsonValue.Create("0.1.142"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.31"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/distribution/current-local-consumption.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/implementation-handoff.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/completed-roadmap-status-reconciliation-rs366.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateCompletedRoadmapStatus.cs"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.141" or "0.1.142")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.142";
    if ((string?)doc["status"] != (target ? "done" : "in_progress") ||
        doc["releases"]!.AsArray().Count != 10 ||
        !doc["releases"]!.AsArray().All(x => (string?)x?["status"] == "done") ||
        !doc["work_packages"]!.AsArray().All(x => (string?)x?["status"] == "done") ||
        !doc["chunks"]!.AsArray().All(x => (string?)x?["status"] == "done" &&
            x!["tasks"]!.AsArray().All(t => (string?)t?["status"] == "done" &&
                t!["subtasks"]!.AsArray().All(s => (string?)s?["status"] == "done"))))
        throw new InvalidOperationException("Unexpected aggregate completed roadmap baseline.");
    doc["status"] = "done";
    doc["plan_version"] = "0.1.142";
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
return p.Complete();
