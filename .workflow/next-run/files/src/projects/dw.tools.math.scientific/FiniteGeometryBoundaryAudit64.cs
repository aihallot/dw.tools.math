using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Refusal and diagnostic categories for a conservative, non-geodesic Euclidean audit.
/// Resolution and cancellation classifications are heuristic warning boundaries, not
/// certified binary64 error estimates or condition-number theorems.
/// </summary>
public enum FiniteGeometryAuditStatus64
{
    Success,
    InvalidDimension,
    InvalidInput,
    ZeroAxis,
    OutsideQualifiedRange,
    NonFiniteResult,
    Cancelled,
    RelativeResolutionRisk,
    ProjectionCancellationRisk
}

/// <summary>
/// A successful diagnostic carries the previously qualified T1 outcome.
/// Refusals never carry a partial result, even if the backend computed one.
/// </summary>
public sealed record FiniteGeometryAuditResult64(
    FiniteGeometryAuditStatus64 Status,
    FiniteGeometryOutcome64? Outcome,
    double? RelativeSignal,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded => Status == FiniteGeometryAuditStatus64.Success &&
        Outcome is { Succeeded: true } && !ErrorBoundGuaranteed;
}

/// <summary>
/// Fail-closed binary64 2D/3D Cartesian input and risk audit over FiniteGeometry64.
/// All coordinates require the same caller-managed linear units and reference frame.
/// The ratio floors reject some mathematically valid but risk-sensitive cases;
/// they are deliberately conservative and are not verified rounding error bounds.
/// </summary>
public static class FiniteGeometryBoundaryAudit64
{
    public const string RuleId = "dw-math-finite-euclidean-boundaries-1";
    public const double MinRelativeDisplacement = 1e-12d;
    public const double MinProjectionDotSignal = 1e-12d;

    public static FiniteGeometryAuditResult64 EvaluateNorm(
        double[]? vector, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);
        var outcome = FiniteGeometry64.Norm(vector, cancellationToken);
        return Wrap(outcome, null, cancellationToken);
    }

    public static FiniteGeometryAuditResult64 EvaluateDistance(
        double[]? start, double[]? end,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);
        var outcome = FiniteGeometry64.Distance(start, end, cancellationToken);
        if (!outcome.Succeeded)
            return Reject(Map(outcome.Status));
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);

        // Backend success proves 2/3 equal dimensions and finite, bounded inputs.
        var maxPosition = 0d;
        for (var i = 0; i < start!.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Reject(FiniteGeometryAuditStatus64.Cancelled);
            maxPosition = global::System.Math.Max(maxPosition,
                global::System.Math.Max(global::System.Math.Abs(start[i]),
                    global::System.Math.Abs(end![i])));
        }
        var observed = outcome.Scalar!.Value;
        double? relative = maxPosition > 0d ? observed / maxPosition : null;
        if (observed > 0d && maxPosition > 0d &&
            relative!.Value < MinRelativeDisplacement)
            return Reject(FiniteGeometryAuditStatus64.RelativeResolutionRisk, relative);
        return Wrap(outcome, relative, cancellationToken);
    }

    public static FiniteGeometryAuditResult64 EvaluateProjection(
        double[]? vector, double[]? axis,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);
        var outcome = FiniteGeometry64.Project(vector, axis, cancellationToken);
        if (!outcome.Succeeded)
            return Reject(Map(outcome.Status));
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);

        // The admitted backend axis is nonzero and its dimension matches vector.
        var scale = 0d;
        foreach (var component in axis!)
        {
            if (cancellationToken.IsCancellationRequested)
                return Reject(FiniteGeometryAuditStatus64.Cancelled);
            scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
        }
        var signed = 0d;
        var absolute = 0d;
        for (var i = 0; i < axis.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Reject(FiniteGeometryAuditStatus64.Cancelled);
            var term = vector![i] * (axis[i] / scale);
            signed += term;
            absolute += global::System.Math.Abs(term);
        }
        if (!double.IsFinite(signed) || !double.IsFinite(absolute))
            return Reject(FiniteGeometryAuditStatus64.NonFiniteResult);
        double? relative = absolute > 0d
            ? global::System.Math.Abs(signed) / absolute : null;
        if (absolute > 0d && relative!.Value < MinProjectionDotSignal)
            return Reject(FiniteGeometryAuditStatus64.ProjectionCancellationRisk, relative);
        return Wrap(outcome, relative, cancellationToken);
    }

    private static FiniteGeometryAuditResult64 Wrap(
        FiniteGeometryOutcome64 outcome, double? relative,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Reject(FiniteGeometryAuditStatus64.Cancelled);
        if (!outcome.Succeeded) return Reject(Map(outcome.Status));
        if (relative is { } signal && (!double.IsFinite(signal) || signal < 0d || signal > 1d))
            return Reject(FiniteGeometryAuditStatus64.NonFiniteResult);
        return new(FiniteGeometryAuditStatus64.Success, outcome, relative, RuleId, false);
    }

    private static FiniteGeometryAuditStatus64 Map(FiniteGeometryStatus64 status) =>
        status switch
        {
            FiniteGeometryStatus64.InvalidDimension => FiniteGeometryAuditStatus64.InvalidDimension,
            FiniteGeometryStatus64.InvalidInput => FiniteGeometryAuditStatus64.InvalidInput,
            FiniteGeometryStatus64.ZeroAxis => FiniteGeometryAuditStatus64.ZeroAxis,
            FiniteGeometryStatus64.OutsideQualifiedRange =>
                FiniteGeometryAuditStatus64.OutsideQualifiedRange,
            FiniteGeometryStatus64.NonFiniteResult => FiniteGeometryAuditStatus64.NonFiniteResult,
            FiniteGeometryStatus64.Cancelled => FiniteGeometryAuditStatus64.Cancelled,
            _ => FiniteGeometryAuditStatus64.NonFiniteResult
        };

    private static FiniteGeometryAuditResult64 Reject(
        FiniteGeometryAuditStatus64 status, double? relative = null) =>
        new(status, null, relative, RuleId, false);
}
