using System;
using System.Linq;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Statistics;

// This is an independently compiled package-only consumer. Its scaling
// adapter belongs to this sample, not the Math product or Decision.
var passed = 0;
void Probe(string name, Action check)
{
    try { check(); passed++; }
    catch (Exception ex)
    {
        throw new InvalidOperationException("C04 independent consumer oracle failed: " + name, ex);
    }
}

Probe("exact rational addition", () =>
    Require(new ExactRational(1, 3) + new ExactRational(1, 6) == new ExactRational(1, 2)));

Probe("scaled exact quarter", () =>
    Require(TryScaleExactly(new ExactRational(3, 4), 100, out var value) && value == 75));

Probe("scaled exact negative quarter", () =>
    Require(TryScaleExactly(new ExactRational(-3, 4), 100, out var value) && value == -75));

Probe("nonintegral third refuses loss", () =>
    Require(!TryScaleExactly(new ExactRational(1, 3), 100, out _)));

Probe("nonintegral thousandth refuses loss", () =>
    Require(!TryScaleExactly(new ExactRational(1, 1000), 100, out _)));

Probe("Int64 upper overflow is refused", () =>
    Require(!TryScaleExactly(new ExactRational((BigInteger)long.MaxValue + 1, BigInteger.One), 1, out _)));

Probe("Int64 lower overflow is refused", () =>
    Require(!TryScaleExactly(new ExactRational((BigInteger)long.MinValue - 1, BigInteger.One), 1, out _)));

Probe("zero scaling remains exact", () =>
    Require(TryScaleExactly(ExactRational.Zero, 100, out var value) && value == 0));

Probe("nonpositive factor refuses", () =>
    ExpectThrows<ArgumentOutOfRangeException>(() =>
        TryScaleExactly(ExactRational.One, 0, out _)));

Probe("exact descriptive sum is rational", () =>
    Require(DescriptiveStatistics64.AnalyzeExact(
        [new ExactRational(1, 3), new ExactRational(1, 6)]).Sum == new ExactRational(1, 2)));

Probe("exact descriptive mean is rational", () =>
    Require(DescriptiveStatistics64.AnalyzeExact(
        [new ExactRational(1, 3), new ExactRational(1, 6)]).Mean == new ExactRational(1, 4)));

Probe("exact Type-7 quarter is five halves", () =>
    Require(QuantileStatistics64.EvaluateExact(
        [ExactRational.Zero, new ExactRational(10, 1)],
        new ExactRational(1, 4), QuantileConvention64.Type7) == new ExactRational(5, 2)));

Probe("exact nearest quarter selects first observation", () =>
    Require(QuantileStatistics64.EvaluateExact(
        [ExactRational.Zero, new ExactRational(10, 1)],
        new ExactRational(1, 4), QuantileConvention64.NearestRank) == ExactRational.Zero));

Probe("finite Type-7 is named and interpolated", () =>
    Require(QuantileStatistics64.Evaluate(
        [0d, 10d], .25d, QuantileConvention64.Type7) == 2.5d));

Probe("finite nearest rank remains distinct", () =>
    Require(QuantileStatistics64.Evaluate(
        [0d, 10d], .25d, QuantileConvention64.NearestRank) == 0d));

Probe("consumer snapshot retains convention and exact sum", () =>
{
    var input = new[] { ExactRational.Zero, new ExactRational(10, 1) };
    var snapshot = new ConsumerStatisticsSnapshot(
        "two-observations", QuantileConvention64.Type7,
        DescriptiveStatistics64.AnalyzeExact(input).Sum,
        QuantileStatistics64.EvaluateExact(input,
            new ExactRational(1, 4), QuantileConvention64.Type7));
    Require(snapshot.Convention == QuantileConvention64.Type7 &&
        snapshot.Sum == new ExactRational(10, 1) &&
        snapshot.Quantile == new ExactRational(5, 2));
});

Probe("compensated finite sum preserves a small term", () =>
    Require(DescriptiveStatistics64.Analyze([1e16, 1d, -1e16]).Sum == 1d));

Probe("population and sample variances stay distinct", () =>
{
    var result = DescriptiveStatistics64.Analyze([1d, 2d, 3d, 4d]);
    Require(result.PopulationVariance == 1.25d &&
        global::System.Math.Abs(result.SampleVariance!.Value - 5d / 3d) < 1e-12);
});

Probe("finite statistics report approximate precision", () =>
    Require(DescriptiveStatistics64.Analyze([1d, 3d]).Precision ==
        DescriptivePrecisionKind.FiniteBinary64));

Probe("affine Pearson is defined", () =>
{
    var result = CorrelationStatistics64.AnalyzeWithStatus([1d, 2d, 3d], [2d, 4d, 6d]);
    Require(result.IsDefined && result.Status == CorrelationDefinitionStatus64.Defined &&
        result.PearsonCorrelation.HasValue &&
        global::System.Math.Abs(result.PearsonCorrelation.Value - 1d) < 1e-12);
});

