using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C03T1Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrExactScalar N(long n)=>new(new ExactRational(n,1));
    private static IrExactScalar Q(long n,long d)=>new(new ExactRational(n,d));
    private static ExactRational R(long n,long d=1)=>new(n,d);
    private static IrNode Sq()=>IrApply.Create(IrOperation.Square,[X()]);
    private static IrNode Pow(int power)=>
        IrApply.Create(IrOperation.Power,[X(),N(power)]);
    private static IrNode Add(params IrNode[] values)=>IrApply.Create(IrOperation.Add,values);
    private static IrNode Sub(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Subtract,[a,b]);
    private static IrNode Mul(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Multiply,[a,b]);
    private static SymbolicPolynomialIntegralResult Integrate(
        IrNode expression,long lower=0,long upper=1,IrSymbol? variable=null)=>
        SymbolicExactPolynomialCalculus.Integrate(expression,variable??X(),R(lower),R(upper));
    private static SymbolicPolynomialLimitResult Limit(
        IrNode expression,ExactRational point,IrSymbol? variable=null)=>
        SymbolicExactPolynomialCalculus.FiniteLimit(expression,variable??X(),point);
    private static ExactRational Value(SymbolicPolynomialIntegralResult result)
    {
        Assert.IsTrue(result.IsExact);
        Assert.AreEqual(SymbolicPolynomialCalculusStatus.Exact,result.Status);
        Assert.IsNotNull(result.ZeroConstantPrimitive);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
            result.ConstantConvention);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.IntegralRule,result.RuleId);
        Assert.AreEqual(SymbolicExactDifferentiation.Rule,result.DerivativeRuleId);
        Assert.IsNotNull(result.SourceSemanticSha256);
        Assert.IsNotNull(result.VariableIdentity);
        if(result.DefiniteValue is not ExactRational value)
            throw new AssertFailedException("Exact integral must contain a rational.");
        return value;
    }
    private static void Refused(SymbolicPolynomialCalculusStatus expected,
        SymbolicPolynomialIntegralResult result)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.IsExact);
        Assert.IsNull(result.ZeroConstantPrimitive);
        Assert.IsFalse(result.DefiniteValue.HasValue);
        Assert.IsNull(result.ConstantConvention);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.DerivativeRuleId);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
    }
    private static ExactRational LimitValue(SymbolicPolynomialLimitResult result)
    {
        Assert.IsTrue(result.IsExact);
        Assert.AreEqual(SymbolicPolynomialCalculusStatus.Exact,result.Status);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.LimitRule,result.RuleId);
        Assert.IsNotNull(result.SourceSemanticSha256);
        Assert.IsNotNull(result.VariableIdentity);
        if(result.Value is not ExactRational value)
            throw new AssertFailedException("Exact finite limit must contain a rational.");
        return value;
    }
    private static void LimitRefused(
        SymbolicPolynomialCalculusStatus status,SymbolicPolynomialLimitResult result)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsExact);
        Assert.IsFalse(result.Value.HasValue);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
    }

    [TestMethod]
    public void IntegralOfXOverZeroOneIsExactHalf() =>
        Assert.AreEqual(R(1,2),Value(Integrate(X())));

    [TestMethod]
    public void IntegralOfXOverZeroTwoIsTwo() =>
        Assert.AreEqual(R(2),Value(Integrate(X(),0,2)));

    [TestMethod]
    public void ReversingIntegrationBoundsNegatesExactHalf() =>
        Assert.AreEqual(R(-1,2),Value(Integrate(X(),1,0)));

    [TestMethod]
    public void SymmetricOddIntegralIsZero() =>
        Assert.AreEqual(R(0),Value(Integrate(X(),-1,1)));

    [TestMethod]
    public void ConstantThreeIntegratedOverZeroTwoIsSix() =>
        Assert.AreEqual(R(6),Value(Integrate(N(3),0,2)));

    [TestMethod]
    public void ZeroIntegrandHasExactZeroIntegralAndPrimitive() =>
        Assert.AreEqual(R(0),Value(Integrate(N(0))));

    [TestMethod]
    public void SquareOverZeroThreeHasIntegralNine() =>
        Assert.AreEqual(R(9),Value(Integrate(Sq(),0,3)));

    [TestMethod]
    public void SquareOverZeroOneIsOneThird() =>
        Assert.AreEqual(R(1,3),Value(Integrate(Sq())));

    [TestMethod]
    public void CubicOverZeroTwoIsFour() =>
        Assert.AreEqual(R(4),Value(Integrate(Pow(3),0,2)));

    [TestMethod]
    public void DegreeSevenOverZeroOneIsOneEighth() =>
        Assert.AreEqual(R(1,8),Value(Integrate(Pow(7))));

    [TestMethod]
    public void DegreeEightExceedsQualifiedDifferentiationBound() =>
        Refused(SymbolicPolynomialCalculusStatus.BudgetExceeded,
            Integrate(Pow(8)));

    [TestMethod]
    public void XPlusOneOverZeroOneIsThreeHalves() =>
        Assert.AreEqual(R(3,2),Value(Integrate(Add(X(),N(1)))));

    [TestMethod]
    public void SquareMinusXOverZeroOneIsMinusOneSixth() =>
        Assert.AreEqual(R(-1,6),Value(Integrate(Sub(Sq(),X()))));

    [TestMethod]
    public void HalfTimesXOverZeroTwoIsOne() =>
        Assert.AreEqual(R(1),Value(Integrate(Mul(Q(1,2),X()),0,2)));

    [TestMethod]
    public void HalfTimesSquareOverZeroOneIsOneSixth() =>
        Assert.AreEqual(R(1,6),Value(Integrate(Mul(Q(1,2),Sq()))));

    [TestMethod]
    public void ScalarOnRightIsAnExactIntegral() =>
        Assert.AreEqual(R(1),Value(Integrate(Mul(X(),N(2)))));

    [TestMethod]
    public void MixedQuadraticLinearConstantHasExactIntegral()
    {
        var polynomial=Add(Sq(),Mul(N(3),X()),N(1));
        Assert.AreEqual(R(17,6),Value(Integrate(polynomial)));
    }

    [TestMethod]
    public void NegativeScalarProducesSignedExactIntegral() =>
        Assert.AreEqual(R(-6),Value(Integrate(Mul(N(-3),X()),0,2)));

    [TestMethod]
    public void EqualBoundsAlwaysYieldZeroForQualifiedPolynomial() =>
        Assert.AreEqual(R(0),Value(Integrate(Pow(7),2,2)));

    [TestMethod]
    public void EvenSquareOverMinusOneOneIsTwoThirds() =>
        Assert.AreEqual(R(2,3),Value(Integrate(Sq(),-1,1)));

    [TestMethod]
    public void FormalDerivativeOfProducedPrimitiveIsAdmitted()
    {
        var result=Integrate(X());
        Value(result);
        var proof=SymbolicExactDifferentiation.TryDifferentiate(
            result.ZeroConstantPrimitive!,X());
        Assert.IsTrue(proof.IsAdmitted);
        Assert.AreEqual(SymbolicExactDifferentiation.Rule,proof.ProofRule);
    }

    [TestMethod]
    public void PointwiseDerivativeOfPrimitiveRecoversXAtTwo()
    {
        var result=Integrate(X());
        Value(result);
        var derivative=SymbolicDerivativePointAnalysis.Analyze(
            result.ZeroConstantPrimitive!,X(),R(2));
        Assert.IsTrue(derivative.HasExactValue);
        Assert.AreEqual(R(2),derivative.Value!.Value);
    }

    [TestMethod]
    public void ExactIntegralCarriesOriginalSourceHashAndTargetIdentity()
    {
        var x=X();var original=Add(Sq(),x);
        var result=SymbolicExactPolynomialCalculus.Integrate(original,x,R(0),R(1));
        Value(result);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SourceSemanticSha256);
        Assert.AreEqual(x.Identity,result.VariableIdentity);
    }

    [TestMethod]
    public void DifferentFreeVariableIsNotAnImplicitConstantParameter() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(IrSymbol.Free("y","x")));

    [TestMethod]
    public void BoundVariableCannotDefineRealPolynomialIntegral() =>
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedDomain,
            Integrate(X(),variable:IrSymbol.Bound("scope",0,"x")));

    [TestMethod]
    public void IntegerVariableCannotDefineRealPolynomialIntegral() =>
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedDomain,
            Integrate(X(),variable:IrSymbol.Free("x","x",IrScalarDomain.Integer)));

    [TestMethod]
    public void DimensionalQuantityMustNotBeIntegratedAsDimensionless()
    {
        var literal=new IrQuantityLiteral(N(1),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedUnits,
            Integrate(literal));
    }

    [TestMethod]
    public void ZeroTimesUnitIsNotSilentlyNormalizedToZero()
    {
        var literal=new IrQuantityLiteral(N(1),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedUnits,
            Integrate(Mul(N(0),literal)));
    }

    [TestMethod]
    public void RestrictedInputDoesNotDropOriginalDomain()
    {
        var assumption=IrAssumptionSet.Create([
            new IrRelation(X(),IrRelationKind.GreaterOrEqual,N(0))]);
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedAssumptions,
            Integrate(new IrRestrictedExpression(Sq(),assumption)));
    }

    [TestMethod]
    public void ApproximateBinary64InputIsExplicitlyRefused() =>
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedApproximation,
            Integrate(IrFiniteBinary64Scalar.FromDouble(0.5)));

    [TestMethod]
    public void HiddenBinary64InSumIsStillRefused() =>
        Refused(SymbolicPolynomialCalculusStatus.UnsupportedApproximation,
            Integrate(Add(X(),IrFiniteBinary64Scalar.FromDouble(1))));

    [TestMethod]
    public void UnqualifiedSqrtHasNoPolynomialPrimitive() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(IrApply.Create(IrOperation.Sqrt,[X()])));

    [TestMethod]
    public void AbsoluteValueNeedsPiecewiseRulesNotAdmittedHere() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(IrApply.Create(IrOperation.Abs,[X()])));

    [TestMethod]
    public void QuotientHasNoSilentAntiderivative()
    {
        var ratio=IrApply.Create(IrOperation.Divide,[X(),Add(X(),N(1))]);
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,Integrate(ratio));
    }

    [TestMethod]
    public void ProductOfTwoFreeVariablesIsNotConstantMultiplication() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(Mul(X(),X())));

    [TestMethod]
    public void NegativeIntegerExponentIsNotAnAdmittedPolynomial() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(IrApply.Create(IrOperation.Power,[X(),N(-1)])));

    [TestMethod]
    public void FractionalExponentRequiresOtherScientificRules() =>
        Refused(SymbolicPolynomialCalculusStatus.Unsupported,
            Integrate(IrApply.Create(IrOperation.Power,[X(),Q(1,2)])));

    [TestMethod]
    public void ExactEndpointOutside128BitsIsBudgetExceeded() =>
        Refused(SymbolicPolynomialCalculusStatus.BudgetExceeded,
            SymbolicExactPolynomialCalculus.Integrate(X(),X(),
                new ExactRational(BigInteger.One<<140,1),R(0)));

    [TestMethod]
    public void ExactCoefficientOutside128BitsIsBudgetExceeded() =>
        Refused(SymbolicPolynomialCalculusStatus.BudgetExceeded,
            Integrate(new IrExactScalar(
                new ExactRational(BigInteger.One<<140,1))));

    [TestMethod]
    public void FiniteLimitOfSquareAtTwoIsFour() =>
        Assert.AreEqual(R(4),LimitValue(Limit(Sq(),R(2))));

    [TestMethod]
    public void FiniteLimitOfXPlusSquareAtOneIsTwo() =>
        Assert.AreEqual(R(2),LimitValue(Limit(Add(X(),Sq()),R(1))));

    [TestMethod]
    public void FiniteLimitOfXAtOneThirdIsExact() =>
        Assert.AreEqual(R(1,3),LimitValue(Limit(X(),R(1,3))));

    [TestMethod]
    public void FiniteLimitOfConstantIsIndependentOfPoint() =>
        Assert.AreEqual(R(5),LimitValue(Limit(N(5),R(-7))));

    [TestMethod]
    public void FiniteLimitOfDegreeSevenAtTwoIs128() =>
        Assert.AreEqual(R(128),LimitValue(Limit(Pow(7),R(2))));

    [TestMethod]
    public void FiniteLimitOfSquareAtNegativeTwoIsFour() =>
        Assert.AreEqual(R(4),LimitValue(Limit(Sq(),R(-2))));

    [TestMethod]
    public void FiniteLimitDoesNotClaimUnknownRadical() =>
        LimitRefused(SymbolicPolynomialCalculusStatus.Unsupported,
            Limit(IrApply.Create(IrOperation.Sqrt,[X()]),R(1)));

    [TestMethod]
    public void FiniteLimitDoesNotDropAbsoluteValueBranch() =>
        LimitRefused(SymbolicPolynomialCalculusStatus.Unsupported,
            Limit(IrApply.Create(IrOperation.Abs,[X()]),R(0)));

    [TestMethod]
    public void FiniteLimitOfDimensionalExpressionIsRefused()
    {
        var unit=new IrQuantityLiteral(N(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        LimitRefused(SymbolicPolynomialCalculusStatus.UnsupportedUnits,
            Limit(unit,R(2)));
    }

    [TestMethod]
    public void FiniteLimitAtOversizedPointIsBudgetExceeded() =>
        LimitRefused(SymbolicPolynomialCalculusStatus.BudgetExceeded,
            Limit(Sq(),new ExactRational(BigInteger.One<<140,1)));

    [TestMethod]
    public void FiniteLimitProvenanceMatchesOriginalSource()
    {
        var expression=Add(X(),Sq());var x=X();
        var result=Limit(expression,R(2),x);
        Assert.AreEqual(R(6),LimitValue(result));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(expression),
            result.SourceSemanticSha256);
        Assert.AreEqual(x.Identity,result.VariableIdentity);
    }
}
