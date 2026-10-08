using dw.quantities;

namespace dw.quantities.standard;

/// <summary>
/// Pure source-to-base-to-target conversion. No locale, presentation or host policy.
/// </summary>
public static class PureUnitConverter
{
    public static ExactRational Convert(ExactRational value, UnitDefinition source, UnitDefinition target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        if (source.Dimension != target.Dimension)
            throw new ArgumentException("Source and target dimensions are incompatible.", nameof(target));
        if (source.TransformKind != target.TransformKind)
            throw new ArgumentException("Source and target transform kinds are incompatible.", nameof(target));
        return target.FromBase(source.ToBase(value));
    }

    public static TemperatureMeasurement Convert(TemperatureMeasurement value,
        UnitDefinition source, UnitDefinition target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        if (source.Dimension != DimensionVector.TemperatureDimension ||
            target.Dimension != DimensionVector.TemperatureDimension)
            throw new ArgumentException("Tagged temperature conversion requires temperature dimensions.");
        return target.FromBase(source.ToBase(value));
    }
}
