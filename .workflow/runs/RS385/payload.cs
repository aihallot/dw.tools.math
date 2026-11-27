using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS384", 1, "test: establish larger-sample quantile RED", "0.1.80", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M11-W01-C02", "done");
p.ProjectPlan.RequireNodeState("M11-W01-C03-T1", "in-progress");
p.ProjectPlan.RequireNodeState("M11-W01-C03-T1-V", "not-ready");
p.ProjectPlan.RequireNodeState("M10-W01-C02", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.157"), JsonValue.Create("0.1.158"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.32"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/backlog.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m11-larger-sample-quantiles-red-closure-rs385.md"));
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
p.ProjectPlan.ConvergeNodeToDone("M11-W01-C03-T1-R");
p.ProjectPlan.TransitionNode("M11-W01-C03-T1-G", "not-ready", "ready");
return p.Complete();
