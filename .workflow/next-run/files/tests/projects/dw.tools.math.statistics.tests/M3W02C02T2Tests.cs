using System;
using System.Linq;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C02T2Tests
{
    private static void Near(double? actual, double expected, double tolerance = 1e-12)
    {
        Assert.IsTrue(actual.HasValue && double.IsFinite(actual.Value),
            "Expected a finite defined numerical result.");
        Assert.IsTrue(global::System.Math.Abs(actual.Value - expected) <= tolerance,
            $"Expected {expected:R}; observed {actual.Value:R}; tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void ConstantFirstAxisReturnsTypedUndefinedNotPearsonZero()
    {
        var outcome = CorrelationStatistics64.AnalyzeWithStatus([4d, 4d, 4d], [1d, 2d, 3d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedZeroVariance, outcome.Status);
        Assert.IsFalse(outcome.IsDefined);
        Assert.IsNull(outcome.PearsonCorrelation);
        Near(outcome.PopulationCovariance, 0d, 0d);
        Near(outcome.SampleCovariance, 0d, 0d);
    }

    [TestMethod]
    public void ConstantSecondAxisAlsoYieldsTypedUndefined()
    {
        var outcome = CorrelationStatistics64.AnalyzeWithStatus([1d, 2d, 3d], [5d, 5d, 5d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedZeroVariance, outcome.Status);
        Assert.IsNull(outcome.PearsonCorrelation);
    }

    [TestMethod]
    public void TwoConstantAxesHaveZeroCovarianceAndUndefinedPearson()
    {
        var outcome = CorrelationStatistics64.AnalyzeWithStatus([1d, 1d], [3d, 3d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedZeroVariance, outcome.Status);
        Near(outcome.SampleCovariance, 0d, 0d);
        Assert.IsNull(outcome.PearsonCorrelation);
    }

    [TestMethod]
    public void SingletonHasPopulationZeroAndUnavailableSampleCovariance()
    {
        var outcome = CorrelationStatistics64.AnalyzeWithStatus([9d], [7d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedInsufficientPairs, outcome.Status);
        Assert.AreEqual(1, outcome.IncludedPairs);
        Near(outcome.PopulationCovariance, 0d, 0d);
        Assert.IsNull(outcome.SampleCovariance);
        Assert.IsNull(outcome.PearsonCorrelation);
    }

    [TestMethod]
    public void SkippedPairsMayLeaveOneAdmittedObservation()
    {
        var outcome = CorrelationStatistics64.AnalyzeWithStatus(
            [(double?)1d, null], [(double?)2d, 9d], MissingPairPolicy64.SkipPair);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedInsufficientPairs, outcome.Status);
        Assert.AreEqual(2, outcome.ProvidedPairs);
        Assert.AreEqual(1, outcome.IncludedPairs);
        Assert.AreEqual(1, outcome.MissingPairs);
    }

    [TestMethod]
    public void IncreasingAffinePairsAreDefinedWithUnitPearson()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus([1d, 2d, 3d], [2d, 4d, 6d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.Defined, result.Status);
        Assert.IsTrue(result.IsDefined);
        Near(result.PopulationCovariance, 4d / 3d);
        Near(result.SampleCovariance, 2d);
        Near(result.PearsonCorrelation, 1d);
    }

    [TestMethod]
    public void DecreasingAffinePairsRetainNegativeUnitPearson()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus([1d, 2d, 3d], [-1d, -3d, -5d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.Defined, result.Status);
        Near(result.PopulationCovariance, -4d / 3d);
        Near(result.PearsonCorrelation, -1d);
    }

    [TestMethod]
    public void ImperfectCorrelationUsesThePreviouslyQualifiedCovarianceOracle()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus([0d, 1d, 2d], [0d, 0d, 3d]);
        Near(result.PopulationCovariance, 1d);
        Near(result.SampleCovariance, 1.5d);
        Near(result.PearsonCorrelation, global::System.Math.Sqrt(3d) / 2d);
    }

    [TestMethod]
    public void ZeroCovarianceCanStillHaveDefinedZeroCorrelation()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus([1d, 2d, 3d], [1d, 0d, 1d]);
        Assert.AreEqual(CorrelationDefinitionStatus64.Defined, result.Status);
        Near(result.PopulationCovariance, 0d, 0d);
        Near(result.PearsonCorrelation, 0d, 0d);
    }

    [TestMethod]
    public void SwappingFiniteAxesPreservesCovarianceAndPearson()
    {
        var a = CorrelationStatistics64.AnalyzeWithStatus([1d, 4d, 7d], [0d, 1d, 3d]);
        var b = CorrelationStatistics64.AnalyzeWithStatus([0d, 1d, 3d], [1d, 4d, 7d]);
        Near(a.PopulationCovariance, b.PopulationCovariance!.Value);
        Near(a.SampleCovariance, b.SampleCovariance!.Value);
        Near(a.PearsonCorrelation, b.PearsonCorrelation!.Value);
    }

    [TestMethod]
    public void PairwiseSkipMatchesManuallyFilteredCompleteCases()
    {
        var skipped = CorrelationStatistics64.AnalyzeWithStatus(
            [(double?)1d, null, 3d, 4d], [2d, 9d, 6d, 8d], MissingPairPolicy64.SkipPair);
        var manual = CorrelationStatistics64.Analyze([1d, 3d, 4d], [2d, 6d, 8d]);
        Assert.AreEqual(4, skipped.ProvidedPairs);
        Assert.AreEqual(3, skipped.IncludedPairs);
        Assert.AreEqual(1, skipped.MissingPairs);
        Near(skipped.PopulationCovariance, manual.PopulationCovariance);
        Near(skipped.SampleCovariance, manual.SampleCovariance);
        Near(skipped.PearsonCorrelation, manual.PearsonCorrelation);
    }

    [TestMethod]
    public void PairwiseSkipDropsNullFromEitherAxisNotIndividualObservations()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus(
            [(double?)1d, 2d, 3d, 4d],
            [(double?)2d, null, 6d, 8d], MissingPairPolicy64.SkipPair);
        Assert.AreEqual(4, result.ProvidedPairs);
        Assert.AreEqual(3, result.IncludedPairs);
        Assert.AreEqual(1, result.MissingPairs);
        Near(result.PearsonCorrelation, 1d);
    }

    [TestMethod]
    public void DefaultMissingPolicyIsReject()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                [(double?)1d, null], [(double?)2d, 4d]));
    }

    [TestMethod]
    public void ExplicitRejectAlsoRefusesNullPairs()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                [(double?)1d, null], [(double?)2d, 4d], MissingPairPolicy64.Reject));
    }

    [TestMethod]
    public void BothNullValuesCountAsOneMissingPair()
    {
        var r = CorrelationStatistics64.AnalyzeWithStatus(
            [(double?)1d, null, 2d, 3d], [(double?)3d, null, 6d, 9d],
            MissingPairPolicy64.SkipPair);
        Assert.AreEqual(1, r.MissingPairs);
        Assert.AreEqual(3, r.IncludedPairs);
    }

    [TestMethod]
    public void AllMissingPairsDoNotProduceFabricatedStatistics()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                new double?[] { null, null },
                new double?[] { null, null }, MissingPairPolicy64.SkipPair));
    }

    [TestMethod]
    public void NonfinitePresentValueInSkippedPairIsStillRefused()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                [(double?)null, 1d], [double.NaN, 2d], MissingPairPolicy64.SkipPair));
    }

    [TestMethod]
    public void NonfinitePairedValuesAreAlwaysRefused()
    {
        foreach (var bad in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            Assert.ThrowsExactly<ArithmeticException>(() =>
                CorrelationStatistics64.AnalyzeWithStatus([1d, 2d], [bad, 3d],
                    MissingPairPolicy64.SkipPair));
    }

    [TestMethod]
    public void InvalidMissingPolicyEnumIsRefused()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus([1d, 2d], [2d, 4d],
                (MissingPairPolicy64)17));
    }

    [TestMethod]
    public void NullFirstSequenceIsRefused()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(null!, new double?[] { 1d, 2d }));
    }

    [TestMethod]
    public void NullSecondSequenceIsRefused()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(new double?[] { 1d, 2d }, null!));
    }

    [TestMethod]
    public void MismatchedLengthsRefuseBeforeMissingFiltering()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                [(double?)1d, null, 3d], [(double?)2d, 3d],
                MissingPairPolicy64.SkipPair));
    }

    [TestMethod]
    public void SourcePairBudgetIsEnforcedBeforeFiltering()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                Enumerable.Repeat((double?)null, 4097),
                Enumerable.Repeat((double?)2d, 4097),
                MissingPairPolicy64.SkipPair));
    }

    [TestMethod]
    public void EmptyVectorsAreNotAnUndefinedCorrelationValue()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                Array.Empty<double?>(), Array.Empty<double?>()));
    }

    [TestMethod]
    public void ConstantAxisWithExtremeOtherAxisRetainsMathematicalZeroCovariance()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus(
            [1d, 1d], [double.MaxValue, -double.MaxValue]);
        Assert.AreEqual(CorrelationDefinitionStatus64.UndefinedZeroVariance, result.Status);
        Near(result.PopulationCovariance, 0d, 0d);
        Assert.IsNull(result.PearsonCorrelation);
    }

    [TestMethod]
    public void OverflowingNonconstantMomentsStillRefuse()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            CorrelationStatistics64.AnalyzeWithStatus(
                [double.MaxValue, -double.MaxValue], [1d, 2d]));
    }

    [TestMethod]
    public void ExistingDefinedOnlyApiStillRejectsConstantVectors()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            CorrelationStatistics64.Analyze([1d, 1d], [2d, 3d]));
    }

    [TestMethod]
    public void Binary64Type7ExactLeftEndpointAvoidsOverflowingDifference()
    {
        var q = QuantileStatistics64.Evaluate(
            [double.MinValue, double.MaxValue], 0d, QuantileConvention64.Type7);
        Assert.AreEqual(double.MinValue, q);
    }

    [TestMethod]
    public void Binary64Type7IntegerKnotAvoidsOverflowingAdjacentSpan()
    {
        var q = QuantileStatistics64.Evaluate(
            [double.MinValue, double.MinValue, double.MaxValue],
            .5d, QuantileConvention64.Type7);
        Assert.AreEqual(double.MinValue, q);
    }

    [TestMethod]
    public void Binary64Type7NontrivialOverflowingInterpolationStillRefuses()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            QuantileStatistics64.Evaluate(
                [double.MinValue, double.MaxValue], .5d, QuantileConvention64.Type7));
    }

    [TestMethod]
    public void MissingPolicyStatusAndPrecisionAreConsumerReadableWithoutDecisionCode()
    {
        var result = CorrelationStatistics64.AnalyzeWithStatus(
            [2d, 3d, 4d], [6d, 8d, 10d]);
        Assert.AreEqual(DescriptivePrecisionKind.FiniteBinary64, result.Precision);
        Assert.AreEqual(CorrelationDefinitionStatus64.Defined, result.Status);
        Assert.AreEqual(0, result.MissingPairs);
        Assert.IsTrue(result.IsDefined && result.PearsonCorrelation.HasValue);
    }
}
