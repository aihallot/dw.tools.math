using System.Globalization;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Codecs;

/// <summary>Parsing is a bounded structural transport, never evaluation or TeX execution.</summary>
public enum CanonicalNotationStatus
{
    Admitted,
    InvalidInput,
    NonCanonical,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedApproximation,
    BudgetExceeded
}

/// <summary>
/// One admitted expression has an exact IR source, canonical ASCII text,
/// display-only LaTeX and stable semantic structural identity.
/// A refusal has no mathematical result or presented notation.
/// </summary>
public sealed record CanonicalNotationResult(
    CanonicalNotationStatus Status,
    IrNode? Expression,
    string? CanonicalText,
    string? Latex,
    string? SemanticStructuralSha256)
{
    public bool IsAdmitted =>
        Status == CanonicalNotationStatus.Admitted &&
        Expression is not null && CanonicalText is not null &&
        Latex is not null && SemanticStructuralSha256 is not null;
}

/// <summary>
/// Explicitly closed T1 notation grammar. Accepted text is canonical and
/// ASCII-only: exact rationals, free real x/y/z, prefix add/sub/mul/div,
/// square/pow/sqrt/abs, three whitelisted units and typed eq/ne/ge
/// original-domain restrictions. No scripting, general TeX parser or CAS.
/// </summary>
public static class CanonicalMathNotation
{
    public const string Grammar = "dw-math-canonical-text/1";
    public const string LatexRule = "dw-math-latex-display-only/1";
    public const int MaximumInputCharacters = 1024;
    public const int MaximumLatexCharacters = 4096;
    public const int MaximumDepth = 16;
    public const int MaximumRationalBits = 128;
    public const int MaximumArguments = 8;
    public const int MaximumConditions = 8;
    public const int MaximumPower = 8;

    /// <summary>
    /// Parse a canonical grammar string into the existing exact Math IR.
    /// Never accept a noncanonical alias or infer a symbolic simplification.
    /// </summary>
    public static CanonicalNotationResult Parse(string canonicalText)
    {
        ArgumentNullException.ThrowIfNull(canonicalText);
        if (canonicalText.Length == 0)
            return Refuse(CanonicalNotationStatus.InvalidInput);
        if (canonicalText.Length > MaximumInputCharacters)
            return Refuse(CanonicalNotationStatus.BudgetExceeded);
        if (canonicalText.Any(ch => ch > 127 || char.IsWhiteSpace(ch)))
            return Refuse(CanonicalNotationStatus.InvalidInput);
        try
        {
            var parser = new Reader(canonicalText);
            var node = parser.ReadNode(1);
            if (!parser.AtEnd)
                return Refuse(CanonicalNotationStatus.InvalidInput);
            var rendered = Render(node);
            if (!rendered.IsAdmitted)
                return rendered;
            return string.Equals(rendered.CanonicalText, canonicalText,
                    StringComparison.Ordinal)
                ? rendered : Refuse(CanonicalNotationStatus.NonCanonical);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(CanonicalNotationStatus.BudgetExceeded);
        }
        catch (UnsupportedNotationException error)
        {
            return Refuse(error.Status);
        }
        catch (NotSupportedException)
        {
            return Refuse(CanonicalNotationStatus.Unsupported);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or DivideByZeroException)
        {
            return Refuse(CanonicalNotationStatus.InvalidInput);
        }
    }

    /// <summary>
    /// Render an admitted Math IR node as a canonical text representation
    /// plus display-only LaTeX. Never allow arbitrary labels to become TeX.
    /// </summary>
    public static CanonicalNotationResult Render(IrNode source)
    {
        ArgumentNullException.ThrowIfNull(source);
        try
        {
            IrGraphLimits.Validate(source);
            Inspect(source);
            var canonical = WriteText(source, 1);
            if (canonical.Length > MaximumInputCharacters)
                return Refuse(CanonicalNotationStatus.BudgetExceeded);
            var latex = WriteLatex(source, 1);
            if (latex.Length > MaximumLatexCharacters)
                return Refuse(CanonicalNotationStatus.BudgetExceeded);
            return new(CanonicalNotationStatus.Admitted, source, canonical,
                latex, IrCanonicalHashes.SemanticStructuralSha256(source));
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(CanonicalNotationStatus.BudgetExceeded);
        }
        catch (UnsupportedNotationException error)
        {
            return Refuse(error.Status);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or NotSupportedException)
        {
            return Refuse(CanonicalNotationStatus.Unsupported);
        }
    }

