using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Conservative refusal categories for caller-supplied finite time coordinates.
/// A time coordinate has caller-owned units; no clock or timezone is inferred.
/// </summary>
public enum FiniteUniformSeriesAuditStatus64
{
    Success,
    SampleTimesRequired,
    InvalidTimeLength,
    InvalidTimeValue,
    NonIncreasingTime,
    IrregularSampling,
    TimestampResolutionRisk,
    RelativeDifferenceRisk,
    MeanCancellationRisk,
    InvalidLength,
    InvalidSampleInterval,
    InvalidWindow,
    InvalidInput,
    OutsideQualifiedRange,
    NonFiniteResult,
    Cancelled
}

/// <summary>
/// A successful result contains one already-qualified uniform series outcome.
/// A refused result never exposes partial numeric samples. Relative signal
/// ratios are screening heuristics, not guaranteed binary64 error bounds.
/// </summary>
public sealed record FiniteUniformSeriesAuditOutcome64(
    FiniteUniformSeriesAuditStatus64 Status,
    FiniteUniformSeriesOutcome64? Outcome,
    double? MinimumSignalRatio,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == FiniteUniformSeriesAuditStatus64.Success &&
        !ErrorBoundGuaranteed && Outcome is { Succeeded: true } &&
        (MinimumSignalRatio is null ||
         (double.IsFinite(MinimumSignalRatio.Value) && MinimumSignalRatio.Value >= 0d));
}

/// <summary>
/// Bounded sidecar over FiniteUniformSeries64, using explicit finite uniform
/// sample coordinates to reject irregular spacing and conservative numerical
/// resolution/cancellation risks. It never interpolates, rescales samples,
/// converts physical units, divides differences by time, or forecasts.
/// </summary>
public static class FiniteUniformSeriesBoundaryAudit64
{
    public const string RuleId = "dw-math-finite-uniform-series-boundaries-1";
    public const double MaxAbsTimeCoordinate = 1e15d;
    public const double MaxRelativeIntervalDeviation = 1e-10d;
    public const double MinRelativeTimeStep = 1e-12d;
    public const double MinRelativeDifferenceSignal = 1e-12d;
    public const double MinMeanSignalRatio = 1e-12d;

    public static FiniteUniformSeriesAuditOutcome64 EvaluateFirstDifferences(
        double[]? samples, double[]? sampleTimes, double sampleInterval,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
        var outcome = FiniteUniformSeries64.FirstDifferences(
            samples, sampleInterval, cancellationToken);
        if (!outcome.Succeeded) return Refuse(Map(outcome.Status));
        var timing = ValidateTimes(sampleTimes, samples!, sampleInterval, cancellationToken);
        if (timing != FiniteUniformSeriesAuditStatus64.Success)
            return Refuse(timing);

        double? minimum = null;
        for (var index = 0; index < outcome.Samples!.Length; index++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
            var scale = global::System.Math.Max(
                global::System.Math.Abs(samples![index]),
                global::System.Math.Abs(samples[index + 1]));
            var delta = outcome.Samples[index];
            if (delta == 0d || scale == 0d) continue;
            var signal = global::System.Math.Abs(delta) / scale;
            if (!double.IsFinite(signal) || signal < 0d)
                return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
            minimum = minimum is { } previous
                ? global::System.Math.Min(previous, signal) : signal;
            if (signal < MinRelativeDifferenceSignal)
                return Refuse(FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk, signal);
        }
        return Success(outcome, minimum, cancellationToken);
    }

    public static FiniteUniformSeriesAuditOutcome64 EvaluateMovingAverage(
        double[]? samples, double[]? sampleTimes, double sampleInterval,
        int window, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
        var outcome = FiniteUniformSeries64.MovingAverage(
            samples, sampleInterval, window, cancellationToken);
        if (!outcome.Succeeded) return Refuse(Map(outcome.Status));
        var timing = ValidateTimes(sampleTimes, samples!, sampleInterval, cancellationToken);
        if (timing != FiniteUniformSeriesAuditStatus64.Success)
            return Refuse(timing);

        double? minimum = null;
        for (var offset = 0; offset < outcome.Samples!.Length; offset++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
            var absoluteSum = 0d;
            for (var index = 0; index < window; index++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
                absoluteSum += global::System.Math.Abs(samples![offset + index]);
            }
            if (!double.IsFinite(absoluteSum))
                return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
            if (absoluteSum == 0d) continue;
            var signal = global::System.Math.Abs(outcome.Samples[offset]) /
                (absoluteSum / window);
            if (!double.IsFinite(signal) || signal < 0d)
                return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
            minimum = minimum is { } previous
                ? global::System.Math.Min(previous, signal) : signal;
            if (signal < MinMeanSignalRatio)
                return Refuse(FiniteUniformSeriesAuditStatus64.MeanCancellationRisk, signal);
        }
        return Success(outcome, minimum, cancellationToken);
    }

