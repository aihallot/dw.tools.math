using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Pointwise exact derivative or a separate scientific refusal.</summary>
public enum SymbolicDerivativePointStatus
{
    ExactValue,
    NonDifferentiable,
    OutsideRealDomain,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    BudgetExceeded
}

/// <summary>
/// A refusal contains no value, proof identifier, or structural provenance.
/// An exact value is never inferred through a numerical finite difference.
/// </summary>
public sealed record SymbolicDerivativePointResult(
    SymbolicDerivativePointStatus Status,
    ExactRational? Value,
    string? RuleId,
    string? SourceSemanticSha256,
    string? VariableIdentity)
{
    public bool HasExactValue => Status == SymbolicDerivativePointStatus.ExactValue &&
        Value.HasValue && RuleId is not null && SourceSemanticSha256 is not null &&
        VariableIdentity is not null;
}

/// <summary>
/// Conservative pointwise analysis over the prior qualified real polynomial
/// derivative and two explicit elementary real-domain boundary forms.
/// It does not infer general analytic derivatives or unit conversions.
/// </summary>
public static class SymbolicDerivativePointAnalysis
{
    public const string PolynomialRule = "pointwise-exact-polynomial-derivative/v1";
    public const string AbsoluteRule = "pointwise-abs-away-from-zero/v1";
    public const string AbsoluteSquareRootRule = "pointwise-sqrt-square-real-away-from-zero/v1";
    public const int MaximumEvaluationDepth = 16;
    public const int MaximumRationalBits = 128;

    public static SymbolicDerivativePointResult Analyze(
        SymbolicRealFunction function, ExactRational point)
    {
        ArgumentNullException.ThrowIfNull(function);
        return Analyze(function.Body,function.Parameter,point);
    }

    public static SymbolicDerivativePointResult Analyze(
        IrNode source,IrSymbol withRespectTo,ExactRational point)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(withRespectTo);

        if (withRespectTo.IsBound || withRespectTo.Domain!=IrScalarDomain.Real)
            return Refuse(SymbolicDerivativePointStatus.UnsupportedDomain);
        if (!Bounded(point))
            return Refuse(SymbolicDerivativePointStatus.BudgetExceeded);
        try
        {
            IrGraphLimits.Validate(source);
            if (source is IrRestrictedExpression)
                return Refuse(SymbolicDerivativePointStatus.UnsupportedAssumptions);
            if (source is IrQuantityLiteral)
                return Refuse(SymbolicDerivativePointStatus.UnsupportedUnits);

            if (source is IrApply {Operation:IrOperation.Abs} abs &&
                abs.Arguments[0] is IrSymbol variable &&
                Same(variable,withRespectTo))
                return AbsoluteAt(source,withRespectTo,point,AbsoluteRule);

            if (source is IrApply {Operation:IrOperation.Sqrt} sqrt)
            {
                if (sqrt.Arguments[0] is IrApply {Operation:IrOperation.Square} square &&
                    square.Arguments[0] is IrSymbol squared &&
                    Same(squared,withRespectTo))
                    return AbsoluteAt(source,withRespectTo,point,AbsoluteSquareRootRule);
                if (sqrt.Arguments[0] is IrSymbol argument &&
                    Same(argument,withRespectTo))
                    return point < ExactRational.Zero
                        ? Refuse(SymbolicDerivativePointStatus.OutsideRealDomain)
                        : point == ExactRational.Zero
                            ? Refuse(SymbolicDerivativePointStatus.NonDifferentiable)
                            : Refuse(SymbolicDerivativePointStatus.Unsupported);
            }

            // A removable singularity is still outside the domain of the
            // original quotient, even when the candidate simplification is
            // algebraically equal at all admitted other points.
            if (source is IrApply {Operation:IrOperation.Divide})
            {
                var cancellation=SymbolicRationalCancellationBridge.TryCancel(source);
                if (cancellation.IsAdmitted &&
                    SymbolicRationalCancellationBridge.EvaluateExactAt(
                        cancellation,withRespectTo,point).Status==SymbolicPointStatus.Excluded)
                    return Refuse(SymbolicDerivativePointStatus.OutsideRealDomain);
                return Refuse(SymbolicDerivativePointStatus.Unsupported);
            }

            var derivative=SymbolicExactDifferentiation.TryDifferentiate(
                source,withRespectTo);
            if (!derivative.IsAdmitted || derivative.Derivative is not IrNode proven)
                return Refuse(derivative.Status switch
                {
                    SymbolicDifferentiationStatus.UnsupportedDomain =>
                        SymbolicDerivativePointStatus.UnsupportedDomain,
                    SymbolicDifferentiationStatus.UnsupportedUnits =>
                        SymbolicDerivativePointStatus.UnsupportedUnits,
                    SymbolicDifferentiationStatus.UnsupportedAssumptions =>
                        SymbolicDerivativePointStatus.UnsupportedAssumptions,
                    SymbolicDifferentiationStatus.BudgetExceeded =>
                        SymbolicDerivativePointStatus.BudgetExceeded,
                    _ => SymbolicDerivativePointStatus.Unsupported
                });

            var evaluated=EvaluateExact(proven,withRespectTo,point,1);
            if (evaluated.Status!=SymbolicDerivativePointStatus.ExactValue ||
                evaluated.Value is not ExactRational value)
                return Refuse(evaluated.Status);
            return Admit(source,withRespectTo,value,PolynomialRule);
        }
        catch(ArgumentOutOfRangeException)
        {
            return Refuse(SymbolicDerivativePointStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refuse(SymbolicDerivativePointStatus.Unsupported);
        }
    }

