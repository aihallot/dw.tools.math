using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>Nonpartial, bounded uniformly sampled series operation statuses.</summary>
public enum FiniteUniformSeriesStatus64
{
    Success,
    InvalidLength,
    InvalidSampleInterval,
    InvalidWindow,
    InvalidInput,
    OutsideQualifiedRange,
    NonFiniteResult,
    Cancelled
}

/// <summary>
/// Approximate binary64 first differences or complete-window arithmetic means.
/// SampleInterval is caller-declared metadata, with no inferred time unit.
/// On refusal Samples is always null; no exact numerical error theorem is implied.
/// </summary>
public sealed record FiniteUniformSeriesOutcome64(
    FiniteUniformSeriesStatus64 Status,
    double[]? Samples,
    double SampleInterval,
    int Window,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded
    {
        get
        {
            if (Status != FiniteUniformSeriesStatus64.Success ||
                ErrorBoundGuaranteed ||
                !double.IsFinite(SampleInterval) || SampleInterval <= 0d ||
                Samples is null || Samples.Length < 1 ||
                Samples.Length > FiniteUniformSeries64.MaxSamplesPerInput)
                return false;
            foreach (var sample in Samples)
                if (!double.IsFinite(sample))
                    return false;
            return true;
        }
    }
}

/// <summary>
/// BCL-only bounded finite uniformly sampled first differences and sliding
/// arithmetic means. Samples share one caller-owned value unit, and all points
/// have an explicitly supplied strictly positive finite sample interval.
/// No derivative, forecast, time-unit conversion, resampling or interpolation.
/// </summary>
public static class FiniteUniformSeries64
{
    public const string RuleId = "dw-math-finite-uniform-series-1";
    public const int MaxSamplesPerInput = 32;
    public const double MaxAbsInputSample = 1e6d;
    public const double MaxAbsOutputSample = 2e6d;

    /// <summary>
    /// Adjacent first differences x[i+1] - x[i], not slopes or derivatives.
    /// Result length N-1. The interval is returned but never divides a value.
    /// </summary>
    public static FiniteUniformSeriesOutcome64 FirstDifferences(
        double[]? samples, double sampleInterval,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesStatus64.Cancelled);
        var status = Validate(samples, sampleInterval, minimumLength: 2);
        if (status != FiniteUniformSeriesStatus64.Success)
            return Refuse(status);

        var differences = new double[samples!.Length - 1];
        for (var i = 0; i < differences.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteUniformSeriesStatus64.Cancelled);
            var value = samples[i + 1] - samples[i];
            if (!double.IsFinite(value))
                return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
            if (global::System.Math.Abs(value) > MaxAbsOutputSample)
                return Refuse(FiniteUniformSeriesStatus64.OutsideQualifiedRange);
            differences[i] = value;
        }
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesStatus64.Cancelled);
        return new(FiniteUniformSeriesStatus64.Success, differences,
            sampleInterval, 2, RuleId, false);
    }

    /// <summary>
    /// Complete-window (valid) sliding arithmetic mean.
    /// Result length N-window+1 with index zero at the first complete window.
    /// No partial windows or implicit missing-value filling.
    /// </summary>
    public static FiniteUniformSeriesOutcome64 MovingAverage(
        double[]? samples, double sampleInterval, int window,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesStatus64.Cancelled);
        var status = Validate(samples, sampleInterval, minimumLength: 1);
        if (status != FiniteUniformSeriesStatus64.Success)
            return Refuse(status);
        if (window < 1 || window > samples!.Length)
            return Refuse(FiniteUniformSeriesStatus64.InvalidWindow);

        var means = new double[samples.Length - window + 1];
        for (var offset = 0; offset < means.Length; offset++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteUniformSeriesStatus64.Cancelled);
            var total = 0d;
            for (var i = 0; i < window; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Refuse(FiniteUniformSeriesStatus64.Cancelled);
                total += samples[offset + i];
            }
            if (!double.IsFinite(total))
                return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
            var mean = total / window;
            if (!double.IsFinite(mean))
                return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
            if (global::System.Math.Abs(mean) > MaxAbsOutputSample)
                return Refuse(FiniteUniformSeriesStatus64.OutsideQualifiedRange);
            means[offset] = mean;
        }
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteUniformSeriesStatus64.Cancelled);
        return new(FiniteUniformSeriesStatus64.Success, means,
            sampleInterval, window, RuleId, false);
    }

    private static FiniteUniformSeriesStatus64 Validate(
        double[]? samples, double interval, int minimumLength)
    {
        if (samples is null || samples.Length < minimumLength ||
            samples.Length > MaxSamplesPerInput)
            return FiniteUniformSeriesStatus64.InvalidLength;
        if (!double.IsFinite(interval) || interval <= 0d)
            return FiniteUniformSeriesStatus64.InvalidSampleInterval;
        foreach (var value in samples)
        {
            if (!double.IsFinite(value))
                return FiniteUniformSeriesStatus64.InvalidInput;
            if (global::System.Math.Abs(value) > MaxAbsInputSample)
                return FiniteUniformSeriesStatus64.OutsideQualifiedRange;
        }
        return FiniteUniformSeriesStatus64.Success;
    }

    private static FiniteUniformSeriesOutcome64 Refuse(
        FiniteUniformSeriesStatus64 status) =>
        new(status, null, 0d, 0, RuleId, false);
}