    private static void Inspect(IrNode source)
    {
        var stack = new Stack<IrNode>();
        stack.Push(source);
        while (stack.Count > 0)
        {
            switch (stack.Pop())
            {
                case IrFiniteBinary64Scalar:
                    throw new UnsupportedNotationException(
                        CanonicalNotationStatus.UnsupportedApproximation);
                case IrSymbol symbol when !SupportedSymbol(symbol):
                    throw new UnsupportedNotationException(
                        CanonicalNotationStatus.UnsupportedDomain);
                case IrExactScalar exact when !Bounded(exact.Value):
                    throw new ArgumentOutOfRangeException(nameof(source));
                case IrQuantityLiteral unit:
                    if (unit.Value is not IrExactScalar literal ||
                        !Bounded(literal.Value) || !MatchesUnit(unit))
                        throw new UnsupportedNotationException(
                            CanonicalNotationStatus.UnsupportedUnits);
                    stack.Push(unit.Value);
                    break;
                case IrRestrictedExpression restricted:
                    if (restricted.Assumptions.Conditions.IsEmpty ||
                        restricted.Assumptions.Conditions.Length > MaximumConditions)
                        throw new UnsupportedNotationException(
                            CanonicalNotationStatus.Unsupported);
                    foreach (var condition in restricted.Assumptions.Conditions)
                    {
                        if (!SupportedSymbol(condition.Symbol) ||
                            !Bounded(condition.Right.Value))
                            throw new UnsupportedNotationException(
                                CanonicalNotationStatus.UnsupportedDomain);
                        if (condition.Kind is not (IrRelationKind.Equal or
                            IrRelationKind.NotEqual or IrRelationKind.GreaterOrEqual))
                            throw new UnsupportedNotationException(
                                CanonicalNotationStatus.Unsupported);
                    }
                    stack.Push(restricted.Expression);
                    break;
                case IrApply apply:
                    if (!AllowedOperation(apply))
                        throw new UnsupportedNotationException(
                            CanonicalNotationStatus.Unsupported);
                    foreach (var item in apply.Arguments)
                        stack.Push(item);
                    break;
                case IrExactScalar:
                    break;
                default:
                    throw new UnsupportedNotationException(
                        CanonicalNotationStatus.Unsupported);
            }
        }
    }

    private static bool AllowedOperation(IrApply apply)
    {
        if (apply.Arguments.Length > MaximumArguments)
            return false;
        if (apply.Operation is IrOperation.Add or IrOperation.Multiply)
            return apply.Arguments.Length >= 2;
        if (apply.Operation is IrOperation.Subtract or IrOperation.Divide)
            return apply.Arguments.Length == 2;
        if (apply.Operation is IrOperation.Square or IrOperation.Sqrt or
            IrOperation.Abs)
            return apply.Arguments.Length == 1;
        return apply.Operation == IrOperation.Power &&
            apply.Arguments.Length == 2 &&
            apply.Arguments[1] is IrExactScalar exponent &&
            exponent.Value.Denominator == BigInteger.One &&
            exponent.Value.Numerator >= BigInteger.Zero &&
            exponent.Value.Numerator <= MaximumPower;
    }

    private static bool SupportedSymbol(IrSymbol symbol) =>
        !symbol.IsBound && symbol.Domain == IrScalarDomain.Real &&
        symbol.Identity is "free:x" or "free:y" or "free:z" &&
        symbol.DisplayName.Length == 1 &&
        symbol.DisplayName[0] == symbol.Identity[^1];

    private static bool Bounded(ExactRational value) =>
        BigInteger.Abs(value.Numerator).GetBitLength() <= MaximumRationalBits &&
        value.Denominator.GetBitLength() <= MaximumRationalBits;

    private static bool MatchesUnit(IrQuantityLiteral literal)
    {
        var mapping = Unit(literal.CanonicalUnitId, literal.Temperature);
        return mapping is not null &&
            literal.System == mapping.Value.System &&
            literal.Dimension == mapping.Value.Dimension;
    }

