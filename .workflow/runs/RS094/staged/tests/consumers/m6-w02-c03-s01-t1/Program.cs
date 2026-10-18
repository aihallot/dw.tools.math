using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.6";
if (args.Length != 1) throw new ArgumentException("One local geometry archive is required.");
var path = Path.GetFullPath(args[0]);
if (Path.GetFileName(path) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(path)) != "m6-w02-c03-s01-t2-local-feed")
    throw new InvalidOperationException("Only the exact local geometry package is accepted.");
var bytes = File.ReadAllBytes(path);
using (var stream = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Expected compiled net10 assembly.");
    var specs = zip.Entries.Where(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected package metadata count.");
    using var item = specs[0].Open();
    var metadata = XDocument.Load(item).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (metadata.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        metadata.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        metadata.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified package identity or dependency.");
}
static void Require(bool valid, string detail)
{
    if (!valid) throw new InvalidOperationException(detail);
}
static void ScalarNear(FiniteGeometryOutcome64 outcome, double expected)
{
    Require(outcome.Succeeded && outcome.Scalar is { } scalar &&
        global::System.Math.Abs(scalar - expected) <= 1e-12d &&
        outcome.Coordinates is null && !outcome.ErrorBoundGuaranteed,
        "Expected bounded approximate Euclidean scalar result.");
}
static void ProjectNear(FiniteGeometryOutcome64 outcome, params double[] expected)
{
    Require(outcome.Succeeded && outcome.Scalar is null && outcome.Coordinates is { } actual &&
        actual.Length == expected.Length && !outcome.ErrorBoundGuaranteed,
        "Expected bounded approximate Euclidean projection.");
    for (var i = 0; i < expected.Length; i++)
        Require(global::System.Math.Abs(outcome.Coordinates![i] - expected[i]) <= 1e-12d,
            "Projected component differs from independent oracle.");
}
static void Refused(FiniteGeometryOutcome64 outcome, FiniteGeometryStatus64 status)
{
    Require(!outcome.Succeeded && outcome.Status == status &&
        outcome.Scalar is null && outcome.Coordinates is null,
        "Refusal leaked a partial value or wrong reason.");
}
var probes = 0;
void Probe(string description, Action assertion)
{
    try { assertion(); probes++; }
    catch (Exception error) { throw new InvalidOperationException("Finite geometry: " + description, error); }
}
Probe("norm 3-4-5", () => ScalarNear(FiniteGeometry64.Norm([3, 4]), 5));
Probe("distance origin 3-4-5", () => ScalarNear(FiniteGeometry64.Distance([0, 0], [3, 4]), 5));
Probe("x-axis projection", () => ProjectNear(FiniteGeometry64.Project([3, 4], [1, 0]), 3, 0));
Probe("three-dimensional projection", () => ProjectNear(FiniteGeometry64.Project([3, 4, 5], [0, 0, 1]), 0, 0, 5));
Probe("nonzero scaled axis", () => ProjectNear(FiniteGeometry64.Project([3, 4], [10, 0]), 3, 0));
Probe("zero axis refusal", () => Refused(FiniteGeometry64.Project([3, 4], [0, 0]), FiniteGeometryStatus64.ZeroAxis));
Probe("nonfinite refusal", () => Refused(FiniteGeometry64.Norm([double.PositiveInfinity, 0]), FiniteGeometryStatus64.InvalidInput));
Probe("retained scientific special and exact providers", () =>
{
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma, 5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value - 24d) < 1e-10d, "Prior special-function contract failed.");
    var exact = BoundedExactScientific.Binomial(5, 2);
    Require(exact.Succeeded && exact.Value is { } number && number == 10,
        "Prior exact scientific contract failed.");
    var references = typeof(FiniteGeometry64).Assembly.GetReferencedAssemblies().Select(x => x.Name ?? "");
    Require(!references.Any(x => new[] { "MathNet", "Meta.Numerics", "Aura", "Decision", "MCDM", "Angouri" }
        .Any(candidate => x.Contains(candidate, StringComparison.OrdinalIgnoreCase))),
        "Unadmitted provider dependency detected.");
});
Require(probes == 8, "Expected eight independent local NuGet oracles.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S01-T1-CONSUMER-SHA256:" + Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-finite-geometry-consumer/1.1 qualified; probes=" + probes);
