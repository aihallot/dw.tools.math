using Dw.Tools.Math.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C03T2Tests
{
    private static FiniteMatrix64 Matrix(params double[] values) =>
        FiniteMatrix64.Create(2,2,values);

    private static void Near(double observed,double expected,double tolerance=1e-11)
    {
        Assert.IsTrue(double.IsFinite(observed),"A reported numerical invariant must be finite.");
        Assert.IsTrue(global::System.Math.Abs(observed-expected)<=tolerance,
            $"Observed {observed:R}, expected {expected:R}, tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void HandVerifiedTwoByTwoSolveReportsSpectralConditionEstimate()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(2,1,1,3),FiniteVector64.Create([4,7]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Assert.IsNotNull(solved.TwoByTwoSpectralConditionEstimate);
        var root5=global::System.Math.Sqrt(5d);
        var independentRatio=(5d+root5)/(5d-root5);
        Near(solved.TwoByTwoSpectralConditionEstimate.Value,independentRatio,1e-12);
    }

    [TestMethod]
    public void DiagonalTwoByTwoConditionIsRatioOfMagnitudes()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(2,0,0,1),FiniteVector64.Create([2,1]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Near(solved.TwoByTwoSpectralConditionEstimate!.Value,2d);
        Near(solved.Solution!.At(0),1d);
        Near(solved.Solution.At(1),1d);
    }

    [TestMethod]
    public void ConditionNumberIsInvariantUnderAdmittedPositiveScaling()
    {
        var baseSolve=BoundedLinearAlgebra64.Solve(
            Matrix(2,0,0,1),FiniteVector64.Create([2,1]));
        var scaled=BoundedLinearAlgebra64.Solve(
            Matrix(2e80,0,0,1e80),FiniteVector64.Create([2e80,1e80]));
        Assert.IsTrue(scaled.HasUniqueSolution);
        Near(scaled.TwoByTwoSpectralConditionEstimate!.Value,
            baseSolve.TwoByTwoSpectralConditionEstimate!.Value,1e-12);
    }

    [TestMethod]
    public void ConditionNumberIsInvariantUnderNegativeDiagonalSigns()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(-2,0,0,1),FiniteVector64.Create([-2,1]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Near(solved.TwoByTwoSpectralConditionEstimate!.Value,2d,1e-12);
    }

    [TestMethod]
    public void OneByOneSolveDoesNotInventTwoByTwoCondition()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            FiniteMatrix64.Create(1,1,[3]),
            FiniteVector64.Create([6]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
    }

    [TestMethod]
    public void ThreeByThreeSolveDoesNotInventHigherOrderCondition()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            FiniteMatrix64.Create(3,3,[2,0,0,0,3,0,0,0,4]),
            FiniteVector64.Create([2,3,4]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
    }

    [TestMethod]
    public void SingularMatrixProvidesNoUniqueSolutionOrConditionEstimate()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(1,2,2,4),FiniteVector64.Create([3,6]));
        Assert.AreEqual(LinearAlgebraStatus64.Singular,solved.Status);
        Assert.IsNull(solved.Solution);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
    }

    [TestMethod]
    public void UnresolvableSpectrumLeavesConditionUnavailableRatherThanInventingValue()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(1,0,0,1e-18),
            FiniteVector64.Create([1,1e-18]),
            relativePivotThreshold:1e-20);
        Assert.IsTrue(solved.HasUniqueSolution);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
        Near(solved.Solution!.At(1),1d);
    }

    [TestMethod]
    public void NearDependentQrIsControlledByDeclaredRankTolerance()
    {
        var almost=Matrix(1,0,0,1e-13);
        var strict=BoundedLinearAlgebra64.FactorQr(almost);
        Assert.AreEqual(LinearAlgebraStatus64.RankDeficient,strict.Status);
        Assert.IsNull(strict.Q);
        var admitted=BoundedLinearAlgebra64.FactorQr(almost,1e-15);
        Assert.IsTrue(admitted.Factored);
        var restored=admitted.Q!.Multiply(admitted.R!);
        Near(restored.At(0,0),1d);
        Near(restored.At(1,1),1e-13,1e-15);
    }

    [TestMethod]
    public void NearSingularLuIsControlledByDeclaredPivotTolerance()
    {
        var matrix=Matrix(1,0,0,1e-13);
        var rhs=FiniteVector64.Create([1,1e-13]);
        var refused=BoundedLinearAlgebra64.Solve(matrix,rhs);
        Assert.AreEqual(LinearAlgebraStatus64.Singular,refused.Status);
        Assert.IsNull(refused.Solution);
        var admitted=BoundedLinearAlgebra64.Solve(matrix,rhs,1e-15);
        Assert.IsTrue(admitted.HasUniqueSolution);
        Near(admitted.Solution!.At(1),1d);
    }

    [TestMethod]
    public void HouseholderQrChecksReconstructionNotArbitraryColumnSigns()
    {
        var matrix=FiniteMatrix64.Create(3,2,[1,2,3,-4,-2,5]);
        var factor=BoundedLinearAlgebra64.FactorQr(matrix);
        Assert.IsTrue(factor.Factored);
        var product=factor.Q!.Multiply(factor.R!);
        for(var row=0;row<matrix.Rows;row++)
            for(var col=0;col<matrix.Columns;col++)
                Near(product.At(row,col),matrix.At(row,col),1e-10);
        for(var i=0;i<factor.Q.Rows;i++)
            for(var j=0;j<factor.Q.Columns;j++)
            {
                var dot=0d;
                for(var k=0;k<factor.Q.Rows;k++)
                    dot+=factor.Q.At(k,i)*factor.Q.At(k,j);
                Near(dot,i==j?1d:0d,1e-10);
            }
    }

    [TestMethod]
    public void SixteenBySixteenDeclaredDimensionCapIsExecutable()
    {
        const int n=16;
        var source=FiniteMatrix64.Create(n,n,
            Enumerable.Range(0,n*n).Select(i=>i%(n+1)==0?2d:0d));
        var solved=BoundedLinearAlgebra64.Solve(
            source,FiniteVector64.Create(Enumerable.Repeat(2d,n)));
        Assert.IsTrue(solved.HasUniqueSolution);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
        for(var i=0;i<n;i++)Near(solved.Solution!.At(i),1d);
        Near(solved.AbsoluteResidualInfinity!.Value,0d);
    }

    [TestMethod]
    public void FiniteSolveWithNonfiniteResidualNormalizerDoesNotFabricateRelativeClaim()
    {
        var maximum=double.MaxValue;
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(maximum,0,0,maximum),
            FiniteVector64.Create([maximum,maximum]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Near(solved.Solution!.At(0),1d);
        Near(solved.AbsoluteResidualInfinity!.Value,0d);
        Assert.IsNull(solved.RelativeResidualInfinity);
        Assert.IsNull(solved.TwoByTwoSpectralConditionEstimate);
    }

    [TestMethod]
    public void RowPermutationsPreserveSystemSolutionNotVectorSignConventions()
    {
        var matrix=Matrix(0,-2,3,1);
        var lu=BoundedLinearAlgebra64.FactorLu(matrix);
        Assert.IsTrue(lu.Factored);
        var permuted=lu.Lower!.Multiply(lu.Upper!);
        for(var row=0;row<2;row++)
            for(var col=0;col<2;col++)
                Near(permuted.At(row,col),matrix.At(lu.RowPermutation[row],col));
        var solved=BoundedLinearAlgebra64.Solve(
            matrix,FiniteVector64.Create([2,2]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Near(solved.Solution!.At(0),1d);
        Near(solved.Solution.At(1),-1d);
    }

    [TestMethod]
    public void VerySmallUniformScalingPreservesFiniteAdmittedConditioning()
    {
        var solved=BoundedLinearAlgebra64.Solve(
            Matrix(2e-100,0,0,1e-100),
            FiniteVector64.Create([2e-100,1e-100]));
        Assert.IsTrue(solved.HasUniqueSolution);
        Near(solved.Solution!.At(0),1d);
        Near(solved.TwoByTwoSpectralConditionEstimate!.Value,2d,1e-12);
    }

    [TestMethod]
    public void ConsumerAssemblyHasNoImplicitMathNetOrAuraBinding()
    {
        var referenced=typeof(BoundedLinearAlgebra64).Assembly
            .GetReferencedAssemblies().Select(v=>v.Name??string.Empty).ToArray();
        Assert.IsFalse(referenced.Any(v=>
            v.Contains("MathNet",StringComparison.OrdinalIgnoreCase)
            ||v.Contains("Aura",StringComparison.OrdinalIgnoreCase)
            ||v.Contains("Decision",StringComparison.OrdinalIgnoreCase)));
    }
}
