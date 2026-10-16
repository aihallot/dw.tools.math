using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>Admitted MathML is a structural subset, never inferred typography.</summary>
public enum MathMlInteropStatus
{
    Admitted,
    InvalidXml,
    Unsupported,
    PresentationAmbiguous,
    NonCanonical,
    IntegrityMismatch,
    BudgetExceeded
}

/// <summary>
/// One admitted exchange retains the exact original IR and a binding
/// between canonical Math text, Content MathML and Presentation MathML.
/// A refusal contains no mathematical expression, exchange, or proof rule.
/// </summary>
public sealed record MathMlInteropResult(
    MathMlInteropStatus Status,
    IrNode? Expression,
    string? CanonicalText,
    string? AnnotatedMathMl,
    string? ContentMathMl,
    string? PresentationMathMl,
    string? SemanticStructuralSha256,
    string? RuleId)
{
    public bool IsAdmitted =>
        Status == MathMlInteropStatus.Admitted && Expression is not null &&
        CanonicalText is not null && AnnotatedMathMl is not null &&
        ContentMathMl is not null && PresentationMathMl is not null &&
        SemanticStructuralSha256 is not null && RuleId is not null;
}

/// <summary>
/// Closed MathML 3 profile with a Math canonical-text annotation, a
/// Content MathML annotation, and passive Presentation MathML.
/// Input XML is non-executing. The annotation is independently checked
/// against deterministic content and presentation trees before admission.
/// No arbitrary MathML, TeX/HTML parser or algebraic solver is claimed.
/// </summary>
public static class ExactMathMlInterop
{
    public const string Rule = "dw-math-mathml3-annotated-ir/1";
    public const string Namespace = "http://www.w3.org/1998/Math/MathML";
    public const string ContentEncoding = "application/mathml-content+xml";
    public const string TextEncoding = "application/x-dw-math-canonical-text-v1";
    public const int MaximumXmlCharacters = 4096;
    public const int MaximumXmlNodes = 32;
    public const int MaximumXmlDepth = 12;

    private static readonly XNamespace MathNs = Namespace;

    /// <summary>Render a qualified original Math IR without normalizing semantics.</summary>
    public static MathMlInteropResult Render(IrNode expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        try
        {
            var text = CanonicalMathNotation.Render(expression);
            if (!text.IsAdmitted || text.Expression is null)
                return Refuse(text.Status == CanonicalNotationStatus.BudgetExceeded
                    ? MathMlInteropStatus.BudgetExceeded
                    : MathMlInteropStatus.Unsupported);
            var (content, presentation) = Write(expression);
            var contentMath = new XElement(MathNs + "math", content)
                .ToString(SaveOptions.DisableFormatting);
            var presentationMath = new XElement(MathNs + "math", presentation)
                .ToString(SaveOptions.DisableFormatting);
            var combined = new XElement(MathNs + "math",
                new XElement(MathNs + "semantics", presentation,
                    new XElement(MathNs + "annotation",
                        new XAttribute("encoding", TextEncoding),
                        text.CanonicalText),
                    new XElement(MathNs + "annotation-xml",
                        new XAttribute("encoding", ContentEncoding),
                        content)));
            var annotated = combined.ToString(SaveOptions.DisableFormatting);
            if (annotated.Length > MaximumXmlCharacters ||
                combined.DescendantsAndSelf().Count() > MaximumXmlNodes ||
                combined.DescendantsAndSelf()
                    .Any(item => item.Ancestors().Count() + 1 > MaximumXmlDepth))
                return Refuse(MathMlInteropStatus.BudgetExceeded);

            var json = IrCanonicalJsonCodec.Encode(expression);
            var roundtrip = IrCanonicalJsonCodec.Decode(json);
            if (!string.Equals(IrCanonicalJsonCodec.Encode(roundtrip), json,
                    StringComparison.Ordinal) ||
                !string.Equals(IrCanonicalHashes.SemanticStructuralSha256(roundtrip),
                    text.SemanticStructuralSha256, StringComparison.Ordinal))
                return Refuse(MathMlInteropStatus.IntegrityMismatch);

            return new(MathMlInteropStatus.Admitted, roundtrip,
                text.CanonicalText, annotated, contentMath,
                presentationMath, text.SemanticStructuralSha256, Rule);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(MathMlInteropStatus.BudgetExceeded);
        }
        catch (Exception exception) when (exception is NotSupportedException or
            ArgumentException or InvalidOperationException)
        {
            return Refuse(MathMlInteropStatus.Unsupported);
        }
    }

    /// <summary>
    /// Import ONLY the exact canonical annotated subset produced by Render.
    /// Presentation MathML alone cannot authorize IR reconstruction.
    /// </summary>
    public static MathMlInteropResult ImportAnnotated(string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);
        if (xml.Length == 0)
            return Refuse(MathMlInteropStatus.InvalidXml);
        if (xml.Length > MaximumXmlCharacters)
            return Refuse(MathMlInteropStatus.BudgetExceeded);
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
            if (root is null || root.Name != MathNs + "math")
                return Refuse(MathMlInteropStatus.Unsupported);
            if (root.DescendantsAndSelf().Count() > MaximumXmlNodes ||
                root.DescendantsAndSelf().Any(item =>
                    item.Ancestors().Count() + 1 > MaximumXmlDepth))
                return Refuse(MathMlInteropStatus.BudgetExceeded);

