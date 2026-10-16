using System.Globalization;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>The Markdown delimiter is a placement contract, not a Markdown parser.</summary>
public enum MarkdownMathPlacement
{
    Inline,
    Block
}

public enum MarkdownUnicodeMathStatus
{
    Admitted,
    Unsupported,
    BudgetExceeded
}

/// <summary>Unicode is display-only; only the retained typed IR carries mathematical identity.</summary>
public sealed record MarkdownUnicodeMathResult(
    MarkdownUnicodeMathStatus Status,
    IrNode? Expression,
    MarkdownMathPlacement? Placement,
    string? CanonicalText,
    string? UnicodeText,
    string? Markdown,
    string? SemanticStructuralSha256,
    string? RuleId)
{
    public bool IsAdmitted =>
        Status == MarkdownUnicodeMathStatus.Admitted &&
        Expression is not null && Placement is not null &&
        CanonicalText is not null && UnicodeText is not null &&
        Markdown is not null && SemanticStructuralSha256 is not null &&
        RuleId is not null;
}

/// <summary>
/// Bounded Unicode math and explicit inline/block Markdown wrappers.
/// Only the previously qualified exact MathML IR subset is displayed.
/// No inverse parse of Unicode glyphs, Markdown, or LaTeX is promised.
/// </summary>
public static class MarkdownUnicodeMathCodec
{
    public const string Rule = "dw-math-markdown-unicode-exact-display/1";
    public const int MaximumDisplayCharacters = 1024;

    public static MarkdownUnicodeMathResult Render(
        IrNode expression, MarkdownMathPlacement placement)
    {
        ArgumentNullException.ThrowIfNull(expression);
        if (placement is not (MarkdownMathPlacement.Inline or MarkdownMathPlacement.Block))
            return Refuse(MarkdownUnicodeMathStatus.Unsupported);
        try
        {
            var exact = ExactMathMlInterop.Render(expression);
            if (!exact.IsAdmitted || exact.CanonicalText is null ||
                exact.SemanticStructuralSha256 is null)
                return Refuse(exact.Status == MathMlInteropStatus.BudgetExceeded
                    ? MarkdownUnicodeMathStatus.BudgetExceeded
                    : MarkdownUnicodeMathStatus.Unsupported);
            var unicode = WriteUnicode(expression);
            var markdown = placement == MarkdownMathPlacement.Inline
                ? "$" + unicode + "$"
                : "$$\n" + unicode + "\n$$";
            if (unicode.Length > MaximumDisplayCharacters ||
                markdown.Length > MaximumDisplayCharacters)
                return Refuse(MarkdownUnicodeMathStatus.BudgetExceeded);
            var replay = IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(expression));
            if (IrCanonicalHashes.SemanticStructuralSha256(replay) !=
                exact.SemanticStructuralSha256)
                return Refuse(MarkdownUnicodeMathStatus.Unsupported);
            return new(MarkdownUnicodeMathStatus.Admitted, replay, placement,
                exact.CanonicalText, unicode, markdown,
                exact.SemanticStructuralSha256, Rule);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(MarkdownUnicodeMathStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is NotSupportedException or
            ArgumentException or InvalidOperationException)
        {
            return Refuse(MarkdownUnicodeMathStatus.Unsupported);
        }
    }

    private static string WriteUnicode(IrNode expression) => expression switch
    {
        IrExactScalar scalar when scalar.Value.Denominator.IsOne =>
            scalar.Value.Numerator.ToString(CultureInfo.InvariantCulture),
        IrExactScalar scalar =>
            scalar.Value.Numerator.ToString(CultureInfo.InvariantCulture) +
            "\u2044" + scalar.Value.Denominator.ToString(CultureInfo.InvariantCulture),
        IrSymbol symbol => symbol.DisplayName,
        IrApply { Operation: IrOperation.Add } sum
            when sum.Arguments.Length == 2 &&
                 sum.Arguments[0] is IrSymbol &&
                 sum.Arguments[1] is IrExactScalar =>
            WriteUnicode(sum.Arguments[0]) + " + " + WriteUnicode(sum.Arguments[1]),
        IrRestrictedExpression restriction
            when restriction.Expression is IrSymbol equationSymbol &&
                 restriction.Assumptions.Conditions.Length == 1 &&
                 restriction.Assumptions.Conditions[0].Kind == IrRelationKind.Equal &&
                 restriction.Assumptions.Conditions[0].Symbol.Identity == equationSymbol.Identity &&
                 restriction.Assumptions.Conditions[0].Symbol.Domain == equationSymbol.Domain =>
            WriteUnicode(equationSymbol) + " = " +
            WriteUnicode(restriction.Assumptions.Conditions[0].Right),
        _ => throw new NotSupportedException("This Math IR shape has no qualified Unicode display.")
    };

    private static MarkdownUnicodeMathResult Refuse(MarkdownUnicodeMathStatus status) =>
        new(status, null, null, null, null, null, null, null);
}
