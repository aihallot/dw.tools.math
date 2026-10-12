using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Numerics;

/// <summary>Policy for an explicitly requested exact-to-binary64 conversion.</summary>
public enum RationalBinary64Policy
{
    RequireExact,
    AllowRounded
}

/// <summary>Conversion outcome, never inferred from the presence of a double alone.</summary>
public enum RationalBinary64Status
{
    Exact,
    Rounded,
    Underflow,
    Overflow,
    BudgetExceeded
}

/// <summary>
/// Exact absolute rounding error is supplied for every accepted finite value.
/// An unsuccessful conversion never supplies a numeric value.
/// </summary>
public sealed record RationalBinary64Result(
    RationalBinary64Status Status,
    double? Value,
    ExactRational? AbsoluteError)
{
    public bool IsFiniteResult => Value.HasValue &&
        Status is RationalBinary64Status.Exact or RationalBinary64Status.Rounded;
}

/// <summary>
/// Bounded exact rational to correctly-rounded IEEE 754 binary64, ties to even.
/// No silent widening and no dependency on an external numerical provider.
/// </summary>
public static class RationalBinary64Conversion
{
    public const int MaximumOperandBits = 4096;

    private static readonly BigInteger NormalUnit = BigInteger.One << 52;
    private static readonly BigInteger OverflowedSignificand = BigInteger.One << 53;

    public static RationalBinary64Result TryConvert(ExactRational rational)
    {
        var numerator = rational.Numerator;
        var denominator = rational.Denominator;
        if (numerator.IsZero)
            return new(RationalBinary64Status.Exact, 0d, ExactRational.Zero);

        var absolute = BigInteger.Abs(numerator);
        var numeratorBits = absolute.GetBitLength();
        var denominatorBits = denominator.GetBitLength();
        if (numeratorBits > MaximumOperandBits || denominatorBits > MaximumOperandBits)
            return Rejected(RationalBinary64Status.BudgetExceeded);

        // The two compared integers have a maximum of 4096+1074 bits.
        var exponent = (int)(numeratorBits - denominatorBits);
        if (exponent >= 0)
        {
            if (absolute < (denominator << exponent)) exponent--;
        }
        else if ((absolute << -exponent) < denominator)
        {
            exponent--;
        }

        if (exponent > 1023)
            return Rejected(RationalBinary64Status.Overflow);

        ulong bits;
        if (exponent >= -1022)
        {
            var shift = 52 - exponent;
            var significand = shift >= 0
                ? RoundedQuotient(absolute << shift, denominator)
                : RoundedQuotient(absolute, denominator << -shift);

            if (significand == OverflowedSignificand)
            {
                significand = NormalUnit;
                exponent++;
            }

            if (exponent > 1023)
                return Rejected(RationalBinary64Status.Overflow);

            bits = ((ulong)(exponent + 1023) << 52) |
                (ulong)(significand - NormalUnit);
        }
        else
        {
            // The exact unit in the subnormal region is 2^-1074.
            var significand = RoundedQuotient(absolute << 1074, denominator);
            if (significand.IsZero)
                return Rejected(RationalBinary64Status.Underflow);

            bits = significand == NormalUnit
                ? 1UL << 52 // Rounding crossed the smallest-normal boundary.
                : (ulong)significand;
        }

        if (numerator.Sign < 0)
            bits |= 1UL << 63;

        var rounded = BitConverter.UInt64BitsToDouble(bits);
        if (!double.IsFinite(rounded))
            return Rejected(RationalBinary64Status.Overflow);

        var error = (ExactBinaryNumber.FromDouble(rounded) - rational).Abs();
        return error == ExactRational.Zero
            ? new(RationalBinary64Status.Exact, rounded, ExactRational.Zero)
            : new(RationalBinary64Status.Rounded, rounded, error);
    }

    private static BigInteger RoundedQuotient(BigInteger numerator, BigInteger denominator)
    {
        var integral = BigInteger.DivRem(numerator, denominator, out var remainder);
        var comparison = (remainder << 1).CompareTo(denominator);
        return comparison > 0 || (comparison == 0 && !integral.IsEven)
            ? integral + BigInteger.One
            : integral;
    }

    private static RationalBinary64Result Rejected(RationalBinary64Status status) =>
        new(status, null, null);
}

/// <summary>
/// Atomic matrix conversion: no partial finite matrix is returned on refusal.
/// The caller explicitly chooses whether rounding is admissible.
/// </summary>
public sealed record ExactMatrixBinary64Result(
    RationalBinary64Status Status,
    FiniteMatrix64? Matrix,
    int RoundedElements,
    ExactRational? MaximumAbsoluteError,
    int? FirstRejectedIndex)
{
    public bool Accepted => Matrix is not null;
}

public static class ExactMatrixBinary64Conversion
{
    public static ExactMatrixBinary64Result TryConvert(
        IrMatrix matrix,
        RationalBinary64Policy policy)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        if (!Enum.IsDefined(policy))
            throw new ArgumentOutOfRangeException(nameof(policy));
        if (matrix.Elements.Any(element => element is not IrExactScalar))
            throw new NotSupportedException(
                "Only exact rational IR matrices may use explicit rational conversion.");

        var values = new double[matrix.Elements.Length];
        var rounded = 0;
        var largestError = ExactRational.Zero;

        for (var index = 0; index < matrix.Elements.Length; index++)
        {
            var exact = ((IrExactScalar)matrix.Elements[index]).Value;
            var result = RationalBinary64Conversion.TryConvert(exact);
            if (!result.IsFiniteResult ||
                policy == RationalBinary64Policy.RequireExact &&
                result.Status == RationalBinary64Status.Rounded)
                return new(result.Status, null, rounded, null, index);

            values[index] = result.Value!.Value;
            if (result.Status == RationalBinary64Status.Rounded)
                rounded++;
            var error = result.AbsoluteError!.Value;
            if (error > largestError)
                largestError = error;
        }

        return new(
            rounded == 0 ? RationalBinary64Status.Exact : RationalBinary64Status.Rounded,
            FiniteMatrix64.Create(matrix.Rows, matrix.Columns, values),
            rounded, largestError, null);
    }
}