            var semantics = root.Element(MathNs + "semantics");
            if (semantics is null)
                return Refuse(MathMlInteropStatus.PresentationAmbiguous);
            var annotations = semantics.Elements(MathNs + "annotation").ToArray();
            var contents = semantics.Elements(MathNs + "annotation-xml").ToArray();
            if (annotations.Length != 1 || contents.Length != 1 ||
                (string?)annotations[0].Attribute("encoding") != TextEncoding ||
                (string?)contents[0].Attribute("encoding") != ContentEncoding)
                return Refuse(MathMlInteropStatus.Unsupported);

            var canonical = CanonicalMathNotation.Parse(annotations[0].Value);
            if (!canonical.IsAdmitted || canonical.Expression is null)
                return Refuse(canonical.Status == CanonicalNotationStatus.BudgetExceeded
                    ? MathMlInteropStatus.BudgetExceeded
                    : MathMlInteropStatus.NonCanonical);

            var expected = Render(canonical.Expression);
            if (!expected.IsAdmitted || expected.AnnotatedMathMl is null)
                return Refuse(MathMlInteropStatus.Unsupported);

            // The annotation is not trusted in isolation: BOTH content and
            // presentation must match an independent regeneration from IR.
            var canonicalRoot = XDocument.Parse(expected.AnnotatedMathMl).Root;
            if (!XNode.DeepEquals(root, canonicalRoot))
                return Refuse(MathMlInteropStatus.IntegrityMismatch);
            if (!string.Equals(xml, expected.AnnotatedMathMl,
                    StringComparison.Ordinal))
                return Refuse(MathMlInteropStatus.NonCanonical);
            return expected;
        }
        catch (XmlException)
        {
            return Refuse(MathMlInteropStatus.InvalidXml);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(MathMlInteropStatus.BudgetExceeded);
        }
        catch (Exception exception) when (exception is FormatException or
            ArgumentException or InvalidOperationException)
        {
            return Refuse(MathMlInteropStatus.Unsupported);
        }
    }

    private static (XElement Content, XElement Presentation) Write(IrNode source)
    {
        switch (source)
        {
            case IrExactScalar scalar:
            {
                var number = scalar.Value;
                var numerator = number.Numerator.ToString(CultureInfo.InvariantCulture);
                if (number.Denominator.IsOne)
                    return (new XElement(MathNs + "cn", new XAttribute("type", "integer"), numerator),
                        new XElement(MathNs + "mn", numerator));
                var denominator = number.Denominator.ToString(
                    CultureInfo.InvariantCulture);
                return (
                    new XElement(MathNs + "apply",
                        new XElement(MathNs + "divide"),
                        new XElement(MathNs + "cn", new XAttribute("type", "integer"), numerator),
                        new XElement(MathNs + "cn", new XAttribute("type", "integer"), denominator)),
                    new XElement(MathNs + "mfrac",
                        new XElement(MathNs + "mn", numerator),
                        new XElement(MathNs + "mn", denominator)));
            }
            case IrSymbol symbol:
                return (new XElement(MathNs + "ci", new XAttribute("type", "real"), symbol.DisplayName),
                    new XElement(MathNs + "mi", symbol.DisplayName));
            case IrApply { Operation: IrOperation.Add } sum
                when sum.Arguments.Length == 2 &&
                     sum.Arguments[0] is IrSymbol &&
                     sum.Arguments[1] is IrExactScalar:
            {
                var left = Write(sum.Arguments[0]);
                var right = Write(sum.Arguments[1]);
                return (
                    new XElement(MathNs + "apply",
                        new XElement(MathNs + "plus"), left.Content, right.Content),
                    new XElement(MathNs + "mrow", left.Presentation,
                        new XElement(MathNs + "mo", "+"), right.Presentation));
            }
            case IrRestrictedExpression restriction
                when restriction.Expression is IrSymbol symbol &&
                    restriction.Assumptions.Conditions.Length == 1 &&
                    restriction.Assumptions.Conditions[0].Kind == IrRelationKind.Equal &&
                    restriction.Assumptions.Conditions[0].Symbol.Identity ==
                        symbol.Identity &&
                    restriction.Assumptions.Conditions[0].Symbol.Domain ==
                        symbol.Domain:
            {
                var relation = restriction.Assumptions.Conditions[0];
                var left = Write(symbol);
                var right = Write(relation.Right);
                return (
                    new XElement(MathNs + "apply",
                        new XElement(MathNs + "eq"), left.Content, right.Content),
                    new XElement(MathNs + "mrow", left.Presentation,
                        new XElement(MathNs + "mo", "="), right.Presentation));
            }
            default:
                throw new NotSupportedException(
                    "Only exact rationals, three real symbols, simple sums and a restricted equality are in the T1 MathML subset.");
        }
    }

    private static MathMlInteropResult Refuse(MathMlInteropStatus status) =>
        new(status, null, null, null, null, null, null, null);
}
