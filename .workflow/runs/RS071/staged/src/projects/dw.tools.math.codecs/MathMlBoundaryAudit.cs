using System.Security.Cryptography;
using System.Text;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>
/// What a closed MathML 3 input preserves or refuses. This is not a
/// generic XML sanitizer or an inverse for presentation typography.
/// </summary>
public enum MathMlBoundaryLossKind
{
    None,
    PresentationWithoutAnnotation,
    InvalidXml,
    UnsupportedProfile,
    NonCanonicalSerialization,
    AlteredSemanticBinding,
    ResourceLimit,
    UnverifiedStructuralIdentity
}

/// <summary>
/// An admitted result binds the original input byte fingerprint to
/// independently replayed exact Math IR. A refused result may retain
/// bounded input provenance but has no mathematical result or rule.
/// </summary>
public sealed record MathMlBoundaryResult(
    MathMlInteropStatus Status,
    MathMlBoundaryLossKind LossKind,
    IrNode? Expression,
    string? CanonicalText,
    string? AnnotatedMathMl,
    string? ContentMathMl,
    string? PresentationMathMl,
    string? SemanticStructuralSha256,
    string? RuleId,
    string? SourceSha256,
    bool StructuralFidelityQualified,
    bool PresentationInverseQualified)
{
    public bool IsAdmitted =>
        Status == MathMlInteropStatus.Admitted &&
        LossKind == MathMlBoundaryLossKind.None &&
        Expression is not null && CanonicalText is not null &&
        AnnotatedMathMl is not null && ContentMathMl is not null &&
        PresentationMathMl is not null &&
        SemanticStructuralSha256 is not null && RuleId is not null &&
        SourceSha256 is not null && StructuralFidelityQualified &&
        !PresentationInverseQualified;
}

/// <summary>
/// Independent, non-executing audit over ExactMathMlInterop's already
/// qualified, closed, exact annotated MathML 3 profile. It does not
/// parse external markup apart from the qualified importer or expand
/// input grammars. DTD/entity and node/depth restrictions remain
/// enforced by that importer before structural admission.
/// </summary>
public static class MathMlBoundaryAudit
{
    public const string Rule = "dw-math-mathml3-boundary-provenance/1";
    public const int MaximumInputCharacters =
        ExactMathMlInterop.MaximumXmlCharacters;

    public static MathMlBoundaryResult Audit(string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);
        // Never compute a digest for unbounded source data.
        if (xml.Length > MaximumInputCharacters)
            return Refuse(MathMlInteropStatus.BudgetExceeded,
                MathMlBoundaryLossKind.ResourceLimit, null);

        var sourceHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(xml))).ToLowerInvariant();
        var imported = ExactMathMlInterop.ImportAnnotated(xml);
        if (!imported.IsAdmitted || imported.Expression is not IrNode original)
            return Refuse(imported.Status, Classify(imported.Status),
                sourceHash);

        try
        {
            // Check the ORIGINAL typed tree independently of the passive
            // presentation: Math IR JSON is a structural, not algebraic,
            // certificate and cannot prove a generic visual inverse.
            var originalJson = IrCanonicalJsonCodec.Encode(original);
            var restored = IrCanonicalJsonCodec.Decode(originalJson);
            if (!string.Equals(originalJson,
                    IrCanonicalJsonCodec.Encode(restored),
                    StringComparison.Ordinal))
                return Unverified(sourceHash);

            var canonical = CanonicalMathNotation.Render(restored);
            var regenerated = ExactMathMlInterop.Render(restored);
            var structuralHash =
                IrCanonicalHashes.SemanticStructuralSha256(restored);
            if (!canonical.IsAdmitted || !regenerated.IsAdmitted ||
                !string.Equals(imported.SemanticStructuralSha256,
                    structuralHash, StringComparison.Ordinal) ||
                !string.Equals(canonical.SemanticStructuralSha256,
                    structuralHash, StringComparison.Ordinal) ||
                !string.Equals(imported.CanonicalText,
                    canonical.CanonicalText, StringComparison.Ordinal) ||
                !string.Equals(imported.AnnotatedMathMl,xml,
                    StringComparison.Ordinal) ||
                !string.Equals(regenerated.AnnotatedMathMl,xml,
                    StringComparison.Ordinal) ||
                !string.Equals(regenerated.ContentMathMl,
                    imported.ContentMathMl,StringComparison.Ordinal) ||
                !string.Equals(regenerated.PresentationMathMl,
                    imported.PresentationMathMl,StringComparison.Ordinal))
                return Unverified(sourceHash);

            return new(MathMlInteropStatus.Admitted,
                MathMlBoundaryLossKind.None,restored,
                canonical.CanonicalText,regenerated.AnnotatedMathMl,
                regenerated.ContentMathMl,regenerated.PresentationMathMl,
                structuralHash,Rule,sourceHash,
                StructuralFidelityQualified:true,
                PresentationInverseQualified:false);
        }
        catch (Exception exception) when (exception is ArgumentException or
            FormatException or NotSupportedException or InvalidOperationException)
        {
            return Unverified(sourceHash);
        }
    }

    private static MathMlBoundaryLossKind Classify(MathMlInteropStatus status) =>
        status switch
        {
            MathMlInteropStatus.PresentationAmbiguous =>
                MathMlBoundaryLossKind.PresentationWithoutAnnotation,
            MathMlInteropStatus.InvalidXml =>
                MathMlBoundaryLossKind.InvalidXml,
            MathMlInteropStatus.Unsupported =>
                MathMlBoundaryLossKind.UnsupportedProfile,
            MathMlInteropStatus.NonCanonical =>
                MathMlBoundaryLossKind.NonCanonicalSerialization,
            MathMlInteropStatus.IntegrityMismatch =>
                MathMlBoundaryLossKind.AlteredSemanticBinding,
            MathMlInteropStatus.BudgetExceeded =>
                MathMlBoundaryLossKind.ResourceLimit,
            _ => MathMlBoundaryLossKind.UnverifiedStructuralIdentity
        };

    private static MathMlBoundaryResult Unverified(string sourceHash) =>
        Refuse(MathMlInteropStatus.IntegrityMismatch,
            MathMlBoundaryLossKind.UnverifiedStructuralIdentity,sourceHash);

    private static MathMlBoundaryResult Refuse(
        MathMlInteropStatus status, MathMlBoundaryLossKind loss,
        string? sourceHash) =>
        new(status,loss,null,null,null,null,null,null,null,sourceHash,
            StructuralFidelityQualified:false,
            PresentationInverseQualified:false);
}
