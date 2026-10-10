using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C02Tests
{
    private static FiniteMatrix64 A() => FiniteMatrix64.Create(2, 2, [1, 2, 3, 4]);
    private static FiniteMatrix64 B() => FiniteMatrix64.Create(2, 2, [4, 3, 2, 1]);

    [TestMethod]
    public void TwoByTwoDenseAdditionMatchesIndependentHandOracle()
    {
        var result = A().Add(B());
        Assert.AreEqual(2, result.Rows);
        Assert.AreEqual(2, result.Columns);
        for (var row = 0; row < 2; row++)
            for (var col = 0; col < 2; col++)
                Assert.AreEqual(5d, result.At(row, col));
        Assert.AreEqual(NumericalPrecisionKind.FiniteBinary64, result.Precision);
    }

    [TestMethod]
    public void TwoByTwoDenseProductMatchesIndependentHandOracle()
    {
        var right = FiniteMatrix64.Create(2, 2, [2, 0, 1, 2]);
        var result = A().Multiply(right);
        Assert.AreEqual(4d, result.At(0, 0));
        Assert.AreEqual(4d, result.At(0, 1));
        Assert.AreEqual(10d, result.At(1, 0));
        Assert.AreEqual(8d, result.At(1, 1));
    }

    [TestMethod]
    public void MatrixVectorProductMatchesHandOracle()
    {
        var output = A().Multiply(FiniteVector64.Create([5, 6]));
        Assert.AreEqual(2, output.Length);
        Assert.AreEqual(17d, output.At(0));
        Assert.AreEqual(39d, output.At(1));
    }

    [TestMethod]
    public void VectorDotProductKeepsApproximatePrecisionExplicit()
    {
        var a = FiniteVector64.Create([1, 2, 3]);
        var b = FiniteVector64.Create([3, 4, 5]);
        Assert.AreEqual(26d, a.Dot(b));
        Assert.AreEqual(NumericalPrecisionKind.FiniteBinary64, a.Precision);
    }

    [TestMethod]
    public void AdditionRejectsIncompatibleShapesBeforeArithmetic()
    {
        var wrong = FiniteMatrix64.Create(1, 2, [1, 2]);
        Assert.ThrowsExactly<ArgumentException>(() => A().Add(wrong));
    }

    [TestMethod]
    public void MultiplicationRejectsIncompatibleInnerDimensions()
    {
        var wrong = FiniteMatrix64.Create(3, 2, [1, 2, 3, 4, 5, 6]);
        Assert.ThrowsExactly<ArgumentException>(() => A().Multiply(wrong));
        Assert.ThrowsExactly<ArgumentException>(() =>
            A().Multiply(FiniteVector64.Create([1, 2, 3])));
    }

    [TestMethod]
    public void MatrixShapeBudgetIsCheckedBeforeMaterialization()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            FiniteMatrix64.Create(65, 64, Array.Empty<double>()));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            FiniteMatrix64.Create(0, 1, Array.Empty<double>()));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            FiniteVector64.Create(Enumerable.Repeat(1d, 4097)));
    }

    [TestMethod]
    public void MultiplicationWorkBudgetIsIndependentOfCellQuota()
    {
        var square = FiniteMatrix64.Create(64, 64, Enumerable.Repeat(1d, 4096));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => square.Multiply(square));
        var accepted = FiniteMatrix64.Create(32, 32, Enumerable.Repeat(1d, 1024));
        Assert.AreEqual(32d, accepted.Multiply(accepted).At(0, 0));
    }

    [TestMethod]
    public void NonfiniteBinary64InputsAreRejected()
    {
        Assert.ThrowsExactly<ArithmeticException>(() =>
            FiniteMatrix64.Create(1, 1, [double.NaN]));
        Assert.ThrowsExactly<ArithmeticException>(() =>
            FiniteMatrix64.Create(1, 1, [double.PositiveInfinity]));
        Assert.ThrowsExactly<ArithmeticException>(() =>
            FiniteVector64.Create([double.NegativeInfinity]));
    }

    [TestMethod]
    public void NonfiniteArithmeticDoesNotBecomeAValidResult()
    {
        var huge = FiniteMatrix64.Create(1, 1, [double.MaxValue]);
        Assert.ThrowsExactly<ArithmeticException>(() => huge.Add(huge));
        var two = FiniteMatrix64.Create(1, 1, [2]);
        Assert.ThrowsExactly<ArithmeticException>(() => huge.Multiply(two));
    }

    [TestMethod]
    public void SparseCoordinatesAreSortedAndConvertExplicitlyToDense()
    {
        var sparse = SparseMatrix64.Create(2, 2, [
            new SparseEntry64(1, 1, 4),
            new SparseEntry64(0, 1, 2),
            new SparseEntry64(0, 0, 1)]);
        Assert.AreEqual(3, sparse.NonZeroCount);
        Assert.AreEqual(0, sparse.Entries[0].Row);
        Assert.AreEqual(0, sparse.Entries[0].Column);
        var dense = sparse.ToDense();
        Assert.AreEqual(1d, dense.At(0, 0));
        Assert.AreEqual(2d, dense.At(0, 1));
        Assert.AreEqual(0d, dense.At(1, 0));
        Assert.AreEqual(4d, dense.At(1, 1));
    }

    [TestMethod]
    public void SparseDuplicateAndZeroCoordinatesAreRefused()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            SparseMatrix64.Create(2, 2, [
                new SparseEntry64(0, 0, 1), new SparseEntry64(0, 0, 2)]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            SparseMatrix64.Create(2, 2, [new SparseEntry64(1, 1, 0)]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            SparseMatrix64.Create(2, 2, [new SparseEntry64(2, 1, 1)]));
    }

    [TestMethod]
    public void ApproximateIrRoundTripPreservesValuesAndSignedZero()
    {
        var source = FiniteMatrix64.Create(1, 2, [-0.0, 0.25]);
        var ir = NumericalMatrixTransport.ToApproximateIr(source);
        Assert.IsInstanceOfType<IrFiniteBinary64Scalar>(ir.At(0, 0));
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(-0.0),
            ((IrFiniteBinary64Scalar)ir.At(0, 0)).Ieee754Bits);
        var roundtrip = NumericalMatrixTransport.FromApproximateIr(ir);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(-0.0),
            BitConverter.DoubleToInt64Bits(roundtrip.At(0, 0)));
        Assert.AreEqual(0.25d, roundtrip.At(0, 1));
    }

    [TestMethod]
    public void ExactRationalIrIsNeverSilentlyConvertedToDouble()
    {
        var ir = IrMatrix.Create(1, 1, [
            new IrExactScalar(new ExactRational(1, 3))]);
        Assert.ThrowsExactly<NotSupportedException>(() =>
            NumericalMatrixTransport.FromApproximateIr(ir));
    }

    [TestMethod]
    public void ProviderArrayConversionsCopyRatherThanAlias()
    {
        var input = new double[,] { { 1, 2 }, { 3, 4 } };
        var matrix = FiniteMatrix64.FromProviderArrayCopy(input);
        input[0, 0] = 99;
        Assert.AreEqual(1d, matrix.At(0, 0));
        var copy = matrix.ToProviderArrayCopy();
        copy[1, 1] = 44;
        Assert.AreEqual(4d, matrix.At(1, 1));
    }

    [TestMethod]
    public void MissingAndExtraMatrixEntriesAreRefused()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            FiniteMatrix64.Create(2, 2, [1, 2, 3]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            FiniteMatrix64.Create(2, 2, [1, 2, 3, 4, 5]));
    }

    [TestMethod]
    public void SparseEmptyMatrixHasOnlyZeroValues()
    {
        var sparse = SparseMatrix64.Create(2, 3, Array.Empty<SparseEntry64>());
        Assert.AreEqual(0, sparse.NonZeroCount);
        var dense = sparse.ToDense();
        Assert.AreEqual(2, dense.Rows);
        Assert.AreEqual(3, dense.Columns);
        Assert.AreEqual(0d, dense.At(1, 2));
    }
}
