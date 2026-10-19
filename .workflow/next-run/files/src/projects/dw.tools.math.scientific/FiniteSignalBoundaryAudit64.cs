using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// An explicit agreement to interpret finite complex input as discrete samples.
/// Acceptance does not demonstrate a Nyquist limit, bandwidth or alias-free sampling.
/// </summary>
public enum FiniteSignalSamplingPolicy64
{
    Unspecified,
    CallerAcceptsDiscreteSamples
}

/// <summary>Request zero-extended linear convolution; circular is not implemented.</summary>
public enum FiniteSignalConvolutionPolicy64
{
    Unspecified,
    LinearZeroExtended,
    CircularUnsupported
}

/// <summary>Typed finite-signal policy and conservative numerical risk refusals.</summary>
public enum FiniteSignalAuditStatus64
{
    Success,
    SamplingPolicyRequired,
    ConvolutionTopologyRequired,
    CircularConvolutionUnsupported,
    SpectralCancellationRisk,
    ConvolutionCancellationRisk,
    InvalidLength,
    InvalidInput,
    OutsideQualifiedRange,
    NonFiniteResult,
    Cancelled
}

/// <summary>
/// Exactly one numeric payload on success, no partial DFT or convolution on failure.
/// A signal ratio is diagnostic only and never a certified floating-point error bound.
/// </summary>
public sealed record FiniteSignalAuditOutcome64(
    FiniteSignalAuditStatus64 Status,
    FiniteDirectDftOutcome64? Transform,
    FiniteLinearConvolutionOutcome64? Convolution,
    double? MinimumSignalRatio,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == FiniteSignalAuditStatus64.Success &&
        !ErrorBoundGuaranteed &&
        ((Transform is { Succeeded: true } && Convolution is null) ||
         (Convolution is { Succeeded: true } && Transform is null));
}

/// <summary>
/// Conservative BCL-only sidecar over FiniteSignal64 direct binary64 algorithms.
/// The caller must explicitly accept discrete sample semantics or request a
/// zero-extended linear topology. These policies do not infer frequency units,
/// Nyquist bandwidth, anti-alias filtering, periodic signals or FFT semantics.
/// </summary>
public static class FiniteSignalBoundaryAudit64
{
    public const string RuleId = "dw-math-finite-direct-signal-boundaries-1";
    public const double MinimumSignalRatio = 1e-12d;

