using dw.quantities;
using dw.quantities.expression;

namespace dw.quantities.standard;

/// <summary>
/// Bridges the BCL-only standard catalogue to the pure expression parser.
/// A profile is supplied by the caller; the parser's culture argument never
/// selects a system or silently resolves ambiguous units.
/// </summary>
public sealed class ExplicitProfileExpressionUnitResolver : IExpressionUnitResolver
{
    public UnitSystem? Profile { get; }
    public bool UseInheritedCatalog { get; }

    public ExplicitProfileExpressionUnitResolver(UnitSystem? profile = null,
        bool useInheritedCatalog = true)
    {
        Profile = profile;
        UseInheritedCatalog = useInheritedCatalog;
    }

    public ExpressionUnitResolution Resolve(string token, string culture)
    {
        if (string.IsNullOrWhiteSpace(token) ||
            token.Length > StandardExpressionUnitResolver.MaximumTokenLength)
            return new(null, ExpressionUnitResolutionFailure.UnitNotFound, []);

        var matches = UseInheritedCatalog
            ? StandardExpressionUnitResolver.FindInheritedCandidates(token, Profile)
            : StandardExpressionUnitResolver.FindCandidates(token, Profile);
        if (matches.Length == 1)
            return new(matches[0], ExpressionUnitResolutionFailure.None, [matches[0].Id]);

        var ids = matches.Select(unit => unit.Id).Order(StringComparer.Ordinal).ToArray();
        return new(null, matches.Length == 0
            ? ExpressionUnitResolutionFailure.UnitNotFound
            : ExpressionUnitResolutionFailure.AmbiguousUnit, ids);
    }
}
