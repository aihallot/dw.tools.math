using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C03T1Tests
{
    private static readonly Func<double,double> Square=x=>x*x;
    private static readonly Func<double,double,double> Growth=(t,y)=>y;
    private static DerivativeResult64 D(Func<double,double>? f=null,
        double at=2d, DerivativePolicy64? policy=null) =>
        BoundedDifferentialAnalysis64.Differentiate(f??Square,at,policy);
    private static OdeResult64 O(Func<double,double,double>? f=null,
        double start=0d,double end=1d,double initial=1d,OdePolicy64? policy=null) =>
        BoundedDifferentialAnalysis64.IntegrateOde(f??Growth,start,end,initial,policy);
    private static void Near(double? actual,double expected,double tolerance=1e-7)
    {
        Assert.IsNotNull(actual);
        Assert.IsTrue(double.IsFinite(actual.Value));
        Assert.IsTrue(global::System.Math.Abs(actual.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {actual.Value:R}");
    }
    private static void DerivativeRefused(DerivativeResult64 result,DifferentialStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Value);
        Assert.IsNull(result.EstimatedAbsoluteError);
        Assert.IsNull(result.UsedStep);
        Assert.AreEqual(0,result.FunctionEvaluations);
        Assert.IsFalse(result.ErrorBoundGuaranteed);
    }
    private static void OdeRefused(OdeResult64 result,DifferentialStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.FinalValue);
        Assert.IsNull(result.EstimatedAbsoluteError);
        Assert.AreEqual(0,result.Steps);
        Assert.AreEqual(0,result.FunctionEvaluations);
        Assert.IsFalse(result.GlobalErrorBoundGuaranteed);
    }

    [TestMethod] public void SquareDerivativeAtTwoIsFour()
    { var d=D();Assert.IsTrue(d.Succeeded);Near(d.Value,4d,1e-9); }
    [TestMethod] public void SquareDerivativeAtNegativeTwoIsNegativeFour()
    { var d=D(at:-2);Assert.IsTrue(d.Succeeded);Near(d.Value,-4d,1e-9); }
    [TestMethod] public void ConstantFunctionDerivativeIsZero()
    { var d=D(_=>7);Assert.IsTrue(d.Succeeded);Near(d.Value,0); }
    [TestMethod] public void AffineFunctionDerivativeIsSlope()
    { var d=D(x=>3*x+1);Assert.IsTrue(d.Succeeded);Near(d.Value,3); }
    [TestMethod] public void SineDerivativeAtZeroIsOne()
    { var d=D(global::System.Math.Sin,0);Assert.IsTrue(d.Succeeded);Near(d.Value,1,1e-6); }
    [TestMethod] public void ExponentialDerivativeAtZeroIsOne()
    { var d=D(global::System.Math.Exp,0);Assert.IsTrue(d.Succeeded);Near(d.Value,1,1e-6); }
    [TestMethod] public void FourthPowerDerivativeAtTwoIsThirtyTwo()
    { var d=D(x=>x*x*x*x,2,new(1e-4,1e-5,4));Assert.IsTrue(d.Succeeded);Near(d.Value,32,1e-6); }
    [TestMethod] public void DerivativeIncludesObservedNoncertifiedEstimate()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.IsTrue(d.EstimatedAbsoluteError>=0&&d.EstimatedAbsoluteError<=1e-6);Assert.IsFalse(d.ErrorBoundGuaranteed); }
    [TestMethod] public void DerivativeUsesRefinedHalfStep()
    { var d=D();Assert.IsTrue(d.Succeeded);Near(d.UsedStep,0.0005,1e-12); }
    [TestMethod] public void DerivativeUsesExactlyFourCallbackEvaluations()
    { var count=0;var d=D(x=>{count++;return x*x;});Assert.IsTrue(d.Succeeded);Assert.AreEqual(4,count);Assert.AreEqual(4,d.FunctionEvaluations); }
    [TestMethod] public void DerivativeRuleIsObservedAtRuntime()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.AreEqual(BoundedDifferentialAnalysis64.RuleId,d.RuleId); }
    [TestMethod] public void DerivativePolicyRefusesZeroStep()
    { DerivativeRefused(D(policy:new(0,1e-6,4)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativePolicyRefusesNegativeStep()
    { DerivativeRefused(D(policy:new(-0.1,1e-6,4)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativePolicyRefusesHugeStep()
    { DerivativeRefused(D(policy:new(2,1e-6,4)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativePolicyRefusesNaNTolerance()
    { DerivativeRefused(D(policy:new(1e-3,double.NaN,4)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativePolicyRefusesExcessBudget()
    { DerivativeRefused(D(policy:new(1e-3,1e-6,1025)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativeInsufficientBudgetIsTypedRefusal()
    { DerivativeRefused(D(policy:new(1e-3,1e-6,3)),DifferentialStatus64.BudgetExceeded); }
    [TestMethod] public void DerivativeAtNaNIsInvalid()
    { DerivativeRefused(D(at:double.NaN),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void DerivativeNullCallbackIsInvalid()
    { DerivativeRefused(BoundedDifferentialAnalysis64.Differentiate(null,2),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void DerivativeAtUnrepresentableLargeCoordinateRefuses()
    { DerivativeRefused(D(at:1e20),DifferentialStatus64.StepNotRepresentable); }
    [TestMethod] public void DerivativeUltraTinyStepRefuses()
    { DerivativeRefused(D(policy:new(double.Epsilon,1e-6,4)),DifferentialStatus64.StepNotRepresentable); }
    [TestMethod] public void DerivativeNonfiniteCallbackRefuses()
    { DerivativeRefused(D(_=>double.NaN),DifferentialStatus64.NonFiniteEvaluation); }
    [TestMethod] public void DerivativeCallbackExceptionRefuses()
    { DerivativeRefused(D(_=>throw new ArithmeticException("test")),DifferentialStatus64.EvaluationFailed); }
    [TestMethod] public void DerivativeCallbackCancellationRefuses()
    { DerivativeRefused(D(_=>throw new OperationCanceledException()),DifferentialStatus64.Cancelled); }
    [TestMethod] public void DerivativePreCancellationRefusesWithoutCallingUser()
    { using var c=new CancellationTokenSource();c.Cancel();var calls=0;var r=BoundedDifferentialAnalysis64.Differentiate(x=>{calls++;return x*x;},2,cancellationToken:c.Token);DerivativeRefused(r,DifferentialStatus64.Cancelled);Assert.AreEqual(0,calls); }
    [TestMethod] public void DerivativeOverflowDifferenceRefuses()
    { DerivativeRefused(D(x=>x>2?double.MaxValue:-double.MaxValue),DifferentialStatus64.NonFiniteIntermediate); }
    [TestMethod] public void DerivativeErrorEstimateBudgetRefuses()
    { DerivativeRefused(D(x=>x*x*x,2,new(1,1e-8,4)),DifferentialStatus64.EstimateNotMet); }

    [TestMethod] public void GrowthOdeAtOneApproximatesEulerNumber()
    { var o=O();Assert.IsTrue(o.Succeeded);Near(o.FinalValue,global::System.Math.E,1e-7); }
    [TestMethod] public void DecayOdeAtOneApproximatesInverseEulerNumber()
    { var o=O((t,y)=>-y);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,1/global::System.Math.E,1e-7); }
    [TestMethod] public void ConstantOdeDerivativeRetainsInitialValue()
    { var o=O((t,y)=>0,0,1,7);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,7); }
    [TestMethod] public void ConstantVelocityOdeIntegratesLinearly()
    { var o=O((t,y)=>3,0,2,0);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,6); }
    [TestMethod] public void TimeDependentOdeIntegratesTwoTimeToOne()
    { var o=O((t,y)=>2*t,0,1,0);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,1); }
    [TestMethod] public void OdeDefaultReports64FineSteps()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.AreEqual(64,o.Steps); }
    [TestMethod] public void OdeDefaultReports384Evaluations()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.AreEqual(384,o.FunctionEvaluations); }
    [TestMethod] public void OdeCallbackCountMatchesReportedEvaluationCount()
    { var count=0;var o=O((t,y)=>{count++;return y;});Assert.IsTrue(o.Succeeded);Assert.AreEqual(count,o.FunctionEvaluations); }
    [TestMethod] public void OdeEstimatedGlobalErrorIsHeuristic()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.IsTrue(o.EstimatedAbsoluteError>=0&&o.EstimatedAbsoluteError<=1e-6);Assert.IsFalse(o.GlobalErrorBoundGuaranteed); }
    [TestMethod] public void OdeRuleIsObservedAtRuntime()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.AreEqual(BoundedDifferentialAnalysis64.RuleId,o.RuleId); }
    [TestMethod] public void OdeReversedTimeRefuses()
    { OdeRefused(O(start:2,end:1),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void OdeEqualTimeRefuses()
    { OdeRefused(O(start:1,end:1),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void OdeInfiniteInitialValueRefuses()
    { OdeRefused(O(initial:double.PositiveInfinity),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void OdeNullCallbackRefuses()
    { OdeRefused(BoundedDifferentialAnalysis64.IntegrateOde(null,0,1,1),DifferentialStatus64.InvalidInput); }
    [TestMethod] public void OdeInvalidZeroStepCountRefuses()
    { OdeRefused(O(policy:new(0,1e-6,1024)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void OdeExcessiveStepCountRefuses()
    { OdeRefused(O(policy:new(257,1e-6,4096)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void OdeInvalidZeroEstimatedErrorBudgetRefuses()
    { OdeRefused(O(policy:new(32,0,1024)),DifferentialStatus64.InvalidPolicy); }
    [TestMethod] public void OdeInsufficientEvaluationBudgetRefuses()
    { OdeRefused(O(policy:new(32,1e-6,383)),DifferentialStatus64.BudgetExceeded); }
    [TestMethod] public void OdeVeryStrictErrorEstimateRefuses()
    { OdeRefused(O(policy:new(2,1e-18,1024)),DifferentialStatus64.EstimateNotMet); }
    [TestMethod] public void OdeNonfiniteCallbackRefuses()
    { OdeRefused(O((t,y)=>double.NaN),DifferentialStatus64.NonFiniteEvaluation); }
    [TestMethod] public void OdeThrownCallbackRefuses()
    { OdeRefused(O((t,y)=>throw new InvalidOperationException("test")),DifferentialStatus64.EvaluationFailed); }
    [TestMethod] public void OdeCancellationFromCallbackRefuses()
    { OdeRefused(O((t,y)=>throw new OperationCanceledException()),DifferentialStatus64.Cancelled); }
    [TestMethod] public void OdePreCancelledRequestNeverCallsFunction()
    { using var c=new CancellationTokenSource();c.Cancel();var count=0;var o=BoundedDifferentialAnalysis64.IntegrateOde((t,y)=>{count++;return y;},0,1,1,cancellationToken:c.Token);OdeRefused(o,DifferentialStatus64.Cancelled);Assert.AreEqual(0,count); }
    [TestMethod] public void OdeOverlargeTimeIntervalRefuses()
    { OdeRefused(O(start:-double.MaxValue,end:double.MaxValue),DifferentialStatus64.NonFiniteIntermediate); }
    [TestMethod] public void OdeNonrepresentableTimeStepRefuses()
    { OdeRefused(O(start:1e16,end:1e16+4,initial:1),DifferentialStatus64.StepNotRepresentable); }
    [TestMethod] public void OdeOverflowIntermediateRefuses()
    { OdeRefused(O((t,y)=>double.MaxValue,initial:double.MaxValue),DifferentialStatus64.NonFiniteIntermediate); }
    [TestMethod] public void OdeTestHasNoUnqualifiedExternalSolver()
    { var deps=typeof(BoundedDifferentialAnalysis64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(deps.Any(x=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}.Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase)))); }
    [TestMethod] public void RepeatDeterministicCalculationsAgree()
    { Assert.AreEqual(D(),D());Assert.AreEqual(O(),O()); }
    [TestMethod] public void OriginalUnivariateNumericalMethodStillWorks()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x*x-2,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,global::System.Math.Sqrt(2)); }
    [TestMethod] public void OriginalCurveInterpolationStillWorks()
    { var r=BoundedCurveAnalysis64.InterpolateLinear([new(0,1),new(1,3)],0.5);Assert.IsTrue(r.Succeeded);Near(r.Value,2); }
}
