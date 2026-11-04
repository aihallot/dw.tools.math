#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Receipt = "docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json";
const string Status = "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished";
const string Ready = "MATH-ANALYSIS-F14:SOURCE=READY;CANDIDATE=PENDING;NATIVE=541";
const string Qualified = "MATH-ANALYSIS-F14:CANDIDATE=QUALIFIED;BINARY=13;DEPENDENCIES=0;NATIVE=541";
const string Limits = "MATH-ANALYSIS-F14:PUBLISHED=false;LICENSE=false;ROLLBACK=false";
var installed = new[]
{
    "docs/distribution/nuget-federation-f14-analysis-candidate.md",
    "docs/distribution/ValidateNugetAnalysisCandidate.cs",
    "tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj",
    "tools/dw.tools.math.nuget-analysis-candidate/Program.cs",
    "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs"
};
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS147 source-pinned candidate: " + reason);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement source, string key) =>
    source.GetProperty(key).GetString() ??
    throw new InvalidDataException("Expected " + key);
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var count = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}
Require(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is supported");
foreach (var file in installed)
    Require(File.Exists(file), "missing staged installed source " + file);
using var report = Read(".aura/workflow/reports/RS146/attempt-001.json");
using var previous = Read("docs/distribution/nuget-federation-f13-ir-published-qualified.json");
using var inventory = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var roadmap = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
using var guidance = Read("docs/workflow/manifest.json");
var last = report.RootElement;
Require(Text(last, "runId") == "RS146" &&
    last.GetProperty("attempt").GetInt32() == 1 &&
    Text(last, "outcome") == "succeeded" &&
    last.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
    last.GetProperty("priorInvocationFailures").GetProperty("firstPassSucceeded").GetBoolean() &&
    Text(last.GetProperty("executor"), "toolVersion") == "0.1.63" &&
    Text(last.GetProperty("git"), "commitMessage") ==
        "test: verify published IR NuGet and exact Quantities dependency binaries",
    "exact RS146 predecessor success must be durable");
Require(Text(previous.RootElement, "status") ==
    "rs146-ir-published-local-two-binaries-independently-verified" &&
    previous.RootElement.GetProperty("physically_published_package_verified").GetBoolean() &&
    previous.RootElement.GetProperty("dependency_package_verified").GetBoolean() &&
    !previous.RootElement.GetProperty("package_published_by_this_run").GetBoolean(),
    "original physical-feed qualification cannot drift");
var packages = inventory.RootElement.GetProperty("observed")
    .GetProperty("packages").EnumerateArray().ToArray();
Require(packages.Length == 13 &&
    inventory.RootElement.GetProperty("package_count").GetInt32() == 13 &&
    packages.Single(x => Text(x, "packageId") == "dw.tools.math.analysis")
        .GetProperty("projectReferences").GetArrayLength() == 0 &&
    packages.Single(x => Text(x, "packageId") == "dw.tools.math.analysis")
        .GetProperty("externalPackageReferences").GetArrayLength() == 0,
    "source-selected package needs exact zero-dependency inventory");
Require(Text(backlog.RootElement, "status") == "done" &&
    Text(backlog.RootElement, "plan_version") == "0.1.122" &&
    Text(guidance.RootElement, "guidanceVersion") == "0.26" &&
    roadmap.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
    roadmap.RootElement.GetProperty("milestones").EnumerateArray()
        .All(m => Text(m, "state") == "done"),
    "roadmap/guidance or 541 completed native nodes drifted");
const string Tool = "tools/dw.tools.math.nuget-analysis-candidate/";
Require(File.ReadAllText(Tool + "dw.tools.math.nuget-analysis-candidate.csproj")
        .Contains("<Compile Include=\"Program.cs\" />", StringComparison.Ordinal) &&
    !File.ReadAllText(Tool + "dw.tools.math.nuget-analysis-candidate.csproj")
        .Contains("PackageReference", StringComparison.Ordinal) &&
    File.ReadAllText(Tool + "fixtures/AnalysisBinaryConsumer.cs")
        .Contains("MATH-ANALYSIS-BINARY|PASS|13", StringComparison.Ordinal) &&
    File.ReadAllText(Tool + "Program.cs")
        .Contains("source-pack", StringComparison.Ordinal) &&
    File.ReadAllText(Tool + "Program.cs")
        .Contains("sourceFeedUnchanged", StringComparison.Ordinal) &&
    File.ReadAllText("src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj")
        .Contains("<Version>1.1.0-preview.6</Version>", StringComparison.Ordinal),
    "source-free consumer or unchanged source project identity missing");
if (File.Exists(Receipt))
{
    using var evidence = Read(Receipt);
    var result = evidence.RootElement;
    Require(Text(result, "status") == Status &&
        Text(result, "source_package_id") == "dw.tools.math.analysis" &&
        Text(result, "feed_selector") == "NUGET_LOCAL" &&
        result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
        !result.GetProperty("package_published").GetBoolean() &&
        !result.GetProperty("release_authorized_by_this_run").GetBoolean() &&
        !result.GetProperty("source_versions_modified").GetBoolean() &&
        !result.GetProperty("new_public_license_selected").GetBoolean() &&
        !result.GetProperty("installed_package_rollback_qualified").GetBoolean(),
        "candidate receipt cannot claim publication or source mutations");
    foreach (var file in installed)
        Require(Text(result.GetProperty("source_files"), file) == Sha(file),
            "staged source changed: " + file);
    var observed = result.GetProperty("candidate");
    Require(Text(observed, "status") ==
            "qualified-independent-analysis-calendar-candidate-not-published" &&
        Text(observed, "packageId") == "dw.tools.math.analysis" &&
        Text(observed, "packageVersion") == "20260.20261104.1001" &&
        Text(observed, "sourceVersion") == "1.1.0-preview.6" &&
        Text(observed, "candidateArchivePath") ==
            "build/artifacts/rs147-analysis-candidate/candidate/dw.tools.math.analysis.20260.20261104.1001.nupkg" &&
        observed.GetProperty("candidateArchiveBytes").GetInt64() > 2000 &&
        Text(observed, "candidateArchiveSha256").Length == 64 &&
        Text(observed, "candidateDllSha256").Length == 64 &&
        observed.GetProperty("packageDependencies").GetInt32() == 0 &&
        observed.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 13 &&
        observed.GetProperty("independentPackageReference").GetBoolean() &&
        observed.GetProperty("sourceProjectUnmodified").GetBoolean() &&
        observed.GetProperty("sourceFeedUnchanged").GetBoolean() &&
        !observed.GetProperty("originalProjectReferencesUsed").GetBoolean() &&
        !observed.GetProperty("packagePublished").GetBoolean() &&
        !observed.GetProperty("versionReserved").GetBoolean() &&
        !observed.GetProperty("publicLicenseSelected").GetBoolean() &&
        !observed.GetProperty("installedPackageRollbackQualified").GetBoolean() &&
        !string.IsNullOrWhiteSpace(Text(observed, "sdkVersion")) &&
        !string.IsNullOrWhiteSpace(Text(observed, "runtimeVersion")) &&
        !string.IsNullOrWhiteSpace(Text(observed, "osDescription")) &&
        !string.IsNullOrWhiteSpace(Text(observed, "processArchitecture")),
        "analysis candidate identity, platform and binary checks incomplete");
    var originalSources = observed.GetProperty("originalSourcesSha256");
    Require(originalSources.EnumerateObject().Count() == 8,
        "exact seven original source files and one csproj SHA required");
    foreach (var source in originalSources.EnumerateObject())
        Require(File.Exists(source.Name) && Sha(source.Name) == source.Value.GetString(),
            "original analysis source bytes changed: " + source.Name);
    var inner = observed.GetProperty("commandReceipts").EnumerateArray().ToArray();
    var outer = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Require(inner.Length == 7 && outer.Length == 4 &&
        inner.Select(x => Text(x, "id")).SequenceEqual(new[]
            { "source-restore", "source-build", "source-pack",
              "consumer-restore", "consumer-build", "consumer-binary", "sdk" }) &&
        outer.Select(x => Text(x, "id")).SequenceEqual(new[]
            { "tool-restore", "tool-build", "analysis-candidate", "sdk" }) &&
        inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
        outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
        "real seven inner and four outer command receipts required");
}
Console.WriteLine(File.Exists(Receipt) ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
