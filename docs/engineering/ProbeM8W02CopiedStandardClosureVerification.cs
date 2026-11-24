#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string OriginalVerifier = "docs/engineering/ValidateM8W02PhysicalStandardClosure.cs";
const string OriginalArchiveAuthority = "docs/engineering/ProbeM8Net11PublishedStandardLockedRestore.cs";
const string Ledger = "docs/engineering/m8-w02-c01-published-standard-closure-rs322.json";
const string Historical253 = ".aura/workflow/reports/RS253/attempt-001.json";
const string Historical254 = ".aura/workflow/reports/RS254/attempt-001.json";

string[] files =
[
    "global.json", "Directory.Build.props", OriginalVerifier,
    OriginalArchiveAuthority, Ledger, Historical253, Historical254,
    "src/projects/dw.quantities/dw.quantities.csproj",
    "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
    "src/projects/dw.quantities.standard/dw.quantities.standard.csproj"
];
Need(files.Length == 10 && files.Distinct(StringComparer.Ordinal).Count() == 10 &&
    files.All(File.Exists),
    "Independent qualification needs exactly ten canonical Math inputs.");
var originals = files.ToDictionary(x => x, Hash, StringComparer.Ordinal);
using var sourceLedger = JsonDocument.Parse(File.ReadAllText(Ledger));
var packages = sourceLedger.RootElement.GetProperty("packages").EnumerateArray()
    .Select(x => (Id: x.GetProperty("packageId").GetString()!,
                  Version: x.GetProperty("version").GetString()!,
                  ArchiveHash: x.GetProperty("archiveSha256").GetString()!))
    .ToArray();
Need(packages.Length == 3 &&
    packages.Select(x => x.Id).ToHashSet(StringComparer.Ordinal)
        .SetEquals(new[] {
            "dw.quantities", "dw.quantities.expression", "dw.quantities.standard"
        }),
    "RS322 is not a bounded three-package archive profile.");

var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "NUGET_LOCAL must be a physical absolute private feed.");

var workspace = Path.GetFullPath(Path.Combine(
    "build", "artifacts", "v325-" + Guid.NewGuid().ToString("N")[..8]));
var copiedFeed = Path.Combine(workspace, "feed");
Directory.CreateDirectory(copiedFeed);
foreach (var file in files)
{
    var dest = Copied(file);
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(file, dest);
    Need(Hash(dest) == originals[file],
        "A copied Math evidence/source file is not byte-identical: " + file);
}
var archiveCopies = new Dictionary<string, (string Original, string Copy, string Sha)>(
    StringComparer.Ordinal);
foreach (var package in packages)
{
    var basename = package.Id + "." + package.Version + ".nupkg";
    var archive = Path.Combine(feed!, basename);
    Need(File.Exists(archive) &&
        (File.GetAttributes(archive) & FileAttributes.ReparsePoint) == 0 &&
        Hash(archive) == package.ArchiveHash,
        "Original immutable archive identity is not the published pin: " + package.Id);
    var copied = Path.Combine(copiedFeed, basename);
    File.Copy(archive, copied);
    Need(Hash(copied) == package.ArchiveHash,
        "Received immutable private archive does not match its original: " + package.Id);
    archiveCopies.Add(package.Id, (archive, copied, package.ArchiveHash));
}
Need(archiveCopies.Count == 3, "Received feed contains an unexpected package set.");

using (var r323 = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS323/attempt-001.json")))
using (var r324 = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS324/attempt-001.json")))
{
    var rs323 = r323.RootElement;
    var rs324 = r324.RootElement;
    var refused = rs323.GetProperty("priorInvocationFailures");
    Need(rs323.GetProperty("runId").GetString() == "RS323" &&
        rs323.GetProperty("outcome").GetString() == "succeeded" &&
        rs323.GetProperty("attempt").GetInt32() == 1 &&
        refused.GetProperty("count").GetInt32() == 1 &&
        refused.GetProperty("preparationFailureCount").GetInt32() == 1 &&
        refused.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
        !refused.GetProperty("firstPassSucceeded").GetBoolean() &&
        rs323.GetProperty("contribution").GetProperty("observed")
            .GetProperty("validations").EnumerateArray().Any(x =>
                x.GetProperty("id").GetString() ==
                "m8-rs323-live-locked-three-package-consumer-and-refusals") &&
        rs324.GetProperty("runId").GetString() == "RS324" &&
        rs324.GetProperty("outcome").GetString() == "succeeded" &&
        rs324.GetProperty("attempt").GetInt32() == 1 &&
        rs324.GetProperty("priorInvocationFailures")
            .GetProperty("firstPassSucceeded").GetBoolean() &&
        rs324.GetProperty("progress").GetProperty("completions").EnumerateArray()
            .Single().GetProperty("id").GetString() == "M8-W02-C01-T2-G",
        "The durable RS323/RS324 GREEN and pre-attempt failure lineage differs.");
}

ProcessResult Verify()
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workspace,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    info.ArgumentList.Add("run");
    info.ArgumentList.Add("--file");
    info.ArgumentList.Add(OriginalVerifier);
    info.ArgumentList.Add("--");
    info.ArgumentList.Add("--check");
    info.Environment["NUGET_LOCAL"] = copiedFeed;
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS325: Cannot start original physical verifier.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS325: Received-copy verification timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" +
        stderr.GetAwaiter().GetResult());
}
void Clean()
{
    var result = Verify();
    Need(result.ExitCode == 0 &&
        result.Output.Contains(
            "MATH-RS322-PHYSICAL-STANDARD-CLOSURE|archives=3|",
            StringComparison.Ordinal) &&
        result.Output.Contains("otherTenPackages=unqualified|",
            StringComparison.Ordinal) &&
        result.Output.Contains("guidanceUpgrade=false|PASS", StringComparison.Ordinal),
        "The original verifier rejected intact received Math inputs: " + result.Output);
}
Clean();

