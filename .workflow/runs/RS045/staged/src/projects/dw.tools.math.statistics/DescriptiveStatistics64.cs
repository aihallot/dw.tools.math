using System.Numerics;
using dw.quantities;

namespace Dw.Tools.Math.Statistics;

public enum DescriptivePrecisionKind { FiniteBinary64 }

/// <summary>Finite approximate aggregates. Sample variance is absent for a singleton.</summary>
public sealed record FiniteDescriptiveStatistics64(
    int Count, double Sum, double Mean, double PopulationVariance,
    double? SampleVariance, double Minimum, double Maximum)
{
    public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
}

/// <summary>Rational aggregates never undergo silent binary64 conversion.</summary>
public sealed record ExactDescriptiveStatistics(
    int Count, ExactRational Sum, ExactRational Mean,
    ExactRational Minimum, ExactRational Maximum);

/// <summary>
/// Bounded provider-neutral descriptive aggregates. No implicit missing-value
/// policy, weighting, distribution assumptions, or third-party provider loading.
/// </summary>
public static class DescriptiveStatistics64
{
    public const int MaximumObservations = 4096;
    public const int MaximumExactOperandBits = 4096;

    /// <summary>Compensated sum with centered two-pass population/sample variance.</summary>
    public static FiniteDescriptiveStatistics64 Analyze(IEnumerable<double> observations)
    {
        ArgumentNullException.ThrowIfNull(observations);
        var values = observations.Take(MaximumObservations + 1).ToArray();
        RequireCount(values.Length);

        double sum = 0d, correction = 0d;
        double minimum = double.PositiveInfinity, maximum = double.NegativeInfinity;
        foreach (var value in values)
        {
            RequireFinite(value);
            Accumulate(value, ref sum, ref correction);
            minimum = global::System.Math.Min(minimum, value);
            maximum = global::System.Math.Max(maximum, value);
        }
        var total = RequireFinite(sum + correction);
        var mean = RequireFinite(total / values.Length);

        // Subtract a real input, avoiding cancellation in the variance oracle.
        var reference = values[0];
        double deviationSum = 0d, deviationCorrection = 0d;
        foreach (var value in values)
            Accumulate(RequireFinite(value - reference),
                ref deviationSum, ref deviationCorrection);
        var centeredMean = RequireFinite(
            RequireFinite(deviationSum + deviationCorrection) / values.Length);
        double squaredSum = 0d, squaredCorrection = 0d;
        foreach (var value in values)
        {
            var offset = RequireFinite(RequireFinite(value - reference) - centeredMean);
            Accumulate(RequireFinite(offset * offset),
                ref squaredSum, ref squaredCorrection);
        }
        var squaredTotal = RequireFinite(squaredSum + squaredCorrection);
        var population = RequireFinite(squaredTotal / values.Length);
        double? sample = values.Length > 1
            ? RequireFinite(squaredTotal / (values.Length - 1))
            : null;
        return new(values.Length,total,mean,population,sample,minimum,maximum);
    }

    /// <summary>Bounded exact sum, mean and extrema over normalized rationals.</summary>
    public static ExactDescriptiveStatistics AnalyzeExact(
        IEnumerable<ExactRational> observations)
    {
        ArgumentNullException.ThrowIfNull(observations);
        var values = observations.Take(MaximumObservations + 1).ToArray();
        RequireCount(values.Length);

        var sum = ExactRational.Zero;
        var minimum = values[0];
        var maximum = values[0];
        foreach (var value in values)
        {
            RequireExactBudget(value);
            sum += value;
            RequireExactBudget(sum);
            if (value < minimum) minimum = value;
            if (value > maximum) maximum = value;
        }
        var mean = sum / new ExactRational(values.Length, BigInteger.One);
        RequireExactBudget(mean);
        return new(values.Length,sum,mean,minimum,maximum);
    }

    private static void RequireCount(int count)
    {
        if (count == 0)
            throw new ArgumentException("At least one observation is required.");
        if (count > MaximumObservations)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Only bounded sequences of at most 4096 observations are admitted.");
    }

    private static void RequireExactBudget(ExactRational value)
    {
        if (BigInteger.Abs(value.Numerator).GetBitLength() > MaximumExactOperandBits ||
            value.Denominator.GetBitLength() > MaximumExactOperandBits)
            throw new ArgumentOutOfRangeException(nameof(value),
                "The exact numerator or denominator exceeds 4096 bits.");
    }

    internal static double RequireFinite(double value)
    {
        if (!double.IsFinite(value))
            throw new ArithmeticException(
                "Nonfinite input, intermediate, or result is not admitted.");
        return value;
    }

    internal static void Accumulate(double value, ref double sum, ref double correction)
    {
        var next = RequireFinite(sum + value);
        var drift = global::System.Math.Abs(sum) >= global::System.Math.Abs(value)
            ? (sum - next) + value : (value - next) + sum;
        correction = RequireFinite(correction + drift);
        sum = next;
    }
}