    private static FiniteUniformSeriesAuditStatus64 ValidateTimes(
        double[]? times, double[] values, double interval, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return FiniteUniformSeriesAuditStatus64.Cancelled;
        if (times is null) return FiniteUniformSeriesAuditStatus64.SampleTimesRequired;
        if (times.Length != values.Length)
            return FiniteUniformSeriesAuditStatus64.InvalidTimeLength;
        foreach (var time in times)
        {
            if (token.IsCancellationRequested)
                return FiniteUniformSeriesAuditStatus64.Cancelled;
            if (!double.IsFinite(time))
                return FiniteUniformSeriesAuditStatus64.InvalidTimeValue;
            if (global::System.Math.Abs(time) > MaxAbsTimeCoordinate)
                return FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange;
        }
        for (var index = 1; index < times.Length; index++)
        {
            if (token.IsCancellationRequested)
                return FiniteUniformSeriesAuditStatus64.Cancelled;
            var largestTime = global::System.Math.Max(
                global::System.Math.Abs(times[index - 1]),
                global::System.Math.Abs(times[index]));
            if (largestTime > 0d &&
                interval / largestTime < MinRelativeTimeStep)
                return FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk;
            var elapsed = times[index] - times[index - 1];
            if (elapsed <= 0d)
                return FiniteUniformSeriesAuditStatus64.NonIncreasingTime;
            if (!double.IsFinite(elapsed))
                return FiniteUniformSeriesAuditStatus64.NonFiniteResult;
            var normalized = elapsed / interval;
            if (!double.IsFinite(normalized) ||
                global::System.Math.Abs(normalized - 1d) > MaxRelativeIntervalDeviation)
                return FiniteUniformSeriesAuditStatus64.IrregularSampling;
        }
        return FiniteUniformSeriesAuditStatus64.Success;
    }

    private static FiniteUniformSeriesAuditOutcome64 Success(
        FiniteUniformSeriesOutcome64 source, double? ratio, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
        if (!source.Succeeded || ratio is { } value &&
            (!double.IsFinite(value) || value < 0d))
            return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
        return new(FiniteUniformSeriesAuditStatus64.Success, source,
            ratio, RuleId, false);
    }

    private static FiniteUniformSeriesAuditStatus64 Map(FiniteUniformSeriesStatus64 status) =>
        status switch
        {
            FiniteUniformSeriesStatus64.InvalidLength =>
                FiniteUniformSeriesAuditStatus64.InvalidLength,
            FiniteUniformSeriesStatus64.InvalidSampleInterval =>
                FiniteUniformSeriesAuditStatus64.InvalidSampleInterval,
            FiniteUniformSeriesStatus64.InvalidWindow =>
                FiniteUniformSeriesAuditStatus64.InvalidWindow,
            FiniteUniformSeriesStatus64.InvalidInput =>
                FiniteUniformSeriesAuditStatus64.InvalidInput,
            FiniteUniformSeriesStatus64.OutsideQualifiedRange =>
                FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange,
            FiniteUniformSeriesStatus64.NonFiniteResult =>
                FiniteUniformSeriesAuditStatus64.NonFiniteResult,
            FiniteUniformSeriesStatus64.Cancelled =>
                FiniteUniformSeriesAuditStatus64.Cancelled,
            _ => FiniteUniformSeriesAuditStatus64.NonFiniteResult
        };

    private static FiniteUniformSeriesAuditOutcome64 Refuse(
        FiniteUniformSeriesAuditStatus64 status, double? signal = null) =>
        new(status, null, signal, RuleId, false);
}
