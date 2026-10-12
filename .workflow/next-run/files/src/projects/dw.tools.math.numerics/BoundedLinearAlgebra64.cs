using System.Collections.Immutable;

namespace Dw.Tools.Math.Numerics;

/// <summary>All numerical outcomes are finite-binary64 only; no exact-solve claim.</summary>
public enum LinearAlgebraStatus64
{
    Success,
    Singular,
    RankDeficient,
    BudgetExceeded,
    NonFiniteIntermediate
}

/// <summary>LU factors satisfy P*A=L*U for the reported original-row permutation.</summary>
public sealed record LuFactorization64(
    LinearAlgebraStatus64 Status,
    FiniteMatrix64? Lower,
    FiniteMatrix64? Upper,
    ImmutableArray<int> RowPermutation,
    double PivotThreshold,
    double? MinimumAbsolutePivot)
{
    public bool Factored => Status == LinearAlgebraStatus64.Success &&
        Lower is not null && Upper is not null;
}

/// <summary>A successful solve reports actual absolute/relative residuals.</summary>
public sealed record LinearSolveResult64(
    LinearAlgebraStatus64 Status,
    FiniteVector64? Solution,
    double? AbsoluteResidualInfinity,
    double? RelativeResidualInfinity,
    double PivotThreshold)
{
    public bool HasUniqueSolution => Status == LinearAlgebraStatus64.Success &&
        Solution is not null;
}

/// <summary>Full-Q QR: Q is m*m, R is m*n, Q*R approximates A.</summary>
public sealed record QrFactorization64(
    LinearAlgebraStatus64 Status,
    FiniteMatrix64? Q,
    FiniteMatrix64? R,
    double RankThreshold)
{
    public bool Factored => Status == LinearAlgebraStatus64.Success &&
        Q is not null && R is not null;
}

/// <summary>2x2 singular-value spectrum only; no U/V factors or complete SVD claim.</summary>
public sealed record SingularSpectrum2x2Result64(
    LinearAlgebraStatus64 Status,
    double? LargestSingularValue,
    double? SmallestSingularValue,
    double? ConditionRatio);

/// <summary>
/// Explicitly bounded provider-neutral dense algorithms. Managed Math.NET is
/// an independent test oracle, not a dependency of this product assembly.
/// </summary>
public static class BoundedLinearAlgebra64
{
    public const int MaximumDimension = 16;
    public const double DefaultRelativeThreshold = 1e-12;

    public static LuFactorization64 FactorLu(
        FiniteMatrix64 matrix,
        double relativePivotThreshold = DefaultRelativeThreshold)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        RequireThreshold(relativePivotThreshold);
        if (matrix.Rows != matrix.Columns)
            throw new ArgumentException("LU factorization requires a square matrix.", nameof(matrix));
        var n = matrix.Rows;
        if (n > MaximumDimension)
            return new(LinearAlgebraStatus64.BudgetExceeded, null, null,
                ImmutableArray<int>.Empty, 0d, null);

        var lu = matrix.ToProviderArrayCopy();
        var originalRows = Enumerable.Range(0, n).ToArray();
        var maximum = MaxAbs(lu);
        var threshold = maximum * relativePivotThreshold;
        var minimumPivot = double.PositiveInfinity;

        for (var column = 0; column < n; column++)
        {
            var pivotRow = column;
            var pivotMagnitude = global::System.Math.Abs(lu[column, column]);
            for (var row = column + 1; row < n; row++)
            {
                var magnitude = global::System.Math.Abs(lu[row, column]);
                if (magnitude > pivotMagnitude)
                {
                    pivotMagnitude = magnitude;
                    pivotRow = row;
                }
            }
            if (pivotMagnitude <= threshold)
                return new(LinearAlgebraStatus64.Singular, null, null,
                    ImmutableArray<int>.Empty, threshold, pivotMagnitude);

            if (pivotRow != column)
            {
                for (var j = 0; j < n; j++)
                    (lu[column, j], lu[pivotRow, j]) = (lu[pivotRow, j], lu[column, j]);
                (originalRows[column], originalRows[pivotRow]) =
                    (originalRows[pivotRow], originalRows[column]);
            }

            var pivot = lu[column, column];
            minimumPivot = global::System.Math.Min(minimumPivot, global::System.Math.Abs(pivot));
            for (var row = column + 1; row < n; row++)
            {
                var multiplier = lu[row, column] / pivot;
                if (!double.IsFinite(multiplier))
                    return LuFailure(LinearAlgebraStatus64.NonFiniteIntermediate, threshold);
                lu[row, column] = multiplier;
                for (var j = column + 1; j < n; j++)
                {
                    var updated = lu[row, j] - multiplier * lu[column, j];
                    if (!double.IsFinite(updated))
                        return LuFailure(LinearAlgebraStatus64.NonFiniteIntermediate, threshold);
                    lu[row, j] = updated;
                }
            }
        }

