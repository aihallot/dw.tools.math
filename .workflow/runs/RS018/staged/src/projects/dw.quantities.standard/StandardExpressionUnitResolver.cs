using dw.quantities;

namespace dw.quantities.standard;

/// <summary>
/// Strict token resolution. Ambiguous aliases need an explicit UnitSystem profile.
/// Process CurrentCulture has no bearing on resolution.
/// </summary>
public static class StandardExpressionUnitResolver
{
    public static UnitDefinition Resolve(string token, UnitSystem? profile = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        var match = StandardUnitCatalog.Units
            .Where(unit => string.Equals(unit.Id, token, StringComparison.OrdinalIgnoreCase)
                || string.Equals(unit.Symbol, token, StringComparison.Ordinal)
                || unit.Aliases.Any(alias => string.Equals(alias, token, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
        if (profile is not null)
            match = match.Where(unit => unit.System == profile.Value).ToArray();
        return match.Length switch
        {
            1 => match[0],
            0 => throw new KeyNotFoundException("Unknown unit token under the supplied explicit profile."),
            _ => throw new InvalidOperationException("Ambiguous unit token: select an explicit unit profile.")
        };
    }
}
