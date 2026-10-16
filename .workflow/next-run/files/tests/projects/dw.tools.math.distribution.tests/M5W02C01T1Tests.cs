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
public sealed class M5W02C01T1Tests
{
    private const string Example="where(sub(square(x),1),ne(x,1))";
    private static ExactRational R(long numerator,long denominator=1)=>
        new(numerator,denominator);
    private static ExactVerticalResult Accepted(
        ExactRational? at=null,RationalBinary64Policy policy=RationalBinary64Policy.RequireExact)
    {
        var output=ExactMathVerticalRunner.Run(Example,at??R(2),numericPolicy:policy);
        Assert.AreEqual(ExactVerticalStatus.Exact,output.Status);
        Assert.IsTrue(output.IsAdmitted);
        return output;
    }
    private static ExactVerticalResult Rejected(string source,
        ExactVerticalStatus status,ExactRational? at=null,
        IrNode? supplied=null,RationalBinary64Policy policy=RationalBinary64Policy.RequireExact)
    {
        var result=ExactMathVerticalRunner.Run(source,at??R(2),supplied,policy);
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Original);
        Assert.IsNull(result.FormalBodyDerivative);
        Assert.IsNull(result.OriginalRootAudit);
        Assert.IsNull(result.UnrestrictedCandidates);
        Assert.IsNull(result.ExactValueAtPoint);
        Assert.IsNull(result.Binary64Projection);
        Assert.IsNull(result.ExactComposition);
        Assert.IsNull(result.Binary64Composition);
        Assert.IsNull(result.CanonicalSource);
        Assert.IsNull(result.SourceCanonicalIrJson);
        Assert.IsNull(result.RuleId);
        return result;
    }

    [TestMethod] public void CanonicalSourceIsParsedAndValidated()
    {
        var result=Accepted();
        Assert.AreEqual(Example,result.CanonicalSource);
        Assert.IsInstanceOfType<IrRestrictedExpression>(result.Original);
    }
    [TestMethod] public void SourceRoundTripReplaysExactlyTheOriginalIr()
    {
        var result=Accepted();
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(result.Original!),
            result.SourceCanonicalIrJson);
        var replay=IrCanonicalJsonCodec.Decode(result.SourceCanonicalIrJson!);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(result.Original!),IrCanonicalJsonCodec.Encode(replay));
    }
    [TestMethod] public void OriginalStructuralHashIsStableAcrossReplay()
    {
        var result=Accepted();
        var replay=IrCanonicalJsonCodec.Decode(result.SourceCanonicalIrJson!);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(replay),
            result.OriginalSemanticSha256);
    }
    [TestMethod] public void OriginalNotEqualOneRestrictionIsKept()
    {
        var result=Accepted();
        var condition=result.Original!.Assumptions.Conditions.Single();
        Assert.AreEqual(IrRelationKind.NotEqual,condition.Kind);
        Assert.AreEqual(R(1),condition.Right.Value);
        Assert.AreEqual("ne(x,1)",result.OriginalAssumption);
    }
    [TestMethod] public void FormalDerivativeOfSourceBodyIsTwoTimesX()
    {
        var result=Accepted();
        Assert.AreEqual("mul(2,x)",result.CanonicalDerivative);
        Assert.AreEqual(IrOperation.Multiply,((IrApply)result.FormalBodyDerivative!).Operation);
    }
    [TestMethod] public void OriginalRestrictionIsNotErasedByFormalDerivative()
    {
        var result=Accepted();
        Assert.AreEqual(IrRelationKind.NotEqual,result.Original!.Assumptions.Conditions.Single().Kind);
        Assert.AreEqual(SymbolicExactDifferentiation.Rule,result.FormalDerivativeRule);
    }
    [TestMethod] public void ExactEvaluationAtTwoIsThree()
    {
        var result=Accepted();
        Assert.AreEqual(R(3),result.ExactValueAtPoint!.Value);
        Assert.AreEqual(ExactMathVerticalRunner.PointRule,result.PointEvaluationRule);
    }
    [TestMethod] public void ExactEvaluationAtMinusOneIsZero()
    {
        var result=Accepted(R(-1));
        Assert.AreEqual(R(0),result.ExactValueAtPoint!.Value);
    }
    [TestMethod] public void UnrestrictedQuadraticCandidateSolverIsComplete()
    {
        var result=Accepted();
        Assert.AreEqual(SymbolicRealSolvingStatus.Finite,result.UnrestrictedCandidates!.Status);
        Assert.IsTrue(result.UnrestrictedCandidates.IsComplete);
        CollectionAssert.AreEqual(new[]{R(-1),R(1)},result.UnrestrictedCandidates.Roots.ToArray());
    }
    [TestMethod] public void OriginalDomainAuditAdmitsOnlyMinusOne()
    {
        var result=Accepted();
        Assert.AreEqual(R(-1),result.OriginalRootAudit!.VerifiedRoots.Single().Value);
        Assert.AreEqual(R(1),result.OriginalRootAudit.ExcludedFromDomain.Single());
    }
    [TestMethod] public void OriginalRootAuditDoesNotClaimGlobalCompleteness()
    {
        var result=Accepted();
        Assert.AreEqual(SymbolicRootAuditStatus.VerifiedSubset,result.OriginalRootAudit!.Status);
        Assert.IsFalse(result.OriginalRootAudit.IsComplete);
    }
    [TestMethod] public void NoCandidateOfOriginalDomainIsInvented()
    {
        var result=Accepted();
        Assert.AreEqual(0,result.OriginalRootAudit!.UnresolvedCandidates.Length);
        Assert.AreEqual(0,result.OriginalRootAudit.RejectedCandidates.Length);
    }
    [TestMethod] public void AForbiddenEvaluationPointIsRefused()
    {
        Rejected(Example,ExactVerticalStatus.ExcludedEvaluationPoint,R(1));
    }
    [TestMethod] public void DerivativePassedToRootStageIsAnIncompatiblePipeline()
    {
        var derivative=Accepted().FormalBodyDerivative;
        Rejected(Example,ExactVerticalStatus.IncompatiblePipeline,R(2),derivative);
    }
    [TestMethod] public void DifferentOriginalEquationCannotReplaceSourceIr()
    {
        var foreign=CanonicalMathNotation.Parse("where(sub(square(x),4),ne(x,1))");
        Assert.IsTrue(foreign.IsAdmitted);
        Rejected(Example,ExactVerticalStatus.IncompatiblePipeline,R(2),foreign.Expression);
    }
    [TestMethod] public void ExplicitBinary64ThreeIsExactlyRepresentable()
    {
        var result=Accepted();
        Assert.AreEqual(RationalBinary64Status.Exact,result.Binary64Projection!.Status);
        Assert.AreEqual(3d,result.Binary64Projection.Value!.Value);
        Assert.AreEqual(R(0),result.Binary64Projection.AbsoluteError!.Value);
    }
    [TestMethod] public void CompositionMaintainsExactVsBinary64PrecisionTypes()
    {
        var result=Accepted();
        Assert.AreEqual(ComputationStatus.Exact,result.ExactComposition!.Status);
        Assert.AreEqual(ComputationStatus.Approximate,result.Binary64Composition!.Status);
        Assert.AreEqual(R(3),((IrExactScalar)result.ExactComposition.Value!).Value);
        Assert.AreEqual(3d,((IrFiniteBinary64Scalar)result.Binary64Composition.Value!).Value);
    }
    [TestMethod] public void ProviderAndLossAreExplicitNotImplicit()
    {
        var result=Accepted();
        Assert.AreEqual(ExactMathVerticalRunner.Provider,result.ProviderId);
        Assert.AreEqual("binary64-exact-representable-original-ir-unchanged",
            result.LossClassification);
        Assert.AreEqual(ExactMathVerticalRunner.Rule,result.RuleId);
    }
    [TestMethod] public void CanonicalSourceRendersLatexWithoutTeXExecution()
    {
        var result=Accepted();
        Assert.IsTrue(result.SourceLatex!.Contains("\\ne",StringComparison.Ordinal));
    }
    [TestMethod] public void NoncanonicalInfixSyntaxIsRejected()
    {
        Rejected("x^2-1",ExactVerticalStatus.InvalidSource);
    }
    [TestMethod] public void UnsupportedFreeVariableIsRejected()
    {
        Rejected("where(sub(square(q),1),ne(q,1))",ExactVerticalStatus.InvalidSource);
    }
    [TestMethod] public void UnsupportedDifferentSymbolsAreNotComposed()
    {
        Rejected("where(sub(square(y),1),ne(x,1))",ExactVerticalStatus.Unsupported);
    }
    [TestMethod] public void UnqualifiedMultiplicationShapeIsNotRewritten()
    {
        Rejected("where(sub(mul(x,x),1),ne(x,1))",ExactVerticalStatus.Unsupported);
    }
    [TestMethod] public void EqualityAssumptionCannotBeReadAsHole()
    {
        Rejected("where(sub(square(x),1),eq(x,1))",ExactVerticalStatus.Unsupported);
    }
    [TestMethod] public void IrrationalQuadraticRootsAreNotDeclaredAbsent()
    {
        Rejected("where(sub(square(x),2),ne(x,1))",ExactVerticalStatus.Unsupported);
    }
    [TestMethod] public void RoundingRequiresExplicitConsent()
    {
        Rejected(Example,ExactVerticalStatus.RoundingNotAuthorized,R(1,3));
    }
    [TestMethod] public void ExplicitRoundedBinary64RetainsErrorAndExactValue()
    {
        var result=Accepted(R(1,3),RationalBinary64Policy.AllowRounded);
        Assert.AreEqual(R(-8,9),result.ExactValueAtPoint!.Value);
        Assert.AreEqual(RationalBinary64Status.Rounded,result.Binary64Projection!.Status);
        Assert.IsTrue(result.Binary64Projection.AbsoluteError!.Value > R(0));
    }
    [TestMethod] public void RoundedCompositionDoesNotReclassifyExactSource()
    {
        var result=Accepted(R(1,3),RationalBinary64Policy.AllowRounded);
        Assert.AreEqual(ComputationStatus.Exact,result.ExactComposition!.Status);
        Assert.AreEqual(ComputationStatus.Approximate,result.Binary64Composition!.Status);
        Assert.AreEqual("binary64-rounded-exact-error-retained-original-ir-unchanged",
            result.LossClassification);
    }
    [TestMethod] public void ExplicitNumericalPolicyMustBeDeclared()
    {
        Rejected(Example,ExactVerticalStatus.Unsupported,R(2),policy:(RationalBinary64Policy)99);
    }
    [TestMethod] public void RestrictedQuantityInputCannotBecomeAnExactPolynomial()
    {
        Rejected("where(sub(square(x),unit(1,si.metre,None)),ne(x,1))",
            ExactVerticalStatus.Unsupported);
    }
    [TestMethod] public void OverBudgetSourceIsExplicitlyRefused()
    {
        Rejected(new string('x',CanonicalMathNotation.MaximumInputCharacters+1),
            ExactVerticalStatus.BudgetExceeded);
    }
    [TestMethod] public void SourceWithoutRestrictionIsNotAnAdmittedVertical()
    {
        Rejected("sub(square(x),1)",ExactVerticalStatus.InvalidSource);
    }
    [TestMethod] public void OriginalSourceCannotBeSilentlyReplacedWithDerivative()
    {
        var source=CanonicalMathNotation.Parse(Example);
        Assert.IsTrue(source.IsAdmitted);
        var rejected=Rejected("where(sub(square(x),4),ne(x,1))",
            ExactVerticalStatus.IncompatiblePipeline,R(2),source.Expression);
        Assert.IsNull(rejected.FormalBodyDerivative);
    }
    [TestMethod] public void NoAgentOrExternalProviderAssemblyIsRequired()
    {
        var result=Accepted();
        var assemblies=new[]{typeof(ExactMathVerticalRunner).Assembly,
            typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
        var references=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
            .Select(a=>a.Name??string.Empty).ToArray();
        Assert.IsFalse(references.Any(n=>
            new[]{"Aura","Decision","MCDM","MathNet","Angouri"}
                .Any(mark=>n.Contains(mark,StringComparison.OrdinalIgnoreCase))));
        Assert.IsTrue(result.IsAdmitted);
    }
}
