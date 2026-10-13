using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using dw.quantities;

namespace Dw.Tools.Math.Statistics;

/// <summary>Two deliberately distinct sample quantile definitions.</summary>
public enum QuantileConvention64
{
    Type7,
    NearestRank
}

/// <summary>
/// Bounded sample quantiles. Exact rationals and finite binary64 have separate
/// entry points; neither is silently converted into the other.
/// </summary>
public static class QuantileStatistics64
{
    public const int MaximumObservations = DescriptiveStatistics64.MaximumObservations;
    public const int MaximumExactOperandBits = DescriptiveStatistics64.MaximumExactOperandBits;

    /// <summary>
    /// Type-7 linearly interpolates at (n-1)*p; nearest-rank returns sorted
    /// max(1, ceiling(n*p)), using the first observation for p=0.
    /// </summary>
    public static double Evaluate(
        IEnumerable<double> observations, double probability, QuantileConvention64 convention)
    {
        ArgumentNullException.ThrowIfNull(observations);
        if (!double.IsFinite(probability) || probability < 0d || probability > 1d)
            throw new ArgumentOutOfRangeException(nameof(probability));
        RequireConvention(convention);
        var sorted = observations.Take(MaximumObservations + 1).ToArray();
        RequireCount(sorted.Length);
        foreach (var value in sorted)
            DescriptiveStatistics64.RequireFinite(value);
        Array.Sort(sorted);

        if (convention == QuantileConvention64.NearestRank)
        {
            var rank = probability == 0d ? 0 :
                (int)global::System.Math.Ceiling(probability * sorted.Length) - 1;
            return sorted[global::System.Math.Clamp(rank, 0, sorted.Length - 1)];
        }

        var position = (sorted.Length - 1) * probability;
        var lower = (int)global::System.Math.Floor(position);
        if (lower == sorted.Length - 1)
            return sorted[lower];
        var fraction = position - lower;
        var span = DescriptiveStatistics64.RequireFinite(sorted[lower + 1] - sorted[lower]);
        return DescriptiveStatistics64.RequireFinite(sorted[lower] +
            DescriptiveStatistics64.RequireFinite(fraction * span));
    }

    /// <summary>
    /// Exact rational Type-7 interpolation and nearest-rank with a rational
    /// probability; every operand and reported intermediate is bit-budgeted.
    /// </summary>
    public static ExactRational EvaluateExact(
        IEnumerable<ExactRational> observations, ExactRational probability,
        QuantileConvention64 convention)
    {
        ArgumentNullException.ThrowIfNull(observations);
        RequireExactBudget(probability);
        if (probability < ExactRational.Zero || probability > ExactRational.One)
            throw new ArgumentOutOfRangeException(nameof(probability));
        RequireConvention(convention);
        var sorted = observations.Take(MaximumObservations + 1).ToArray();
        RequireCount(sorted.Length);
        foreach (var value in sorted)
            RequireExactBudget(value);
        Array.Sort(sorted);

        if (convention == QuantileConvention64.NearestRank)
        {
            if (probability == ExactRational.Zero)
                return sorted[0];
            var scaled = probability.Numerator * sorted.Length;
            var rank = BigInteger.DivRem(scaled, probability.Denominator, out var rem);
            if (!rem.IsZero) rank++;
            return sorted[(int)rank - 1];
        }

        var scaledPosition = probability.Numerator * (sorted.Length - 1);
        var integer = BigInteger.DivRem(scaledPosition, probability.Denominator,
            out var remainder);
        var lower = (int)integer;
        if (remainder.IsZero || lower == sorted.Length - 1)
            return sorted[lower];
        var fraction = new ExactRational(remainder, probability.Denominator);
        var delta = sorted[lower + 1] - sorted[lower];
        RequireExactBudget(delta);
        var scaledDelta = fraction * delta;
        RequireExactBudget(scaledDelta);
        var result = sorted[lower] + scaledDelta;
        RequireExactBudget(result);
        return result;
    }

    private static void RequireConvention(QuantileConvention64 convention)
    {
        if (!Enum.IsDefined(convention))
            throw new ArgumentOutOfRangeException(nameof(convention));
    }

    private static void RequireCount(int count)
    {
        if (count == 0)
            throw new ArgumentException("At least one observation is required.");
        if (count > MaximumObservations)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Quantiles admit at most 4096 observations.");
    }

    private static void RequireExactBudget(ExactRational value)
    {
        if (BigInteger.Abs(value.Numerator).GetBitLength() > MaximumExactOperandBits ||
            value.Denominator.GetBitLength() > MaximumExactOperandBits)
            throw new ArgumentOutOfRangeException(nameof(value),
                "An exact quantile operand or intermediate exceeds 4096 bits.");
    }
}
