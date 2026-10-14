using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C02T1Tests
{
    private static IrSymbol X() => IrSymbol.Free("x", "x", IrScalarDomain.Real);
    private static IrExactScalar Exact(ExactRational value) => new(value);
    private static IrExactScalar Number(long value) => Exact(new ExactRational(value, 1));

    private static IrNode Ratio(IrSymbol x, ExactRational c, bool power = false)
    {
        var squared = power
            ? IrApply.Create(IrOperation.Power, [x, Number(2)])
            : IrApply.Create(IrOperation.Square, [x]);
        var numerator = IrApply.Create(IrOperation.Subtract, [squared, Exact(c * c)]);
        var denominator = IrApply.Create(IrOperation.Subtract, [x, Exact(c)]);
        return IrApply.Create(IrOperation.Divide, [numerator, denominator]);
    }

    private static SymbolicCancellationResult Cancel(IrNode input) =>
        SymbolicRationalCancellationBridge.TryCancel(input);

    private static IrRestrictedExpression Admitted(IrNode input)
    {
        var result = Cancel(input);
        if (!result.IsAdmitted || result.Conditional is not IrRestrictedExpression typed)
            throw new AssertFailedException("The exact quotient must yield a typed restricted IR.");
        return typed;
    }

    private static SymbolicPointResult ValueAt(IrNode input, IrSymbol x, ExactRational point)
    {
        var simplified = Cancel(input);
        return SymbolicRationalCancellationBridge.EvaluateExactAt(simplified, x, point);
    }

    [TestMethod]
    public void CanonicalCancellationHasExactlyTheExcludedOriginalPole()
    {
        var x = X();
        var typed = Admitted(Ratio(x, ExactRational.One));
        Assert.AreEqual(1, typed.Assumptions.Conditions.Length);
        var condition = typed.Assumptions.Conditions[0];
        Assert.AreEqual(IrRelationKind.NotEqual, condition.Kind);
        Assert.AreEqual(x.Identity, condition.Symbol.Identity);
        Assert.AreEqual(ExactRational.One, condition.Right.Value);
        if (typed.Expression is not IrApply { Operation: IrOperation.Add } sum)
            throw new AssertFailedException("The narrowed result must be x+1.");
        Assert.AreEqual(x, sum.Arguments[0]);
        Assert.AreEqual(Number(1), sum.Arguments[1]);
    }

    [TestMethod]
    public void PoleAtOneIsExcludedWithoutAnyNumericResult()
    {
        var x = X();
        var result = ValueAt(Ratio(x, ExactRational.One), x, ExactRational.One);
        Assert.AreEqual(SymbolicPointStatus.Excluded, result.Status);
        Assert.IsFalse(result.HasExactValue);
        Assert.IsFalse(result.Value.HasValue);
    }

    [TestMethod]
    public void PointZeroGivesExactlyOne()
    {
        var x = X();
        var result = ValueAt(Ratio(x, ExactRational.One), x, ExactRational.Zero);
        Assert.AreEqual(SymbolicPointStatus.ExactValue, result.Status);
        Assert.AreEqual(ExactRational.One, result.Value);
    }

    [TestMethod]
    public void PointTwoGivesExactlyThree()
    {
        var x = X();
        var result = ValueAt(Ratio(x, ExactRational.One), x, Number(2).Value);
        Assert.AreEqual(SymbolicPointStatus.ExactValue, result.Status);
        Assert.AreEqual(new ExactRational(3, 1), result.Value);
    }

    [TestMethod]
    public void NegativeValidPointMatchesOriginalRationalValue()
    {
        var x = X();
        var actual = ValueAt(Ratio(x, ExactRational.One), x, new ExactRational(-2, 1));
        Assert.AreEqual(new ExactRational(-1, 1), actual.Value);
        // Independent original quotient: ((-2)^2 - 1)/(-2-1) = -1.
        Assert.AreEqual((new ExactRational(4, 1) - ExactRational.One) /
            new ExactRational(-3, 1), actual.Value);
    }

    [TestMethod]
    public void DifferentPoleAtTwoProducesXPlusTwoAndExcludesTwo()
    {
        var x = X();
        var expr = Ratio(x, new ExactRational(2, 1));
        Assert.AreEqual(new ExactRational(5, 1),
            ValueAt(expr, x, new ExactRational(3, 1)).Value);
        Assert.AreEqual(SymbolicPointStatus.Excluded,
            ValueAt(expr, x, new ExactRational(2, 1)).Status);
    }

    [TestMethod]
    public void NegativeRationalPoleIsNotImplicitlyRounded()
    {
        var x = X();
        var expr = Ratio(x, new ExactRational(-3, 1));
        Assert.AreEqual(new ExactRational(-2, 1),
            ValueAt(expr, x, ExactRational.One).Value);
        Assert.AreEqual(SymbolicPointStatus.Excluded,
            ValueAt(expr, x, new ExactRational(-3, 1)).Status);
    }

    [TestMethod]
    public void FractionalPolePreservesExactNumeratorAndDenominator()
    {
        var x = X();
        var half = new ExactRational(1, 2);
        var expr = Ratio(x, half);
        Assert.AreEqual(half, ValueAt(expr, x, ExactRational.Zero).Value);
        Assert.AreEqual(SymbolicPointStatus.Excluded, ValueAt(expr, x, half).Status);
        Assert.AreEqual(new ExactRational(3, 2),
            ValueAt(expr, x, ExactRational.One).Value);
    }

    [TestMethod]
    public void RemovableZeroPoleRemainsExcluded()
    {
        var x = X();
        var expr = Ratio(x, ExactRational.Zero);
        Assert.AreEqual(SymbolicPointStatus.Excluded, ValueAt(expr, x, ExactRational.Zero).Status);
        Assert.AreEqual(new ExactRational(3, 1),
            ValueAt(expr, x, new ExactRational(3, 1)).Value);
    }

    [TestMethod]
    public void ExplicitPowerTwoAndSquareUseTheSameProvedIdentity()
    {
        var x = X();
        var c = new ExactRational(2, 1);
        var square = Admitted(Ratio(x, c));
        var power = Admitted(Ratio(x, c, power: true));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(square),
            IrCanonicalJsonCodec.Encode(power));
    }

    [TestMethod]
    public void ExistingNonnegativeRestrictionIsPreservedAndEnforced()
    {
        var x = X();
        var restriction = IrAssumptionSet.Create([
            new IrRelation(x, IrRelationKind.GreaterOrEqual, Number(0))]);
        var original = new IrRestrictedExpression(Ratio(x, ExactRational.One), restriction);
        var result = Cancel(original);
        Assert.AreEqual(SymbolicCancellationStatus.Admitted, result.Status);
        Assert.AreEqual(2, result.Conditional!.Assumptions.Conditions.Length);
        Assert.AreEqual(SymbolicPointStatus.Excluded,
            SymbolicRationalCancellationBridge.EvaluateExactAt(
                result, x, new ExactRational(-1, 1)).Status);
        Assert.AreEqual(new ExactRational(3, 1),
            SymbolicRationalCancellationBridge.EvaluateExactAt(
                result, x, new ExactRational(2, 1)).Value);
    }

    [TestMethod]
    public void AlreadyPresentPoleExclusionIsNotDuplicated()
    {
        var x = X();
        var input = new IrRestrictedExpression(Ratio(x, ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(x, IrRelationKind.NotEqual, Number(1))]));
        var result = Admitted(input);
        Assert.AreEqual(1, result.Assumptions.Conditions.Length);
    }

    [TestMethod]
    public void EqualityAtOriginalPoleIsContradictoryNotSuccess()
    {
        var x = X();
        var input = new IrRestrictedExpression(Ratio(x, ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(x, IrRelationKind.Equal, Number(1))]));
        var result = Cancel(input);
        Assert.AreEqual(SymbolicCancellationStatus.ContradictoryAssumptions, result.Status);
        Assert.IsNull(result.Conditional);
    }

    [TestMethod]
    public void InconsistentEqualityAndInequalityMustNotBeNormalized()
    {
        var x = X();
        var input = new IrRestrictedExpression(Ratio(x, ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(x, IrRelationKind.Equal, Number(2)),
                new IrRelation(x, IrRelationKind.NotEqual, Number(2))]));
        Assert.AreEqual(SymbolicCancellationStatus.ContradictoryAssumptions,
            Cancel(input).Status);
    }

    [TestMethod]
    public void UnreferencedScopeMustNotLeakIntoSimplifiedResult()
    {
        var x = X();
        var y = IrSymbol.Free("y", "x");
        var guarded = new IrRestrictedExpression(Ratio(x, ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(y, IrRelationKind.NotEqual, Number(1))]));
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported, Cancel(guarded).Status);
    }

    [TestMethod]
    public void BoundVariableIsNotSilentlyTreatedAsFree()
    {
        var bound = IrSymbol.Bound("scope-a", 0, "x");
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported,
            Cancel(Ratio(bound, ExactRational.One)).Status);
    }

    [TestMethod]
    public void IntegerDomainIsNotSilentlyWidenedIntoReal()
    {
        var integer = IrSymbol.Free("x", "x", IrScalarDomain.Integer);
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported,
            Cancel(Ratio(integer, ExactRational.One)).Status);
    }

    [TestMethod]
    public void IncorrectSquareConstantCannotBecomeAFalseAlgebraicIdentity()
    {
        var x = X();
        var numerator = IrApply.Create(IrOperation.Subtract,
            [IrApply.Create(IrOperation.Square, [x]), Number(2)]);
        var denominator = IrApply.Create(IrOperation.Subtract, [x, Number(1)]);
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported,
            Cancel(IrApply.Create(IrOperation.Divide, [numerator, denominator])).Status);
    }

    [TestMethod]
    public void ArbitrarySquareRootIsOutsideTheQualifiedPattern()
    {
        var x = X();
        var expression = IrApply.Create(IrOperation.Sqrt,
            [IrApply.Create(IrOperation.Square, [x])]);
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported, Cancel(expression).Status);
    }

    [TestMethod]
    public void VeryLargeExactCoefficientsRefuseUnboundedQuadraticWork()
    {
        var x = X();
        var huge = new ExactRational(BigInteger.One << 129, BigInteger.One);
        Assert.AreEqual(SymbolicCancellationStatus.BudgetExceeded,
            Cancel(Ratio(x, huge)).Status);
    }

    [TestMethod]
    public void EncodedRestrictedIrRoundTripKeepsIdentityAndPole()
    {
        var x = X();
        var admitted = Admitted(Ratio(x, ExactRational.One));
        var decoded = IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(admitted));
        if(decoded is not IrRestrictedExpression restricted)
            throw new AssertFailedException("Canonical IR must retain a typed restriction.");
        Assert.AreEqual(1, restricted.Assumptions.Conditions.Length);
        Assert.AreEqual(x.Identity, restricted.Assumptions.Conditions[0].Symbol.Identity);
        Assert.AreEqual(IrRelationKind.NotEqual, restricted.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(ExactRational.One, restricted.Assumptions.Conditions[0].Right.Value);
    }

    [TestMethod]
    public void EvaluatingWithADifferentSymbolIdentityIsUnsupported()
    {
        var x = X();
        var result = Cancel(Ratio(x, ExactRational.One));
        var other = IrSymbol.Free("different", "x");
        var evaluated = SymbolicRationalCancellationBridge.EvaluateExactAt(
            result, other, new ExactRational(2, 1));
        Assert.AreEqual(SymbolicPointStatus.Unsupported, evaluated.Status);
        Assert.IsFalse(evaluated.Value.HasValue);
    }

    [TestMethod]
    public void UnknownMatrixAndApproximateScalarAreNotInterpretedAsProviderSyntax()
    {
        var matrix = IrMatrix.Create(1, 1, [Number(1)]);
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported, Cancel(matrix).Status);
        Assert.AreEqual(SymbolicCancellationStatus.Unsupported,
            Cancel(IrFiniteBinary64Scalar.FromDouble(0.5)).Status);
    }

    [TestMethod]
    public void SymbolicProductAssemblyHasNoCasOrAuraDependency()
    {
        var dependencies = typeof(SymbolicRationalCancellationBridge).Assembly
            .GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
        Assert.IsFalse(dependencies.Any(x =>
            x.Contains("Angouri", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("MathNet", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Decision", StringComparison.OrdinalIgnoreCase)),
            "Mathematical IR contracts cannot depend on optional CAS providers.");
    }

    [TestMethod]
    public void ANullRootIsAlwaysRejectedRatherThanAcceptedAsAFormula()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            SymbolicRationalCancellationBridge.TryCancel(null!));
    }
}
