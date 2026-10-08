using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Numerics;

namespace dw.quantities.standard;

/// <summary>
/// Data-faithful AURA 0.2.0 catalogue with host localization removed.
/// Unlike StandardUnitCatalog, this retains inherited unit identifiers and alias data.
/// </summary>
public static class InheritedStandardUnitCatalog
{
    public const string Version = "0.2.0";

    private static readonly DimensionVector Area = new(2, 0, 0, 0, 0, 0, 0);
    private static readonly DimensionVector Volume = new(3, 0, 0, 0, 0, 0, 0);
    private static readonly DimensionVector Speed = new(1, 0, -1, 0, 0, 0, 0);

    public static ImmutableArray<UnitDefinition> All { get; } =
    [
        Linear("si.metre", "m", "metre", UnitSystem.Si, DimensionVector.LengthDimension, "1", "meter", "metre"),
        Linear("si.millimetre", "mm", "millimetre", UnitSystem.Si, DimensionVector.LengthDimension, "0.001", "millimeter", "millimetre"),
        Linear("si.centimetre", "cm", "centimetre", UnitSystem.Si, DimensionVector.LengthDimension, "0.01", "centimeter", "centimetre"),
        Linear("si.kilometre", "km", "kilometre", UnitSystem.Si, DimensionVector.LengthDimension, "1000", "kilometer", "kilometre"),
        Linear("si.kilogram", "kg", "kilogram", UnitSystem.Si, DimensionVector.MassDimension, "1", "kilogramme"),
        Linear("si.gram", "g", "gram", UnitSystem.Si, DimensionVector.MassDimension, "0.001", "gramme"),
        Linear("si.second", "s", "second", UnitSystem.Si, DimensionVector.TimeDimension, "1", "seconde"),
        Linear("si.minute", "min", "minute", UnitSystem.Si, DimensionVector.TimeDimension, "60"),
        Linear("si.hour", "h", "hour", UnitSystem.Si, DimensionVector.TimeDimension, "3600", "hr", "heure"),
        AbsoluteTemperature("si.kelvin", "K", "kelvin", UnitSystem.Si, "1", "0"),
        AbsoluteTemperature("si.celsius", "degC", "degree Celsius", UnitSystem.Si, "1", "273.15", "celsius"),
        Linear("si.square-metre", "m2", "square metre", UnitSystem.Si, Area, "1", "square-meter", "metre-carre"),
        Linear("si.cubic-metre", "m3", "cubic metre", UnitSystem.Si, Volume, "1", "cubic-meter", "metre-cube"),
        Linear("metric.litre", "L", "litre", UnitSystem.MetricInternational, Volume, "0.001", "l", "liter", "litre"),
        Linear("metric.millilitre", "mL", "millilitre", UnitSystem.MetricInternational, Volume, "0.000001", "ml", "cc", "milliliter", "millilitre"),
        Linear("si.metre-per-second", "m/s", "metre per second", UnitSystem.Si, Speed, "1", "meter-per-second"),
        LinearFraction("metric.kilometre-per-hour", "km/h", "kilometre per hour", UnitSystem.MetricInternational, Speed, 5, 18, "kph"),

        .. InformationUnits(),

        Linear("imperial.inch", "in", "inch", UnitSystem.BritishImperial, DimensionVector.LengthDimension, "0.0254", "pouce"),
        Linear("imperial.foot", "ft", "foot", UnitSystem.BritishImperial, DimensionVector.LengthDimension, "0.3048", "feet", "pied"),
        Linear("imperial.yard", "yd", "yard", UnitSystem.BritishImperial, DimensionVector.LengthDimension, "0.9144"),
        Linear("imperial.mile", "mi", "mile", UnitSystem.BritishImperial, DimensionVector.LengthDimension, "1609.344"),
        Linear("imperial.square-foot", "ft2", "square foot", UnitSystem.BritishImperial, Area, "0.09290304"),
        Linear("imperial.acre", "ac", "acre", UnitSystem.BritishImperial, Area, "4046.8564224"),
        Linear("imperial.pound", "lb", "pound", UnitSystem.BritishImperial, DimensionVector.MassDimension, "0.45359237", "livre"),
        Linear("imperial.ounce", "oz", "ounce", UnitSystem.BritishImperial, DimensionVector.MassDimension, "0.028349523125"),
        Linear("imperial.gallon", "imp-gal", "imperial gallon", UnitSystem.BritishImperial, Volume, "0.00454609"),
        Linear("imperial.pint", "imp-pt", "imperial pint", UnitSystem.BritishImperial, Volume, "0.00056826125", "pint"),

        Linear("us.gallon", "US-gal", "US liquid gallon", UnitSystem.UsCustomary, Volume, "0.003785411784"),
        Linear("us.pint", "US-pt", "US liquid pint", UnitSystem.UsCustomary, Volume, "0.000473176473", "pint"),
        Linear("us.cup", "US-cup", "US customary cup", UnitSystem.UsCustomary, Volume, "0.0002365882365", "cup"),
        Linear("us.tablespoon", "US-tbsp", "US tablespoon", UnitSystem.UsCustomary, Volume, "0.00001478676478125", "tbsp", "tablespoon"),
        Linear("us.teaspoon", "US-tsp", "US teaspoon", UnitSystem.UsCustomary, Volume, "0.00000492892159375", "tsp", "teaspoon"),
        Linear("us.mile-per-hour", "mph", "mile per hour", UnitSystem.UsCustomary, Speed, "0.44704"),
        new UnitDefinition(
            "us.fahrenheit",
            "degF",
            "degree Fahrenheit",
            UnitSystem.UsCustomary,
            DimensionVector.TemperatureDimension,
            new ExactRational(5, 9),
            new ExactRational(45967, 180),
            UnitTransformKind.AbsoluteTemperature,
            "fahrenheit"),

        Linear("metric.cup", "cup", "metric-international cup", UnitSystem.MetricInternational, Volume, "0.00025"),
        Linear("metric.tablespoon", "tbsp", "metric-international tablespoon", UnitSystem.MetricInternational, Volume, "0.000015", "tablespoon", "cs"),
        Linear("metric.teaspoon", "tsp", "metric-international teaspoon", UnitSystem.MetricInternational, Volume, "0.000005", "teaspoon", "cc-spoon"),
        Linear("metric.pint", "pt", "AURA metric-international pint", UnitSystem.MetricInternational, Volume, "0.0005", "pint"),

        Linear("au.cup", "AU-cup", "Australian culinary cup", UnitSystem.AustralianCulinary, Volume, "0.00025", "cup"),
        Linear("au.tablespoon", "AU-tbsp", "Australian tablespoon", UnitSystem.AustralianCulinary, Volume, "0.00002", "tbsp", "tablespoon"),
        Linear("au.teaspoon", "AU-tsp", "Australian teaspoon", UnitSystem.AustralianCulinary, Volume, "0.000005", "tsp", "teaspoon"),
    ];

