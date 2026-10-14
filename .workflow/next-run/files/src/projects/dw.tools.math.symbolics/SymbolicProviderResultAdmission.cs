using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Strict admission result for an externally proposed symbolic object.</summary>
public enum SymbolicProviderAdmissionStatus
{
    Admitted,
    UnrepresentableOutput,
    ConditionalMismatch,
    UnsupportedSource,
    ContradictoryAssumptions,
    BudgetExceeded
}

/// <summary>
/// A successful result is a fresh, locally proven typed IR restriction, not
/// an external provider object, string, permission or general CAS proof.
/// </summary>
public sealed record SymbolicProviderAdmissionResult(
    SymbolicProviderAdmissionStatus Status,
    IrRestrictedExpression? Conditional)
{
    public bool IsAdmitted =>
        Status == SymbolicProviderAdmissionStatus.Admitted && Conditional is not null;
}

/// <summary>
/// Admits only a typed candidate that exactly matches Math's independently
/// proven, bounded rational-cancellation identity AND its original exclusions.
/// No external provider code executes, and no text is parsed as mathematics.
/// </summary>
public static class SymbolicProviderResultAdmission
{
    public static SymbolicProviderAdmissionResult TryAdmit(
        IrNode source, object? providerProposedResult)
    {
        ArgumentNullException.ThrowIfNull(source);

        // Strings, ASTs from foreign assemblies, arbitrary objects and even
        // an unconditional native IrApply are unsupported. Never call the
        // candidate's ToString, reflection methods or user callbacks.
        if (providerProposedResult is not IrRestrictedExpression candidate)
            return Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput);

        SymbolicCancellationResult proof;
        try
        {
            proof = SymbolicRationalCancellationBridge.TryCancel(source);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refused(SymbolicProviderAdmissionStatus.UnsupportedSource);
        }

        if (!proof.IsAdmitted)
        {
            return Refused(proof.Status switch
            {
                SymbolicCancellationStatus.ContradictoryAssumptions =>
                    SymbolicProviderAdmissionStatus.ContradictoryAssumptions,
                SymbolicCancellationStatus.BudgetExceeded =>
                    SymbolicProviderAdmissionStatus.BudgetExceeded,
                _ => SymbolicProviderAdmissionStatus.UnsupportedSource
            });
        }

        // IR's canonical decoder checks graph limits, unknown types and
        // whether restriction variables are referenced by the expression.
        // Structural byte equality is deliberately stronger than mere
        // algebraic or presentational similarity: no missing, added, or
        // silently reordered condition is allowed at this boundary.
        string incoming;
        try
        {
            incoming = IrCanonicalJsonCodec.Encode(candidate);
            if (IrCanonicalJsonCodec.Decode(incoming) is not IrRestrictedExpression)
                return Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput);
        }

        var localResult = proof.Conditional!;
        var localCanonical = IrCanonicalJsonCodec.Encode(localResult);
        if (!string.Equals(incoming, localCanonical, StringComparison.Ordinal))
            return Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch);

        // Return Math's locally proven object, not the caller's candidate.
        return new(SymbolicProviderAdmissionStatus.Admitted, localResult);
    }

    private static SymbolicProviderAdmissionResult Refused(
        SymbolicProviderAdmissionStatus reason) => new(reason, null);
}
