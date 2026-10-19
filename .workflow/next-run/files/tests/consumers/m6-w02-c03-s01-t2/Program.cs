using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.12";
if (args.Length != 1) throw new ArgumentException("One local scientific boundary archive required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c03-t2-local-feed")
    throw new InvalidOperationException("Only the exact local S01-T2 scientific archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var buffer = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(buffer, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing compiled net10 scientific archive.");
    var specs = zip.Entries.Where(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected nuspec count.");
    using var stream = specs[0].Open();
    var metadata = XDocument.Load(stream).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (metadata.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        metadata.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        metadata.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Local package identity or dependency drift.");
}
static void Require(bool predicate, string explanation)
{
    if (!predicate) throw new InvalidOperationException(explanation);
}
static void Scalar(FiniteGeometryAuditResult64 result, double expected)
{
    Require(result.Succeeded && result.Outcome is { Succeeded: true, Scalar: { } actual } &&
        global::System.Math.Abs(actual - expected) <= 1e-12d &&
        !result.ErrorBoundGuaranteed,
        "Finite scientific scalar audit does not match independent oracle.");
}
static void Vector(FiniteGeometryAuditResult64 result, params double[] expected)
{
    Require(result.Succeeded && result.Outcome is { Succeeded: true, Coordinates: { } actual } &&
        actual.Length == expected.Length && !result.ErrorBoundGuaranteed,
        "Finite scientific vector audit does not match independent oracle.");
    for (var i = 0; i < expected.Length; i++)
        Require(global::System.Math.Abs(result.Outcome!.Coordinates![i] - expected[i]) <= 1e-12d,
            "Finite geometry projected component mismatch.");
}
static void Refused(FiniteGeometryAuditResult64 result, FiniteGeometryAuditStatus64 reason)
{
    Require(!result.Succeeded && result.Status == reason && result.Outcome is null &&
        !result.ErrorBoundGuaranteed,
        "Audit refusal has an incorrect reason or a partial result.");
}
var total = 0;
void Probe(string label, Action check)
{
    try { check(); total++; }
    catch (Exception error) { throw new InvalidOperationException("NuGet geometry boundary: " + label, error); }
}
Probe("retained norm 3-4-5", () => Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([3d, 4d]), 5d));
Probe("retained distance 3-4-5", () =>
    Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d, 0d], [3d, 4d]), 5d));
Probe("retained projection on x axis", () =>
    Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d, 4d], [1d, 0d]), 3d, 0d));
Probe("relative coordinate resolution risk", () =>
    Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d, 0d], [1e14d + 1d, 0d]),
        FiniteGeometryAuditStatus64.RelativeResolutionRisk));
Probe("near-orthogonal projection cancellation risk", () =>
    Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([1e10d, 1e10d], [1d, -1d]),
        FiniteGeometryAuditStatus64.ProjectionCancellationRisk));
Probe("zero axis, mismatched dimensions and nonfinite input refusals", () =>
{
    Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d, 4d], [0d, 0d]),
        FiniteGeometryAuditStatus64.ZeroAxis);
    Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d, 4d, 5d], [1d, 0d]),
        FiniteGeometryAuditStatus64.InvalidDimension);
    Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([double.NaN, 4d]),
        FiniteGeometryAuditStatus64.InvalidInput);
});
Probe("pre-emptive cancellation and no leaked partial data", () =>
{
    using var cancel = new CancellationTokenSource();
    cancel.Cancel();
    Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d, 0d], [3d, 4d], cancel.Token),
        FiniteGeometryAuditStatus64.Cancelled);
});
Probe("unchanged T1, Gamma and exact contracts with no unadmitted providers", () =>
{
    var direct = FiniteGeometry64.Distance([1e14d, 0d], [1e14d + 1d, 0d]);
    Require(direct.Succeeded && direct.Scalar is { } delta &&
        global::System.Math.Abs(delta - 1d) <= 1e-12d,
        "Previously qualified T1 direct Euclidean calculation changed.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma, 5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value - 24d) <= 1e-10d,
        "Previously qualified managed special functions changed.");
    var exact = BoundedExactScientific.Binomial(5, 2);
    Require(exact.Succeeded && exact.Value is { } binomial && binomial == 10,
        "Previously qualified exact integers changed.");
    var refs = typeof(FiniteGeometryBoundaryAudit64).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!refs.Any(x => new[] { "MathNet", "Meta.Numerics", "Aura", "Decision", "MCDM", "Angouri" }
        .Any(banned => x.Contains(banned, StringComparison.OrdinalIgnoreCase))),
        "Unadmitted provider dependency appears in NuGet package.");
});
Require(total == 8, "Eight distinct geometry boundary package probes are required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S01-T2-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-finite-geometry-boundary-consumer/1.1 qualified; probes=" + total);
