using System;
using System.Collections.Generic;
using System.Linq;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// Immutable unweighted paired-sample covariance and Pearson correlation.
/// Each reported moment is approximate finite binary64, not an exact rational.
/// </summary>
public sealed record FiniteBivariateStatistics64(
    int Count,
    double MeanX,
    double MeanY,
    double PopulationCovariance,
    double SampleCovariance,
    double PearsonCorrelation)
{
    public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
}

/// <summary>
/// Bounded two-pass centered covariance and Pearson correlation for two
/// aligned finite vectors. Undefined/degenerate-result typing is reserved
/// for M3-W02-C02-T2; this T1 entry point refuses that input.
/// </summary>
public static class CorrelationStatistics64
{
    public const int MaximumPairs = DescriptiveStatistics64.MaximumObservations;

    public static FiniteBivariateStatistics64 Analyze(
        IEnumerable<double> first, IEnumerable<double> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        var x = first.Take(MaximumPairs + 1).ToArray();
        var y = second.Take(MaximumPairs + 1).ToArray();
        if (x.Length > MaximumPairs || y.Length > MaximumPairs)
            throw new ArgumentOutOfRangeException(nameof(first),
                "At most 4096 paired observations are admitted.");
        if (x.Length != y.Length || x.Length < 2)
            throw new ArgumentException("Two aligned vectors with at least two observations are required.");
        foreach (var v in x) DescriptiveStatistics64.RequireFinite(v);
        foreach (var v in y) DescriptiveStatistics64.RequireFinite(v);

        var firstReference = x[0];
        var secondReference = y[0];
        double sx = 0d, cx = 0d, sy = 0d, cy = 0d;
        for (var i = 0; i < x.Length; i++)
        {
            DescriptiveStatistics64.Accumulate(
                DescriptiveStatistics64.RequireFinite(x[i] - firstReference), ref sx, ref cx);
            DescriptiveStatistics64.Accumulate(
                DescriptiveStatistics64.RequireFinite(y[i] - secondReference), ref sy, ref cy);
        }

        var centeredX = DescriptiveStatistics64.RequireFinite(
            DescriptiveStatistics64.RequireFinite(sx + cx) / x.Length);
        var centeredY = DescriptiveStatistics64.RequireFinite(
            DescriptiveStatistics64.RequireFinite(sy + cy) / x.Length);
        var meanX = DescriptiveStatistics64.RequireFinite(firstReference + centeredX);
        var meanY = DescriptiveStatistics64.RequireFinite(secondReference + centeredY);
        double xy = 0d, cxy = 0d, xx = 0d, cxx = 0d, yy = 0d, cyy = 0d;

        for (var i = 0; i < x.Length; i++)
        {
            var dx = DescriptiveStatistics64.RequireFinite(
                DescriptiveStatistics64.RequireFinite(x[i] - firstReference) - centeredX);
            var dy = DescriptiveStatistics64.RequireFinite(
                DescriptiveStatistics64.RequireFinite(y[i] - secondReference) - centeredY);
            DescriptiveStatistics64.Accumulate(
                DescriptiveStatistics64.RequireFinite(dx * dy), ref xy, ref cxy);
            DescriptiveStatistics64.Accumulate(
                DescriptiveStatistics64.RequireFinite(dx * dx), ref xx, ref cxx);
            DescriptiveStatistics64.Accumulate(
                DescriptiveStatistics64.RequireFinite(dy * dy), ref yy, ref cyy);
        }

        var cross = DescriptiveStatistics64.RequireFinite(xy + cxy);
        var squareX = DescriptiveStatistics64.RequireFinite(xx + cxx);
        var squareY = DescriptiveStatistics64.RequireFinite(yy + cyy);
        if (squareX <= 0d || squareY <= 0d)
            throw new InvalidOperationException(
                "Pearson correlation has no admitted nonzero variance; typed undefined results are reserved for T2.");
        var denominator = DescriptiveStatistics64.RequireFinite(
            global::System.Math.Sqrt(squareX) * global::System.Math.Sqrt(squareY));
        if (denominator == 0d)
            throw new ArithmeticException("Pearson normalization underflowed to zero.");
        var correlation = DescriptiveStatistics64.RequireFinite(cross / denominator);
        return new(x.Length, meanX, meanY,
            DescriptiveStatistics64.RequireFinite(cross / x.Length),
            DescriptiveStatistics64.RequireFinite(cross / (x.Length - 1)),
            correlation);
    }
}
