using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C02T1Tests
{
    private static SpecialRealResult64 R(
        SpecialFunctionKind64 kind,double argument,SpecialFunctionPolicy64? p=null) =>
        BoundedSpecialFunctions64.EvaluateReal(kind,argument,p);
    private static SpecialComplexResult64 C(
        SpecialFunctionKind64 kind,Complex argument,SpecialFunctionPolicy64? p=null) =>
        BoundedSpecialFunctions64.EvaluateComplex(kind,argument,p);
    private static void Near(SpecialRealResult64 r,double expected,double tolerance=1e-10)
    {
        Assert.IsTrue(r.Succeeded);
        Assert.IsNotNull(r.Value);
        Assert.IsTrue(double.IsFinite(r.Value.Value));
        Assert.IsTrue(global::System.Math.Abs(r.Value.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {r.Value.Value:R}");
    }
    private static void ComplexNear(SpecialComplexResult64 r,Complex expected,double tolerance=1e-10)
    {
        Assert.IsTrue(r.Succeeded);
        Assert.IsNotNull(r.Value);
        var observed=r.Value.Value;
        Assert.IsTrue(global::System.Math.Abs(observed.Real-expected.Real)<=tolerance,
            $"Real: expected {expected.Real:R}, observed {observed.Real:R}");
        Assert.IsTrue(global::System.Math.Abs(observed.Imaginary-expected.Imaginary)<=tolerance,
            $"Imaginary: expected {expected.Imaginary:R}, observed {observed.Imaginary:R}");
    }
    private static void RealRefused(SpecialRealResult64 r,SpecialFunctionStatus64 status)
    {
        Assert.IsFalse(r.Succeeded);
        Assert.AreEqual(status,r.Status);
        Assert.IsNull(r.Value);
        Assert.IsFalse(r.ErrorBoundGuaranteed);
    }
    private static void ComplexRefused(SpecialComplexResult64 r,SpecialFunctionStatus64 status)
    {
        Assert.IsFalse(r.Succeeded);
        Assert.AreEqual(status,r.Status);
        Assert.IsNull(r.Value);
        Assert.IsFalse(r.ErrorBoundGuaranteed);
    }

    [TestMethod] public void SineZeroIsExactlyZeroWithinObservedTolerance()
    { Near(R(SpecialFunctionKind64.Sine,0),0); }
    [TestMethod] public void SineHalfPiIsOne()
    { Near(R(SpecialFunctionKind64.Sine,global::System.Math.PI/2d),1); }
    [TestMethod] public void SineNegativeHalfPiIsMinusOne()
    { Near(R(SpecialFunctionKind64.Sine,-global::System.Math.PI/2d),-1); }
    [TestMethod] public void SinePiIsNumericallyZero()
    { Near(R(SpecialFunctionKind64.Sine,global::System.Math.PI),0); }
    [TestMethod] public void CosineZeroIsOne()
    { Near(R(SpecialFunctionKind64.Cosine,0),1); }
    [TestMethod] public void CosinePiIsMinusOne()
    { Near(R(SpecialFunctionKind64.Cosine,global::System.Math.PI),-1); }
    [TestMethod] public void CosineHalfPiIsNumericallyZero()
    { Near(R(SpecialFunctionKind64.Cosine,global::System.Math.PI/2d),0); }
    [TestMethod] public void SineIsOddForAdmittedArgument()
    { var a=R(SpecialFunctionKind64.Sine,0.6);var b=R(SpecialFunctionKind64.Sine,-0.6);Assert.IsTrue(a.Succeeded);Near(b,-a.Value!.Value); }
    [TestMethod] public void CosineIsEvenForAdmittedArgument()
    { var a=R(SpecialFunctionKind64.Cosine,0.6);var b=R(SpecialFunctionKind64.Cosine,-0.6);Assert.IsTrue(a.Succeeded);Near(b,a.Value!.Value); }
    [TestMethod] public void TrigonometricPythagoreanIdentityIsNumericallyAdmitted()
    { var a=R(SpecialFunctionKind64.Sine,0.8);var b=R(SpecialFunctionKind64.Cosine,0.8);Assert.IsTrue(a.Succeeded&&b.Succeeded);Assert.IsTrue(global::System.Math.Abs(a.Value!.Value*a.Value.Value+b.Value!.Value*b.Value.Value-1d)<=1e-12); }
    [TestMethod] public void ExponentialZeroIsOne()
    { Near(R(SpecialFunctionKind64.Exponential,0),1); }
    [TestMethod] public void ExponentialOneIsEulerNumber()
    { Near(R(SpecialFunctionKind64.Exponential,1),global::System.Math.E); }
    [TestMethod] public void ExponentialMinusOneIsInverseEuler()
    { Near(R(SpecialFunctionKind64.Exponential,-1),1d/global::System.Math.E); }
    [TestMethod] public void ExponentialNaturalLogTwoIsTwo()
    { Near(R(SpecialFunctionKind64.Exponential,global::System.Math.Log(2d)),2); }
    [TestMethod] public void NaturalLogarithmOneIsZero()
    { Near(R(SpecialFunctionKind64.NaturalLogarithm,1),0); }
    [TestMethod] public void NaturalLogarithmEulerIsOne()
    { Near(R(SpecialFunctionKind64.NaturalLogarithm,global::System.Math.E),1); }
    [TestMethod] public void NaturalLogarithmTwoUsesBclReference()
    { Near(R(SpecialFunctionKind64.NaturalLogarithm,2),global::System.Math.Log(2)); }
    [TestMethod] public void NaturalLogarithmZeroIsOutsideRealDomain()
    { RealRefused(R(SpecialFunctionKind64.NaturalLogarithm,0),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void NaturalLogarithmNegativeInputRefuses()
    { RealRefused(R(SpecialFunctionKind64.NaturalLogarithm,-1),SpecialFunctionStatus64.UnsupportedDomain); }

    [TestMethod] public void GammaOneIsOne()
    { Near(R(SpecialFunctionKind64.Gamma,1),1,1e-12); }
    [TestMethod] public void GammaTwoIsOne()
    { Near(R(SpecialFunctionKind64.Gamma,2),1,1e-12); }
    [TestMethod] public void GammaThreeIsTwo()
    { Near(R(SpecialFunctionKind64.Gamma,3),2,1e-11); }
    [TestMethod] public void GammaFiveIsTwentyFour()
    { Near(R(SpecialFunctionKind64.Gamma,5),24,1e-10); }
    [TestMethod] public void GammaSixIsOneHundredTwenty()
    { Near(R(SpecialFunctionKind64.Gamma,6),120,1e-9); }
    [TestMethod] public void GammaHalfIsSquareRootPi()
    { Near(R(SpecialFunctionKind64.Gamma,0.5d),global::System.Math.Sqrt(global::System.Math.PI),1e-11); }
    [TestMethod] public void GammaThreeHalvesIsHalfRootPi()
    { Near(R(SpecialFunctionKind64.Gamma,1.5d),global::System.Math.Sqrt(global::System.Math.PI)/2d,1e-11); }
    [TestMethod] public void GammaFiveHalvesIsThreeQuarterRootPi()
    { Near(R(SpecialFunctionKind64.Gamma,2.5d),3d*global::System.Math.Sqrt(global::System.Math.PI)/4d,1e-11); }
    [TestMethod] public void GammaPositiveIntegerRecurrenceHoldsNumerically()
    { var a=R(SpecialFunctionKind64.Gamma,5);var b=R(SpecialFunctionKind64.Gamma,6);Assert.IsTrue(a.Succeeded);Near(b,5d*a.Value!.Value,1e-10); }
    [TestMethod] public void GammaAtTwentyMatchesIndependentFactorialReference()
    { var r=R(SpecialFunctionKind64.Gamma,20);Assert.IsTrue(r.Succeeded);var oracle=121645100408832000d;Assert.IsTrue(global::System.Math.Abs(r.Value!.Value-oracle)/oracle<1e-11); }
    [TestMethod] public void GammaBelowHalfOutsideQualifiedSubsetRefuses()
    { RealRefused(R(SpecialFunctionKind64.Gamma,0.49),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void GammaZeroPoleIsNotAdmitted()
    { RealRefused(R(SpecialFunctionKind64.Gamma,0),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void GammaNegativeIntegerIsNotAdmitted()
    { RealRefused(R(SpecialFunctionKind64.Gamma,-1),SpecialFunctionStatus64.UnsupportedDomain); }

    [TestMethod] public void ExponentialAtRealBoundIsFinite()
    { var r=R(SpecialFunctionKind64.Exponential,20);Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.Value>1e8); }
    [TestMethod] public void ExponentialAboveRealHardBoundRefuses()
    { RealRefused(R(SpecialFunctionKind64.Exponential,20.01),SpecialFunctionStatus64.OutsideQualifiedRange); }
    [TestMethod] public void SineAboveRealHardBoundRefuses()
    { RealRefused(R(SpecialFunctionKind64.Sine,21),SpecialFunctionStatus64.OutsideQualifiedRange); }
    [TestMethod] public void RealNanIsInvalidInput()
    { RealRefused(R(SpecialFunctionKind64.Sine,double.NaN),SpecialFunctionStatus64.InvalidInput); }
    [TestMethod] public void RealInfinityIsInvalidInput()
    { RealRefused(R(SpecialFunctionKind64.Sine,double.PositiveInfinity),SpecialFunctionStatus64.InvalidInput); }
    [TestMethod] public void RealUnknownOperationIsInvalidPolicy()
    { RealRefused(R((SpecialFunctionKind64)99,1),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void ZeroRealPolicyLimitRefuses()
    { RealRefused(R(SpecialFunctionKind64.Sine,0.5,new(0,12)),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void AboveHardRealPolicyLimitRefuses()
    { RealRefused(R(SpecialFunctionKind64.Sine,0.5,new(21,12)),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void NonfiniteComplexPolicyLimitRefusesRealCall()
    { RealRefused(R(SpecialFunctionKind64.Sine,0.5,new(20,double.NaN)),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void CallerSmallerRealRangeRefuses()
    { RealRefused(R(SpecialFunctionKind64.Sine,2,new(1,12)),SpecialFunctionStatus64.OutsideQualifiedRange); }
    [TestMethod] public void CallerSmallerRealRangeAdmitsWithinBound()
    { Near(R(SpecialFunctionKind64.Sine,0.5,new(1,12)),global::System.Math.Sin(0.5)); }
    [TestMethod] public void RealPreCancellationReturnsNonpartialRefusal()
    { using var c=new CancellationTokenSource();c.Cancel();RealRefused(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Sine,1,cancellationToken:c.Token),SpecialFunctionStatus64.Cancelled); }
    [TestMethod] public void RealResultNamesManagedRuntimeProvider()
    { var r=R(SpecialFunctionKind64.Sine,0.5);Assert.IsTrue(r.Succeeded);Assert.AreEqual(ManagedBclSpecialFunctionsProvider64.ProviderIdentity,r.ProviderId); }
    [TestMethod] public void RealResultCarriesVersionedRuntimeRule()
    { var r=R(SpecialFunctionKind64.Sine,0.5);Assert.IsTrue(r.Succeeded);Assert.AreEqual(BoundedSpecialFunctions64.RuleId,r.RuleId); }
    [TestMethod] public void TrigonometryUsesRadiansAtRuntime()
    { var r=R(SpecialFunctionKind64.Sine,global::System.Math.PI/2);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialAngleConvention64.Radians,r.AngleConvention); }
    [TestMethod] public void NoRealApproximationClaimsProvenErrorBound()
    { var r=R(SpecialFunctionKind64.Gamma,2.5);Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ErrorBoundGuaranteed); }

    [TestMethod] public void ComplexExponentialZeroIsOne()
    { ComplexNear(C(SpecialFunctionKind64.Exponential,Complex.Zero),Complex.One); }
    [TestMethod] public void ComplexExponentialImaginaryPiIsMinusOne()
    { ComplexNear(C(SpecialFunctionKind64.Exponential,new Complex(0,global::System.Math.PI)),new Complex(-1,0)); }
    [TestMethod] public void ComplexExponentialRealOneIsEulerNumber()
    { ComplexNear(C(SpecialFunctionKind64.Exponential,new Complex(1,0)),new Complex(global::System.Math.E,0)); }
    [TestMethod] public void ComplexSineZeroIsZero()
    { ComplexNear(C(SpecialFunctionKind64.Sine,Complex.Zero),Complex.Zero); }
    [TestMethod] public void ComplexSineImaginaryUnitIsImaginarySinhOne()
    { ComplexNear(C(SpecialFunctionKind64.Sine,Complex.ImaginaryOne),new Complex(0,global::System.Math.Sinh(1))); }
    [TestMethod] public void ComplexCosineZeroIsOne()
    { ComplexNear(C(SpecialFunctionKind64.Cosine,Complex.Zero),Complex.One); }
    [TestMethod] public void ComplexCosineImaginaryUnitIsCoshOne()
    { ComplexNear(C(SpecialFunctionKind64.Cosine,Complex.ImaginaryOne),new Complex(global::System.Math.Cosh(1),0)); }
    [TestMethod] public void ComplexSineRealHalfPiIsOne()
    { ComplexNear(C(SpecialFunctionKind64.Sine,new Complex(global::System.Math.PI/2,0)),Complex.One); }
    [TestMethod] public void ComplexLogarithmOneIsZero()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,Complex.One),Complex.Zero); }
    [TestMethod] public void ComplexLogarithmMinusOnePrincipalBranchIsPositivePi()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1,0)),new Complex(0,global::System.Math.PI)); }
    [TestMethod] public void ComplexLogarithmImaginaryUnitHasHalfPiArgument()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,Complex.ImaginaryOne),new Complex(0,global::System.Math.PI/2)); }
    [TestMethod] public void ComplexLogarithmOnePlusIHasPrincipalPhase()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(1,1)),new Complex(global::System.Math.Log(2)/2,global::System.Math.PI/4)); }
    [TestMethod] public void ComplexExponentialOfComplexLogarithmRecoversAdmittedPoint()
    { var z=new Complex(1,1);var logged=C(SpecialFunctionKind64.NaturalLogarithm,z);Assert.IsTrue(logged.Succeeded);ComplexNear(C(SpecialFunctionKind64.Exponential,logged.Value!.Value),z); }
    [TestMethod] public void ComplexLogarithmZeroIsNotAdmitted()
    { ComplexRefused(C(SpecialFunctionKind64.NaturalLogarithm,Complex.Zero),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void ComplexGammaAtOneIsExplicitlyDeferred()
    { ComplexRefused(C(SpecialFunctionKind64.Gamma,Complex.One),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void ComplexGammaHalfIsNotSilentlyComputed()
    { ComplexRefused(C(SpecialFunctionKind64.Gamma,new Complex(0.5,0)),SpecialFunctionStatus64.UnsupportedDomain); }
    [TestMethod] public void ComplexNanRealComponentIsInvalid()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(double.NaN,1)),SpecialFunctionStatus64.InvalidInput); }
    [TestMethod] public void ComplexInfinityImaginaryComponentIsInvalid()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(1,double.PositiveInfinity)),SpecialFunctionStatus64.InvalidInput); }
    [TestMethod] public void ComplexUnknownOperationIsInvalidPolicy()
    { ComplexRefused(C((SpecialFunctionKind64)99,Complex.One),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void ComplexHardComponentLimitRefuses()
    { ComplexRefused(C(SpecialFunctionKind64.Cosine,new Complex(12.1,0)),SpecialFunctionStatus64.OutsideQualifiedRange); }
    [TestMethod] public void ComplexCallerComponentLimitRefuses()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(2,0),new(20,1)),SpecialFunctionStatus64.OutsideQualifiedRange); }
    [TestMethod] public void ComplexInvalidZeroLimitIsInvalidPolicy()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,Complex.One,new(20,0)),SpecialFunctionStatus64.InvalidPolicy); }
    [TestMethod] public void ComplexPreCancellationRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();ComplexRefused(BoundedSpecialFunctions64.EvaluateComplex(SpecialFunctionKind64.Sine,Complex.One,cancellationToken:c.Token),SpecialFunctionStatus64.Cancelled); }
    [TestMethod] public void ComplexResultCarriesPrincipalRadianConvention()
    { var r=C(SpecialFunctionKind64.NaturalLogarithm,Complex.ImaginaryOne);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialAngleConvention64.Radians,r.AngleConvention); }
    [TestMethod] public void ComplexResultCannotPromiseCertifiedError()
    { var r=C(SpecialFunctionKind64.Sine,new Complex(1,1));Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ErrorBoundGuaranteed); }
    [TestMethod] public void ComplexResultUsesExplicitManagedProviderAtRuntime()
    { var r=C(SpecialFunctionKind64.Sine,new Complex(1,1));Assert.IsTrue(r.Succeeded);Assert.AreEqual(ManagedBclSpecialFunctionsProvider64.ProviderIdentity,r.ProviderId); }

    [TestMethod] public void ExplicitManagedProviderCanBeCalledThroughInterface()
    { ISpecialFunctionsProvider64 provider=new ManagedBclSpecialFunctionsProvider64();Near(provider.EvaluateReal(SpecialFunctionKind64.Gamma,5),24,1e-10); }
    [TestMethod] public void PriorExactIntegerFactorialRemainsQualified()
    { var r=BoundedExactScientific.Factorial(5);Assert.IsTrue(r.Succeeded);Assert.AreEqual(new BigInteger(120),r.Value!.Value); }
    [TestMethod] public void PriorSignedMagnitudeBoundaryRemainsQualified()
    { var r=ExactScientificBoundaryAudit.GreatestCommonDivisor(-48,18,SignedOperandPolicy.NormalizeMagnitude);Assert.IsTrue(r.Succeeded);Assert.AreEqual(new BigInteger(6),r.Value!.Value); }
    [TestMethod] public void ManagedBackendHasNoForeignProviderOrAgentAssemblies()
    { var references=typeof(BoundedSpecialFunctions64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(references.Any(x=>new[]{"MathNet","Meta.Numerics","Aura","Decision","MCDM","Angouri"}.Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase)))); }
    [TestMethod] public void RepeatedManagedObservationsAreDeterministic()
    { Assert.AreEqual(R(SpecialFunctionKind64.Gamma,5),R(SpecialFunctionKind64.Gamma,5));Assert.AreEqual(C(SpecialFunctionKind64.Sine,new Complex(1,1)),C(SpecialFunctionKind64.Sine,new Complex(1,1))); }
}
