using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>A classified boundary is not a fabricated integral or point value.</summary>
public enum SymbolicCalculusBoundaryStatus
{
    Exact,
    ExactRemovableLimit,
    SimplePole,
    OriginalDomainHole,
    UnknownConvergence,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    UnsupportedApproximation,
    BudgetExceeded
}

/// <summary>
/// ExactRemovableLimit is the limit of the function, never the value at the
/// original excluded point. A typed refusal carries no exact answer.
/// </summary>
public sealed record SymbolicCalculusBoundaryResult(
    SymbolicCalculusBoundaryStatus Status,
    ExactRational? ExactValue,
    ExactRational? ExceptionalPoint,
    string? ConstantConvention,
    string? RuleId,
    string? SourceSemanticSha256,
    string? VariableIdentity)
{
    public bool HasExactValue =>
        (Status is SymbolicCalculusBoundaryStatus.Exact or
            SymbolicCalculusBoundaryStatus.ExactRemovableLimit) &&
        ExactValue.HasValue && RuleId is not null &&
        SourceSemanticSha256 is not null && VariableIdentity is not null;

    public bool HasQualifiedBoundary =>
        (Status is SymbolicCalculusBoundaryStatus.SimplePole or
            SymbolicCalculusBoundaryStatus.OriginalDomainHole or
            SymbolicCalculusBoundaryStatus.ExactRemovableLimit) &&
        ExceptionalPoint.HasValue && RuleId is not null &&
        SourceSemanticSha256 is not null && VariableIdentity is not null;
}

/// <summary>
/// Conservative real symbolic calculus boundary adapter. It delegates exact
/// polynomials to the qualified T1 contract; two hand-provable quotient
/// families classify simple poles versus removable original-domain holes.
/// No principal values, implicit numerical evaluation or arbitrary CAS.
/// </summary>
public static class SymbolicRealCalculusBoundaries
{
    public const string SimplePoleRule = "real-simple-rational-pole/original-domain/v1";
    public const string HoleRule = "real-original-removable-hole/v1";
    public const string QuotientLimitRule = "real-simple-rational-finite-limit/exact/v1";
    public const string RemovableLimitRule = "real-qualified-removable-limit/exact/v1";
    public const string HoleFreeIntegralRule = "real-hole-free-rational-polynomial-integral/exact/v1";
    public const string ZeroQuotientRule = "real-hole-free-zero-quotient-integral/exact/v1";
    public const int MaximumRationalBits = 128;

    public static SymbolicCalculusBoundaryResult AnalyzeIntegral(
        IrNode integrand, IrSymbol variable,
        ExactRational lowerBound, ExactRational upperBound)
    {
        ArgumentNullException.ThrowIfNull(integrand);
        ArgumentNullException.ThrowIfNull(variable);
        if (!FreeReal(variable))
            return Refuse(SymbolicCalculusBoundaryStatus.UnsupportedDomain);
        if (!Bounded(lowerBound) || !Bounded(upperBound))
            return Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
        try
        {
            IrGraphLimits.Validate(integrand);
            var inspected = Inspect(integrand);
            if (inspected != SymbolicCalculusBoundaryStatus.Exact)
                return Refuse(inspected);

            // The previously qualified polynomial gate remains the primary
            // source of exact antiderivatives and arbitrary +C semantics.
            var prior = SymbolicExactPolynomialCalculus.Integrate(
                integrand, variable, lowerBound, upperBound);
            if (prior.IsExact && prior.DefiniteValue is ExactRational value)
                return Exact(integrand, variable, value,
                    SymbolicExactPolynomialCalculus.IntegralRule,
                    prior.ConstantConvention);
            if (prior.Status != SymbolicPolynomialCalculusStatus.Unsupported)
                return Refuse(Map(prior.Status));

            if (integrand is not IrApply { Operation: IrOperation.Divide } quotient ||
                !LinearDenominator(quotient.Arguments[1], variable, out var c))
                return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);

            var inInterval = InsideClosedInterval(c, lowerBound, upperBound);
            if (quotient.Arguments[0] is IrExactScalar numerator)
            {
                if (inInterval)
                    return Boundary(integrand,variable,c,
                        numerator.Value == ExactRational.Zero
                            ? SymbolicCalculusBoundaryStatus.OriginalDomainHole
                            : SymbolicCalculusBoundaryStatus.SimplePole,
                        numerator.Value == ExactRational.Zero ? HoleRule : SimplePoleRule);

                // Zero divided by a nonzero linear expression is identically
                // zero on the entire admitted interval, including its ends.
                if (numerator.Value == ExactRational.Zero)
                    return Exact(integrand,variable,ExactRational.Zero,
                        ZeroQuotientRule,
                        SymbolicExactPolynomialCalculus.AdditiveConstantConvention);
                // A logarithmic primitive is outside the admitted grammar,
                // even when its interval does not touch the pole.
                return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);
            }

