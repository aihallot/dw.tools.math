#:property NuGetAudit=false
#:property PublishAot=false

using System.Text;
using System.Text.Json;

const string Input = "docs/discovery/qualification/m8-discovery-manifest.json";
const string Output = ".agents/skills/dw-tools-math/SKILL.md";
if (args.Length != 1 || (args[0] != "--write" && args[0] != "--check"))
    throw new ArgumentException("Expected --write or --check.");

using var doc = JsonDocument.Parse(File.ReadAllText(Input));
var root = doc.RootElement;
string Read(JsonElement e, string key) => e.GetProperty(key).GetString()
    ?? throw new InvalidDataException("Missing " + key);
string Flat(string value) => value.Replace('\r', ' ').Replace('\n', ' ');
void Require(bool value, string message)
{
    if (!value) throw new InvalidDataException("Math skill: " + message);
}
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    Read(root, "protocolId") == "dw.discovery" && Read(root, "owner") == "math",
    "Unqualified protocol or owner.");
var caps = root.GetProperty("capabilities");
Require(caps.GetArrayLength() == 4 &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
    "Selected capability cardinality or broker candidate boundary.");
var output = new StringBuilder();
void Line(string value = "") => output.Append(value).Append('\n');
Line("---");
Line("name: dw-tools-math");
Line("description: Source-qualified Math Discovery CLR capability guide; not a broker or execution authorization.");
Line("---");
Line("# Math Discovery");
Line();
Line("Protocol: `" + Read(root, "protocolId") + "/" + Read(root, "protocolVersion") +
    "`. Skill bundle: `" + Read(root, "skillBundleVersion") + "`.");
Line("Exposure: `AgentCallable` only. This file grants no runtime permission, broker readiness, package installation, or AURA adoption.");
Line();
Line("## Qualified callable surfaces");
Line();
var ids = new HashSet<string>(StringComparer.Ordinal);
foreach (var cap in caps.EnumerateArray())
{
    var id = Read(cap, "capabilityId");
    Require(ids.Add(id) && Read(cap, "exposure") == "AgentCallable" &&
        Read(cap.GetProperty("execution"), "authorization") == "runtime-only" &&
        !cap.TryGetProperty("brokerContract", out _), "Selection or authorization drift.");
    var binding = cap.GetProperty("binding");
    var semantics = cap.GetProperty("semantics");
    var result = cap.GetProperty("result");
    Line("### `" + Flat(id) + "`");
    Line();
    Line("- CLR: `" + Flat(Read(binding, "type")) + "." +
        Flat(Read(binding, "member")) + "` (`" + Flat(Read(binding, "assembly")) + "`).");
    Line("- Signature: `" + Flat(Read(binding, "canonicalSignature")) + "`.");
    Line("- Fingerprint (SHA-256): `" + Flat(Read(binding, "fingerprint")) + "`.");
    Line("- Result: `" + Flat(Read(result, "type")) + "` — " + Flat(Read(result, "interpretation")));
    Line("- Numeric model: " + Flat(Read(semantics, "numericModel")) + ".");
    Line("- Limits: " + Flat(Read(semantics, "limits")) + ".");
    Line("- Guarantee: " + Flat(Read(semantics, "guarantee")) + ".");
    Line("- Source example: `" + Flat(Read(cap.GetProperty("example"), "file")) + "`.");
    Line("- Authorization: `" + Flat(Read(cap.GetProperty("execution"), "authorization")) +
        "`; resolve permissions in the caller.");
    Line();
}
Line("## Boundaries");
Line();
Line("Only explicitly typed, qualified, bounded Math inputs are supported. Respect each CLR method's failure and cancellation contract. Structural IR hashes do not imply algebraic equivalence. Do not expose any capability as BrokerReady.");
Line();
Line("## Evidence");
Line();
Line("Generated from `docs/discovery/qualification/m8-discovery-manifest.json`; qualified by RS186 and RS187. Recreate with `dotnet run --file docs/discovery/qualification/GenerateM8MathSkill.cs -- --write`; verify drift with `--check`.");
var expected = output.ToString();
if (args[0] == "--write")
{
    Directory.CreateDirectory(Path.GetDirectoryName(Output)!);
    File.WriteAllText(Output, expected, new UTF8Encoding(false));
}
else
    Require(File.Exists(Output) && File.ReadAllText(Output) == expected,
        "Generated skill is absent or stale.");
Console.WriteLine("MATH-RS188-SKILL|capabilities=4|brokerReady=0|mode=" + args[0]);
