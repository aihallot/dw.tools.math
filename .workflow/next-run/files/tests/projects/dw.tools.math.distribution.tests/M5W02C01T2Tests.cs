using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Distribution;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C01T2Tests
{
    private const string Example="where(sub(square(x),1),ne(x,1))";
    private static ExactRational R(long n,long d=1)=>new(n,d);
    private static string Sha(string text)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)))
            .ToLowerInvariant();

    private static ExactVerticalBoundaryResult Admit(
        string source=Example,ExactRational? point=null,
        RationalBinary64Policy policy=RationalBinary64Policy.RequireExact)
    {
        var audited=ExactVerticalBoundaryAudit.Audit(source,point??R(2),
            numericPolicy:policy);
        Assert.AreEqual(ExactVerticalBoundaryStatus.Admitted,audited.Status);
        Assert.AreEqual(ExactVerticalBoundaryLossKind.None,audited.LossKind);
        Assert.IsTrue(audited.IsAdmitted);
        Assert.IsNotNull(audited.Vertical);
        Assert.IsTrue(audited.Vertical.IsAdmitted);
        Assert.AreEqual(ExactVerticalBoundaryAudit.Rule,audited.AuditRuleId);
        Assert.AreEqual(Sha(source),audited.SourceSha256);
        Assert.AreEqual(audited.Vertical.OriginalSemanticSha256,
            audited.OriginalSemanticSha256);
        Assert.AreEqual(ExactMathVerticalRunner.Provider,audited.ProviderId);
        Assert.IsTrue(audited.OriginalIrReplayed);
        Assert.IsTrue(audited.FormalDerivativeSeparated);
        Assert.IsTrue(audited.OriginalRootsAudited);
        Assert.IsTrue(audited.UnrestrictedCandidatesComplete);
        Assert.IsFalse(audited.SourceFingerprintIsMathematicalProof);
        return audited;
    }

    private static ExactVerticalBoundaryResult Refuse(
        string source,ExactVerticalBoundaryStatus status,
        ExactVerticalBoundaryLossKind loss,ExactRational? point=null,
        IrNode? candidateSource=null,
        RationalBinary64Policy policy=RationalBinary64Policy.RequireExact)
    {
        var audited=ExactVerticalBoundaryAudit.Audit(source,point??R(2),
            candidateSource,policy);
        Assert.AreEqual(status,audited.Status);
        Assert.AreEqual(loss,audited.LossKind);
        Assert.IsFalse(audited.IsAdmitted);
        Assert.IsNull(audited.Vertical);
        Assert.IsNull(audited.OriginalSemanticSha256);
        Assert.IsNull(audited.ProviderId);
        Assert.IsNull(audited.OriginalAssumption);
        Assert.IsNull(audited.NumericLoss);
        Assert.IsNull(audited.AuditRuleId);
        Assert.IsFalse(audited.OriginalIrReplayed);
        Assert.IsFalse(audited.FormalDerivativeSeparated);
        Assert.IsFalse(audited.OriginalRootsAudited);
        Assert.IsFalse(audited.UnrestrictedCandidatesComplete);
        Assert.IsFalse(audited.OriginalRootAuditComplete);
        Assert.IsFalse(audited.SourceFingerprintIsMathematicalProof);
        Assert.AreEqual(source.Length>ExactVerticalBoundaryAudit.MaximumInputCharacters
            ? null:Sha(source),audited.SourceSha256);
        return audited;
    }

    [TestMethod] public void OriginalCanonicalIrRoundTripsExactly()
    {
        var audit=Admit();
        Assert.AreEqual(Example,audit.Vertical!.CanonicalSource);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(audit.Vertical.Original!),
            audit.Vertical.SourceCanonicalIrJson);
    }
    [TestMethod] public void SemanticDigestReplaysOriginalRestrictedIr()
    {
        var audit=Admit();
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(
            audit.Vertical!.Original!),audit.OriginalSemanticSha256);
    }
    [TestMethod] public void InputFingerprintIsNotMathematicalStructuralDigest()
    {
        var audit=Admit();
        Assert.AreNotEqual(audit.SourceSha256,audit.OriginalSemanticSha256);
    }
    [TestMethod] public void OriginalNotEqualRestrictionRemainsTyped()
    {
        var audit=Admit();
        Assert.AreEqual("ne(x,1)",audit.OriginalAssumption);
        Assert.AreEqual(IrRelationKind.NotEqual,
            audit.Vertical!.Original!.Assumptions.Conditions.Single().Kind);
    }
    [TestMethod] public void FormalDerivativeRemainsTwoX()
    {
        var audit=Admit();
        Assert.AreEqual("mul(2,x)",audit.Vertical!.CanonicalDerivative);
        Assert.AreEqual(SymbolicExactDifferentiation.Rule,
            audit.Vertical.FormalDerivativeRule);
    }
    [TestMethod] public void FormalDerivativeCannotBeOriginalEquation()
    {
        var audit=Admit();
        Assert.AreNotEqual(IrCanonicalJsonCodec.Encode(
            audit.Vertical!.FormalBodyDerivative!),audit.Vertical.SourceCanonicalIrJson);
        Assert.IsTrue(audit.FormalDerivativeSeparated);
    }
    [TestMethod] public void AtTwoExactPolynomialValueIsThree()
    {
        var audit=Admit();
        Assert.AreEqual(R(3),audit.Vertical!.ExactValueAtPoint!.Value);
        Assert.AreEqual(ExactMathVerticalRunner.PointRule,audit.Vertical.PointEvaluationRule);
    }
    [TestMethod] public void OriginalRootMinusOneIsProven()
    {
        var audit=Admit();
        Assert.AreEqual(R(-1),
            audit.Vertical!.OriginalRootAudit!.VerifiedRoots.Single().Value);
    }
    [TestMethod] public void OriginalCandidatePositiveOneIsExcluded()
    {
        var audit=Admit();
        Assert.AreEqual(R(1),audit.Vertical!.OriginalRootAudit!.ExcludedFromDomain.Single());
    }
    [TestMethod] public void CompleteCandidatesAreNotCompleteOriginalRootAudit()
    {
        var audit=Admit();
        Assert.IsTrue(audit.Vertical!.UnrestrictedCandidates!.IsComplete);
        Assert.IsFalse(audit.Vertical.OriginalRootAudit!.IsComplete);
        Assert.IsFalse(audit.OriginalRootAuditComplete);
    }
    [TestMethod] public void OriginalCandidateAuditHasNoUnresolvedPoints()
    {
        var audit=Admit();
        Assert.AreEqual(0,audit.Vertical!.OriginalRootAudit!.UnresolvedCandidates.Length);
    }
    [TestMethod] public void ExactlyRepresentableThreeHasNoNumericError()
    {
        var audit=Admit();
        Assert.AreEqual(RationalBinary64Status.Exact,
            audit.Vertical!.Binary64Projection!.Status);
        Assert.AreEqual(R(0),audit.Vertical.Binary64Projection.AbsoluteError!.Value);
        Assert.AreEqual("binary64-exact-representable-original-ir-unchanged",
            audit.NumericLoss);
    }
    [TestMethod] public void ExactAndBinary64CompositionAreTypedSeparately()
    {
        var audit=Admit();
        Assert.AreEqual(ComputationStatus.Exact,
            audit.Vertical!.ExactComposition!.Status);
        Assert.AreEqual(ComputationStatus.Approximate,
            audit.Vertical.Binary64Composition!.Status);
    }
    [TestMethod] public void ProviderAndVersionedProofRemainVisible()
    {
        var audit=Admit();
        Assert.AreEqual(ExactMathVerticalRunner.Provider,audit.ProviderId);
        Assert.AreEqual(ExactMathVerticalRunner.Rule,audit.Vertical!.RuleId);
    }
    [TestMethod] public void NegativeOneIsAdmissibleEvaluation()
    {
        var audit=Admit(point:R(-1));
        Assert.AreEqual(R(0),audit.Vertical!.ExactValueAtPoint!.Value);
    }
    [TestMethod] public void ZeroIsAdmissibleWithNegativeOneResult()
    {
        var audit=Admit(point:R(0));
        Assert.AreEqual(R(-1),audit.Vertical!.ExactValueAtPoint!.Value);
    }
    [TestMethod] public void RoundingWithoutConsentIsRefused()
    {
        Refuse(Example,ExactVerticalBoundaryStatus.UnauthorizedRounding,
            ExactVerticalBoundaryLossKind.DisallowedNumericLoss,point:R(1,3));
    }
    [TestMethod] public void ConsentedRoundingKeepsExactMinusEightNinths()
    {
        var audit=Admit(point:R(1,3),policy:RationalBinary64Policy.AllowRounded);
        Assert.AreEqual(R(-8,9),audit.Vertical!.ExactValueAtPoint!.Value);
        Assert.AreEqual(RationalBinary64Status.Rounded,
            audit.Vertical.Binary64Projection!.Status);
    }
    [TestMethod] public void NumericRoundingRetainsPositiveExactError()
    {
        var audit=Admit(point:R(1,3),policy:RationalBinary64Policy.AllowRounded);
        Assert.IsTrue(audit.Vertical!.Binary64Projection!.AbsoluteError!.Value>R(0));
        Assert.AreEqual("binary64-rounded-exact-error-retained-original-ir-unchanged",
            audit.NumericLoss);
    }
    [TestMethod] public void RoundedBinary64DoesNotReplaceExactComposition()
    {
        var audit=Admit(point:R(1,3),policy:RationalBinary64Policy.AllowRounded);
        Assert.AreEqual(ComputationStatus.Exact,audit.Vertical!.ExactComposition!.Status);
        Assert.AreEqual(ComputationStatus.Approximate,
            audit.Vertical.Binary64Composition!.Status);
    }
    [TestMethod] public void ExcludedOriginalEvaluationPointIsRefused()
    {
        Refuse(Example,ExactVerticalBoundaryStatus.ExcludedOriginalPoint,
            ExactVerticalBoundaryLossKind.OriginalDomainExclusion,point:R(1));
    }
    [TestMethod] public void DerivativeAsEquationIsIncompatible()
    {
        var derivative=Admit().Vertical!.FormalBodyDerivative;
        Refuse(Example,ExactVerticalBoundaryStatus.IncompatiblePipeline,
            ExactVerticalBoundaryLossKind.WrongRootEquationOrDerivative,
            candidateSource:derivative);
    }
    [TestMethod] public void ForeignOriginalEquationCannotReplaceSource()
    {
        var foreign=CanonicalMathNotation.Parse("where(sub(square(x),4),ne(x,1))");
        Assert.IsTrue(foreign.IsAdmitted);
        Refuse(Example,ExactVerticalBoundaryStatus.IncompatiblePipeline,
            ExactVerticalBoundaryLossKind.WrongRootEquationOrDerivative,
            candidateSource:foreign.Expression);
    }
    [TestMethod] public void IdenticalOriginalIrCandidateSourceIsAdmitted()
    {
        var parsed=CanonicalMathNotation.Parse(Example);
        Assert.IsTrue(parsed.IsAdmitted);
        var audit=ExactVerticalBoundaryAudit.Audit(Example,R(2),parsed.Expression);
        Assert.IsTrue(audit.IsAdmitted);
        Assert.AreEqual(Example,audit.Vertical!.CanonicalSource);
    }
    [TestMethod] public void InfixStringIsNotCanonicalMath()
    {
        Refuse("x^2-1",ExactVerticalBoundaryStatus.InvalidSource,
            ExactVerticalBoundaryLossKind.NonCanonicalInput);
    }
    [TestMethod] public void EmptySourceNeverQualifiesMath()
    {
        Refuse("",ExactVerticalBoundaryStatus.InvalidSource,
            ExactVerticalBoundaryLossKind.NonCanonicalInput);
    }
    [TestMethod] public void UnreducedRationalAliasIsNotCanonical()
    {
        Refuse("where(sub(square(x),2/4),ne(x,1))",
            ExactVerticalBoundaryStatus.InvalidSource,
            ExactVerticalBoundaryLossKind.NonCanonicalInput);
    }
    [TestMethod] public void UnknownFreeVariableCannotBeInvented()
    {
        Refuse("where(sub(square(q),1),ne(q,1))",
            ExactVerticalBoundaryStatus.InvalidSource,
            ExactVerticalBoundaryLossKind.NonCanonicalInput);
    }
    [TestMethod] public void MismatchedOriginalSymbolAndRestrictionAreUnsupported()
    {
        Refuse("where(sub(square(y),1),ne(x,1))",
            ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract);
    }
    [TestMethod] public void MultiplicationIsNotRewrittenToSquare()
    {
        Refuse("where(sub(mul(x,x),1),ne(x,1))",
            ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract);
    }
    [TestMethod] public void EqualityAssumptionIsNotHoleExclusion()
    {
        Refuse("where(sub(square(x),1),eq(x,1))",
            ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract);
    }
    [TestMethod] public void UnprovenIrrationalRootFamilyCannotBeClaimedEmpty()
    {
        Refuse("where(sub(square(x),2),ne(x,1))",
            ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract);
    }
    [TestMethod] public void DimensionalLiteralIsNotSilentlyScalar()
    {
        Refuse("where(sub(square(x),unit(1,si.metre,None)),ne(x,1))",
            ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract);
    }
    [TestMethod] public void InvalidNumericalPolicyCannotBeAccepted()
    {
        Refuse(Example,ExactVerticalBoundaryStatus.Unsupported,
            ExactVerticalBoundaryLossKind.UnsupportedScientificContract,
            policy:(RationalBinary64Policy)99);
    }
    [TestMethod] public void OversizedSourceHasNoFingerprintAllocation()
    {
        var large=new string('x',ExactVerticalBoundaryAudit.MaximumInputCharacters+1);
        var audit=Refuse(large,ExactVerticalBoundaryStatus.BudgetExceeded,
            ExactVerticalBoundaryLossKind.ResourceLimit);
        Assert.IsNull(audit.SourceSha256);
    }
    [TestMethod] public void AlternativeSquareFourQualifiesBothOriginalRoots()
    {
        var audit=Admit("where(sub(square(x),4),ne(x,1))",R(3));
        Assert.AreEqual(R(5),audit.Vertical!.ExactValueAtPoint!.Value);
        CollectionAssert.AreEquivalent(new[]{R(-2),R(2)},
            audit.Vertical.OriginalRootAudit!.VerifiedRoots.Select(root=>root.Value).ToArray());
        Assert.AreEqual(0,audit.Vertical.OriginalRootAudit.ExcludedFromDomain.Length);
    }
    [TestMethod] public void AlternativeExclusionMinusOneRetainsPositiveRoot()
    {
        var audit=Admit("where(sub(square(x),1),ne(x,-1))");
        Assert.AreEqual(R(1),
            audit.Vertical!.OriginalRootAudit!.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(-1),audit.Vertical.OriginalRootAudit.ExcludedFromDomain.Single());
    }
    [TestMethod] public void NonrootExclusionStillAdmitsBothRationalRoots()
    {
        var audit=Admit("where(sub(square(x),1),ne(x,3))");
        Assert.AreEqual(2,audit.Vertical!.OriginalRootAudit!.VerifiedRoots.Length);
        Assert.AreEqual(0,audit.Vertical.OriginalRootAudit.ExcludedFromDomain.Length);
    }
    [TestMethod] public void ChangingOriginalAssumptionsChangesSemanticFingerprint()
    {
        var baseline=Admit();
        var alternate=Admit("where(sub(square(x),1),ne(x,3))");
        Assert.AreNotEqual(baseline.SourceSha256,alternate.SourceSha256);
        Assert.AreNotEqual(baseline.OriginalSemanticSha256,alternate.OriginalSemanticSha256);
    }
    [TestMethod] public void RefusedFingerprintCannotImplySemanticProof()
    {
        var audit=Refuse("x^2-1",ExactVerticalBoundaryStatus.InvalidSource,
            ExactVerticalBoundaryLossKind.NonCanonicalInput);
        Assert.IsNotNull(audit.SourceSha256);
        Assert.IsFalse(audit.SourceFingerprintIsMathematicalProof);
    }
    [TestMethod] public void MathOnlyAssembliesNeedNoAgentOrExternalCas()
    {
        var audit=Admit();
        var dependencies=new[]{typeof(ExactVerticalBoundaryAudit).Assembly,
            typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
            .SelectMany(a=>a.GetReferencedAssemblies())
            .Select(a=>a.Name??string.Empty);
        Assert.IsFalse(dependencies.Any(n=>
            new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
                .Any(token=>n.Contains(token,StringComparison.OrdinalIgnoreCase))));
        Assert.IsTrue(audit.IsAdmitted);
    }
}
