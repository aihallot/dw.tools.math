using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Outcome of one closed, exact IR rational-cancellation contract.</summary>
public enum SymbolicCancellationStatus
{
    Admitted,
    Unsupported,
    ContradictoryAssumptions,
    BudgetExceeded
}

/// <summary>A result is only admitted with an explicit, immutable IR restriction.</summary>
public sealed class SymbolicCancellationResult
{
    internal SymbolicCancellationResult(
        SymbolicCancellationStatus status, IrRestrictedExpression? conditional)
    {
        Status = status;
        Conditional = conditional;
    }

    public SymbolicCancellationStatus Status { get; }
    public IrRestrictedExpression? Conditional { get; }
    public bool IsAdmitted => Status == SymbolicCancellationStatus.Admitted &&
        Conditional is not null;
}

public enum SymbolicPointStatus
{
    ExactValue,
    Excluded,
    Unsupported
}

/// <summary>No exact number is supplied for a refused or unqualified point.</summary>
public sealed record SymbolicPointResult(SymbolicPointStatus Status, ExactRational? Value)
{
    public bool HasExactValue => Status == SymbolicPointStatus.ExactValue &&
        Value.HasValue;
}

/// <summary>
/// Provider-independent, closed IR bridge for (x²-c²)/(x-c) = x+c with x!=c.
/// This is a hand-provable structural rule, NOT a general CAS simplifier or
/// a parser for AngouriMath's human-readable "provided" output.
/// </summary>
public static class SymbolicRationalCancellationBridge
{
    public const int MaximumConstantOperandBits = 128;

    /// <summary>
    /// Admit only a single free real symbol and an exact rational c,
    /// producing a typed IrRestrictedExpression with the removable pole
    /// explicitly excluded. Other input forms have no mathematical claim.
    /// </summary>
    public static SymbolicCancellationResult TryCancel(IrNode original)
    {
        ArgumentNullException.ThrowIfNull(original);
        IrGraphLimits.Validate(original);
        var underlying = original is IrRestrictedExpression restricted
            ? restricted.Expression : original;
        if (underlying is not IrApply { Operation: IrOperation.Divide } quotient ||
            quotient.Arguments[0] is not IrApply { Operation: IrOperation.Subtract } numerator ||
            quotient.Arguments[1] is not IrApply { Operation: IrOperation.Subtract } denominator ||
            denominator.Arguments[0] is not IrSymbol x ||
            x.IsBound || x.Domain != IrScalarDomain.Real ||
            denominator.Arguments[1] is not IrExactScalar offset ||
            numerator.Arguments[1] is not IrExactScalar squareConstant ||
            !IsSquareOf(numerator.Arguments[0], x))
            return new(SymbolicCancellationStatus.Unsupported, null);

        var c = offset.Value;
        if (BigInteger.Abs(c.Numerator).GetBitLength() > MaximumConstantOperandBits ||
            c.Denominator.GetBitLength() > MaximumConstantOperandBits)
            return new(SymbolicCancellationStatus.BudgetExceeded, null);
        if (squareConstant.Value != c * c)
            return new(SymbolicCancellationStatus.Unsupported, null);

        var previous = original is IrRestrictedExpression wrapper
            ? wrapper.Assumptions.Conditions.ToArray()
            : Array.Empty<IrRelation>();
        if (previous.Any(condition => !SameSymbol(condition.Symbol, x)))
            return new(SymbolicCancellationStatus.Unsupported, null);
        var equalities = previous.Where(condition => condition.Kind == IrRelationKind.Equal)
            .Select(condition => condition.Right.Value).Distinct().ToArray();
        if (equalities.Length > 1)
            return new(SymbolicCancellationStatus.ContradictoryAssumptions, null);
        if (equalities.Length == 1 &&
            (equalities[0] == c ||
             previous.Any(condition =>
                 condition.Kind == IrRelationKind.NotEqual &&
                 condition.Right.Value == equalities[0]) ||
             previous.Any(condition =>
                 condition.Kind == IrRelationKind.GreaterOrEqual &&
                 equalities[0] < condition.Right.Value)))
            return new(SymbolicCancellationStatus.ContradictoryAssumptions, null);

        var constraints = previous.ToList();
        if (!constraints.Any(condition =>
            condition.Kind == IrRelationKind.NotEqual && condition.Right.Value == c))
            constraints.Add(new IrRelation(x, IrRelationKind.NotEqual, new IrExactScalar(c)));
        if (constraints.Count > 32)
            return new(SymbolicCancellationStatus.BudgetExceeded, null);

        var reduced = IrApply.Create(IrOperation.Add, [x, new IrExactScalar(c)]);
        var typed = new IrRestrictedExpression(reduced, IrAssumptionSet.Create(constraints));
        // Force the previously qualified public IR codec's restriction checks
        // before admitting this new product API's typed result.
        if (IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(typed))
            is not IrRestrictedExpression)
            throw new InvalidOperationException("A typed IR restriction was not round-trippable.");
        return new(SymbolicCancellationStatus.Admitted, typed);
    }

    /// <summary>Evaluate only a bridge-generated exact result, enforcing every retained condition.</summary>
    public static SymbolicPointResult EvaluateExactAt(
        SymbolicCancellationResult result, IrSymbol variable, ExactRational point)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(variable);
        var conditional = result.Conditional;
        if (!result.IsAdmitted || conditional is null ||
            conditional.Expression is not IrApply { Operation: IrOperation.Add } sum ||
            sum.Arguments.Length != 2 ||
            sum.Arguments[0] is not IrSymbol expressionVariable ||
            sum.Arguments[1] is not IrExactScalar offset ||
            !SameSymbol(expressionVariable, variable))
            return new(SymbolicPointStatus.Unsupported, null);

        foreach (var condition in conditional.Assumptions.Conditions)
        {
            if (!SameSymbol(condition.Symbol, variable))
                return new(SymbolicPointStatus.Unsupported, null);
            var threshold = condition.Right.Value;
            var accepted = condition.Kind switch
            {
                IrRelationKind.NotEqual => point != threshold,
                IrRelationKind.Equal => point == threshold,
                IrRelationKind.GreaterOrEqual => point >= threshold,
                _ => false
            };
            if (!accepted)
                return new(SymbolicPointStatus.Excluded, null);
        }
        return new(SymbolicPointStatus.ExactValue, point + offset.Value);
    }

    private static bool IsSquareOf(IrNode node, IrSymbol symbol)
    {
        if (node is not IrApply square)
            return false;
        if (square.Operation == IrOperation.Square &&
            square.Arguments.Length == 1 &&
            square.Arguments[0] is IrSymbol direct)
            return SameSymbol(direct, symbol);
        if (square.Operation == IrOperation.Power &&
            square.Arguments.Length == 2 &&
            square.Arguments[0] is IrSymbol baseSymbol &&
            square.Arguments[1] is IrExactScalar exponent &&
            exponent.Value == new ExactRational(2, 1))
            return SameSymbol(baseSymbol, symbol);
        return false;
    }

    private static bool SameSymbol(IrSymbol left, IrSymbol right) =>
        left.Identity == right.Identity && left.Domain == right.Domain &&
        !left.IsBound && !right.IsBound;
}
