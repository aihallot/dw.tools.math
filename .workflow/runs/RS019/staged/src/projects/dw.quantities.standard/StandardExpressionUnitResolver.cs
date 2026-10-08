using System.Collections.Immutable;
using dw.quantities;

namespace dw.quantities.standard;

/// <summary>
/// Culture-independent exact resolution. Every ambiguous token requires an
/// explicit UnitSystem profile; no fallback from caller culture is permitted.
/// </summary>
public static class StandardExpressionUnitResolver
{
    public const int MaximumTokenLength = 128;

    public static ImmutableArray<UnitDefinition> FindCandidates(string token, UnitSystem? profile = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        if (token.Length > MaximumTokenLength)
            throw new ArgumentOutOfRangeException(nameof(token), "Unit tokens are limited to 128 characters.");

        var candidates = StandardUnitCatalog.Units
            .Where(unit => string.Equals(unit.Id, token, StringComparison.OrdinalIgnoreCase)
                || string.Equals(unit.Symbol, token, StringComparison.Ordinal)
                || unit.Aliases.Any(alias => string.Equals(alias, token, StringComparison.OrdinalIgnoreCase)));

        if (profile is not null)
            candidates = candidates.Where(unit => unit.System == profile.Value);

        return candidates.ToImmutableArray();
    }

    public static UnitDefinition Resolve(string token, UnitSystem? profile = null)
    {
        var matches = FindCandidates(token, profile);
        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new KeyNotFoundException("Unknown unit token under the supplied explicit profile."),
            _ => throw new InvalidOperationException("Ambiguous unit token: select an explicit unit profile.")
        };
    }
}
