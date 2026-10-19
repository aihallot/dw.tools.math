using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.10";
if (args.Length != 1)
    throw new ArgumentException("One locally qualified uniform-grid package archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c03-s03-t2-local-feed")
    throw new InvalidOperationException("Unexpected scientific archive path or version.");
var bytes = File.ReadAllBytes(archive);
using (var stream = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing actual compiled net10 scientific assembly.");
    var specifications = zip.Entries.Where(x =>
        x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specifications.Length != 1)
        throw new InvalidOperationException("Unexpected local NuGet metadata count.");
    using var entry = specifications[0].Open();
    var metadata = XDocument.Load(entry).Descendants()
        .Single(x => x.Name.LocalName == "metadata");
    if (metadata.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        metadata.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        metadata.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified scientific metadata or dependencies.");
}
static void Require(bool condition, string reason)
{
    if (!condition) throw new InvalidOperationException(reason);
}
static void Same(FiniteUniformSeriesAuditOutcome64 result, double[] expected,
    double interval, int window)
{
    var samples = result.Outcome?.Samples ??
        throw new InvalidOperationException("Successful uniform-series audit has no samples.");
    Require(result.Succeeded && samples.Length == expected.Length &&
        result.Outcome!.SampleInterval == interval &&
        result.Outcome.Window == window && !result.ErrorBoundGuaranteed,
        "Unexpected uniform-series audit length, interval, window or outcome.");
    for (var i = 0; i < expected.Length; i++)
        Require(global::System.Math.Abs(samples[i] - expected[i]) <= 1e-12d,
            "Independent finite uniform-grid numerical oracle differs.");
}
static void Refused(FiniteUniformSeriesAuditOutcome64 result,
    FiniteUniformSeriesAuditStatus64 status)
{
    Require(!result.Succeeded && result.Status == status &&
        result.Outcome is null && !result.ErrorBoundGuaranteed,
        "Expected typed nonpartial uniform-grid refusal.");
}
var probes = 0;
void Probe(string label, Action check)
{
    try { check(); probes++; }
    catch (Exception error)
    {
        throw new InvalidOperationException("Uniform grid NuGet: " + label, error);
    }
}
Probe("explicit equal-time-step adjacent differences", () =>
    Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
        [1d,3d,6d],[0d,1d,2d],1d),[2d,3d],1d,2));
Probe("complete-window sliding arithmetic means", () =>
    Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
        [1d,3d,6d],[0d,1d,2d],1d,2),[2d,4.5d],1d,2));
Probe("refuse missing and irregular time-grid data", () =>
{
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
        [1d,3d],null,1d),FiniteUniformSeriesAuditStatus64.SampleTimesRequired);
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
        [1d,3d,6d],[0d,1d,3d],1d,2),FiniteUniformSeriesAuditStatus64.IrregularSampling);
});
Probe("conservative relative timestamp resolution refusal", () =>
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
        [1d,3d],[1e14d,1e14d+1d],1d),
        FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk));
Probe("conservative small adjacent difference risk", () =>
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
        [1e6d,1e6d+1e-8d],[0d,1d],1d),
        FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk));
Probe("signed sliding mean cancellation risk", () =>
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
        [1d,-1d],[0d,1d],1d,2),
        FiniteUniformSeriesAuditStatus64.MeanCancellationRisk));
Probe("typed invalid window and cancellation refusals", () =>
{
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
        [1d,3d],[0d,1d],1d,3),FiniteUniformSeriesAuditStatus64.InvalidWindow);
    using var source = new CancellationTokenSource();
    source.Cancel();
    Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
        [1d,3d],[0d,1d],1d,2,source.Token),FiniteUniformSeriesAuditStatus64.Cancelled);
});
Probe("retained uniform, signal, geometry, special, exact and dependency contracts", () =>
{
    var previous = FiniteUniformSeries64.FirstDifferences([1e6d,1e6d+1e-8d],1d);
    Require(previous.Succeeded && previous.Samples is { Length: 1 },
        "Previously qualified raw adjacent-difference facade changed.");
    var signal = FiniteSignal64.DirectDft([Complex.One,Complex.Zero]);
    Require(signal.Succeeded && signal.Bins is { Length: 2 },
        "Previously qualified direct DFT facade changed.");
    var signalAudit = FiniteSignalBoundaryAudit64.EvaluateDft(
        [Complex.One],sampling:FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples);
    Require(signalAudit.Succeeded,
        "Previously qualified explicit DFT audit changed.");
    var geometry = FiniteGeometry64.Norm([3d,4d]);
    Require(geometry.Succeeded && geometry.Scalar is { } length &&
        global::System.Math.Abs(length-5d) <= 1e-12d,
        "Previously qualified Euclidean geometry changed.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value-24d) <= 1e-10d,
        "Previously qualified Gamma changed.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } number && number == 10,
        "Previously qualified exact integer changed.");
    var dependencies = typeof(FiniteUniformSeriesBoundaryAudit64).Assembly
        .GetReferencedAssemblies().Select(x => x.Name ?? "");
    Require(!dependencies.Any(x => new[] { "MathNet", "Meta.Numerics", "Aura",
        "Decision", "MCDM", "Angouri" }.Any(banned =>
        x.Contains(banned,StringComparison.OrdinalIgnoreCase))),
        "An unsupported foreign numerical provider or downstream agent dependency appeared.");
});
Require(probes == 8, "Eight independently named source-free uniform-grid probes required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S03-T2-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-uniform-series-boundary-consumer/1.1 qualified; probes=" + probes);
