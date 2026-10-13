using System;
using System.Linq;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C03T1Tests
{
    private static void Near(double actual, double expected, double tolerance = 1e-15)
    {
        Assert.IsTrue(double.IsFinite(actual), "A distribution result must remain finite.");
        Assert.IsTrue(global::System.Math.Abs(actual - expected) <= tolerance,
            $"Expected {expected:R}, observed {actual:R}.");
    }

    [TestMethod]
    public void SeedZeroMatchesIndependentFiveWordReferenceVector()
    {
        var generator = new SplitMix64Random(0UL);
        ulong[] expected =
        [
            0xe220a8397b1dcdafUL, 0x6e789e6aa1b965f4UL,
            0x06c45d188009454fUL, 0xf88bb8a8724c81ecUL,
            0x1b39896a51a8749bUL
        ];
        foreach (var word in expected)
            Assert.AreEqual(word, generator.NextUInt64());
        Assert.AreEqual(5UL, generator.Counter);
    }

    [TestMethod]
    public void SeedOneMatchesIndependentFiveWordReferenceVector()
    {
        var generator = new SplitMix64Random(1UL);
        ulong[] expected =
        [
            0x910a2dec89025cc1UL, 0xbeeb8da1658eec67UL,
            0xf893a2eefb32555eUL, 0x71c18690ee42c90bUL,
            0x71bb54d8d101b5b9UL
        ];
        foreach (var word in expected)
            Assert.AreEqual(word, generator.NextUInt64());
    }

    [TestMethod]
    public void SeedFortyTwoMatchesPublishedFiveWordReferenceVector()
    {
        var generator = new SplitMix64Random(42UL);
        ulong[] expected =
        [
            0xbdd732262feb6e95UL, 0x28efe333b266f103UL,
            0x47526757130f9f52UL, 0x581ce1ff0e4ae394UL,
            0x09bc585a244823f2UL
        ];
        foreach (var word in expected)
            Assert.AreEqual(word, generator.NextUInt64());
    }

    [TestMethod]
    public void SameSeedProducesIdenticalByteIndependentStreams()
    {
        var left = new SplitMix64Random(1234567UL);
        var right = new SplitMix64Random(1234567UL);
        for (var i = 0; i < 128; i++)
            Assert.AreEqual(left.NextUInt64(), right.NextUInt64());
    }

    [TestMethod]
    public void DifferentSeedsStartAtDistinctPublishedValues()
    {
        var zero = new SplitMix64Random(0UL);
        var one = new SplitMix64Random(1UL);
        Assert.AreNotEqual(zero.NextUInt64(), one.NextUInt64());
    }

    [TestMethod]
    public void SeedAndCounterConstructorReproducesExactStreamSuffix()
    {
        var original = new SplitMix64Random(42UL);
        for (var i = 0; i < 100; i++)
            original.NextUInt64();
        var resumed = new SplitMix64Random(42UL, 100UL);
        Assert.AreEqual(original.NextUInt64(), resumed.NextUInt64());
        Assert.AreEqual(101UL, resumed.Counter);
    }

    [TestMethod]
    public void SnapshotRestoresNextDrawRatherThanLastDraw()
    {
        var generator = new SplitMix64Random(17UL);
        for (var i = 0; i < 7; i++)
            generator.NextUInt64();
        var snapshot = generator.Snapshot();
        var restored = SplitMix64Random.FromSnapshot(snapshot);
        Assert.AreEqual(SplitMix64Random.AlgorithmId, snapshot.Algorithm);
        Assert.AreEqual(17UL, snapshot.Seed);
        Assert.AreEqual(7UL, snapshot.Counter);
        Assert.AreEqual(generator.NextUInt64(), restored.NextUInt64());
    }

    [TestMethod]
    public void UnsupportedSnapshotAlgorithmFailsClosed()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            SplitMix64Random.FromSnapshot(new SplitMix64Position("other/v1", 0UL, 0UL)));
    }

    [TestMethod]
    public void MaximumCounterRefusesWithoutWrapping()
    {
        var generator = new SplitMix64Random(0UL, ulong.MaxValue);
        Assert.ThrowsExactly<InvalidOperationException>(() => generator.NextUInt64());
        Assert.AreEqual(ulong.MaxValue, generator.Counter);
    }

    [TestMethod]
    public void MaximumSeedIsSupportedUnderModularArithmetic()
    {
        var a = new SplitMix64Random(ulong.MaxValue);
        var b = new SplitMix64Random(ulong.MaxValue);
        Assert.AreEqual(a.NextUInt64(), b.NextUInt64());
    }

    [TestMethod]
    public void UnitIntervalOutputIsAlwaysBelowOne()
    {
        var generator = new SplitMix64Random(22UL);
        for (var i = 0; i < 4096; i++)
        {
            var x = generator.NextUnitInterval();
            Assert.IsTrue(double.IsFinite(x) && x >= 0d && x < 1d);
        }
        Assert.AreEqual(4096UL, generator.Counter);
    }

    [TestMethod]
    public void UnitIntervalUsesHigh53BitsOfReferenceWord()
    {
        var generator = new SplitMix64Random(0UL);
        const ulong first = 0xe220a8397b1dcdafUL;
        const double scale = 1d / 9007199254740992d;
        var expected = (first >> 11) * scale;
        Near(generator.NextUnitInterval(), expected, 0d);
        Assert.AreEqual(1UL, generator.Counter);
    }

    [TestMethod]
    public void UnitIntervalAndWordDrawsShareOneCounterAndStream()
    {
        var mixed = new SplitMix64Random(1UL);
        var words = new SplitMix64Random(1UL);
        mixed.NextUnitInterval();
        words.NextUInt64();
        Assert.AreEqual(words.NextUInt64(), mixed.NextUInt64());
        Assert.AreEqual(2UL, mixed.Counter);
    }

    [TestMethod]
    public void PublicAlgorithmIdentifierIsVersioned()
    {
        var algorithm = new SplitMix64Random(1UL).Snapshot().Algorithm;
        Assert.AreEqual("splitmix64/v1", algorithm);
    }

    [TestMethod]
    public void BernoulliZeroAndOneHaveExactProbabilityMass()
    {
        var zero = new BernoulliDistribution64(0d);
        var one = new BernoulliDistribution64(1d);
        Near(zero.ProbabilityMass(0), 1d, 0d);
        Near(zero.ProbabilityMass(1), 0d, 0d);
        Near(one.ProbabilityMass(0), 0d, 0d);
        Near(one.ProbabilityMass(1), 1d, 0d);
    }

    [TestMethod]
    public void BernoulliOutsideSupportHasZeroProbabilityMass()
    {
        var distribution = new BernoulliDistribution64(.25d);
        Near(distribution.ProbabilityMass(-4), 0d, 0d);
        Near(distribution.ProbabilityMass(2), 0d, 0d);
        Near(distribution.ProbabilityMass(0), .75d);
        Near(distribution.ProbabilityMass(1), .25d);
    }

    [TestMethod]
    public void IntegerCdfMatchesIndependentBernoulliStepFunction()
    {
        var distribution = new BernoulliDistribution64(.25d);
        Near(distribution.CumulativeProbability(-1), 0d, 0d);
        Near(distribution.CumulativeProbability(0), .75d, 0d);
        Near(distribution.CumulativeProbability(1), 1d, 0d);
        Near(distribution.CumulativeProbability(10), 1d, 0d);
    }

    [TestMethod]
    public void BernoulliBoundaryCdfsAreExact()
    {
        var zero = new BernoulliDistribution64(0d);
        var one = new BernoulliDistribution64(1d);
        Near(zero.CumulativeProbability(0), 1d, 0d);
        Near(one.CumulativeProbability(0), 0d, 0d);
        Near(zero.CumulativeProbability(1), 1d, 0d);
        Near(one.CumulativeProbability(1), 1d, 0d);
    }

    [TestMethod]
    public void InteriorBernoulliQuantileUsesInclusiveCdfThreshold()
    {
        var distribution = new BernoulliDistribution64(.25d);
        Assert.AreEqual(0, distribution.Quantile(0d));
        Assert.AreEqual(0, distribution.Quantile(.75d));
        Assert.AreEqual(1, distribution.Quantile(.875d));
        Assert.AreEqual(1, distribution.Quantile(1d));
    }

    [TestMethod]
    public void DegenerateBernoulliQuantilesSelectPositiveMassSupport()
    {
        var zero = new BernoulliDistribution64(0d);
        var one = new BernoulliDistribution64(1d);
        foreach (var q in new[] { 0d, .5d, 1d })
        {
            Assert.AreEqual(0, zero.Quantile(q));
            Assert.AreEqual(1, one.Quantile(q));
        }
    }

    [TestMethod]
    public void BernoulliCdfIsMonotoneAndBounded()
    {
        var distribution = new BernoulliDistribution64(.125d);
        var previous = 0d;
        foreach (var x in new[] { -100, -1, 0, 1, 2, 100 })
        {
            var value = distribution.CumulativeProbability(x);
            Assert.IsTrue(value >= previous && value >= 0d && value <= 1d);
            previous = value;
        }
    }

    [TestMethod]
    public void SamplingAtZeroProbabilityIsAlwaysZeroButConsumesDraws()
    {
        var distribution = new BernoulliDistribution64(0d);
        var generator = new SplitMix64Random(0UL);
        for (var i = 0; i < 128; i++)
            Assert.AreEqual(0, distribution.Sample(generator));
        Assert.AreEqual(128UL, generator.Counter);
    }

    [TestMethod]
    public void SamplingAtUnitProbabilityIsAlwaysOneButConsumesDraws()
    {
        var distribution = new BernoulliDistribution64(1d);
        var generator = new SplitMix64Random(0UL);
        for (var i = 0; i < 128; i++)
            Assert.AreEqual(1, distribution.Sample(generator));
        Assert.AreEqual(128UL, generator.Counter);
    }

    [TestMethod]
    public void HalfProbabilitySamplingMatchesIndependentSeedZeroGoldenBits()
    {
        var distribution = new BernoulliDistribution64(.5d);
        var generator = new SplitMix64Random(0UL);
        // High bit of each published raw golden word; threshold 0.5.
        int[] expected = [0, 1, 1, 0, 1];
        foreach (var sample in expected)
            Assert.AreEqual(sample, distribution.Sample(generator));
        Assert.AreEqual(5UL, generator.Counter);
    }

    [TestMethod]
    public void SameSeedAndDistributionReproduceSampleSequence()
    {
        var distribution = new BernoulliDistribution64(.3d);
        var first = new SplitMix64Random(9UL);
        var second = new SplitMix64Random(9UL);
        for (var i = 0; i < 128; i++)
            Assert.AreEqual(distribution.Sample(first), distribution.Sample(second));
    }

    [TestMethod]
    public void BoundarySamplesAdvanceTheSameStreamAsRawWordDraws()
    {
        var distribution = new BernoulliDistribution64(1d);
        var sampled = new SplitMix64Random(42UL);
        var raw = new SplitMix64Random(42UL);
        distribution.Sample(sampled);
        raw.NextUInt64();
        Assert.AreEqual(raw.NextUInt64(), sampled.NextUInt64());
    }

    [TestMethod]
    public void NullGeneratorIsRefused()
    {
        var distribution = new BernoulliDistribution64(.5d);
        Assert.ThrowsExactly<ArgumentNullException>(() => distribution.Sample(null!));
    }

    [TestMethod]
    public void NonfiniteAndOutOfRangeBernoulliParametersAreRefused()
    {
        foreach (var bad in new[] { double.NaN, double.PositiveInfinity,
            double.NegativeInfinity, -.01d, 1.01d })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new BernoulliDistribution64(bad));
    }

    [TestMethod]
    public void NonfiniteAndOutOfRangeQuantileArgumentsAreRefused()
    {
        var distribution = new BernoulliDistribution64(.3d);
        foreach (var bad in new[] { double.NaN, double.PositiveInfinity,
            double.NegativeInfinity, -.001d, 1.001d })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => distribution.Quantile(bad));
    }

    [TestMethod]
    public void BernoulliParameterIsImmutableAcrossSampling()
    {
        var distribution = new BernoulliDistribution64(.25d);
        var generator = new SplitMix64Random(5UL);
        for (var i = 0; i < 32; i++)
            distribution.Sample(generator);
        Near(distribution.ProbabilityOfOne, .25d, 0d);
    }

    [TestMethod]
    public void DistributionAssemblyRemainsProviderAndAgentNeutral()
    {
        var references = typeof(BernoulliDistribution64).Assembly
            .GetReferencedAssemblies().Select(x => x.Name ?? string.Empty);
        Assert.IsFalse(references.Any(x =>
            x.Contains("MathNet", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Decision", StringComparison.OrdinalIgnoreCase)));
    }
}
