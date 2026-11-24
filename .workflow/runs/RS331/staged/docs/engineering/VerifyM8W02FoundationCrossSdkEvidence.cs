#:property NuGetAudit=false
#:property PublishAot=false

using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Source = "src/projects/dw.tools.math.foundation/FoundationContract.cs";
const string Project = "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj";
const string Profile = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Matrix = "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json";
const string GreenId = "m8-rs329-isolated-foundation-net10-net11-parity-and-two-contract-refusals";
var tracked = new[] { Source, Project, Profile, Matrix, "global.json", "Directory.Build.props" };
var hashes = tracked.ToDictionary(path => path, Hash, StringComparer.Ordinal);

var green = Read(".aura/workflow/reports/RS329/attempt-001.json");
var closure = Read(".aura/workflow/reports/RS330/attempt-001.json");
var greenProof = Read(".workflow/runs/RS329/preparation-proof.json");
CheckGreen(green);
CheckClosure(closure);
Need(greenProof["payloadCompiled"]?.GetValue<bool>() == true &&
    greenProof["rehearsedValidationCount"]?.GetValue<int>() == 5 &&
    greenProof["preparationValidationIds"]?.AsArray()
        .Any(x => x?.GetValue<string>() == GreenId) == true,
    "The selected GREEN preparation lacks its compiled and rehearsed contract.");

var original = File.ReadAllText(Source).Replace("\r\n", "\n", StringComparison.Ordinal);
Need(original.Contains("public const string PackageId = \"dw.tools.math.foundation\";",
        StringComparison.Ordinal) &&
    original.Contains("public const string ContractVersion = \"0.1\";",
        StringComparison.Ordinal) &&
    original.Contains("public static string Identity => PackageId + \"/\" + ContractVersion;",
        StringComparison.Ordinal),
    "The current original Foundation public identity differs.");
var project = XDocument.Load(Project);
Need(Field(project, "PackageId") == "dw.tools.math.foundation" &&
    Field(project, "IsPackable") == "true" &&
    !project.Descendants().Any(x => x.Name.LocalName == "ProjectReference"),
    "Foundation is no longer the selected source-only leaf project.");
var sdk = Read("global.json");
Need(sdk["sdk"]?["version"]?.GetValue<string>() == "10.0.401" &&
    Field(XDocument.Load("Directory.Build.props"), "TargetFramework") == "net10.0" &&
    Field(XDocument.Load("Directory.Build.props"), "LangVersion") == "14.0",
    "The original production SDK or language baseline changed.");

var risk = Read(Profile);
Need(risk["status"]?.GetValue<string>() == "blocked-not-authorized" &&
    risk["gates"]?["migrationAuthorization"]?.GetValue<string>() == "not-granted" &&
    risk["observed"]?["packableSourceProjects"]?.GetValue<int>() == 13 &&
    risk["observed"]?["publishedArchiveClosuresPhysicallyProven"]?.GetValue<int>() == 3 &&
    risk["observed"]?["publishedArchiveClosuresUnqualified"]?.GetValue<int>() == 10 &&
    risk["observed"]?["unqualifiedSemanticVerdicts"]?.GetValue<int>() == 13,
    "The bounded migration refusal frontier changed.");
var matrix = Read(Matrix);
var foundationRows = matrix["packages"]!.AsArray()
    .Where(x => x?["packageId"]?.GetValue<string>() == "dw.tools.math.foundation").ToArray();
Need(matrix["packages"]!.AsArray().Count == 13 &&
    foundationRows.Length == 1 &&
    foundationRows[0]?["compatibility"]?.GetValue<string>() == "unqualified" &&
    foundationRows[0]?["proposalCandidateIds"]?.AsArray().Count == 0,
    "Source-parity evidence was widened into semantic/API certification.");

var native = Read(".aura/workflow/plan/project.json");
var m8 = native["milestones"]!.AsArray().Single(x => x?["id"]?.GetValue<string>() == "M8")!;
var w2 = m8["workPackages"]!.AsArray()
    .Single(x => x?["id"]?.GetValue<string>() == "M8-W02")!;
var c2 = w2["phases"]!.AsArray()
    .Single(x => x?["id"]?.GetValue<string>() == "M8-W02-C02")!;
var t1 = c2["tasks"]!.AsArray()
    .Single(x => x?["id"]?.GetValue<string>() == "M8-W02-C02-T1")!;
