using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Explicit statement that the caller manages the Cartesian frame, discrete
/// signal interpretation, uniform-series interval and independent value units.
/// This is an acknowledgement, not proof of unit compatibility.
/// </summary>
public enum M6W02C03UnitPolicy64
{
    Unspecified,
    CallerManagesIndependentUnits
}

/// <summary>
/// Requests ALL three previously qualified conservative family audits together.
/// No implicit common coordinate system, time unit or physical signal semantics.
/// </summary>
public sealed record M6W02C03BoundaryRequest64(
    M6W02C03Request64? Scientific,
    double[]? SampleTimes,
    M6W02C03UnitPolicy64 UnitPolicy,
    FiniteSignalSamplingPolicy64 SamplingPolicy,
    FiniteSignalConvolutionPolicy64 ConvolutionTopology);

/// <summary>
/// One aggregate cross-family failure class; the optional typed family reason
/// preserves the precise accepted S01/S02/S03 audit refusal.
/// </summary>
public enum M6W02C03BoundaryStatus64
{
    Success,
    InvalidRequest,
    UnitPolicyRequired,
    GeometryRefused,
    SignalRefused,
    UniformSeriesRefused,
    Cancelled
}

/// <summary>
/// Seven family-audited results are returned only together. Refusals expose
/// their reason, never partial numeric results or a certified error theorem.
/// </summary>
public sealed record M6W02C03BoundaryOutcome64(
    M6W02C03BoundaryStatus64 Status,
    FiniteGeometryAuditResult64? Norm,
    FiniteGeometryAuditResult64? Distance,
    FiniteGeometryAuditResult64? Projection,
    FiniteSignalAuditOutcome64? Dft,
    FiniteSignalAuditOutcome64? LinearConvolution,
    FiniteUniformSeriesAuditOutcome64? Differences,
    FiniteUniformSeriesAuditOutcome64? MovingAverage,
    FiniteGeometryAuditStatus64? GeometryReason,
    FiniteSignalAuditStatus64? SignalReason,
    FiniteUniformSeriesAuditStatus64? UniformSeriesReason,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == M6W02C03BoundaryStatus64.Success &&
        !ErrorBoundGuaranteed &&
        GeometryReason is null && SignalReason is null &&
        UniformSeriesReason is null &&
        Norm is { Succeeded: true } &&
        Distance is { Succeeded: true } &&
        Projection is { Succeeded: true } &&
        Dft is { Succeeded: true, Transform: not null } &&
        LinearConvolution is { Succeeded: true, Convolution: not null } &&
        Differences is { Succeeded: true } &&
        MovingAverage is { Succeeded: true };
}

/// <summary>
/// BCL-only cross-family orchestration of the independently qualified S01
/// Euclidean, S02 direct discrete-signal and S03 uniform-grid numerical
/// boundary audits. Policies are explicit. This does not create new FFT,
/// geodesic, irregular resampling, forecasting or numeric error guarantees.
/// </summary>
public static class M6W02C03BoundaryAggregate64
{
    public const string RuleId = "dw-math-m6-w02-c03-cross-family-boundaries-1";

    public static M6W02C03BoundaryOutcome64 Evaluate(
        M6W02C03BoundaryRequest64? request,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03BoundaryStatus64.Cancelled);
        if (request is null)
            return Refuse(M6W02C03BoundaryStatus64.InvalidRequest);
        var input = request.Scientific;
        if (input is null)
            return Refuse(M6W02C03BoundaryStatus64.InvalidRequest);
        if (request.UnitPolicy != M6W02C03UnitPolicy64.CallerManagesIndependentUnits)
            return Refuse(M6W02C03BoundaryStatus64.UnitPolicyRequired);

        var norm = FiniteGeometryBoundaryAudit64.EvaluateNorm(
            input.GeometryEnd, cancellationToken);
        if (!norm.Succeeded) return RefuseGeometry(norm.Status);
        var distance = FiniteGeometryBoundaryAudit64.EvaluateDistance(
            input.GeometryStart, input.GeometryEnd, cancellationToken);
        if (!distance.Succeeded) return RefuseGeometry(distance.Status);
        var projection = FiniteGeometryBoundaryAudit64.EvaluateProjection(
            input.GeometryEnd, input.ProjectionAxis, cancellationToken);
        if (!projection.Succeeded) return RefuseGeometry(projection.Status);
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03BoundaryStatus64.Cancelled);

        var dft = FiniteSignalBoundaryAudit64.EvaluateDft(
            input.DiscreteSamples, sampling: request.SamplingPolicy,
            cancellationToken: cancellationToken);
        if (!dft.Succeeded) return RefuseSignal(dft.Status);
        var convolution = FiniteSignalBoundaryAudit64.EvaluateConvolution(
            input.ConvolutionLeft, input.ConvolutionRight,
            topology: request.ConvolutionTopology,
            cancellationToken: cancellationToken);
        if (!convolution.Succeeded) return RefuseSignal(convolution.Status);
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03BoundaryStatus64.Cancelled);

        var differences = FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
            input.UniformSamples, request.SampleTimes,
            input.SampleInterval, cancellationToken);
        if (!differences.Succeeded) return RefuseSeries(differences.Status);
        var average = FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
            input.UniformSamples, request.SampleTimes, input.SampleInterval,
            input.MovingWindow, cancellationToken);
        if (!average.Succeeded) return RefuseSeries(average.Status);
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03BoundaryStatus64.Cancelled);

        return new(M6W02C03BoundaryStatus64.Success,
            norm, distance, projection, dft, convolution,
            differences, average, null, null, null, RuleId, false);
    }

    private static M6W02C03BoundaryOutcome64 RefuseGeometry(
        FiniteGeometryAuditStatus64 reason) =>
        reason == FiniteGeometryAuditStatus64.Cancelled
            ? Refuse(M6W02C03BoundaryStatus64.Cancelled)
            : Refuse(M6W02C03BoundaryStatus64.GeometryRefused, geometry: reason);

    private static M6W02C03BoundaryOutcome64 RefuseSignal(
        FiniteSignalAuditStatus64 reason) =>
        reason == FiniteSignalAuditStatus64.Cancelled
            ? Refuse(M6W02C03BoundaryStatus64.Cancelled)
            : Refuse(M6W02C03BoundaryStatus64.SignalRefused, signal: reason);

    private static M6W02C03BoundaryOutcome64 RefuseSeries(
        FiniteUniformSeriesAuditStatus64 reason) =>
        reason == FiniteUniformSeriesAuditStatus64.Cancelled
            ? Refuse(M6W02C03BoundaryStatus64.Cancelled)
            : Refuse(M6W02C03BoundaryStatus64.UniformSeriesRefused, series: reason);

    private static M6W02C03BoundaryOutcome64 Refuse(
        M6W02C03BoundaryStatus64 status,
        FiniteGeometryAuditStatus64? geometry = null,
        FiniteSignalAuditStatus64? signal = null,
        FiniteUniformSeriesAuditStatus64? series = null) =>
        new(status, null, null, null, null, null, null, null,
            geometry, signal, series, RuleId, false);
}
