using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M11LargerSampleQuantilesVerificationTests
{
    [TestMethod]
    public void RepeatedNonuniformSampleAgreesWithHandDerivedQuartiles()
    {
        // Exactly 1250 copies of each ordered value, but interleaved input.
        double[] pattern = [21d, -8d, 9d, 2d];
        var values = Enumerable.Range(0, 5000).Select(i => pattern[i % 4]).ToArray();
        double[] probabilities = [0.25d, 0.5d, 0.75d];
        double[] type7 = [-0.5d, 5.5d, 12d];
        double[] nearest = [-8d, 2d, 9d];
        for (int i = 0; i < probabilities.Length; i++)
        {
            Assert.AreEqual(type7[i], QuantileStatistics64.Evaluate(values, probabilities[i], QuantileConvention64.Type7, 5000));
            Assert.AreEqual(nearest[i], QuantileStatistics64.Evaluate(values.Reverse(), probabilities[i], QuantileConvention64.NearestRank, 5000));
        }
        CollectionAssert.AreEqual(new[] { 21d, -8d, 9d, 2d }, values.Take(4).ToArray());
    }

    [TestMethod]
    public void ExactNonDyadicProbabilitiesRetainRationalResults()
    {
        ExactRational[] pattern = [new(7, 3), new(-5, 3), new(2, 3), new(-1, 3), new(2, 3)];
        // 1000 repetitions give 5000 observations. At p=1/5, Type7
        // interpolates 4/5 of the gap from -5/3 to -1/3: -3/5.
        var values = Enumerable.Range(0, 5000).Select(i => pattern[i % 5]);
        var p = new ExactRational(1, 5);
        Assert.AreEqual(new ExactRational(-3, 5), QuantileStatistics64.EvaluateExact(values, p, QuantileConvention64.Type7, 5000));
        Assert.AreEqual(new ExactRational(-5, 3), QuantileStatistics64.EvaluateExact(values, p, QuantileConvention64.NearestRank, 5000));
        Assert.AreEqual(ExactRational.Zero, QuantileStatistics64.EvaluateExact(pattern, new ExactRational(1, 3), QuantileConvention64.Type7, 5));
        Assert.AreEqual(new ExactRational(-1, 3), QuantileStatistics64.EvaluateExact(pattern, new ExactRational(1, 3), QuantileConvention64.NearestRank, 5));
    }

    [TestMethod]
    public void ExactInterpolationRefusesOversizedDeltaButEndpointsAndRankRemainUsable()
    {
        BigInteger magnitude = BigInteger.One << 4095;
        var low = new ExactRational(-magnitude, BigInteger.One);
        var high = new ExactRational(magnitude, BigInteger.One);
        ExactRational[] values = [high, low];
        var half = new ExactRational(1, 2);
        Assert.AreEqual(low, QuantileStatistics64.EvaluateExact(values, ExactRational.Zero, QuantileConvention64.Type7, 8192));
        Assert.AreEqual(high, QuantileStatistics64.EvaluateExact(values, ExactRational.One, QuantileConvention64.Type7, 8192));
        Assert.AreEqual(low, QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.NearestRank, 8192));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.Type7, 8192));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void EnumerationDisposesAfterSuccessAndBudgetRefusal()
    {
        int consumed = 0;
        int disposed = 0;
        IEnumerable<double> Sample(int count)
        {
            try
            {
                for (int i = 0; i < count; i++) { consumed++; yield return i; }
            }
            finally { disposed++; }
        }
        Assert.AreEqual(1d, QuantileStatistics64.Evaluate(Sample(3), 0.5d, QuantileConvention64.Type7, 3));
        Assert.AreEqual(3, consumed);
        Assert.AreEqual(1, disposed);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(Sample(100), 0.5d, QuantileConvention64.Type7, 3));
        Assert.AreEqual(7, consumed);
        Assert.AreEqual(2, disposed);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(Sample(100), double.NaN, QuantileConvention64.Type7, 3));
        Assert.AreEqual(7, consumed);
        Assert.AreEqual(2, disposed);
    }
}
