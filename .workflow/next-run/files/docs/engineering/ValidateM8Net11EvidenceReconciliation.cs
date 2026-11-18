#:property NuGetAudit=false
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Audit = "docs/engineering/m8-net11-csharp15-migration-evidence.md";
var audit = File.ReadAllText(Audit);
var commits = new[]
{
    "test: qualify Math exact parser Linux WSL net11 consumer",
    "test: qualify published Math quantities binary in net11 consumer",
    "test: qualify published Math quantities net10 net11 binary parity",
    "test: qualify published Math quantities binary on Linux net11",
    "test: qualify published Math Expression binaries on net11",
    "test: qualify published Math Standard binaries on net11",
    "test: qualify published Math Standard binaries on Linux net11",
    "test: qualify Math distribution project graph on net11",
    "test: qualify Math distribution project graph on Linux net11"
};
for (var run = 240; run <= 251; run++)
{
    using var report = JsonDocument.Parse(File.ReadAllText(
        ".aura/workflow/reports/RS" + run + "/attempt-001.json"));
    var result = report.RootElement;
    Require(result.GetProperty("runId").GetString() == "RS" + run &&
        result.GetProperty("outcome").GetString() == "succeeded" &&
        result.GetProperty("attempt").GetInt32() == 1, "Run result changed: RS" + run);
    var prior = result.GetProperty("priorInvocationFailures");
    var corrected = run <= 242;
    Require(prior.GetProperty("count").GetInt32() == (corrected ? 1 : 0) &&
        prior.GetProperty("firstPassSucceeded").GetBoolean() == !corrected,
        "Run first-pass lineage differs: RS" + run);
    if (run >= 243)
        Require(result.GetProperty("git").GetProperty("commitMessage").GetString() ==
            commits[run - 243], "Unexpected run commit: RS" + run);
}
foreach (var phrase in new[]
{
    "RS244-RS249", "RS250-RS251", "not-ready",
    "published Distribution binary", "global.json", "Directory.Build.props"
})
    Require(audit.Contains(phrase, StringComparison.Ordinal),
        "Audit missing evidence limit: " + phrase);
Require(File.ReadAllText("global.json").Contains("\"10.0.401\"", StringComparison.Ordinal) &&
    File.ReadAllText("Directory.Build.props").Contains("<TargetFramework>net10.0</TargetFramework>", StringComparison.Ordinal),
    "Unapproved production SDK/TFM migration.");
Console.WriteLine("MATH-RS252-AUDIT|reports=12|corrected=3|firstPass=9|" +
    "distribution=bounded|publishedQuantities=bounded|m8=not-ready|PASS");
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS252: " + reason);
}
