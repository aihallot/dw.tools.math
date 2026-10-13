using System;

namespace Dw.Tools.Math.Statistics;

/// <summary>
/// Portable checkpoint for the exact versioned generator and draw index.
/// A checkpoint identifies the next draw, not the last emitted value.
/// </summary>
public readonly record struct SplitMix64Position(string Algorithm, ulong Seed, ulong Counter);

/// <summary>
/// Deterministic SplitMix64 with explicit seed and unsigned draw counter.
/// Arithmetic is modulo 2^64. This is not a cryptographic generator.
/// </summary>
public sealed class SplitMix64Random
{
    public const string AlgorithmId = "splitmix64/v1";
    private const ulong Increment = 0x9E3779B97F4A7C15UL;
    private const ulong Mix1 = 0xBF58476D1CE4E5B9UL;
    private const ulong Mix2 = 0x94D049BB133111EBUL;
    private const double Unit53 = 1d / 9007199254740992d;

    private ulong _state;

    /// <summary>
    /// counter is the number of 64-bit draws already consumed. All seeds,
    /// including zero, are valid; the counter cannot overflow.
    /// </summary>
    public SplitMix64Random(ulong seed, ulong counter = 0UL)
    {
        Seed = seed;
        Counter = counter;
        _state = unchecked(seed + Increment * counter);
    }

    public ulong Seed { get; }

    /// <summary>Number of consumed 64-bit values since the seed.</summary>
    public ulong Counter { get; private set; }

    public SplitMix64Position Snapshot() => new(AlgorithmId, Seed, Counter);

    public static SplitMix64Random FromSnapshot(SplitMix64Position snapshot)
    {
        if (!string.Equals(snapshot.Algorithm, AlgorithmId, StringComparison.Ordinal))
            throw new ArgumentException("Unsupported deterministic generator algorithm.", nameof(snapshot));
        return new SplitMix64Random(snapshot.Seed, snapshot.Counter);
    }

    /// <summary>Advance once and return one exact 64-bit generator output.</summary>
    public ulong NextUInt64()
    {
        if (Counter == ulong.MaxValue)
            throw new InvalidOperationException("The versioned PRNG draw counter is exhausted.");

        var z = _state = unchecked(_state + Increment);
        z = unchecked((z ^ (z >> 30)) * Mix1);
        z = unchecked((z ^ (z >> 27)) * Mix2);
        Counter++;
        return z ^ (z >> 31);
    }

    /// <summary>
    /// Map the most significant 53 output bits into IEEE binary64 [0,1).
    /// This consumes exactly one 64-bit draw and never returns 1.
    /// </summary>
    public double NextUnitInterval() => (NextUInt64() >> 11) * Unit53;
}
