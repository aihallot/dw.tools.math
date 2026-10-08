namespace dw.quantities;

/// <summary>
/// Pure exact symmetric comparison policy for physical quantities.
/// Absolute tolerance has the compared dimension (or scalar zero);
/// relative tolerance is a non-negative dimensionless exact rational.
/// </summary>
public readonly record struct QuantityTolerance(Quantity Absolute, ExactRational Relative)
{
    public static QuantityTolerance FromQuantities(Quantity absolute, Quantity relative)
    {
        if (relative.Dimension != DimensionVector.Scalar)
            throw new ArgumentException("Relative tolerance must be dimensionless.", nameof(relative));
        if (relative.Value < ExactRational.Zero)
            throw new ArgumentOutOfRangeException(nameof(relative), "Relative tolerance must not be negative.");
        return new QuantityTolerance(absolute, relative.Value);
    }
}

public static class ExactQuantityComparison
{
    public static bool AreEquivalent(Quantity left, Quantity right, QuantityTolerance tolerance)
    {
        if (left.Dimension != right.Dimension)
            throw new ArgumentException("Compared quantities must have identical dimensions.", nameof(right));
        if (tolerance.Relative < ExactRational.Zero)
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Relative tolerance must not be negative.");
        if (tolerance.Absolute.Value < ExactRational.Zero)
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Absolute tolerance must not be negative.");

        if (tolerance.Absolute.Dimension != left.Dimension &&
            !(tolerance.Absolute.Dimension == DimensionVector.Scalar &&
              tolerance.Absolute.Value == ExactRational.Zero))
            throw new ArgumentException(
                "Absolute tolerance must have the compared dimension; scalar zero is admitted.",
                nameof(tolerance));

        return ExactSymmetricBound.AreWithin(
            left.Value, right.Value, tolerance.Absolute.Value, tolerance.Relative);
    }
}

/// <summary>
/// Tagged temperatures must already be expressed on a common base scale
/// (for example Kelvin), using UnitDefinition's typed ToBase conversion.
/// Absolute tolerance is always an interval; never an absolute temperature.
/// </summary>
public static class ExactTemperatureComparison
{
    public static bool AreEquivalent(
        TemperatureMeasurement left,
        TemperatureMeasurement right,
        TemperatureMeasurement absoluteTolerance,
        ExactRational relativeTolerance)
    {
        Validate(left, nameof(left));
        Validate(right, nameof(right));
        Validate(absoluteTolerance, nameof(absoluteTolerance));
        if (left.Kind != right.Kind)
            throw new ArgumentException("Cannot compare an absolute temperature with an interval.", nameof(right));
        if (absoluteTolerance.Kind != TemperatureKind.Interval)
            throw new ArgumentException("Absolute tolerance must be a temperature interval, not a point.",
                nameof(absoluteTolerance));
        if (absoluteTolerance.Value < ExactRational.Zero)
            throw new ArgumentOutOfRangeException(nameof(absoluteTolerance), "Absolute tolerance must be non-negative.");
        if (relativeTolerance < ExactRational.Zero)
            throw new ArgumentOutOfRangeException(nameof(relativeTolerance), "Relative tolerance must be non-negative.");

        return ExactSymmetricBound.AreWithin(
            left.Value, right.Value, absoluteTolerance.Value, relativeTolerance);
    }

    private static void Validate(TemperatureMeasurement value, string paramName)
    {
        if (value.Kind is not (TemperatureKind.Absolute or TemperatureKind.Interval))
            throw new ArgumentOutOfRangeException(paramName, "Unknown temperature kind.");
    }
}

internal static class ExactSymmetricBound
{
    public static bool AreWithin(
        ExactRational left, ExactRational right,
        ExactRational absoluteTolerance, ExactRational relativeTolerance)
    {
        var distance = (left - right).Abs();
        var leftMagnitude = left.Abs();
        var rightMagnitude = right.Abs();
        var maxMagnitude = leftMagnitude >= rightMagnitude ? leftMagnitude : rightMagnitude;
        var relativeBound = relativeTolerance * maxMagnitude;
        var bound = absoluteTolerance >= relativeBound ? absoluteTolerance : relativeBound;
        return distance <= bound;
    }
}
