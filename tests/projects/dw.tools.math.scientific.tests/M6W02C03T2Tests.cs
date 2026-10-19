using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03T2Tests
{
    private static M6W02C03Request64 Base() =>
        new([0d,0d],[3d,4d],[1d,0d],
            [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
            [1d,2d],[1d,1d],[1d,3d,6d],1d,2);

    private static M6W02C03BoundaryRequest64 Good() =>
        new(Base(),[0d,1d,2d],
            M6W02C03UnitPolicy64.CallerManagesIndependentUnits,
            FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples,
            FiniteSignalConvolutionPolicy64.LinearZeroExtended);

    private static M6W02C03BoundaryOutcome64 Evaluate(M6W02C03BoundaryRequest64? request) =>
        M6W02C03BoundaryAggregate64.Evaluate(request);

    private static M6W02C03BoundaryOutcome64 Accepted(M6W02C03BoundaryRequest64 request)
    {
        var outcome=Evaluate(request);
        Assert.IsTrue(outcome.Succeeded);
        Assert.AreEqual(M6W02C03BoundaryStatus64.Success,outcome.Status);
        Assert.IsFalse(outcome.ErrorBoundGuaranteed);
        return outcome;
    }

    private static void Canonical(M6W02C03BoundaryOutcome64 outcome)
    {
        Assert.IsTrue(outcome.Succeeded);
        Assert.AreEqual(5d,outcome.Norm!.Outcome!.Scalar!.Value);
        Assert.AreEqual(5d,outcome.Distance!.Outcome!.Scalar!.Value);
        Vector(outcome.Projection!.Outcome!.Coordinates,[3d,0d]);
        Bins(outcome.Dft!.Transform!.Bins,[Complex.One,Complex.One,Complex.One,Complex.One]);
        Vector(outcome.LinearConvolution!.Convolution!.Samples,[1d,3d,2d]);
        Vector(outcome.Differences!.Outcome!.Samples,[2d,3d]);
        Vector(outcome.MovingAverage!.Outcome!.Samples,[2d,4.5d]);
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

    private static void Refused(M6W02C03BoundaryOutcome64 actual,M6W02C03BoundaryStatus64 status,
        FiniteGeometryAuditStatus64? geometry=null,
        FiniteSignalAuditStatus64? signal=null,
        FiniteUniformSeriesAuditStatus64? series=null)
    {
        Assert.AreEqual(status,actual.Status);
        Assert.IsFalse(actual.Succeeded);
        Assert.IsNull(actual.Norm);
        Assert.IsNull(actual.Distance);
        Assert.IsNull(actual.Projection);
        Assert.IsNull(actual.Dft);
        Assert.IsNull(actual.LinearConvolution);
        Assert.IsNull(actual.Differences);
        Assert.IsNull(actual.MovingAverage);
        Assert.AreEqual(geometry,actual.GeometryReason);
        Assert.AreEqual(signal,actual.SignalReason);
        Assert.AreEqual(series,actual.UniformSeriesReason);
        Assert.IsFalse(actual.ErrorBoundGuaranteed);
    }

    [TestMethod] public void CanonicalSevenRiskAudits()
    { Canonical(Accepted(Good())); }

    [TestMethod] public void RuntimeAggregateRuleId()
    { var outcome=Accepted(Good());Assert.AreEqual(M6W02C03BoundaryAggregate64.RuleId,outcome.RuleId); }

    [TestMethod] public void NoClaimOfErrorBound()
    { var outcome=Accepted(Good());Assert.IsFalse(outcome.ErrorBoundGuaranteed);Assert.IsFalse(outcome.Norm!.ErrorBoundGuaranteed);Assert.IsFalse(outcome.Dft!.ErrorBoundGuaranteed); }

    [TestMethod] public void AllSuccessReasonsAreAbsent()
    { var outcome=Accepted(Good());Assert.IsNull(outcome.GeometryReason);Assert.IsNull(outcome.SignalReason);Assert.IsNull(outcome.UniformSeriesReason); }

    [TestMethod] public void SevenDirectBackendResultsRemainQualified()
    { var outcome=Accepted(Good());Assert.IsTrue(outcome.Norm!.Outcome!.Succeeded);Assert.IsTrue(outcome.Distance!.Outcome!.Succeeded);Assert.IsTrue(outcome.Projection!.Outcome!.Succeeded);Assert.IsTrue(outcome.Dft!.Transform!.Succeeded);Assert.IsTrue(outcome.LinearConvolution!.Convolution!.Succeeded);Assert.IsTrue(outcome.Differences!.Outcome!.Succeeded);Assert.IsTrue(outcome.MovingAverage!.Outcome!.Succeeded); }

    [TestMethod] public void EuclideanNormThreeFourFive()
    { var outcome=Accepted(Good());Assert.AreEqual(5d,outcome.Norm!.Outcome!.Scalar!.Value); }

    [TestMethod] public void EuclideanDistanceThreeFourFive()
    { var outcome=Accepted(Good());Assert.AreEqual(5d,outcome.Distance!.Outcome!.Scalar!.Value); }

    [TestMethod] public void CartesianXProjection()
    { var outcome=Accepted(Good());Vector(outcome.Projection!.Outcome!.Coordinates,[3d,0d]); }

    [TestMethod] public void DirectDftImpulseSpectrum()
    { var outcome=Accepted(Good());Bins(outcome.Dft!.Transform!.Bins,[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void LinearConvolutionZeroExtended()
    { var outcome=Accepted(Good());Vector(outcome.LinearConvolution!.Convolution!.Samples,[1d,3d,2d]); }

    [TestMethod] public void RawSeriesFirstDifferences()
    { var outcome=Accepted(Good());Vector(outcome.Differences!.Outcome!.Samples,[2d,3d]); }

    [TestMethod] public void CompleteSeriesMovingAverages()
    { var outcome=Accepted(Good());Vector(outcome.MovingAverage!.Outcome!.Samples,[2d,4.5d]); }

    [TestMethod] public void AdmitsIndependentThreeDimensionalCartesian()
    { var result=Accepted(Good() with {Scientific=Base() with {GeometryStart=[0d,0d,0d],GeometryEnd=[3d,4d,0d],ProjectionAxis=[0d,1d,0d]}});Vector(result.Projection!.Outcome!.Coordinates,[0d,4d,0d]); }

    [TestMethod] public void AdmitsCartesianZeroPosition()
    { var result=Accepted(Good() with {Scientific=Base() with {GeometryStart=[0d,0d],GeometryEnd=[0d,0d]}});Assert.AreEqual(0d,result.Norm!.Outcome!.Scalar!.Value);Assert.AreEqual(0d,result.Distance!.Outcome!.Scalar!.Value); }

    [TestMethod] public void AdmitsChangedDistanceStart()
    { var result=Accepted(Good() with {Scientific=Base() with {GeometryStart=[3d,0d]}});Assert.AreEqual(4d,result.Distance!.Outcome!.Scalar!.Value); }

    [TestMethod] public void AdmitsShiftedImpulseNegativeDftSign()
    { var result=Accepted(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.Zero,Complex.One,Complex.Zero,Complex.Zero]}});Bins(result.Dft!.Transform!.Bins,[Complex.One,-Complex.ImaginaryOne,-Complex.One,Complex.ImaginaryOne]); }

    [TestMethod] public void AdmitsComplexImpulse()
    { var z=new Complex(2d,-1d);var result=Accepted(Good() with {Scientific=Base() with {DiscreteSamples=[z,Complex.Zero]}});Bins(result.Dft!.Transform!.Bins,[z,z]); }

    [TestMethod] public void AdmitsZeroDiscreteSignal()
    { var result=Accepted(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.Zero,Complex.Zero]}});Bins(result.Dft!.Transform!.Bins,[Complex.Zero,Complex.Zero]); }

    [TestMethod] public void AdmitsSingleDiscreteSample()
    { var result=Accepted(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.One]}});Bins(result.Dft!.Transform!.Bins,[Complex.One]); }

    [TestMethod] public void AdmitsDifferentLinearConvolution()
    { var result=Accepted(Good() with {Scientific=Base() with {ConvolutionLeft=[1d,2d],ConvolutionRight=[3d,4d]}});Vector(result.LinearConvolution!.Convolution!.Samples,[3d,10d,8d]); }

    [TestMethod] public void AdmitsNegativeNonCancellingConvolution()
    { var result=Accepted(Good() with {Scientific=Base() with {ConvolutionLeft=[-1d],ConvolutionRight=[2d]}});Vector(result.LinearConvolution!.Convolution!.Samples,[-2d]); }

    [TestMethod] public void AdmitsDifferentUniformSamples()
    { var result=Accepted(Good() with {Scientific=Base() with {UniformSamples=[2d,4d,6d],MovingWindow=3}});Vector(result.Differences!.Outcome!.Samples,[2d,2d]);Vector(result.MovingAverage!.Outcome!.Samples,[4d]); }

    [TestMethod] public void AdmitsNegativeUniformValues()
    { var result=Accepted(Good() with {Scientific=Base() with {UniformSamples=[-1d,-3d,-6d]}});Vector(result.Differences!.Outcome!.Samples,[-2d,-3d]);Vector(result.MovingAverage!.Outcome!.Samples,[-2d,-4.5d]); }

    [TestMethod] public void AdmitsAllZeroUniformSeries()
    { var result=Accepted(Good() with {Scientific=Base() with {UniformSamples=[0d,0d,0d]}});Vector(result.Differences!.Outcome!.Samples,[0d,0d]);Vector(result.MovingAverage!.Outcome!.Samples,[0d,0d]); }

    [TestMethod] public void AdmitsCompleteWindowOne()
    { var result=Accepted(Good() with {Scientific=Base() with {MovingWindow=1}});Vector(result.MovingAverage!.Outcome!.Samples,[1d,3d,6d]); }

    [TestMethod] public void AdmitsHalfIntervalWithMatchingGrid()
    { var result=Accepted(Good() with {Scientific=Base() with {SampleInterval=0.5d},SampleTimes=[0d,0.5d,1d]});Vector(result.Differences!.Outcome!.Samples,[2d,3d]);Assert.AreEqual(0.5d,result.Differences.Outcome.SampleInterval); }

    [TestMethod] public void AdmitsFractionalGridWithinTolerance()
    { var result=Accepted(Good() with {Scientific=Base() with {SampleInterval=0.1d},SampleTimes=[0d,0.1d,0.2d]});Assert.IsTrue(result.Succeeded); }

    [TestMethod] public void AdmitsNegativeTimeOrigin()
    { var result=Accepted(Good() with {SampleTimes=[-2d,-1d,0d]});Vector(result.MovingAverage!.Outcome!.Samples,[2d,4.5d]); }

    [TestMethod] public void AdmitsModerateTimeOffset()
    { var result=Accepted(Good() with {SampleTimes=[1e6d,1e6d+1d,1e6d+2d]});Assert.IsTrue(result.Succeeded); }

    [TestMethod] public void AdmitsMax32UniformPoints()
    { var values=Enumerable.Repeat(1d,32).ToArray();var times=Enumerable.Range(0,32).Select(i=>(double)i).ToArray();var result=Accepted(Good() with {Scientific=Base() with {UniformSamples=values},SampleTimes=times});Assert.AreEqual(31,result.Differences!.Outcome!.Samples!.Length); }

    [TestMethod] public void NullBoundaryRequestRefused()
    { Refused(Evaluate(null),M6W02C03BoundaryStatus64.InvalidRequest); }

    [TestMethod] public void NullScientificInputRefused()
    { Refused(Evaluate(Good() with {Scientific=null}),M6W02C03BoundaryStatus64.InvalidRequest); }

    [TestMethod] public void UnitPolicyRequired()
    { Refused(Evaluate(Good() with {UnitPolicy=M6W02C03UnitPolicy64.Unspecified}),M6W02C03BoundaryStatus64.UnitPolicyRequired); }

    [TestMethod] public void UnknownUnitPolicyRefused()
    { Refused(Evaluate(Good() with {UnitPolicy=(M6W02C03UnitPolicy64)99}),M6W02C03BoundaryStatus64.UnitPolicyRequired); }

    [TestMethod] public void PreCancelledOverridesNull()
    { using var cancel=new CancellationTokenSource();cancel.Cancel();Refused(M6W02C03BoundaryAggregate64.Evaluate(null,cancel.Token),M6W02C03BoundaryStatus64.Cancelled); }

    [TestMethod] public void PreCancelledOverridesMissingUnits()
    { using var cancel=new CancellationTokenSource();cancel.Cancel();Refused(M6W02C03BoundaryAggregate64.Evaluate(Good() with {UnitPolicy=M6W02C03UnitPolicy64.Unspecified},cancel.Token),M6W02C03BoundaryStatus64.Cancelled); }

    [TestMethod] public void GeometryInvalidEndRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryEnd=null}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void GeometryInvalidStartRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryStart=null}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void GeometryProjectionAxisMissing()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ProjectionAxis=null}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void GeometryZeroAxisRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ProjectionAxis=[0d,0d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.ZeroAxis); }

    [TestMethod] public void GeometryDimensionMismatchRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryStart=[0d,0d,0d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void GeometryNonfiniteRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryEnd=[double.NaN,4d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.InvalidInput); }

    [TestMethod] public void GeometryOverBudgetRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryEnd=[1e101d,0d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void GeometryRelativeResolutionRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryStart=[1e14d,0d],GeometryEnd=[1e14d+1d,0d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.RelativeResolutionRisk); }

    [TestMethod] public void GeometryProjectionCancellationRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {GeometryEnd=[1e10d,1e10d],ProjectionAxis=[1d,-1d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.ProjectionCancellationRisk); }

    [TestMethod] public void SignalSamplingPolicyMissing()
    { Refused(Evaluate(Good() with {SamplingPolicy=FiniteSignalSamplingPolicy64.Unspecified}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.SamplingPolicyRequired); }

    [TestMethod] public void SignalUnknownSamplingPolicy()
    { Refused(Evaluate(Good() with {SamplingPolicy=(FiniteSignalSamplingPolicy64)99}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.SamplingPolicyRequired); }

    [TestMethod] public void SignalTopologyMissing()
    { Refused(Evaluate(Good() with {ConvolutionTopology=FiniteSignalConvolutionPolicy64.Unspecified}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.ConvolutionTopologyRequired); }

    [TestMethod] public void SignalCircularConvolutionRefused()
    { Refused(Evaluate(Good() with {ConvolutionTopology=FiniteSignalConvolutionPolicy64.CircularUnsupported}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.CircularConvolutionUnsupported); }

    [TestMethod] public void SignalDftSpectralCancellationRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.One,Complex.One]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.SpectralCancellationRisk); }

    [TestMethod] public void SignalDftAlternatingSpectralRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.One,-Complex.One]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.SpectralCancellationRisk); }

    [TestMethod] public void SignalConvolutionCancellationRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ConvolutionLeft=[1d,-1d],ConvolutionRight=[1d,1d]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.ConvolutionCancellationRisk); }

    [TestMethod] public void SignalNearConvolutionCancellationRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ConvolutionLeft=[1d,-1d+1e-13d],ConvolutionRight=[1d,1d]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.ConvolutionCancellationRisk); }

    [TestMethod] public void SignalInvalidLengthRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void SignalNonfiniteDftRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[new Complex(double.NaN,0d)]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void SignalOutOfBudgetDftRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[new Complex(1e7d,0d)]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void SignalMissingConvolutionOperandRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ConvolutionLeft=null}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void SignalNonfiniteConvolutionRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ConvolutionRight=[double.PositiveInfinity]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void UniformMissingTimesRefused()
    { Refused(Evaluate(Good() with {SampleTimes=null}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.SampleTimesRequired); }

    [TestMethod] public void UniformMismatchedTimesRefused()
    { Refused(Evaluate(Good() with {SampleTimes=[0d,1d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.InvalidTimeLength); }

    [TestMethod] public void UniformDuplicateTimesRefused()
    { Refused(Evaluate(Good() with {SampleTimes=[0d,0d,2d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void UniformDecreasingTimesRefused()
    { Refused(Evaluate(Good() with {SampleTimes=[0d,-1d,-2d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.NonIncreasingTime); }

    [TestMethod] public void UniformIrregularTimeRefused()
    { Refused(Evaluate(Good() with {SampleTimes=[0d,1d,3d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.IrregularSampling); }

    [TestMethod] public void UniformTimestampResolutionRisk()
    { Refused(Evaluate(Good() with {SampleTimes=[1e14d,1e14d+1d,1e14d+2d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk); }

    [TestMethod] public void UniformRelativeDifferenceRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {UniformSamples=[1e5d,1e5d+1e-8d]},SampleTimes=[0d,1d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk); }

    [TestMethod] public void UniformMeanCancellationRisk()
    { Refused(Evaluate(Good() with {Scientific=Base() with {UniformSamples=[1d,-1d]},SampleTimes=[0d,1d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.MeanCancellationRisk); }

    [TestMethod] public void UniformNullSamplesRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {UniformSamples=null}}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.InvalidLength); }

    [TestMethod] public void UniformMissingDataRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {UniformSamples=[1d,double.NaN,6d]}}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.InvalidInput); }

    [TestMethod] public void UniformInvalidIntervalRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {SampleInterval=0d}}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.InvalidSampleInterval); }

    [TestMethod] public void UniformInvalidWindowRefused()
    { Refused(Evaluate(Good() with {Scientific=Base() with {MovingWindow=4}}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.InvalidWindow); }

    [TestMethod] public void GeometryRefusalPrecedesSignalRefusal()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ProjectionAxis=[0d,0d],DiscreteSamples=null}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.ZeroAxis); }

    [TestMethod] public void SignalRefusalPrecedesSeriesRefusal()
    { Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=null,UniformSamples=null}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void UnitPolicyPrecedesGeometryError()
    { Refused(Evaluate(Good() with {UnitPolicy=M6W02C03UnitPolicy64.Unspecified,Scientific=Base() with {GeometryEnd=null}}),M6W02C03BoundaryStatus64.UnitPolicyRequired); }

    [TestMethod] public void NoGeometryPartialDataAfterSignalRefusal()
    { Refused(Evaluate(Good() with {Scientific=Base() with {ConvolutionLeft=null}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void NoSignalPartialDataAfterSeriesRefusal()
    { Refused(Evaluate(Good() with {SampleTimes=null}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.SampleTimesRequired); }

    [TestMethod] public void RawDftDirectFacadeStillCallable()
    { var raw=FiniteSignal64.DirectDft([Complex.One,Complex.One]);Assert.IsTrue(raw.Succeeded);Refused(Evaluate(Good() with {Scientific=Base() with {DiscreteSamples=[Complex.One,Complex.One]}}),M6W02C03BoundaryStatus64.SignalRefused,signal:FiniteSignalAuditStatus64.SpectralCancellationRisk); }

    [TestMethod] public void RawGeometryDirectFacadeStillCallable()
    { var raw=FiniteGeometry64.Distance([1e14d,0d],[1e14d+1d,0d]);Assert.IsTrue(raw.Succeeded);Refused(Evaluate(Good() with {Scientific=Base() with {GeometryStart=[1e14d,0d],GeometryEnd=[1e14d+1d,0d]}}),M6W02C03BoundaryStatus64.GeometryRefused,geometry:FiniteGeometryAuditStatus64.RelativeResolutionRisk); }

    [TestMethod] public void RawUniformDirectFacadeStillCallable()
    { var raw=FiniteUniformSeries64.MovingAverage([1d,-1d],1d,2);Assert.IsTrue(raw.Succeeded);Refused(Evaluate(Good() with {Scientific=Base() with {UniformSamples=[1d,-1d]},SampleTimes=[0d,1d]}),M6W02C03BoundaryStatus64.UniformSeriesRefused,series:FiniteUniformSeriesAuditStatus64.MeanCancellationRisk); }
}
