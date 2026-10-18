using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S01T2Tests
{
    private static void Scalar(FiniteGeometryAuditResult64 result, double expected, double tolerance = 1e-12d)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteGeometryAuditStatus64.Success, result.Status);
        Assert.IsNotNull(result.Outcome);
        Assert.IsNotNull(result.Outcome.Scalar);
        Assert.IsNull(result.Outcome.Coordinates);
        Assert.IsTrue(global::System.Math.Abs(result.Outcome.Scalar.Value - expected) <= tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Vector(FiniteGeometryAuditResult64 result, double[] expected, double tolerance = 1e-12d)
    {
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(FiniteGeometryAuditStatus64.Success, result.Status);
        Assert.IsNotNull(result.Outcome);
        Assert.IsNull(result.Outcome.Scalar);
        Assert.IsNotNull(result.Outcome.Coordinates);
        Assert.AreEqual(expected.Length, result.Outcome.Coordinates.Length);
        for (var i = 0; i < expected.Length; i++)
            Assert.IsTrue(global::System.Math.Abs(result.Outcome.Coordinates[i] - expected[i]) <= tolerance);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }

    private static void Refused(FiniteGeometryAuditResult64 result,
        FiniteGeometryAuditStatus64 status, bool requiresRiskRatio = false)
    {
        Assert.AreEqual(status, result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Outcome);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
        if (requiresRiskRatio)
        {
            Assert.IsNotNull(result.RelativeSignal);
            Assert.IsTrue(double.IsFinite(result.RelativeSignal.Value));
            Assert.IsTrue(result.RelativeSignal.Value >= 0d);
        }
    }

    [TestMethod] public void NormThreeFourRetained()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([3d, 4d]), 5d); }

    [TestMethod] public void NormThreeDimensional()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([3d, 4d, 0d]), 5d); }

    [TestMethod] public void NormZero()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([0d, 0d]), 0d); }

    [TestMethod] public void NormSignedComponents()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([-3d, -4d]), 5d); }

    [TestMethod] public void NormMaximumAdmittedComponent()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateNorm([1e100d, 0d]), 1e100d, 1e85d); }

    [TestMethod] public void NormNullRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm(null), FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void NormOneDimensionRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([1d]), FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void NormFourDimensionsRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([1d, 2d, 3d, 4d]), FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void NormNaNRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([double.NaN, 1d]), FiniteGeometryAuditStatus64.InvalidInput); }

    [TestMethod] public void NormInfinityRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([double.PositiveInfinity, 1d]), FiniteGeometryAuditStatus64.InvalidInput); }

    [TestMethod] public void NormOutsideBoundRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([1e101d, 0d]), FiniteGeometryAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void NormPreCancelledRefused()
    { using var cancel = new CancellationTokenSource(); cancel.Cancel(); Refused(FiniteGeometryBoundaryAudit64.EvaluateNorm([3d, 4d], cancel.Token), FiniteGeometryAuditStatus64.Cancelled); }

    [TestMethod] public void NormDoesNotInventConditionRatio()
    { var observed=FiniteGeometryBoundaryAudit64.EvaluateNorm([3d,4d]); Scalar(observed,5d); Assert.IsNull(observed.RelativeSignal); }

    [TestMethod] public void DistanceOriginThreeFour()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[3d,4d]),5d); }

    [TestMethod] public void DistanceSymmetric()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([3d,4d],[0d,0d]),5d); }

    [TestMethod] public void DistanceThreeDimensions()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([1d,2d,3d],[4d,6d,3d]),5d); }

    [TestMethod] public void DistanceEqualLargeCoordinates()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d,0d],[1e14d,0d]),0d); }

    [TestMethod] public void DistanceBothOrigin()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[0d,0d]),0d); }

    [TestMethod] public void DistanceTinyNearOrigin()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[1e-100d,0d]),1e-100d,1e-112d); }

    [TestMethod] public void DistanceLargeButResolvable()
    { Scalar(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d,0d],[1e14d+1000d,0d]),1000d); }

    [TestMethod] public void DistanceCloseHugeCoordinatesRisk()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d,0d],[1e14d+1d,0d]), FiniteGeometryAuditStatus64.RelativeResolutionRisk, true); }

    [TestMethod] public void DistanceCloseNegativeHugeCoordinatesRisk()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([-1e14d,0d],[-1e14d+1d,0d]), FiniteGeometryAuditStatus64.RelativeResolutionRisk, true); }

    [TestMethod] public void DistanceCloseHugeThreeDimensionalRisk()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d,0d,0d],[1e14d+1d,0d,0d]), FiniteGeometryAuditStatus64.RelativeResolutionRisk, true); }

    [TestMethod] public void DistanceRiskSymmetry()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d+1d,0d],[1e14d,0d]), FiniteGeometryAuditStatus64.RelativeResolutionRisk, true); }

    [TestMethod] public void DistanceNonFiniteRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,double.NaN],[1d,2d]), FiniteGeometryAuditStatus64.InvalidInput); }

    [TestMethod] public void DistanceDimensionsRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1d,2d],[1d,2d,3d]), FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void DistanceNullRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance(null,[1d,2d]), FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void DistancePreCancelledRefused()
    { using var cancel = new CancellationTokenSource(); cancel.Cancel(); Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[3d,4d],cancel.Token),FiniteGeometryAuditStatus64.Cancelled); }

    [TestMethod] public void DistanceObservedRatioIsFinite()
    { var result=FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[3d,4d]); Scalar(result,5d); Assert.IsNotNull(result.RelativeSignal); Assert.IsTrue(double.IsFinite(result.RelativeSignal.Value)); }

    [TestMethod] public void ProjectionXAxis()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1d,0d]),[3d,0d]); }

    [TestMethod] public void ProjectionYAxis()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[0d,1d]),[0d,4d]); }

    [TestMethod] public void ProjectionThreeDimensional()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d,5d],[0d,0d,1d]),[0d,0d,5d]); }

    [TestMethod] public void ProjectionDiagonal()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([2d,0d],[1d,1d]),[1d,1d]); }

    [TestMethod] public void ProjectionNegativeAxisSameResult()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[-1d,0d]),[3d,0d]); }

    [TestMethod] public void ProjectionAdmittedTinyAxisAtThreshold()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1e-100d,0d]),[3d,0d]); }

    [TestMethod] public void ProjectionTinyNonzeroAxisRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1e-101d,0d]),FiniteGeometryAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void ProjectionZeroAxisRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[0d,0d]),FiniteGeometryAuditStatus64.ZeroAxis); }

    [TestMethod] public void ProjectionNullAxisRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],null),FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void ProjectionDimensionMismatchRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d,5d],[1d,0d]),FiniteGeometryAuditStatus64.InvalidDimension); }

    [TestMethod] public void ProjectionNaNRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[double.NaN,0d]),FiniteGeometryAuditStatus64.InvalidInput); }

    [TestMethod] public void ProjectionOutOfBudgetRefused()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([1e101d,1d],[1d,0d]),FiniteGeometryAuditStatus64.OutsideQualifiedRange); }

    [TestMethod] public void ProjectionPreCancelledRefused()
    { using var cancel = new CancellationTokenSource(); cancel.Cancel(); Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1d,0d],cancel.Token),FiniteGeometryAuditStatus64.Cancelled); }

    [TestMethod] public void ProjectionOrthogonalDotCancellation()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([1e10d,1e10d],[1d,-1d]),FiniteGeometryAuditStatus64.ProjectionCancellationRisk,true); }

    [TestMethod] public void ProjectionNearOrthogonalDotCancellation()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([1e10d,1e10d],[1d,-1d+1e-13d]),FiniteGeometryAuditStatus64.ProjectionCancellationRisk,true); }

    [TestMethod] public void ProjectionThreeDimensionalDotCancellation()
    { Refused(FiniteGeometryBoundaryAudit64.EvaluateProjection([1e10d,1e10d,7d],[1d,-1d,0d]),FiniteGeometryAuditStatus64.ProjectionCancellationRisk,true); }

    [TestMethod] public void ProjectionZeroVectorIsAdmitted()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([0d,0d],[1d,1d]),[0d,0d]); }

    [TestMethod] public void ProjectionScaleInvariance()
    { Vector(FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1e90d,0d]),[3d,0d]); }

    [TestMethod] public void ProjectionObservedSignalRatio()
    { var result=FiniteGeometryBoundaryAudit64.EvaluateProjection([3d,4d],[1d,0d]); Vector(result,[3d,0d]); Assert.IsNotNull(result.RelativeSignal); Assert.IsTrue(result.RelativeSignal.Value>0.99d); }

    [TestMethod] public void BoundaryRuleIdFromRuntime()
    { var result=FiniteGeometryBoundaryAudit64.EvaluateNorm([3d,4d]); Scalar(result,5d); Assert.AreEqual(FiniteGeometryBoundaryAudit64.RuleId,result.RuleId); }

    [TestMethod] public void BackendRulePreserved()
    { var result=FiniteGeometryBoundaryAudit64.EvaluateNorm([3d,4d]); Scalar(result,5d); Assert.IsNotNull(result.Outcome); Assert.AreEqual(FiniteGeometry64.RuleId,result.Outcome.RuleId); }

    [TestMethod] public void ExplicitNoErrorTheorem()
    { var result=FiniteGeometryBoundaryAudit64.EvaluateDistance([0d,0d],[3d,4d]); Scalar(result,5d); Assert.IsFalse(result.ErrorBoundGuaranteed); Assert.IsFalse(result.Outcome!.ErrorBoundGuaranteed); }

    [TestMethod] public void OriginalT1RemainsAvailableForRiskCase()
    { var baseline=FiniteGeometry64.Distance([1e14d,0d],[1e14d+1d,0d]); Assert.IsTrue(baseline.Succeeded); Assert.IsNotNull(baseline.Scalar); Assert.AreEqual(1d,baseline.Scalar.Value); Refused(FiniteGeometryBoundaryAudit64.EvaluateDistance([1e14d,0d],[1e14d+1d,0d]),FiniteGeometryAuditStatus64.RelativeResolutionRisk,true); }
}
