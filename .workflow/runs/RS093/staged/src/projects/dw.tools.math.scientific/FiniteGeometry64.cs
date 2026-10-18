using System;

namespace Dw.Tools.Math.Scientific;

/// <summary>Fail-closed statuses for bounded Cartesian Euclidean coordinates.</summary>
public enum FiniteGeometryStatus64
{
    Success, InvalidDimension, InvalidInput, ZeroAxis, OutsideQualifiedRange,
    NonFiniteResult, Cancelled
}

/// <summary>
/// A scalar norm/distance or vector projection, never a partial result.
/// Accepted values are approximate binary64; no numerical error theorem is implied.
/// </summary>
public sealed record FiniteGeometryOutcome64(
    FiniteGeometryStatus64 Status, double? Scalar, double[]? Coordinates,
    string RuleId, bool ErrorBoundGuaranteed)
{
    public bool Succeeded
    {
        get
        {
            if (Status != FiniteGeometryStatus64.Success || ErrorBoundGuaranteed)
                return false;
            if (Scalar is { } scalar)
                return double.IsFinite(scalar) && Coordinates is null;
            if (Coordinates is not { Length: 2 or 3 } coordinates)
                return false;
            foreach (var coordinate in coordinates)
                if (!double.IsFinite(coordinate)) return false;
            return true;
        }
    }
}

/// <summary>
/// Dimensionless-in-the-API, caller-supplied Cartesian 2D/3D Euclidean operations.
/// Coordinates must all use the same caller-managed linear unit and frame.
/// This is not geodesic geometry; no unit conversion or exact error bound is provided.
/// </summary>
public static class FiniteGeometry64
{
    public const string RuleId = "dw-math-finite-euclidean-1";
    public const double MaxAbsCoordinate = 1e100d;
    public const double MinNonzeroAxisMagnitude = 1e-100d;

    public static FiniteGeometryOutcome64 Norm(
        double[]? vector, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
        var status = Validate(vector);
        if (status != FiniteGeometryStatus64.Success) return Refuse(status);
        var result = Magnitude(vector!);
        return Scalar(result);
    }

    public static FiniteGeometryOutcome64 Distance(
        double[]? start, double[]? end, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
        var status = Validate(start);
        if (status != FiniteGeometryStatus64.Success) return Refuse(status);
        status = Validate(end);
        if (status != FiniteGeometryStatus64.Success) return Refuse(status);
        if (start!.Length != end!.Length) return Refuse(FiniteGeometryStatus64.InvalidDimension);
        var delta = new double[start.Length];
        for (var i = 0; i < delta.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
            delta[i] = end[i] - start[i];
        }
        return Scalar(Magnitude(delta));
    }

    public static FiniteGeometryOutcome64 Project(
        double[]? vector, double[]? axis, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
        var status = Validate(vector);
        if (status != FiniteGeometryStatus64.Success) return Refuse(status);
        status = Validate(axis);
        if (status != FiniteGeometryStatus64.Success) return Refuse(status);
        if (vector!.Length != axis!.Length) return Refuse(FiniteGeometryStatus64.InvalidDimension);

        var scale = 0d;
        foreach (var component in axis)
            scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
        if (scale == 0d) return Refuse(FiniteGeometryStatus64.ZeroAxis);
        if (scale < MinNonzeroAxisMagnitude)
            return Refuse(FiniteGeometryStatus64.OutsideQualifiedRange);

        var normalizedAxis = new double[axis.Length];
        var denominator = 0d;
        var numerator = 0d;
        for (var i = 0; i < axis.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
            normalizedAxis[i] = axis[i] / scale;
            denominator += normalizedAxis[i] * normalizedAxis[i];
            numerator += vector[i] * normalizedAxis[i];
        }
        if (!double.IsFinite(numerator) || !double.IsFinite(denominator) || denominator <= 0d)
            return Refuse(FiniteGeometryStatus64.NonFiniteResult);
        var coefficient = numerator / denominator;
        if (!double.IsFinite(coefficient)) return Refuse(FiniteGeometryStatus64.NonFiniteResult);
        var projected = new double[axis.Length];
        for (var i = 0; i < projected.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested) return Refuse(FiniteGeometryStatus64.Cancelled);
            projected[i] = coefficient * normalizedAxis[i];
            if (!double.IsFinite(projected[i])) return Refuse(FiniteGeometryStatus64.NonFiniteResult);
            if (global::System.Math.Abs(projected[i]) > MaxAbsCoordinate)
                return Refuse(FiniteGeometryStatus64.OutsideQualifiedRange);
        }
        return new(FiniteGeometryStatus64.Success, null, projected, RuleId, false);
    }

    private static FiniteGeometryStatus64 Validate(double[]? vector)
    {
        if (vector is not { Length: 2 or 3 }) return FiniteGeometryStatus64.InvalidDimension;
        foreach (var component in vector)
        {
            if (!double.IsFinite(component)) return FiniteGeometryStatus64.InvalidInput;
            if (global::System.Math.Abs(component) > MaxAbsCoordinate)
                return FiniteGeometryStatus64.OutsideQualifiedRange;
        }
        return FiniteGeometryStatus64.Success;
    }

    private static double Magnitude(double[] vector)
    {
        var scale = 0d;
        foreach (var component in vector)
            scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
        if (scale == 0d) return 0d;
        var squares = 0d;
        foreach (var component in vector)
        {
            var normalized = component / scale;
            squares += normalized * normalized;
        }
        return scale * global::System.Math.Sqrt(squares);
    }

    private static FiniteGeometryOutcome64 Scalar(double result)
    {
        if (!double.IsFinite(result)) return Refuse(FiniteGeometryStatus64.NonFiniteResult);
        if (result > MaxAbsCoordinate) return Refuse(FiniteGeometryStatus64.OutsideQualifiedRange);
        return new(FiniteGeometryStatus64.Success, result, null, RuleId, false);
    }

    private static FiniteGeometryOutcome64 Refuse(FiniteGeometryStatus64 status) =>
        new(status, null, null, RuleId, false);
}
