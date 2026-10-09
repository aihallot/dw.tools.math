using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using dw.quantities;

namespace Dw.Tools.Math.Ir;

/// <summary>Canonical structural JSON for the closed v1 IR subset; not a symbolic equivalence proof.</summary>
public static class IrCanonicalJsonCodec
{
    public const string Version = "math-ir/1";
    public const int MaximumJsonCharacters = 262144;

    public static string Encode(IrNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        IrGraphLimits.Validate(root);
        var envelope = new JsonObject
        {
            ["version"] = Version,
            ["root"] = WriteNode(root)
        };
        var result = envelope.ToJsonString();
        if (result.Length > MaximumJsonCharacters)
            throw new ArgumentOutOfRangeException(nameof(root), "IR JSON output exceeds the bounded codec size.");
        return result;
    }

    public static IrNode Decode(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        if (json.Length > MaximumJsonCharacters)
            throw new ArgumentOutOfRangeException(nameof(json), "IR JSON input exceeds the bounded codec size.");
        var doc = JsonNode.Parse(json, new JsonNodeOptions { PropertyNameCaseInsensitive = false },
            new JsonDocumentOptions { MaxDepth = 64, CommentHandling = JsonCommentHandling.Disallow });
        var envelope = RequireObject(doc);
        if (Text(envelope, "version") != Version)
            throw new NotSupportedException("Unknown IR JSON schema version.");
        if (envelope.Count != 2)
            throw new FormatException("IR envelope has unexpected properties.");
        var node = ReadNode(envelope["root"]);
        IrGraphLimits.Validate(node);
        return node;
    }

    private static JsonObject WriteNode(IrNode node) => node switch
    {
        IrExactScalar exact => new JsonObject
        {
            ["kind"] = "exact",
            ["numerator"] = exact.Value.Numerator.ToString(CultureInfo.InvariantCulture),
            ["denominator"] = exact.Value.Denominator.ToString(CultureInfo.InvariantCulture)
        },
        IrFiniteBinary64Scalar approximate => new JsonObject
        {
            ["kind"] = "binary64",
            ["bits"] = unchecked((ulong)approximate.Ieee754Bits).ToString("x16", CultureInfo.InvariantCulture)
        },
        IrSymbol symbol => new JsonObject
        {
            ["kind"] = "symbol",
            ["identity"] = symbol.Identity,
            ["label"] = symbol.DisplayName,
            ["domain"] = symbol.Domain.ToString(),
            ["scope"] = symbol.ScopeId,
            ["slot"] = symbol.Slot
        },
        IrQuantityLiteral quantity => new JsonObject
        {
            ["kind"] = "quantity",
            ["value"] = WriteNode(quantity.Value),
            ["unit"] = quantity.CanonicalUnitId,
            ["system"] = quantity.System.ToString(),
            ["dimensions"] = new JsonArray(
                JsonValue.Create(quantity.Dimension.Length), JsonValue.Create(quantity.Dimension.Mass),
                JsonValue.Create(quantity.Dimension.Time), JsonValue.Create(quantity.Dimension.ElectricCurrent),
                JsonValue.Create(quantity.Dimension.Temperature), JsonValue.Create(quantity.Dimension.AmountOfSubstance),
                JsonValue.Create(quantity.Dimension.LuminousIntensity), JsonValue.Create(quantity.Dimension.Information)),
            ["temperature"] = quantity.Temperature.ToString()
        },
        IrApply apply => new JsonObject
        {
            ["kind"] = "apply",
            ["operator"] = apply.Operation.ToString(),
            ["arguments"] = new JsonArray(apply.Arguments.Select(x => (JsonNode?)WriteNode(x)).ToArray())
        },
        IrMatrix matrix => new JsonObject
        {
            ["kind"] = "matrix",
            ["rows"] = matrix.Rows,
            ["columns"] = matrix.Columns,
            ["elements"] = new JsonArray(matrix.Elements.Select(x => (JsonNode?)WriteNode(x)).ToArray())
        },
        IrRestrictedExpression restricted => new JsonObject
        {
            ["kind"] = "restricted",
            ["expression"] = WriteNode(restricted.Expression),
            ["assumptions"] = new JsonArray(restricted.Assumptions.Conditions.Select(x => (JsonNode?)
                new JsonObject
                {
                    ["symbol"] = WriteNode(x.Symbol),
                    ["relation"] = x.Kind.ToString(),
                    ["right"] = WriteNode(x.Right)
                }).ToArray())
        },
        _ => throw new NotSupportedException("Unknown or unsupported IR node.")
    };

