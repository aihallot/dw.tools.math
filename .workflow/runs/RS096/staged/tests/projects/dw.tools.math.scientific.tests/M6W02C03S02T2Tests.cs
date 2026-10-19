using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S02T2Tests
{
    private const FiniteSignalSamplingPolicy64 AcceptedSampling =
        FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples;
    private const FiniteSignalConvolutionPolicy64 Linear =
        FiniteSignalConvolutionPolicy64.LinearZeroExtended;

    private static void Dft(FiniteSignalAuditOutcome64 result, Complex[] expected,
        double tolerance = 1e-10d)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteSignalAuditStatus64.Success, result.Status);
        Assert.IsNull(result.Convolution);
        Assert.IsNotNull(result.Transform);
        Assert.IsNotNull(result.Transform.Bins);
        Assert.AreEqual(expected.Length, result.Transform.Bins.Length);
        for(var i=0;i<expected.Length;i++)
        {
            Assert.IsTrue(global::System.Math.Abs(result.Transform.Bins[i].Real-expected[i].Real) <= tolerance);
            Assert.IsTrue(global::System.Math.Abs(result.Transform.Bins[i].Imaginary-expected[i].Imaginary) <= tolerance);
        }
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Convolution(FiniteSignalAuditOutcome64 result, double[] expected,
        double tolerance=1e-12d)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteSignalAuditStatus64.Success, result.Status);
        Assert.IsNull(result.Transform);
        Assert.IsNotNull(result.Convolution);
        Assert.IsNotNull(result.Convolution.Samples);
        Assert.AreEqual(expected.Length,result.Convolution.Samples.Length);
        for(var i=0;i<expected.Length;i++)
            Assert.IsTrue(global::System.Math.Abs(result.Convolution.Samples[i]-expected[i])<=tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Refused(FiniteSignalAuditOutcome64 result,
        FiniteSignalAuditStatus64 expected,bool requiresRatio=false)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Transform);
        Assert.IsNull(result.Convolution);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
        if(requiresRatio)
        {
            Assert.IsNotNull(result.MinimumSignalRatio);
            Assert.IsTrue(double.IsFinite(result.MinimumSignalRatio.Value));
            Assert.IsTrue(result.MinimumSignalRatio.Value >= 0d);
        }
    }

    [TestMethod] public void DftRequiresExplicitSamplingPolicy()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One]),FiniteSignalAuditStatus64.SamplingPolicyRequired); }

    [TestMethod] public void DftRejectsUnknownSamplingPolicy()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:(FiniteSignalSamplingPolicy64)99),FiniteSignalAuditStatus64.SamplingPolicyRequired); }

    [TestMethod] public void DftPreCancelledOverridesMissingPolicy()
    { using var src=new CancellationTokenSource();src.Cancel();Refused(FiniteSignalBoundaryAudit64.EvaluateDft(null,cancellationToken:src.Token),FiniteSignalAuditStatus64.Cancelled); }

    [TestMethod] public void DftAcceptedSingleImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:AcceptedSampling),[Complex.One]); }

    [TestMethod] public void DftAcceptedTwoImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.Zero],sampling:AcceptedSampling),[Complex.One,Complex.One]); }

    [TestMethod] public void DftAcceptedFourImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],sampling:AcceptedSampling),[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void DftAcceptedThreeImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.Zero,Complex.Zero],sampling:AcceptedSampling),[Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void DftAcceptedThirtyTwoImpulse()
    { var input=new Complex[32];input[0]=Complex.One;Dft(FiniteSignalBoundaryAudit64.EvaluateDft(input,sampling:AcceptedSampling),Enumerable.Repeat(Complex.One,32).ToArray()); }

    [TestMethod] public void DftAcceptedShiftedImpulseSign()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.Zero,Complex.One,Complex.Zero,Complex.Zero],sampling:AcceptedSampling),[Complex.One,-Complex.ImaginaryOne,-Complex.One,Complex.ImaginaryOne]); }

    [TestMethod] public void DftAcceptedImaginaryImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.ImaginaryOne,Complex.Zero],sampling:AcceptedSampling),[Complex.ImaginaryOne,Complex.ImaginaryOne]); }

    [TestMethod] public void DftAcceptedComplexImpulse()
    { var z=new Complex(2d,-1d);Dft(FiniteSignalBoundaryAudit64.EvaluateDft([z,Complex.Zero],sampling:AcceptedSampling),[z,z]); }

    [TestMethod] public void DftAcceptedInverseDc()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(4d,0d),Complex.Zero,Complex.Zero,Complex.Zero],inverse:true,sampling:AcceptedSampling),[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void DftAcceptedInverseSingle()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(3d,2d)],inverse:true,sampling:AcceptedSampling),[new Complex(3d,2d)]); }

    [TestMethod] public void DftAcceptedInverseShiftedSpectrum()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.Zero,new Complex(4d,0d),Complex.Zero,Complex.Zero],inverse:true,sampling:AcceptedSampling),[Complex.One,Complex.ImaginaryOne,-Complex.One,-Complex.ImaginaryOne]); }

    [TestMethod] public void DftAcceptedAllZeroSamples()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.Zero,Complex.Zero,Complex.Zero],sampling:AcceptedSampling),[Complex.Zero,Complex.Zero,Complex.Zero]); }

    [TestMethod] public void DftAcceptedTinyImpulse()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(1e-100d,0d)],sampling:AcceptedSampling),[new Complex(1e-100d,0d)],1e-110d); }

    [TestMethod] public void DftAcceptedMaximumInputMagnitude()
    { Dft(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(1e6d,0d)],sampling:AcceptedSampling),[new Complex(1e6d,0d)]); }

    [TestMethod] public void DftResultHasRuntimeRuleId()
    { var o=FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:AcceptedSampling);Dft(o,[Complex.One]);Assert.AreEqual(FiniteSignalBoundaryAudit64.RuleId,o.RuleId); }

    [TestMethod] public void DftRetainsT1RuleId()
    { var o=FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:AcceptedSampling);Dft(o,[Complex.One]);Assert.IsNotNull(o.Transform);Assert.AreEqual(FiniteSignal64.RuleId,o.Transform.RuleId); }

    [TestMethod] public void DftNoCertifiedRoundingError()
    { var o=FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:AcceptedSampling);Dft(o,[Complex.One]);Assert.IsFalse(o.ErrorBoundGuaranteed);Assert.IsFalse(o.Transform!.ErrorBoundGuaranteed); }

    [TestMethod] public void DftRelativeRatioObservedAtImpulse()
    { var o=FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.Zero],sampling:AcceptedSampling);Dft(o,[Complex.One,Complex.One]);Assert.IsNotNull(o.MinimumSignalRatio);Assert.IsTrue(global::System.Math.Abs(o.MinimumSignalRatio.Value-1d)<1e-12d); }

    [TestMethod] public void DftAllZeroHasNoRelativeRatio()
    { var o=FiniteSignalBoundaryAudit64.EvaluateDft([Complex.Zero,Complex.Zero],sampling:AcceptedSampling);Dft(o,[Complex.Zero,Complex.Zero]);Assert.IsNull(o.MinimumSignalRatio); }

    [TestMethod] public void DftRefusesSpectralCancellationOnConstantSamples()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.One,Complex.One,Complex.One],sampling:AcceptedSampling),FiniteSignalAuditStatus64.SpectralCancellationRisk,true); }

    [TestMethod] public void DftRefusesSpectralCancellationOnAlternatingSamples()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,-Complex.One],sampling:AcceptedSampling),FiniteSignalAuditStatus64.SpectralCancellationRisk,true); }

    [TestMethod] public void DftRefusesInverseCancellationOnFlatSpectrum()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.One,Complex.One,Complex.One],inverse:true,sampling:AcceptedSampling),FiniteSignalAuditStatus64.SpectralCancellationRisk,true); }

    [TestMethod] public void DftDirectFacadeRemainsCallable()
    { var raw=FiniteSignal64.DirectDft([Complex.One,Complex.One]);Assert.IsTrue(raw.Succeeded);Assert.IsNotNull(raw.Bins);Assert.AreEqual(2,raw.Bins.Length);Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One,Complex.One],sampling:AcceptedSampling),FiniteSignalAuditStatus64.SpectralCancellationRisk,true); }

    [TestMethod] public void DftRefusesNullInputWithPolicy()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft(null,sampling:AcceptedSampling),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void DftRefusesEmptyInputWithPolicy()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([],sampling:AcceptedSampling),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void DftRefusesOversizeInputWithPolicy()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft(new Complex[33],sampling:AcceptedSampling),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void DftRefusesNonfiniteReal()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(double.NaN,0d)],sampling:AcceptedSampling),FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void DftRefusesNonfiniteImaginary()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(0d,double.PositiveInfinity)],sampling:AcceptedSampling),FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void DftRefusesOversizeReal()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(1e7d,0d)],sampling:AcceptedSampling),FiniteSignalAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DftRefusesOversizeImaginary()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateDft([new Complex(0d,-1e7d)],sampling:AcceptedSampling),FiniteSignalAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DftRefusesPreCancelledWithPolicy()
    { using var src=new CancellationTokenSource();src.Cancel();Refused(FiniteSignalBoundaryAudit64.EvaluateDft([Complex.One],sampling:AcceptedSampling,cancellationToken:src.Token),FiniteSignalAuditStatus64.Cancelled); }

    [TestMethod] public void ConvolutionRequiresExplicitTopology()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d]),FiniteSignalAuditStatus64.ConvolutionTopologyRequired); }

    [TestMethod] public void ConvolutionRejectsUnknownTopology()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d],topology:(FiniteSignalConvolutionPolicy64)99),FiniteSignalAuditStatus64.ConvolutionTopologyRequired); }

    [TestMethod] public void ConvolutionRefusesCircularTopology()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d],[3d,4d],topology:FiniteSignalConvolutionPolicy64.CircularUnsupported),FiniteSignalAuditStatus64.CircularConvolutionUnsupported); }

    [TestMethod] public void ConvolutionPreCancelledOverridesMissingTopology()
    { using var src=new CancellationTokenSource();src.Cancel();Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution(null,null,cancellationToken:src.Token),FiniteSignalAuditStatus64.Cancelled); }

    [TestMethod] public void ConvolutionPrimaryOracle()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d],[1d,1d],topology:Linear),[1d,3d,2d]); }

    [TestMethod] public void ConvolutionCommutativeOracle()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,1d],[1d,2d],topology:Linear),[1d,3d,2d]); }

    [TestMethod] public void ConvolutionSingleValueOracle()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([2d],[3d],topology:Linear),[6d]); }

    [TestMethod] public void ConvolutionSignedNoCancellation()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([-1d],[3d],topology:Linear),[-3d]); }

    [TestMethod] public void ConvolutionImpulseIdentity()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[2d,3d,4d],topology:Linear),[2d,3d,4d]); }

    [TestMethod] public void ConvolutionShiftedImpulse()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([0d,1d,0d],[2d,3d],topology:Linear),[0d,2d,3d,0d]); }

    [TestMethod] public void ConvolutionAllZeroSamples()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([0d,0d],[1d,2d],topology:Linear);Convolution(o,[0d,0d,0d]);Assert.IsNull(o.MinimumSignalRatio); }

    [TestMethod] public void ConvolutionThreeByThree()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d,3d],[1d,1d,1d],topology:Linear),[1d,3d,6d,5d,3d]); }

    [TestMethod] public void ConvolutionRequiresZeroExtendedLength()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d],[3d,4d],topology:Linear);Convolution(o,[3d,10d,8d]);Assert.AreEqual(3,o.Convolution!.Samples!.Length); }

    [TestMethod] public void ConvolutionAdmitsThirtyTwoSamples()
    { var a=Enumerable.Repeat(1d,32).ToArray();var o=FiniteSignalBoundaryAudit64.EvaluateConvolution(a,a,topology:Linear);Assert.IsTrue(o.Succeeded);Assert.IsNotNull(o.Convolution);Assert.IsNotNull(o.Convolution.Samples);Assert.AreEqual(63,o.Convolution.Samples.Length);Assert.AreEqual(32d,o.Convolution.Samples[31]); }

    [TestMethod] public void ConvolutionAdmitsMaximumMagnitude()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1e6d],[1e6d],topology:Linear),[1e12d],1d); }

    [TestMethod] public void ConvolutionRuleIdFromRuntime()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d],topology:Linear);Convolution(o,[1d]);Assert.AreEqual(FiniteSignalBoundaryAudit64.RuleId,o.RuleId); }

    [TestMethod] public void ConvolutionRetainsT1RuleId()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d],topology:Linear);Convolution(o,[1d]);Assert.IsNotNull(o.Convolution);Assert.AreEqual(FiniteSignal64.RuleId,o.Convolution.RuleId); }

    [TestMethod] public void ConvolutionNoCertifiedRoundingError()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d],topology:Linear);Convolution(o,[1d]);Assert.IsFalse(o.ErrorBoundGuaranteed);Assert.IsFalse(o.Convolution!.ErrorBoundGuaranteed); }

    [TestMethod] public void ConvolutionRelativeRatioObserved()
    { var o=FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d],[1d,1d],topology:Linear);Convolution(o,[1d,3d,2d]);Assert.IsNotNull(o.MinimumSignalRatio);Assert.IsTrue(o.MinimumSignalRatio.Value>0.999d); }

    [TestMethod] public void ConvolutionRefusesSignedCancellation()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,-1d],[1d,1d],topology:Linear),FiniteSignalAuditStatus64.ConvolutionCancellationRisk,true); }

    [TestMethod] public void ConvolutionRefusesNearCancellation()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,-1d+1e-13d],[1d,1d],topology:Linear),FiniteSignalAuditStatus64.ConvolutionCancellationRisk,true); }

    [TestMethod] public void ConvolutionAdmitsResolvableCompensation()
    { Convolution(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,-1d+1e-9d],[1d,1d],topology:Linear),[1d,1e-9d,-1d+1e-9d],1e-12d); }

    [TestMethod] public void ConvolutionDirectFacadeRemainsCallable()
    { var direct=FiniteSignal64.LinearConvolution([1d,-1d],[1d,1d]);Assert.IsTrue(direct.Succeeded);Assert.IsNotNull(direct.Samples);Assert.AreEqual(0d,direct.Samples[1]);Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,-1d],[1d,1d],topology:Linear),FiniteSignalAuditStatus64.ConvolutionCancellationRisk,true); }

    [TestMethod] public void ConvolutionRefusesNullFirst()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution(null,[1d],topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesNullSecond()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],null,topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesEmptyFirst()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([],[1d],topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesEmptySecond()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[],topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesOversizeFirst()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution(new double[33],[1d],topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesOversizeSecond()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],new double[33],topology:Linear),FiniteSignalAuditStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionRefusesNaN()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([double.NaN],[1d],topology:Linear),FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void ConvolutionRefusesInfinity()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[double.PositiveInfinity],topology:Linear),FiniteSignalAuditStatus64.InvalidInput); }

    [TestMethod] public void ConvolutionRefusesBudget()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1e7d],[1d],topology:Linear),FiniteSignalAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void ConvolutionPreCancelledWithTopology()
    { using var src=new CancellationTokenSource();src.Cancel();Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d],[1d],topology:Linear,cancellationToken:src.Token),FiniteSignalAuditStatus64.Cancelled); }

    [TestMethod] public void ConvolutionTopologyCannotInventCircularOutput()
    { Refused(FiniteSignalBoundaryAudit64.EvaluateConvolution([1d,2d],[1d,1d],topology:FiniteSignalConvolutionPolicy64.CircularUnsupported),FiniteSignalAuditStatus64.CircularConvolutionUnsupported); }
}
