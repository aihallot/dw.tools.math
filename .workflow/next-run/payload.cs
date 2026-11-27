using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS408", 1, "chore: close M10 cutover readiness GREEN and ready verification", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.175"), JsonValue.Create("0.1.176"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/m10-cutover-readiness-verification-rs409.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/VerifyM10CutoverReadiness.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-cutover-readiness-verification-rs409.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var green = Find(task, "subtasks", "M10-W01-C01-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C01-T1-V");
    var target = (string?)doc["plan_version"] == "0.1.176";
    const string evidencePath = ".aura/workflow/reports/RS408/attempt-001.json";
    var evidence = green["evidence"]!.AsArray();
    if ((string?)green["status"] != (target ? "done" : "in_progress") ||
        (string?)verification["status"] != (target ? "ready" : "planned") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
        throw new InvalidOperationException("Unexpected M10 GREEN closure baseline.");
    green["status"] = "done";
    green["evidence"] = p.Json.StringArray(evidencePath);
    verification["status"] = "ready";
    chunk["files"] = p.Json.StringArray("global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx", "docs/engineering/m10-post-m11-project-inventory-rs399.json", "docs/engineering/m10-cutover-readiness-red-rs400.json", "docs/engineering/ProbeM10CutoverReadinessRed.cs", "docs/engineering/InspectM8Net11SourceBaseline.cs", "docs/engineering/InspectM8Net11PackageGraph.cs", "docs/engineering/InspectM10ProjectDispositions.cs", "docs/engineering/m10-project-dispositions-rs402.json", "docs/engineering/ProbeM10IsolatedSolutionGraph.cs", "docs/planning/decisions/m10-cutover-readiness-green-closure-rs408.md", "docs/planning/decisions/m10-cutover-readiness-verification-rs409.md", "docs/engineering/VerifyM10CutoverReadiness.cs", "docs/engineering/m10-cutover-readiness-verification-rs409.json");
    chunk["commands"] = p.Json.StringArray("dotnet run --file docs/engineering/ProbeM10CutoverReadinessRed.cs -- --red", "dotnet run --file docs/engineering/InspectM8Net11SourceBaseline.cs -- --check-sdk11", "dotnet run --file docs/engineering/InspectM8Net11PackageGraph.cs -- --check", "dotnet run --file docs/engineering/InspectM10ProjectDispositions.cs -- --check", "dotnet run --file docs/engineering/ProbeM10IsolatedSolutionGraph.cs -- --check", "dotnet run --file docs/engineering/ProbeM10IsolatedSolutionGraph.cs -- --focused", "dotnet run --file docs/engineering/ProbeM10IsolatedSolutionGraph.cs -- --tools", "dotnet run --file docs/engineering/ProbeM10IsolatedSolutionGraph.cs -- --inspect", "dotnet run --file docs/engineering/ProbeM10IsolatedSolutionGraph.cs -- --rollback", "dotnet run --file docs/engineering/VerifyM10CutoverReadiness.cs -- --check");
    chunk["readiness"] = "RS408 durable GREEN closure; independent cutover contract verification and native Verification activation pending durable result.";
    doc["plan_version"] = "0.1.176";
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
p.ProjectPlan.ActivateReadyContinuation("M10-W01-C01-T1-V");
return p.Complete();
