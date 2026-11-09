using System.Collections.Frozen;
using System.Collections.Immutable;
using dw.quantities;

namespace dw.quantities.standard;

/// <summary>
/// Opt-in, Math-owned SI profile for electrical and molar calculations.
/// Neither historical standard catalogue is changed or implicitly extended.
/// </summary>
public static class ElectrochemicalUnitProfile
{
    public const string ProfileId = "math.si-electrochemical";
    public const string ProfileVersion = "2026-11-electrochemical-exact-v1";

    public static UnitDefinition Ampere { get; } = Define(
        "si.ampere", "A", "ampere",
        new DimensionVector(0, 0, 0, 1, 0, 0, 0, 0));

    public static UnitDefinition Volt { get; } = Define(
        "si.volt", "V", "volt",
        new DimensionVector(2, 1, -3, -1, 0, 0, 0, 0));

    public static UnitDefinition Ohm { get; } = Define(
        "si.ohm", "Ω", "ohm",
        new DimensionVector(2, 1, -3, -2, 0, 0, 0, 0));

    public static UnitDefinition Watt { get; } = Define(
        "si.watt", "W", "watt",
        new DimensionVector(2, 1, -3, 0, 0, 0, 0, 0));

    public static UnitDefinition Mole { get; } = Define(
        "si.mole", "mol", "mole",
        new DimensionVector(0, 0, 0, 0, 0, 1, 0, 0));

    public static UnitDefinition MolePerCubicMetre { get; } = Define(
        "si.mole-per-cubic-metre", "mol/m³", "mole per cubic metre",
        new DimensionVector(-3, 0, 0, 0, 0, 1, 0, 0));

    public static ImmutableArray<UnitDefinition> All { get; } =
        [Ampere, Volt, Ohm, Watt, Mole, MolePerCubicMetre];

    private static readonly FrozenDictionary<string, UnitDefinition> ById =
        All.ToFrozenDictionary(unit => unit.Id, StringComparer.Ordinal);

    /// <summary>
    /// Resolves only the six explicitly admitted profile identities.
    /// Never silently falls back to an inherited or presentation catalogue.
    /// </summary>
    public static bool TryGetById(string id, out UnitDefinition? unit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (id.Length > 128)
            throw new ArgumentOutOfRangeException(nameof(id),
                "Unit identifiers are limited to 128 characters.");
        return ById.TryGetValue(id, out unit);
    }

    private static UnitDefinition Define(string id, string symbol, string name,
        DimensionVector dimension) =>
        new(id, symbol, name, UnitSystem.Si, dimension,
            ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
}
