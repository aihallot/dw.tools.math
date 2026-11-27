using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS401", 1, "chore: close M10 cutover readiness RED and ready GREEN", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.173"), JsonValue.Create("0.1.174"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/InspectM10ProjectDispositions.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-green-project-dispositions-rs402.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var red = Find(task, "subtasks", "M10-W01-C01-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C01-T1-G");
    var target = (string?)doc["plan_version"] == "0.1.174";
    const string evidencePath = ".aura/workflow/reports/RS401/attempt-001.json";
    var evidence = red["evidence"]!.AsArray();
    if ((string?)red["status"] != (target ? "done" : "in_progress") ||
        (string?)green["status"] != (target ? "ready" : "planned") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
        throw new InvalidOperationException("Unexpected M10 RED closure baseline.");
    red["status"] = "done";
    red["evidence"] = p.Json.StringArray(evidencePath);
    green["status"] = "ready";
    chunk["files"] = p.Json.StringArray("global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx", "docs/engineering/m10-post-m11-project-inventory-rs399.json", "docs/engineering/m10-cutover-readiness-red-rs400.json", "docs/engineering/ProbeM10CutoverReadinessRed.cs", "docs/engineering/InspectM8Net11SourceBaseline.cs", "docs/engineering/InspectM8Net11PackageGraph.cs", "docs/engineering/InspectM10ProjectDispositions.cs", "docs/engineering/m10-project-dispositions-rs402.json");
    chunk["commands"] = p.Json.StringArray("dotnet run --file docs/engineering/ProbeM10CutoverReadinessRed.cs -- --red", "dotnet run --file docs/engineering/InspectM8Net11SourceBaseline.cs -- --check-sdk11", "dotnet run --file docs/engineering/InspectM8Net11PackageGraph.cs -- --check", "dotnet run --file docs/engineering/InspectM10ProjectDispositions.cs -- --check");
    chunk["readiness"] = "RS401 durable RED closure; GREEN activation and project dispositions pending durable result.";
    doc["plan_version"] = "0.1.174";
});
using (var process = new Process { StartInfo = new ProcessStartInfo
{
    FileName = "dotnet", WorkingDirectory = p.RepositoryRoot, UseShellExecute = false,
    RedirectStandardOutput = true, RedirectStandardError = true
}})
{
    foreach (var arg in new[] { "run", "--file", "docs/engineering/InspectM10ProjectDispositions.cs", "--", "--write" }) process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start()) throw new InvalidOperationException("Disposition generator unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(240000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Disposition generator timed out."); }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (process.ExitCode != 0) throw new InvalidOperationException("Disposition generator failed: " + output);
}
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
p.ProjectPlan.TransitionNode("M10-W01-C01-T1-G", "ready", "in-progress");
return p.Complete();
