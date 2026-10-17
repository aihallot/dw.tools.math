using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C02T2Tests
{
    private static readonly Func<double,double> Polynomial = x => x*x-2d;
    private static readonly Func<double,double> Square = x => x*x;
    private static AuditedRoot64 Root(Func<double,double>? f = null, RootPolicy64? p = null) =>
        UnivariateBoundaryAudit64.AuditRoot(f ?? Polynomial,1d,2d,p);
    private static AuditedQuadrature64 Integrate(Func<double,double>? f = null,
        QuadraturePolicy64? p = null, QuadratureRegularity64 regularity = QuadratureRegularity64.CallerAssumesSmooth) =>
        UnivariateBoundaryAudit64.AuditQuadrature(f ?? Square,0d,1d,regularity,p);
    private static void Near(double? observed,double expected,double tolerance=1e-8)
    {
        Assert.IsNotNull(observed);
        Assert.IsTrue(double.IsFinite(observed.Value));
        Assert.IsTrue(global::System.Math.Abs(observed.Value-expected)<=tolerance,
            $"Expected {expected:R}, observed {observed.Value:R}.");
    }
    private static void RefusedRoot(AuditedRoot64 value,UnivariateAuditStatus64 status)
    {
        Assert.AreEqual(status,value.Status);
        Assert.IsFalse(value.Succeeded);
        Assert.IsNull(value.Root);
        Assert.IsNull(value.AbsoluteResidual);
        Assert.IsNull(value.ObservedBracketWidth);
        Assert.AreEqual(0,value.Iterations);
        Assert.AreEqual(0,value.FunctionEvaluations);
        Assert.IsFalse(value.ContinuityProven);
        Assert.IsFalse(value.ErrorBoundGuaranteed);
    }
    private static void RefusedQuadrature(AuditedQuadrature64 value,UnivariateAuditStatus64 status)
    {
        Assert.AreEqual(status,value.Status);
        Assert.IsFalse(value.Succeeded);
        Assert.IsNull(value.Integral);
        Assert.IsNull(value.EstimatedAbsoluteError);
        Assert.AreEqual(0,value.Panels);
        Assert.AreEqual(0,value.FunctionEvaluations);
        Assert.IsFalse(value.SmoothnessAssumedByCaller);
        Assert.IsFalse(value.ErrorBoundGuaranteed);
    }

    [TestMethod] public void SmoothRootIsAdmitted()
    { var r=Root();Assert.IsTrue(r.Succeeded);Near(r.Root,global::System.Math.Sqrt(2),1e-8); }
    [TestMethod] public void SmoothRootResidualMeetsExplicitTolerance()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.AbsoluteResidual<=1e-10); }
    [TestMethod] public void SmoothRootDoesNotProveContinuity()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ContinuityProven); }
    [TestMethod] public void SmoothRootDoesNotClaimGuaranteedError()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ErrorBoundGuaranteed); }
    [TestMethod] public void SmoothRootKeepsObservedBracketWidth()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.ObservedBracketWidth>0&&r.ObservedBracketWidth<=1); }
    [TestMethod] public void SmoothRootKeepsObservedFunctionCount()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.AreEqual(r.Iterations+2,r.FunctionEvaluations); }
    [TestMethod] public void ExactLowerEndpointRootIsAdmitted()
    { var r=UnivariateBoundaryAudit64.AuditRoot(x=>x-1,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,1); }
    [TestMethod] public void ExactUpperEndpointRootIsAdmitted()
    { var r=UnivariateBoundaryAudit64.AuditRoot(x=>x-2,1,2);Assert.IsTrue(r.Succeeded);Near(r.Root,2); }
    [TestMethod] public void FallingLinearRootIsAdmitted()
    { var r=UnivariateBoundaryAudit64.AuditRoot(x=>2-x,1,3);Assert.IsTrue(r.Succeeded);Near(r.Root,2); }
    [TestMethod] public void RootOutsideSignBracketIsNonpartialRefusal()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(x=>x*x+1,1,2),UnivariateAuditStatus64.MissingBracket); }
    [TestMethod] public void SignedJumpFailsIndependentResidualGate()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(x=>x<0?-1e200:1e200,-1,1),UnivariateAuditStatus64.ResidualNotMet); }
    [TestMethod] public void NarrowBracketIsNotSufficientIfResidualIsLarge()
    { RefusedRoot(Root(p:new(0.1,1e-20,128)),UnivariateAuditStatus64.ResidualNotMet); }
    [TestMethod] public void LegacyBisectionRetainsItsUncertifiedWidthSuccess()
    { var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x<0?-1e200:1e200,-1,1);Assert.IsTrue(r.Succeeded);Assert.IsTrue(r.AbsoluteResidual>1e199);Assert.IsFalse(r.GuaranteedRootError); }
    [TestMethod] public void InvalidRootPolicyIsTypedRefusal()
    { RefusedRoot(Root(p:new(0,1e-10,128)),UnivariateAuditStatus64.InvalidPolicy); }
    [TestMethod] public void ExcessiveRootIterationLimitIsRefused()
    { RefusedRoot(Root(p:new(1e-10,1e-10,257)),UnivariateAuditStatus64.InvalidPolicy); }
    [TestMethod] public void InsufficientIterationsAreNonconvergence()
    { RefusedRoot(Root(p:new(1e-15,1e-15,1)),UnivariateAuditStatus64.NonConverged); }
    [TestMethod] public void NullRootFunctionRefuses()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(null,1,2),UnivariateAuditStatus64.InvalidInput); }
    [TestMethod] public void ReversedRootIntervalRefuses()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(Polynomial,2,1),UnivariateAuditStatus64.InvalidInput); }
    [TestMethod] public void RootNaNEvaluationRefuses()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(_=>double.NaN,1,2),UnivariateAuditStatus64.NonFiniteEvaluation); }
    [TestMethod] public void RootCallbackFailureRefuses()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(_=>throw new ArithmeticException("test"),1,2),UnivariateAuditStatus64.EvaluationFailed); }
    [TestMethod] public void RootCancellationRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(Polynomial,1,2,cancellationToken:c.Token),UnivariateAuditStatus64.Cancelled); }
    [TestMethod] public void RootIntervalOverflowRefuses()
    { RefusedRoot(UnivariateBoundaryAudit64.AuditRoot(x=>x,-double.MaxValue,double.MaxValue),UnivariateAuditStatus64.NonFiniteIntermediate); }
    [TestMethod] public void RootUnknownErrorGuaranteeNeverChangesWithTolerance()
    { var r=Root(p:new(1e-12,1e-8,128));Assert.IsTrue(r.Succeeded);Assert.IsFalse(r.ErrorBoundGuaranteed); }
    [TestMethod] public void RootResultContainsObservedRuleId()
    { var r=Root();Assert.IsTrue(r.Succeeded);Assert.AreEqual(UnivariateBoundaryAudit64.RuleId,r.RuleId); }

    [TestMethod] public void QuadratureWithoutRegularityAssumptionRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(Square,0,1),UnivariateAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void DiscontinuousStepWithoutRegularityAssumptionRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(x=>x<0.5?0:1,0,1),UnivariateAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void PointSpikeWithoutRegularityAssumptionRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(x=>x==0.5?1000:0,0,1),UnivariateAuditStatus64.RegularityUnspecified); }
    [TestMethod] public void InvalidRegularityEnumRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(Square,0,1,(QuadratureRegularity64)99),UnivariateAuditStatus64.InvalidPolicy); }
    [TestMethod] public void CallerDeclaredSmoothSquareIntegrates()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Near(q.Integral,1d/3d,1e-9); }
    [TestMethod] public void CallerDeclaredSmoothnessRecordedNotVerified()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.SmoothnessAssumedByCaller);Assert.IsFalse(q.ErrorBoundGuaranteed); }
    [TestMethod] public void SmoothSquareErrorEstimateIsObserved()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.EstimatedAbsoluteError>=0&&q.EstimatedAbsoluteError<=1e-8); }
    [TestMethod] public void SmoothSineIntegralIsTwo()
    { var q=UnivariateBoundaryAudit64.AuditQuadrature(global::System.Math.Sin,0,global::System.Math.PI,QuadratureRegularity64.CallerAssumesSmooth,new(1e-6,128,512));Assert.IsTrue(q.Succeeded);Near(q.Integral,2,1e-5); }
    [TestMethod] public void LinearFunctionIntegratesExactly()
    { var q=Integrate(x=>x);Assert.IsTrue(q.Succeeded);Near(q.Integral,0.5); }
    [TestMethod] public void ConstantFunctionIntegratesExactly()
    { var q=Integrate(_=>4);Assert.IsTrue(q.Succeeded);Near(q.Integral,4); }
    [TestMethod] public void FinitePanelCountIsReported()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.Panels>=4&&q.Panels<=128); }
    [TestMethod] public void FiniteCallbackCountIsReported()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Assert.IsTrue(q.FunctionEvaluations>=8&&q.FunctionEvaluations<=1025); }
    [TestMethod] public void StrongToleranceWithTinyBudgetRefuses()
    { RefusedQuadrature(Integrate(global::System.Math.Exp,new(1e-20,128,8)),UnivariateAuditStatus64.BudgetExceeded); }
    [TestMethod] public void StrongToleranceWithFewPanelsRefuses()
    { RefusedQuadrature(Integrate(global::System.Math.Exp,new(1e-20,4,1025)),UnivariateAuditStatus64.NonConverged); }
    [TestMethod] public void InvalidPanelCountRefuses()
    { RefusedQuadrature(Integrate(p:new(1e-8,7,1025)),UnivariateAuditStatus64.InvalidPolicy); }
    [TestMethod] public void InvalidAbsoluteErrorToleranceRefuses()
    { RefusedQuadrature(Integrate(p:new(double.NaN,128,1025)),UnivariateAuditStatus64.InvalidPolicy); }
    [TestMethod] public void NullQuadratureFunctionRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(null,0,1,QuadratureRegularity64.CallerAssumesSmooth),UnivariateAuditStatus64.InvalidInput); }
    [TestMethod] public void QuadratureNaNEvaluationRefuses()
    { RefusedQuadrature(Integrate(_=>double.NaN),UnivariateAuditStatus64.NonFiniteEvaluation); }
    [TestMethod] public void QuadratureThrownCallbackRefuses()
    { RefusedQuadrature(Integrate(_=>throw new ArithmeticException("test")),UnivariateAuditStatus64.EvaluationFailed); }
    [TestMethod] public void QuadratureCancellationRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(Square,0,1,QuadratureRegularity64.CallerAssumesSmooth,cancellationToken:c.Token),UnivariateAuditStatus64.Cancelled); }
    [TestMethod] public void QuadratureOverflowIntervalRefuses()
    { RefusedQuadrature(UnivariateBoundaryAudit64.AuditQuadrature(Square,-double.MaxValue,double.MaxValue,QuadratureRegularity64.CallerAssumesSmooth),UnivariateAuditStatus64.NonFiniteIntermediate); }
    [TestMethod] public void QuadratureObservedRuleIsVersioned()
    { var q=Integrate();Assert.IsTrue(q.Succeeded);Assert.AreEqual(UnivariateBoundaryAudit64.RuleId,q.RuleId); }
    [TestMethod] public void PriorQuadratureRemainsUsableWithoutNewAuditPolicy()
    { var q=BoundedUnivariateAnalysis64.IntegrateSimpson(Square,0,1);Assert.IsTrue(q.Succeeded);Near(q.Integral,1d/3d); }
    [TestMethod] public void PriorCurveBoundaryContractStillAdmitsUnits()
    { var c=CurveBoundaryAudit64.EvaluateLinear([new(0,1),new(1,3)],0.5,new("s","m"),"s");Assert.IsTrue(c.Admitted);Near(c.Value,2); }
    [TestMethod] public void AuditResultsAreDeterministicForDeterministicCallbacks()
    { Assert.AreEqual(Root(),Root());Assert.AreEqual(Integrate(),Integrate()); }
    [TestMethod] public void AuditHasNoForeignBrokerOrProviderDependency()
    { var deps=typeof(UnivariateBoundaryAudit64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(deps.Any(d=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}.Any(t=>d.Contains(t,StringComparison.OrdinalIgnoreCase)))); }
}
