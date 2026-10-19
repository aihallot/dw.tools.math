using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.9";
if (args.Length != 1)
    throw new ArgumentException("One local bounded signal audit NuGet archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c03-s03-t1-local-feed")
    throw new InvalidOperationException("Only the exact local S02-T2 scientific archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var stream = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing real compiled net10 scientific DLL.");
    var specs = zip.Entries.Where(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected nuspec metadata count.");
    using var item = specs[0].Open();
    var metadata = XDocument.Load(item).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (metadata.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        metadata.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        metadata.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified scientific NuGet identity or dependencies.");
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
static void DftNear(FiniteSignalAuditOutcome64 observed, params Complex[] expected)
{
    var bins = observed.Transform?.Bins ??
        throw new InvalidOperationException("Accepted direct DFT audit contains no bins.");
    Require(observed.Succeeded && observed.Convolution is null &&
        bins.Length == expected.Length && !observed.ErrorBoundGuaranteed,
        "Bounded direct DFT audit did not succeed with the expected bin count.");
    for (var i = 0; i < bins.Length; i++)
        Require(global::System.Math.Abs(bins[i].Real - expected[i].Real) < 1e-10d &&
            global::System.Math.Abs(bins[i].Imaginary - expected[i].Imaginary) < 1e-10d,
            "DFT bin differs from independent sign/normalization oracle.");
}
static void ConvolutionNear(FiniteSignalAuditOutcome64 observed, params double[] expected)
{
    var samples = observed.Convolution?.Samples ??
        throw new InvalidOperationException("Accepted linear convolution contains no samples.");
    Require(observed.Succeeded && observed.Transform is null &&
        samples.Length == expected.Length && !observed.ErrorBoundGuaranteed,
        "Linear convolution audit failed or returned a wrong output length.");
    for (var i = 0; i < samples.Length; i++)
        Require(global::System.Math.Abs(samples[i] - expected[i]) < 1e-12d,
            "Linear convolution differs from zero-extended independent oracle.");
}
static void Refused(FiniteSignalAuditOutcome64 observed, FiniteSignalAuditStatus64 expected)
{
    Require(!observed.Succeeded && observed.Status == expected &&
        observed.Transform is null && observed.Convolution is null &&
        !observed.ErrorBoundGuaranteed,
        "A signal audit refusal returned an incorrect status or a partial result.");
}
const FiniteSignalSamplingPolicy64 accepted = FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples;
const FiniteSignalConvolutionPolicy64 linear = FiniteSignalConvolutionPolicy64.LinearZeroExtended;
var total = 0;
void Probe(string label, Action assertion)
{
    try { assertion(); total++; }
    catch (Exception error) { throw new InvalidOperationException("Scientific signal audit: " + label, error); }
}
Probe("explicit discrete forward DFT impulse", () =>
    DftNear(FiniteSignalBoundaryAudit64.EvaluateDft(
        [Complex.One, Complex.Zero, Complex.Zero, Complex.Zero], sampling: accepted),
        Complex.One, Complex.One, Complex.One, Complex.One));
Probe("forward negative exponent shifted impulse", () =>
    DftNear(FiniteSignalBoundaryAudit64.EvaluateDft(
        [Complex.Zero, Complex.One, Complex.Zero, Complex.Zero], sampling: accepted),
        Complex.One, -Complex.ImaginaryOne, -Complex.One, Complex.ImaginaryOne));
Probe("inverse 1/N normalization of DC spectrum", () =>
    DftNear(FiniteSignalBoundaryAudit64.EvaluateDft(
        [new Complex(4d, 0d), Complex.Zero, Complex.Zero, Complex.Zero],
        inverse: true, sampling: accepted),
        Complex.One, Complex.One, Complex.One, Complex.One));
Probe("explicit sampling consent and no alias-free claim", () =>
    Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One]),
        FiniteSignalAuditStatus64.SamplingPolicyRequired));
Probe("conservative coherent spectral cancellation refusal", () =>
    Refused(FiniteSignalBoundaryAudit64.EvaluateDft(
        [Complex.One, Complex.One, Complex.One, Complex.One], sampling: accepted),
        FiniteSignalAuditStatus64.SpectralCancellationRisk));
Probe("zero-extended linear convolution not circular wrap", () =>
    ConvolutionNear(FiniteSignalBoundaryAudit64.EvaluateConvolution(
        [1d, 2d], [1d, 1d], topology: linear), 1d, 3d, 2d));
Probe("circular refusal and signed convolution cancellation", () =>
{
    Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d, 2d], [1d, 1d],
        topology: FiniteSignalConvolutionPolicy64.CircularUnsupported),
        FiniteSignalAuditStatus64.CircularConvolutionUnsupported);
    Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d, -1d], [1d, 1d],
        topology: linear), FiniteSignalAuditStatus64.ConvolutionCancellationRisk);
});
Probe("retained direct DFT, geometry, special functions and no foreign dependencies", () =>
{
    var raw = FiniteSignal64.DirectDft([Complex.One, Complex.Zero]);
    Require(raw.Succeeded && raw.Bins is { Length: 2 },
        "T1 direct DFT baseline changed.");
    var geometry = FiniteGeometryBoundaryAudit64.EvaluateNorm([3d, 4d]);
    Require(geometry.Succeeded && geometry.Outcome?.Scalar is { } norm &&
        global::System.Math.Abs(norm - 5d) < 1e-12d,
        "Previously qualified finite Euclidean outcome changed.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma, 5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value - 24d) < 1e-10d,
        "Previously qualified Gamma result changed.");
    var exact = BoundedExactScientific.Binomial(5, 2);
    Require(exact.Succeeded && exact.Value is { } number && number == 10,
        "Previously qualified exact integer changed.");
    var dependencies = typeof(FiniteSignalBoundaryAudit64).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!dependencies.Any(x => new[] { "MathNet", "Meta.Numerics", "Aura",
        "Decision", "MCDM", "Angouri" }.Any(banned =>
        x.Contains(banned, StringComparison.OrdinalIgnoreCase))),
        "Unexpected downstream agent/provider dependency was introduced.");
});
Require(total == 8, "Eight independent source-free signal audit probes are required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S02-T2-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-direct-signal-boundary-consumer/1.1 qualified; probes=" + total);
