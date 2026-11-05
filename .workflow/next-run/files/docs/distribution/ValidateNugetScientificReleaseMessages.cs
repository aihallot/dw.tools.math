#:property NuGetAudit=false
#:property PublishAot=false

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json";
const string Status = "rs155-scientific-private-release-operator-messages-qualified-unpublished";
const string Ready = "MATH-SCIENTIFIC-F22:SOURCE=CORRECTED;BUILD=PENDING;NATIVE=541";
const string Qualified = "MATH-SCIENTIFIC-F22:SOURCE=CORRECTED;BUILD=PASS;BINARY-LABEL=15;NATIVE=541";
const string Limits = "MATH-SCIENTIFIC-F22:PUBLICATION=false;PUBLIC=false;ROLLBACK=false";

var files = new[]
{
    "docs/distribution/nuget-federation-f22-scientific-release-messages.md",
    "docs/distribution/ValidateNugetScientificReleaseMessages.cs",
    "tools/dw.tools.math.nuget-scientific-release/Program.cs"
};
const string Tool = "tools/dw.tools.math.nuget-scientific-release/";
const string Source = Tool + "Program.cs";
const string Candidate = "docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json";
const string Rehearsal = "docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json";
const string Previous = ".aura/workflow/reports/RS154/attempt-001.json";
const string ArchiveSha = "233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f";
const string DllSha = "604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c";
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS155 Scientific release outputs: " + reason);
}
static JsonDocument Load(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Value(JsonElement element, string name) =>
    element.GetProperty(name).GetString() ??
    throw new InvalidDataException("Missing exact JSON text field: " + name);
static bool Flag(JsonElement element, string name) => element.GetProperty(name).GetBoolean();
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement node)
{
    var total = 1;
    foreach (var key in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(key, out var children))
            total += children.EnumerateArray().Sum(Count);
    return total;
}

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "only observational --check is supported");
foreach (var path in files) Need(File.Exists(path), "missing installed source: " + path);
using var prior = Load(Previous);
using var f20 = Load(Candidate);
using var f21 = Load(Rehearsal);
using var backlog = Load("docs/planning/backlog.json");
using var plan = Load(".aura/workflow/plan/project.json");
using var guidance = Load("docs/workflow/manifest.json");

