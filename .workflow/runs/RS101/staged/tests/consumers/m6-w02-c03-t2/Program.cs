using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.13";
if (args.Length != 1)
    throw new ArgumentException("One locally qualified cross-family boundary NuGet archive required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c04-t1-local-feed")
    throw new InvalidOperationException("Unexpected local cross-family audit archive path or identity.");
var bytes = File.ReadAllBytes(archive);
using (var memory = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(memory, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing actual compiled net10 scientific assembly.");
    var specs = zip.Entries.Where(x =>
        x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected scientific nuspec count.");
    using var item = specs[0].Open();
    var meta = XDocument.Load(item).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (meta.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        meta.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        meta.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified local archive identity or NuGet dependencies.");
}
static void Require(bool condition, string error)
{
    if (!condition) throw new InvalidOperationException(error);
}
static M6W02C03Request64 Family() =>
    new([0d,0d],[3d,4d],[1d,0d],
        [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
        [1d,2d],[1d,1d],[1d,3d,6d],1d,2);
static M6W02C03BoundaryRequest64 Valid() =>
    new(Family(),[0d,1d,2d],
        M6W02C03UnitPolicy64.CallerManagesIndependentUnits,
        FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples,
        FiniteSignalConvolutionPolicy64.LinearZeroExtended);
static M6W02C03BoundaryOutcome64 Accepted(M6W02C03BoundaryRequest64 input)
{
    var result = M6W02C03BoundaryAggregate64.Evaluate(input);
    Require(result.Succeeded && result.Status == M6W02C03BoundaryStatus64.Success &&
        result.GeometryReason is null && result.SignalReason is null &&
        result.UniformSeriesReason is null && !result.ErrorBoundGuaranteed,
        "Cross-family boundary aggregate did not return seven successful audited results.");
    return result;
}
static void Refused(M6W02C03BoundaryOutcome64 result,
    M6W02C03BoundaryStatus64 status,
    FiniteGeometryAuditStatus64? geometry = null,
    FiniteSignalAuditStatus64? signal = null,
    FiniteUniformSeriesAuditStatus64? series = null)
{
    Require(!result.Succeeded && result.Status == status &&
        result.Norm is null && result.Distance is null &&
        result.Projection is null && result.Dft is null &&
        result.LinearConvolution is null && result.Differences is null &&
        result.MovingAverage is null && result.GeometryReason == geometry &&
        result.SignalReason == signal && result.UniformSeriesReason == series &&
        !result.ErrorBoundGuaranteed,
        "Cross-family boundary refusal must be typed and contain no partial numeric results.");
}
static void Vector(double[]? samples, params double[] expected)
{
    var values = samples ??
        throw new InvalidOperationException("Expected successful numeric vector is missing.");
    Require(values.Length == expected.Length, "Cross-family numeric vector length differs.");
    for (var index = 0; index < values.Length; index++)
        Require(global::System.Math.Abs(values[index] - expected[index]) <= 1e-12d,
            "Cross-family numeric vector does not match independent oracle.");
}
static void Bins(Complex[]? bins, params Complex[] expected)
{
    var values = bins ??
        throw new InvalidOperationException("Expected successful DFT bins are missing.");
    Require(values.Length == expected.Length, "Cross-family DFT bin length differs.");
    for (var index = 0; index < values.Length; index++)
        Require(global::System.Math.Abs(values[index].Real - expected[index].Real) <= 1e-10d &&
            global::System.Math.Abs(values[index].Imaginary - expected[index].Imaginary) <= 1e-10d,
            "Cross-family DFT sign or component differs.");
}
var probes = 0;
void Probe(string label, Action assertion)
{
    try { assertion(); probes++; }
    catch (Exception error)
    {
        throw new InvalidOperationException("Cross-family boundary NuGet: " + label, error);
    }
}
Probe("all seven audited scientific family oracles", () =>
{
    var result = Accepted(Valid());
    Require(result.Norm?.Outcome?.Scalar is { } norm &&
        global::System.Math.Abs(norm-5d) <= 1e-12d,
        "Euclidean norm(3,4) did not equal 5.");
    Require(result.Distance?.Outcome?.Scalar is { } distance &&
        global::System.Math.Abs(distance-5d) <= 1e-12d,
        "Euclidean distance did not equal 5.");
    Vector(result.Projection?.Outcome?.Coordinates,3d,0d);
    Bins(result.Dft?.Transform?.Bins,Complex.One,Complex.One,Complex.One,Complex.One);
    Vector(result.LinearConvolution?.Convolution?.Samples,1d,3d,2d);
    Vector(result.Differences?.Outcome?.Samples,2d,3d);
    Vector(result.MovingAverage?.Outcome?.Samples,2d,4.5d);
});
Probe("caller-managed units and pre-emptive cancellation", () =>
{
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {UnitPolicy=M6W02C03UnitPolicy64.Unspecified}),
        M6W02C03BoundaryStatus64.UnitPolicyRequired);
    using var stop = new CancellationTokenSource();
    stop.Cancel();
    Refused(M6W02C03BoundaryAggregate64.Evaluate(Valid(),stop.Token),
        M6W02C03BoundaryStatus64.Cancelled);
});
Probe("geometry relative resolution and projection cancellation", () =>
{
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {Scientific=Family() with {
            GeometryStart=[1e14d,0d],GeometryEnd=[1e14d+1d,0d]}}),
        M6W02C03BoundaryStatus64.GeometryRefused,
        geometry:FiniteGeometryAuditStatus64.RelativeResolutionRisk);
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {Scientific=Family() with {
            GeometryEnd=[1e10d,1e10d],ProjectionAxis=[1d,-1d]}}),
        M6W02C03BoundaryStatus64.GeometryRefused,
        geometry:FiniteGeometryAuditStatus64.ProjectionCancellationRisk);
});
Probe("explicit discrete sampling and noncircular topology", () =>
{
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {SamplingPolicy=FiniteSignalSamplingPolicy64.Unspecified}),
        M6W02C03BoundaryStatus64.SignalRefused,
        signal:FiniteSignalAuditStatus64.SamplingPolicyRequired);
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {ConvolutionTopology=FiniteSignalConvolutionPolicy64.CircularUnsupported}),
        M6W02C03BoundaryStatus64.SignalRefused,
        signal:FiniteSignalAuditStatus64.CircularConvolutionUnsupported);
});
Probe("signal spectral and convolution cancellation refuse globally", () =>
{
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {Scientific=Family() with {
            DiscreteSamples=[Complex.One,Complex.One]}}),
        M6W02C03BoundaryStatus64.SignalRefused,
        signal:FiniteSignalAuditStatus64.SpectralCancellationRisk);
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {Scientific=Family() with {
            ConvolutionLeft=[1d,-1d],ConvolutionRight=[1d,1d]}}),
        M6W02C03BoundaryStatus64.SignalRefused,
        signal:FiniteSignalAuditStatus64.ConvolutionCancellationRisk);
});
Probe("no irregular timestamps, unqualified resolution or mean cancellation", () =>
{
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {SampleTimes=[0d,1d,3d]}),
        M6W02C03BoundaryStatus64.UniformSeriesRefused,
        series:FiniteUniformSeriesAuditStatus64.IrregularSampling);
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {SampleTimes=[1e14d,1e14d+1d,1e14d+2d]}),
        M6W02C03BoundaryStatus64.UniformSeriesRefused,
        series:FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk);
    Refused(M6W02C03BoundaryAggregate64.Evaluate(
        Valid() with {Scientific=Family() with {
            UniformSamples=[1d,-1d]},SampleTimes=[0d,1d]}),
        M6W02C03BoundaryStatus64.UniformSeriesRefused,
        series:FiniteUniformSeriesAuditStatus64.MeanCancellationRisk);
});
Probe("caller-configured 3D and half-interval admitted without reinterpolation", () =>
{
    var result = Accepted(Valid() with {
        Scientific=Family() with {
            GeometryStart=[0d,0d,0d],GeometryEnd=[3d,4d,0d],
            ProjectionAxis=[0d,1d,0d],SampleInterval=0.5d,MovingWindow=1},
        SampleTimes=[0d,0.5d,1d]});
    Vector(result.Projection?.Outcome?.Coordinates,0d,4d,0d);
    Vector(result.Differences?.Outcome?.Samples,2d,3d);
    Vector(result.MovingAverage?.Outcome?.Samples,1d,3d,6d);
});
Probe("prior direct aggregate and source-free exact scientific dependencies", () =>
{
    var original = M6W02C03Contract.Evaluate(Family());
    Require(original.Succeeded && original.Dft?.Bins is { Length: 4 },
        "Prior T1 cross-family composition changed.");
    var special = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(special.Succeeded && special.Value is { } gamma &&
        global::System.Math.Abs(gamma - 24d) <= 1e-10d,
        "Previously qualified Gamma changed.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } value && value == 10,
        "Previously qualified exact integer changed.");
    var refs = typeof(M6W02C03BoundaryAggregate64).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!refs.Any(x => new[]{"MathNet","Meta.Numerics","Aura",
        "Decision","MCDM","Angouri"}.Any(banned =>
        x.Contains(banned,StringComparison.OrdinalIgnoreCase))),
        "Foreign numerical provider or sibling agent dependency introduced.");
});
Require(probes == 8, "Eight independently named source-free cross-family boundary probes required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-T2-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-scientific-boundary-consumer/1.1 qualified; probes=" + probes);