    private static IrNode ReadNode(JsonNode? raw)
    {
        var obj = RequireObject(raw);
        return Text(obj, "kind") switch
        {
            "exact" => ReadExact(obj),
            "binary64" => ReadBinary64(obj),
            "symbol" => ReadSymbol(obj),
            "quantity" => ReadQuantity(obj),
            "apply" => IrApply.Create(Choice<IrOperation>(Text(obj, "operator")),
                Items(obj, "arguments").Select(ReadNode)),
            "matrix" => IrMatrix.Create(Integer(obj, "rows"), Integer(obj, "columns"),
                Items(obj, "elements").Select(x=>ReadNode(x) as IrScalar
                    ?? throw new FormatException("Matrix element must be a scalar."))),
            "restricted" => new IrRestrictedExpression(ReadNode(obj["expression"]),
                IrAssumptionSet.Create(Items(obj, "assumptions").Select(ReadRelation))),
            _ => throw new NotSupportedException("Unsupported IR node kind.")
        };
    }

    private static IrExactScalar ReadExact(JsonObject obj)
    {
        if (!BigInteger.TryParse(Text(obj, "numerator"), NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out var numerator) ||
            !BigInteger.TryParse(Text(obj, "denominator"), NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out var denominator) || denominator <= BigInteger.Zero)
            throw new FormatException("Invalid exact rational numerator or denominator.");
        var value = new ExactRational(numerator, denominator);
        if (value.Numerator != numerator || value.Denominator != denominator)
            throw new FormatException("Noncanonical exact rational is not admitted.");
        return new IrExactScalar(value);
    }

    private static IrFiniteBinary64Scalar ReadBinary64(JsonObject obj)
    {
        var bits = Text(obj, "bits");
        if (bits.Length != 16 ||
            !ulong.TryParse(bits, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture,
                out var unsigned) ||
            bits != unsigned.ToString("x16", CultureInfo.InvariantCulture))
            throw new FormatException("Binary64 bits require 16 lowercase hexadecimal digits.");
        return IrFiniteBinary64Scalar.FromDouble(
            BitConverter.Int64BitsToDouble(unchecked((long)unsigned)));
    }

    private static IrSymbol ReadSymbol(JsonObject obj)
    {
        var id = Text(obj, "identity");
        var label = Text(obj, "label");
        var domain = Choice<IrScalarDomain>(Text(obj, "domain"));
        var slot = Integer(obj, "slot");
        var scope = obj["scope"] is null ? null : Text(obj, "scope");
        IrSymbol symbol;
        if (scope is null && slot == -1 && id.StartsWith("free:", StringComparison.Ordinal))
            symbol = IrSymbol.Free(id[5..], label, domain);
        else if (scope is not null && slot >= 0)
            symbol = IrSymbol.Bound(scope, slot, label, domain);
        else
            throw new FormatException("Invalid IR symbol binding.");
        if (symbol.Identity != id)
            throw new FormatException("IR symbol identity mismatch.");
        return symbol;
    }

    private static IrQuantityLiteral ReadQuantity(JsonObject obj)
    {
        var dim = Items(obj, "dimensions");
        if (dim.Count != 8)
            throw new FormatException("An IR quantity requires eight dimensional exponents.");
        var n = dim.Select(x => x?.GetValue<int>()
            ?? throw new FormatException("Null dimension exponent.")).ToArray();
        var scalar = ReadNode(obj["value"]) as IrScalar
            ?? throw new FormatException("IR quantity value must be exact or finite scalar.");
        return new IrQuantityLiteral(scalar, Text(obj, "unit"),
            Choice<UnitSystem>(Text(obj, "system")),
            new DimensionVector(n[0],n[1],n[2],n[3],n[4],n[5],n[6],n[7]),
            Choice<IrTemperatureKind>(Text(obj, "temperature")));
    }

    private static IrRelation ReadRelation(JsonNode? raw)
    {
        var obj = RequireObject(raw);
        var symbol = ReadNode(obj["symbol"]) as IrSymbol
            ?? throw new FormatException("An assumption must refer to a symbol.");
        var right = ReadNode(obj["right"]) as IrExactScalar
            ?? throw new FormatException("An assumption right side must be exact.");
        return new IrRelation(symbol, Choice<IrRelationKind>(Text(obj, "relation")), right);
    }

    private static T Choice<T>(string text) where T : struct, Enum =>
        Enum.TryParse<T>(text, ignoreCase: false, out var value) &&
        Enum.IsDefined(value) && value.ToString() == text
            ? value : throw new FormatException("Unknown IR enum token.");

    private static JsonObject RequireObject(JsonNode? node) =>
        node as JsonObject ?? throw new FormatException("Expected IR JSON object.");

    private static JsonArray Items(JsonObject node, string name) =>
        node[name] as JsonArray ?? throw new FormatException("Expected IR JSON array: " + name);

    private static string Text(JsonObject node, string name) =>
        node[name]?.GetValue<string>() ?? throw new FormatException("Expected IR JSON text: " + name);

    private static int Integer(JsonObject node, string name) =>
        node[name]?.GetValue<int>() ?? throw new FormatException("Expected IR JSON integer: " + name);
}
