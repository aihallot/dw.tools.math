using System.Numerics;

namespace dw.quantities.expression;

public enum ExpressionUnitResolutionFailure
{
    None,
    UnsupportedCulture,
    UnitNotFound,
    AmbiguousUnit,
}

public sealed record ExpressionUnitResolution(
    UnitDefinition? Unit,
    ExpressionUnitResolutionFailure Failure,
    IReadOnlyList<string> CandidateIds);

public interface IExpressionUnitResolver
{
    ExpressionUnitResolution Resolve(string token, string culture);
}

public static class ExpressionDefaults
{
    public const string Culture = "metric-international";
}

public static class MathExpressionLimits
{
    public const int MaximumCharacters = 4096;
    public const int MaximumTokens = 256;
    public const int MaximumDepth = 32;
    public const int MaximumMagnitudeDigits = 256;
    public const int MaximumPower = 16;
    public const int MaximumRootDegree = 16;
}

public enum TemperatureSemantics
{
    None,
    Absolute,
    Interval,
}

public enum ExpressionFailureKind
{
    Syntax,
    UnitNotFound,
    UnitAmbiguous,
    UnsupportedCulture,
    IncompatibleDimensions,
    DivideByZero,
    Domain,
    MagnitudeLimit,
    ComplexityLimit,
    TemperatureAlgebra,
}

public readonly record struct EvaluatedQuantity(
    ExactRational Value,
    DimensionVector Dimension,
    TemperatureSemantics Temperature);

public abstract record ExpressionEvaluationOutcome
{
    private ExpressionEvaluationOutcome() { }

    public sealed record Success(EvaluatedQuantity Quantity) : ExpressionEvaluationOutcome;

    public sealed record Failure(ExpressionFailureKind Kind, int Position) : ExpressionEvaluationOutcome;
}

public static class ExpressionParser
{
    public static ExpressionEvaluationOutcome Evaluate(string expression) =>
        Evaluate(expression, MissingUnitResolver.Instance, ExpressionDefaults.Culture);

    public static ExpressionEvaluationOutcome Evaluate(
        string expression,
        IExpressionUnitResolver unitResolver,
        string culture = ExpressionDefaults.Culture)
    {
        ArgumentNullException.ThrowIfNull(unitResolver);

        if (string.IsNullOrWhiteSpace(expression) || expression.Length > MathExpressionLimits.MaximumCharacters)
        {
            return new ExpressionEvaluationOutcome.Failure(
                expression?.Length > MathExpressionLimits.MaximumCharacters
                    ? ExpressionFailureKind.ComplexityLimit
                    : ExpressionFailureKind.Syntax,
                0);
        }

        try
        {
            var parser = new Parser(Tokenize(expression), unitResolver, culture);
            var result = parser.Parse();
            return new ExpressionEvaluationOutcome.Success(result);
        }
        catch (ExpressionException exception)
        {
            return new ExpressionEvaluationOutcome.Failure(exception.Kind, exception.Position);
        }
        catch (OverflowException)
        {
            // Public parser failures must not leak dimension integer overflow.
            return new ExpressionEvaluationOutcome.Failure(ExpressionFailureKind.MagnitudeLimit, 0);
        }
    }

    private static IReadOnlyList<Token> Tokenize(string expression)
    {
        var tokens = new List<Token>();
        for (var index = 0; index < expression.Length;)
        {
            if (char.IsWhiteSpace(expression[index]))
            {
                index++;
                continue;
            }

            var start = index;
            var character = expression[index];
            if (character is >= '0' and <= '9' || character == '.')
            {
                var dots = 0;
                while (index < expression.Length &&
                       (expression[index] is >= '0' and <= '9' || expression[index] == '.'))
                {
                    dots += expression[index] == '.' ? 1 : 0;
                    index++;
                }

                if (dots > 1)
                {
                    throw new ExpressionException(ExpressionFailureKind.Syntax, start);
                }

                tokens.Add(new(TokenKind.Number, expression[start..index], start));
            }
            else if (char.IsLetter(character))
            {
                index++;
                while (index < expression.Length && IsIdentifierContinuation(expression, index))
                {
                    index++;
                }

                tokens.Add(new(TokenKind.Identifier, expression[start..index], start));
            }
            else
            {
                var kind = character switch
                {
                    '+' => TokenKind.Plus,
                    '-' => TokenKind.Minus,
                    '*' => TokenKind.Star,
                    '/' => TokenKind.Slash,
                    '^' => TokenKind.Caret,
                    '(' => TokenKind.LeftParenthesis,
                    ')' => TokenKind.RightParenthesis,
                    ',' => TokenKind.Comma,
                    _ => throw new ExpressionException(ExpressionFailureKind.Syntax, start),
                };
                tokens.Add(new(kind, character.ToString(), start));
                index++;
            }

            if (tokens.Count > MathExpressionLimits.MaximumTokens)
            {
                throw new ExpressionException(ExpressionFailureKind.ComplexityLimit, start);
            }
        }

        tokens.Add(new(TokenKind.End, string.Empty, expression.Length));
        return tokens;
    }

