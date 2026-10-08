using System.Globalization;
using System.Numerics;

namespace dw.quantities;

public readonly record struct ExactRational : IComparable<ExactRational>
{
    public ExactRational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
        {
            throw new DivideByZeroException("An exact rational denominator cannot be zero.");
        }

        if (denominator.Sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
            denominator = BigInteger.Negate(denominator);
        }

        var divisor = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
        Numerator = numerator / divisor;
        _denominatorMinusOne = denominator / divisor - BigInteger.One;
    }

    public BigInteger Numerator { get; }

    // Store denominator minus one so default(ExactRational) is canonical zero (0/1).
    // This also preserves value equality with ExactRational.Zero.
    private readonly BigInteger _denominatorMinusOne;

    public BigInteger Denominator => _denominatorMinusOne + BigInteger.One;

    public static ExactRational Zero { get; } = new(BigInteger.Zero, BigInteger.One);

    public static ExactRational One { get; } = new(BigInteger.One, BigInteger.One);

    public ExactRational Abs() => Numerator.Sign < 0
        ? new ExactRational(BigInteger.Abs(Numerator), Denominator)
        : this;

    public int CompareTo(ExactRational other) =>
        (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);

    public static ExactRational ParseInvariantDecimal(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var span = value.AsSpan();
        var sign = 1;
        if (span[0] is '+' or '-')
        {
            sign = span[0] == '-' ? -1 : 1;
            span = span[1..];
        }

        if (span.IsEmpty)
        {
            throw new FormatException("The exact decimal has no digits.");
        }

        var decimalIndex = span.IndexOf('.');
        ReadOnlySpan<char> integral = decimalIndex < 0 ? span : span[..decimalIndex];
        ReadOnlySpan<char> fractional = decimalIndex < 0 ? [] : span[(decimalIndex + 1)..];
        if ((integral.IsEmpty && fractional.IsEmpty) ||
            !AllAsciiDigits(integral) || !AllAsciiDigits(fractional))
        {
            throw new FormatException("The exact decimal must use invariant base-10 notation.");
        }

        var digits = string.Concat(integral.IsEmpty ? "0" : integral.ToString(), fractional.ToString());
        var numerator = BigInteger.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture);
        if (sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
        }

        return new ExactRational(numerator, BigInteger.Pow(10, fractional.Length));
    }

    public static ExactRational ParseInvariant(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var parts = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            1 when !parts[0].Contains("/", StringComparison.Ordinal) =>
                ParseInvariantDecimal(parts[0]),
            1 => ParseSimpleFraction(parts[0]),
            2 => ParseMixedNumber(parts[0], parts[1]),
            _ => throw new FormatException("The exact number must be a decimal, fraction, or mixed number."),
        };
    }

    public string ToFractionString() => Denominator.IsOne
        ? Numerator.ToString(CultureInfo.InvariantCulture)
        : string.Create(
            CultureInfo.InvariantCulture,
            $"{Numerator}/{Denominator}");

    public string ToMixedNumberString()
    {
        var absoluteNumerator = BigInteger.Abs(Numerator);
        var whole = BigInteger.DivRem(absoluteNumerator, Denominator, out var remainder);
        var sign = Numerator.Sign < 0 ? "-" : string.Empty;

        if (remainder.IsZero)
        {
            return string.Concat(sign, whole.ToString(CultureInfo.InvariantCulture));
        }

        if (whole.IsZero)
        {
            return string.Create(
                CultureInfo.InvariantCulture,
                $"{sign}{remainder}/{Denominator}");
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{sign}{whole} {remainder}/{Denominator}");
    }

    public static ExactRational operator +(ExactRational left, ExactRational right) =>
        new(
            left.Numerator * right.Denominator + right.Numerator * left.Denominator,
            left.Denominator * right.Denominator);

    public static ExactRational operator -(ExactRational left, ExactRational right) =>
        new(
            left.Numerator * right.Denominator - right.Numerator * left.Denominator,
            left.Denominator * right.Denominator);

    public static ExactRational operator *(ExactRational left, ExactRational right) =>
        new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public static ExactRational operator /(ExactRational left, ExactRational right)
    {
        if (right.Numerator.IsZero)
        {
            throw new DivideByZeroException("Cannot divide an exact rational by zero.");
        }

        return new ExactRational(
            left.Numerator * right.Denominator,
            left.Denominator * right.Numerator);
    }

    public static bool operator <(ExactRational left, ExactRational right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(ExactRational left, ExactRational right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(ExactRational left, ExactRational right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(ExactRational left, ExactRational right) =>
        left.CompareTo(right) >= 0;

    private static ExactRational ParseSimpleFraction(string value)
    {
        var parts = value.Split('/');
        if (parts.Length != 2 ||
            !BigInteger.TryParse(parts[0], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var numerator) ||
            !BigInteger.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var denominator) ||
            denominator <= BigInteger.Zero)
        {
            throw new FormatException("The exact fraction must use numerator/positive-denominator notation.");
        }

        return new ExactRational(numerator, denominator);
    }

    private static ExactRational ParseMixedNumber(string wholeText, string fractionText)
    {
        if (!BigInteger.TryParse(
                wholeText,
                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var whole))
        {
            throw new FormatException("The mixed number has an invalid whole component.");
        }

        var fraction = ParseSimpleFraction(fractionText);
        if (fraction.Numerator.Sign < 0 || fraction.Numerator >= fraction.Denominator)
        {
            throw new FormatException("The mixed number requires a non-negative proper fraction.");
        }

        var magnitude = new ExactRational(
            BigInteger.Abs(whole) * fraction.Denominator + fraction.Numerator,
            fraction.Denominator);
        return wholeText.StartsWith("-", StringComparison.Ordinal)
            ? new ExactRational(BigInteger.Negate(magnitude.Numerator), magnitude.Denominator)
            : magnitude;
    }

    private static bool AllAsciiDigits(ReadOnlySpan<char> value)
    {
        foreach (var character in value)
        {
            if (character is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }
}
