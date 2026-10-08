using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string BudgetSource = "src/projects/dw.quantities/ExactRationalBudget.cs";
const string BudgetTests = "tests/projects/dw.quantities.tests/M1W01C01BudgetTests.cs";
const string Evidence = "docs/planning/evidence/M1-W01-C01-boundary-verification.json";
var p = PayloadContext.Create();
p.Files.ReplaceFromStaged("staged/" + BudgetSource, BudgetSource);
p.Files.ReplaceFromStaged("staged/" + BudgetTests, BudgetTests);

RunRequired(p.RepositoryRoot, "dotnet", "test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c", "Release", "--filter", "FullyQualifiedName~M1W01C01",
    "--logger", "console;verbosity=minimal");
RunRequired(p.RepositoryRoot, "pwsh", "-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1");

var sha = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(p.RepositoryRoot, BudgetSource)))).ToLowerInvariant();
var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C01-boundary-verification",
    ["status"] = "verified-with-explicit-budget-policy",
    ["source"] = BudgetSource,
    ["source_sha256"] = sha,
    ["typed_api"] = "dw.quantities.ExactRationalBudget.Add/Multiply",
    ["default_policy"] = "default(ExactRational) equals canonical zero 0/1.",
    ["precision"] = "Integer-only BigInteger arithmetic; no conversion to double.",
    ["resource_policy"] = "Callers explicitly select maxBits. Checked Add/Multiply conservatively reject oversized operands or intermediate products. Unbudgeted ExactRational operators remain unbounded; this is an opt-in API, not global enforcement.",
    ["scope"] = "M1-W01-C01 ExactRational boundary verification only.",
    ["focused_test_filter"] = "FullyQualifiedName~M1W01C01",
    ["foundation_chain"] = "scripts/verify.ps1",
    ["focused_tests_passed"] = true,
    ["foundation_chain_passed"] = true,
    ["external_aura_modified"] = false
};
p.Files.WriteComplete(Evidence, evidence.ToJsonString(new JsonSerializerOptions {WriteIndented=true}) + "\n");

var baselineProgress = false;
p.Json.EditObject("docs/planning/backlog.json", root =>
{
    var c01 = root["chunks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01");
    var t2 = c01["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01-T2");
    var verify = t2["subtasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == "M1-W01-C01-T2-V");
    baselineProgress = (string?)verify["status"] == "ready" &&
        (string?)t2["status"] == "in_progress" && (string?)c01["status"] == "in_progress" &&
        true;
    var targetProgress = (string?)verify["status"] == "done" &&
        (string?)t2["status"] == "done" && (string?)c01["status"] == "done" &&
        true;
    if (!baselineProgress && !targetProgress)
        throw new InvalidOperationException("RS011 progress is neither baseline nor target.");
    root["plan_version"] = "0.1.10";
    verify["status"] = "done";
    verify["evidence"] = p.Json.StringArray(Evidence);
    t2["status"] = "done";
    t2["evidence"] = p.Json.StringArray("docs/planning/evidence/M1-W01-C01-boundary-red.json",
        "docs/planning/evidence/M1-W01-C01-boundary-green.json", Evidence);
    c01["status"] = "done";
    var evidenceList = c01["evidence"]!.AsArray();
    if (!evidenceList.Any(x => (string?)x == Evidence))
        evidenceList.Add((JsonNode?)JsonValue.Create(Evidence));

});
if (baselineProgress)
{
    p.ProjectPlan.ActivateReadyContinuation("M1-W01-C01-T2-V");
    p.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T2-V");
    p.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T2");
    p.ProjectPlan.ConvergeNodeToDone("M1-W01-C01");
}
else
{
    foreach (var id in new[] {"M1-W01-C01-T2-V", "M1-W01-C01-T2", "M1-W01-C01"})
        p.ProjectPlan.RequireNodeState(id, "done");
}
RunRequired(p.RepositoryRoot, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
return p.Complete();

static void RunRequired(string root, string executable, params string[] args)
{
    using var proc = new Process { StartInfo = new ProcessStartInfo
    {
        FileName = executable, WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    }};
    foreach (var arg in args) proc.StartInfo.ArgumentList.Add(arg);
    if (!proc.Start()) throw new InvalidOperationException("Cannot start " + executable);
    var stdout = proc.StandardOutput.ReadToEndAsync();
    var stderr = proc.StandardError.ReadToEndAsync();
    if (!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree: true);
        throw new TimeoutException(executable + " timed out.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (proc.ExitCode != 0) throw new InvalidOperationException(executable + " failed: " + output);
}
