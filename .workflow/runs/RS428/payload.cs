using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS427", 1, "chore: close M10 migration work package", "0.1.81", 0, 0, 0, true, expectedCorrectionMode: false, expectedCorrectionSourceAttempt: null);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.194"), JsonValue.Create("0.1.195"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-milestone-closure-rs428.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(string key, string id) => doc[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find("chunks", "M10-W01-C03");
    var wp = Find("work_packages", "M10-W01");
    var release = Find("releases", "M10");
    var target = (string?)doc["plan_version"] == "0.1.195";
    const string evidencePath = ".aura/workflow/reports/RS427/attempt-001.json";
    var evidence = wp["evidence"]!.AsArray();
    if ((string?)release["status"] != "in_progress" || (string?)release["gate"] != "G-M10" ||
        (string?)wp["status"] != (target ? "done" : "in_progress") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0) ||
        new[] { "M10-W01-C01", "M10-W01-C02", "M10-W01-C03" }.Any(id => (string?)Find("chunks", id)["status"] != "done"))
        throw new InvalidOperationException("Unexpected M10 canonical work package closure baseline.");
    wp["status"] = "done";
    wp["evidence"] = p.Json.StringArray(evidencePath);
    chunk["files"] = p.Json.StringArray(chunk["files"]!.AsArray().Select(x => (string)x!).Append("docs/planning/decisions/m10-milestone-closure-rs428.md").Distinct(StringComparer.Ordinal).ToArray());
    chunk["readiness"] = "RS427 durable work package closure; G-M10 native milestone closure pending durable RS428 result.";
    doc["plan_version"] = "0.1.195";
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
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-G", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1-V", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03-T1", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C03", "done");
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9", "done");
p.ProjectPlan.RequireNodeState("M10-W01", "done");
p.ProjectPlan.ConvergeNodeToDone("M10");
return p.Complete();
