using System.Numerics;

namespace dw.quantities;

/// <summary>Preserves the exact IEEE 754 binary64 value, without a decimal round-trip.</summary>
public static class ExactBinaryNumber
{
    public static ExactRational FromDouble(double value)
    {
        if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
        var bits = BitConverter.DoubleToUInt64Bits(value);
        var exponent = (int)((bits >> 52) & 0x7ff);
        var fraction = bits & 0x000f_ffff_ffff_ffff;
        var significand = new BigInteger(exponent == 0 ? fraction : fraction | (1UL << 52));
        if ((bits >> 63) != 0) significand = -significand;
        var power = exponent == 0 ? -1074 : exponent - 1023 - 52;
        return power >= 0
            ? new ExactRational(significand << power, BigInteger.One)
            : new ExactRational(significand, BigInteger.One << -power);
    }
}
