using System;
using System.Linq;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C03T2Tests
{
    private static void Near(double observed, double expected, double tolerance = 1e-14)
    {
        Assert.IsTrue(double.IsFinite(observed), "A finite binary64 observation is required.");
        Assert.IsTrue(global::System.Math.Abs(observed - expected) <= tolerance,
            $"Expected {expected:R}; observed {observed:R}; tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void FiniteUniformDensityHasExplicitHalfOpenConvention()
    {
        var law = new ContinuousUniformDistribution64(-2d, 6d);
        Near(law.ProbabilityDensity(-2d), .125d, 0d);
        Near(law.ProbabilityDensity(2d), .125d, 0d);
        Near(law.ProbabilityDensity(6d), 0d, 0d);
        Near(law.ProbabilityDensity(-3d), 0d, 0d);
    }

    [TestMethod]
    public void UniformCdfMatchesIndependentQuarterAndMidpointHandOracles()
    {
        var law = new ContinuousUniformDistribution64(-2d, 6d);
        Near(law.CumulativeProbability(0d), .25d, 0d);
        Near(law.CumulativeProbability(2d), .5d, 0d);
        Near(law.CumulativeProbability(4d), .75d, 0d);
    }

    [TestMethod]
    public void UniformCdfSaturatesExactlyOutsideSupport()
    {
        var law = new ContinuousUniformDistribution64(-2d, 6d);
        Near(law.CumulativeProbability(-10d), 0d, 0d);
        Near(law.CumulativeProbability(-2d), 0d, 0d);
        Near(law.CumulativeProbability(6d), 1d, 0d);
        Near(law.CumulativeProbability(20d), 1d, 0d);
    }

    [TestMethod]
    public void ContinuousQuantileAcceptsBothMathematicalEndpoints()
    {
        var law = new ContinuousUniformDistribution64(-2d, 6d);
        Near(law.Quantile(0d), -2d, 0d);
        Near(law.Quantile(.5d), 2d, 0d);
        Near(law.Quantile(1d), 6d, 0d);
    }

    [TestMethod]
    public void UniformQuantileMatchesIndependentAffineQuartiles()
    {
        var law = new ContinuousUniformDistribution64(10d, 18d);
        Near(law.Quantile(.25d), 12d, 0d);
        Near(law.Quantile(.75d), 16d, 0d);
    }

    [TestMethod]
    public void UniformDensityIsOrderIndependent()
    {
        var law = new ContinuousUniformDistribution64(0d, 8d);
        var inputs = new[] { -1d, 0d, 4d, 8d, 9d };
        Near(inputs.Sum(law.ProbabilityDensity),
            inputs.Reverse().Sum(law.ProbabilityDensity), 0d);
    }

    [TestMethod]
    public void UniformRejectsEqualReversedAndNonfiniteEndpoints()
    {
        foreach (var bounds in new[]
        {
            (0d, 0d), (1d, 0d), (double.NaN, 1d),
            (0d, double.NaN), (double.NegativeInfinity, 1d),
            (0d, double.PositiveInfinity)
        })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new ContinuousUniformDistribution64(bounds.Item1, bounds.Item2));
    }

    [TestMethod]
    public void WidthBeyondBinary64CannotBeAdmitted()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            new ContinuousUniformDistribution64(-double.MaxValue, double.MaxValue));
    }

    [TestMethod]
    public void SubnormalWidthWithOverflowingDensityIsRefused()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            new ContinuousUniformDistribution64(0d, double.Epsilon));
    }

    [TestMethod]
    public void UniformQuantileRequiresFiniteUnitProbability()
    {
        var law = new ContinuousUniformDistribution64(0d, 1d);
        foreach (var bad in new[] { double.NaN, double.NegativeInfinity,
            double.PositiveInfinity, -.1d, 1.1d })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => law.Quantile(bad));
    }

    [TestMethod]
    public void UniformCdfAndDensityRefuseNonfiniteArguments()
    {
        var law = new ContinuousUniformDistribution64(-1d, 1d);
        foreach (var bad in new[] { double.NaN, double.PositiveInfinity,
            double.NegativeInfinity })
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => law.CumulativeProbability(bad));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => law.ProbabilityDensity(bad));
        }
    }

    [TestMethod]
    public void UniformSampleMatchesIndependentHigh53BitSeedZeroOracle()
    {
        var law = new ContinuousUniformDistribution64(0d, 1d);
        var generator = new SplitMix64Random(0UL);
        const ulong raw = 0xe220a8397b1dcdafUL;
        const double scale = 1d / 9007199254740992d;
        Near(law.Sample(generator), (raw >> 11) * scale, 0d);
        Assert.AreEqual(1UL, generator.Counter);
    }

    [TestMethod]
    public void UniformSampleIsReproducibleAcrossIndependentGenerators()
    {
        var law = new ContinuousUniformDistribution64(-4d, 8d);
        var first = new SplitMix64Random(42UL);
        var second = new SplitMix64Random(42UL);
        for (var i = 0; i < 128; i++)
            Near(law.Sample(first), law.Sample(second), 0d);
        Assert.AreEqual(128UL, first.Counter);
    }

    [TestMethod]
    public void UniformSamplingAlwaysRemainsFiniteHalfOpen()
    {
        var law = new ContinuousUniformDistribution64(-2d, 6d);
        var generator = new SplitMix64Random(123UL);
        for (var i = 0; i < 4096; i++)
        {
            var value = law.Sample(generator);
            Assert.IsTrue(double.IsFinite(value) && value >= -2d && value < 6d);
        }
        Assert.AreEqual(4096UL, generator.Counter);
    }

    [TestMethod]
    public void RoundedSampleUpperEndpointIsClampedBelowMaximum()
    {
        var low = 1e16;
        var high = low + 2d;
        var law = new ContinuousUniformDistribution64(low, high);
        var observed = law.Sample(new SplitMix64Random(0UL));
        Assert.IsTrue(observed >= low && observed < high);
        Near(observed, low, 0d);
    }

    [TestMethod]
    public void UniformSampleRefusesMissingGenerator()
    {
        var law = new ContinuousUniformDistribution64(0d, 1d);
        Assert.ThrowsExactly<ArgumentNullException>(() => law.Sample(null!));
    }

    [TestMethod]
    public void UniformAndBernoulliMixedDrawsPreserveOneSharedCounter()
    {
        var uniform = new ContinuousUniformDistribution64(-3d, 3d);
        var bernoulli = new BernoulliDistribution64(.5d);
        var mixed = new SplitMix64Random(42UL);
        var words = new SplitMix64Random(42UL);
        uniform.Sample(mixed);
        words.NextUInt64();
        bernoulli.Sample(mixed);
        words.NextUInt64();
        Assert.AreEqual(words.NextUInt64(), mixed.NextUInt64());
        Assert.AreEqual(3UL, mixed.Counter);
    }

    [TestMethod]
    public void ExhaustedCounterCannotBeBypassedByUniformSample()
    {
        var law = new ContinuousUniformDistribution64(0d, 1d);
        var generator = new SplitMix64Random(7UL, ulong.MaxValue);
        Assert.ThrowsExactly<InvalidOperationException>(() => law.Sample(generator));
        Assert.AreEqual(ulong.MaxValue, generator.Counter);
    }

    [TestMethod]
    public void PositionCodecFormatsExactlyTwoLowercaseUnsignedHexWords()
    {
        var position = new SplitMix64Position(SplitMix64Random.AlgorithmId,
            0xabcdef1234567890UL, 0x1aUL);
        var encoded = SplitMix64PositionCodec.Format(position);
        Assert.AreEqual("splitmix64/v1|abcdef1234567890|000000000000001a", encoded);
    }

    [TestMethod]
    public void PositionCodecParsesIndependentExternalLiteral()
    {
        var position = SplitMix64PositionCodec.Parse(
            "splitmix64/v1|000000000000002a|0000000000000005");
        Assert.AreEqual(SplitMix64Random.AlgorithmId, position.Algorithm);
        Assert.AreEqual(42UL, position.Seed);
        Assert.AreEqual(5UL, position.Counter);
    }

    [TestMethod]
    public void CodecRoundTripReproducesTheNextGoldenWord()
    {
        var source = new SplitMix64Random(42UL);
        for (var i = 0; i < 4; i++) source.NextUInt64();
        var encoded = SplitMix64PositionCodec.Format(source.Snapshot());
        var restored = SplitMix64Random.FromSnapshot(SplitMix64PositionCodec.Parse(encoded));
        var fifth = restored.NextUInt64();
        Assert.AreEqual(0x09bc585a244823f2UL, fifth);
        Assert.AreEqual(source.NextUInt64(), fifth);
        Assert.AreEqual(source.NextUInt64(), restored.NextUInt64());
    }

    [TestMethod]
    public void CodecRoundTripRetainsMaximumUnsignedPosition()
    {
        var original = new SplitMix64Position(SplitMix64Random.AlgorithmId,
            ulong.MaxValue, ulong.MaxValue);
        var parsed = SplitMix64PositionCodec.Parse(SplitMix64PositionCodec.Format(original));
        Assert.AreEqual(original, parsed);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            SplitMix64Random.FromSnapshot(parsed).NextUInt64());
    }

    [TestMethod]
    public void CodecRejectsUnsupportedAlgorithmOnEitherBoundary()
    {
        Assert.ThrowsExactly<FormatException>(() =>
            SplitMix64PositionCodec.Parse(
                "other/v1|0000000000000000|0000000000000000"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            SplitMix64PositionCodec.Format(new SplitMix64Position("other/v1", 0UL, 0UL)));
    }

    [TestMethod]
    public void CodecRejectsNoncanonicalUppercaseHex()
    {
        Assert.ThrowsExactly<FormatException>(() =>
            SplitMix64PositionCodec.Parse(
                "splitmix64/v1|ABCDEF1234567890|0000000000000000"));
    }

    [TestMethod]
    public void CodecRejectsWhitespaceInvalidDigitsAndTruncation()
    {
        foreach (var value in new[]
        {
            "splitmix64/v1|0|0000000000000000",
            " splitmix64/v1|0000000000000000|0000000000000000",
            "splitmix64/v1|0000000000000000|0000000000000000 ",
            "splitmix64/v1|0000000000000000|00000000000000g0"
        })
            Assert.ThrowsExactly<FormatException>(() => SplitMix64PositionCodec.Parse(value));
    }

    [TestMethod]
    public void CodecRejectsMissingAndExtraSegments()
    {
        foreach (var value in new[]
        {
            "", "splitmix64/v1",
            "splitmix64/v1|0000000000000000",
            "splitmix64/v1|0000000000000000|0000000000000000|extra"
        })
            Assert.ThrowsExactly<FormatException>(() => SplitMix64PositionCodec.Parse(value));
    }

    [TestMethod]
    public void CodecRejectsNullInput()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => SplitMix64PositionCodec.Parse(null!));
    }

    [TestMethod]
    public void BernoulliDegenerateQuantilesRemainAtPositiveMassSupport()
    {
        var zero = new BernoulliDistribution64(0d);
        var one = new BernoulliDistribution64(1d);
        foreach (var probability in new[] { 0d, .5d, 1d })
        {
            Assert.AreEqual(0, zero.Quantile(probability));
            Assert.AreEqual(1, one.Quantile(probability));
        }
    }

    [TestMethod]
    public void BernoulliDegenerateSamplingStillConsumesOneWord()
    {
        var law = new BernoulliDistribution64(0d);
        var generator = new SplitMix64Random(1UL);
        Assert.AreEqual(0UL, generator.Counter);
        Assert.AreEqual(0, law.Sample(generator));
        Assert.AreEqual(1UL, generator.Counter);
    }

    [TestMethod]
    public void DistributionAssemblyHasNoProviderAgentOrDecisionDependency()
    {
        var names = typeof(ContinuousUniformDistribution64).Assembly
            .GetReferencedAssemblies().Select(x => x.Name ?? string.Empty);
        Assert.IsFalse(names.Any(x =>
            x.Contains("MathNet", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Decision", StringComparison.OrdinalIgnoreCase)));
    }
}
