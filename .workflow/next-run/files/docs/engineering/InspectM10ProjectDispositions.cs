#:property PublishAot=false
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--write" or "--check")) throw new ArgumentException("Expected --write or --check.");
const string Output = "docs/engineering/m10-project-dispositions-rs402.json";
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 dispositions: " + message); }
var red = JsonNode.Parse(File.ReadAllText("docs/engineering/m10-cutover-readiness-red-rs400.json"))!.AsObject();
var entries = red["projects"]!.AsArray().Select(x => x!.AsObject()).ToArray();
var paths = entries.Select(x => (string)x["path"]!).ToArray();
var observed = new[] { "src", "tests", "tools", "samples", "docs" }
    .SelectMany(root => Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories))
    .Select(path => path.Replace('\\', '/'))
    .Where(path => !path.Split('/').Any(part => part is "bin" or "obj" or "build" or ".git"))
    .OrderBy(path => path, StringComparer.Ordinal).ToArray();
Need(paths.Length == 123 && paths.SequenceEqual(observed, StringComparer.Ordinal), "Project inventory drift.");
var rows = new List<object>();
var counts = new SortedDictionary<string, int>(StringComparer.Ordinal);
foreach (var entry in entries)
{
    var path = (string)entry["path"]!;
    var role = (string)entry["role"]!;
    var doc = XDocument.Load(path);
    string[] Properties(string name) => doc.Descendants().Where(x => x.Name.LocalName == name)
        .Select(x => x.Value.Trim()).ToArray();
    string[] References(string name) => doc.Descendants().Where(x => x.Name.LocalName == name)
        .Select(x => (x.Attribute("Include")?.Value ?? x.Attribute("Update")?.Value ?? "") +
            (x.Attribute("Version") is { } version ? "@" + version.Value : ""))
        .OrderBy(x => x, StringComparer.Ordinal).ToArray();
    var sourceRefs = References("ProjectReference");
    var packageRefs = References("PackageReference");
    var frameworks = Properties("TargetFramework").Concat(Properties("TargetFrameworks")).ToArray();
    var language = Properties("LangVersion");
    var directory = Path.GetDirectoryName(path)!;
    var lockFile = Path.Combine(directory, "packages.lock.json");
    string disposition;
    string reason;
    if (role is "production" or "solution-test")
    {
        disposition = "migrate-solution";
        reason = "Mandatory maintained solution project; qualify actual net11 graph and lock closure.";
    }
    else if (path == "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj")
    {
        disposition = "migrate-maintained-tool";
        reason = "Current source-linked discovery binding generator; qualify project references and explicit target override.";
        Need(sourceRefs.Length > 0, "Discovery tool source references disappeared.");
    }
    else if (path == "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj")
    {
        disposition = "migrate-maintained-tool";
        reason = "Reusable package preflight inspector; qualify its package dependency and current SDK assumptions without publishing.";
    }
    else if (role == "tool-review")
    {
        Need(path.StartsWith("tools/dw.tools.math.nuget-", StringComparison.Ordinal), "Unexpected tool requires individual review: " + path);
        disposition = "preserve-versioned-tool";
        reason = "Version-specific candidate, release, rehearsal or readback recipe; reproduce from its historical commit with original SDK and feeds, not by running publication against migrated source.";
    }
    else if (role == "consumer-preserve" && sourceRefs.Length > 0)
    {
        disposition = "preserve-source-fixture";
        reason = "Source-linked acceptance fixture; preserve original reproduction using its recorded source commit. A separate current net11 consumer qualification is required.";
    }
    else if (role == "consumer-preserve")
    {
        disposition = "preserve-binary-fixture";
        reason = "Historical package consumer; preserve its package versions and original effective target using its historical checkout.";
    }
    else if (role is "provider-preserve" or "historical-preserve" or "sample-preserve")
    {
        disposition = "preserve-historical-fixture";
        reason = "Provider, document or sample reproduction fixture; keep original SDK and dependencies using the historical checkout. Requalify current usage separately if needed.";
    }
    else throw new InvalidDataException("Unclassified role: " + role);
    counts[disposition] = counts.GetValueOrDefault(disposition) + 1;
    rows.Add(new { path, role, disposition, reason, explicitFrameworks = frameworks, explicitLanguage = language,
        effectiveBaselineFramework = frameworks.Length == 0 ? "net10.0 (central default)" : string.Join(";", frameworks),
        effectiveBaselineLanguage = language.Length == 0 ? "14.0 (central default)" : string.Join(";", language),
        projectReferences = sourceRefs, packageReferences = packageRefs,
        lockFilePresent = File.Exists(lockFile), restoreLockPolicy = Properties("RestorePackagesWithLockFile"),
        centralPackagePolicy = Properties("ManagePackageVersionsCentrally"),
        restoreSources = Properties("RestoreSources"), restorePackagesPath = Properties("RestorePackagesPath") });
}
Need(counts.GetValueOrDefault("migrate-solution") == 24 && counts.GetValueOrDefault("migrate-maintained-tool") == 2 &&
    counts.GetValueOrDefault("preserve-versioned-tool") == 38 && counts.Values.Sum() == 123, "Disposition coverage differs.");
var serialized = JsonSerializer.Serialize(new { schemaVersion = 1, sourceCommit = "a62933f1f666961600ed2527229429489d0f0423",
    status = "proposed-dispositions-not-cutover-qualified", counts, projects = rows,
    limits = new[] { "Historical checkouts are required for preserved fixtures whose inherited defaults would otherwise change.",
        "Individual target compatibility, isolated restore, fragile tests and actual rollback are not established by this inventory.",
        "No publication is authorized." } }, new JsonSerializerOptions { WriteIndented = true }) + "\n";
if (args[0] == "--write")
    File.WriteAllText(Output, serialized.Replace("\r\n", "\n", StringComparison.Ordinal));
else
{
    var actual = JsonNode.Parse(File.ReadAllText(Output));
    var expected = JsonNode.Parse(serialized);
    if (!JsonNode.DeepEquals(actual, expected))
    {
        var actualRows = actual?["projects"] as JsonArray;
        var expectedRows = expected?["projects"] as JsonArray;
        if (actualRows is not null && expectedRows is not null && actualRows.Count == expectedRows.Count)
        {
            for (var i = 0; i < expectedRows.Count; i++)
            {
                if (JsonNode.DeepEquals(actualRows[i], expectedRows[i])) continue;
                var expectedRow = expectedRows[i]!.AsObject();
                var actualRow = actualRows[i] as JsonObject;
                var fields = expectedRow.Select(x => x.Key)
                    .Concat(actualRow?.Select(x => x.Key) ?? Enumerable.Empty<string>())
                    .Distinct(StringComparer.Ordinal)
                    .Where(key => actualRow is null || !actualRow.ContainsKey(key) || !expectedRow.ContainsKey(key) ||
                        !JsonNode.DeepEquals(actualRow[key], expectedRow[key]));
                throw new InvalidDataException("M10 dispositions: Project ledger differs at row " + i +
                    ", path=" + (string?)expectedRow["path"] + ", fields=" + string.Join(",", fields));
            }
        }
        throw new InvalidDataException("M10 dispositions: Ledger envelope, counts or project roster differs.");
    }
}
Console.WriteLine("MATH-RS402-DISPOSITIONS|projects=123|solution=24|maintainedTools=2|versionedTools=38|qualification=false|PASS");