var rs154 = prior.RootElement;
Need(Value(rs154, "runId") == "RS154" &&
     rs154.GetProperty("attempt").GetInt32() == 1 &&
     Value(rs154, "outcome") == "succeeded" &&
     Value(rs154.GetProperty("executor"), "toolVersion") == "0.1.63" &&
     rs154.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 0 &&
     Flag(rs154.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Value(rs154.GetProperty("git"), "commitMessage") ==
         "test: qualify Scientific immutable NuGet publisher rehearsal",
     "exact RS154 first-pass predecessor required");
var original = f20.RootElement.GetProperty("candidate");
var rehearsed = f21.RootElement.GetProperty("rehearsal");
Need(Value(f20.RootElement, "status") ==
         "rs153-independent-scientific-calendar-nuget-candidate-qualified-unpublished" &&
     !Flag(f20.RootElement, "package_published") &&
     Value(original, "packageId") == "dw.tools.math.scientific" &&
     Value(original, "packageVersion") == "20260.20261105.1001" &&
     original.GetProperty("candidateArchiveBytes").GetInt32() == 37265 &&
     Value(original, "candidateArchiveSha256") == ArchiveSha &&
     Value(original, "candidateDllSha256") == DllSha &&
     original.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
     original.GetProperty("packageDependencies").GetInt32() == 0 &&
     original.GetProperty("originalSourcesSha256").EnumerateObject().Count() == 18,
     "original F20 Scientific identity/source closure drifted");
Need(Value(f21.RootElement, "status") ==
         "rs154-scientific-immutable-private-publisher-rehearsal-qualified-unpublished" &&
     !Flag(f21.RootElement, "package_published") &&
     Value(rehearsed, "originalCandidateArchiveSha256") == ArchiveSha &&
     Value(rehearsed, "originalCandidateDllSha256") == DllSha &&
     rehearsed.GetProperty("consumerChecks").GetInt32() == 15 &&
     Flag(rehearsed, "disposableFeedPublished") &&
     !Flag(rehearsed, "publishedToNugetLocal"),
     "F21 must attest only fifteen-case disposable rehearsal");
Need(Value(backlog.RootElement, "plan_version") == "0.1.122" &&
     Value(backlog.RootElement, "status") == "done" &&
     Value(guidance.RootElement, "guidanceVersion") == "0.26" &&
     plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(m => Value(m, "state") == "done"),
     "accepted Math native roadmap changed");

foreach (var entry in original.GetProperty("originalSourcesSha256").EnumerateObject())
    Need(File.Exists(entry.Name) && Hash(entry.Name) == entry.Value.GetString(),
         "original Scientific source fingerprint changed: " + entry.Name);

const string CorrectBytes =
    "only the exact original 37,265-byte RS153 Scientific candidate may be published";
const string WrongBytes =
    "only the exact original 3,863-byte RS153 Scientific candidate may be published";
const string CorrectBinary = "|binary=15|archive=";
const string WrongBinary = "|binary=5|archive=";
var program = File.ReadAllText(Source);
Need(program.Split(CorrectBytes, StringSplitOptions.None).Length - 1 == 1 &&
     !program.Contains(WrongBytes, StringComparison.Ordinal) &&
     program.Split(CorrectBinary, StringSplitOptions.None).Length - 1 == 1 &&
     !program.Contains(WrongBinary, StringComparison.Ordinal) &&
     program.Contains("Need(IsExactPhysical(original, 37265, CandidateSha)", StringComparison.Ordinal) &&
     program.Contains("Need(IsExactPhysical(archive, 37265, CandidateSha)", StringComparison.Ordinal) &&
     program.Contains("PackageDllHash(original) == CandidateDllSha", StringComparison.Ordinal) &&
     program.Contains("RequireCleanOwnerAndRs154(root)", StringComparison.Ordinal) &&
     program.Contains("MATH-SCIENTIFIC-BINARY|PASS|15", StringComparison.Ordinal) &&
     program.Contains("\"MATH-SCIENTIFIC-CHECK|\").Length - 1 == 15", StringComparison.Ordinal) &&
     program.Contains("DotNetLocalPackageRepository", StringComparison.Ordinal) &&
     program.Contains("PublisherDllSha", StringComparison.Ordinal),
     "Scientific owner output markers, archive guards or fifteen binary oracle checks drifted");
Need(File.ReadAllText("docs/distribution/nuget-federation-f22-scientific-release-messages.md")
         .Contains("## Exact repair and qualification boundary", StringComparison.Ordinal),
     "bounded repair rationale absent");
Need(File.ReadAllText(Tool + "Directory.Packages.props")
         .Contains("Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(Tool + "dw.tools.math.nuget-scientific-release.csproj")
         .Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
             StringComparison.Ordinal),
     "shared NuGet publisher identity must remain pinned");

var completed = File.Exists(Evidence);
if (completed)
{
    using var record = Load(Evidence);
    var result = record.RootElement;
    Need(Value(result, "status") == Status &&
         Value(result, "source_package_id") == "dw.tools.math.scientific" &&
         Value(result, "preceding_run") == "RS154" &&
         Value(result, "sourceBaselineCommit") ==
             "c6621ea1c5032c8e52da391bd233e0e0df5f1ff1" &&
         Value(result, "original_archive_sha256") == ArchiveSha &&
         Value(result, "original_dll_sha256") == DllSha &&
         result.GetProperty("operator_message_archive_bytes").GetInt32() == 37265 &&
         result.GetProperty("operator_message_binary_checks").GetInt32() == 15 &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Flag(result, "release_controller_compiled") &&
         !Flag(result, "release_modes_invoked") &&
         !Flag(result, "package_published_by_this_run") &&
         !Flag(result, "new_public_license_selected") &&
         !Flag(result, "installed_package_rollback_qualified") &&
         !Flag(result, "original_scientific_sources_modified") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "discovery_migration_qualified"),
         "F22 must be source/build evidence, not a release or migration claim");
    var hashes = result.GetProperty("source_files");
    foreach (var file in files)
        Need(Value(hashes, file) == Hash(file),
             "RS155 installed source checksum changed: " + file);
    var commands = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.Length == 3 &&
         commands.Select(x => Value(x, "id")).SequenceEqual(
             new[] { "release-restore", "release-build", "sdk" }) &&
         commands.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
         !string.IsNullOrWhiteSpace(Value(result, "sdk_version")),
         "actual restore, compilation and SDK command receipts required");
}
Console.WriteLine(completed ? Qualified : Ready);
Console.WriteLine(Limits);
return 0;
