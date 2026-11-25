#:property NuGetAudit=false
#:property PublishAot=false

using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Profile = "docs/engineering/m8-w02-c02-source-free-consumer-red-rs333.json";
const string Ledger = "docs/engineering/m8-w02-c01-published-standard-closure-rs322.json";
const string Risk = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
var tracked = new[] { Profile, Ledger, Risk, "global.json", "Directory.Build.props" };
var hashes = tracked.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var profile = Read(Profile);
var ledger = Read(Ledger);
var risk = Read(Risk);
Need(risk["status"]?.GetValue<string>() == "blocked-not-authorized" &&
    risk["gates"]?["migrationAuthorization"]?.GetValue<string>() == "not-granted" &&
    risk["observed"]?["publishedArchiveClosuresUnqualified"]?.GetValue<int>() == 10 &&
    risk["observed"]?["unqualifiedSemanticVerdicts"]?.GetValue<int>() == 13,
    "Existing migration refusal frontier changed.");
Need(ledger["packages"]!.AsArray().Count == 3, "Historical physical scope changed.");
Need(Read("global.json")["sdk"]?["version"]?.GetValue<string>() == "10.0.401",
    "Production SDK changed.");
var props = XDocument.Load("Directory.Build.props");
Need(props.Descendants("TargetFramework").First().Value == "net10.0" &&
    props.Descendants("LangVersion").First().Value == "14.0", "Production baseline changed.");
Check(profile);
int negatives = 0;
Reject("source-reference", x => x["projectReferences"] = 1);
Reject("floating-version", x => x["exactVersionRequired"] = false);
Reject("ambient-feed", x => x["isolatedFeedRequired"] = false);
Reject("unlocked-restore", x => x["lockedRestoreRequired"] = false);
Reject("missing-binary-identity", x => x["archiveAndDllHashesRequired"] = false);
Reject("false-candidate-selection", x => x["candidateSelected"] = true);
Reject("false-all-library-claim", x => x["qualifiedPublishedClosures"] = 13);
Reject("unauthorized-publication", x => x["publicationAuthorized"] = true);
Need(negatives == 8, "All eight refusal cases required.");
foreach (var item in hashes) Need(Hash(item.Key) == item.Value, "Authority mutated: " + item.Key);
Console.WriteLine("MATH-RS333-SOURCE-FREE-RED|negativeControls=8|candidateSelected=false|actualConsumerExecuted=false|productionMutation=false|PASS");

void Reject(string name, Action<JsonObject> change)
{
    var copy = profile.DeepClone().AsObject();
    change(copy);
    var refused = false;
    try { Check(copy); }
    catch (InvalidDataException) { refused = true; }
    Need(refused, "Invalid boundary accepted: " + name);
    negatives++;
    Console.WriteLine("MATH-RS333-NEGATIVE|" + name + "|PASS");
}
static void Check(JsonObject p)
{
    Need(p["schemaVersion"]?.GetValue<int>() == 1 &&
        p["status"]?.GetValue<string>() == "blocked-pending-source-free-proof" &&
        p["targetFramework"]?.GetValue<string>() == "net11.0" &&
        p["sdk"]?.GetValue<string>() == "11.0.100" &&
        p["projectReferences"]?.GetValue<int>() == 0 &&
        p["sourceCopies"]?.GetValue<int>() == 0 &&
        p["exactVersionRequired"]?.GetValue<bool>() == true &&
        p["isolatedFeedRequired"]?.GetValue<bool>() == true &&
        p["lockedRestoreRequired"]?.GetValue<bool>() == true &&
        p["archiveAndDllHashesRequired"]?.GetValue<bool>() == true &&
        p["runtimeContractRequired"]?.GetValue<bool>() == true &&
        p["candidateSelected"]?.GetValue<bool>() == false &&
        p["actualConsumerExecuted"]?.GetValue<bool>() == false &&
        p["qualifiedPublishedClosures"]?.GetValue<int>() == 3 &&
        p["publicationAuthorized"]?.GetValue<bool>() == false &&
        p["migrationAuthorized"]?.GetValue<bool>() == false,
        "Source-free consumer RED boundary refused.");
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static void Need(bool ok, string message)
{
    if (!ok) throw new InvalidDataException("RS333: " + message);
}
