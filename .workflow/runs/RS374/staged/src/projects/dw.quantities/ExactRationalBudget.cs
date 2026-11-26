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

    /// <summary>
    /// Converts an exact rational multiplied by a signed integer to Int64 without
    /// rounding. The bit budget covers normalized operands and the integral result.
    /// Refusal precedence is input budget, nonintegrality, result budget, then overflow.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The bit budget is less than two.</exception>
    public static ExactInt64ScaleResult TryScaleToInt64(ExactRational value, BigInteger scale, int maxBits)
    {
        Validate(maxBits);
        try
        {
            RequireWithin(value, maxBits);
            RequireWithin(scale, maxBits);
        }
        catch (ArithmeticException)
        {
            return new(ExactInt64ScaleStatus.ResourceLimit, null);
        }

        // A reduced rational is integral after scaling exactly when its
        // positive denominator divides the scale. Cancel before multiplying.
        var factor = BigInteger.DivRem(scale, value.Denominator, out var remainder);
        if (!remainder.IsZero)
            return new(ExactInt64ScaleStatus.NonIntegral, null);

        BigInteger result;
        try
        {
            // RequireProduct rejects definitely oversized products before allocation.
            // The ambiguous boundary needs at most one extra bit for the exact check.
            RequireProduct(value.Numerator, factor, maxBits);
            result = value.Numerator * factor;
            RequireWithin(result, maxBits);
        }
        catch (ArithmeticException)
        {
            return new(ExactInt64ScaleStatus.ResourceLimit, null);
        }

        if (result < long.MinValue || result > long.MaxValue)
            return new(ExactInt64ScaleStatus.Overflow, null);

        return new(ExactInt64ScaleStatus.Success, (long)result);
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

public enum ExactInt64ScaleStatus
{
    Success,
    NonIntegral,
    Overflow,
    ResourceLimit
}

/// <summary>An exact Int64 value is present only on successful conversion.</summary>
public sealed class ExactInt64ScaleResult
{
    internal ExactInt64ScaleResult(ExactInt64ScaleStatus status, long? value)
    {
        Status = status;
        Value = value;
    }

    public ExactInt64ScaleStatus Status { get; }

    public long? Value { get; }
}
