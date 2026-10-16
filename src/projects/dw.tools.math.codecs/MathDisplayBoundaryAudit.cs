using System.Security.Cryptography;
using System.Text;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>Independent source format, never an invitation to infer mathematics from glyphs.</summary>
public enum MathDisplayInputKind
{
    StandaloneOmml,
    MarkdownMath
}

public enum MathDisplayBoundaryStatus
{
    Admitted,
    InvalidSource,
    Unsupported,
    PresentationAmbiguous,
    NonCanonical,
    IntegrityMismatch,
    BudgetExceeded
}

/// <summary>Conservative reason why visual interchange cannot certify source Math IR.</summary>
public enum MathDisplayLossKind
{
    None,
    InvalidXml,
    UnsupportedDocumentOrProfile,
    UnqualifiedLayoutOrSemanticMarkup,
    NonCanonicalSerialization,
    MarkdownPlacementMismatch,
    UnqualifiedMarkdownOrUnicode,
    UnsupportedOriginalIrOrPlacement,
    ResourceLimit,
    UnverifiedStructuralIdentity
}

/// <summary>
/// A bounded source fingerprint is provenance only. On refusal there is no
/// reconstructed IR, semantic structural hash, canonical representation or rule.
/// Font/positioning and whole-document fidelity are never qualified.
/// </summary>
public sealed record MathDisplayBoundaryResult(
    MathDisplayInputKind InputKind,
    MathDisplayBoundaryStatus Status,
    MathDisplayLossKind LossKind,
    IrNode? Expression,
    string? CanonicalText,
    string? CanonicalRepresentation,
    string? SemanticStructuralSha256,
    string? RuleId,
    string? SourceSha256,
    bool StructuralFidelityQualified,
    bool TypographyInverseQualified,
    bool RequiresOfficeInstallation,
    bool WholeDocumentConversionQualified,
    bool FontFidelityQualified,
    bool PositioningFidelityQualified)
{
    public bool IsAdmitted =>
        Status == MathDisplayBoundaryStatus.Admitted &&
        LossKind == MathDisplayLossKind.None &&
        Expression is not null && CanonicalText is not null &&
        CanonicalRepresentation is not null && SemanticStructuralSha256 is not null &&
        RuleId is not null && SourceSha256 is not null &&
        StructuralFidelityQualified && !TypographyInverseQualified &&
        !RequiresOfficeInstallation && !WholeDocumentConversionQualified &&
        !FontFidelityQualified && !PositioningFidelityQualified;
}

/// <summary>
/// Audits ONLY exact already-qualified standalone OMML and Markdown math
/// wrappers against caller-provided original typed IR. No second XML/Markdown
/// parser, font interpreter, whole-document converter or Office runtime.
/// </summary>
public static class MathDisplayBoundaryAudit
{
    public const string Rule = "dw-math-omml-markdown-display-boundary/1";
    public const int MaximumOmmlCharacters = ExactOfficeOmmlCodec.MaximumXmlCharacters;
    public const int MaximumMarkdownCharacters =
        MarkdownUnicodeMathCodec.MaximumDisplayCharacters;

    /// <summary>
    /// Delegates secure XML handling to the qualified Office codec, independently
    /// replays source Math IR, and refuses any extra markup or altered source.
    /// </summary>
    public static MathDisplayBoundaryResult AuditOmml(string xml, IrNode original)
    {
        ArgumentNullException.ThrowIfNull(xml);
        ArgumentNullException.ThrowIfNull(original);
        const MathDisplayInputKind kind = MathDisplayInputKind.StandaloneOmml;
        if (xml.Length > MaximumOmmlCharacters)
            return Refuse(kind, MathDisplayBoundaryStatus.BudgetExceeded,
                MathDisplayLossKind.ResourceLimit, null);
        var sourceSha = Fingerprint(xml);
        var imported = ExactOfficeOmmlCodec.ImportCanonical(xml,original);
        if (!imported.IsAdmitted || imported.Expression is null)
        {
            var (status, loss) = MapOmml(imported.Status);
            return Refuse(kind,status,loss,sourceSha);
        }
        if (!string.Equals(imported.Omml,xml,StringComparison.Ordinal))
            return Unverified(kind,sourceSha);
        return Verify(kind,original,imported.CanonicalText,
            imported.Omml,imported.SemanticStructuralSha256,sourceSha);
    }

