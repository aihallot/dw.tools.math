using System.Collections.Immutable;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>No verified candidate is not a proof that an equation has no roots.</summary>
public enum SymbolicRootAuditStatus
{
    Complete,
    VerifiedSubset,
    NoVerifiedCandidates,
    Unknown,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    BudgetExceeded
}

/// <summary>
/// The multiplicity is null unless independently justified by the exact
/// elementary T1 equation grammar. A root is never inferred from cancellation.
/// </summary>
public sealed record SymbolicAuditedRoot(ExactRational Value, int? Multiplicity);

/// <summary>
/// Verified roots satisfy the ORIGINAL relation at their exact rational points.
/// Excluded points violate the original domain; rejected points are defined but
/// fail the equation. An unresolved candidate is neither verified nor rejected.
/// Only Complete asserts that all real solutions have been accounted for.
/// </summary>
public sealed record SymbolicRootAuditResult(
    SymbolicRootAuditStatus Status,
    ImmutableArray<SymbolicAuditedRoot> VerifiedRoots,
    ImmutableArray<ExactRational> ExcludedFromDomain,
    ImmutableArray<ExactRational> RejectedCandidates,
    ImmutableArray<ExactRational> UnresolvedCandidates,
    bool ProvenEmpty,
    string? RuleId,
    string? LeftSemanticSha256,
    string? RightSemanticSha256,
    string? VariableIdentity)
{
    public bool IsComplete => Status == SymbolicRootAuditStatus.Complete;
    public bool HasVerifiedRoots => VerifiedRoots.Length > 0 &&
        Status is SymbolicRootAuditStatus.Complete or SymbolicRootAuditStatus.VerifiedSubset;
}

/// <summary>
/// Bounded, exact candidate substitution into an ORIGINAL real equality.
/// Does not treat candidates, a simplified quotient or missing rational roots
/// as exhaustive without a separate qualified T1 complete-set certificate.
/// </summary>
public static class SymbolicRootAudit
{
    public const string Rule = "real-original-equation-candidate-audit/exact/v1";
    public const int MaximumCandidates = 8;
    public const int MaximumEvaluationDepth = 16;
    public const int MaximumRationalBits = 128;
    public const int MaximumPower = 8;

