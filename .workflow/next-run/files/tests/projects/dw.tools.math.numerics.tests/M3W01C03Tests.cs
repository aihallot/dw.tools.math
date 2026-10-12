using Dw.Tools.Math.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C03Tests
{
    private static FiniteMatrix64 Matrix() => FiniteMatrix64.Create(2, 2, [2, 1, 1, 3]);
    private static FiniteVector64 Right() => FiniteVector64.Create([4, 7]);

    private static void Near(double actual, double expected, double tolerance = 1e-11)
    {
        Assert.IsTrue(double.IsFinite(actual), "The result must be finite.");
        Assert.IsTrue(Math.Abs(actual - expected) <= tolerance,
            $"Expected {expected:R}, got {actual:R} beyond {tolerance:R}.");
    }

    [TestMethod]
    public void SolvesHandVerifiedTwoByTwoWithoutExplicitInverse()
    {
        var result = BoundedLinearAlgebra64.Solve(Matrix(), Right());
        Assert.AreEqual(LinearAlgebraStatus64.Success, result.Status);
        Assert.IsTrue(result.HasUniqueSolution);
        Near(result.Solution!.At(0), 1);
        Near(result.Solution.At(1), 2);
        Near(result.AbsoluteResidualInfinity!.Value, 0);
        Near(result.RelativeResidualInfinity!.Value, 0);
    }

    [TestMethod]
    public void DirectResidualMatchesIndependentMatrixVectorReconstruction()
    {
        var matrix = Matrix();
        var rhs = Right();
        var solved = BoundedLinearAlgebra64.Solve(matrix, rhs);
        var reconstructed = matrix.Multiply(solved.Solution!);
        for (var i = 0; i < rhs.Length; i++)
            Near(reconstructed.At(i), rhs.At(i), 1e-12);
        Near(solved.AbsoluteResidualInfinity!.Value, 0, 1e-12);
    }

    [TestMethod]
    public void SingularMatrixNeverClaimsAUniqueSolution()
    {
        var rankOne = FiniteMatrix64.Create(2, 2, [1, 2, 2, 4]);
        var solved = BoundedLinearAlgebra64.Solve(rankOne, FiniteVector64.Create([3, 6]));
        Assert.AreEqual(LinearAlgebraStatus64.Singular, solved.Status);
        Assert.IsFalse(solved.HasUniqueSolution);
        Assert.IsNull(solved.Solution);
        Assert.IsNull(solved.RelativeResidualInfinity);
    }

    [TestMethod]
    public void InconsistentSingularSystemAlsoNeverClaimsUniqueSolution()
    {
        var rankOne = FiniteMatrix64.Create(2, 2, [1, 2, 2, 4]);
        var solved = BoundedLinearAlgebra64.Solve(rankOne, FiniteVector64.Create([3, 7]));
        Assert.AreEqual(LinearAlgebraStatus64.Singular, solved.Status);
        Assert.IsNull(solved.Solution);
    }

    [TestMethod]
    public void ZeroMatrixIsSingularRatherThanAZeroSolution()
    {
        var zeros = FiniteMatrix64.Create(2, 2, [0, 0, 0, 0]);
        var solved = BoundedLinearAlgebra64.Solve(zeros, FiniteVector64.Create([0, 0]));
        Assert.AreEqual(LinearAlgebraStatus64.Singular, solved.Status);
        Assert.IsNull(solved.Solution);
    }

    [TestMethod]
    public void LuReconstructsOriginalAfterPermutation()
    {
        var source = Matrix();
        var lu = BoundedLinearAlgebra64.FactorLu(source);
        Assert.AreEqual(LinearAlgebraStatus64.Success, lu.Status);
        Assert.IsTrue(lu.Factored);
        var product = lu.Lower!.Multiply(lu.Upper!);
        Assert.AreEqual(source.Rows, lu.RowPermutation.Length);
        for (var row = 0; row < source.Rows; row++)
            for (var column = 0; column < source.Columns; column++)
                Near(product.At(row, column), source.At(lu.RowPermutation[row], column));
    }

    [TestMethod]
    public void LuPivotSwapUsesIndependentRowPermutation()
    {
        var source = FiniteMatrix64.Create(2, 2, [0, 1, 2, 3]);
        var factors = BoundedLinearAlgebra64.FactorLu(source);
        Assert.AreEqual(LinearAlgebraStatus64.Success, factors.Status);
        Assert.AreEqual(1, factors.RowPermutation[0]);
        Assert.AreEqual(0, factors.RowPermutation[1]);
        var pa = factors.Lower!.Multiply(factors.Upper!);
        for (var row = 0; row < 2; row++)
            for (var col = 0; col < 2; col++)
                Near(pa.At(row, col), source.At(factors.RowPermutation[row], col));
        var solution = BoundedLinearAlgebra64.Solve(
            source, FiniteVector64.Create([1, 5]));
        Assert.IsTrue(solution.HasUniqueSolution);
        Near(solution.Solution!.At(0), 1);
        Near(solution.Solution.At(1), 1);
    }

    [TestMethod]
    public void ThreeByThreeUpperTriangularSolveMatchesHandOracle()
    {
        var upper = FiniteMatrix64.Create(3, 3, [
            3, 1, 0,
            0, 2, 1,
            0, 0, 4]);
        var rhs = FiniteVector64.Create([5, 7, 12]);
        var result = BoundedLinearAlgebra64.Solve(upper, rhs);
        Assert.IsTrue(result.HasUniqueSolution);
        Near(result.Solution!.At(0), 1);
        Near(result.Solution.At(1), 2);
        Near(result.Solution.At(2), 3);
    }

    [TestMethod]
    public void OneByOneSolveIsAdmitted()
    {
        var one = FiniteMatrix64.Create(1, 1, [4]);
        var result = BoundedLinearAlgebra64.Solve(one, FiniteVector64.Create([8]));
        Assert.IsTrue(result.HasUniqueSolution);
        Near(result.Solution!.At(0), 2);
    }

    [TestMethod]
    public void RelativeThresholdClassifiesNearDependentMatrixExplicitly()
    {
        var near = FiniteMatrix64.Create(2, 2, [1, 0, 0, 1e-13]);
        var result = BoundedLinearAlgebra64.Solve(
            near, FiniteVector64.Create([1, 1e-13]));
        Assert.AreEqual(LinearAlgebraStatus64.Singular, result.Status);
        Assert.IsNull(result.Solution);
        var admitted = BoundedLinearAlgebra64.Solve(
            near, FiniteVector64.Create([1, 1e-13]), 1e-15);
        Assert.IsTrue(admitted.HasUniqueSolution);
        Near(admitted.Solution!.At(1), 1);
    }

    [TestMethod]
    public void InvalidOrNonfinitePivotThresholdsAreRejected()
    {
        foreach (var tolerance in new[] { -1d, 0d, 1e-4, double.NaN, double.PositiveInfinity })
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                BoundedLinearAlgebra64.Solve(Matrix(), Right(), tolerance));
    }

    [TestMethod]
    public void NonSquareSystemsRejectAmbiguousUniqueSolve()
    {
        var rectangular = FiniteMatrix64.Create(2, 3, [1, 0, 0, 0, 1, 0]);
        Assert.ThrowsExactly<ArgumentException>(() =>
            BoundedLinearAlgebra64.Solve(
                rectangular, FiniteVector64.Create([1, 2])));
        Assert.ThrowsExactly<ArgumentException>(() =>
            BoundedLinearAlgebra64.FactorLu(rectangular));
    }

    [TestMethod]
    public void DimensionBudgetCannotBeMistakenForSingularity()
    {
        var large = FiniteMatrix64.Create(17, 17,
            Enumerable.Repeat(0d, 17 * 17));
        var factors = BoundedLinearAlgebra64.FactorLu(large);
        Assert.AreEqual(LinearAlgebraStatus64.BudgetExceeded, factors.Status);
        var solve = BoundedLinearAlgebra64.Solve(large,
            FiniteVector64.Create(Enumerable.Repeat(1d, 17)));
        Assert.AreEqual(LinearAlgebraStatus64.BudgetExceeded, solve.Status);
        Assert.IsNull(solve.Solution);
    }

    [TestMethod]
    public void NonfiniteIntermediateIsTypedRefusalNotAFalseSolve()
    {
        var overflow = FiniteMatrix64.Create(2, 2, [
            double.MaxValue, double.MaxValue,
            -double.MaxValue, double.MaxValue]);
        var result = BoundedLinearAlgebra64.Solve(
            overflow, FiniteVector64.Create([1, 1]));
        Assert.AreEqual(LinearAlgebraStatus64.NonFiniteIntermediate, result.Status);
        Assert.IsNull(result.Solution);
    }

    [TestMethod]
    public void QrFullReconstructionMatchesHandOracle()
    {
        var source = Matrix();
        var factor = BoundedLinearAlgebra64.FactorQr(source);
        Assert.IsTrue(factor.Factored);
        var reconstructed = factor.Q!.Multiply(factor.R!);
        for (var row = 0; row < 2; row++)
            for (var column = 0; column < 2; column++)
                Near(reconstructed.At(row, column), source.At(row, column), 1e-11);
    }

    [TestMethod]
    public void QrOrthogonalityUsesInvariantNotArbitraryColumnSigns()
    {
        var factor = BoundedLinearAlgebra64.FactorQr(Matrix());
        var q = factor.Q!;
        for (var i = 0; i < q.Rows; i++)
            for (var j = 0; j < q.Columns; j++)
            {
                var dot = 0d;
                for (var k = 0; k < q.Rows; k++) dot += q.At(k, i) * q.At(k, j);
                Near(dot, i == j ? 1d : 0d, 1e-11);
            }
    }

    [TestMethod]
    public void RectangularThreeByTwoQrReconstructsAllRows()
    {
        var source = FiniteMatrix64.Create(3, 2, [1, 2, 3, 4, 5, 7]);
        var factored = BoundedLinearAlgebra64.FactorQr(source);
        Assert.AreEqual(LinearAlgebraStatus64.Success, factored.Status);
        var reconstructed = factored.Q!.Multiply(factored.R!);
        for (var i = 0; i < source.Rows; i++)
            for (var j = 0; j < source.Columns; j++)
                Near(reconstructed.At(i, j), source.At(i, j), 1e-11);
    }

    [TestMethod]
    public void RankDeficientQrDoesNotClaimCompleteFactorization()
    {
        var rankOne = FiniteMatrix64.Create(2, 2, [1, 2, 2, 4]);
        var factor = BoundedLinearAlgebra64.FactorQr(rankOne);
        Assert.AreEqual(LinearAlgebraStatus64.RankDeficient, factor.Status);
        Assert.IsFalse(factor.Factored);
        Assert.IsNull(factor.Q);
        Assert.IsNull(factor.R);
    }

    [TestMethod]
    public void WideQrIsExplicitlyOutsideTheAdmittedContract()
    {
        var wide = FiniteMatrix64.Create(1, 2, [1, 2]);
        Assert.ThrowsExactly<ArgumentException>(() =>
            BoundedLinearAlgebra64.FactorQr(wide));
    }

    [TestMethod]
    public void TwoByTwoSingularValuesUseAnalyticalInvariants()
    {
        var spectrum = BoundedLinearAlgebra64.SingularValues2x2(Matrix());
        Assert.AreEqual(LinearAlgebraStatus64.Success, spectrum.Status);
        var expectedLargest = (5d + Math.Sqrt(5d)) / 2d;
        var expectedSmallest = (5d - Math.Sqrt(5d)) / 2d;
        Near(spectrum.LargestSingularValue!.Value, expectedLargest, 1e-12);
        Near(spectrum.SmallestSingularValue!.Value, expectedSmallest, 1e-12);
        Near(spectrum.ConditionRatio!.Value, expectedLargest / expectedSmallest, 1e-11);
    }

    [TestMethod]
    public void ZeroSingularValueDoesNotProduceAClaimedFiniteConditionRatio()
    {
        var rankOne = FiniteMatrix64.Create(2, 2, [1, 2, 2, 4]);
        var spectrum = BoundedLinearAlgebra64.SingularValues2x2(rankOne);
        Assert.AreEqual(LinearAlgebraStatus64.Success, spectrum.Status);
        Near(spectrum.SmallestSingularValue!.Value, 0d);
        Assert.IsNull(spectrum.ConditionRatio);
    }

    [TestMethod]
    public void TwoByTwoSpectrumRejectsNonfiniteIntermediate()
    {
        var huge = FiniteMatrix64.Create(2, 2, [
            double.MaxValue, 0, 0, double.MaxValue]);
        var spectrum = BoundedLinearAlgebra64.SingularValues2x2(huge);
        Assert.AreEqual(LinearAlgebraStatus64.NonFiniteIntermediate, spectrum.Status);
        Assert.IsNull(spectrum.LargestSingularValue);
    }

    [TestMethod]
    public void ThreeByThreeFullSvdIsNotYetPubliclyAdmitted()
    {
        var identity = FiniteMatrix64.Create(3, 3, [
            1, 0, 0, 0, 1, 0, 0, 0, 1]);
        Assert.ThrowsExactly<NotSupportedException>(() =>
            BoundedLinearAlgebra64.SingularValues2x2(identity));
    }

    [TestMethod]
    public void ResultMatricesRemainOwnedCopies()
    {
        var a = Matrix();
        var factor = BoundedLinearAlgebra64.FactorQr(a);
        var copy = factor.R!.ToProviderArrayCopy();
        copy[0, 0] += 100;
        var after = factor.R.ToProviderArrayCopy();
        Near(after[0, 0], factor.R.At(0, 0), 1e-12);
        Near(a.At(0, 0), 2);
    }

    [TestMethod]
    public void PublicNumericalAssemblyDoesNotLoadManagedProvider()
    {
        var names = typeof(BoundedLinearAlgebra64).Assembly.GetReferencedAssemblies()
            .Select(x => x.Name ?? string.Empty).ToArray();
        Assert.IsFalse(names.Any(x => x.Contains("MathNet", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Aura", StringComparison.OrdinalIgnoreCase) ||
            x.Contains("Decision", StringComparison.OrdinalIgnoreCase)));
    }
}
