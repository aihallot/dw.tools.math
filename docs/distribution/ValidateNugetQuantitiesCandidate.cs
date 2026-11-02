#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Evidence = "docs/distribution/nuget-federation-f2-quantities-qualified.json";
const string Status = "rs135-quantities-calendar-candidate-qualified-unpublished";
const string Scope = "MATH-NUGET-F2:PACKAGE=dw.quantities;BINARY=4;NATIVE=541";
const string Limits = "MATH-NUGET-F2:PUBLISHED=false;ROLLBACK=false;DISCOVERY=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f2-quantities.md",
    "docs/distribution/ValidateNugetQuantitiesCandidate.cs",
    "tools/dw.tools.math.nuget-candidate/Program.cs",
    "tools/dw.tools.math.nuget-candidate/dw.tools.math.nuget-candidate.csproj",
    "tools/dw.tools.math.nuget-candidate/Directory.Packages.props",
    "tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs"
};
static void Need(bool ok, string detail)
{
    if (!ok) throw new InvalidOperationException("RS135 candidate: " + detail);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Text(JsonElement node, string name) => node.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing string field " + name);
static bool Flag(JsonElement node, string name) => node.GetProperty(name).GetBoolean();
static string Digest(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var value = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            value += children.EnumerateArray().Sum(Count);
    return value;
}
static bool Hash64(string digest) =>
    Regex.IsMatch(digest, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant);

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only --check is allowed");
foreach (var file in files)
    Need(File.Exists(file), "missing source " + file);

using var prior = Read(".aura/workflow/reports/RS134/attempt-001.json");
using var f1 = Read("docs/distribution/nuget-federation-f1-qualified.json");
using var plan = Read(".aura/workflow/plan/project.json");
using var backlog = Read("docs/planning/backlog.json");
Need(Text(prior.RootElement, "runId") == "RS134" &&
     prior.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Text(prior.RootElement, "outcome") == "succeeded" &&
     Text(prior.RootElement.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     prior.RootElement.GetProperty("priorInvocationFailures")
         .GetProperty("count").GetInt32() == 2 &&
     !Flag(prior.RootElement.GetProperty("priorInvocationFailures"),
         "firstPassSucceeded"),
     "RS134 DWF historical lineage must remain exact");
Need(Text(f1.RootElement, "status") ==
     "rs134-math-f1-calendar-feed-inventory-no-publication" &&
     f1.RootElement.GetProperty("observed").GetProperty("packageCount").GetInt32() == 13 &&
     !Flag(f1.RootElement, "package_published"),
     "the accepted F1 inventory is missing");
var f1OwnedSources = f1.RootElement.GetProperty("source_files");
foreach (var historicFile in new[]
{
    "docs/distribution/nuget-federation-f1.md",
    "docs/distribution/ValidateNugetFederationF1.cs",
    "tools/dw.tools.math.nuget-preflight/Program.cs",
    "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj",
    "tools/dw.tools.math.nuget-preflight/Directory.Packages.props"
})
    Need(Text(f1OwnedSources, historicFile) == Digest(historicFile),
         "a source-pinned RS134 F1 file was changed: " + historicFile);
Need(Text(backlog.RootElement, "status") == "done" &&
     Text(backlog.RootElement, "plan_version") == "0.1.122",
     "Math accepted backlog changed");
Need(plan.RootElement.GetProperty("milestones").EnumerateArray()
         .Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Text(m, "state") == "done"),
     "accepted 541-node native Math completion changed");
Need(File.ReadAllText("src/projects/dw.quantities/dw.quantities.csproj")
        .Contains("<Version>0.2.0-preview.1</Version>", StringComparison.Ordinal),
     "source package version was changed");

if (File.Exists(Evidence))
{
    using var receiptDoc = Read(Evidence);
    var receipt = receiptDoc.RootElement;
    Need(Text(receipt, "status") == Status &&
         Text(receipt, "source_package_id") == "dw.quantities" &&
         Text(receipt, "feed_selector") == "NUGET_LOCAL" &&
         receipt.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(receipt, "package_published") &&
         !Flag(receipt, "release_authorized_by_this_run") &&
         !Flag(receipt, "installed_package_rollback_qualified") &&
         !Flag(receipt, "source_versions_modified") &&
         !Flag(receipt, "external_requests_transmitted") &&
         !Flag(receipt, "discovery_migration_qualified"),
         "receipt claims unauthorized publication, rollback or migration");
    var hashes = receipt.GetProperty("source_files");
    foreach (var file in files)
        Need(Text(hashes, file) == Digest(file),
            "new source SHA differs from verified receipt: " + file);

    var candidate = receipt.GetProperty("candidate");
    Need(Text(candidate, "status") == "qualified-local-candidate-not-published" &&
         Text(candidate, "sourcePackageId") == "dw.quantities" &&
         Text(candidate, "sourceVersion") == "0.2.0-preview.1" &&
         Regex.IsMatch(Text(candidate, "packageVersion"),
             @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
             RegexOptions.CultureInvariant) &&
         Text(candidate, "sharedCore") == "dw.tools.nuget/[20260.20261026.1010]" &&
         Text(candidate, "sharedCoreDllSha256") ==
            "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         Hash64(Text(candidate, "candidateArchiveSha256")) &&
         Hash64(Text(candidate, "candidateDllSha256")) &&
         candidate.GetProperty("candidateArchiveBytes").GetInt64() > 0 &&
         candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
         candidate.GetProperty("consumerChecks").GetInt32() == 4 &&
         Flag(candidate, "sourceFeedUnchanged") &&
         Flag(candidate, "oldSourcePackageVersionRetained") &&
         Flag(candidate, "compiledSourceFreeConsumerPassed") &&
         !Flag(candidate, "versionReserved") &&
         !Flag(candidate, "published"),
         "candidate package provenance, loaded binary or release status unqualified");
    Need(Text(candidate, "candidateArchivePath").StartsWith(
         "build/artifacts/rs135-quantities-candidate/candidate/dw.quantities.",
         StringComparison.Ordinal), "candidate path must remain in ignored local artifacts");
    var internalCommands = candidate.GetProperty("commands").EnumerateArray().ToArray();
    var internalIds = new[]
    {
        "f1-inventory-before", "source-locked-restore", "source-release-build",
        "candidate-pack", "consumer-restore", "consumer-release-build",
        "consumer-binary-execution", "f1-inventory-after", "sdk"
    };
    Need(internalCommands.Length == internalIds.Length &&
         internalCommands.Select(c => Text(c, "id"))
             .SequenceEqual(internalIds, StringComparer.Ordinal) &&
         internalCommands.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "as-run source, package and isolated consumer commands incomplete");
    var outer = receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(outer.Length == 6 &&
         outer.Select(x => Text(x, "id")).SequenceEqual(
             new[] { "f1-restore", "f1-build", "tool-restore",
                     "tool-build", "candidate-consumer", "sdk" },
             StringComparer.Ordinal) &&
         outer.All(x => x.GetProperty("exit_code").GetInt32() == 0),
         "as-run DWF parent command receipts incomplete");
}
Console.WriteLine(Scope);
Console.WriteLine(Limits);
return 0;
