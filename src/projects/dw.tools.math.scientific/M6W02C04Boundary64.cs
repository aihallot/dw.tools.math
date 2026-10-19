using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Explicit actual-day denominators. Neither convention imports holidays,
/// business-day rolling, interest curves, timezone assumptions or market data.
/// </summary>
public enum FinancialDayCountConvention64
{
    Unspecified,
    Actual365Fixed,
    Actual360
}

/// <summary>
/// Conservative, nonpartial statuses for date-aware effective annual finance.
/// PotentialMultipleRoots records an ambiguous sign pattern, NOT a root count.
/// A root outside a caller bracket is NOT proof that no global root exists.
/// </summary>
public enum FinancialDatedStatus64
{
    Success,
    DayCountRequired,
    InvalidInput,
    InvalidRate,
    InvalidRateBounds,
    InvalidCashflows,
    MissingDates,
    InvalidDates,
    OutsideQualifiedRange,
    NoSignChange,
    IndeterminateZeroFlows,
    PotentialMultipleRoots,
    RootNotBracketed,
    NonFiniteResult,
    Cancelled
}

/// <summary>
/// A finite present value or bracketed effective annual IRR and actual span.
/// Caller owns currency, day-count selection and physical interpretation.
/// Rejected results contain no partial Value or ElapsedDays.
/// </summary>
public sealed record FinancialDatedOutcome64(
    FinancialDatedStatus64 Status,
    double? Value,
    int? ElapsedDays,
    FinancialDayCountConvention64 DayCount,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == FinancialDatedStatus64.Success && !ErrorBoundGuaranteed &&
        Value is { } v && double.IsFinite(v) &&
        ElapsedDays is >= 0 and <= M6W02C04Boundary64.MaxCalendarSpanDays &&
        DayCount is FinancialDayCountConvention64.Actual365Fixed or
            FinancialDayCountConvention64.Actual360;
}

/// <summary>
/// BCL-only dated finance sidecar over the qualified pure T1 effective-period
/// domain. Annual effective rate r is applied to fractional actual days:
/// present value = amount / (1+r)^(actualDays / fixedDayDenominator).
/// No implicit calendar, ACT/ACT, business-day, compounding frequency,
/// market-data service, recommendation or certified numerical error bound.
/// </summary>
public static class M6W02C04Boundary64
{
    public const string RuleId = "dw-math-m6-w02-c04-actual-day-finance-boundaries-1";
    public const int MaxCalendarSpanDays = 36500;
    public const int MaxBisectionIterations = 96;

