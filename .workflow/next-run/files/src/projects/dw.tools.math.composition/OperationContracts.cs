using System.Collections.Immutable;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Composition;

/// <summary>Whether a completed computation is exact, approximate, or has no final value.</summary>
public enum ComputationStatus { Exact, Approximate, Unsupported, BudgetExceeded }
public enum MathValueKind { Scalar, Matrix }
public enum MathPrecisionKind { Exact, Approximate }

/// <summary>A bounded, provider-neutral description of an admitted IR value.</summary>
public sealed record MathValueDescriptor
{
    private MathValueDescriptor(MathValueKind kind, MathPrecisionKind precision, int rows, int columns)
    {
        Kind = kind;
        Precision = precision;
        Rows = rows;
        Columns = columns;
    }

    public MathValueKind Kind { get; }
    public MathPrecisionKind Precision { get; }
    public int Rows { get; }
    public int Columns { get; }

    public static MathValueDescriptor Inspect(IrNode value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value switch
        {
            IrExactScalar => new(MathValueKind.Scalar, MathPrecisionKind.Exact, 1, 1),
            IrFiniteBinary64Scalar => new(MathValueKind.Scalar, MathPrecisionKind.Approximate, 1, 1),
            IrMatrix matrix when matrix.Elements[0] is IrExactScalar =>
                new(MathValueKind.Matrix, MathPrecisionKind.Exact, matrix.Rows, matrix.Columns),
            IrMatrix matrix when matrix.Elements[0] is IrFiniteBinary64Scalar =>
                new(MathValueKind.Matrix, MathPrecisionKind.Approximate, matrix.Rows, matrix.Columns),
            _ => throw new NotSupportedException("No inspectable numerical result shape for this IR node.")
        };
    }
}

/// <summary>Discoverability is only a capability description, never permission or execution.</summary>
public sealed record MathOperationCapability
{
    private MathOperationCapability(string operationId, ImmutableArray<MathValueKind> inputKinds,
        int maximumMatrixElements, bool supportsExact, bool supportsApproximate)
    {
        OperationId = operationId;
        AcceptedInputKinds = inputKinds;
        MaximumMatrixElements = maximumMatrixElements;
        SupportsExact = supportsExact;
        SupportsApproximate = supportsApproximate;
    }

    public string OperationId { get; }
    public ImmutableArray<MathValueKind> AcceptedInputKinds { get; }
    public int MaximumMatrixElements { get; }
    public bool SupportsExact { get; }
    public bool SupportsApproximate { get; }

    public static MathOperationCapability MatrixInspection { get; } =
        new("matrix.inspect", ImmutableArray.Create(MathValueKind.Matrix), 4096, true, true);

    public bool Accepts(IrNode? input)
    {
        if (input is null) return false;
        MathValueDescriptor descriptor;
        try { descriptor = MathValueDescriptor.Inspect(input); }
        catch (NotSupportedException) { return false; }
        return AcceptedInputKinds.Contains(descriptor.Kind)
            && (descriptor.Kind != MathValueKind.Matrix ||
                (long)descriptor.Rows * descriptor.Columns <= MaximumMatrixElements)
            && (descriptor.Precision == MathPrecisionKind.Exact ? SupportsExact : SupportsApproximate);
    }
}

/// <summary>Explicit versioned provenance, without binding any provider or AURA permissions.</summary>
public sealed record MathResultProvenance
{
    private MathResultProvenance(string operationId, string contractVersion)
    {
        OperationId = operationId;
        ContractVersion = contractVersion;
    }

    public string OperationId { get; }
    public string ContractVersion { get; }

    public static MathResultProvenance Create(string operationId, string contractVersion) =>
        new(Validate(operationId, nameof(operationId)),
            Validate(contractVersion, nameof(contractVersion)));

    private static string Validate(string value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        if (value.Length > 128) throw new ArgumentOutOfRangeException(name);
        return value;
    }
}

/// <summary>A final result exists only for exact or approximate success.</summary>
public sealed record MathOperationResult
{
    private MathOperationResult(ComputationStatus status, IrNode? value, string? reason,
        MathResultProvenance provenance)
    {
        Status = status;
        Value = value;
        Reason = reason;
        Provenance = provenance;
    }

    public ComputationStatus Status { get; }
    public IrNode? Value { get; }
    public string? Reason { get; }
    public MathResultProvenance Provenance { get; }
    public bool HasFinalValue => Value is not null;

    public static MathOperationResult Exact(IrNode value, MathResultProvenance provenance) =>
        Successful(ComputationStatus.Exact, MathPrecisionKind.Exact, value, provenance);

    public static MathOperationResult Approximate(IrNode value, MathResultProvenance provenance) =>
        Successful(ComputationStatus.Approximate, MathPrecisionKind.Approximate, value, provenance);

    public static MathOperationResult Unsupported(string reason, MathResultProvenance provenance) =>
        Failure(ComputationStatus.Unsupported, reason, provenance);

    public static MathOperationResult BudgetExceeded(string reason, MathResultProvenance provenance) =>
        Failure(ComputationStatus.BudgetExceeded, reason, provenance);

    private static MathOperationResult Successful(ComputationStatus status,
        MathPrecisionKind expected, IrNode value, MathResultProvenance provenance)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(provenance);
        if (MathValueDescriptor.Inspect(value).Precision != expected)
            throw new ArgumentException("Result precision must match its explicit status.", nameof(value));
        return new(status, value, null, provenance);
    }

    private static MathOperationResult Failure(ComputationStatus status, string reason,
        MathResultProvenance provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (reason.Length > 256) throw new ArgumentOutOfRangeException(nameof(reason));
        return new(status, null, reason, provenance);
    }
}
