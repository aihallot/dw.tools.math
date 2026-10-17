using System;

namespace Dw.Tools.Math.Analysis;

/// <summary>Typed refusal for audited univariate numerical observations.</summary>
public enum UnivariateAuditStatus64
{
    Success, InvalidInput, InvalidPolicy, MissingBracket,
    NonFiniteEvaluation, NonFiniteIntermediate, EvaluationFailed,
    NonConverged, BudgetExceeded, Cancelled, ResidualNotMet,
    RegularityUnspecified
}

/// <summary>
/// An explicit caller assumption, not a verified mathematical regularity proof.
/// </summary>
public enum QuadratureRegularity64
{
    Unspecified,
    CallerAssumesSmooth
}

/// <summary>Residual-qualified observation, with no continuity or root-error theorem.</summary>
public sealed record AuditedRoot64(
    UnivariateAuditStatus64 Status, double? Root, double? AbsoluteResidual,
    double? ObservedBracketWidth, int Iterations, int FunctionEvaluations,
    bool ContinuityProven, bool ErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded =>
        Status == UnivariateAuditStatus64.Success &&
        Root is { } root && double.IsFinite(root) &&
        AbsoluteResidual is { } residual && double.IsFinite(residual) &&
        ObservedBracketWidth is { } width && double.IsFinite(width) &&
        residual >= 0d && width >= 0d &&
        !ContinuityProven && !ErrorBoundGuaranteed;
}

/// <summary>
/// Simpson refinement is an error estimate only if assumed smoothness applies;
/// neither this result nor the finite samples certify a rigorous error bound.
/// </summary>
public sealed record AuditedQuadrature64(
    UnivariateAuditStatus64 Status, double? Integral,
    double? EstimatedAbsoluteError, int Panels, int FunctionEvaluations,
    bool SmoothnessAssumedByCaller, bool ErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded =>
        Status == UnivariateAuditStatus64.Success &&
        Integral is { } value && double.IsFinite(value) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && SmoothnessAssumedByCaller && !ErrorBoundGuaranteed;
}

/// <summary>
/// Narrow, fail-closed numerical sidecar over the qualified T1 algorithms.
/// Refusal always erases partial numeric results. T1 remains unchanged.
/// </summary>
public static class UnivariateBoundaryAudit64
{
    public const string RuleId = "dw-math-univariate-boundary-audit-binary64-1";

    public static AuditedRoot64 AuditRoot(
        Func<double,double>? function, double lower, double upper,
        RootPolicy64? policy = null, CancellationToken cancellationToken = default)
    {
        var effective = policy ?? new RootPolicy64();
        var calculated = BoundedUnivariateAnalysis64.SolveBracketed(
            function, lower, upper, policy, cancellationToken);
        if(!calculated.Succeeded)
            return RootRefuse(Map(calculated.Status));
        if(calculated.Root is not { } root ||
           calculated.AbsoluteResidual is not { } residual ||
           calculated.ObservedBracketWidth is not { } width ||
           !double.IsFinite(root) || !double.IsFinite(residual) ||
           !double.IsFinite(width))
            return RootRefuse(UnivariateAuditStatus64.NonFiniteIntermediate);
        if(residual > effective.AbsoluteResidualTolerance)
            return RootRefuse(UnivariateAuditStatus64.ResidualNotMet);
        return new(UnivariateAuditStatus64.Success, root, residual, width,
            calculated.Iterations, calculated.FunctionEvaluations,
            false, false, RuleId);
    }

    public static AuditedQuadrature64 AuditQuadrature(
        Func<double,double>? function, double lower, double upper,
        QuadratureRegularity64 regularity = QuadratureRegularity64.Unspecified,
        QuadraturePolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return QuadratureRefuse(UnivariateAuditStatus64.Cancelled);
        if(regularity is not (
            QuadratureRegularity64.Unspecified or
            QuadratureRegularity64.CallerAssumesSmooth))
            return QuadratureRefuse(UnivariateAuditStatus64.InvalidPolicy);
        if(regularity != QuadratureRegularity64.CallerAssumesSmooth)
            return QuadratureRefuse(UnivariateAuditStatus64.RegularityUnspecified);
        var calculated = BoundedUnivariateAnalysis64.IntegrateSimpson(
            function, lower, upper, policy, cancellationToken);
        if(!calculated.Succeeded)
            return QuadratureRefuse(Map(calculated.Status));
        if(calculated.Integral is not { } integral ||
           calculated.EstimatedAbsoluteError is not { } estimate ||
           !double.IsFinite(integral) || !double.IsFinite(estimate))
            return QuadratureRefuse(UnivariateAuditStatus64.NonFiniteIntermediate);
        return new(UnivariateAuditStatus64.Success, integral, estimate,
            calculated.Panels, calculated.FunctionEvaluations, true, false, RuleId);
    }

    private static UnivariateAuditStatus64 Map(UnivariateStatus64 status) => status switch
    {
        UnivariateStatus64.InvalidInput => UnivariateAuditStatus64.InvalidInput,
        UnivariateStatus64.InvalidPolicy => UnivariateAuditStatus64.InvalidPolicy,
        UnivariateStatus64.MissingBracket => UnivariateAuditStatus64.MissingBracket,
        UnivariateStatus64.NonFiniteEvaluation => UnivariateAuditStatus64.NonFiniteEvaluation,
        UnivariateStatus64.NonFiniteIntermediate => UnivariateAuditStatus64.NonFiniteIntermediate,
        UnivariateStatus64.EvaluationFailed => UnivariateAuditStatus64.EvaluationFailed,
        UnivariateStatus64.NonConverged => UnivariateAuditStatus64.NonConverged,
        UnivariateStatus64.BudgetExceeded => UnivariateAuditStatus64.BudgetExceeded,
        UnivariateStatus64.Cancelled => UnivariateAuditStatus64.Cancelled,
        _ => UnivariateAuditStatus64.NonFiniteIntermediate
    };

    private static AuditedRoot64 RootRefuse(UnivariateAuditStatus64 status) =>
        new(status, null, null, null, 0, 0, false, false, RuleId);

    private static AuditedQuadrature64 QuadratureRefuse(UnivariateAuditStatus64 status) =>
        new(status, null, null, 0, 0, false, false, RuleId);
}
