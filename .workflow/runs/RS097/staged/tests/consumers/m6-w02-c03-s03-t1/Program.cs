using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.9";
if (args.Length != 1)
    throw new ArgumentException("One local uniform-series NuGet archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c03-s03-t1-local-feed")
    throw new InvalidOperationException("Only the pinned S03-T1 local archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var buffer = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(buffer, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Expected real compiled net10 scientific package.");
    var specs = zip.Entries.Where(x =>
        x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Unexpected nuspec count.");
    using var spec = specs[0].Open();
    var meta = XDocument.Load(spec).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (meta.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        meta.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        meta.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Foreign NuGet dependencies or package version drift.");
}
static void Require(bool accepted, string reason)
{
    if (!accepted) throw new InvalidOperationException(reason);
}
static void Same(FiniteUniformSeriesOutcome64 outcome, double[] expected,
    double interval, int window)
{
    var values = outcome.Samples ??
        throw new InvalidOperationException("Uniform-series success did not provide samples.");
    Require(outcome.Succeeded && values.Length == expected.Length &&
        outcome.SampleInterval == interval && outcome.Window == window &&
        !outcome.ErrorBoundGuaranteed,
        "Uniform-series output length, interval, window or status differs from oracle.");
    for (var i = 0; i < values.Length; i++)
        Require(global::System.Math.Abs(values[i] - expected[i]) <= 1e-12d,
            "Uniform-series binary64 value differs from independent oracle.");
}
static void Refused(FiniteUniformSeriesOutcome64 outcome, FiniteUniformSeriesStatus64 status)
{
    Require(!outcome.Succeeded && outcome.Status == status &&
        outcome.Samples is null && !outcome.ErrorBoundGuaranteed,
        "A uniform-series typed refusal leaked a partial value.");
}
var probes = 0;
void Probe(string label, Action check)
{
    try { check(); probes++; }
    catch (Exception error)
    {
        throw new InvalidOperationException("Uniform-series package: " + label, error);
    }
}
Probe("first differences are raw adjacent deltas", () =>
    Same(FiniteUniformSeries64.FirstDifferences([1d,3d,6d],1d),[2d,3d],1d,2));
Probe("valid window 2 sliding means", () =>
    Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],1d,2),[2d,4.5d],1d,2));
Probe("interval preserved as metadata without dividing differences", () =>
{
    Same(FiniteUniformSeries64.FirstDifferences([1d,3d],0.5d),[2d],0.5d,2);
    Same(FiniteUniformSeries64.MovingAverage([1d,3d],0.5d,2),[2d],0.5d,2);
});
Probe("signed deltas and complete window only", () =>
{
    Same(FiniteUniformSeries64.FirstDifferences([-2d,5d,-1d],1d),[7d,-6d],1d,2);
    Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],1d,1),[1d,3d,6d],1d,1);
});
Probe("positive finite interval and missing values are not synthesized", () =>
{
    Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],0d),
        FiniteUniformSeriesStatus64.InvalidSampleInterval);
    Refused(FiniteUniformSeries64.MovingAverage([1d,double.NaN],1d,2),
        FiniteUniformSeriesStatus64.InvalidInput);
});
Probe("nonpartial invalid window and cancellation refusals", () =>
{
    Refused(FiniteUniformSeries64.MovingAverage([1d,3d],1d,3),
        FiniteUniformSeriesStatus64.InvalidWindow);
    using var cancel = new CancellationTokenSource();
    cancel.Cancel();
    Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],1d,cancel.Token),
        FiniteUniformSeriesStatus64.Cancelled);
});
Probe("bounded input and absent forecasting or resampling", () =>
{
    Refused(FiniteUniformSeries64.FirstDifferences([1e7d,0d],1d),
        FiniteUniformSeriesStatus64.OutsideQualifiedRange);
    Refused(FiniteUniformSeries64.MovingAverage(new double[33],1d,2),
        FiniteUniformSeriesStatus64.InvalidLength);
});
Probe("retained signal, geometry, Gamma, exact and source isolation", () =>
{
    var direct = FiniteSignal64.DirectDft([Complex.One,Complex.Zero]);
    Require(direct.Succeeded && direct.Bins is { Length: 2 },
        "Previously qualified direct DFT drifted.");
    var audit = FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],
        sampling: FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples);
    Require(audit.Succeeded, "Previously qualified DFT risk audit drifted.");
    var geometry = FiniteGeometry64.Norm([3d,4d]);
    Require(geometry.Succeeded && geometry.Scalar is { } norm &&
        global::System.Math.Abs(norm-5d) < 1e-12d,
        "Previously qualified finite Euclidean geometry drifted.");
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value-24d) < 1e-10d,
        "Previously qualified Gamma drifted.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } choose && choose == 10,
        "Previously qualified exact integer drifted.");
    var references = typeof(FiniteUniformSeries64).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!references.Any(x => new[] { "MathNet", "Meta.Numerics", "Aura",
        "Decision", "MCDM", "Angouri" }.Any(bad =>
        x.Contains(bad, StringComparison.OrdinalIgnoreCase))),
        "Unexpected external provider or agent dependency.");
});
Require(probes == 8, "Eight independent uniform-series NuGet probes are required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S03-T1-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-uniform-series-consumer/1.1 qualified; probes=" + probes);
