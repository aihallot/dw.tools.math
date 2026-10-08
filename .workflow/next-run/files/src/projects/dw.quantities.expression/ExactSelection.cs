namespace dw.quantities.expression;

/// <summary>
/// Preserves the first input on exact ties, with its zero-based source index.
/// Values must share physical dimension and temperature semantics.
/// </summary>
public readonly record struct ExpressionSelectionResult(int Index, EvaluatedQuantity Quantity);

public static class ExactSelection
{
    public static ExpressionSelectionResult Minimum(IReadOnlyList<EvaluatedQuantity> operands) =>
        Select(operands, isMinimum: true);

    public static ExpressionSelectionResult Maximum(IReadOnlyList<EvaluatedQuantity> operands) =>
        Select(operands, isMinimum: false);

    private static ExpressionSelectionResult Select(
        IReadOnlyList<EvaluatedQuantity> operands, bool isMinimum)
    {
        ArgumentNullException.ThrowIfNull(operands);
        if (operands.Count < 2)
            throw new ArgumentException("Selection requires at least two operands.", nameof(operands));
        var first = operands[0];
        var selectedIndex = 0;
        for (var i = 1; i < operands.Count; i++)
        {
            var next = operands[i];
            if (next.Dimension != first.Dimension)
                throw new ArgumentException("Selection operands must have identical dimensions.", nameof(operands));
            if (next.Temperature != first.Temperature)
                throw new ArgumentException("Selection operands must have identical temperature semantics.", nameof(operands));
            var comparison = next.Value.CompareTo(operands[selectedIndex].Value);
            if (isMinimum ? comparison < 0 : comparison > 0)
                selectedIndex = i;
        }
        return new ExpressionSelectionResult(selectedIndex, operands[selectedIndex]);
    }
}
