#:property NuGetAudit=false
#:property PublishAot=false

using System.Text;
using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string ReferencePath = ".agents/skills/dw-tools-math/references/selected-capabilities.md";
if (args.Length != 1 || args[0] is not ("--write" or "--check"))
    throw new ArgumentException("Use --write or --check.");
using var doc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var root = doc.RootElement;
var caps = root.GetProperty("capabilities").EnumerateArray().ToArray();
void Require(bool ok, string message)
{
    if (!ok) throw new InvalidDataException("RS190: " + message);
}
string Read(JsonElement value, string key) => value.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Missing " + key);
string Flat(string value) => value.Replace('\r', ' ').Replace('\n', ' ');
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    Read(root, "protocolId") == "dw.discovery" && Read(root, "owner") == "math",
    "Qualified manifest identity");
Require(caps.Length == 4 && root.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
    "Selected capability count and broker absence");
var output = new StringBuilder();
void Line(string value = "") => output.Append(value).Append('\n');
Line("# Math Discovery — selected capability reference");
Line();
Line("Generated from `docs/discovery/qualification/m8-discovery-manifest.json` (schema " +
    root.GetProperty("schemaVersion").GetInt32() + ").");
Line();
Line("This is source-project CLR evidence, not a package-installed or broker-authorized consumer. No public distribution or AURA adoption is implied.");
Line();
var ids = new HashSet<string>(StringComparer.Ordinal);
foreach (var cap in caps)
{
    string id = Read(cap, "capabilityId");
    var binding = cap.GetProperty("binding");
    var result = cap.GetProperty("result");
    var semantics = cap.GetProperty("semantics");
    var execution = cap.GetProperty("execution");
    Require(ids.Add(id) && Read(cap, "exposure") == "AgentCallable" &&
        Read(execution, "authorization") == "runtime-only" &&
        !cap.TryGetProperty("brokerContract", out _), "Unqualified exposure or duplicate");
    Line("## `" + id + "`");
    Line();
    Line("- Package: `" + Read(cap, "package") + "` (`" +
        Read(cap, "packageVersion") + "`).");
    Line("- CLR binding: `" + Read(binding, "type") + "." + Read(binding, "member") + "`.");
    Line("- Signature SHA-256: `" + Read(binding, "fingerprint") + "`.");
    Line("- Result: `" + Read(result, "type") + "`.");
    Line("- Interpretation: " + Flat(Read(result, "interpretation")));
    Line("- Numerical limits: " + Flat(Read(semantics, "limits")));
    Line("- Failure policy: " + Flat(Read(execution, "failurePolicy")));
    Line("- Evidence example: `" + Read(cap.GetProperty("example"), "file") + "`.");
    Line("- Exposure: `" + Read(cap, "exposure") +
        "`; authorization: `" + Read(execution, "authorization") + "`.");
    Line("- Parameters:");
    foreach (var parameter in cap.GetProperty("parameters").EnumerateArray())
        Line("  - `" + Read(parameter, "name") + "`: `" +
            Read(parameter, "type") + "`" +
            (parameter.GetProperty("optional").GetBoolean() ? " (optional)" : " (required)") + ".");
    Line();
}
Line("No item is BrokerReady; invocation must be authorized independently by the runtime.");
var expected = output.ToString();
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(ReferencePath)!);
    File.WriteAllText(ReferencePath, expected, new UTF8Encoding(false));
}
else
    Require(File.Exists(ReferencePath) && File.ReadAllText(ReferencePath) == expected,
        "Generated reference is missing or stale");
const string IndexPath = ".agents/skills/dw-tools-math/references/index.md";
var navigation = new StringBuilder();
void Nav(string value = "") => navigation.Append(value).Append('\n');
Nav("# Math Discovery — navigation");
Nav();
Nav("Selected Math capability metadata from `docs/discovery/qualification/m8-discovery-manifest.json`.");
Nav();
Nav("- [Capability signatures, parameters and numerical limits](selected-capabilities.md)");
Nav("- [Separated protocol, generator, bundle and package versions](skill-version.json)");
Nav();
Nav("The entries below are intentionally selected `AgentCallable` surfaces, not broker authorization.");
string? family = null;
foreach (var cap in caps)
{
    var currentFamily = Read(cap, "family");
    if (family != currentFamily)
    {
        family = currentFamily;
        Nav();
        Nav("## " + family);
    }
    Nav("- `" + Read(cap, "capabilityId") + "` — " +
        Flat(Read(cap.GetProperty("result"), "interpretation")));
}
Nav();
Nav("Inspect without invoking: `dotnet run --file docs/discovery/qualification/DescribeM8Discovery.cs -- --list` or `--explain <selected-capability-id>`.");
Nav();
Nav("Runtime permission and broker routing are external; none of these capabilities is `BrokerReady`.");
var expectedIndex = navigation.ToString();
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(IndexPath)!);
    File.WriteAllText(IndexPath, expectedIndex, new UTF8Encoding(false));
}
else
    Require(File.Exists(IndexPath) && File.ReadAllText(IndexPath) == expectedIndex,
        "Generated navigation index is missing or stale");
Console.WriteLine("MATH-RS199-NAVIGATION|capabilities=4|families=3|brokerReady=0|mode=" + args[0]);
