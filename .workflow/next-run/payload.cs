using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Root = "docs/discovery/qualification/";
const string Tests = Root + "RS133-neutral-consumers/";
const string Boundary = Root + "RS133-neutral-candidate-boundary.md";
const string Validator = Root + "ValidateRS133NeutralCandidate.cs";
const string Evidence = Root + "RS133-neutral-candidate-qualified.json";
const string Legacy = Tests + "legacy.csproj";
const string Neutral = Tests + "neutral.csproj";
const string Status = "rs133-neutral-1001-profile-incompatible-old-consumer-preserved";
const string PackageId = "dw.discovery";
const string Version = "20260.20261027.1001";
const string PackageHash = "fff0cd1aa9cf81528030f52de012e1aac1fa7a1b98da11b764096bf926f433ec";
const string DllHash = "70d38e80963378d8df5ea76fcb59a14e79d0c169c8f5926f4018628370b15064";
const string Feed = "I:/nuget.local";

var p = PayloadContext.Create();
var repository = p.RepositoryRoot;
var target = File.Exists(Path.Combine(repository, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS133", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS132", 1,
    "docs: qualify four compiled Math CLR Discovery signatures without skill",
    "0.1.59", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M7", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/discovery/qualification/RS131-portable-protocol-qualified.json",
    "/total_cases", JsonValue.Create(13));
p.Require.JsonValueEquals("docs/discovery/qualification/RS132-clr-seed-qualified.json",
    "/observed_static_bindings", JsonValue.Create(4));
p.Require.JsonValueEquals("docs/planning/proposals/math-agent-discovery-work-package.json",
    "/status", JsonValue.Create("draft-not-merged-into-canonical-backlog-or-native-DWF"));
p.Require.TextContains("docs/discovery/README.md", "No generated Math skill");
p.Require.TextContains("docs/coordination/requests/README.md", "have not been sent");

var destinations = new[]
{
    Boundary, Tests + "Directory.Packages.props", Tests + "legacy.csproj", Tests + "neutral.csproj",
    Tests + "LegacyConsumer.cs", Tests + "NeutralConsumer.cs", Validator
};
if (!target)
{
    foreach (var path in destinations.Append(Evidence))
        p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
    p.Require.JsonValueEquals(Evidence, "/neutral_math_profile_supported", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/neutral_migration_qualified", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/broker_ready", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/skill_generated", JsonValue.Create(false));
}

var hashes = new JsonObject();
foreach (var path in destinations)
{
    var change = p.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = change.AfterSha256 ??
        throw new InvalidOperationException("RS133 staged file SHA is missing: " + path);
}
p.Require.TextContains(Boundary, "## Controlled Windows/.NET 10 execution");
p.Require.TextContains(Boundary, "## Strict exit and nonclaims");
p.Require.TextContains(Tests + "LegacyConsumer.cs", "LEGACY-MATH-CONSUMER|PASS|4");
p.Require.TextContains(Tests + "NeutralConsumer.cs", "NEUTRAL-MATH-CONSUMER|EXPECTED-UNSUPPORTED|4");
p.Require.TextContains("Directory.Packages.props", "<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>");
p.Require.TextContains(Tests + "Directory.Packages.props",
    "<PackageVersion Include=\"dw.discovery\" Version=\"[20260.20261027.1001]\" />");

var seedCheck = Run(repository, "seed-check", "dotnet",
    "run", "--file", Validator, "--", "--check");
Need(seedCheck.Output.Contains("MATH-RS133:PORTABLE=13;REFLECTED=4;", StringComparison.Ordinal),
    "independent baseline checker rejected");
if (!target)
{
    Need(OperatingSystem.IsWindows() && RuntimeInformation.ProcessArchitecture == Architecture.X64,
        "current package qualification requires Windows/x64; other RIDs need an independent run");
    var archivePath = Path.Combine(Feed, "dw.discovery.20260.20261027.1001.nupkg");
    Need(File.Exists(archivePath), "published local immutable Discovery package is unavailable: " + archivePath);
    Need(new FileInfo(archivePath).Length == 48093,
        "published Discovery archive byte length drifted");
    var actualPackage = Hex(File.ReadAllBytes(archivePath));
    Need(actualPackage == PackageHash, "published Discovery archive SHA-256 drifted");
    string packagedDll;
    using (var zip = ZipFile.OpenRead(archivePath))
    {
        var dll = zip.GetEntry("lib/net10.0/dw.discovery.dll") ??
            throw new InvalidOperationException("Published neutral DLL is missing");
        using var stream = dll.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        packagedDll = Hex(memory.ToArray());
        Need(packagedDll == DllHash, "packaged Discovery DLL SHA-256 drifted");
        var nuspec = zip.GetEntry("dw.discovery.nuspec") ??
            throw new InvalidOperationException("Published package nuspec is missing");
        using var reader = nuspec.Open();
        var metadata = XDocument.Load(reader);
        var id = metadata.Descendants().Single(x => x.Name.LocalName == "id").Value;
        var version = metadata.Descendants().Single(x => x.Name.LocalName == "version").Value;
        Need(id == PackageId && version == Version &&
             !metadata.Descendants().Any(x => x.Name.LocalName == "dependency"),
             "published package identity or dependency closure drifted");
        Need(!zip.Entries.Any(x =>
            x.FullName.Contains("..", StringComparison.Ordinal) ||
            x.FullName.Contains('\\') ||
            x.FullName.StartsWith("/", StringComparison.Ordinal) ||
            x.FullName.EndsWith(".targets", StringComparison.OrdinalIgnoreCase) ||
            x.FullName.EndsWith(".props", StringComparison.OrdinalIgnoreCase) ||
            x.FullName.Contains("/analyzers/", StringComparison.OrdinalIgnoreCase) ||
            x.FullName.Contains("/tools/", StringComparison.OrdinalIgnoreCase)),
            "published archive contains an unreviewed execution or path surface");
    }

    var commands = new List<JsonNode>();
    var portable = Run(repository, "portable-regression", "dotnet",
        "run", "--file", Root + "ValidatePortableProtocol.cs");
    commands.Add(portable.Receipt);
    var prior = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository,
        Root + "RS131-portable-protocol-qualified.json")));
    using (prior)
    {
        var cases = prior.RootElement.GetProperty("case_results").EnumerateArray().ToArray();
        Need(cases.Length == 13, "RS131 portable receipt is not complete");
        var caseRows = new List<JsonNode>();
        foreach (var item in cases)
        {
            var id = item.GetProperty("name").GetString()!;
            var accepted = item.GetProperty("expected_accept").GetBoolean();
            var verdict = accepted ? "accept" : "reject";
            Need(portable.Output.Contains(
                "CASE " + id + ": expected=" + verdict + " observed=" + verdict + " PASS",
                StringComparison.Ordinal), "portable as-run verdict drifted: " + id);
            caseRows.Add(new JsonObject
            {
                ["name"] = id, ["expected_accept"] = accepted, ["observed_accept"] = accepted
            });
        }

        commands.Add(Run(repository, "legacy-restore", "dotnet",
            "restore", Legacy, "--source", Feed, "-p:NuGetAudit=false").Receipt);
        commands.Add(Run(repository, "legacy-build", "dotnet",
            "build", Legacy, "-c", "Release", "--no-restore", "-p:NuGetAudit=false").Receipt);
        var oldRun = Run(repository, "legacy-direct", "dotnet",
            "run", "--project", Legacy, "-c", "Release", "--no-build", "--no-restore");
        commands.Add(oldRun.Receipt);

        var ids = new[]
        {
            "math.quantity.exact-pipeline.evaluate", "math.replay.try-evaluate",
            "math.ir.canonical.encode", "math.ir.hash.semantic"
        };
        var oldRows = new List<JsonNode>();
        foreach (var id in ids)
        {
            Need(oldRun.Output.Contains("LEGACY-CALL|" + id + "|PASS", StringComparison.Ordinal),
                "independent direct Math consumer failed: " + id);
            oldRows.Add(new JsonObject { ["id"] = id, ["direct_invocation"] = true });
        }
        Need(oldRun.Output.Contains("LEGACY-MATH-CONSUMER|PASS|4", StringComparison.Ordinal),
            "legacy four-method consumer incomplete");

        commands.Add(Run(repository, "neutral-restore", "dotnet",
            "restore", Neutral, "--source", Feed, "-p:NuGetAudit=false").Receipt);
        commands.Add(Run(repository, "neutral-build", "dotnet",
            "build", Neutral, "-c", "Release", "--no-restore", "-p:NuGetAudit=false").Receipt);
        var candidate = Run(repository, "neutral-observe", "dotnet",
            "run", "--project", Neutral, "-c", "Release", "--no-build", "--no-restore");
        commands.Add(candidate.Receipt);
        Need(candidate.Output.Contains(
            "NEUTRAL-IDENTITY|dw.discovery|20260.20261027.1001|PASS",
            StringComparison.Ordinal) &&
            candidate.Output.Contains("NEUTRAL-PRIMITIVE-CONTROL|PASS", StringComparison.Ordinal) &&
            candidate.Output.Contains("NEUTRAL-MATH-CONSUMER|EXPECTED-UNSUPPORTED|4",
                StringComparison.Ordinal),
            "independent neutral package consumer identity or positive control failed");
        var negativeRows = new List<JsonNode>();
        for (var i = 0; i < ids.Length; i++)
        {
            var code = i == 0 ? "sidecar-method-unresolved" : "unsupported-signature";
            Need(candidate.Output.Contains(
                "NEUTRAL-UNSUPPORTED|" + ids[i] + "|" + code, StringComparison.Ordinal),
                "unexpected neutral Math candidate exposure: " + ids[i]);
            negativeRows.Add(new JsonObject { ["id"] = ids[i], ["reason"] = code });
        }

        var back = Run(repository, "legacy-fallback", "dotnet",
            "run", "--project", Legacy, "-c", "Release", "--no-build", "--no-restore");
        commands.Add(back.Receipt);
        Need(String.Equals(oldRun.Output.Trim(), back.Output.Trim(), StringComparison.Ordinal),
            "old direct consumer output drifted after the neutral package consumer");

        var reflected = Run(repository, "clr-regression", "dotnet",
            "run", "--file", Root + "ValidateClrSeedReflection.cs", "--", "--reflect");
        commands.Add(reflected.Receipt);
        var original = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository,
            Root + "RS132-clr-seed-qualified.json")));
        var reflectedRows = new List<JsonNode>();
        using (original)
        {
            foreach (var item in original.RootElement.GetProperty("observed_bindings")
                         .EnumerateArray())
            {
                var id = item.GetProperty("capability_id").GetString()!;
                var digest = item.GetProperty("signature_sha256").GetString()!;
                var assembly = item.GetProperty("assembly").GetString()!;
                Need(reflected.Output.Contains(
                    "RS132-CLR|" + id + "|" + digest + "|" + assembly,
                    StringComparison.Ordinal), "original CLR signature was not reobserved: " + id);
                reflectedRows.Add(new JsonObject {
                    ["id"] = id, ["assembly"] = assembly, ["signature_sha256"] = digest
                });
            }
        }
        var sdk = Run(repository, "sdk", "dotnet", "--version");
        commands.Add(sdk.Receipt);
        Need(commands.Count == 10, "expected exactly ten real as-run command receipts");

        var receipt = new JsonObject
        {
            ["schema_version"] = 1,
            ["id"] = "RS133-neutral-candidate-qualified",
            ["status"] = Status,
            ["question"] = "Do four previously qualified direct Math CLR methods work as a fallback, and can the immutable neutral Discovery package currently inspect them as supported candidates?",
            ["observation"] = "Two independent Math-owned C# consumers were restored and compiled from real Math projects plus exact immutable local Discovery package. Four direct calls succeeded; four neutral opt-in sidecars were correctly refused by the published neutral CLR profile. The unchanged direct consumer succeeded again after the neutral test.",
            ["decision"] = "Current neutral package is distribution-identifiable but incompatible with the four original Math candidate shapes. Retain existing direct Math consumers and block neutral migration, skill/agent/host exposure and package promotion.",
            ["owner_request_id"] = "RS142-DISC-029",
            ["request_source_commit"] = "a780d0816dedfdaa5636dbfe516d6a3cbe0058cc",
            ["neutral_publisher_source_commit"] = "47055b49cfdfca511f4f4684b239d7118d3821cc",
            ["package_id"] = PackageId, ["package_version"] = Version,
            ["package_feed"] = Feed, ["archive_sha256"] = actualPackage,
            ["packaged_dll_sha256"] = packagedDll, ["loaded_dll_sha256"] = DllHash,
            ["clr_assembly_version"] = "1.0.0.0",
            ["portable_schema_sha256"] = "678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170",
            ["portable_fixtures_sha256"] = "5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e",
            ["previous_portable_receipt"] = Root + "RS131-portable-protocol-qualified.json",
            ["previous_clr_receipt"] = Root + "RS132-clr-seed-qualified.json",
            ["portable_cases"] = p.Json.Array(caseRows.ToArray()),
            ["legacy_operations"] = p.Json.Array(oldRows.ToArray()),
            ["neutral_refusals"] = p.Json.Array(negativeRows.ToArray()),
            ["reflected_bindings"] = p.Json.Array(reflectedRows.ToArray()),
            ["command_receipts"] = p.Json.Array(commands.ToArray()),
            ["source_files"] = hashes,
            ["sdk_version"] = sdk.Output.Trim(),
            ["observed_runtime_identifier"] = RuntimeInformation.RuntimeIdentifier,
            ["native_nodes_retained"] = 541,
            ["direct_legacy_invocation_executed"] = true,
            ["neutral_consumer_executed"] = true,
            ["old_consumer_fallback_rerun_verified"] = true,
            ["neutral_math_profile_supported"] = false,
            ["neutral_migration_qualified"] = false,
            ["installed_package_rollback_qualified"] = false,
            ["broker_ready"] = false,
            ["skill_generated"] = false,
            ["new_package_published"] = false,
            ["external_requests_transmitted"] = false,
            ["next_action"] = "Neutral owner must support explicit complex DTO/generic/overload binding under an independently released and qualified package; Math must adopt only with a new DWF scope and independent rollback evidence.",
        };
        p.Files.WriteComplete(Evidence,
            receipt.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
    }
}
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
var checkedTarget = Run(repository, "seed-target", "dotnet",
    "run", "--file", Validator, "--", "--check");
