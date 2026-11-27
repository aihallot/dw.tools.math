#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json;
using System.Linq;

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
const string LinuxReport = ".aura/workflow/reports/RS277/attempt-001.json";
Need(handoff.Contains("Linux/WSL binary consumer", StringComparison.Ordinal) &&
    handoff.Contains("RS277 report:", StringComparison.Ordinal) &&
    handoff.Contains("source-free locked", StringComparison.OrdinalIgnoreCase) &&
    handoff.Contains("aed67c6e5b2a4ccceb87e9737a74bbf8e1f0ff6b", StringComparison.Ordinal) &&
    handoff.Contains("Assessa adoption pending", StringComparison.Ordinal) &&
    handoff.Contains("no rebuild, republication, Assessa adoption", StringComparison.Ordinal),
    "Missing Linux consumer evidence or non-adoption boundary.");
using (var linuxDoc = JsonDocument.Parse(File.ReadAllText(LinuxReport)))
{
    var linux = linuxDoc.RootElement;
    var prior = linux.GetProperty("priorInvocationFailures");
    var observed = linux.GetProperty("contribution").GetProperty("observed");
    Need(linux.GetProperty("runId").GetString() == "RS277" &&
        linux.GetProperty("attempt").GetInt32() == 1 &&
        linux.GetProperty("outcome").GetString() == "succeeded" &&
        linux.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.66" &&
        linux.GetProperty("git").GetProperty("commitMessage").GetString() ==
            "test: qualify published bounded Statistics binaries on Linux net11" &&
        prior.GetProperty("count").GetInt32() == 0 &&
        prior.GetProperty("firstPassSucceeded").GetBoolean() &&
        linux.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
        observed.GetProperty("mutationBoundaryClean").GetBoolean() &&
        observed.GetProperty("changedPaths").EnumerateArray().Count() == 1 &&
        observed.GetProperty("changedPaths").EnumerateArray().Any(x =>
            x.GetString() == "docs/engineering/ProbeM9WslNet11PublishedBoundedStatistics.cs") &&
        observed.GetProperty("validations").EnumerateArray().Any(x =>
            x.GetProperty("id").GetString() ==
                "m9-published-bounded-statistics-linux-net11-independent-binary"),
        "RS277 Linux/WSL source-free independent binary evidence differs.");
}
using var backlog=JsonDocument.Parse(File.ReadAllText("docs/planning/backlog.json"));
Need(backlog.RootElement.GetProperty("plan_version").GetString()=="0.1.155",
    "The canonical backlog baseline changed.");
Console.WriteLine("MATH-RS274-ASSESSA-HANDOFF|package=dw.tools.math.statistics|version="+
    PublishedVersion+"|durableRS273=first-pass|status=published-private|assessaAdoption=pending|PASS");
Console.WriteLine("MATH-RS278-LINUX-HANDOFF|durableRS277=first-pass|linuxNet11=source-free-locked|" +
    "publishedVersion=" + PublishedVersion + "|assessaAdoption=pending|historicalM9=not-ready|PASS");
