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
}
