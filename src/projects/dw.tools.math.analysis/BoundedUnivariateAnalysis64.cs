using System;

namespace Dw.Tools.Math.Analysis;

/// <summary>Status of a bounded, caller-selected binary64 univariate calculation.</summary>
public enum UnivariateStatus64
{
    Success, InvalidInput, InvalidPolicy, MissingBracket, NonFiniteEvaluation,
    NonFiniteIntermediate, EvaluationFailed, NonConverged, BudgetExceeded, Cancelled
}

/// <summary>Finite numerical requests, not proof of a root error guarantee.</summary>
public sealed record RootPolicy64(
    double AbsoluteXTolerance = 1e-10,
    double AbsoluteResidualTolerance = 1e-10,
    int MaximumIterations = 128);

/// <summary>Composite Simpson controls. The error estimate is not a rigorous bound.</summary>
public sealed record QuadraturePolicy64(
    double AbsoluteErrorEstimateTolerance = 1e-8,
    int MaximumPanels = 128,
    int MaximumEvaluations = 1025);

/// <summary>Observed bracket, function residual and evaluation counts.</summary>
public sealed record RootSolveResult64(
    UnivariateStatus64 Status, double? Root, double? AbsoluteResidual,
    double? ObservedBracketWidth, int Iterations, int FunctionEvaluations,
    bool GuaranteedRootError, string RuleId)
{
    public bool Succeeded => Status == UnivariateStatus64.Success &&
        Root is { } root && double.IsFinite(root) &&
        AbsoluteResidual is { } residual && double.IsFinite(residual) &&
        ObservedBracketWidth is { } span && double.IsFinite(span) &&
        residual >= 0d && span >= 0d && !GuaranteedRootError;
}

/// <summary>Estimated integral with a noncertified smooth-case refinement diagnostic.</summary>
public sealed record QuadratureResult64(
    UnivariateStatus64 Status, double? Integral, double? EstimatedAbsoluteError,
    int Panels, int FunctionEvaluations, bool ErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded => Status == UnivariateStatus64.Success &&
        Integral is { } integral && double.IsFinite(integral) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && !ErrorBoundGuaranteed;
}

/// <summary>
/// Bounded deterministic binary64 bisector and composite Simpson estimator.
/// Continuity, smoothness and correctness of the caller's callback are assumptions,
/// not verified mathematical properties. No provider, agent or decision dependency.
/// </summary>
public static class BoundedUnivariateAnalysis64
{
    public const string RuleId = "dw-math-bounded-univariate-binary64-1";
    public const int MaximumRootIterations = 256;
    public const int MaximumQuadraturePanels = 256;
    private static readonly RootPolicy64 DefaultRootPolicy = new();
    private static readonly QuadraturePolicy64 DefaultQuadraturePolicy = new();

    public static RootSolveResult64 SolveBracketed(
        Func<double, double>? function, double lower, double upper,
        RootPolicy64? policy = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RootRefuse(UnivariateStatus64.Cancelled);
        if(function is null || !ValidInterval(lower, upper))
            return RootRefuse(UnivariateStatus64.InvalidInput);
        var selected = policy ?? DefaultRootPolicy;
        if(!ValidRootPolicy(selected))
            return RootRefuse(UnivariateStatus64.InvalidPolicy);
        var width = upper - lower;
        if(!double.IsFinite(width))
            return RootRefuse(UnivariateStatus64.NonFiniteIntermediate);
        var leftState = Evaluate(function, lower, out var fleft);
        if(leftState != UnivariateStatus64.Success) return RootRefuse(leftState);
        var rightState = Evaluate(function, upper, out var fright);
        if(rightState != UnivariateStatus64.Success) return RootRefuse(rightState);
        if(fleft == 0d) return RootAdmit(lower, 0d, 0d, 0, 2);
        if(fright == 0d) return RootAdmit(upper, 0d, 0d, 0, 2);
        if(!Opposite(fleft, fright))
            return RootRefuse(UnivariateStatus64.MissingBracket);
        var left = lower;
        var right = upper;
        for(var iteration = 1; iteration <= selected.MaximumIterations; iteration++)
        {
            if(cancellationToken.IsCancellationRequested)
                return RootRefuse(UnivariateStatus64.Cancelled);
            var observedWidth = right - left;
            var midpoint = left + observedWidth / 2d;
            if(!double.IsFinite(midpoint) || midpoint == left || midpoint == right)
                return RootRefuse(UnivariateStatus64.NonConverged);
            var status = Evaluate(function, midpoint, out var fmid);
            if(status != UnivariateStatus64.Success) return RootRefuse(status);
            var residual = global::System.Math.Abs(fmid);
            if(residual <= selected.AbsoluteResidualTolerance ||
               observedWidth <= selected.AbsoluteXTolerance)
                return RootAdmit(midpoint, residual, observedWidth, iteration, iteration + 2);
            if(Opposite(fleft, fmid))
            {
                right = midpoint;
            }
            else
            {
                left = midpoint;
                fleft = fmid;
            }
        }
        return RootRefuse(UnivariateStatus64.NonConverged);
    }