    private static bool IsIdentifierContinuation(string expression, int index)
    {
        var character = expression[index];
        if (char.IsLetterOrDigit(character) || character == '.')
        {
            return true;
        }

        return character == '-' && index + 1 < expression.Length && char.IsLetter(expression[index + 1]);
    }

    private sealed class Parser(
        IReadOnlyList<Token> tokens,
        IExpressionUnitResolver unitResolver,
        string culture)
    {
        private int index;
        private int depth;

        public EvaluatedQuantity Parse()
        {
            var value = ParseSum();
            Require(TokenKind.End);
            return value;
        }

        private EvaluatedQuantity ParseSum()
        {
            var left = ParseProduct();
            while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
            {
                var operation = Advance();
                left = Add(left, ParseProduct(), operation.Kind == TokenKind.Minus, operation.Position);
            }

            return left;
        }

        private EvaluatedQuantity ParseProduct()
        {
            var left = ParsePower();
            while (Current.Kind is TokenKind.Star or TokenKind.Slash)
            {
                var operation = Advance();
                left = Multiply(left, ParsePower(), operation.Kind == TokenKind.Slash, operation.Position);
            }

            return left;
        }

        private EvaluatedQuantity ParsePower()
        {
            var value = ParseUnary();
            if (Current.Kind != TokenKind.Caret)
            {
                return value;
            }

            var operation = Advance();
            var sign = Match(TokenKind.Minus) ? -1 : Match(TokenKind.Plus) ? 1 : 1;
            var token = Require(TokenKind.Number);
            if (token.Text.Contains('.') || !int.TryParse(token.Text, out var exponent) ||
                exponent > MathExpressionLimits.MaximumPower)
            {
                throw new ExpressionException(ExpressionFailureKind.Domain, token.Position);
            }

            return Power(value, exponent * sign, operation.Position);
        }

        private EvaluatedQuantity ParseUnary()
        {
            if (Match(TokenKind.Plus))
            {
                return ParseUnary();
            }

            if (Match(TokenKind.Minus))
            {
                var value = ParseUnary();
                return value with { Value = new ExactRational(-value.Value.Numerator, value.Value.Denominator) };
            }

            return ParsePrimary();
        }

        private EvaluatedQuantity ParsePrimary()
        {
            EnterDepth();
            try
            {
                if (Current.Kind == TokenKind.Number)
                {
                    return ParseLiteral();
                }

                if (Current.Kind == TokenKind.Identifier && Current.Text == "root")
                {
                    return ParseRoot();
                }

                if (Current.Kind == TokenKind.Identifier && (Current.Text is "abs" or "min" or "max"))
                {
                    return ParseSelectionFunction();
                }

                if (Match(TokenKind.LeftParenthesis))
                {
                    var value = ParseSum();
                    Require(TokenKind.RightParenthesis);
                    return value;
                }

                throw new ExpressionException(ExpressionFailureKind.Syntax, Current.Position);
            }
            finally
            {
                depth--;
            }
        }

        private EvaluatedQuantity ParseLiteral()
        {
            var number = Advance();
            if (number.Text.Length > MathExpressionLimits.MaximumMagnitudeDigits)
            {
                throw new ExpressionException(ExpressionFailureKind.MagnitudeLimit, number.Position);
            }

            ExactRational value;
            try
            {
                value = ExactRational.ParseInvariantDecimal(number.Text);
            }
            catch (FormatException)
            {
                throw new ExpressionException(ExpressionFailureKind.Syntax, number.Position);
            }

            if (Current.Kind != TokenKind.Identifier)
            {
                return Checked(new(value, DimensionVector.Scalar, TemperatureSemantics.None), number.Position);
            }

            var unitToken = Advance();
            var resolution = unitResolver.Resolve(unitToken.Text, culture);
            if (resolution.Failure != ExpressionUnitResolutionFailure.None)
            {
                var kind = resolution.Failure switch
                {
                    ExpressionUnitResolutionFailure.UnsupportedCulture => ExpressionFailureKind.UnsupportedCulture,
                    ExpressionUnitResolutionFailure.UnitNotFound => ExpressionFailureKind.UnitNotFound,
                    _ => ExpressionFailureKind.UnitAmbiguous,
                };
                throw new ExpressionException(kind, unitToken.Position);
            }

            var unit = resolution.Unit!;
            return Checked(new(
                unit.ToBase(value),
                unit.Dimension,
                unit.TransformKind == UnitTransformKind.AbsoluteTemperature
                    ? TemperatureSemantics.Absolute
                    : TemperatureSemantics.None), unitToken.Position);
        }

        private EvaluatedQuantity ParseSelectionFunction()
        {
            var operation = Advance();
            Require(TokenKind.LeftParenthesis);
            var selected = ParseSum();
            if (operation.Text == "abs")
            {
                Require(TokenKind.RightParenthesis);
                if (selected.Temperature == TemperatureSemantics.Absolute)
                    throw new ExpressionException(ExpressionFailureKind.TemperatureAlgebra, operation.Position);
                return selected with { Value = selected.Value.Abs() };
            }

            Require(TokenKind.Comma);
            var candidates = new List<EvaluatedQuantity> { selected };
            do
            {
                var candidate = ParseSum();
                if (candidate.Dimension != selected.Dimension)
                    throw new ExpressionException(ExpressionFailureKind.IncompatibleDimensions, operation.Position);
                if (candidate.Temperature != selected.Temperature)
                    throw new ExpressionException(ExpressionFailureKind.TemperatureAlgebra, operation.Position);
                candidates.Add(candidate);
            }
            while (Match(TokenKind.Comma));
            Require(TokenKind.RightParenthesis);
            return (operation.Text == "min"
                ? ExactSelection.Minimum(candidates)
                : ExactSelection.Maximum(candidates)).Quantity;
        }

        private EvaluatedQuantity ParseRoot()
        {
            var operation = Advance();
            Require(TokenKind.LeftParenthesis);
            var value = ParseSum();
            Require(TokenKind.Comma);
            var degreeToken = Require(TokenKind.Number);
            if (degreeToken.Text.Contains('.') || !int.TryParse(degreeToken.Text, out var degree) ||
                degree is < 2 or > MathExpressionLimits.MaximumRootDegree)
            {
                throw new ExpressionException(ExpressionFailureKind.Domain, degreeToken.Position);
            }

            Require(TokenKind.RightParenthesis);
            return Root(value, degree, operation.Position);
        }

        private static EvaluatedQuantity Add(EvaluatedQuantity left, EvaluatedQuantity right, bool subtract, int position)
        {
            if (left.Dimension != right.Dimension)
            {
                throw new ExpressionException(ExpressionFailureKind.IncompatibleDimensions, position);
            }

            var temperature = CombineTemperature(left.Temperature, right.Temperature, subtract, position);
            var value = subtract ? left.Value - right.Value : left.Value + right.Value;
            return Checked(new(value, left.Dimension, temperature), position);
        }

        private static TemperatureSemantics CombineTemperature(
            TemperatureSemantics left,
            TemperatureSemantics right,
            bool subtract,
            int position)
        {
            if (left == TemperatureSemantics.None && right == TemperatureSemantics.None)
            {
                return TemperatureSemantics.None;
            }

            return (left, right, subtract) switch
            {
                (TemperatureSemantics.Absolute, TemperatureSemantics.Absolute, true) => TemperatureSemantics.Interval,
                (TemperatureSemantics.Absolute, TemperatureSemantics.Interval, _) => TemperatureSemantics.Absolute,
                (TemperatureSemantics.Interval, TemperatureSemantics.Interval, _) => TemperatureSemantics.Interval,
                (TemperatureSemantics.Interval, TemperatureSemantics.Absolute, false) => TemperatureSemantics.Absolute,
                _ => throw new ExpressionException(ExpressionFailureKind.TemperatureAlgebra, position),
            };
        }

        private static EvaluatedQuantity Multiply(EvaluatedQuantity left, EvaluatedQuantity right, bool divide, int position)
        {
            if (left.Temperature == TemperatureSemantics.Absolute || right.Temperature == TemperatureSemantics.Absolute)
            {
                throw new ExpressionException(ExpressionFailureKind.TemperatureAlgebra, position);
            }

            if (divide && right.Value.Numerator.IsZero)
            {
                throw new ExpressionException(ExpressionFailureKind.DivideByZero, position);
            }

            return Checked(new(
                divide ? left.Value / right.Value : left.Value * right.Value,
                divide ? left.Dimension - right.Dimension : left.Dimension + right.Dimension,
                TemperatureSemantics.None), position);
        }

        private static EvaluatedQuantity Power(EvaluatedQuantity value, int exponent, int position)
        {
            if (value.Temperature == TemperatureSemantics.Absolute || exponent is < -MathExpressionLimits.MaximumPower or > MathExpressionLimits.MaximumPower)
            {
                throw new ExpressionException(ExpressionFailureKind.Domain, position);
            }

            if (exponent < 0 && value.Value.Numerator.IsZero)
            {
                throw new ExpressionException(ExpressionFailureKind.DivideByZero, position);
            }

            var power = Math.Abs(exponent);
            var numerator = BigInteger.Pow(value.Value.Numerator, power);
            var denominator = BigInteger.Pow(value.Value.Denominator, power);
            var rational = exponent < 0
                ? new ExactRational(denominator, numerator)
                : new ExactRational(numerator, denominator);
            return Checked(new(rational, Scale(value.Dimension, exponent), TemperatureSemantics.None), position);
        }

        private static EvaluatedQuantity Root(EvaluatedQuantity value, int degree, int position)
        {
            if (value.Temperature == TemperatureSemantics.Absolute || !Divisible(value.Dimension, degree) ||
                !TryExactRoot(value.Value.Numerator, degree, out var numerator) ||
                !TryExactRoot(value.Value.Denominator, degree, out var denominator))
            {
                throw new ExpressionException(ExpressionFailureKind.Domain, position);
            }

            return Checked(new(
                new ExactRational(numerator, denominator),
                Divide(value.Dimension, degree),
                TemperatureSemantics.None), position);
        }

        private static bool TryExactRoot(BigInteger value, int degree, out BigInteger root)
        {
            if (value.Sign < 0 && degree % 2 == 0)
            {
                root = default;
                return false;
            }

            var negative = value.Sign < 0;
            var target = BigInteger.Abs(value);
            var low = BigInteger.Zero;
            var high = target + BigInteger.One;
            while (low + BigInteger.One < high)
            {
                var middle = (low + high) / 2;
                if (BigInteger.Pow(middle, degree) <= target) low = middle;
                else high = middle;
            }

            root = negative ? -low : low;
            return BigInteger.Pow(root, degree) == value;
        }

        private static DimensionVector Scale(DimensionVector value, int factor) => new(
            checked(value.Length * factor), checked(value.Mass * factor), checked(value.Time * factor),
            checked(value.ElectricCurrent * factor), checked(value.Temperature * factor),
            checked(value.AmountOfSubstance * factor), checked(value.LuminousIntensity * factor),
            checked(value.Information * factor));

        private static DimensionVector Divide(DimensionVector value, int divisor) => new(
            value.Length / divisor, value.Mass / divisor, value.Time / divisor,
            value.ElectricCurrent / divisor, value.Temperature / divisor,
            value.AmountOfSubstance / divisor, value.LuminousIntensity / divisor,
            value.Information / divisor);

        private static bool Divisible(DimensionVector value, int divisor) =>
            value.Length % divisor == 0 && value.Mass % divisor == 0 && value.Time % divisor == 0 &&
            value.ElectricCurrent % divisor == 0 && value.Temperature % divisor == 0 &&
            value.AmountOfSubstance % divisor == 0 && value.LuminousIntensity % divisor == 0 &&
            value.Information % divisor == 0;

        private static EvaluatedQuantity Checked(EvaluatedQuantity value, int position)
        {
            if (Digits(value.Value.Numerator) > MathExpressionLimits.MaximumMagnitudeDigits ||
                Digits(value.Value.Denominator) > MathExpressionLimits.MaximumMagnitudeDigits)
            {
                throw new ExpressionException(ExpressionFailureKind.MagnitudeLimit, position);
            }

            return value;
        }

        private static int Digits(BigInteger value) => BigInteger.Abs(value).ToString().Length;

        private void EnterDepth()
        {
            depth++;
            if (depth > MathExpressionLimits.MaximumDepth)
            {
                throw new ExpressionException(ExpressionFailureKind.ComplexityLimit, Current.Position);
            }
        }

        private bool Match(TokenKind kind)
        {
            if (Current.Kind != kind) return false;
            index++;
            return true;
        }

        private Token Require(TokenKind kind)
        {
            if (Current.Kind != kind) throw new ExpressionException(ExpressionFailureKind.Syntax, Current.Position);
            return Advance();
        }

        private Token Advance() => tokens[index++];
        private Token Current => tokens[index];
    }

    private enum TokenKind { Number, Identifier, Plus, Minus, Star, Slash, Caret, LeftParenthesis, RightParenthesis, Comma, End }
    private readonly record struct Token(TokenKind Kind, string Text, int Position);
    private sealed class ExpressionException(ExpressionFailureKind kind, int position) : Exception
    {
        public ExpressionFailureKind Kind { get; } = kind;
        public int Position { get; } = position;
    }

    private sealed class MissingUnitResolver : IExpressionUnitResolver
    {
        public static MissingUnitResolver Instance { get; } = new();

        public ExpressionUnitResolution Resolve(string token, string culture) =>
            new(null, ExpressionUnitResolutionFailure.UnitNotFound, []);
    }
}
