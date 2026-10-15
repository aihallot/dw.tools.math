using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Accepted exact derivative or a typed scientific refusal.</summary>
public enum SymbolicDifferentiationStatus
{
    Admitted,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    BudgetExceeded
}

/// <summary>
/// One explicitly real-to-real formal function. The symbolic body may
/// contain other free real symbols treated as independent parameters.
/// </summary>
public sealed class SymbolicRealFunction
{
    public SymbolicRealFunction(
        IrSymbol parameter, IrNode body,
        IrScalarDomain domain = IrScalarDomain.Real,
        IrScalarDomain codomain = IrScalarDomain.Real)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentNullException.ThrowIfNull(body);
        if(domain != IrScalarDomain.Real || codomain != IrScalarDomain.Real ||
           parameter.IsBound || parameter.Domain != IrScalarDomain.Real)
            throw new NotSupportedException(
                "This formal function admits one free real parameter and a real codomain only.");
        IrGraphLimits.Validate(body);
        Parameter = parameter;
        Body = body;
        Domain = domain;
        Codomain = codomain;
    }

    public IrSymbol Parameter { get; }
    public IrNode Body { get; }
    public IrScalarDomain Domain { get; }
    public IrScalarDomain Codomain { get; }
}

/// <summary>A failed computation never supplies a derivative or proof claim.</summary>
public sealed record SymbolicDerivativeResult(
    SymbolicDifferentiationStatus Status,
    IrNode? Derivative,
    string? ProofRule,
    string? SourceSemanticSha256,
    string? DifferentiationVariableIdentity)
{
    public bool IsAdmitted =>
        Status == SymbolicDifferentiationStatus.Admitted &&
        Derivative is not null && ProofRule is not null &&
        SourceSemanticSha256 is not null &&
        DifferentiationVariableIdentity is not null;
}

/// <summary>
/// Closed exact, provider-neutral polynomial differentiation over free real
/// IR symbols, canonical rationals and elementary sums/constant products.
/// This is not an analytic-function or general symbolic CAS derivative.
/// </summary>
public static class SymbolicExactDifferentiation
{
    public const string Rule = "formal-polynomial-derivative/exact/v1";
    public const int MaximumPower = 8;
    public const int MaximumRecursionDepth = 16;

    private static readonly IrExactScalar Zero = new(ExactRational.Zero);
    private static readonly IrExactScalar One = new(ExactRational.One);

    public static SymbolicDerivativeResult TryDifferentiate(SymbolicRealFunction function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return TryDifferentiate(function.Body, function.Parameter);
    }

    public static SymbolicDerivativeResult TryDifferentiate(
        IrNode expression, IrSymbol withRespectTo)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(withRespectTo);
        if(!FreeReal(withRespectTo))
            return Refused(SymbolicDifferentiationStatus.UnsupportedDomain);
        try
        {
            IrGraphLimits.Validate(expression);
            var result = Differentiate(expression,withRespectTo,1);
            var output = result.Value;
            if(result.Status!=SymbolicDifferentiationStatus.Admitted ||
               output is null)
                return Refused(result.Status);
            // Validate and copy through the existing closed math-ir/1 transport.
            // This does not convert a semantic input digest into a CAS proof.
            var derivative=IrCanonicalJsonCodec.Decode(
                IrCanonicalJsonCodec.Encode(output));
            return new(SymbolicDifferentiationStatus.Admitted,derivative,Rule,
                IrCanonicalHashes.SemanticStructuralSha256(expression),
                withRespectTo.Identity);
        }
        catch(ArgumentOutOfRangeException)
        {
            return Refused(SymbolicDifferentiationStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refused(SymbolicDifferentiationStatus.Unsupported);
        }
    }

