using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS409", 1, "test: verify M10 cutover readiness contract", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.176"), JsonValue.Create("0.1.177"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-cutover-readiness-closure-rs410.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C01");
    var task = Find(chunk, "tasks", "M10-W01-C01-T1");
    var green = Find(task, "subtasks", "M10-W01-C01-T1-G");
    var verification = Find(task, "subtasks", "M10-W01-C01-T1-V");
    var target = (string?)doc["plan_version"] == "0.1.177";
    if ((string?)green["status"] != "done" ||
        (string?)verification["status"] != (target ? "in_progress" : "ready") ||
        verification["evidence"]!.AsArray().Count != 0)
        throw new InvalidOperationException("Unexpected M10 Verification activation baseline.");
    verification["status"] = "in_progress";
    chunk["files"] = p.Json.StringArray("global.json", "Directory.Build.props", "Directory.Packages.props", "dw.tools.math.slnx", "docs/engineering/m10-post-m11-project-inventory-rs399.json", "docs/engineering/m10-cutover-readiness-red-rs400.json", "docs/engineering/ProbeM10CutoverReadinessRed.cs", "docs/engineering/InspectM8Net11SourceBaseline.cs", "docs/engineering/InspectM8Net11PackageGraph.cs", "docs/engineering/InspectM10ProjectDispositions.cs", "docs/engineering/m10-project-dispositions-rs402.json", "docs/engineering/ProbeM10IsolatedSolutionGraph.cs", "docs/planning/decisions/m10-cutover-readiness-green-closure-rs408.md", "docs/planning/decisions/m10-cutover-readiness-verification-rs409.md", "docs/engineering/VerifyM10CutoverReadiness.cs", "docs/engineering/m10-cutover-readiness-verification-rs409.json", "docs/planning/decisions/m10-cutover-readiness-closure-rs410.md");
    chunk["readiness"] = "RS409 durable independent Verification; native C01 closure and C02 RED readiness pending durable RS410 result.";
    doc["plan_version"] = "0.1.177";
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
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C01-T1-V");
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C01-T1");
p.ProjectPlan.ConvergeNodeToDone("M10-W01-C01");
p.ProjectPlan.TransitionNode("M10-W01-C02", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M10-W01-C02-T1", "not-ready", "ready");
p.ProjectPlan.TransitionNode("M10-W01-C02-T1-R", "not-ready", "ready");
return p.Complete();
