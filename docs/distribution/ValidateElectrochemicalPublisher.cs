#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Evidence = "docs/distribution/electrochemical-publisher-qualified.json";
const string Status = "rs175-electrochemical-original-archive-publisher-qualified-unpublished";
const string Tool = "tools/dw.tools.math.nuget-electrochemical-release/";
string[] files =
[
    "docs/distribution/electrochemical-publisher.md",
    "docs/distribution/ValidateElectrochemicalPublisher.cs",
    Tool + "dw.tools.math.nuget-electrochemical-release.csproj",
    Tool + "Directory.Packages.props",
    Tool + "Program.cs"
];
string[] controls =
[
    "prepare-no-write", "exact-archive-publish", "independent-archive-readback",
    "collision-refused", "digest-mismatch-refused", "cancellation-refused", "original-inputs-preserved"
];
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check", "only observational --check is supported");
foreach (var path in files) Need(File.Exists(path), "missing publisher source " + path);
using var original = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/electrochemical-nuget-candidate-qualified.json"));
var candidate = original.RootElement.GetProperty("candidate");
Need(Text(original.RootElement, "status") ==
         "rs174-electrochemical-standard-calendar-candidate-qualified-unpublished" &&
     !original.RootElement.GetProperty("package_published").GetBoolean() &&
     Text(candidate, "packageVersion") == "20260.20261110.1001" &&
     Text(candidate, "candidateArchiveSha256") ==
         "766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d" &&
     candidate.GetProperty("consumerChecks").GetInt32() == 12,
     "RS174 original acquired qualification changed");
Need(File.ReadAllText(files[2]).Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />", StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("Version=\"[20260.20261026.1010]\"", StringComparison.Ordinal) &&
     File.ReadAllText(files[4]).Contains("--rehearse-publisher", StringComparison.Ordinal),
     "publisher binding or bounded CLI changed");
if (File.Exists(Evidence))
{
    using var document = JsonDocument.Parse(File.ReadAllText(Evidence));
    var result = document.RootElement;
    var publisher = result.GetProperty("publisher");
    Need(Text(result, "status") == Status &&
         Text(result, "previous_result_commit") == "bca063866734c105f5448bba049e620b9d490c42" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !result.GetProperty("package_published").GetBoolean() &&
         !result.GetProperty("aura_code_reuse_rights_established").GetBoolean() &&
         result.GetProperty("source_files").EnumerateObject().Count() == files.Length &&
         result.GetProperty("source_files").EnumerateObject().All(x =>
             files.Contains(x.Name, StringComparer.Ordinal) && Hash(x.Name) == x.Value.GetString()) &&
         result.GetProperty("command_receipts").GetArrayLength() == 4,
         "publisher receipt authority, source hashes or command receipts changed");
    Need(Text(publisher, "status") == "qualified-original-electrochemical-publisher-unpublished" &&
         Text(publisher, "packageId") == "dw.quantities.standard" &&
         Text(publisher, "packageVersion") == "20260.20261110.1001" &&
         Text(publisher, "originalArchiveSha256") == Text(candidate, "candidateArchiveSha256") &&
         Text(publisher, "originalDllSha256") == Text(candidate, "candidateDllSha256") &&
         Text(publisher, "publisherDllSha256") == Text(candidate, "sharedNugetDllSha256") &&
         publisher.GetProperty("originalArchiveBytes").GetInt32() == 14269 &&
         publisher.GetProperty("controlsPassed").GetInt32() == controls.Length &&
         publisher.GetProperty("controls").EnumerateArray().Select(x => x.GetString()).SequenceEqual(controls) &&
         publisher.GetProperty("archiveInventoryEntriesVerified").GetInt32() == 5 &&
         publisher.GetProperty("requiredPublishedArchivesPreserved").GetInt32() == 3 &&
         publisher.GetProperty("disposableFeedPublished").GetBoolean() &&
         publisher.GetProperty("independentArchiveReadback").GetBoolean() &&
         publisher.GetProperty("originalCandidatePreserved").GetBoolean() &&
         publisher.GetProperty("sourceFilesPreserved").GetBoolean() &&
         !publisher.GetProperty("packageRebuilt").GetBoolean() &&
         !publisher.GetProperty("sourceCampaignReplayed").GetBoolean() &&
         !publisher.GetProperty("binaryCampaignReplayed").GetBoolean() &&
         !publisher.GetProperty("publishedToNugetLocal").GetBoolean() &&
         !publisher.GetProperty("auraCodeReuseRightsEstablished").GetBoolean() &&
         !publisher.GetProperty("installedRollbackQualified").GetBoolean(),
         "seven exact-byte publisher controls or publication boundaries incomplete");
}
Console.WriteLine(File.Exists(Evidence)
    ? "MATH-ELECTROCHEM-PUBLISHER:CONTROLS=7;ORIGINAL=EXACT;NATIVE=541"
    : "MATH-ELECTROCHEM-PUBLISHER:SOURCE=READY;QUALIFICATION=PENDING;NATIVE=541");
Console.WriteLine("MATH-ELECTROCHEM-PUBLISHER:SHARED-PUBLISHED=false;REBUILT=false;PUBLIC=false");

static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS175: " + message);
}
static string Text(JsonElement value, string key) =>
    value.GetProperty(key).GetString() ?? throw new InvalidDataException("missing field " + key);
static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
