using System.Numerics;
using dw.quantities;
using dw.quantities.expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C02Tests
{
    [TestMethod]
    public void ExactFractionsAddAndOperatorsRespectPrecedence()
    {
        Assert.AreEqual(new ExactRational(1, 2), Success("1/3+1/6").Value);
        Assert.AreEqual(new ExactRational(14, 1), Success("2+3*4").Value);
        Assert.AreEqual(new ExactRational(-5, 1), Success("1-2*3").Value);
        Assert.AreEqual(new ExactRational(9, 4), Success("root(81/16,2)").Value);
    }

    [TestMethod]
    public void RationalRootsAndPowersRemainExactWithoutFloatingPoint()
    {
        Assert.AreEqual(new ExactRational(2, 1), Success("root(65536,16)").Value);
        Assert.AreEqual(new ExactRational(1, 8), Success("2^-3").Value);
        Assert.AreEqual(new ExactRational(65536, 1), Success("2^16").Value);
        Failure("root(2,2)", ExpressionFailureKind.Domain);
        Failure("root(-4,2)", ExpressionFailureKind.Domain);
        Failure("root(65536,17)", ExpressionFailureKind.Domain);
        Failure("2^17", ExpressionFailureKind.Domain);
    }

    [TestMethod]
    public void SelectionFunctionsRemainExact()
    {
        Assert.AreEqual(new ExactRational(3, 2), Success("abs(-3/2)").Value);
        Assert.AreEqual(new ExactRational(-2, 1), Success("min(3,-2,1)").Value);
        Assert.AreEqual(new ExactRational(3, 1), Success("max(-2,3,1)").Value);
    }

    [TestMethod]
    public void ResolverIsInjectedAndInformationDimensionsRetainPowers()
    {
        var resolver = new StubResolver(
            new UnitDefinition("si.metre", "m", "metre", UnitSystem.Si,
                DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
                UnitTransformKind.Linear),
            new UnitDefinition("information.byte", "B", "byte", UnitSystem.Si,
                DimensionVector.InformationDimension, ExactRational.One, ExactRational.Zero,
                UnitTransformKind.Linear));
        var length = Success("1 m+2 m", resolver);
        Assert.AreEqual(new ExactRational(3, 1), length.Value);
        Assert.AreEqual(DimensionVector.LengthDimension, length.Dimension);
        var info = Success("(2 B)^2", resolver);
        Assert.AreEqual(new ExactRational(4, 1), info.Value);
        Assert.AreEqual(2, info.Dimension.Information);
        Failure("1 unknown", ExpressionFailureKind.UnitNotFound, resolver);
        Failure("1 m + 1 B", ExpressionFailureKind.IncompatibleDimensions, resolver);
    }

    [TestMethod]
    public void UnsupportedCultureAndAmbiguityAreTypedFailures()
    {
        Failure("1 cup", ExpressionFailureKind.UnitAmbiguous,
            new FailureResolver(ExpressionUnitResolutionFailure.AmbiguousUnit));
        Failure("1 cup", ExpressionFailureKind.UnsupportedCulture,
            new FailureResolver(ExpressionUnitResolutionFailure.UnsupportedCulture));
        Failure("1 cup", ExpressionFailureKind.UnitNotFound,
            new FailureResolver(ExpressionUnitResolutionFailure.UnitNotFound));
    }

    [TestMethod]
    public void AbsoluteTemperatureAlgebraRejectsInvalidExpressions()
    {
        var resolver = new StubResolver(
            new UnitDefinition("si.celsius", "degC", "Celsius", UnitSystem.Si,
                DimensionVector.TemperatureDimension, ExactRational.One,
                new ExactRational(27315, 100), UnitTransformKind.AbsoluteTemperature),
            new UnitDefinition("us.fahrenheit", "degF", "Fahrenheit", UnitSystem.UsCustomary,
                DimensionVector.TemperatureDimension, new ExactRational(5, 9),
                new ExactRational(45967, 180), UnitTransformKind.AbsoluteTemperature));
        var delta = Success("32 degF - 0 degC", resolver);
        Assert.AreEqual(ExactRational.Zero, delta.Value);
        Assert.AreEqual(TemperatureSemantics.Interval, delta.Temperature);
        Failure("0 degC + 32 degF", ExpressionFailureKind.TemperatureAlgebra, resolver);
        Failure("0 degC * 2", ExpressionFailureKind.TemperatureAlgebra, resolver);
    }

    [TestMethod]
    public void CharacterCountLimitAccepts4096AndRejects4097()
    {
        var accepted = "1" + new string(' ', 4095);
        var rejected = "1" + new string(' ', 4096);
        Assert.AreEqual(ExactRational.One, Success(accepted).Value);
        Failure(rejected, ExpressionFailureKind.ComplexityLimit);
    }

    [TestMethod]
    public void TokenCountLimitAccepts255AndRejects257()
    {
        var accepted = "1" + string.Concat(Enumerable.Repeat("+1", 127));
        var rejected = "1" + string.Concat(Enumerable.Repeat("+1", 128));
        Assert.AreEqual(new ExactRational(128,1), Success(accepted).Value);
        Failure(rejected, ExpressionFailureKind.ComplexityLimit);
    }

    [TestMethod]
    public void NestingDepthLimitAccepts32PrimariesAndRejects33()
    {
        var accepted = new string('(', 31) + "1" + new string(')', 31);
        var rejected = new string('(', 32) + "1" + new string(')', 32);
        Assert.AreEqual(ExactRational.One, Success(accepted).Value);
        Failure(rejected, ExpressionFailureKind.ComplexityLimit);
    }

    [TestMethod]
    public void DecimalMagnitudeLimitAccepts256DigitsAndRejects257()
    {
        var accepted = new string('9', 256);
        var rejected = new string('9', 257);
        Assert.AreEqual(new ExactRational(BigInteger.Parse(accepted), BigInteger.One), Success(accepted).Value);
        Failure(rejected, ExpressionFailureKind.MagnitudeLimit);
        Failure(new string('9', 20) + "^16", ExpressionFailureKind.MagnitudeLimit);
    }

    [TestMethod]
    public void InvalidSyntaxAndFreeVariablesNeverExecuteCSharp()
    {
        Failure("x", ExpressionFailureKind.Syntax);
        Failure("System.IO.File.ReadAllText(1)", ExpressionFailureKind.Syntax);
        Failure("1; 2", ExpressionFailureKind.Syntax);
        Failure("1..2", ExpressionFailureKind.Syntax);
        Failure("1/0", ExpressionFailureKind.DivideByZero);
    }

    private static EvaluatedQuantity Success(string expression, IExpressionUnitResolver? resolver = null)
    {
        var outcome = resolver is null ? ExpressionParser.Evaluate(expression) :
            ExpressionParser.Evaluate(expression, resolver);
        if (outcome is ExpressionEvaluationOutcome.Success result)
            return result.Quantity;
        throw new AssertFailedException("Expected exact evaluation success for " + expression +
            ", observed " + outcome);
    }

    private static void Failure(string expression, ExpressionFailureKind expected,
        IExpressionUnitResolver? resolver = null)
    {
        var outcome = resolver is null ? ExpressionParser.Evaluate(expression) :
            ExpressionParser.Evaluate(expression, resolver);
        if (outcome is not ExpressionEvaluationOutcome.Failure failure)
            throw new AssertFailedException("Expected failure " + expected + " for " + expression +
                ", observed " + outcome);
        Assert.AreEqual(expected, failure.Kind, expression);
    }

    private sealed class StubResolver(params UnitDefinition[] units) : IExpressionUnitResolver
    {
        public ExpressionUnitResolution Resolve(string token, string culture)
        {
            var candidates = units.Where(unit =>
                string.Equals(unit.Symbol, token, StringComparison.Ordinal)).ToArray();
            return candidates.Length == 1
                ? new ExpressionUnitResolution(candidates[0], ExpressionUnitResolutionFailure.None,
                    [candidates[0].Id])
                : new ExpressionUnitResolution(null, ExpressionUnitResolutionFailure.UnitNotFound, []);
        }
    }

    private sealed class FailureResolver(ExpressionUnitResolutionFailure failure) : IExpressionUnitResolver
    {
        public ExpressionUnitResolution Resolve(string token, string culture) =>
            new(null, failure, []);
    }
}
