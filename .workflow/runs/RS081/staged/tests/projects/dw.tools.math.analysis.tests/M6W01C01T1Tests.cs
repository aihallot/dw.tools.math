using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C01T1Tests
{
    private static CurvePoint64[] Straight()=>[new(0,1),new(1,3),new(2,5)];
    private static CurvePoint64[] Points(params (double X,double Y)[] items)=>
        items.Select(point=>new CurvePoint64(point.X,point.Y)).ToArray();

    private static void Near(double? result,double expected,double tolerance=1e-10)
    {
        Assert.IsNotNull(result,"Expected admitted finite computation.");
        Assert.IsTrue(double.IsFinite(result.Value),"The numerical result must be finite.");
        Assert.IsTrue(global::System.Math.Abs(result.Value-expected)<=tolerance,
            $"Expected {expected:R}, actual {result.Value:R}, tol {tolerance:R}");
    }
    private static void Near(CurveValueResult64 actual,double expected)
    {
        Assert.AreEqual(CurveAnalysisStatus64.Success,actual.Status);
        Assert.IsTrue(actual.Succeeded);
        Near(actual.Value,expected);
    }
    private static void Refuse(CurveValueResult64 result,CurveAnalysisStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Value);
    }
    private static void Refuse(CurveFitResult64 result,CurveAnalysisStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Slope);
        Assert.IsNull(result.Intercept);
        Assert.IsNull(result.SumSquaredResiduals);
    }
    [TestMethod]
    public void LinearAtHalfReturnsTwo()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Straight(),0.5),2);
    }

    [TestMethod]
    public void LinearAtBeginningReturnsOriginalObservation()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Straight(),0),1);
    }

    [TestMethod]
    public void LinearAtFirstKnotReturnsOriginalObservation()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Straight(),1),3);
    }

    [TestMethod]
    public void LinearAtLastKnotReturnsOriginalObservation()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Straight(),2),5);
    }

    [TestMethod]
    public void LinearAtSecondSegmentIsCorrect()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Straight(),1.5),4);
    }

    [TestMethod]
    public void LinearHandlesNegativeSlope()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Points((0,6),(2,2)),1),4);
    }

    [TestMethod]
    public void LinearHandlesNegativeAbscissae()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Points((-3,-5),(-1,-1)), -2),-3);
    }

    [TestMethod]
    public void LinearReturnsExactPointAtInteriorKnot()
    {
        var p=Points((0,0),(1,17),(2,1));Near(BoundedCurveAnalysis64.InterpolateLinear(p,1),17);
    }

    [TestMethod]
    public void PiecewiseLineUsesLeftSegment()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Points((0,0),(1,1),(2,5)),0.5),0.5);
    }

    [TestMethod]
    public void PiecewiseLineUsesRightSegment()
    {
        Near(BoundedCurveAnalysis64.InterpolateLinear(Points((0,0),(1,1),(2,5)),1.5),3);
    }

    [TestMethod]
    public void NaturalSplineAtHalfPreservesCollinearData()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Straight(),0.5),2);
    }

    [TestMethod]
    public void NaturalSplineAtOneAndHalfPreservesCollinearData()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Straight(),1.5),4);
    }

    [TestMethod]
    public void NaturalSplineTwoPointsFallsBackToLinear()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,1),(1,3)),0.5),2);
    }

    [TestMethod]
    public void NaturalSplineReturnsKnotWithoutReconstructionDrift()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,0),(1,1),(2,0)),1),1);
    }

    [TestMethod]
    public void NaturalSplineCurvedFixtureAtHalfMatchesExactBclOracle()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,0),(1,1),(2,0)),0.5),0.6875);
    }

    [TestMethod]
    public void NaturalSplineCurvedFixtureIsSymmetric()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,0),(1,1),(2,0)),1.5),0.6875);
    }

    [TestMethod]
    public void NaturalSplineFourLinearKnotsPreserveLine()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,1),(1,3),(2,5),(3,7)),1.25),3.5);
    }

    [TestMethod]
    public void NaturalSplineNegativeYCanBeInterpolated()
    {
        Near(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,-2),(1,-4),(2,-6)),0.5),-3);
    }

    [TestMethod]
    public void LeastSquaresFitsAcceptedSlope()
    {
        Near(BoundedCurveAnalysis64.FitLeastSquares(Straight()).Slope,2);
    }

    [TestMethod]
    public void LeastSquaresFitsAcceptedIntercept()
    {
        Near(BoundedCurveAnalysis64.FitLeastSquares(Straight()).Intercept,1);
    }

    [TestMethod]
    public void LeastSquaresFitsAcceptedZeroResidual()
    {
        Near(BoundedCurveAnalysis64.FitLeastSquares(Straight()).SumSquaredResiduals,0);
    }

    [TestMethod]
    public void LeastSquaresTwoSamplesFitExactly()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((0,1),(1,3)));Near(r.Slope,2);Near(r.Intercept,1);Near(r.SumSquaredResiduals,0);
    }

    [TestMethod]
    public void LeastSquaresNegativeSlopeAdmitted()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((0,4),(1,2),(2,0)));Near(r.Slope,-2);Near(r.Intercept,4);
    }

    [TestMethod]
    public void LeastSquaresFlatLineAdmitted()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((0,7),(1,7),(2,7)));Near(r.Slope,0);Near(r.Intercept,7);
    }

    [TestMethod]
    public void LeastSquaresNoisyPointsReportNonzeroSse()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((0,0),(1,1),(2,0)));Near(r.Slope,0);Near(r.Intercept,1d/3);Near(r.SumSquaredResiduals,2d/3);
    }

    [TestMethod]
    public void LeastSquaresThreePointsWithShiftedX()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((10,21),(11,23),(12,25)));Near(r.Slope,2);Near(r.Intercept,1);
    }

    [TestMethod]
    public void NullPointsAreTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(null,0),CurveAnalysisStatus64.InvalidInput);Refuse(BoundedCurveAnalysis64.FitLeastSquares(null),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void EmptyPointsAreTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateNaturalSpline([],0),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void SinglePointCannotProveInterpolation()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Points((0,1)),0),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void DuplicateAbscissaRejectsInterpolation()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Points((0,1),(0,2)),0),CurveAnalysisStatus64.DuplicateAbscissa);
    }

    [TestMethod]
    public void DuplicateAbscissaRejectsFit()
    {
        Refuse(BoundedCurveAnalysis64.FitLeastSquares(Points((0,1),(0,2))),CurveAnalysisStatus64.DuplicateAbscissa);
    }

    [TestMethod]
    public void UnsortedPointsCannotSilentlyReorder()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Points((1,1),(0,0)),0.5),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void NaNAbscissaIsTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,0),(double.NaN,1)),0),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void NonFiniteYIsTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.FitLeastSquares(Points((0,1),(1,double.PositiveInfinity))),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void NonFiniteQueryIsTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Straight(),double.NaN),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void InfiniteQueryIsTypedInvalidInput()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateNaturalSpline(Straight(),double.PositiveInfinity),CurveAnalysisStatus64.InvalidInput);
    }

    [TestMethod]
    public void LinearNeverExtrapolatesBelowDomain()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Straight(),-1),CurveAnalysisStatus64.OutsideDomain);
    }

    [TestMethod]
    public void SplineNeverExtrapolatesAboveDomain()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateNaturalSpline(Straight(),3),CurveAnalysisStatus64.OutsideDomain);
    }

    [TestMethod]
    public void TooManySamplesExplicitlyExceedBudget()
    {
        var p=Enumerable.Range(0,33).Select(i=>new CurvePoint64(i,i)).ToArray();Refuse(BoundedCurveAnalysis64.FitLeastSquares(p),CurveAnalysisStatus64.BudgetExceeded);
    }

    [TestMethod]
    public void MaximumSamplesAreAdmitted()
    {
        var p=Enumerable.Range(0,32).Select(i=>new CurvePoint64(i,2*i+1)).ToArray();Near(BoundedCurveAnalysis64.InterpolateLinear(p,30.5),62);
    }

    [TestMethod]
    public void InfSlopeFromOpposingExtremesRefuses()
    {
        Refuse(BoundedCurveAnalysis64.InterpolateLinear(Points((-double.MaxValue,-double.MaxValue),(double.MaxValue,double.MaxValue)),0),CurveAnalysisStatus64.NonFiniteIntermediate);
    }

    [TestMethod]
    public void TinyNonzeroDistinctAbscissaeHaveRankDeficiency()
    {
        Refuse(BoundedCurveAnalysis64.FitLeastSquares(Points((0,0),(double.Epsilon,1))),CurveAnalysisStatus64.RankDeficient);
    }

    [TestMethod]
    public void SuccessResultsExposeMeasuredRuleAndFiniteValues()
    {
        var r=BoundedCurveAnalysis64.InterpolateLinear(Straight(),0.5);Assert.AreEqual(BoundedCurveAnalysis64.RuleId,r.RuleId);Assert.IsTrue(r.Succeeded);Assert.IsTrue(double.IsFinite(r.Value!.Value));
    }

    [TestMethod]
    public void FailedResultNeverCarriesAValue()
    {
        var r=BoundedCurveAnalysis64.InterpolateLinear(Straight(),-1);Assert.IsFalse(r.Succeeded);Assert.IsNull(r.Value);
    }

    [TestMethod]
    public void FailedFitNeverCarriesNumericCoefficients()
    {
        var r=BoundedCurveAnalysis64.FitLeastSquares(Points((0,1),(0,2)));Assert.IsFalse(r.Succeeded);Assert.IsNull(r.Slope);Assert.IsNull(r.Intercept);Assert.IsNull(r.SumSquaredResiduals);
    }

    [TestMethod]
    public void MutationOfCallerArrayDoesNotRetroactivelyChangeReceipt()
    {
        var p=Straight();var result=BoundedCurveAnalysis64.InterpolateLinear(p,0.5);p[0]=new(0,1000);Near(result,2);
    }

    [TestMethod]
    public void IndependentBinary64AnalysisDoesNotRequireForeignAgent()
    {
        var deps=typeof(BoundedCurveAnalysis64).Assembly.GetReferencedAssemblies().Select(a=>a.Name??"");Assert.IsFalse(deps.Any(n=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}.Any(k=>n.Contains(k,StringComparison.OrdinalIgnoreCase))));
    }
}
