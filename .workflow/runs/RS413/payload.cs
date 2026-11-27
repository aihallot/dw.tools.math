using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS412", 1, "chore: close M10 production cutover RED and ready GREEN", "0.1.81", 1, 1, 0, false);
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M10-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M11", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.179"), JsonValue.Create("0.1.180"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM11NumericSubsidiarityAdmission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-production-cutover-green-rs413.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ProbeM10ProductionCutover.cs"));
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    JsonNode Find(JsonNode parent, string key, string id) => parent[key]!.AsArray().Single(x => (string?)x?["id"] == id)!;
    var chunk = Find(doc, "chunks", "M10-W01-C02");
    var task = Find(chunk, "tasks", "M10-W01-C02-T1");
    var red = Find(task, "subtasks", "M10-W01-C02-T1-R");
    var green = Find(task, "subtasks", "M10-W01-C02-T1-G");
    var target = (string?)doc["plan_version"] == "0.1.180";
    const string evidencePath = ".aura/workflow/reports/RS412/attempt-001.json";
    var evidence = red["evidence"]!.AsArray();
    if ((string?)red["status"] != (target ? "done" : "in_progress") ||
        (string?)green["status"] != (target ? "ready" : "planned") ||
        (target ? evidence.Count != 1 || (string?)evidence[0] != evidencePath : evidence.Count != 0))
        throw new InvalidOperationException("Unexpected C02 RED closure baseline.");
    red["status"] = "done";
    red["evidence"] = p.Json.StringArray(evidencePath);
    green["status"] = "ready";
    var files = chunk["files"]!.AsArray().Select(x => (string)x!).Concat(new[] { "docs/planning/decisions/m10-production-cutover-green-rs413.md", "docs/engineering/ProbeM10ProductionCutover.cs", "Directory.Build.props", "global.json", "src/projects/dw.quantities.expression/packages.lock.json", "src/projects/dw.quantities.standard/packages.lock.json", "src/projects/dw.quantities/packages.lock.json", "src/projects/dw.tools.math.analysis/packages.lock.json", "src/projects/dw.tools.math.codecs/packages.lock.json", "src/projects/dw.tools.math.composition/packages.lock.json", "src/projects/dw.tools.math.distribution/packages.lock.json", "src/projects/dw.tools.math.foundation/packages.lock.json", "src/projects/dw.tools.math.ir/packages.lock.json", "src/projects/dw.tools.math.numerics/packages.lock.json", "src/projects/dw.tools.math.scientific/packages.lock.json", "src/projects/dw.tools.math.statistics/packages.lock.json", "src/projects/dw.tools.math.symbolics/packages.lock.json", "tests/projects/dw.quantities.tests/packages.lock.json", "tests/projects/dw.tools.math.analysis.tests/packages.lock.json", "tests/projects/dw.tools.math.codecs.tests/packages.lock.json", "tests/projects/dw.tools.math.composition.tests/packages.lock.json", "tests/projects/dw.tools.math.distribution.tests/packages.lock.json", "tests/projects/dw.tools.math.foundation.tests/packages.lock.json", "tests/projects/dw.tools.math.ir.tests/packages.lock.json", "tests/projects/dw.tools.math.numerics.tests/packages.lock.json", "tests/projects/dw.tools.math.scientific.tests/packages.lock.json", "tests/projects/dw.tools.math.statistics.tests/packages.lock.json", "tests/projects/dw.tools.math.symbolics.tests/packages.lock.json", "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj", "tools/dw.tools.math.nuget-preflight/Program.cs" }).Distinct(StringComparer.Ordinal).ToArray();
    chunk["files"] = p.Json.StringArray(files);
    var commands = chunk["commands"]!.AsArray().Select(x => (string)x!).Concat(new[] { "dotnet run --file docs/engineering/ProbeM10ProductionCutover.cs -- --check" }).Distinct(StringComparer.Ordinal).ToArray();
    chunk["commands"] = p.Json.StringArray(commands);
    chunk["readiness"] = "RS412 durable RED closure; actual production cutover and GREEN activation pending durable RS413 result.";
    doc["plan_version"] = "0.1.180";
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
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("Directory.Build.props"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("global.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.quantities.expression/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.quantities.standard/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.quantities/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.analysis/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.codecs/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.composition/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.distribution/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.foundation/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.ir/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.numerics/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.scientific/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.statistics/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("src/projects/dw.tools.math.symbolics/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.quantities.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.analysis.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.codecs.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.composition.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.distribution.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.foundation.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.ir.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.numerics.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.scientific.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.statistics.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tests/projects/dw.tools.math.symbolics.tests/packages.lock.json"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tools/dw.tools.math.nuget-preflight/Program.cs"));
p.ProjectPlan.ActivateReadyContinuation("M10-W01-C02-T1-G");
return p.Complete();