        var lower = new double[n * n];
        var upper = new double[n * n];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
            {
                if (i == j) lower[i * n + j] = 1d;
                else if (i > j) lower[i * n + j] = lu[i, j];
                if (j >= i) upper[i * n + j] = lu[i, j];
            }

        return new(LinearAlgebraStatus64.Success,
            FiniteMatrix64.Create(n, n, lower),
            FiniteMatrix64.Create(n, n, upper),
            originalRows.ToImmutableArray(), threshold, minimumPivot);
    }

    public static LinearSolveResult64 Solve(
        FiniteMatrix64 matrix,
        FiniteVector64 right,
        double relativePivotThreshold = DefaultRelativeThreshold)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(right);
        RequireThreshold(relativePivotThreshold);
        if (matrix.Rows != matrix.Columns || right.Length != matrix.Rows)
            throw new ArgumentException("Unique dense solve requires square A and matching RHS.");
        var factors = FactorLu(matrix, relativePivotThreshold);
        if (!factors.Factored)
            return new(factors.Status, null, null, null, factors.PivotThreshold);

        var n = matrix.Rows;
        var l = factors.Lower!;
        var u = factors.Upper!;
        var y = new double[n];
        var x = new double[n];

        for (var i = 0; i < n; i++)
        {
            var value = right.At(factors.RowPermutation[i]);
            for (var j = 0; j < i; j++)
                value -= l.At(i, j) * y[j];
            if (!double.IsFinite(value))
                return SolveFailure(factors.PivotThreshold);
            y[i] = value;
        }
        for (var i = n - 1; i >= 0; i--)
        {
            var value = y[i];
            for (var j = i + 1; j < n; j++)
                value -= u.At(i, j) * x[j];
            x[i] = value / u.At(i, i);
            if (!double.IsFinite(x[i]))
                return SolveFailure(factors.PivotThreshold);
        }

        var residual = 0d;
        var matrixMax = 0d;
        var rightMax = 0d;
        var xMax = 0d;
        foreach (var value in x) xMax = global::System.Math.Max(xMax, global::System.Math.Abs(value));
        for (var row = 0; row < n; row++)
        {
            var sum = 0d;
            rightMax = global::System.Math.Max(rightMax, global::System.Math.Abs(right.At(row)));
            for (var column = 0; column < n; column++)
            {
                matrixMax = global::System.Math.Max(matrixMax, global::System.Math.Abs(matrix.At(row, column)));
                sum += matrix.At(row, column) * x[column];
            }
            if (!double.IsFinite(sum))
                return SolveFailure(factors.PivotThreshold);
            var error = global::System.Math.Abs(sum - right.At(row));
            if (!double.IsFinite(error))
                return SolveFailure(factors.PivotThreshold);
            residual = global::System.Math.Max(residual, error);
        }
        // A relative residual is optional when a binary64 normalizer would
        // overflow or underflow; never manufacture a finite conditioning claim.
        var normalizer = matrixMax * xMax + rightMax;
        double? relative = double.IsFinite(normalizer) && normalizer > 0d
            ? residual / normalizer
            : null;
        return new(LinearAlgebraStatus64.Success, FiniteVector64.Create(x),
            residual, relative, factors.PivotThreshold);
    }

    public static QrFactorization64 FactorQr(
        FiniteMatrix64 matrix,
        double relativeRankThreshold = DefaultRelativeThreshold)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        RequireThreshold(relativeRankThreshold);
        var rows = matrix.Rows;
        var columns = matrix.Columns;
        if (rows < columns)
            throw new ArgumentException("Unpivoted full-Q factorization requires rows >= columns.", nameof(matrix));
        if (rows > MaximumDimension || columns > MaximumDimension)
            return new(LinearAlgebraStatus64.BudgetExceeded, null, null, 0d);

        var r = matrix.ToProviderArrayCopy();
        var q = new double[rows, rows];
        for (var i = 0; i < rows; i++) q[i, i] = 1d;
        var threshold = MaxAbs(r) * relativeRankThreshold;

        for (var k = 0; k < columns; k++)
        {
            var maxInColumn = 0d;
            for (var i = k; i < rows; i++)
                maxInColumn = global::System.Math.Max(maxInColumn, global::System.Math.Abs(r[i, k]));
            if (maxInColumn <= threshold)
                return new(LinearAlgebraStatus64.RankDeficient, null, null, threshold);

            var sumSquares = 0d;
            for (var i = k; i < rows; i++)
            {
                var scaled = r[i, k] / maxInColumn;
                sumSquares += scaled * scaled;
            }
            var norm = maxInColumn * global::System.Math.Sqrt(sumSquares);
            if (!double.IsFinite(norm) || norm == 0d)
                return QrFailure(threshold);

            // Normalize the Householder vector relative to its column norm.
            // v[0] is chosen away from zero to avoid cancellation.
            var v = new double[rows - k];
            for (var i = k; i < rows; i++) v[i - k] = r[i, k] / norm;
            v[0] += v[0] >= 0d ? 1d : -1d;
            var vSquared = 0d;
            foreach (var entry in v) vSquared += entry * entry;
            var beta = 2d / vSquared;

            for (var j = k; j < columns; j++)
            {
                var dot = 0d;
                for (var i = k; i < rows; i++)
                    dot += v[i - k] * r[i, j];
                var scale = beta * dot;
                for (var i = k; i < rows; i++)
                {
                    r[i, j] -= scale * v[i - k];
                    if (!double.IsFinite(r[i, j])) return QrFailure(threshold);
                }
            }
            for (var i = k + 1; i < rows; i++) r[i, k] = 0d;

            // Q <- Q*H; H is symmetric and orthogonal.
            for (var i = 0; i < rows; i++)
            {
                var dot = 0d;
                for (var j = k; j < rows; j++) dot += q[i, j] * v[j - k];
                var scale = beta * dot;
                for (var j = k; j < rows; j++)
                {
                    q[i, j] -= scale * v[j - k];
                    if (!double.IsFinite(q[i, j])) return QrFailure(threshold);
                }
            }
        }
        var flatQ = new double[rows * rows];
        var flatR = new double[rows * columns];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < rows; j++) flatQ[i * rows + j] = q[i, j];
            for (var j = 0; j < columns; j++) flatR[i * columns + j] = r[i, j];
        }
        return new(LinearAlgebraStatus64.Success,
            FiniteMatrix64.Create(rows, rows, flatQ),
            FiniteMatrix64.Create(rows, columns, flatR), threshold);
    }

    /// <summary>
    /// Analytical 2x2 singular-value spectrum only; full SVD vectors are
    /// intentionally deferred pending a separate numerical admission.
    /// </summary>
    public static SingularSpectrum2x2Result64 SingularValues2x2(FiniteMatrix64 matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        if (matrix.Rows != 2 || matrix.Columns != 2)
            throw new NotSupportedException("Only the 2x2 singular-value spectrum is admitted.");
        var a = matrix.At(0, 0);
        var b = matrix.At(0, 1);
        var c = matrix.At(1, 0);
        var d = matrix.At(1, 1);
        var first = HypotFinite(a + d, c - b);
        var second = HypotFinite(a - d, c + b);
        if (!first.HasValue || !second.HasValue)
            return new(LinearAlgebraStatus64.NonFiniteIntermediate, null, null, null);
        var largest = 0.5d * first.Value + 0.5d * second.Value;
        var smallest = 0.5d * global::System.Math.Abs(first.Value - second.Value);
        if (!double.IsFinite(largest) || !double.IsFinite(smallest))
            return new(LinearAlgebraStatus64.NonFiniteIntermediate, null, null, null);
        var ratio = smallest > 0d && double.IsFinite(largest / smallest)
            ? largest / smallest : (double?)null;
        return new(LinearAlgebraStatus64.Success, largest, smallest, ratio);
    }

    private static LuFactorization64 LuFailure(
        LinearAlgebraStatus64 status, double threshold) =>
        new(status, null, null, ImmutableArray<int>.Empty, threshold, null);

    private static LinearSolveResult64 SolveFailure(double threshold) =>
        new(LinearAlgebraStatus64.NonFiniteIntermediate, null, null, null, threshold);

    private static QrFactorization64 QrFailure(double threshold) =>
        new(LinearAlgebraStatus64.NonFiniteIntermediate, null, null, threshold);

    private static double? HypotFinite(double a, double b)
    {
        if (!double.IsFinite(a) || !double.IsFinite(b)) return null;
        var scale = global::System.Math.Max(global::System.Math.Abs(a), global::System.Math.Abs(b));
        if (scale == 0d) return 0d;
        var x = a / scale;
        var y = b / scale;
        var value = scale * global::System.Math.Sqrt(x * x + y * y);
        return double.IsFinite(value) ? value : null;
    }

    private static double MaxAbs(double[,] array)
    {
        var best = 0d;
        foreach (var entry in array) best = global::System.Math.Max(best, global::System.Math.Abs(entry));
        return best;
    }

    private static void RequireThreshold(double relative)
    {
        if (!double.IsFinite(relative) || relative <= 0d || relative > 1e-6)
            throw new ArgumentOutOfRangeException(nameof(relative),
                "Relative pivot/rank tolerance must be finite and in (0, 1e-6].");
    }
}
