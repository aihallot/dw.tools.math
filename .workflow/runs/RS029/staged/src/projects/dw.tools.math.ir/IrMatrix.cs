using System.Collections.Immutable;

namespace Dw.Tools.Math.Ir;

/// <summary>
/// Homogeneous, rectangular, provider-neutral scalar matrix.
/// Exact and approximate elements cannot be silently mixed.
/// </summary>
public sealed record IrMatrix : IrNode
{
    private IrMatrix(int rows, int columns, ImmutableArray<IrScalar> elements)
    {
        Rows = rows;
        Columns = columns;
        Elements = elements;
    }

    public int Rows { get; }
    public int Columns { get; }
    public ImmutableArray<IrScalar> Elements { get; }

    public static IrMatrix Create(int rows, int columns, IEnumerable<IrScalar> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (rows <= 0 || columns <= 0 || (long)rows * columns > IrGraphLimits.MaximumMatrixElements)
            throw new ArgumentOutOfRangeException(nameof(rows),
                "Matrix shape must have 1..4096 positive rectangular cells.");
        var size = checked(rows * columns);
        var values = elements.Take(size + 1).ToImmutableArray();
        if (values.Length != size || values.Any(x => x is null))
            throw new ArgumentException("Matrix entries must match the declared shape.", nameof(elements));
        var firstKind = values[0].GetType();
        if (values.Any(x => x.GetType() != firstKind))
            throw new ArgumentException("No implicit exact/approximate matrix widening.", nameof(elements));
        var matrix = new IrMatrix(rows, columns, values);
        IrGraphLimits.Validate(matrix);
        return matrix;
    }

    public IrScalar At(int row, int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        if (row >= Rows || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(row));
        return Elements[checked(row * Columns + column)];
    }
}
