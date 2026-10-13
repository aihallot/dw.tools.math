using System;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// Bounded finite binary64 Bernoulli mass, integer-support CDF, inverse-CDF
/// quantile and deterministic sampling, without an external RNG provider.
/// </summary>
public sealed class BernoulliDistribution64
{
    public BernoulliDistribution64(double probabilityOfOne)
    {
        if (!double.IsFinite(probabilityOfOne) ||
            probabilityOfOne < 0d || probabilityOfOne > 1d)
            throw new ArgumentOutOfRangeException(nameof(probabilityOfOne),
                "Bernoulli probability must be finite and in [0,1].");
        ProbabilityOfOne = probabilityOfOne;
    }

    public double ProbabilityOfOne { get; }

    /// <summary>Mass on {0,1}; all other integer outcomes have zero mass.</summary>
    public double ProbabilityMass(int value) => value switch
    {
        0 => 1d - ProbabilityOfOne,
        1 => ProbabilityOfOne,
        _ => 0d
    };

    /// <summary>P(X less than or equal to value), for integer thresholds.</summary>
    public double CumulativeProbability(int value) => value switch
    {
        < 0 => 0d,
        0 => 1d - ProbabilityOfOne,
        _ => 1d
    };

    /// <summary>
    /// Smallest positive-mass support value whose CDF is at least q.
    /// At q=0 this deliberately selects the lowest point of positive mass,
    /// so p=1 has quantile 1 rather than a zero-mass value 0.
    /// </summary>
    public int Quantile(double cumulativeProbability)
    {
        if (!double.IsFinite(cumulativeProbability) ||
            cumulativeProbability < 0d || cumulativeProbability > 1d)
            throw new ArgumentOutOfRangeException(nameof(cumulativeProbability),
                "Quantile probability must be finite and in [0,1].");

        if (ProbabilityOfOne == 1d)
            return 1;
        return cumulativeProbability <= 1d - ProbabilityOfOne ? 0 : 1;
    }

    /// <summary>
    /// Consume exactly one SplitMix64 draw even when p is zero or one,
    /// preserving the caller-visible counter and subsequent stream position.
    /// </summary>
    public int Sample(SplitMix64Random generator)
    {
        ArgumentNullException.ThrowIfNull(generator);
        return generator.NextUnitInterval() < ProbabilityOfOne ? 1 : 0;
    }
}
