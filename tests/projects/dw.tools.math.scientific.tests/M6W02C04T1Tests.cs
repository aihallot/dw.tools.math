using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C04T1Tests
{
    private static void Near(FinancialMathOutcome64 actual,
        double expected,double tolerance=1e-11d)
    {
        Assert.IsTrue(actual.Succeeded);
        Assert.AreEqual(FinancialMathStatus64.Success,actual.Status);
        Assert.IsNotNull(actual.Value);
        var diff=global::System.Math.Abs(actual.Value.Value-expected);
        Assert.IsTrue(diff <= tolerance*
            global::System.Math.Max(1d,global::System.Math.Abs(expected)),
            $"Expected {expected:R}; observed {actual.Value.Value:R}; delta {diff:R}");
        Assert.IsFalse(actual.ErrorBoundGuaranteed);
    }

    private static void Refused(FinancialMathOutcome64 actual,FinancialMathStatus64 status)
    {
        Assert.AreEqual(status,actual.Status);
        Assert.IsFalse(actual.Succeeded);
        Assert.IsNull(actual.Value);
        Assert.IsFalse(actual.ErrorBoundGuaranteed);
    }

    [TestMethod] public void DiscountOnePeriodCanonical()
    { Near(M6W02C04Contract.Discount(100d,0.1d,1),1000d/11d); }

    [TestMethod] public void DiscountFromFuture110()
    { Near(M6W02C04Contract.Discount(110d,0.1d,1),100d); }

    [TestMethod] public void DiscountTwoPeriods121()
    { Near(M6W02C04Contract.Discount(121d,0.1d,2),100d); }

    [TestMethod] public void DiscountNegativeAmount()
    { Near(M6W02C04Contract.Discount(-110d,0.1d,1),-100d); }

    [TestMethod] public void DiscountZeroAmount()
    { Near(M6W02C04Contract.Discount(0d,0.1d,1),0d); }

    [TestMethod] public void DiscountAtZeroRate()
    { Near(M6W02C04Contract.Discount(100d,0d,16),100d); }

    [TestMethod] public void DiscountAtZeroPeriods()
    { Near(M6W02C04Contract.Discount(100d,0.5d,0),100d); }

    [TestMethod] public void DiscountUpperRateBoundary()
    { Near(M6W02C04Contract.Discount(100d,4d,1),20d); }

    [TestMethod] public void DiscountLowerRateBoundary()
    { Near(M6W02C04Contract.Discount(100d,-0.9d,1),1000d); }

    [TestMethod] public void DiscountNegativeRate()
    { Near(M6W02C04Contract.Discount(100d,-0.5d,1),200d); }

    [TestMethod] public void DiscountAtMaxInput()
    { Near(M6W02C04Contract.Discount(1e6d,0d,1),1e6d); }

    [TestMethod] public void DiscountThirtyTwoPeriods()
    { Near(M6W02C04Contract.Discount(100d,0d,32),100d); }

    [TestMethod] public void DiscountAtAdmittedGrowth()
    { Near(M6W02C04Contract.Discount(100d,1d,3),12.5d); }

    [TestMethod] public void DiscountAtAdmittedFractionalRate()
    { Near(M6W02C04Contract.Discount(125d,0.25d,1),100d); }

    [TestMethod] public void DiscountRuleId()
    { var o=M6W02C04Contract.Discount(100d,0d,1);Assert.IsTrue(o.Succeeded);Assert.AreEqual(M6W02C04Contract.RuleId,o.RuleId); }

    [TestMethod] public void DiscountNoErrorTheorem()
    { var o=M6W02C04Contract.Discount(100d,0d,1);Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.ErrorBoundGuaranteed); }

    [TestMethod] public void DiscountNaNAmountRefused()
    { Refused(M6W02C04Contract.Discount(double.NaN,0.1d,1),FinancialMathStatus64.InvalidInput); }

    [TestMethod] public void DiscountInfiniteAmountRefused()
    { Refused(M6W02C04Contract.Discount(double.PositiveInfinity,0.1d,1),FinancialMathStatus64.InvalidInput); }

    [TestMethod] public void DiscountNaNRateRefused()
    { Refused(M6W02C04Contract.Discount(100d,double.NaN,1),FinancialMathStatus64.InvalidRate); }

    [TestMethod] public void DiscountPositiveInfiniteRateRefused()
    { Refused(M6W02C04Contract.Discount(100d,double.PositiveInfinity,1),FinancialMathStatus64.InvalidRate); }

    [TestMethod] public void DiscountRateBelowFloorRefused()
    { Refused(M6W02C04Contract.Discount(100d,-0.91d,1),FinancialMathStatus64.InvalidRate); }

    [TestMethod] public void DiscountRateAboveCeilingRefused()
    { Refused(M6W02C04Contract.Discount(100d,4.01d,1),FinancialMathStatus64.InvalidRate); }

    [TestMethod] public void DiscountNegativePeriodRefused()
    { Refused(M6W02C04Contract.Discount(100d,0.1d,-1),FinancialMathStatus64.InvalidPeriod); }

    [TestMethod] public void DiscountExcessPeriodRefused()
    { Refused(M6W02C04Contract.Discount(100d,0.1d,33),FinancialMathStatus64.InvalidPeriod); }

    [TestMethod] public void DiscountOversizedAmountRefused()
    { Refused(M6W02C04Contract.Discount(1e6d+1d,0.1d,1),FinancialMathStatus64.OutsideQualifiedRange); }

    [TestMethod] public void DiscountPreCancelled()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Contract.Discount(100d,0.1d,1,c.Token),FinancialMathStatus64.Cancelled); }

    [TestMethod] public void DiscountCancelPriorToRateValidation()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Contract.Discount(double.NaN,double.NaN,-1,c.Token),FinancialMathStatus64.Cancelled); }

    [TestMethod] public void IrrCanonicalTenPercent()
    { Near(M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrCanonicalWideBracket()
    { Near(M6W02C04Contract.SolveIrr([-100d,110d],-0.9d,4d),0.1d,1e-10d); }

    [TestMethod] public void IrrReversedSignFlows()
    { Near(M6W02C04Contract.SolveIrr([100d,-110d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrTwentyOnePercent()
    { Near(M6W02C04Contract.SolveIrr([-100d,121d],0d,0.5d),0.21d,1e-10d); }

    [TestMethod] public void IrrZeroPercent()
    { Near(M6W02C04Contract.SolveIrr([-100d,100d],0d,0.5d),0d); }

    [TestMethod] public void IrrZeroWithinWideBracket()
    { Near(M6W02C04Contract.SolveIrr([-100d,100d],-0.5d,0.5d),0d,1e-10d); }

    [TestMethod] public void IrrNegativeRate()
    { Near(M6W02C04Contract.SolveIrr([-100d,50d],-0.9d,0d),-0.5d,1e-10d); }

    [TestMethod] public void IrrUpperBoundaryExact()
    { Near(M6W02C04Contract.SolveIrr([-100d,500d],0d,4d),4d); }

    [TestMethod] public void IrrThreePeriodZeroMiddle()
    { Near(M6W02C04Contract.SolveIrr([-100d,0d,121d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrLeadingZeroAccepted()
    { Near(M6W02C04Contract.SolveIrr([0d,-100d,110d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrTrailingZeroAccepted()
    { Near(M6W02C04Contract.SolveIrr([-100d,110d,0d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrThreeCashflows()
    { Near(M6W02C04Contract.SolveIrr([-100d,55d,60.5d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrThreePeriods()
    { Near(M6W02C04Contract.SolveIrr([-100d,0d,0d,133.1d],0d,0.5d),0.1d,1e-10d); }

    [TestMethod] public void IrrSingleSignChangeRule()
    { var o=M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d);Assert.IsTrue(o.Succeeded);Assert.AreEqual(M6W02C04Contract.RuleId,o.RuleId); }

    [TestMethod] public void IrrNoNumericErrorTheorem()
    { var o=M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d);Assert.IsTrue(o.Succeeded);Assert.IsFalse(o.ErrorBoundGuaranteed); }

    [TestMethod] public void IrrNoCalendarInResult()
    { var o=M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d);Near(o,0.1d);Assert.IsNotNull(o.Value); }

    [TestMethod] public void IrrNullArrayRefused()
    { Refused(M6W02C04Contract.SolveIrr(null,0d,0.5d),FinancialMathStatus64.InvalidCashflows); }

    [TestMethod] public void IrrEmptyArrayRefused()
    { Refused(M6W02C04Contract.SolveIrr([],0d,0.5d),FinancialMathStatus64.InvalidCashflows); }

    [TestMethod] public void IrrSingleCashflowRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d],0d,0.5d),FinancialMathStatus64.InvalidCashflows); }

    [TestMethod] public void IrrSeventeenCashflowsRefused()
    { Refused(M6W02C04Contract.SolveIrr(new double[17],0d,0.5d),FinancialMathStatus64.InvalidCashflows); }

    [TestMethod] public void IrrNaNCashflowRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,double.NaN],0d,0.5d),FinancialMathStatus64.InvalidInput); }

    [TestMethod] public void IrrInfiniteCashflowRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,double.NegativeInfinity],0d,0.5d),FinancialMathStatus64.InvalidInput); }

    [TestMethod] public void IrrOutOfBudgetCashflowRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,1e6d+1d],0d,0.5d),FinancialMathStatus64.OutsideQualifiedRange); }

    [TestMethod] public void IrrNaNLowerBoundRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],double.NaN,0.5d),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrInfiniteUpperBoundRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],0d,double.PositiveInfinity),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrInvertedBracketRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],0.5d,0d),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrZeroWidthBracketRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],0d,0d),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrLowerBoundBelowRateFloorRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],-0.95d,0d),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrUpperBoundAboveCeilingRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],0d,5d),FinancialMathStatus64.InvalidRateBounds); }

    [TestMethod] public void IrrAllPositiveNoSignChange()
    { Refused(M6W02C04Contract.SolveIrr([100d,110d],0d,0.5d),FinancialMathStatus64.NoSignChange); }

    [TestMethod] public void IrrAllNegativeNoSignChange()
    { Refused(M6W02C04Contract.SolveIrr([-100d,-110d],0d,0.5d),FinancialMathStatus64.NoSignChange); }

    [TestMethod] public void IrrAllZeroNoSignChange()
    { Refused(M6W02C04Contract.SolveIrr([0d,0d,0d],0d,0.5d),FinancialMathStatus64.NoSignChange); }

    [TestMethod] public void IrrMixedSignsAmbiguousRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,220d,-121d],-0.9d,4d),FinancialMathStatus64.AmbiguousCashflowPattern); }

    [TestMethod] public void IrrAlternatingSignsAmbiguousRefused()
    { Refused(M6W02C04Contract.SolveIrr([-100d,200d,-150d,100d],-0.9d,4d),FinancialMathStatus64.AmbiguousCashflowPattern); }

    [TestMethod] public void IrrRootOutsideSuppliedBracket()
    { Refused(M6W02C04Contract.SolveIrr([-100d,110d],0.2d,0.5d),FinancialMathStatus64.RootNotBracketed); }

    [TestMethod] public void IrrRootAboveMaxSupportedRate()
    { Refused(M6W02C04Contract.SolveIrr([-100d,550d],0d,4d),FinancialMathStatus64.RootNotBracketed); }

    [TestMethod] public void IrrRootBelowSuppliedBracket()
    { Refused(M6W02C04Contract.SolveIrr([-100d,50d],0d,0.5d),FinancialMathStatus64.RootNotBracketed); }

    [TestMethod] public void IrrPreCancelled()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d,c.Token),FinancialMathStatus64.Cancelled); }

    [TestMethod] public void IrrPreCancelledBeforeNullInput()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(M6W02C04Contract.SolveIrr(null,double.NaN,double.NaN,c.Token),FinancialMathStatus64.Cancelled); }
}
