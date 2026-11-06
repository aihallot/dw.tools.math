using System.Security.Cryptography;
using Dw.Tools.Math.Statistics;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("Expected exact packaged Statistics DLL SHA-256.");

var passed = 0;
void Probe(string id, Action assertion)
{
    try
    {
        assertion();
        passed++;
        Console.WriteLine("MATH-STATISTICS-CHECK|" + id + "|PASS");
    }
    catch (Exception error)
    {
        throw new InvalidOperationException("Statistics binary oracle failed: " + id, error);
    }
}
static void Need(bool okay, string message)
{
    if (!okay) throw new InvalidOperationException(message);
}
static void Near(double actual, double expected)
{
    Need(double.IsFinite(actual) && Math.Abs(actual - expected) <= 1e-10,
        "Expected an independently calculated finite result.");
}
static void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new InvalidOperationException("Expected a typed refusal: " + typeof(T).Name);
}

Probe("loaded-package-sha256", () =>
{
    var assembly = typeof(DescriptiveStatistics64).Assembly;
    Need(assembly.GetName().Name == "dw.tools.math.statistics", "Incorrect loaded assembly identity.");
    Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(assembly.Location))) == args[0],
        "The actually loaded Statistics DLL differs from the packaged DLL.");
});
Probe("descriptive-mean", () =>
{
    var result = DescriptiveStatistics64.Analyze(new double[] { 1d, 2d, 3d });
    Need(result.Count == 3 && result.Sum == 6d && result.Minimum == 1d &&
         result.Maximum == 3d, "Descriptive aggregate or bounds differ.");
    Near(result.Mean, 2d);
});
Probe("descriptive-variances", () =>
{
    var result = DescriptiveStatistics64.Analyze(new double[] { 1d, 2d, 3d });
    Near(result.PopulationVariance, 2d / 3d);
    Need(result.SampleVariance.HasValue, "Sample variance absent.");
    Near(result.SampleVariance.Value, 1d);
});
Probe("empty-sample-refusal", () =>
    Throws<ArgumentException>(() => DescriptiveStatistics64.Analyze(Array.Empty<double>())));
Probe("quantile-type-seven", () =>
    Near(QuantileStatistics64.Evaluate(
        new double[] { 30d, 0d, 20d, 10d }, 0.25d, QuantileConvention64.Type7), 7.5d));
Probe("quantile-nearest-rank", () =>
    Near(QuantileStatistics64.Evaluate(
        new double[] { 30d, 0d, 20d, 10d }, 0.25d, QuantileConvention64.NearestRank), 0d));
Probe("invalid-quantile-probability", () =>
    Throws<ArgumentOutOfRangeException>(() =>
        QuantileStatistics64.Evaluate(new double[] { 1d, 2d }, -0.1d, QuantileConvention64.Type7)));
Probe("paired-perfect-correlation", () =>
{
    var result = CorrelationStatistics64.Analyze(
        new double[] { 1d, 2d, 3d }, new double[] { 2d, 4d, 6d });
    Need(result.Count == 3, "Paired observation count differs.");
    Near(result.PearsonCorrelation, 1d);
    Near(result.PopulationCovariance, 4d / 3d);
});
Probe("unaligned-pairs-refusal", () =>
    Throws<ArgumentException>(() => CorrelationStatistics64.Analyze(
        new double[] { 1d, 2d, 3d }, new double[] { 1d, 2d })));
Probe("bernoulli-mass", () =>
{
    var bernoulli = new BernoulliDistribution64(0.25d);
    Near(bernoulli.ProbabilityMass(1), 0.25d);
    Near(bernoulli.ProbabilityMass(0), 0.75d);
    Near(bernoulli.ProbabilityMass(2), 0d);
});
Probe("bernoulli-quantile", () =>
{
    var bernoulli = new BernoulliDistribution64(0.25d);
    Need(bernoulli.Quantile(0.50d) == 0 && bernoulli.Quantile(0.90d) == 1,
        "Discrete inverse CDF differs.");
});
Probe("continuous-uniform", () =>
{
    var uniform = new ContinuousUniformDistribution64(2d, 6d);
    Near(uniform.ProbabilityDensity(3d), 0.25d);
    Near(uniform.CumulativeProbability(4d), 0.50d);
    Near(uniform.Quantile(0.25d), 3d);
});
Probe("invalid-uniform-bounds", () =>
    Throws<ArgumentOutOfRangeException>(() => new ContinuousUniformDistribution64(3d, 3d)));
Probe("frequency-weighted-mean", () =>
{
    var result = FrequencyWeightedStatistics64.Analyze(new[]
    {
        new FrequencyWeightedObservation64(2d, 1),
        new FrequencyWeightedObservation64(4d, 3)
    });
    Need(result.EffectiveCount == 4 && result.IncludedCount == 2 && result.MissingCount == 0,
        "Weighted row counts differ.");
    Near(result.Mean, 3.5d);
});
Probe("frequency-missing-skip", () =>
{
    var result = FrequencyWeightedStatistics64.Analyze(new[]
    {
        new FrequencyWeightedObservation64(null, 3),
        new FrequencyWeightedObservation64(2d, 1)
    }, MissingObservationPolicy64.Skip);
    Need(result.MissingCount == 1 && result.IncludedCount == 1 && result.EffectiveCount == 1,
        "Explicit missing-value handling differs.");
    Near(result.Mean, 2d);
});
Probe("splitmix-checkpoint-replay", () =>
{
    var generator = new SplitMix64Random(42UL);
    _ = generator.NextUInt64();
    var saved = generator.Snapshot();
    var resumed = SplitMix64Random.FromSnapshot(saved);
    Need(saved.Counter == 1UL && generator.NextUInt64() == resumed.NextUInt64() &&
         generator.Counter == 2UL, "Seeded portable replay drifted.");
});
Probe("splitmix-position-codec", () =>
{
    var position = new SplitMix64Position(SplitMix64Random.AlgorithmId, 1UL, 1UL);
    var text = SplitMix64PositionCodec.Format(position);
    Need(text == "splitmix64/v1|0000000000000001|0000000000000001" &&
         SplitMix64PositionCodec.Parse(text) == position,
         "Canonical versioned checkpoint differs.");
});
Probe("splitmix-noncanonical-refusal", () =>
    Throws<FormatException>(() =>
        SplitMix64PositionCodec.Parse("splitmix64/v1|000000000000000A|0000000000000001")));

Need(passed == 18, "Exactly eighteen independent Statistics package binary oracles required.");
Console.WriteLine("MATH-STATISTICS-BINARY|PASS|18");
