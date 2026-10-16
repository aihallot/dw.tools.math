using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;

namespace Dw.Tools.Math.Distribution;

/// <summary>No failed computation supplies a partial mathematical certificate.</summary>
public enum ExactVerticalBoundaryStatus
{
    Admitted, InvalidSource, Unsupported, ExcludedOriginalPoint,
    IncompatiblePipeline, UnauthorizedRounding, BudgetExceeded, EvidenceMismatch
}

public enum ExactVerticalBoundaryLossKind
{
    None, NonCanonicalInput, UnsupportedScientificContract,
    OriginalDomainExclusion, WrongRootEquationOrDerivative,
    DisallowedNumericLoss, ResourceLimit, UnverifiedScientificEvidence
}

/// <summary>
/// Source SHA identifies bounded bytes, not mathematical semantics.
/// A complete unrestricted candidate set is not a complete original root audit.
/// </summary>
public sealed record ExactVerticalBoundaryResult(
    ExactVerticalBoundaryStatus Status,
    ExactVerticalBoundaryLossKind LossKind,
    ExactVerticalResult? Vertical,
    string? SourceSha256,
    string? OriginalSemanticSha256,
    string? ProviderId,
    string? OriginalAssumption,
    string? NumericLoss,
    string? AuditRuleId,
    bool OriginalIrReplayed,
    bool FormalDerivativeSeparated,
    bool OriginalRootsAudited,
    bool UnrestrictedCandidatesComplete,
    bool OriginalRootAuditComplete,
    bool SourceFingerprintIsMathematicalProof)
{
    public bool IsAdmitted =>
        Status == ExactVerticalBoundaryStatus.Admitted &&
        LossKind == ExactVerticalBoundaryLossKind.None &&
        Vertical is { IsAdmitted: true } && SourceSha256 is not null &&
        OriginalSemanticSha256 is not null && ProviderId is not null &&
        OriginalAssumption is not null && NumericLoss is not null &&
        AuditRuleId is not null && OriginalIrReplayed &&
        FormalDerivativeSeparated && OriginalRootsAudited &&
        UnrestrictedCandidatesComplete && !SourceFingerprintIsMathematicalProof;
}

/// <summary>
/// Independent evidence replay over qualified engines, not a second
/// parser, CAS, derivative engine, root solver or evaluator.
/// </summary>
public static class ExactVerticalBoundaryAudit
{
    public const string Rule = "m5-exact-numeric-symbolic-vertical-boundary/1";
    public const int MaximumInputCharacters = CanonicalMathNotation.MaximumInputCharacters;

