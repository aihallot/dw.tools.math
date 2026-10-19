using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Explicit status for bounded, parameter-only per-period financial calculations.
/// Refusals never carry a partial monetary value, a rate or a recommendation.
/// </summary>
public enum FinancialMathStatus64
{
    Success,
    InvalidInput,
    InvalidRate,
    InvalidPeriod,
    InvalidCashflows,
    InvalidRateBounds,
    OutsideQualifiedRange,
    NoSignChange,
    AmbiguousCashflowPattern,
    RootNotBracketed,
    NonFiniteResult,
    Cancelled
}

/// <summary>
/// One finite binary64 present value or a single accepted bracketed IRR.
/// Value denotes currency in Discount and per-period effective rate in SolveIrr.
/// These are caller-managed units and not a rounding-error guarantee.
/// </summary>
public sealed record FinancialMathOutcome64(
    FinancialMathStatus64 Status,
    double? Value,
    string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded =>
        Status == FinancialMathStatus64.Success &&
        Value is { } value && double.IsFinite(value) &&
        !ErrorBoundGuaranteed;
}

/// <summary>
/// Pure bounded financial arithmetic. Rates are effective DISCRETE rates
/// per caller-supplied period; no date, day-count, calendar, discount curve,
/// market data, continuous compounding, financial advice or ranking.
/// IRR uses exactly one nonzero cashflow sign change and a caller-supplied
/// finite bracket; more complex root topology is reserved for C04-T2.
/// </summary>
public static class M6W02C04Contract
{
    public const string RuleId = "dw-math-m6-w02-c04-effective-period-rate-1";
    public const int MaxPeriods = 32;
    public const int MaxCashflows = 16;
    public const double MinimumAdmittedRate = -0.9d;
    public const double MaximumAdmittedRate = 4d;
    public const double MaxAbsCashflow = 1e6d;
    public const double MaxAbsPresentValue = 1e40d;

    /// <summary>
    /// PresentValue = amount / (1 + effectiveRate) ^ periods.
    /// An input amount may be negative; its unit remains caller-managed.
    /// </summary>
    public static FinancialMathOutcome64 Discount(
        double amount, double effectiveRate, int periods,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialMathStatus64.Cancelled);
        if (!double.IsFinite(amount))
            return Refuse(FinancialMathStatus64.InvalidInput);
        if (!ValidRate(effectiveRate))
            return Refuse(FinancialMathStatus64.InvalidRate);
        if (periods < 0 || periods > MaxPeriods)
            return Refuse(FinancialMathStatus64.InvalidPeriod);
        if (global::System.Math.Abs(amount) > MaxAbsCashflow)
            return Refuse(FinancialMathStatus64.OutsideQualifiedRange);

