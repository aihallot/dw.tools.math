# MATH-XR-004 — Private bounded Statistics package handoff to Assessa

Status: **Math-owned package published and independently qualified; Assessa adoption pending**.
Consumer: `aihallot/dw.tools.assessa`, `GENERATE-001 / EG-A-01 / T002`. No Assessa repository edits are performed here.

## Exact published private identities

| Identity | Exact value |
| --- | --- |
| Package ID | `dw.tools.math.statistics` |
| Version | `20260.20261120.1001` |
| Feed | `NUGET_LOCAL` (private; no public registry publication) |
| Package TFM | `net10.0` |
| Physical archive SHA-256 | `feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1` |
| Packaged DLL SHA-256 | `684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea` |
| Dependency | `dw.quantities [20260.20261102.1001]` |
| Dependency archive SHA-256 | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` |
| Independent runtime | Windows net11.0 / C#15 / SDK 11.0.100; locked NuGet PackageReference; 12 binary checks |

The prior published `dw.tools.math.statistics 20260.20261106.1001` is immutable and **does not** contain the new API. No version overwrite or repeat publication is authorized.

## Versioned Math API

```csharp
var rng = new SplitMix64Random(seed);
ulong value = rng.NextBoundedUInt64(exclusiveUpperBound, maxRawDraws);
long signedValue = rng.NextInt64Inclusive(lowerInclusive, upperInclusive, maxRawDraws);
var checkpoint = SplitMix64PositionCodec.Format(rng.Snapshot());
```

- Raw PRNG: `SplitMix64Random.AlgorithmId = "splitmix64/v1"`, original golden vectors and replay checkpoints unchanged.
- Bounded transform: `SplitMix64Random.BoundedTransformId = "splitmix64-bounded-rejection/v1"`.
- Exclusive bound `n > 0` yields `[0,n)`; inclusive signed interval accepts `lo <= hi` and the full 64-bit signed domain without overflow.
- Exact rejection threshold `(2^64 mod n)`; accepted residues have equal preimage counts assuming uniform raw 64-bit inputs. No cryptographic claim.
- Positive `maxRawDraws` includes **rejected** words; invalid arguments refuse before consuming; budget/counter exhaustion refuses with `InvalidOperationException`. Every raw rejection advances the shared snapshot counter.
- `SplitMix64PositionCodec` checkpoint replay remains versioned, deterministic and rejects incompatible algorithm identifiers.

## Durable qualification provenance

| Run | Verified responsibility |
| --- | --- |
| RS268 | Source API and exact bounded rejection oracles |
| RS269 | New unpublished NuGet candidate |
| RS270 | Independent package-only locked net11 consumer |
| RS271 | Private publication readiness, first-pass |
| RS272 | Authorized guarded publisher rehearsal, disposable feed |
| RS273 | **Actual NUGET_LOCAL physical readback and independent net11 consumer, first-pass** |

RS273 report: `.aura/workflow/reports/RS273/attempt-001.json`; `succeeded`, attempt `001`, `priorInvocationFailures.count=0`, `firstPassSucceeded=true`.
RS273 commit: `f31003c94f722cb0b15f1f4d4697002c3d36e788`.
Reader source: `docs/engineering/ProbeM9PublishedStatisticsReadback.cs`, `docs/engineering/VerifyM9BoundedStatisticsPrivateFeed.cs`, fixture `docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs`.

## Assessa-owned acceptance gate

Pin the real version via private `PackageReference` and independently verify the selected DLL and lockfile. Reuse generic unbiased bounded selection in Assessa's `fraction-part-of-whole` and `fraction-compare` recipes. Consumer-specific bounds: F0 denominator 2–4, reference integer <=24; F2 denominator 2–24, LCM <=144. These are **Assessa limits**, not Math API limits. Never silently widen them. Assessa owns recipes, item generation, profiles, answer protection, sessions, identifiers, receipts and EG-A-01/T002 acceptance.

Use existing `ExactRational` for fraction operations only after separately verifying the relevant published dependency and zero-denominator contracts; the Statistics readback does not attest this distinct surface.

Math's M9 backlog and native plan states remain unchanged (planned/not-ready). No retroactive task completion is inferred from the completed runs. Only the Assessa owner may claim GENERATE-001 / EG-A-01 / T002 successful adoption.