    public static QuadratureResult64 IntegrateSimpson(
        Func<double, double>? function, double lower, double upper,
        QuadraturePolicy64? policy = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return QuadratureRefuse(UnivariateStatus64.Cancelled);
        if(function is null || !ValidInterval(lower, upper))
            return QuadratureRefuse(UnivariateStatus64.InvalidInput);
        var selected = policy ?? DefaultQuadraturePolicy;
        if(!ValidQuadraturePolicy(selected))
            return QuadratureRefuse(UnivariateStatus64.InvalidPolicy);
        var span = upper - lower;
        if(!double.IsFinite(span))
            return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
        var evaluated = 0;
        double? previous = null;
        for(var panels = 2; panels <= selected.MaximumPanels; panels *= 2)
        {
            if(cancellationToken.IsCancellationRequested)
                return QuadratureRefuse(UnivariateStatus64.Cancelled);
            if(evaluated + panels + 1 > selected.MaximumEvaluations)
                return QuadratureRefuse(UnivariateStatus64.BudgetExceeded);
            var step = span / panels;
            if(!(step > 0d) || !double.IsFinite(step))
                return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
            var sum = 0d;
            var compensation = 0d;
            for(var i = 0; i <= panels; i++)
            {
                if(cancellationToken.IsCancellationRequested)
                    return QuadratureRefuse(UnivariateStatus64.Cancelled);
                var x = i == 0 ? lower : i == panels ? upper : lower + step * i;
                if(!double.IsFinite(x))
                    return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
                var status = Evaluate(function, x, out var y);
                if(status != UnivariateStatus64.Success)
                    return QuadratureRefuse(status);
                evaluated++;
                var coefficient = i == 0 || i == panels ? 1d : i % 2 == 0 ? 2d : 4d;
                var term = coefficient * y;
                if(!double.IsFinite(term))
                    return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
                var adjusted = term - compensation;
                var next = sum + adjusted;
                compensation = (next - sum) - adjusted;
                sum = next;
                if(!double.IsFinite(sum))
                    return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
            }
            var estimate = (step / 3d) * sum;
            if(!double.IsFinite(estimate))
                return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
            if(previous is { } old)
            {
                var errorEstimate = global::System.Math.Abs(estimate - old) / 15d;
                if(!double.IsFinite(errorEstimate))
                    return QuadratureRefuse(UnivariateStatus64.NonFiniteIntermediate);
                if(errorEstimate <= selected.AbsoluteErrorEstimateTolerance)
                    return new(UnivariateStatus64.Success, estimate, errorEstimate,
                        panels, evaluated, false, RuleId);
            }
            previous = estimate;
        }
        return QuadratureRefuse(UnivariateStatus64.NonConverged);
    }

    private static bool ValidInterval(double lower, double upper) =>
        double.IsFinite(lower) && double.IsFinite(upper) && lower < upper;

    private static bool ValidRootPolicy(RootPolicy64 options) =>
        double.IsFinite(options.AbsoluteXTolerance) &&
        options.AbsoluteXTolerance > 0d &&
        double.IsFinite(options.AbsoluteResidualTolerance) &&
        options.AbsoluteResidualTolerance > 0d &&
        options.MaximumIterations is >= 1 and <= MaximumRootIterations;

    private static bool ValidQuadraturePolicy(QuadraturePolicy64 options) =>
        double.IsFinite(options.AbsoluteErrorEstimateTolerance) &&
        options.AbsoluteErrorEstimateTolerance > 0d &&
        options.MaximumPanels is >= 4 and <= MaximumQuadraturePanels &&
        (options.MaximumPanels & (options.MaximumPanels - 1)) == 0 &&
        options.MaximumEvaluations is >= 8 and <= 2048;

    private static bool Opposite(double a, double b) =>
        (a < 0d && b > 0d) || (a > 0d && b < 0d);

    private static UnivariateStatus64 Evaluate(
        Func<double, double> function, double x, out double value)
    {
        value = double.NaN;
        try { value = function(x); }
        catch(OperationCanceledException) { return UnivariateStatus64.Cancelled; }
        catch(Exception) { return UnivariateStatus64.EvaluationFailed; }
        return double.IsFinite(value) ? UnivariateStatus64.Success :
            UnivariateStatus64.NonFiniteEvaluation;
    }

    private static RootSolveResult64 RootAdmit(
        double root, double residual, double width, int iterations, int evaluations) =>
        new(UnivariateStatus64.Success, root, residual, width,
            iterations, evaluations, false, RuleId);

    private static RootSolveResult64 RootRefuse(UnivariateStatus64 status) =>
        new(status, null, null, null, 0, 0, false, RuleId);

    private static QuadratureResult64 QuadratureRefuse(UnivariateStatus64 status) =>
        new(status, null, null, 0, 0, false, RuleId);
}
