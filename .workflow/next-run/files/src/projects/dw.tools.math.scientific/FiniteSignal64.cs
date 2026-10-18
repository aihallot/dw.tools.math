using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>Explicit, nonpartial outcomes of bounded direct signal calculations.</summary>
public enum FiniteSignalStatus64
{
    Success, InvalidLength, InvalidInput, OutsideQualifiedRange, NonFiniteResult, Cancelled
}

/// <summary>Approximate direct complex DFT bins, not FFT or a certified error bound.</summary>
public sealed record FiniteDirectDftOutcome64(
    FiniteSignalStatus64 Status, Complex[]? Bins, bool Inverse,
    string RuleId, bool ErrorBoundGuaranteed)
{
    public bool Succeeded
    {
        get
        {
            if (Status != FiniteSignalStatus64.Success || ErrorBoundGuaranteed ||
                Bins is null || Bins.Length < 1 ||
                Bins.Length > FiniteSignal64.MaxSamplesPerInput)
                return false;
            foreach (var value in Bins)
                if (!double.IsFinite(value.Real) || !double.IsFinite(value.Imaginary))
                    return false;
            return true;
        }
    }
}

/// <summary>Approximate zero-extended linear convolution; not periodic/circular.</summary>
public sealed record FiniteLinearConvolutionOutcome64(
    FiniteSignalStatus64 Status, double[]? Samples,
    string RuleId, bool ErrorBoundGuaranteed)
{
    public bool Succeeded
    {
        get
        {
            if (Status != FiniteSignalStatus64.Success || ErrorBoundGuaranteed ||
                Samples is null || Samples.Length < 1 ||
                Samples.Length > 2 * FiniteSignal64.MaxSamplesPerInput - 1)
                return false;
            foreach (var value in Samples)
                if (!double.IsFinite(value)) return false;
            return true;
        }
    }
}

/// <summary>
/// Managed BCL-only O(N^2) DFT and zero-extended linear convolution.
/// Complex bins use the negative-exponent forward convention and positive
/// inverse exponent with one division by N. Operations use bounded binary64
/// and do not guarantee exactness, alias-free sampling or numerical error bounds.
/// </summary>
public static class FiniteSignal64
{
    public const string RuleId = "dw-math-finite-direct-signal-1";
    public const int MaxSamplesPerInput = 32;
    public const double MaxAbsInputComponent = 1e6d;
    public const double MaxAbsResultComponent = 4e13d;

    public static FiniteDirectDftOutcome64 DirectDft(
        Complex[]? samples, bool inverse = false,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return RejectDft(FiniteSignalStatus64.Cancelled, inverse);
        var status = CheckComplexSamples(samples);
        if (status != FiniteSignalStatus64.Success) return RejectDft(status, inverse);

        var length = samples!.Length;
        var bins = new Complex[length];
        var sign = inverse ? 1d : -1d;
        for (var k = 0; k < length; k++)
        {
            if (cancellationToken.IsCancellationRequested)
                return RejectDft(FiniteSignalStatus64.Cancelled, inverse);
            var real = 0d;
            var imaginary = 0d;
            for (var n = 0; n < length; n++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return RejectDft(FiniteSignalStatus64.Cancelled, inverse);
                var angle = sign * 2d * global::System.Math.PI * k * n / length;
                var cosine = global::System.Math.Cos(angle);
                var sine = global::System.Math.Sin(angle);
                var value = samples[n];
                real += value.Real * cosine - value.Imaginary * sine;
                imaginary += value.Real * sine + value.Imaginary * cosine;
                if (!double.IsFinite(real) || !double.IsFinite(imaginary))
                    return RejectDft(FiniteSignalStatus64.NonFiniteResult, inverse);
            }
            if (inverse)
            {
                real /= length;
                imaginary /= length;
            }
            if (global::System.Math.Abs(real) > MaxAbsResultComponent ||
                global::System.Math.Abs(imaginary) > MaxAbsResultComponent)
                return RejectDft(FiniteSignalStatus64.OutsideQualifiedRange, inverse);
            bins[k] = new Complex(real, imaginary);
        }
        if (cancellationToken.IsCancellationRequested)
            return RejectDft(FiniteSignalStatus64.Cancelled, inverse);
        return new(FiniteSignalStatus64.Success, bins, inverse, RuleId, false);
    }

    public static FiniteLinearConvolutionOutcome64 LinearConvolution(
        double[]? first, double[]? second,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return RejectConvolution(FiniteSignalStatus64.Cancelled);
        var status = CheckRealSamples(first);
        if (status != FiniteSignalStatus64.Success) return RejectConvolution(status);
        status = CheckRealSamples(second);
        if (status != FiniteSignalStatus64.Success) return RejectConvolution(status);

        var result = new double[first!.Length + second!.Length - 1];
        for (var k = 0; k < result.Length; k++)
        {
            if (cancellationToken.IsCancellationRequested)
                return RejectConvolution(FiniteSignalStatus64.Cancelled);
            var begin = global::System.Math.Max(0, k - second.Length + 1);
            var end = global::System.Math.Min(k, first.Length - 1);
            var total = 0d;
            for (var i = begin; i <= end; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return RejectConvolution(FiniteSignalStatus64.Cancelled);
                total += first[i] * second[k - i];
                if (!double.IsFinite(total))
                    return RejectConvolution(FiniteSignalStatus64.NonFiniteResult);
                if (global::System.Math.Abs(total) > MaxAbsResultComponent)
                    return RejectConvolution(FiniteSignalStatus64.OutsideQualifiedRange);
            }
            result[k] = total;
        }
        if (cancellationToken.IsCancellationRequested)
            return RejectConvolution(FiniteSignalStatus64.Cancelled);
        return new(FiniteSignalStatus64.Success, result, RuleId, false);
    }

    private static FiniteSignalStatus64 CheckComplexSamples(Complex[]? samples)
    {
        if (samples is null || samples.Length is < 1 or > MaxSamplesPerInput)
            return FiniteSignalStatus64.InvalidLength;
        foreach (var value in samples)
        {
            if (!double.IsFinite(value.Real) || !double.IsFinite(value.Imaginary))
                return FiniteSignalStatus64.InvalidInput;
            if (global::System.Math.Abs(value.Real) > MaxAbsInputComponent ||
                global::System.Math.Abs(value.Imaginary) > MaxAbsInputComponent)
                return FiniteSignalStatus64.OutsideQualifiedRange;
        }
        return FiniteSignalStatus64.Success;
    }

    private static FiniteSignalStatus64 CheckRealSamples(double[]? samples)
    {
        if (samples is null || samples.Length is < 1 or > MaxSamplesPerInput)
            return FiniteSignalStatus64.InvalidLength;
        foreach (var value in samples)
        {
            if (!double.IsFinite(value)) return FiniteSignalStatus64.InvalidInput;
            if (global::System.Math.Abs(value) > MaxAbsInputComponent)
                return FiniteSignalStatus64.OutsideQualifiedRange;
        }
        return FiniteSignalStatus64.Success;
    }

    private static FiniteDirectDftOutcome64 RejectDft(FiniteSignalStatus64 status, bool inverse) =>
        new(status, null, inverse, RuleId, false);

    private static FiniteLinearConvolutionOutcome64 RejectConvolution(FiniteSignalStatus64 status) =>
        new(status, null, RuleId, false);
}
