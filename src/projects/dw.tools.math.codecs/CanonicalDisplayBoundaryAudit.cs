using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>
/// A safe display is not an executable TeX program, a reversible TeX codec,
/// or proof of algebraic equivalence between different IR trees.
/// </summary>
public enum CanonicalDisplayBoundaryStatus
{
    SafeDisplay,
    SafeDisplayWithCollision,
    RefusedInput,
    UnboundRestriction,
    ContradictoryAssumptions,
    StructuralLoss,
    UnsafeDisplay
}

/// <summary>
/// Only admitted results expose IR, exact canonical text, display-only LaTeX,
/// the original structural semantic SHA-256 and a qualified boundary rule.
/// Refusals carry no presentation, mathematical result or proof marker.
/// </summary>
public sealed record CanonicalDisplayBoundaryResult(
    CanonicalDisplayBoundaryStatus Status,
    CanonicalNotationStatus? UnderlyingStatus,
    IrNode? Expression,
    string? CanonicalText,
    string? DisplayLatex,
    string? SemanticStructuralSha256,
    string? RuleId,
    bool LatexInverseQualified)
{
    public bool IsAdmitted =>
        (Status is CanonicalDisplayBoundaryStatus.SafeDisplay or
            CanonicalDisplayBoundaryStatus.SafeDisplayWithCollision) &&
        Expression is not null && CanonicalText is not null &&
        DisplayLatex is not null && SemanticStructuralSha256 is not null &&
        RuleId is not null && !LatexInverseQualified;

    public bool PresentationMayCollide =>
        Status == CanonicalDisplayBoundaryStatus.SafeDisplayWithCollision &&
        IsAdmitted;
}

/// <summary>
/// Bounded, passive audit over the T1 canonical text/IR/LaTeX contract.
/// Only renderer-generated LaTeX is inspected. External LaTeX is NEVER
/// parsed or executed, and no Unicode/infix/TeX importer is admitted.
/// </summary>
public static class CanonicalDisplayBoundaryAudit
{
    public const string Rule = "canonical-safe-math-display-domain-audit/v1";
    public const int MaximumDisplayCharacters = 4096;
    public const int MaximumBraceDepth = 64;

    public static CanonicalDisplayBoundaryResult AuditText(string canonicalText)
    {
        ArgumentNullException.ThrowIfNull(canonicalText);
        var parsed = CanonicalMathNotation.Parse(canonicalText);
        return AuditAdmitted(parsed);
    }

    public static CanonicalDisplayBoundaryResult AuditIr(IrNode original)
    {
        ArgumentNullException.ThrowIfNull(original);
        var rendered = CanonicalMathNotation.Render(original);
        return AuditAdmitted(rendered);
    }

    private static CanonicalDisplayBoundaryResult AuditAdmitted(
        CanonicalNotationResult source)
    {
        if (!source.IsAdmitted || source.Expression is not IrNode expression)
            return Refuse(CanonicalDisplayBoundaryStatus.RefusedInput,
                source.Status);

        try
        {
            // IR's already-qualified JSON decoder independently checks that
            // every typed restriction actually references an original
            // expression symbol, rather than a fabricated free variable.
            var json = IrCanonicalJsonCodec.Encode(expression);
            var restored = IrCanonicalJsonCodec.Decode(json);
            if (!string.Equals(json, IrCanonicalJsonCodec.Encode(restored),
                    StringComparison.Ordinal))
                return Refuse(CanonicalDisplayBoundaryStatus.StructuralLoss);

            if (Contradictory(restored))
                return Refuse(CanonicalDisplayBoundaryStatus.ContradictoryAssumptions);

            var canonical = CanonicalMathNotation.Render(restored);
            if (!canonical.IsAdmitted ||
                canonical.Expression is null ||
                !string.Equals(source.CanonicalText, canonical.CanonicalText,
                    StringComparison.Ordinal) ||
                !string.Equals(source.SemanticStructuralSha256,
                    canonical.SemanticStructuralSha256, StringComparison.Ordinal))
                return Refuse(CanonicalDisplayBoundaryStatus.StructuralLoss);

            // Canonical text -> IR -> canonical text is validated independently.
            // LaTeX is output only and has NO qualified inverse.
            var reparsed = CanonicalMathNotation.Parse(canonical.CanonicalText!);
            if (!reparsed.IsAdmitted || reparsed.Expression is null ||
                !string.Equals(IrCanonicalJsonCodec.Encode(reparsed.Expression),
                    json, StringComparison.Ordinal) ||
                !string.Equals(reparsed.SemanticStructuralSha256,
                    canonical.SemanticStructuralSha256, StringComparison.Ordinal))
                return Refuse(CanonicalDisplayBoundaryStatus.StructuralLoss);

            if (!SafeLatex(canonical.Latex!))
                return Refuse(CanonicalDisplayBoundaryStatus.UnsafeDisplay);

            var collision = HasKnownCollision(restored);
            return new(
                collision
                    ? CanonicalDisplayBoundaryStatus.SafeDisplayWithCollision
                    : CanonicalDisplayBoundaryStatus.SafeDisplay,
                CanonicalNotationStatus.Admitted,
                restored, canonical.CanonicalText, canonical.Latex,
                canonical.SemanticStructuralSha256, Rule,
                LatexInverseQualified: false);
        }
        catch (FormatException)
        {
            return Refuse(CanonicalDisplayBoundaryStatus.UnboundRestriction);
        }
        catch (ArgumentException)
        {
            return Refuse(CanonicalDisplayBoundaryStatus.StructuralLoss);
        }
        catch (NotSupportedException)
        {
            return Refuse(CanonicalDisplayBoundaryStatus.StructuralLoss);
        }
    }

