using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C03T2Tests
{
    private static readonly Func<double,double> Square=x=>x*x;
    private static readonly Func<double,double,double> Growth=(t,y)=>y;
    private static AuditedDerivative64 D(Func<double,double>? function=null,
        double at=2d, DerivativeRegularity64 regularity=DerivativeRegularity64.CallerAssumesSmooth,
        DerivativePolicy64? policy=null) =>
        DifferentialBoundaryAudit64.Differentiate(function??Square,at,regularity,policy);
    private static AuditedOde64 O(Func<double,double,double>? function=null,
        double start=0d,double end=1d,double initial=1d,
        OdeRegime64 regime=OdeRegime64.CallerAssumesNonStiffNoEvents,
        OdePolicy64? policy=null) =>
        DifferentialBoundaryAudit64.IntegrateOde(function??Growth,start,end,initial,regime,policy);
    private static void Near(double? value,double expected,double tolerance=1e-7)
    {
        Assert.IsNotNull(value);
        Assert.IsTrue(double.IsFinite(value.Value));
        Assert.IsTrue(global::System.Math.Abs(value.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {value.Value:R}");
    }
    private static void RefuseDerivative(AuditedDerivative64 result,DifferentialAuditStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Value);
        Assert.IsNull(result.EstimatedAbsoluteError);
        Assert.IsNull(result.UsedStep);
        Assert.AreEqual(0,result.FunctionEvaluations);
        Assert.IsFalse(result.SmoothnessIndependentlyVerified);
        Assert.IsFalse(result.LocalErrorBoundGuaranteed);
    }
    private static void RefuseOde(AuditedOde64 result,DifferentialAuditStatus64 expected)
    {
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.FinalValue);
        Assert.IsNull(result.EstimatedAbsoluteError);
        Assert.AreEqual(0,result.Steps);
        Assert.AreEqual(0,result.FunctionEvaluations);
        Assert.IsFalse(result.NonStiffRegimeIndependentlyVerified);
        Assert.IsFalse(result.EventAbsenceIndependentlyVerified);
        Assert.IsFalse(result.GlobalErrorBoundGuaranteed);
    }

    [TestMethod] public void DerivativeDefaultRequiresExplicitRegularity()
    { RefuseDerivative(DifferentialBoundaryAudit64.Differentiate(Square,2),DifferentialAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void CuspWithoutCallerSmoothnessRefuses()
    { RefuseDerivative(DifferentialBoundaryAudit64.Differentiate(global::System.Math.Abs,0),DifferentialAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void JumpWithoutCallerSmoothnessRefuses()
    { RefuseDerivative(DifferentialBoundaryAudit64.Differentiate(x=>x<0?0:1,0),DifferentialAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void UnknownDerivativeRegularityIsInvalidPolicy()
    { RefuseDerivative(D(regularity:(DerivativeRegularity64)99),DifferentialAuditStatus64.InvalidPolicy); }
    [TestMethod] public void AssumedSmoothSquareAtTwoDerivativeFour()
    { var d=D();Assert.IsTrue(d.Succeeded);Near(d.Value,4,1e-9); }
    [TestMethod] public void AssumedSmoothSquareAtMinusTwoDerivativeMinusFour()
    { var d=D(at:-2);Assert.IsTrue(d.Succeeded);Near(d.Value,-4,1e-9); }
    [TestMethod] public void AssumedSmoothSineAtZeroDerivativeOne()
    { var d=D(global::System.Math.Sin,0);Assert.IsTrue(d.Succeeded);Near(d.Value,1,1e-6); }
    [TestMethod] public void AssumedSmoothExponentialAtZeroDerivativeOne()
    { var d=D(global::System.Math.Exp,0);Assert.IsTrue(d.Succeeded);Near(d.Value,1,1e-6); }
    [TestMethod] public void AssumedSmoothConstantDerivativeZero()
    { var d=D(_=>7);Assert.IsTrue(d.Succeeded);Near(d.Value,0); }
    [TestMethod] public void AssumedSmoothAffineDerivativeThree()
    { var d=D(x=>3*x+1);Assert.IsTrue(d.Succeeded);Near(d.Value,3); }
    [TestMethod] public void CallerSmoothnessIsNotIndependentlyVerified()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.IsFalse(d.SmoothnessIndependentlyVerified); }
    [TestMethod] public void LocalDerivativeErrorBoundIsNotProven()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.IsFalse(d.LocalErrorBoundGuaranteed); }
    [TestMethod] public void DerivativeExposesFiniteHeuristicEstimate()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.IsTrue(d.EstimatedAbsoluteError>=0&&d.EstimatedAbsoluteError<=1e-6); }
    [TestMethod] public void DerivativeExposesRefinedHalfStep()
    { var d=D();Assert.IsTrue(d.Succeeded);Near(d.UsedStep,0.0005,1e-12); }
    [TestMethod] public void DerivativeExposesActualEvaluationCount()
    { var count=0;var d=D(x=>{count++;return x*x;});Assert.IsTrue(d.Succeeded);Assert.AreEqual(count,d.FunctionEvaluations);Assert.AreEqual(4,count); }
    [TestMethod] public void FalseCallerSmoothnessAssertionDoesNotBecomeProof()
    { var d=D(global::System.Math.Abs,0);Assert.IsTrue(d.Succeeded);Near(d.Value,0);Assert.IsFalse(d.SmoothnessIndependentlyVerified);Assert.IsFalse(d.LocalErrorBoundGuaranteed); }
    [TestMethod] public void ExistingCuspOperationIsUnchanged()
    { var old=BoundedDifferentialAnalysis64.Differentiate(global::System.Math.Abs,0);Assert.IsTrue(old.Succeeded);Near(old.Value,0);Assert.IsFalse(old.ErrorBoundGuaranteed); }
    [TestMethod] public void DerivativeLargeCoordinateRefusesUnrepresentableStep()
    { RefuseDerivative(D(at:1e20),DifferentialAuditStatus64.StepNotRepresentable); }
    [TestMethod] public void DerivativeTinyStepRefusesUnrepresentableStep()
    { RefuseDerivative(D(policy:new(double.Epsilon,1e-6,4)),DifferentialAuditStatus64.StepNotRepresentable); }
    [TestMethod] public void DerivativeZeroStepIsInvalidPolicy()
    { RefuseDerivative(D(policy:new(0,1e-6,4)),DifferentialAuditStatus64.InvalidPolicy); }
    [TestMethod] public void DerivativeInsufficientBudgetRefuses()
    { RefuseDerivative(D(policy:new(1e-3,1e-6,3)),DifferentialAuditStatus64.BudgetExceeded); }
    [TestMethod] public void DerivativeNonfiniteEvaluationRefusesWithoutPartial()
    { RefuseDerivative(D(_=>double.NaN),DifferentialAuditStatus64.NonFiniteEvaluation); }
    [TestMethod] public void DerivativeThrownCallbackIsTypedRefusal()
    { RefuseDerivative(D(_=>throw new ArithmeticException("callback")),DifferentialAuditStatus64.EvaluationFailed); }
    [TestMethod] public void DerivativeCancellationPreemptsCallback()
    { using var tokenSource=new CancellationTokenSource();tokenSource.Cancel();var count=0;var r=DifferentialBoundaryAudit64.Differentiate(x=>{count++;return x*x;},2,DerivativeRegularity64.CallerAssumesSmooth,cancellationToken:tokenSource.Token);RefuseDerivative(r,DifferentialAuditStatus64.Cancelled);Assert.AreEqual(0,count); }
    [TestMethod] public void DerivativeCallbackCancellationIsTyped()
    { RefuseDerivative(D(_=>throw new OperationCanceledException()),DifferentialAuditStatus64.Cancelled); }
    [TestMethod] public void DerivativeOverflowDifferenceIsTyped()
    { RefuseDerivative(D(x=>x>2?double.MaxValue:-double.MaxValue),DifferentialAuditStatus64.NonFiniteIntermediate); }
    [TestMethod] public void DerivativeUnmetEstimateRefusesWithoutPartial()
    { RefuseDerivative(D(x=>x*x*x,2,policy:new(1,1e-8,4)),DifferentialAuditStatus64.EstimateNotMet); }
    [TestMethod] public void DerivativeNullFunctionIsInvalidInput()
    { RefuseDerivative(DifferentialBoundaryAudit64.Differentiate(null,2,DerivativeRegularity64.CallerAssumesSmooth),DifferentialAuditStatus64.InvalidInput); }
    [TestMethod] public void DerivativeNaNPointIsInvalidInput()
    { RefuseDerivative(D(at:double.NaN),DifferentialAuditStatus64.InvalidInput); }
    [TestMethod] public void DerivativeRuleIdIsObservedFromRuntime()
    { var d=D();Assert.IsTrue(d.Succeeded);Assert.AreEqual(DifferentialBoundaryAudit64.RuleId,d.RuleId); }

    [TestMethod] public void OdeDefaultRequiresExplicitRegime()
    { RefuseOde(DifferentialBoundaryAudit64.IntegrateOde(Growth,0,1,1),DifferentialAuditStatus64.OdeRegimeUnspecified); }
    [TestMethod] public void KnownStiffOdeRegimeIsOutOfScope()
    { RefuseOde(O(regime:OdeRegime64.KnownStiff),DifferentialAuditStatus64.UnsupportedRegime); }
    [TestMethod] public void EventDrivenOdeRegimeIsOutOfScope()
    { RefuseOde(O(regime:OdeRegime64.EventDriven),DifferentialAuditStatus64.UnsupportedRegime); }
    [TestMethod] public void PdeRegimeIsOutOfScope()
    { RefuseOde(O(regime:OdeRegime64.PartialDifferentialEquation),DifferentialAuditStatus64.UnsupportedRegime); }
    [TestMethod] public void UnknownOdeEnumIsInvalidPolicy()
    { RefuseOde(O(regime:(OdeRegime64)99),DifferentialAuditStatus64.InvalidPolicy); }
    [TestMethod] public void AssumedNonstiffGrowthApproximatesEulerNumber()
    { var o=O();Assert.IsTrue(o.Succeeded);Near(o.FinalValue,global::System.Math.E,1e-7); }
    [TestMethod] public void AssumedNonstiffDecayApproximatesInverseEuler()
    { var o=O((t,y)=>-y);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,1/global::System.Math.E,1e-7); }
    [TestMethod] public void AssumedNonstiffConstantStateIsPreserved()
    { var o=O((t,y)=>0,initial:7);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,7); }
    [TestMethod] public void AssumedNonstiffConstantVelocityIsIntegrated()
    { var o=O((t,y)=>3,end:2,initial:0);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,6); }
    [TestMethod] public void AssumedNonstiffTimeDependentRateIsIntegrated()
    { var o=O((t,y)=>2*t,initial:0);Assert.IsTrue(o.Succeeded);Near(o.FinalValue,1); }
    [TestMethod] public void ODEDeclaredNonstiffnessIsNotIndependentlyVerified()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.NonStiffRegimeIndependentlyVerified); }
    [TestMethod] public void OdeDeclaredEventAbsenceIsNotIndependentlyVerified()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.EventAbsenceIndependentlyVerified); }
    [TestMethod] public void OdeGlobalErrorIsNotCertified()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.GlobalErrorBoundGuaranteed); }
    [TestMethod] public void OdeReportedHeuristicEstimateIsFinite()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.IsTrue(o.EstimatedAbsoluteError>=0&&o.EstimatedAbsoluteError<=1e-6); }
    [TestMethod] public void OdeReportsFiniteFineStepCount()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.AreEqual(64,o.Steps); }
    [TestMethod] public void OdeReportsActualEvaluationCount()
    { var count=0;var o=O((t,y)=>{count++;return y;});Assert.IsTrue(o.Succeeded);Assert.AreEqual(count,o.FunctionEvaluations);Assert.AreEqual(384,count); }
    [TestMethod] public void OdeReversedTimeIsInvalidInput()
    { RefuseOde(O(start:2,end:1),DifferentialAuditStatus64.InvalidInput); }
    [TestMethod] public void OdeZeroWidthTimeIsInvalidInput()
    { RefuseOde(O(start:1,end:1),DifferentialAuditStatus64.InvalidInput); }
    [TestMethod] public void OdeNullFunctionIsInvalidInput()
    { RefuseOde(DifferentialBoundaryAudit64.IntegrateOde(null,0,1,1,OdeRegime64.CallerAssumesNonStiffNoEvents),DifferentialAuditStatus64.InvalidInput); }
    [TestMethod] public void OdeInvalidZeroStepBudgetRefuses()
    { RefuseOde(O(policy:new(0,1e-6,1024)),DifferentialAuditStatus64.InvalidPolicy); }
    [TestMethod] public void OdeExcessiveStepCountRefuses()
    { RefuseOde(O(policy:new(257,1e-6,4096)),DifferentialAuditStatus64.InvalidPolicy); }
    [TestMethod] public void OdeInsufficientCallbackBudgetRefuses()
    { RefuseOde(O(policy:new(32,1e-6,383)),DifferentialAuditStatus64.BudgetExceeded); }
    [TestMethod] public void OdeStrictHeuristicToleranceRefuses()
    { RefuseOde(O(policy:new(2,1e-18,1024)),DifferentialAuditStatus64.EstimateNotMet); }
    [TestMethod] public void OdeNonfiniteCallbackIsTyped()
    { RefuseOde(O((t,y)=>double.NaN),DifferentialAuditStatus64.NonFiniteEvaluation); }
    [TestMethod] public void OdeThrownCallbackIsTyped()
    { RefuseOde(O((t,y)=>throw new ArithmeticException("callback")),DifferentialAuditStatus64.EvaluationFailed); }
    [TestMethod] public void OdePrecancellationSkipsAllCallbacks()
    { using var tokenSource=new CancellationTokenSource();tokenSource.Cancel();var count=0;var result=DifferentialBoundaryAudit64.IntegrateOde((t,y)=>{count++;return y;},0,1,1,OdeRegime64.CallerAssumesNonStiffNoEvents,cancellationToken:tokenSource.Token);RefuseOde(result,DifferentialAuditStatus64.Cancelled);Assert.AreEqual(0,count); }
    [TestMethod] public void OdeCallbackCancellationIsTyped()
    { RefuseOde(O((t,y)=>throw new OperationCanceledException()),DifferentialAuditStatus64.Cancelled); }
    [TestMethod] public void OdeOverwideTimeIntervalRefusesWithoutPartial()
    { RefuseOde(O(start:-double.MaxValue,end:double.MaxValue),DifferentialAuditStatus64.NonFiniteIntermediate); }
    [TestMethod] public void OdeNonrepresentableTimeStepRefuses()
    { RefuseOde(O(start:1e16,end:1e16+4),DifferentialAuditStatus64.StepNotRepresentable); }
    [TestMethod] public void OdeIntermediateOverflowRefusesWithoutPartial()
    { RefuseOde(O((t,y)=>double.MaxValue,initial:double.MaxValue),DifferentialAuditStatus64.NonFiniteIntermediate); }
    [TestMethod] public void OdeRuleIdIsObservedAtRuntime()
    { var o=O();Assert.IsTrue(o.Succeeded);Assert.AreEqual(DifferentialBoundaryAudit64.RuleId,o.RuleId); }
    [TestMethod] public void PreviouslyQualifiedRk4MethodIsUnchanged()
    { var old=BoundedDifferentialAnalysis64.IntegrateOde(Growth,0,1,1);Assert.IsTrue(old.Succeeded);Near(old.FinalValue,global::System.Math.E,1e-7); }
    [TestMethod] public void RepeatedDeterministicAuditRecordsMatch()
    { Assert.AreEqual(D(),D());Assert.AreEqual(O(),O()); }
    [TestMethod] public void PriorUnivariateRootAndQuadratureRemainsCallable()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x*x-2,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,global::System.Math.Sqrt(2)); }
    [TestMethod] public void PriorCurveAnalysisRemainsCallable()
    { var r=BoundedCurveAnalysis64.InterpolateLinear([new(0,1),new(1,3)],0.5);Assert.IsTrue(r.Succeeded);Near(r.Value,2); }
    [TestMethod] public void AuditHasNoAgentOrForeignProviderDependency()
    { var refs=typeof(DifferentialBoundaryAudit64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(refs.Any(x=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}.Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase)))); }
}
