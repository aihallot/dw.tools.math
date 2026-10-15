using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C02T1Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrExactScalar N(long value)=>new(new ExactRational(value,1));
    private static IrExactScalar Q(long numerator,long denominator)=>
        new(new ExactRational(numerator,denominator));
    private static IrNode Square(IrSymbol x)=>IrApply.Create(IrOperation.Square,[x]);
    private static SymbolicRealSolvingResult Eq(IrNode left,IrNode right,IrSymbol? x=null)=>
        SymbolicExactRealSolving.Solve(left,IrRelationKind.Equal,right,x??X());
    private static SymbolicRealSolvingResult Ge(IrNode left,IrNode right,IrSymbol? x=null)=>
        SymbolicExactRealSolving.Solve(left,IrRelationKind.GreaterOrEqual,right,x??X());
    private static void Refused(SymbolicRealSolvingStatus status,SymbolicRealSolvingResult r)
    {
        Assert.AreEqual(status,r.Status);
        Assert.IsFalse(r.IsQualified);
        Assert.IsFalse(r.IsComplete);
        Assert.IsTrue(r.Roots.IsEmpty);
        Assert.IsNull(r.ClosedBound);
        Assert.IsNull(r.RuleId);
        Assert.IsNull(r.LeftSemanticSha256);
        Assert.IsNull(r.RightSemanticSha256);
        Assert.IsNull(r.VariableIdentity);
    }
    private static void Finite(SymbolicRealSolvingResult r,params ExactRational[] expected)
    {
        Assert.IsTrue(r.IsQualified);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(SymbolicRealSolvingStatus.Finite,r.Status);
        CollectionAssert.AreEqual(expected,r.Roots.ToArray());
        Assert.IsNull(r.ClosedBound);
    }

    [TestMethod]
    public void UnitSquareEqualsOneHasBothOrderedRoots() =>
        Finite(Eq(Square(X()),N(1)),new ExactRational(-1,1),ExactRational.One);

    [TestMethod]
    public void SquareEqualsFourNinthsHasExactFractionalRoots() =>
        Finite(Eq(Square(X()),Q(4,9)),new ExactRational(-2,3),new ExactRational(2,3));

    [TestMethod]
    public void SquareEqualsZeroHasSingleDistinctRoot() =>
        Finite(Eq(Square(X()),N(0)),ExactRational.Zero);

    [TestMethod]
    public void SquareEqualsNegativeOneIsProvablyEmptyInReals()
    {
        var r=Eq(Square(X()),N(-1));
        Assert.AreEqual(SymbolicRealSolvingStatus.Empty,r.Status);
        Assert.IsTrue(r.IsQualified);
        Assert.IsTrue(r.Roots.IsEmpty);
    }

    [TestMethod]
    public void SquareEqualsTwoIsUnknownNotEmpty() =>
        Refused(SymbolicRealSolvingStatus.Unknown,Eq(Square(X()),N(2)));

    [TestMethod]
    public void SquareEqualsOneHalfIsUnknownNotEmpty() =>
        Refused(SymbolicRealSolvingStatus.Unknown,Eq(Square(X()),Q(1,2)));

    [TestMethod]
    public void ExplicitPowerTwoHasSameExactRoots() =>
        Finite(Eq(IrApply.Create(IrOperation.Power,[X(),N(2)]),N(9)),
            new ExactRational(-3,1),new ExactRational(3,1));

    [TestMethod]
    public void PowerThreeDoesNotPretendToBeQuadratic() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Eq(IrApply.Create(IrOperation.Power,[X(),N(3)]),N(8)));

    [TestMethod]
    public void RealVariableEqualsFiveHasOneRoot() =>
        Finite(Eq(X(),N(5)),new ExactRational(5,1));

    [TestMethod]
    public void ReversedLinearEquationHasSameRoot() =>
        Finite(Eq(N(5),X()),new ExactRational(5,1));

    [TestMethod]
    public void ReversedQuadraticEquationHasBothRoots() =>
        Finite(Eq(N(1),Square(X())),new ExactRational(-1,1),ExactRational.One);

    [TestMethod]
    public void SameFreeVariableEqualsItselfForAllReals()
    {
        var r=Eq(X(),X());
        Assert.AreEqual(SymbolicRealSolvingStatus.AllReal,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void TrueConstantEquationHoldsForAllReals()
    {
        var r=Eq(N(3),N(3));
        Assert.AreEqual(SymbolicRealSolvingStatus.AllReal,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void FalseConstantEquationIsEmpty()
    {
        var r=Eq(N(3),N(4));
        Assert.AreEqual(SymbolicRealSolvingStatus.Empty,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void DifferentFreeSymbolIsUnknownNotAnInventedRoot() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Eq(IrSymbol.Free("y","x"),N(3)));

    [TestMethod]
    public void UnknownSqrtEquationDoesNotClaimAnExactSolution() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Eq(IrApply.Create(IrOperation.Sqrt,[X()]),N(1)));

    [TestMethod]
    public void QuotientEquationIsNotSilentlyCancelled() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Eq(IrApply.Create(IrOperation.Divide,[Square(X()),X()]),N(1)));

    [TestMethod]
    public void RestrictedOriginalEquationMustRetainItsAssumptions()
    {
        var expr=new IrRestrictedExpression(Square(X()),IrAssumptionSet.Empty);
        Refused(SymbolicRealSolvingStatus.UnsupportedAssumptions,Eq(expr,N(1)));
    }

    [TestMethod]
    public void DimensionalLiteralIsNotAnExactRootEquation()
    {
        var measure=new IrQuantityLiteral(N(1),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicRealSolvingStatus.UnsupportedUnits,Eq(X(),measure));
    }

    [TestMethod]
    public void NestedDimensionalLiteralIsStillRefused()
    {
        var measure=new IrQuantityLiteral(N(1),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        var nested=IrApply.Create(IrOperation.Add,[X(),measure]);
        Refused(SymbolicRealSolvingStatus.UnsupportedUnits,Eq(nested,N(1)));
    }

    [TestMethod]
    public void Binary64CannotSilentlyBecomeAnExactRational() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Eq(X(),IrFiniteBinary64Scalar.FromDouble(1)));

    [TestMethod]
    public void IntegerTargetIsNotAnAdmittedRealDomain() =>
        Refused(SymbolicRealSolvingStatus.UnsupportedDomain,
            SymbolicExactRealSolving.Solve(Square(X()),IrRelationKind.Equal,N(1),
                X(),IrScalarDomain.Integer));

    [TestMethod]
    public void BoundVariableCannotBeSolvedAsAFreeRealVariable() =>
        Refused(SymbolicRealSolvingStatus.UnsupportedDomain,
            Eq(Square(X()),N(1),IrSymbol.Bound("scope",0,"x")));

    [TestMethod]
    public void RealVariableGreaterOrEqualTwoIsClosedLowerRay()
    {
        var r=Ge(X(),N(2));
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(SymbolicRealSolvingStatus.LowerClosedRay,r.Status);
        Assert.AreEqual(new ExactRational(2,1),r.ClosedBound);
        Assert.IsTrue(r.Roots.IsEmpty);
    }

    [TestMethod]
    public void TwoGreaterOrEqualRealVariableIsClosedUpperRay()
    {
        var r=Ge(N(2),X());
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(SymbolicRealSolvingStatus.UpperClosedRay,r.Status);
        Assert.AreEqual(new ExactRational(2,1),r.ClosedBound);
        Assert.IsTrue(r.Roots.IsEmpty);
    }

    [TestMethod]
    public void VariableGreaterOrEqualItselfHoldsForAllReals()
    {
        var r=Ge(X(),X());
        Assert.AreEqual(SymbolicRealSolvingStatus.AllReal,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void SquareGreaterOrEqualZeroHoldsForAllReals()
    {
        var r=Ge(Square(X()),N(0));
        Assert.AreEqual(SymbolicRealSolvingStatus.AllReal,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void SquareGreaterOrEqualPositiveNeedsDisjointRays() =>
        Refused(SymbolicRealSolvingStatus.Unknown,Ge(Square(X()),N(1)));

    [TestMethod]
    public void NegativeBoundGreaterOrEqualSquareIsEmpty()
    {
        var r=Ge(N(-1),Square(X()));
        Assert.AreEqual(SymbolicRealSolvingStatus.Empty,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void ZeroGreaterOrEqualSquareHasOnlyZero() =>
        Finite(Ge(N(0),Square(X())),ExactRational.Zero);

    [TestMethod]
    public void ExactProvenanceBindsBothSidesAndVariable()
    {
        var x=X();var left=Square(x);var right=N(1);
        var r=Eq(left,right,x);
        Assert.IsTrue(r.IsQualified);
        Assert.AreEqual(SymbolicExactRealSolving.Rule,r.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(left),
            r.LeftSemanticSha256);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(right),
            r.RightSemanticSha256);
        Assert.AreEqual(x.Identity,r.VariableIdentity);
    }

    [TestMethod]
    public void UnsupportedNotEqualRelationDoesNotGuessSolutions() =>
        Refused(SymbolicRealSolvingStatus.Unsupported,
            SymbolicExactRealSolving.Solve(X(),IrRelationKind.NotEqual,N(1),X()));

    [TestMethod]
    public void OversizeExactInputExhaustsTheExplicitRationalBudget() =>
        Refused(SymbolicRealSolvingStatus.BudgetExceeded,
            Eq(X(),new IrExactScalar(new ExactRational(BigInteger.One<<140,1))));

    [TestMethod]
    public void ConstantTrueGreaterOrEqualProducesAllReals()
    {
        var r=Ge(N(5),N(3));
        Assert.AreEqual(SymbolicRealSolvingStatus.AllReal,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void ConstantFalseGreaterOrEqualProducesEmptySet()
    {
        var r=Ge(N(3),N(5));
        Assert.AreEqual(SymbolicRealSolvingStatus.Empty,r.Status);
        Assert.IsTrue(r.IsComplete);
    }

    [TestMethod]
    public void UnsupportedSqrtInequalityMustRemainUnknown() =>
        Refused(SymbolicRealSolvingStatus.Unknown,
            Ge(IrApply.Create(IrOperation.Sqrt,[X()]),N(0)));
}
