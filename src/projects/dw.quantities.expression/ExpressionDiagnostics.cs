namespace dw.quantities.expression;

/// <summary>
/// Bounded, nonlocalized diagnostic metadata; never embeds untrusted expression text.
/// </summary>
public sealed record ExpressionDiagnostic(string Code, int Position, string Message);

public static class ExpressionDiagnostics
{
    public const int MaximumMessageCharacters = 160;

    public static ExpressionDiagnostic? Describe(ExpressionEvaluationOutcome outcome)
    {
        ArgumentNullException.ThrowIfNull(outcome);
        return outcome switch
        {
            ExpressionEvaluationOutcome.Success => null,
            ExpressionEvaluationOutcome.Failure failure => DescribeFailure(failure),
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), "Unknown evaluation outcome.")
        };
    }

    public static string CodeFor(ExpressionFailureKind kind) => kind switch
    {
        ExpressionFailureKind.Syntax => "math.expression.syntax",
        ExpressionFailureKind.UnitNotFound => "math.expression.unit-not-found",
        ExpressionFailureKind.UnitAmbiguous => "math.expression.unit-ambiguous",
        ExpressionFailureKind.UnsupportedCulture => "math.expression.unsupported-culture",
        ExpressionFailureKind.IncompatibleDimensions => "math.expression.incompatible-dimensions",
        ExpressionFailureKind.DivideByZero => "math.expression.divide-by-zero",
        ExpressionFailureKind.Domain => "math.expression.domain",
        ExpressionFailureKind.MagnitudeLimit => "math.expression.magnitude-limit",
        ExpressionFailureKind.ComplexityLimit => "math.expression.complexity-limit",
        ExpressionFailureKind.TemperatureAlgebra => "math.expression.temperature-algebra",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), "Unknown expression failure kind.")
    };

    private static ExpressionDiagnostic DescribeFailure(ExpressionEvaluationOutcome.Failure failure)
    {
        var code = CodeFor(failure.Kind);
        return new ExpressionDiagnostic(code, failure.Position, "Exact expression rejected: " + code + ".");
    }
}