    public static SymbolicRootAuditResult Verify(
        IrNode originalLeft, IrNode originalRight,
        IrSymbol variable, IEnumerable<ExactRational> proposedCandidates)
    {
        ArgumentNullException.ThrowIfNull(originalLeft);
        ArgumentNullException.ThrowIfNull(originalRight);
        ArgumentNullException.ThrowIfNull(variable);
        ArgumentNullException.ThrowIfNull(proposedCandidates);
        if (variable.IsBound || variable.Domain != IrScalarDomain.Real)
            return Refused(SymbolicRootAuditStatus.UnsupportedDomain);
        try
        {
            IrGraphLimits.Validate(originalLeft);
            IrGraphLimits.Validate(originalRight);
            var seenNodes=new Stack<IrNode>();
            seenNodes.Push(originalLeft);
            seenNodes.Push(originalRight);
            while(seenNodes.Count>0)
            {
                var node=seenNodes.Pop();
                switch(node)
                {
                    case IrQuantityLiteral:
                        return Refused(SymbolicRootAuditStatus.UnsupportedUnits);
                    case IrExactScalar scalar when !Bounded(scalar.Value):
                        return Refused(SymbolicRootAuditStatus.BudgetExceeded);
                    case IrRestrictedExpression restricted:
                        foreach(var c in restricted.Assumptions.Conditions)
                            if(!Same(c.Symbol,variable) || !Bounded(c.Right.Value))
                                return Refused(SymbolicRootAuditStatus.UnsupportedAssumptions);
                        seenNodes.Push(restricted.Expression);
                        break;
                    case IrApply operation:
                        foreach(var child in operation.Arguments)
                            seenNodes.Push(child);
                        break;
                }
            }
            var input=proposedCandidates.Take(MaximumCandidates+1).ToArray();
            if(input.Length>MaximumCandidates ||
               input.Any(x=>!Bounded(x)))
                return Refused(SymbolicRootAuditStatus.BudgetExceeded);
            if(input.Distinct().Count()!=input.Length)
                return Refused(SymbolicRootAuditStatus.Unsupported);

            var verified=ImmutableArray.CreateBuilder<SymbolicAuditedRoot>();
            var excluded=ImmutableArray.CreateBuilder<ExactRational>();
            var rejected=ImmutableArray.CreateBuilder<ExactRational>();
            var unresolved=ImmutableArray.CreateBuilder<ExactRational>();

            // Completeness and algebraic multiplicity are ONLY inherited from
            // the exact original-form T1 contract; this never solves a quotient.
            var prior=SymbolicExactRealSolving.Solve(originalLeft,
                IrRelationKind.Equal,originalRight,variable);

            foreach(var point in input)
            {
                var lhs=Evaluate(originalLeft,variable,point,1);
                var rhs=Evaluate(originalRight,variable,point,1);
                if(lhs.Status==EvaluationStatus.BudgetExceeded ||
                   rhs.Status==EvaluationStatus.BudgetExceeded)
                    return Refused(SymbolicRootAuditStatus.BudgetExceeded);
                if(lhs.Status==EvaluationStatus.Excluded ||
                   rhs.Status==EvaluationStatus.Excluded)
                {
                    excluded.Add(point);
                    continue;
                }
                if(lhs.Status!=EvaluationStatus.Exact ||
                   rhs.Status!=EvaluationStatus.Exact ||
                   !lhs.Value.HasValue || !rhs.Value.HasValue)
                {
                    unresolved.Add(point);
                    continue;
                }
                if(lhs.Value.Value != rhs.Value.Value)
                {
                    rejected.Add(point);
                    continue;
                }
                var multiplicity=ProvenMultiplicity(originalLeft,originalRight,
                    variable,point,prior);
                verified.Add(new SymbolicAuditedRoot(point,multiplicity));
            }

            // A complete empty set is asserted ONLY by a complete T1
            // original-form certificate, never by an empty candidate list.
            var globallyEmpty=prior.IsComplete &&
                prior.Status==SymbolicRealSolvingStatus.Empty;
            var originalFinite=prior.IsComplete &&
                prior.Status==SymbolicRealSolvingStatus.Finite;
            var allFiniteAccountedFor=originalFinite &&
                verified.Count==prior.Roots.Length &&
                prior.Roots.All(root=>verified.Any(v=>v.Value==root));
            var complete=(globallyEmpty && verified.Count==0 &&
                unresolved.Count==0) ||
                (allFiniteAccountedFor && unresolved.Count==0);
            var status=complete ? SymbolicRootAuditStatus.Complete
                : verified.Count>0 ? SymbolicRootAuditStatus.VerifiedSubset
                : unresolved.Count>0 || input.Length==0 ? SymbolicRootAuditStatus.Unknown
                : SymbolicRootAuditStatus.NoVerifiedCandidates;

            var holdsProof=status is SymbolicRootAuditStatus.Complete
                or SymbolicRootAuditStatus.VerifiedSubset;
            return new(status,verified.ToImmutable(),excluded.ToImmutable(),
                rejected.ToImmutable(),unresolved.ToImmutable(),
                complete && globallyEmpty,
                holdsProof?Rule:null,
                holdsProof?IrCanonicalHashes.SemanticStructuralSha256(originalLeft):null,
                holdsProof?IrCanonicalHashes.SemanticStructuralSha256(originalRight):null,
                holdsProof?variable.Identity:null);
        }
        catch(ArgumentOutOfRangeException)
        {
            return Refused(SymbolicRootAuditStatus.BudgetExceeded);
        }
        catch(Exception error) when(error is ArgumentException or
            FormatException or NotSupportedException)
        {
            return Refused(SymbolicRootAuditStatus.Unsupported);
        }
    }

    private enum EvaluationStatus { Exact, Excluded, Unknown, BudgetExceeded }

    private readonly record struct Evaluation(
        EvaluationStatus Status,ExactRational? Value);

