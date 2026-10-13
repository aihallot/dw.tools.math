using System;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// A continuous uniform law on finite [minimum, maximum], calculated with
/// approximate IEEE binary64 arithmetic. Sampling uses [minimum, maximum).
/// </summary>
public sealed class ContinuousUniformDistribution64
{
    private readonly double _width;
    private readonly double _density;

    public ContinuousUniformDistribution64(double minimum, double maximum)
    {
        if (!double.IsFinite(minimum))
            throw new ArgumentOutOfRangeException(nameof(minimum), "Lower bound must be finite.");
        if (!double.IsFinite(maximum) || maximum <= minimum)
            throw new ArgumentOutOfRangeException(nameof(maximum), "Upper bound must be finite and strictly larger.");
        var width = maximum - minimum;
        if (!double.IsFinite(width))
            throw new ArithmeticException("Uniform width exceeds finite binary64.");
        var density = 1d / width;
        if (!double.IsFinite(density))
            throw new ArithmeticException("Uniform density exceeds finite binary64.");
        Minimum = minimum;
        Maximum = maximum;
        _width = width;
        _density = density;
    }

    public double Minimum { get; }
    public double Maximum { get; }

    /// <summary>
    /// Density is 1/width on the half-open interval and 0 outside, including
    /// at the upper endpoint. Nonfinite arguments always refuse.
    /// </summary>
    public double ProbabilityDensity(double value)
    {
        RequireFinite(value);
        return value >= Minimum && value < Maximum ? _density : 0d;
    }

    /// <summary>Finite approximate CDF with exact endpoint saturation.</summary>
    public double CumulativeProbability(double value)
    {
        RequireFinite(value);
        if (value <= Minimum) return 0d;
        if (value >= Maximum) return 1d;
        return global::System.Math.Clamp((value - Minimum) / _width, 0d, 1d);
    }

    /// <summary>
    /// Continuous generalized inverse on [0,1]. q=1 returns Maximum.
    /// Binary64 rounding may send an interior quantile to an endpoint.
    /// </summary>
    public double Quantile(double probability)
    {
        if (!double.IsFinite(probability) || probability < 0d || probability > 1d)
            throw new ArgumentOutOfRangeException(nameof(probability),
                "Quantile probability must be finite and in [0,1].");
        if (probability == 0d) return Minimum;
        if (probability == 1d) return Maximum;
        var result = Minimum + probability * _width;
        if (!double.IsFinite(result))
            throw new ArithmeticException("Uniform quantile overflowed binary64.");
        return global::System.Math.Clamp(result, Minimum, Maximum);
    }

    /// <summary>
    /// Exactly one PRNG word per sample; rounded-up affine values are clamped
    /// down to the greatest representable value strictly below Maximum.
    /// </summary>
    public double Sample(SplitMix64Random generator)
    {
        ArgumentNullException.ThrowIfNull(generator);
        var value = Quantile(generator.NextUnitInterval());
        return value >= Maximum ? global::System.Math.BitDecrement(Maximum) : value;
    }

    private static void RequireFinite(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value),
                "Uniform evaluation requires finite binary64.");
    }
}
