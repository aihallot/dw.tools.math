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
            checked(left.Length + right.Length),
            checked(left.Mass + right.Mass),
            checked(left.Time + right.Time),
            checked(left.ElectricCurrent + right.ElectricCurrent),
            checked(left.Temperature + right.Temperature),
            checked(left.AmountOfSubstance + right.AmountOfSubstance),
            checked(left.LuminousIntensity + right.LuminousIntensity),
            checked(left.Information + right.Information));

    public static DimensionVector operator -(DimensionVector left, DimensionVector right) =>
        new(
            checked(left.Length - right.Length),
            checked(left.Mass - right.Mass),
            checked(left.Time - right.Time),
            checked(left.ElectricCurrent - right.ElectricCurrent),
            checked(left.Temperature - right.Temperature),
            checked(left.AmountOfSubstance - right.AmountOfSubstance),
            checked(left.LuminousIntensity - right.LuminousIntensity),
            checked(left.Information - right.Information));
}
