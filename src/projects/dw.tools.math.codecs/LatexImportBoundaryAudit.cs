using System.Security.Cryptography;
using System.Text;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>Why a LaTeX import did or did not preserve qualified meaning.</summary>
public enum LatexImportLossKind
{
    None,
    PassiveNotationChange,
    ExplicitMultiplicationConvention,
    AmbiguousInterpretation,
    NonCanonicalRepresentation,
    InvalidSyntax,
    UnsupportedShape,
    ResourceLimit,
    UnverifiedStructuralIdentity
}

/// <summary>
/// SourceSha256 binds the original bounded untrusted bytes, not a proof of
/// mathematical meaning. A refusal has no result expression or rule.
/// Importing source LaTeX never qualifies a reverse LaTeX renderer.
/// </summary>
public sealed record LatexImportBoundaryResult(
    LatexImportStatus Status,
    LatexImportLossKind LossKind,
    IrNode? Expression,
    string? CanonicalText,
    string? SemanticStructuralSha256,
    string? RuleId,
    LatexJuxtapositionConvention? AppliedConvention,
    string? SourceSha256,
    bool SemanticFidelityQualified,
    bool LatexInverseQualified)
{
    public bool IsAdmitted =>
        Status == LatexImportStatus.Admitted &&
        Expression is not null && CanonicalText is not null &&
        SemanticStructuralSha256 is not null && RuleId is not null &&
        AppliedConvention.HasValue && SourceSha256 is not null &&
        SemanticFidelityQualified && !LatexInverseQualified;

    public bool UsedExplicitMultiplicationConvention =>
        IsAdmitted && LossKind == LatexImportLossKind.ExplicitMultiplicationConvention;
}

/// <summary>
/// An independent provenance and loss classification for the qualified,
/// non-executing BoundedLatexImporter. It neither widens the admitted TeX
/// grammar nor parses arbitrary macros or interprets TeX code.
/// </summary>
public static class LatexImportBoundaryAudit
{
    public const string Rule = "math-latex-import-boundary-loss/v1";
    public const int MaximumInputCharacters = BoundedLatexImporter.MaximumInputCharacters;

    public static LatexImportBoundaryResult Audit(
        string latex,
        LatexJuxtapositionConvention convention =
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        ArgumentNullException.ThrowIfNull(latex);
        // Do not allocate a hash buffer for an unbounded untrusted input.
        if (latex.Length > MaximumInputCharacters)
            return Refuse(LatexImportStatus.BudgetExceeded,
                LatexImportLossKind.ResourceLimit, null);

        var sourceHash=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(latex))).ToLowerInvariant();
        var imported=BoundedLatexImporter.Import(latex,convention);
        if (!imported.IsAdmitted || imported.Expression is not IrNode original)
            return Refuse(imported.Status, Classify(imported.Status),
                sourceHash);

        try
        {
            // The qualified Math IR JSON codec independently verifies the
            // exact original structure. This is structural fidelity, not
            // algebraic equivalence or an inverse display proof.
            var json=IrCanonicalJsonCodec.Encode(original);
            var decoded=IrCanonicalJsonCodec.Decode(json);
            if (!string.Equals(json,IrCanonicalJsonCodec.Encode(decoded),
                    StringComparison.Ordinal))
                return Unverified(sourceHash);

            var canonical=CanonicalMathNotation.Render(decoded);
            var display=CanonicalDisplayBoundaryAudit.AuditIr(decoded);
            if (!canonical.IsAdmitted || !display.IsAdmitted ||
                canonical.Expression is null ||
                !string.Equals(canonical.CanonicalText,imported.CanonicalText,
                    StringComparison.Ordinal) ||
                !string.Equals(canonical.SemanticStructuralSha256,
                    imported.SemanticStructuralSha256,StringComparison.Ordinal) ||
                !string.Equals(display.SemanticStructuralSha256,
                    imported.SemanticStructuralSha256,StringComparison.Ordinal))
                return Unverified(sourceHash);

            var defaultAmbiguous=convention ==
                LatexJuxtapositionConvention.Multiplication &&
                BoundedLatexImporter.Import(latex,
                    LatexJuxtapositionConvention.RequireExplicitOperator).Status ==
                    LatexImportStatus.Ambiguous;

            var loss=defaultAmbiguous
                ? LatexImportLossKind.ExplicitMultiplicationConvention
                : string.Equals(latex,imported.CanonicalText,
                    StringComparison.Ordinal)
                    ? LatexImportLossKind.None
                    : LatexImportLossKind.PassiveNotationChange;

            return new(LatexImportStatus.Admitted,loss,decoded,
                imported.CanonicalText,imported.SemanticStructuralSha256,
                Rule,imported.AppliedConvention,sourceHash,
                SemanticFidelityQualified:true,LatexInverseQualified:false);
        }
        catch (ArgumentException)
        {
            return Unverified(sourceHash);
        }
        catch (FormatException)
        {
            return Unverified(sourceHash);
        }
        catch (NotSupportedException)
        {
            return Unverified(sourceHash);
        }
    }

    private static LatexImportLossKind Classify(LatexImportStatus status) =>
        status switch
        {
            LatexImportStatus.Ambiguous =>
                LatexImportLossKind.AmbiguousInterpretation,
            LatexImportStatus.Invalid =>
                LatexImportLossKind.InvalidSyntax,
            LatexImportStatus.Unsupported =>
                LatexImportLossKind.UnsupportedShape,
            LatexImportStatus.NonCanonical =>
                LatexImportLossKind.NonCanonicalRepresentation,
            LatexImportStatus.BudgetExceeded =>
                LatexImportLossKind.ResourceLimit,
            _ => LatexImportLossKind.UnverifiedStructuralIdentity
        };

    private static LatexImportBoundaryResult Unverified(string sourceHash) =>
        Refuse(LatexImportStatus.Unsupported,
            LatexImportLossKind.UnverifiedStructuralIdentity,sourceHash);

    private static LatexImportBoundaryResult Refuse(
        LatexImportStatus status,LatexImportLossKind loss,string? sourceHash) =>
        new(status,loss,null,null,null,null,null,sourceHash,
            SemanticFidelityQualified:false,LatexInverseQualified:false);
}
