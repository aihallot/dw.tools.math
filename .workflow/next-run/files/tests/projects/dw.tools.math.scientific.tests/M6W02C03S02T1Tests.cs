using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S02T1Tests
{
    private static void Near(FiniteDirectDftOutcome64 observed, Complex[] expected,
        double tolerance=1e-10d)
    {
        Assert.IsTrue(observed.Succeeded);
        Assert.AreEqual(FiniteSignalStatus64.Success, observed.Status);
        Assert.IsNotNull(observed.Bins);
        Assert.AreEqual(expected.Length, observed.Bins.Length);
        for(var i=0;i<expected.Length;i++)
        {
            Assert.IsTrue(double.IsFinite(observed.Bins[i].Real));
            Assert.IsTrue(double.IsFinite(observed.Bins[i].Imaginary));
            Assert.IsTrue(global::System.Math.Abs(observed.Bins[i].Real-expected[i].Real)<=tolerance,
                $"DFT bin {i}: expected real {expected[i].Real:R} actual {observed.Bins[i].Real:R}");
            Assert.IsTrue(global::System.Math.Abs(observed.Bins[i].Imaginary-expected[i].Imaginary)<=tolerance,
                $"DFT bin {i}: expected imaginary {expected[i].Imaginary:R} actual {observed.Bins[i].Imaginary:R}");
        }
        Assert.IsFalse(observed.ErrorBoundGuaranteed);
    }

    private static void Vector(FiniteLinearConvolutionOutcome64 observed, double[] expected,
        double tolerance=1e-12d)
    {
        Assert.IsTrue(observed.Succeeded);
        Assert.AreEqual(FiniteSignalStatus64.Success, observed.Status);
        Assert.IsNotNull(observed.Samples);
        Assert.AreEqual(expected.Length, observed.Samples.Length);
        for(var i=0;i<expected.Length;i++)
            Assert.IsTrue(global::System.Math.Abs(observed.Samples[i]-expected[i])<=tolerance,
                $"Convolution sample {i}: expected {expected[i]:R} actual {observed.Samples[i]:R}");
        Assert.IsFalse(observed.ErrorBoundGuaranteed);
    }

    private static void DftRefused(FiniteDirectDftOutcome64 observed,FiniteSignalStatus64 status)
    {
        Assert.AreEqual(status,observed.Status);
        Assert.IsFalse(observed.Succeeded);
        Assert.IsNull(observed.Bins);
        Assert.IsFalse(observed.ErrorBoundGuaranteed);
    }

    private static void ConvolutionRefused(
        FiniteLinearConvolutionOutcome64 observed,FiniteSignalStatus64 status)
    {
        Assert.AreEqual(status,observed.Status);
        Assert.IsFalse(observed.Succeeded);
        Assert.IsNull(observed.Samples);
        Assert.IsFalse(observed.ErrorBoundGuaranteed);
    }

    [TestMethod] public void ForwardImpulseFour()
    { Near(FiniteSignal64.DirectDft([Complex.One,Complex.Zero,Complex.Zero,Complex.Zero]),[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void ForwardImpulseThree()
    { Near(FiniteSignal64.DirectDft([Complex.One,Complex.Zero,Complex.Zero]),[Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void ForwardImpulseOne()
    { Near(FiniteSignal64.DirectDft([new Complex(3d,0d)]),[new Complex(3d,0d)]); }

    [TestMethod] public void ForwardImpulseThirtyTwo()
    { var bins=new Complex[32];bins[0]=Complex.One;Near(FiniteSignal64.DirectDft(bins),Enumerable.Repeat(Complex.One,32).ToArray()); }

    [TestMethod] public void ForwardTwoSamples()
    { Near(FiniteSignal64.DirectDft([Complex.One,new Complex(2d,0d)]),[new Complex(3d,0d),new Complex(-1d,0d)]); }

    [TestMethod] public void ForwardShiftedImpulseFour()
    { Near(FiniteSignal64.DirectDft([Complex.Zero,Complex.One,Complex.Zero,Complex.Zero]),[Complex.One,-Complex.ImaginaryOne,-Complex.One,Complex.ImaginaryOne]); }

    [TestMethod] public void ForwardOnesFour()
    { Near(FiniteSignal64.DirectDft([Complex.One,Complex.One,Complex.One,Complex.One]),[new Complex(4d,0d),Complex.Zero,Complex.Zero,Complex.Zero]); }

    [TestMethod] public void ForwardZeroFour()
    { Near(FiniteSignal64.DirectDft([Complex.Zero,Complex.Zero,Complex.Zero,Complex.Zero]),[Complex.Zero,Complex.Zero,Complex.Zero,Complex.Zero]); }

    [TestMethod] public void ForwardAlternatingFour()
    { Near(FiniteSignal64.DirectDft([Complex.One,-Complex.One,Complex.One,-Complex.One]),[Complex.Zero,Complex.Zero,new Complex(4d,0d),Complex.Zero]); }

    [TestMethod] public void ForwardRampFourSign()
    { Near(FiniteSignal64.DirectDft([new Complex(1d,0d),new Complex(2d,0d),new Complex(3d,0d),new Complex(4d,0d)]),[new Complex(10d,0d),new Complex(-2d,2d),new Complex(-2d,0d),new Complex(-2d,-2d)]); }

    [TestMethod] public void ForwardImaginaryImpulse()
    { Near(FiniteSignal64.DirectDft([Complex.ImaginaryOne,Complex.Zero]),[Complex.ImaginaryOne,Complex.ImaginaryOne]); }

    [TestMethod] public void ForwardMixedComplexImpulse()
    { Near(FiniteSignal64.DirectDft([new Complex(1d,1d),Complex.Zero]),[new Complex(1d,1d),new Complex(1d,1d)]); }

    [TestMethod] public void InverseUnitSpectrum()
    { Near(FiniteSignal64.DirectDft([Complex.One,Complex.One,Complex.One,Complex.One],inverse:true),[Complex.One,Complex.Zero,Complex.Zero,Complex.Zero]); }

    [TestMethod] public void InverseDcSpectrum()
    { Near(FiniteSignal64.DirectDft([new Complex(4d,0d),Complex.Zero,Complex.Zero,Complex.Zero],inverse:true),[Complex.One,Complex.One,Complex.One,Complex.One]); }

    [TestMethod] public void InverseTwoPoints()
    { Near(FiniteSignal64.DirectDft([new Complex(2d,0d),Complex.Zero],inverse:true),[Complex.One,Complex.One]); }

    [TestMethod] public void InverseZeroFour()
    { Near(FiniteSignal64.DirectDft([Complex.Zero,Complex.Zero,Complex.Zero,Complex.Zero],inverse:true),[Complex.Zero,Complex.Zero,Complex.Zero,Complex.Zero]); }

    [TestMethod] public void InverseImaginarySpectrum()
    { Near(FiniteSignal64.DirectDft([new Complex(0d,2d),Complex.Zero],inverse:true),[Complex.ImaginaryOne,Complex.ImaginaryOne]); }

    [TestMethod] public void DirectInverseComplexRoundTrip()
    { var inputs=new[]{new Complex(1d,1d),new Complex(2d,-1d),new Complex(3d,2d),new Complex(4d,-2d)};var transform=FiniteSignal64.DirectDft(inputs);Assert.IsTrue(transform.Succeeded);Near(FiniteSignal64.DirectDft(transform.Bins,inverse:true),inputs); }

    [TestMethod] public void DftForwardSignMetadata()
    { var result=FiniteSignal64.DirectDft([Complex.One,Complex.Zero]);Assert.IsTrue(result.Succeeded);Assert.IsFalse(result.Inverse); }

    [TestMethod] public void DftInverseSignMetadata()
    { var result=FiniteSignal64.DirectDft([Complex.One,Complex.Zero],inverse:true);Assert.IsTrue(result.Succeeded);Assert.IsTrue(result.Inverse); }

    [TestMethod] public void DftRuleFromRuntime()
    { var result=FiniteSignal64.DirectDft([Complex.One]);Assert.IsTrue(result.Succeeded);Assert.AreEqual(FiniteSignal64.RuleId,result.RuleId); }

    [TestMethod] public void DftApproximationNotTheorem()
    { var result=FiniteSignal64.DirectDft([Complex.One,Complex.Zero]);Assert.IsTrue(result.Succeeded);Assert.IsFalse(result.ErrorBoundGuaranteed); }

    [TestMethod] public void DftAdmitsLimitComponent()
    { Near(FiniteSignal64.DirectDft([new Complex(1e6d,0d)]),[new Complex(1e6d,0d)]); }

    [TestMethod] public void DftNullRefused()
    { DftRefused(FiniteSignal64.DirectDft(null),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void DftEmptyRefused()
    { DftRefused(FiniteSignal64.DirectDft([]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void DftOverMaxLengthRefused()
    { DftRefused(FiniteSignal64.DirectDft(new Complex[33]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void DftNaNRealRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(double.NaN,0d)]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void DftNaNImaginaryRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(0d,double.NaN)]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void DftInfinityRealRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(double.PositiveInfinity,0d)]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void DftInfinityImaginaryRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(0d,double.NegativeInfinity)]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void DftRealBudgetRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(1e7d,0d)]),FiniteSignalStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DftImagBudgetRefused()
    { DftRefused(FiniteSignal64.DirectDft([new Complex(0d,-1e7d)]),FiniteSignalStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DftPreCancelledRefused()
    { using var token=new CancellationTokenSource();token.Cancel();DftRefused(FiniteSignal64.DirectDft([Complex.One],cancellationToken:token.Token),FiniteSignalStatus64.Cancelled); }

    [TestMethod] public void DftPreCancelledPriority()
    { using var token=new CancellationTokenSource();token.Cancel();DftRefused(FiniteSignal64.DirectDft(null,cancellationToken:token.Token),FiniteSignalStatus64.Cancelled); }

    [TestMethod] public void ConvolutionOneTwoByOneOne()
    { Vector(FiniteSignal64.LinearConvolution([1d,2d],[1d,1d]),[1d,3d,2d]); }

    [TestMethod] public void ConvolutionReversedCommutative()
    { Vector(FiniteSignal64.LinearConvolution([1d,1d],[1d,2d]),[1d,3d,2d]); }

    [TestMethod] public void ConvolutionSingleValues()
    { Vector(FiniteSignal64.LinearConvolution([2d],[3d]),[6d]); }

    [TestMethod] public void ConvolutionZeros()
    { Vector(FiniteSignal64.LinearConvolution([0d,0d],[2d,3d]),[0d,0d,0d]); }

    [TestMethod] public void ConvolutionImpulseIdentity()
    { Vector(FiniteSignal64.LinearConvolution([1d],[2d,3d,4d]),[2d,3d,4d]); }

    [TestMethod] public void ConvolutionShiftedImpulse()
    { Vector(FiniteSignal64.LinearConvolution([0d,1d,0d],[2d,3d]),[0d,2d,3d,0d]); }

    [TestMethod] public void ConvolutionNegativeValues()
    { Vector(FiniteSignal64.LinearConvolution([-1d,2d],[1d,-1d]),[-1d,3d,-2d]); }

    [TestMethod] public void ConvolutionThreeByThree()
    { Vector(FiniteSignal64.LinearConvolution([1d,2d,3d],[1d,1d,1d]),[1d,3d,6d,5d,3d]); }

    [TestMethod] public void ConvolutionNonCircularLength()
    { var outcome=FiniteSignal64.LinearConvolution([1d,2d],[3d,4d]);Vector(outcome,[3d,10d,8d]);Assert.AreEqual(3,outcome.Samples!.Length); }

    [TestMethod] public void ConvolutionHalfValues()
    { Vector(FiniteSignal64.LinearConvolution([0.5d,1d],[2d,2d]),[1d,3d,2d]); }

    [TestMethod] public void ConvolutionFullThirtyTwoInputs()
    { var a=Enumerable.Repeat(1d,32).ToArray();var b=Enumerable.Repeat(1d,32).ToArray();var outcome=FiniteSignal64.LinearConvolution(a,b);Assert.IsTrue(outcome.Succeeded);Assert.IsNotNull(outcome.Samples);Assert.AreEqual(63,outcome.Samples.Length);Assert.AreEqual(1d,outcome.Samples[0]);Assert.AreEqual(32d,outcome.Samples[31]);Assert.AreEqual(1d,outcome.Samples[62]); }

    [TestMethod] public void ConvolutionLimitMagnitude()
    { Vector(FiniteSignal64.LinearConvolution([1e6d],[1e6d]),[1e12d],1d); }

    [TestMethod] public void ConvolutionRuleFromRuntime()
    { var outcome=FiniteSignal64.LinearConvolution([1d],[1d]);Assert.IsTrue(outcome.Succeeded);Assert.AreEqual(FiniteSignal64.RuleId,outcome.RuleId); }

    [TestMethod] public void ConvolutionApproximationNotTheorem()
    { var outcome=FiniteSignal64.LinearConvolution([1d],[1d]);Assert.IsTrue(outcome.Succeeded);Assert.IsFalse(outcome.ErrorBoundGuaranteed); }

    [TestMethod] public void ConvolutionNullFirstRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution(null,[1d]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionNullSecondRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([1d],null),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionEmptyFirstRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([],[1d]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionEmptySecondRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([1d],[]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionOversizedFirstRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution(new double[33],[1d]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionOversizedSecondRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([1d],new double[33]),FiniteSignalStatus64.InvalidLength); }

    [TestMethod] public void ConvolutionNaNRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([double.NaN],[1d]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void ConvolutionInfinityRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([1d],[double.NegativeInfinity]),FiniteSignalStatus64.InvalidInput); }

    [TestMethod] public void ConvolutionBudgetRefused()
    { ConvolutionRefused(FiniteSignal64.LinearConvolution([1e7d],[1d]),FiniteSignalStatus64.OutsideQualifiedRange); }

    [TestMethod] public void ConvolutionPreCancelledRefused()
    { using var token=new CancellationTokenSource();token.Cancel();ConvolutionRefused(FiniteSignal64.LinearConvolution([1d],[1d],token.Token),FiniteSignalStatus64.Cancelled); }

    [TestMethod] public void ConvolutionPreCancelledPriority()
    { using var token=new CancellationTokenSource();token.Cancel();ConvolutionRefused(FiniteSignal64.LinearConvolution(null,null,token.Token),FiniteSignalStatus64.Cancelled); }
}
