using System.Collections.Immutable;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Numerics;

/// <summary>Explicitly approximate numerical values. No implicit exact-rational conversion.</summary>
public enum NumericalPrecisionKind { FiniteBinary64 }

/// <summary>Bounded, immutable row-major finite binary64 matrix; provider-neutral.</summary>
public sealed class FiniteMatrix64
{
    public const int MaximumElements = 4096;
    public const int MaximumScalarProducts = 65536;
    private readonly ImmutableArray<double> _values;

    private FiniteMatrix64(int rows, int columns, ImmutableArray<double> values)
    {
        Rows = rows;
        Columns = columns;
        _values = values;
    }

    public int Rows { get; }
    public int Columns { get; }
    public NumericalPrecisionKind Precision => NumericalPrecisionKind.FiniteBinary64;

    public static FiniteMatrix64 Create(int rows, int columns, IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        ValidateShape(rows, columns);
        var count = checked(rows * columns);
        var entries = values.Take(count + 1).ToImmutableArray();
        if (entries.Length != count)
            throw new ArgumentException("The number of matrix entries must match its shape.", nameof(values));
        foreach (var number in entries)
            RequireFinite(number);
        return new(rows, columns, entries);
    }

    public static FiniteMatrix64 FromProviderArrayCopy(double[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var rows = values.GetLength(0);
        var columns = values.GetLength(1);
        ValidateShape(rows, columns);
        var copy = new double[checked(rows * columns)];
        for (var row = 0; row < rows; row++)
            for (var column = 0; column < columns; column++)
                copy[row * columns + column] = values[row, column];
        return Create(rows, columns, copy);
    }

    public double[,] ToProviderArrayCopy()
    {
        var copy = new double[Rows, Columns];
        for (var row = 0; row < Rows; row++)
            for (var column = 0; column < Columns; column++)
                copy[row, column] = At(row, column);
        return copy;
    }

    public double At(int row, int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        if (row >= Rows || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(row), "Matrix index is outside the admitted shape.");
        return _values[checked(row * Columns + column)];
    }

    public FiniteMatrix64 Add(FiniteMatrix64 other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Rows != other.Rows || Columns != other.Columns)
            throw new ArgumentException("Matrix addition requires identical dimensions.", nameof(other));
        var sum = new double[_values.Length];
        for (var i = 0; i < sum.Length; i++)
            sum[i] = RequireFinite(_values[i] + other._values[i]);
        return Create(Rows, Columns, sum);
    }

    public FiniteMatrix64 Multiply(FiniteMatrix64 other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Columns != other.Rows)
            throw new ArgumentException("Matrix multiplication requires compatible inner dimensions.", nameof(other));
        ValidateShape(Rows, other.Columns);
        var operations = (long)Rows * Columns * other.Columns;
        if (operations > MaximumScalarProducts)
            throw new ArgumentOutOfRangeException(nameof(other), "Matrix multiplication work budget exceeded.");
        var result = new double[checked(Rows * other.Columns)];
        for (var row = 0; row < Rows; row++)
            for (var column = 0; column < other.Columns; column++)
            {
                var sum = 0d;
                for (var k = 0; k < Columns; k++)
                {
                    var term = RequireFinite(At(row, k) * other.At(k, column));
                    sum = RequireFinite(sum + term);
                }
                result[row * other.Columns + column] = sum;
            }
        return Create(Rows, other.Columns, result);
    }

    public FiniteVector64 Multiply(FiniteVector64 vector)
    {
        ArgumentNullException.ThrowIfNull(vector);
        if (Columns != vector.Length)
            throw new ArgumentException("Matrix-vector multiplication requires a compatible vector length.", nameof(vector));
        var result = new double[Rows];
        for (var row = 0; row < Rows; row++)
        {
            var sum = 0d;
            for (var k = 0; k < Columns; k++)
                sum = RequireFinite(sum + RequireFinite(At(row, k) * vector.At(k)));
            result[row] = sum;
        }
        return FiniteVector64.Create(result);
    }

    internal static void ValidateShape(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0 || (long)rows * columns > MaximumElements)
            throw new ArgumentOutOfRangeException(nameof(rows), "Only positive bounded matrices up to 4096 cells are admitted.");
    }

    internal static double RequireFinite(double value)
    {
        if (!double.IsFinite(value))
            throw new ArithmeticException("Nonfinite binary64 input or intermediate result is not admitted.");
        return value;
    }
}

