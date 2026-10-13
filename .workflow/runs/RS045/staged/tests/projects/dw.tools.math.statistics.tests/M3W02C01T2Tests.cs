using System;
using System.Linq;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C01T2Tests
{
    private static FrequencyWeightedObservation64 Item(double? value,int frequency=1)
        => new(value,frequency);

    private static void Near(double observed,double expected,double tolerance=1e-12)
    {
        Assert.IsTrue(double.IsFinite(observed),"The observed statistic must be finite.");
        Assert.IsTrue(global::System.Math.Abs(observed-expected)<=tolerance,
            $"Expected {expected:R}, observed {observed:R} with tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void FrequencyReplicationHasHandVerifiedMeanAndTwoVarianceDenominators()
    {
        var result=FrequencyWeightedStatistics64.Analyze([Item(1,2),Item(3)]);
        Assert.AreEqual(2,result.ProvidedCount);
        Assert.AreEqual(2,result.IncludedCount);
        Assert.AreEqual(0,result.MissingCount);
        Assert.AreEqual(3,result.EffectiveCount);
        Near(result.Sum,5);
        Near(result.Mean,5d/3d);
        Near(result.PopulationVariance,8d/9d);
        Near(result.SampleVariance!.Value,4d/3d);
        Near(result.Minimum,1);
        Near(result.Maximum,3);
    }

    [TestMethod]
    public void IntegerFrequencyResultsAgreeWithExplicitExpandedData()
    {
        var weighted=FrequencyWeightedStatistics64.Analyze([Item(1,2),Item(3)]);
        var expanded=DescriptiveStatistics64.Analyze([1d,1d,3d]);
        Near(weighted.Sum,expanded.Sum);
        Near(weighted.Mean,expanded.Mean);
        Near(weighted.PopulationVariance,expanded.PopulationVariance);
        Near(weighted.SampleVariance!.Value,expanded.SampleVariance!.Value);
    }

    [TestMethod]
    public void UnitFrequenciesPreserveTheQualifiedFourObservationOracles()
    {
        var weighted=FrequencyWeightedStatistics64.Analyze(
            [Item(1),Item(2),Item(3),Item(4)]);
        Near(weighted.Mean,2.5);
        Near(weighted.PopulationVariance,1.25);
        Near(weighted.SampleVariance!.Value,5d/3d);
    }

    [TestMethod]
    public void MissingNullIsRefusedUnlessSkipPolicyWasExplicitlyRequested()
    {
        Assert.ThrowsExactly<ArgumentException>(()=>
            FrequencyWeightedStatistics64.Analyze([Item(1),Item(null)]));
    }

    [TestMethod]
    public void ExplicitMissingSkipPreservesRecordAndFrequencyCounts()
    {
        var result=FrequencyWeightedStatistics64.Analyze(
            [Item(null,2),Item(1),Item(3)],MissingObservationPolicy64.Skip);
        Assert.AreEqual(3,result.ProvidedCount);
        Assert.AreEqual(2,result.IncludedCount);
        Assert.AreEqual(1,result.MissingCount);
        Assert.AreEqual(2,result.EffectiveCount);
        Near(result.Sum,4);
        Near(result.Mean,2);
        Near(result.PopulationVariance,1);
        Near(result.SampleVariance!.Value,2);
    }

    [TestMethod]
    public void EntirelyMissingDatasetIsRefusedEvenWithSkip()
    {
        Assert.ThrowsExactly<ArgumentException>(()=>
            FrequencyWeightedStatistics64.Analyze(
                [Item(null,2),Item(null,1)],MissingObservationPolicy64.Skip));
    }

    [TestMethod]
    public void SkippedMissingFrequencyDoesNotExpandEffectiveCount()
    {
        var result=FrequencyWeightedStatistics64.Analyze(
            [Item(null,4096),Item(7,1)],MissingObservationPolicy64.Skip);
        Assert.AreEqual(1,result.EffectiveCount);
        Assert.AreEqual(1,result.MissingCount);
        Near(result.Sum,7);
        Assert.IsNull(result.SampleVariance);
    }

    [TestMethod]
    public void NaNIsNotSilentlyInterpretedAsMissing()
    {
        Assert.ThrowsExactly<ArithmeticException>(()=>
            FrequencyWeightedStatistics64.Analyze(
                [Item(1),Item(double.NaN)],MissingObservationPolicy64.Skip));
    }

    [TestMethod]
    public void InfinityIsRefusedUnderAllMissingPolicies()
    {
        foreach(var invalid in new[]{double.PositiveInfinity,double.NegativeInfinity})
            Assert.ThrowsExactly<ArithmeticException>(()=>
                FrequencyWeightedStatistics64.Analyze(
                    [Item(1),Item(invalid)],MissingObservationPolicy64.Skip));
    }

    [TestMethod]
    public void UnknownMissingPolicyEnumIsRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            FrequencyWeightedStatistics64.Analyze([Item(1)],
                (MissingObservationPolicy64)42));
    }

    [TestMethod]
    public void ZeroFrequencyIsNotSilentlyDiscarded()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            FrequencyWeightedStatistics64.Analyze([Item(2,0)]));
    }

    [TestMethod]
    public void NegativeFrequencyIsRejectedEvenForMissingRecord()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            FrequencyWeightedStatistics64.Analyze(
                [Item(null,-1),Item(1)],MissingObservationPolicy64.Skip));
    }

    [TestMethod]
    public void EffectiveReplicationBudgetIsEnforced()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            FrequencyWeightedStatistics64.Analyze([Item(1,4096),Item(1,1)]));
    }

    [TestMethod]
    public void SourceRowBudgetIsBoundedBeforeMaterialization()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            FrequencyWeightedStatistics64.Analyze(
                Enumerable.Repeat(Item(1),4097)));
    }

    [TestMethod]
    public void MaximumAdmittedFrequencyIsARealExecutableBoundary()
    {
        var result=FrequencyWeightedStatistics64.Analyze([Item(0.5,4096)]);
        Assert.AreEqual(4096,result.EffectiveCount);
        Near(result.Sum,2048);
        Near(result.Mean,0.5);
        Near(result.PopulationVariance,0);
        Near(result.SampleVariance!.Value,0);
    }

    [TestMethod]
    public void OneEffectiveObservationHasUndefinedSampleVariance()
    {
        var result=FrequencyWeightedStatistics64.Analyze([Item(4,1)]);
        Near(result.PopulationVariance,0);
        Assert.IsNull(result.SampleVariance);
    }

    [TestMethod]
    public void OneDistinctRecordRepeatedTwiceHasDefinedZeroSampleVariance()
    {
        var result=FrequencyWeightedStatistics64.Analyze([Item(4,2)]);
        Assert.AreEqual(2,result.EffectiveCount);
        Near(result.SampleVariance!.Value,0);
        Near(result.Sum,8);
    }

    [TestMethod]
    public void FrequencySummationPreservesSmallCancellationTerm()
    {
        var result=FrequencyWeightedStatistics64.Analyze(
            [Item(1e16),Item(1),Item(-1e16)]);
        Near(result.Sum,1,0);
        Near(result.Mean,1d/3d,1e-15);
    }

    [TestMethod]
    public void CenteredFrequencyVarianceHandlesLargeOffsets()
    {
        var result=FrequencyWeightedStatistics64.Analyze(
            [Item(1e16),Item(1e16+2d)]);
        Near(result.PopulationVariance,1,0);
        Near(result.SampleVariance!.Value,2,0);
    }

    [TestMethod]
    public void NonfiniteWeightedSumCannotBeReportedAsFinite()
    {
        Assert.ThrowsExactly<ArithmeticException>(()=>
            FrequencyWeightedStatistics64.Analyze([Item(double.MaxValue,2)]));
    }

    [TestMethod]
    public void OverflowedCenteredDeviationIsARefusal()
    {
        Assert.ThrowsExactly<ArithmeticException>(()=>
            FrequencyWeightedStatistics64.Analyze(
                [Item(double.MaxValue),Item(-double.MaxValue)]));
    }

    [TestMethod]
    public void ResultsDoNotAliasTheMutableCallerArray()
    {
        var rows=new[]{Item(1,2),Item(3)};
        var result=FrequencyWeightedStatistics64.Analyze(rows);
        rows[0]=Item(999,1);
        Near(result.Mean,5d/3d);
        Near(result.Minimum,1);
    }

    [TestMethod]
    public void StatisticsLibraryIsIndependentOfDecisionAuraAndMathNet()
    {
        var names=typeof(FrequencyWeightedStatistics64).Assembly
            .GetReferencedAssemblies().Select(v=>v.Name??string.Empty).ToArray();
        Assert.IsFalse(names.Any(n=>
            n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)
            ||n.Contains("Aura",StringComparison.OrdinalIgnoreCase)
            ||n.Contains("Decision",StringComparison.OrdinalIgnoreCase)));
    }
}