            var cancellation=SymbolicRationalCancellationBridge.TryCancel(integrand);
            if (!cancellation.IsAdmitted ||
                cancellation.Conditional?.Expression is not IrNode reduced)
                return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);
            if (inInterval)
                return Boundary(integrand,variable,c,
                    SymbolicCalculusBoundaryStatus.OriginalDomainHole,HoleRule);
            var delegated=SymbolicExactPolynomialCalculus.Integrate(
                reduced,variable,lowerBound,upperBound);
            return delegated.IsExact && delegated.DefiniteValue is ExactRational integral
                ? Exact(integrand,variable,integral,HoleFreeIntegralRule,
                    delegated.ConstantConvention)
                : Refuse(Map(delegated.Status));
        }
        catch(ArgumentOutOfRangeException)
        {
            return Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);
        }
    }

    public static SymbolicCalculusBoundaryResult AnalyzeLimit(
        IrNode expression, IrSymbol variable, ExactRational point)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(variable);
        if (!FreeReal(variable))
            return Refuse(SymbolicCalculusBoundaryStatus.UnsupportedDomain);
        if (!Bounded(point))
            return Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
        try
        {
            IrGraphLimits.Validate(expression);
            var inspected=Inspect(expression);
            if (inspected!=SymbolicCalculusBoundaryStatus.Exact)
                return Refuse(inspected);
            var prior=SymbolicExactPolynomialCalculus.FiniteLimit(
                expression,variable,point);
            if (prior.IsExact && prior.Value is ExactRational polynomialValue)
                return Exact(expression,variable,polynomialValue,
                    SymbolicExactPolynomialCalculus.LimitRule);
            if (prior.Status!=SymbolicPolynomialCalculusStatus.Unsupported)
                return Refuse(Map(prior.Status));
            if(expression is not IrApply {Operation:IrOperation.Divide} quotient ||
               !LinearDenominator(quotient.Arguments[1],variable,out var c))
                return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);

            if(quotient.Arguments[0] is IrExactScalar k)
            {
                if(point==c)
                {
                    if(k.Value!=ExactRational.Zero)
                        return Boundary(expression,variable,c,
                            SymbolicCalculusBoundaryStatus.SimplePole,
                            SimplePoleRule);
                    // 0/(x-c) is undefined at x=c, but the real limit
                    // from either side is exactly 0.
                    return Exact(expression,variable,ExactRational.Zero,
                        RemovableLimitRule,exceptionalPoint:c);
                }
                var delta=point-c;
                if(!Bounded(delta))
                    return Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
                var evaluated=k.Value/delta;
                return Bounded(evaluated)
                    ? Exact(expression,variable,evaluated,QuotientLimitRule)
                    : Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
            }

            var cancellation=SymbolicRationalCancellationBridge.TryCancel(expression);
            if(!cancellation.IsAdmitted ||
                cancellation.Conditional?.Expression is not IrNode reduced)
                return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);
            var limit=SymbolicExactPolynomialCalculus.FiniteLimit(reduced,variable,point);
            if(!limit.IsExact || limit.Value is not ExactRational value)
                return Refuse(Map(limit.Status));
            return Exact(expression,variable,value,
                point==c?RemovableLimitRule:QuotientLimitRule,
                exceptionalPoint:point==c?c:null);
        }
        catch(ArgumentOutOfRangeException)
        {
            return Refuse(SymbolicCalculusBoundaryStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refuse(SymbolicCalculusBoundaryStatus.UnknownConvergence);
        }
    }

    private static SymbolicCalculusBoundaryStatus Inspect(IrNode expression)
    {
        var pending=new Stack<IrNode>();
        pending.Push(expression);
        while(pending.Count>0)
        {
            switch(pending.Pop())
            {
                case IrQuantityLiteral:
                    return SymbolicCalculusBoundaryStatus.UnsupportedUnits;
                case IrRestrictedExpression:
                    return SymbolicCalculusBoundaryStatus.UnsupportedAssumptions;
                case IrFiniteBinary64Scalar:
                    return SymbolicCalculusBoundaryStatus.UnsupportedApproximation;
                case IrSymbol symbol when !FreeReal(symbol):
                    return SymbolicCalculusBoundaryStatus.UnsupportedDomain;
                case IrExactScalar exact when !Bounded(exact.Value):
                    return SymbolicCalculusBoundaryStatus.BudgetExceeded;
                case IrApply application:
                    foreach(var child in application.Arguments)
                        pending.Push(child);
                    break;
            }
        }
        return SymbolicCalculusBoundaryStatus.Exact;
    }

    private static bool LinearDenominator(
        IrNode expression, IrSymbol variable, out ExactRational c)
    {
        c=ExactRational.Zero;
        if(expression is IrSymbol x && Same(x,variable))
            return true;
        if(expression is not IrApply {Operation:IrOperation.Subtract} subtract ||
            subtract.Arguments[0] is not IrSymbol symbol ||
            subtract.Arguments[1] is not IrExactScalar shift ||
            !Same(symbol,variable))
            return false;
        c=shift.Value;
        return Bounded(c);
    }

    private static bool InsideClosedInterval(
        ExactRational point,ExactRational a,ExactRational b) =>
        point >= (a<=b?a:b) && point <= (a>=b?a:b);

    private static bool FreeReal(IrSymbol symbol) =>
        !symbol.IsBound && symbol.Domain==IrScalarDomain.Real;
    private static bool Same(IrSymbol a,IrSymbol b) =>
        FreeReal(a) && FreeReal(b) && a.Identity==b.Identity;
    private static bool Bounded(ExactRational x) =>
        BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
        x.Denominator.GetBitLength()<=MaximumRationalBits;

    private static SymbolicCalculusBoundaryStatus Map(SymbolicPolynomialCalculusStatus status) =>
        status switch
        {
            SymbolicPolynomialCalculusStatus.UnsupportedDomain =>
                SymbolicCalculusBoundaryStatus.UnsupportedDomain,
            SymbolicPolynomialCalculusStatus.UnsupportedUnits =>
                SymbolicCalculusBoundaryStatus.UnsupportedUnits,
            SymbolicPolynomialCalculusStatus.UnsupportedAssumptions =>
                SymbolicCalculusBoundaryStatus.UnsupportedAssumptions,
            SymbolicPolynomialCalculusStatus.UnsupportedApproximation =>
                SymbolicCalculusBoundaryStatus.UnsupportedApproximation,
            SymbolicPolynomialCalculusStatus.BudgetExceeded =>
                SymbolicCalculusBoundaryStatus.BudgetExceeded,
            _ => SymbolicCalculusBoundaryStatus.UnknownConvergence
        };

    private static SymbolicCalculusBoundaryResult Exact(
        IrNode original,IrSymbol variable,ExactRational value,
        string rule,string? constantConvention=null,ExactRational? exceptionalPoint=null) =>
        new(exceptionalPoint.HasValue?
                SymbolicCalculusBoundaryStatus.ExactRemovableLimit:
                SymbolicCalculusBoundaryStatus.Exact,
            value,exceptionalPoint,constantConvention,rule,
            IrCanonicalHashes.SemanticStructuralSha256(original),variable.Identity);

    private static SymbolicCalculusBoundaryResult Boundary(
        IrNode original,IrSymbol variable,ExactRational point,
        SymbolicCalculusBoundaryStatus status,string rule) =>
        new(status,null,point,null,rule,
            IrCanonicalHashes.SemanticStructuralSha256(original),variable.Identity);

    private static SymbolicCalculusBoundaryResult Refuse(SymbolicCalculusBoundaryStatus status) =>
        new(status,null,null,null,null,null,null);
}