/// <summary>Bounded immutable vector, separate from exact quantities and binary transport.</summary>
public sealed class FiniteVector64
{
    private readonly ImmutableArray<double> _values;
    private FiniteVector64(ImmutableArray<double> values) => _values = values;

    public int Length => _values.Length;
    public NumericalPrecisionKind Precision => NumericalPrecisionKind.FiniteBinary64;

    public static FiniteVector64 Create(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var copy = values.Take(FiniteMatrix64.MaximumElements + 1).ToImmutableArray();
        if (copy.Length is < 1 or > FiniteMatrix64.MaximumElements)
            throw new ArgumentOutOfRangeException(nameof(values), "Vector length must be 1..4096.");
        foreach (var value in copy)
            FiniteMatrix64.RequireFinite(value);
        return new(copy);
    }

    public double At(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        if (index >= Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        return _values[index];
    }

    public double Dot(FiniteVector64 other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other.Length != Length)
            throw new ArgumentException("Dot product requires equal lengths.", nameof(other));
        var result = 0d;
        for (var i = 0; i < Length; i++)
            result = FiniteMatrix64.RequireFinite(result +
                FiniteMatrix64.RequireFinite(At(i) * other.At(i)));
        return result;
    }
}

/// <summary>Admitted sparse storage: sorted, unique, nonzero coordinates; no implicit provider.</summary>
public sealed record SparseEntry64(int Row, int Column, double Value);

public sealed class SparseMatrix64
{
    private SparseMatrix64(int rows, int columns, ImmutableArray<SparseEntry64> entries)
    {
        Rows = rows;
        Columns = columns;
        Entries = entries;
    }

    public int Rows { get; }
    public int Columns { get; }
    public int NonZeroCount => Entries.Length;
    public ImmutableArray<SparseEntry64> Entries { get; }

    public static SparseMatrix64 Create(int rows, int columns, IEnumerable<SparseEntry64> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        FiniteMatrix64.ValidateShape(rows, columns);
        var copied = entries.Take(FiniteMatrix64.MaximumElements + 1).ToArray();
        if (copied.Length > FiniteMatrix64.MaximumElements)
            throw new ArgumentOutOfRangeException(nameof(entries), "Sparse entry budget exceeded.");
        var used = new HashSet<(int, int)>();
        foreach (var entry in copied)
        {
            if (entry is null || entry.Row < 0 || entry.Column < 0 ||
                entry.Row >= rows || entry.Column >= columns)
                throw new ArgumentException("Invalid or out-of-range sparse coordinate.", nameof(entries));
            FiniteMatrix64.RequireFinite(entry.Value);
            if (entry.Value == 0 || !used.Add((entry.Row, entry.Column)))
                throw new ArgumentException("Sparse coordinates must be unique and nonzero.", nameof(entries));
        }
        return new(rows, columns, copied.OrderBy(e=>e.Row).ThenBy(e=>e.Column).ToImmutableArray());
    }

    public FiniteMatrix64 ToDense()
    {
        var values = new double[checked(Rows * Columns)];
        foreach (var entry in Entries)
            values[entry.Row * Columns + entry.Column] = entry.Value;
        return FiniteMatrix64.Create(Rows, Columns, values);
    }
}

/// <summary>
/// Lossless within the admitted finite binary64 IR subset. Exact IR requires
/// a separate explicit rational-to-binary64 policy, never silent widening.
/// </summary>
public static class NumericalMatrixTransport
{
    public static IrMatrix ToApproximateIr(FiniteMatrix64 matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var data = matrix.ToProviderArrayCopy();
        var nodes = new List<IrScalar>(checked(matrix.Rows * matrix.Columns));
        for (var row = 0; row < matrix.Rows; row++)
            for (var col = 0; col < matrix.Columns; col++)
                nodes.Add(IrFiniteBinary64Scalar.FromDouble(data[row, col]));
        return IrMatrix.Create(matrix.Rows, matrix.Columns, nodes);
    }

    public static FiniteMatrix64 FromApproximateIr(IrMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        if (matrix.Elements.Any(node => node is not IrFiniteBinary64Scalar))
            throw new NotSupportedException("No implicit conversion from exact rational IR to binary64.");
        return FiniteMatrix64.Create(matrix.Rows, matrix.Columns,
            matrix.Elements.Cast<IrFiniteBinary64Scalar>().Select(node => node.Value));
    }
}
