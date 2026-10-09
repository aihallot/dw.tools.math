using System.Collections.Immutable;

namespace Dw.Tools.Math.Composition;

/// <summary>
/// Published capability descriptions, independent of provider installation,
/// execution, identity, permission, and AURA host state.
/// </summary>
public static class MathCapabilityCatalog
{
    private static readonly ImmutableArray<MathOperationCapability> Entries =
        ImmutableArray.Create(MathOperationCapability.MatrixInspection);

    /// <summary>Lists described contracts, not executable or authorized operations.</summary>
    public static ImmutableArray<MathOperationCapability> Discover() => Entries;

    /// <summary>Unknown operation ids are absent; no fallback provider is assumed.</summary>
    public static MathOperationCapability? Find(string? operationId) =>
        string.Equals(operationId, MathOperationCapability.MatrixInspection.OperationId,
            StringComparison.Ordinal)
            ? MathOperationCapability.MatrixInspection
            : null;
}
