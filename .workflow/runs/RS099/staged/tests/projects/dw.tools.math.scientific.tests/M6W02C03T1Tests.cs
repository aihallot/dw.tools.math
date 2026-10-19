using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03T1Tests
{
    private static M6W02C03Request64 Good() =>
        new([0d,0d],[3d,4d],[1d,0d],
            [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
            [1d,2d],[1d,1d],[1d,3d,6d],1d,2);

    private static M6W02C03Outcome64 Evaluate(M6W02C03Request64? request) =>
        M6W02C03Contract.Evaluate(request);

    private static M6W02C03Outcome64 Accepted(M6W02C03Request64 request)
    {
        var actual=Evaluate(request);
        Assert.IsTrue(actual.Succeeded);
        Assert.AreEqual(M6W02C03Status64.Success,actual.Status);
        Assert.IsFalse(actual.ErrorBoundGuaranteed);
        return actual;
    }

    private static void Scalar(FiniteGeometryOutcome64? result,double expected)
    {
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Scalar);
        Assert.IsTrue(global::System.Math.Abs(result.Scalar.Value-expected)<=1e-12d);
    }

    private static void Vector(double[]? actual,double[] expected)
    {
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Length,actual.Length);
        for(var i=0;i<expected.Length;i++)
            Assert.IsTrue(global::System.Math.Abs(actual[i]-expected[i])<=1e-12d);
    }

    private static void Bins(Complex[]? actual,Complex[] expected)
    {
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Length,actual.Length);
        for(var i=0;i<expected.Length;i++)
        {
            Assert.IsTrue(global::System.Math.Abs(actual[i].Real-expected[i].Real)<=1e-10d);
            Assert.IsTrue(global::System.Math.Abs(actual[i].Imaginary-expected[i].Imaginary)<=1e-10d);
        }
    }

    private static void Canonical(M6W02C03Outcome64 result)
    {
        Assert.IsTrue(result.Succeeded);
        Scalar(result.Norm,5d);
        Scalar(result.Distance,5d);
        Vector(result.Projection!.Coordinates,[3d,0d]);
        Bins(result.Dft!.Bins,[Complex.One,Complex.One,Complex.One,Complex.One]);
        Vector(result.LinearConvolution!.Samples,[1d,3d,2d]);
        Vector(result.Differences!.Samples,[2d,3d]);
        Vector(result.MovingAverage!.Samples,[2d,4.5d]);
    }

    private static void Refused(M6W02C03Outcome64 result,M6W02C03Status64 status)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Norm);
        Assert.IsNull(result.Distance);
        Assert.IsNull(result.Projection);
        Assert.IsNull(result.Dft);
        Assert.IsNull(result.LinearConvolution);
        Assert.IsNull(result.Differences);
        Assert.IsNull(result.MovingAverage);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    [TestMethod] public void CanonicalAllSevenOracles()
    { Canonical(Evaluate(Good())); }

    [TestMethod] public void RuntimeAggregateIdentity()
    { var o=Evaluate(Good());Assert.IsTrue(o.Succeeded);Assert.AreEqual(M6W02C03Contract.RuleId,o.RuleId); }

    [TestMethod] public void NoCertifiedAggregateErrorBound()
    { var o=Evaluate(Good());Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.ErrorBoundGuaranteed); }

    [TestMethod] public void NormThreeFourFive()
    { var o=Accepted(Good());Scalar(o.Norm,5d); }

    [TestMethod] public void DistanceThreeFourFive()
    { var o=Accepted(Good());Scalar(o.Distance,5d); }

    [TestMethod] public void ProjectionXAxis()
    { var o=Accepted(Good());Vector(o.Projection!.Coordinates,[3d,0d]); }

    [TestMethod] public void ImpulseSpectrumFour()
    { var o=Accepted(Good());Bins(o.Dft!.Bins,[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void LinearConvolutionOneTwo()
    { var o=Accepted(Good());Vector(o.LinearConvolution!.Samples,[1d,3d,2d]); }

    [TestMethod] public void UniformFirstDifferences()
    { var o=Accepted(Good());Vector(o.Differences!.Samples,[2d,3d]); }

    [TestMethod] public void UniformTwoPointMeans()
    { var o=Accepted(Good());Vector(o.MovingAverage!.Samples,[2d,4.5d]); }

    [TestMethod] public void AllSevenResultsHaveIndependentRuleIds()
    { var o=Accepted(Good());Assert.AreEqual(FiniteGeometry64.RuleId,o.Norm!.RuleId);Assert.AreEqual(FiniteGeometry64.RuleId,o.Distance!.RuleId);Assert.AreEqual(FiniteGeometry64.RuleId,o.Projection!.RuleId);Assert.AreEqual(FiniteSignal64.RuleId,o.Dft!.RuleId);Assert.AreEqual(FiniteSignal64.RuleId,o.LinearConvolution!.RuleId);Assert.AreEqual(FiniteUniformSeries64.RuleId,o.Differences!.RuleId);Assert.AreEqual(FiniteUniformSeries64.RuleId,o.MovingAverage!.RuleId); }

    [TestMethod] public void SevenProviderOutcomesAllSuccessful()
    { var o=Accepted(Good());Assert.IsTrue(o.Norm!.Succeeded);Assert.IsTrue(o.Distance!.Succeeded);Assert.IsTrue(o.Projection!.Succeeded);Assert.IsTrue(o.Dft!.Succeeded);Assert.IsTrue(o.LinearConvolution!.Succeeded);Assert.IsTrue(o.Differences!.Succeeded);Assert.IsTrue(o.MovingAverage!.Succeeded); }

    [TestMethod] public void UsesDirectDftForwardFlag()
    { var o=Accepted(Good());Assert.IsFalse(o.Dft!.Inverse); }

    [TestMethod] public void UsesNoncircularConvolutionLength()
    { var o=Accepted(Good());Assert.AreEqual(3,o.LinearConvolution!.Samples!.Length); }

    [TestMethod] public void UsesSeriesWindowTwoMetadata()
    { var o=Accepted(Good());Assert.AreEqual(2,o.MovingAverage!.Window);Assert.AreEqual(1d,o.MovingAverage.SampleInterval); }

    [TestMethod] public void NoHiddenTimeDivision()
    { var o=Accepted(Good() with {SampleInterval=0.5d});Vector(o.Differences!.Samples,[2d,3d]);Assert.AreEqual(0.5d,o.Differences.SampleInterval); }

    [TestMethod] public void ModifiedCartesianFrameThreeDimensions()
    { var o=Accepted(Good() with {GeometryStart=[0d,0d,0d],GeometryEnd=[3d,4d,0d],ProjectionAxis=[0d,1d,0d]});Scalar(o.Norm,5d);Vector(o.Projection!.Coordinates,[0d,4d,0d]); }

    [TestMethod] public void ModifiedOriginDistance()
    { var o=Accepted(Good() with {GeometryStart=[3d,0d],GeometryEnd=[3d,4d]});Scalar(o.Distance,4d); }

    [TestMethod] public void ModifiedComplexSignal()
    { var o=Accepted(Good() with {DiscreteSamples=[Complex.One,Complex.Zero]});Bins(o.Dft!.Bins,[Complex.One,Complex.One]); }

    [TestMethod] public void ModifiedRealConvolution()
    { var o=Accepted(Good() with {ConvolutionLeft=[2d,3d],ConvolutionRight=[1d]});Vector(o.LinearConvolution!.Samples,[2d,3d]); }

    [TestMethod] public void ModifiedUniformSeries()
    { var o=Accepted(Good() with {UniformSamples=[2d,4d,6d],MovingWindow=3});Vector(o.Differences!.Samples,[2d,2d]);Vector(o.MovingAverage!.Samples,[4d]); }

    [TestMethod] public void SingleSeriesWindowIdentity()
    { var o=Accepted(Good() with {MovingWindow=1});Vector(o.MovingAverage!.Samples,[1d,3d,6d]); }

    [TestMethod] public void NegativeSeriesValues()
    { var o=Accepted(Good() with {UniformSamples=[-1d,-3d,-6d]});Vector(o.Differences!.Samples,[-2d,-3d]);Vector(o.MovingAverage!.Samples,[-2d,-4.5d]); }

    [TestMethod] public void ValidZeroGeometryNorm()
    { var o=Accepted(Good() with {GeometryStart=[0d,0d],GeometryEnd=[0d,0d],ProjectionAxis=[1d,0d]});Scalar(o.Norm,0d);Scalar(o.Distance,0d);Vector(o.Projection!.Coordinates,[0d,0d]); }

    [TestMethod] public void ValidZeroDiscreteSignal()
    { var o=Accepted(Good() with {DiscreteSamples=[Complex.Zero,Complex.Zero]});Bins(o.Dft!.Bins,[Complex.Zero,Complex.Zero]); }

    [TestMethod] public void NullAggregateRequestRefused()
    { Refused(Evaluate(null),M6W02C03Status64.InvalidRequest); }

    [TestMethod] public void PreCancelledRequestRefused()
    { using var cancel=new CancellationTokenSource();cancel.Cancel();Refused(M6W02C03Contract.Evaluate(Good(),cancel.Token),M6W02C03Status64.Cancelled); }

    [TestMethod] public void PreCancelledNullRequestPriority()
    { using var cancel=new CancellationTokenSource();cancel.Cancel();Refused(M6W02C03Contract.Evaluate(null,cancel.Token),M6W02C03Status64.Cancelled); }

    [TestMethod] public void NullGeometryEndRefused()
    { Refused(Evaluate(Good() with {GeometryEnd=null}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void NullGeometryStartRefused()
    { Refused(Evaluate(Good() with {GeometryStart=null}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void NullProjectionAxisRefused()
    { Refused(Evaluate(Good() with {ProjectionAxis=null}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void MismatchedGeometryDimensionsRefused()
    { Refused(Evaluate(Good() with {GeometryStart=[0d,0d,0d]}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void ZeroProjectionAxisRefused()
    { Refused(Evaluate(Good() with {ProjectionAxis=[0d,0d]}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void NonfiniteGeometryCoordinateRefused()
    { Refused(Evaluate(Good() with {GeometryEnd=[double.NaN,4d]}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void OverBudgetGeometryRefused()
    { Refused(Evaluate(Good() with {GeometryEnd=[1e101d,0d]}),M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void NullDiscreteSamplesRefused()
    { Refused(Evaluate(Good() with {DiscreteSamples=null}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void EmptyDiscreteSamplesRefused()
    { Refused(Evaluate(Good() with {DiscreteSamples=[]}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void InvalidComplexSampleRefused()
    { Refused(Evaluate(Good() with {DiscreteSamples=[new Complex(double.NaN,0d)]}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void OverBudgetComplexSampleRefused()
    { Refused(Evaluate(Good() with {DiscreteSamples=[new Complex(1e7d,0d)]}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void NullConvolutionOperandRefused()
    { Refused(Evaluate(Good() with {ConvolutionLeft=null}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void EmptyConvolutionOperandRefused()
    { Refused(Evaluate(Good() with {ConvolutionRight=[]}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void InvalidConvolutionSampleRefused()
    { Refused(Evaluate(Good() with {ConvolutionRight=[double.PositiveInfinity]}),M6W02C03Status64.SignalRefused); }

    [TestMethod] public void NullUniformSeriesRefused()
    { Refused(Evaluate(Good() with {UniformSamples=null}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void SingleUniformSampleRefused()
    { Refused(Evaluate(Good() with {UniformSamples=[1d]}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void MissingUniformValueRefused()
    { Refused(Evaluate(Good() with {UniformSamples=[1d,double.NaN,6d]}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void ZeroSampleIntervalRefused()
    { Refused(Evaluate(Good() with {SampleInterval=0d}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void NegativeSampleIntervalRefused()
    { Refused(Evaluate(Good() with {SampleInterval=-1d}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void NonfiniteIntervalRefused()
    { Refused(Evaluate(Good() with {SampleInterval=double.PositiveInfinity}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void ZeroMovingWindowRefused()
    { Refused(Evaluate(Good() with {MovingWindow=0}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void OversizedMovingWindowRefused()
    { Refused(Evaluate(Good() with {MovingWindow=4}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void OverBudgetUniformValueRefused()
    { Refused(Evaluate(Good() with {UniformSamples=[1e7d,2d]}),M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void GeometryRefusalReturnsNoPartialSignalData()
    { var o=Evaluate(Good() with {ProjectionAxis=[0d,0d]});Refused(o,M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void SignalRefusalReturnsNoPartialGeometryData()
    { var o=Evaluate(Good() with {ConvolutionLeft=null});Refused(o,M6W02C03Status64.SignalRefused); }

    [TestMethod] public void SeriesRefusalReturnsNoPartialPriorFamilies()
    { var o=Evaluate(Good() with {SampleInterval=0d});Refused(o,M6W02C03Status64.UniformSeriesRefused); }

    [TestMethod] public void GeometryRefusalPriorityOverInvalidSignals()
    { var o=Evaluate(Good() with {ProjectionAxis=[0d,0d],DiscreteSamples=null});Refused(o,M6W02C03Status64.GeometryRefused); }

    [TestMethod] public void SignalRefusalPriorityOverInvalidSeries()
    { var o=Evaluate(Good() with {ConvolutionLeft=null,SampleInterval=0d});Refused(o,M6W02C03Status64.SignalRefused); }
}
