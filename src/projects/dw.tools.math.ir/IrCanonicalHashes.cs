using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace Dw.Tools.Math.Ir;

/// <summary>
/// Versioned structural digests; these are not algebraic-equivalence proofs.
/// The presentation digest also includes passive symbol labels.
/// </summary>
public static class IrCanonicalHashes
{
    public const string Scheme = "math-ir/1-sha256";

    public static string SemanticStructuralSha256(IrNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var json = JsonNode.Parse(IrCanonicalJsonCodec.Encode(node))!;
        RemoveLabels(json);
        return Hex("semantic-structural", json.ToJsonString());
    }

    public static string PresentationSha256(IrNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return Hex("presentation", IrCanonicalJsonCodec.Encode(node));
    }

    private static string Hex(string domain, string json)
    {
        var data = Encoding.UTF8.GetBytes(Scheme + "\n" + domain + "\n" + json);
        return Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
    }

    private static void RemoveLabels(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                if ((string?)obj["kind"] == "symbol")
                    obj.Remove("label");
                foreach (var item in obj.ToArray())
                    RemoveLabels(item.Value);
                break;
            case JsonArray array:
                foreach (var item in array)
                    RemoveLabels(item);
                break;
        }
    }
}
