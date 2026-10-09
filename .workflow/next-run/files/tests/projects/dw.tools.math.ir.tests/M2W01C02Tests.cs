using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C02Tests
{
    [TestMethod]
    public void ExactScalarAlwaysRetainsCanonicalRationalInsteadOfBinary64()
    {
        var exact = new IrExactScalar(new ExactRational(1, 3));
        Assert.AreEqual(new ExactRational(1, 3), exact.Value);
        Assert.AreEqual("1/3", exact.Value.ToFractionString());
        Assert.IsFalse(typeof(IrExactScalar).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Any(method => method.Name is "op_Implicit" or "op_Explicit"));
        Assert.IsFalse(exact is IrFiniteBinary64Scalar);
    }

    [TestMethod]
    public void FiniteBinary64HasDistinctKindAndPreservesReceivedBits()
    {
        var binary = IrFiniteBinary64Scalar.FromDouble(0.1);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(0.1), binary.Ieee754Bits);
        Assert.IsFalse(binary is IrExactScalar);
        Assert.AreNotEqual(binary, (object)new IrExactScalar(new ExactRational(1, 10)));
        var negativeZero = IrFiniteBinary64Scalar.FromDouble(-0.0);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(-0.0), negativeZero.Ieee754Bits);
        Assert.AreNotEqual(IrFiniteBinary64Scalar.FromDouble(0.0), negativeZero);
    }

    [TestMethod]
    public void NonfiniteBinary64CannotEnterAdmittedApproximateScalar()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrFiniteBinary64Scalar.FromDouble(double.NaN));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrFiniteBinary64Scalar.FromDouble(double.PositiveInfinity));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrFiniteBinary64Scalar.FromDouble(double.NegativeInfinity));
    }

    [TestMethod]
    public void BoundAndFreeSymbolsWithIdenticalTextAreDifferentIdentities()
    {
        var free = IrSymbol.Free("x", "x");
        var first = IrSymbol.Bound("scope-a", 0, "x");
        var second = IrSymbol.Bound("scope-b", 0, "x");
        Assert.AreEqual("x", free.DisplayName);
        Assert.AreEqual("x", first.DisplayName);
        Assert.AreNotEqual(free.Identity, first.Identity);
        Assert.AreNotEqual(first.Identity, second.Identity);
        Assert.IsFalse(free.IsBound);
        Assert.IsTrue(first.IsBound);
        Assert.AreEqual(first, IrSymbol.Bound("scope-a", 0, "x"));
    }

    [TestMethod]
    public void RealSquareRootOfSquareNormalizesOnlyToAbsoluteWithoutProof()
    {
        var x = IrSymbol.Free("x", "x", IrScalarDomain.Real);
        var square = IrApply.Create(IrOperation.Square, [x]);
        var sqrt = IrApply.Create(IrOperation.Sqrt, [square]);
        var result = IrRealDomainRules.NormalizeSqrtOfSquare(sqrt, IrAssumptionSet.Empty);
        if (result is not IrApply absolute)
            throw new AssertFailedException("sqrt(x²) must retain abs(x) over the reals.");
        Assert.AreEqual(IrOperation.Abs, absolute.Operation);
        Assert.AreEqual(x, absolute.Arguments[0]);
    }

    [TestMethod]
    public void ProvenNonnegativeSymbolMayEliminateAbsoluteWithoutCapture()
    {
        var x = IrSymbol.Free("x", "x");
        var other = IrSymbol.Bound("nested", 0, "x");
        var condition = new IrRelation(x, IrRelationKind.GreaterOrEqual,
            new IrExactScalar(ExactRational.Zero));
        var assumptions = IrAssumptionSet.Create([condition]);
        var sqrt = IrApply.Create(IrOperation.Sqrt,
            [IrApply.Create(IrOperation.Square, [x])]);
        Assert.AreEqual(x, IrRealDomainRules.NormalizeSqrtOfSquare(sqrt, assumptions));
        var wrongScope = IrAssumptionSet.Create([
            new IrRelation(other, IrRelationKind.GreaterOrEqual,
                new IrExactScalar(ExactRational.Zero))]);
        Assert.IsTrue(IrRealDomainRules.NormalizeSqrtOfSquare(sqrt, wrongScope)
            is IrApply { Operation: IrOperation.Abs });
    }

    [TestMethod]
    public void NonzeroDenominatorExclusionMustSurviveRestrictedExpression()
    {
        var x = IrSymbol.Free("x", "x");
        var numerator = IrApply.Create(IrOperation.Subtract,
            [IrApply.Create(IrOperation.Square, [x]), new IrExactScalar(ExactRational.One)]);
        var denominator = IrApply.Create(IrOperation.Subtract,
            [x, new IrExactScalar(ExactRational.One)]);
        var original = IrApply.Create(IrOperation.Divide, [numerator, denominator]);
        var notOne = new IrRelation(x, IrRelationKind.NotEqual,
            new IrExactScalar(ExactRational.One));
        var restricted = new IrRestrictedExpression(original, IrAssumptionSet.Create([notOne]));
        Assert.AreEqual(IrOperation.Divide, ((IrApply)restricted.Expression).Operation);
        Assert.AreEqual(IrRelationKind.NotEqual, restricted.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(x.Identity, restricted.Assumptions.Conditions[0].Symbol.Identity);
        Assert.AreEqual(ExactRational.One, restricted.Assumptions.Conditions[0].Right.Value);
    }

    [TestMethod]
    public void QuantityLiteralsPreserveSourceUnitIdentityAndEightDimensionalType()
    {
        var exact = new IrExactScalar(ExactRational.One);
        var quantity = new IrQuantityLiteral(exact, "imperial.inch",
            UnitSystem.BritishImperial, DimensionVector.LengthDimension);
        Assert.AreEqual("imperial.inch", quantity.CanonicalUnitId);
        Assert.AreEqual(UnitSystem.BritishImperial, quantity.System);
        Assert.AreEqual(DimensionVector.LengthDimension, quantity.Dimension);
        Assert.AreEqual(exact, quantity.Value);
        var information = new IrQuantityLiteral(exact, "iec.kibibyte",
            UnitSystem.IecBinary, DimensionVector.InformationDimension);
        Assert.AreEqual(1, information.Dimension.Information);
    }

    [TestMethod]
    public void InputMutationsCannotChangeStoredOperationOrAssumptions()
    {
        var x = IrSymbol.Free("x", "x");
        var args = new List<IrNode> { x, new IrExactScalar(ExactRational.One) };
        var expression = IrApply.Create(IrOperation.Add, args);
        args[0] = new IrExactScalar(ExactRational.Zero);
        Assert.AreEqual(x, expression.Arguments[0]);
        var guards = new List<IrRelation> {
            new(x, IrRelationKind.NotEqual, new IrExactScalar(ExactRational.One)) };
        var assumptions = IrAssumptionSet.Create(guards);
        guards.Clear();
        Assert.AreEqual(1, assumptions.Conditions.Length);
    }

    [TestMethod]
    public void InvalidArityAndIdentityAreRejectedBeforeMaterializingIr()
    {
        var x = IrSymbol.Free("x", "x");
        Assert.ThrowsExactly<ArgumentException>(() => IrApply.Create(IrOperation.Sqrt, [x,x]));
        Assert.ThrowsExactly<ArgumentException>(() => IrApply.Create(IrOperation.Add, [x]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrApply.Create((IrOperation)1000, [x]));
        Assert.ThrowsExactly<ArgumentException>(() => IrSymbol.Free(" ", "x"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrSymbol.Bound("s", -1, "x"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrSymbol.Free(new string('x', 129), "x"));
    }
}