    private static Evaluation Evaluate(
        IrNode expression,IrSymbol target,ExactRational point,int depth)
    {
        if(depth>MaximumEvaluationDepth)
            return new(EvaluationStatus.BudgetExceeded,null);
        switch(expression)
        {
            case IrExactScalar constant:
                return Bounded(constant.Value)
                    ? new(EvaluationStatus.Exact,constant.Value)
                    : new(EvaluationStatus.BudgetExceeded,null);
            case IrSymbol symbol:
                return Same(symbol,target)
                    ? new(EvaluationStatus.Exact,point)
                    : new(EvaluationStatus.Unknown,null);
            case IrRestrictedExpression restricted:
            {
                foreach(var condition in restricted.Assumptions.Conditions)
                {
                    if(!Same(condition.Symbol,target))
                        return new(EvaluationStatus.Unknown,null);
                    var threshold=condition.Right.Value;
                    var accepted=condition.Kind switch
                    {
                        IrRelationKind.Equal => point==threshold,
                        IrRelationKind.NotEqual => point!=threshold,
                        IrRelationKind.GreaterOrEqual => point>=threshold,
                        _ => false
                    };
                    if(!accepted)
                        return new(EvaluationStatus.Excluded,null);
                }
                return Evaluate(restricted.Expression,target,point,depth+1);
            }
            case IrApply app:
            {
                if(app.Operation is not (IrOperation.Add or IrOperation.Subtract
                    or IrOperation.Multiply or IrOperation.Divide
                    or IrOperation.Square or IrOperation.Power or IrOperation.Abs))
                    return new(EvaluationStatus.Unknown,null);
                var values=new ExactRational[app.Arguments.Length];
                for(var i=0;i<values.Length;i++)
                {
                    var child=Evaluate(app.Arguments[i],target,point,depth+1);
                    if(child.Status!=EvaluationStatus.Exact ||
                        child.Value is not ExactRational value)
                        return child;
                    values[i]=value;
                }
                ExactRational result;
                switch(app.Operation)
                {
                    case IrOperation.Add:
                        result=ExactRational.Zero;
                        foreach(var number in values)
                        {
                            result+=number;
                            if(!Bounded(result))
                                return new(EvaluationStatus.BudgetExceeded,null);
                        }
                        break;
                    case IrOperation.Subtract:
                        result=values[0]-values[1];
                        break;
                    case IrOperation.Multiply:
                        result=ExactRational.One;
                        foreach(var number in values)
                        {
                            result*=number;
                            if(!Bounded(result))
                                return new(EvaluationStatus.BudgetExceeded,null);
                        }
                        break;
                    case IrOperation.Divide:
                        if(values[1]==ExactRational.Zero)
                            return new(EvaluationStatus.Excluded,null);
                        result=values[0]/values[1];
                        break;
                    case IrOperation.Square:
                        result=values[0]*values[0];
                        break;
                    case IrOperation.Abs:
                        result=values[0].Abs();
                        break;
                    case IrOperation.Power:
                    {
                        var exponent=values[1];
                        if(exponent.Denominator!=BigInteger.One ||
                            exponent.Numerator<0 || exponent.Numerator>MaximumPower)
                            return new(EvaluationStatus.Unknown,null);
                        result=ExactRational.One;
                        for(var i=0;i<(int)exponent.Numerator;i++)
                        {
                            result*=values[0];
                            if(!Bounded(result))
                                return new(EvaluationStatus.BudgetExceeded,null);
                        }
                        break;
                    }
                    default:
                        return new(EvaluationStatus.Unknown,null);
                }
                return Bounded(result)
                    ? new(EvaluationStatus.Exact,result)
                    : new(EvaluationStatus.BudgetExceeded,null);
            }
            default:
                return new(EvaluationStatus.Unknown,null);
        }
    }

    private static int? ProvenMultiplicity(
        IrNode left,IrNode right,IrSymbol variable,ExactRational root,
        SymbolicRealSolvingResult prior)
    {
        if(!prior.IsComplete ||
            prior.Status!=SymbolicRealSolvingStatus.Finite ||
            !prior.Roots.Contains(root))
            return null;
        if((left is IrSymbol l && Same(l,variable) &&
            right is IrExactScalar) ||
           (right is IrSymbol r && Same(r,variable) &&
            left is IrExactScalar))
            return 1;
        if((SquareOf(left,variable) && right is IrExactScalar) ||
           (SquareOf(right,variable) && left is IrExactScalar))
            return root==ExactRational.Zero?2:1;
        return null;
    }

    private static bool SquareOf(IrNode expression,IrSymbol variable) =>
        expression is IrApply {Operation:IrOperation.Square} square
            && square.Arguments[0] is IrSymbol s && Same(s,variable)
        || expression is IrApply {Operation:IrOperation.Power} power
            && power.Arguments[0] is IrSymbol p && Same(p,variable)
            && power.Arguments[1] is IrExactScalar exponent
            && exponent.Value == new ExactRational(2,1);

    private static bool Same(IrSymbol a,IrSymbol b) =>
        !a.IsBound && !b.IsBound && a.Domain==IrScalarDomain.Real &&
        b.Domain==IrScalarDomain.Real && a.Identity==b.Identity;

    private static bool Bounded(ExactRational x) =>
        BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
        x.Denominator.GetBitLength()<=MaximumRationalBits;

    private static SymbolicRootAuditResult Refused(SymbolicRootAuditStatus status) =>
        new(status,ImmutableArray<SymbolicAuditedRoot>.Empty,
            ImmutableArray<ExactRational>.Empty,
            ImmutableArray<ExactRational>.Empty,
            ImmutableArray<ExactRational>.Empty,false,null,null,null,null);
}
