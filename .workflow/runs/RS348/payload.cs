using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS347", 1, "chore: close bounded M8 internal qualification milestone", "0.1.76", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.124"), JsonValue.Create("0.1.125"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.27"));
var ids = new[] { "M8", "M8-W01", "M8-W02", "M8-W01-C01", "M8-W01-C01-T1", "M8-W01-C01-T1-R", "M8-W01-C01-T1-G", "M8-W01-C01-T1-V", "M8-W01-C01-T2", "M8-W01-C01-T2-R", "M8-W01-C01-T2-G", "M8-W01-C01-T2-V", "M8-W01-C02", "M8-W01-C02-T1", "M8-W01-C02-T1-R", "M8-W01-C02-T1-G", "M8-W01-C02-T1-V", "M8-W01-C02-T2", "M8-W01-C02-T2-R", "M8-W01-C02-T2-G", "M8-W01-C02-T2-V", "M8-W02-C01", "M8-W02-C01-T1", "M8-W02-C01-T1-R", "M8-W02-C01-T1-G", "M8-W02-C01-T1-V", "M8-W02-C01-T2", "M8-W02-C01-T2-R", "M8-W02-C01-T2-G", "M8-W02-C01-T2-V", "M8-W02-C02", "M8-W02-C02-T1", "M8-W02-C02-T1-R", "M8-W02-C02-T1-G", "M8-W02-C02-T1-V", "M8-W02-C02-T2", "M8-W02-C02-T2-R", "M8-W02-C02-T2-G", "M8-W02-C02-T2-V" };
var expected = ids.ToHashSet(StringComparer.Ordinal);
foreach (var id in ids) p.ProjectPlan.RequireNodeState(id, "done");
var root = p.RepositoryRoot;
var evidence = new Dictionary<string, string>(StringComparer.Ordinal);
for (int i = 280; i <= 347; i++)
{
    var dir = Path.Combine(root, ".aura", "workflow", "reports", "RS" + i.ToString("D3"));
    if (!Directory.Exists(dir)) continue;
    foreach (var file in Directory.EnumerateFiles(dir, "attempt-*.json").OrderBy(x => x, StringComparer.Ordinal))
    {
        var report = JsonNode.Parse(File.ReadAllText(file))!.AsObject();
        if ((string?)report["outcome"] != "succeeded") continue;
        if ((string?)report["runId"] != "RS" + i.ToString("D3") || report["boundaryViolations"]?.AsArray().Count != 0)
            throw new InvalidOperationException("Invalid completion report identity or boundary: " + file);
        var completions = report["progress"]?["completions"] as JsonArray;
        if (completions is null) continue;
        foreach (var item in completions)
        {
            var id = (string?)item?["id"];
            if (id is null || !expected.Contains(id)) continue;
            if ((string?)item?["afterState"] != "done" || item?["completed"]?.GetValue<bool>() != true ||
                string.IsNullOrWhiteSpace((string?)item?["achievement"]))
                throw new InvalidOperationException("Incomplete achievement: " + id);
            var path = Path.GetRelativePath(root, file).Replace('\\', '/');
            if (!evidence.TryAdd(id, path))
                throw new InvalidOperationException("Ambiguous M8 completion: " + id);
        }
    }
}
if (!expected.SetEquals(evidence.Keys))
    throw new InvalidOperationException("Missing durable M8 completion: " + string.Join(", ", expected.Except(evidence.Keys)));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m8-canonical-backlog-reconciliation-rs348.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.124" or "0.1.125")) throw new InvalidOperationException("Unknown backlog version.");
    var target = version == "0.1.125";
    var seen = new HashSet<string>(StringComparer.Ordinal);
    void Complete(JsonNode node)
    {
        var id = (string?)node["id"] ?? throw new InvalidOperationException("Missing node id.");
        if (!expected.Contains(id) || !seen.Add(id)) throw new InvalidOperationException("Unexpected M8 node: " + id);
        if ((string?)node["status"] != (target ? "done" : "planned"))
            throw new InvalidOperationException("Unexpected canonical node state: " + id);
        var oldEvidence = node["evidence"]!.AsArray();
        if (target ? oldEvidence.Count != 1 || (string?)oldEvidence[0] != evidence[id] : oldEvidence.Count != 0)
            throw new InvalidOperationException("Unexpected evidence: " + id);
        node["status"] = "done";
        node["evidence"] = p.Json.StringArray(evidence[id]);
    }
    Complete(doc["releases"]!.AsArray().Single(x => (string?)x?["id"] == "M8")!);
    foreach (var w in doc["work_packages"]!.AsArray().Where(x => (string?)x?["release"] == "M8")) Complete(w!);
    foreach (var c in doc["chunks"]!.AsArray().Where(x => (string?)x?["release"] == "M8"))
    {
        Complete(c!);
        var id = (string)c!["id"]!;
        var file = id switch
        {
            "M8-W01-C01" => "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs",
            "M8-W01-C02" => "docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs",
            "M8-W02-C01" => "docs/engineering/ProbeM8W02MigrationRiskRed.cs",
            "M8-W02-C02" => "docs/engineering/AssessM8W02C02Acceptance.cs",
            _ => throw new InvalidOperationException("Unexpected chunk: " + id)
        };
        var command = "dotnet run --file " + file + " -- " + (id == "M8-W02-C02" ? "--check-closure" : "--check");
        if (!File.Exists(Path.Combine(root, file))) throw new InvalidOperationException("Missing accepted validator: " + file);
        c["files"] = p.Json.StringArray(file, evidence[id]);
        c["commands"] = p.Json.StringArray(command);
        c["refinement"] = "Reconciled from exact durable native phase completion " + evidence[id] + "; see bounded achievement in the report.";
        c["readiness"] = "Completed bounded internal qualification; no broader release, migration or consumer authorization.";
        foreach (var t in c["tasks"]!.AsArray())
        {
            Complete(t!);
            foreach (var s in t!["subtasks"]!.AsArray()) Complete(s!);
        }
    }
    if (!seen.SetEquals(expected)) throw new InvalidOperationException("Canonical M8 roster differs.");
    doc["plan_version"] = "0.1.125";
});
using (var process = new Process { StartInfo = new ProcessStartInfo
{
    FileName = "dotnet", WorkingDirectory = root, UseShellExecute = false,
    RedirectStandardOutput = true, RedirectStandardError = true
}})
{
    foreach (var arg in new[] { "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write" })
        process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start()) throw new InvalidOperationException("Planning renderer could not start.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(240000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Planning renderer timed out.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (process.ExitCode != 0) throw new InvalidOperationException("Planning renderer failed: " + output);
}
return p.Complete();
