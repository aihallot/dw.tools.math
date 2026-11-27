using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS396", 1, "chore: close M11 numeric subsidiarity work package", "0.1.81", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M3", "done");
p.ProjectPlan.RequireNodeState("M9", "done");
p.ProjectPlan.RequireNodeState("M11-W01", "done");
p.ProjectPlan.RequireNodeState("M11-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M11-W01-C02", "done");
p.ProjectPlan.RequireNodeState("M11-W01-C03", "done");
p.ProjectPlan.RequireNodeState("M11-W01-C04", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.169"), JsonValue.Create("0.1.170"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m11-numeric-milestone-closure-rs397.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(string key, string id) => doc[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var wp = Find("work_packages", "M11-W01");
    var release = Find("releases", "M11");
    var chunk = Find("chunks", "M11-W01-C04");
    const string evidencePath = ".aura/workflow/reports/RS396/attempt-001.json";
    var target = (string?)doc["plan_version"] == "0.1.170";
    var evidence = wp["evidence"]!.AsArray();
    if ((string?)release["status"] != "in_progress" || (string?)release["gate"] != "G-M11" ||
        (string?)wp["status"] != (target ? "done" : "in_progress") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0) ||
        (string?)chunk["status"] != "done")
        throw new InvalidOperationException("Unexpected M11 canonical work package closure baseline.");
    wp["status"] = "done";
    wp["evidence"] = p.Json.StringArray(evidencePath);
    chunk["readiness"] = "RS396 durable work package closure; G-M11 milestone closure pending durable result.";
    doc["plan_version"] = "0.1.170";
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
p.ProjectPlan.ConvergeNodeToDone("M11");
return p.Complete();
