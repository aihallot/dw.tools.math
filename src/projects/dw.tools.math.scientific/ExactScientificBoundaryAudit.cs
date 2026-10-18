using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>Explicit caller choice for signed Euclidean arguments.</summary>
public enum SignedOperandPolicy
{
    RejectNegative,
    NormalizeMagnitude
}

/// <summary>Exact integer admission with an auditable optional signed normalization.</summary>
public sealed record ExactScientificAuditResult(
    ExactScientificStatus Status, BigInteger? Value, int Steps,
    bool NegativeOperandsNormalized, long? MaximumIntermediateBitsObserved,
    string RuleId)
{
    public bool Succeeded => Status == ExactScientificStatus.Success &&
        Value is { } && Steps >= 0;
}

/// <summary>
/// Provider-neutral exact BigInteger boundary facade. T1 operations remain intact.
/// Operand sign normalization is opt-in; finite arithmetic-sum intermediates can
/// be bounded independently from the result. No probabilistic primality claim.
/// </summary>
public static class ExactScientificBoundaryAudit
{
    public const string RuleId = "dw-math-exact-scientific-boundary-1";
    public const int DefaultMaximumIntermediateBits = 4096;
    public const int MaximumIntermediateBitLimit = 16384;
    private static readonly ExactScientificBudget DefaultBudget = new();

    public static ExactScientificAuditResult GreatestCommonDivisor(
        BigInteger left, BigInteger right,
        SignedOperandPolicy signPolicy = SignedOperandPolicy.RejectNegative,
        ExactScientificBudget? budget = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        if(!AdmittedSignPolicy(signPolicy))
            return Refuse(ExactScientificStatus.InvalidPolicy);
        var normalized=signPolicy==SignedOperandPolicy.NormalizeMagnitude &&
            (left.Sign<0||right.Sign<0);
        var result=BoundedExactScientific.GreatestCommonDivisor(
            normalized?BigInteger.Abs(left):left,
            normalized?BigInteger.Abs(right):right,budget,cancellationToken);
        return Wrap(result,normalized,null);
    }

    public static ExactScientificAuditResult LeastCommonMultiple(
        BigInteger left, BigInteger right,
        SignedOperandPolicy signPolicy = SignedOperandPolicy.RejectNegative,
        ExactScientificBudget? budget = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        if(!AdmittedSignPolicy(signPolicy))
            return Refuse(ExactScientificStatus.InvalidPolicy);
        var normalized=signPolicy==SignedOperandPolicy.NormalizeMagnitude &&
            (left.Sign<0||right.Sign<0);
        var result=BoundedExactScientific.LeastCommonMultiple(
            normalized?BigInteger.Abs(left):left,
            normalized?BigInteger.Abs(right):right,budget,cancellationToken);
        return Wrap(result,normalized,null);
    }

    public static ExactScientificAuditResult Factorial(
        int argument, ExactScientificBudget? budget = null,
        CancellationToken cancellationToken = default) =>
        Wrap(BoundedExactScientific.Factorial(argument,budget,cancellationToken),
            false,null);

    public static ExactScientificAuditResult Binomial(
        int n,int k,ExactScientificBudget? budget = null,
        CancellationToken cancellationToken = default) =>
        Wrap(BoundedExactScientific.Binomial(n,k,budget,cancellationToken),
            false,null);

    public static ExactScientificAuditResult SumArithmeticSequence(
        BigInteger first,BigInteger difference,int count,
        int maximumIntermediateBits = DefaultMaximumIntermediateBits,
        ExactScientificBudget? budget = null,CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        if(maximumIntermediateBits is < 1 or > MaximumIntermediateBitLimit)
            return Refuse(ExactScientificStatus.InvalidPolicy);
        var limits=budget??DefaultBudget;
        if(!ValidBudget(limits))
            return Refuse(ExactScientificStatus.InvalidPolicy);
        if(count<0)return Refuse(ExactScientificStatus.InvalidInput);
        if(count>BoundedExactScientific.MaximumSequenceLength ||
           count>limits.MaximumSteps ||
           BigInteger.Abs(first).GetBitLength()>limits.MaximumOperandBits ||
           BigInteger.Abs(difference).GetBitLength()>limits.MaximumOperandBits)
            return Refuse(ExactScientificStatus.BudgetExceeded);

        // Do not claim a tiny final result proves its intermediate work was tiny:
        // two large signed products can cancel to exactly zero.
        var n=new BigInteger(count);
        var pairs=n*(n-BigInteger.One)/2;
        var left=n*first;
        var right=pairs*difference;
        var total=left+right;
        var maximum=global::System.Math.Max(
            BigInteger.Abs(left).GetBitLength(),
            global::System.Math.Max(BigInteger.Abs(right).GetBitLength(),
                BigInteger.Abs(total).GetBitLength()));
        if(maximum>maximumIntermediateBits)
            return Refuse(ExactScientificStatus.ResultTooLarge);
        if(cancellationToken.IsCancellationRequested)
            return Refuse(ExactScientificStatus.Cancelled);
        return Wrap(BoundedExactScientific.SumArithmeticSequence(
            first,difference,count,limits,cancellationToken),false,maximum);
    }

    private static bool AdmittedSignPolicy(SignedOperandPolicy policy) =>
        policy is SignedOperandPolicy.RejectNegative or
            SignedOperandPolicy.NormalizeMagnitude;

    private static bool ValidBudget(ExactScientificBudget limits) =>
        limits.MaximumOperandBits is >=1 and <=4096 &&
        limits.MaximumResultBits is >=1 and <=16384 &&
        limits.MaximumSteps is >=1 and <=4096;

    private static ExactScientificAuditResult Wrap(
        ExactScientificResult result,bool normalized,long? intermediateBits) =>
        result.Succeeded && result.Value is { } value
            ? new(ExactScientificStatus.Success,value,result.Steps,
                normalized,intermediateBits,RuleId)
            : Refuse(result.Status);

    private static ExactScientificAuditResult Refuse(ExactScientificStatus status) =>
        new(status,null,0,false,null,RuleId);
}
