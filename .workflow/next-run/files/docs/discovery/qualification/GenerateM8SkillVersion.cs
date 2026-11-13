#:property NuGetAudit=false
#:property PublishAot=false

using System.Text;
using System.Text.Json;

const string Input = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Output = ".agents/skills/dw-tools-math/references/skill-version.json";
if (args.Length != 1 || args[0] is not ("--write" or "--check"))
    throw new ArgumentException("Use --write or --check.");

using var doc = JsonDocument.Parse(File.ReadAllText(Input));
var root = doc.RootElement;
string Read(JsonElement value, string key) => value.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Missing " + key);
void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("M8 version: " + reason);
}
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    Read(root, "protocolId") == "dw.discovery" &&
    Read(root, "owner") == "math", "Qualified protocol");
var capabilities = root.GetProperty("capabilities").EnumerateArray().ToArray();
Require(capabilities.Length == 4 &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0, "Selected surface");
var ids = new HashSet<string>(StringComparer.Ordinal);
var packages = new SortedDictionary<string, string>(StringComparer.Ordinal);
foreach (var capability in capabilities)
{
    Require(ids.Add(Read(capability, "capabilityId")) &&
        Read(capability, "exposure") == "AgentCallable" &&
        Read(capability.GetProperty("execution"), "authorization") == "runtime-only" &&
        !capability.TryGetProperty("brokerContract", out _), "Unqualified capability");
    var id = Read(capability, "package");
    var version = Read(capability, "packageVersion");
    if (packages.TryGetValue(id, out var previous))
        Require(previous == version, "Conflicting package version");
    else
        packages.Add(id, version);
}
Require(packages.Count == 2, "Qualified package cardinality");
var output = new StringBuilder();
void Line(string value = "") => output.Append(value).Append('\n');
string Quote(string value) => JsonSerializer.Serialize(value);
Line("{");
Line("  \"schemaVersion\": 1,");
Line("  \"owner\": " + Quote(Read(root, "owner")) + ",");
Line("  \"protocolId\": " + Quote(Read(root, "protocolId")) + ",");
Line("  \"protocolVersion\": " + Quote(Read(root, "protocolVersion")) + ",");
Line("  \"protocolSchemaVersion\": " + root.GetProperty("schemaVersion").GetInt32() + ",");
Line("  \"libraryRelease\": " + Quote(Read(root, "libraryRelease")) + ",");
Line("  \"manifestGeneratorVersion\": " + Quote(Read(root, "generatorVersion")) + ",");
Line("  \"skillBundleVersion\": " + Quote(Read(root, "skillBundleVersion")) + ",");
Line("  \"capabilityCount\": " + capabilities.Length + ",");
Line("  \"brokerReadyCount\": 0,");
Line("  \"authorization\": \"runtime-only\",");
Line("  \"sourceManifest\": " + Quote(Input) + ",");
Line("  \"packages\": [");
var packageList = packages.ToArray();
for (var index = 0; index < packageList.Length; index++)
{
    var package = packageList[index];
    Line("    {");
    Line("      \"id\": " + Quote(package.Key) + ",");
    Line("      \"version\": " + Quote(package.Value));
    Line(index == packageList.Length - 1 ? "    }" : "    },");
}
Line("  ]");
Line("}");
var text = output.ToString();
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(Output)!);
    File.WriteAllText(Output, text, new UTF8Encoding(false));
}
else
    Require(File.Exists(Output) && File.ReadAllText(Output) == text,
        "Skill version file missing or drifted");
Console.WriteLine("MATH-RS193-SKILL-VERSION|capabilities=4|packages=2|brokerReady=0|mode=" + args[0]);
