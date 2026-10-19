using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S03T2Tests
{
    private static void Same(
        FiniteUniformSeriesAuditOutcome64 result, double[] expected,
        double interval, int window, double tolerance = 1e-10d)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteUniformSeriesAuditStatus64.Success, result.Status);
        Assert.IsNotNull(result.Outcome);
        Assert.IsNotNull(result.Outcome.Samples);
        Assert.AreEqual(expected.Length, result.Outcome.Samples.Length);
        Assert.AreEqual(interval, result.Outcome.SampleInterval);
        Assert.AreEqual(window, result.Outcome.Window);
        for (var i = 0; i < expected.Length; i++)
            Assert.IsTrue(global::System.Math.Abs(result.Outcome.Samples[i] - expected[i]) <= tolerance,
                $"Uniform boundary sample {i}: expected {expected[i]:R}, observed {result.Outcome.Samples[i]:R}");
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Refused(
        FiniteUniformSeriesAuditOutcome64 result, FiniteUniformSeriesAuditStatus64 status,
        bool requireSignal = false)
    {
        Assert.AreEqual(status, result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Outcome);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
        if (requireSignal)
        {
            Assert.IsNotNull(result.MinimumSignalRatio);
            Assert.IsTrue(double.IsFinite(result.MinimumSignalRatio.Value));
            Assert.IsTrue(result.MinimumSignalRatio.Value >= 0d);
        }
    }

    [TestMethod] public void PrimaryDifferences()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d,2d],1d),[2d,3d],1d,2); }

    [TestMethod] public void DifferenceHalfInterval()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,0.5d,1d],0.5d),[2d,3d],0.5d,2); }

    [TestMethod] public void DifferenceQuarterInterval()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,0.25d,0.5d],0.25d),[2d,3d],0.25d,2); }

    [TestMethod] public void DifferenceFractionalTimeGrid()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,0.1d,0.2d],0.1d),[2d,3d],0.1d,2); }

    [TestMethod] public void DifferenceNegativeTimes()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[-2d,-1d,0d],1d),[2d,3d],1d,2); }

    [TestMethod] public void DifferenceNonzeroTimeOrigin()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[10d,11d,12d],1d),[2d,3d],1d,2); }

    [TestMethod] public void DifferenceTwoSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],1d),[2d],1d,2); }

    [TestMethod] public void DifferenceNegativeSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([-1d,-3d,-6d],[0d,1d,2d],1d),[-2d,-3d],1d,2); }

    [TestMethod] public void DifferenceDecreasingSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([6d,3d,1d],[0d,1d,2d],1d),[-3d,-2d],1d,2); }

    [TestMethod] public void DifferenceAlternatingSigns()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,-1d,1d],[0d,1d,2d],1d),[-2d,2d],1d,2); }

    [TestMethod] public void DifferenceMixedSigns()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([-2d,5d,-1d],[0d,1d,2d],1d),[7d,-6d],1d,2); }

    [TestMethod] public void DifferenceFractions()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([0.5d,1.5d,3d],[0d,1d,2d],1d),[1d,1.5d],1d,2); }

    [TestMethod] public void DifferenceIdenticalValues()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([5d,5d],[0d,1d],1d);Same(r,[0d],1d,2);Assert.IsNull(r.MinimumSignalRatio); }

    [TestMethod] public void DifferenceAllZero()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([0d,0d,0d],[0d,1d,2d],1d);Same(r,[0d,0d],1d,2);Assert.IsNull(r.MinimumSignalRatio); }

    [TestMethod] public void DifferenceOneTinyValue()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e-100d,2e-100d],[0d,1d],1d),[1e-100d],1d,2,1e-112d); }

    [TestMethod] public void DifferenceThirtyTwoValues()
    { var values=Enumerable.Range(0,32).Select(i=>(double)i).ToArray();var times=Enumerable.Range(0,32).Select(i=>(double)i).ToArray();var result=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(values,times,1d);Same(result,Enumerable.Repeat(1d,31).ToArray(),1d,2); }

    [TestMethod] public void DifferenceAtInputBudgets()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e6d,-1e6d],[0d,1d],1d),[-2e6d],1d,2); }

    [TestMethod] public void DifferenceAtModerateTimeOrigin()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[1e6d,1e6d+1d],1d),[2d],1d,2); }

    [TestMethod] public void DifferenceAtTinyTimeInterval()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1e-100d,2e-100d],1e-100d),[2d,3d],1e-100d,2); }

    [TestMethod] public void DifferenceRatioObserved()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d,2d],1d);Same(r,[2d,3d],1d,2);Assert.IsNotNull(r.MinimumSignalRatio);Assert.IsTrue(global::System.Math.Abs(r.MinimumSignalRatio.Value-0.5d)<1e-12d); }

    [TestMethod] public void DifferenceRuntimeAuditIdentity()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],1d);Same(r,[2d],1d,2);Assert.AreEqual(FiniteUniformSeriesBoundaryAudit64.RuleId,r.RuleId); }

    [TestMethod] public void DifferenceRuntimeBackendIdentity()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],1d);Same(r,[2d],1d,2);Assert.IsNotNull(r.Outcome);Assert.AreEqual(FiniteUniformSeries64.RuleId,r.Outcome.RuleId); }

    [TestMethod] public void DifferenceNoCertifiedError()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],1d);Same(r,[2d],1d,2);Assert.IsFalse(r.ErrorBoundGuaranteed);Assert.IsFalse(r.Outcome!.ErrorBoundGuaranteed); }

    [TestMethod] public void DifferenceNoDivisionByTimeInterval()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,2d,4d],2d),[2d,3d],2d,2); }

    [TestMethod] public void DifferenceNullSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(null,[0d,1d],1d),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void DifferenceEmptySamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void DifferenceSingleSample()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d],[0d],1d),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void DifferenceOversizedSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(new double[33],new double[33],1d),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void DifferenceNaNSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([double.NaN,1d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.InvalidInput); }

    [TestMethod] public void DifferenceInfiniteSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,double.PositiveInfinity],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.InvalidInput); }

    [TestMethod] public void DifferenceExcessSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e7d,0d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DifferenceZeroInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],0d),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceNegativeInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],-1d),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceNonfiniteInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],double.PositiveInfinity),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceMissingTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],null,1d),FiniteUniformSeriesAuditStatus64.SampleTimesRequired); }

    [TestMethod] public void DifferenceWrongTimeLength()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.InvalidTimeLength); }

    [TestMethod] public void DifferenceEmptyTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[],1d),FiniteUniformSeriesAuditStatus64.InvalidTimeLength); }

    [TestMethod] public void DifferenceNaNTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,double.NaN],1d),FiniteUniformSeriesAuditStatus64.InvalidTimeValue); }

    [TestMethod] public void DifferenceInfiniteTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,double.PositiveInfinity],1d),FiniteUniformSeriesAuditStatus64.InvalidTimeValue); }

    [TestMethod] public void DifferenceExcessTimeBudget()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1e16d],1d),FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DifferenceDuplicateTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[1d,1d],1d),FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void DifferenceDecreasingTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[1d,0d],1d),FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void DifferenceIrregularTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d,3d],1d),FiniteUniformSeriesAuditStatus64.IrregularSampling); }

    [TestMethod] public void DifferenceSmallIrregularDrift()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d,2.001d],1d),FiniteUniformSeriesAuditStatus64.IrregularSampling); }

    [TestMethod] public void DifferenceAcceptsTinyGridRounding()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d,6d],[0d,1d,2d+1e-11d],1d),[2d,3d],1d,2); }

    [TestMethod] public void DifferenceTimestampResolutionRisk()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[1e14d,1e14d+1d],1d),FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk); }

    [TestMethod] public void DifferenceRelativeDifferenceRisk()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e5d,1e5d+1e-8d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk,true); }

    [TestMethod] public void DifferenceRelativeRiskAtNegativePosition()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([-1e6d,-1e5d+1e-8d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk,true); }

    [TestMethod] public void DifferenceResolvableDeltaAccepted()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e5d,1e5d+1e-4d],[0d,1d],1d),[1e-4d],1d,2,1e-9d); }

    [TestMethod] public void DifferenceDirectFacadeStillAllowed()
    { var raw=FiniteUniformSeries64.FirstDifferences([1e5d,1e5d+1e-8d],1d);Assert.IsTrue(raw.Succeeded);Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1e5d,1e5d+1e-8d],[0d,1d],1d),FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk,true); }

    [TestMethod] public void DifferencePreCancelled()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],[0d,1d],1d,c.Token),FiniteUniformSeriesAuditStatus64.Cancelled); }

    [TestMethod] public void DifferencePreCancelledOverridesMissingTimes()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences([1d,3d],null,1d,c.Token),FiniteUniformSeriesAuditStatus64.Cancelled); }

    [TestMethod] public void MeanPrimaryWindowTwo()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,2d],1d,2),[2d,4.5d],1d,2); }

    [TestMethod] public void MeanWindowOneIdentity()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,2d],1d,1),[1d,3d,6d],1d,1); }

    [TestMethod] public void MeanWholeWindow()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([2d,4d,6d],[0d,1d,2d],1d,3),[4d],1d,3); }

    [TestMethod] public void MeanSingletonWindow()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([4d],[0d],1d,1),[4d],1d,1); }

    [TestMethod] public void MeanTwoSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],1d,2),[2d],1d,2); }

    [TestMethod] public void MeanNegativeSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([-1d,-3d,-6d],[0d,1d,2d],1d,2),[-2d,-4.5d],1d,2); }

    [TestMethod] public void MeanNonzeroSampleOrigin()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[10d,11d,12d],1d,2),[2d,4.5d],1d,2); }

    [TestMethod] public void MeanFractionalGrid()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,0.1d,0.2d],0.1d,2),[2d,4.5d],0.1d,2); }

    [TestMethod] public void MeanNegativeGridCoordinates()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[-2d,-1d,0d],1d,2),[2d,4.5d],1d,2); }

    [TestMethod] public void MeanFractionalSamples()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([0.5d,1.5d,2.5d],[0d,1d,2d],1d,2),[1d,2d],1d,2); }

    [TestMethod] public void MeanAllPositive()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,2d,3d],[0d,1d,2d],1d,3),[2d],1d,3); }

    [TestMethod] public void MeanAllNegative()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([-1d,-2d,-3d],[0d,1d,2d],1d,3),[-2d],1d,3); }

    [TestMethod] public void MeanAllZero()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([0d,0d],[0d,1d],1d,2);Same(r,[0d],1d,2);Assert.IsNull(r.MinimumSignalRatio); }

    [TestMethod] public void MeanConstantSignal()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([5d,5d,5d],[0d,1d,2d],1d,2),[5d,5d],1d,2); }

    [TestMethod] public void MeanBoundaryMagnitude()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1e6d,1e6d],[0d,1d],1d,2),[1e6d],1d,2); }

    [TestMethod] public void MeanThirtyTwoFullWindow()
    { var a=Enumerable.Repeat(1d,32).ToArray();var t=Enumerable.Range(0,32).Select(i=>(double)i).ToArray();Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(a,t,1d,32),[1d],1d,32); }

    [TestMethod] public void MeanTinyInterval()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1e-100d],1e-100d,2),[2d],1e-100d,2); }

    [TestMethod] public void MeanMaximumAllowedTimeOrigin()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[1e6d,1e6d+1d],1d,2),[2d],1d,2); }

    [TestMethod] public void MeanMinimumSignalObserved()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,2d],1d,2);Same(r,[2d,4.5d],1d,2);Assert.IsNotNull(r.MinimumSignalRatio);Assert.IsTrue(global::System.Math.Abs(r.MinimumSignalRatio.Value-1d)<1e-12d); }

    [TestMethod] public void MeanRuntimeAuditId()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([2d],[0d],1d,1);Same(r,[2d],1d,1);Assert.AreEqual(FiniteUniformSeriesBoundaryAudit64.RuleId,r.RuleId); }

    [TestMethod] public void MeanRuntimeBackendId()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([2d],[0d],1d,1);Same(r,[2d],1d,1);Assert.IsNotNull(r.Outcome);Assert.AreEqual(FiniteUniformSeries64.RuleId,r.Outcome.RuleId); }

    [TestMethod] public void MeanNoCertifiedError()
    { var r=FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([2d],[0d],1d,1);Same(r,[2d],1d,1);Assert.IsFalse(r.ErrorBoundGuaranteed);Assert.IsFalse(r.Outcome!.ErrorBoundGuaranteed); }

    [TestMethod] public void MeanNoImplicitTimeUnits()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,2d,4d],2d,2),[2d,4.5d],2d,2); }

    [TestMethod] public void MeanNullSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(null,[0d],1d,1),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void MeanEmptySamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([],[0d],1d,1),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void MeanOversizedSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(new double[33],new double[33],1d,2),FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void MeanZeroWindow()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],1d,0),FiniteUniformSeriesAuditStatus64.InvalidWindow); }

    [TestMethod] public void MeanNegativeWindow()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],1d,-1),FiniteUniformSeriesAuditStatus64.InvalidWindow); }

    [TestMethod] public void MeanOversizedWindow()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],1d,3),FiniteUniformSeriesAuditStatus64.InvalidWindow); }

    [TestMethod] public void MeanNaNSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([double.NaN,1d],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.InvalidInput); }

    [TestMethod] public void MeanInfiniteSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,double.NegativeInfinity],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.InvalidInput); }

    [TestMethod] public void MeanExcessSamples()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1e7d,1d],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void MeanZeroInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],0d,2),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void MeanNegativeInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],-1d,2),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void MeanNaNInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],double.NaN,2),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void MeanInfiniteInterval()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],double.PositiveInfinity,2),FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void MeanMissingTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],null,1d,2),FiniteUniformSeriesAuditStatus64.SampleTimesRequired); }

    [TestMethod] public void MeanWrongTimeLength()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d],1d,2),FiniteUniformSeriesAuditStatus64.InvalidTimeLength); }

    [TestMethod] public void MeanNaNTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,double.NaN],1d,2),FiniteUniformSeriesAuditStatus64.InvalidTimeValue); }

    [TestMethod] public void MeanInfiniteTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,double.PositiveInfinity],1d,2),FiniteUniformSeriesAuditStatus64.InvalidTimeValue); }

    [TestMethod] public void MeanExcessTimeBudget()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1e16d],1d,2),FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void MeanDuplicateTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[1d,1d],1d,2),FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void MeanDecreasingTimes()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[1d,0d],1d,2),FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void MeanIrregularGrid()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,3d],1d,2),FiniteUniformSeriesAuditStatus64.IrregularSampling); }

    [TestMethod] public void MeanTinyIrregularDrift()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,2.001d],1d,2),FiniteUniformSeriesAuditStatus64.IrregularSampling); }

    [TestMethod] public void MeanAcceptsGridRounding()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d,6d],[0d,1d,2d+1e-11d],1d,2),[2d,4.5d],1d,2); }

    [TestMethod] public void MeanTimestampResolutionRisk()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[1e14d,1e14d+1d],1d,2),FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk); }

    [TestMethod] public void MeanCancellationRisk()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,-1d],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.MeanCancellationRisk,true); }

    [TestMethod] public void MeanNearCancellationRisk()
    { Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,-1d+1e-13d],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.MeanCancellationRisk,true); }

    [TestMethod] public void MeanResolvableCompensation()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,-1d+1e-9d],[0d,1d],1d,2),[5e-10d],1d,2,1e-12d); }

    [TestMethod] public void MeanDirectFacadeStillAdmitsZero()
    { var raw=FiniteUniformSeries64.MovingAverage([1d,-1d],1d,2);Assert.IsTrue(raw.Succeeded);Assert.IsNotNull(raw.Samples);Assert.AreEqual(0d,raw.Samples[0]);Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,-1d],[0d,1d],1d,2),FiniteUniformSeriesAuditStatus64.MeanCancellationRisk,true); }

    [TestMethod] public void MeanWindowOneNegativeAccepted()
    { Same(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([-1d],[-2d],1d,1),[-1d],1d,1); }

    [TestMethod] public void MeanPreCancelled()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],[0d,1d],1d,2,c.Token),FiniteUniformSeriesAuditStatus64.Cancelled); }

    [TestMethod] public void MeanPreCancelledOverridesMissingTimes()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage([1d,3d],null,1d,2,c.Token),FiniteUniformSeriesAuditStatus64.Cancelled); }
}
