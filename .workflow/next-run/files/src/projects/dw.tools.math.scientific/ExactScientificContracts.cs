using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>The admitted exact calculation succeeded or a typed nonpartial refusal occurred.</summary>
public enum ExactScientificStatus
{
    Success, InvalidInput, InvalidPolicy, BudgetExceeded, ResultTooLarge, Cancelled
}

/// <summary>
/// Bounds exact arbitrary-size integer work to finite input, result and iteration budgets.
/// Defaults are finite. No probabilistic primality or decision semantics are included.
/// </summary>
public sealed record ExactScientificBudget(
    int MaximumOperandBits = 128,
    int MaximumResultBits = 4096,
    int MaximumSteps = 4096);

/// <summary>An admitted exact BigInteger or a typed refusal with no partial number.</summary>
public sealed record ExactScientificResult(
    ExactScientificStatus Status, BigInteger? Value, int Steps, string RuleId)
{
    public bool Succeeded =>
        Status == ExactScientificStatus.Success &&
        Value is { } && Steps >= 0;
}
