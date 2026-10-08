using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C03Tests
{
    private static readonly IExpressionUnitResolver Units =
        new ExplicitProfileExpressionUnitResolver();

    [TestMethod]
    public void MinEquivalentMetreAndCentimetreIsExactAndStable()
    {
        var actual = Success("min(1 m,100 cm)");
        Assert.AreEqual(ExactRational.One, actual.Value);
        Assert.AreEqual(DimensionVector.LengthDimension, actual.Dimension);

        var first = new EvaluatedQuantity(ExactRational.One, DimensionVector.LengthDimension,
            TemperatureSemantics.None);
        var second = first with { Value = new ExactRational(100, 100) };
        var selected = ExactSelection.Minimum([first, second]);
        Assert.AreEqual(0, selected.Index);
        Assert.AreEqual(first, selected.Quantity);
    }

    [TestMethod]
    public void SelectionStableTiesPreserveFirstIndexAndChooseLaterStrictImprovements()
    {
        EvaluatedQuantity Q(long n) => new(new ExactRational(n, 1),
            DimensionVector.Scalar, TemperatureSemantics.None);
        Assert.AreEqual(0, ExactSelection.Maximum([Q(3), Q(3), Q(1)]).Index);
        Assert.AreEqual(0, ExactSelection.Minimum([Q(1), Q(1), Q(3)]).Index);
        Assert.AreEqual(1, ExactSelection.Minimum([Q(3), Q(1), Q(1)]).Index);
        Assert.AreEqual(1, ExactSelection.Maximum([Q(1), Q(3), Q(3)]).Index);
    }

    [TestMethod]
    public void NestedMinAndMaxRemainRationalAndRespectOriginalDimensions()
    {
        Assert.AreEqual(new ExactRational(3, 2),
            Success("max(min(1 m,100 cm),1.5 m)").Value);
        Assert.AreEqual(new ExactRational(1, 2),
            Success("max(min(1/3,1/6),max(1/4,1/2))").Value);
        Assert.AreEqual(new ExactRational(2, 1),
            Success("abs(-2 m)").Value);
        Assert.AreEqual(DimensionVector.LengthDimension,
            Success("abs(-2 m)").Dimension);
    }

    [TestMethod]
    public void SelectionInputRequiresMultipleCompatibleOperands()
    {
        var scalar = new EvaluatedQuantity(ExactRational.One, DimensionVector.Scalar,
            TemperatureSemantics.None);
        var length = scalar with { Dimension = DimensionVector.LengthDimension };
        var absolute = scalar with { Temperature = TemperatureSemantics.Absolute };
        Assert.ThrowsExactly<ArgumentNullException>(() => ExactSelection.Minimum(null!));
        Assert.ThrowsExactly<ArgumentException>(() => ExactSelection.Minimum([scalar]));
        Assert.ThrowsExactly<ArgumentException>(() => ExactSelection.Maximum([scalar, length]));
        Assert.ThrowsExactly<ArgumentException>(() => ExactSelection.Minimum([scalar, absolute]));
    }

    [TestMethod]
    public void AbsoluteTemperatureSelectionPreservesAffineSemanticsButAbsRejectsPoints()
    {
        var selected = Success("min(0 degC,32 degF)");
        Assert.AreEqual(TemperatureSemantics.Absolute, selected.Temperature);
        Assert.AreEqual(new ExactRational(27315, 100), selected.Value);
        AssertFailure("abs(0 degC)", ExpressionFailureKind.TemperatureAlgebra);
        AssertFailure("min(0 degC,1 m)", ExpressionFailureKind.IncompatibleDimensions);
    }

    [TestMethod]
    public void SelectionArityAndUnknownFunctionsProduceStableSyntaxFailures()
    {
        foreach(var expression in new[] {
            "min()", "min(1)", "max()", "max(1)", "abs()", "abs(1,2)",
            "min(1,)", "max(1,2,)", "sin(1)", "eval(1)", "unknown(2)"
        })
            AssertFailure(expression, ExpressionFailureKind.Syntax);
    }

    [TestMethod]
    public void DivisionByZeroAndIncompatibleUnitsAreNotReclassified()
    {
        AssertFailure("1/0", ExpressionFailureKind.DivideByZero);
        AssertFailure("min(1 m,1 B)", ExpressionFailureKind.IncompatibleDimensions);
        AssertFailure("min(1 cup,2 cup)", ExpressionFailureKind.UnitAmbiguous);
        AssertFailure("min(1 m,2 m,3 cm)", ExpressionFailureKind.UnitAmbiguous == ExpressionFailureKind.Syntax
            ? ExpressionFailureKind.Syntax : ExpressionFailureKind.Syntax);
    }

    [TestMethod]
    public void EveryExpressionFailureHasDistinctStableNeutralCode()
    {
        var kinds = Enum.GetValues<ExpressionFailureKind>();
        var codes = kinds.Select(ExpressionDiagnostics.CodeFor).ToArray();
        Assert.AreEqual(kinds.Length, codes.Distinct(StringComparer.Ordinal).Count());
        foreach(var code in codes)
        {
            Assert.IsTrue(code.StartsWith("math.expression.", StringComparison.Ordinal));
            Assert.IsTrue(code.Length < ExpressionDiagnostics.MaximumMessageCharacters);
        }
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExpressionDiagnostics.CodeFor((ExpressionFailureKind)123456));
    }

    [TestMethod]
    public void DiagnosticsContainBoundedMessagesAndOriginalPositionsWithoutUserInput()
    {
        var expression = "1/0";
        var outcome = ExpressionParser.Evaluate(expression);
        var diagnostic = ExpressionDiagnostics.Describe(outcome);
        Assert.IsNotNull(diagnostic);
        Assert.AreEqual("math.expression.divide-by-zero", diagnostic.Code);
        Assert.AreEqual(1, diagnostic.Position);
        Assert.IsTrue(diagnostic.Message.Length <= ExpressionDiagnostics.MaximumMessageCharacters);
        Assert.IsFalse(diagnostic.Message.Contains(expression, StringComparison.Ordinal));
        Assert.IsNull(ExpressionDiagnostics.Describe(ExpressionParser.Evaluate("1+2")));
        Assert.ThrowsExactly<ArgumentNullException>(() => ExpressionDiagnostics.Describe(null!));
    }

    [TestMethod]
    public void DiagnosticsStayBoundedForMaximumAdmittedInputAndNeverEchoIt()
    {
        var userInput = new string('Z', MathExpressionLimits.MaximumCharacters);
        var result = ExpressionParser.Evaluate(userInput);
        var diagnostic = ExpressionDiagnostics.Describe(result);
        Assert.IsNotNull(diagnostic);
        Assert.AreEqual("math.expression.syntax", diagnostic.Code);
        Assert.IsTrue(diagnostic.Message.Length <= ExpressionDiagnostics.MaximumMessageCharacters);
        Assert.IsFalse(diagnostic.Message.Contains("ZZZZ", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ErrorCategoriesRemainTypedAtParserBoundary()
    {
        AssertFailure("root(2,2)", ExpressionFailureKind.Domain);
        AssertFailure("1 USD", ExpressionFailureKind.UnitNotFound);
        AssertFailure("2^17", ExpressionFailureKind.Domain);
        AssertFailure("1"+new string(' ',4096), ExpressionFailureKind.ComplexityLimit);
        AssertFailure(new string('9',257), ExpressionFailureKind.MagnitudeLimit);
        AssertFailure("1 m + 1 B", ExpressionFailureKind.IncompatibleDimensions);
    }

    private static EvaluatedQuantity Success(string expression)
    {
        var actual = ExpressionParser.Evaluate(expression, Units);
        if (actual is ExpressionEvaluationOutcome.Success success)
            return success.Quantity;
        throw new AssertFailedException("Expected success for: " + expression + ", got " + actual);
    }

    private static void AssertFailure(string expression, ExpressionFailureKind expected)
    {
        var outcome = ExpressionParser.Evaluate(expression, Units);
        if(outcome is not ExpressionEvaluationOutcome.Failure failure)
            throw new AssertFailedException("Expected failure " + expected + " for: " + expression);
        Assert.AreEqual(expected, failure.Kind, expression);
        var diagnostic = ExpressionDiagnostics.Describe(outcome);
        Assert.IsNotNull(diagnostic);
        Assert.AreEqual(ExpressionDiagnostics.CodeFor(expected), diagnostic.Code);
    }
}