    private static (UnitSystem System, DimensionVector Dimension,
        string Latex)? Unit(string id, IrTemperatureKind kind) =>
        (id, kind) switch
        {
            ("si.metre", IrTemperatureKind.None) =>
                (UnitSystem.Si, DimensionVector.LengthDimension, "m"),
            ("imperial.inch", IrTemperatureKind.None) =>
                (UnitSystem.BritishImperial, DimensionVector.LengthDimension, "in"),
            ("si.kelvin", IrTemperatureKind.Absolute) =>
                (UnitSystem.Si, DimensionVector.TemperatureDimension,
                    "K"),
            _ => null
        };

    private static string WriteText(IrNode source, int depth)
    {
        if (depth > MaximumDepth)
            throw new ArgumentOutOfRangeException(nameof(depth));
        switch (source)
        {
            case IrExactScalar rational:
                return rational.Value.ToFractionString();
            case IrSymbol symbol:
                return symbol.DisplayName;
            case IrQuantityLiteral unit:
                return "unit(" + WriteText(unit.Value, depth + 1) + "," +
                    unit.CanonicalUnitId + "," + unit.Temperature + ")";
            case IrRestrictedExpression restricted:
                return "where(" + WriteText(restricted.Expression, depth + 1) +
                    "," + string.Join(",",
                        restricted.Assumptions.Conditions.Select(
                            c => ConditionText(c, depth + 1))) + ")";
            case IrApply application:
            {
                var word = application.Operation switch
                {
                    IrOperation.Add => "add",
                    IrOperation.Subtract => "sub",
                    IrOperation.Multiply => "mul",
                    IrOperation.Divide => "div",
                    IrOperation.Power => "pow",
                    IrOperation.Square => "square",
                    IrOperation.Sqrt => "sqrt",
                    IrOperation.Abs => "abs",
                    _ => throw new NotSupportedException(
                        "The Math notation operation is not admitted.")
                };
                return word + "(" + string.Join(",",
                    application.Arguments.Select(node => WriteText(node, depth + 1))) + ")";
            }
            default:
                throw new NotSupportedException("The Math IR type is not admitted.");
        }
    }

    private static string ConditionText(IrRelation condition, int depth)
    {
        var name = condition.Kind switch
        {
            IrRelationKind.Equal => "eq",
            IrRelationKind.NotEqual => "ne",
            IrRelationKind.GreaterOrEqual => "ge",
            _ => throw new NotSupportedException("Unsupported condition.")
        };
        return name + "(" + WriteText(condition.Symbol, depth + 1) +
            "," + WriteText(condition.Right, depth + 1) + ")";
    }

    private static string WriteLatex(IrNode source, int depth)
    {
        if (depth > MaximumDepth)
            throw new ArgumentOutOfRangeException(nameof(depth));
        switch (source)
        {
            case IrExactScalar rational:
            {
                var value = rational.Value;
                if (value.Denominator.IsOne)
                    return value.Numerator.ToString(CultureInfo.InvariantCulture);
                var sign = value.Numerator.Sign < 0 ? "-" : "";
                return sign + "\\frac{" +
                    BigInteger.Abs(value.Numerator).ToString(CultureInfo.InvariantCulture) +
                    "}{" + value.Denominator.ToString(CultureInfo.InvariantCulture) + "}";
            }
            case IrSymbol symbol:
                return symbol.DisplayName;
            case IrQuantityLiteral unit:
            {
                var mapping = Unit(unit.CanonicalUnitId, unit.Temperature)
                    ?? throw new NotSupportedException("Unqualified unit.");
                return WriteLatex(unit.Value, depth + 1) + "\\,\\mathrm{" +
                    (unit.Temperature == IrTemperatureKind.Absolute ? "K" : mapping.Latex) + "}" +
                    (unit.Temperature == IrTemperatureKind.Absolute ? "_{\\mathrm{absolute}}" : "");
            }
            case IrRestrictedExpression restricted:
                return "\\left." + WriteLatex(restricted.Expression, depth + 1) +
                    "\\right|_{" +
                    string.Join(",\\;",restricted.Assumptions.Conditions.Select(
                        c => ConditionLatex(c, depth + 1))) + "}";
            case IrApply expression:
            {
                var args = expression.Arguments.Select(x => WriteLatex(x, depth + 1))
                    .ToArray();
                return expression.Operation switch
                {
                    IrOperation.Add => "\\left(" + string.Join("+", args) + "\\right)",
                    IrOperation.Subtract => "\\left(" + args[0] + "-" + args[1] + "\\right)",
                    IrOperation.Multiply => "\\left(" + string.Join("\\cdot ", args) + "\\right)",
                    IrOperation.Divide => "\\frac{" + args[0] + "}{" + args[1] + "}",
                    IrOperation.Power => "\\left(" + args[0] + "\\right)^{" +
                        ((IrExactScalar)expression.Arguments[1]).Value.ToFractionString() + "}",
                    IrOperation.Square => "\\left(" + args[0] + "\\right)^{2}",
                    IrOperation.Sqrt => "\\sqrt{" + args[0] + "}",
                    IrOperation.Abs => "\\left|" + args[0] + "\\right|",
                    _ => throw new NotSupportedException("Unknown LaTeX operation.")
                };
            }
            default:
                throw new NotSupportedException("The Math IR node is not displayable.");
        }
    }