Probe("constant Pearson is typed undefined not zero", () =>
{
    var result = CorrelationStatistics64.AnalyzeWithStatus([2d, 2d, 2d], [1d, 2d, 3d]);
    Require(result.Status == CorrelationDefinitionStatus64.UndefinedZeroVariance &&
        result.PearsonCorrelation is null && !result.IsDefined);
});

Probe("opt-in pair-skip preserves source counts", () =>
{
    var result = CorrelationStatistics64.AnalyzeWithStatus(
        [(double?)1d, null, 3d], [(double?)2d, 4d, 6d],
        MissingPairPolicy64.SkipPair);
    Require(result.ProvidedPairs == 3 && result.IncludedPairs == 2 &&
        result.MissingPairs == 1 && result.PearsonCorrelation.HasValue);
});

Probe("default null policy refuses missing", () =>
    ExpectThrows<ArgumentException>(() =>
        CorrelationStatistics64.AnalyzeWithStatus(
            [(double?)1d, null], [(double?)2d, 4d])));

Probe("Bernoulli p=1 quantile preserves positive-mass support", () =>
    Require(new BernoulliDistribution64(1d).Quantile(0d) == 1));

Probe("Bernoulli p=0 quantile preserves positive-mass support", () =>
    Require(new BernoulliDistribution64(0d).Quantile(1d) == 0));

Probe("degenerate Bernoulli consumes exactly one raw word", () =>
{
    var random = new SplitMix64Random(42UL);
    Require(new BernoulliDistribution64(1d).Sample(random) == 1 && random.Counter == 1UL);
});

Probe("seed-zero generator matches hardcoded golden word", () =>
    Require(new SplitMix64Random(0UL).NextUInt64() == 0xe220a8397b1dcdafUL));

Probe("canonical checkpoint replays golden suffix", () =>
{
    var random = new SplitMix64Random(42UL);
    for (var i = 0; i < 4; i++) random.NextUInt64();
    var encoded = SplitMix64PositionCodec.Format(random.Snapshot());
    Require(encoded == "splitmix64/v1|000000000000002a|0000000000000004");
    var resumed = SplitMix64Random.FromSnapshot(SplitMix64PositionCodec.Parse(encoded));
    var fifth = random.NextUInt64();
    Require(fifth == 0x09bc585a244823f2UL && resumed.NextUInt64() == fifth);
    Require(resumed.NextUInt64() == random.NextUInt64());
});

Probe("uniform samples stay in finite half-open interval", () =>
{
    var distribution = new ContinuousUniformDistribution64(-2d, 6d);
    var random = new SplitMix64Random(123UL);
    for (var i = 0; i < 128; i++)
    {
        var value = distribution.Sample(random);
        Require(double.IsFinite(value) && value >= -2d && value < 6d);
    }
    Require(random.Counter == 128UL);
});

Probe("exact and approximate public entry points stay distinct", () =>
    Require(typeof(ExactRational) != typeof(double) &&
        typeof(QuantileStatistics64).Assembly.GetName().Name == "dw.tools.math.statistics"));

Probe("package assemblies have no broker or provider reference", () =>
{
    var names = new[] { typeof(ExactRational).Assembly, typeof(DescriptiveStatistics64).Assembly }
        .SelectMany(a => a.GetReferencedAssemblies().Select(r => r.Name ?? string.Empty)).ToArray();
    Require(!names.Any(n => n.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
        n.Contains("Decision", StringComparison.OrdinalIgnoreCase) ||
        n.Contains("MathNet", StringComparison.OrdinalIgnoreCase)));
});

if (passed != 31)
    throw new InvalidOperationException($"Expected 31 independent consumer oracles; executed {passed}.");
Console.WriteLine($"dw.tools.math/c04-isolated-consumer/0.4 qualified; probes={passed}");

static bool TryScaleExactly(ExactRational amount, long positiveScale, out long scaled)
{
    if (positiveScale <= 0)
        throw new ArgumentOutOfRangeException(nameof(positiveScale));
    var exact = amount * new ExactRational(positiveScale, BigInteger.One);
    if (!exact.Denominator.IsOne ||
        exact.Numerator < long.MinValue ||
        exact.Numerator > long.MaxValue)
    {
        scaled = default;
        return false;
    }
    scaled = (long)exact.Numerator;
    return true;
}

static void Require(bool condition)
{
    if (!condition)
        throw new InvalidOperationException("Independent hand-derived oracle was not satisfied.");
}

static void ExpectThrows<T>(Action action) where T : Exception
{
    try { action(); }
    catch (Exception exception) when (exception is T) { return; }
    throw new InvalidOperationException("Expected " + typeof(T).Name + " was not observed.");
}

internal readonly record struct ConsumerStatisticsSnapshot(
    string SampleId, QuantileConvention64 Convention, ExactRational Sum, ExactRational Quantile);
