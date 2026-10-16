using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

public enum OfficeOmmlStatus
{
    Admitted,
    InvalidXml,
    UnsupportedProfile,
    NonCanonical,
    IntegrityMismatch,
    BudgetExceeded
}

/// <summary>Admitted OMML is bound to independently retained exact Math IR.</summary>
public sealed record OfficeOmmlResult(
    OfficeOmmlStatus Status,
    IrNode? Expression,
    string? CanonicalText,
    string? Omml,
    string? SemanticStructuralSha256,
    string? RuleId)
{
    public bool IsAdmitted =>
        Status == OfficeOmmlStatus.Admitted && Expression is not null &&
        CanonicalText is not null && Omml is not null &&
        SemanticStructuralSha256 is not null && RuleId is not null;
}

/// <summary>
/// Canonical OMML m:oMath, m:r/m:t and m:f/m:num/m:den only.
/// A caller MUST supply original typed IR for import. Typography alone
/// does not establish original symbol identity, domains or restrictions.
/// This is a standalone .NET XML codec; no Office or document SDK is required.
/// </summary>
public static class ExactOfficeOmmlCodec
{
    public const string Rule = "dw-math-office-omml-exact-original-ir/1";
    public const string NamespaceUri =
        "http://schemas.openxmlformats.org/officeDocument/2006/math";
    public const int MaximumXmlCharacters = 4096;
    public const int MaximumXmlNodes = 32;
    public const int MaximumXmlDepth = 12;

    private static readonly XNamespace M = NamespaceUri;

    public static OfficeOmmlResult Render(IrNode expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        try
        {
            var qualified = ExactMathMlInterop.Render(expression);
            if (!qualified.IsAdmitted || qualified.CanonicalText is null ||
                qualified.SemanticStructuralSha256 is null)
                return Refuse(qualified.Status == MathMlInteropStatus.BudgetExceeded
                    ? OfficeOmmlStatus.BudgetExceeded
                    : OfficeOmmlStatus.UnsupportedProfile);
            var xml = new XElement(M + "oMath",
                new XAttribute(XNamespace.Xmlns + "m", NamespaceUri),
                Write(expression));
            if (xml.DescendantsAndSelf().Count() > MaximumXmlNodes ||
                xml.DescendantsAndSelf().Any(item =>
                    item.Ancestors().Count() + 1 > MaximumXmlDepth))
                return Refuse(OfficeOmmlStatus.BudgetExceeded);
            var omml = xml.ToString(SaveOptions.DisableFormatting);
            if (omml.Length > MaximumXmlCharacters)
                return Refuse(OfficeOmmlStatus.BudgetExceeded);
            var replay = IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(expression));
            if (IrCanonicalHashes.SemanticStructuralSha256(replay) !=
                qualified.SemanticStructuralSha256)
                return Refuse(OfficeOmmlStatus.IntegrityMismatch);
            return new(OfficeOmmlStatus.Admitted, replay, qualified.CanonicalText,
                omml, qualified.SemanticStructuralSha256, Rule);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(OfficeOmmlStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is NotSupportedException or
            ArgumentException or InvalidOperationException)
        {
            return Refuse(OfficeOmmlStatus.UnsupportedProfile);
        }
    }

    /// <summary>
    /// Admit only this codec's canonical standalone OMML serialization with
    /// a caller-supplied original IR that independently reproduces every token.
    /// No source-free semantic inference or Office document conversion.
    /// </summary>
    public static OfficeOmmlResult ImportCanonical(string xml, IrNode original)
    {
        ArgumentNullException.ThrowIfNull(xml);
        ArgumentNullException.ThrowIfNull(original);
        if (xml.Length == 0)
            return Refuse(OfficeOmmlStatus.InvalidXml);
        if (xml.Length > MaximumXmlCharacters)
            return Refuse(OfficeOmmlStatus.BudgetExceeded);
        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersFromEntities = 0,
                MaxCharactersInDocument = MaximumXmlCharacters,
                IgnoreWhitespace = false,
                IgnoreComments = false
            };
            using var input = new StringReader(xml);
            using var reader = XmlReader.Create(input, settings);
            var parsed = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
            var root = parsed.Root;
            if (root is null || root.Name != M + "oMath")
                return Refuse(OfficeOmmlStatus.UnsupportedProfile);
            if (root.DescendantsAndSelf().Count() > MaximumXmlNodes ||
                root.DescendantsAndSelf().Any(item =>
                    item.Ancestors().Count() + 1 > MaximumXmlDepth))
                return Refuse(OfficeOmmlStatus.BudgetExceeded);
            var expected = Render(original);
            if (!expected.IsAdmitted || expected.Omml is null)
                return Refuse(OfficeOmmlStatus.UnsupportedProfile);
            var canonicalRoot = XDocument.Parse(expected.Omml).Root;
            if (!XNode.DeepEquals(root, canonicalRoot))
                return Refuse(OfficeOmmlStatus.IntegrityMismatch);
            if (!string.Equals(xml, expected.Omml, StringComparison.Ordinal))
                return Refuse(OfficeOmmlStatus.NonCanonical);
            return expected;
        }
        catch (XmlException)
        {
            return Refuse(OfficeOmmlStatus.InvalidXml);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(OfficeOmmlStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is FormatException or
            ArgumentException or InvalidOperationException)
        {
            return Refuse(OfficeOmmlStatus.UnsupportedProfile);
        }
    }

    private static XElement Run(string text) =>
        new(M + "r", new XElement(M + "t", text));

    private static XElement[] Write(IrNode expression)
    {
        switch (expression)
        {
            case IrExactScalar scalar:
            {
                var numerator = scalar.Value.Numerator.ToString(CultureInfo.InvariantCulture);
                if (scalar.Value.Denominator.IsOne)
                    return [Run(numerator)];
                var denominator = scalar.Value.Denominator.ToString(CultureInfo.InvariantCulture);
                return [new XElement(M + "f",
                    new XElement(M + "num", Run(numerator)),
                    new XElement(M + "den", Run(denominator)))];
            }
            case IrSymbol freeSymbol:
                return [Run(freeSymbol.DisplayName)];
            case IrApply { Operation: IrOperation.Add } sum
                when sum.Arguments.Length == 2 &&
                     sum.Arguments[0] is IrSymbol &&
                     sum.Arguments[1] is IrExactScalar:
                return [..Write(sum.Arguments[0]), Run("+"), ..Write(sum.Arguments[1])];
            case IrRestrictedExpression restriction
                when restriction.Expression is IrSymbol equationSymbol &&
                     restriction.Assumptions.Conditions.Length == 1 &&
                     restriction.Assumptions.Conditions[0].Kind == IrRelationKind.Equal &&
                     restriction.Assumptions.Conditions[0].Symbol.Identity == equationSymbol.Identity &&
                     restriction.Assumptions.Conditions[0].Symbol.Domain == equationSymbol.Domain:
                return [..Write(equationSymbol), Run("="),
                    ..Write(restriction.Assumptions.Conditions[0].Right)];
            default:
                throw new NotSupportedException(
                    "Only exact MathML-qualified scalar, real symbol, ordered sum and original equality are admitted.");
        }
    }

    private static OfficeOmmlResult Refuse(OfficeOmmlStatus status) =>
        new(status, null, null, null, null, null);
}