    private static string ConditionLatex(IrRelation condition, int depth)
    {
        var symbol = WriteLatex(condition.Symbol, depth + 1);
        var right = WriteLatex(condition.Right, depth + 1);
        var relation = condition.Kind switch
        {
            IrRelationKind.Equal => "=",
            IrRelationKind.NotEqual => "\\ne ",
            IrRelationKind.GreaterOrEqual => "\\ge ",
            _ => throw new NotSupportedException("Unknown relation.")
        };
        return symbol + relation + right;
    }

    private static CanonicalNotationResult Refuse(CanonicalNotationStatus status) =>
        new(status, null, null, null, null);

    private sealed class UnsupportedNotationException : Exception
    {
        internal UnsupportedNotationException(CanonicalNotationStatus status) =>
            Status = status;
        internal CanonicalNotationStatus Status { get; }
    }

    /// <summary>A closed structural reader. No TeX macros, escapes or code paths.</summary>
    private sealed class Reader
    {
        private readonly string _text;
        private int _position;

        internal Reader(string text) => _text = text;
        internal bool AtEnd => _position == _text.Length;

        internal IrNode ReadNode(int depth)
        {
            if (depth > MaximumDepth)
                throw new ArgumentOutOfRangeException(nameof(depth));
            if (AtEnd)
                throw new FormatException("Unexpected end of mathematical text.");
            if (Current == '-' || (Current >= '0' && Current <= '9'))
                return new IrExactScalar(ReadRational());
            var name = ReadWord();
            if (!Consume('('))
            {
                if (name is "x" or "y" or "z")
                    return IrSymbol.Free(name, name);
                throw new NotSupportedException("Only x/y/z are free real identifiers.");
            }
            switch (name)
            {
                case "add":
                    return IrApply.Create(IrOperation.Add, ReadMany(depth));
                case "mul":
                    return IrApply.Create(IrOperation.Multiply, ReadMany(depth));
                case "sub":
                    return ReadBinary(IrOperation.Subtract, depth);
                case "div":
                    return ReadBinary(IrOperation.Divide, depth);
                case "pow":
                {
                    var x = ReadNode(depth + 1);
                    Expect(',');
                    var exponent = ReadNode(depth + 1);
                    Expect(')');
                    if (exponent is not IrExactScalar scalar ||
                        scalar.Value.Denominator != BigInteger.One ||
                        scalar.Value.Numerator < BigInteger.Zero ||
                        scalar.Value.Numerator > MaximumPower)
                        throw new NotSupportedException("Exponent outside admitted bound.");
                    return IrApply.Create(IrOperation.Power, [x, exponent]);
                }
                case "square":
                    return ReadUnary(IrOperation.Square, depth);
                case "sqrt":
                    return ReadUnary(IrOperation.Sqrt, depth);
                case "abs":
                    return ReadUnary(IrOperation.Abs, depth);
                case "unit":
                {
                    var numeric = ReadNode(depth + 1);
                    if (numeric is not IrExactScalar value)
                        throw new NotSupportedException("Only exact scalar unit quantities.");
                    Expect(',');
                    var identifier = ReadWord(allowDot: true);
                    Expect(',');
                    var temperature = ReadWord(allowUppercase: true) switch
                    {
                        "None" => IrTemperatureKind.None,
                        "Absolute" => IrTemperatureKind.Absolute,
                        _ => throw new NotSupportedException("Unit temperature kind unsupported.")
                    };
                    Expect(')');
                    var definition = Unit(identifier, temperature) ??
                        throw new UnsupportedNotationException(
                            CanonicalNotationStatus.UnsupportedUnits);
                    return new IrQuantityLiteral(value, identifier,
                        definition.System, definition.Dimension, temperature);
                }
                case "where":
                {
                    var expression = ReadNode(depth + 1);
                    var conditions = new List<IrRelation>();
                    while (Consume(','))
                    {
                        if (conditions.Count == MaximumConditions)
                            throw new ArgumentOutOfRangeException(nameof(depth));
                        conditions.Add(ReadCondition(depth + 1));
                    }
                    Expect(')');
                    if (conditions.Count == 0)
                        throw new FormatException("A restriction requires a condition.");
                    return new IrRestrictedExpression(expression,
                        IrAssumptionSet.Create(conditions));
                }
                default:
                    throw new NotSupportedException("Unrecognized notation function.");
            }
        }

