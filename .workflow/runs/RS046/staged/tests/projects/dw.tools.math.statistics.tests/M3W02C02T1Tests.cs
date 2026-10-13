using System;
using System.Linq;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C02T1Tests
{
    private static void Near(double actual, double expected, double tolerance = 1e-12)
    {
        Assert.IsTrue(double.IsFinite(actual), "The observed numerical result must be finite.");
        Assert.IsTrue(global::System.Math.Abs(actual - expected) <= tolerance,
            $"Expected {expected:R}; observed {actual:R}; tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void Type7TwoPointQuarterUsesLinearInterpolation()
    {
        Near(QuantileStatistics64.Evaluate([0d, 10d], .25d, QuantileConvention64.Type7), 2.5d);
    }

    [TestMethod]
    public void NearestRankTwoPointQuarterSelectsFirstObservation()
    {
        Near(QuantileStatistics64.Evaluate([0d, 10d], .25d, QuantileConvention64.NearestRank), 0d);
        Near(QuantileStatistics64.Evaluate([0d, 10d], .75d, QuantileConvention64.NearestRank), 10d);
    }

    [TestMethod]
    public void BothConventionsRespectEndpointsAndSingletons()
    {
        foreach (var convention in Enum.GetValues<QuantileConvention64>())
        {
            Near(QuantileStatistics64.Evaluate([4d, 8d], 0d, convention), 4d);
            Near(QuantileStatistics64.Evaluate([4d, 8d], 1d, convention), 8d);
            Near(QuantileStatistics64.Evaluate([7d], .75d, convention), 7d);
        }
    }

    [TestMethod]
    public void QuantileIsOrderInvariant()
    {
        Near(QuantileStatistics64.Evaluate([10d, 0d, 2d], .5d, QuantileConvention64.Type7), 2d);
        Near(QuantileStatistics64.Evaluate([2d, 10d, 0d], .5d, QuantileConvention64.Type7), 2d);
    }

    [TestMethod]
    public void Type7InterpolatesDuplicatedKnots()
    {
        Near(QuantileStatistics64.Evaluate([10d, 0d, 0d, 10d], .25d, QuantileConvention64.Type7), 0d);
        Near(QuantileStatistics64.Evaluate([10d, 0d, 0d, 10d], .5d, QuantileConvention64.Type7), 5d);
    }

    [TestMethod]
    public void ExactQuarterOfTwoValuesRemainsFiveHalves()
    {
        var result = QuantileStatistics64.EvaluateExact(
            [ExactRational.Zero, new ExactRational(10, 1)],
            new ExactRational(1, 4), QuantileConvention64.Type7);
        Assert.AreEqual(new ExactRational(5, 2), result);
    }

    [TestMethod]
    public void ExactNearestRankCeilingUsesIntegerArithmetic()
    {
        var values = new[] { ExactRational.Zero, new ExactRational(10, 1) };
        Assert.AreEqual(ExactRational.Zero,
            QuantileStatistics64.EvaluateExact(values, new ExactRational(1, 4), QuantileConvention64.NearestRank));
        Assert.AreEqual(new ExactRational(10, 1),
            QuantileStatistics64.EvaluateExact(values, new ExactRational(3, 4), QuantileConvention64.NearestRank));
    }

    [TestMethod]
    public void ExactRationalMedianDoesNotWidenToBinary64()
    {
        var result = QuantileStatistics64.EvaluateExact(
            [new ExactRational(1, 3), new ExactRational(2, 3)],
            new ExactRational(1, 2), QuantileConvention64.Type7);
        Assert.AreEqual(new ExactRational(1, 2), result);
    }

    [TestMethod]
    public void ExactEndpointsAndSingletonAreDefined()
    {
        var singleton = new[] { new ExactRational(1, 3) };
        Assert.AreEqual(singleton[0],
            QuantileStatistics64.EvaluateExact(singleton, ExactRational.Zero, QuantileConvention64.NearestRank));
        Assert.AreEqual(singleton[0],
            QuantileStatistics64.EvaluateExact(singleton, ExactRational.One, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void FiniteProbabilityOutsideUnitIntervalIsRefused()
    {
        foreach (var probability in new[] { double.NaN, double.PositiveInfinity, -.1d, 1.1d })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                QuantileStatistics64.Evaluate([1d, 2d], probability, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void InvalidQuantileConventionIsRefused()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.Evaluate([1d, 2d], .5d, (QuantileConvention64)100));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.EvaluateExact([ExactRational.One], ExactRational.One, (QuantileConvention64)100));
    }

    [TestMethod]
    public void QuantileNonfiniteObservationsAreRefused()
    {
        foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            Assert.ThrowsExactly<ArithmeticException>(() =>
                QuantileStatistics64.Evaluate([1d, value], .5d, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void QuantilesEnforceBoundedNonemptyInput()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            QuantileStatistics64.Evaluate(Array.Empty<double>(), .5d, QuantileConvention64.Type7));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.Evaluate(Enumerable.Repeat(1d, 4097), .5d, QuantileConvention64.Type7));
        Near(QuantileStatistics64.Evaluate(Enumerable.Repeat(2d, 4096), .5d,
            QuantileConvention64.Type7), 2d);
    }

    [TestMethod]
    public void ExactQuantilesEnforceOperandAndCountBudgets()
    {
        var big = new ExactRational(BigInteger.One << 4096, BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.EvaluateExact([big], ExactRational.Zero, QuantileConvention64.Type7));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.EvaluateExact(Enumerable.Repeat(ExactRational.One, 4097),
                ExactRational.Zero, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void ExactProbabilityOutsideUnitIntervalIsRefused()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.EvaluateExact([ExactRational.One],
                new ExactRational(-1, 3), QuantileConvention64.Type7));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantileStatistics64.EvaluateExact([ExactRational.One],
                new ExactRational(4, 3), QuantileConvention64.Type7));
    }

    [TestMethod]
    public void ThreePairsGiveIndependentCovarianceAndPearsonOracles()
    {
        var result = CorrelationStatistics64.Analyze([1d, 2d, 3d], [2d, 4d, 6d]);
        Assert.AreEqual(3, result.Count);
        Near(result.MeanX, 2d);
        Near(result.MeanY, 4d);
        Near(result.PopulationCovariance, 4d / 3d);
        Near(result.SampleCovariance, 2d);
        Near(result.PearsonCorrelation, 1d);
        Assert.AreEqual(DescriptivePrecisionKind.FiniteBinary64, result.Precision);
    }

    [TestMethod]
    public void NegativeAffineVectorHasNegativeUnitCorrelation()
    {
        var result = CorrelationStatistics64.Analyze([1d, 2d, 3d], [-1d, -3d, -5d]);
        Near(result.PopulationCovariance, -4d / 3d);
        Near(result.SampleCovariance, -2d);
        Near(result.PearsonCorrelation, -1d);
    }

    [TestMethod]
    public void PairedCovarianceIsSymmetricUnderVectorExchange()
    {
        var a = CorrelationStatistics64.Analyze([1d, 4d, 7d], [0d, 1d, 3d]);
        var b = CorrelationStatistics64.Analyze([0d, 1d, 3d], [1d, 4d, 7d]);
        Near(a.PopulationCovariance, b.PopulationCovariance);
        Near(a.SampleCovariance, b.SampleCovariance);
        Near(a.PearsonCorrelation, b.PearsonCorrelation);
    }

    [TestMethod]
    public void TranslationDoesNotChangeCovarianceOrCorrelation()
    {
        var a = CorrelationStatistics64.Analyze([1d, 2d, 3d], [2d, 4d, 6d]);
        var b = CorrelationStatistics64.Analyze([101d, 102d, 103d], [1002d, 1004d, 1006d]);
        Near(a.PopulationCovariance, b.PopulationCovariance);
        Near(a.SampleCovariance, b.SampleCovariance);
        Near(a.PearsonCorrelation, b.PearsonCorrelation);
    }

    [TestMethod]
    public void ImperfectCorrelationMatchesIndependentCenteredHandOracle()
    {
        var result = CorrelationStatistics64.Analyze([0d, 1d, 2d], [0d, 0d, 3d]);
        Near(result.PopulationCovariance, 1d);
        Near(result.SampleCovariance, 1.5d);
        Near(result.PearsonCorrelation, global::System.Math.Sqrt(3d) / 2d);
    }

    [TestMethod]
    public void SingletonPairedDataIsOutsideT1CorrelationContract()
    {
        Assert.ThrowsExactly<ArgumentException>(() => CorrelationStatistics64.Analyze([1d], [2d]));
    }

    [TestMethod]
    public void MismatchedLengthsAndEmptyPairsAreRefused()
    {
        Assert.ThrowsExactly<ArgumentException>(() => CorrelationStatistics64.Analyze([1d, 2d], [1d]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.Analyze(Array.Empty<double>(), Array.Empty<double>()));
    }

    [TestMethod]
    public void NonfinitePairedObservationsAreRefused()
    {
        foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            Assert.ThrowsExactly<ArithmeticException>(() =>
                CorrelationStatistics64.Analyze([1d, 2d], [value, 3d]));
    }

    [TestMethod]
    public void PairedInputBudgetIsEnforced()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            CorrelationStatistics64.Analyze(Enumerable.Repeat(1d, 4097), Enumerable.Repeat(2d, 4097)));
    }

    [TestMethod]
    public void OverflowingBivariateMomentsAreRefused()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            CorrelationStatistics64.Analyze([double.MaxValue, -double.MaxValue], [1d, 2d]));
    }

    [TestMethod]
    public void NewStatisticsRemainProviderAndBrokerNeutral()
    {
        var names = typeof(CorrelationStatistics64).Assembly.GetReferencedAssemblies()
            .Select(x => x.Name ?? string.Empty).ToArray();
        Assert.IsFalse(names.Any(x => x.Contains("MathNet", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Decision", StringComparison.OrdinalIgnoreCase)));
    }
}
