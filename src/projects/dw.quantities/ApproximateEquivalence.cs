namespace dw.quantities;

public readonly record struct ApproximationTolerance
{
    public ApproximationTolerance(ExactRational absolute, ExactRational relative)
    {
        if (absolute < ExactRational.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(absolute));
        }

        if (relative < ExactRational.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(relative));
        }

        Absolute = absolute;
        Relative = relative;
    }

    public ExactRational Absolute { get; }

    public ExactRational Relative { get; }

    public static ApproximationTolerance Exact { get; } =
        new(ExactRational.Zero, ExactRational.Zero);
}

public static class ApproximateEquivalence
{
    public static bool AreEquivalent(
        ExactRational left,
        ExactRational right,
        ApproximationTolerance tolerance)
    {
        var difference = (left - right).Abs();
        var largestMagnitude = Max(left.Abs(), right.Abs());
        var allowedDifference = Max(tolerance.Absolute, tolerance.Relative * largestMagnitude);
        return difference <= allowedDifference;
    }

    public static bool AreEquivalent(
        Quantity left,
        Quantity right,
        ApproximationTolerance tolerance) =>
        left.Dimension == right.Dimension && AreEquivalent(left.Value, right.Value, tolerance);

    private static ExactRational Max(ExactRational left, ExactRational right) =>
        left >= right ? left : right;
}
