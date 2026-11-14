#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text.Json;

const string FixturePath = "docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string SchemaPath = "docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json";
const string PinPath = "docs/discovery/interop/decision-1.0.0-draft.1/source.json";
const string ReceiptPath = "docs/discovery/qualification/RS131-portable-protocol-qualified.json";
const string ReportPath = ".aura/workflow/reports/RS131/attempt-001.json";
const string ExpectedCommit = "5d3af8580ea984425b314d589def18e031172c2c";
const string ExpectedFixtureHash = "5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";
const string ExpectedSchemaHash = "678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
string[] ids = [
    "valid-math-envelope", "valid-decision-envelope", "unknown-schema",
    "unknown-protocol", "documented-leak", "wrong-owner", "stale-signature",
    "missing-limits", "premature-broker-ready", "metadata-authorization",
    "bogus-version", "duplicate-capability", "catalogue-leak"
];

using var fixturesDoc = JsonDocument.Parse(File.ReadAllText(FixturePath));
using var sourceDoc = JsonDocument.Parse(File.ReadAllText(PinPath));
using var receiptDoc = JsonDocument.Parse(File.ReadAllText(ReceiptPath));
using var reportDoc = JsonDocument.Parse(File.ReadAllText(ReportPath));
var fixtures = fixturesDoc.RootElement;
var source = sourceDoc.RootElement;
var receipt = receiptDoc.RootElement;
var report = reportDoc.RootElement;

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS209: " + message);
}
static string Text(JsonElement element, string key) =>
    element.GetProperty(key).GetString() ??
    throw new InvalidDataException("RS209: missing " + key);
static string Hash(string path) =>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

Need(Hash(FixturePath) == ExpectedFixtureHash &&
     Hash(SchemaPath) == ExpectedSchemaHash, "Pinned file bytes changed.");
Need(Text(source, "source_commit") == ExpectedCommit &&
     Text(source, "protocol_id") == "dw.discovery" &&
     Text(source, "protocol_version") == "1.0.0-draft.1" &&
     source.GetProperty("manifest_schema_version").GetInt32() == 2,
     "Pinned Decision protocol identity.");
var sourceHashes = source.GetProperty("content_sha256_verified");
Need(Text(sourceHashes, "docs/discovery/conformance.json") == ExpectedFixtureHash &&
     Text(sourceHashes, "docs/discovery/manifest.schema.json") == ExpectedSchemaHash,
     "Pinned Decision source hashes.");
Need(Text(receipt, "decision_source_commit") == ExpectedCommit &&
     Text(receipt, "schema_sha256") == ExpectedSchemaHash &&
     Text(receipt, "fixtures_sha256") == ExpectedFixtureHash &&
     Text(receipt, "protocol_id") == "dw.discovery" &&
     Text(receipt, "protocol_version") == "1.0.0-draft.1" &&
     receipt.GetProperty("manifest_schema_version").GetInt32() == 2 &&
     receipt.GetProperty("csharp_protocol_verifier_executed").GetBoolean() &&
     receipt.GetProperty("csharp_portable_envelope_conformant").GetBoolean(),
     "Durable RS131 C# qualification identity.");
Need(Text(report, "runId") == "RS131" &&
     Text(report, "outcome") == "succeeded" &&
     report.GetProperty("attempt").GetInt32() == 1,
     "RS131 execution report.");
Need(fixtures.GetProperty("fixtureVersion").GetInt32() == 1 &&
     source.GetProperty("fixture_expected").GetProperty("cases").GetInt32() == 13 &&
     source.GetProperty("fixture_expected").GetProperty("accepted").GetInt32() == 2 &&
     source.GetProperty("fixture_expected").GetProperty("rejected").GetInt32() == 11 &&
     receipt.GetProperty("total_cases").GetInt32() == 13 &&
     receipt.GetProperty("accepted_cases").GetInt32() == 2 &&
     receipt.GetProperty("rejected_cases").GetInt32() == 11,
     "Portable case cardinality.");
var fixtureCases = fixtures.GetProperty("cases").EnumerateArray().ToArray();
var observed = receipt.GetProperty("case_results").EnumerateArray().ToArray();
Need(fixtureCases.Length == ids.Length && observed.Length == ids.Length, "Complete fixture results.");
var positive = 0;
var negative = 0;
for (var i = 0; i < ids.Length; i++)
{
    var test = fixtureCases[i];
    var recorded = observed[i];
    var accept = test.GetProperty("accept").GetBoolean();
    Need(Text(test, "name") == ids[i] &&
         Text(recorded, "name") == ids[i] &&
         recorded.GetProperty("expected_accept").GetBoolean() == accept &&
         recorded.GetProperty("observed_accept").GetBoolean() == accept &&
         recorded.GetProperty("passed").GetBoolean(),
         "Pinned fixture/recorded verdict mismatch: " + ids[i]);
    if (accept) positive++; else negative++;
}
Need(positive == 2 && negative == 11, "Expected two accepts and eleven refusals.");
Need(!receipt.GetProperty("broker_ready").GetBoolean() &&
     !receipt.GetProperty("math_decision_runtime_interoperability_certified").GetBoolean() &&
     !receipt.GetProperty("publicly_published").GetBoolean(),
     "Historical scope must remain portable-envelope only.");
Console.WriteLine("MATH-RS209-PORTABLE-FIXTURE-RECEIPT|cases=13|accepted=2|rejected=11|pinned=true|historical-rs131=true|brokerReady=0|PASS");