        private IrNode ReadUnary(IrOperation operation, int depth)
        {
            var operand = ReadNode(depth + 1);
            Expect(')');
            return IrApply.Create(operation, [operand]);
        }

        private IrNode ReadBinary(IrOperation operation, int depth)
        {
            var left = ReadNode(depth + 1);
            Expect(',');
            var right = ReadNode(depth + 1);
            Expect(')');
            return IrApply.Create(operation, [left, right]);
        }

        private IReadOnlyList<IrNode> ReadMany(int depth)
        {
            var arguments = new List<IrNode>();
            arguments.Add(ReadNode(depth + 1));
            while (Consume(','))
            {
                if (arguments.Count == MaximumArguments)
                    throw new ArgumentOutOfRangeException(nameof(depth));
                arguments.Add(ReadNode(depth + 1));
            }
            Expect(')');
            if (arguments.Count < 2)
                throw new FormatException("Add/mul require two or more terms.");
            return arguments;
        }

        private IrRelation ReadCondition(int depth)
        {
            if (depth > MaximumDepth)
                throw new ArgumentOutOfRangeException(nameof(depth));
            var name = ReadWord();
            Expect('(');
            var symbol = ReadNode(depth + 1) as IrSymbol ??
                throw new FormatException("An assumption requires a free symbol.");
            Expect(',');
            var scalar = ReadNode(depth + 1) as IrExactScalar ??
                throw new FormatException("An assumption needs an exact rational.");
            Expect(')');
            var kind = name switch
            {
                "eq" => IrRelationKind.Equal,
                "ne" => IrRelationKind.NotEqual,
                "ge" => IrRelationKind.GreaterOrEqual,
                _ => throw new NotSupportedException("Unsupported relation.")
            };
            return new IrRelation(symbol, kind, scalar);
        }

        private ExactRational ReadRational()
        {
            var start = _position;
            Consume('-');
            var digits = ReadDigits();
            if (digits == 0)
                throw new FormatException("An exact rational requires digits.");
            if (Consume('/') && ReadDigits() == 0)
                throw new FormatException("Missing positive rational denominator.");
            var value = ExactRational.ParseInvariant(_text[start.._position]);
            if (!Bounded(value))
                throw new ArgumentOutOfRangeException("canonicalText",
                    "Exact rational numerator or denominator exceeds the grammar bit budget.");
            return value;
        }

        private int ReadDigits()
        {
            var start = _position;
            while (!AtEnd && Current is >= '0' and <= '9')
                _position++;
            return _position - start;
        }

        private string ReadWord(bool allowDot = false, bool allowUppercase = false)
        {
            var start = _position;
            while (!AtEnd)
            {
                var value = Current;
                if ((value is >= 'a' and <= 'z') ||
                    (allowDot && value == '.') ||
                    (allowUppercase && value is >= 'A' and <= 'Z'))
                    _position++;
                else
                    break;
            }
            if (_position == start)
                throw new FormatException("Missing canonical identifier.");
            return _text[start.._position];
        }

        private char Current => _text[_position];

        private bool Consume(char character)
        {
            if (!AtEnd && Current == character)
            {
                _position++;
                return true;
            }
            return false;
        }

        private void Expect(char character)
        {
            if (!Consume(character))
                throw new FormatException("Unexpected canonical grammar delimiter.");
        }
    }
}
