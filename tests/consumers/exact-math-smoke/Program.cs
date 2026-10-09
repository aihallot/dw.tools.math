using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;

static void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException("Isolated exact Math consumer failed: " + message);
}

Require(typeof(ExactRational).Assembly.GetName().Name == "dw.quantities", "rational assembly");
Require(typeof(ExpressionParser).Assembly.GetName().Name == "dw.quantities.expression", "parser assembly");
Require(typeof(InheritedStandardUnitCatalog).Assembly.GetName().Name == "dw.quantities.standard", "catalog assembly");

var oneThird = new ExactRational(1, 3);
var oneSixth = new ExactRational(1, 6);
Require(oneThird + oneSixth == new ExactRational(1, 2), "fraction arithmetic");

var unitResolver = new ExplicitProfileExpressionUnitResolver();
var parsed = ExpressionParser.Evaluate("1 in + 1 cm", unitResolver);
Require(parsed is ExpressionEvaluationOutcome.Success parsedValue &&
    parsedValue.Quantity.Value == new ExactRational(177, 5000) &&
    parsedValue.Quantity.Dimension == DimensionVector.LengthDimension, "exact injected parsing");

Require(InheritedStandardUnitCatalog.All.Length == 58, "full inherited unit data");
Require(InheritedStandardUnitCatalog.TryGetById("imperial.inch", out var inch) && inch is not null,
    "canonical inherited inch");
var metre = StandardExpressionUnitResolver.ResolveInherited("si.metre");
Require(PureUnitConverter.Convert(ExactRational.One, inch!, metre) == new ExactRational(127, 5000),
    "exact inch conversion");
var kibibyte = StandardExpressionUnitResolver.ResolveInherited("iec.kibibyte");
var byteUnit = StandardExpressionUnitResolver.ResolveInherited("information.byte");
Require(PureUnitConverter.Convert(ExactRational.One, kibibyte, byteUnit) == new ExactRational(1024, 1),
    "exact IEC information");

var length = DimensionVector.LengthDimension;
var boundaryTolerance = new QuantityTolerance(
    new Quantity(new ExactRational(1, 100), length), ExactRational.Zero);
Require(ExactQuantityComparison.AreEquivalent(
    new Quantity(ExactRational.One, length),
    new Quantity(new ExactRational(101, 100), length), boundaryTolerance),
    "inclusive exact tolerance");
Require(!ExactQuantityComparison.AreEquivalent(
    new Quantity(ExactRational.One, length),
    new Quantity(new ExactRational(101, 100), length),
    new QuantityTolerance(new Quantity(ExactRational.Zero, length), ExactRational.Zero)),
    "zero tolerance boundary");

Require(ExpressionParser.Evaluate("1 cup", unitResolver) is
    ExpressionEvaluationOutcome.Failure { Kind: ExpressionFailureKind.UnitAmbiguous },
    "ambiguous cup rejection");
Require(ExpressionParser.Evaluate("1 cup",
    new ExplicitProfileExpressionUnitResolver(UnitSystem.UsCustomary)) is
    ExpressionEvaluationOutcome.Success, "explicit cup profile");

var delta = ExpressionParser.Evaluate("32 degF - 0 degC", unitResolver);
Require(delta is ExpressionEvaluationOutcome.Success temperature &&
    temperature.Quantity.Temperature == TemperatureSemantics.Interval &&
    temperature.Quantity.Value == ExactRational.Zero, "exact affine temperatures");

var failure = ExpressionParser.Evaluate("1/0", unitResolver);
var diagnostic = ExpressionDiagnostics.Describe(failure);
Require(diagnostic?.Code == "math.expression.divide-by-zero" &&
    diagnostic.Position >= 0 && diagnostic.Message.Length <= 160,
    "bounded neutral diagnostics");

Console.WriteLine("dw.tools.math/exact-consumer/0.2 qualified");
