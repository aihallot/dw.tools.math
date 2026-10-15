using System.Globalization;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>No incomplete LaTeX import can masquerade as an exact Math result.</summary>
public enum LatexImportStatus
{
    Admitted,
    Ambiguous,
    Invalid,
    Unsupported,
    NonCanonical,
    BudgetExceeded
}

/// <summary>Juxtaposition is never assigned mathematical meaning by default.</summary>
public enum LatexJuxtapositionConvention
{
    RequireExplicitOperator,
    Multiplication
}

/// <summary>
/// One admitted result retains typed IR, canonical exact Math text,
/// original structural identity and the explicit convention applied.
/// A refused result carries no mathematical expression or proof marker.
/// </summary>
public sealed record LatexImportResult(
    LatexImportStatus Status,
    IrNode? Expression,
    string? CanonicalText,
    string? SemanticStructuralSha256,
    string? RuleId,
    LatexJuxtapositionConvention? AppliedConvention)
{
    public bool IsAdmitted =>
        Status == LatexImportStatus.Admitted &&
        Expression is not null && CanonicalText is not null &&
        SemanticStructuralSha256 is not null && RuleId is not null &&
        AppliedConvention.HasValue;
}

/// <summary>
/// Non-executing bounded LaTeX importer for selected exact rational,
/// free-real-symbol and passive expression forms. No arbitrary TeX,
/// macro expansion, generic function name or CAS is admitted.
/// </summary>
public static class BoundedLatexImporter
{
    public const string Rule = "math-bounded-latex-exact-subset/v1";
    public const int MaximumInputCharacters = 512;
    public const int MaximumDepth = 12;
    public const int MaximumExactBits = 128;

    public static LatexImportResult Import(
        string latex,
        LatexJuxtapositionConvention convention =
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        ArgumentNullException.ThrowIfNull(latex);
        if (latex.Length == 0)
            return Refuse(LatexImportStatus.Invalid);
        if (latex.Length > MaximumInputCharacters)
            return Refuse(LatexImportStatus.BudgetExceeded);
        if (!Enum.IsDefined(convention))
            return Refuse(LatexImportStatus.Unsupported);
        if (latex.Any(character => character < ' ' || character > '~'))
            return Refuse(LatexImportStatus.Invalid);

        try
        {
            var parser = new Reader(latex, convention);
            var result = parser.ReadExpression(1);
            if (!parser.AtEnd)
                return Refuse(parser.UnconsumedStatus);
            var rendered = CanonicalMathNotation.Render(result);
            if (!rendered.IsAdmitted)
                return Refuse(rendered.Status == CanonicalNotationStatus.BudgetExceeded
                    ? LatexImportStatus.BudgetExceeded
                    : LatexImportStatus.Unsupported);
            return new(LatexImportStatus.Admitted,
                result, rendered.CanonicalText,
                IrCanonicalHashes.SemanticStructuralSha256(result),
                Rule, convention);
        }
        catch (ImportRefusal exception)
        {
            return Refuse(exception.Status);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(LatexImportStatus.BudgetExceeded);
        }
        catch (Exception exception) when (
            exception is ArgumentException or FormatException or
            DivideByZeroException)
        {
            return Refuse(LatexImportStatus.Invalid);
        }
        catch (NotSupportedException)
        {
            return Refuse(LatexImportStatus.Unsupported);
        }
    }

    private static LatexImportResult Refuse(LatexImportStatus status) =>
        new(status, null, null, null, null, null);

    private sealed class ImportRefusal : Exception
    {
        internal ImportRefusal(LatexImportStatus status) => Status = status;
        internal LatexImportStatus Status { get; }
    }

    private sealed class Reader
    {
        private readonly string _source;
        private readonly LatexJuxtapositionConvention _convention;
        private int _offset;

        internal Reader(string source, LatexJuxtapositionConvention convention)
        {
            _source = source;
            _convention = convention;
        }

        internal bool AtEnd => _offset == _source.Length;

        internal LatexImportStatus UnconsumedStatus =>
            !AtEnd && (_source[_offset] == '\\' || _source[_offset] == '^')
                ? LatexImportStatus.Unsupported
                : LatexImportStatus.Invalid;

        internal IrNode ReadExpression(int depth)
        {
            GuardDepth(depth);
            var left = ReadProduct(depth + 1);
            while (!AtEnd && Current is '+' or '-')
            {
                var operation = Current == '+'
                    ? IrOperation.Add : IrOperation.Subtract;
                _offset++;
                var right = ReadProduct(depth + 1);
                left = IrApply.Create(operation, [left, right]);
            }
            return left;
        }

        private IrNode ReadProduct(int depth)
        {
            GuardDepth(depth);
            var left = ReadPrimary(depth + 1);
            while (!AtEnd)
            {
                if (Starts("\\cdot"))
                {
                    _offset += "\\cdot".Length;
                    Consume(' ');
                    var right = ReadPrimary(depth + 1);
                    left = IrApply.Create(IrOperation.Multiply, [left, right]);
                    continue;
                }
                if (Current == '(' && left is IrSymbol)
                {
                    if (_convention == LatexJuxtapositionConvention.RequireExplicitOperator)
                        throw new ImportRefusal(LatexImportStatus.Ambiguous);
                    if (_convention != LatexJuxtapositionConvention.Multiplication)
                        throw new ImportRefusal(LatexImportStatus.Unsupported);
                    var right = ReadParenthesis(depth + 1);
                    left = IrApply.Create(IrOperation.Multiply, [left, right]);
                    continue;
                }
                if ((Current is 'x' or 'y' or 'z') && left is IrSymbol)
                    throw new ImportRefusal(LatexImportStatus.Ambiguous);
                if (Starts("^{2}"))
                    throw new ImportRefusal(LatexImportStatus.Ambiguous);
                break;
            }
            return left;
        }

