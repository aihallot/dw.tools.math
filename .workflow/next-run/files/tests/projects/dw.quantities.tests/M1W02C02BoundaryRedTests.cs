using dw.quantities;
using dw.quantities.expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C02BoundaryRedTests
{
    private const string ScaleMarker =
        "M1-W02-C02-T2 RED: public evaluation wraps dimension exponent multiplication.";
    private const string AdditionMarker =
        "M1-W02-C02-T2 RED: public evaluation leaks checked dimension overflow.";

    [TestMethod]
    public void PublicExponentEvaluationMustRejectDimensionIntOverflow()
    {
        var unit = new UnitDefinition("huge", "huge", "max exponent dimension", UnitSystem.Si,
            new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0),
            ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
        var outcome = ExpressionParser.Evaluate("(2 huge)^2", new OneUnit(unit));
        Assert.IsTrue(outcome is ExpressionEvaluationOutcome.Failure
            { Kind: ExpressionFailureKind.MagnitudeLimit }, ScaleMarker);
    }

    [TestMethod]
    public void PublicMultiplyEvaluationMustConvertOverflowToTypedFailure()
    {
        var huge = new UnitDefinition("huge", "huge", "max exponent dimension", UnitSystem.Si,
            new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0),
            ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
        var length = new UnitDefinition("metre", "m", "metre", UnitSystem.Si,
            DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
            UnitTransformKind.Linear);
        ExpressionEvaluationOutcome? outcome = null;
        try
        {
            outcome = ExpressionParser.Evaluate("1 huge * 1 m", new TwoUnits(huge, length));
        }
        catch (OverflowException)
        {
            // Before GREEN the checked DimensionVector addition leaks here.
            // The failed assertion keeps a stable, recognizable RED oracle.
        }
        Assert.IsTrue(outcome is ExpressionEvaluationOutcome.Failure
            { Kind: ExpressionFailureKind.MagnitudeLimit }, AdditionMarker);
    }

    private sealed class OneUnit(UnitDefinition unit) : IExpressionUnitResolver
    {
        public ExpressionUnitResolution Resolve(string token, string culture) =>
            string.Equals(token, unit.Symbol, StringComparison.Ordinal)
                ? new ExpressionUnitResolution(unit, ExpressionUnitResolutionFailure.None, [unit.Id])
                : new ExpressionUnitResolution(null, ExpressionUnitResolutionFailure.UnitNotFound, []);
    }

    private sealed class TwoUnits(UnitDefinition a, UnitDefinition b) : IExpressionUnitResolver
    {
        public ExpressionUnitResolution Resolve(string token, string culture) =>
            string.Equals(token, a.Symbol, StringComparison.Ordinal)
                ? new ExpressionUnitResolution(a, ExpressionUnitResolutionFailure.None, [a.Id])
                : string.Equals(token, b.Symbol, StringComparison.Ordinal)
                    ? new ExpressionUnitResolution(b, ExpressionUnitResolutionFailure.None, [b.Id])
                    : new ExpressionUnitResolution(null, ExpressionUnitResolutionFailure.UnitNotFound, []);
    }
}
