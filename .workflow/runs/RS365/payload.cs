using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS364", 1, "chore: close M9 bounded sampling milestone", "0.1.78", 0, 0, 0, true);
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
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.140"), JsonValue.Create("0.1.141"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.31"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-sampling-canonical-reconciliation-rs365.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.140" or "0.1.141")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.141";
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M9-W01-C01");
    var wp = Find(doc, "work_packages", "M9-W01");
    var release = Find(doc, "releases", "M9");
    const string evidencePath = ".aura/workflow/reports/RS364/attempt-001.json";
    var evidence = release["evidence"]!.AsArray();
    if ((string?)release["gate"] != "G-M9" || (string?)wp["status"] != "done" ||
        (string?)chunk["status"] != "done" || (string?)Find(doc, "releases", "M3")["status"] != "done" ||
        (string?)release["status"] != (target ? "done" : "in_progress") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
        throw new InvalidOperationException("Unexpected canonical milestone closure baseline.");
    release["status"] = "done";
    release["evidence"] = p.Json.StringArray(evidencePath);
    chunk["readiness"] = "RS364 durable G-M9 closure; bounded Math qualification complete, Assessa adoption remains external.";
    doc["plan_version"] = "0.1.141";
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
