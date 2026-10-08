using System.Globalization;
using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C02BoundaryTests
{
    [TestMethod]
    public void InheritedExactUnitsResolveThroughInjectedProfileBridge()
    {
        var resolver = new ExplicitProfileExpressionUnitResolver();
        var outcome = ExpressionParser.Evaluate("1 in + 1 cm", resolver);
        var success = RequireSuccess(outcome);
        Assert.AreEqual(new ExactRational(177, 5000), success.Value);
        Assert.AreEqual(DimensionVector.LengthDimension, success.Dimension);
        var memory = RequireSuccess(ExpressionParser.Evaluate("2 KiB", resolver));
        Assert.AreEqual(new ExactRational(2048, 1), memory.Value);
        Assert.AreEqual(DimensionVector.InformationDimension, memory.Dimension);
    }

    [TestMethod]
    public void AmbiguousInheritedCupsNeedExplicitProfileAndExposeCandidates()
    {
        var noProfile = new ExplicitProfileExpressionUnitResolver();
        var failure = RequireFailure(ExpressionParser.Evaluate("1 cup", noProfile),
            ExpressionFailureKind.UnitAmbiguous);
        var candidates = noProfile.Resolve("cup", "en-US");
        Assert.AreEqual(ExpressionUnitResolutionFailure.AmbiguousUnit, candidates.Failure);
        CollectionAssert.AreEquivalent(new[] { "au.cup", "metric.cup", "us.cup" },
            candidates.CandidateIds.ToArray());
        Assert.AreEqual("us.cup",
            new ExplicitProfileExpressionUnitResolver(UnitSystem.UsCustomary)
                .Resolve("cup", "fr-BE").Unit?.Id);
        var us = RequireSuccess(ExpressionParser.Evaluate("1 cup",
            new ExplicitProfileExpressionUnitResolver(UnitSystem.UsCustomary)));
        Assert.AreEqual(new ExactRational(2365882365, 10000000000000L), us.Value);
        Assert.AreEqual(DimensionVector.LengthDimension + DimensionVector.LengthDimension +
            DimensionVector.LengthDimension, us.Dimension);
    }

    [TestMethod]
    public void ParserCultureArgumentCannotChangeUnitProfile()
    {
        var prior = CultureInfo.CurrentCulture;
        var priorUi = CultureInfo.CurrentUICulture;
        try
        {
            foreach (var culture in new[] { "en-US", "en-GB", "fr-BE", "tr-TR" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                var resolver = new ExplicitProfileExpressionUnitResolver();
                RequireFailure(ExpressionParser.Evaluate("1 pint", resolver, culture),
                    ExpressionFailureKind.UnitAmbiguous);
                var withProfile = RequireSuccess(ExpressionParser.Evaluate("1 pint",
                    new ExplicitProfileExpressionUnitResolver(UnitSystem.BritishImperial), culture));
                Assert.AreEqual(new ExactRational(56826125, 100000000000L), withProfile.Value);
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = prior;
            CultureInfo.CurrentUICulture = priorUi;
        }
    }

    [TestMethod]
    public void AdmittedAndInheritedCataloguesRemainExplicitChoices()
    {
        var admitted = new ExplicitProfileExpressionUnitResolver(
            UnitSystem.AustralianCulinary, useInheritedCatalog:false);
        var inherited = new ExplicitProfileExpressionUnitResolver(
            UnitSystem.AustralianCulinary, useInheritedCatalog:true);
        Assert.AreEqual("au-beer-pint-570ml", admitted.Resolve("pint", "").Unit?.Id);
        Assert.AreEqual(ExpressionUnitResolutionFailure.UnitNotFound,
            inherited.Resolve("pint", "").Failure);
    }

    [TestMethod]
    public void UnsupportedTokensCurrenciesAndFreeVariablesAreRejected()
    {
        var resolver = new ExplicitProfileExpressionUnitResolver();
        RequireFailure(ExpressionParser.Evaluate("1 USD", resolver), ExpressionFailureKind.UnitNotFound);
        RequireFailure(ExpressionParser.Evaluate("1 dB", resolver), ExpressionFailureKind.UnitNotFound);
        RequireFailure(ExpressionParser.Evaluate("x+1", resolver), ExpressionFailureKind.Syntax);
        RequireFailure(ExpressionParser.Evaluate("System.IO.File.ReadAllText(1)", resolver),
            ExpressionFailureKind.Syntax);
        var token = new string('x', StandardExpressionUnitResolver.MaximumTokenLength + 1);
        Assert.AreEqual(ExpressionUnitResolutionFailure.UnitNotFound,
            resolver.Resolve(token, "en-US").Failure);
    }

    [TestMethod]
    public void ParserNeverReturnsWrappedDimensionsOrLeaksOverflowExceptions()
    {
        var huge = new UnitDefinition("huge", "huge", "dimension-test", UnitSystem.Si,
            new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0),
            ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
        var metre = new UnitDefinition("metre", "m", "metre", UnitSystem.Si,
            DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
            UnitTransformKind.Linear);
        var resolver = new PairResolver(huge, metre);
        RequireFailure(ExpressionParser.Evaluate("(2 huge)^2", resolver),
            ExpressionFailureKind.MagnitudeLimit);
        RequireFailure(ExpressionParser.Evaluate("1 huge * 1 m", resolver),
            ExpressionFailureKind.MagnitudeLimit);
        RequireFailure(ExpressionParser.Evaluate("(1 huge)^-2", resolver),
            ExpressionFailureKind.MagnitudeLimit);
    }

    [TestMethod]
    public void AdversarialInputsRetainTypedBoundsAndExactResults()
    {
        var resolver = new ExplicitProfileExpressionUnitResolver();
        RequireFailure(ExpressionParser.Evaluate("2^17", resolver), ExpressionFailureKind.Domain);
        RequireFailure(ExpressionParser.Evaluate("root(2,2)", resolver), ExpressionFailureKind.Domain);
        RequireFailure(ExpressionParser.Evaluate(new string('9',257), resolver),
            ExpressionFailureKind.MagnitudeLimit);
        RequireFailure(ExpressionParser.Evaluate("1"+new string(' ',4096), resolver),
            ExpressionFailureKind.ComplexityLimit);
        RequireFailure(ExpressionParser.Evaluate("1"+string.Concat(Enumerable.Repeat("+1",128)), resolver),
            ExpressionFailureKind.ComplexityLimit);
        var exact = RequireSuccess(ExpressionParser.Evaluate("1/3 + 1/6", resolver));
        Assert.AreEqual(new ExactRational(1,2), exact.Value);
    }

    [TestMethod]
    public void OriginalTemperatureUnitsRemainExactAndAffine()
    {
        var resolver = new ExplicitProfileExpressionUnitResolver();
        var delta = RequireSuccess(ExpressionParser.Evaluate("32 degF - 0 degC", resolver));
        Assert.AreEqual(ExactRational.Zero, delta.Value);
        Assert.AreEqual(TemperatureSemantics.Interval, delta.Temperature);
        RequireFailure(ExpressionParser.Evaluate("32 degF + 0 degC", resolver),
            ExpressionFailureKind.TemperatureAlgebra);
    }

    private static EvaluatedQuantity RequireSuccess(ExpressionEvaluationOutcome outcome) =>
        outcome is ExpressionEvaluationOutcome.Success success ? success.Quantity
        : throw new AssertFailedException("Expected Success, got: " + outcome);

    private static ExpressionEvaluationOutcome.Failure RequireFailure(
        ExpressionEvaluationOutcome outcome, ExpressionFailureKind kind)
    {
        if (outcome is not ExpressionEvaluationOutcome.Failure failure)
            throw new AssertFailedException("Expected " + kind + ", got: " + outcome);
        Assert.AreEqual(kind, failure.Kind);
        return failure;
    }

    private sealed class PairResolver(UnitDefinition first, UnitDefinition second) : IExpressionUnitResolver
    {
        public ExpressionUnitResolution Resolve(string token, string culture)
        {
            UnitDefinition? unit = string.Equals(token, first.Symbol, StringComparison.Ordinal)
                ? first : string.Equals(token, second.Symbol, StringComparison.Ordinal) ? second : null;
            return unit is not null
                ? new ExpressionUnitResolution(unit, ExpressionUnitResolutionFailure.None, [unit.Id])
                : new ExpressionUnitResolution(null, ExpressionUnitResolutionFailure.UnitNotFound, []);
        }
    }
}
