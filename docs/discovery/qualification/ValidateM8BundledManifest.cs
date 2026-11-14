#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json;

const string SourcePath = "docs/discovery/qualification/m8-discovery-manifest.json";
const string BundlePath = ".agents/skills/dw-tools-math/references/manifest.json";
var source = File.ReadAllText(SourcePath).Replace("\r\n", "\n", StringComparison.Ordinal);
var bundle = File.ReadAllText(BundlePath).Replace("\r\n", "\n", StringComparison.Ordinal);
Validate(source, source, "canonical source");
Validate(bundle, source, "bundled projection");
var refusals = 0;
Reject("changed-content", bundle.Replace("\"owner\": \"math\"", "\"owner\": \"other\"", StringComparison.Ordinal));
Reject("broker-promotion", bundle.Replace("\"exposure\": \"AgentCallable\"", "\"exposure\": \"BrokerReady\"", StringComparison.Ordinal));
Reject("runtime-authorization", bundle.Replace("\"authorization\": \"runtime-only\"", "\"authorization\": \"broker-approved\"", StringComparison.Ordinal));
Reject("broker-candidate", bundle.Replace("\"brokerCandidateIds\": []", "\"brokerCandidateIds\": [\"unqualified\"]", StringComparison.Ordinal));
Reject("selected-id-drift", bundle.Replace("math.ir.hash.semantic", "math.ir.hash.algebraic", StringComparison.Ordinal));
Reject("byte-only-drift", bundle.Replace("\"owner\": \"math\",", "\"owner\":  \"math\",", StringComparison.Ordinal));
Require(refusals == 6, "Expected six independent refusals.");
Console.WriteLine("MATH-RS202-BUNDLED-MANIFEST|positive=2|negative=6|capabilities=4|brokerReady=0|PASS");

void Reject(string label, string altered)
{
    Require(!string.Equals(altered, bundle, StringComparison.Ordinal), "Mutation absent: " + label);
    try
    {
        Validate(altered, source, label);
    }
    catch (InvalidDataException)
    {
        refusals++;
        Console.WriteLine("REFUSAL " + label + " PASS");
        return;
    }
    throw new InvalidDataException("RS202 accepted altered manifest: " + label);
}

void Validate(string candidate, string canonical, string label)
{
    using var document = JsonDocument.Parse(candidate);
    var root = document.RootElement;
    Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
        root.GetProperty("protocolId").GetString() == "dw.discovery" &&
        root.GetProperty("owner").GetString() == "math", label + ": source identity");
    Require(root.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
        label + ": broker candidates");
    var caps = root.GetProperty("capabilities").EnumerateArray().ToArray();
    string[] ids = [
        "math.quantity.exact-pipeline.evaluate",
        "math.replay.try-evaluate",
        "math.ir.canonical.encode",
        "math.ir.hash.semantic"
    ];
    Require(caps.Length == ids.Length, label + ": selected count");
    for (var i = 0; i < caps.Length; i++)
    {
        var cap = caps[i];
        Require(cap.GetProperty("capabilityId").GetString() == ids[i],
            label + ": selected id " + i);
        Require(cap.GetProperty("exposure").GetString() == "AgentCallable" &&
            cap.GetProperty("execution").GetProperty("authorization").GetString() == "runtime-only" &&
            !cap.TryGetProperty("brokerContract", out _),
            label + ": execution authorization " + i);
    }
    Require(string.Equals(candidate, canonical, StringComparison.Ordinal),
        label + ": canonical LF byte drift");
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS202: " + message);
}
