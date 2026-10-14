using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>One bounded, provider-free exact symbolic transformation status.</summary>
public enum SymbolicTransformStatus
{
    Admitted,
    Unsupported,
    ContradictoryAssumptions,
    BudgetExceeded
}

/// <summary>A mathematical result has an explicit proof rule or no expression.</summary>
public sealed record SymbolicTransformResult(
    SymbolicTransformStatus Status, IrNode? Expression, string? ProofRule)
{
    public bool IsAdmitted => Status == SymbolicTransformStatus.Admitted &&
        Expression is not null && ProofRule is not null;
}

/// <summary>
/// Closed exact rules. No external CAS, implicit numerical widening, text
/// parser, binder alpha-renaming or general symbolic equivalence is admitted.
/// </summary>
public static class SymbolicExactTransformations
{
    public const int MaximumCoefficientBits = 128;
    public const string ExpandRule = "binomial-square/exact/v1";
    public const string FactorRule = "binomial-factor/exact/v1";
    public const string SubstitutionRule = "free-symbol-substitute/closed/v1";
    public const string SqrtRule = "sqrt-real-square/domain/v1";

    /// <summary>(x+a)^2 -> x^2+2ax+a^2, preserving existing typed assumptions.</summary>
    public static SymbolicTransformResult ExpandSquare(IrNode source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!WithinLimits(source))
            return No(SymbolicTransformStatus.BudgetExceeded);
        var (node, prior) = Unwrap(source);
        if (node is not IrApply { Operation: IrOperation.Square } square ||
            square.Arguments[0] is not IrApply { Operation: IrOperation.Add } add ||
            add.Arguments.Length != 2 ||
            add.Arguments[0] is not IrSymbol x || !FreeReal(x) ||
            add.Arguments[1] is not IrExactScalar constant)
            return No(SymbolicTransformStatus.Unsupported);

        // An admitted exact binomial whose coefficient exceeds the declared
        // 128-bit bound is a budget refusal, not an unsupported expression.
        if (!Small(constant.Value))
            return No(SymbolicTransformStatus.BudgetExceeded);

