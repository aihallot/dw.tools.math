using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.8";
if (args.Length != 1) throw new ArgumentException("One local direct-signal archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c03-s02-t2-local-feed")
    throw new InvalidOperationException("Only the exact bounded local S02-T1 archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var stream = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing compiled net10 scientific assembly.");
    var specs = zip.Entries.Where(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Ambiguous scientific nuspec.");
    using var item = specs[0].Open();
    var meta = XDocument.Load(item).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (meta.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        meta.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        meta.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified NuGet identity or foreign dependencies.");
}
static void Require(bool success, string reason)
{
    if (!success) throw new InvalidOperationException(reason);
}
static void DftNear(FiniteDirectDftOutcome64 outcome, params Complex[] expected)
{
    var bins = outcome.Bins ??
        throw new InvalidOperationException("Direct DFT outcome has no bins.");
    Require(outcome.Succeeded && bins.Length == expected.Length &&
        !outcome.ErrorBoundGuaranteed,
        "Expected finite approximate direct DFT outcome.");
    for (var i = 0; i < bins.Length; i++)
        Require(global::System.Math.Abs(bins[i].Real - expected[i].Real) < 1e-10d &&
            global::System.Math.Abs(bins[i].Imaginary - expected[i].Imaginary) < 1e-10d,
            "Direct DFT sign or inverse convention differs from oracle.");
}
static void ConvolutionNear(FiniteLinearConvolutionOutcome64 outcome, params double[] expected)
{
    var values = outcome.Samples ??
        throw new InvalidOperationException("Linear convolution outcome has no samples.");
    Require(outcome.Succeeded && values.Length == expected.Length &&
        !outcome.ErrorBoundGuaranteed,
        "Expected finite approximate nonperiodic convolution.");
    for (var i = 0; i < expected.Length; i++)
        Require(global::System.Math.Abs(values[i] - expected[i]) < 1e-12d,
            "Convolution value differs from zero-extended oracle.");
}
static void DftRefused(FiniteDirectDftOutcome64 outcome, FiniteSignalStatus64 status)
{
    Require(!outcome.Succeeded && outcome.Status == status && outcome.Bins is null,
        "DFT refusal must be typed and nonpartial.");
}
static void ConvRefused(FiniteLinearConvolutionOutcome64 outcome, FiniteSignalStatus64 status)
{
    Require(!outcome.Succeeded && outcome.Status == status && outcome.Samples is null,
        "Convolution refusal must be typed and nonpartial.");
}
var probes = 0;
void Probe(string label, Action check)
{
    try { check(); probes++; }
    catch (Exception error) { throw new InvalidOperationException("Direct-signal NuGet: " + label, error); }
}
Probe("direct DFT impulse four", () =>
    DftNear(FiniteSignal64.DirectDft([Complex.One,Complex.Zero,Complex.Zero,Complex.Zero]),
        Complex.One,Complex.One,Complex.One,Complex.One));
Probe("forward negative exponent sign", () =>
    DftNear(FiniteSignal64.DirectDft([Complex.Zero,Complex.One,Complex.Zero,Complex.Zero]),
        Complex.One,-Complex.ImaginaryOne,-Complex.One,Complex.ImaginaryOne));
Probe("inverse positive sign and 1/N normalization", () =>
    DftNear(FiniteSignal64.DirectDft([Complex.One,Complex.One,Complex.One,Complex.One],inverse:true),
        Complex.One,Complex.Zero,Complex.Zero,Complex.Zero));
Probe("direct forward-inverse round trip", () =>
{
    var original = new[]{new Complex(1d,0d),new Complex(2d,0d),new Complex(3d,0d),new Complex(4d,0d)};
    var forward = FiniteSignal64.DirectDft(original);
    Require(forward.Succeeded, "Forward direct DFT failed.");
    DftNear(FiniteSignal64.DirectDft(forward.Bins,inverse:true), original);
});
Probe("noncircular convolution primary oracle", () =>
    ConvolutionNear(FiniteSignal64.LinearConvolution([1d,2d],[1d,1d]),1d,3d,2d));
Probe("zero-extended output length and ordering", () =>
    ConvolutionNear(FiniteSignal64.LinearConvolution([1d,2d],[3d,4d]),3d,10d,8d));
Probe("bounded nonfinite and cancellation refusals", () =>
{
    DftRefused(FiniteSignal64.DirectDft([new Complex(double.NaN,0d)]),FiniteSignalStatus64.InvalidInput);
    ConvRefused(FiniteSignal64.LinearConvolution([1d],[double.PositiveInfinity]),FiniteSignalStatus64.InvalidInput);
    using var cancel = new CancellationTokenSource();
    cancel.Cancel();
    DftRefused(FiniteSignal64.DirectDft([Complex.One],cancellationToken:cancel.Token),
        FiniteSignalStatus64.Cancelled);
});
Probe("retained geometry, special functions, exact integer and dependency isolation", () =>
{
    var geometry = FiniteGeometry64.Distance([0d,0d],[3d,4d]);
    Require(geometry.Succeeded && geometry.Scalar is { } dist &&
        global::System.Math.Abs(dist - 5d) <= 1e-12d,
        "Previously qualified Euclidean geometry changed.");
    var boundary = FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1d,0d]);
    Require(boundary.Succeeded && boundary.Outcome?.Coordinates is { Length: 2 } projected &&
        global::System.Math.Abs(projected[0] - 3d) <= 1e-12d,
        "Previously qualified Euclidean boundary audit changed.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } realGamma &&
        global::System.Math.Abs(realGamma - 24d) <= 1e-10d,
        "Previously qualified Gamma changed.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } number && number == 10,
        "Previously qualified exact integer changed.");
    var references = typeof(FiniteSignal64).Assembly.GetReferencedAssemblies().Select(x => x.Name ?? "");
    Require(!references.Any(x => new[]{"MathNet","Meta.Numerics","Aura","Decision","MCDM","Angouri"}
        .Any(id => x.Contains(id,StringComparison.OrdinalIgnoreCase))),
        "A forbidden provider or sibling agent dependency was introduced.");
});
Require(probes == 8, "Eight independently named source-free direct-signal probes required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S02-T1-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-direct-signal-consumer/1.1 qualified; probes=" + probes);
