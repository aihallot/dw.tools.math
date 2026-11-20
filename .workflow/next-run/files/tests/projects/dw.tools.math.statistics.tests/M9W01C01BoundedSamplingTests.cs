using System;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M9W01C01BoundedSamplingTests
{
    [TestMethod]
    public void VersionedTransformAndLegacyRawVectorRemainStable()
    {
        Assert.AreEqual("splitmix64-bounded-rejection/v1", SplitMix64Random.BoundedTransformId);
        var random = new SplitMix64Random(0UL);
        Assert.AreEqual(0xe220a8397b1dcdafUL, random.NextUInt64());
        Assert.AreEqual(0x6e789e6aa1b965f4UL, random.NextUInt64());
        Assert.AreEqual(2UL, random.Counter);
    }

    [TestMethod]
    public void PublishedSeedZeroGoldenWordsMapToIndependentResidues()
    {
        var random = new SplitMix64Random(0UL);
        ulong[] expected = [1UL, 0UL, 1UL, 1UL, 1UL];
        foreach (var item in expected)
            Assert.AreEqual(item, random.NextBoundedUInt64(3UL, 1));
        Assert.AreEqual(5UL, random.Counter);
    }

    [TestMethod]
    public void SmallBoundsAndUnitBoundConsumeExactlyOneRawWord()
    {
        foreach (var upper in new ulong[] { 1UL, 2UL, 3UL, 4UL, 24UL })
        {
            var g = new SplitMix64Random(0UL);
            for (var i = 0; i < 32; i++)
                Assert.IsTrue(g.NextBoundedUInt64(upper, 100) < upper);
            Assert.IsTrue(g.Counter >= 32UL);
        }
        var singleton = new SplitMix64Random(5UL);
        Assert.AreEqual(0UL, singleton.NextBoundedUInt64(1UL, 1));
        Assert.AreEqual(1UL, singleton.Counter);
    }

    [TestMethod]
    public void ReducedDomainRejectionHasExactlyEqualResiduePreimages()
    {
        // Independent exhaustive 8-bit oracle for the same rejection principle.
        // The accepted domain size 256 - (256 % bound) is divisible by bound.
        for (var bound = 1; bound <= 255; bound++)
        {
            var counts = new int[bound];
            var rejected = 256 % bound;
            for (var raw = 0; raw < 256; raw++)
                if (raw >= rejected)
                    counts[raw % bound]++;
            var expected = (256 - rejected) / bound;
            foreach (var count in counts)
                Assert.AreEqual(expected, count, $"Biased residue for bound {bound}.");
        }
    }

    [TestMethod]
    public void RejectedWordsAdvanceCounterAndExhaustBoundedBudget()
    {
        const ulong difficultBound = 0x8000000000000001UL;
        var source = new SplitMix64Random(0UL);
        _ = source.NextUInt64(); // Published second and third words fall below threshold.
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            source.NextBoundedUInt64(difficultBound, 1));
        Assert.AreEqual(2UL, source.Counter);
        // Third raw word rejects, fourth accepts with a larger explicit budget.
        var expected = 0xf88bb8a8724c81ecUL % difficultBound;
        Assert.AreEqual(expected, source.NextBoundedUInt64(difficultBound, 2));
        Assert.AreEqual(4UL, source.Counter);
    }

    [TestMethod]
    public void FailedValidationDoesNotAdvanceGenerator()
    {
        var g = new SplitMix64Random(42UL);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => g.NextBoundedUInt64(0UL, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => g.NextBoundedUInt64(3UL, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => g.NextInt64Inclusive(4L, 2L, 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => g.NextInt64Inclusive(2L, 4L, -1));
        Assert.AreEqual(0UL, g.Counter);
    }

    [TestMethod]
    public void InclusiveRangesHandleSmallAndExtremeValuesWithoutOverflow()
    {
        var g = new SplitMix64Random(0UL);
        for (var i = 0; i < 128; i++)
        {
            var x = g.NextInt64Inclusive(2L, 4L, 64);
            Assert.IsTrue(x is >= 2L and <= 4L);
            var y = g.NextInt64Inclusive(2L, 24L, 64);
            Assert.IsTrue(y is >= 2L and <= 24L);
        }
        var one = new SplitMix64Random(0UL);
        Assert.AreEqual(long.MinValue, one.NextInt64Inclusive(long.MinValue, long.MinValue, 1));
        Assert.AreEqual(long.MaxValue, one.NextInt64Inclusive(long.MaxValue, long.MaxValue, 1));
        var full = new SplitMix64Random(0UL);
        Assert.AreEqual(unchecked((long)0xe220a8397b1dcdafUL),
            full.NextInt64Inclusive(long.MinValue, long.MaxValue, 1));
        Assert.AreEqual(1UL, full.Counter);
        var edge = new SplitMix64Random(17UL);
        for (var i = 0; i < 20; i++)
        {
            var value = edge.NextInt64Inclusive(long.MinValue, long.MaxValue - 1L, 100);
            Assert.IsTrue(value < long.MaxValue);
        }
    }

    [TestMethod]
    public void CheckpointReplayRestoresAcceptedAndRejectedRawSequence()
    {
        var original = new SplitMix64Random(0UL);
        original.NextBoundedUInt64(24UL, 1);
        var checkpoint = SplitMix64PositionCodec.Format(original.Snapshot());
        var resumed = SplitMix64Random.FromSnapshot(SplitMix64PositionCodec.Parse(checkpoint));
        const ulong bound = 0x8000000000000001UL;
        Assert.AreEqual(original.NextBoundedUInt64(bound, 4), resumed.NextBoundedUInt64(bound, 4));
        Assert.AreEqual(original.Counter, resumed.Counter);
        Assert.AreEqual(original.NextBoundedUInt64(3UL, 4), resumed.NextBoundedUInt64(3UL, 4));
        Assert.AreEqual(original.Snapshot(), resumed.Snapshot());
    }

    [TestMethod]
    public void CounterExhaustionAndIncompatibleCheckpointRefuse()
    {
        var exhausted = new SplitMix64Random(0UL, ulong.MaxValue);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            exhausted.NextBoundedUInt64(1UL, 1));
        Assert.AreEqual(ulong.MaxValue, exhausted.Counter);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            exhausted.NextInt64Inclusive(long.MinValue, long.MaxValue, 1));
        Assert.ThrowsExactly<ArgumentException>(() =>
            SplitMix64Random.FromSnapshot(new SplitMix64Position("other/v1", 0UL, 0UL)));
    }

    [TestMethod]
    public void InclusiveSelectionPreservesOneRawCounterStream()
    {
        var mixed = new SplitMix64Random(1UL);
        var words = new SplitMix64Random(1UL);
        mixed.NextInt64Inclusive(2L, 4L, 1);
        words.NextUInt64();
        Assert.AreEqual(words.NextUInt64(), mixed.NextUInt64());
        Assert.AreEqual(2UL, mixed.Counter);
    }
}
