using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M11StreamingMomentsVerificationTests
{
    [TestMethod]
    public void VariedDyadicOffsetsAgreeWithIndependentIntegerMomentOracle()
    {
        const int count = 257;
        const double scale = 1d / 1024d;
        double[] origins = [0d, 1099511627776d, -1099511627776d];
        foreach (double origin in origins)
        {
            var stream = new StreamingMoments64(count);
            long sum = 0;
            long squares = 0;
            for (int i = 0; i < count; i++)
            {
                int deviation = (i * 37 % 19) - 9;
                sum += deviation;
                squares += deviation * deviation;
                stream.Add(origin + deviation * scale);
            }

            // Independent exact integer sufficient statistics, not Welford.
            long numerator = count * squares - sum * sum;
            double mean = origin + (double)sum / count * scale;
            double population = (double)numerator / (count * count) * scale * scale;
            double sample = (double)numerator / (count * (count - 1)) * scale * scale;
            var result = stream.GetResult();
            Assert.AreEqual(count, result.Count);
            Assert.AreEqual(mean, result.Mean!.Value, 0.00025d);
            Assert.AreEqual(population, result.PopulationVariance!.Value, 1e-15);
            Assert.AreEqual(sample, result.SampleVariance!.Value, 1e-15);
        }
    }

    [TestMethod]
    public void MultipleCheckpointBoundariesPreserveAllBinary64StateBits()
    {
        const int count = 257;
        int[] boundaries = [0, 1, 17, 128, 256, 257];
        var continuous = new StreamingMoments64(count);
        var resumed = new StreamingMoments64(count);
        for (int i = 0; i <= count; i++)
        {
            if (boundaries.Contains(i))
            {
                var checkpoint = resumed.Snapshot();
                resumed = StreamingMoments64.FromSnapshot(checkpoint);
                SameBits(checkpoint, resumed.Snapshot());
            }
            SameBits(continuous.Snapshot(), resumed.Snapshot());
            if (i == count) break;
            double value = 1e12 + ((i * 37 % 19) - 9) / 1024d;
            continuous.Add(value);
            resumed.Add(value);
        }
        var a = continuous.GetResult();
        var b = resumed.GetResult();
        Assert.AreEqual(a.Count, b.Count);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(a.Mean!.Value), BitConverter.DoubleToInt64Bits(b.Mean!.Value));
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(a.PopulationVariance!.Value), BitConverter.DoubleToInt64Bits(b.PopulationVariance!.Value));
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(a.SampleVariance!.Value), BitConverter.DoubleToInt64Bits(b.SampleVariance!.Value));
        Assert.ThrowsExactly<InvalidOperationException>(() => resumed.Add(0d));
        SameBits(continuous.Snapshot(), resumed.Snapshot());
    }

    [TestMethod]
    public void SecondMomentOverflowIsAtomicAndContinuationRemainsUsable()
    {
        var stream = new StreamingMoments64(3);
        stream.Add(0d);
        var checkpoint = stream.Snapshot();
        // Difference is finite; its squared contribution is not.
        Assert.ThrowsExactly<ArithmeticException>(() => stream.Add(1e155));
        SameBits(checkpoint, stream.Snapshot());
        stream.Add(2d);
        stream.Add(4d);
        var control = StreamingMoments64.FromSnapshot(checkpoint);
        control.Add(2d);
        control.Add(4d);
        SameBits(control.Snapshot(), stream.Snapshot());
        Assert.AreEqual(2d, stream.GetResult().Mean!.Value);
        Assert.AreEqual(8d / 3d, stream.GetResult().PopulationVariance!.Value, 1e-14);
        Assert.AreEqual(4d, stream.GetResult().SampleVariance!.Value);
    }

    [TestMethod]
    public void NegativeZeroSingletonSurvivesCheckpointRoundTrip()
    {
        double negativeZero = BitConverter.Int64BitsToDouble(long.MinValue);
        var stream = new StreamingMoments64(2);
        stream.Add(negativeZero);
        var checkpoint = stream.Snapshot();
        var restored = StreamingMoments64.FromSnapshot(checkpoint);
        SameBits(checkpoint, restored.Snapshot());
        Assert.AreEqual(long.MinValue, BitConverter.DoubleToInt64Bits(restored.GetResult().Mean!.Value));
        Assert.IsNull(restored.GetResult().SampleVariance);
    }

    private static void SameBits(StreamingMomentsSnapshot64 expected, StreamingMomentsSnapshot64 actual)
    {
        Assert.AreEqual(expected.Algorithm, actual.Algorithm);
        Assert.AreEqual(expected.MaxObservations, actual.MaxObservations);
        Assert.AreEqual(expected.Count, actual.Count);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(expected.Origin), BitConverter.DoubleToInt64Bits(actual.Origin));
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(expected.MeanOffset), BitConverter.DoubleToInt64Bits(actual.MeanOffset));
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(expected.M2), BitConverter.DoubleToInt64Bits(actual.M2));
    }
}
