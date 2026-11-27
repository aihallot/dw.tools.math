using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M11StreamingMomentsTests
{
    [TestMethod]
    public void EmptySingletonAndConstantStreamHaveExplicitVarianceSemantics()
    {
        var stream = new StreamingMoments64(4);
        Assert.AreEqual(new StreamingMomentsResult64(0, null, null, null), stream.GetResult());
        stream.Add(12d);
        Assert.AreEqual(new StreamingMomentsResult64(1, 12d, 0d, null), stream.GetResult());
        for (int i = 0; i < 3; i++) stream.Add(12d);
        Assert.AreEqual(new StreamingMomentsResult64(4, 12d, 0d, 0d), stream.GetResult());
        Assert.AreEqual(DescriptivePrecisionKind.FiniteBinary64, stream.GetResult().Precision);
    }

    [TestMethod]
    public void MalformedCheckpointsAreRejectedWithoutNormalization()
    {
        var empty = new StreamingMoments64(8).Snapshot();
        Assert.ThrowsExactly<ArgumentNullException>(() => StreamingMoments64.FromSnapshot(null!));
        StreamingMomentsSnapshot64[] invalid = [
            empty with { Algorithm = "unknown/v2" },
            empty with { MaxObservations = 0 },
            empty with { MaxObservations = StreamingMoments64.MaximumObservations + 1 },
            empty with { Count = -1 },
            empty with { Count = 9 },
            empty with { Origin = 1d },
            empty with { MeanOffset = BitConverter.Int64BitsToDouble(long.MinValue) },
            empty with { M2 = 1d },
            empty with { Count = 1, MeanOffset = 1d },
            empty with { Count = 1, M2 = 1d },
            empty with { Count = 2, M2 = -1d },
            empty with { Count = 2, Origin = double.NaN },
            empty with { Count = 2, MeanOffset = double.PositiveInfinity },
            empty with { Count = 2, M2 = double.PositiveInfinity },
            empty with { Count = 2, Origin = double.MaxValue, MeanOffset = double.MaxValue }
        ];
        foreach (var snapshot in invalid)
            Assert.ThrowsExactly<ArgumentException>(() => StreamingMoments64.FromSnapshot(snapshot));
        Assert.AreEqual(empty, StreamingMoments64.FromSnapshot(empty).Snapshot());
    }

    [TestMethod]
    public void RefusalsAreAtomicAndNonfiniteInputPrecedesExhaustion()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new StreamingMoments64(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new StreamingMoments64(StreamingMoments64.MaximumObservations + 1));
        var stream = new StreamingMoments64(1);
        stream.Add(1d);
        var full = stream.Snapshot();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stream.Add(double.NaN));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stream.Add(double.NegativeInfinity));
        Assert.ThrowsExactly<InvalidOperationException>(() => stream.Add(2d));
        Assert.AreEqual(full, stream.Snapshot());

        var extreme = new StreamingMoments64(3);
        extreme.Add(double.MaxValue);
        var before = extreme.Snapshot();
        Assert.ThrowsExactly<ArithmeticException>(() => extreme.Add(-double.MaxValue));
        Assert.AreEqual(before, extreme.Snapshot());
        extreme.Add(double.MaxValue);
        Assert.AreEqual(new StreamingMomentsResult64(2, double.MaxValue, 0d, 0d), extreme.GetResult());
    }

    [TestMethod]
    public void CheckpointReplayPreservesOrderedStateAndBudget()
    {
        double[] values = [1e12 - 2d, 1e12 + 2d, 1e12 - 1d, 1e12 + 1d];
        var continuous = new StreamingMoments64(4);
        continuous.Add(values[0]);
        continuous.Add(values[1]);
        var checkpoint = continuous.Snapshot();
        var resumed = StreamingMoments64.FromSnapshot(checkpoint);
        foreach (double value in values.Skip(2))
        {
            continuous.Add(value);
            resumed.Add(value);
        }
        Assert.AreEqual(continuous.Snapshot(), resumed.Snapshot());
        Assert.AreEqual(continuous.GetResult(), resumed.GetResult());
        Assert.AreEqual(1e12, resumed.GetResult().Mean!.Value);
        Assert.AreEqual(2.5d, resumed.GetResult().PopulationVariance!.Value, 1e-12);
        Assert.AreEqual(10d / 3d, resumed.GetResult().SampleVariance!.Value, 1e-12);
        Assert.AreEqual(2, checkpoint.Count);
        Assert.ThrowsExactly<InvalidOperationException>(() => resumed.Add(0d));
    }
}