        var status = CheckConditions(prior, x);
        if (status != SymbolicTransformStatus.Admitted)
            return No(status);
        var a = constant.Value;
        var result = IrApply.Create(IrOperation.Add, [
            IrApply.Create(IrOperation.Square, [x]),
            IrApply.Create(IrOperation.Multiply, [
                new IrExactScalar(new ExactRational(2, 1) * a), x]),
            new IrExactScalar(a * a)]);
        return Wrap(result, prior, ExpandRule);
    }

    /// <summary>x^2+2ax+a^2 -> (x+a)^2, only in the exact canonical shape above.</summary>
    public static SymbolicTransformResult FactorSquare(IrNode source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!WithinLimits(source))
            return No(SymbolicTransformStatus.BudgetExceeded);
        var (node, prior) = Unwrap(source);
        if (node is not IrApply { Operation: IrOperation.Add } sum ||
            sum.Arguments.Length != 3 ||
            sum.Arguments[0] is not IrApply { Operation: IrOperation.Square } square ||
            square.Arguments[0] is not IrSymbol x || !FreeReal(x) ||
            sum.Arguments[1] is not IrApply { Operation: IrOperation.Multiply } middle ||
            middle.Arguments.Length != 2 ||
            middle.Arguments[0] is not IrExactScalar twice ||
            middle.Arguments[1] is not IrSymbol second || !Same(x, second) ||
            sum.Arguments[2] is not IrExactScalar squared)
            return No(SymbolicTransformStatus.Unsupported);

        var a = twice.Value / new ExactRational(2, 1);
        if (!Small(a))
            return No(SymbolicTransformStatus.BudgetExceeded);
        if (squared.Value != a * a)
            return No(SymbolicTransformStatus.Unsupported);
        var status = CheckConditions(prior, x);
        if (status != SymbolicTransformStatus.Admitted)
            return No(status);
        return Wrap(IrApply.Create(IrOperation.Square, [
            IrApply.Create(IrOperation.Add, [x, new IrExactScalar(a)])]),
            prior, FactorRule);
    }

    /// <summary>
    /// Closed capture-avoiding substitution: only exact scalar or free real
    /// symbol replacement in an unbound exact IR Add/Subtract/Multiply/Square
    /// tree. Bound symbols and restricted expressions are refused.
    /// </summary>
    public static SymbolicTransformResult SubstituteFree(
        IrNode source, IrSymbol symbol, IrNode replacement)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(symbol);
        ArgumentNullException.ThrowIfNull(replacement);
        if (!WithinLimits(source) || !WithinLimits(replacement))
            return No(SymbolicTransformStatus.BudgetExceeded);
        if (!FreeReal(symbol) ||
            replacement is not (IrExactScalar or IrSymbol) ||
            replacement is IrSymbol candidate && !FreeReal(candidate) ||
            source is IrRestrictedExpression)
            return No(SymbolicTransformStatus.Unsupported);

        if (!Rewrite(source, symbol, replacement, out var output, out var changed) ||
            !changed || output is null)
            return No(SymbolicTransformStatus.Unsupported);
        return Wrap(output, IrAssumptionSet.Empty, SubstitutionRule);
    }

    /// <summary>
    /// Over real x: sqrt(x^2)=abs(x). Only a typed x>=0 hypothesis permits x,
    /// which remains restricted by its assumptions in the result.
    /// </summary>
    public static SymbolicTransformResult NormalizeRealSqrtSquare(
        IrNode source, IrAssumptionSet assumptions)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(assumptions);
        if (!WithinLimits(source))
            return No(SymbolicTransformStatus.BudgetExceeded);
        if (source is not IrApply { Operation: IrOperation.Sqrt } sqrt ||
            sqrt.Arguments[0] is not IrApply { Operation: IrOperation.Square } square ||
            square.Arguments[0] is not IrSymbol x || !FreeReal(x))
            return No(SymbolicTransformStatus.Unsupported);
        var status=CheckConditions(assumptions,x);
        if(status!=SymbolicTransformStatus.Admitted)
            return No(status);
        var normalized=IrRealDomainRules.NormalizeSqrtOfSquare(sqrt,assumptions);
        return Wrap(normalized,assumptions,SqrtRule);
    }

    private static bool Rewrite(IrNode node, IrSymbol symbol, IrNode replacement,
        out IrNode? result, out bool changed)
    {
        changed=false;
        result=null;
        switch(node)
        {
            case IrExactScalar:
                result=node;
                return true;
            case IrSymbol current when FreeReal(current):
                changed=Same(current,symbol);
                result=changed?replacement:current;
                return true;
            case IrApply apply when apply.Operation is IrOperation.Add or
                IrOperation.Subtract or IrOperation.Multiply or IrOperation.Square:
                var rewritten=new List<IrNode>();
                foreach(var arg in apply.Arguments)
                {
                    if(!Rewrite(arg,symbol,replacement,out var item,out var touched) ||
                        item is null)
                        return false;
                    rewritten.Add(item);
                    changed|=touched;
                }
                if(!changed)return false;
                result=IrApply.Create(apply.Operation,rewritten);
                return true;
            default:
                return false;
        }
    }

    private static SymbolicTransformResult Wrap(
        IrNode node, IrAssumptionSet assumptions, string rule)
    {
        try
        {
            var result=assumptions.Conditions.Length==0
                ? node : new IrRestrictedExpression(node,assumptions);
            var roundtrip=IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(result));
            if(roundtrip is null)return No(SymbolicTransformStatus.Unsupported);
            return new(SymbolicTransformStatus.Admitted,result,rule);
        }
        catch(ArgumentOutOfRangeException)
        {
            return No(SymbolicTransformStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return No(SymbolicTransformStatus.Unsupported);
        }
    }

    private static SymbolicTransformStatus CheckConditions(
        IrAssumptionSet assumptions, IrSymbol variable)
    {
        var conditions=assumptions.Conditions;
        if(conditions.Length>32 || conditions.Any(c=>!Same(c.Symbol,variable)))
            return SymbolicTransformStatus.Unsupported;
        var equal=conditions.Where(c=>c.Kind==IrRelationKind.Equal)
            .Select(c=>c.Right.Value).Distinct().ToArray();
        if(equal.Length>1)
            return SymbolicTransformStatus.ContradictoryAssumptions;
        if(equal.Length==1)
        {
            var value=equal[0];
            if(conditions.Any(c=>c.Kind switch
            {
                IrRelationKind.NotEqual => value==c.Right.Value,
                IrRelationKind.GreaterOrEqual => value<c.Right.Value,
                _ => false
            }))
                return SymbolicTransformStatus.ContradictoryAssumptions;
        }
        return SymbolicTransformStatus.Admitted;
    }

    private static (IrNode, IrAssumptionSet) Unwrap(IrNode node) =>
        node is IrRestrictedExpression wrapper
            ? (wrapper.Expression,wrapper.Assumptions)
            : (node,IrAssumptionSet.Empty);

    private static bool WithinLimits(IrNode node)
    {
        try
        {
            IrGraphLimits.Validate(node);
            return true;
        }
        catch(ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private static bool Small(ExactRational number) =>
        BigInteger.Abs(number.Numerator).GetBitLength() <= MaximumCoefficientBits &&
        number.Denominator.GetBitLength() <= MaximumCoefficientBits;

    private static bool Same(IrSymbol a, IrSymbol b) =>
        a.Identity == b.Identity && a.Domain == b.Domain &&
        !a.IsBound && !b.IsBound;

    private static bool FreeReal(IrSymbol symbol) =>
        !symbol.IsBound && symbol.Domain==IrScalarDomain.Real;

    private static SymbolicTransformResult No(SymbolicTransformStatus status) =>
        new(status,null,null);
}