    public static FinancialDatedOutcome64 DiscountDated(
        double amount, double effectiveAnnualRate,
        DateOnly valuationDate, DateOnly paymentDate,
        FinancialDayCountConvention64 dayCount,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialDatedStatus64.Cancelled);
        if (!ValidConvention(dayCount))
            return Refuse(FinancialDatedStatus64.DayCountRequired);
        if (!double.IsFinite(amount))
            return Refuse(FinancialDatedStatus64.InvalidInput);
        if (!ValidRate(effectiveAnnualRate))
            return Refuse(FinancialDatedStatus64.InvalidRate);
        if (global::System.Math.Abs(amount) > M6W02C04Contract.MaxAbsCashflow)
            return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
        var elapsed = paymentDate.DayNumber - valuationDate.DayNumber;
        if (elapsed < 0) return Refuse(FinancialDatedStatus64.InvalidDates);
        if (elapsed > MaxCalendarSpanDays)
            return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
        var exponent = (double)elapsed / Denominator(dayCount);
        var factor = global::System.Math.Pow(1d + effectiveAnnualRate, exponent);
        var present = amount / factor;
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialDatedStatus64.Cancelled);
        if (!double.IsFinite(factor) || factor <= 0d || !double.IsFinite(present))
            return Refuse(FinancialDatedStatus64.NonFiniteResult);
        if (global::System.Math.Abs(present) > M6W02C04Contract.MaxAbsPresentValue)
            return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
        return Accepted(present, elapsed, dayCount);
    }

    /// <summary>
    /// Solve sum(cashflows[i]/(1+r)^((dates[i]-dates[0])/daysPerYear))=0.
    /// Dates must strictly increase and be within 36500 actual days.
    /// Only one nonzero sign transition and an explicit sign bracket
    /// are admitted. More sign changes are refused as POTENTIALLY
    /// multiple-root, without inferring the exact global root count.
    /// </summary>
    public static FinancialDatedOutcome64 SolveDatedIrr(
        double[]? cashflows, DateOnly[]? dates,
        double minRate, double maxRate,
        FinancialDayCountConvention64 dayCount,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialDatedStatus64.Cancelled);
        if (!ValidConvention(dayCount))
            return Refuse(FinancialDatedStatus64.DayCountRequired);
        if (cashflows is null || cashflows.Length < 2 ||
            cashflows.Length > M6W02C04Contract.MaxCashflows)
            return Refuse(FinancialDatedStatus64.InvalidCashflows);
        if (dates is null) return Refuse(FinancialDatedStatus64.MissingDates);
        if (dates.Length != cashflows.Length)
            return Refuse(FinancialDatedStatus64.InvalidDates);
        if (!ValidRate(minRate) || !ValidRate(maxRate) || minRate >= maxRate)
            return Refuse(FinancialDatedStatus64.InvalidRateBounds);

        var transitions = 0;
        var lastSign = 0;
        var anyNonZero = false;
        for (var index = 0; index < cashflows.Length; index++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FinancialDatedStatus64.Cancelled);
            var cashflow = cashflows[index];
            if (!double.IsFinite(cashflow))
                return Refuse(FinancialDatedStatus64.InvalidInput);
            if (global::System.Math.Abs(cashflow) > M6W02C04Contract.MaxAbsCashflow)
                return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
            var days = dates[index].DayNumber - dates[0].DayNumber;
            if (index > 0 && dates[index].DayNumber <= dates[index-1].DayNumber)
                return Refuse(FinancialDatedStatus64.InvalidDates);
            if (days > MaxCalendarSpanDays)
                return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
            var sign = global::System.Math.Sign(cashflow);
            if (sign != 0)
            {
                anyNonZero = true;
                if (lastSign != 0 && lastSign != sign) transitions++;
                lastSign = sign;
            }
        }
        if (!anyNonZero)
            return Refuse(FinancialDatedStatus64.IndeterminateZeroFlows);
        if (transitions == 0)
            return Refuse(FinancialDatedStatus64.NoSignChange);
        if (transitions > 1)
            return Refuse(FinancialDatedStatus64.PotentialMultipleRoots);

        var lower = minRate;
        var upper = maxRate;
        var lowerNpv = PresentValueAt(cashflows, dates, lower, dayCount, cancellationToken);
        if (!lowerNpv.Succeeded) return Refuse(lowerNpv.Status);
        var upperNpv = PresentValueAt(cashflows, dates, upper, dayCount, cancellationToken);
        if (!upperNpv.Succeeded) return Refuse(upperNpv.Status);
        var fl = lowerNpv.Value!.Value;
        var fu = upperNpv.Value!.Value;
        var span = dates[^1].DayNumber - dates[0].DayNumber;
        if (fl == 0d) return Accepted(lower, span, dayCount);
        if (fu == 0d) return Accepted(upper, span, dayCount);
        if (global::System.Math.Sign(fl) == global::System.Math.Sign(fu))
            return Refuse(FinancialDatedStatus64.RootNotBracketed);

        for (var attempt = 0; attempt < MaxBisectionIterations; attempt++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FinancialDatedStatus64.Cancelled);
            var middle = lower + (upper-lower)*0.5d;
            if (middle == lower || middle == upper) break;
            var midResult = PresentValueAt(cashflows, dates, middle,
                dayCount, cancellationToken);
            if (!midResult.Succeeded) return Refuse(midResult.Status);
            var fm = midResult.Value!.Value;
            if (fm == 0d) return Accepted(middle, span, dayCount);
            if (global::System.Math.Sign(fm) == global::System.Math.Sign(fl))
            {
                lower = middle;
                fl = fm;
            }
            else
            {
                upper = middle;
            }
        }
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialDatedStatus64.Cancelled);
        var rate = lower + (upper-lower)*0.5d;
        return double.IsFinite(rate)
            ? Accepted(rate, span, dayCount)
            : Refuse(FinancialDatedStatus64.NonFiniteResult);
    }

    private static FinancialDatedOutcome64 PresentValueAt(
        double[] cashflows, DateOnly[] dates, double annualRate,
        FinancialDayCountConvention64 convention, CancellationToken cancellationToken)
    {
        var sum = 0d;
        var baseRate = 1d + annualRate;
        for (var i = 0; i < cashflows.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FinancialDatedStatus64.Cancelled);
            var days = dates[i].DayNumber - dates[0].DayNumber;
            var factor = global::System.Math.Pow(baseRate,
                (double)days / Denominator(convention));
            if (!double.IsFinite(factor) || factor <= 0d)
                return Refuse(FinancialDatedStatus64.NonFiniteResult);
            var term = cashflows[i] / factor;
            sum += term;
            if (!double.IsFinite(term) || !double.IsFinite(sum))
                return Refuse(FinancialDatedStatus64.NonFiniteResult);
            if (global::System.Math.Abs(sum) > M6W02C04Contract.MaxAbsPresentValue)
                return Refuse(FinancialDatedStatus64.OutsideQualifiedRange);
        }
        return Accepted(sum, dates[^1].DayNumber - dates[0].DayNumber, convention);
    }

    private static bool ValidRate(double rate) =>
        double.IsFinite(rate) &&
        rate >= M6W02C04Contract.MinimumAdmittedRate &&
        rate <= M6W02C04Contract.MaximumAdmittedRate;

    private static bool ValidConvention(FinancialDayCountConvention64 convention) =>
        convention is FinancialDayCountConvention64.Actual365Fixed or
            FinancialDayCountConvention64.Actual360;

    private static int Denominator(FinancialDayCountConvention64 convention) =>
        convention == FinancialDayCountConvention64.Actual360 ? 360 : 365;

    private static FinancialDatedOutcome64 Accepted(
        double value, int elapsed, FinancialDayCountConvention64 convention) =>
        new(FinancialDatedStatus64.Success, value, elapsed, convention, RuleId, false);

    private static FinancialDatedOutcome64 Refuse(FinancialDatedStatus64 status) =>
        new(status, null, null, FinancialDayCountConvention64.Unspecified, RuleId, false);
}
