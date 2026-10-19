using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Explicit cross-family, bounded scientific input. Each input family retains
/// its own caller-managed Cartesian, sample and time units. This does not
/// reconcile coordinate frames or imply geodesy, alias-free sampling or
/// a forecasting model.
/// </summary>
public sealed record M6W02C03Request64(
    double[]? GeometryStart,
    double[]? GeometryEnd,
    double[]? ProjectionAxis,
    Complex[]? DiscreteSamples,
    double[]? ConvolutionLeft,
    double[]? ConvolutionRight,
    double[]? UniformSamples,
    double SampleInterval,
    int MovingWindow);

/// <summary>Fail-closed family identification without exposing partial results.</summary>
public enum M6W02C03Status64
{
    Success,
    InvalidRequest,
    GeometryRefused,
    SignalRefused,
    UniformSeriesRefused,
    Cancelled
}

/// <summary>
/// All seven independently qualified numeric results are admitted together,
/// or NONE are exposed. No certified binary64 error bound is implied.
/// </summary>
public sealed record M6W02C03Outcome64(
    M6W02C03Status64 Status,
    FiniteGeometryOutcome64? Norm,
    FiniteGeometryOutcome64? Distance,
    FiniteGeometryOutcome64? Projection,
    FiniteDirectDftOutcome64? Dft,
    FiniteLinearConvolutionOutcome64? LinearConvolution,
    FiniteUniformSeriesOutcome64? Differences,
    FiniteUniformSeriesOutcome64? MovingAverage,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == M6W02C03Status64.Success && !ErrorBoundGuaranteed &&
        Norm is { Succeeded: true, Scalar: not null } &&
        Distance is { Succeeded: true, Scalar: not null } &&
        Projection is { Succeeded: true, Coordinates: not null } &&
        Dft is { Succeeded: true, Bins: not null } &&
        LinearConvolution is { Succeeded: true, Samples: not null } &&
        Differences is { Succeeded: true, Samples: not null } &&
        MovingAverage is { Succeeded: true, Samples: not null };
}

/// <summary>
/// One explicit, all-or-nothing composition of the already qualified S01,
/// S02 and S03 direct T1 numerical kernels. Uses BCL-only binary64 data.
/// This does NOT replace independently qualified S01/S02/S03 T2 domain-risk
/// audits. Cross-family boundary integration remains the parent C03 T2 gate.
/// </summary>
public static class M6W02C03Contract
{
    public const string RuleId = "dw-math-m6-w02-c03-aggregate-1";

    public static M6W02C03Outcome64 Evaluate(
        M6W02C03Request64? request,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03Status64.Cancelled);
        if (request is null)
            return Refuse(M6W02C03Status64.InvalidRequest);

        var norm = FiniteGeometry64.Norm(request.GeometryEnd, cancellationToken);
        if (!norm.Succeeded) return Refuse(SelectGeometry(norm.Status));
        var distance = FiniteGeometry64.Distance(
            request.GeometryStart, request.GeometryEnd, cancellationToken);
        if (!distance.Succeeded) return Refuse(SelectGeometry(distance.Status));
        var projection = FiniteGeometry64.Project(
            request.GeometryEnd, request.ProjectionAxis, cancellationToken);
        if (!projection.Succeeded) return Refuse(SelectGeometry(projection.Status));
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03Status64.Cancelled);

        var dft = FiniteSignal64.DirectDft(
            request.DiscreteSamples, cancellationToken: cancellationToken);
        if (!dft.Succeeded) return Refuse(SelectSignal(dft.Status));
        var convolution = FiniteSignal64.LinearConvolution(
            request.ConvolutionLeft, request.ConvolutionRight, cancellationToken);
        if (!convolution.Succeeded) return Refuse(SelectSignal(convolution.Status));
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03Status64.Cancelled);

        var differences = FiniteUniformSeries64.FirstDifferences(
            request.UniformSamples, request.SampleInterval, cancellationToken);
        if (!differences.Succeeded) return Refuse(SelectSeries(differences.Status));
        var average = FiniteUniformSeries64.MovingAverage(
            request.UniformSamples, request.SampleInterval,
            request.MovingWindow, cancellationToken);
        if (!average.Succeeded) return Refuse(SelectSeries(average.Status));
        if (cancellationToken.IsCancellationRequested)
            return Refuse(M6W02C03Status64.Cancelled);

        return new(M6W02C03Status64.Success, norm, distance, projection,
            dft, convolution, differences, average, RuleId, false);
    }

    private static M6W02C03Status64 SelectGeometry(FiniteGeometryStatus64 status) =>
        status == FiniteGeometryStatus64.Cancelled
            ? M6W02C03Status64.Cancelled : M6W02C03Status64.GeometryRefused;

    private static M6W02C03Status64 SelectSignal(FiniteSignalStatus64 status) =>
        status == FiniteSignalStatus64.Cancelled
            ? M6W02C03Status64.Cancelled : M6W02C03Status64.SignalRefused;

    private static M6W02C03Status64 SelectSeries(FiniteUniformSeriesStatus64 status) =>
        status == FiniteUniformSeriesStatus64.Cancelled
            ? M6W02C03Status64.Cancelled : M6W02C03Status64.UniformSeriesRefused;

    private static M6W02C03Outcome64 Refuse(M6W02C03Status64 status) =>
        new(status, null, null, null, null, null, null, null, RuleId, false);
}
