# Generic bounded integer sampling — M9-W01-C01

Status: **published privately and independently qualified in RS273**. The old immutable Statistics package `20260.20261106.1001` does **not** contain this API. The new `dw.tools.math.statistics 20260.20261120.1001` does; Assessa-owned adoption is pending.

`SplitMix64Random` remains the single versioned generator. `splitmix64/v1` raw outputs, `SplitMix64Position` and its canonical codec are unchanged. It is noncryptographic.

Published private package public members:
- `SplitMix64Random.BoundedTransformId = "splitmix64-bounded-rejection/v1"`.
- `ulong NextBoundedUInt64(ulong exclusiveUpperBound, int maxRawDraws)`: output `[0, exclusiveUpperBound)`; bound and budget must be positive.
- `long NextInt64Inclusive(long lowerInclusive, long upperInclusive, int maxRawDraws)`: inclusive, overflow-safe signed range. The full `Int64` range is a special `2^64`-wide case consuming one raw word.

For a bound `n`, the rejection threshold is `(2^64 mod n)`, computed as `unchecked(0UL - n) % n`. Every raw word below the threshold is discarded, **but consumes a PRNG step and a unit of the declared budget**. Each accepted residue has precisely `floor(2^64 / n)` preimages, assuming uniform input words. The algorithm does **not** claim that SplitMix64 is cryptographically secure or perfectly statistically random.

Invalid bounds and budgets refuse before consuming words. A budget exhausted without an accepted word raises `InvalidOperationException`, after consuming at most the specified number of raw words. The existing PRNG's exhausted `ulong` counter refuses independently. Existing snapshots capture every consumed raw word, including rejections. Range selection uses signed-order `Int64` bit encoding to avoid width overflow.

Qualified chronology: RS268 source API, RS269 candidate, RS270 independent locked net11 consumer, RS271 private release readiness, RS272 guarded disposable-feed publisher rehearsal and RS273 physical NUGET_LOCAL published readback. Published archive SHA-256 `feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1`; DLL SHA-256 `684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea`. Exact dependency `dw.quantities [20260.20261102.1001]` with archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`. Independent Windows .NET 11 SDK 11.0.100, source-free locked PackageReference consumer passed twelve checks. No cryptographic or Assessa pedagogical claims are made.