int refusals = 0;
void Reject(string label, string target, Action<string> mutate,
    string expectedReason)
{
    var pristine = File.ReadAllBytes(target);
    try
    {
        mutate(target);
        Need(Hash(target) != Convert.ToHexStringLower(SHA256.HashData(pristine)),
            "The injected negative control did not change its copied input: " + label);
        var result = Verify();
        Need(result.ExitCode != 0 &&
            result.Output.Contains("RS322: " + expectedReason, StringComparison.Ordinal),
            "Original physical verifier failed to refuse " + label + ": " + result.Output);
        refusals++;
        Console.WriteLine("MATH-RS325-COPIED-REFUSAL|" + label + "|PASS");
    }
    finally
    {
        File.WriteAllBytes(target, pristine);
    }
    Need(Hash(target) == Convert.ToHexStringLower(SHA256.HashData(pristine)),
        "Received input was not restored: " + label);
    Clean();
}
void ModifyJson(string path, Action<JsonObject> change)
{
    var node = JsonNode.Parse(File.ReadAllText(path))?.AsObject() ??
        throw new InvalidDataException("RS325: Invalid copied JSON input.");
    change(node);
    File.WriteAllText(path, node.ToJsonString(
        new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
JsonObject Standard(JsonObject root) =>
    root["packages"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => x["packageId"]!.GetValue<string>() == "dw.quantities.standard");

var standardCopy = archiveCopies["dw.quantities.standard"].Copy;
Reject("tampered-published-archive", standardCopy, path =>
{
    var bytes = File.ReadAllBytes(path);
    bytes[bytes.Length - 1] ^= 0x01;
    File.WriteAllBytes(path, bytes);
}, "The immutable NUGET_LOCAL archive is missing, redirected or altered: dw.quantities.standard");

Reject("forged-standard-version", Copied(Ledger),
    path => ModifyJson(path, j =>
        Standard(j)["version"] = "20260.20261103.9999"),
    "The archive/DLL fingerprint does not match the qualified RS254 authority: dw.quantities.standard");

Reject("false-rs254-firstpass", Copied(Historical254),
    path => ModifyJson(path, j =>
        j["priorInvocationFailures"]!["firstPassSucceeded"] = true),
    "Physical/history report or its true first-pass lineage differs: RS254");

Reject("erase-ten-unqualified-gaps", Copied(Ledger),
    path => ModifyJson(path, j =>
        j["limitations"]![0] = "All thirteen Math packages are certified."),
    "Bounded historical vs current physical acceptance limit was lost");

Need(refusals == 4, "Four actual copied-input refusals were required.");
foreach (var file in files)
    Need(Hash(file) == originals[file] && Hash(Copied(file)) == originals[file],
        "A canonical or copied Math file changed: " + file);
foreach (var (_, item) in archiveCopies)
    Need(Hash(item.Original) == item.Sha && Hash(item.Copy) == item.Sha,
        "An original or received NuGet archive was modified.");

Console.WriteLine("MATH-RS325-COPIED-STANDARD-VERIFY|evidenceFiles=10|archives=3|" +
    "cleanVerifier=PASS|archiveTamper=refused|versionForgery=refused|" +
    "historicalFirstPassForgery=refused|allThirteenOverclaim=refused|" +
    "cleanRestored=4|originalHashes=stable|otherTen=unqualified|" +
    "newNet11LockedRestore=false|productionMutation=false|PASS");

string Copied(string relative) => Path.Combine(workspace,
    relative.Replace('/', Path.DirectorySeparatorChar));
static string Hash(string path)
{
    using var file = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(file));
}
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidDataException("RS325: " + reason);
}
sealed record ProcessResult(int ExitCode, string Output);