    private static SymbolicDerivativePointResult AbsoluteAt(
        IrNode source,IrSymbol variable,ExactRational point,string rule) =>
        point==ExactRational.Zero
            ? Refuse(SymbolicDerivativePointStatus.NonDifferentiable)
            : Admit(source,variable,
                point>ExactRational.Zero?ExactRational.One:
                    new ExactRational(-1,1),rule);

    private static (SymbolicDerivativePointStatus Status,ExactRational? Value)
        EvaluateExact(IrNode node,IrSymbol variable,ExactRational point,int depth)
    {
        if(depth>MaximumEvaluationDepth)
            return(SymbolicDerivativePointStatus.BudgetExceeded,null);
        switch(node)
        {
            case IrExactScalar exact:
                return Bounded(exact.Value)
                    ?(SymbolicDerivativePointStatus.ExactValue,exact.Value)
                    :(SymbolicDerivativePointStatus.BudgetExceeded,null);
            case IrSymbol symbol:
                return Same(symbol,variable)
                    ?(SymbolicDerivativePointStatus.ExactValue,point)
                    :(SymbolicDerivativePointStatus.Unsupported,null);
            case IrApply {Operation:IrOperation.Square} square:
            {
                var operand=EvaluateExact(square.Arguments[0],variable,point,depth+1);
                if (operand.Status!=SymbolicDerivativePointStatus.ExactValue ||
                    operand.Value is not ExactRational r)
                    return operand;
                var value=r*r;
                return Bounded(value)
                    ?(SymbolicDerivativePointStatus.ExactValue,value)
                    :(SymbolicDerivativePointStatus.BudgetExceeded,null);
            }
            case IrApply {Operation:IrOperation.Power} power:
            {
                if(power.Arguments[1] is not IrExactScalar exponent ||
                   exponent.Value.Denominator!=BigInteger.One ||
                   exponent.Value.Numerator<1)
                    return(SymbolicDerivativePointStatus.Unsupported,null);
                if(exponent.Value.Numerator>SymbolicExactDifferentiation.MaximumPower)
                    return(SymbolicDerivativePointStatus.BudgetExceeded,null);
                var operand=EvaluateExact(power.Arguments[0],variable,point,depth+1);
                if(operand.Status!=SymbolicDerivativePointStatus.ExactValue ||
                   operand.Value is not ExactRational factor)
                    return operand;
                var total=ExactRational.One;
                for(var i=0;i<(int)exponent.Value.Numerator;i++)
                {
                    total*=factor;
                    if(!Bounded(total))
                        return(SymbolicDerivativePointStatus.BudgetExceeded,null);
                }
                return(SymbolicDerivativePointStatus.ExactValue,total);
            }
            case IrApply {Operation:IrOperation.Add} add:
            {
                var sum=ExactRational.Zero;
                foreach(var term in add.Arguments)
                {
                    var operand=EvaluateExact(term,variable,point,depth+1);
                    if(operand.Status!=SymbolicDerivativePointStatus.ExactValue ||
                       operand.Value is not ExactRational value)
                        return operand;
                    sum+=value;
                    if(!Bounded(sum))
                        return(SymbolicDerivativePointStatus.BudgetExceeded,null);
                }
                return(SymbolicDerivativePointStatus.ExactValue,sum);
            }
            case IrApply {Operation:IrOperation.Subtract} subtract:
            {
                var left=EvaluateExact(subtract.Arguments[0],variable,point,depth+1);
                if(left.Status!=SymbolicDerivativePointStatus.ExactValue ||
                   left.Value is not ExactRational a)
                    return left;
                var right=EvaluateExact(subtract.Arguments[1],variable,point,depth+1);
                if(right.Status!=SymbolicDerivativePointStatus.ExactValue ||
                   right.Value is not ExactRational b)
                    return right;
                var value=a-b;
                return Bounded(value)
                    ?(SymbolicDerivativePointStatus.ExactValue,value)
                    :(SymbolicDerivativePointStatus.BudgetExceeded,null);
            }
            case IrApply {Operation:IrOperation.Multiply} multiply:
            {
                var product=ExactRational.One;
                foreach(var operandNode in multiply.Arguments)
                {
                    var operand=EvaluateExact(operandNode,variable,point,depth+1);
                    if(operand.Status!=SymbolicDerivativePointStatus.ExactValue ||
                       operand.Value is not ExactRational value)
                        return operand;
                    product*=value;
                    if(!Bounded(product))
                        return(SymbolicDerivativePointStatus.BudgetExceeded,null);
                }
                return(SymbolicDerivativePointStatus.ExactValue,product);
            }
            default:
                return(SymbolicDerivativePointStatus.Unsupported,null);
        }
    }

    private static SymbolicDerivativePointResult Admit(
        IrNode source,IrSymbol variable,ExactRational value,string rule) =>
        new(SymbolicDerivativePointStatus.ExactValue,value,rule,
            IrCanonicalHashes.SemanticStructuralSha256(source),variable.Identity);

    private static SymbolicDerivativePointResult Refuse(SymbolicDerivativePointStatus status) =>
        new(status,null,null,null,null);

    private static bool Same(IrSymbol a,IrSymbol b) =>
        !a.IsBound && !b.IsBound &&
        a.Domain==IrScalarDomain.Real && b.Domain==IrScalarDomain.Real &&
        a.Identity==b.Identity;

    private static bool Bounded(ExactRational value) =>
        BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits &&
        value.Denominator.GetBitLength()<=MaximumRationalBits;
}
