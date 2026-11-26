#:property NuGetAudit=false
#:property PublishAot=false
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check") throw new ArgumentException("Expected --check.");
static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("M9 package Verification: " + reason);
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static string?[] Ids(JsonNode entries) => entries.AsArray().Select(x => (string?)x?["id"]).ToArray();
static string Hash(string path) { using var stream = File.OpenRead(path); return Convert.ToHexStringLower(SHA256.HashData(stream)); }
var cases = new[]
{
    (Run: "RS356", Driver: "0.1.76", Commit: "78b7500554384d4148c305d6d4a0a1292284f342", Count: 8, Check: "m9-rs356-actual-exact-locked-package-refusals"),
    (Run: "RS358", Driver: "0.1.76", Commit: "1b71d972dc468f3206ba485b5a4cb7ec20d3e3ea", Count: 8, Check: "m9-rs358-physical-published-windows-net11-readback"),
    (Run: "RS359", Driver: "0.1.78", Commit: "68c584652635320f186ff70994d953ab2eb5bd43", Count: 7, Check: "m9-rs359-package-green-closure-evidence")
};
foreach (var item in cases)
{
    var report = Read(".aura/workflow/reports/" + item.Run + "/attempt-001.json");
    var proof = Read(".workflow/runs/" + item.Run + "/preparation-proof.json");
    Check(report, proof, item);
}
var green = cases[1];
var greenReport = Read(".aura/workflow/reports/RS358/attempt-001.json");
var greenProof = Read(".workflow/runs/RS358/preparation-proof.json");
var negatives = new (string Name, Action<JsonObject> Mutate)[]
{
    ("false-success", x => x["outcome"] = "failed"),
    ("missing-validation", x => x["preparationValidationEvidence"]!["validations"]!.AsArray().Clear()),
    ("concealed-boundary", x => x["boundaryViolations"]!.AsArray().Add(JsonValue.Create("synthetic")))
};
foreach (var negative in negatives)
{
    var copy = greenReport.DeepClone().AsObject();
    negative.Mutate(copy);
    var refused = false;
    try { Check(copy, greenProof, green); } catch (InvalidDataException) { refused = true; }
    Need(refused, "Altered evidence escaped: " + negative.Name);
    Console.WriteLine("MATH-RS360-NEGATIVE|" + negative.Name + "|PASS");
}
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed), "Physical feed missing.");
var packages = new[]
{
    (Id: "dw.tools.math.statistics", Version: "20260.20261120.1001", Archive: "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1", Binary: "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea"),
    (Id: "dw.quantities", Version: "20260.20261102.1001", Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d", Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
foreach (var item in packages)
{
    var path = Path.Combine(feed!, item.Id + "." + item.Version + ".nupkg");
    Need(File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0 && Hash(path) == item.Archive,
        "Physical immutable archive differs: " + item.Id);
    using var zip = ZipFile.OpenRead(path);
    using var dll = zip.Entries.Single(x => x.FullName == "lib/net10.0/" + item.Id + ".dll").Open();
    Need(Convert.ToHexStringLower(SHA256.HashData(dll)) == item.Binary, "Packaged DLL differs: " + item.Id);
    using var nuspec = zip.Entries.Single(x => x.FullName == item.Id + ".nuspec").Open();
    var xml = XDocument.Load(nuspec);
    var ns = xml.Root!.Name.Namespace;
    var meta = xml.Root.Element(ns + "metadata")!;
    Need(meta.Element(ns + "id")?.Value == item.Id && meta.Element(ns + "version")?.Value == item.Version,
        "NuGet identity differs: " + item.Id);
    if (item.Id == packages[0].Id)
    {
        var deps = meta.Element(ns + "dependencies")!.Descendants(ns + "dependency").ToArray();
        Need(deps.Length == 1 && deps[0].Attribute("id")?.Value == packages[1].Id &&
            deps[0].Attribute("version")?.Value == "[" + packages[1].Version + "]", "Exact dependency differs.");
    }
    Need(Hash(path) == item.Archive, "Archive changed during audit.");
}
var previous = Path.Combine(feed!, "dw.tools.math.statistics.20260.20261106.1001.nupkg");
Need(File.Exists(previous) && Hash(previous) == "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8",
    "Earlier immutable Statistics archive differs.");
Console.WriteLine("MATH-RS360-PACKAGE-VERIFY|reportProofPairs=3|negativeControls=3|archiveAndDllIdentities=2|dependency=exact|publication=false|PASS");

static void Check(JsonObject r, JsonObject p, (string Run, string Driver, string Commit, int Count, string Check) item)
{
    var observed = r["contribution"]!["observed"]!;
    var ids = Ids(observed["validations"]!);
    var preparationIds = Ids(r["preparationValidationEvidence"]!["validations"]!);
    Need((string?)r["runId"] == item.Run && (string?)r["outcome"] == "succeeded" &&
        (int?)r["attempt"] == 1 && (bool?)r["correctionMode"] == false &&
        (string?)r["executor"]?["toolVersion"] == item.Driver &&
        (int?)r["priorInvocationFailures"]?["count"] == 0 &&
        (bool?)r["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
        (int?)r["payload"]?["process"]?["ExitCode"] == 0 &&
        r["boundaryViolations"]!.AsArray().Count == 0 &&
        (bool?)observed["mutationBoundaryClean"] == true &&
        !observed["changedPaths"]!.AsArray().Any(x => ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true),
        "Invalid durable provenance: " + item.Run);
    Need((string?)p["runId"] == item.Run && (string?)p["sourceCommit"] == item.Commit &&
        (bool?)p["payloadCompiled"] == true && (int?)p["rehearsedValidationCount"] == item.Count &&
        ids.Length == item.Count && ids.Contains(item.Check, StringComparer.Ordinal) &&
        preparationIds.SequenceEqual(ids, StringComparer.Ordinal) &&
        p["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(ids, StringComparer.Ordinal),
        "Compiled validation proof mismatch: " + item.Run);
    var moves = r["progress"]!["transitions"]!.AsArray();
    var completions = r["progress"]!["completions"]!.AsArray();
    bool Move(string id, string before, string after) => moves.Any(x => (string?)x?["id"] == id &&
        (string?)x?["beforeState"] == before && (string?)x?["afterState"] == after);
    Need(item.Run switch
    {
        "RS356" => moves.Count == 2 && completions.Count == 0 && Move("M9-W01-C01-T2", "ready", "in-progress") && Move("M9-W01-C01-T2-R", "ready", "in-progress"),
        "RS358" => moves.Count == 1 && completions.Count == 0 && Move("M9-W01-C01-T2-G", "ready", "in-progress"),
        "RS359" => moves.Count == 2 && completions.Count == 1 && (string?)completions[0]?["id"] == "M9-W01-C01-T2-G" &&
            Move("M9-W01-C01-T2-G", "in-progress", "done") && Move("M9-W01-C01-T2-V", "not-ready", "ready"),
        _ => false
    }, "Unexpected native transition evidence: " + item.Run);
}