    private static (SymbolicDifferentiationStatus Status, IrNode? Value)
        Differentiate(IrNode node, IrSymbol target, int depth)
    {
        if(depth>MaximumRecursionDepth)
            return (SymbolicDifferentiationStatus.BudgetExceeded,null);

        switch(node)
        {
            case IrExactScalar:
                return Valid(Zero);
            case IrSymbol symbol:
                return FreeReal(symbol)
                    ? Valid(Same(symbol,target)?One:Zero)
                    : (SymbolicDifferentiationStatus.UnsupportedDomain,null);
            case IrQuantityLiteral:
                return (SymbolicDifferentiationStatus.UnsupportedUnits,null);
            case IrRestrictedExpression:
                // Never erase source restrictions to claim a derivative.
                return (SymbolicDifferentiationStatus.UnsupportedAssumptions,null);
            case IrApply { Operation: IrOperation.Square } square:
                return DifferentiatePower(square.Arguments[0],2,target);
            case IrApply { Operation: IrOperation.Power } power:
                if(power.Arguments[1] is not IrExactScalar exponent ||
                   exponent.Value.Denominator!=BigInteger.One)
                    return (SymbolicDifferentiationStatus.Unsupported,null);
                if(exponent.Value.Numerator<1)
                    return (SymbolicDifferentiationStatus.Unsupported,null);
                if(exponent.Value.Numerator>MaximumPower)
                    return (SymbolicDifferentiationStatus.BudgetExceeded,null);
                return DifferentiatePower(power.Arguments[0],
                    (int)exponent.Value.Numerator,target);
            case IrApply { Operation: IrOperation.Add } add:
            {
                var operands=new List<IrNode>();
                foreach(var argument in add.Arguments)
                {
                    var differentiated=Differentiate(argument,target,depth+1);
                    var output = differentiated.Value;
                    if(differentiated.Status!=SymbolicDifferentiationStatus.Admitted ||
                       output is null)
                        return differentiated;
                    if(!IsZero(output))
                        operands.Add(output);
                }
                return Valid(operands.Count switch
                {
                    0=>Zero,
                    1=>operands[0],
                    _=>IrApply.Create(IrOperation.Add,operands)
                });
            }
            case IrApply { Operation: IrOperation.Subtract } subtract:
            {
                var left=Differentiate(subtract.Arguments[0],target,depth+1);
                var leftValue=left.Value;
                if(left.Status!=SymbolicDifferentiationStatus.Admitted ||
                   leftValue is null)
                    return left;
                var right=Differentiate(subtract.Arguments[1],target,depth+1);
                var rightValue=right.Value;
                if(right.Status!=SymbolicDifferentiationStatus.Admitted ||
                   rightValue is null)
                    return right;
                if(IsZero(rightValue))
                    return Valid(leftValue);
                return Valid(IrApply.Create(IrOperation.Subtract,[leftValue,rightValue]));
            }
            case IrApply { Operation: IrOperation.Multiply } multiply:
            {
                if(multiply.Arguments.Length!=2)
                    return (SymbolicDifferentiationStatus.Unsupported,null);
                IrExactScalar? factor=null;
                IrNode? polynomial=null;
                if(multiply.Arguments[0] is IrExactScalar left)
                {
                    factor=left;
                    polynomial=multiply.Arguments[1];
                }
                else if(multiply.Arguments[1] is IrExactScalar right)
                {
                    factor=right;
                    polynomial=multiply.Arguments[0];
                }
                if(factor is null || polynomial is null)
                    return (SymbolicDifferentiationStatus.Unsupported,null);
                var inner=Differentiate(polynomial,target,depth+1);
                var innerValue = inner.Value;
                if(inner.Status!=SymbolicDifferentiationStatus.Admitted ||
                   innerValue is null)
                    return inner;
                if(factor.Value==ExactRational.Zero || IsZero(innerValue))
                    return Valid(Zero);
                if(factor.Value==ExactRational.One)
                    return Valid(innerValue);
                if(innerValue is IrExactScalar exact)
                    return Valid(new IrExactScalar(factor.Value*exact.Value));
                return Valid(IrApply.Create(IrOperation.Multiply,
                    [factor,innerValue]));
            }
            default:
                // No silent differentiation of sqrt, |x|, division, binary64,
                // piecewise functions, foreign AST or unknown analytic forms.
                return (SymbolicDifferentiationStatus.Unsupported,null);
        }
    }

    private static (SymbolicDifferentiationStatus Status, IrNode? Value)
        DifferentiatePower(IrNode baseNode,int exponent,IrSymbol target)
    {
        if(baseNode is not IrSymbol symbol)
            return (SymbolicDifferentiationStatus.Unsupported,null);
        if(!FreeReal(symbol))
            return (SymbolicDifferentiationStatus.UnsupportedDomain,null);
        if(!Same(symbol,target))
            return Valid(Zero);
        if(exponent==1)
            return Valid(One);
        var coefficient=new IrExactScalar(new ExactRational(exponent,1));
        var remainder=exponent==2
            ? (IrNode)symbol
            : IrApply.Create(IrOperation.Power,[
                symbol,new IrExactScalar(new ExactRational(exponent-1,1))]);
        return Valid(IrApply.Create(IrOperation.Multiply,[coefficient,remainder]));
    }

    private static bool FreeReal(IrSymbol symbol)=>
        !symbol.IsBound && symbol.Domain==IrScalarDomain.Real;

    private static bool Same(IrSymbol a,IrSymbol b)=>
        a.Identity==b.Identity && a.Domain==b.Domain &&
        FreeReal(a) && FreeReal(b);

    private static bool IsZero(IrNode value)=>
        value is IrExactScalar scalar && scalar.Value==ExactRational.Zero;

    private static (SymbolicDifferentiationStatus Status, IrNode? Value)
        Valid(IrNode value)=>(SymbolicDifferentiationStatus.Admitted,value);

    private static SymbolicDerivativeResult Refused(SymbolicDifferentiationStatus status)=>
        new(status,null,null,null,null);
}
