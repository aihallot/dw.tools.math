namespace dw.quantities;

public readonly record struct DimensionVector(
    int Length,
    int Mass,
    int Time,
    int ElectricCurrent,
    int Temperature,
    int AmountOfSubstance,
    int LuminousIntensity,
    int Information = 0)
{
    public static DimensionVector Scalar { get; } = new(0, 0, 0, 0, 0, 0, 0);

    public static DimensionVector LengthDimension { get; } = new(1, 0, 0, 0, 0, 0, 0);

    public static DimensionVector MassDimension { get; } = new(0, 1, 0, 0, 0, 0, 0);

    public static DimensionVector TimeDimension { get; } = new(0, 0, 1, 0, 0, 0, 0);

    public static DimensionVector TemperatureDimension { get; } = new(0, 0, 0, 0, 1, 0, 0);

    public static DimensionVector InformationDimension { get; } = new(0, 0, 0, 0, 0, 0, 0, 1);

    public static DimensionVector operator +(DimensionVector left, DimensionVector right) =>
        new(
            left.Length + right.Length,
            left.Mass + right.Mass,
            left.Time + right.Time,
            left.ElectricCurrent + right.ElectricCurrent,
            left.Temperature + right.Temperature,
            left.AmountOfSubstance + right.AmountOfSubstance,
            left.LuminousIntensity + right.LuminousIntensity,
            left.Information + right.Information);

    public static DimensionVector operator -(DimensionVector left, DimensionVector right) =>
        new(
            left.Length - right.Length,
            left.Mass - right.Mass,
            left.Time - right.Time,
            left.ElectricCurrent - right.ElectricCurrent,
            left.Temperature - right.Temperature,
            left.AmountOfSubstance - right.AmountOfSubstance,
            left.LuminousIntensity - right.LuminousIntensity,
            left.Information - right.Information);
}
