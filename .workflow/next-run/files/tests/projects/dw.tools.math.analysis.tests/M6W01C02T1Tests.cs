using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C02T1Tests
{
    private static readonly Func<double,double> Polynomial = x => x*x-2d;
    private static RootSolveResult64 Root(Func<double,double>? f = null,
        RootPolicy64? p = null) =>
        BoundedUnivariateAnalysis64.SolveBracketed(f ?? Polynomial, 1d, 2d, p);
    private static QuadratureResult64 Integral(Func<double,double>? f = null,
        QuadraturePolicy64? p = null) =>
        BoundedUnivariateAnalysis64.IntegrateSimpson(f ?? (x => x*x), 0d, 1d, p);
    private static void Near(double? actual,double expected,double tolerance=1e-8)
    {
        Assert.IsNotNull(actual);
        Assert.IsTrue(double.IsFinite(actual.Value));
        Assert.IsTrue(global::System.Math.Abs(actual.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {actual.Value:R}");
    }
    private static void RootRejected(RootSolveResult64 value, UnivariateStatus64 expected)
    {
        Assert.AreEqual(expected,value.Status);
        Assert.IsFalse(value.Succeeded);
        Assert.IsNull(value.Root);
        Assert.IsNull(value.AbsoluteResidual);
        Assert.IsNull(value.ObservedBracketWidth);
        Assert.IsFalse(value.GuaranteedRootError);
    }
    private static void QuadratureRejected(QuadratureResult64 value, UnivariateStatus64 expected)
    {
        Assert.AreEqual(expected,value.Status);
        Assert.IsFalse(value.Succeeded);
        Assert.IsNull(value.Integral);
        Assert.IsNull(value.EstimatedAbsoluteError);
        Assert.IsFalse(value.ErrorBoundGuaranteed);
    }

    [TestMethod] public void PolynomialRootIsSqrtTwo()
    { var r=Root();Assert.IsTrue(r.Succeeded);Near(r.Root,global::System.Math.Sqrt(2),1e-8); }
    [TestMethod] public void RootResidualIsObservedAndBoundedByRequestedTolerance()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.AbsoluteResidual<=1e-10); }
    [TestMethod] public void RootReportsFiniteBracketWidth()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.ObservedBracketWidth>=0&&r.ObservedBracketWidth<=1); }
    [TestMethod] public void RootReportsActualIterationAndFunctionBudgets()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.Iterations>0&&r.Iterations<=128);Assert.AreEqual(r.Iterations+2,r.FunctionEvaluations); }
    [TestMethod] public void RootNeverCertifiesUnprovedErrorBound()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.GuaranteedRootError); }
    [TestMethod] public void LowerEndpointRootNeedsNoIteration()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x-1,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,1);Assert.AreEqual(0,r.Iterations); }
    [TestMethod] public void UpperEndpointRootNeedsNoIteration()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x-2,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,2);Assert.AreEqual(0,r.Iterations); }
    [TestMethod] public void NegativeSlopeBracketIsAdmitted()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>2-x,1,3);Assert.IsTrue(r.Succeeded);Near(r.Root,2); }
    [TestMethod] public void BisectionHandlesSignedOppositeWithoutOverflowingProduct()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x<0?-1e200:1e200,-1,1);Assert.IsFalse(r.Succeeded);Assert.AreEqual(UnivariateStatus64.NonConverged,r.Status); }
    [TestMethod] public void MissingBracketProducesTypedNonpartialRefusal()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(x=>x*x+1,1,2),UnivariateStatus64.MissingBracket); }
    [TestMethod] public void NullFunctionIsInvalidInput()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(null,1,2),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void ReversedIntervalIsInvalidInput()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(Polynomial,2,1),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void ZeroWidthIntervalIsInvalidInput()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(Polynomial,1,1),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void NaNEndpointIsInvalidInput()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(Polynomial,double.NaN,2),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void InfiniteFunctionIsTypedRefusal()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(_=>double.PositiveInfinity,1,2),UnivariateStatus64.NonFiniteEvaluation); }
    [TestMethod] public void ThrowingFunctionIsTypedRefusal()
    { RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(_=>throw new InvalidOperationException("failed"),1,2),UnivariateStatus64.EvaluationFailed); }
    [TestMethod] public void InvalidZeroResidualToleranceRefused()
    { RootRejected(Root(p:new(1e-10,0,128)),UnivariateStatus64.InvalidPolicy); }
    [TestMethod] public void InvalidIterationBudgetRefused()
    { RootRejected(Root(p:new(1e-10,1e-10,257)),UnivariateStatus64.InvalidPolicy); }
    [TestMethod] public void IterationLimitIsNonconvergenceNotPartialRoot()
    { RootRejected(Root(p:new(1e-15,1e-15,1)),UnivariateStatus64.NonConverged); }
    [TestMethod] public void CancelledRootIsNonpartial()
    { using var c=new CancellationTokenSource();c.Cancel();RootRejected(BoundedUnivariateAnalysis64.SolveBracketed(Polynomial,1,2,cancellationToken:c.Token),UnivariateStatus64.Cancelled); }
    [TestMethod] public void RootRuleIsVersionedRuntimeEvidence()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.AreEqual(BoundedUnivariateAnalysis64.RuleId,r.RuleId); }

    [TestMethod] public void CompositeSimpsonIntegratesSquareToOneThird()
    { var q=Integral();Assert.IsTrue(q.Succeeded);Near(q.Integral,1d/3d,1e-9); }
    [TestMethod] public void SimpsonIntegratesConstantFunction()
    { var q=Integral(_=>4);Assert.IsTrue(q.Succeeded);Near(q.Integral,4); }
    [TestMethod] public void SimpsonIntegratesLinearFunction()
    { var q=Integral(x=>2*x+1);Assert.IsTrue(q.Succeeded);Near(q.Integral,2); }
    [TestMethod] public void SimpsonIntegratesSymmetricQuadratic()
    { var q=BoundedUnivariateAnalysis64.IntegrateSimpson(x=>x*x,-1,1);Assert.IsTrue(q.Succeeded);Near(q.Integral,2d/3d); }
    [TestMethod] public void SimpsonIntegratesSineOnZeroToPi()
    { var q=BoundedUnivariateAnalysis64.IntegrateSimpson(global::System.Math.Sin,0,global::System.Math.PI,new(1e-6,128,512));Assert.IsTrue(q.Succeeded);Near(q.Integral,2,1e-5); }
    [TestMethod] public void SimpsonReportsRefinedEvenPanelCount()
    { var q=Integral();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.Panels>=4&&q.Panels<=128&&q.Panels%2==0); }
    [TestMethod] public void SimpsonReportsObservedEstimationNotRigorousBound()
    { var q=Integral();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.EstimatedAbsoluteError>=0&&q.EstimatedAbsoluteError<=1e-8);Assert.IsFalse(q.ErrorBoundGuaranteed); }
    [TestMethod] public void SimpsonReportsBoundedEvaluationCount()
    { var q=Integral();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.FunctionEvaluations>=8&&q.FunctionEvaluations<=1025); }
    [TestMethod] public void SimpsonRejectsReversedInterval()
    { QuadratureRejected(BoundedUnivariateAnalysis64.IntegrateSimpson(x=>x,2,1),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void SimpsonRejectsNullCallback()
    { QuadratureRejected(BoundedUnivariateAnalysis64.IntegrateSimpson(null,0,1),UnivariateStatus64.InvalidInput); }
    [TestMethod] public void SimpsonRejectsNonfiniteValue()
    { QuadratureRejected(Integral(_=>double.NaN),UnivariateStatus64.NonFiniteEvaluation); }
    [TestMethod] public void SimpsonRejectsThrowingCallback()
    { QuadratureRejected(Integral(_=>throw new ArithmeticException("fail")),UnivariateStatus64.EvaluationFailed); }
    [TestMethod] public void SimpsonRejectsInvalidOddPanelBudget()
    { QuadratureRejected(Integral(p:new(1e-8,7,1025)),UnivariateStatus64.InvalidPolicy); }
    [TestMethod] public void SimpsonRejectsZeroTolerance()
    { QuadratureRejected(Integral(p:new(0,128,1025)),UnivariateStatus64.InvalidPolicy); }
    [TestMethod] public void SimpsonRejectsInsufficientEvaluationBudget()
    { QuadratureRejected(Integral(p:new(1e-8,128,7)),UnivariateStatus64.InvalidPolicy); }
    [TestMethod] public void SimpsonRejectsBudgetExhaustionWithoutPartialIntegral()
    { QuadratureRejected(Integral(x=>global::System.Math.Exp(x),new(1e-20,128,8)),UnivariateStatus64.BudgetExceeded); }
    [TestMethod] public void SimpsonRejectsNonconvergedBoundedPanelCount()
    { QuadratureRejected(Integral(x=>global::System.Math.Exp(x),new(1e-20,4,1025)),UnivariateStatus64.NonConverged); }
    [TestMethod] public void SimpsonRejectsCancelledRequest()
    { using var c=new CancellationTokenSource();c.Cancel();QuadratureRejected(BoundedUnivariateAnalysis64.IntegrateSimpson(x=>x,0,1,cancellationToken:c.Token),UnivariateStatus64.Cancelled); }
    [TestMethod] public void SimpsonRuleIsVersionedAtRuntime()
    { var q=Integral();Assert.IsTrue(q.Succeeded);Assert.AreEqual(BoundedUnivariateAnalysis64.RuleId,q.RuleId); }
    [TestMethod] public void RepeatCalculationsProduceIdenticalRecords()
    { Assert.AreEqual(Root(),Root());Assert.AreEqual(Integral(),Integral()); }
    [TestMethod] public void PreviouslyQualifiedCurveStillMatchesExactOracle()
    { var r=BoundedCurveAnalysis64.InterpolateLinear([new(0,1),new(1,3),new(2,5)],0.5);Assert.IsTrue(r.Succeeded);Near(r.Value,2); }
    [TestMethod] public void LibraryHasNoAgentOrForeignSolverReference()
    { var names=typeof(BoundedUnivariateAnalysis64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(names.Any(n=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}.Any(token=>n.Contains(token,StringComparison.OrdinalIgnoreCase)))); }
}
