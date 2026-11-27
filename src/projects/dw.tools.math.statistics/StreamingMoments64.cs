namespace Dw.Tools.Math.Statistics;

/// <summary>Finite approximate moments; empty results have no numeric values.</summary>
public sealed record StreamingMomentsResult64(
    int Count, double? Mean, double? PopulationVariance, double? SampleVariance)
{
    public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
}

/// <summary>Versioned structural checkpoint for same-order, same-runtime replay.</summary>
public sealed record StreamingMomentsSnapshot64(
    string Algorithm, int MaxObservations, int Count, double Origin, double MeanOffset, double M2);

/// <summary>
/// Bounded unweighted streaming moments using a fixed first-observation origin
/// and Welford updates of the centered values. The state has constant size.
/// Instances are mutable and not thread-safe.
/// </summary>
public sealed class StreamingMoments64
{
    public const string AlgorithmId = "streaming-moments-shifted-welford/v1";
    public const int MaximumObservations = 1_000_000;

    private readonly int maxObservations;
    private int count;
    private double origin;
    private double meanOffset;
    private double m2;

    public StreamingMoments64(int maxObservations)
    {
        if (maxObservations is < 1 or > MaximumObservations)
            throw new ArgumentOutOfRangeException(nameof(maxObservations));
        this.maxObservations = maxObservations;
    }

    /// <summary>Admits one finite observation or leaves the state unchanged on refusal.</summary>
    public void Add(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "A finite observation is required.");
        if (count == maxObservations)
            throw new InvalidOperationException("The observation budget is exhausted.");

        if (count == 0)
        {
            origin = value;
            count = 1;
            return;
        }

        int nextCount = count + 1;
        double centered = DescriptiveStatistics64.RequireFinite(value - origin);
        double delta = DescriptiveStatistics64.RequireFinite(centered - meanOffset);
        double nextOffset = DescriptiveStatistics64.RequireFinite(meanOffset + delta / nextCount);
        double residual = DescriptiveStatistics64.RequireFinite(centered - nextOffset);
        double increment = DescriptiveStatistics64.RequireFinite(delta * residual);
        double nextM2 = DescriptiveStatistics64.RequireFinite(m2 + increment);
        if (nextM2 < 0d)
            throw new ArithmeticException("A negative centered second moment is not admitted.");

        // Validate the reconstructed public result before committing any fields.
        _ = DescriptiveStatistics64.RequireFinite(origin + nextOffset);
        meanOffset = nextOffset;
        m2 = nextM2 == 0d ? 0d : nextM2;
        count = nextCount;
    }

    public StreamingMomentsResult64 GetResult()
    {
        if (count == 0)
            return new(0, null, null, null);
        double mean = count == 1 ? origin : DescriptiveStatistics64.RequireFinite(origin + meanOffset);
        return new(count, mean, m2 / count, count > 1 ? m2 / (count - 1) : null);
    }

    public StreamingMomentsSnapshot64 Snapshot() =>
        new(AlgorithmId, maxObservations, count, origin, meanOffset, m2);

    /// <summary>Restores structurally valid state, not proof of historical sample provenance.</summary>
    public static StreamingMoments64 FromSnapshot(StreamingMomentsSnapshot64 snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (!string.Equals(snapshot.Algorithm, AlgorithmId, StringComparison.Ordinal) ||
            snapshot.MaxObservations is < 1 or > MaximumObservations ||
            snapshot.Count < 0 || snapshot.Count > snapshot.MaxObservations ||
            !double.IsFinite(snapshot.Origin) || !double.IsFinite(snapshot.MeanOffset) ||
            !double.IsFinite(snapshot.M2) || snapshot.M2 < 0d ||
            !double.IsFinite(snapshot.Origin + snapshot.MeanOffset) ||
            (snapshot.Count == 0 && (!IsPositiveZero(snapshot.Origin) ||
                !IsPositiveZero(snapshot.MeanOffset) || !IsPositiveZero(snapshot.M2))) ||
            (snapshot.Count == 1 && (!IsPositiveZero(snapshot.MeanOffset) || !IsPositiveZero(snapshot.M2))))
            throw new ArgumentException("Invalid streaming moments checkpoint.", nameof(snapshot));

        return new StreamingMoments64(snapshot.MaxObservations)
        {
            count = snapshot.Count,
            origin = snapshot.Origin,
            meanOffset = snapshot.MeanOffset,
            m2 = snapshot.M2
        };
    }

    private static bool IsPositiveZero(double value) => BitConverter.DoubleToInt64Bits(value) == 0L;
}
