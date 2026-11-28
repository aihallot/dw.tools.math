using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS435", 1, "chore: activate M12 source and API inventory question", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.ProjectPlan.RequireNodeState("M12-W01-C01-T1-C", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.198"), JsonValue.Create("0.1.199"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.32"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/m12-source-api-inventory-protocol-rs436.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateMath20Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m12-source-api-evidence-inventory-protocol-rs436.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var release = Find(doc, "releases", "M12");
    var work = Find(doc, "work_packages", "M12-W01");
    var chunk = Find(doc, "chunks", "M12-W01-C01");
    var task = Find(chunk, "tasks", "M12-W01-C01-T1");
    var question = Find(task, "subtasks", "M12-W01-C01-T1-A");
    var target = (string?)doc["plan_version"] == "0.1.199";
    foreach (var node in new[] { release, work, chunk, task, question })
    {
        if ((string?)node["status"] != (target ? "in_progress" : "ready") || node["evidence"]!.AsArray().Count != 0)
            throw new InvalidOperationException("Unexpected Question activation reconciliation baseline.");
        node["status"] = "in_progress";
    }
    var priorFiles = chunk["files"]!.AsArray().Select(x => (string)x!).ToArray();
    foreach (var path in new[] { "docs/planning/decisions/m12-source-api-evidence-inventory-protocol-rs436.md", "docs/engineering/m12-source-api-inventory-protocol-rs436.json" })
        if (!priorFiles.Contains(path, StringComparer.Ordinal))
            chunk["files"]!.AsArray().Add(JsonValue.Create(path));
    chunk["refinement"] = "RS435 durable activation reconciled. RS436 specifies pinned source, project, metadata and evidence observation with an exact roster, independent negative controls and resource limits. Observation must qualify or reuse the missing metadata reader and gather real evidence before modernization.";
    chunk["readiness"] = "RS435 delivered five native Question activation transitions; RS436 reconciles those statuses as in_progress and targets native Question closure and Observation readiness. Canonical closure and Observation readiness await durable RS436 evidence.";
    doc["plan_version"] = "0.1.199";
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
p.ProjectPlan.ConvergeNodeToDone("M12-W01-C01-T1-A");
p.ProjectPlan.TransitionNode("M12-W01-C01-T1-B", "not-ready", "ready");
return p.Complete();
