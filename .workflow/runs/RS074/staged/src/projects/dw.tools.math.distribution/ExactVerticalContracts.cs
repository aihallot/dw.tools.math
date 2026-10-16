using dw.quantities;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;

namespace Dw.Tools.Math.Distribution;

/// <summary>One bounded public exact vertical or a typed non-admission.</summary>
public enum ExactVerticalStatus
{
    Exact,
    InvalidSource,
    Unsupported,
    ExcludedEvaluationPoint,
    IncompatiblePipeline,
    RoundingNotAuthorized,
    BudgetExceeded
}

/// <summary>
/// Source restrictions, independent symbolic proofs and numerical conversion
/// are never collapsed into a single opaque mathematical claim. A refusal
/// carries no final computed value, derivative, candidate proof or replay.
/// </summary>
public sealed record ExactVerticalResult
{
    public ExactVerticalStatus Status { get; init; }
    public IrRestrictedExpression? Original { get; init; }
    public IrNode? FormalBodyDerivative { get; init; }
    public SymbolicRootAuditResult? OriginalRootAudit { get; init; }
    public SymbolicRealSolvingResult? UnrestrictedCandidates { get; init; }
    public ExactRational? ExactValueAtPoint { get; init; }
    public RationalBinary64Result? Binary64Projection { get; init; }
    public MathOperationResult? ExactComposition { get; init; }
    public MathOperationResult? Binary64Composition { get; init; }
    public string? CanonicalSource { get; init; }
    public string? CanonicalDerivative { get; init; }
    public string? SourceLatex { get; init; }
    public string? SourceCanonicalIrJson { get; init; }
    public string? OriginalSemanticSha256 { get; init; }
    public string? FormalDerivativeRule { get; init; }
    public string? PointEvaluationRule { get; init; }
    public string? ProviderId { get; init; }
    public string? OriginalAssumption { get; init; }
    public string? LossClassification { get; init; }
    public string? RuleId { get; init; }

    public bool IsAdmitted =>
        Status == ExactVerticalStatus.Exact &&
        Original is not null && FormalBodyDerivative is not null &&
        OriginalRootAudit is { HasVerifiedRoots: true } &&
        UnrestrictedCandidates is { IsComplete: true } &&
        ExactValueAtPoint.HasValue &&
        Binary64Projection is { IsFiniteResult: true } &&
        ExactComposition is { Status: ComputationStatus.Exact, HasFinalValue: true } &&
        Binary64Composition is { Status: ComputationStatus.Approximate, HasFinalValue: true } &&
        CanonicalSource is not null && CanonicalDerivative is not null &&
        SourceLatex is not null && SourceCanonicalIrJson is not null &&
        OriginalSemanticSha256 is not null && FormalDerivativeRule is not null &&
        PointEvaluationRule is not null && ProviderId is not null &&
        OriginalAssumption is not null && LossClassification is not null &&
        RuleId is not null;
}
