using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C02T2Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrExactScalar N(long n)=>new(new ExactRational(n,1));
    private static IrExactScalar Q(long n,long d)=>new(new ExactRational(n,d));
    private static IrNode Square(IrNode x)=>IrApply.Create(IrOperation.Square,[x]);
    private static IrNode Minus(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Subtract,[a,b]);
    private static IrNode Plus(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Add,[a,b]);
    private static IrNode Times(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Multiply,[a,b]);
    private static IrNode Divide(IrNode a,IrNode b)=>
        IrApply.Create(IrOperation.Divide,[a,b]);
    private static IrNode Quotient()=>
        Divide(Minus(Square(X()),N(1)),Minus(X(),N(1)));
    private static ExactRational R(long n,long d=1)=>new(n,d);
    private static SymbolicRootAuditResult Check(
        IrNode a,IrNode b,params long[] candidates)=>
        SymbolicRootAudit.Verify(a,b,X(),candidates.Select(x=>R(x)));
    private static SymbolicRootAuditResult Check(
        IrNode a,IrNode b,params ExactRational[] candidates)=>
        SymbolicRootAudit.Verify(a,b,X(),candidates);
    private static void Refused(SymbolicRootAuditStatus expected,
        SymbolicRootAuditResult result)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.IsComplete);
        Assert.IsFalse(result.HasVerifiedRoots);
        Assert.IsTrue(result.VerifiedRoots.IsEmpty);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.LeftSemanticSha256);
        Assert.IsNull(result.RightSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
        Assert.IsFalse(result.ProvenEmpty);
    }

    [TestMethod]
    public void QuadraticTwoRootsExactlyAccountedForIsComplete()
    {
        var r=Check(Square(X()),N(1),-1,1);
        Assert.IsTrue(r.IsComplete);
        Assert.IsTrue(r.HasVerifiedRoots);
        Assert.AreEqual(2,r.VerifiedRoots.Length);
        Assert.AreEqual(R(-1),r.VerifiedRoots[0].Value);
        Assert.AreEqual(R(1),r.VerifiedRoots[1].Value);
        Assert.IsFalse(r.ProvenEmpty);
    }

    [TestMethod]
    public void MissingOneKnownQuadraticRootIsExplicitlyPartial()
    {
        var r=Check(Square(X()),N(1),1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.IsFalse(r.IsComplete);
        Assert.AreEqual(1,r.VerifiedRoots.Length);
    }

    [TestMethod]
    public void ZeroSquaredHasVerifiedDoubleRoot()
    {
        var r=Check(Square(X()),N(0),0);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(2,r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void NonzeroQuadraticRootsHaveMultiplicityOne()
    {
        var r=Check(Square(X()),N(4),-2,2);
        Assert.IsTrue(r.IsComplete);
        Assert.IsTrue(r.VerifiedRoots.All(x=>x.Multiplicity==1));
    }

    [TestMethod]
    public void LinearRootHasMultiplicityOne()
    {
        var r=Check(X(),N(5),5);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(1,r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void ReversedLinearEqualityRetainsSimpleMultiplicity()
    {
        var r=Check(N(5),X(),5);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(1,r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void ReversedSquareEqualityKeepsDoubleRootAtZero()
    {
        var r=Check(N(0),Square(X()),0);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(2,r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void ExplicitPowerTwoRetainsProvedMultiplicity()
    {
        var power=IrApply.Create(IrOperation.Power,[X(),N(2)]);
        var r=Check(power,N(0),0);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(2,r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void NegativeQuadraticTargetHasProvenEmptyCompleteSetWithoutCandidates()
    {
        var r=Check(Square(X()),N(-1),Array.Empty<long>());
        Assert.AreEqual(SymbolicRootAuditStatus.Complete,r.Status);
        Assert.IsTrue(r.ProvenEmpty);
        Assert.IsTrue(r.VerifiedRoots.IsEmpty);
        Assert.IsNotNull(r.RuleId);
    }

    [TestMethod]
    public void EmptyListForKnownNonemptyEquationIsUnknown()
    {
        var r=Check(Square(X()),N(1),Array.Empty<long>());
        Refused(SymbolicRootAuditStatus.Unknown,r);
    }

    [TestMethod]
    public void UnknownIrrationalRootsNeverBecomeEmptyForMissingCandidates()
    {
        var r=Check(Square(X()),N(2),Array.Empty<long>());
        Refused(SymbolicRootAuditStatus.Unknown,r);
    }

    [TestMethod]
    public void FalseRationalCandidateForIrrationalEquationIsNotNoSolutionProof()
    {
        var r=Check(Square(X()),N(2),1);
        Assert.AreEqual(SymbolicRootAuditStatus.NoVerifiedCandidates,r.Status);
        Assert.IsFalse(r.ProvenEmpty);
        Assert.IsFalse(r.IsComplete);
        Assert.AreEqual(R(1),r.RejectedCandidates.Single());
    }

    [TestMethod]
    public void FalseCandidateOnLinearEquationIsRecordedRatherThanAccepted()
    {
        var r=Check(X(),N(2),3);
        Assert.AreEqual(SymbolicRootAuditStatus.NoVerifiedCandidates,r.Status);
        Assert.AreEqual(R(3),r.RejectedCandidates.Single());
    }

    [TestMethod]
    public void ExtraFalseCandidateDoesNotInvalidateAnAlreadyProvenCompleteSet()
    {
        var r=Check(Square(X()),N(0),0,1);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(R(1),r.RejectedCandidates.Single());
    }

    [TestMethod]
    public void RationalOriginalQuotientFiltersThePoleAtOne()
    {
        var r=Check(Quotient(),N(0),-1,1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(R(-1),r.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(1),r.ExcludedFromDomain.Single());
        Assert.IsFalse(r.IsComplete);
    }

    [TestMethod]
    public void FalseCancellationRootAtOriginalPoleIsNotARealRoot()
    {
        var r=Check(Quotient(),N(2),1);
        Assert.AreEqual(SymbolicRootAuditStatus.NoVerifiedCandidates,r.Status);
        Assert.IsFalse(r.ProvenEmpty);
        Assert.AreEqual(R(1),r.ExcludedFromDomain.Single());
    }

    [TestMethod]
    public void ExactOriginalQuotientCanRejectNonrootAtDefinedPoint()
    {
        var r=Check(Quotient(),N(0),2);
        Assert.AreEqual(SymbolicRootAuditStatus.NoVerifiedCandidates,r.Status);
        Assert.AreEqual(R(2),r.RejectedCandidates.Single());
    }

    [TestMethod]
    public void QuotientVerifiedRootDoesNotInventMultiplicity()
    {
        var r=Check(Quotient(),N(0),-1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.IsNull(r.VerifiedRoots.Single().Multiplicity);
    }

    [TestMethod]
    public void ExistingNotEqualRestrictionExcludesCandidateEvenIfBodyMatches()
    {
        var assumptions=IrAssumptionSet.Create([
            new IrRelation(X(),IrRelationKind.NotEqual,N(1))]);
        var restricted=new IrRestrictedExpression(Square(X()),assumptions);
        var r=Check(restricted,N(1),-1,1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(R(-1),r.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(1),r.ExcludedFromDomain.Single());
    }

    [TestMethod]
    public void ExistingGreaterOrEqualRestrictionExcludesNegativeCandidate()
    {
        var assumptions=IrAssumptionSet.Create([
            new IrRelation(X(),IrRelationKind.GreaterOrEqual,N(0))]);
        var restricted=new IrRestrictedExpression(Square(X()),assumptions);
        var r=Check(restricted,N(1),-1,1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(R(1),r.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(-1),r.ExcludedFromDomain.Single());
    }

    [TestMethod]
    public void ExistingEqualityRestrictionExcludesOtherCandidate()
    {
        var assumptions=IrAssumptionSet.Create([
            new IrRelation(X(),IrRelationKind.Equal,N(1))]);
        var restricted=new IrRestrictedExpression(Square(X()),assumptions);
        var r=Check(restricted,N(1),-1,1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(R(1),r.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(-1),r.ExcludedFromDomain.Single());
    }

    [TestMethod]
    public void ForeignRestrictionRefusedInsteadOfSilentlyIgnored()
    {
        var assumptions=IrAssumptionSet.Create([
            new IrRelation(IrSymbol.Free("y","y"),IrRelationKind.NotEqual,N(1))]);
        var restricted=new IrRestrictedExpression(Square(X()),assumptions);
        Refused(SymbolicRootAuditStatus.UnsupportedAssumptions,
            Check(restricted,N(1),1));
    }

    [TestMethod]
    public void NestedDimensionalQuantityIsRefusedWithNoProof()
    {
        var unit=new IrQuantityLiteral(N(1),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicRootAuditStatus.UnsupportedUnits,
            Check(Plus(X(),unit),N(2),1));
    }

    [TestMethod]
    public void UnsupportedSqrtCandidateIsUnresolvedNotWronglyRejected()
    {
        var sqrt=IrApply.Create(IrOperation.Sqrt,[X()]);
        var r=Check(sqrt,N(1),1);
        Assert.AreEqual(SymbolicRootAuditStatus.Unknown,r.Status);
        Assert.AreEqual(R(1),r.UnresolvedCandidates.Single());
        Assert.IsFalse(r.ProvenEmpty);
    }

    [TestMethod]
    public void UnknownSymbolCandidateIsUnresolved()
    {
        var r=Check(IrSymbol.Free("y","y"),N(1),1);
        Assert.AreEqual(SymbolicRootAuditStatus.Unknown,r.Status);
        Assert.AreEqual(R(1),r.UnresolvedCandidates.Single());
    }

    [TestMethod]
    public void WrongRealVariableDomainIsRejectedEarly()
    {
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Refused(SymbolicRootAuditStatus.UnsupportedDomain,
            SymbolicRootAudit.Verify(Square(X()),N(1),integer,[R(1)]));
    }

    [TestMethod]
    public void BoundTargetIsRejectedEarly()
    {
        var bound=IrSymbol.Bound("scope",0,"x");
        Refused(SymbolicRootAuditStatus.UnsupportedDomain,
            SymbolicRootAudit.Verify(Square(X()),N(1),bound,[R(1)]));
    }

    [TestMethod]
    public void DuplicateCandidatesAreRefusedRatherThanSilentlyDeduplicated() =>
        Refused(SymbolicRootAuditStatus.Unsupported,
            Check(Square(X()),N(1),1,1));

    [TestMethod]
    public void MoreThanEightCandidatesExceedsBudget() =>
        Refused(SymbolicRootAuditStatus.BudgetExceeded,
            Check(Square(X()),N(1),0,1,2,3,4,5,6,7,8));

    [TestMethod]
    public void OversizedCandidateIsRefusedByRationalBitBudget() =>
        Refused(SymbolicRootAuditStatus.BudgetExceeded,
            Check(Square(X()),N(0),new ExactRational(BigInteger.One<<140,1)));

    [TestMethod]
    public void OversizedIntermediateSquareIsRefusedByRationalBitBudget() =>
        Refused(SymbolicRootAuditStatus.BudgetExceeded,
            Check(Square(X()),N(0),new ExactRational(BigInteger.One<<80,1)));

    [TestMethod]
    public void ExactRationalRootVerificationNeverUsesBinary64()
    {
        var r=Check(Square(X()),Q(4,9),R(-2,3),R(2,3));
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(R(-2,3),r.VerifiedRoots[0].Value);
        Assert.AreEqual(R(2,3),r.VerifiedRoots[1].Value);
    }

    [TestMethod]
    public void ApproximateBinary64InputNeverClaimsExactRoot()
    {
        var r=Check(X(),IrFiniteBinary64Scalar.FromDouble(1),1);
        Assert.AreEqual(SymbolicRootAuditStatus.Unknown,r.Status);
        Assert.AreEqual(R(1),r.UnresolvedCandidates.Single());
        Assert.IsFalse(r.IsComplete);
    }

    [TestMethod]
    public void OriginalSourceHashesCoverBothSidesAndVariable()
    {
        var a=Square(X());var b=N(1);
        var r=Check(a,b,-1,1);
        Assert.IsTrue(r.IsComplete);
        Assert.AreEqual(SymbolicRootAudit.Rule,r.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(a),r.LeftSemanticSha256);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(b),r.RightSemanticSha256);
        Assert.AreEqual(X().Identity,r.VariableIdentity);
    }

    [TestMethod]
    public void VerifiedQuotientSourceHashDoesNotHideOriginalDomain()
    {
        var a=Quotient();var b=N(0);
        var r=Check(a,b,-1,1);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(a),r.LeftSemanticSha256);
        Assert.AreEqual(R(1),r.ExcludedFromDomain.Single());
        Assert.IsFalse(r.IsComplete);
    }

    [TestMethod]
    public void SupportedExactSubtractionCanValidateOriginalUnsimplifiedEquation()
    {
        var left=Minus(Square(X()),N(1));
        var r=Check(left,N(0),-1,1);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(2,r.VerifiedRoots.Length);
        Assert.IsTrue(r.VerifiedRoots.All(x=>x.Multiplicity is null));
    }

    [TestMethod]
    public void SupportedExactMultiplicationCanValidateOriginalUnsimplifiedEquation()
    {
        var left=Times(Minus(X(),N(1)),Minus(X(),N(2)));
        var r=Check(left,N(0),1,2);
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,r.Status);
        Assert.AreEqual(2,r.VerifiedRoots.Length);
    }

    [TestMethod]
    public void UnsupportedExponentNineNeverClaimsAValidatedRoot()
    {
        var expr=IrApply.Create(IrOperation.Power,[X(),N(9)]);
        var r=Check(expr,N(1),1);
        Assert.AreEqual(SymbolicRootAuditStatus.Unknown,r.Status);
        Assert.AreEqual(R(1),r.UnresolvedCandidates.Single());
    }

    [TestMethod]
    public void ZeroTimesUnqualifiedSqrtDoesNotSilentlyBecomeZero()
    {
        var sqrt=IrApply.Create(IrOperation.Sqrt,[X()]);
        var product=Times(N(0),sqrt);
        var r=Check(product,N(0),1);
        Assert.AreEqual(SymbolicRootAuditStatus.Unknown,r.Status);
        Assert.AreEqual(R(1),r.UnresolvedCandidates.Single());
    }

    [TestMethod]
    public void OriginalDenominatorZeroIsExcludedBeforeAnyCancellation()
    {
        var ratio=Divide(N(0),Minus(X(),N(1)));
        var r=Check(ratio,N(0),1);
        Assert.AreEqual(SymbolicRootAuditStatus.NoVerifiedCandidates,r.Status);
        Assert.AreEqual(R(1),r.ExcludedFromDomain.Single());
    }
}
