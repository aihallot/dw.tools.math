using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C03T2Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static ExactRational R(long n,long d=1)=>new(n,d);
    private static IrExactScalar N(long n)=>new(R(n));
    private static IrExactScalar Q(long n,long d)=>new(R(n,d));
    private static IrNode Square()=>IrApply.Create(IrOperation.Square,[X()]);
    private static IrNode Sub(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Subtract,[a,b]);
    private static IrNode Add(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Add,[a,b]);
    private static IrNode Mul(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Multiply,[a,b]);
    private static IrNode Div(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Divide,[a,b]);
    private static IrNode Simple(int numerator=1,int c=1)=>
        Div(N(numerator),Sub(X(),N(c)));
    private static IrNode Cancel(int c=1)=>
        Div(Sub(Square(),N(c*c)),Sub(X(),N(c)));
    private static SymbolicCalculusBoundaryResult Integral(
        IrNode source,long a=0,long b=1,IrSymbol? target=null)=>
        SymbolicRealCalculusBoundaries.AnalyzeIntegral(
            source,target??X(),R(a),R(b));
    private static SymbolicCalculusBoundaryResult At(
        IrNode source,long point,IrSymbol? target=null)=>
        SymbolicRealCalculusBoundaries.AnalyzeLimit(source,target??X(),R(point));
    private static SymbolicCalculusBoundaryResult At(
        IrNode source,ExactRational point,IrSymbol? target=null)=>
        SymbolicRealCalculusBoundaries.AnalyzeLimit(source,target??X(),point);
    private static ExactRational Value(SymbolicCalculusBoundaryResult result)
    {
        Assert.IsTrue(result.HasExactValue);
        Assert.IsTrue(result.Status is SymbolicCalculusBoundaryStatus.Exact or
            SymbolicCalculusBoundaryStatus.ExactRemovableLimit);
        Assert.IsNotNull(result.RuleId);
        Assert.IsNotNull(result.SourceSemanticSha256);
        Assert.IsNotNull(result.VariableIdentity);
        if(result.ExactValue is not ExactRational value)
            throw new AssertFailedException("An exact result must contain a rational.");
        return value;
    }
    private static void Boundary(
        SymbolicCalculusBoundaryStatus expected,ExactRational point,
        SymbolicCalculusBoundaryResult result)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsTrue(result.HasQualifiedBoundary);
        Assert.IsFalse(result.HasExactValue);
        Assert.IsNull(result.ExactValue);
        Assert.AreEqual(point,result.ExceptionalPoint);
        Assert.IsNull(result.ConstantConvention);
        Assert.IsNotNull(result.RuleId);
        Assert.IsNotNull(result.SourceSemanticSha256);
        Assert.IsNotNull(result.VariableIdentity);
    }
    private static void Refused(
        SymbolicCalculusBoundaryStatus expected,SymbolicCalculusBoundaryResult result)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.HasExactValue);
        Assert.IsFalse(result.HasQualifiedBoundary);
        Assert.IsNull(result.ExactValue);
        Assert.IsNull(result.ExceptionalPoint);
        Assert.IsNull(result.ConstantConvention);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
    }

    [TestMethod]
    public void PriorPolynomialIntegralOfXStillReturnsExactHalf()
    {
        var result=Integral(X());
        Assert.AreEqual(R(1,2),Value(result));
        Assert.AreEqual(SymbolicExactPolynomialCalculus.IntegralRule,result.RuleId);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
            result.ConstantConvention);
    }

    [TestMethod]
    public void PriorPolynomialIntegralOfSquareStillReturnsExactThird() =>
        Assert.AreEqual(R(1,3),Value(Integral(Square())));

    [TestMethod]
    public void ReversePolynomialBoundsProduceSignedExactNegativeHalf() =>
        Assert.AreEqual(R(-1,2),Value(Integral(X(),1,0)));

    [TestMethod]
    public void ConstantPolynomialIntegralUsesOriginalRationalBounds() =>
        Assert.AreEqual(R(6),Value(Integral(N(3),0,2)));

    [TestMethod]
    public void PolynomialFiniteLimitAtTwoIsExactlyFour() =>
        Assert.AreEqual(R(4),Value(At(Square(),2)));

    [TestMethod]
    public void PolynomialFiniteLimitAtOneThirdIsExact() =>
        Assert.AreEqual(R(1,3),Value(At(X(),R(1,3))));

    [TestMethod]
    public void SimplePoleInsideIntegralIntervalIsClassifiedExactly() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            Integral(Simple(),0,2));

    [TestMethod]
    public void SimplePoleAtLeftIntegralEndpointIsClassifiedExactly() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            Integral(Simple(),1,2));

    [TestMethod]
    public void SimplePoleAtRightIntegralEndpointIsClassifiedExactly() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            Integral(Simple(),0,1));

    [TestMethod]
    public void ReversedIntegralBoundsDoNotHideInteriorPole() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            Integral(Simple(),2,0));

    [TestMethod]
    public void PoleOutsideIntervalDoesNotInventLogarithmicPrimitive() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(Simple(),2,3));

    [TestMethod]
    public void ReciprocalOfXHasRealPoleAtZero() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(0),
            Integral(Div(N(1),X()),-1,1));

    [TestMethod]
    public void NonzeroNegativeNumeratorStillHasSimplePole() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            Integral(Simple(-3),0,2));

    [TestMethod]
    public void ZeroQuotientAcrossExcludedPointHasHoleNotPole() =>
        Boundary(SymbolicCalculusBoundaryStatus.OriginalDomainHole,R(1),
            Integral(Simple(0),0,2));

    [TestMethod]
    public void ZeroQuotientOnHoleFreeIntervalHasExactZeroIntegral()
    {
        var result=Integral(Simple(0),2,3);
        Assert.AreEqual(R(0),Value(result));
        Assert.AreEqual(SymbolicRealCalculusBoundaries.ZeroQuotientRule,result.RuleId);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
            result.ConstantConvention);
    }

    [TestMethod]
    public void ZeroQuotientReversedHoleFreeIntervalIsStillExactlyZero() =>
        Assert.AreEqual(R(0),Value(Integral(Simple(0),3,2)));

    [TestMethod]
    public void ZeroQuotientAtRemovedPointHasExactRemovableLimitNotValue()
    {
        var result=At(Simple(0),1);
        Assert.AreEqual(SymbolicCalculusBoundaryStatus.ExactRemovableLimit,result.Status);
        Assert.AreEqual(R(0),Value(result));
        Assert.AreEqual(R(1),result.ExceptionalPoint);
        Assert.IsTrue(result.HasQualifiedBoundary);
    }

    [TestMethod]
    public void ZeroQuotientAwayFromHoleHasExactFiniteLimit() =>
        Assert.AreEqual(R(0),Value(At(Simple(0),2)));

    [TestMethod]
    public void SimpleRationalAtPoleHasNoFiniteExactLimit() =>
        Boundary(SymbolicCalculusBoundaryStatus.SimplePole,R(1),
            At(Simple(),1));

    [TestMethod]
    public void SimpleRationalAwayFromPoleHasExactLimitOfThree() =>
        Assert.AreEqual(R(3),Value(At(Simple(3),2)));

    [TestMethod]
    public void SimpleRationalAtHalfDistanceHasExactLimitOfTwo() =>
        Assert.AreEqual(R(2),Value(At(Simple(),R(3,2))));

    [TestMethod]
    public void SimpleRationalToLeftHasNegativeFiniteLimit() =>
        Assert.AreEqual(R(-3),Value(At(Simple(3),0)));

    [TestMethod]
    public void RemovableOriginalRationalHoleWithinBoundsIsExplicit()
    {
        var original=Cancel();
        var result=Integral(original,0,2);
        Boundary(SymbolicCalculusBoundaryStatus.OriginalDomainHole,R(1),result);
        Assert.AreEqual(SymbolicRealCalculusBoundaries.HoleRule,result.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SourceSemanticSha256);
    }

    [TestMethod]
    public void RemovableOriginalHoleAtIntegralEndpointIsExplicit() =>
        Boundary(SymbolicCalculusBoundaryStatus.OriginalDomainHole,R(1),
            Integral(Cancel(),1,2));

    [TestMethod]
    public void RemovableOriginalHoleInReversedBoundsRemainsExcluded() =>
        Boundary(SymbolicCalculusBoundaryStatus.OriginalDomainHole,R(1),
            Integral(Cancel(),2,0));

    [TestMethod]
    public void CancellingOnlyOutsideOriginalHoleYieldsExactSevenHalves()
    {
        var original=Cancel();
        var result=Integral(original,2,3);
        Assert.AreEqual(R(7,2),Value(result));
        Assert.AreEqual(SymbolicRealCalculusBoundaries.HoleFreeIntegralRule,
            result.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SourceSemanticSha256);
        Assert.AreEqual(SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
            result.ConstantConvention);
    }

    [TestMethod]
    public void ReversedHoleFreeCancellationIntegralIsExactMinusSevenHalves() =>
        Assert.AreEqual(R(-7,2),Value(Integral(Cancel(),3,2)));

    [TestMethod]
    public void RemovableHoleLimitIsTwoButOriginalFunctionUndefined()
    {
        var original=Cancel();
        var result=At(original,1);
        Assert.AreEqual(SymbolicCalculusBoundaryStatus.ExactRemovableLimit,
            result.Status);
        Assert.AreEqual(R(2),Value(result));
        Assert.AreEqual(R(1),result.ExceptionalPoint);
        Assert.IsTrue(result.HasQualifiedBoundary);
        Assert.AreEqual(SymbolicRealCalculusBoundaries.RemovableLimitRule,result.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SourceSemanticSha256);
    }

    [TestMethod]
    public void RemovableRationalAwayFromHoleHasExactLimitOfThree() =>
        Assert.AreEqual(R(3),Value(At(Cancel(),2)));

    [TestMethod]
    public void RemovableRationalAtZeroHasExactLimitOfOne() =>
        Assert.AreEqual(R(1),Value(At(Cancel(),0)));

    [TestMethod]
    public void RemovableRationalWithOffsetTwoHasLimitFourAtHole()
    {
        var result=At(Cancel(2),2);
        Assert.AreEqual(R(4),Value(result));
        Assert.AreEqual(SymbolicCalculusBoundaryStatus.ExactRemovableLimit,
            result.Status);
        Assert.AreEqual(R(2),result.ExceptionalPoint);
    }

    [TestMethod]
    public void OffsetTwoHoleFreeIntegralOverThreeFourIsElevenHalves() =>
        Assert.AreEqual(R(11,2),Value(Integral(Cancel(2),3,4)));

    [TestMethod]
    public void UnqualifiedGenericQuotientIsNotInventedAsASimplePole() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(Div(X(),Sub(X(),N(1))),0,2));

    [TestMethod]
    public void UnqualifiedGenericQuotientAwayFromPoleIsStillUnknown() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(Div(X(),Sub(X(),N(1))),2,3));

    [TestMethod]
    public void UnqualifiedGenericQuotientLimitStaysUnknown() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            At(Div(X(),Sub(X(),N(1))),1));

    [TestMethod]
    public void UnknownSqrtIntegralCannotBeClaimedAsExactPolynomial() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(IrApply.Create(IrOperation.Sqrt,[X()])));

    [TestMethod]
    public void UnknownAbsoluteIntegralDoesNotDiscardPiecewiseForm() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(IrApply.Create(IrOperation.Abs,[X()])));

    [TestMethod]
    public void UnknownSqrtFiniteLimitRequiresSeparateRadicalQualification() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            At(IrApply.Create(IrOperation.Sqrt,[X()]),1));

    [TestMethod]
    public void NestedQuotientDoesNotClaimUnprovedConvergence() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(Add(X(),Simple()),0,2));

    [TestMethod]
    public void DimensionalLiteralIsNeverTreatedAsRealPolynomial()
    {
        var u=new IrQuantityLiteral(N(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedUnits,Integral(u));
    }

    [TestMethod]
    public void NestedDimensionalTermInsideQuotientIsNotSilentlyIgnored()
    {
        var u=new IrQuantityLiteral(N(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedUnits,
            Integral(Div(u,Sub(X(),N(1))),2,3));
    }

    [TestMethod]
    public void RetainedOriginalAssumptionIsNotRemovedByCalculus()
    {
        var a=IrAssumptionSet.Create([
            new IrRelation(X(),IrRelationKind.NotEqual,N(1))]);
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedAssumptions,
            Integral(new IrRestrictedExpression(Cancel(),a),2,3));
    }

    [TestMethod]
    public void Binary64InQuotientCannotTurnIntoImplicitExactValue() =>
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedApproximation,
            At(Div(IrFiniteBinary64Scalar.FromDouble(1),Sub(X(),N(1))),2));

    [TestMethod]
    public void BoundTargetCannotClaimRealIntegral() =>
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedDomain,
            Integral(X(),0,1,IrSymbol.Bound("s",0,"x")));

    [TestMethod]
    public void IntegerTargetCannotClaimRealLimit() =>
        Refused(SymbolicCalculusBoundaryStatus.UnsupportedDomain,
            At(X(),1,IrSymbol.Free("x","x",IrScalarDomain.Integer)));

    [TestMethod]
    public void IndependentFreeRealSymbolRemainsUnqualified() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(IrSymbol.Free("y","y")));

    [TestMethod]
    public void DegreeEightInputExceedsQualifiedPrimitiveBound() =>
        Refused(SymbolicCalculusBoundaryStatus.BudgetExceeded,
            Integral(IrApply.Create(IrOperation.Power,[X(),N(8)])));

    [TestMethod]
    public void OversizedRationalIntegrationBoundIsBudgetExceeded() =>
        Refused(SymbolicCalculusBoundaryStatus.BudgetExceeded,
            SymbolicRealCalculusBoundaries.AnalyzeIntegral(
                X(),X(),new ExactRational(BigInteger.One<<140,1),R(1)));

    [TestMethod]
    public void OversizedRationalLimitPointIsBudgetExceeded() =>
        Refused(SymbolicCalculusBoundaryStatus.BudgetExceeded,
            At(X(),new ExactRational(BigInteger.One<<140,1)));

    [TestMethod]
    public void UnqualifiedUnknownMustCarryNeitherResultNorMathematicalProof() =>
        Refused(SymbolicCalculusBoundaryStatus.UnknownConvergence,
            Integral(Mul(X(),X())));

    [TestMethod]
    public void PolynomialIntegralProvenancePreservesOriginalSource()
    {
        var original=Add(X(),N(1));
        var result=Integral(original);
        Assert.AreEqual(R(3,2),Value(result));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SourceSemanticSha256);
        Assert.AreEqual(X().Identity,result.VariableIdentity);
    }
}
