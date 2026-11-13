#:property NuGetAudit=false
#:property PublishAot=false

using System.Text;
using System.Text.Json;

const string ManifestPath = "docs/discovery/qualification/m8-discovery-manifest.json";
if (args.Length is < 1 or > 2)
    throw new ArgumentException("Use --list, --explain <selected-capability-id>, or --self-test.");
using var document = JsonDocument.Parse(File.ReadAllText(ManifestPath));
var root = document.RootElement;
string Text(JsonElement value, string name) => value.GetProperty(name).GetString()
    ?? throw new InvalidDataException("Missing " + name);
void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS192: " + reason);
}
Require(root.GetProperty("schemaVersion").GetInt32() == 2 &&
    Text(root, "protocolId") == "dw.discovery" && Text(root, "owner") == "math" &&
    root.GetProperty("brokerCandidateIds").GetArrayLength() == 0,
    "Unexpected protocol or broker candidates.");
var capabilities = root.GetProperty("capabilities").EnumerateArray().ToArray();
Require(capabilities.Length == 4, "Exactly four selected capabilities expected.");
var selected = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
foreach (var cap in capabilities)
{
    var id = Text(cap, "capabilityId");
    Require(id.StartsWith("math.", StringComparison.Ordinal) &&
        selected.TryAdd(id, cap) &&
        Text(cap, "exposure") == "AgentCallable" &&
        Text(cap.GetProperty("execution"), "authorization") == "runtime-only" &&
        !cap.TryGetProperty("brokerContract", out _),
        "Unqualified capability, duplicate or authorization.");
}

string List()
{
    var output = new StringBuilder();
    output.AppendLine("MATH-DISCOVERY-LIST|protocol=dw.discovery/1.0.0-draft.1|count=4|brokerReady=0");
    foreach (var cap in capabilities)
    {
        var binding = cap.GetProperty("binding");
        output.Append(Text(cap, "capabilityId")).Append('|')
            .Append(Text(binding, "type")).Append('.')
            .Append(Text(binding, "member")).AppendLine("|AgentCallable|runtime-only");
    }
    return output.ToString().Replace("\r\n", "\n", StringComparison.Ordinal);
}

string Explain(string id)
{
    if (!selected.TryGetValue(id, out var cap))
        throw new InvalidDataException("RS192: unknown or unselected Math capability.");
    var binding = cap.GetProperty("binding");
    var semantics = cap.GetProperty("semantics");
    var result = cap.GetProperty("result");
    var output = new StringBuilder();
    output.Append("MATH-DISCOVERY-EXPLAIN|").AppendLine(id);
    output.Append("assembly=").AppendLine(Text(binding, "assembly"));
    output.Append("type=").AppendLine(Text(binding, "type"));
    output.Append("member=").AppendLine(Text(binding, "member"));
    output.Append("signature=").AppendLine(Text(binding, "canonicalSignature"));
    output.Append("fingerprint=").AppendLine(Text(binding, "fingerprint"));
    output.Append("result=").AppendLine(Text(result, "type"));
    output.Append("numericModel=").AppendLine(Text(semantics, "numericModel"));
    output.Append("limits=").AppendLine(Text(semantics, "limits"));
    output.AppendLine("exposure=AgentCallable");
    output.AppendLine("authorization=runtime-only");
    output.AppendLine("brokerReady=false");
    foreach (var parameter in cap.GetProperty("parameters").EnumerateArray())
    {
        output.Append("parameter=").Append(Text(parameter, "name")).Append('|')
            .Append(Text(parameter, "type")).Append('|')
            .AppendLine(parameter.GetProperty("optional").GetBoolean() ? "optional" : "required");
    }
    return output.ToString().Replace("\r\n", "\n", StringComparison.Ordinal);
}

if (args.Length == 1 && args[0] == "--list")
{
    Console.Write(List());
}
else if (args.Length == 2 && args[0] == "--explain")
{
    Console.Write(Explain(args[1]));
}
else if (args.Length == 1 && args[0] == "--self-test")
{
    var first = List();
    Require(first == List() &&
        first.Split("|AgentCallable|runtime-only", StringSplitOptions.None).Length - 1 == 4,
        "List determinism or capability count.");
    foreach (var id in selected.Keys)
    {
        var explanation = Explain(id);
        Require(explanation == Explain(id) &&
            explanation.Contains("authorization=runtime-only\n", StringComparison.Ordinal) &&
            explanation.Contains("brokerReady=false\n", StringComparison.Ordinal) &&
            explanation.Contains("fingerprint=", StringComparison.Ordinal) &&
            explanation.Contains("limits=", StringComparison.Ordinal),
            "Explain determinism or qualified metadata.");
    }
    var refusals = 0;
    foreach (var id in new[] { "math.unselected.helper", "math.ir.hash.semantic-extra", "", "MATH.IR.HASH.SEMANTIC" })
    {
        try { Explain(id); }
        catch (InvalidDataException) { refusals++; }
    }
    Require(refusals == 4, "Unselected capability refusals.");
    Console.WriteLine("MATH-RS192-DISCOVERY|list=4|explain=4|refusals=4|brokerReady=0|PASS");
}
else
    throw new ArgumentException("Use --list, --explain <selected-capability-id>, or --self-test.");
