using System;
using System.Globalization;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// Canonical versioned checkpoint: splitmix64/v1 followed by exactly two
/// vertical-bar-delimited, lowercase, 16-character ASCII hexadecimal words.
/// </summary>
public static class SplitMix64PositionCodec
{
    public static string Format(SplitMix64Position position)
    {
        if (!string.Equals(position.Algorithm, SplitMix64Random.AlgorithmId,
            StringComparison.Ordinal))
            throw new ArgumentException("Unsupported versioned checkpoint.", nameof(position));
        return string.Concat(SplitMix64Random.AlgorithmId, "|",
            position.Seed.ToString("x16", CultureInfo.InvariantCulture), "|",
            position.Counter.ToString("x16", CultureInfo.InvariantCulture));
    }

    public static SplitMix64Position Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var parts = value.Split('|');
        if (parts.Length != 3 ||
            !string.Equals(parts[0], SplitMix64Random.AlgorithmId, StringComparison.Ordinal) ||
            !CanonicalHex16(parts[1]) || !CanonicalHex16(parts[2]) ||
            !ulong.TryParse(parts[1], NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture, out var seed) ||
            !ulong.TryParse(parts[2], NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture, out var counter))
            throw new FormatException("Checkpoint must contain splitmix64/v1 and two lowercase 16-digit hex words.");
        return new SplitMix64Position(SplitMix64Random.AlgorithmId, seed, counter);
    }

    private static bool CanonicalHex16(string value)
    {
        if (value.Length != 16) return false;
        foreach (var digit in value)
            if (!((digit >= '0' && digit <= '9') || (digit >= 'a' && digit <= 'f')))
                return false;
        return true;
    }
}