    public static FiniteSignalAuditOutcome64 EvaluateDft(
        Complex[]? samples,
        bool inverse = false,
        FiniteSignalSamplingPolicy64 sampling = FiniteSignalSamplingPolicy64.Unspecified,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteSignalAuditStatus64.Cancelled);
        if (sampling != FiniteSignalSamplingPolicy64.CallerAcceptsDiscreteSamples)
            return Refuse(FiniteSignalAuditStatus64.SamplingPolicyRequired);
        var result = FiniteSignal64.DirectDft(samples, inverse, cancellationToken);
        if (!result.Succeeded)
            return Refuse(Map(result.Status));
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteSignalAuditStatus64.Cancelled);

        // For a DFT bin every twiddle has magnitude one. A small bin relative
        // to the sum of input magnitudes can reflect severe signed cancellation,
        // including an exactly zero bin. This ratio is NOT an error estimate.
        var sumAbsoluteInputs = 0d;
        foreach (var sample in samples!)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteSignalAuditStatus64.Cancelled);
            sumAbsoluteInputs += Complex.Abs(sample);
        }
        if (!double.IsFinite(sumAbsoluteInputs))
            return Refuse(FiniteSignalAuditStatus64.NonFiniteResult);
        double? minimum = null;
        if (sumAbsoluteInputs > 0d)
        {
            foreach (var bin in result.Bins!)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Refuse(FiniteSignalAuditStatus64.Cancelled);
                var ratio = Complex.Abs(bin) / sumAbsoluteInputs;
                if (!double.IsFinite(ratio) || ratio < 0d)
                    return Refuse(FiniteSignalAuditStatus64.NonFiniteResult);
                minimum = minimum is { } prior
                    ? global::System.Math.Min(prior, ratio) : ratio;
                if (ratio < MinimumSignalRatio)
                    return Refuse(FiniteSignalAuditStatus64.SpectralCancellationRisk, ratio);
            }
        }
        return Success(result, null, minimum, cancellationToken);
    }

    public static FiniteSignalAuditOutcome64 EvaluateConvolution(
        double[]? first,
        double[]? second,
        FiniteSignalConvolutionPolicy64 topology = FiniteSignalConvolutionPolicy64.Unspecified,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteSignalAuditStatus64.Cancelled);
        if (topology == FiniteSignalConvolutionPolicy64.CircularUnsupported)
            return Refuse(FiniteSignalAuditStatus64.CircularConvolutionUnsupported);
        if (topology != FiniteSignalConvolutionPolicy64.LinearZeroExtended)
            return Refuse(FiniteSignalAuditStatus64.ConvolutionTopologyRequired);

        var result = FiniteSignal64.LinearConvolution(first, second, cancellationToken);
        if (!result.Succeeded)
            return Refuse(Map(result.Status));
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteSignalAuditStatus64.Cancelled);

        double? minimum = null;
        for (var k = 0; k < result.Samples!.Length; k++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FiniteSignalAuditStatus64.Cancelled);
            var sumAbsoluteTerms = 0d;
            var begin = global::System.Math.Max(0, k - second!.Length + 1);
            var end = global::System.Math.Min(k, first!.Length - 1);
            for (var i = begin; i <= end; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Refuse(FiniteSignalAuditStatus64.Cancelled);
                sumAbsoluteTerms += global::System.Math.Abs(first[i] * second[k - i]);
            }
            if (!double.IsFinite(sumAbsoluteTerms))
                return Refuse(FiniteSignalAuditStatus64.NonFiniteResult);
            if (sumAbsoluteTerms == 0d)
                continue;

            var ratio = global::System.Math.Abs(result.Samples[k]) / sumAbsoluteTerms;
            if (!double.IsFinite(ratio) || ratio < 0d)
                return Refuse(FiniteSignalAuditStatus64.NonFiniteResult);
            minimum = minimum is { } prior
                ? global::System.Math.Min(prior, ratio) : ratio;
            if (ratio < MinimumSignalRatio)
                return Refuse(FiniteSignalAuditStatus64.ConvolutionCancellationRisk, ratio);
        }
        return Success(null, result, minimum, cancellationToken);
    }

    private static FiniteSignalAuditOutcome64 Success(
        FiniteDirectDftOutcome64? transform, FiniteLinearConvolutionOutcome64? convolution,
        double? signalRatio, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FiniteSignalAuditStatus64.Cancelled);
        if ((transform?.Succeeded != true && convolution?.Succeeded != true) ||
            (transform is not null && convolution is not null))
            return Refuse(FiniteSignalAuditStatus64.NonFiniteResult);
        return new(FiniteSignalAuditStatus64.Success, transform, convolution,
            signalRatio, RuleId, false);
    }

    private static FiniteSignalAuditStatus64 Map(FiniteSignalStatus64 status) =>
        status switch
        {
            FiniteSignalStatus64.InvalidLength => FiniteSignalAuditStatus64.InvalidLength,
            FiniteSignalStatus64.InvalidInput => FiniteSignalAuditStatus64.InvalidInput,
            FiniteSignalStatus64.OutsideQualifiedRange =>
                FiniteSignalAuditStatus64.OutsideQualifiedRange,
            FiniteSignalStatus64.NonFiniteResult =>
                FiniteSignalAuditStatus64.NonFiniteResult,
            FiniteSignalStatus64.Cancelled => FiniteSignalAuditStatus64.Cancelled,
            _ => FiniteSignalAuditStatus64.NonFiniteResult
        };

    private static FiniteSignalAuditOutcome64 Refuse(
        FiniteSignalAuditStatus64 status, double? ratio = null) =>
        new(status, null, null, ratio, RuleId, false);
}