    private static readonly FrozenDictionary<string, UnitDefinition> ById =
        All.ToFrozenDictionary(unit => unit.Id, StringComparer.Ordinal);

    static InheritedStandardUnitCatalog()
    {
        if (All.Any(unit => unit.Dimension == DimensionVector.Scalar))
        {
            throw new InvalidOperationException("The V1 unit catalog cannot contain scalar pseudo-units.");
        }
    }

    public static bool TryGetById(string id, out UnitDefinition? unit) =>
        ById.TryGetValue(id, out unit);

    private static UnitDefinition Linear(
        string id,
        string symbol,
        string name,
        UnitSystem system,
        DimensionVector dimension,
        string scale,
        params string[] aliases) =>
        new(
            id,
            symbol,
            name,
            system,
            dimension,
            ExactRational.ParseInvariantDecimal(scale),
            ExactRational.Zero,
            UnitTransformKind.Linear,
            aliases);

    private static IEnumerable<UnitDefinition> InformationUnits()
    {
        yield return InformationUnit("information.byte", "B", "byte", UnitSystem.Si, BigInteger.One);

        var decimalUnits = new (string Id, string Symbol, string Name)[]
        {
            ("si.kilobyte", "kB", "kilobyte"),
            ("si.megabyte", "MB", "megabyte"),
            ("si.gigabyte", "GB", "gigabyte"),
            ("si.terabyte", "TB", "terabyte"),
            ("si.petabyte", "PB", "petabyte"),
            ("si.exabyte", "EB", "exabyte"),
            ("si.zettabyte", "ZB", "zettabyte"),
            ("si.yottabyte", "YB", "yottabyte"),
        };
        var decimalScale = BigInteger.One;
        foreach (var unit in decimalUnits)
        {
            decimalScale *= 1000;
            yield return InformationUnit(unit.Id, unit.Symbol, unit.Name, UnitSystem.Si, decimalScale);
        }

        var binaryUnits = new (string Id, string Symbol, string Name)[]
        {
            ("iec.kibibyte", "KiB", "kibibyte"),
            ("iec.mebibyte", "MiB", "mebibyte"),
            ("iec.gibibyte", "GiB", "gibibyte"),
            ("iec.tebibyte", "TiB", "tebibyte"),
            ("iec.pebibyte", "PiB", "pebibyte"),
            ("iec.exbibyte", "EiB", "exbibyte"),
            ("iec.zebibyte", "ZiB", "zebibyte"),
            ("iec.yobibyte", "YiB", "yobibyte"),
        };
        var binaryScale = BigInteger.One;
        foreach (var unit in binaryUnits)
        {
            binaryScale *= 1024;
            yield return InformationUnit(unit.Id, unit.Symbol, unit.Name, UnitSystem.IecBinary, binaryScale);
        }
    }

    private static UnitDefinition InformationUnit(
        string id,
        string symbol,
        string name,
        UnitSystem system,
        BigInteger scale) =>
        new(
            id,
            symbol,
            name,
            system,
            DimensionVector.InformationDimension,
            new ExactRational(scale, BigInteger.One),
            ExactRational.Zero,
            UnitTransformKind.Linear,
            name);

    private static UnitDefinition AbsoluteTemperature(
        string id,
        string symbol,
        string name,
        UnitSystem system,
        string scale,
        string offset,
        params string[] aliases) =>
        new(
            id,
            symbol,
            name,
            system,
            DimensionVector.TemperatureDimension,
            ExactRational.ParseInvariantDecimal(scale),
            ExactRational.ParseInvariantDecimal(offset),
            UnitTransformKind.AbsoluteTemperature,
            aliases);

    private static UnitDefinition LinearFraction(
        string id,
        string symbol,
        string name,
        UnitSystem system,
        DimensionVector dimension,
        int numerator,
        int denominator,
        params string[] aliases) =>
        new(
            id,
            symbol,
            name,
            system,
            dimension,
            new ExactRational(numerator, denominator),
            ExactRational.Zero,
            UnitTransformKind.Linear,
            aliases);
}
