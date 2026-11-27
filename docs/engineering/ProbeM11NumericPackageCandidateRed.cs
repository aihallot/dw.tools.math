#:property PublishAot=false
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("Candidate RED is scoped to the Windows private feed.");

const string QuantityId = "dw.quantities";
const string StatisticsId = "dw.tools.math.statistics";
const string QuantityVersion = "20260.20261102.1001";
const string StatisticsVersion = "20260.20261120.1001";
const string QuantitySha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string StatisticsSha = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string QuantityDll = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
const string StatisticsDll = "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea";
var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
    "NUGET_LOCAL must identify the existing absolute private feed.");
var quantities = Path.Combine(feed!, QuantityId + "." + QuantityVersion + ".nupkg");
var statistics = Path.Combine(feed!, StatisticsId + "." + StatisticsVersion + ".nupkg");

Inspect(quantities, QuantityId, QuantityVersion, QuantitySha, QuantityDll, false);
Inspect(statistics, StatisticsId, StatisticsVersion, StatisticsSha, StatisticsDll, true);
Need(File.ReadAllText("src/projects/dw.quantities/ExactRationalBudget.cs").Contains("TryScaleToInt64(", StringComparison.Ordinal),
    "Qualified exact scaling source missing.");
Need(File.Exists("src/projects/dw.tools.math.statistics/StreamingMoments64.cs"),
    "Qualified streaming moments source missing.");
var quantiles = File.ReadAllText("src/projects/dw.tools.math.statistics/QuantileStatistics64.cs");
Need(quantiles.Contains("MaximumFiniteObservations", StringComparison.Ordinal) &&
    quantiles.Contains("MaximumExactObservations", StringComparison.Ordinal),
    "Qualified larger-sample source missing.");
Need(Hash(quantities) == QuantitySha && Hash(statistics) == StatisticsSha, "Historical archives changed.");
Console.WriteLine("MATH-RS390-CANDIDATE-RED|oldArchives=2|oldDlls=2|exactDependency=PASS|missingM11Surfaces=3|sourcePresent=true|publication=false|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS390: " + message);
}
static void Inspect(string path, string id, string version, string archiveHash, string dllHash, bool statistics)
{
    Need(File.Exists(path) && Hash(path) == archiveHash, "Missing or changed physical archive: " + id);
    using var archive = ZipFile.OpenRead(path);
    using var descriptor = archive.Entries.Single(x => x.FullName == id + ".nuspec").Open();
    var root = XDocument.Load(descriptor).Root!;
    var ns = root.Name.Namespace;
    var metadata = root.Element(ns + "metadata")!;
    Need(metadata.Element(ns + "id")?.Value == id && metadata.Element(ns + "version")?.Value == version,
        "Historical package identity differs: " + id);
    var dependencies = metadata.Descendants(ns + "dependency").ToArray();
    if (statistics)
        Need(dependencies.Length == 1 && dependencies[0].Attribute("id")?.Value == "dw.quantities" &&
            dependencies[0].Attribute("version")?.Value == "[20260.20261102.1001]",
            "Historical exact dependency differs.");
    using var binary = archive.Entries.Single(x => x.FullName == "lib/net10.0/" + id + ".dll").Open();
    using var bytes = new MemoryStream();
    binary.CopyTo(bytes);
    Need(Convert.ToHexStringLower(SHA256.HashData(bytes.ToArray())) == dllHash, "Historical DLL hash differs: " + id);
    bytes.Position = 0;
    using var pe = new PEReader(bytes);
    var reader = pe.GetMetadataReader();
    bool HasType(string namespaceName, string type) => reader.TypeDefinitions.Any(handle =>
    {
        var def = reader.GetTypeDefinition(handle);
        return reader.GetString(def.Namespace) == namespaceName && reader.GetString(def.Name) == type;
    });
    bool HasMethod(string namespaceName, string type, string name, int count)
    {
        foreach (var handle in reader.TypeDefinitions)
        {
            var def = reader.GetTypeDefinition(handle);
            if (reader.GetString(def.Namespace) != namespaceName || reader.GetString(def.Name) != type) continue;
            foreach (var methodHandle in def.GetMethods())
            {
                var method = reader.GetMethodDefinition(methodHandle);
                if (reader.GetString(method.Name) == name && method.GetParameters()
                    .Count(p => reader.GetParameter(p).SequenceNumber != 0) == count)
                    return true;
            }
        }
        return false;
    }
    if (statistics)
    {
        const string statisticsNamespace = "Dw.Tools.Math.Statistics";
        Need(HasMethod(statisticsNamespace, "QuantileStatistics64", "Evaluate", 3) &&
            HasMethod(statisticsNamespace, "QuantileStatistics64", "EvaluateExact", 3), "Historical quantile controls missing.");
        Need(!HasMethod(statisticsNamespace, "QuantileStatistics64", "Evaluate", 4) &&
            !HasMethod(statisticsNamespace, "QuantileStatistics64", "EvaluateExact", 4), "Explicit quantile budgets already present.");
        Need(!HasType(statisticsNamespace, "StreamingMoments64"), "Streaming moments already present.");
    }
    else
    {
        Need(HasType("dw.quantities", "ExactRational"), "Historical rational control missing.");
        Need(!HasMethod("dw.quantities", "ExactRationalBudget", "TryScaleToInt64", 3),
            "Exact scaling already present.");
    }
    Console.WriteLine("MATH-RS390-OLD-PACKAGE|id=" + id + "|version=" + version +
        "|archiveSha256=" + archiveHash + "|dllSha256=" + dllHash + "|PASS");
}
