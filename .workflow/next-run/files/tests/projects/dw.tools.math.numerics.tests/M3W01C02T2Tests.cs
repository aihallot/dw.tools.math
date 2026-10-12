using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C02T2Tests
{
    private static IrMatrix ExactMatrix(params ExactRational[] entries) =>
        IrMatrix.Create(1, entries.Length, entries.Select(value => (IrScalar)new IrExactScalar(value)));

    [TestMethod]
    public void DyadicHalfIsExactlyRepresentable()
    {
        var result = RationalBinary64Conversion.TryConvert(new ExactRational(1, 2));
        Assert.AreEqual(RationalBinary64Status.Exact, result.Status);
        Assert.AreEqual(0.5d, result.Value);
        Assert.AreEqual(ExactRational.Zero, result.AbsoluteError);
    }

    [TestMethod]
    public void ZeroIsExactWithoutNegativeZeroInvention()
    {
        var result = RationalBinary64Conversion.TryConvert(default);
        Assert.AreEqual(RationalBinary64Status.Exact, result.Status);
        Assert.AreEqual(0L, BitConverter.DoubleToInt64Bits(result.Value!.Value));
        Assert.AreEqual(ExactRational.Zero, result.AbsoluteError);
    }

    [TestMethod]
    public void OneThirdReportsExactPositiveApproximationError()
    {
        var original = new ExactRational(1, 3);
        var result = RationalBinary64Conversion.TryConvert(original);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.IsTrue(result.IsFiniteResult);
        Assert.IsTrue(result.AbsoluteError!.Value > ExactRational.Zero);
        Assert.AreEqual((ExactBinaryNumber.FromDouble(result.Value!.Value) - original).Abs(),
            result.AbsoluteError.Value);
    }

    [TestMethod]
    public void DecimalOneTenthNeverClaimsExactBinaryRepresentation()
    {
        var result = RationalBinary64Conversion.TryConvert(new ExactRational(1, 10));
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.IsTrue(result.AbsoluteError!.Value > ExactRational.Zero);
    }

    [TestMethod]
    public void MidpointTieRoundsToEvenLowerSignificand()
    {
        var midpoint = new ExactRational((BigInteger.One << 53) + 1, BigInteger.One);
        var result = RationalBinary64Conversion.TryConvert(midpoint);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.AreEqual(9007199254740992d, result.Value);
        Assert.AreEqual(ExactRational.One, result.AbsoluteError);
    }

    [TestMethod]
    public void MidpointTieRoundsToEvenHigherSignificand()
    {
        var midpoint = new ExactRational((BigInteger.One << 53) + 3, BigInteger.One);
        var result = RationalBinary64Conversion.TryConvert(midpoint);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.AreEqual(9007199254740996d, result.Value);
        Assert.AreEqual(ExactRational.One, result.AbsoluteError);
    }

    [TestMethod]
    public void SmallestSubnormalRationalIsExactlyRepresented()
    {
        var original = new ExactRational(1, BigInteger.One << 1074);
        var result = RationalBinary64Conversion.TryConvert(original);
        Assert.AreEqual(RationalBinary64Status.Exact, result.Status);
        Assert.AreEqual(1L, BitConverter.DoubleToInt64Bits(result.Value!.Value));
        Assert.AreEqual(ExactRational.Zero, result.AbsoluteError);
    }

    [TestMethod]
    public void HalfOfSmallestSubnormalIsTypedUnderflowWithoutSilentZero()
    {
        var original = new ExactRational(1, BigInteger.One << 1075);
        var result = RationalBinary64Conversion.TryConvert(original);
        Assert.AreEqual(RationalBinary64Status.Underflow, result.Status);
        Assert.IsFalse(result.IsFiniteResult);
        Assert.IsNull(result.Value);
        Assert.IsNull(result.AbsoluteError);
    }

    [TestMethod]
    public void NonRepresentableOverflowIsTypedRefusal()
    {
        var huge = new ExactRational(BigInteger.One << 1024, 1);
        var result = RationalBinary64Conversion.TryConvert(huge);
        Assert.AreEqual(RationalBinary64Status.Overflow, result.Status);
        Assert.IsNull(result.Value);
        Assert.IsNull(result.AbsoluteError);
    }

    [TestMethod]
    public void OversizedExactOperandIsRefusedBeforeNumericalEvaluation()
    {
        var huge = new ExactRational(BigInteger.One << 4096, 1);
        var result = RationalBinary64Conversion.TryConvert(huge);
        Assert.AreEqual(RationalBinary64Status.BudgetExceeded, result.Status);
        Assert.IsNull(result.Value);
        Assert.AreEqual(4096, RationalBinary64Conversion.MaximumOperandBits);
    }

    [TestMethod]
    public void NegativeMidpointPreservesSignAndTieEven()
    {
        var value = new ExactRational(-((BigInteger.One << 53) + 1), 1);
        var result = RationalBinary64Conversion.TryConvert(value);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.AreEqual(-9007199254740992d, result.Value);
        Assert.AreEqual(ExactRational.One, result.AbsoluteError);
    }

    [TestMethod]
    public void ExactOnlyPolicyRejectsAnyRoundedCellWithoutPartialMatrix()
    {
        var matrix = ExactMatrix(new ExactRational(1, 2), new ExactRational(1, 3));
        var result = ExactMatrixBinary64Conversion.TryConvert(matrix, RationalBinary64Policy.RequireExact);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.IsFalse(result.Accepted);
        Assert.IsNull(result.Matrix);
        Assert.AreEqual(1, result.FirstRejectedIndex);
    }

    [TestMethod]
    public void ExplicitRoundedPolicyReturnsAQuantifiedLossReport()
    {
        var matrix = ExactMatrix(new ExactRational(1, 2), new ExactRational(1, 3));
        var result = ExactMatrixBinary64Conversion.TryConvert(matrix, RationalBinary64Policy.AllowRounded);
        Assert.AreEqual(RationalBinary64Status.Rounded, result.Status);
        Assert.IsTrue(result.Accepted);
        Assert.AreEqual(1, result.RoundedElements);
        Assert.IsTrue(result.MaximumAbsoluteError!.Value > ExactRational.Zero);
        Assert.AreEqual(2, result.Matrix!.Columns);
        Assert.AreEqual(0.5d, result.Matrix.At(0, 0));
    }

    [TestMethod]
    public void ExactOnlyPolicyAcceptsAllDyadicMatrixEntries()
    {
        var matrix = ExactMatrix(new ExactRational(1, 2), new ExactRational(3, 4));
        var result = ExactMatrixBinary64Conversion.TryConvert(matrix, RationalBinary64Policy.RequireExact);
        Assert.AreEqual(RationalBinary64Status.Exact, result.Status);
        Assert.AreEqual(0, result.RoundedElements);
        Assert.AreEqual(ExactRational.Zero, result.MaximumAbsoluteError);
        Assert.IsNull(result.FirstRejectedIndex);
        Assert.AreEqual(0.75d, result.Matrix!.At(0, 1));
    }

    [TestMethod]
    public void ApproximateIrDoesNotPretendToBeExactRationalInput()
    {
        var approximate = IrMatrix.Create(1, 1, [IrFiniteBinary64Scalar.FromDouble(0.25)]);
        Assert.ThrowsExactly<NotSupportedException>(() =>
            ExactMatrixBinary64Conversion.TryConvert(
                approximate, RationalBinary64Policy.AllowRounded));
    }

    [TestMethod]
    public void InvalidConversionPolicyIsRefusedBeforeAnyMaterialization()
    {
        var matrix = ExactMatrix(ExactRational.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactMatrixBinary64Conversion.TryConvert(matrix, (RationalBinary64Policy)42));
    }

    [TestMethod]
    public void ConversionProducesIndependentCopiedProviderArrays()
    {
        var exact = ExactMatrix(ExactRational.One, new ExactRational(1, 2));
        var accepted = ExactMatrixBinary64Conversion.TryConvert(exact, RationalBinary64Policy.RequireExact);
        var matrix = accepted.Matrix!;
        var one = matrix.ToProviderArrayCopy();
        var two = matrix.ToProviderArrayCopy();
        one[0, 0] = 100;
        two[0, 1] = 200;
        Assert.AreEqual(1d, matrix.At(0, 0));
        Assert.AreEqual(0.5d, matrix.At(0, 1));
        Assert.AreEqual(1d, two[0, 0]);
        Assert.AreEqual(0.5d, one[0, 1]);
    }

    [TestMethod]
    public void ProductAssemblyHasNoMathNetOrHostDependency()
    {
        var referencedNames = typeof(FiniteMatrix64).Assembly.GetReferencedAssemblies()
            .Select(name => name.Name).ToArray();
        Assert.IsFalse(referencedNames.Any(name =>
            name is not null && (name.Contains("MathNet", StringComparison.OrdinalIgnoreCase)
                || name.Contains("Aura", StringComparison.OrdinalIgnoreCase)
                || name.Contains("Decision", StringComparison.OrdinalIgnoreCase))));
    }

    [TestMethod]
    public void EquivalentRationalsYieldIdenticalRoundedBitsAndError()
    {
        var a = RationalBinary64Conversion.TryConvert(new ExactRational(2, 6));
        var b = RationalBinary64Conversion.TryConvert(new ExactRational(1, 3));
        Assert.AreEqual(a.Status, b.Status);
        Assert.AreEqual(a.Value, b.Value);
        Assert.AreEqual(a.AbsoluteError, b.AbsoluteError);
    }
}
