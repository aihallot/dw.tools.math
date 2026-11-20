# Generic bounded integer sampling — M9-W01-C01

Status: **source candidate pending RS268 validation**. The currently published Statistics package `20260.20261106.1001` does **not** contain this API. No new candidate has been packed or published.

`SplitMix64Random` remains the single versioned generator. `splitmix64/v1` raw outputs, `SplitMix64Position` and its canonical codec are unchanged. It is noncryptographic.

Proposed public members:
- `SplitMix64Random.BoundedTransformId = "splitmix64-bounded-rejection/v1"`.
- `ulong NextBoundedUInt64(ulong exclusiveUpperBound, int maxRawDraws)`: output `[0, exclusiveUpperBound)`; bound and budget must be positive.
- `long NextInt64Inclusive(long lowerInclusive, long upperInclusive, int maxRawDraws)`: inclusive, overflow-safe signed range. The full `Int64` range is a special `2^64`-wide case consuming one raw word.

For a bound `n`, the rejection threshold is `(2^64 mod n)`, computed as `unchecked(0UL - n) % n`. Every raw word below the threshold is discarded, **but consumes a PRNG step and a unit of the declared budget**. Each accepted residue has precisely `floor(2^64 / n)` preimages, assuming uniform input words. The algorithm does **not** claim that SplitMix64 is cryptographically secure or perfectly statistically random.

Invalid bounds and budgets refuse before consuming words. A budget exhausted without an accepted word raises `InvalidOperationException`, after consuming at most the specified number of raw words. The existing PRNG's exhausted `ulong` counter refuses independently. Existing snapshots capture every consumed raw word, including rejections. Range selection uses signed-order `Int64` bit encoding to avoid width overflow.

RS268 checks: independent seed-zero raw reference words, mathematical exhaustive 8-bit rejection oracle, forced rejection/budget, range edges, checkpoint replay and existing SplitMix64 regression tests. This is **source-level evidence only** until durable RS268 success. A new immutable package candidate and .NET 11 package-only consumer are separate follow-up gates; no Assessa pedagogical contract enters Math.
