using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.14";
if (args.Length != 1)
    throw new ArgumentException("One local cross-family scientific archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c04-t2-local-feed")
    throw new InvalidOperationException("Only the exact local cross-family archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var source = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(source, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No actual compiled net10 scientific assembly.");
    var specs = zip.Entries.Where(x =>
        x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected nuspec count.");
    using var entry = specs[0].Open();
    var meta = XDocument.Load(entry).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (meta.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        meta.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        meta.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified scientific NuGet identity or dependencies.");
}
static void Require(bool accepted, string detail)
{
    if (!accepted) throw new InvalidOperationException(detail);
}
static M6W02C03Request64 Valid() =>
    new([0d,0d],[3d,4d],[1d,0d],
        [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
        [1d,2d],[1d,1d],[1d,3d,6d],1d,2);
static M6W02C03Outcome64 Accepted(M6W02C03Request64 request)
{
    var outcome = M6W02C03Contract.Evaluate(request);
    Require(outcome.Succeeded && outcome.Status == M6W02C03Status64.Success &&
        !outcome.ErrorBoundGuaranteed,
        "Cross-family contract did not return seven accepted outcomes.");
    return outcome;
}
static void Vector(double[]? actual, params double[] expected)
{
    var values = actual ??
        throw new InvalidOperationException("A successful aggregate vector is missing.");
    Require(values.Length == expected.Length, "Cross-family vector length differs.");
    for (var i = 0; i < values.Length; i++)
        Require(global::System.Math.Abs(values[i] - expected[i]) <= 1e-12d,
            "Cross-family vector oracle differs.");
}
static void Bins(Complex[]? actual, params Complex[] expected)
{
    var values = actual ??
        throw new InvalidOperationException("A successful aggregate DFT is missing.");
    Require(values.Length == expected.Length, "Cross-family DFT bin length differs.");
    for (var i = 0; i < values.Length; i++)
        Require(global::System.Math.Abs(values[i].Real - expected[i].Real) < 1e-10d &&
            global::System.Math.Abs(values[i].Imaginary - expected[i].Imaginary) < 1e-10d,
            "Cross-family direct DFT convention differs.");
}
static void Refused(M6W02C03Outcome64 outcome, M6W02C03Status64 expected)
{
    Require(!outcome.Succeeded && outcome.Status == expected &&
        outcome.Norm is null && outcome.Distance is null &&
        outcome.Projection is null && outcome.Dft is null &&
        outcome.LinearConvolution is null && outcome.Differences is null &&
        outcome.MovingAverage is null && !outcome.ErrorBoundGuaranteed,
        "Cross-family typed refusal leaked a partial result.");
}
var probes = 0;
void Probe(string label, Action check)
{
    try { check(); probes++; }
    catch (Exception failure) { throw new InvalidOperationException("Cross-family NuGet: " + label, failure); }
}
Probe("cross-family canonical seven-oracle acceptance", () =>
{
    var result = Accepted(Valid());
    Require(result.Norm?.Scalar is { } norm &&
        global::System.Math.Abs(norm - 5d) <= 1e-12d,
        "3-4-5 Euclidean norm oracle failed.");
    Require(result.Distance?.Scalar is { } distance &&
        global::System.Math.Abs(distance - 5d) <= 1e-12d,
        "3-4-5 Euclidean distance oracle failed.");
    Vector(result.Projection?.Coordinates,3d,0d);
    Bins(result.Dft?.Bins,Complex.One,Complex.One,Complex.One,Complex.One);
    Vector(result.LinearConvolution?.Samples,1d,3d,2d);
    Vector(result.Differences?.Samples,2d,3d);
    Vector(result.MovingAverage?.Samples,2d,4.5d);
});
Probe("independent three-dimensional geometry and projection", () =>
{
    var result = Accepted(Valid() with {
        GeometryStart=[0d,0d,0d],GeometryEnd=[3d,4d,0d],ProjectionAxis=[0d,1d,0d]});
    Vector(result.Projection?.Coordinates,0d,4d,0d);
    Require(result.Distance?.Scalar is { } length &&
        global::System.Math.Abs(length - 5d) <= 1e-12d,
        "3D distance contract changed.");
});
Probe("independent negative-exponent direct DFT sign", () =>
{
    var result = Accepted(Valid() with {
        DiscreteSamples=[Complex.Zero,Complex.One,Complex.Zero,Complex.Zero]});
    Bins(result.Dft?.Bins,Complex.One,-Complex.ImaginaryOne,
        -Complex.One,Complex.ImaginaryOne);
});
Probe("zero-extended noncircular convolution", () =>
{
    var result = Accepted(Valid() with {
        ConvolutionLeft=[1d,2d],ConvolutionRight=[3d,4d]});
    Vector(result.LinearConvolution?.Samples,3d,10d,8d);
});
Probe("caller interval and complete uniform-series windows", () =>
{
    var result = Accepted(Valid() with {SampleInterval=0.5d,MovingWindow=1});
    Vector(result.Differences?.Samples,2d,3d);
    Vector(result.MovingAverage?.Samples,1d,3d,6d);
    Require(result.Differences?.SampleInterval == 0.5d,
        "Unexpected implicit derivative or sample-unit conversion.");
});
Probe("geometry refusal with no leaked signal or series data", () =>
    Refused(M6W02C03Contract.Evaluate(
        Valid() with {ProjectionAxis=[0d,0d]}),
        M6W02C03Status64.GeometryRefused));
Probe("signal and uniform-series refusals remain all-or-nothing", () =>
{
    Refused(M6W02C03Contract.Evaluate(
        Valid() with {ConvolutionLeft=null}),
        M6W02C03Status64.SignalRefused);
    Refused(M6W02C03Contract.Evaluate(
        Valid() with {SampleInterval=0d}),
        M6W02C03Status64.UniformSeriesRefused);
});
Probe("pre-cancelled aggregate and retained family/no-foreign-dependency smoke", () =>
{
    using var token = new CancellationTokenSource();
    token.Cancel();
    Refused(M6W02C03Contract.Evaluate(Valid(),token.Token),M6W02C03Status64.Cancelled);
    var geometry = FiniteGeometryBoundaryAudit64.EvaluateNorm([3d,4d]);
    Require(geometry.Succeeded && geometry.Outcome?.Scalar is { } length &&
        global::System.Math.Abs(length - 5d) <= 1e-12d,
        "Previously qualified geometry audit drifted.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value - 24d) <= 1e-10d,
        "Previously qualified Gamma drifted.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } choose && choose == 10,
        "Previously qualified exact scientific arithmetic drifted.");
    var references = typeof(M6W02C03Contract).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!references.Any(x => new[]{"MathNet","Meta.Numerics","Aura",
        "Decision","MCDM","Angouri"}.Any(id =>
        x.Contains(id,StringComparison.OrdinalIgnoreCase))),
        "Foreign numerical provider or agent dependency introduced.");
});
Require(probes == 8, "Eight independently named local cross-family package probes required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-T1-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-scientific-families-consumer/1.1 qualified; probes=" + probes);
