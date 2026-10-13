using System;
using System.Collections.Generic;
using System.Linq;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// Missing denotes a null observation only. NaN or infinity are never missing.
/// </summary>
public enum MissingObservationPolicy64
{
    Reject,
    Skip
}

/// <summary>
/// Frequency is an integer repetition count, not a continuous reliability weight.
/// Zero or negative frequencies are never silently discarded.
/// </summary>
public readonly record struct FrequencyWeightedObservation64(double? Value, int Frequency);

/// <summary>
/// Counts are per provided row unless named EffectiveCount, which is the sum
/// of admitted nonmissing frequencies. All results are finite binary64.
/// </summary>
public sealed record FrequencyWeightedDescriptiveResult64(
    int ProvidedCount,
    int IncludedCount,
    int MissingCount,
    int EffectiveCount,
    double Sum,
    double Mean,
    double PopulationVariance,
    double? SampleVariance,
    double Minimum,
    double Maximum);

/// <summary>
/// An explicitly frequency-weighted, bounded descriptive statistics contract.
/// It never interprets arbitrary real-valued weights as replicated samples.
/// </summary>
public static class FrequencyWeightedStatistics64
{
    public const int MaximumRows = DescriptiveStatistics64.MaximumObservations;
    public const int MaximumEffectiveCount = DescriptiveStatistics64.MaximumObservations;

    public static FrequencyWeightedDescriptiveResult64 Analyze(
        IEnumerable<FrequencyWeightedObservation64> observations,
        MissingObservationPolicy64 missingPolicy = MissingObservationPolicy64.Reject)
    {
        ArgumentNullException.ThrowIfNull(observations);
        if (!Enum.IsDefined(missingPolicy))
            throw new ArgumentOutOfRangeException(nameof(missingPolicy),
                "Only explicit Reject or Skip missing policies are admitted.");

        var input = observations.Take(MaximumRows + 1).ToArray();
        if (input.Length == 0)
            throw new ArgumentException("At least one record is required.", nameof(observations));
        if (input.Length > MaximumRows)
            throw new ArgumentOutOfRangeException(nameof(observations),
                "The 4096-record source budget was exceeded.");

        var included = new List<FrequencyWeightedObservation64>(input.Length);
        var missing = 0;
        var effectiveCount = 0;
        double sum = 0d, correction = 0d;
        double minimum = double.PositiveInfinity, maximum = double.NegativeInfinity;

        foreach (var item in input)
        {
            if (item.Frequency < 1 || item.Frequency > MaximumEffectiveCount)
                throw new ArgumentOutOfRangeException(nameof(observations),
                    "Every record requires a frequency in 1..4096.");

            if (!item.Value.HasValue)
            {
                if (missingPolicy == MissingObservationPolicy64.Reject)
                    throw new ArgumentException(
                        "Missing null requires an explicit Skip policy.", nameof(observations));
                missing++;
                continue;
            }

            var value = DescriptiveStatistics64.RequireFinite(item.Value.Value);
            if (item.Frequency > MaximumEffectiveCount - effectiveCount)
                throw new ArgumentOutOfRangeException(nameof(observations),
                    "The sum of admitted frequencies exceeds 4096.");

            effectiveCount += item.Frequency;
            included.Add(item);
            var weightedValue = DescriptiveStatistics64.RequireFinite(value * item.Frequency);
            DescriptiveStatistics64.Accumulate(weightedValue, ref sum, ref correction);
            minimum = global::System.Math.Min(minimum, value);
            maximum = global::System.Math.Max(maximum, value);
        }

        if (included.Count == 0)
            throw new ArgumentException("All input records are missing.", nameof(observations));

        var total = DescriptiveStatistics64.RequireFinite(sum + correction);
        var mean = DescriptiveStatistics64.RequireFinite(total / effectiveCount);

        // Center at an admitted datum rather than forming E[X^2]-E[X]^2.
        var reference = included[0].Value!.Value;
        double deviations = 0d, deviationCorrection = 0d;
        foreach (var item in included)
        {
            var delta = DescriptiveStatistics64.RequireFinite(item.Value!.Value - reference);
            var weightedDelta = DescriptiveStatistics64.RequireFinite(delta * item.Frequency);
            DescriptiveStatistics64.Accumulate(weightedDelta,
                ref deviations, ref deviationCorrection);
        }

        var centeredMean = DescriptiveStatistics64.RequireFinite(
            DescriptiveStatistics64.RequireFinite(deviations + deviationCorrection)
            / effectiveCount);
        double squaredSum = 0d, squaredCorrection = 0d;
        foreach (var item in included)
        {
            var delta = DescriptiveStatistics64.RequireFinite(item.Value!.Value - reference);
            var offset = DescriptiveStatistics64.RequireFinite(delta - centeredMean);
            var square = DescriptiveStatistics64.RequireFinite(offset * offset);
            var weightedSquare = DescriptiveStatistics64.RequireFinite(
                square * item.Frequency);
            DescriptiveStatistics64.Accumulate(weightedSquare,
                ref squaredSum, ref squaredCorrection);
        }

        var centralSquareSum = DescriptiveStatistics64.RequireFinite(
            squaredSum + squaredCorrection);
        var population = DescriptiveStatistics64.RequireFinite(
            centralSquareSum / effectiveCount);
        double? sample = effectiveCount > 1
            ? DescriptiveStatistics64.RequireFinite(
                centralSquareSum / (effectiveCount - 1))
            : null;
        return new(input.Length, included.Count, missing, effectiveCount,
            total, mean, population, sample, minimum, maximum);
    }
}
