using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C04T2Tests
{
    private static readonly DateOnly Start = new(2025, 1, 1);
    private const FinancialDayCountConvention64 Y365 =
        FinancialDayCountConvention64.Actual365Fixed;
    private const FinancialDayCountConvention64 Y360 =
        FinancialDayCountConvention64.Actual360;

    private static DateOnly[] Dates(params int[] elapsed) =>
        elapsed.Select(days => Start.AddDays(days)).ToArray();

    private static void Near(
        FinancialDatedOutcome64 observed, double expected,
        int elapsed, FinancialDayCountConvention64 convention,
        double tolerance=1e-10d)
    {
        Assert.IsTrue(observed.Succeeded);
        Assert.AreEqual(FinancialDatedStatus64.Success, observed.Status);
        Assert.IsNotNull(observed.Value);
        Assert.AreEqual(elapsed, observed.ElapsedDays);
        Assert.AreEqual(convention, observed.DayCount);
        var delta=global::System.Math.Abs(observed.Value.Value-expected);
        Assert.IsTrue(delta<=tolerance*
            global::System.Math.Max(1d,global::System.Math.Abs(expected)),
            $"Expected {expected:R} but got {observed.Value.Value:R} (delta {delta:R})");
        Assert.AreEqual(M6W02C04Boundary64.RuleId,observed.RuleId);
        Assert.IsFalse(observed.ErrorBoundGuaranteed);
    }

    private static void Refused(
        FinancialDatedOutcome64 outcome, FinancialDatedStatus64 expected)
    {
        Assert.AreEqual(expected,outcome.Status);
        Assert.IsFalse(outcome.Succeeded);
        Assert.IsNull(outcome.Value);
        Assert.IsNull(outcome.ElapsedDays);
        Assert.AreEqual(FinancialDayCountConvention64.Unspecified,outcome.DayCount);
        Assert.IsFalse(outcome.ErrorBoundGuaranteed);
    }

    [TestMethod] public void Discount365Canonical()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),Y365),1000d/11d,365,Y365); }

    [TestMethod] public void Discount360Canonical()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(360),Y360),1000d/11d,360,Y360); }

    [TestMethod] public void DiscountLeapActual365Fixed()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,new DateOnly(2024,1,1),new DateOnly(2025,1,1),Y365),100d/global::System.Math.Pow(1.1d,366d/365d),366,Y365); }

    [TestMethod] public void DiscountLeapActual360()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,new DateOnly(2024,1,1),new DateOnly(2025,1,1),Y360),100d/global::System.Math.Pow(1.1d,366d/360d),366,Y360); }

    [TestMethod] public void DiscountSame365DaysActual360()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),Y360),100d/global::System.Math.Pow(1.1d,365d/360d),365,Y360); }

    [TestMethod] public void DiscountConventionsAreDifferent()
    { var a=M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),Y365);var b=M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),Y360);Assert.IsTrue(a.Succeeded&&b.Succeeded);Assert.IsTrue(a.Value!.Value>b.Value!.Value); }

    [TestMethod] public void DiscountTwoYears365()
    { Near(M6W02C04Boundary64.DiscountDated(121d,0.1d,Start,Start.AddDays(730),Y365),100d,730,Y365); }

    [TestMethod] public void DiscountNegativeAmount()
    { Near(M6W02C04Boundary64.DiscountDated(-110d,0.1d,Start,Start.AddDays(365),Y365),-100d,365,Y365); }

    [TestMethod] public void DiscountZeroAmount()
    { Near(M6W02C04Boundary64.DiscountDated(0d,0.1d,Start,Start.AddDays(365),Y365),0d,365,Y365); }

    [TestMethod] public void DiscountSameDay()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start,Y365),100d,0,Y365); }

    [TestMethod] public void DiscountZeroRate()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0d,Start,Start.AddDays(365),Y365),100d,365,Y365); }

    [TestMethod] public void DiscountNegativeRate()
    { Near(M6W02C04Boundary64.DiscountDated(100d,-0.5d,Start,Start.AddDays(365),Y365),200d,365,Y365); }

    [TestMethod] public void DiscountMaximumRate()
    { Near(M6W02C04Boundary64.DiscountDated(100d,4d,Start,Start.AddDays(365),Y365),20d,365,Y365); }

    [TestMethod] public void DiscountMinimumRate()
    { Near(M6W02C04Boundary64.DiscountDated(100d,-0.9d,Start,Start.AddDays(365),Y365),1000d,365,Y365); }

    [TestMethod] public void DiscountMaximumCashflow()
    { Near(M6W02C04Boundary64.DiscountDated(1e6d,0d,Start,Start.AddDays(365),Y365),1e6d,365,Y365); }

    [TestMethod] public void DiscountFractionalDayCount()
    { Near(M6W02C04Boundary64.DiscountDated(100d,0.21d,Start,Start.AddDays(182),Y365),100d/global::System.Math.Pow(1.21d,182d/365d),182,Y365); }

    [TestMethod] public void DiscountActualDaysAreReturned()
    { var x=M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(5),Y365);Assert.IsTrue(x.Succeeded);Assert.AreEqual(5,x.ElapsedDays); }

    [TestMethod] public void DiscountReturnsRuntimeRuleId()
    { var x=M6W02C04Boundary64.DiscountDated(100d,0d,Start,Start,Y365);Assert.IsTrue(x.Succeeded);Assert.AreEqual(M6W02C04Boundary64.RuleId,x.RuleId); }

    [TestMethod] public void DiscountDoesNotCertifyErrorBound()
    { var x=M6W02C04Boundary64.DiscountDated(100d,0d,Start,Start,Y365);Assert.IsTrue(x.Succeeded);Assert.IsFalse(x.ErrorBoundGuaranteed); }

    [TestMethod] public void DiscountUnspecifiedDayCountRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),FinancialDayCountConvention64.Unspecified),FinancialDatedStatus64.DayCountRequired); }

    [TestMethod] public void DiscountUnknownDayCountRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),(FinancialDayCountConvention64)99),FinancialDatedStatus64.DayCountRequired); }

    [TestMethod] public void DiscountNaNAmountRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(double.NaN,0.1d,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidInput); }

    [TestMethod] public void DiscountInfinityAmountRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(double.NegativeInfinity,0.1d,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidInput); }

    [TestMethod] public void DiscountNaNRateRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,double.NaN,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidRate); }

    [TestMethod] public void DiscountInfiniteRateRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,double.PositiveInfinity,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidRate); }

    [TestMethod] public void DiscountRateBelowLimitRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,-0.91d,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidRate); }

    [TestMethod] public void DiscountRateAboveLimitRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,4.01d,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.InvalidRate); }

    [TestMethod] public void DiscountOversizedCashflowRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(1e6d+1d,0.1d,Start,Start.AddDays(365),Y365),FinancialDatedStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DiscountBackwardDatesRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(-1),Y365),FinancialDatedStatus64.InvalidDates); }

    [TestMethod] public void DiscountBeyondHundredYearsRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(36501),Y365),FinancialDatedStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DiscountLargePresentValueRefused()
    { Refused(M6W02C04Boundary64.DiscountDated(1e6d,-0.9d,Start,Start.AddDays(36500),Y365),FinancialDatedStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DiscountPrecancelled()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Boundary64.DiscountDated(100d,0.1d,Start,Start.AddDays(365),Y365,c.Token),FinancialDatedStatus64.Cancelled); }

    [TestMethod] public void DiscountPrecancelOverridesPolicy()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Boundary64.DiscountDated(double.NaN,double.NaN,Start,Start,(FinancialDayCountConvention64)99,c.Token),FinancialDatedStatus64.Cancelled); }

    [TestMethod] public void Irr365Canonical()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365),0.1d,365,Y365); }

    [TestMethod] public void Irr360Canonical()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,360),0d,0.5d,Y360),0.1d,360,Y360); }

    [TestMethod] public void IrrReversedSigns()
    { Near(M6W02C04Boundary64.SolveDatedIrr([100d,-110d],Dates(0,365),0d,0.5d,Y365),0.1d,365,Y365); }

    [TestMethod] public void IrrWideBracket()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),-0.9d,4d,Y365),0.1d,365,Y365); }

    [TestMethod] public void IrrNegativeRate()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,50d],Dates(0,365),-0.9d,0d,Y365),-0.5d,365,Y365); }

    [TestMethod] public void IrrZeroRate()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,100d],Dates(0,365),-0.5d,0.5d,Y365),0d,365,Y365); }

    [TestMethod] public void IrrZeroRateAtLowerBound()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,100d],Dates(0,365),0d,0.5d,Y365),0d,365,Y365); }

    [TestMethod] public void IrrUpperBoundFour()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,500d],Dates(0,365),0d,4d,Y365),4d,365,Y365); }

    [TestMethod] public void IrrZeroMiddleCashflow()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,0d,121d],Dates(0,365,730),0d,0.5d,Y365),0.1d,730,Y365); }

    [TestMethod] public void IrrLeadingZeroAccepted()
    { Near(M6W02C04Boundary64.SolveDatedIrr([0d,-100d,110d],Dates(0,365,730),0d,0.5d,Y365),0.1d,730,Y365); }

    [TestMethod] public void IrrTrailingZeroAccepted()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d,0d],Dates(0,365,730),0d,0.5d,Y365),0.1d,730,Y365); }

    [TestMethod] public void IrrTwoYearSpan()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,121d],Dates(0,730),0d,0.5d,Y365),0.1d,730,Y365); }

    [TestMethod] public void IrrThreePeriods()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,0d,0d,133.1d],Dates(0,365,730,1095),0d,0.5d,Y365),0.1d,1095,Y365); }

    [TestMethod] public void IrrThreeCashflows()
    { Near(M6W02C04Boundary64.SolveDatedIrr([-100d,55d,60.5d],Dates(0,365,730),0d,0.5d,Y365),0.1d,730,Y365); }

    [TestMethod] public void IrrAct360Vs365OfSameDates()
    { var a=M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365);var b=M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y360);Assert.IsTrue(a.Succeeded&&b.Succeeded);Assert.IsTrue(b.Value!.Value<a.Value!.Value); }

    [TestMethod] public void IrrAct360With365Days()
    { var expected=global::System.Math.Pow(1.1d,360d/365d)-1d;Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y360),expected,365,Y360); }

    [TestMethod] public void IrrLeapYearActualDays()
    { var dates=new[]{new DateOnly(2024,1,1),new DateOnly(2025,1,1)};var expected=global::System.Math.Pow(1.1d,365d/366d)-1d;Near(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],dates,0d,0.5d,Y365),expected,366,Y365); }

    [TestMethod] public void IrrNonUniformDatesExplicitlyAllowed()
    { var values=new[]{-100d,30d,90d};var dates=Dates(0,200,365);var root=M6W02C04Boundary64.SolveDatedIrr(values,dates,-0.5d,1d,Y365);Assert.IsTrue(root.Succeeded);Assert.IsNotNull(root.Value);var rate=root.Value.Value;var npv=-100d+30d/global::System.Math.Pow(1d+rate,200d/365d)+90d/(1d+rate);Assert.IsTrue(global::System.Math.Abs(npv)<1e-8d); }

    [TestMethod] public void IrrReturnsActualSpan()
    { var x=M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365);Assert.IsTrue(x.Succeeded);Assert.AreEqual(365,x.ElapsedDays); }

    [TestMethod] public void IrrRuntimeRuleId()
    { var x=M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365);Assert.IsTrue(x.Succeeded);Assert.AreEqual(M6W02C04Boundary64.RuleId,x.RuleId); }

    [TestMethod] public void IrrNoErrorTheorem()
    { var x=M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365);Assert.IsTrue(x.Succeeded);Assert.IsFalse(x.ErrorBoundGuaranteed); }

    [TestMethod] public void IrrUnspecifiedDayCount()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,FinancialDayCountConvention64.Unspecified),FinancialDatedStatus64.DayCountRequired); }

    [TestMethod] public void IrrUnknownDayCount()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,(FinancialDayCountConvention64)99),FinancialDatedStatus64.DayCountRequired); }

    [TestMethod] public void IrrNullCashflows()
    { Refused(M6W02C04Boundary64.SolveDatedIrr(null,Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.InvalidCashflows); }

    [TestMethod] public void IrrEmptyCashflows()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.InvalidCashflows); }

    [TestMethod] public void IrrOneCashflow()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d],Dates(0),0d,0.5d,Y365),FinancialDatedStatus64.InvalidCashflows); }

    [TestMethod] public void IrrSeventeenCashflows()
    { Refused(M6W02C04Boundary64.SolveDatedIrr(new double[17],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.InvalidCashflows); }

    [TestMethod] public void IrrMissingDates()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],null,0d,0.5d,Y365),FinancialDatedStatus64.MissingDates); }

    [TestMethod] public void IrrMismatchedDates()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0),0d,0.5d,Y365),FinancialDatedStatus64.InvalidDates); }

    [TestMethod] public void IrrDuplicateDates()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,0),0d,0.5d,Y365),FinancialDatedStatus64.InvalidDates); }

    [TestMethod] public void IrrDecreasingDates()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(365,0),0d,0.5d,Y365),FinancialDatedStatus64.InvalidDates); }

    [TestMethod] public void IrrDatesBeyondCentury()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,36501),0d,0.5d,Y365),FinancialDatedStatus64.OutsideQualifiedRange); }

    [TestMethod] public void IrrNaNFlow()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,double.NaN],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.InvalidInput); }

    [TestMethod] public void IrrInfinityFlow()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,double.PositiveInfinity],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.InvalidInput); }

    [TestMethod] public void IrrCashflowBudget()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,1e6d+1d],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.OutsideQualifiedRange); }

    [TestMethod] public void IrrNaNLowerRate()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),double.NaN,0.5d,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrInfiniteUpperRate()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,double.PositiveInfinity,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrEqualRateBounds()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0.1d,0.1d,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrReversedRateBounds()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0.5d,0d,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrRateBelowFloor()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),-0.91d,0.5d,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrRateAboveCeiling()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,4.01d,Y365),FinancialDatedStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrAllPositiveNoSignChange()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([100d,110d],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.NoSignChange); }

    [TestMethod] public void IrrAllNegativeNoSignChange()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,-110d],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.NoSignChange); }

    [TestMethod] public void IrrAllZeroIndeterminate()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([0d,0d,0d],Dates(0,365,730),0d,0.5d,Y365),FinancialDatedStatus64.IndeterminateZeroFlows); }

    [TestMethod] public void IrrSingleNonzeroFlowNoSignChange()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([0d,-100d,0d],Dates(0,365,730),0d,0.5d,Y365),FinancialDatedStatus64.NoSignChange); }

    [TestMethod] public void IrrTwoSignChangesPotentialMultiple()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,230d,-132d],Dates(0,365,730),-0.9d,4d,Y365),FinancialDatedStatus64.PotentialMultipleRoots); }

    [TestMethod] public void IrrOppositeTwoSignChangesPotentialMultiple()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([100d,-230d,132d],Dates(0,365,730),-0.9d,4d,Y365),FinancialDatedStatus64.PotentialMultipleRoots); }

    [TestMethod] public void IrrFourFlowsMultipleTransitions()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,220d,-120d,10d],Dates(0,365,730,1095),-0.9d,4d,Y365),FinancialDatedStatus64.PotentialMultipleRoots); }

    [TestMethod] public void IrrMultiplePatternHasTwoIndependentZeroOracles()
    { var a=-100d+230d/1.1d-132d/(1.1d*1.1d);var b=-100d+230d/1.2d-132d/(1.2d*1.2d);Assert.IsTrue(global::System.Math.Abs(a)<1e-10d);Assert.IsTrue(global::System.Math.Abs(b)<1e-10d);Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,230d,-132d],Dates(0,365,730),0d,0.5d,Y365),FinancialDatedStatus64.PotentialMultipleRoots); }

    [TestMethod] public void IrrRootNotInSuppliedBracket()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0.2d,0.5d,Y365),FinancialDatedStatus64.RootNotBracketed); }

    [TestMethod] public void IrrNegativeRootOutsidePositiveBracket()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,50d],Dates(0,365),0d,0.5d,Y365),FinancialDatedStatus64.RootNotBracketed); }

    [TestMethod] public void IrrRootBeyondRateCeiling()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,550d],Dates(0,365),0d,4d,Y365),FinancialDatedStatus64.RootNotBracketed); }

    [TestMethod] public void IrrRootBelowRateFloor()
    { Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,5d],Dates(0,365),-0.9d,4d,Y365),FinancialDatedStatus64.RootNotBracketed); }

    [TestMethod] public void IrrPrecancelled()
    { using var stop=new CancellationTokenSource();stop.Cancel();Refused(M6W02C04Boundary64.SolveDatedIrr([-100d,110d],Dates(0,365),0d,0.5d,Y365,stop.Token),FinancialDatedStatus64.Cancelled); }

    [TestMethod] public void IrrPrecancelOverridesInvalidInputs()
    { using var stop=new CancellationTokenSource();stop.Cancel();Refused(M6W02C04Boundary64.SolveDatedIrr(null,null,double.NaN,double.NaN,FinancialDayCountConvention64.Unspecified,stop.Token),FinancialDatedStatus64.Cancelled); }

    [TestMethod] public void LegacyT1DiscountUnchanged()
    { var old=M6W02C04Contract.Discount(100d,0.1d,1);Assert.IsTrue(old.Succeeded);Assert.IsNotNull(old.Value);Assert.IsTrue(global::System.Math.Abs(old.Value.Value-1000d/11d)<1e-10d); }

    [TestMethod] public void LegacyT1IrrUnchanged()
    { var old=M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d);Assert.IsTrue(old.Succeeded);Assert.IsNotNull(old.Value);Assert.IsTrue(global::System.Math.Abs(old.Value.Value-0.1d)<1e-10d); }
}
