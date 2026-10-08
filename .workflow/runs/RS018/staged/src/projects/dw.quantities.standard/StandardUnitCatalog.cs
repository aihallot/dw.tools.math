using System.Collections.Immutable;
using dw.quantities;

namespace dw.quantities.standard;

/// <summary>
/// BCL-only, versioned catalog. Volume definitions use cubic metres as base.
/// Profiles are explicit; no process culture, OS locale or AURA localization is read.
/// </summary>
public static class StandardUnitCatalog
{
    public const string CatalogVersion = "2026-10-exact-v1";

    public static DimensionVector Volume { get; } = new(3, 0, 0, 0, 0, 0, 0);

    public static ImmutableArray<UnitDefinition> Units { get; } =
    [
        Define("metre", "m", "metre", UnitSystem.Si, DimensionVector.LengthDimension,
            ExactRational.One, "meter", "metres", "meters"),
        Define("centimetre", "cm", "centimetre", UnitSystem.Si, DimensionVector.LengthDimension,
            new ExactRational(1, 100), "centimeter"),
        Define("kilometre", "km", "kilometre", UnitSystem.Si, DimensionVector.LengthDimension,
            new ExactRational(1000, 1), "kilometer"),
        Define("inch", "in", "inch", UnitSystem.BritishImperial, DimensionVector.LengthDimension,
            new ExactRational(127, 5000), "inches"),
        Define("second", "s", "second", UnitSystem.Si, DimensionVector.TimeDimension,
            ExactRational.One, "sec"),
        Define("minute", "min", "minute", UnitSystem.Si, DimensionVector.TimeDimension,
            new ExactRational(60, 1), "minutes"),
        Define("byte", "B", "byte", UnitSystem.IecBinary, DimensionVector.InformationDimension,
            ExactRational.One, "bytes"),
        Define("kibibyte", "KiB", "kibibyte", UnitSystem.IecBinary, DimensionVector.InformationDimension,
            new ExactRational(1024, 1), "kibibytes"),
        Define("us-liquid-cup", "cup", "US liquid cup", UnitSystem.UsCustomary, Volume,
            new ExactRational(2365882365, 10000000000000L), "cups"),
        Define("uk-imperial-cup", "cup", "UK imperial cup", UnitSystem.BritishImperial, Volume,
            new ExactRational(284130625, 1000000000000L), "cups"),
        Define("au-metric-cup", "cup", "Australian metric cup", UnitSystem.AustralianCulinary, Volume,
            new ExactRational(1, 4000), "cups"),
        Define("us-liquid-pint", "pint", "US liquid pint", UnitSystem.UsCustomary, Volume,
            new ExactRational(473176473, 1000000000000L), "pints"),
        Define("uk-imperial-pint", "pint", "UK imperial pint", UnitSystem.BritishImperial, Volume,
            new ExactRational(56826125, 100000000000L), "pints"),
        Define("au-beer-pint-570ml", "pint", "Australian 570 mL beer-serving profile", UnitSystem.AustralianCulinary, Volume,
            new ExactRational(57, 100000), "pints")
    ];

    public static ImmutableDictionary<string, string> PassiveNames { get; } =
        new Dictionary<string,string>(StringComparer.Ordinal)
        {
            ["en"] = "standard physical and culinary units",
            ["fr"] = "unités physiques et culinaires standard"
        }.ToImmutableDictionary(StringComparer.Ordinal);

    private static UnitDefinition Define(string id, string symbol, string name, UnitSystem system,
        DimensionVector dimension, ExactRational scale, params string[] aliases) =>
        new(id, symbol, name, system, dimension, scale, ExactRational.Zero,
            UnitTransformKind.Linear, aliases);
}
