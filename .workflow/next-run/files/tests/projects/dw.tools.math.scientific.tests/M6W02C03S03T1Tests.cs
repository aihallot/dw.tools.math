using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S03T1Tests
{
    private static void Same(FiniteUniformSeriesOutcome64 outcome, double[] expected,
        double interval, int window, double tolerance = 1e-10d)
    {
        Assert.IsTrue(outcome.Succeeded);
        Assert.AreEqual(FiniteUniformSeriesStatus64.Success, outcome.Status);
        Assert.IsNotNull(outcome.Samples);
        Assert.AreEqual(expected.Length, outcome.Samples.Length);
        Assert.AreEqual(interval, outcome.SampleInterval);
        Assert.AreEqual(window, outcome.Window);
        for (var i = 0; i < expected.Length; i++)
            Assert.IsTrue(global::System.Math.Abs(outcome.Samples[i]-expected[i])<=tolerance,
                $"Uniform series sample {i}: expected {expected[i]:R}, observed {outcome.Samples[i]:R}");
        Assert.IsFalse(outcome.ErrorBoundGuaranteed);
    }

    private static void Refused(FiniteUniformSeriesOutcome64 outcome,
        FiniteUniformSeriesStatus64 status)
    {
        Assert.AreEqual(status, outcome.Status);
        Assert.IsFalse(outcome.Succeeded);
        Assert.IsNull(outcome.Samples);
        Assert.IsFalse(outcome.ErrorBoundGuaranteed);
    }

    [TestMethod] public void PrimaryFirstDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d,6d],1d),[2d,3d],1d,2); }

    [TestMethod] public void TwoSamplesDifference()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d],1d),[2d],1d,2); }

    [TestMethod] public void NegativeDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([-1d,-3d,-6d],1d),[-2d,-3d],1d,2); }

    [TestMethod] public void DescendingDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([6d,3d,1d],1d),[-3d,-2d],1d,2); }

    [TestMethod] public void ConstantDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([5d,5d,5d],1d),[0d,0d],1d,2); }

    [TestMethod] public void AllZeroDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([0d,0d],1d),[0d],1d,2); }

    [TestMethod] public void MixedSignedDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([-2d,5d,-1d],1d),[7d,-6d],1d,2); }

    [TestMethod] public void FractionalDifferences()
    { Same(FiniteUniformSeries64.FirstDifferences([0.5d,1.5d,3d],1d),[1d,1.5d],1d,2); }

    [TestMethod] public void SampleIntervalHalfPreserved()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d,6d],0.5d),[2d,3d],0.5d,2); }

    [TestMethod] public void SampleIntervalHundredPreserved()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d,6d],100d),[2d,3d],100d,2); }

    [TestMethod] public void SampleIntervalTinyPositivePreserved()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d],1e-100d),[2d],1e-100d,2); }

    [TestMethod] public void DifferenceInputBoundaryValues()
    { Same(FiniteUniformSeries64.FirstDifferences([1e6d,-1e6d],1d),[-2e6d],1d,2); }

    [TestMethod] public void DifferenceOutput31Samples()
    { var data=Enumerable.Range(0,32).Select(x=>(double)x).ToArray();var x=FiniteUniformSeries64.FirstDifferences(data,1d);Assert.IsTrue(x.Succeeded);Assert.IsNotNull(x.Samples);Assert.AreEqual(31,x.Samples.Length);Assert.IsTrue(x.Samples.All(y=>y==1d)); }

    [TestMethod] public void DifferenceNullRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences(null,1d),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void DifferenceEmptyRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([],1d),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void DifferenceSingletonRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1d],1d),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void DifferenceOversizedInputRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences(new double[33],1d),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void DifferenceNaNRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([double.NaN,1d],1d),FiniteUniformSeriesStatus64.InvalidInput); }

    [TestMethod] public void DifferenceInfinityRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([0d,double.PositiveInfinity],1d),FiniteUniformSeriesStatus64.InvalidInput); }

    [TestMethod] public void DifferenceExcessMagnitudeRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1e7d,0d],1d),FiniteUniformSeriesStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DifferenceZeroIntervalRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],0d),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceNegativeIntervalRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],-1d),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceNaNIntervalRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],double.NaN),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferenceInfiniteIntervalRefused()
    { Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],double.PositiveInfinity),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void DifferencePrecancelRefused()
    { using var t=new CancellationTokenSource();t.Cancel();Refused(FiniteUniformSeries64.FirstDifferences([1d,3d],1d,t.Token),FiniteUniformSeriesStatus64.Cancelled); }

    [TestMethod] public void DifferencePrecancelTakesPrecedence()
    { using var t=new CancellationTokenSource();t.Cancel();Refused(FiniteUniformSeries64.FirstDifferences(null,0d,t.Token),FiniteUniformSeriesStatus64.Cancelled); }

    [TestMethod] public void DifferenceDoesNotDivideByInterval()
    { Same(FiniteUniformSeries64.FirstDifferences([1d,3d,6d],2d),[2d,3d],2d,2); }

    [TestMethod] public void DifferenceRuleIdentityRuntime()
    { var result=FiniteUniformSeries64.FirstDifferences([1d,3d],1d);Assert.IsTrue(result.Succeeded);Assert.AreEqual(FiniteUniformSeries64.RuleId,result.RuleId); }

    [TestMethod] public void DifferenceNoCertifiedErrorBound()
    { var result=FiniteUniformSeries64.FirstDifferences([1d,3d],1d);Assert.IsTrue(result.Succeeded);Assert.IsFalse(result.ErrorBoundGuaranteed); }

    [TestMethod] public void PrimaryMovingAverages()
    { Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],1d,2),[2d,4.5d],1d,2); }

    [TestMethod] public void MovingAverageWindowOneIsIdentity()
    { Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],1d,1),[1d,3d,6d],1d,1); }

    [TestMethod] public void MovingAverageEntireSeries()
    { Same(FiniteUniformSeries64.MovingAverage([2d,4d,6d],1d,3),[4d],1d,3); }

    [TestMethod] public void MovingAverageTwoSamples()
    { Same(FiniteUniformSeries64.MovingAverage([1d,3d],1d,2),[2d],1d,2); }

    [TestMethod] public void MovingAverageSingleSample()
    { Same(FiniteUniformSeries64.MovingAverage([4d],1d,1),[4d],1d,1); }

    [TestMethod] public void MovingAverageConstant()
    { Same(FiniteUniformSeries64.MovingAverage([2d,2d,2d],1d,2),[2d,2d],1d,2); }

    [TestMethod] public void MovingAverageAllZero()
    { Same(FiniteUniformSeries64.MovingAverage([0d,0d,0d],1d,2),[0d,0d],1d,2); }

    [TestMethod] public void MovingAverageNegative()
    { Same(FiniteUniformSeries64.MovingAverage([-1d,-3d,-6d],1d,2),[-2d,-4.5d],1d,2); }

    [TestMethod] public void MovingAverageMixedSigns()
    { Same(FiniteUniformSeries64.MovingAverage([-2d,2d,4d],1d,2),[0d,3d],1d,2); }

    [TestMethod] public void MovingAverageFractional()
    { Same(FiniteUniformSeries64.MovingAverage([0.5d,1.5d,2.5d],1d,2),[1d,2d],1d,2); }

    [TestMethod] public void MovingAverageIntervalPreserved()
    { Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],0.25d,2),[2d,4.5d],0.25d,2); }

    [TestMethod] public void MovingAverageFull32Window()
    { var a=Enumerable.Repeat(1d,32).ToArray();Same(FiniteUniformSeries64.MovingAverage(a,1d,32),[1d],1d,32); }

    [TestMethod] public void MovingAverageInputBoundary()
    { Same(FiniteUniformSeries64.MovingAverage([1e6d,1e6d],1d,2),[1e6d],1d,2); }

    [TestMethod] public void MovingAverageNullRefused()
    { Refused(FiniteUniformSeries64.MovingAverage(null,1d,1),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void MovingAverageEmptyRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([],1d,1),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void MovingAverageOversizedInputRefused()
    { Refused(FiniteUniformSeries64.MovingAverage(new double[33],1d,2),FiniteUniformSeriesStatus64.InvalidLength); }

    [TestMethod] public void MovingAverageZeroWindowRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],1d,0),FiniteUniformSeriesStatus64.InvalidWindow); }

    [TestMethod] public void MovingAverageNegativeWindowRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],1d,-1),FiniteUniformSeriesStatus64.InvalidWindow); }

    [TestMethod] public void MovingAverageOversizedWindowRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],1d,3),FiniteUniformSeriesStatus64.InvalidWindow); }

    [TestMethod] public void MovingAverageNaNRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([double.NaN,3d],1d,2),FiniteUniformSeriesStatus64.InvalidInput); }

    [TestMethod] public void MovingAverageInfinityRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,double.NegativeInfinity],1d,2),FiniteUniformSeriesStatus64.InvalidInput); }

    [TestMethod] public void MovingAverageExcessMagnitudeRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1e7d,2d],1d,2),FiniteUniformSeriesStatus64.OutsideQualifiedRange); }

    [TestMethod] public void MovingAverageZeroIntervalRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],0d,2),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void MovingAverageNegativeIntervalRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],-1d,2),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void MovingAverageNaNIntervalRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],double.NaN,2),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void MovingAverageInfiniteIntervalRefused()
    { Refused(FiniteUniformSeries64.MovingAverage([1d,3d],double.PositiveInfinity,2),FiniteUniformSeriesStatus64.InvalidSampleInterval); }

    [TestMethod] public void MovingAveragePrecancelRefused()
    { using var t=new CancellationTokenSource();t.Cancel();Refused(FiniteUniformSeries64.MovingAverage([1d,3d],1d,2,t.Token),FiniteUniformSeriesStatus64.Cancelled); }

    [TestMethod] public void MovingAveragePrecancelPriority()
    { using var t=new CancellationTokenSource();t.Cancel();Refused(FiniteUniformSeries64.MovingAverage(null,0d,0,t.Token),FiniteUniformSeriesStatus64.Cancelled); }

    [TestMethod] public void MovingAverageNoImplicitSampleUnits()
    { Same(FiniteUniformSeries64.MovingAverage([1d,3d,6d],4d,2),[2d,4.5d],4d,2); }

    [TestMethod] public void MovingAverageRuleIdentityRuntime()
    { var result=FiniteUniformSeries64.MovingAverage([1d],1d,1);Assert.IsTrue(result.Succeeded);Assert.AreEqual(FiniteUniformSeries64.RuleId,result.RuleId); }

    [TestMethod] public void MovingAverageNoCertifiedErrorBound()
    { var result=FiniteUniformSeries64.MovingAverage([1d],1d,1);Assert.IsTrue(result.Succeeded);Assert.IsFalse(result.ErrorBoundGuaranteed); }
}
