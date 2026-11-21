#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json;

const string PublishedVersion = "20260.20261120.1001";
const string Archive = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string Binary = "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea";
const string DependencyVersion = "20260.20261102.1001";
const string Dependency = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
void Need(bool value,string reason)
{
    if(!value)throw new InvalidDataException("RS274: "+reason);
}
var docs = new[]
{
    "docs/distribution/bounded-integer-sampling.md",
    "docs/distribution/m9-statistics-physical-private-publication-readback.md",
    "docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md"
};
foreach(var path in docs)
{
    var text = File.ReadAllText(path);
    Need(text.Contains(PublishedVersion, StringComparison.Ordinal) &&
        text.Contains(Archive, StringComparison.Ordinal) &&
        text.Contains(Binary, StringComparison.Ordinal) &&
        text.Contains(DependencyVersion, StringComparison.Ordinal) &&
        text.Contains(Dependency, StringComparison.Ordinal),
        "Unreconciled publication identity in "+path);
}
var handoff=File.ReadAllText(docs[2]);
Need(handoff.Contains("EG-A-01 / T002", StringComparison.Ordinal) &&
     handoff.Contains("maxRawDraws", StringComparison.Ordinal) &&
     handoff.Contains("source-free", StringComparison.OrdinalIgnoreCase) &&
     handoff.Contains("Assessa", StringComparison.Ordinal) &&
     handoff.Contains("not-ready", StringComparison.Ordinal),
     "External contract boundaries or native-plan caveat missing.");
using var doc=JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS273/attempt-001.json"));
var report=doc.RootElement;
Need(report.GetProperty("runId").GetString()=="RS273" &&
    report.GetProperty("attempt").GetInt32()==1 &&
    report.GetProperty("outcome").GetString()=="succeeded" &&
    report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0 &&
    report.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    report.GetProperty("git").GetProperty("commitMessage").GetString() ==
        "test: qualify published bounded Statistics private feed readback",
    "RS273 durable publication qualification differs.");
using var backlog=JsonDocument.Parse(File.ReadAllText("docs/planning/backlog.json"));
Need(backlog.RootElement.GetProperty("plan_version").GetString()=="0.1.124",
    "The canonical backlog baseline changed.");
Console.WriteLine("MATH-RS274-ASSESSA-HANDOFF|package=dw.tools.math.statistics|version="+
    PublishedVersion+"|durableRS273=first-pass|status=published-private|assessaAdoption=pending|PASS");
