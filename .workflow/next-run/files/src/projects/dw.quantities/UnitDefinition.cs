using System.Collections.Immutable;

namespace dw.quantities;

public enum UnitSystem
{
    Si,
    MetricInternational,
    UsCustomary,
    BritishImperial,
    AustralianCulinary,
    IecBinary,
}

public enum UnitTransformKind
{
    Linear,
    AbsoluteTemperature,
}

public sealed record UnitDefinition
{
    public UnitDefinition(
        string id,
        string symbol,
        string name,
        UnitSystem system,
        DimensionVector dimension,
        ExactRational scaleToBase,
        ExactRational offsetToBase,
        UnitTransformKind transformKind,
        params string[] aliases)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (scaleToBase.Numerator.IsZero)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleToBase), "A unit scale cannot be zero.");
        }

        Id = id;
        Symbol = symbol;
        Name = name;
        System = system;
        Dimension = dimension;
        ScaleToBase = scaleToBase;
        OffsetToBase = offsetToBase;
        TransformKind = transformKind;
        Aliases = aliases
            .Where(alias => !string.IsNullOrWhiteSpace(alias))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToImmutableArray();
    }

    public string Id { get; }
    public string Symbol { get; }
    public string Name { get; }
    public UnitSystem System { get; }
    public DimensionVector Dimension { get; }
    public ExactRational ScaleToBase { get; }
    public ExactRational OffsetToBase { get; }
    public UnitTransformKind TransformKind { get; }
    public ImmutableArray<string> Aliases { get; }

    public ExactRational ToBase(ExactRational value) => value * ScaleToBase + OffsetToBase;

    public ExactRational FromBase(ExactRational value) => (value - OffsetToBase) / ScaleToBase;

    /// <summary>
    /// Converts a tagged absolute temperature or interval without applying an
    /// affine offset to intervals. The legacy scalar conversion remains available
    /// for existing callers; use this overload when temperature semantics matter.
    /// </summary>
    public TemperatureMeasurement ToBase(TemperatureMeasurement value)
    {
        RequireTemperatureConversion();
        return value.Kind switch
        {
            TemperatureKind.Absolute => TemperatureMeasurement.Absolute(ToBase(value.Value)),
            TemperatureKind.Interval => TemperatureMeasurement.Interval(value.Value * ScaleToBase),
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unknown temperature kind.")
        };
    }

    public TemperatureMeasurement FromBase(TemperatureMeasurement value)
    {
        RequireTemperatureConversion();
        return value.Kind switch
        {
            TemperatureKind.Absolute => TemperatureMeasurement.Absolute(FromBase(value.Value)),
            TemperatureKind.Interval => TemperatureMeasurement.Interval(value.Value / ScaleToBase),
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unknown temperature kind.")
        };
    }

    private void RequireTemperatureConversion()
    {
        if (Dimension != DimensionVector.TemperatureDimension)
            throw new InvalidOperationException("Temperature conversion requires the temperature dimension.");
        if (TransformKind == UnitTransformKind.Linear && OffsetToBase != ExactRational.Zero)
            throw new InvalidOperationException("A linear temperature unit must have zero affine offset.");
    }
}

public enum TemperatureKind
{
    Interval,
    Absolute
}

/// <summary>
/// Distinguishes affine temperature points from temperature differences.
/// Arithmetic rejects absolute + absolute and interval - absolute.
/// </summary>
public readonly record struct TemperatureMeasurement(ExactRational Value, TemperatureKind Kind)
{
    public static TemperatureMeasurement Absolute(ExactRational value) => new(value, TemperatureKind.Absolute);
    public static TemperatureMeasurement Interval(ExactRational value) => new(value, TemperatureKind.Interval);

    public TemperatureMeasurement Add(TemperatureMeasurement other)
    {
        Validate();
        other.Validate();
        if (Kind == TemperatureKind.Absolute && other.Kind == TemperatureKind.Absolute)
            throw new InvalidOperationException("Cannot add two absolute temperatures.");
        return new TemperatureMeasurement(
            Value + other.Value,
            Kind == TemperatureKind.Absolute || other.Kind == TemperatureKind.Absolute
                ? TemperatureKind.Absolute : TemperatureKind.Interval);
    }

    public TemperatureMeasurement Subtract(TemperatureMeasurement other)
    {
        Validate();
        other.Validate();
        if (Kind == TemperatureKind.Interval && other.Kind == TemperatureKind.Absolute)
            throw new InvalidOperationException("Cannot subtract an absolute temperature from an interval.");
        return new TemperatureMeasurement(
            Value - other.Value,
            Kind == TemperatureKind.Absolute && other.Kind == TemperatureKind.Interval
                ? TemperatureKind.Absolute : TemperatureKind.Interval);
    }

    private void Validate()
    {
        if (Kind is not (TemperatureKind.Absolute or TemperatureKind.Interval))
            throw new ArgumentOutOfRangeException(nameof(Kind), "Unknown temperature kind.");
    }
}

