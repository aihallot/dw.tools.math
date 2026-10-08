using System.Globalization;
using System.Numerics;

namespace dw.quantities;

public enum DecimalRoundingMode
{
    HalfEven,
    HalfAwayFromZero,
    TowardZero,
    Floor,
    Ceiling,
}

public static class ExactDecimalFormatter
{
    public const int MaximumDecimalPlaces = 100;

    public static string Format(ExactRational value, int decimalPlaces, DecimalRoundingMode rounding)
    {
        if (decimalPlaces is < 0 or > MaximumDecimalPlaces)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
        }

        if (!Enum.IsDefined(rounding))
        {
            throw new ArgumentOutOfRangeException(nameof(rounding));
        }

        var negative = value.Numerator.Sign < 0;
        var scale = BigInteger.Pow(10, decimalPlaces);
        var quotient = BigInteger.DivRem(
            BigInteger.Abs(value.Numerator) * scale,
            value.Denominator,
            out var remainder);
        if (ShouldIncrement(quotient, remainder, value.Denominator, negative, rounding))
        {
            quotient += BigInteger.One;
        }

        if (quotient.IsZero)
        {
            negative = false;
        }

        var digits = quotient.ToString(CultureInfo.InvariantCulture);
        string result;
        if (decimalPlaces == 0)
        {
            result = digits;
        }
        else
        {
            digits = digits.PadLeft(decimalPlaces + 1, '0');
            var split = digits.Length - decimalPlaces;
            result = string.Concat(digits.AsSpan(0, split), ".", digits.AsSpan(split));
            result = result.TrimEnd('0').TrimEnd('.');
        }

        return negative ? $"-{result}" : result;
    }

    private static bool ShouldIncrement(
        BigInteger quotient,
        BigInteger remainder,
        BigInteger denominator,
        bool negative,
        DecimalRoundingMode rounding)
    {
        if (remainder.IsZero)
        {
            return false;
        }

        return rounding switch
        {
            DecimalRoundingMode.TowardZero => false,
            DecimalRoundingMode.Floor => negative,
            DecimalRoundingMode.Ceiling => !negative,
            DecimalRoundingMode.HalfAwayFromZero => remainder * 2 >= denominator,
            DecimalRoundingMode.HalfEven => remainder * 2 > denominator ||
                (remainder * 2 == denominator && !quotient.IsEven),
            _ => throw new InvalidOperationException("Unknown decimal rounding mode."),
        };
    }
}
