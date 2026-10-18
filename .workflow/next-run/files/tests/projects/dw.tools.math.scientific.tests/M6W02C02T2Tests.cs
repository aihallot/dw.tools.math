using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C02T2Tests
{
    private static SpecialRealBoundaryResult64 R(SpecialFunctionKind64 kind,double argument,
        SpecialAngleInput64 unit=SpecialAngleInput64.NotApplicable,
        SpecialFunctionPolicy64? policy=null) =>
        SpecialFunctionBoundaryAudit64.EvaluateReal(kind,argument,unit,policy);

    private static SpecialComplexBoundaryResult64 C(SpecialFunctionKind64 kind,Complex argument,
        ComplexLogCutPolicy64 cut=ComplexLogCutPolicy64.RejectNegativeRealCut,
        SpecialFunctionPolicy64? policy=null) =>
        SpecialFunctionBoundaryAudit64.EvaluateComplex(kind,argument,cut,policy);

    private static void Near(SpecialRealBoundaryResult64 result,double expected,double tolerance=1e-10)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Value);
        Assert.IsTrue(double.IsFinite(result.Value.Value));
        Assert.IsTrue(global::System.Math.Abs(result.Value.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {result.Value.Value:R}");
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }
    private static void ComplexNear(SpecialComplexBoundaryResult64 result,Complex expected,double tolerance=1e-10)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Value);
        var v=result.Value.Value;
        Assert.IsTrue(double.IsFinite(v.Real)&&double.IsFinite(v.Imaginary));
        Assert.IsTrue(global::System.Math.Abs(v.Real-expected.Real)<=tolerance);
        Assert.IsTrue(global::System.Math.Abs(v.Imaginary-expected.Imaginary)<=tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }
    private static void RealRefused(SpecialRealBoundaryResult64 result,SpecialBoundaryStatus64 status)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Value);
        Assert.IsNull(result.EffectiveRadians);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }
    private static void ComplexRefused(SpecialComplexBoundaryResult64 result,SpecialBoundaryStatus64 status)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Value);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    [TestMethod] public void Sine90Degrees()
    { Near(R(SpecialFunctionKind64.Sine,90,SpecialAngleInput64.Degrees),1d,1e-10); }
    [TestMethod] public void Sine30Degrees()
    { Near(R(SpecialFunctionKind64.Sine,30,SpecialAngleInput64.Degrees),0.5d,1e-10); }
    [TestMethod] public void Sine180Degrees()
    { Near(R(SpecialFunctionKind64.Sine,180,SpecialAngleInput64.Degrees),0d,1e-10); }
    [TestMethod] public void Sine270Degrees()
    { Near(R(SpecialFunctionKind64.Sine,270,SpecialAngleInput64.Degrees),-1d,1e-10); }
    [TestMethod] public void Sine720Degrees()
    { Near(R(SpecialFunctionKind64.Sine,720,SpecialAngleInput64.Degrees),0d,1e-10); }
    [TestMethod] public void Cosine180Degrees()
    { Near(R(SpecialFunctionKind64.Cosine,180,SpecialAngleInput64.Degrees),-1d,1e-10); }
    [TestMethod] public void Cosine60Degrees()
    { Near(R(SpecialFunctionKind64.Cosine,60,SpecialAngleInput64.Degrees),0.5d,1e-10); }
    [TestMethod] public void Cosine0Degrees()
    { Near(R(SpecialFunctionKind64.Cosine,0,SpecialAngleInput64.Degrees),1d,1e-10); }
    [TestMethod] public void SinePiOver2Radians()
    { Near(R(SpecialFunctionKind64.Sine,global::System.Math.PI/2d,SpecialAngleInput64.Radians),1d,1e-10); }
    [TestMethod] public void CosinePiRadians()
    { Near(R(SpecialFunctionKind64.Cosine,global::System.Math.PI,SpecialAngleInput64.Radians),-1d,1e-10); }
    [TestMethod] public void SineOneRadian()
    { Near(R(SpecialFunctionKind64.Sine,1d,SpecialAngleInput64.Radians),global::System.Math.Sin(1d),1e-10); }
    [TestMethod] public void CosineOneRadian()
    { Near(R(SpecialFunctionKind64.Cosine,1d,SpecialAngleInput64.Radians),global::System.Math.Cos(1d),1e-10); }
    [TestMethod] public void ConvertedRadianObservation()
    { var r=R(SpecialFunctionKind64.Sine,90,SpecialAngleInput64.Degrees);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.EffectiveRadians);Assert.IsTrue(global::System.Math.Abs(r.EffectiveRadians.Value-global::System.Math.PI/2d)<1e-14); }
    [TestMethod] public void NativeRadianObservation()
    { var r=R(SpecialFunctionKind64.Sine,0.5d,SpecialAngleInput64.Radians);Assert.IsTrue(r.Succeeded);Assert.AreEqual(0.5d,r.EffectiveRadians); }
    [TestMethod] public void DegreeMetadataRecorded()
    { var r=R(SpecialFunctionKind64.Cosine,60d,SpecialAngleInput64.Degrees);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialAngleInput64.Degrees,r.InputAngleUnit); }
    [TestMethod] public void RadianMetadataRecorded()
    { var r=R(SpecialFunctionKind64.Sine,1d,SpecialAngleInput64.Radians);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialAngleInput64.Radians,r.InputAngleUnit); }
    [TestMethod] public void SineWithoutAngle()
    { RealRefused(R(SpecialFunctionKind64.Sine,1d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.AngleUnitRequired); }
    [TestMethod] public void CosineWithoutAngle()
    { RealRefused(R(SpecialFunctionKind64.Cosine,1d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.AngleUnitRequired); }
    [TestMethod] public void UnknownAngleUnit()
    { RealRefused(R(SpecialFunctionKind64.Sine,1d,(SpecialAngleInput64)99),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void ExpWithDegrees()
    { RealRefused(R(SpecialFunctionKind64.Exponential,1d,SpecialAngleInput64.Degrees),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void GammaWithRadians()
    { RealRefused(R(SpecialFunctionKind64.Gamma,5d,SpecialAngleInput64.Radians),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void LogWithRadians()
    { RealRefused(R(SpecialFunctionKind64.NaturalLogarithm,1d,SpecialAngleInput64.Radians),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void TooManyDegrees()
    { RealRefused(R(SpecialFunctionKind64.Sine,1200d,SpecialAngleInput64.Degrees),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void CallerRadianRangeRestrictsDegrees()
    { RealRefused(R(SpecialFunctionKind64.Sine,90d,SpecialAngleInput64.Degrees,new(1d,12d)),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void SubnormalDegreeUnderflow()
    { RealRefused(R(SpecialFunctionKind64.Sine,double.Epsilon,SpecialAngleInput64.Degrees),SpecialBoundaryStatus64.UnderflowRisk); }
    [TestMethod] public void AngleNaN()
    { RealRefused(R(SpecialFunctionKind64.Sine,double.NaN,SpecialAngleInput64.Degrees),SpecialBoundaryStatus64.InvalidInput); }
    [TestMethod] public void AngleInfinity()
    { RealRefused(R(SpecialFunctionKind64.Sine,double.PositiveInfinity,SpecialAngleInput64.Radians),SpecialBoundaryStatus64.InvalidInput); }
    [TestMethod] public void RealPolicyInvalid()
    { RealRefused(R(SpecialFunctionKind64.Sine,1d,SpecialAngleInput64.Radians,new(0d,12d)),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void SmallDegreesWithinCallerLimit()
    { Near(R(SpecialFunctionKind64.Sine,30d,SpecialAngleInput64.Degrees,new(1d,12d)),0.5d,1e-10); }
    [TestMethod] public void InvalidRealOperation()
    { RealRefused(R((SpecialFunctionKind64)99,1d),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void RealCancellationPreemptive()
    { using var c=new CancellationTokenSource();c.Cancel();RealRefused(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Sine,90d,SpecialAngleInput64.Degrees,cancellationToken:c.Token),SpecialBoundaryStatus64.Cancelled); }
    [TestMethod] public void RealProviderIdFromRuntime()
    { var r=R(SpecialFunctionKind64.Sine,90d,SpecialAngleInput64.Degrees);Assert.IsTrue(r.Succeeded);Assert.AreEqual(ManagedBclSpecialFunctionsProvider64.ProviderIdentity,r.ProviderId); }
    [TestMethod] public void RealBoundaryRuleFromRuntime()
    { var r=R(SpecialFunctionKind64.Cosine,60d,SpecialAngleInput64.Degrees);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialFunctionBoundaryAudit64.RuleId,r.RuleId); }
    [TestMethod] public void GammaZeroPole()
    { RealRefused(R(SpecialFunctionKind64.Gamma,0d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.GammaPole); }
    [TestMethod] public void GammaNegativeOnePole()
    { RealRefused(R(SpecialFunctionKind64.Gamma,-1d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.GammaPole); }
    [TestMethod] public void GammaNegativeTwoPole()
    { RealRefused(R(SpecialFunctionKind64.Gamma,-2d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.GammaPole); }
    [TestMethod] public void GammaNegativeHalfOutsideDomain()
    { RealRefused(R(SpecialFunctionKind64.Gamma,-0.5d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.UnsupportedDomain); }
    [TestMethod] public void GammaPositiveBelowHalfOutsideDomain()
    { RealRefused(R(SpecialFunctionKind64.Gamma,0.49d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.UnsupportedDomain); }
    [TestMethod] public void GammaAboveTwentyOutsideRange()
    { RealRefused(R(SpecialFunctionKind64.Gamma,21d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void GammaOne()
    { Near(R(SpecialFunctionKind64.Gamma,1d,SpecialAngleInput64.NotApplicable),1d,1e-12); }
    [TestMethod] public void GammaAtHalf()
    { Near(R(SpecialFunctionKind64.Gamma,0.5d,SpecialAngleInput64.NotApplicable),global::System.Math.Sqrt(global::System.Math.PI),1e-11); }
    [TestMethod] public void GammaFive()
    { Near(R(SpecialFunctionKind64.Gamma,5d,SpecialAngleInput64.NotApplicable),24d,1e-10); }
    [TestMethod] public void GammaSix()
    { Near(R(SpecialFunctionKind64.Gamma,6d,SpecialAngleInput64.NotApplicable),120d,1e-9); }
    [TestMethod] public void GammaResultUncertified()
    { var r=R(SpecialFunctionKind64.Gamma,5d,SpecialAngleInput64.NotApplicable);Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ErrorBoundGuaranteed);Assert.IsNull(r.EffectiveRadians); }
    [TestMethod] public void ExponentialZero()
    { Near(R(SpecialFunctionKind64.Exponential,0d,SpecialAngleInput64.NotApplicable),1d,1e-10); }
    [TestMethod] public void ExponentialTwentyFinite()
    { var r=R(SpecialFunctionKind64.Exponential,20d,SpecialAngleInput64.NotApplicable);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.IsTrue(r.Value.Value>1e8d); }
    [TestMethod] public void ExpOutsideLimit()
    { RealRefused(R(SpecialFunctionKind64.Exponential,21d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void ExponentialOverflowPreemptive()
    { RealRefused(R(SpecialFunctionKind64.Exponential,900d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.OverflowRisk); }
    [TestMethod] public void ExponentialUnderflowPreemptive()
    { RealRefused(R(SpecialFunctionKind64.Exponential,-900d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.UnderflowRisk); }
    [TestMethod] public void LogZero()
    { RealRefused(R(SpecialFunctionKind64.NaturalLogarithm,0d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.UnsupportedDomain); }
    [TestMethod] public void LogNegative()
    { RealRefused(R(SpecialFunctionKind64.NaturalLogarithm,-1d,SpecialAngleInput64.NotApplicable),SpecialBoundaryStatus64.UnsupportedDomain); }
    [TestMethod] public void LogOne()
    { Near(R(SpecialFunctionKind64.NaturalLogarithm,1d,SpecialAngleInput64.NotApplicable),0d,1e-10); }
    [TestMethod] public void LogEuler()
    { Near(R(SpecialFunctionKind64.NaturalLogarithm,global::System.Math.E,SpecialAngleInput64.NotApplicable),1d,1e-10); }
    [TestMethod] public void LogNegativeAxisDefault()
    { ComplexRefused(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.BranchCutRequiresOptIn); }
    [TestMethod] public void LogNegativeTwoDefault()
    { ComplexRefused(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-2d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.BranchCutRequiresOptIn); }
    [TestMethod] public void LogZeroComplex()
    { ComplexRefused(C(SpecialFunctionKind64.NaturalLogarithm,Complex.Zero,ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.UnsupportedDomain); }
    [TestMethod] public void ComplexGammaOne()
    { ComplexRefused(C(SpecialFunctionKind64.Gamma,Complex.One,ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.ComplexGammaUnsupported); }
    [TestMethod] public void ComplexGammaHalf()
    { ComplexRefused(C(SpecialFunctionKind64.Gamma,new Complex(0.5d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.ComplexGammaUnsupported); }
    [TestMethod] public void ComplexSinOutsideRange()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(13d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void ComplexCallerRange()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(2d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut,new(20d,1d)),SpecialBoundaryStatus64.OutsideQualifiedRange); }
    [TestMethod] public void ComplexOverflowPreemptive()
    { ComplexRefused(C(SpecialFunctionKind64.Exponential,new Complex(900d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.OverflowRisk); }
    [TestMethod] public void ComplexUnderflowPreemptive()
    { ComplexRefused(C(SpecialFunctionKind64.Exponential,new Complex(-900d,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.UnderflowRisk); }
    [TestMethod] public void ComplexNaNReal()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(double.NaN,0d),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.InvalidInput); }
    [TestMethod] public void ComplexInfinityImaginary()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,new Complex(0d,double.PositiveInfinity),ComplexLogCutPolicy64.RejectNegativeRealCut),SpecialBoundaryStatus64.InvalidInput); }
    [TestMethod] public void ComplexInvalidPolicy()
    { ComplexRefused(C(SpecialFunctionKind64.Cosine,Complex.One,ComplexLogCutPolicy64.RejectNegativeRealCut,new(20d,0d)),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void ComplexBadBranchEnum()
    { ComplexRefused(C(SpecialFunctionKind64.NaturalLogarithm,Complex.One,(ComplexLogCutPolicy64)99),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void ComplexNonlogCutApprovalInvalid()
    { ComplexRefused(C(SpecialFunctionKind64.Sine,Complex.One,ComplexLogCutPolicy64.CallerAcceptsPrincipalCut),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void ComplexUnknownFunctionKind()
    { ComplexRefused(C((SpecialFunctionKind64)99,Complex.One),SpecialBoundaryStatus64.InvalidPolicy); }
    [TestMethod] public void LogNegativeOneWithApproval()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),ComplexLogCutPolicy64.CallerAcceptsPrincipalCut),new Complex(0d,global::System.Math.PI)); }
    [TestMethod] public void LogNegativeTwoWithApproval()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-2d,0d),ComplexLogCutPolicy64.CallerAcceptsPrincipalCut),new Complex(global::System.Math.Log(2d),global::System.Math.PI)); }
    [TestMethod] public void LogUpperOffCut()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,1d),ComplexLogCutPolicy64.RejectNegativeRealCut),new Complex(global::System.Math.Log(2d)/2d,3d*global::System.Math.PI/4d)); }
    [TestMethod] public void LogLowerOffCut()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,-1d),ComplexLogCutPolicy64.RejectNegativeRealCut),new Complex(global::System.Math.Log(2d)/2d,-3d*global::System.Math.PI/4d)); }
    [TestMethod] public void LogOneComplex()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,Complex.One,ComplexLogCutPolicy64.RejectNegativeRealCut),Complex.Zero); }
    [TestMethod] public void LogImaginaryUnit()
    { ComplexNear(C(SpecialFunctionKind64.NaturalLogarithm,Complex.ImaginaryOne,ComplexLogCutPolicy64.RejectNegativeRealCut),new Complex(0d,global::System.Math.PI/2d)); }
    [TestMethod] public void ComplexSineZero()
    { ComplexNear(C(SpecialFunctionKind64.Sine,Complex.Zero,ComplexLogCutPolicy64.RejectNegativeRealCut),Complex.Zero); }
    [TestMethod] public void ComplexCosineZero()
    { ComplexNear(C(SpecialFunctionKind64.Cosine,Complex.Zero,ComplexLogCutPolicy64.RejectNegativeRealCut),Complex.One); }
    [TestMethod] public void ComplexExponentialZero()
    { ComplexNear(C(SpecialFunctionKind64.Exponential,Complex.Zero,ComplexLogCutPolicy64.RejectNegativeRealCut),Complex.One); }
    [TestMethod] public void ComplexExpImaginaryPi()
    { ComplexNear(C(SpecialFunctionKind64.Exponential,new Complex(0d,global::System.Math.PI),ComplexLogCutPolicy64.RejectNegativeRealCut),new Complex(-1d,0d)); }
    [TestMethod] public void ComplexCancellationPreemptive()
    { using var c=new CancellationTokenSource();c.Cancel();ComplexRefused(SpecialFunctionBoundaryAudit64.EvaluateComplex(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),cancellationToken:c.Token),SpecialBoundaryStatus64.Cancelled); }
    [TestMethod] public void ComplexSelectedBranchRecorded()
    { var r=C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),ComplexLogCutPolicy64.CallerAcceptsPrincipalCut);Assert.IsTrue(r.Succeeded);Assert.AreEqual(ComplexLogCutPolicy64.CallerAcceptsPrincipalCut,r.CutPolicy); }
    [TestMethod] public void ComplexBoundaryMetadataObserved()
    { var r=C(SpecialFunctionKind64.Cosine,Complex.One,ComplexLogCutPolicy64.RejectNegativeRealCut);Assert.IsTrue(r.Succeeded);Assert.AreEqual(SpecialFunctionBoundaryAudit64.RuleId,r.RuleId);Assert.AreEqual(ManagedBclSpecialFunctionsProvider64.ProviderIdentity,r.ProviderId);Assert.IsFalse(r.ErrorBoundGuaranteed); }
    [TestMethod] public void PreviouslyQualifiedT1RadiansUnchanged()
    { var r=BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Sine,global::System.Math.PI/2d);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.IsTrue(global::System.Math.Abs(r.Value.Value-1d)<1e-12); }
    [TestMethod] public void PreviouslyQualifiedT1NegativeComplexLogUnchanged()
    { var r=BoundedSpecialFunctions64.EvaluateComplex(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d));Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.IsTrue(global::System.Math.Abs(r.Value.Value.Imaginary-global::System.Math.PI)<1e-12); }
    [TestMethod] public void PreviouslyQualifiedExactBinomialUnchanged()
    { var r=BoundedExactScientific.Binomial(5,2);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.AreEqual(new BigInteger(10),r.Value.Value); }
    [TestMethod] public void NoUnqualifiedForeignProviderDependencies()
    { var refs=typeof(SpecialFunctionBoundaryAudit64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(refs.Any(x=>new[]{"MathNet","Meta.Numerics","Aura","Decision","MCDM","Angouri"}.Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase)))); }
    [TestMethod] public void DeterministicRealBoundaryRecord()
    { Assert.AreEqual(R(SpecialFunctionKind64.Sine,90d,SpecialAngleInput64.Degrees),R(SpecialFunctionKind64.Sine,90d,SpecialAngleInput64.Degrees)); }
    [TestMethod] public void DeterministicComplexBoundaryRecord()
    { Assert.AreEqual(C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),ComplexLogCutPolicy64.CallerAcceptsPrincipalCut),C(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1d,0d),ComplexLogCutPolicy64.CallerAcceptsPrincipalCut)); }
}
