#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Feed = @"I:\nuget.local";
const string Evidence = "docs/distribution/electrochemical-published-readback.json";
const string Candidate = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
const string Version = "20260.20261110.1001";
const string Prior = "20260.20261102.1001";
if (args is not (["--check-feed"] or ["--check-receipt"]))
    throw new ArgumentException("Expected --check-feed or --check-receipt.");
using var authority = JsonDocument.Parse(File.ReadAllBytes(Candidate));
var c = authority.RootElement.GetProperty("candidate");
var packages = new[]
{
    (Id: "dw.quantities.standard", Version: Version, Bytes: 14269, Archive: Text(c, "candidateArchiveSha256"), Dll: Text(c, "candidateDllSha256")),
    (Id: "dw.quantities.expression", Version: Prior, Bytes: 13951, Archive: Text(c, "dependencyExpressionArchiveSha256"), Dll: Text(c, "dependencyExpressionDllSha256")),
    (Id: "dw.quantities", Version: Prior, Bytes: 14747, Archive: Text(c, "dependencyQuantitiesArchiveSha256"), Dll: Text(c, "dependencyQuantitiesDllSha256"))
};
if (args[0] == "--check-feed")
{
    var configured = Environment.GetEnvironmentVariable("NUGET_LOCAL");
    Need(OperatingSystem.IsWindows() && !string.IsNullOrEmpty(configured) && Path.IsPathFullyQualified(configured) &&
        string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(configured)), Feed, StringComparison.OrdinalIgnoreCase),
        "NUGET_LOCAL must select the feed observed by RS176");
    for (DirectoryInfo? directory = new(Feed); directory != null; directory = directory.Parent)
        Need(directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) == 0, "redirected or missing feed");
    foreach (var p in packages)
        CheckArchive(Path.Combine(Feed, p.Id + "." + p.Version + ".nupkg"), p.Bytes, p.Archive);
    CheckArchive(Path.Combine(Feed, "dw.quantities.standard.20260.20261103.1001.nupkg"), 13604, Text(c, "originalStandardArchiveSha256"));
    Console.WriteLine("RS177-FEED|closure=3|older-standard=preserved");
}
else
{
    using var evidence = JsonDocument.Parse(File.ReadAllBytes(Evidence));
    var e = evidence.RootElement;
    var result = e.GetProperty("readback");
    Need(Text(e, "status") == "rs177-electrochemical-published-readback-verified" &&
        Text(e, "publicationCommit") == "8fbea2196c0f60e92da6608fd84d9b6356f75093" &&
        Text(e, "gate") == "MATH-ELECTROCHEM-UNIT-PROFILE" && Text(e, "consumerNeed") == "A2-W02-C01-U-N" &&
        !e.GetProperty("auraIntegrationVerified").GetBoolean() && !e.GetProperty("auraHistoricalReuseRightsEstablished").GetBoolean() &&
        !e.GetProperty("remotePublication").GetBoolean() && e.GetProperty("olderStandardPreserved").GetBoolean() &&
        e.GetProperty("commands").EnumerateArray().Select(x => Text(x, "id")).SequenceEqual(
            new[] { "feed-before", "create-lock", "locked-restore", "consumer-build", "consumer-readback", "feed-after", "sdk" }) &&
        e.GetProperty("commands").EnumerateArray().All(x => x.GetProperty("exit_code").GetInt32() == 0), "incomplete readback provenance");
    var observed = result.GetProperty("packages").EnumerateArray().ToArray();
    Need(observed.Length == 3 && Text(result, "feed") == Feed && Text(result, "directPackage") == "dw.quantities.standard" &&
        Text(result, "profileId") == Text(c, "profileId") && Text(result, "profileVersion") == Text(c, "profileVersion") &&
        result.GetProperty("unitIds").GetArrayLength() == 6 && result.GetProperty("inheritedUnits").GetInt32() == 58 &&
        result.GetProperty("standardUnits").GetInt32() == 14, "published closure or catalogue identity differs");
    foreach (var p in packages)
    {
        var o = observed.Single(x => Text(x, "id") == p.Id);
        var dll = o.GetProperty("inventory").EnumerateArray().Single(x => Text(x, "path") == "lib/net10.0/" + p.Id + ".dll");
        Need(Text(o, "version") == p.Version && o.GetProperty("bytes").GetInt64() == p.Bytes &&
            Text(o, "archiveSha256") == p.Archive && Text(o, "cachedArchiveSha256") == p.Archive &&
            Text(o, "packagedDllSha256") == p.Dll && Text(o, "loadedDllSha256") == p.Dll &&
            Text(dll, "sha256") == p.Dll, "invalid archive-to-loaded-DLL receipt for " + p.Id);
    }
    Console.WriteLine("RS177-READBACK|archives=3|loaded-dlls=3|opt-in=6|inherited=58|standard=14");
}
static void CheckArchive(string path, long bytes, string expected)
{
    Need((File.GetAttributes(path) & (FileAttributes.ReparsePoint | FileAttributes.Directory)) == 0, "expected regular file: " + path);
    using var input = File.OpenRead(path);
    Need(input.Length == bytes && Convert.ToHexStringLower(SHA256.HashData(input)) == expected, "published file changed: " + path);
}
static string Text(JsonElement element, string key) => element.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing string " + key);
static void Need(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS177: " + reason);
}