Need(t1["state"]?.GetValue<string>() == "in-progress" &&
    t1["subtasks"]!.AsArray().Single(x => x?["id"]?.GetValue<string>() ==
        "M8-W02-C02-T1-G")?["state"]?.GetValue<string>() == "done" &&
    t1["subtasks"]!.AsArray().Single(x => x?["id"]?.GetValue<string>() ==
        "M8-W02-C02-T1-V")?["state"]?.GetValue<string>() == "in-progress" &&
    c2["tasks"]!.AsArray().Single(x => x?["id"]?.GetValue<string>() ==
        "M8-W02-C02-T2")?["state"]?.GetValue<string>() == "not-ready",
    "Verification activation incorrectly moved adjacent plan nodes.");

int negativeControls = 0;
Reject("false-success", x => x["outcome"] = "failed");
Reject("missing-cross-sdk-evidence", x =>
    x["preparationValidationEvidence"]!["validations"]!.AsArray().Clear());
Reject("concealed-boundary-violation", x =>
    x["boundaryViolations"]!.AsArray().Add(JsonValue.Create("synthetic")));
Need(negativeControls == 3, "Three isolated negative controls are required.");
foreach (var item in hashes)
    Need(Hash(item.Key) == item.Value, "An original authority file was modified: " + item.Key);
Console.WriteLine("MATH-RS331-FOUNDATION-VERIFY|greenReport=PASS|closureReport=PASS|" +
    "negativeControls=3|scope=foundation-source-only|otherTwelve=unqualified|" +
    "migrationAuthorization=not-granted|originalsPreserved=true|PASS");

void Reject(string label, Action<JsonObject> corrupt)
{
    var copy = JsonNode.Parse(green.ToJsonString())!.AsObject();
    corrupt(copy);
    bool refused = false;
    try { CheckGreen(copy); }
    catch (InvalidDataException) { refused = true; }
    Need(refused, "Tampered GREEN evidence escaped the verifier: " + label);
    negativeControls++;
    Console.WriteLine("MATH-RS331-FOUNDATION-NEGATIVE|" + label + "|PASS");
}
static void CheckGreen(JsonObject report)
{
    CheckRun(report, "RS329", "0.1.72");
    Need(report["projectNodes"]?.AsArray().Count == 1 &&
        report["projectNodes"]![0]?.GetValue<string>() == "M8-W02-C02-T1-G" &&
        report["preparationValidationEvidence"]?["validations"]?.AsArray()
            .Any(x => x?["id"]?.GetValue<string>() ==
                "m8-rs329-isolated-foundation-net10-net11-parity-and-two-contract-refusals") == true &&
        report["contribution"]?["observed"]?["validations"]?.AsArray()
            .Any(x => x?["id"]?.GetValue<string>() ==
                "m8-rs329-isolated-foundation-net10-net11-parity-and-two-contract-refusals") == true,
        "RS329 is missing its scoped isolated source-parity qualification.");
}
static void CheckClosure(JsonObject report)
{
    CheckRun(report, "RS330", "0.1.73");
    var transitions = report["progress"]?["transitions"]?.AsArray();
    Need(transitions?.Count == 2 &&
        transitions[0]?["id"]?.GetValue<string>() == "M8-W02-C02-T1-G" &&
        transitions[0]?["afterState"]?.GetValue<string>() == "done" &&
        transitions[1]?["id"]?.GetValue<string>() == "M8-W02-C02-T1-V" &&
        transitions[1]?["afterState"]?.GetValue<string>() == "ready" &&
        report["progress"]?["completions"]?.AsArray().Count == 1,
        "RS330 did not close only GREEN and ready Verification.");
}
static void CheckRun(JsonObject report, string id, string toolVersion)
{
    Need(report["runId"]?.GetValue<string>() == id &&
        report["attempt"]?.GetValue<int>() == 1 &&
        report["outcome"]?.GetValue<string>() == "succeeded" &&
        report["correctionMode"]?.GetValue<bool>() == false &&
        report["executor"]?["toolVersion"]?.GetValue<string>() == toolVersion &&
        report["priorInvocationFailures"]?["count"]?.GetValue<int>() == 0 &&
        report["priorInvocationFailures"]?["firstPassSucceeded"]?.GetValue<bool>() == true &&
        report["boundaryViolations"]?.AsArray().Count == 0 &&
        report["payload"]?["process"]?["ExitCode"]?.GetValue<int>() == 0 &&
        report["contribution"]?["observed"]?["mutationBoundaryClean"]?.GetValue<bool>() == true,
        "An expected first-pass durable run or clean boundary is absent: " + id);
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string? Field(XDocument x, string name) =>
    x.Descendants().FirstOrDefault(e => e.Name.LocalName == name)?.Value.Trim();
static string Hash(string path)
{
    using var input = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(input));
}
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("RS331-VERIFY: " + reason);
}
