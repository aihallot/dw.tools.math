using System.Collections.Immutable;
using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Exact real solution forms. Unknown is never a proof of emptiness.</summary>
public enum SymbolicRealSolvingStatus
{
    Finite,
    Empty,
    LowerClosedRay,
    UpperClosedRay,
    AllReal,
    Unknown,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    BudgetExceeded
}

/// <summary>
/// An admitted result is complete only for its named, proved elementary form.
/// Refusals never carry candidate roots, bounds or proof/provenance claims.
/// </summary>
public sealed record SymbolicRealSolvingResult(
    SymbolicRealSolvingStatus Status,
    ImmutableArray<ExactRational> Roots,
    ExactRational? ClosedBound,
    string? RuleId,
    string? LeftSemanticSha256,
    string? RightSemanticSha256,
    string? VariableIdentity)
{
    public bool IsQualified => Status is
        (SymbolicRealSolvingStatus.Finite or SymbolicRealSolvingStatus.Empty
            or SymbolicRealSolvingStatus.LowerClosedRay
            or SymbolicRealSolvingStatus.UpperClosedRay
            or SymbolicRealSolvingStatus.AllReal)
        && RuleId is not null && LeftSemanticSha256 is not null
        && RightSemanticSha256 is not null && VariableIdentity is not null;

    public bool IsComplete => IsQualified;
}

/// <summary>
/// Provider-free, bounded, exact elementary REAL solving. The admitted grammar
/// is x=c, x^2=c, constant relations and x >= c / c >= x. No other polynomial,
/// quotient, radical, complex, conditional or arbitrary inequality is solved.
/// An irrational real root is UNKNOWN, never an empty real solution set.
/// </summary>
public static class SymbolicExactRealSolving
{
    public const string Rule = "real-elementary-solution-sets/exact/v1";
    public const int MaximumRationalBits = 128;

    public static SymbolicRealSolvingResult Solve(
        IrNode left, IrRelationKind relation, IrNode right,
        IrSymbol withRespectTo, IrScalarDomain domain = IrScalarDomain.Real)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        ArgumentNullException.ThrowIfNull(withRespectTo);
        if (domain != IrScalarDomain.Real || withRespectTo.IsBound
            || withRespectTo.Domain != IrScalarDomain.Real)
            return Refuse(SymbolicRealSolvingStatus.UnsupportedDomain);
        if (!Enum.IsDefined(relation) || relation is IrRelationKind.NotEqual)
            return Refuse(SymbolicRealSolvingStatus.Unsupported);
        try
        {
            IrGraphLimits.Validate(left);
            IrGraphLimits.Validate(right);
            var nodes = new Stack<IrNode>();
            nodes.Push(left);
            nodes.Push(right);
            var hasUnits = false;
            var hasRestrictions = false;
            while (nodes.Count != 0)
            {
                switch (nodes.Pop())
                {
                    case IrQuantityLiteral:
                        hasUnits = true;
                        break;
                    case IrRestrictedExpression restricted:
                        hasRestrictions = true;
                        nodes.Push(restricted.Expression);
                        break;
                    case IrExactScalar scalar when !Bounded(scalar.Value):
                        return Refuse(SymbolicRealSolvingStatus.BudgetExceeded);
                    case IrApply expression:
                        foreach (var operand in expression.Arguments)
                            nodes.Push(operand);
                        break;
                }
            }
            if (hasUnits)
                return Refuse(SymbolicRealSolvingStatus.UnsupportedUnits);
            if (hasRestrictions)
                return Refuse(SymbolicRealSolvingStatus.UnsupportedAssumptions);

            if (left is IrExactScalar l && right is IrExactScalar r)
                return Admit(RelationHolds(l.Value, relation, r.Value)
                    ? SymbolicRealSolvingStatus.AllReal
                    : SymbolicRealSolvingStatus.Empty,
                    left, right, withRespectTo);
            if (left is IrSymbol lhs && right is IrSymbol rhs
                && Same(lhs, withRespectTo) && Same(rhs, withRespectTo))
                return Admit(SymbolicRealSolvingStatus.AllReal,
                    left, right, withRespectTo);

            if (relation == IrRelationKind.Equal)
            {
                if (left is IrSymbol linear && Same(linear, withRespectTo)
                    && right is IrExactScalar exactRight)
                    return Admit(SymbolicRealSolvingStatus.Finite,
                        left, right, withRespectTo, roots: [exactRight.Value]);
                if (right is IrSymbol symmetric && Same(symmetric, withRespectTo)
                    && left is IrExactScalar exactLeft)
                    return Admit(SymbolicRealSolvingStatus.Finite,
                        left, right, withRespectTo, roots: [exactLeft.Value]);

                if (SquareOf(left, withRespectTo) && right is IrExactScalar c)
                    return SolveSquare(left, right, withRespectTo, c.Value);
                if (SquareOf(right, withRespectTo) && left is IrExactScalar d)
                    return SolveSquare(left, right, withRespectTo, d.Value);
            }
            else if (relation == IrRelationKind.GreaterOrEqual)
            {
                if (left is IrSymbol lower && Same(lower, withRespectTo)
                    && right is IrExactScalar min)
                    return Admit(SymbolicRealSolvingStatus.LowerClosedRay,
                        left, right, withRespectTo, min.Value);
                if (right is IrSymbol upper && Same(upper, withRespectTo)
                    && left is IrExactScalar max)
                    return Admit(SymbolicRealSolvingStatus.UpperClosedRay,
                        left, right, withRespectTo, max.Value);
                if (SquareOf(left, withRespectTo) && right is IrExactScalar bound)
                    return bound.Value <= ExactRational.Zero
                        ? Admit(SymbolicRealSolvingStatus.AllReal,
                            left, right, withRespectTo)
                        : Refuse(SymbolicRealSolvingStatus.Unknown);
                if (SquareOf(right, withRespectTo) && left is IrExactScalar limit)
                    return limit.Value < ExactRational.Zero
                        ? Admit(SymbolicRealSolvingStatus.Empty,
                            left, right, withRespectTo)
                        : limit.Value == ExactRational.Zero
                            ? Admit(SymbolicRealSolvingStatus.Finite,
                                left, right, withRespectTo,
                                roots: [ExactRational.Zero])
                            : Refuse(SymbolicRealSolvingStatus.Unknown);
            }
            return Refuse(SymbolicRealSolvingStatus.Unknown);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(SymbolicRealSolvingStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is ArgumentException or
            FormatException or NotSupportedException)
        {
            return Refuse(SymbolicRealSolvingStatus.Unsupported);
        }
    }

