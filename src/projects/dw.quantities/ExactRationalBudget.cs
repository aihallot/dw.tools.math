using System.Numerics;

namespace dw.quantities;

/// <summary>
/// Explicit resource policy for exact operations. Unbudgeted ExactRational arithmetic is exact but unbounded;
/// callers requiring resource limits must invoke these checked entry points.
/// </summary>
public static class ExactRationalBudget
{
    public static ExactRational Add(ExactRational left, ExactRational right, int maxBits)
    {
        Validate(maxBits);
        RequireWithin(left, maxBits);
        RequireWithin(right, maxBits);
        RequireProduct(left.Numerator, right.Denominator, maxBits);
        RequireProduct(right.Numerator, left.Denominator, maxBits);
        RequireProduct(left.Denominator, right.Denominator, maxBits);

        // The sum of two products may need one extra bit. Check before constructing it.
        var first = left.Numerator * right.Denominator;
        var second = right.Numerator * left.Denominator;
        if (BigInteger.Abs(first).GetBitLength() >= maxBits ||
            BigInteger.Abs(second).GetBitLength() >= maxBits)
            throw new ArithmeticException("Intermediate sum exceeds the exact bit budget.");
        var numerator = first + second;
        RequireWithin(numerator, maxBits);
        var denominator = left.Denominator * right.Denominator;
        return new ExactRational(numerator, denominator);
    }

    public static ExactRational Multiply(ExactRational left, ExactRational right, int maxBits)
    {
        Validate(maxBits);
        RequireWithin(left, maxBits);
        RequireWithin(right, maxBits);
        RequireProduct(left.Numerator, right.Numerator, maxBits);
        RequireProduct(left.Denominator, right.Denominator, maxBits);
        return new ExactRational(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
    }

    private static void Validate(int bits)
    {
        if (bits < 2)
            throw new ArgumentOutOfRangeException(nameof(bits), "The exact bit budget must be at least two.");
    }

    private static void RequireWithin(ExactRational value, int bits)
    {
        RequireWithin(value.Numerator, bits);
        RequireWithin(value.Denominator, bits);
    }

    private static void RequireWithin(BigInteger value, int bits)
    {
        if (BigInteger.Abs(value).GetBitLength() > bits)
            throw new ArithmeticException("Exact rational exceeds the configured bit budget.");
    }

    private static void RequireProduct(BigInteger left, BigInteger right, int bits)
    {
        var a = BigInteger.Abs(left).GetBitLength();
        var b = BigInteger.Abs(right).GetBitLength();
        if (left.IsZero || right.IsZero) return;
        if (a + b > bits + 1L)
            throw new ArithmeticException("Exact intermediate product exceeds the configured bit budget.");
    }
}
