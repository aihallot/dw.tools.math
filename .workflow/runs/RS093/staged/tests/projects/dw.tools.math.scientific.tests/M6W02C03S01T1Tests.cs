using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S01T1Tests
{
    private static void Near(FiniteGeometryOutcome64 result, double expected, double tolerance = 1e-12)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteGeometryStatus64.Success, result.Status);
        Assert.IsNotNull(result.Scalar);
        Assert.IsNull(result.Coordinates);
        Assert.IsTrue(global::System.Math.Abs(result.Scalar.Value - expected) <= tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void VectorNear(FiniteGeometryOutcome64 result, double[] expected,
        double tolerance = 1e-12)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteGeometryStatus64.Success, result.Status);
        Assert.IsNull(result.Scalar);
        Assert.IsNotNull(result.Coordinates);
        Assert.HasCount(expected.Length, result.Coordinates);
        for (var i = 0; i < expected.Length; i++)
            Assert.IsTrue(global::System.Math.Abs(result.Coordinates[i] - expected[i]) <= tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Refused(FiniteGeometryOutcome64 result, FiniteGeometryStatus64 reason)
    {
        Assert.AreEqual(reason, result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Scalar);
        Assert.IsNull(result.Coordinates);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    [TestMethod] public void NormThreeFourIsFive()
    { Near(FiniteGeometry64.Norm([3d, 4d]), 5d); }

    [TestMethod] public void NormThreeDimensionalThreeFourZero()
    { Near(FiniteGeometry64.Norm([3d, 4d, 0d]), 5d); }

    [TestMethod] public void NormZeroTwoDimensional()
    { Near(FiniteGeometry64.Norm([0d, 0d]), 0d); }

    [TestMethod] public void NormZeroThreeDimensional()
    { Near(FiniteGeometry64.Norm([0d, 0d, 0d]), 0d); }

    [TestMethod] public void NormNegativeComponents()
    { Near(FiniteGeometry64.Norm([-3d, -4d]), 5d); }

    [TestMethod] public void NormOfOneTenthComponents()
    { Near(FiniteGeometry64.Norm([0.3d, 0.4d]), 0.5d); }

    [TestMethod] public void NormOfVeryLargeFiniteComponents()
    { Near(FiniteGeometry64.Norm([3e90d, 4e90d]), 5e90d, 1e76d); }

    [TestMethod] public void DistanceFromOriginThreeFour()
    { Near(FiniteGeometry64.Distance([0d, 0d], [3d, 4d]), 5d); }

    [TestMethod] public void DistanceSymmetry()
    { Near(FiniteGeometry64.Distance([3d, 4d], [0d, 0d]), 5d); }

    [TestMethod] public void DistanceSamePoint()
    { Near(FiniteGeometry64.Distance([3d, 4d], [3d, 4d]), 0d); }

    [TestMethod] public void DistanceInThreeDimensions()
    { Near(FiniteGeometry64.Distance([1d, 2d, 3d], [4d, 6d, 3d]), 5d); }

    [TestMethod] public void ProjectionOntoXAxis()
    { VectorNear(FiniteGeometry64.Project([3d, 4d], [1d, 0d]), [3d, 0d]); }

    [TestMethod] public void ProjectionOntoYAxis()
    { VectorNear(FiniteGeometry64.Project([3d, 4d], [0d, 1d]), [0d, 4d]); }

    [TestMethod] public void ProjectionOntoDiagonal()
    { VectorNear(FiniteGeometry64.Project([2d, 0d], [1d, 1d]), [1d, 1d]); }

    [TestMethod] public void ProjectionInThreeDimensions()
    { VectorNear(FiniteGeometry64.Project([3d, 4d, 5d], [1d, 0d, 0d]), [3d, 0d, 0d]); }

    [TestMethod] public void ProjectionIgnoresAxisScale()
    { VectorNear(FiniteGeometry64.Project([3d, 4d], [10d, 0d]), [3d, 0d]); }

    [TestMethod] public void InvalidVectorDimensionIsRefused()
    { Refused(FiniteGeometry64.Norm([1d]), FiniteGeometryStatus64.InvalidDimension); }

    [TestMethod] public void MismatchedDistanceDimensionsAreRefused()
    { Refused(FiniteGeometry64.Distance([1d, 2d], [1d, 2d, 3d]), FiniteGeometryStatus64.InvalidDimension); }

    [TestMethod] public void ZeroProjectionAxisIsRefused()
    { Refused(FiniteGeometry64.Project([3d, 4d], [0d, 0d]), FiniteGeometryStatus64.ZeroAxis); }

    [TestMethod] public void NonfiniteCoordinateIsRefused()
    { Refused(FiniteGeometry64.Norm([double.NaN, 4d]), FiniteGeometryStatus64.InvalidInput); }

    [TestMethod] public void OutOfBudgetMagnitudeIsRefused()
    { Refused(FiniteGeometry64.Norm([1e101d, 1d]), FiniteGeometryStatus64.OutsideQualifiedRange); }

    [TestMethod] public void TinyNonzeroAxisIsOutsideQualifiedRange()
    { Refused(FiniteGeometry64.Project([3d, 4d], [1e-200d, 0d]), FiniteGeometryStatus64.OutsideQualifiedRange); }

    [TestMethod] public void PreCancelledComputationIsRefused()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        Refused(FiniteGeometry64.Norm([3d, 4d], source.Token), FiniteGeometryStatus64.Cancelled);
    }

    [TestMethod] public void SuccessfulRuleIdentityAndApproximationAreExplicit()
    {
        var result = FiniteGeometry64.Norm([3d, 4d]);
        Near(result, 5d);
        Assert.AreEqual(FiniteGeometry64.RuleId, result.RuleId);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }
}
