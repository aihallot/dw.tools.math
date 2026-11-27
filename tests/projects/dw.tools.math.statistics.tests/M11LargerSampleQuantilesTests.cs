using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M11LargerSampleQuantilesTests
{
    [TestMethod]
    public void FiniteLargeSampleAndLegacyBoundaryUseTheSameConventions()
    {
        var values = Enumerable.Range(0, 1_000_000).Select(x => (double)x);
        Assert.AreEqual(499999.5d, QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.Type7, 1_000_000));
        Assert.AreEqual(499999d, QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.NearestRank, 1_000_000));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.Type7));
        var small = Enumerable.Range(0, 4096).Select(x => (double)x);
        foreach (var convention in Enum.GetValues<QuantileConvention64>())
            Assert.AreEqual(QuantileStatistics64.Evaluate(small, 0.25d, convention),
                QuantileStatistics64.Evaluate(small, 0.25d, convention, 4096));
    }

    [TestMethod]
    public void ExactSeparateCeilingAndRationalInterpolationRemainExact()
    {
        var values = Enumerable.Range(0, 8192).Select(x => new ExactRational(x, BigInteger.One));
        var half = new ExactRational(1, 2);
        Assert.AreEqual(new ExactRational(8191, 2), QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.Type7, 8192));
        Assert.AreEqual(new ExactRational(4095, 1), QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.NearestRank, 8192));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.Type7));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(values, half, QuantileConvention64.Type7, 8193));
        var oversized = new ExactRational(BigInteger.One << 4096, BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact([oversized], half, QuantileConvention64.Type7, 8192));
    }

    [TestMethod]
    public void InvalidBudgetsDoNotEnumerateAndExcessConsumesOnlyOneExtra()
    {
        int consumed = 0;
        IEnumerable<double> Finite()
        {
            while (true) { consumed++; yield return 1d; }
        }
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(Finite(), 0.5d, QuantileConvention64.Type7, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(Finite(), 0.5d, QuantileConvention64.Type7, 1_000_001));
        Assert.AreEqual(0, consumed);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.Evaluate(Finite(), 0.5d, QuantileConvention64.Type7, 3));
        Assert.AreEqual(4, consumed);
        int exactConsumed = 0;
        IEnumerable<ExactRational> Exact()
        {
            while (true) { exactConsumed++; yield return ExactRational.One; }
        }
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(Exact(), ExactRational.One, QuantileConvention64.NearestRank, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(Exact(), ExactRational.One, QuantileConvention64.NearestRank, 8193));
        Assert.AreEqual(0, exactConsumed);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => QuantileStatistics64.EvaluateExact(Exact(), ExactRational.One, QuantileConvention64.NearestRank, 3));
        Assert.AreEqual(4, exactConsumed);
    }

    [TestMethod]
    public void TiesEndpointsAndExtremeRefusalsPreserveHistoricalSemantics()
    {
        double[] ties = [9d, 3d, 3d, 3d, 1d];
        foreach (var convention in Enum.GetValues<QuantileConvention64>())
        {
            Assert.AreEqual(1d, QuantileStatistics64.Evaluate(ties, 0d, convention, 10));
            Assert.AreEqual(9d, QuantileStatistics64.Evaluate(ties, 1d, convention, 10));
            Assert.AreEqual(3d, QuantileStatistics64.Evaluate(ties, 0.5d, convention, 10));
        }
        double[] extremes = [-double.MaxValue, double.MaxValue];
        Assert.AreEqual(-double.MaxValue, QuantileStatistics64.Evaluate(extremes, 0d, QuantileConvention64.Type7, 10));
        Assert.AreEqual(double.MaxValue, QuantileStatistics64.Evaluate(extremes, 1d, QuantileConvention64.Type7, 10));
        Assert.ThrowsExactly<ArithmeticException>(() => QuantileStatistics64.Evaluate(extremes, 0.5d, QuantileConvention64.Type7, 10));
        Assert.ThrowsExactly<ArithmeticException>(() => QuantileStatistics64.Evaluate([double.NaN], 0.5d, QuantileConvention64.Type7, 10));
        Assert.ThrowsExactly<ArgumentException>(() => QuantileStatistics64.Evaluate([], 0.5d, QuantileConvention64.Type7, 10));
    }
}
