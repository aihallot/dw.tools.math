using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;

namespace Dw.Tools.Math.Distribution;

/// <summary>
/// Exact source -> typed IR -> formal body derivative -> admitted point value ->
/// candidate solving -> original-domain root audit -> explicit binary64 ->
/// typed composition and IR replay. Only x²-c with one x!=k restriction,
/// closed source grammar and independently qualified engines are admitted.
/// This is an ordinary .NET library: no installed AURA or external CAS.
/// </summary>
public static class ExactMathVerticalRunner
{
    public const string Rule = "m5-exact-numeric-symbolic-public-vertical/1";
    public const string Provider = "dw.tools.math/managed-qualified-engines";
    public const string PointRule =
        "real-polynomial-finite-limit/exact/v1+original-domain-admitted-continuity";

    /// <summary>
    /// A distinct candidateEquationSource is an incompatible pipeline: passing
    /// the formal derivative as the object to solve must never be accepted.
    /// Approximation requires explicit AllowRounded; the exact result stays exact.
    /// </summary>
    public static ExactVerticalResult Run(string canonicalSource,
        ExactRational evaluationPoint,
        IrNode? candidateEquationSource = null,
        RationalBinary64Policy numericPolicy = RationalBinary64Policy.RequireExact)
    {
        ArgumentNullException.ThrowIfNull(canonicalSource);
        if (!Enum.IsDefined(numericPolicy))
            return Refuse(ExactVerticalStatus.Unsupported);
        try
        {
            var parsed=CanonicalMathNotation.Parse(canonicalSource);
            if (parsed.Status==CanonicalNotationStatus.BudgetExceeded)
                return Refuse(ExactVerticalStatus.BudgetExceeded);
            if (!parsed.IsAdmitted ||
                parsed.Expression is not IrRestrictedExpression source ||
                !string.Equals(parsed.CanonicalText,canonicalSource,
                    StringComparison.Ordinal))
                return Refuse(ExactVerticalStatus.InvalidSource);

            if (source.Assumptions.Conditions.Length!=1)
                return Refuse(ExactVerticalStatus.Unsupported);
            var condition=source.Assumptions.Conditions[0];
            if (condition.Kind!=IrRelationKind.NotEqual ||
                condition.Symbol.IsBound ||
                condition.Symbol.Domain!=IrScalarDomain.Real ||
                source.Expression is not IrApply { Operation: IrOperation.Subtract } body ||
                body.Arguments.Length!=2 ||
                body.Arguments[0] is not IrApply { Operation: IrOperation.Square } squared ||
                squared.Arguments.Length!=1 ||
                squared.Arguments[0] is not IrSymbol variable ||
                variable.IsBound || variable.Domain!=IrScalarDomain.Real ||
                variable.Identity!=condition.Symbol.Identity ||
                variable.Domain!=condition.Symbol.Domain ||
                body.Arguments[1] is not IrExactScalar constant)
                return Refuse(ExactVerticalStatus.Unsupported);

            var originalJson=IrCanonicalJsonCodec.Encode(source);
            if (candidateEquationSource is not null &&
                !string.Equals(IrCanonicalJsonCodec.Encode(candidateEquationSource),
                    originalJson,StringComparison.Ordinal))
                return Refuse(ExactVerticalStatus.IncompatiblePipeline);
            if (evaluationPoint==condition.Right.Value)
                return Refuse(ExactVerticalStatus.ExcludedEvaluationPoint);

            // Deliberately differentiate the body, retaining the ORIGINAL
            // restriction separately: the existing derivative refuses a
            // restricted IR rather than silently discarding assumptions.
            var derivative=SymbolicExactDifferentiation.TryDifferentiate(
                source.Expression,variable);
            if (!derivative.IsAdmitted || derivative.Derivative is null ||
                derivative.ProofRule is null)
                return Refuse(ExactVerticalStatus.Unsupported);
            var derivativeText=CanonicalMathNotation.Render(derivative.Derivative);
            if (!derivativeText.IsAdmitted ||
                derivativeText.CanonicalText is null)
                return Refuse(ExactVerticalStatus.Unsupported);

            // The prior exact polynomial finite-point theorem gives the value
            // at any defined point by continuity. It does not evaluate at a hole.
            var value=SymbolicExactPolynomialCalculus.FiniteLimit(
                source.Expression,variable,evaluationPoint);
            if (!value.IsExact || !value.Value.HasValue ||
                value.RuleId!=SymbolicExactPolynomialCalculus.LimitRule)
                return Refuse(value.Status==SymbolicPolynomialCalculusStatus.BudgetExceeded
                    ? ExactVerticalStatus.BudgetExceeded : ExactVerticalStatus.Unsupported);

            // Solve x²=c in the previously certified exact elementary grammar.
            // Recheck every candidate against ORIGINAL x²-c with x!=k;
            // original RootAudit.IsComplete may remain false and MUST NOT
            // inherit the unrestricted candidate-set completeness claim.
            var candidates=SymbolicExactRealSolving.Solve(squared,
                IrRelationKind.Equal,constant,variable);
            if (!candidates.IsComplete ||
                candidates.Status!=SymbolicRealSolvingStatus.Finite ||
                candidates.Roots.Length==0)
                return Refuse(ExactVerticalStatus.Unsupported);
            var rootAudit=SymbolicRootAudit.Verify(source,
                new IrExactScalar(ExactRational.Zero),variable,candidates.Roots);
            if (!rootAudit.HasVerifiedRoots ||
                rootAudit.UnresolvedCandidates.Length!=0 ||
                rootAudit.RejectedCandidates.Length!=0 ||
                rootAudit.VerifiedRoots.Length+
                    rootAudit.ExcludedFromDomain.Length!=candidates.Roots.Length)
                return Refuse(ExactVerticalStatus.Unsupported);

            var binary=RationalBinary64Conversion.TryConvert(value.Value.Value);
            if (!binary.IsFiniteResult || !binary.Value.HasValue ||
                !binary.AbsoluteError.HasValue)
                return Refuse(binary.Status==RationalBinary64Status.BudgetExceeded
                    ? ExactVerticalStatus.BudgetExceeded : ExactVerticalStatus.Unsupported);
            if (binary.Status==RationalBinary64Status.Rounded &&
                numericPolicy==RationalBinary64Policy.RequireExact)
                return Refuse(ExactVerticalStatus.RoundingNotAuthorized);

            var sourceReplay=IrCanonicalJsonCodec.Decode(originalJson);
            if (!string.Equals(IrCanonicalJsonCodec.Encode(sourceReplay),
                    originalJson,StringComparison.Ordinal) ||
                !string.Equals(
                    IrCanonicalHashes.SemanticStructuralSha256(sourceReplay),
                    parsed.SemanticStructuralSha256,StringComparison.Ordinal))
                return Refuse(ExactVerticalStatus.IncompatiblePipeline);

            var exactComposition=MathOperationResult.Exact(
                new IrExactScalar(value.Value.Value),
                MathResultProvenance.Create("vertical.exact-point","1"));
            var binaryComposition=MathOperationResult.Approximate(
                IrFiniteBinary64Scalar.FromDouble(binary.Value.Value),
                MathResultProvenance.Create("vertical.explicit-binary64","1"));
            return new ExactVerticalResult
            {
                Status=ExactVerticalStatus.Exact,
                Original=source,
                FormalBodyDerivative=derivative.Derivative,
                OriginalRootAudit=rootAudit,
                UnrestrictedCandidates=candidates,
                ExactValueAtPoint=value.Value,
                Binary64Projection=binary,
                ExactComposition=exactComposition,
                Binary64Composition=binaryComposition,
                CanonicalSource=parsed.CanonicalText,
                CanonicalDerivative=derivativeText.CanonicalText,
                SourceLatex=parsed.Latex,
                SourceCanonicalIrJson=originalJson,
                OriginalSemanticSha256=parsed.SemanticStructuralSha256,
                FormalDerivativeRule=derivative.ProofRule,
                PointEvaluationRule=PointRule,
                ProviderId=Provider,
                OriginalAssumption="ne("+variable.DisplayName+","+
                    condition.Right.Value.ToFractionString()+")",
                LossClassification=binary.Status==RationalBinary64Status.Rounded
                    ? "binary64-rounded-exact-error-retained-original-ir-unchanged"
                    : "binary64-exact-representable-original-ir-unchanged",
                RuleId=Rule
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(ExactVerticalStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or NotSupportedException or InvalidOperationException)
        {
            return Refuse(ExactVerticalStatus.Unsupported);
        }
    }

    private static ExactVerticalResult Refuse(ExactVerticalStatus status) =>
        new() { Status=status };
}