    public static ExactVerticalBoundaryResult Audit(
        string canonicalSource, ExactRational evaluationPoint,
        IrNode? candidateEquationSource = null,
        RationalBinary64Policy numericPolicy = RationalBinary64Policy.RequireExact)
    {
        ArgumentNullException.ThrowIfNull(canonicalSource);
        if (canonicalSource.Length > MaximumInputCharacters)
            return Refuse(ExactVerticalBoundaryStatus.BudgetExceeded,
                ExactVerticalBoundaryLossKind.ResourceLimit,null);

        var sourceSha=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonicalSource)))
            .ToLowerInvariant();
        var vertical=ExactMathVerticalRunner.Run(canonicalSource,evaluationPoint,
            candidateEquationSource,numericPolicy);
        if (!vertical.IsAdmitted)
        {
            var refusal=Map(vertical.Status);
            return Refuse(refusal.Status,refusal.Loss,sourceSha);
        }
        try
        {
            var parsed=CanonicalMathNotation.Parse(canonicalSource);
            if (!parsed.IsAdmitted ||
                parsed.Expression is not IrRestrictedExpression original ||
                vertical.Original is null ||
                !string.Equals(IrCanonicalJsonCodec.Encode(original),
                    IrCanonicalJsonCodec.Encode(vertical.Original),
                    StringComparison.Ordinal) ||
                !string.Equals(parsed.CanonicalText,vertical.CanonicalSource,
                    StringComparison.Ordinal) ||
                !string.Equals(parsed.Latex,vertical.SourceLatex,
                    StringComparison.Ordinal))
                return Unverified(sourceSha);

            var originalJson=IrCanonicalJsonCodec.Encode(original);
            var replay=IrCanonicalJsonCodec.Decode(originalJson);
            var originalHash=IrCanonicalHashes.SemanticStructuralSha256(replay);
            if (!string.Equals(IrCanonicalJsonCodec.Encode(replay),originalJson,
                    StringComparison.Ordinal) ||
                !string.Equals(originalJson,vertical.SourceCanonicalIrJson,
                    StringComparison.Ordinal) ||
                !string.Equals(originalHash,vertical.OriginalSemanticSha256,
                    StringComparison.Ordinal) ||
                !string.Equals(originalHash,parsed.SemanticStructuralSha256,
                    StringComparison.Ordinal))
                return Unverified(sourceSha);

            if (original.Assumptions.Conditions.Length!=1 ||
                original.Assumptions.Conditions[0].Kind!=IrRelationKind.NotEqual ||
                original.Expression is not IrApply { Operation: IrOperation.Subtract } body ||
                body.Arguments.Length!=2 ||
                body.Arguments[0] is not IrApply { Operation: IrOperation.Square } square ||
                square.Arguments.Length!=1 ||
                square.Arguments[0] is not IrSymbol variable ||
                body.Arguments[1] is not IrExactScalar constant)
                return Unverified(sourceSha);
            var restriction=original.Assumptions.Conditions[0];
            var expectedAssumption="ne("+variable.DisplayName+","+
                restriction.Right.Value.ToFractionString()+")";
            if (restriction.Symbol.Identity!=variable.Identity ||
                restriction.Symbol.Domain!=variable.Domain ||
                !string.Equals(vertical.OriginalAssumption,expectedAssumption,
                    StringComparison.Ordinal) ||
                evaluationPoint==restriction.Right.Value)
                return Unverified(sourceSha);

            // The derivative is ONLY formal differentiation of the BODY.
            var derivative=SymbolicExactDifferentiation.TryDifferentiate(
                original.Expression,variable);
            if (!derivative.IsAdmitted || derivative.Derivative is null ||
                vertical.FormalBodyDerivative is null ||
                !string.Equals(IrCanonicalJsonCodec.Encode(derivative.Derivative),
                    IrCanonicalJsonCodec.Encode(vertical.FormalBodyDerivative),
                    StringComparison.Ordinal) ||
                !string.Equals(derivative.ProofRule,vertical.FormalDerivativeRule,
                    StringComparison.Ordinal))
                return Unverified(sourceSha);
            var derivativeText=CanonicalMathNotation.Render(derivative.Derivative);
            if (!derivativeText.IsAdmitted ||
                !string.Equals(derivativeText.CanonicalText,
                    vertical.CanonicalDerivative,StringComparison.Ordinal))
                return Unverified(sourceSha);

            // Qualified polynomial continuity applies ONLY at admissible points.
            var point=SymbolicExactPolynomialCalculus.FiniteLimit(
                original.Expression,variable,evaluationPoint);
            if (!point.IsExact || point.Value!=vertical.ExactValueAtPoint ||
                !string.Equals(vertical.PointEvaluationRule,
                    ExactMathVerticalRunner.PointRule,StringComparison.Ordinal))
                return Unverified(sourceSha);

            var unrestricted=SymbolicExactRealSolving.Solve(square,
                IrRelationKind.Equal,constant,variable);
            var roots=SymbolicRootAudit.Verify(original,
                new IrExactScalar(ExactRational.Zero),variable,unrestricted.Roots);
            if (!unrestricted.IsComplete ||
                unrestricted.Status!=SymbolicRealSolvingStatus.Finite ||
                vertical.UnrestrictedCandidates is null ||
                !vertical.UnrestrictedCandidates.IsComplete ||
                unrestricted.Status!=vertical.UnrestrictedCandidates.Status ||
                !unrestricted.Roots.SequenceEqual(vertical.UnrestrictedCandidates.Roots) ||
                !roots.HasVerifiedRoots ||
                vertical.OriginalRootAudit is null ||
                roots.Status!=vertical.OriginalRootAudit.Status ||
                roots.IsComplete!=vertical.OriginalRootAudit.IsComplete ||
                !roots.VerifiedRoots.SequenceEqual(
                    vertical.OriginalRootAudit.VerifiedRoots) ||
                !roots.ExcludedFromDomain.SequenceEqual(
                    vertical.OriginalRootAudit.ExcludedFromDomain) ||
                !roots.RejectedCandidates.SequenceEqual(
                    vertical.OriginalRootAudit.RejectedCandidates) ||
                !roots.UnresolvedCandidates.SequenceEqual(
                    vertical.OriginalRootAudit.UnresolvedCandidates) ||
                roots.VerifiedRoots.Length+
                    roots.ExcludedFromDomain.Length!=unrestricted.Roots.Length ||
                !string.Equals(roots.RuleId,
                    vertical.OriginalRootAudit.RuleId,StringComparison.Ordinal))
                return Unverified(sourceSha);

            if (!point.Value.HasValue || vertical.Binary64Projection is null ||
                vertical.ExactComposition is not
                    { Status: ComputationStatus.Exact, Value: IrExactScalar exactValue } ||
                vertical.Binary64Composition is not
                    { Status: ComputationStatus.Approximate,
                      Value: IrFiniteBinary64Scalar approximateValue } ||
                exactValue.Value!=point.Value.Value)
                return Unverified(sourceSha);
            var converted=RationalBinary64Conversion.TryConvert(point.Value.Value);
            if (!converted.IsFiniteResult || !converted.Value.HasValue ||
                converted.Status!=vertical.Binary64Projection.Status ||
                converted.Value!=vertical.Binary64Projection.Value ||
                converted.AbsoluteError!=vertical.Binary64Projection.AbsoluteError ||
                BitConverter.DoubleToInt64Bits(approximateValue.Value)!=
                    BitConverter.DoubleToInt64Bits(converted.Value.Value) ||
                (numericPolicy==RationalBinary64Policy.RequireExact &&
                    converted.Status==RationalBinary64Status.Rounded))
                return Unverified(sourceSha);

            var numericLoss=converted.Status==RationalBinary64Status.Rounded
                ? "binary64-rounded-exact-error-retained-original-ir-unchanged"
                : "binary64-exact-representable-original-ir-unchanged";
            if (!string.Equals(vertical.LossClassification,numericLoss,
                    StringComparison.Ordinal) ||
                !string.Equals(vertical.ProviderId,ExactMathVerticalRunner.Provider,
                    StringComparison.Ordinal) ||
                !string.Equals(vertical.RuleId,ExactMathVerticalRunner.Rule,
                    StringComparison.Ordinal) ||
                !string.Equals(vertical.ExactComposition!.Provenance.OperationId,
                    "vertical.exact-point",StringComparison.Ordinal) ||
                !string.Equals(vertical.Binary64Composition!.Provenance.OperationId,
                    "vertical.explicit-binary64",StringComparison.Ordinal))
                return Unverified(sourceSha);

            return new(ExactVerticalBoundaryStatus.Admitted,
                ExactVerticalBoundaryLossKind.None,vertical,sourceSha,originalHash,
                vertical.ProviderId,expectedAssumption,numericLoss,Rule,
                OriginalIrReplayed:true,FormalDerivativeSeparated:true,
                OriginalRootsAudited:true,UnrestrictedCandidatesComplete:true,
                OriginalRootAuditComplete:roots.IsComplete,
                SourceFingerprintIsMathematicalProof:false);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or NotSupportedException or InvalidOperationException)
        {
            return Unverified(sourceSha);
        }
    }

    private static (ExactVerticalBoundaryStatus Status,
        ExactVerticalBoundaryLossKind Loss) Map(ExactVerticalStatus status) =>
        status switch
        {
            ExactVerticalStatus.InvalidSource =>
                (ExactVerticalBoundaryStatus.InvalidSource,
                    ExactVerticalBoundaryLossKind.NonCanonicalInput),
            ExactVerticalStatus.Unsupported =>
                (ExactVerticalBoundaryStatus.Unsupported,
                    ExactVerticalBoundaryLossKind.UnsupportedScientificContract),
            ExactVerticalStatus.ExcludedEvaluationPoint =>
                (ExactVerticalBoundaryStatus.ExcludedOriginalPoint,
                    ExactVerticalBoundaryLossKind.OriginalDomainExclusion),
            ExactVerticalStatus.IncompatiblePipeline =>
                (ExactVerticalBoundaryStatus.IncompatiblePipeline,
                    ExactVerticalBoundaryLossKind.WrongRootEquationOrDerivative),
            ExactVerticalStatus.RoundingNotAuthorized =>
                (ExactVerticalBoundaryStatus.UnauthorizedRounding,
                    ExactVerticalBoundaryLossKind.DisallowedNumericLoss),
            ExactVerticalStatus.BudgetExceeded =>
                (ExactVerticalBoundaryStatus.BudgetExceeded,
                    ExactVerticalBoundaryLossKind.ResourceLimit),
            _ => (ExactVerticalBoundaryStatus.EvidenceMismatch,
                    ExactVerticalBoundaryLossKind.UnverifiedScientificEvidence)
        };

    private static ExactVerticalBoundaryResult Unverified(string sourceSha) =>
        Refuse(ExactVerticalBoundaryStatus.EvidenceMismatch,
            ExactVerticalBoundaryLossKind.UnverifiedScientificEvidence,sourceSha);

    private static ExactVerticalBoundaryResult Refuse(
        ExactVerticalBoundaryStatus status,
        ExactVerticalBoundaryLossKind loss,string? sourceSha) =>
        new(status,loss,null,sourceSha,null,null,null,null,null,
            OriginalIrReplayed:false,FormalDerivativeSeparated:false,
            OriginalRootsAudited:false,UnrestrictedCandidatesComplete:false,
            OriginalRootAuditComplete:false,
            SourceFingerprintIsMathematicalProof:false);
}
