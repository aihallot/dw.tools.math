using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// Bounded exact integer, finite combinatorial and arithmetic sequence primitives.
/// Nonnegative operands are required for GCD/LCM; signed first/difference values
/// are admitted for finite sums. No primality or probabilistic guarantee.
/// </summary>
public static class BoundedExactScientific
{
    public const string RuleId="dw-math-bounded-exact-scientific-1";
    public const int MaximumFactorialArgument=256;
    public const int MaximumBinomialArgument=256;
    public const int MaximumSequenceLength=4096;
    private static readonly ExactScientificBudget DefaultBudget=new();

    public static ExactScientificResult GreatestCommonDivisor(
        BigInteger left, BigInteger right, ExactScientificBudget? budget=null,
        CancellationToken cancellationToken=default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        var limits=budget??DefaultBudget;
        if(!Valid(limits))return Refuse(ExactScientificStatus.InvalidPolicy);
        if(left.Sign<0||right.Sign<0)return Refuse(ExactScientificStatus.InvalidInput);
        if(!WithinOperand(left,limits)||!WithinOperand(right,limits))
            return Refuse(ExactScientificStatus.BudgetExceeded);
        var steps=0;
        while(!right.IsZero)
        {
            if(cancellationToken.IsCancellationRequested)
                return Refuse(ExactScientificStatus.Cancelled);
            if(steps>=limits.MaximumSteps)
                return Refuse(ExactScientificStatus.BudgetExceeded);
            var remainder=left%right;
            left=right;
            right=remainder;
            steps++;
        }
        return Admit(left,steps,limits);
    }

    public static ExactScientificResult LeastCommonMultiple(
        BigInteger left, BigInteger right, ExactScientificBudget? budget=null,
        CancellationToken cancellationToken=default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        var limits=budget??DefaultBudget;
        if(!Valid(limits))return Refuse(ExactScientificStatus.InvalidPolicy);
        if(left.Sign<0||right.Sign<0)return Refuse(ExactScientificStatus.InvalidInput);
        if(!WithinOperand(left,limits)||!WithinOperand(right,limits))
            return Refuse(ExactScientificStatus.BudgetExceeded);
        if(left.IsZero||right.IsZero)return Admit(BigInteger.Zero,0,limits);
        var divisor=GreatestCommonDivisor(left,right,limits,cancellationToken);
        if(!divisor.Succeeded)return divisor;
        if(divisor.Steps>=limits.MaximumSteps)
            return Refuse(ExactScientificStatus.BudgetExceeded);
        var gcd=divisor.Value!.Value;
        return Admit((left/gcd)*right,divisor.Steps+1,limits);
    }

    public static ExactScientificResult Factorial(
        int argument, ExactScientificBudget? budget=null,
        CancellationToken cancellationToken=default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        var limits=budget??DefaultBudget;
        if(!Valid(limits))return Refuse(ExactScientificStatus.InvalidPolicy);
        if(argument<0)return Refuse(ExactScientificStatus.InvalidInput);
        if(argument>MaximumFactorialArgument||
           argument-1>limits.MaximumSteps)
            return Refuse(ExactScientificStatus.BudgetExceeded);
        var value=BigInteger.One;
        var steps=0;
        for(var i=2;i<=argument;i++)
        {
            if(cancellationToken.IsCancellationRequested)
                return Refuse(ExactScientificStatus.Cancelled);
            value*=i;
            steps++;
            if(!WithinResult(value,limits))
                return Refuse(ExactScientificStatus.ResultTooLarge);
        }
        return Admit(value,steps,limits);
    }

    public static ExactScientificResult Binomial(
        int n,int k, ExactScientificBudget? budget=null,
        CancellationToken cancellationToken=default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        var limits=budget??DefaultBudget;
        if(!Valid(limits))return Refuse(ExactScientificStatus.InvalidPolicy);
        if(n<0||k<0||k>n)return Refuse(ExactScientificStatus.InvalidInput);
        if(n>MaximumBinomialArgument)
            return Refuse(ExactScientificStatus.BudgetExceeded);
        var iterations=global::System.Math.Min(k,n-k);
        if(iterations>limits.MaximumSteps)
            return Refuse(ExactScientificStatus.BudgetExceeded);
        var value=BigInteger.One;
        for(var i=1;i<=iterations;i++)
        {
            if(cancellationToken.IsCancellationRequested)
                return Refuse(ExactScientificStatus.Cancelled);
            value=value*(n-iterations+i)/i;
            if(!WithinResult(value,limits))
                return Refuse(ExactScientificStatus.ResultTooLarge);
        }
        return Admit(value,iterations,limits);
    }

    public static ExactScientificResult SumArithmeticSequence(
        BigInteger first,BigInteger difference,int count,
        ExactScientificBudget? budget=null,
        CancellationToken cancellationToken=default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        var limits=budget??DefaultBudget;
        if(!Valid(limits))return Refuse(ExactScientificStatus.InvalidPolicy);
        if(count<0)return Refuse(ExactScientificStatus.InvalidInput);
        if(count>MaximumSequenceLength||count>limits.MaximumSteps||
           !WithinOperand(first,limits)||!WithinOperand(difference,limits))
            return Refuse(ExactScientificStatus.BudgetExceeded);
        if(count==0)return Admit(BigInteger.Zero,0,limits);
        // Closed exact formula: n*a + (n*(n-1)/2)*d. No array allocation.
        var n=new BigInteger(count);
        var pairs=(n*(n-1))/2;
        var value=n*first+pairs*difference;
        return Admit(value,count,limits);
    }

    private static bool Valid(ExactScientificBudget limits) =>
        limits.MaximumOperandBits is >=1 and <=4096 &&
        limits.MaximumResultBits is >=1 and <=16384 &&
        limits.MaximumSteps is >=1 and <=4096;

    private static bool WithinOperand(BigInteger value,ExactScientificBudget limits) =>
        BigInteger.Abs(value).GetBitLength()<=limits.MaximumOperandBits;

    private static bool WithinResult(BigInteger value,ExactScientificBudget limits) =>
        BigInteger.Abs(value).GetBitLength()<=limits.MaximumResultBits;

    private static ExactScientificResult Admit(
        BigInteger value,int steps,ExactScientificBudget limits) =>
        WithinResult(value,limits)
            ? new(ExactScientificStatus.Success,value,steps,RuleId)
            : Refuse(ExactScientificStatus.ResultTooLarge);

    private static ExactScientificResult Refuse(ExactScientificStatus status) =>
        new(status,null,0,RuleId);
}
