using System.Text.Json;

namespace Dw.Tools.Math.Ir;

/// <summary>Closed v1 JSON shape admission before JsonNode materialization.</summary>
internal static class IrCanonicalJsonGuard
{
    public const int MaximumIntegerDigits = 1024;

    public static void Validate(JsonElement root)
    {
        Object(root, "envelope", "version", "root");
        var version = root.GetProperty("version");
        if (version.ValueKind != JsonValueKind.String)
            throw new FormatException("IR version must be a string.");
        Node(root.GetProperty("root"));
    }

    public static void ValidateRestrictions(IrNode root)
    {
        var nodes = new Stack<IrNode>();
        nodes.Push(root);
        while (nodes.Count > 0)
        {
            var node = nodes.Pop();
            switch (node)
            {
                case IrRestrictedExpression restricted:
                    var referenced = new HashSet<(string, IrScalarDomain)>();
                    var pending = new Stack<IrNode>();
                    pending.Push(restricted.Expression);
                    while (pending.Count > 0)
                    {
                        switch (pending.Pop())
                        {
                            case IrSymbol s:
                                referenced.Add((s.Identity, s.Domain));
                                break;
                            case IrApply app:
                                foreach (var arg in app.Arguments) pending.Push(arg);
                                break;
                            case IrRestrictedExpression inner:
                                pending.Push(inner.Expression);
                                break;
                            case IrMatrix matrix:
                                foreach (var cell in matrix.Elements) pending.Push(cell);
                                break;
                        }
                    }
                    foreach (var condition in restricted.Assumptions.Conditions)
                        if (!referenced.Contains((condition.Symbol.Identity, condition.Symbol.Domain)))
                            throw new FormatException("An IR restriction references an unbound expression symbol.");
                    nodes.Push(restricted.Expression);
                    break;
                case IrApply app:
                    foreach (var arg in app.Arguments) nodes.Push(arg);
                    break;
                case IrMatrix matrix:
                    foreach (var element in matrix.Elements) nodes.Push(element);
                    break;
            }
        }
    }

    private static void Node(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
            throw new FormatException("IR node must be an object.");
        if (!element.TryGetProperty("kind", out var kind) || kind.ValueKind != JsonValueKind.String)
            throw new FormatException("IR node kind is required.");
        switch (kind.GetString())
        {
            case "exact":
                Object(element, "exact", "kind", "numerator", "denominator");
                CheckDigits(element.GetProperty("numerator"));
                CheckDigits(element.GetProperty("denominator"));
                break;
            case "binary64":
                Object(element, "binary64", "kind", "bits");
                break;
            case "symbol":
                Object(element, "symbol", "kind", "identity", "label", "domain", "scope", "slot");
                break;
            case "quantity":
                Object(element, "quantity", "kind", "value", "unit", "system", "dimensions", "temperature");
                Node(element.GetProperty("value"));
                Array(element.GetProperty("dimensions"), "dimensions");
                break;
            case "apply":
                Object(element, "apply", "kind", "operator", "arguments");
                foreach (var child in Array(element.GetProperty("arguments"), "arguments").EnumerateArray())
                    Node(child);
                break;
            case "matrix":
                Object(element, "matrix", "kind", "rows", "columns", "elements");
                foreach (var child in Array(element.GetProperty("elements"), "elements").EnumerateArray())
                    Node(child);
                break;
            case "restricted":
                Object(element, "restricted", "kind", "expression", "assumptions");
                Node(element.GetProperty("expression"));
                foreach (var condition in Array(element.GetProperty("assumptions"), "assumptions").EnumerateArray())
                {
                    Object(condition, "assumption", "symbol", "relation", "right");
                    Node(condition.GetProperty("symbol"));
                    Node(condition.GetProperty("right"));
                }
                break;
            default:
                throw new NotSupportedException("Unsupported IR node kind.");
        }
    }

    private static JsonElement Array(JsonElement element, string context)
    {
        if (element.ValueKind != JsonValueKind.Array)
            throw new FormatException("IR " + context + " must be an array.");
        return element;
    }

    private static void CheckDigits(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.String)
            throw new FormatException("An exact rational component must be a decimal string.");
        var value = element.GetString()!;
        if (value.Length > MaximumIntegerDigits || value.Length == 0)
            throw new FormatException("Exact rational numeral exceeds admitted digits.");
    }

    private static void Object(JsonElement element, string context, params string[] fields)
    {
        if (element.ValueKind != JsonValueKind.Object)
            throw new FormatException("IR " + context + " must be an object.");
        var accepted = fields.ToHashSet(StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var field in element.EnumerateObject())
        {
            if (!accepted.Contains(field.Name))
                throw new FormatException("Unexpected IR " + context + " field.");
            if (!seen.Add(field.Name))
                throw new FormatException("Duplicate IR " + context + " field.");
        }
        if (seen.Count != accepted.Count)
            throw new FormatException("Missing IR " + context + " field.");
    }
}