Need(checkedTarget.Output.Contains(
    "MATH-RS133:NEUTRAL-MIGRATION=false;FALLBACK-DIRECT=true;",
    StringComparison.Ordinal), "independent target checker rejected");
return p.Complete();

static void Need(bool value, string reason)
{
    if (!value) throw new InvalidOperationException("RS133 neutral evidence: " + reason);
}
static string Hex(byte[] bytes) =>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static (JsonObject Receipt, string Output) Run(
    string root, string id, string executable, params string[] args)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = executable, WorkingDirectory = root,
            UseShellExecute = false, RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["NUGET_PACKAGES"] =
        Path.Combine(root, "build", "artifacts", "nuget", "rs133");
    foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
    var clock = Stopwatch.StartNew();
    if (!process.Start()) throw new InvalidOperationException("Cannot start RS133 command: " + id);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS133 command timed out: " + id);
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    clock.Stop();
    if (process.ExitCode != 0)
        throw new InvalidOperationException("RS133 " + id + " failed exit=" +
            process.ExitCode + "\n" + output[..Math.Min(output.Length, 4000)]);
    Console.WriteLine("RS133 " + id + " exit=0 duration_ms=" + clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"] = id, ["exit_code"] = process.ExitCode,
        ["command"] = executable + " " + string.Join(" ", args),
        ["elapsed_ms"] = clock.ElapsedMilliseconds,
        ["output_excerpt"] = output[..Math.Min(output.Length, 1200)]
    }, output);
}