        var factor = global::System.Math.Pow(1d + effectiveRate, periods);
        var present = amount / factor;
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialMathStatus64.Cancelled);
        if (!double.IsFinite(factor) || factor <= 0d || !double.IsFinite(present))
            return Refuse(FinancialMathStatus64.NonFiniteResult);
        if (global::System.Math.Abs(present) > MaxAbsPresentValue)
            return Refuse(FinancialMathStatus64.OutsideQualifiedRange);
        return new(FinancialMathStatus64.Success, present, RuleId, false);
    }

    /// <summary>
    /// Solve sum(cashflows[t]/(1+r)^t) = 0 by bounded bisection.
    /// The initial cashflow is at period zero. Only an input with exactly
    /// one nonzero sign transition AND a root bracketed in [minRate,maxRate]
    /// is admitted. Refusing other patterns is not a proof of multiple IRRs.
    /// </summary>
    public static FinancialMathOutcome64 SolveIrr(
        double[]? cashflows, double minRate, double maxRate,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialMathStatus64.Cancelled);
        if (cashflows is null || cashflows.Length < 2 ||
            cashflows.Length > MaxCashflows)
            return Refuse(FinancialMathStatus64.InvalidCashflows);
        if (!ValidRate(minRate) || !ValidRate(maxRate) || minRate >= maxRate)
            return Refuse(FinancialMathStatus64.InvalidRateBounds);

        var transitions = 0;
        var lastSign = 0;
        foreach (var value in cashflows)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FinancialMathStatus64.Cancelled);
            if (!double.IsFinite(value))
                return Refuse(FinancialMathStatus64.InvalidInput);
            if (global::System.Math.Abs(value) > MaxAbsCashflow)
                return Refuse(FinancialMathStatus64.OutsideQualifiedRange);
            var sign = global::System.Math.Sign(value);
            if (sign == 0) continue;
            if (lastSign != 0 && sign != lastSign) transitions++;
            lastSign = sign;
        }
        if (transitions == 0)
            return Refuse(FinancialMathStatus64.NoSignChange);
        if (transitions != 1)
            return Refuse(FinancialMathStatus64.AmbiguousCashflowPattern);

        var a = minRate;
        var b = maxRate;
        var fa = PresentValueAtRate(cashflows, a, cancellationToken);
        var fb = PresentValueAtRate(cashflows, b, cancellationToken);
        if (fa.Status != FinancialMathStatus64.Success) return Refuse(fa.Status);
        if (fb.Status != FinancialMathStatus64.Success) return Refuse(fb.Status);
        var left = fa.Value!.Value;
        var right = fb.Value!.Value;
        if (left == 0d) return new(FinancialMathStatus64.Success, a, RuleId, false);
        if (right == 0d) return new(FinancialMathStatus64.Success, b, RuleId, false);
        if (global::System.Math.Sign(left) == global::System.Math.Sign(right))
            return Refuse(FinancialMathStatus64.RootNotBracketed);

        for (var iteration = 0; iteration < 96; iteration++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Refuse(FinancialMathStatus64.Cancelled);
            var middle = a + (b - a) * 0.5d;
            if (middle == a || middle == b)
                break;
            var result = PresentValueAtRate(cashflows, middle, cancellationToken);
            if (result.Status != FinancialMathStatus64.Success) return Refuse(result.Status);
            var fm = result.Value!.Value;
            if (fm == 0d)
                return new(FinancialMathStatus64.Success, middle, RuleId, false);
            if (global::System.Math.Sign(fm) == global::System.Math.Sign(left))
            {
                a = middle;
                left = fm;
            }
            else
            {
                b = middle;
            }
        }
        if (cancellationToken.IsCancellationRequested)
            return Refuse(FinancialMathStatus64.Cancelled);
        var rate = a + (b - a) * 0.5d;
        return double.IsFinite(rate)
            ? new(FinancialMathStatus64.Success, rate, RuleId, false)
            : Refuse(FinancialMathStatus64.NonFiniteResult);
    }

    private static FinancialMathOutcome64 PresentValueAtRate(
        double[] cashflows, double rate, CancellationToken token)
    {
        var total = 0d;
        var divisor = 1d;
        var baseRate = 1d + rate;
        for (var i = 0; i < cashflows.Length; i++)
        {
            if (token.IsCancellationRequested)
                return Refuse(FinancialMathStatus64.Cancelled);
            if (i > 0) divisor *= baseRate;
            var term = cashflows[i] / divisor;
            total += term;
            if (!double.IsFinite(total) || !double.IsFinite(term) ||
                !double.IsFinite(divisor) || divisor <= 0d)
                return Refuse(FinancialMathStatus64.NonFiniteResult);
            if (global::System.Math.Abs(total) > MaxAbsPresentValue)
                return Refuse(FinancialMathStatus64.OutsideQualifiedRange);
        }
        return new(FinancialMathStatus64.Success, total, RuleId, false);
    }

    private static bool ValidRate(double rate) =>
        double.IsFinite(rate) && rate >= MinimumAdmittedRate &&
        rate <= MaximumAdmittedRate;

    private static FinancialMathOutcome64 Refuse(FinancialMathStatus64 status) =>
        new(status, null, RuleId, false);
}