    private static bool Contradictory(IrNode root)
    {
        var stack = new Stack<IrNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            switch (stack.Pop())
            {
                case IrRestrictedExpression restricted:
                {
                    foreach (var group in restricted.Assumptions.Conditions
                        .GroupBy(condition => (condition.Symbol.Identity,
                            condition.Symbol.Domain)))
                    {
                        var equality = group
                            .Where(c => c.Kind == IrRelationKind.Equal)
                            .Select(c => c.Right.Value).Distinct().ToArray();
                        if (equality.Length > 1)
                            return true;
                        if (equality.Length == 1)
                        {
                            var value = equality[0];
                            if (group.Any(c =>
                                c.Kind == IrRelationKind.NotEqual &&
                                    c.Right.Value == value ||
                                c.Kind == IrRelationKind.GreaterOrEqual &&
                                    value < c.Right.Value))
                                return true;
                        }
                    }
                    stack.Push(restricted.Expression);
                    break;
                }
                case IrApply apply:
                    foreach (var child in apply.Arguments)
                        stack.Push(child);
                    break;
            }
        }
        return false;
    }

    private static bool HasKnownCollision(IrNode root)
    {
        var stack = new Stack<IrNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            switch (stack.Pop())
            {
                // square(x) and pow(x,2) are distinct IR constructors but
                // produce exactly the same T1 LaTeX string.
                case IrApply { Operation: IrOperation.Square }:
                    return true;
                case IrApply { Operation: IrOperation.Power } power
                    when power.Arguments[1] is IrExactScalar exponent &&
                        exponent.Value == new ExactRational(2,1):
                    return true;
                case IrApply apply:
                    foreach (var child in apply.Arguments)
                        stack.Push(child);
                    break;
                case IrRestrictedExpression restricted:
                    stack.Push(restricted.Expression);
                    break;
            }
        }
        return false;
    }

    private static bool SafeLatex(string latex)
    {
        if (latex.Length == 0 || latex.Length > MaximumDisplayCharacters)
            return false;

        var braces = 0;
        for (var index = 0; index < latex.Length; index++)
        {
            var current = latex[index];
            if (current < ' ' || current > '~')
                return false;
            if (current == '{')
            {
                if (++braces > MaximumBraceDepth)
                    return false;
                continue;
            }
            if (current == '}')
            {
                if (--braces < 0)
                    return false;
                continue;
            }
            if (current != '\\')
                continue;

            if (++index == latex.Length)
                return false;
            if (latex[index] is ',' or ';')
                continue;

            var first = index;
            while (index < latex.Length &&
                latex[index] >= 'a' && latex[index] <= 'z')
                index++;
            if (index == first)
                return false;
            var command = latex[first..index];
            if (command is not ("frac" or "left" or "right" or
                "cdot" or "sqrt" or "mathrm" or "ne" or "ge"))
                return false;
            index--;
        }
        return braces == 0;
    }

    private static CanonicalDisplayBoundaryResult Refuse(
        CanonicalDisplayBoundaryStatus status,
        CanonicalNotationStatus? sourceStatus = null) =>
        new(status, sourceStatus, null, null, null, null, null,
            LatexInverseQualified: false);
}
