using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using dw.quantities;

namespace Dw.Tools.Math.Composition;

/// <summary>Explicit versions for replay identity, not provider discovery or authority.</summary>
public sealed record ExactReplayContext
{
    private ExactReplayContext(string assumptions, string policy, string catalog,
        string tolerance, string provider)
    {
        Assumptions = assumptions;
        Policy = policy;
        Catalog = catalog;
        Tolerance = tolerance;
        Provider = provider;
    }

    public string Assumptions { get; }
    public string Policy { get; }
    public string Catalog { get; }
    public string Tolerance { get; }
    public string Provider { get; }

    public static ExactReplayContext Create(string assumptions, string policy, string catalog,
        string tolerance, string provider) =>
        new(Validate(assumptions, nameof(assumptions)),
            Validate(policy, nameof(policy)), Validate(catalog, nameof(catalog)),
            Validate(tolerance, nameof(tolerance)), Validate(provider, nameof(provider)));

    private static string Validate(string value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        if (value.Length > 128 || value.Any(char.IsControl))
            throw new ArgumentOutOfRangeException(name, "Replay labels must be bounded printable text.");
        return value;
    }
}

/// <summary>Domain-separated deterministic SHA-256 identity of a closed exact replay request.</summary>
public sealed record ExactReplayKey
{
    private ExactReplayKey(string digest) => Digest = digest;
    public const string Scheme = "exact-quantity-replay/1-sha256";
    public string Digest { get; }

    internal static ExactReplayKey Compute(ImmutableArray<ExactPipelineStep> steps, ExactReplayContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (steps.IsDefaultOrEmpty || steps.Length > ExactQuantityPipeline.MaximumSteps)
            throw new ArgumentException("Replay requires 1..16 ordered steps.", nameof(steps));
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            Frame(writer, Scheme);
            Frame(writer, context.Assumptions);
            Frame(writer, context.Policy);
            Frame(writer, context.Catalog);
            Frame(writer, context.Tolerance);
            Frame(writer, context.Provider);
            writer.Write(steps.Length);
            foreach (var step in steps)
            {
                if (step is null) throw new ArgumentException("Null replay step.", nameof(steps));
                writer.Write((int)step.Kind);
                var unit = step.Unit;
                if (unit is null) throw new ArgumentException("Null replay unit.", nameof(steps));
                Frame(writer, unit.Id);
                writer.Write((int)unit.System);
                writer.Write((int)unit.TransformKind);
                foreach (var d in new[] {
                    unit.Dimension.Length, unit.Dimension.Mass, unit.Dimension.Time,
                    unit.Dimension.ElectricCurrent, unit.Dimension.Temperature,
                    unit.Dimension.AmountOfSubstance, unit.Dimension.LuminousIntensity,
                    unit.Dimension.Information })
                    writer.Write(d);
                Rational(writer, unit.ScaleToBase);
                Rational(writer, unit.OffsetToBase);
                Rational(writer, step.Amount);
            }
        }
        return new(Convert.ToHexString(SHA256.HashData(stream.ToArray())).ToLowerInvariant());
    }

    private static void Rational(BinaryWriter writer, ExactRational value)
    {
        var numerator = value.Numerator.ToString(CultureInfo.InvariantCulture);
        var denominator = value.Denominator.ToString(CultureInfo.InvariantCulture);
        if (numerator.TrimStart('-').Length > ExactQuantityPipeline.MaximumNumeralDigits ||
            denominator.Length > ExactQuantityPipeline.MaximumNumeralDigits)
            throw new ArgumentOutOfRangeException(nameof(value), "Replay numeral exceeds 256 digits.");
        Frame(writer, numerator);
        Frame(writer, denominator);
    }

    private static void Frame(BinaryWriter writer, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length > 128) throw new ArgumentOutOfRangeException(nameof(value));
        var bytes = Encoding.UTF8.GetBytes(value);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }
}

/// <summary>Caller-owned, volatile and bounded first-in-first-out cache of successful local results.</summary>
public sealed class ExactReplayCache
{
    public const int MaximumEntries = 32;
    private readonly Dictionary<string, ExactPipelineResult> _results = new(StringComparer.Ordinal);
    private readonly Queue<string> _insertionOrder = new();
    private readonly object _gate = new();

    public ExactReplayCache(int capacity = 16)
    {
        if (capacity is < 1 or > MaximumEntries)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity = capacity;
    }

    public int Capacity { get; }

    public int Count
    {
        get { lock (_gate) return _results.Count; }
    }

    internal bool TryRead(ExactReplayKey key, out ExactPipelineResult? result)
    {
        lock (_gate) return _results.TryGetValue(key.Digest, out result);
    }

    internal void Store(ExactReplayKey key, ExactPipelineResult result)
    {
        lock (_gate)
        {
            if (_results.ContainsKey(key.Digest)) return;
            if (_results.Count == Capacity)
                _results.Remove(_insertionOrder.Dequeue());
            _results.Add(key.Digest, result);
            _insertionOrder.Enqueue(key.Digest);
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _results.Clear();
            _insertionOrder.Clear();
        }
    }
}

/// <summary>Explicit local replay receipt; a cache hit is not provider execution.</summary>
public sealed record ExactReplayReceipt
{
    internal ExactReplayReceipt(ExactReplayKey key, ExactPipelineResult result, bool cacheHit)
    {
        Key = key;
        Result = result;
        CacheHit = cacheHit;
    }

    public ExactReplayKey Key { get; }
    public ExactPipelineResult Result { get; }
    public bool CacheHit { get; }
}

public static class ExactReplayRunner
{
    /// <summary>Identical requests replay locally under identical context, with optional bounded cache.</summary>
    public static ExactReplayReceipt Run(IEnumerable<ExactPipelineStep> steps,
        ExactReplayContext context, ExactReplayCache? cache = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(steps);
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var finite = steps.Take(ExactQuantityPipeline.MaximumSteps + 1).ToImmutableArray();
        cancellationToken.ThrowIfCancellationRequested();
        var key = ExactReplayKey.Compute(finite, context);
        if (cache is not null && cache.TryRead(key, out var hit))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new ExactReplayReceipt(key, hit!, cacheHit: true);
        }
        var computed = ExactQuantityPipeline.Evaluate(finite, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        cache?.Store(key, computed);
        return new ExactReplayReceipt(key, computed, cacheHit: false);
    }
}