    private static SymbolicRealSolvingResult SolveSquare(
        IrNode left, IrNode right, IrSymbol variable, ExactRational c)
    {
        if (c < ExactRational.Zero)
            return Admit(SymbolicRealSolvingStatus.Empty,left,right,variable);
        if (c == ExactRational.Zero)
            return Admit(SymbolicRealSolvingStatus.Finite,left,right,variable,
                roots: [ExactRational.Zero]);
        if (!PerfectSquare(c.Numerator,out var n) ||
            !PerfectSquare(c.Denominator,out var d))
            return Refuse(SymbolicRealSolvingStatus.Unknown);
        var positive = new ExactRational(n,d);
        return Admit(SymbolicRealSolvingStatus.Finite,left,right,variable,
            roots: [new ExactRational(-n,d),positive]);
    }

    private static bool PerfectSquare(BigInteger nonnegative,out BigInteger root)
    {
        root = BigInteger.Zero;
        if (nonnegative.Sign < 0)
            return false;
        var low = BigInteger.Zero;
        var high = BigInteger.One << (int)((nonnegative.GetBitLength()+1)/2);
        while (low <= high)
        {
            var mid = (low+high) >> 1;
            var square = mid*mid;
            if (square == nonnegative)
            {
                root = mid;
                return true;
            }
            if (square < nonnegative)
                low = mid+BigInteger.One;
            else
                high = mid-BigInteger.One;
        }
        return false;
    }

    private static bool SquareOf(IrNode node,IrSymbol variable) =>
        node is IrApply {Operation:IrOperation.Square} square
            && square.Arguments[0] is IrSymbol s && Same(s,variable)
        || node is IrApply {Operation:IrOperation.Power} power
            && power.Arguments[0] is IrSymbol p && Same(p,variable)
            && power.Arguments[1] is IrExactScalar exponent
            && exponent.Value == new ExactRational(2,1);

    private static bool Same(IrSymbol a,IrSymbol b) =>
        !a.IsBound && !b.IsBound && a.Domain==IrScalarDomain.Real
        && b.Domain==IrScalarDomain.Real && a.Identity==b.Identity;

    private static bool Bounded(ExactRational value) =>
        BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits
        && value.Denominator.GetBitLength()<=MaximumRationalBits;

    private static bool RelationHolds(
        ExactRational left,IrRelationKind relation,ExactRational right) =>
        relation == IrRelationKind.Equal ? left == right : left >= right;

    private static SymbolicRealSolvingResult Admit(
        SymbolicRealSolvingStatus status,IrNode left,IrNode right,
        IrSymbol variable,ExactRational? bound=null,
        ExactRational[]? roots=null) =>
        new(status,(roots??[]).ToImmutableArray(),bound,Rule,
            IrCanonicalHashes.SemanticStructuralSha256(left),
            IrCanonicalHashes.SemanticStructuralSha256(right),variable.Identity);

    private static SymbolicRealSolvingResult Refuse(SymbolicRealSolvingStatus status) =>
        new(status,ImmutableArray<ExactRational>.Empty,null,null,null,null,null);
}