        private IrNode ReadPrimary(int depth)
        {
            GuardDepth(depth);
            if (AtEnd)
                throw new FormatException("Missing LaTeX operand.");

            if (Starts("-\\frac{"))
            {
                _offset++;
                var positive = ReadFraction();
                return new IrExactScalar(new ExactRational(
                    BigInteger.Negate(positive.Numerator),positive.Denominator));
            }
            if (Starts("\\frac{"))
                return new IrExactScalar(ReadFraction());
            if (Starts("\\sqrt{"))
            {
                _offset += "\\sqrt{".Length;
                var inner = ReadExpression(depth + 1);
                Expect('}');
                if (inner is not (IrSymbol or IrExactScalar))
                    throw new ImportRefusal(LatexImportStatus.Unsupported);
                return IrApply.Create(IrOperation.Sqrt, [inner]);
            }
            if (Starts("\\left|"))
            {
                _offset += "\\left|".Length;
                var inner = ReadExpression(depth + 1);
                Expect("\\right|");
                return IrApply.Create(IrOperation.Abs, [inner]);
            }
            if (Starts("\\left("))
            {
                _offset += "\\left(".Length;
                var inner = ReadExpression(depth + 1);
                Expect("\\right)");
                if (Starts("^{2}"))
                    throw new ImportRefusal(LatexImportStatus.Ambiguous);
                if (!AtEnd && Current == '^')
                    throw new ImportRefusal(LatexImportStatus.Unsupported);
                return inner;
            }
            if (Current == '(')
                return ReadParenthesis(depth + 1);
            if (Current == '-' || (Current >= '0' && Current <= '9'))
                return new IrExactScalar(ReadInteger(true));
            if (Current is 'x' or 'y' or 'z')
            {
                var identity = Current.ToString();
                _offset++;
                return IrSymbol.Free(identity, identity);
            }
            if (Current == '\\' ||
                (Current >= 'a' && Current <= 'z') ||
                (Current >= 'A' && Current <= 'Z'))
                throw new ImportRefusal(LatexImportStatus.Unsupported);
            throw new FormatException("Unknown LaTeX token.");
        }

        private IrNode ReadParenthesis(int depth)
        {
            GuardDepth(depth);
            Expect('(');
            var node = ReadExpression(depth + 1);
            Expect(')');
            return node;
        }

        private ExactRational ReadFraction()
        {
            Expect("\\frac{");
            var numerator = ReadInteger(true);
            Expect('}');
            Expect('{');
            var denominator = ReadInteger(false);
            Expect('}');
            if (denominator <= BigInteger.Zero)
                throw new FormatException("Fraction denominator must be positive.");
            var gcd = BigInteger.GreatestCommonDivisor(
                BigInteger.Abs(numerator), denominator);
            if (denominator.IsOne || numerator.IsZero || gcd != BigInteger.One)
                throw new ImportRefusal(LatexImportStatus.NonCanonical);
            return new ExactRational(numerator, denominator);
        }

        private BigInteger ReadInteger(bool allowSign)
        {
            var negative = allowSign && Consume('-');
            var beginning = _offset;
            while (!AtEnd && Current is >= '0' and <= '9')
                _offset++;
            var length = _offset - beginning;
            if (length == 0)
                throw new FormatException("Expected decimal digits.");
            if (length > 39)
                throw new ArgumentOutOfRangeException("latex");
            var digits = _source[beginning.._offset];
            if (digits.Length > 1 && digits[0] == '0')
                throw new ImportRefusal(LatexImportStatus.NonCanonical);
            if (negative && digits == "0")
                throw new ImportRefusal(LatexImportStatus.NonCanonical);
            if (!BigInteger.TryParse(digits,NumberStyles.None,
                    CultureInfo.InvariantCulture,out var integer))
                throw new FormatException("Invalid exact integer.");
            if (integer.GetBitLength() > MaximumExactBits)
                throw new ArgumentOutOfRangeException("latex");
            return negative ? BigInteger.Negate(integer) : integer;
        }

        private bool Starts(string token) =>
            _source.AsSpan(_offset).StartsWith(token,StringComparison.Ordinal);

        private char Current => _source[_offset];

        private bool Consume(char ch)
        {
            if (!AtEnd && Current == ch)
            {
                _offset++;
                return true;
            }
            return false;
        }

        private void Expect(char ch)
        {
            if (!Consume(ch))
                throw new FormatException("Malformed bounded LaTeX delimiter.");
        }

        private void Expect(string token)
        {
            if (!Starts(token))
                throw new FormatException("Malformed bounded LaTeX delimiter.");
            _offset += token.Length;
        }

        private static void GuardDepth(int depth)
        {
            if (depth > MaximumDepth)
                throw new ArgumentOutOfRangeException(nameof(depth));
        }
    }
}