    /// <summary>
    /// Exact-placement, exact-byte verification only. No inverse of Unicode,
    /// dollar delimiters, glyph normalization, Markdown or LaTeX.
    /// </summary>
    public static MathDisplayBoundaryResult AuditMarkdown(
        string markdown, MarkdownMathPlacement placement, IrNode original)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        ArgumentNullException.ThrowIfNull(original);
        const MathDisplayInputKind kind = MathDisplayInputKind.MarkdownMath;
        if (markdown.Length > MaximumMarkdownCharacters)
            return Refuse(kind,MathDisplayBoundaryStatus.BudgetExceeded,
                MathDisplayLossKind.ResourceLimit,null);
        var sourceSha = Fingerprint(markdown);
        var rendered = MarkdownUnicodeMathCodec.Render(original,placement);
        if (!rendered.IsAdmitted || rendered.Markdown is null)
            return Refuse(kind,rendered.Status == MarkdownUnicodeMathStatus.BudgetExceeded
                    ? MathDisplayBoundaryStatus.BudgetExceeded
                    : MathDisplayBoundaryStatus.Unsupported,
                rendered.Status == MarkdownUnicodeMathStatus.BudgetExceeded
                    ? MathDisplayLossKind.ResourceLimit
                    : MathDisplayLossKind.UnsupportedOriginalIrOrPlacement,sourceSha);
        if (!string.Equals(markdown,rendered.Markdown,StringComparison.Ordinal))
        {
            var oppositePlacement=placement==MarkdownMathPlacement.Inline
                ? MarkdownMathPlacement.Block : MarkdownMathPlacement.Inline;
            var opposite=MarkdownUnicodeMathCodec.Render(original,oppositePlacement);
            if (opposite.IsAdmitted &&
                string.Equals(markdown,opposite.Markdown,StringComparison.Ordinal))
                return Refuse(kind,MathDisplayBoundaryStatus.PresentationAmbiguous,
                    MathDisplayLossKind.MarkdownPlacementMismatch,sourceSha);
            return Refuse(kind,MathDisplayBoundaryStatus.Unsupported,
                MathDisplayLossKind.UnqualifiedMarkdownOrUnicode,sourceSha);
        }
        return Verify(kind,original,rendered.CanonicalText,
            rendered.Markdown,rendered.SemanticStructuralSha256,sourceSha);
    }

    private static MathDisplayBoundaryResult Verify(
        MathDisplayInputKind kind, IrNode original,
        string? canonicalText, string? canonicalDisplay,
        string? semanticSha, string sourceSha)
    {
        try
        {
            var canonicalJson=IrCanonicalJsonCodec.Encode(original);
            var replay=IrCanonicalJsonCodec.Decode(canonicalJson);
            var canonical=CanonicalMathNotation.Render(replay);
            if (!string.Equals(canonicalJson,IrCanonicalJsonCodec.Encode(replay),
                    StringComparison.Ordinal) ||
                !canonical.IsAdmitted ||
                !string.Equals(canonical.CanonicalText,canonicalText,
                    StringComparison.Ordinal) ||
                !string.Equals(canonical.SemanticStructuralSha256,semanticSha,
                    StringComparison.Ordinal) ||
                !string.Equals(IrCanonicalHashes.SemanticStructuralSha256(replay),
                    semanticSha,StringComparison.Ordinal) ||
                string.IsNullOrEmpty(canonicalDisplay))
                return Unverified(kind,sourceSha);
            return new(kind,MathDisplayBoundaryStatus.Admitted,
                MathDisplayLossKind.None,replay,canonicalText,canonicalDisplay,
                semanticSha,Rule,sourceSha,StructuralFidelityQualified:true,
                TypographyInverseQualified:false,RequiresOfficeInstallation:false,
                WholeDocumentConversionQualified:false,FontFidelityQualified:false,
                PositioningFidelityQualified:false);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or NotSupportedException or InvalidOperationException)
        {
            return Unverified(kind,sourceSha);
        }
    }

    private static (MathDisplayBoundaryStatus,MathDisplayLossKind) MapOmml(
        OfficeOmmlStatus status) => status switch
    {
        OfficeOmmlStatus.InvalidXml =>
            (MathDisplayBoundaryStatus.InvalidSource,MathDisplayLossKind.InvalidXml),
        OfficeOmmlStatus.UnsupportedProfile =>
            (MathDisplayBoundaryStatus.Unsupported,
                MathDisplayLossKind.UnsupportedDocumentOrProfile),
        OfficeOmmlStatus.NonCanonical =>
            (MathDisplayBoundaryStatus.NonCanonical,
                MathDisplayLossKind.NonCanonicalSerialization),
        OfficeOmmlStatus.IntegrityMismatch =>
            (MathDisplayBoundaryStatus.IntegrityMismatch,
                MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup),
        OfficeOmmlStatus.BudgetExceeded =>
            (MathDisplayBoundaryStatus.BudgetExceeded,MathDisplayLossKind.ResourceLimit),
        _ => (MathDisplayBoundaryStatus.IntegrityMismatch,
                MathDisplayLossKind.UnverifiedStructuralIdentity)
    };

    private static string Fingerprint(string source) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source)))
            .ToLowerInvariant();

    private static MathDisplayBoundaryResult Unverified(
        MathDisplayInputKind kind,string sourceSha) =>
        Refuse(kind,MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnverifiedStructuralIdentity,sourceSha);

    private static MathDisplayBoundaryResult Refuse(
        MathDisplayInputKind kind,MathDisplayBoundaryStatus status,
        MathDisplayLossKind loss,string? sourceSha) =>
        new(kind,status,loss,null,null,null,null,null,sourceSha,
            StructuralFidelityQualified:false,TypographyInverseQualified:false,
            RequiresOfficeInstallation:false,WholeDocumentConversionQualified:false,
            FontFidelityQualified:false,PositioningFidelityQualified:false);
}
